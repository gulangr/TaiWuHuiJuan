using System;
using Config;
using FrameWork;
using TMPro;

// Token: 0x020002EB RID: 747
public class UI_MouseTipBuildingFeast : MouseTipBase
{
	// Token: 0x06002BF8 RID: 11256 RVA: 0x0015834C File Offset: 0x0015654C
	protected override void Init(ArgumentBox argsBox)
	{
		short type;
		argsBox.Get("type", out type);
		FeastItem config = Feast.Instance[type];
		bool flag = config == null;
		if (!flag)
		{
			base.CGet<TextMeshProUGUI>("Title").text = config.Name.ColorReplace();
			base.CGet<TextMeshProUGUI>("ConditionContent").text = config.ConditionDesc.ColorReplace();
			base.CGet<TextMeshProUGUI>("EffectContent").text = config.EffectDesc.ColorReplace();
		}
	}
}
