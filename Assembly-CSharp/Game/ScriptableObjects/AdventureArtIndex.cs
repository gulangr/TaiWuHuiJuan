using System;
using System.Collections.Generic;

namespace Game.ScriptableObjects
{
	// Token: 0x02000C85 RID: 3205
	[Serializable]
	public class AdventureArtIndex
	{
		// Token: 0x04007F4D RID: 32589
		public string name;

		// Token: 0x04007F4E RID: 32590
		public List<string> textureNames;

		// Token: 0x04007F4F RID: 32591
		public List<AdventureArtIndex> subIndexes;
	}
}
