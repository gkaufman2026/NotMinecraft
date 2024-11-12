using System;
using UnityEngine;

public static class DirectionExtensions {
    public static Vector3Int GetVector(this Direction direction) {
        return direction switch {
            Direction.UP => Vector3Int.up,
            Direction.DOWN => Vector3Int.down,
            Direction.RIGHT => Vector3Int.right,
            Direction.LEFT => Vector3Int.left,
            Direction.FORWARD => Vector3Int.forward,
            Direction.BACKWARDS => Vector3Int.back,
            _ => throw new Exception("Invalid input direction")
        };
    }
}