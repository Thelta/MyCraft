using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetChunkShaderValues : Editor
{
    const string TILES_MATERIAL_PATH = "Assets/Materials/tiles.mat";
    const string TEXTURE_ARRAY_PATH = "Assets/2DArray.asset";

    const string ALPHA_TEXTURE_COUNT_VAR_NAME = "_AlphaTextureCount";
    const string WATER_TEXTURE_VAR_NAME = "_WaterTex";
    const string TEXTURE_ARRAY_VAR_NAME = "_MainTexArr";

    [MenuItem("GameObject/Set Chunk Shader Values")]
    static void SetChunkShaderValue()
    {
        Material tiles = AssetDatabase.LoadAssetAtPath<Material>(TILES_MATERIAL_PATH);
        Texture2DArray texArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(TEXTURE_ARRAY_PATH);

        tiles.SetTexture(TEXTURE_ARRAY_VAR_NAME, texArray);
        tiles.SetInt(ALPHA_TEXTURE_COUNT_VAR_NAME, TextureValues.ALPHA_TEXTURE_COUNT);
        tiles.SetInt(WATER_TEXTURE_VAR_NAME, (int)TextureType.water);

        Debug.Log("All values are initialized.");
    }

}
