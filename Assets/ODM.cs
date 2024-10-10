using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ODM : MonoBehaviour
{

    [Header("Line Renderers")]
    [SerializeField] private LineRenderer lr;

    [Header("Grappling Settings")]
    [SerializeField] private LayerMask ODMGearLayer;
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [HideInInspector] public Transform ODMGearPoint;
    [SerializeField] private GameObject ODMGearPointPrefab;
    [SerializeField] private float maxDistance = 170f;
    [SerializeField] private float ODMGearVelocity = 10;
    [SerializeField] private float jointTolerance;
    [SerializeField] private ODM odm;

    [Header("Keycode")]
    [SerializeField] private KeyCode grappling = KeyCode.Mouse0;
    [SerializeField] private KeyCode rewind;


    private Rigidbody playerRb;
    [HideInInspector] public SpringJoint joint;

    void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    private void Update()
    {
        HandleInput();

        if (joint)
            UpdateJoint();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(rewind))
            Rewind();
        else if (!playerRb.useGravity && !Input.GetKey(odm.rewind))
            playerRb.useGravity = true;

    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(grappling))
            StartODMGear();

        if (Input.GetKeyUp(grappling))
            StopODMGear();
    }

    private void Rewind()
    {
        if (!joint)
            return;

        playerRb.useGravity = false;

        Vector3 direction = (ODMGearPoint.position - player.position).normalized;
        playerRb.AddForce(direction * ODMGearVelocity * Time.deltaTime);
    }

    private void StartODMGear()
    {

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, maxDistance, ODMGearLayer))
        {

            if (ODMGearPoint == null)
            {
                Transform odmGearPoint = Instantiate(ODMGearPointPrefab.transform);
                odmGearPoint.name = "ODMGearPoint";
                ODMGearPoint = odmGearPoint;
            }

            ODMGearPoint.position = hit.point;
            ODMGearPoint.parent = hit.transform;

            player.transform.LookAt(ODMGearPoint);
            CreateSpringJoint();
            SetConnectedAunchor();
            DrawRope();
        }

    }

    public void StopODMGear()
    {
        lr.positionCount = 0;

        if (joint != null)
        {
            Destroy(joint);
        }
    }

    private void CreateSpringJoint()
    {
        if (joint != null)
            return;

        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = ODMGearPoint.position;
        joint.spring = 4.5f;
        joint.damper = 10;
        joint.massScale = 4.5f;
        joint.tolerance = jointTolerance; 
        joint.maxDistance = Vector3.Distance(player.position, ODMGearPoint.position); ;
        joint.minDistance = 0;
    }

    private void SetConnectedAunchor()
    {
        if (!joint)
            return;

        joint.connectedAnchor = ODMGearPoint.position;
    }

    private void UpdateJoint()
    {
        joint.maxDistance = Vector3.Distance(player.position, ODMGearPoint.position);

        if (joint.maxDistance < 0)
            joint.maxDistance = 0;
    }

    private void DrawRope()
    {
        if (joint == null)
        {
            lr.positionCount = 0;
            return;
        }

        if (lr.positionCount != 2)
        {
            lr.positionCount = 2;
        }

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, ODMGearPoint.position);
    }

    void OnValidate()
    {
        if (odm != null)
        {
            odm.odm = this;
        }
    }
}


