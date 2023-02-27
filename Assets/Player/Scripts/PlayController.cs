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
        // 地图跟随玩家生成
        Map.instance.CreateMap(this.transform.position);
        
        // 从摄像机中心发射一条射线
        ray = cameraMove.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(rayPosi, ray.direction * 10, Color.red);
        Person();
        MoveUpdate();
        HeightUpdate();
        characterController.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(1))
        {
            CreateBlock();
        }
        if (Input.GetMouseButton(0))
        {
            DestroyBlock();
        }
        if (Input.GetMouseButtonUp(0))
        {
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
}
