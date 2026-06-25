using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B89 RID: 2953
	public class SkillBreakCellNormal : SkillBreakCellBase
	{
		// Token: 0x060091D3 RID: 37331 RVA: 0x0043E744 File Offset: 0x0043C944
		protected override void RefreshInternal()
		{
			bool isReallyNormal = base.TemplateId == 3;
			this.RefreshNormalCellImage(isReallyNormal);
			ESkillBreakGridState state = this._cellData.State;
			base.SetAllImageDim(state != ESkillBreakGridState.Selected && state != ESkillBreakGridState.Failed && state != ESkillBreakGridState.CanSelect);
			short successRate = this._plate.CalcSuccessRate(this._coordinate);
			this.RefreshButton(base.CanSelect());
			base.RefreshPowerLabel(this.powerLabel);
			this.cellName.gameObject.SetActive(!isReallyNormal);
			this.cellName.text = this._cellData.Template.Name;
			this.successRateLabel.text = string.Format("{0}%", successRate);
			this.RefreshNormalCellTip(this.button.GetComponent<TooltipInvoker>());
			CommonUtils.SetActiveByMove(this.canSelect, this._cellData.State == ESkillBreakGridState.CanSelect);
			this._swapSelectableOverride = false;
		}

		// Token: 0x060091D4 RID: 37332 RVA: 0x0043E838 File Offset: 0x0043CA38
		private void RefreshNormalCellImage(bool isReallyNormal)
		{
			string buttonSprite;
			string hoverSprite;
			string canSelectSprite;
			if (isReallyNormal)
			{
				buttonSprite = "ui9_back_charactermenu_skillbreak_ruchang_1";
				hoverSprite = "ui9_back_charactermenu_skillbreak_ruchang_0";
				canSelectSprite = "ui9_back_charactermenu_skillbreak_ruchang_2";
			}
			else
			{
				buttonSprite = "ui9_back_charactermenu_skillbreak_ruchang_1";
				hoverSprite = "ui9_back_charactermenu_skillbreak_ruchang_0";
				canSelectSprite = "ui9_back_charactermenu_skillbreak_ruchang_2";
			}
			this.buttonImage.SetSprite(buttonSprite, false, null);
			this.hover.SetSprite(hoverSprite, false, null);
			this.canSelectImage.SetSprite(canSelectSprite, false, null);
		}

		// Token: 0x060091D5 RID: 37333 RVA: 0x0043E8A4 File Offset: 0x0043CAA4
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

		// Token: 0x060091D6 RID: 37334 RVA: 0x0043E95A File Offset: 0x0043CB5A
		private void RefreshButton(bool interactable)
		{
			this.button.interactable = interactable;
			base.BindButtonEvent(this.button);
			this.button.GetComponent<PointerTrigger>().enabled = interactable;
		}

		// Token: 0x060091D7 RID: 37335 RVA: 0x0043E989 File Offset: 0x0043CB89
		public override void SetAllImageDoubleDim()
		{
			this._valueFactor = 0.25f;
			base.ApplyStyleNow();
		}

		// Token: 0x060091D8 RID: 37336 RVA: 0x0043E9A0 File Offset: 0x0043CBA0
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
						this._swapSelectablePrevState = this.canSelect.gameObject.activeSelf;
						this._swapSelectablePrevInteractable = this.button.interactable;
						this._swapSelectablePrevIsDim = base.IsDimOrDisableApplied;
						this._swapSelectableOverride = true;
					}
					CommonUtils.SetActiveByMove(this.canSelect, true);
					this.RefreshButton(true);
					this._valueFactor = 1f;
					base.ApplyStyleNow();
				}
				else
				{
					bool flag3 = !this._swapSelectableOverride;
					if (!flag3)
					{
						CommonUtils.SetActiveByMove(this.canSelect, this._swapSelectablePrevState);
						this.RefreshButton(this._swapSelectablePrevInteractable);
						this._valueFactor = (this._swapSelectablePrevIsDim ? 0.4f : 1f);
						base.ApplyStyleNow();
						this._swapSelectableOverride = false;
					}
				}
			}
		}

		// Token: 0x04007052 RID: 28754
		[FormerlySerializedAs("_normalButton")]
		[Header("普通格子组件")]
		[SerializeField]
		private CButton button;

		// Token: 0x04007053 RID: 28755
		[SerializeField]
		private CImage buttonImage;

		// Token: 0x04007054 RID: 28756
		[SerializeField]
		private RectTransform canSelect;

		// Token: 0x04007055 RID: 28757
		[SerializeField]
		private CImage canSelectImage;

		// Token: 0x04007056 RID: 28758
		[SerializeField]
		private CImage hover;

		// Token: 0x04007057 RID: 28759
		[SerializeField]
		private TextMeshProUGUI cellName;

		// Token: 0x04007058 RID: 28760
		[SerializeField]
		private TextMeshProUGUI powerLabel;

		// Token: 0x04007059 RID: 28761
		[SerializeField]
		private TextMeshProUGUI successRateLabel;

		// Token: 0x0400705A RID: 28762
		private bool _swapSelectableOverride;

		// Token: 0x0400705B RID: 28763
		private bool _swapSelectablePrevState;

		// Token: 0x0400705C RID: 28764
		private bool _swapSelectablePrevInteractable;

		// Token: 0x0400705D RID: 28765
		private bool _swapSelectablePrevIsDim;
	}
}
