using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x0200024B RID: 587
public class UI_DebateResult : UIBase
{
	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x06002680 RID: 9856 RVA: 0x00119F83 File Offset: 0x00118183
	private CButtonObsolete CloseButton
	{
		get
		{
			return base.CGet<CButtonObsolete>("Close");
		}
	}

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x06002681 RID: 9857 RVA: 0x00119F90 File Offset: 0x00118190
	private GameObject EvaluationPrefab
	{
		get
		{
			return base.CGet<GameObject>("EvaluationPrefab");
		}
	}

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x06002682 RID: 9858 RVA: 0x00119F9D File Offset: 0x0011819D
	private GameObject CommentPrefab
	{
		get
		{
			return base.CGet<GameObject>("CommentPrefab");
		}
	}

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x06002683 RID: 9859 RVA: 0x00119FAA File Offset: 0x001181AA
	private GameObject CharacterPrefab
	{
		get
		{
			return base.CGet<GameObject>("CharPrefab");
		}
	}

	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x06002684 RID: 9860 RVA: 0x00119FB7 File Offset: 0x001181B7
	private RectTransform CharacterHolder
	{
		get
		{
			return base.CGet<RectTransform>("CharRoot");
		}
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x00119FC4 File Offset: 0x001181C4
	public override void OnInit(ArgumentBox argsBox)
	{
		UI_DebateResult.<>c__DisplayClass17_0 CS$<>8__locals1 = new UI_DebateResult.<>c__DisplayClass17_0();
		CS$<>8__locals1.<>4__this = this;
		argsBox.Get("IsTaiwuWin", out this._isTaiwuWin);
		argsBox.Get<DebateResult>("DebateResult", out this._debateResult);
		bool showReadingEvent = this._debateResult.ShowReadingEvent;
		if (showReadingEvent)
		{
			CombatEvaluationItem config = CombatEvaluation.Instance[33];
			this.AddEvaluation(config.Name, config.Desc);
		}
		bool showLoopingEvent = this._debateResult.ShowLoopingEvent;
		if (showLoopingEvent)
		{
			CombatEvaluationItem config2 = CombatEvaluation.Instance[43];
			this.AddEvaluation(config2.Name, config2.Desc);
		}
		foreach (short evaluation in this._debateResult.Evaluations)
		{
			DebateEvaluationItem config3 = DebateEvaluation.Instance[evaluation];
			this.AddEvaluation(config3);
		}
		foreach (KeyValuePair<short, int> keyValuePair in this._debateResult.TaiwuComments)
		{
			short num;
			int num2;
			keyValuePair.Deconstruct(out num, out num2);
			short comment = num;
			int count = num2;
			this.AddComment(Config.DebateComment.Instance[comment], true, count);
		}
		foreach (KeyValuePair<short, int> keyValuePair in this._debateResult.NpcComments)
		{
			short num;
			int num2;
			keyValuePair.Deconstruct(out num, out num2);
			short comment2 = num;
			int count2 = num2;
			this.AddComment(Config.DebateComment.Instance[comment2], false, count2);
		}
		base.CGet<TextMeshProUGUI>("ExpText").text = ((this._debateResult.Exp.Second != 0) ? (CommonUtils.GetDisplayStringForNum(this._debateResult.Exp.First, 100000) + this.GetValueString(this._debateResult.Exp.Second, false)) : "-".SetColor("grey"));
		base.CGet<TextMeshProUGUI>("AuthorityText").text = ((this._debateResult.Authority.Second != 0) ? (CommonUtils.GetDisplayStringForNum(this._debateResult.Authority.First, 100000) + this.GetValueString(this._debateResult.Authority.Second, false)) : "-".SetColor("grey"));
		foreach (int charId in this._debateResult.Happiness.Keys)
		{
			this.SetPlayer(charId);
		}
		foreach (int charId2 in this._debateResult.Favorability.Keys)
		{
			this.SetSpectator(charId2);
		}
		this.CharacterHolder.gameObject.SetActive(this._debateResult.Favorability.Count != 0);
		base.CGet<GameObject>("LootTitleBack").SetActive(this._debateResult.Favorability.Count != 0);
		CS$<>8__locals1.aniTime = (this._isTaiwuWin ? 1.333f : 1.333f);
		CS$<>8__locals1.resultAni = base.CGet<SkeletonGraphic>("ResultAni");
		CS$<>8__locals1.mainWindow = base.CGet<CanvasGroup>("MainWindow");
		CS$<>8__locals1.pointerMask = base.CGet<GameObject>("PointerMask");
		CS$<>8__locals1.btnCanvas = this.CloseButton.GetComponent<CanvasGroup>();
		CS$<>8__locals1.readingEventTips = base.CGet<CanvasGroup>("ReadingEventTips");
		CS$<>8__locals1.loopingEventTips = base.CGet<CanvasGroup>("LoopingEventTips");
		CS$<>8__locals1.resultAni.AnimationState.ClearTracks();
		CS$<>8__locals1.resultAni.Skeleton.SetToSetupPose();
		CS$<>8__locals1.resultAni.color = Color.white.SetAlpha(0f);
		CS$<>8__locals1.mainWindow.alpha = 0f;
		this.CloseButton.interactable = false;
		CS$<>8__locals1.btnCanvas.alpha = 0f;
		CS$<>8__locals1.readingEventTips.gameObject.SetActive(false);
		CS$<>8__locals1.readingEventTips.alpha = 0f;
		CS$<>8__locals1.loopingEventTips.gameObject.SetActive(false);
		CS$<>8__locals1.loopingEventTips.alpha = 0f;
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(CS$<>8__locals1.<OnInit>g__OnShowed|0));
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x0011A4C0 File Offset: 0x001186C0
	private void AddComment(DebateCommentItem config, bool isTaiwu, int count)
	{
		Refers holderParent = isTaiwu ? base.CGet<Refers>("Taiwu") : base.CGet<Refers>("Npc");
		RectTransform holder = holderParent.CGet<RectTransform>("Content");
		GameObject obj = Object.Instantiate<GameObject>(this.CommentPrefab, holder);
		Refers refers = obj.GetComponent<Refers>();
		TooltipInvoker tips = obj.GetComponent<TooltipInvoker>();
		StringBuilder sb = new StringBuilder();
		sbyte behaviorType = SingletonObject.getInstance<LifeSkillCombatModel>().TaiwuCharData.BehaviorType;
		sb.AppendLine(config.ResultTip);
		bool flag = config.Happiness[(int)behaviorType] != 0;
		if (flag)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Self) + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness) + this.GetValueString((int)config.Happiness[(int)behaviorType], false));
		}
		bool flag2 = config.Favor != 0;
		if (flag2)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_LifeskillCombat_Spectator) + LocalStringManager.Get(LanguageKey.LK_Favorability) + this.GetValueString((int)config.Favor, false));
		}
		refers.CGet<TextMeshProUGUI>("Content").text = config.Name;
		bool flag3 = count > 1;
		if (flag3)
		{
			refers.CGet<TextMeshProUGUI>("CountText").text = string.Format("x{0}", count);
			refers.CGet<GameObject>("CountBg").SetActive(true);
		}
		else
		{
			refers.CGet<GameObject>("CountBg").SetActive(false);
		}
		tips.PresetParam[0] = config.Name;
		tips.PresetParam[1] = sb.ToString();
		obj.SetActive(true);
		this._objList.Add(obj);
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x0011A658 File Offset: 0x00118858
	private void AddEvaluation(string eName, string eDesc)
	{
		RectTransform holder = base.CGet<RectTransform>("EvaluationHolder");
		GameObject obj = Object.Instantiate<GameObject>(this.EvaluationPrefab, holder);
		TooltipInvoker tips = obj.GetComponent<TooltipInvoker>();
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Content").text = eName;
		tips.PresetParam[0] = eName;
		tips.PresetParam[1] = eDesc;
		obj.SetActive(true);
		this._objList.Add(obj);
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x0011A6C4 File Offset: 0x001188C4
	private void AddEvaluation(DebateEvaluationItem config)
	{
		RectTransform holder = base.CGet<RectTransform>("EvaluationHolder");
		GameObject obj = Object.Instantiate<GameObject>(this.EvaluationPrefab, holder);
		TooltipInvoker tips = obj.GetComponent<TooltipInvoker>();
		string eName = config.Name;
		StringBuilder sb = new StringBuilder();
		sb.AppendLine(config.ResultTip);
		bool flag = config.HappinessDelta != 0;
		if (flag)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Self) + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness) + this.GetValueString(config.HappinessDelta, false));
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_LifeskillCombat_Spectator) + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness) + this.GetValueString(config.HappinessDelta, false));
		}
		bool flag2 = config.ExpB != 0;
		if (flag2)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Exp) + this.GetValuePercentString(config.ExpB, false));
		}
		bool flag3 = config.ExpC != 0;
		if (flag3)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Exp) + this.GetValuePercentString(config.ExpC, false));
		}
		bool flag4 = config.AuthorityB != 0;
		if (flag4)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority) + this.GetValuePercentString(config.AuthorityB, false));
		}
		bool flag5 = config.AuthorityC != 0;
		if (flag5)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority) + this.GetValuePercentString(config.AuthorityC, false));
		}
		bool flag6 = config.FavorIncreaseB != 0;
		if (flag6)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityUpInfection) + this.GetValuePercentString(config.FavorIncreaseB, false));
		}
		bool flag7 = config.FavorIncreaseC != 0;
		if (flag7)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityUpInfection) + this.GetValuePercentString(config.FavorIncreaseC, false));
		}
		bool flag8 = config.FavorDecreaseB != 0;
		if (flag8)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityDownInfection) + this.GetValuePercentString(config.FavorDecreaseB, true));
		}
		bool flag9 = config.FavorDecreaseC != 0;
		if (flag9)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityDownInfection) + this.GetValuePercentString(config.FavorDecreaseC, true));
		}
		obj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Content").text = eName;
		tips.PresetParam[0] = eName;
		tips.PresetParam[1] = sb.ToString();
		obj.SetActive(true);
		this._objList.Add(obj);
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x0011A980 File Offset: 0x00118B80
	private void SetPlayer(int charId)
	{
		CharacterDisplayData displayData;
		bool flag = !this._debateResult.CharacterDisplayDataMap.TryGetValue(charId, out displayData);
		if (!flag)
		{
			Refers parent = (charId == this._taiwuId) ? base.CGet<Refers>("Taiwu") : base.CGet<Refers>("Npc");
			Refers refers = parent.CGet<Refers>("Character");
			IntPair happiness = this._debateResult.Happiness[charId];
			refers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(displayData, true);
			refers.CGet<TextMeshProUGUI>("Name").text = NameCenter.GetMonasticTitleOrDisplayName(displayData, charId == this._taiwuId);
			refers.CGet<CImage>("HappinessIcon").SetSprite(CommonUtils.GetHappinessIconLegacy(HappinessType.GetHappinessType((sbyte)happiness.Second)), false, null);
			refers.CGet<TextMeshProUGUI>("HappinessText").text = this.GetValueString(happiness.Second - happiness.First, true);
		}
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x0011AA6C File Offset: 0x00118C6C
	private void SetSpectator(int charId)
	{
		CharacterDisplayData displayData;
		bool flag = !this._debateResult.CharacterDisplayDataMap.TryGetValue(charId, out displayData);
		if (!flag)
		{
			GameObject obj = Object.Instantiate<GameObject>(this.CharacterPrefab, this.CharacterHolder);
			Refers refers = obj.GetComponent<Refers>();
			IntPair favor = this._debateResult.Favorability[charId];
			refers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(displayData, true);
			refers.CGet<TextMeshProUGUI>("Name").text = NameCenter.GetMonasticTitleOrDisplayName(displayData, charId == this._taiwuId);
			refers.CGet<CImage>("FavorIcon").SetSprite(CommonUtils.GetFavorIconLegacy(displayData.FavorabilityToTaiwu), false, null);
			refers.CGet<TextMeshProUGUI>("FavorText").text = this.GetValueString(favor.Second - favor.First, true);
			obj.SetActive(true);
			this._objList.Add(obj);
		}
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x0011AB54 File Offset: 0x00118D54
	private string GetValueString(int value, bool displayZero)
	{
		string color = (value > 0) ? "brightblue" : "brightred";
		if (!true)
		{
		}
		string result;
		if (value <= 0)
		{
			if (value >= 0)
			{
				result = (displayZero ? "0" : "");
			}
			else
			{
				result = (CommonUtils.GetDisplayStringForNum(value, 100000) ?? "").SetColor(color);
			}
		}
		else
		{
			result = ("+" + CommonUtils.GetDisplayStringForNum(value, 100000)).SetColor(color);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x0011ABD8 File Offset: 0x00118DD8
	private string GetValuePercentString(int value, bool reverseColor)
	{
		string color = (value > 0 == !reverseColor) ? "brightblue" : "brightred";
		if (!true)
		{
		}
		string result;
		if (value <= 0)
		{
			if (value >= 0)
			{
				result = "";
			}
			else
			{
				result = (CommonUtils.GetDisplayStringForNum(value, 100000) + "%").SetColor(color);
			}
		}
		else
		{
			result = ("+" + CommonUtils.GetDisplayStringForNum(value, 100000) + "%").SetColor(color);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x0011AC60 File Offset: 0x00118E60
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x0011AC90 File Offset: 0x00118E90
	private void OnEnable()
	{
		bool exist = UIElement.SystemOption.Exist;
		if (exist)
		{
			UIManager.Instance.HideUI(UIElement.SystemOption);
		}
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x0011ACBC File Offset: 0x00118EBC
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x0011ACEC File Offset: 0x00118EEC
	public override void QuickHide()
	{
		bool flag = !this.CloseButton.interactable;
		if (!flag)
		{
			foreach (GameObject obj in this._objList)
			{
				Object.Destroy(obj);
			}
			this._objList.Clear();
			base.CGet<CScrollRectLegacy>("ResultScroll").ScrollTo(Vector2.zero, 0.3f);
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			UIManager.Instance.HideUI(this.Element);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("LifeSkillBattleComplete", "WinState", this._isTaiwuWin);
			TaiwuEventDomainMethod.Call.TriggerListener("LifeSkillBattleComplete", true);
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
			UIManager.Instance.HideUI(UIElement.LifeSkillCombatOld);
			SingletonObject.getInstance<LifeSkillCombatModel>().End(this._isTaiwuWin);
			GEvent.OnEvent(UiEvents.OnTaiwuReadingBookProgressMayChange, null);
		}
	}

	// Token: 0x04001C5A RID: 7258
	private const float WinAniTime = 1.333f;

	// Token: 0x04001C5B RID: 7259
	private const float LoseAniTime = 1.333f;

	// Token: 0x04001C5C RID: 7260
	private const float FadeTime = 0.5f;

	// Token: 0x04001C5D RID: 7261
	private bool _isTaiwuWin;

	// Token: 0x04001C5E RID: 7262
	private DebateResult _debateResult;

	// Token: 0x04001C5F RID: 7263
	private readonly int _taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;

	// Token: 0x04001C60 RID: 7264
	private readonly List<GameObject> _objList = new List<GameObject>();
}
