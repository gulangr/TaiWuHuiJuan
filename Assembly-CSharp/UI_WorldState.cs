using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Adventure;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003B9 RID: 953
public class UI_WorldState : UIBase
{
	// Token: 0x170005D2 RID: 1490
	// (get) Token: 0x060039A6 RID: 14758 RVA: 0x001D4B35 File Offset: 0x001D2D35
	private bool IgnoreDangerHint
	{
		get
		{
			return SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.InAdventure || SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventTaiwu.InAdventure || UIManager.Instance.IsFocusElement(UIElement.BuildingArea) || WorldMapModel.Traveling;
		}
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x001D4B74 File Offset: 0x001D2D74
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitRefers();
		this.InitLevelIcons();
		BasicGameData basicData = SingletonObject.getInstance<BasicGameData>();
		this._taiwuCharId = basicData.TaiwuCharId;
		this.OnWorldMapPlayerAreaChange(null);
		this._uiAnim = base.GetComponent<UIAnim>();
		this._uiAnim.Init(Vector3.zero, Vector3.zero.SetY(500f));
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x001D4BD8 File Offset: 0x001D2DD8
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 22, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 62, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(1, 31, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(3, 7, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(3, 18, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 95, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(1, 1, ulong.MaxValue, null));
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x001D4C88 File Offset: 0x001D2E88
	private void Awake()
	{
		this._xiangshuProgress = -1;
		this.InitRefers();
		this._stateHolder = base.CGet<RectTransform>("StateIconHolder");
		this._otherStateRect = base.CGet<RectTransform>("OtherStateBase");
		PoolManager.SetSrcObject("WorldStateTemplate_Path", this._worldStateTemplate);
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x001D4CD7 File Offset: 0x001D2ED7
	private void OnDestroy()
	{
		PoolManager.RemoveData("WorldStateTemplate_Path");
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x001D4CE8 File Offset: 0x001D2EE8
	private void OnEnable()
	{
		GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.UpdateTime));
		GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.UpdateSolarTerm));
		GEvent.Add(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Add(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.OnWorldMapPlayerAreaChange));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		GEvent.Add(UiEvents.OnTaiwuOriginalSurnameSettingChanged, new GEvent.Callback(this.OnTaiwuOriginalSurnameSettingChanged));
		this.UpdateTime(null);
		this.UpdateSolarTerm(null);
	}

	// Token: 0x060039AC RID: 14764 RVA: 0x001D4DD8 File Offset: 0x001D2FD8
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.UpdateTime));
		GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.UpdateSolarTerm));
		GEvent.Remove(EEvents.OnTaiwuCharIdChange, new GEvent.Callback(this.OnTaiwuCharIdChange));
		GEvent.Remove(UiEvents.WorldMapPlayerAreaChange, new GEvent.Callback(this.OnWorldMapPlayerAreaChange));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		GEvent.Remove(UiEvents.OnTaiwuOriginalSurnameSettingChanged, new GEvent.Callback(this.OnTaiwuOriginalSurnameSettingChanged));
	}

	// Token: 0x060039AD RID: 14765 RVA: 0x001D4EB8 File Offset: 0x001D30B8
	private void LateUpdate()
	{
		RectTransform holder = base.CGet<RectTransform>("StateIconHolder");
		CImage image = holder.GetComponent<CImage>();
		image.enabled = false;
		int i = 0;
		int len = holder.childCount;
		while (i < len)
		{
			bool activeSelf = holder.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				image.enabled = true;
				break;
			}
			i++;
		}
	}

	// Token: 0x060039AE RID: 14766 RVA: 0x001D4F20 File Offset: 0x001D3120
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 1 && uid.DataId == 31;
				if (flag)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._worldStateData);
					this.RefreshWorldState();
					this.UpdateSectTaskTips();
				}
				else
				{
					bool flag2 = uid.DomainId == 5;
					if (flag2)
					{
						bool flag3 = uid.DataId == 22;
						if (flag3)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._moveTimeCostPercent);
						}
						else
						{
							bool flag4 = uid.DataId == 62;
							if (flag4)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._overweightSanctionPercent);
								this.UpdateInventoryOverflowTips(WorldState.Instance[11]);
							}
						}
					}
					else
					{
						bool flag5 = uid.DomainId == 3 && uid.DataId == 7;
						if (flag5)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._martialArtTournamentPreparationInfoList);
							this.UpdateMartialArtTournament(null);
							this.Element.ShowAfterRefresh();
						}
						else
						{
							bool flag6 = uid.DomainId == 3 && uid.DataId == 18;
							if (flag6)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._tournamentPreparationEndDate);
								this.UpdateMartialArtTournament(null);
							}
							else
							{
								bool flag7 = uid.DomainId == 19 && uid.DataId == 95;
								if (flag7)
								{
									Dictionary<short, LoongInfo> fiveLoongDict = new Dictionary<short, LoongInfo>();
									Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, fiveLoongDict);
									int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
									foreach (KeyValuePair<short, LoongInfo> pair in fiveLoongDict)
									{
										ushort count;
										this._loongDebuff[pair.Key] = ((pair.Value.CharacterDebuffCounts != null && pair.Value.CharacterDebuffCounts.TryGetValue(taiwuCharId, out count)) ? count : 0);
									}
									this.UpdateLoongDebuff(null);
									this.Element.ShowAfterRefresh();
								}
								else
								{
									bool flag8 = uid.DomainId == 1 && uid.DataId == 1;
									if (flag8)
									{
										bool initial = this._xiangshuProgress < 0;
										sbyte progressOld = this._xiangshuProgress;
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._xiangshuProgress);
										sbyte levelOld = GameData.Domains.World.SharedMethods.GetXiangshuLevel(progressOld);
										sbyte levelNew = GameData.Domains.World.SharedMethods.GetXiangshuLevel(this._xiangshuProgress);
										bool flag9 = !initial && this._xiangshuProgress > progressOld;
										if (flag9)
										{
											UI_WorldState.<>c__DisplayClass26_0 CS$<>8__locals1 = new UI_WorldState.<>c__DisplayClass26_0();
											CS$<>8__locals1.args = new ArgumentBox().Set("XiangshuLevelNew", levelNew).Set("XiangshuLevelOld", levelOld).Set("ConsummateLevelNew", this._xiangshuProgress).Set("ConsummateLevelOld", progressOld);
											CS$<>8__locals1.<OnNotifyGameData>g__ShowPopup|0();
										}
									}
								}
							}
						}
					}
				}
			}
		}
		UI_WorldState.SetActiveByChildren(base.CGet<RectTransform>("SectTaskLisk"));
		UI_WorldState.SetActiveByChildren(this._stateHolder);
	}

	// Token: 0x060039AF RID: 14767 RVA: 0x001D52CC File Offset: 0x001D34CC
	private void RefreshWorldState()
	{
		this._curSectTask.Clear();
		foreach (WorldStateItem worldStateItem in ((IEnumerable<WorldStateItem>)WorldState.Instance))
		{
			bool worldState = this._worldStateData.GetWorldState((short)worldStateItem.TemplateId);
			bool flag = this._invasionWorldStateTemplateIds.Contains(worldStateItem.TemplateId);
			if (flag)
			{
				this.UpdateXiangshuProcess(worldStateItem, worldState);
			}
			else
			{
				bool flag2 = worldStateItem.TemplateId == 11;
				if (flag2)
				{
					this.UpdateInventoryOverflowTips(worldStateItem);
				}
				else
				{
					bool flag3 = worldStateItem.TemplateId == 12;
					if (flag3)
					{
						this.UpdateWarehouseOverflowTips(worldStateItem, worldState);
					}
					else
					{
						bool flag4 = worldStateItem.TemplateId == 13;
						if (flag4)
						{
							this.UpdateResourceOverflowTips(worldStateItem, worldState);
						}
						else
						{
							bool flag5 = worldStateItem.TemplateId == 14 || worldStateItem.TemplateId == 15;
							if (flag5)
							{
								this.UpdateInjuryTips(worldStateItem, worldStateItem.TemplateId == 14, worldState);
							}
							else
							{
								bool flag6 = worldStateItem.TemplateId == 17;
								if (flag6)
								{
									this.UpdatePoisonTips(worldStateItem, worldState);
								}
								else
								{
									bool flag7 = worldStateItem.TemplateId == 40;
									if (flag7)
									{
										this.UpdateTeammateInjuryTips(worldStateItem, worldState);
									}
									else
									{
										bool flag8 = worldStateItem.TemplateId == 20;
										if (flag8)
										{
											this.UpdateXiangshuAwakeTips(worldStateItem, worldState);
										}
										else
										{
											bool flag9 = worldStateItem.TemplateId == 21;
											if (flag9)
											{
												this.UpdateXiangShuAttackTips(worldStateItem, worldState);
											}
											else
											{
												bool flag10 = worldStateItem.TemplateId >= 25 && worldStateItem.TemplateId <= 39 && worldState;
												if (flag10)
												{
													this._curSectTask.Add(worldStateItem.TemplateId);
												}
												else
												{
													bool flag11 = worldStateItem.TemplateId == 22;
													if (flag11)
													{
														this.UpdateMartialArtTournamentPrepare();
													}
													else
													{
														bool flag12 = worldStateItem.TemplateId == 24;
														if (flag12)
														{
															this.UpdateMartialArtTournamentOpen();
														}
														else
														{
															bool flag13 = worldStateItem.TemplateId >= 42 && worldStateItem.TemplateId <= 46;
															if (flag13)
															{
																this.UpdateLoongDebuff(null);
															}
															else
															{
																bool flag14 = worldStateItem.TemplateId == 49;
																if (flag14)
																{
																	this.UpdateTaiwuWanted(worldState);
																}
																else
																{
																	bool flag15 = worldStateItem.TemplateId == 51;
																	if (flag15)
																	{
																		this.UpdateTearmateDyingTips(worldStateItem, worldState);
																	}
																	else
																	{
																		bool flag16 = worldStateItem.TemplateId == 52;
																		if (flag16)
																		{
																			this.UpdateHomelessVillagerTips(worldStateItem, worldState);
																		}
																		else
																		{
																			bool flag17 = worldStateItem.TemplateId == 53;
																			if (flag17)
																			{
																				this.UpdateNeiliConflictingTips(worldStateItem, worldState);
																			}
																			else
																			{
																				this.UpdateGeneralTips(worldStateItem, worldState);
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		UI_WorldState.SetActiveByChildren(this._stateHolder);
	}

	// Token: 0x060039B0 RID: 14768 RVA: 0x001D558C File Offset: 0x001D378C
	private GameObject GetCurObject(WorldStateItem worldStateItem, bool needShow, RectTransform stateHolderRect, int extraId = -1)
	{
		string objName = "WorldState_" + worldStateItem.TemplateId.ToString() + ((extraId >= 0) ? string.Format("_{0}", extraId) : "");
		Transform stateRect = stateHolderRect.Find(objName);
		GameObject state = (stateRect != null) ? stateRect.gameObject : null;
		bool flag = !needShow;
		if (flag)
		{
			bool flag2 = state != null;
			if (flag2)
			{
				state.SetActive(false);
			}
		}
		else
		{
			bool flag3 = state == null;
			if (flag3)
			{
				state = PoolManager.GetObject("WorldStateTemplate_Path");
				state.name = objName;
				state.transform.SetParent(stateHolderRect, false);
				state.GetComponent<TooltipInvoker>().PresetParam[0] = worldStateItem.Name.ColorReplace();
			}
			state.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
			state.SetActive(true);
		}
		bool flag4 = state != null;
		if (flag4)
		{
			TooltipInvoker display = state.GetComponent<TooltipInvoker>();
			display.RuntimeParam = null;
			display.enabled = true;
		}
		UI_WorldState.SetActiveByChildren(stateHolderRect);
		UI_WorldState.SetActiveByChildren(this._stateHolder);
		return state;
	}

	// Token: 0x060039B1 RID: 14769 RVA: 0x001D56B4 File Offset: 0x001D38B4
	internal void UpdateSectTaskTips()
	{
		RectTransform sectTaskRect = base.CGet<RectTransform>("SectTaskLisk");
		sectTaskRect.gameObject.SetActive(this._curSectTask.Count > 0);
		for (int i = 0; i < this._curSectTask.Count; i++)
		{
			WorldStateItem worldStateItem = WorldState.Instance.GetItem(this._curSectTask[i]);
			bool worldState = this._worldStateData.GetWorldState((short)worldStateItem.TemplateId);
			bool flag = i >= sectTaskRect.childCount;
			GameObject sectTaskStateObj;
			if (flag)
			{
				sectTaskStateObj = this.GetCurObject(worldStateItem, worldState, sectTaskRect, -1);
			}
			else
			{
				sectTaskStateObj = sectTaskRect.GetChild(i).gameObject;
			}
			sectTaskStateObj.transform.SetParent(sectTaskRect, false);
			sectTaskRect.GetChild(i).GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
			TooltipInvoker mouseTip = sectTaskRect.GetChild(i).GetComponent<TooltipInvoker>();
			StoryDomainMethod.AsyncCall.GetSectMainStoryTriggerConditions(this, (short)(sectTaskRect.GetChild(i).GetComponent<WorldStateToggle>().SectId = worldStateItem.Sect), delegate(int offset, RawDataPool dataPool)
			{
				int status = 0;
				Serializer.Deserialize(dataPool, offset, ref status);
				TooltipInvoker mouseTip;
				mouseTip.Type = TipType.SectStory;
				mouseTip = mouseTip;
				if (mouseTip.RuntimeParam == null)
				{
					mouseTip.RuntimeParam = new ArgumentBox();
				}
				mouseTip.RuntimeParam.Set("TemplateId", worldStateItem.TemplateId);
				mouseTip.RuntimeParam.Set("ConditionStatus", status);
				mouseTip.gameObject.SetActive(true);
			});
		}
		for (int j = this._curSectTask.Count; j < sectTaskRect.childCount; j++)
		{
			sectTaskRect.GetChild(j).gameObject.SetActive(false);
		}
		UI_WorldState.SetActiveByChildren(sectTaskRect);
	}

	// Token: 0x060039B2 RID: 14770 RVA: 0x001D5830 File Offset: 0x001D3A30
	private void UpdateTime(ArgumentBox argBox = null)
	{
		TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
		base.CGet<TextMeshProUGUI>("Time").text = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
		{
			timeManager.GetYear(),
			(int)(timeManager.GetMonthInCurrYear() + 1),
			Month.Instance[timeManager.GetMonthInCurrYear()].Name + LocalStringManager.Get(LanguageKey.LK_Dot_Symbol),
			LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetCurrSeason()))
		});
	}

	// Token: 0x060039B3 RID: 14771 RVA: 0x001D58C8 File Offset: 0x001D3AC8
	private void UpdateInventoryOverflowTips(WorldStateItem worldStateItem)
	{
		bool isTrue = this._overweightSanctionPercent != null && this._worldStateData.GetWorldState((short)worldStateItem.TemplateId);
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			TooltipInvoker mouseTip = state.GetComponent<TooltipInvoker>();
			MouseTip_Util.SetInventoryOverLoadTips(mouseTip, this._overweightSanctionPercent, this._moveTimeCostPercent);
		}
	}

	// Token: 0x060039B4 RID: 14772 RVA: 0x001D5934 File Offset: 0x001D3B34
	private void UpdateGeneralTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.ColorReplace();
		}
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x001D5980 File Offset: 0x001D3B80
	private void UpdateResourceOverflowTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			StringBuilder resourceNameBuilder = EasyPool.Get<StringBuilder>();
			resourceNameBuilder.Clear();
			for (sbyte type = 0; type < 6; type += 1)
			{
				bool flag2 = this._worldStateData.IsResourceOverloaded(type);
				if (flag2)
				{
					bool flag3 = resourceNameBuilder.Length > 0;
					if (flag3)
					{
						resourceNameBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					resourceNameBuilder.Append(ResourceType.Instance[type].Name);
				}
			}
			state.SetActive(resourceNameBuilder.Length > 0);
			bool flag4 = resourceNameBuilder.Length > 0;
			if (flag4)
			{
				state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(resourceNameBuilder.ToString()).ColorReplace();
			}
			EasyPool.Free<StringBuilder>(resourceNameBuilder);
		}
	}

	// Token: 0x060039B6 RID: 14774 RVA: 0x001D5A70 File Offset: 0x001D3C70
	private void UpdateWarehouseOverflowTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			foreach (WorldStateData.EStorageType t in this._worldStateData.IterateOverloadStorageTypes())
			{
				bool flag2 = sb.Length > 0;
				if (flag2)
				{
					sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
				StringBuilder stringBuilder = sb;
				if (!true)
				{
				}
				LanguageKey id;
				if (t != WorldStateData.EStorageType.Warehouse)
				{
					if (t != WorldStateData.EStorageType.Trough)
					{
						throw new ArgumentOutOfRangeException();
					}
					id = LanguageKey.LK_Trough;
				}
				else
				{
					id = LanguageKey.LK_Storage;
				}
				if (!true)
				{
				}
				stringBuilder.Append(LocalStringManager.Get(id));
			}
			state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(sb.ToString()).ColorReplace();
			EasyPool.Free<StringBuilder>(sb);
		}
	}

	// Token: 0x060039B7 RID: 14775 RVA: 0x001D5B80 File Offset: 0x001D3D80
	private void UpdateInjuryTips(WorldStateItem worldStateItem, bool isOuterInjury, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Clear();
			for (sbyte type = 0; type < 7; type += 1)
			{
				bool isInjury = isOuterInjury ? this._worldStateData.BodyPartHasOuterInjury(type) : this._worldStateData.BodyPartHasInnerInjury(type);
				bool flag2 = isInjury;
				if (flag2)
				{
					bool flag3 = strBuilder.Length > 0;
					if (flag3)
					{
						strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					strBuilder.Append(BodyPart.Instance[type].Name);
				}
			}
			state.SetActive(strBuilder.Length > 0);
			bool flag4 = strBuilder.Length > 0;
			if (flag4)
			{
				state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(strBuilder.ToString()).ColorReplace();
			}
			EasyPool.Free<StringBuilder>(strBuilder);
		}
	}

	// Token: 0x060039B8 RID: 14776 RVA: 0x001D5C84 File Offset: 0x001D3E84
	private void UpdatePoisonTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Clear();
			for (sbyte order = 0; order < 6; order += 1)
			{
				sbyte poisonType = PoisonType.GetTypeBySortingOrder(order);
				bool isPoisoned = this._worldStateData.IsPoisonedWithType(poisonType);
				bool flag2 = isPoisoned;
				if (flag2)
				{
					PoisonItem configData = Poison.Instance[poisonType];
					bool flag3 = strBuilder.Length > 0;
					if (flag3)
					{
						strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					strBuilder.Append(configData.Name.SetColor(configData.FontColor));
				}
			}
			state.SetActive(strBuilder.Length > 0);
			bool flag4 = strBuilder.Length > 0;
			if (flag4)
			{
				state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(strBuilder.ToString()).ColorReplace();
			}
			EasyPool.Free<StringBuilder>(strBuilder);
		}
	}

	// Token: 0x060039B9 RID: 14777 RVA: 0x001D5D90 File Offset: 0x001D3F90
	private void UpdateTeammateInjuryTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			TaiwuDomainMethod.AsyncCall.GetSeverelyInjuredGroupCharNames(this, delegate(int offset, RawDataPool dataPool)
			{
				List<NameRelatedData> names = null;
				Serializer.Deserialize(dataPool, offset, ref names);
				bool flag2 = names != null && names.Count > 0;
				if (flag2)
				{
					state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(NameCenter.GetNameSequenceStringByNameRelatedDataList(names, false).SetColor("brightred")).ColorReplace();
					state.SetActive(true);
				}
				else
				{
					state.SetActive(false);
				}
			});
		}
	}

	// Token: 0x060039BA RID: 14778 RVA: 0x001D5DF4 File Offset: 0x001D3FF4
	private void UpdateXiangshuAwakeTips(WorldStateItem worldStateItem, bool isTrue)
	{
		RectTransform awakeList = base.CGet<RectTransform>("XiangShuAwakeList");
		int awakeCount = -1;
		for (sbyte i = 0; i < 9; i += 1)
		{
			bool isAwakening = this._worldStateData.IsXiangshuAvatarAwakening(i);
			string swordTombName = AdventureRemakeModel.Core.GetAdventureAny(SwordTomb.Instance[i].AdventureCoreId).Name;
			bool flag = !isAwakening;
			if (!flag)
			{
				awakeCount++;
				bool flag2 = awakeCount >= awakeList.childCount;
				if (flag2)
				{
					this.GetCurObject(worldStateItem, isTrue, awakeList, (int)i);
				}
				else
				{
					GameObject awakeIconPrefab = awakeList.GetChild(awakeCount).gameObject;
					awakeIconPrefab.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
				}
				TooltipInvoker tipDisplayer = awakeList.GetChild(awakeCount).GetComponent<TooltipInvoker>();
				tipDisplayer.PresetParam[1] = worldStateItem.Desc.GetFormat(swordTombName).ColorReplace();
			}
		}
		for (int j = awakeCount + 1; j < awakeList.childCount; j++)
		{
			awakeList.GetChild(j).gameObject.SetActive(false);
		}
		UI_WorldState.SetActiveByChildren(awakeList);
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x001D5F10 File Offset: 0x001D4110
	private void UpdateXiangShuAttackTips(WorldStateItem worldStateItem, bool isTrue)
	{
		RectTransform attackList = base.CGet<RectTransform>("XiangShuAttackList");
		int attackCount = -1;
		for (sbyte i = 0; i < 9; i += 1)
		{
			bool isAttacking = this._worldStateData.IsXiangshuAvatarAttacking(i);
			string swordTombName = AdventureRemakeModel.Core.GetAdventureAny(SwordTomb.Instance[i].AdventureCoreId).Name;
			bool flag = !isAttacking;
			if (!flag)
			{
				attackCount++;
				bool flag2 = attackCount >= attackList.childCount;
				if (flag2)
				{
					this.GetCurObject(worldStateItem, isTrue, attackList, (int)i);
				}
				else
				{
					GameObject attackIconPrefab = attackList.GetChild(attackCount).gameObject;
					attackIconPrefab.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
				}
				TooltipInvoker tipDisplayer = attackList.GetChild(attackCount).GetComponent<TooltipInvoker>();
				tipDisplayer.PresetParam[1] = worldStateItem.Desc.GetFormat(swordTombName).ColorReplace();
				tipDisplayer.gameObject.SetActive(true);
			}
		}
		for (int j = attackCount + 1; j < attackList.childCount; j++)
		{
			attackList.GetChild(j).gameObject.SetActive(false);
		}
		UI_WorldState.SetActiveByChildren(attackList);
	}

	// Token: 0x060039BC RID: 14780 RVA: 0x001D6040 File Offset: 0x001D4240
	private void UpdateSolarTerm(ArgumentBox argBox = null)
	{
		TextMeshProUGUI solarTermName = base.CGet<TextMeshProUGUI>("SolarTermName");
		solarTermName.text = "";
	}

	// Token: 0x060039BD RID: 14781 RVA: 0x001D6066 File Offset: 0x001D4266
	private void UpdateMartialArtTournament(ArgumentBox box = null)
	{
		this.UpdateMartialArtTournamentPrepare();
		this.UpdateMartialArtTournamentOpen();
	}

	// Token: 0x060039BE RID: 14782 RVA: 0x001D6078 File Offset: 0x001D4278
	private void UpdateMartialArtTournamentPrepare()
	{
		bool needShow = this._worldStateData.GetWorldState(22);
		WorldStateItem worldStateItem = WorldState.DefValue.MartialArtTournamentPrepare;
		GameObject tipObj = this.GetCurObject(worldStateItem, needShow, this._otherStateRect, -1);
		bool flag = !needShow || null == tipObj;
		if (!flag)
		{
			tipObj.SetActive(true);
			TooltipInvoker mouseTipDisplayer = tipObj.GetComponent<TooltipInvoker>();
			string[] tipsData = new string[2];
			mouseTipDisplayer.PresetParam = tipsData;
			CImage tipImg = tipObj.GetComponent<CImage>();
			tipImg.SetSprite(worldStateItem.Icon, true, null);
			List<MartialArtTournamentPreparationInfo> martialArtTournamentPreparationInfoList = this._martialArtTournamentPreparationInfoList;
			bool flag2 = martialArtTournamentPreparationInfoList != null && martialArtTournamentPreparationInfoList.Count > 0;
			if (flag2)
			{
				this._martialArtTournamentPreparationInfoList.Sort();
				this._martialArtTournamentPreparationInfoList.Reverse();
				List<short> settlementIdList = this._martialArtTournamentPreparationInfoList.ConvertAll<short>((MartialArtTournamentPreparationInfo e) => e.SettlementId);
				OrganizationDomainMethod.AsyncCall.GetSettlementNameRelatedData(this, settlementIdList, delegate(int offset, RawDataPool dataPool)
				{
					tipsData[0] = worldStateItem.Name;
					List<SettlementNameRelatedData> nameRelatedDataList = new List<SettlementNameRelatedData>();
					Serializer.Deserialize(dataPool, offset, ref nameRelatedDataList);
					List<string> prepareDescList = new List<string>();
					prepareDescList.Add(LocalStringManager.Get(LanguageKey.UI_WorldState_MartialArtTournament_State_PrepairPower));
					string baseStr = LocalStringManager.Get(LanguageKey.UI_WorldState_MartialArtTournament_Prepair_SectDesc);
					for (int i = 0; i < this._martialArtTournamentPreparationInfoList.Count; i++)
					{
						SettlementNameRelatedData nameRelatedData = nameRelatedDataList[i];
						MartialArtTournamentPreparationInfo info = this._martialArtTournamentPreparationInfoList[i];
						string settlementName = LocalStringManager.Get(this._sectShortNameCache[nameRelatedData.MapBlockTemplateId]);
						prepareDescList.Add(baseStr.GetFormat(new object[]
						{
							settlementName,
							info.TotalScore,
							info.CombatPowerPreparation,
							info.ResourcePreparation,
							info.AuthorityPreparation
						}));
						this._settlementNameRelatedDataCache.Remove(settlementIdList[i]);
						this._settlementNameRelatedDataCache.Add(settlementIdList[i], nameRelatedDataList[i]);
					}
					prepareDescList.Add(string.Empty);
					int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
					int remainingMonth = this._tournamentPreparationEndDate - currDate;
					prepareDescList.Add(LocalStringManager.GetFormat(LanguageKey.UI_WorldState_MartialArtTournament_Prepair_MonthCountToOpen, remainingMonth));
					tipsData[1] = string.Join("\n", prepareDescList);
					mouseTipDisplayer.PresetParam = tipsData;
				});
			}
		}
	}

	// Token: 0x060039BF RID: 14783 RVA: 0x001D61BC File Offset: 0x001D43BC
	private void UpdateMartialArtTournamentOpen()
	{
		bool needShow = this._worldStateData.GetWorldState(24);
		WorldStateItem worldStateItem = WorldState.Instance.GetItem(24);
		GameObject tipObj = this.GetCurObject(worldStateItem, needShow, this._otherStateRect, -1);
		bool flag = !needShow || null == tipObj;
		if (!flag)
		{
			tipObj.SetActive(true);
			CImage tipImg = tipObj.GetComponent<CImage>();
			tipImg.SetSprite(worldStateItem.Icon, true, null);
			string[] tipsData = new string[2];
			AdventureMajorEvent majorEventDict = SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventDict.Values.FirstOrDefault((AdventureMajorEvent majorEvent) => majorEvent.CoreId == 114668976);
			int remainingMonth = (majorEventDict != null) ? majorEventDict.RemainMonths : 0;
			OrganizationDomainMethod.AsyncCall.GetMartialArtTournamentCurrentHostSettlementId(this, delegate(int offset, RawDataPool dataPool)
			{
				short settlementId = -1;
				Serializer.Deserialize(dataPool, offset, ref settlementId);
				SettlementNameRelatedData nameRelatedData;
				bool flag2 = this._settlementNameRelatedDataCache.TryGetValue(settlementId, out nameRelatedData);
				if (flag2)
				{
					tipsData[0] = worldStateItem.Name;
					string sectName = CommonUtils.GetSettlementString(nameRelatedData.RandomNameId, nameRelatedData.MapBlockTemplateId);
					tipsData[1] = worldStateItem.Desc.GetFormat(sectName, remainingMonth.ToString()).ColorReplace();
				}
				else
				{
					List<short> list = new List<short>
					{
						settlementId
					};
					OrganizationDomainMethod.AsyncCall.GetSettlementNameRelatedData(this, list, delegate(int offsetSingle, RawDataPool dataPoolSingle)
					{
						List<SettlementNameRelatedData> nameRelatedDataList = new List<SettlementNameRelatedData>();
						Serializer.Deserialize(dataPoolSingle, offsetSingle, ref nameRelatedDataList);
						nameRelatedData = nameRelatedDataList[0];
						this._settlementNameRelatedDataCache.Remove(settlementId);
						this._settlementNameRelatedDataCache.Add(settlementId, nameRelatedDataList[0]);
						tipsData[0] = worldStateItem.Name;
						string sectName2 = CommonUtils.GetSettlementString(nameRelatedData.RandomNameId, nameRelatedData.MapBlockTemplateId);
						tipsData[1] = worldStateItem.Desc.GetFormat(sectName2, remainingMonth.ToString()).ColorReplace();
					});
				}
				tipObj.GetComponent<TooltipInvoker>().PresetParam = tipsData;
			});
		}
	}

	// Token: 0x060039C0 RID: 14784 RVA: 0x001D62C0 File Offset: 0x001D44C0
	private void UpdateLoongDebuff(ArgumentBox box = null)
	{
		foreach (KeyValuePair<short, ushort> pair in this._loongDebuff)
		{
			sbyte templateId = (sbyte)Loong.Instance[(int)(pair.Key - 246)].WorldState;
			WorldStateItem worldStateItem = WorldState.Instance.GetItem(templateId);
			bool needShow = this._worldStateData.GetWorldState((short)templateId);
			GameObject tipObj = this.GetCurObject(worldStateItem, needShow, this._otherStateRect, -1);
			bool flag = pair.Value > 0;
			if (flag)
			{
				bool flag2 = !needShow || null == tipObj;
				if (flag2)
				{
					break;
				}
				tipObj.SetActive(needShow);
				CImage tipImg = tipObj.GetComponent<CImage>();
				tipImg.SetSprite(worldStateItem.Icon, true, null);
				tipObj.GetComponent<TooltipInvoker>().PresetParam[0] = worldStateItem.Name;
				tipObj.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(pair.Value.ToString()).ColorReplace();
			}
		}
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x001D63F8 File Offset: 0x001D45F8
	private void UpdateTaiwuWanted(bool isTrue)
	{
		WorldStateItem worldStateItem = WorldState.Instance.GetItem(49);
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			TooltipInvoker display = state.GetComponent<TooltipInvoker>();
			display.enabled = false;
			OrganizationDomainMethod.AsyncCall.GetBountyCharacterDisplayDataFromCharacterList(null, SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds(), delegate(int offset, RawDataPool pool)
			{
				SettlementBountyDisplayData data = new SettlementBountyDisplayData();
				Serializer.Deserialize(pool, offset, ref data);
				bool flag2 = null != display;
				if (flag2)
				{
					display.enabled = true;
					display.Type = TipType.TaiwuWanted;
					display.RuntimeParam = new ArgumentBox();
					display.RuntimeParam.SetObject("Data", data);
					display.Refresh(false, -1);
				}
			});
		}
	}

	// Token: 0x060039C2 RID: 14786 RVA: 0x001D6478 File Offset: 0x001D4678
	private void UpdateTearmateDyingTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			TaiwuDomainMethod.AsyncCall.GetDyingGroupCharNames(this, true, delegate(int offset, RawDataPool dataPool)
			{
				List<NameRelatedData> names = new List<NameRelatedData>();
				Serializer.Deserialize(dataPool, offset, ref names);
				bool includeTaiwu = false;
				foreach (NameRelatedData name in names)
				{
					bool flag2 = name.OrgTemplateId == 16 && name.OrgGrade == 8;
					if (flag2)
					{
						includeTaiwu = true;
					}
				}
				bool flag3 = names.Count > 0;
				if (flag3)
				{
					state.GetComponent<TooltipInvoker>().PresetParam[1] = string.Format(worldStateItem.Desc, NameCenter.GetNameSequenceStringByNameRelatedDataList(names, includeTaiwu).SetColor("brightred")).ColorReplace();
					state.SetActive(true);
				}
				else
				{
					state.SetActive(false);
				}
			});
		}
	}

	// Token: 0x060039C3 RID: 14787 RVA: 0x001D64DC File Offset: 0x001D46DC
	private void UpdateHomelessVillagerTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			BuildingDomainMethod.AsyncCall.GetResidenceInfo(this, delegate(int offset, RawDataPool dataPool)
			{
				ValueTuple<int, int, int> info = default(ValueTuple<int, int, int>);
				Serializer.Deserialize(dataPool, offset, ref info);
				TooltipInvoker mouseTip = state.GetComponent<TooltipInvoker>();
				mouseTip.PresetParam[1] = string.Format(worldStateItem.Desc, info.Item1, info.Item2, info.Item3);
			});
		}
	}

	// Token: 0x060039C4 RID: 14788 RVA: 0x001D6540 File Offset: 0x001D4740
	private void UpdateNeiliConflictingTips(WorldStateItem worldStateItem, bool isTrue)
	{
		GameObject state = this.GetCurObject(worldStateItem, isTrue, this._otherStateRect, -1);
		bool flag = state == null || !isTrue;
		if (!flag)
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			TaiwuDomainMethod.AsyncCall.GetGroupNeiliConflictingCharDataList(this, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayData> list = new List<CharacterDisplayData>();
				Serializer.Deserialize(dataPool, offset, ref list);
				TooltipInvoker mouseTip = state.GetComponent<TooltipInvoker>();
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				stringBuilder.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					CharacterDisplayData charData = list[i];
					bool isTaiwu = charData.CharacterId == taiwuCharId;
					string charName = NameCenter.GetMonasticTitleOrDisplayName(charData, isTaiwu).SetColor("orange");
					stringBuilder.Append(charName);
					bool flag2 = i < list.Count - 1;
					if (flag2)
					{
						string separator = LocalStringManager.Get(LanguageKey.LK_Separator);
						stringBuilder.Append(separator);
					}
				}
				mouseTip.PresetParam[1] = string.Format(worldStateItem.Desc, stringBuilder);
				EasyPool.Free<StringBuilder>(stringBuilder);
			});
		}
	}

	// Token: 0x060039C5 RID: 14789 RVA: 0x001D65B4 File Offset: 0x001D47B4
	private void OnTaiwuCharIdChange(ArgumentBox argumentBox)
	{
		int newTaiwuCharId;
		argumentBox.Get("NewTaiwuCharId", out newTaiwuCharId);
		bool flag = newTaiwuCharId == this._taiwuCharId;
		if (!flag)
		{
			this._taiwuCharId = newTaiwuCharId;
		}
	}

	// Token: 0x060039C6 RID: 14790 RVA: 0x001D65E8 File Offset: 0x001D47E8
	private void OnWorldMapPlayerAreaChange(ArgumentBox argumentBox)
	{
		GameObject container = base.CGet<GameObject>("DangerHint");
		container.SetActive(false);
		bool ignoreDangerHint = this.IgnoreDangerHint;
		if (!ignoreDangerHint)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool flag = mapModel != null && mapModel.Areas != null && mapModel.Areas.CheckIndex((int)mapModel.CurrentAreaId);
			if (flag)
			{
				bool flag2 = mapModel.IsAreaHasDangerTips(mapModel.CurrentAreaId);
				if (flag2)
				{
					TextMeshProUGUI text = base.CGet<TextMeshProUGUI>("DangerHintText");
					string info = LocalStringManager.Get(LanguageKey.LK_Danger_Area_Tips);
					text.text = info;
					container.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060039C7 RID: 14791 RVA: 0x001D6680 File Offset: 0x001D4880
	private void OnTopUiChanged(ArgumentBox argumentBox)
	{
		bool ignoreDangerHint = this.IgnoreDangerHint;
		if (ignoreDangerHint)
		{
			GameObject container = base.CGet<GameObject>("DangerHint");
			container.SetActive(false);
		}
		else
		{
			this.OnWorldMapPlayerAreaChange(null);
		}
	}

	// Token: 0x060039C8 RID: 14792 RVA: 0x001D66BA File Offset: 0x001D48BA
	private void OnTaiwuOriginalSurnameSettingChanged(ArgumentBox argumentBox)
	{
		this.RefreshWorldState();
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x001D66C4 File Offset: 0x001D48C4
	public void ShowWorldStatePanel()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		UIElement.WorldStatePanel.SetOnInitArgs(args);
		UIManager.Instance.MaskUI(UIElement.WorldStatePanel);
	}

	// Token: 0x060039CA RID: 14794 RVA: 0x001D66F4 File Offset: 0x001D48F4
	private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
	{
		this._uiAnim.PlayHideAnimation(null, true);
	}

	// Token: 0x060039CB RID: 14795 RVA: 0x001D6704 File Offset: 0x001D4904
	private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
	{
		this._uiAnim.PlayShowAnimation(null, true);
	}

	// Token: 0x060039CC RID: 14796 RVA: 0x001D6714 File Offset: 0x001D4914
	private static void SetActiveByChildren(Transform t)
	{
		bool hasActive = false;
		for (int i = 0; i < t.childCount; i++)
		{
			bool activeSelf = t.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				hasActive = true;
				break;
			}
		}
		t.gameObject.SetActive(hasActive);
	}

	// Token: 0x170005D3 RID: 1491
	// (get) Token: 0x060039CD RID: 14797 RVA: 0x001D6764 File Offset: 0x001D4964
	private float XiangshuLevelIconSpacing
	{
		get
		{
			return this._levelIconHolder.parent.GetComponent<RectTransform>().rect.width / 9f;
		}
	}

	// Token: 0x060039CE RID: 14798 RVA: 0x001D6794 File Offset: 0x001D4994
	private void InitLevelIcons()
	{
		CImage[] icons = this._levelIconHolder.GetComponentsInChildren<CImage>();
		for (int i = 0; i < icons.Length; i++)
		{
			CImage icon = icons[i];
			RectTransform rectTransform = icon.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2((float)(i + 1) * this.XiangshuLevelIconSpacing, 0f);
		}
	}

	// Token: 0x060039CF RID: 14799 RVA: 0x001D67EC File Offset: 0x001D49EC
	private void UpdateXiangshuProcess(WorldStateItem worldStateItem, bool isTrue)
	{
		RectTransform xiangshuLevelRect = base.CGet<RectTransform>("XiangShuLevel");
		Transform stateRect = xiangshuLevelRect.Find("WorldState_" + worldStateItem.TemplateId.ToString());
		GameObject state = (stateRect != null) ? stateRect.gameObject : null;
		bool flag = !isTrue;
		if (flag)
		{
			bool flag2 = state != null;
			if (flag2)
			{
				state.SetActive(false);
			}
		}
		else
		{
			bool flag3 = state == null;
			if (flag3)
			{
				state = this.GetCurObject(worldStateItem, isTrue, xiangshuLevelRect, -1);
				state.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.ColorReplace();
				state.SetActive(true);
			}
		}
		sbyte xiangshuProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
		sbyte xiangshuLevel = GameData.Domains.World.SharedMethods.GetXiangshuLevel(xiangshuProgress);
		float fillAmount = (float)xiangshuProgress / 18f;
		base.CGet<CImage>("XiangshuLevelBar").fillAmount = fillAmount;
		for (int i = 0; i < this._levelIconHolder.childCount; i++)
		{
			CImage image = this._levelIconHolder.GetChild(i).GetComponent<CImage>();
			bool flag4 = (int)xiangshuLevel > i;
			if (flag4)
			{
				image.SetSprite("ui_main_bottom_scale_1", false, null);
				TooltipInvoker mouseTip = this._levelTipsHolder.GetChild(i).GetComponent<TooltipInvoker>();
				mouseTip.NeedRefresh = true;
				mouseTip.PresetParam = new string[]
				{
					WorldState.Instance[this._invasionWorldStateTemplateIds[i + 1]].Name
				};
				mouseTip.enabled = true;
			}
			else
			{
				image.SetSprite("ui_main_bottom_scale_0", false, null);
			}
		}
		this._currentLevel.anchoredPosition = new Vector2(fillAmount * this._currentLevel.parent.GetComponent<RectTransform>().rect.width, this._currentLevel.anchoredPosition.y);
	}

	// Token: 0x060039D0 RID: 14800 RVA: 0x001D69C8 File Offset: 0x001D4BC8
	private void InitRefers()
	{
		this._time = base.CGet<TextMeshProUGUI>("Time");
		this._xiangshuLevelBar = base.CGet<CImage>("XiangshuLevelBar");
		this._worldStateTemplate = base.CGet<GameObject>("WorldStateTemplate");
		this._xiangShuAwakeList = base.CGet<RectTransform>("XiangShuAwakeList");
		this._xiangShuAttackList = base.CGet<RectTransform>("XiangShuAttackList");
		this._solarTermName = base.CGet<TextMeshProUGUI>("SolarTermName");
		this._levelIconHolder = base.CGet<RectTransform>("LevelIconHolder");
		this._stateIconHolder = base.CGet<RectTransform>("StateIconHolder");
		this._taskHint = base.CGet<GameObject>("TaskHint");
		this._taskHintText = base.CGet<TextMeshProUGUI>("TaskHintText");
		this._dangerHint = base.CGet<GameObject>("DangerHint");
		this._dangerHintText = base.CGet<TextMeshProUGUI>("DangerHintText");
		this._sectTaskLisk = base.CGet<RectTransform>("SectTaskLisk");
		this._xiangShuLevel = base.CGet<RectTransform>("XiangShuLevel");
		this._otherStateBase = base.CGet<RectTransform>("OtherStateBase");
		this._focusModeUpMask = base.CGet<CButtonObsolete>("FocusModeUpMask");
		this._focusModeDownMask = base.CGet<CButtonObsolete>("FocusModeDownMask");
		this._focusModeIconMask = base.CGet<CButtonObsolete>("FocusModeIconMask");
		this._currentLevel = base.CGet<RectTransform>("CurrentLevel");
		this._levelTipsHolder = base.CGet<RectTransform>("LevelTipsHolder");
	}

	// Token: 0x0400299F RID: 10655
	private int _taiwuCharId;

	// Token: 0x040029A0 RID: 10656
	private int _moveTimeCostPercent;

	// Token: 0x040029A1 RID: 10657
	private List<IntPair> _overweightSanctionPercent;

	// Token: 0x040029A2 RID: 10658
	private WorldStateData _worldStateData;

	// Token: 0x040029A3 RID: 10659
	private const string WorldStatePath = "WorldStateTemplate_Path";

	// Token: 0x040029A4 RID: 10660
	private const string WorldStateNamePre = "WorldState_";

	// Token: 0x040029A5 RID: 10661
	private List<sbyte> _curSectTask = new List<sbyte>();

	// Token: 0x040029A6 RID: 10662
	private RectTransform _stateHolder;

	// Token: 0x040029A7 RID: 10663
	private RectTransform _otherStateRect;

	// Token: 0x040029A8 RID: 10664
	private Dictionary<short, ushort> _loongDebuff = new Dictionary<short, ushort>();

	// Token: 0x040029A9 RID: 10665
	private sbyte _xiangshuProgress;

	// Token: 0x040029AA RID: 10666
	private List<MartialArtTournamentPreparationInfo> _martialArtTournamentPreparationInfoList;

	// Token: 0x040029AB RID: 10667
	private int _tournamentPreparationEndDate;

	// Token: 0x040029AC RID: 10668
	private Dictionary<short, SettlementNameRelatedData> _settlementNameRelatedDataCache = new Dictionary<short, SettlementNameRelatedData>();

	// Token: 0x040029AD RID: 10669
	private readonly Dictionary<short, LanguageKey> _sectShortNameCache = new Dictionary<short, LanguageKey>
	{
		{
			19,
			LanguageKey.LK_Sect_Name_Short_1
		},
		{
			20,
			LanguageKey.LK_Sect_Name_Short_2
		},
		{
			21,
			LanguageKey.LK_Sect_Name_Short_3
		},
		{
			22,
			LanguageKey.LK_Sect_Name_Short_4
		},
		{
			23,
			LanguageKey.LK_Sect_Name_Short_5
		},
		{
			24,
			LanguageKey.LK_Sect_Name_Short_6
		},
		{
			25,
			LanguageKey.LK_Sect_Name_Short_7
		},
		{
			26,
			LanguageKey.LK_Sect_Name_Short_8
		},
		{
			27,
			LanguageKey.LK_Sect_Name_Short_9
		},
		{
			28,
			LanguageKey.LK_Sect_Name_Short_10
		},
		{
			29,
			LanguageKey.LK_Sect_Name_Short_11
		},
		{
			30,
			LanguageKey.LK_Sect_Name_Short_12
		},
		{
			31,
			LanguageKey.LK_Sect_Name_Short_13
		},
		{
			32,
			LanguageKey.LK_Sect_Name_Short_14
		},
		{
			33,
			LanguageKey.LK_Sect_Name_Short_15
		}
	};

	// Token: 0x040029AE RID: 10670
	private readonly List<sbyte> _invasionWorldStateTemplateIds = new List<sbyte>
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9
	};

	// Token: 0x040029AF RID: 10671
	private UIAnim _uiAnim;

	// Token: 0x040029B0 RID: 10672
	private const int MaxXiangshuLevel = 9;

	// Token: 0x040029B1 RID: 10673
	private TextMeshProUGUI _time;

	// Token: 0x040029B2 RID: 10674
	private CImage _xiangshuLevelBar;

	// Token: 0x040029B3 RID: 10675
	private GameObject _worldStateTemplate;

	// Token: 0x040029B4 RID: 10676
	private RectTransform _xiangShuAwakeList;

	// Token: 0x040029B5 RID: 10677
	private RectTransform _xiangShuAttackList;

	// Token: 0x040029B6 RID: 10678
	private TextMeshProUGUI _solarTermName;

	// Token: 0x040029B7 RID: 10679
	private RectTransform _levelIconHolder;

	// Token: 0x040029B8 RID: 10680
	private RectTransform _stateIconHolder;

	// Token: 0x040029B9 RID: 10681
	private GameObject _taskHint;

	// Token: 0x040029BA RID: 10682
	private TextMeshProUGUI _taskHintText;

	// Token: 0x040029BB RID: 10683
	private GameObject _dangerHint;

	// Token: 0x040029BC RID: 10684
	private TextMeshProUGUI _dangerHintText;

	// Token: 0x040029BD RID: 10685
	private RectTransform _sectTaskLisk;

	// Token: 0x040029BE RID: 10686
	private RectTransform _xiangShuLevel;

	// Token: 0x040029BF RID: 10687
	private RectTransform _otherStateBase;

	// Token: 0x040029C0 RID: 10688
	private CButtonObsolete _focusModeUpMask;

	// Token: 0x040029C1 RID: 10689
	private CButtonObsolete _focusModeDownMask;

	// Token: 0x040029C2 RID: 10690
	private CButtonObsolete _focusModeIconMask;

	// Token: 0x040029C3 RID: 10691
	private RectTransform _currentLevel;

	// Token: 0x040029C4 RID: 10692
	private RectTransform _levelTipsHolder;
}
