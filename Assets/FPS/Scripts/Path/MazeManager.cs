using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    void GenerateMaze()
    {
        // Generate the initial maze
        MazeTile[,] maze = InitMaze();
        // Start and finish
        maze[0, 0].deleteWall(1);
        maze[4, 4].deleteWall(0);

        Vector2Int cur = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(4, 4);
        while (cur != end)
        {

        }
    }

    /*int GenerateDirection(int x, int y, List<Vector2Int> visited)
    {

    }*/

    void ConnectCell(MazeTile a, MazeTile b)
    {
        // A - B
        if (b.X == a.X && b.Y > a.Y)
        {

            a.deleteWall(3);
            b.deleteWall(2); 
        }

        // B - A
        if (b.X == a.X && b.Y < a.Y)
        {
            a.deleteWall(2);
            b.deleteWall(3);
        }
        // A
        // |
        // B
        if (a.X < b.X && b.Y == a.Y)
        {
            a.deleteWall(0);
            b.deleteWall(1);
        }

        // B
        // |
        // A
        if (a.X > b.X && b.Y == a.Y)
        {
            a.deleteWall(1);
            b.deleteWall(0);
        }
    }

    MazeTile[,] InitMaze()
    {
        MazeTile[,] maze = new MazeTile[5, 5];
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                maze[i, j] = Instantiate(tile, new Vector3(14 + i * 10, 0, -1 + j * 10), Quaternion.identity);
                maze[i, j].X = i;
                maze[i, j].Y = j;
            }
            
        }
        return maze;

    }
}
