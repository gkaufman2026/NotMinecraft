using UnityEngine;

public class MoveCamera : MonoBehaviour {
    [SerializeField] Transform cameraPos;

    void Update() {
        transform.position = cameraPos.position;
    }
}
