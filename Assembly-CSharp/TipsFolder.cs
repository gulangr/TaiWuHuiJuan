using System;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class TipsFolder : MonoBehaviour
{
	// Token: 0x06002BE1 RID: 11233 RVA: 0x00157628 File Offset: 0x00155828
	private void Awake()
	{
		bool flag = null == this.mouseTip;
		if (flag)
		{
			this.mouseTip = base.GetComponent<TooltipInvoker>();
		}
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x00157654 File Offset: 0x00155854
	private void Update()
	{
		bool flag = null == this.mouseTip;
		if (!flag)
		{
			bool unfold = Input.GetKey(this.unfoldKeyL) || Input.GetKey(this.unfoldKeyR);
			bool unfoldAlways = this.canUnfoldAlways && !SingletonObject.getInstance<GlobalSettings>().AbbreviatedInformation;
			TipType showType = (unfold || unfoldAlways) ? this.baseType : this.foldType;
			bool flag2 = this.mouseTip.Type == showType;
			if (!flag2)
			{
				bool showing = this.mouseTip.Showing;
				bool flag3 = showing;
				if (flag3)
				{
					this.mouseTip.HideTips();
				}
				this.mouseTip.Type = showType;
				bool flag4 = showing;
				if (flag4)
				{
					this.mouseTip.ShowTips();
				}
			}
		}
	}

	// Token: 0x04001FE0 RID: 8160
	public TipType baseType = TipType.Simple;

	// Token: 0x04001FE1 RID: 8161
	public TipType foldType = TipType.Fold;

	// Token: 0x04001FE2 RID: 8162
	public KeyCode unfoldKeyL = KeyCode.LeftShift;

	// Token: 0x04001FE3 RID: 8163
	public KeyCode unfoldKeyR = KeyCode.RightShift;

	// Token: 0x04001FE4 RID: 8164
	[SerializeField]
	private bool canUnfoldAlways = true;

	// Token: 0x04001FE5 RID: 8165
	[SerializeField]
	private TooltipInvoker mouseTip;
}
