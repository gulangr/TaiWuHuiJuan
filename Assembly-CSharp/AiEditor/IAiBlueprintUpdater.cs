using System;

namespace AiEditor
{
	// Token: 0x02000665 RID: 1637
	public interface IAiBlueprintUpdater
	{
		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x06004DC1 RID: 19905
		string FromVersion { get; }

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x06004DC2 RID: 19906
		string ToVersion { get; }

		// Token: 0x06004DC3 RID: 19907
		void Update(AiBlueprintSnapshot blueprint);
	}
}
