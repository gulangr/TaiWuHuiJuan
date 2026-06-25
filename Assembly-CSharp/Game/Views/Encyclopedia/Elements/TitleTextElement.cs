using System;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A93 RID: 2707
	public class TitleTextElement : Element, ISearch
	{
		// Token: 0x06008488 RID: 33928 RVA: 0x003DA4BD File Offset: 0x003D86BD
		protected override void OnInit()
		{
			this.showInLevelFour.Init(base.NodeData);
			this.RefreshSearchResultHighlight(null, false);
		}

		// Token: 0x06008489 RID: 33929 RVA: 0x003DA4DC File Offset: 0x003D86DC
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			bool flag = this._cachedSearchingValue != ((value != null) ? value.Pattern : null);
			if (flag)
			{
				this._cachedSearchingValue = ((value != null) ? value.Pattern : null);
				this._currHighlightResult = Utility.GetHighlightText(base.NodeData.Title, base.NodeData, value, BasicInfoView.CurSearchResultIndex.SingleTextIndex, false);
			}
			bool isSelecting = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
			this.title.text = (isSelecting ? Utility.GetHighlightText(base.NodeData.Title, base.NodeData, value, BasicInfoView.CurSearchResultIndex.SingleTextIndex, true) : this._currHighlightResult);
		}

		// Token: 0x0600848A RID: 33930 RVA: 0x003DA597 File Offset: 0x003D8797
		public override void RefreshShowStatus()
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x0400659A RID: 26010
		[SerializeField]
		private TMP_Text title;

		// Token: 0x0400659B RID: 26011
		[SerializeField]
		internal LeveledContainer childrenContainer;

		// Token: 0x0400659C RID: 26012
		[SerializeField]
		internal ShowInLevelFour showInLevelFour;

		// Token: 0x0400659D RID: 26013
		private string _currHighlightResult;
	}
}
