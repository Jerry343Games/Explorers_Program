using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class AutoSpriteSlicer
{
    public static void Slice(Texture2D texture, Vector2 cellSize, Vector2 pivot, bool slice, bool pixelated)
    {
        ProcessAnimation(texture, cellSize, pivot, slice, pixelated);
        ProcessAnimation(texture, cellSize, pivot, slice, pixelated);
    }


    static void ProcessAnimation(Texture2D texture, Vector2 cellSize, Vector2 pivot, bool slice, bool pixelated)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        //importer.isReadable = true;
        importer.textureType = TextureImporterType.Sprite;
        if (slice)
            importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.mipmapEnabled = false;

        if (pixelated)
            importer.filterMode = FilterMode.Point;
        else
            importer.filterMode = FilterMode.Bilinear;


        if (pixelated)
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        else
            importer.textureCompression = TextureImporterCompression.Compressed;

        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteMeshType = SpriteMeshType.FullRect;
        textureSettings.spriteExtrude = 1;

        importer.SetTextureSettings(textureSettings);

        Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, new Vector2(0, 0), cellSize, new Vector2(0, 0));
        var rectsList = new List<Rect>(rects);
        //rectsList = SortRects(rectsList, texture.width);

        string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
        var metas = new List<SpriteMetaData>();
        int rectNum = 0;

        if (slice)
        {
            foreach (Rect rect in rectsList)
            {
                var meta = new SpriteMetaData();
                meta.alignment = (int)SpriteAlignment.Custom;
                meta.pivot = pivot;
                meta.rect = rect;
                meta.name = filenameNoExtension + "_" + rectNum++;
                metas.Add(meta);
            }
            importer.spritesheet = metas.ToArray();
        }

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    static List<Rect> SortRects(List<Rect> rects, float textureWidth)
    {
        List<Rect> list = new List<Rect>();
        while (rects.Count > 0)
        {
            Rect rect = rects[rects.Count - 1];
            Rect sweepRect = new Rect(0f, rect.yMin, textureWidth, rect.height);
            List<Rect> list2 = RectSweep(rects, sweepRect);
            if (list2.Count <= 0)
            {
                list.AddRange(rects);
                break;
            }
            list.AddRange(list2);
        }
        return list;
    }

    static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
    {
        List<Rect> result;
        if (rects == null || rects.Count == 0)
        {
            result = new List<Rect>();
        }
        else
        {
            List<Rect> list = new List<Rect>();
            foreach (Rect current in rects)
            {
                if (current.Overlaps(sweepRect))
                {
                    list.Add(current);
                }
            }
            foreach (Rect current2 in list)
            {
                rects.Remove(current2);
            }
            list.Sort((a, b) => a.x.CompareTo(b.x));
            result = list;
        }
        return result;
    }
}