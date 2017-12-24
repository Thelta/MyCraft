using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

public class CreateTextureTypeEnum : Editor
{
    static string enumPath = "Assets/Script/Blocks/TextureType.cs";
    static string texturePath = "Tiles";

    [MenuItem("GameObject/Create Texture Type Enum")]
    static void CreateTextureType()
    {
        string[] textureNames = GetTextureName();
        string[] enumLines = GetEnumLines(textureNames);

        CreateEnum(enumLines);

        AssetDatabase.Refresh();

    }

    static string[] GetTextureName()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(texturePath);
        string[] texNames = new string[textures.Length];

        for(int i = 0; i < textures.Length; i++)
        {
            texNames[i] = textures[i].name;
        }

        /*
            It seems Unity reads file in normal string sort like 1, 100, 1000, 2 etc
            So we need to do a natural sort using beginning number lie 1, 2, 100, 1000
        */
        
        texNames = texNames.OrderBy(t => Convert.ToInt32((t.Substring(0, t.IndexOf('_'))))).ToArray();

        return texNames;
    }

    static string[] GetEnumLines(string[] textureNames)
    {
        string[] enumLines = new string[textureNames.Length];
        for(int i = 0; i < textureNames.Length; i++)
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

            foreach(string enumLine in enumLines)
            {
                streamWriter.WriteLine(enumLine);
            }

            streamWriter.WriteLine("}");
        }
    }
}
