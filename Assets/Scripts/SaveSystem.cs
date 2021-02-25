using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private const string SAVE_EXTENSION = "txt";

    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private static bool isInit = false;

    public static void Init()
    {
        if(!isInit)
        {
            isInit = true;

            if(!Dictionary.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }

    public static void Save(string fileName, string saveString, bool overwrite)
    {
        Init();
        string saveFileName = fileName;
        if(!overwrite)
        {

        }
    }
}
