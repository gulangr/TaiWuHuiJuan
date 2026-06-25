using System;
using System.Collections.Generic;
using Game.Views.Building.BuildingManage;

namespace Game.Components.SortAndFilter.Chicken
{
	// Token: 0x02000E4C RID: 3660
	public class ChickenFilterLine : DetailedFilterLineLogic<ChickenData>
	{
		// Token: 0x17001338 RID: 4920
		// (get) Token: 0x0600AC17 RID: 44055 RVA: 0x004ED79F File Offset: 0x004EB99F
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AC18 RID: 44056 RVA: 0x004ED7A2 File Offset: 0x004EB9A2
		protected override IEnumerable<DetailedFilterMenuLogic<ChickenData>> GenerateMenus()
		{
			yield return new PersonalityMenu();
			yield break;
		}

		// Token: 0x17001339 RID: 4921
		// (get) Token: 0x0600AC19 RID: 44057 RVA: 0x004ED7B2 File Offset: 0x004EB9B2
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700133A RID: 4922
		// (get) Token: 0x0600AC1A RID: 44058 RVA: 0x004ED7B5 File Offset: 0x004EB9B5
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AC1B RID: 44059 RVA: 0x004ED7B8 File Offset: 0x004EB9B8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
