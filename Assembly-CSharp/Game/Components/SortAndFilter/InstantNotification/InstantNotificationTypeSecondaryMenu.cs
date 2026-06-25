using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using UILogic.DisplayDataStructure;

namespace Game.Components.SortAndFilter.InstantNotification
{
	// Token: 0x02000DFA RID: 3578
	public class InstantNotificationTypeSecondaryMenu : DetailedFilterMenuLogic<NotificationItem>
	{
		// Token: 0x170012CD RID: 4813
		// (get) Token: 0x0600AAC3 RID: 43715 RVA: 0x004E94E5 File Offset: 0x004E76E5
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012CE RID: 4814
		// (get) Token: 0x0600AAC4 RID: 43716 RVA: 0x004E94E8 File Offset: 0x004E76E8
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AAC5 RID: 43717 RVA: 0x004E94EB File Offset: 0x004E76EB
		public override StringKey GetMenuBarLabel()
		{
			return StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type);
		}

		// Token: 0x0600AAC6 RID: 43718 RVA: 0x004E94F8 File Offset: 0x004E76F8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_0)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_1)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_2)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_3)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_4)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_5)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_Filter_InstantNotification_Type_6)
				}
			};
		}

		// Token: 0x0600AAC7 RID: 43719 RVA: 0x004E95F8 File Offset: 0x004E77F8
		public override bool IsDataMatch(NotificationItem data, IReadOnlyCollection<int> selectedIndices)
		{
			InstantNotificationItem config = InstantNotification.Instance[data.RecordType];
			return selectedIndices.Any((int selectedSubType) => config.Type == (EInstantNotificationType)selectedSubType);
		}
	}
}
