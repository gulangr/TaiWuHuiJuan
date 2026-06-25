using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Common
{
	// Token: 0x02000F9D RID: 3997
	public class CommonSecondToggleContentRefreshAnim : MonoBehaviour
	{
		// Token: 0x0600B7AF RID: 47023 RVA: 0x0053B07F File Offset: 0x0053927F
		private void Awake()
		{
			this._toggleGroup = base.transform.GetComponent<CToggleGroup>();
			this._toggleGroup.OnActiveIndexChange += this.OnContentRefresh;
			this._subChangeOver = true;
		}

		// Token: 0x0600B7B0 RID: 47024 RVA: 0x0053B0B4 File Offset: 0x005392B4
		private void OnDestroy()
		{
			this._toggleGroup.OnActiveIndexChange -= this.OnContentRefresh;
			bool flag = this._dropdown != null;
			if (flag)
			{
				this._dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnDropdownIndexChange));
			}
		}

		// Token: 0x0600B7B1 RID: 47025 RVA: 0x0053B108 File Offset: 0x00539308
		public void CallAnim()
		{
			bool flag = this.isPlayCopyAnim;
			if (flag)
			{
				this.PlaySpecialAnim();
			}
			else
			{
				this.PlayAnim();
			}
		}

		// Token: 0x0600B7B2 RID: 47026 RVA: 0x0053B134 File Offset: 0x00539334
		public void SetWaitCallParam(List<RectTransform> contentList, CDropdown dropdown = null)
		{
			this._isWaitCall = true;
			this._currentContentIndex = 0;
			this._contentList = contentList;
			this._dropdown = dropdown;
			bool flag = this._dropdown == null;
			if (!flag)
			{
				this._dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownIndexChange));
				bool flag2 = this._dropdown.options.Count != this._contentList.Count;
				if (flag2)
				{
					Debug.LogError("DropdownOptions.Count is not same to ContentList's Count");
				}
			}
		}

		// Token: 0x0600B7B3 RID: 47027 RVA: 0x0053B1C0 File Offset: 0x005393C0
		private void OnContentRefresh(int newIndex, int oldIndex)
		{
			bool flag = oldIndex < 0 || newIndex == oldIndex;
			if (!flag)
			{
				this._isRightMove = (newIndex < oldIndex);
				bool isWaitCall = this._isWaitCall;
				if (!isWaitCall)
				{
					bool flag2 = this.isPlayCopyAnim;
					if (flag2)
					{
						this.PlaySpecialAnim();
					}
					else
					{
						this.PlayAnim();
					}
				}
			}
		}

		// Token: 0x0600B7B4 RID: 47028 RVA: 0x0053B214 File Offset: 0x00539414
		private void PlayAnim()
		{
			Sequence sequence = this._sequence;
			if (sequence != null)
			{
				sequence.Pause<Sequence>();
			}
			Sequence sequence2 = this._sequence;
			if (sequence2 != null)
			{
				sequence2.Kill(true);
			}
			this._sequence = null;
			bool flag = !this._subChangeOver;
			if (flag)
			{
				this.ExecuteLastAnim();
			}
			else
			{
				bool isWaitCall = this._isWaitCall;
				RectTransform targetContent;
				if (isWaitCall)
				{
					targetContent = this._contentList[this._currentContentIndex];
				}
				else
				{
					targetContent = this.content;
				}
				CanvasGroup inCg = targetContent.gameObject.GetOrAddComponent<CanvasGroup>();
				Vector2 inEndPos = new Vector2(targetContent.anchoredPosition.x, 0f);
				Vector2 inStartPos = inEndPos + Vector2.right * (this._isRightMove ? (-1f * this.moveDis) : this.moveDis);
				this._lastInAnimInfo = new SecondToggleSubAnimInfo
				{
					Cg = inCg,
					RectTs = targetContent,
					StartAnchorPos = inStartPos,
					EndAnchorPos = inEndPos,
					MoveDuration = this.moveDuration,
					StartAlpha = this.startAlpha,
					EndAlpha = this.endAlpha,
					FadeDuration = this.fadeDuration,
					StartScale = this.startScale,
					EndScale = this.endScale,
					ScaleDuration = this.scaleDuration
				};
				this._sequence = ToggleGroupAnimUtility.SecondToggleContentRefreshAnim(this._lastInAnimInfo, delegate()
				{
					targetContent.anchoredPosition = inEndPos;
					this._subChangeOver = true;
				});
				this._sequence.SetUpdate(true);
				this._sequence.Restart(true, -1f);
			}
		}

		// Token: 0x0600B7B5 RID: 47029 RVA: 0x0053B3CC File Offset: 0x005395CC
		private void PlaySpecialAnim()
		{
			Sequence sequence = this._sequence;
			if (sequence != null)
			{
				sequence.Pause<Sequence>();
			}
			Sequence sequence2 = this._sequence;
			if (sequence2 != null)
			{
				sequence2.Kill(true);
			}
			this._sequence = null;
			bool flag = !this._subChangeOver;
			if (flag)
			{
				this.ExecuteLastAnim();
			}
			else
			{
				bool isWaitCall = this._isWaitCall;
				RectTransform targetContent;
				if (isWaitCall)
				{
					targetContent = this._contentList[this._currentContentIndex];
				}
				else
				{
					targetContent = this.content;
				}
				bool flag2 = this._copyContent != null;
				if (flag2)
				{
					Object.Destroy(this._copyContent);
				}
				this._copyContent = Object.Instantiate<RectTransform>(targetContent, targetContent.transform.parent);
				this._copyContent.transform.SetAsLastSibling();
				CanvasGroup outCg = this._copyContent.gameObject.GetOrAddComponent<CanvasGroup>();
				RectTransform outRectTs = this._copyContent.GetComponent<RectTransform>();
				Vector2 outStartPos = outRectTs.anchoredPosition;
				Vector2 outEndPos = outStartPos + Vector2.right * (this._isRightMove ? this.moveDis : (-1f * this.moveDis));
				this._lastOutAnimInfo = new SecondToggleSubAnimInfo
				{
					Cg = outCg,
					RectTs = this._copyContent,
					StartAnchorPos = outStartPos,
					EndAnchorPos = outEndPos,
					MoveDuration = this.moveDuration,
					StartAlpha = this.endAlpha,
					EndAlpha = this.startAlpha,
					FadeDuration = this.fadeDuration,
					StartScale = this.startScale,
					EndScale = this.endScale,
					ScaleDuration = this.scaleDuration
				};
				CanvasGroup inCg = targetContent.gameObject.GetOrAddComponent<CanvasGroup>();
				Vector2 inEndPos = new Vector2(targetContent.anchoredPosition.x, 0f);
				Vector2 inStartPos = inEndPos + Vector2.right * (this._isRightMove ? (-1f * this.moveDis) : this.moveDis);
				this._lastInAnimInfo = new SecondToggleSubAnimInfo
				{
					Cg = inCg,
					RectTs = targetContent,
					StartAnchorPos = inStartPos,
					EndAnchorPos = inEndPos,
					MoveDuration = this.moveDuration,
					StartAlpha = this.startAlpha,
					EndAlpha = this.endAlpha,
					FadeDuration = this.fadeDuration,
					StartScale = this.startScale,
					EndScale = this.endScale,
					ScaleDuration = this.scaleDuration
				};
				this._sequence = ToggleGroupAnimUtility.SecondToggleContentRefreshAnim(this._lastOutAnimInfo, this._lastInAnimInfo, delegate()
				{
					targetContent.anchoredPosition = inEndPos;
					Object.Destroy(this._copyContent.gameObject);
					this._copyContent = null;
					this._subChangeOver = true;
				});
				this._sequence.SetUpdate(true);
				this._sequence.Restart(true, -1f);
			}
		}

		// Token: 0x0600B7B6 RID: 47030 RVA: 0x0053B6B4 File Offset: 0x005398B4
		private void OnDropdownIndexChange(int index)
		{
			bool flag = index >= this._contentList.Count;
			if (flag)
			{
				Debug.LogError("Dropdown Index Is Over ContentList's Count !");
				this._currentContentIndex = 0;
			}
			else
			{
				this._currentContentIndex = index;
			}
		}

		// Token: 0x0600B7B7 RID: 47031 RVA: 0x0053B6F4 File Offset: 0x005398F4
		private void ExecuteLastAnim()
		{
			bool flag = this._lastInAnimInfo != null;
			if (flag)
			{
				this._lastInAnimInfo.RectTs.anchoredPosition = this._lastInAnimInfo.EndAnchorPos;
				this._lastInAnimInfo.Cg.alpha = 1f;
				this._lastInAnimInfo.RectTs.gameObject.SetActive(true);
			}
			bool flag2 = this._copyContent != null;
			if (flag2)
			{
				Object.Destroy(this._copyContent.gameObject);
			}
			this._copyContent = null;
			this._lastOutAnimInfo = null;
			this._lastInAnimInfo = null;
			this._subChangeOver = true;
		}

		// Token: 0x04008EA4 RID: 36516
		[SerializeField]
		private RectTransform content;

		// Token: 0x04008EA5 RID: 36517
		[SerializeField]
		private bool isPlayCopyAnim = false;

		// Token: 0x04008EA6 RID: 36518
		[SerializeField]
		private float startAlpha = 0f;

		// Token: 0x04008EA7 RID: 36519
		[SerializeField]
		private float endAlpha = 1f;

		// Token: 0x04008EA8 RID: 36520
		[SerializeField]
		private float fadeDuration = 0.13f;

		// Token: 0x04008EA9 RID: 36521
		[SerializeField]
		private float moveDis = 200f;

		// Token: 0x04008EAA RID: 36522
		[SerializeField]
		private float moveDuration = 0.13f;

		// Token: 0x04008EAB RID: 36523
		[SerializeField]
		private float startScale = 1f;

		// Token: 0x04008EAC RID: 36524
		[SerializeField]
		private float endScale = 1f;

		// Token: 0x04008EAD RID: 36525
		[SerializeField]
		private float scaleDuration = 0f;

		// Token: 0x04008EAE RID: 36526
		private CToggleGroup _toggleGroup;

		// Token: 0x04008EAF RID: 36527
		private bool _subChangeOver;

		// Token: 0x04008EB0 RID: 36528
		private Sequence _sequence;

		// Token: 0x04008EB1 RID: 36529
		private SecondToggleSubAnimInfo _lastInAnimInfo;

		// Token: 0x04008EB2 RID: 36530
		private SecondToggleSubAnimInfo _lastOutAnimInfo;

		// Token: 0x04008EB3 RID: 36531
		private bool _isRightMove;

		// Token: 0x04008EB4 RID: 36532
		private RectTransform _copyContent;

		// Token: 0x04008EB5 RID: 36533
		private bool _isWaitCall;

		// Token: 0x04008EB6 RID: 36534
		private CDropdown _dropdown;

		// Token: 0x04008EB7 RID: 36535
		private List<RectTransform> _contentList;

		// Token: 0x04008EB8 RID: 36536
		private int _currentContentIndex;
	}
}
