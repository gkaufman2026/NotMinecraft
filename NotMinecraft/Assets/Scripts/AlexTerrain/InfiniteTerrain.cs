using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    public const float maxViewDistance = 300;
    public Transform player;

    public static Vector2 playerPosition;
    int chunkSize;
    int visibleChunks;

    // Vector for chunk coordinate and terrainChunk as the value
    Dictionary<Vector2, TerrainChunk> chunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> previouslyVisibleTerrainChunks = new List<TerrainChunk>();

    private void Start()
    {
        chunkSize = AlexMapGenerator.mapChunkSize - 1;
        // How many times we can divide the chunk size into view distance
        visibleChunks = Mathf.RoundToInt(maxViewDistance) / chunkSize;
    }

    void Update()
    {
        playerPosition = new Vector2(player.position.x, player.position.z);
        updateVisibleChunks();
    }

    void updateVisibleChunks()
    {
        for (int i = 0; i < previouslyVisibleTerrainChunks.Count; i++)
        {
            previouslyVisibleTerrainChunks[i].setVisibility(false);
        }

        previouslyVisibleTerrainChunks.Clear();

        int chunkXCord = Mathf.RoundToInt(playerPosition.x/chunkSize);
        int chunkYCord = Mathf.RoundToInt(playerPosition.y / chunkSize);

        for (int y = -visibleChunks; y <= visibleChunks; y++)
        {
            for (int x = -visibleChunks; x <= visibleChunks; x++)
            {
                Vector2 viewedChunkCord = new Vector2(chunkXCord + x, chunkYCord + y);

                if (chunkDictionary.ContainsKey(viewedChunkCord))
                {
                    chunkDictionary[viewedChunkCord].updateChunk();
                    if (chunkDictionary[viewedChunkCord].isVisible())
                    {
                        previouslyVisibleTerrainChunks.Add(chunkDictionary[viewedChunkCord]);
                    }
                } else {
                    chunkDictionary.Add(viewedChunkCord, new TerrainChunk(viewedChunkCord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 cord, int size, Transform parent)
        {
            position = cord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3;
            meshObject.transform.localScale = Vector3.one * size / 10f;
            meshObject.transform.parent = parent;

            // Set to invisible by default so the update can handle the change if it is needed
            setVisibility(false);
        }

        public void updateChunk()
        {
            float playerDistanceFromNearestEdge = bounds.SqrDistance(playerPosition);
            bool isVisible = playerDistanceFromNearestEdge <= maxViewDistance;

            setVisibility(isVisible);
        }

        public void setVisibility(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
