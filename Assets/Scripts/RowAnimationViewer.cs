using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RowAnimationViewer : MonoBehaviour
{
    public int rowNumber = 0;
    public float fps = 8f;

    private SpriteRenderer sr;
    private Sprite[] rowSprites;
    private float timer;
    private int currentFrame;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        LoadRowSprites();
    }

    private void LoadRowSprites()
    {
        Object[] allAssets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/SATYR_sprite_sheet/SPRITE_SHEET.png");
        if (allAssets == null || allAssets.Length == 0) return;

        System.Collections.Generic.List<Sprite> list = new System.Collections.Generic.List<Sprite>();

        if (rowNumber == 9)
        {
            // Merged Row 9 + Row 10 Attack Animation
            AddSpritesForPrefix(allAssets, list, "Satyr_R9_C");
            AddSpritesForPrefix(allAssets, list, "Satyr_R10_C");
        }
        else
        {
            AddSpritesForPrefix(allAssets, list, $"Satyr_R{rowNumber}_C");
        }

        rowSprites = list.ToArray();
        if (rowSprites.Length > 0 && sr != null)
        {
            sr.sprite = rowSprites[0];
            sr.color = Color.white;
        }
    }

    private void AddSpritesForPrefix(Object[] assets, System.Collections.Generic.List<Sprite> list, string prefix)
    {
        System.Collections.Generic.List<Sprite> subList = new System.Collections.Generic.List<Sprite>();
        foreach (Object obj in assets)
        {
            if (obj is Sprite s && s.name.StartsWith(prefix))
            {
                subList.Add(s);
            }
        }

        subList.Sort((a, b) => {
            int colA = GetColIndex(a.name);
            int colB = GetColIndex(b.name);
            return colA.CompareTo(colB);
        });

        list.AddRange(subList);
    }

    private int GetColIndex(string name)
    {
        int cIdx = name.IndexOf("_C");
        if (cIdx >= 0)
        {
            int val;
            if (int.TryParse(name.Substring(cIdx + 2), out val)) return val;
        }
        return 0;
    }

    void Update()
    {
        if (rowSprites == null || rowSprites.Length == 0 || sr == null) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fps)
        {
            timer -= 1f / fps;
            currentFrame = (currentFrame + 1) % rowSprites.Length;
            sr.sprite = rowSprites[currentFrame];
        }
    }
}
