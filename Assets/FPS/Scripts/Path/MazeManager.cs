using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
	// MazeManager: Algorithm that can both generate the only valid path in the maze but also the maze itself.
    public GameObject FloorTile;
    public GameObject Wall;
    GameObject[,] visited = new GameObject[26, 5];
    List<Vector2Int> UnvisitedTile = new List<Vector2Int>();
    // Path List, this is to track the ONLY path on the maze
    List<Vector2Int> PathList = new List<Vector2Int>();
    List<Vector2Int> CutDirFrontBack = new List<Vector2Int>();
    List<Vector2Int> CutDirLeftRight = new List<Vector2Int>();
    // List of points that represent to be on the branch
    List<Vector2Int> branch = new List<Vector2Int>();
    // Visited, but not yet added to the branch
    List<Vector2Int> ToBranch = new List<Vector2Int>(); 
    // Path Vector, (x,y,dir) where x, y represents the coordinates and dir represents the direction
    List<Vector3Int> PathVector = new List<Vector3Int>();
    // Start is called before the first frame update
    void Start()
    {

        int scaler = 8;

        for (int x = 1; x < 6; x++)
        {
            for (int y = 1; y < 6; y++)
            {
            	// Create Floor Tile
                visited[(x - 1) * 5 + y, 4] = Instantiate(FloorTile, new Vector3(x * scaler + 24, 0.1f, y * scaler + 12), Quaternion.identity);
                // Back Wall
                visited[(x - 1) * 5 + y, 0] = Instantiate(Wall, new Vector3(x * scaler + 24, 0.1f, y * scaler + 12), Quaternion.identity);
                // Front Wall
                visited[(x - 1) * 5 + y, 1] = Instantiate(Wall, new Vector3(x * scaler + 27, 0.1f, y * scaler + 12), Quaternion.identity);
                // Crete Left Wall
                visited[(x - 1) * 5 + y, 2] = Instantiate(Wall, new Vector3(x * scaler + 24, 0.1f, y * scaler + 12), Quaternion.Euler(0, 90, 0));
                // Create Right Wall
                visited[(x - 1) * 5 + y, 3] = Instantiate(Wall, new Vector3(x * scaler + 24, 0.1f, y * scaler + 15), Quaternion.Euler(0, 90, 0));
                UnvisitedTile.Add(new Vector2Int(x, y));
            }
        }
        // Open the entrance and the end
        Destroy(visited[1, 0]);
        Destroy(visited[25, 1]);

        Generate();
        GenerateUnvisitedTile(); 
        Debug.Log(PathList.Count);   
    }

    // Update is called once per frame
    void Update()
    {
    	for (int i = 1; i < PathList.Count; i++)
    	{
    		if(visited[(PathList[i].x / 8 - 1) * 5 + PathList[i].y / 8, 4] == null){
    				Debug.Log("Destroyed");
    		}
    	}
    }


    List<int> Direction(int x, int y, List<Vector2Int> p)
    {
    	// Direction helper that's useful for navigating unvisited tiles. 
    	// Normally we pass in the PathList, but sometimes we may want to pass in those who have been visited but has not been branched
    	// CutDirFrontBack and CutDirLeftRight instruct the program to remove a certain direction. It is essentially used to prevent "loops"
        List<int> dir = new List<int>(){1,2,3,4};
        int scaler = 8;
        if (x == 5 * scaler || p.Contains(new Vector2Int(x + scaler, y)) || CutDirFrontBack[PathList.Count - 1].x == 1)
        {
        	// Cannot go forward
            dir.Remove(1);
        }
        if (x == 1 * scaler || y == 1 * scaler || y == 5 * scaler || p.Contains(new Vector2Int(x - scaler, y)) || CutDirFrontBack[PathList.Count - 1].y == 1)
        {
        	// Cannot go Backward
            dir.Remove(2);
        }
        if (y == 1 * scaler || x == 5 * scaler || x == 1 * scaler || p.Contains(new Vector2Int(x, y - scaler)) || CutDirLeftRight[PathList.Count - 1].x == 1)
        {
        	// Cannot go left
            dir.Remove(3);
        }
        if (y == 5 * scaler || p.Contains(new Vector2Int(x, y + scaler)) || CutDirLeftRight[PathList.Count - 1].y == 1)
        {
        	// Cannot go right
            dir.Remove(4);
        }


        return dir;
    }
    void Generate()
    {
        int scaler = 8;
        int x = scaler;
        int y = scaler;
        CutDirFrontBack.Add(new Vector2Int(0, 0));
        CutDirLeftRight.Add(new Vector2Int(0, 0));
        PathList.Add(new Vector2Int(scaler, scaler));
        branch.Add(new Vector2Int(scaler, scaler));
        UnvisitedTile.Remove(new Vector2Int(1, 1));

        int i = Random.Range(0, Direction(x, y, PathList).Count);
        while (x != 5 * scaler || y != 5 * scaler)
        {
            List<int> availableDirection = Direction(x, y, PathList);
            if (availableDirection.Count > 0)
            {
            	i = Random.Range(0, availableDirection.Count);
                CutDirFrontBack.Add(new Vector2Int(0, 0));
				CutDirLeftRight.Add(new Vector2Int(0, 0));
				if (availableDirection[i] == 1)
                {
                    PathVector.Add(new Vector3Int(x, y, 1));
                    x += scaler;
                } 
                else if (availableDirection[i] == 2)
                {
                    PathVector.Add(new Vector3Int(x, y, 2));
                    x -= scaler;
                }
                else if (availableDirection[i] == 3)
                {
                    PathVector.Add(new Vector3Int(x, y, 3));
                    y -= scaler;
                }
                else if (availableDirection[i] == 4)
                {
                    PathVector.Add(new Vector3Int(x, y, 4));
                    y += scaler;
                }
                PathList.Add(new Vector2Int(x, y));
                branch.Add(new Vector2Int(x, y));
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler));
            }
            else
            {
            	// Back track if there is no viable path.
            	// Delete the latest from PathList and Branch.
                PathList.Remove(new Vector2Int(x, y));
                branch.Remove(new Vector2Int(x, y));
                UnvisitedTile.Add(new Vector2Int(x / scaler, y / scaler));
                int last = PathList.Count;
                CutDirFrontBack.RemoveAt(last);
                CutDirLeftRight.RemoveAt(last);
                int x1 = x;
                int y1 = y;
                x = PathList[last - 1].x;
                y = PathList[last - 1].y;
                if (x == x1)
                {
                	int NewX = CutDirLeftRight[last - 1].x;
                    int NewY = CutDirLeftRight[last - 1].y;
                    CutDirLeftRight.RemoveAt(last - 1);
                    // Cannot turn left
                    if (y - y1 == scaler)
                    {
                        CutDirLeftRight.Add(new Vector2Int(1, NewY));
                    }
                    else
                    {
                        CutDirLeftRight.Add(new Vector2Int(NewX, 1));
                    }
                }
                else
                {
                	int NewX = CutDirFrontBack[last - 1].x;
                    int NewY = CutDirFrontBack[last - 1].y;
                    CutDirFrontBack.RemoveAt(last - 1);
                    if (x - x1 == scaler)
                    {
                        CutDirFrontBack.Add(new Vector2Int(NewX, 1));
                    }
                    else
                    {
                        CutDirFrontBack.Add(new Vector2Int(1, NewY));
                    }
                }
            }


        }
        for (int k = 0; k < PathVector.Count; k++)
        {
        	// Generate the path
            int Path_X = PathVector[k].x;
            int Path_Y = PathVector[k].y;
            int dirChoose = PathVector[k].z;
            if (dirChoose == 1)
            {
            	// Front
                Destroy(visited[(Path_X / scaler - 1) * 5 + Path_Y / scaler, 1]);
        		Destroy(visited[(Path_X / scaler ) * 5 + Path_Y / scaler , 0]);
            }
            if (dirChoose == 2)
            {
            	// Back
                Destroy(visited[(x/scaler - 1) * 5 + y/scaler, 0]);
        		Destroy(visited[(x/scaler - 2) * 5 + y/scaler , 1]);
            }
            if (dirChoose == 3)
            {
            	// Left
                Destroy(visited[(Path_X / scaler - 1) * 5 + Path_Y/scaler, 2]);
        		Destroy(visited[(Path_X / scaler - 1) * 5 + Path_Y/scaler - 1, 3]);
    
            }
            if (dirChoose == 4)
            {
            	// Right
                Destroy(visited[(Path_X / scaler - 1) * 5 + Path_Y / scaler, 3]);
        		Destroy(visited[(Path_X / scaler - 1) * 5 + Path_Y / scaler + 1, 2]);
            }
        }

    }
    
    void GenerateUnvisitedTile()
    {
        int scaler = 8;
        int num = Random.Range(0, UnvisitedTile.Count);
        int x = UnvisitedTile[num].x * scaler;
        int y = UnvisitedTile[num].y * scaler;
        int size = UnvisitedTile.Count;
        int i = Random.Range(0, Direction(x, y, ToBranch).Count);

        bool branchAttached = false;
        while (UnvisitedTile.Count != 0)
        {
            
            if (Direction(x, y, ToBranch)[i] == 1)
            {
            	// Opening a path forward, destroy the two corresponding walls
                Destroy(visited[(x / scaler - 1) * 5 + y / scaler, 1]);
        		Destroy(visited[(x / scaler) * 5 + y / scaler, 0]);
                x += scaler;
                // Is it attached to a branch?
                if (branch.Contains(new Vector2Int(x, y)))
                {
                    branchAttached = true;
                    branch.Add(new Vector2Int(x - scaler, y));
                    while (ToBranch.Count > 0)
                    {
                        Vector2Int trans = ToBranch[0];
                        ToBranch.Remove(trans);
                        branch.Add(trans);
                    }
                }
                else
                {
                	// Not a part of a branch, so add to ToBranch List
                    ToBranch.Add(new Vector2Int(x - scaler, y));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scaler - 1, y / scaler));
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler));

            }
            else if (Direction(x, y, ToBranch)[i] == 2)
            {
                Destroy(visited[(x / scaler - 1) * 5 + y / scaler, 0]);
        		Destroy(visited[(x / scaler - 2) * 5 + y / scaler, 1]);
                x -= scaler;
                // does the branch contains itself?
                if (branch.Contains(new Vector2Int(x, y)))
                {

                    branchAttached = true;
                    branch.Add(new Vector2Int(x + scaler, y));
                    // Attach all the positions to the Branch
                    while (ToBranch.Count > 0)
                    {
                        Vector2Int trans = ToBranch[0];
                        ToBranch.Remove(trans);
                        branch.Add(trans);
                    }
                }
                else
                {
                	// Not attached to a branch, so addto ToBranch list
                    ToBranch.Add(new Vector2Int(x + scaler, y));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scaler + 1, y / scaler));
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler));

            }
			else if (Direction(x, y, ToBranch)[i] == 3)
            {
                Destroy(visited[(x / scaler - 1) * 5 + y / scaler, 2]);
        		Destroy(visited[(x / scaler - 1) * 5 + y / scaler - 1, 3]);
                y -= scaler;
                if (branch.Contains(new Vector2Int(x, y)))
                {
                    branchAttached = true;
                    branch.Add(new Vector2Int(x, y + scaler));
                    while (ToBranch.Count > 0)
                    {
                        Vector2Int trans = ToBranch[0];
                        ToBranch.Remove(trans);
                        branch.Add(trans);
                    }
                }
                else
                {
                    ToBranch.Add(new Vector2Int(x, y + scaler));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler + 1));
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler));
            }
            else if (Direction(x, y, ToBranch)[i] == 4)
            {
            	// Open a path to the right
                Destroy(visited[(x / scaler - 1) * 5 + y / scaler, 3]);
        		Destroy(visited[(x / scaler - 1) * 5 + y / scaler + 1, 2]);
                y += scaler;
                // Is it attached to a branch?
                if (branch.Contains(new Vector2Int(x, y)))
                {
                    branchAttached = true;
                    branch.Add(new Vector2Int(x, y - scaler));
                    while (ToBranch.Count > 0)
                    {
                        Vector2Int trans = ToBranch[0];
                        ToBranch.Remove(trans);
                        branch.Add(trans);
                    }
                }
                else
                {
                	// Attach to ToBranch List
                    ToBranch.Add(new Vector2Int(x, y - scaler));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler - 1));
                UnvisitedTile.Remove(new Vector2Int(x / scaler, y / scaler));
            }

            if (branchAttached == true && UnvisitedTile.Count != 0)
            {
                num = Random.Range(0, UnvisitedTile.Count);
                x = UnvisitedTile[num].x * scaler;
                y = UnvisitedTile[num].y * scaler;
                i = Random.Range(0, Direction(x, y, ToBranch).Count);
                branchAttached = false;
            }
            else
            {
                i = Random.Range(0, Direction(x, y, ToBranch).Count);
            }

        }
    }
}

