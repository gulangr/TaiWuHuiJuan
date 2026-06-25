using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000498 RID: 1176
	public class CombatSkillBookDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x0600415C RID: 16732 RVA: 0x00201521 File Offset: 0x001FF721
		public override int Id
		{
			get
			{
				return 23;
			}
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x0600415D RID: 16733 RVA: 0x00201525 File Offset: 0x001FF725
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600415E RID: 16734 RVA: 0x00201528 File Offset: 0x001FF728
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new CombatSkillReadStatusMenu();
			yield return new CombatSkillTypeMenu();
			yield return new CombatSkillSectMenu();
			yield break;
		}

		// Token: 0x0600415F RID: 16735 RVA: 0x00201538 File Offset: 0x001FF738
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(4, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
