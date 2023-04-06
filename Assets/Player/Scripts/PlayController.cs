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

    // 鼠标按下时间
    public bool dectoryFlag = false;

    // 手部物品对象
    GameObject handItem;
    GameObject handItem_;

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
            if (CreateBlock())
            {
                // 放置动画
                this.GetComponent<AnimationState>().SetExcavateOne();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            int layer = CheckLayer();
            if(layer != LayerMask.NameToLayer("Cube"))
            {
                // 攻击动画
                this.GetComponent<AnimationState>().SetSwordAttackOne();
            }
            dectoryFlag = false;
        }
        if (Input.GetMouseButton(0))
        {
            if (DestroyBlock())
            {
                // 切换持续挖掘动画
                this.GetComponent<AnimationState>().SetExcavate(true);
                dectoryFlag = true;
            }
            else
            {
                // 停止持续挖掘动画
                this.GetComponent<AnimationState>().SetExcavate(false);
                // 停止销毁动画
                StopDestory();
                // 停止销毁，置空
                LastTrans = null;
                dectoryFlag = false;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(dectoryFlag) { 
                // 停止持续挖掘动画
                this.GetComponent<AnimationState>().SetExcavate(false);
                // 停止销毁动画
                StopDestory();
                // 停止销毁，置空
                LastTrans = null;
                dectoryFlag = false;
            }
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
            rayPosi = ray.origin + ray.direction * 0.01f;
        }
        else
        {
            // 第三人称
            rayPosi = transform.position + new Vector3(0, 0.7f, 0) + ray.direction * 0.2f;
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
            if (Input.GetButton("Jump"))
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
        // 忽略玩家
        int layerMask = (1 << LayerMask.NameToLayer("First")) | (1 << LayerMask.NameToLayer("Third"));
        layerMask = ~layerMask;
        return Physics.Raycast(rayPosi, ray.direction * 10, out hitInfo, 5, layerMask);
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

    // 检测层级
    public int CheckLayer()
    {
        bool hit = RayDetection(out RaycastHit hitInfo);
        if (hit)
        {
            return hitInfo.transform.gameObject.layer;
        }
        return -1;
    }

    // 生成方块
    public bool CreateBlock()
    {
        bool hit = RayDetection(out RaycastHit hitInfo);
        if (hit)
        {
            // 获取碰撞点坐标
            Vector3 point = hitInfo.point;
            // 获取对方得Transform组件
            Transform trans = hitInfo.transform;
            // 获取碰撞点的法向量
            Vector3 normal = hitInfo.normal;

            if(trans.gameObject.layer != LayerMask.NameToLayer("Cube"))
            {
                // 如果目标不是方块，则不执行放置
                return false;
            }

            // 判断选中的是否是宝箱
            string key = trans.GetComponent<Chunk>().GetBox(point - normal * 0.01f);
            if (key != "_")
            {
                // 是宝箱则打开宝箱
                GameObject.Find("UI").GetComponent<UI>().OpenBox(key);
                return false;
            }

            if (item == null || item.type != Type.Block) return false;
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
            return true;
        }
        return false;
    }

    // 销毁方块
    public bool DestroyBlock()
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

            if (trans.gameObject.layer != LayerMask.NameToLayer("Cube"))
            {
                // 如果目标不是方块，则不执行销毁
                return false;
            }

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
            return true;
        }
        return false;
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
        Destroy(handItem);
        Destroy(handItem_);
        item = inventory.GetItem(inventoryID);
        inventory.SetSelect(inventoryID);
        this.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<CreateUI>().CreateBlank();
        this.transform.GetChild(3).GetChild(1).GetComponent<CreateUI>().CreateBlank();
        if (item == null)
        {
            this.transform.GetChild(6).GetChild(0).GetChild(0).gameObject.SetActive(true);
            this.transform.GetChild(6).GetChild(0).GetChild(1).gameObject.SetActive(false);
            return;
        }
        this.transform.GetChild(6).GetChild(0).GetChild(1).gameObject.SetActive(true);
        this.transform.GetChild(6).GetChild(0).GetChild(0).gameObject.SetActive(false);
        if (item.type == Type.Block)
        {
            // 如果是方块
            //this.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<CreateUI>().CreateBlockUI(BlockList.GetBlock(item.ID), true, 0.1f, Vector3.zero);
            //this.transform.GetChild(3).GetChild(1).GetComponent<CreateUI>().CreateBlockUI(BlockList.GetBlock(item.ID), true, 0.1f, Vector3.zero);
            this.transform.GetChild(6).GetChild(0).GetChild(1).GetComponent<CreateUI>().CreateBlockDrop(BlockList.GetBlock(item.ID), 0.3f, new Vector3(1f, -1f, 0));
            this.transform.GetChild(3).GetChild(1).GetComponent<CreateUI>().CreateBlockDrop(BlockList.GetBlock(item.ID), 0.3f, Vector3.zero);
        }
        else if(item.type == Type.Weapon)
        {
            // 如果是武器
            // 加载武器模型
            GameObject weaponPrefab = Resources.Load("Prefabs/Weapons/" + item.GetName()) as GameObject;
            handItem = Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity, this.transform.GetChild(6).GetChild(0).GetChild(1));
            handItem.transform.localPosition = new(0, -0.1f, 0);
            handItem.transform.localRotation = Quaternion.identity;
            handItem.transform.localEulerAngles = new(-15, -45, 0);
            handItem.layer = LayerMask.NameToLayer("First");
            handItem.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("First");
            handItem_ = Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity, this.transform.GetChild(3).GetChild(1));
            handItem_.transform.localPosition = new(0, -0.1f, 0);
            handItem_.transform.localRotation = Quaternion.identity;
            handItem_.layer = LayerMask.NameToLayer("Third");
            handItem_.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Third");
        }
    }

    // 单个打击怪物
    public void AttackMonster()
    {
        bool hit = RayDetection(out RaycastHit hitInfo);
        if (hit)
        {
            // 获取怪物信息
            Transform monster = hitInfo.transform;

            if(monster.gameObject.layer != LayerMask.NameToLayer("Monster"))
            {
                // 如果不是怪物，则不触发攻击效果
                return;
            }
            if(Vector3.Distance(monster.position, this.transform.position) > 5)
            {
                // 如果超出攻击范围，则不触发攻击效果
                return;
            }
            // 获取方向
            Vector3 direction = -hitInfo.normal.normalized;
            direction.y = 1;

            Debug.Log(monster.name);

            // 扣除怪物血量
            monster.GetComponent<Monster>().Hit(this.transform.GetComponent<PlayState>().damage, direction, 1);
        }
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
