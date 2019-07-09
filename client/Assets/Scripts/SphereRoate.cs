using UnityEngine;
using System.Collections;

public class SphereRoate : MonoBehaviour
{
    private bool onDrag = false;  //是否被拖拽//    
    public float speed = 6f;   //旋转速度//    
    private float tempSpeed;   //阻尼速度// 
    private float axisX = 1;
    //鼠标沿水平方向移动的增量//   
    private float axisY = 1;    //鼠标沿竖直方向移动的增量//   
    private float cXY;
    void OnMouseDown()
    {
        //接受鼠标按下的事件// 
        Debug.Log("OnMouseDown");
        axisX = 0f; axisY = 0f;
    }
    void OnMouseDrag()     //鼠标拖拽时的操作// 
    {
        Debug.Log("OnMouseDrag");

        onDrag = true;
        axisX = -Input.GetAxis("Mouse X");
        //获得鼠标增量// 
        axisY = Input.GetAxis("Mouse Y");
        cXY = Mathf.Sqrt(axisX * axisX + axisY * axisY); //计算鼠标移动的长度//
        if (cXY == 0f) { cXY = 1f; }

    }
    float Rigid()      //计算阻尼速度//    
    {
        if (onDrag)
        {
            tempSpeed = speed;
        }
        else
        {
            if (tempSpeed > 0)
            {
                tempSpeed -= speed * 2 * Time.deltaTime / cXY; //通过除以鼠标移动长度实现拖拽越长速度减缓越慢// 
            }
            else
            {
                tempSpeed = 0;
            }
        }
        return tempSpeed;
    }

    void Update()
    {
        // this.transform.Rotate(new Vector3(axisY, axisX, 0) * Rigid(), Space.World); //这个是是按照之前方向一直慢速旋转
        if (Input.GetMouseButton(0))
        {
            onDrag = false;
            this.transform.Rotate(new Vector3(axisY, axisX, 0) * speed, Space.World);
        }
    }
}
