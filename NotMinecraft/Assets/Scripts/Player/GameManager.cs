using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3Int playerPos;
    public World world;
    public float detectionTime = 1f;

    private Vector3Int currentChunkCenter;
    private GameObject player;

    public void SpawnPlayer() {
        if (player != null) {
            return;
        }
        Vector3Int raycastStartposition = new Vector3Int(world.chunkSize / 2, 100, world.chunkSize / 2);
        RaycastHit hit;
        if (Physics.Raycast(raycastStartposition, Vector3.down, out hit, 120)) {
            Debug.Log("Player made");
            player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
            CheckWorld();
        }
    }

    public void CheckWorld() {
        SetCurrentChunkCoordinates();
        StopAllCoroutines();
        StartCoroutine(ShouldWorldLoadNextPos());
    }

    IEnumerator ShouldWorldLoadNextPos() {
        yield return new WaitForSeconds(detectionTime);
        if (
            Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > world.chunkSize ||
            Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > world.chunkSize ||
            (Mathf.Abs(currentChunkCenter.y - player.transform.position.y) > world.chunkHeight)
            ) {
            world.LoadAdditionalChunksRequest(player);
        } else {
            StartCoroutine(ShouldWorldLoadNextPos());
        }
    }

    private void SetCurrentChunkCoordinates() {
        playerPos = WorldDataHelper.ChunkPosFromBlockCoords(world, Vector3Int.RoundToInt(player.transform.position));
        currentChunkCenter.x = playerPos.x + world.chunkSize / 2;
        currentChunkCenter.z = playerPos.z + world.chunkSize / 2;
    }
}
