using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C37 RID: 3127
	public class MapBlockFindItem_MultiSelect : MapBlockFindItemBase
	{
		// Token: 0x06009EE0 RID: 40672 RVA: 0x004A4B00 File Offset: 0x004A2D00
		protected override void InitComponents()
		{
			base.InitComponents();
			this.toggleGroup.Init();
			this.toggleGroup.OnActiveIndexChange += this.OnActiveIndexChange;
		}

		// Token: 0x06009EE1 RID: 40673 RVA: 0x004A4B30 File Offset: 0x004A2D30
		public override void Set(ViewFindMapBlock mapBlockFind, EFilterItemKey key, FilterItemConfig itemConfig)
		{
			base.Set(mapBlockFind, key, itemConfig);
			this._directionStates.Clear();
			EFilterItemKey efilterItemKey = base.EFilterItemKey;
			bool flag = efilterItemKey == EFilterItemKey.CharacterCombatSkillType || efilterItemKey == EFilterItemKey.CharacterLifeSkillType;
			if (flag)
			{
				this._mapBlockFind.UpdateSkillTypeVisibility();
			}
			this.SetWithoutNotify();
		}

		// Token: 0x06009EE2 RID: 40674 RVA: 0x004A4B88 File Offset: 0x004A2D88
		protected override void Init()
		{
			base.Init();
			bool flag = this._itemConfig.Options != null;
			if (flag)
			{
				CommonUtils.PrepareEnoughChildren(this.toggleGroup.transform, this.toggleGroup.transform.GetChild(0).gameObject, this._itemConfig.Options.Length, null);
				for (int i = 0; i < this._itemConfig.Options.Length; i++)
				{
					ToggleStyle toggleStyle = this.toggleGroup.transform.GetChild(i).GetComponent<ToggleStyle>();
					toggleStyle.SetLabelText(this._itemConfig.Options[i]);
				}
				this.toggleGroup.AddAllChildToggles();
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				this.AutoLayoutToggles();
				this.toggleGroup.Init();
			});
		}

		// Token: 0x06009EE3 RID: 40675 RVA: 0x004A4C60 File Offset: 0x004A2E60
		private void AutoLayoutToggles()
		{
			bool isActive = base.gameObject.activeSelf;
			base.gameObject.SetActive(true);
			bool flag = this._itemConfig.Options == null || this._itemConfig.Options.Length == 0;
			if (!flag)
			{
				int count = this._itemConfig.Options.Length;
				RectTransform containerRect = (RectTransform)this.toggleGroup.transform;
				for (int i = 0; i < count; i++)
				{
					LayoutElement layoutElement = containerRect.GetChild(i).GetComponent<LayoutElement>();
					bool flag2 = layoutElement != null;
					if (flag2)
					{
						layoutElement.preferredWidth = -1f;
					}
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);
				float maxWidth = 0f;
				for (int j = 0; j < count; j++)
				{
					RectTransform childRect = (RectTransform)containerRect.GetChild(j);
					float preferredWidth = LayoutUtility.GetPreferredWidth(childRect);
					bool flag3 = preferredWidth > maxWidth;
					if (flag3)
					{
						maxWidth = preferredWidth;
					}
				}
				for (int k = 0; k < count; k++)
				{
					LayoutElement layoutElement2 = containerRect.GetChild(k).GetComponent<LayoutElement>();
					bool flag4 = layoutElement2 != null;
					if (flag4)
					{
						layoutElement2.preferredWidth = maxWidth;
					}
				}
				float containerWidth = containerRect.rect.width;
				float toggleWithSpacing = maxWidth + 2f;
				int togglesPerRow = (toggleWithSpacing > 0f) ? Mathf.Max(1, Mathf.FloorToInt(containerWidth / toggleWithSpacing)) : 1;
				int rowCount = Mathf.CeilToInt((float)count / (float)togglesPerRow);
				float containerHeight = (float)rowCount * 40f + (float)Mathf.Max(0, rowCount - 1) * 2f;
				LayoutElement containerLayout = this.toggleGroup.GetComponent<LayoutElement>();
				bool flag5 = containerLayout == null;
				if (flag5)
				{
					containerLayout = this.toggleGroup.gameObject.AddComponent<LayoutElement>();
				}
				containerLayout.preferredHeight = containerHeight;
				for (int l = 0; l < count; l++)
				{
					RectTransform childRect2 = (RectTransform)containerRect.GetChild(l);
					childRect2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
					childRect2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40f);
					int col = l % togglesPerRow;
					int row = l / togglesPerRow;
					float x = (float)col * (maxWidth + 2f);
					float y = -((float)row * 42f);
					childRect2.anchoredPosition = new Vector2(x, y);
				}
				base.gameObject.SetActive(isActive);
			}
		}

		// Token: 0x06009EE4 RID: 40676 RVA: 0x004A4EB8 File Offset: 0x004A30B8
		private void OnActiveIndexChange(int newIndex, int removedIndex)
		{
			bool flag = removedIndex >= 0;
			if (flag)
			{
				this._directionStates.Remove(removedIndex);
			}
			bool flag2 = newIndex >= 0 && this.IsDirectionOption(newIndex);
			if (flag2)
			{
				this._directionStates[newIndex] = 0;
			}
			this.NotifyDataChanged();
		}

		// Token: 0x06009EE5 RID: 40677 RVA: 0x004A4F0C File Offset: 0x004A310C
		public void ToggleDirection(int optionIndex)
		{
			bool flag = !this.IsDirectionOption(optionIndex);
			if (!flag)
			{
				List<int> activeIndices = this.toggleGroup.GetActiveIndices();
				bool flag2 = !activeIndices.Contains(optionIndex);
				if (!flag2)
				{
					int state;
					bool flag3 = this._directionStates.TryGetValue(optionIndex, out state);
					if (flag3)
					{
						bool flag4 = state < 2;
						if (flag4)
						{
							this._directionStates[optionIndex] = state + 1;
						}
						else
						{
							this.toggleGroup.DeSelectWithoutNotify(optionIndex);
							this._directionStates.Remove(optionIndex);
						}
					}
					this.NotifyDataChanged();
				}
			}
		}

		// Token: 0x06009EE6 RID: 40678 RVA: 0x004A4FA0 File Offset: 0x004A31A0
		private bool IsDirectionOption(int index)
		{
			bool flag = this._directionOptionIndices == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int i in this._directionOptionIndices)
				{
					bool flag2 = i == index;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06009EE7 RID: 40679 RVA: 0x004A4FF0 File Offset: 0x004A31F0
		private void NotifyDataChanged()
		{
			List<int> indices = this.toggleGroup.GetActiveIndices();
			this._mapBlockFind.UpdateMultiSelectData(base.EFilterItemKey, indices.ToArray());
		}

		// Token: 0x06009EE8 RID: 40680 RVA: 0x004A5022 File Offset: 0x004A3222
		public override void Reset()
		{
			this.toggleGroup.DeSelectAll(false);
			this._directionStates.Clear();
			base.Reset();
		}

		// Token: 0x06009EE9 RID: 40681 RVA: 0x004A5048 File Offset: 0x004A3248
		public override void SetWithoutNotify()
		{
			for (int i = 0; i < this.toggleGroup.Count(); i++)
			{
				this.toggleGroup.DeSelectWithoutNotify(i);
			}
			IntList value;
			bool flag = !base.Data.MultiSelectData.TryGetValue(base.EFilterItemKey, out value) || value.Items == null;
			if (!flag)
			{
				foreach (int idx in value.Items)
				{
					this.toggleGroup.SelectWithoutNotify(idx);
				}
			}
		}

		// Token: 0x06009EEA RID: 40682 RVA: 0x004A5100 File Offset: 0x004A3300
		public override bool HasFilterValue()
		{
			return this.toggleGroup.AnyTogglesOn();
		}

		// Token: 0x04007AF2 RID: 31474
		private const float ToggleRowHeight = 40f;

		// Token: 0x04007AF3 RID: 31475
		private const float ColumnSpacing = 2f;

		// Token: 0x04007AF4 RID: 31476
		private const float RowSpacing = 2f;

		// Token: 0x04007AF5 RID: 31477
		[SerializeField]
		private CToggleGroupMultiSelect toggleGroup;

		// Token: 0x04007AF6 RID: 31478
		private readonly Dictionary<int, int> _directionStates = new Dictionary<int, int>();

		// Token: 0x04007AF7 RID: 31479
		private int[] _directionOptionIndices;
	}
}
