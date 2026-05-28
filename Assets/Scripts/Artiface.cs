using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Artiface : MonoBehaviour
{
    public float timerRate;
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
    public enum behaviours { Wander, Chase, Search, Attack };
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
    private Transform _transform;
    private readonly Collider[] _hits = new Collider[1];
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

    public void Start()
    {
        _transform = transform;
        entityNavAgent = GetComponent<NavMeshAgent>();
        if (visionEnabled)
        {
            InvokeRepeating(nameof(FieldOfViewCheck), 0f, 0.2f);
        }
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





    }

    #region FOV
    private bool FieldOfViewCheck()
    {
        bool sightCheck = false;
        if (visionTimer >= VisionTimerGoal)
        {

            if (Physics.OverlapSphereNonAlloc(_transform.position, radius, _hits, targetMask) == 0)
            {
                return sightCheck;
            }
            Vector3 dir = (_hits[0].transform.position - _transform.position).normalized;
            sightCheck = Vector3.Angle(_transform.forward, dir) < angle / 2 && !Physics.Raycast(_transform.position, dir, Vector3.Distance(_transform.position, _hits[0].transform.position), obstructionMask);
            visionTimer = 0;
            return sightCheck;
        }
        visionTimer += Time.deltaTime * timerRate;
        return sightCheck;
    }
    #endregion

    #region wander Behaviour
    public void WanderRoutine()
    {
        if (wanderTimer < wanderTimerGoal)
        {
            wanderTimer += Time.deltaTime * timerRate;
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

        if (hearingTimer >= hearingTimerGoal)
        {
            hearingTimer = 0;
            Collider[] surroundings = Physics.OverlapSphere(transform.position, hearingRange, targetMask);
            if (surroundings.Length > 0)
            {
                for (int i = 0; i < NyanManager.instance.PlayerRef.Count; i++)
                {
                    if (NyanManager.instance.playerMagnitude[i] >= hearingThreshold)
                    {
                        lastHeardLocation = NyanManager.instance.PlayerRef[i].transform.position;
                        return true;
                    }

                }
            }
            return false;
        }
        hearingTimer += Time.deltaTime * timerRate;
        return canHearPlayer;
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