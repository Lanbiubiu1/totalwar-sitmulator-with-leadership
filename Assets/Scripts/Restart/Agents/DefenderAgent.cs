﻿using System.Collections;
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
        sensor.AddObservation(u.range); // Add archer range as observation.
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


    // Process actions received from the neural network. Here, a reward of 1.0f is given for any received action.
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        // Assign a reward for the received action.
        SetReward(1.0f);

    }



}
