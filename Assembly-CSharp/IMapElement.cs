using System;
using GameData.Domains.Map;

// Token: 0x020003E4 RID: 996
public interface IMapElement
{
	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x06003BBE RID: 15294
	EMapLayer Layer { get; }

	// Token: 0x06003BBF RID: 15295
	void Refresh(Location location);

	// Token: 0x06003BC0 RID: 15296
	void Scale(float wheel);

	// Token: 0x06003BC1 RID: 15297
	void Collect();
}
