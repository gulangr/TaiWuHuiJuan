using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using UILogic.DisplayDataStructure;

namespace CommonSortAndFilterLegacy.InstantNotification
{
	// Token: 0x02000564 RID: 1380
	public class InstantNotificationImportanceSecondaryMenu : StaticDetailedFilterMenuBase<NotificationItem>
	{
		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06004454 RID: 17492 RVA: 0x002096F8 File Offset: 0x002078F8
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06004455 RID: 17493 RVA: 0x002096FB File Offset: 0x002078FB
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004456 RID: 17494 RVA: 0x00209700 File Offset: 0x00207900
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance);
		}

		// Token: 0x06004457 RID: 17495 RVA: 0x00209724 File Offset: 0x00207924
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance_0)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance_1)
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance_2)
				}
			};
		}

		// Token: 0x06004458 RID: 17496 RVA: 0x002097C4 File Offset: 0x002079C4
		public override bool IsDataMatch(NotificationItem data, IReadOnlyCollection<int> selectedIndices)
		{
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int selectedSubType = enumerator.Current;
					InstantNotificationItem config = InstantNotification.Instance[data.RecordType];
					return config.Importance == (sbyte)selectedSubType;
				}
			}
			return false;
		}
	}
}
