using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000494 RID: 1172
	public class LifeSkillBookDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x0600414F RID: 16719 RVA: 0x002013E5 File Offset: 0x001FF5E5
		public override int Id
		{
			get
			{
				return 24;
			}
		}

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06004150 RID: 16720 RVA: 0x002013E9 File Offset: 0x001FF5E9
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x002013EC File Offset: 0x001FF5EC
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new LifeSkillReadStatusMenu();
			yield return new LifeSkillTypeMenu();
			yield break;
		}

		// Token: 0x06004152 RID: 16722 RVA: 0x002013FC File Offset: 0x001FF5FC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(4, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
