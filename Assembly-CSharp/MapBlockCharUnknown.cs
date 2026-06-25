using System;
using GameData.Domains.Map;

// Token: 0x020003D4 RID: 980
public sealed class MapBlockCharUnknown : MapBlockCharBase
{
	// Token: 0x06003B03 RID: 15107 RVA: 0x001DE81E File Offset: 0x001DCA1E
	public void Init(bool canInteract, MapBlockData mapBlock)
	{
		base.Init(canInteract, mapBlock, null);
		this.Refresh();
	}

	// Token: 0x06003B04 RID: 15108 RVA: 0x001DE832 File Offset: 0x001DCA32
	protected override void RefreshName()
	{
		this.nameText.text = LocalStringManager.Get(LanguageKey.LK_Unknown).SetColor("lightgrey");
	}

	// Token: 0x06003B05 RID: 15109 RVA: 0x001DE855 File Offset: 0x001DCA55
	protected override void RefreshOrganization()
	{
		this.organizationText.text = LocalStringManager.Get(LanguageKey.LK_Unknown);
		this.organizationIcon.gameObject.SetActive(false);
	}
}
