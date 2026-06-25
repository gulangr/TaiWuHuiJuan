using System;
using System.Collections.Generic;
using GameData.DLC.FiveLoong;
using UnityEngine;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000934 RID: 2356
	public interface IMapBlockCharHolder
	{
		// Token: 0x17000CB6 RID: 3254
		// (get) Token: 0x06006DE4 RID: 28132 RVA: 0x0032C5EF File Offset: 0x0032A7EF
		List<LoongInfo> LoongInfos
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06006DE5 RID: 28133
		bool CanClick(DisplayType type, int id);

		// Token: 0x06006DE6 RID: 28134
		void OnClick(DisplayType type, int id);

		// Token: 0x06006DE7 RID: 28135 RVA: 0x0032C5F2 File Offset: 0x0032A7F2
		void OnHover(RectTransform transform, MapBlockChar charObj)
		{
		}

		// Token: 0x06006DE8 RID: 28136 RVA: 0x0032C5F5 File Offset: 0x0032A7F5
		void OnChildHoverEnd()
		{
		}

		// Token: 0x06006DE9 RID: 28137 RVA: 0x0032C5F8 File Offset: 0x0032A7F8
		void OnChildHoverEnd(MapBlockChar charObj)
		{
		}
	}
}
