using System;
using System.Collections.Generic;
using Config;
using UILogic.DisplayDataStructure;

namespace Game.Components.SortAndFilter.InstantNotification
{
	// Token: 0x02000DF9 RID: 3577
	public class InstantNotificationImportanceSecondaryMenu : DetailedFilterMenuLogic<NotificationItem>
	{
		// Token: 0x170012CB RID: 4811
		// (get) Token: 0x0600AABD RID: 43709 RVA: 0x004E93D8 File Offset: 0x004E75D8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012CC RID: 4812
		// (get) Token: 0x0600AABE RID: 43710 RVA: 0x004E93DB File Offset: 0x004E75DB
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AABF RID: 43711 RVA: 0x004E93DE File Offset: 0x004E75DE
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance);
		}

		// Token: 0x0600AAC0 RID: 43712 RVA: 0x004E93EC File Offset: 0x004E75EC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance_0)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance_1)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Importance_2)
				}
			};
		}

		// Token: 0x0600AAC1 RID: 43713 RVA: 0x004E9468 File Offset: 0x004E7668
		public override bool IsDataMatch(NotificationItem data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				InstantNotificationItem config = InstantNotification.Instance[data.RecordType];
				bool flag = config.Importance == (sbyte)selectedSubType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
