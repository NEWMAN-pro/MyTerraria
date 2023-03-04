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

    void Start()
    {
        //Block block = new Block(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        //CreateUI(block, true, size, posi);
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
        AddFrontFace(block, true);
        if (!flag)
        {
            // ��Ҫ���Ʒ������ٻ��Ʒ���
            AddFrontFace(block, false);
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
    void AddFrontFace(Block block, bool flag)
    {
        //���4����
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0.8f, 0.4f, 0) + posi) * size);
        vertices.Add((new Vector3(0.8f, 1.4f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-0.8f, 1.4f, 0) + posi) * size);
        vertices.Add((new Vector3(-0.8f, 0.4f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(0.8f, 1.4f, 0) + posi) * size);
        vertices.Add((new Vector3(0f, 1.8f, 0) + posi) * size);
        vertices.Add((new Vector3(-0.8f, 1.4f, 0) + posi) * size);

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
            triangles.Add(0);
            triangles.Add(4);
            triangles.Add(3);

            //���ĸ�������
            triangles.Add(0);
            triangles.Add(5);
            triangles.Add(4);

            //�����������
            triangles.Add(6);
            triangles.Add(9);
            triangles.Add(8);

            //������������
            triangles.Add(8);
            triangles.Add(7);
            triangles.Add(6);
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
            triangles.Add(3);
            triangles.Add(4);
            triangles.Add(0);

            //���ĸ�������
            triangles.Add(4);
            triangles.Add(5);
            triangles.Add(0);

            //�����������
            triangles.Add(8);
            triangles.Add(9);
            triangles.Add(6);

            //������������
            triangles.Add(6);
            triangles.Add(7);
            triangles.Add(8);
        }

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }
}