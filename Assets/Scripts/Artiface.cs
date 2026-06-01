using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Artiface : MonoBehaviour
{
    #region Basic AI Settings
    public enum aiType { Enemy, Mob };
    public aiType agentType;

    [Header("Capabilities")]
    public NavMeshAgent entityNavAgent;
    public bool hearingEnabled;
    public bool visionEnabled;
    public bool wanderEnabled;
    #endregion

    #region Behaviour Vars
    public enum behaviours { Wander, Chase, Search, Investigate, Attack };
    public behaviours currentBehaviour;

    #endregion

    #region FOV Vars
    [Header("Field Of View")]
    public float visionTimer;
    public float VisionTimerGoal;
    public float radius;
    [Range(0, 360)] public float angle;
    public LayerMask targetMask, obstructionMask;
    public bool canSeePlayer;
    public GameObject currentVisualTarget;
    private Transform _transform;
    #endregion

    #region Wander Variables
    [Header("Wander")]
    public float wanderTimer;
    public float wanderTimerGoal;
    public float wanderRange;
    public Vector3 wanderTarget;
    public float MaxTargetHeight;
    #endregion

    #region Hearing Variables
    [Header("Hearing")]
    public float hearingTimer;
    public float hearingTimerGoal;
    public float hearingRange;
    public float hearingThreshold;
    public GameObject currentAudioTarget;
    public Vector3 lastHeardLocation;
    public bool canHearPlayer;
    #endregion

    #region search Variables
    [Header("Search")]
    public float searchRange;
    public LayerMask searchMask;
    public List<Collider> searchTargets;
    public GameObject currentSearchTarget;
    #endregion

    #region Interaction Variables
    [Header("Interaction")]
    public float interactableCheckDistance;
    public LayerMask interactableMask;
    #endregion

    #region movementVars
    public float walkSpeed;
    public float runSpeed;
    #endregion

    public float searchTimer;
    public float searchTimerGoal;
    public float searchMinDistance;


    public float investigationRange;
    public LayerMask investigationMask;
    public int investigationAttempts;
    public int maxInvestigationAttempts;
    public GameObject InvestigationTarget;
    Collider[] investigationObjects;
    public float closeInvestigationRange;
    public void Start()
    {
        _transform = transform;
        entityNavAgent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        //Checks
        if (visionEnabled)
        {
            canSeePlayer = FieldOfViewCheck();
        }
        if (hearingEnabled)
        {
            canHearPlayer = playerSoundCheck();
        }






        if (currentBehaviour == behaviours.Chase)
        {
            if (!canSeePlayer)
            {
                currentBehaviour = behaviours.Wander;
                return;
            }
            currentBehaviour = behaviours.Chase;

        }

        if (currentBehaviour == behaviours.Search)
        {
            Search();
            return;
        }

        if (currentBehaviour == behaviours.Investigate)
        {
            investigate();
            return;
        }

        if (currentBehaviour == behaviours.Wander)
        {
            if (wanderEnabled) WanderRoutine();
        }
    }

    public void Chase()
    {
        if (entityNavAgent.speed != runSpeed)
        {
            entityNavAgent.speed = runSpeed;
        }
        if (currentVisualTarget != null)
        {
            entityNavAgent.destination = currentVisualTarget.transform.position;
        }
    }

    public void Search()
    {

        if (entityNavAgent.speed != runSpeed) entityNavAgent.speed = runSpeed;
        if (Vector3.Distance(transform.position, lastHeardLocation) <= searchMinDistance)
        {
            if (canSeePlayer) return;
            entityNavAgent.destination = transform.position;
            currentBehaviour = behaviours.Investigate;
            return;
        }


        if (currentAudioTarget != null)
        {
            entityNavAgent.destination = lastHeardLocation;
        }
    }

    public void investigate()
    {
        if (investigationAttempts >= maxInvestigationAttempts)
        {
            investigationAttempts = 0;
            currentBehaviour = behaviours.Wander;
        }

        if (InvestigationTarget != null)
        {
            if (Vector3.Distance(transform.position, currentSearchTarget.transform.position) <= searchMinDistance)
            {
                investigationAttempts += 1;
                InvestigationTarget = null;
                if (Physics.OverlapSphere(transform.position, closeInvestigationRange, targetMask).Length < 0)
                {
                    ///insert attack check later
                }



            }
        }

        if (InvestigationTarget == null)
        {   
            Collider[] targetCandidates = Physics.OverlapSphere(transform.position, investigationRange, investigationMask); 
            currentSearchTarget = targetCandidates[Random.Range(0, targetCandidates.Length)].transform.gameObject;
        }


    }

    #region FOV
    private bool FieldOfViewCheck()
    {
        if (visionTimer < VisionTimerGoal)
        {
            visionTimer += Time.deltaTime * NyanManager.instance.timerRate;
            return false;
        }

        visionTimer = 0;

        List<GameObject> players = NyanManager.instance.PlayerRef;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;

            Vector3 dirToPlayer = (players[i].transform.position - _transform.position).normalized;
            float distToPlayer = Vector3.Distance(_transform.position, players[i].transform.position);

            if (distToPlayer > radius) continue;
            if (Vector3.Angle(_transform.forward, dirToPlayer) > angle / 2) continue;
            if (Physics.Raycast(_transform.position, dirToPlayer, distToPlayer, obstructionMask)) continue;

            // First valid player found
            currentVisualTarget = players[i];
            return true;
        }

        currentVisualTarget = null;
        return false;
    }
    #endregion

    #region wander Behaviour
    public void WanderRoutine()
    {
        if (entityNavAgent.speed != walkSpeed) entityNavAgent.speed = walkSpeed;

        if (wanderTimer < wanderTimerGoal)
        {
            wanderTimer += Time.deltaTime * NyanManager.instance.timerRate;
            return;
        }

        wanderTarget = GenerateWanderTarget();
        RaycastHit rayhit;
        if (!Physics.Raycast(wanderTarget, new Vector3(wanderTarget.x, wanderTarget.y - MaxTargetHeight, wanderTarget.z), out rayhit, 10f))
        {
            wanderTarget = GenerateWanderTarget();
            return;
        }
        wanderTimer = 0;
        entityNavAgent.SetDestination(wanderTarget);

    }

    public Vector3 GenerateWanderTarget()
    {
        return transform.position + new Vector3(Random.Range(-wanderRange, wanderRange), Random.Range(-5f, 5f), Random.Range(-wanderRange, wanderRange));
    }
    #endregion

    #region Hearing Behaviour
    public bool playerSoundCheck()
    {
        if (hearingTimer < hearingTimerGoal)
        {
            hearingTimer += Time.deltaTime * NyanManager.instance.timerRate;
            return canHearPlayer;
        }
        hearingTimer = 0;
        List<GameObject> players = NyanManager.instance.PlayerRef;
        Collider[] surroundings = Physics.OverlapSphere(_transform.position, hearingRange, targetMask);
        if (surroundings.Length == 0) return false;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;
            if (NyanManager.instance.playerMagnitude[i] < hearingThreshold) continue;

            // First player loud enough
            lastHeardLocation = players[i].transform.position;
            currentAudioTarget = players[i];
            return true;
        }

        currentAudioTarget = null;
        return false;
    }
    #endregion

    #region Debugging
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(wanderTarget, new Vector3(1, 1, 1));
        Gizmos.DrawLine(wanderTarget, new Vector3(wanderTarget.x, wanderTarget.y - MaxTargetHeight, wanderTarget.z));
        Gizmos.color = Color.pink;
        Gizmos.DrawWireSphere(transform.position, wanderRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Gizmos.DrawWireSphere(lastHeardLocation, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }
    #endregion
}