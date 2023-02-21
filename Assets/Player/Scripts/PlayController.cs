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

    // Start is called before the first frame update
    void Start()
    {
        // �������
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
                //Y�������������16��Chunk������߶������256
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
        }
        if (person)
        {
            // ��һ�˳�
            cameraMove.FirstPerson();
        }
        else
        {
            // �����˳�
            cameraMove.ThirdPerson();
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
        Vector3 dir = this.transform.right * h + this.transform.forward * v;
        velocity.x = dir.x * speed;
        velocity.z = dir.z * speed;
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
        return Physics.Raycast(this.transform.position, Camera.main.transform.forward, out hitInfo, 10, LayerMask.GetMask("Cube"));
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

            // ��ײ�����ɫ�ƶ�һ����룬��֤��������λ��׼ȷ
            point += normal * 0.01f;
            if (trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position) == 2)
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
                    trans.GetComponent<Chunk>().CreateBlock(point, this.transform.position);
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

            // ������ϴ�ѡ��ķ��鲻ͬ�������
            if(trans != LastTrans)
            {
                LastTrans = trans;
                trans.GetComponent<Chunk>().setDestroyTime(point);
            }

            if (trans.GetComponent<Chunk>().DestroyBlock(point) == 0)
            {
                // ���ٳɹ����ÿ�
                LastTrans = null;
            }
        }
    }
}
