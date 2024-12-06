using UnityEngine;

public class PlayerLook : MonoBehaviour {
    [SerializeField] private Vector2 sensitivity = new(100f, 100f);
    [SerializeField] Transform head;

    private Vector2 mouse, rot;
    private const float multiplier = 0.01f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        mouse.x = Input.GetAxis("Mouse X");
        mouse.y = Input.GetAxis("Mouse Y");

        rot.y += mouse.x * sensitivity.x * multiplier;
        rot.x -= mouse.y * sensitivity.y * multiplier;

        rot.x = Mathf.Clamp(rot.x, -90f, 90f);

        gameObject.transform.rotation = Quaternion.Euler(0, rot.y, 0);
        head.transform.rotation = Quaternion.Euler(-rot.x, rot.y, 0);
    }
}
