using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeTile : MonoBehaviour
{
    public GameObject NorthWall;
    public GameObject SouthWall;
    public GameObject EastWall;
    public GameObject WestWall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void deleteWall(int i)
    {
        switch(i)
        { 
            case 0:
                Destroy(NorthWall);
                break;
            case 1:
                Destroy(SouthWall);
                break;
            case 2:
                Destroy(EastWall);
                break;
            case 3:
                Destroy(WestWall);
                break;

        }
            
    }
}
