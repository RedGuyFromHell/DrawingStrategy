using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;
using Unity.Mathematics;

public class TerrainGenerator : MonoBehaviour
{
    static TerrainGenerator _instance;
    public static TerrainGenerator Instance { get { return _instance; } }

    [SerializeField] Transform terrainSpot;
    [SerializeField] GameObject squarePrefab;
    [SerializeField] public int heigth = 24;
    [SerializeField] public int width = 12;

    public SquareHandler[,] terrainGrid;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        terrainGrid = new SquareHandler[heigth, width];    
    }

    private void Start()
    {
        GenerateTerrainGrid(heigth, width);
    }

    void GenerateTerrainGrid (int heigth, int width)
    {
        for (int i = 0; i < heigth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 squarePos = new Vector3(j, 0, i) + terrainSpot.position;
                GameObject temp = Instantiate(squarePrefab, squarePos, Quaternion.identity, terrainSpot);
                AstarPath.active.UpdateGraphs(temp.GetComponent<Collider>().bounds);

                terrainGrid[i, j] = temp.GetComponent<SquareHandler>();
                terrainGrid[i, j].i = i;
                terrainGrid[i, j].j = j;
            }
        }
    }

    public int2 GetSquareCoords (SquareHandler square)
    {
        for (int i = 0; i < heigth; i++)
            for (int j = 0; j < width; j++)
                if (terrainGrid[i, j] == square)
                    return new int2(i, j);

        return int2.zero;
    }
}
