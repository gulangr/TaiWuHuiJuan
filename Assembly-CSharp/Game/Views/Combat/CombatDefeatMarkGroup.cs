using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Combat
{
	// Token: 0x02000B2B RID: 2859
	public class CombatDefeatMarkGroup : TooltipInvoker
	{
		// Token: 0x17000F72 RID: 3954
		// (get) Token: 0x06008C1E RID: 35870 RVA: 0x0040BE48 File Offset: 0x0040A048
		// (set) Token: 0x06008C1F RID: 35871 RVA: 0x0040BE50 File Offset: 0x0040A050
		public bool RaycastBlocked { get; set; }

		// Token: 0x06008C20 RID: 35872 RVA: 0x0040BE5C File Offset: 0x0040A05C
		private void Update()
		{
			DefeatMarkKey markKey = this.GetHoveringMarkKey();
			bool valid = markKey.Valid;
			if (valid)
			{
				this._runtimeArgs.Set("MarkKey", markKey);
				base.RuntimeParam = this._runtimeArgs;
				this.ShowGroup(markKey);
			}
			else
			{
				base.RuntimeParam = null;
				this.HideGroup();
			}
			bool flag = this._showingMarkKey.Valid && !markKey.Valid;
			if (flag)
			{
				base.HideTips();
			}
			else
			{
				bool flag2 = markKey.Valid && !this._showingMarkKey.Equals(markKey);
				if (flag2)
				{
					SingletonObject.getInstance<TooltipManager>().HideTips(TipType.Count, true);
					base.ShowTips();
				}
			}
			this._showingMarkKey = markKey;
		}

		// Token: 0x06008C21 RID: 35873 RVA: 0x0040BF26 File Offset: 0x0040A126
		public void Set(RectTransform markHolder, List<RectTransform> markQueue, List<DefeatMarkKey> markKeyList, int charId)
		{
			this._markHolder = markHolder;
			this._markQueue = markQueue;
			this._markKeyList = markKeyList;
			this._runtimeArgs.SetObject("MarkKeyList", markKeyList);
			this._runtimeArgs.Set("CharId", charId);
		}

		// Token: 0x06008C22 RID: 35874 RVA: 0x0040BF63 File Offset: 0x0040A163
		public void Set(int defeatMarkCount)
		{
			this._defeatMarkCount = defeatMarkCount;
		}

		// Token: 0x06008C23 RID: 35875 RVA: 0x0040BF6D File Offset: 0x0040A16D
		public void Set(HeavyOrBreakInjuryData heavyOrBreakInjuryData)
		{
			this._runtimeArgs.SetObject("HeavyOrBreakData", heavyOrBreakInjuryData);
		}

		// Token: 0x06008C24 RID: 35876 RVA: 0x0040BF88 File Offset: 0x0040A188
		private DefeatMarkKey GetHoveringMarkKey()
		{
			bool raycastBlocked = this.RaycastBlocked;
			DefeatMarkKey result;
			if (raycastBlocked)
			{
				result = DefeatMarkKey.Invalid;
			}
			else
			{
				List<RectTransform> markQueue = this._markQueue;
				bool flag = markQueue == null || markQueue.Count <= 0;
				if (flag)
				{
					result = DefeatMarkKey.Invalid;
				}
				else
				{
					bool flag2 = this._markHolder == null;
					if (flag2)
					{
						result = DefeatMarkKey.Invalid;
					}
					else
					{
						Camera uiCamera = UIManager.Instance.UiCamera;
						bool flag3 = !RectTransformUtility.RectangleContainsScreenPoint(this._markHolder, Input.mousePosition, uiCamera);
						if (flag3)
						{
							result = DefeatMarkKey.Invalid;
						}
						else
						{
							Vector2 localPos;
							bool flag4 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this._markHolder, Input.mousePosition, uiCamera, out localPos);
							if (flag4)
							{
								result = DefeatMarkKey.Invalid;
							}
							else
							{
								bool flag5 = !UIManager.Instance.IsFocusElement(UIElement.Combat);
								if (flag5)
								{
									result = DefeatMarkKey.Invalid;
								}
								else
								{
									float spacing = this._markHolder.GetComponent<HorizontalLayoutGroup>().spacing;
									float width = this._markQueue[0].rect.width;
									bool flag6 = width + spacing <= 0f;
									if (flag6)
									{
										result = DefeatMarkKey.Invalid;
									}
									else
									{
										float maxDelta = width * this.maxDeltaRatio;
										DefeatMarkKey maxDeltaMark = DefeatMarkKey.Invalid;
										foreach (RectTransform mark in this._markQueue)
										{
											float delta = Mathf.Abs(mark.localPosition.x - localPos.x);
											bool flag7 = delta < maxDelta;
											if (flag7)
											{
												maxDelta = delta;
												maxDeltaMark = (DefeatMarkKey)mark.GetComponent<CombatDefeatMark>().UserInt;
											}
										}
										result = maxDeltaMark;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008C25 RID: 35877 RVA: 0x0040C16C File Offset: 0x0040A36C
		private void ShowGroup(DefeatMarkKey markKey)
		{
			int index = this._markKeyList.IndexOf(markKey);
			int firstKeyIndex = index;
			for (int i = index - 1; i >= 0; i--)
			{
				bool flag = this._markKeyList[i].GroupEquals(markKey);
				if (flag)
				{
					firstKeyIndex = i;
				}
			}
			int lastKeyIndex = index;
			for (int j = index + 1; j < this._markKeyList.Count; j++)
			{
				bool flag2 = this._markKeyList[j].GroupEquals(markKey);
				if (flag2)
				{
					lastKeyIndex = j;
				}
			}
			lastKeyIndex = Mathf.Min(lastKeyIndex, this._markQueue.Count - 1);
			firstKeyIndex = Mathf.Min(firstKeyIndex, lastKeyIndex);
			int keyCount = lastKeyIndex - firstKeyIndex + 1;
			float spacing = this._markHolder.GetComponent<HorizontalLayoutGroup>().spacing;
			float markWidth = this._markQueue[0].rect.width;
			float posX = this.groupReverse ? Mathf.Round(this._markHolder.rect.width - this._markQueue[firstKeyIndex].anchoredPosition.x) : this._markQueue[firstKeyIndex].anchoredPosition.x;
			posX -= markWidth / 2f + this.groupPadding;
			posX *= (float)(this.groupReverse ? -1 : 1);
			float width = markWidth * (float)keyCount + spacing * (float)(keyCount - 1) + this.groupPadding * 2f;
			bool flag3 = spacing < 0f && lastKeyIndex != this._markQueue.Count - 1;
			if (flag3)
			{
				width += spacing;
			}
			this.group.anchoredPosition = this.group.anchoredPosition.SetX(posX);
			this.group.sizeDelta = this.group.sizeDelta.SetX(width);
			this.group.gameObject.SetActive(true);
		}

		// Token: 0x06008C26 RID: 35878 RVA: 0x0040C366 File Offset: 0x0040A566
		private void HideGroup()
		{
			this.group.gameObject.SetActive(false);
		}

		// Token: 0x04006B3F RID: 27455
		[SerializeField]
		private float maxDeltaRatio = 1.2f;

		// Token: 0x04006B40 RID: 27456
		[SerializeField]
		private float groupPadding = 3f;

		// Token: 0x04006B41 RID: 27457
		[SerializeField]
		private bool groupReverse;

		// Token: 0x04006B42 RID: 27458
		[SerializeField]
		private RectTransform group;

		// Token: 0x04006B44 RID: 27460
		private RectTransform _markHolder;

		// Token: 0x04006B45 RID: 27461
		private List<RectTransform> _markQueue;

		// Token: 0x04006B46 RID: 27462
		private List<DefeatMarkKey> _markKeyList;

		// Token: 0x04006B47 RID: 27463
		private int _defeatMarkCount;

		// Token: 0x04006B48 RID: 27464
		private readonly ArgumentBox _runtimeArgs = new ArgumentBox();

		// Token: 0x04006B49 RID: 27465
		private DefeatMarkKey _showingMarkKey;
	}
}
