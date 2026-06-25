using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CC7 RID: 3271
	public class SortButtonGroup : MonoBehaviour, ISortUiWithDisplayingSortIds, ISortUi
	{
		// Token: 0x17001142 RID: 4418
		// (get) Token: 0x0600A57A RID: 42362 RVA: 0x004D1EE2 File Offset: 0x004D00E2
		public HashSet<short> DisplayingSortIds { get; } = new HashSet<short>();

		// Token: 0x0600A57B RID: 42363 RVA: 0x004D1EEC File Offset: 0x004D00EC
		public void Setup(SortUiConfig uiConfig, Action onSortChanged, Action onSortMenuShow, Action onSortMenuHide)
		{
			this._config = uiConfig;
			this._onSortChanged = onSortChanged;
			this._isInitialized = true;
			this._itemStates.Clear();
			this.DisplayingSortIds.Clear();
			this._sortIds.Clear();
			bool flag = this._config.FixedSortId != null;
			if (flag)
			{
				this._sortIds.AddRange(this._config.FixedSortId);
			}
			bool flag2 = this._config.SortIds != null;
			if (flag2)
			{
				this._sortIds.AddRange(this._config.SortIds);
			}
			bool flag3 = uiConfig.DefaultSortState.ItemStates != null;
			if (flag3)
			{
				foreach (SortItemState state in uiConfig.DefaultSortState.ItemStates)
				{
					this.AddOrReplaceState(state);
				}
			}
			this.RefreshDisplayingSortIds();
			this.RebuildItems();
			this.CleanupInvalidStates();
			this.RefreshAll();
		}

		// Token: 0x0600A57C RID: 42364 RVA: 0x004D200C File Offset: 0x004D020C
		public void Setup(IEnumerable<short> sortIdsUsedByTableHead)
		{
			this._isInitialized = true;
			this._itemStates.Clear();
			this.DisplayingSortIds.Clear();
			this._additionalSortIds = sortIdsUsedByTableHead;
			this._sortIds.Clear();
			bool flag = this._config.FixedSortId != null;
			if (flag)
			{
				this._sortIds.AddRange(this._config.FixedSortId);
			}
			bool flag2 = this._additionalSortIds != null;
			if (flag2)
			{
				this._sortIds.AddRange(from t in this._additionalSortIds
				where t >= 0
				select t);
			}
			this._sortIds = this._sortIds.Distinct<short>().ToList<short>();
			this.RefreshDisplayingSortIds();
			this.RebuildItems();
			this.CleanupInvalidStates();
			this.RefreshAll();
		}

		// Token: 0x0600A57D RID: 42365 RVA: 0x004D20F0 File Offset: 0x004D02F0
		public SortStateData GetSortData()
		{
			SortStateData sortStateData = new SortStateData();
			sortStateData.ItemStates = (from item in this._itemStates
			select new SortItemState
			{
				SortId = item.SortId,
				SortDirection = item.SortDirection
			}).ToList<SortItemState>();
			return sortStateData;
		}

		// Token: 0x0600A57E RID: 42366 RVA: 0x004D213C File Offset: 0x004D033C
		public void SetSortData(SortStateData data)
		{
			this._itemStates.Clear();
			bool flag = ((data != null) ? data.ItemStates : null) != null;
			if (flag)
			{
				foreach (SortItemState state in data.ItemStates)
				{
					this.AddOrReplaceState(state);
				}
			}
			this.CleanupInvalidStates();
			this.RefreshAll();
		}

		// Token: 0x0600A57F RID: 42367 RVA: 0x004D21C4 File Offset: 0x004D03C4
		public void RefreshDisplayOptions<TData>(List<FilterLineBase<TData>> filterLines, List<LineState> lineStates)
		{
			this.DisplayingSortIds.Clear();
			this.CollectDisplayingSortIds<TData>(filterLines, lineStates);
			bool flag = this.DisplayingSortIds.Count == 0;
			if (flag)
			{
				this.RefreshDisplayingSortIds();
			}
			this.CleanupInvalidStates();
			this.RefreshAll();
		}

		// Token: 0x0600A580 RID: 42368 RVA: 0x004D2210 File Offset: 0x004D0410
		public void Clear()
		{
			this._itemStates.Clear();
			bool flag = this._config.DefaultSortState.ItemStates != null;
			if (flag)
			{
				foreach (SortItemState state in this._config.DefaultSortState.ItemStates)
				{
					this.AddOrReplaceState(state);
				}
			}
			this.CleanupInvalidStates();
			this.RefreshAll();
			Action onSortChanged = this._onSortChanged;
			if (onSortChanged != null)
			{
				onSortChanged();
			}
		}

		// Token: 0x0600A581 RID: 42369 RVA: 0x004D22B8 File Offset: 0x004D04B8
		private void RefreshDisplayingSortIds()
		{
			this.DisplayingSortIds.Clear();
			bool flag = this._additionalSortIds != null;
			if (flag)
			{
				foreach (short item in this._additionalSortIds)
				{
					this.DisplayingSortIds.Add(item);
				}
			}
			bool flag2 = this._config.FixedSortId != null && this._config.FixedSortId.Count > 0;
			if (flag2)
			{
				foreach (short id in this._config.FixedSortId)
				{
					this.DisplayingSortIds.Add(id);
				}
			}
			else
			{
				bool flag3 = this._config.DefaultSortIds != null && this._config.DefaultSortIds.Count > 0;
				if (flag3)
				{
					foreach (short id2 in this._config.DefaultSortIds)
					{
						this.DisplayingSortIds.Add(id2);
					}
				}
				else
				{
					bool flag4 = this._sortIds == null || this._sortIds.Count == 0;
					if (!flag4)
					{
						foreach (short id3 in this._sortIds)
						{
							this.DisplayingSortIds.Add(id3);
						}
					}
				}
			}
		}

		// Token: 0x0600A582 RID: 42370 RVA: 0x004D2498 File Offset: 0x004D0698
		private void CollectDisplayingSortIds<TData>(List<FilterLineBase<TData>> filterLines, List<LineState> lineStates)
		{
			bool flag = this._config.FilterTypeDic == null;
			if (!flag)
			{
				for (int i = 0; i < filterLines.Count; i++)
				{
					bool flag2 = i >= lineStates.Count || lineStates[i].Type > ESortAndFilterOneLineType.ToggleGroup;
					if (!flag2)
					{
						List<short> sortIds;
						bool flag3 = this._config.FilterTypeDic.TryGetValue(new ValueTuple<int, int>(filterLines[i].Id, lineStates[i].ToggleGroupState.Index), out sortIds);
						if (flag3)
						{
							foreach (short id in sortIds)
							{
								this.DisplayingSortIds.Add(id);
							}
						}
						List<short> parentSortIds;
						bool flag4 = lineStates[i].ToggleGroupState.Index != -1 && this._config.FilterTypeDic.TryGetValue(new ValueTuple<int, int>(filterLines[i].Id, -1), out parentSortIds);
						if (flag4)
						{
							foreach (short id2 in parentSortIds)
							{
								this.DisplayingSortIds.Add(id2);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A583 RID: 42371 RVA: 0x004D2618 File Offset: 0x004D0818
		private void RebuildItems()
		{
			bool flag = this.itemRoot == null || this.itemTemplate == null || this._sortIds == null || this._sortIds.Count == 0;
			if (flag)
			{
				CommonUtils.PrepareEnoughChildren(this.itemRoot, this.itemTemplate.gameObject, 0, null);
			}
			else
			{
				CommonUtils.PrepareEnoughChildren(this.itemRoot, this.itemTemplate.gameObject, this._sortIds.Count, null);
				for (int i = 0; i < this._sortIds.Count; i++)
				{
					int index = i;
					SortButtonGroupItem item = this.itemRoot.GetChild(i).GetComponent<SortButtonGroupItem>();
					item.gameObject.SetActive(true);
					item.SetClickHandler(delegate
					{
						this.OnClickItem(index);
					});
				}
			}
		}

		// Token: 0x0600A584 RID: 42372 RVA: 0x004D2718 File Offset: 0x004D0918
		private void OnClickItem(int index)
		{
			bool flag = !this._isInitialized;
			if (!flag)
			{
				bool flag2 = this._sortIds == null || index < 0 || index >= this._sortIds.Count;
				if (!flag2)
				{
					short sortId = this._sortIds[index];
					bool flag3 = this.DisplayingSortIds.Count != 0 && !this.DisplayingSortIds.Contains(sortId);
					if (!flag3)
					{
						int oldStateIndex = this._itemStates.FindIndex((SortItemState item) => item.SortId == sortId);
						ESortDirection oldDirection = (oldStateIndex >= 0) ? this._itemStates[oldStateIndex].SortDirection : ESortDirection.None;
						this._itemStates.Clear();
						switch (oldDirection)
						{
						case ESortDirection.None:
							this.AddOrReplaceState(new SortItemState
							{
								SortId = sortId,
								SortDirection = ESortDirection.Descending
							});
							break;
						case ESortDirection.Ascending:
							break;
						case ESortDirection.Descending:
							this.AddOrReplaceState(new SortItemState
							{
								SortId = sortId,
								SortDirection = ESortDirection.Ascending
							});
							break;
						default:
							this.AddOrReplaceState(new SortItemState
							{
								SortId = sortId,
								SortDirection = ESortDirection.Descending
							});
							break;
						}
						this.CleanupInvalidStates();
						this.RefreshAll();
						Action onSortChanged = this._onSortChanged;
						if (onSortChanged != null)
						{
							onSortChanged();
						}
					}
				}
			}
		}

		// Token: 0x0600A585 RID: 42373 RVA: 0x004D28A0 File Offset: 0x004D0AA0
		private void AddOrReplaceState(SortItemState state)
		{
			bool flag = state.SortDirection == ESortDirection.None;
			if (!flag)
			{
				int stateIndex = this._itemStates.FindIndex((SortItemState item) => item.SortId == state.SortId);
				bool flag2 = stateIndex >= 0;
				if (flag2)
				{
					this._itemStates[stateIndex] = state;
				}
				else
				{
					this._itemStates.Add(state);
				}
			}
		}

		// Token: 0x0600A586 RID: 42374 RVA: 0x004D2919 File Offset: 0x004D0B19
		private void CleanupInvalidStates()
		{
			this._itemStates.RemoveAll((SortItemState item) => item.SortDirection == ESortDirection.None);
		}

		// Token: 0x0600A587 RID: 42375 RVA: 0x004D2948 File Offset: 0x004D0B48
		private void RefreshAll()
		{
			bool flag = this._sortIds == null || this._sortIds.Count == 0 || this.itemRoot == null;
			if (!flag)
			{
				for (int i = 0; i < this._sortIds.Count; i++)
				{
					short sortId = this._sortIds[i];
					bool flag2 = sortId < 0;
					if (!flag2)
					{
						SortButtonGroupItem item = this.itemRoot.GetChild(i).GetComponent<SortButtonGroupItem>();
						bool isVisible = this.DisplayingSortIds.Count == 0 || this.DisplayingSortIds.Contains(sortId);
						item.gameObject.SetActive(isVisible);
						bool flag3 = !isVisible;
						if (!flag3)
						{
							int nameIndex = (this._config.SortNameIndexList != null && i < this._config.SortNameIndexList.Count) ? this._config.SortNameIndexList[i] : 0;
							string labelText = SortItem.Instance[sortId].Names[nameIndex];
							int matchedIndex = this._itemStates.FindIndex((SortItemState itemState) => itemState.SortId == sortId);
							ESortDirection direction = (matchedIndex >= 0) ? this._itemStates[matchedIndex].SortDirection : ESortDirection.None;
							int order = (matchedIndex >= 0) ? (matchedIndex + 1) : 0;
							item.Refresh(labelText, direction, order);
						}
					}
				}
			}
		}

		// Token: 0x040082AA RID: 33450
		private readonly List<SortItemState> _itemStates = new List<SortItemState>();

		// Token: 0x040082AB RID: 33451
		private SortUiConfig _config;

		// Token: 0x040082AC RID: 33452
		private Action _onSortChanged;

		// Token: 0x040082AD RID: 33453
		private bool _isInitialized;

		// Token: 0x040082AF RID: 33455
		private IEnumerable<short> _additionalSortIds;

		// Token: 0x040082B0 RID: 33456
		private List<short> _sortIds = new List<short>();

		// Token: 0x040082B1 RID: 33457
		[SerializeField]
		private RectTransform itemRoot;

		// Token: 0x040082B2 RID: 33458
		[SerializeField]
		private SortButtonGroupItem itemTemplate;
	}
}
