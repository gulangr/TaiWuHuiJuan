using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE4 RID: 3812
	public class SingleButtonCell : MonoBehaviour, ICellContent<SingleButtonCellData>, ICellContent
	{
		// Token: 0x0600AF40 RID: 44864 RVA: 0x004FD80C File Offset: 0x004FBA0C
		public void SetData(SingleButtonCellData data)
		{
			this.label.text = data.LabelText;
			this.button.onClick.ResetListener(data.OnClick);
			bool flag = this.mouseTipDisplayer != null;
			if (flag)
			{
				this.mouseTipDisplayer.PresetParam[0] = data.MouseTipText;
				this.mouseTipDisplayer.enabled = ((data.SingleButtonCellStatus & SingleButtonCellStatus.Tip) > SingleButtonCellStatus.None);
			}
			Graphic[] targets = this.raycastTargets;
			bool flag2 = targets != null && targets.Length > 0;
			if (flag2)
			{
				foreach (Graphic target in targets)
				{
					target.raycastTarget = ((SingleButtonCellStatus.Raycast & data.SingleButtonCellStatus) > SingleButtonCellStatus.None);
				}
			}
			this.button.interactable = ((SingleButtonCellStatus.Interactable & data.SingleButtonCellStatus) > SingleButtonCellStatus.None);
		}

		// Token: 0x040087D2 RID: 34770
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040087D3 RID: 34771
		[SerializeField]
		private CButton button;

		// Token: 0x040087D4 RID: 34772
		[SerializeField]
		private Graphic[] raycastTargets;

		// Token: 0x040087D5 RID: 34773
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;
	}
}
