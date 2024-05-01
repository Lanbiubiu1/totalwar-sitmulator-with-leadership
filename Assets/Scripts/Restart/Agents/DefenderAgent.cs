using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Unity Window > Package Manager to install the ML Agent package
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Sensors; //https://docs.unity3d.com/Packages/com.unity.ml-agents%401.0/api/Unity.MLAgents.Sensors.html
using Unity.MLAgents.Actuators; //https://docs.unity3d.com/Packages/com.unity.ml-agents@2.0/api/Unity.MLAgents.Actuators.html
using Unity.MLAgents.Policies; //https://docs.unity3d.com/Packages/com.unity.ml-agents@2.0/api/Unity.MLAgents.Policies.html
using NoOpArmy.WiseFeline.InfluenceMaps;
using Unity.VisualScripting;
using NetTopologySuite.Algorithm;



public class DefenderAgent : Agent
{
    // Public variable to reference the army, which presumably includes various units like infantry, cavalry, and archers.
    public ArmyNew army;

    

    // add all the ims installed
    public InfluenceMapComponent im;// use this to retrive values and locations, see InfluenceMapComponent script for reference

    public override void Initialize()
    {
        var array = im.Map.GetCellsArray();
        Debug.Log((array.GetLength(0)).ToString());
        Debug.Log((array.GetLength(1)).ToString());
        /*        //initialize the im
                im = GameObject.Find("Map").GetComponent<InfluenceMapComponent>();

                if (im == null)
                {
                    Debug.LogError("InfluenceMapComponent not found on the object named 'Map'.");
                }*/

    }
    
        

    

    // Update is called once per frame.
    private void Update()
    {
        
        // If the game is not currently playing, adjust the Brain's VectorObservationSize based on the army's unit counts.
        // This dynamically changes the input size for the neural network based on the composition of the army.
        if (!Application.isPlaying)
            GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize =
                5 * (army.infantryStats.Count + army.cavalryStats.Count) +
                6 * (army.archersStats.Count) +
                3;
    }

    // Override the OnEpisodeBegin method from the Agent class. Used for resetting the environment at the start of training episodes.
    public override void OnEpisodeBegin()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Add observations to the sensor for melee units (e.g., infantry and cavalry) including their ID and position/direction as Vector2.
    private void AddMeleeInformation(VectorSensor sensor, UnitNew u)
    {
        //add ideal destination 3 observation
        Vector3 dest = getIdealDest(u);
        sensor.AddObservation(dest);


        // Add unit ID as observation.
        sensor.AddObservation(u.ID);  
        sensor.AddObservation(new Vector2(u.transform.position.x, u.transform.position.z));
        sensor.AddObservation(new Vector2(u.transform.forward.x, u.transform.forward.z));

        sensor.AddObservation(u.lost_precentage); //loss percentage
        sensor.AddObservation((float)u.state);
        sensor.AddObservation(u.morale/100.0f);
    }

    // Add observations for ranged units (archers), similar to melee but also includes the unit's range.
    private void AddRangedInformation(VectorSensor sensor, ArcherNew u)
    {
        //add ideal destination 3 observation
        Vector3 dest = getIdealDest(u);
        sensor.AddObservation(dest);

        // Add archer ID as observation.
        sensor.AddObservation(u.ID);
        
        sensor.AddObservation(new Vector2(u.transform.position.x, u.transform.position.z));
        sensor.AddObservation(new Vector2(u.transform.forward.x, u.transform.forward.z));
        //sensor.AddObservation(u.range); // Add archer range as observation.

        sensor.AddObservation(u.lost_precentage); //loss percentage
        sensor.AddObservation((float)u.state);
        sensor.AddObservation(u.morale/100.0f);
    }

    private void AddEnemyMeleeInformation(VectorSensor sensor, UnitNew u)
    { 
        sensor.AddObservation(u.ID);
        sensor.AddObservation((float)u.army.role);// 0 is the attacker and 1 is the defender
        sensor.AddObservation(new Vector2(u.transform.position.x, u.transform.position.z));
        sensor.AddObservation(new Vector2(u.transform.forward.x, u.transform.forward.z));
        sensor.AddObservation(u.morale/100.0f);
        sensor.AddObservation((float)u.state);

    }

    private void AddEnemyRangedInformation(VectorSensor sensor, ArcherNew u)
    { 
        sensor.AddObservation(u.ID);
        sensor.AddObservation((float)u.army.role);// 0 is the attacker and 1 is the defender
        sensor.AddObservation(new Vector2(u.transform.position.x, u.transform.position.z));
        sensor.AddObservation(new Vector2(u.transform.forward.x, u.transform.forward.z));
        sensor.AddObservation(u.morale/100.0f);
        sensor.AddObservation((float)u.state);

    }

    // Collect observations from the environment to be used by the neural network for making decisions.
    public override void CollectObservations(VectorSensor sensor)
    {

        // Add counts of each unit type to the observation vector.
        sensor.AddObservation(army.infantryUnits.Count);// Add infantry count.
        foreach (var i in army.infantryUnits)
            
            AddMeleeInformation(sensor, i);// 8 observations per unit
            
            


        sensor.AddObservation(army.archerUnits.Count);// Add archer count
        foreach (var a in army.archerUnits)
            
            AddRangedInformation(sensor, a);// 8 observations per unit


        sensor.AddObservation(army.cavalryUnits.Count);// Add cavalry count.
        foreach (var c in army.cavalryUnits)
            
            AddMeleeInformation(sensor, c);// 8 observations per unit



        // Add counts of each enemy unit type to the observation vector.

        // 1 observation
        sensor.AddObservation(army.enemy.infantryUnits.Count);// Add enemy infantry count.
        foreach (var e_i in army.enemy.infantryUnits)
            AddEnemyMeleeInformation(sensor, e_i);// 8 observations per unit

        // 1 observation
        sensor.AddObservation(army.enemy.archerUnits.Count);// Add enemy archer count
        foreach (var a in army.enemy.archerUnits)
           
            AddEnemyRangedInformation(sensor, a);// 8 observations per unit

        // 1 observation
        sensor.AddObservation(army.enemy.cavalryUnits.Count);// Add enemy archer count
        foreach (var c in army.enemy.cavalryUnits)
            
            AddEnemyMeleeInformation(sensor, c); // 8 observations per unit


        // IMPORTANT: Count the number of values for each observation in comment
        // For example, a vector3 contains 3 observations
        

        // add destination with highest value, reference influencemap searchhighestvaluecloesttocenter
        // this need to be per unit
        // add direction vector & distance

        // 1 observation
        // sensor.AddObservation(army.units.Count);
        //foreach (var u in army.units)


        // add all ims installed(start with the one we have for the defender team)


       

    }

    // A method for testing and debugging. It can be used to manually input actions.
    // But I do not know how to use it
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Log message
        Debug.Log("Heuristica");

    }

    // Assuming a method to find a unit by its ID exists
    private UnitNew FindUnitById(int id)
    {
        // Iterate through each unit in the units collection
        foreach (UnitNew unit in army.units)
        {
            // Check if the current unit's ID matches the provided ID
            if (unit.ID == id)
            {
                // If a match is found, return the current unit
                return unit;
            }
        }

        // If no matching unit is found, return null
        return null;
    }

    // Process actions received from the neural network. Here, a reward of 1.0f is given for any received action.
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // NEW STRUCTURE:
        // action reference page:https://docs.unity3d.com/Packages/com.unity.ml-agents@3.0/api/Unity.MLAgents.Actuators.ActionBuffers.html

        // 3 discrete action(assuming 3 units per army) for unit selection:
        //      0 -> unitid =0 and so on

        // 2 continousaction(clamped between -1 and 1): rotation & distance
        //      index0: rotation: -1 -> 0 degree
        //                        +1 -> 360 degree
        //      index1: distance: 1 -> max distance based on board size(1 * board width/height) i think we have a square board so this should work
        //                        0 -> 0
        //                penalize(negative reward) values < 0

        // psudocode for carrying out actions:

        // if (discreteaction == one of the unit id
        //      selectedunit = unit.findByUnitid
        //      if selectedunit.isMoving, do nothing //i.e wait for last movement action to complete
        //      if !selectedunit.isMoving & !selectedunit.inFight //if idle and not in fight, move and evaluate

        //          dest = calculate the destination based on selectunit.postion and continousaction[0]and [1] // add penalty(-1reward) for negative value fron contunousaction[1] here
        //          selectedunit.moveat(dest)
        //          if dest.imvalue >= threshhold(determined using maxvalue in the im, set to maxvalue/2)
        //              addreward(1f)
        //          else if dest.imvalue = maxvalue, addreward(3f)
        //      if !selectedunit.isMoving & selectedunit.inFight // combate evaluation, helper functions idea below
        //          combatEval(selectedUnit)


        // need a collision evaluation to penalize hitting boundries, the below code is from hummingbird example
        /// <summary>
        /// Called when the agent collides with something solid
        /// </summary>
        /// <param name="collision">The collision info</param>
        /*private void OnCollisionEnter(Collision collision)
        {
            if (trainingMode && collision.collider.CompareTag("boundary"))
            {
                // Collided with the area boundary, give a negative reward
                AddReward(-.5f);
            }
        }*/



        //Combat evaluation helper

        // private void combatEval(UnitNew myUnit){
        //      if myunit.health < maxhealth/2 : addreward(-3f)
        //      else addreward(1f)

        //      targettype = myunit.target.type

        ////    ignoring cases like melee vs melee for now
        //      if myunit is melee & targettype is range: addreward(-2f)for now
        //      else if myunit is calvary & targettype is range: addreward(2f)
        //      else if myunit is range & targettype is range: addreward(-0.5f)
        //}


        army.DEBUG_MODE = true;

        ActionSegment<int> discreteActions = actionBuffers.DiscreteActions;

       
        UnitNew unit = FindUnitById( discreteActions[0]);
        Vector3 newPosition;

        ActionSegment<float> continuousactions= actionBuffers.ContinuousActions;
        float rotationAction = continuousactions[0]; // Rotation action from -1 to 1
        float distanceAction = continuousactions[1]; // Distance action from -1 to 1
        if (distanceAction < 0f) AddReward(-1f);
        float width = 150f;
        float height = 150f;
        
            
        if (unit != null && unit.cunit != null)
        {

            //Debug.Log(((int)unit.morale).ToString());
            if (unit.currentMoraleState == UnitNew.MoraleState.Wavering){
                float range = 20.0f;

                newPosition = new Vector3(unit.position.x, unit.position.y, -75);
                Debug.Log("escaping");
            }
            else{
                float rotationDegrees = MapRange(rotationAction, -1f, 1f, 0f, 360f);
                float distance = MapRange(distanceAction, 0f, 1f, 0f, 150f);
                Quaternion rotation = Quaternion.Euler(0, rotationDegrees, 0);
                Vector3 direction = rotation * Vector3.forward;

                newPosition = unit.position + direction * distance;
                newPosition.y = unit.position.y;
                
            }

            newPosition.x = Mathf.Clamp(newPosition.x, -70f, 70f);
            newPosition.z = Mathf.Clamp(newPosition.z, -70f, 70f);

            //var destMapPos = im.WorldToMapPosition(newPosition);
            //float influence = im.GetCellValue(newPosition);
            //Debug.Log((influence).ToString());
            //Debug.Log((destMapPos).ToString());


            /*var destMapPos = im.WorldToMapPosition(newPosition);
            float influence = im.GetCellValue(newPosition);
            Debug.Log((influence).ToString());*/
            Vector3 newDirection = (newPosition - unit.position).normalized;
                    
                
            unit.cunit.MoveAt(newPosition, newDirection);
            if (newPosition == getIdealDest(unit))
            {
                AddReward(1f);
                Debug.Log("best");
            }
            if (im.GetCellValue(newPosition) > im.Map.MaxValue * 0.8)
            {
                AddReward(0.5f);
                //Debug.Log("ideal");
            }

            //else AddReward(-0.1f);
            /*if (unit.position != null)
            {
                AddReward(+0.2f);  // Positive reward for successful action
            }
            else
            {
                AddReward(-0.1f);
            }

            InCombate(unit);*/
                
        }

        

    }

    float MapRange(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void InCombate(UnitNew myUnit)
    {
        if (myUnit.isInFight)
        {
            SetReward(-10f);
        }
    }
    private Vector3 getIdealDest(UnitNew u)
    {
        var mapPos = im.WorldToMapPosition(u.position);
        Vector2Int targetMapPos;
        float highestValue = im.SearchForHighestValueClosestToCenter(mapPos, 10, out targetMapPos);
        Vector3 targetWorldPos = im.MapToWorldPosition(targetMapPos.x, targetMapPos.y);
        return targetWorldPos;
        //calculate rotation and distance to add to observation
    }
}
