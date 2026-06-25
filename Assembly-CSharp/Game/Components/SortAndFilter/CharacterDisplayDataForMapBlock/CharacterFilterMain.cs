using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterDisplayDataForMapBlock
{
	// Token: 0x02000E6F RID: 3695
	public class CharacterFilterMain : DetailedFilterLineLogic<CharacterDisplayData>
	{
		// Token: 0x0600AC84 RID: 44164 RVA: 0x004EE9E2 File Offset: 0x004ECBE2
		public CharacterFilterMain(CharacterDisplayDataSortAndFilterController controller)
		{
			this._controller = controller;
		}

		// Token: 0x1700135C RID: 4956
		// (get) Token: 0x0600AC85 RID: 44165 RVA: 0x004EE9F2 File Offset: 0x004ECBF2
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700135D RID: 4957
		// (get) Token: 0x0600AC86 RID: 44166 RVA: 0x004EE9F5 File Offset: 0x004ECBF5
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700135E RID: 4958
		// (get) Token: 0x0600AC87 RID: 44167 RVA: 0x004EE9F8 File Offset: 0x004ECBF8
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AC88 RID: 44168 RVA: 0x004EE9FB File Offset: 0x004ECBFB
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AC89 RID: 44169 RVA: 0x004EE9FE File Offset: 0x004ECBFE
		protected override IEnumerable<DetailedFilterMenuLogic<CharacterDisplayData>> GenerateMenus()
		{
			yield return new CharacterOrganizationMenu(this._controller);
			yield return new CharacterSectMenu(this._controller);
			yield return new CharacterCityMenu(this._controller);
			yield return new CharacterStatusMenu(this._controller);
			yield break;
		}

		// Token: 0x040085B2 RID: 34226
		private readonly CharacterDisplayDataSortAndFilterController _controller;
	}
}
