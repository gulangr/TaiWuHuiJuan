using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001AE RID: 430
public class TeaHorseCaravanExtraItemRow : MonoBehaviour
{
	// Token: 0x06001859 RID: 6233 RVA: 0x000956C4 File Offset: 0x000938C4
	public void SetStatus(bool normalInteractable = true, UnityAction normalClicked = null)
	{
		bool isNormal = normalClicked != null;
		this.normal.SetActive(isNormal);
		this.placeHolder.SetActive(!isNormal);
		this.parent.SetLocked(!isNormal || !normalInteractable);
		bool flag = isNormal && normalInteractable;
		if (flag)
		{
			this.parent.SetClickEvent(normalClicked);
		}
	}

	// Token: 0x04001390 RID: 5008
	[SerializeField]
	private GameObject placeHolder;

	// Token: 0x04001391 RID: 5009
	[SerializeField]
	private GameObject normal;

	// Token: 0x04001392 RID: 5010
	[SerializeField]
	private CommonTableRowForItem parent;
}
