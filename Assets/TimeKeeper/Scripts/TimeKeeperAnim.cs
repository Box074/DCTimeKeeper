using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeperAnim : CommonAnimControl
{
    public Texture2D currentNormal;
    public Texture2D[] Textures;
    private MaterialPropertyBlock propBlock;
    private Dictionary<string, Texture2D> _texMap = new Dictionary<string, Texture2D>();
    private void Awake()
    {
        foreach (var v in Textures)
        {
            var name = v.name;
            _texMap[name] = v;
            if(!name.EndsWith("_n"))
            {
                var sr =  Sprite.Create(v, new Rect(0, 0, v.width, v.height), new Vector2(0.5f, 0.5f), 22);
                sr.name = name;
                spriteCache[name] = sr;
            }
        }

    }
    protected override void OnUpdateSprite()
    {
        base.OnUpdateSprite();
        var sprite = renderers[0].sprite;
        if (_texMap.TryGetValue(GetSpriteName() + "_n", out var bump))
        {
            currentNormal = bump;
            rendererMat.SetTexture("_BumpMap", bump);
        }
    }
}

