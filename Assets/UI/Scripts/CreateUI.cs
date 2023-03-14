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

    // 销毁方块贴图增值
    public float add;
    // 每秒播放的帧数
    public float fps = 30.0f;
    // 销毁方块
    Block block;

    void Start()
    {
        //Block block = new Block(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        //CreateUI(block, true, size, posi);
        block = BlockList.GetBlock(9);
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
        AddFace(block, true);
        if (!flag)
        {
            // 需要绘制反面则再绘制反面
            AddFace(block, false);
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
    void AddFace(Block block, bool flag)
    {
        //添加12个点
        // 侧面
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0.87f, 0.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0.87f, 1.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);

        // 前面
        vertices.Add((new Vector3(-0.87f, 0.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-0.87f, 1.5f, 0) + posi) * size);

        // 顶面
        vertices.Add((new Vector3(-0.87f, 1.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(0.87f, 1.5f, 0) + posi) * size);
        vertices.Add((new Vector3(0f, 2f, 0) + posi) * size);

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
            triangles.Add(4);
            triangles.Add(7);
            triangles.Add(6);

            //第四个三角面
            triangles.Add(6);
            triangles.Add(5);
            triangles.Add(4);

            //第五个三角面
            triangles.Add(8);
            triangles.Add(11);
            triangles.Add(10);

            //第六个三角面
            triangles.Add(10);
            triangles.Add(9);
            triangles.Add(8);
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
            triangles.Add(6);
            triangles.Add(7);
            triangles.Add(4);

            //第四个三角面
            triangles.Add(4);
            triangles.Add(5);
            triangles.Add(6);

            //第五个三角面
            triangles.Add(10);
            triangles.Add(11);
            triangles.Add(8);

            //第六个三角面
            triangles.Add(8);
            triangles.Add(9);
            triangles.Add(10);
        }

        //添加UV坐标点，跟上面12个点循环的顺序一致
        // 侧面
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));

        // 前面
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));

        // 上面
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    // 绘制方块掉落贴图
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

    //前面
    void AddFrontFace(Block block)
    {
        //第一个三角面
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //第二个三角面
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //添加4个点
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 0, 1) + posi) * size);
        vertices.Add((new Vector3(0, 1, 1) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //背面
    void AddBackFace(Block block)
    {
        //第一个三角面
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //第二个三角面
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //添加4个点
        vertices.Add((new Vector3(-1, 0, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 0, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 1) + posi) * size);

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //右面
    void AddRightFace(Block block)
    {
        //第一个三角面
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //第二个三角面
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //添加4个点
        vertices.Add((new Vector3(0, 0, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 0, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 1) + posi) * size);
        vertices.Add((new Vector3(0, 1, 1) + posi) * size);

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //左面
    void AddLeftFace(Block block)
    {
        //第一个三角面
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);

        //第二个三角面
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);


        //添加4个点
        vertices.Add((new Vector3(-1, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 0) + posi) * size);

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //上面
    void AddTopFace(Block block)
    {
        //第一个三角面
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);

        //第二个三角面
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);


        //添加4个点
        vertices.Add((new Vector3(0, 1, 0) + posi) * size);
        vertices.Add((new Vector3(0, 1, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 1) + posi) * size);
        vertices.Add((new Vector3(-1, 1, 0) + posi) * size);

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }

    //下面
    void AddBottomFace(Block block)
    {
        //第一个三角面
        triangles.Add(1 + vertices.Count);
        triangles.Add(0 + vertices.Count);
        triangles.Add(3 + vertices.Count);

        //第二个三角面
        triangles.Add(3 + vertices.Count);
        triangles.Add(2 + vertices.Count);
        triangles.Add(1 + vertices.Count);


        //添加4个点
        vertices.Add((new Vector3(-1, 0, 0) + posi) * size);
        vertices.Add((new Vector3(-1, 0, 1) + posi) * size);
        vertices.Add((new Vector3(0, 0, 1) + posi) * size);
        vertices.Add((new Vector3(0, 0, 0) + posi) * size);

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
        uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
    }
}