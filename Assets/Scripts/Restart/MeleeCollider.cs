using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Utils;

public class MeleeCollider : MonoBehaviour
{
    public UnitNew unit;
    public DefenderAgent agent;// = unit.transform.parent.GetComponent<DefenderAgent>();
   

    private void OnTriggerEnter(Collider other)
    {
        //agent = unit.transform.parent.GetComponent<DefenderAgent>();

        if (other.GetType() != typeof(BoxCollider)) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("Enter wall");
            unit.WallCollided = true;
            //return;
            agent.AddReward(-.5f);
            Debug.Log("unit " + unit.ID + " Wall collision");
            return;
        }
        else if (other.gameObject.CompareTag("Melee"))
        {
            agent.AddReward(0.2f);
            Debug.Log("melee in fight reward");
        }

        if (unit.fightingAgainst.Count == 0)
            unit.fightingTarget = other.GetComponentInParent<UnitNew>();

        unit.fightingAgainst.Add(other.GetComponentInParent<UnitNew>());
        //agent.AddReward(0.2f);
        if (!unit.isInFight)
        {
            if (!other.gameObject) return;

            unit.isInFight = true;
            unit.state = UnitState.FIGHTING;
            var dir = GetVector3Down(other.transform.position - transform.position);
            float dist = dir.magnitude;
            if(dist>0)
            {
                unit.transform.rotation = Quaternion.LookRotation(dir);
                unit.transform.position = transform.position + dir * (dist - unit.meleeCollider.size.z + 2) / dist;
            }
        }
        //agent.AddReward(0.5f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("stay wall");
            unit.WallCollided = true;
            //agent.AddReward(-.5f);
            return;
        }
        Debug.DrawLine(unit.position + Vector3.up * 5, other.gameObject.transform.position + Vector3.up, Color.magenta);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("Exit wall");
            unit.WallCollided = false;
            return;
        }

        unit.fightingAgainst.Remove(other.GetComponentInParent<UnitNew>());

        if (unit.fightingAgainst.Count == 0)
        {
            unit.fightingTarget = null;
            unit.isInFight = false;
            return;
        }

        if (unit.fightingTarget == other.GetComponentInParent<UnitNew>())
            unit.fightingTarget = unit.fightingAgainst.OrderBy(en => Vector3.SqrMagnitude(en.position - unit.position)).First();
    }




}
