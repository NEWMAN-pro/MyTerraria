using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    private float angle = 0.0f; // ��ǰ�ڶ��Ƕ�
    public bool direction = true; // ��ǰ�ڶ�����trueΪ������falseΪ������

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // trans�����ƹ���point��axis�ڶ����ٶ�Ϊspeed, ���Ƕ�Ϊrange
    public void Move(float speed, float min, float max, Transform trans, Vector3 point, Vector3 axis)
    {
        // ���ݰڶ��ٶȺͷ�����°ڶ��Ƕ�
        angle += (direction ? 1 : -1) * speed * Time.deltaTime;

        // �ж��Ƿ񳬳��ڶ���Χ��������ı䷽��
        if(angle < min || angle > max)
        {
            direction = !direction;
            angle = (angle < min) ? min : max;
            //angle = direction ? -range : range;
        }

        trans.RotateAround(point, axis, (direction ? 1 : -1) * speed * Time.deltaTime);
    }

    // ��λ 
    public void Recovery(Transform trans, Vector3 angle_)
    {
        trans.localRotation = Quaternion.Euler(angle_.x, angle_.y, angle_.z);
        angle = 0.0f;
    }
}
