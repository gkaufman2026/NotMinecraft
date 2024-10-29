using UnityEngine;

public class CameraTest : MonoBehaviour {
    public float movementSpeed = 10f;
    public float fastMovementSpeed = 25f;
    public float freeLookSensitivity = 3f;
    public float zoomSensitivity = 10f;
    public float fastZoomSensitivity = 50f;
    private bool isLooking = false;

    void Update() {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float movementSpeed = isSprinting ? fastMovementSpeed : this.movementSpeed;
        Vector2 rot;

        if (Input.GetKey(KeyCode.A)) {
            transform.position = transform.position + (movementSpeed * Time.deltaTime * -transform.right);
        }

        if (Input.GetKey(KeyCode.D)) {
            transform.position = transform.position + (movementSpeed * Time.deltaTime * transform.right);
        }

        if (Input.GetKey(KeyCode.W)) {
            transform.position = transform.position + (movementSpeed * Time.deltaTime * transform.forward);
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.position = transform.position + (movementSpeed * Time.deltaTime * -transform.forward);
        }

        if (isLooking) {
            rot.x = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            rot.y = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(rot.y, rot.x, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            StartLooking();
        } else if (Input.GetKeyUp(KeyCode.Mouse1)) {
            StopLooking();
        }
    }

    void OnDisable() {
        StopLooking();
    }

    public void StartLooking() {
        isLooking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StopLooking() {
        isLooking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}