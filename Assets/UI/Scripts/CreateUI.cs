using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CreateUI : MonoBehaviour
{
    private Mesh mesh;

    //����Ҫ�ĵ�
    private List<Vector3> vertices = new List<Vector3>();
    //����������ʱ�õ���vertices��index
    private List<int> triangles = new List<int>();

    //���е�uv��Ϣ
    private List<Vector2> uv = new List<Vector2>();
    //uv��ͼÿ��ÿ�еĿ��(0~1)�������ҵ���ͼ��32��32�ģ�������1/32
    public static float textureOffset = 1 / 32f;
    //��UV��΢��Сһ�㣬����������Աߵ���ͼ
    public static float shrinkSize = 0.001f;

    // ���ƴ�С
    public float size;
    // ����λ��
    public Vector3 posi;

    // ���ٷ�����ͼ��ֵ
    public float add;
    // ÿ�벥�ŵ�֡��
    public float fps = 30.0f;
    // ���ٷ���
    Block block;

    void Start()
    {
        //Block block = new Block(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        //CreateUI(block, true, size, posi);
        block = BlockList.GetBlock(9);
    }

    // ���ƿհ�
    public void CreateBlank()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        //���¼��㶥��ͷ���
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //�����ɺõ��渳ֵ�����
        GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // ���Ʒ���UI
    public void CreateBlockUI(Block block, bool flag, float size, Vector3 posi)
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        this.size = size;
        this.posi = posi;
        mesh = new Mesh();

        // �Ȼ�������
        AddFace(block, true);
        if (!flag)
        {
            // ��Ҫ���Ʒ������ٻ��Ʒ���
            AddFace(block, false);
        }

        //Ϊ���index��ֵ
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        //���¼��㶥��ͷ���
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //�����ɺõ��渳ֵ�����
        GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // ���Ʒ����UI��ʾ, flag������
    void AddFace(Block block, bool flag)
    {
        //���12����
        // ����
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0.87f, 0.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0.87f, 1.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);

        // ǰ��
        vertices.Add((new Vector3(-0.87f, 0.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-0.87f, 1.5f, 0) + posi) * size);

        // ����
        vertices.Add((new Vector3(-0.87f, 1.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(0.87f, 1.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0f, 2f, 0) + posi) * size);

        if (flag)
        {
            //��һ��������
            triangles.Add(0);
            triangles.Add(3);
            triangles.Add(2);

            //�ڶ���������
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(0);

            //������������
            triangles.Add(4);
            triangles.Add(7);
            triangles.Add(6);

            //���ĸ�������
            triangles.Add(6);
            triangles.Add(5);
            triangles.Add(4);

            //�����������
            triangles.Add(8);
            triangles.Add(11);
            triangles.Add(10);

            //������������
            triangles.Add(10);
            triangles.Add(9);
            triangles.Add(8);
        }
        else
        {
            //��һ��������
            triangles.Add(2);
            triangles.Add(3);
            triangles.Add(0);

            //�ڶ���������
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);

            //������������
            triangles.Add(6);
            triangles.Add(7);
            triangles.Add(4);

            //���ĸ�������
            triangles.Add(4);
            triangles.Add(5);
            triangles.Add(6);

            //�����������
            triangles.Add(10);
            triangles.Add(11);
            triangles.Add(8);

            //������������
            triangles.Add(8);
            triangles.Add(9);
            triangles.Add(10);
        }

        //���UV����㣬������12����ѭ����˳��һ��
        // ����
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));

        // ǰ��
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));

        // ����
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    // ���Ʒ��������ͼ
    public void CreateBlockDrop(Block block, float size, Vector3 posi)
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        this.size = size;
        this.posi = posi;
        mesh = new Mesh();

        AddFrontFace(block);
        AddBackFace(block);
        AddRightFace(block);
        AddLeftFace(block);
        AddTopFace(block);
        AddBottomFace(block);

        //Ϊ���index��ֵ
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        //���¼��㶥��ͷ���
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //�����ɺõ��渳ֵ�����
        GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    //ǰ��
    void AddFrontFace(Block block)
    {
        //��һ��������
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //�ڶ���������
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //���4����
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 0, 1) + posi) * size);
        vertices.Add((new Vector3(0, 1, 1) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //����
    void AddBackFace(Block block)
    {
        //��һ��������
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //�ڶ���������
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //���4����
        vertices.Add((new Vector3(-1, 0, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 0, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 1) + posi) * size);

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //����
    void AddRightFace(Block block)
    {
        //��һ��������
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //�ڶ���������
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //���4����
        vertices.Add((new Vector3(0, 0, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 0, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 1) + posi) * size);
        vertices.Add((new Vector3(0, 1, 1) + posi) * size);

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //����
    void AddLeftFace(Block block)
    {
        //��һ��������
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //�ڶ���������
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //���4����
        vertices.Add((new Vector3(-1, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 0) + posi) * size);

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //����
    void AddTopFace(Block block)
    {
        //��һ��������
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);

        //�ڶ���������
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);


        //���4����
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 0) + posi) * size);

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //����
    void AddBottomFace(Block block)
    {
        //��һ��������
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);

        //�ڶ���������
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);


        //���4����
        vertices.Add((new Vector3(-1, 0, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 0, 1) + posi) * size);
        vertices.Add((new Vector3(0, 0, 1) + posi) * size);
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }
}