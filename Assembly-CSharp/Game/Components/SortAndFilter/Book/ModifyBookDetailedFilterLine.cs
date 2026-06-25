using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E83 RID: 3715
	public class ModifyBookDetailedFilterLine : CombatSkillBookDetailedFilterLine
	{
		// Token: 0x0600ACDB RID: 44251 RVA: 0x004EF9AC File Offset: 0x004EDBAC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600ACDC RID: 44252 RVA: 0x004EF9BF File Offset: 0x004EDBBF
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new CombatSkillReadStatusMenu();
			yield return new CombatSkillTypeMenu();
			yield return new CombatSkillSectMenu();
			yield break;
		}
	}
}
