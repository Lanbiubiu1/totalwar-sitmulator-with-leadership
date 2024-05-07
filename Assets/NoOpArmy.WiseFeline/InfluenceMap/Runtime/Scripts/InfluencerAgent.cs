using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps
{

    /// <summary>
    /// This component can be attached to an agent so it influences the map
    /// </summary>
    public class InfluencerAgent : MonoBehaviour
    {
        /// <summary>
        /// The map that this agent influences
        /// </summary>
        [Header("The map to influence")]
        [Tooltip("The map that this agent influences")]
        public InfluenceMapComponent mapComponent;

        /// <summary>
        /// The name of the map which should be read from the InfluenceMapCollection
        /// This will only be used if the mapComponent is null
        /// </summary>
        [Tooltip("The name of the map which should be read from the InfluenceMapCollection. This will only be used if the mapComponent is null.")]
        public string mapName = "";

        /// <summary>
        /// Getter for the agent's map
        /// </summary>
        public InfluenceMapComponentBase AgentMap { get; set; }

        /// <summary>
        /// Should the agent removes its influence from the old place when moving. You might not want this to put breadcrumbs
        /// </summary>
        [Tooltip("Should the agent removes its influence from the old place when moving. You might not want this to put breadcrumbs")]
        public bool shouldRemoveInfluenceWhenMoving = true;


        /// <summary>
        /// The map that this agent considers obstacles for line of sight
        /// </summary>
        [Header("The obstacles map for line of sight")]
        [Tooltip("The map that this agent considers obstacles for line of sight")]
        public InfluenceMapComponent obstaclesMapComponent;

        /// <summary>
        /// The name of the obstacles map which should be read from the InfluenceMapCollection
        /// This will only be used if the obstacleMapComponent is null
        /// </summary>
        [Tooltip("The name of the obstacles map which should be read from the InfluenceMapCollection. This will only be used if the mapComponent is null.")]
        public string obstaclesMapName = "";

        public InfluenceMapComponentBase ObstaclesMap { get; set; }

        /// <summary>
        /// Set this to true if your agents need to consider obstacles and their line of sight when adding their influence. Usually only range units in shooter games might need this.
        /// </summary>
        [Tooltip("Set this to true if your agents need to consider obstacles and their line of sight when adding their influence. Usually only range units in shooter games might need this.")]
        public bool useLineOfSightWhenCalculatingInfluence = false;

        /// <summary>
        /// The way that this map will add its influence
        /// </summary>
        public enum Behavior
        {
            /// <summary>
            /// Uses a single template defined in myMapTemplate
            /// </summary>
            SingleTemplate,

            /// <summary>
            /// Uses a list of tempates defined in templatesList and mapIndex
            /// </summary>
            TemplateList,

            /// <summary>
            /// Uses a curve and radius directly
            /// This is much slower than the two other approaches since it needs to evaluate the curves but templates evaluate their curves only once at initialization
            /// Use this only if using templates is cost prohibitive in terms of memory.
            /// </summary>
            DirectValue
        }

        /// <summary>
        /// The behavior of the map. You can use a single template, a list of them with the ability to change the index of the current one or directly use a radius and a curve
        /// </summary>
        [Tooltip("The behavior of the map. You can use a single template, a list of them with the ability to change the index of the current one or directly use a radius and a curve")]
        public Behavior behavior = Behavior.SingleTemplate;

        /// <summary>
        /// The template that the agent uses for its influence stamp shape
        /// </summary>
        [Tooltip("The template that the agent uses for its influence stamp shape")]
        public InfluenceMapTemplate myMapTemplate;

        /// <summary>
        /// The list of templates when the TemplateList behavior is selected
        /// </summary>
        public List<InfluenceMapTemplate> mapTemplatesList;

        /// <summary>
        /// The index in the list to use when TemplateList behavior is selected. It should be between 0 and list's count - 1
        /// </summary>
        public int mapIndex;

        /// <summary>
        /// The radius to use for the direct value behavior
        /// </summary>
        public int directRadius;

        /// <summary>
        /// The curve to use for the direct value behavior
        /// </summary>
        public AnimationCurve directCurve;

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


        private Vector2Int previousPoint = Vector2Int.one * int.MinValue;
        private int previousIndex;
        private int previousRadius;
        private AnimationCurve previousCurve;

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

            if (obstaclesMapComponent != null)
            {
                ObstaclesMap = obstaclesMapComponent;
            }
            else
            {
                ObstaclesMap = InfluenceMapCollection.Instance.GetMap(obstaclesMapName);
            }
            if (useLineOfSightWhenCalculatingInfluence && (ObstaclesMap == null || !ObstaclesMap.IsMapValid()))
            {
                Debug.LogError("You want to use line of sight and have useLineOfSightWhenCalculatingInfluence set to true but the obstaclesMapComponent or obstaclesMapName are not set to a valid map");
            }
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
            if (AgentMap != null && AgentMap.IsMapValid())
                StartCoroutine(UpdatePosition());
        }

        // <summary>
        /// Use this to set the obstacles map later on if it cannot be set automatically at Start()
        /// </summary>
        /// <param name="map"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetObstaclesMap(InfluenceMapComponent map)
        {
            if (ObstaclesMap != null)
                throw new InvalidOperationException("The map should be null if you want to set it to a new value");
            ObstaclesMap = map;
        }

        private void OnDestroy()
        {
            //ATTEMPT TO FIX RELOAD ISSUE
            //if (!this.gameObject.scene.isLoaded) return;

            if (shouldRemoveInfluenceWhenMoving && AgentMap.IsMapValid() && myMapTemplate != null)
            {
                if (!useLineOfSightWhenCalculatingInfluence)
                {
                    if (behavior == Behavior.SingleTemplate)
                    {
                        AgentMap.AddInfluence(previousPoint.x, previousPoint.y, myMapTemplate, -1);// removes old influence
                        previousPoint = Vector2Int.one * int.MinValue;
                    }
                    else if (behavior == Behavior.TemplateList)
                    {
                        AgentMap.AddInfluence(previousPoint.x, previousPoint.y, mapTemplatesList[previousIndex], -1);// removes old influence
                        previousPoint = Vector2Int.one * int.MinValue;
                    }
                    else if (behavior == Behavior.DirectValue)
                    {
                        AgentMap.PropagateInfluence(previousPoint.x, previousPoint.y, previousRadius, previousCurve, -1);// removes old influence
                        previousPoint = Vector2Int.one * int.MinValue;
                    }
                }
                else if (useLineOfSightWhenCalculatingInfluence && ObstaclesMap != null && ObstaclesMap.IsMapValid())
                {
                    if (behavior == Behavior.SingleTemplate)
                    {
                        AgentMap.AddInfluenceWithLineOfSight(previousPoint.x, previousPoint.y, myMapTemplate, ObstaclesMap, -1);// removes old influence
                        previousPoint = Vector2Int.one * int.MinValue;
                    }
                    else if (behavior == Behavior.TemplateList)
                    {
                        AgentMap.AddInfluenceWithLineOfSight(previousPoint.x, previousPoint.y, mapTemplatesList[previousIndex], ObstaclesMap, -1);// removes old influence
                        previousPoint = Vector2Int.one * int.MinValue;
                    }
                    else if (behavior == Behavior.DirectValue)
                    {
                        AgentMap.PropagateInfluenceWithLineOfSight(previousPoint.x, previousPoint.y, previousRadius, previousCurve, ObstaclesMap, -1);// removes old influence
                        previousPoint = Vector2Int.one * int.MinValue;
                    }
                }
            }
        }

        private IEnumerator UpdatePosition()
        {
            if (AgentMap.IsMapValid() && myMapTemplate != null)
            {
                if (!useLineOfSightWhenCalculatingInfluence)
                {

                    var currentPoint = AgentMap.WorldToMapPosition(transform.position);
                    if (behavior == Behavior.SingleTemplate)
                    {
                        AgentMap.AddInfluence(currentPoint.x, currentPoint.y, myMapTemplate);
                        previousPoint = currentPoint;
                    }
                    else if (behavior == Behavior.TemplateList)
                    {
                        AgentMap.AddInfluence(currentPoint.x, currentPoint.y, mapTemplatesList[mapIndex]);
                        previousPoint = currentPoint;
                        previousIndex = mapIndex;
                    }
                    else if (behavior == Behavior.DirectValue)
                    {
                        AgentMap.PropagateInfluence(currentPoint.x, currentPoint.y, directRadius, directCurve);
                        previousPoint = currentPoint;
                        previousRadius = directRadius;
                        previousCurve = directCurve;
                    }

                }
                else if (useLineOfSightWhenCalculatingInfluence && ObstaclesMap != null && ObstaclesMap.IsMapValid())
                {
                    var currentPoint = AgentMap.WorldToMapPosition(transform.position);
                    if (behavior == Behavior.SingleTemplate)
                    {
                        AgentMap.AddInfluenceWithLineOfSight(currentPoint.x, currentPoint.y, myMapTemplate, ObstaclesMap);
                        previousPoint = currentPoint;
                    }
                    else if (behavior == Behavior.TemplateList)
                    {
                        AgentMap.AddInfluenceWithLineOfSight(currentPoint.x, currentPoint.y, mapTemplatesList[mapIndex], ObstaclesMap);
                        previousPoint = currentPoint;
                        previousIndex = mapIndex;
                    }
                    else if (behavior == Behavior.DirectValue)
                    {
                        AgentMap.PropagateInfluenceWithLineOfSight(currentPoint.x, currentPoint.y, directRadius, directCurve, ObstaclesMap);
                        previousPoint = currentPoint;
                        previousRadius = directRadius;
                        previousCurve = directCurve;
                    }
                }
            }

            //the loop for the rest of the run
            while (updatePositionAutomatically)
            {
                if (AgentMap.IsMapValid() && myMapTemplate != null)
                {
                    if (!useLineOfSightWhenCalculatingInfluence)
                    {
                        var currentPoint = AgentMap.WorldToMapPosition(transform.position);
                        if (previousPoint != currentPoint)
                        {
                            if (behavior == Behavior.SingleTemplate)
                            {
                                if (shouldRemoveInfluenceWhenMoving)
                                    AgentMap.AddInfluence(previousPoint.x, previousPoint.y, myMapTemplate, -1);// removes old influence
                                AgentMap.AddInfluence(currentPoint.x, currentPoint.y, myMapTemplate);
                                previousPoint = currentPoint;
                            }
                            else if (behavior == Behavior.TemplateList)
                            {
                                if (shouldRemoveInfluenceWhenMoving)
                                    AgentMap.AddInfluence(previousPoint.x, previousPoint.y, mapTemplatesList[previousIndex], -1);// removes old influence
                                AgentMap.AddInfluence(currentPoint.x, currentPoint.y, mapTemplatesList[mapIndex]);
                                previousPoint = currentPoint;
                                previousIndex = mapIndex;
                            }
                            else if (behavior == Behavior.DirectValue)
                            {
                                if (shouldRemoveInfluenceWhenMoving)
                                    AgentMap.PropagateInfluence(previousPoint.x, previousPoint.y, previousRadius, previousCurve, -1);// removes old influence
                                AgentMap.PropagateInfluence(currentPoint.x, currentPoint.y, directRadius, directCurve);
                                previousPoint = currentPoint;
                                previousRadius = directRadius;
                                previousCurve = directCurve;
                            }
                        }
                    }
                    else if (useLineOfSightWhenCalculatingInfluence && ObstaclesMap != null && ObstaclesMap.IsMapValid())
                    {
                        var currentPoint = AgentMap.WorldToMapPosition(transform.position);
                        if (previousPoint != currentPoint)
                        {
                            if (behavior == Behavior.SingleTemplate)
                            {
                                if (shouldRemoveInfluenceWhenMoving)
                                    AgentMap.AddInfluenceWithLineOfSight(previousPoint.x, previousPoint.y, myMapTemplate, ObstaclesMap, -1);// removes old influence
                                AgentMap.AddInfluenceWithLineOfSight(currentPoint.x, currentPoint.y, myMapTemplate, ObstaclesMap);
                                previousPoint = currentPoint;
                            }
                            else if (behavior == Behavior.TemplateList)
                            {
                                if (shouldRemoveInfluenceWhenMoving)
                                    AgentMap.AddInfluenceWithLineOfSight(previousPoint.x, previousPoint.y, mapTemplatesList[previousIndex], ObstaclesMap, -1);// removes old influence
                                AgentMap.AddInfluenceWithLineOfSight(currentPoint.x, currentPoint.y, mapTemplatesList[mapIndex], ObstaclesMap);
                                previousPoint = currentPoint;
                                previousIndex = mapIndex;
                            }
                            else if (behavior == Behavior.DirectValue)
                            {
                                if (shouldRemoveInfluenceWhenMoving)
                                    AgentMap.PropagateInfluenceWithLineOfSight(previousPoint.x, previousPoint.y, previousRadius, previousCurve, ObstaclesMap, -1);// removes old influence
                                AgentMap.PropagateInfluenceWithLineOfSight(currentPoint.x, currentPoint.y, directRadius, directCurve, ObstaclesMap);
                                previousPoint = currentPoint;
                                previousRadius = directRadius;
                                previousCurve = directCurve;
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(updateInterval);
            }
        }
    }
}