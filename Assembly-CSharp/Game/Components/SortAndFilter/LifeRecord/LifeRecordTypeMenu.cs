using System;
using System.Collections.Generic;
using GameData.Domains.LifeRecord;

namespace Game.Components.SortAndFilter.LifeRecord
{
	// Token: 0x02000D1F RID: 3359
	public class LifeRecordTypeMenu : DetailedFilterMenuLogic<TransferableRecord>
	{
		// Token: 0x1700119B RID: 4507
		// (get) Token: 0x0600A755 RID: 42837 RVA: 0x004DD9C4 File Offset: 0x004DBBC4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700119C RID: 4508
		// (get) Token: 0x0600A756 RID: 42838 RVA: 0x004DD9C7 File Offset: 0x004DBBC7
		public override int Id
		{
			get
			{
				return EJieqingMurderMenuId.LifeRecordType.ToInt();
			}
		}

		// Token: 0x0600A757 RID: 42839 RVA: 0x004DD9D4 File Offset: 0x004DBBD4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Resource_Choosy_ItemMenu;
		}

		// Token: 0x0600A758 RID: 42840 RVA: 0x004DD9F0 File Offset: 0x004DBBF0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Great),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Normal),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Relation),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Study),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Produce),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Combat),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Negative),
				new FilterDropdownItemConfig(LanguageKey.LK_LifeRecord_DisplayType_Crime)
			};
		}

		// Token: 0x0600A759 RID: 42841 RVA: 0x004DDABC File Offset: 0x004DBCBC
		public override bool IsDataMatch(TransferableRecord data, IReadOnlyCollection<int> selectedIndices)
		{
			return true;
		}

		// Token: 0x04008367 RID: 33639
		private readonly List<string> _organizationOptions = new List<string>();
	}
}
