using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using UILogic.DisplayDataStructure;

namespace CommonSortAndFilterLegacy.InstantNotification
{
	// Token: 0x02000565 RID: 1381
	public class InstantNotificationTypeSecondaryMenu : StaticDetailedFilterMenuBase<NotificationItem>
	{
		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x0600445A RID: 17498 RVA: 0x00209835 File Offset: 0x00207A35
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x0600445B RID: 17499 RVA: 0x00209838 File Offset: 0x00207A38
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600445C RID: 17500 RVA: 0x0020983C File Offset: 0x00207A3C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type);
		}

		// Token: 0x0600445D RID: 17501 RVA: 0x00209860 File Offset: 0x00207A60
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_0)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_1)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_2)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_3)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_4)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_5)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_6)
				}
			};
		}

		// Token: 0x0600445E RID: 17502 RVA: 0x002099B4 File Offset: 0x00207BB4
		public override bool IsDataMatch(NotificationItem data, IReadOnlyCollection<int> selectedIndices)
		{
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int selectedSubType = enumerator.Current;
					InstantNotificationItem config = InstantNotification.Instance[data.RecordType];
					return config.Type == (EInstantNotificationType)selectedSubType;
				}
			}
			return false;
		}
	}
}
