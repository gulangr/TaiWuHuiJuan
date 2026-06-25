using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Make
{
	// Token: 0x02000954 RID: 2388
	public class MakeSubPageMake : MakeSubPage
	{
		// Token: 0x17000CE6 RID: 3302
		// (get) Token: 0x0600710A RID: 28938 RVA: 0x00345C20 File Offset: 0x00343E20
		private ViewMake.ItemSourceTogKey CurMaterialTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.materialSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CE7 RID: 3303
		// (get) Token: 0x0600710B RID: 28939 RVA: 0x00345C2D File Offset: 0x00343E2D
		private ItemSourceType CurMaterialItemSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurMaterialTogKey);
			}
		}

		// Token: 0x17000CE8 RID: 3304
		// (get) Token: 0x0600710C RID: 28940 RVA: 0x00345C3A File Offset: 0x00343E3A
		private bool IsPerfect
		{
			get
			{
				return this.targetSlot.IsToggleOn;
			}
		}

		// Token: 0x17000CE9 RID: 3305
		// (get) Token: 0x0600710D RID: 28941 RVA: 0x00345C48 File Offset: 0x00343E48
		private MakeResult CurMakeResult
		{
			get
			{
				bool flag = this._makeResultDict.Count == 1;
				MakeResult result;
				if (flag)
				{
					result = this._makeResultDict.Values.First<MakeResult>();
				}
				else
				{
					MakeResult makeResult;
					this._makeResultDict.TryGetValue((int)(this._isManual ? this._makeItemSubTypeId : -1), out makeResult);
					result = makeResult;
				}
				return result;
			}
		}

		// Token: 0x17000CEA RID: 3306
		// (get) Token: 0x0600710E RID: 28942 RVA: 0x00345C9F File Offset: 0x00343E9F
		private bool InGuiding
		{
			get
			{
				return SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			}
		}

		// Token: 0x0600710F RID: 28943 RVA: 0x00345CAC File Offset: 0x00343EAC
		public override void Init(ViewMake parentView)
		{
			base.Init(parentView);
			this.ParentView.ShowToolPanel(false);
			this._isAutoSelectTool = false;
			this.ClearMakeData();
			this.materialSourceToggleGroup.OnActiveIndexChange -= this.MaterialSourceToggleGroupOnOnActiveIndexChange;
			this.materialSourceToggleGroup.OnActiveIndexChange += this.MaterialSourceToggleGroupOnOnActiveIndexChange;
			this.materialSourceToggleGroup.Init(-1);
			this.targetSlot.Init(EMakeTargetSlotInteract.Always, EMakeTargetSlotType.MakeTarget, new Action<int, ItemDisplayData>(this.OnCancelTarget), new Action(this.OnSelectTarget), null, new Action<bool>(this.OnMakePerfectToggleValueChanged), -1, new Func<bool>(this.GetShowPerfectToggle), false, null);
			this.materialSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.MakeMaterial, new Action<int, ItemDisplayData>(this.OnCancelMaterial), delegate
			{
			}, new Func<bool>(this.GetMaterialInteractable), null, -1, null, false, null);
			this.toolSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.Tool, new Action<int, ItemDisplayData>(this.OnCancelTool), delegate
			{
			}, new Func<bool>(this.GetToolInteractable), new Action<bool>(this.OnAutoSelectToolToggleChange), -1, null, false, null);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.targetPanel.gameObject.SetActive(false);
			this.targetListScroll.CustomMakeAttainmentDataGenerator = ((ITradeableContent content) => this.GetMakeCellData(content, MakeCellType.Attainment));
			this.targetListScroll.CustomMakeToolDataGenerator = ((ITradeableContent content) => this.GetMakeCellData(content, MakeCellType.Tool));
			this.targetListScroll.CustomMakeMaterialDataGenerator = ((ITradeableContent content) => this.GetMakeCellData(content, MakeCellType.Material));
			this.targetListScroll.Init("MakeSubPageMakeTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderTarget), new Action<ITradeableContent, RowItemLine>(this.OnItemClickTarget), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.MakeAttainment | ItemListScroll.EColumnType.MakeTool | ItemListScroll.EColumnType.MakeMaterial, null, null, null);
			List<int> list;
			if (this.InGuiding)
			{
				list = new List<int>
				{
					EMainFilterKeys.Misc.ToInt()
				};
			}
			else
			{
				sbyte curLifeSkillType = this.ParentView.CurLifeSkillType;
				if (!true)
				{
				}
				List<int> list2;
				switch (curLifeSkillType)
				{
				case 6:
				case 7:
				case 11:
					list2 = new List<int>
					{
						EMainFilterKeys.Equip.ToInt()
					};
					goto IL_2DB;
				case 8:
				case 9:
					list2 = new List<int>
					{
						EMainFilterKeys.Medicine.ToInt(),
						EMainFilterKeys.Equip.ToInt()
					};
					goto IL_2DB;
				case 10:
					list2 = new List<int>
					{
						EMainFilterKeys.Equip.ToInt(),
						EMainFilterKeys.Misc.ToInt()
					};
					goto IL_2DB;
				case 14:
					list2 = new List<int>
					{
						EMainFilterKeys.Food.ToInt()
					};
					goto IL_2DB;
				}
				throw new ArgumentOutOfRangeException();
				IL_2DB:
				if (!true)
				{
				}
				list = list2;
			}
			List<int> toggleIndexList = list;
			this.targetListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), toggleIndexList, true);
			this.targetListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), -1);
			this._upgradeBuildingConfig = BuildingBlock.Instance.FirstOrDefault((BuildingBlockItem b) => b.UpgradeMakeItem && b.RequireLifeSkillType == this.ParentView.CurLifeSkillType);
			this._materialList.Clear();
			this.materialPanel.gameObject.SetActive(false);
			this.materialListScroll.Init("MakeSubPageMakeMaterial", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRenderMaterial), new Action<ITradeableContent, RowItemLine>(this.OnItemClickMaterial), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, this.GenerateMaterialListColumnDefinitions(), new Action<RowItem>(this.GenerateMaterialListRowTemplateContainers));
			this.materialListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Material.ToInt());
			this.materialListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Material.ToInt());
			this.materialListScroll.SetItemList(this._materialList);
			this.materialSourceToggleGroup.Init(-1);
			this.RefreshMakeTypeDropdown();
			this.InitSubTypeToggleGroup();
			this.InitResourcePanel();
			this.InitMakeCount();
			this.InitStorageDropdown();
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnClickButtonConfirm));
			this.buttonCancelSelectTarget.ClearAndAddListener(new Action(this.OnCancelSelectTarget));
			this.toolSlot.gameObject.SetActive(false);
			this.materialSlot.gameObject.SetActive(false);
			this.perfectDropdown.onValueChanged.RemoveAllListeners();
			this.perfectDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnPerfectDropdownValueChanged));
			this.perfectDropdown.OnItemAdded = new Action<int, RectTransform>(this.OnPerfectDropdownItemAdded);
			this.RefreshPerfectDropdown();
			this.targetSlot.SetEffectHandlerState(true);
			bool inGuiding = this.InGuiding;
			if (inGuiding)
			{
				this.materialSourceToggleGroup.Set(ViewMake.ItemSourceTogKey.Warehouse.ToInt(), false);
				this.ParentView.ToolSourceToggleGroup.Set(ViewMake.ItemSourceTogKey.Warehouse.ToInt(), false);
			}
			switch (this.ParentView.CurLifeSkillType)
			{
			case 6:
				this._canMakeItemSubTypeList = new List<short>
				{
					0,
					1,
					2,
					3,
					4,
					5,
					8,
					9,
					10,
					11,
					12,
					100,
					101,
					102,
					103,
					200
				};
				break;
			case 7:
				this._canMakeItemSubTypeList = new List<short>
				{
					12,
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					100,
					101,
					102,
					103,
					200,
					400
				};
				break;
			case 8:
				this._canMakeItemSubTypeList = new List<short>
				{
					14,
					800
				};
				break;
			case 9:
				this._canMakeItemSubTypeList = new List<short>
				{
					15,
					801
				};
				break;
			case 10:
				this._canMakeItemSubTypeList = new List<short>
				{
					4,
					6,
					13,
					7,
					100,
					101,
					102,
					103,
					200,
					300,
					201,
					1206
				};
				break;
			case 11:
				this._canMakeItemSubTypeList = new List<short>
				{
					0,
					1,
					2,
					3,
					4,
					5,
					8,
					9,
					10,
					11,
					13,
					100,
					101,
					102,
					103,
					200
				};
				break;
			case 14:
				this._canMakeItemSubTypeList = new List<short>
				{
					701,
					700
				};
				break;
			}
		}

		// Token: 0x06007110 RID: 28944 RVA: 0x003464F0 File Offset: 0x003446F0
		private IEnumerable<ColumnDefinition> GenerateMaterialListColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 200f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = 0;
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, IconAndTextCellData> columnDefinition2 = new ColumnDefinition<ITradeableContent, IconAndTextCellData>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 200f,
				FlexibleWidth = 1f,
				PreferredWidth = 200f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Make_Target_Attainment_Tip_Title.Tr());
			columnDefinition2.CellDataGenerator = delegate(ITradeableContent d)
			{
				MaterialItem materialConfig = Config.Material.Instance[d.RealKey.TemplateId];
				LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[materialConfig.RequiredLifeSkillType];
				ItemDisplayData itemData = d as ItemDisplayData;
				bool flag = !MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
				IconAndTextCellData result;
				if (flag)
				{
					short num;
					int num2;
					int num3;
					int num4;
					int needAttainment = this.GetMakeNeedAttainment(d.RealKey, out num, out num2, out num3, out num4);
					itemData.MakeNeedAttainment = needAttainment;
					result = new IconAndTextCellData(skillConfig.Icon, string.Format("{0}", needAttainment), true, false, false, false);
				}
				else
				{
					itemData.MakeNeedAttainment = (int)materialConfig.RequiredAttainment;
					result = new IconAndTextCellData(skillConfig.Icon, string.Format("{0}", materialConfig.RequiredAttainment), true, false, false, false);
				}
				return result;
			};
			columnDefinition2.SortId = 156;
			yield return columnDefinition2;
			ColumnDefinition<ITradeableContent, string> columnDefinition3 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition3.LayoutOption = new LayoutOption
			{
				MinWidth = 150f,
				FlexibleWidth = 1f,
				PreferredWidth = 150f,
				Priority = 1
			};
			columnDefinition3.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition3.CellDataGenerator = ((ITradeableContent data) => CommonUtils.GetDisplayStringForNum(data.Amount, 100000));
			columnDefinition3.SortId = 17;
			yield return columnDefinition3;
			yield break;
		}

		// Token: 0x06007111 RID: 28945 RVA: 0x00346500 File Offset: 0x00344700
		private void GenerateMaterialListRowTemplateContainers(RowItem rowItem)
		{
			MakeSubPageMake.<GenerateMaterialListRowTemplateContainers>g__CreateCellContainers|79_0(rowItem.ContainerRoot, this.itemIconAndNameCellContainer);
			MakeSubPageMake.<GenerateMaterialListRowTemplateContainers>g__CreateCellContainers|79_0(rowItem.ContainerRoot, this.iconAndTextCellContainer);
			MakeSubPageMake.<GenerateMaterialListRowTemplateContainers>g__CreateCellContainers|79_0(rowItem.ContainerRoot, this.singleTextCellContainer);
		}

		// Token: 0x06007112 RID: 28946 RVA: 0x0034653C File Offset: 0x0034473C
		private void OnAutoSelectToolToggleChange(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake = isOn;
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
			bool flag = isOn && this.materialSlot.IsValid;
			if (flag)
			{
				this.ParentView.AutoSelectTool();
			}
		}

		// Token: 0x06007113 RID: 28947 RVA: 0x00346584 File Offset: 0x00344784
		private void OnEnable()
		{
			GEvent.Add(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
			this.collectResource.Clear();
		}

		// Token: 0x06007114 RID: 28948 RVA: 0x003465AC File Offset: 0x003447AC
		private void OnDisable()
		{
			GEvent.Remove(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
			this.subTypeToggleGroup.gameObject.SetActive(false);
			this.propertyToolAttainment.gameObject.SetActive(false);
			this.resourcePanel.gameObject.SetActive(false);
			this.collectResource.Clear();
		}

		// Token: 0x06007115 RID: 28949 RVA: 0x00346615 File Offset: 0x00344815
		private void OnTaiwuResourceChange(ArgumentBox _)
		{
			this.CheckCondition();
		}

		// Token: 0x06007116 RID: 28950 RVA: 0x00346620 File Offset: 0x00344820
		public override void Refresh(BuildingMakeDisplayData displayData)
		{
			base.Refresh(displayData);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.ParentView.RefreshSourceToggleInteractable(this.materialSourceToggleGroup);
			this.RefreshAllMaterialList();
			this.RefreshTargetList();
			this.ReloadSlot();
			this.RefreshMakeTypeDropdown();
			this.RefreshStorageDropdown();
			this.CheckCondition();
		}

		// Token: 0x06007117 RID: 28951 RVA: 0x00346688 File Offset: 0x00344888
		private void ReloadSlot()
		{
			bool flag = !this.targetSlot.IsValid;
			if (!flag)
			{
				ItemDisplayData targetData = this._targetList.Find((ItemDisplayData d) => d.RealKey == this.targetSlot.ItemData.RealKey);
				bool flag2 = targetData == null;
				if (flag2)
				{
					this.targetSlot.Cancel();
				}
				else
				{
					this.SelectTarget(targetData, false);
					bool flag3 = !this.materialSlot.IsValid;
					if (!flag3)
					{
						ItemDisplayData materialData = this._materialList.Find((ItemDisplayData d) => d.RealKey == this.materialSlot.ItemData.RealKey);
						bool flag4 = materialData == null;
						if (flag4)
						{
							this.materialSlot.Cancel();
						}
						else
						{
							this.SelectMaterial(materialData, false);
							bool flag5 = !this.toolSlot.IsValid;
							if (!flag5)
							{
								ItemDisplayData toolData = (this.toolSlot.ItemData == this.ParentView.EmptyTool) ? this.ParentView.EmptyTool : this.ParentView.AllToolList.Find((ItemDisplayData d) => d.RealKey == this.toolSlot.ItemData.RealKey && d.ItemSourceTypeEnum == this.toolSlot.ItemData.ItemSourceTypeEnum);
								bool flag6 = toolData == null;
								if (flag6)
								{
									this.toolSlot.Cancel();
								}
								else
								{
									this.SelectTool(toolData, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06007118 RID: 28952 RVA: 0x003467BA File Offset: 0x003449BA
		private void OnSelectTarget()
		{
			this.targetPanel.gameObject.SetActive(true);
			this.ParentView.EnterFocusMode(this.targetPanel.transform, null);
		}

		// Token: 0x06007119 RID: 28953 RVA: 0x003467E8 File Offset: 0x003449E8
		private void SelectTarget(ItemDisplayData itemData, bool check = true)
		{
			if (check)
			{
				this._lastMakeResourceCountInts.Initialize();
				this._curMakeResourceCountInts.Initialize();
			}
			this._isAutoSelectTool = false;
			this.targetPanel.gameObject.SetActive(false);
			this.ParentView.ExitFocusMode();
			this.targetSlot.Select(itemData, false);
			bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(itemData);
			bool flag = isRandomMake;
			if (flag)
			{
				this._currentSelectRandomMakeItemSubType = itemData.Key.TemplateId;
				string iconKey = MakeSubPageMakeHelper.GetRandomMakeIcon(itemData.Key.TemplateId);
				this.targetSlot.SetRandomIcon(iconKey);
				bool flag2 = !this.materialSlot.IsValid;
				if (flag2)
				{
					this.ClearMakeData();
				}
				this.RefreshMaterialList();
			}
			else
			{
				this._makeItemSubTypeId = ItemTemplateHelper.GetMakeItemSubType(itemData.RealKey.ItemType, itemData.RealKey.TemplateId);
				this._isManual = true;
				MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance.FirstOrDefault((MakeItemTypeItem m) => m.MakeItemSubTypes.Contains(this._makeItemSubTypeId));
				this._makeItemTypeId = makeItemTypeConfig.TemplateId;
				this.targetSlot.Refresh(false);
				this.RefreshMaterialList();
			}
			this.materialSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.ParentView.SetRightMask(true);
			bool flag3 = !isRandomMake && check;
			if (flag3)
			{
				bool isValid = this.targetSlot.IsValid;
				if (isValid)
				{
					this.AutoSelectMaterial();
				}
				bool isValid2 = this.materialSlot.IsValid;
				if (isValid2)
				{
					this.ParentView.AutoSelectTool();
					this._isAutoSelectTool = true;
				}
			}
			this.RefreshPerfectDropdown();
			this.targetListScroll.ReRender();
			if (check)
			{
				this.CheckCondition();
			}
		}

		// Token: 0x0600711A RID: 28954 RVA: 0x003469A0 File Offset: 0x00344BA0
		private void OnCancelTarget(int index, ItemDisplayData itemDisplayData)
		{
			this.ClearMakeData();
			this.RefreshPerfectDropdown();
			this.targetListScroll.ReRender();
			this.materialSlot.Cancel();
			this.materialSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.ClearMaterialList();
			this.CheckCondition();
			this.ParentView.ClearToolList("");
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.toolSlot.gameObject.SetActive(false);
			this.materialSlot.gameObject.SetActive(false);
			this.goMaterialGoldLine.SetActive(false);
			this.goToolGreyLine.SetActive(false);
			this.goToolGoldLine.SetActive(false);
			this.targetSlot.SetEffectHandlerState(true);
			this.toolSlot.SetEffectHandlerState(true);
			this.materialSlot.SetEffectHandlerState(true);
		}

		// Token: 0x0600711B RID: 28955 RVA: 0x00346A7F File Offset: 0x00344C7F
		private void ClearMakeData()
		{
			this._makeItemSubTypeId = -1;
			this._makeItemTypeId = -1;
			this._makeItemSubtypeIdList = null;
		}

		// Token: 0x0600711C RID: 28956 RVA: 0x00346A98 File Offset: 0x00344C98
		private int GetMakeNeedAttainment(ItemKey materialKey, out short baseRequiredAttainment, out int manualAttainment, out int perfectAttainment, out int buildingReduceAttainment)
		{
			MaterialItem materialConfig = Config.Material.Instance[materialKey.TemplateId];
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
			sbyte itemType = makeItemSubTypeConfig.Result.ItemType;
			sbyte lifeSkillType = materialConfig.RequiredLifeSkillType;
			MakeItemTypeItem makeItemType = MakeItemType.Instance[this._makeItemTypeId];
			List<short> makeItemSubTypes = makeItemType.MakeItemSubTypes;
			short makeItemSubTypeId = this._isManual ? this._makeItemSubTypeId : -1;
			bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
			bool flag = isRandomMake;
			int result;
			if (flag)
			{
				sbyte materialGrade;
				GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(materialKey.TemplateId, itemType, lifeSkillType, int.MaxValue, makeItemSubTypes, out materialGrade, out baseRequiredAttainment, this.DisplayData.AllPagesReadCookingSkillBookCount, makeItemSubTypeId, this.DisplayData.BuildingAttainmentEffect, this.IsPerfect, false, -1);
				short needAttainment = GameData.Domains.Building.SharedMethods.GetMakeRequiredLifeSkillAttainment(this._makeItemSubTypeId, this._isManual, this.IsPerfect, this.DisplayData.BuildingAttainmentEffect, (int)baseRequiredAttainment, (int)materialGrade, out manualAttainment, out perfectAttainment, out buildingReduceAttainment);
				result = (int)needAttainment;
			}
			else
			{
				bool flag2;
				if (itemType != 7)
				{
					short itemSubType = makeItemType.ItemSubType;
					flag2 = (itemSubType == 14 || itemSubType == 15);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					short manulFoodTemplateId = (this._isManual && this.targetSlot.IsValid) ? this.targetSlot.ItemData.RealKey.TemplateId : -1;
					sbyte materialGrade2;
					GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(materialKey.TemplateId, itemType, lifeSkillType, int.MaxValue, makeItemSubTypes, out materialGrade2, out baseRequiredAttainment, this.DisplayData.AllPagesReadCookingSkillBookCount, makeItemSubTypeId, this.DisplayData.BuildingAttainmentEffect, this.IsPerfect, true, manulFoodTemplateId);
					short needAttainment2 = GameData.Domains.Building.SharedMethods.GetMakeRequiredLifeSkillAttainment(this._makeItemSubTypeId, this._isManual, this.IsPerfect, this.DisplayData.BuildingAttainmentEffect, (int)baseRequiredAttainment, (int)materialGrade2, out manualAttainment, out perfectAttainment, out buildingReduceAttainment);
					result = (int)needAttainment2;
				}
				else
				{
					bool flag4 = this.CurMakeResult.MakeResultItemArray != null;
					if (flag4)
					{
						MakeResultStage resultStage = this.CurMakeResult.MakeResultItemArray.Find((MakeResultStage r) => r.TemplateId == this.targetSlot.ItemData.RealKey.TemplateId);
						baseRequiredAttainment = (short)resultStage.LifeSkillRequiredAttainment;
					}
					else
					{
						baseRequiredAttainment = 0;
					}
					manualAttainment = 0;
					perfectAttainment = 0;
					buildingReduceAttainment = 0;
					result = (int)baseRequiredAttainment;
				}
			}
			return result;
		}

		// Token: 0x0600711D RID: 28957 RVA: 0x00346CB8 File Offset: 0x00344EB8
		private void SelectMaterial(ItemDisplayData itemData, bool check = true)
		{
			this.ParentView.SetRightMask(false);
			this.materialSlot.Select(itemData, false);
			this.RefreshMakeTypeDropdown();
			this.ResetResourceCount();
			this.RefreshSubTypeToggleGroup();
			this.RefreshPerfectDropdown();
			this.RefreshToolList();
			bool flag = !this.toolSlot.IsToggleOn;
			if (flag)
			{
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
			}
			if (check)
			{
				this.CheckCondition();
			}
		}

		// Token: 0x0600711E RID: 28958 RVA: 0x00346D34 File Offset: 0x00344F34
		private void OnCancelMaterial(int index, ItemDisplayData itemDisplayData)
		{
			this.materialListScroll.ReRender();
			this.RefreshMakeTypeDropdown();
			bool flag = this.targetSlot.IsValid && MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
			if (flag)
			{
				this.ClearMakeData();
				this.targetSlot.Refresh(false);
			}
			this.RefreshSubTypeToggleGroup();
			this.ParentView.ClearToolList("");
			this.ParentView.SetRightMask(true);
			this.toolSlot.Cancel();
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.toolSlot.SetEffectHandlerState(false);
			this.materialSlot.SetEffectHandlerState(true);
			this.RefreshPerfectDropdown();
			this.goMaterialGoldLine.SetActive(false);
			this.goToolGoldLine.SetActive(false);
			this.goToolGreyLine.SetActive(false);
		}

		// Token: 0x0600711F RID: 28959 RVA: 0x00346E15 File Offset: 0x00345015
		private bool GetMaterialInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x06007120 RID: 28960 RVA: 0x00346E22 File Offset: 0x00345022
		private void MaterialSourceToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshMaterialList();
		}

		// Token: 0x06007121 RID: 28961 RVA: 0x00346E2C File Offset: 0x0034502C
		private void SelectTool(ItemDisplayData itemData, bool check = true)
		{
			bool flag = this.toolSlot.ItemData == itemData;
			if (!flag)
			{
				this.toolSlot.Select(itemData, false);
				if (check)
				{
					this.CheckCondition();
				}
				this.goToolGoldLine.SetActive(true);
				this.goToolGreyLine.SetActive(false);
			}
		}

		// Token: 0x06007122 RID: 28962 RVA: 0x00346E84 File Offset: 0x00345084
		private void OnCancelTool(int index, ItemDisplayData itemDisplayData)
		{
			this.ParentView.RerenderToolList(this.targetSlot.ItemData);
			this.CheckCondition();
			this.goToolGoldLine.SetActive(false);
			this.goToolGreyLine.SetActive(true);
			this.toolSlot.SetEffectHandlerState(true);
		}

		// Token: 0x06007123 RID: 28963 RVA: 0x00346ED7 File Offset: 0x003450D7
		private bool GetToolInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x06007124 RID: 28964 RVA: 0x00346EE4 File Offset: 0x003450E4
		private MakeCellData GetMakeCellData(ITradeableContent content, MakeCellType type)
		{
			MakeCellData data = this._makeCellDataDict.GetOrDefault(content.RealKey);
			data.Type = type;
			return data;
		}

		// Token: 0x06007125 RID: 28965 RVA: 0x00346F14 File Offset: 0x00345114
		private void RefreshTargetList()
		{
			this._targetList.Clear();
			this._missingBuildingTargetList.Clear();
			bool inGuiding = this.InGuiding;
			if (inGuiding)
			{
				MiscItem config = Misc.Instance[270];
				this.<RefreshTargetList>g__AddItem|99_0(config);
				this.targetListScroll.SetItemList(this._targetList);
			}
			else
			{
				bool flag = this._canMakeItemSubTypeList != null && this._canMakeItemSubTypeList.Count > 0;
				if (flag)
				{
					foreach (short tempId in this._canMakeItemSubTypeList)
					{
						short templateId = tempId;
						this._targetList.Add(new ItemDisplayData(new ItemKey(-1, byte.MaxValue, templateId, -1), 0)
						{
							Interactable = true
						});
					}
				}
				foreach (WeaponItem config2 in ((IEnumerable<WeaponItem>)Weapon.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config2);
				}
				foreach (ArmorItem config3 in ((IEnumerable<ArmorItem>)Armor.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config3);
				}
				foreach (AccessoryItem config4 in ((IEnumerable<AccessoryItem>)Accessory.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config4);
				}
				foreach (ClothingItem config5 in ((IEnumerable<ClothingItem>)Clothing.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config5);
				}
				foreach (CarrierItem config6 in ((IEnumerable<CarrierItem>)Carrier.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config6);
				}
				foreach (MiscItem config7 in ((IEnumerable<MiscItem>)Misc.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config7);
				}
				foreach (MedicineItem config8 in ((IEnumerable<MedicineItem>)Medicine.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config8);
				}
				foreach (FoodItem config9 in ((IEnumerable<FoodItem>)Food.Instance))
				{
					this.<RefreshTargetList>g__AddItem|99_0(config9);
				}
				this.targetListScroll.SetItemList(this._targetList);
			}
		}

		// Token: 0x06007126 RID: 28966 RVA: 0x00347240 File Offset: 0x00345440
		private bool CheckMakeTarget(ItemDisplayData targetData, short makeItemSubType, MakeItemTypeItem makeItemTypeConfig, out bool isShowMissingBuildingWarning)
		{
			isShowMissingBuildingWarning = false;
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[makeItemSubType];
			MaterialItem materialConfig = Config.Material.Instance.FirstOrDefault((MaterialItem m) => m.CraftableItemTypes.Contains(makeItemTypeConfig.TemplateId));
			sbyte skillType = this.ParentView.CurLifeSkillType;
			short lifeSkillAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
			bool isFood = targetData.RealKey.ItemType == 7;
			this._maxFinalAttainment = (int)ViewMake.GetFinalAttainment(-1, lifeSkillAttainment, skillType);
			foreach (ItemDisplayData d3 in this.ParentView.AllToolList)
			{
				short finalAttainment = ViewMake.GetFinalAttainment(d3.RealKey.TemplateId, lifeSkillAttainment, skillType);
				this._maxFinalAttainment = Math.Max(this._maxFinalAttainment, (int)finalAttainment);
			}
			sbyte itemGrade = targetData.Grade;
			List<ItemDisplayData> availableMaterialList = EasyPool.Get<List<ItemDisplayData>>();
			availableMaterialList.Clear();
			List<ItemDisplayData> upgradeMaterialList = EasyPool.Get<List<ItemDisplayData>>();
			upgradeMaterialList.Clear();
			int minNeedAttainment = int.MaxValue;
			int maxNeedAttainment = 0;
			targetData.MakeAvailableMaterialCount = 0;
			MakeCellData makeCellData = default(MakeCellData);
			makeCellData.MaterialTipData = new MakeMaterialTipData
			{
				ResourceType = materialConfig.ResourceType,
				Hardness = materialConfig.FilterHardness.ToInt(),
				ItemDataList = new List<ItemDisplayData>(),
				MinGrade = 8,
				MaxGrade = 0
			};
			using (IEnumerator<MaterialItem> enumerator2 = ((IEnumerable<MaterialItem>)Config.Material.Instance).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					MaterialItem materialItem = enumerator2.Current;
					MaterialItem curMaterialConfig = Config.Material.Instance[materialItem.TemplateId];
					bool isTypeMeet = curMaterialConfig.CraftableItemTypes.Contains(makeItemTypeConfig.TemplateId);
					bool flag = !isTypeMeet;
					if (!flag)
					{
						int targetRequirement = 0;
						bool flag2 = isFood;
						if (flag2)
						{
							for (int bookCount = 0; bookCount <= 9; bookCount++)
							{
								sbyte materialGrade;
								short num;
								short materialAttainment = GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(materialItem.TemplateId, targetData.RealKey.ItemType, skillType, int.MaxValue, makeItemTypeConfig.MakeItemSubTypes, out materialGrade, out num, bookCount, makeItemSubType, this.DisplayData.BuildingAttainmentEffect, this.IsPerfect, true, -1);
								ValueTuple<sbyte, sbyte> gradeRange = GameData.Domains.Building.SharedMethods.GetMakeResultGradeRange(materialGrade, targetData.RealKey.ItemType);
								bool isGradeMeet = itemGrade >= gradeRange.Item1 && itemGrade <= gradeRange.Item2;
								bool flag3 = !isGradeMeet;
								if (!flag3)
								{
									int stageIndex = Math.Max(0, (int)(itemGrade - gradeRange.Item1));
									short subTypeExtraLifeSkill = GameData.Domains.Building.SharedMethods.GetMakeExtraLifeSkillAttainment(makeItemSubType, true, (int)materialGrade);
									sbyte b;
									GameData.Domains.Building.SharedMethods.GetStageRequirementAndGrade(stageIndex, materialGrade, targetData.RealKey.ItemType, (int)materialAttainment, subTypeExtraLifeSkill, out b, out targetRequirement);
									minNeedAttainment = Math.Min(minNeedAttainment, targetRequirement);
									maxNeedAttainment = Math.Max(maxNeedAttainment, targetRequirement);
								}
							}
						}
						else
						{
							short num;
							sbyte materialGrade2;
							short materialAttainment2 = GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(materialItem.TemplateId, targetData.RealKey.ItemType, skillType, int.MaxValue, makeItemTypeConfig.MakeItemSubTypes, out materialGrade2, out num, 9, makeItemSubType, this.DisplayData.BuildingAttainmentEffect, this.IsPerfect, true, -1);
							ValueTuple<sbyte, sbyte> gradeRange2 = GameData.Domains.Building.SharedMethods.GetMakeResultGradeRange(materialGrade2, targetData.RealKey.ItemType);
							bool isGradeMeet2 = itemGrade >= gradeRange2.Item1 && itemGrade <= gradeRange2.Item2;
							bool flag4 = !isGradeMeet2;
							if (flag4)
							{
								continue;
							}
							int stageIndex2 = Math.Max(0, (int)(itemGrade - gradeRange2.Item1));
							short subTypeExtraLifeSkill2 = GameData.Domains.Building.SharedMethods.GetMakeExtraLifeSkillAttainment(makeItemSubType, true, (int)materialGrade2);
							sbyte b;
							GameData.Domains.Building.SharedMethods.GetStageRequirementAndGrade(stageIndex2, materialGrade2, targetData.RealKey.ItemType, (int)materialAttainment2, subTypeExtraLifeSkill2, out b, out targetRequirement);
							minNeedAttainment = Math.Min(minNeedAttainment, targetRequirement);
							maxNeedAttainment = Math.Max(maxNeedAttainment, targetRequirement);
							short groupId = ItemTemplateHelper.GetGroupId(makeItemSubTypeConfig.Result.ItemType, makeItemSubTypeConfig.Result.TemplateId);
							bool needUpgrade = itemGrade == gradeRange2.Item2 && groupId >= 0;
							ItemDisplayData materialData = this._allMaterialList.Find((ItemDisplayData d) => d.RealKey.TemplateEquals(materialItem.ItemType, materialItem.TemplateId));
							bool flag5 = materialData != null && needUpgrade && !upgradeMaterialList.Contains(materialData);
							if (flag5)
							{
								upgradeMaterialList.Add(materialData);
							}
						}
						List<ItemDisplayData> availableMaterials = (from d in this._allMaterialList
						where d.RealKey.TemplateEquals(materialItem.ItemType, materialItem.TemplateId)
						select d).ToList<ItemDisplayData>();
						ItemDisplayData itemData = new ItemDisplayData(5, materialItem.TemplateId);
						itemData.Amount = availableMaterials.Sum((ItemDisplayData d) => d.Amount);
						makeCellData.MaterialTipData.ItemDataList.Add(itemData);
						makeCellData.MaterialTipData.MinGrade = Math.Min(materialItem.Grade, makeCellData.MaterialTipData.MinGrade);
						makeCellData.MaterialTipData.MaxGrade = Math.Max(materialItem.Grade, makeCellData.MaterialTipData.MaxGrade);
						bool flag6 = isFood;
						if (flag6)
						{
							short num;
							sbyte materialGrade3;
							short materialAttainment3 = GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(materialItem.TemplateId, targetData.RealKey.ItemType, skillType, this._maxFinalAttainment, makeItemTypeConfig.MakeItemSubTypes, out materialGrade3, out num, this.DisplayData.AllPagesReadCookingSkillBookCount, makeItemSubType, this.DisplayData.BuildingAttainmentEffect, this.IsPerfect, true, -1);
							ValueTuple<sbyte, sbyte> gradeRange3 = GameData.Domains.Building.SharedMethods.GetMakeResultGradeRange(materialGrade3, targetData.RealKey.ItemType);
							bool isGradeMeet3 = itemGrade <= gradeRange3.Item2;
							bool flag7 = !isGradeMeet3;
							if (flag7)
							{
								continue;
							}
							int stageIndex3 = Math.Max(0, (int)(itemGrade - gradeRange3.Item1));
							short subTypeExtraLifeSkill3 = GameData.Domains.Building.SharedMethods.GetMakeExtraLifeSkillAttainment(makeItemSubType, true, (int)materialGrade3);
							sbyte b;
							GameData.Domains.Building.SharedMethods.GetStageRequirementAndGrade(stageIndex3, materialGrade3, targetData.RealKey.ItemType, (int)materialAttainment3, subTypeExtraLifeSkill3, out b, out targetRequirement);
							bool needUpgrade2 = itemGrade == gradeRange3.Item2;
							bool flag8 = needUpgrade2 && !upgradeMaterialList.Contains(targetData);
							if (flag8)
							{
								upgradeMaterialList.Add(targetData);
							}
						}
						bool isAttainmentMeet = this._maxFinalAttainment >= targetRequirement;
						bool flag9 = isAttainmentMeet;
						if (flag9)
						{
							foreach (ItemDisplayData materialData2 in availableMaterials)
							{
								availableMaterialList.Add(materialData2);
								targetData.MakeAvailableMaterialCount += materialData2.Amount;
							}
						}
					}
				}
			}
			targetData.MakeNeedAttainment = minNeedAttainment;
			int availableToolCount = 0;
			short finalAttainmentEmpty = ViewMake.GetFinalAttainment(this.ParentView.EmptyTool.RealKey.TemplateId, lifeSkillAttainment, skillType);
			bool isEmptyToolMeet = (int)finalAttainmentEmpty >= minNeedAttainment;
			bool flag10 = isEmptyToolMeet;
			if (flag10)
			{
				availableToolCount++;
			}
			foreach (ItemDisplayData d2 in this.ParentView.AllToolList)
			{
				short finalAttainment2 = ViewMake.GetFinalAttainment(d2.RealKey.TemplateId, lifeSkillAttainment, skillType);
				bool flag11 = (int)finalAttainment2 >= minNeedAttainment;
				if (flag11)
				{
					availableToolCount++;
				}
			}
			targetData.MakeAvailableToolCount = availableToolCount;
			bool flag12 = this.ParentView.AllToolList.Count > 0 || isEmptyToolMeet;
			if (flag12)
			{
				makeCellData.IsMeetAttainment = (targetData.MakeAvailableToolCount > 0);
				makeCellData.IsMeetTool = true;
			}
			else
			{
				CraftToolItem minToolConfig = CraftTool.Instance.First((CraftToolItem c) => c.Grade == 0 && c.RequiredLifeSkillTypes.Contains(skillType));
				short minAttainment = ViewMake.GetFinalAttainment(minToolConfig.TemplateId, lifeSkillAttainment, skillType);
				makeCellData.IsMeetAttainment = ((int)minAttainment >= minNeedAttainment);
				makeCellData.IsMeetTool = false;
			}
			int availableMaterialTemplateCount = (from d in availableMaterialList
			select d.RealKey.TemplateId).Distinct<short>().Count<short>();
			int upgradeMaterialTemplateCount = (from d in upgradeMaterialList
			select d.RealKey.TemplateId).Distinct<short>().Count<short>();
			bool flag13 = availableMaterialTemplateCount > 0 && availableMaterialTemplateCount == upgradeMaterialTemplateCount && !this.DisplayData.BuildingUpgradeMakeItem;
			if (flag13)
			{
				isShowMissingBuildingWarning = true;
			}
			bool isMissingBuilding = availableMaterialList.Intersect(upgradeMaterialList).Distinct<ItemDisplayData>().Any<ItemDisplayData>() && !this.DisplayData.BuildingUpgradeMakeItem;
			availableMaterialList.Clear();
			EasyPool.Free<List<ItemDisplayData>>(availableMaterialList);
			upgradeMaterialList.Clear();
			EasyPool.Free<List<ItemDisplayData>>(upgradeMaterialList);
			makeCellData.IsMeetMaterial = (targetData.MakeAvailableMaterialCount > 0);
			makeCellData.AttainmentTipData = new MakeAttainmentTipData
			{
				CharAttainment = (int)lifeSkillAttainment,
				LifeSkillType = skillType,
				MinNeedAttainment = minNeedAttainment,
				MaxNeedAttainment = maxNeedAttainment,
				MaxToolAttainment = this._maxFinalAttainment - (int)lifeSkillAttainment
			};
			makeCellData.ToolTipData = new MakeToolTipData
			{
				AvailableToolCount = targetData.MakeAvailableToolCount
			};
			makeCellData.IsShow = (targetData.RealKey != ItemKey.Invalid);
			this._makeCellDataDict[targetData.RealKey] = makeCellData;
			bool inGuiding = this.InGuiding;
			return inGuiding || (makeCellData.IsMeetTool && makeCellData.IsMeetMaterial && !isMissingBuilding);
		}

		// Token: 0x06007127 RID: 28967 RVA: 0x00347C28 File Offset: 0x00345E28
		private void OnItemClickTarget(ITradeableContent content, RowItemLine rowItemLine)
		{
			bool isValid = this.materialSlot.IsValid;
			if (isValid)
			{
				this.materialSlot.Cancel();
				this.materialSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
				this.ClearMaterialList();
			}
			bool isValid2 = this.toolSlot.IsValid;
			if (isValid2)
			{
				this.ParentView.ClearToolList("");
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			}
			this.SelectTarget(content as ItemDisplayData, true);
			this.materialSlot.SetEffectHandlerState(true);
			this.materialSlot.gameObject.SetActive(true);
			this.toolSlot.SetEffectHandlerState(false);
			this.toolSlot.gameObject.SetActive(true);
			this.goMaterialGoldLine.SetActive(this.materialSlot.IsValid);
			this.goToolGoldLine.SetActive(this.toolSlot.IsValid);
			this.goToolGreyLine.SetActive(!this.toolSlot.IsValid);
		}

		// Token: 0x06007128 RID: 28968 RVA: 0x00347D2C File Offset: 0x00345F2C
		private void OnItemRenderTarget(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			short tempId = content.RealKey.TemplateKey.TemplateId;
			bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(content);
			rowItemMain.SetData(content);
			bool flag = isRandomMake;
			if (flag)
			{
				string randomIconName = MakeSubPageMakeHelper.GetRandomMakeIcon(tempId);
				rowItemMain.ItemBack.SetIcon(randomIconName);
				rowItemMain.ItemBack.SetBack(-1);
				string randomMakeName = MakeSubPageMakeHelper.GetRandomMakeTypeName(tempId);
				string randomTxt = randomMakeName ?? "";
				rowItemMain.SetName(randomTxt);
			}
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(content.Interactable, true);
			rowItemLine.SetDisabled(!content.Interactable);
			bool isWarning = this._missingBuildingTargetList.Contains(content);
			bool flag2 = isWarning;
			if (flag2)
			{
				string buildingName = this._upgradeBuildingConfig.Name.SetColor(this._upgradeBuildingConfig.Color);
				string warningTip = LanguageKey.LK_Make_Need_Upgrade_Building_Tip.TrFormat(buildingName);
				rowItemMain.SetItemNotCanSelectReason(warningTip);
			}
			else
			{
				rowItemMain.HideInteractionState();
			}
			rowItemLine.SetSelected(content == this.targetSlot.ItemData);
		}

		// Token: 0x06007129 RID: 28969 RVA: 0x00347E40 File Offset: 0x00346040
		private void RefreshAllMaterialList()
		{
			this._allMaterialList.Clear();
			bool canTransferItemToWarehouse = this.DisplayData.CanTransferItemToWarehouse;
			if (canTransferItemToWarehouse)
			{
				this.<RefreshAllMaterialList>g__Add|103_0(this.DisplayData.InventoryItemList);
			}
			this.<RefreshAllMaterialList>g__Add|103_0(this.DisplayData.WarehouseItemList);
			this.<RefreshAllMaterialList>g__Add|103_0(this.DisplayData.TreasuryItemList);
		}

		// Token: 0x0600712A RID: 28970 RVA: 0x00347EA1 File Offset: 0x003460A1
		public void ShowMaterialPanel(bool isShow)
		{
			this.materialPanel.gameObject.SetActive(isShow);
		}

		// Token: 0x0600712B RID: 28971 RVA: 0x00347EB8 File Offset: 0x003460B8
		public void AutoSelectMaterial()
		{
			ItemDisplayData material = (from m in this._allMaterialList
			where m.Interactable
			orderby m.Grade
			select m).FirstOrDefault<ItemDisplayData>();
			bool flag = material == null;
			if (flag)
			{
				this.materialSlot.Cancel();
				this.ParentView.SetRightMask(true);
			}
			else
			{
				this.SelectMaterial(material, true);
			}
		}

		// Token: 0x0600712C RID: 28972 RVA: 0x00347F48 File Offset: 0x00346148
		private void RefreshMaterialList()
		{
			ItemSourceType curSourceType = this.CurMaterialItemSource;
			this._materialList.Clear();
			bool flag = !this.targetSlot.IsValid;
			if (flag)
			{
				this.materialListScroll.SetItemList(this._materialList);
			}
			else
			{
				bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
				bool inGuiding = this.InGuiding;
				bool flag2 = isRandomMake || inGuiding;
				if (flag2)
				{
					foreach (ItemDisplayData materialData in this._allMaterialList)
					{
						materialData.Interactable = true;
						bool flag3 = materialData.ItemSourceTypeEnum != curSourceType;
						if (!flag3)
						{
							bool canMake = MakeSubPageMakeHelper.CheckCanMakeTargetRandomType(this._currentSelectRandomMakeItemSubType, materialData);
							bool flag4 = (canMake && !this._materialList.Contains(materialData)) || inGuiding;
							if (flag4)
							{
								this._materialList.Add(materialData);
							}
						}
					}
					this.materialListScroll.SetItemList(this._materialList);
				}
				else
				{
					ItemDisplayData targetData = this.targetSlot.ItemData;
					MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance[this._makeItemTypeId];
					sbyte itemGrade = targetData.Grade;
					bool isFood = targetData.RealKey.ItemType == 7;
					foreach (ItemDisplayData materialData2 in this._allMaterialList)
					{
						bool isSourceTypeMeet = materialData2.ItemSourceTypeEnum == curSourceType;
						bool flag5 = materialData2.RealKey.ItemType == 12 && isSourceTypeMeet;
						if (flag5)
						{
							this._materialList.Add(materialData2);
						}
						short materialId = materialData2.RealKey.TemplateId;
						MaterialItem curMaterialConfig = Config.Material.Instance[materialId];
						bool isTypeMeet = curMaterialConfig.CraftableItemTypes.Contains(makeItemTypeConfig.TemplateId);
						sbyte materialGrade = 0;
						bool flag6 = isFood;
						short materialAttainment;
						if (flag6)
						{
							materialGrade = itemGrade;
							materialAttainment = curMaterialConfig.RequiredAttainment;
						}
						else
						{
							short makeItemSubTypeId = this._isManual ? this._makeItemSubTypeId : -1;
							short num;
							materialAttainment = GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(materialId, targetData.RealKey.ItemType, this.ParentView.CurLifeSkillType, this._maxFinalAttainment, makeItemTypeConfig.MakeItemSubTypes, out materialGrade, out num, this.DisplayData.AllPagesReadCookingSkillBookCount, makeItemSubTypeId, this.DisplayData.BuildingAttainmentEffect, this.IsPerfect, this._isManual, -1);
						}
						ValueTuple<sbyte, sbyte> gradeRange = GameData.Domains.Building.SharedMethods.GetMakeResultGradeRange(materialGrade, targetData.RealKey.ItemType);
						bool isGradeMeet = itemGrade >= gradeRange.Item1 && itemGrade <= gradeRange.Item2;
						bool flag7 = itemGrade == gradeRange.Item2 && !this.DisplayData.BuildingUpgradeMakeItem && curMaterialConfig.Transferable;
						if (flag7)
						{
							isGradeMeet = false;
						}
						bool isAttainmentMeet = this._maxFinalAttainment >= (int)materialAttainment;
						materialData2.Interactable = (isTypeMeet && isGradeMeet && isAttainmentMeet);
						bool flag8 = isSourceTypeMeet && isTypeMeet && isGradeMeet && !this._materialList.Contains(materialData2);
						if (flag8)
						{
							this._materialList.Add(materialData2);
						}
					}
					this.materialListScroll.SetItemList(this._materialList);
				}
			}
		}

		// Token: 0x0600712D RID: 28973 RVA: 0x003482B0 File Offset: 0x003464B0
		private void ClearMaterialList()
		{
			this._materialList.Clear();
			this.materialListScroll.SetEmptyContent(string.Empty);
			this.materialListScroll.SetItemList(this._materialList);
		}

		// Token: 0x0600712E RID: 28974 RVA: 0x003482E4 File Offset: 0x003464E4
		private void OnItemRenderMaterial(ITradeableContent content, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(content.Interactable, true);
			rowItemLine.SetDisabled(!content.Interactable);
			rowItemLine.SetSelected(this.materialSlot.ItemData == content);
			bool interactable = content.Interactable;
			if (interactable)
			{
				rowItemMain.HideInteractionState();
			}
			else
			{
				rowItemMain.SetInteractionStateLockText(LanguageKey.LK_Item_Operation_AttainmentNotMeet.Tr());
			}
		}

		// Token: 0x0600712F RID: 28975 RVA: 0x00348364 File Offset: 0x00346564
		private void OnItemClickMaterial(ITradeableContent content, RowItemLine rowItemLine)
		{
			bool flag = this.materialSlot.ItemData == content;
			if (flag)
			{
				this.materialSlot.Cancel();
				this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
				this.goMaterialGoldLine.SetActive(false);
				this.goToolGoldLine.SetActive(false);
				this.goToolGreyLine.SetActive(false);
			}
			else
			{
				bool isValid = this.materialSlot.IsValid;
				if (isValid)
				{
					this.materialSlot.Cancel();
				}
				this.SelectMaterial(content as ItemDisplayData, true);
				this.goMaterialGoldLine.SetActive(true);
				bool flag2 = this.toolSlot.IsToggleOn && !this.CheckTool();
				if (flag2)
				{
					this.ParentView.AutoSelectTool();
					this.<OnItemClickMaterial>g__SetNewTool|109_0();
				}
				else
				{
					bool isValid2 = this.toolSlot.IsValid;
					if (isValid2)
					{
						this.<OnItemClickMaterial>g__SetNewTool|109_0();
					}
					else
					{
						this.goToolGoldLine.SetActive(false);
						this.goToolGreyLine.SetActive(true);
					}
				}
				this.toolSlot.SetEffectHandlerState(true);
			}
		}

		// Token: 0x06007130 RID: 28976 RVA: 0x0034847C File Offset: 0x0034667C
		private void RefreshMakeTypeDropdown()
		{
			bool isTargetUnsigned = MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
			bool isShow = this.materialSlot.IsValid && isTargetUnsigned;
			bool flag = isShow;
			if (flag)
			{
				this._makeTypeList.Clear();
				this._makeTypeDict.Clear();
				MaterialItem materialConfig = Config.Material.Instance[this.materialSlot.ItemData.RealKey.TemplateId];
				List<short> craftableItemTypes = materialConfig.CraftableItemTypes;
				craftableItemTypes.ForEach(delegate(short i)
				{
					this._makeTypeList.Add(i);
					this._makeTypeDict[i] = MakeItemType.Instance[i].MakeItemSubTypes;
				});
				int index = this._makeTypeList.FindIndex(delegate(short id)
				{
					MakeItemTypeItem itemInfo = MakeItemType.Instance[id];
					return itemInfo.ItemSubType == this._currentSelectRandomMakeItemSubType;
				});
				bool flag2 = index < 0;
				if (flag2)
				{
					Debug.LogError(string.Format("Can't Find Target Index | Material: {0} RandomMake: {1}", materialConfig.Name, this._currentSelectRandomMakeItemSubType));
				}
				else
				{
					short typeId = this._makeTypeList[index];
					bool flag3 = this._makeItemTypeId != typeId;
					if (flag3)
					{
						this._makeItemTypeId = typeId;
						this._makeItemSubtypeIdList = this._makeTypeDict[this._makeItemTypeId];
						this._makeItemSubTypeId = this._makeItemSubtypeIdList.GetRandom<short>();
						this._isManual = false;
						this.targetSlot.Refresh(false);
					}
				}
			}
			else
			{
				bool flag4 = this.targetSlot.IsValid && this._makeItemTypeId >= 0;
				if (flag4)
				{
					this._makeItemSubtypeIdList = MakeItemType.Instance[this._makeItemTypeId].MakeItemSubTypes;
					bool flag5 = !this._makeItemSubtypeIdList.Contains(this._makeItemSubTypeId);
					if (flag5)
					{
						this._makeItemSubTypeId = this._makeItemSubtypeIdList.GetRandom<short>();
						this._isManual = false;
						this.targetSlot.Refresh(false);
					}
				}
			}
		}

		// Token: 0x06007131 RID: 28977 RVA: 0x00348640 File Offset: 0x00346840
		private void InitSubTypeToggleGroup()
		{
			this.subTypeToggleGroup.Clear();
			this.subTypeToggleGroup.OnActiveIndexChange -= this.SubTypeToggleGroupOnActiveIndexChange;
			this.subTypeToggleGroup.OnActiveIndexChange += this.SubTypeToggleGroupOnActiveIndexChange;
			this.subTypeToggleGroup.gameObject.SetActive(false);
		}

		// Token: 0x06007132 RID: 28978 RVA: 0x0034869C File Offset: 0x0034689C
		private void SubTypeToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex == -1;
			if (flag)
			{
				this._makeItemSubTypeId = this._makeItemSubtypeIdList.GetRandom<short>();
				this._isManual = false;
			}
			else
			{
				this._makeItemSubTypeId = this._makeItemSubtypeIdList[newIndex];
				this._isManual = true;
			}
			this.targetSlot.Refresh(false);
			this.ResetResourceCount();
			this.CheckCondition();
		}

		// Token: 0x06007133 RID: 28979 RVA: 0x00348704 File Offset: 0x00346904
		private void RefreshSubTypeToggleGroup()
		{
			int activeIndex = this.subTypeToggleGroup.GetActiveIndex();
			bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
			bool flag;
			if (this.targetSlot.IsValid && isRandomMake)
			{
				List<short> makeItemSubtypeIdList = this._makeItemSubtypeIdList;
				flag = (makeItemSubtypeIdList != null && makeItemSubtypeIdList.Count > 1);
			}
			else
			{
				flag = false;
			}
			bool isShow = flag;
			this.subTypeToggleGroup.gameObject.SetActive(isShow);
			bool flag2 = !isShow;
			if (!flag2)
			{
				this.subTypeToggleGroup.Clear();
				Transform template = this.subTypeToggleGroup.transform.GetChild(0);
				for (int i = 0; i < this._makeItemSubtypeIdList.Count; i++)
				{
					Transform child = (i < this.subTypeToggleGroup.transform.childCount) ? this.subTypeToggleGroup.transform.GetChild(i) : Object.Instantiate<Transform>(template, this.subTypeToggleGroup.transform);
					child.gameObject.SetActive(true);
					CToggle tog = child.GetComponent<CToggle>();
					TextMeshProUGUI textName = child.GetComponentInChildren<TextMeshProUGUI>();
					short subType = this._makeItemSubtypeIdList[i];
					MakeItemSubTypeItem subTypeConfig = MakeItemSubType.Instance[subType];
					textName.SetText(subTypeConfig.Name, true);
					TooltipInvoker tip = child.GetComponentInChildren<TooltipInvoker>();
					tip.Type = TipType.Simple;
					string title = LanguageKey.LK_Make_SubType_Manual.Tr();
					StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
					stringBuilder.AppendLine(subTypeConfig.Desc);
					stringBuilder.AppendLine(LanguageKey.LK_Make_SubType_Manual_Tip.Tr().SetColor("darkred"));
					string content = stringBuilder.ToString();
					tip.PresetParam = new string[]
					{
						title,
						content
					};
					this.subTypeToggleGroup.Add(tog);
				}
				for (int j = this._makeItemSubtypeIdList.Count; j < this.subTypeToggleGroup.transform.childCount; j++)
				{
					this.subTypeToggleGroup.transform.GetChild(j).gameObject.SetActive(false);
				}
				this.subTypeToggleGroup.Init(-1);
				bool flag3 = activeIndex < 0;
				if (flag3)
				{
					this.subTypeToggleGroup.DeSelect(true);
				}
				else
				{
					this.subTypeToggleGroup.Set(activeIndex, true);
				}
			}
		}

		// Token: 0x06007134 RID: 28980 RVA: 0x00348948 File Offset: 0x00346B48
		private void ResetResourceCount()
		{
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
			bool needAverage = !this._isManual && makeItemSubTypeConfig.Result.ItemType == 8 && this._makeItemSubtypeIdList.Count > 1;
			short maxMakeResourceTotalCount;
			if (!needAverage)
			{
				maxMakeResourceTotalCount = makeItemSubTypeConfig.ResourceTotalCount;
			}
			else
			{
				maxMakeResourceTotalCount = (short)this._makeItemSubtypeIdList.Average((short id) => (int)MakeItemSubType.Instance[id].ResourceTotalCount);
			}
			this._maxMakeResourceTotalCount = maxMakeResourceTotalCount;
			int maxCount = 0;
			int resourceTypeCount = 0;
			sbyte resourceType;
			sbyte resourceType2;
			for (resourceType = 0; resourceType < 6; resourceType = resourceType2 + 1)
			{
				short count = needAverage ? ((short)this._makeItemSubtypeIdList.Average((short id) => (int)MakeItemSubType.Instance[id].MaxMaterialResources.Get((int)resourceType))) : makeItemSubTypeConfig.MaxMaterialResources.Get((int)resourceType);
				bool flag = (int)count > maxCount;
				if (flag)
				{
					this._mainRequiredResourceType = resourceType;
					maxCount = (int)count;
				}
				bool flag2 = count > 0;
				if (flag2)
				{
					resourceTypeCount++;
				}
				this._maxMakeResourceCountInts.Set((int)resourceType, (int)count);
				resourceType2 = resourceType;
			}
			bool flag3 = resourceTypeCount == 1;
			if (flag3)
			{
				this._curMakeResourceCountInts.Initialize();
				this._curMakeResourceCountInts.Add(ref this._maxMakeResourceCountInts);
				this._lastMakeResourceCountInts.Initialize();
				this._lastMakeResourceCountInts.Add(ref this._maxMakeResourceCountInts);
			}
			else
			{
				this._curMakeResourceCountInts.Initialize();
				this._curMakeResourceCountInts.Add(ref this._lastMakeResourceCountInts);
				this._lastMakeResourceCountInts.Initialize();
				this._lastMakeResourceCountInts.Add(ref this._curMakeResourceCountInts);
			}
		}

		// Token: 0x06007135 RID: 28981 RVA: 0x00348B0C File Offset: 0x00346D0C
		private void InitResourcePanel()
		{
			this._lastMakeResourceCountInts.Initialize();
			this._curMakeResourceCountInts.Initialize();
			for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
			{
				MakeResourceItem item = this.resourceItemArray[(int)resourceType];
				item.Init(resourceType, new Action<sbyte, int, bool>(this.OnResourceCountChanged));
			}
		}

		// Token: 0x06007136 RID: 28982 RVA: 0x00348B62 File Offset: 0x00346D62
		private void OnResourceCountChanged(sbyte resourceType, int count, bool isAdd = false)
		{
			count = Math.Clamp(count, 0, this._maxMakeResourceCountInts.Get((int)resourceType));
			this._curMakeResourceCountInts.Set((int)resourceType, count);
			this._lastMakeResourceCountInts.Set((int)resourceType, count);
			this.CheckCondition();
		}

		// Token: 0x06007137 RID: 28983 RVA: 0x00348BA0 File Offset: 0x00346DA0
		private bool RefreshResourcePanel()
		{
			bool isShow = this.materialSlot.IsValid && this.toolSlot.IsValid;
			this.resourcePanel.SetActive(isShow);
			this.imageResourceCountProgress.gameObject.SetActive(isShow);
			bool flag = !isShow;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short materialId = this.materialSlot.ItemData.RealKey.TemplateId;
				int totalCount = this._curMakeResourceCountInts.GetSum();
				int remainCount = (int)this._maxMakeResourceTotalCount - totalCount;
				bool isAllMeet = true;
				this._makeRequiredResourceInts.Initialize();
				for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
				{
					MakeResourceItem item = this.resourceItemArray[(int)resourceType];
					int curCount = this._curMakeResourceCountInts.Get((int)resourceType);
					int maxCount = this._maxMakeResourceCountInts.Get((int)resourceType);
					bool isShowItem = maxCount > 0;
					item.gameObject.SetActive(isShowItem);
					bool flag2 = !isShowItem;
					if (!flag2)
					{
						short unitAmount = ItemTemplateHelper.GetCraftMaterialRequiredResourceAmount(materialId);
						int needAmount = curCount * (int)this._makeCount * (int)unitAmount;
						this._makeRequiredResourceInts.Set((int)resourceType, needAmount);
						bool isMain = this._mainRequiredResourceType == resourceType;
						bool isMeet = item.Refresh(this._makeItemSubTypeId, isMain, needAmount, curCount, maxCount, remainCount);
						bool flag3 = !isMeet;
						if (flag3)
						{
							isAllMeet = false;
						}
					}
				}
				this.imageResourceCountProgress.fillAmount = (float)totalCount / (float)this._maxMakeResourceTotalCount;
				result = isAllMeet;
			}
			return result;
		}

		// Token: 0x06007138 RID: 28984 RVA: 0x00348D18 File Offset: 0x00346F18
		private void InitMakeCount()
		{
			this.sliderMakeCount.onValueChanged.RemoveAllListeners();
			this.sliderMakeCount.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderMakeCountValueChanged));
			this.buttonLessMakeCount.ClearAndAddListener(new Action(this.OnClickButtonLessMakeCount));
			this.buttonMoreMakeCount.ClearAndAddListener(new Action(this.OnClickButtonMoreMakeCount));
		}

		// Token: 0x06007139 RID: 28985 RVA: 0x00348D84 File Offset: 0x00346F84
		private void OnSliderMakeCountValueChanged(float value)
		{
			this._makeCount = (short)value;
			this.OnMakeCountChanged();
		}

		// Token: 0x0600713A RID: 28986 RVA: 0x00348D96 File Offset: 0x00346F96
		private void OnClickButtonLessMakeCount()
		{
			this._makeCount -= 1;
			this.OnMakeCountChanged();
		}

		// Token: 0x0600713B RID: 28987 RVA: 0x00348DAF File Offset: 0x00346FAF
		private void OnClickButtonMoreMakeCount()
		{
			this._makeCount += 1;
			this.OnMakeCountChanged();
		}

		// Token: 0x0600713C RID: 28988 RVA: 0x00348DC8 File Offset: 0x00346FC8
		private void OnMakeCountChanged()
		{
			this.RefreshToolList();
			this.CheckCondition();
		}

		// Token: 0x0600713D RID: 28989 RVA: 0x00348DDC File Offset: 0x00346FDC
		private void RefreshToolList()
		{
			bool flag = !this.materialSlot.IsValid;
			if (!flag)
			{
				short num;
				int num2;
				int num3;
				int num4;
				int needAttainment = this.GetMakeNeedAttainment(this.materialSlot.ItemData.RealKey, out num, out num2, out num3, out num4);
				this.ParentView.RefreshToolList(needAttainment, new List<sbyte>
				{
					this.ParentView.CurLifeSkillType
				}, new List<List<sbyte>>
				{
					new List<sbyte>
					{
						this.materialSlot.ItemData.Grade
					}
				}, this.toolSlot.ItemData, delegate(ItemDisplayData tool)
				{
					this.SelectTool(tool, true);
				}, true, (int)this._makeCount);
			}
		}

		// Token: 0x0600713E RID: 28990 RVA: 0x00348E8C File Offset: 0x0034708C
		private void RefreshMakeCount()
		{
			bool isShow = this.materialSlot.IsValid && this.toolSlot.IsValid;
			this.rootMakeCount.SetActive(isShow);
			bool flag = isShow;
			if (flag)
			{
				int materialAmount = this.materialSlot.ItemData.Amount;
				bool flag2 = !ViewMake.IsEmptyTool(this.toolSlot.ItemData) && this._makeToolDurabilityCost > 0;
				if (flag2)
				{
					int count = ((int)this.toolSlot.ItemData.Durability % this._makeToolDurabilityCost == 0) ? ((int)this.toolSlot.ItemData.Durability / this._makeToolDurabilityCost) : ((int)this.toolSlot.ItemData.Durability / this._makeToolDurabilityCost + 1);
					this._maxMakeCount = Mathf.Max(1, count);
					this._maxMakeCount = Mathf.Min(materialAmount, this._maxMakeCount);
				}
				else
				{
					this._maxMakeCount = Mathf.Min(materialAmount, int.MaxValue);
				}
				short makeCount = (short)Mathf.Clamp((int)this._makeCount, 1, this._maxMakeCount);
				this.sliderMakeCount.wholeNumbers = true;
				this.sliderMakeCount.minValue = 1f;
				this.sliderMakeCount.maxValue = (float)this._maxMakeCount;
				this.sliderMakeCount.value = (float)makeCount;
				this.sliderMakeCount.interactable = (this._maxMakeCount > 1);
				this.buttonLessMakeCount.interactable = (this._makeCount > 1);
				this.buttonMoreMakeCount.interactable = ((int)this._makeCount < this._maxMakeCount);
			}
			else
			{
				this._makeCount = 1;
			}
			this.textConfirm.text = LanguageKey.LK_Make_Confirm.TrFormat(this._makeCount);
		}

		// Token: 0x0600713F RID: 28991 RVA: 0x0034904D File Offset: 0x0034724D
		private void CheckCondition()
		{
			this.RefreshResourcePanel();
			this.RefreshMakeCount();
			this.RefreshStorageDropdown();
			this.RefreshMakeResult();
		}

		// Token: 0x06007140 RID: 28992 RVA: 0x0034906C File Offset: 0x0034726C
		private void RefreshPanel()
		{
			bool isSkillMeet = this.CheckSkillAttainment();
			bool isToolMeet = this.CheckTool();
			bool isResourceMeet = this.RefreshResourcePanel();
			bool isResourceCountMeet = this._curMakeResourceCountInts.GetSum() == (int)this._maxMakeResourceTotalCount;
			this.RefreshButtonConfirm(isToolMeet, isSkillMeet, isResourceMeet, isResourceCountMeet);
			bool hasTarget = this.targetSlot.IsValid;
			this.ShowMaterialPanel(hasTarget);
			this.tipNoTarget.SetActive(!hasTarget);
			this.ParentView.ShowToolPanel(hasTarget);
		}

		// Token: 0x06007141 RID: 28993 RVA: 0x003490E4 File Offset: 0x003472E4
		private bool CheckSkillAttainment()
		{
			bool isSkillMeet = false;
			sbyte skillType = this.ParentView.CurLifeSkillType;
			LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[skillType];
			short charAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
			bool hasMaterial = this.materialSlot.IsValid;
			bool hasSignedTarget = this.targetSlot.IsValid && !MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
			bool flag = hasMaterial;
			if (flag)
			{
				short baseRequiredAttainment;
				int manualAttainment;
				int perfectAttainment;
				int buildingReduceAttainment;
				int needAttainment = this.GetMakeNeedAttainment(this.materialSlot.ItemData.RealKey, out baseRequiredAttainment, out manualAttainment, out perfectAttainment, out buildingReduceAttainment);
				short finalAttainment = this.toolSlot.IsValid ? ViewMake.GetFinalAttainment(this.toolSlot.ItemData.RealKey.TemplateId, charAttainment, skillType) : charAttainment;
				isSkillMeet = ((int)finalAttainment >= needAttainment);
				string finalAttainmentColor = isSkillMeet ? "brightblue" : "brightred";
				string attainmentStr = string.Format("{0}/{1}", finalAttainment.ToString().SetColor(finalAttainmentColor), needAttainment);
				string tile = LanguageKey.LK_Make_Need_Attainment_Tip_Title.Tr();
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				stringBuilder.Clear();
				stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Total.TrFormat(needAttainment));
				stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Base.TrFormat(baseRequiredAttainment));
				stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Manual.TrFormat(manualAttainment));
				stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Perfect.TrFormat(perfectAttainment));
				stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Building.TrFormat(buildingReduceAttainment));
				string content = stringBuilder.ToString();
				EasyPool.Free<StringBuilder>(stringBuilder);
			}
			else
			{
				bool flag2 = hasSignedTarget;
				if (flag2)
				{
					MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance[this._makeItemTypeId];
					bool isFood = this.targetSlot.ItemData.RealKey.ItemType == 7;
					List<ValueTuple<short, sbyte>> materialInfoList = (from m in Config.Material.Instance
					where m.CraftableItemTypes.Contains(this._makeItemTypeId)
					select m).Select(delegate(MaterialItem m)
					{
						bool isFood = isFood;
						ValueTuple<short, sbyte> result;
						if (isFood)
						{
							result = new ValueTuple<short, sbyte>(m.RequiredAttainment, m.Grade);
						}
						else
						{
							short makeItemSubTypeId = this._isManual ? this._makeItemSubTypeId : -1;
							sbyte materialFinalGrade;
							short num;
							short attainment = GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(m.TemplateId, this.targetSlot.ItemData.RealKey.ItemType, this.ParentView.CurLifeSkillType, this._maxFinalAttainment, makeItemTypeConfig.MakeItemSubTypes, out materialFinalGrade, out num, this.DisplayData.AllPagesReadCookingSkillBookCount, makeItemSubTypeId, this.DisplayData.BuildingAttainmentEffect, this._isManual, false, -1);
							result = new ValueTuple<short, sbyte>(attainment, materialFinalGrade);
						}
						return result;
					}).ToList<ValueTuple<short, sbyte>>();
					short minAttainment = materialInfoList.Min((ValueTuple<short, sbyte> d) => d.Item1);
					short maxAttainment = materialInfoList.Max((ValueTuple<short, sbyte> d) => d.Item1);
					string attainmentStr2 = string.Format("{0}~{1}", minAttainment, maxAttainment);
					sbyte minGrade = materialInfoList.Min((ValueTuple<short, sbyte> d) => d.Item2);
					sbyte maxGrade = materialInfoList.Max((ValueTuple<short, sbyte> d) => d.Item2);
					string gradeStr = CommonUtils.GetShortGradeText((int)minGrade, true) + "~" + CommonUtils.GetShortGradeText((int)maxGrade, true);
					MaterialItem materialConfig = Config.Material.Instance.First((MaterialItem m) => m.CraftableItemTypes.Contains(this._makeItemTypeId));
					ResourceTypeItem resourceTypeConfig = Config.ResourceType.Instance[materialConfig.ResourceType];
					bool flag3 = materialConfig.FilterHardness > EMaterialFilterHardness.Invalid;
					if (flag3)
					{
						LanguageKey hardnessKey = LanguageKey.LK_CommonSortAndFilter_Material_FilterHardness_0 + materialConfig.FilterHardness.ToInt();
						string resourceStr = resourceTypeConfig.Name + "-" + hardnessKey.Tr();
					}
					else
					{
						bool flag4 = materialConfig.FilterType > EMaterialFilterType.Invalid;
						if (flag4)
						{
							LanguageKey hardnessKey2 = LanguageKey.LK_CommonSortAndFilter_Material_Filter_FilterType_0 + materialConfig.FilterType.ToInt();
							string resourceStr = resourceTypeConfig.Name + "-" + hardnessKey2.Tr();
						}
						else
						{
							string text = resourceTypeConfig.Name ?? "";
						}
					}
				}
			}
			return isSkillMeet;
		}

		// Token: 0x06007142 RID: 28994 RVA: 0x003494C0 File Offset: 0x003476C0
		private bool CheckTool()
		{
			bool isToolMeet = false;
			bool hasTool = this.toolSlot.IsValid;
			bool flag = hasTool;
			if (flag)
			{
				short oneCost = ViewMake.GetToolDurabilityCost(this.toolSlot.ItemData, this.materialSlot.ItemData.Grade);
				this._makeToolDurabilityCost = (int)oneCost;
				short totalCost = Convert.ToInt16((int)(oneCost * this._makeCount));
				short curDurability = this.toolSlot.ItemData.Durability;
				short maxDurability = this.toolSlot.ItemData.MaxDurability;
				isToolMeet = (curDurability >= totalCost || curDurability + oneCost > totalCost);
				string durabilityStr = (this.toolSlot.IsValid && ViewMake.IsEmptyTool(this.toolSlot.ItemData)) ? "-" : string.Format("{0}-{1}/{2}", curDurability, totalCost.ToString().SetColor("brightred"), maxDurability);
				this.propertyToolDurability.SetValue(durabilityStr);
				sbyte skillType = this.ParentView.CurLifeSkillType;
				LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[skillType];
				short lifeSkillAttainment = this.DisplayData.LifeSkillAttainments.Get((int)skillType);
				bool hasMaterial = this.materialSlot.IsValid;
				bool isNotRandomMake = this.targetSlot.IsValid && !MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
				short finalAttainment = this.toolSlot.IsValid ? ViewMake.GetFinalAttainment(this.toolSlot.ItemData.RealKey.TemplateId, lifeSkillAttainment, skillType) : lifeSkillAttainment;
				bool flag2 = hasMaterial;
				if (flag2)
				{
					short baseRequiredAttainment;
					int manualAttainment;
					int perfectAttainment;
					int buildingReduceAttainment;
					int needAttainment = this.GetMakeNeedAttainment(this.materialSlot.ItemData.RealKey, out baseRequiredAttainment, out manualAttainment, out perfectAttainment, out buildingReduceAttainment);
					string finalAttainmentColor = ((int)finalAttainment >= needAttainment) ? "brightblue" : "brightred";
					string attainmentStr = string.Format("{0}/{1}", finalAttainment.ToString().SetColor(finalAttainmentColor), needAttainment);
					this.propertyToolAttainment.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, attainmentStr, null, false);
					TooltipInvoker tip = this.propertyToolAttainment.Tip;
					tip.Type = TipType.Simple;
					tip.enabled = true;
					string tile = LanguageKey.LK_Make_Need_Attainment_Tip_Title.Tr();
					StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
					stringBuilder.Clear();
					stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Total.TrFormat(needAttainment));
					stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Base.TrFormat(baseRequiredAttainment));
					stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Manual.TrFormat(manualAttainment));
					stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Perfect.TrFormat(perfectAttainment));
					stringBuilder.AppendLine(LanguageKey.LK_Make_Need_Attainment_Tip_Building.TrFormat(buildingReduceAttainment));
					string content = stringBuilder.ToString();
					EasyPool.Free<StringBuilder>(stringBuilder);
					tip.PresetParam = new string[]
					{
						tile,
						content
					};
				}
				else
				{
					bool flag3 = isNotRandomMake;
					if (flag3)
					{
						MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance[this._makeItemTypeId];
						bool isFood = this.targetSlot.ItemData.RealKey.ItemType == 7;
						List<short> materialInfoList = (from m in Config.Material.Instance
						where m.CraftableItemTypes.Contains(this._makeItemTypeId)
						select m).Select(delegate(MaterialItem m)
						{
							bool isFood = isFood;
							short result;
							if (isFood)
							{
								result = m.RequiredAttainment;
							}
							else
							{
								short makeItemSubTypeId = this._isManual ? this._makeItemSubTypeId : -1;
								sbyte b;
								short num;
								short attainment = GameData.Domains.Building.SharedMethods.GetMaterialGradeAndAttainment(m.TemplateId, this.targetSlot.ItemData.RealKey.ItemType, this.ParentView.CurLifeSkillType, this._maxFinalAttainment, makeItemTypeConfig.MakeItemSubTypes, out b, out num, this.DisplayData.AllPagesReadCookingSkillBookCount, makeItemSubTypeId, this.DisplayData.BuildingAttainmentEffect, this._isManual, false, -1);
								result = attainment;
							}
							return result;
						}).ToList<short>();
						short minAttainment = materialInfoList.Min((short d) => d);
						short maxAttainment = materialInfoList.Max((short d) => d);
						string needAttainmentStr = string.Format("{0}~{1}", minAttainment, maxAttainment);
						string finalAttainmentColor = (finalAttainment >= minAttainment) ? "brightblue" : "brightred";
						string attainmentStr2 = finalAttainment.ToString().SetColor(finalAttainmentColor) + "/" + needAttainmentStr;
						this.propertyToolAttainment.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, attainmentStr2, null, false);
					}
				}
			}
			this.propertyToolDurability.gameObject.SetActive(hasTool);
			this.propertyToolAttainment.gameObject.SetActive(hasTool);
			return isToolMeet;
		}

		// Token: 0x06007143 RID: 28995 RVA: 0x0034993C File Offset: 0x00347B3C
		private void RefreshButtonConfirm(bool isToolMeet, bool isSkillMeet, bool isResourceMeet, bool isResourceCountMeet)
		{
			bool hasMaterial = this.materialSlot.IsValid;
			bool hasTool = this.toolSlot.IsValid;
			this.buttonConfirm.interactable = (isToolMeet && isSkillMeet && isResourceMeet && isResourceCountMeet);
			this.tipConfirm.enabled = !this.buttonConfirm.interactable;
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			bool flag = !hasMaterial;
			if (flag)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Make_Material_Not_Selected.Tr().ColorReplace());
			}
			else
			{
				bool flag2 = !hasTool;
				if (flag2)
				{
					stringBuilder.AppendLine(LanguageKey.LK_Making_Tool_Not_Selected.Tr().ColorReplace());
				}
				else
				{
					bool flag3 = !isToolMeet;
					if (flag3)
					{
						stringBuilder.AppendLine(LanguageKey.LK_Making_Tool_Durability_Not_Enough.Tr().ColorReplace());
					}
					bool flag4 = !isSkillMeet;
					if (flag4)
					{
						stringBuilder.AppendLine(LanguageKey.LK_Making_Attainment_Not_Enough.Tr().ColorReplace());
					}
					bool flag5 = !isResourceMeet;
					if (flag5)
					{
						stringBuilder.AppendLine(LanguageKey.LK_Making_Resource_Not_Enough.Tr().ColorReplace());
					}
					else
					{
						bool flag6 = !isResourceCountMeet;
						if (flag6)
						{
							stringBuilder.AppendLine(LanguageKey.LK_Making_Resource_Count_Not_Match.Tr().ColorReplace());
						}
					}
				}
			}
			this.tipConfirm.PresetParam = new string[]
			{
				stringBuilder.ToString()
			};
			stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x06007144 RID: 28996 RVA: 0x00349A94 File Offset: 0x00347C94
		private void OnClickButtonConfirm()
		{
			this.buttonConfirm.interactable = false;
			this.collectResource.Clear();
			List<short> resultTemplateIdList = new List<short>((int)this._makeCount);
			bool isTargetUnsigned = MakeSubPageMakeHelper.CheckIsRandomMake(this.targetSlot.ItemData);
			for (int i = 0; i < (int)this._makeCount; i++)
			{
				bool flag = isTargetUnsigned;
				if (flag)
				{
					short id = (this.CurMakeResult.TargetResultStage.TemplateId >= 0) ? this.CurMakeResult.TargetResultStage.TemplateId : this.CurMakeResult.TargetResultStage.TemplateIdList.GetRandom<short>();
					resultTemplateIdList.Add(id);
				}
				else
				{
					resultTemplateIdList.Add(this.targetSlot.ItemData.RealKey.TemplateId);
				}
			}
			short manulFoodTemplateId = (this._isManual && this.targetSlot.ItemData.RealKey.ItemType == 7 && this.targetSlot.IsValid) ? this.targetSlot.ItemData.RealKey.TemplateId : -1;
			MakeConditionArguments makeConditionArguments = new MakeConditionArguments
			{
				BuildingBlockKey = this.ParentView.BuildingBlockKey,
				CharId = this.ParentView.TaiwuCharId,
				IsManual = this._isManual,
				MakeCount = this._makeCount,
				MakeItemSubTypeId = this._makeItemSubTypeId,
				MakeItemTypeId = this._makeItemTypeId,
				MaterialKey = this.materialSlot.ItemData.RealKey,
				ResourceCount = this._curMakeResourceCountInts,
				ToolKey = this.toolSlot.ItemData.RealKey,
				IsPerfect = this.IsPerfect,
				ManulFoodTemplateId = manulFoodTemplateId
			};
			BuildingDomainMethod.AsyncCall.CheckMakeCondition(this.ParentView, makeConditionArguments, delegate(int offset, RawDataPool dataPool)
			{
				bool isMeet = false;
				Serializer.Deserialize(dataPool, offset, ref isMeet);
				bool flag2 = !isMeet;
				if (flag2)
				{
					UIElement.FullScreenMask.Hide(false);
				}
				else
				{
					base.<OnClickButtonConfirm>g__Action|1();
				}
			});
		}

		// Token: 0x06007145 RID: 28997 RVA: 0x00349CAB File Offset: 0x00347EAB
		private void OnCancelSelectTarget()
		{
			this.targetPanel.gameObject.SetActive(false);
			this.ParentView.ExitFocusMode();
		}

		// Token: 0x06007146 RID: 28998 RVA: 0x00349CCC File Offset: 0x00347ECC
		private void InitStorageDropdown()
		{
			bool canShowTreasury = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			this.storageDropdown.onValueChanged.RemoveAllListeners();
			this.storageDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnStorageDropdownValueChanged));
			this.storageDropdown.ClearOptions();
			int count = canShowTreasury ? MakeSubPageMake.StorageNameKeys.Length : 2;
			List<string> list = (from k in MakeSubPageMake.StorageNameKeys
			select k.Tr()).Take(count).ToList<string>();
			this.storageDropdown.AddOptions(list);
		}

		// Token: 0x06007147 RID: 28999 RVA: 0x00349D71 File Offset: 0x00347F71
		private void OnStorageDropdownValueChanged(int value)
		{
			BuildingDomainMethod.Call.SetStoreLocation(-1, value);
			this.DisplayData.StoreLocation = value;
		}

		// Token: 0x06007148 RID: 29000 RVA: 0x00349D88 File Offset: 0x00347F88
		private void RefreshStorageDropdown()
		{
			bool isShow = this.materialSlot.IsValid && Config.Material.Instance[this.materialSlot.ItemData.RealKey.TemplateId].Transferable && this.toolSlot.IsValid;
			this.storageDropdown.gameObject.SetActive(isShow);
			this.storageDropdown.SetValueWithoutNotify(this.DisplayData.StoreLocation);
		}

		// Token: 0x06007149 RID: 29001 RVA: 0x00349E04 File Offset: 0x00348004
		private void RefreshMakeResult()
		{
			MakeSubPageMake.<>c__DisplayClass137_0 CS$<>8__locals1 = new MakeSubPageMake.<>c__DisplayClass137_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._makeItemSubtypeIdList == null || !this.materialSlot.IsValid;
			if (flag)
			{
				this.RefreshPanel();
			}
			else
			{
				MakeSubPageMake.<>c__DisplayClass137_0 CS$<>8__locals2 = CS$<>8__locals1;
				ItemDisplayData itemData = this.toolSlot.ItemData;
				CS$<>8__locals2.toolKey = ((itemData != null) ? itemData.Key : ItemKey.Invalid);
				this._makeResultDict.Clear();
				CS$<>8__locals1.curSendCount = 0;
				CS$<>8__locals1.allSendCount = ((this._makeItemSubtypeIdList.Count > 1) ? (this._makeItemSubtypeIdList.Count + 1) : 1);
				CS$<>8__locals1.materialId = this.materialSlot.ItemData.RealKey.TemplateId;
				bool flag2 = this._makeItemSubtypeIdList.Count > 1;
				if (flag2)
				{
					foreach (short makeItemSubtypeId in this._makeItemSubtypeIdList)
					{
						CS$<>8__locals1.<RefreshMakeResult>g__Send|0(makeItemSubtypeId, true);
					}
				}
				CS$<>8__locals1.<RefreshMakeResult>g__Send|0(-1, false);
			}
		}

		// Token: 0x0600714A RID: 29002 RVA: 0x00349F28 File Offset: 0x00348128
		private void Tutorial(List<ItemDisplayData> itemDataList)
		{
			bool inGuiding = this.InGuiding;
			if (inGuiding)
			{
				bool flag = itemDataList == null;
				if (!flag)
				{
					foreach (ItemDisplayData itemDisplayData in itemDataList)
					{
						bool flag2 = itemDisplayData.Key.ItemType == 12 && itemDisplayData.Key.TemplateId == 270;
						if (flag2)
						{
							TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("MakeSystemShowed", "MadeBambooThorn", true);
							TaiwuEventDomainMethod.Call.TriggerListener("MakeSystemShowed", true);
						}
					}
				}
			}
		}

		// Token: 0x0600714B RID: 29003 RVA: 0x00349FD8 File Offset: 0x003481D8
		private bool GetShowPerfectToggle()
		{
			bool flag = this._makeItemSubTypeId >= 0;
			if (flag)
			{
				MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
				bool flag2 = ItemType.IsEquipmentEffectType(makeItemSubTypeConfig.Result.ItemType);
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600714C RID: 29004 RVA: 0x0034A026 File Offset: 0x00348226
		private void OnMakePerfectToggleValueChanged(bool isOn)
		{
			this.RefreshPerfectDropdown();
			this.RefreshMaterialList();
			this.RefreshToolList();
			this.CheckCondition();
		}

		// Token: 0x0600714D RID: 29005 RVA: 0x0034A048 File Offset: 0x00348248
		private void RefreshPerfectDropdown()
		{
			bool isShow = this.IsPerfect && this.targetSlot.IsValid && this._makeItemSubTypeId >= 0 && ItemType.IsEquipmentEffectType(MakeItemSubType.Instance[this._makeItemSubTypeId].Result.ItemType);
			this.RefreshPerfectDropdown(isShow);
		}

		// Token: 0x0600714E RID: 29006 RVA: 0x0034A0A0 File Offset: 0x003482A0
		private void RefreshPerfectDropdown(bool isShow)
		{
			isShow = (isShow && this._makeItemSubTypeId >= 0);
			this.perfectDropdown.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
				sbyte itemType = makeItemSubTypeConfig.Result.ItemType;
				this._perfectEffectIdList.ClearAndAddRange(from e in EquipmentEffect.Instance.Where(delegate(EquipmentEffectItem e)
				{
					bool special = e.Special;
					bool result;
					if (special)
					{
						result = false;
					}
					else
					{
						sbyte type = e.Type;
						if (!true)
						{
						}
						bool flag2;
						switch (type)
						{
						case 0:
							flag2 = (itemType == 0 || itemType == 1 || itemType == 2);
							break;
						case 1:
							flag2 = (itemType == 0);
							break;
						case 2:
							flag2 = (itemType == 1);
							break;
						default:
							flag2 = false;
							break;
						}
						if (!true)
						{
						}
						bool isTypeMeet = flag2;
						result = isTypeMeet;
					}
					return result;
				})
				select e.TemplateId);
				this.perfectDropdown.ClearOptions();
				List<string> optionList = (from id in this._perfectEffectIdList
				select EquipmentEffect.Instance[id].Name).ToList<string>();
				this.perfectDropdown.AddOptions(optionList);
				this.perfectDropdown.SetValueWithoutNotify(0);
				this.OnPerfectDropdownValueChanged(0);
			}
		}

		// Token: 0x0600714F RID: 29007 RVA: 0x0034A1B0 File Offset: 0x003483B0
		private void OnPerfectDropdownValueChanged(int value)
		{
			bool flag = !this._perfectEffectIdList.CheckIndex(value);
			if (!flag)
			{
				short effectId = this._perfectEffectIdList[value];
				EquipmentEffectItem config = EquipmentEffect.Instance[effectId];
				this.tipPerfectDropdown.Type = TipType.Simple;
				this.tipPerfectDropdown.PresetParam = new string[]
				{
					config.Name,
					config.Desc
				};
			}
		}

		// Token: 0x06007150 RID: 29008 RVA: 0x0034A21C File Offset: 0x0034841C
		private void OnPerfectDropdownItemAdded(int index, RectTransform rectTransform)
		{
			bool flag = !this._perfectEffectIdList.CheckIndex(index);
			if (!flag)
			{
				short effectId = this._perfectEffectIdList[index];
				EquipmentEffectItem config = EquipmentEffect.Instance[effectId];
				TooltipInvoker tip = rectTransform.GetComponent<TooltipInvoker>();
				tip.Type = TipType.Simple;
				tip.PresetParam = new string[]
				{
					config.Name,
					config.Desc
				};
			}
		}

		// Token: 0x06007151 RID: 29009 RVA: 0x0034A284 File Offset: 0x00348484
		public MakeSubPageMake()
		{
			LanguageKey[] array = new LanguageKey[4];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.84B640E47BFF9FE0EB74EA8FD0FD33F0682C7D43BE6B2FDD6EB31A3448F07E2C).FieldHandle);
			this._descKeys = array;
			base..ctor();
		}

		// Token: 0x06007152 RID: 29010 RVA: 0x0034A357 File Offset: 0x00348557
		// Note: this type is marked as 'beforefieldinit'.
		static MakeSubPageMake()
		{
			LanguageKey[] array = new LanguageKey[4];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.BC3B57D4FF2F5BD824816205FB98E948A047DBFB9AC78738401C4CAF6712959A).FieldHandle);
			MakeSubPageMake.StorageNameKeys = array;
		}

		// Token: 0x06007158 RID: 29016 RVA: 0x0034A47C File Offset: 0x0034867C
		[CompilerGenerated]
		internal static void <GenerateMaterialListRowTemplateContainers>g__CreateCellContainers|79_0(Transform containerRoot, RowCellContainer prefab)
		{
			RowCellContainer container = Object.Instantiate<RowCellContainer>(prefab, containerRoot);
			container.gameObject.SetActive(true);
		}

		// Token: 0x0600715E RID: 29022 RVA: 0x0034A548 File Offset: 0x00348748
		[CompilerGenerated]
		private void <RefreshTargetList>g__AddItem|99_0(IItemConfig itemConfig)
		{
			sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemConfig.ItemType, itemConfig.TemplateId);
			bool flag = lifeSkillType != this.ParentView.CurLifeSkillType;
			if (!flag)
			{
				short makeItemSubType = ItemTemplateHelper.GetMakeItemSubType(itemConfig.ItemType, itemConfig.TemplateId);
				bool flag2 = makeItemSubType < 0;
				if (!flag2)
				{
					MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance.FirstOrDefault((MakeItemTypeItem m) => m.MakeItemSubTypes.Contains(makeItemSubType));
					bool flag3 = makeItemTypeConfig == null;
					if (!flag3)
					{
						ItemDisplayData itemData = new ItemDisplayData(itemConfig.ItemType, itemConfig.TemplateId);
						bool isShowMissingBuildingWarning;
						itemData.Interactable = this.CheckMakeTarget(itemData, makeItemSubType, makeItemTypeConfig, out isShowMissingBuildingWarning);
						this._targetList.Add(itemData);
						bool flag4 = isShowMissingBuildingWarning;
						if (flag4)
						{
							this._missingBuildingTargetList.Add(itemData);
						}
					}
				}
			}
		}

		// Token: 0x0600715F RID: 29023 RVA: 0x0034A620 File Offset: 0x00348820
		[CompilerGenerated]
		private void <RefreshAllMaterialList>g__Add|103_0(List<ItemDisplayData> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				foreach (ItemDisplayData itemData in list)
				{
					sbyte itemType = itemData.RealKey.ItemType;
					sbyte b = itemType;
					if (b == 5)
					{
						MaterialItem config = Config.Material.Instance[itemData.RealKey.TemplateId];
						bool flag2 = config.RequiredLifeSkillType == this.ParentView.CurLifeSkillType && config.CraftableItemTypes.Count > 0;
						if (flag2)
						{
							this._allMaterialList.Add(itemData);
						}
					}
				}
			}
		}

		// Token: 0x06007160 RID: 29024 RVA: 0x0034A6E4 File Offset: 0x003488E4
		[CompilerGenerated]
		private void <OnItemClickMaterial>g__SetNewTool|109_0()
		{
			this.goToolGoldLine.SetActive(true);
			this.goToolGreyLine.SetActive(false);
		}

		// Token: 0x040053F2 RID: 21490
		[SerializeField]
		private MakeTargetSlot materialSlot;

		// Token: 0x040053F3 RID: 21491
		[SerializeField]
		private MakeTargetSlot targetSlot;

		// Token: 0x040053F4 RID: 21492
		[SerializeField]
		private MakeTargetSlot toolSlot;

		// Token: 0x040053F5 RID: 21493
		[SerializeField]
		private PropertyItem propertyToolDurability;

		// Token: 0x040053F6 RID: 21494
		[SerializeField]
		private PropertyItem propertyToolAttainment;

		// Token: 0x040053F7 RID: 21495
		[SerializeField]
		private CDropdown perfectDropdown;

		// Token: 0x040053F8 RID: 21496
		[SerializeField]
		private TooltipInvoker tipPerfectDropdown;

		// Token: 0x040053F9 RID: 21497
		[SerializeField]
		private GameObject goMaterialGoldLine;

		// Token: 0x040053FA RID: 21498
		[SerializeField]
		private GameObject goToolGoldLine;

		// Token: 0x040053FB RID: 21499
		[SerializeField]
		private GameObject goToolGreyLine;

		// Token: 0x040053FC RID: 21500
		[SerializeField]
		private CButton buttonCancelSelectTarget;

		// Token: 0x040053FD RID: 21501
		[SerializeField]
		private CToggleGroup materialSourceToggleGroup;

		// Token: 0x040053FE RID: 21502
		[SerializeField]
		private ItemListScroll materialListScroll;

		// Token: 0x040053FF RID: 21503
		[SerializeField]
		private GameObject materialPanel;

		// Token: 0x04005400 RID: 21504
		[SerializeField]
		private ItemListScroll targetListScroll;

		// Token: 0x04005401 RID: 21505
		[SerializeField]
		private GameObject targetPanel;

		// Token: 0x04005402 RID: 21506
		[SerializeField]
		private Sprite spriteUnsigned;

		// Token: 0x04005403 RID: 21507
		[SerializeField]
		private CToggleGroup subTypeToggleGroup;

		// Token: 0x04005404 RID: 21508
		[SerializeField]
		private MakeResourceItem[] resourceItemArray;

		// Token: 0x04005405 RID: 21509
		[SerializeField]
		private GameObject resourcePanel;

		// Token: 0x04005406 RID: 21510
		[SerializeField]
		private CImage imageResourceCountProgress;

		// Token: 0x04005407 RID: 21511
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04005408 RID: 21512
		[SerializeField]
		private TooltipInvoker tipConfirm;

		// Token: 0x04005409 RID: 21513
		[SerializeField]
		private TextMeshProUGUI textConfirm;

		// Token: 0x0400540A RID: 21514
		[SerializeField]
		private GameObject rootMakeCount;

		// Token: 0x0400540B RID: 21515
		[SerializeField]
		private CSlider sliderMakeCount;

		// Token: 0x0400540C RID: 21516
		[SerializeField]
		private CButton buttonMoreMakeCount;

		// Token: 0x0400540D RID: 21517
		[SerializeField]
		private CButton buttonLessMakeCount;

		// Token: 0x0400540E RID: 21518
		[SerializeField]
		private CDropdown storageDropdown;

		// Token: 0x0400540F RID: 21519
		[SerializeField]
		private GameObject tipNoTarget;

		// Token: 0x04005410 RID: 21520
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x04005411 RID: 21521
		[SerializeField]
		private RowCellContainer iconAndTextCellContainer;

		// Token: 0x04005412 RID: 21522
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x04005413 RID: 21523
		[SerializeField]
		private GameObject eff_resourceNotEnough;

		// Token: 0x04005414 RID: 21524
		[SerializeField]
		private MakeCollectResourcePanel collectResource;

		// Token: 0x04005415 RID: 21525
		private readonly List<ItemDisplayData> _targetList = new List<ItemDisplayData>();

		// Token: 0x04005416 RID: 21526
		private readonly List<ITradeableContent> _unsignedTargetList = new List<ITradeableContent>();

		// Token: 0x04005417 RID: 21527
		private readonly List<ITradeableContent> _availableTargetList = new List<ITradeableContent>();

		// Token: 0x04005418 RID: 21528
		private readonly List<ITradeableContent> _notAvailableTargetList = new List<ITradeableContent>();

		// Token: 0x04005419 RID: 21529
		private readonly List<ItemDisplayData> _missingBuildingTargetList = new List<ItemDisplayData>();

		// Token: 0x0400541A RID: 21530
		private readonly List<ItemDisplayData> _allMaterialList = new List<ItemDisplayData>();

		// Token: 0x0400541B RID: 21531
		private readonly Dictionary<ItemKey, MakeCellData> _makeCellDataDict = new Dictionary<ItemKey, MakeCellData>();

		// Token: 0x0400541C RID: 21532
		private readonly List<ItemDisplayData> _materialList = new List<ItemDisplayData>();

		// Token: 0x0400541D RID: 21533
		private readonly List<ITradeableContent> _availableMaterialList = new List<ITradeableContent>();

		// Token: 0x0400541E RID: 21534
		private readonly List<ITradeableContent> _notAvailableMaterialList = new List<ITradeableContent>();

		// Token: 0x0400541F RID: 21535
		private int _maxFinalAttainment;

		// Token: 0x04005420 RID: 21536
		private BuildingBlockItem _upgradeBuildingConfig;

		// Token: 0x04005421 RID: 21537
		private short _makeCount = 1;

		// Token: 0x04005422 RID: 21538
		private int _maxMakeCount = 1;

		// Token: 0x04005423 RID: 21539
		private int _makeToolDurabilityCost;

		// Token: 0x04005424 RID: 21540
		private readonly Dictionary<short, List<short>> _makeTypeDict = new Dictionary<short, List<short>>();

		// Token: 0x04005425 RID: 21541
		private readonly List<short> _makeTypeList = new List<short>();

		// Token: 0x04005426 RID: 21542
		private short _makeItemTypeId;

		// Token: 0x04005427 RID: 21543
		private short _makeItemSubTypeId;

		// Token: 0x04005428 RID: 21544
		private List<short> _makeItemSubtypeIdList;

		// Token: 0x04005429 RID: 21545
		private bool _isManual;

		// Token: 0x0400542A RID: 21546
		private List<short> _canMakeItemSubTypeList;

		// Token: 0x0400542B RID: 21547
		private short _currentSelectRandomMakeItemSubType;

		// Token: 0x0400542C RID: 21548
		private short _maxMakeResourceTotalCount;

		// Token: 0x0400542D RID: 21549
		private ResourceInts _maxMakeResourceCountInts;

		// Token: 0x0400542E RID: 21550
		private ResourceInts _curMakeResourceCountInts;

		// Token: 0x0400542F RID: 21551
		private ResourceInts _lastMakeResourceCountInts;

		// Token: 0x04005430 RID: 21552
		private ResourceInts _makeRequiredResourceInts;

		// Token: 0x04005431 RID: 21553
		private sbyte _mainRequiredResourceType;

		// Token: 0x04005432 RID: 21554
		private bool _isAutoSelectTool;

		// Token: 0x04005433 RID: 21555
		private readonly Dictionary<int, MakeResult> _makeResultDict = new Dictionary<int, MakeResult>();

		// Token: 0x04005434 RID: 21556
		private readonly List<short> _perfectEffectIdList = new List<short>();

		// Token: 0x04005435 RID: 21557
		public static readonly LanguageKey[] StorageNameKeys;

		// Token: 0x04005436 RID: 21558
		private LanguageKey[] _descKeys;
	}
}
