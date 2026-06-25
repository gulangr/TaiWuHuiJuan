using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003A8 RID: 936
public class UI_SetEquipmentEffect : UIBase
{
	// Token: 0x170005C6 RID: 1478
	// (get) Token: 0x0600384B RID: 14411 RVA: 0x001C6363 File Offset: 0x001C4563
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170005C7 RID: 1479
	// (get) Token: 0x0600384C RID: 14412 RVA: 0x001C636F File Offset: 0x001C456F
	private bool TypeTogIsCommon
	{
		get
		{
			return this._toggleGroup.GetActive().Key == UI_SetEquipmentEffect.EffectTogType.Common.ToInt();
		}
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x001C6390 File Offset: 0x001C4590
	public override void OnInit(ArgumentBox argsBox)
	{
		ItemKey selectedItemKey;
		this._selectedItemKey = ((argsBox != null && argsBox.Get<ItemKey>("SelectedItemKey", out selectedItemKey)) ? selectedItemKey : ItemKey.Invalid);
		this._selectedEffectId = -1;
		this._selectedItemView = null;
		this._popupWindow = base.CGet<PopupWindow>("PopupWindowBase");
		this._popupWindow.OnConfirmClick = new Action(this.OnConfirmSelect);
		this._popupWindow.OnCancelClick = delegate()
		{
			UIManager.Instance.HideUI(this.Element);
		};
		this._popupWindow.ConfirmButton.interactable = false;
		this._popupWindow.CancelButton.interactable = true;
		ProfessionSkillItem skillConfig = ProfessionSkill.Instance[10];
		this._popupWindow.SetTitle(skillConfig.Name);
		this._itemScroll = base.CGet<ItemScrollView>("ItemScrollView");
		this._itemScroll.Init();
		this._itemScroll.SetItemList(ref this._showItems, true, null, this._itemScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderItem));
		this._toggleGroup = base.CGet<CToggleGroupObsolete>("ToggleGroup");
		this._toggleGroup.InitPreOnToggle(-1);
		this._toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		this._toggleGroup.Set(UI_SetEquipmentEffect.EffectTogType.Exclusive.ToInt(), true, true);
		Refers carEffectItem = base.CGet<Refers>("CurEffectItem");
		this.SetEffectItemNone(carEffectItem, true);
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			CharacterDomainMethod.Call.GetInventoryEquipment(this.Element.GameDataListenerId, this.TaiwuCharId);
			CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, this.TaiwuCharId);
			this.Element.ShowAfterRefresh();
		}));
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x001C652C File Offset: 0x001C472C
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this.TaiwuCharId), new uint[]
		{
			97U
		}));
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x001C6554 File Offset: 0x001C4754
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
						bool flag2 = notification.MethodId == 117;
						if (flag2)
						{
							this._inventoryItems.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
						}
						else
						{
							bool flag3 = notification.MethodId == 29;
							if (flag3)
							{
								List<ItemDisplayData> equipItems = new List<ItemDisplayData>();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref equipItems);
								sbyte slotIndex = 0;
								while ((int)slotIndex < equipItems.Count)
								{
									ItemDisplayData itemData = equipItems[(int)slotIndex];
									bool flag4 = itemData.Key.IsValid();
									if (flag4)
									{
										itemData.UsingType = ItemDisplayData.ItemUsingType.Equiped;
										this._inventoryItems.Add(itemData);
									}
									slotIndex += 1;
								}
								this._showItems.Clear();
								foreach (ItemDisplayData itemData2 in this._inventoryItems)
								{
									bool flag5 = itemData2.CanSetEquipmentEffect();
									if (flag5)
									{
										this._showItems.Add(itemData2);
									}
								}
								this._itemScroll.SetItemList(ref this._showItems, false, null, false, null);
							}
						}
					}
				}
			}
			else
			{
				bool flag6 = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0;
				if (flag6)
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

	// Token: 0x06003850 RID: 14416 RVA: 0x001C6798 File Offset: 0x001C4998
	private void OnActiveToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this.RefreshEffect();
	}

	// Token: 0x06003851 RID: 14417 RVA: 0x001C67A4 File Offset: 0x001C49A4
	public static bool CanSetEquipmentEffect(ItemDisplayData itemData, LifeSkillShorts lifeSkillAttainments, int reqFactor, out sbyte lifeSkillType, out short meetRequirements)
	{
		short requiredAttainment = ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability);
		lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
		short curAttainment = lifeSkillAttainments.Get((int)lifeSkillType);
		meetRequirements = Convert.ToInt16((int)requiredAttainment * reqFactor / 100);
		return curAttainment >= meetRequirements;
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x001C6818 File Offset: 0x001C4A18
	private void OnRenderItem(ItemDisplayData itemData, ItemView itemView)
	{
		sbyte attainmentType;
		short meetRequirements;
		bool isMeet = UI_SetEquipmentEffect.CanSetEquipmentEffect(itemData, this._lifeSkillAttainments, this._reqFactor, out attainmentType, out meetRequirements);
		itemView.SetInteractable(isMeet);
		bool flag = isMeet;
		if (flag)
		{
			itemView.HideInteractionState();
		}
		else
		{
			itemView.ShowInteractionStateAttainment(attainmentType, meetRequirements, false);
		}
		itemView.SetClickEvent(delegate
		{
			Refers carEffectItem = this.CGet<Refers>("CurEffectItem");
			CToggleObsolete toggle = carEffectItem.GetComponent<CToggleObsolete>();
			toggle.isOn = true;
			List<short> equipmentEffectIds = itemData.EquipmentEffectIds;
			bool flag3 = equipmentEffectIds != null && equipmentEffectIds.Count > 0;
			if (flag3)
			{
				EquipmentEffectItem effectConfig = EquipmentEffect.Instance[itemData.EquipmentEffectIds[0]];
				carEffectItem.CGet<TextMeshProUGUI>("Label").text = LocalStringManager.GetFormat(LanguageKey.LK_EquipmentEffect_Item, effectConfig.Name, effectConfig.Desc).ColorReplace();
				this.SetEffectItemNone(carEffectItem, false);
			}
			else
			{
				this.SetEffectItemNone(carEffectItem, true);
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(carEffectItem.GetComponent<RectTransform>());
			});
			ItemView selectedItemView = this._selectedItemView;
			if (selectedItemView != null)
			{
				selectedItemView.SetHighLight(false);
			}
			this._selectedItemView = itemView;
			this._selectedItemView.SetHighLight(true);
			this.RefreshEffect();
		});
		bool flag2 = itemData.RealKey.Equals(this._selectedItemKey);
		if (flag2)
		{
			itemView.Click();
		}
	}

	// Token: 0x06003853 RID: 14419 RVA: 0x001C68D4 File Offset: 0x001C4AD4
	private void SetEffectItemNone(Refers refers, bool isNone)
	{
		TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
		GameObject none = refers.CGet<GameObject>("None");
		label.gameObject.SetActive(!isNone);
		none.SetActive(isNone);
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x001C6914 File Offset: 0x001C4B14
	private void RefreshEffectData()
	{
		UI_SetEquipmentEffect.EffectType effectType3;
		if (this.TypeTogIsCommon)
		{
			effectType3 = UI_SetEquipmentEffect.EffectType.Common;
		}
		else
		{
			sbyte itemType = this._selectedItemView.Data.Key.ItemType;
			if (!true)
			{
			}
			UI_SetEquipmentEffect.EffectType effectType2;
			if (itemType != 0)
			{
				if (itemType != 1)
				{
					effectType2 = UI_SetEquipmentEffect.EffectType.None;
				}
				else
				{
					effectType2 = UI_SetEquipmentEffect.EffectType.Armor;
				}
			}
			else
			{
				effectType2 = UI_SetEquipmentEffect.EffectType.Weapon;
			}
			if (!true)
			{
			}
			effectType3 = effectType2;
		}
		UI_SetEquipmentEffect.EffectType effectType = effectType3;
		this._effectConfigList.Clear();
		this._effectConfigList.Add(null);
		EquipmentEffect.Instance.Iterate(delegate(EquipmentEffectItem effect)
		{
			bool flag = !effect.Special && (int)effect.Type == effectType.ToInt();
			if (flag)
			{
				this._effectConfigList.Add(effect);
			}
			return true;
		});
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x001C69AC File Offset: 0x001C4BAC
	private void RefreshEffect()
	{
		this._popupWindow.ConfirmButton.interactable = false;
		this._selectedEffectId = -1;
		CScrollRectLegacy scrollRect = base.CGet<CScrollRectLegacy>("EffectScrollView");
		bool flag = this._selectedItemView == null;
		if (flag)
		{
			for (int i = 1; i < scrollRect.Content.childCount; i++)
			{
				scrollRect.Content.GetChild(i).gameObject.SetActive(false);
			}
			Refers refers = scrollRect.Content.GetChild(0).GetComponent<Refers>();
			this.SetEffectItemNone(refers, true);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.Content);
			});
		}
		else
		{
			this.RefreshEffectData();
			Transform template = scrollRect.Content.GetChild(0);
			for (int j = 0; j < this._effectConfigList.Count; j++)
			{
				EquipmentEffectItem effectConfig = this._effectConfigList[j];
				Transform child = (j < scrollRect.Content.childCount) ? scrollRect.Content.GetChild(j) : Object.Instantiate<Transform>(template, scrollRect.Content);
				child.gameObject.SetActive(true);
				Refers refers2 = child.GetComponent<Refers>();
				bool flag2 = effectConfig == null;
				if (flag2)
				{
					this.SetEffectItemNone(refers2, true);
				}
				else
				{
					this.SetEffectItemNone(refers2, false);
					refers2.CGet<TextMeshProUGUI>("Label").text = LocalStringManager.GetFormat(LanguageKey.LK_EquipmentEffect_Item, effectConfig.Name, effectConfig.Desc).ColorReplace();
				}
			}
			for (int k = this._effectConfigList.Count; k < scrollRect.Content.childCount; k++)
			{
				Transform child2 = scrollRect.Content.GetChild(k);
				child2.gameObject.SetActive(false);
			}
			CToggleGroupObsolete toggleGroup = scrollRect.Content.GetComponent<CToggleGroupObsolete>();
			toggleGroup.AddAllChildToggles();
			bool flag3 = toggleGroup.OnActiveToggleChange == null;
			if (flag3)
			{
				toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnEffectToggleChange);
			}
			toggleGroup.InitPreOnToggle(-1);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.Content);
			});
		}
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x001C6C20 File Offset: 0x001C4E20
	private void OnEffectToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog;
		if (flag)
		{
			EquipmentEffectItem equipmentEffectItem = this._effectConfigList[newTog.Key];
			this._selectedEffectId = ((equipmentEffectItem != null) ? equipmentEffectItem.TemplateId : -1);
			List<short> equipmentEffectIds = this._selectedItemView.Data.EquipmentEffectIds;
			bool flag2 = equipmentEffectIds != null && equipmentEffectIds.Count > 0;
			if (flag2)
			{
				this._popupWindow.ConfirmButton.interactable = (this._selectedEffectId != this._selectedItemView.Data.EquipmentEffectIds[0]);
			}
			else
			{
				this._popupWindow.ConfirmButton.interactable = true;
			}
		}
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x001C6CD0 File Offset: 0x001C4ED0
	private void OnConfirmSelect()
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = 2,
			SkillId = 10,
			IsSuccess = true,
			ItemKey = this._selectedItemView.Data.Key,
			EffectId = this._selectedEffectId
		};
		argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
		argumentBox.SetObject("OnConfirm", new Action(this.<OnConfirmSelect>g__Action|27_0));
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x001C6DEC File Offset: 0x001C4FEC
	[CompilerGenerated]
	private void <OnConfirmSelect>g__Action|27_0()
	{
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x040028C9 RID: 10441
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x040028CA RID: 10442
	private LifeSkillShorts _lifeSkillAttainments;

	// Token: 0x040028CB RID: 10443
	private ItemView _selectedItemView;

	// Token: 0x040028CC RID: 10444
	private List<ItemDisplayData> _showItems = new List<ItemDisplayData>();

	// Token: 0x040028CD RID: 10445
	private PopupWindow _popupWindow;

	// Token: 0x040028CE RID: 10446
	private ItemScrollView _itemScroll;

	// Token: 0x040028CF RID: 10447
	private CToggleGroupObsolete _toggleGroup;

	// Token: 0x040028D0 RID: 10448
	private short _selectedEffectId = -1;

	// Token: 0x040028D1 RID: 10449
	private readonly List<EquipmentEffectItem> _effectConfigList = new List<EquipmentEffectItem>();

	// Token: 0x040028D2 RID: 10450
	private int _reqFactor;

	// Token: 0x040028D3 RID: 10451
	private ItemKey _selectedItemKey;

	// Token: 0x02001806 RID: 6150
	private enum EffectType
	{
		// Token: 0x0400AD4C RID: 44364
		None = -1,
		// Token: 0x0400AD4D RID: 44365
		Common,
		// Token: 0x0400AD4E RID: 44366
		Weapon,
		// Token: 0x0400AD4F RID: 44367
		Armor
	}

	// Token: 0x02001807 RID: 6151
	private enum EffectTogType
	{
		// Token: 0x0400AD51 RID: 44369
		Exclusive,
		// Token: 0x0400AD52 RID: 44370
		Common
	}
}
