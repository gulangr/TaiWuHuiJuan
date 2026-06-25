using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.SelectCharacter;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Team
{
	// Token: 0x02000CD7 RID: 3287
	public class TeamCharacterMainFilterLine : DetailedFilterLineLogic<GroupCharDisplayData>
	{
		// Token: 0x0600A616 RID: 42518 RVA: 0x004D4B44 File Offset: 0x004D2D44
		public TeamCharacterMainFilterLine(List<ESelectCharacterFilterMenuId> menuIds)
		{
			this._menuIds = (menuIds ?? new List<ESelectCharacterFilterMenuId>());
		}

		// Token: 0x17001157 RID: 4439
		// (get) Token: 0x0600A617 RID: 42519 RVA: 0x004D4B5E File Offset: 0x004D2D5E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001158 RID: 4440
		// (get) Token: 0x0600A618 RID: 42520 RVA: 0x004D4B61 File Offset: 0x004D2D61
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001159 RID: 4441
		// (get) Token: 0x0600A619 RID: 42521 RVA: 0x004D4B64 File Offset: 0x004D2D64
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A61A RID: 42522 RVA: 0x004D4B67 File Offset: 0x004D2D67
		protected override IEnumerable<DetailedFilterMenuLogic<GroupCharDisplayData>> GenerateMenus()
		{
			foreach (ESelectCharacterFilterMenuId menuId in this._menuIds)
			{
				DetailedFilterMenuLogic<ISelectCharacterData> menu = SelectCharacterFilterMenuFactory<ISelectCharacterData>.CreateBasicMenu(menuId);
				bool flag = menu != null;
				if (flag)
				{
					yield return new GroupCharSelectCharacterFilterMenuWrapper(menu);
				}
				menu = null;
			}
			List<ESelectCharacterFilterMenuId>.Enumerator enumerator = default(List<ESelectCharacterFilterMenuId>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600A61B RID: 42523 RVA: 0x004D4B78 File Offset: 0x004D2D78
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x04008304 RID: 33540
		private readonly List<ESelectCharacterFilterMenuId> _menuIds;
	}
}
