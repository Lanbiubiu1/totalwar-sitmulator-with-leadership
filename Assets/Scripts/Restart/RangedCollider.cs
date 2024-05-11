using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedCollider : MonoBehaviour
{
    public ArcherNew unit;
    public DefenderAgent agent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;


        if (unit.commandTarget != null && other.GetComponentInParent<UnitNew>() == unit.commandTarget)
        {
            unit.state = Utils.UnitState.FIGHTING;
            unit.cunit.Stop();
            agent.AddReward(0.5f);
            Debug.Log("Ranged fight rewarded");
        }
            

        unit.unitsInRange.Add(other.GetComponentInParent<UnitNew>());
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;

        Debug.DrawLine(unit.position + Vector3.up * 5, other.gameObject.transform.position + Vector3.up, Color.green);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;

        unit.unitsInRange.Remove(other.GetComponentInParent<UnitNew>());

        if (unit.commandTarget != null && unit.commandTarget == other.GetComponentInParent<UnitNew>())
            unit.commandTarget = null;
    }

    

}
