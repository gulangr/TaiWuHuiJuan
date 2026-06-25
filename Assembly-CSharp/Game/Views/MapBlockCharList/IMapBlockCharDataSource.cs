using System;
using UnityEngine;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000935 RID: 2357
	public interface IMapBlockCharDataSource
	{
		// Token: 0x06006DEA RID: 28138
		void OnItemRender(int index, GameObject obj);

		// Token: 0x17000CB7 RID: 3255
		// (get) Token: 0x06006DEB RID: 28139
		// (set) Token: 0x06006DEC RID: 28140
		bool CanClick { get; set; }
	}
}
