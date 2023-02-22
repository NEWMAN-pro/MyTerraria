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
    public float nowAngle = 30;

    // 角色
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

    // 基础摄像机移动
    public void BaseCameraMove()
    {
        Vector3 originalPos = camera.transform.position;
        Quaternion originalRotation = camera.transform.rotation;
        camera.transform.RotateAround(player.position, camera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));//影响的属性有两个，position，rotation；

        // 限制摄像机垂直滑动的距离；
        float xx = camera.transform.eulerAngles.x;
        float zz = camera.transform.eulerAngles.z;
        if (zz != 0f)
        {
            camera.transform.eulerAngles = new Vector3(xx, camera.transform.eulerAngles.y, 0);
        }
        if (xx > 85 && xx < 275)    //当超出范围之后，我们将属性归位原来的，就是让旋转无效；
        {
            camera.transform.position = originalPos;
            camera.transform.rotation = originalRotation;
        }
        nowAngle = camera.transform.eulerAngles.x;
    }

    // 第三人称
    public void ThirdPerson()
    {
        BaseCameraMove();

        distance += Input.GetAxis("Mouse ScrollWheel") * scrollspeed;   //拉近视野
        float cos = Mathf.Cos(Mathf.Deg2Rad * nowAngle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * nowAngle);
        float x = player.position.x - distance * player.forward.x * cos;
        float y = player.position.y + distance * sin;
        float z = player.position.z - distance * player.forward.z * cos;
        camera.transform.position = new Vector3(x, y, z);

        camera.transform.LookAt(player.position);
    }

    // 第一人称摄像头移动
    public void FirstPerson()
    {
        BaseCameraMove();

        // 左右旋转
        camera.transform.RotateAround(player.position, player.transform.up, rotateSpeed * Input.GetAxis("Mouse X"));//影响的属性有两个，position，rotation；
        camera.transform.position = player.position + new Vector3(0, 0.5f, 0);
    }
}
