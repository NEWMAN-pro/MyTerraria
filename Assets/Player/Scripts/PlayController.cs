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
    // �Ƿ�ס������
    public bool mouse0Flag = false;
    // ��ǰѡ�����ƷID
    public byte itemID = 1;
    // ��Ʒ��
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
        // ��ͼ�����������
        Map.instance.CreateMap(this.transform.position);
        
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
                // ��ʼ����ʱ�����ֱ�λ��
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
            cameraMove.ThirdPerson();
            rayPosi = transform.position + new Vector3(0, 0.7f, 0);
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
            // �ڶ��Ȳ�
            Transform trans = this.transform.GetChild(5);
            trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
            trans = this.transform.GetChild(4);
            trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
            // �ڶ��ֲ�
            if (!mouse0Flag)
            {
                // ������������£���ֹͣ���߰ڶ�
                trans = this.transform.GetChild(3);
                trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
            }
            trans = this.transform.GetChild(2);
            trans.GetComponent<Wobble>().Move(160, -40, 40, trans.transform, trans.position, trans.right);
        }
        else
        {
            // ��λ�Ȳ�
            Transform trans = this.transform.GetChild(5);
            trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
            trans = this.transform.GetChild(4);
            trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
            // ��λ�ֲ�
            if (!mouse0Flag)
            {
                trans = this.transform.GetChild(3);
                trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
            }
            trans = this.transform.GetChild(2);
            trans.GetComponent<Wobble>().Recovery(trans.transform, Vector3.zero);
        }
    }

    // �߶ȸ��º���
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

    // ���߼��
    public bool RayDetection(out RaycastHit hitInfo)
    {
        return Physics.Raycast(rayPosi, ray.direction * 10, out hitInfo, 10, LayerMask.GetMask("Cube"));
    }

    // ���ɷ���
    public void CreateBlock(byte blockID)
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

            // ��ײ�����ɫ�ƶ�һ����룬��֤��������λ��׼ȷ
            point += normal * 0.01f;
            if (trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, blockID) == 2)
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
                    trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position, blockID);
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

    // ��ȡ���µ����ּ�
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

    // �����ֲ�ͼ��
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
