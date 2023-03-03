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
    // 视线范围
    public int viewRange = 30;
    // 视角
    public bool person = true;
    // 上次选择的方块
    public Transform LastTrans;
    // 射线
    Ray ray;
    // 射线发射点
    public Vector3 rayPosi;
    // 是否按住鼠标左键
    public bool mouse0Flag = false;
    // 当前选择的物品ID
    public byte itemID = 1;
    // 物品栏
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        velocity.y = -1f;
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        DrawItem(1);
    }

    // Update is called once per frame
    void Update()
    {
        // 地图跟随玩家生成
        Map.instance.CreateMap(this.transform.position);
        
        // 从摄像机中心发射一条射线
        ray = cameraMove.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(rayPosi, ray.direction * 10, Color.red);
        Person();
        MoveUpdate();
        HeightUpdate();
        characterController.Move(velocity * Time.deltaTime);

        GetNumber();
        MouseButton();
    }

    // 处理鼠标事件
    public void MouseButton()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(itemID < 10)
            {
                CreateBlock(itemID);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Transform trans = this.transform.GetChild(3);
            if (!mouse0Flag)
            {
                // 开始按下时重置手臂位置
                trans.GetComponent<Wobble>().Recovery(trans, Vector3.zero);
            }
            mouse0Flag = true;
            trans.GetComponent<Wobble>().Move(160, -90, -20, trans, trans.position, trans.right);
            trans = this.transform.GetChild(6).GetChild(0);
            trans.GetComponent<Wobble>().Move(160, -20, 40, trans, trans.position, trans.right);
            DestroyBlock();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Transform trans = this.transform.GetChild(3);
            trans.GetComponent<Wobble>().Recovery(trans, Vector3.zero);
            trans = this.transform.GetChild(6).GetChild(0);
            trans.GetComponent<Wobble>().Recovery(trans, new Vector3(-120, 0, 0));
            mouse0Flag = false;
            // 停止销毁，置空
            LastTrans = null;
        }
    }

    // 视角更新
    public void Person()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            person = !person;
            cameraMove.SetMash();
        }
        if (person)
        {
            // 第一人称
            cameraMove.FirstPerson();
            rayPosi = ray.origin;
        }
        else
        {
            // 第三人称
            cameraMove.ThirdPerson();
            rayPosi = transform.position + new Vector3(0, 0.7f, 0);
        }
    }

    // 移动更新函数
    public void MoveUpdate()
    {
        // 旋转
        transform.RotateAround(this.transform.position, this.transform.up, rotateSpeed * Input.GetAxis("Mouse X"));
        // 左右
        float h = Input.GetAxis("Horizontal");
        // 上下
        float v = Input.GetAxis("Vertical");

        //Vector3 dir = Vector3.right * h + Vector3.forward * v;
        Vector3 dir = (this.transform.right * h + this.transform.forward * v).normalized;
        velocity.x = dir.x * speed;
        velocity.z = dir.z * speed;

        if(velocity.x != 0 || velocity.z != 0)
        {
            // 摆动腿部
            Transform trans = this.transform.GetChild(5);
            trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
            trans = this.transform.GetChild(4);
            trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
            // 摆动手部
            if (!mouse0Flag)
            {
                // 如果鼠标左键按下，则停止行走摆动
                trans = this.transform.GetChild(3);
                trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
            }
            trans = this.transform.GetChild(2);
            trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
        }
        else
        {
            // 复位腿部
            Transform trans = this.transform.GetChild(5);
            trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
            trans = this.transform.GetChild(4);
            trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
            // 复位手部
            if (!mouse0Flag)
            {
                trans = this.transform.GetChild(3);
                trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
            }
            trans = this.transform.GetChild(2);
            trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
        }
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
        return Physics.Raycast(rayPosi, ray.direction * 10, out hitInfo, 10, LayerMask.GetMask("Cube"));
    }

    // 生成方块
    public void CreateBlock(byte blockID)
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
            if (trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, blockID) == 2)
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
                    trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, blockID);
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

            // 碰撞点向角色远移一点距离，保证销毁的方块位置准确
            point -= normal * 0.01f;

            // 如果与上次选择的区块不同，则更新
            if(trans != LastTrans)
            {
                LastTrans = trans;
                trans.GetComponent<Chunk>().SetDestroyTime(point);
            }

            if (trans.GetComponent<Chunk>().DestroyBlock(point) == 0)
            {
                // 销毁成功，置空
                LastTrans = null;
            }
        }
    }

    // 获取按下的数字键
    public void GetNumber()
    {
        for(byte i = 0; i <= 9; i++)
        {
            if(Input.GetKey(KeyCode.Alpha0 + i))
            {
                DrawItem(i);
                return;
            }
        }
    }

    // 绘制手部图案
    public void DrawItem(byte i)
    {
        Item item = new();
        item = inventory.GetItem(i);
        if (item == null) return;
        if (BlockList.GetBlock(item.ID) != null)
        {
            inventory.SetSelect(i);
            itemID = item.ID;
            this.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<CreateUI>().CreateBlockUI(BlockList.GetBlock(itemID), true, 0.1f, Vector3.zero);
            this.transform.GetChild(3).GetChild(1).GetComponent<CreateUI>().CreateBlockUI(BlockList.GetBlock(itemID), true, 0.1f, Vector3.zero);
        }
    }
}
