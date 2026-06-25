using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Extra;
using GameData.Domains.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000390 RID: 912
public class UI_MonthlyNotificationSortingGroupSettings : UIBase
{
	// Token: 0x06003622 RID: 13858 RVA: 0x001B3D4C File Offset: 0x001B1F4C
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = this._icons != null;
		if (!flag)
		{
			this._icons = new Dictionary<int, List<short>>();
			this._sectionTypes = new Dictionary<int, HashSet<EMonthlyNotificationSectionType>>();
			UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups = SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			foreach (int id in UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data.Keys)
			{
				this._icons.Add(id, new List<short>());
				this._sectionTypes.Add(id, new HashSet<EMonthlyNotificationSectionType>());
			}
			foreach (MonthlyNotificationItem item in ((IEnumerable<MonthlyNotificationItem>)MonthlyNotification.Instance))
			{
				bool flag2 = item.SortingGroup >= 0;
				if (flag2)
				{
					this._icons[(int)item.SortingGroup].Add(item.TemplateId);
					this._sectionTypes[(int)item.SortingGroup].Add(item.SectionType);
				}
			}
		}
	}

	// Token: 0x06003623 RID: 13859 RVA: 0x001B3E88 File Offset: 0x001B2088
	public void Awake()
	{
		this._groupPool = new PoolItem("UI_EventLog_DialogPrefab", base.CGet<GameObject>("ItemTemplate"));
		this._iconPool = new PoolItem("UI_EventLog_DialogPrefab", base.CGet<GameObject>("IconTemplate"));
		this._filterController = base.CGet<HorizontalPageSwitchController>("PageSwitchController");
		this._filterController.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshMonthlyNotificationSectionFilterItem);
		this._filterController.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetMonthlyNotificationSectionFilterSelectState);
		this._filterController.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectMonthlyNotificationSectionFilterIndexChange));
		this._searchText = base.CGet<TMP_InputField>("SearchOwner");
		this._searchText.onValueChanged.RemoveAllListeners();
		this._searchText.onValueChanged.AddListener(delegate(string inputValue)
		{
			this.FilterOwnerListBySearchName();
		});
		this._searchText.onEndEdit.RemoveAllListeners();
		this._searchText.onEndEdit.AddListener(delegate(string inputValue)
		{
			this.FilterOwnerListBySearchName();
		});
	}

	// Token: 0x06003624 RID: 13860 RVA: 0x001B3F90 File Offset: 0x001B2190
	private void OnEnable()
	{
		this._filterController.InitPageCount(5, 0, false);
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06003625 RID: 13861 RVA: 0x001B3FB0 File Offset: 0x001B21B0
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "ButtonConfirm";
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
	}

	// Token: 0x06003626 RID: 13862 RVA: 0x001B3FE7 File Offset: 0x001B21E7
	private void OnDestroy()
	{
		PoolItem iconPool = this._iconPool;
		if (iconPool != null)
		{
			iconPool.Destroy();
		}
		this._iconPool = null;
		PoolItem groupPool = this._groupPool;
		if (groupPool != null)
		{
			groupPool.Destroy();
		}
		this._groupPool = null;
	}

	// Token: 0x06003627 RID: 13863 RVA: 0x001B401C File Offset: 0x001B221C
	private void RefreshScrollView()
	{
		Transform content = base.CGet<GameObject>("Content").transform;
		for (int i = 0; i < this._filteredGroups.Count; i++)
		{
			GameObject obj = (i >= content.childCount) ? this._groupPool.GetObject() : content.GetChild(i).gameObject;
			this.RenderGroupItem(this._filteredGroups[i], obj.GetComponent<Refers>());
			obj.transform.SetParent(content.transform, false);
			obj.SetActive(true);
		}
		bool flag = this._filteredGroups.Count < content.childCount;
		if (flag)
		{
			for (int j = this._filteredGroups.Count; j < content.childCount; j++)
			{
				this._groupPool.DestroyObject(content.GetChild(j).gameObject);
			}
		}
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x001B410C File Offset: 0x001B230C
	private void RenderGroupItem(int id, Refers refers)
	{
		MonthlyNotificationSortingGroupItem config = MonthlyNotificationSortingGroup.Instance[id];
		CButtonObsolete buttonOnTop = refers.CGet<CButtonObsolete>("ButtonOnTop");
		CButtonObsolete buttonHide = refers.CGet<CButtonObsolete>("ButtonHide");
		TextMeshProUGUI buttonOnTopText = refers.CGet<TextMeshProUGUI>("ButtonOnTopText");
		TextMeshProUGUI buttonHideText = refers.CGet<TextMeshProUGUI>("ButtonHideText");
		buttonOnTop.enabled = config.OnTop;
		buttonOnTopText.text = LocalStringManager.Get(LanguageKey.LK_MonthlyNotificationSortingGroupSettings_OnTop).SetColor(config.OnTop ? "pinkyellow" : "grey");
		buttonHide.enabled = config.Hidden;
		buttonHideText.text = LocalStringManager.Get(LanguageKey.LK_MonthlyNotificationSortingGroupSettings_Hide).SetColor(config.Hidden ? "pinkyellow" : "grey");
		refers.CGet<GameObject>("CheckMarkOnTop").SetActive(UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsOnTop);
		refers.CGet<GameObject>("CheckMarkHide").SetActive(UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsHidden);
		buttonHide.GetComponent<TooltipInvoker>().PresetParam[0] = LocalStringManager.Get(config.Hidden ? LanguageKey.LK_MonthlyNotificationSortingGroupSettings_Tip_Hide : LanguageKey.LK_MonthlyNotificationSortingGroupSettings_Tip_Hide_Disable);
		refers.CGet<TextMeshProUGUI>("Title").text = config.Name;
		refers.CGet<TextMeshProUGUI>("Desc").text = config.Desc;
		this.RenderIconItems(id, refers);
		buttonOnTop.ClearAndAddListener(delegate
		{
			UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsOnTop = !UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsOnTop;
			refers.CGet<GameObject>("CheckMarkOnTop").SetActive(UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsOnTop);
			ExtraDomainMethod.Call.SetMonthlyNotificationSortingGroup(UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id]);
		});
		buttonHide.ClearAndAddListener(delegate
		{
			UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsHidden = !UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsHidden;
			refers.CGet<GameObject>("CheckMarkHide").SetActive(UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id].IsHidden);
			ExtraDomainMethod.Call.SetMonthlyNotificationSortingGroup(UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data[id]);
		});
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x001B42EC File Offset: 0x001B24EC
	private void RenderIconItems(int id, Refers refers)
	{
		Transform container = refers.CGet<GameObject>("IconContainer").transform;
		for (int i = 0; i < this._icons[id].Count; i++)
		{
			GameObject obj = (i >= container.childCount) ? this._iconPool.GetObject() : container.GetChild(i).gameObject;
			obj.GetComponent<Refers>().CGet<CImage>("Icon").SetSprite(MonthlyNotification.Instance[this._icons[id][i]].Icon, false, null);
			obj.transform.SetParent(container.transform, false);
			obj.SetActive(true);
		}
		bool flag = this._icons[id].Count < container.childCount;
		if (flag)
		{
			for (int j = this._icons[id].Count; j < container.childCount; j++)
			{
				this._iconPool.DestroyObject(container.GetChild(j).gameObject);
			}
		}
		int childCount = container.childCount;
		container.GetComponent<HorizontalLayoutGroup>().spacing = ((childCount >= 10) ? ((float)(1002 - 90 * childCount) / (float)(childCount - 1)) : 10f);
	}

	// Token: 0x0600362A RID: 13866 RVA: 0x001B443F File Offset: 0x001B263F
	private void OnSelectMonthlyNotificationSectionFilterIndexChange(int index)
	{
		this.FilterGroupListBySectionType(index);
	}

	// Token: 0x0600362B RID: 13867 RVA: 0x001B444A File Offset: 0x001B264A
	private void SetMonthlyNotificationSectionFilterSelectState(Refers refers, bool isSelected)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = isSelected;
		refers.CGet<CToggleObsolete>("Toggle").interactable = (!isSelected && this._tempDisbaleRefer != refers);
	}

	// Token: 0x0600362C RID: 13868 RVA: 0x001B4484 File Offset: 0x001B2684
	private void RefreshMonthlyNotificationSectionFilterItem(int index, Refers refers)
	{
		TextMeshProUGUI labelOff = refers.CGet<TextMeshProUGUI>("LabelOff");
		TextMeshProUGUI labelOn = refers.CGet<TextMeshProUGUI>("LabelOn");
		CImage icon = refers.CGet<CImage>("Icon");
		CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
		icon.SetSprite(string.Format("monthnotify_eventmanage_icon_tab_{0}_0", index), true, null);
		toggle.onValueChanged.RemoveAllListeners();
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
			}
		});
	}

	// Token: 0x0600362D RID: 13869 RVA: 0x001B451C File Offset: 0x001B271C
	private void FilterGroupListBySectionType(int filterIndex)
	{
		bool lockFilter = this._lockFilter;
		if (!lockFilter)
		{
			this._lockSearch = true;
			this._searchText.text = "";
			this._lockSearch = false;
			this._filteredGroups.Clear();
			foreach (NotificationSortingGroup group in UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data.Values)
			{
				MonthlyNotificationSortingGroupItem config = MonthlyNotificationSortingGroup.Instance[group.Id];
				bool flag = this._sectionTypes[group.Id].Contains((EMonthlyNotificationSectionType)filterIndex) && (config.DlcAppId == 0U || SingletonObject.getInstance<DlcManager>().IsDlcInstalled(config.DlcAppId));
				if (flag)
				{
					this._filteredGroups.Add(group.Id);
				}
			}
			this.RefreshScrollView();
		}
	}

	// Token: 0x0600362E RID: 13870 RVA: 0x001B461C File Offset: 0x001B281C
	private void FilterOwnerListBySearchName()
	{
		bool lockSearch = this._lockSearch;
		if (!lockSearch)
		{
			bool flag = string.IsNullOrEmpty(this._searchText.text);
			if (flag)
			{
				this._filterController.SelectFirst();
				this.FilterGroupListBySectionType(0);
			}
			else
			{
				this._lockFilter = true;
				this._filterController.SelectFirst();
				this._lockFilter = false;
				this._filteredGroups.Clear();
				foreach (NotificationSortingGroup group in UI_MonthlyNotificationSortingGroupSettings._monthlyNotificationSortingGroups.Data.Values)
				{
					MonthlyNotificationSortingGroupItem config = MonthlyNotificationSortingGroup.Instance[group.Id];
					string groupName = MonthlyNotificationSortingGroup.Instance[group.Id].Name;
					bool flag2 = (config.DlcAppId == 0U || SingletonObject.getInstance<DlcManager>().IsDlcInstalled(config.DlcAppId)) && groupName.Contains(this._searchText.text);
					if (flag2)
					{
						this._filteredGroups.Add(group.Id);
					}
				}
				this.RefreshScrollView();
			}
		}
	}

	// Token: 0x0400273F RID: 10047
	private HorizontalPageSwitchController _filterController;

	// Token: 0x04002740 RID: 10048
	private TMP_InputField _searchText;

	// Token: 0x04002741 RID: 10049
	private PoolItem _groupPool;

	// Token: 0x04002742 RID: 10050
	private PoolItem _iconPool;

	// Token: 0x04002743 RID: 10051
	private static MonthlyNotificationSortingGroups _monthlyNotificationSortingGroups;

	// Token: 0x04002744 RID: 10052
	private List<int> _filteredGroups = new List<int>();

	// Token: 0x04002745 RID: 10053
	private Dictionary<int, List<short>> _icons = null;

	// Token: 0x04002746 RID: 10054
	private Dictionary<int, HashSet<EMonthlyNotificationSectionType>> _sectionTypes = null;

	// Token: 0x04002747 RID: 10055
	private const int ContainerWidth = 1002;

	// Token: 0x04002748 RID: 10056
	private const int IconWidth = 90;

	// Token: 0x04002749 RID: 10057
	private const int DefaultSpacing = 10;

	// Token: 0x0400274A RID: 10058
	private bool _lockFilter = false;

	// Token: 0x0400274B RID: 10059
	private bool _lockSearch = false;

	// Token: 0x0400274C RID: 10060
	private Refers _tempDisbaleRefer = null;
}
