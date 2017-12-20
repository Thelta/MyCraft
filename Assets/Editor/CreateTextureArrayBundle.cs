using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateTextureArrayBundle : Editor
{
    public string textureLocation;

    [MenuItem("GameObject/Create Texture Array")]
    static void CreateTextureArray()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Tiles");
        Debug.Log(textures.Length);

        Texture2DArray textureArray = new Texture2DArray(textures[0].width, textures[1].height, textures.Length, TextureFormat.RGBA32, false);
        for(int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(0), i, 0);
        }
        textureArray.Apply();

        AssetDatabase.CreateAsset(textureArray, "Assets/2DArray.asset");
    }
}
