using UnityEngine;
using PBMessage;
[MessageHandler]
public class MSG_BattleBeginNotify : Message<BattleBeginNotify>
{
    public MSG_BattleBeginNotify() : base(MessageID.BATTLE_BEGIN_NOTIFY)
    {

    }

    public static MSG_BattleBeginNotify Get()
    {
        return MessageManager.Instance.Get<MSG_BattleBeginNotify>(MessageID.BATTLE_BEGIN_NOTIFY);
    }

    protected override void OnRecv(Connection connection)
    {
        for (int i = 0; i < message.list.Count; ++i)
        {
            var data = message.list[i];
            BattleEntity entity = ObjectPool.GetInstance<BattleEntity>();

            entity.id = data.id;
            var table = DTHero.Get((int)data.config);
            entity.config = table.config;
            entity.campflag = data.camp;

            entity.type = data.type;

            entity.UpdateProperty(data.properties);

            entity.position = new Vector3(data.position.x, 0, data.position.y);
            entity.rotation = Quaternion.LookRotation(new Vector3(data.direction.x, 0, data.direction.y));
            if (BattleManager.Instance.AddEntity(entity))
            {
                entity.active = true;
            }
            else
            {
                ObjectPool.ReturnInstance(entity);
            }
        }
    }
}
