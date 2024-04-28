using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NoOpArmy.WiseFeline.InfluenceMaps
{
    /// <summary>
    /// This class allows you to calculate a map from multiple others.
    /// You could previously do this without a view using code but this makes it easier to make maps from other maps without writing a line of code
    /// </summary>
    public class InfluenceMapView : InfluenceMapComponentBase
    {
        [Serializable]
        public class ViewOperation
        {
            public enum Operation
            {
                Add,
                AddInverse,
                Subtract,
                Multiply,
                MultiplyByValue,
                Normalize,
                Inverse,
                AddWithStrength,
                SubtractWithStrength,
                MultiplyWithStrength,
            }

            public string mapName;
            public Operation operation;

            /// <summary>
            /// The additional argument needed by the operation like the strength of the second map or the value to multiply with in case of MultiplyByValue
            /// </summary>
            [Tooltip("The additional argument needed by the operation like the strength of the second map or the value to multiply with in case of MultiplyByValue")]
            public float additionalArgument = 1;
        }

        /// <summary>
        /// The first map to start operations from
        /// </summary>
        public string firstMapName;

        /// <summary>
        /// The operations which will be done on the map consecutively 
        /// </summary>
        public List<ViewOperation> operations;

        /// <summary>
        /// Initial Delay In Seconds
        /// </summary>
        [Tooltip("Initial Delay In Seconds")]
        public float InitialDelayInSeconds = 0;

        /// <summary>
        /// If the object moves, you should check this and set the delay between calculations based on speed and the accuracy you require
        /// </summary>
        [Tooltip("If the object moves, you should check this and set the delay between calculations based on speed and the accuracy you require")]
        public bool updatePositionAutomatically = true;

        /// <summary>
        /// Delay between calculations of the map
        /// </summary>
        public float delayBetweenCalculations = 5;

        /// <summary>
        /// Width of the map in cell count
        /// </summary>
        [Tooltip("Width of the map in cell count")]
        public int width = 10;

        /// <summary>
        /// Height of the map in cells
        /// </summary>
        [Tooltip("Height of the map in cells")]
        public int height = 10;

        /// <summary>
        /// Cell size of the map
        /// </summary>
        [Tooltip("Cell size of the map")]
        public float cellSize = 1;


        /// <summary>
        /// ///MapName
        /// </summary>
        public string mapName;

#if UNITY_EDITOR
        public Color lowestColor = new Color(0, 1, 1, 0.58f);
        public Color highestColor = new Color(1, 1, 0, 0.58f);
        public bool drawGizmos;
        public float delayBetweenMinMaxUpdates = 1;
        private float lastMinMaxUpdate;
#endif

        private void Awake()
        {
            CreateMap();
        }

        private void CreateMap()
        {
#if !WF_BURST
            Map = new InfluenceMap(width, height, cellSize);
            Map.anchorLocation = transform.position;
#else
            Map = new InfluenceMapStruct
            {
                Width = width,
                Height = height,
                CellSize = cellSize,
                anchorLocation = transform.position,
                allocator = Unity.Collections.Allocator.Persistent,
            };
            Map.Init();

#endif
            RegisterMapWithTheCollection();
        }

        private void RegisterMapWithTheCollection()
        {
            if (IsMapValid())
                InfluenceMapCollection.Instance?.Register(mapName, this);

        }

        private void OnDestroy()
        {
            InfluenceMapCollection.Instance?.Unregister(mapName);
#if WF_BURST
            if (Map.Width != 0)
                Map.Dispose();
#endif
        }


        private IEnumerator Start()
        {
            yield return null;
            if (InitialDelayInSeconds != 0)
                yield return new WaitForSeconds(InitialDelayInSeconds);

            do
            {
                var firstOp = InfluenceMapCollection.Instance.GetMap(firstMapName);
                for (int i = 0; i < operations.Count; ++i)
                {
                    var secondOp = InfluenceMapCollection.Instance.GetMap(operations[i].mapName);
                    ViewOperation op = operations[i];
                    ExecuteOperation(firstOp.Map, op, secondOp.Map);
                    firstOp = this;
                }

                if (updatePositionAutomatically)
                    yield return new WaitForSeconds(delayBetweenCalculations);

            } while (updatePositionAutomatically);
        }

#if !WF_BURST
        private void ExecuteOperation(InfluenceMap firstOp, ViewOperation operation, InfluenceMap secondOp)
        {
            switch (operation.operation)
            {
                case ViewOperation.Operation.Add:
                    firstOp.Add(secondOp, Map);
                    break;
                case ViewOperation.Operation.AddWithStrength:
                    firstOp.Add(secondOp, operation.additionalArgument, Map);
                    break;
                case ViewOperation.Operation.AddInverse:
                    firstOp.AddInverse(secondOp, Map);
                    break;
                case ViewOperation.Operation.Subtract:
                    firstOp.Subtract(secondOp, Map);
                    break;
                case ViewOperation.Operation.SubtractWithStrength:
                    firstOp.Subtract(secondOp,operation.additionalArgument, Map);
                    break;
                case ViewOperation.Operation.Multiply:
                    firstOp.Multiply(secondOp, Map);
                    break;
                case ViewOperation.Operation.MultiplyWithStrength:
                    firstOp.Multiply(secondOp,operation.additionalArgument, Map);
                    break;
                case ViewOperation.Operation.MultiplyByValue:
                    firstOp.Multiply(operation.additionalArgument, Map);
                    break;
                case ViewOperation.Operation.Normalize:
                    firstOp.Normalize(Map);
                    break;
                case ViewOperation.Operation.Inverse:
                    firstOp.Invert(Map);
                    break;
                default:
                    break;
            }
        }
#else
        private void ExecuteOperation(InfluenceMapStruct firstOp, ViewOperation operation, InfluenceMapStruct secondOp)
        {
            switch (operation.operation)
            {
                case ViewOperation.Operation.Add:
                    InfluenceMapStruct.Add(ref firstOp, secondOp, ref Map);
                    break;
                case ViewOperation.Operation.AddWithStrength:
                    InfluenceMapStruct.Add(ref firstOp, secondOp, operation.additionalArgument, ref Map);
                    break;
                case ViewOperation.Operation.AddInverse:
                    InfluenceMapStruct.AddInverse(ref firstOp, secondOp, ref Map);
                    break;
                case ViewOperation.Operation.Subtract:
                    InfluenceMapStruct.Subtract(ref firstOp, secondOp, ref Map);
                    break;
                case ViewOperation.Operation.SubtractWithStrength:
                    InfluenceMapStruct.Subtract(ref firstOp, secondOp, operation.additionalArgument, ref Map);
                    break;
                case ViewOperation.Operation.Multiply:
                    InfluenceMapStruct.Multiply(ref firstOp, secondOp, ref Map);
                    break;
                case ViewOperation.Operation.MultiplyWithStrength:
                    InfluenceMapStruct.Multiply(ref firstOp, secondOp, operation.additionalArgument, ref Map);
                    break;
                case ViewOperation.Operation.MultiplyByValue:
                    InfluenceMapStruct.Multiply(ref firstOp, operation.additionalArgument, ref Map);
                    break;
                case ViewOperation.Operation.Normalize:
                    InfluenceMapStruct.Normalize(ref firstOp, ref Map);
                    break;
                case ViewOperation.Operation.Inverse:
                    InfluenceMapStruct.Invert(ref firstOp, ref Map);
                    break;
                default:
                    break;
            }
        }

#endif

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
#if !WF_BURST
            if (drawGizmos && Map != null)
            {
                if (lastMinMaxUpdate == 0 || Time.time - lastMinMaxUpdate >= delayBetweenMinMaxUpdates)
                {
                    lastMinMaxUpdate = Time.time;
                    Map.UpdateMinMaxValue();
                }
#else
            if (drawGizmos && Map.Width != 0)
            {
                if (lastMinMaxUpdate == 0 || Time.time - lastMinMaxUpdate >= delayBetweenMinMaxUpdates)
                {
                    lastMinMaxUpdate = Time.time;
                    InfluenceMapStruct.UpdateMinMaxValue(ref Map);
                }
#endif

                for (int j = 0; j < Map.Height; ++j)
                {
                    for (int i = 0; i < Map.Width; ++i)
                    {
                        var defaultColor = Gizmos.color;
                        Gizmos.color = Color.Lerp(lowestColor, highestColor,
#if !WF_BURST
                            Map.GetCellValue(i, j)
#else
                            InfluenceMapStruct.GetCellValue(ref Map, i, j)
#endif
                            / (Map.MaxValue - Map.MinValue + float.Epsilon));
                        Gizmos.DrawCube(new Vector3(i * Map.CellSize + Map.anchorLocation.x, Map.anchorLocation.y, j * Map.CellSize + Map.anchorLocation.z), Vector3.one * Map.CellSize);
                        Gizmos.color = defaultColor;
                    }
                }
            }
        }
#endif
    }
}
