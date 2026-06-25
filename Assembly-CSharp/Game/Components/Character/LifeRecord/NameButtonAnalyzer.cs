using System;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F53 RID: 3923
	public class NameButtonAnalyzer : NameButtonAnalyzerBase
	{
		// Token: 0x17001461 RID: 5217
		// (get) Token: 0x0600B3CD RID: 46029 RVA: 0x0051D49C File Offset: 0x0051B69C
		public override string FullBtnKey
		{
			get
			{
				return LifeRecord.LifeRecordNameFullBtnKey;
			}
		}

		// Token: 0x17001462 RID: 5218
		// (get) Token: 0x0600B3CE RID: 46030 RVA: 0x0051D4A3 File Offset: 0x0051B6A3
		public override string LeftBtnKey
		{
			get
			{
				return LifeRecord.LifeRecordNameLeftBtnKey;
			}
		}

		// Token: 0x17001463 RID: 5219
		// (get) Token: 0x0600B3CF RID: 46031 RVA: 0x0051D4AA File Offset: 0x0051B6AA
		public override string RightBtnKey
		{
			get
			{
				return LifeRecord.LifeRecordNameRightBtnKey;
			}
		}
	}
}
