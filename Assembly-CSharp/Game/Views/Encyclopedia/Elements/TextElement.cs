using System;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A8F RID: 2703
	public class TextElement : SingleTextElement, ISearch
	{
		// Token: 0x06008460 RID: 33888 RVA: 0x003D9630 File Offset: 0x003D7830
		protected override void OnInit()
		{
			Debug.LogError("Error: TextElement should not be used.");
		}

		// Token: 0x06008461 RID: 33889 RVA: 0x003D964C File Offset: 0x003D784C
		public void RefreshSearchResultHighlight(string value, bool onlyTitle = false)
		{
			bool flag = BasicInfoView.IsShowSearchResult && base.NodeData.Id == BasicInfoView.CurSearchResultId;
		}

		// Token: 0x04006571 RID: 25969
		[SerializeField]
		private TextMeshProUGUI title;
	}
}
