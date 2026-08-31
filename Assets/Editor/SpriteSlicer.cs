using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteSlicer
{
    [MenuItem("Tools/Slice Satyr Sprites")]
    public static void ProcessSatyrSprites()
    {
        string[] relativePaths = new string[]
        {
            "Assets/Sprites/SATYR_sprite_sheet/SPRITE_SHEET.png",
            "Assets/Sprites/SATYR_sprite_sheet/SPRITE_PORTRAIT.png"
        };

        foreach (string path in relativePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                // Load image dimensions directly
                byte[] fileData = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                int texWidth = tex.width;
                int texHeight = tex.height;

                Debug.Log($"Processing {path} (Dimensions: {texWidth}x{texHeight})");

                // Determine grid slicing or automatic frames
                List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

                // Check common sprite grid sizes (e.g. 32x32, 64x64, 48x48, 16x16)
                int sliceWidth = 32;
                int sliceHeight = 32;

                if (texWidth % 32 == 0 && texHeight % 32 == 0)
                {
                    sliceWidth = 32;
                    sliceHeight = 32;
                }
                else if (texWidth % 64 == 0 && texHeight % 64 == 0)
                {
                    sliceWidth = 64;
                    sliceHeight = 64;
                }
                else if (texWidth % 16 == 0 && texHeight % 16 == 0)
                {
                    sliceWidth = 16;
                    sliceHeight = 16;
                }
                else
                {
                    sliceWidth = texWidth;
                    sliceHeight = texHeight;
                }

                int cols = texWidth / sliceWidth;
                int rows = texHeight / sliceHeight;
                int count = 0;

                for (int r = rows - 1; r >= 0; r--)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        SpriteMetaData meta = new SpriteMetaData();
                        meta.rect = new Rect(c * sliceWidth, r * sliceHeight, sliceWidth, sliceHeight);
                        meta.name = $"{Path.GetFileNameWithoutExtension(path)}_{count++}";
                        meta.pivot = new Vector2(0.5f, 0.5f);
                        meta.alignment = (int)SpriteAlignment.Center;
                        metaDataList.Add(meta);
                    }
                }

                importer.spritesheet = metaDataList.ToArray();
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                Debug.Log($"Successfully sliced {path} into {metaDataList.Count} sprites ({sliceWidth}x{sliceHeight} grid).");
            }
        }

        // Now assign first sliced sprite from SPRITE_SHEET to Player
        AssignSpriteToPlayer();
    }

    [MenuItem("Tools/Assign Satyr Sprite to Player")]
    public static void AssignSpriteToPlayer()
    {
        string spriteSheetPath = "Assets/Sprites/SATYR_sprite_sheet/SPRITE_SHEET.png";
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);
        Sprite firstSprite = null;

        foreach (Object obj in assets)
        {
            if (obj is Sprite s)
            {
                firstSprite = s;
                break;
            }
        }

        if (firstSprite != null)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Undo.RecordObject(sr, "Assign Satyr Sprite");
                    sr.sprite = firstSprite;
                    sr.color = Color.white; // reset color tint to white so sprite texture is clearly visible!
                    EditorUtility.SetDirty(sr);
                    Debug.Log($"Assigned sprite '{firstSprite.name}' to Player GameObject.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No sliced sprite found on " + spriteSheetPath);
        }
    }
}
