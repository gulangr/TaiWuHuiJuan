using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CDE RID: 3294
	public class SecretSortAndFilterData
	{
		// Token: 0x17001162 RID: 4450
		// (get) Token: 0x0600A645 RID: 42565 RVA: 0x004D67AD File Offset: 0x004D49AD
		public SecretInformationItem GetConfig
		{
			get
			{
				return SecretInformation.Instance[this.Data.SecretInformationTemplateId];
			}
		}

		// Token: 0x17001163 RID: 4451
		// (get) Token: 0x0600A646 RID: 42566 RVA: 0x004D67C4 File Offset: 0x004D49C4
		public int LifeTime
		{
			get
			{
				return (this.GetConfig.Duration >= 0) ? ((int)this.GetConfig.Duration - (this.Date - this.Data.OccurenceDate)) : 1;
			}
		}

		// Token: 0x17001164 RID: 4452
		// (get) Token: 0x0600A647 RID: 42567 RVA: 0x004D67F5 File Offset: 0x004D49F5
		public int CanUseCount
		{
			get
			{
				return Math.Max(((this.Data.AuthorityCostWhenDisseminating == 0) ? GlobalConfig.Instance.SecretInformationInBroadcastMaxUseCount : GlobalConfig.Instance.SecretInformationInPrivateMaxUseCount) - this.Data.UsedCount, 0);
			}
		}

		// Token: 0x17001165 RID: 4453
		// (get) Token: 0x0600A648 RID: 42568 RVA: 0x004D682C File Offset: 0x004D4A2C
		public int DisseminationLevel
		{
			get
			{
				return (this.Data.DisseminationRate < (int)GlobalConfig.Instance.SecretInformationReceivedDisseminateLevels[1]) ? 0 : ((this.Data.DisseminationRate < (int)GlobalConfig.Instance.SecretInformationReceivedDisseminateLevels[2]) ? 1 : ((this.Data.DisseminationRate < (int)GlobalConfig.Instance.SecretInformationReceivedDisseminateLevels[3]) ? 2 : 3));
			}
		}

		// Token: 0x04008309 RID: 33545
		public SecretInformationDisplayData Data;

		// Token: 0x0400830A RID: 33546
		public Dictionary<int, CharacterDisplayData> Characters;

		// Token: 0x0400830B RID: 33547
		public int LevelScore;

		// Token: 0x0400830C RID: 33548
		public short Relation;

		// Token: 0x0400830D RID: 33549
		public int Date;
	}
}
