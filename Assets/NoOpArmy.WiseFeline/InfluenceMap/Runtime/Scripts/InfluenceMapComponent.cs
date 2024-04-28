using System;
using UnityEngine;
using Unity.Collections;

namespace NoOpArmy.WiseFeline.InfluenceMaps
{
    /// <summary>
    /// A helper component which makes it easier to use influence maps
    /// Attach this component to a GameObject to create an influence map at awake and use it as a reference for
    /// InfluencerAgent component.
    /// The map field is the actual influence map that you can use to search for values or directly change cell values
    /// </summary>
    public class InfluenceMapComponent : InfluenceMapComponentBase
    {

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
        /// Create the map on awake
        /// </summary>
        public bool CreateOnAwake = true;

        /// <summary>
        /// ///MapName
        /// </summary>
        public string mapName;


#if UNITY_EDITOR
        public Color lowestColor = new Color(0, 0, 1, 0.58f);
        public Color highestColor = new Color(1, 0, 0, 0.58f);
        public bool drawGizmos;
        public float delayBetweenMinMaxUpdates = 1;
        private float lastMinMaxUpdate;
#endif

        private void Awake()
        {
            if (CreateOnAwake)
            {
                CreateMap();
            }

        }

        [Obsolete("This method is depricated and will be removed. Use CreateMap() instead")]
        public void CreateOnStartup()
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
                allocator = Allocator.Persistent,
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
#if !WF_BURST
                        Gizmos.color = Color.Lerp(lowestColor, highestColor, Map.GetCellValue(i, j) / (Map.MaxValue - Map.MinValue + Mathf.Epsilon));
#else
                        Gizmos.color = Color.Lerp(lowestColor, highestColor, InfluenceMapStruct.GetCellValue(ref Map, i, j) / (Map.MaxValue - Map.MinValue + Mathf.Epsilon));
#endif
                        if (!is2dMap)
                            Gizmos.DrawCube(new Vector3(i * Map.CellSize + Map.anchorLocation.x, Map.anchorLocation.y, j * Map.CellSize + Map.anchorLocation.z), Vector3.one * Map.CellSize);
                        else
                            Gizmos.DrawCube(new Vector3(i * Map.CellSize + Map.anchorLocation.x, j * Map.CellSize + Map.anchorLocation.y, Map.anchorLocation.z), Vector3.one * Map.CellSize);
                        Gizmos.color = defaultColor;
                    }
                }
            }
        }
#endif
    }

    public class InfluenceMapComponentBase : MonoBehaviour
    {
        /// <summary>
        /// Is this map used for a 2d game in the x,y plane
        /// </summary>
        [Tooltip("Is this map used for a 2d game in the x,y plane")]
        public bool is2dMap = false;

#if !WF_BURST

        /// <summary>
        /// The actual influence map object which can be used for searching or setting values
        /// </summary>
        public InfluenceMap Map { get; protected set; }

        public bool IsMapValid()
        {
            return Map != null;
        }
#else 
        /// <summary>
        /// The actual influence map object which can be used for searching or setting values
        /// </summary>
        public InfluenceMapStruct Map;

        public bool IsMapValid()
        {
            return Map.Width != 0;
        }
#endif

        public void AddAndClampValue(float value, float min, float max)
        {
#if !WF_BURST
            Map.AddAndClamp(value, min, max);
#else
            InfluenceMapStruct.AddAndClamp(ref Map, value, min, max, ref Map);
#endif
        }

        public void MultiplyAndClampValue(float value, float min, float max)
        {
#if !WF_BURST
            Map.MultiplyAndClamp(value, min, max);
#else
            InfluenceMapStruct.MultiplyAndClamp(ref Map, value, min, max, ref Map);
#endif
        }

        /// <summary>
        /// Adds the inverse of a map to the current map and returns a new map containing the result
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="myMapTemplate"></param>
        /// <param name="magnitude"></param>
        public void AddInfluence(int x, int y, InfluenceMapTemplate myMapTemplate, float magnitude = 1)
        {
#if !WF_BURST
            Map.AddInfluence(x, y, myMapTemplate, magnitude);
#else
            InfluenceMapStruct.AddInfluence(ref Map, x, y, myMapTemplate.Map.cells, myMapTemplate.Radius, myMapTemplate.MapSize, magnitude);
#endif
        }

        /// <summary>
        /// Adds the inverse of a map to the current map and returns a new map containing the result
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="myMapTemplate"></param>
        /// <param name="obstaclesMap">The map to check line of sight in</param>
        /// <param name="magnitude"></param>
        public void AddInfluenceWithLineOfSight(int x, int y, InfluenceMapTemplate myMapTemplate, InfluenceMapComponentBase obstaclesMap, float magnitude = 1)
        {
#if !WF_BURST
            Map.AddInfluenceWithLineOfSight(x, y, myMapTemplate, obstaclesMap.Map, magnitude);
#else
            InfluenceMapStruct.AddInfluenceWithLineOfSight(ref Map, x, y, myMapTemplate.Map.cells, myMapTemplate.Radius, myMapTemplate.MapSize, in obstaclesMap.Map, magnitude);
#endif
        }

        /// <summary>
        /// Adds a rectangular template of a constant value to the map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        public void AddRectangularInfluence(int x, int y, int width, int height, float value = 1)
        {
#if !WF_BURST
            Map.AddRectangularInfluence(x, y, width, height, value);
#else
            InfluenceMapStruct.AddRectangularInfluence(ref Map, x, y, width, height, value);
#endif
        }

        /// <summary>
        /// Propagates an influence stamp based on a curve, a radius and a magnitude
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="curve"></param>
        /// <param name="magnitude"></param>
        public void PropagateInfluenceWithLineOfSight(int x, int y, int radius, AnimationCurve curve, InfluenceMapComponentBase ObstaclesMap, int magnitude = 1)
        {
#if !WF_BURST
            Map.PropagateInfluenceWithLineOfSight(x, y, radius, curve, ObstaclesMap.Map, magnitude);
#else
            InfluenceMapStruct.PropagateInfluenceWithLineOfSight(ref Map, x, y, radius, BakeCurveData(curve, 100), in ObstaclesMap.Map, magnitude);
#endif
        }

        /// <summary>
        /// Propagates an influence stamp based on a curve, a radius and a magnitude
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="curve"></param>
        /// <param name="magnitude"></param>
        public void PropagateInfluence(int x, int y, int radius, AnimationCurve curve, int magnitude = 1)
        {
#if !WF_BURST
            Map.PropagateInfluence(x, y, radius, curve, magnitude);// removes old influence
#else
            InfluenceMapStruct.PropagateInfluence(ref Map, x, y, radius, BakeCurveData(curve, 100), magnitude);// removes old influence
#endif
        }

        /// <summary>
        /// Converts a position from unity world space to map coordinates.
        /// The y component of the vector has no effect and x and z are converted to 2d map coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <remarks>
        /// Keep in mind that the position can be outside the map depending on where the position is and the map size
        /// </remarks>
        public Vector2Int WorldToMapPosition(Vector3 position)
        {
            return (!is2dMap) ? Map.WorldToMapPosition(position) : Map.WorldToMapPosition2d(position);
        }

        /// <summary>
        /// Searches for a value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="searchCondition"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool SearchForValueWithRandomStartingPoint(float searchValue, SearchCondition searchCondition, Vector3 position, int radius, out Vector3 result)
        {
#if !WF_BURST
            var pos = Map.WorldToMapPosition(position);
            Vector2Int res;
            bool success = Map.SearchForValueWithRandomStartingPoint(searchValue, searchCondition, pos, radius, out res);
            result = (success) ? Map.MapToWorldPosition(res.x, res.y) : Vector3.zero;
            return success;
#else
            Vector2Int pos = Vector2Int.zero;
            Vector2Int res;
            result = Vector3.zero;
            InfluenceMapStruct.WorldToMapPosition(ref Map, position, ref pos);
            bool success = InfluenceMapStruct.SearchForValueWithRandomStartingPoint(ref Map, searchValue, searchCondition, pos, radius, out res);
            InfluenceMapStruct.MapToWorldPosition(ref Map, res.x, res.y, ref result);
            return success;
#endif
        }

        /// <summary>
        /// Searches for the highest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForHighestValueWithRandomStartingPoint(Vector3 position, int radius, out Vector3 result)
        {
#if !WF_BURST
            var pos = Map.WorldToMapPosition(position);
            Vector2Int res;
            float val = Map.SearchForHighestValueWithRandomStartingPoint(pos, radius, out res);
            result = Map.MapToWorldPosition(res.x, res.y);
            return val;
#else
            Vector2Int pos = Vector2Int.zero;
            Vector2Int res;
            result = Vector3.zero;
            InfluenceMapStruct.WorldToMapPosition(ref Map, position, ref pos);
            float val = InfluenceMapStruct.SearchForHighestValueWithRandomStartingPoint(ref Map, pos, radius, out res);
            InfluenceMapStruct.MapToWorldPosition(ref Map, res.x, res.y, ref result);
            return val;
#endif
        }

        /// <summary>
        /// Searches for the lowest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForLowestValueWithRandomStartingPoint(Vector3 position, int radius, out Vector3 result)
        {
#if !WF_BURST
            var pos = Map.WorldToMapPosition(position);
            Vector2Int res;
            float val = Map.SearchForLowestValueWithRandomStartingPoint(pos, radius, out res);
            result = Map.MapToWorldPosition(res.x, res.y);
            return val;
#else
            Vector2Int pos = Vector2Int.zero;
            Vector2Int res;
            result = Vector3.zero;
            InfluenceMapStruct.WorldToMapPosition(ref Map, position, ref pos);
            float val = InfluenceMapStruct.SearchForLowestValueWithRandomStartingPoint(ref Map, pos, radius, out res);
            InfluenceMapStruct.MapToWorldPosition(ref Map, res.x, res.y, ref result);
            return val;
#endif
        }

        /// <summary>
        /// Searches for the highest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForHighestValueClosestToCenter(Vector3 position, int radius, out Vector3 result)
        {
#if !WF_BURST
            var pos = Map.WorldToMapPosition(position);
            Vector2Int res;
            float val = Map.SearchForHighestValueCloestToCenter(pos, radius, out res);
            result = Map.MapToWorldPosition(res.x, res.y);
            return val;
#else
            Vector2Int pos = Vector2Int.zero;
            Vector2Int res;
            result = Vector3.zero;
            InfluenceMapStruct.WorldToMapPosition(ref Map, position, ref pos);
            float val = InfluenceMapStruct.SearchForHighestValueCloestToCenter(ref Map, pos, radius, out res);
            InfluenceMapStruct.MapToWorldPosition(ref Map, res.x, res.y, ref result);
            return val;
#endif
        }

        /// <summary>
        /// Searches for the lowest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForLowestValueClosestToCenter(Vector3 position, int radius, out Vector3 result)
        {
#if !WF_BURST
            var pos = Map.WorldToMapPosition(position);
            Vector2Int res;
            float val = Map.SearchForLowestValueClosestToCenter(pos, radius, out res);
            result = Map.MapToWorldPosition(res.x, res.y);
            return val;
#else
            Vector2Int pos = Vector2Int.zero;
            Vector2Int res;
            result = Vector3.zero;
            InfluenceMapStruct.WorldToMapPosition(ref Map, position, ref pos);
            float val = InfluenceMapStruct.SearchForLowestValueClosestToCenter(ref Map, pos, radius, out res);
            InfluenceMapStruct.MapToWorldPosition(ref Map, res.x, res.y, ref result);
            return val;
#endif
        }

        /// <summary>
        /// Searches for a value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="searchCondition"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool SearchForValueWithRandomStartingPoint(float searchValue, SearchCondition searchCondition, Vector2Int position, int radius, out Vector2Int result)
        {
#if !WF_BURST
            return Map.SearchForValueWithRandomStartingPoint(searchValue, searchCondition, position, radius, out result);
#else
            return InfluenceMapStruct.SearchForValueWithRandomStartingPoint(ref Map, searchValue, searchCondition, position, radius, out result);
#endif
        }

        /// <summary>
        /// Searches for the highest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForHighestValueWithRandomStartingPoint(Vector2Int position, int radius, out Vector2Int result)
        {
#if !WF_BURST
            return Map.SearchForHighestValueWithRandomStartingPoint(position, radius, out result);
#else
            return InfluenceMapStruct.SearchForHighestValueWithRandomStartingPoint(ref Map, position, radius, out result);
#endif
        }

        /// <summary>
        /// Searches for the lowest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForLowestValueWithRandomStartingPoint(Vector2Int position, int radius, out Vector2Int result)
        {
#if !WF_BURST
            return Map.SearchForLowestValueWithRandomStartingPoint(position, radius, out result);
#else
            return InfluenceMapStruct.SearchForLowestValueWithRandomStartingPoint(ref Map, position, radius, out result);
#endif
        }

        /// <summary>
        /// Searches for the highest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForHighestValueClosestToCenter(Vector2Int position, int radius, out Vector2Int result)
        {
#if !WF_BURST
            return Map.SearchForHighestValueCloestToCenter(position, radius, out result);
#else
            return InfluenceMapStruct.SearchForHighestValueCloestToCenter(ref Map, position, radius, out result);
#endif
        }

        /// <summary>
        /// Searches for the lowest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForLowestValueClosestToCenter(Vector2Int position, int radius, out Vector2Int result)
        {
#if !WF_BURST
            return Map.SearchForLowestValueClosestToCenter(position, radius, out result);
#else
            return InfluenceMapStruct.SearchForLowestValueClosestToCenter(ref Map, position, radius, out result);
#endif
        }

        /// <summary>
        /// Get a cell's value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetCellValue(int x, int y)
        {
#if !WF_BURST
            return Map.GetCellValue(x, y);
#else
            return InfluenceMapStruct.GetCellValue(ref Map, x, y);
#endif
        }

        /// <summary>
        /// Get a cell's value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetCellValue(Vector3 worldPosition)
        {
#if !WF_BURST
            Vector2Int pos = Vector2Int.zero;
            pos = Map.WorldToMapPosition(worldPosition);
            return Map.GetCellValue(pos.x, pos.y);
#else
            Vector2Int pos = Vector2Int.zero;
            InfluenceMapStruct.WorldToMapPosition(ref Map, worldPosition, ref pos);
            return InfluenceMapStruct.GetCellValue(ref Map, pos.x, pos.y);
#endif
        }

        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 MapToWorldPosition(int x, int y)
        {
            return (!is2dMap) ? Map.MapToWorldPosition(x, y) : Map.MapToWorldPosition2d(x, y);
        }

        /// <summary>
        /// Bakes a curve's data to an array to be used by burst methods. Uses JobTemp allocator so you cannot hold on to this 
        /// Curves of tempates have their data baked in a persistent map.This is only for temporary curves
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="sampleCount"></param>
        /// <returns></returns>
#if WF_BURST
        public NativeArray<float> BakeCurveData(AnimationCurve curve, int sampleCount)
        {
            NativeArray<float> baked = new NativeArray<float>(sampleCount, Allocator.Temp);
            for (int i = 0; i < sampleCount; ++i)
            {
                float w = ((float)i) / ((float)sampleCount);
                if (w < 0)
                    w = 0;
                if (w > 1)
                    w = 1;
                baked[i] = curve.Evaluate(w);
            }
            return baked;
        }
#endif

    }
}
