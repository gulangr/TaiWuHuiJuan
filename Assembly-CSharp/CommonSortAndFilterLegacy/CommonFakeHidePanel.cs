using System;
using DG.Tweening;
using UnityEngine;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000422 RID: 1058
	public abstract class CommonFakeHidePanel : MonoBehaviour
	{
		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06003ED2 RID: 16082 RVA: 0x001F7361 File Offset: 0x001F5561
		protected bool _fakeMaskTweenAvtive
		{
			get
			{
				return this._fakeMaskTween != null && this._fakeMaskTween.IsActive() && !this._fakeMaskTween.IsComplete();
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06003ED3 RID: 16083 RVA: 0x001F7389 File Offset: 0x001F5589
		protected virtual bool IsMenuShow
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003ED4 RID: 16084
		protected abstract void ShowMenu();

		// Token: 0x06003ED5 RID: 16085
		public abstract void HideMenu();

		// Token: 0x06003ED6 RID: 16086 RVA: 0x001F738C File Offset: 0x001F558C
		protected virtual bool NeedFakeHideMenu()
		{
			bool shortCutClicked = this.ParentElement != null && CommonCommandKit.Alt.Check(this.ParentElement, true, false, false, true, false);
			bool flag = shortCutClicked;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (RectTransform rect in this.checkFakeHideRects)
				{
					bool flag2 = !rect.gameObject.activeSelf;
					if (!flag2)
					{
						bool flag3 = CommonFakeHidePanel.IsMouseInRect(rect);
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06003ED7 RID: 16087 RVA: 0x001F7414 File Offset: 0x001F5614
		protected virtual void Awake()
		{
			this._fakeHideMarkCanvasGroup = this.fakeHideMark.gameObject.GetComponent<CanvasGroup>();
			this._fakeHideMarkCanvasGroup.alpha = 0f;
			bool flag = this.ParentElement == null;
			if (flag)
			{
				this.ParentElement = base.GetComponentInParent<UIBase>().Element;
			}
		}

		// Token: 0x06003ED8 RID: 16088 RVA: 0x001F746C File Offset: 0x001F566C
		protected virtual void Update()
		{
			bool isMenuShow = this.IsMenuShow;
			if (isMenuShow)
			{
				bool flag = !this.CheckFakeHideMenu();
				if (flag)
				{
					this.CheckHideMenu();
				}
			}
			else
			{
				this.CheckShowMenu();
			}
		}

		// Token: 0x06003ED9 RID: 16089 RVA: 0x001F74A8 File Offset: 0x001F56A8
		protected virtual bool CheckFakeHideMenu()
		{
			bool needFakeHideMenu = this.NeedFakeHideMenu();
			bool shortCutClicked = CommonCommandKit.Alt.Check(this.ParentElement, true, false, false, true, false);
			bool flag = needFakeHideMenu || shortCutClicked;
			if (flag)
			{
				bool flag2 = this._fakeMaskState != CommonFakeHidePanel.EFakeMaskState.PendingHighlight && this._fakeMaskState != CommonFakeHidePanel.EFakeMaskState.HighlightMask;
				if (flag2)
				{
					this.UpdateFakeHideGlobalState(true);
					this.DoFadeFakeMask(true);
				}
			}
			else
			{
				bool flag3 = this._fakeMaskState != CommonFakeHidePanel.EFakeMaskState.PendingNormal && this._fakeMaskState != CommonFakeHidePanel.EFakeMaskState.NormalMask;
				if (flag3)
				{
					this.UpdateFakeHideGlobalState(false);
					this.DoFadeFakeMask(false);
				}
			}
			return needFakeHideMenu;
		}

		// Token: 0x06003EDA RID: 16090 RVA: 0x001F7548 File Offset: 0x001F5748
		protected virtual void CheckHideMenu()
		{
			foreach (RectTransform rect in this.checkExitRects)
			{
				bool flag = !rect.gameObject.activeSelf;
				if (!flag)
				{
					bool flag2 = CommonFakeHidePanel.IsMouseInRect(rect);
					if (flag2)
					{
						return;
					}
				}
			}
			this.HideMenu();
		}

		// Token: 0x06003EDB RID: 16091
		protected abstract void CheckShowMenu();

		// Token: 0x06003EDC RID: 16092 RVA: 0x001F759C File Offset: 0x001F579C
		protected static bool IsMouseInRect(RectTransform rect)
		{
			Vector2 localPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, UIManager.Instance.UiCamera, out localPos);
			return rect.rect.Contains(localPos);
		}

		// Token: 0x06003EDD RID: 16093 RVA: 0x001F75DC File Offset: 0x001F57DC
		protected void DoFadeFakeMask(bool goFakeHide)
		{
			bool fakeMaskTweenAvtive = this._fakeMaskTweenAvtive;
			if (fakeMaskTweenAvtive)
			{
				this._fakeMaskTween.Kill(true);
			}
			this._fakeMaskTween = DOTween.Sequence();
			this._fakeMaskState = (goFakeHide ? CommonFakeHidePanel.EFakeMaskState.PendingHighlight : CommonFakeHidePanel.EFakeMaskState.PendingNormal);
			this.BeforeFadeFakeMask(goFakeHide);
			this._fakeHideMarkCanvasGroup.alpha = (goFakeHide ? 0f : 0.4f);
			this._fakeMaskTween.Join(this._fakeHideMarkCanvasGroup.DOFade(goFakeHide ? 0.4f : 0f, 0.2f));
			this.JoinFadeFakeMask(this._fakeMaskTween, goFakeHide);
			this._fakeMaskTween.OnComplete(delegate
			{
				this.FadeFakeMaskOver(goFakeHide);
				this._fakeHideMarkCanvasGroup.alpha = (goFakeHide ? 0.4f : 0f);
				this._fakeMaskState = (goFakeHide ? CommonFakeHidePanel.EFakeMaskState.HighlightMask : CommonFakeHidePanel.EFakeMaskState.NormalMask);
			});
		}

		// Token: 0x06003EDE RID: 16094 RVA: 0x001F76BC File Offset: 0x001F58BC
		protected virtual void FadeFakeMaskOver(bool goFakeHide)
		{
		}

		// Token: 0x06003EDF RID: 16095 RVA: 0x001F76BF File Offset: 0x001F58BF
		protected virtual void JoinFadeFakeMask(Sequence fakeMaskTween, bool goFakeHide)
		{
		}

		// Token: 0x06003EE0 RID: 16096 RVA: 0x001F76C2 File Offset: 0x001F58C2
		protected virtual void BeforeFadeFakeMask(bool goFakeHide)
		{
		}

		// Token: 0x06003EE1 RID: 16097 RVA: 0x001F76C8 File Offset: 0x001F58C8
		protected void UpdateFakeHideGlobalState(bool isFakeHide)
		{
			if (isFakeHide)
			{
				bool flag = !this._isInFakeHideState;
				if (flag)
				{
					this._isInFakeHideState = true;
					CommonFakeHidePanel._fakeHideMenuCount++;
				}
			}
			else
			{
				bool isInFakeHideState = this._isInFakeHideState;
				if (isInFakeHideState)
				{
					this._isInFakeHideState = false;
					CommonFakeHidePanel._fakeHideMenuCount--;
				}
			}
		}

		// Token: 0x04002D1B RID: 11547
		protected CanvasGroup _fakeHideMarkCanvasGroup;

		// Token: 0x04002D1C RID: 11548
		[SerializeField]
		protected RectTransform[] checkFakeHideRects;

		// Token: 0x04002D1D RID: 11549
		[SerializeField]
		protected RectTransform[] checkExitRects;

		// Token: 0x04002D1E RID: 11550
		[SerializeField]
		protected RectTransform fakeHideMark;

		// Token: 0x04002D1F RID: 11551
		public UIElement ParentElement;

		// Token: 0x04002D20 RID: 11552
		protected static int _fakeHideMenuCount;

		// Token: 0x04002D21 RID: 11553
		protected bool _isInFakeHideState;

		// Token: 0x04002D22 RID: 11554
		protected Sequence _fakeMaskTween = null;

		// Token: 0x04002D23 RID: 11555
		protected CommonFakeHidePanel.EFakeMaskState _fakeMaskState = CommonFakeHidePanel.EFakeMaskState.None;

		// Token: 0x04002D24 RID: 11556
		protected const float _fakeMaskHighlightAlpha = 0.4f;

		// Token: 0x04002D25 RID: 11557
		protected const float _fakeMaskNormalAlpha = 0f;

		// Token: 0x04002D26 RID: 11558
		protected const float _fakeMaskTweenDuration = 0.2f;

		// Token: 0x04002D27 RID: 11559
		protected Vector3[] _tempCorners = new Vector3[4];

		// Token: 0x020018C6 RID: 6342
		protected enum EFakeMaskState
		{
			// Token: 0x0400B007 RID: 45063
			None,
			// Token: 0x0400B008 RID: 45064
			PendingNormal,
			// Token: 0x0400B009 RID: 45065
			NormalMask,
			// Token: 0x0400B00A RID: 45066
			PendingHighlight,
			// Token: 0x0400B00B RID: 45067
			HighlightMask
		}
	}
}
