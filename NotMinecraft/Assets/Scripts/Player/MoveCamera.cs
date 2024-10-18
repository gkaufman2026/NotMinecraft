using UnityEngine;

public class MoveCamera : MonoBehaviour {
    [SerializeField] Transform cameraPos;

    void Update() {
        cameraPos.position = transform.position;
    }
}
