using System;
using System.Collections.Generic;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CFE RID: 3326
	public class SelectCharacterMainFilterLine : DetailedFilterLineLogic<ISelectCharacterData>
	{
		// Token: 0x0600A6C8 RID: 42696 RVA: 0x004D88B2 File Offset: 0x004D6AB2
		public SelectCharacterMainFilterLine(List<ESelectCharacterFilterMenuId> menuIds)
		{
			this._menuIds = (menuIds ?? new List<ESelectCharacterFilterMenuId>());
		}

		// Token: 0x17001183 RID: 4483
		// (get) Token: 0x0600A6C9 RID: 42697 RVA: 0x004D88CC File Offset: 0x004D6ACC
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001184 RID: 4484
		// (get) Token: 0x0600A6CA RID: 42698 RVA: 0x004D88CF File Offset: 0x004D6ACF
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001185 RID: 4485
		// (get) Token: 0x0600A6CB RID: 42699 RVA: 0x004D88D2 File Offset: 0x004D6AD2
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A6CC RID: 42700 RVA: 0x004D88D5 File Offset: 0x004D6AD5
		protected override IEnumerable<DetailedFilterMenuLogic<ISelectCharacterData>> GenerateMenus()
		{
			foreach (ESelectCharacterFilterMenuId menuId in this._menuIds)
			{
				DetailedFilterMenuLogic<ISelectCharacterData> menu = SelectCharacterFilterMenuFactory<ISelectCharacterData>.CreateBasicMenu(menuId);
				bool flag = menu != null;
				if (flag)
				{
					yield return menu;
				}
				menu = null;
			}
			List<ESelectCharacterFilterMenuId>.Enumerator enumerator = default(List<ESelectCharacterFilterMenuId>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600A6CD RID: 42701 RVA: 0x004D88E8 File Offset: 0x004D6AE8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0400832E RID: 33582
		private readonly List<ESelectCharacterFilterMenuId> _menuIds;
	}
}
