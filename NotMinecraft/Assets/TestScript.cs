using UnityEngine;

public class TestScript : MonoBehaviour {

    [SerializeField] private KeyCode addBlockKB = KeyCode.B;
    [SerializeField] private World world;
    [SerializeField] private GameObject player;

    private void FixedUpdate() {
        if (player == null) {
            player = GetComponent<GameManager>().player;
        }
    }

    void Update() {
        if (Input.GetKey(addBlockKB)) {
            
        }
    }
}
