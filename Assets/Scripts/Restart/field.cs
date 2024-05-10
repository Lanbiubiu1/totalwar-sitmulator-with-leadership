using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum ArmyRole
{
    Friendly,
    Enemy
}*/

public class field : MonoBehaviour
{
    public List<ArmyNew> armies;
    public ArmyNew ally, enemy;
    public float robotRadius = 10.0f;
    public float KP = 10.0f;
    public float ETA = 100.0f;
    public float movementSpeed = 2.0f;
    public bool autoMove = false;

    private List<UnitNew> units = new List<UnitNew>();
    private List<UnitNew> enemies = new List<UnitNew>();

    private Vector3[] motions = new Vector3[]
    {
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, -1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, 1)
    };
    
    public void InitializeField(ArmyNew ally, ArmyNew enemy)
    {
        this.ally = ally;
        this.enemy = enemy;

        units = ally.units;
        enemies = enemy.units;

        Debug.Log($"Field initialized with {units.Count} units and {enemies.Count} enemies.");
    }

    void Start()
    {
        

    }


    void Update()
    {
        foreach (var unit in units)
        {
            //LogUnitPosition(unit);
            if(autoMove) MoveUnit(unit);
        }
    }

    void MoveUnit(UnitNew unit)
    {
        Vector3 bestMove = Vector3.zero;
        float minPotential = float.MaxValue;
        float maxPotential = float.MinValue;

        foreach (var motion in motions)
        {
            Vector3 nextPos = unit.transform.position + motion;
            float potential = ComputePotential(nextPos, unit);
            //Debug.Log($"Unit: {unit.name}, Next Position: {nextPos}, Potential: {potential}");
            /*if (potential < minPotential)
            {
                minPotential = potential;
                bestMove = motion;
            }*/
            if (potential > maxPotential)
            {
                maxPotential = potential;
                bestMove = motion;
            }
        }

        Vector3 target = unit.transform.position + bestMove;
        //Debug.Log($"Unit: {unit.name} moving towards {target}");
        unit.transform.position = Vector3.MoveTowards(unit.transform.position, target, movementSpeed * Time.deltaTime);
        //Debug.Log($"Unit: {unit.name} current position: {unit.transform.position}");
    }

    public Vector3 FindBestAttraction(UnitNew unit)
    {
        Vector3 bestMove = Vector3.zero;
        float minPotential = float.MaxValue;

        foreach (var motion in motions)
        {
            Vector3 nextPos = unit.transform.position + motion;
            float potential = ComputePotential(nextPos, unit);
            //Debug.Log($"Unit: {unit.name}, Next Position: {nextPos}, Potential: {potential}");
            if (potential < minPotential)
            {
                minPotential = potential;
                bestMove = motion;
            }
        }

       /* Vector3 target = unit.transform.position + bestMove;
        Debug.Log($"Unit: {unit.name} moving towards {target}");
        unit.transform.position = Vector3.MoveTowards(unit.transform.position, target, movementSpeed * Time.deltaTime);
        Debug.Log($"Unit: {unit.name} current position: {unit.transform.position}");*/

        return bestMove;
    }

    public Vector3 FindBestRepulsion(UnitNew unit)
    {
        Vector3 bestMove = Vector3.zero;
        float maxPotential = float.MinValue;
        foreach (var motion in motions)
        {
            Vector3 nextPos = unit.transform.position + motion;
            float potential = ComputePotential(nextPos, unit);
            //Debug.Log($"Unit: {unit.name}, Next Position: {nextPos}, Potential: {potential}");
            if (potential > maxPotential)
            {
                maxPotential = potential;
                bestMove = motion;
            }
        }

        /* Vector3 target = unit.transform.position + bestMove;
         Debug.Log($"Unit: {unit.name} moving towards {target}");
         unit.transform.position = Vector3.MoveTowards(unit.transform.position, target, movementSpeed * Time.deltaTime);
         Debug.Log($"Unit: {unit.name} current position: {unit.transform.position}");*/

        return bestMove;
    }

    void LogUnitPosition(UnitNew unit)
    {
        Debug.Log($"Unit {unit.name} at position {unit.transform.position}");
    }

    public float ComputePotential(Vector3 position, UnitNew unit)
    {
        UnitNew closestEnemy = GetClosestEnemy(unit);
        if (closestEnemy == null)
        {
            Debug.LogWarning($"Unit: {unit.name} has no enemies");
            return float.MaxValue;
        }

        float attractivePotential = ComputeAttractivePotential(position, closestEnemy.transform.position);
        float repulsivePotential = ComputeRepulsivePotential(position, unit);
        float totalPotential = attractivePotential + repulsivePotential;
        //Debug.Log($"Unit: {unit.name}, Position: {position}, Total Potential: {totalPotential}");
        return totalPotential;
    }

    float ComputeAttractivePotential(Vector3 position, Vector3 goal)
    {
        float attractivePotential = 0.5f * KP * Vector3.Distance(position, goal);
        //Debug.Log($"Attractive Potential to goal {goal}: {attractivePotential}");
        return attractivePotential;
    }

    float ComputeRepulsivePotential(Vector3 position, UnitNew unit)
    {
        float repulsivePotential = 0.0f;

        foreach (var otherUnit in units)
        {
            if (otherUnit != unit)
            {
                float distance = Vector3.Distance(position, otherUnit.transform.position);
                if (distance < robotRadius)
                {
                    repulsivePotential += 0.5f * ETA * Mathf.Pow((1.0f / distance - 1.0f / robotRadius), 2);
                    //Debug.Log($"Repulsive Potential from {otherUnit.name} at distance {distance}: {repulsivePotential}");
                }
            }
        }

        return repulsivePotential;
    }

    UnitNew GetClosestEnemy(UnitNew unit)
    {
        UnitNew closestEnemy = null;
        float closestDistance = float.MaxValue;
        UnitNew.Type counter = UnitNew.Type.Archer;
        if (unit.type == UnitNew.Type.Cavalry)
        {
            counter = UnitNew.Type.Archer;
        }
        if (unit.type == UnitNew.Type.Archer)
        {
            counter = UnitNew.Type.Infantry;
        }
        if (unit.type == UnitNew.Type.Infantry)
        {
            counter = UnitNew.Type.Cavalry;
        }
        foreach (var enemy in enemies)
        {

            if (enemy.type == counter)
            {
                float distance = Vector3.Distance(unit.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            
        }

        //Debug.Log($"Closest enemy for {unit.name} is {closestEnemy?.name ?? "None"} at distance {closestDistance}");
        return closestEnemy;
    }
}
