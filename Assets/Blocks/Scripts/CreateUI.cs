using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class CreateUI : MonoBehaviour
{
    private Mesh mesh;

    //面需要的点
    private List<Vector3> vertices = new List<Vector3>();
    //生成三边面时用到的vertices的index
    private List<int> triangles = new List<int>();

    //所有的uv信息
    private List<Vector2> uv = new List<Vector2>();
    //uv贴图每行每列的宽度(0~1)，这里我的贴图是32×32的，所以是1/32
    public static float textureOffset = 1 / 32f;
    //让UV稍微缩小一点，避免出现它旁边的贴图
    public static float shrinkSize = 0.001f;

    // 绘制大小
    public float size;
    // 绘制位置
    public Vector3 posi;

    void Start()
    {
        //Block block = new Block(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        //CreateUI(block, true, size, posi);
    }

    // 绘制空白
    public void CreateBlank()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        //重新计算顶点和法线
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //将生成好的面赋值给组件
        GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // 绘制方块UI
    public void CreateBlockUI(Block block, bool flag, float size, Vector3 posi)
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        this.size = size;
        this.posi = posi;
        mesh = new Mesh();

        // 先绘制正面
        AddFrontFace(block, true);
        if (!flag)
        {
            // 需要绘制反面则再绘制反面
            AddFrontFace(block, false);
        }

        //为点和index赋值
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        //重新计算顶点和法线
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //将生成好的面赋值给组件
        GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // 绘制方块的UI显示, flag正反面
    void AddFrontFace(Block block, bool flag)
    {
        //添加4个点
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
            //第一个三角面
            triangles.Add(0);
            triangles.Add(3);
            triangles.Add(2);

            //第二个三角面
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(0);

            //第三个三角面
            triangles.Add(0);
            triangles.Add(4);
            triangles.Add(3);

            //第四个三角面
            triangles.Add(0);
            triangles.Add(5);
            triangles.Add(4);

            //第五个三角面
            triangles.Add(6);
            triangles.Add(9);
            triangles.Add(8);

            //第六个三角面
            triangles.Add(8);
            triangles.Add(7);
            triangles.Add(6);
        }
        else
        {
            //第一个三角面
            triangles.Add(2);
            triangles.Add(3);
            triangles.Add(0);

            //第二个三角面
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);

            //第三个三角面
            triangles.Add(3);
            triangles.Add(4);
            triangles.Add(0);

            //第四个三角面
            triangles.Add(4);
            triangles.Add(5);
            triangles.Add(0);

            //第五个三角面
            triangles.Add(8);
            triangles.Add(9);
            triangles.Add(6);

            //第六个三角面
            triangles.Add(6);
            triangles.Add(7);
            triangles.Add(8);
        }

        //添加UV坐标点，跟上面4个点循环的顺序一致
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