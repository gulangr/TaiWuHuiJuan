using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B8C RID: 2956
	public class SkillBreakCellSpecial : SkillBreakCellBase
	{
		// Token: 0x060091E1 RID: 37345 RVA: 0x0043EDFC File Offset: 0x0043CFFC
		protected override void RefreshInternal()
		{
			ESkillBreakGridState state = this._cellData.State;
			base.SetAllImageDim(state != ESkillBreakGridState.Selected && state != ESkillBreakGridState.Failed && state != ESkillBreakGridState.CanSelect);
			short successRate = this._plate.CalcSuccessRate(this._coordinate);
			this.RefreshButton(base.CanSelect());
			base.RefreshPowerLabel(this.powerLabel);
			this.cellName.text = this._cellData.Template.Name;
			this.successRateLabel.text = string.Format("{0}%", successRate);
			this.RefreshNormalCellTip(this.button.GetComponent<TooltipInvoker>());
			this.canSelect.SetActive(this._cellData.State == ESkillBreakGridState.CanSelect);
			this._swapSelectableOverride = false;
		}

		// Token: 0x060091E2 RID: 37346 RVA: 0x0043EEC8 File Offset: 0x0043D0C8
		private void RefreshNormalCellTip(TooltipInvoker tip)
		{
			bool flag = tip == null;
			if (!flag)
			{
				if (tip.RuntimeParam == null)
				{
					tip.RuntimeParam = new ArgumentBox();
				}
				tip.Type = TipType.SkillBreakNormalCell;
				tip.RuntimeParam.SetObject("Plate", this._plate).Set("SkillId", this._skillId).SetObject("LifeSkillAttainments", this._lifeSkillAttainments).Set<SkillBreakPlateIndex>("Coord", this._coordinate).Set("NeedExp", base.GetNeedExp()).Set("CurrentExp", this._currentExp);
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x060091E3 RID: 37347 RVA: 0x0043EF7E File Offset: 0x0043D17E
		private void RefreshButton(bool interactable)
		{
			this.button.interactable = interactable;
			base.BindButtonEvent(this.button);
			this.button.GetComponent<PointerTrigger>().enabled = interactable;
		}

		// Token: 0x060091E4 RID: 37348 RVA: 0x0043EFAD File Offset: 0x0043D1AD
		public override void SetAllImageDoubleDim()
		{
			this._valueFactor = 0.25f;
			base.ApplyStyleNow();
		}

		// Token: 0x060091E5 RID: 37349 RVA: 0x0043EFC4 File Offset: 0x0043D1C4
		public override void SetSwapSelectable(bool selectable)
		{
			bool flag = this.canSelect == null || this.button == null;
			if (!flag)
			{
				if (selectable)
				{
					bool flag2 = !this._swapSelectableOverride;
					if (flag2)
					{
						this._swapSelectablePrevState = this.canSelect.activeSelf;
						this._swapSelectablePrevInteractable = this.button.interactable;
						this._swapSelectablePrevIsDim = base.IsDimOrDisableApplied;
						this._swapSelectableOverride = true;
					}
					this.canSelect.SetActive(true);
					this.RefreshButton(true);
					this._valueFactor = 1f;
					base.ApplyStyleNow();
				}
				else
				{
					bool flag3 = !this._swapSelectableOverride;
					if (!flag3)
					{
						this.canSelect.SetActive(this._swapSelectablePrevState);
						this.RefreshButton(this._swapSelectablePrevInteractable);
						this._valueFactor = (this._swapSelectablePrevIsDim ? 0.4f : 1f);
						base.ApplyStyleNow();
						this._swapSelectableOverride = false;
					}
				}
			}
		}

		// Token: 0x04007067 RID: 28775
		[Header("特殊格子组件")]
		[SerializeField]
		private CButton button;

		// Token: 0x04007068 RID: 28776
		[SerializeField]
		private GameObject canSelect;

		// Token: 0x04007069 RID: 28777
		[SerializeField]
		private TextMeshProUGUI cellName;

		// Token: 0x0400706A RID: 28778
		[SerializeField]
		private TextMeshProUGUI powerLabel;

		// Token: 0x0400706B RID: 28779
		[SerializeField]
		private TextMeshProUGUI successRateLabel;

		// Token: 0x0400706C RID: 28780
		private bool _swapSelectableOverride;

		// Token: 0x0400706D RID: 28781
		private bool _swapSelectablePrevState;

		// Token: 0x0400706E RID: 28782
		private bool _swapSelectablePrevInteractable;

		// Token: 0x0400706F RID: 28783
		private bool _swapSelectablePrevIsDim;
	}
}
