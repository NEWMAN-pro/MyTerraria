using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    private float angle = 0.0f; // 当前摆动角度
    public bool direction = true; // 当前摆动方向（true为正方向，false为反方向）

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // trans物体绕过点point的axis摆动，速度为speed, 最大角度为range
    public void Move(float speed, float min, float max, Transform trans, Vector3 point, Vector3 axis)
    {
        // 根据摆动速度和方向更新摆动角度
        angle += (direction ? 1 : -1) * speed * Time.deltaTime;

        // 判断是否超出摆动范围，超出则改变方向
        if(angle < min || angle > max)
        {
            direction = !direction;
            angle = (angle < min) ? min : max;
            //angle = direction ? -range : range;
        }

        trans.RotateAround(point, axis, (direction ? 1 : -1) * speed * Time.deltaTime);
    }

    // 复位 
    public void Recovery(Transform trans, Vector3 angle_)
    {
        trans.localRotation = Quaternion.Euler(angle_.x, angle_.y, angle_.z);
        angle = 0.0f;
    }
}
