using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class UI_MakeMedicine : UIBase
{
	// Token: 0x06002C34 RID: 11316 RVA: 0x0015A288 File Offset: 0x00158488
	public override void OnInit(ArgumentBox argsBox)
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		short settlementId = SingletonObject.getInstance<WorldMapModel>().GetTaiwuCharOnSettlement();
		this._isSettlement = (settlementId >= 0);
		this._itemViewToolSelected = base.CGet<ItemView>("ItemViewToolSelected");
		this._itemViewTargetSelected = base.CGet<ItemView>("ItemViewTargetSelected");
		this._buttonConfirm = base.CGet<CButtonObsolete>("ButtonConfirm");
		this._confirmNormalLabel = base.CGet<TextMeshProUGUI>("ConfirmNormalLabel");
		this._confirmDisableLabel = base.CGet<TextMeshProUGUI>("ConfirmDisableLabel");
		this._itemViewToolSelected.GetComponent<CButtonObsolete>().ClearAndAddListener(new Action(this.OnClickToolItemView));
		this._itemViewTargetSelected.GetComponent<CButtonObsolete>().ClearAndAddListener(new Action(this.OnClickTargetItemView));
		this._toolDurabilityText = base.CGet<TextMeshProUGUI>("ToolDurabilityText");
		this._requireLifeSkillList = base.CGet<RectTransform>("RequireLifeSkillList");
		this._makePanel = base.CGet<Refers>("Make");
		this._previewTip = base.CGet<TooltipInvoker>("PreviewTip");
		this._previewTip.gameObject.SetActive(true);
		this.InitItemView();
		this.InitMake();
		this.NeedDataListenerId = true;
		this._itemViewToolSelected.gameObject.SetActive(false);
		this._itemViewTargetSelected.gameObject.SetActive(false);
		this._itemCacheTools.Clear();
		this._itemCacheTargets.Clear();
		this._buttonConfirm.interactable = false;
		this._currentTarget = null;
		this._currentTool = null;
		base.CGet<TextMeshProUGUI>("TipContent").text = LocalStringManager.GetFormat(LanguageKey.LK_MakeMedicine_Tip, 3, 1);
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x0015A438 File Offset: 0x00158638
	private void InitItemView()
	{
		this._itemViewMaterial = base.CGet<ItemScrollView>("ItemViewMaterial");
		this._itemViewMaterial.Init();
		this._itemViewMaterial.SetCharId(this._taiwuCharId);
		this._itemViewMaterial.SetItemList(ref this._inventoryItems, true, "UI_Make._itemViewMaterial", false, new Action<ItemDisplayData, ItemView>(this.OnRenderItemTarget));
		this._itemViewTools = base.CGet<ItemScrollView>("ItemViewTools");
		this._itemViewTools.Init();
		this._itemViewTools.SetCharId(this._taiwuCharId);
		this._itemViewTools.SetItemList(ref this._inventoryItems, true, "UI_Make._itemViewTools", false, new Action<ItemDisplayData, ItemView>(this.OnRenderItemTool));
		this._itemViewTools.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Make);
		this._itemViewTools.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Make, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		this._toolTogGroup = base.CGet<CToggleGroupObsolete>("ToolTogGroup");
		this._toolTogGroup.InitPreOnToggle(-1);
		this._toolTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToolTogChange);
		this._materialTogGroup = base.CGet<CToggleGroupObsolete>("MaterialTogGroup");
		this._materialTogGroup.InitPreOnToggle(-1);
		this._materialTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnMaterialTogChange);
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x0015A57D File Offset: 0x0015877D
	private void OnMaterialTogChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this.ChangeCurrentTarget(this._currentTarget, false);
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x0015A58E File Offset: 0x0015878E
	private void OnToolTogChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this.ChangeCurrentTool(this._currentTool);
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x0015A59E File Offset: 0x0015879E
	private void OnEnable()
	{
		this.<OnEnable>g__InitTogGroup|43_0(this._toolTogGroup);
		this.<OnEnable>g__InitTogGroup|43_0(this._materialTogGroup);
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x0015A5BD File Offset: 0x001587BD
	private void OnDisable()
	{
		this.PlayCenterAnim(false, false);
		this.ChangeCurrentTarget(null, false);
		this.ChangeCurrentTool(null);
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x0015A5DC File Offset: 0x001587DC
	private void InitMake()
	{
		this._makeCountButtonLess = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonLess");
		this._makeCountButtonLess.ClearAndAddListener(delegate
		{
			this.ChangeMakeCount(false);
		});
		this._makeCountButtonMore = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonMore");
		this._makeCountButtonMore.ClearAndAddListener(delegate
		{
			this.ChangeMakeCount(true);
		});
		this._makeCountButtonMin = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonMin");
		this._makeCountButtonMin.ClearAndAddListener(delegate
		{
			this.ChangeMakeCountRange(false);
		});
		this._makeCountButtonMax = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonMax");
		this._makeCountButtonMax.ClearAndAddListener(delegate
		{
			this.ChangeMakeCountRange(true);
		});
		this._makePanel.gameObject.SetActive(false);
		this._makeCountButtonLess.interactable = false;
		this._makeCountButtonMore.interactable = false;
		this._makeCountButtonMin.interactable = false;
		this._makeCountButtonMax.interactable = false;
		this._confirmButtonTipDisplayer = this._buttonConfirm.GetComponent<TooltipInvoker>();
		this._confirmButtonTipDisplayer.PresetParam = new string[1];
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x0015A720 File Offset: 0x00158920
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			97U
		}));
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			GameDataBridge.AddMethodCall(this.Element.GameDataListenerId, 6, 29);
			this.RefreshAllItems();
		}));
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x0015A77C File Offset: 0x0015897C
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
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 27;
						if (flag2)
						{
							this._inventoryItems.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
						}
					}
					else
					{
						bool flag3 = notification.DomainId == 5;
						if (flag3)
						{
							bool flag4 = notification.MethodId == 15;
							if (flag4)
							{
								this._warehouseItems.Clear();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseItems);
							}
							else
							{
								bool flag5 = notification.MethodId == 139;
								if (flag5)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._villagerNeededItemSet);
								}
								else
								{
									bool flag6 = notification.MethodId == 64;
									if (flag6)
									{
										this._treasuryItems.Clear();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._treasuryItems);
										this._allItems.Clear();
										this._allItems.AddRange(this._inventoryItems);
										this._allItems.AddRange(this._warehouseItems);
										this._allItems.AddRange(this._treasuryItems);
										this.Element.ShowAfterRefresh();
										this.OnItemsDataRefresh();
									}
								}
							}
						}
						else
						{
							bool flag7 = notification.DomainId == 6;
							if (flag7)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._emptyToolKey);
							}
						}
					}
				}
			}
			else
			{
				bool flag8 = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0;
				if (flag8)
				{
					uint subId = notification.Uid.SubId1;
					uint num = subId;
					if (num == 97U)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._lifeSkillAttainments);
					}
				}
			}
		}
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x0015A9E0 File Offset: 0x00158BE0
	private void OnItemsDataRefresh()
	{
		CToggleGroupObsolete toolTogGroup = this._toolTogGroup;
		CToggleObsolete active = this._toolTogGroup.GetActive();
		toolTogGroup.Set((active != null) ? active.Key : 0, true, true);
		CToggleGroupObsolete materialTogGroup = this._materialTogGroup;
		CToggleObsolete active2 = this._materialTogGroup.GetActive();
		materialTogGroup.Set((active2 != null) ? active2.Key : 0, true, true);
		bool flag = this._currentTool != null;
		if (flag)
		{
			ItemDisplayData tool = this.IsEmptyTool(this._currentTool) ? this._currentTool : this._allItems.Find((ItemDisplayData d) => this._currentTool != null && d.Key.Id == this._currentTool.Key.Id);
			this.ChangeCurrentTool(tool);
		}
		bool flag2 = this._currentTarget != null;
		if (flag2)
		{
			ItemDisplayData searchData = this._currentTarget;
			ItemDisplayData target = this._allItems.Find((ItemDisplayData d) => d.Key.TemplateEquals(searchData.Key) && d.PoisonEffects == searchData.PoisonEffects) ?? this._allItems.Find((ItemDisplayData d) => d.Key.TemplateEquals(searchData.Key));
			this.ChangeCurrentTarget(target, true);
		}
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x0015AAE0 File Offset: 0x00158CE0
	private void OnClickToolItemView()
	{
		bool flag = this._currentTool != null;
		if (flag)
		{
			this.ChangeCurrentTool(null);
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x0015AB04 File Offset: 0x00158D04
	private void OnClickTargetItemView()
	{
		bool flag = this._currentTarget != null;
		if (flag)
		{
			this.ChangeCurrentTarget(null, false);
		}
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x0015AB28 File Offset: 0x00158D28
	private void OnRenderItemTarget(ItemDisplayData itemData, ItemView itemView)
	{
		this.SetVillagerNeedMark(itemView, itemData.ItemSourceTypeEnum);
		short num;
		bool canMake = ItemTemplateHelper.CanMedicineUpgrade(itemData.Key.ItemType, itemData.Key.TemplateId, out num);
		bool flag = !canMake;
		if (flag)
		{
			itemView.SetLocked(true);
		}
		else
		{
			itemView.SetClickEvent(delegate
			{
				bool flag2 = this._currentTarget == null || !this._currentTarget.ContainsItemKey(itemData.Key);
				if (flag2)
				{
					this.ChangeCurrentTarget(itemData, false);
				}
			});
		}
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x0015ABAC File Offset: 0x00158DAC
	private void OnRenderItemTool(ItemDisplayData itemData, ItemView itemView)
	{
		this.SetVillagerNeedMark(itemView, itemData.ItemSourceTypeEnum);
		itemView.SetClickEvent(delegate
		{
			bool flag2 = this._currentTool == null || !this._currentTool.ContainsItemKey(itemData.Key);
			if (flag2)
			{
				this.ChangeCurrentTool((itemData == this._invalidItemDisplayData) ? itemData : itemData.Clone(-1));
			}
		});
		bool flag = itemData.Key.IsValid() && !this.IsEmptyTool(itemData);
		if (flag)
		{
			itemView.ShowDurability();
		}
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x0015AC28 File Offset: 0x00158E28
	private void ChangeMakeCount(bool isMore)
	{
		if (isMore)
		{
			this._makeCount += 1;
		}
		else
		{
			this._makeCount -= 1;
		}
		this.CheckMakeCondition();
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x0015AC68 File Offset: 0x00158E68
	private void ChangeMakeCountRange(bool isMax)
	{
		if (isMax)
		{
			while (this._makeCountButtonMax.interactable)
			{
				this.ChangeMakeCount(true);
			}
		}
		else
		{
			while (this._makeCountButtonMin.interactable)
			{
				this.ChangeMakeCount(false);
			}
		}
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x0015ACB8 File Offset: 0x00158EB8
	private void RefreshAllItems()
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId);
		TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetTreasuryNeededItemList(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x0015AD10 File Offset: 0x00158F10
	private bool CheckTargetHasChanged(ItemDisplayData target)
	{
		bool flag = target != this._currentTarget && (target == null || this._currentTarget == null);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = target != null && this._currentTarget != null;
			if (flag2)
			{
				bool flag3 = !target.ContainsItemKey(this._currentTarget.Key);
				if (flag3)
				{
					return true;
				}
				bool flag4 = target.PoisonEffects != this._currentTarget.PoisonEffects;
				if (flag4)
				{
					return true;
				}
				bool flag5 = !target.RefiningEffects.Equals(this._currentTarget.RefiningEffects);
				if (flag5)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x0015ADC8 File Offset: 0x00158FC8
	private bool CheckTool(short reduce = 0)
	{
		this._toolDurabilityCost = reduce;
		this._itemViewToolSelected.SetUsedDurability(reduce, false);
		bool flag = this._currentTool == null;
		bool result;
		if (flag)
		{
			this.SetConfirmButtonTip(7692, true, TipType.SingleDesc, null);
			result = false;
		}
		else
		{
			bool flag2 = this.IsEmptyTool(this._currentTool);
			if (flag2)
			{
				this.SetConfirmButtonTip(-1, true, TipType.SingleDesc, null);
				result = true;
			}
			else
			{
				bool flag3 = this._currentTool.Durability < reduce;
				if (flag3)
				{
					this.SetConfirmButtonTip(7689, true, TipType.SingleDesc, null);
					result = false;
				}
				else
				{
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x0015AE58 File Offset: 0x00159058
	private void ChangeCurrentTool(ItemDisplayData target)
	{
		this._toolDurabilityCost = 0;
		this._itemViewToolSelected.SetUsedDurability(0, false);
		bool hasChanged = (target != this._currentTool && (target == null || this._currentTool == null)) || (target != null && this._currentTool != null && !target.Key.Equals(this._currentTool.Key));
		bool dontStopAnim = false;
		this._currentTool = target;
		bool flag = this._currentTool != null;
		if (flag)
		{
			dontStopAnim = (target != null && target.Key.Id == this._currentTool.Key.Id);
			this._itemViewToolSelected.gameObject.SetActive(true);
			this._itemViewToolSelected.SetData(this._currentTool, false, 1, false, true, null, false, true);
			bool isEmptyTool = this.IsEmptyTool(this._currentTool);
			bool flag2 = !isEmptyTool;
			if (flag2)
			{
				this._itemViewToolSelected.ShowDurability();
			}
			bool flag3 = isEmptyTool;
			if (flag3)
			{
				LanguageKey key = ((float)this._currentTool.Durability > (float)this._currentTool.MaxDurability * 0.5f) ? LanguageKey.LK_Tool_Durability_Sufficient : LanguageKey.LK_Tool_Durability_Insufficient;
				this._toolDurabilityText.text = LocalStringManager.GetFormat(key, this._currentTool.Durability, this._currentTool.MaxDurability);
			}
			else
			{
				this._toolDurabilityText.text = LocalStringManager.Get(LanguageKey.LK_Tool_Durability_Infinity);
			}
			this.RefreshToolAttainment(true);
			this.PlayConditionAnim(false, true);
		}
		else
		{
			this._itemViewToolSelected.gameObject.SetActive(false);
			this._toolDurabilityText.transform.parent.gameObject.SetActive(false);
			this.RefreshToolAttainment(false);
			this.PlayConditionAnim(false, false);
		}
		this._itemCacheTools.Clear();
		List<ItemDisplayData> sourceItems = this.GetItemsSource(this._toolTogGroup);
		bool flag4 = this._currentTool == null || !this.IsEmptyTool(this._currentTool);
		if (flag4)
		{
			this._itemCacheTools.Add(new ItemDisplayData
			{
				Key = this._emptyToolKey
			});
		}
		this._itemViewTools.SortAndFilter.StaticAheadItemKeysList.Clear();
		this._itemViewTools.SortAndFilter.StaticAheadItemKeysList.Add(this._emptyToolKey);
		this._itemCacheTools.AddRange(from d in sourceItems
		where (target == null || !d.ContainsItemKey(target.Key)) && d.Key.ItemType == 6 && CraftTool.Instance[d.Key.TemplateId].RequiredLifeSkillTypes.Contains(this._curLifeSkillType)
		select d);
		this._itemViewTools.SetItemList(ref this._itemCacheTools, false, null, false, null);
		bool flag5 = this._currentTarget != null;
		if (flag5)
		{
			this.CheckCondition(hasChanged);
		}
		bool flag6 = !dontStopAnim;
		if (flag6)
		{
			this.PlayCenterAnim(false, false);
		}
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x0015B150 File Offset: 0x00159350
	private void RefreshToolAttainment(bool show)
	{
		RectTransform layout = base.CGet<RectTransform>("ToolLifeSkillList");
		layout.gameObject.SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			CraftToolItem toolConfig = CraftTool.Instance[this._currentTool.Key.TemplateId];
			for (int i = 0; i < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; i++)
			{
				sbyte skillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[i];
				bool showSkill = toolConfig.RequiredLifeSkillTypes.Contains(skillType);
				bool flag2 = showSkill;
				if (flag2)
				{
					showSkill = (skillType == this._curLifeSkillType);
				}
				Refers refers = layout.transform.GetChild(i).GetComponent<Refers>();
				refers.gameObject.SetActive(showSkill);
				bool flag3 = showSkill;
				if (flag3)
				{
					short skillAttainment = this._lifeSkillAttainments.Get((int)skillType);
					short toolAttainment = UI_Make.GetToolAttainment(this._currentTool.Key.TemplateId, skillAttainment, this._curLifeSkillType);
					string toolAttainmentText = (toolAttainment >= 0) ? string.Format("+{0}", toolAttainment) : toolAttainment.ToString().SetColor("brightred");
					LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[skillType];
					refers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.GetFormat(LanguageKey.LK_Tool_Add_Attainment, skillConfig.Name);
					refers.CGet<TextMeshProUGUI>("Value").text = toolAttainmentText;
					refers.CGet<CImage>("Icon").SetSprite(skillConfig.DisplayIcon, false, null);
				}
			}
		}
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x0015B2D8 File Offset: 0x001594D8
	private void ChangeCurrentTarget(ItemDisplayData target, bool forceRefresh = false)
	{
		bool hasChanged = forceRefresh || this.CheckTargetHasChanged(target);
		this.ChangeCurrentTargetOnMake(target, hasChanged);
		bool flag = this._currentTarget != null;
		if (flag)
		{
			this._itemViewTargetSelected.gameObject.SetActive(true);
			this._itemViewTargetSelected.SetData(this._currentTarget, false, this._currentTarget.Amount, false, true, null, false, true);
			this.PlayConditionAnim(true, true);
		}
		else
		{
			this._itemViewTargetSelected.gameObject.SetActive(false);
			this.PlayConditionAnim(true, false);
			this._requireLifeSkillList.gameObject.SetActive(false);
		}
		this.CheckCondition(hasChanged);
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x0015B384 File Offset: 0x00159584
	private void ChangeCurrentTargetOnMake(ItemDisplayData target, bool hasChanged)
	{
		List<ItemDisplayData> itemSource = this.GetItemsSource(this._materialTogGroup);
		this._currentTarget = target;
		this._itemCacheTargets.Clear();
		this._itemCacheTargets.AddRange(itemSource.Where(delegate(ItemDisplayData d)
		{
			short num;
			return (target == null || !d.ContainsItemKey(target.Key)) && ItemTemplateHelper.MedicineIsOdd(d.Key.ItemType, d.Key.TemplateId, out num);
		}));
		this._itemViewMaterial.SetItemList(ref this._itemCacheTargets, false, null, false, null);
		if (hasChanged)
		{
			this._makeCount = 1;
		}
		bool flag = target == null;
		if (flag)
		{
			this._makePanel.gameObject.SetActive(false);
			this._previewTip.enabled = false;
			this.PlayCenterAnim(false, false);
		}
		else
		{
			this._makePanel.gameObject.SetActive(true);
			this.PlayCenterAnim(false, false);
		}
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x0015B45C File Offset: 0x0015965C
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "ButtonCancel"))
		{
			if (a == "ButtonConfirm")
			{
				this.ConfirmMake();
			}
		}
		else
		{
			base.QuickHide();
		}
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x0015B4A4 File Offset: 0x001596A4
	private void CheckCondition(bool targetOrToolHasChanged = false)
	{
		this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Start").SetActive(true);
		this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Get").SetActive(false);
		this.CheckMakeCondition();
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x0015B4F1 File Offset: 0x001596F1
	private void CheckMakeCondition()
	{
		this.OnCheckMakeCondition();
		this.RefreshMakeTip();
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x0015B504 File Offset: 0x00159704
	private unsafe void OnCheckMakeCondition()
	{
		UI_MakeMedicine.<>c__DisplayClass66_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this._buttonConfirm.interactable = false;
		this._makeCountButtonLess.gameObject.SetActive(true);
		this._makeCountButtonMore.gameObject.SetActive(true);
		this._makeCountButtonMin.gameObject.SetActive(true);
		this._makeCountButtonMax.gameObject.SetActive(true);
		this.RefreshMakeCount();
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this._makeToolDurabilityCost = 0;
			this._makeCountButtonLess.interactable = false;
			this._makeCountButtonMore.interactable = false;
			this._makeCountButtonMin.interactable = false;
			this._makeCountButtonMax.interactable = false;
			this._makePanel.gameObject.SetActive(false);
			this.CheckTool(0);
			this.SetConfirmButtonTip(7603, true, TipType.SingleDesc, null);
		}
		else
		{
			this._makePanel.gameObject.SetActive(true);
			LifeSkillInts needLifeSkill = default(LifeSkillInts);
			LifeSkillInts showLifeSkill = default(LifeSkillInts);
			sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
			short requiredAttainment = GlobalConfig.Instance.MakeMadicineAttainments[(int)grade];
			*(ref needLifeSkill.Items.FixedElementField + (IntPtr)this._curLifeSkillType * 4) = (int)requiredAttainment;
			bool flag2 = requiredAttainment > 0;
			if (flag2)
			{
				*(ref showLifeSkill.Items.FixedElementField + (IntPtr)this._curLifeSkillType * 4) = 1;
			}
			bool lifeSkillMeet = this.CheckLifeSkill(needLifeSkill, showLifeSkill);
			short targetTemplateId;
			ItemTemplateHelper.CanMedicineUpgrade(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId, out targetTemplateId);
			ref MakeResultStage[] ptr = ref this._makeResult.MakeResultItemArray;
			if (ptr == null)
			{
				ptr = new MakeResultStage[1];
			}
			this._makeResult.MakeResultItemArray[0] = new MakeResultStage((int)requiredAttainment, lifeSkillMeet, this._currentTarget.Key.ItemType, targetTemplateId, -1);
			bool flag3 = this._currentTarget.Amount < 3;
			if (flag3)
			{
				this.SetConfirmButtonTip(7565, true, TipType.SingleDesc, null);
			}
			else
			{
				CS$<>8__locals1.allMeet = true;
				short cost = this.GetToolDurabilityCost(this._currentTool, grade);
				this._makeToolDurabilityCost = (int)cost;
				short durabilityCost = Convert.ToInt16((int)(cost * this._makeCount));
				bool toolMeet = this.CheckTool(durabilityCost);
				bool flag4 = !toolMeet;
				if (flag4)
				{
					CS$<>8__locals1.allMeet = false;
					this.<OnCheckMakeCondition>g__RefreshCountButton|66_0(ref CS$<>8__locals1);
				}
				else
				{
					bool flag5 = !lifeSkillMeet;
					if (flag5)
					{
						this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
						CS$<>8__locals1.allMeet = false;
						this.<OnCheckMakeCondition>g__RefreshCountButton|66_0(ref CS$<>8__locals1);
					}
					else
					{
						this.SetConfirmButtonTip(-1, true, TipType.SingleDesc, null);
						this._buttonConfirm.interactable = CS$<>8__locals1.allMeet;
						this.<OnCheckMakeCondition>g__RefreshCountButton|66_0(ref CS$<>8__locals1);
					}
				}
			}
		}
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x0015B7D4 File Offset: 0x001599D4
	private void RefreshMakeCount()
	{
		ItemDisplayData currentTarget = this._currentTarget;
		int amount = (currentTarget != null) ? currentTarget.Amount : 1;
		ItemDisplayData currentTool = this._currentTool;
		short durability = (currentTool != null) ? currentTool.Durability : 0;
		int toolCount = (this._makeToolDurabilityCost <= 0) ? int.MaxValue : ((int)durability / this._makeToolDurabilityCost);
		int itemCount = amount / 3;
		int maxCount = Mathf.Min(toolCount, itemCount);
		maxCount = Mathf.Max(maxCount, 1);
		this._makeCount = (short)Mathf.Clamp((int)this._makeCount, 1, maxCount);
		this._confirmNormalLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Making_Times, this._makeCount);
		this._confirmDisableLabel.text = this._confirmNormalLabel.text;
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x0015B888 File Offset: 0x00159A88
	private void ConfirmMake()
	{
		this._onClickConfirm = delegate()
		{
			GEvent.OnEvent(UiEvents.RealConfirmExecuteProfessionSkill, null);
			UIElement.FullScreenMask.Hide(false);
			this.RefreshAllItems();
		};
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = 13,
			IsSuccess = true,
			MakeMedicineCostMedicine = this._currentTarget,
			MakeMedicineCostTool = this._currentTool,
			MakeMedicineCount = (int)this._makeCount
		};
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
		argumentBox.Set("InstantlyConfirm", false);
		Action action = delegate()
		{
			UIElement.FullScreenMask.Show();
			this._buttonConfirm.interactable = false;
			this.PlayCenterAnim(true, false);
		};
		argumentBox.SetObject("OnConfirm", action);
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x0015B938 File Offset: 0x00159B38
	private unsafe short GetLifeSkillTotalAttainment(sbyte type)
	{
		short attainment = *(ref this._lifeSkillAttainments.Items.FixedElementField + (IntPtr)type * 2);
		bool flag = this._currentTool == null;
		short result;
		if (flag)
		{
			result = attainment;
		}
		else
		{
			bool flag2 = this.IsEmptyTool(this._currentTool);
			if (flag2)
			{
				short finalAttainment = UI_Make.GetFinalAttainment(this._currentTool.Key.TemplateId, attainment, this._curLifeSkillType);
				result = finalAttainment;
			}
			else
			{
				CraftToolItem toolConfig = CraftTool.Instance[this._currentTool.Key.TemplateId];
				bool flag3 = toolConfig != null && toolConfig.RequiredLifeSkillTypes.Contains(type);
				if (flag3)
				{
					short finalAttainment2 = UI_Make.GetFinalAttainment(this._currentTool.Key.TemplateId, attainment, this._curLifeSkillType);
					result = finalAttainment2;
				}
				else
				{
					result = attainment;
				}
			}
		}
		return result;
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x0015BA03 File Offset: 0x00159C03
	private bool IsEmptyTool(ItemDisplayData data)
	{
		return this.IsEmptyTool(data.Key);
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x0015BA11 File Offset: 0x00159C11
	private bool IsEmptyTool(ItemKey itemKey)
	{
		return ItemTemplateHelper.IsEmptyTool(itemKey.ItemType, itemKey.TemplateId);
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x0015BA24 File Offset: 0x00159C24
	private short GetToolDurabilityCost(ItemDisplayData tool, sbyte materialGrade)
	{
		bool flag = tool == null || !tool.Key.IsValid() || this.IsEmptyTool(tool);
		short result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			result = CraftTool.Instance[tool.Key.TemplateId].DurabilityCost[(int)materialGrade];
		}
		return result;
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x0015BA78 File Offset: 0x00159C78
	private unsafe bool CheckLifeSkill(LifeSkillInts needLifeSkill, LifeSkillInts showLifeSkill)
	{
		bool lifeSkillMeet = true;
		this._requireLifeSkillList.gameObject.SetActive(true);
		for (int i = 0; i < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; i++)
		{
			sbyte skillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[i];
			short finalLifeSkillAttainment = this.GetLifeSkillTotalAttainment(skillType);
			bool curSkillMeet = true;
			int needAttainment = Mathf.Max(0, *(ref needLifeSkill.Items.FixedElementField + (IntPtr)skillType * 4));
			bool flag = (int)finalLifeSkillAttainment < needAttainment;
			if (flag)
			{
				lifeSkillMeet = false;
				curSkillMeet = false;
			}
			LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[skillType];
			Refers refers = this._requireLifeSkillList.transform.GetChild(i).GetComponent<Refers>();
			refers.CGet<CImage>("Icon").SetSprite(skillConfig.DisplayIcon, false, null);
			string color = curSkillMeet ? "brightblue" : "brightred";
			refers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_LifeSkill_Require_Name, skillConfig.Name);
			refers.CGet<TextMeshProUGUI>("Value").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_LifeSkill_Require_Value, finalLifeSkillAttainment.ToString().SetColor(color), needAttainment);
			refers.gameObject.SetActive(showLifeSkill.Get((int)skillType) > 0);
		}
		for (int j = 0; j < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; j++)
		{
			Refers refers2 = this._requireLifeSkillList.transform.GetChild(j).GetComponent<Refers>();
			this.RefreshAttainmentTip(refers2.CGet<TooltipInvoker>("Tip"));
		}
		return lifeSkillMeet;
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x0015BC08 File Offset: 0x00159E08
	private void SetConfirmButtonTip(int contentKey = -1, bool enabled = true, TipType type = TipType.SingleDesc, ItemDisplayData itemData = null)
	{
		this._confirmButtonTipDisplayer.enabled = enabled;
		this._confirmButtonTipDisplayer.Type = type;
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		bool flag = -1 != contentKey;
		if (flag)
		{
			stringBuilder.AppendLine(LocalStringManager.Get((LanguageKey)contentKey));
		}
		string content = stringBuilder.ToString();
		this._confirmButtonTipDisplayer.PresetParam[0] = content;
		bool flag2 = content.IsNullOrEmpty();
		if (flag2)
		{
			this._confirmButtonTipDisplayer.enabled = false;
		}
		EasyPool.Free<StringBuilder>(stringBuilder);
		this._confirmButtonTipDisplayer.RuntimeParam = ((itemData == null) ? null : new ArgumentBox().SetObject("ItemData", itemData));
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x0015BCAC File Offset: 0x00159EAC
	private void RefreshMakeTip()
	{
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this._previewTip.enabled = false;
		}
		else
		{
			bool flag2 = !this._previewTip.gameObject.activeSelf;
			if (flag2)
			{
				this._previewTip.gameObject.SetActive(true);
			}
			this._previewTip.enabled = true;
			this._previewTip.Type = TipType.MakeItem;
			string title = ItemTemplateHelper.GetName(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
			this._previewTip.RuntimeParam = new ArgumentBox().SetObject("MakeResult", this._makeResult).Set("Title", title);
		}
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x0015BD74 File Offset: 0x00159F74
	private void RefreshAttainmentTip(TooltipInvoker tipDisplayer)
	{
		bool flag = this._currentTool == null;
		if (flag)
		{
			tipDisplayer.enabled = false;
		}
		else
		{
			bool isEmptyTool = this._currentTool.Key == this._emptyToolKey;
			CraftToolItem toolConfig = CraftTool.Instance[this._currentTool.Key.TemplateId];
			tipDisplayer.enabled = true;
			List<HealAttainmentTipsHelper.AttainmentItem> attainmentItems = EasyPool.Get<List<HealAttainmentTipsHelper.AttainmentItem>>();
			attainmentItems.Clear();
			for (int index = 0; index < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; index++)
			{
				sbyte tipSkillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[index];
				bool flag2 = !this._requireLifeSkillList.GetChild(index).gameObject.activeSelf || !toolConfig.RequiredLifeSkillTypes.Contains(tipSkillType);
				if (!flag2)
				{
					short toolAttainment = UI_Make.GetToolAttainment(this._currentTool.Key.TemplateId, this._lifeSkillAttainments.Get((int)tipSkillType), tipSkillType);
					int delta = (int)(toolConfig.RequiredLifeSkillTypes.Contains(tipSkillType) ? toolAttainment : 0);
					attainmentItems.Add(new HealAttainmentTipsHelper.AttainmentItem
					{
						SkillType = tipSkillType,
						DeltaAttainment = delta,
						Attainment = (int)this._lifeSkillAttainments.Get((int)tipSkillType)
					});
				}
			}
			bool flag3 = attainmentItems.Count > 0;
			if (flag3)
			{
				HealAttainmentTipsHelper.RefreshTips(tipDisplayer, this._currentTool.Key, isEmptyTool, attainmentItems, false);
			}
			EasyPool.Free<List<HealAttainmentTipsHelper.AttainmentItem>>(attainmentItems);
		}
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x0015BEE4 File Offset: 0x0015A0E4
	private void PlayConditionAnim(bool isTarget, bool isIn)
	{
		string name = isTarget ? "TargetConditionMeet" : "ToolConditionMeet";
		base.CGet<CImage>(name).DOFillAmount((float)(isIn ? 1 : 0), 0.2f);
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x0015BF1C File Offset: 0x0015A11C
	private void PlayCenterAnim(bool play, bool loop = false)
	{
		UI_MakeMedicine.<>c__DisplayClass78_0 CS$<>8__locals1 = new UI_MakeMedicine.<>c__DisplayClass78_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.play = play;
		CS$<>8__locals1.loop = loop;
		CS$<>8__locals1.craftAnim = base.CGet<SkeletonGraphic>("CraftAnim");
		bool ready = this.Element.Ready;
		if (ready)
		{
			CS$<>8__locals1.<PlayCenterAnim>g__Play|0();
		}
		else
		{
			base.DelayCall(new Action(CS$<>8__locals1.<PlayCenterAnim>g__Play|0), this.AnimIn.duration);
		}
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x0015BF91 File Offset: 0x0015A191
	private void AnimationStateOnComplete(TrackEntry trackentry)
	{
		Action onClickConfirm = this._onClickConfirm;
		if (onClickConfirm != null)
		{
			onClickConfirm();
		}
		this.PlayCenterAnim(false, false);
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x0015BFB0 File Offset: 0x0015A1B0
	private List<ItemDisplayData> GetItemsSource(CToggleGroupObsolete toggleGroup)
	{
		List<ItemDisplayData> result;
		switch (this.GetTogType(toggleGroup))
		{
		case UI_MakeMedicine.TogType.Inventory:
			result = this._inventoryItems;
			break;
		case UI_MakeMedicine.TogType.Warehouse:
			result = this._warehouseItems;
			break;
		case UI_MakeMedicine.TogType.Treasury:
			result = this._treasuryItems;
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x0015C000 File Offset: 0x0015A200
	private UI_MakeMedicine.TogType GetTogType(CToggleGroupObsolete toggleGroup)
	{
		return (UI_MakeMedicine.TogType)toggleGroup.GetActive().Key;
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x0015C020 File Offset: 0x0015A220
	private void SetVillagerNeedMark(ItemView itemView, ItemSourceType sourceType)
	{
		bool sourceTypeIsMeet = sourceType == ItemSourceType.Treasury;
		bool flag = !sourceTypeIsMeet;
		if (flag)
		{
			itemView.SetVillagerNeedMark(false, true);
		}
		else
		{
			ItemKey tempKey = ItemKey.Invalid;
			tempKey.ItemType = itemView.Data.Key.ItemType;
			tempKey.TemplateId = itemView.Data.Key.TemplateId;
			bool isNeeded = this._villagerNeededItemSet.Contains(tempKey);
			itemView.SetVillagerNeedMark(isNeeded, true);
		}
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x0015C128 File Offset: 0x0015A328
	[CompilerGenerated]
	private void <OnEnable>g__InitTogGroup|43_0(CToggleGroupObsolete toggleGroup)
	{
		toggleGroup.Set(UI_MakeMedicine.TogType.Inventory.ToInt(), true, false);
		this.<OnEnable>g__InitSettlementTog|43_1(toggleGroup.Get(UI_MakeMedicine.TogType.Warehouse.ToInt()));
		this.<OnEnable>g__InitSettlementTog|43_1(toggleGroup.Get(UI_MakeMedicine.TogType.Treasury.ToInt()));
		MonoJoint[] componentsInChildren = toggleGroup.Get(UI_MakeMedicine.TogType.Inventory.ToInt()).gameObject.GetComponentsInChildren<MonoJoint>(true);
		if (componentsInChildren != null)
		{
			componentsInChildren.ForEach(delegate(int i, MonoJoint joint)
			{
				joint.JointSync();
				return false;
			});
		}
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x0015C1C1 File Offset: 0x0015A3C1
	[CompilerGenerated]
	private void <OnEnable>g__InitSettlementTog|43_1(CToggleObsolete toggle)
	{
		toggle.interactable = this._isSettlement;
		toggle.GetComponent<TooltipInvoker>().enabled = !this._isSettlement;
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x0015C258 File Offset: 0x0015A458
	[CompilerGenerated]
	private void <OnCheckMakeCondition>g__RefreshCountButton|66_0(ref UI_MakeMedicine.<>c__DisplayClass66_0 A_1)
	{
		bool flag = this._currentTool == null;
		if (flag)
		{
			this._makeCountButtonLess.interactable = false;
			this._makeCountButtonMore.interactable = false;
			this._makeCountButtonMin.interactable = false;
			this._makeCountButtonMax.interactable = false;
		}
		else
		{
			this._makeCountButtonMin.interactable = (this._makeCountButtonLess.interactable = (this._makeCount > 1));
			int toolDurability = (!this.IsEmptyTool(this._currentTool)) ? ((int)this._currentTool.Durability) : int.MaxValue;
			int itemCount = this._currentTarget.Amount / 3;
			bool canAdd = A_1.allMeet && (float)this._toolDurabilityCost + (float)this._toolDurabilityCost / (float)this._makeCount <= (float)toolDurability && (int)this._makeCount < itemCount;
			this._makeCountButtonMax.interactable = (this._makeCountButtonMore.interactable = canAdd);
		}
	}

	// Token: 0x04001FFE RID: 8190
	private ItemScrollView _itemViewTools;

	// Token: 0x04001FFF RID: 8191
	private ItemScrollView _itemViewMaterial;

	// Token: 0x04002000 RID: 8192
	private ItemView _itemViewToolSelected;

	// Token: 0x04002001 RID: 8193
	private ItemView _itemViewTargetSelected;

	// Token: 0x04002002 RID: 8194
	private CButtonObsolete _buttonConfirm;

	// Token: 0x04002003 RID: 8195
	private TextMeshProUGUI _toolDurabilityText;

	// Token: 0x04002004 RID: 8196
	private TextMeshProUGUI _confirmNormalLabel;

	// Token: 0x04002005 RID: 8197
	private TextMeshProUGUI _confirmDisableLabel;

	// Token: 0x04002006 RID: 8198
	private RectTransform _requireLifeSkillList;

	// Token: 0x04002007 RID: 8199
	private TooltipInvoker _confirmButtonTipDisplayer;

	// Token: 0x04002008 RID: 8200
	private CToggleGroupObsolete _toolTogGroup;

	// Token: 0x04002009 RID: 8201
	private CToggleGroupObsolete _materialTogGroup;

	// Token: 0x0400200A RID: 8202
	private TooltipInvoker _previewTip;

	// Token: 0x0400200B RID: 8203
	private CButtonObsolete _makeCountButtonLess;

	// Token: 0x0400200C RID: 8204
	private CButtonObsolete _makeCountButtonMore;

	// Token: 0x0400200D RID: 8205
	private CButtonObsolete _makeCountButtonMin;

	// Token: 0x0400200E RID: 8206
	private CButtonObsolete _makeCountButtonMax;

	// Token: 0x0400200F RID: 8207
	private Refers _makePanel;

	// Token: 0x04002010 RID: 8208
	private short _makeCount = 1;

	// Token: 0x04002011 RID: 8209
	private int _makeToolDurabilityCost;

	// Token: 0x04002012 RID: 8210
	private MakeResult _makeResult = default(MakeResult);

	// Token: 0x04002013 RID: 8211
	private bool _isSettlement;

	// Token: 0x04002014 RID: 8212
	private short _toolDurabilityCost;

	// Token: 0x04002015 RID: 8213
	private ItemDisplayData _currentTool;

	// Token: 0x04002016 RID: 8214
	private ItemDisplayData _currentTarget;

	// Token: 0x04002017 RID: 8215
	private LifeSkillShorts _lifeSkillAttainments;

	// Token: 0x04002018 RID: 8216
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x04002019 RID: 8217
	private List<ItemDisplayData> _warehouseItems = new List<ItemDisplayData>();

	// Token: 0x0400201A RID: 8218
	private List<ItemDisplayData> _treasuryItems = new List<ItemDisplayData>();

	// Token: 0x0400201B RID: 8219
	private List<ItemDisplayData> _allItems = new List<ItemDisplayData>();

	// Token: 0x0400201C RID: 8220
	private List<ItemDisplayData> _itemCacheTools = new List<ItemDisplayData>();

	// Token: 0x0400201D RID: 8221
	private List<ItemDisplayData> _itemCacheTargets = new List<ItemDisplayData>();

	// Token: 0x0400201E RID: 8222
	private readonly sbyte _curLifeSkillType = 8;

	// Token: 0x0400201F RID: 8223
	private int _taiwuCharId;

	// Token: 0x04002020 RID: 8224
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x04002021 RID: 8225
	private ItemKey _emptyToolKey;

	// Token: 0x04002022 RID: 8226
	private readonly ItemDisplayData _invalidItemDisplayData = new ItemDisplayData
	{
		Key = ItemKey.Invalid
	};

	// Token: 0x04002023 RID: 8227
	private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

	// Token: 0x04002024 RID: 8228
	private Action _onClickConfirm;

	// Token: 0x0200164C RID: 5708
	private enum TogType
	{
		// Token: 0x0400A77F RID: 42879
		Inventory,
		// Token: 0x0400A780 RID: 42880
		Warehouse,
		// Token: 0x0400A781 RID: 42881
		Treasury
	}
}
