using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D29 RID: 3369
	public class CombatSkillBookDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011A7 RID: 4519
		// (get) Token: 0x0600A786 RID: 42886 RVA: 0x004DF356 File Offset: 0x004DD556
		public override int Id
		{
			get
			{
				return 26;
			}
		}

		// Token: 0x170011A8 RID: 4520
		// (get) Token: 0x0600A787 RID: 42887 RVA: 0x004DF35A File Offset: 0x004DD55A
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A788 RID: 42888 RVA: 0x004DF35D File Offset: 0x004DD55D
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new CombatSkillReadStatusMenu();
			yield return new CombatSkillTypeMenu();
			yield return new CombatSkillSectMenu();
			yield break;
		}

		// Token: 0x0600A789 RID: 42889 RVA: 0x004DF370 File Offset: 0x004DD570
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(4, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
