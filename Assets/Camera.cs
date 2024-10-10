using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float sensitivityX = 2.0f;
    [SerializeField] private float sensitivityY = 2.0f;
    [SerializeField] private float maxXRot = 90f;
    [SerializeField] private float minXRot = -75f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float WallDistance = 0.4f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, 0);
    [SerializeField] private float initialDistance;

    private float rotationX = 0;
    private float rotationY = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        HandleCameraPosition();
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        rotationX -= mouseY;
        rotationY += mouseX;

        rotationX = Mathf.Clamp(rotationX, minXRot, maxXRot);

        Quaternion cameraRotation = Quaternion.Euler(rotationX, rotationY, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleCameraPosition()
    {
        Vector3 offset = player.position - transform.forward * distance + cameraOffset;
        RaycastHit hit;

        if (Physics.Raycast(player.position, -transform.forward, out hit, distance))
        {
            offset = hit.point + transform.forward * WallDistance;
        }

        transform.position = offset;
    }
}
