using System;
using DG.Tweening;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F9E RID: 3998
	public class CommonSecondToggleSubChangeAnim : MonoBehaviour
	{
		// Token: 0x0600B7B9 RID: 47033 RVA: 0x0053B80F File Offset: 0x00539A0F
		private void Awake()
		{
			this._toggleGroup = base.transform.GetComponent<CToggleGroup>();
			this._toggleGroup.OnActiveIndexChange += this.OnSubChange;
			this._subChangeOver = true;
		}

		// Token: 0x0600B7BA RID: 47034 RVA: 0x0053B842 File Offset: 0x00539A42
		private void OnDestroy()
		{
			this._toggleGroup.OnActiveIndexChange -= this.OnSubChange;
		}

		// Token: 0x0600B7BB RID: 47035 RVA: 0x0053B860 File Offset: 0x00539A60
		public void ResetSubState()
		{
			bool flag = this._seq != null;
			if (flag)
			{
				this._seq.Pause<Sequence>();
				this._seq.Kill(false);
				this._seq = null;
			}
			this._subChangeOver = true;
			this._lastOutInfo = null;
			this._lastInInfo = null;
			for (int i = 0; i < this.toggleSubArray.Length; i++)
			{
				GameObject obj = this.toggleSubArray[i];
				bool flag2 = obj == null;
				if (!flag2)
				{
					CanvasGroup cg = obj.GetOrAddComponent<CanvasGroup>();
					cg.alpha = 1f;
					obj.GetComponent<RectTransform>().localScale = Vector3.one;
					obj.SetActive(i == 0);
				}
			}
		}

		// Token: 0x0600B7BC RID: 47036 RVA: 0x0053B914 File Offset: 0x00539B14
		private void OnSubChange(int newIndex, int oldIndex)
		{
			bool flag = oldIndex < 0 || newIndex == oldIndex;
			if (!flag)
			{
				GameObject outObj = this.GetTargetGameObj(oldIndex);
				GameObject intObj = this.GetTargetGameObj(newIndex);
				bool flag2 = outObj == null || intObj == null;
				if (!flag2)
				{
					bool flag3 = this._seq != null;
					if (flag3)
					{
						this._seq.Pause<Sequence>();
						this._seq.Kill(false);
						this._seq = null;
					}
					bool flag4 = !this._subChangeOver;
					if (flag4)
					{
						this.ExecuteLastAnim();
						outObj.GetOrAddComponent<CanvasGroup>().alpha = 1f;
						outObj.SetActive(false);
						intObj.GetOrAddComponent<CanvasGroup>().alpha = 1f;
						intObj.SetActive(true);
						this.ExecuteAnimEndAdjustInfo();
						InfinityScroll componentInChildren = intObj.GetComponentInChildren<InfinityScroll>();
						if (componentInChildren != null)
						{
							componentInChildren.RefreshDisplayRange();
						}
					}
					else
					{
						this._subChangeOver = false;
						bool isRightMove = newIndex < oldIndex;
						CanvasGroup outCg = outObj.GetOrAddComponent<CanvasGroup>();
						RectTransform outRectTs = outObj.GetComponent<RectTransform>();
						Vector2 outStartPos = outRectTs.anchoredPosition;
						Vector2 outEndPos = outStartPos + Vector2.right * (isRightMove ? this.outInfo.moveDis : (-1f * this.outInfo.moveDis));
						CanvasGroup inCg = intObj.GetOrAddComponent<CanvasGroup>();
						RectTransform inRectTs = intObj.GetComponent<RectTransform>();
						Vector2 inEndPos = inRectTs.anchoredPosition;
						Vector2 inStartPos = inEndPos + Vector2.right * (isRightMove ? (-1f * this.inInfo.moveDis) : this.inInfo.moveDis);
						this._lastOutInfo = new SecondToggleSubAnimInfo
						{
							Cg = outCg,
							RectTs = outRectTs,
							StartAnchorPos = outStartPos,
							EndAnchorPos = outEndPos,
							MoveDuration = this.outInfo.moveDuration,
							StartAlpha = this.outInfo.startAlpha,
							EndAlpha = this.outInfo.endAlpha,
							FadeDuration = this.outInfo.fadeDuration,
							StartScale = this.outInfo.startScale,
							EndScale = this.outInfo.endScale,
							ScaleDuration = this.outInfo.scaleDuration
						};
						this._lastInInfo = new SecondToggleSubAnimInfo
						{
							Cg = inCg,
							RectTs = inRectTs,
							StartAnchorPos = inStartPos,
							EndAnchorPos = inEndPos,
							MoveDuration = this.inInfo.moveDuration,
							StartAlpha = this.inInfo.startAlpha,
							EndAlpha = this.inInfo.endAlpha,
							FadeDuration = this.inInfo.fadeDuration,
							StartScale = this.inInfo.startScale,
							EndScale = this.inInfo.endScale,
							ScaleDuration = this.inInfo.scaleDuration
						};
						this._seq = ToggleGroupAnimUtility.SecondToggleSubChangeAnim(this._lastOutInfo, this._lastInInfo, delegate
						{
							inRectTs.anchoredPosition = inEndPos;
							outRectTs.anchoredPosition = outStartPos;
							this.ExecuteAnimEndAdjustInfo();
							InfinityScroll componentInChildren2 = intObj.GetComponentInChildren<InfinityScroll>();
							if (componentInChildren2 != null)
							{
								componentInChildren2.RefreshDisplayRange();
							}
							this._subChangeOver = true;
						});
						this._seq.SetUpdate(true);
						this._seq.Restart(true, -1f);
					}
				}
			}
		}

		// Token: 0x0600B7BD RID: 47037 RVA: 0x0053BC88 File Offset: 0x00539E88
		private GameObject GetTargetGameObj(int index)
		{
			bool flag = index >= this.toggleSubArray.Length;
			GameObject result;
			if (flag)
			{
				Debug.LogError(string.Format("SubArray is not enough ! Array.Len:{0} Index:{1}", this.toggleSubArray.Length, index));
				result = null;
			}
			else
			{
				result = this.toggleSubArray[index];
			}
			return result;
		}

		// Token: 0x0600B7BE RID: 47038 RVA: 0x0053BCDC File Offset: 0x00539EDC
		private void ExecuteLastAnim()
		{
			bool flag = this._lastInInfo != null;
			if (flag)
			{
				this._lastInInfo.RectTs.anchoredPosition = this._lastInInfo.EndAnchorPos;
				this._lastInInfo.Cg.alpha = 1f;
				this._lastInInfo.RectTs.gameObject.SetActive(false);
			}
			bool flag2 = this._lastOutInfo != null;
			if (flag2)
			{
				this._lastOutInfo.RectTs.anchoredPosition = this._lastOutInfo.StartAnchorPos;
				this._lastOutInfo.Cg.alpha = 1f;
				this._lastOutInfo.RectTs.gameObject.SetActive(false);
			}
			this._lastInInfo = null;
			this._lastOutInfo = null;
			this._subChangeOver = true;
		}

		// Token: 0x0600B7BF RID: 47039 RVA: 0x0053BDB0 File Offset: 0x00539FB0
		private void ExecuteAnimEndAdjustInfo()
		{
			bool flag = this.onAnimEndAdjustPosInfo == null || this.onAnimEndAdjustPosInfo.Length == 0;
			if (!flag)
			{
				for (int i = 0; i < this.onAnimEndAdjustPosInfo.Length; i++)
				{
					CommonSecondToggleAnimAdjustPosInfo info = this.onAnimEndAdjustPosInfo[i];
					info.adjustRoot.anchoredPosition = info.adjustPos;
				}
			}
		}

		// Token: 0x04008EB9 RID: 36537
		[SerializeField]
		[Tooltip("Sub顺序要与ToggleGroup的ToggleList元素对应")]
		private GameObject[] toggleSubArray;

		// Token: 0x04008EBA RID: 36538
		[SerializeField]
		private CommonSecondToggleSubChangeAnimInfo outInfo;

		// Token: 0x04008EBB RID: 36539
		[SerializeField]
		private CommonSecondToggleSubChangeAnimInfo inInfo;

		// Token: 0x04008EBC RID: 36540
		[SerializeField]
		private CommonSecondToggleAnimAdjustPosInfo[] onAnimEndAdjustPosInfo;

		// Token: 0x04008EBD RID: 36541
		private CToggleGroup _toggleGroup;

		// Token: 0x04008EBE RID: 36542
		private bool _subChangeOver;

		// Token: 0x04008EBF RID: 36543
		private Sequence _seq;

		// Token: 0x04008EC0 RID: 36544
		private SecondToggleSubAnimInfo _lastOutInfo;

		// Token: 0x04008EC1 RID: 36545
		private SecondToggleSubAnimInfo _lastInInfo;
	}
}
