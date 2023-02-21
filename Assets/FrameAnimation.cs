using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    //public Texture[] frames; // �洢����֡����������
    public Sprite[] sprite;
    public float fps = 30.0f; // ÿ�벥�ŵ�֡��

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("NextFrame", 0, 1 / fps); // ÿ1/fps���л�һ��֡
    }

    void NextFrame()
    {
        if (sprite.Length == 0)
            return;
        spriteRenderer.sprite = sprite[currentFrame % sprite.Length]; // ��������
        currentFrame++;
    }
}
