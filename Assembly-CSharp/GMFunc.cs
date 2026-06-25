using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CharacterDataMonitor;
using Config;
using Config.Common;
using FrameWork;
using Game.Views.Building;
using Game.Views.Building.BuildingManage;
using Game.Views.Map;
using Game.Views.SectInteract;
using GameData.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Adventure;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.LegendaryBook;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Mod;
using GameData.Domains.Organization;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GM;
using Steamworks;
using UnityEngine;

// Token: 0x02000137 RID: 311
public static class GMFunc
{
	// Token: 0x06000E38 RID: 3640 RVA: 0x000598CC File Offset: 0x00057ACC
	[GMFunc(EGMGroup.CharacterBase, 0.25f, -999, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void EditCharacterInfo(int charId)
	{
		CharacterDomainMethod.AsyncCall.GetNameAndLifeRelatedData(null, charId, delegate(int offset, RawDataPool dataPool)
		{
			NameAndLifeRelatedData data = default(NameAndLifeRelatedData);
			Serializer.Deserialize(dataPool, offset, ref data);
			bool flag = data.LifeState == 0;
			if (flag)
			{
				UI_GMWindow.Instance.CharacterEditor.GetComponent<GMCharacterEditor>().SetCharacterId(charId);
				UI_GMWindow.Instance.CharacterEditor.gameObject.SetActive(true);
			}
		});
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00059900 File Offset: 0x00057B00
	[GMFunc(EGMGroup.CharacterBase, 0.25f, -998, null, GmRunMode.Default)]
	public static void OpenCheckAvatarPanel()
	{
		UI_GMWindow.Instance.CheckAvatarPanel.Open();
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00059914 File Offset: 0x00057B14
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void EnterCharacterMenu(int charId)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", charId);
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00059954 File Offset: 0x00057B54
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.LifeSkillTypeField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public unsafe static void SetCharacterLifeSkillQualification(int charId, sbyte lifeSkillType, int modifiedQualification)
	{
		DataUid uid = new DataUid(4, 0, (ulong)((long)charId), 30U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			LifeSkillShorts qualifications = (LifeSkillShorts)data[uid];
			bool flag = lifeSkillType < 0;
			if (flag)
			{
				lifeSkillType = Config.LifeSkillType.Instance.GetAllKeys().GetRandom<sbyte>();
			}
			*(ref qualifications.Items.FixedElementField + (IntPtr)lifeSkillType * 2) = (short)modifiedQualification;
			bool flag2 = modifiedQualification > 0;
			if (flag2)
			{
				GameDataBridge.AddDataModification<LifeSkillShorts>(4, 0, (ulong)((long)charId), 30U, qualifications);
			}
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(LifeSkillShorts))
		});
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x000599CC File Offset: 0x00057BCC
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CombatSkillTypeField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public unsafe static void SetCharacterCombatSkillQualification(int charId, sbyte combatSkillType, int modifiedQualification)
	{
		DataUid uid = new DataUid(4, 0, (ulong)((long)charId), 32U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			CombatSkillShorts qualifications = (CombatSkillShorts)data[uid];
			bool flag = combatSkillType < 0;
			if (flag)
			{
				combatSkillType = Config.CombatSkillType.Instance.GetAllKeys().GetRandom<sbyte>();
			}
			*(ref qualifications.Items.FixedElementField + (IntPtr)combatSkillType * 2) = (short)modifiedQualification;
			bool flag2 = modifiedQualification > 0;
			if (flag2)
			{
				GameDataBridge.AddDataModification<CombatSkillShorts>(4, 0, (ulong)((long)charId), 32U, qualifications);
			}
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(CombatSkillShorts))
		});
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x00059A44 File Offset: 0x00057C44
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.LifeSkillIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public unsafe static void SetCharacterLifeSkillFullLearned(int charId, int templateId, int modifiedQualification)
	{
		DataUid uid = new DataUid(4, 0, (ulong)((long)charId), 29U);
		DataUid uid2 = new DataUid(4, 0, (ulong)((long)charId), 30U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			List<GameData.Domains.Character.LifeSkillItem> learned = (List<GameData.Domains.Character.LifeSkillItem>)data[uid];
			LifeSkillShorts qualifications = (LifeSkillShorts)data[uid2];
			bool flag = templateId < 0;
			if (flag)
			{
				short i = 0;
				while ((int)i < LifeSkill.Instance.Count)
				{
					GameData.Domains.Character.LifeSkillItem item = new GameData.Domains.Character.LifeSkillItem(i, 5);
					Config.LifeSkillItem config = LifeSkill.Instance.GetItem(item.SkillTemplateId);
					int findItemIndex = learned.FindIndex((GameData.Domains.Character.LifeSkillItem skill) => skill.SkillTemplateId == config.TemplateId);
					bool flag2 = findItemIndex >= 0;
					if (flag2)
					{
						learned[findItemIndex] = item;
					}
					else
					{
						learned.Add(item);
					}
					*(ref qualifications.Items.FixedElementField + (IntPtr)config.Type * 2) = (short)modifiedQualification;
					i += 1;
				}
			}
			else
			{
				GameData.Domains.Character.LifeSkillItem item2 = new GameData.Domains.Character.LifeSkillItem((short)templateId, 5);
				Config.LifeSkillItem config = LifeSkill.Instance.GetItem(item2.SkillTemplateId);
				int findItemIndex2 = learned.FindIndex((GameData.Domains.Character.LifeSkillItem skill) => skill.SkillTemplateId == config.TemplateId);
				bool flag3 = findItemIndex2 >= 0;
				if (flag3)
				{
					learned[findItemIndex2] = item2;
				}
				else
				{
					learned.Add(item2);
				}
				*(ref qualifications.Items.FixedElementField + (IntPtr)config.Type * 2) = (short)modifiedQualification;
			}
			CharacterDomainMethod.Call.GmCmd_SetLearnedLifeSkills(charId, learned);
			bool flag4 = modifiedQualification > 0;
			if (flag4)
			{
				GameDataBridge.AddDataModification<LifeSkillShorts>(4, 0, (ulong)((long)charId), 30U, qualifications);
			}
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(List<GameData.Domains.Character.LifeSkillItem>)),
			new ValueTuple<DataUid, Type>(uid2, typeof(LifeSkillShorts))
		});
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00059AF0 File Offset: 0x00057CF0
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CombatSkillIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public unsafe static void SetCharacterCombatSkillFullLearned(int charId, int templateId, int modifiedQualification)
	{
		DataUid uid = new DataUid(4, 0, (ulong)((long)charId), 59U);
		DataUid uid2 = new DataUid(4, 0, (ulong)((long)charId), 32U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			List<short> learned = (List<short>)data[uid];
			CombatSkillShorts qualifications = (CombatSkillShorts)data[uid2];
			bool flag = templateId < 0;
			if (flag)
			{
				short i = 0;
				while ((int)i < CombatSkill.Instance.Count)
				{
					CombatSkillItem config = CombatSkill.Instance.GetItem(i);
					bool flag2 = config.EquipType < 0;
					if (!flag2)
					{
						int findItemIndex = learned.FindIndex((short skillId) => skillId == config.TemplateId);
						bool flag3 = findItemIndex >= 0;
						if (flag3)
						{
							GameDataBridge.AddDataModification<ushort>(7, 0, (ulong)new CombatSkillKey(charId, config.TemplateId), 1U, ushort.MaxValue);
						}
						else
						{
							CharacterDomainMethod.Call.LearnCombatSkill(charId, config.TemplateId, ushort.MaxValue);
							learned.Add(config.TemplateId);
						}
						*(ref qualifications.Items.FixedElementField + (IntPtr)config.Type * 2) = (short)modifiedQualification;
					}
					i += 1;
				}
			}
			else
			{
				CombatSkillItem config = CombatSkill.Instance.GetItem((short)templateId);
				int findItemIndex2 = learned.FindIndex((short skillId) => skillId == config.TemplateId);
				bool flag4 = findItemIndex2 >= 0;
				if (flag4)
				{
					GameDataBridge.AddDataModification<ushort>(7, 0, (ulong)new CombatSkillKey(charId, config.TemplateId), 1U, ushort.MaxValue);
				}
				else
				{
					CharacterDomainMethod.Call.LearnCombatSkill(charId, config.TemplateId, ushort.MaxValue);
					learned.Add(config.TemplateId);
				}
				*(ref qualifications.Items.FixedElementField + (IntPtr)config.Type * 2) = (short)modifiedQualification;
			}
			bool flag5 = modifiedQualification > 0;
			if (flag5)
			{
				GameDataBridge.AddDataModification<CombatSkillShorts>(4, 0, (ulong)((long)charId), 32U, qualifications);
			}
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(List<short>)),
			new ValueTuple<DataUid, Type>(uid2, typeof(CombatSkillShorts))
		});
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00059B9A File Offset: 0x00057D9A
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void QueryAliveCharByPreexistenceChar(int deadCharId)
	{
		CharacterDomainMethod.AsyncCall.GmCmd_GetAliveCharByPreexistenceChar(null, deadCharId, delegate(int offset, RawDataPool dataPool)
		{
			int ret = -1;
			Serializer.Deserialize(dataPool, offset, ref ret);
			UI_GMWindow.Instance.Log(LocalStringManager.GetFormat(LanguageKey.GM_Message_GMFunc_QueryAliveCharByPreexistenceChar_Msg_0, (ret >= 0) ? ret.ToString() : LocalStringManager.Get(LanguageKey.LK_None)));
		});
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x00059BC4 File Offset: 0x00057DC4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void QueryConsummateLevelProgress(int charId)
	{
		ExtraDomainMethod.AsyncCall.GetCharacterConsummateLevelProgress(null, charId, delegate(int offset, RawDataPool pool)
		{
			int value = 0;
			Serializer.Deserialize(pool, offset, ref value);
			UI_GMWindow.Instance.Log(value.ToString());
		});
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x00059BF0 File Offset: 0x00057DF0
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void QueryAvatarData(int charId)
	{
		CharacterDomainMethod.AsyncCall.GetAvatarData(null, charId, delegate(int offset, RawDataPool pool)
		{
			AvatarData avatarData = new AvatarData();
			Serializer.Deserialize(pool, offset, ref avatarData);
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.Append(string.Format("charId: {0}\n", charId));
			FieldInfo[] fields = avatarData.GetType().GetFields();
			foreach (FieldInfo field in fields)
			{
				GMFunc.AppendFieldToStringBuilder(avatarData, field, sb);
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		});
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x00059C24 File Offset: 0x00057E24
	private static void AppendFieldToStringBuilder(AvatarData avatarData, FieldInfo field, StringBuilder sb)
	{
		SerializableGameDataFieldAttribute attr = field.GetCustomAttribute<SerializableGameDataFieldAttribute>();
		bool flag = attr == null;
		if (!flag)
		{
			sb.Append(string.Format("{0}: {1}\n", field.Name, field.GetValue(avatarData)));
		}
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x00059C64 File Offset: 0x00057E64
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void QueryCharacterLifeSpan(int charId)
	{
		DataUid uidMaxHealth = new DataUid(4, 0, (ulong)((long)charId), 94U);
		DataUid uidPhysAge = new DataUid(4, 0, (ulong)((long)charId), 75U);
		DataUid uidCurrAge = new DataUid(4, 0, (ulong)((long)charId), 65U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			short age = (short)data[uidPhysAge];
			short maxHp = (short)data[uidMaxHealth];
			sb.Append(string.Format("=== Char {0} ===\n", charId));
			sb.Append(string.Format("MaxHealth(月): {0}  ≈ {1}.{2}年\n", maxHp, (int)(maxHp / 12), (int)(maxHp % 12)));
			sb.Append(string.Format("PhysiologicalAge(年): {0}\n", age));
			UI_GMWindow.Instance.Log(sb.ToString());
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uidMaxHealth, typeof(short)),
			new ValueTuple<DataUid, Type>(uidPhysAge, typeof(short)),
			new ValueTuple<DataUid, Type>(uidCurrAge, typeof(short))
		});
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x00059D29 File Offset: 0x00057F29
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void LogCharacterSamsaraInfo()
	{
		CharacterDomainMethod.Call.GmCmd_LogCharacterSamsaraInfo();
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x00059D34 File Offset: 0x00057F34
	[GMFunc(EGMGroup.CharacterGroup, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.WorldCreationInfoTypeField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditWorldCreationInfo(int type, byte value)
	{
		WorldDomainMethod.AsyncCall.GetWorldCreationInfo(null, delegate(int offset, RawDataPool pool)
		{
			WorldCreationInfo worldCreationInfo = default(WorldCreationInfo);
			Serializer.Deserialize(pool, offset, ref worldCreationInfo);
			switch (type)
			{
			case 0:
				worldCreationInfo.CharacterLifespanType = value;
				break;
			case 1:
				worldCreationInfo.CombatDifficulty = value;
				break;
			case 2:
				worldCreationInfo.ReadingDifficulty = value;
				break;
			case 3:
				worldCreationInfo.BreakoutDifficulty = value;
				break;
			case 4:
				worldCreationInfo.LoopingDifficulty = value;
				break;
			case 5:
				worldCreationInfo.HereticsAmountType = value;
				break;
			case 6:
				worldCreationInfo.BossInvasionSpeedType = value;
				break;
			case 7:
				worldCreationInfo.WorldResourceAmountType = value;
				break;
			case 8:
				worldCreationInfo.WorldPopulationType = value;
				break;
			case 9:
				worldCreationInfo.RestrictOptionsBehaviorType = (value > 0);
				break;
			case 10:
				worldCreationInfo.AllowRandomTaiwuHeir = (value > 0);
				break;
			case 11:
				worldCreationInfo.EnemyPracticeLevel = value;
				break;
			case 12:
				worldCreationInfo.FavorabilityChange = value;
				break;
			case 13:
				worldCreationInfo.ProfessionUpgrade = value;
				break;
			case 14:
				worldCreationInfo.LootYield = (short)value;
				break;
			}
			WorldDomainMethod.Call.SetWorldCreationInfo(worldCreationInfo, false);
		});
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00059D6C File Offset: 0x00057F6C
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void KillCharacter(int charId)
	{
		DialogCmd successCmd = new DialogCmd
		{
			Title = LocalStringManager.Get("LK_Success"),
			Content = LocalStringManager.GetFormat(LanguageKey.GM_Message_GMFunc_KillCharacter_Msg_1, charId),
			Yes = new Action(GMFunc.<KillCharacter>g__OnKillConfirm|14_0)
		};
		CharacterDomainMethod.Call.GmCmd_Die(charId);
		bool flag = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		if (!flag)
		{
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", successCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00059E00 File Offset: 0x00058000
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void ResetCharacterHairGrowth(int charId)
	{
		CharacterDomainMethod.Call.GmCmd_ResetHairGrowth(charId);
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00059E0A File Offset: 0x0005800A
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CombatTypeField, 0.1f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.16f)]
	[GMFuncArg(2, EWidgetType.CharIdField, 0.16f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.08f)]
	[GMFuncArg(4, EWidgetType.IntField, 0.08f)]
	[GMFuncArg(5, EWidgetType.IntField, 0.08f)]
	public static void SimulateNpcCombat(sbyte combatType, int charIdA, int charIdB, int killBaseChance, int kidnapBaseChance, int releaseBaseChance)
	{
		CharacterDomainMethod.Call.GmCmd_SimulateNpcCombat(charIdA, charIdB, combatType, killBaseChance, kidnapBaseChance, releaseBaseChance);
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00059E1B File Offset: 0x0005801B
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	public static void SetXiangshuInfection(int charId, byte value)
	{
		GameDataBridge.AddDataModification<byte>(4, 0, (ulong)((long)charId), 64U, value);
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x00059E2B File Offset: 0x0005802B
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ChangeXiangshuInfection(int charId, int value)
	{
		CharacterDomainMethod.Call.GmCmd_ChangeXiangshuInfection(charId, value);
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x00059E36 File Offset: 0x00058036
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.12f)]
	public static void GenerateRandomRefinedItem(int charId, sbyte itemType, int times)
	{
		CharacterDomainMethod.Call.GmCmd_GenerateRandomRefinedItemToCharacter(charId, itemType, times);
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x00059E42 File Offset: 0x00058042
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	public static void CheckFavorability(int selfCharId, int relatedCharId)
	{
		CharacterDomainMethod.AsyncCall.GetFavorability(null, selfCharId, relatedCharId, delegate(int offset, RawDataPool dataPool)
		{
			short favorability = 0;
			Serializer.Deserialize(dataPool, offset, ref favorability);
			UI_GMWindow.Instance.Log(favorability.ToString());
		});
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x00059E70 File Offset: 0x00058070
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void ChangeFavorability(int selfCharId, int relatedCharId, short delta)
	{
		DataUid selfCharTemplateIdUid = new DataUid(4, 0, (ulong)((long)selfCharId), 1U);
		DataUid relatedCharTemplateIdUid = new DataUid(4, 0, (ulong)((long)relatedCharId), 1U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			CharacterDomainMethod.Call.GmCmd_ChangeFavorability(selfCharId, relatedCharId, delta);
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(selfCharTemplateIdUid, typeof(short)),
			new ValueTuple<DataUid, Type>(relatedCharTemplateIdUid, typeof(short))
		});
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x00059F08 File Offset: 0x00058108
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void QuestCharacterCombatSkillAttainmentPanels(int selfCharId)
	{
		DataUid selfCharCombatSkillAttainmentPanelsUid = new DataUid(4, 0, (ulong)((long)selfCharId), 61U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			StringBuilder sb = new StringBuilder();
			List<short> combatSkillAttainmentPanels = (List<short>)data[selfCharCombatSkillAttainmentPanelsUid];
			for (sbyte i = 0; i < 14; i += 1)
			{
				sb.Append(Config.CombatSkillType.Instance.GetItem(i).Name + ": ");
				for (int j = 0; j < 9; j++)
				{
					short skillId = combatSkillAttainmentPanels[(int)(i * 9) + j];
					bool flag = j != 0;
					if (flag)
					{
						sb.Append(", ");
					}
					bool flag2 = skillId >= 0;
					if (flag2)
					{
						sb.Append(CombatSkill.Instance.GetItem(skillId).Name ?? "");
					}
					else
					{
						sb.Append(LocalStringManager.Get(LanguageKey.LK_None));
					}
				}
				sb.AppendLine();
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(selfCharCombatSkillAttainmentPanelsUid, typeof(List<short>))
		});
	}

	// Token: 0x06000E4F RID: 3663 RVA: 0x00059F66 File Offset: 0x00058166
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.1f)]
	public static void RecordFameAction(int charId, short fameActionId, int targetCharId = -1, short fameMultiplier = 1)
	{
		CharacterDomainMethod.Call.GmCmd_RecordFameAction(charId, fameActionId, targetCharId, fameMultiplier);
	}

	// Token: 0x06000E50 RID: 3664 RVA: 0x00059F74 File Offset: 0x00058174
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	public static void AddCharacterExtraTitle(int charId, short titleTemplateId, int duration = -1)
	{
		CharacterTitleItem config = CharacterTitle.Instance[titleTemplateId];
		bool flag = config.Duration < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log("只能增加配置表中Duration字段大于等于0的条目。\n Config's Duration should great than or equal to zero");
		}
		else
		{
			CharacterDomainMethod.Call.GmCmd_AddCharacterExtraTitle(charId, titleTemplateId, duration);
		}
	}

	// Token: 0x06000E51 RID: 3665 RVA: 0x00059FB7 File Offset: 0x000581B7
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void ClearFameActionRecords(int charId)
	{
		CharacterDomainMethod.Call.GmCmd_ClearFameActionRecords(charId);
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x00059FC4 File Offset: 0x000581C4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void DisplayAllFameActions()
	{
		string ret = "";
		for (int i = 0; i < FameAction.Instance.Count; i++)
		{
			ret += string.Format("{0}-{1}  ", FameAction.Instance[i].Name.SetColor(Color.green), i);
		}
		UI_GMWindow.Instance.Log(ret);
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x0005A030 File Offset: 0x00058230
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.12f)]
	public static void MoveCharacterToCurrentMapBlock(int charId)
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		CharacterDomainMethod.Call.GmCmd_MoveIntelligentCharacter(-1, charId, new Location(mapModel.CurrentAreaId, mapModel.CurrentBlockId));
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x0005A05D File Offset: 0x0005825D
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.22f)]
	public static void CreateRandomIntelligentCharacters(int charCount, sbyte orgTemplateId, bool createHere = true)
	{
		CharacterDomainMethod.Call.GmCmd_CreateRandomIntelligentCharacters(charCount, orgTemplateId, createHere);
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0005A069 File Offset: 0x00058269
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.12f)]
	public static void RecordKilledByLongYufuCharacter(int charId)
	{
		ExtraDomainMethod.Call.GmCmd_RecordKilledByLongYufuCharacter(charId);
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x0005A073 File Offset: 0x00058273
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ReleaseAllKilledByLongYufuCharacters()
	{
		ExtraDomainMethod.Call.GmCmd_ReleaseAllKilledByLongYufuCharacters();
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x0005A07C File Offset: 0x0005827C
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void RandomizeRelationShipsInSettlement(int orgTemplateId, int times)
	{
		for (int i = 0; i < times; i++)
		{
			CharacterDomainMethod.Call.GmCmd_RandomizeRelationShipsInSettlement((sbyte)orgTemplateId);
		}
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0005A0A4 File Offset: 0x000582A4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void SetCharacterOrganization(int charId, sbyte orgTemplateId)
	{
		CharacterDomainMethod.Call.GmCmd_ForceChangeOrganization(charId, orgTemplateId);
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0005A0AF File Offset: 0x000582AF
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.StringField, 0.2f)]
	public static void SetCharacterOrganizationByName(int charId, string settlementName)
	{
		CharacterDomainMethod.AsyncCall.GmCmd_ForceChangeOrganizationByName(null, charId, settlementName, delegate(int offset, RawDataPool dataPool)
		{
			bool succeed = true;
			Serializer.Deserialize(dataPool, offset, ref succeed);
			bool flag = succeed;
			if (flag)
			{
				UI_GMWindow.Instance.Log("success");
			}
			else
			{
				UI_GMWindow.Instance.Log("failed");
			}
		});
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0005A0DA File Offset: 0x000582DA
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.2f)]
	public static void SetCharacterGrade(int charId, sbyte grade, bool principal = true)
	{
		CharacterDomainMethod.AsyncCall.GmCmd_ForceChangeGrade(null, charId, grade, principal, delegate(int offset, RawDataPool dataPool)
		{
			bool succeed = true;
			Serializer.Deserialize(dataPool, offset, ref succeed);
			bool flag = succeed;
			if (!flag)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get("LK_Failed"),
					Content = LocalStringManager.Get("GM_Message_GMFunc_SetCharacterGrade_Msg"),
					Type = 2
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		});
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0005A108 File Offset: 0x00058308
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.2f)]
	public static void QueryOrgTemplateIdByName(string orgName)
	{
		string ret = "";
		foreach (OrganizationItem config in ((IEnumerable<OrganizationItem>)Organization.Instance))
		{
			bool flag = config.Name.Contains(orgName);
			if (flag)
			{
				ret += string.Format("{0}-{1}  ", config.Name.SetColor(Color.green), config.TemplateId);
			}
		}
		UI_GMWindow.Instance.Log(ret);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x0005A1A4 File Offset: 0x000583A4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditDisorderOfQi(int charId, int value)
	{
		bool flag = value < (int)DisorderLevelOfQi.MinValue || value > (int)DisorderLevelOfQi.MaxValue;
		if (!flag)
		{
			GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 21U, (short)value);
		}
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x0005A1DA File Offset: 0x000583DA
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void ChangeDisorderOfQi(int charId, int delta)
	{
		CharacterDomainMethod.Call.GmCmd_ChangeCharDisorderOfQi(charId, delta);
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0005A1E8 File Offset: 0x000583E8
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(4, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(5, EWidgetType.IntField, 0.1f)]
	public unsafe static void EditCharBaseNeiliProportionOfFiveElements(int charId, int metal = 0, int wood = 0, int water = 0, int fire = 0, int earth = 0)
	{
		NeiliProportionOfFiveElements fiveElements = default(NeiliProportionOfFiveElements);
		fiveElements.Items.FixedElementField = (sbyte)metal;
		*(ref fiveElements.Items.FixedElementField + 1) = (sbyte)wood;
		*(ref fiveElements.Items.FixedElementField + 2) = (sbyte)water;
		*(ref fiveElements.Items.FixedElementField + 3) = (sbyte)fire;
		*(ref fiveElements.Items.FixedElementField + 4) = (sbyte)earth;
		sbyte delta = (sbyte)(fiveElements.Sum() - 100);
		int unchangedCount = 0;
		for (int i = 0; i < 5; i++)
		{
			bool flag = *(ref fiveElements.Items.FixedElementField + i) != 0;
			if (!flag)
			{
				unchangedCount++;
			}
		}
		bool flag2 = unchangedCount == 0;
		if (flag2)
		{
			unchangedCount = 5;
		}
		int avgDelta = (int)delta / unchangedCount;
		int remDelta = (int)delta - avgDelta * unchangedCount;
		for (int j = 0; j < 5; j++)
		{
			bool flag3 = *(ref fiveElements.Items.FixedElementField + j) != 0 && unchangedCount != 5;
			if (!flag3)
			{
				ref sbyte ptr = ref fiveElements.Items.FixedElementField + j;
				ptr -= (sbyte)avgDelta;
			}
		}
		fiveElements.Items.FixedElementField = fiveElements.Items.FixedElementField - (sbyte)remDelta;
		CharacterDomainMethod.Call.GmCmd_SetCharBaseNeiliProportionOfFiveElements(charId, fiveElements);
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x0005A320 File Offset: 0x00058520
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(4, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(5, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(6, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(7, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(8, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(9, EWidgetType.BoolField, 0.08f)]
	public unsafe static void EditCharacterResource(int charId, int value, bool food = true, bool wood = true, bool metal = true, bool jade = true, bool fabric = true, bool herb = true, bool money = true, bool author = true)
	{
		bool flag = charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		if (flag)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get("LK_Failed"),
				Content = LocalStringManager.Get("GM_Message_GMFunc_EditCharacterResource_Msg_0")
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			bool flag2 = value < 0;
			if (!flag2)
			{
				DataUid uid = new DataUid(4, 0, (ulong)((long)charId), 34U);
				UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
				{
					ResourceInts resources = (ResourceInts)data[uid];
					bool food2 = food;
					if (food2)
					{
						resources.Items.FixedElementField = value;
					}
					bool wood2 = wood;
					if (wood2)
					{
						*(ref resources.Items.FixedElementField + 4) = value;
					}
					bool metal2 = metal;
					if (metal2)
					{
						*(ref resources.Items.FixedElementField + (IntPtr)2 * 4) = value;
					}
					bool jade2 = jade;
					if (jade2)
					{
						*(ref resources.Items.FixedElementField + (IntPtr)3 * 4) = value;
					}
					bool fabric2 = fabric;
					if (fabric2)
					{
						*(ref resources.Items.FixedElementField + (IntPtr)4 * 4) = value;
					}
					bool herb2 = herb;
					if (herb2)
					{
						*(ref resources.Items.FixedElementField + (IntPtr)5 * 4) = value;
					}
					bool money2 = money;
					if (money2)
					{
						*(ref resources.Items.FixedElementField + (IntPtr)6 * 4) = value;
					}
					bool author2 = author;
					if (author2)
					{
						*(ref resources.Items.FixedElementField + (IntPtr)7 * 4) = value;
					}
					GameDataBridge.AddDataModification<ResourceInts>(uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1, resources);
				}, new ValueTuple<DataUid, Type>[]
				{
					new ValueTuple<DataUid, Type>(uid, typeof(ResourceInts))
				});
			}
		}
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x0005A437 File Offset: 0x00058637
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void EditCharacterHealthAndBaseMaxHealth(int charId, int value, int max)
	{
		GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 19U, (short)value);
		GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 20U, (short)max);
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x0005A456 File Offset: 0x00058656
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void EditCharacterLovingAndHatingItemSubType(int charId, int loving, int hating)
	{
		GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 35U, (short)loving);
		GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 36U, (short)hating);
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x0005A478 File Offset: 0x00058678
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(4, EWidgetType.IntField, 0.2f)]
	public unsafe static void EditExtraNeiliAllocation(int charId, int neili0, int neili1, int neili2, int neili3)
	{
		NeiliAllocation value = default(NeiliAllocation);
		value.Items.FixedElementField = (short)neili0;
		*(ref value.Items.FixedElementField + 2) = (short)neili1;
		*(ref value.Items.FixedElementField + (IntPtr)2 * 2) = (short)neili2;
		*(ref value.Items.FixedElementField + (IntPtr)3 * 2) = (short)neili3;
		CharacterDomainMethod.Call.GmCmd_EditExtraNeiliAllocation(charId, value);
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x0005A4DF File Offset: 0x000586DF
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditExtraNeili(int charId, int value)
	{
		GameDataBridge.AddDataModification<int>(4, 0, (ulong)((long)charId), 27U, value);
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x0005A4EF File Offset: 0x000586EF
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditCurrNeili(int charId, int value)
	{
		CharacterDomainMethod.Call.GmCmd_SetCurrNeili(charId, value);
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x0005A4FA File Offset: 0x000586FA
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditConsummateLevel(int charId, int value)
	{
		GameDataBridge.AddDataModification<sbyte>(4, 0, (ulong)((long)charId), 28U, (sbyte)value);
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x0005A50B File Offset: 0x0005870B
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.1f)]
	public static void EditActualAge(int charId, int value, bool isActualAge = false)
	{
		GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), isActualAge ? 4U : 65U, (short)value);
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x0005A524 File Offset: 0x00058724
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditBaseHealth(int charId, int health)
	{
		short target = (short)health;
		bool flag = target < 0;
		if (!flag)
		{
			DataUid maxHealthUid = new DataUid(4, 0, (ulong)((long)charId), 94U);
			DataUid baseMaxHealthUid = new DataUid(4, 0, (ulong)((long)charId), 20U);
			UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
			{
				short maxHealth = (short)data[maxHealthUid];
				bool flag2 = maxHealth < target;
				if (flag2)
				{
					short baseMaxHealth = (short)data[baseMaxHealthUid];
					GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 20U, baseMaxHealth + (target - maxHealth));
				}
				GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 19U, target);
			}, new ValueTuple<DataUid, Type>[]
			{
				new ValueTuple<DataUid, Type>(maxHealthUid, typeof(short)),
				new ValueTuple<DataUid, Type>(baseMaxHealthUid, typeof(short))
			});
		}
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x0005A5DC File Offset: 0x000587DC
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetMaxHealth(int charId, int health)
	{
		short target = (short)health;
		bool flag = target < 0;
		if (!flag)
		{
			DataUid maxHealthUid = new DataUid(4, 0, (ulong)((long)charId), 94U);
			DataUid baseMaxHealthUid = new DataUid(4, 0, (ulong)((long)charId), 20U);
			UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
			{
				short maxHealth = (short)data[maxHealthUid];
				bool flag2 = maxHealth < target;
				if (flag2)
				{
					short baseMaxHealth = (short)data[baseMaxHealthUid];
					GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 20U, baseMaxHealth + (target - maxHealth));
				}
			}, new ValueTuple<DataUid, Type>[]
			{
				new ValueTuple<DataUid, Type>(maxHealthUid, typeof(short)),
				new ValueTuple<DataUid, Type>(baseMaxHealthUid, typeof(short))
			});
		}
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0005A692 File Offset: 0x00058892
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetHealth(int charId, int health)
	{
		GameDataBridge.AddDataModification<int>(4, 0, (ulong)((long)charId), 19U, health);
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x0005A6A2 File Offset: 0x000588A2
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditHappiness(int charId, int value)
	{
		GameDataBridge.AddDataModification<sbyte>(4, 0, (ulong)((long)charId), 6U, (sbyte)value);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0005A6B2 File Offset: 0x000588B2
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditBaseMorality(int charId, int value)
	{
		GameDataBridge.AddDataModification<short>(4, 0, (ulong)((long)charId), 7U, (short)value);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x0005A6C2 File Offset: 0x000588C2
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	public static void EditBisexual(int charId, bool value)
	{
		GameDataBridge.AddDataModification<bool>(4, 0, (ulong)((long)charId), 14U, value);
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x0005A6D2 File Offset: 0x000588D2
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditMonkType(int charId, int value)
	{
		GameDataBridge.AddDataModification<byte>(4, 0, (ulong)((long)charId), 16U, (byte)value);
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x0005A6E4 File Offset: 0x000588E4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(4, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(5, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(6, EWidgetType.IntField, 0.1f)]
	public unsafe static void EditBaseMainAttributes(int charId, int attr0, int attr1, int attr2, int attr3, int attr4, int attr5)
	{
		GMFunc.<>c__DisplayClass54_0 CS$<>8__locals1 = new GMFunc.<>c__DisplayClass54_0();
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.value = new short[]
		{
			(short)attr0,
			(short)attr1,
			(short)attr2,
			(short)attr3,
			(short)attr4,
			(short)attr5
		};
		DataUid uid = new DataUid(4, 0, (ulong)((long)CS$<>8__locals1.charId), 18U);
		DataUid uid2 = new DataUid(4, 0, (ulong)((long)CS$<>8__locals1.charId), 43U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			MainAttributes baseAttr = (MainAttributes)data[uid];
			MainAttributes currAttr = (MainAttributes)data[uid2];
			for (int i = 0; i < 6; i++)
			{
				ref short ptr = ref currAttr.Items.FixedElementField + (IntPtr)i * 2;
				ptr += CS$<>8__locals1.value[i] - *(ref baseAttr.Items.FixedElementField + (IntPtr)i * 2);
				*(ref baseAttr.Items.FixedElementField + (IntPtr)i * 2) = CS$<>8__locals1.value[i];
			}
			GameDataBridge.AddDataModification<MainAttributes>(4, 0, (ulong)((long)CS$<>8__locals1.charId), 18U, baseAttr);
			GameDataBridge.AddDataModification<MainAttributes>(4, 0, (ulong)((long)CS$<>8__locals1.charId), 43U, currAttr);
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(MainAttributes)),
			new ValueTuple<DataUid, Type>(uid2, typeof(MainAttributes))
		});
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0005A7C8 File Offset: 0x000589C8
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(4, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(5, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(6, EWidgetType.IntField, 0.1f)]
	public unsafe static void EditCurrMainAttributes(int charId, int attr0, int attr1, int attr2, int attr3, int attr4, int attr5)
	{
		GMFunc.<>c__DisplayClass55_0 CS$<>8__locals1 = new GMFunc.<>c__DisplayClass55_0();
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.value = new short[]
		{
			(short)attr0,
			(short)attr1,
			(short)attr2,
			(short)attr3,
			(short)attr4,
			(short)attr5
		};
		DataUid uid = new DataUid(4, 0, (ulong)((long)CS$<>8__locals1.charId), 43U);
		DataUid uid2 = new DataUid(4, 0, (ulong)((long)CS$<>8__locals1.charId), 79U);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			MainAttributes currAttr = (MainAttributes)data[uid];
			MainAttributes maxAttr = (MainAttributes)data[uid2];
			for (int i = 0; i < 6; i++)
			{
				*(ref currAttr.Items.FixedElementField + (IntPtr)i * 2) = (short)Mathf.Clamp((int)CS$<>8__locals1.value[i], 0, (int)(*(ref maxAttr.Items.FixedElementField + (IntPtr)i * 2)));
			}
			GameDataBridge.AddDataModification<MainAttributes>(4, 0, (ulong)((long)CS$<>8__locals1.charId), 43U, currAttr);
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(MainAttributes)),
			new ValueTuple<DataUid, Type>(uid2, typeof(MainAttributes))
		});
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0005A8AA File Offset: 0x00058AAA
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(4, EWidgetType.BoolField, 0.2f)]
	public static void EditDisableState(int charId, bool haveLeftArm, bool haveRightArm, bool haveLeftLeg, bool haveRightLeg)
	{
		GameDataBridge.AddDataModification<bool>(4, 0, (ulong)((long)charId), 22U, haveLeftArm);
		GameDataBridge.AddDataModification<bool>(4, 0, (ulong)((long)charId), 23U, haveRightArm);
		GameDataBridge.AddDataModification<bool>(4, 0, (ulong)((long)charId), 24U, haveLeftLeg);
		GameDataBridge.AddDataModification<bool>(4, 0, (ulong)((long)charId), 25U, haveRightLeg);
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x0005A8E4 File Offset: 0x00058AE4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	public static void MakeCharacterKidnapped(int charId, int targetCharId)
	{
		bool flag = charId == targetCharId;
		if (!flag)
		{
			bool flag2 = targetCharId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			if (flag2)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get("GM_Message_GMFunc_TargetTaiwu_Msg_0"),
					Content = LocalStringManager.Get("GM_Message_GMFunc_TargetTaiwu_Msg_0"),
					Type = 2
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				DataUid uid = new DataUid(4, 0, (ulong)((long)targetCharId), 68U);
				DataUid selfCharTemplateIdUid = new DataUid(4, 0, (ulong)((long)charId), 1U);
				DataUid targetCharTemplateIdUid = new DataUid(4, 0, (ulong)((long)targetCharId), 1U);
				UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
				{
					int kidnapper = (int)data[uid];
					byte selfCharCreatingType = Character.Instance[(short)data[selfCharTemplateIdUid]].CreatingType;
					byte targetCharCreatingType = Character.Instance[(short)data[targetCharTemplateIdUid]].CreatingType;
					bool flag3 = kidnapper < 0 && (charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId || selfCharCreatingType == 1) && targetCharCreatingType == 1;
					if (flag3)
					{
						CharacterDomainMethod.Call.GmCmd_MakeCharacterKidnapped(charId, targetCharId);
					}
				}, new ValueTuple<DataUid, Type>[]
				{
					new ValueTuple<DataUid, Type>(uid, typeof(int)),
					new ValueTuple<DataUid, Type>(selfCharTemplateIdUid, typeof(short)),
					new ValueTuple<DataUid, Type>(targetCharTemplateIdUid, typeof(short))
				});
			}
		}
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x0005AA42 File Offset: 0x00058C42
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.2f)]
	public static void MakeCharacterHaveSex(int selfCharId, int targetCharId, bool isRaped, int pregnantRemainTime)
	{
		pregnantRemainTime = Math.Max(0, pregnantRemainTime) + 1;
		CharacterDomainMethod.Call.GmCmd_MakeCharacterHaveSex(selfCharId, targetCharId, isRaped, pregnantRemainTime);
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x0005AA5C File Offset: 0x00058C5C
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.2f)]
	public static void ChangeInjury(int charId, bool isInnerInjury, sbyte bodyPartType, sbyte delta)
	{
		bool flag = bodyPartType < 0 || bodyPartType > 6;
		if (flag)
		{
			UI_GMWindow.Instance.Log("身体部位值设置错误(0-胸背；1-腰腹；2-头颅；3-左臂；4-右臂；5-左腿；6-右腿)");
		}
		else
		{
			CharacterDomainMethod.Call.GmCmd_ChangeInjury(charId, isInnerInjury, bodyPartType, delta);
			UI_GMWindow.Instance.Log("设置成功");
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0005AAA6 File Offset: 0x00058CA6
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void SetReadingEvent(int charId)
	{
		CharacterDomainMethod.Call.GmCmd_SetCurReadingEvent(charId);
		UI_GMWindow.Instance.Log("设置成功");
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0005AAC0 File Offset: 0x00058CC0
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void SetLoopingEvent(int charId)
	{
		CharacterDomainMethod.Call.GmCmd_SetCurLoopingEvent(charId);
		UI_GMWindow.Instance.Log("设置成功");
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0005AADA File Offset: 0x00058CDA
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void FollowNpc(int charId)
	{
		TaiwuDomainMethod.Call.TaiwuFollowNpc(charId);
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0005AAE4 File Offset: 0x00058CE4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ApplyLoopingEffect()
	{
		TaiwuDomainMethod.Call.GmCmd_TaiwuActiveLoopingApply();
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0005AAED File Offset: 0x00058CED
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void UnfollowNpc(int charId)
	{
		TaiwuDomainMethod.Call.TaiwuUnfollowNpc(charId);
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0005AAF7 File Offset: 0x00058CF7
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void FollowRandomNpc(int count)
	{
		TaiwuDomainMethod.Call.GmCmd_FollowRandomNpc(count);
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0005AB01 File Offset: 0x00058D01
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void AddFeature(int charId, short templateID)
	{
		CharacterDomainMethod.Call.GmCmd_AddFeature(charId, templateID);
		UI_GMWindow.Instance.Log("设置成功");
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0005AB1C File Offset: 0x00058D1C
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void RemoveFeature(int charId, short templateID)
	{
		CharacterDomainMethod.Call.GmCmd_RemoveFeature(charId, templateID);
		UI_GMWindow.Instance.Log("设置成功");
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0005AB38 File Offset: 0x00058D38
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.StringField, 0.2f)]
	public static void AddFeatureByName(int charId, string featureName)
	{
		CharacterFeatureItem characterFeatureItem = CharacterFeature.Instance.FirstOrDefault((CharacterFeatureItem x) => x.Name == featureName);
		short featureId = (characterFeatureItem != null) ? characterFeatureItem.TemplateId : -1;
		bool flag = featureId < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log(("调用失败，无法找到 " + featureName + " 对应的特性").SetColor(Color.red));
		}
		else
		{
			CharacterDomainMethod.Call.GmCmd_AddFeature(charId, featureId);
			UI_GMWindow.Instance.Log("调用成功".SetColor(Color.green));
		}
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0005ABD0 File Offset: 0x00058DD0
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.StringField, 0.2f)]
	public static void RemoveFeatureByName(int charId, string featureName)
	{
		CharacterFeatureItem characterFeatureItem = CharacterFeature.Instance.FirstOrDefault((CharacterFeatureItem x) => x.Name == featureName);
		short featureId = (characterFeatureItem != null) ? characterFeatureItem.TemplateId : -1;
		bool flag = featureId < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log(("调用失败，无法找到 " + featureName + " 对应的特性").SetColor(Color.red));
		}
		else
		{
			CharacterDomainMethod.Call.GmCmd_RemoveFeature(charId, featureId);
			UI_GMWindow.Instance.Log("调用成功".SetColor(Color.green));
		}
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0005AC68 File Offset: 0x00058E68
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.StringField, 0.2f)]
	public static void SetFeatures(int charId, string features)
	{
		List<short> featureList = (from p in features.Split(' ', StringSplitOptions.None)
		select Convert.ToInt16(p)).ToList<short>();
		CharacterDomainMethod.Call.GmCmd_SetFeatures(charId, featureList);
		UI_GMWindow.Instance.Log("设置成功");
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0005ACC4 File Offset: 0x00058EC4
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void ChangePoisoned(int charId, sbyte poisonType, int changeValue)
	{
		bool flag = poisonType < 0 || poisonType > 5;
		if (flag)
		{
			UI_GMWindow.Instance.Log("中毒类型设置错误（0-烈毒；1-郁毒；2-寒毒；3-赤毒；4-腐毒；5-幻毒）");
		}
		else
		{
			CharacterDomainMethod.Call.GmCmd_ChangePoisonByType(charId, poisonType, changeValue);
			UI_GMWindow.Instance.Log("设置成功");
		}
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0005AD0D File Offset: 0x00058F0D
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.4f)]
	public static void QueryFixedCharacterIdByTemplateId(short templateId)
	{
		CharacterDomainMethod.AsyncCall.GetFixedCharacterIdByTemplateId(null, templateId, delegate(int offset, RawDataPool dataPool)
		{
			int ret = 0;
			Serializer.Deserialize(dataPool, offset, ref ret);
			UI_GMWindow.Instance.Log(string.Format("{0}", ret));
		});
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0005AD37 File Offset: 0x00058F37
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.ProfessionIdField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	public static void SetCharacterProfessionSeniority(int charId, int professionId, int value = -1)
	{
		CharacterDomainMethod.Call.GmCmd_SetCharacterCurrProfessionSeniority(charId, professionId, value);
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0005AD43 File Offset: 0x00058F43
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharacterRelationshipTypeField, 0.1f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.CharIdField, 0.2f)]
	public static void AddCharacterRelationship(sbyte type, int charIdA, int charIdB)
	{
		CharacterDomainMethod.Call.GmCmd_AddRelation(charIdA, charIdB, RelationType.GetRelationType(type));
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0005AD54 File Offset: 0x00058F54
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharacterRelationshipTypeField, 0.1f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.CharIdField, 0.2f)]
	public static void RemoveCharacterRelationship(sbyte type, int charIdA, int charIdB)
	{
		CharacterDomainMethod.Call.GmCmd_RemoveRelation(charIdA, charIdB, RelationType.GetRelationType(type));
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0005AD65 File Offset: 0x00058F65
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void AddJieqingMaskCharId(int charId)
	{
		TaiwuEventDomainMethod.Call.GmCmd_AddJieqingMaskCharId(charId);
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0005AD6F File Offset: 0x00058F6F
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void RemoveJieqingMaskCharId(int charId)
	{
		TaiwuEventDomainMethod.Call.GmCmd_RemoveJieqingMaskCharId(charId);
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x0005AD79 File Offset: 0x00058F79
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.1f)]
	public static void DriveWugKing(int charId, int wugKingType, bool isPositiveType)
	{
		CharacterDomainMethod.Call.GmCmd_DriveWugKing(charId, (sbyte)wugKingType, isPositiveType);
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x0005AD86 File Offset: 0x00058F86
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.1f)]
	public static void GetAlertness(int charId)
	{
		CharacterDomainMethod.AsyncCall.GetAlertnessValue(null, charId, delegate(int offset, RawDataPool pool)
		{
			int ret = 0;
			Serializer.Deserialize(pool, offset, ref ret);
			UI_GMWindow.Instance.Log(string.Format("{0}", ret));
		});
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0005ADB0 File Offset: 0x00058FB0
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.3f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.4f)]
	public static void SetAlertness(int charId, int value, bool alsoSetFavorToMaxAvailable = false)
	{
		CharacterDomainMethod.Call.SetAlertnessValue(charId, value);
		if (alsoSetFavorToMaxAvailable)
		{
			GMFunc.ChangeFavorability(charId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, 30000);
		}
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x0005ADE4 File Offset: 0x00058FE4
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(4, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(5, EWidgetType.BoolField, 0.08f)]
	[GMFuncArg(6, EWidgetType.BoolField, 0.08f)]
	public static void GetBaseResource(int value, bool food = true, bool wood = true, bool metal = true, bool jade = true, bool fabric = true, bool herb = true)
	{
		if (food)
		{
			GMFunc.AddResource(0, value);
		}
		if (wood)
		{
			GMFunc.AddResource(1, value);
		}
		if (metal)
		{
			GMFunc.AddResource(2, value);
		}
		if (jade)
		{
			GMFunc.AddResource(3, value);
		}
		if (fabric)
		{
			GMFunc.AddResource(4, value);
		}
		if (herb)
		{
			GMFunc.AddResource(5, value);
		}
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0005AE48 File Offset: 0x00059048
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.1f)]
	public static void GetAdvancedResource(int value, bool money = true, bool authority = true, bool exp = true)
	{
		if (money)
		{
			GMFunc.AddResource(6, value);
		}
		if (authority)
		{
			GMFunc.AddResource(7, value);
		}
		if (exp)
		{
			TaiwuDomainMethod.Call.GmCmd_AddExp(value);
		}
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0005AE7C File Offset: 0x0005907C
	private static void AddResource(sbyte type, int count)
	{
		TaiwuDomainMethod.Call.GmCmd_AddResource(type, count);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x0005AE88 File Offset: 0x00059088
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryCricketLuckPoint()
	{
		DataUid dataId1 = new DataUid(5, 2, ulong.MaxValue, uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			int ret = (int)data[dataId1];
			UI_GMWindow.Instance.Log(string.Format("{0}", ret));
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(dataId1, typeof(int))
		});
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x0005AEE5 File Offset: 0x000590E5
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void EditCricketLuckPoint(int point)
	{
		GameDataBridge.AddDataModification<int>(5, 2, ulong.MaxValue, uint.MaxValue, point);
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0005AEF4 File Offset: 0x000590F4
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.JiaoField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(4, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(5, EWidgetType.BoolField, 0.2f)]
	public static void AddJiao(int templateId, int nurturanceTempalteId, int growthStage, bool isMale, bool tamed, bool maxProperty)
	{
		ExtraDomainMethod.AsyncCall.GmCmd_AddJiao(null, (short)templateId, (short)nurturanceTempalteId, (sbyte)growthStage, isMale, tamed, maxProperty, delegate(int offsetE, RawDataPool dataPoolE)
		{
			int result = -1;
			Serializer.Deserialize(dataPoolE, offsetE, ref result);
			UI_GMWindow.Instance.Log(string.Format("jiao Id: {0}", result));
		});
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x0005AF33 File Offset: 0x00059133
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.1f)]
	public static void PutJiaoInFirstPool(int JiaoId)
	{
		ExtraDomainMethod.Call.GmCmd_PutJiaoInFirstPool(JiaoId);
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x0005AF3D File Offset: 0x0005913D
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.ChildrenOfLoongField, 0.1f)]
	public static void AddChildOfLoong(int templateId)
	{
		ExtraDomainMethod.AsyncCall.GmCmd_AddChildOfLoong(null, (short)templateId, delegate(int offsetE, RawDataPool dataPoolE)
		{
			int result = -1;
			Serializer.Deserialize(dataPoolE, offsetE, ref result);
			UI_GMWindow.Instance.Log(string.Format("jiao Id: {0}", result));
		});
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x0005AF68 File Offset: 0x00059168
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	public static void AddFleeCarrier(bool isJiaoLoong)
	{
		ExtraDomainMethod.Call.GmCmd_AddFleeCarrier(isJiaoLoong);
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x0005AF72 File Offset: 0x00059172
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void AddRanshanThreeCorpses()
	{
		ExtraDomainMethod.Call.GmCmd_AddThreeCorpses();
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0005AF7B File Offset: 0x0005917B
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void AddRanshanLegendaryBookOwnerData()
	{
		ExtraDomainMethod.Call.GmCmd_AddDisplayEventLegendaryBookKeeping();
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0005AF84 File Offset: 0x00059184
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GiveTaiwuLegendaryBooksOut()
	{
		LegendaryBookDomainMethod.Call.GmCmd_GiveAllTaiwuLegendaryBookToRandomNpc();
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0005AF8D File Offset: 0x0005918D
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void InitYuanshanThreeVitals()
	{
		ExtraDomainMethod.Call.GmCmd_InitThreeVitals();
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0005AF96 File Offset: 0x00059196
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void InitJieQingGameData()
	{
		ExtraDomainMethod.Call.InitJieqingGameData();
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0005AF9F File Offset: 0x0005919F
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void OpenJieQingTangram()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			UIManager.Instance.ShowUI(UIElement.JieQingTangram, true);
		});
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0005AFD0 File Offset: 0x000591D0
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void SetJieqingChapterIndex(short chapterIndex)
	{
		bool flag = chapterIndex < 0 || chapterIndex > 6;
		if (flag)
		{
			UI_GMWindow.Instance.Log("chapterIndex need set 0~6".SetColor(Color.red));
		}
		ViewJieQingTangram jieQingTangram = UIElement.JieQingTangram.UiBaseAs<ViewJieQingTangram>();
		bool flag2 = jieQingTangram == null;
		if (flag2)
		{
			UI_GMWindow.Instance.Log("Need InitJieQingGameData and  OpenJieQingTangram first".SetColor(Color.red));
		}
		else
		{
			jieQingTangram.CurrentTotalChapterIndex = chapterIndex;
		}
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0005B044 File Offset: 0x00059244
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 1000, null, GmRunMode.Default)]
	public static void OpenJieQingInteract()
	{
		UIManager.Instance.ShowUI(UIElement.JieQingInteract, true);
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x0005B058 File Offset: 0x00059258
	[GMFunc(EGMGroup.CharacterResource, 0.4f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	public static void SetJieqingSpecialInteractionUnlocked(bool unlocked)
	{
		OrganizationDomainMethod.Call.GmCmd_SetSectFunctionStatus(13, SectFunctionStatuses.SectFunctionStatusType.SpecialInteractionUnlocked, unlocked);
		GEvent.OnEvent(UiEvents.OnJieqingSignStateRefresh, null);
		UI_GMWindow.Instance.Log(("界青特殊互动已" + (unlocked ? "开启" : "关闭") + "，世界地图标记已刷新").SetColor(Color.green));
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0005B0B4 File Offset: 0x000592B4
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 900, null, GmRunMode.Default)]
	public static void InitJixiSpecialInteractData()
	{
		ExtraDomainMethod.Call.InitJixiSpecialInteractData();
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0005B0BD File Offset: 0x000592BD
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 901, null, GmRunMode.Default)]
	public static void TestGetDisplayData()
	{
		ExtraDomainMethod.AsyncCall.GetJixiSpecialInteractDisplayData(null, delegate(int offset, RawDataPool pool)
		{
			JixiSpecialInteractDisplayData data = new JixiSpecialInteractDisplayData();
			Serializer.Deserialize(pool, offset, ref data);
			Debug.Log(string.Concat(new string[]
			{
				"test GetJixiSpecialInteractDisplayData ",
				string.Format("charId:{0}\r\n", data.JixiCharId),
				string.Format("JixiCurrentTemplateId:{0}\r\n", data.JixiCurrentTemplateId),
				string.Format("JixiTargetCharIdByTaiwu:{0}\r\n", data.JixiTargetCharIdByTaiwu),
				string.Format("DrainTargetNeiliAllocType:{0}\r\n", data.DrainTargetNeiliAllocType),
				string.Format("NeiliAllocProgressDrained 0:{0}\r\n", data.NeiliAllocProgressDrained.Items[0]),
				string.Format("NeiliAllocProgressDrained 1:{0}\r\n", data.NeiliAllocProgressDrained.Items[1]),
				string.Format("NeiliAllocProgressDrained 2:{0}\r\n", data.NeiliAllocProgressDrained.Items[2]),
				string.Format("NeiliAllocProgressDrained 3:{0}\r\n", data.NeiliAllocProgressDrained.Items[3]),
				string.Format("FixedNeiliProgressPerAllocation:{0}\r\n", data.FixedNeiliProgressPerAllocation),
				string.Format("GrowthValue:{0}\r\n", data.GrowthValue),
				string.Format("GrowthTotalYoung:{0}\r\n", data.GrowthTotalYoung),
				string.Format("GrowthTotalAdult:{0}\r\n", data.GrowthTotalAdult),
				string.Format("KillAmount:{0}\r\n", data.KillAmount),
				string.Format("NeiliAllocDrainedTotal:{0}\r\n", data.NeiliAllocDrainedTotal),
				string.Format("TransferToTaiwuTotal:{0}\r\n", data.TransferToTaiwuTotal),
				string.Format("TransferFromTaiwuTotal:{0}\r\n", data.TransferFromTaiwuTotal),
				string.Format("TempalteEnemyKilledTotal:{0}\r\n", data.TempalteEnemyKilledTotal),
				string.Format("KillTempalteEnemyGainTotal:{0}\r\n", data.KillTempalteEnemyGainTotal),
				string.Format("ChangeFormTotal:{0},{1},{2}\r\n", data.ChangeFormTotal[0], data.ChangeFormTotal[1], data.ChangeFormTotal[2]),
				string.Format("RescueTaiwuTimes:{0}\r\n", data.RescueTaiwuTimes),
				string.Format("CurrentTargetNeiliAllocProgressDrained:{0},{1},{2},{3}\r\n", new object[]
				{
					data.CurrentTargetNeiliAllocProgressDrained.Items[0],
					data.CurrentTargetNeiliAllocProgressDrained.Items[1],
					data.CurrentTargetNeiliAllocProgressDrained.Items[2],
					data.CurrentTargetNeiliAllocProgressDrained.Items[3]
				}),
				string.Format("CurrentFormKillAmount:{0}\r\n", data.CurrentFormKillAmount),
				string.Format("CurrentFormNeiliAllocProgressDrained:{0},{1},{2},{3}\r\n", new object[]
				{
					data.CurrentFormNeiliAllocProgressDrained.Items[0],
					data.CurrentFormNeiliAllocProgressDrained.Items[1],
					data.CurrentFormNeiliAllocProgressDrained.Items[2],
					data.CurrentFormNeiliAllocProgressDrained.Items[3]
				}),
				string.Format("CurrentFormMonthlyProgress:{0}\r\n", data.CurrentFormMonthlyProgress),
				string.Format("KillTargets:{0}\r\n", data.KillTargets != null),
				string.Format("JixiDrainNeili:{0}\r\n", data.JixiDrainNeili),
				string.Format("Favorability:{0}\r\n", data.Favorability),
				string.Format("TaiwuTargetCharacterId:{0}\r\n", data.TaiwuTargetCharacterId),
				string.Format("TaiwuTargetFiveElementsType:{0}\r\n", data.TaiwuTargetFiveElementsType),
				"TaiwuTransformFiveElementsTotal:",
				(data.TaiwuTransformFiveElementsTotal != null) ? string.Join<int>(",", data.TaiwuTransformFiveElementsTotal) : "null",
				"\r\n"
			}));
		});
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x0005B0E6 File Offset: 0x000592E6
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 902, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	public static void ChangeJixiTarget(bool isDrainNeili, int targetId, int neiliType)
	{
		ExtraDomainMethod.Call.SetJixiDrainNeili(isDrainNeili);
		ExtraDomainMethod.Call.SetJixiTarget(targetId);
		ExtraDomainMethod.Call.SetJixiDrainType((sbyte)neiliType);
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0005B100 File Offset: 0x00059300
	[GMFunc(EGMGroup.CharacterResource, 0.25f, 903, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	public static void TransferJixiNeili(bool toTaiwu, int amount, int neiliTypeInt)
	{
		byte neiliType = (byte)neiliTypeInt;
		if (toTaiwu)
		{
			ExtraDomainMethod.Call.JixiTransferNeiliAllocToTaiwu(amount, neiliType);
		}
		else
		{
			ExtraDomainMethod.Call.TaiwuTransferNeiliAllocToJixi(amount, neiliType);
		}
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0005B12C File Offset: 0x0005932C
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(2, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(3, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(4, EWidgetType.ItemIdField, 0.12f)]
	public static void GetItem(int charId, int count, sbyte itemType, short idStar, short? idEnd)
	{
		bool flag = idEnd != null;
		if (flag)
		{
			idEnd = new short?(Math.Max(idStar, idEnd.Value));
			short itemId = idStar;
			for (;;)
			{
				int num = (int)itemId;
				short? num2 = idEnd;
				int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
				if (!(num <= num3.GetValueOrDefault() & num3 != null))
				{
					break;
				}
				CharacterDomainMethod.Call.CreateInventoryItem(charId, itemType, itemId, count);
				itemId += 1;
			}
		}
		else
		{
			CharacterDomainMethod.Call.CreateInventoryItem(charId, itemType, idStar, count);
		}
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0005B1BC File Offset: 0x000593BC
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.StringField, 0.2f)]
	public static void GetItemByName(int charId, int count, string name)
	{
		IItemConfig config = ItemConfigHelper.AllConfigs.FirstOrDefault((IItemConfig x) => x.Name == name);
		bool flag = config == null;
		if (flag)
		{
			UI_GMWindow.Instance.Log(("Cannot find item named " + name).SetColor(Color.red));
		}
		else
		{
			GMFunc.GetItem(charId, count, config.ItemType, config.TemplateId, null);
			UI_GMWindow.Instance.Log(string.Format("Success get item {0} {1}", config.ItemType, config.TemplateId).SetColor(Color.green));
		}
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x0005B274 File Offset: 0x00059474
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.2f)]
	public static void QueryItemIdByName(string name)
	{
		bool flag = string.IsNullOrEmpty(name);
		if (!flag)
		{
			string ret = "";
			IEnumerable<IItemConfig> allConfigs = ItemConfigHelper.AllConfigs;
			Func<IItemConfig, bool> <>9__0;
			Func<IItemConfig, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((IItemConfig x) => x.Name.Contains(name)));
			}
			foreach (IItemConfig one in allConfigs.Where(predicate))
			{
				ret += string.Format("{0}-type:{1}-id:{2}\n", one.Name.SetColor(Color.green), one.ItemType, one.TemplateId);
			}
			UI_GMWindow.Instance.Log(ret);
		}
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0005B350 File Offset: 0x00059550
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryItemType()
	{
		string ret = "";
		for (int i = 0; i < 13; i++)
		{
			ret += string.Format("{0}-{1}  ", LocalStringManager.Get(string.Format("LK_ItemType_{0}", i)).SetColor(Color.green), i);
		}
		UI_GMWindow.Instance.Log(ret);
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0005B3B9 File Offset: 0x000595B9
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void GetCricket(short colorId, short partId)
	{
		CharacterDomainMethod.Call.GmCmd_GetCricket(colorId, partId);
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0005B3C4 File Offset: 0x000595C4
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void GetPoison(sbyte poisonType, sbyte grade, int count = 1)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		short itemId = -1;
		foreach (MedicineItem medicine in ((IEnumerable<MedicineItem>)Medicine.Instance))
		{
			bool flag = medicine.Grade == grade && medicine.EffectType == EMedicineEffectType.ApplyPoison && medicine.PoisonType == poisonType;
			if (flag)
			{
				itemId = medicine.TemplateId;
			}
		}
		bool flag2 = itemId >= 0;
		if (flag2)
		{
			CharacterDomainMethod.Call.CreateInventoryItem(taiwuCharId, 8, itemId, count);
		}
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0005B45C File Offset: 0x0005965C
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void GetRandomCricket(int grade, int winsCount, int lossesCount)
	{
		bool flag = grade < 0 || grade > 8;
		if (flag)
		{
			UI_GMWindow.Instance.Log(LocalStringManager.Get("GM_Message_GMFunc_GetRandomCricket_Msg"));
		}
		CharacterDomainMethod.Call.GmCmd_GetCricket(0, 0, grade, (short)winsCount, (short)lossesCount);
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0005B49C File Offset: 0x0005969C
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryCricketPartIds(string partName = "")
	{
		string ret = "";
		foreach (CricketPartsItem config in ((IEnumerable<CricketPartsItem>)CricketParts.Instance))
		{
			bool flag = config.Name.Contains(partName);
			if (flag)
			{
				ret += string.Format("{0}-{1}  ", config.Name.SetColor(Color.green), config.TemplateId);
			}
			else
			{
				bool flag2 = config.NameAtSecond.Contains(partName);
				if (flag2)
				{
					ret += string.Format("Sec-{0}-{1}  ", config.NameAtSecond.SetColor(Color.green), config.TemplateId);
				}
			}
		}
		UI_GMWindow.Instance.Log(ret);
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0005B57C File Offset: 0x0005977C
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	public static void GetCricketPolymorph(int templateId, bool male)
	{
		TaiwuDomainMethod.Call.GmCmd_GenerateCricketPolymorph((short)templateId, male ? 1 : 0);
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0005B58E File Offset: 0x0005978E
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	public static void DisplayCricketPreview()
	{
		UI_GMWindow.Instance.CricketPreview.SetActive(true);
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0005B5A4 File Offset: 0x000597A4
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	public static void ShowCricketPolymorphEffect(int charId)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CricketItemId", -1);
		argBox.Set("ColorId", Random.Range(0, CricketParts.Instance.Count));
		argBox.Set("PartId", Random.Range(0, CricketParts.Instance.Count));
		argBox.Set("CharId", charId);
		UIElement.CricketPolymorphEffect.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CricketPolymorphEffect, true);
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0005B627 File Offset: 0x00059827
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	public static void TaiwuCrossArchive()
	{
		TaiwuEventDomainMethod.Call.GmCmd_TaiwuCrossArchive();
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0005B630 File Offset: 0x00059830
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	public static void TravelToPastTaiwuVillage()
	{
		TaiwuEventDomainMethod.Call.GmCmd_TravelToPastTaiwuVillage();
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0005B639 File Offset: 0x00059839
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	public static void BackFromPastTaiwuVillage()
	{
		TaiwuEventDomainMethod.Call.GmCmd_BackFromPastTaiwuVillage();
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0005B642 File Offset: 0x00059842
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	public static void TaiwuWantedSectPunished(sbyte settlementId, sbyte severity)
	{
		TaiwuEventDomainMethod.Call.GmCmd_TaiwuWantedSectPunished(settlementId, severity);
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0005B64D File Offset: 0x0005984D
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.1f)]
	public static void GetFriendOrFamilySendGift(int charId)
	{
		ExtraDomainMethod.AsyncCall.GM_GetFriendOrFamilySendGift(null, charId, delegate(int offset, RawDataPool pool)
		{
			ItemKey itemKey = ItemKey.Invalid;
			Serializer.Deserialize(pool, offset, ref itemKey);
			string str = itemKey.HasTemplate ? itemKey.ToString() : "no item or resource, but exp";
			UI_GMWindow.Instance.Log(str);
		});
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x0005B678 File Offset: 0x00059878
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(2, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(3, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(4, EWidgetType.ItemIdField, 0.12f)]
	public static void ChangeItemDurability(int charId, short change, sbyte itemType, short idStar, short? idEnd)
	{
		bool flag = idEnd == null;
		if (flag)
		{
			idEnd = new short?(idStar);
		}
		ItemDomainMethod.Call.ChangeDurability(charId, change, itemType, idStar, idEnd.GetValueOrDefault());
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0005B6AD File Offset: 0x000598AD
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ChangeMysteryCompatibility(int deltaValue)
	{
		ItemDomainMethod.Call.GmCmd_ChangeAllMysteryCompatibility(deltaValue);
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0005B6B7 File Offset: 0x000598B7
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ChangeCricketSpirit(int addValue)
	{
		ItemDomainMethod.Call.GmCmd_ChangeAllCricketSpirit(addValue);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0005B6C1 File Offset: 0x000598C1
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.1f)]
	public static void ChangePoisonIdentified(int charId, bool isIdentified)
	{
		ItemDomainMethod.Call.ChangePoisonIdentified(charId, isIdentified);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x0005B6CC File Offset: 0x000598CC
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(2, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(3, EWidgetType.PoisonItemIdField, 0.12f)]
	[GMFuncArg(4, EWidgetType.PoisonItemIdField, 0.12f)]
	[GMFuncArg(5, EWidgetType.PoisonItemIdField, 0.12f)]
	public static void AddPoisonedInventoryItem(int charId, sbyte baseItemType, short baseItemId, short poisonId1 = -1, short poisonId2 = -1, short poisonId3 = -1)
	{
		bool flag = !ItemTemplateHelper.CheckTemplateValid(baseItemType, baseItemId);
		if (flag)
		{
			UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", baseItemType, baseItemId));
		}
		else
		{
			bool flag2 = !ItemTemplateHelper.IsPoisonable(baseItemType, baseItemId);
			if (flag2)
			{
				UI_GMWindow.Instance.Log("InValid field : not a valid item to poison : " + ItemTemplateHelper.GetName(baseItemType, baseItemId));
			}
			else
			{
				bool flag3 = baseItemType != 7 && baseItemType != 8 && baseItemType != 9;
				if (flag3)
				{
					UI_GMWindow.Instance.Log("InValid field : not a valid item to eat : " + ItemTemplateHelper.GetName(baseItemType, baseItemId));
				}
				else
				{
					bool flag4 = poisonId1 >= 0;
					if (flag4)
					{
						bool flag5 = !ItemTemplateHelper.CheckTemplateValid(8, poisonId1);
						if (flag5)
						{
							UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", 8, poisonId1));
							return;
						}
						bool flag6 = Medicine.Instance[poisonId1].EffectType != EMedicineEffectType.ApplyPoison;
						if (flag6)
						{
							UI_GMWindow.Instance.Log("InValid field : not poison : " + ItemTemplateHelper.GetName(8, poisonId1));
							return;
						}
					}
					bool flag7 = poisonId2 >= 0;
					if (flag7)
					{
						bool flag8 = !ItemTemplateHelper.CheckTemplateValid(8, poisonId2);
						if (flag8)
						{
							UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", 8, poisonId2));
							return;
						}
						bool flag9 = Medicine.Instance[poisonId2].EffectType != EMedicineEffectType.ApplyPoison;
						if (flag9)
						{
							UI_GMWindow.Instance.Log("InValid field : not poison : " + ItemTemplateHelper.GetName(8, poisonId2));
							return;
						}
					}
					bool flag10 = poisonId3 >= 0;
					if (flag10)
					{
						bool flag11 = !ItemTemplateHelper.CheckTemplateValid(8, poisonId3);
						if (flag11)
						{
							UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", 8, poisonId3));
							return;
						}
						bool flag12 = Medicine.Instance[poisonId3].EffectType != EMedicineEffectType.ApplyPoison;
						if (flag12)
						{
							UI_GMWindow.Instance.Log("InValid field : not poison : " + ItemTemplateHelper.GetName(8, poisonId3));
							return;
						}
					}
					bool flag13 = poisonId1 >= 0;
					if (flag13)
					{
						bool flag14 = poisonId2 >= 0 && Medicine.Instance[poisonId1].PoisonType == Medicine.Instance[poisonId2].PoisonType;
						if (flag14)
						{
							UI_GMWindow.Instance.Log("InValid field : same poison type : " + ItemTemplateHelper.GetName(8, poisonId1) + " " + ItemTemplateHelper.GetName(8, poisonId2));
							return;
						}
						bool flag15 = poisonId3 >= 0 && Medicine.Instance[poisonId1].PoisonType == Medicine.Instance[poisonId3].PoisonType;
						if (flag15)
						{
							UI_GMWindow.Instance.Log("InValid field : same poison type : " + ItemTemplateHelper.GetName(8, poisonId1) + " " + ItemTemplateHelper.GetName(8, poisonId3));
							return;
						}
					}
					bool flag16 = poisonId2 >= 0;
					if (flag16)
					{
						bool flag17 = poisonId3 >= 0 && Medicine.Instance[poisonId2].PoisonType == Medicine.Instance[poisonId3].PoisonType;
						if (flag17)
						{
							UI_GMWindow.Instance.Log("InValid field : same poison type : " + ItemTemplateHelper.GetName(8, poisonId2) + " " + ItemTemplateHelper.GetName(8, poisonId3));
							return;
						}
					}
					CharacterDomainMethod.Call.GmCmd_AddPoisonedInventoryItem(charId, baseItemType, baseItemId, poisonId1, poisonId2, poisonId3);
				}
			}
		}
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x0005BA58 File Offset: 0x00059C58
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(2, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(3, EWidgetType.PoisonItemIdField, 0.12f)]
	[GMFuncArg(4, EWidgetType.PoisonItemIdField, 0.12f)]
	[GMFuncArg(5, EWidgetType.PoisonItemIdField, 0.12f)]
	public static void AddPoisonedEatingItem(int charId, sbyte baseItemType, short baseItemId, short poisonId1 = -1, short poisonId2 = -1, short poisonId3 = -1)
	{
		bool flag = !ItemTemplateHelper.CheckTemplateValid(baseItemType, baseItemId);
		if (flag)
		{
			UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", baseItemType, baseItemId));
		}
		else
		{
			bool flag2 = !ItemTemplateHelper.IsPoisonable(baseItemType, baseItemId);
			if (flag2)
			{
				UI_GMWindow.Instance.Log("InValid field : not a valid item to poison : " + ItemTemplateHelper.GetName(baseItemType, baseItemId));
			}
			else
			{
				bool flag3 = baseItemType != 7 && baseItemType != 8 && baseItemType != 9;
				if (flag3)
				{
					UI_GMWindow.Instance.Log("InValid field : not a valid item to eat : " + ItemTemplateHelper.GetName(baseItemType, baseItemId));
				}
				else
				{
					bool flag4 = poisonId1 >= 0;
					if (flag4)
					{
						bool flag5 = !ItemTemplateHelper.CheckTemplateValid(8, poisonId1);
						if (flag5)
						{
							UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", 8, poisonId1));
							return;
						}
						bool flag6 = Medicine.Instance[poisonId1].EffectType != EMedicineEffectType.ApplyPoison;
						if (flag6)
						{
							UI_GMWindow.Instance.Log("InValid field : not poison : " + ItemTemplateHelper.GetName(8, poisonId1));
							return;
						}
					}
					bool flag7 = poisonId2 >= 0;
					if (flag7)
					{
						bool flag8 = !ItemTemplateHelper.CheckTemplateValid(8, poisonId2);
						if (flag8)
						{
							UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", 8, poisonId2));
							return;
						}
						bool flag9 = Medicine.Instance[poisonId2].EffectType != EMedicineEffectType.ApplyPoison;
						if (flag9)
						{
							UI_GMWindow.Instance.Log("InValid field : not poison : " + ItemTemplateHelper.GetName(8, poisonId2));
							return;
						}
					}
					bool flag10 = poisonId3 >= 0;
					if (flag10)
					{
						bool flag11 = !ItemTemplateHelper.CheckTemplateValid(8, poisonId3);
						if (flag11)
						{
							UI_GMWindow.Instance.Log(string.Format("InValid field : not a valid item : {0}, {1}", 8, poisonId3));
							return;
						}
						bool flag12 = Medicine.Instance[poisonId3].EffectType != EMedicineEffectType.ApplyPoison;
						if (flag12)
						{
							UI_GMWindow.Instance.Log("InValid field : not poison : " + ItemTemplateHelper.GetName(8, poisonId3));
							return;
						}
					}
					bool flag13 = poisonId1 >= 0;
					if (flag13)
					{
						bool flag14 = poisonId2 >= 0 && Medicine.Instance[poisonId1].PoisonType == Medicine.Instance[poisonId2].PoisonType;
						if (flag14)
						{
							UI_GMWindow.Instance.Log("InValid field : same poison type : " + ItemTemplateHelper.GetName(8, poisonId1) + " " + ItemTemplateHelper.GetName(8, poisonId2));
							return;
						}
						bool flag15 = poisonId3 >= 0 && Medicine.Instance[poisonId1].PoisonType == Medicine.Instance[poisonId3].PoisonType;
						if (flag15)
						{
							UI_GMWindow.Instance.Log("InValid field : same poison type : " + ItemTemplateHelper.GetName(8, poisonId1) + " " + ItemTemplateHelper.GetName(8, poisonId3));
							return;
						}
					}
					bool flag16 = poisonId2 >= 0;
					if (flag16)
					{
						bool flag17 = poisonId3 >= 0 && Medicine.Instance[poisonId2].PoisonType == Medicine.Instance[poisonId3].PoisonType;
						if (flag17)
						{
							UI_GMWindow.Instance.Log("InValid field : same poison type : " + ItemTemplateHelper.GetName(8, poisonId2) + " " + ItemTemplateHelper.GetName(8, poisonId3));
							return;
						}
					}
					CharacterDomainMethod.Call.GmCmd_AddPoisonedEatingItem(charId, baseItemType, baseItemId, poisonId1, poisonId2, poisonId3);
				}
			}
		}
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x0005BDE4 File Offset: 0x00059FE4
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(2, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(3, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(4, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(5, EWidgetType.IntField, 0.12f)]
	public static void MerchantGetItem(int charId, int count, sbyte itemType, short idStar, short? idEnd, int? level)
	{
		bool flag = level == null;
		if (flag)
		{
			level = new int?(0);
		}
		bool flag2 = idEnd != null;
		if (flag2)
		{
			idEnd = new short?(Math.Max(idStar, idEnd.Value));
			short itemId = idStar;
			for (;;)
			{
				int num = (int)itemId;
				short? num2 = idEnd;
				int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
				if (!(num <= num3.GetValueOrDefault() & num3 != null))
				{
					break;
				}
				MerchantDomainMethod.Call.GmCmd_AddItem(charId, itemType, itemId, count, level.Value);
				itemId += 1;
			}
		}
		else
		{
			MerchantDomainMethod.Call.GmCmd_AddItem(charId, itemType, idStar, count, level.Value);
		}
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x0005BE98 File Offset: 0x0005A098
	[GMFunc(EGMGroup.CharacterItem, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.12f)]
	public static void VitalInfectionInOut(int charId, int type, int value)
	{
		ExtraDomainMethod.Call.GmCmd_VitalInfectionInOut(charId, type, value);
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0005BEA4 File Offset: 0x0005A0A4
	[GMFunc(EGMGroup.CharacterInformation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.2f)]
	[GMFuncArg(1, EWidgetType.StringField, 0.4f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.1f)]
	public static void CreateSecretInformationByCharacterIds(string templateDefKeyName, string charIds, bool pub)
	{
		List<int> charIdList = (from p in charIds.Split(' ', StringSplitOptions.None)
		select Convert.ToInt32(p)).ToList<int>();
		InformationDomainMethod.AsyncCall.GmCmd_CreateSecretInformationByCharacterIds(null, templateDefKeyName, charIdList, delegate(int offset, RawDataPool pool)
		{
			int metaDataId = -1;
			Serializer.Deserialize(pool, offset, ref metaDataId);
			UI_GMWindow.Instance.Log(string.Format("{0} = {1}", "metaDataId", metaDataId));
			bool flag = pub && metaDataId >= 0;
			if (flag)
			{
				GMFunc.MakeSecretInformationBroadcast(metaDataId, (charIdList.Count > 0) ? charIdList[0] : -1);
			}
		});
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0005BF11 File Offset: 0x0005A111
	[GMFunc(EGMGroup.CharacterInformation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	public static void MakeCharacterReceiveSecretInformation(int secretId, int characterId)
	{
		InformationDomainMethod.Call.GmCmd_MakeCharacterReceiveSecretInformation(-1, characterId, (SecretInformationId)secretId);
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x0005BF22 File Offset: 0x0005A122
	[GMFunc(EGMGroup.CharacterInformation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	public static void MakeSecretInformationBroadcast(int secretId, int sourceCharId)
	{
		InformationDomainMethod.Call.GmCmd_MakeSecretInformationBroadcast((SecretInformationId)secretId, sourceCharId);
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0005BF32 File Offset: 0x0005A132
	[GMFunc(EGMGroup.CharacterInformation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void DisseminationSecretInformationToRandomCharacters(int secretId, int sourceCharId, int amount)
	{
		InformationDomainMethod.AsyncCall.GmCmd_DisseminationSecretInformationToRandomCharacters(null, (SecretInformationId)secretId, sourceCharId, amount, delegate(int offset, RawDataPool pool)
		{
			int realAmount = 0;
			Serializer.Deserialize(pool, offset, ref realAmount);
			UI_GMWindow.Instance.Log(string.Format("{0} = {1}", "realAmount", realAmount));
		});
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x0005BF64 File Offset: 0x0005A164
	[GMFunc(EGMGroup.CharacterInformation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.4f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	public static void GiveNormalInformationToCharacter(string informationTarget, int characterId)
	{
		foreach (string text in informationTarget.Split(',', StringSplitOptions.None))
		{
			string[] unit = text.Trim().Split('-', StringSplitOptions.None);
			short templateId = Convert.ToInt16(unit[0]);
			sbyte level = Convert.ToSByte(unit[1]);
			InformationInfoItem info = InformationInfo.Instance[Config.Information.Instance[templateId].InfoIds[(int)level]];
			UI_GMWindow.Instance.Log(string.Format("{0}-{1} [{2} Grade: {3}]", new object[]
			{
				templateId,
				level,
				info.Name,
				info.Grade
			}));
			InformationDomainMethod.Call.AddNormalInformationToCharacter(characterId, new NormalInformation(templateId, level));
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x0005C034 File Offset: 0x0005A234
	[GMFunc(EGMGroup.CharacterInformation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.InformationTypeIdField, 0.4f)]
	public static void QueryNormalInformationTemplateId(sbyte type)
	{
		StringBuilder sb = new StringBuilder();
		foreach (InformationItem config in ((IEnumerable<InformationItem>)Config.Information.Instance))
		{
			bool flag = config == null || config.InfoIds == null || config.Type != type;
			if (!flag)
			{
				int level = 0;
				int len = config.InfoIds.Length;
				while (level < len)
				{
					short infoId = config.InfoIds[level];
					bool flag2 = infoId < 0;
					if (!flag2)
					{
						InformationInfoItem info = InformationInfo.Instance[infoId];
						sb.AppendLine(string.Format("{0}-{1} [{2} Grade: {3}]", new object[]
						{
							config.TemplateId,
							level,
							info.Name,
							info.Grade
						}));
					}
					level++;
				}
			}
		}
		UI_GMWindow.Instance.Log(sb.ToString());
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x0005C154 File Offset: 0x0005A354
	[GMFunc(EGMGroup.CharacterEvent, 0.6f, -1, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.TaskInfoIdField, 0.25f)]
	public static void AddExtraTask(int taskInfoId)
	{
		TaskInfoItem taskCfg = TaskInfo.Instance[taskInfoId];
		int taskChainId = -1;
		foreach (TaskChainItem taskChainConfig in ((IEnumerable<TaskChainItem>)TaskChain.Instance))
		{
			bool flag = taskChainConfig.TaskList.Contains(taskInfoId);
			if (flag)
			{
				taskChainId = taskChainConfig.TemplateId;
				break;
			}
		}
		bool flag2 = taskChainId == -1;
		if (flag2)
		{
			UI_GMWindow.Instance.Log("Task Adding Failed! taskChain not found");
		}
		else
		{
			bool flag3 = !SingletonObject.getInstance<TaskModel>().CheckRequiredTasksStatus(taskCfg.RequireFinishedTask, taskCfg.RequireUntriggeredTask);
			if (flag3)
			{
				UI_GMWindow.Instance.Log("Task Adding Failed! The task cannot be triggered in the current main story line!");
			}
			else
			{
				WorldDomainMethod.Call.GmCmd_AddExtraTask(taskChainId, taskInfoId);
			}
		}
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x0005C220 File Offset: 0x0005A420
	[GMFunc(EGMGroup.CharacterEvent, 0.6f, -1, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.TaskInfoIdField, 0.25f)]
	public static void RemoveExtraTask(int taskInfoId)
	{
		int taskChainId = -1;
		foreach (TaskChainItem taskChainConfig in ((IEnumerable<TaskChainItem>)TaskChain.Instance))
		{
			bool flag = taskChainConfig.TaskList.Contains(taskInfoId);
			if (flag)
			{
				taskChainId = taskChainConfig.TemplateId;
				break;
			}
		}
		bool flag2 = taskChainId == -1;
		if (flag2)
		{
			UI_GMWindow.Instance.Log("Task Adding Failed! taskChain not found");
		}
		else
		{
			WorldDomainMethod.Call.GmCmd_RemoveTriggeredExtraTask(taskChainId, taskInfoId);
		}
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0005C2AC File Offset: 0x0005A4AC
	// (set) Token: 0x06000EC0 RID: 3776 RVA: 0x0005C2B4 File Offset: 0x0005A4B4
	[GMProperty(EGMGroup.CharacterEvent, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool IgnoreEventBehavior
	{
		get
		{
			return EventModel.IgnoreEventBehavior;
		}
		set
		{
			EventModel.IgnoreEventBehavior = value;
			bool exist = UIElement.EventWindow.Exist;
			if (exist)
			{
				UIElement.EventWindow.UiBaseAs<UI_EventWindow>().Refresh();
			}
		}
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0005C2E8 File Offset: 0x0005A4E8
	[GMFunc(EGMGroup.CharacterEvent, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SearchMonthlyEventByName(string eventName)
	{
		eventName = eventName.Trim();
		List<MonthlyEventItem> list = (from m in MonthlyEvent.Instance
		where m.Name.Contains(eventName)
		select m).ToList<MonthlyEventItem>();
		bool flag = list == null || list.Count <= 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log("未找到名称含有" + eventName + "的过月事件");
		}
		else
		{
			StringBuilder sb = new StringBuilder();
			foreach (MonthlyEventItem monthlyEventItem in list)
			{
				sb.AppendLine(string.Format("{0} {1}", monthlyEventItem.Name, monthlyEventItem.TemplateId));
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		}
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0005C3E4 File Offset: 0x0005A5E4
	[GMFunc(EGMGroup.CharacterEvent, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SearchMonthlyEventById(short templateId)
	{
		MonthlyEventItem monthlyEventItem = MonthlyEvent.Instance.GetItem(templateId);
		bool flag = monthlyEventItem == null;
		if (flag)
		{
			UI_GMWindow.Instance.Log(string.Format("未找到ID为{0}的过月事件", templateId));
		}
		else
		{
			UI_GMWindow.Instance.Log(monthlyEventItem.Name);
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0005C434 File Offset: 0x0005A634
	[GMFunc(EGMGroup.CharacterEvent, 0.25f, 1000, null, GmRunMode.Default)]
	public static void AddMonthlyEvent(short templateId = -1, short endTemplateId = -1, int charId = -1, int targetCharId = -1)
	{
		WorldDomainMethod.AsyncCall.GmCmd_AddMonthlyEvent(null, templateId, endTemplateId, charId, targetCharId, delegate(int offset, RawDataPool pool)
		{
			bool success = false;
			Serializer.Deserialize(pool, offset, ref success);
			UI_GMWindow.Instance.Log(success ? "添加成功" : "添加失败，检查后端配置和代码是否同时存在");
		});
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0005C461 File Offset: 0x0005A661
	[GMFunc(EGMGroup.CharacterGroup, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void CreateGearMate(int templateId)
	{
		ExtraDomainMethod.Call.GmCmd_CreateGearMate(templateId, "");
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0005C470 File Offset: 0x0005A670
	[GMFunc(EGMGroup.CharacterGroup, 0.25f, 1000, null, GmRunMode.Default)]
	public static void CreateGearMateMale()
	{
		ExtraDomainMethod.Call.GmCmd_CreateGearMate(722, "");
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x0005C483 File Offset: 0x0005A683
	[GMFunc(EGMGroup.CharacterGroup, 0.25f, 1000, null, GmRunMode.Default)]
	public static void CreateGearMateFemale()
	{
		ExtraDomainMethod.Call.GmCmd_CreateGearMate(723, "");
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0005C498 File Offset: 0x0005A698
	[GMFunc(EGMGroup.CharacterGroup, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetTaiwuLocation()
	{
		Location taiwuLocation = SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
		UI_GMWindow.Instance.Log(string.Format("TaiwuLocation {0}", taiwuLocation));
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x0005C4CC File Offset: 0x0005A6CC
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.15f)]
	public static void GetFlagValue(sbyte flagType)
	{
		GlobalDomainMethod.AsyncCall.GetGlobalFlag(null, flagType, delegate(int offsetE, RawDataPool dataPoolE)
		{
			bool result = false;
			Serializer.Deserialize(dataPoolE, offsetE, ref result);
			UI_GMWindow.Instance.Log(string.Format("Flag({0}) = {1}", flagType, result));
		});
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x0005C500 File Offset: 0x0005A700
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.15f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.15f)]
	public static void SetFlagValue(sbyte flagType, bool flagValue)
	{
		GlobalDomainMethod.Call.SetGlobalFlag(flagType, flagValue);
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0005C50B File Offset: 0x0005A70B
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	public static void AddResetWorldSettingsChance()
	{
		WorldDomainMethod.Call.GmCmd_AddResetWorldSettingsChance();
		UI_GMWindow.Instance.Log("已添加重置世界设置次数");
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x0005C524 File Offset: 0x0005A724
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.15f)]
	public static void ShowUi(string elementName)
	{
		FieldInfo fieldInfo = typeof(UIElement).GetFields(BindingFlags.Static | BindingFlags.Public).FirstOrDefault((FieldInfo f) => f.FieldType == typeof(UIElement) && f.Name == elementName);
		UIElement element = ((fieldInfo != null) ? fieldInfo.GetValue(null) : null) as UIElement;
		bool flag = element != null;
		if (flag)
		{
			UIManager.Instance.ShowUI(element, true);
		}
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0005C58C File Offset: 0x0005A78C
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.15f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.15f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.15f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.15f)]
	[GMFuncArg(4, EWidgetType.BoolField, 0.15f)]
	public static void QuickDeployGame(bool disableMove, bool disableItem, bool disableSkill, bool disableResource, bool disableSwordTomb)
	{
		BasicGameData basic = SingletonObject.getInstance<BasicGameData>();
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(null, new List<int>
		{
			basic.TaiwuCharId
		}, delegate(int offsetE, RawDataPool dataPoolE)
		{
			List<CharacterDisplayData> taiWuData = new List<CharacterDisplayData>();
			Serializer.Deserialize(dataPoolE, offsetE, ref taiWuData);
			bool flag = taiWuData != null && taiWuData.Count > 0 && taiWuData[0].Location.IsValid() && mapModel.Areas[(int)taiWuData[0].Location.AreaId].GetConfig().TemplateId == 0;
			if (flag)
			{
				GMFunc.OpenAllWorldFunction();
				GMFunc.EditMainlineProgress(8);
				GMFunc.SetCharacterOrganization(basic.TaiwuCharId, 16);
				bool flag2 = !disableItem;
				if (flag2)
				{
					int amount = 10;
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 12, 18, 200);
					for (int type = 0; type < 13; type++)
					{
						bool flag3 = type == 11 || type == 0 || type == 1 || type == 2 || type == 4 || type == 12 || type == -1;
						if (!flag3)
						{
							for (int templateId = 0; templateId < amount; templateId++)
							{
								CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, (sbyte)type, (short)templateId, 1);
							}
						}
					}
				}
				bool flag4 = !disableSwordTomb;
				if (flag4)
				{
					Location[] locations = mapModel.SwordTombLocations;
					int i = 0;
					int len = locations.Length;
					while (i < len)
					{
						MapDomainMethod.Call.ChangeBlockTemplate(-1, locations[i], (short)(128 + i), true);
						i++;
					}
				}
				DataUid learnedUid = new DataUid(4, 0, (ulong)((long)basic.TaiwuCharId), 59U);
				UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
				{
					bool flag9 = !disableSkill;
					if (flag9)
					{
						List<short> learned = (List<short>)data[learnedUid];
						int amount2 = 10;
						ushort readingState = ushort.MaxValue;
						short templateId2 = 0;
						while ((int)templateId2 < amount2)
						{
							bool flag10 = !learned.Contains(templateId2);
							if (flag10)
							{
								CharacterDomainMethod.Call.LearnCombatSkill(basic.TaiwuCharId, templateId2, readingState);
							}
							templateId2 += 1;
						}
					}
				}, new ValueTuple<DataUid, Type>[]
				{
					new ValueTuple<DataUid, Type>(learnedUid, typeof(List<short>))
				});
				bool flag5 = !disableItem;
				if (flag5)
				{
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 27, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 28, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 29, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 30, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 31, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 32, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 33, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 34, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 35, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 18, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 19, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 20, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 21, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 22, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 23, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 24, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 25, 1);
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 9, 26, 1);
				}
				bool flag6 = !disableResource;
				if (flag6)
				{
					int count = 1048576;
					for (sbyte type2 = -1; type2 < 8; type2 += 1)
					{
						bool flag7 = type2 == -1;
						if (!flag7)
						{
							TaiwuDomainMethod.Call.GmCmd_AddResource(type2, count);
						}
					}
				}
				short id = 26;
				bool flag8 = !disableItem;
				if (flag8)
				{
					CharacterDomainMethod.Call.CreateInventoryItem(basic.TaiwuCharId, 4, id, 1);
				}
				CharacterDomainMethod.AsyncCall.GetInventoryItems(null, basic.TaiwuCharId, Carrier.Instance[id].ItemSubType, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> items = new List<ItemDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref items);
					bool flag9 = !disableItem;
					if (flag9)
					{
						foreach (ItemDisplayData item in items)
						{
							bool flag10 = item.Key.TemplateId == id;
							if (flag10)
							{
								CharacterDomainMethod.Call.ChangeEquipment(basic.TaiwuCharId, -1, 11, item.Key);
								break;
							}
						}
					}
					bool flag11 = !disableMove;
					if (flag11)
					{
						Location taiWuLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
						ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
						worldMap.StartCoroutine(worldMap.QuickTravel(taiWuLocation.AreaId, null));
					}
				});
			}
		});
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0005C604 File Offset: 0x0005A804
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GotoSecretVillage()
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		GMFunc.IntraStateTravel = true;
		GMFunc.InterStateTravel = true;
		ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
		worldMap.StartCoroutine(worldMap.QuickTravel(137, null));
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0005C648 File Offset: 0x0005A848
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GotoBrokenPerform()
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		GMFunc.IntraStateTravel = true;
		GMFunc.InterStateTravel = true;
		ViewWorldMap worldMap = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
		worldMap.StartCoroutine(worldMap.QuickTravel(138, null));
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0005C68A File Offset: 0x0005A88A
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void CmdGotoPathingBlock(short blockId)
	{
		GMFunc.GotoPathingBlock(blockId);
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0005C694 File Offset: 0x0005A894
	public static void GotoPathingBlock(short blockId)
	{
		bool flag = blockId < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log("地块ID无效");
		}
		else
		{
			UIElement.WorldMap.UiBaseAs<ViewWorldMap>().MoveToBlock(blockId);
		}
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0005C6D0 File Offset: 0x0005A8D0
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void GotoArea(short areaId)
	{
		bool flag = areaId < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log("区域ID无效");
		}
		else
		{
			MapDomainMethod.Call.QuickTravel(areaId);
		}
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0005C700 File Offset: 0x0005A900
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ShowLegacyActive()
	{
		UIManager.Instance.MaskUI(UIElement.LegacyActivate);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0005C713 File Offset: 0x0005A913
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void SetFuyuFaith(int value)
	{
		GameDataBridge.AddDataModification<int>(4, 39, ulong.MaxValue, uint.MaxValue, value);
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0005C724 File Offset: 0x0005A924
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void AddAnimal(short animalTemplateId)
	{
		bool flag = (animalTemplateId >= 228 && animalTemplateId <= 245) || (animalTemplateId >= 246 && animalTemplateId <= 295);
		if (flag)
		{
			MapDomainMethod.Call.GmCmd_AddAnimal(animalTemplateId);
		}
		else
		{
			UI_GMWindow.Instance.Log("Need input right animalTemplateId, Look Character.xlsx");
		}
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0005C77C File Offset: 0x0005A97C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.MapBlockIdField, 0.2f)]
	public static void ChangeCurrentBlockTemplate(short blockTemplateId)
	{
		EMapBlockType type = MapBlock.Instance[blockTemplateId].Type;
		bool flag = type == EMapBlockType.Sect || type == EMapBlockType.City || type == EMapBlockType.Town || type == EMapBlockType.Station;
		if (!flag)
		{
			Location currentLocation = SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
			MapDomainMethod.Call.ChangeBlockTemplate(-1, currentLocation, blockTemplateId, true);
		}
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0005C7D0 File Offset: 0x0005A9D0
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	public static void FindFiveLoongLocation()
	{
		MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
		ExtraDomainMethod.AsyncCall.GmCmd_FindFiveLoongLocation(DispatcherUtils.RegisterDispatcher(), delegate(int offset, RawDataPool dataPool)
		{
			List<LoongInfo> fiveLoongInfos = new List<LoongInfo>();
			Serializer.Deserialize(dataPool, offset, ref fiveLoongInfos);
			StringBuilder info = new StringBuilder();
			foreach (LoongInfo fiveLoongInfo in fiveLoongInfos)
			{
				info.Append(Character.Instance[fiveLoongInfo.CharacterTemplateId].GivenName + ":  AreaName:" + areas[(int)fiveLoongInfo.LoongTerrainCenterLocation.AreaId].GetConfig().Name + " \n");
			}
			UI_GMWindow.Instance.Log(info.ToString());
		});
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0005C80B File Offset: 0x0005AA0B
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GenerateTreasure()
	{
		ExtraDomainMethod.Call.GmCmd_GenerateTreasure();
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0005C814 File Offset: 0x0005AA14
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void GenerateWishingCricket(int wishingCricketId)
	{
		TaiwuDomainMethod.Call.GmCmd_GenerateCricketWishing((short)wishingCricketId);
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0005C81F File Offset: 0x0005AA1F
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	public static void TurnMapBlockIntoAshes()
	{
		MapDomainMethod.Call.GmCmd_TurnMapBlockIntoAshes();
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0005C828 File Offset: 0x0005AA28
	[GMFunc(EGMGroup.MapQuickOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void UpdateWeather(int weatherTemplateId)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.Set("WeatherTemplateId", (sbyte)weatherTemplateId);
		GEvent.OnEvent(UiEvents.GmUpdateWeather, args);
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x0005C85B File Offset: 0x0005AA5B
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BossCharIdField, 0.2f)]
	public static void CreateFixedCharacterAtCurrentBlock(int bossTemplateId)
	{
		MapDomainMethod.Call.GmCmd_CreateFixedCharacterAtCurrentBlock((short)bossTemplateId);
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x0005C866 File Offset: 0x0005AA66
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetTreasuryValueByTaiwuLocation()
	{
		MapDomainMethod.AsyncCall.GmCmd_GetTreasuryValueByTaiwuLocation(null, delegate(int offset, RawDataPool dataPool)
		{
			int value = 0;
			Serializer.Deserialize(dataPool, offset, ref value);
			UI_GMWindow.Instance.Log(value.ToString());
		});
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x0005C88F File Offset: 0x0005AA8F
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.TwelveImmortalsIndexField, 0.2f)]
	public static void FightTwelveImmortals(int twelveImmortalsIndex)
	{
		CombatDomainMethod.Call.GmCmd_FightTwelveImmortals(twelveImmortalsIndex);
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x0005C89C File Offset: 0x0005AA9C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BossCharIdField, 0.2f)]
	public static void FightWithBoss(int bossCharacterId)
	{
		short characterId = (short)bossCharacterId;
		bool flag = Character.Instance.GetItem(characterId) == null;
		if (flag)
		{
			Debug.LogError(string.Format("[GmError] no boss has template id {0}", characterId));
		}
		else
		{
			CombatDomainMethod.Call.GmCmd_FightBoss(characterId);
		}
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0005C8E0 File Offset: 0x0005AAE0
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.AnimalCharIdField, 0.2f)]
	public static void FightWithAnimal(int animalCharacterId)
	{
		short characterId = (short)animalCharacterId;
		bool flag = Character.Instance.GetItem(characterId) == null;
		if (flag)
		{
			Debug.LogError(string.Format("[GmError] no animal has template id {0}", characterId));
		}
		else
		{
			CombatDomainMethod.Call.GmCmd_FightAnimal(characterId);
		}
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0005C923 File Offset: 0x0005AB23
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.OrgMemberCharIdField, 0.3f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	public static void FightTestOrgMember(short testOrgMember, int testCount = 9)
	{
		CombatDomainMethod.Call.GmCmd_FightTestOrgMember(testOrgMember, testCount);
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0005C92E File Offset: 0x0005AB2E
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.RandomEnemyCharIdField, 0.3f)]
	[GMFuncArg(1, EWidgetType.CombatTypeField, 0.1f)]
	public static void FightRandomEnemy(short randomEnemyCharId, CombatType combatType)
	{
		CombatDomainMethod.Call.GmCmd_FightRandomEnemy(randomEnemyCharId, (sbyte)combatType);
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0005C939 File Offset: 0x0005AB39
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.RandomEnemyCharIdField, 0.3f)]
	public static void AddRandomEnemyOnMap(short randomEnemyCharId)
	{
		MapDomainMethod.Call.GmCmd_AddRandomEnemyOnMap(randomEnemyCharId);
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0005C943 File Offset: 0x0005AB43
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QuestAllFactionInfos()
	{
		OrganizationDomainMethod.AsyncCall.GmCmd_GetAllFactionMembers(null, delegate(int offset, RawDataPool dataPool)
		{
			List<List<CharacterDisplayData>> data = new List<List<CharacterDisplayData>>();
			Serializer.Deserialize(dataPool, offset, ref data);
			StringBuilder sb = new StringBuilder();
			foreach (List<CharacterDisplayData> factionMembers in data)
			{
				CharacterDisplayData leaderData = factionMembers[0];
				factionMembers.RemoveAt(0);
				sb.AppendFormat("{0}[{1}][{2}]\n", CommonUtils.GetOrganizationGradeString(leaderData.OrgInfo, leaderData.Gender, leaderData.PhysiologicalAge, -1), NameCenter.GetNameByDisplayData(leaderData, false, true), leaderData.CharacterId);
				int i = 0;
				int len = factionMembers.Count;
				while (i < len)
				{
					CharacterDisplayData memberData = factionMembers[i];
					sb.AppendFormat("\t- {0}[{1}]\n", NameCenter.GetNameByDisplayData(memberData, false, true), memberData.CharacterId);
					i++;
				}
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		});
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x0005C96C File Offset: 0x0005AB6C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QuestAllGroupInfos()
	{
		CharacterDomainMethod.AsyncCall.GmCmd_GetAllGroupMembers(null, delegate(int offset, RawDataPool dataPool)
		{
			List<List<CharacterDisplayData>> data = new List<List<CharacterDisplayData>>();
			Serializer.Deserialize(dataPool, offset, ref data);
			StringBuilder sb = new StringBuilder();
			foreach (List<CharacterDisplayData> groupMembers in data)
			{
				CharacterDisplayData leaderData = groupMembers[0];
				groupMembers.RemoveAt(0);
				sb.AppendFormat("{0}[{1}][{2}]\n", CommonUtils.GetOrganizationGradeString(leaderData.OrgInfo, leaderData.Gender, leaderData.PhysiologicalAge, -1), NameCenter.GetNameByDisplayData(leaderData, false, true), leaderData.CharacterId);
				int i = 0;
				int len = groupMembers.Count;
				while (i < len)
				{
					CharacterDisplayData memberData = groupMembers[i];
					sb.AppendFormat("\t- {0}[{1}]\n", NameCenter.GetNameByDisplayData(memberData, false, true), memberData.CharacterId);
					i++;
				}
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		});
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0005C998 File Offset: 0x0005AB98
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LegacyTemplateIdField, 0.4f)]
	public static void AddLegacy(short templateId)
	{
		DataUid uid = new DataUid(5, 36, ulong.MaxValue, uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			List<short> availableLegacyList = (List<short>)data[uid];
			availableLegacyList.Add(templateId);
			GameDataBridge.AddDataModification<List<short>>(uid.DomainId, uid.DataId, uid.SubId0, uid.SubId1, availableLegacyList);
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(List<short>))
		});
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0005C9FD File Offset: 0x0005ABFD
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LegacyPointTemplateIdField, 0.4f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	public static void AddLegacyPoint(short template, int percent)
	{
		TaiwuDomainMethod.Call.GmCmd_AddLegacyPoint(template, percent);
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x0005CA08 File Offset: 0x0005AC08
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void FillLegacyPoints()
	{
		foreach (LegacyPointItem template in ((IEnumerable<LegacyPointItem>)LegacyPoint.Instance))
		{
			TaiwuDomainMethod.Call.GmCmd_FillLegacyPoint(template.TemplateId);
		}
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0005CA5C File Offset: 0x0005AC5C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.TravelEventIdField, 0.5f)]
	public static void TriggerTravelingEvent(short templateId)
	{
		MapDomainMethod.AsyncCall.GmCmd_TriggerTravelingEvent(null, templateId, delegate(int offset, RawDataPool pool)
		{
		});
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0005CA86 File Offset: 0x0005AC86
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void DisplayAllLegendaryBookStates()
	{
		LegendaryBookDomainMethod.AsyncCall.GmCmd_GetAllLegendaryBookStates(null, delegate(int offset, RawDataPool dataPool)
		{
			List<IntPair> data = new List<IntPair>();
			Serializer.Deserialize(dataPool, offset, ref data);
			StringBuilder sb = new StringBuilder();
			sb.Append("Legendary book owners:");
			sb.AppendLine();
			for (int i = 0; i < 14; i++)
			{
				bool flag = data[i].First >= 0;
				if (flag)
				{
					sb.Append(Misc.Instance[i + 240].Name);
					sb.Append(" owner: ");
					sb.Append(data[i].First);
					sb.AppendLine();
				}
				else
				{
					bool flag2 = data[i].Second >= 0;
					if (flag2)
					{
						sb.Append(Misc.Instance[i + 240].Name);
						sb.Append(" adventure area: ");
						sb.Append(data[i].Second);
						sb.AppendLine();
					}
				}
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		});
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0005CAAF File Offset: 0x0005ACAF
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void AddRandomLegendaryBookContestChar()
	{
		LegendaryBookDomainMethod.Call.GmCmd_AddRandomLegendaryBookContestChar();
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0005CAB8 File Offset: 0x0005ACB8
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetArchiveEnabledMods()
	{
		sbyte index = SingletonObject.getInstance<GlobalSettings>().LastEnterWorldIndex;
		ArchiveInfo archiveInfo = GlobalOperations.ArchivesInfo[(int)index];
		List<ModId> modIds = archiveInfo.WorldInfo.ModIds;
		List<ModId> steamModIds = modIds.FindAll((ModId modId) => modId.Source == 1);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Total Non-steam Mod Count: {0}", modIds.Count - steamModIds.Count));
		stringBuilder.AppendLine();
		GMFunc.GetModNames(steamModIds, delegate(string[] modNames)
		{
			stringBuilder.AppendLine(string.Format("Total Steam Mod Count: {0}", steamModIds.Count));
			for (int i = 0; i < steamModIds.Count; i++)
			{
				stringBuilder.AppendLine(string.Format("{0}. {1} ({2})", i + 1, modNames[i], steamModIds[i].FileId));
			}
			UI_GMWindow.Instance.Log(stringBuilder.ToString());
		});
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0005CB74 File Offset: 0x0005AD74
	private static void GetModNames(List<ModId> modIds, Action<string[]> onFinish)
	{
		string[] result = new string[modIds.Count];
		PublishedFileId_t[] publishedFileIds = new PublishedFileId_t[modIds.Count];
		for (int i = 0; i < publishedFileIds.Length; i++)
		{
			publishedFileIds[i] = new PublishedFileId_t(modIds[i].FileId);
		}
		CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create(null);
		UGCQueryHandle_t ugcHandle = SteamUGC.CreateQueryUGCDetailsRequest(publishedFileIds, (uint)publishedFileIds.Length);
		SteamUGC.SetReturnChildren(ugcHandle, true);
		SteamAPICall_t steamAPICall = SteamUGC.SendQueryUGCRequest(ugcHandle);
		callResult.Set(steamAPICall, delegate(SteamUGCQueryCompleted_t t, bool failure)
		{
			if (failure)
			{
				AdaptableLog.Warning(string.Format("Get ugc details failed with result {0}.", t.m_eResult), false);
				Action<string[]> onFinish2 = onFinish;
				if (onFinish2 != null)
				{
					onFinish2(result);
				}
			}
			else
			{
				for (uint j = 0U; j < t.m_unNumResultsReturned; j += 1U)
				{
					SteamUGCDetails_t details;
					SteamUGC.GetQueryUGCResult(ugcHandle, j, out details);
					int index = modIds.FindIndex((ModId d) => d.FileId == details.m_nPublishedFileId.m_PublishedFileId);
					bool flag = index == -1;
					if (flag)
					{
						AdaptableLog.Warning(string.Format("Unrecognized mod id {0}", details.m_nPublishedFileId), false);
					}
					else
					{
						result[index] = details.m_rgchTitle;
					}
				}
				AdaptableLog.Warning(string.Format("Get ugc details succeed with result {0} and {1} results returned.", t.m_eResult, t.m_unNumResultsReturned), false);
				Action<string[]> onFinish3 = onFinish;
				if (onFinish3 != null)
				{
					onFinish3(result);
				}
				SteamUGC.ReleaseQueryUGCRequest(ugcHandle);
			}
		});
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0005CC37 File Offset: 0x0005AE37
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void UnlockAllSettlementInformation()
	{
		OrganizationDomainMethod.Call.GmCmd_SetAllSettlementInformationVisited();
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0005CC40 File Offset: 0x0005AE40
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.SectIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	public static void SetAllSettlementMemberApprovedTaiwu(int sectIndex, bool approvedTaiwu = true)
	{
		sbyte orgTemplateId = (sbyte)(1 + sectIndex);
		OrganizationDomainMethod.Call.GmCmd_SetAllSettlementMemberApprovedTaiwu(orgTemplateId, approvedTaiwu);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x0005CC5B File Offset: 0x0005AE5B
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void GetSettlementPrisoner(int level)
	{
		OrganizationDomainMethod.AsyncCall.GmCmd_GetSettlementPrisoner(null, level, delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayData> prisoners = new List<CharacterDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref prisoners);
			bool flag = prisoners != null;
			if (flag)
			{
				UI_GMWindow instance = UI_GMWindow.Instance;
				if (instance != null)
				{
					instance.Log(string.Format("got prisoners x{0}:", prisoners.Count));
				}
				foreach (CharacterDisplayData prisoner in prisoners)
				{
					UI_GMWindow instance2 = UI_GMWindow.Instance;
					if (instance2 != null)
					{
						instance2.Log(string.Format("  {0}: {1}", prisoner.CharacterId, NameCenter.GetNameByDisplayData(prisoner, false, true)));
					}
				}
			}
			else
			{
				UI_GMWindow instance3 = UI_GMWindow.Instance;
				if (instance3 != null)
				{
					instance3.Log("got null, Taiwu might not in any settlement.");
				}
			}
		});
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x0005CC85 File Offset: 0x0005AE85
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void UnlockAllStation()
	{
		MapDomainMethod.Call.GmCmd_UnlockAllStation();
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0005CC8E File Offset: 0x0005AE8E
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ShowAllMapBlocks()
	{
		MapDomainMethod.Call.GmCmd_ShowAllMapBlock();
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0005CC97 File Offset: 0x0005AE97
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void HideAllMapBlocks()
	{
		MapDomainMethod.Call.GmCmd_HideAllMapBlock();
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0005CCA0 File Offset: 0x0005AEA0
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void Save()
	{
		DialogCmd forbidNoticeCmd = new DialogCmd
		{
			Title = LocalStringManager.Get(LanguageKey.LK_GameName),
			Content = LocalStringManager.Get(LanguageKey.LK_Notice_DoNotSaveDuringGameMonth),
			Type = 2
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", forbidNoticeCmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0005CD08 File Offset: 0x0005AF08
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void AdvanceManyMonths(int monthCount, int saveSpacing)
	{
		bool flag = GMFunc.AdvanceMonthCoroutine != null;
		if (!flag)
		{
			bool flag2 = UIManager.Instance.IsInStack(UIElement.Combat) || UIElement.Combat.Exist;
			if (!flag2)
			{
				SingletonObject.getInstance<YieldHelper>().StartCoroutine(GMFunc.AdvanceMonthCoroutine = GMFunc.QuickAdvance(monthCount, saveSpacing));
			}
		}
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0005CD60 File Offset: 0x0005AF60
	private static IEnumerator QuickAdvance(int times, int saveSpacing)
	{
		WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
		BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
		int advancedTimes = 0;
		bool disableSavingOrigin = GMFunc.DisableAutoSaving;
		Func<bool> <>9__0;
		Func<bool> <>9__1;
		Func<bool> <>9__5;
		while (advancedTimes < times)
		{
			GMFunc.DisableAutoSaving = (advancedTimes < times - 1 && (saveSpacing <= 0 || advancedTimes % saveSpacing != saveSpacing - 1));
			int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			bool flag = leftDays > 0;
			if (flag)
			{
				WorldDomainMethod.Call.AdvanceDaysInMonth(leftDays);
				yield return waitFrame;
			}
			int num = advancedTimes;
			advancedTimes = num + 1;
			bool flag2 = basicGameData.AdvancingMonthState != 0;
			if (flag2)
			{
				Debug.LogWarning(string.Format("assert failed: unexpected month state: {0}", basicGameData.AdvancingMonthState));
			}
			WorldDomainMethod.Call.AdvanceMonth();
			GameApp.AdvancingMonth = true;
			Func<bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = (() => basicGameData.AdvancingMonthState != 0));
			}
			yield return new WaitUntil(predicate);
			yield return waitFrame;
			Func<bool> predicate2;
			if ((predicate2 = <>9__1) == null)
			{
				predicate2 = (<>9__1 = (() => basicGameData.AdvancingMonthState == 14));
			}
			yield return new WaitUntil(predicate2);
			yield return waitFrame;
			yield return new WaitUntil(() => UIElement.MonthNotify.IsInState(EUiElementState.Ready));
			yield return waitFrame;
			bool flag3 = !GMFunc.DisableAutoSaving;
			if (flag3)
			{
				yield return new WaitUntil(() => SingletonObject.getInstance<BasicGameData>().SavingWorld);
				yield return waitFrame;
				yield return new WaitUntil(() => !SingletonObject.getInstance<BasicGameData>().SavingWorld);
				yield return waitFrame;
			}
			UIManager.Instance.HideUI(UIElement.MonthNotify);
			Func<bool> predicate3;
			if ((predicate3 = <>9__5) == null)
			{
				predicate3 = (<>9__5 = (() => basicGameData.AdvancingMonthState == 0));
			}
			yield return new WaitUntil(predicate3);
			yield return waitFrame;
		}
		GMFunc.DisableAutoSaving = disableSavingOrigin;
		GMFunc.AdvanceMonthCoroutine = null;
		yield break;
	}

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0005CD76 File Offset: 0x0005AF76
	// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x0005CD80 File Offset: 0x0005AF80
	[GMProperty(EGMGroup.MapBase, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool LockTime
	{
		get
		{
			return GMFunc._lockTime;
		}
		set
		{
			GMFunc._lockTime = value;
			MapDomainMethod.Call.GmCmd_SetLockTime(GMFunc._lockTime);
			bool flag = !GMFunc._lockTime;
			if (flag)
			{
				GMFunc.TeleportMove = false;
			}
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x0005CDB2 File Offset: 0x0005AFB2
	// (set) Token: 0x06000EF9 RID: 3833 RVA: 0x0005CDB9 File Offset: 0x0005AFB9
	[GMProperty(EGMGroup.MapBase, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool AvoidTravelEvent { get; set; }

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0005CDC1 File Offset: 0x0005AFC1
	// (set) Token: 0x06000EFB RID: 3835 RVA: 0x0005CDC8 File Offset: 0x0005AFC8
	[GMProperty(EGMGroup.MapBase, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool TeleportMove
	{
		get
		{
			return GMFunc._teleportMove;
		}
		set
		{
			GMFunc._teleportMove = value;
			MapDomainMethod.Call.GmCmd_SetTeleportMove(GMFunc._teleportMove);
			bool teleportMove = GMFunc._teleportMove;
			if (teleportMove)
			{
				GMFunc.LockTime = true;
			}
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000EFC RID: 3836 RVA: 0x0005CDF7 File Offset: 0x0005AFF7
	// (set) Token: 0x06000EFD RID: 3837 RVA: 0x0005CE06 File Offset: 0x0005B006
	[GMProperty(EGMGroup.MapBase, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool SkipMainStoryLine
	{
		get
		{
			return PlayerPrefs.GetInt("SkipMainStoryLine") == 1;
		}
		set
		{
			PlayerPrefs.SetInt("SkipMainStoryLine", value ? 1 : 0);
		}
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0005CE1A File Offset: 0x0005B01A
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void TeleportToBlock(int blockId)
	{
		GMFunc.TeleportMove = true;
		MapDomainMethod.Call.Move((short)blockId);
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0005CE2C File Offset: 0x0005B02C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.4f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void SetGlobalArgBoxInt(string key, int value)
	{
		TaiwuEventDomainMethod.Call.GmCmd_SetGlobalArgBoxInt(key, value);
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x0005CE38 File Offset: 0x0005B038
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.4f)]
	public static void GetGlobalArgBoxInt(string key)
	{
		TaiwuEventDomainMethod.AsyncCall.GmCmd_GetGlobalArgBoxInt(null, key, delegate(int offset, RawDataPool dataPool)
		{
			GlobalArgValue value = default(GlobalArgValue);
			Serializer.Deserialize(dataPool, offset, ref value);
			bool val = value.Val1;
			if (val)
			{
				UI_GMWindow.Instance.Log(value.Val2.ToString());
			}
			else
			{
				UI_GMWindow.Instance.Log("No value with key " + key + ".");
			}
		});
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x0005CE6C File Offset: 0x0005B06C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void KillTaiWuVillagers()
	{
		int taiWuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		OrganizationDomainMethod.AsyncCall.GetSettlementMembers(null, SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId(), delegate(int offset, RawDataPool dataPool)
		{
			List<CharacterDisplayData> villagers = new List<CharacterDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref villagers);
			foreach (CharacterDisplayData character in villagers)
			{
				int charId = character.CharacterId;
				bool flag = charId == taiWuId;
				if (!flag)
				{
					CharacterDomainMethod.Call.GmCmd_Die(charId);
				}
			}
		});
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x0005CEB0 File Offset: 0x0005B0B0
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryXiangshuLevel()
	{
		DataUid dataId1 = new DataUid(1, 1, ulong.MaxValue, uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			sbyte xiangshuProgress = (sbyte)data[dataId1];
			UI_GMWindow.Instance.Log(string.Format("{0}", GameData.Domains.World.SharedMethods.GetXiangshuLevel(xiangshuProgress)));
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(dataId1, typeof(sbyte))
		});
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x0005CF10 File Offset: 0x0005B110
	[GMFunc(EGMGroup.MapAreaOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.2f)]
	public static void QueryAreaId(string areaName)
	{
		MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < areas.Length; i++)
		{
			MapAreaData area = areas[i];
			bool flag = area.GetConfig().Name.Equals(areaName);
			if (flag)
			{
				sb.AppendFormat("{0}：{1}\n", areaName, i);
			}
		}
		UI_GMWindow.Instance.Log(sb.ToString());
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0005CF84 File Offset: 0x0005B184
	[GMFunc(EGMGroup.MapAreaOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void QueryAreaName(int id)
	{
		MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
		bool flag = id >= 0 && id < areas.Length;
		if (flag)
		{
			UI_GMWindow.Instance.Log(areas[id].GetConfig().Name);
		}
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0005CFC8 File Offset: 0x0005B1C8
	[GMFunc(EGMGroup.MapAreaOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void ChangeSpiritualDebt(int areaId, int spiritualDebt)
	{
		MapDomainMethod.Call.GmCmd_ChangeSpiritualDebt((short)areaId, spiritualDebt);
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0005CFD4 File Offset: 0x0005B1D4
	[GMFunc(EGMGroup.MapAreaOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void QuerySpiritualDebt(int areaId)
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		bool flag = areaId >= 0 && areaId < mapModel.Areas.Length;
		if (flag)
		{
			MapAreaData area = mapModel.Areas[areaId];
			UI_GMWindow.Instance.Log(string.Format("{0}[{1}]: {2}", area.GetConfig().Name, areaId, mapModel.GetAreaSpiritualDebt((short)areaId)));
		}
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x0005D03C File Offset: 0x0005B23C
	[GMFunc(EGMGroup.MapAreaOperation, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ChangeAllSpiritualDebt(int spiritualDebt)
	{
		MapDomainMethod.Call.GmCmd_ChangeAllSpiritualDebt(spiritualDebt);
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0005D046 File Offset: 0x0005B246
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StoryProgressField, 0.5f)]
	public static void EditMainlineProgress(short value)
	{
		GameDataBridge.AddDataModification<short>(1, 4, ulong.MaxValue, uint.MaxValue, value);
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x0005D058 File Offset: 0x0005B258
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryMainlineProgress()
	{
		DataUid uid = new DataUid(1, 4, ulong.MaxValue, uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			Dictionary<short, string> table = UI_GMWindow.StoryProgress2Name;
			short value = (short)data[uid];
			bool flag = table.ContainsKey(value);
			if (flag)
			{
				UI_GMWindow.Instance.Log(table[value] ?? "");
			}
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(short))
		});
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x0005D0B5 File Offset: 0x0005B2B5
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.SectIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditStateTaskStatus(int id, int value)
	{
		GameDataBridge.AddDataModification<sbyte>(20, 0, (ulong)((long)id), uint.MaxValue, (sbyte)value);
		UI_GMWindow.Instance.Log(string.Format("{0}: {1}", id, value));
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0005D0E8 File Offset: 0x0005B2E8
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RandomSetAllStateTaskStatus()
	{
		foreach (OrganizationItem org in ((IEnumerable<OrganizationItem>)Organization.Instance))
		{
			bool isSect = org.IsSect;
			if (isSect)
			{
				ulong id = (ulong)((long)org.TemplateId - 1L);
				int value = Random.Range(1, 3);
				GameDataBridge.AddDataModification<sbyte>(1, 0, id, uint.MaxValue, (sbyte)value);
				UI_GMWindow.Instance.Log(string.Format("{0}, {1}的地区主线进度已被修改为: {2}", id, org.Name, value));
			}
		}
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0005D188 File Offset: 0x0005B388
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.XiangshuAvatarIdField, 0.2f)]
	public static void QueryXiangshuAvatarTaskStatus(sbyte id)
	{
		DataUid uid = new DataUid(1, 2, (ulong)((long)id), uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			XiangshuAvatarTaskStatus value = (XiangshuAvatarTaskStatus)data[uid];
			UI_GMWindow.Instance.Log(string.Format("剑冢状态: {0} 紫竹状态: {1} 紫竹角色 Id: {2}", value.SwordTombStatus, value.JuniorXiangshuTaskStatus, value.JuniorXiangshuCharId));
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(XiangshuAvatarTaskStatus))
		});
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x0005D1E8 File Offset: 0x0005B3E8
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetXiangshuAvatarTaskStatus(sbyte id, bool isGood)
	{
		DataUid uid = new DataUid(1, 2, (ulong)((long)id), uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			XiangshuAvatarTaskStatus value = (XiangshuAvatarTaskStatus)data[uid];
			value.JuniorXiangshuTaskStatus = (isGood ? 6 : 5);
			GameDataBridge.AddDataModification<XiangshuAvatarTaskStatus>(1, 2, (ulong)((long)id), uint.MaxValue, value);
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(XiangshuAvatarTaskStatus))
		});
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x0005D258 File Offset: 0x0005B458
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.XiangshuAvatarIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void EditXiangshuAvatarFavorability(sbyte id, short delta)
	{
		DataUid uid = new DataUid(1, 2, (ulong)((long)id), uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			XiangshuAvatarTaskStatus value = (XiangshuAvatarTaskStatus)data[uid];
			CharacterDomainMethod.Call.GmCmd_ChangeFavorability(value.JuniorXiangshuCharId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delta);
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(XiangshuAvatarTaskStatus))
		});
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x0005D2BC File Offset: 0x0005B4BC
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.XiangshuAvatarIdField, 0.2f)]
	public static void QueryXiangshuAvatarFavorability(sbyte id)
	{
		DataUid uid = new DataUid(1, 2, (ulong)((long)id), uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			XiangshuAvatarTaskStatus value = (XiangshuAvatarTaskStatus)data[uid];
			CharacterDomainMethod.AsyncCall.GetFavorability(null, value.JuniorXiangshuCharId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				short favorability = 0;
				Serializer.Deserialize(dataPool, offset, ref favorability);
				UI_GMWindow.Instance.Log(favorability.ToString());
			});
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(XiangshuAvatarTaskStatus))
		});
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x0005D31C File Offset: 0x0005B51C
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public unsafe static void QueryCurrentMapBlockData()
	{
		bool exist = UIElement.WorldMap.Exist;
		if (exist)
		{
			ViewWorldMap map = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			FieldInfo field = map.GetType().GetField("_curSelectBlock", BindingFlags.Instance | BindingFlags.NonPublic);
			MapBlockData currentBlockData = ((field != null) ? field.GetValue(map) : null) as MapBlockData;
			bool flag = currentBlockData != null;
			if (flag)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get("LK_MapBlockData_Malice"), currentBlockData.Malice));
				sb.Append(LocalStringManager.Get(LanguageKey.LK_Resource) + ": ");
				for (sbyte i = 0; i < 6; i += 1)
				{
					bool flag2 = i != 0;
					if (flag2)
					{
						sb.Append(", ");
					}
					sb.Append(string.Format("[{0}: {1} / {2}]", Config.ResourceType.Instance.GetItem(i).Name, *(ref currentBlockData.CurrResources.Items.FixedElementField + (IntPtr)i * 2), *(ref currentBlockData.MaxResources.Items.FixedElementField + (IntPtr)i * 2)));
				}
				sb.AppendLine();
				UI_GMWindow.Instance.Log(sb.ToString());
			}
		}
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0005D470 File Offset: 0x0005B670
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void EditCurrentMapBlockMalice(int value)
	{
		bool exist = UIElement.WorldMap.Exist;
		if (exist)
		{
			ViewWorldMap map = UIElement.WorldMap.UiBaseAs<ViewWorldMap>();
			MapBlockData currentBlockData = map.SelectedBlock;
			bool flag = currentBlockData != null;
			if (flag)
			{
				currentBlockData.Malice = (short)value;
				MapDomainMethod.Call.GmCmd_SetMapBlockData(currentBlockData);
			}
		}
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0005D4B9 File Offset: 0x0005B6B9
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void OpenAllWorldFunction()
	{
		GMFunc._lastWorldFunctionsStatuses = ulong.MaxValue;
		UI_GMWindow.Instance.SwitchWindow();
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0005D4D0 File Offset: 0x0005B6D0
	// (set) Token: 0x06000F14 RID: 3860 RVA: 0x0005D4F8 File Offset: 0x0005B6F8
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool IsFinalBossDefeated
	{
		get
		{
			bool flag = !GMFunc._lastWorldFunctionsStatusesLoaded;
			return !flag && GMFunc._isFinalBossDefeated;
		}
		set
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame || UI_GMWindow.Instance.IsGameDataReceiving();
			if (!flag)
			{
				GMFunc._isFinalBossDefeated = value;
				GameDataBridge.AddDataModification<bool>(1, 5, ulong.MaxValue, uint.MaxValue, value);
				UI_GMWindow.Instance.CloseWithoutSave();
			}
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x0005D544 File Offset: 0x0005B744
	private static bool _EditWorldFunctionsStatus(byte id)
	{
		bool flag = !GMFunc._lastWorldFunctionsStatusesLoaded;
		return !flag && WorldFunctionType.Get(GMFunc._lastWorldFunctionsStatuses, id);
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x0005D574 File Offset: 0x0005B774
	public static void RefreshWorldFunctionsStatus()
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
		if (!flag)
		{
			bool flag2 = !GMFunc._lastWorldFunctionsStatusesLoaded;
			if (flag2)
			{
				FunctionLockManager functionLockManager = SingletonObject.getInstance<FunctionLockManager>();
				GMFunc._lastWorldFunctionsStatuses = 0UL;
				for (byte i = 0; i < 64; i += 1)
				{
					bool flag3 = functionLockManager.IsFunctionUnlock(i);
					if (flag3)
					{
						GMFunc._lastWorldFunctionsStatuses |= 1UL << (int)i;
					}
				}
				GMFunc._lastWorldFunctionsStatusesLoaded = true;
			}
			DataUid dataId1 = new DataUid(1, 6, ulong.MaxValue, uint.MaxValue);
			DataUid dataId2 = new DataUid(1, 5, ulong.MaxValue, uint.MaxValue);
			UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
			{
				GMFunc._lastWorldFunctionsStatuses = (ulong)data[dataId1];
				GMFunc._isFinalBossDefeated = (bool)data[dataId2];
				UI_GMWindow.Instance.RefreshPage(1);
			}, new ValueTuple<DataUid, Type>[]
			{
				new ValueTuple<DataUid, Type>(dataId1, typeof(ulong)),
				new ValueTuple<DataUid, Type>(dataId2, typeof(bool))
			});
		}
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x0005D674 File Offset: 0x0005B874
	public static void ModifyWorldFunctionsStatus()
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
		if (!flag)
		{
			GameDataBridge.AddDataModification<ulong>(1, 6, ulong.MaxValue, uint.MaxValue, GMFunc._lastWorldFunctionsStatuses);
		}
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x0005D6AC File Offset: 0x0005B8AC
	private static bool _EditWorldFunctionsStatus(byte id, bool value)
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame || UI_GMWindow.Instance.IsGameDataReceiving();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			if (value)
			{
				GMFunc._lastWorldFunctionsStatuses = WorldFunctionType.Set(GMFunc._lastWorldFunctionsStatuses, id);
			}
			else
			{
				GMFunc._lastWorldFunctionsStatuses = WorldFunctionType.Reset(GMFunc._lastWorldFunctionsStatuses, id);
			}
			GameDataBridge.AddDataModification<ulong>(1, 6, ulong.MaxValue, uint.MaxValue, GMFunc._lastWorldFunctionsStatuses);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				UI_GMWindow.Instance.CloseWithoutSave();
			});
			result = value;
		}
		return result;
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06000F19 RID: 3865 RVA: 0x0005D744 File Offset: 0x0005B944
	// (set) Token: 0x06000F1A RID: 3866 RVA: 0x0005D74B File Offset: 0x0005B94B
	[GMProperty(EGMGroup.CharacterBase, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool EnableRandomGenealogyConnection { get; set; }

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06000F1B RID: 3867 RVA: 0x0005D753 File Offset: 0x0005B953
	// (set) Token: 0x06000F1C RID: 3868 RVA: 0x0005D765 File Offset: 0x0005B965
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static sbyte YuanShanExitWithStage
	{
		get
		{
			return (GMFunc._yuanShanExitWithStage < 4) ? GMFunc._yuanShanExitWithStage : -1;
		}
		set
		{
			GMFunc._yuanShanExitWithStage = value;
		}
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06000F1D RID: 3869 RVA: 0x0005D76D File Offset: 0x0005B96D
	// (set) Token: 0x06000F1E RID: 3870 RVA: 0x0005D776 File Offset: 0x0005B976
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool ShowChaishan
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(34);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(34, value);
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06000F1F RID: 3871 RVA: 0x0005D781 File Offset: 0x0005B981
	// (set) Token: 0x06000F20 RID: 3872 RVA: 0x0005D789 File Offset: 0x0005B989
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool LocalMonthlyNotice
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(0);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(0, value);
		}
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06000F21 RID: 3873 RVA: 0x0005D793 File Offset: 0x0005B993
	// (set) Token: 0x06000F22 RID: 3874 RVA: 0x0005D79B File Offset: 0x0005B99B
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool GlobalMonthlyNotice
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(1);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(1, value);
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0005D7A5 File Offset: 0x0005B9A5
	// (set) Token: 0x06000F24 RID: 3876 RVA: 0x0005D7AD File Offset: 0x0005B9AD
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool MiniMapViewing
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(2);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(2, value);
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0005D7B7 File Offset: 0x0005B9B7
	// (set) Token: 0x06000F26 RID: 3878 RVA: 0x0005D7BF File Offset: 0x0005B9BF
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool IntraStateTravel
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(3);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(3, value);
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06000F27 RID: 3879 RVA: 0x0005D7C9 File Offset: 0x0005B9C9
	// (set) Token: 0x06000F28 RID: 3880 RVA: 0x0005D7D1 File Offset: 0x0005B9D1
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool InterStateTravel
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(4);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(4, value);
		}
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06000F29 RID: 3881 RVA: 0x0005D7DB File Offset: 0x0005B9DB
	// (set) Token: 0x06000F2A RID: 3882 RVA: 0x0005D7E3 File Offset: 0x0005B9E3
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool WorldResourceCollection
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(5);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(5, value);
		}
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06000F2B RID: 3883 RVA: 0x0005D7ED File Offset: 0x0005B9ED
	// (set) Token: 0x06000F2C RID: 3884 RVA: 0x0005D7F5 File Offset: 0x0005B9F5
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool LocationMarking
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(6);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(6, value);
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06000F2D RID: 3885 RVA: 0x0005D7FF File Offset: 0x0005B9FF
	// (set) Token: 0x06000F2E RID: 3886 RVA: 0x0005D807 File Offset: 0x0005BA07
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool HereticStrongholdGenerating
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(7);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(7, value);
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06000F2F RID: 3887 RVA: 0x0005D811 File Offset: 0x0005BA11
	// (set) Token: 0x06000F30 RID: 3888 RVA: 0x0005D819 File Offset: 0x0005BA19
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool RighteousStrongholdGenerating
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(8);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(8, value);
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06000F31 RID: 3889 RVA: 0x0005D823 File Offset: 0x0005BA23
	// (set) Token: 0x06000F32 RID: 3890 RVA: 0x0005D82C File Offset: 0x0005BA2C
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool CaravanDisplay
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(9);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(9, value);
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06000F33 RID: 3891 RVA: 0x0005D837 File Offset: 0x0005BA37
	// (set) Token: 0x06000F34 RID: 3892 RVA: 0x0005D840 File Offset: 0x0005BA40
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool TaiwuVillageManagement
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(10);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(10, value);
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06000F35 RID: 3893 RVA: 0x0005D84B File Offset: 0x0005BA4B
	// (set) Token: 0x06000F36 RID: 3894 RVA: 0x0005D854 File Offset: 0x0005BA54
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool Chicken
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(11);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(11, value);
		}
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06000F37 RID: 3895 RVA: 0x0005D85F File Offset: 0x0005BA5F
	// (set) Token: 0x06000F38 RID: 3896 RVA: 0x0005D868 File Offset: 0x0005BA68
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool Crafting
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(12);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(12, value);
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06000F39 RID: 3897 RVA: 0x0005D873 File Offset: 0x0005BA73
	// (set) Token: 0x06000F3A RID: 3898 RVA: 0x0005D87C File Offset: 0x0005BA7C
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool InfluenceInformation
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(13);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(13, value);
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06000F3B RID: 3899 RVA: 0x0005D887 File Offset: 0x0005BA87
	// (set) Token: 0x06000F3C RID: 3900 RVA: 0x0005D890 File Offset: 0x0005BA90
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool SpiritualDebtAction
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(14);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(14, value);
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06000F3D RID: 3901 RVA: 0x0005D89B File Offset: 0x0005BA9B
	// (set) Token: 0x06000F3E RID: 3902 RVA: 0x0005D8A4 File Offset: 0x0005BAA4
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool SkillLearning
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(15);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(15, value);
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06000F3F RID: 3903 RVA: 0x0005D8AF File Offset: 0x0005BAAF
	// (set) Token: 0x06000F40 RID: 3904 RVA: 0x0005D8B8 File Offset: 0x0005BAB8
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool SkillBookExchange
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(16);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(16, value);
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06000F41 RID: 3905 RVA: 0x0005D8C3 File Offset: 0x0005BAC3
	// (set) Token: 0x06000F42 RID: 3906 RVA: 0x0005D8CC File Offset: 0x0005BACC
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool CombatSkillBreakOut
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(17);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(17, value);
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0005D8D7 File Offset: 0x0005BAD7
	// (set) Token: 0x06000F44 RID: 3908 RVA: 0x0005D8E0 File Offset: 0x0005BAE0
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool Aspiration
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(18);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(18, value);
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06000F45 RID: 3909 RVA: 0x0005D8EB File Offset: 0x0005BAEB
	// (set) Token: 0x06000F46 RID: 3910 RVA: 0x0005D8F4 File Offset: 0x0005BAF4
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool Kidnap
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(19);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(19, value);
		}
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06000F47 RID: 3911 RVA: 0x0005D8FF File Offset: 0x0005BAFF
	// (set) Token: 0x06000F48 RID: 3912 RVA: 0x0005D908 File Offset: 0x0005BB08
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool Information
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(20);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(20, value);
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06000F49 RID: 3913 RVA: 0x0005D913 File Offset: 0x0005BB13
	// (set) Token: 0x06000F4A RID: 3914 RVA: 0x0005D91C File Offset: 0x0005BB1C
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool LegendaryBook
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(21);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(21, value);
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0005D927 File Offset: 0x0005BB27
	// (set) Token: 0x06000F4C RID: 3916 RVA: 0x0005D930 File Offset: 0x0005BB30
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool WesternRegionMerchant
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(22);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(22, value);
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06000F4D RID: 3917 RVA: 0x0005D93B File Offset: 0x0005BB3B
	// (set) Token: 0x06000F4E RID: 3918 RVA: 0x0005D944 File Offset: 0x0005BB44
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool TeaCaravan
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(23);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(23, value);
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06000F4F RID: 3919 RVA: 0x0005D94F File Offset: 0x0005BB4F
	// (set) Token: 0x06000F50 RID: 3920 RVA: 0x0005D958 File Offset: 0x0005BB58
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool JuniorXiangshuSummoning
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(24);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(24, value);
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06000F51 RID: 3921 RVA: 0x0005D963 File Offset: 0x0005BB63
	// (set) Token: 0x06000F52 RID: 3922 RVA: 0x0005D96C File Offset: 0x0005BB6C
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool MartialArtContest
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(25);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(25, value);
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06000F53 RID: 3923 RVA: 0x0005D977 File Offset: 0x0005BB77
	// (set) Token: 0x06000F54 RID: 3924 RVA: 0x0005D980 File Offset: 0x0005BB80
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool TaiwuProfession
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(27);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(27, value);
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000F55 RID: 3925 RVA: 0x0005D98B File Offset: 0x0005BB8B
	// (set) Token: 0x06000F56 RID: 3926 RVA: 0x0005D994 File Offset: 0x0005BB94
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool SectXvannvMusic
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(28);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(28, value);
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000F57 RID: 3927 RVA: 0x0005D99F File Offset: 0x0005BB9F
	// (set) Token: 0x06000F58 RID: 3928 RVA: 0x0005D9A8 File Offset: 0x0005BBA8
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool CricketCombat
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(32);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(32, value);
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0005D9B3 File Offset: 0x0005BBB3
	// (set) Token: 0x06000F5A RID: 3930 RVA: 0x0005D9BC File Offset: 0x0005BBBC
	[GMProperty(EGMGroup.MapWorldFunction, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool ActionPlanning
	{
		get
		{
			return GMFunc._EditWorldFunctionsStatus(30);
		}
		set
		{
			GMFunc._EditWorldFunctionsStatus(30, value);
		}
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0005D9C7 File Offset: 0x0005BBC7
	[GMFunc(EGMGroup.MapBase, 0.4f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CombatResultTypeField, 0.2f)]
	public static void OvercomeBattleInEvent(sbyte combatResultType)
	{
		GMFunc.OvercomeCombatResultType = combatResultType;
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0005D9D0 File Offset: 0x0005BBD0
	public static bool IsOvercomeCombat(short combatConfigId, List<int> enemyTeam)
	{
		bool flag = GMFunc.OvercomeCombatResultType < 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			CombatConfigItem combatConfig = CombatConfig.Instance.GetItem(combatConfigId);
			sbyte currentOvercomeCombatResultType = GMFunc.OvercomeCombatResultType;
			sbyte b = currentOvercomeCombatResultType;
			sbyte b2 = b;
			if (b2 > 5)
			{
				bool flag2 = combatConfig.CombatType == 2;
				sbyte[] results;
				if (flag2)
				{
					results = new sbyte[]
					{
						5,
						3,
						4,
						2
					};
				}
				else
				{
					results = new sbyte[]
					{
						5,
						3,
						1,
						4,
						2,
						0
					};
				}
				results.Shuffle(1);
				currentOvercomeCombatResultType = results.First<sbyte>();
			}
			UI_GMWindow instance = UI_GMWindow.Instance;
			bool flag3 = instance != null;
			if (flag3)
			{
				instance.LogCombatOvercome(currentOvercomeCombatResultType);
			}
			else
			{
				AdaptableLog.Info(string.Format("currentOvercomeCombatResultType = {0}. (UI_GMWindow not initialized)", currentOvercomeCombatResultType));
			}
			TaiwuEventDomainMethod.Call.GmCmd_TriggerOvercomeCombatOver((int)currentOvercomeCombatResultType, (int)combatConfig.CombatType, enemyTeam[0]);
			result = true;
		}
		return result;
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0005DAB8 File Offset: 0x0005BCB8
	public static bool IsOvercomeLifeSkillCombat()
	{
		bool flag = GMFunc.OvercomeLifeSkillCombatResultType < 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			sbyte currentOvercomeLifeSkillCombatResultType = GMFunc.OvercomeLifeSkillCombatResultType;
			sbyte b = currentOvercomeLifeSkillCombatResultType;
			sbyte b2 = b;
			if (b2 > 1)
			{
				if (b2 - 2 <= 1)
				{
					return false;
				}
				currentOvercomeLifeSkillCombatResultType = (sbyte)Random.Range(0, 2);
			}
			UI_GMWindow.Instance.LogLifeSkillCombatOvercome(currentOvercomeLifeSkillCombatResultType);
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("LifeSkillBattleComplete", "WinState", currentOvercomeLifeSkillCombatResultType == 1);
			TaiwuEventDomainMethod.Call.TriggerListener("LifeSkillBattleComplete", true);
			result = true;
		}
		return result;
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0005DB34 File Offset: 0x0005BD34
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default, ConsoleName = "switch_active_debug")]
	public static void SwitchActiveReadDebugMode()
	{
		ReadAndLoop.DebugMode = !ReadAndLoop.DebugMode;
		Debug.Log(string.Format("#GM# current active read/loop debug mode is {0}", ReadAndLoop.DebugMode));
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0005DB5E File Offset: 0x0005BD5E
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.NoTry)]
	public static void ThrowFrontend()
	{
		throw new Exception("a frontend test exception");
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0005DB6B File Offset: 0x0005BD6B
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ThrowBackend()
	{
		MapDomainMethod.Call.GMCmd_ThrowBackend();
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0005DB74 File Offset: 0x0005BD74
	[GMFunc(EGMGroup.MapBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ChangeCatchCricketAreaType(sbyte areaType)
	{
		UIElement.CatchCricket.UiBaseAs<ViewCatchCricket>().AreaType = (EMapAreaAreaDirection)areaType;
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0005DB88 File Offset: 0x0005BD88
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ForceUpdateInfluencePower(short settlementId)
	{
		OrganizationDomainMethod.Call.GmCmd_ForceUpdateInfluencePower(settlementId);
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0005DB94 File Offset: 0x0005BD94
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetCurrentLocationSettlementId()
	{
		WorldMapModel worldMapWorld = SingletonObject.getInstance<WorldMapModel>();
		MapAreaData area = worldMapWorld.Areas[(int)worldMapWorld.CurrentAreaId];
		foreach (SettlementInfo settlement in area.SettlementInfos)
		{
			MapBlockData settlementBlockData = worldMapWorld.GetBlockData(settlement.BlockId);
			MapBlockData currentBlockData = worldMapWorld.GetBlockData(worldMapWorld.CurrentBlockId);
			bool flag = settlementBlockData.GetRootBlock() == currentBlockData.GetRootBlock();
			if (flag)
			{
				UI_GMWindow.Instance.Log(settlement.SettlementId.ToString());
				break;
			}
		}
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0005DC24 File Offset: 0x0005BE24
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ForceUpdateTreasuryGuards(short settlementId)
	{
		OrganizationDomainMethod.Call.GmCmd_ForceUpdateTreasuryGuards(settlementId);
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0005DC2E File Offset: 0x0005BE2E
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ClearSettlementTreasuryAlertTime(short settlementId)
	{
		OrganizationDomainMethod.Call.GmCmd_ClearSettlementTreasuryAlertTime(settlementId);
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0005DC38 File Offset: 0x0005BE38
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void ClearSettlementTreasuryItemAndResource(short settlementId)
	{
		OrganizationDomainMethod.Call.GmCmd_ClearSettlementTreasuryItemAndResource(settlementId);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0005DC42 File Offset: 0x0005BE42
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void UpdateSettlementTreasury(short settlementId)
	{
		OrganizationDomainMethod.Call.GmCmd_UpdateSettlementTreasury(settlementId);
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0005DC4C File Offset: 0x0005BE4C
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.SectIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void AddSectBounty(int sectIndex, int charId, short punishmentType)
	{
		sbyte orgTemplateId = (sbyte)(1 + sectIndex);
		PunishmentTypeItem punishmentTypeCfg = PunishmentType.Instance[punishmentType];
		PunishmentSeverityItem severityCfg = PunishmentSeverity.Instance[punishmentTypeCfg.Severity];
		bool flag = severityCfg.BountyDuration <= 0;
		if (!flag)
		{
			OrganizationDomainMethod.Call.AddSectBounty(orgTemplateId, charId, punishmentTypeCfg.Severity, punishmentType, severityCfg.BountyDuration);
		}
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0005DCA4 File Offset: 0x0005BEA4
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.SectIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.2f)]
	public static void AddSectPrisoner(int sectIndex, int charId, short punishmentType, sbyte punishmentSeverity)
	{
		sbyte orgTemplateId = (sbyte)(1 + sectIndex);
		PunishmentSeverityItem severityCfg = PunishmentSeverity.Instance[punishmentSeverity];
		bool flag = severityCfg != null && severityCfg.PrisonTime <= 0;
		if (!flag)
		{
			OrganizationDomainMethod.Call.AddSectPrisoner(orgTemplateId, charId, punishmentSeverity, punishmentType, severityCfg.PrisonTime);
		}
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0005DCEB File Offset: 0x0005BEEB
	[GMFunc(EGMGroup.SettlementTreasury, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StateIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.2f)]
	public static void UpdateCityPunishmentSeverityCustomizeData(sbyte stateTemplateId, bool isSect, short punishmentTypeTemplateId, sbyte customizedPunishmentSeverityTemplateId)
	{
		OrganizationDomainMethod.Call.UpdateCityPunishmentSeverityCustomizeData(stateTemplateId, isSect, punishmentTypeTemplateId, customizedPunishmentSeverityTemplateId);
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0005DCF8 File Offset: 0x0005BEF8
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetEventCaravanDisplayData()
	{
		CaravanDisplayData caravanData = SingletonObject.getInstance<EventModel>().DisplayingEventData.ExtraData.CaravanData;
		bool flag = caravanData == null || caravanData.CaravanId < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log("not find caravan data");
		}
		else
		{
			UI_GMWindow.Instance.Log(caravanData.ToString());
		}
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0005DD54 File Offset: 0x0005BF54
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetCaravanDisplayData(int caravanId)
	{
		MerchantDomainMethod.AsyncCall.GetCaravanDisplayData(null, caravanId, delegate(int offset, RawDataPool dataPool)
		{
			CaravanDisplayData caravanDisplayData = null;
			Serializer.Deserialize(dataPool, offset, ref caravanDisplayData);
			bool flag = caravanDisplayData == null;
			if (flag)
			{
				UI_GMWindow.Instance.Log(string.Format("not find caravan data with id {0}", caravanId));
			}
			else
			{
				UI_GMWindow.Instance.Log(caravanDisplayData.ToString());
			}
		});
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0005DD88 File Offset: 0x0005BF88
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetCaravanInvested(int caravanId, bool isInvested)
	{
		MerchantDomainMethod.Call.GmCmd_SetCaravanInvested(caravanId, isInvested);
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x0005DD93 File Offset: 0x0005BF93
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetAllCaravanInvested(bool isInvested)
	{
		MerchantDomainMethod.Call.GmCmd_SetAllCaravanInvested(isInvested);
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x0005DD9D File Offset: 0x0005BF9D
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(1, EWidgetType.CaravanStateField, 0.1f)]
	public static void SetCaravanState(int caravanId, int state)
	{
		MerchantDomainMethod.Call.GmCmd_SetCaravanState(caravanId, (sbyte)state);
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x0005DDA9 File Offset: 0x0005BFA9
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetCaravanRobbedRate(int caravanId, short robbedRate)
	{
		MerchantDomainMethod.Call.GmCmd_SetCaravanRobbedRate(caravanId, robbedRate);
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0005DDB4 File Offset: 0x0005BFB4
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetCaravanIncomeData(int caravanId, short incomeBonus, short incomeCriticalRate, short incomeCriticalResult)
	{
		MerchantDomainMethod.Call.GmCmd_SetCaravanIncomeData(caravanId, incomeBonus, incomeCriticalRate, incomeCriticalResult);
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0005DDC1 File Offset: 0x0005BFC1
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ProtectCaravan(int caravanId)
	{
		MerchantDomainMethod.Call.GmCmd_ProtectCaravan(caravanId);
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x0005DDCB File Offset: 0x0005BFCB
	[GMFunc(EGMGroup.Caravan, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ProtectAllCaravan()
	{
		MerchantDomainMethod.Call.GmCmd_ProtectAllCaravan();
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x0005DDD4 File Offset: 0x0005BFD4
	[GMFunc(EGMGroup.Merchant, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RemoveMerchantData(int merchantId)
	{
		MerchantDomainMethod.Call.GmCmd_RemoveMerchantData(merchantId);
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0005DDDE File Offset: 0x0005BFDE
	[GMFunc(EGMGroup.Merchant, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetMerchantCharToType(int merchantId, sbyte type)
	{
		MerchantDomainMethod.Call.GmCmd_SetMerchantCharToType(merchantId, type);
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0005DDE9 File Offset: 0x0005BFE9
	[GMFunc(EGMGroup.Merchant, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetMerchantExp(int merchantId, sbyte type, int value)
	{
		MerchantDomainMethod.Call.GmCmd_SetMerchantExp(merchantId, type, value);
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0005DDF5 File Offset: 0x0005BFF5
	[GMFunc(EGMGroup.Merchant, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetMerchantExp(int merchantId, sbyte type)
	{
		MerchantDomainMethod.AsyncCall.GmCmd_GetMerchantExp(null, merchantId, type, delegate(int offset, RawDataPool dataPool)
		{
			int exp = 0;
			Serializer.Deserialize(dataPool, offset, ref exp);
			UI_GMWindow.Instance.Log(exp.ToString());
		});
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x0005DE20 File Offset: 0x0005C020
	[GMFunc(EGMGroup.Merchant, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetMerchantLevel(int merchantId, sbyte type)
	{
		MerchantDomainMethod.AsyncCall.GmCmd_GetMerchantLevel(null, merchantId, type, delegate(int offset, RawDataPool dataPool)
		{
			int level = 0;
			Serializer.Deserialize(dataPool, offset, ref level);
			UI_GMWindow.Instance.Log(level.ToString());
		});
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x0005DE4C File Offset: 0x0005C04C
	[GMObject(EGMGroup.CombatBase, 2000)]
	public static GameObject GetCombatEditor()
	{
		GameObject obj = Object.Instantiate<GameObject>(UI_GMWindow.Instance.CombatEditor);
		GMCombatEditor editor = obj.GetComponent<GMCombatEditor>();
		UI_GMWindow instance = UI_GMWindow.Instance;
		instance.OnWorldDataReadyChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(instance.OnWorldDataReadyChild, new UI_GMWindow.WorldStateCallback(editor.OnWorldDataReady));
		UI_GMWindow instance2 = UI_GMWindow.Instance;
		instance2.OnLeaveWorldChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(instance2.OnLeaveWorldChild, new UI_GMWindow.WorldStateCallback(editor.OnLeaveWorld));
		obj.SetActive(true);
		editor.Init();
		return obj;
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x0005DED4 File Offset: 0x0005C0D4
	[GMFunc(EGMGroup.CombatBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BattleSceneIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.2f)]
	public static void ReLoadCombatScene(short sceneId, int index, bool forceWinter)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("SceneId", sceneId);
		argBox.Set("Index", index);
		argBox.Set("ForceWinter", forceWinter);
		GEvent.OnEvent(UiEvents.GmReloadCombatScene, argBox);
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0005DF20 File Offset: 0x0005C120
	[GMFunc(EGMGroup.CombatBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CombatWeatherField, 0.2f)]
	public static void SwitchCombatWeather(string weatherNodeName)
	{
		bool flag = UI_CombatBackground.Instance == null || UI_CombatBackground.Instance.Scene == null;
		if (flag)
		{
			UI_GMWindow.Instance.Log("当前不在战斗场景，无法切换天气。");
		}
		else
		{
			global::CombatScene scene = UI_CombatBackground.Instance.Scene;
			bool flag2 = weatherNodeName == "__random__";
			if (flag2)
			{
				sbyte month = SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear();
				scene.RefreshSeasonalAndRandomVisibility((int)month);
			}
			else
			{
				bool flag3 = string.IsNullOrEmpty(weatherNodeName);
				if (flag3)
				{
					scene.GmForceNoWeather();
				}
				else
				{
					bool succeed = scene.GmSwitchToWeatherNode(weatherNodeName);
					bool flag4 = !succeed;
					if (flag4)
					{
						UI_GMWindow.Instance.Log("当前战斗场景不存在名为" + weatherNodeName + "的分组节点，未发生切换。");
					}
				}
			}
		}
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x0005DFDC File Offset: 0x0005C1DC
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.2f)]
	public static void QueryBuildId(string buildName)
	{
		string ret = "";
		foreach (BuildingBlockItem config in ((IEnumerable<BuildingBlockItem>)BuildingBlock.Instance))
		{
			bool flag = config.Name.Contains(buildName);
			if (flag)
			{
				ret += string.Format("{0}-{1}  ", config.Name.SetColor(Color.green), config.TemplateId);
			}
		}
		UI_GMWindow.Instance.Log(ret);
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0005E078 File Offset: 0x0005C278
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void PlaceBuilding(short buildId, sbyte level)
	{
		bool flag = !UIElement.BuildingArea.Exist;
		if (flag)
		{
			UI_GMWindow.Instance.Log(LocalStringManager.Get("GM_Message_GMFunc_PlaceBuilding_Msg"));
		}
		else
		{
			ViewBuildingArea buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
			buildingArea.StartPlacingBuilding(BuildingBlock.Instance[buildId], null, level, true);
		}
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0005E0D0 File Offset: 0x0005C2D0
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RemoveBuilding()
	{
		bool flag = !UIElement.BuildingManageOld.Exist;
		if (flag)
		{
			UI_GMWindow.Instance.Log(LocalStringManager.Get("GM_Message_GMFunc_RemoveBuilding_Msg"));
		}
		else
		{
			ViewBuildingManage buildManage = UIElement.BuildingManage.UiBaseAs<ViewBuildingManage>();
			BuildingBlockKey key = buildManage.BlockKey;
			BuildingDomainMethod.AsyncCall.GmCmd_RemoveBuildingImmediately(null, key, delegate(int offset, RawDataPool dataPool)
			{
				ValueTuple<short, BuildingBlockData> result = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
				Serializer.Deserialize(dataPool, offset, ref result);
				ViewBuildingArea area = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
				area.UpdateBuildingData(key, result.Item2, true);
				area.UpdateRoad();
				UIElement.BuildingManage.UiBaseAs<ViewBuildingManage>().QuickHide();
			});
		}
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0005E140 File Offset: 0x0005C340
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RemoveAllBuilding()
	{
		bool exist = UIElement.BuildingManageOld.Exist;
		if (exist)
		{
			UIElement.BuildingManageOld.UiBaseAs<UI_BuildingManage>().QuickHide();
		}
		Location blockKey = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		BuildingDomainMethod.AsyncCall.GetBuildingBlockList(null, blockKey, delegate(int offset, RawDataPool pool)
		{
			List<BuildingBlockData> blockList = new List<BuildingBlockData>();
			Serializer.Deserialize(pool, offset, ref blockList);
			foreach (BuildingBlockData block in blockList)
			{
				short templateId = block.TemplateId;
				bool flag = templateId < 0 || templateId == 45 || templateId == 49 || templateId == 51;
				if (!flag)
				{
					BuildingBlockItem config = block.ConfigData;
					EBuildingBlockType type = config.Type;
					bool canBeRemoved = type == EBuildingBlockType.NormalResource || type == EBuildingBlockType.SpecialResource || type == EBuildingBlockType.UselessResource || type == EBuildingBlockType.Building;
					bool flag2 = !canBeRemoved;
					if (!flag2)
					{
						BuildingBlockKey key = new BuildingBlockKey(blockKey.AreaId, blockKey.BlockId, block.BlockIndex);
						BuildingDomainMethod.AsyncCall.GmCmd_RemoveBuildingImmediately(null, key, delegate(int offset2, RawDataPool dataPool2)
						{
							ValueTuple<short, BuildingBlockData> result = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
							Serializer.Deserialize(dataPool2, offset2, ref result);
							bool exist2 = UIElement.BuildingArea.Exist;
							if (exist2)
							{
								ViewBuildingArea area = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
								area.UpdateBuildingData(key, result.Item2, true);
								area.UpdateRoad();
							}
						});
					}
				}
			}
		});
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0005E19F File Offset: 0x0005C39F
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetBuildingAreaEffectProgresses()
	{
		ExtraDomainMethod.AsyncCall.GmCmd_GetBuildingAreaEffectProgresses(null, delegate(int offset, RawDataPool dataPool)
		{
			ValueTuple<int, int, int> result = new ValueTuple<int, int, int>(0, 0, 0);
			Serializer.Deserialize(dataPool, offset, ref result);
			UI_GMWindow.Instance.Log(string.Format("Animal:{0}, Cricket:{1}, Adventure:{2}", result.Item1, result.Item2, result.Item3));
		});
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0005E1C8 File Offset: 0x0005C3C8
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void SetBuildingAreaEffectProgresses(int animal, int cricket, int adventure)
	{
		ExtraDomainMethod.Call.GmCmd_SetBuildingAreaEffectProgresses(animal, cricket, adventure);
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void AddBuildingLegacy(short templateId)
	{
		bool flag = templateId > 0 && templateId < 44;
		if (flag)
		{
			BuildingDomainMethod.Call.GmCmd_AddLegacyBuilding(templateId);
		}
		else
		{
			UI_GMWindow.Instance.Log("BuildingTemplateId out Range,Only ResourceType useful");
		}
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x0005E210 File Offset: 0x0005C410
	[GMFunc(EGMGroup.Chicken, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryLocalChickenCharacterFeatureInfo()
	{
		bool flag = !UIElement.BuildingArea.Exist;
		if (flag)
		{
			UI_GMWindow.Instance.Log(LocalStringManager.Get("GM_Message_GMFunc_PlaceBuilding_Msg"));
		}
		else
		{
			ViewBuildingArea buildingArea = UIElement.BuildingArea.UiBaseAs<ViewBuildingArea>();
			Location location = buildingArea.CurrentLocation;
			BuildingDomainMethod.AsyncCall.GetSettlementChickenIdList(null, location, delegate(int offset, RawDataPool dataPool)
			{
				GMFunc.<>c__DisplayClass383_0 CS$<>8__locals1 = new GMFunc.<>c__DisplayClass383_0();
				List<int> result = new List<int>();
				Serializer.Deserialize(dataPool, offset, ref result);
				CS$<>8__locals1.localChicken = new List<GameData.Domains.Building.Chicken>();
				CS$<>8__locals1.localChickenFeatureIds = new HashSet<short>();
				int i = 0;
				int leni = result.Count;
				while (i < leni)
				{
					int chickenId = result[i];
					BuildingDomainMethod.AsyncCall.GetChickenData(null, chickenId, delegate(int offset2, RawDataPool dataPool2)
					{
						GameData.Domains.Building.Chicken chicken = default(GameData.Domains.Building.Chicken);
						Serializer.Deserialize(dataPool2, offset2, ref chicken);
						CS$<>8__locals1.localChicken.Add(chicken);
						CS$<>8__locals1.localChickenFeatureIds.Add(Config.Chicken.Instance[chicken.TemplateId].FeatureId);
						bool flag2 = CS$<>8__locals1.localChicken.Count == leni;
						if (flag2)
						{
							HashSet<int> characterSet = new HashSet<int>();
							int characterSetCount = 0;
							AsyncMethodCallbackDelegate <>9__2;
							for (int j = 0; j < leni; j++)
							{
								IAsyncMethodRequestHandler requestHandler = null;
								short settlementId = (short)CS$<>8__locals1.localChicken[j].CurrentSettlementId;
								AsyncMethodCallbackDelegate callback;
								if ((callback = <>9__2) == null)
								{
									callback = (<>9__2 = delegate(int offset3, RawDataPool dataPool3)
									{
										List<CharacterDisplayData> list = new List<CharacterDisplayData>();
										Serializer.Deserialize(dataPool3, offset3, ref list);
										characterSet.UnionWith(from data in list
										select data.CharacterId);
										characterSetCount++;
										bool flag3 = characterSetCount == leni;
										if (flag3)
										{
											IEnumerable<ValueTuple<DataUid, Type>> key = from characterId in characterSet
											select new ValueTuple<DataUid, Type>(new DataUid(4, 0, (ulong)((long)characterId), 17U), typeof(List<short>));
											UI_GMWindow instance = UI_GMWindow.Instance;
											Action<Dictionary<DataUid, object>> callback2;
											if ((callback2 = CS$<>8__locals1.<>9__5) == null)
											{
												callback2 = (CS$<>8__locals1.<>9__5 = delegate(Dictionary<DataUid, object> data)
												{
													StringBuilder sb = new StringBuilder();
													foreach (KeyValuePair<DataUid, object> pair in data)
													{
														ulong characterId = pair.Key.SubId0;
														IEnumerable<short> source = (List<short>)pair.Value;
														Func<short, bool> predicate;
														if ((predicate = CS$<>8__locals1.<>9__6) == null)
														{
															predicate = (CS$<>8__locals1.<>9__6 = ((short featureId) => CS$<>8__locals1.localChickenFeatureIds.Contains(featureId)));
														}
														short[] featureIds = source.Where(predicate).ToArray<short>();
														sb.AppendFormat("({0}: ", characterId);
														bool flag4 = featureIds.Any<short>();
														if (flag4)
														{
															for (int idx = 0; idx < featureIds.Length; idx++)
															{
																bool flag5 = idx != 0;
																if (flag5)
																{
																	sb.Append(", ");
																}
																sb.Append(CharacterFeature.Instance[featureIds[idx]].Name);
															}
															sb.Append(')');
														}
														else
														{
															sb.AppendFormat("{0})", LocalStringManager.Get(LanguageKey.LK_None));
														}
													}
													UI_GMWindow.Instance.Log(sb.ToString());
												});
											}
											instance.RequestGameDataReceiving(callback2, key.ToArray<ValueTuple<DataUid, Type>>());
										}
									});
								}
								OrganizationDomainMethod.AsyncCall.GetSettlementMembers(requestHandler, settlementId, callback);
							}
						}
					});
					i++;
				}
			});
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0005E280 File Offset: 0x0005C480
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void SetTeaHorseCaravanWeather(short weatherId)
	{
		BuildingDomainMethod.Call.SetTeaHorseCaravanWeather(weatherId);
		UI_GMWindow.Instance.Log("0-晴空 1-冰雹 2-沙暴 3-烈日 4-雷暴雨 5-小雨 6-小雪 7-暴风雪 8-狂风 9-雾");
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0005E29A File Offset: 0x0005C49A
	[GMFunc(EGMGroup.Chicken, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryAllChickenInfo()
	{
		BuildingDomainMethod.AsyncCall.GmCmd_GetChickenData(null, delegate(int offset, RawDataPool pool)
		{
			StringBuilder sb = new StringBuilder();
			List<GameData.Domains.Building.Chicken> chicken = new List<GameData.Domains.Building.Chicken>();
			WorldMapModel worldMapWorld = SingletonObject.getInstance<WorldMapModel>();
			Dictionary<int, string> settlementStringDict = new Dictionary<int, string>();
			Serializer.Deserialize(pool, offset, ref chicken);
			short areaId = 0;
			while ((int)areaId < worldMapWorld.Areas.Length)
			{
				MapAreaData area = worldMapWorld.Areas[(int)areaId];
				foreach (SettlementInfo settlement in area.SettlementInfos)
				{
					string settlementString = string.Format("location: ({0}, {1})", areaId, settlement.BlockId);
					OrganizationItem orgConfig = Organization.Instance.GetItem(settlement.OrgTemplateId);
					bool flag = orgConfig != null;
					if (flag)
					{
						MapAreaItem mapAreaItem = MapArea.Instance[area.GetTemplateId()];
						string stateName = MapState.Instance[mapAreaItem.StateID].Name;
						string areaName = mapAreaItem.Name;
						string settlementName = orgConfig.Name;
						bool flag2 = settlement.RandomNameId >= 0;
						if (flag2)
						{
							settlementName = LocalTownNames.Instance.TownNameCore[(int)settlement.RandomNameId].Name;
						}
						settlementString = string.Concat(new string[]
						{
							stateName,
							"-",
							areaName,
							"-",
							settlementName,
							" "
						});
					}
					settlementStringDict[(int)settlement.SettlementId] = settlementString;
				}
				areaId += 1;
			}
			for (int i = 0; i < chicken.Count; i++)
			{
				GameData.Domains.Building.Chicken chick = chicken[i];
				sb.Append(string.Format("index: {0} tid: {1} ", chick.Id, chick.TemplateId));
				Config.ChickenItem chickConfig = Config.Chicken.Instance.GetItem(chick.TemplateId);
				bool flag3 = chickConfig != null;
				if (flag3)
				{
					sb.Append(chickConfig.Name + " ");
				}
				string settlementString2;
				bool flag4 = settlementStringDict.TryGetValue(chick.CurrentSettlementId, out settlementString2);
				if (flag4)
				{
					sb.Append(settlementString2 + " ");
				}
				sb.AppendLine();
			}
			UI_GMWindow.Instance.Log(sb.ToString());
		});
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0005E2C4 File Offset: 0x0005C4C4
	[GMFunc(EGMGroup.Chicken, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	public static void TransferChicken(int amount)
	{
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		short taiWuSettlementId = worldMapModel.GetTaiwuVillageSettlementId();
		AsyncMethodCallbackDelegate <>9__0;
		foreach (MapAreaData area in worldMapModel.Areas)
		{
			bool flag = ((area != null) ? area.SettlementInfos : null) == null;
			if (!flag)
			{
				foreach (SettlementInfo settlementInfo in area.SettlementInfos)
				{
					bool flag2 = taiWuSettlementId == settlementInfo.SettlementId;
					if (!flag2)
					{
						IAsyncMethodRequestHandler requestHandler = null;
						int settlementId = (int)settlementInfo.SettlementId;
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__0) == null)
						{
							callback = (<>9__0 = delegate(int offset, RawDataPool dataPool)
							{
								List<int> chickenList = new List<int>();
								Serializer.Deserialize(dataPool, offset, ref chickenList);
								chickenList.Shuffle(1);
								foreach (int chickenId in chickenList)
								{
									BuildingDomainMethod.Call.TransferChicken(chickenId, (int)taiWuSettlementId);
								}
							});
						}
						BuildingDomainMethod.AsyncCall.GetSettlementChickenList(requestHandler, settlementId, callback);
					}
				}
			}
		}
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0005E396 File Offset: 0x0005C596
	[GMFunc(EGMGroup.Chicken, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void TransferChickenToSettlement(int chickenId, int settlementId)
	{
		BuildingDomainMethod.Call.TransferChicken(chickenId, settlementId);
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x0005E3A1 File Offset: 0x0005C5A1
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BuildingBlockTemplateIdCollectField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void BuildingCollectPerform(int totalAttainment, short buildingTemplateId, int repeat)
	{
		BuildingDomainMethod.AsyncCall.GmCmd_BuildingCollectPerform(null, totalAttainment, buildingTemplateId, repeat, delegate(int offset, RawDataPool pool)
		{
		});
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x0005E3CD File Offset: 0x0005C5CD
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void BeatMinionPerform(sbyte grade, int repeat)
	{
		BuildingDomainMethod.AsyncCall.GmCmd_BeatMinionPerform(null, grade, repeat, delegate(int offset, RawDataPool pool)
		{
		});
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0005E3F8 File Offset: 0x0005C5F8
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	public static void UnlockAllProfessionSkills(bool maxSeniority = true)
	{
		GMFunc.TaiwuProfession = true;
		ExtraDomainMethod.Call.UnlockAllProfessionSkills(maxSeniority);
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0005E409 File Offset: 0x0005C609
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.2f)]
	public static void SetProfessionTestSetting(bool noChangeCoolDown = true, bool noSkillCooldown = true, bool noSkillCost = true)
	{
		ExtraDomainMethod.Call.SetProfessionTestSetting(noSkillCooldown, noSkillCost);
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0005E414 File Offset: 0x0005C614
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.ProfessionIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	public static void SetTargetProfessionData(int professionId, int seniority)
	{
		GMFunc.TaiwuProfession = true;
		GMFunc.ModifyWorldFunctionsStatus();
		ExtraDomainMethod.Call.SetProfessionSeniorityTarget(seniority, professionId);
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x0005E42C File Offset: 0x0005C62C
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.ProfessionIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	public static void SetTargetExtraProfessionData(int professionId, int extraSeniority)
	{
		GMFunc.TaiwuProfession = true;
		GMFunc.ModifyWorldFunctionsStatus();
		ExtraDomainMethod.Call.SetProfessionExtraSeniority(professionId, extraSeniority);
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x0005E444 File Offset: 0x0005C644
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.ProfessionIdField, 0.1f)]
	public static void ShowTargetProfession(int professionId)
	{
		ProfessionData profession = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(professionId);
		ProfessionItem config = profession.GetConfig();
		UI_GMWindow.Instance.Log(string.Format("{0} {1} {2}%", config.Name, profession.Seniority, profession.GetSeniorityPercent()));
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x0005E496 File Offset: 0x0005C696
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StateIdField, 0.1f)]
	public static void SetStateTempleVisited(sbyte stateTemplateId)
	{
		ExtraDomainMethod.Call.GmCmd_Profession_SetTempleVisited(stateTemplateId);
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0005E4A0 File Offset: 0x0005C6A0
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.1f)]
	public static void SetBuddhistMonkSavedSoulCount(int count)
	{
		ExtraDomainMethod.Call.GmCmd_Profession_SetBuddhistMonkSavedSoulCount(count);
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x0005E4AA File Offset: 0x0005C6AA
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RecoverHunterCarrierAttackCount()
	{
		ExtraDomainMethod.Call.GmCmd_Profession_RecoverHunterCarrierAttackCount();
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x0005E4B3 File Offset: 0x0005C6B3
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	public static void CastTasterUltimateOnCurrentBlock(bool isCombatSkill)
	{
		ExtraDomainMethod.AsyncCall.GmCmd_CastTasterUltimateOnCurrentBlock(null, isCombatSkill, delegate(int offset, RawDataPool dataPool)
		{
			string result = null;
			Serializer.Deserialize(dataPool, offset, ref result);
			UI_GMWindow.Instance.Log((result == null) ? "Either no npc on the block or no skill book in inventory" : "Success");
			UI_GMWindow.Instance.Log(result);
		});
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x0005E4DD File Offset: 0x0005C6DD
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void MarkAllCarrierFullTamePoint()
	{
		TaiwuDomainMethod.Call.GmCmd_MarkAllCarrierFullTamePoint();
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0005E4E6 File Offset: 0x0005C6E6
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ForceUpdateTaiwuVillager()
	{
		OrganizationDomainMethod.Call.ForceUpdateTaiwuVillager();
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0005E4EF File Offset: 0x0005C6EF
	[GMFunc(EGMGroup.BuildBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.TeaHorseCaravanEventIdField, 0.2f)]
	public static void SetNextTeaHorseEvent(short templateId)
	{
		BuildingDomainMethod.Call.SetNextTeaHorseCaravanEvent(templateId);
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0005E4F9 File Offset: 0x0005C6F9
	[GMFunc(EGMGroup.VillagerRole, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.VillagerRoleField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	public static void SetVillagerRole(short roleId, int charId)
	{
		TaiwuDomainMethod.AsyncCall.SetVillagerRole(null, charId, roleId, delegate(int offset, RawDataPool pool)
		{
			bool result = false;
			Serializer.Deserialize(pool, offset, ref result);
			UI_GMWindow.Instance.Log(string.Format("Result: {0}", result));
		});
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0005E524 File Offset: 0x0005C724
	[GMFunc(EGMGroup.VillagerRole, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.VillagerRoleField, 0.2f)]
	[GMFuncArg(1, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void DispatchVillagerArrangement(short roleId, int charId, short areaId, short blockId)
	{
		VillagerRoleArrangementItem arrangement = VillagerRoleArrangement.Instance.FirstOrDefault((VillagerRoleArrangementItem a) => !a.InvisibleInGui && a.TemplateId == roleId);
		bool flag = arrangement == null;
		if (!flag)
		{
			TaiwuDomainMethod.AsyncCall.DispatchVillagerArrangement(null, charId, arrangement.TemplateId, new Location(areaId, blockId), delegate(int offset, RawDataPool pool)
			{
				bool result = false;
				Serializer.Deserialize(pool, offset, ref result);
				UI_GMWindow.Instance.Log(string.Format("Result: {0}", result));
			});
		}
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0005E594 File Offset: 0x0005C794
	[GMFunc(EGMGroup.VillagerRole, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.CharIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.1f)]
	[GMFuncArg(3, EWidgetType.BoolField, 0.1f)]
	public static void SetFarmerAction(int charId, int areaId, int resourceType, bool isCollectResource)
	{
		if (isCollectResource)
		{
			TaiwuDomainMethod.Call.SetFarmerCollectResourceWork(-1, charId, (short)areaId, (sbyte)resourceType);
		}
		else
		{
			TaiwuDomainMethod.Call.SetFarmerMigrateWork(-1, charId, (short)areaId, (sbyte)resourceType);
		}
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x0005E5C1 File Offset: 0x0005C7C1
	[GMFunc(EGMGroup.VillagerRole, 0.25f, 1000, null, GmRunMode.Default)]
	public static void QueryIsCurrentBlockHasVillagerWork()
	{
		UI_GMWindow.Instance.Log(string.Format("Result: {0}", SingletonObject.getInstance<WorldMapModel>().VillagerWorkLocations.Contains(SingletonObject.getInstance<WorldMapModel>().CurrentLocation)));
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000F9A RID: 3994 RVA: 0x0005E5F7 File Offset: 0x0005C7F7
	// (set) Token: 0x06000F9B RID: 3995 RVA: 0x0005E5FE File Offset: 0x0005C7FE
	[GMProperty(EGMGroup.CombatSkill, 0.25f, 0.25f, 0, EWidgetType.CharIdField)]
	public static int CombatSkillCharId { get; set; }

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0005E606 File Offset: 0x0005C806
	// (set) Token: 0x06000F9D RID: 3997 RVA: 0x0005E60D File Offset: 0x0005C80D
	[GMProperty(EGMGroup.CombatSkill, 0.25f, 0.25f, 0, EWidgetType.StringField)]
	public static string CombatSkillTarget { get; set; }

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000F9E RID: 3998 RVA: 0x0005E615 File Offset: 0x0005C815
	// (set) Token: 0x06000F9F RID: 3999 RVA: 0x0005E61C File Offset: 0x0005C81C
	[GMProperty(EGMGroup.LifeSkill, 0.25f, 0.25f, 0, EWidgetType.CharIdField)]
	public static int LifeSkillCharId
	{
		get
		{
			return GMFunc._lifeSkillCharId;
		}
		set
		{
			GMFunc._lifeSkillCharId = value;
		}
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0005E628 File Offset: 0x0005C828
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.1f)]
	public static void UnLockLifeSkill(bool low, bool middle, bool high)
	{
		bool flag = GMFunc.LifeSkillCharId == -1;
		if (!flag)
		{
			List<GameData.Domains.Character.LifeSkillItem> lifeSkillItems = new List<GameData.Domains.Character.LifeSkillItem>();
			int stage = 0;
			lifeSkillItems.AddRange(from item in LifeSkill.Instance
			where (int)item.Grade < (stage + 1) * 3 && (int)item.Grade >= stage * 3
			select new GameData.Domains.Character.LifeSkillItem(item.TemplateId, low ? 5 : 0));
			stage = 1;
			lifeSkillItems.AddRange(from item in LifeSkill.Instance
			where (int)item.Grade < (stage + 1) * 3 && (int)item.Grade >= stage * 3
			select new GameData.Domains.Character.LifeSkillItem(item.TemplateId, middle ? 5 : 0));
			stage = 2;
			lifeSkillItems.AddRange(from item in LifeSkill.Instance
			where (int)item.Grade < (stage + 1) * 3 && (int)item.Grade >= stage * 3
			select new GameData.Domains.Character.LifeSkillItem(item.TemplateId, high ? 5 : 0));
			CharacterDomainMethod.Call.GmCmd_SetLearnedLifeSkills(GMFunc.LifeSkillCharId, lifeSkillItems);
		}
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0005E714 File Offset: 0x0005C914
	[GMObject(EGMGroup.LifeSkill, 2000)]
	public static GameObject GetLifeSkillEditor()
	{
		GameObject obj = Object.Instantiate<GameObject>(UI_GMWindow.Instance.LifeSkillEditor);
		GMLifeSkillEditor editor = obj.GetComponent<GMLifeSkillEditor>();
		UI_GMWindow instance = UI_GMWindow.Instance;
		instance.OnWorldDataReadyChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(instance.OnWorldDataReadyChild, new UI_GMWindow.WorldStateCallback(editor.OnWorldDataReady));
		UI_GMWindow instance2 = UI_GMWindow.Instance;
		instance2.OnLeaveWorldChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(instance2.OnLeaveWorldChild, new UI_GMWindow.WorldStateCallback(editor.OnLeaveWorld));
		obj.SetActive(true);
		editor.Init();
		return obj;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0005E79C File Offset: 0x0005C99C
	[GMFunc(EGMGroup.CombatSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.1f)]
	[GMFuncArg(2, EWidgetType.BoolField, 0.1f)]
	public static void UnLockCombatSkill(bool low, bool middle, bool high)
	{
		GMFunc.<>c__DisplayClass420_0 CS$<>8__locals1 = new GMFunc.<>c__DisplayClass420_0();
		CS$<>8__locals1.low = low;
		CS$<>8__locals1.middle = middle;
		CS$<>8__locals1.high = high;
		bool flag = GMFunc.CombatSkillCharId == -1;
		if (!flag)
		{
			CS$<>8__locals1.readingState = ushort.MaxValue;
			CS$<>8__locals1.breakPages = 0;
			CS$<>8__locals1.breakPages = CombatSkillStateHelper.SetPageActive(CS$<>8__locals1.breakPages, 2);
			CS$<>8__locals1.breakPages = CombatSkillStateHelper.SetPageActive(CS$<>8__locals1.breakPages, 5);
			CS$<>8__locals1.breakPages = CombatSkillStateHelper.SetPageActive(CS$<>8__locals1.breakPages, 6);
			CS$<>8__locals1.breakPages = CombatSkillStateHelper.SetPageActive(CS$<>8__locals1.breakPages, 7);
			CS$<>8__locals1.breakPages = CombatSkillStateHelper.SetPageActive(CS$<>8__locals1.breakPages, 8);
			CS$<>8__locals1.breakPages = CombatSkillStateHelper.SetPageActive(CS$<>8__locals1.breakPages, 9);
			CS$<>8__locals1.combatSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<CombatSkillMonitor>(GMFunc.CombatSkillCharId, false);
			bool init = CS$<>8__locals1.combatSkillMonitor.Init;
			if (init)
			{
				CS$<>8__locals1.<UnLockCombatSkill>g__CallBack|1();
			}
			else
			{
				CS$<>8__locals1.combatSkillMonitor.AddLearnedSkillsListener(new Action(CS$<>8__locals1.<UnLockCombatSkill>g__CallBack|1));
			}
		}
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0005E8A4 File Offset: 0x0005CAA4
	[GMObject(EGMGroup.CombatSkill, 1999)]
	public static GameObject GetCombatSkillEditor()
	{
		GameObject obj = Object.Instantiate<GameObject>(UI_GMWindow.Instance.CombatSkillEditor);
		GMCombatSkillEditor editor = obj.GetComponent<GMCombatSkillEditor>();
		UI_GMWindow instance = UI_GMWindow.Instance;
		instance.OnWorldDataReadyChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(instance.OnWorldDataReadyChild, new UI_GMWindow.WorldStateCallback(editor.OnWorldDataReady));
		UI_GMWindow instance2 = UI_GMWindow.Instance;
		instance2.OnLeaveWorldChild = (UI_GMWindow.WorldStateCallback)Delegate.Combine(instance2.OnLeaveWorldChild, new UI_GMWindow.WorldStateCallback(editor.OnLeaveWorld));
		obj.SetActive(true);
		editor.Init();
		return obj;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x0005E929 File Offset: 0x0005CB29
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ResetStatsAndAchievements()
	{
		WorldDomainMethod.Call.ResetStatsAndAchievements();
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x0005E932 File Offset: 0x0005CB32
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	public static void SetAllGuidingChapter(bool value)
	{
		WorldDomainMethod.Call.GmCmd_SetAllGuidingChapter(value);
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x0005E93C File Offset: 0x0005CB3C
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LanguageTypeField, 0.4f)]
	public static void SwitchGameLanguage(LocalStringManager.LanguageType languageType)
	{
		GlobalSettings beat = SingletonObject.getInstance<GlobalSettings>();
		beat.Language = languageType.ToString();
		IAsyncMethodRequestHandler requestHandler = null;
		SharedGlobalSettings sharedGlobalSettings = new SharedGlobalSettings();
		sharedGlobalSettings.Language = languageType.ToString();
		sharedGlobalSettings.AutoTriggerMapNormalPickup = beat.EnableAutoTriggerNormalMapPickup;
		sharedGlobalSettings.NormalMapPickupAutoTriggerSetting = new MapPickupAutoTriggerSetting(beat.AutoTriggerNormalPickupIncludeXiangshuMinion, beat.AutoTriggerNormalPickupMinGrade, beat.AutoTriggerNormalPickupTypes);
		sharedGlobalSettings.AutoWipeOut = beat.AutoWipeOut;
		GlobalDomainMethod.AsyncCall.UpdateSharedGlobalSettings(requestHandler, sharedGlobalSettings, delegate(int _, RawDataPool _)
		{
		});
		GlobalDomainMethod.AsyncCall.ReloadAllConfigData(null, delegate(int _, RawDataPool _)
		{
			LocalStringManager.Init(languageType);
			Parallel.ForEach<IConfigData>(ConfigCollection.Items, delegate(IConfigData item)
			{
				item.Init();
			});
			GEvent.OnEvent(UiEvents.OnLanguageChange, null);
		});
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x0005E9FF File Offset: 0x0005CBFF
	// (set) Token: 0x06000FA8 RID: 4008 RVA: 0x0005EA0B File Offset: 0x0005CC0B
	[GMProperty(EGMGroup.SystemGlobal, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool SkipSaving
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>().SkipSaving;
		}
		set
		{
			SingletonObject.getInstance<GlobalSettings>().SkipSaving = value;
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0005EA19 File Offset: 0x0005CC19
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	public static void EnterInGameGuideWorld()
	{
		GameApp.EnterInGameGuideWorld();
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0005EA22 File Offset: 0x0005CC22
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ExitInGameGuideWorld()
	{
		GameApp.ExitInGameGuideWorld();
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0005EA2B File Offset: 0x0005CC2B
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	public static void SetGlobalFlagTypePastEnding(bool value)
	{
		GlobalDomainMethod.Call.SetGlobalFlag(0, value);
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x0005EA36 File Offset: 0x0005CC36
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.1f)]
	public static void SetGlobalFlagTypePastPerformArea(bool value)
	{
		GlobalDomainMethod.Call.SetGlobalFlag(1, value);
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0005EA41 File Offset: 0x0005CC41
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.1f)]
	public static void SetMainBlockOrders(string value)
	{
		TaiwuDomainMethod.Call.SetMainOperationOrder(value.Split(',', StringSplitOptions.None).Select(delegate(string x)
		{
			int o;
			return int.TryParse(x, out o) ? o : 0;
		}).ToArray<int>());
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0005EA7C File Offset: 0x0005CC7C
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RequestNeiliProportion()
	{
		TaiwuDomainMethod.AsyncCall.RequestTaiwuNeiliProportionDisplayData(null, delegate(int offset, RawDataPool pool)
		{
			TaiwuNeiliProportionDisplayData data = new TaiwuNeiliProportionDisplayData();
			Serializer.Deserialize(pool, offset, ref data);
			UI_GMWindow.Instance.Log(string.Format("five element delta: {0} {1} {2} {3} {4}", new object[]
			{
				data[0],
				data[1],
				data[2],
				data[3],
				data[4]
			}));
		});
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0005EAA5 File Offset: 0x0005CCA5
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	public static void LifeSkillCombatSetForceAiBribery(bool value)
	{
		TaiwuDomainMethod.Call.GmCmd_SetForceAiBribery(value);
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x0005EAAF File Offset: 0x0005CCAF
	// (set) Token: 0x06000FB1 RID: 4017 RVA: 0x0005EABB File Offset: 0x0005CCBB
	[GMProperty(EGMGroup.LifeSkill, 0.5f, 0.25f, 0, EWidgetType.Auto)]
	public static bool LifeSkillCombatShowHiddenInfo
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>().ShowHiddenInfo;
		}
		set
		{
			SingletonObject.getInstance<LifeSkillCombatModel>().ShowHiddenInfo = value;
			GEvent.OnEvent(UiEvents.ShowCombatLifeSkillHiddenInfo, null);
		}
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0005EADC File Offset: 0x0005CCDC
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LifeSkillCombatTalkIdField, 0.2f)]
	public static void LifeSkillCombatCharacterSpeak(short id)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("Id", id);
		GEvent.OnEvent(UiEvents.ShowCombatLifeSkillTalk, args);
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x0005EB0C File Offset: 0x0005CD0C
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LifeSkillCombatStrategyIdField, 0.1f)]
	[GMFuncArg(1, EWidgetType.LifeSkillCombatStrategyIdField, 0.1f)]
	public static void ShowUnlockedDebateStrategy(short start, short end)
	{
		TaiwuDomainMethod.Call.GmCmd_ShowUnlockedDebateStrategy(start, end);
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0005EB17 File Offset: 0x0005CD17
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.ProfessionSkillIdField, 0.1f)]
	public static void ShowUnlockedProfessionSkill(int id)
	{
		ExtraDomainMethod.Call.GmCmd_ShowUnlockedProfessionSkill(id);
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x0005EB21 File Offset: 0x0005CD21
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LifeSkillCombatStrategyIdField, 0.1f)]
	public static void GetDebateStrategyCard(int id)
	{
		TaiwuDomainMethod.Call.GmCmd_GetDebateStrategyCard(id);
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0005EB2B File Offset: 0x0005CD2B
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	public static void EmptyAiOwnedCard()
	{
		TaiwuDomainMethod.Call.GmCmd_EmptyAiOwnedCard();
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0005EB34 File Offset: 0x0005CD34
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LifeSkillCombatStrategyIdField, 0.1f)]
	public static void AddAiOwnedCard(int id)
	{
		TaiwuDomainMethod.Call.GmCmd_AddAiOwnedCard(id);
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x0005EB3E File Offset: 0x0005CD3E
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void ChangeStrategyPoint(bool isTaiwu, int delta)
	{
		TaiwuDomainMethod.Call.GmCmd_ChangeStrategyPoint(isTaiwu, delta);
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x0005EB49 File Offset: 0x0005CD49
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void ChangeBases(bool isTaiwu, int delta)
	{
		TaiwuDomainMethod.Call.GmCmd_ChangeBases(isTaiwu, delta);
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x0005EB54 File Offset: 0x0005CD54
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void ChangePressure(bool isTaiwu, int delta)
	{
		TaiwuDomainMethod.Call.GmCmd_ChangePressure(isTaiwu, delta);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x0005EB5F File Offset: 0x0005CD5F
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void ChangeGamePoint(bool isTaiwu, int delta)
	{
		TaiwuDomainMethod.Call.GmCmd_ChangeGamePoint(isTaiwu, delta);
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0005EB6C File Offset: 0x0005CD6C
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	public static void GetSpectatorIds()
	{
		foreach (CharacterDisplayData data in SingletonObject.getInstance<LifeSkillCombatModel>().SelfAudienceList)
		{
			UI_GMWindow.Instance.Log(string.Format("{0}({1})", data.CharacterId, NameCenter.GetMonasticTitleOrDisplayName(data, false)));
		}
		foreach (CharacterDisplayData data2 in SingletonObject.getInstance<LifeSkillCombatModel>().EnemyAudienceList)
		{
			UI_GMWindow.Instance.Log(string.Format("{0}({1})", data2.CharacterId, NameCenter.GetMonasticTitleOrDisplayName(data2, false)));
		}
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0005EC50 File Offset: 0x0005CE50
	[GMFunc(EGMGroup.LifeSkill, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.LifeSkillCombatNodeEffectIdField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	public static void AddNodeEffect(int templateId, int spectatorId)
	{
		bool flag = spectatorId < 0;
		if (flag)
		{
			UI_GMWindow.Instance.Log(string.Format("{0} is not a spectator in Debate!", spectatorId));
		}
		else
		{
			TaiwuDomainMethod.Call.GmCmd_AddNodeEffect((short)templateId, spectatorId);
		}
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x0005EC90 File Offset: 0x0005CE90
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	public static void PrintCurrentAdventureId()
	{
		AdventureRemakeModel adventureModel = SingletonObject.getInstance<AdventureRemakeModel>();
		bool notInAdventure = adventureModel.AdventureTaiwu.NotInAdventure;
		if (notInAdventure)
		{
			UI_GMWindow.Instance.Log("Need In Adventure");
		}
		else
		{
			UI_GMWindow.Instance.Log(string.Format("Current AdventureId:{0}", adventureModel.AdventureTaiwu.AdventureId));
		}
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x0005ECEB File Offset: 0x0005CEEB
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.15f)]
	[GMFuncArg(1, EWidgetType.StringField, 0.15f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.15f)]
	public static void AdventureRemakeChangeParameter(int adventureId, string key, int delta)
	{
		AdventureDomainMethod.Call.GmCmd_ChangeAdventureRemakeParameter(adventureId, key, delta);
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x0005ECF7 File Offset: 0x0005CEF7
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.AdventureRemakeIdField, 0.2f)]
	public static void GenerateAdventureRemake(int adventureId)
	{
		AdventureDomainMethod.Call.GmCmd_GenerateAdventureRemake(adventureId);
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x0005ED01 File Offset: 0x0005CF01
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ReGenerateAdventure()
	{
		AdventureDomainMethod.AsyncCall.GmCmd_ReGenerateAdventure(null, delegate(int offset, RawDataPool pool)
		{
			bool success = false;
			Serializer.Deserialize(pool, offset, ref success);
			UI_GMWindow.Instance.Log(success ? "ReGenerate successful.".SetColor(Color.green) : "ReGenerate failed.".SetColor(Color.red));
		});
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0005ED2A File Offset: 0x0005CF2A
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	public static void RemoveAllAdventures()
	{
		AdventureDomainMethod.Call.GmCmd_RemoveAllAdventures();
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0005ED34 File Offset: 0x0005CF34
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.12f)]
	[GMFuncArg(1, EWidgetType.ItemTypeIdField, 0.12f)]
	[GMFuncArg(2, EWidgetType.ItemIdField, 0.12f)]
	[GMFuncArg(3, EWidgetType.ItemIdField, 0.12f)]
	public static void AdventureAddTemporaryItem(int count, sbyte itemType, short idStar, short? idEnd)
	{
		AdventureRemakeModel adventureModel = SingletonObject.getInstance<AdventureRemakeModel>();
		bool notInAdventure = adventureModel.AdventureTaiwu.NotInAdventure;
		if (notInAdventure)
		{
			UI_GMWindow.Instance.Log("Need In Adventure");
		}
		else
		{
			bool flag = idEnd != null;
			if (flag)
			{
				idEnd = new short?(Math.Max(idStar, idEnd.Value));
				short itemId = idStar;
				for (;;)
				{
					int num = (int)itemId;
					short? num2 = idEnd;
					int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
					if (!(num <= num3.GetValueOrDefault() & num3 != null))
					{
						break;
					}
					AdventureDomainMethod.Call.GmCmd_AdventureAddTemporaryItem(count, itemType, itemId);
					itemId += 1;
				}
			}
			else
			{
				AdventureDomainMethod.Call.GmCmd_AdventureAddTemporaryItem(count, itemType, idStar);
			}
		}
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x0005EDF0 File Offset: 0x0005CFF0
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.StringField, 0.2f)]
	public static void QueryAdventureRemakeInArea(string name)
	{
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		AdventureRemakeModel adventureModel = SingletonObject.getInstance<AdventureRemakeModel>();
		string log = "";
		foreach (AdventureRuntime adventureRemake in adventureModel.AdventureRemakeDict.Values)
		{
			bool flag = adventureRemake.MapLocation.AreaId != mapModel.CurrentAreaId;
			if (!flag)
			{
				bool flag2 = !string.IsNullOrEmpty(name) && !adventureRemake.Core.Name.Contains(name);
				if (!flag2)
				{
					bool canEnter = adventureRemake.StatusType >= EAdventureStatusType.Ready;
					string statusText = adventureRemake.StatusType.ToString().SetColor(canEnter ? Color.green : Color.red);
					log += string.Format("{0} - {1} - {2}\n", adventureRemake.Core.Name, adventureRemake.MapLocation, statusText);
				}
			}
		}
		UI_GMWindow.Instance.Log(log);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0005EF1C File Offset: 0x0005D11C
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	public static void OpenAdventureEditorRemake()
	{
		bool flag = UIManager.Instance.IsFocusElement(UIElement.WorldMap);
		if (flag)
		{
			UIManager.Instance.StackToUI(UIElement.AdventureEditorRemake);
		}
		else
		{
			UI_GMWindow.Instance.Log("需要在大地图界面调用");
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x0005EF61 File Offset: 0x0005D161
	// (set) Token: 0x06000FC7 RID: 4039 RVA: 0x0005EF68 File Offset: 0x0005D168
	[GMProperty(EGMGroup.AdventureRemake, 0.25f, 0.25f, 0, EWidgetType.Auto)]
	public static bool AdventureRemakeShowAllElement
	{
		get
		{
			return GMFunc._adventureRemakeShowAllElement;
		}
		set
		{
			GMFunc._adventureRemakeShowAllElement = value;
			GEvent.OnEvent(UiEvents.OnAdventureTaiwuChanged, null);
		}
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x0005EF82 File Offset: 0x0005D182
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ShowGMGlobalParameterPanel()
	{
		UI_GMWindow.Instance.ShowGMGlobalParameterPanel();
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x0005EF90 File Offset: 0x0005D190
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ShowGMElementParameterPanel()
	{
		UI_GMWindow.Instance.ShowGMElementParameterPanel();
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0005EF9E File Offset: 0x0005D19E
	[GMFunc(EGMGroup.AdventureRemake, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.15f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.15f)]
	[GMFuncArg(2, EWidgetType.StringField, 0.15f)]
	[GMFuncArg(3, EWidgetType.IntField, 0.15f)]
	public static void AdventureRemakeChangeElementParameter(int adventureId, int elementId, string key, int delta)
	{
		AdventureDomainMethod.Call.GmCmd_ChangeAdventureRemakeElementParameter(adventureId, elementId, key, delta);
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0005EFAB File Offset: 0x0005D1AB
	[GMFunc(EGMGroup.AdventureMajorEvent, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.AdventureMajorEventIdField, 0.2f)]
	public static void GenerateAdventureMajorEvent(int adventureId)
	{
		AdventureDomainMethod.Call.GmCmd_GenerateAdventureMajorEvent(adventureId);
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0005EFB8 File Offset: 0x0005D1B8
	[GMFunc(EGMGroup.AdventureMajorEvent, 0.25f, 1000, null, GmRunMode.Default)]
	public static void OpenAdventureMajorEventEditor()
	{
		bool flag = UIManager.Instance.IsFocusElement(UIElement.WorldMap);
		if (flag)
		{
			UIManager.Instance.StackToUI(UIElement.AdventureMajorEventEditor);
		}
		else
		{
			UI_GMWindow.Instance.Log("需要在大地图界面调用");
		}
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0005EFFD File Offset: 0x0005D1FD
	[GMFunc(EGMGroup.CharacterBase, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.BoolField, 0.2f)]
	[GMFuncArg(1, EWidgetType.BoolField, 0.2f)]
	public static void CollectLog(bool captureScreen, bool compression)
	{
		LogCollector.CollectLog(null, captureScreen, compression);
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0005F00C File Offset: 0x0005D20C
	[GMFunc(EGMGroup.MapWorldFunction, 0.4f, 1000, null, GmRunMode.Default)]
	public static void QueryTaiwuLifeSummaries()
	{
		DataUid uid = new DataUid(5, 104, ulong.MaxValue, uint.MaxValue);
		UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TaiwuLifeSummary summaries = (TaiwuLifeSummary)data[uid];
			foreach (KeyValuePair<string, int> keyValuePair in TaiwuLifeSummaryType.Instance.RefNameMap)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string refName = text;
				int templateId = num;
				int value = summaries.Get(templateId);
				bool flag = value > 0;
				if (flag)
				{
					stringBuilder.AppendFormat("{0}: {1}\n", refName, value.ToString());
				}
			}
			UI_GMWindow.Instance.Log(stringBuilder.ToString());
		}, new ValueTuple<DataUid, Type>[]
		{
			new ValueTuple<DataUid, Type>(uid, typeof(TaiwuLifeSummary))
		});
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x0005F06A File Offset: 0x0005D26A
	[GMFunc(EGMGroup.SystemGlobal, 0.25f, 1000, null, GmRunMode.Default)]
	public static void PlayCGVideo(string cgName)
	{
		GameApp.PlayCg(cgName, null);
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x0005F078 File Offset: 0x0005D278
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	[GMFuncArg(0, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(1, EWidgetType.IntField, 0.2f)]
	[GMFuncArg(2, EWidgetType.IntField, 0.2f)]
	public static void ShowUnlockSkillSlotAnim(int combatSkillEquipType, int slotCount, int neiliCount)
	{
		UIElement.GameLineAnim.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("DisplayEventType", 145).Set("CombatSkillEquipType", combatSkillEquipType).Set("SlotCount", slotCount).Set("NeiliCount", neiliCount));
		UIManager.Instance.MaskUI(UIElement.GameLineAnim);
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0005F0D6 File Offset: 0x0005D2D6
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetIconPlateIsUnlocked(bool isUnlocked)
	{
		StoryDomainMethod.Call.GmCmd_SetIconPlateIsUnlocked(isUnlocked);
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x0005F0E0 File Offset: 0x0005D2E0
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ClearIronPlateCooldown()
	{
		StoryDomainMethod.Call.GmCmd_ClearIronPlateCooldown();
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x0005F0E9 File Offset: 0x0005D2E9
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	public static void SetDivineFlameIsUnlocked(bool isUnlocked)
	{
		StoryDomainMethod.Call.GmCmd_SetDivineFlameIsUnlocked(isUnlocked);
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x0005F0F3 File Offset: 0x0005D2F3
	[GMFunc(EGMGroup.MapWorldFunction, 0.25f, 1000, null, GmRunMode.Default)]
	public static void ClearDivineFlameCooldown()
	{
		StoryDomainMethod.Call.GmCmd_ClearDivineFlameCooldown();
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0005F0FC File Offset: 0x0005D2FC
	public static void Reset()
	{
		GMFunc._lockTime = false;
		GMFunc._teleportMove = false;
		GMFunc.CombatSkillCharId = -1;
		GMFunc.CombatSkillTarget = string.Empty;
		GMFunc.LifeSkillCharId = -1;
		UI_GMWindow instance = UI_GMWindow.Instance;
		if (instance != null)
		{
			instance.Reset();
		}
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x0005F170 File Offset: 0x0005D370
	[CompilerGenerated]
	internal static void <KillCharacter>g__OnKillConfirm|14_0()
	{
		bool exist = UIElement.EventWindow.Exist;
		if (exist)
		{
			UIManager.Instance.HideUI(UIElement.EventWindow);
		}
		bool exist2 = UIElement.CharacterMenu.Exist;
		if (exist2)
		{
			UIManager.Instance.StackBack(null);
		}
	}

	// Token: 0x04000E71 RID: 3697
	public static IEnumerator AdvanceMonthCoroutine = null;

	// Token: 0x04000E72 RID: 3698
	public static bool DisableAutoSaving = false;

	// Token: 0x04000E73 RID: 3699
	private static bool _lockTime;

	// Token: 0x04000E75 RID: 3701
	private static bool _teleportMove;

	// Token: 0x04000E76 RID: 3702
	private static bool _isFinalBossDefeated = false;

	// Token: 0x04000E77 RID: 3703
	private static bool _lastWorldFunctionsStatusesLoaded = false;

	// Token: 0x04000E78 RID: 3704
	private static ulong _lastWorldFunctionsStatuses = 0UL;

	// Token: 0x04000E7A RID: 3706
	private static sbyte _yuanShanExitWithStage = -1;

	// Token: 0x04000E7B RID: 3707
	public static sbyte OvercomeCombatResultType = -1;

	// Token: 0x04000E7C RID: 3708
	public static sbyte OvercomeLifeSkillCombatResultType = -1;

	// Token: 0x04000E7D RID: 3709
	public static bool IgnoreProfessionSkillUnlockAnimation;

	// Token: 0x04000E80 RID: 3712
	private static int _lifeSkillCharId = -1;

	// Token: 0x04000E81 RID: 3713
	private static bool _adventureRemakeShowAllElement;
}
