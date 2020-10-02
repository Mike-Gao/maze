using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MazeManager : MonoBehaviour
{
    public MazeTile tile;
    public List<MazeTile> MazePath;
    public MazeTile[,] maze;
  MazeTile[,] Visited = new MazeTile[5,5];
    List<Vector2Int> Unvisited = new List<Vector2Int>();
    // Generated Path
    List<Vector2Int> PathGenerated = new List<Vector2Int>();//store the path
    List<Vector2Int> CutDirNS = new List<Vector2Int>();
    List<Vector2Int> CutDirWE = new List<Vector2Int>();
    // Store the Branch
    List<Vector2Int> Branch = new List<Vector2Int>();
    // Store the stuff to be added to the branch, but not unvisited
    List<Vector2Int> BranchWaitingList = new List<Vector2Int>();
    // Direction and Place Vector
    List<Vector3Int> PathVector = new List<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    List<int> GenerateDirection(int x, int y, List<Vector2Int> prev)
    {
        List<int> PossibleDirections = new List<int>() { 0, 1, 2, 3 };


        if (x == 0 || y == 0 || y == 4 || prev.Contains(new Vector2Int(x - 1, y)))
            PossibleDirections.Remove(1);
        if (x == 4 || prev.Contains(new Vector2Int(x + 1, y)))
            PossibleDirections.Remove(0);
        if (y == 0 || x == 4 || x == 0 || prev.Contains(new Vector2Int(x, y - 1)))
            PossibleDirections.Remove(2);
        if (y == 4 || prev.Contains(new Vector2Int(x, y + 1)))
            PossibleDirections.Remove(3);

        return PossibleDirections;
    }

    void GenerateMaze()
    {
        // Generate the initial maze
        maze = InitMaze();
        // Start and finish
        maze[0, 0].deleteWall(1);
        maze[4, 4].deleteWall(0);

        Vector2Int cur = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(4, 4);
        int dir = 0;

        // North / South Cut Direction
        CutDirNS.Add(new Vector2Int(0, 0));
        // West / East Cut Direction
        CutDirWE.Add(new Vector2Int(0, 0));

        // Entrance of the Maze
        int x = 14;
        int y = -1;
        // Store arbitrary position in PathGenerated and Branch, and remove from Unvisited
        PathGenerated.Add(new Vector2Int(0, 0));
        Branch.Add(new Vector2Int(0, 0));
        Unvisited.Remove(new Vector2Int(0, 0));
        // Generate the path of the maze first
        int i = Random.Range(0, GenerateDirection(x, y, PathGenerated).Count);

        while(x!=4 && y!=4)
        {
            List<int> dirAvailable = GenerateDirection(x, y, PathGenerated);
            if (CutDirNS[CutDirNS.Count - 1].x == 1)
                // Cannot Go Forward
            {
                dirAvailable.Remove(0);
            }
            if (CutDirNS[CutDirNS.Count - 1].y == 1)
            {
                // Cannot Go Backword
                dirAvailable.Remove(1);
            }
            if (CutDirWE[CutDirWE.Count - 1].x == 1)
            {
                dirAvailable.Remove(2);
            }
            if (CutDirWE[CutDirWE.Count - 1].y == 1)
            {
                dirAvailable.Remove(3);
            }
            

            if (dirAvailable.Count > 0)
            {
                i = Random.Range(0, dirAvailable.Count);

                switch (dirAvailable[i])
                {
                    case 0:
                        PathVector.Add(new Vector3Int(x, y, 0));
                        x++;
                        break;
                    case 1:
                        PathVector.Add(new Vector3Int(x, y, 1));
                        x--;
                        break;
                    case 2:
                        PathVector.Add(new Vector3Int(x, y, 2));
                        y--;
                        break;
                    case 3:
                        PathVector.Add(new Vector3Int(x, y, 3));
                        y++;
                        break;

                }
                CutDirNS.Add(new Vector2Int(0, 0));
                CutDirWE.Add(new Vector2Int(0, 0));
                PathGenerated.Add(new Vector2Int(x, y));
                Branch.Add(new Vector2Int(x, y));
                Unvisited.Remove(new Vector2Int(x, y));
            } 
            else 
            {
                PathGenerated.Remove(new Vector2Int(x, y));
                Branch.Remove(new Vector2Int(x, y));
                Unvisited.Add(new Vector2Int(x, y));
                ///////////////////////////////////////
                int last = PathGenerated.Count;
                CutDirNS.RemoveAt(last);
                CutDirWE.RemoveAt(last);
                int x1 = x;
                int y1 = y;
                x = PathGenerated[last - 1].x;
                y = PathGenerated[last - 1].y;
                if (x == x1)
                {
                    if (y - y1 == 1)//turn left no way
                    {
                        int xReplace = CutDirWE[last - 1].x;
                        int yReplace = CutDirWE[last - 1].y;
                        CutDirWE.RemoveAt(last - 1);
                        CutDirWE.Add(new Vector2Int(1, yReplace));
                    }
                    else
                    {
                        int xReplace = CutDirWE[last - 1].x;
                        int yReplace = CutDirWE[last - 1].y;
                        CutDirWE.RemoveAt(last - 1);
                        CutDirWE.Add(new Vector2Int(xReplace, 1));
                    }
                }
                else
                {
                    if (x - x1 == 1)//turn back no way
                    {
                        int xReplace = CutDirNS[last - 1].x;
                        int yReplace = CutDirNS[last - 1].y;
                        CutDirNS.RemoveAt(last - 1);
                        CutDirNS.Add(new Vector2Int(xReplace, 1));
                    }
                    else
                    {
                        int xReplace = CutDirNS[last - 1].x;
                        int yReplace = CutDirNS[last - 1].y;
                        CutDirNS.RemoveAt(last - 1);
                        CutDirNS.Add(new Vector2Int(1, yReplace));
                    }
                }
            }

        }
        for (int q = 0; q < PathVector.Count; q++)
        {
            int xPlace = PathVector[q].x;
            int yPlace = PathVector[q].y;
            int dirChoose = PathVector[q].z;
            ConnectCellDir(maze[xPlace, yPlace], dirChoose);
        }


    }

    void ConnectCellDir(MazeTile a, int dir)
    {
        switch (dir)
        {
            case 0:
                a.deleteWall(1);
                maze[a.X + 1, a.Y].deleteWall(0);
                break;
            case 1:
                a.deleteWall(0);
                maze[a.X - 1, a.Y].deleteWall(1);
                break;
            case 2:
                a.deleteWall(3);
                maze[a.X, a.Y + 1].deleteWall(2);
                break;
            case 3:
                a.deleteWall(2);
                maze[a.X, a.Y - 1].deleteWall(3);
                break;
        }

    }

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

    
}
