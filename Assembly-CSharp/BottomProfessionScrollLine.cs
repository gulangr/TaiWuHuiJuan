using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class BottomProfessionScrollLine : MonoBehaviour
{
	// Token: 0x04000FC5 RID: 4037
	public CImage professionIcon;

	// Token: 0x04000FC6 RID: 4038
	public RectTransform skillLayout;

	// Token: 0x04000FC7 RID: 4039
	public TextMeshProUGUI professionNameLabel;

	// Token: 0x04000FC8 RID: 4040
	public CImage progressImage;

	// Token: 0x04000FC9 RID: 4041
	public TextMeshProUGUI progressLabel;

	// Token: 0x04000FCA RID: 4042
	public List<CImage> split;

	// Token: 0x04000FCB RID: 4043
	public GameObject progressArea;

	// Token: 0x04000FCC RID: 4044
	public TooltipInvoker progressTip;

	// Token: 0x04000FCD RID: 4045
	public CImage extraProgressImage;
}
