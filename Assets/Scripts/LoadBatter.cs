using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBatter : MonoBehaviour
{
    public static int batterNumber = 0;

    public GameObject[] batters;
    public Vector3 batterPosition;

    void Awake()
    {
        InstantiateBatter();
    }

    public void InstantiateBatter()
    {
        if (System.IO.File.Exists(Save.savePath))
        {
            batterNumber = Int32.Parse(System.IO.File.ReadAllText(Save.savePath));
        }

        if (batterNumber == batters.Length)
        {
            SceneManager.LoadScene("VictoryScreen");
            Save.SaveGame(0);
        }

        Instantiate(batters[batterNumber], batterPosition, Quaternion.identity);
    }
}
