using UnityEngine;

public  class EffectEntityMove:EffectEntity
{
    private Vector3 mFrom;
    private Vector3 mTo;
    private float mTime;
    private float mDuration;

    protected override void OnInit(IGameObject on)
    {
        base.OnInit(on);
        var move = param as EntityParamEffectMove;
   
        if (on!=null)
        {
            Vector3 direction = (rotation*Vector3.right) * move.direction.x
                              + (rotation*Vector3.forward) * move.direction.z
                              + (rotation*Vector3.up) * move.direction.y;

            //归一化
            direction.Normalize();

            mFrom = position;
            mTo = mFrom + direction * move.distance;

            mDuration = move.distance / move.speed;
            mTime = mDuration;

            rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            OnEnd();
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        
        base.OnUpdate(deltaTime);

        if (gameObject != null)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
        }

        if (mTime > 0)
        {
            mTime -= deltaTime;
            if (mTime < 0) mTime = 0;
            if (mDuration != 0)
            {
                float factor = 1 - mTime / mDuration;
                position = mFrom * (1 - factor) + mTo * factor;
            }
            if (mTime == 0)
            {
                OnTrigger();
                OnEnd();
            }
        }
    }

   
}

