using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedCollider : MonoBehaviour
{
    public ArcherNew unit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Enter wall");
            return;
        }

        if (unit.commandTarget != null && other.GetComponentInParent<UnitNew>() == unit.commandTarget)
            unit.cunit.Stop();

        unit.unitsInRange.Add(other.GetComponentInParent<UnitNew>());

    }

  
    private void OnTriggerStay(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Stay wall");
            return;
        }
        Debug.DrawLine(unit.position + Vector3.up * 5, other.gameObject.transform.position + Vector3.up, Color.green);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetType() != typeof(BoxCollider)) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Exit wall");
            return;
        }
        unit.unitsInRange.Remove(other.GetComponentInParent<UnitNew>());

        if (unit.commandTarget != null && unit.commandTarget == other.GetComponentInParent<UnitNew>())
            unit.commandTarget = null;
    }



}
