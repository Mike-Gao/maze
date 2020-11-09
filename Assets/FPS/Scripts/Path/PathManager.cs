using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{

    public GameObject pathTile;
    public GameObject ammo;
    public static int tileScale = 3; //default tile size 3
    public List<Vector2Int> allpoints = new List<Vector2Int>();
    // Start is called before the first frame update
    void Start()
    {
        GeneratePath();
        GenerateAmmo();
    }


    // Update is called once per frame
    void Update()
    {

    }

    void SpawnFloor(int x, int y)
    {
        // Spawn a tile
        Instantiate(pathTile, new Vector3(x, 0.1f, y), Quaternion.identity);
    }

    void SpawnAmmo(int x, int y)
    {
        // Spawn ammo on the generated path
        Instantiate(ammo, new Vector3(x, 0.1f, y), Quaternion.identity);
    }

    void GenerateAmmo()
    {
        for (int i = 0; i < 8; ++i)
        {
            int q = Random.Range(0, allpoints.Count);
            SpawnAmmo(allpoints[q].x, allpoints[q].y);
            allpoints.RemoveAt(q);
        }
    }

    List<int> GetAvailableDirection(int x, int y, List<Vector2Int> visited)
    {
        // visited: track already visited tiles
        List<int> direction = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            direction.Add(i);
        }
        if (y == 7 * tileScale || visited.Contains(new Vector2Int(x, y + 3 * tileScale)))
        {
            // Cannot go forward
            direction.Remove(0);
        }
        if (y == tileScale || (x == 7 * tileScale && y == 4 * tileScale) || visited.Contains(new Vector2Int(x, y - 3 * tileScale))) 
        {
            // Cannot go backward
            direction.Remove(1);
        }
        if (x == tileScale || (y == 7 * tileScale && x == 4 * tileScale) || visited.Contains(new Vector2Int(x - 3 * tileScale, y)))
        {
            // Cannot go left
            direction.Remove(2);
        }
        if (x == 7 * tileScale || visited.Contains(new Vector2Int(x + 3 * tileScale, y)))
        {
            // Cannot go right
            direction.Remove(3);
        }
        // Return a set of available direction
        return direction;
    }

    void GeneratePath()
    {
        List<Vector2Int> visited = new List<Vector2Int>();
        int x = tileScale;
        int y = tileScale;
        visited.Add(new Vector2Int(x, y));
        SpawnFloor(x, y);
        int i = Random.Range(0, GetAvailableDirection(x, y, visited).Count);
        // Generate path leading up to the maze entrance
        while (x != 7 * tileScale || y != 7 * tileScale)
        {

            switch (GetAvailableDirection(x, y, visited)[i])
            {
                case 0:
                    for (int j = 1; j < 4; ++j)
                    {
                        SpawnFloor(x, y + j * tileScale);
                        allpoints.Add(new Vector2Int(x, y + j * tileScale));
                    }
                    y = y + 3 * tileScale;
                    visited.Add(new Vector2Int(x, y));
                    break;
                case 1:
                    for (int j = 1; j < 4; ++j)
                    {
                        SpawnFloor(x, y - j * tileScale);
                        allpoints.Add(new Vector2Int(x, y - j * tileScale));
                    }
                    y = y - 3 * tileScale;
                    visited.Add(new Vector2Int(x, y));
                    break;
                case 2:
                    for (int j = 1; j < 4; ++j)
                    {
                        SpawnFloor(x - j * tileScale, y);
                        allpoints.Add(new Vector2Int(x - j * tileScale, y));
                    }
                    x = x - 3 * tileScale;
                    visited.Add(new Vector2Int(x, y));
                    break;
                case 3:
                    for (int j = 1; j < 4; ++j)
                    {
                        SpawnFloor(x + j * tileScale, y);
                        allpoints.Add(new Vector2Int(x + j * tileScale, y));
                    }
                    x = x + 3 * tileScale;
                    visited.Add(new Vector2Int(x, y));
                    break;
            }    
            i = Random.Range(0, GetAvailableDirection(x, y, visited).Count);
        }

    }

    
}
