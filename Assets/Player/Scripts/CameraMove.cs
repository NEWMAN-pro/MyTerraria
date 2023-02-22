using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    // ���
    public new Camera camera;
    // ����ͷ���ɫ�ľ���
    public float distance = 10;
    public float scrollspeed = 1;//������Ұ�ٶ�
    public float rotateSpeed = 2;//�������ת���ٶȡ�
    // ����ͷ���ɫ�Ĺ̶��Ƕ�
    public float fixedAngle = 30;
    // ����ͷ���ɫ�ĵ�ǰ�Ƕ�
    public float nowAngle = 30;

    // ��ɫ
    public Transform player;

    private void Start()
    {
        //camera = GameObject.Find("Play Camera");
        rotateSpeed = player.GetComponent<PlayController>().rotateSpeed;
        camera.transform.forward = player.transform.forward;
        //ThirdPerson();
    }

    void Update()
    {

    }

    // ����������ƶ�
    public void BaseCameraMove()
    {
        Vector3 originalPos = camera.transform.position;
        Quaternion originalRotation = camera.transform.rotation;
        camera.transform.RotateAround(player.position, camera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));//Ӱ���������������position��rotation��

        // �����������ֱ�����ľ��룻
        float xx = camera.transform.eulerAngles.x;
        float zz = camera.transform.eulerAngles.z;
        if (zz != 0f)
        {
            camera.transform.eulerAngles = new Vector3(xx, camera.transform.eulerAngles.y, 0);
        }
        if (xx > 85 && xx < 275)    //��������Χ֮�����ǽ����Թ�λԭ���ģ���������ת��Ч��
        {
            camera.transform.position = originalPos;
            camera.transform.rotation = originalRotation;
        }
        nowAngle = camera.transform.eulerAngles.x;
    }

    // �����˳�
    public void ThirdPerson()
    {
        BaseCameraMove();

        distance += Input.GetAxis("Mouse ScrollWheel") * scrollspeed;   //������Ұ
        float cos = Mathf.Cos(Mathf.Deg2Rad * nowAngle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * nowAngle);
        float x = player.position.x - distance * player.forward.x * cos;
        float y = player.position.y + distance * sin;
        float z = player.position.z - distance * player.forward.z * cos;
        camera.transform.position = new Vector3(x, y, z);

        camera.transform.LookAt(player.position);
    }

    // ��һ�˳�����ͷ�ƶ�
    public void FirstPerson()
    {
        BaseCameraMove();

        // ������ת
        camera.transform.RotateAround(player.position, player.transform.up, rotateSpeed * Input.GetAxis("Mouse X"));//Ӱ���������������position��rotation��
        camera.transform.position = player.position + new Vector3(0, 0.5f, 0);
    }
}
