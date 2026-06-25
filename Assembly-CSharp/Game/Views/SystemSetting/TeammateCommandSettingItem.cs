using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000768 RID: 1896
	public class TeammateCommandSettingItem : SettingItemBase
	{
		// Token: 0x06005BB7 RID: 23479 RVA: 0x002A8DC4 File Offset: 0x002A6FC4
		private void Awake()
		{
			bool flag = this.openAllBtn != null;
			if (flag)
			{
				this.openAllBtn.onClick.AddListener(new UnityAction(this.OnOpenAllClick));
			}
			bool flag2 = this.closeAllBtn != null;
			if (flag2)
			{
				this.closeAllBtn.onClick.AddListener(new UnityAction(this.OnCloseAllClick));
			}
			bool flag3 = this.toggleGroup != null;
			if (flag3)
			{
				this.toggleGroup.OnActiveIndexChange += this.OnToggleChanged;
			}
		}

		// Token: 0x06005BB8 RID: 23480 RVA: 0x002A8E54 File Offset: 0x002A7054
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			this._attr = (info.Attribute as TeammateCommandSettingAttribute);
			this._aiOptions = SystemSettingMapping.AiOptionsRef;
			bool flag = this._aiOptions == null;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this.InitOptionNames();
				this._values = new bool[this._optionNames.Count];
				int i = 0;
				while (i < this._values.Length && i < this._aiOptions.AutoUseTeammateCommand.Length)
				{
					this._values[i] = this._aiOptions.AutoUseTeammateCommand[i];
					i++;
				}
				this.CreateToggles();
			}
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x002A8F08 File Offset: 0x002A7108
		private void InitOptionNames()
		{
			this._optionNames.Clear();
			Dictionary<int, string> implementToOptions = new Dictionary<int, string>();
			foreach (TeammateCommandItem config in ((IEnumerable<TeammateCommandItem>)TeammateCommand.Instance))
			{
				implementToOptions[(int)config.Option] = config.Name;
			}
			for (int i = 0; i < 25; i++)
			{
				string name;
				bool flag = implementToOptions.TryGetValue(i, out name);
				if (flag)
				{
					this._optionNames.Add(name);
				}
				else
				{
					this._optionNames.Add(string.Format("Option_{0}", i));
				}
			}
		}

		// Token: 0x06005BBA RID: 23482 RVA: 0x002A8FCC File Offset: 0x002A71CC
		private void CreateToggles()
		{
			bool flag = this.toggleTemplate == null || this.toggleGroup == null;
			if (!flag)
			{
				CommonUtils.PrepareEnoughChildren(this.toggleGroup.transform, this.toggleTemplate.gameObject, this._optionNames.Count, null);
				this.toggleGroup.AddAllChildToggles();
				List<CToggle> toggles = this.toggleGroup.GetAll();
				int i = 0;
				while (i < toggles.Count && i < this._optionNames.Count)
				{
					CToggle toggle = toggles[i];
					toggle.isOn = this._values[i];
					TextMeshProUGUI label = toggle.GetComponentInChildren<TextMeshProUGUI>();
					bool flag2 = label != null;
					if (flag2)
					{
						label.text = this._optionNames[i];
					}
					TooltipInvoker tip = toggle.GetComponent<TooltipInvoker>();
					tip.enabled = false;
					i++;
				}
				this.toggleGroup.Init();
				this.ApplyAutoWrapLayout();
			}
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x002A90DC File Offset: 0x002A72DC
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

		// Token: 0x06005BBC RID: 23484 RVA: 0x002A9418 File Offset: 0x002A7618
		private void SetHeightByRowCount(int rowCount)
		{
			bool flag = rowCount <= 0;
			if (!flag)
			{
				float targetHeight = 72f + 66f * (float)(rowCount - 1 + 1);
				RectTransform rectTransform = base.GetComponent<RectTransform>();
				bool flag2 = rectTransform;
				if (flag2)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
				}
			}
		}

		// Token: 0x06005BBD RID: 23485 RVA: 0x002A9462 File Offset: 0x002A7662
		private void OnOpenAllClick()
		{
			this.SetAllValues(true);
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x002A946D File Offset: 0x002A766D
		private void OnCloseAllClick()
		{
			this.SetAllValues(false);
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x002A9478 File Offset: 0x002A7678
		private void OnToggleChanged(int newIndex, int oldIndex)
		{
			bool isUpdating = this._isUpdating;
			if (!isUpdating)
			{
				bool flag = newIndex >= 0 && newIndex < this._values.Length;
				if (flag)
				{
					this._values[newIndex] = true;
				}
				bool flag2 = oldIndex >= 0 && oldIndex < this._values.Length;
				if (flag2)
				{
					this._values[oldIndex] = false;
				}
				this.SyncToAiOptions();
				base.NotifyChanged();
			}
		}

		// Token: 0x06005BC0 RID: 23488 RVA: 0x002A94E0 File Offset: 0x002A76E0
		private void SetAllValues(bool value)
		{
			this._isUpdating = true;
			for (int i = 0; i < this._values.Length; i++)
			{
				this._values[i] = value;
			}
			bool flag = this.toggleGroup != null;
			if (flag)
			{
				List<CToggle> toggles = this.toggleGroup.GetAll();
				int j = 0;
				while (j < toggles.Count && j < this._values.Length)
				{
					if (value)
					{
						this.toggleGroup.SelectWithoutNotify(j);
					}
					else
					{
						this.toggleGroup.DeSelectWithoutNotify(j);
					}
					j++;
				}
			}
			this._isUpdating = false;
			this.SyncToAiOptions();
			base.NotifyChanged();
		}

		// Token: 0x06005BC1 RID: 23489 RVA: 0x002A959C File Offset: 0x002A779C
		private void SyncToAiOptions()
		{
			bool flag = this._aiOptions == null || this._aiOptions.AutoUseTeammateCommand == null;
			if (!flag)
			{
				int i = 0;
				while (i < this._values.Length && i < this._aiOptions.AutoUseTeammateCommand.Length)
				{
					this._aiOptions.AutoUseTeammateCommand[i] = this._values[i];
					i++;
				}
			}
		}

		// Token: 0x06005BC2 RID: 23490 RVA: 0x002A960A File Offset: 0x002A780A
		public override object GetValue()
		{
			return this._values;
		}

		// Token: 0x06005BC3 RID: 23491 RVA: 0x002A9614 File Offset: 0x002A7814
		public override void SetValue(object value)
		{
			bool[] boolArray = value as bool[];
			bool flag = boolArray != null;
			if (flag)
			{
				this._isUpdating = true;
				int i = 0;
				while (i < boolArray.Length && i < this._values.Length)
				{
					this._values[i] = boolArray[i];
					i++;
				}
				bool flag2 = this.toggleGroup != null;
				if (flag2)
				{
					List<CToggle> toggles = this.toggleGroup.GetAll();
					int j = 0;
					while (j < toggles.Count && j < this._values.Length)
					{
						bool flag3 = this._values[j];
						if (flag3)
						{
							this.toggleGroup.SelectWithoutNotify(j);
						}
						else
						{
							this.toggleGroup.DeSelectWithoutNotify(j);
						}
						j++;
					}
				}
				this._isUpdating = false;
				this.SyncToAiOptions();
			}
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x002A96F3 File Offset: 0x002A78F3
		public void SetTypedValue(bool[] values)
		{
			this.SetValue(values);
		}

		// Token: 0x06005BC5 RID: 23493 RVA: 0x002A9700 File Offset: 0x002A7900
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.openAllBtn != null;
			if (flag)
			{
				this.openAllBtn.interactable = interactable;
			}
			bool flag2 = this.closeAllBtn != null;
			if (flag2)
			{
				this.closeAllBtn.interactable = interactable;
			}
			bool flag3 = this.toggleGroup != null;
			if (flag3)
			{
				List<CToggle> toggles = this.toggleGroup.GetAll();
				foreach (CToggle toggle in toggles)
				{
					toggle.interactable = interactable;
				}
			}
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x002A97B0 File Offset: 0x002A79B0
		private void OnDestroy()
		{
			bool flag = this.openAllBtn != null;
			if (flag)
			{
				this.openAllBtn.onClick.RemoveListener(new UnityAction(this.OnOpenAllClick));
			}
			bool flag2 = this.closeAllBtn != null;
			if (flag2)
			{
				this.closeAllBtn.onClick.RemoveListener(new UnityAction(this.OnCloseAllClick));
			}
			bool flag3 = this.toggleGroup != null;
			if (flag3)
			{
				this.toggleGroup.OnActiveIndexChange -= this.OnToggleChanged;
			}
		}

		// Token: 0x04003F41 RID: 16193
		[SerializeField]
		private CButton openAllBtn;

		// Token: 0x04003F42 RID: 16194
		[SerializeField]
		private CButton closeAllBtn;

		// Token: 0x04003F43 RID: 16195
		[SerializeField]
		private CToggleGroupMultiSelect toggleGroup;

		// Token: 0x04003F44 RID: 16196
		[SerializeField]
		private CToggle toggleTemplate;

		// Token: 0x04003F45 RID: 16197
		private const float MaxRowWidth = 824f;

		// Token: 0x04003F46 RID: 16198
		private const float SingleLineHeight = 72f;

		// Token: 0x04003F47 RID: 16199
		private const float ExtraLineHeight = 66f;

		// Token: 0x04003F48 RID: 16200
		private const float HorizontalSpacing = 8f;

		// Token: 0x04003F49 RID: 16201
		private const float VerticalSpacing = 8f;

		// Token: 0x04003F4A RID: 16202
		private bool[] _values;

		// Token: 0x04003F4B RID: 16203
		private List<string> _optionNames = new List<string>();

		// Token: 0x04003F4C RID: 16204
		private TeammateCommandSettingAttribute _attr;

		// Token: 0x04003F4D RID: 16205
		private AiOptions _aiOptions;

		// Token: 0x04003F4E RID: 16206
		private bool _isUpdating;
	}
}
