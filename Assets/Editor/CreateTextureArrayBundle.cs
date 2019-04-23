using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class CreateTextureArrayBundle : Editor
{
    const string ENUM_PATH = "Assets/Script/Blocks/TextureType.cs";
    const string TEXTURE_PATH = "Tiles";
    const string TEXTURE_ARRAY_PATH = "Assets/2DArray.asset";

    const int ALPHA_TEXTURE_MAX = 100;

    [MenuItem("GameObject/Process Tile Textures")]
    static void ProcessTileTextures()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(TEXTURE_PATH);

        /*
            It seems Unity reads file in normal string sort like 1, 100, 1000, 2 etc
            So we need to do a natural sort using beginning number like 1, 2, 100, 1000
        */
        textures = textures.OrderBy(t => Convert.ToInt32(t.name.Substring(0, t.name.IndexOf('_')))).ToArray();

        //Count textures that contain alpha
        int alphaTextureCount = textures.Count(t => Convert.ToInt32(t.name.Substring(0, t.name.IndexOf('_'))) < ALPHA_TEXTURE_MAX);

        //Create Texture2DArray prefab
        CreateTextureArray(textures);

        //Create tile name enum
        string[] textureNames = GetTextureName(textures);
        string[] enumLines = GetEnumLines(textureNames);
        CreateEnum(enumLines, alphaTextureCount);

        AssetDatabase.Refresh();

        Debug.Log("Texture Array Created Succesfully.");
    }

    static void CreateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textures[0].width, textures[1].height, textures.Length, TextureFormat.RGBA32, true);

        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(0), i, 0);
        }
        textureArray.Apply();

        AssetDatabase.CreateAsset(textureArray, TEXTURE_ARRAY_PATH);
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

    static void CreateEnum(string[] enumLines, int alphaTextureCount)
    {
        using (StreamWriter streamWriter = new StreamWriter(ENUM_PATH))
        {
            streamWriter.WriteLine("public enum TextureType");
            streamWriter.WriteLine("{");

            foreach (string enumLine in enumLines)
            {
                streamWriter.WriteLine(enumLine);
            }

            streamWriter.WriteLine("}");


            streamWriter.WriteLine("\n public static class TextureValues");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\t public const int ALPHA_TEXTURE_COUNT = {0};", alphaTextureCount);
            streamWriter.WriteLine("}");
        }
    }
}
