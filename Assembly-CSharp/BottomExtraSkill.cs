using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class BottomExtraSkill : MonoBehaviour
{
	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06001290 RID: 4752 RVA: 0x0007107D File Offset: 0x0006F27D
	public CButton Button
	{
		get
		{
			return this.button;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x06001291 RID: 4753 RVA: 0x00071085 File Offset: 0x0006F285
	public GameObject CoolDown
	{
		get
		{
			return this.coolDown;
		}
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06001292 RID: 4754 RVA: 0x0007108D File Offset: 0x0006F28D
	public TooltipInvoker Tip
	{
		get
		{
			return this.tip;
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x00071098 File Offset: 0x0006F298
	public void Refresh(bool isReady, Action onClick)
	{
		base.gameObject.SetActive(true);
		bool flag = this.button != null;
		if (flag)
		{
			this.button.interactable = isReady;
			this.button.ClearAndAddListener(onClick);
		}
		bool flag2 = this.coolDown != null;
		if (flag2)
		{
			this.coolDown.SetActive(!isReady);
		}
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x00071100 File Offset: 0x0006F300
	public void RefreshTip()
	{
		bool flag = this.tip == null;
		if (!flag)
		{
			this.tip.Type = TipType.ExtraProfessionSkill;
			this.tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
	}

	// Token: 0x04000FC2 RID: 4034
	[SerializeField]
	private CButton button;

	// Token: 0x04000FC3 RID: 4035
	[SerializeField]
	private GameObject coolDown;

	// Token: 0x04000FC4 RID: 4036
	[SerializeField]
	private TooltipInvoker tip;
}
