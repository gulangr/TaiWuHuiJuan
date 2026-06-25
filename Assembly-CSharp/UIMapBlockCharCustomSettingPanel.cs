using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Extra;
using TMPro;
using UnityEngine;

// Token: 0x020003DA RID: 986
public class UIMapBlockCharCustomSettingPanel : UIBase
{
	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x06003B38 RID: 15160 RVA: 0x001DFA00 File Offset: 0x001DDC00
	private WorldMapModel WorldMapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x001DFA07 File Offset: 0x001DDC07
	public override void OnInit(ArgumentBox argsBox)
	{
		this.InitConfigureItems();
		this.Refresh();
	}

	// Token: 0x06003B3A RID: 15162 RVA: 0x001DFA18 File Offset: 0x001DDC18
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnMapBlockCharCustomInfoChanged, new GEvent.Callback(this.OnMapBlockCharCustomInfoChanged));
		GEvent.Add(UiEvents.OnMapBlockCharCustomButtonChanged, new GEvent.Callback(this.OnMapBlockCharCustomButtonChanged));
	}

	// Token: 0x06003B3B RID: 15163 RVA: 0x001DFA4D File Offset: 0x001DDC4D
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnMapBlockCharCustomInfoChanged, new GEvent.Callback(this.OnMapBlockCharCustomInfoChanged));
		GEvent.Remove(UiEvents.OnMapBlockCharCustomButtonChanged, new GEvent.Callback(this.OnMapBlockCharCustomButtonChanged));
	}

	// Token: 0x06003B3C RID: 15164 RVA: 0x001DFA82 File Offset: 0x001DDC82
	private void OnMapBlockCharCustomInfoChanged(ArgumentBox _)
	{
		this.Refresh();
	}

	// Token: 0x06003B3D RID: 15165 RVA: 0x001DFA8C File Offset: 0x001DDC8C
	private void OnMapBlockCharCustomButtonChanged(ArgumentBox _)
	{
		this.Refresh();
	}

	// Token: 0x06003B3E RID: 15166 RVA: 0x001DFA98 File Offset: 0x001DDC98
	private void InitConfigureItems()
	{
		int configureItemCount = MapBlockCharCustomInfo.Instance.Count;
		CommonUtils.PrepareEnoughChildren(this.configureLayout, this.configureItemTemplate.gameObject, configureItemCount, null);
		for (int i = 0; i < configureItemCount; i++)
		{
			CommonConfigureBase item = this.configureLayout.GetChild(i).GetComponent<CommonConfigureBase>();
			MapBlockCharCustomInfoItem info = MapBlockCharCustomInfo.Instance[i];
			item.Text = info.Name;
			CommonSwitch commonSwitch = item.GetComponentInChildren<CommonSwitch>();
			commonSwitch.onValueChanged.RemoveAllListeners();
			int index = i;
			commonSwitch.onValueChanged.AddListener(delegate(bool isOn)
			{
				bool settingConfigureItemByCode = this._settingConfigureItemByCode;
				if (!settingConfigureItemByCode)
				{
					if (isOn)
					{
						bool flag = !this._cachedCustomInfoList.Contains((short)index) && this._cachedCustomInfoList.Count < 6;
						if (flag)
						{
							this._cachedCustomInfoList.Add((short)index);
						}
					}
					else
					{
						this._cachedCustomInfoList.Remove((short)index);
					}
					this.Save();
				}
			});
		}
		int buttonConfigureItemCount = MapBlockCharCustomButton.Instance.Count;
		CommonUtils.PrepareEnoughChildren(this.buttonConfigureLayout, this.buttonConfigureItemTemplate.gameObject, buttonConfigureItemCount, null);
		for (int j = 0; j < buttonConfigureItemCount; j++)
		{
			CommonConfigureBase item2 = this.buttonConfigureLayout.GetChild(j).GetComponent<CommonConfigureBase>();
			MapBlockCharCustomButtonItem buttonInfo = MapBlockCharCustomButton.Instance[j];
			item2.Text = buttonInfo.Name;
			CommonSwitch commonSwitch2 = item2.GetComponentInChildren<CommonSwitch>();
			commonSwitch2.onValueChanged.RemoveAllListeners();
			int index = j;
			commonSwitch2.onValueChanged.AddListener(delegate(bool isOn)
			{
				bool settingConfigureItemByCode = this._settingConfigureItemByCode;
				if (!settingConfigureItemByCode)
				{
					if (isOn)
					{
						bool flag = !this._cachedCustomButtonList.Contains((short)index) && this._cachedCustomButtonList.Count < 8;
						if (flag)
						{
							this._cachedCustomButtonList.Add((short)index);
						}
					}
					else
					{
						this._cachedCustomButtonList.Remove((short)index);
					}
					this.SaveButtons();
				}
			});
		}
	}

	// Token: 0x06003B3F RID: 15167 RVA: 0x001DFC22 File Offset: 0x001DDE22
	private void Save()
	{
		this.SortSpecialInfoToEnd();
		ExtraDomainMethod.Call.SetMapBlockCharCustomInfoList(this._cachedCustomInfoList);
	}

	// Token: 0x06003B40 RID: 15168 RVA: 0x001DFC38 File Offset: 0x001DDE38
	private void SaveButtons()
	{
		this.SortButtonCannotCancelToStart();
		ExtraDomainMethod.Call.SetMapBlockCharCustomButtonList(this._cachedCustomButtonList);
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x001DFC50 File Offset: 0x001DDE50
	private void SortButtonCannotCancelToStart()
	{
		short cannotCancelButton = this._buttonCannotCancelKey;
		bool flag = this._cachedCustomButtonList.Contains(cannotCancelButton);
		if (flag)
		{
			this._cachedCustomButtonList.Remove(cannotCancelButton);
			this._cachedCustomButtonList.Insert(0, cannotCancelButton);
		}
	}

	// Token: 0x06003B42 RID: 15170 RVA: 0x001DFC94 File Offset: 0x001DDE94
	private void SortSpecialInfoToEnd()
	{
		short specialInfo = this.InfoSpecialKey;
		bool flag = this._cachedCustomInfoList.Contains(specialInfo);
		if (flag)
		{
			this._cachedCustomInfoList.Remove(specialInfo);
			this._cachedCustomInfoList.Add(specialInfo);
		}
	}

	// Token: 0x06003B43 RID: 15171 RVA: 0x001DFCD8 File Offset: 0x001DDED8
	private void OnSlotSelected(int slotIndex)
	{
		bool flag = this._selectedSlotForSwap == -1;
		if (flag)
		{
			this._selectedSlotForSwap = slotIndex;
			this.RefreshInner();
		}
		else
		{
			bool flag2 = this._selectedSlotForSwap == slotIndex;
			if (flag2)
			{
				this._selectedSlotForSwap = -1;
				this.RefreshInner();
			}
			else
			{
				List<short> displayInfoList = EasyPool.Get<List<short>>();
				displayInfoList.Clear();
				displayInfoList.AddRange(this._cachedCustomInfoList);
				bool hadSpecial = displayInfoList.Remove(this.InfoSpecialKey);
				bool flag3 = this._selectedSlotForSwap < displayInfoList.Count && slotIndex < displayInfoList.Count;
				if (flag3)
				{
					short temp = displayInfoList[this._selectedSlotForSwap];
					displayInfoList[this._selectedSlotForSwap] = displayInfoList[slotIndex];
					displayInfoList[slotIndex] = temp;
					this._cachedCustomInfoList.Clear();
					this._cachedCustomInfoList.AddRange(displayInfoList);
					bool flag4 = hadSpecial;
					if (flag4)
					{
						this._cachedCustomInfoList.Add(this.InfoSpecialKey);
					}
					this.Save();
				}
				this._selectedSlotForSwap = -1;
				EasyPool.Free<List<short>>(displayInfoList);
			}
		}
	}

	// Token: 0x06003B44 RID: 15172 RVA: 0x001DFDEC File Offset: 0x001DDFEC
	public void Refresh()
	{
		List<short> customInfoList = this.WorldMapModel.CustomMapBlockCharInfoList;
		this._cachedCustomInfoList.Clear();
		this._cachedCustomInfoList.AddRange(customInfoList);
		List<short> customButtonList = this.WorldMapModel.CustomMapBlockCharButtonList;
		this._cachedCustomButtonList.Clear();
		this._cachedCustomButtonList.AddRange(customButtonList);
		this.RefreshInner();
	}

	// Token: 0x06003B45 RID: 15173 RVA: 0x001DFE4C File Offset: 0x001DE04C
	private void RefreshInner()
	{
		List<short> customInfoList = this._cachedCustomInfoList;
		int configureItemCount = MapBlockCharCustomInfo.Instance.Count;
		this._settingConfigureItemByCode = true;
		for (int i = 0; i < configureItemCount; i++)
		{
			CommonConfigureBase item = this.configureLayout.GetChild(i).GetComponent<CommonConfigureBase>();
			MapBlockCharCustomInfoItem info = MapBlockCharCustomInfo.Instance[i];
			CommonSwitch commonSwitch = item.GetComponentInChildren<CommonSwitch>();
			bool selected = customInfoList.Contains((short)i);
			commonSwitch.isOn = selected;
			bool isSpecial = info.TemplateId == this.InfoSpecialKey;
			commonSwitch.interactable = (!isSpecial && (selected || customInfoList.Count < 6));
		}
		List<short> customButtonList = this._cachedCustomButtonList;
		int buttonConfigureItemCount = MapBlockCharCustomButton.Instance.Count;
		for (int j = 0; j < buttonConfigureItemCount; j++)
		{
			CommonConfigureBase item2 = this.buttonConfigureLayout.GetChild(j).GetComponent<CommonConfigureBase>();
			MapBlockCharCustomButtonItem buttonInfo = MapBlockCharCustomButton.Instance[j];
			CommonSwitch commonSwitch2 = item2.GetComponentInChildren<CommonSwitch>();
			bool selected2 = customButtonList.Contains((short)j);
			commonSwitch2.isOn = selected2;
			bool isCannotCancel = buttonInfo.TemplateId == this._buttonCannotCancelKey;
			commonSwitch2.interactable = (!isCannotCancel && (selected2 || customButtonList.Count < 8));
		}
		this._settingConfigureItemByCode = false;
		List<short> displayInfoList = EasyPool.Get<List<short>>();
		displayInfoList.Clear();
		displayInfoList.AddRange(customInfoList);
		displayInfoList.Remove(this.InfoSpecialKey);
		for (int k = 0; k < 5; k++)
		{
			Refers slot = this.slotGrid.GetChild(k).GetComponent<Refers>();
			slot.gameObject.SetActive(k < displayInfoList.Count);
			bool flag = k >= displayInfoList.Count;
			if (!flag)
			{
				CImage infoIcon = slot.CGet<CImage>("InfoIcon");
				CImage selectedImage = slot.CGet<CImage>("Selected");
				TextMeshProUGUI infoLabel = slot.CGet<TextMeshProUGUI>("InfoLabel");
				CToggleObsolete toggle = slot.GetComponent<CToggleObsolete>();
				TooltipInvoker tip = slot.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				MapBlockCharCustomInfoItem info2 = MapBlockCharCustomInfo.Instance[displayInfoList[k]];
				tip.RuntimeParam.Set("arg0", info2.Name);
				EMapBlockCharCustomInfoDisplayType type = info2.DisplayType;
				infoIcon.gameObject.SetActive(type == EMapBlockCharCustomInfoDisplayType.Icon);
				infoLabel.gameObject.SetActive(type == EMapBlockCharCustomInfoDisplayType.Text);
				selectedImage.gameObject.SetActive(this._selectedSlotForSwap == k);
				toggle.onValueChanged.RemoveAllListeners();
				int slotIndex = k;
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (isOn)
					{
						this.OnSlotSelected(slotIndex);
					}
				});
			}
		}
		EasyPool.Free<List<short>>(displayInfoList);
	}

	// Token: 0x04002A93 RID: 10899
	[SerializeField]
	private RectTransform configureLayout;

	// Token: 0x04002A94 RID: 10900
	[SerializeField]
	private CommonConfigureBase configureItemTemplate;

	// Token: 0x04002A95 RID: 10901
	[SerializeField]
	private RectTransform slotGrid;

	// Token: 0x04002A96 RID: 10902
	[SerializeField]
	private RectTransform buttonConfigureLayout;

	// Token: 0x04002A97 RID: 10903
	[SerializeField]
	private CommonConfigureBase buttonConfigureItemTemplate;

	// Token: 0x04002A98 RID: 10904
	private readonly List<short> _cachedCustomInfoList = new List<short>();

	// Token: 0x04002A99 RID: 10905
	private readonly List<short> _cachedCustomButtonList = new List<short>();

	// Token: 0x04002A9A RID: 10906
	private bool _settingConfigureItemByCode;

	// Token: 0x04002A9B RID: 10907
	private int _selectedSlotForSwap = -1;

	// Token: 0x04002A9C RID: 10908
	private short InfoSpecialKey = 10;

	// Token: 0x04002A9D RID: 10909
	private short _buttonCannotCancelKey = 9;

	// Token: 0x04002A9E RID: 10910
	private const int MaxSlot = 6;

	// Token: 0x04002A9F RID: 10911
	private const int MaxDisplaySlot = 5;

	// Token: 0x04002AA0 RID: 10912
	private const int MaxButtonSlot = 8;
}
