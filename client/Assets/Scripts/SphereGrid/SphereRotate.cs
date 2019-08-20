using UnityEngine;
using System.Collections;

public class SphereRotate : MonoBehaviour
{
    public bool onDrag = false;  //是否被拖拽//    
    public float speed = 10f;   //旋转速度//    
    private float currentSpeed;   //阻尼速度// 
    private float axisX = 1;
    //鼠标沿水平方向移动的增量//   
    private float axisY = 1;    //鼠标沿竖直方向移动的增量//   
 
    private float currentVelocity = 0;
    private float smoothTime = 1;

    public Transform target;

    private Vector3 lastMousePosition;
   

    void Start()
    {
        if (target == null)
        {
            target = transform;
        }
    }
 
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            onDrag = true;
            lastMousePosition = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(1))
        {
            onDrag = false;
        }

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
    
            Vector3 mousePosition = Input.mousePosition;

            axisX = (lastMousePosition.x - mousePosition.x) / Screen.width;
            axisY = (mousePosition.y - lastMousePosition.y) / Screen.height;

            lastMousePosition = mousePosition;
        }
        if (currentSpeed != 0)
        {
            if (target == transform)
            {
                target.Rotate(new Vector3(axisY, axisX, 0).normalized * currentSpeed, Space.World);
            }
            else
            {
                target.Rotate(new Vector3(-axisY, -axisX, 0).normalized * currentSpeed, Space.Self);
            }
        }
    }
}
