using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	// Token: 0x020006F3 RID: 1779
	public class ViewSelectFeatureNewGame : UIBase
	{
		// Token: 0x06005462 RID: 21602 RVA: 0x0027205C File Offset: 0x0027025C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<List<short>>("FeatureIds", out this._featureIds);
			argsBox.Get("CurrentSelected", out this._currentSelected);
			argsBox.Get<Action<short>>("OnConfirm", out this._onConfirm);
			argsBox.Get<Action>("OnCancel", out this._onCancel);
			bool flag = this._currentSelected < 0;
			if (flag)
			{
				this._currentSelected = this._featureIds[0];
			}
		}

		// Token: 0x06005463 RID: 21603 RVA: 0x002720D4 File Offset: 0x002702D4
		private void Awake()
		{
			this.featureTogGroup.OnActiveIndexChange += this.OnSelectedFeatureChange;
			this.confirmBtn.ClearAndAddListener(new Action(this.OnConfirm));
			this.cancelBtn.ClearAndAddListener(new Action(this.OnCancel));
		}

		// Token: 0x06005464 RID: 21604 RVA: 0x0027212A File Offset: 0x0027032A
		private void OnConfirm()
		{
			Action<short> onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm(this._featureIds[this.featureTogGroup.GetActiveIndex()]);
			}
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x06005465 RID: 21605 RVA: 0x00272166 File Offset: 0x00270366
		private void OnCancel()
		{
			this.QuickHide();
		}

		// Token: 0x06005466 RID: 21606 RVA: 0x00272170 File Offset: 0x00270370
		public override void QuickHide()
		{
			base.QuickHide();
			Action onCancel = this._onCancel;
			if (onCancel != null)
			{
				onCancel();
			}
		}

		// Token: 0x06005467 RID: 21607 RVA: 0x0027218C File Offset: 0x0027038C
		private void OnSelectedFeatureChange(int newTog, int oldTog)
		{
		}

		// Token: 0x06005468 RID: 21608 RVA: 0x0027218F File Offset: 0x0027038F
		private void OnEnable()
		{
			this.RefreshDisplay();
		}

		// Token: 0x06005469 RID: 21609 RVA: 0x0027219C File Offset: 0x0027039C
		private void RefreshDisplay()
		{
			this.initialPointsArea.gameObject.SetActive(false);
			int colAmount = (this._featureIds.Count > 6) ? 4 : 3;
			this.featureLayout.constraintCount = colAmount;
			this.featureTogGroup.Clear();
			int selectedIndex = this._featureIds.IndexOf(this._currentSelected);
			selectedIndex = Math.Max(0, selectedIndex);
			CommonUtils.PrepareEnoughChildren(this.featureLayout.transform, this.featureTemplate.gameObject, this._featureIds.Count, null);
			for (int i = 0; i < this._featureIds.Count; i++)
			{
				Feature featureItem = this.featureLayout.transform.GetChild(i).GetComponent<Feature>();
				this.featureTogGroup.Add(featureItem.GetComponent<CToggle>());
				featureItem.Set(this._featureIds[i], -1, false, -1);
				featureItem.SetTipTemplateDataOnly(true);
				featureItem.SetTipEnabled(true);
			}
			this.featureTogGroup.Init(-1);
			this.featureTogGroup.SetWithoutNotify(0);
		}

		// Token: 0x0600546A RID: 21610 RVA: 0x002722C0 File Offset: 0x002704C0
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.confirmBtn.interactable;
			if (flag)
			{
				this.OnConfirm();
			}
		}

		// Token: 0x04003924 RID: 14628
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04003925 RID: 14629
		[SerializeField]
		private CButton cancelBtn;

		// Token: 0x04003926 RID: 14630
		[SerializeField]
		private GridLayoutGroup featureLayout;

		// Token: 0x04003927 RID: 14631
		[SerializeField]
		private CToggleGroup featureTogGroup;

		// Token: 0x04003928 RID: 14632
		[SerializeField]
		private Feature featureTemplate;

		// Token: 0x04003929 RID: 14633
		[SerializeField]
		private GameObject initialPointsArea;

		// Token: 0x0400392A RID: 14634
		private List<short> _featureIds = new List<short>();

		// Token: 0x0400392B RID: 14635
		private Action<short> _onConfirm;

		// Token: 0x0400392C RID: 14636
		private Action _onCancel;

		// Token: 0x0400392D RID: 14637
		private short _currentSelected = -1;
	}
}
