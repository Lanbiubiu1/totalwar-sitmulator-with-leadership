#if WF_BURST
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
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

    [BurstCompile]
    public struct InfluenceMapStruct : IDisposable
    {
        /// <summary>
        /// anchor location of the map in the world. y value is ignored in the main logic it is only useful for drawing gizmos
        /// </summary>
        public Vector3 anchorLocation;

        /// <summary>
        /// The cell size in meters
        /// </summary>
        public float CellSize;

        /// <summary>
        /// Height of the map in cells
        /// </summary>
        public int Height;

        /// <summary>
        /// Width of the map in cells
        /// </summary>
        public int Width;

        /// <summary>
        /// Minimum value which a cell in the map has at the moment
        /// </summary>
        public float MinValue;

        /// <summary>
        /// Maximum value which a cell in the map has at the moment
        /// </summary>
        public float MaxValue;

        /// <summary>
        /// The actual array of cells
        /// </summary>
        public NativeArray<float> cells;

        public Allocator allocator;

        public void Init()
        {
            if (allocator == Allocator.None)
            {
                Debug.LogWarning("The allocator for an influence map was set to None, It will use Persistent");
                allocator = Allocator.Persistent;
            }
            var s = Width * Height;
            cells = new NativeArray<float>(s + (s % 4), allocator);
        }


        [BurstCompile]
        public static void UpdateMinMaxValue(ref InfluenceMapStruct s)
        {
            s.MaxValue = float.NegativeInfinity;
            s.MinValue = float.PositiveInfinity;
            for (int i = 0; i < s.Width; i++)
            {
                for (int j = 0; j < s.Height; j++)
                {
                    int index = CalculateArrayIndex(in s, i, j);
                    if (s.cells[index] > s.MaxValue)
                    {
                        s.MaxValue = s.cells[index];
                    }
                    if (s.cells[index] < s.MinValue)
                    {
                        s.MinValue = s.cells[index];
                    }
                }
            }
        }

        [BurstCompile]
        private static int CalculateArrayIndex(in InfluenceMapStruct s, int height, int width)
        {
            return height * s.Height + width;
        }

        [BurstCompile]
        private static int CalculateArrayIndex(int totalHeight, int height, int width)
        {
            return height * totalHeight + width;
        }

        /// <summary>
        /// Sets a cell's value to the specified value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        [BurstCompile]
        public static void SetCellValue(ref InfluenceMapStruct s, int x, int y, float value)
        {
            s.cells[CalculateArrayIndex(in s, x, y)] = value;
        }

        #region MAP_OPERATIONS

        [BurstCompile]
        public static void Add(ref InfluenceMapStruct s, float value, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2 = f1 + value;
                newCells[i] = f2[0];
                newCells[i + 1] = f2[1];
                newCells[i + 2] = f2[2];
                newCells[i + 3] = f2[3];
            }
            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        [BurstCompile]
        public static void AddAndClamp(ref InfluenceMapStruct s, float value, float min, float max, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2 = math.clamp(f1 + value, min, max);
                newCells[i] = f2[0];
                newCells[i + 1] = f2[1];
                newCells[i + 2] = f2[2];
                newCells[i + 3] = f2[3];
            }
            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Adds two maps with the same size to each other and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        [BurstCompile]
        public static void Add(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 + f2;
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }
            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Adds two maps with the same size to each other and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="strength">The other map will be multiplied by this value to increase/decrease its effect</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        [BurstCompile]
        public static void Add(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, float strength, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 + (f2 * strength);
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Adds the inverse of a map to the current map and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        [BurstCompile]
        public static void AddInverse(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 + (1 - f2);
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }


            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Adds the inverse of a map to the current map and returns a new map containing the result
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="strength">The other map will be multiplied by this value to increase/decrease its effect</param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        [BurstCompile]
        public static void AddInverse(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, float strength, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 + ((1 - f2) * strength);
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }


        /// <summary>
        /// Subtract a map from this map and returns a new map containing the result
        /// The maps should be the same size
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        [BurstCompile]
        public static void Subtract(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 - f2;
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
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
        [BurstCompile]
        public static void Subtract(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, float strength, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 - (f2 * strength);
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        [BurstCompile]
        public static void Multiply(ref InfluenceMapStruct s, float value, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2 = f1 * value;
                newCells[i] = f2[0];
                newCells[i + 1] = f2[1];
                newCells[i + 2] = f2[2];
                newCells[i + 3] = f2[3];
            }
            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        [BurstCompile]
        public static void MultiplyAndClamp(ref InfluenceMapStruct s, float value, float min, float max, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2 = math.clamp(f1 * value, min, max);
                newCells[i] = f2[0];
                newCells[i + 1] = f2[1];
                newCells[i + 2] = f2[2];
                newCells[i + 3] = f2[3];
            }
            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Multiplies a map to this map and returns a new map containing the result
        /// The maps should be the same size
        /// </summary>
        /// <param name="otherMap"></param>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        [BurstCompile]
        public static void Multiply(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 * f2;
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
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
        [BurstCompile]
        public static void Multiply(ref InfluenceMapStruct s, in InfluenceMapStruct otherMap, float strength, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f2 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f2[0] = otherMap.cells[i];
                f2[1] = otherMap.cells[i + 1];
                f2[2] = otherMap.cells[i + 2];
                f2[3] = otherMap.cells[i + 3];
                f3 = f1 * (f2 * strength);
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Normalizes the values in a map to values between 0 and 1
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [BurstCompile]
        public static void Normalize(ref InfluenceMapStruct s, ref InfluenceMapStruct result)
        {
            UpdateMinMaxValue(ref s);
            var distance = s.MaxValue - s.MinValue;
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f3 = Mathf.Clamp01((f1 - s.MinValue) / distance);
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }



            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
        }

        /// <summary>
        /// Inverts the map values so 0.1 will become 0.9 and 0.6 will become 0.4
        /// </summary>
        /// <param name="result">If you have a map already allocated for the result, you can supply it here</param>
        /// <returns></returns>
        [BurstCompile]
        public static void Invert(ref InfluenceMapStruct s, ref InfluenceMapStruct result)
        {
            NativeArray<float> newCells = (result.Width != 0) ? result.cells : new NativeArray<float>(s.Width * s.Height, Allocator.TempJob);
            for (int i = 0; i < newCells.Length; i += 4)
            {

                float4 f1 = new float4();
                float4 f3 = new float4();
                f1[0] = s.cells[i];
                f1[1] = s.cells[i + 1];
                f1[2] = s.cells[i + 2];
                f1[3] = s.cells[i + 3];
                f3 = 1 - f1;
                newCells[i] = f3[0];
                newCells[i + 1] = f3[1];
                newCells[i + 2] = f3[2];
                newCells[i + 3] = f3[3];
            }

            if (result.Width == 0)
            {
                result = new InfluenceMapStruct
                {
                    CellSize = s.CellSize,
                    Width = s.Width,
                    Height = s.Height,
                    cells = newCells,
                };
            }
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
        [BurstCompile]
        public NativeArray<float> GetCellsArray()
        {
            return cells;
        }

        /// <summary>
        /// Sets the cells of the map to the array you created
        /// </summary>
        /// <param name="cells"></param>
        [BurstCompile]
        public static void SetCellsArray(ref InfluenceMapStruct s, ref NativeArray<float> cells)
        {
            s.cells = cells;

        }

        /// <summary>
        /// Get a cell's value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float GetCellValue(ref InfluenceMapStruct s, int x, int y)
        {
            return s.cells[CalculateArrayIndex(in s, x, y)];
        }

        /// <summary>
        /// Adds a value to a cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        [BurstCompile]
        public static void AddValueToCell(ref InfluenceMapStruct s, int x, int y, float value)
        {
            s.cells[CalculateArrayIndex(in s, x, y)] += value;

        }

        /// <summary>
        /// Propagates an influence stamp based on a curve, a radius and a magnitude
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="influenceCurve"></param>
        /// <param name="magnitude"></param>
        [BurstCompile]
        public static void PropagateInfluence(ref InfluenceMapStruct s, int centerX, int centerY, int radius, in NativeArray<float> influenceCurve, float magnitude = 1)
        {
            int startX = centerX - radius;
            int startY = centerY - radius;
            int endX = centerX + radius;
            int endY = centerY + radius;

            int minX = math.max(0, startX);
            int maxX = math.min(endX, s.Width - 1);
            int minY = math.max(0, startY);
            int maxY = math.min(endY, s.Height - 1);

            float maxDistance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(centerX + radius, centerY + radius));
            maxDistance = math.max(maxDistance, 1); // Prevent division by zero when radius is zero
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    s.cells[CalculateArrayIndex(in s, x, y)] = (PropCurveValue(new Vector2(centerX, centerY), new Vector2(x, y), maxDistance, influenceCurve) * magnitude);
                }
            }

        }

        /// <summary>
        /// Propagates an influence stamp based on a curve, a radius and a magnitude
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="influenceCurve"></param>
        /// <param name="magnitude"></param>
        [BurstCompile]
        public static void PropagateInfluenceWithLineOfSight(ref InfluenceMapStruct s, int centerX, int centerY, int radius, in NativeArray<float> influenceCurve, in InfluenceMapStruct obstaclesMap, float magnitude = 1)
        {
            int startX = centerX - radius;
            int startY = centerY - radius;
            int endX = centerX + radius;
            int endY = centerY + radius;

            int minX = math.max(0, startX);
            int maxX = math.min(endX, s.Width - 1);
            int minY = math.max(0, startY);
            int maxY = math.min(endY, s.Height - 1);

            float maxDistance = Vector2.Distance(new Vector2(centerX, centerY), new Vector2(centerX + radius, centerY + radius));
            maxDistance = math.max(maxDistance, 1); // Prevent division by zero when radius is zero
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    if (InfluenceMapStruct.CheckMapForLineOfSight(in obstaclesMap, centerX, centerY, x, y))
                        s.cells[CalculateArrayIndex(in s, x, y)] = (PropCurveValue(new Vector2(centerX, centerY), new Vector2(x, y), maxDistance, influenceCurve) * magnitude);
                }
            }

        }

        [BurstCompile]
        private static float PropCurveValue(in Vector2 centerPoint, in Vector2 currentPoint, float maxDistance, in NativeArray<float> curve)
        {
            float distance = Vector2.Distance(currentPoint, centerPoint);
            float normalizedDistance = distance / maxDistance;
            float result = GetBakedValue(curve, normalizedDistance);

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
        [BurstCompile]
        public static void AddInfluence(ref InfluenceMapStruct s, int centerX, int centerY, in NativeArray<float> template, int radius, int templateHeight, float magnitude = 1)
        {
            int startX = centerX - radius;
            int startY = centerY - radius;
            int endX = centerX + radius;
            int endY = centerY + radius;

            int minX = math.max(0, startX);
            int maxX = math.min(endX, s.Width - 1);
            int minY = math.max(0, startY);
            int maxY = math.min(endY, s.Height - 1);
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    s.cells[CalculateArrayIndex(in s, x, y)] +=
                        template[CalculateArrayIndex(templateHeight, x - startX, y - startY)] * magnitude;
                }
            }

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
        [BurstCompile]
        public static void AddInfluenceWithLineOfSight(ref InfluenceMapStruct s, int centerX, int centerY, in NativeArray<float> template, int radius, int templateHeight, in InfluenceMapStruct obstaclesMap, float magnitude = 1)
        {
            int startX = centerX - radius;
            int startY = centerY - radius;
            int endX = centerX + radius;
            int endY = centerY + radius;

            int minX = math.max(0, startX);
            int maxX = math.min(endX, s.Width - 1);
            int minY = math.max(0, startY);
            int maxY = math.min(endY, s.Height - 1);
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    if (InfluenceMapStruct.CheckMapForLineOfSight(in obstaclesMap, centerX, centerY, x, y))
                        s.cells[CalculateArrayIndex(in s, x, y)] +=
                            template[CalculateArrayIndex(templateHeight, x - startX, y - startY)] * magnitude;
                }
            }

        }

        /// <summary>
        /// Adds a rectangular template of a constant value to the map
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="value"></param>
        [BurstCompile]
        public static void AddRectangularInfluence(ref InfluenceMapStruct s, int startX, int startY, int width, int height, float value = 1)
        {
            int endX = startX + width;
            int endY = startY + height;

            int minX = math.max(0, startX);
            int maxX = math.min(endX, s.Width - 1);
            int minY = math.max(0, startY);
            int maxY = math.min(endY, s.Height - 1);
            for (int x = minX; x <= maxX; ++x)
            {
                for (int y = minY; y <= maxY; ++y)
                {
                    s.cells[CalculateArrayIndex(in s, x, y)] += value;
                }
            }
        }

        [BurstCompile]
        public static bool CheckMapForLineOfSight(in InfluenceMapStruct s, int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2 && x1 >= 0 && x1 < s.Width && y1 >= 0 && y1 < s.Height && s.cells[CalculateArrayIndex(in s, x1, y1)] > 0)
                return false;
            float len = math.max(math.abs(x2 - x1), math.abs(y2 - y1));
            for (int i = 0; i <= len; ++i)
            {
                //# interpolate between (x1,y1) and (x2,y2)
                float t = (float)i / len;
                //# at t=0.0 we get (x1,y1); at t=1.0 we get (x2,y2)
                int x = (int)math.round(x1 * (1.0f - t) + x2 * t);
                int y = (int)math.round(y1 * (1.0f - t) + y2 * t);
                if (x >= 0 && x < s.Width && y >= 0 && y < s.Height && s.cells[CalculateArrayIndex(in s, x, y)] > 0)
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
        [BurstCompile]
        public static bool SearchForValueWithRandomStartingPoint(ref InfluenceMapStruct s, float searchValue, SearchCondition condition, in Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = math.max(0, centerCell.x - radius);
            int startAreaY = math.max(0, centerCell.y - radius);
            int endAreaX = math.min(s.Height - 1, centerCell.x + radius);
            int endAreaY = math.min(s.Width - 1, centerCell.y + radius);

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

                    bool found = condition == SearchCondition.Equal && s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)] == searchValue ||
                                 condition == SearchCondition.Greater && s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)] > searchValue ||
                                 condition == SearchCondition.GreaterOrEqual && s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)] >= searchValue ||
                                 condition == SearchCondition.Less && s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)] < searchValue ||
                                 condition == SearchCondition.LessOrEqual && s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)] <= searchValue ||
                                 condition == SearchCondition.NotEqual && s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)] != searchValue;
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
        /// Searches for the highest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float SearchForHighestValueWithRandomStartingPoint(ref InfluenceMapStruct s, in Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = math.max(0, centerCell.x - radius);
            int startAreaY = math.max(0, centerCell.y - radius);
            int endAreaX = math.min(s.Height - 1, centerCell.x + radius);
            int endAreaY = math.min(s.Width - 1, centerCell.y + radius);

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

                    float val = s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)];
                    if (val > max)
                    {
                        max = val;
                        result = new Vector2Int(wrappedI, wrappedJ);

                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Searches for the lowest value in an area specified by a center and a radius and starts at a random point in the area
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float SearchForLowestValueWithRandomStartingPoint(ref InfluenceMapStruct s, in Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = math.max(0, centerCell.x - radius);
            int startAreaY = math.max(0, centerCell.y - radius);
            int endAreaX = math.min(s.Height - 1, centerCell.x + radius);
            int endAreaY = math.min(s.Width - 1, centerCell.y + radius);

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

                    float val = s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)];
                    float dist = Vector2Int.Distance(new Vector2Int(wrappedI, wrappedJ), centerCell);
                    if (val < min)
                    {
                        min = val;
                        result = new Vector2Int(wrappedI, wrappedJ);
                        distance = dist;
                    }
                }
            }
            return min;
        }

        /// <summary>
        /// Searches for the highest value in an area specified by a center and a radius and starts at a random point around me
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float SearchForHighestValueClosestToCenter(ref InfluenceMapStruct s, in Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = math.max(0, centerCell.x - radius);
            int startAreaY = math.max(0, centerCell.y - radius);
            int endAreaX = math.min(s.Height - 1, centerCell.x + radius);
            int endAreaY = math.min(s.Width - 1, centerCell.y + radius);

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

                    float val = s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)];
                    float dist = Vector2Int.Distance(new Vector2Int(wrappedI, wrappedJ),CenterCell);
                    if (val > max || (val == max && dist < distance))
                    {
                        max = val;
                        result = new Vector2Int(wrappedI, wrappedJ);
                        distance = dist;
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Searches for the lowest value in an area specified by a center and a radius and starts at a random point around me
        /// to not return the same value as the result of the flood fill algorithm
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float SearchForLowestValueClosestToCenter(ref InfluenceMapStruct s, in Vector2Int centerCell, int radius, out Vector2Int result)
        {
            int startAreaX = math.max(0, centerCell.x - radius);
            int startAreaY = math.max(0, centerCell.y - radius);
            int endAreaX = math.min(s.Height - 1, centerCell.x + radius);
            int endAreaY = math.min(s.Width - 1, centerCell.y + radius);

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

                    float val = s.cells[CalculateArrayIndex(in s, wrappedI, wrappedJ)];
                    if (val < min)
                    {
                        min = val;
                        result = new Vector2Int(wrappedI, wrappedJ);

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
        [BurstCompile]
        public static void MapToWorldPosition(ref InfluenceMapStruct s, int x, int y, ref Vector3 result)
        {
            result = new Vector3(x * s.CellSize + s.anchorLocation.x, 0, y * s.CellSize + s.anchorLocation.z);
        }

        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates
        /// The z value of the returned vector is always 0 and x and y are set to the converted values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [BurstCompile]
        public static void MapToWorldPosition2d(ref InfluenceMapStruct s, int x, int y, ref Vector3 result)
        {
            result = new Vector3(x * s.CellSize + s.anchorLocation.x, y * s.CellSize + s.anchorLocation.y, 0);
        }

        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 MapToWorldPosition(int x, int y)
        {
            return new Vector3(x * CellSize + anchorLocation.x, 0, y * CellSize + anchorLocation.z);
        }


        /// <summary>
        /// Converts a position from the map coordinates to unity's world space coordinates in 2d x,y plane
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 MapToWorldPosition2d(int x, int y)
        {
            return new Vector3(x * CellSize + anchorLocation.x, y * CellSize + anchorLocation.y, 0);
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
        [BurstCompile]
        public static void WorldToMapPosition(ref InfluenceMapStruct s, in Vector3 position, ref Vector2Int result)
        {
            result = new Vector2Int(
                (int)math.round((position.x - s.anchorLocation.x) / s.CellSize),
                (int)math.round((position.z - s.anchorLocation.z) / s.CellSize));
        }

        /// <summary>
        /// Converts a position from unity 2d world space of x,y plane to map coordinates.
        /// The y component of the vector has no effect and x and z are converted to 2d map coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <remarks>
        /// Keep in mind that the position can be outside the map depending on where the position is and the map size
        /// </remarks>
        [BurstCompile]
        public static void WorldToMapPosition2d(ref InfluenceMapStruct s, in Vector3 position, ref Vector2Int result)
        {
            result = new Vector2Int(
                (int)math.round((position.x - s.anchorLocation.x) / s.CellSize),
                (int)math.round((position.y - s.anchorLocation.y) / s.CellSize));
        }

        public Vector2Int WorldToMapPosition(in Vector3 position)
        {
            return new Vector2Int(
                (int)math.round((position.x - anchorLocation.x) / CellSize),
                (int)math.round((position.z - anchorLocation.z) / CellSize));
        }

        public Vector2Int WorldToMapPosition2d(in Vector3 position)
        {
            return new Vector2Int(
                (int)math.round((position.x - anchorLocation.x) / CellSize),
                (int)math.round((position.y - anchorLocation.y) / CellSize));
        }

        /// <summary>
        /// Gets a value from a baked curve data array and linearly interpolates between elements when it has to get a value
        /// between two elements
        /// </summary>
        /// <param name="bakedArray"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        [BurstCompile]
        public static float GetBakedValue(in NativeArray<float> bakedArray, float w)
        {
            if (w < 0)
                w = 0;
            if (w > 1)
                w = 1;

            //see what array elements is w between.
            //for example if sample count is 100 and w is 0.557 then w*100 = 55.7
            //then it is between 55th and 56 element of the array and it is with the weight 0.7 between them
            //so a lerp between 55th and 56th elements with a w = 0.7 is the most accurate value we can get and here we consider the
            //value between 55th and 56th to be linear
            var wTimesSampleCount = w * (bakedArray.Length - 1);
            float floor = math.floor(wTimesSampleCount);
            float ceil = math.ceil(wTimesSampleCount);
            float w2 = math.lerp(bakedArray[(int)floor], bakedArray[(int)ceil], wTimesSampleCount - floor);
            return w2;
        }

        public void Dispose()
        {
            if (Width != 0)
            {
                cells.Dispose();
                cells = default;
                Width = 0;
            }
        }

    }
}
#endif