using Unity.Collections;
using UnityEngine;

namespace NoOpArmy.WiseFeline.InfluenceMaps
{
    /// <summary>
    /// This asset type is used as a template for the stamp shape of agents which influence the amp
    /// Usually different agents based on their behavior need both different sizes and fall off curve shapes
    /// </summary>
    [CreateAssetMenu(menuName = "NoOpArmy/Wise Feline/InfluenceMapTemplate")]
    public class InfluenceMapTemplate : ScriptableObject
    {
#if WF_BURST
        /// <summary>
        /// Should this template bake its curve for burst usage at startup or it should be baked at first usag
        /// </summary>
        [Tooltip("Should this template bake its curve for burst usage at startup or it should be baked at first usag")]
        public bool shouldBakeCurveForBurstAtStartup = true;

        /// <summary>
        /// Number of samples for the baked data array
        /// </summary>
        [Tooltip("Number of samples for the baked data array")]
        public int sampleCount = 100;
#endif

        /// <summary>
        /// Radius of the agent in cell count
        /// </summary>
        [Tooltip("Radius of the agent in count")]
        public int Radius = 5;


        /// <summary>
        /// The fall off curve of the influence
        /// </summary>
        [Tooltip("The fall off curve of the influence")]
        public AnimationCurve Curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1, 0, 0, 0, 0), new Keyframe(1, 0, 0, 0, 0, 0) });
#if !WF_BURST
        public InfluenceMap Map { get; private set; }
#else
        public InfluenceMapStruct Map;
#endif

        public int MapSize { get; private set; }

        /// <summary>
        /// The baked version of the animation curve
        /// </summary>
        public NativeArray<float> baked;

#if WF_BURST
        public NativeArray<float> GetBaked()
        {
            if (baked == default)
            {
                BakeCurveData();
            }
            return baked;
        }

        private void BakeCurveData()
        {
            baked = new NativeArray<float>(sampleCount, Allocator.Persistent);
            for (int i = 0; i < sampleCount; ++i)
            {
                float w = ((float)i) / ((float)sampleCount);
                if (w < 0)
                    w = 0;
                if (w > 1)
                    w = 1;
                baked[i] = Curve.Evaluate(w);
            }
        }

        public float GetBakedValue(float w)
        {
            if (w < 0)
                w = 0;
            if (w > 1)
                w = 1;

            var b = GetBaked();

            //see what array elements is w between.
            //for example if sample count is 100 and w is 0.557 then w*100 = 55.7
            //then it is between 55th and 56 element of the array and it is with the weight 0.7 between them
            //so a lerp between 55th and 56th elements with a w = 0.7 is the most accurate value we can get and here we consider the
            //value between 55th and 56th to be linear
            var wTimesSampleCount = w * sampleCount;
            float floor = Mathf.Floor(wTimesSampleCount);
            float ceil = Mathf.Ceil(wTimesSampleCount);
            float w2 = Mathf.Lerp(b[(int)floor], b[(int)ceil], wTimesSampleCount - floor);
            return w2;
        }
#endif

        public void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
#if WF_BURST
            if (Map.Width != 0)
                Map.Dispose();
            if (baked != default)
            {
                baked.Dispose();
                baked = default;
            }
#endif
        }

        public void Init()
        {
            MapSize = (Radius * 2) + 1;
#if !WF_BURST
            Map = new InfluenceMap(MapSize, MapSize, 1);
            Map.PropagateInfluence(MapSize / 2, MapSize / 2, Radius, Curve);
#else
            Map = new InfluenceMapStruct
            {
                Width = MapSize,
                Height = MapSize,
                CellSize = 1,
                allocator = Allocator.Persistent,
            };
            Map.Init();
            if (shouldBakeCurveForBurstAtStartup)
                BakeCurveData();
            InfluenceMapStruct.PropagateInfluence(ref Map, MapSize / 2, MapSize / 2, Radius, GetBaked());

#endif
        }
    }
}