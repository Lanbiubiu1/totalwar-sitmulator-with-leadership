using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NoOpArmy.WiseFeline.InfluenceMaps
{
    /// <summary>
    /// Decays an influence map using the value you gave it and clamps the values to not go above/beyond specified values
    /// </summary>
    public class InfluenceMapDecayer : MonoBehaviour
    {
        /// <summary>
        /// The operations done on the map
        /// </summary>
        public enum OperationMode
        {
            /// <summary>
            /// Add the decayValue to the map
            /// </summary>
            Add,

            /// <summary>
            /// Multiply the decay value by the map
            /// </summary>
            Multiply,
        }

        private InfluenceMapComponentBase map;

        /// <summary>
        /// The operation to do
        /// </summary>
        public OperationMode operation = OperationMode.Multiply;

        /// <summary>
        /// Initial Delay In Seconds
        /// </summary>
        [Tooltip("Initial Delay In Seconds")]
        public float InitialDelayInSeconds = 0;

        /// <summary>
        /// Delay between calculations of the map
        /// </summary>
        [Tooltip("Delay between calculations of the map")]
        public float delayBetweenCalculations = 5;

        /// <summary>
        /// The value used for decaying
        /// </summary>
        public float decayValue = 0.1f;

        /// <summary>
        /// The minimum value each map cell should be in
        /// </summary>
        [Tooltip("The minimum value each map cell should be in")]
        public float minValue = 0;

        /// <summary>
        /// The maximum value each map cell should be in
        /// </summary>
        [Tooltip("The maximum value each map cell should be in")]
        public float maxValue = 1;

        private void Awake()
        {
            map = GetComponent<InfluenceMapComponentBase>();
        }

        private IEnumerator Start()
        {
            yield return null;
            if (InitialDelayInSeconds != 0)
                yield return new WaitForSeconds(InitialDelayInSeconds);

            do
            {
                if (operation == OperationMode.Add)
                    map.AddAndClampValue(decayValue, minValue, maxValue);
                else if (operation == OperationMode.Multiply)
                    map.MultiplyAndClampValue(decayValue, minValue, maxValue);

                yield return new WaitForSeconds(delayBetweenCalculations);
            } while (true);
        }
    }
}