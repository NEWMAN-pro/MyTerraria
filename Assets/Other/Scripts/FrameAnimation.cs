using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    // �洢����֡����������
    public Sprite[] sprite;
    // ÿ�벥�ŵ�֡��
    public float fps = 10.0f;

    public SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    // �Ƿ�ʼ
    bool flag = false;

    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CreateDestory(float time)
    {
        if (flag) return;
        flag = true;
        float newTime = Mathf.Round(100f / time);
        if (newTime < 0) return;
        InvokeRepeating("NextFrame", 0, newTime / fps); // ÿnewTime/fps���л�һ��֡
    }

    public void Stop()
    {
        if (!flag) return;
        CancelInvoke("NextFrame");
        Debug.Log("+++");
        currentFrame = 0;
        spriteRenderer.sprite = null;
        flag = false;
    }

    void NextFrame()
    {
        if (sprite.Length == 0) return;
        spriteRenderer.sprite = sprite[currentFrame % sprite.Length]; // ��������
        currentFrame++;
    }
}
