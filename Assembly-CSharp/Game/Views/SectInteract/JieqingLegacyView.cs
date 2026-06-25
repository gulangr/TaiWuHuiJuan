using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A2 RID: 2466
	public class JieqingLegacyView : MonoBehaviour
	{
		// Token: 0x060076B7 RID: 30391 RVA: 0x00374F7A File Offset: 0x0037317A
		private void Awake()
		{
			this.btnMain.ClearAndAddListener(new Action(this.OnMainButtonClick));
		}

		// Token: 0x060076B8 RID: 30392 RVA: 0x00374F95 File Offset: 0x00373195
		private void OnMainButtonClick()
		{
			this._isSelected = !this._isSelected;
			Action<bool> onClick = this._onClick;
			if (onClick != null)
			{
				onClick(this._isSelected);
			}
		}

		// Token: 0x060076B9 RID: 30393 RVA: 0x00374FC0 File Offset: 0x003731C0
		public void RefreshBasicInfo(LegacyItem configData)
		{
			this.gradeBack.SetSprite(string.Format("{0}{1}", "ui9_icon_item_grade_", configData.Grade), false, null);
			this.validObj.SetActive(true);
			this.invalidObj.SetActive(false);
			this.nameLabel.SetText(configData.Name, true);
			this.icon.SetSprite(configData.Icon, false, null);
		}

		// Token: 0x060076BA RID: 30394 RVA: 0x00375037 File Offset: 0x00373237
		private void SetSelectedVisual(bool isFixed = false)
		{
			this.selected.SetActive(this._isSelected);
		}

		// Token: 0x060076BB RID: 30395 RVA: 0x0037504C File Offset: 0x0037324C
		public void RefreshCostInfo(LegacyItem configData, bool showCost, bool isSelected, bool hasEnoughPoints, bool isExtra, bool isStarFortune = false)
		{
			this._isSelected = isSelected;
			int cost = (int)(isStarFortune ? configData.ExtraCost : configData.Cost);
			cost = ((isExtra && cost > 0) ? 0 : cost);
			this.costBack.SetActive(showCost);
			this.txtCostNotEnoughLabel.SetText(cost.ToString(), true);
			this.txtCostLabel.SetText(cost.ToString(), true);
			this.txtCostNotEnoughLabel.gameObject.SetActive(!hasEnoughPoints);
			this.txtCostLabel.gameObject.SetActive(hasEnoughPoints);
		}

		// Token: 0x060076BC RID: 30396 RVA: 0x003750E0 File Offset: 0x003732E0
		public void RefreshInteraction(bool isInteractable, bool isSelected, bool isDisabled)
		{
			this.SetSelected(isSelected, false);
			this.btnMain.interactable = (isSelected || isInteractable);
			if (isDisabled)
			{
				DisableStyleRoot disableStyleRoot = base.gameObject.GetOrAddComponent<DisableStyleRoot>();
				disableStyleRoot.SetStyleEffect(true, false);
				this.btnMain.interactable = false;
			}
			else
			{
				DisableStyleRoot disableStyleRoot2 = base.gameObject.GetComponent<DisableStyleRoot>();
				bool flag = disableStyleRoot2;
				if (flag)
				{
					disableStyleRoot2.SetStyleEffect(false, false);
				}
			}
		}

		// Token: 0x060076BD RID: 30397 RVA: 0x00375152 File Offset: 0x00373352
		public void SetSelected(bool isSelected, bool notify = false)
		{
			this._isSelected = isSelected;
			this.SetSelectedVisual(isSelected);
		}

		// Token: 0x060076BE RID: 30398 RVA: 0x00375164 File Offset: 0x00373364
		public void RefreshMouseTip(LegacyItem configData, string desc)
		{
			bool flag = !string.IsNullOrEmpty(desc);
			if (flag)
			{
				this.mouseTip.enabled = true;
				this.mouseTip.PresetParam[0] = configData.Name;
				this.mouseTip.PresetParam[1] = desc;
				this.mouseTip.Refresh(false, -1);
			}
			else
			{
				this.mouseTip.enabled = false;
			}
		}

		// Token: 0x060076BF RID: 30399 RVA: 0x003751CC File Offset: 0x003733CC
		public void SetOnToggleValueChanged(Action<bool> action)
		{
			this._onClick = action;
		}

		// Token: 0x04005987 RID: 22919
		[SerializeField]
		private CImage gradeBack;

		// Token: 0x04005988 RID: 22920
		[SerializeField]
		private GameObject validObj;

		// Token: 0x04005989 RID: 22921
		[SerializeField]
		private GameObject invalidObj;

		// Token: 0x0400598A RID: 22922
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x0400598B RID: 22923
		[SerializeField]
		private CImage icon;

		// Token: 0x0400598C RID: 22924
		[SerializeField]
		private GameObject costBack;

		// Token: 0x0400598D RID: 22925
		[SerializeField]
		private TextMeshProUGUI txtCostNotEnoughLabel;

		// Token: 0x0400598E RID: 22926
		[SerializeField]
		private TextMeshProUGUI txtCostLabel;

		// Token: 0x0400598F RID: 22927
		[SerializeField]
		private GameObject selected;

		// Token: 0x04005990 RID: 22928
		[SerializeField]
		private CButton btnMain;

		// Token: 0x04005991 RID: 22929
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04005992 RID: 22930
		private bool _isSelected = false;

		// Token: 0x04005993 RID: 22931
		private Action<bool> _onClick;
	}
}
