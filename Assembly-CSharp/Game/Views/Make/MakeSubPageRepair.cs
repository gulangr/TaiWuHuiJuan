using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Building;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Make
{
	// Token: 0x02000958 RID: 2392
	public class MakeSubPageRepair : MakeSubPage
	{
		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x06007200 RID: 29184 RVA: 0x00350039 File Offset: 0x0034E239
		private ViewMake.ItemSourceTogKey CurTargetSourceTogKey
		{
			get
			{
				return (ViewMake.ItemSourceTogKey)this.targetSourceToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x06007201 RID: 29185 RVA: 0x00350046 File Offset: 0x0034E246
		private ItemSourceType CurTargetSource
		{
			get
			{
				return ViewMake.GetItemSourceType(this.CurTargetSourceTogKey);
			}
		}

		// Token: 0x17000CF8 RID: 3320
		// (get) Token: 0x06007202 RID: 29186 RVA: 0x00350053 File Offset: 0x0034E253
		private ItemDisplayData CurrentTool
		{
			get
			{
				return this.ParentView.SelectedTool;
			}
		}

		// Token: 0x17000CF9 RID: 3321
		// (get) Token: 0x06007203 RID: 29187 RVA: 0x00350060 File Offset: 0x0034E260
		private int TaiwuCharId
		{
			get
			{
				return this.ParentView.TaiwuCharId;
			}
		}

		// Token: 0x17000CFA RID: 3322
		// (get) Token: 0x06007204 RID: 29188 RVA: 0x0035006D File Offset: 0x0034E26D
		private BuildingModel BuildingModel
		{
			get
			{
				return this.ParentView.BuildingModel;
			}
		}

		// Token: 0x17000CFB RID: 3323
		// (get) Token: 0x06007205 RID: 29189 RVA: 0x0035007A File Offset: 0x0034E27A
		private sbyte CurLifeSkillType
		{
			get
			{
				return this.ParentView.CurLifeSkillType;
			}
		}

		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x06007206 RID: 29190 RVA: 0x00350087 File Offset: 0x0034E287
		private BuildingBlockKey BuildingBlockKey
		{
			get
			{
				return this.ParentView.BuildingBlockKey;
			}
		}

		// Token: 0x06007207 RID: 29191 RVA: 0x00350094 File Offset: 0x0034E294
		private List<ItemDisplayData> GetItemList(ItemSourceType source)
		{
			return this.ParentView.GetItemList(source);
		}

		// Token: 0x06007208 RID: 29192 RVA: 0x003500A2 File Offset: 0x0034E2A2
		private int GetResourceCount(sbyte type)
		{
			return SingletonObject.getInstance<BuildingModel>().GetResourceCount(type);
		}

		// Token: 0x06007209 RID: 29193 RVA: 0x003500B0 File Offset: 0x0034E2B0
		private void Awake()
		{
			this.targetSourceToggleGroup.OnActiveIndexChange += this.OnTargetSourceToggleChange;
			this.targetListScroll.Init("ViewMakeRepairTarget", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), new Action<ITradeableContent, RowItemLine>(this.OnItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Durability, null, null, null);
			this.targetListScroll.SetCustomBuildGroup(new Action(this.CustomBuildGroup), true);
			this.targetListScroll.SortAndFilterController.SetToggleVisible(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.targetListScroll.SortAndFilterController.SetToggleIsOnWithoutNotify(EFilterLine.MainFilter.ToInt(), EMainFilterKeys.Equip.ToInt());
			this.targetSourceToggleGroup.Init(-1);
			this.toolSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.Tool, new Action<int, ItemDisplayData>(this.OnCancelTool), new Action(this.OnClickToolSlot), new Func<bool>(this.GetToolInteractable), new Action<bool>(this.OnAutoSelectToggleChange), -1, null, false, null);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.targetSlot.Init(EMakeTargetSlotInteract.Custom, EMakeTargetSlotType.MakeTarget, new Action<int, ItemDisplayData>(this.OnCancelTarget), new Action(this.OnClickTargetSlot), new Func<bool>(this.GetTargetInteractable), null, -1, null, false, null);
			this.confirmBtn.ClearAndAddListener(new Action(this.ConfirmRepair));
		}

		// Token: 0x0600720A RID: 29194 RVA: 0x00350224 File Offset: 0x0034E424
		private void OnAutoSelectToggleChange(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake = isOn;
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
			bool flag = this.targetSlot.IsValid && isOn;
			if (flag)
			{
				this.ShowToolPanel();
			}
		}

		// Token: 0x0600720B RID: 29195 RVA: 0x00350264 File Offset: 0x0034E464
		private void CustomBuildGroup()
		{
			foreach (ITradeableContent item in this.targetListScroll.FilteredData)
			{
				bool flag = this.CheckBestToolAttainment(item as ItemDisplayData);
				if (flag)
				{
					this._availableTargetList.Add(item);
				}
				else
				{
					this._notAvailableTargetList.Add(item);
				}
			}
			string title = LanguageKey.LK_Building_Repair_Items_Available.Tr().ColorReplace();
			this.targetListScroll.AddGroup(0, title, this._availableTargetList, null, true);
			string title2 = LanguageKey.LK_Building_Repair_Items_UnAvailable.Tr().ColorReplace();
			this.targetListScroll.AddGroup(1, title2, this._notAvailableTargetList, null, true);
		}

		// Token: 0x0600720C RID: 29196 RVA: 0x00350338 File Offset: 0x0034E538
		public override void Init(ViewMake parentView)
		{
			base.Init(parentView);
			this._currentTarget = null;
			this.targetSlot.Select(this._currentTarget, false);
			this.toolSlot.Cancel();
			this.targetSlot.SetEffectHandlerState(true);
			this.toolSlot.SetEffectHandlerState(false);
			this.targetSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.targetPanel.gameObject.SetActive(false);
			parentView.ShowToolPanel(false);
			this.goDurabilityHolder.SetActive(false);
			this.goResourceRoot.SetActive(false);
			this.goLineGold.SetActive(false);
		}

		// Token: 0x0600720D RID: 29197 RVA: 0x003503E9 File Offset: 0x0034E5E9
		public override void RequestData()
		{
		}

		// Token: 0x0600720E RID: 29198 RVA: 0x003503EC File Offset: 0x0034E5EC
		public override void Refresh(BuildingMakeDisplayData displayData)
		{
			base.Refresh(displayData);
			this.toolSlot.IsToggleOn = SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake;
			this.ParentView.ShowToolPanel(this.targetSlot.IsValid);
			this.UpdateMaxToolAttainmentCache();
			this.RefreshAllViews();
		}

		// Token: 0x0600720F RID: 29199 RVA: 0x00350440 File Offset: 0x0034E640
		private void UpdateMaxToolAttainmentCache()
		{
			this._cachedMaxToolAttainment = 0;
			sbyte lifeSkillType = this.CurLifeSkillType;
			foreach (object obj in Enum.GetValues(typeof(ViewMake.ItemSourceTogKey)))
			{
				ViewMake.ItemSourceTogKey sourceTogKey = (ViewMake.ItemSourceTogKey)obj;
				List<ItemDisplayData> itemList = this.GetItemList(ViewMake.GetItemSourceType(sourceTogKey));
				bool flag = itemList == null;
				if (!flag)
				{
					foreach (ItemDisplayData item in itemList)
					{
						bool flag2 = item == null || item.Key.ItemType != 6;
						if (!flag2)
						{
							CraftToolItem toolConfig = CraftTool.Instance[item.Key.TemplateId];
							bool flag3 = toolConfig == null;
							if (!flag3)
							{
								bool flag4 = !toolConfig.RequiredLifeSkillTypes.Contains(lifeSkillType);
								if (!flag4)
								{
									short attainment = toolConfig.AttainmentBonus;
									bool flag5 = attainment > this._cachedMaxToolAttainment;
									if (flag5)
									{
										this._cachedMaxToolAttainment = attainment;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06007210 RID: 29200 RVA: 0x00350594 File Offset: 0x0034E794
		public override bool QuickHide()
		{
			return base.QuickHide();
		}

		// Token: 0x06007211 RID: 29201 RVA: 0x0035059C File Offset: 0x0034E79C
		private void OnTargetSourceToggleChange(int newIndex, int prevIndex)
		{
			this.RefreshTargetList();
		}

		// Token: 0x06007212 RID: 29202 RVA: 0x003505A8 File Offset: 0x0034E7A8
		private void OnItemClick(ITradeableContent content, RowItemLine line)
		{
			bool flag = this._currentTarget == content;
			if (flag)
			{
				this._currentTarget = null;
			}
			else
			{
				this._currentTarget = (content as ItemDisplayData);
			}
			this.targetSlot.Select(this._currentTarget, false);
			this.toolSlot.Refresh(false);
			this.CheckAndRefreshCondition();
			bool flag2 = this._currentTarget == null;
			if (flag2)
			{
				this.toolSlot.SetEffectHandlerState(false);
				this.goDurabilityHolder.SetActive(false);
				this.goResourceRoot.SetActive(false);
				this.ParentView.ShowToolPanel(false);
				this.goLineGold.SetActive(false);
			}
			else
			{
				this.goDurabilityHolder.SetActive(true);
				this.goResourceRoot.SetActive(true);
				bool isToggleOn = this.toolSlot.IsToggleOn;
				if (isToggleOn)
				{
					this.ShowToolPanel();
					this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.None);
				}
				else
				{
					this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
				}
				this.toolSlot.SetEffectHandlerState(true);
			}
		}

		// Token: 0x06007213 RID: 29203 RVA: 0x003506B4 File Offset: 0x0034E8B4
		private void OnItemRender(ITradeableContent content, RowItemLine rowItemLine)
		{
			ItemDisplayData itemData = content as ItemDisplayData;
			string curText = itemData.Durability.ToString().SetColor("pinkyellow");
			string maxText = itemData.MaxDurability.ToString().SetColor("pinkyellow");
			itemData.DurabilityChange = curText + "/" + maxText;
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(content);
			rowItemLine.Set(rowItemMain, true);
			bool isSelected = this._currentTarget == content;
			rowItemLine.SetSelected(isSelected);
			bool interactable = this.CheckBestToolAttainment(itemData);
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
			rowItemMain.HideInteractionState();
		}

		// Token: 0x06007214 RID: 29204 RVA: 0x00350759 File Offset: 0x0034E959
		private bool GetTargetInteractable()
		{
			return true;
		}

		// Token: 0x06007215 RID: 29205 RVA: 0x0035075C File Offset: 0x0034E95C
		private void OnClickTargetSlot()
		{
			this.targetPanel.gameObject.SetActive(true);
		}

		// Token: 0x06007216 RID: 29206 RVA: 0x00350774 File Offset: 0x0034E974
		private void OnCancelTarget(int index, ItemDisplayData itemDisplayData)
		{
			this._currentTarget = null;
			this.CheckAndRefreshCondition();
			this.toolSlot.Cancel();
			this.targetSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Enable);
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.targetSlot.SetEffectHandlerState(true);
			this.toolSlot.SetEffectHandlerState(false);
			this.targetPanel.gameObject.SetActive(false);
			this.goDurabilityHolder.SetActive(false);
			this.goResourceRoot.SetActive(false);
			this.ParentView.ShowToolPanel(false);
			this.goLineGold.SetActive(false);
		}

		// Token: 0x06007217 RID: 29207 RVA: 0x00350816 File Offset: 0x0034EA16
		private bool GetToolInteractable()
		{
			return this.targetSlot.IsValid;
		}

		// Token: 0x06007218 RID: 29208 RVA: 0x00350824 File Offset: 0x0034EA24
		private void OnCancelTool(int index, ItemDisplayData itemDisplayData)
		{
			this.ParentView.ClearToolList("");
			this.CheckAndRefreshCondition();
			this.toolSlot.ChangeButtonAddSprite(MakeTargetSlot.EMakeTargetSlotBtnAddState.Disable);
			this.ParentView.ShowToolPanel(false);
			this.goLineGold.SetActive(false);
		}

		// Token: 0x06007219 RID: 29209 RVA: 0x00350871 File Offset: 0x0034EA71
		private void OnClickToolSlot()
		{
			this.ShowToolPanel();
		}

		// Token: 0x0600721A RID: 29210 RVA: 0x0035087C File Offset: 0x0034EA7C
		private void ShowToolPanel()
		{
			bool flag = this.targetSlot.ItemData == null;
			if (flag)
			{
				this.ParentView.ClearToolList("");
			}
			else
			{
				int needAttainment = this.GetNeedAttainment();
				sbyte lifeSkillType = this.CurLifeSkillType;
				this._toolDurabilityCost = this.CalculateToolDurabilityCost();
				this.ParentView.RefreshToolList(needAttainment, new List<sbyte>
				{
					lifeSkillType
				}, new List<List<sbyte>>
				{
					new List<sbyte>((int)this.targetSlot.ItemData.Grade)
				}, this.CurrentTool ?? this.toolSlot.ItemData, new Action<ItemDisplayData>(this.OnToolSelected), this.toolSlot.IsToggleOn, 1);
				this.ParentView.ShowToolPanel(true);
			}
		}

		// Token: 0x0600721B RID: 29211 RVA: 0x00350942 File Offset: 0x0034EB42
		private void OnToolSelected(ItemDisplayData tool)
		{
			this.toolSlot.Select(tool, false);
			this.CheckAndRefreshCondition();
			this.goDurabilityHolder.SetActive(true);
			this.goResourceRoot.SetActive(true);
			this.goLineGold.SetActive(true);
		}

		// Token: 0x0600721C RID: 29212 RVA: 0x00350981 File Offset: 0x0034EB81
		private void OnEnable()
		{
		}

		// Token: 0x0600721D RID: 29213 RVA: 0x00350984 File Offset: 0x0034EB84
		private void RefreshAllViews()
		{
			this.ParentView.RefreshSourceToggleInteractable(this.targetSourceToggleGroup);
			this.targetSlot.Refresh(false);
			this.toolSlot.Refresh(false);
			this.RefreshTargetList();
			this.ShowToolPanel();
			this.CheckAndRefreshCondition();
		}

		// Token: 0x0600721E RID: 29214 RVA: 0x003509D3 File Offset: 0x0034EBD3
		private void CheckAndRefreshCondition()
		{
			this.RefreshConditionDisplay();
			this.CheckCondition();
		}

		// Token: 0x0600721F RID: 29215 RVA: 0x003509E4 File Offset: 0x0034EBE4
		private void RefreshTargetList()
		{
			List<ItemDisplayData> itemList = this.GetItemList(this.CurTargetSource) ?? new List<ItemDisplayData>();
			itemList = itemList.Concat(this.GetItemList(ItemSourceType.Equipment) ?? new List<ItemDisplayData>()).ToList<ItemDisplayData>();
			this._targetList.Clear();
			this._availableTargetList.Clear();
			this._notAvailableTargetList.Clear();
			this._targetList.AddRange(itemList.Where(new Func<ItemDisplayData, bool>(this.FilterRepairableItem)).ToList<ItemDisplayData>());
			this.targetListScroll.SetItemList(this._targetList);
		}

		// Token: 0x06007220 RID: 29216 RVA: 0x00350A7D File Offset: 0x0034EC7D
		private void RefreshConditionDisplay()
		{
			this.RefreshResourceRequirement();
			this.RefreshDurabilityDisplay();
			this.RefreshAttainmentDisplay();
			this.RefreshWarning();
		}

		// Token: 0x06007221 RID: 29217 RVA: 0x00350A9C File Offset: 0x0034EC9C
		private void RefreshDurabilityDisplay()
		{
			this.targetDurabilityLabel.gameObject.SetActive(this._currentTarget != null);
			this.durabilityOriginalLabel.gameObject.SetActive(this._currentTarget != null);
			this.durabilityPreiviewLabel.gameObject.SetActive(this._currentTarget != null);
			bool flag = this._currentTarget == null;
			if (!flag)
			{
				this.targetDurabilityLabel.text = LanguageKey.LK_Building_Repair_Durability.TrFormat(this._currentTarget.Durability, this._currentTarget.MaxDurability);
				this.durabilityOriginalLabel.text = LanguageKey.LK_Building_Repair_Durability.TrFormat(this._currentTarget.Durability, this._currentTarget.MaxDurability);
				this.durabilityPreiviewLabel.text = LanguageKey.LK_Building_Repair_Durability.TrFormat(this._currentTarget.MaxDurability, this._currentTarget.MaxDurability);
			}
		}

		// Token: 0x06007222 RID: 29218 RVA: 0x00350BAC File Offset: 0x0034EDAC
		private void RefreshAttainmentDisplay()
		{
			this.goNeedAttainment.SetActive(this._currentTarget != null);
			bool flag = this._currentTarget == null;
			if (!flag)
			{
				sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				LifeSkillTypeItem lifeSkillConfig = LifeSkillType.Instance[lifeSkillType];
				float factor = (this._currentTarget.Durability == 0) ? 1f : 0.5f;
				short requiredAttainment = (short)((float)GlobalConfig.Instance.RepairAttainments[(int)grade] * factor);
				this.imageNeedAttainment.SetSprite(lifeSkillConfig.Icon, false, null);
				this.attainmentRequirementLabel.text = LanguageKey.LK_Heal_AttainmentTip_Attainment.TrFormat(lifeSkillConfig.Name, requiredAttainment.ToString());
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.goNeedAttainment.GetComponent<RectTransform>());
			}
		}

		// Token: 0x06007223 RID: 29219 RVA: 0x00350CAF File Offset: 0x0034EEAF
		private void RefreshWarning()
		{
			this.warning.gameObject.SetActive(false);
		}

		// Token: 0x06007224 RID: 29220 RVA: 0x00350CC4 File Offset: 0x0034EEC4
		private unsafe short GetLifeSkillTotalAttainment(sbyte lifeSkillType)
		{
			short attainment = *this.DisplayData.LifeSkillAttainments[(int)lifeSkillType];
			bool flag = this.CurrentTool == null;
			short result;
			if (flag)
			{
				result = attainment;
			}
			else
			{
				bool flag2 = this.IsEmptyTool(this.CurrentTool);
				if (flag2)
				{
					int bonus = this.GetEmptyToolAttainmentBonus(lifeSkillType);
					result = (short)((int)attainment + (int)attainment * bonus / 100);
				}
				else
				{
					CraftToolItem toolConfig = CraftTool.Instance[this.CurrentTool.Key.TemplateId];
					bool flag3 = toolConfig != null && toolConfig.RequiredLifeSkillTypes.Contains(lifeSkillType);
					if (flag3)
					{
						short toolAttainment = this.GetToolAttainment(this.CurrentTool.Key.TemplateId, lifeSkillType);
						result = attainment + toolAttainment;
					}
					else
					{
						result = attainment;
					}
				}
			}
			return result;
		}

		// Token: 0x06007225 RID: 29221 RVA: 0x00350D7C File Offset: 0x0034EF7C
		private int GetEmptyToolAttainmentBonus(sbyte lifeSkillType)
		{
			return -50;
		}

		// Token: 0x06007226 RID: 29222 RVA: 0x00350D90 File Offset: 0x0034EF90
		private short GetToolAttainment(short toolTemplateId, sbyte lifeSkillType)
		{
			bool flag = toolTemplateId < 0;
			short result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				CraftToolItem toolConfig = CraftTool.Instance[toolTemplateId];
				result = ((toolConfig != null) ? toolConfig.AttainmentBonus : 0);
			}
			return result;
		}

		// Token: 0x06007227 RID: 29223 RVA: 0x00350DC8 File Offset: 0x0034EFC8
		private void RefreshResourceRequirement()
		{
			this.resourceRequirementHolder.gameObject.SetActive(this._currentTarget != null);
			bool flag = this._currentTarget == null;
			if (!flag)
			{
				sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				float factor = (this._currentTarget.Durability == 0) ? 1f : 0.5f;
				short baseResource = GlobalConfig.Instance.RepairBaseResourseRequirement[(int)grade];
				int resourceTypeCount = 0;
				for (sbyte i = 0; i < 6; i += 1)
				{
					short curCount = this._currentTarget.MaterialResources.Get((int)i);
					bool flag2 = curCount > 0;
					if (flag2)
					{
						resourceTypeCount++;
					}
				}
				float resourceHolderHeight = 40f + (float)resourceTypeCount * 64f;
				this.goResourceRoot.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(298f, resourceHolderHeight));
				CommonUtils.PrepareEnoughChildren(this.resourceRequirementHolder, this.resourceRequirementHolder.GetChild(0).gameObject, resourceTypeCount, null);
				int index = 0;
				this._makeRequiredResourceList.Clear();
				for (sbyte j = 0; j < 6; j += 1)
				{
					short curCount2 = this._currentTarget.MaterialResources.Get((int)j);
					int requiredResource = (int)((float)curCount2 * factor * (float)baseResource);
					this._makeRequiredResourceList.Add(requiredResource);
					bool flag3 = curCount2 <= 0;
					if (!flag3)
					{
						int ownedResource = this.GetResourceCount(j);
						ResourceCostItem item = this.resourceRequirementHolder.GetChild(index).GetComponent<ResourceCostItem>();
						item.Set(j, requiredResource, ownedResource);
						item.gameObject.SetActive(true);
						index++;
					}
				}
			}
		}

		// Token: 0x06007228 RID: 29224 RVA: 0x00350F90 File Offset: 0x0034F190
		public void CheckCondition()
		{
			bool res = true;
			TooltipInvoker tip = this.confirmBtn.GetComponent<TooltipInvoker>();
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			this.confirmBtn.interactable = false;
			bool flag = this._currentTarget == null;
			if (flag)
			{
				res = false;
				sb.AppendLine(LanguageKey.LK_Building_Repair_UnselectTarget.Tr().SetColor("brightred"));
			}
			else
			{
				bool flag2 = this._currentTarget.Durability >= this._currentTarget.MaxDurability;
				if (flag2)
				{
					res = false;
				}
				bool flag3 = !this.CheckResourceSufficient();
				if (flag3)
				{
					res = false;
					sb.AppendLine(LanguageKey.LK_Building_Repair_Resource_NotEnough.Tr().SetColor("brightred"));
				}
				bool flag4 = !this.CheckToolSufficient();
				if (flag4)
				{
					res = false;
					sb.AppendLine(LanguageKey.LK_Building_Repair_UnselectTool.Tr().SetColor("brightred"));
				}
				bool flag5 = !this.CheckAttainment();
				if (flag5)
				{
					res = false;
					sb.AppendLine(LanguageKey.LK_Building_Repair_Attainment_NotMeet.Tr().SetColor("brightred"));
				}
			}
			tip.PresetParam[0] = sb.ToString();
			tip.enabled = (!res && sb.Length > 0);
			this.confirmBtn.interactable = res;
			EasyPool.Free<StringBuilder>(sb);
		}

		// Token: 0x06007229 RID: 29225 RVA: 0x003510DC File Offset: 0x0034F2DC
		private bool CheckResourceSufficient()
		{
			bool flag = this._currentTarget == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (sbyte i = 0; i < 6; i += 1)
				{
					int required = this._makeRequiredResourceList[(int)i];
					bool flag2 = required <= 0;
					if (!flag2)
					{
						int curResource = this.BuildingModel.GetResourceCount(i);
						bool flag3 = curResource < required;
						if (flag3)
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600722A RID: 29226 RVA: 0x00351150 File Offset: 0x0034F350
		private bool CheckToolSufficient()
		{
			bool flag = this.CurrentTool == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.IsEmptyTool(this.CurrentTool);
				result = (flag2 || this.CurrentTool.Durability >= this._toolDurabilityCost);
			}
			return result;
		}

		// Token: 0x0600722B RID: 29227 RVA: 0x003511A0 File Offset: 0x0034F3A0
		private short CalculateToolDurabilityCost()
		{
			bool flag = this._currentTarget == null || this.CurrentTool == null || this.IsEmptyTool(this.CurrentTool);
			short result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				result = CraftTool.Instance[this.CurrentTool.Key.TemplateId].DurabilityCost[(int)grade];
			}
			return result;
		}

		// Token: 0x0600722C RID: 29228 RVA: 0x00351220 File Offset: 0x0034F420
		public void ConfirmRepair()
		{
			bool flag = this.CurrentTool == null || this._currentTarget == null;
			if (!flag)
			{
				BuildingDomainMethod.AsyncCall.CheckRepairConditionIsMeet(null, this.TaiwuCharId, this.CurrentTool.Key, this._currentTarget.Key, this.BuildingBlockKey, delegate(int offset, RawDataPool pool)
				{
					bool isMeet = false;
					Serializer.Deserialize(pool, offset, ref isMeet);
					bool flag2 = !isMeet;
					if (!flag2)
					{
						BuildingDomainMethod.AsyncCall.RepairItemOptional(null, this.TaiwuCharId, this.CurrentTool.Key, this._currentTarget.Key, this.CurrentTool.ItemSourceType, delegate(int offset2, RawDataPool pool2)
						{
							Serializer.Deserialize(pool2, offset2, ref this._resultItemDisplayData);
							ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
							ItemDisplayData itemData = new ItemDisplayData
							{
								Key = this._resultItemDisplayData.Key,
								Amount = 1
							};
							argBox.SetObject("ItemList", new List<ItemDisplayData>
							{
								itemData
							});
							UIElement.GetItem.SetOnInitArgs(argBox);
							UIManager.Instance.MaskUI(UIElement.GetItem);
							this._currentTarget = this._resultItemDisplayData;
							this.targetSlot.Select(this._currentTarget, false);
							this.ParentView.RequestData();
						});
					}
				});
			}
		}

		// Token: 0x0600722D RID: 29229 RVA: 0x0035127D File Offset: 0x0034F47D
		private bool IsEmptyTool(ItemDisplayData data)
		{
			return data != null && this.IsEmptyTool(data.Key);
		}

		// Token: 0x0600722E RID: 29230 RVA: 0x00351291 File Offset: 0x0034F491
		private bool IsEmptyTool(ItemKey itemKey)
		{
			return ItemTemplateHelper.IsEmptyTool(itemKey.ItemType, itemKey.TemplateId);
		}

		// Token: 0x0600722F RID: 29231 RVA: 0x003512A4 File Offset: 0x0034F4A4
		private bool FilterRepairableItem(ItemDisplayData d)
		{
			bool flag = !d.Key.IsValid();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !ItemTemplateHelper.IsRepairable(d.Key.ItemType, d.Key.TemplateId);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this._currentTarget != null && d.ContainsItemKey(this._currentTarget.Key);
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = d.Durability >= d.MaxDurability;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = this.CurLifeSkillType != -1 && this.CurLifeSkillType != ItemTemplateHelper.GetCraftRequiredLifeSkillType(d.Key.ItemType, d.Key.TemplateId);
							result = !flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06007230 RID: 29232 RVA: 0x0035137C File Offset: 0x0034F57C
		private unsafe bool CheckBestToolAttainment(ItemDisplayData itemData)
		{
			bool flag = itemData == null || !itemData.Key.IsValid();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
				sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
				float factor = (itemData.Durability == 0) ? 1f : 0.5f;
				short requiredAttainment = (short)((float)GlobalConfig.Instance.RepairAttainments[(int)grade] * factor);
				short currentAttainment = *this.DisplayData.LifeSkillAttainments[(int)lifeSkillType];
				bool flag2 = currentAttainment >= requiredAttainment;
				if (flag2)
				{
					result = true;
				}
				else
				{
					int needToolAttainment = (int)(requiredAttainment - currentAttainment);
					short maxToolAttainment = this.GetMaxToolAttainmentBonus(lifeSkillType);
					result = ((int)maxToolAttainment >= needToolAttainment);
				}
			}
			return result;
		}

		// Token: 0x06007231 RID: 29233 RVA: 0x00351454 File Offset: 0x0034F654
		private bool CheckAttainment()
		{
			short currentAttainment = this.GetLifeSkillTotalAttainment(this.ParentView.CurLifeSkillType);
			sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
			float factor = (this._currentTarget.Durability == 0) ? 1f : 0.5f;
			short requiredAttainment = (short)((float)GlobalConfig.Instance.RepairAttainments[(int)grade] * factor);
			return currentAttainment >= requiredAttainment;
		}

		// Token: 0x06007232 RID: 29234 RVA: 0x003514D4 File Offset: 0x0034F6D4
		private short GetMaxToolAttainmentBonus(sbyte lifeSkillType)
		{
			return this._cachedMaxToolAttainment;
		}

		// Token: 0x06007233 RID: 29235 RVA: 0x003514EC File Offset: 0x0034F6EC
		private unsafe int CalculateMinToolAttainmentBonus()
		{
			sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
			int requiredAttainment = this.GetNeedAttainment();
			short currentAttainment = *this.DisplayData.LifeSkillAttainments[(int)lifeSkillType];
			int minToolAttainment = requiredAttainment - (int)currentAttainment;
			return Math.Max(0, minToolAttainment);
		}

		// Token: 0x06007234 RID: 29236 RVA: 0x0035154C File Offset: 0x0034F74C
		private int GetNeedAttainment()
		{
			bool flag = this._currentTarget == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				float factor = (this._currentTarget.Durability == 0) ? 1f : 0.5f;
				short requiredAttainment = (short)((float)GlobalConfig.Instance.RepairAttainments[(int)grade] * factor);
				result = (int)requiredAttainment;
			}
			return result;
		}

		// Token: 0x04005485 RID: 21637
		[SerializeField]
		private CToggleGroup targetSourceToggleGroup;

		// Token: 0x04005486 RID: 21638
		[SerializeField]
		private ItemListScroll targetListScroll;

		// Token: 0x04005487 RID: 21639
		[SerializeField]
		private GameObject targetPanel;

		// Token: 0x04005488 RID: 21640
		[SerializeField]
		private MakeTargetSlot targetSlot;

		// Token: 0x04005489 RID: 21641
		[SerializeField]
		private MakeTargetSlot toolSlot;

		// Token: 0x0400548A RID: 21642
		[SerializeField]
		private RectTransform warning;

		// Token: 0x0400548B RID: 21643
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x0400548C RID: 21644
		[SerializeField]
		private CImage imageNeedAttainment;

		// Token: 0x0400548D RID: 21645
		[SerializeField]
		private TextMeshProUGUI targetDurabilityLabel;

		// Token: 0x0400548E RID: 21646
		[SerializeField]
		private TextMeshProUGUI attainmentRequirementLabel;

		// Token: 0x0400548F RID: 21647
		[SerializeField]
		private TextMeshProUGUI durabilityOriginalLabel;

		// Token: 0x04005490 RID: 21648
		[SerializeField]
		private TextMeshProUGUI durabilityPreiviewLabel;

		// Token: 0x04005491 RID: 21649
		[SerializeField]
		private RectTransform resourceRequirementHolder;

		// Token: 0x04005492 RID: 21650
		[SerializeField]
		private GameObject goResourceRoot;

		// Token: 0x04005493 RID: 21651
		[SerializeField]
		private GameObject goDurabilityHolder;

		// Token: 0x04005494 RID: 21652
		[SerializeField]
		private GameObject goLineGold;

		// Token: 0x04005495 RID: 21653
		[SerializeField]
		private GameObject goNeedAttainment;

		// Token: 0x04005496 RID: 21654
		private ItemDisplayData _currentTarget;

		// Token: 0x04005497 RID: 21655
		private short _toolDurabilityCost;

		// Token: 0x04005498 RID: 21656
		private readonly List<int> _makeRequiredResourceList = new List<int>(6);

		// Token: 0x04005499 RID: 21657
		private ItemDisplayData _resultItemDisplayData;

		// Token: 0x0400549A RID: 21658
		private short _cachedMaxToolAttainment;

		// Token: 0x0400549B RID: 21659
		private readonly List<ITradeableContent> _availableTargetList = new List<ITradeableContent>();

		// Token: 0x0400549C RID: 21660
		private readonly List<ITradeableContent> _notAvailableTargetList = new List<ITradeableContent>();

		// Token: 0x0400549D RID: 21661
		private readonly List<ITradeableContent> _targetList = new List<ITradeableContent>();

		// Token: 0x0400549E RID: 21662
		private const float BaseResourceHolderHeight = 40f;

		// Token: 0x0400549F RID: 21663
		private const float ResourceHolderMaxHeight = 298f;

		// Token: 0x040054A0 RID: 21664
		private const float ResourceItemHeight = 64f;
	}
}
