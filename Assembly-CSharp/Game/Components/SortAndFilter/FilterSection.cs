using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C9E RID: 3230
	[RequireComponent(typeof(CToggleGroup))]
	public class FilterSection : MonoBehaviour
	{
		// Token: 0x17001126 RID: 4390
		// (get) Token: 0x0600A450 RID: 42064 RVA: 0x004CBE16 File Offset: 0x004CA016
		// (set) Token: 0x0600A451 RID: 42065 RVA: 0x004CBE1E File Offset: 0x004CA01E
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x17001127 RID: 4391
		// (get) Token: 0x0600A452 RID: 42066 RVA: 0x004CBE27 File Offset: 0x004CA027
		public int MenuId
		{
			get
			{
				return this._menuId;
			}
		}

		// Token: 0x17001128 RID: 4392
		// (get) Token: 0x0600A453 RID: 42067 RVA: 0x004CBE30 File Offset: 0x004CA030
		private CToggleGroup ToggleGroup
		{
			get
			{
				CToggleGroup result;
				if ((result = this._toggleGroup) == null)
				{
					result = (this._toggleGroup = base.GetComponent<CToggleGroup>());
				}
				return result;
			}
		}

		// Token: 0x0600A454 RID: 42068 RVA: 0x004CBE58 File Offset: 0x004CA058
		public void Setup(int menuId, string title, List<FilterDropdownItemConfig> itemConfigs, Action<int> onSelectionChanged)
		{
			this._menuId = menuId;
			this._itemConfigs = itemConfigs;
			this._onSelectionChanged = onSelectionChanged;
			this.CacheConfig(title, itemConfigs);
			this.titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
			this.titleText.SetText(title, true);
			this.RefreshOptions();
		}

		// Token: 0x0600A455 RID: 42069 RVA: 0x004CBEB4 File Offset: 0x004CA0B4
		public void SetSelectionChangedCallback(Action<int> onSelectionChanged)
		{
			this._onSelectionChanged = onSelectionChanged;
		}

		// Token: 0x0600A456 RID: 42070 RVA: 0x004CBEC0 File Offset: 0x004CA0C0
		public bool IsSameConfig(int menuId, string title, List<FilterDropdownItemConfig> itemConfigs)
		{
			bool flag = this._menuId != menuId;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string normalizedTitle = title ?? string.Empty;
				bool flag2 = !string.Equals(this._cachedTitle, normalizedTitle, StringComparison.Ordinal);
				if (flag2)
				{
					result = false;
				}
				else
				{
					int itemCount = (itemConfigs != null) ? itemConfigs.Count : 0;
					bool flag3 = this._cachedOptionTexts.Count != itemCount;
					if (flag3)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < itemCount; i++)
						{
							string targetText = itemConfigs[i].Text.GetString();
							bool flag4 = !string.Equals(this._cachedOptionTexts[i], targetText, StringComparison.Ordinal);
							if (flag4)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A457 RID: 42071 RVA: 0x004CBF8C File Offset: 0x004CA18C
		public void SetSelectedIndex(int index, bool notify = true)
		{
			int targetToggleIndex = index + 1;
			bool flag = targetToggleIndex < 0 || targetToggleIndex >= this._options.Count;
			if (flag)
			{
				index = -1;
				targetToggleIndex = 0;
			}
			this.SelectedIndex = index;
			if (notify)
			{
				this.ToggleGroup.Set(targetToggleIndex, false);
			}
			else
			{
				this._suppressSelectionChangedCallback = true;
				this.ToggleGroup.SetWithoutNotify(targetToggleIndex);
				this._suppressSelectionChangedCallback = false;
			}
		}

		// Token: 0x0600A458 RID: 42072 RVA: 0x004CC000 File Offset: 0x004CA200
		public List<int> GetSelectedIndices()
		{
			List<int> result;
			if (this.SelectedIndex >= 0)
			{
				(result = new List<int>()).Add(this.SelectedIndex);
			}
			else
			{
				result = new List<int>();
			}
			return result;
		}

		// Token: 0x0600A459 RID: 42073 RVA: 0x004CC034 File Offset: 0x004CA234
		public void SetVisible(bool isVisible)
		{
			base.gameObject.SetActive(isVisible);
		}

		// Token: 0x0600A45A RID: 42074 RVA: 0x004CC044 File Offset: 0x004CA244
		public void ClearSelection()
		{
			this.SetSelectedIndex(-1, true);
		}

		// Token: 0x0600A45B RID: 42075 RVA: 0x004CC050 File Offset: 0x004CA250
		public RectTransform GetContentRoot()
		{
			return this.optionRoot;
		}

		// Token: 0x0600A45C RID: 42076 RVA: 0x004CC068 File Offset: 0x004CA268
		public void SetOptionInteractable(int optionIndex, bool interactable, string disabledTooltip)
		{
			int toggleIndex = optionIndex + 1;
			bool flag = toggleIndex < 0 || toggleIndex >= this._options.Count;
			if (!flag)
			{
				this._options[toggleIndex].SetInteractable(interactable, disabledTooltip);
			}
		}

		// Token: 0x0600A45D RID: 42077 RVA: 0x004CC0AC File Offset: 0x004CA2AC
		public void RefreshOptionCounts(int[] counts)
		{
			int i = 0;
			while (i < this._options.Count && i < counts.Length)
			{
				this._options[i].SetCount(counts[i]);
				i++;
			}
		}

		// Token: 0x0600A45E RID: 42078 RVA: 0x004CC0F4 File Offset: 0x004CA2F4
		private void RefreshOptions()
		{
			bool flag = this.optionRoot == null || this.optionTemplate == null || this.ToggleGroup == null;
			if (!flag)
			{
				this.ToggleGroup.Clear();
				this._options.Clear();
				List<FilterDropdownItemConfig> itemConfigs = this._itemConfigs;
				int totalCount = ((itemConfigs != null) ? itemConfigs.Count : 0) + 1;
				CommonUtils.PrepareEnoughChildren(this.optionRoot, this.optionTemplate.gameObject, totalCount, null);
				this.CreateAllOption();
				int i = 0;
				for (;;)
				{
					int num = i;
					List<FilterDropdownItemConfig> itemConfigs2 = this._itemConfigs;
					if (num >= ((itemConfigs2 != null) ? itemConfigs2.Count : 0))
					{
						break;
					}
					this.CreateOption(i, this._itemConfigs[i]);
					i++;
				}
				this.RegisterOptionsToToggleGroup();
				this.ToggleGroup.Init(-1);
				this.SetSelectedIndex(-1, false);
			}
		}

		// Token: 0x0600A45F RID: 42079 RVA: 0x004CC1E4 File Offset: 0x004CA3E4
		private void CreateAllOption()
		{
			FilterSectionOption allOption = this.optionRoot.GetChild(0).GetComponent<FilterSectionOption>();
			allOption.Refresh(LocalStringManager.Get(LanguageKey.LK_Common_All), false);
			allOption.Toggle.onValueChanged.RemoveAllListeners();
			allOption.Toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.OnOptionSelected(-1);
				}
			});
			this._options.Add(allOption);
		}

		// Token: 0x0600A460 RID: 42080 RVA: 0x004CC254 File Offset: 0x004CA454
		private void CreateOption(int index, FilterDropdownItemConfig config)
		{
			int childIndex = index + 1;
			FilterSectionOption option = this.optionRoot.GetChild(childIndex).GetComponent<FilterSectionOption>();
			option.Refresh(config.Text.GetString(), true);
			option.Toggle.onValueChanged.RemoveAllListeners();
			int capturedIndex = index;
			option.Toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.OnOptionSelected(capturedIndex);
				}
			});
			this._options.Add(option);
		}

		// Token: 0x0600A461 RID: 42081 RVA: 0x004CC2DC File Offset: 0x004CA4DC
		private void RegisterOptionsToToggleGroup()
		{
			foreach (FilterSectionOption option in this._options)
			{
				bool flag = option == null;
				if (!flag)
				{
					this.ToggleGroup.Add(option.Toggle);
				}
			}
		}

		// Token: 0x0600A462 RID: 42082 RVA: 0x004CC34C File Offset: 0x004CA54C
		private void OnOptionSelected(int index)
		{
			this.SelectedIndex = index;
			bool suppressSelectionChangedCallback = this._suppressSelectionChangedCallback;
			if (!suppressSelectionChangedCallback)
			{
				Action<int> onSelectionChanged = this._onSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged(index);
				}
			}
		}

		// Token: 0x0600A463 RID: 42083 RVA: 0x004CC384 File Offset: 0x004CA584
		private void CacheConfig(string title, List<FilterDropdownItemConfig> itemConfigs)
		{
			this._cachedTitle = (title ?? string.Empty);
			this._cachedOptionTexts.Clear();
			bool flag = itemConfigs == null;
			if (!flag)
			{
				foreach (FilterDropdownItemConfig config in itemConfigs)
				{
					List<string> cachedOptionTexts = this._cachedOptionTexts;
					StringKey text = config.Text;
					cachedOptionTexts.Add(text.GetString());
				}
			}
		}

		// Token: 0x0600A464 RID: 42084 RVA: 0x004CC410 File Offset: 0x004CA610
		private void OnDestroy()
		{
			bool flag = this.ToggleGroup != null;
			if (flag)
			{
				this.ToggleGroup.Clear();
			}
			this._options.Clear();
		}

		// Token: 0x04008218 RID: 33304
		private CToggleGroup _toggleGroup;

		// Token: 0x04008219 RID: 33305
		private readonly List<FilterSectionOption> _options = new List<FilterSectionOption>();

		// Token: 0x0400821A RID: 33306
		private List<FilterDropdownItemConfig> _itemConfigs;

		// Token: 0x0400821B RID: 33307
		private Action<int> _onSelectionChanged;

		// Token: 0x0400821C RID: 33308
		private int _menuId;

		// Token: 0x0400821D RID: 33309
		private bool _suppressSelectionChangedCallback;

		// Token: 0x0400821E RID: 33310
		private string _cachedTitle = string.Empty;

		// Token: 0x0400821F RID: 33311
		private readonly List<string> _cachedOptionTexts = new List<string>();

		// Token: 0x04008220 RID: 33312
		[Header("UI组件")]
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04008221 RID: 33313
		[SerializeField]
		private RectTransform optionRoot;

		// Token: 0x04008222 RID: 33314
		[SerializeField]
		private FilterSectionOption optionTemplate;
	}
}
