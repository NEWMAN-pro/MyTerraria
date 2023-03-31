using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;
using System.Text.RegularExpressions;

public class PlayController : MonoBehaviour
{
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
    // 旋转的速度
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
    // 当前选择的物品ID
    public Item item;
    public byte inventoryID = 1;
    // 物品栏
    public Inventory inventory;
    // 背包
    public Backpack backpack;

    // 销毁方块
    public GameObject destory;

    // Start is called before the first frame update
    void Start()
    {
        this.name = "Player";
        velocity.y = -1f;
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        backpack = GameObject.Find("Backpack").GetComponent<Backpack>();
        DrawItem();
    }

    // Update is called once per frame
    void Update()
    {
        // 地图跟随玩家生成
        Map.instance.CreateMap(this.transform.position);

        if (inventory.selectID < 10 && inventoryID == inventory.selectID)
        {
            // 物品栏物品发生改变，重新绘制手部图案
            DrawItem();
            inventory.selectID = 10;
        }
        if (this.transform.GetComponent<PauseGame>().pause)
        {
            return;
        }

        // 从摄像机中心发射一条射线
        ray = cameraMove.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //Debug.DrawRay(rayPosi, ray.direction * 10, Color.red);
        Person();
        MoveUpdate();
        HeightUpdate();
        characterController.Move(velocity * Time.deltaTime);

        GetNumber();
        MouseButton();
        DetectionDrops();
    }

    // 处理鼠标事件
    public void MouseButton()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // 放置方块
            CreateBlock();
            // 单击动画
            this.GetComponent<AnimationState>().SetExcavateOne();
        }
        if (Input.GetMouseButton(0))
        {
            // 切换持续攻击动画
            this.GetComponent<AnimationState>().SetExcavate(true);
            DestroyBlock();
        }
        else
        {
            // 停止持续攻击动画
            this.GetComponent<AnimationState>().SetExcavate(false);
            // 停止销毁动画
            StopDestory();
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
            rayPosi = transform.position + new Vector3(0, 0.7f, 0);
            cameraMove.ThirdPerson(rayPosi, ray);
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

        if (velocity.x != 0 || velocity.z != 0)
        {
            // 切换行走
            this.GetComponent<AnimationState>().SetWalk(true);
        }
        else
        {
            //  停止行走
            this.GetComponent<AnimationState>().SetWalk(false);
        }
    }

    // 高度更新函数
    public void HeightUpdate()
    {
        if (characterController.isGrounded)
        {
            // 只有角色在地面才能跳跃
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
            }

            if (velocity.y < -1)
            {
                velocity.y = -1;
            }
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        if (velocity.y == -1)
        {
            this.GetComponent<AnimationState>().SetJump(false);
        }
        else
        {
            this.GetComponent<AnimationState>().SetJump(true);
        }
    }

    // 射线检测
    public bool RayDetection(out RaycastHit hitInfo)
    {
        return Physics.Raycast(rayPosi, ray.direction * 10, out hitInfo, 10, LayerMask.GetMask("Cube"));
    }

    // 球形检测
    public bool Sphere(out Collider[] colliders, float radius, LayerMask layerMask)
    {
        colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        if (colliders.Length == 0)
        {
            return false;
        }
        return true;
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

            // 判断选中的是否是宝箱
            string key = trans.GetComponent<Chunk>().GetBox(point - normal * 0.01f);
            if (key != "_")
            {
                // 是宝箱则打开宝箱
                GameObject.Find("UI").GetComponent<UI>().OpenBox(key);
                return;
            }

            if (item == null) return;
            // 碰撞点向角色移动一点距离，保证方块生成位置准确
            point += normal * 0.01f;
            byte create = trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, item.ID);
            if (create == 2)
            {
                // 方块生成位置不在当前区块内，则需改变区块trans
                Regex regex = new(@"\((-?\d+),(-?\d+),(-?\d+)\)");
                Match match = regex.Match(trans.name);
                if (match.Success)
                {
                    int x = int.Parse(match.Groups[1].Value);
                    int y = int.Parse(match.Groups[2].Value);
                    int z = int.Parse(match.Groups[3].Value);
                    Vector3i posi = new Vector3i(x, y, z) + new Vector3i((normal * 16f));
                    String newName = "(" + posi.x + "," + posi.y + "," + posi.z + ")";
                    trans = GameObject.Find(newName).transform;
                    //Debug.Log(trans.name);
                    // 在新区块生成方块
                    create = trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, item.ID);
                }
                else
                {
                    Debug.LogError("String format is invalid.");
                }

            }
            if(create == 0)
            {
                // 方块生成成功
                item.count--;
                byte backID = (byte)(inventoryID - 1);
                if(item.count == 0)
                {
                    // 如果方块放置完，则在背包中删除方块
                    backpack.SetItem(backID, null);
                    item = null;
                }
                else
                {
                    // 否则只减少数量
                    backpack.SetItem(backID, item);
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
                // 重置销毁动画
                StopDestory();
            }

            byte flag = trans.GetComponent<Chunk>().DestroyBlock(point);
            if (flag == 0)
            {
                // 销毁成功，置空
                LastTrans = null;
                // 停止销毁动画
                StopDestory();
            }
            else if(flag == 3)
            {
                // 销毁中，绘制销毁方块
                CreateDestory(trans, point);
            }
            else if(flag == 4)
            {
                // 更换方块，重置动画
                StopDestory();
            }
        }
    }

    // 启用绘制摧毁方块
    public void CreateDestory(Transform trans, Vector3 point)
    {
        Vector3 posi = Vector3.zero;
        float time = 0;
        trans.GetComponent<Chunk>().CreateDestroy(point, ref time , ref posi);
        destory.transform.position = posi;
        for(int i = 0; i < 6; i++)
        {
            destory.transform.GetChild(i).GetComponent<FrameAnimation>().CreateDestory(time);
        }
    }

    // 停止绘制摧毁方块
    public void StopDestory()
    {
        for(int i = 0; i < 6; i++)
        {
            destory.transform.GetChild(i).GetComponent<FrameAnimation>().Stop();
        }
    }

    // 更改选择的物品
    public void GetNumber()
    {
        // 鼠标中键滚动切换
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            inventoryID = (byte)(inventoryID == 9 ? (byte)0 : inventoryID + (byte)1);
            DrawItem();
            return;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            inventoryID = (byte)(inventoryID == 0 ? (byte)9 : inventoryID - (byte)1);
            DrawItem();
            return;
        }
        // 数字键切换
        for(byte i = 0; i <= 9; i++)
        {
            if(Input.GetKey(KeyCode.Alpha0 + i))
            {
                inventoryID = i;
                DrawItem();
                return;
            }
        }
    }

    // 绘制手部图案
    public void DrawItem()
    {
        item = inventory.GetItem(inventoryID);
        inventory.SetSelect(inventoryID);
        if (item == null)
        {
            this.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<CreateUI>().CreateBlank();
            this.transform.GetChild(3).GetChild(1).GetComponent<CreateUI>().CreateBlank();
            return;
        }
        this.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<CreateUI>().CreateBlockUI(BlockList.GetBlock(item.ID), true, 0.1f, Vector3.zero);
        this.transform.GetChild(3).GetChild(1).GetComponent<CreateUI>().CreateBlockUI(BlockList.GetBlock(item.ID), true, 0.1f, Vector3.zero);
    }

    // 检测掉落物
    public void DetectionDrops()
    {
        Collider[] colliders;
        if(Sphere(out colliders, 1f, LayerMask.GetMask("Drop")))
        {
            // 如果检测到掉落物
            foreach(Collider collider in colliders)
            {
                string name = collider.name;
                Item item = DropList.GetDrop(name);
                if(item == null)
                {
                    continue;
                }
                DropList.Destroy(name);
                backpack.Storage(item);
            }
        }
    }
}
