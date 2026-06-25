using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000824 RID: 2084
	public class CommonAtomSubTitle : CommonAtomBase
	{
		// Token: 0x0600664A RID: 26186 RVA: 0x002EB537 File Offset: 0x002E9737
		public override void SetMarginLeft(int marginLeft)
		{
		}

		// Token: 0x0600664B RID: 26187 RVA: 0x002EB53A File Offset: 0x002E973A
		public void SetText(string content)
		{
			CommonAtomBase.SetLabelText(this.textLabel, content);
		}

		// Token: 0x0400478E RID: 18318
		[SerializeField]
		private TextMeshProUGUI textLabel;
	}
}
