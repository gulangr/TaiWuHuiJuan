using System;
using System.Collections.Generic;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A52 RID: 2642
	[Serializable]
	public class EncyclopediaData
	{
		// Token: 0x040063D6 RID: 25558
		public Dictionary<int, NodeData> NodeIdDict = new Dictionary<int, NodeData>();

		// Token: 0x040063D7 RID: 25559
		public Dictionary<string, NodeData> NodeKeyDict = new Dictionary<string, NodeData>();

		// Token: 0x040063D8 RID: 25560
		public List<NodeData> NodeList = new List<NodeData>();
	}
}
