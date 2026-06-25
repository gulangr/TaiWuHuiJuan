using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.SelectCharacter;
using GameData.Domains.Character.Display;

namespace Game.Views
{
	// Token: 0x02000718 RID: 1816
	public class VillagerFilterLineLogic : DetailedFilterLineLogic<VillagerCharDisplayData>
	{
		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06005693 RID: 22163 RVA: 0x00281CC5 File Offset: 0x0027FEC5
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06005694 RID: 22164 RVA: 0x00281CC8 File Offset: 0x0027FEC8
		protected override IEnumerable<DetailedFilterMenuLogic<VillagerCharDisplayData>> GenerateMenus()
		{
			yield return new VillagerCharGenderMenu();
			yield return new VillagerCharBehaviorTypeMenu();
			yield return new RelationFilterMenu<VillagerCharDisplayData>();
			yield break;
		}

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x06005695 RID: 22165 RVA: 0x00281CD8 File Offset: 0x0027FED8
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06005696 RID: 22166 RVA: 0x00281CDB File Offset: 0x0027FEDB
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005697 RID: 22167 RVA: 0x00281CE0 File Offset: 0x0027FEE0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
