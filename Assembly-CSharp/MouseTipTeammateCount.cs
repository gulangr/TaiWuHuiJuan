using System;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x020002DF RID: 735
public class MouseTipTeammateCount : MouseTipBase
{
	// Token: 0x06002B9B RID: 11163 RVA: 0x00153CA0 File Offset: 0x00151EA0
	protected override void Init(ArgumentBox argsBox)
	{
		bool flag = !this._initialized;
		if (flag)
		{
			this.InitRefers();
			this._initialized = true;
			this._desc.text.ColorReplace();
		}
		CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
		int specialMateCount = monitor.GetTaiwuSpecialGroup().Count;
		string peopleText = LocalStringManager.Get(LanguageKey.LK_Building_PeopleBooty);
		this._specialText.text = string.Format("{0}{1}", specialMateCount, peopleText);
		TaiwuDomainMethod.AsyncCall.GetGroupBabyCount(this, delegate(int offset, RawDataPool pool)
		{
			int babyCount = 0;
			Serializer.Deserialize(pool, offset, ref babyCount);
			this._babyText.text = string.Format("{0}{1}", babyCount, peopleText);
			int teamCount = monitor.GetTaiwuTeamCharIds().Count;
			int normalMateCount = teamCount - babyCount - 1;
			this._normalText.text = string.Format("{0}{1}", normalMateCount, peopleText);
			this._currentAndLimitText.text = string.Format("{0}/{1}", teamCount + specialMateCount, monitor.GetTaiwuGroupMaxCount() + specialMateCount + 1 + babyCount);
		});
		BuildingDomainMethod.AsyncCall.CalcExtraTaiwuGroupMaxCountByStrategyRoom(this, delegate(int offset, RawDataPool pool)
		{
			int extraCount = 0;
			Serializer.Deserialize(pool, offset, ref extraCount);
			this._teamMateLimitText.text = string.Format("+{0}", extraCount);
		});
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x00153D68 File Offset: 0x00151F68
	private void InitRefers()
	{
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._currentAndLimitText = base.CGet<TextMeshProUGUI>("CurrentAndLimitText");
		this._normalText = base.CGet<TextMeshProUGUI>("NormalText");
		this._specialText = base.CGet<TextMeshProUGUI>("SpecialText");
		this._babyText = base.CGet<TextMeshProUGUI>("BabyText");
		this._teamMateLimitText = base.CGet<TextMeshProUGUI>("TeamMateLimitText");
	}

	// Token: 0x04001FCD RID: 8141
	private bool _initialized;

	// Token: 0x04001FCE RID: 8142
	private TextMeshProUGUI _desc;

	// Token: 0x04001FCF RID: 8143
	private TextMeshProUGUI _currentAndLimitText;

	// Token: 0x04001FD0 RID: 8144
	private TextMeshProUGUI _normalText;

	// Token: 0x04001FD1 RID: 8145
	private TextMeshProUGUI _specialText;

	// Token: 0x04001FD2 RID: 8146
	private TextMeshProUGUI _babyText;

	// Token: 0x04001FD3 RID: 8147
	private TextMeshProUGUI _teamMateLimitText;
}
