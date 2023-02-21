using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    //public Texture[] frames; // 存储动画帧的纹理数组
    public Sprite[] sprite;
    public float fps = 30.0f; // 每秒播放的帧数

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("NextFrame", 0, 1 / fps); // 每1/fps秒切换一次帧
    }

    void NextFrame()
    {
        if (sprite.Length == 0)
            return;
        spriteRenderer.sprite = sprite[currentFrame % sprite.Length]; // 更新纹理
        currentFrame++;
    }
}
