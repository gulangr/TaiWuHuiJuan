using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu.EquipCombatSkill
{
	// Token: 0x02000BBD RID: 3005
	public sealed class EquipCombatSkillScrollContentLayout
	{
		// Token: 0x06009773 RID: 38771 RVA: 0x00468CF1 File Offset: 0x00466EF1
		public void Initialize(MonoBehaviour host, ScrollRect scrollRect, IReadOnlyList<EquipCombatSkillLine> lines, float originContentWidth)
		{
			this._host = host;
			this._scrollRect = scrollRect;
			this._lines = lines;
			this._originContentWidth = originContentWidth;
			this._dirty = true;
		}

		// Token: 0x06009774 RID: 38772 RVA: 0x00468D18 File Offset: 0x00466F18
		public void MarkDirty()
		{
			this._dirty = true;
		}

		// Token: 0x06009775 RID: 38773 RVA: 0x00468D24 File Offset: 0x00466F24
		public void Invalidate()
		{
			this._dirty = true;
			this._cachedWidth = 0f;
			bool flag = this._applyCoroutine != null && this._host != null;
			if (flag)
			{
				this._host.StopCoroutine(this._applyCoroutine);
				this._applyCoroutine = null;
			}
			ScrollRect scrollRect = this._scrollRect;
			bool flag2 = ((scrollRect != null) ? scrollRect.content : null) == null;
			if (!flag2)
			{
				RectTransform content = this._scrollRect.content;
				content.sizeDelta = new Vector2(this._originContentWidth, content.sizeDelta.y);
			}
		}

		// Token: 0x06009776 RID: 38774 RVA: 0x00468DC4 File Offset: 0x00466FC4
		public void Dispose()
		{
			bool flag = this._applyCoroutine == null || this._host == null;
			if (!flag)
			{
				this._host.StopCoroutine(this._applyCoroutine);
				this._applyCoroutine = null;
			}
		}

		// Token: 0x06009777 RID: 38775 RVA: 0x00468E08 File Offset: 0x00467008
		public void ScheduleApplyIfDirty()
		{
			bool flag = !this._dirty || this._host == null || !this._host.isActiveAndEnabled;
			if (!flag)
			{
				bool flag2 = this._applyCoroutine != null;
				if (flag2)
				{
					this._host.StopCoroutine(this._applyCoroutine);
				}
				this._applyCoroutine = this._host.StartCoroutine(this.ApplyNextFrame());
			}
		}

		// Token: 0x06009778 RID: 38776 RVA: 0x00468E79 File Offset: 0x00467079
		private IEnumerator ApplyNextFrame()
		{
			yield return null;
			ScrollRect scrollRect = this._scrollRect;
			RectTransform content = (scrollRect != null) ? scrollRect.content : null;
			bool flag = content != null;
			if (flag)
			{
				int wait = 0;
				while (!content.gameObject.activeInHierarchy && wait < 30)
				{
					int num = wait;
					wait = num + 1;
					yield return null;
				}
			}
			yield return null;
			this.ApplyIfDirty();
			this._applyCoroutine = null;
			yield break;
		}

		// Token: 0x06009779 RID: 38777 RVA: 0x00468E88 File Offset: 0x00467088
		public void ApplyIfDirty()
		{
			bool flag;
			if (this._dirty)
			{
				ScrollRect scrollRect = this._scrollRect;
				if (!(((scrollRect != null) ? scrollRect.content : null) == null))
				{
					flag = (this._lines == null);
					goto IL_2F;
				}
			}
			flag = true;
			IL_2F:
			bool flag2 = flag;
			if (!flag2)
			{
				RectTransform content = this._scrollRect.content;
				bool flag3 = !content.gameObject.activeInHierarchy;
				if (!flag3)
				{
					foreach (EquipCombatSkillLine line in this._lines)
					{
						EquipCombatSkillLineWithoutScroll withoutScrollLine = line as EquipCombatSkillLineWithoutScroll;
						bool flag4 = withoutScrollLine != null;
						if (flag4)
						{
							withoutScrollLine.RecalculateLineWidth();
						}
					}
					Canvas.ForceUpdateCanvases();
					LayoutRebuilder.ForceRebuildLayoutImmediate(content);
					float maxExtraWidth = 0f;
					foreach (EquipCombatSkillLine line2 in this._lines)
					{
						EquipCombatSkillLineWithoutScroll withoutScrollLine2 = line2 as EquipCombatSkillLineWithoutScroll;
						bool flag5 = withoutScrollLine2 != null;
						if (flag5)
						{
							maxExtraWidth = Mathf.Max(maxExtraWidth, withoutScrollLine2.GetWidthBeyondDesign());
						}
					}
					float targetWidth = this._originContentWidth + maxExtraWidth;
					bool flag6 = Mathf.Approximately(content.sizeDelta.x, targetWidth);
					if (flag6)
					{
						this._cachedWidth = targetWidth;
						this._dirty = false;
					}
					else
					{
						content.sizeDelta = new Vector2(targetWidth, content.sizeDelta.y);
						this._cachedWidth = targetWidth;
						this._dirty = false;
					}
				}
			}
		}

		// Token: 0x0400741B RID: 29723
		private MonoBehaviour _host;

		// Token: 0x0400741C RID: 29724
		private ScrollRect _scrollRect;

		// Token: 0x0400741D RID: 29725
		private IReadOnlyList<EquipCombatSkillLine> _lines;

		// Token: 0x0400741E RID: 29726
		private float _originContentWidth;

		// Token: 0x0400741F RID: 29727
		private bool _dirty = true;

		// Token: 0x04007420 RID: 29728
		private float _cachedWidth;

		// Token: 0x04007421 RID: 29729
		private Coroutine _applyCoroutine;
	}
}
