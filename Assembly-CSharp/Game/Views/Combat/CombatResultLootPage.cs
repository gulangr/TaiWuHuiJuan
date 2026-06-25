using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Views.Select;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000AF0 RID: 2800
	public class CombatResultLootPage : MonoBehaviour
	{
		// Token: 0x06008996 RID: 35222 RVA: 0x003FABC0 File Offset: 0x003F8DC0
		public void Init(CombatResultDisplayData displayData, Dictionary<ItemKey, int> selectedItems, Action<bool> onSelectAllChanged, int[] currentResourcesArray, int exp, int dept)
		{
			this._displayData = displayData;
			this._selectedItems = selectedItems;
			this._onSelectAllChanged = onSelectAllChanged;
			this._currentResourcesArray = currentResourcesArray;
			this._curExp = exp;
			this._curDept = dept;
			this.RefreshEvaluations();
			this.RefreshBonusStats();
			this.RefreshResources();
			this.RefreshLegacies();
			this.RefreshCharacters();
			bool flag = this._sortAndFilterController == null;
			if (flag)
			{
				this._sortAndFilterController = new SortOnlyItemController(this.sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey);
				this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), "CombatResultLoot");
			}
			this.InitItemScroll();
			this._selectedItems.Clear();
			bool flag2 = this._displayData.ItemList != null;
			if (flag2)
			{
				foreach (ItemDisplayData item in this._displayData.ItemList)
				{
					this._selectedItems[item.Key] = item.Amount;
				}
			}
			this.RefreshItems();
			this.InitSelectAllToggle();
		}

		// Token: 0x06008997 RID: 35223 RVA: 0x003FACF0 File Offset: 0x003F8EF0
		private void RefreshEvaluations()
		{
			List<sbyte> evaluationList = this._displayData.EvaluationList;
			int count = (evaluationList != null) ? evaluationList.Count : 0;
			CommonUtils.PrepareEnoughChildren(this.evaluationContainer, this.evaluationItemPrefab, count, null);
			for (int i = 0; i < count; i++)
			{
				Transform child = this.evaluationContainer.GetChild(i);
				CombatEvaluationItem config = CombatEvaluation.Instance[this._displayData.EvaluationList[i]];
				TextMeshProUGUI label = child.GetComponentInChildren<TextMeshProUGUI>();
				label.text = config.Name;
				TooltipInvoker tip;
				bool flag = child.TryGetComponent<TooltipInvoker>(out tip);
				if (flag)
				{
					tip.PresetParam = new string[]
					{
						config.Name,
						CombatResultLootPage.GetEvaluationDesc(config)
					};
				}
			}
		}

		// Token: 0x06008998 RID: 35224 RVA: 0x003FADBC File Offset: 0x003F8FBC
		private static string GetEvaluationDesc(CombatEvaluationItem config)
		{
			string desc = config.Desc;
			bool flag = config.ExpAddPercent != 0;
			if (flag)
			{
				desc += string.Format("\n{0}{1}{2}%", LocalStringManager.Get(LanguageKey.LK_Exp), (config.ExpAddPercent > 0) ? "+" : "", config.ExpAddPercent);
			}
			bool flag2 = config.ExpTotalPercent != 0;
			if (flag2)
			{
				desc += string.Format("\n{0}×{1}%", LocalStringManager.Get(LanguageKey.LK_Exp), Mathf.Max((int)(100 + config.ExpTotalPercent), 0));
			}
			bool flag3 = config.AuthorityAddPercent != 0;
			if (flag3)
			{
				desc += string.Format("\n{0}{1}{2}%", LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority), (config.AuthorityAddPercent > 0) ? "+" : "", config.AuthorityAddPercent);
			}
			bool flag4 = config.AuthorityTotalPercent != 0;
			if (flag4)
			{
				desc += string.Format("\n{0}×{1}%", LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority), Mathf.Max((int)(100 + config.AuthorityTotalPercent), 0));
			}
			return desc;
		}

		// Token: 0x06008999 RID: 35225 RVA: 0x003FAEE4 File Offset: 0x003F90E4
		private void RefreshBonusStats()
		{
			bool flag = this._displayData.EvaluationList == null || !this._displayData.EvaluationList.Any<sbyte>();
			if (flag)
			{
				this.bonusArea.SetActive(false);
			}
			else
			{
				int expAddSum;
				int expTotal;
				this._displayData.GetExpBonusStats(out expAddSum, out expTotal);
				int authorityAddSum;
				int authorityTotal;
				this._displayData.GetAuthorityBonusStats(out authorityAddSum, out authorityTotal);
				bool hasBonus = expAddSum != 0 || expTotal != 0 || authorityAddSum != 0 || authorityTotal != 0;
				this.bonusArea.SetActive(hasBonus);
				bool flag2 = hasBonus;
				if (flag2)
				{
					this.expBonusLabel.text = CombatResultLootPage.FormatBonus(expAddSum, expTotal);
					this.authorityBonusLabel.text = CombatResultLootPage.FormatBonus(authorityAddSum, authorityTotal);
				}
			}
		}

		// Token: 0x0600899A RID: 35226 RVA: 0x003FAF9C File Offset: 0x003F919C
		private static string FormatBonus(int addPercent, int totalPercent)
		{
			int finalPercent = (100 + addPercent) * (100 + totalPercent) / 100 - 100;
			bool flag = finalPercent == 0;
			string result;
			if (flag)
			{
				result = "-";
			}
			else
			{
				result = string.Format("{0}{1}%</color>", (finalPercent > 0) ? "<color=#8DC3C3>+" : "<color=#ec5f68>", finalPercent);
			}
			return result;
		}

		// Token: 0x0600899B RID: 35227 RVA: 0x003FAFF0 File Offset: 0x003F91F0
		private void RefreshResources()
		{
			int dynamicIndex = 0;
			for (sbyte i = 0; i < 8; i += 1)
			{
				int value = this._displayData.Resource.Get((int)i);
				bool flag = value <= 0;
				if (!flag)
				{
					Transform itemTrans = null;
					while (dynamicIndex < this.resourceContainer.childCount)
					{
						Transform child = this.resourceContainer.GetChild(dynamicIndex);
						bool flag2 = child.gameObject != this.expResourceObj && child.gameObject != this.debtResourceObj;
						if (flag2)
						{
							itemTrans = child;
							break;
						}
						dynamicIndex++;
					}
					bool flag3 = itemTrans == null;
					if (flag3)
					{
						itemTrans = Object.Instantiate<GameObject>(this.resourceItemPrefab, this.resourceContainer).transform;
					}
					itemTrans.SetSiblingIndex(dynamicIndex);
					Transform transform = itemTrans.Find("Icon");
					CImage icon = (transform != null) ? transform.GetComponent<CImage>() : null;
					Transform transform2 = itemTrans.Find("Name");
					TextMeshProUGUI nameLabel = (transform2 != null) ? transform2.GetComponent<TextMeshProUGUI>() : null;
					Transform transform3 = itemTrans.Find("Value");
					TextMeshProUGUI valueLabel = (transform3 != null) ? transform3.GetComponent<TextMeshProUGUI>() : null;
					int baseResource = this._currentResourcesArray[(int)i] - value;
					bool flag4 = icon != null;
					if (flag4)
					{
						icon.SetSprite(ResourceType.Instance[i].Icon, false, null);
					}
					bool flag5 = nameLabel != null;
					if (flag5)
					{
						nameLabel.text = ResourceType.Instance[i].Name;
					}
					bool flag6 = valueLabel != null;
					if (flag6)
					{
						valueLabel.text = CommonUtils.GetDisplayStringForNum(baseResource, 100000) + "<color=#8DC3C3>+" + CommonUtils.GetDisplayStringForNum(value, 100000) + "</color>";
					}
					itemTrans.gameObject.SetActive(true);
					dynamicIndex++;
				}
			}
			for (int j = dynamicIndex; j < this.resourceContainer.childCount; j++)
			{
				Transform child2 = this.resourceContainer.GetChild(j);
				bool flag7 = child2.gameObject != this.expResourceObj && child2.gameObject != this.debtResourceObj;
				if (flag7)
				{
					child2.gameObject.SetActive(false);
				}
			}
			bool flag8 = this.expResourceObj != null;
			if (flag8)
			{
				bool show = this._displayData.Exp > 0;
				this.expResourceObj.SetActive(show);
				bool flag9 = show;
				if (flag9)
				{
					int baseResource2 = this._curExp - this._displayData.Exp;
					Transform transform4 = this.expResourceObj.transform.Find("Icon");
					CImage cimage = (transform4 != null) ? transform4.GetComponent<CImage>() : null;
					Transform transform5 = this.expResourceObj.transform.Find("Value");
					TextMeshProUGUI valueLabel2 = (transform5 != null) ? transform5.GetComponent<TextMeshProUGUI>() : null;
					bool flag10 = valueLabel2 != null;
					if (flag10)
					{
						valueLabel2.text = CommonUtils.GetDisplayStringForNum(baseResource2, 100000) + "<color=#8DC3C3>+" + CommonUtils.GetDisplayStringForNum(this._displayData.Exp, 100000) + "</color>";
					}
					this.expResourceObj.transform.SetAsFirstSibling();
				}
			}
			bool flag11 = this.debtResourceObj != null;
			if (flag11)
			{
				bool show2 = this._displayData.AreaSpiritualDebt > 0;
				this.debtResourceObj.SetActive(show2);
				this.emptyForLayout.SetActive(show2);
				bool flag12 = show2;
				if (flag12)
				{
					int baseResource3 = this._curDept - this._displayData.AreaSpiritualDebt;
					Transform transform6 = this.debtResourceObj.transform.Find("Value");
					TextMeshProUGUI valueLabel3 = (transform6 != null) ? transform6.GetComponent<TextMeshProUGUI>() : null;
					bool flag13 = valueLabel3 != null;
					if (flag13)
					{
						valueLabel3.text = CommonUtils.GetDisplayStringForNum(baseResource3, 100000) + "<color=#8DC3C3>+" + CommonUtils.GetDisplayStringForNum(this._displayData.AreaSpiritualDebt, 100000) + "</color>";
					}
					this.debtResourceObj.transform.SetAsLastSibling();
				}
			}
			bool hasResource = this._displayData.Resource.IsNonZero() || this._displayData.Exp > 0 || this._displayData.AreaSpiritualDebt > 0;
			this.resourceArea.SetActive(hasResource);
		}

		// Token: 0x0600899C RID: 35228 RVA: 0x003FB448 File Offset: 0x003F9648
		private void RefreshLegacies()
		{
			List<short> legacyTemplateIds = this._displayData.LegacyTemplateIds;
			int count = (legacyTemplateIds != null) ? legacyTemplateIds.Count : 0;
			bool flag;
			if (count > 0)
			{
				flag = this._displayData.LegacyTemplateIds.All((short id) => id >= 0);
			}
			else
			{
				flag = false;
			}
			bool showLegacy = flag;
			this.legacyArea.SetActive(showLegacy);
			bool flag2 = !showLegacy;
			if (!flag2)
			{
				CommonUtils.PrepareEnoughChildren(this.legacyContainer, this.legacyItemPrefab, count, null);
				for (int i = 0; i < count; i++)
				{
					Transform child = this.legacyContainer.GetChild(i);
					short legacyId = this._displayData.LegacyTemplateIds[i];
					LegacyItem legacyConfig = Legacy.Instance[legacyId];
					Transform transform = child.Find("Icon");
					CImage icon = (transform != null) ? transform.GetComponent<CImage>() : null;
					Transform transform2 = child.Find("GradeIcon");
					CImage gradeIcon = (transform2 != null) ? transform2.GetComponent<CImage>() : null;
					Transform transform3 = child.Find("ItemName");
					TextMeshProUGUI name = (transform3 != null) ? transform3.GetComponent<TextMeshProUGUI>() : null;
					bool flag3 = icon != null;
					if (flag3)
					{
						icon.SetSprite(Legacy.Instance[legacyId].Icon, false, null);
					}
					bool flag4 = gradeIcon != null;
					if (flag4)
					{
						gradeIcon.SetSprite("ui9_icon_item_grade_" + legacyConfig.Grade.ToString(), false, null);
					}
					bool flag5 = name != null;
					if (flag5)
					{
						name.text = Legacy.Instance[legacyId].Name;
					}
					TooltipInvoker tip;
					bool flag6 = child.TryGetComponent<TooltipInvoker>(out tip);
					if (flag6)
					{
						tip.Type = TipType.Simple;
						TooltipInvoker tooltipInvoker = tip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						tip.RuntimeParam.Set("arg0", legacyConfig.Name);
						tip.RuntimeParam.Set("arg1", legacyConfig.Desc);
					}
				}
				for (int j = count; j < this.legacyContainer.childCount; j++)
				{
					this.legacyContainer.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600899D RID: 35229 RVA: 0x003FB698 File Offset: 0x003F9898
		private void RefreshCharacters()
		{
			List<CharacterDisplayData> charList = this._displayData.CharList;
			int count = (charList != null) ? charList.Count : 0;
			this.charArea.SetActive(count > 0);
			bool flag = count == 0;
			if (!flag)
			{
				for (int i = 0; i < count; i++)
				{
					bool flag2 = i >= this.charContainer.childCount;
					AvatarWithName avatar;
					if (flag2)
					{
						avatar = Object.Instantiate<AvatarWithName>(this.avatarWithNamePrefab, this.charContainer);
					}
					else
					{
						avatar = this.charContainer.GetChild(i).GetComponent<AvatarWithName>();
					}
					CharacterDisplayData charData = this._displayData.CharList[i];
					string displayName = NameCenter.GetNameByDisplayData(charData, false, false);
					avatar.Set(charData.AvatarRelatedData, charData.TemplateId, displayName, charData.CharacterId, null, false, false);
					avatar.gameObject.SetActive(true);
				}
				for (int j = count; j < this.charContainer.childCount; j++)
				{
					this.charContainer.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600899E RID: 35230 RVA: 0x003FB7B8 File Offset: 0x003F99B8
		private void InitItemScroll()
		{
			bool flag = this.itemScroll == null;
			if (!flag)
			{
				ColumnDefinition nameColumn = SelectItemColumnHelper.CreateIconAndNameColumn();
				nameColumn.TableHeadLabel = (() => LanguageKey.LK_ItemName.Tr());
				List<ColumnDefinition> columns = new List<ColumnDefinition>
				{
					nameColumn,
					SelectItemColumnHelper.CreateAmountColumn(),
					SelectItemColumnHelper.CreateValueColumn(),
					SelectItemColumnHelper.CreateWeightColumn()
				};
				this.itemScroll.Init<ItemDisplayData>(columns, true, new Action<int, GameObject>(this.OnItemRender), null);
				this.itemScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsItemSelected);
				this.itemScroll.OnRowClicked -= this.OnRowClicked;
				this.itemScroll.OnRowClicked += this.OnRowClicked;
				this.itemScroll.SetSortController(this._sortAndFilterController);
			}
		}

		// Token: 0x0600899F RID: 35231 RVA: 0x003FB8B0 File Offset: 0x003F9AB0
		private void OnItemRender(int index, GameObject rowObject)
		{
			bool flag = index < 0 || index >= this._filteredData.Count;
			if (!flag)
			{
				RowItem rowItem = rowObject.GetComponent<RowItem>();
				bool flag2 = rowItem == null || rowItem.TipDisplayer == null;
				if (!flag2)
				{
					ITradeableContent itemData = this._filteredData[index];
					RowItemLine.SetMouseTipDisplayer(true, itemData, rowItem.TipDisplayer);
				}
			}
		}

		// Token: 0x060089A0 RID: 35232 RVA: 0x003FB91C File Offset: 0x003F9B1C
		private void OnSortAndFilterChanged()
		{
			this.RefreshItems();
		}

		// Token: 0x060089A1 RID: 35233 RVA: 0x003FB928 File Offset: 0x003F9B28
		private bool IsItemSelected(int index, object dataObj)
		{
			ItemDisplayData data = dataObj as ItemDisplayData;
			bool flag = data != null;
			return flag && this._selectedItems.ContainsKey(data.Key);
		}

		// Token: 0x060089A2 RID: 35234 RVA: 0x003FB960 File Offset: 0x003F9B60
		private void OnRowClicked(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._filteredData.Count;
			if (!flag)
			{
				ITradeableContent data = this._filteredData[index];
				bool flag2 = this._selectedItems.ContainsKey(data.Key);
				if (flag2)
				{
					this._selectedItems.Remove(data.Key);
				}
				else
				{
					this._selectedItems.Add(data.Key, data.Amount);
				}
				this.itemScroll.InfiniteScroll.ReRender();
				this.UpdateSelectAllToggleState();
			}
		}

		// Token: 0x060089A3 RID: 35235 RVA: 0x003FB9F8 File Offset: 0x003F9BF8
		private void UpdateSelectAllToggleState()
		{
			bool isAllSelected = this._displayData.ItemList != null && this._displayData.ItemList.Count > 0 && this._selectedItems.Count == this._displayData.ItemList.Count;
			this.selectAllToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnSelectAllToggleChanged));
			this.selectAllToggle.isOn = isAllSelected;
			this.selectAllToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSelectAllToggleChanged));
		}

		// Token: 0x060089A4 RID: 35236 RVA: 0x003FBA90 File Offset: 0x003F9C90
		private void RefreshItems()
		{
			bool flag = this._displayData.ItemList == null || this._displayData.ItemList.Count == 0;
			if (flag)
			{
				this.itemScroll.gameObject.SetActive(false);
				this.sortAndFilter.gameObject.SetActive(false);
			}
			else
			{
				this.itemScroll.gameObject.SetActive(true);
				this.sortAndFilter.gameObject.SetActive(true);
				this._filteredData.Clear();
				List<ItemDisplayData> sourceData = this._displayData.ItemList;
				bool flag2 = this._sortAndFilterController != null;
				if (flag2)
				{
					Func<ITradeableContent, bool> filter = this._sortAndFilterController.GenerateFilter();
					foreach (ItemDisplayData item in sourceData)
					{
						bool flag3 = filter(item);
						if (flag3)
						{
							this._filteredData.Add(item);
						}
					}
					Comparison<ITradeableContent> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
					this._filteredData.Sort(comparer);
				}
				else
				{
					this._filteredData.AddRange(sourceData);
				}
				this.itemScroll.SetData<ITradeableContent>(this._filteredData, -1);
			}
		}

		// Token: 0x060089A5 RID: 35237 RVA: 0x003FBBE8 File Offset: 0x003F9DE8
		private void InitSelectAllToggle()
		{
			this.selectAllToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnSelectAllToggleChanged));
			bool hasItems = this._displayData.ItemList != null && this._displayData.ItemList.Count > 0;
			this.selectAllToggle.isOn = hasItems;
			this.selectAllToggle.interactable = hasItems;
			this.selectAllToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSelectAllToggleChanged));
		}

		// Token: 0x060089A6 RID: 35238 RVA: 0x003FBC70 File Offset: 0x003F9E70
		private void OnSelectAllToggleChanged(bool isOn)
		{
			Action<bool> onSelectAllChanged = this._onSelectAllChanged;
			if (onSelectAllChanged != null)
			{
				onSelectAllChanged(isOn);
			}
			bool flag = this.itemScroll != null && this._displayData.ItemList != null;
			if (flag)
			{
				if (isOn)
				{
					foreach (ItemDisplayData item in this._displayData.ItemList)
					{
						bool flag2 = !this._selectedItems.ContainsKey(item.Key);
						if (flag2)
						{
							this._selectedItems.Add(item.Key, item.Amount);
						}
					}
				}
				else
				{
					this._selectedItems.Clear();
				}
				this.itemScroll.InfiniteScroll.ReRender();
			}
		}

		// Token: 0x060089A7 RID: 35239 RVA: 0x003FBD5C File Offset: 0x003F9F5C
		public bool IsSelectAll()
		{
			return this.selectAllToggle.isOn;
		}

		// Token: 0x060089A8 RID: 35240 RVA: 0x003FBD6C File Offset: 0x003F9F6C
		public void SetSelectAllState(bool selectAll)
		{
			bool flag = this.selectAllToggle.isOn != selectAll;
			if (flag)
			{
				this.selectAllToggle.isOn = selectAll;
			}
		}

		// Token: 0x060089A9 RID: 35241 RVA: 0x003FBD9E File Offset: 0x003F9F9E
		private void OnDestroy()
		{
			this.selectAllToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnSelectAllToggleChanged));
		}

		// Token: 0x04006975 RID: 26997
		[Header("战斗评价")]
		[SerializeField]
		private Transform evaluationContainer;

		// Token: 0x04006976 RID: 26998
		[SerializeField]
		private GameObject evaluationItemPrefab;

		// Token: 0x04006977 RID: 26999
		[Header("加成统计")]
		[SerializeField]
		private GameObject bonusArea;

		// Token: 0x04006978 RID: 27000
		[SerializeField]
		private TextMeshProUGUI expBonusLabel;

		// Token: 0x04006979 RID: 27001
		[SerializeField]
		private TextMeshProUGUI authorityBonusLabel;

		// Token: 0x0400697A RID: 27002
		[SerializeField]
		private GameObject emptyForLayout;

		// Token: 0x0400697B RID: 27003
		[Header("获得资源")]
		[SerializeField]
		private GameObject resourceArea;

		// Token: 0x0400697C RID: 27004
		[SerializeField]
		private Transform resourceContainer;

		// Token: 0x0400697D RID: 27005
		[SerializeField]
		private GameObject resourceItemPrefab;

		// Token: 0x0400697E RID: 27006
		[SerializeField]
		private GameObject expResourceObj;

		// Token: 0x0400697F RID: 27007
		[SerializeField]
		private GameObject debtResourceObj;

		// Token: 0x04006980 RID: 27008
		[Header("获得遗惠")]
		[SerializeField]
		private GameObject legacyArea;

		// Token: 0x04006981 RID: 27009
		[SerializeField]
		private Transform legacyContainer;

		// Token: 0x04006982 RID: 27010
		[SerializeField]
		private GameObject legacyItemPrefab;

		// Token: 0x04006983 RID: 27011
		[Header("人物获得")]
		[SerializeField]
		private GameObject charArea;

		// Token: 0x04006984 RID: 27012
		[SerializeField]
		private Transform charContainer;

		// Token: 0x04006985 RID: 27013
		[SerializeField]
		private AvatarWithName avatarWithNamePrefab;

		// Token: 0x04006986 RID: 27014
		[Header("物品列表")]
		[SerializeField]
		private ListStyleGeneralScroll itemScroll;

		// Token: 0x04006987 RID: 27015
		[SerializeField]
		private CToggle selectAllToggle;

		// Token: 0x04006988 RID: 27016
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04006989 RID: 27017
		private CombatResultDisplayData _displayData;

		// Token: 0x0400698A RID: 27018
		private Dictionary<ItemKey, int> _selectedItems;

		// Token: 0x0400698B RID: 27019
		private Action<bool> _onSelectAllChanged;

		// Token: 0x0400698C RID: 27020
		private ItemSortAndFilterController _sortAndFilterController;

		// Token: 0x0400698D RID: 27021
		private readonly List<ITradeableContent> _filteredData = new List<ITradeableContent>();

		// Token: 0x0400698E RID: 27022
		private int[] _currentResourcesArray;

		// Token: 0x0400698F RID: 27023
		private int _curExp;

		// Token: 0x04006990 RID: 27024
		private int _curDept;
	}
}
