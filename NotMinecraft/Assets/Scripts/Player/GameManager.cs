using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3Int playerPos;
    public Vector3 playerSpawnPos;
    public World world;

    [SerializeField] private Vector3Int currentChunkCenter = Vector3Int.zero;
    private GameObject player;

    public void SpawnPlayer() {
        if (player != null) return;

        if (world != null) {
            player = Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
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
