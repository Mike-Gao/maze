using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammo : MonoBehaviour
{
    // Start is called before the first frame update
    public Text AmmoCount;
    public int CurrentAmmo { get; set; }
    public int UsedAmmo = 0;
    void Start()
    {
        CurrentAmmo = 0;
        AmmoCount.text = $"{CurrentAmmo}";
        UsedAmmo = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (UsedAmmo >= 8 && !FindObjectOfType<MazeManager>().IsDestroyed)
        {
            // End game. bullet used so cannot destroy maze anymore
            var gfm = FindObjectOfType<GameFlowManager>();
            gfm.EndGame(false);
        }
    }

    public void AddAmmo()
    {
        CurrentAmmo++;
        AmmoCount.text = $"{CurrentAmmo}";
    }

    public bool UseAmmo()
    {
        if (CurrentAmmo <= 0) return false;
        CurrentAmmo--;
        UsedAmmo++;
        AmmoCount.text = $"{CurrentAmmo}";
        return true;
    }
}
