using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000764 RID: 1892
	public class MultiToggleGroupSettingItem : SettingItemBase<int>
	{
		// Token: 0x06005B88 RID: 23432 RVA: 0x002A7EA8 File Offset: 0x002A60A8
		private void Awake()
		{
			bool flag = this.toggleGroup != null;
			if (flag)
			{
				this.toggleGroup.OnActiveIndexChange += this.OnToggleChanged;
			}
		}

		// Token: 0x06005B89 RID: 23433 RVA: 0x002A7EE0 File Offset: 0x002A60E0
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			MultiToggleGroupSettingAttribute attr = info.Attribute as MultiToggleGroupSettingAttribute;
			LanguageKey[] options;
			if ((options = ((attr != null) ? attr.Options : null)) == null)
			{
				LanguageKey[] array = new LanguageKey[2];
				array[0] = LanguageKey.LK_EventWindow_SelectOption_1;
				options = array;
				array[1] = LanguageKey.LK_EventWindow_SelectOption_2;
			}
			this._options = options;
			this._value = this._typedInfo.GetValue();
			this.CreateToggles(info.Attribute as MultiToggleGroupSettingAttribute);
		}

		// Token: 0x06005B8A RID: 23434 RVA: 0x002A7F54 File Offset: 0x002A6154
		private void CreateToggles(MultiToggleGroupSettingAttribute attribute)
		{
			bool flag = this.toggleTemplate == null || this.toggleGroup == null;
			if (!flag)
			{
				this.toggleTemplate.gameObject.SetActive(false);
				CommonUtils.PrepareEnoughChildren(this.toggleGroup.transform, this.toggleTemplate.gameObject, this._options.Length, null);
				this.toggleGroup.AddAllChildToggles();
				List<CToggle> toggles = this.toggleGroup.GetAll();
				int i = 0;
				while (i < toggles.Count && i < this._options.Length)
				{
					CToggle toggle = toggles[i];
					TextMeshProUGUI label = toggle.GetComponentInChildren<TextMeshProUGUI>();
					bool flag2 = label != null;
					if (flag2)
					{
						label.text = this._options[i].Tr();
					}
					TooltipInvoker tip = toggle.GetComponent<TooltipInvoker>();
					bool flag3 = attribute.ExtraTipLanguageKeys == null || i >= attribute.ExtraTipLanguageKeys.Length;
					if (flag3)
					{
						tip.enabled = false;
					}
					else
					{
						tip.enabled = true;
						tip.PresetParam[0] = attribute.ExtraTipLanguageKeys[i].Tr();
					}
					i++;
				}
				this.toggleGroup.Init();
				this.UpdateToggleStates();
				this.ApplyAutoWrapLayout();
			}
		}

		// Token: 0x06005B8B RID: 23435 RVA: 0x002A80B0 File Offset: 0x002A62B0
		private void ApplyAutoWrapLayout()
		{
			bool flag = this.toggleGroup == null;
			if (!flag)
			{
				List<CToggle> toggles = this.toggleGroup.GetAll();
				bool flag2 = toggles.Count == 0;
				if (!flag2)
				{
					List<List<ValueTuple<int, float>>> rows = new List<List<ValueTuple<int, float>>>();
					List<float> rowHeights = new List<float>();
					List<ValueTuple<int, float>> currentRow = new List<ValueTuple<int, float>>();
					float currentOffset = 0f;
					float currentRowHeight = 0f;
					for (int i = 0; i < toggles.Count; i++)
					{
						CToggle toggle = toggles[i];
						bool flag3 = !toggle.gameObject.activeSelf;
						if (!flag3)
						{
							RectTransform rectTransform = toggle.transform as RectTransform;
							bool flag4 = rectTransform == null;
							if (!flag4)
							{
								LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
								float toggleWidth = rectTransform.rect.width;
								float toggleHeight = rectTransform.rect.height;
								bool flag5 = currentOffset + toggleWidth > 824f && currentRow.Count > 0;
								if (flag5)
								{
									rows.Add(currentRow);
									rowHeights.Add(currentRowHeight);
									currentRow = new List<ValueTuple<int, float>>();
									currentOffset = 0f;
									currentRowHeight = 0f;
								}
								currentRow.Add(new ValueTuple<int, float>(i, currentOffset));
								currentOffset += toggleWidth + 8f;
								currentRowHeight = Mathf.Max(currentRowHeight, toggleHeight);
							}
						}
					}
					bool flag6 = currentRow.Count > 0;
					if (flag6)
					{
						rows.Add(currentRow);
						rowHeights.Add(currentRowHeight);
					}
					float currentY = 0f;
					for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
					{
						List<ValueTuple<int, float>> row = rows[rowIndex];
						float rowHeight = rowHeights[rowIndex];
						float rowWidth = 0f;
						bool flag7 = row.Count > 0;
						if (flag7)
						{
							int lastIndex = row[row.Count - 1].Item1;
							float lastOffset = row[row.Count - 1].Item2;
							CToggle lastToggle = toggles[lastIndex];
							RectTransform lastRect = lastToggle.transform as RectTransform;
							float lastWidth = (lastRect != null) ? lastRect.rect.width : 0f;
							rowWidth = lastOffset + lastWidth;
						}
						foreach (ValueTuple<int, float> valueTuple in row)
						{
							int toggleIndex = valueTuple.Item1;
							float offset = valueTuple.Item2;
							CToggle toggle2 = toggles[toggleIndex];
							RectTransform rectTransform2 = toggle2.transform as RectTransform;
							bool flag8 = rectTransform2 == null;
							if (!flag8)
							{
								rectTransform2.anchorMin = new Vector2(1f, 1f);
								rectTransform2.anchorMax = new Vector2(1f, 1f);
								rectTransform2.pivot = new Vector2(0f, 1f);
								rectTransform2.anchoredPosition = new Vector2(-(rowWidth - offset), currentY);
							}
						}
						currentY -= rowHeight + 8f;
					}
					this.SetHeightByRowCount(rows.Count);
				}
			}
		}

		// Token: 0x06005B8C RID: 23436 RVA: 0x002A83EC File Offset: 0x002A65EC
		private void SetHeightByRowCount(int rowCount)
		{
			bool flag = rowCount <= 0;
			if (!flag)
			{
				float targetHeight = 72f + 66f * (float)(rowCount - 1);
				RectTransform rectTransform = base.GetComponent<RectTransform>();
				bool flag2 = rectTransform;
				if (flag2)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
				}
			}
		}

		// Token: 0x06005B8D RID: 23437 RVA: 0x002A8434 File Offset: 0x002A6634
		private void OnToggleChanged(int newIndex, int oldIndex)
		{
			bool isUpdating = this._isUpdating;
			if (!isUpdating)
			{
				bool flag = newIndex >= 0;
				if (flag)
				{
					this._value |= 1 << newIndex;
				}
				bool flag2 = oldIndex >= 0;
				if (flag2)
				{
					this._value &= ~(1 << oldIndex);
				}
				base.InvokeTypedValueChanged(this._value);
			}
		}

		// Token: 0x06005B8E RID: 23438 RVA: 0x002A8498 File Offset: 0x002A6698
		private void UpdateToggleStates()
		{
			this._isUpdating = true;
			bool flag = this.toggleGroup != null;
			if (flag)
			{
				List<CToggle> toggles = this.toggleGroup.GetAll();
				int i = 0;
				while (i < toggles.Count && i < this._options.Length)
				{
					bool isOn = (this._value & 1 << i) != 0;
					bool flag2 = isOn;
					if (flag2)
					{
						this.toggleGroup.SelectWithoutNotify(i);
					}
					else
					{
						this.toggleGroup.DeSelectWithoutNotify(i);
					}
					i++;
				}
			}
			this._isUpdating = false;
		}

		// Token: 0x06005B8F RID: 23439 RVA: 0x002A852D File Offset: 0x002A672D
		public override object GetValue()
		{
			return this._value;
		}

		// Token: 0x06005B90 RID: 23440 RVA: 0x002A853A File Offset: 0x002A673A
		public override void SetValue(object value)
		{
			this._value = Convert.ToInt32(value);
			this.UpdateToggleStates();
		}

		// Token: 0x06005B91 RID: 23441 RVA: 0x002A8550 File Offset: 0x002A6750
		public override void SetTypedValue(int value)
		{
			this._value = value;
			this.UpdateToggleStates();
		}

		// Token: 0x06005B92 RID: 23442 RVA: 0x002A8564 File Offset: 0x002A6764
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.toggleGroup != null;
			if (flag)
			{
				List<CToggle> toggles = this.toggleGroup.GetAll();
				foreach (CToggle toggle in toggles)
				{
					toggle.interactable = interactable;
				}
				HSVStyleRoot hsvStyleRoot;
				bool flag2 = this.toggleGroup.TryGetComponent<HSVStyleRoot>(out hsvStyleRoot);
				if (flag2)
				{
					hsvStyleRoot.SetInteractable(interactable);
				}
			}
		}

		// Token: 0x06005B93 RID: 23443 RVA: 0x002A85F8 File Offset: 0x002A67F8
		private void OnDestroy()
		{
			bool flag = this.toggleGroup != null;
			if (flag)
			{
				this.toggleGroup.OnActiveIndexChange -= this.OnToggleChanged;
			}
		}

		// Token: 0x04003F25 RID: 16165
		[SerializeField]
		private CToggleGroupMultiSelect toggleGroup;

		// Token: 0x04003F26 RID: 16166
		[SerializeField]
		private CToggle toggleTemplate;

		// Token: 0x04003F27 RID: 16167
		private const float MaxRowWidth = 824f;

		// Token: 0x04003F28 RID: 16168
		private const float SingleLineHeight = 72f;

		// Token: 0x04003F29 RID: 16169
		private const float ExtraLineHeight = 66f;

		// Token: 0x04003F2A RID: 16170
		private const float HorizontalSpacing = 8f;

		// Token: 0x04003F2B RID: 16171
		private const float VerticalSpacing = 8f;

		// Token: 0x04003F2C RID: 16172
		private int _value;

		// Token: 0x04003F2D RID: 16173
		private LanguageKey[] _options;

		// Token: 0x04003F2E RID: 16174
		private bool _isUpdating;
	}
}
