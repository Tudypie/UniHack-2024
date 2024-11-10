using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;



public class Tile : MonoBehaviour
{
    [SerializeField] GameObject cheese;
    [SerializeField] GameObject trap;
    [SerializeField] GameObject plate;

    [SerializeField] List<WallToTransform> wallToTransforms;

    [SerializeField] Wall walls;
    [SerializeField] bool hasCheese;
    [SerializeField] bool hasTrap;
    [SerializeField] bool hasPlayer;
    [SerializeField] bool hasPlate;

    [Serializable]
    public class WallToTransform
    {
        public Wall wall;
        public GameObject gameObject;
    }

    public enum Wall
    {
        None = 0,
        Right = 1,
        Left = 2,
        Front = 4,
        Back = 8,
    }

    public TileData GetData()
    {
        return new TileData
        {
            hasCheese = hasCheese,
            hasPlate = hasPlate,
            hasTrap = hasTrap,
            position = transform.position,
            rotation = transform.rotation,
            wall = walls,
        };
    }

    public void LoadData(TileData data)
    {
        hasCheese = data.hasCheese;
        hasPlate = data.hasPlate;
        hasTrap = data.hasTrap;

        walls = data.wall;

        trap.SetActive(hasTrap); 
        cheese.SetActive(hasCheese);
        plate.SetActive(hasPlate);

        foreach(var kv in wallToTransforms)
        {
            if((walls & kv.wall) != Wall.None)
            {
                kv.gameObject.SetActive(true);
            }
            else
            {
                kv.gameObject.SetActive(false);
            }
        }
    }
    
}
