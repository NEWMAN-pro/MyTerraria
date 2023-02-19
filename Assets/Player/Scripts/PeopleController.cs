using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;

public class PeopleController : MonoBehaviour
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
    //���߷�Χ
    public int viewRange = 30;

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
            for (float z = transform.position.z - Chunk.width * 3; z < transform.position.z + Chunk.width * 3; z += Chunk.width)
            {
                int xx = Chunk.width * Mathf.FloorToInt(x / Chunk.width);
                int zz = Chunk.width * Mathf.FloorToInt(z / Chunk.width);
                if (!Map.instance.ChunkExists(xx, 0, zz))
                {
                    Map.instance.CreateChunk(new Vector3i(xx, 0, zz));
                }
            }
        }

        //gameObject.GetComponent<MakeCube>().Click(this.transform.position);

        //if(velocity.x == 0f && velocity.z == 0f && velocity.y == -1f)
        //{
        //    cameraMove.StopCameraMove();
        //}
        //else
        //{
            cameraMove.ActiveCameraMove();

            transform.RotateAround(this.transform.position, this.transform.up, rotateSpeed * Input.GetAxis("Mouse X"));
        //}

        MoveUpdate();
        HeightUpdate();

        characterController.Move(velocity * Time.deltaTime);

        check();
    }

    // �ƶ����º���
    public void MoveUpdate()
    {
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

    public void check()
    {
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(this.transform.position, this.transform.position - Camera.main.transform.position, out hitInfo, 100, LayerMask.GetMask("Cube"));
        if (hit)
        {
            // ��ȡ��ײ������
            Vector3 point = hitInfo.point;
            // ��ȡ�Է���ײ�����
            //Collider coll = hitInfo.collider;
            // ��ȡ�Է���Transform���
            Transform trans = hitInfo.transform;
            // ��ȡ�Է���������
            string name = hitInfo.collider.name;
            // ��ȡ��ײ��ķ�����
            Vector3 normal = hitInfo.normal;
            // ��ȡ�Է�����
            Vector3 otherPosition = trans.position;
            // �����Լ�����
            Vector3 myPosition = new Vector3(otherPosition.x + 1f * normal.x, otherPosition.y + 1f * normal.y, otherPosition.z + 1f * normal.z);

            Vector3i newPosition = trans.GetComponent<Chunk>().WorldTransferBlock(point);
            //Debug.Log(newPosition);
            //Vector3 wordPosition = trans.GetComponent<Chunk>().BlockTransferWorld(newPosition);
            //Debug.Log(wordPosition);

            //GameObject cube = Resources.Load<GameObject>("Prefab/Cube/0000");
            //Instantiate(cube, wordPosition, Quaternion.identity);

            //float dis = Vector3.Distance(this.transform.position, point);
            //Sphere.transform.localPosition = new Vector3(0, 0, 10 + Math.Min(dis, 5.0f));
        }
    }
}
