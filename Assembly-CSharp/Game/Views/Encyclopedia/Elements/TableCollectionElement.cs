using System;
using System.Linq;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A8D RID: 2701
	public class TableCollectionElement : Element, ISearch
	{
		// Token: 0x06008444 RID: 33860 RVA: 0x003D86ED File Offset: 0x003D68ED
		private void Awake()
		{
			this.expandBtn.ClearAndAddListener(new Action(this.OnClickExpandBtn));
		}

		// Token: 0x06008445 RID: 33861 RVA: 0x003D8708 File Offset: 0x003D6908
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			string cachedSearchingValue = this._cachedSearchingValue;
			OptimizedHtmlPatternMatcher value2 = value;
			bool flag = cachedSearchingValue != ((value2 != null) ? value2.Pattern : null);
			if (flag)
			{
				OptimizedHtmlPatternMatcher value3 = value;
				this._cachedSearchingValue = ((value3 != null) ? value3.Pattern : null);
				this._highlightedTitle = Utility.GetHighlightText(this._title, base.NodeData, value, -1, false);
				this._highlightedTogs = (from togName in this._togsNames
				select Utility.GetHighlightText(togName, this.NodeData, value, -1, false)).ToArray<string>();
			}
			int refTemplateId = base.NodeData.ConfigItem.Inserts.FirstOrDefault((int t) => EncyclopediaReference.Instance[t].InsertType == EEncyclopediaReferenceInsertType.TableCollection);
			EncyclopediaReferenceItem cfg = EncyclopediaReference.Instance[refTemplateId];
			bool isSelecting = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
			TMP_Text tmp_Text = this.title;
			string text;
			if (isSelecting)
			{
				int titleIndex = BasicInfoView.CurSearchResultIndex.SingleTextIndex;
				if (titleIndex > -1)
				{
					text = Utility.GetHighlightText(this._title, base.NodeData, value, titleIndex, true);
					goto IL_12D;
				}
			}
			text = this._highlightedTitle;
			IL_12D:
			tmp_Text.text = text;
			for (int i = 0; i < this._tables.Length; i++)
			{
				CToggle tog = this.toggleGroup.Get(i);
				int tableHeaderIndex = BasicInfoView.CurSearchResultIndex.TableCollectionHeaderIndex(i);
				bool needGet = isSelecting && tableHeaderIndex > -1;
				TextMeshProUGUI label = tog.GetComponentInChildren<TextMeshProUGUI>();
				label.text = (needGet ? Utility.GetHighlightText(this._togsNames[i], base.NodeData, value, tableHeaderIndex, true) : this._highlightedTogs[i]);
			}
			this.tableElement.RefreshSearchResultHighlightImpl(value, onlyTitle, isSelecting);
			this.RefreshCollapseState();
		}

		// Token: 0x06008446 RID: 33862 RVA: 0x003D88EC File Offset: 0x003D6AEC
		protected override void OnInit()
		{
			base.NodeData.IsCollapse = false;
			this.tableElement.Parent = this;
			int refTemplateId = base.NodeData.ConfigItem.Inserts.FirstOrDefault((int t) => EncyclopediaReference.Instance[t].InsertType == EEncyclopediaReferenceInsertType.TableCollection);
			EncyclopediaReferenceItem cfg = EncyclopediaReference.Instance[refTemplateId];
			this._tables = (from x in cfg.Params
			select EncyclopediaReference.Instance[x]).ToArray<EncyclopediaReferenceItem>();
			this._title = cfg.Title;
			bool flag = this._tables.Length == 0;
			if (!flag)
			{
				bool inited = this.toggleGroup.Count() > 0;
				string[] togsNames = this._togsNames;
				int? num = (togsNames != null) ? new int?(togsNames.Length) : null;
				int num2 = this._tables.Length;
				bool flag2 = !(num.GetValueOrDefault() == num2 & num != null);
				if (flag2)
				{
					this._togsNames = new string[this._tables.Length];
				}
				for (int i = 0; i < this._tables.Length; i++)
				{
					bool flag3 = this.toggleGroup.Count() > i;
					CToggle tog;
					if (flag3)
					{
						tog = this.toggleGroup.Get(i);
					}
					else
					{
						tog = Object.Instantiate<CToggle>(this.togglePrefab, this.pager);
						this.toggleGroup.Add(tog);
					}
					tog.gameObject.SetActive(true);
					tog.onValueChanged.RemoveAllListeners();
					EncyclopediaReferenceItem cellCfg = this._tables[i];
					this._togsNames[i] = (cfg.Desc.CheckIndex(i) ? cfg.Desc[i] : cellCfg.Title);
					int cellIdx = i;
					tog.onValueChanged.AddListener(delegate(bool isOn)
					{
						bool flag6 = !isOn;
						if (!flag6)
						{
							this.tableElement.InitData(cellCfg, cellIdx);
							bool isShowSearchResult = BasicInfoView.IsShowSearchResult;
							if (isShowSearchResult)
							{
								TableCollectionElement <>4__this = this;
								BasicInfoView instance = BasicInfoView.Instance;
								<>4__this.RefreshSearchResultHighlight((instance != null) ? instance.Searcher : null, false);
							}
							BasicInfoView instance2 = BasicInfoView.Instance;
							if (instance2 != null)
							{
								instance2.pageDetailElement.ClearFixedTableHeaderData();
							}
						}
					});
				}
				for (int j = this._tables.Length; j < this.toggleGroup.Count(); j++)
				{
					this.toggleGroup.Get(j).gameObject.SetActive(false);
				}
				bool flag4 = inited;
				if (flag4)
				{
					CToggle ctoggle = this.toggleGroup.Get(0);
					bool flag5 = ctoggle != null && ctoggle.isOn;
					if (flag5)
					{
						this.tableElement.InitData(this._tables[0], 0);
					}
					else
					{
						this.toggleGroup.Set(0, false);
					}
				}
				else
				{
					this.toggleGroup.Init(0);
				}
				this.RefreshSearchResultHighlight(null, false);
			}
		}

		// Token: 0x06008447 RID: 33863 RVA: 0x003D8BAC File Offset: 0x003D6DAC
		private void OnClickExpandBtn()
		{
			base.NodeData.IsCollapse = !base.NodeData.IsCollapse;
			this.RefreshCollapseState();
		}

		// Token: 0x06008448 RID: 33864 RVA: 0x003D8BD0 File Offset: 0x003D6DD0
		private void RefreshCollapseState()
		{
			bool show = !base.NodeData.IsCollapse;
			this.arrow.localScale = new Vector3(1f, (float)(show ? -1 : 1), 1f);
			this.content.gameObject.SetActive(show);
		}

		// Token: 0x06008449 RID: 33865 RVA: 0x003D8C24 File Offset: 0x003D6E24
		public override RectTransform GetSearchingRect(SearchIndex searchIndex)
		{
			bool flag = searchIndex == null || searchIndex.Index2 == -1;
			RectTransform result;
			if (flag)
			{
				result = (this.title.transform as RectTransform);
			}
			else
			{
				bool flag2 = searchIndex.Index1 == -1;
				if (flag2)
				{
					this.toggleGroup.Set(searchIndex.Index2, false);
					result = this.pager;
				}
				else
				{
					SearchResultType resultType = searchIndex.ResultType;
					SearchResultType searchResultType = resultType;
					if (searchResultType != SearchResultType.TableCell && searchResultType != SearchResultType.TableFooter)
					{
						result = base.GetSearchingRect(searchIndex);
					}
					else
					{
						this.toggleGroup.Set(searchIndex.Index2, false);
						result = this.tableElement.GetSearchingRect(searchIndex);
					}
				}
			}
			return result;
		}

		// Token: 0x04006552 RID: 25938
		[SerializeField]
		internal TableElement tableElement;

		// Token: 0x04006553 RID: 25939
		[SerializeField]
		internal CToggle togglePrefab;

		// Token: 0x04006554 RID: 25940
		[SerializeField]
		internal RectTransform pager;

		// Token: 0x04006555 RID: 25941
		[SerializeField]
		internal RectTransform arrow;

		// Token: 0x04006556 RID: 25942
		[SerializeField]
		internal RectTransform content;

		// Token: 0x04006557 RID: 25943
		[SerializeField]
		internal CToggleGroup toggleGroup;

		// Token: 0x04006558 RID: 25944
		[SerializeField]
		internal TMP_Text title;

		// Token: 0x04006559 RID: 25945
		[SerializeField]
		internal CButton expandBtn;

		// Token: 0x0400655A RID: 25946
		private string _title;

		// Token: 0x0400655B RID: 25947
		private EncyclopediaReferenceItem[] _tables = Array.Empty<EncyclopediaReferenceItem>();

		// Token: 0x0400655C RID: 25948
		private string _highlightedTitle;

		// Token: 0x0400655D RID: 25949
		private string[] _highlightedTogs;

		// Token: 0x0400655E RID: 25950
		private string[] _togsNames;
	}
}
