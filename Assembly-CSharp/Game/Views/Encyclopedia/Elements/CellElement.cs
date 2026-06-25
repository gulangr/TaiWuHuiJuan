using System;
using Game.Views.Encyclopedia.SyntaxTree;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A82 RID: 2690
	[RequireComponent(typeof(VariableGridCell))]
	public class CellElement : MonoBehaviour, ISearch
	{
		// Token: 0x17000E75 RID: 3701
		// (get) Token: 0x060083F0 RID: 33776 RVA: 0x003D57A7 File Offset: 0x003D39A7
		public TableCell CellData
		{
			get
			{
				return this._cellData;
			}
		}

		// Token: 0x17000E76 RID: 3702
		// (get) Token: 0x060083F1 RID: 33777 RVA: 0x003D57AF File Offset: 0x003D39AF
		public EEncyclopediaContentLevel Level
		{
			get
			{
				return this._level;
			}
		}

		// Token: 0x17000E77 RID: 3703
		// (get) Token: 0x060083F2 RID: 33778 RVA: 0x003D57B7 File Offset: 0x003D39B7
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x060083F3 RID: 33779 RVA: 0x003D57C4 File Offset: 0x003D39C4
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			this.RefreshSearchResultHighlightImpl(value, onlyTitle, false);
		}

		// Token: 0x060083F4 RID: 33780 RVA: 0x003D57D0 File Offset: 0x003D39D0
		public void RefreshSearchResultHighlightImpl(OptimizedHtmlPatternMatcher value, bool onlyTitle = false, bool parentIsSelecting = false)
		{
			bool flag = this._cachedSearchingValue != ((value != null) ? value.Pattern : null);
			if (flag)
			{
				this._cachedSearchingValue = ((value != null) ? value.Pattern : null);
				this._cachedHighlightResult = Utility.GetHighlightText(this._cellData.text, this._level, value, -1, false);
			}
			int index = BasicInfoView.CurSearchResultIndex.CellIndex(this._cellData);
			bool isSelecting = BasicInfoView.IsShowSearchResult && (parentIsSelecting || this._cellData.parentId == BasicInfoView.CurSearchResultId) && index != -1;
			this.content.text = (isSelecting ? Utility.GetHighlightText(this._cellData.text, this._level, value, index, true) : this._cachedHighlightResult);
		}

		// Token: 0x060083F5 RID: 33781 RVA: 0x003D5894 File Offset: 0x003D3A94
		public void Init(TableCell tableCell, EEncyclopediaContentLevel level)
		{
			this._cellData = tableCell;
			this._level = level;
			this._cachedSearchingValue = string.Empty;
			bool flag = !this._cellData.Invisible;
			if (flag)
			{
				this.bgImage.sprite = (this._cellData.isHeader ? this.headerBg : this.contentBg);
				this.RefreshSearchResultHighlight(null, false);
			}
			this.content.gameObject.SetActive(this.bgImage.enabled = !this._cellData.Invisible);
		}

		// Token: 0x04006507 RID: 25863
		[SerializeField]
		private TextMeshProUGUI content;

		// Token: 0x04006508 RID: 25864
		[SerializeField]
		private CImage bgImage;

		// Token: 0x04006509 RID: 25865
		[SerializeField]
		private Sprite headerBg;

		// Token: 0x0400650A RID: 25866
		[SerializeField]
		private Sprite contentBg;

		// Token: 0x0400650B RID: 25867
		private TableCell _cellData;

		// Token: 0x0400650C RID: 25868
		private EEncyclopediaContentLevel _level;

		// Token: 0x0400650D RID: 25869
		private string _cachedSearchingValue;

		// Token: 0x0400650E RID: 25870
		private string _cachedHighlightResult = string.Empty;
	}
}
