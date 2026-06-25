using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001029 RID: 4137
	public class UIHorizontalLayoutSwitcher : MonoBehaviour
	{
		// Token: 0x0600BD2E RID: 48430 RVA: 0x0055F4E0 File Offset: 0x0055D6E0
		private static AnimationCurve CreateOutQuartCurve()
		{
			AnimationCurve curve = new AnimationCurve();
			curve.AddKey(new Keyframe(0f, 0f, 0f, 1.5f));
			curve.AddKey(new Keyframe(0.25f, 0.68f));
			curve.AddKey(new Keyframe(0.5f, 0.9375f));
			curve.AddKey(new Keyframe(0.75f, 0.996f));
			curve.AddKey(new Keyframe(1f, 1f, 0.25f, 0f));
			for (int i = 0; i < curve.keys.Length; i++)
			{
				curve.SmoothTangents(i, 0f);
			}
			return curve;
		}

		// Token: 0x0600BD2F RID: 48431 RVA: 0x0055F5A1 File Offset: 0x0055D7A1
		[ContextMenu("Apply OutQuart Curve")]
		public void ApplyOutQuartCurve()
		{
			this.animationCurve = UIHorizontalLayoutSwitcher.CreateOutQuartCurve();
		}

		// Token: 0x0600BD30 RID: 48432 RVA: 0x0055F5B0 File Offset: 0x0055D7B0
		private void Start()
		{
			this.ValidateStates();
			this.ApplyCurrentState(false, null, null, null);
		}

		// Token: 0x0600BD31 RID: 48433 RVA: 0x0055F5D8 File Offset: 0x0055D7D8
		private void Update()
		{
			bool isAnimating = this._isAnimating;
			if (isAnimating)
			{
				this.UpdateAnimation();
			}
		}

		// Token: 0x0600BD32 RID: 48434 RVA: 0x0055F5FC File Offset: 0x0055D7FC
		private void ValidateStates()
		{
			int elementCount = this.targetElements.Count;
			while (this.stateA.elementStates.Count < elementCount)
			{
				this.stateA.elementStates.Add(new UIHorizontalLayoutSwitcher.ElementState());
			}
			while (this.stateB.elementStates.Count < elementCount)
			{
				this.stateB.elementStates.Add(new UIHorizontalLayoutSwitcher.ElementState());
			}
		}

		// Token: 0x0600BD33 RID: 48435 RVA: 0x0055F678 File Offset: 0x0055D878
		public void SwitchToStateA(bool? withAnimation = null, Action onComplete = null, Action<float> onProgress = null)
		{
			bool flag = !this.isStateB;
			if (!flag)
			{
				this.isStateB = false;
				this.ApplyCurrentState(false, withAnimation, onComplete, onProgress);
			}
		}

		// Token: 0x0600BD34 RID: 48436 RVA: 0x0055F6A8 File Offset: 0x0055D8A8
		public void SwitchToStateB(bool? withAnimation = null, Action onComplete = null, Action<float> onProgress = null)
		{
			bool flag = this.isStateB;
			if (!flag)
			{
				this.isStateB = true;
				this.ApplyCurrentState(false, withAnimation, onComplete, onProgress);
			}
		}

		// Token: 0x0600BD35 RID: 48437 RVA: 0x0055F6D4 File Offset: 0x0055D8D4
		public void ToggleState(bool? withAnimation = null, Action onComplete = null, Action<float> onProgress = null)
		{
			this.isStateB = !this.isStateB;
			this.ApplyCurrentState(false, withAnimation, onComplete, onProgress);
		}

		// Token: 0x0600BD36 RID: 48438 RVA: 0x0055F6F4 File Offset: 0x0055D8F4
		public void ApplyCurrentState(bool forceAnimation = false, bool? withAnimation = null, Action onComplete = null, Action<float> onProgress = null)
		{
			bool flag = this.targetElements.Count == 0;
			if (!flag)
			{
				this.ValidateStates();
				UIHorizontalLayoutSwitcher.LayoutState currentState = this.isStateB ? this.stateB : this.stateA;
				bool shouldUseAnimation = withAnimation ?? this.useAnimation;
				bool flag2 = shouldUseAnimation && (Application.isPlaying || forceAnimation);
				if (flag2)
				{
					this.StartAnimation(forceAnimation, onComplete, onProgress);
				}
				else
				{
					this.ApplyStateImmediate(currentState);
					if (onComplete != null)
					{
						onComplete();
					}
				}
			}
		}

		// Token: 0x0600BD37 RID: 48439 RVA: 0x0055F788 File Offset: 0x0055D988
		private void ApplyStateImmediate(UIHorizontalLayoutSwitcher.LayoutState state)
		{
			int i = 0;
			while (i < this.targetElements.Count && i < state.elementStates.Count)
			{
				RectTransform element = this.targetElements[i];
				UIHorizontalLayoutSwitcher.ElementState elementState = state.elementStates[i];
				bool flag = element == null;
				if (!flag)
				{
					Vector2 anchoredPos = element.anchoredPosition;
					anchoredPos.x = elementState.positionX;
					element.anchoredPosition = anchoredPos;
					Vector2 sizeDelta = element.sizeDelta;
					sizeDelta.x = elementState.width;
					element.sizeDelta = sizeDelta;
					bool flag2 = elementState.visibilityMode != VisibilityMode.DontControl;
					if (flag2)
					{
						element.gameObject.SetActive(elementState.visibilityMode == VisibilityMode.Show);
					}
				}
				i++;
			}
		}

		// Token: 0x0600BD38 RID: 48440 RVA: 0x0055F858 File Offset: 0x0055DA58
		private void StartAnimation(bool isEditorMode = false, Action onComplete = null, Action<float> onProgress = null)
		{
			this._currentAnimationCallback = onComplete;
			this._currentAnimationProgressCallback = onProgress;
			this._startPositions.Clear();
			this._startSizes.Clear();
			foreach (RectTransform element in this.targetElements)
			{
				bool flag = element != null;
				if (flag)
				{
					this._startPositions.Add(element.anchoredPosition);
					this._startSizes.Add(element.sizeDelta);
				}
				else
				{
					this._startPositions.Add(Vector3.zero);
					this._startSizes.Add(Vector2.zero);
				}
			}
			this._isAnimating = true;
			this._animationTimer = 0f;
		}

		// Token: 0x0600BD39 RID: 48441 RVA: 0x0055F940 File Offset: 0x0055DB40
		private void UpdateAnimation()
		{
			this._animationTimer += Time.deltaTime;
			this.PerformAnimationUpdate();
		}

		// Token: 0x0600BD3A RID: 48442 RVA: 0x0055F95C File Offset: 0x0055DB5C
		private void PerformAnimationUpdate()
		{
			float progress = Mathf.Clamp01(this._animationTimer / this.animationDuration);
			float curveValue = this.animationCurve.Evaluate(progress);
			UIHorizontalLayoutSwitcher.LayoutState currentState = this.isStateB ? this.stateB : this.stateA;
			float aToBProgress = this.isStateB ? progress : (1f - progress);
			Action<float> currentAnimationProgressCallback = this._currentAnimationProgressCallback;
			if (currentAnimationProgressCallback != null)
			{
				currentAnimationProgressCallback(aToBProgress);
			}
			int i = 0;
			while (i < this.targetElements.Count && i < currentState.elementStates.Count)
			{
				RectTransform element = this.targetElements[i];
				UIHorizontalLayoutSwitcher.ElementState elementState = currentState.elementStates[i];
				bool flag = element == null;
				if (!flag)
				{
					Vector3 startPos = this._startPositions[i];
					Vector3 targetPos = new Vector3(elementState.positionX, startPos.y, startPos.z);
					element.anchoredPosition = Vector3.Lerp(startPos, targetPos, curveValue);
					Vector2 startSize = this._startSizes[i];
					Vector2 targetSize = new Vector2(elementState.width, startSize.y);
					element.sizeDelta = Vector2.Lerp(startSize, targetSize, curveValue);
					bool flag2 = progress >= 0.5f && elementState.visibilityMode != VisibilityMode.DontControl;
					if (flag2)
					{
						element.gameObject.SetActive(elementState.visibilityMode == VisibilityMode.Show);
					}
				}
				i++;
			}
			bool flag3 = progress >= 1f;
			if (flag3)
			{
				this._isAnimating = false;
				this.ApplyStateImmediate(currentState);
				Action callback = this._currentAnimationCallback;
				this._currentAnimationCallback = null;
				this._currentAnimationProgressCallback = null;
				if (callback != null)
				{
					callback();
				}
			}
		}

		// Token: 0x17001553 RID: 5459
		// (get) Token: 0x0600BD3B RID: 48443 RVA: 0x0055FB27 File Offset: 0x0055DD27
		public bool IsStateB
		{
			get
			{
				return this.isStateB;
			}
		}

		// Token: 0x0600BD3C RID: 48444 RVA: 0x0055FB30 File Offset: 0x0055DD30
		public string GetCurrentStateName()
		{
			return this.isStateB ? this.stateB.stateName : this.stateA.stateName;
		}

		// Token: 0x17001554 RID: 5460
		// (get) Token: 0x0600BD3D RID: 48445 RVA: 0x0055FB62 File Offset: 0x0055DD62
		public bool IsAnimatingAToB
		{
			get
			{
				return this._isAnimating && this.isStateB;
			}
		}

		// Token: 0x17001555 RID: 5461
		// (get) Token: 0x0600BD3E RID: 48446 RVA: 0x0055FB75 File Offset: 0x0055DD75
		public bool IsAnimatingBToA
		{
			get
			{
				return this._isAnimating && !this.isStateB;
			}
		}

		// Token: 0x0400919C RID: 37276
		[Header("目标UI元素")]
		[Tooltip("需要控制横向布局的RectTransform列表")]
		public List<RectTransform> targetElements = new List<RectTransform>();

		// Token: 0x0400919D RID: 37277
		[Header("布局状态配置")]
		[Tooltip("状态A的配置")]
		public UIHorizontalLayoutSwitcher.LayoutState stateA = new UIHorizontalLayoutSwitcher.LayoutState
		{
			stateName = "State A"
		};

		// Token: 0x0400919E RID: 37278
		[Tooltip("状态B的配置")]
		public UIHorizontalLayoutSwitcher.LayoutState stateB = new UIHorizontalLayoutSwitcher.LayoutState
		{
			stateName = "State B"
		};

		// Token: 0x0400919F RID: 37279
		[Header("当前状态")]
		[SerializeField]
		private bool isStateB;

		// Token: 0x040091A0 RID: 37280
		[Header("动画设置")]
		[Tooltip("是否使用动画过渡")]
		public bool useAnimation = true;

		// Token: 0x040091A1 RID: 37281
		[Tooltip("动画持续时间")]
		public float animationDuration = 0.3f;

		// Token: 0x040091A2 RID: 37282
		[Tooltip("动画曲线")]
		public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		// Token: 0x040091A3 RID: 37283
		private bool _isAnimating;

		// Token: 0x040091A4 RID: 37284
		private float _animationTimer;

		// Token: 0x040091A5 RID: 37285
		private readonly List<Vector3> _startPositions = new List<Vector3>();

		// Token: 0x040091A6 RID: 37286
		private readonly List<Vector2> _startSizes = new List<Vector2>();

		// Token: 0x040091A7 RID: 37287
		private Action _currentAnimationCallback;

		// Token: 0x040091A8 RID: 37288
		private Action<float> _currentAnimationProgressCallback;

		// Token: 0x02002672 RID: 9842
		[Serializable]
		public class ElementState
		{
			// Token: 0x06011BEB RID: 72683 RVA: 0x00688AA3 File Offset: 0x00686CA3
			public ElementState()
			{
				this.positionX = 0f;
				this.width = 100f;
				this.visibilityMode = VisibilityMode.DontControl;
			}

			// Token: 0x06011BEC RID: 72684 RVA: 0x00688ACA File Offset: 0x00686CCA
			public ElementState(float x, float w, VisibilityMode visibility)
			{
				this.positionX = x;
				this.width = w;
				this.visibilityMode = visibility;
			}

			// Token: 0x0400EABA RID: 60090
			[Tooltip("X位置")]
			public float positionX;

			// Token: 0x0400EABB RID: 60091
			[Tooltip("宽度")]
			public float width;

			// Token: 0x0400EABC RID: 60092
			[Tooltip("可见性控制模式")]
			public VisibilityMode visibilityMode;
		}

		// Token: 0x02002673 RID: 9843
		[Serializable]
		public class LayoutState
		{
			// Token: 0x0400EABD RID: 60093
			[Tooltip("布局状态名称")]
			public string stateName = "State";

			// Token: 0x0400EABE RID: 60094
			[Tooltip("各元素的状态配置")]
			public List<UIHorizontalLayoutSwitcher.ElementState> elementStates = new List<UIHorizontalLayoutSwitcher.ElementState>();
		}
	}
}
