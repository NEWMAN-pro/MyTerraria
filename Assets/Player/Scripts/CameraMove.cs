using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    // 相机
    public new Camera camera;
    // 摄像头与角色的距离
    public float distance = 10;
    public float scrollspeed = 1;//拉近视野速度
    public float rotateSpeed = 2;//摄像机旋转的速度。
    // 摄像头与角色的固定角度
    public float fixedAngle = 30;
    // 摄像头与角色的当前角度
    public Vector3 nowAngle = new Vector3(30, 0, 0);
    // 设置遮罩
    int maskFirst;
    int maskThird;

    // 角色
    public Transform player;

    private void Start()
    {
        camera.transform.forward = player.transform.forward;
        maskFirst = 1 << LayerMask.NameToLayer("First");
        maskThird = 1 << LayerMask.NameToLayer("Third");
    }

    // 基础摄像机移动，垂直旋转
    public void BaseCameraMove(Transform trans)
    {
        Vector3 originalPos = trans.position;
        Quaternion originalRotation = trans.rotation;
        trans.RotateAround(player.position, trans.right, -rotateSpeed * Input.GetAxis("Mouse Y"));//影响的属性有两个，position，rotation；

        // 限制摄像机垂直滑动的距离；
        float xx = trans.eulerAngles.x;
        float zz = trans.eulerAngles.z;
        if (zz != 0f)
        {
            trans.eulerAngles = new Vector3(xx, trans.eulerAngles.y, 0);
        }
        if (xx > 85 && xx < 275)    //当超出范围之后，我们将属性归位原来的，就是让旋转无效；
        {
            trans.position = originalPos;
            trans.rotation = originalRotation;
        }
        nowAngle = trans.eulerAngles;
    }

    // 第三人称
    public void ThirdPerson()
    {
        BaseCameraMove(player.transform);

        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollspeed;   //拉近视野
        if(Mathf.Abs(distance) < 1 || Mathf.Abs(distance) > 10)
        {
            // 如果超出范围，则回滚
            distance += Input.GetAxis("Mouse ScrollWheel") * scrollspeed;
        }

        // 计算摄像机坐标
        float dis = distance / (Mathf.Pow(player.forward.x, 2f) + Mathf.Pow(player.forward.y, 2f) + Mathf.Pow(player.forward.z, 2f));
        float x = -player.forward.x * dis + player.position.x;
        float y = -player.forward.y * dis + player.position.y;
        float z = -player.forward.z * dis + player.position.z;
        camera.transform.position = new Vector3(x, y, z);

        camera.transform.LookAt(player.position);

    }

    // 第一人称摄像头移动
    public void FirstPerson()
    {
        BaseCameraMove(player.transform);

        // 左右旋转
        // 摄像机跟随头部旋转
        camera.transform.rotation = player.rotation;
        camera.transform.position = player.position + new Vector3(0, -0.1f, 0);
    }

    // 更改遮罩
    public void SetMash()
    {
        camera.cullingMask ^= maskFirst;
        camera.cullingMask ^= maskThird;
    }
}
