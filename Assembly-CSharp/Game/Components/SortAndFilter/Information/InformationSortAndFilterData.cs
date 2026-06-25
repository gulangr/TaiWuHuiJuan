using System;
using Config;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E0F RID: 3599
	public class InformationSortAndFilterData
	{
		// Token: 0x170012EC RID: 4844
		// (get) Token: 0x0600AB1E RID: 43806 RVA: 0x004EA470 File Offset: 0x004E8670
		public sbyte OrganizationTemplateId
		{
			get
			{
				return InformationInfo.Instance[Information.Instance[this.TemplateId].InfoIds[(int)this.Level]].Oraganization;
			}
		}

		// Token: 0x170012ED RID: 4845
		// (get) Token: 0x0600AB1F RID: 43807 RVA: 0x004EA49D File Offset: 0x004E869D
		public sbyte LifeSkillType
		{
			get
			{
				return InformationInfo.Instance[Information.Instance[this.TemplateId].InfoIds[(int)this.Level]].LifeSkillType;
			}
		}

		// Token: 0x170012EE RID: 4846
		// (get) Token: 0x0600AB20 RID: 43808 RVA: 0x004EA4CA File Offset: 0x004E86CA
		public short WesternRegionTemplateId
		{
			get
			{
				return InformationInfo.Instance[Information.Instance[this.TemplateId].InfoIds[(int)this.Level]].WesternRegionId;
			}
		}

		// Token: 0x170012EF RID: 4847
		// (get) Token: 0x0600AB21 RID: 43809 RVA: 0x004EA4F7 File Offset: 0x004E86F7
		public EInformationInfoSwordInformationType SwordTombType
		{
			get
			{
				return InformationInfo.Instance[Information.Instance[this.TemplateId].InfoIds[(int)this.Level]].SwordInformationType;
			}
		}

		// Token: 0x170012F0 RID: 4848
		// (get) Token: 0x0600AB22 RID: 43810 RVA: 0x004EA524 File Offset: 0x004E8724
		public int Profession
		{
			get
			{
				return (int)InformationInfo.Instance[Information.Instance[this.TemplateId].InfoIds[(int)this.Level]].Profession;
			}
		}

		// Token: 0x170012F1 RID: 4849
		// (get) Token: 0x0600AB23 RID: 43811 RVA: 0x004EA551 File Offset: 0x004E8751
		public int RemainCount
		{
			get
			{
				return (int)(this.UsedCountMax - this.UsedCount);
			}
		}

		// Token: 0x040084E1 RID: 34017
		public short TemplateId;

		// Token: 0x040084E2 RID: 34018
		public sbyte Level;

		// Token: 0x040084E3 RID: 34019
		public sbyte UsedCount;

		// Token: 0x040084E4 RID: 34020
		public sbyte UsedCountMax;
	}
}
