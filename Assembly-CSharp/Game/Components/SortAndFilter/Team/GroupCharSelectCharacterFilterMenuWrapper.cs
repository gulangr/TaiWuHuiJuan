using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Team
{
	// Token: 0x02000CD6 RID: 3286
	internal sealed class GroupCharSelectCharacterFilterMenuWrapper : DetailedFilterMenuLogic<GroupCharDisplayData>
	{
		// Token: 0x0600A60E RID: 42510 RVA: 0x004D4ABD File Offset: 0x004D2CBD
		public GroupCharSelectCharacterFilterMenuWrapper(DetailedFilterMenuLogic<ISelectCharacterData> inner)
		{
			this._inner = inner;
		}

		// Token: 0x17001154 RID: 4436
		// (get) Token: 0x0600A60F RID: 42511 RVA: 0x004D4ACE File Offset: 0x004D2CCE
		public override int Id
		{
			get
			{
				return this._inner.Id;
			}
		}

		// Token: 0x17001155 RID: 4437
		// (get) Token: 0x0600A610 RID: 42512 RVA: 0x004D4ADB File Offset: 0x004D2CDB
		public override EFilterLogic LogicType
		{
			get
			{
				return this._inner.LogicType;
			}
		}

		// Token: 0x17001156 RID: 4438
		// (get) Token: 0x0600A611 RID: 42513 RVA: 0x004D4AE8 File Offset: 0x004D2CE8
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return this._inner.Dependency;
			}
		}

		// Token: 0x0600A612 RID: 42514 RVA: 0x004D4AF5 File Offset: 0x004D2CF5
		public override void OnInit()
		{
			this._inner.OnInit();
		}

		// Token: 0x0600A613 RID: 42515 RVA: 0x004D4B03 File Offset: 0x004D2D03
		public override StringKey GetMenuBarLabel()
		{
			return this._inner.GetMenuBarLabel();
		}

		// Token: 0x0600A614 RID: 42516 RVA: 0x004D4B10 File Offset: 0x004D2D10
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return this._inner.GetMenuItemConfigs();
		}

		// Token: 0x0600A615 RID: 42517 RVA: 0x004D4B20 File Offset: 0x004D2D20
		public override bool IsDataMatch(GroupCharDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			return this._inner.IsDataMatch(GroupCharDisplaySelectAdapter.Wrap(data), selectedIndices);
		}

		// Token: 0x04008303 RID: 33539
		private readonly DetailedFilterMenuLogic<ISelectCharacterData> _inner;
	}
}
