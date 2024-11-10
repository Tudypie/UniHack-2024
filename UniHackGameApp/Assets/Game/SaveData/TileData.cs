using System;
using UnityEngine;

[Serializable]
public class TileData 
{
    public Vector3 position;
    public Quaternion rotation;

    public Tile.Wall walls;

    public bool hasCheese;
    public bool hasTrap;
    public bool hasPlate;
}
