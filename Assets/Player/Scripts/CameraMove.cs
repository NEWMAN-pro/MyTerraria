using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    // ���
    public new GameObject camera;
    //λ��ƫ�ƣ���������ˣ�
    private Vector3 offsetPosition;
    // ����λ��ƫ��
    private Vector3 initialOffsetPosition = new Vector3(0, 6, -10);
    // ����ͷ���ɫ�ľ���
    public float distance = 10;
    public float scrollspeed = 1;//������Ұ�ٶ�
    public float rotateSpeed = 2;//�������ת���ٶȡ�
    // ����ͷ���ɫ�Ĺ̶��Ƕ�
    public float fixedAngle = 30;
    // ����ͷ���ɫ�ĵ�ǰ�Ƕ�
    public float nowAngle = 30;

    // ����ͷ��֡λ��
    Vector3 oldPositon;

    bool isRotating;
    // ��ɫ
    public Transform player;

    private void Start()
    {
        camera = GameObject.Find("Main Camera");
        ActiveCameraMove();
    }

    void Update()
    {

    }

    // ֹͣʱ����ͷת��
    public void StopCameraMove()
    {
        // ������Ұ����ת
        RotateView();
        // ������Ұ����������Զ��Ч��
        //ScrollView();
    }

    // �ƶ�ʱ����ͷ�����ƶ�
    public void ActiveCameraMove()
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
        if (xx < 1 || xx > 85)//��������Χ֮�����ǽ����Թ�λԭ���ģ���������ת��Ч��
        {
            camera.transform.position = originalPos;
            camera.transform.rotation = originalRotation;
        }
        nowAngle = camera.transform.eulerAngles.x;

        float cos = Mathf.Cos(Mathf.Deg2Rad * nowAngle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * nowAngle);
        float x = player.position.x - distance * player.forward.x * cos;
        float y = player.position.y + distance * sin;
        float z = player.position.z - distance * player.forward.z * cos;
        camera.transform.position = new Vector3(x, y, z);

        
        //float dis = Vector3.Distance(camera.transform.position, player.position);
        //print("���������֮��ľ����ǣ�" + dis);
        camera.transform.LookAt(player.position);
    }

    //private void ScrollView()
    //{
    //    print(Input.GetAxis("Mouse ScrollWheel"));//��� ���ظ�ֵ��������Ұ������ǰ���� ������ֵ����Զ��Ұ��
    //    distance = offsetPosition.magnitude;//��þ��롣
    //    distance += Input.GetAxis("Mouse ScrollWheel") * scrollspeed;//������Ұ
    //    offsetPosition = offsetPosition.normalized * distance;//���򲻱䣬�����ȱ�Ϊdistance
    //}

    void RotateView()
    {

        // ���һ���ʱ��ֵ�����󻬶��Ǹ�ֵ��
        camera.transform.RotateAround(player.position, player.up, rotateSpeed * Input.GetAxis("Mouse X"));

        Vector3 originalPos = camera.transform.position;
        Quaternion originalRotation = camera.transform.rotation;


        camera.transform.RotateAround(player.position, camera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));//Ӱ���������������position��rotation��

        // �����������ֱ�����ľ��룻
        float x = camera.transform.eulerAngles.x;
        float z = camera.transform.eulerAngles.z;
        if(z != 0f)
        {
            camera.transform.eulerAngles = new Vector3(x, camera.transform.eulerAngles.y, 0);
        }
        if (x < 1 || x > 85)//��������Χ֮�����ǽ����Թ�λԭ���ģ���������ת��Ч��
        {
            camera.transform.position = originalPos;
            camera.transform.rotation = originalRotation;
        }
        nowAngle = camera.transform.eulerAngles.x;

        //ÿ�θ���һ�¡�
        offsetPosition = camera.transform.position - player.position;
    }
}
