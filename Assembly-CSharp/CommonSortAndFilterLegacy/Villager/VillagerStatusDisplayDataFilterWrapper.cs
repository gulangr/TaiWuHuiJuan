using System;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Display.VillagerRoleArrangement;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x0200046E RID: 1134
	public readonly struct VillagerStatusDisplayDataFilterWrapper : IVillagerSortAndFilterData
	{
		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x060040B2 RID: 16562 RVA: 0x001FFFC8 File Offset: 0x001FE1C8
		public string Name
		{
			get
			{
				NameRelatedData name = this._inner.Name;
				return NameCenter.GetMonasticTitleOrDisplayName(ref name, false, false);
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x060040B3 RID: 16563 RVA: 0x001FFFEF File Offset: 0x001FE1EF
		public sbyte Gender
		{
			get
			{
				return this._inner.Gender;
			}
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x060040B4 RID: 16564 RVA: 0x001FFFFC File Offset: 0x001FE1FC
		public short Age
		{
			get
			{
				return this._inner.PhysiologicalAge;
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x060040B5 RID: 16565 RVA: 0x00200009 File Offset: 0x001FE209
		public sbyte Grade
		{
			get
			{
				return this.CalcGrade();
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x060040B6 RID: 16566 RVA: 0x00200011 File Offset: 0x001FE211
		public sbyte BehaviorType
		{
			get
			{
				return this._inner.BehaviorType;
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x060040B7 RID: 16567 RVA: 0x0020001E File Offset: 0x001FE21E
		public ushort RelationToTaiwu
		{
			get
			{
				return this._inner.RelationToTaiwu;
			}
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x060040B8 RID: 16568 RVA: 0x0020002B File Offset: 0x001FE22B
		public ushort RelationFromTaiwu
		{
			get
			{
				return this._inner.RelationFromTaiwu;
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x060040B9 RID: 16569 RVA: 0x00200038 File Offset: 0x001FE238
		public sbyte WorkType
		{
			get
			{
				return this._inner.WorkType;
			}
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x060040BA RID: 16570 RVA: 0x00200045 File Offset: 0x001FE245
		public short Charm
		{
			get
			{
				return this._inner.Charm;
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x060040BB RID: 16571 RVA: 0x00200052 File Offset: 0x001FE252
		public short Health
		{
			get
			{
				return this._inner.Health;
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060040BC RID: 16572 RVA: 0x0020005F File Offset: 0x001FE25F
		public short FavorabilityToTaiwu
		{
			get
			{
				return this._inner.FavorabilityToTaiwu;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060040BD RID: 16573 RVA: 0x0020006C File Offset: 0x001FE26C
		public short Happiness
		{
			get
			{
				return this._inner.Happiness;
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x060040BE RID: 16574 RVA: 0x00200079 File Offset: 0x001FE279
		public sbyte LeftPotentialCount
		{
			get
			{
				return this._inner.LeftPotentialCount;
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x060040BF RID: 16575 RVA: 0x00200086 File Offset: 0x001FE286
		public VillagerRoleArrangementDisplayDataWrapper ArrangementData
		{
			get
			{
				return this._inner.ArrangementDisplayData;
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x060040C0 RID: 16576 RVA: 0x00200093 File Offset: 0x001FE293
		public bool IsSameFactionWithTaiwu
		{
			get
			{
				return this._inner.IsSameFactionWithTaiwu;
			}
		}

		// Token: 0x060040C1 RID: 16577 RVA: 0x002000A0 File Offset: 0x001FE2A0
		public bool IsArrangementMatched(EArrangementMenuOption arrangementMenuOption)
		{
			VillagerRoleArrangementDisplayDataWrapper arrangementData = this.ArrangementData;
			int roleArrangement = (arrangementData != null) ? arrangementData.ArrangementTemplateId : -1;
			if (!true)
			{
			}
			bool result;
			switch (arrangementMenuOption)
			{
			case EArrangementMenuOption.Collect:
				result = (this.WorkType == 10);
				break;
			case EArrangementMenuOption.Migrate:
				result = (this.WorkType == 14);
				break;
			case EArrangementMenuOption.Healing:
				result = (roleArrangement == 6);
				break;
			case EArrangementMenuOption.Sell:
				result = (roleArrangement == 8 && VillagerStatusDisplayDataFilterWrapper.IsPeddlingBuy(this.ArrangementData.ArrangementData));
				break;
			case EArrangementMenuOption.Buy:
				result = (roleArrangement == 8 && !VillagerStatusDisplayDataFilterWrapper.IsPeddlingBuy(this.ArrangementData.ArrangementData));
				break;
			case EArrangementMenuOption.Entertain:
				result = (roleArrangement == 11);
				break;
			case EArrangementMenuOption.GuardSwordTomb:
				result = (roleArrangement == 13);
				break;
			case EArrangementMenuOption.Envoy:
				result = (roleArrangement == 15);
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060040C2 RID: 16578 RVA: 0x0020016C File Offset: 0x001FE36C
		public bool IsWorkStatusMatched(EWorkStatusMenuOption workStatusMenuOption)
		{
			sbyte workStatus = this.WorkType;
			VillagerRoleArrangementDisplayDataWrapper arrangementData = this.ArrangementData;
			int roleArrangement = (arrangementData != null) ? arrangementData.ArrangementTemplateId : -1;
			if (!true)
			{
			}
			bool result;
			switch (workStatusMenuOption)
			{
			case EWorkStatusMenuOption.NoWork:
				result = (workStatus == -1);
				break;
			case EWorkStatusMenuOption.ShopManage:
				result = (workStatus == 1);
				break;
			case EWorkStatusMenuOption.RoleWork:
				result = (roleArrangement >= 0 || workStatus == 10 || workStatus == 14);
				break;
			case EWorkStatusMenuOption.KeepGrave:
				result = (workStatus == 12);
				break;
			case EWorkStatusMenuOption.Idle:
				result = (workStatus == 13);
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x060040C3 RID: 16579 RVA: 0x002001F6 File Offset: 0x001FE3F6
		public VillagerStatusDisplayData Inner
		{
			get
			{
				return this._inner;
			}
		}

		// Token: 0x060040C4 RID: 16580 RVA: 0x002001FE File Offset: 0x001FE3FE
		public VillagerStatusDisplayDataFilterWrapper(VillagerStatusDisplayData data)
		{
			this._inner = data;
		}

		// Token: 0x060040C5 RID: 16581 RVA: 0x00200208 File Offset: 0x001FE408
		private sbyte CalcGrade()
		{
			short roleTemplateId = this._inner.RoleTemplateId;
			bool flag = roleTemplateId < 0;
			sbyte result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				short orgMember = VillagerRole.Instance[roleTemplateId].OrganizationMember;
				OrganizationMemberItem organizationMemberItem = OrganizationMember.Instance[orgMember];
				result = organizationMemberItem.Grade;
			}
			return result;
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x00200258 File Offset: 0x001FE458
		private static bool IsPeddlingBuy(IVillagerRoleArrangementDisplayData roleArrangementDisplayData)
		{
			PeddlingDisplayData peddlingDisplayData = roleArrangementDisplayData as PeddlingDisplayData;
			bool flag = peddlingDisplayData == null;
			return !flag && !peddlingDisplayData.IsBuy;
		}

		// Token: 0x04002E2B RID: 11819
		private readonly VillagerStatusDisplayData _inner;
	}
}
