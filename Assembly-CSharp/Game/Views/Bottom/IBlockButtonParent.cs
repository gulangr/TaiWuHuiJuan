using System;
using GameData.Domains.Map;

namespace Game.Views.Bottom
{
	// Token: 0x02000C4B RID: 3147
	public interface IBlockButtonParent : IAsyncMethodRequestHandler
	{
		// Token: 0x0600A00B RID: 40971
		void Refresh();

		// Token: 0x0600A00C RID: 40972
		void Hide(BlockButton child);

		// Token: 0x0600A00D RID: 40973
		void Show(BlockButton child);

		// Token: 0x0600A00E RID: 40974
		void OnChildEnter(BlockButton child);

		// Token: 0x0600A00F RID: 40975
		void OnChildExit(BlockButton child);

		// Token: 0x0600A010 RID: 40976
		void MoveToBlock(BlockButton child);

		// Token: 0x0600A011 RID: 40977
		void MuteOnDisable();

		// Token: 0x170010E0 RID: 4320
		// (get) Token: 0x0600A012 RID: 40978
		int CharId { get; }

		// Token: 0x170010E1 RID: 4321
		// (get) Token: 0x0600A013 RID: 40979
		// (set) Token: 0x0600A014 RID: 40980
		bool DisableMove { get; set; }

		// Token: 0x170010E2 RID: 4322
		// (get) Token: 0x0600A015 RID: 40981
		int MoveCost { get; }

		// Token: 0x170010E3 RID: 4323
		// (get) Token: 0x0600A016 RID: 40982
		MapBlockData BlockData { get; }

		// Token: 0x170010E4 RID: 4324
		// (get) Token: 0x0600A017 RID: 40983
		// (set) Token: 0x0600A018 RID: 40984
		bool ExtraViewOpened { get; set; }
	}
}
