using System;
using System.Collections.Generic;
using Config;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CE2 RID: 3298
	public class TypeFilterLine : DetailedFilterLineLogic<SecretSortAndFilterData>
	{
		// Token: 0x17001166 RID: 4454
		// (get) Token: 0x0600A650 RID: 42576 RVA: 0x004D6B25 File Offset: 0x004D4D25
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001167 RID: 4455
		// (get) Token: 0x0600A651 RID: 42577 RVA: 0x004D6B28 File Offset: 0x004D4D28
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001168 RID: 4456
		// (get) Token: 0x0600A652 RID: 42578 RVA: 0x004D6B2B File Offset: 0x004D4D2B
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A653 RID: 42579 RVA: 0x004D6B30 File Offset: 0x004D4D30
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600A654 RID: 42580 RVA: 0x004D6B43 File Offset: 0x004D4D43
		protected override IEnumerable<DetailedFilterMenuLogic<SecretSortAndFilterData>> GenerateMenus()
		{
			yield return new GeneralFilterMenu();
			foreach (short templateId in SecretFilterOptionHelper.GetGeneralFilterTemplateIds())
			{
				SecretInformationGeneralFilterItem configItem = SecretInformationGeneralFilter.Instance.GetItem(templateId);
				SecretInformationGeneralFilterItem secretInformationGeneralFilterItem = configItem;
				List<short> list = (secretInformationGeneralFilterItem != null) ? secretInformationGeneralFilterItem.DetailedFilter : null;
				bool flag = list != null && list.Count > 0;
				if (flag)
				{
					yield return new GeneralDetailedMenu(templateId);
				}
				configItem = null;
			}
			List<short>.Enumerator enumerator = default(List<short>.Enumerator);
			yield break;
			yield break;
		}
	}
}
