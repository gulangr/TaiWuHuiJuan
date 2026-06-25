using System;
using System.Collections.Generic;

namespace Game.Views.Bottom
{
	// Token: 0x02000C52 RID: 3154
	public struct FilterLevelKey
	{
		// Token: 0x0600A09A RID: 41114 RVA: 0x004AFA20 File Offset: 0x004ADC20
		public FilterLevelKey(EPrimaryFilterType primary, ESecondaryFilterType? secondary = null)
		{
			this.Primary = primary;
			this.Secondary = ((primary == EPrimaryFilterType.Character) ? secondary : null);
		}

		// Token: 0x04007C82 RID: 31874
		public EPrimaryFilterType Primary;

		// Token: 0x04007C83 RID: 31875
		public ESecondaryFilterType? Secondary;

		// Token: 0x04007C84 RID: 31876
		public static readonly FilterLevelKey CharacterIdentity = new FilterLevelKey(EPrimaryFilterType.Character, new ESecondaryFilterType?(ESecondaryFilterType.Identity));

		// Token: 0x04007C85 RID: 31877
		public static readonly FilterLevelKey CharacterStatus = new FilterLevelKey(EPrimaryFilterType.Character, new ESecondaryFilterType?(ESecondaryFilterType.Status));

		// Token: 0x04007C86 RID: 31878
		public static readonly FilterLevelKey CharacterAttribute = new FilterLevelKey(EPrimaryFilterType.Character, new ESecondaryFilterType?(ESecondaryFilterType.Attribute));

		// Token: 0x04007C87 RID: 31879
		public static readonly FilterLevelKey Merchant = new FilterLevelKey(EPrimaryFilterType.Merchant, null);

		// Token: 0x04007C88 RID: 31880
		public static readonly FilterLevelKey Grave = new FilterLevelKey(EPrimaryFilterType.Grave, null);

		// Token: 0x04007C89 RID: 31881
		public static readonly FilterLevelKey Beast = new FilterLevelKey(EPrimaryFilterType.Beast, null);

		// Token: 0x04007C8A RID: 31882
		public static readonly FilterLevelKey Terrain = new FilterLevelKey(EPrimaryFilterType.Terrain, null);

		// Token: 0x04007C8B RID: 31883
		public static readonly Dictionary<FilterLevelKey, int> FilterLevelKeyDict = new Dictionary<FilterLevelKey, int>
		{
			{
				FilterLevelKey.CharacterIdentity,
				0
			},
			{
				FilterLevelKey.CharacterStatus,
				1
			},
			{
				FilterLevelKey.CharacterAttribute,
				2
			},
			{
				FilterLevelKey.Merchant,
				3
			},
			{
				FilterLevelKey.Grave,
				4
			},
			{
				FilterLevelKey.Beast,
				5
			},
			{
				FilterLevelKey.Terrain,
				6
			}
		};
	}
}
