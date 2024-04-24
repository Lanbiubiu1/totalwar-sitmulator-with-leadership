using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Unity Window > Package Manager to install the ML Agent package
using Unity.MLAgents;
using Unity.MLAgents.Sensors; //https://docs.unity3d.com/Packages/com.unity.ml-agents%401.0/api/Unity.MLAgents.Sensors.html
using Unity.MLAgents.Actuators; //https://docs.unity3d.com/Packages/com.unity.ml-agents@2.0/api/Unity.MLAgents.Actuators.html
using Unity.MLAgents.Policies; //https://docs.unity3d.com/Packages/com.unity.ml-agents@2.0/api/Unity.MLAgents.Policies.html


public class DefenderAgent : Agent
{
    // Public variable to reference the army, which presumably includes various units like infantry, cavalry, and archers.
    public ArmyNew army;

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
    }


    // Add observations to the sensor for melee units (e.g., infantry and cavalry) including their ID and position/direction as Vector2.
    private void AddMeleeInformation(VectorSensor sensor, UnitNew u)
    {
        // Add unit ID as observation.
        sensor.AddObservation(u.ID);
        sensor.AddObservation(new Vector2(u.transform.position.x, u.transform.position.z));
        sensor.AddObservation(new Vector2(u.transform.forward.x, u.transform.forward.z));
    }

    // Add observations for ranged units (archers), similar to melee but also includes the unit's range.
    private void AddRangedInformation(VectorSensor sensor, ArcherNew u)
    {   
        // Add archer ID as observation.
        sensor.AddObservation(u.ID); 
        sensor.AddObservation(new Vector2(u.transform.position.x, u.transform.position.z));
        sensor.AddObservation(new Vector2(u.transform.forward.x, u.transform.forward.z));
        //sensor.AddObservation(u.range); // Add archer range as observation.
    }

    // Collect observations from the environment to be used by the neural network for making decisions.
    public override void CollectObservations(VectorSensor sensor)
    {

        // Add counts of each unit type to the observation vector.
        sensor.AddObservation(army.infantryUnits.Count);// Add infantry count.
        foreach (var i in army.infantryUnits)
            // Add information for each infantry unit.
            AddMeleeInformation(sensor, i);


        sensor.AddObservation(army.archerUnits.Count);// Add archer count
        foreach (var a in army.archerUnits)
            // Add information for each archer.
            AddRangedInformation(sensor, a);


        sensor.AddObservation(army.cavalryUnits.Count);// Add cavalry count.
        foreach (var c in army.cavalryUnits)
            // Add information for each cavalry unit.
            AddMeleeInformation(sensor, c);


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
        //1. set selectedunit using unit id

        //2. use selectedunit.moveat(provided function in CUnitNew.cs) to move unit to new location

        //3. set reward for the move(all positive for now maybe negative if hit walls if it's easy to implement)

        //repeat for all unit
        
        army.DEBUG_MODE = true;

        for (int i = 0; i < army.units.Count; i++){
            int actionIndex = i * 2;
            UnitNew unit = army.units[i];

            Vector3 newPosition;
            float movementRange = 100.0f;
            
            if (unit != null && unit.cunit != null)
            {

                //Debug.Log(unit.morale);
                if (unit.currentMoraleState == UnitNew.MoraleState.Wavering){
                    float range = 20.0f;
                    float posX = unit.position.x + Random.Range(-range, range);
                    float posZ = unit.position.z + Random.Range(-range, range);
                    newPosition = new Vector3(posX*movementRange, unit.position.y, posZ*movementRange);

                }
                else{
                    float posX = actionBuffers.ContinuousActions[actionIndex];
                    float posZ = actionBuffers.ContinuousActions[actionIndex+1];
                    newPosition = new Vector3(posX*movementRange, unit.position.y, posZ*movementRange);
                }

                 

                
        
                Vector3 newDirection = (newPosition - unit.position).normalized;
                    
                //Debug.Log($"Unit {i} moving to Target Position X: {newPosition.x}, Z: {newPosition.z}, Direction: {newDirection}");

                    // Move the unit to the new position and face the direction it's moving
                unit.cunit.MoveAt(newPosition, newDirection);

                if (unit.position != null)
                {
                    SetReward(+0.2f);  // Positive reward for successful action
                }
                else
                {
                    SetReward(-0.1f);
                }

                InCombate(unit);
                
            }

        }

    }

    private void InCombate(UnitNew myUnit){
        if(myUnit.isInFight){
            SetReward(+1f);
        }else{
            SetReward(-0.5f);
        }
    }
}
