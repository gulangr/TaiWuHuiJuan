using System;
using TMPro;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class CharacterSettlementAndInfluence : MonoBehaviour
{
	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001C00 RID: 7168 RVA: 0x000C1B86 File Offset: 0x000BFD86
	// (set) Token: 0x06001C01 RID: 7169 RVA: 0x000C1B90 File Offset: 0x000BFD90
	public int Influence
	{
		get
		{
			return this.influence;
		}
		set
		{
			TMP_Text tmp_Text = this.influenceText;
			this.influence = value;
			int num = value;
			tmp_Text.text = num.ToString();
		}
	}

	// Token: 0x040015D5 RID: 5589
	[SerializeField]
	private int influence;

	// Token: 0x040015D6 RID: 5590
	[SerializeField]
	private TMP_Text influenceText;

	// Token: 0x040015D7 RID: 5591
	[NonSerialized]
	public int CurrSettlement;
}
