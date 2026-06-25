using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;

namespace GameData.Domains.Combat
{
	// Token: 0x02000FCC RID: 4044
	public static class CombatDomainMethod
	{
		// Token: 0x02002613 RID: 9747
		public static class Call
		{
			// Token: 0x06011582 RID: 71042 RVA: 0x0067E47F File Offset: 0x0067C67F
			public static void PlayMoveStepSound(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 0, isAlly);
			}

			// Token: 0x06011583 RID: 71043 RVA: 0x0067E48C File Offset: 0x0067C68C
			public static void ExecuteTeammateCommand(int listenerId, bool isAlly, int index, int charId)
			{
				GameDataBridge.AddMethodCall<bool, int, int>(listenerId, 8, 1, isAlly, index, charId);
			}

			// Token: 0x06011584 RID: 71044 RVA: 0x0067E49B File Offset: 0x0067C69B
			public static void GetCombatCharDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 2, charId);
			}

			// Token: 0x06011585 RID: 71045 RVA: 0x0067E4A8 File Offset: 0x0067C6A8
			public static void SelectMercyOption(bool isAlly, bool mercy)
			{
				GameDataBridge.AddMethodCall<bool, bool>(-1, 8, 3, isAlly, mercy);
			}

			// Token: 0x06011586 RID: 71046 RVA: 0x0067E4B6 File Offset: 0x0067C6B6
			public static void ChangeWeapon(int weaponIndex)
			{
				GameDataBridge.AddMethodCall<int>(-1, 8, 4, weaponIndex);
			}

			// Token: 0x06011587 RID: 71047 RVA: 0x0067E4C3 File Offset: 0x0067C6C3
			public static void ChangeWeapon(int weaponIndex, bool isAlly)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 8, 4, weaponIndex, isAlly);
			}

			// Token: 0x06011588 RID: 71048 RVA: 0x0067E4D1 File Offset: 0x0067C6D1
			public static void ChangeWeapon(int weaponIndex, bool isAlly, bool forceChange)
			{
				GameDataBridge.AddMethodCall<int, bool, bool>(-1, 8, 4, weaponIndex, isAlly, forceChange);
			}

			// Token: 0x06011589 RID: 71049 RVA: 0x0067E4E0 File Offset: 0x0067C6E0
			public static void NormalAttack()
			{
				GameDataBridge.AddMethodCall(-1, 8, 5);
			}

			// Token: 0x0601158A RID: 71050 RVA: 0x0067E4EC File Offset: 0x0067C6EC
			public static void NormalAttack(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 5, isAlly);
			}

			// Token: 0x0601158B RID: 71051 RVA: 0x0067E4F9 File Offset: 0x0067C6F9
			public static void StartChangeTrick()
			{
				GameDataBridge.AddMethodCall(-1, 8, 6);
			}

			// Token: 0x0601158C RID: 71052 RVA: 0x0067E505 File Offset: 0x0067C705
			public static void StartChangeTrick(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 6, isAlly);
			}

			// Token: 0x0601158D RID: 71053 RVA: 0x0067E512 File Offset: 0x0067C712
			public static void SelectChangeTrick(sbyte trickType, sbyte bodyPart, int flawOrAcupointType)
			{
				GameDataBridge.AddMethodCall<sbyte, sbyte, int>(-1, 8, 7, trickType, bodyPart, flawOrAcupointType);
			}

			// Token: 0x0601158E RID: 71054 RVA: 0x0067E521 File Offset: 0x0067C721
			public static void ChangeTaiwuWeaponInnerRatio(int index, sbyte expectInnerRatio)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(-1, 8, 8, index, expectInnerRatio);
			}

			// Token: 0x0601158F RID: 71055 RVA: 0x0067E52F File Offset: 0x0067C72F
			public static void GetWeaponInnerRatio(int listenerId, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 8, 9, weaponKey);
			}

			// Token: 0x06011590 RID: 71056 RVA: 0x0067E53D File Offset: 0x0067C73D
			public static void GetWeaponEffects(int listenerId, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 8, 10, weaponKey);
			}

			// Token: 0x06011591 RID: 71057 RVA: 0x0067E54B File Offset: 0x0067C74B
			public static void StartPrepareOtherAction(sbyte actionType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 8, 11, actionType);
			}

			// Token: 0x06011592 RID: 71058 RVA: 0x0067E559 File Offset: 0x0067C759
			public static void StartPrepareOtherAction(sbyte actionType, bool isAlly)
			{
				GameDataBridge.AddMethodCall<sbyte, bool>(-1, 8, 11, actionType, isAlly);
			}

			// Token: 0x06011593 RID: 71059 RVA: 0x0067E568 File Offset: 0x0067C768
			public static void GetProactiveSkillList(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 12, charId);
			}

			// Token: 0x06011594 RID: 71060 RVA: 0x0067E576 File Offset: 0x0067C776
			public static void StartPrepareSkill(short skillId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 8, 13, skillId);
			}

			// Token: 0x06011595 RID: 71061 RVA: 0x0067E584 File Offset: 0x0067C784
			public static void StartPrepareSkill(short skillId, bool isAlly)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 8, 13, skillId, isAlly);
			}

			// Token: 0x06011596 RID: 71062 RVA: 0x0067E593 File Offset: 0x0067C793
			public static void GmCmd_ForceRecoverBreathAndStance()
			{
				GameDataBridge.AddMethodCall(-1, 8, 14);
			}

			// Token: 0x06011597 RID: 71063 RVA: 0x0067E5A0 File Offset: 0x0067C7A0
			public static void GmCmd_AddTrick(bool isAlly, sbyte trickType)
			{
				GameDataBridge.AddMethodCall<bool, sbyte>(-1, 8, 15, isAlly, trickType);
			}

			// Token: 0x06011598 RID: 71064 RVA: 0x0067E5AF File Offset: 0x0067C7AF
			public static void GmCmd_AddInjury(bool isAlly, sbyte bodyPart, bool isInner)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, bool>(-1, 8, 16, isAlly, bodyPart, isInner);
			}

			// Token: 0x06011599 RID: 71065 RVA: 0x0067E5BF File Offset: 0x0067C7BF
			public static void GmCmd_AddInjury(bool isAlly, sbyte bodyPart, bool isInner, int count)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, bool, int>(-1, 8, 16, isAlly, bodyPart, isInner, count);
			}

			// Token: 0x0601159A RID: 71066 RVA: 0x0067E5D0 File Offset: 0x0067C7D0
			public static void GmCmd_AddInjury(bool isAlly, sbyte bodyPart, bool isInner, int count, bool changeToOld)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, bool, int, bool>(-1, 8, 16, isAlly, bodyPart, isInner, count, changeToOld);
			}

			// Token: 0x0601159B RID: 71067 RVA: 0x0067E5E3 File Offset: 0x0067C7E3
			public static void GmCmd_ForceHealAllInjury()
			{
				GameDataBridge.AddMethodCall(-1, 8, 17);
			}

			// Token: 0x0601159C RID: 71068 RVA: 0x0067E5F0 File Offset: 0x0067C7F0
			public static void GmCmd_ForceHealAllInjury(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 17, isAlly);
			}

			// Token: 0x0601159D RID: 71069 RVA: 0x0067E5FE File Offset: 0x0067C7FE
			public static void GmCmd_AddPoison(bool isAlly, sbyte poisonType)
			{
				GameDataBridge.AddMethodCall<bool, sbyte>(-1, 8, 18, isAlly, poisonType);
			}

			// Token: 0x0601159E RID: 71070 RVA: 0x0067E60D File Offset: 0x0067C80D
			public static void GmCmd_AddPoison(bool isAlly, sbyte poisonType, int count)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, int>(-1, 8, 18, isAlly, poisonType, count);
			}

			// Token: 0x0601159F RID: 71071 RVA: 0x0067E61D File Offset: 0x0067C81D
			public static void GmCmd_AddPoison(bool isAlly, sbyte poisonType, int count, bool changeToOld)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, int, bool>(-1, 8, 18, isAlly, poisonType, count, changeToOld);
			}

			// Token: 0x060115A0 RID: 71072 RVA: 0x0067E62E File Offset: 0x0067C82E
			public static void GmCmd_ForceHealAllPoison()
			{
				GameDataBridge.AddMethodCall(-1, 8, 19);
			}

			// Token: 0x060115A1 RID: 71073 RVA: 0x0067E63B File Offset: 0x0067C83B
			public static void GmCmd_ForceHealAllPoison(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 19, isAlly);
			}

			// Token: 0x060115A2 RID: 71074 RVA: 0x0067E649 File Offset: 0x0067C849
			public static void GmCmd_ForceEnemyUseSkill(short skillId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 8, 20, skillId);
			}

			// Token: 0x060115A3 RID: 71075 RVA: 0x0067E657 File Offset: 0x0067C857
			public static void GmCmd_ForceEnemyUseOtherAction(sbyte actionType)
			{
				GameDataBridge.AddMethodCall<sbyte>(-1, 8, 21, actionType);
			}

			// Token: 0x060115A4 RID: 71076 RVA: 0x0067E665 File Offset: 0x0067C865
			public static void GmCmd_ForceEnemyDefeat()
			{
				GameDataBridge.AddMethodCall(-1, 8, 22);
			}

			// Token: 0x060115A5 RID: 71077 RVA: 0x0067E672 File Offset: 0x0067C872
			public static void GmCmd_ForceSelfDefeat()
			{
				GameDataBridge.AddMethodCall(-1, 8, 23);
			}

			// Token: 0x060115A6 RID: 71078 RVA: 0x0067E67F File Offset: 0x0067C87F
			public static void GmCmd_SetNeiliAllocation(bool isAlly, short[] neiliAllocation)
			{
				GameDataBridge.AddMethodCall<bool, short[]>(-1, 8, 24, isAlly, neiliAllocation);
			}

			// Token: 0x060115A7 RID: 71079 RVA: 0x0067E68E File Offset: 0x0067C88E
			public static void GmCmd_AddFlaw(bool isAlly, sbyte bodyPart)
			{
				GameDataBridge.AddMethodCall<bool, sbyte>(-1, 8, 25, isAlly, bodyPart);
			}

			// Token: 0x060115A8 RID: 71080 RVA: 0x0067E69D File Offset: 0x0067C89D
			public static void GmCmd_AddFlaw(bool isAlly, sbyte bodyPart, int count)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, int>(-1, 8, 25, isAlly, bodyPart, count);
			}

			// Token: 0x060115A9 RID: 71081 RVA: 0x0067E6AD File Offset: 0x0067C8AD
			public static void GmCmd_HealAllFlaw(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 26, isAlly);
			}

			// Token: 0x060115AA RID: 71082 RVA: 0x0067E6BB File Offset: 0x0067C8BB
			public static void GmCmd_AddAcupoint(bool isAlly, sbyte bodyPart)
			{
				GameDataBridge.AddMethodCall<bool, sbyte>(-1, 8, 27, isAlly, bodyPart);
			}

			// Token: 0x060115AB RID: 71083 RVA: 0x0067E6CA File Offset: 0x0067C8CA
			public static void GmCmd_AddAcupoint(bool isAlly, sbyte bodyPart, int count)
			{
				GameDataBridge.AddMethodCall<bool, sbyte, int>(-1, 8, 27, isAlly, bodyPart, count);
			}

			// Token: 0x060115AC RID: 71084 RVA: 0x0067E6DA File Offset: 0x0067C8DA
			public static void GmCmd_HealAllAcupoint(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 28, isAlly);
			}

			// Token: 0x060115AD RID: 71085 RVA: 0x0067E6E8 File Offset: 0x0067C8E8
			public static void GmCmd_FightBoss(short charTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 8, 29, charTemplateId);
			}

			// Token: 0x060115AE RID: 71086 RVA: 0x0067E6F6 File Offset: 0x0067C8F6
			public static void GmCmd_FightAnimal(short charTemplateId)
			{
				GameDataBridge.AddMethodCall<short>(-1, 8, 30, charTemplateId);
			}

			// Token: 0x060115AF RID: 71087 RVA: 0x0067E704 File Offset: 0x0067C904
			public static void GmCmd_EnableEnemyAi(bool on)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 31, on);
			}

			// Token: 0x060115B0 RID: 71088 RVA: 0x0067E712 File Offset: 0x0067C912
			public static void GmCmd_EnableSkillFreeCast(bool on)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 32, on);
			}

			// Token: 0x060115B1 RID: 71089 RVA: 0x0067E720 File Offset: 0x0067C920
			public static void GetHealInjuryBanReason(int listenerId, int doctorCharId, int patientCharId)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 8, 33, doctorCharId, patientCharId);
			}

			// Token: 0x060115B2 RID: 71090 RVA: 0x0067E72F File Offset: 0x0067C92F
			public static void GetHealPoisonBanReason(int listenerId, int doctorCharId, int patientCharId)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 8, 34, doctorCharId, patientCharId);
			}

			// Token: 0x060115B3 RID: 71091 RVA: 0x0067E73E File Offset: 0x0067C93E
			public static void UseItem(ItemKey itemKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(-1, 8, 35, itemKey);
			}

			// Token: 0x060115B4 RID: 71092 RVA: 0x0067E74C File Offset: 0x0067C94C
			public static void UseItem(ItemKey itemKey, sbyte useType)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte>(-1, 8, 35, itemKey, useType);
			}

			// Token: 0x060115B5 RID: 71093 RVA: 0x0067E75B File Offset: 0x0067C95B
			public static void UseItem(ItemKey itemKey, sbyte useType, bool isAlly)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte, bool>(-1, 8, 35, itemKey, useType, isAlly);
			}

			// Token: 0x060115B6 RID: 71094 RVA: 0x0067E76B File Offset: 0x0067C96B
			public static void UseItem(ItemKey itemKey, sbyte useType, bool isAlly, List<sbyte> targetBodyParts)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte, bool, List<sbyte>>(-1, 8, 35, itemKey, useType, isAlly, targetBodyParts);
			}

			// Token: 0x060115B7 RID: 71095 RVA: 0x0067E77C File Offset: 0x0067C97C
			public static void PrepareCombat(int listenerId, short combatConfigId, List<int> leftTeam, List<int> rightTeam)
			{
				GameDataBridge.AddMethodCall<short, List<int>, List<int>>(listenerId, 8, 36, combatConfigId, leftTeam, rightTeam);
			}

			// Token: 0x060115B8 RID: 71096 RVA: 0x0067E78C File Offset: 0x0067C98C
			public static void StartCombat(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 37);
			}

			// Token: 0x060115B9 RID: 71097 RVA: 0x0067E799 File Offset: 0x0067C999
			public static void SetTimeScale(float timeScale)
			{
				GameDataBridge.AddMethodCall<float>(-1, 8, 38, timeScale);
			}

			// Token: 0x060115BA RID: 71098 RVA: 0x0067E7A7 File Offset: 0x0067C9A7
			public static void SetPlayerAutoCombat(bool autoCombat)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 39, autoCombat);
			}

			// Token: 0x060115BB RID: 71099 RVA: 0x0067E7B5 File Offset: 0x0067C9B5
			public static void SetAiOptions(AiOptions aiOptions)
			{
				GameDataBridge.AddMethodCall<AiOptions>(-1, 8, 40, aiOptions);
			}

			// Token: 0x060115BC RID: 71100 RVA: 0x0067E7C3 File Offset: 0x0067C9C3
			public static void SetMoveState(MoveState state)
			{
				GameDataBridge.AddMethodCall<MoveState>(-1, 8, 41, state);
			}

			// Token: 0x060115BD RID: 71101 RVA: 0x0067E7D1 File Offset: 0x0067C9D1
			public static void SetMoveState(MoveState state, bool isAlly)
			{
				GameDataBridge.AddMethodCall<MoveState, bool>(-1, 8, 41, state, isAlly);
			}

			// Token: 0x060115BE RID: 71102 RVA: 0x0067E7E0 File Offset: 0x0067C9E0
			public static void SetMoveState(MoveState state, bool isAlly, bool setByPlayer)
			{
				GameDataBridge.AddMethodCall<MoveState, bool, bool>(-1, 8, 41, state, isAlly, setByPlayer);
			}

			// Token: 0x060115BF RID: 71103 RVA: 0x0067E7F0 File Offset: 0x0067C9F0
			public static void GetCombatResultDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 42);
			}

			// Token: 0x060115C0 RID: 71104 RVA: 0x0067E7FD File Offset: 0x0067C9FD
			public static void SelectGetItem(List<ItemKey> acceptItems, List<int> acceptCounts)
			{
				GameDataBridge.AddMethodCall<List<ItemKey>, List<int>>(-1, 8, 43, acceptItems, acceptCounts);
			}

			// Token: 0x060115C1 RID: 71105 RVA: 0x0067E80C File Offset: 0x0067CA0C
			public static void Surrender()
			{
				GameDataBridge.AddMethodCall(-1, 8, 44);
			}

			// Token: 0x060115C2 RID: 71106 RVA: 0x0067E819 File Offset: 0x0067CA19
			public static void Surrender(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 44, isAlly);
			}

			// Token: 0x060115C3 RID: 71107 RVA: 0x0067E827 File Offset: 0x0067CA27
			public static void EnterBossPuppetCombat(short puppetCharTemplateId, sbyte consummateLevel)
			{
				GameDataBridge.AddMethodCall<short, sbyte>(-1, 8, 45, puppetCharTemplateId, consummateLevel);
			}

			// Token: 0x060115C4 RID: 71108 RVA: 0x0067E836 File Offset: 0x0067CA36
			public static void EnterBossPuppetCombat(short puppetCharTemplateId, sbyte consummateLevel, bool playground)
			{
				GameDataBridge.AddMethodCall<short, sbyte, bool>(-1, 8, 45, puppetCharTemplateId, consummateLevel, playground);
			}

			// Token: 0x060115C5 RID: 71109 RVA: 0x0067E846 File Offset: 0x0067CA46
			public static void RepairItem(ItemKey toolKey, ItemKey targetKey)
			{
				GameDataBridge.AddMethodCall<ItemKey, ItemKey>(-1, 8, 46, toolKey, targetKey);
			}

			// Token: 0x060115C6 RID: 71110 RVA: 0x0067E855 File Offset: 0x0067CA55
			public static void RepairItem(ItemKey toolKey, ItemKey targetKey, bool isAlly)
			{
				GameDataBridge.AddMethodCall<ItemKey, ItemKey, bool>(-1, 8, 46, toolKey, targetKey, isAlly);
			}

			// Token: 0x060115C7 RID: 71111 RVA: 0x0067E865 File Offset: 0x0067CA65
			public static void PrepareEnemyEquipments(short combatConfigId, List<int> enemyList)
			{
				GameDataBridge.AddMethodCall<short, List<int>>(-1, 8, 47, combatConfigId, enemyList);
			}

			// Token: 0x060115C8 RID: 71112 RVA: 0x0067E874 File Offset: 0x0067CA74
			public static void EnableBulletTime(bool enable)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 48, enable);
			}

			// Token: 0x060115C9 RID: 71113 RVA: 0x0067E882 File Offset: 0x0067CA82
			public static void GmCmd_SetImmortal(bool isAlly, bool on)
			{
				GameDataBridge.AddMethodCall<bool, bool>(-1, 8, 49, isAlly, on);
			}

			// Token: 0x060115CA RID: 71114 RVA: 0x0067E891 File Offset: 0x0067CA91
			public static void CancelChangeTrick()
			{
				GameDataBridge.AddMethodCall(-1, 8, 50);
			}

			// Token: 0x060115CB RID: 71115 RVA: 0x0067E89E File Offset: 0x0067CA9E
			public static void CancelChangeTrick(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 50, isAlly);
			}

			// Token: 0x060115CC RID: 71116 RVA: 0x0067E8AC File Offset: 0x0067CAAC
			public static void ClearAllReserveAction()
			{
				GameDataBridge.AddMethodCall(-1, 8, 51);
			}

			// Token: 0x060115CD RID: 71117 RVA: 0x0067E8B9 File Offset: 0x0067CAB9
			public static void ClearAllReserveAction(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 51, isAlly);
			}

			// Token: 0x060115CE RID: 71118 RVA: 0x0067E8C7 File Offset: 0x0067CAC7
			public static void IsInCombat(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 52);
			}

			// Token: 0x060115CF RID: 71119 RVA: 0x0067E8D4 File Offset: 0x0067CAD4
			public static void GmCmd_FightTestOrgMember(short charTemplateId, int testCount)
			{
				GameDataBridge.AddMethodCall<short, int>(-1, 8, 53, charTemplateId, testCount);
			}

			// Token: 0x060115D0 RID: 71120 RVA: 0x0067E8E3 File Offset: 0x0067CAE3
			public static void GmCmd_FightRandomEnemy(short charTemplateId, sbyte combatTypeAsSbyte)
			{
				GameDataBridge.AddMethodCall<short, sbyte>(-1, 8, 54, charTemplateId, combatTypeAsSbyte);
			}

			// Token: 0x060115D1 RID: 71121 RVA: 0x0067E8F2 File Offset: 0x0067CAF2
			public static void GmCmd_ForceRecoverMobilityValue()
			{
				GameDataBridge.AddMethodCall(-1, 8, 55);
			}

			// Token: 0x060115D2 RID: 71122 RVA: 0x0067E8FF File Offset: 0x0067CAFF
			public static void GmCmd_UnitTestSetDistanceToTarget(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 56, isAlly);
			}

			// Token: 0x060115D3 RID: 71123 RVA: 0x0067E90D File Offset: 0x0067CB0D
			public static void GmCmd_UnitTestEquipSkill(int listenerId, int charId, short skillTemplateId, bool isDirect)
			{
				GameDataBridge.AddMethodCall<int, short, bool>(listenerId, 8, 57, charId, skillTemplateId, isDirect);
			}

			// Token: 0x060115D4 RID: 71124 RVA: 0x0067E91D File Offset: 0x0067CB1D
			public static void GmCmd_UnitTestPrepare()
			{
				GameDataBridge.AddMethodCall(-1, 8, 58);
			}

			// Token: 0x060115D5 RID: 71125 RVA: 0x0067E92A File Offset: 0x0067CB2A
			public static void GmCmd_UnitTestPrepare(bool testing)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 58, testing);
			}

			// Token: 0x060115D6 RID: 71126 RVA: 0x0067E938 File Offset: 0x0067CB38
			public static void GmCmd_UnitTestClearAllEquipSkill(int charId)
			{
				GameDataBridge.AddMethodCall<int>(-1, 8, 59, charId);
			}

			// Token: 0x060115D7 RID: 71127 RVA: 0x0067E946 File Offset: 0x0067CB46
			public static void GetFatalDamageStepDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 60, charId);
			}

			// Token: 0x060115D8 RID: 71128 RVA: 0x0067E954 File Offset: 0x0067CB54
			public static void GetMindDamageStepDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 61, charId);
			}

			// Token: 0x060115D9 RID: 71129 RVA: 0x0067E962 File Offset: 0x0067CB62
			public static void GetBodyPartDamageStepDisplayData(int listenerId, int charId, sbyte bodyPart)
			{
				GameDataBridge.AddMethodCall<int, sbyte>(listenerId, 8, 62, charId, bodyPart);
			}

			// Token: 0x060115DA RID: 71130 RVA: 0x0067E971 File Offset: 0x0067CB71
			public static void GetCompleteDamageStepDisplayData(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 63, charId);
			}

			// Token: 0x060115DB RID: 71131 RVA: 0x0067E97F File Offset: 0x0067CB7F
			public static void GmCmd_ForceRecoverWugCount(bool isAlly, short wugCount)
			{
				GameDataBridge.AddMethodCall<bool, short>(-1, 8, 64, isAlly, wugCount);
			}

			// Token: 0x060115DC RID: 71132 RVA: 0x0067E98E File Offset: 0x0067CB8E
			public static void GmCmd_FightCharacter(int charId, short combatConfig)
			{
				GameDataBridge.AddMethodCall<int, short>(-1, 8, 65, charId, combatConfig);
			}

			// Token: 0x060115DD RID: 71133 RVA: 0x0067E99D File Offset: 0x0067CB9D
			public static void GetChangeTrickDisplayData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 66);
			}

			// Token: 0x060115DE RID: 71134 RVA: 0x0067E9AA File Offset: 0x0067CBAA
			public static void GetChangeTrickDisplayData(int listenerId, bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 8, 66, isAlly);
			}

			// Token: 0x060115DF RID: 71135 RVA: 0x0067E9B8 File Offset: 0x0067CBB8
			public static void ClearAffectingDefenseSkillManual(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 67, isAlly);
			}

			// Token: 0x060115E0 RID: 71136 RVA: 0x0067E9C6 File Offset: 0x0067CBC6
			public static void ClearDefendInBlockAttackSkill(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 68);
			}

			// Token: 0x060115E1 RID: 71137 RVA: 0x0067E9D3 File Offset: 0x0067CBD3
			public static void ClearDefendInBlockAttackSkill(int listenerId, bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 8, 68, isAlly);
			}

			// Token: 0x060115E2 RID: 71138 RVA: 0x0067E9E1 File Offset: 0x0067CBE1
			public static void GmCmd_HealAllFatal(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 69, isAlly);
			}

			// Token: 0x060115E3 RID: 71139 RVA: 0x0067E9EF File Offset: 0x0067CBEF
			public static void GmCmd_HealAllDefeatMark(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 70, isAlly);
			}

			// Token: 0x060115E4 RID: 71140 RVA: 0x0067E9FD File Offset: 0x0067CBFD
			public static void GmCmd_AddAllDefeatMark(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 71, isAlly);
			}

			// Token: 0x060115E5 RID: 71141 RVA: 0x0067EA0B File Offset: 0x0067CC0B
			public static void GmCmd_AddAllDefeatMark(bool isAlly, int count)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 8, 71, isAlly, count);
			}

			// Token: 0x060115E6 RID: 71142 RVA: 0x0067EA1A File Offset: 0x0067CC1A
			public static void GmCmd_AddFatal(bool isAlly, int count)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 8, 72, isAlly, count);
			}

			// Token: 0x060115E7 RID: 71143 RVA: 0x0067EA29 File Offset: 0x0067CC29
			public static void GmCmd_HealAllDie(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 73, isAlly);
			}

			// Token: 0x060115E8 RID: 71144 RVA: 0x0067EA37 File Offset: 0x0067CC37
			public static void GmCmd_AddDie(bool isAlly, int count)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 8, 74, isAlly, count);
			}

			// Token: 0x060115E9 RID: 71145 RVA: 0x0067EA46 File Offset: 0x0067CC46
			public static void GmCmd_HealAllMind(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 75, isAlly);
			}

			// Token: 0x060115EA RID: 71146 RVA: 0x0067EA54 File Offset: 0x0067CC54
			public static void GmCmd_HealInjury(bool isAlly, bool isInner)
			{
				GameDataBridge.AddMethodCall<bool, bool>(-1, 8, 76, isAlly, isInner);
			}

			// Token: 0x060115EB RID: 71147 RVA: 0x0067EA63 File Offset: 0x0067CC63
			public static void GmCmd_AddMind(bool isAlly, int count)
			{
				GameDataBridge.AddMethodCall<bool, int>(-1, 8, 77, isAlly, count);
			}

			// Token: 0x060115EC RID: 71148 RVA: 0x0067EA72 File Offset: 0x0067CC72
			public static void SetTargetDistance(short targetDistance)
			{
				GameDataBridge.AddMethodCall<short>(-1, 8, 78, targetDistance);
			}

			// Token: 0x060115ED RID: 71149 RVA: 0x0067EA80 File Offset: 0x0067CC80
			public static void SetTargetDistance(short targetDistance, bool isAlly)
			{
				GameDataBridge.AddMethodCall<short, bool>(-1, 8, 78, targetDistance, isAlly);
			}

			// Token: 0x060115EE RID: 71150 RVA: 0x0067EA8F File Offset: 0x0067CC8F
			public static void ClearTargetDistance()
			{
				GameDataBridge.AddMethodCall(-1, 8, 79);
			}

			// Token: 0x060115EF RID: 71151 RVA: 0x0067EA9C File Offset: 0x0067CC9C
			public static void SetJumpThreshold(int listenerId, short combatSkillId, short jumpThreshold)
			{
				GameDataBridge.AddMethodCall<short, short>(listenerId, 8, 80, combatSkillId, jumpThreshold);
			}

			// Token: 0x060115F0 RID: 71152 RVA: 0x0067EAAB File Offset: 0x0067CCAB
			public static void GetPreviewAttackRange(int listenerId, int charId, short skillId)
			{
				GameDataBridge.AddMethodCall<int, short>(listenerId, 8, 81, charId, skillId);
			}

			// Token: 0x060115F1 RID: 71153 RVA: 0x0067EABA File Offset: 0x0067CCBA
			public static void GetPreviewAttackRange(int listenerId, int charId, short skillId, int weaponIndex)
			{
				GameDataBridge.AddMethodCall<int, short, int>(listenerId, 8, 81, charId, skillId, weaponIndex);
			}

			// Token: 0x060115F2 RID: 71154 RVA: 0x0067EACA File Offset: 0x0067CCCA
			public static void SetPuppetUnyieldingFallen(int listenerId, bool unyieldingFallen)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 8, 82, unyieldingFallen);
			}

			// Token: 0x060115F3 RID: 71155 RVA: 0x0067EAD8 File Offset: 0x0067CCD8
			public static void SetPuppetDisableAi(int listenerId, bool disableAi)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 8, 83, disableAi);
			}

			// Token: 0x060115F4 RID: 71156 RVA: 0x0067EAE6 File Offset: 0x0067CCE6
			public static void InterruptSkillManual(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 84);
			}

			// Token: 0x060115F5 RID: 71157 RVA: 0x0067EAF3 File Offset: 0x0067CCF3
			public static void InterruptSkillManual(int listenerId, bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(listenerId, 8, 84, isAlly);
			}

			// Token: 0x060115F6 RID: 71158 RVA: 0x0067EB01 File Offset: 0x0067CD01
			public static void ClearAffectingMoveSkillManual(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 85, isAlly);
			}

			// Token: 0x060115F7 RID: 71159 RVA: 0x0067EB0F File Offset: 0x0067CD0F
			public static void UnlockAttack(int index)
			{
				GameDataBridge.AddMethodCall<int>(-1, 8, 86, index);
			}

			// Token: 0x060115F8 RID: 71160 RVA: 0x0067EB1D File Offset: 0x0067CD1D
			public static void UnlockAttack(int index, bool isAlly)
			{
				GameDataBridge.AddMethodCall<int, bool>(-1, 8, 86, index, isAlly);
			}

			// Token: 0x060115F9 RID: 71161 RVA: 0x0067EB2C File Offset: 0x0067CD2C
			public static void IgnoreAllRawCreate(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 87);
			}

			// Token: 0x060115FA RID: 71162 RVA: 0x0067EB39 File Offset: 0x0067CD39
			public static void IgnoreRawCreate(int listenerId, int effectId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 88, effectId);
			}

			// Token: 0x060115FB RID: 71163 RVA: 0x0067EB47 File Offset: 0x0067CD47
			public static void DoRawCreate(int listenerId, int effectId, sbyte equipmentSlot, short newTemplateId)
			{
				GameDataBridge.AddMethodCall<int, sbyte, short>(listenerId, 8, 89, effectId, equipmentSlot, newTemplateId);
			}

			// Token: 0x060115FC RID: 71164 RVA: 0x0067EB57 File Offset: 0x0067CD57
			public static void GetAllCanRawCreateEquipmentSlots(int listenerId, int effectId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 90, effectId);
			}

			// Token: 0x060115FD RID: 71165 RVA: 0x0067EB65 File Offset: 0x0067CD65
			public static void GetUnlockSimulateResult(int listenerId, int index)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 91, index);
			}

			// Token: 0x060115FE RID: 71166 RVA: 0x0067EB73 File Offset: 0x0067CD73
			public static void GetUnlockSimulateResult(int listenerId, int index, bool isAlly)
			{
				GameDataBridge.AddMethodCall<int, bool>(listenerId, 8, 91, index, isAlly);
			}

			// Token: 0x060115FF RID: 71167 RVA: 0x0067EB82 File Offset: 0x0067CD82
			public static void GetDefeatMarksCountOutOfCombat(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 92, charId);
			}

			// Token: 0x06011600 RID: 71168 RVA: 0x0067EB90 File Offset: 0x0067CD90
			public static void ApplyCombatResultDataEffect(CombatResultDisplayData combatResultData, List<ItemDisplayData> selectedLootItem)
			{
				GameDataBridge.AddMethodCall<CombatResultDisplayData, List<ItemDisplayData>>(-1, 8, 93, combatResultData, selectedLootItem);
			}

			// Token: 0x06011601 RID: 71169 RVA: 0x0067EB9F File Offset: 0x0067CD9F
			public static void ClearReserveNormalAttack()
			{
				GameDataBridge.AddMethodCall(-1, 8, 94);
			}

			// Token: 0x06011602 RID: 71170 RVA: 0x0067EBAC File Offset: 0x0067CDAC
			public static void ClearReserveNormalAttack(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 94, isAlly);
			}

			// Token: 0x06011603 RID: 71171 RVA: 0x0067EBBA File Offset: 0x0067CDBA
			public static void ApplyVitalOnTeammate(int listenerId, int typeInt, int index)
			{
				GameDataBridge.AddMethodCall<int, int>(listenerId, 8, 95, typeInt, index);
			}

			// Token: 0x06011604 RID: 71172 RVA: 0x0067EBC9 File Offset: 0x0067CDC9
			public static void RevertVitalOnTeammate(int listenerId, int typeInt)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 96, typeInt);
			}

			// Token: 0x06011605 RID: 71173 RVA: 0x0067EBD7 File Offset: 0x0067CDD7
			public static void GmCmd_ForceRecoverTeammateCommand()
			{
				GameDataBridge.AddMethodCall(-1, 8, 97);
			}

			// Token: 0x06011606 RID: 71174 RVA: 0x0067EBE4 File Offset: 0x0067CDE4
			public static void RequestValidItemsInCombat(int listenerId, int charId)
			{
				GameDataBridge.AddMethodCall<int>(listenerId, 8, 98, charId);
			}

			// Token: 0x06011607 RID: 71175 RVA: 0x0067EBF2 File Offset: 0x0067CDF2
			public static void RequestSwordFragmentSkillIds(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 99);
			}

			// Token: 0x06011608 RID: 71176 RVA: 0x0067EBFF File Offset: 0x0067CDFF
			public static void UseSpecialItem(sbyte itemType, short templateId)
			{
				GameDataBridge.AddMethodCall<sbyte, short>(-1, 8, 100, itemType, templateId);
			}

			// Token: 0x06011609 RID: 71177 RVA: 0x0067EC0E File Offset: 0x0067CE0E
			public static void UseSpecialItem(sbyte itemType, short templateId, bool isAlly)
			{
				GameDataBridge.AddMethodCall<sbyte, short, bool>(-1, 8, 100, itemType, templateId, isAlly);
			}

			// Token: 0x0601160A RID: 71178 RVA: 0x0067EC1E File Offset: 0x0067CE1E
			public static void NormalAttackImmediate()
			{
				GameDataBridge.AddMethodCall(-1, 8, 101);
			}

			// Token: 0x0601160B RID: 71179 RVA: 0x0067EC2B File Offset: 0x0067CE2B
			public static void NormalAttackImmediate(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 101, isAlly);
			}

			// Token: 0x0601160C RID: 71180 RVA: 0x0067EC39 File Offset: 0x0067CE39
			public static void InterruptOtherActionManual()
			{
				GameDataBridge.AddMethodCall(-1, 8, 102);
			}

			// Token: 0x0601160D RID: 71181 RVA: 0x0067EC46 File Offset: 0x0067CE46
			public static void InterruptOtherActionManual(bool isAlly)
			{
				GameDataBridge.AddMethodCall<bool>(-1, 8, 102, isAlly);
			}

			// Token: 0x0601160E RID: 71182 RVA: 0x0067EC54 File Offset: 0x0067CE54
			public static void PrepareSimulate(int listenerId, List<int> leftTeam, List<int> rightTeam)
			{
				GameDataBridge.AddMethodCall<List<int>, List<int>>(listenerId, 8, 103, leftTeam, rightTeam);
			}

			// Token: 0x0601160F RID: 71183 RVA: 0x0067EC63 File Offset: 0x0067CE63
			public static void PreparePreRandomTeammateCommands(int listenerId, short combatConfigId, List<int> leftTeam, List<int> rightTeam)
			{
				GameDataBridge.AddMethodCall<short, List<int>, List<int>>(listenerId, 8, 104, combatConfigId, leftTeam, rightTeam);
			}

			// Token: 0x06011610 RID: 71184 RVA: 0x0067EC73 File Offset: 0x0067CE73
			public static void GmCmd_FightNpc(int leftCharId, int rightCharId, short combatConfig)
			{
				GameDataBridge.AddMethodCall<int, int, short>(-1, 8, 105, leftCharId, rightCharId, combatConfig);
			}

			// Token: 0x06011611 RID: 71185 RVA: 0x0067EC83 File Offset: 0x0067CE83
			public static void SetCombatQuickUseItemSlotData(List<CombatQuickUseItemSlotData> list)
			{
				GameDataBridge.AddMethodCall<List<CombatQuickUseItemSlotData>>(-1, 8, 106, list);
			}

			// Token: 0x06011612 RID: 71186 RVA: 0x0067EC91 File Offset: 0x0067CE91
			public static void GetCombatQuickUseItemSlotData(int listenerId)
			{
				GameDataBridge.AddMethodCall(listenerId, 8, 107);
			}

			// Token: 0x06011613 RID: 71187 RVA: 0x0067EC9E File Offset: 0x0067CE9E
			public static void GmCmd_FightBossInternal(short leftCharTemplateId, short rightCharTemplateId)
			{
				GameDataBridge.AddMethodCall<short, short>(-1, 8, 108, leftCharTemplateId, rightCharTemplateId);
			}

			// Token: 0x06011614 RID: 71188 RVA: 0x0067ECAD File Offset: 0x0067CEAD
			public static void ChangeTaiwuWeaponInnerRatioByWeaponKey(ItemKey itemKey, sbyte expectInnerRatio)
			{
				GameDataBridge.AddMethodCall<ItemKey, sbyte>(-1, 8, 109, itemKey, expectInnerRatio);
			}

			// Token: 0x06011615 RID: 71189 RVA: 0x0067ECBC File Offset: 0x0067CEBC
			public static void GetWeaponExpectInnerRatio(int listenerId, ItemKey weaponKey)
			{
				GameDataBridge.AddMethodCall<ItemKey>(listenerId, 8, 110, weaponKey);
			}

			// Token: 0x06011616 RID: 71190 RVA: 0x0067ECCA File Offset: 0x0067CECA
			public static void GetMarkDisplayData(int listenerId, int charId, DefeatMarkKey markKey)
			{
				GameDataBridge.AddMethodCall<int, DefeatMarkKey>(listenerId, 8, 111, charId, markKey);
			}

			// Token: 0x06011617 RID: 71191 RVA: 0x0067ECD9 File Offset: 0x0067CED9
			public static void GmCmd_FightTwelveImmortals(int index)
			{
				GameDataBridge.AddMethodCall<int>(-1, 8, 112, index);
			}
		}

		// Token: 0x02002614 RID: 9748
		public static class AsyncCall
		{
			// Token: 0x06011618 RID: 71192 RVA: 0x0067ECE7 File Offset: 0x0067CEE7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.PlayMoveStepSound instead.", true)]
			public static void PlayMoveStepSound(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011619 RID: 71193 RVA: 0x0067ECF0 File Offset: 0x0067CEF0
			public static void ExecuteTeammateCommand(IAsyncMethodRequestHandler requestHandler, bool isAlly, int index, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool, int, int>(8, 1, isAlly, index, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601161A RID: 71194 RVA: 0x0067ED20 File Offset: 0x0067CF20
			public static void GetCombatCharDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 2, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601161B RID: 71195 RVA: 0x0067ED4A File Offset: 0x0067CF4A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SelectMercyOption instead.", true)]
			public static void SelectMercyOption(IAsyncMethodRequestHandler requestHandler, bool isAlly, bool mercy, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601161C RID: 71196 RVA: 0x0067ED52 File Offset: 0x0067CF52
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ChangeWeapon instead.", true)]
			public static void ChangeWeapon(IAsyncMethodRequestHandler requestHandler, int weaponIndex, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601161D RID: 71197 RVA: 0x0067ED5A File Offset: 0x0067CF5A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ChangeWeapon instead.", true)]
			public static void ChangeWeapon(IAsyncMethodRequestHandler requestHandler, int weaponIndex, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601161E RID: 71198 RVA: 0x0067ED62 File Offset: 0x0067CF62
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ChangeWeapon instead.", true)]
			public static void ChangeWeapon(IAsyncMethodRequestHandler requestHandler, int weaponIndex, bool isAlly, bool forceChange, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601161F RID: 71199 RVA: 0x0067ED6A File Offset: 0x0067CF6A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.NormalAttack instead.", true)]
			public static void NormalAttack(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011620 RID: 71200 RVA: 0x0067ED72 File Offset: 0x0067CF72
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.NormalAttack instead.", true)]
			public static void NormalAttack(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011621 RID: 71201 RVA: 0x0067ED7A File Offset: 0x0067CF7A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.StartChangeTrick instead.", true)]
			public static void StartChangeTrick(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011622 RID: 71202 RVA: 0x0067ED82 File Offset: 0x0067CF82
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.StartChangeTrick instead.", true)]
			public static void StartChangeTrick(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011623 RID: 71203 RVA: 0x0067ED8A File Offset: 0x0067CF8A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SelectChangeTrick instead.", true)]
			public static void SelectChangeTrick(IAsyncMethodRequestHandler requestHandler, sbyte trickType, sbyte bodyPart, int flawOrAcupointType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011624 RID: 71204 RVA: 0x0067ED92 File Offset: 0x0067CF92
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ChangeTaiwuWeaponInnerRatio instead.", true)]
			public static void ChangeTaiwuWeaponInnerRatio(IAsyncMethodRequestHandler requestHandler, int index, sbyte expectInnerRatio, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011625 RID: 71205 RVA: 0x0067ED9C File Offset: 0x0067CF9C
			public static void GetWeaponInnerRatio(IAsyncMethodRequestHandler requestHandler, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(8, 9, weaponKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011626 RID: 71206 RVA: 0x0067EDC8 File Offset: 0x0067CFC8
			public static void GetWeaponEffects(IAsyncMethodRequestHandler requestHandler, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(8, 10, weaponKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011627 RID: 71207 RVA: 0x0067EDF3 File Offset: 0x0067CFF3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.StartPrepareOtherAction instead.", true)]
			public static void StartPrepareOtherAction(IAsyncMethodRequestHandler requestHandler, sbyte actionType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011628 RID: 71208 RVA: 0x0067EDFB File Offset: 0x0067CFFB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.StartPrepareOtherAction instead.", true)]
			public static void StartPrepareOtherAction(IAsyncMethodRequestHandler requestHandler, sbyte actionType, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011629 RID: 71209 RVA: 0x0067EE04 File Offset: 0x0067D004
			public static void GetProactiveSkillList(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 12, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601162A RID: 71210 RVA: 0x0067EE2F File Offset: 0x0067D02F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.StartPrepareSkill instead.", true)]
			public static void StartPrepareSkill(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601162B RID: 71211 RVA: 0x0067EE37 File Offset: 0x0067D037
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.StartPrepareSkill instead.", true)]
			public static void StartPrepareSkill(IAsyncMethodRequestHandler requestHandler, short skillId, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601162C RID: 71212 RVA: 0x0067EE3F File Offset: 0x0067D03F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceRecoverBreathAndStance instead.", true)]
			public static void GmCmd_ForceRecoverBreathAndStance(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601162D RID: 71213 RVA: 0x0067EE47 File Offset: 0x0067D047
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddTrick instead.", true)]
			public static void GmCmd_AddTrick(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte trickType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601162E RID: 71214 RVA: 0x0067EE4F File Offset: 0x0067D04F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddInjury instead.", true)]
			public static void GmCmd_AddInjury(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, bool isInner, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601162F RID: 71215 RVA: 0x0067EE57 File Offset: 0x0067D057
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddInjury instead.", true)]
			public static void GmCmd_AddInjury(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, bool isInner, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011630 RID: 71216 RVA: 0x0067EE5F File Offset: 0x0067D05F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddInjury instead.", true)]
			public static void GmCmd_AddInjury(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, bool isInner, int count, bool changeToOld, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011631 RID: 71217 RVA: 0x0067EE67 File Offset: 0x0067D067
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceHealAllInjury instead.", true)]
			public static void GmCmd_ForceHealAllInjury(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011632 RID: 71218 RVA: 0x0067EE6F File Offset: 0x0067D06F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceHealAllInjury instead.", true)]
			public static void GmCmd_ForceHealAllInjury(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011633 RID: 71219 RVA: 0x0067EE77 File Offset: 0x0067D077
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddPoison instead.", true)]
			public static void GmCmd_AddPoison(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte poisonType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011634 RID: 71220 RVA: 0x0067EE7F File Offset: 0x0067D07F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddPoison instead.", true)]
			public static void GmCmd_AddPoison(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte poisonType, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011635 RID: 71221 RVA: 0x0067EE87 File Offset: 0x0067D087
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddPoison instead.", true)]
			public static void GmCmd_AddPoison(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte poisonType, int count, bool changeToOld, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011636 RID: 71222 RVA: 0x0067EE8F File Offset: 0x0067D08F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceHealAllPoison instead.", true)]
			public static void GmCmd_ForceHealAllPoison(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011637 RID: 71223 RVA: 0x0067EE97 File Offset: 0x0067D097
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceHealAllPoison instead.", true)]
			public static void GmCmd_ForceHealAllPoison(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011638 RID: 71224 RVA: 0x0067EE9F File Offset: 0x0067D09F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceEnemyUseSkill instead.", true)]
			public static void GmCmd_ForceEnemyUseSkill(IAsyncMethodRequestHandler requestHandler, short skillId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011639 RID: 71225 RVA: 0x0067EEA7 File Offset: 0x0067D0A7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceEnemyUseOtherAction instead.", true)]
			public static void GmCmd_ForceEnemyUseOtherAction(IAsyncMethodRequestHandler requestHandler, sbyte actionType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601163A RID: 71226 RVA: 0x0067EEAF File Offset: 0x0067D0AF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceEnemyDefeat instead.", true)]
			public static void GmCmd_ForceEnemyDefeat(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601163B RID: 71227 RVA: 0x0067EEB7 File Offset: 0x0067D0B7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceSelfDefeat instead.", true)]
			public static void GmCmd_ForceSelfDefeat(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601163C RID: 71228 RVA: 0x0067EEBF File Offset: 0x0067D0BF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_SetNeiliAllocation instead.", true)]
			public static void GmCmd_SetNeiliAllocation(IAsyncMethodRequestHandler requestHandler, bool isAlly, short[] neiliAllocation, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601163D RID: 71229 RVA: 0x0067EEC7 File Offset: 0x0067D0C7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddFlaw instead.", true)]
			public static void GmCmd_AddFlaw(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601163E RID: 71230 RVA: 0x0067EECF File Offset: 0x0067D0CF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddFlaw instead.", true)]
			public static void GmCmd_AddFlaw(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601163F RID: 71231 RVA: 0x0067EED7 File Offset: 0x0067D0D7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealAllFlaw instead.", true)]
			public static void GmCmd_HealAllFlaw(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011640 RID: 71232 RVA: 0x0067EEDF File Offset: 0x0067D0DF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddAcupoint instead.", true)]
			public static void GmCmd_AddAcupoint(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011641 RID: 71233 RVA: 0x0067EEE7 File Offset: 0x0067D0E7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddAcupoint instead.", true)]
			public static void GmCmd_AddAcupoint(IAsyncMethodRequestHandler requestHandler, bool isAlly, sbyte bodyPart, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011642 RID: 71234 RVA: 0x0067EEEF File Offset: 0x0067D0EF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealAllAcupoint instead.", true)]
			public static void GmCmd_HealAllAcupoint(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011643 RID: 71235 RVA: 0x0067EEF7 File Offset: 0x0067D0F7
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightBoss instead.", true)]
			public static void GmCmd_FightBoss(IAsyncMethodRequestHandler requestHandler, short charTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011644 RID: 71236 RVA: 0x0067EEFF File Offset: 0x0067D0FF
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightAnimal instead.", true)]
			public static void GmCmd_FightAnimal(IAsyncMethodRequestHandler requestHandler, short charTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011645 RID: 71237 RVA: 0x0067EF07 File Offset: 0x0067D107
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_EnableEnemyAi instead.", true)]
			public static void GmCmd_EnableEnemyAi(IAsyncMethodRequestHandler requestHandler, bool on, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011646 RID: 71238 RVA: 0x0067EF0F File Offset: 0x0067D10F
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_EnableSkillFreeCast instead.", true)]
			public static void GmCmd_EnableSkillFreeCast(IAsyncMethodRequestHandler requestHandler, bool on, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011647 RID: 71239 RVA: 0x0067EF18 File Offset: 0x0067D118
			public static void GetHealInjuryBanReason(IAsyncMethodRequestHandler requestHandler, int doctorCharId, int patientCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(8, 33, doctorCharId, patientCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011648 RID: 71240 RVA: 0x0067EF44 File Offset: 0x0067D144
			public static void GetHealPoisonBanReason(IAsyncMethodRequestHandler requestHandler, int doctorCharId, int patientCharId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(8, 34, doctorCharId, patientCharId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011649 RID: 71241 RVA: 0x0067EF70 File Offset: 0x0067D170
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UseItem instead.", true)]
			public static void UseItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601164A RID: 71242 RVA: 0x0067EF78 File Offset: 0x0067D178
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UseItem instead.", true)]
			public static void UseItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte useType, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601164B RID: 71243 RVA: 0x0067EF80 File Offset: 0x0067D180
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UseItem instead.", true)]
			public static void UseItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte useType, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601164C RID: 71244 RVA: 0x0067EF88 File Offset: 0x0067D188
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UseItem instead.", true)]
			public static void UseItem(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte useType, bool isAlly, List<sbyte> targetBodyParts, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601164D RID: 71245 RVA: 0x0067EF90 File Offset: 0x0067D190
			public static void PrepareCombat(IAsyncMethodRequestHandler requestHandler, short combatConfigId, List<int> leftTeam, List<int> rightTeam, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, List<int>, List<int>>(8, 36, combatConfigId, leftTeam, rightTeam, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601164E RID: 71246 RVA: 0x0067EFC0 File Offset: 0x0067D1C0
			public static void StartCombat(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 37, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601164F RID: 71247 RVA: 0x0067EFEA File Offset: 0x0067D1EA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetTimeScale instead.", true)]
			public static void SetTimeScale(IAsyncMethodRequestHandler requestHandler, float timeScale, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011650 RID: 71248 RVA: 0x0067EFF2 File Offset: 0x0067D1F2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetPlayerAutoCombat instead.", true)]
			public static void SetPlayerAutoCombat(IAsyncMethodRequestHandler requestHandler, bool autoCombat, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011651 RID: 71249 RVA: 0x0067EFFA File Offset: 0x0067D1FA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetAiOptions instead.", true)]
			public static void SetAiOptions(IAsyncMethodRequestHandler requestHandler, AiOptions aiOptions, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011652 RID: 71250 RVA: 0x0067F002 File Offset: 0x0067D202
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetMoveState instead.", true)]
			public static void SetMoveState(IAsyncMethodRequestHandler requestHandler, MoveState state, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011653 RID: 71251 RVA: 0x0067F00A File Offset: 0x0067D20A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetMoveState instead.", true)]
			public static void SetMoveState(IAsyncMethodRequestHandler requestHandler, MoveState state, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011654 RID: 71252 RVA: 0x0067F012 File Offset: 0x0067D212
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetMoveState instead.", true)]
			public static void SetMoveState(IAsyncMethodRequestHandler requestHandler, MoveState state, bool isAlly, bool setByPlayer, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011655 RID: 71253 RVA: 0x0067F01C File Offset: 0x0067D21C
			public static void GetCombatResultDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 42, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011656 RID: 71254 RVA: 0x0067F046 File Offset: 0x0067D246
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SelectGetItem instead.", true)]
			public static void SelectGetItem(IAsyncMethodRequestHandler requestHandler, List<ItemKey> acceptItems, List<int> acceptCounts, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011657 RID: 71255 RVA: 0x0067F04E File Offset: 0x0067D24E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.Surrender instead.", true)]
			public static void Surrender(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011658 RID: 71256 RVA: 0x0067F056 File Offset: 0x0067D256
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.Surrender instead.", true)]
			public static void Surrender(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011659 RID: 71257 RVA: 0x0067F05E File Offset: 0x0067D25E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.EnterBossPuppetCombat instead.", true)]
			public static void EnterBossPuppetCombat(IAsyncMethodRequestHandler requestHandler, short puppetCharTemplateId, sbyte consummateLevel, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601165A RID: 71258 RVA: 0x0067F066 File Offset: 0x0067D266
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.EnterBossPuppetCombat instead.", true)]
			public static void EnterBossPuppetCombat(IAsyncMethodRequestHandler requestHandler, short puppetCharTemplateId, sbyte consummateLevel, bool playground, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601165B RID: 71259 RVA: 0x0067F06E File Offset: 0x0067D26E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.RepairItem instead.", true)]
			public static void RepairItem(IAsyncMethodRequestHandler requestHandler, ItemKey toolKey, ItemKey targetKey, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601165C RID: 71260 RVA: 0x0067F076 File Offset: 0x0067D276
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.RepairItem instead.", true)]
			public static void RepairItem(IAsyncMethodRequestHandler requestHandler, ItemKey toolKey, ItemKey targetKey, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601165D RID: 71261 RVA: 0x0067F07E File Offset: 0x0067D27E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.PrepareEnemyEquipments instead.", true)]
			public static void PrepareEnemyEquipments(IAsyncMethodRequestHandler requestHandler, short combatConfigId, List<int> enemyList, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601165E RID: 71262 RVA: 0x0067F086 File Offset: 0x0067D286
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.EnableBulletTime instead.", true)]
			public static void EnableBulletTime(IAsyncMethodRequestHandler requestHandler, bool enable, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601165F RID: 71263 RVA: 0x0067F08E File Offset: 0x0067D28E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_SetImmortal instead.", true)]
			public static void GmCmd_SetImmortal(IAsyncMethodRequestHandler requestHandler, bool isAlly, bool on, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011660 RID: 71264 RVA: 0x0067F096 File Offset: 0x0067D296
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.CancelChangeTrick instead.", true)]
			public static void CancelChangeTrick(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011661 RID: 71265 RVA: 0x0067F09E File Offset: 0x0067D29E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.CancelChangeTrick instead.", true)]
			public static void CancelChangeTrick(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011662 RID: 71266 RVA: 0x0067F0A6 File Offset: 0x0067D2A6
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearAllReserveAction instead.", true)]
			public static void ClearAllReserveAction(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011663 RID: 71267 RVA: 0x0067F0AE File Offset: 0x0067D2AE
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearAllReserveAction instead.", true)]
			public static void ClearAllReserveAction(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011664 RID: 71268 RVA: 0x0067F0B8 File Offset: 0x0067D2B8
			public static void IsInCombat(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 52, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011665 RID: 71269 RVA: 0x0067F0E2 File Offset: 0x0067D2E2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightTestOrgMember instead.", true)]
			public static void GmCmd_FightTestOrgMember(IAsyncMethodRequestHandler requestHandler, short charTemplateId, int testCount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011666 RID: 71270 RVA: 0x0067F0EA File Offset: 0x0067D2EA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightRandomEnemy instead.", true)]
			public static void GmCmd_FightRandomEnemy(IAsyncMethodRequestHandler requestHandler, short charTemplateId, sbyte combatTypeAsSbyte, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011667 RID: 71271 RVA: 0x0067F0F2 File Offset: 0x0067D2F2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceRecoverMobilityValue instead.", true)]
			public static void GmCmd_ForceRecoverMobilityValue(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011668 RID: 71272 RVA: 0x0067F0FA File Offset: 0x0067D2FA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_UnitTestSetDistanceToTarget instead.", true)]
			public static void GmCmd_UnitTestSetDistanceToTarget(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011669 RID: 71273 RVA: 0x0067F104 File Offset: 0x0067D304
			public static void GmCmd_UnitTestEquipSkill(IAsyncMethodRequestHandler requestHandler, int charId, short skillTemplateId, bool isDirect, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, bool>(8, 57, charId, skillTemplateId, isDirect, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601166A RID: 71274 RVA: 0x0067F132 File Offset: 0x0067D332
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_UnitTestPrepare instead.", true)]
			public static void GmCmd_UnitTestPrepare(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601166B RID: 71275 RVA: 0x0067F13A File Offset: 0x0067D33A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_UnitTestPrepare instead.", true)]
			public static void GmCmd_UnitTestPrepare(IAsyncMethodRequestHandler requestHandler, bool testing, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601166C RID: 71276 RVA: 0x0067F142 File Offset: 0x0067D342
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_UnitTestClearAllEquipSkill instead.", true)]
			public static void GmCmd_UnitTestClearAllEquipSkill(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601166D RID: 71277 RVA: 0x0067F14C File Offset: 0x0067D34C
			public static void GetFatalDamageStepDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 60, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601166E RID: 71278 RVA: 0x0067F178 File Offset: 0x0067D378
			public static void GetMindDamageStepDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 61, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601166F RID: 71279 RVA: 0x0067F1A4 File Offset: 0x0067D3A4
			public static void GetBodyPartDamageStepDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, sbyte bodyPart, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte>(8, 62, charId, bodyPart, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011670 RID: 71280 RVA: 0x0067F1D0 File Offset: 0x0067D3D0
			public static void GetCompleteDamageStepDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 63, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011671 RID: 71281 RVA: 0x0067F1FB File Offset: 0x0067D3FB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceRecoverWugCount instead.", true)]
			public static void GmCmd_ForceRecoverWugCount(IAsyncMethodRequestHandler requestHandler, bool isAlly, short wugCount, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011672 RID: 71282 RVA: 0x0067F203 File Offset: 0x0067D403
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightCharacter instead.", true)]
			public static void GmCmd_FightCharacter(IAsyncMethodRequestHandler requestHandler, int charId, short combatConfig, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011673 RID: 71283 RVA: 0x0067F20C File Offset: 0x0067D40C
			public static void GetChangeTrickDisplayData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 66, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011674 RID: 71284 RVA: 0x0067F238 File Offset: 0x0067D438
			public static void GetChangeTrickDisplayData(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(8, 66, isAlly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011675 RID: 71285 RVA: 0x0067F263 File Offset: 0x0067D463
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearAffectingDefenseSkillManual instead.", true)]
			public static void ClearAffectingDefenseSkillManual(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011676 RID: 71286 RVA: 0x0067F26C File Offset: 0x0067D46C
			public static void ClearDefendInBlockAttackSkill(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 68, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011677 RID: 71287 RVA: 0x0067F298 File Offset: 0x0067D498
			public static void ClearDefendInBlockAttackSkill(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(8, 68, isAlly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011678 RID: 71288 RVA: 0x0067F2C3 File Offset: 0x0067D4C3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealAllFatal instead.", true)]
			public static void GmCmd_HealAllFatal(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011679 RID: 71289 RVA: 0x0067F2CB File Offset: 0x0067D4CB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealAllDefeatMark instead.", true)]
			public static void GmCmd_HealAllDefeatMark(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601167A RID: 71290 RVA: 0x0067F2D3 File Offset: 0x0067D4D3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddAllDefeatMark instead.", true)]
			public static void GmCmd_AddAllDefeatMark(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601167B RID: 71291 RVA: 0x0067F2DB File Offset: 0x0067D4DB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddAllDefeatMark instead.", true)]
			public static void GmCmd_AddAllDefeatMark(IAsyncMethodRequestHandler requestHandler, bool isAlly, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601167C RID: 71292 RVA: 0x0067F2E3 File Offset: 0x0067D4E3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddFatal instead.", true)]
			public static void GmCmd_AddFatal(IAsyncMethodRequestHandler requestHandler, bool isAlly, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601167D RID: 71293 RVA: 0x0067F2EB File Offset: 0x0067D4EB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealAllDie instead.", true)]
			public static void GmCmd_HealAllDie(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601167E RID: 71294 RVA: 0x0067F2F3 File Offset: 0x0067D4F3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddDie instead.", true)]
			public static void GmCmd_AddDie(IAsyncMethodRequestHandler requestHandler, bool isAlly, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601167F RID: 71295 RVA: 0x0067F2FB File Offset: 0x0067D4FB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealAllMind instead.", true)]
			public static void GmCmd_HealAllMind(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011680 RID: 71296 RVA: 0x0067F303 File Offset: 0x0067D503
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_HealInjury instead.", true)]
			public static void GmCmd_HealInjury(IAsyncMethodRequestHandler requestHandler, bool isAlly, bool isInner, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011681 RID: 71297 RVA: 0x0067F30B File Offset: 0x0067D50B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_AddMind instead.", true)]
			public static void GmCmd_AddMind(IAsyncMethodRequestHandler requestHandler, bool isAlly, int count, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011682 RID: 71298 RVA: 0x0067F313 File Offset: 0x0067D513
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetTargetDistance instead.", true)]
			public static void SetTargetDistance(IAsyncMethodRequestHandler requestHandler, short targetDistance, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011683 RID: 71299 RVA: 0x0067F31B File Offset: 0x0067D51B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetTargetDistance instead.", true)]
			public static void SetTargetDistance(IAsyncMethodRequestHandler requestHandler, short targetDistance, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011684 RID: 71300 RVA: 0x0067F323 File Offset: 0x0067D523
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearTargetDistance instead.", true)]
			public static void ClearTargetDistance(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011685 RID: 71301 RVA: 0x0067F32C File Offset: 0x0067D52C
			public static void SetJumpThreshold(IAsyncMethodRequestHandler requestHandler, short combatSkillId, short jumpThreshold, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, short>(8, 80, combatSkillId, jumpThreshold, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011686 RID: 71302 RVA: 0x0067F358 File Offset: 0x0067D558
			public static void GetPreviewAttackRange(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short>(8, 81, charId, skillId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011687 RID: 71303 RVA: 0x0067F384 File Offset: 0x0067D584
			public static void GetPreviewAttackRange(IAsyncMethodRequestHandler requestHandler, int charId, short skillId, int weaponIndex, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, short, int>(8, 81, charId, skillId, weaponIndex, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011688 RID: 71304 RVA: 0x0067F3B4 File Offset: 0x0067D5B4
			public static void SetPuppetUnyieldingFallen(IAsyncMethodRequestHandler requestHandler, bool unyieldingFallen, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(8, 82, unyieldingFallen, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011689 RID: 71305 RVA: 0x0067F3E0 File Offset: 0x0067D5E0
			public static void SetPuppetDisableAi(IAsyncMethodRequestHandler requestHandler, bool disableAi, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(8, 83, disableAi, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601168A RID: 71306 RVA: 0x0067F40C File Offset: 0x0067D60C
			public static void InterruptSkillManual(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 84, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601168B RID: 71307 RVA: 0x0067F438 File Offset: 0x0067D638
			public static void InterruptSkillManual(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<bool>(8, 84, isAlly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601168C RID: 71308 RVA: 0x0067F463 File Offset: 0x0067D663
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearAffectingMoveSkillManual instead.", true)]
			public static void ClearAffectingMoveSkillManual(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601168D RID: 71309 RVA: 0x0067F46B File Offset: 0x0067D66B
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UnlockAttack instead.", true)]
			public static void UnlockAttack(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601168E RID: 71310 RVA: 0x0067F473 File Offset: 0x0067D673
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UnlockAttack instead.", true)]
			public static void UnlockAttack(IAsyncMethodRequestHandler requestHandler, int index, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601168F RID: 71311 RVA: 0x0067F47C File Offset: 0x0067D67C
			public static void IgnoreAllRawCreate(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 87, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011690 RID: 71312 RVA: 0x0067F4A8 File Offset: 0x0067D6A8
			public static void IgnoreRawCreate(IAsyncMethodRequestHandler requestHandler, int effectId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 88, effectId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011691 RID: 71313 RVA: 0x0067F4D4 File Offset: 0x0067D6D4
			public static void DoRawCreate(IAsyncMethodRequestHandler requestHandler, int effectId, sbyte equipmentSlot, short newTemplateId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, sbyte, short>(8, 89, effectId, equipmentSlot, newTemplateId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011692 RID: 71314 RVA: 0x0067F504 File Offset: 0x0067D704
			public static void GetAllCanRawCreateEquipmentSlots(IAsyncMethodRequestHandler requestHandler, int effectId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 90, effectId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011693 RID: 71315 RVA: 0x0067F530 File Offset: 0x0067D730
			public static void GetUnlockSimulateResult(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 91, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011694 RID: 71316 RVA: 0x0067F55C File Offset: 0x0067D75C
			public static void GetUnlockSimulateResult(IAsyncMethodRequestHandler requestHandler, int index, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, bool>(8, 91, index, isAlly, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011695 RID: 71317 RVA: 0x0067F588 File Offset: 0x0067D788
			public static void GetDefeatMarksCountOutOfCombat(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 92, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x06011696 RID: 71318 RVA: 0x0067F5B3 File Offset: 0x0067D7B3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ApplyCombatResultDataEffect instead.", true)]
			public static void ApplyCombatResultDataEffect(IAsyncMethodRequestHandler requestHandler, CombatResultDisplayData combatResultData, List<ItemDisplayData> selectedLootItem, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011697 RID: 71319 RVA: 0x0067F5BB File Offset: 0x0067D7BB
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearReserveNormalAttack instead.", true)]
			public static void ClearReserveNormalAttack(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011698 RID: 71320 RVA: 0x0067F5C3 File Offset: 0x0067D7C3
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ClearReserveNormalAttack instead.", true)]
			public static void ClearReserveNormalAttack(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06011699 RID: 71321 RVA: 0x0067F5CC File Offset: 0x0067D7CC
			public static void ApplyVitalOnTeammate(IAsyncMethodRequestHandler requestHandler, int typeInt, int index, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(8, 95, typeInt, index, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601169A RID: 71322 RVA: 0x0067F5F8 File Offset: 0x0067D7F8
			public static void RevertVitalOnTeammate(IAsyncMethodRequestHandler requestHandler, int typeInt, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 96, typeInt, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601169B RID: 71323 RVA: 0x0067F623 File Offset: 0x0067D823
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_ForceRecoverTeammateCommand instead.", true)]
			public static void GmCmd_ForceRecoverTeammateCommand(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601169C RID: 71324 RVA: 0x0067F62C File Offset: 0x0067D82C
			public static void RequestValidItemsInCombat(IAsyncMethodRequestHandler requestHandler, int charId, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int>(8, 98, charId, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601169D RID: 71325 RVA: 0x0067F658 File Offset: 0x0067D858
			public static void RequestSwordFragmentSkillIds(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 99, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x0601169E RID: 71326 RVA: 0x0067F682 File Offset: 0x0067D882
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UseSpecialItem instead.", true)]
			public static void UseSpecialItem(IAsyncMethodRequestHandler requestHandler, sbyte itemType, short templateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0601169F RID: 71327 RVA: 0x0067F68A File Offset: 0x0067D88A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.UseSpecialItem instead.", true)]
			public static void UseSpecialItem(IAsyncMethodRequestHandler requestHandler, sbyte itemType, short templateId, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A0 RID: 71328 RVA: 0x0067F692 File Offset: 0x0067D892
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.NormalAttackImmediate instead.", true)]
			public static void NormalAttackImmediate(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A1 RID: 71329 RVA: 0x0067F69A File Offset: 0x0067D89A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.NormalAttackImmediate instead.", true)]
			public static void NormalAttackImmediate(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A2 RID: 71330 RVA: 0x0067F6A2 File Offset: 0x0067D8A2
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.InterruptOtherActionManual instead.", true)]
			public static void InterruptOtherActionManual(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A3 RID: 71331 RVA: 0x0067F6AA File Offset: 0x0067D8AA
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.InterruptOtherActionManual instead.", true)]
			public static void InterruptOtherActionManual(IAsyncMethodRequestHandler requestHandler, bool isAlly, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A4 RID: 71332 RVA: 0x0067F6B4 File Offset: 0x0067D8B4
			public static void PrepareSimulate(IAsyncMethodRequestHandler requestHandler, List<int> leftTeam, List<int> rightTeam, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<List<int>, List<int>>(8, 103, leftTeam, rightTeam, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060116A5 RID: 71333 RVA: 0x0067F6E0 File Offset: 0x0067D8E0
			public static void PreparePreRandomTeammateCommands(IAsyncMethodRequestHandler requestHandler, short combatConfigId, List<int> leftTeam, List<int> rightTeam, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<short, List<int>, List<int>>(8, 104, combatConfigId, leftTeam, rightTeam, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060116A6 RID: 71334 RVA: 0x0067F70E File Offset: 0x0067D90E
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightNpc instead.", true)]
			public static void GmCmd_FightNpc(IAsyncMethodRequestHandler requestHandler, int leftCharId, int rightCharId, short combatConfig, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A7 RID: 71335 RVA: 0x0067F716 File Offset: 0x0067D916
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.SetCombatQuickUseItemSlotData instead.", true)]
			public static void SetCombatQuickUseItemSlotData(IAsyncMethodRequestHandler requestHandler, List<CombatQuickUseItemSlotData> list, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116A8 RID: 71336 RVA: 0x0067F720 File Offset: 0x0067D920
			public static void GetCombatQuickUseItemSlotData(IAsyncMethodRequestHandler requestHandler, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall(8, 107, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060116A9 RID: 71337 RVA: 0x0067F74A File Offset: 0x0067D94A
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightBossInternal instead.", true)]
			public static void GmCmd_FightBossInternal(IAsyncMethodRequestHandler requestHandler, short leftCharTemplateId, short rightCharTemplateId, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116AA RID: 71338 RVA: 0x0067F752 File Offset: 0x0067D952
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.ChangeTaiwuWeaponInnerRatioByWeaponKey instead.", true)]
			public static void ChangeTaiwuWeaponInnerRatioByWeaponKey(IAsyncMethodRequestHandler requestHandler, ItemKey itemKey, sbyte expectInnerRatio, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}

			// Token: 0x060116AB RID: 71339 RVA: 0x0067F75C File Offset: 0x0067D95C
			public static void GetWeaponExpectInnerRatio(IAsyncMethodRequestHandler requestHandler, ItemKey weaponKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<ItemKey>(8, 110, weaponKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060116AC RID: 71340 RVA: 0x0067F788 File Offset: 0x0067D988
			public static void GetMarkDisplayData(IAsyncMethodRequestHandler requestHandler, int charId, DefeatMarkKey markKey, AsyncMethodCallbackDelegate callback)
			{
				int callId = SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, DefeatMarkKey>(8, 111, charId, markKey, callback);
				if (requestHandler != null)
				{
					requestHandler.RegisterAsyncMethodCall(callId);
				}
			}

			// Token: 0x060116AD RID: 71341 RVA: 0x0067F7B4 File Offset: 0x0067D9B4
			[Obsolete("AsyncCall can only be used for domain methods with a return value. Use CombatDomainMethod.Call.GmCmd_FightTwelveImmortals instead.", true)]
			public static void GmCmd_FightTwelveImmortals(IAsyncMethodRequestHandler requestHandler, int index, AsyncMethodCallbackDelegate callback)
			{
				throw new NotSupportedException();
			}
		}
	}
}
