using NetTopologySuite.Geometries;
using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MeleeStats;
using static Utils;


using NoOpArmy.WiseFeline.InfluenceMaps;
using UnityEditor;

public class UnitNew : MonoBehaviour
{



    [Header("Unit state")]
    public UnitState state;
    public UnitCombactState combactState;
    public UnitMovementState movementState;

    [Header("Unit targets")]
    public UnitNew fightingTarget;
    public UnitNew commandTarget;

    public static int NextID_D= 0;
    public static int NextID_A = 0;
    public int ID;
    public bool WallCollided;


    public List<SoldierNew> soldiers;
    public HashSet<UnitNew> fightingAgainst = new HashSet<UnitNew>();

    public ArmyNew army;

    public CUnitNew cunit;

    public int numCols;
    public float soldierDistLateral;
    public float soldierDistVertical;
    public float pathSpeed;

    private bool _isSelected, _inFight;

    public float halfWidth { get { return meleeCollider.size.x / 2f; } }
    public float halfHeight { get { return meleeCollider.size.z / 2f; } }

    //Morale Properties
    public int initialSoldiersCount;
    public float morale = 70; // Base morale
    private float moraleUpdateTime = 5f; // Time in seconds to update morale
    private float lastMoraleUpdateTime = 0;

    public enum MoraleState { Impetuous = 1, Eager = 2, Confident = 3, Steady, Wavering = 4, Routing = 5 }
    public MoraleState currentMoraleState;

    private int waveringFrameCounter = 0;
    private const int framesBeforeRecovery = 10;

    private bool decreasedFor90 = false;
    private bool decreasedFor80 = false;
    private bool decreasedFor20 = false;

    public float lost_precentage = 0.0f;

    public enum Type {Archer = 1, Infantry = 2, Cavalry = 3}
    public Type type;

    private void InitializeMorale()
    {
        UpdateMoraleState();
    }

    public void UpdateMoraleState()
    {
        if (morale > 100) currentMoraleState = MoraleState.Impetuous;
        else if (morale >= 90) currentMoraleState = MoraleState.Eager;
        else if (morale >= 65) currentMoraleState = MoraleState.Confident;
        else if (morale >= 30) currentMoraleState = MoraleState.Steady;
        else if (morale >= 0) currentMoraleState = MoraleState.Wavering;
        else {
            currentMoraleState = MoraleState.Routing;
            state = UnitState.ESCAPING;
        }
    }

    private void RecoverMorale()
    {
        int away = 0;
        if (army.role == ArmyRole.DEFENDER)
        {
            away = -1;
        }
        else away = 1;
        Vector3 backwardDirection = new Vector3(this.position.x, this.position.y, 75 * away).normalized;
        Vector3 newPosition = this.position + backwardDirection * 20f;
        this.cunit.MoveAt(newPosition, backwardDirection);
        morale += 10; // Recover morale
        Debug.Log("revocer");
        lastMoraleUpdateTime = Time.time;
        UpdateMoraleState();
    }

    //need to find a way to pass in the loss percentage
    public void DecreaseMoraleOnLoss()
    {
        float remainPercentage = (float)soldiers.Count / initialSoldiersCount * 100f;

        if (remainPercentage <= 90 && !decreasedFor90)
        {
            morale -= 5;
            decreasedFor90 = true;
        }
        else if (remainPercentage <= 80 && !decreasedFor80)
        {
            morale -= 55;
            decreasedFor80 = true;
        }
        else if (remainPercentage <= 20 && !decreasedFor20)
        {
            morale -= 60;
            decreasedFor20 = true;
        }

        UpdateMoraleState();
    }

    public void ResetMoraleDecreaseFlags()
    {
        if ((float)soldiers.Count / initialSoldiersCount * 100f > 90)
        {
            decreasedFor90 = false;
        }
        if ((float)soldiers.Count / initialSoldiersCount * 100f > 70)
        {
            decreasedFor80 = false;
        }
        if ((float)soldiers.Count / initialSoldiersCount * 100f > 20)
        {
            decreasedFor20 = false;
        }
    }

    //need to call when specific event is triggered
    public void ApplyMoraleBuff(float buffAmount)
    {
        morale += buffAmount;
        UpdateMoraleState();
    }

    //need to call when specific event is triggered
    public void ApplyMoraleDebuff(float debuffAmount)
    {
        morale -= debuffAmount;
        UpdateMoraleState();
    }


    public Coordinate[] rectangle
    {
        get
        {
            var pp = new Vector2(position.x, position.z);
            var ff = new Vector2( (transform.forward * halfHeight).x, (transform.forward * halfHeight).z);
            var rr = new Vector2( (transform.right * halfWidth).x, (transform.right * halfWidth).z);

            return new Coordinate[]
            {
                new Coordinate((float)(pp+ff+rr).x, (float)(pp+ff+rr).y),
                new Coordinate((float)(pp-ff+rr).x, (float)(pp-ff+rr).y),
                new Coordinate((float)(pp-ff-rr).x, (float)(pp-ff-rr).y),
                new Coordinate((float)(pp+ff-rr).x, (float)(pp+ff-rr).y),
                new Coordinate((float)(pp+ff+rr).x, (float)(pp+ff+rr).y)
            };
        }
    }




    public int numOfSoldiers 
    { get { return soldiers.Count; } }
    public string soldierLayerName
    {
        get { return "unitSoldier" + ((int)army.role + 1); }  // 1 is the attacker and 2 is the defender
    }
    public bool isSelected
    {
        get { return _isSelected; }
        set
        {
            if (isSelected != value)
            {
                if (value)
                {
                    var color = soldiers.First().transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                    color.b = 1;
                    foreach(var s in soldiers)
                        s.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
                }
                else
                {
                    var color = soldiers.First().transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
                    color.b = 0.21f;
                    foreach (var s in soldiers)
                        s.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;

                }
                _isSelected = value;
            }
        }
    }
    public bool isInFight
    {
        get { return _inFight; }
        set
        {
            if (value && !_inFight) // If it was not fighting before but now yes
            {
                //targetPos = position;
                //targetDirection = transform.forward;
            }
            if (!value && _inFight)
            {
                if (state != UnitState.ESCAPING)
                    state = UnitState.IDLE;
            }
            _inFight = value;
        }
    }
    public Vector3 position
    {
        get { return _position; }
        set { transform.position = value; }
    }
    public Vector3 direction
    {
        get { return _direction; }
        set { transform.rotation = Quaternion.LookRotation(value); }
    }
    private Vector3 _position, _direction;

    public BoxCollider meleeCollider;

    public LineRenderer lr;

    protected void OnDrawGizmos()
    {
        if (!army.DEBUG_MODE) return;

        Gizmos.DrawSphere(position + Vector3.up * 5, 0.1f);
        Gizmos.DrawRay(position + Vector3.up * 5, direction);

        Gizmos.color = Color.red;
        foreach (var s in soldiers)
        {
            Gizmos.DrawRay(s.position, s.direction);
            Gizmos.DrawLine(s.position, s.targetPos);
        }     
    }

    public void UpdateMeleeCollider() // called when soldiers die
    {
        var frontExp = CalculateFrontalExpansion(soldierDistVertical, numOfSoldiers, numCols, army.expansion);
        var latExp = CalculateLateralExpansion(soldierDistLateral, numCols, army.expansion);
        
        // Moving forward to compensate the reduction in collider size
        var frontalDisff = (meleeCollider.size.z - 2*frontExp);

        transform.position += transform.forward * frontalDisff / 2;
        meleeCollider.size = new Vector3(2 * latExp, meleeCollider.size.y, 2 * frontExp);
    }

   


    #region INSTATIONATION STUFF    
    public void Instantiate(Vector3 pos, Vector3 dir, MeleeStatsHolder meleeStats, Transform soldiersHolder, GameObject soldierPrefab, ArmyNew army)
    {
        
        
        position = pos;
        direction = dir;

        SetLinks(meleeStats, army);
        InstantiateSoldiers(pos, dir, meleeStats, soldierPrefab, soldiersHolder);

        var pC = new GameObject("PathCreator").AddComponent<PathCreator>();
        pC.transform.parent = army.pathCreatorsHolder.transform;
        //pC.transform.localPosition = Vector3.zero;
        //pC.transform.localRotation = Quaternion.identity;

        cunit = gameObject.AddComponent<CUnitNew>();
        cunit.Initialize(this, pC, meleeStats.noise, meleeStats.attackingFactor);

        // Log the ID and any other relevant information
        //Debug.Log("Unit instantiated with ID: " + ID + " at position: " + pos);

    }
    private void SetLinks(MeleeStatsHolder meleeStats, ArmyNew army)
    {
        this.army = army;
        numCols = meleeStats.startingCols;
        soldierDistLateral = meleeStats.soldierDistLateral;
        soldierDistVertical = meleeStats.soldierDistVertical;
        pathSpeed = meleeStats.pathSpeed;
    }
    private void InstantiateSoldiers(Vector3 pos, Vector3 dir, MeleeStatsHolder meleeStats, GameObject soldierPrefab, Transform soldiersHolder)
    {
        soldiers = new List<SoldierNew>(meleeStats.startingNumOfSoldiers);
        var res = GetFormationAtPos(pos, dir, meleeStats.startingNumOfSoldiers, numCols, soldierDistLateral, soldierDistVertical);
        GameObject g;
        foreach (Vector3 p in res)
        {
            g = Instantiate(soldierPrefab, p, Quaternion.LookRotation(dir), soldiersHolder);
            g.layer = LayerMask.NameToLayer(soldierLayerName);
            var s = g.GetComponent<SoldierNew>();
            s.Initialize(this, meleeStats, pos, dir);
            soldiers.Add(s);
        }


        //morale needed
        initialSoldiersCount = soldiers.Count;
        
    }
    #endregion


    public bool letItPass;
    private void Update()
    {
        
        _position = transform.position;
        _direction = transform.forward;

        if (!meleeCollider)
            meleeCollider = GetComponentInChildren<BoxCollider>();


        if (!isInFight && !letItPass && soldiers.Select(s => s.allyCollision).Sum() > 0)
        {
            letItPass = true;
            StartCoroutine(JustLetItPass());
        }


        DecreaseMoraleOnLoss();
        
        
        if (currentMoraleState == MoraleState.Wavering)
        {
            //Debug.Log("wavering");
/*            Vector3 backwardDirection = -this.transform.forward;
            Vector3 newPosition = this.position + backwardDirection.normalized * 20f;
            newPosition.y = this.position.y;
            newPosition.x = Mathf.Clamp(newPosition.x, -70, 70);
            newPosition.z = Mathf.Clamp(newPosition.z, -70, 70);
            this.cunit.MoveAt(newPosition, backwardDirection);*/




            waveringFrameCounter++; // Increment the wavering counter
            if (waveringFrameCounter >= 100) // Check if it has been wavering for 10 frames
            {

                RecoverMorale();
                waveringFrameCounter = 0; // Reset counter after recovery starts
            }
            
        }
        else
        {
            waveringFrameCounter = 0; // Reset counter if not wavering
        }
        
        lost_precentage = numOfSoldiers / initialSoldiersCount;
            
    }
    

    
    WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
    private IEnumerator JustLetItPass()
    {
        var oldLateral = soldierDistLateral;
        var oldVert = soldierDistVertical;

        while (soldiers.Select(s => s.allyCollision).Sum() != 0)
        {
            soldierDistLateral = soldierDistLateral < 1.5f ? soldierDistLateral * 1.005f : soldierDistLateral;
            soldierDistVertical = soldierDistVertical < 1.5f ? soldierDistVertical * 1.005f : soldierDistVertical;
            yield return wfeof;
        }

        
        soldierDistLateral = oldLateral;
        soldierDistVertical = oldVert;
        letItPass = false;
    }




}
