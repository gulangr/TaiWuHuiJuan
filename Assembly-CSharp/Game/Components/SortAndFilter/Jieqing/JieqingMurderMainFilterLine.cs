using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Jieqing
{
	// Token: 0x02000D25 RID: 3365
	public class JieqingMurderMainFilterLine : DetailedFilterLineLogic<CharacterDisplayData>
	{
		// Token: 0x1700119D RID: 4509
		// (get) Token: 0x0600A76B RID: 42859 RVA: 0x004DEFBE File Offset: 0x004DD1BE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A76C RID: 42860 RVA: 0x004DEFC1 File Offset: 0x004DD1C1
		protected override IEnumerable<DetailedFilterMenuLogic<CharacterDisplayData>> GenerateMenus()
		{
			yield return new JieqingMurderOrganizationMenu();
			yield return new JieqingMurderMapStateMenu();
			yield break;
		}

		// Token: 0x1700119E RID: 4510
		// (get) Token: 0x0600A76D RID: 42861 RVA: 0x004DEFD1 File Offset: 0x004DD1D1
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700119F RID: 4511
		// (get) Token: 0x0600A76E RID: 42862 RVA: 0x004DEFD4 File Offset: 0x004DD1D4
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A76F RID: 42863 RVA: 0x004DEFD8 File Offset: 0x004DD1D8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
