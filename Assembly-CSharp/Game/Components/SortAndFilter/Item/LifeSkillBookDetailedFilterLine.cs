using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D2A RID: 3370
	public class LifeSkillBookDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011A9 RID: 4521
		// (get) Token: 0x0600A78B RID: 42891 RVA: 0x004DF3A3 File Offset: 0x004DD5A3
		public override int Id
		{
			get
			{
				return 27;
			}
		}

		// Token: 0x170011AA RID: 4522
		// (get) Token: 0x0600A78C RID: 42892 RVA: 0x004DF3A7 File Offset: 0x004DD5A7
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A78D RID: 42893 RVA: 0x004DF3AA File Offset: 0x004DD5AA
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new LifeSkillReadStatusMenu();
			yield return new LifeSkillTypeMenu();
			yield break;
		}

		// Token: 0x0600A78E RID: 42894 RVA: 0x004DF3BC File Offset: 0x004DD5BC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(4, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
