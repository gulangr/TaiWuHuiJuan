using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E0D RID: 3597
	public class SwordTombMenu : DetailedFilterMenuLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012E8 RID: 4840
		// (get) Token: 0x0600AB12 RID: 43794 RVA: 0x004EA1B8 File Offset: 0x004E83B8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012E9 RID: 4841
		// (get) Token: 0x0600AB13 RID: 43795 RVA: 0x004EA1BB File Offset: 0x004E83BB
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600AB14 RID: 43796 RVA: 0x004EA1C0 File Offset: 0x004E83C0
		public override StringKey GetMenuBarLabel()
		{
			return InformationType.DefValue.SwordTomb.Name;
		}

		// Token: 0x0600AB15 RID: 43797 RVA: 0x004EA1E4 File Offset: 0x004E83E4
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			this._options.Clear();
			for (sbyte i = 0; i < 4; i += 1)
			{
				EInformationInfoSwordInformationType type = (EInformationInfoSwordInformationType)i;
				List<FilterDropdownItemConfig> list = dropdownConfigs;
				FilterDropdownItemConfig item = default(FilterDropdownItemConfig);
				if (!true)
				{
				}
				string directString;
				switch (type)
				{
				case EInformationInfoSwordInformationType.SwordTombHeaven:
					directString = LocalStringManager.Get(LanguageKey.LK_NormalInformationSwordTombTypeA);
					break;
				case EInformationInfoSwordInformationType.SwordTombEarth:
					directString = LocalStringManager.Get(LanguageKey.LK_NormalInformationSwordTombTypeB);
					break;
				case EInformationInfoSwordInformationType.SwordTombHuman:
					directString = LocalStringManager.Get(LanguageKey.LK_NormalInformationSwordTombTypeC);
					break;
				case EInformationInfoSwordInformationType.SwordTombNormal:
					directString = LocalStringManager.Get(LanguageKey.LK_NormalInformationSwordTombTypeD);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				item.Text = directString;
				list.Add(item);
				this._options.Add(i);
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AB16 RID: 43798 RVA: 0x004EA2B4 File Offset: 0x004E84B4
		public override bool IsDataMatch(InformationSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectionIndex in selectedIndices)
			{
				bool flag = (EInformationInfoSwordInformationType)this._options[selectionIndex] == data.SwordTombType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040084DF RID: 34015
		private readonly List<sbyte> _options = new List<sbyte>();
	}
}
