using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Extra;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008BE RID: 2238
	public class MonthNotifyReviewCardItem : MonoBehaviour
	{
		// Token: 0x17000C99 RID: 3225
		// (get) Token: 0x06006AAB RID: 27307 RVA: 0x00313FDF File Offset: 0x003121DF
		private MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x06006AAC RID: 27308 RVA: 0x00313FE8 File Offset: 0x003121E8
		public void Set(Action<NotificationItem, ArgumentBox> action, NotificationItem item)
		{
			MonthlyNotificationItem config = ViewMonthNotify.GetConfig(item);
			this._sortingGroup = (int)config.SortingGroup;
			this.icon.SetSprite(config.Icon, false, null);
			bool flag = item.RenderInfoList.Count > 1;
			if (flag)
			{
				this.mergeAmountBack.SetActive(true);
				this.mergeAmount.text = item.RenderInfoList.Count.ToString();
			}
			else
			{
				this.mergeAmountBack.SetActive(false);
			}
			this.toggle.SetIsOnWithoutNotify(this.MonthlyNotificationSortingGroups.Data[this._sortingGroup].IsOnTop);
			this.toggle.onValueChanged.ResetListener(new Action<bool>(this.OnClickToggle));
			TooltipInvoker tooltipInvoker = this.tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			action(item, this.tips.RuntimeParam);
			this.tips.Refresh(true, -1);
		}

		// Token: 0x06006AAD RID: 27309 RVA: 0x003140F4 File Offset: 0x003122F4
		private void OnClickToggle(bool value)
		{
			this.MonthlyNotificationSortingGroups.Data[this._sortingGroup].IsOnTop = value;
			GEvent.OnEvent(UiEvents.OnMonthNotifySortingGroupChanged, null);
			ExtraDomainMethod.Call.SetMonthlyNotificationSortingGroup(this.MonthlyNotificationSortingGroups.Data[this._sortingGroup]);
		}

		// Token: 0x04004D16 RID: 19734
		[SerializeField]
		protected CImage icon;

		// Token: 0x04004D17 RID: 19735
		[SerializeField]
		protected TooltipInvoker tips;

		// Token: 0x04004D18 RID: 19736
		[SerializeField]
		protected TextMeshProUGUI mergeAmount;

		// Token: 0x04004D19 RID: 19737
		[SerializeField]
		protected GameObject mergeAmountBack;

		// Token: 0x04004D1A RID: 19738
		[SerializeField]
		protected CToggle toggle;

		// Token: 0x04004D1B RID: 19739
		private int _sortingGroup;
	}
}
