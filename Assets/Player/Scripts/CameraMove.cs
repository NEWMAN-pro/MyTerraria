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
    public Vector3 nowAngle = new Vector3(30, 0, 0);
    // ��������
    int maskFirst;
    int maskThird;

    // ��ɫ
    public Transform player;

    private void Start()
    {
        camera.transform.forward = player.transform.forward;
        maskFirst = 1 << LayerMask.NameToLayer("First");
        maskThird = 1 << LayerMask.NameToLayer("Third");
    }

    // ����������ƶ�����ֱ��ת
    public void BaseCameraMove(Transform trans)
    {
        Vector3 originalPos = trans.position;
        Quaternion originalRotation = trans.rotation;
        trans.RotateAround(player.position, trans.right, -rotateSpeed * Input.GetAxis("Mouse Y"));//Ӱ���������������position��rotation��

        // �����������ֱ�����ľ��룻
        float xx = trans.eulerAngles.x;
        float zz = trans.eulerAngles.z;
        if (zz != 0f)
        {
            trans.eulerAngles = new Vector3(xx, trans.eulerAngles.y, 0);
        }
        if (xx > 85 && xx < 275)    //��������Χ֮�����ǽ����Թ�λԭ���ģ���������ת��Ч��
        {
            trans.position = originalPos;
            trans.rotation = originalRotation;
        }
        nowAngle = trans.eulerAngles;
    }

    // �����˳�
    public void ThirdPerson()
    {
        BaseCameraMove(player.transform);

        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollspeed;   //������Ұ
        if(Mathf.Abs(distance) < 1 || Mathf.Abs(distance) > 10)
        {
            // ���������Χ����ع�
            distance += Input.GetAxis("Mouse ScrollWheel") * scrollspeed;
        }

        // �������������
        float dis = distance / (Mathf.Pow(player.forward.x, 2f) + Mathf.Pow(player.forward.y, 2f) + Mathf.Pow(player.forward.z, 2f));
        float x = -player.forward.x * dis + player.position.x;
        float y = -player.forward.y * dis + player.position.y;
        float z = -player.forward.z * dis + player.position.z;
        camera.transform.position = new Vector3(x, y, z);

        camera.transform.LookAt(player.position);

    }

    // ��һ�˳�����ͷ�ƶ�
    public void FirstPerson()
    {
        BaseCameraMove(player.transform);

        // ������ת
        // ���������ͷ����ת
        camera.transform.rotation = player.rotation;
        camera.transform.position = player.position + new Vector3(0, -0.1f, 0);
    }

    // ��������
    public void SetMash()
    {
        camera.cullingMask ^= maskFirst;
        camera.cullingMask ^= maskThird;
    }
}
