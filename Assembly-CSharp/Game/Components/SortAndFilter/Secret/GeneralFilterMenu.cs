using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CE4 RID: 3300
	public class GeneralFilterMenu : DetailedFilterMenuLogic<SecretSortAndFilterData>
	{
		// Token: 0x17001169 RID: 4457
		// (get) Token: 0x0600A657 RID: 42583 RVA: 0x004D6BCC File Offset: 0x004D4DCC
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700116A RID: 4458
		// (get) Token: 0x0600A658 RID: 42584 RVA: 0x004D6BCF File Offset: 0x004D4DCF
		public override int Id
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x0600A659 RID: 42585 RVA: 0x004D6BD4 File Offset: 0x004D4DD4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_Filter;
		}

		// Token: 0x0600A65A RID: 42586 RVA: 0x004D6BF0 File Offset: 0x004D4DF0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			this._generalFilterTemplateIds.Clear();
			foreach (short templateId in SecretFilterOptionHelper.GetGeneralFilterTemplateIds())
			{
				SecretInformationGeneralFilterItem config = SecretInformationGeneralFilter.Instance.GetItem(templateId);
				bool flag = config == null;
				if (!flag)
				{
					this._generalFilterTemplateIds.Add(templateId);
					configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(config.Name)));
				}
			}
			return configs;
		}

		// Token: 0x0600A65B RID: 42587 RVA: 0x004D6C98 File Offset: 0x004D4E98
		public override bool IsDataMatch(SecretSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				bool flag = selectedSubType < 0 || selectedSubType >= this._generalFilterTemplateIds.Count;
				if (!flag)
				{
					bool flag2 = data.GetConfig.GeneralFilterType == this._generalFilterTemplateIds[selectedSubType];
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04008311 RID: 33553
		private readonly List<short> _generalFilterTemplateIds = new List<short>();
	}
}
