using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombactManagerNew : MonoBehaviour
{

    public ArmyNew attacker, defender;
    private int initialDefenderCount;
    private int initialAttackerCount;
    //public field attackerField;
    //public field defenderField;

    public List<UnitNew> unitsAttacker, unitsDefender;
    public static List<UnitNew> allUnits;

    private float startTime;


    void Start()
    {
        startTime = Time.time;

        attacker.InstantiateArmy();
        defender.InstantiateArmy();

        unitsAttacker = attacker.units;
        unitsDefender = defender.units;
        allUnits = unitsAttacker.Concat(unitsDefender).ToList();

        initialDefenderCount = unitsDefender.Sum(unit => unit.soldiers.Count);
        initialAttackerCount = unitsAttacker.Sum(unit => unit.soldiers.Count);

        /*       attackerField.InitializeField(attacker, defender);
               defenderField.InitializeField(defender, attacker);*/
        attacker.field.InitializeField(attacker, defender);
        defender.field.InitializeField(defender, attacker);

        StartCoroutine(UpdateCombactManager());
    }

    private bool CheckGameEndCondition()
    {
        int currentDefenderCount = unitsDefender.Sum(unit => unit.soldiers.Count);
        bool defendersReduced = currentDefenderCount <= initialDefenderCount * 0.3;
        int currentAttackerCount = unitsDefender.Sum(unit => unit.soldiers.Count);
        bool attackersReduced = currentAttackerCount <= initialAttackerCount * 0.3;
        bool timeElapsed = (Time.time - startTime) >= 300; // 480 seconds = 8 minutes

        return defendersReduced || timeElapsed || attackersReduced;
    }

/*    public void Reset()
    {
        //EndGame();
        startTime = Time.time;

        attacker.InstantiateArmy();
        defender.InstantiateArmy();

        unitsAttacker = attacker.units;
        unitsDefender = defender.units;
        allUnits = unitsAttacker.Concat(unitsDefender).ToList();

        initialDefenderCount = unitsDefender.Sum(unit => unit.soldiers.Count);
*//*
        attackerField.InitializeField(attacker, defender);
        defenderField.InitializeField(defender, attacker);*//*
        attacker.field.InitializeField(attacker, defender);
        defender.field.InitializeField(defender, attacker);

        StartCoroutine(UpdateCombactManager());
    }*/
    private void EndGame()
    {
        
        Debug.Log("Game Over: Defender or Attacker forces are reduced below 20% or reach 5 mins");
        //Application.Quit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UnitNew.NextID_A = 0;
        UnitNew.NextID_D = 0;

    }

    private HashSet<SoldierNew> prova = new HashSet<SoldierNew>();
    private HashSet<SoldierNew> deads = new HashSet<SoldierNew>();
    private HashSet<UnitNew> deadUnits = new HashSet<UnitNew>();

    private void CheckForDeads()
    {

        deadUnits.Clear();
        foreach (UnitNew u in allUnits)
        {
            deads.Count();
            if (u.soldiers.Count == 0)
            {
                deadUnits.Add(u);

                if (attacker.units.Contains(u))
                    attacker.RemoveUnit(u);
                else
                    defender.RemoveUnit(u);


                continue;
            }


            deads.Clear();
            foreach(var s in u.soldiers)
                if (s.health < 0){
                    deads.Add(s);}
                    
            
            
            
            
            foreach(var d in deads)
            {
                u.soldiers.Remove(d);
                Destroy(d.gameObject);
            }
            if (deads.Count > 0)
                u.UpdateMeleeCollider();



            if (u.fightingAgainst.Count > 0)
                AddSoldiersTargets(u);

        }

        foreach(var du in deadUnits)
        {
            if (unitsAttacker.Contains(du))
                unitsAttacker.Remove(du);
            else
                unitsDefender.Remove(du);
            allUnits.Remove(du);

            StartCoroutine(DestroyUnitCO(du));

        }
    }

    private IEnumerator DestroyUnitCO(UnitNew du)
    {
        du.position = -Vector3.up * 100;
        yield return new WaitForSeconds(10);
        Destroy(du);
    }



    private void AddSoldiersTargets(UnitNew u)
    {
        prova.Clear();
        foreach (var enemyUnit in u.fightingAgainst)
            foreach (var es in enemyUnit.soldiers)
                prova.Add(es);

        if (prova.Count == 0) return;

        foreach (var s in u.soldiers)
            s.enemySoldierPosition = prova.OrderBy(es => Vector3.SqrMagnitude(es.position - s.position)).First().position;
    }


    private struct UpdateCUnitJOB : IJob
    {
        public int i;

        public void Execute()
        {
            allUnits[i].cunit.UnitUpdate();
        }
    }

    private void UpdateCUnit()
    {
        //NativeArray<JobHandle> handles = new NativeArray<JobHandle>(allUnits.Count, Allocator.TempJob);

        //for (int i=0; i<allUnits.Count; i++)
        //{
        //    allUnits[i].cunit.CalculateVariablesBeforeScheduling();
        //    UpdateCUnitJOB jobData = new UpdateCUnitJOB { i = i};
        //    handles[i] = jobData.Schedule();
        //}

        //JobHandle jh = JobHandle.CombineDependencies(handles);
        //jh.Complete();
        //handles.Dispose();


        // For some weird reason this is much faster
        foreach (var u in allUnits)
            u.cunit.UnitUpdate();
    }



    private WaitForEndOfFrame wfeof;
    IEnumerator UpdateCombactManager()
    {
        wfeof = new WaitForEndOfFrame();


        int k = 0;
        while (true)
        {

            if (k < 5)
            {
                k++;
                yield return wfeof;
            }
            else
                k = 0;

            //placeholder to put the function to check for the specific morale setting
            //and set the moving direction of the unit

            CheckForDeads();
            UpdateCUnit();

            if (CheckGameEndCondition())
            {
                EndGame();
                break;
            }
            
        }
    }
}
