using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.VillagerRoleView
{
	// Token: 0x02000735 RID: 1845
	public class SlideBookComponent : MonoBehaviour
	{
		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x060058A0 RID: 22688 RVA: 0x00291F8A File Offset: 0x0029018A
		private int _totalSiblingAmount
		{
			get
			{
				return this.pageIndexArr.Length + this.pageContentArr.Length;
			}
		}

		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x060058A1 RID: 22689 RVA: 0x00291F9D File Offset: 0x0029019D
		public int CurrentAvtiveIndex
		{
			get
			{
				return this._currentIndex;
			}
		}

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x060058A2 RID: 22690 RVA: 0x00291FA5 File Offset: 0x002901A5
		public bool IsAnimating
		{
			get
			{
				return this.seq != null && this.seq.IsActive() && !this.seq.IsComplete();
			}
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x00291FCD File Offset: 0x002901CD
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x060058A4 RID: 22692 RVA: 0x00291FD8 File Offset: 0x002901D8
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this.seq = DOTween.Sequence();
				this.subpageToggleGroup.Init(-1);
				this.subpageToggleGroup.OnActiveIndexChange += this.OnToggleChange;
				this._siblingOrders = new RectTransform[this.pageIndexArr.Length + this.pageContentArr.Length];
				this._indexOffsetArr = new Vector3[this.pageIndexArr.Length];
				this._contentOffsetArr = new Vector3[this.pageContentArr.Length];
				bool flag = this._contentDarkenMask == null;
				if (flag)
				{
					this._contentDarkenMask = new CImage[this.pageContentArr.Length];
				}
				for (int i = 0; i < this.pageIndexArr.Length; i++)
				{
					this._indexOffsetArr[i] = this.pageIndexArr[i].transform.position - this._leftSign.position;
					this._contentOffsetArr[i] = this.pageContentArr[i].transform.position - this._leftSign.position;
					bool flag2 = this._contentDarkenMask[i] == null;
					if (flag2)
					{
						this._contentDarkenMask[i] = this.pageContentArr[i].gameObject.GetComponent<CImage>();
					}
				}
				bool flag3 = this._mainMask != null;
				if (flag3)
				{
					this._mainMaskGraphic = this._mainMask.GetComponent<MaskableGraphic>();
				}
			}
		}

		// Token: 0x060058A5 RID: 22693 RVA: 0x00292161 File Offset: 0x00290361
		private void OnToggleChange(int newTog, int oldTog)
		{
			this.SetDisplayPage(newTog, true);
		}

		// Token: 0x060058A6 RID: 22694 RVA: 0x00292170 File Offset: 0x00290370
		public void SetDisplayPage(int index, bool animated = true)
		{
			bool flag = animated && this._currentIndex != index;
			if (flag)
			{
				bool isAnimating = this.IsAnimating;
				if (isAnimating)
				{
					this.seq.Kill(true);
				}
				this.seq = DOTween.Sequence();
				this.MaskActive(true);
				this.ArrangeSiblingAnimate(index);
				this.MovePages(index);
			}
			else
			{
				this.ArrangeSiblingFinal(index);
			}
			bool flag2 = this.subpageToggleGroup.GetActiveIndex() != index;
			if (flag2)
			{
				this.subpageToggleGroup.SetWithoutNotify(index);
			}
		}

		// Token: 0x060058A7 RID: 22695 RVA: 0x00292204 File Offset: 0x00290404
		private void MaskActive(bool active)
		{
			bool flag = this._mainMask != null;
			if (flag)
			{
				this._mainMaskGraphic.enabled = active;
				this._mainMask.enabled = active;
			}
		}

		// Token: 0x060058A8 RID: 22696 RVA: 0x00292240 File Offset: 0x00290440
		private void MovePages(int index)
		{
			this._triggerOnComplete = true;
			this.seq.Pause<Sequence>();
			bool flag = this._currentIndex > index;
			if (flag)
			{
				for (int i = this._currentIndex; i > index; i--)
				{
					this.MoveToTween(i, false, this.seq);
				}
			}
			else
			{
				bool flag2 = this._currentIndex < index;
				if (flag2)
				{
					for (int j = index; j > this._currentIndex; j--)
					{
						this.MoveToTween(j, true, this.seq);
					}
				}
			}
			this._contentDarkenMask[index].SetAlpha(0f);
			this._contentDarkenMask[this._currentIndex].SetAlpha(0f);
			this.seq.Join(this._contentDarkenMask[this._currentIndex].DOFade(this.DarkenAlpha, this.MoveDuration));
			this.seq.OnComplete(delegate
			{
				int tempCurrent = this._currentIndex;
				this.ArrangeSiblingFinal(index);
				bool triggerOnComplete = this._triggerOnComplete;
				if (triggerOnComplete)
				{
					Action<int, int> onPageSwitchComplete = this.OnPageSwitchComplete;
					if (onPageSwitchComplete != null)
					{
						onPageSwitchComplete(tempCurrent, this._currentIndex);
					}
				}
			});
			this.seq.Play<Sequence>();
		}

		// Token: 0x060058A9 RID: 22697 RVA: 0x0029237C File Offset: 0x0029057C
		private void MoveToTween(int index, bool isLeft, Sequence seq)
		{
			Vector3 targetPositionIndex = this.GetPosition(index, isLeft, true);
			Vector3 targetPositionContent = this.GetPosition(index, isLeft, false);
			seq.Join(this.pageIndexArr[index].DOMove(targetPositionIndex, this.MoveDuration, false).SetEase(SlideBookComponent.AccelerateCurve));
			seq.Join(this.pageContentArr[index].DOMove(targetPositionContent, this.MoveDuration, false).SetEase(SlideBookComponent.AccelerateCurve));
		}

		// Token: 0x060058AA RID: 22698 RVA: 0x002923EC File Offset: 0x002905EC
		private Vector3 GetPosition(int index, bool isLeft, bool isIndex)
		{
			Vector3[] offsetArr = isIndex ? this._indexOffsetArr : this._contentOffsetArr;
			Transform sign = isLeft ? this._leftSign : this._rightSign;
			return sign.position + offsetArr[index];
		}

		// Token: 0x060058AB RID: 22699 RVA: 0x00292434 File Offset: 0x00290634
		private void ArrangeSiblingFinal(int activePageIndex)
		{
			this.ResetArr(this._siblingOrders);
			for (int i = 0; i < this.pageIndexArr.Length; i++)
			{
				this.pageContentArr[i].gameObject.SetActive(activePageIndex == i);
				this._contentDarkenMask[i].SetAlpha(0f);
				bool flag = i < activePageIndex;
				if (flag)
				{
					this._siblingOrders[this._totalSiblingAmount - i - 1] = this.pageIndexArr[i];
					this.pageIndexArr[i].transform.position = this._indexOffsetArr[i] + this._leftSign.position;
					this.pageContentArr[i].transform.position = this._contentOffsetArr[i] + this._leftSign.position;
				}
				else
				{
					bool flag2 = i == activePageIndex;
					if (flag2)
					{
						this._siblingOrders[this._totalSiblingAmount - i - 1] = this.pageIndexArr[i];
						this.pageIndexArr[i].transform.position = this._indexOffsetArr[i] + this._leftSign.position;
						this._siblingOrders[this._totalSiblingAmount - i - 2] = this.pageContentArr[i];
						this.pageContentArr[i].transform.position = this._contentOffsetArr[i] + this._leftSign.position;
					}
					else
					{
						this._siblingOrders[this._totalSiblingAmount - i - 2] = this.pageIndexArr[i];
						this.pageIndexArr[i].transform.position = this._indexOffsetArr[i] + this._rightSign.position;
						this.pageContentArr[i].transform.position = this._contentOffsetArr[i] + this._rightSign.position;
					}
				}
			}
			this.MaskActive(false);
			this.SetSiblingOrder(this._siblingOrders);
			this._currentIndex = activePageIndex;
		}

		// Token: 0x060058AC RID: 22700 RVA: 0x00292654 File Offset: 0x00290854
		private void SetSiblingOrder(RectTransform[] siblingOrders)
		{
			for (int i = 0; i < siblingOrders.Length; i++)
			{
				bool flag = siblingOrders[i] == null;
				if (!flag)
				{
					siblingOrders[i].SetSiblingIndex(i);
				}
			}
		}

		// Token: 0x060058AD RID: 22701 RVA: 0x00292690 File Offset: 0x00290890
		private void ResetArr(RectTransform[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = null;
			}
		}

		// Token: 0x060058AE RID: 22702 RVA: 0x002926B8 File Offset: 0x002908B8
		private void ArrangeSiblingAnimate(int activePageIndex)
		{
			for (int i = 0; i < this.pageIndexArr.Length; i++)
			{
				this.pageContentArr[i].gameObject.SetActive(activePageIndex == i || this._currentIndex == i);
				this._siblingOrders[this._totalSiblingAmount - i - 1] = this.pageIndexArr[i];
			}
			bool flag = this._currentIndex > activePageIndex;
			if (flag)
			{
				this._siblingOrders[this.pageContentArr.Length - 2] = this.pageContentArr[activePageIndex];
				this._siblingOrders[this.pageContentArr.Length - 1] = this.pageContentArr[this._currentIndex];
			}
			bool flag2 = this._currentIndex < activePageIndex;
			if (flag2)
			{
				this._siblingOrders[this.pageContentArr.Length - 1] = this.pageContentArr[activePageIndex];
				this._siblingOrders[this.pageContentArr.Length - 2] = this.pageContentArr[this._currentIndex];
			}
			this.SetSiblingOrder(this._siblingOrders);
		}

		// Token: 0x060058AF RID: 22703 RVA: 0x002927B4 File Offset: 0x002909B4
		public void StopAnimation(bool triggerOnComplete)
		{
			this._triggerOnComplete = triggerOnComplete;
			bool isAnimating = this.IsAnimating;
			if (isAnimating)
			{
				this.seq.Kill(triggerOnComplete);
			}
		}

		// Token: 0x060058B0 RID: 22704 RVA: 0x002927E2 File Offset: 0x002909E2
		public void SetToggleInteractable(int index, bool interactable)
		{
			this.subpageToggleGroup.Get(index).interactable = interactable;
		}

		// Token: 0x04003D0C RID: 15628
		[SerializeField]
		private RectTransform[] pageIndexArr;

		// Token: 0x04003D0D RID: 15629
		[SerializeField]
		private RectTransform[] pageContentArr;

		// Token: 0x04003D0E RID: 15630
		[SerializeField]
		private CToggleGroup subpageToggleGroup;

		// Token: 0x04003D0F RID: 15631
		[SerializeField]
		private CImage[] _contentDarkenMask;

		// Token: 0x04003D10 RID: 15632
		[SerializeField]
		private Transform _leftSign;

		// Token: 0x04003D11 RID: 15633
		[SerializeField]
		private Transform _rightSign;

		// Token: 0x04003D12 RID: 15634
		[SerializeField]
		private Mask _mainMask;

		// Token: 0x04003D13 RID: 15635
		private Graphic _mainMaskGraphic;

		// Token: 0x04003D14 RID: 15636
		public Action<int, int> OnPageSwitchComplete;

		// Token: 0x04003D15 RID: 15637
		private Vector3[] _indexOffsetArr;

		// Token: 0x04003D16 RID: 15638
		private Vector3[] _contentOffsetArr;

		// Token: 0x04003D17 RID: 15639
		public float MoveDuration = 0.33f;

		// Token: 0x04003D18 RID: 15640
		public float DarkenAlpha = 0.7f;

		// Token: 0x04003D19 RID: 15641
		private Sequence seq;

		// Token: 0x04003D1A RID: 15642
		private static readonly AnimationCurve AccelerateCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f, 1f, 1f),
			new Keyframe(0.33f, 1f, 3f, 3f)
		});

		// Token: 0x04003D1B RID: 15643
		private int _currentIndex;

		// Token: 0x04003D1C RID: 15644
		private bool _inited = false;

		// Token: 0x04003D1D RID: 15645
		private RectTransform[] _siblingOrders;

		// Token: 0x04003D1E RID: 15646
		private bool _triggerOnComplete;
	}
}
