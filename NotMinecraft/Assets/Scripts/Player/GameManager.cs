using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3Int playerPos;
    public Vector3 playerSpawnPos;
    [HideInInspector] public World world;

    [SerializeField] private Vector3Int currentChunkCenter = Vector3Int.zero;
    [HideInInspector] public GameObject player;

    private Camera mainCamera;

    private void Awake() {
        mainCamera = GetComponent<CameraSwitcher>().mainCamera;
    }

    public void SpawnPlayer() {
        if (player != null) return;

        if (world != null) {
            player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
            mainCamera.transform.position = new Vector3(playerSpawnPos.x, playerSpawnPos.y + 5, playerSpawnPos.z); 
            CheckWorld();
        }
    }

    public void CheckWorld() {
        SetCurrentChunkCoordinates();
    }

    private void SetCurrentChunkCoordinates() {
        playerPos = ChunkPositionFromBlockCoords(world, Vector3Int.RoundToInt(player.transform.position));
        currentChunkCenter.x = playerPos.x + world.chunkSize / 2;
        currentChunkCenter.z = playerPos.z + world.chunkSize / 2;
    }

    private static Vector3Int ChunkPositionFromBlockCoords(World world, Vector3Int pos) {
        return new Vector3Int {
            x = Mathf.FloorToInt(pos.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(pos.y / (float)world.chunkHeight) * world.chunkSize,
            z = Mathf.FloorToInt(pos.z / (float)world.chunkSize) * world.chunkSize,
        };
    }
}
