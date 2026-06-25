using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000819 RID: 2073
	public class SubPageCustomPresetToggle : CToggle
	{
		// Token: 0x060065AB RID: 26027 RVA: 0x002E7071 File Offset: 0x002E5271
		public void SetSubPageIndex(int index)
		{
			this._subPageIndex = index;
		}

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x060065AC RID: 26028 RVA: 0x002E707B File Offset: 0x002E527B
		public int SubPageIndex
		{
			get
			{
				return this._subPageIndex;
			}
		}

		// Token: 0x060065AD RID: 26029 RVA: 0x002E7084 File Offset: 0x002E5284
		public void RefreshPointsDisplay(int remainingPoints)
		{
			bool hasRemainingPoints = remainingPoints > 0;
			this.toggleMarker.SetActive(hasRemainingPoints);
			this.toggleMouseTipDisplayer.enabled = hasRemainingPoints;
			this.togglePointsNode.SetActive(hasRemainingPoints);
			this.togglePointsText.text = (hasRemainingPoints ? remainingPoints.ToString() : string.Empty);
		}

		// Token: 0x040046E3 RID: 18147
		[Header("点数显示")]
		[SerializeField]
		private GameObject toggleMarker;

		// Token: 0x040046E4 RID: 18148
		[SerializeField]
		private TooltipInvoker toggleMouseTipDisplayer;

		// Token: 0x040046E5 RID: 18149
		[SerializeField]
		private GameObject togglePointsNode;

		// Token: 0x040046E6 RID: 18150
		[SerializeField]
		private TextMeshProUGUI togglePointsText;

		// Token: 0x040046E7 RID: 18151
		private int _subPageIndex;
	}
}
