using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Combat.MixPoison;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200011A RID: 282
public class CombatSubProcessorCharacter : CombatSubProcessor, ICombatNotifySubProcessor
{
	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000A04 RID: 2564 RVA: 0x00041D3A File Offset: 0x0003FF3A
	public int CharacterId { get; }

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000A05 RID: 2565 RVA: 0x00041D42 File Offset: 0x0003FF42
	ulong ICombatNotifySubProcessor.SubId0
	{
		get
		{
			return (ulong)((long)this.CharacterId);
		}
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00041D4C File Offset: 0x0003FF4C
	public CombatSubProcessorCharacter(int characterId)
	{
		this.CharacterId = characterId;
		base.Setup();
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00041DA0 File Offset: 0x0003FFA0
	protected override void OnSetup()
	{
		base.OnSetup();
		bool flag = this.Display != null;
		if (flag)
		{
			this.Display.Setup();
		}
		else
		{
			this.Display = new CombatSubProcessorCharacterDisplay(this.CharacterId);
		}
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00041DE2 File Offset: 0x0003FFE2
	protected override void OnClose()
	{
		this.Display.Close();
		base.OnClose();
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000A09 RID: 2569 RVA: 0x00041DF8 File Offset: 0x0003FFF8
	private bool IsAlly
	{
		get
		{
			return CombatSubProcessor.Model.CharIsAlly(this.CharacterId);
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000A0A RID: 2570 RVA: 0x00041E0A File Offset: 0x0004000A
	// (set) Token: 0x06000A0B RID: 2571 RVA: 0x00041E12 File Offset: 0x00040012
	public CombatSubProcessorCharacterDisplay Display { get; private set; }

	// Token: 0x06000A0C RID: 2572 RVA: 0x00041E1B File Offset: 0x0004001B
	public IEnumerable<short> IterCombatSkillIds()
	{
		bool flag = this.NeigongList != null;
		if (flag)
		{
			foreach (short skillId in this.NeigongList)
			{
				yield return skillId;
			}
			List<short>.Enumerator enumerator = default(List<short>.Enumerator);
		}
		bool flag2 = this.AttackSkillList != null;
		if (flag2)
		{
			foreach (short skillId2 in this.AttackSkillList)
			{
				yield return skillId2;
			}
			List<short>.Enumerator enumerator2 = default(List<short>.Enumerator);
		}
		bool flag3 = this.AssistSkillList != null;
		if (flag3)
		{
			foreach (short skillId3 in this.AssistSkillList)
			{
				yield return skillId3;
			}
			List<short>.Enumerator enumerator3 = default(List<short>.Enumerator);
		}
		yield break;
		yield break;
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00041E2C File Offset: 0x0004002C
	public bool GetTeammateCmdCanUse(int commandIndex)
	{
		List<sbyte> currTeammateCommands = this.CurrTeammateCommands;
		bool flag = currTeammateCommands == null || currTeammateCommands.Count <= commandIndex;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			List<bool> canUseList = this.TeammateCommandCanUse;
			bool flag2 = canUseList == null || canUseList.Count <= commandIndex;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool isAlly = CombatSubProcessor.Model.CharIsAlly(this.CharacterId);
				CombatSubProcessorCharacter orDefault = CombatSubProcessor.Model.ProcessorCharacters.GetOrDefault(CombatSubProcessor.Model.MainCharId(isAlly));
				sbyte cmdType = (orDefault != null && orDefault.ShowTransferInjuryCommand) ? 13 : currTeammateCommands[commandIndex];
				TeammateCommandItem cmdConfig = TeammateCommand.Instance[cmdType];
				result = (cmdConfig.Type != ETeammateCommandType.Negative && canUseList[commandIndex]);
			}
		}
		return result;
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00041EF8 File Offset: 0x000400F8
	public int FindTeammateCmdIndex(ETeammateCommandImplement implement)
	{
		bool flag = this.CurrTeammateCommands == null;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			List<sbyte> cmdTypes = this.CurrTeammateCommands;
			bool isAlly = CombatSubProcessor.Model.CharIsAlly(this.CharacterId);
			CombatSubProcessorCharacter orDefault = CombatSubProcessor.Model.ProcessorCharacters.GetOrDefault(CombatSubProcessor.Model.MainCharId(isAlly));
			bool showTransferInjuryCommand = orDefault != null && orDefault.ShowTransferInjuryCommand;
			bool flag2 = showTransferInjuryCommand;
			if (flag2)
			{
				result = ((implement == ETeammateCommandImplement.TransferInjury) ? 0 : -1);
			}
			else
			{
				for (int i = 0; i < cmdTypes.Count; i++)
				{
					ETeammateCommandImplement cmdImplement = (cmdTypes[i] < 0) ? ETeammateCommandImplement.Invalid : TeammateCommand.Instance[cmdTypes[i]].Implement;
					bool flag3 = cmdImplement == implement;
					if (flag3)
					{
						return i;
					}
				}
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00041FCC File Offset: 0x000401CC
	[CombatNotifyData(4, 0, 111U)]
	private void HandlerDataNeiliType(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.NeiliType);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onNeiliTypeChanged = CombatSubProcessor.Model.OnNeiliTypeChanged;
			if (onNeiliTypeChanged != null)
			{
				onNeiliTypeChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00042030 File Offset: 0x00040230
	[CombatNotifyData(4, 0, 45U)]
	private void HandlerDataCurrNeili(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrNeili);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onCurrNeiliChanged = CombatSubProcessor.Model.OnCurrNeiliChanged;
			if (onCurrNeiliChanged != null)
			{
				onCurrNeiliChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00042094 File Offset: 0x00040294
	[CombatNotifyData(4, 0, 108U)]
	private void HandlerDataMaxNeili(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MaxNeili);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onMaxNeiliChanged = CombatSubProcessor.Model.OnMaxNeiliChanged;
			if (onMaxNeiliChanged != null)
			{
				onMaxNeiliChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x000420F8 File Offset: 0x000402F8
	[CombatNotifyData(4, 0, 19U)]
	private void HandlerDataHealth(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Health);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHealthChanged = CombatSubProcessor.Model.OnHealthChanged;
			if (onHealthChanged != null)
			{
				onHealthChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0004215C File Offset: 0x0004035C
	[CombatNotifyData(4, 0, 28U)]
	private void HandlerDataConsummateLevel(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ConsummateLevel);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onConsummateLevelChanged = CombatSubProcessor.Model.OnConsummateLevelChanged;
			if (onConsummateLevelChanged != null)
			{
				onConsummateLevelChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x000421C0 File Offset: 0x000403C0
	[CombatNotifyData(4, 0, 85U)]
	private void HandlerDataMoveSpeed(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MoveSpeed);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onMoveSpeedChanged = CombatSubProcessor.Model.OnMoveSpeedChanged;
			if (onMoveSpeedChanged != null)
			{
				onMoveSpeedChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00042224 File Offset: 0x00040424
	[CombatNotifyData(4, 0, 91U)]
	private void HandlerDataInnerRatio(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.InnerRatio);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onInnerRatioChanged = CombatSubProcessor.Model.OnInnerRatioChanged;
			if (onInnerRatioChanged != null)
			{
				onInnerRatioChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00042287 File Offset: 0x00040487
	[CombatNotifyData(4, 0, 89U)]
	private void HandlerDataWeaponSwitchSpeed(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.WeaponSwitchSpeed);
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00042298 File Offset: 0x00040498
	[CombatNotifyData(8, 10, 19U)]
	private void HandlerDataWeapons(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Weapons);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onWeaponsChanged = CombatSubProcessor.Model.OnWeaponsChanged;
			if (onWeaponsChanged != null)
			{
				onWeaponsChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x000422FC File Offset: 0x000404FC
	[CombatNotifyData(8, 10, 16U)]
	private void HandlerDataUsingWeaponIndex(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.UsingWeaponIndex);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onUsingWeaponIndexChanged = CombatSubProcessor.Model.OnUsingWeaponIndexChanged;
			if (onUsingWeaponIndexChanged != null)
			{
				onUsingWeaponIndexChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00042360 File Offset: 0x00040560
	[CombatNotifyData(8, 10, 51U)]
	private void HandlerDataNeigongList(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.NeigongList);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onNeigongListChanged = CombatSubProcessor.Model.OnNeigongListChanged;
			if (onNeigongListChanged != null)
			{
				onNeigongListChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x000423C4 File Offset: 0x000405C4
	[CombatNotifyData(8, 10, 52U)]
	private void HandlerDataAttackSkillList(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AttackSkillList);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onAttackSkillListChanged = CombatSubProcessor.Model.OnAttackSkillListChanged;
			if (onAttackSkillListChanged != null)
			{
				onAttackSkillListChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00042428 File Offset: 0x00040628
	[CombatNotifyData(8, 10, 55U)]
	private void HandlerDataAssistSkillList(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AssistSkillList);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onAssistSkillListChanged = CombatSubProcessor.Model.OnAssistSkillListChanged;
			if (onAssistSkillListChanged != null)
			{
				onAssistSkillListChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0004248B File Offset: 0x0004068B
	[CombatNotifyData(4, 0, 1U)]
	private void HandlerDataTemplateId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TemplateId);
		OnCharacterDataChangedEvent onCharacterTemplateIdChanged = CombatSubProcessor.Model.OnCharacterTemplateIdChanged;
		if (onCharacterTemplateIdChanged != null)
		{
			onCharacterTemplateIdChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x000424B8 File Offset: 0x000406B8
	[CombatNotifyData(4, 0, 114U)]
	private void HandlerDataAllocatedNeiliEffects(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AllocatedNeiliEffects);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onAllocatedNeiliEffectsChanged = CombatSubProcessor.Model.OnAllocatedNeiliEffectsChanged;
			if (onAllocatedNeiliEffectsChanged != null)
			{
				onAllocatedNeiliEffectsChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0004251C File Offset: 0x0004071C
	[CombatNotifyData(4, 0, 21U)]
	private void HandlerDataDisorderOfQi(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.DisorderOfQi);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onDisorderOfQiChanged = CombatSubProcessor.Model.OnDisorderOfQiChanged;
			if (onDisorderOfQiChanged != null)
			{
				onDisorderOfQiChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00042580 File Offset: 0x00040780
	[CombatNotifyData(8, 10, 3U)]
	private void HandlerDataNeiliAllocation(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.NeiliAllocation);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onNeiliAllocationChanged = CombatSubProcessor.Model.OnNeiliAllocationChanged;
			if (onNeiliAllocationChanged != null)
			{
				onNeiliAllocationChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x000425E4 File Offset: 0x000407E4
	[CombatNotifyData(8, 10, 4U)]
	private void HandlerDataOriginNeiliAllocation(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OriginNeiliAllocation);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOriginNeiliAllocationChanged = CombatSubProcessor.Model.OnOriginNeiliAllocationChanged;
			if (onOriginNeiliAllocationChanged != null)
			{
				onOriginNeiliAllocationChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00042648 File Offset: 0x00040848
	[CombatNotifyData(8, 10, 117U)]
	private void HandlerDataNeiliAllocationCd(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.NeiliAllocationCd);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onDataNeiliAllocationCdChanged = CombatSubProcessor.Model.OnDataNeiliAllocationCdChanged;
			if (onDataNeiliAllocationCdChanged != null)
			{
				onDataNeiliAllocationCdChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x000426AC File Offset: 0x000408AC
	[CombatNotifyData(8, 10, 1U)]
	private void HandlerDataBreathValue(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.BreathValue);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onBreathValueChanged = CombatSubProcessor.Model.OnBreathValueChanged;
			if (onBreathValueChanged != null)
			{
				onBreathValueChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x00042710 File Offset: 0x00040910
	[CombatNotifyData(8, 10, 2U)]
	private void HandlerDataStanceValue(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.StanceValue);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onStanceValueChanged = CombatSubProcessor.Model.OnStanceValueChanged;
			if (onStanceValueChanged != null)
			{
				onStanceValueChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x00042773 File Offset: 0x00040973
	[CombatNotifyData(8, 10, 11U)]
	private void HandlerDataMobilityValue(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MobilityValue);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x00042784 File Offset: 0x00040984
	[CombatNotifyData(8, 10, 65U)]
	private void HandlerDataWugCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.WugCount);
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00042798 File Offset: 0x00040998
	[CombatNotifyData(8, 10, 123U)]
	private void HandlerDataNormalAttackRecovery(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.NormalAttackRecovery);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onNormalAttackRecoveryChanged = CombatSubProcessor.Model.OnNormalAttackRecoveryChanged;
			if (onNormalAttackRecoveryChanged != null)
			{
				onNormalAttackRecoveryChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x000427FC File Offset: 0x000409FC
	[CombatNotifyData(8, 10, 138U)]
	private void HandlerDataAttackRange(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AttackRange);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onAttackRangeChanged = CombatSubProcessor.Model.OnAttackRangeChanged;
			if (onAttackRangeChanged != null)
			{
				onAttackRangeChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0004285F File Offset: 0x00040A5F
	[CombatNotifyData(8, 10, 127U)]
	private void HandlerDataMoveState(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MoveState);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x00042870 File Offset: 0x00040A70
	[CombatNotifyData(8, 10, 128U)]
	private void HandlerDataPlayerControllingMove(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.PlayerControllingMove);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00042884 File Offset: 0x00040A84
	[CombatNotifyData(8, 10, 17U)]
	private void HandlerDataWeaponTricks(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.WeaponTricks);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onWeaponTricksChanged = CombatSubProcessor.Model.OnWeaponTricksChanged;
			if (onWeaponTricksChanged != null)
			{
				onWeaponTricksChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x000428E8 File Offset: 0x00040AE8
	[CombatNotifyData(8, 10, 18U)]
	private void HandlerDataWeaponTrickIndex(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.WeaponTrickIndex);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onWeaponTrickIndexChanged = CombatSubProcessor.Model.OnWeaponTrickIndexChanged;
			if (onWeaponTrickIndexChanged != null)
			{
				onWeaponTrickIndexChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0004294C File Offset: 0x00040B4C
	[CombatNotifyData(8, 10, 28U)]
	private void HandlerDataTricks(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Tricks);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onTricksChanged = CombatSubProcessor.Model.OnTricksChanged;
			if (onTricksChanged != null)
			{
				onTricksChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x000429B0 File Offset: 0x00040BB0
	[CombatNotifyData(8, 10, 134U)]
	private void HandlerDataMaxTrickCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MaxTrickCount);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onMaxTrickCountChanged = CombatSubProcessor.Model.OnMaxTrickCountChanged;
			if (onMaxTrickCountChanged != null)
			{
				onMaxTrickCountChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00042A14 File Offset: 0x00040C14
	[CombatNotifyData(8, 10, 23U)]
	private void HandlerDataChangeTrickCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ChangeTrickCount);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onChangeTrickCountChanged = CombatSubProcessor.Model.OnChangeTrickCountChanged;
			if (onChangeTrickCountChanged != null)
			{
				onChangeTrickCountChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00042A78 File Offset: 0x00040C78
	[CombatNotifyData(8, 10, 22U)]
	private void HandlerDataChangeTrickProgress(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ChangeTrickProgress);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onChangeTrickProgressChanged = CombatSubProcessor.Model.OnChangeTrickProgressChanged;
			if (onChangeTrickProgressChanged != null)
			{
				onChangeTrickProgressChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00042ADC File Offset: 0x00040CDC
	[CombatNotifyData(8, 10, 24U)]
	private void HandlerDataCanChangeTrick(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CanChangeTrick);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onCanChangeTrickChanged = CombatSubProcessor.Model.OnCanChangeTrickChanged;
			if (onCanChangeTrickChanged != null)
			{
				onCanChangeTrickChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x00042B40 File Offset: 0x00040D40
	[CombatNotifyData(8, 10, 25U)]
	private void HandlerDataChangingTrick(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ChangingTrick);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onChangingTrickChanged = CombatSubProcessor.Model.OnChangingTrickChanged;
			if (onChangingTrickChanged != null)
			{
				onChangingTrickChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00042BA4 File Offset: 0x00040DA4
	[CombatNotifyData(8, 10, 26U)]
	private void HandlerDataChangeTrickAttack(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ChangeTrickAttack);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onChangeTrickAttackChanged = CombatSubProcessor.Model.OnChangeTrickAttackChanged;
			if (onChangeTrickAttackChanged != null)
			{
				onChangeTrickAttackChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00042C08 File Offset: 0x00040E08
	[CombatNotifyData(8, 10, 124U)]
	private void HandlerDataReserveNormalAttack(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ReserveNormalAttack);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onReserveNormalAttack = CombatSubProcessor.Model.OnReserveNormalAttack;
			if (onReserveNormalAttack != null)
			{
				onReserveNormalAttack(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x00042C6C File Offset: 0x00040E6C
	[CombatNotifyData(8, 10, 79U)]
	private void HandlerDataSkillEffectCollection(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SkillEffectCollection);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onSkillEffectCollectionChanged = CombatSubProcessor.Model.OnSkillEffectCollectionChanged;
			if (onSkillEffectCollectionChanged != null)
			{
				onSkillEffectCollectionChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x00042CD0 File Offset: 0x00040ED0
	[CombatNotifyData(8, 10, 56U)]
	private void HandlerDataPreparingSkillId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.PreparingSkillId);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onPreparingSkillIdChanged = CombatSubProcessor.Model.OnPreparingSkillIdChanged;
			if (onPreparingSkillIdChanged != null)
			{
				onPreparingSkillIdChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00042D34 File Offset: 0x00040F34
	[CombatNotifyData(8, 10, 57U)]
	private void HandlerDataSkillPreparePercent(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.SkillPreparePercent);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onSkillPreparePercentChanged = CombatSubProcessor.Model.OnSkillPreparePercentChanged;
			if (onSkillPreparePercentChanged != null)
			{
				onSkillPreparePercentChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00042D98 File Offset: 0x00040F98
	[CombatNotifyData(8, 10, 58U)]
	private void HandlerDataPerformingSkillId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.PerformingSkillId);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onPerformingSkillIdChanged = CombatSubProcessor.Model.OnPerformingSkillIdChanged;
			if (onPerformingSkillIdChanged != null)
			{
				onPerformingSkillIdChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00042DFC File Offset: 0x00040FFC
	[CombatNotifyData(8, 10, 59U)]
	private void HandlerDataAutoCastingSkill(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AutoCastingSkill);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onAutoCastingSkillChanged = CombatSubProcessor.Model.OnAutoCastingSkillChanged;
			if (onAutoCastingSkillChanged != null)
			{
				onAutoCastingSkillChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00042E5F File Offset: 0x0004105F
	[CombatNotifyData(8, 10, 63U)]
	private void HandlerDataAffectingDefendSkillId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AffectingDefendSkillId);
		CombatSubProcessor.Model.RaiseEvent(ECombatEvents.OnAffectingDefendSkillIdChanged);
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00042E80 File Offset: 0x00041080
	[CombatNotifyData(8, 10, 69U)]
	private void HandlerDataPreparingOtherAction(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.PreparingOtherAction);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onPreparingOtherActionChanged = CombatSubProcessor.Model.OnPreparingOtherActionChanged;
			if (onPreparingOtherActionChanged != null)
			{
				onPreparingOtherActionChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00042EE4 File Offset: 0x000410E4
	[CombatNotifyData(8, 10, 150U)]
	private void HandlerDataPreparingOtherActionCanInterrupt(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.PreparingOtherActionInterruptType);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onPreparingOtherActionInterruptTypeChanged = CombatSubProcessor.Model.OnPreparingOtherActionInterruptTypeChanged;
			if (onPreparingOtherActionInterruptTypeChanged != null)
			{
				onPreparingOtherActionInterruptTypeChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00042F48 File Offset: 0x00041148
	[CombatNotifyData(8, 10, 70U)]
	private void HandlerDataOtherActionPreparePercent(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OtherActionPreparePercent);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOtherActionPreparePercentChanged = CombatSubProcessor.Model.OnOtherActionPreparePercentChanged;
			if (onOtherActionPreparePercentChanged != null)
			{
				onOtherActionPreparePercentChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x00042FAC File Offset: 0x000411AC
	[CombatNotifyData(8, 10, 68U)]
	private void HandlerDataOtherActionCanUse(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OtherActionCanUse);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOtherActionCanUseChanged = CombatSubProcessor.Model.OnOtherActionCanUseChanged;
			if (onOtherActionCanUseChanged != null)
			{
				onOtherActionCanUseChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00043010 File Offset: 0x00041210
	[CombatNotifyData(8, 10, 66U)]
	private void HandlerDataHealInjuryCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HealInjuryCount);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHealInjuryCountChanged = CombatSubProcessor.Model.OnHealInjuryCountChanged;
			if (onHealInjuryCountChanged != null)
			{
				onHealInjuryCountChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x00043074 File Offset: 0x00041274
	[CombatNotifyData(8, 10, 67U)]
	private void HandlerDataHealPoisonCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HealPoisonCount);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHealPoisonCountChanged = CombatSubProcessor.Model.OnHealPoisonCountChanged;
			if (onHealPoisonCountChanged != null)
			{
				onHealPoisonCountChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x000430D8 File Offset: 0x000412D8
	[CombatNotifyData(8, 10, 72U)]
	private void HandlerDataCanUseItem(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CanUseItem);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onCanUseItemChanged = CombatSubProcessor.Model.OnCanUseItemChanged;
			if (onCanUseItemChanged != null)
			{
				onCanUseItemChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0004313C File Offset: 0x0004133C
	[CombatNotifyData(8, 10, 73U)]
	private void HandlerDataPreparingItem(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.PreparingItem);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onPreparingItemChanged = CombatSubProcessor.Model.OnPreparingItemChanged;
			if (onPreparingItemChanged != null)
			{
				onPreparingItemChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x000431A0 File Offset: 0x000413A0
	[CombatNotifyData(8, 10, 74U)]
	private void HandlerDataUseItemPreparePercent(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.UseItemPreparePercent);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onUseItemPreparePercentChanged = CombatSubProcessor.Model.OnUseItemPreparePercentChanged;
			if (onUseItemPreparePercentChanged != null)
			{
				onUseItemPreparePercentChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x00043203 File Offset: 0x00041403
	[CombatNotifyData(8, 10, 148U)]
	private void HandlerDataUseItemCostNoWisdom(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.UseItemCostNoWisdom);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x00043214 File Offset: 0x00041414
	[CombatNotifyData(8, 10, 50U)]
	private void HandlerDataDefeatMarkCollection(RawDataPool pool, int offset)
	{
		DefeatMarkCollection oldValue = this.DefeatMarkCollection;
		this.DefeatMarkCollection = EasyPool.Get<DefeatMarkCollection>();
		Serializer.Deserialize(pool, offset, ref this.DefeatMarkCollection);
		OnCharacterDataChangedEvent<DefeatMarkCollection> onDefeatMarkCollectionChanged = CombatSubProcessor.Model.OnDefeatMarkCollectionChanged;
		if (onDefeatMarkCollectionChanged != null)
		{
			onDefeatMarkCollectionChanged(this.CharacterId, oldValue);
		}
		EasyPool.Free<DefeatMarkCollection>(oldValue);
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00043266 File Offset: 0x00041466
	[CombatNotifyData(8, 10, 102U)]
	private void HandlerDataCurrTeammateCommands(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrTeammateCommands);
		OnCharacterDataChangedEvent onCurrTeammateCommandsChanged = CombatSubProcessor.Model.OnCurrTeammateCommandsChanged;
		if (onCurrTeammateCommandsChanged != null)
		{
			onCurrTeammateCommandsChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00043293 File Offset: 0x00041493
	[CombatNotifyData(8, 10, 136U)]
	private void HandlerDataTeammateCommandCanUse(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TeammateCommandCanUse);
		OnCharacterDataChangedEvent onTeammateCommandCanUseChanged = CombatSubProcessor.Model.OnTeammateCommandCanUseChanged;
		if (onTeammateCommandCanUseChanged != null)
		{
			onTeammateCommandCanUseChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x000432C0 File Offset: 0x000414C0
	[CombatNotifyData(8, 10, 101U)]
	private void HandlerDataShowTransferInjuryCommand(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ShowTransferInjuryCommand);
		OnCharacterDataChangedEvent onShowTransferInjuryCommandChanged = CombatSubProcessor.Model.OnShowTransferInjuryCommandChanged;
		if (onShowTransferInjuryCommandChanged != null)
		{
			onShowTransferInjuryCommandChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x000432ED File Offset: 0x000414ED
	[CombatNotifyData(8, 10, 112U)]
	private void HandlerDataTeammateCommandBanReasons(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TeammateCommandBanReasons);
		OnCharacterDataChangedEvent onTeammateCommandBanReasonsChanged = CombatSubProcessor.Model.OnTeammateCommandBanReasonsChanged;
		if (onTeammateCommandBanReasonsChanged != null)
		{
			onTeammateCommandBanReasonsChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0004331C File Offset: 0x0004151C
	[CombatNotifyData(8, 10, 103U)]
	private void HandlerDataExecutingTeammateCommand(RawDataPool pool, int offset)
	{
		sbyte oldValue = this.ExecutingTeammateCommand;
		Serializer.Deserialize(pool, offset, ref this.ExecutingTeammateCommand);
		OnCharacterDataChangedEvent<sbyte> onExecutingTeammateCommandChanged = CombatSubProcessor.Model.OnExecutingTeammateCommandChanged;
		if (onExecutingTeammateCommandChanged != null)
		{
			onExecutingTeammateCommandChanged(this.CharacterId, oldValue);
		}
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0004335C File Offset: 0x0004155C
	[CombatNotifyData(8, 10, 132U)]
	private void HandlerDataTeammateCommandCd(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TeammateCommandCd);
		OnCharacterDataChangedEvent onTeammateCommandCdChanged = CombatSubProcessor.Model.OnTeammateCommandCdChanged;
		if (onTeammateCommandCdChanged != null)
		{
			onTeammateCommandCdChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x00043389 File Offset: 0x00041589
	[CombatNotifyData(8, 10, 105U)]
	private void HandlerDataTeammateCommandPreparePercent(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TeammateCommandPreparePercent);
		OnCharacterDataChangedEvent onTeammateCommandPreparePercentChanged = CombatSubProcessor.Model.OnTeammateCommandPreparePercentChanged;
		if (onTeammateCommandPreparePercentChanged != null)
		{
			onTeammateCommandPreparePercentChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x000433B6 File Offset: 0x000415B6
	[CombatNotifyData(8, 10, 106U)]
	private void HandlerDataTeammateCommandTimePercent(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TeammateCommandTimePercent);
		OnCharacterDataChangedEvent onTeammateCommandTimePercentChanged = CombatSubProcessor.Model.OnTeammateCommandTimePercentChanged;
		if (onTeammateCommandTimePercentChanged != null)
		{
			onTeammateCommandTimePercentChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x000433E3 File Offset: 0x000415E3
	[CombatNotifyData(8, 10, 107U)]
	private void HandlerDataAttackCommandWeaponKey(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AttackCommandWeaponKey);
		OnCharacterDataChangedEvent onAttackCommandWeaponKeyChanged = CombatSubProcessor.Model.OnAttackCommandWeaponKeyChanged;
		if (onAttackCommandWeaponKeyChanged != null)
		{
			onAttackCommandWeaponKeyChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x00043410 File Offset: 0x00041610
	[CombatNotifyData(8, 10, 108U)]
	private void HandlerDataAttackCommandTrickType(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AttackCommandTrickType);
		OnCharacterDataChangedEvent onAttackCommandTrickTypeChanged = CombatSubProcessor.Model.OnAttackCommandTrickTypeChanged;
		if (onAttackCommandTrickTypeChanged != null)
		{
			onAttackCommandTrickTypeChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0004343D File Offset: 0x0004163D
	[CombatNotifyData(8, 10, 111U)]
	private void HandlerDataAttackCommandSkillId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AttackCommandSkillId);
		OnCharacterDataChangedEvent onAttackCommandSkillIdChanged = CombatSubProcessor.Model.OnAttackCommandSkillIdChanged;
		if (onAttackCommandSkillIdChanged != null)
		{
			onAttackCommandSkillIdChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0004346A File Offset: 0x0004166A
	[CombatNotifyData(8, 10, 109U)]
	private void HandlerDataDefendCommandSkillId(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.DefendCommandSkillId);
		OnCharacterDataChangedEvent onDefendCommandSkillIdChanged = CombatSubProcessor.Model.OnDefendCommandSkillIdChanged;
		if (onDefendCommandSkillIdChanged != null)
		{
			onDefendCommandSkillIdChanged(this.CharacterId);
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00043498 File Offset: 0x00041698
	[CombatNotifyData(4, 0, 22U)]
	private void HandlerDataHaveLeftArm(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HaveLeftArm);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHaveLeftArmChanged = CombatSubProcessor.Model.OnHaveLeftArmChanged;
			if (onHaveLeftArmChanged != null)
			{
				onHaveLeftArmChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x000434FC File Offset: 0x000416FC
	[CombatNotifyData(4, 0, 23U)]
	private void HandlerDataHaveRightArm(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HaveRightArm);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHaveRightArmChanged = CombatSubProcessor.Model.OnHaveRightArmChanged;
			if (onHaveRightArmChanged != null)
			{
				onHaveRightArmChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00043560 File Offset: 0x00041760
	[CombatNotifyData(4, 0, 24U)]
	private void HandlerDataHaveLeftLeg(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HaveLeftLeg);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHaveLeftLegChanged = CombatSubProcessor.Model.OnHaveLeftLegChanged;
			if (onHaveLeftLegChanged != null)
			{
				onHaveLeftLegChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x000435C4 File Offset: 0x000417C4
	[CombatNotifyData(4, 0, 25U)]
	private void HandlerDataHaveRightLeg(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HaveRightLeg);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHaveRightLegChanged = CombatSubProcessor.Model.OnHaveRightLegChanged;
			if (onHaveRightLegChanged != null)
			{
				onHaveRightLegChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00043628 File Offset: 0x00041828
	[CombatNotifyData(4, 0, 80U)]
	private void HandlerDataHitValues(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.HitValues);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onHitValuesChanged = CombatSubProcessor.Model.OnHitValuesChanged;
			if (onHitValuesChanged != null)
			{
				onHitValuesChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0004368C File Offset: 0x0004188C
	[CombatNotifyData(4, 0, 82U)]
	private void HandlerDataAvoidValues(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.AvoidValues);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onAvoidValuesChanged = CombatSubProcessor.Model.OnAvoidValuesChanged;
			if (onAvoidValuesChanged != null)
			{
				onAvoidValuesChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x000436F0 File Offset: 0x000418F0
	[CombatNotifyData(8, 10, 33U)]
	private void HandlerDataOuterDamageValue(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OuterDamageValue);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOuterDamageValueChanged = CombatSubProcessor.Model.OnOuterDamageValueChanged;
			if (onOuterDamageValueChanged != null)
			{
				onOuterDamageValueChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00043754 File Offset: 0x00041954
	[CombatNotifyData(8, 10, 34U)]
	private void HandlerDataInnerDamageValue(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.InnerDamageValue);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onInnerDamageValueChanged = CombatSubProcessor.Model.OnInnerDamageValueChanged;
			if (onInnerDamageValueChanged != null)
			{
				onInnerDamageValueChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x000437B8 File Offset: 0x000419B8
	[CombatNotifyData(8, 10, 30U)]
	private void HandlerDataOldInjuries(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OldInjuries);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOldInjuriesChanged = CombatSubProcessor.Model.OnOldInjuriesChanged;
			if (onOldInjuriesChanged != null)
			{
				onOldInjuriesChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0004381C File Offset: 0x00041A1C
	[CombatNotifyData(8, 10, 47U)]
	private void HandlerDataOldPoison(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OldPoison);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOldPoisonChanged = CombatSubProcessor.Model.OnOldPoisonChanged;
			if (onOldPoisonChanged != null)
			{
				onOldPoisonChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00043880 File Offset: 0x00041A80
	[CombatNotifyData(8, 10, 6U)]
	private void HandlerDataOldDisorderOfQi(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.OldDisorderOfQi);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onOldDisorderOfQiChanged = CombatSubProcessor.Model.OnOldDisorderOfQiChanged;
			if (onOldDisorderOfQiChanged != null)
			{
				onOldDisorderOfQiChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x000438E3 File Offset: 0x00041AE3
	[CombatNotifyData(8, 10, 75U)]
	private void HandlerDataCombatReserveData(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CombatReserveData);
		OnCharacterDataChangedEvent<CombatReserveData> onCombatReserveDataChanged = CombatSubProcessor.Model.OnCombatReserveDataChanged;
		if (onCombatReserveDataChanged != null)
		{
			onCombatReserveDataChanged(this.CharacterId, this.CombatReserveData);
		}
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00043918 File Offset: 0x00041B18
	[CombatNotifyData(8, 10, 125U)]
	private void HandlerDataGangqi(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Gangqi);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onGangqiChanged = CombatSubProcessor.Model.OnGangqiChanged;
			if (onGangqiChanged != null)
			{
				onGangqiChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0004397C File Offset: 0x00041B7C
	[CombatNotifyData(8, 10, 126U)]
	private void HandlerDataGangqiMax(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.GangqiMax);
		bool flag = this.CharacterId == CombatSubProcessor.Model.SelfCharId || this.CharacterId == CombatSubProcessor.Model.EnemyCharId;
		if (flag)
		{
			OnDataChangedEvent onGangqiMaxChanged = CombatSubProcessor.Model.OnGangqiMaxChanged;
			if (onGangqiMaxChanged != null)
			{
				onGangqiMaxChanged(this.IsAlly);
			}
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000439DF File Offset: 0x00041BDF
	[CombatNotifyData(8, 10, 130U)]
	private void HandlerDataMindRhythm(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MindRhythm);
		OnDataChangedEvent onMindRhythmChanged = CombatSubProcessor.Model.OnMindRhythmChanged;
		if (onMindRhythmChanged != null)
		{
			onMindRhythmChanged(this.IsAlly);
		}
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00043A0C File Offset: 0x00041C0C
	[CombatNotifyData(8, 10, 131U)]
	private void HandlerDataMindUpheavalTime(RawDataPool pool, int offset)
	{
		CountdownData oldValue = this.MindUpheavalTime;
		Serializer.Deserialize(pool, offset, ref this.MindUpheavalTime);
		bool flag = oldValue.Progress <= 0f && this.MindUpheavalTime.Progress > 0f;
		if (flag)
		{
			OnDataChangedEvent onMindUpheavalChanged = CombatSubProcessor.Model.OnMindUpheavalChanged;
			if (onMindUpheavalChanged != null)
			{
				onMindUpheavalChanged(this.IsAlly);
			}
		}
		OnDataChangedEvent onMindUpheavalTimeChanged = CombatSubProcessor.Model.OnMindUpheavalTimeChanged;
		if (onMindUpheavalTimeChanged != null)
		{
			onMindUpheavalTimeChanged(this.IsAlly);
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00043A8E File Offset: 0x00041C8E
	[CombatNotifyData(8, 10, 100U)]
	private void HandlerDataBossPhase(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.BossPhase);
		OnDataChangedEvent onBossPhaseChanged = CombatSubProcessor.Model.OnBossPhaseChanged;
		if (onBossPhaseChanged != null)
		{
			onBossPhaseChanged(this.IsAlly);
		}
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00043ABC File Offset: 0x00041CBC
	public bool ContainsBodyPart(sbyte bodyPart)
	{
		if (!true)
		{
		}
		bool result;
		switch (bodyPart)
		{
		case 3:
			result = this.HaveLeftArm;
			break;
		case 4:
			result = this.HaveRightArm;
			break;
		case 5:
			result = this.HaveLeftLeg;
			break;
		case 6:
			result = this.HaveRightLeg;
			break;
		default:
			result = true;
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00043B14 File Offset: 0x00041D14
	[CombatNotifyData(8, 10, 151U)]
	private void HandlerDataMixPoisonCanAffectCount(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MixPoisonCanAffectCount);
	}

	// Token: 0x04000CFA RID: 3322
	private const ushort Objects = 0;

	// Token: 0x04000CFB RID: 3323
	private const ushort CombatCharacterDict = 10;

	// Token: 0x04000CFD RID: 3325
	public sbyte NeiliType;

	// Token: 0x04000CFE RID: 3326
	public int CurrNeili;

	// Token: 0x04000CFF RID: 3327
	public int MaxNeili;

	// Token: 0x04000D00 RID: 3328
	public short Health;

	// Token: 0x04000D01 RID: 3329
	public sbyte ConsummateLevel;

	// Token: 0x04000D02 RID: 3330
	public short MoveSpeed;

	// Token: 0x04000D03 RID: 3331
	public short InnerRatio;

	// Token: 0x04000D04 RID: 3332
	public short WeaponSwitchSpeed;

	// Token: 0x04000D05 RID: 3333
	public ItemKey[] Weapons;

	// Token: 0x04000D06 RID: 3334
	public int UsingWeaponIndex;

	// Token: 0x04000D07 RID: 3335
	public List<short> NeigongList;

	// Token: 0x04000D08 RID: 3336
	public List<short> AttackSkillList;

	// Token: 0x04000D09 RID: 3337
	public List<short> AssistSkillList;

	// Token: 0x04000D0A RID: 3338
	public short TemplateId;

	// Token: 0x04000D0B RID: 3339
	public NeiliAllocation AllocatedNeiliEffects;

	// Token: 0x04000D0C RID: 3340
	public short DisorderOfQi;

	// Token: 0x04000D0D RID: 3341
	public NeiliAllocation NeiliAllocation;

	// Token: 0x04000D0E RID: 3342
	public NeiliAllocation OriginNeiliAllocation;

	// Token: 0x04000D0F RID: 3343
	public CountdownData NeiliAllocationCd;

	// Token: 0x04000D10 RID: 3344
	public int BreathValue;

	// Token: 0x04000D11 RID: 3345
	public int StanceValue;

	// Token: 0x04000D12 RID: 3346
	public int MobilityValue;

	// Token: 0x04000D13 RID: 3347
	public short WugCount;

	// Token: 0x04000D14 RID: 3348
	public CountdownData NormalAttackRecovery;

	// Token: 0x04000D15 RID: 3349
	public OuterAndInnerShorts AttackRange;

	// Token: 0x04000D16 RID: 3350
	public MoveState MoveState;

	// Token: 0x04000D17 RID: 3351
	public bool PlayerControllingMove;

	// Token: 0x04000D18 RID: 3352
	public sbyte[] WeaponTricks;

	// Token: 0x04000D19 RID: 3353
	public byte WeaponTrickIndex;

	// Token: 0x04000D1A RID: 3354
	public TrickCollection Tricks;

	// Token: 0x04000D1B RID: 3355
	public int MaxTrickCount;

	// Token: 0x04000D1C RID: 3356
	public short ChangeTrickCount;

	// Token: 0x04000D1D RID: 3357
	public sbyte ChangeTrickProgress;

	// Token: 0x04000D1E RID: 3358
	public bool CanChangeTrick;

	// Token: 0x04000D1F RID: 3359
	public bool ChangingTrick;

	// Token: 0x04000D20 RID: 3360
	public bool ChangeTrickAttack;

	// Token: 0x04000D21 RID: 3361
	public bool ReserveNormalAttack;

	// Token: 0x04000D22 RID: 3362
	public SkillEffectCollection SkillEffectCollection;

	// Token: 0x04000D23 RID: 3363
	public short PreparingSkillId;

	// Token: 0x04000D24 RID: 3364
	public byte SkillPreparePercent;

	// Token: 0x04000D25 RID: 3365
	public short PerformingSkillId;

	// Token: 0x04000D26 RID: 3366
	public bool AutoCastingSkill;

	// Token: 0x04000D27 RID: 3367
	public short AffectingDefendSkillId = -1;

	// Token: 0x04000D28 RID: 3368
	public sbyte PreparingOtherAction;

	// Token: 0x04000D29 RID: 3369
	public EOtherActionInterruptType PreparingOtherActionInterruptType;

	// Token: 0x04000D2A RID: 3370
	public byte OtherActionPreparePercent;

	// Token: 0x04000D2B RID: 3371
	public bool[] OtherActionCanUse;

	// Token: 0x04000D2C RID: 3372
	public byte HealInjuryCount;

	// Token: 0x04000D2D RID: 3373
	public byte HealPoisonCount;

	// Token: 0x04000D2E RID: 3374
	public bool CanUseItem;

	// Token: 0x04000D2F RID: 3375
	public ItemKey PreparingItem;

	// Token: 0x04000D30 RID: 3376
	public byte UseItemPreparePercent;

	// Token: 0x04000D31 RID: 3377
	public bool UseItemCostNoWisdom;

	// Token: 0x04000D32 RID: 3378
	public DefeatMarkCollection DefeatMarkCollection;

	// Token: 0x04000D33 RID: 3379
	public List<sbyte> CurrTeammateCommands;

	// Token: 0x04000D34 RID: 3380
	public List<bool> TeammateCommandCanUse;

	// Token: 0x04000D35 RID: 3381
	public bool ShowTransferInjuryCommand;

	// Token: 0x04000D36 RID: 3382
	public List<SByteList> TeammateCommandBanReasons;

	// Token: 0x04000D37 RID: 3383
	public sbyte ExecutingTeammateCommand = -1;

	// Token: 0x04000D38 RID: 3384
	public List<CountdownData> TeammateCommandCd;

	// Token: 0x04000D39 RID: 3385
	public byte TeammateCommandPreparePercent;

	// Token: 0x04000D3A RID: 3386
	public byte TeammateCommandTimePercent;

	// Token: 0x04000D3B RID: 3387
	public ItemKey AttackCommandWeaponKey = ItemKey.Invalid;

	// Token: 0x04000D3C RID: 3388
	public sbyte AttackCommandTrickType = -1;

	// Token: 0x04000D3D RID: 3389
	public short AttackCommandSkillId = -1;

	// Token: 0x04000D3E RID: 3390
	public short DefendCommandSkillId = -1;

	// Token: 0x04000D3F RID: 3391
	public bool HaveLeftArm;

	// Token: 0x04000D40 RID: 3392
	public bool HaveRightArm;

	// Token: 0x04000D41 RID: 3393
	public bool HaveLeftLeg;

	// Token: 0x04000D42 RID: 3394
	public bool HaveRightLeg;

	// Token: 0x04000D43 RID: 3395
	public HitOrAvoidInts HitValues;

	// Token: 0x04000D44 RID: 3396
	public HitOrAvoidInts AvoidValues;

	// Token: 0x04000D45 RID: 3397
	public int[] OuterDamageValue;

	// Token: 0x04000D46 RID: 3398
	public int[] InnerDamageValue;

	// Token: 0x04000D47 RID: 3399
	public Injuries OldInjuries;

	// Token: 0x04000D48 RID: 3400
	public PoisonInts OldPoison;

	// Token: 0x04000D49 RID: 3401
	public short OldDisorderOfQi;

	// Token: 0x04000D4A RID: 3402
	public CombatReserveData CombatReserveData;

	// Token: 0x04000D4B RID: 3403
	public int Gangqi;

	// Token: 0x04000D4C RID: 3404
	public int GangqiMax;

	// Token: 0x04000D4D RID: 3405
	public CountdownData MindRhythm;

	// Token: 0x04000D4E RID: 3406
	public CountdownData MindUpheavalTime;

	// Token: 0x04000D4F RID: 3407
	public sbyte BossPhase;

	// Token: 0x04000D50 RID: 3408
	public MixPoisonAffectedCountCollection MixPoisonCanAffectCount;
}
