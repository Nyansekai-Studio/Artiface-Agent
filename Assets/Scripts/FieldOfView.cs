using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)] public float angle;
    public GameObject playerRef;
    public LayerMask targetMask, obstructionMask;
    public bool canSeePlayer;
    private Transform _transform;
    private readonly Collider[] _hits = new Collider[1];
    private void Start()
    {
        _transform = transform;
        playerRef = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating(nameof(FieldOfViewCheck), 0f, 0.2f);
    }
    private void FieldOfViewCheck()
    {
        if (Physics.OverlapSphereNonAlloc(_transform.position, radius, _hits, targetMask) == 0)
        {
            canSeePlayer = false;
            return;
        }
        Vector3 dir = (_hits[0].transform.position - _transform.position).normalized;
        canSeePlayer = Vector3.Angle(_transform.forward, dir) < angle / 2 && !Physics.Raycast(_transform.position, dir, Vector3.Distance(_transform.position, _hits[0].transform.position),obstructionMask);
    }
}