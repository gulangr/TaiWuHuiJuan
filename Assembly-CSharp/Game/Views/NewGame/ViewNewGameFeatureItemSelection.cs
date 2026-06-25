using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x0200081C RID: 2076
	public class ViewNewGameFeatureItemSelection : UIBase
	{
		// Token: 0x17000C51 RID: 3153
		// (get) Token: 0x060065F7 RID: 26103 RVA: 0x002E92BC File Offset: 0x002E74BC
		public ProtagonistFeatureItem FeatureData
		{
			get
			{
				return this._featureData;
			}
		}

		// Token: 0x060065F8 RID: 26104 RVA: 0x002E92C4 File Offset: 0x002E74C4
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox == null;
			if (!flag)
			{
				ProtagonistFeatureItem featureData;
				bool flag2 = !argsBox.Get<ProtagonistFeatureItem>("Feature", out featureData);
				if (!flag2)
				{
					this._featureData = featureData;
					List<TemplateKey> selection;
					argsBox.Get<List<TemplateKey>>("Selection", out selection);
					Action<List<TemplateKey>> callback;
					argsBox.Get<Action<List<TemplateKey>>>("Callback", out callback);
					this._onConfirm = callback;
					this._currentSelection = new List<TemplateKey>(selection ?? new List<TemplateKey>());
					this._initialSelectionCount = this._currentSelection.Count;
					this.InitUI();
					this.Element.ShowAfterRefresh();
				}
			}
		}

		// Token: 0x060065F9 RID: 26105 RVA: 0x002E9358 File Offset: 0x002E7558
		private void InitUI()
		{
			this.titleText.text = this._featureData.Name;
			this._totalMaxCount = 0;
			List<TemplateKey>[] groups = this._featureData.CustomGroupItem;
			string[] groupNames = this._featureData.CustomGroupName;
			int[] groupCounts = this._featureData.CustomGroupCount;
			int validGroupCount = 0;
			bool flag = groups != null;
			if (flag)
			{
				for (int i = 0; i < groups.Length; i++)
				{
					bool flag2 = groups[i] != null && groups[i].Count > 0;
					if (flag2)
					{
						validGroupCount++;
					}
				}
			}
			CommonUtils.PrepareEnoughChildren(this.groupContainer, this.groupPrefab.gameObject, validGroupCount, null);
			int groupIndex = 0;
			bool flag3 = groups != null;
			if (flag3)
			{
				for (int j = 0; j < groups.Length; j++)
				{
					bool flag4 = groups[j] == null || groups[j].Count == 0;
					if (!flag4)
					{
						NewGameFeatureItemGroup groupView = this.groupContainer.GetChild(groupIndex).GetComponent<NewGameFeatureItemGroup>();
						string groupName = (groupNames != null && j < groupNames.Length) ? groupNames[j] : LanguageKey.LK_NewGame_Feature_ItemGroup_Default.TrFormat(j + 1);
						int count = (groupCounts != null && j < groupCounts.Length) ? groupCounts[j] : 1;
						groupView.Init(groupName, count, groups[j], this._currentSelection, this);
						this._totalMaxCount += count;
						groupIndex++;
					}
				}
			}
			this.UpdateConfirmButton();
			this.confirmButton.ClearAndAddListener(new Action(this.Close));
			this.resetButton.ClearAndAddListener(new Action(this.ResetSelection));
			this.closeButton.ClearAndAddListener(new Action(this.ClosePanel));
		}

		// Token: 0x060065FA RID: 26106 RVA: 0x002E9527 File Offset: 0x002E7727
		private void ResetSelection()
		{
			this._currentSelection.Clear();
			this.RefreshGroups();
			this.UpdateConfirmButton();
		}

		// Token: 0x060065FB RID: 26107 RVA: 0x002E9544 File Offset: 0x002E7744
		private void UpdateConfirmButton()
		{
			bool hasSelection = this._currentSelection.Count > 0;
			bool isSelectionComplete = this._currentSelection.Count >= this._totalMaxCount;
			bool canConfirmCleared = !hasSelection && this._initialSelectionCount > 0;
			bool flag = this.confirmButton != null;
			if (flag)
			{
				this.confirmButton.interactable = (isSelectionComplete || canConfirmCleared);
			}
			bool flag2 = this.resetButton != null;
			if (flag2)
			{
				this.resetButton.interactable = hasSelection;
			}
		}

		// Token: 0x060065FC RID: 26108 RVA: 0x002E95C6 File Offset: 0x002E77C6
		public void OnItemAdd(TemplateKey itemKey)
		{
			this._currentSelection.Add(itemKey);
			this.RefreshGroups();
		}

		// Token: 0x060065FD RID: 26109 RVA: 0x002E95DD File Offset: 0x002E77DD
		public void OnItemRemove(TemplateKey itemKey)
		{
			this._currentSelection.Remove(itemKey);
			this.RefreshGroups();
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x002E95F4 File Offset: 0x002E77F4
		private void RefreshGroups()
		{
			for (int i = 0; i < this.groupContainer.childCount; i++)
			{
				Transform child = this.groupContainer.GetChild(i);
				bool flag = !child.gameObject.activeSelf;
				if (!flag)
				{
					child.GetComponent<NewGameFeatureItemGroup>().UpdateStatus(this._currentSelection);
				}
			}
			this.UpdateConfirmButton();
		}

		// Token: 0x060065FF RID: 26111 RVA: 0x002E9658 File Offset: 0x002E7858
		private void Close()
		{
			Action<List<TemplateKey>> onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm(this._currentSelection);
			}
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x06006600 RID: 26112 RVA: 0x002E9684 File Offset: 0x002E7884
		private void ClosePanel()
		{
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x04004733 RID: 18227
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004734 RID: 18228
		[SerializeField]
		private Transform groupContainer;

		// Token: 0x04004735 RID: 18229
		[SerializeField]
		private NewGameFeatureItemGroup groupPrefab;

		// Token: 0x04004736 RID: 18230
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04004737 RID: 18231
		[SerializeField]
		private CButton resetButton;

		// Token: 0x04004738 RID: 18232
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04004739 RID: 18233
		private ProtagonistFeatureItem _featureData;

		// Token: 0x0400473A RID: 18234
		private List<TemplateKey> _currentSelection;

		// Token: 0x0400473B RID: 18235
		private Action<List<TemplateKey>> _onConfirm;

		// Token: 0x0400473C RID: 18236
		private int _totalMaxCount;

		// Token: 0x0400473D RID: 18237
		private int _initialSelectionCount;
	}
}
