using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x0200070C RID: 1804
	public class ViewRename : UIBase
	{
		// Token: 0x06005566 RID: 21862 RVA: 0x002794A9 File Offset: 0x002776A9
		private void Awake()
		{
			this.confirm.onClick.ResetListener(new Action(this.TrySubmit));
			this.cancel.onClick.ResetListener(new Action(this.QuickHide));
		}

		// Token: 0x06005567 RID: 21863 RVA: 0x002794E8 File Offset: 0x002776E8
		public override void QuickHide()
		{
			bool isHideDescription = this._renameCfg.IsHideDescription;
			if (isHideDescription)
			{
				this.description.gameObject.SetActive(true);
			}
			Action action = this._renameCfg.Cancel;
			if (action != null)
			{
				action();
			}
			base.QuickHide();
		}

		// Token: 0x06005568 RID: 21864 RVA: 0x00279538 File Offset: 0x00277738
		public override void OnInit(ArgumentBox argsBox)
		{
			if (argsBox != null)
			{
				argsBox.Get<RenameCfg>("Cfg", out this._renameCfg);
			}
			this.title.text = this._renameCfg.Title;
			this.description.text = this._renameCfg.Description;
			this.emptyDesc.text = this._renameCfg.EmptyDesc;
			this.inputField.text = this._renameCfg.Default;
			this.inputField.characterLimit = this._renameCfg.CharCount;
			this.sensitiveWarningTip.alpha = 0f;
			bool isHideDescription = this._renameCfg.IsHideDescription;
			if (isHideDescription)
			{
				this.description.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005569 RID: 21865 RVA: 0x00279604 File Offset: 0x00277804
		private void Update()
		{
			bool flag = !this.inputField.isFocused && CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.TrySubmit();
			}
		}

		// Token: 0x0600556A RID: 21866 RVA: 0x00279644 File Offset: 0x00277844
		private void TrySubmit()
		{
			List<SensitiveWordsMatchResult> list = SensitiveWordsSystem.Instance.TryMatch(this.inputField.text, 10);
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				this.sensitiveWarningTip.alpha = 1f;
				bool flag2 = this._sensitiveTipsCoroutine != null;
				if (flag2)
				{
					base.StopCoroutine(this._sensitiveTipsCoroutine);
				}
				Tween sensitiveTipsTween = this._sensitiveTipsTween;
				if (sensitiveTipsTween != null)
				{
					sensitiveTipsTween.Kill(false);
				}
				bool activeInHierarchy = this.sensitiveWarningTip.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					this._sensitiveTipsCoroutine = base.StartCoroutine(this.DelayHideSensitiveCanvas);
				}
			}
			else
			{
				Action<string> submit = this._renameCfg.Submit;
				if (submit != null)
				{
					submit(this.inputField.text.Trim());
				}
				this.QuickHide();
			}
		}

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x0600556B RID: 21867 RVA: 0x00279714 File Offset: 0x00277914
		private IEnumerator DelayHideSensitiveCanvas
		{
			get
			{
				yield return new WaitForSeconds(SensitiveWordsSystem.SensitiveWordAnimationStayTime);
				bool activeInHierarchy = this.sensitiveWarningTip.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					this._sensitiveTipsTween = this.sensitiveWarningTip.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
				}
				yield break;
			}
		}

		// Token: 0x04003A35 RID: 14901
		private RenameCfg _renameCfg;

		// Token: 0x04003A36 RID: 14902
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04003A37 RID: 14903
		[SerializeField]
		private TMP_Text description;

		// Token: 0x04003A38 RID: 14904
		[SerializeField]
		private TMP_Text emptyDesc;

		// Token: 0x04003A39 RID: 14905
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04003A3A RID: 14906
		[SerializeField]
		public CButton confirm;

		// Token: 0x04003A3B RID: 14907
		[SerializeField]
		public CButton cancel;

		// Token: 0x04003A3C RID: 14908
		[SerializeField]
		public CanvasGroup sensitiveWarningTip;

		// Token: 0x04003A3D RID: 14909
		private Coroutine _sensitiveTipsCoroutine;

		// Token: 0x04003A3E RID: 14910
		private Tween _sensitiveTipsTween;
	}
}
