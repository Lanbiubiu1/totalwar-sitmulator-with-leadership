using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// Use this component to add health to a GameObject
    /// </summary>
    public class Health : MonoBehaviour, IDamagable
    {
        public bool changeMaterialAtDamageForDebugging = false;

        /// <summary>
        /// Called when health changes
        /// The first parameter is the current health points, the sceond one is the amount of the change and the third is the other GameObject which caused this.
        /// </summary>
        public event Action<float, float, GameObject> OnHealthChanged;

        /// <summary>
        /// The health points of the character
        /// </summary>
        [Tooltip("The health points of the character")]
        public float HealthPoints = 100;

        /// <summary>
        /// Destroy the entity when health reaches 0
        /// </summary>
        [Tooltip("Destroy the entity when health reaches 0")]
        public bool destroyWhenHealthReachesZero;

        /// <summary>
        /// The delay that should happen when destroying the GameObject
        /// </summary>
        [Tooltip("The delay that should happen when destroying the GameObject")]
        public float destroyDelay;

        /// <summary>
        /// The maximum health which this component can have
        /// </summary>
        [Tooltip("The maximum health which this component can have")]
        public float maxHealth = 100;

        public virtual void OnNoHealth(GameObject damager)
        {
            if (destroyWhenHealthReachesZero)
            {
                Destroy(this.gameObject, destroyDelay);
            }
        }

        public virtual void OnFullHealth(GameObject healer)
        {

        }

        IEnumerator ChangeMaterial(Color color)
        {
            float time = 0;
            bool makeColored = true;
            while (time < 1)
            {
                time += (makeColored) ? 0.2f : 0.1f;
                makeColored = !makeColored;
                if (makeColored)
                {
                    GetComponent<Renderer>().material.color = color;
                }
                else
                {
                    GetComponent<Renderer>().material.color = Color.white;
                }
                yield return new WaitForSeconds((makeColored) ? 0.2f : 0.1f);

            }
            GetComponent<Renderer>().material.color = Color.white;
        }

        /// <summary>
        /// Applies damage to this health component
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="damager"></param>
        public void ApplyDamage(float amount, GameObject damager)
        {
            if (amount < 0)
                throw new ArgumentException("Amount should be greater than 0");
            if (changeMaterialAtDamageForDebugging)
            {
                StartCoroutine(ChangeMaterial(Color.red));
            }
            HealthPoints -= amount;
            OnHealthChanged?.Invoke(HealthPoints, amount, damager);
            if (HealthPoints < 0)
            {
                HealthPoints = 0;
            }
        }

        /// <summary>
        /// Heals the agent by some amount
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="healer"></param>
        /// <exception cref="ArgumentException"></exception>
        public void IncreaseHealth(float amount, GameObject healer)
        {
            if (amount < 0)
                throw new ArgumentException("Amount should be greater than 0");
            if (changeMaterialAtDamageForDebugging)
            {
                StartCoroutine(ChangeMaterial(Color.blue));
            }
            HealthPoints += amount;
            OnHealthChanged?.Invoke(HealthPoints, amount, healer);
            if (HealthPoints > maxHealth)
            {
                HealthPoints = maxHealth;
            }
        }
    }
}