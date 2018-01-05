using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

public class CreateTextureArrayBundle : Editor
{
    static string enumPath = "Assets/Script/Blocks/TextureType.cs";
    static string texturePath = "Tiles";
    static string textureArrayPath = "Assets/2DArray.asset";

    [MenuItem("GameObject/Process Tile Textures")]
    static void ProcessTileTextures()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(texturePath);
        /*
            It seems Unity reads file in normal string sort like 1, 100, 1000, 2 etc
            So we need to do a natural sort using beginning number like 1, 2, 100, 1000
        */

        textures = textures.OrderBy(t => Convert.ToInt32((t.name.Substring(0, t.name.IndexOf('_'))))).ToArray();

        //Create Texture2DArray prefab
        CreateTextureArray(textures);

        //Create tile name enum
        string[] textureNames = GetTextureName(textures);
        string[] enumLines = GetEnumLines(textureNames);
        CreateEnum(enumLines);

        AssetDatabase.Refresh();
    }

    static void CreateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textures[0].width, textures[1].height, textures.Length, TextureFormat.RGBA32, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(0), i, 0);
        }
        textureArray.Apply();

        AssetDatabase.CreateAsset(textureArray, textureArrayPath);
    }

    static string[] GetTextureName(Texture2D[] textures)
    {
        string[] texNames = new string[textures.Length];

        for (int i = 0; i < textures.Length; i++)
        {
            texNames[i] = textures[i].name;
        }

        return texNames;
    }

    static string[] GetEnumLines(string[] textureNames)
    {
        string[] enumLines = new string[textureNames.Length];
        for (int i = 0; i < textureNames.Length; i++)
        {
            int seperatorIndex = textureNames[i].IndexOf("_");
            string tileName = textureNames[i].Substring(seperatorIndex + 1);
            enumLines[i] = string.Format("\t {0},", tileName);
        }

        return enumLines;
    }

    static void CreateEnum(string[] enumLines)
    {
        using (StreamWriter streamWriter = new StreamWriter(enumPath))
        {
            streamWriter.WriteLine("public enum TextureType");
            streamWriter.WriteLine("{");

            foreach (string enumLine in enumLines)
            {
                streamWriter.WriteLine(enumLine);
            }

            streamWriter.WriteLine("}");
        }
    }
}
