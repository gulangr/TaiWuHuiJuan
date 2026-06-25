using System;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A4D RID: 2637
	[Serializable]
	public class FavoriteInfo : IEquatable<FavoriteInfo>
	{
		// Token: 0x06008267 RID: 33383 RVA: 0x003CC9B4 File Offset: 0x003CABB4
		public override string ToString()
		{
			return string.Format("{0}-{1}", this.PersistentFavoriteInfo.TitleLevel, this.PersistentFavoriteInfo.DataId);
		}

		// Token: 0x06008268 RID: 33384 RVA: 0x003CC9F0 File Offset: 0x003CABF0
		public bool Equals(FavoriteInfo other)
		{
			return this.PersistentFavoriteInfo.TitleLevel == other.PersistentFavoriteInfo.TitleLevel && this.PersistentFavoriteInfo.DataId == other.PersistentFavoriteInfo.DataId;
		}

		// Token: 0x040063AE RID: 25518
		public PersistentFavoriteInfo PersistentFavoriteInfo;

		// Token: 0x040063AF RID: 25519
		public string Title;
	}
}
