using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class UI_LoopingEvent : UIBase
{
	// Token: 0x06002759 RID: 10073 RVA: 0x0012220B File Offset: 0x0012040B
	public override void OnInit(ArgumentBox argsBox)
	{
		this._usedStrategies.Clear();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
			{
				SingletonObject.getInstance<BasicGameData>().TaiwuCharId
			});
		}));
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x00122244 File Offset: 0x00120444
	private void Awake()
	{
		this._concentrationCostLabel = base.CGet<TextMeshProUGUI>("ConcentrationCost");
		this._combatSkillIntro = base.CGet<CombatSkillIntro>("CombatSkillIntro");
		this._loopingInformation = base.CGet<LoopingInformation>("LoopingInformation");
		this._avatar = base.CGet<Game.Components.Avatar.Avatar>("Avatar");
		this._availableLoopingStrategySlots = base.CGetList<Refers>("AvailableLoopingStrategySlot_");
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x001222A7 File Offset: 0x001204A7
	private void OnDisable()
	{
		TaiwuDomainMethod.Call.ClearCurrentLoopingNeigongEvent();
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x001222B0 File Offset: 0x001204B0
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)UI_LoopingEvent.TaiwuCharId, new uint[]
		{
			46U,
			43U,
			79U,
			59U,
			45U,
			72U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 120, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 123, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 121, ulong.MaxValue, null));
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x00122330 File Offset: 0x00120530
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					this.HandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
				}
			}
			else
			{
				this.HandleDataModification(notification.Uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x001223D8 File Offset: 0x001205D8
	private unsafe void HandleDataModification(DataUid uid, int offset, RawDataPool dataPool)
	{
		ushort domainId = uid.DomainId;
		ushort num = domainId;
		if (num != 4)
		{
			if (num == 19)
			{
				switch (uid.DataId)
				{
				case 120:
					Serializer.Deserialize(dataPool, offset, ref this._referenceSkillList);
					while (this._referenceSkillList.Count < 3)
					{
						this._referenceSkillList.Add(-1);
					}
					CombatSkillDomainMethod.Call.CalcTaiwuExtraDeltaNeiliPerLoop(this.Element.GameDataListenerId);
					CombatSkillDomainMethod.Call.CalcTaiwuExtraDeltaNeiliAllocationPerLoop(this.Element.GameDataListenerId);
					break;
				case 121:
				{
					bool flag = this._loopingNeigong >= 0;
					if (flag)
					{
						TaiwuDomainMethod.Call.GetLoopingNeigongAvailableQiArtStrategies(this.Element.GameDataListenerId);
					}
					break;
				}
				case 123:
				{
					bool flag2 = this._loopingNeigong >= 0;
					if (flag2)
					{
						TaiwuDomainMethod.Call.GetLoopingNeigongQiArtStrategyDisplayDatas(this.Element.GameDataListenerId);
					}
					break;
				}
				}
			}
		}
		else
		{
			bool flag3 = uid.DataId == 0;
			if (flag3)
			{
				uint subId = uid.SubId1;
				uint num2 = subId;
				if (num2 <= 59U)
				{
					switch (num2)
					{
					case 43U:
					{
						MainAttributes mainAttributes;
						Serializer.Deserialize(dataPool, offset, ref mainAttributes);
						this._curConcentration = (int)(*(ref mainAttributes.Items.FixedElementField + (IntPtr)2 * 2));
						this.RefreshLoopingEventResource();
						break;
					}
					case 44U:
						break;
					case 45U:
						Serializer.Deserialize(dataPool, offset, ref this._currNeili);
						this.RefreshAvailableStrategies();
						break;
					case 46U:
					{
						Serializer.Deserialize(dataPool, offset, ref this._loopingNeigong);
						bool flag4 = this._loopingNeigong >= 0;
						if (flag4)
						{
							TaiwuDomainMethod.Call.GetLoopingNeigongQiArtStrategyDisplayDatas(this.Element.GameDataListenerId);
							TaiwuDomainMethod.Call.GetLoopingNeigongAvailableQiArtStrategies(this.Element.GameDataListenerId);
						}
						this.OnLoopingNeigongChanged();
						break;
					}
					default:
						if (num2 == 59U)
						{
							Serializer.Deserialize(dataPool, offset, ref this._learnedSkillList);
							this._learnedSkillList.RemoveAll((short id) => CombatSkill.Instance[id].EquipType != 0);
							this._combatSkillDisplayDataDict.Clear();
							bool flag5 = this._learnedSkillList.Count > 0;
							if (flag5)
							{
								CombatSkillModel.GetCombatSkillDisplayData(this.Element.GameDataListenerId, UI_LoopingEvent.TaiwuCharId, this._learnedSkillList);
							}
						}
						break;
					}
				}
				else if (num2 != 72U)
				{
					if (num2 == 79U)
					{
						MainAttributes mainAttributes2;
						Serializer.Deserialize(dataPool, offset, ref mainAttributes2);
						this._maxConcentration = (int)(*(ref mainAttributes2.Items.FixedElementField + (IntPtr)2 * 2));
						this.RefreshLoopingEventResource();
					}
				}
				else
				{
					Serializer.Deserialize(dataPool, offset, ref this._taiwuExtraNeiliAllocationProgress);
					this.RefreshLoopingInfomation();
				}
			}
		}
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x00122690 File Offset: 0x00120890
	private void HandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
	{
		switch (domainId)
		{
		case 4:
			if (methodId == 48)
			{
				this._charDataList = null;
				Serializer.Deserialize(dataPool, offset, ref this._charDataList);
				bool flag = this._charDataList.Count > 0;
				if (flag)
				{
					this._avatar.Refresh(this._charDataList[0], true);
				}
				else
				{
					this._avatar.gameObject.SetActive(false);
				}
				this.TryFinishShow();
			}
			break;
		case 5:
			if (methodId != 95)
			{
				if (methodId == 96)
				{
					Serializer.Deserialize(dataPool, offset, ref this._availableStrategies);
					foreach (int usedIndex in this._usedStrategies)
					{
						bool flag2 = usedIndex < this._availableStrategies.Items.Count;
						if (flag2)
						{
							this._availableStrategies.Items[usedIndex] = -1;
						}
					}
					this.RefreshAvailableStrategies();
					this.TryFinishShow();
				}
			}
			else
			{
				Serializer.Deserialize(dataPool, offset, ref this._taiwuQiArtStrategyList);
				CombatSkillDomainMethod.Call.CalcTaiwuExtraDeltaNeiliPerLoop(this.Element.GameDataListenerId);
				CombatSkillDomainMethod.Call.CalcTaiwuExtraDeltaNeiliAllocationPerLoop(this.Element.GameDataListenerId);
				bool flag3 = this._loopingNeigong != -1;
				if (flag3)
				{
					CombatSkillDomainMethod.Call.GetCombatSkillDisplayDataOnce(this.Element.GameDataListenerId, UI_LoopingEvent.TaiwuCharId, this._loopingNeigong);
				}
				this.RefreshAvailableStrategies();
			}
			break;
		case 7:
			switch (methodId)
			{
			case 0:
			{
				List<CombatSkillDisplayData> dataList = null;
				Serializer.Deserialize(dataPool, offset, ref dataList);
				foreach (CombatSkillDisplayData data in dataList)
				{
					this._combatSkillDisplayDataDict.Add(data.TemplateId, data);
				}
				bool isCombatSkillDisplayerDataReady = this.IsCombatSkillDisplayerDataReady;
				if (isCombatSkillDisplayerDataReady)
				{
					this.FirstTimeRefreshCombatSkills();
				}
				break;
			}
			case 3:
			{
				CombatSkillDisplayData combatSkillDisplayData = null;
				Serializer.Deserialize(dataPool, offset, ref combatSkillDisplayData);
				this._combatSkillDisplayDataDict[combatSkillDisplayData.TemplateId] = combatSkillDisplayData;
				this.RefreshLoopingInfomation();
				break;
			}
			case 5:
				Serializer.Deserialize(dataPool, offset, ref this._extraNeiliPerLoop);
				this.RefreshLoopingInfomation();
				break;
			case 6:
				Serializer.Deserialize(dataPool, offset, ref this._extraNeiliAllocationPerLoop);
				this.RefreshLoopingInfomation();
				break;
			}
			break;
		}
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x00122958 File Offset: 0x00120B58
	private void TryFinishShow()
	{
		bool flag = this._charDataList == null || this._charDataList.Count == 0;
		if (!flag)
		{
			bool flag2 = !this.IsCombatSkillDisplayerDataReady;
			if (!flag2)
			{
				List<sbyte> list = this._availableStrategies.Items;
				bool flag3 = list == null || list.Count == 0;
				if (!flag3)
				{
					this.Element.ShowAfterRefresh();
				}
			}
		}
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x001229C1 File Offset: 0x00120BC1
	private void RefreshLoopingEventResource()
	{
		this._concentrationCostLabel.SetText(this._curConcentration.ToString() + "/" + this._maxConcentration.ToString(), true);
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x001229F4 File Offset: 0x00120BF4
	private void OnLoopingNeigongChanged()
	{
		this.RefreshCombatSkillIntro();
		bool flag = this._loopingNeigong < 0;
		if (flag)
		{
			this.ShowEmpty();
		}
		else
		{
			CombatSkillDomainMethod.Call.CalcTaiwuExtraDeltaNeiliPerLoop(this.Element.GameDataListenerId);
			CombatSkillDomainMethod.Call.CalcTaiwuExtraDeltaNeiliAllocationPerLoop(this.Element.GameDataListenerId);
			this._loopingInformation.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x00122A54 File Offset: 0x00120C54
	private void ShowEmpty()
	{
		this._loopingInformation.gameObject.SetActive(false);
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x00122A69 File Offset: 0x00120C69
	private void FirstTimeRefreshCombatSkills()
	{
		this.RefreshCombatSkillIntro();
		this.RefreshLoopingInfomation();
		this.TryFinishShow();
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x00122A84 File Offset: 0x00120C84
	private void RefreshCombatSkillIntro()
	{
		CombatSkillDisplayData displayData;
		bool flag = this._combatSkillDisplayDataDict.TryGetValue(this._loopingNeigong, out displayData);
		if (flag)
		{
			this._combatSkillIntro.Refresh(displayData);
		}
		else
		{
			this._combatSkillIntro.Refresh(null);
		}
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x00122AC8 File Offset: 0x00120CC8
	private void RefreshLoopingInfomation()
	{
		bool flag = !this.IsCombatSkillDisplayerDataReady;
		if (!flag)
		{
			bool flag2 = this._loopingNeigong < 0;
			if (!flag2)
			{
				bool flag3 = this._referenceSkillList == null;
				if (!flag3)
				{
					bool flag4 = this._taiwuExtraNeiliAllocationProgress.Items == null;
					if (!flag4)
					{
						bool flag5 = this._taiwuQiArtStrategyList == null;
						if (!flag5)
						{
							this._loopingInformation.SetData(this._combatSkillDisplayDataDict[this._loopingNeigong], this._referenceSkillList, this._taiwuExtraNeiliAllocationProgress.Items, this._taiwuQiArtStrategyList, this._extraNeiliPerLoop, this._extraNeiliAllocationPerLoop, null, false, true, new Action<int>(this.OnSelectStrategySlot));
							this.AutoSelectStrategySlot();
						}
					}
				}
			}
		}
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x00122B84 File Offset: 0x00120D84
	private void AutoSelectStrategySlot()
	{
		for (int i = 0; i < this._taiwuQiArtStrategyList.Count; i++)
		{
			bool flag = this._taiwuQiArtStrategyList[i].TemplateId == -1;
			if (flag)
			{
				this._loopingInformation.SelectStrategy(i);
				this.RefreshAvailableStrategies();
				return;
			}
		}
		this._selectedStrategySlot = -1;
		this.RefreshAvailableStrategies();
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x00122BEC File Offset: 0x00120DEC
	private void RefreshAvailableStrategies()
	{
		List<sbyte> list = this._availableStrategies.Items;
		bool flag = list == null || list.Count == 0;
		if (!flag)
		{
			for (int i = 0; i < this._availableLoopingStrategySlots.Count; i++)
			{
				bool configValid = i < this._availableStrategies.Items.Count && this._availableStrategies.Items[i] != -1;
				bool available = this.HasEmptySlot && configValid && !this._usedStrategies.Contains(i);
				Refers refers = this._availableLoopingStrategySlots[i];
				refers.gameObject.SetActive(configValid);
				CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
				bool flag2 = configValid;
				if (flag2)
				{
					sbyte strategyId = this._availableStrategies.Items[i];
					QiArtStrategyItem strategyConfig = QiArtStrategy.Instance[strategyId];
					available = (available && this._curConcentration >= (int)strategyConfig.ConcentrationCost && this._currNeili >= (int)strategyConfig.NeiliCost);
					TextMeshProUGUI strategyName = refers.CGet<TextMeshProUGUI>("StrategyName");
					strategyName.text = QiArtStrategy.Instance[strategyId].Name;
					button.GetComponent<LoopingStrategyTipHelper>().Refresh(strategyId);
					TextMeshProUGUI cost = refers.CGet<TextMeshProUGUI>("Cost");
					cost.text = strategyConfig.ConcentrationCost.ToString();
				}
				button.GetComponent<PointerTrigger>().enabled = available;
				CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
				toggle.isOn = this._usedStrategies.Contains(i);
				int ii = i;
				DisableStyleRoot grey2 = refers.CGet<DisableStyleRoot>("StrategyBack");
				grey2.SetStyleEffect(!available, false);
				button.interactable = available;
				button.ClearAndAddListener(delegate
				{
					this.TrySetStrategy(ii);
				});
			}
			CircularLayout extraStrategy = base.CGet<CircularLayout>("ExtraStrategy");
			extraStrategy.ArrangeChildren();
		}
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x00122E04 File Offset: 0x00121004
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "CloseBtn";
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x00122E3C File Offset: 0x0012103C
	private void TrySetStrategy(int availableIndex)
	{
		bool flag = this._selectedStrategySlot < 0;
		if (!flag)
		{
			sbyte strategyId = this._availableStrategies.Items[availableIndex];
			TaiwuDomainMethod.Call.SetQiArtStrategy(this._selectedStrategySlot, strategyId);
			this._usedStrategies.Add(availableIndex);
			this._selectedStrategySlot = -1;
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x00122E8C File Offset: 0x0012108C
	private void OnSelectStrategySlot(int index)
	{
		this._selectedStrategySlot = index;
	}

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x0600276C RID: 10092 RVA: 0x00122E96 File Offset: 0x00121096
	private static int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x0600276D RID: 10093 RVA: 0x00122EA2 File Offset: 0x001210A2
	private bool IsCombatSkillDisplayerDataReady
	{
		get
		{
			return this._combatSkillDisplayDataDict.Count == this._learnedSkillList.Count;
		}
	}

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x0600276E RID: 10094 RVA: 0x00122EBC File Offset: 0x001210BC
	private bool HasEmptySlot
	{
		get
		{
			return this._selectedStrategySlot != -1;
		}
	}

	// Token: 0x04001C9B RID: 7323
	private int _curConcentration;

	// Token: 0x04001C9C RID: 7324
	private int _maxConcentration;

	// Token: 0x04001C9D RID: 7325
	private List<short> _learnedSkillList = new List<short>();

	// Token: 0x04001C9E RID: 7326
	private List<short> _referenceSkillList = new List<short>();

	// Token: 0x04001C9F RID: 7327
	private IntList _taiwuExtraNeiliAllocationProgress;

	// Token: 0x04001CA0 RID: 7328
	private List<QiArtStrategyDisplayData> _taiwuQiArtStrategyList;

	// Token: 0x04001CA1 RID: 7329
	private int _currNeili;

	// Token: 0x04001CA2 RID: 7330
	private readonly Dictionary<short, CombatSkillDisplayData> _combatSkillDisplayDataDict = new Dictionary<short, CombatSkillDisplayData>();

	// Token: 0x04001CA3 RID: 7331
	private short _loopingNeigong = -1;

	// Token: 0x04001CA4 RID: 7332
	private List<CharacterDisplayData> _charDataList;

	// Token: 0x04001CA5 RID: 7333
	private SByteList _availableStrategies;

	// Token: 0x04001CA6 RID: 7334
	private int _selectedStrategySlot = -1;

	// Token: 0x04001CA7 RID: 7335
	private readonly HashSet<int> _usedStrategies = new HashSet<int>();

	// Token: 0x04001CA8 RID: 7336
	private ValueTuple<int, int> _extraNeiliPerLoop = new ValueTuple<int, int>(0, 0);

	// Token: 0x04001CA9 RID: 7337
	private IntList _extraNeiliAllocationPerLoop = IntList.Create();

	// Token: 0x04001CAA RID: 7338
	private TextMeshProUGUI _concentrationCostLabel;

	// Token: 0x04001CAB RID: 7339
	private CombatSkillIntro _combatSkillIntro;

	// Token: 0x04001CAC RID: 7340
	private LoopingInformation _loopingInformation;

	// Token: 0x04001CAD RID: 7341
	private Game.Components.Avatar.Avatar _avatar;

	// Token: 0x04001CAE RID: 7342
	private List<Refers> _availableLoopingStrategySlots;
}
