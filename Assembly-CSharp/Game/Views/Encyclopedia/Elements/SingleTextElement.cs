using System;
using System.Linq;
using Game.Views.Encyclopedia.Utilities;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A8C RID: 2700
	public class SingleTextElement : Element, ISearch
	{
		// Token: 0x06008441 RID: 33857 RVA: 0x003D858B File Offset: 0x003D678B
		protected override void OnInit()
		{
			this.textSpriteHelper.Parse();
			this.layoutGroup.padding.left = base.GetLayoutPadding();
			this.RefreshSearchResultHighlight(null, false);
		}

		// Token: 0x06008442 RID: 33858 RVA: 0x003D85BC File Offset: 0x003D67BC
		public void RefreshSearchResultHighlight(OptimizedHtmlPatternMatcher value, bool onlyTitle = false)
		{
			EEncyclopediaContentLayout[] layout = base.NodeData.ConfigItem.Layout;
			bool flag;
			if (layout == null)
			{
				flag = false;
			}
			else
			{
				flag = layout.Any((EEncyclopediaContentLayout x) => x == EEncyclopediaContentLayout.Enum0);
			}
			string dotStr = flag ? LanguageKey.LK_Dot_Symbol.Tr() : string.Empty;
			bool flag2 = this._cachedSearchingValue != ((value != null) ? value.Pattern : null);
			if (flag2)
			{
				this._cachedSearchingValue = ((value != null) ? value.Pattern : null);
				this._currHighlightResult = dotStr + Utility.GetHighlightText(base.NodeData.Content, base.NodeData, value, -1, false);
			}
			int index = BasicInfoView.CurSearchResultIndex.SingleTextIndex;
			string text = (BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId && index != -1) ? (dotStr + Utility.GetHighlightText(base.NodeData.Content, base.NodeData, value, index, true)) : this._currHighlightResult;
			this.content.text = text.SetColor("brightyellow");
		}

		// Token: 0x0400654E RID: 25934
		[SerializeField]
		protected TextMeshProUGUI content;

		// Token: 0x0400654F RID: 25935
		[SerializeField]
		protected TMPTextSpriteHelper textSpriteHelper;

		// Token: 0x04006550 RID: 25936
		[SerializeField]
		protected LayoutGroup layoutGroup;

		// Token: 0x04006551 RID: 25937
		private string _currHighlightResult;
	}
}
