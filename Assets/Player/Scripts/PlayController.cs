using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;
using System.Text.RegularExpressions;

public class PlayController : MonoBehaviour
{
    public GameObject Sphere;
    public CameraMove cameraMove;
    public CharacterController characterController;
    // 移动速度
    public float speed;
    // 重力
    public float gravity = 9.8f;
    // 速度向量
    public Vector3 velocity = Vector3.zero;
    // 跳跃高度
    public float jumpHeight = 1.2f;
    // 旋转的速度。
    public float rotateSpeed = 2;
    //视线范围
    public int viewRange = 30;

    // Start is called before the first frame update
    void Start()
    {
        // 隐藏鼠标
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        velocity.y = -1f;
    }

    // Update is called once per frame
    void Update()
    {

        for (float x = transform.position.x - Chunk.width * 3; x < transform.position.x + Chunk.width * 3; x += Chunk.width)
        {
            for (float y = transform.position.y - Chunk.height * 3; y < transform.position.y + Chunk.height * 3; y += Chunk.height)
            {
                //Y轴上是允许最大16个Chunk，方块高度最大是256
                if (y <= Chunk.height * 16 && y > 0)
                {
                    for (float z = transform.position.z - Chunk.width * 3; z < transform.position.z + Chunk.width * 3; z += Chunk.width)
                    {
                        int xx = Chunk.width * Mathf.FloorToInt(x / Chunk.width);
                        int yy = Chunk.height * Mathf.FloorToInt(y / Chunk.height);
                        int zz = Chunk.width * Mathf.FloorToInt(z / Chunk.width);
                        if (!Map.instance.ChunkExists(xx, yy, zz))
                        {
                            Map.instance.CreateChunk(new Vector3i(xx, yy, zz));
                        }
                    }
                }
            }
        }

        //gameObject.GetComponent<MakeCube>().Click(this.transform.position);

        // 第三人称摄像机移动方式
        cameraMove.ActiveCameraMove();

        transform.RotateAround(this.transform.position, this.transform.up, rotateSpeed * Input.GetAxis("Mouse X"));

        MoveUpdate();
        HeightUpdate();
        characterController.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(1))
        {
            // 鼠标左键点击事件
            CreateBlock();
        }
        if (Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }

    }

    // 移动更新函数
    public void MoveUpdate()
    {
        // 左右
        float h = Input.GetAxis("Horizontal");
        // 上下
        float v = Input.GetAxis("Vertical");

        //Vector3 dir = Vector3.right * h + Vector3.forward * v;
        Vector3 dir = this.transform.right * h + this.transform.forward * v;
        velocity.x = dir.x * speed;
        velocity.z = dir.z * speed;
    }

    // 高度更新函数
    public void HeightUpdate()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
            }

            if(velocity.y < -1)
            {
                velocity.y = -1;
            }
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    // 射线检测
    public bool RayDetection(out RaycastHit hitInfo)
    {
        return Physics.Raycast(this.transform.position, this.transform.position - Camera.main.transform.position, out hitInfo, 10, LayerMask.GetMask("Cube"));
    }

    // 生成方块
    public void CreateBlock()
    {
        RaycastHit hitInfo;
        bool hit = RayDetection(out hitInfo);
        if (hit)
        {
            // 获取碰撞点坐标
            Vector3 point = hitInfo.point;
            // 获取对方得Transform组件
            Transform trans = hitInfo.transform;
            // 获取碰撞点的法向量
            Vector3 normal = hitInfo.normal;

            // 碰撞点向角色移动一点距离，保证方块生成位置准确
            point += normal * 0.01f;
            if (trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position) == 2)
            {
                // 方块生成位置不在当前区块内，则需改变区块trans
                Regex regex = new Regex(@"\((-?\d+),(-?\d+),(-?\d+)\)");
                Match match = regex.Match(trans.name);
                if (match.Success)
                {
                    int x = int.Parse(match.Groups[1].Value);
                    int y = int.Parse(match.Groups[2].Value);
                    int z = int.Parse(match.Groups[3].Value);
                    Vector3i posi = new Vector3i(x, y, z) + new Vector3i((normal * 16f));
                    String newName = "(" + posi.x + "," + posi.y + "," + posi.z + ")";
                    trans = GameObject.Find(newName).transform;
                    Debug.Log(trans.name);
                    // 在新区块生成方块
                    trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position);
                }
                else
                {
                    Debug.LogError("String format is invalid.");
                }

            }
        
        }
    }

    // 销毁方块
    public void DestroyBlock()
    {
        RaycastHit hitInfo;
        bool hit = RayDetection(out hitInfo);
        if (hit)
        {
            // 获取碰撞点坐标
            Vector3 point = hitInfo.point;
            // 获取对方得Transform组件
            Transform trans = hitInfo.transform;
            // 获取碰撞点的法向量
            Vector3 normal = hitInfo.normal;

            // 碰撞点向角色移动一点距离，保证销毁的方块位置准确
            point -= normal * 0.01f;

            trans.GetComponent<Chunk>().DestroyBlock(point);
        }
    }
}
