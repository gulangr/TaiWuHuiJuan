using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using TMPro;

// Token: 0x020002A3 RID: 675
public class MouseTipInnateFiveElements : MouseTipBase
{
	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x06002A33 RID: 10803 RVA: 0x00141ED4 File Offset: 0x001400D4
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x00141ED8 File Offset: 0x001400D8
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		int birthMonth;
		argsBox.Get("BirthMonth", out birthMonth);
		MonthItem monthConfig = Month.Instance[birthMonth];
		NeiliTypeItem config = NeiliType.Instance[monthConfig.FiveElementsType];
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.GetFormat(LanguageKey.LK_Innate_FiveElements_Tips_Title, monthConfig.FiveElementsTypeDesc);
		base.CGet<TextMeshProUGUI>("BornText").text = monthConfig.Name;
		base.CGet<TextMeshProUGUI>("NeiliTypeText").text = string.Format("<SpName=mousetip_shuxing_{0}>{1}", monthConfig.FiveElementsType, config.Name.SetColor(Colors.Instance.FiveElementsColors[(int)monthConfig.FiveElementsType]));
		base.CGet<TextMeshProUGUI>("NeiliDescText").text = config.EffectDesc.ColorReplace();
		base.CGet<TextMeshProUGUI>("NeiliTypeText").GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x00141FD0 File Offset: 0x001401D0
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.BirthTime);
		}
	}
}
