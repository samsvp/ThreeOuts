using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Save
{
    public static string savePath = Application.persistentDataPath + "/batterNumber.txt";

    public static void SaveGame(int n)
    {
        System.IO.File.WriteAllText(savePath, n.ToString());
    }
}
