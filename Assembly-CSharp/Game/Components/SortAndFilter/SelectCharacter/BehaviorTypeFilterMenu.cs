using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CF5 RID: 3317
	public class BehaviorTypeFilterMenu<TData> : SelectCharacterFilterMenuBase<TData> where TData : ISelectCharacterData
	{
		// Token: 0x17001178 RID: 4472
		// (get) Token: 0x0600A699 RID: 42649 RVA: 0x004D7C8D File Offset: 0x004D5E8D
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A69A RID: 42650 RVA: 0x004D7C90 File Offset: 0x004D5E90
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_1;
		}

		// Token: 0x0600A69B RID: 42651 RVA: 0x004D7CAC File Offset: 0x004D5EAC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from behaviorTypeItem in BehaviorType.Instance
			select new FilterDropdownItemConfig
			{
				Text = StringKey.CreateDirect(behaviorTypeItem.Name)
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A69C RID: 42652 RVA: 0x004D7CEC File Offset: 0x004D5EEC
		public override bool IsDataMatch(TData data, IReadOnlyCollection<int> selectedIndices)
		{
			CharacterDisplayDataForGeneralScrollList generalData = base.GetGeneralData(data);
			bool flag = generalData == null;
			return !flag && selectedIndices.Any((int index) => BehaviorType.Instance[index].TemplateId == (short)generalData.BehaviorType);
		}
	}
}
