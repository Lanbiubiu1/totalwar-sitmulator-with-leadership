using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps
{

    /// <summary>
    /// This component can be attached to an agent so it influences the map
    /// </summary>
    public class RectangularInfluencerAgent : MonoBehaviour
    {
        /// <summary>
        /// The map that this agent influences
        /// </summary>
        [Tooltip("The map that this agent influences")]
        public InfluenceMapComponent mapComponent;

        /// <summary>
        /// The name of the map which should be read from the InfluenceMapCollection
        /// This will only be used if the mapComponent is null
        /// </summary>
        [Tooltip("This will only be used if the mapComponent is null.")]
        public string mapName = "";
        public InfluenceMapComponentBase AgentMap { get; set; }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        [Tooltip("Width of the rectangle")]
        public int TemplateWidth = 10;

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        [Tooltip("Height of the rectangle")]
        public int TemplateHeight = 5;

        /// <summary>
        /// The offset applied to agent position to calculate starting x and y of the rectangle which then we move to right and up to fill.
        /// </summary>
        [Tooltip("The offset applied to agent position to calculate starting x and y of the rectangle which then we move to right and up to fill.")]
        public Vector2Int agentPositionOffset;

        /// <summary>
        /// The value to add for this object.
        /// </summary>
        public float Value = 1;

        /// <summary>
        /// Checking this will cause the object influence to be updated in the influence map automatically.
        /// If the object moves, you should check this and set the interval based on its speed and the accuracy you require
        /// </summary>
        [Tooltip("If the object moves, you should check this and set the interval based on its speed and the accuracy you require")]
        public bool updatePositionAutomatically = true;

        /// <summary>
        /// The update interval of the agent. 0.5 means the agent updates its position on the map two times a second.
        /// </summary>
        [Tooltip("The update interval of the agent. 0.5 means the agent updates its position on the map two times a second.")]
        public float updateInterval = 0.3f;

        public bool shouldDrawGizmos;
        public Color gizmoColor = Color.black;

        private Vector2Int previousPoint = Vector2Int.one * int.MinValue;

        private void Start()
        {
            if (mapComponent != null)
            {
                AgentMap = mapComponent;
            }
            else
            {
                AgentMap = InfluenceMapCollection.Instance.GetMap(mapName);
            }
            if (AgentMap.IsMapValid())
                StartCoroutine(UpdatePosition());
        }

        /// <summary>
        /// Use this to set the map later on if it cannot be set automatically at Start()
        /// </summary>
        /// <param name="map"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetMap(InfluenceMapComponent map)
        {
            if (AgentMap != null)
                throw new InvalidOperationException("The map should be null if you want to set it to a new value");
            AgentMap = map;
            if (AgentMap != null)
                StartCoroutine(UpdatePosition());

        }

        private void OnDestroy()
        {
            if (AgentMap.IsMapValid())
            {
                AgentMap.AddRectangularInfluence(previousPoint.x, previousPoint.y, TemplateWidth, TemplateHeight, -Value);// removes old influence
                previousPoint = Vector2Int.one * int.MinValue;
            }
        }

        private IEnumerator UpdatePosition()
        {
            if (AgentMap.IsMapValid())
            {
                Vector2Int currentPoint = AgentMap.WorldToMapPosition(transform.position) + agentPositionOffset;
                AgentMap.AddRectangularInfluence(currentPoint.x, currentPoint.y, TemplateWidth, TemplateHeight, Value);
                previousPoint = currentPoint;
            }
            while (updatePositionAutomatically)
            {
                if (AgentMap.IsMapValid())
                {
                    Vector2Int currentPoint = AgentMap.WorldToMapPosition(transform.position) + agentPositionOffset;
                    if (previousPoint != currentPoint)
                    {
                        AgentMap.AddRectangularInfluence(previousPoint.x, previousPoint.y, TemplateWidth, TemplateHeight, -Value);// removes old influence
                        AgentMap.AddRectangularInfluence(currentPoint.x, currentPoint.y, TemplateWidth, TemplateHeight, Value);
                        previousPoint = currentPoint;
                    }
                }

                yield return new WaitForSeconds(updateInterval);

            }
        }

        private void OnDrawGizmos()
        {
            if (shouldDrawGizmos && AgentMap != null && AgentMap.IsMapValid())
            {
                for (int i = 0; i < TemplateWidth; i++)
                {
                    Gizmos.color = gizmoColor;
                    Vector3 offset = new Vector3(agentPositionOffset.x * AgentMap.Map.CellSize, 0, agentPositionOffset.y * AgentMap.Map.CellSize);
                    for (int j = 0; j < TemplateHeight; j++)
                    {
                        Vector3 position = transform.position +offset + new Vector3(i * AgentMap.Map.CellSize, 0, j * AgentMap.Map.CellSize);
                        Gizmos.DrawCube(position, Vector3.one * AgentMap.Map.CellSize);
                    }
                    Gizmos.color = Color.white;
                }

            }

        }

    }
}