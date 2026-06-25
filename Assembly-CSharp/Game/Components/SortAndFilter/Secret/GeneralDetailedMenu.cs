using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CE5 RID: 3301
	public class GeneralDetailedMenu : DetailedFilterMenuLogic<SecretSortAndFilterData>
	{
		// Token: 0x0600A65D RID: 42589 RVA: 0x004D6D3C File Offset: 0x004D4F3C
		public GeneralDetailedMenu(short generalFilterTemplateId)
		{
			this._generalFilterTemplateId = generalFilterTemplateId;
		}

		// Token: 0x1700116B RID: 4459
		// (get) Token: 0x0600A65E RID: 42590 RVA: 0x004D6D58 File Offset: 0x004D4F58
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700116C RID: 4460
		// (get) Token: 0x0600A65F RID: 42591 RVA: 0x004D6D5B File Offset: 0x004D4F5B
		public override int Id
		{
			get
			{
				return (int)this._generalFilterTemplateId;
			}
		}

		// Token: 0x1700116D RID: 4461
		// (get) Token: 0x0600A660 RID: 42592 RVA: 0x004D6D64 File Offset: 0x004D4F64
		public override MenuOptionIndex? Dependency
		{
			get
			{
				int index = SecretFilterOptionHelper.GetGeneralFilterTemplateIds().IndexOf(this._generalFilterTemplateId);
				bool flag = index < 0;
				MenuOptionIndex? result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = new MenuOptionIndex?(new MenuOptionIndex(-1, index));
				}
				return result;
			}
		}

		// Token: 0x1700116E RID: 4462
		// (get) Token: 0x0600A661 RID: 42593 RVA: 0x004D6DA7 File Offset: 0x004D4FA7
		private SecretInformationGeneralFilterItem ConfigItem
		{
			get
			{
				return SecretInformationGeneralFilter.Instance.GetItem(this._generalFilterTemplateId);
			}
		}

		// Token: 0x0600A662 RID: 42594 RVA: 0x004D6DB9 File Offset: 0x004D4FB9
		public override StringKey GetMenuBarLabel()
		{
			return (this.ConfigItem == null) ? LanguageKey.LK_Filter : StringKey.CreateKey(this.ConfigItem.Name);
		}

		// Token: 0x0600A663 RID: 42595 RVA: 0x004D6DE0 File Offset: 0x004D4FE0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._detailedFilterTemplateIds.Clear();
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			bool flag = this.ConfigItem == null || this.ConfigItem.DetailedFilter == null;
			List<FilterDropdownItemConfig> result;
			if (flag)
			{
				result = configs;
			}
			else
			{
				foreach (short templateId in this.ConfigItem.DetailedFilter)
				{
					SecretInformationDetailedFilterItem detailedItem = SecretInformationDetailedFilter.Instance.GetItem(templateId);
					bool flag2 = detailedItem == null;
					if (!flag2)
					{
						this._detailedFilterTemplateIds.Add(templateId);
						configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(detailedItem.Name)));
					}
				}
				result = configs;
			}
			return result;
		}

		// Token: 0x0600A664 RID: 42596 RVA: 0x004D6EB0 File Offset: 0x004D50B0
		public override bool IsDataMatch(SecretSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			foreach (int selectedSubType in selectedIndices)
			{
				bool flag = selectedSubType < 0 || selectedSubType >= this._detailedFilterTemplateIds.Count;
				if (!flag)
				{
					bool flag2 = data.GetConfig.DetailedFilterType == this._detailedFilterTemplateIds[selectedSubType];
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04008312 RID: 33554
		private readonly short _generalFilterTemplateId;

		// Token: 0x04008313 RID: 33555
		private readonly List<short> _detailedFilterTemplateIds = new List<short>();
	}
}
