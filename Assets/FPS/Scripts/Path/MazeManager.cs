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

   	public bool IsDestroyed = false;
    // Start is called before the first frame update
    public int scalar = 8;
    void Start()
    {


        for (int x = 1; x < 6; x++)
        {
            for (int y = 1; y < 6; y++)
            {
            	// Create Floor Tile
                visited[(x - 1) * 5 + y, 4] = Instantiate(FloorTile, new Vector3(x * scalar + 24, 0.1f, y * scalar + 12), Quaternion.identity);
                // Back Wall
                visited[(x - 1) * 5 + y, 0] = Instantiate(Wall, new Vector3(x * scalar + 24, 0.1f, y * scalar + 12), Quaternion.identity);
                // Front Wall
                visited[(x - 1) * 5 + y, 1] = Instantiate(Wall, new Vector3(x * scalar + 27, 0.1f, y * scalar + 12), Quaternion.identity);
                // Crete Left Wall
                visited[(x - 1) * 5 + y, 2] = Instantiate(Wall, new Vector3(x * scalar + 24, 0.1f, y * scalar + 12), Quaternion.Euler(0, 90, 0));
                // Create Right Wall
                visited[(x - 1) * 5 + y, 3] = Instantiate(Wall, new Vector3(x * scalar + 24, 0.1f, y * scalar + 15), Quaternion.Euler(0, 90, 0));
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
    // Check whether the maze has been destroyed or not
    void Update()
    {
    	for(int k = 0; k < PathList.Count; k++)
        { 
        	// If the floor tile has been destroyed
            if (visited[(((PathList[k].x / scalar) - 1) * 5) + (PathList[k].x / scalar), 4] == null)
            {
                Debug.Log("Destroyed");
                // Set the destroy tag to true. We call this in Ammo and Maze Completion to check and make sure
                IsDestroyed = true;
            }
        }
    }


    List<int> Direction(int x, int y, List<Vector2Int> p)
    {
    	// Direction helper that's useful for navigating Unvisited tiles. 
    	// Normally we pass in the PathList, but sometimes we may want to pass in those who have been visited but has not been branched
    	// CutDirFrontBack and CutDirLeftRight instruct the program to remove a certain direction. It is essentially used to prevent "loops"
        List<int> dir = new List<int>(){1,2,3,4};
        if (x == 5 * scalar || p.Contains(new Vector2Int(x + scalar, y)) || CutDirFrontBack[p.Count - 1].x == 1)
        {
        	// Cannot go forward
            dir.Remove(1);
        }
        if (x == 1 * scalar || y == 1 * scalar || y == 5 * scalar || p.Contains(new Vector2Int(x - scalar, y)) || CutDirFrontBack[p.Count - 1].y == 1)
        {
        	// Cannot go Backward
            dir.Remove(2);
        }
        if (y == 1 * scalar || x == 5 * scalar || x == 1 * scalar || p.Contains(new Vector2Int(x, y - scalar)) || CutDirLeftRight[p.Count - 1].x == 1)
        {
        	// Cannot go left
            dir.Remove(3);
        }
        if (y == 5 * scalar || p.Contains(new Vector2Int(x, y + scalar))|| CutDirLeftRight[p.Count - 1].y == 1)
        {
        	// Cannot go right
            dir.Remove(4);
        }


        return dir;
    }

    List<int> AvailableDirectionForUnvisited(int x, int y)
    {
        List<int> dir = new List<int>(){1,2,3,4};
        if (x == 5 * scalar || ToBranch.Contains(new Vector2Int(x+scalar, y)))
        {
            dir.Remove(1);
        }
        if (x == 1 * scalar  || ToBranch.Contains(new Vector2Int(x-scalar,y)))
        {
            dir.Remove(2);
        }
        if (y == 1 * scalar ||ToBranch.Contains(new Vector2Int(x, y-scalar)))
        {
            dir.Remove(3);
        }
        if (y == 5 * scalar || ToBranch.Contains(new Vector2Int(x, y+scalar)))
        {
            dir.Remove(4);
        }
        
        return dir;
    }

    void Generate()
    {
        int x = scalar;
        int y = scalar;
        CutDirFrontBack.Add(new Vector2Int(0, 0));
        CutDirLeftRight.Add(new Vector2Int(0, 0));
        PathList.Add(new Vector2Int(scalar, scalar));
        branch.Add(new Vector2Int(scalar, scalar));
        UnvisitedTile.Remove(new Vector2Int(1, 1));

        int i = Random.Range(0, Direction(x, y, PathList).Count);
        while (x != 5 * scalar || y != 5 * scalar)
        {
            List<int> availableDirection = Direction(x, y, PathList);

            if (availableDirection.Count > 0)
            {
            	i = Random.Range(0, availableDirection.Count);    
				if (availableDirection[i] == 1)
                {
                    PathVector.Add(new Vector3Int(x, y, 1));
                    x += scalar;
                } 
                else if (availableDirection[i] == 2)
                {
                    PathVector.Add(new Vector3Int(x, y, 2));
                    x -= scalar;
                }
                else if (availableDirection[i] == 3)
                {
                    PathVector.Add(new Vector3Int(x, y, 3));
                    y -= scalar;
                }
                else if (availableDirection[i] == 4)
                {
                    PathVector.Add(new Vector3Int(x, y, 4));
                    y += scalar;
                }
                CutDirFrontBack.Add(new Vector2Int(0, 0));
				CutDirLeftRight.Add(new Vector2Int(0, 0));
                PathList.Add(new Vector2Int(x, y));
                branch.Add(new Vector2Int(x, y));
                UnvisitedTile.Remove(new Vector2Int(x / scalar, y / scalar));
            }
            else
            {
            	// Back track if there is no viable path.
            	// Delete the latest from PathList and Branch.
                PathList.Remove(new Vector2Int(x, y));
                branch.Remove(new Vector2Int(x, y));
                UnvisitedTile.Add(new Vector2Int(x / scalar, y / scalar));
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
                    if (y - y1 == scalar)
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
                    if (x - x1 == scalar)
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
            int x_cor = PathVector[k].x;
            int y_cor = PathVector[k].y;
            int dirChoose = PathVector[k].z;
            if (dirChoose == 1)
            {
            	// Front
                Destroy(visited[(x_cor/scalar - 1) * 5 + y_cor/scalar, 1]);
        		Destroy(visited[(x_cor/scalar ) * 5 + y_cor/scalar , 0]);
            }
            if (dirChoose == 2)
            {
            	// Back
                Destroy(visited[(x_cor/scalar - 1) * 5 + y_cor/scalar, 0]);
        Destroy(visited[(x_cor/scalar - 2) * 5 + y_cor/scalar , 1]);
            }
            if (dirChoose == 3)
            {
            	// Left
                Destroy(visited[(x_cor / scalar - 1) * 5 + y_cor/scalar, 2]);
        Destroy(visited[(x_cor / scalar -1) * 5 + y_cor/scalar-1, 3]);
    
            }
            if (dirChoose == 4)
            {
            	// Right
                Destroy(visited[(x_cor / scalar - 1) * 5 + y_cor / scalar, 3]);
        		Destroy(visited[(x_cor / scalar -1) * 5 + y_cor / scalar+1, 2]);
    
            }
        }

    }
    

    void GenerateUnvisitedTile()
    {
        int num = Random.Range(0, UnvisitedTile.Count);
        int x = UnvisitedTile[num].x * scalar;
        int y = UnvisitedTile[num].y * scalar;
        int size = UnvisitedTile.Count;
        int i = Random.Range(0,AvailableDirectionForUnvisited(x, y).Count);

        bool branchAttached = false;
        while (UnvisitedTile.Count != 0)
        {
            
            if (AvailableDirectionForUnvisited(x, y)[i] == 1)
            {
            	// Opening a path forward, destroy the two corresponding walls
                Destroy(visited[(x / scalar - 1) * 5 + y / scalar, 1]);
        		Destroy(visited[(x / scalar) * 5 + y / scalar, 0]);
                x += scalar;
                // Is it attached to a branch?
                if (branch.Contains(new Vector2Int(x, y)))
                {
                    branchAttached = true;
                    branch.Add(new Vector2Int(x - scalar, y));
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
                    ToBranch.Add(new Vector2Int(x - scalar, y));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scalar - 1, y / scalar));

            }
            else if (AvailableDirectionForUnvisited(x, y)[i] == 2)
            {
                Destroy(visited[(x / scalar - 1) * 5 + y / scalar, 0]);
        		Destroy(visited[(x / scalar - 2) * 5 + y / scalar, 1]);
                x -= scalar;
                // does the branch contains itself?
                if (branch.Contains(new Vector2Int(x, y)))
                {

                    branchAttached = true;
                    branch.Add(new Vector2Int(x + scalar, y));
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
                    ToBranch.Add(new Vector2Int(x + scalar, y));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scalar + 1, y / scalar));

            }
			else if (AvailableDirectionForUnvisited(x, y)[i] == 3)
            {
                Destroy(visited[(x / scalar - 1) * 5 + y / scalar, 2]);
        		Destroy(visited[(x / scalar - 1) * 5 + y / scalar - 1, 3]);
                y -= scalar;
                if (branch.Contains(new Vector2Int(x, y)))
                {
                    branchAttached = true;
                    branch.Add(new Vector2Int(x, y + scalar));
                    while (ToBranch.Count > 0)
                    {
                        Vector2Int trans = ToBranch[0];
                        ToBranch.Remove(trans);
                        branch.Add(trans);
                    }
                }
                else
                {
                    ToBranch.Add(new Vector2Int(x, y + scalar));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scalar, y / scalar + 1));
            }
            else if (AvailableDirectionForUnvisited(x, y)[i] == 4)
            {
            	// Open a path to the right
                Destroy(visited[(x / scalar - 1) * 5 + y / scalar, 3]);
        		Destroy(visited[(x / scalar - 1) * 5 + y / scalar + 1, 2]);
                y += scalar;
                // Is it attached to a branch?
                if (branch.Contains(new Vector2Int(x, y)))
                {
                    branchAttached = true;
                    branch.Add(new Vector2Int(x, y - scalar));
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
                    ToBranch.Add(new Vector2Int(x, y - scalar));
                }
                UnvisitedTile.Remove(new Vector2Int(x / scalar, y / scalar - 1));
            }

            if (branchAttached == true && UnvisitedTile.Count != 0)
            {
                num = Random.Range(0, UnvisitedTile.Count);
                x = UnvisitedTile[num].x * scalar;
                y = UnvisitedTile[num].y * scalar;
                i = Random.Range(0, AvailableDirectionForUnvisited(x, y).Count);
                branchAttached = false;
            }
            else
            {
                i = Random.Range(0, AvailableDirectionForUnvisited(x, y).Count);
            }

        }
    }
}

