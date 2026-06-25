using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C6 RID: 2246
	public class ViewMonthNotifySortingGroupSettings : UIBase
	{
		// Token: 0x17000CA5 RID: 3237
		// (get) Token: 0x06006B33 RID: 27443 RVA: 0x00318A74 File Offset: 0x00316C74
		private static MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x06006B34 RID: 27444 RVA: 0x00318A7B File Offset: 0x00316C7B
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06006B35 RID: 27445 RVA: 0x00318A80 File Offset: 0x00316C80
		private void Awake()
		{
			foreach (int id in ViewMonthNotifySortingGroupSettings.MonthlyNotificationSortingGroups.Data.Keys)
			{
				this.Icons.Add(id, new List<short>());
				this._sectionTypes.Add(id, new HashSet<EMonthlyNotificationSectionType>());
			}
			foreach (MonthlyNotificationItem item in ((IEnumerable<MonthlyNotificationItem>)MonthlyNotification.Instance))
			{
				bool flag = item.SortingGroup >= 0;
				if (flag)
				{
					this.Icons[(int)item.SortingGroup].Add(item.TemplateId);
					this._sectionTypes[(int)item.SortingGroup].Add(item.SectionType);
				}
			}
			int count = 3;
			for (int i = 0; i <= count; i++)
			{
				CToggle obj = Object.Instantiate<CToggle>(this.sectionToggleTemplate, this.sectionToggleGroup.transform);
				obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ViewMonthNotifySortingGroupSettings.MonthlyNotificationSortingGroups.ReviewTitles[(EMonthlyNotificationSectionType)i].Tr();
				obj.transform.GetChild(2).gameObject.SetActive(i != count);
				this.sectionToggleGroup.Add(obj);
			}
			this.sectionToggleGroup.Init(-1);
			this.sectionToggleGroup.OnActiveIndexChange += this.OnSectionToggleGroupChange;
			this.searchField.onValueChanged.RemoveAllListeners();
			this.searchField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearch));
			this.scroll.InitPageCount();
			this.scroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06006B36 RID: 27446 RVA: 0x00318C98 File Offset: 0x00316E98
		private void OnEnable()
		{
			this.searchField.SetTextWithoutNotify(string.Empty);
			this.OnSearch(string.Empty);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06006B37 RID: 27447 RVA: 0x00318CC4 File Offset: 0x00316EC4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "ButtonExit" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06006B38 RID: 27448 RVA: 0x00318CF4 File Offset: 0x00316EF4
		private void OnSearch(string value)
		{
			bool flag = CommonUtils.FixToShowAbleString(ref value, this.searchField.textComponent.font);
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(value);
			}
			this.Filter();
		}

		// Token: 0x06006B39 RID: 27449 RVA: 0x00318D31 File Offset: 0x00316F31
		private void OnSectionToggleGroupChange(int _, int __)
		{
			this.Filter();
		}

		// Token: 0x06006B3A RID: 27450 RVA: 0x00318D3C File Offset: 0x00316F3C
		private void Filter()
		{
			this._filteredList.Clear();
			EMonthlyNotificationSectionType sectionType = (EMonthlyNotificationSectionType)this.sectionToggleGroup.GetActiveIndex();
			foreach (NotificationSortingGroup group in ViewMonthNotifySortingGroupSettings.MonthlyNotificationSortingGroups.Data.Values)
			{
				MonthlyNotificationSortingGroupItem config = MonthlyNotificationSortingGroup.Instance[group.Id];
				string groupName = MonthlyNotificationSortingGroup.Instance[group.Id].Name;
				bool flag = (config.DlcAppId == 0U || SingletonObject.getInstance<DlcManager>().IsDlcInstalled(config.DlcAppId)) && groupName.Contains(this.searchField.text) && this._sectionTypes[group.Id].Contains(sectionType);
				if (flag)
				{
					this._filteredList.Add(group.Id);
				}
			}
			this.scroll.SetDataCount(this._filteredList.Count);
		}

		// Token: 0x06006B3B RID: 27451 RVA: 0x00318E54 File Offset: 0x00317054
		private void OnItemRender(int index, GameObject obj)
		{
			MonthNotifySortingGroupSettingItem item = obj.GetComponent<MonthNotifySortingGroupSettingItem>();
			item.Init(this);
			item.Set(this._filteredList[index]);
		}

		// Token: 0x04004DD4 RID: 19924
		[SerializeField]
		private CToggleGroup sectionToggleGroup;

		// Token: 0x04004DD5 RID: 19925
		[SerializeField]
		private CToggle sectionToggleTemplate;

		// Token: 0x04004DD6 RID: 19926
		[SerializeField]
		private TMP_InputField searchField;

		// Token: 0x04004DD7 RID: 19927
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04004DD8 RID: 19928
		[NonSerialized]
		public Dictionary<int, List<short>> Icons = new Dictionary<int, List<short>>();

		// Token: 0x04004DD9 RID: 19929
		private Dictionary<int, HashSet<EMonthlyNotificationSectionType>> _sectionTypes = new Dictionary<int, HashSet<EMonthlyNotificationSectionType>>();

		// Token: 0x04004DDA RID: 19930
		private List<int> _filteredList = new List<int>();
	}
}
