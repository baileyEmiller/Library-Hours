//Bailey Miller, 4/23/25
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public static float offsetnum = 10f;
    public Vector3 offset = new Vector3(0, offsetnum, 0);
    public float followSpeed = 100000f; //if you want a delay in camera follow, lower number

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
