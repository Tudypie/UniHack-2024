using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;



public class Tile : MonoBehaviour
{
    public GameObject cheese;
    public GameObject trap;
    public GameObject plate;

    [SerializeField] List<WallToTransform> wallToTransforms;


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
        Wall wall = Wall.None;
        
        foreach(var kv in wallToTransforms)
        {
            if (kv.gameObject.activeSelf)
            {
                wall |= kv.wall;
            }
        }


        return new TileData
        {
            hasCheese = cheese.activeSelf,
            hasPlate = plate.activeSelf,
            hasTrap = trap.activeSelf,
            position = transform.position,
            rotation = transform.rotation,
            walls = wall,
        };
    }

    public void LoadData(TileData data)
    {
        trap.SetActive(data.hasTrap); 
        cheese.SetActive(data.hasCheese);
        plate.SetActive(data.hasPlate);

        foreach(var kv in wallToTransforms)
        {
            if((data.walls & kv.wall) != Wall.None)
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
