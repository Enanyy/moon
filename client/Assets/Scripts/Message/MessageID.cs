using System;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>

public enum MessageID : int
{
//MESSAGEID_BEGIN	LOGIN_REQUEST = 1000,
	LOGIN_RETURN = 1001,
	LOGIN_GAME_NOTIFY = 1002,
	LOGIN_GAME_REQUEST = 1003,
	LOGIN_GAME_RETURN = 1004,
	BATTLE_BEGIN_REQUEST = 1005,
	BATTLE_BEGIN_RETURN = 1006,
	BATTLE_BEGIN_NOTIFY = 1007,
	BATTLE_ENTITY_IDLE_NOTIFY = 1008,
	BATTLE_ENTITY_RUN_NOTIFY = 1009,
	BATTLE_ENTITY_ATTACK_NOTIFY = 1010,
	BATTLE_ENTITY_ATTACK_CHANGE_NOTIFY = 1011,
	BATTLE_ENTITY_DIE_NOTIFY = 1012,
	BATTLE_ENTITY_BLOOD_NOTIFY = 1013,
	BATTLE_ENTITY_PROPERTY_NOTIFY = 1014,
	BATTLE_END_NOTIFY = 1015,
//MESSAGEID_END
}

