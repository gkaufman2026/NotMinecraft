using UnityEngine;

public class CameraTest : MonoBehaviour {
    public float movementSpeed = 10f, fastMovementSpeed = 25f, sensitivity = 3f;
    private bool isLooking = false;

    private Vector2 rot;

    void Update() {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float movementSpeed = isSprinting ? fastMovementSpeed : this.movementSpeed;

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
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            rot.x -= mouseY;
            rot.y += mouseX;
            rot.x = Mathf.Clamp(rot.x, -90f, 90f);

            transform.localRotation = Quaternion.Euler(rot.x, rot.y, 0f);
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