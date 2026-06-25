using System;
using Game.Components.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

// Token: 0x020002BA RID: 698
public class MouseTipMakeTargetMaterialItem : MonoBehaviour
{
	// Token: 0x06002AC3 RID: 10947 RVA: 0x001478F3 File Offset: 0x00145AF3
	public void Init(ITradeableContent content)
	{
		this.itemBack.Set(content, false);
		this.itemBack.SetCount(content.Amount, false);
		this.objNone.gameObject.SetActive(content.Amount == 0);
	}

	// Token: 0x04001EF5 RID: 7925
	[SerializeField]
	private ItemBack itemBack;

	// Token: 0x04001EF6 RID: 7926
	[SerializeField]
	private GameObject objNone;
}
