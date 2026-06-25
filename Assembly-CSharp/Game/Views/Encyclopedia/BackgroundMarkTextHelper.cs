using System;
using System.Text.RegularExpressions;
using Game.Views.Encyclopedia.Elements;
using Game.Views.Encyclopedia.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A47 RID: 2631
	[RequireComponent(typeof(TMP_Text))]
	public class BackgroundMarkTextHelper : MonoBehaviour
	{
		// Token: 0x0600825C RID: 33372 RVA: 0x003CC848 File Offset: 0x003CAA48
		private void TryGenerateRender()
		{
			bool flag = this.render != null;
			if (!flag)
			{
				this.render = SingletonBehaviour<ElementFactory>.Instance.CreateTextHighlightRender();
				this.render.Init(this.text);
			}
		}

		// Token: 0x0600825D RID: 33373 RVA: 0x003CC88C File Offset: 0x003CAA8C
		public void Update()
		{
			bool flag = this.currText == this.text.text;
			if (!flag)
			{
				this.currText = this.text.text;
				bool matched = BackgroundMarkTextHelper.Marks.IsMatch(this.currText);
				bool flag2 = matched;
				if (flag2)
				{
					this.TryGenerateRender();
					this.render.SetText(this.currText);
				}
				else
				{
					bool flag3 = this.render != null;
					if (flag3)
					{
						SingletonBehaviour<ElementFactory>.Instance.ReturnTextHighlightRender(this.render);
						this.render = null;
					}
				}
			}
		}

		// Token: 0x040063A3 RID: 25507
		[SerializeField]
		private TMP_Text text;

		// Token: 0x040063A4 RID: 25508
		[SerializeField]
		private TextHighlightRender render;

		// Token: 0x040063A5 RID: 25509
		public static readonly Regex Marks = TextHighlightRender.Marks;

		// Token: 0x040063A6 RID: 25510
		private string currText = "";
	}
}
