using UnityEngine;

public static class BlockHelper {
    public static Direction[] directions = {
        Direction.BACKWARDS,
        Direction.DOWN,
        Direction.FORWARD,
        Direction.LEFT,
        Direction.RIGHT,
        Direction.UP
    };

    public static MeshData GetMeshData(ChunkData chunk, Vector3Int coords, MeshData meshData, BlockType blockType) {
        if (blockType == BlockType.AIR || blockType == BlockType.NOTHING) {
            return meshData;
        }

        foreach (Direction direction in directions) {
            var updatedCoords = coords + direction.GetVector();
            var type = Chunk.GetBlockFromChunkCoordinates(chunk, updatedCoords);

            if (type != BlockType.NOTHING && !BlockDataManager.textureList[type].isSolid) {
                if (type == BlockType.WATER || type == BlockType.AIR) {
                    meshData.waterMesh = GetFaceDataIn(direction, coords, meshData.waterMesh, blockType);
                } else {
                    meshData = GetFaceDataIn(direction, coords, meshData, type);
                }
            }
        }
        return meshData;
    }


    public static MeshData GetFaceDataIn(Direction direction, Vector3Int coords, MeshData meshData, BlockType blockType) {
        GetFaceVertices(direction, coords, meshData, blockType);
        meshData.AddQuadTriangles(BlockDataManager.textureList[blockType].hasCollider);
        meshData.uv.AddRange(FaceUVs(direction, blockType));
        return meshData;
    }

    public static void GetFaceVertices(Direction direction, Vector3Int coords, MeshData meshData, BlockType blockType) {
        var generatesCollider = BlockDataManager.textureList[blockType].hasCollider;
        switch (direction) {
            case Direction.BACKWARDS:
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y - 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y + 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y + 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y - 0.5f, coords.z - 0.5f), generatesCollider);
                break;
            case Direction.FORWARD:
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y - 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y + 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y + 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y - 0.5f, coords.z + 0.5f), generatesCollider);
                break;
            case Direction.LEFT:
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y - 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y + 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y + 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y - 0.5f, coords.z - 0.5f), generatesCollider);
                break;
            case Direction.RIGHT:
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y - 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y + 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y + 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y - 0.5f, coords.z + 0.5f), generatesCollider);
                break;
            case Direction.DOWN:
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y - 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y - 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y - 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y - 0.5f, coords.z + 0.5f), generatesCollider);
                break;
            case Direction.UP:
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y + 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y + 0.5f, coords.z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x + 0.5f, coords.y + 0.5f, coords.z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(coords.x - 0.5f, coords.y + 0.5f, coords.z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType) {
        Vector2[] UVs = new Vector2[4];
        Vector2Int tilePos = TexturePosition(direction, blockType);
        UVs[0] = new Vector2(BlockDataManager.tileSize.x * tilePos.x + BlockDataManager.tileSize.x - BlockDataManager.textureOffset, BlockDataManager.tileSize.y * tilePos.y + BlockDataManager.textureOffset);
        UVs[1] = new Vector2(BlockDataManager.tileSize.x * tilePos.x + BlockDataManager.tileSize.x - BlockDataManager.textureOffset, BlockDataManager.tileSize.y * tilePos.y + BlockDataManager.tileSize.y - BlockDataManager.textureOffset);
        UVs[2] = new Vector2(BlockDataManager.tileSize.x * tilePos.x + BlockDataManager.textureOffset, BlockDataManager.tileSize.y * tilePos.y + BlockDataManager.tileSize.y - BlockDataManager.textureOffset);
        UVs[3] = new Vector2(BlockDataManager.tileSize.x * tilePos.x + BlockDataManager.textureOffset, BlockDataManager.tileSize.y * tilePos.y + BlockDataManager.textureOffset);
        return UVs;
    }

    public static Vector2Int TexturePosition(Direction direction, BlockType blockType) {
        return direction switch {
            Direction.UP => BlockDataManager.textureList[blockType].up,
            Direction.DOWN => BlockDataManager.textureList[blockType].down,
            _ => BlockDataManager.textureList[blockType].side
        };
    }
}