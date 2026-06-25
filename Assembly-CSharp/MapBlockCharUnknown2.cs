using System;
using GameData.Domains.Map;

// Token: 0x020003D5 RID: 981
public sealed class MapBlockCharUnknown2 : MapBlockCharBase2
{
	// Token: 0x06003B07 RID: 15111 RVA: 0x001DE889 File Offset: 0x001DCA89
	public void Init(bool canInteract, MapBlockData mapBlock)
	{
		base.Init(canInteract, mapBlock, null);
		this.Refresh();
	}

	// Token: 0x06003B08 RID: 15112 RVA: 0x001DE89D File Offset: 0x001DCA9D
	protected override void RefreshName()
	{
		this.nameLabel.text = LocalStringManager.Get(LanguageKey.LK_Unknown).SetColor("lightgrey");
	}

	// Token: 0x06003B09 RID: 15113 RVA: 0x001DE8C0 File Offset: 0x001DCAC0
	protected override void RefreshOrganization()
	{
		this.organizationLabel.text = LocalStringManager.Get(LanguageKey.LK_Unknown);
	}
}
