using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectEntityFollow : EffectEntity
{
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if(gameObject!=null)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
        }

        if (target == null)
        {
            return;
        }
       
        var follow = param as EntityParamEffectFollow ;
        Vector3 from = position;
        Vector3 to = Vector3.zero;
        if (target == null)
        {
#if UNITY_EDITOR
            //测试场景使用
            //if (BattleTest.TEST)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    Plane plane = new Plane(Vector3.up, Vector3.zero);
            //    float distance = 0;

            //    plane.Raycast(ray, out distance);

            //    to = ray.GetPoint(distance);
            //}
            //else
            //{
            //    OnEnd();
            //    return;
            //}
#else
            OnEnd();
            return;
#endif
        }
        else
        {
            to = target.position;
            to += target.rotation * Vector3.right * target. param.hitPosition.x;
            to += target.rotation * Vector3.forward * target.param.hitPosition.z;
            to += target.rotation * Vector3.up * target.param.hitPosition.y;
        }
       

        Vector3 direction = (to - from);
        //这一帧需要的位移
        float displacement = follow.speed * Time.deltaTime;
        if (direction.magnitude < displacement)
        {
            position = to;

            OnTrigger();
            OnEnd();
        }
        else
        {
            rotation = Quaternion.LookRotation(direction);
            position += direction.normalized * displacement;
        }


    }

   
}

