using System;
using FrameWork;
using TMPro;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A65 RID: 2661
	public class UI_MouseTipEncyclopedia : MouseTipBase
	{
		// Token: 0x060082C3 RID: 33475 RVA: 0x003CE990 File Offset: 0x003CCB90
		protected override void Init(ArgumentBox argsBox)
		{
			TextMeshProUGUI title = base.CGet<TextMeshProUGUI>("Title");
			TextMeshProUGUI desc = base.CGet<TextMeshProUGUI>("Desc");
			string titleString;
			argsBox.Get("Title", out titleString);
			string descString;
			argsBox.Get("Desc", out descString);
			title.text = titleString;
			desc.text = descString;
			desc.GetComponent<TMPTextSpriteHelper>().Parse();
		}
	}
}
