using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    public Transform pathHolder;
    public float speed = 3;
    public float waitTime = .1f;
    public float turnSpeed = 90; // 90 deg per sec

    [SerializeField] private Animator animator;

    public Light spotlight;
    public float viewDistance;
    private float viewAngle;

    private Transform player;
    
    // original spotlight color if player cannot be seen
    private Color originalSpotlightColour;
    
    public LayerMask viewMask;

    void OnDrawGizmos() {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPisition = startPosition;
        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPisition, waypoint.position);
            previousPisition = waypoint.position;
        }

        Gizmos.DrawLine(previousPisition, startPosition);

        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;

        originalSpotlightColour = spotlight.color;
        
        
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));
    }

    IEnumerator TurnToFace(Vector3 lookTarget) {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        while ( Mathf.Abs( Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) ) > 0.05f ) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }


    IEnumerator FollowPath(Vector3[] waypoints) {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint) {

                // a % b ==> when a == b, go back to 0 :)
                targetWaypointIndex = (targetWaypointIndex +1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                // yield return new WaitForSeconds(waitTime);
                yield return null;
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
            
        } // while
    } // FollowPath

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            spotlight.color = Color.red;
            animator.SetBool("Attacking", true);
        }
        else
        {
            spotlight.color = originalSpotlightColour;
            animator.SetBool("Attacking", false);
        }

        /*if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("Attacking", true);
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            animator.SetBool("Attacking", false);
        }*/

    }


    private bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }

        return false;
    }


}
