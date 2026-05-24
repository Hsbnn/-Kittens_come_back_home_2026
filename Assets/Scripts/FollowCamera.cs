using UnityEngine;

public sealed class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 7.5f, -8.5f);
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0f, 1.05f, 1.18f);
    [SerializeField] private float followSharpness = 9f;
    [SerializeField] private float lookHeight = 1f;
    [SerializeField] private float firstPersonLookHeight = 0.82f;

    private bool firstPersonMode;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private void Start()
    {
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            firstPersonMode = !firstPersonMode;
            GameUI.ShowMessage(firstPersonMode ? "Камера: вид от первого лица кота. Tab - вернуть обычную." : "Камера: обычный вид сбоку. Tab - вид глазами кота.", 3f);
            SnapToTarget();
        }

        if (firstPersonMode)
        {
            Camera camera = GetComponent<Camera>();
            if (camera != null)
            {
                camera.nearClipPlane = 0.02f;
            }

            Vector3 firstPersonPosition = target.TransformPoint(firstPersonOffset);
            transform.position = Vector3.Lerp(transform.position, firstPersonPosition, Time.deltaTime * followSharpness * 1.4f);
            Vector3 lookTarget = target.position + target.forward * 7f + Vector3.up * firstPersonLookHeight;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position, Vector3.up), Time.deltaTime * followSharpness * 1.4f);
            return;
        }

        Camera thirdPersonCamera = GetComponent<Camera>();
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.nearClipPlane = 0.1f;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSharpness);
        transform.LookAt(target.position + Vector3.up * lookHeight);
    }

    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        if (firstPersonMode)
        {
            transform.position = target.TransformPoint(firstPersonOffset);
            transform.rotation = Quaternion.LookRotation(target.forward + Vector3.up * 0.02f, Vector3.up);
            return;
        }

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * lookHeight);
    }
}
