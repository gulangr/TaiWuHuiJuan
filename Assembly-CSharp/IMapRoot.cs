using System;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public interface IMapRoot
{
	// Token: 0x06003BBB RID: 15291
	void SetDirty();

	// Token: 0x06003BBC RID: 15292
	Vector2 ToPos(Location location);

	// Token: 0x06003BBD RID: 15293
	RectTransform Layer2Root(EMapLayer layer, Location location);
}
