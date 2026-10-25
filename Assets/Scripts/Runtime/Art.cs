using System.Collections.Generic;
using UnityEngine;
namespace LittleElsewhere
{
    public static class Art
    {
        static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        public static Texture2D Texture(string name) { return Resources.Load<Texture2D>("HD/" + name) ?? Resources.Load<Texture2D>("Art/" + name); }
        public static Sprite Sprite(string name)
        {
            if (sprites.TryGetValue(name, out var sprite)) return sprite;
            var texture = Texture(name);
            if (texture == null) { Debug.LogError("Missing artwork: " + name); return null; }
            sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .12f), 64, 0, SpriteMeshType.FullRect);
            sprite.name = name;
            sprites[name] = sprite;
            return sprite;
        }
        public static SpriteRenderer Renderer(string name, Transform parent, float height, int order = 0)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite(name);
            renderer.sortingOrder = order;
            if (renderer.sprite != null) go.transform.localScale = Vector3.one * height / renderer.sprite.bounds.size.y;
            return renderer;
        }
    }
}
