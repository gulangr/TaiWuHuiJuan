using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FEF RID: 4079
	[RequireComponent(typeof(RectTransform))]
	public class LanguageRuleExpandOnHover : PointerTrigger, ILanguage
	{
		// Token: 0x0600BA1D RID: 47645 RVA: 0x0054C284 File Offset: 0x0054A484
		protected override void OnDisable()
		{
			bool isExpanded = this._isExpanded;
			if (isExpanded)
			{
				this.RestoreOriginalState();
			}
			this._isHovering = false;
			base.OnDisable();
		}

		// Token: 0x0600BA1E RID: 47646 RVA: 0x0054C2B1 File Offset: 0x0054A4B1
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			this._isWorking = (languageType == LocalStringManager.LanguageType.EN);
		}

		// Token: 0x0600BA1F RID: 47647 RVA: 0x0054C2C0 File Offset: 0x0054A4C0
		public void AddLayoutToDisable(LayoutGroup layout)
		{
			if (this.layoutsToDisable == null)
			{
				this.layoutsToDisable = new List<LayoutGroup>();
			}
			bool flag = !this.layoutsToDisable.Contains(layout);
			if (flag)
			{
				this.layoutsToDisable.Add(layout);
			}
		}

		// Token: 0x0600BA20 RID: 47648 RVA: 0x0054C304 File Offset: 0x0054A504
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			bool flag = !this._isWorking;
			if (!flag)
			{
				bool flag2 = this._isHovering || this._isExpanded;
				if (!flag2)
				{
					this._isHovering = true;
					bool flag3 = !this.NeedsExpansion();
					if (!flag3)
					{
						this.BackupOriginalState();
						this.Expand();
					}
				}
			}
		}

		// Token: 0x0600BA21 RID: 47649 RVA: 0x0054C364 File Offset: 0x0054A564
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this._isHovering = false;
			bool isExpanded = this._isExpanded;
			if (isExpanded)
			{
				this.RestoreOriginalState();
			}
		}

		// Token: 0x0600BA22 RID: 47650 RVA: 0x0054C394 File Offset: 0x0054A594
		private bool NeedsExpansion()
		{
			bool flag = this.contentContainer == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				RectTransform contentContainerRect = this.contentContainer.GetComponent<RectTransform>();
				LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainerRect);
				Vector2 preferredSize = new Vector2(LayoutUtility.GetPreferredWidth(contentContainerRect), LayoutUtility.GetPreferredHeight(contentContainerRect));
				Rect rect = contentContainerRect.rect;
				LanguageRuleExpandOnHover.EExpandDirection eexpandDirection = this.expandDirection;
				bool flag2 = eexpandDirection == LanguageRuleExpandOnHover.EExpandDirection.Left || eexpandDirection == LanguageRuleExpandOnHover.EExpandDirection.Right;
				bool needsExpand;
				if (flag2)
				{
					needsExpand = (preferredSize.x > rect.width + 0.01f);
				}
				else
				{
					needsExpand = (preferredSize.y > rect.height + 0.01f);
				}
				result = needsExpand;
			}
			return result;
		}

		// Token: 0x0600BA23 RID: 47651 RVA: 0x0054C440 File Offset: 0x0054A640
		private void BackupOriginalState()
		{
			LanguageRuleExpandOnHover.OriginalState state = new LanguageRuleExpandOnHover.OriginalState
			{
				LayoutStates = new Dictionary<LayoutGroup, bool>(),
				TargetSizes = new Dictionary<RectTransform, Vector2>(),
				TargetPivots = new Dictionary<RectTransform, Vector2>(),
				TargetAnchorMin = new Dictionary<RectTransform, Vector2>(),
				TargetAnchorMax = new Dictionary<RectTransform, Vector2>()
			};
			bool flag = this.contentContainer != null;
			if (flag)
			{
				state.OriginalContentPadding = this.GetLayoutPadding(this.contentContainer);
			}
			bool flag2 = this.changeSiblingOnExpand;
			if (flag2)
			{
				state.SiblingIndex = base.transform.GetSiblingIndex();
				bool flag3 = this.layoutsToDisable != null;
				if (flag3)
				{
					foreach (LayoutGroup layout in this.layoutsToDisable)
					{
						bool flag4 = layout != null;
						if (flag4)
						{
							state.LayoutStates[layout] = layout.enabled;
						}
					}
				}
			}
			bool flag5 = this.expandTargets != null;
			if (flag5)
			{
				foreach (RectTransform target in this.expandTargets)
				{
					bool flag6 = target == null;
					if (!flag6)
					{
						state.TargetSizes[target] = target.rect.size;
						state.TargetPivots[target] = target.pivot;
						state.TargetAnchorMin[target] = target.anchorMin;
						state.TargetAnchorMax[target] = target.anchorMax;
					}
				}
			}
			this._originalState = new LanguageRuleExpandOnHover.OriginalState?(state);
		}

		// Token: 0x0600BA24 RID: 47652 RVA: 0x0054C600 File Offset: 0x0054A800
		private RectOffset GetLayoutPadding(LayoutGroup layoutGroup)
		{
			if (!true)
			{
			}
			HorizontalLayoutGroup hlg = layoutGroup as HorizontalLayoutGroup;
			RectOffset result;
			if (hlg == null)
			{
				VerticalLayoutGroup vlg = layoutGroup as VerticalLayoutGroup;
				if (vlg == null)
				{
					GridLayoutGroup glg = layoutGroup as GridLayoutGroup;
					if (glg == null)
					{
						result = new RectOffset();
					}
					else
					{
						result = glg.padding;
					}
				}
				else
				{
					result = vlg.padding;
				}
			}
			else
			{
				result = hlg.padding;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600BA25 RID: 47653 RVA: 0x0054C668 File Offset: 0x0054A868
		private void SetLayoutPadding(LayoutGroup layoutGroup, RectOffset paddingValue)
		{
			HorizontalLayoutGroup hlg = layoutGroup as HorizontalLayoutGroup;
			if (hlg == null)
			{
				VerticalLayoutGroup vlg = layoutGroup as VerticalLayoutGroup;
				if (vlg == null)
				{
					GridLayoutGroup glg = layoutGroup as GridLayoutGroup;
					if (glg != null)
					{
						glg.padding = paddingValue;
					}
				}
				else
				{
					vlg.padding = paddingValue;
				}
			}
			else
			{
				hlg.padding = paddingValue;
			}
		}

		// Token: 0x0600BA26 RID: 47654 RVA: 0x0054C6C0 File Offset: 0x0054A8C0
		private void Expand()
		{
			bool flag = this._originalState == null;
			if (!flag)
			{
				LanguageRuleExpandOnHover.OriginalState state = this._originalState.Value;
				bool flag2 = this.changeSiblingOnExpand;
				if (flag2)
				{
					bool flag3 = this.layoutsToDisable != null;
					if (flag3)
					{
						foreach (LayoutGroup layout in this.layoutsToDisable)
						{
							bool flag4 = layout != null;
							if (flag4)
							{
								layout.enabled = false;
							}
						}
					}
					base.transform.SetAsLastSibling();
				}
				LanguageRuleExpandOnHover.EExpandDirection finalDirection = this.GetFinalExpandDirection();
				bool flag5 = this.contentContainer != null;
				if (flag5)
				{
					RectOffset originalPadding = state.OriginalContentPadding;
					bool isHorizontal = finalDirection == LanguageRuleExpandOnHover.EExpandDirection.Left || finalDirection == LanguageRuleExpandOnHover.EExpandDirection.Right;
					bool isVertical = finalDirection == LanguageRuleExpandOnHover.EExpandDirection.Up || finalDirection == LanguageRuleExpandOnHover.EExpandDirection.Down;
					RectOffset paddingToApply = new RectOffset(originalPadding.left + (isHorizontal ? this.padding : 0), originalPadding.right + (isHorizontal ? this.padding : 0), originalPadding.top + (isVertical ? this.padding : 0), originalPadding.bottom + (isVertical ? this.padding : 0));
					this.SetLayoutPadding(this.contentContainer, paddingToApply);
				}
				bool flag6 = this.expandTargets == null;
				if (!flag6)
				{
					foreach (RectTransform target in this.expandTargets)
					{
						bool flag7 = target == null;
						if (!flag7)
						{
							Vector2 originalSize = state.TargetSizes[target];
							this.SetTargetAlignment(target, finalDirection);
							Vector2 newSize = this.CalculateNewSize(originalSize, finalDirection);
							target.SetSize(newSize);
						}
					}
					RectTransform contentContainerRect = this.contentContainer.GetComponent<RectTransform>();
					LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainerRect);
					this._isExpanded = true;
				}
			}
		}

		// Token: 0x0600BA27 RID: 47655 RVA: 0x0054C8AC File Offset: 0x0054AAAC
		private void SetTargetAlignment(RectTransform target, LanguageRuleExpandOnHover.EExpandDirection direction)
		{
			switch (direction)
			{
			case LanguageRuleExpandOnHover.EExpandDirection.Up:
				this.SetPivotAndAnchor(target, new Vector2(target.pivot.x, 0f), new Vector2(target.anchorMin.x, 0f), new Vector2(target.anchorMax.x, 0f));
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Down:
				this.SetPivotAndAnchor(target, new Vector2(target.pivot.x, 1f), new Vector2(target.anchorMin.x, 1f), new Vector2(target.anchorMax.x, 1f));
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Left:
				this.SetPivotAndAnchor(target, new Vector2(1f, target.pivot.y), new Vector2(1f, target.anchorMin.y), new Vector2(1f, target.anchorMax.y));
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Right:
				this.SetPivotAndAnchor(target, new Vector2(0f, target.pivot.y), new Vector2(0f, target.anchorMin.y), new Vector2(0f, target.anchorMax.y));
				break;
			}
		}

		// Token: 0x0600BA28 RID: 47656 RVA: 0x0054CA04 File Offset: 0x0054AC04
		private void SetPivotAndAnchor(RectTransform target, Vector2 newPivot, Vector2 newAnchorMin, Vector2 newAnchorMax)
		{
			Vector3[] corners = new Vector3[4];
			target.GetWorldCorners(corners);
			Vector3[] savedCorners = new Vector3[4];
			Array.Copy(corners, savedCorners, 4);
			target.anchorMin = newAnchorMin;
			target.anchorMax = newAnchorMax;
			target.pivot = newPivot;
			target.GetWorldCorners(corners);
			Vector3 offset = savedCorners[0] - corners[0];
			target.position += offset;
		}

		// Token: 0x0600BA29 RID: 47657 RVA: 0x0054CA7C File Offset: 0x0054AC7C
		private Vector2 CalculateNewSize(Vector2 originalSize, LanguageRuleExpandOnHover.EExpandDirection direction)
		{
			Vector2 newSize = originalSize;
			RectTransform contentContainerRect = this.contentContainer.GetComponent<RectTransform>();
			bool flag = direction == LanguageRuleExpandOnHover.EExpandDirection.Left || direction == LanguageRuleExpandOnHover.EExpandDirection.Right;
			if (flag)
			{
				float preferredWidth = LayoutUtility.GetPreferredWidth(contentContainerRect);
				float currentWidth = originalSize.x;
				int extra = Mathf.CeilToInt(preferredWidth - currentWidth);
				bool flag2 = extra > 0;
				if (flag2)
				{
					extra += 2 * this.padding;
					newSize.x = currentWidth + (float)extra;
				}
			}
			else
			{
				float preferredHeight = LayoutUtility.GetPreferredHeight(contentContainerRect);
				float currentHeight = originalSize.y;
				int extra2 = Mathf.CeilToInt(preferredHeight - currentHeight);
				bool flag3 = extra2 > 0;
				if (flag3)
				{
					extra2 += 2 * this.padding;
					newSize.y = currentHeight + (float)extra2;
				}
			}
			return newSize;
		}

		// Token: 0x0600BA2A RID: 47658 RVA: 0x0054CB38 File Offset: 0x0054AD38
		private void RestoreOriginalState()
		{
			bool flag = this._originalState == null;
			if (!flag)
			{
				LanguageRuleExpandOnHover.OriginalState state = this._originalState.Value;
				bool flag2 = this.contentContainer != null;
				if (flag2)
				{
					this.SetLayoutPadding(this.contentContainer, state.OriginalContentPadding);
				}
				bool flag3 = this.changeSiblingOnExpand;
				if (flag3)
				{
					base.transform.SetSiblingIndex(state.SiblingIndex);
					foreach (KeyValuePair<LayoutGroup, bool> kvp in state.LayoutStates)
					{
						bool flag4 = kvp.Key != null;
						if (flag4)
						{
							kvp.Key.enabled = kvp.Value;
						}
					}
				}
				foreach (KeyValuePair<RectTransform, Vector2> kvp2 in state.TargetSizes)
				{
					bool flag5 = kvp2.Key == null;
					if (!flag5)
					{
						kvp2.Key.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, kvp2.Value.x);
						kvp2.Key.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, kvp2.Value.y);
					}
				}
				foreach (KeyValuePair<RectTransform, Vector2> kvp3 in state.TargetPivots)
				{
					bool flag6 = kvp3.Key == null;
					if (!flag6)
					{
						this.SetPivotAndAnchor(kvp3.Key, kvp3.Value, state.TargetAnchorMin[kvp3.Key], state.TargetAnchorMax[kvp3.Key]);
					}
				}
				this._isExpanded = false;
			}
		}

		// Token: 0x0600BA2B RID: 47659 RVA: 0x0054CD34 File Offset: 0x0054AF34
		private LanguageRuleExpandOnHover.EExpandDirection GetFinalExpandDirection()
		{
			bool flag = !this.autoDetectReverseDirection;
			LanguageRuleExpandOnHover.EExpandDirection result;
			if (flag)
			{
				result = this.expandDirection;
			}
			else
			{
				bool flag2 = this.autoDetectReverseDirectionTarget == null;
				if (flag2)
				{
					this.autoDetectReverseDirectionTarget = (base.transform.parent as RectTransform);
				}
				RectTransform selfRect = base.transform.GetComponent<RectTransform>();
				RectTransform contentContainerRect = this.contentContainer.GetComponent<RectTransform>();
				LanguageRuleExpandOnHover.EExpandDirection eexpandDirection = this.expandDirection;
				bool flag3 = eexpandDirection == LanguageRuleExpandOnHover.EExpandDirection.Left || eexpandDirection == LanguageRuleExpandOnHover.EExpandDirection.Right;
				float extraSpace;
				if (flag3)
				{
					float preferredWidth = LayoutUtility.GetPreferredWidth(contentContainerRect);
					extraSpace = preferredWidth - selfRect.rect.width + (float)this.padding;
				}
				else
				{
					float preferredHeight = LayoutUtility.GetPreferredHeight(contentContainerRect);
					extraSpace = preferredHeight - selfRect.rect.height + (float)this.padding;
				}
				bool flag4 = extraSpace <= 0f;
				if (flag4)
				{
					result = this.expandDirection;
				}
				else
				{
					bool hasSpaceInDirection = LanguageRuleExpandOnHover.HasSpaceToExpand(selfRect, this.autoDetectReverseDirectionTarget, this.expandDirection, extraSpace);
					bool flag5 = hasSpaceInDirection;
					if (flag5)
					{
						result = this.expandDirection;
					}
					else
					{
						LanguageRuleExpandOnHover.EExpandDirection reverseDirection = LanguageRuleExpandOnHover.GetReverseDirection(this.expandDirection);
						result = (LanguageRuleExpandOnHover.HasSpaceToExpand(selfRect, this.autoDetectReverseDirectionTarget, reverseDirection, extraSpace) ? reverseDirection : this.expandDirection);
					}
				}
			}
			return result;
		}

		// Token: 0x0600BA2C RID: 47660 RVA: 0x0054CE80 File Offset: 0x0054B080
		private static bool HasSpaceToExpand(RectTransform selfRect, RectTransform parentRect, LanguageRuleExpandOnHover.EExpandDirection direction, float extraSpace)
		{
			Vector3[] selfCorners = new Vector3[4];
			selfRect.GetWorldCorners(selfCorners);
			Vector3[] parentCorners = new Vector3[4];
			parentRect.GetWorldCorners(parentCorners);
			for (int i = 0; i < 4; i++)
			{
				selfCorners[i] = parentRect.InverseTransformPoint(selfCorners[i]);
				parentCorners[i] = parentRect.InverseTransformPoint(parentCorners[i]);
			}
			bool result;
			switch (direction)
			{
			case LanguageRuleExpandOnHover.EExpandDirection.Up:
				result = (selfCorners[1].y + extraSpace <= parentCorners[1].y);
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Down:
				result = (selfCorners[0].y - extraSpace >= parentCorners[0].y);
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Left:
				result = (selfCorners[0].x - extraSpace >= parentCorners[0].x);
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Right:
				result = (selfCorners[2].x + extraSpace <= parentCorners[2].x);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x0600BA2D RID: 47661 RVA: 0x0054CF9C File Offset: 0x0054B19C
		private static LanguageRuleExpandOnHover.EExpandDirection GetReverseDirection(LanguageRuleExpandOnHover.EExpandDirection direction)
		{
			if (!true)
			{
			}
			LanguageRuleExpandOnHover.EExpandDirection result;
			switch (direction)
			{
			case LanguageRuleExpandOnHover.EExpandDirection.Up:
				result = LanguageRuleExpandOnHover.EExpandDirection.Down;
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Down:
				result = LanguageRuleExpandOnHover.EExpandDirection.Up;
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Left:
				result = LanguageRuleExpandOnHover.EExpandDirection.Right;
				break;
			case LanguageRuleExpandOnHover.EExpandDirection.Right:
				result = LanguageRuleExpandOnHover.EExpandDirection.Left;
				break;
			default:
				result = direction;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600BA2E RID: 47662 RVA: 0x0054CFE4 File Offset: 0x0054B1E4
		public void ForceExpand()
		{
			bool isExpanded = this._isExpanded;
			if (!isExpanded)
			{
				bool flag = !this.NeedsExpansion();
				if (!flag)
				{
					this._isHovering = true;
					this.BackupOriginalState();
					this.Expand();
				}
			}
		}

		// Token: 0x0600BA2F RID: 47663 RVA: 0x0054D024 File Offset: 0x0054B224
		public void ForceCollapse()
		{
			bool flag = !this._isExpanded;
			if (!flag)
			{
				this._isHovering = false;
				this.RestoreOriginalState();
			}
		}

		// Token: 0x0600BA30 RID: 47664 RVA: 0x0054D050 File Offset: 0x0054B250
		public void SetContentContainer(LayoutGroup newContainer)
		{
			bool isExpanded = this._isExpanded;
			if (isExpanded)
			{
				this.ForceCollapse();
			}
			this.contentContainer = newContainer;
		}

		// Token: 0x0600BA31 RID: 47665 RVA: 0x0054D078 File Offset: 0x0054B278
		public void SetExpandTargets(RectTransform[] newTargets)
		{
			bool isExpanded = this._isExpanded;
			if (isExpanded)
			{
				this.ForceCollapse();
			}
			this.expandTargets = newTargets;
		}

		// Token: 0x04008FEC RID: 36844
		[Header("核心配置")]
		[Tooltip("文本所在的容器，需要是一个布局组件，用于计算内部所有元素的尺寸总和")]
		[SerializeField]
		private LayoutGroup contentContainer;

		// Token: 0x04008FED RID: 36845
		[Tooltip("在计算出的尺寸基础上增加的额外边距")]
		[SerializeField]
		private int padding = 30;

		// Token: 0x04008FEE RID: 36846
		[Header("扩展配置")]
		[Tooltip("额外控制大小的目标，这些RectTransform会在悬浮时变化大小。不用包括contentContainer")]
		[SerializeField]
		private RectTransform[] expandTargets;

		// Token: 0x04008FEF RID: 36847
		[Tooltip("扩展方向：Up, Down, Left, Right")]
		[SerializeField]
		private LanguageRuleExpandOnHover.EExpandDirection expandDirection = LanguageRuleExpandOnHover.EExpandDirection.Right;

		// Token: 0x04008FF0 RID: 36848
		[Tooltip("扩展期间是否改变sibling，使其显示在最上层。如果contentContainer在一个layout内，就需要使用")]
		[SerializeField]
		private bool changeSiblingOnExpand;

		// Token: 0x04008FF1 RID: 36849
		[Tooltip("改变sibling时需要临时关闭的布局组件，防止位置错乱。一般就是contentContainer的父节点")]
		[SerializeField]
		private List<LayoutGroup> layoutsToDisable;

		// Token: 0x04008FF2 RID: 36850
		[Header("自动反向检测")]
		[Tooltip("是否自动检测反方向扩展空间")]
		[SerializeField]
		private bool autoDetectReverseDirection;

		// Token: 0x04008FF3 RID: 36851
		[Tooltip("用于检测反向扩展空间的目标节点，通常是当前UI的父节点")]
		[SerializeField]
		private RectTransform autoDetectReverseDirectionTarget;

		// Token: 0x04008FF4 RID: 36852
		private bool _isWorking;

		// Token: 0x04008FF5 RID: 36853
		private LanguageRuleExpandOnHover.OriginalState? _originalState;

		// Token: 0x04008FF6 RID: 36854
		private bool _isExpanded;

		// Token: 0x04008FF7 RID: 36855
		private bool _isHovering;

		// Token: 0x02002631 RID: 9777
		private enum EExpandDirection
		{
			// Token: 0x0400E9E6 RID: 59878
			Up,
			// Token: 0x0400E9E7 RID: 59879
			Down,
			// Token: 0x0400E9E8 RID: 59880
			Left,
			// Token: 0x0400E9E9 RID: 59881
			Right
		}

		// Token: 0x02002632 RID: 9778
		private struct OriginalState
		{
			// Token: 0x0400E9EA RID: 59882
			public int SiblingIndex;

			// Token: 0x0400E9EB RID: 59883
			public Dictionary<LayoutGroup, bool> LayoutStates;

			// Token: 0x0400E9EC RID: 59884
			public Dictionary<RectTransform, Vector2> TargetSizes;

			// Token: 0x0400E9ED RID: 59885
			public Dictionary<RectTransform, Vector2> TargetPivots;

			// Token: 0x0400E9EE RID: 59886
			public Dictionary<RectTransform, Vector2> TargetAnchorMin;

			// Token: 0x0400E9EF RID: 59887
			public Dictionary<RectTransform, Vector2> TargetAnchorMax;

			// Token: 0x0400E9F0 RID: 59888
			public RectOffset OriginalContentPadding;
		}
	}
}
