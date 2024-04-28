#if !WF_BURST
using System;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps
{
    /// <summary>
    /// The stop condition for search
    /// </summary>
    public enum SearchCondition : byte
    {
        Equal,
        Greater,
        Less,
        NotEqual,
        GreaterOrEqual,
        LessOrEqual
    }

    /// <summary>
    /// This is the main influence map class of the system which contains the actual map logic and can be created in C#
    /// using its constructor and does not depend on any component/MonoBehaviour
    /// </summary>
    public class InfluenceMap
    {
        /// <summary>
        /// anchor location of the map in the world. y value is ignored in the main logic it is only useful for drawing gizmos unless you are using the 2d function
        /// </summary>
        public Vector3 anchorLocation;
        /// <summary>
        /// The cell size in meters
        /// </summary>
        public float CellSize { get; private set; }

        /// <summary>
        /// Height of the map in cells
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Width of the map in cells
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Minimum value which a cell in the map has at the moment
        /// </summary>
        public float MinValue
        {
            get
            {
                if (minMaxValueNeedUpdate)
                    UpdateMinMaxValue();
                return minValue;
            }
            private set => minValue = value;
        }

        /// <summary>
        /// Maximum value which a cell in the map has at the moment
        /// </summary>
        public float MaxValue
        {
            get
            {
                if (minMaxValueNeedUpdate)
                    UpdateMinMaxValue();
                return maxValue;
            }
            private set => maxValue = value;
        }

        /// <summary>
        /// The actual array of cells
        /// </summary>
        private float[,] cells;

        private bool minMaxValueNeedUpdate = true;
        private float minValue;
        private float maxValue;

        /// <summary>
        /// Creates an influence map
        /// </summary>
        /// <param name="mapWidth">Width of the map in cells</param>
        /// <param name="mapHeight">Height of the map in cells</param>
        /// <param name="cellSize">Size of each cell in meters</param>
        public InfluenceMap(int mapWidth, int mapHeight, float cellSize)
        {
            Height = mapHeight;
            Width = mapWidth;
            this.CellSize = cellSize;
            cells = new float[Width, Height];
        }

        /// <summary>
        /// Creates an influence map from a pre-defined array of cells
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="cellSize"></param>
        public InfluenceMap(float[,] cells, float cellSize)
        {
            this.CellSize = cellSize;
            this.cells = cells;
            Height = cells.GetLength(1);
            Width = cells.GetLength(0);
        }

        public void UpdateMinMaxValue()
        {
            maxValue = float.NegativeInfinity;
            minValue = float.PositiveInfinity;
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j] > maxValue)
                    {
                        maxValue = cells[i, j];
                    }
                    if (cells[i, j] < minValue)
                    {
                        minValue = cells[i, j];
                    }
                }
            }
            minMaxValueNeedUpdate = false;
        }

        /// <summary>
        /// Sets a cell's value to the specified value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetCellValue(int x, int y, float value)
        {
            cells[x, y] = value;
            minMaxValueNeedUpdate = true;
        }

        #region MAP_OPERATIONS

        public InfluenceMap Add(float value, InfluenceMap result = null)
        {
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] + value;
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        public InfluenceMap AddAndClamp(float value, float min, float max, InfluenceMap result = null)
        {
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = Math.Clamp(cells[i, j] + value, min, max);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }


        /// <summary>
        /// Adds two maps with the same size to each other and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        public InfluenceMap Add(InfluenceMap otherMap, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] + otherMap.cells[i, j];
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Adds two maps with the same size to each other and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="strength">The other map will be multiplied by this value to increase/decrease its effect</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        public InfluenceMap Add(InfluenceMap otherMap, float strength, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] + (otherMap.cells[i, j] * strength);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Adds the inverse of a map to the current map and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        public InfluenceMap AddInverse(InfluenceMap otherMap, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] + (1 - otherMap.cells[i, j]);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Adds the inverse of a map to the current map and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="strength">The other map will be multiplied by this value to increase/decrease its effect</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        public InfluenceMap AddInverse(InfluenceMap otherMap, float strength, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] + ((1 - otherMap.cells[i, j]) * strength);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        private void AssertCorrectSizes(InfluenceMap otherMap, InfluenceMap result)
        {
            if (otherMap.cells.GetLength(0) != cells.GetLength(0) || otherMap.cells.GetLength(1) != cells.GetLength(1) ||
                            (result != null && (cells.GetLength(0) != result.cells.GetLength(0) || cells.GetLength(1) != result.cells.GetLength(1))))
                throw new ArgumentException("The maps should be the same size to be able to add them together");
        }

        private void AssertCorrectSizes(InfluenceMap result)
        {
            if (result != null && (cells.GetLength(0) != result.cells.GetLength(0) || cells.GetLength(1) != result.cells.GetLength(1)))
                throw new ArgumentException("The maps should be the same size to be able to add them together");
        }

        /// <summary>
        /// Subtract a map from this map and returns a new map containing the result
        /// The maps should be the same size
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public InfluenceMap Subtract(InfluenceMap otherMap, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] - otherMap.cells[i, j];
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Subtract a map from this map and returns a new map containing the result
        /// The maps should be the same size
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="strength">The other map will be multiplied by this value to increase/decrease its effect</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public InfluenceMap Subtract(InfluenceMap otherMap, float strength, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] - (otherMap.cells[i, j] * strength);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Multiplies a map to this map and returns a new map containing the result
        /// The maps should be the same size
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public InfluenceMap Multiply(InfluenceMap otherMap, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] * otherMap.cells[i, j];
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Multiplies a map to this map and returns a new map containing the result
        /// The maps should be the same size
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="strength">The other map will be multiplied by this value to increase/decrease its effect</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public InfluenceMap Multiply(InfluenceMap otherMap, float strength, InfluenceMap result = null)
        {
            AssertCorrectSizes(otherMap, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] * (otherMap.cells[i, j] * strength);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Normalizes the values in a map to values between 0 and 1
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public InfluenceMap Normalize(InfluenceMap result = null)
        {
            AssertCorrectSizes(result, result);
            var distance = MaxValue - MinValue;
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = Mathf.Clamp01((cells[i, j] - minValue) / distance);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Multiplies a map with such a value
        /// </summary>
        /// <param name="value">The value that all map cells will be multiplied with</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        public InfluenceMap Multiply(float value, InfluenceMap result = null)
        {
            AssertCorrectSizes(result, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = cells[i, j] * value;
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        public InfluenceMap MultiplyAndClamp(float value, float min, float max, InfluenceMap result = null)
        {
            AssertCorrectSizes(result, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = Math.Clamp(cells[i, j] * value, min, max);
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }

        /// <summary>
        /// Inverts the map values so 0.1 will become 0.9 and 0.6 will become 0.4
        /// </summary>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        public InfluenceMap Invert(InfluenceMap result = null)
        {
            AssertCorrectSizes(result, result);
            float[,] newCells = (result != null) ? result.cells : new float[cells.GetLength(0), cells.GetLength(1)];
            for (int i = 0; i < cells.GetLength(0); ++i)
            {
                for (int j = 0; j < cells.GetLength(1); ++j)
                {
                    newCells[i, j] = 1 - cells[i, j];
                }
            }
            minMaxValueNeedUpdate = true;
            if (result != null)
            {
                result.minMaxValueNeedUpdate = true;
                return result;
            }
            return new InfluenceMap(newCells, CellSize);
        }
        #endregion

        /// <summary>
        /// Returns the cells of the map, keep in mind that this is unsafe if you modify the array yourself
        /// You need this if you want to do operations on the map's array
        /// </summary>
        /// <returns>A reference to the array that the map uses</returns>
        /// <remarks>
        /// This method does not return a copy so be careful to not modify the array and just use it to calculate new maps
        /// </remarks>
        public float[,] GetCellsArray()
        {
            return cells;
        }

        /// <summary>
        /// Sets the cells of the map to the array you created
        /// </summary>
        /// <param name="cells"></param>
        public void SetCellsArray(float[,] cells)
        {
            this.cells = cells;
            minMaxValueNeedUpdate = true;
        }

        /// <summary>
        /// Get a cell's value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetCellValue(int x, int y)
        {
            return cells[x, y];
        }

        /// <summary>
        /// Adds a value to a cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void AddValueToCell(int x, int y, float value)
        {
            cells[x, y] += value;
            minMaxValueNeedUpdate = true;
        }

        /// <summary>
        /// Propagates an influence stamp based on a curve, a radius and a magnitude
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="influenceCurve"></param>
        /// <param name="magnitude"></param>
        /// <remarks>
        /// This is slower than AddInfluence and is used by the template to bake its values but you can use it
        /// if the number of different templates you need is high enough that the amount of memory is used is not acceptable
        /// </remarks>
        public void PropagateInfluence(int centerX, int centerY, int radius, AnimationCurve influenceCurve, float magnitude = 1)
        {
            int startX = centerX - radius;
            int startY = centerY - radius;
            int endX = centerX + radius;
            int endY = centerY + radius;

            int minX = Mathf.Max(0, startX);
            int maxX = Mathf.Min(endX, this.Width - 1);
            int minY = Mathf.Max(0, startY);
            int maxY = Mathf.Min(endY, this.Height - 1);


            float maxDistance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(centerX + radius, centerY + radius));
            maxDistance = Mathf.Max(maxDistance, 1); // Prevent division by zero when radius is zero
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    cells[x, y] = (PropCurveValue(new Vector2(centerX, centerY), new Vector2(x, y), maxDistance, influenceCurve) * magnitude);
                }
            }
            minMaxValueNeedUpdate = true;
        }

        /// <summary>
        /// Propagates an influence stamp based on a curve, a radius and a magnitude
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="influenceCurve"></param>
        /// <param name="magnitude"></param>
        /// <remarks>
        /// This is slower than AddInfluence and is used by the template to bake its values but you can use it
        /// if the number of different templates you need is high enough that the amount of memory is used is not acceptable
        /// </remarks>
        public void PropagateInfluenceWithLineOfSight(int centerX, int centerY, int radius, AnimationCurve influenceCurve, InfluenceMap obstaclesMap, float magnitude = 1)
        {
            int startX = centerX - radius;
            int startY = centerY - radius;
            int endX = centerX + radius;
            int endY = centerY + radius;

            int minX = Mathf.Max(0, startX);
            int maxX = Mathf.Min(endX, this.Width - 1);
            int minY = Mathf.Max(0, startY);
            int maxY = Mathf.Min(endY, this.Height - 1);


            float maxDistance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(centerX + radius, centerY + radius));
            maxDistance = Mathf.Max(maxDistance, 1); // Prevent division by zero when radius is zero
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    if (obstaclesMap.CheckMapForLineOfSight(centerX, centerY, x, y))
                        cells[x, y] = (PropCurveValue(new Vector2(centerX, centerY), new Vector2(x, y), maxDistance, influenceCurve) * magnitude);
                }
            }
            minMaxValueNeedUpdate = true;
        }

        private float PropCurveValue(Vector2 centerPoint, Vector2 currentPoint, float maxDistance, AnimationCurve curve)
        {
            float distance = Vector2.Distance(currentPoint, centerPoint);
            float normalizedDistance = distance / maxDistance;
            float result = curve.Evaluate(normalizedDistance);

            return result;
        }

        /// <summary>
        /// Adds an influence stamp to the map
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="template">The template which should be propagated </param>
        /// <param name="magnitude"></param>
        /// <remarks>
        /// Templates are scriptable objects and can be created in the project window in the editor or at runtime depending on your needs
        /// </remarks>
        public void AddInfluence(int centerX, int centerY, InfluenceMapTemplate template, float magnitude = 1)
        {
            int startX = centerX - template.Radius;
            int startY = centerY - template.Radius;
            int endX = centerX + template.Radius;
            int endY = centerY + template.Radius;

            int minX = Mathf.Max(0, startX);
            int maxX = Mathf.Min(endX, this.Width - 1);
            int minY = Mathf.Max(0, startY);
            int maxY = Mathf.Min(endY, this.Height - 1);
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    cells[x, y] += (template.Map.GetCellValue(x - startX, y - startY) * magnitude);
                }
            }
            minMaxValueNeedUpdate = true;
        }

        /// <summary>
        /// Adds an influence stamp to the map
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="template">The template which should be propagated </param>
        /// <param name="obstaclesMap">The map which contains obstacles and we check line of sight in</param>
        /// <param name="magnitude"></param>
        /// <remarks>
        /// Templates are scriptable objects and can be created in the project window in the editor or at runtime depending on your needs
        /// </remarks>
        public void AddInfluenceWithLineOfSight(int centerX, int centerY, InfluenceMapTemplate template, InfluenceMap obstaclesMap, float magnitude = 1)
        {
            int startX = centerX - template.Radius;
            int startY = centerY - template.Radius;
            int endX = centerX + template.Radius;
            int endY = centerY + template.Radius;

            int minX = Mathf.Max(0, startX);
            int maxX = Mathf.Min(endX, this.Width - 1);
            int minY = Mathf.Max(0, startY);
            int maxY = Mathf.Min(endY, this.Height - 1);
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    if (obstaclesMap.CheckMapForLineOfSight(centerX, centerY, x, y))
                    {
                        cells[x, y] += (template.Map.GetCellValue(x - startX, y - startY) * magnitude);
                    }
                }
            }
            minMaxValueNeedUpdate = true;
        }

        /// <summary>
        /// Adds a rectangular template of a constant value to the map
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        /// <remarks>
        /// This can have negative width and height to move left/down from the starting point
        /// </remarks>
        public void AddRectangularInfluence(int startX, int startY, int width, int height, float value = 1)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("Width and Height should be >= 0");
            int endX = startX + width;
            int endY = startY + height;

            int minX = Mathf.Max(0, startX);
            int maxX = Mathf.Min(endX, this.Width - 1);
            int minY = Mathf.Max(0, startY);
            int maxY = Mathf.Min(endY, this.Height - 1);
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    cells[x, y] += value;
                }
            }
            minMaxValueNeedUpdate = true;
        }


        /// <summary>
        /// Checks to see if there is any line of sight between two points.
        /// Returns true if there is any and false otherwise.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public bool CheckMapForLineOfSight(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2 && x1 >= 0 && x1 < Width && y1 >= 0 && y1 < Height && cells[x1, y1] > 0)
                return false;
            float len = Mathf.Max(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
            for (int i = 0; i <= len; ++i)
            {
                //# interpolate between (x1,y1) and (x2,y2)
                float t = (float)i / len;
                //# at t=0.0 we get (x1,y1); at t=1.0 we get (x2,y2)
                int x = Mathf.RoundToInt(x1 * (1.0f - t) + x2 * t);
                int y = Mathf.RoundToInt(y1 * (1.0f - t) + y2 * t);
                if (x >= 0 && x < Width && y >= 0 && y < Height && cells[x, y] > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Searches for a value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="condition"></param>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool SearchForValueWithRandomStartingPoint(float searchValue, SearchCondition condition, Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = Mathf.Max(0, centerCell.x - radius);
            int startAreaY = Mathf.Max(0, centerCell.y - radius);
            int endAreaX = Mathf.Min(cells.GetLength(0) - 1, centerCell.x + radius);
            int endAreaY = Mathf.Min(cells.GetLength(1) - 1, centerCell.y + radius);

            int startI = UnityEngine.Random.Range(startAreaX, endAreaX + 1);
            int startJ = UnityEngine.Random.Range(startAreaY, endAreaY + 1);

            int lenX = endAreaX - startAreaX + 1;
            int lenY = endAreaY - startAreaY + 1;

            for (int i = startI; i < lenX + startI; i++)
            {
                for (int j = startJ; j < lenY + startJ; j++)
                {
                    int wrappedI = (i % lenX) + startAreaX;
                    int wrappedJ = (j % lenY) + startAreaY;

                    bool found = condition == SearchCondition.Equal && cells[wrappedI, wrappedJ] == searchValue ||
                                 condition == SearchCondition.Greater && cells[wrappedI, wrappedJ] > searchValue ||
                                 condition == SearchCondition.GreaterOrEqual && cells[wrappedI, wrappedJ] >= searchValue ||
                                 condition == SearchCondition.Less && cells[wrappedI, wrappedJ] < searchValue ||
                                 condition == SearchCondition.LessOrEqual && cells[wrappedI, wrappedJ] <= searchValue ||
                                 condition == SearchCondition.NotEqual && cells[wrappedI, wrappedJ] != searchValue;
                    if (found)
                    {
                        result = new Vector2Int(wrappedI, wrappedJ);
                        return true;
                    }
                }
            }

            result = Vector2Int.zero;
            return false;
        }

        /// <summary>
        /// Searches for the highest value around
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForHighestValueWithRandomStartingPoint(Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = Mathf.Max(0, centerCell.x - radius);
            int startAreaY = Mathf.Max(0, centerCell.y - radius);
            int endAreaX = Mathf.Min(cells.GetLength(0) - 1, centerCell.x + radius);
            int endAreaY = Mathf.Min(cells.GetLength(1) - 1, centerCell.y + radius);

            int startI = UnityEngine.Random.Range(startAreaX, endAreaX + 1);
            int startJ = UnityEngine.Random.Range(startAreaY, endAreaY + 1);

            int lenX = endAreaX - startAreaX + 1;
            int lenY = endAreaY - startAreaY + 1;
            float max = float.MinValue;
            result = new Vector2Int(0, 0);
            for (int i = startI; i < lenX + startI; i++)
            {
                for (int j = startJ; j < lenY + startJ; j++)
                {
                    int wrappedI = (i % lenX) + startAreaX;
                    int wrappedJ = (j % lenY) + startAreaY;

                    float val = cells[wrappedI, wrappedJ];
                    if (val > max)
                    {
                        result = new Vector2Int(wrappedI, wrappedJ);
                        max = val;
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Searches for the lowest value around
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForLowestValueWithRandomStartingPoint(Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = Mathf.Max(0, centerCell.x - radius);
            int startAreaY = Mathf.Max(0, centerCell.y - radius);
            int endAreaX = Mathf.Min(cells.GetLength(0) - 1, centerCell.x + radius);
            int endAreaY = Mathf.Min(cells.GetLength(1) - 1, centerCell.y + radius);

            int startI = UnityEngine.Random.Range(startAreaX, endAreaX + 1);
            int startJ = UnityEngine.Random.Range(startAreaY, endAreaY + 1);
            int lenX = endAreaX - startAreaX + 1;
            int lenY = endAreaY - startAreaY + 1;
            float min = float.MaxValue;
            float distance = float.MaxValue;
            result = new Vector2Int(0, 0);
            for (int i = startI; i < lenX + startI; i++)
            {
                for (int j = startJ; j < lenY + startJ; j++)
                {
                    int wrappedI = (i % lenX) + startAreaX;
                    int wrappedJ = (j % lenY) + startAreaY;

                    float val = cells[wrappedI, wrappedJ];
                    var dist = Vector2Int.Distance(centerCell, new Vector2Int(wrappedI, wrappedJ));
                    if (val < min || (val == min && dist < distance))
                    {
                        result = new Vector2Int(wrappedI, wrappedJ);
                        min = val;
                        distance = dist;
                    }
                }
            }
            return min;
        }

        /// <summary>
        /// Searches for the highest value closest to me
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForHighestValueCloestToCenter(Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = Mathf.Max(0, centerCell.x - radius);
            int startAreaY = Mathf.Max(0, centerCell.y - radius);
            int endAreaX = Mathf.Min(cells.GetLength(0) - 1, centerCell.x + radius);
            int endAreaY = Mathf.Min(cells.GetLength(1) - 1, centerCell.y + radius);
            int startI = UnityEngine.Random.Range(startAreaX, endAreaX + 1);
            int startJ = UnityEngine.Random.Range(startAreaY, endAreaY + 1);

            int lenX = endAreaX - startAreaX + 1;
            int lenY = endAreaY - startAreaY + 1;
            float max = float.MinValue;
            float distance = float.MaxValue;
            result = new Vector2Int(0, 0);
            for (int i = startI; i < lenX + startI; i++)
            {
                for (int j = startJ; j < lenY + startJ; j++)
                {
                    int wrappedI = (i % lenX) + startAreaX;
                    int wrappedJ = (j % lenY) + startAreaY;

                    float val = cells[wrappedI, wrappedJ];
                    var dist = Vector2Int.Distance(centerCell, new Vector2Int(wrappedI, wrappedJ));
                    if (val > max || (val == max && dist < distance))
                    {
                        result = new Vector2Int(wrappedI, wrappedJ);
                        max = val;
                        distance = dist;
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Searches for the lowest value to me
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public float SearchForLowestValueClosestToCenter(Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = Mathf.Max(0, centerCell.x - radius);
            int startAreaY = Mathf.Max(0, centerCell.y - radius);
            int endAreaX = Mathf.Min(cells.GetLength(0) - 1, centerCell.x + radius);
            int endAreaY = Mathf.Min(cells.GetLength(1) - 1, centerCell.y + radius);

            int startI = UnityEngine.Random.Range(startAreaX, endAreaX + 1);
            int startJ = UnityEngine.Random.Range(startAreaY, endAreaY + 1);
            int lenX = endAreaX - startAreaX + 1;
            int lenY = endAreaY - startAreaY + 1;
            float min = float.MaxValue;
            result = new Vector2Int(0, 0);
            for (int i = startI; i < lenX + startI; i++)
            {
                for (int j = startJ; j < lenY + startJ; j++)
                {
                    int wrappedI = (i % lenX) + startAreaX;
                    int wrappedJ = (j % lenY) + startAreaY;

                    float val = cells[wrappedI, wrappedJ];
                    if (val < min)
                    {
                        result = new Vector2Int(wrappedI, wrappedJ);
                        min = val;
                    }
                }
            }
            return min;
        }

        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates
        /// The y value of the returned vector is always 0 and x and z are set to the converted values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 MapToWorldPosition(int x, int y)
        {
            return new Vector3(x * CellSize + anchorLocation.x, 0, y * CellSize + anchorLocation.z);
        }

        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates in 2d plane. 
        /// The z value of the returned vector is always 0 and x and y are set to the converted values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 MapToWorldPosition2d(int x, int y)
        {
            return new Vector3(x * CellSize + anchorLocation.x, y * CellSize + anchorLocation.y, 0);
        }

        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates
        /// The y value of the returned vector is always 0 and x and z are set to the converted values
        /// </summary>
        /// <param name="x">x position on the influence map</param>
        /// <param name="y">y position on the influence map</param>
        /// <param name="terrain">The terrain to get the height from</param>
        /// <returns></returns>
        public Vector3 MapToWorldPosition(int x, int y, Terrain terrain)
        {
            var v = new Vector3(x * CellSize + anchorLocation.x, 0, y * CellSize + anchorLocation.z);
            v.y = terrain.SampleHeight(v);
            return v;
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
            return new Vector2Int(
                (int)Mathf.Round((position.x - anchorLocation.x) / CellSize),
                (int)Mathf.Round((position.z - anchorLocation.z) / CellSize));
        }

        /// <summary>
        /// Converts a position from unity world space to map coordinates. It uses the 2D x,y plane instead of x,z
        /// The z component of the vector has no effect and x and y are converted to 2d map coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <remarks>
        /// Keep in mind that the position can be outside the map depending on where the position is and the map size
        /// </remarks>
        public Vector2Int WorldToMapPosition2d(Vector3 position)
        {
            return new Vector2Int(
                (int)Mathf.Round((position.x - anchorLocation.x) / CellSize),
                (int)Mathf.Round((position.y - anchorLocation.y) / CellSize));
        }
    }
}
#endif