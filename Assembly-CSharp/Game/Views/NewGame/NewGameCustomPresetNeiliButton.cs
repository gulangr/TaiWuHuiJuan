using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007E5 RID: 2021
	public class NewGameCustomPresetNeiliButton : MonoBehaviour
	{
		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x06006289 RID: 25225 RVA: 0x002D1D2A File Offset: 0x002CFF2A
		public sbyte FiveElementType
		{
			get
			{
				return this.fiveElementType;
			}
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x002D1D34 File Offset: 0x002CFF34
		private void Awake()
		{
			this.tips.Type = TipType.GeneralLines;
			TooltipInvoker tooltipInvoker = this.tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
		}

		// Token: 0x0600628B RID: 25227 RVA: 0x002D1D6C File Offset: 0x002CFF6C
		public void Initialize(Action<sbyte> onClick)
		{
			this.button.onClick.ResetListener(delegate()
			{
				onClick(this.FiveElementType);
			});
		}

		// Token: 0x0600628C RID: 25228 RVA: 0x002D1DAB File Offset: 0x002CFFAB
		public void RefreshState(bool isSelected, bool isConflicting)
		{
			this.selectedState.SetActive(isSelected);
			this.conflictingEffect.SetActive(isSelected && isConflicting);
		}

		// Token: 0x0600628D RID: 25229 RVA: 0x002D1DCC File Offset: 0x002CFFCC
		public void RefreshTips(Action<ArgumentBox> fillTipData)
		{
			TooltipInvoker tooltipInvoker = this.tips;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = new ArgumentBox());
			}
			ArgumentBox tipParam = argumentBox;
			tipParam.Clear();
			fillTipData(tipParam);
			this.tips.Refresh(false, -1);
		}

		// Token: 0x0400448F RID: 17551
		[SerializeField]
		private sbyte fiveElementType = 5;

		// Token: 0x04004490 RID: 17552
		[SerializeField]
		private CButton button;

		// Token: 0x04004491 RID: 17553
		[SerializeField]
		private TooltipInvoker tips;

		// Token: 0x04004492 RID: 17554
		[SerializeField]
		private GameObject selectedState;

		// Token: 0x04004493 RID: 17555
		[SerializeField]
		private GameObject conflictingEffect;
	}
}
