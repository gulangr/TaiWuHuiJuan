using System;
using TMPro;

namespace AiEditor
{
	// Token: 0x02000685 RID: 1669
	public class AiTemplateIdItem : Refers
	{
		// Token: 0x06004EB7 RID: 20151 RVA: 0x0024F167 File Offset: 0x0024D367
		public void Refresh(Action<int> callback, int index, string text, string desc)
		{
			this._callback = callback;
			this._index = index;
			base.CGet<TextMeshProUGUI>("Name").text = text;
			base.CGet<TextMeshProUGUI>("Desc").text = desc;
		}

		// Token: 0x06004EB8 RID: 20152 RVA: 0x0024F19D File Offset: 0x0024D39D
		public void OnClick()
		{
			Action<int> callback = this._callback;
			if (callback != null)
			{
				callback(this._index);
			}
		}

		// Token: 0x0400364B RID: 13899
		private Action<int> _callback;

		// Token: 0x0400364C RID: 13900
		private int _index;
	}
}
