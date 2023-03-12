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
    // �ƶ��ٶ�
    public float speed;
    // ����
    public float gravity = 9.8f;
    // �ٶ�����
    public Vector3 velocity = Vector3.zero;
    // ��Ծ�߶�
    public float jumpHeight = 1.2f;
    // ��ת���ٶȡ�
    public float rotateSpeed = 2;
    // ���߷�Χ
    public int viewRange = 30;
    // �ӽ�
    public bool person = true;
    // �ϴ�ѡ��ķ���
    public Transform LastTrans;
    // ����
    Ray ray;
    // ���߷����
    public Vector3 rayPosi;
    // ��ǰѡ�����ƷID
    public Item item;
    public byte inventoryID = 1;
    // ��Ʒ��
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        this.name = "Player";
        velocity.y = -1f;
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        DrawItem();
    }

    // Update is called once per frame
    void Update()
    {
        // ��ͼ�����������
        Map.instance.CreateMap(this.transform.position);

        if(inventory.selectID < 10 && inventoryID == inventory.selectID)
        {
            // ��Ʒ����Ʒ�����ı䣬���»����ֲ�ͼ��
            DrawItem();
            inventory.selectID = 10;
        }
        if (this.transform.GetComponent<PauseGame>().pause)
        {
            return;
        }
        
        // ����������ķ���һ������
        ray = cameraMove.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(rayPosi, ray.direction * 10, Color.red);
        Person();
        MoveUpdate();
        HeightUpdate();
        characterController.Move(velocity * Time.deltaTime);

        GetNumber();
        MouseButton();
    }

    // ��������¼�
    public void MouseButton()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // ���÷���
            CreateBlock();
            // ��������
            this.GetComponent<AnimationState>().SetExcavateOne();
        }
        if (Input.GetMouseButton(0))
        {
            // �л�������������
            this.GetComponent<AnimationState>().SetExcavate(true);
            DestroyBlock();
        }
        if (Input.GetMouseButtonUp(0))
        {
            // ֹͣ������������
            this.GetComponent<AnimationState>().SetExcavate(false);
            // ֹͣ���٣��ÿ�
            LastTrans = null;
        }
    }

    // �ӽǸ���
    public void Person()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            person = !person;
            cameraMove.SetMash();
        }
        if (person)
        {
            // ��һ�˳�
            cameraMove.FirstPerson();
            rayPosi = ray.origin;
        }
        else
        {
            // �����˳�
            rayPosi = transform.position + new Vector3(0, 0.7f, 0);
            cameraMove.ThirdPerson(rayPosi, ray);
        }
    }

    // �ƶ����º���
    public void MoveUpdate()
    {
        // ��ת
        transform.RotateAround(this.transform.position, this.transform.up, rotateSpeed * Input.GetAxis("Mouse X"));
        // ����
        float h = Input.GetAxis("Horizontal");
        // ����
        float v = Input.GetAxis("Vertical");

        //Vector3 dir = Vector3.right * h + Vector3.forward * v;
        Vector3 dir = (this.transform.right * h + this.transform.forward * v).normalized;
        velocity.x = dir.x * speed;
        velocity.z = dir.z * speed;

        if(velocity.x != 0 || velocity.z != 0)
        {
            // �л�����
            this.GetComponent<AnimationState>().SetWalk(true);
        }
        else
        {
            //  ֹͣ����
            this.GetComponent<AnimationState>().SetWalk(false);
        }
    }

    // �߶ȸ��º���
    public void HeightUpdate()
    {
        if (characterController.isGrounded)
        {
            // ֻ�н�ɫ�ڵ��������Ծ
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
        
        if(velocity.y == -1)
        {
            this.GetComponent<AnimationState>().SetJump(false);
        }
        else
        {
            this.GetComponent<AnimationState>().SetJump(true);
        }
    }

    // ���߼��
    public bool RayDetection(out RaycastHit hitInfo)
    {
        return Physics.Raycast(rayPosi, ray.direction * 10, out hitInfo, 10, LayerMask.GetMask("Cube"));
    }

    // ���ɷ���
    public void CreateBlock()
    {
        RaycastHit hitInfo;
        bool hit = RayDetection(out hitInfo);
        if (hit)
        {
            // ��ȡ��ײ������
            Vector3 point = hitInfo.point;
            // ��ȡ�Է���Transform���
            Transform trans = hitInfo.transform;
            // ��ȡ��ײ��ķ�����
            Vector3 normal = hitInfo.normal;

            // �ж�ѡ�е��Ƿ��Ǳ���
            string key = trans.GetComponent<Chunk>().GetBox(point - normal * 0.01f);
            if(key != "_")
            {
                // �Ǳ�����򿪱���
                GameObject.Find("UI").GetComponent<UI>().OpenBox(key);
                return;
            }

            // ��ײ�����ɫ�ƶ�һ����룬��֤��������λ��׼ȷ
            point += normal * 0.01f;
            if (item != null && trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, item.ID) == 2)
            {
                // ��������λ�ò��ڵ�ǰ�����ڣ�����ı�����trans
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
                    // �����������ɷ���
                    trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, item.ID);
                }
                else
                {
                    Debug.LogError("String format is invalid.");
                }

            }
        
        }
    }

    // ���ٷ���
    public void DestroyBlock()
    {
        RaycastHit hitInfo;
        bool hit = RayDetection(out hitInfo);
        if (hit)
        {
            // ��ȡ��ײ������
            Vector3 point = hitInfo.point;
            // ��ȡ�Է���Transform���
            Transform trans = hitInfo.transform;
            // ��ȡ��ײ��ķ�����
            Vector3 normal = hitInfo.normal;

            // ��ײ�����ɫԶ��һ����룬��֤���ٵķ���λ��׼ȷ
            point -= normal * 0.01f;

            // ������ϴ�ѡ������鲻ͬ�������
            if(trans != LastTrans)
            {
                LastTrans = trans;
                trans.GetComponent<Chunk>().SetDestroyTime(point);
            }

            if (trans.GetComponent<Chunk>().DestroyBlock(point) == 0)
            {
                // ���ٳɹ����ÿ�
                LastTrans = null;
            }
        }
    }

    // ����ѡ�����Ʒ
    public void GetNumber()
    {
        // ����м������л�
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
        // ���ּ��л�
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

    // �����ֲ�ͼ��
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
}
