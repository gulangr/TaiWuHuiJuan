using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E63 RID: 3683
	public class CharacterLocationDisplayDataMainTypeFilterLine : DetailedFilterLineLogic<CharacterLocationDisplayData>
	{
		// Token: 0x17001342 RID: 4930
		// (get) Token: 0x0600AC41 RID: 44097 RVA: 0x004EDE83 File Offset: 0x004EC083
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001343 RID: 4931
		// (get) Token: 0x0600AC42 RID: 44098 RVA: 0x004EDE86 File Offset: 0x004EC086
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001344 RID: 4932
		// (get) Token: 0x0600AC43 RID: 44099 RVA: 0x004EDE89 File Offset: 0x004EC089
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AC44 RID: 44100 RVA: 0x004EDE8C File Offset: 0x004EC08C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AC45 RID: 44101 RVA: 0x004EDE8F File Offset: 0x004EC08F
		protected override IEnumerable<DetailedFilterMenuLogic<CharacterLocationDisplayData>> GenerateMenus()
		{
			yield return new CharacterLocationDisplayDataMainTypeFilterLineDetail();
			yield return new CharacterLocationDisplayDataCityMenu();
			yield return new CharacterLocationDisplayDataSectMenu();
			yield return new CharacterLocationDisplayDataTownMenu();
			yield return new CharacterLocationDisplayDataDevelopedMenu();
			yield return new CharacterLocationDisplayDataNormalMenu();
			yield return new CharacterLocationDisplayDataWildMenu();
			yield return new CharacterLocationDisplayDataBadMenu();
			yield break;
		}
	}
}
