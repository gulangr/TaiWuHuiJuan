using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.CharacterLocationDisplayData;
using Game.Views.World;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Display.VillagerRoleArrangement;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Villager
{
	// Token: 0x0200073F RID: 1855
	public class ViewVillagerWork : UIBase, IRequestData, IAsyncMethodRequestHandler
	{
		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x060059CF RID: 22991 RVA: 0x0029A338 File Offset: 0x00298538
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x060059D0 RID: 22992 RVA: 0x0029A344 File Offset: 0x00298544
		private void Awake()
		{
			this.assignPanel.Parent = this;
			this.assignPanel.OnAssignVillager = new Action<int, string>(this.ShowNormalDispatchEffect);
			this.assignPanel.OnAssignTombVillager = new Action<int>(this.ShowTombDispatchEffect);
			AreaMap areaMap = this.areaMap;
			areaMap.PostRender = (Action<Area, AreaDisplayData>)Delegate.Combine(areaMap.PostRender, new Action<Area, AreaDisplayData>(delegate(Area area, AreaDisplayData displayData)
			{
				MapAreaItem areaConfig = area.Config;
				short areaId = this._mapModel.GetAreaIdByAreaTemplateId(areaConfig.TemplateId);
				MapAreaData areaData = this._mapModel.Areas[(int)areaId];
				bool stationUnlocked = areaData.StationUnlocked && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(4);
				bool isTaiwuVillageArea = areaId == this._mapModel.GetTaiwuVillageBlock().AreaId;
				short selectedArrangementId = this.SelectedArrangementTemplateId;
				Dictionary<short, int> arrangementDictOnThisArea = this.FindArrangementDictOnArea(areaId, true);
				int count;
				bool hasOccupied = this.IsAreaOccupiedByArrangement() && arrangementDictOnThisArea.TryGetValue(selectedArrangementId, out count) && count > 0;
				bool emissaryInvalid = this.SelectedRoleTemplateId == 6 && !this._validEmissaryArea.Contains((int)areaConfig.TemplateId) && (this.SelectedRoleTemplateId != 6 || !this.CheckAreaHasArrangement(areaId, 4));
				bool canDispatch = (isTaiwuVillageArea || stationUnlocked) && !displayData.IsBroken && !hasOccupied && !emissaryInvalid;
				area.SetStyle(!canDispatch, true);
				bool flag = !canDispatch && areaId == this.SelectedAreaId;
				if (flag)
				{
					this.SelectedAreaId = -1;
				}
				area.RemoveAllAreaStateItem();
				int index = 9;
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations = this._locations;
				area.InsertAreaStateItem(index, (locations != null) ? locations.Count(delegate([TupleElementNames(new string[]
				{
					"location",
					"charData"
				})] ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData> lc)
				{
					VillagerRoleCharacterDisplayData item = lc.Item2;
					if (item != null)
					{
						VillagerWorkData data = item.VillagerWorkData;
						if (data != null)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index2 = 10;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index2, (villagerCharacterDisplayDataDict != null) ? villagerCharacterDisplayDataDict.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 0)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index3 = 11;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict2 = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index3, (villagerCharacterDisplayDataDict2 != null) ? villagerCharacterDisplayDataDict2.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 1)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index4 = 12;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict3 = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index4, (villagerCharacterDisplayDataDict3 != null) ? villagerCharacterDisplayDataDict3.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 2)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index5 = 13;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict4 = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index5, (villagerCharacterDisplayDataDict4 != null) ? villagerCharacterDisplayDataDict4.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 3)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index6 = 14;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict5 = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index6, (villagerCharacterDisplayDataDict5 != null) ? villagerCharacterDisplayDataDict5.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 4)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index7 = 15;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict6 = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index7, (villagerCharacterDisplayDataDict6 != null) ? villagerCharacterDisplayDataDict6.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 5)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				int index8 = 16;
				Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict7 = this._villagerCharacterDisplayDataDict;
				area.InsertAreaStateItem(index8, (villagerCharacterDisplayDataDict7 != null) ? villagerCharacterDisplayDataDict7.Values.Count(delegate(VillagerRoleCharacterDisplayData lc)
				{
					if (lc != null && lc.RoleTemplateId == 6)
					{
						VillagerRoleArrangementDisplayDataWrapper data = lc.ArrangementDisplayData;
						if (data != null && data.ArrangementTemplateId != -1)
						{
							return data.AreaId == areaId;
						}
					}
					return false;
				}) : 0);
				TooltipInvoker tip = area.Displayer;
				bool flag2 = !canDispatch;
				if (flag2)
				{
					ViewVillagerWork.RefreshDisabledAreaTip(tip, stationUnlocked, displayData, true, emissaryInvalid);
				}
				else
				{
					tip.Type = TipType.VillagerAssign;
					TooltipInvoker displayer = area.Displayer;
					ArgumentBox argumentBox;
					if ((argumentBox = displayer.RuntimeParam) == null)
					{
						argumentBox = (displayer.RuntimeParam = EasyPool.Get<ArgumentBox>());
					}
					argumentBox.Set("farmer", this.SelectedRoleTemplateId == 0).Set("areaId", areaId).SetObject("displayData", displayData);
					tip.enabled = true;
				}
				EasyPool.Free<Dictionary<short, int>>(arrangementDictOnThisArea);
			}));
			this.charScroll.OnItemRender += delegate(int index, GameObject go)
			{
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> orderedLocations = this._orderedLocations;
				VillagerBlock block;
				bool flag;
				if (orderedLocations != null && orderedLocations.CheckIndex(index))
				{
					block = go.GetComponent<VillagerBlock>();
					flag = (block == null);
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				if (flag2)
				{
					go.SetActive(false);
				}
				else
				{
					block.gameObject.SetActive(true);
					block.Refresh(this, this._orderedLocations[index].Item1, this._orderedLocations[index].Item2);
				}
			};
			this.detailScroll.OnItemRender += delegate(int index, GameObject go)
			{
				List<int> sortedCharacterIdList = this.SortedCharacterIdList;
				VillagerWorkDetail detail;
				bool flag;
				if (sortedCharacterIdList != null && sortedCharacterIdList.CheckIndex(index))
				{
					detail = go.GetComponent<VillagerWorkDetail>();
					flag = (detail == null);
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				if (flag2)
				{
					go.SetActive(false);
				}
				else
				{
					detail.Set(this, index, this._chickenUnlocked, this.SortedCharacterIdList[index] == this.SelectedCharacterId);
				}
			};
			this.swordTombSelector.InfoLayerToggleGroup.OnActiveIndexChange += delegate(int newOne, int _)
			{
				int xiangshuId;
				bool flag = this._key2IdMap.TryGetValue(newOne, out xiangshuId);
				if (flag)
				{
					this._selectedSwordTombId = (sbyte)xiangshuId;
				}
				bool flag2 = newOne == -1;
				if (!flag2)
				{
					this.RefreshSwordTombAssignPanel();
					this.RefreshDispatchButton();
				}
			};
			this.InitSortAndFilter();
		}

		// Token: 0x060059D1 RID: 22993 RVA: 0x0029A407 File Offset: 0x00298607
		private void InitSortAndFilter()
		{
			this._locationSortAndFilterController = new CharacterLocationDisplayDataSortAndFilterControllerController(this.locationSortAndFilter, LanguageKey.LK_FindMapBlock_Type_Block);
			this._locationSortAndFilterController.Init(new Action(this.RefreshCharacterLocationFilter), "RefreshCharacterLocationFilter");
			this._detailFilterInited = false;
		}

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x060059D2 RID: 22994 RVA: 0x0029A444 File Offset: 0x00298644
		internal short SelectedRoleTemplateId
		{
			get
			{
				return ViewVillagerWork.EnabledRoleIdArray[this._selectedRoleUiIndex];
			}
		}

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x060059D3 RID: 22995 RVA: 0x0029A454 File Offset: 0x00298654
		private short SelectedArrangementTemplateId
		{
			get
			{
				short roleId = this.SelectedRoleTemplateId;
				VillagerRoleArrangementItem item2 = VillagerRoleArrangement.Instance.FirstOrDefault((VillagerRoleArrangementItem item) => !item.InvisibleInGui && item.VillagerRole == roleId);
				return (item2 != null) ? item2.TemplateId : -1;
			}
		}

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x060059D4 RID: 22996 RVA: 0x0029A49B File Offset: 0x0029869B
		private bool DispatchingGuardingSwordTomb
		{
			get
			{
				return this.SelectedRoleTemplateId == 5;
			}
		}

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x060059D5 RID: 22997 RVA: 0x0029A4A6 File Offset: 0x002986A6
		private bool SelectedLocation
		{
			get
			{
				return this.DispatchingGuardingSwordTomb ? (this._selectedSwordTombId >= 0) : (this.SelectedAreaId >= 0);
			}
		}

		// Token: 0x060059D6 RID: 22998 RVA: 0x0029A4CC File Offset: 0x002986CC
		public static bool RoleFunctionEnabled(short roleId)
		{
			BuildingBlockData buildingBlockData;
			return SingletonObject.getInstance<BuildingModel>().GetBuilding(45, out buildingBlockData) && (roleId != 5 || SingletonObject.getInstance<TaskModel>().IsTaskFinished(27));
		}

		// Token: 0x060059D7 RID: 22999 RVA: 0x0029A500 File Offset: 0x00298700
		public override void OnInit(ArgumentBox argsBox)
		{
			this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool flag = !this._inited;
			if (flag)
			{
				this.InitWorldMap();
				this.InitValidEmissaryArea();
				this.InitRoleToggles();
				this._inited = true;
			}
			this.RefreshRoleToggle();
			this.InitSwordTombItems();
			short roleTemplateId;
			bool flag2 = argsBox == null || !argsBox.Get("RoleTemplateId", out roleTemplateId);
			if (flag2)
			{
				roleTemplateId = -1;
			}
			this._selectedRoleUiIndex = ViewVillagerWork.EnabledRoleIdArray.IndexOf(roleTemplateId);
			bool flag3 = this._selectedRoleUiIndex == -1;
			if (flag3)
			{
				throw new ArgumentException(string.Format("roleTemplateId {0} is not in EnabledRoleIdArray", roleTemplateId));
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x060059D8 RID: 23000 RVA: 0x0029A5D0 File Offset: 0x002987D0
		public void RequestData()
		{
			this.RequestData(null);
		}

		// Token: 0x060059D9 RID: 23001 RVA: 0x0029A5DC File Offset: 0x002987DC
		public void RequestData(Action callback)
		{
			TaiwuDomainMethod.AsyncCall.GetTaiwuVillagerRoleDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				if (this.TaiwuVillagerRoleDisplayData == null)
				{
					this.TaiwuVillagerRoleDisplayData = new TaiwuVillagerRoleDisplayData();
				}
				Serializer.Deserialize(pool, offset, ref this.TaiwuVillagerRoleDisplayData);
				int index = this.TaiwuVillagerRoleDisplayData.VillagerRoleNpcNickNames.Length;
				while (index-- > 0)
				{
					bool flag = string.IsNullOrWhiteSpace(this.TaiwuVillagerRoleDisplayData.VillagerRoleNpcNickNames[index]);
					if (flag)
					{
						this.TaiwuVillagerRoleDisplayData.VillagerRoleNpcNickNames[index] = OrganizationMember.Instance[VillagerRole.Instance[index].OrganizationMember].GradeName;
					}
					this.toggleText[index].text = this.TaiwuVillagerRoleDisplayData.VillagerRoleNpcNickNames[index];
				}
				bool detailFilterInited = this._detailFilterInited;
				if (detailFilterInited)
				{
					this._detailSortAndFilterController.Data = this.TaiwuVillagerRoleDisplayData;
				}
				else
				{
					this._detailFilterInited = true;
					this._detailSortAndFilterController = new CharacterDetailDisplayDataSortAndFilterControllerController(this.detailSortAndFilter, LanguageKey.LK_VillagerRole_WorkStatus)
					{
						Data = this.TaiwuVillagerRoleDisplayData
					};
					this._detailSortAndFilterController.Init(new Action(this.RefreshCharacterDetailFilter), "RefreshCharacterDetailFilter");
				}
				this._swordTombList = this.TaiwuVillagerRoleDisplayData.SwordTombList;
				this._roleManageDisplayList = this.TaiwuVillagerRoleDisplayData.VillagerRoleManageDisplayData;
				this._taiwuTeam = this.TaiwuVillagerRoleDisplayData.Teammates;
				this._roleExtraEffectUnlockList = this.TaiwuVillagerRoleDisplayData.VillagerRoleExtraEffectUnlockState;
				this._chickenUnlocked = this._roleExtraEffectUnlockList.GetOrDefault((int)this.SelectedRoleTemplateId);
				this._bigEvents = this.TaiwuVillagerRoleDisplayData.BigEvents;
				this._villagerCharacterDisplayDataDict = this.TaiwuVillagerRoleDisplayData.Villagers;
				this._locations = this.TaiwuVillagerRoleDisplayData.OrderedLocations;
				this.villagerDetailSummary.ReadData(this.TaiwuVillagerRoleDisplayData);
				bool flag2 = this.roleToggles.GetActiveIndex() != this._selectedRoleUiIndex;
				if (flag2)
				{
					this.roleToggles.Set(this._selectedRoleUiIndex, false);
				}
				this.RefreshSummaryPanel();
				this.areaMap.Refresh(this.TaiwuVillagerRoleDisplayData.AreaDisplayData);
				this.RefreshCharacterLocationFilter();
				this.RefreshSwordTombs(false);
				this.OnRoleToggleChanged(!this.Element.Ready);
				TMP_Text tmp_Text = this.idleCount;
				LanguageKey languageKey = LanguageKey.LK_VillagerRole_Summary_Count_Idle;
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations2 = this._locations;
				int num;
				if (locations2 == null)
				{
					num = 0;
				}
				else
				{
					num = locations2.Where(delegate([TupleElementNames(new string[]
					{
						"location",
						"charData"
					})] ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData> x)
					{
						VillagerRoleCharacterDisplayData item = x.Item2;
						int? num5;
						if (item == null)
						{
							num5 = null;
						}
						else
						{
							VillagerWorkData villagerWorkData = item.VillagerWorkData;
							num5 = ((villagerWorkData != null) ? new sbyte?(villagerWorkData.WorkType) : null);
						}
						return num5 == 13;
					}).Count<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>>();
				}
				tmp_Text.text = languageKey.TrFormat(num);
				TMP_Text tmp_Text2 = this.keepGraveCount;
				LanguageKey languageKey2 = LanguageKey.LK_VillagerRole_Summary_Count_KeepGrave;
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations3 = this._locations;
				int num2;
				if (locations3 == null)
				{
					num2 = 0;
				}
				else
				{
					num2 = locations3.Where(delegate([TupleElementNames(new string[]
					{
						"location",
						"charData"
					})] ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData> x)
					{
						VillagerRoleCharacterDisplayData item = x.Item2;
						int? num5;
						if (item == null)
						{
							num5 = null;
						}
						else
						{
							VillagerWorkData villagerWorkData = item.VillagerWorkData;
							num5 = ((villagerWorkData != null) ? new sbyte?(villagerWorkData.WorkType) : null);
						}
						return num5 == 12;
					}).Count<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>>();
				}
				tmp_Text2.text = languageKey2.TrFormat(num2);
				TMP_Text tmp_Text3 = this.unassignedCount;
				LanguageKey languageKey3 = LanguageKey.LK_VillagerRole_Summary_Count_Unassigned;
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations4 = this._locations;
				int num3;
				if (locations4 == null)
				{
					num3 = 0;
				}
				else
				{
					num3 = locations4.Where(delegate([TupleElementNames(new string[]
					{
						"location",
						"charData"
					})] ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData> x)
					{
						VillagerRoleCharacterDisplayData item = x.Item2;
						return item != null && item.Age >= 16 && item.VillagerWorkData == null;
					}).Count<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>>();
				}
				tmp_Text3.text = languageKey3.TrFormat(num3);
				TMP_Text tmp_Text4 = this.blockCount;
				LanguageKey languageKey4 = LanguageKey.LK_VillagerRole_Summary_Count_Block;
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations5 = this._locations;
				tmp_Text4.text = languageKey4.TrFormat((locations5 != null) ? locations5.Count : 0);
				TMP_Text tmp_Text5 = this.ruinedCount;
				LanguageKey languageKey5 = LanguageKey.LK_VillagerRole_Summary_Count_Destroyed;
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations6 = this._locations;
				int num4;
				if (locations6 == null)
				{
					num4 = 0;
				}
				else
				{
					num4 = locations6.Where(delegate([TupleElementNames(new string[]
					{
						"location",
						"charData"
					})] ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData> x)
					{
						CharacterLocationDisplayData item = x.Item1;
						bool? flag4;
						if (item == null)
						{
							flag4 = null;
						}
						else
						{
							MapBlockData blockData = item.BlockData;
							flag4 = ((blockData != null) ? new bool?(blockData.Destroyed) : null);
						}
						bool? flag5 = flag4;
						return flag5.GetValueOrDefault();
					}).Count<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>>();
				}
				tmp_Text5.text = languageKey5.TrFormat(num4);
				List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> locations7 = this._locations;
				ValueTuple<EMapBlockSubType, int>[] array;
				if (locations7 == null)
				{
					array = null;
				}
				else
				{
					array = (from x in locations7.GroupBy(delegate([TupleElementNames(new string[]
					{
						"location",
						"charData"
					})] ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData> x)
					{
						CharacterLocationDisplayData item = x.Item1;
						EMapBlockSubType? emapBlockSubType;
						if (item == null)
						{
							emapBlockSubType = null;
						}
						else
						{
							MapBlockData blockData = item.BlockData;
							if (blockData == null)
							{
								emapBlockSubType = null;
							}
							else
							{
								MapBlockItem config = blockData.GetConfig();
								emapBlockSubType = ((config != null) ? new EMapBlockSubType?(config.SubType) : null);
							}
						}
						return emapBlockSubType ?? EMapBlockSubType.Invalid;
					})
					select new ValueTuple<EMapBlockSubType, int>(x.Key, x.Count<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>>()) into x
					where x.Item1 != EMapBlockSubType.Invalid
					orderby x.Item2 descending, x.Item1
					select x).ToArray<ValueTuple<EMapBlockSubType, int>>();
				}
				ValueTuple<EMapBlockSubType, int>[] locations = array ?? Array.Empty<ValueTuple<EMapBlockSubType, int>>();
				int i;
				for (i = 0; i < this.extraLocations.Count; i++)
				{
					bool flag3 = locations.CheckIndex(i);
					if (flag3)
					{
						this.extraLocations[i].gameObject.SetActive(true);
						this.extraLocations[i].text = LanguageKey.LK_VillagerRole_Summary_Count.TrFormat(GameData.Domains.Map.SharedMethods.MapBlockSubTypeName(locations[i].Item1), locations[i].Item2);
					}
					else
					{
						this.extraLocations[i].gameObject.SetActive(false);
					}
				}
				while (i < locations.Length)
				{
					TMP_Text text = Object.Instantiate<TMP_Text>(this.extraLocations[0], this.extraLocations[0].transform.parent, false);
					text.text = LanguageKey.LK_VillagerRole_Summary_Count.TrFormat(GameData.Domains.Map.SharedMethods.MapBlockSubTypeName(locations[i].Item1), locations[i].Item2);
					this.extraLocations.Add(text);
					i++;
				}
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x060059DA RID: 23002 RVA: 0x0029A614 File Offset: 0x00298814
		private void InitValidEmissaryArea()
		{
			this._validEmissaryArea = new HashSet<int>();
			foreach (MapStateItem item in ((IEnumerable<MapStateItem>)MapState.Instance))
			{
				this._validEmissaryArea.Add((int)item.MainAreaID);
				this._validEmissaryArea.Add((int)item.SectAreaID);
			}
		}

		// Token: 0x060059DB RID: 23003 RVA: 0x0029A690 File Offset: 0x00298890
		private void InitSwordTombItems()
		{
			RectTransform root = this.swordTombSelector.GetComponent<RectTransform>();
			RectTransform infoLayer = this.swordTombSelector.InfoLayer;
			for (int i = infoLayer.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(infoLayer.GetChild(i).gameObject);
			}
			SwordTombPiece swordTombTemplate = root.Find("SwordTombPiece").GetComponent<SwordTombPiece>();
			swordTombTemplate.gameObject.SetActive(false);
			short areaId = this._mapModel.GetTaiwuVillageAreaId();
			byte areaSize = this._mapModel.GetAreaSize(areaId);
			this._xiangshuId2BlockId.Clear();
			MapDomainMethod.AsyncCall.GetMapBlockDataListOptional(null, (from blockId in Enumerable.Range(0, (int)(areaSize * areaSize))
			select new Location(areaId, (short)blockId)).ToList<Location>(), false, false, delegate(int offset, RawDataPool pool)
			{
				List<MapBlockData> blockList = new List<MapBlockData>();
				Serializer.Deserialize(pool, offset, ref blockList);
				this.swordTombSelector.RebuildMap(areaSize, blockList, new Action<MapBlockData, Graphic>(base.<InitSwordTombItems>g__RefreshBlockPieceGraphic|2));
				using (IEnumerator<MapBlockData> enumerator = blockList.Where(delegate(MapBlockData blockData)
				{
					MapBlockItem config = blockData.GetConfig();
					return config != null && config.SubType == EMapBlockSubType.SwordTomb;
				}).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MapBlockData blockData = enumerator.Current;
						IEnumerable<KeyValuePair<sbyte, short>> xiangshuId2BlockConfigId = GameData.Domains.Map.SharedConstValue.XiangshuId2BlockConfigId;
						Func<KeyValuePair<sbyte, short>, bool> predicate;
						Func<KeyValuePair<sbyte, short>, bool> <>9__4;
						if ((predicate = <>9__4) == null)
						{
							predicate = (<>9__4 = ((KeyValuePair<sbyte, short> pair) => pair.Value == blockData.TemplateId));
						}
						using (IEnumerator<KeyValuePair<sbyte, short>> enumerator2 = xiangshuId2BlockConfigId.Where(predicate).GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								KeyValuePair<sbyte, short> pair2 = enumerator2.Current;
								Dictionary<sbyte, short> xiangshuId2BlockId = this._xiangshuId2BlockId;
								sbyte key = pair2.Key;
								short value = this._xiangshuId2BlockId[~pair2.Key] = blockData.BlockId;
								xiangshuId2BlockId[key] = value;
							}
						}
					}
				}
				BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
				int xiangshuId = 0;
				while (xiangshuId < SwordTomb.Instance.Count)
				{
					short value;
					bool flag = !this._xiangshuId2BlockId.TryGetValue((sbyte)xiangshuId, out value);
					if (!flag)
					{
						goto IL_203;
					}
					int xiangshuIndex = -1;
					for (int index = 0; index < this._mapModel.SwordTombLocations.Length; index++)
					{
						bool flag2 = (int)basicGameData.GetXiangshuAvatarTaskInOrderIndex(index) != xiangshuId;
						if (!flag2)
						{
							xiangshuIndex = index;
							break;
						}
					}
					bool flag3 = !this._mapModel.SwordTombLocations.CheckIndex(xiangshuIndex);
					if (!flag3)
					{
						this._xiangshuId2BlockId[(sbyte)xiangshuId] = this._mapModel.SwordTombLocations[xiangshuIndex].BlockId;
						goto IL_203;
					}
					IL_257:
					xiangshuId++;
					continue;
					IL_203:
					SwordTombPiece swordTomb = (xiangshuId >= infoLayer.childCount) ? Object.Instantiate<SwordTombPiece>(swordTombTemplate, infoLayer) : infoLayer.GetChild(xiangshuId).GetComponent<SwordTombPiece>();
					swordTomb.name = xiangshuId.ToString();
					swordTomb.gameObject.SetActive(true);
					goto IL_257;
				}
				this._swordTombMapLookAtBlock = new Action<short>(this.swordTombSelector.LookAtBlock);
				CToggleGroup infoGroup = this.swordTombSelector.InfoLayerToggleGroup;
				infoGroup.Clear();
				this._key2IdMap.Clear();
				foreach (object obj in infoLayer)
				{
					RectTransform swordTombRect = (RectTransform)obj;
					int xiangshuId2;
					short blockId;
					bool flag4 = int.TryParse(swordTombRect.name, out xiangshuId2) && this._xiangshuId2BlockId.TryGetValue((sbyte)xiangshuId2, out blockId);
					if (flag4)
					{
						RectTransform blockPiece = this.swordTombSelector.FindBlock(blockId);
						swordTombRect.localPosition = blockPiece.localPosition;
						bool isSwordTomb = this._xiangshuId2BlockId.ContainsKey((sbyte)(~(sbyte)xiangshuId2));
						bool flag5 = isSwordTomb;
						if (flag5)
						{
							CToggle toggle = swordTombRect.GetComponentInChildren<CToggle>();
							bool flag6 = !toggle;
							if (flag6)
							{
								continue;
							}
							toggle.isOn = true;
							toggle.isOn = false;
							this._key2IdMap[infoGroup.Count()] = xiangshuId2;
							infoGroup.Add(toggle);
						}
						else
						{
							RectTransform block = this.swordTombSelector.FindBlock(blockId + 1);
							swordTombRect.localPosition = block.localPosition;
						}
						SwordTombItem cfg = SwordTomb.Instance[xiangshuId2];
						CharacterItem charCfg = Character.Instance[cfg.XiangshuAvatarBegin];
						swordTombRect.GetComponent<SwordTombPiece>().Set(charCfg, isSwordTomb);
					}
				}
				this._selectedSwordTombId = -1;
				infoGroup.Init(-1);
				this.RefreshSwordTombs(true);
			});
		}

		// Token: 0x060059DC RID: 23004 RVA: 0x0029A79C File Offset: 0x0029899C
		private void RefreshSwordTombAssignPanel()
		{
			DispatchSwordTombDisplayData dispatchSwordTombDisplayData = this._swordTombList.Find((DispatchSwordTombDisplayData displayData) => displayData.Id == this._selectedSwordTombId);
			short blockId = (dispatchSwordTombDisplayData != null) ? dispatchSwordTombDisplayData.Location.BlockId : -1;
			this.SelectSwordTomb(blockId);
		}

		// Token: 0x060059DD RID: 23005 RVA: 0x0029A7DB File Offset: 0x002989DB
		internal void SelectSwordTombByTombId(sbyte swordTombId)
		{
			this._selectedSwordTombId = swordTombId;
			this.RefreshSwordTombAssignPanel();
		}

		// Token: 0x060059DE RID: 23006 RVA: 0x0029A7EC File Offset: 0x002989EC
		internal void SelectSwordTomb(short blockId)
		{
			bool flag = blockId == -1;
			if (flag)
			{
				this.swordTombSelector.InfoLayerToggleGroup.DeSelect(false);
			}
			else
			{
				this.assignPanel.SelectArea(this.swordTombSelector.FindBlock(blockId), (int)blockId);
				this.assignPanel.RefreshPanel();
				this.swordTombSelector.scale.OnPointerExit();
				this.assignPanel.DeactiveAction = delegate()
				{
					this.swordTombSelector.InfoLayerToggleGroup.DeSelect(false);
				};
			}
		}

		// Token: 0x060059DF RID: 23007 RVA: 0x0029A864 File Offset: 0x00298A64
		private void RefreshCharacterLocationFilter()
		{
			Func<CharacterLocationDisplayData, bool> filter = this._locationSortAndFilterController.GenerateFilter();
			this._orderedLocations = (from x in this._locations
			where filter(x.Item1)
			select x).ToList<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>>();
			this._locationSortAndFilterController.AfterFilter((from x in this._locations
			select x.Item1).ToList<CharacterLocationDisplayData>());
			this.charScroll.SetDataCount(this._orderedLocations.Count);
			this.charScroll.ReRender();
		}

		// Token: 0x060059E0 RID: 23008 RVA: 0x0029A90C File Offset: 0x00298B0C
		private void RefreshCharacterDetailFilter()
		{
			Func<int, bool> filter = this._detailSortAndFilterController.GenerateFilter();
			this.SortedCharacterIdList.Clear();
			this.SortedCharacterIdList.AddRange(this._characterIdList.Where(filter));
			Comparison<int> sorter = this._detailSortAndFilterController.GenerateComparer(this.SortedCharacterIdList);
			this.SortedCharacterIdList.Sort(sorter);
			this._detailSortAndFilterController.AfterFilter(this._characterIdList);
			this.detailScroll.SetDataCount(this.SortedCharacterIdList.Count);
			this.detailScroll.ReRender();
			bool flag = this.SortedCharacterIdList.Count == 0;
			if (flag)
			{
				this.assignPanel.gameObject.SetActive(false);
			}
		}

		// Token: 0x060059E1 RID: 23009 RVA: 0x0029A9C4 File Offset: 0x00298BC4
		private void SortAndRefreshCharacterList(bool autoSelect = false)
		{
			this.SortedCharacterIdList.Clear();
			this._characterIdList.Clear();
			List<VillagerRoleManageDisplayData> roleManageDisplayList = this._roleManageDisplayList;
			bool flag = roleManageDisplayList[((this.SelectedRoleTemplateId < 0) ? new Index(1, true) : ((int)this.SelectedRoleTemplateId)).GetOffset(roleManageDisplayList.Count)].CharacterIds == null;
			if (flag)
			{
				this.assignPanel.Character = new ValueTuple<int, int>(this.SelectedCharacterId = -1, -1);
			}
			else
			{
				List<int> characterIdList = this._characterIdList;
				roleManageDisplayList = this._roleManageDisplayList;
				characterIdList.AddRange(from id in roleManageDisplayList[((this.SelectedRoleTemplateId < 0) ? new Index(1, true) : ((int)this.SelectedRoleTemplateId)).GetOffset(roleManageDisplayList.Count)].CharacterIds
				where this._villagerCharacterDisplayDataDict.ContainsKey(id) && !this._taiwuTeam.Contains(id)
				select id);
				this.RefreshCharacterDetailFilter();
				if (autoSelect)
				{
					this.SelectedCharacterId = -1;
				}
				bool flag2 = autoSelect && this.SortedCharacterIdList.Count > 0 && this.SelectedCharacterId == -1;
				if (flag2)
				{
					foreach (int charId in this.SortedCharacterIdList)
					{
						VillagerRoleCharacterDisplayData displayData = this._villagerCharacterDisplayDataDict[charId];
						bool flag3 = displayData.ArrangementDisplayData.ArrangementTemplateId == -1;
						if (flag3)
						{
							this.assignPanel.Character = new ValueTuple<int, int>(this.SelectedCharacterId = charId, (int)this.SelectedRoleTemplateId);
							break;
						}
					}
					bool flag4 = this.SelectedCharacterId == -1 && this.SortedCharacterIdList.Count > 0;
					if (flag4)
					{
						this.assignPanel.Character = new ValueTuple<int, int>(this.SelectedCharacterId = this.SortedCharacterIdList[0], (int)this.SelectedRoleTemplateId);
					}
				}
				bool flag5 = this.SelectedCharacterId == -1;
				if (flag5)
				{
					this.assignPanel.Character = new ValueTuple<int, int>(this.SelectedCharacterId = -1, -1);
				}
			}
			this.detailScroll.SetDataCount(this.SortedCharacterIdList.Count);
			this.detailScroll.ReRender();
			this.RefreshSwordTombs(false);
			this.RefreshDispatchButton();
		}

		// Token: 0x060059E2 RID: 23010 RVA: 0x0029AC24 File Offset: 0x00298E24
		private void OnEnable()
		{
			this.assignPanel.gameObject.SetActive(false);
			GEvent.Add(UiEvents.RefreshVillagerRoleDispatch, new GEvent.Callback(this.OnRefreshVillagerRoleDispatch));
		}

		// Token: 0x060059E3 RID: 23011 RVA: 0x0029AC55 File Offset: 0x00298E55
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.RefreshVillagerRoleDispatch, new GEvent.Callback(this.OnRefreshVillagerRoleDispatch));
			this.SelectedCharacterId = -1;
			this.SelectedAreaId = -1;
		}

		// Token: 0x060059E4 RID: 23012 RVA: 0x0029AC82 File Offset: 0x00298E82
		private void OnRefreshVillagerRoleDispatch(ArgumentBox argsBox)
		{
			argsBox.Get<List<VillagerRoleManageDisplayData>>("RoleManageDisplayList", out this._roleManageDisplayList);
			this.TaiwuVillagerRoleDisplayData.VillagerRoleManageDisplayData = this._roleManageDisplayList;
			this.RefreshAllArea(false);
			this.SortAndRefreshCharacterList(false);
		}

		// Token: 0x060059E5 RID: 23013 RVA: 0x0029ACB8 File Offset: 0x00298EB8
		private void InitWorldMap()
		{
			this.areaMap.OnSelectArea = new Action<short>(this.OnClickArea);
			this.scaleAndMoveRoot.OnScale = new Action<Vector3>(this.OnScale);
			this.areaMap.Init(false, this, false);
		}

		// Token: 0x060059E6 RID: 23014 RVA: 0x0029ACF8 File Offset: 0x00298EF8
		private void InitRoleToggles()
		{
			this.roleToggles.Init(-1);
			this.roleToggles.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				bool flag = (ViewVillagerWork.EnabledRoleIdArray.CheckIndex(newTog) && ViewVillagerWork.EnabledRoleIdArray[newTog] == 5) || (ViewVillagerWork.EnabledRoleIdArray.CheckIndex(oldTog) && ViewVillagerWork.EnabledRoleIdArray[oldTog] == 5);
				if (flag)
				{
					this.assignPanel.Hide();
				}
				this.SelectedCharacterId = -1;
				bool flag2 = newTog != -1;
				if (flag2)
				{
					this.villagerSummary.gameObject.SetActive(newTog == 0);
					this.villagerDetail.gameObject.SetActive(newTog != 0);
					this._selectedRoleUiIndex = newTog;
					this.OnRoleToggleChanged(true);
				}
			};
		}

		// Token: 0x060059E7 RID: 23015 RVA: 0x0029AD20 File Offset: 0x00298F20
		private void RefreshRoleToggle()
		{
			int uiIndex = 0;
			foreach (short roleTemplateId in ViewVillagerWork.EnabledRoleIdArray)
			{
				VillagerRoleItem roleConfigItem = (roleTemplateId < 0) ? null : VillagerRole.Instance[roleTemplateId];
				CToggle item = this.roleToggles.Get(uiIndex);
				item.onValueChanged.RemoveAllListeners();
				this.InitRoleToggle(item, roleConfigItem, uiIndex);
				uiIndex++;
			}
		}

		// Token: 0x060059E8 RID: 23016 RVA: 0x0029AD8C File Offset: 0x00298F8C
		private void RefreshSummaryPanel()
		{
			switch (this._selectedRoleUiIndex)
			{
			case 1:
				this.villagerDetailSummary.RefreshAsFarmer();
				return;
			case 3:
				this.villagerDetailSummary.RefreshAsMerchant();
				return;
			}
			this.villagerDetailSummary.gameObject.SetActive(false);
		}

		// Token: 0x060059E9 RID: 23017 RVA: 0x0029ADF7 File Offset: 0x00298FF7
		private void AutoSelectRoleToggle()
		{
			this.roleToggles.Set(this._selectedRoleUiIndex, false);
			this.RequestData();
		}

		// Token: 0x060059EA RID: 23018 RVA: 0x0029AE14 File Offset: 0x00299014
		private void OnRoleToggleChanged(bool autoSelectCharId)
		{
			this.RefreshSummaryPanel();
			this._chickenUnlocked = this._roleExtraEffectUnlockList.GetOrDefault((int)this.SelectedRoleTemplateId);
			this.villagerDetail.gameObject.SetActive(this._selectedRoleUiIndex != 0);
			this.villagerSummary.gameObject.SetActive(this._selectedRoleUiIndex == 0);
			bool flag = this._selectedRoleUiIndex == 0;
			if (flag)
			{
				this.detailScroll.SetDataCount(0);
				this.RefreshCharacterLocationFilter();
			}
			else
			{
				this.assignPanel.areaId = (int)this.areaMap.SelectedAreaId;
				this.charScroll.SetDataCount(0);
				this.detailScroll.SetDataCount(this.SortedCharacterIdList.Count);
				this.SortAndRefreshCharacterList(autoSelectCharId);
				this.RefreshDispatchLocationSelector();
				bool flag2 = this.assignPanel.areaId == -1;
				if (flag2)
				{
					this.assignPanel.gameObject.SetActive(false);
				}
				CharacterDetailDisplayDataSortAndFilterControllerController detailSortAndFilterController = this._detailSortAndFilterController;
				short[] visibleSortIds;
				if (this._chickenUnlocked)
				{
					ViewVillagerWork.ERolePage selectedRoleUiIndex = (ViewVillagerWork.ERolePage)this._selectedRoleUiIndex;
					if (!true)
					{
					}
					short[] array;
					switch (selectedRoleUiIndex)
					{
					case ViewVillagerWork.ERolePage.Farmer:
						array = new short[]
						{
							179,
							180,
							181,
							182
						};
						break;
					case ViewVillagerWork.ERolePage.Doctor:
						array = new short[]
						{
							183,
							184
						};
						break;
					case ViewVillagerWork.ERolePage.Merchant:
						array = new short[]
						{
							185,
							186,
							187,
							188,
							189
						};
						break;
					case ViewVillagerWork.ERolePage.Literati:
						array = new short[]
						{
							190,
							191,
							192,
							193
						};
						break;
					case ViewVillagerWork.ERolePage.SwordTombKeeper:
						array = new short[]
						{
							194,
							195,
							196,
							197,
							198
						};
						break;
					case ViewVillagerWork.ERolePage.VillageHead:
						array = new short[]
						{
							199,
							200
						};
						break;
					default:
						array = Array.Empty<short>();
						break;
					}
					if (!true)
					{
					}
					visibleSortIds = array;
				}
				else
				{
					ViewVillagerWork.ERolePage selectedRoleUiIndex2 = (ViewVillagerWork.ERolePage)this._selectedRoleUiIndex;
					if (!true)
					{
					}
					short[] array;
					switch (selectedRoleUiIndex2)
					{
					case ViewVillagerWork.ERolePage.Farmer:
						array = new short[]
						{
							179,
							180,
							181
						};
						break;
					case ViewVillagerWork.ERolePage.Doctor:
						array = new short[]
						{
							183
						};
						break;
					case ViewVillagerWork.ERolePage.Merchant:
						array = new short[]
						{
							185,
							186,
							187
						};
						break;
					case ViewVillagerWork.ERolePage.Literati:
						array = new short[]
						{
							190,
							191
						};
						break;
					case ViewVillagerWork.ERolePage.SwordTombKeeper:
						array = new short[]
						{
							194,
							195,
							196,
							197
						};
						break;
					case ViewVillagerWork.ERolePage.VillageHead:
						array = new short[]
						{
							199,
							200
						};
						break;
					default:
						array = Array.Empty<short>();
						break;
					}
					if (!true)
					{
					}
					visibleSortIds = array;
				}
				detailSortAndFilterController.SetVisibleSortIds(visibleSortIds);
			}
			this.RefreshAllArea(false);
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x0029B0C4 File Offset: 0x002992C4
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			base.OnNotifyGameData(notifications);
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
					DataUid uid = notification.Uid;
					bool flag = uid.DomainId == 1 && uid.DataId == 43;
					if (flag)
					{
						Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._bigEvents);
						this.RefreshSwordTombs(false);
					}
					else
					{
						bool flag2 = notification.Uid.DataId == 31;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuTeam);
						}
					}
				}
			}
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x0029B1DC File Offset: 0x002993DC
		private void HandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool pool)
		{
			bool flag = domainId == 5 && methodId == 112;
			if (flag)
			{
				bool success = false;
				Serializer.Deserialize(pool, offset, ref success);
				bool flag2 = !success;
				if (flag2)
				{
					Debug.LogError("DispatchVillagerArrangement failed");
					return;
				}
				this.RequestData();
				bool needAssignItem = this._dispatchContext.NeedAssignItem;
				if (needAssignItem)
				{
					ArgumentBox arg = EasyPool.Get<ArgumentBox>();
					arg.SetObject("OnSelectItem", new Action<TemplateKey>(delegate(TemplateKey key)
					{
						TaiwuDomainMethod.Call.AssignTargetItem(this.Element.GameDataListenerId, this._dispatchContext.CharacterId, key);
					})).SetObject("OnCancel", new Action(delegate
					{
						TaiwuDomainMethod.Call.RecallVillager(this.Element.GameDataListenerId, this._dispatchContext.CharacterId);
						this._villagerCharacterDisplayDataDict[this._dispatchContext.CharacterId].ArrangementDisplayData.ArrangementTemplateId = -1;
						this.SortAndRefreshCharacterList(true);
					})).Set("IsMerchantMode", true).Set("IsForceSelect", false).SetObject("VillagerCharacterDisplayDataDict", this._villagerCharacterDisplayDataDict).SetObject("RoleManageDisplayList", this._roleManageDisplayList).Set("ArrangementId", this.SelectedArrangementTemplateId).Set("AreaId", this.SelectedAreaId);
					UIElement.SelectAreaItem.SetOnInitArgs(arg);
					UIManager.Instance.ShowUI(UIElement.SelectAreaItem, true);
				}
				GEvent.OnEvent(UiEvents.OnVillagerDispatched, EasyPool.Get<ArgumentBox>().Set("CharacterId", this._dispatchContext.CharacterId));
			}
			bool flag3 = domainId == 5 && methodId == 113;
			if (flag3)
			{
				bool success2 = false;
				Serializer.Deserialize(pool, offset, ref success2);
				bool flag4 = !success2;
				if (flag4)
				{
					Debug.LogError("RecallVillager failed");
					return;
				}
				this.AutoSelectRoleToggle();
				GEvent.OnEvent(UiEvents.OnVillagerDispatched, EasyPool.Get<ArgumentBox>().Set("CharacterId", this._dispatchContext.CharacterId));
			}
			bool flag5 = domainId == 5 && methodId == 114;
			if (flag5)
			{
				bool success3 = false;
				Serializer.Deserialize(pool, offset, ref success3);
				bool flag6 = !success3;
				if (flag6)
				{
					Debug.LogError("AssignTargetItem failed");
				}
				else
				{
					GEvent.OnEvent(UiEvents.OnVillagerDispatched, EasyPool.Get<ArgumentBox>().Set("CharacterId", this._dispatchContext.CharacterId));
				}
			}
		}

		// Token: 0x060059ED RID: 23021 RVA: 0x0029B3E8 File Offset: 0x002995E8
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "CloseBtn";
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				bool flag2 = btn.name == "DispatchButton";
				if (flag2)
				{
					this.AlertAndDispatch();
				}
				else
				{
					bool flag3 = btn.name == "UnDispatchButton";
					if (flag3)
					{
						this.AlertAndUnDispatch();
					}
				}
			}
		}

		// Token: 0x060059EE RID: 23022 RVA: 0x0029B454 File Offset: 0x00299654
		private void AlertAndDispatch()
		{
			bool flag = this.SelectedCharacterId < 0;
			if (!flag)
			{
				VillagerRoleCharacterDisplayData displayData = this._villagerCharacterDisplayDataDict[this.SelectedCharacterId];
				bool flag2 = displayData.ArrangementDisplayData.ArrangementTemplateId != -1;
				if (flag2)
				{
					DialogCmd dialogCmd = new DialogCmd
					{
						Type = 1,
						Title = LocalStringManager.Get(LanguageKey.LK_VillagerRole_Dispatch_Dialog_HasArrangement_Title),
						Content = LocalStringManager.Get(LanguageKey.LK_VillagerRole_Dispatch_Dialog_HasArrangement_Content).ColorReplace(),
						Yes = new Action(this.DoDispatch)
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					this.DoDispatch();
				}
			}
		}

		// Token: 0x060059EF RID: 23023 RVA: 0x0029B518 File Offset: 0x00299718
		private void AlertAndUnDispatch()
		{
			bool flag = this.SelectedCharacterId < 0;
			if (!flag)
			{
				this._dispatchContext.CharacterId = this.SelectedCharacterId;
				this._dispatchContext.NeedAssignItem = false;
				TaiwuDomainMethod.Call.DispatchVillagerArrangement(this.Element.GameDataListenerId, this.SelectedCharacterId, -1, Location.Invalid);
			}
		}

		// Token: 0x060059F0 RID: 23024 RVA: 0x0029B570 File Offset: 0x00299770
		private void DoDispatch()
		{
			short arrangementId = this.SelectedArrangementTemplateId;
			bool flag = arrangementId < 0;
			if (!flag)
			{
				bool flag2 = !this.SelectedLocation;
				if (!flag2)
				{
					this._dispatchContext.CharacterId = this.SelectedCharacterId;
					this._dispatchContext.NeedAssignItem = false;
					Location location = this.DispatchingGuardingSwordTomb ? this._swordTombList.Find((DispatchSwordTombDisplayData displayData) => displayData.Id == this._selectedSwordTombId).Location : new Location(this.SelectedAreaId, -1);
					TaiwuDomainMethod.Call.DispatchVillagerArrangement(this.Element.GameDataListenerId, this.SelectedCharacterId, arrangementId, location);
				}
			}
		}

		// Token: 0x060059F1 RID: 23025 RVA: 0x0029B608 File Offset: 0x00299808
		private void OnScale(Vector3 obj)
		{
			bool flag = this.areaMap.dragMove;
			if (flag)
			{
				this.areaMap.dragMove.AdjustOffsetAfterScale();
			}
		}

		// Token: 0x060059F2 RID: 23026 RVA: 0x0029B63C File Offset: 0x0029983C
		internal void OnClickArea(short areaId)
		{
			ViewVillagerWork.<>c__DisplayClass68_0 CS$<>8__locals1 = new ViewVillagerWork.<>c__DisplayClass68_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = areaId == -1;
			if (!flag)
			{
				bool flag2 = this.SelectedCharacterId == -1;
				if (flag2)
				{
					this.areaMap.SelectedAreaTemplateId = -1;
				}
				else
				{
					AreaMap areaMap = this.areaMap;
					MapAreaData[] areas = this._mapModel.Areas;
					this.SelectedAreaId = areaId;
					areaMap.SelectedAreaTemplateIdWithoutNotify(areas[(int)areaId].GetTemplateId());
					this.RefreshDispatchButton();
					this.areaMap.LookAt(areaId, 0.5f, this.tolerance);
					CS$<>8__locals1.cfg = this._mapModel.Areas[(int)areaId].GetConfig();
					short areaTemplateId = CS$<>8__locals1.cfg.TemplateId;
					this.assignPanel.SelectArea(this.areaMap.GetTransform(areaTemplateId), (int)areaId);
					ViewVillagerWork.<>c__DisplayClass68_0 CS$<>8__locals2 = CS$<>8__locals1;
					Game.Views.World.State state = this.areaMap.states.FirstOrDefault((Game.Views.World.State x) => x.Config.TemplateId == CS$<>8__locals1.cfg.StateID);
					CS$<>8__locals2.mask = ((state != null) ? state.stateMask : null);
					bool flag3 = CS$<>8__locals1.mask;
					if (flag3)
					{
						CS$<>8__locals1.mask.Drag = true;
						CS$<>8__locals1.mask.OnPointerEnterImpl();
					}
					this.assignPanel.DeactiveAction = delegate()
					{
						CS$<>8__locals1.<>4__this.areaMap.SelectedAreaTemplateId = -1;
						bool flag5 = CS$<>8__locals1.mask;
						if (flag5)
						{
							CS$<>8__locals1.mask.Drag = false;
						}
					};
					this.assignPanel.Character = this.assignPanel.Character;
					VillagerRoleCharacterDisplayData data;
					bool flag4 = this._villagerCharacterDisplayDataDict.TryGetValue(this.SelectedCharacterId, out data);
					if (flag4)
					{
						this.assignPanel.SetToggleWithData(data);
					}
				}
			}
		}

		// Token: 0x060059F3 RID: 23027 RVA: 0x0029B7BC File Offset: 0x002999BC
		internal void OnClickRecallButton(int charId)
		{
			this._dispatchContext.CharacterId = charId;
			OrganizationDomainMethod.AsyncCall.WillCustomizePunishmentBreakWithoutVillagerHead(this, charId, delegate(int offset, RawDataPool pool)
			{
				bool willBreak = false;
				Serializer.Deserialize(pool, offset, ref willBreak);
				bool flag = willBreak;
				if (flag)
				{
					DialogCmd dialogCmd = new DialogCmd
					{
						Type = 1,
						Title = LocalStringManager.Get(LanguageKey.LK_VillagerRole_Recall_Dialog_WillBreak_Title),
						Content = LocalStringManager.Get(LanguageKey.LK_VillagerRole_Recall_Dialog_WillBreak_Content).ColorReplace(),
						Yes = new Action(base.<OnClickRecallButton>g__ConfirmRecall|1)
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
				else
				{
					base.<OnClickRecallButton>g__ConfirmRecall|1();
				}
			});
		}

		// Token: 0x060059F4 RID: 23028 RVA: 0x0029B80C File Offset: 0x00299A0C
		private void RefreshCharacterItemRelation(SelectableCharacterLongItem item, int charId)
		{
			CharacterDomainMethod.AsyncCall.GetRelationBetweenCharacters(this, this.TaiwuCharId, charId, delegate(int offset, RawDataPool pool)
			{
				ValueTuple<ushort, ushort> result = default(ValueTuple<ushort, ushort>);
				Serializer.Deserialize(pool, offset, ref result);
				item.SetCharacterRelationShow(result.Item1);
			});
		}

		// Token: 0x060059F5 RID: 23029 RVA: 0x0029B844 File Offset: 0x00299A44
		private void InitRoleToggle(CToggle item, VillagerRoleItem roleConfigItem, int uiIndex)
		{
			VillagerToggleHelper toggle = item.GetComponent<VillagerToggleHelper>();
			bool flag = toggle != null;
			if (flag)
			{
				toggle.RefreshName(this, (roleConfigItem != null) ? roleConfigItem.TemplateId : -1, roleConfigItem == null || ViewVillagerWork.RoleFunctionEnabled(roleConfigItem.TemplateId));
			}
		}

		// Token: 0x060059F6 RID: 23030 RVA: 0x0029B888 File Offset: 0x00299A88
		private void RefreshAllArea(bool focusTaiwuVillage = false)
		{
			this.areaMap.RequestData();
			this.RefreshDispatchButton();
			if (focusTaiwuVillage)
			{
				this.areaMap.LookAtTaiwuVillage();
			}
		}

		// Token: 0x060059F7 RID: 23031 RVA: 0x0029B8BC File Offset: 0x00299ABC
		private static void RefreshDisabledAreaTip(TooltipInvoker tip, bool stationUnlocked, AreaDisplayData displayData, bool canDispatchByKeeperRelationshipAndArea, bool emissaryInvalid)
		{
			tip.enabled = true;
			tip.Type = TipType.SingleDesc;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Clear();
			LanguageKey key;
			if (emissaryInvalid)
			{
				key = LanguageKey.LK_VillagerDispatch_MainAreaRestrict;
			}
			else
			{
				bool flag = !stationUnlocked;
				if (flag)
				{
					key = LanguageKey.LK_VillagerRoleDispatch_AreaTips_Lock;
				}
				else
				{
					bool isBroken = displayData.IsBroken;
					if (isBroken)
					{
						key = LanguageKey.LK_VillagerRoleDispatch_AreaTips_Broken;
					}
					else
					{
						bool flag2 = !canDispatchByKeeperRelationshipAndArea;
						if (flag2)
						{
							key = LanguageKey.LK_VillagerRole_Dispatch_Tips_NotSect;
						}
						else
						{
							key = LanguageKey.LK_VillagerRoleDispatch_AreaTips_HaveVillager;
						}
					}
				}
			}
			string tipText = key.Tr().ColorReplace();
			tip.RuntimeParam.Set("arg0", tipText);
		}

		// Token: 0x060059F8 RID: 23032 RVA: 0x0029B969 File Offset: 0x00299B69
		private bool IsAreaOccupiedByArrangement()
		{
			return false;
		}

		// Token: 0x060059F9 RID: 23033 RVA: 0x0029B96C File Offset: 0x00299B6C
		private Dictionary<short, int> FindArrangementDictOnArea(short areaId, bool sameRole = true)
		{
			Dictionary<short, int> dict = EasyPool.Get<Dictionary<short, int>>();
			dict.Clear();
			Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict = this._villagerCharacterDisplayDataDict;
			bool flag = villagerCharacterDisplayDataDict == null || villagerCharacterDisplayDataDict.Count <= 0;
			Dictionary<short, int> result;
			if (flag)
			{
				result = new Dictionary<short, int>();
			}
			else
			{
				foreach (VillagerRoleCharacterDisplayData characterDisplayData in this._villagerCharacterDisplayDataDict.Values)
				{
					bool flag2 = characterDisplayData.ArrangementDisplayData.AreaId == areaId;
					if (flag2)
					{
						bool flag3 = sameRole && characterDisplayData.RoleTemplateId != this.SelectedRoleTemplateId;
						if (!flag3)
						{
							short arrangementId = (short)characterDisplayData.ArrangementDisplayData.ArrangementTemplateId;
							bool flag4 = arrangementId == -1;
							if (!flag4)
							{
								bool flag5 = !dict.TryAdd(arrangementId, 1);
								if (flag5)
								{
									Dictionary<short, int> dictionary = dict;
									short key = arrangementId;
									int num = dictionary[key];
									dictionary[key] = num + 1;
								}
							}
						}
					}
				}
				result = dict;
			}
			return result;
		}

		// Token: 0x060059FA RID: 23034 RVA: 0x0029BA84 File Offset: 0x00299C84
		private Dictionary<short, int> FindArrangementRoleDictOnArea(short areaId, bool sameRole = true)
		{
			Dictionary<short, int> dict = EasyPool.Get<Dictionary<short, int>>();
			dict.Clear();
			foreach (VillagerRoleCharacterDisplayData characterDisplayData in this._villagerCharacterDisplayDataDict.Values)
			{
				bool flag = characterDisplayData.ArrangementDisplayData.AreaId == areaId && characterDisplayData.ArrangementDisplayData.ArrangementData != null;
				if (flag)
				{
					bool flag2 = sameRole && characterDisplayData.RoleTemplateId != this.SelectedRoleTemplateId;
					if (!flag2)
					{
						short arrangementId = (short)characterDisplayData.ArrangementDisplayData.ArrangementTemplateId;
						bool flag3 = arrangementId == -1;
						if (!flag3)
						{
							bool flag4 = !dict.TryAdd(characterDisplayData.RoleTemplateId, 1);
							if (flag4)
							{
								Dictionary<short, int> dictionary = dict;
								short roleTemplateId = characterDisplayData.RoleTemplateId;
								int num = dictionary[roleTemplateId];
								dictionary[roleTemplateId] = num + 1;
							}
						}
					}
				}
			}
			return dict;
		}

		// Token: 0x060059FB RID: 23035 RVA: 0x0029BB88 File Offset: 0x00299D88
		private void RefreshDispatchButton()
		{
			short arrangementId = this.SelectedArrangementTemplateId;
			bool interactable = arrangementId >= 0 && this.SelectedLocation && this.SelectedCharacterId >= 0;
			VillagerRoleCharacterDisplayData characterDisplayData;
			bool flag;
			if (interactable && this._villagerCharacterDisplayDataDict.TryGetValue(this.SelectedCharacterId, out characterDisplayData))
			{
				GuardingSwordTombDisplayData data = characterDisplayData.ArrangementDisplayData.ArrangementData as GuardingSwordTombDisplayData;
				if (data == null)
				{
					flag = (characterDisplayData.ArrangementDisplayData.AreaId == this.SelectedAreaId && characterDisplayData.VillagerWorkData.WorkType == 2);
				}
				else if (data.SwordTombId == this._selectedSwordTombId)
				{
					VillagerWorkData villagerWorkData = characterDisplayData.VillagerWorkData;
					flag = (villagerWorkData != null && villagerWorkData.WorkType == 2);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			bool hasOccupied = flag;
			bool invalidEmissary = this.SelectedRoleTemplateId == 6 && this.SelectedAreaId >= 0 && this._mapModel.Areas.Length > (int)this.SelectedAreaId && !this._validEmissaryArea.Contains((int)this._mapModel.Areas[(int)this.SelectedAreaId].GetConfig().TemplateId);
			interactable = (interactable && !hasOccupied && !invalidEmissary);
			this.DispatchButton.interactable = interactable;
			Selectable unDispatchButton = this.UnDispatchButton;
			VillagerRoleCharacterDisplayData selectedCh;
			bool interactable2;
			if (this.SelectedCharacterId >= 0 && this._villagerCharacterDisplayDataDict.TryGetValue(this.SelectedCharacterId, out selectedCh))
			{
				VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = selectedCh.ArrangementDisplayData;
				interactable2 = (arrangementDisplayData != null && arrangementDisplayData.ArrangementTemplateId >= 0);
			}
			else
			{
				interactable2 = false;
			}
			unDispatchButton.interactable = interactable2;
		}

		// Token: 0x060059FC RID: 23036 RVA: 0x0029BCF8 File Offset: 0x00299EF8
		private bool CheckAreaHasArrangement(short areaId, short arrangementId = 4)
		{
			Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict = this._villagerCharacterDisplayDataDict;
			bool flag = villagerCharacterDisplayDataDict == null || villagerCharacterDisplayDataDict.Count <= 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (VillagerRoleCharacterDisplayData characterDisplayData in this._villagerCharacterDisplayDataDict.Values)
				{
					GuardingSwordTombDisplayData guardingSwordTombDisplayData = characterDisplayData.ArrangementDisplayData.ArrangementData as GuardingSwordTombDisplayData;
					bool flag2 = guardingSwordTombDisplayData != null;
					if (flag2)
					{
						bool flag3 = guardingSwordTombDisplayData.SwordTombId != this._selectedSwordTombId;
						if (flag3)
						{
							continue;
						}
					}
					else
					{
						bool flag4 = characterDisplayData.ArrangementDisplayData.AreaId != areaId;
						if (flag4)
						{
							continue;
						}
					}
					bool flag5 = characterDisplayData.RoleTemplateId != this.SelectedRoleTemplateId;
					if (!flag5)
					{
						bool flag6 = characterDisplayData.ArrangementDisplayData.ArrangementTemplateId != (int)arrangementId;
						if (!flag6)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060059FD RID: 23037 RVA: 0x0029BE10 File Offset: 0x0029A010
		internal void RefreshFocusLocation()
		{
			this.assignPanel.Character = new ValueTuple<int, int>(this.SelectedCharacterId, (int)this.SelectedRoleTemplateId);
			bool flag = this.SelectedRoleTemplateId == 5;
			if (flag)
			{
				VillagerRoleCharacterDisplayData data;
				short blockId;
				bool flag2;
				GuardingSwordTombDisplayData display;
				if (this._villagerCharacterDisplayDataDict.TryGetValue(this.SelectedCharacterId, out data) && !this.assignPanel.gameObject.activeSelf)
				{
					VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = data.ArrangementDisplayData;
					if (arrangementDisplayData != null && arrangementDisplayData.ArrangementTemplateId >= 0)
					{
						IVillagerRoleArrangementDisplayData arrangementData = arrangementDisplayData.ArrangementData;
						display = (arrangementData as GuardingSwordTombDisplayData);
						if (display != null)
						{
							flag2 = this._xiangshuId2BlockId.TryGetValue(display.SwordTombId, out blockId);
							goto IL_AB;
						}
					}
				}
				flag2 = false;
				IL_AB:
				bool flag3 = flag2;
				if (flag3)
				{
					Action<short> swordTombMapLookAtBlock = this._swordTombMapLookAtBlock;
					if (swordTombMapLookAtBlock != null)
					{
						swordTombMapLookAtBlock(blockId);
					}
					this.swordTombSelector.InfoLayerToggleGroup.Set(this._key2IdMap.First((KeyValuePair<int, int> x) => x.Value == (int)display.SwordTombId).Key, false);
				}
				this.assignPanel.SetToggleWithData(data);
			}
			else
			{
				VillagerRoleCharacterDisplayData data2;
				short num;
				if (this._villagerCharacterDisplayDataDict.TryGetValue(this.SelectedCharacterId, out data2))
				{
					VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = data2.ArrangementDisplayData;
					if (arrangementDisplayData != null)
					{
						short villagerId = arrangementDisplayData.AreaId;
						if (villagerId >= 0 && arrangementDisplayData.ArrangementTemplateId >= 0)
						{
							num = villagerId;
							goto IL_155;
						}
					}
				}
				num = this._mapModel.CurrentAreaId;
				IL_155:
				short areaId = num;
				this.areaMap.SelectedAreaTemplateId = ((this.assignPanel.gameObject.activeSelf && this.areaMap.SelectedAreaTemplateId != -1) ? this.areaMap.SelectedAreaTemplateId : this._mapModel.Areas[(int)areaId].GetTemplateId());
				this.assignPanel.SetToggleWithData(data2);
			}
		}

		// Token: 0x060059FE RID: 23038 RVA: 0x0029BFD0 File Offset: 0x0029A1D0
		private void RefreshDispatchLocationSelector()
		{
			short arrangementId = this.SelectedArrangementTemplateId;
			Debug.Log(arrangementId);
			bool flag = arrangementId < 0;
			if (flag)
			{
				this.RefreshSwordTombs(false);
				this.RefreshAllArea(false);
			}
			else
			{
				bool dispatchingGuardingSwordTomb = this.DispatchingGuardingSwordTomb;
				if (dispatchingGuardingSwordTomb)
				{
					this.areaMap.gameObject.SetActive(false);
					this.swordTombSelector.gameObject.SetActive(true);
					this.RefreshSwordTombs(false);
				}
				else
				{
					this.areaMap.gameObject.SetActive(true);
					this.swordTombSelector.gameObject.SetActive(false);
					this.RefreshAllArea(false);
				}
			}
		}

		// Token: 0x060059FF RID: 23039 RVA: 0x0029C074 File Offset: 0x0029A274
		internal void RefreshSwordTombs(bool autoLookAt)
		{
			short areaId = this._mapModel.GetTaiwuVillageAreaId();
			byte areaSize = this._mapModel.GetAreaSize(areaId);
			RectTransform infoLayer = this.swordTombSelector.InfoLayer;
			GuardingSwordTombDisplayData guardingSwordTomb;
			VillagerRoleCharacterDisplayData character;
			bool flag;
			if (this.assignPanel.gameObject.activeSelf && this._villagerCharacterDisplayDataDict.TryGetValue(this.SelectedCharacterId, out character))
			{
				VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = character.ArrangementDisplayData;
				if (arrangementDisplayData != null && arrangementDisplayData.ArrangementTemplateId >= 0)
				{
					IVillagerRoleArrangementDisplayData arrangementData = arrangementDisplayData.ArrangementData;
					guardingSwordTomb = (arrangementData as GuardingSwordTombDisplayData);
					if (guardingSwordTomb != null)
					{
						flag = (guardingSwordTomb.SwordTombId >= 0);
						goto IL_A3;
					}
				}
				flag = false;
				IL_A3:;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.swordTombSelector.InfoLayerToggleGroup.Set(this._key2IdMap.FirstOrDefault((KeyValuePair<int, int> kv) => kv.Value == (int)guardingSwordTomb.SwordTombId).Key, true);
			}
			foreach (object obj in infoLayer)
			{
				RectTransform swordTombRect = (RectTransform)obj;
				short num;
				int xiangshuId;
				bool flag3 = int.TryParse(swordTombRect.name, out xiangshuId) && this._xiangshuId2BlockId.TryGetValue((sbyte)xiangshuId, out num);
				if (flag3)
				{
					SwordTombPiece swordTombPiece = swordTombRect.GetComponent<SwordTombPiece>();
					SwordTombPiece swordTombPiece2 = swordTombPiece;
					Dictionary<int, VillagerRoleCharacterDisplayData> villagerCharacterDisplayDataDict = this._villagerCharacterDisplayDataDict;
					swordTombPiece2.SetCount((villagerCharacterDisplayDataDict != null) ? villagerCharacterDisplayDataDict.Values.Count(delegate(VillagerRoleCharacterDisplayData data)
					{
						VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData2 = data.ArrangementDisplayData;
						if (arrangementDisplayData2 != null && arrangementDisplayData2.ArrangementTemplateId >= 0)
						{
							GuardingSwordTombDisplayData gData = arrangementDisplayData2.ArrangementData as GuardingSwordTombDisplayData;
							if (gData != null)
							{
								return (int)gData.SwordTombId == xiangshuId;
							}
						}
						return false;
					}) : 0);
					SwordTombItem cfg = SwordTomb.Instance[xiangshuId];
					Dictionary<short, BigEventRecord> bigEvents = this._bigEvents;
					BigEventRecord bigEvent;
					bool flag4 = bigEvents != null && bigEvents.TryGetValue(cfg.BigEventWhenRemoved, out bigEvent);
					if (flag4)
					{
						TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
						swordTombPiece.SetRemoved(LanguageKey.LK_VillagerRole_SwordTomb_Removed.TrFormat(timeManager.GetYearByDate(bigEvent.OccurDate), (int)(timeManager.GetMonthInYear(bigEvent.OccurDate) + 1), LocalStringManager.Get(string.Format("LK_SwordTomb_{0}", xiangshuId)).SetColor("red")));
					}
					else
					{
						swordTombPiece.SetRemoved(LanguageKey.LK_VillagerRole_SwordTomb_Removed.TrFormat(LanguageKey.LK_UnknownTimePoint.Tr(), LanguageKey.LK_UnknownTimePoint.Tr(), LocalStringManager.Get(string.Format("LK_SwordTomb_{0}", xiangshuId)).SetColor("red")));
					}
				}
			}
			if (autoLookAt)
			{
				DispatchSwordTombDisplayData dispatchSwordTombDisplayData = this._swordTombList.Find((DispatchSwordTombDisplayData displayData) => displayData.Id == this._selectedSwordTombId);
				short? num2 = (dispatchSwordTombDisplayData != null) ? new short?(dispatchSwordTombDisplayData.Location.BlockId) : null;
				short id;
				bool flag5;
				if (num2 != null)
				{
					id = num2.GetValueOrDefault();
					flag5 = (id != -1);
				}
				else
				{
					flag5 = false;
				}
				bool flag6 = flag5;
				if (flag6)
				{
					Action<short> swordTombMapLookAtBlock = this._swordTombMapLookAtBlock;
					if (swordTombMapLookAtBlock != null)
					{
						swordTombMapLookAtBlock(id);
					}
				}
				else
				{
					using (Dictionary<sbyte, short>.Enumerator enumerator2 = this._xiangshuId2BlockId.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							KeyValuePair<sbyte, short> pair = enumerator2.Current;
							Action<short> swordTombMapLookAtBlock2 = this._swordTombMapLookAtBlock;
							if (swordTombMapLookAtBlock2 != null)
							{
								swordTombMapLookAtBlock2(pair.Value);
							}
							return;
						}
					}
					Action<short> swordTombMapLookAtBlock3 = this._swordTombMapLookAtBlock;
					if (swordTombMapLookAtBlock3 != null)
					{
						swordTombMapLookAtBlock3(WorldMapModel.CoordinateToIndex(new ByteCoordinate(areaSize / 2, areaSize / 2), areaSize));
					}
				}
			}
		}

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06005A00 RID: 23040 RVA: 0x0029C424 File Offset: 0x0029A624
		public string ColoredRoleName
		{
			get
			{
				return this.TaiwuVillagerRoleDisplayData.VillagerRoleNpcNickNames[(int)this.SelectedRoleTemplateId].SetColor(string.Format("GradeColor_{0}", OrganizationMember.Instance[VillagerRole.Instance[this.SelectedRoleTemplateId].OrganizationMember].Grade));
			}
		}

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06005A01 RID: 23041 RVA: 0x0029C47B File Offset: 0x0029A67B
		[CanBeNull]
		public VillagerRoleArrangementItem DefaultRoleAction
		{
			get
			{
				return VillagerRoleArrangement.Instance.FirstOrDefault((VillagerRoleArrangementItem item) => !item.InvisibleInGui && item.VillagerRole == this.SelectedRoleTemplateId);
			}
		}

		// Token: 0x06005A02 RID: 23042 RVA: 0x0029C494 File Offset: 0x0029A694
		[CanBeNull]
		public VillagerRoleCharacterDisplayData PrepareShowEffect(int charId)
		{
			Tween dispatchEffectAnim = this._dispatchEffectAnim;
			if (dispatchEffectAnim != null)
			{
				dispatchEffectAnim.Kill(true);
			}
			VillagerRoleCharacterDisplayData data = this.TaiwuVillagerRoleDisplayData.Villagers.GetValueOrDefault(charId);
			bool flag = data == null;
			VillagerRoleCharacterDisplayData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				this._dispatchEffectAnim = DOTween.Sequence().Append(this.effectRender.DOFade(1f, this.animShow)).AppendInterval(this.animWait).Append(this.effectRender.DOFade(0f, this.animHide)).OnComplete(delegate
				{
					this._dispatchEffectAnim = null;
				});
				result = data;
			}
			return result;
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x0029C538 File Offset: 0x0029A738
		public void ShowTombDispatchEffect(int charId)
		{
			VillagerRoleCharacterDisplayData data = this.PrepareShowEffect(charId);
			bool flag = data == null;
			if (!flag)
			{
				TMP_Text tmp_Text = this.effectText;
				VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = data.ArrangementDisplayData;
				GuardingSwordTombDisplayData guardingSwordTomb = ((arrangementDisplayData != null) ? arrangementDisplayData.ArrangementData : null) as GuardingSwordTombDisplayData;
				string srcString;
				if (guardingSwordTomb != null)
				{
					VillagerWorkData villagerWorkData = data.VillagerWorkData;
					if (villagerWorkData != null && villagerWorkData.WorkType == 2)
					{
						LanguageKey languageKey = LanguageKey.LK_VillagerRole_Dispatched;
						object[] array = new object[4];
						array[0] = this.ColoredRoleName;
						array[1] = NameCenter.GetDisplayName(ref data.Name, false);
						array[2] = LocalStringManager.Get(string.Format("LK_SwordTomb_{0}", guardingSwordTomb.SwordTombId));
						int num = 3;
						VillagerRoleArrangementItem defaultRoleAction = this.DefaultRoleAction;
						array[num] = (((defaultRoleAction != null) ? defaultRoleAction.DescName : null) ?? LanguageKey.LK_VillagerRole_Dispatched_Task.Tr());
						srcString = languageKey.TrFormat(array);
						goto IL_D3;
					}
				}
				srcString = LanguageKey.LK_VillagerRole_Dispatched_Redraw.TrFormat(this.ColoredRoleName, NameCenter.GetDisplayName(ref data.Name, false));
				IL_D3:
				tmp_Text.text = srcString.ColorReplace();
			}
		}

		// Token: 0x06005A04 RID: 23044 RVA: 0x0029C624 File Offset: 0x0029A824
		public void ShowNormalDispatchEffect(int charId, [CanBeNull] string roleActionName = null)
		{
			VillagerRoleCharacterDisplayData data = this.PrepareShowEffect(charId);
			bool flag = data == null;
			if (!flag)
			{
				TMP_Text tmp_Text = this.effectText;
				VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = data.ArrangementDisplayData;
				string srcString;
				if (arrangementDisplayData != null)
				{
					short areaId = arrangementDisplayData.AreaId;
					if (areaId != -1)
					{
						LanguageKey languageKey = LanguageKey.LK_VillagerRole_Dispatched;
						object[] array = new object[4];
						array[0] = this.ColoredRoleName;
						array[1] = NameCenter.GetDisplayName(ref data.Name, false);
						array[2] = this.areaMap.MapModel.Areas[(int)areaId].GetConfig().Name;
						int num = 3;
						object obj = roleActionName;
						if (roleActionName == null)
						{
							VillagerRoleArrangementItem defaultRoleAction = this.DefaultRoleAction;
							obj = (((defaultRoleAction != null) ? defaultRoleAction.DescName : null) ?? LanguageKey.LK_VillagerRole_Dispatched_Task.Tr());
						}
						array[num] = obj;
						srcString = languageKey.TrFormat(array);
						goto IL_C1;
					}
				}
				srcString = LanguageKey.LK_VillagerRole_Dispatched_Redraw.TrFormat(this.ColoredRoleName, NameCenter.GetDisplayName(ref data.Name, false));
				IL_C1:
				tmp_Text.text = srcString.ColorReplace();
			}
		}

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06005A05 RID: 23045 RVA: 0x0029C6FD File Offset: 0x0029A8FD
		private CButton DispatchButton
		{
			get
			{
				return this.assignPanel.assign;
			}
		}

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x06005A06 RID: 23046 RVA: 0x0029C70A File Offset: 0x0029A90A
		private CButton UnDispatchButton
		{
			get
			{
				return this.assignPanel.unassign;
			}
		}

		// Token: 0x04003DBE RID: 15806
		private bool _inited;

		// Token: 0x04003DBF RID: 15807
		private WorldMapModel _mapModel;

		// Token: 0x04003DC0 RID: 15808
		private Dictionary<short, BigEventRecord> _bigEvents;

		// Token: 0x04003DC1 RID: 15809
		private CharacterSet _taiwuTeam;

		// Token: 0x04003DC2 RID: 15810
		public static readonly short[] EnabledRoleIdArray = new short[]
		{
			-1,
			0,
			2,
			3,
			4,
			5,
			6
		};

		// Token: 0x04003DC3 RID: 15811
		private HashSet<int> _validEmissaryArea;

		// Token: 0x04003DC4 RID: 15812
		private bool _chickenUnlocked;

		// Token: 0x04003DC5 RID: 15813
		private bool _detailFilterInited;

		// Token: 0x04003DC6 RID: 15814
		[TupleElementNames(new string[]
		{
			"location",
			"charData"
		})]
		private List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> _locations;

		// Token: 0x04003DC7 RID: 15815
		[TupleElementNames(new string[]
		{
			"location",
			"charData"
		})]
		private List<ValueTuple<CharacterLocationDisplayData, VillagerRoleCharacterDisplayData>> _orderedLocations;

		// Token: 0x04003DC8 RID: 15816
		private int _selectedRoleUiIndex;

		// Token: 0x04003DC9 RID: 15817
		internal short SelectedAreaId = -1;

		// Token: 0x04003DCA RID: 15818
		internal int SelectedCharacterId = -1;

		// Token: 0x04003DCB RID: 15819
		private sbyte _selectedSwordTombId = -1;

		// Token: 0x04003DCC RID: 15820
		private ViewVillagerWork.AssignItemContext _dispatchContext;

		// Token: 0x04003DCD RID: 15821
		internal Dictionary<int, VillagerRoleCharacterDisplayData> _villagerCharacterDisplayDataDict = new Dictionary<int, VillagerRoleCharacterDisplayData>();

		// Token: 0x04003DCE RID: 15822
		private List<VillagerRoleManageDisplayData> _roleManageDisplayList;

		// Token: 0x04003DCF RID: 15823
		private IReadOnlyList<bool> _roleExtraEffectUnlockList;

		// Token: 0x04003DD0 RID: 15824
		private List<DispatchSwordTombDisplayData> _swordTombList = new List<DispatchSwordTombDisplayData>();

		// Token: 0x04003DD1 RID: 15825
		internal readonly List<int> SortedCharacterIdList = new List<int>();

		// Token: 0x04003DD2 RID: 15826
		internal TaiwuVillagerRoleDisplayData TaiwuVillagerRoleDisplayData;

		// Token: 0x04003DD3 RID: 15827
		private Dictionary<int, int> _key2IdMap = new Dictionary<int, int>();

		// Token: 0x04003DD4 RID: 15828
		private Action<short> _swordTombMapLookAtBlock;

		// Token: 0x04003DD5 RID: 15829
		private readonly Dictionary<sbyte, short> _xiangshuId2BlockId = new Dictionary<sbyte, short>();

		// Token: 0x04003DD6 RID: 15830
		private List<int> _characterIdList = new List<int>();

		// Token: 0x04003DD7 RID: 15831
		[SerializeField]
		private Vector2 tolerance = new Vector2(719f, 215f);

		// Token: 0x04003DD8 RID: 15832
		private Tween _dispatchEffectAnim;

		// Token: 0x04003DD9 RID: 15833
		[SerializeField]
		internal AreaMap areaMap;

		// Token: 0x04003DDA RID: 15834
		[SerializeField]
		private MouseWheelScale scaleAndMoveRoot;

		// Token: 0x04003DDB RID: 15835
		[SerializeField]
		private CToggleGroup roleToggles;

		// Token: 0x04003DDC RID: 15836
		[SerializeField]
		private UnityEngine.Material materialGrayScale;

		// Token: 0x04003DDD RID: 15837
		[SerializeField]
		private GameObject villagerSummary;

		// Token: 0x04003DDE RID: 15838
		[SerializeField]
		private GameObject villagerDetail;

		// Token: 0x04003DDF RID: 15839
		[SerializeField]
		private VillagerDetailSummary villagerDetailSummary;

		// Token: 0x04003DE0 RID: 15840
		[SerializeField]
		internal InfinityScroll charScroll;

		// Token: 0x04003DE1 RID: 15841
		[SerializeField]
		internal InfinityScroll detailScroll;

		// Token: 0x04003DE2 RID: 15842
		[SerializeField]
		private SortAndFilter locationSortAndFilter;

		// Token: 0x04003DE3 RID: 15843
		[SerializeField]
		private SortAndFilter detailSortAndFilter;

		// Token: 0x04003DE4 RID: 15844
		[SerializeField]
		private TMP_Text idleCount;

		// Token: 0x04003DE5 RID: 15845
		[SerializeField]
		private TMP_Text keepGraveCount;

		// Token: 0x04003DE6 RID: 15846
		[SerializeField]
		private TMP_Text unassignedCount;

		// Token: 0x04003DE7 RID: 15847
		[SerializeField]
		private TMP_Text blockCount;

		// Token: 0x04003DE8 RID: 15848
		[SerializeField]
		private TMP_Text ruinedCount;

		// Token: 0x04003DE9 RID: 15849
		[SerializeField]
		private List<TMP_Text> extraLocations;

		// Token: 0x04003DEA RID: 15850
		[SerializeField]
		private SimpleWorldMap swordTombSelector;

		// Token: 0x04003DEB RID: 15851
		[SerializeField]
		internal AssignPanel assignPanel;

		// Token: 0x04003DEC RID: 15852
		[SerializeField]
		private CanvasGroup effectRender;

		// Token: 0x04003DED RID: 15853
		[SerializeField]
		private TMP_Text effectText;

		// Token: 0x04003DEE RID: 15854
		[SerializeField]
		private TMP_Text[] toggleText;

		// Token: 0x04003DEF RID: 15855
		[SerializeField]
		private float animShow = 0.5f;

		// Token: 0x04003DF0 RID: 15856
		[SerializeField]
		private float animWait = 5f;

		// Token: 0x04003DF1 RID: 15857
		[SerializeField]
		private float animHide = 0.5f;

		// Token: 0x04003DF2 RID: 15858
		private CharacterLocationDisplayDataSortAndFilterControllerController _locationSortAndFilterController;

		// Token: 0x04003DF3 RID: 15859
		private CharacterDetailDisplayDataSortAndFilterControllerController _detailSortAndFilterController;

		// Token: 0x02001C23 RID: 7203
		private struct AssignItemContext
		{
			// Token: 0x0400BFA1 RID: 49057
			public bool NeedAssignItem;

			// Token: 0x0400BFA2 RID: 49058
			public int CharacterId;
		}

		// Token: 0x02001C24 RID: 7204
		public enum ERolePage
		{
			// Token: 0x0400BFA4 RID: 49060
			Summary,
			// Token: 0x0400BFA5 RID: 49061
			Farmer,
			// Token: 0x0400BFA6 RID: 49062
			Doctor,
			// Token: 0x0400BFA7 RID: 49063
			Merchant,
			// Token: 0x0400BFA8 RID: 49064
			Literati,
			// Token: 0x0400BFA9 RID: 49065
			SwordTombKeeper,
			// Token: 0x0400BFAA RID: 49066
			VillageHead
		}
	}
}
