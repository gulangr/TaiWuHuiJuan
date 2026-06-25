using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using Game.Views.Looping;
using Game.Views.Main.Reading;
using GameData.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Adventure;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Global;
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

namespace Game.Views.Bottom
{
	// Token: 0x02000C5A RID: 3162
	public class ViewWorldState : UIBase
	{
		// Token: 0x170010EA RID: 4330
		// (get) Token: 0x0600A104 RID: 41220 RVA: 0x004B2E2C File Offset: 0x004B102C
		private bool IgnoreDangerHint
		{
			get
			{
				AdventureTaiwu adventureTaiwu = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu;
				bool result;
				if (adventureTaiwu == null)
				{
					AdventureMajorEventTaiwu adventureMajorEventTaiwu = SingletonObject.getInstance<AdventureRemakeModel>().AdventureMajorEventTaiwu;
					result = ((adventureMajorEventTaiwu != null && adventureMajorEventTaiwu.InAdventure) || UIManager.Instance.IsFocusElement(UIElement.BuildingArea) || WorldMapModel.Traveling);
				}
				else
				{
					result = adventureTaiwu.InAdventure;
				}
				return result;
			}
		}

		// Token: 0x0600A105 RID: 41221 RVA: 0x004B2E80 File Offset: 0x004B1080
		public override void OnInit(ArgumentBox argsBox)
		{
			BasicGameData basicData = SingletonObject.getInstance<BasicGameData>();
			this._taiwuCharId = basicData.TaiwuCharId;
			this.OnWorldMapPlayerAreaChange(null);
			this.uiAnim.Init(Vector3.zero, Vector3.zero.SetY(500f));
		}

		// Token: 0x0600A106 RID: 41222 RVA: 0x004B2EC8 File Offset: 0x004B10C8
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 22, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 62, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(1, 31, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(3, 7, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(3, 18, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 95, ulong.MaxValue, null));
			bool flag = !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (flag)
			{
				this.MonitorFields.Add(new UIBase.MonitorDataField(1, 1, ulong.MaxValue, null));
			}
		}

		// Token: 0x0600A107 RID: 41223 RVA: 0x004B2F8C File Offset: 0x004B118C
		private void Awake()
		{
			this._xiangshuProgress = -1;
			PoolManager.SetSrcObject("WorldStateTemplate_Path", this.worldStateTemplate);
			this.shortDisplayModeBtn.ClearAndAddListener(new Action(this.OnToggleDisplayMode));
			this.fullDisplayModeBtn.ClearAndAddListener(new Action(this.OnToggleDisplayMode));
			this.timeBack.ClearAndAddListener(new Action(this.ShowWorldStatePanel));
			this.timeBack.GetComponent<CEmptyGraphic>().enabled = !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			this.shortDisplayModeBtn.gameObject.SetActive(false);
			this.fullDisplayModeBtn.gameObject.SetActive(false);
			GEvent.Add(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Add(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
		}

		// Token: 0x0600A108 RID: 41224 RVA: 0x004B306E File Offset: 0x004B126E
		private void OnToggleDisplayMode()
		{
			this._isShowAllStates = !this._isShowAllStates;
			this.RefreshAllStateDisplay();
		}

		// Token: 0x0600A109 RID: 41225 RVA: 0x004B3087 File Offset: 0x004B1287
		private void RefreshAllStateDisplay()
		{
			this.shortDisplayModeBtn.gameObject.SetActive(!this._isShowAllStates);
			this.fullDisplayModeBtn.gameObject.SetActive(this._isShowAllStates);
			this.RefreshWorldState();
		}

		// Token: 0x0600A10A RID: 41226 RVA: 0x004B30C4 File Offset: 0x004B12C4
		private void OnDestroy()
		{
			PoolManager.RemoveData("WorldStateTemplate_Path");
			this._worldStateObjects.Clear();
			this._generalStateObjects.Clear();
			this._sectTaskStateObjects.Clear();
			this._xiangshuLevelStateObjects.Clear();
			this._xiangshuAwakeStateObjects.Clear();
			this._xiangshuAttackStateObjects.Clear();
			GEvent.Remove(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Remove(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
		}

		// Token: 0x0600A10B RID: 41227 RVA: 0x004B3158 File Offset: 0x004B1358
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
			GEvent.Add(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIElementHide));
			GEvent.Add(UiEvents.OnForceUpdateTime, new GEvent.Callback(this.ForceUpdateTime));
			this.UpdateTime(null);
			this.UpdateSolarTerm(null);
		}

		// Token: 0x0600A10C RID: 41228 RVA: 0x004B327C File Offset: 0x004B147C
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
			GEvent.Remove(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIElementHide));
			GEvent.Remove(UiEvents.OnForceUpdateTime, new GEvent.Callback(this.ForceUpdateTime));
		}

		// Token: 0x0600A10D RID: 41229 RVA: 0x004B3390 File Offset: 0x004B1590
		private void LateUpdate()
		{
			RectTransform holder = this.stateIconHolder;
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
			bool flag = !this._isShowAllStates;
			if (flag)
			{
				List<ViewWorldState.StateActiveData> allStateData = this._worldStateObjects.Values.ToList<ViewWorldState.StateActiveData>();
				allStateData.Sort((ViewWorldState.StateActiveData a, ViewWorldState.StateActiveData b) => a.State.transform.GetSiblingIndex().CompareTo(b.State.transform.GetSiblingIndex()));
				int activeCount = 0;
				for (int j = 0; j < allStateData.Count; j++)
				{
					ViewWorldState.StateActiveData data = allStateData[j];
					bool flag2 = activeCount < 8 && data.ShouldActive;
					if (flag2)
					{
						data.State.SetActive(true);
						activeCount++;
					}
					else
					{
						data.State.SetActive(false);
					}
				}
				this.UpdateStateHolderSize();
			}
			else
			{
				List<ViewWorldState.StateActiveData> allStateData2 = this._worldStateObjects.Values.ToList<ViewWorldState.StateActiveData>();
				foreach (ViewWorldState.StateActiveData data2 in allStateData2)
				{
					data2.State.SetActive(data2.ShouldActive);
				}
				this.UpdateStateHolderSize();
			}
		}

		// Token: 0x0600A10E RID: 41230 RVA: 0x004B3520 File Offset: 0x004B1720
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
						this.RefreshGuidings();
						this.RefreshWorldState();
						this.UpdateSectTaskTips();
						int totalStateCount = this.GetCurrentAllStateCount();
						bool shouldShowToggleButtons = totalStateCount > 8;
						bool flag2 = shouldShowToggleButtons;
						if (flag2)
						{
							this.shortDisplayModeBtn.gameObject.SetActive(!this._isShowAllStates);
							this.fullDisplayModeBtn.gameObject.SetActive(this._isShowAllStates);
						}
						else
						{
							this.shortDisplayModeBtn.gameObject.SetActive(false);
							this.fullDisplayModeBtn.gameObject.SetActive(false);
						}
					}
					else
					{
						bool flag3 = uid.DomainId == 5;
						if (flag3)
						{
							bool flag4 = uid.DataId == 22;
							if (flag4)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._moveTimeCostPercent);
							}
							else
							{
								bool flag5 = uid.DataId == 62;
								if (flag5)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._overweightSanctionPercent);
									this.UpdateInventoryOverflowTips(WorldState.Instance[11]);
								}
							}
						}
						else
						{
							bool flag6 = uid.DomainId == 3 && uid.DataId == 7;
							if (flag6)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._martialArtTournamentPreparationInfoList);
								this.UpdateMartialArtTournament(null);
								this.Element.ShowAfterRefresh();
							}
							else
							{
								bool flag7 = uid.DomainId == 3 && uid.DataId == 18;
								if (flag7)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._tournamentPreparationEndDate);
									this.UpdateMartialArtTournament(null);
								}
								else
								{
									bool flag8 = uid.DomainId == 19 && uid.DataId == 95;
									if (flag8)
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
										bool flag9 = uid.DomainId == 1 && uid.DataId == 1;
										if (flag9)
										{
											bool initial = this._xiangshuProgress < 0;
											sbyte progressOld = this._xiangshuProgress;
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._xiangshuProgress);
											sbyte levelOld = GameData.Domains.World.SharedMethods.GetXiangshuLevel(progressOld);
											sbyte levelNew = GameData.Domains.World.SharedMethods.GetXiangshuLevel(this._xiangshuProgress);
											bool flag10 = !initial && this._xiangshuProgress > progressOld;
											if (flag10)
											{
												ViewWorldState.<>c__DisplayClass64_0 CS$<>8__locals1 = new ViewWorldState.<>c__DisplayClass64_0();
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
			this.UpdateStateHolderSize();
		}

		// Token: 0x0600A10F RID: 41231 RVA: 0x004B3934 File Offset: 0x004B1B34
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
													bool flag10 = worldStateItem.TemplateId == 22;
													if (flag10)
													{
														this.UpdateMartialArtTournamentPrepare();
													}
													else
													{
														bool flag11 = worldStateItem.TemplateId == 24;
														if (flag11)
														{
															this.UpdateMartialArtTournamentOpen();
														}
														else
														{
															bool flag12 = worldStateItem.TemplateId >= 42 && worldStateItem.TemplateId <= 46;
															if (flag12)
															{
																this.UpdateLoongDebuff(null);
															}
															else
															{
																bool flag13 = worldStateItem.TemplateId == 49;
																if (flag13)
																{
																	this.UpdateTaiwuWanted(worldState);
																}
																else
																{
																	bool flag14 = worldStateItem.TemplateId == 51;
																	if (flag14)
																	{
																		this.UpdateTearmateDyingTips(worldStateItem, worldState);
																	}
																	else
																	{
																		bool flag15 = worldStateItem.TemplateId == 52;
																		if (flag15)
																		{
																			this.UpdateHomelessVillagerTips(worldStateItem, worldState);
																		}
																		else
																		{
																			bool flag16 = worldStateItem.TemplateId == 53;
																			if (flag16)
																			{
																				this.UpdateNeiliConflictingTips(worldStateItem, worldState);
																			}
																			else
																			{
																				bool flag17 = worldStateItem.TemplateId >= 25 && worldStateItem.TemplateId <= 39;
																				if (flag17)
																				{
																					bool flag18 = worldState;
																					if (flag18)
																					{
																						this._curSectTask.Add(worldStateItem.TemplateId);
																					}
																				}
																				else
																				{
																					bool flag19 = worldStateItem.TemplateId == 54;
																					if (flag19)
																					{
																						this.UpdatePracticeNotice(worldStateItem, worldState);
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
			}
		}

		// Token: 0x0600A110 RID: 41232 RVA: 0x004B3C10 File Offset: 0x004B1E10
		private void UpdateStateHolderSize()
		{
			int stateCount = this.GetCurrentDisplayCount();
			bool flag = stateCount == 0;
			if (flag)
			{
				this.stateIconHolder.sizeDelta = new Vector2(0f, 0f);
			}
			else
			{
				int itemsInRow = Mathf.Min(stateCount, 8);
				float totalWidth = 53f + 45f * (float)itemsInRow + 12f * (float)(itemsInRow - 1);
				int rows = Mathf.CeilToInt((float)stateCount / 8f);
				float totalHeight = 8f + 45f * (float)rows + 6f * (float)(rows - 1);
				this.stateIconHolder.sizeDelta = new Vector2(totalWidth, totalHeight);
			}
		}

		// Token: 0x0600A111 RID: 41233 RVA: 0x004B3CAC File Offset: 0x004B1EAC
		private ViewWorldState.StateActiveData GetCurObject(WorldStateItem worldStateItem, bool needShow, int extraId = -1, ViewWorldState.EnumWorldStateType category = ViewWorldState.EnumWorldStateType.General)
		{
			string objName = "WorldState_" + worldStateItem.TemplateId.ToString() + ((extraId >= 0) ? string.Format("_{0}", extraId) : "");
			ViewWorldState.StateActiveData stateActiveData;
			this._worldStateObjects.TryGetValue(objName, out stateActiveData);
			bool flag = !needShow;
			if (flag)
			{
				bool flag2 = stateActiveData != null;
				if (flag2)
				{
					stateActiveData.ShouldActive = false;
				}
			}
			else
			{
				bool flag3 = stateActiveData == null;
				if (flag3)
				{
					GameObject state = PoolManager.GetObject("WorldStateTemplate_Path");
					state.name = objName;
					state.transform.SetParent(this.stateIconHolder, false);
					state.GetComponent<TooltipInvoker>().PresetParam[0] = worldStateItem.Name.ColorReplace();
					stateActiveData = new ViewWorldState.StateActiveData
					{
						State = state,
						ShouldActive = true
					};
					stateActiveData.State = state;
					this.AddToCategoryList(objName, stateActiveData, category);
				}
				stateActiveData.State.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
				stateActiveData.ShouldActive = true;
			}
			bool flag4 = stateActiveData != null;
			if (flag4)
			{
				TooltipInvoker display = stateActiveData.State.GetComponent<TooltipInvoker>();
				display.RuntimeParam = null;
				display.enabled = true;
				CButton btn = stateActiveData.State.GetComponent<CButton>();
				btn.enabled = true;
				btn.ClearAndAddListener(delegate
				{
					this.OnJumpButtonClicked(worldStateItem, UIManager.Instance.UiCamera.WorldToScreenPoint(btn.transform.position));
				});
			}
			return stateActiveData;
		}

		// Token: 0x0600A112 RID: 41234 RVA: 0x004B3E50 File Offset: 0x004B2050
		private int GetCurrentDisplayCount()
		{
			int count = 0;
			for (int i = 0; i < this.stateIconHolder.childCount; i++)
			{
				GameObject child = this.stateIconHolder.GetChild(i).gameObject;
				bool flag = child.activeSelf && child.name.StartsWith("WorldState_");
				if (flag)
				{
					count++;
				}
			}
			return count;
		}

		// Token: 0x0600A113 RID: 41235 RVA: 0x004B3EBC File Offset: 0x004B20BC
		private int GetCurrentAllStateCount()
		{
			List<ViewWorldState.StateActiveData> allStateList = this._worldStateObjects.Values.ToList<ViewWorldState.StateActiveData>();
			return allStateList.Count((ViewWorldState.StateActiveData stateActiveData) => stateActiveData.ShouldActive);
		}

		// Token: 0x0600A114 RID: 41236 RVA: 0x004B3F04 File Offset: 0x004B2104
		private void AddToCategoryList(string objName, ViewWorldState.StateActiveData obj, ViewWorldState.EnumWorldStateType category)
		{
			switch (category)
			{
			case ViewWorldState.EnumWorldStateType.SectTask:
			{
				bool flag = !this._sectTaskStateObjects.Contains(obj);
				if (flag)
				{
					this._sectTaskStateObjects.Add(obj);
				}
				goto IL_D7;
			}
			case ViewWorldState.EnumWorldStateType.XiangshuLevel:
			{
				bool flag2 = !this._xiangshuLevelStateObjects.Contains(obj);
				if (flag2)
				{
					this._xiangshuLevelStateObjects.Add(obj);
				}
				goto IL_D7;
			}
			case ViewWorldState.EnumWorldStateType.XiangshuAwake:
			{
				bool flag3 = !this._xiangshuAwakeStateObjects.Contains(obj);
				if (flag3)
				{
					this._xiangshuAwakeStateObjects.Add(obj);
				}
				goto IL_D7;
			}
			case ViewWorldState.EnumWorldStateType.XiangshuAttack:
			{
				bool flag4 = !this._xiangshuAttackStateObjects.Contains(obj);
				if (flag4)
				{
					this._xiangshuAttackStateObjects.Add(obj);
				}
				goto IL_D7;
			}
			}
			bool flag5 = !this._generalStateObjects.Contains(obj);
			if (flag5)
			{
				this._generalStateObjects.Add(obj);
			}
			IL_D7:
			bool flag6 = !this._worldStateObjects.TryAdd(objName, obj);
			if (flag6)
			{
				Debug.LogError("Try add same state name, that shouldn't happen");
			}
		}

		// Token: 0x0600A115 RID: 41237 RVA: 0x004B400C File Offset: 0x004B220C
		internal void UpdateSectTaskTips()
		{
			for (int i = 0; i < this._curSectTask.Count; i++)
			{
				WorldStateItem worldStateItem = WorldState.Instance.GetItem(this._curSectTask[i]);
				bool worldState = this._worldStateData.GetWorldState((short)worldStateItem.TemplateId);
				bool flag = i >= this._sectTaskStateObjects.Count;
				ViewWorldState.StateActiveData data;
				if (flag)
				{
					data = this.GetCurObject(worldStateItem, worldState, -1, ViewWorldState.EnumWorldStateType.SectTask);
				}
				else
				{
					data = this._sectTaskStateObjects[i];
				}
				data.State.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
				data.ShouldActive = worldState;
				TooltipInvoker mouseTip = data.State.GetComponent<TooltipInvoker>();
				StoryDomainMethod.AsyncCall.GetSectMainStoryTriggerConditions(this, (short)(data.State.GetComponent<WorldStateToggle>().SectId = worldStateItem.Sect), delegate(int offset, RawDataPool dataPool)
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
					CButton btn = data.State.GetComponent<CButton>();
					btn.enabled = true;
					btn.ClearAndAddListener(delegate
					{
						ArgumentBox args = EasyPool.Get<ArgumentBox>();
						args.Set("TemplateId", worldStateItem.TemplateId).Set("ConditionStatus", status).Set("IsShowConfirmButton", false);
						UIElement.SectStoryPopUpToggle.SetOnInitArgs(args);
						UIManager.Instance.MaskUI(UIElement.SectStoryPopUpToggle);
					});
				});
			}
			for (int j = this._curSectTask.Count; j < this._sectTaskStateObjects.Count; j++)
			{
				this._sectTaskStateObjects[j].ShouldActive = false;
			}
		}

		// Token: 0x0600A116 RID: 41238 RVA: 0x004B4170 File Offset: 0x004B2370
		private void UpdateTime(ArgumentBox argBox = null)
		{
			bool flag = this.CheckShowUnknowTime();
			if (flag)
			{
				this.UpdateTimeUnknown();
			}
			else
			{
				this.UpdateTimeNormal();
			}
		}

		// Token: 0x0600A117 RID: 41239 RVA: 0x004B419C File Offset: 0x004B239C
		private void ForceUpdateTime(ArgumentBox argBox = null)
		{
			bool normalFlag;
			bool showNormalTime = argBox == null || (argBox.Get("normal", out normalFlag) && normalFlag);
			bool flag = !showNormalTime;
			if (flag)
			{
				this.UpdateTimeUnknown();
			}
			else
			{
				this.UpdateTimeNormal();
			}
		}

		// Token: 0x0600A118 RID: 41240 RVA: 0x004B41DC File Offset: 0x004B23DC
		private bool CheckShowUnknowTime()
		{
			return this.CheckInPast();
		}

		// Token: 0x0600A119 RID: 41241 RVA: 0x004B41F4 File Offset: 0x004B23F4
		private bool CheckInPast()
		{
			return SingletonObject.getInstance<WorldMapModel>().CurrentAreaId == 139;
		}

		// Token: 0x0600A11A RID: 41242 RVA: 0x004B4218 File Offset: 0x004B2418
		private void UpdateTimeUnknown()
		{
			TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
			this.time.text = LanguageKey.LK_ThreeQuestionMark.Tr();
			this.CalTimeHoverRect();
		}

		// Token: 0x0600A11B RID: 41243 RVA: 0x004B424C File Offset: 0x004B244C
		private void UpdateTimeNormal()
		{
			TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
			this.time.text = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
			{
				timeManager.GetYear(),
				(int)(timeManager.GetMonthInCurrYear() + 1),
				Month.Instance[timeManager.GetMonthInCurrYear()].Name + LocalStringManager.Get(LanguageKey.LK_Dot_Symbol),
				LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetCurrSeason()))
			});
			this.CalTimeHoverRect();
		}

		// Token: 0x0600A11C RID: 41244 RVA: 0x004B42E4 File Offset: 0x004B24E4
		private void CalTimeHoverRect()
		{
			bool flag = this.time == null || this.timeHoverRect == null;
			if (!flag)
			{
				this.time.ForceMeshUpdate(false, false);
				float x = this.time.preferredWidth;
				float y = this.time.preferredHeight;
				this.timeHoverRect.SetSize(new Vector2(x + this.timeHoverSizeOffset.x, y + this.timeHoverSizeOffset.y));
			}
		}

		// Token: 0x0600A11D RID: 41245 RVA: 0x004B4368 File Offset: 0x004B2568
		private void UpdateInventoryOverflowTips(WorldStateItem worldStateItem)
		{
			bool isTrue = this._overweightSanctionPercent != null && this._worldStateData.GetWorldState((short)worldStateItem.TemplateId);
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
			if (!flag)
			{
				TooltipInvoker mouseTip = data.State.GetComponent<TooltipInvoker>();
				MouseTip_Util.SetInventoryOverLoadTips(mouseTip, this._overweightSanctionPercent, this._moveTimeCostPercent);
			}
		}

		// Token: 0x0600A11E RID: 41246 RVA: 0x004B43D0 File Offset: 0x004B25D0
		private void UpdateGeneralTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
			if (!flag)
			{
				data.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.ColorReplace();
			}
		}

		// Token: 0x0600A11F RID: 41247 RVA: 0x004B4418 File Offset: 0x004B2618
		private void UpdateResourceOverflowTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
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
						resourceNameBuilder.Append(Config.ResourceType.Instance[type].Name);
					}
				}
				data.ShouldActive = (resourceNameBuilder.Length > 0);
				bool flag4 = resourceNameBuilder.Length > 0;
				if (flag4)
				{
					data.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(resourceNameBuilder.ToString()).ColorReplace();
				}
				EasyPool.Free<StringBuilder>(resourceNameBuilder);
			}
		}

		// Token: 0x0600A120 RID: 41248 RVA: 0x004B4500 File Offset: 0x004B2700
		private void UpdateWarehouseOverflowTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
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
				data.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(sb.ToString()).ColorReplace();
				EasyPool.Free<StringBuilder>(sb);
			}
		}

		// Token: 0x0600A121 RID: 41249 RVA: 0x004B460C File Offset: 0x004B280C
		private void UpdateInjuryTips(WorldStateItem worldStateItem, bool isOuterInjury, bool isTrue)
		{
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
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
				data.ShouldActive = (strBuilder.Length > 0);
				bool flag4 = strBuilder.Length > 0;
				if (flag4)
				{
					data.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(strBuilder.ToString()).ColorReplace();
				}
				EasyPool.Free<StringBuilder>(strBuilder);
			}
		}

		// Token: 0x0600A122 RID: 41250 RVA: 0x004B4708 File Offset: 0x004B2908
		private void UpdatePoisonTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
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
				data.ShouldActive = (strBuilder.Length > 0);
				bool flag4 = strBuilder.Length > 0;
				if (flag4)
				{
					data.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(strBuilder.ToString()).ColorReplace();
				}
				EasyPool.Free<StringBuilder>(strBuilder);
			}
		}

		// Token: 0x0600A123 RID: 41251 RVA: 0x004B480C File Offset: 0x004B2A0C
		private void UpdateTeammateInjuryTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData data = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = data == null || !isTrue;
			if (!flag)
			{
				TaiwuDomainMethod.AsyncCall.GetSeverelyInjuredGroupCharNames(this, delegate(int offset, RawDataPool dataPool)
				{
					List<CharNameRelatedData> charNames = null;
					Serializer.Deserialize(dataPool, offset, ref charNames);
					bool flag2 = charNames != null && charNames.Count > 0;
					if (flag2)
					{
						this._severelyInjuredCharIds.Clear();
						List<NameRelatedData> nameList = EasyPool.Get<List<NameRelatedData>>();
						nameList.Clear();
						foreach (CharNameRelatedData charName in charNames)
						{
							this._severelyInjuredCharIds.Add(charName.CharId);
							nameList.Add(charName.NameData);
						}
						data.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(NameCenter.GetNameSequenceStringByNameRelatedDataList(nameList, false).SetColor("brightred")).ColorReplace();
						data.ShouldActive = true;
						EasyPool.Free<List<NameRelatedData>>(nameList);
					}
					else
					{
						data.ShouldActive = false;
					}
				});
			}
		}

		// Token: 0x0600A124 RID: 41252 RVA: 0x004B486C File Offset: 0x004B2A6C
		private void UpdateXiangshuAwakeTips(WorldStateItem worldStateItem, bool isTrue)
		{
			int awakeCount = -1;
			for (sbyte i = 0; i < 9; i += 1)
			{
				bool isAwakening = this._worldStateData.IsXiangshuAvatarAwakening(i);
				string swordTombName = AdventureRemakeModel.Core.GetAdventureAny(SwordTomb.Instance[i].AdventureCoreId).Name;
				bool flag = !isAwakening;
				if (!flag)
				{
					awakeCount++;
					bool flag2 = awakeCount >= this._xiangshuAwakeStateObjects.Count;
					ViewWorldState.StateActiveData data;
					if (flag2)
					{
						data = this.GetCurObject(worldStateItem, isTrue, (int)i, ViewWorldState.EnumWorldStateType.XiangshuAwake);
					}
					else
					{
						data = this._xiangshuAwakeStateObjects[awakeCount];
						data.State.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
						data.ShouldActive = true;
					}
					TooltipInvoker tipDisplayer = data.State.GetComponent<TooltipInvoker>();
					tipDisplayer.PresetParam[1] = worldStateItem.Desc.GetFormat(swordTombName, this._worldStateData.GetMinAwakeSwordTombRemainMonths()).ColorReplace();
				}
			}
			for (int j = awakeCount + 1; j < this._xiangshuAwakeStateObjects.Count; j++)
			{
				this._xiangshuAwakeStateObjects[j].ShouldActive = false;
			}
		}

		// Token: 0x0600A125 RID: 41253 RVA: 0x004B49A4 File Offset: 0x004B2BA4
		private void UpdateXiangShuAttackTips(WorldStateItem worldStateItem, bool isTrue)
		{
			int attackCount = -1;
			for (sbyte i = 0; i < 9; i += 1)
			{
				bool isAttacking = this._worldStateData.IsXiangshuAvatarAttacking(i);
				string swordTombName = AdventureRemakeModel.Core.GetAdventureAny(SwordTomb.Instance[i].AdventureCoreId).Name;
				bool flag = !isAttacking;
				if (!flag)
				{
					attackCount++;
					bool flag2 = attackCount >= this._xiangshuAttackStateObjects.Count;
					ViewWorldState.StateActiveData data;
					if (flag2)
					{
						data = this.GetCurObject(worldStateItem, isTrue, (int)i, ViewWorldState.EnumWorldStateType.XiangshuAttack);
					}
					else
					{
						data = this._xiangshuAttackStateObjects[attackCount];
						data.State.GetComponent<CImage>().SetSprite(worldStateItem.Icon, true, null);
						data.ShouldActive = true;
					}
					TooltipInvoker tipDisplayer = data.State.GetComponent<TooltipInvoker>();
					tipDisplayer.PresetParam[1] = worldStateItem.Desc.GetFormat(swordTombName).ColorReplace();
				}
			}
			for (int j = attackCount + 1; j < this._xiangshuAttackStateObjects.Count; j++)
			{
				this._xiangshuAttackStateObjects[j].ShouldActive = false;
			}
		}

		// Token: 0x0600A126 RID: 41254 RVA: 0x004B4AC9 File Offset: 0x004B2CC9
		private void UpdateSolarTerm(ArgumentBox argBox = null)
		{
			this.solarTermName.text = "";
		}

		// Token: 0x0600A127 RID: 41255 RVA: 0x004B4ADD File Offset: 0x004B2CDD
		private void UpdateMartialArtTournament(ArgumentBox box = null)
		{
			this.UpdateMartialArtTournamentPrepare();
			this.UpdateMartialArtTournamentOpen();
		}

		// Token: 0x0600A128 RID: 41256 RVA: 0x004B4AF0 File Offset: 0x004B2CF0
		private void UpdateMartialArtTournamentPrepare()
		{
			bool needShow = this._worldStateData.GetWorldState(22);
			WorldStateItem worldStateItem = WorldState.DefValue.MartialArtTournamentPrepare;
			ViewWorldState.StateActiveData tipObj = this.GetCurObject(worldStateItem, needShow, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = !needShow || tipObj == null;
			if (!flag)
			{
				tipObj.ShouldActive = true;
				TooltipInvoker mouseTipDisplayer = tipObj.State.GetComponent<TooltipInvoker>();
				string[] tipsData = new string[2];
				mouseTipDisplayer.PresetParam = tipsData;
				CImage tipImg = tipObj.State.GetComponent<CImage>();
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

		// Token: 0x0600A129 RID: 41257 RVA: 0x004B4C34 File Offset: 0x004B2E34
		private void UpdateMartialArtTournamentOpen()
		{
			bool needShow = this._worldStateData.GetWorldState(24);
			WorldStateItem worldStateItem = WorldState.Instance.GetItem(24);
			ViewWorldState.StateActiveData tipObj = this.GetCurObject(worldStateItem, needShow, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = !needShow || tipObj == null;
			if (!flag)
			{
				tipObj.ShouldActive = true;
				CImage tipImg = tipObj.State.GetComponent<CImage>();
				tipImg.SetSprite(worldStateItem.Icon, true, null);
				string[] tipsData = new string[2];
				IAdventureRuntime adventure;
				int remainingMonth = SingletonObject.getInstance<AdventureRemakeModel>().TryGetAnyByCoreId(114668976, out adventure) ? adventure.RemainMonths : 0;
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
					tipObj.State.GetComponent<TooltipInvoker>().PresetParam = tipsData;
				});
			}
		}

		// Token: 0x0600A12A RID: 41258 RVA: 0x004B4D10 File Offset: 0x004B2F10
		private void UpdateLoongDebuff(ArgumentBox box = null)
		{
			foreach (KeyValuePair<short, ushort> pair in this._loongDebuff)
			{
				sbyte templateId = (sbyte)Loong.Instance[(int)(pair.Key - 246)].WorldState;
				WorldStateItem worldStateItem = WorldState.Instance.GetItem(templateId);
				bool needShow = this._worldStateData.GetWorldState((short)templateId);
				ViewWorldState.StateActiveData tipObj = this.GetCurObject(worldStateItem, needShow, -1, ViewWorldState.EnumWorldStateType.General);
				bool flag = pair.Value > 0;
				if (flag)
				{
					bool flag2 = !needShow || tipObj == null;
					if (flag2)
					{
						break;
					}
					tipObj.ShouldActive = needShow;
					CImage tipImg = tipObj.State.GetComponent<CImage>();
					tipImg.SetSprite(worldStateItem.Icon, true, null);
					tipObj.State.GetComponent<TooltipInvoker>().PresetParam[0] = worldStateItem.Name;
					tipObj.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.GetFormat(pair.Value.ToString()).ColorReplace();
				}
			}
		}

		// Token: 0x0600A12B RID: 41259 RVA: 0x004B4E58 File Offset: 0x004B3058
		private void UpdateTaiwuWanted(bool isTrue)
		{
			WorldStateItem worldStateItem = WorldState.Instance.GetItem(49);
			ViewWorldState.StateActiveData state = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = state == null || !isTrue;
			if (!flag)
			{
				TooltipInvoker display = state.State.GetComponent<TooltipInvoker>();
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

		// Token: 0x0600A12C RID: 41260 RVA: 0x004B4ED0 File Offset: 0x004B30D0
		private void UpdateTearmateDyingTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData state = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = state == null || !isTrue;
			if (!flag)
			{
				TaiwuDomainMethod.AsyncCall.GetDyingGroupCharNames(this, true, delegate(int offset, RawDataPool dataPool)
				{
					List<CharNameRelatedData> charNames = new List<CharNameRelatedData>();
					Serializer.Deserialize(dataPool, offset, ref charNames);
					bool includeTaiwu = false;
					int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					this._dyingCharIds.Clear();
					List<NameRelatedData> nameList = EasyPool.Get<List<NameRelatedData>>();
					nameList.Clear();
					foreach (CharNameRelatedData charName in charNames)
					{
						this._dyingCharIds.Add(charName.CharId);
						nameList.Add(charName.NameData);
						bool flag2 = charName.CharId == taiwuCharId;
						if (flag2)
						{
							includeTaiwu = true;
						}
					}
					bool flag3 = nameList.Count > 0;
					if (flag3)
					{
						state.State.GetComponent<TooltipInvoker>().PresetParam[1] = string.Format(worldStateItem.Desc, NameCenter.GetNameSequenceStringByNameRelatedDataList(nameList, includeTaiwu).SetColor("brightred")).ColorReplace();
						state.ShouldActive = true;
					}
					else
					{
						state.ShouldActive = false;
					}
					EasyPool.Free<List<NameRelatedData>>(nameList);
				});
			}
		}

		// Token: 0x0600A12D RID: 41261 RVA: 0x004B4F30 File Offset: 0x004B3130
		private void UpdateHomelessVillagerTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData state = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = state == null || !isTrue;
			if (!flag)
			{
				BuildingDomainMethod.AsyncCall.GetResidenceInfo(this, delegate(int offset, RawDataPool dataPool)
				{
					ValueTuple<int, int, int> info = default(ValueTuple<int, int, int>);
					Serializer.Deserialize(dataPool, offset, ref info);
					TooltipInvoker mouseTip = state.State.GetComponent<TooltipInvoker>();
					mouseTip.PresetParam[1] = string.Format(worldStateItem.Desc, info.Item1, info.Item2, info.Item3);
				});
			}
		}

		// Token: 0x0600A12E RID: 41262 RVA: 0x004B4F88 File Offset: 0x004B3188
		private void UpdateNeiliConflictingTips(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData state = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = state == null || !isTrue;
			if (!flag)
			{
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				TaiwuDomainMethod.AsyncCall.GetGroupNeiliConflictingCharDataList(this, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayData> list = new List<CharacterDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref list);
					TooltipInvoker mouseTip = state.State.GetComponent<TooltipInvoker>();
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

		// Token: 0x0600A12F RID: 41263 RVA: 0x004B4FF0 File Offset: 0x004B31F0
		private void UpdatePracticeNotice(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData state = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.General);
			bool flag = state == null || !isTrue;
			if (!flag)
			{
				TooltipInvoker mouseTip = state.State.GetComponent<TooltipInvoker>();
				mouseTip.Type = TipType.PracticeNotice;
				mouseTip.enabled = true;
				mouseTip.NeedRefresh = true;
				mouseTip.RuntimeParam = new ArgumentBox();
				mouseTip.RuntimeParam.SetObject("WorldStateData", this._worldStateData);
				mouseTip.Refresh(false, -1);
			}
		}

		// Token: 0x0600A130 RID: 41264 RVA: 0x004B5070 File Offset: 0x004B3270
		private void OnTaiwuCharIdChange(ArgumentBox argumentBox)
		{
			int newTaiwuCharId;
			argumentBox.Get("NewTaiwuCharId", out newTaiwuCharId);
			bool flag = newTaiwuCharId == this._taiwuCharId;
			if (!flag)
			{
				this._taiwuCharId = newTaiwuCharId;
				base.ClearMonitorFields();
				this.InitMonitorFieldIds();
				foreach (UIBase.MonitorDataField dataField in this.MonitorFields)
				{
					bool flag2 = dataField.SubId1List != null;
					if (flag2)
					{
						GameDataBridge.AddDataMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
					}
					else
					{
						GameDataBridge.AddDataMonitor(this.Element.GameDataListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
					}
				}
			}
		}

		// Token: 0x0600A131 RID: 41265 RVA: 0x004B5154 File Offset: 0x004B3354
		private void OnWorldMapPlayerAreaChange(ArgumentBox argumentBox)
		{
			GameObject container = this.dangerHint;
			container.SetActive(false);
			this.RefreshPastTaiwuVillage();
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
						TextMeshProUGUI text = this.dangerHintText;
						string info = LocalStringManager.Get(LanguageKey.LK_Danger_Area_Tips);
						text.text = info;
						container.SetActive(true);
					}
				}
			}
		}

		// Token: 0x0600A132 RID: 41266 RVA: 0x004B51EC File Offset: 0x004B33EC
		private void OnTopUiChanged(ArgumentBox argumentBox)
		{
			bool ignoreDangerHint = this.IgnoreDangerHint;
			if (ignoreDangerHint)
			{
				GameObject container = this.dangerHint;
				container.SetActive(false);
			}
			else
			{
				this.OnWorldMapPlayerAreaChange(null);
			}
		}

		// Token: 0x0600A133 RID: 41267 RVA: 0x004B5224 File Offset: 0x004B3424
		private void OnUIElementHide(ArgumentBox argumentBox)
		{
			UIElement element;
			bool flag = !argumentBox.Get<UIElement>("Element", out element);
			if (!flag)
			{
				bool flag2 = element == UIElement.XiangshuLevelChanged;
				if (flag2)
				{
					this.xiangshuParticle.SetActive(false);
					this.xiangshuParticle.SetActive(true);
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, delegate
					{
						this.xiangshuParticle.SetActive(false);
					});
				}
			}
		}

		// Token: 0x0600A134 RID: 41268 RVA: 0x004B528C File Offset: 0x004B348C
		private void OnTaiwuOriginalSurnameSettingChanged(ArgumentBox argumentBox)
		{
			this.RefreshWorldState();
		}

		// Token: 0x0600A135 RID: 41269 RVA: 0x004B5298 File Offset: 0x004B3498
		public void ShowWorldStatePanel()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			UIElement.WorldStatePanel.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.WorldStatePanel);
		}

		// Token: 0x0600A136 RID: 41270 RVA: 0x004B52C8 File Offset: 0x004B34C8
		private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
		{
			this.uiAnim.PlayHideAnimation(null, true);
		}

		// Token: 0x0600A137 RID: 41271 RVA: 0x004B52D8 File Offset: 0x004B34D8
		private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
		{
			this.uiAnim.PlayShowAnimation(null, true);
		}

		// Token: 0x0600A138 RID: 41272 RVA: 0x004B52E8 File Offset: 0x004B34E8
		private void UpdateXiangshuProcess(WorldStateItem worldStateItem, bool isTrue)
		{
			ViewWorldState.StateActiveData state = null;
			bool found = false;
			foreach (ViewWorldState.StateActiveData obj in this._xiangshuLevelStateObjects)
			{
				bool flag = obj.State.name == "WorldState_" + worldStateItem.TemplateId.ToString();
				if (flag)
				{
					state = obj;
					found = true;
					break;
				}
			}
			bool flag2 = !isTrue;
			if (flag2)
			{
				bool flag3 = state != null;
				if (flag3)
				{
					state.ShouldActive = false;
					this._xiangshuLevelStateObjects.Remove(state);
				}
			}
			else
			{
				bool flag4 = !found;
				if (flag4)
				{
					state = this.GetCurObject(worldStateItem, isTrue, -1, ViewWorldState.EnumWorldStateType.XiangshuLevel);
					bool flag5 = state != null;
					if (flag5)
					{
						state.State.GetComponent<TooltipInvoker>().PresetParam[1] = worldStateItem.Desc.ColorReplace();
					}
				}
				else
				{
					bool flag6 = state != null;
					if (flag6)
					{
						state.ShouldActive = true;
					}
				}
			}
			sbyte xiangshuProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
			sbyte xiangshuLevel = GameData.Domains.World.SharedMethods.GetXiangshuLevel(xiangshuProgress);
			float fillAmount = (float)(xiangshuLevel - 1) * (1f / (float)(this.levelIconHolder.childCount - 1));
			this.xiangshuLevelBar.fillAmount = fillAmount;
			for (int i = 0; i < this.levelIconHolder.childCount; i++)
			{
				CImage image = this.levelIconHolder.GetChild(i).GetComponent<CImage>();
				bool flag7 = (int)xiangshuLevel > i;
				if (flag7)
				{
					image.SetSprite("ui9_icon_world_state_progress_1", false, null);
					TooltipInvoker mouseTip = this.levelTipsHolder.GetChild(i).GetComponent<TooltipInvoker>();
					mouseTip.NeedRefresh = true;
					mouseTip.PresetParam = new string[]
					{
						WorldState.Instance[this._invasionWorldStateTemplateIds[i + 1]].Name
					};
					mouseTip.enabled = true;
				}
				else
				{
					image.SetSprite("ui9_icon_world_state_progress_0", false, null);
				}
			}
		}

		// Token: 0x0600A139 RID: 41273 RVA: 0x004B54F4 File Offset: 0x004B36F4
		private void OnJumpButtonClicked(WorldStateItem worldStateItem, Vector3 position)
		{
			ViewWorldState.<>c__DisplayClass109_0 CS$<>8__locals1 = new ViewWorldState.<>c__DisplayClass109_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.worldStateItem = worldStateItem;
			CS$<>8__locals1.position = position;
			if (this._popupMenuBtnList == null)
			{
				this._popupMenuBtnList = new List<ViewPopupMenu.BtnData>();
			}
			this._popupMenuBtnList.Clear();
			CS$<>8__locals1.displayOrder = 0;
			CS$<>8__locals1.pendingAsyncCalls = 0;
			sbyte templateId = CS$<>8__locals1.worldStateItem.TemplateId;
			sbyte b = templateId;
			int displayOrder;
			if (b >= 25)
			{
				if (b <= 39)
				{
					return;
				}
				if (b <= 41)
				{
					if (b != 40)
					{
						if (b != 41)
						{
							goto IL_318;
						}
						UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", false));
						UIManager.Instance.MaskUI(UIElement.Legacy);
						goto IL_321;
					}
				}
				else if (b != 51)
				{
					if (b != 54)
					{
						goto IL_318;
					}
					string name = LocalStringManager.Get(LanguageKey.LK_Bottom_Reading);
					bool interactable = true;
					displayOrder = CS$<>8__locals1.displayOrder;
					CS$<>8__locals1.displayOrder = displayOrder + 1;
					ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(name, interactable, (EItemMenuDisplayOrder)displayOrder, delegate()
					{
						ViewWorldState.ShowReadingView(CS$<>8__locals1.<>4__this._worldStateData);
					}, null, null, false);
					btnData.SetTip(string.Empty, this._worldStateData.HasReadingTypes() ? string.Empty : LanguageKey.LK_WorldState_PracticeNotice_UnenableNotice.Tr());
					this._popupMenuBtnList.Add(btnData);
					string name2 = LocalStringManager.Get(LanguageKey.LK_Bottom_Looping);
					bool interactable2 = true;
					displayOrder = CS$<>8__locals1.displayOrder;
					CS$<>8__locals1.displayOrder = displayOrder + 1;
					btnData = new ViewPopupMenu.BtnData(name2, interactable2, (EItemMenuDisplayOrder)displayOrder, delegate()
					{
						ViewWorldState.PrepareLoopingView(CS$<>8__locals1.<>4__this._worldStateData);
						TaiwuDomainMethod.AsyncCall.GetLoopingViewDisplayData(CS$<>8__locals1.<>4__this, delegate(int offset, RawDataPool dataPool)
						{
							LoopingViewDisplayData displayData = null;
							Serializer.Deserialize(dataPool, offset, ref displayData);
							UIElement.Looping.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("LoopingViewDisplayData", displayData));
							UIManager.Instance.ShowUI(UIElement.Looping, true);
						});
					}, null, null, false);
					btnData.SetTip(string.Empty, this._worldStateData.HasLoopingTypes() ? string.Empty : LanguageKey.LK_WorldState_PracticeNotice_UnenableNotice.Tr());
					this._popupMenuBtnList.Add(btnData);
					goto IL_321;
				}
			}
			else
			{
				switch (b)
				{
				case 10:
					ViewWorldState.<OnJumpButtonClicked>g__ShowCharacterMenuSubPage|109_0(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None, -1);
					return;
				case 11:
				{
					List<ViewPopupMenu.BtnData> popupMenuBtnList = this._popupMenuBtnList;
					string name3 = LocalStringManager.Get(LanguageKey.LK_Inventory);
					bool interactable3 = true;
					displayOrder = CS$<>8__locals1.displayOrder;
					CS$<>8__locals1.displayOrder = displayOrder + 1;
					popupMenuBtnList.Add(new ViewPopupMenu.BtnData(name3, interactable3, (EItemMenuDisplayOrder)displayOrder, delegate()
					{
						ViewWorldState.<OnJumpButtonClicked>g__ShowCharacterMenuSubPage|109_0(ECharacterSubToggleBase.ItemBase, ECharacterSubPage.None, -1);
					}, null, null, false));
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowWarehouseBtn|1();
					goto IL_321;
				}
				case 12:
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowWarehouseBtn|1();
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowBuildingAreaBtn|2();
					goto IL_321;
				case 13:
				{
					List<ViewPopupMenu.BtnData> popupMenuBtnList2 = this._popupMenuBtnList;
					string name4 = LocalStringManager.Get(LanguageKey.LK_Resource_Choosy);
					bool interactable4 = true;
					displayOrder = CS$<>8__locals1.displayOrder;
					CS$<>8__locals1.displayOrder = displayOrder + 1;
					popupMenuBtnList2.Add(new ViewPopupMenu.BtnData(name4, interactable4, (EItemMenuDisplayOrder)displayOrder, delegate()
					{
						ResourceInts targetResources = default(ResourceInts);
						targetResources.Initialize();
						BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
						BasicGameData baseGameData = SingletonObject.getInstance<BasicGameData>();
						for (sbyte i = 0; i < 6; i += 1)
						{
							int target = Math.Max(buildingModel.GetResourceCount(i) - baseGameData.MaterialResourceMaxCount, GlobalConfig.Instance.ChoosyResourceBaseCost);
							targetResources.Set((int)i, target);
						}
						ArgumentBox args = EasyPool.Get<ArgumentBox>();
						args.SetObject("targetResources", targetResources);
						UIElement.ChoosyResource.SetOnInitArgs(args);
						UIManager.Instance.MaskUI(UIElement.ChoosyResource);
					}, null, null, false));
					CS$<>8__locals1.<OnJumpButtonClicked>g__ShowBuildingAreaBtn|2();
					goto IL_321;
				}
				case 14:
				case 15:
				case 16:
				case 17:
					break;
				default:
					goto IL_318;
				}
			}
			List<ViewPopupMenu.BtnData> popupMenuBtnList3 = this._popupMenuBtnList;
			string name5 = LocalStringManager.Get(LanguageKey.LK_Heal_Entry);
			bool interactable5 = true;
			displayOrder = CS$<>8__locals1.displayOrder;
			CS$<>8__locals1.displayOrder = displayOrder + 1;
			popupMenuBtnList3.Add(new ViewPopupMenu.BtnData(name5, interactable5, (EItemMenuDisplayOrder)displayOrder, delegate()
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
				List<int> teamCharList = monitor.GetTaiwuTeamCharIds();
				teamCharList.AddRange(monitor.GetTaiwuSpecialGroup());
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				args.SetObject("DoctorList", teamCharList);
				List<int> patientList = new List<int>();
				patientList.AddRange(teamCharList);
				args.SetObject("PatientList", patientList);
				args.Set("NeedPay", false);
				args.Set("CurrentCharacterId", taiwuCharId);
				CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, taiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					KidnappedCharacterList kidnappedCharacterList = null;
					Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
					bool flag2 = kidnappedCharacterList != null;
					if (flag2)
					{
						for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
						{
							patientList.Add(kidnappedCharacterList.Get(i).CharId);
						}
					}
					UIElement.Heal.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.Heal, true);
				});
			}, null, null, false));
			CS$<>8__locals1.<OnJumpButtonClicked>g__ShowUsingMedicineBtn|3(CS$<>8__locals1.worldStateItem.TemplateId);
			goto IL_321;
			IL_318:
			this.ShowWorldStatePanel();
			return;
			IL_321:
			bool flag = CS$<>8__locals1.pendingAsyncCalls == 0;
			if (flag)
			{
				CS$<>8__locals1.<OnJumpButtonClicked>g__ShowPopupMenu|4();
			}
		}

		// Token: 0x0600A13A RID: 41274 RVA: 0x004B5840 File Offset: 0x004B3A40
		private void RefreshGuidings()
		{
			bool flag = !this._overWeight && (this._worldStateData.GetWorldState(11) || this._worldStateData.GetWorldState(11));
			if (flag)
			{
				this._overWeight = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(234);
				GlobalDomainMethod.Call.InvokeGuidingTrigger(242);
			}
			bool flag2 = !this._sectStory && Enumerable.Range(25, 15).Any((int x) => this._worldStateData.GetWorldState((short)x));
			if (flag2)
			{
				this._sectStory = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(60);
			}
			bool flag3 = !this._equipOverload && this._worldStateData.GetWorldState(10);
			if (flag3)
			{
				this._equipOverload = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(224);
			}
		}

		// Token: 0x0600A13B RID: 41275 RVA: 0x004B5908 File Offset: 0x004B3B08
		public static string GetWorldStateJumpNotice(WorldStateItem worldStateItem)
		{
			return string.Empty;
		}

		// Token: 0x0600A13C RID: 41276 RVA: 0x004B5920 File Offset: 0x004B3B20
		public static bool TriggerWorldStateJumpInteract(WorldStateItem worldStateItem, Vector3 position)
		{
			ViewWorldState worldState = UIElement.WorldState.UiBaseAs<ViewWorldState>();
			bool flag = worldState == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte templateId = worldStateItem.TemplateId;
				sbyte b = templateId;
				if (b != 10 && b - 25 > 14)
				{
					bool flag2 = worldStateItem.TemplateId == 11 || worldStateItem.TemplateId == 13 || worldStateItem.TemplateId == 12 || worldStateItem.TemplateId == 14 || worldStateItem.TemplateId == 15 || worldStateItem.TemplateId == 16 || worldStateItem.TemplateId == 17 || worldStateItem.TemplateId == 40 || worldStateItem.TemplateId == 51 || worldStateItem.TemplateId == 54;
					if (flag2)
					{
						worldState.OnJumpButtonClicked(worldStateItem, position);
						result = true;
					}
					else
					{
						worldState.OnJumpButtonClicked(worldStateItem, position);
						result = false;
					}
				}
				else
				{
					worldState.OnJumpButtonClicked(worldStateItem, position);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600A13D RID: 41277 RVA: 0x004B5A01 File Offset: 0x004B3C01
		private void CancelPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x0600A13E RID: 41278 RVA: 0x004B5A11 File Offset: 0x004B3C11
		private void StartPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600A13F RID: 41279 RVA: 0x004B5A24 File Offset: 0x004B3C24
		public static void ShowReadingView(WorldStateData data)
		{
			byte readingIndex = (byte)data.IterateReadingTypes().FirstOrDefault<WorldStateData.EReadingType>();
			bool flag = readingIndex == 4;
			if (flag)
			{
				UIManager.Instance.ShowUI(UIElement.ReadingEvent, true);
			}
			else
			{
				bool flag2 = readingIndex == 0 || readingIndex == 2;
				if (flag2)
				{
					UIElement reading = UIElement.Reading;
					reading.OnShowed = (Action)Delegate.Combine(reading.OnShowed, new Action(delegate()
					{
						UIElement.Reading.UiBaseAs<ViewReading>().ShowSelectBookPanel(26, 0, 2);
					}));
					UIManager.Instance.ShowUI(UIElement.Reading, true);
				}
				else
				{
					bool flag3 = readingIndex == 3;
					if (flag3)
					{
						UIElement reading2 = UIElement.Reading;
						reading2.OnShowed = (Action)Delegate.Combine(reading2.OnShowed, new Action(delegate()
						{
							UIElement.Reading.UiBaseAs<ViewReading>().ShowSelectReferenceBookPanel();
						}));
						UIManager.Instance.ShowUI(UIElement.Reading, true);
					}
					else
					{
						UIManager.Instance.ShowUI(UIElement.Reading, true);
					}
				}
			}
		}

		// Token: 0x0600A140 RID: 41280 RVA: 0x004B5B28 File Offset: 0x004B3D28
		public static void PrepareLoopingView(WorldStateData data)
		{
			byte loopingIndex = (byte)data.IterateLoopingTypes().FirstOrDefault<WorldStateData.ELoopingType>();
			bool flag = loopingIndex == 0;
			if (flag)
			{
				ViewLooping.JumpToStrategy();
			}
			else
			{
				bool flag2 = loopingIndex == 1 || loopingIndex == 3;
				if (flag2)
				{
					ViewLooping.JumpToIncompleteLoopingSkill();
				}
				else
				{
					bool flag3 = loopingIndex == 2;
					if (flag3)
					{
						ViewLooping.JumpToGetExtraNeiliSkill();
					}
					else
					{
						bool flag4 = loopingIndex == 4;
						if (flag4)
						{
							ViewLooping.JumpToSelectReferenceSkill(null);
						}
						else
						{
							bool flag5 = loopingIndex == 5;
							if (flag5)
							{
								ViewLooping.JumpToFiveElementTransferSkill();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A141 RID: 41281 RVA: 0x004B5BA8 File Offset: 0x004B3DA8
		private void RefreshPastTaiwuVillage()
		{
			bool atPastTaiwuVillage = SingletonObject.getInstance<WorldMapModel>().AtPastTaiwuVillage;
			this.stateIconHolder.gameObject.SetActive(!atPastTaiwuVillage);
			this.levelHolder.SetActive(!atPastTaiwuVillage);
			this.timeBackPointerTrigger.enabled = !atPastTaiwuVillage;
			this.timeBack.enabled = !atPastTaiwuVillage;
			bool flag = atPastTaiwuVillage;
			if (flag)
			{
				this.UpdateTimeUnknown();
			}
		}

		// Token: 0x0600A142 RID: 41282 RVA: 0x004B5C14 File Offset: 0x004B3E14
		public static bool IsMedicineEffectTypeMatchWorldState(sbyte worldStateItemTemplateId, EMedicineEffectType medicineEffectType)
		{
			if (!true)
			{
			}
			bool result;
			switch (worldStateItemTemplateId)
			{
			case 14:
				result = (medicineEffectType == EMedicineEffectType.RecoverOuterInjury || medicineEffectType == EMedicineEffectType.RecoverHealth);
				break;
			case 15:
				result = (medicineEffectType == EMedicineEffectType.RecoverInnerInjury || medicineEffectType == EMedicineEffectType.RecoverHealth);
				break;
			case 16:
				result = (medicineEffectType == EMedicineEffectType.ChangeDisorderOfQi || medicineEffectType == EMedicineEffectType.RecoverHealth);
				break;
			case 17:
				result = (medicineEffectType == EMedicineEffectType.ApplyPoison || medicineEffectType == EMedicineEffectType.RecoverHealth);
				break;
			default:
				if (worldStateItemTemplateId != 40)
				{
					result = (worldStateItemTemplateId == 51 && (medicineEffectType == EMedicineEffectType.RecoverOuterInjury || medicineEffectType == EMedicineEffectType.RecoverInnerInjury || medicineEffectType == EMedicineEffectType.RecoverHealth || medicineEffectType == EMedicineEffectType.ChangeDisorderOfQi || medicineEffectType == EMedicineEffectType.ApplyPoison));
				}
				else
				{
					result = (medicineEffectType == EMedicineEffectType.RecoverOuterInjury || medicineEffectType == EMedicineEffectType.RecoverInnerInjury || medicineEffectType == EMedicineEffectType.RecoverHealth || medicineEffectType == EMedicineEffectType.ChangeDisorderOfQi || medicineEffectType == EMedicineEffectType.ApplyPoison);
				}
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600A145 RID: 41285 RVA: 0x004B5EC8 File Offset: 0x004B40C8
		[CompilerGenerated]
		internal static void <OnJumpButtonClicked>g__ShowCharacterMenuSubPage|109_0(ECharacterSubToggleBase subToggle = ECharacterSubToggleBase.None, ECharacterSubPage subPage = ECharacterSubPage.None, int characterId = -1)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", (characterId == -1) ? SingletonObject.getInstance<BasicGameData>().TaiwuCharId : characterId);
			argBox.Set("CanOperate", true);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(subToggle, subPage));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x04007CDC RID: 31964
		[Header("UI显隐动画组件")]
		[SerializeField]
		private UIAnim uiAnim;

		// Token: 0x04007CDD RID: 31965
		[Header("状态显示")]
		[SerializeField]
		private RectTransform stateIconHolder;

		// Token: 0x04007CDE RID: 31966
		[Header("显示模式切换")]
		[SerializeField]
		private CButton shortDisplayModeBtn;

		// Token: 0x04007CDF RID: 31967
		[SerializeField]
		private CButton fullDisplayModeBtn;

		// Token: 0x04007CE0 RID: 31968
		[Header("时间显示")]
		[SerializeField]
		private CButton timeBack;

		// Token: 0x04007CE1 RID: 31969
		[SerializeField]
		private PointerTrigger timeBackPointerTrigger;

		// Token: 0x04007CE2 RID: 31970
		[SerializeField]
		private TextMeshProUGUI time;

		// Token: 0x04007CE3 RID: 31971
		[SerializeField]
		private RectTransform timeHoverRect;

		// Token: 0x04007CE4 RID: 31972
		[SerializeField]
		private Vector2 timeHoverSizeOffset = new Vector2(54f, 0f);

		// Token: 0x04007CE5 RID: 31973
		[SerializeField]
		private TextMeshProUGUI solarTermName;

		// Token: 0x04007CE6 RID: 31974
		[Header("相枢等级")]
		[SerializeField]
		private CImage xiangshuLevelBar;

		// Token: 0x04007CE7 RID: 31975
		[SerializeField]
		private RectTransform levelIconHolder;

		// Token: 0x04007CE8 RID: 31976
		[SerializeField]
		private RectTransform levelTipsHolder;

		// Token: 0x04007CE9 RID: 31977
		[SerializeField]
		private GameObject xiangshuParticle;

		// Token: 0x04007CEA RID: 31978
		[SerializeField]
		private GameObject levelHolder;

		// Token: 0x04007CEB RID: 31979
		[Header("危险提示")]
		[SerializeField]
		private GameObject dangerHint;

		// Token: 0x04007CEC RID: 31980
		[SerializeField]
		private TextMeshProUGUI dangerHintText;

		// Token: 0x04007CED RID: 31981
		[Header("模板")]
		[SerializeField]
		private GameObject worldStateTemplate;

		// Token: 0x04007CEE RID: 31982
		private int _taiwuCharId;

		// Token: 0x04007CEF RID: 31983
		private int _moveTimeCostPercent;

		// Token: 0x04007CF0 RID: 31984
		private List<IntPair> _overweightSanctionPercent;

		// Token: 0x04007CF1 RID: 31985
		private WorldStateData _worldStateData;

		// Token: 0x04007CF2 RID: 31986
		private const string WorldStatePath = "WorldStateTemplate_Path";

		// Token: 0x04007CF3 RID: 31987
		private const string WorldStateNamePre = "WorldState_";

		// Token: 0x04007CF4 RID: 31988
		private List<sbyte> _curSectTask = new List<sbyte>();

		// Token: 0x04007CF5 RID: 31989
		private Dictionary<short, ushort> _loongDebuff = new Dictionary<short, ushort>();

		// Token: 0x04007CF6 RID: 31990
		private sbyte _xiangshuProgress;

		// Token: 0x04007CF7 RID: 31991
		private List<int> _severelyInjuredCharIds = new List<int>();

		// Token: 0x04007CF8 RID: 31992
		private List<int> _dyingCharIds = new List<int>();

		// Token: 0x04007CF9 RID: 31993
		private Dictionary<string, ViewWorldState.StateActiveData> _worldStateObjects = new Dictionary<string, ViewWorldState.StateActiveData>();

		// Token: 0x04007CFA RID: 31994
		private List<ViewWorldState.StateActiveData> _generalStateObjects = new List<ViewWorldState.StateActiveData>();

		// Token: 0x04007CFB RID: 31995
		private List<ViewWorldState.StateActiveData> _sectTaskStateObjects = new List<ViewWorldState.StateActiveData>();

		// Token: 0x04007CFC RID: 31996
		private List<ViewWorldState.StateActiveData> _xiangshuLevelStateObjects = new List<ViewWorldState.StateActiveData>();

		// Token: 0x04007CFD RID: 31997
		private List<ViewWorldState.StateActiveData> _xiangshuAwakeStateObjects = new List<ViewWorldState.StateActiveData>();

		// Token: 0x04007CFE RID: 31998
		private List<ViewWorldState.StateActiveData> _xiangshuAttackStateObjects = new List<ViewWorldState.StateActiveData>();

		// Token: 0x04007CFF RID: 31999
		private bool _isShowAllStates = false;

		// Token: 0x04007D00 RID: 32000
		private const int MaxDisplayCount = 8;

		// Token: 0x04007D01 RID: 32001
		private const float IconHeight = 45f;

		// Token: 0x04007D02 RID: 32002
		private const float RootHeight = 53f;

		// Token: 0x04007D03 RID: 32003
		private const float VerticalSpacing = 6f;

		// Token: 0x04007D04 RID: 32004
		private const float VerticalPadding = 8f;

		// Token: 0x04007D05 RID: 32005
		private const float IconWidth = 45f;

		// Token: 0x04007D06 RID: 32006
		private const float RootWidth = 100f;

		// Token: 0x04007D07 RID: 32007
		private const float HorizontalSpacing = 12f;

		// Token: 0x04007D08 RID: 32008
		private const float HorizontalPadding = 55f;

		// Token: 0x04007D09 RID: 32009
		private List<MartialArtTournamentPreparationInfo> _martialArtTournamentPreparationInfoList;

		// Token: 0x04007D0A RID: 32010
		private int _tournamentPreparationEndDate;

		// Token: 0x04007D0B RID: 32011
		private Dictionary<short, SettlementNameRelatedData> _settlementNameRelatedDataCache = new Dictionary<short, SettlementNameRelatedData>();

		// Token: 0x04007D0C RID: 32012
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

		// Token: 0x04007D0D RID: 32013
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

		// Token: 0x04007D0E RID: 32014
		[TupleElementNames(new string[]
		{
			"buttonName",
			"checkAction",
			"clickAction"
		})]
		private readonly List<ValueTuple<string, Action<CButton>, Action>> _dropdownButtonConfigs = new List<ValueTuple<string, Action<CButton>, Action>>();

		// Token: 0x04007D0F RID: 32015
		private List<ViewPopupMenu.BtnData> _popupMenuBtnList;

		// Token: 0x04007D10 RID: 32016
		private const int MaxXiangshuLevel = 9;

		// Token: 0x04007D11 RID: 32017
		private bool _overWeight;

		// Token: 0x04007D12 RID: 32018
		private bool _sectStory;

		// Token: 0x04007D13 RID: 32019
		private bool _equipOverload;

		// Token: 0x0200237D RID: 9085
		private class StateActiveData
		{
			// Token: 0x17001A5F RID: 6751
			// (get) Token: 0x0601039E RID: 66462 RVA: 0x0065702C File Offset: 0x0065522C
			public bool CurrentActiveState
			{
				get
				{
					return this.State.activeSelf;
				}
			}

			// Token: 0x0400DEFD RID: 57085
			public GameObject State;

			// Token: 0x0400DEFE RID: 57086
			public bool ShouldActive;
		}

		// Token: 0x0200237E RID: 9086
		private enum EnumWorldStateType
		{
			// Token: 0x0400DF00 RID: 57088
			General,
			// Token: 0x0400DF01 RID: 57089
			SectTask,
			// Token: 0x0400DF02 RID: 57090
			XiangshuLevel,
			// Token: 0x0400DF03 RID: 57091
			XiangshuAwake,
			// Token: 0x0400DF04 RID: 57092
			XiangshuAttack
		}
	}
}
