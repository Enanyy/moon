using UnityEngine;
using  PBMessage;
public class MSG_BattleEntityRunNotify : Message<BattleEntityRunNotify>
{
    public MSG_BattleEntityRunNotify() : base(MessageID.BATTLE_ENTITY_RUN_NOTIFY)
    {

    }

    protected override void OnMessage()
    {
        Vector3 velocity = new Vector3(message.velocity.x, 0, message.velocity.y);
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            entity.UpdateEntity(message.data);
            EntityAction action = entity.GetFirst(ActionType.Run);

            if (action != null)
            {
                //Vector3 position = new Vector3(ret.data.position.x, 0, ret.data.position.y);
                //Vector3 direction = position - entity.position;
                //float angle = Vector3.Angle(velocity, direction);
                //if (angle > 10)
                //{
                //    action.destination = position;
                //    action.sync = true;
                //    action.doneWhenSync = false;
                //}

                action.velocity = velocity;
            }
            else
            {
                action = ObjectPool.GetInstance<EntityAction>();
                action.velocity = velocity;
                entity.PlayAction(ActionType.Run, action);
            }
        }
    }
}
