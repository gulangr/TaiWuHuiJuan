using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E55 RID: 3669
	public class CharacterTaskStatusLine : DetailedFilterLineLogic<int>
	{
		// Token: 0x0600AC30 RID: 44080 RVA: 0x004EDD0A File Offset: 0x004EBF0A
		public CharacterTaskStatusLine(CharacterDetailDisplayDataSortAndFilterControllerController controller)
		{
			this._controller = controller;
		}

		// Token: 0x1700133D RID: 4925
		// (get) Token: 0x0600AC31 RID: 44081 RVA: 0x004EDD1A File Offset: 0x004EBF1A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700133E RID: 4926
		// (get) Token: 0x0600AC32 RID: 44082 RVA: 0x004EDD1D File Offset: 0x004EBF1D
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700133F RID: 4927
		// (get) Token: 0x0600AC33 RID: 44083 RVA: 0x004EDD20 File Offset: 0x004EBF20
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AC34 RID: 44084 RVA: 0x004EDD23 File Offset: 0x004EBF23
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AC35 RID: 44085 RVA: 0x004EDD26 File Offset: 0x004EBF26
		protected override IEnumerable<DetailedFilterMenuLogic<int>> GenerateMenus()
		{
			yield return new CharacterTaskStatusMenu(this._controller);
			yield break;
		}

		// Token: 0x04008544 RID: 34116
		private readonly CharacterDetailDisplayDataSortAndFilterControllerController _controller;
	}
}
