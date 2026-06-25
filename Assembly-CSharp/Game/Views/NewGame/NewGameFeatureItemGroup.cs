using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007E9 RID: 2025
	public class NewGameFeatureItemGroup : MonoBehaviour
	{
		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x060062AC RID: 25260 RVA: 0x002D2C03 File Offset: 0x002D0E03
		public ViewNewGameFeatureItemSelection ParentView
		{
			get
			{
				return this._parentView;
			}
		}

		// Token: 0x060062AD RID: 25261 RVA: 0x002D2C0C File Offset: 0x002D0E0C
		public void Init(string groupName, int maxCount, List<TemplateKey> allChoices, List<TemplateKey> currentSelection, ViewNewGameFeatureItemSelection parentView)
		{
			this._groupName = groupName;
			this._maxCount = maxCount;
			this._allChoices = allChoices;
			this._parentView = parentView;
			CommonUtils.PrepareEnoughChildren(this.itemGrid, this.itemCellPrefab.gameObject, allChoices.Count, null);
			for (int i = 0; i < allChoices.Count; i++)
			{
				NewGameFeatureItemCell cell = this.itemGrid.GetChild(i).GetComponent<NewGameFeatureItemCell>();
				TemplateKey itemKey = allChoices[i];
				int count = this.CountItemInSelection(itemKey, currentSelection);
				cell.Init(itemKey, count, this);
			}
			this.UpdateStatus(currentSelection);
		}

		// Token: 0x060062AE RID: 25262 RVA: 0x002D2CB0 File Offset: 0x002D0EB0
		private int CountItemInSelection(TemplateKey itemKey, List<TemplateKey> selection)
		{
			int count = 0;
			foreach (TemplateKey item in selection)
			{
				bool flag = item.Equals(itemKey);
				if (flag)
				{
					count++;
				}
			}
			return count;
		}

		// Token: 0x060062AF RID: 25263 RVA: 0x002D2D18 File Offset: 0x002D0F18
		private int GetSelectedCount(List<TemplateKey> currentSelection)
		{
			int count = 0;
			foreach (TemplateKey item in currentSelection)
			{
				bool flag = this._allChoices.Contains(item);
				if (flag)
				{
					count++;
				}
			}
			return count;
		}

		// Token: 0x060062B0 RID: 25264 RVA: 0x002D2D84 File Offset: 0x002D0F84
		public void UpdateStatus(List<TemplateKey> currentSelection)
		{
			this._currentCount = this.GetSelectedCount(currentSelection);
			this.groupNameText.text = this._groupName;
			this.groupMaxText.text = this._maxCount.ToString();
			this.groupCurrentText.text = this._currentCount.ToString();
			for (int i = 0; i < this.itemGrid.childCount; i++)
			{
				Transform child = this.itemGrid.GetChild(i);
				bool flag = !child.gameObject.activeSelf;
				if (!flag)
				{
					NewGameFeatureItemCell cell = child.GetComponent<NewGameFeatureItemCell>();
					int count = this.CountItemInSelection(cell.ItemKey, currentSelection);
					cell.UpdateCount(count);
					cell.UpdateButtons(this._currentCount < this._maxCount, count > 0);
				}
			}
		}

		// Token: 0x060062B1 RID: 25265 RVA: 0x002D2E58 File Offset: 0x002D1058
		public void OnAdd(TemplateKey itemKey)
		{
			bool flag = this._currentCount >= this._maxCount;
			if (!flag)
			{
				this._parentView.OnItemAdd(itemKey);
			}
		}

		// Token: 0x060062B2 RID: 25266 RVA: 0x002D2E8A File Offset: 0x002D108A
		public void OnRemove(TemplateKey itemKey)
		{
			this._parentView.OnItemRemove(itemKey);
		}

		// Token: 0x040044C1 RID: 17601
		[SerializeField]
		private TextMeshProUGUI groupNameText;

		// Token: 0x040044C2 RID: 17602
		[SerializeField]
		private TextMeshProUGUI groupMaxText;

		// Token: 0x040044C3 RID: 17603
		[SerializeField]
		private TextMeshProUGUI groupCurrentText;

		// Token: 0x040044C4 RID: 17604
		[SerializeField]
		private Transform itemGrid;

		// Token: 0x040044C5 RID: 17605
		[SerializeField]
		private NewGameFeatureItemCell itemCellPrefab;

		// Token: 0x040044C6 RID: 17606
		private string _groupName;

		// Token: 0x040044C7 RID: 17607
		private int _maxCount;

		// Token: 0x040044C8 RID: 17608
		private int _currentCount;

		// Token: 0x040044C9 RID: 17609
		private List<TemplateKey> _allChoices;

		// Token: 0x040044CA RID: 17610
		private ViewNewGameFeatureItemSelection _parentView;
	}
}
