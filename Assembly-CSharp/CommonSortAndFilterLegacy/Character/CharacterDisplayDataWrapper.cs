using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x02000596 RID: 1430
	public class CharacterDisplayDataWrapper : ICharacterSortAndFilterData
	{
		// Token: 0x06004526 RID: 17702 RVA: 0x0020BADD File Offset: 0x00209CDD
		public CharacterDisplayDataWrapper(CharacterDisplayData inner)
		{
			this.Inner = inner;
		}

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x06004527 RID: 17703 RVA: 0x0020BAEF File Offset: 0x00209CEF
		public string Name
		{
			get
			{
				return NameCenter.GetMonasticTitleOrDisplayName(this.Inner, false);
			}
		}

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x06004528 RID: 17704 RVA: 0x0020BAFD File Offset: 0x00209CFD
		public sbyte Gender
		{
			get
			{
				return this.Inner.Gender;
			}
		}

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x06004529 RID: 17705 RVA: 0x0020BB0A File Offset: 0x00209D0A
		public short Age
		{
			get
			{
				return this.Inner.AvatarRelatedData.DisplayAge;
			}
		}

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x0600452A RID: 17706 RVA: 0x0020BB1C File Offset: 0x00209D1C
		public sbyte Grade
		{
			get
			{
				return this.Inner.OrgInfo.Grade;
			}
		}

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x0600452B RID: 17707 RVA: 0x0020BB2E File Offset: 0x00209D2E
		public sbyte BehaviorType
		{
			get
			{
				return this.Inner.BehaviorType;
			}
		}

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x0600452C RID: 17708 RVA: 0x0020BB3B File Offset: 0x00209D3B
		public ushort RelationToTaiwu
		{
			get
			{
				return this.Inner.RelationToTaiwu;
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x0600452D RID: 17709 RVA: 0x0020BB48 File Offset: 0x00209D48
		public ushort RelationFromTaiwu
		{
			get
			{
				return this.Inner.RelationFromTaiwu;
			}
		}

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x0600452E RID: 17710 RVA: 0x0020BB55 File Offset: 0x00209D55
		public short Charm
		{
			get
			{
				return this.Inner.Charm;
			}
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x0600452F RID: 17711 RVA: 0x0020BB62 File Offset: 0x00209D62
		public short Health
		{
			get
			{
				return this.Inner.Health;
			}
		}

		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06004530 RID: 17712 RVA: 0x0020BB6F File Offset: 0x00209D6F
		public short FavorabilityToTaiwu
		{
			get
			{
				return this.Inner.FavorabilityToTaiwu;
			}
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x06004531 RID: 17713 RVA: 0x0020BB7C File Offset: 0x00209D7C
		public short Happiness
		{
			get
			{
				return (short)this.Inner.Happiness;
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x06004532 RID: 17714 RVA: 0x0020BB89 File Offset: 0x00209D89
		public OrganizationInfo Organization
		{
			get
			{
				return this.Inner.OrgInfo;
			}
		}

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x06004533 RID: 17715 RVA: 0x0020BB96 File Offset: 0x00209D96
		public bool IsSameFactionWithTaiwu
		{
			get
			{
				return this.Inner.IsSameFactionWithTaiwu;
			}
		}

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06004534 RID: 17716 RVA: 0x0020BBA3 File Offset: 0x00209DA3
		public Location Location
		{
			get
			{
				return this.Inner.Location;
			}
		}

		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06004535 RID: 17717 RVA: 0x0020BBB0 File Offset: 0x00209DB0
		// (set) Token: 0x06004536 RID: 17718 RVA: 0x0020BBB8 File Offset: 0x00209DB8
		public CharacterDisplayData Inner { get; private set; }
	}
}
