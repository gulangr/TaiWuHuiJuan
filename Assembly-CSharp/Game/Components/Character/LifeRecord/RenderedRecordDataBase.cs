using System;
using GameData.Domains.LifeRecord;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F57 RID: 3927
	public abstract class RenderedRecordDataBase
	{
		// Token: 0x0600B3FA RID: 46074
		public abstract void SetData(TransferableRecord record, TransferableRecordDataBase data);

		// Token: 0x04008BF4 RID: 35828
		public short TemplateId;

		// Token: 0x04008BF5 RID: 35829
		public string Main;

		// Token: 0x04008BF6 RID: 35830
		public string Sub;
	}
}
