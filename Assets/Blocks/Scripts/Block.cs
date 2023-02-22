using UnityEngine;

/// <summary>
/// ����ķ���
/// </summary>
public enum BlockDirection : byte
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3,
    Top = 4,
    Bottom = 5
}

/// <summary>
/// ������󣬴洢�����������Ϣ
/// </summary>
public class Block
{
    // �����ID
    public byte id;

    // ���������
    public string name;

    // ��������ʱ��
    public float destroyTime;

    // �����ͼ�꣬�������������Ϸ�ж�̬���ɵ�����
    public Texture icon;

    // ����ָ����ǰ�����泯�ķ���
    public BlockDirection direction = BlockDirection.Front;

    //ǰ����ͼ������
    public byte textureFrontX;
    public byte textureFrontY;

    // ������ͼ������
    public byte textureBackX;
    public byte textureBackY;

    // ������ͼ������
    public byte textureRightX;
    public byte textureRightY;

    // ������ͼ������
    public byte textureLeftX;
    public byte textureLeftY;

    // ������ͼ������
    public byte textureTopX;
    public byte textureTopY;

    // ������ͼ������
    public byte textureBottomX;
    public byte textureBottomY;

    // ����A��ķ���
    public Block(byte id, string name, float destroyTime, byte textureX, byte textureY)
        : this(id, name, destroyTime, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY)
    {
    }

    // ������A����������B�ķ���
    public Block(byte id, string name, float destroyTime, byte textureX, byte textureY, byte textureTopX, byte textureTopY)
        : this(id, name, destroyTime, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY, textureTopX, textureTopY, textureX, textureY)
    {
    }

    // ������A��������B����������C�ķ���
    public Block(byte id, string name, float destroyTime, byte textureX, byte textureY, byte textureTopX, byte textureTopY, byte textureBottomX, byte textureBottomY)
        : this(id, name, destroyTime, textureX, textureY, textureX, textureY, textureX, textureY, textureX, textureY, textureTopX, textureTopY, textureBottomX, textureBottomY)
    {
    }

    // ������A��������B��ǰ����C����������D�ķ���
    public Block(byte id, string name, float destroyTime, byte textureFrontX, byte textureFrontY, byte textureX, byte textureY, byte textureTopX, byte textureTopY, byte textureBottomX, byte textureBottomY)
        : this(id, name, destroyTime, textureFrontX, textureFrontY, textureX, textureY, textureX, textureY, textureX, textureY, textureTopX, textureTopY, textureBottomX, textureBottomY)
    {
    }

    // ��������ǰ���涼��һ���ķ���
    public Block(byte id, string name, float destroyTime, byte textureFrontX, byte textureFrontY, byte textureBackX, byte textureBackY, byte textureRightX, byte textureRightY,
        byte textureLeftX, byte textureLeftY, byte textureTopX, byte textureTopY, byte textureBottomX, byte textureBottomY)
    {
        this.id = id;
        this.name = name;
        this.destroyTime = destroyTime;

        this.textureFrontX = textureFrontX;
        this.textureFrontY = textureFrontY;

        this.textureBackX = textureBackX;
        this.textureBackY = textureBackY;

        this.textureRightX = textureRightX;
        this.textureRightY = textureRightY;

        this.textureLeftX = textureLeftX;
        this.textureLeftY = textureLeftY;

        this.textureTopX = textureTopX;
        this.textureTopY = textureTopY;

        this.textureBottomX = textureBottomX;
        this.textureBottomY = textureBottomY;
    }
}