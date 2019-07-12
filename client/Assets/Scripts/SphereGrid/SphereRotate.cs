using UnityEngine;
using System.Collections;

public class SphereRotate : MonoBehaviour
{
    private bool onDrag = false;  //是否被拖拽//    
    public float speed = 6f;   //旋转速度//    
    private float currentSpeed;   //阻尼速度// 
    private float axisX = 1;
    //鼠标沿水平方向移动的增量//   
    private float axisY = 1;    //鼠标沿竖直方向移动的增量//   
 
    private float currentVelocity = 0;
    private float smoothTime = 1;

    public Transform target;

   

    void Start()
    {
        if (target == null)
        {
            target = transform;
        }
    }
    void OnMouseDown()
    {
        //接受鼠标按下的事件// 
        axisX = 0f; axisY = 0f;
    }
    void OnMouseDrag()     //鼠标拖拽时的操作// 
    {
        onDrag = true;
        axisX = -Input.GetAxis("Mouse X");
        //获得鼠标增量// 
        axisY = Input.GetAxis("Mouse Y");
    }

    void OnMouseUp()
    {
        onDrag = false;
        currentSpeed = speed;
    }

    void Update()
    {
        if (onDrag == false)
        {
            if (currentSpeed > 0.01f)
            {
                currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref currentVelocity, smoothTime);
            }
            else
            {
                currentSpeed = 0;
            }
            if (currentSpeed < 0) currentSpeed = 0;
        }
        else
        {
            currentSpeed = speed;
        }
        if (currentSpeed != 0)
        {
            if (target == transform)
            {
                target.Rotate(new Vector3(axisY, axisX, 0) * currentSpeed, Space.World);
            }
            else
            {
                Debug.Log(-axisY+"," +(-axisX).ToString());
                target.Rotate(new Vector3(-axisY, -axisX, 0) * currentSpeed, Space.Self);
            }
        }
    }
}
