using UnityEngine;

public class CameraSwitcher : MonoBehaviour {
    [SerializeField] public Camera mainCamera;
    [SerializeField] private GameObject player;
    [SerializeField] KeyCode spectactorKey = KeyCode.F3;

    private Camera playerCamera;

    void Start() {
        if (player == null) { return; }
        mainCamera.enabled = true;
        playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null) {
            playerCamera.enabled = false;
        }
    }

    private void FixedUpdate() {
        if (player == null) {
            player = GetComponent<GameManager>().player;
            if (player != null) {
                playerCamera = player.GetComponentInChildren<Camera>();
                if (playerCamera != null) {
                    playerCamera.enabled = true;
                    mainCamera.enabled = false;
                }
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(spectactorKey)) {
            mainCamera.enabled = !mainCamera.enabled;
            mainCamera.GetComponent<CameraTest>().enabled = mainCamera.enabled;
            if (playerCamera != null) {
                playerCamera.enabled = !playerCamera.enabled;
                player.GetComponentInChildren<PlayerMovement>().enabled = playerCamera.enabled;
            }
        }
    }
}
