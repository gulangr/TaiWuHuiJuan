using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Creation;
using GameData.Domains.Item;
using GameData.Domains.World;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000809 RID: 2057
	public class NewGameSubPageFeature : NewGameSubPage
	{
		// Token: 0x060064E1 RID: 25825 RVA: 0x002E1DB0 File Offset: 0x002DFFB0
		protected override void Awake()
		{
			base.Awake();
			this.LoadFeatures();
			bool flag = !this._poolInitialized;
			if (flag)
			{
				this.InitializePool();
				this._poolInitialized = true;
			}
			bool flag2 = this.featureItemPrefab != null;
			if (flag2)
			{
				this.featureItemPrefab.gameObject.SetActive(false);
			}
			bool flag3 = this.clearButton != null;
			if (flag3)
			{
				this.clearButton.ClearAndAddListener(new Action(this.OnClearSelectedFeatures));
			}
		}

		// Token: 0x060064E2 RID: 25826 RVA: 0x002E1E38 File Offset: 0x002E0038
		public override void Init()
		{
			bool flag = this._allFeatures.Count == 0;
			if (flag)
			{
				this.LoadFeatures();
			}
			bool flag2 = !this._poolInitialized;
			if (flag2)
			{
				this.InitializePool();
				this._poolInitialized = true;
			}
			this._selectedFeatures.Clear();
			this._selectedCustomItems.Clear();
			string idListString;
			bool flag3 = base.CreationInfoMap.TryGetValue("ProtagonistFeatureIds", out idListString) && !string.IsNullOrEmpty(idListString);
			if (flag3)
			{
				List<short> ids = idListString.Split('|', StringSplitOptions.None).Select(new Func<string, short>(short.Parse)).ToList<short>();
				using (List<short>.Enumerator enumerator = ids.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						short id = enumerator.Current;
						ProtagonistFeatureItem feature = this._allFeatures.Find((ProtagonistFeatureItem f) => f.TemplateId == id);
						bool flag4 = feature != null;
						if (flag4)
						{
							this._selectedFeatures.Add(feature);
						}
						List<TemplateKey> customItems = this.LoadCustomItemsFromCreationInfo(id);
						bool flag5 = customItems != null;
						if (flag5)
						{
							this._selectedCustomItems[id] = customItems;
						}
					}
				}
			}
			this.OnEnable();
		}

		// Token: 0x060064E3 RID: 25827 RVA: 0x002E1F98 File Offset: 0x002E0198
		private void OnEnable()
		{
			this._itemSelectionChecked = false;
			this.RefreshUI();
		}

		// Token: 0x060064E4 RID: 25828 RVA: 0x002E1FAC File Offset: 0x002E01AC
		private void LoadFeatures()
		{
			this._allFeatures.Clear();
			List<short> allKeys = ProtagonistFeature.Instance.GetAllKeys();
			foreach (short key in allKeys)
			{
				this._allFeatures.Add(ProtagonistFeature.Instance[key]);
			}
		}

		// Token: 0x060064E5 RID: 25829 RVA: 0x002E2028 File Offset: 0x002E0228
		private void InitializePool()
		{
			foreach (ProtagonistFeatureItem feature in this._allFeatures)
			{
				NewGameFeatureItem cell = Object.Instantiate<NewGameFeatureItem>(this.featureItemPrefab, this.featureCategoryContainers[(int)feature.Type]);
				cell.gameObject.SetActive(false);
				this._featureItemPool[feature.TemplateId] = cell;
			}
		}

		// Token: 0x060064E6 RID: 25830 RVA: 0x002E20B4 File Offset: 0x002E02B4
		private void RefreshUI()
		{
			this.UpdateData();
			this.UpdateUI();
		}

		// Token: 0x060064E7 RID: 25831 RVA: 0x002E20C8 File Offset: 0x002E02C8
		private void UpdateData()
		{
			Array.Clear(this._spentPoints, 0, this._spentPoints.Length);
			this.ValidateSelection();
			this.CleanupCustomItems();
			foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
			{
				this._spentPoints[(int)feature.Type] += (int)feature.Cost;
			}
			this._spentPoints[3] = this._spentPoints[0] + this._spentPoints[1] + this._spentPoints[2];
			base.RefreshDisableEnterGameReason();
		}

		// Token: 0x060064E8 RID: 25832 RVA: 0x002E2180 File Offset: 0x002E0380
		private void ValidateSelection()
		{
			bool changed = true;
			while (changed)
			{
				changed = false;
				List<ProtagonistFeatureItem> toRemove = new List<ProtagonistFeatureItem>();
				int[] tempPoints = new int[3];
				this._selectedFeatures.Sort(delegate(ProtagonistFeatureItem a, ProtagonistFeatureItem b)
				{
					bool flag2 = a.Type != b.Type;
					int result;
					if (flag2)
					{
						result = (int)(a.Type - b.Type);
					}
					else
					{
						bool flag3 = a.PrerequisiteCost != b.PrerequisiteCost;
						if (flag3)
						{
							result = (int)(a.PrerequisiteCost - b.PrerequisiteCost);
						}
						else
						{
							result = (int)(a.Cost - b.Cost);
						}
					}
					return result;
				});
				foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
				{
					bool flag = (int)feature.PrerequisiteCost > tempPoints[(int)feature.Type];
					if (flag)
					{
						toRemove.Add(feature);
						changed = true;
					}
					else
					{
						tempPoints[(int)feature.Type] += (int)feature.Cost;
					}
				}
				foreach (ProtagonistFeatureItem item in toRemove)
				{
					this._selectedFeatures.Remove(item);
				}
			}
		}

		// Token: 0x060064E9 RID: 25833 RVA: 0x002E22AC File Offset: 0x002E04AC
		private void CleanupCustomItems()
		{
			foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
			{
				List<TemplateKey> currentItems;
				bool flag = !this._selectedCustomItems.TryGetValue(feature.TemplateId, out currentItems);
				if (!flag)
				{
					List<TemplateKey> sanitizedItems = this.SanitizeCustomItems(feature, currentItems);
					bool flag2 = sanitizedItems != null && sanitizedItems.Count > 0;
					if (flag2)
					{
						bool flag3 = !sanitizedItems.SequenceEqual(currentItems);
						if (flag3)
						{
							this._selectedCustomItems[feature.TemplateId] = sanitizedItems;
							this.SaveCustomItemsToCreationInfo(feature.TemplateId, sanitizedItems);
						}
					}
					else
					{
						this.ClearCustomItems(feature.TemplateId);
					}
				}
			}
		}

		// Token: 0x060064EA RID: 25834 RVA: 0x002E2388 File Offset: 0x002E0588
		private List<TemplateKey> SanitizeCustomItems(ProtagonistFeatureItem feature, List<TemplateKey> items)
		{
			bool flag = items == null || items.Count <= 0;
			List<TemplateKey> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<TemplateKey>[] groups = feature.CustomGroupItem;
				bool flag2;
				if (groups != null)
				{
					flag2 = !groups.Any((List<TemplateKey> group) => group != null && group.Count > 0);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					result = null;
				}
				else
				{
					int[] counts = feature.CustomGroupCount;
					int[] selectedCounts = new int[groups.Length];
					List<TemplateKey> sanitizedItems = new List<TemplateKey>();
					foreach (TemplateKey item in items)
					{
						int i = 0;
						while (i < groups.Length)
						{
							bool flag4 = groups[i] == null || groups[i].Count == 0 || !groups[i].Contains(item);
							if (flag4)
							{
								i++;
							}
							else
							{
								int maxCount = (counts != null && i < counts.Length) ? counts[i] : 1;
								bool flag5 = selectedCounts[i] >= maxCount;
								if (flag5)
								{
									break;
								}
								selectedCounts[i]++;
								sanitizedItems.Add(item);
								break;
							}
						}
					}
					result = sanitizedItems;
				}
			}
			return result;
		}

		// Token: 0x060064EB RID: 25835 RVA: 0x002E24E0 File Offset: 0x002E06E0
		private void ClearCustomItems(short featureId)
		{
			this._selectedCustomItems.Remove(featureId);
			this.SaveCustomItemsToCreationInfo(featureId, null);
		}

		// Token: 0x060064EC RID: 25836 RVA: 0x002E24FC File Offset: 0x002E06FC
		private void UpdateUI()
		{
			bool flag = this.toggleCategoryPointTexts != null;
			if (flag)
			{
				int i = 0;
				while (i < 3 && i < this.toggleCategoryPointTexts.Length)
				{
					bool flag2 = this.toggleCategoryPointTexts[i] != null;
					if (flag2)
					{
						this.toggleCategoryPointTexts[i].text = this._spentPoints[i].ToString();
					}
					i++;
				}
			}
			bool flag3 = this.pageCategoryPointTexts != null;
			if (flag3)
			{
				int j = 0;
				while (j < 3 && j < this.pageCategoryPointTexts.Length)
				{
					bool flag4 = this.pageCategoryPointTexts[j] != null;
					if (flag4)
					{
						this.pageCategoryPointTexts[j].text = this._spentPoints[j].ToString();
					}
					j++;
				}
			}
			bool flag5 = this.remainingPointsText != null;
			if (flag5)
			{
				this.remainingPointsText.text = LocalStringManager.GetFormat(LanguageKey.UI_NewGame_BornAbility_LeftPoint, this._spentPoints[3], this.MaxPoints).ColorReplace();
			}
			List<ProtagonistFeatureItem> sortedFeatures = (from f in this._allFeatures
			orderby f.TemplateId
			select f).ToList<ProtagonistFeatureItem>();
			List<ProtagonistFeatureItem> sortedSelectedFeatures = (from f in this._selectedFeatures
			orderby f.Type, f.PrerequisiteCost, f.Cost
			select f).ToList<ProtagonistFeatureItem>();
			foreach (ProtagonistFeatureItem feature in sortedFeatures)
			{
				NewGameFeatureItem cell;
				bool flag6 = !this._featureItemPool.TryGetValue(feature.TemplateId, out cell);
				if (!flag6)
				{
					bool isSelected = this._selectedFeatures.Contains(feature);
					Transform targetContainer = isSelected ? this.selectedFeatureContainer : this.featureCategoryContainers[(int)feature.Type];
					bool flag7 = cell.transform.parent != targetContainer;
					if (flag7)
					{
						cell.transform.SetParent(targetContainer);
					}
					bool flag8 = !isSelected;
					if (flag8)
					{
						cell.transform.SetAsLastSibling();
					}
					else
					{
						int sortedIndex = sortedSelectedFeatures.IndexOf(feature);
						cell.transform.SetSiblingIndex(sortedIndex);
					}
					bool flag9;
					if (feature.CustomGroupItem != null)
					{
						flag9 = feature.CustomGroupItem.Any((List<TemplateKey> g) => g != null && g.Count > 0);
					}
					else
					{
						flag9 = false;
					}
					bool hasCustomItems = flag9;
					bool canSelect = isSelected || this.CheckCanSelect(feature);
					List<TemplateKey> customItems = this.GetFeatureCustomItems(feature.TemplateId);
					bool customItemsSelected = hasCustomItems && NewGameSubPageFeature.IsCustomItemsFilled(feature, customItems);
					cell.Init(feature, isSelected, canSelect, this._spentPoints, this.MaxPoints, new Action<NewGameFeatureItem>(this.OnFeatureClick), (isSelected && hasCustomItems) ? new Action<ProtagonistFeatureItem>(this.OnSelectCustomItemClick) : null, customItemsSelected);
					cell.gameObject.SetActive(true);
				}
			}
			this.UpdateClearButtonVisibility();
		}

		// Token: 0x060064ED RID: 25837 RVA: 0x002E2890 File Offset: 0x002E0A90
		private int GetSelectedFeatureContainerActiveCount()
		{
			bool flag = this.selectedFeatureContainer == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				for (int i = 0; i < this.selectedFeatureContainer.childCount; i++)
				{
					bool activeSelf = this.selectedFeatureContainer.GetChild(i).gameObject.activeSelf;
					if (activeSelf)
					{
						count++;
					}
				}
				result = count;
			}
			return result;
		}

		// Token: 0x060064EE RID: 25838 RVA: 0x002E28F8 File Offset: 0x002E0AF8
		private void UpdateClearButtonVisibility()
		{
			bool flag = this.clearButton == null;
			if (!flag)
			{
				this.clearButton.gameObject.SetActive(this.GetSelectedFeatureContainerActiveCount() > 1);
			}
		}

		// Token: 0x060064EF RID: 25839 RVA: 0x002E2934 File Offset: 0x002E0B34
		private void OnClearSelectedFeatures()
		{
			foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
			{
				this.ClearCustomItems(feature.TemplateId);
			}
			this._selectedFeatures.Clear();
			base.CreationInfoMap.Remove("ProtagonistFeatureIds");
			this._itemSelectionChecked = false;
			this.RefreshUI();
		}

		// Token: 0x060064F0 RID: 25840 RVA: 0x002E29BC File Offset: 0x002E0BBC
		private bool CheckCanSelect(ProtagonistFeatureItem feature)
		{
			bool flag = (int)feature.PrerequisiteCost > this._spentPoints[(int)feature.Type];
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = (int)feature.Cost > this.MaxPoints - this._spentPoints[3];
				result = !flag2;
			}
			return result;
		}

		// Token: 0x060064F1 RID: 25841 RVA: 0x002E2A0C File Offset: 0x002E0C0C
		private void OnFeatureClick(NewGameFeatureItem feature)
		{
			bool flag = this._selectedFeatures.Contains(feature.Data);
			if (flag)
			{
				this._selectedFeatures.Remove(feature.Data);
			}
			else
			{
				bool flag2 = this.CheckCanSelect(feature.Data);
				if (flag2)
				{
					this._selectedFeatures.Add(feature.Data);
				}
			}
			this.RefreshUI();
		}

		// Token: 0x060064F2 RID: 25842 RVA: 0x002E2A72 File Offset: 0x002E0C72
		private static string GetFeatureCustomItemsKey(short featureId)
		{
			return string.Format("FeatureCustomItems_{0}", featureId);
		}

		// Token: 0x060064F3 RID: 25843 RVA: 0x002E2A84 File Offset: 0x002E0C84
		private List<TemplateKey> LoadCustomItemsFromCreationInfo(short featureId)
		{
			string key = NewGameSubPageFeature.GetFeatureCustomItemsKey(featureId);
			string value;
			bool flag = !base.CreationInfoMap.TryGetValue(key, out value) || string.IsNullOrEmpty(value);
			List<TemplateKey> result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				List<TemplateKey> result = new List<TemplateKey>();
				string[] parts = value.Split('|', StringSplitOptions.None);
				foreach (string part in parts)
				{
					uint uintValue;
					bool flag2 = uint.TryParse(part, out uintValue);
					if (flag2)
					{
						result.Add((TemplateKey)uintValue);
					}
				}
				result2 = ((result.Count > 0) ? result : null);
			}
			return result2;
		}

		// Token: 0x060064F4 RID: 25844 RVA: 0x002E2B20 File Offset: 0x002E0D20
		private void SaveCustomItemsToCreationInfo(short featureId, List<TemplateKey> items)
		{
			string key = NewGameSubPageFeature.GetFeatureCustomItemsKey(featureId);
			bool flag = items == null || items.Count == 0;
			if (flag)
			{
				base.CreationInfoMap.Remove(key);
			}
			else
			{
				base.CreationInfoMap[key] = string.Join("|", items.ConvertAll<string>((TemplateKey k) => ((uint)k).ToString()));
			}
		}

		// Token: 0x060064F5 RID: 25845 RVA: 0x002E2B94 File Offset: 0x002E0D94
		private void OnSelectCustomItemClick(ProtagonistFeatureItem feature)
		{
			bool flag = this._selectedCustomItems.ContainsKey(feature.TemplateId);
			List<TemplateKey> currentItems;
			if (flag)
			{
				currentItems = this._selectedCustomItems[feature.TemplateId];
			}
			else
			{
				currentItems = this.LoadCustomItemsFromCreationInfo(feature.TemplateId);
				bool flag2 = currentItems != null;
				if (flag2)
				{
					this._selectedCustomItems[feature.TemplateId] = currentItems;
				}
			}
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("Feature", feature).SetObject("Selection", currentItems).SetObject("Callback", new Action<List<TemplateKey>>(delegate(List<TemplateKey> items)
			{
				bool flag3 = items == null || items.Count == 0;
				if (flag3)
				{
					this.ClearCustomItems(feature.TemplateId);
				}
				else
				{
					this._selectedCustomItems[feature.TemplateId] = items;
					this.SaveCustomItemsToCreationInfo(feature.TemplateId, items);
				}
				this.RefreshUI();
			}));
			UIElement.NewGameFeatureItemSelection.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.NewGameFeatureItemSelection);
		}

		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x060064F6 RID: 25846 RVA: 0x002E2C76 File Offset: 0x002E0E76
		public override string DisableEnterGameReason
		{
			get
			{
				return (this._spentPoints[3] > this.MaxPoints) ? LanguageKey.UI_NewGame_CreateTip_AbilityPointError.Tr() : "";
			}
		}

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x060064F7 RID: 25847 RVA: 0x002E2C9C File Offset: 0x002E0E9C
		public override DialogCmd StartGameCheck
		{
			get
			{
				bool flag = this._spentPoints[3] > this.MaxPoints;
				DialogCmd result;
				if (flag)
				{
					result = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
						Content = LocalStringManager.Get(LanguageKey.UI_NewGame_CreateTip_AbilityPointError),
						Type = 2,
						Yes = new Action(base.FocusToPage),
						No = new Action(base.FocusToPage)
					};
				}
				else
				{
					bool flag2 = !this._itemSelectionChecked && this.HasUnfilledCustomItems();
					if (flag2)
					{
						result = new DialogCmd
						{
							Title = LocalStringManager.Get(LanguageKey.LK_NewGame_Feature_ItemNotFull_Title),
							Content = LocalStringManager.Get(LanguageKey.LK_NewGame_Feature_ItemNotFull_Content),
							Type = 1,
							Yes = delegate()
							{
								this._itemSelectionChecked = true;
								this.parent.TryStartNewGame();
							},
							No = delegate()
							{
								this.parent.ResetChecks();
								base.FocusToPage();
							}
						};
					}
					else
					{
						bool flag3 = this._spentPoints[3] < this.MaxPoints;
						if (flag3)
						{
							result = new DialogCmd
							{
								Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
								Content = LocalStringManager.Get(LanguageKey.UI_NewGame_CreateTip_AbilityPoint),
								Type = 1,
								Yes = delegate()
								{
									this.StartGameChecked = true;
								},
								No = delegate()
								{
									this.parent.ResetChecks();
								}
							};
						}
						else
						{
							result = null;
						}
					}
				}
				return result;
			}
		}

		// Token: 0x060064F8 RID: 25848 RVA: 0x002E2DEC File Offset: 0x002E0FEC
		private List<TemplateKey> GetFeatureCustomItems(short featureId)
		{
			List<TemplateKey> items;
			bool flag = this._selectedCustomItems.TryGetValue(featureId, out items);
			List<TemplateKey> result;
			if (flag)
			{
				result = items;
			}
			else
			{
				result = this.LoadCustomItemsFromCreationInfo(featureId);
			}
			return result;
		}

		// Token: 0x060064F9 RID: 25849 RVA: 0x002E2E1C File Offset: 0x002E101C
		private static bool IsCustomItemsFilled(ProtagonistFeatureItem feature, List<TemplateKey> selectedItems)
		{
			List<TemplateKey>[] groups = feature.CustomGroupItem;
			int[] counts = feature.CustomGroupCount;
			bool flag = groups == null || groups.Length == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = selectedItems == null || selectedItems.Count == 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < groups.Length; i++)
					{
						bool flag3 = groups[i] == null || groups[i].Count == 0;
						if (!flag3)
						{
							int maxCount = (counts != null && i < counts.Length) ? counts[i] : 1;
							int selectedCount = 0;
							foreach (TemplateKey item in selectedItems)
							{
								bool flag4 = groups[i].Contains(item);
								if (flag4)
								{
									selectedCount++;
								}
							}
							bool flag5 = selectedCount < maxCount;
							if (flag5)
							{
								return false;
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060064FA RID: 25850 RVA: 0x002E2F34 File Offset: 0x002E1134
		private bool HasUnfilledCustomItems()
		{
			foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
			{
				bool flag = !NewGameSubPageFeature.IsCustomItemsFilled(feature, this.GetFeatureCustomItems(feature.TemplateId));
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x060064FB RID: 25851 RVA: 0x002E2FA8 File Offset: 0x002E11A8
		// (set) Token: 0x060064FC RID: 25852 RVA: 0x002E2FB0 File Offset: 0x002E11B0
		public override bool StartGameChecked
		{
			get
			{
				return this._checked;
			}
			set
			{
				this._checked = value;
				bool @checked = this._checked;
				if (@checked)
				{
					this.parent.ContinueStartNewGame();
				}
				else
				{
					this._itemSelectionChecked = false;
				}
			}
		}

		// Token: 0x060064FD RID: 25853 RVA: 0x002E2FE4 File Offset: 0x002E11E4
		private void FillUnfilledCustomItems()
		{
			foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
			{
				List<TemplateKey>[] groups = feature.CustomGroupItem;
				int[] counts = feature.CustomGroupCount;
				bool flag = groups == null;
				if (!flag)
				{
					List<TemplateKey> selectedItems;
					bool flag2 = !this._selectedCustomItems.TryGetValue(feature.TemplateId, out selectedItems);
					if (flag2)
					{
						selectedItems = new List<TemplateKey>();
						this._selectedCustomItems[feature.TemplateId] = selectedItems;
					}
					for (int i = 0; i < groups.Length; i++)
					{
						bool flag3 = groups[i] == null || groups[i].Count == 0;
						if (!flag3)
						{
							int maxCount = (counts != null && i < counts.Length) ? counts[i] : 1;
							int selectedCount = 0;
							foreach (TemplateKey item in selectedItems)
							{
								bool flag4 = groups[i].Contains(item);
								if (flag4)
								{
									selectedCount++;
								}
							}
							bool flag5 = selectedCount < maxCount;
							if (flag5)
							{
								List<TemplateKey> availableItems = new List<TemplateKey>();
								foreach (TemplateKey item2 in groups[i])
								{
									bool flag6 = !selectedItems.Contains(item2);
									if (flag6)
									{
										availableItems.Add(item2);
									}
								}
								int needed = maxCount - selectedCount;
								int j = 0;
								while (j < needed && availableItems.Count > 0)
								{
									int randomIndex = Random.Range(0, availableItems.Count);
									selectedItems.Add(availableItems[randomIndex]);
									availableItems.RemoveAt(randomIndex);
									j++;
								}
							}
						}
					}
					this.SaveCustomItemsToCreationInfo(feature.TemplateId, selectedItems);
				}
			}
		}

		// Token: 0x060064FE RID: 25854 RVA: 0x002E3238 File Offset: 0x002E1438
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			bool flag = protagonistCreationInfo == null;
			if (!flag)
			{
				this.CleanupCustomItems();
				this.FillUnfilledCustomItems();
				protagonistCreationInfo.ProtagonistFeatureIds = (from f in this._selectedFeatures
				select f.TemplateId).ToList<short>();
				protagonistCreationInfo.ProtagonistCustomItems = new Dictionary<short, DataList<TemplateKey>>();
				foreach (ProtagonistFeatureItem feature in this._selectedFeatures)
				{
					List<TemplateKey> items;
					bool flag2 = this._selectedCustomItems.TryGetValue(feature.TemplateId, out items) && items != null && items.Count > 0;
					if (flag2)
					{
						protagonistCreationInfo.ProtagonistCustomItems[feature.TemplateId] = new DataList<TemplateKey>(items);
					}
				}
				bool flag3 = this._selectedFeatures.Count > 0;
				if (flag3)
				{
					base.CreationInfoMap["ProtagonistFeatureIds"] = string.Join<short>("|", protagonistCreationInfo.ProtagonistFeatureIds);
				}
				else
				{
					base.CreationInfoMap.Remove("ProtagonistFeatureIds");
				}
			}
		}

		// Token: 0x04004641 RID: 17985
		[SerializeField]
		private CButton clearButton;

		// Token: 0x04004642 RID: 17986
		[Header("UI Containers")]
		[SerializeField]
		private Transform[] featureCategoryContainers;

		// Token: 0x04004643 RID: 17987
		[SerializeField]
		private Transform selectedFeatureContainer;

		// Token: 0x04004644 RID: 17988
		[SerializeField]
		private NewGameFeatureItem featureItemPrefab;

		// Token: 0x04004645 RID: 17989
		[Header("Points UI")]
		[SerializeField]
		private TextMeshProUGUI remainingPointsText;

		// Token: 0x04004646 RID: 17990
		[SerializeField]
		private TextMeshProUGUI[] toggleCategoryPointTexts;

		// Token: 0x04004647 RID: 17991
		[SerializeField]
		private TextMeshProUGUI[] pageCategoryPointTexts;

		// Token: 0x04004648 RID: 17992
		[Header("Config")]
		public int MaxPoints = 15;

		// Token: 0x04004649 RID: 17993
		private List<ProtagonistFeatureItem> _allFeatures = new List<ProtagonistFeatureItem>();

		// Token: 0x0400464A RID: 17994
		private List<ProtagonistFeatureItem> _selectedFeatures = new List<ProtagonistFeatureItem>();

		// Token: 0x0400464B RID: 17995
		private Dictionary<short, List<TemplateKey>> _selectedCustomItems = new Dictionary<short, List<TemplateKey>>();

		// Token: 0x0400464C RID: 17996
		private int[] _spentPoints = new int[4];

		// Token: 0x0400464D RID: 17997
		private bool _itemSelectionChecked;

		// Token: 0x0400464E RID: 17998
		private Dictionary<short, NewGameFeatureItem> _featureItemPool = new Dictionary<short, NewGameFeatureItem>();

		// Token: 0x0400464F RID: 17999
		private bool _poolInitialized;

		// Token: 0x04004650 RID: 18000
		private bool _checked;
	}
}
