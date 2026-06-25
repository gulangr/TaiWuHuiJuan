using System;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Extra;
using TMPro;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C0 RID: 2240
	public class MonthNotifySortingGroupSettingItem : MonoBehaviour
	{
		// Token: 0x17000C9C RID: 3228
		// (get) Token: 0x06006AB9 RID: 27321 RVA: 0x00314448 File Offset: 0x00312648
		private static MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x06006ABA RID: 27322 RVA: 0x00314450 File Offset: 0x00312650
		public void Init(ViewMonthNotifySortingGroupSettings parent)
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._parent = parent;
				this.iconScroll.InitPageCount();
				this.iconScroll.OnItemRender += this.OnItemRender;
				this.toggleOnTop.onValueChanged.ResetListener(new Action<bool>(this.OnClickToggleOnTop));
				this.toggleHide.onValueChanged.ResetListener(new Action<bool>(this.OnClickHide));
			}
		}

		// Token: 0x06006ABB RID: 27323 RVA: 0x003144D0 File Offset: 0x003126D0
		public void Set(int id)
		{
			this._id = id;
			MonthlyNotificationSortingGroupItem config = MonthlyNotificationSortingGroup.Instance[this._id];
			this.toggleOnTop.enabled = config.OnTop;
			this.toggleHide.enabled = config.Hidden;
			this.toggleOnTop.SetIsOnWithoutNotify(MonthNotifySortingGroupSettingItem.MonthlyNotificationSortingGroups.Data[this._id].IsOnTop);
			this.toggleHide.SetIsOnWithoutNotify(MonthNotifySortingGroupSettingItem.MonthlyNotificationSortingGroups.Data[this._id].IsHidden);
			this.toggleHide.GetComponent<TooltipInvoker>().PresetParam = new string[]
			{
				config.Hidden ? LanguageKey.LK_MonthlyNotificationSortingGroupSettings_Tip_Hide.Tr() : LanguageKey.LK_MonthlyNotificationSortingGroupSettings_Tip_Hide_Disable.Tr()
			};
			this.titleLabel.text = config.Name;
			this.descLabel.text = config.Desc;
			this.iconScroll.SetDataCount(this._parent.Icons[this._id].Count);
		}

		// Token: 0x06006ABC RID: 27324 RVA: 0x003145E8 File Offset: 0x003127E8
		private void OnClickToggleOnTop(bool value)
		{
			MonthNotifySortingGroupSettingItem.MonthlyNotificationSortingGroups.Data[this._id].IsOnTop = value;
			GEvent.OnEvent(UiEvents.OnMonthNotifySortingGroupChanged, null);
			ExtraDomainMethod.Call.SetMonthlyNotificationSortingGroup(MonthNotifySortingGroupSettingItem.MonthlyNotificationSortingGroups.Data[this._id]);
		}

		// Token: 0x06006ABD RID: 27325 RVA: 0x0031463C File Offset: 0x0031283C
		private void OnClickHide(bool value)
		{
			MonthNotifySortingGroupSettingItem.MonthlyNotificationSortingGroups.Data[this._id].IsHidden = value;
			GEvent.OnEvent(UiEvents.OnMonthNotifySortingGroupChanged, null);
			ExtraDomainMethod.Call.SetMonthlyNotificationSortingGroup(MonthNotifySortingGroupSettingItem.MonthlyNotificationSortingGroups.Data[this._id]);
		}

		// Token: 0x06006ABE RID: 27326 RVA: 0x0031468E File Offset: 0x0031288E
		private void OnItemRender(int index, GameObject obj)
		{
			obj.GetComponent<CImage>().SetSprite(MonthlyNotification.Instance[this._parent.Icons[this._id][index]].Icon, false, null);
		}

		// Token: 0x04004D24 RID: 19748
		[SerializeField]
		protected TextMeshProUGUI titleLabel;

		// Token: 0x04004D25 RID: 19749
		[SerializeField]
		protected TextMeshProUGUI descLabel;

		// Token: 0x04004D26 RID: 19750
		[SerializeField]
		protected CToggle toggleOnTop;

		// Token: 0x04004D27 RID: 19751
		[SerializeField]
		protected CToggle toggleHide;

		// Token: 0x04004D28 RID: 19752
		[SerializeField]
		protected InfinityScroll iconScroll;

		// Token: 0x04004D29 RID: 19753
		private bool _inited = false;

		// Token: 0x04004D2A RID: 19754
		private int _id;

		// Token: 0x04004D2B RID: 19755
		private ViewMonthNotifySortingGroupSettings _parent;
	}
}
