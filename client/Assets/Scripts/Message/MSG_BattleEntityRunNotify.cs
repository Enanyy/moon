using UnityEngine;
using PBMessage;
public class MSG_BattleEntityRunNotify : Message<BattleEntityRunNotify>
{
    public MSG_BattleEntityRunNotify() : base(MessageID.BATTLE_ENTITY_RUN_NOTIFY)
    {

    }
    public static MSG_BattleEntityRunNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityRunNotify>(MessageID.BATTLE_ENTITY_RUN_NOTIFY);
    }
    protected override void OnMessage()
    {
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            Vector3 pos = new Vector3(message.data.position.x, 0, message.data.position.y);
            Vector3 velocity = new Vector3(message.velocity.x, 0, message.velocity.y);

            entity.UpdateEntity(message.data);

           
            EntityAction action = entity.GetFirst(ActionType.Run);

            if (action != null)
            {
                action.AddPathPoint(pos, velocity, false);               
            }
            else
            {
                action = ObjectPool.GetInstance<EntityAction>();
                action.AddPathPoint(pos, velocity, false);

                entity.PlayAction(ActionType.Run, action);
            }
        }
    }
}
