using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    public MazeTile tile;
    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnRoom(int x, int y)
    {
        // Spawn ammo on the generated path
        Instantiate(tile, new Vector3(x, 0.1f, y), Quaternion.identity);
    }
    void GenerateMaze()
    {
        MazeTile[,] maze = InitMaze();
        maze[0, 4].deleteWall(1);
        maze[4, 0].deleteWall(0);


    }

    MazeTile[,] InitMaze()
    {
        MazeTile[,] maze = new MazeTile[5, 5];
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                maze[i, j] = Instantiate(tile, new Vector3(14 + i * 10, 0, -42 + j * 10), Quaternion.identity);
                maze[i, j].X = i;
                maze[i, j].Y = j;
            }
            
        }
        return maze;

    }
}
