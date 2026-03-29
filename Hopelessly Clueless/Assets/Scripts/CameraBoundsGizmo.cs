using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBoundsGizmo : MonoBehaviour
{
    private Camera cam;

    private void OnDrawGizmos()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // 1. Pick a bright color that stands out against your background
        Gizmos.color = Color.cyan;

        // 2. Do the math to find the edges of your 2D screen
        float height = cam.orthographicSize * 2;
        float width = height * cam.aspect;

        // 3. Draw a permanent box right where the camera is looking!
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }
}
