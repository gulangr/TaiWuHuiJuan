using System;
using FrameWork;
using TMPro;

namespace AiEditor
{
	// Token: 0x02000690 RID: 1680
	public class UI_AiSearchInputField : UIBase
	{
		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06004EF9 RID: 20217 RVA: 0x002503C6 File Offset: 0x0024E5C6
		private TMP_InputField InputField
		{
			get
			{
				return base.CGet<TMP_InputField>("InputField");
			}
		}

		// Token: 0x06004EFA RID: 20218 RVA: 0x002503D3 File Offset: 0x0024E5D3
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("StartAt", out this._startAt);
			argsBox.Get<AiSearchTryNextDelegate>("TryNext", out this._tryNext);
		}

		// Token: 0x06004EFB RID: 20219 RVA: 0x002503FA File Offset: 0x0024E5FA
		public void OnNext()
		{
			this._startAt = this._tryNext(this._startAt, this.InputField.text);
		}

		// Token: 0x0400365E RID: 13918
		private int _startAt;

		// Token: 0x0400365F RID: 13919
		private AiSearchTryNextDelegate _tryNext;
	}
}
