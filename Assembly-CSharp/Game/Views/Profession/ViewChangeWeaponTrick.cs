using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.Item;
using Game.Views.Make;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007C8 RID: 1992
	public class ViewChangeWeaponTrick : UIBase
	{
		// Token: 0x17000BDB RID: 3035
		// (get) Token: 0x0600613D RID: 24893 RVA: 0x002C8A09 File Offset: 0x002C6C09
		private ItemSourceType CurWeaponSourceType
		{
			get
			{
				return this.GetItemSourceType(this.weaponSourceToggleGroup.GetActiveIndex());
			}
		}

		// Token: 0x17000BDC RID: 3036
		// (get) Token: 0x0600613E RID: 24894 RVA: 0x002C8A1C File Offset: 0x002C6C1C
		private ItemSourceType CurToolSourceType
		{
			get
			{
				return this.GetItemSourceType(this.toolSourceToggleGroup.GetActiveIndex());
			}
		}

		// Token: 0x0600613F RID: 24895 RVA: 0x002C8A30 File Offset: 0x002C6C30
		public override void OnInit(ArgumentBox argsBox)
		{
			ItemKey selectedItemKey;
			this._selectedItemKey = ((argsBox != null && argsBox.Get<ItemKey>("SelectedItemKey", out selectedItemKey)) ? selectedItemKey : ItemKey.Invalid);
			this._professionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(2);
			this._craftSkillsData = (this._professionData.SkillsData as CraftSkillsData);
			bool flag = this._craftSkillsData == null;
			if (flag)
			{
				this._professionData.SkillsData = (this._craftSkillsData = new CraftSkillsData());
			}
			CraftSkillsData craftSkillsData = this._craftSkillsData;
			if (craftSkillsData.WeaponOriginTrickDict == null)
			{
				craftSkillsData.WeaponOriginTrickDict = new Dictionary<ItemKey, GameData.Utilities.ShortList>();
			}
			this.textTitle.text = ProfessionSkill.Instance[10].Name;
			this.toolSlot.Init(EMakeTargetSlotInteract.Valid, EMakeTargetSlotType.Tool, new Action<int, ItemDisplayData>(this.OnToolCancel), null, null, null, -1, null, false, null);
			this.weaponSlot.Init(EMakeTargetSlotInteract.Valid, EMakeTargetSlotType.Weapon, new Action<int, ItemDisplayData>(this.OnWeaponCancel), null, null, null, -1, null, false, null);
			this._costResources.Initialize();
			this.NeedWaitData = true;
			this.RequestData();
		}

		// Token: 0x06006140 RID: 24896 RVA: 0x002C8B3E File Offset: 0x002C6D3E
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetChangeWeaponTrickDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this.Refresh();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06006141 RID: 24897 RVA: 0x002C8B54 File Offset: 0x002C6D54
		private void Awake()
		{
			this.weaponScroll.Init("ViewSetWeaponTrickWeaponScroll", ESortAndFilterControllerType.Equip, true, new Action<ITradeableContent, RowItemLine>(this.OnWeaponRender), new Action<ITradeableContent, RowItemLine>(this.OnWeaponClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.WeaponTrick, null, null, null);
			this.weaponScroll.SortAndFilterController.SetToggleIsOn(EFilterLine.EquipFilter.ToInt(), EEquipSubFilterKeys.Weapon.ToInt());
			this.weaponScroll.SortAndFilterController.SetToggleVisible(EFilterLine.EquipFilter.ToInt(), EEquipSubFilterKeys.Weapon.ToInt());
			this.toolScroll.SetCustomBuildGroup(new Action(this.CustomBuildGroup), false);
			this.toolScroll.Init("ViewSetWeaponTrickToolScroll", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnToolRender), new Action<ITradeableContent, RowItemLine>(this.OnToolClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.ToolAttainment, null, null, null);
			this.toolScroll.SetEmptyContent(LanguageKey.LK_Make_List_Empty.Tr());
			this.toolScroll.SortAndFilterController.SetToggleIsOn(EFilterLine.MainFilter.ToInt(), EFilterLine.CraftToolFilter.ToInt() - 1);
			this.toolScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EFilterLine.CraftToolFilter.ToInt() - 1);
			this.weaponSourceToggleGroup.Init(-1);
			this.weaponSourceToggleGroup.OnActiveIndexChange += this.WeaponSourceToggleGroupOnActiveIndexChange;
			this.toolSourceToggleGroup.Init(-1);
			this.toolSourceToggleGroup.OnActiveIndexChange += this.ToolSourceToggleGroupOnActiveIndexChange;
			this.buttonConfirm.ClearAndAddListener(new Action(this.OnClickButtonConfirm));
			this.buttonClose.ClearAndAddListener(new Action(this.OnClickButtonClose));
			this.buttonChangeCostItem.ClearAndAddListener(new Action(this.OnClickButtonChangeCostItem));
		}

		// Token: 0x06006142 RID: 24898 RVA: 0x002C8D2B File Offset: 0x002C6F2B
		private void OnEnable()
		{
			this.weaponSourceToggleGroup.SetWithoutNotify(0);
			this.toolSourceToggleGroup.SetWithoutNotify(0);
		}

		// Token: 0x06006143 RID: 24899 RVA: 0x002C8D48 File Offset: 0x002C6F48
		private void OnDestroy()
		{
			this.weaponSourceToggleGroup.OnActiveIndexChange -= this.WeaponSourceToggleGroupOnActiveIndexChange;
			this.toolSourceToggleGroup.OnActiveIndexChange -= this.ToolSourceToggleGroupOnActiveIndexChange;
		}

		// Token: 0x06006144 RID: 24900 RVA: 0x002C8D7B File Offset: 0x002C6F7B
		private void RefreshSourceToggleInteractable(CToggleGroup toggleGroup)
		{
			ItemSourceToggleHelper.RefreshInteractableForInteract(toggleGroup, this._displayData.CanTransferItemToWarehouse, false);
		}

		// Token: 0x06006145 RID: 24901 RVA: 0x002C8D91 File Offset: 0x002C6F91
		private void WeaponSourceToggleGroupOnActiveIndexChange(int arg1, int arg2)
		{
			this.RefreshWeaponList();
		}

		// Token: 0x06006146 RID: 24902 RVA: 0x002C8D9A File Offset: 0x002C6F9A
		private void ToolSourceToggleGroupOnActiveIndexChange(int arg1, int arg2)
		{
			this.RefreshToolList();
		}

		// Token: 0x06006147 RID: 24903 RVA: 0x002C8DA4 File Offset: 0x002C6FA4
		private void OnWeaponRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetSelected(this.weaponScroll.SelectedItem == content);
		}

		// Token: 0x06006148 RID: 24904 RVA: 0x002C8DE8 File Offset: 0x002C6FE8
		private void OnWeaponClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			bool flag = this.weaponSlot.IsValid && this.weaponSlot.ItemData.RealKey.Equals(content.RealKey);
			if (flag)
			{
				this.weaponSlot.Cancel();
			}
			else
			{
				this.SelectWeapon(content);
			}
		}

		// Token: 0x06006149 RID: 24905 RVA: 0x002C8E40 File Offset: 0x002C7040
		private void SelectWeapon(ITradeableContent content)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			this.weaponSlot.Cancel();
			this.weaponSlot.Select(itemData, false);
			this.weaponScroll.SelectedItem = content;
			this.weaponScroll.ReRender();
			GameData.Utilities.ShortList originTricks;
			bool flag = !this._craftSkillsData.WeaponOriginTrickDict.TryGetValue(this.weaponSlot.ItemData.RealKey, out originTricks);
			if (flag)
			{
				originTricks = GameData.Utilities.ShortList.Create();
				foreach (sbyte t in this.weaponSlot.ItemData.WeaponTrickList)
				{
					originTricks.Items.Add((short)t);
				}
				this._craftSkillsData.WeaponOriginTrickDict[this.weaponSlot.ItemData.RealKey] = originTricks;
			}
			this.AutoAddNeedMaterial();
			this.AutoSelectMaterial();
			this.RefreshTrick(itemData);
			this.RefreshToolList();
			bool isValid = this.toolSlot.IsValid;
			if (isValid)
			{
				MaterialItem materialConfig = this.GetMaterialConfig();
				CraftToolItem toolConfig = CraftTool.Instance[this.toolSlot.ItemData.RealKey.TemplateId];
				bool flag2 = !toolConfig.RequiredLifeSkillTypes.Contains(materialConfig.RequiredLifeSkillType);
				if (flag2)
				{
					this.toolSlot.Cancel();
				}
			}
			bool isToggleOn = this.toolSlot.IsToggleOn;
			if (isToggleOn)
			{
				ItemDisplayData tool = this.GetAutoSelectTool();
				bool flag3 = tool != null;
				if (flag3)
				{
					this.SelectTool(tool);
				}
			}
			this.toolScroll.ReRender();
			this.RefreshButtonConfirm();
		}

		// Token: 0x0600614A RID: 24906 RVA: 0x002C8FF8 File Offset: 0x002C71F8
		private void OnWeaponCancel(int index, ItemDisplayData itemDisplayData)
		{
			this.toolSlot.Cancel();
			this.RefreshToolList();
			this.RefreshTrick(null);
			this.weaponScroll.SelectedItem = null;
			this.weaponScroll.ReRender();
			this.RefreshButtonConfirm();
			this.toolScroll.ReRender();
		}

		// Token: 0x0600614B RID: 24907 RVA: 0x002C904C File Offset: 0x002C724C
		private void RefreshWeaponList()
		{
			List<ItemDisplayData> curItemList = this.GetItemList(this.CurWeaponSourceType);
			this.weaponScroll.SetItemList(curItemList);
		}

		// Token: 0x0600614C RID: 24908 RVA: 0x002C9074 File Offset: 0x002C7274
		private void OnToolRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetSelected(this.toolScroll.SelectedItem == content);
			bool interactable = content.Interactable;
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
			bool flag = interactable;
			if (flag)
			{
				rowItemMain.HideInteractionState();
			}
			else
			{
				rowItemMain.SetInteractionStateLockText(LanguageKey.LK_Item_Operation_AttainmentNotMeet.Tr());
			}
		}

		// Token: 0x0600614D RID: 24909 RVA: 0x002C90F0 File Offset: 0x002C72F0
		private void OnToolClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			bool flag = this.toolSlot.IsValid && this.toolSlot.ItemData.RealKey.Equals(content.RealKey);
			if (flag)
			{
				this.toolSlot.Cancel();
			}
			else
			{
				this.SelectTool(content);
			}
		}

		// Token: 0x0600614E RID: 24910 RVA: 0x002C9148 File Offset: 0x002C7348
		private void SelectTool(ITradeableContent content)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			this.toolSlot.Cancel();
			this.toolSlot.Select(itemData, false);
			this.toolScroll.SelectedItem = content;
			this.toolScroll.ReRender();
			this.RefreshButtonConfirm();
		}

		// Token: 0x0600614F RID: 24911 RVA: 0x002C9196 File Offset: 0x002C7396
		private void OnToolCancel(int index, ItemDisplayData itemDisplayData)
		{
			this.toolScroll.SelectedItem = null;
			this.toolScroll.ReRender();
			this.RefreshButtonConfirm();
		}

		// Token: 0x06006150 RID: 24912 RVA: 0x002C91B8 File Offset: 0x002C73B8
		private void RefreshToolList()
		{
			this._toolList.Clear();
			bool flag = !this.weaponSlot.IsValid;
			if (flag)
			{
				foreach (ItemDisplayData data in this._allToolList)
				{
					data.DurabilityChange = null;
				}
				this.toolScroll.SetItemList(this._toolList);
			}
			else
			{
				this._toolList.Add(this._emptyTool);
				this._emptyTool.Interactable = this.CheckTool(this._emptyTool);
				MaterialItem materialConfig = this.GetMaterialConfig();
				sbyte lifeSkillType = materialConfig.RequiredLifeSkillType;
				ItemSourceType curToolSourceType = this.CurToolSourceType;
				sbyte grade = this.weaponSlot.ItemData.Grade;
				foreach (ItemDisplayData data2 in this._allToolList)
				{
					bool flag2 = data2.ItemSourceTypeEnum != curToolSourceType;
					if (!flag2)
					{
						CraftToolItem toolConfig = CraftTool.Instance[data2.RealKey.TemplateId];
						bool flag3 = !toolConfig.RequiredLifeSkillTypes.Contains(lifeSkillType);
						if (!flag3)
						{
							short costDurability = toolConfig.DurabilityCost[(int)grade];
							string curText = data2.Durability.ToString().SetColor("pinkyellow");
							string costText = string.Format("-{0}", costDurability).SetColor("brightred");
							string maxText = data2.MaxDurability.ToString().SetColor("pinkyellow");
							data2.DurabilityChange = curText + costText + "/" + maxText;
							data2.Interactable = (!this.weaponSlot.IsValid || this.CheckTool(data2));
							this._toolList.Add(data2);
						}
					}
				}
				this.toolScroll.SetItemList(this._toolList);
			}
		}

		// Token: 0x06006151 RID: 24913 RVA: 0x002C93F0 File Offset: 0x002C75F0
		private void CustomBuildGroup()
		{
			string title = LanguageKey.LK_Make_Tool_Group_CanUse.Tr();
			this._availableToolList.Clear();
			this._availableToolList.AddRange(from d in this.toolScroll.FilteredData
			where d.Interactable
			select d);
			this.toolScroll.AddGroup(0, title, this._availableToolList, null, true);
			string title2 = LanguageKey.LK_Make_Tool_Group_CanNotUse.Tr();
			this._notAvailableToolList.Clear();
			this._notAvailableToolList.AddRange(from d in this.toolScroll.FilteredData
			where !d.Interactable
			select d);
			this.toolScroll.AddGroup(1, title2, this._notAvailableToolList, null, true);
		}

		// Token: 0x06006152 RID: 24914 RVA: 0x002C94D4 File Offset: 0x002C76D4
		public ItemDisplayData GetAutoSelectTool()
		{
			bool flag = this.CheckTool(this._emptyTool);
			ItemDisplayData result;
			if (flag)
			{
				result = this._emptyTool;
			}
			else
			{
				IOrderedEnumerable<ItemDisplayData> orderList = this._allToolList.Where(new Func<ItemDisplayData, bool>(this.CheckTool)).OrderByDescending(delegate(ItemDisplayData d)
				{
					sbyte grade = d.Grade;
					short onceCost = ViewMake.GetToolDurabilityCost(d, this.weaponSlot.ItemData.Grade);
					int gradeScore = GlobalConfig.Instance.MakeAutoSelectToolGradeScore[(int)grade] * (int)onceCost;
					int destroyScore = (onceCost >= d.Durability) ? GlobalConfig.Instance.MakeAutoSelectToolDestroyScore[(int)grade] : 0;
					return gradeScore + destroyScore;
				});
				result = orderList.FirstOrDefault<ItemDisplayData>();
			}
			return result;
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x002C9530 File Offset: 0x002C7730
		private bool CheckTool(ITradeableContent toolData)
		{
			sbyte weaponGrade = this.weaponSlot.ItemData.Grade;
			MaterialItem materialConfig = this.GetMaterialConfig();
			sbyte lifeSkillType = materialConfig.RequiredLifeSkillType;
			CraftToolItem toolConfig = CraftTool.Instance[toolData.RealKey.TemplateId];
			bool flag = !toolConfig.RequiredLifeSkillTypes.Contains(lifeSkillType);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short needAttainment = GlobalConfig.Instance.RepairAttainments[(int)weaponGrade];
				short charAttainment = this._displayData.LifeSkillAttainments.Get((int)lifeSkillType);
				short finalAttainment = ViewMake.GetFinalAttainment(toolData.RealKey.TemplateId, charAttainment, lifeSkillType);
				bool isSkillMeet = finalAttainment >= needAttainment;
				bool isToolMeet = toolData.MaxDurability == 0 || toolData.Durability > 0;
				result = (isSkillMeet && isToolMeet);
			}
			return result;
		}

		// Token: 0x06006154 RID: 24916 RVA: 0x002C95F0 File Offset: 0x002C77F0
		private void RefreshAllToolList()
		{
			this._allToolList.Clear();
			this.<RefreshAllToolList>g__Add|59_0(this._displayData.InventoryItemList);
			bool canTransferItemToWarehouse = this._displayData.CanTransferItemToWarehouse;
			if (canTransferItemToWarehouse)
			{
				this.<RefreshAllToolList>g__Add|59_0(this._displayData.WarehouseItemList);
				this.<RefreshAllToolList>g__Add|59_0(this._displayData.TreasuryItemList);
			}
		}

		// Token: 0x06006155 RID: 24917 RVA: 0x002C9654 File Offset: 0x002C7854
		private void RefreshTrick(ItemDisplayData itemData)
		{
			this._trickList.Clear();
			this._trickOptionList.Clear();
			bool flag = itemData != null;
			if (flag)
			{
				this._trickList.AddRange(itemData.WeaponTrickList);
				WeaponItem weaponConfig = Weapon.Instance[itemData.RealKey.TemplateId];
				foreach (WeaponItem weaponItem in ((IEnumerable<WeaponItem>)Weapon.Instance))
				{
					bool flag2 = weaponItem.GroupId == weaponConfig.GroupId;
					if (flag2)
					{
						foreach (sbyte weaponItemTrick in weaponItem.Tricks)
						{
							bool flag3 = !this._trickOptionList.Contains(weaponItemTrick);
							if (flag3)
							{
								this._trickOptionList.Add(weaponItemTrick);
							}
						}
					}
				}
			}
			bool flag4 = this._trickList.Count > 0;
			if (flag4)
			{
				List<string> optionNameList = this._trickOptionList.Select(delegate(sbyte id)
				{
					TrickTypeItem config = TrickType.Instance[id];
					return config.ChineseName.SetColor(config.FontColor);
				}).ToList<string>();
				for (int slotIndex = 0; slotIndex < this._trickList.Count; slotIndex++)
				{
					sbyte curId = this._trickList[slotIndex];
					WeaponTrickSlot slot = this.trickSlotArray[slotIndex];
					slot.gameObject.SetActive(true);
					int optionIndex = this._trickOptionList.IndexOf(curId);
					slot.Refresh(slotIndex, optionIndex, optionNameList, new Action<int, int>(this.OnTrickChange));
					GameData.Utilities.ShortList originTricks;
					this._craftSkillsData.WeaponOriginTrickDict.TryGetValue(this.weaponSlot.ItemData.RealKey, out originTricks);
					short originId = originTricks.Items[slotIndex];
					bool isChanged = (short)curId != originId;
					slot.SetIsChanged(isChanged);
				}
			}
			for (int i = this._trickList.Count; i < this.trickSlotArray.Length; i++)
			{
				this.trickSlotArray[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x002C98AC File Offset: 0x002C7AAC
		private void OnTrickChange(int slotIndex, int optionIndex)
		{
			this._trickList[slotIndex] = this._trickOptionList[optionIndex];
			sbyte curId = this._trickList[slotIndex];
			GameData.Utilities.ShortList originTricks;
			this._craftSkillsData.WeaponOriginTrickDict.TryGetValue(this.weaponSlot.ItemData.RealKey, out originTricks);
			short originId = originTricks.Items[slotIndex];
			bool isChanged = (short)curId != originId;
			WeaponTrickSlot slot = this.trickSlotArray[slotIndex];
			slot.SetIsChanged(isChanged);
			this.RefreshChangeCountToOrigin();
			this.AutoSelectMaterial();
			this.RefreshButtonConfirm();
		}

		// Token: 0x06006157 RID: 24919 RVA: 0x002C9940 File Offset: 0x002C7B40
		private List<ItemDisplayData> GetItemList(ItemSourceType itemSourceType)
		{
			if (!true)
			{
			}
			List<ItemDisplayData> result;
			switch (itemSourceType)
			{
			case ItemSourceType.Inventory:
				result = (this._displayData.InventoryItemList ?? new List<ItemDisplayData>());
				goto IL_91;
			case ItemSourceType.Warehouse:
				result = (this._displayData.WarehouseItemList ?? new List<ItemDisplayData>());
				goto IL_91;
			case ItemSourceType.Treasury:
				result = (this._displayData.TreasuryItemList ?? new List<ItemDisplayData>());
				goto IL_91;
			case ItemSourceType.Stock:
				result = (this._displayData.StockItemList ?? new List<ItemDisplayData>());
				goto IL_91;
			}
			throw new ArgumentOutOfRangeException("itemSourceType", itemSourceType, null);
			IL_91:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006158 RID: 24920 RVA: 0x002C99E8 File Offset: 0x002C7BE8
		private void RefreshButtonConfirm()
		{
			this.RefreshChangeCountToOrigin();
			int remainActionPoint = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
			int remainDays = remainActionPoint / 10;
			short costDays = ProfessionSkill.Instance[10].TimeCost;
			bool isTimeMeet = (int)costDays <= remainDays;
			string color = isTimeMeet ? "brightblue" : "brightred";
			string costStr = string.Format("{0}/{1}", remainDays.ToString().SetColor(color), costDays);
			this.propertyCostTime.SetValue(costStr);
			bool hasWeapon = this.weaponSlot.IsValid;
			bool hasChangeTrick = false;
			int changeTrickCountToLast = 0;
			bool flag = hasWeapon;
			if (flag)
			{
				for (int i = 0; i < this._trickList.Count; i++)
				{
					bool flag2 = this._trickList[i] != this.weaponSlot.ItemData.WeaponTrickList[i];
					if (flag2)
					{
						changeTrickCountToLast++;
						hasChangeTrick = true;
					}
				}
			}
			bool hasTool = this.toolSlot.ItemData != null;
			bool flag3 = hasWeapon;
			if (flag3)
			{
				bool flag4 = hasTool;
				if (flag4)
				{
					this.propertyToolDurability.SetValue(this.toolSlot.ItemData.DurabilityChange);
				}
				this.propertyToolDurability.gameObject.SetActive(hasTool);
			}
			else
			{
				this.propertyToolDurability.gameObject.SetActive(false);
			}
			bool isMaterialMeet = false;
			bool isResourceMeet = false;
			bool isSkillMeet = false;
			bool flag5 = hasWeapon;
			if (flag5)
			{
				WeaponItem weaponConfig = Weapon.Instance[this.weaponSlot.ItemData.RealKey.TemplateId];
				MaterialItem materialConfig = this.GetMaterialConfig();
				int ownMaterialCount = 0;
				bool flag6 = this._selectedMaterialList.Count > 0;
				if (flag6)
				{
					foreach (SelectedItemData selectedItemData in this._selectedMaterialList)
					{
						ownMaterialCount += this._displayData.AllMaterialInventory.GetInventoryItemCount(5, selectedItemData.ItemData.RealKey.TemplateId);
					}
				}
				else
				{
					foreach (short templateId in this._needMaterialTemplateIdList)
					{
						ownMaterialCount += this._displayData.AllMaterialInventory.GetInventoryItemCount(5, templateId);
					}
				}
				int costMaterialCount = this._professionData.GetChangeWeaponTrickCostMaterialCount(this._changeCountToOrigin);
				isMaterialMeet = (ownMaterialCount >= costMaterialCount);
				string ownMaterialCountColor = isMaterialMeet ? "brightblue" : "brightred";
				string materialCountStr = string.Format("{0}/{1}", ownMaterialCount.ToString().SetColor(ownMaterialCountColor), costMaterialCount);
				LanguageKey gradeNameKey = LanguageKey.LK_Mousetip_Grade_Short_0 + (int)materialConfig.Grade;
				short itemSubType = materialConfig.ItemSubType;
				if (!true)
				{
				}
				string text;
				if (itemSubType != 505)
				{
					if (itemSubType != 506)
					{
						text = materialConfig.Name;
					}
					else
					{
						text = gradeNameKey.Tr() + LanguageKey.LK_ItemSubType_506.Tr();
					}
				}
				else
				{
					text = gradeNameKey.Tr() + LanguageKey.LK_ItemSubType_505.Tr();
				}
				if (!true)
				{
				}
				string itemName = text;
				this.textCostItemName.text = itemName.SetGradeColor((int)materialConfig.Grade);
				short itemSubType2 = materialConfig.ItemSubType;
				if (!true)
				{
				}
				if (itemSubType2 != 505)
				{
					if (itemSubType2 != 506)
					{
						text = materialConfig.Icon;
					}
					else
					{
						text = "ui9_icon_profession_craft_poison_material";
					}
				}
				else
				{
					text = "ui9_icon_profession_craft_medicine_material";
				}
				if (!true)
				{
				}
				string icon = text;
				this.costItem.SetIcon(icon);
				this.costItem.SetBack(materialConfig.Grade);
				this.costItem.SetCountInfo(materialCountStr, string.Empty);
				int costResource = this._professionData.GetSeniorityChangeWeaponTrickCostResource(changeTrickCountToLast, this.weaponSlot.ItemData.Grade);
				int ownResource = SingletonObject.getInstance<BuildingModel>().GetResourceCount(ItemSourceType.Resources, weaponConfig.ResourceType);
				isResourceMeet = (ownResource >= costResource);
				this._costResources.Initialize();
				this._costResources.Set((int)materialConfig.ResourceType, costResource);
				string resourceColor = isResourceMeet ? "brightblue" : "brightred";
				string resourceStr = string.Format("{0}/{1}", CommonUtils.GetDisplayStringForNum(ownResource, 100000).SetColor(resourceColor), costResource);
				ResourceTypeItem resourceConfig = Config.ResourceType.Instance[materialConfig.ResourceType];
				this.propertyCostResource.Set(resourceConfig.Icon, resourceConfig.Name, resourceStr, null, false);
				sbyte lifeSkillType = materialConfig.RequiredLifeSkillType;
				LifeSkillTypeItem lifeSkillConfig = Config.LifeSkillType.Instance[lifeSkillType];
				short needAttainment = GlobalConfig.Instance.RepairAttainments[(int)weaponConfig.Grade];
				short charAttainment = this._displayData.LifeSkillAttainments.Get((int)lifeSkillType);
				short toolTemplateId = hasTool ? this.toolSlot.ItemData.RealKey.TemplateId : -1;
				short finalAttainment = hasTool ? ViewMake.GetFinalAttainment(toolTemplateId, charAttainment, lifeSkillType) : charAttainment;
				isSkillMeet = (finalAttainment >= needAttainment);
				string finalAttainmentColor = isSkillMeet ? "brightblue" : "brightred";
				string attainmentStr = string.Format("{0}/{1}", finalAttainment.ToString().SetColor(finalAttainmentColor), needAttainment);
				this.propertyNeedAttainment.Set(lifeSkillConfig.Icon, lifeSkillConfig.Name, attainmentStr, null, false);
			}
			this.costItem.gameObject.SetActive(hasWeapon);
			this.propertyNeedAttainment.gameObject.SetActive(hasWeapon);
			this.propertyCostResource.gameObject.SetActive(hasWeapon);
			this.buttonConfirm.interactable = (isTimeMeet && hasWeapon && hasTool && hasChangeTrick && isMaterialMeet && isSkillMeet && isResourceMeet);
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			stringBuilder.AppendLine(LanguageKey.LK_Profession_ChangeWeaponTrick_Confirm_Tip.Tr());
			bool flag7 = !hasChangeTrick;
			if (flag7)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Profession_ChangeWeaponTrick_NoChange.Tr().ColorReplace());
			}
			bool flag8 = !isTimeMeet;
			if (flag8)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Making_Time_Not_Enough.Tr().ColorReplace());
			}
			bool flag9 = !hasWeapon;
			if (flag9)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Multiply_Select_None_Tip.Tr().ColorReplace());
			}
			else
			{
				bool flag10 = !hasTool;
				if (flag10)
				{
					stringBuilder.AppendLine(LanguageKey.LK_Making_Tool_Not_Selected.Tr().ColorReplace());
				}
			}
			bool flag11 = !isMaterialMeet;
			if (flag11)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Profession_ChangeWeaponTrick_Material_NotEnough.Tr().ColorReplace());
			}
			bool flag12 = !isResourceMeet;
			if (flag12)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Making_Resource_Not_Enough.Tr().ColorReplace());
			}
			bool flag13 = !isSkillMeet;
			if (flag13)
			{
				stringBuilder.AppendLine(LanguageKey.LK_Making_Attainment_Not_Enough.Tr().ColorReplace());
			}
			string tipContent = stringBuilder.ToString();
			this.tipButtonConfirm.enabled = (!this.buttonConfirm.interactable && !tipContent.IsNullOrEmpty());
			bool enabled = this.tipButtonConfirm.enabled;
			if (enabled)
			{
				this.tipButtonConfirm.PresetParam = new string[]
				{
					tipContent
				};
			}
			stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x06006159 RID: 24921 RVA: 0x002CA138 File Offset: 0x002C8338
		private void RefreshChangeCountToOrigin()
		{
			this._changeCountToOrigin = 0;
			bool flag = !this.weaponSlot.IsValid;
			if (!flag)
			{
				GameData.Utilities.ShortList originTricks;
				this._craftSkillsData.WeaponOriginTrickDict.TryGetValue(this.weaponSlot.ItemData.RealKey, out originTricks);
				for (int i = 0; i < originTricks.Items.Count; i++)
				{
					bool flag2 = originTricks.Items[i] != (short)this._trickList[i];
					if (flag2)
					{
						this._changeCountToOrigin++;
					}
				}
			}
		}

		// Token: 0x0600615A RID: 24922 RVA: 0x002CA1D0 File Offset: 0x002C83D0
		private void OnClickButtonConfirm()
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			Dictionary<ItemSourceType, Inventory> costMaterials = new Dictionary<ItemSourceType, Inventory>();
			foreach (SelectedItemData selectedItemData in this._selectedMaterialList)
			{
				ItemDisplayData itemData = selectedItemData.ItemData as ItemDisplayData;
				Inventory inventory;
				bool flag = !costMaterials.TryGetValue(itemData.ItemSourceTypeEnum, out inventory);
				if (flag)
				{
					inventory = (costMaterials[itemData.ItemSourceTypeEnum] = new Inventory());
				}
				Inventory keyList = itemData.GetOperationInventoryFromPool(selectedItemData.SelectedAmount, true);
				inventory.OfflineAdd(keyList);
				ItemDisplayData.ReturnInventoryToPool(keyList);
			}
			ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
			{
				ProfessionId = 2,
				SkillId = 10,
				IsSuccess = true,
				WeaponKey = this.weaponSlot.ItemData.RealKey,
				TrickList = this._trickList,
				ToolKey = this.toolSlot.ItemData.RealKey,
				ToolSourceType = this.toolSlot.ItemData.ItemSourceTypeEnum,
				CostMaterials = costMaterials
			};
			argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
			argumentBox.SetObject("OnConfirm", new Action(this.<OnClickButtonConfirm>g__Action|65_0));
			argumentBox.SetObject("CostResources", this._costResources);
			UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
			UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
		}

		// Token: 0x0600615B RID: 24923 RVA: 0x002CA360 File Offset: 0x002C8560
		private void Refresh()
		{
			if (this._emptyTool == null)
			{
				this._emptyTool = new ItemDisplayData(this._displayData.EmptyToolKey, 1);
			}
			this.RefreshAllToolList();
			this.weaponScroll.SelectedItem = null;
			this.toolScroll.SelectedItem = null;
			this.RefreshSourceToggleInteractable(this.weaponSourceToggleGroup);
			this.RefreshSourceToggleInteractable(this.toolSourceToggleGroup);
			this.RefreshWeaponList();
			this.RefreshToolList();
			bool flag = this._selectedItemKey.IsValid();
			if (flag)
			{
				List<ItemDisplayData> itemList = this.GetItemList(ItemSourceType.Inventory);
				ItemDisplayData itemData = itemList.Find((ItemDisplayData d) => d.RealKey.Equals(this._selectedItemKey));
				bool flag2 = itemData != null;
				if (flag2)
				{
					this.SelectWeapon(itemData);
				}
				this.RefreshTrick(itemData);
			}
			else
			{
				bool isValid = this.weaponSlot.IsValid;
				if (isValid)
				{
					List<ItemDisplayData> itemList2 = this.GetItemList(this.weaponSlot.ItemData.ItemSourceTypeEnum);
					ItemDisplayData itemData2 = itemList2.Find((ItemDisplayData d) => d.RealKey.Equals(this.weaponSlot.ItemData.RealKey));
					bool flag3 = itemData2 != null;
					if (flag3)
					{
						this.SelectWeapon(itemData2);
					}
					this.RefreshTrick(itemData2);
				}
				else
				{
					this.RefreshTrick(null);
				}
			}
			bool flag4 = this.toolSlot.IsValid && !this.toolSlot.IsToggleOn;
			if (flag4)
			{
				List<ItemDisplayData> itemList3 = this.GetItemList(this.toolSlot.ItemData.ItemSourceTypeEnum);
				ItemDisplayData itemData3 = itemList3.Find((ItemDisplayData d) => d.RealKey.Equals(this.toolSlot.ItemData.RealKey));
				bool flag5 = itemData3 != null;
				if (flag5)
				{
					this.SelectTool(itemData3);
				}
			}
			this.RefreshButtonConfirm();
		}

		// Token: 0x0600615C RID: 24924 RVA: 0x002CA4F1 File Offset: 0x002C86F1
		private void OnClickButtonClose()
		{
			this.QuickHide();
		}

		// Token: 0x0600615D RID: 24925 RVA: 0x002CA4FC File Offset: 0x002C86FC
		private MaterialItem GetMaterialConfig()
		{
			bool flag = !this.weaponSlot.IsValid;
			MaterialItem result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = Config.Material.Instance[this._needMaterialTemplateIdList.First<short>()];
			}
			return result;
		}

		// Token: 0x0600615E RID: 24926 RVA: 0x002CA53C File Offset: 0x002C873C
		private void AddNeedMaterial(List<SelectedItemData> selectedItems)
		{
			this._needMaterialTemplateIdList.Clear();
			foreach (SelectedItemData selectedItemData in selectedItems)
			{
				this._needMaterialTemplateIdList.Add(selectedItemData.ItemData.RealKey.TemplateId);
			}
		}

		// Token: 0x0600615F RID: 24927 RVA: 0x002CA5B0 File Offset: 0x002C87B0
		private void AutoAddNeedMaterial()
		{
			this._needMaterialTemplateIdList.Clear();
			ProfessionData professionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(2);
			WeaponItem weaponConfig = Weapon.Instance[this.weaponSlot.ItemData.RealKey.TemplateId];
			MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance.First((MakeItemTypeItem m) => m.MakeItemSubTypes.Contains(weaponConfig.MakeItemSubType));
			sbyte materialGrade = (sbyte)professionData.GetChangeWeaponTrickCostMaterialGrade(weaponConfig.Grade);
			materialGrade = this.GetGrade(makeItemTypeConfig.TemplateId, weaponConfig.ResourceType, materialGrade);
			foreach (MaterialItem materialItem in ((IEnumerable<MaterialItem>)Config.Material.Instance))
			{
				bool flag = materialItem.Grade == materialGrade && materialItem.CraftableItemTypes.Contains(makeItemTypeConfig.TemplateId);
				if (flag)
				{
					this._needMaterialTemplateIdList.Add(materialItem.TemplateId);
				}
			}
		}

		// Token: 0x06006160 RID: 24928 RVA: 0x002CA6BC File Offset: 0x002C88BC
		private void SelectMaterial(List<SelectedItemData> selectedItems)
		{
			this._selectedMaterialList.Clear();
			this._selectedMaterialList.AddRange(selectedItems);
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x002CA6D8 File Offset: 0x002C88D8
		private void AutoSelectMaterial()
		{
			this._selectedMaterialList.Clear();
			int needCount = this._professionData.GetChangeWeaponTrickCostMaterialCount(this._changeCountToOrigin);
			this.AutoSelectMaterial(this._displayData.InventoryItemList, ref needCount);
			this.AutoSelectMaterial(this._displayData.WarehouseItemList, ref needCount);
			this.AutoSelectMaterial(this._displayData.TreasuryItemList, ref needCount);
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x002CA740 File Offset: 0x002C8940
		private void AutoSelectMaterial(List<ItemDisplayData> itemList, ref int needCount)
		{
			bool flag = itemList == null || needCount <= 0;
			if (!flag)
			{
				List<ItemDisplayData> materials = (from d in itemList.Where(new Func<ItemDisplayData, bool>(this.FilterMaterial))
				orderby d.RealKey.TemplateId
				select d).ToList<ItemDisplayData>();
				foreach (ItemDisplayData material in materials)
				{
					int selectCont = Math.Min(material.Amount, needCount);
					needCount -= selectCont;
					this._selectedMaterialList.Add(new SelectedItemData(material, selectCont));
					bool flag2 = needCount <= 0;
					if (flag2)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x002CA81C File Offset: 0x002C8A1C
		private sbyte GetGrade(short makeItemType, sbyte resourceType, sbyte grade)
		{
			bool flag = resourceType != 5;
			sbyte result;
			if (flag)
			{
				result = grade;
			}
			else
			{
				sbyte finalGrade = (from g in (from m in Config.Material.Instance
				where m.CraftableItemTypes.Contains(makeItemType) && m.ResourceType == 5
				select m.Grade).Distinct<sbyte>()
				orderby g descending
				select g).First((sbyte g) => grade >= g);
				result = finalGrade;
			}
			return result;
		}

		// Token: 0x06006164 RID: 24932 RVA: 0x002CA8CC File Offset: 0x002C8ACC
		private ItemSourceType GetItemSourceType(int togIndex)
		{
			if (!true)
			{
			}
			ItemSourceType result;
			switch (togIndex)
			{
			case 0:
				result = ItemSourceType.Inventory;
				break;
			case 1:
				result = ItemSourceType.Warehouse;
				break;
			case 2:
				result = ItemSourceType.Treasury;
				break;
			case 3:
				result = ItemSourceType.Stock;
				break;
			default:
				throw new ArgumentOutOfRangeException("togIndex", togIndex, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006165 RID: 24933 RVA: 0x002CA920 File Offset: 0x002C8B20
		private void OnClickButtonChangeCostItem()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
			{
				OnlyFromInventory = false
			}, new SelectItemsCallback(this.OnSelectItems), "", null);
			config.OperationMode = ESelectItemOperationMode.Slot;
			int costMaterialCount = this._professionData.GetChangeWeaponTrickCostMaterialCount(this._changeCountToOrigin);
			config.MinSelectCount = (config.MaxSelectCount = costMaterialCount);
			config.ShowQuickButton = false;
			config.IsAllowSameTemplateIdItem = false;
			config.AllowEmpty = true;
			config.ExternalItems = this._displayData.InventoryItemList.Where(new Func<ItemDisplayData, bool>(this.FilterMaterial)).ToList<ItemDisplayData>();
			config.ExternalWarehouseItems = (this._displayData.CanTransferItemToWarehouse ? this._displayData.WarehouseItemList.Where(new Func<ItemDisplayData, bool>(this.FilterMaterial)).ToList<ItemDisplayData>() : null);
			config.ExternalTreasuryItems = (this._displayData.CanTransferItemToWarehouse ? this._displayData.TreasuryItemList.Where(new Func<ItemDisplayData, bool>(this.FilterMaterial)).ToList<ItemDisplayData>() : null);
			config.InitialSelectedItems = this._selectedMaterialList;
			config.Rules = new SelectItemRules
			{
				OnlyFromInventory = !this._displayData.CanTransferItemToWarehouse
			};
			UIElement.SelectItem.SetOnInitArgs(argBox.SetObject("SelectItemConfig", config));
			UIManager.Instance.MaskUI(UIElement.SelectItem);
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x002CAA94 File Offset: 0x002C8C94
		private bool FilterMaterial(ItemDisplayData itemData)
		{
			bool flag = itemData.RealKey.ItemType != 5;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ProfessionData professionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(2);
				WeaponItem weaponConfig = Weapon.Instance[this.weaponSlot.ItemData.RealKey.TemplateId];
				MakeItemTypeItem makeItemTypeConfig = MakeItemType.Instance.First((MakeItemTypeItem m) => m.MakeItemSubTypes.Contains(weaponConfig.MakeItemSubType));
				sbyte materialGrade = (sbyte)professionData.GetChangeWeaponTrickCostMaterialGrade(weaponConfig.Grade);
				materialGrade = this.GetGrade(makeItemTypeConfig.TemplateId, weaponConfig.ResourceType, materialGrade);
				MaterialItem materialConfig = Config.Material.Instance[itemData.RealKey.TemplateId];
				result = (materialConfig.Grade == materialGrade && materialConfig.CraftableItemTypes.Contains(makeItemTypeConfig.TemplateId));
			}
			return result;
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x002CAB78 File Offset: 0x002C8D78
		private void OnSelectItems(List<SelectedItemData> selectedItems)
		{
			bool flag = selectedItems != null && selectedItems.Count > 0;
			if (flag)
			{
				this.AddNeedMaterial(selectedItems);
				this.SelectMaterial(selectedItems);
			}
			else
			{
				this.AutoAddNeedMaterial();
				this.AutoSelectMaterial();
			}
			this.RefreshButtonConfirm();
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x002CACB4 File Offset: 0x002C8EB4
		[CompilerGenerated]
		private void <RefreshAllToolList>g__Add|59_0(List<ItemDisplayData> list)
		{
			bool flag = list == null;
			if (!flag)
			{
				foreach (ItemDisplayData itemData in list)
				{
					sbyte itemType = itemData.RealKey.ItemType;
					sbyte b = itemType;
					if (b == 6)
					{
						this._allToolList.Add(itemData);
					}
				}
			}
		}

		// Token: 0x0600616C RID: 24940 RVA: 0x002CAD30 File Offset: 0x002C8F30
		[CompilerGenerated]
		private void <OnClickButtonConfirm>g__Action|65_0()
		{
			this.QuickHide();
		}

		// Token: 0x04004373 RID: 17267
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04004374 RID: 17268
		[SerializeField]
		private ItemListScroll weaponScroll;

		// Token: 0x04004375 RID: 17269
		[SerializeField]
		private ItemListScroll toolScroll;

		// Token: 0x04004376 RID: 17270
		[SerializeField]
		private MakeTargetSlot weaponSlot;

		// Token: 0x04004377 RID: 17271
		[SerializeField]
		private MakeTargetSlot toolSlot;

		// Token: 0x04004378 RID: 17272
		[SerializeField]
		private CToggleGroup weaponSourceToggleGroup;

		// Token: 0x04004379 RID: 17273
		[SerializeField]
		private CToggleGroup toolSourceToggleGroup;

		// Token: 0x0400437A RID: 17274
		[SerializeField]
		private WeaponTrickSlot[] trickSlotArray;

		// Token: 0x0400437B RID: 17275
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x0400437C RID: 17276
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x0400437D RID: 17277
		[SerializeField]
		private TooltipInvoker tipButtonConfirm;

		// Token: 0x0400437E RID: 17278
		[SerializeField]
		private PropertyItem propertyCostTime;

		// Token: 0x0400437F RID: 17279
		[SerializeField]
		private PropertyItem propertyCostResource;

		// Token: 0x04004380 RID: 17280
		[SerializeField]
		private ItemBack costItem;

		// Token: 0x04004381 RID: 17281
		[SerializeField]
		private CButton buttonChangeCostItem;

		// Token: 0x04004382 RID: 17282
		[SerializeField]
		private TextMeshProUGUI textCostItemName;

		// Token: 0x04004383 RID: 17283
		[SerializeField]
		private PropertyItem propertyToolDurability;

		// Token: 0x04004384 RID: 17284
		[SerializeField]
		private PropertyItem propertyNeedAttainment;

		// Token: 0x04004385 RID: 17285
		private ChangeWeaponTrickDisplayData _displayData;

		// Token: 0x04004386 RID: 17286
		private readonly List<sbyte> _trickList = new List<sbyte>();

		// Token: 0x04004387 RID: 17287
		private readonly List<sbyte> _trickOptionList = new List<sbyte>();

		// Token: 0x04004388 RID: 17288
		private ResourceInts _costResources;

		// Token: 0x04004389 RID: 17289
		private ItemKey _selectedItemKey;

		// Token: 0x0400438A RID: 17290
		private readonly List<short> _needMaterialTemplateIdList = new List<short>();

		// Token: 0x0400438B RID: 17291
		private readonly List<SelectedItemData> _selectedMaterialList = new List<SelectedItemData>();

		// Token: 0x0400438C RID: 17292
		private int _changeCountToOrigin;

		// Token: 0x0400438D RID: 17293
		private ProfessionData _professionData;

		// Token: 0x0400438E RID: 17294
		private CraftSkillsData _craftSkillsData;

		// Token: 0x0400438F RID: 17295
		private readonly List<ItemDisplayData> _allToolList = new List<ItemDisplayData>();

		// Token: 0x04004390 RID: 17296
		private readonly List<ITradeableContent> _toolList = new List<ITradeableContent>();

		// Token: 0x04004391 RID: 17297
		private readonly List<ITradeableContent> _availableToolList = new List<ITradeableContent>();

		// Token: 0x04004392 RID: 17298
		private readonly List<ITradeableContent> _notAvailableToolList = new List<ITradeableContent>();

		// Token: 0x04004393 RID: 17299
		private ItemDisplayData _emptyTool;

		// Token: 0x02001D12 RID: 7442
		private enum TogKey
		{
			// Token: 0x0400C4E9 RID: 50409
			Inventory,
			// Token: 0x0400C4EA RID: 50410
			Warehouse,
			// Token: 0x0400C4EB RID: 50411
			Treasury,
			// Token: 0x0400C4EC RID: 50412
			Stock
		}
	}
}
