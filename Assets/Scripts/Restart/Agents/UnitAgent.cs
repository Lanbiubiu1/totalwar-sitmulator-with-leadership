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
using System.Net;



public class UnitAgent : Agent
{
    //public static int round = 0;
    // Public variable to reference the army, which presumably includes various units like infantry, cavalry, and archers.
    public ArmyNew army;
    public UnitNew unit;
/*    public GameObject wall_left;
    public GameObject wall_right;
    public GameObject wall_behind;
    public GameObject wall_forward;*/
    public float force = 10f;
    float minX = -75;
    float maxX = 75;
    float minZ = -75;
    float maxZ = 75;
   
    // add all the ims installed
    public InfluenceMapComponent defMap;
    public InfluenceMapComponent attMap;
    public InfluenceMapComponentBase confidentMap;
    public InfluenceMapComponentBase cautionMap;
    //public InfluenceMapComponent confidentMap;
    //public InfluenceMapComponent cautionMap;// use this to retrive values and locations, see InfluenceMapComponent script for reference

    public override void Initialize()
    {
        defMap = GameObject.Find("DefenderMap").GetComponent<InfluenceMapComponent>();
        attMap = GameObject.Find("AttackerMap").GetComponent<InfluenceMapComponent>();
        confidentMap = GameObject.Find("ConfidentMap").GetComponent<InfluenceMapView>();
        cautionMap = GameObject.Find("CautionMap").GetComponent<InfluenceMapView>();
        army = this.transform.parent.GetComponent<ArmyNew>();
        unit = this.GetComponent<UnitNew>();    
    }





    // Update is called once per frame.
    private void Update()
    {

        // If the game is not currently playing, adjust the Brain's VectorObservationSize based on the army's unit counts.
        // This dynamically changes the input size for the neural network based on the composition of the army.
        /*if (Application.isPlaying)
            GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize =
                5 * (army.infantryStats.Count + army.cavalryStats.Count) +
                6 * (army.archersStats.Count) +
                3;*/
    }

    // Override the OnEpisodeBegin method from the Agent class. Used for resetting the environment at the start of training episodes.
    public override void OnEpisodeBegin()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("new ep");
    }

    //12 observation per unit
    private void AddUnitInformation(VectorSensor sensor, UnitNew u)
    {
        //var (dest, distance, signedRotation) = getIdealDest(u);
        Vector3 toEnemy = ToClosestEnemy(u);
        Vector3 motion = MotionSuggestion(u);
        sensor.AddObservation(toEnemy);//3 observation
        sensor.AddObservation(motion);  //3 observation
        sensor.AddObservation(u.transform.position);//3 observation
        sensor.AddObservation((int)u.type);//1 observation
        sensor.AddObservation((int)u.state);//1 observation
        sensor.AddObservation((int)u.currentMoraleState);//1 observation
    }

    // Collect observations from the environment to be used by the neural network for making decisions.
    //total observation = unit counts * (12) + 3
    public override void CollectObservations(VectorSensor sensor)
    {
        AddUnitInformation(sensor, unit);

    }

    // A method for testing and debugging. It can be used to manually input actions.
    // But I do not know how to use it
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Log message
        Debug.Log("Heuristica");

    }

    // Assuming a method to find a unit by its ID exists

    // Process actions received from the neural network. Here, a reward of 1.0f is given for any received action.
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 newPosition;

        // continous[0]: x-axis direction (+1 = right, -1 = left)
        // continous[1]: z-axis direction (+1 = forward, -1 = backward)
        ActionSegment<float> continuousactions = actionBuffers.ContinuousActions;
        float moveX = continuousactions[0]; // Rotation action from -1 to 1
        float moveZ = continuousactions[1]; // Distance action from -1 to 1
        //Debug.Log("unit:" + discreteActions[0].ToString() + ". direction: " + moveX.ToString() + ", " + moveZ.ToString());
        Vector3 move = new Vector3(moveX, 0, moveZ);// * force;

        float step = force * Time.deltaTime; // Moving force
        newPosition = unit.transform.position + move * force;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        //move if not wavering, otherwise escape as implemented in unit
        if (unit.currentMoraleState is not UnitNew.MoraleState.Wavering)
        {
            Debug.Log("unit" + unit.ID.ToString() + "new position: " + newPosition.ToString());
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, newPosition, step);
        }

        //Debug.Log(unit.WallCollided.ToString());
        //evaluate either way

        /*ImEval(unit, newPosition);
        PfEval(unit, newPosition);
        if (unit.isInFight)
        {
            combatEval(unit);
        }
        if (unit.WallCollided)
        {
            //Debug.Log("crashing wall penalized");
            AddReward(-0.5f);
        }*/



    }



    private (Vector3 targetPosition, float distance, float signed_rotation) getIdealDest(UnitNew u)
    {

        if (u.currentMoraleState is UnitNew.MoraleState.Confident || u.currentMoraleState is UnitNew.MoraleState.Impetuous
            || u.currentMoraleState is UnitNew.MoraleState.Eager)
        {
            var map = confidentMap;
            var mapPos = map.WorldToMapPosition(u.transform.position);
            Vector2Int targetMapPos;
            float highestValue = map.SearchForHighestValueClosestToCenter(mapPos, 150, out targetMapPos);
            Vector3 targetWorldPos = map.MapToWorldPosition(targetMapPos.x, targetMapPos.y);
            Vector3 direction = targetWorldPos - u.transform.position;
            float distance = direction.magnitude;
            float signed_rotation = Vector3.SignedAngle(u.transform.forward, direction.normalized, Vector3.up);
            return (targetWorldPos, distance, signed_rotation);
        }
        else
        {
            var map = cautionMap;
            var mapPos = map.WorldToMapPosition(u.transform.position);
            Vector2Int targetMapPos;
            float highestValue = map.SearchForHighestValueClosestToCenter(mapPos, 150, out targetMapPos);
            Vector3 targetWorldPos = map.MapToWorldPosition(targetMapPos.x, targetMapPos.y);
            //calculate rotation and distance to add to observation
            Vector3 direction = targetWorldPos - u.transform.position;
            float distance = direction.magnitude;
            float signed_rotation = Vector3.SignedAngle(u.transform.forward, direction.normalized, Vector3.up);
            return (targetWorldPos, distance, signed_rotation);
        }
    }



    private void combatEval(UnitNew unit)
    {
        /*        if (unit.lost_precentage < 0.5f) {
                    AddReward(-0.1f);
                } 
                else {AddReward(0.2f);}*/

        foreach (var enemy in unit.fightingAgainst)
        {
            if (unit.type == UnitNew.Type.Archer)
            {
                if (enemy.type == UnitNew.Type.Infantry)
                {
                    AddReward(0.2f);
                }
            }
            else if (unit.type == UnitNew.Type.Cavalry)
            {
                if (enemy.type == UnitNew.Type.Archer)
                {
                    AddReward(0.2f);
                }
            }
            else if (unit.type == UnitNew.Type.Infantry)
            {
                if (enemy.type == UnitNew.Type.Cavalry)
                {
                    AddReward(0.2f);
                }
            }

        }
    }
    ////    ignoring cases like melee vs melee for now
    //      if myunit is melee & targettype is range: addreward(-2f)for now
    //      else if myunit is calvary & targettype is range: addreward(2f)
    //      else if myunit is range & targettype is range: addreward(-0.5f)
    //}
    private void FixedUpdate()
    {
        Academy.Instance.EnvironmentStep();
    }

    private void ImEval(UnitNew u, Vector3 dest)
    {
        if (u.currentMoraleState is UnitNew.MoraleState.Confident || u.currentMoraleState is UnitNew.MoraleState.Impetuous
            || u.currentMoraleState is UnitNew.MoraleState.Eager || u.currentMoraleState is UnitNew.MoraleState.Steady)
        {
            var v = confidentMap.GetCellValue(dest);
            var (targetPos, _, _) = getIdealDest(u);
            var v2 = confidentMap.GetCellValue(targetPos);
            if (v == v2)
            {
                AddReward(1f);
                Debug.Log("best im confidence rewarded");
            }
            /*            else if (v >= (v2 * 0.8))
                        {
                            AddReward(0.2f);
                            Debug.Log("acceptable");
                        }*/
        }
        else
        {
            var v = cautionMap.GetCellValue(dest);
            var (targetPos, _, _) = getIdealDest(u);
            var v2 = cautionMap.GetCellValue(targetPos);
            if (v == v2)
            {
                AddReward(1f);
                Debug.Log("best caution im rewarded");
            }
            /*            else if (v >= (v2 * 0.8f))
                        {
                            AddReward(0.2f);
                            Debug.Log("acceptable");
                        }*/
        }
    }

    private void PfEval(UnitNew u, Vector3 dest)
    {
        if (u.currentMoraleState is UnitNew.MoraleState.Confident || u.currentMoraleState is UnitNew.MoraleState.Impetuous
            || u.currentMoraleState is UnitNew.MoraleState.Eager || u.currentMoraleState is UnitNew.MoraleState.Steady)
        {
            float newPotential = army.field.ComputePotential(dest, u);
            Vector3 dir = army.field.FindBestAttraction(u);
            Vector3 motionDest = u.transform.position + dir * force;
            motionDest.x = Mathf.Clamp(motionDest.x, minX, maxX);
            motionDest.z = Mathf.Clamp(motionDest.z, minZ, maxZ);
            if (newPotential < army.field.ComputePotential(motionDest, u))
            {
                Debug.Log("PF attraction rewarded");
                AddReward(0.1f);
            }
        }
        else
        {
            float newPotential = army.field.ComputePotential(dest, u);
            Vector3 dir = army.field.FindBestRepulsion(u);
            Vector3 motionDest = u.transform.position + dir * force;
            motionDest.x = Mathf.Clamp(motionDest.x, minX, maxX);
            motionDest.z = Mathf.Clamp(motionDest.z, minZ, maxZ);
            if (newPotential > army.field.ComputePotential(motionDest, u))
            {
                Debug.Log("PF repulsion rewarded");
                AddReward(0.1f);
            }
        }

    }

    private Vector3 MotionSuggestion(UnitNew u)
    {
        Vector3 dir;
        if (u.currentMoraleState is UnitNew.MoraleState.Confident || u.currentMoraleState is UnitNew.MoraleState.Impetuous
            || u.currentMoraleState is UnitNew.MoraleState.Eager || u.currentMoraleState is UnitNew.MoraleState.Steady)
        {
            dir = army.field.FindBestAttraction(u);
        }
        else
        {
            dir = army.field.FindBestRepulsion(u);
        }

        return dir;
    }
    private Vector3 ToClosestEnemy(UnitNew unit)
    {
        UnitNew closestEnemy = null;
        float closestDistance = float.MaxValue;
        UnitNew.Type counter = UnitNew.Type.Archer;
        if (unit.type == UnitNew.Type.Cavalry)
        {
            counter = UnitNew.Type.Archer;
        }
        if (unit.type == UnitNew.Type.Archer)
        {
            counter = UnitNew.Type.Infantry;
        }
        if (unit.type == UnitNew.Type.Infantry)
        {
            counter = UnitNew.Type.Cavalry;
        }
        foreach (var enemy in army.enemy.units)
        {

            if (enemy.type == counter)
            {
                float distance = Vector3.Distance(unit.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

        }
        Vector3 dir = closestEnemy.position - unit.transform.position;

        //Debug.Log($"Closest enemy for {unit.name} is {closestEnemy?.name ?? "None"} at distance {closestDistance}");
        return dir.normalized;
    }


}
