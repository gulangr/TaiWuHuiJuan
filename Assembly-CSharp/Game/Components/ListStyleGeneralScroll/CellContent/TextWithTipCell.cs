using System;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EEB RID: 3819
	public class TextWithTipCell : MonoBehaviour, ICellContent<TextWithTipCellData>, ICellContent
	{
		// Token: 0x0600AF4F RID: 44879 RVA: 0x004FDD30 File Offset: 0x004FBF30
		public void SetData(TextWithTipCellData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.label.text = string.Empty;
			}
			else
			{
				this.label.text = (data.Text ?? string.Empty);
				TMPTextSpriteHelper spriteHelper;
				bool flag2 = this.label.TryGetComponent<TMPTextSpriteHelper>(out spriteHelper);
				if (flag2)
				{
					spriteHelper.Parse();
				}
				Action<TooltipInvoker> tipRefresher = data.TipRefresher;
				if (tipRefresher != null)
				{
					tipRefresher(this.tipDisplayer);
				}
			}
		}

		// Token: 0x040087E7 RID: 34791
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040087E8 RID: 34792
		[SerializeField]
		private TooltipInvoker tipDisplayer;
	}
}
