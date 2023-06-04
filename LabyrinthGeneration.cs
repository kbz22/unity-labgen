using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabyrinthNamespace;
using System.Drawing;

public class LabyrinthGeneration : MonoBehaviour
{  
    public GameObject wallPrefab;
    public GameObject terrainObject;
    public Labyrinth GeneratedLabyrinth;
    private Vector2Int labyrinthSize = new Vector2Int(14, 14);
    private float wallLength;

    private static Quaternion verticalRotation; 
    private static Quaternion horizontalRotation;

    void Start()
    {
        // Initialize angles, adjust based on the walls initial rotation
        MeshRenderer wallMesh = wallPrefab.GetComponent<MeshRenderer>();
        if (wallMesh.bounds.size.x > wallMesh.bounds.size.z)
        {
            verticalRotation = Quaternion.Euler(0, 90, 0);
            horizontalRotation = Quaternion.Euler(0, 0, 0);
            wallLength = wallMesh.bounds.size.x;
        }
        else
        {
            verticalRotation = Quaternion.Euler(0, 0, 0);
            horizontalRotation = Quaternion.Euler(0, 90, 0);
            wallLength = wallMesh.bounds.size.z;
        }

        wallPrefab.transform.rotation = verticalRotation;

        GeneratedLabyrinth = new Labyrinth(labyrinthSize);
        ResizeTerrain(new Vector2Int(GeneratedLabyrinth.Width, GeneratedLabyrinth.Height));
        GenerateWalls();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ResizeTerrain(Vector2Int size)
    {
        Terrain terrain = terrainObject.GetComponent<Terrain>();

        // Resize the terrain and move it so that the middle is at (0,0)
        terrain.terrainData.size = new Vector3(size.x * wallLength, 0, size.y * wallLength);
        terrainObject.transform.position = new Vector3(-size.x * wallLength / 2, 0, -size.y * wallLength / 2);

        // Update size for this class
        labyrinthSize.x = size.x;
        labyrinthSize.y = size.y;
    }

    private void GenerateWalls()
    {
        MeshRenderer wallMesh = wallPrefab.GetComponent<MeshRenderer>();
        float xOffset, yOffset;

        if(labyrinthSize.y % 2 == 0)
            xOffset = -labyrinthSize.y * wallLength / 2 + wallLength / 2;
        else xOffset = -labyrinthSize.y * wallLength / 2 + wallLength;

        if (labyrinthSize.x % 2 == 0)
            yOffset = -labyrinthSize.x * wallLength / 2 + wallLength / 2;
        else yOffset = -labyrinthSize.x * wallLength / 2 + wallLength;

        for (int i = 0; i < GeneratedLabyrinth.Width; i++)
            for (int k = 0; k < GeneratedLabyrinth.Height; k++)
            {
                try { if (GeneratedLabyrinth[i + 1, k] != LabyrinthDirection.Left && GeneratedLabyrinth[i, k] != LabyrinthDirection.Right) Instantiate(wallPrefab, new Vector3((float)i * wallLength + yOffset + wallMesh.bounds.size.z / 2, 0, (float)k * wallLength + xOffset), verticalRotation); }
                catch { }
                try { if (GeneratedLabyrinth[i - 1, k] != LabyrinthDirection.Right && GeneratedLabyrinth[i, k] != LabyrinthDirection.Left) Instantiate(wallPrefab, new Vector3((float)i * wallLength + yOffset - wallMesh.bounds.size.z / 2, 0, (float)k * wallLength + xOffset), verticalRotation); }
                catch { }
                try { if (GeneratedLabyrinth[i, k - 1] != LabyrinthDirection.Up && GeneratedLabyrinth[i, k] != LabyrinthDirection.Down) Instantiate(wallPrefab, new Vector3((float)i * wallLength + yOffset, 0, (float)k * wallLength + xOffset - wallMesh.bounds.size.z / 2), horizontalRotation); }
                catch { }
                try { if (GeneratedLabyrinth[i, k + 1] != LabyrinthDirection.Down && GeneratedLabyrinth[i, k] != LabyrinthDirection.Up) Instantiate(wallPrefab, new Vector3((float)i * wallLength + yOffset, 0, (float)k * wallLength + xOffset + wallMesh.bounds.size.z / 2), horizontalRotation); }
                catch { }
            }
    }
}