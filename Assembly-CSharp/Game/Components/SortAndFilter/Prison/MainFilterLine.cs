using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Prison
{
	// Token: 0x02000D11 RID: 3345
	public class MainFilterLine : DetailedFilterLineLogic<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x1700118C RID: 4492
		// (get) Token: 0x0600A717 RID: 42775 RVA: 0x004DBD33 File Offset: 0x004D9F33
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A718 RID: 42776 RVA: 0x004DBD36 File Offset: 0x004D9F36
		protected override IEnumerable<DetailedFilterMenuLogic<CharacterDisplayDataForSettlementPrisoner>> GenerateMenus()
		{
			yield return new OrganizationMenu();
			yield return new PunishmentTypeMenu();
			yield break;
		}

		// Token: 0x1700118D RID: 4493
		// (get) Token: 0x0600A719 RID: 42777 RVA: 0x004DBD46 File Offset: 0x004D9F46
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700118E RID: 4494
		// (get) Token: 0x0600A71A RID: 42778 RVA: 0x004DBD49 File Offset: 0x004D9F49
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A71B RID: 42779 RVA: 0x004DBD4C File Offset: 0x004D9F4C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
