using UnityEngine;
using PBMessage;
public class MSG_BattleEntityAttackNotify : Message<BattleEntityAttackNotify>
{
    public MSG_BattleEntityAttackNotify() : base(MessageID.BATTLE_ENTITY_ATTACK_NOTIFY)
    {

    }

    protected override void OnMessage()
    {
        var entity = BattleManager.Instance.GetEntity(message.id);
        if (entity != null)
        {
            if (entity.machine != null && entity.machine.current != null &&
                entity.machine.current.type == (int)ActionType.Run)
            {
                var run = entity.machine.current as EntityAction;
                if (run != null)
                {
                    run.destination = new Vector3(message.data.position.x, 0, message.data.position.y); ;
                    run.doneWhenSync = true;
                    run.sync = true;
                    run.Done();
                }
            }
            entity.UpdateEntity(message.data);

            EntityAction action = ObjectPool.GetInstance<EntityAction>();
            action.skillid = message.skill;
            action.duration = message.attackspeed;
            action.target = message.target;


            entity.PlayAction(ActionType.Attack, action);
        }
    }
}
