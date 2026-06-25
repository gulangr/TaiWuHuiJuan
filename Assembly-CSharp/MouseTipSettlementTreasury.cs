using System;
using FrameWork;
using GameData.Domains.Organization.Display;
using GameDataExtensions;
using TMPro;

// Token: 0x020002D0 RID: 720
public class MouseTipSettlementTreasury : MouseTipBase
{
	// Token: 0x06002B2D RID: 11053 RVA: 0x00150FF4 File Offset: 0x0014F1F4
	protected override void Init(ArgumentBox argsBox)
	{
		SettlementTreasuryDisplayData settlementTreasuryDisplayData;
		argsBox.Get<SettlementTreasuryDisplayData>("SettlementTreasuryDisplayData", out settlementTreasuryDisplayData);
		base.CGet<TextMeshProUGUI>("Desc").text = LanguageKey.LK_Building_Treasury_Tip_Content.TrFormat(settlementTreasuryDisplayData.SettlementNameRelatedData.GetName(), settlementTreasuryDisplayData.InfluenceRefreshTime).ColorReplace();
		TextMeshProUGUI levelText = base.CGet<TextMeshProUGUI>("Level");
		string levelContent = "<SpName=" + settlementTreasuryDisplayData.GetDisplayLevelIcon() + ">" + settlementTreasuryDisplayData.GetDisplayLevelStr();
		levelText.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Treasury_Tip_Level, levelContent).ColorReplace();
		sbyte sectStoryEnding = settlementTreasuryDisplayData.SectStoryEnding;
		if (!true)
		{
		}
		string text;
		switch (sectStoryEnding)
		{
		case 0:
			text = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Tip_NoEffect).SetColor("pinkyellow");
			break;
		case 1:
			text = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Tip_Sect_Up).SetColor("brightblue");
			break;
		case 2:
			text = LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Tip_Sect_Down).SetColor("brightred");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		string sectStatusStr = text;
		base.CGet<TextMeshProUGUI>("Sect").text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Treasury_Tip_Sect, sectStatusStr);
		string martialArtTournamentStr = settlementTreasuryDisplayData.MartialArtTournamentResult ? LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Tip_MartialArtContest_Win).SetColor("brightblue") : LocalStringManager.Get(LanguageKey.LK_Building_Treasury_Tip_NoEffect).SetColor("pinkyellow");
		base.CGet<TextMeshProUGUI>("Tournament").text = LocalStringManager.GetFormat(LanguageKey.LK_Building_Treasury_Tip_MartialArtContest, martialArtTournamentStr);
	}

	// Token: 0x06002B2E RID: 11054 RVA: 0x00151169 File Offset: 0x0014F369
	protected override void OnEnable()
	{
		base.OnEnable();
		base.CGet<TextMeshProUGUI>("Desc").GetComponent<TMPTextSpriteHelper>().Parse();
		base.CGet<TextMeshProUGUI>("Level").GetComponent<TMPTextSpriteHelper>().Parse();
	}
}
