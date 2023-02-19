using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    // 相机
    public new GameObject camera;
    //位置偏移（摄像机与人）
    private Vector3 offsetPosition;
    // 回正位置偏移
    private Vector3 initialOffsetPosition = new Vector3(0, 6, -10);
    // 摄像头与角色的距离
    public float distance = 10;
    public float scrollspeed = 1;//拉近视野速度
    public float rotateSpeed = 2;//摄像机旋转的速度。
    // 摄像头与角色的固定角度
    public float fixedAngle = 30;
    // 摄像头与角色的当前角度
    public float nowAngle = 30;

    // 摄像头上帧位置
    Vector3 oldPositon;

    bool isRotating;
    // 角色
    public Transform player;

    private void Start()
    {
        camera = GameObject.Find("Main Camera");
        ActiveCameraMove();
    }

    void Update()
    {

    }

    // 停止时摄像头转动
    public void StopCameraMove()
    {
        // 处理视野的旋转
        RotateView();
        // 处理视野的拉近和拉远的效果
        //ScrollView();
    }

    // 移动时摄像头跟随移动
    public void ActiveCameraMove()
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
        if (xx < 1 || xx > 85)//当超出范围之后，我们将属性归位原来的，就是让旋转无效；
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
        //print("正方体和球之间的距离是：" + dis);
        camera.transform.LookAt(player.position);
    }

    //private void ScrollView()
    //{
    //    print(Input.GetAxis("Mouse ScrollWheel"));//向后 返回负值（拉近视野），向前滑动 返回正值（拉远视野）
    //    distance = offsetPosition.magnitude;//获得距离。
    //    distance += Input.GetAxis("Mouse ScrollWheel") * scrollspeed;//拉近视野
    //    offsetPosition = offsetPosition.normalized * distance;//方向不变，将长度变为distance
    //}

    void RotateView()
    {

        // 向右滑动时正值，向左滑动是负值。
        camera.transform.RotateAround(player.position, player.up, rotateSpeed * Input.GetAxis("Mouse X"));

        Vector3 originalPos = camera.transform.position;
        Quaternion originalRotation = camera.transform.rotation;


        camera.transform.RotateAround(player.position, camera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));//影响的属性有两个，position，rotation；

        // 限制摄像机垂直滑动的距离；
        float x = camera.transform.eulerAngles.x;
        float z = camera.transform.eulerAngles.z;
        if(z != 0f)
        {
            camera.transform.eulerAngles = new Vector3(x, camera.transform.eulerAngles.y, 0);
        }
        if (x < 1 || x > 85)//当超出范围之后，我们将属性归位原来的，就是让旋转无效；
        {
            camera.transform.position = originalPos;
            camera.transform.rotation = originalRotation;
        }
        nowAngle = camera.transform.eulerAngles.x;

        //每次更新一下。
        offsetPosition = camera.transform.position - player.position;
    }
}
