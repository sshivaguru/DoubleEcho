using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GroundHelper : MonoBehaviour
{
    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
        {
            Texture2D tex = new Texture2D(16, 16);
            Color[] colors = new Color[16 * 16];
            for (int i = 0; i < colors.Length; i++) colors[i] = new Color(0.3f, 0.3f, 0.3f, 1f);
            tex.SetPixels(colors);
            tex.Apply();

            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        }
    }
}
