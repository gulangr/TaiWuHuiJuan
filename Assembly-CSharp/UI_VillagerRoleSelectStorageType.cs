using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.VillagerRole;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000414 RID: 1044
public class UI_VillagerRoleSelectStorageType : UIBase
{
	// Token: 0x06003E3C RID: 15932 RVA: 0x001F34C4 File Offset: 0x001F16C4
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = !this._initialized;
		if (flag)
		{
			this.InitRefers();
		}
		RectTransform anchorItem;
		bool flag2 = argsBox.Get<RectTransform>("AnchorItem", out anchorItem);
		if (flag2)
		{
			this._anchorItem = anchorItem;
			this.SetContentPosition(anchorItem);
			this._anchorOriginParent = anchorItem.parent;
			anchorItem.SetParent(this._buttonSlot);
		}
		bool flag3 = argsBox.Get<CButtonObsolete>("ExtraCloseButton", out this._extraCloseButton);
		if (flag3)
		{
			this._extraCloseButton.onClick.AddListener(new UnityAction(this.OnExtraButtonClicked));
		}
		this._dropdownUIMask.gameObject.SetActive(false);
		this._dropdownMask2.SetActive(false);
		argsBox.Get("ResourceType", out this._resourceType);
		argsBox.Get("SelectedCharacterId", out this._selectedCharacterId);
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		this._initialized = true;
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x001F35CF File Offset: 0x001F17CF
	private void OnExtraButtonClicked()
	{
		this.QuickHide();
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x001F35DC File Offset: 0x001F17DC
	private void OnDisable()
	{
		CommonUtils.PrepareEnoughChildren(this._actionLayout, this._actionSettingLineTemplate.gameObject, 0, null);
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x001F360B File Offset: 0x001F180B
	private void OnListenerIdReady()
	{
		this.GetAllSetting();
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x001F3615 File Offset: 0x001F1815
	private void GetAllSetting()
	{
		TaiwuDomainMethod.Call.GetVillagerCollectStorageType(this.Element.GameDataListenerId, this._selectedCharacterId);
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x001F3630 File Offset: 0x001F1830
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 5 && notification.MethodId == 147;
				if (flag)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._villagerRoleStorageType);
					this.RefreshAll();
					this.Element.ShowAfterRefresh();
				}
			}
		}
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x001F36E4 File Offset: 0x001F18E4
	private void RefreshAll()
	{
		this.RefreshActionDropdowns();
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x001F36F0 File Offset: 0x001F18F0
	private void RefreshActionDropdowns()
	{
		CommonUtils.PrepareEnoughChildren(this._actionLayout, this._actionSettingLineTemplate.gameObject, 1, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
		{
			TemplateOrder = CommonUtils.EPrepareTemplateOrder.BeforeExtraItems,
			ExtraItemCount = 1
		}));
		this._dropdownMask2.transform.SetAsLastSibling();
		this._dropdownList.Clear();
		Refers refers = this._actionLayout.transform.GetChild(0).GetComponent<Refers>();
		TextMeshProUGUI nameLabel = refers.CGet<TextMeshProUGUI>("NameLabel");
		CDropdownLegacy dropdown = refers.CGet<CDropdownLegacy>("Dropdown");
		this._dropdownList.Add(dropdown);
		nameLabel.text = LocalStringManager.Get(LanguageKey.LK_VillagerRoleSelectStorageType_Desc);
		IReadOnlyList<VillagerRoleStorageType> optionList = SharedData.CollectStorageDict[this._resourceType];
		dropdown.options.Clear();
		foreach (VillagerRoleStorageType optionValue in optionList)
		{
			string text = LocalStringManager.Get(string.Format("LK_VillagerRoleStorageType_{0}", (int)optionValue));
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
			{
				text = text
			};
			dropdown.options.Add(option);
			AtlasInfo.Instance.GetSprite(UI_VillagerRoleSelectStorageType.GetDropdownOptionIcon(optionValue), delegate(Sprite sprite)
			{
				option.image = sprite;
			});
		}
		dropdown.onValueChanged.RemoveAllListeners();
		dropdown.onValueChanged.AddListener(delegate(int value)
		{
			VillagerRoleStorageType optionValue2 = optionList[value];
			this._villagerRoleStorageType = (sbyte)optionValue2;
			this.SaveAllSetting();
			dropdown.transform.Find("Icon").GetComponent<CImage>().sprite = dropdown.options[value].image;
		});
		int newValue = optionList.IndexOf((VillagerRoleStorageType)this._villagerRoleStorageType);
		dropdown.value = newValue;
		dropdown.transform.Find("Icon").GetComponent<CImage>().sprite = dropdown.options[newValue].image;
		dropdown.RefreshShownValue();
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x001F391C File Offset: 0x001F1B1C
	private void SetContentPosition(RectTransform anchorItem)
	{
		Vector3[] corners = new Vector3[4];
		anchorItem.GetWorldCorners(corners);
		Vector3 anchorBottomLeft = corners[0];
		float anchorWidth = corners[2].x - corners[0].x;
		Vector3 nodeAWorldPos = new Vector3(anchorBottomLeft.x + anchorWidth / 2f, anchorBottomLeft.y, anchorBottomLeft.z);
		float nodeAHeight = this._content.rect.height;
		nodeAWorldPos.y -= nodeAHeight * (1f - this._content.pivot.y);
		Vector2 localPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._content.parent as RectTransform, RectTransformUtility.WorldToScreenPoint(null, nodeAWorldPos), null, out localPos);
		this._content.localPosition = localPos;
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x001F39EC File Offset: 0x001F1BEC
	protected override void OnClick(Transform btn)
	{
		Debug.Log(btn.name);
		string btnName = btn.name;
		bool flag = btnName == "UIMask";
		if (flag)
		{
			this.QuickHide();
		}
		else
		{
			bool flag2 = btnName == "InventoryToggleButton";
			if (flag2)
			{
				this.SaveAllSetting();
				this.GetAllSetting();
			}
		}
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x001F3A46 File Offset: 0x001F1C46
	private void SaveAllSetting()
	{
		TaiwuDomainMethod.Call.SetVillagerCollectStorageType(-1, this._selectedCharacterId, this._villagerRoleStorageType);
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x001F3A5C File Offset: 0x001F1C5C
	private void LateUpdate()
	{
		bool flag = this._dropdownList.Count == 0;
		if (!flag)
		{
			IReadOnlyList<VillagerRoleStorageType> optionList = SharedData.CollectStorageDict[this._resourceType];
			TMP_Dropdown dropdown = this._dropdownList[0];
			Transform arrow = dropdown.transform.Find("Arrow");
			Vector3 localScale = arrow.localScale;
			arrow.localScale = new Vector3(localScale.x, (float)(dropdown.IsExpanded ? -1 : 1), localScale.z);
			bool isExpanded = dropdown.IsExpanded;
			if (isExpanded)
			{
				Transform dropdownList = dropdown.transform.Find("Dropdown List");
				bool flag2 = dropdownList != null;
				if (flag2)
				{
					RectTransform content = dropdownList.GetComponent<ScrollRect>().content;
					for (int i = 1; i < content.childCount; i++)
					{
						bool isSelected = dropdown.value == i - 1;
						Transform item = content.GetChild(i);
						bool flag3 = !item.gameObject.activeSelf;
						if (!flag3)
						{
							PointerTrigger pointerTrigger = item.GetComponent<PointerTrigger>();
							bool flag4 = i - 1 > optionList.Count - 1;
							if (!flag4)
							{
								item.GetComponent<CToggleObsolete>().interactable = !isSelected;
								VillagerRoleStorageType optionValue = optionList[i - 1];
								bool flag5 = optionValue == VillagerRoleStorageType.AutoStorageMaterial || optionValue == VillagerRoleStorageType.AutoStorageWarehouse;
								if (flag5)
								{
									pointerTrigger.enabled = true;
									pointerTrigger.EnterEvent.RemoveAllListeners();
									pointerTrigger.EnterEvent.AddListener(delegate()
									{
										string text = LocalStringManager.Get(string.Format("LK_VillagerRoleStorageType_TIPS_{0}", (int)optionValue));
										this.ShowFakeTips(item.GetComponent<RectTransform>(), text);
									});
									pointerTrigger.ExitEvent.RemoveAllListeners();
									pointerTrigger.ExitEvent.AddListener(new UnityAction(this.HideFakeTips));
								}
								else
								{
									pointerTrigger.enabled = false;
								}
							}
						}
					}
				}
			}
			bool flag6 = dropdown.IsExpanded && this._focusedDropdown == null;
			if (flag6)
			{
				this._focusedDropdownOriginParent = dropdown.transform.parent;
				dropdown.transform.SetParent(this._dropdownSlot);
				this._dropdownUIMask.gameObject.SetActive(true);
				this._dropdownMask2.SetActive(true);
				this._focusedDropdown = dropdown;
			}
			bool flag7 = this._focusedDropdown == dropdown && !dropdown.IsExpanded;
			if (flag7)
			{
				dropdown.transform.SetParent(this._focusedDropdownOriginParent);
				this._dropdownUIMask.gameObject.SetActive(false);
				this._dropdownMask2.SetActive(false);
				this._focusedDropdown = null;
			}
		}
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x001F3D28 File Offset: 0x001F1F28
	private void ShowFakeTips(RectTransform anchor, string content)
	{
		this._optionTips.gameObject.SetActive(true);
		this._optionTips.Find("TipContent").GetComponent<TextMeshProUGUI>().text = content;
		Vector3[] corners = new Vector3[4];
		anchor.GetWorldCorners(corners);
		Vector3 anchorUpRight = corners[2];
		Vector2 localPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._optionTips.parent as RectTransform, RectTransformUtility.WorldToScreenPoint(null, anchorUpRight), null, out localPos);
		this._optionTips.localPosition = localPos;
	}

	// Token: 0x06003E49 RID: 15945 RVA: 0x001F3DAD File Offset: 0x001F1FAD
	private void HideFakeTips()
	{
		this._optionTips.gameObject.SetActive(false);
	}

	// Token: 0x06003E4A RID: 15946 RVA: 0x001F3DC4 File Offset: 0x001F1FC4
	public override void QuickHide()
	{
		bool flag = this._extraCloseButton != null;
		if (flag)
		{
			this._extraCloseButton.onClick.RemoveListener(new UnityAction(this.OnExtraButtonClicked));
		}
		bool flag2 = this._focusedDropdown != null && this._focusedDropdown.IsExpanded;
		if (flag2)
		{
			this._focusedDropdown.OnCancel(null);
		}
		else
		{
			base.QuickHide();
			bool flag3 = this._anchorOriginParent != null;
			if (flag3)
			{
				this._anchorItem.SetParent(this._anchorOriginParent);
			}
			bool exist = UIElement.Bottom.Exist;
			if (exist)
			{
				UIElement.Bottom.UiBaseAs<UI_Bottom>().SetIsMouseInPanelRange(true);
			}
		}
	}

	// Token: 0x06003E4B RID: 15947 RVA: 0x001F3E80 File Offset: 0x001F2080
	private static string GetDropdownOptionIcon(VillagerRoleStorageType option)
	{
		switch (option)
		{
		case VillagerRoleStorageType.Inventory:
			return "building_icon_allocationtype_0";
		case VillagerRoleStorageType.Warehouse:
			return "building_icon_allocationtype_1";
		case VillagerRoleStorageType.Treasury:
			return "building_icon_allocationtype_2";
		case VillagerRoleStorageType.StockStorageWarehouse:
		case VillagerRoleStorageType.StockStorageGoodsShelf:
			return "building_icon_allocationtype_3";
		case VillagerRoleStorageType.CraftStorageWarehouse:
		case VillagerRoleStorageType.CraftStorageMaterial:
		case VillagerRoleStorageType.CraftStorageToFix:
		case VillagerRoleStorageType.CraftStorageToDisassemble:
			return "building_icon_allocationtype_4";
		case VillagerRoleStorageType.MedicineStorageWarehouse:
		case VillagerRoleStorageType.MedicineStorageMaterial:
		case VillagerRoleStorageType.MedicineStorageToDetox:
		case VillagerRoleStorageType.MedicineStorageToAddPoison:
			return "building_icon_allocationtype_5";
		case VillagerRoleStorageType.FoodStorageWarehouse:
		case VillagerRoleStorageType.FoodStorageMaterial:
			return "building_icon_allocationtype_6";
		case VillagerRoleStorageType.AutoStorageWarehouse:
		case VillagerRoleStorageType.AutoStorageMaterial:
			return "building_icon_allocationtype_7";
		}
		return "";
	}

	// Token: 0x06003E4C RID: 15948 RVA: 0x001F3F2C File Offset: 0x001F212C
	private void InitRefers()
	{
		this._inventoryToggleOn = base.CGet<GameObject>("InventoryToggleOn");
		this._inventoryToggleOff = base.CGet<GameObject>("InventoryToggleOff");
		this._actionLayout = base.CGet<RectTransform>("ActionLayout");
		this._content = base.CGet<RectTransform>("Content");
		this._actionSettingLineTemplate = base.CGet<Refers>("ActionSettingLineTemplate");
		this._dropdownUIMask = base.CGet<CButtonObsolete>("DropdownUIMask");
		this._dropdownSlot = base.CGet<RectTransform>("DropdownSlot");
		this._buttonSlot = base.CGet<RectTransform>("ButtonSlot");
		this._dropdownMask2 = base.CGet<GameObject>("DropdownMask2");
		this._optionTips = base.CGet<RectTransform>("OptionTips");
	}

	// Token: 0x04002CE2 RID: 11490
	private bool _initialized;

	// Token: 0x04002CE3 RID: 11491
	private Transform _anchorOriginParent;

	// Token: 0x04002CE4 RID: 11492
	private Transform _anchorItem;

	// Token: 0x04002CE5 RID: 11493
	private sbyte _resourceType;

	// Token: 0x04002CE6 RID: 11494
	private readonly List<TMP_Dropdown> _dropdownList = new List<TMP_Dropdown>();

	// Token: 0x04002CE7 RID: 11495
	private TMP_Dropdown _focusedDropdown;

	// Token: 0x04002CE8 RID: 11496
	private Transform _focusedDropdownOriginParent;

	// Token: 0x04002CE9 RID: 11497
	private CButtonObsolete _extraCloseButton;

	// Token: 0x04002CEA RID: 11498
	private int _selectedCharacterId;

	// Token: 0x04002CEB RID: 11499
	private sbyte _villagerRoleStorageType = 1;

	// Token: 0x04002CEC RID: 11500
	private GameObject _inventoryToggleOn;

	// Token: 0x04002CED RID: 11501
	private GameObject _inventoryToggleOff;

	// Token: 0x04002CEE RID: 11502
	private RectTransform _actionLayout;

	// Token: 0x04002CEF RID: 11503
	private RectTransform _content;

	// Token: 0x04002CF0 RID: 11504
	private Refers _actionSettingLineTemplate;

	// Token: 0x04002CF1 RID: 11505
	private CButtonObsolete _dropdownUIMask;

	// Token: 0x04002CF2 RID: 11506
	private RectTransform _dropdownSlot;

	// Token: 0x04002CF3 RID: 11507
	private RectTransform _buttonSlot;

	// Token: 0x04002CF4 RID: 11508
	private GameObject _dropdownMask2;

	// Token: 0x04002CF5 RID: 11509
	private RectTransform _optionTips;
}
