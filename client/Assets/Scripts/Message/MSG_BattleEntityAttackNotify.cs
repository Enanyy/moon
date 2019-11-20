using UnityEngine;
using PBMessage;
public class MSG_BattleEntityAttackNotify : Message<BattleEntityAttackNotify>
{
    public MSG_BattleEntityAttackNotify() : base(MessageID.BATTLE_ENTITY_ATTACK_NOTIFY)
    {

    }
    public static MSG_BattleEntityAttackNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleEntityAttackNotify>(MessageID.BATTLE_ENTITY_ATTACK_NOTIFY);
    }
    protected override void OnRecv(Connection connection)
    {
       
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            Vector3 pos = new Vector3(message.position.x, 0, message.position.y);

            
            if (entity.machine != null && entity.machine.current != null &&
                entity.machine.current.type == (int)ActionType.Run)
            {
                var run = entity.machine.current as EntityAction;
                if (run != null)
                {
                    run.AddPathPoint(pos, Vector3.zero, true);
                }
            }
            

            EntityAction action = ObjectPool.GetInstance<EntityAction>();
            action.skillid = message.skill;
            action.duration = message.duration;
            action.target = message.target;
            action.speed = message.speed;


            entity.PlayAction(ActionType.Attack, action);
        }
    }
}
