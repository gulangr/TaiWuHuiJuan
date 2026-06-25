using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu.Debate;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate.DebateResult
{
	// Token: 0x02000AAC RID: 2732
	public class ViewDebateResult : UIBase
	{
		// Token: 0x0600862B RID: 34347 RVA: 0x003E72A4 File Offset: 0x003E54A4
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<DebateResult>("DebateResult", out this._debateResult);
			this.selfEvaluationScroll.OnItemRender += this.OnSelfItemRender;
			this.enemyEvaluationScroll.OnItemRender += this.OnEnemyItemRender;
			this.RefreshSelfEvaluation();
			this.selfCharacterArea.Refresh(this._debateResult, true);
			this.RefreshEnemyEvaluation();
			this.enemyCharacterArea.Refresh(this._debateResult, false);
			this.buttonConfirm.ClearAndAddListener(new Action(this.QuickHide));
			this.resultWinLose.Set(this._debateResult.IsTaiwuWin);
		}

		// Token: 0x0600862C RID: 34348 RVA: 0x003E7358 File Offset: 0x003E5558
		private void OnEnable()
		{
			bool exist = UIElement.SystemOption.Exist;
			if (exist)
			{
				UIManager.Instance.HideUI(UIElement.SystemOption);
			}
		}

		// Token: 0x0600862D RID: 34349 RVA: 0x003E7384 File Offset: 0x003E5584
		private void OnDisable()
		{
			this.selfEvaluationScroll.OnItemRender -= this.OnSelfItemRender;
			this.enemyEvaluationScroll.OnItemRender -= this.OnEnemyItemRender;
		}

		// Token: 0x0600862E RID: 34350 RVA: 0x003E73B8 File Offset: 0x003E55B8
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600862F RID: 34351 RVA: 0x003E73E8 File Offset: 0x003E55E8
		private void RefreshSelfEvaluation()
		{
			this._selfEvaluationDataList.Clear();
			bool showReadingEvent = this._debateResult.ShowReadingEvent;
			if (showReadingEvent)
			{
				ViewDebateResult.EvaluationData data = new ViewDebateResult.EvaluationData(33, ViewDebateResult.EEvaluationType.CombatEvaluation, 1);
				this._selfEvaluationDataList.Add(data);
			}
			bool showLoopingEvent = this._debateResult.ShowLoopingEvent;
			if (showLoopingEvent)
			{
				ViewDebateResult.EvaluationData data2 = new ViewDebateResult.EvaluationData(43, ViewDebateResult.EEvaluationType.CombatEvaluation, 1);
				this._selfEvaluationDataList.Add(data2);
			}
			foreach (short evaluation in this._debateResult.Evaluations)
			{
				ViewDebateResult.EvaluationData data3 = new ViewDebateResult.EvaluationData((int)evaluation, ViewDebateResult.EEvaluationType.DebateEvaluation, 1);
				this._selfEvaluationDataList.Add(data3);
			}
			foreach (KeyValuePair<short, int> keyValuePair in this._debateResult.TaiwuComments)
			{
				short num;
				int num2;
				keyValuePair.Deconstruct(out num, out num2);
				short comment = num;
				int count = num2;
				ViewDebateResult.EvaluationData data4 = new ViewDebateResult.EvaluationData((int)comment, ViewDebateResult.EEvaluationType.DebateComment, count);
				this._selfEvaluationDataList.Add(data4);
			}
			this.selfEvaluationScroll.SetDataCount(this._selfEvaluationDataList.Count);
		}

		// Token: 0x06008630 RID: 34352 RVA: 0x003E7540 File Offset: 0x003E5740
		private void RefreshEnemyEvaluation()
		{
			this._enemyEvaluationDataList.Clear();
			foreach (KeyValuePair<short, int> keyValuePair in this._debateResult.NpcComments)
			{
				short num;
				int num2;
				keyValuePair.Deconstruct(out num, out num2);
				short comment = num;
				int count = num2;
				ViewDebateResult.EvaluationData data = new ViewDebateResult.EvaluationData((int)comment, ViewDebateResult.EEvaluationType.DebateComment, count);
				this._enemyEvaluationDataList.Add(data);
			}
			this.enemyEvaluationScroll.SetDataCount(this._enemyEvaluationDataList.Count);
		}

		// Token: 0x06008631 RID: 34353 RVA: 0x003E75E4 File Offset: 0x003E57E4
		private void OnSelfItemRender(int index, GameObject obj)
		{
			ViewDebateResult.EvaluationData data = this._selfEvaluationDataList[index];
			this.OnEnemyItemRender(data, obj);
		}

		// Token: 0x06008632 RID: 34354 RVA: 0x003E7608 File Offset: 0x003E5808
		private void OnEnemyItemRender(int index, GameObject obj)
		{
			ViewDebateResult.EvaluationData data = this._enemyEvaluationDataList[index];
			this.OnEnemyItemRender(data, obj);
		}

		// Token: 0x06008633 RID: 34355 RVA: 0x003E762C File Offset: 0x003E582C
		private void OnEnemyItemRender(ViewDebateResult.EvaluationData data, GameObject obj)
		{
			string dataName;
			string dataDesc;
			switch (data.Type)
			{
			case ViewDebateResult.EEvaluationType.CombatEvaluation:
			{
				CombatEvaluationItem config = CombatEvaluation.Instance[data.TemplateId];
				dataName = config.Name;
				dataDesc = config.Desc;
				break;
			}
			case ViewDebateResult.EEvaluationType.DebateEvaluation:
			{
				DebateEvaluationItem config2 = DebateEvaluation.Instance[data.TemplateId];
				dataName = config2.Name;
				dataDesc = this.GetDebateEvaluationDesc(config2);
				break;
			}
			case ViewDebateResult.EEvaluationType.DebateComment:
			{
				DebateCommentItem config3 = Config.DebateComment.Instance[data.TemplateId];
				dataName = config3.Name;
				dataDesc = this.GetDebateCommentDesc(config3);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
			text.text = string.Format("{0}X{1}", dataName, data.Count);
			TooltipInvoker tip = obj.GetComponentInChildren<TooltipInvoker>();
			tip.Type = TipType.Simple;
			tip.PresetParam = new string[]
			{
				dataName,
				dataDesc
			};
		}

		// Token: 0x06008634 RID: 34356 RVA: 0x003E7718 File Offset: 0x003E5918
		private string GetDebateEvaluationDesc(DebateEvaluationItem config)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.AppendLine(config.ResultTip);
			bool flag = config.HappinessDelta != 0;
			if (flag)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Self) + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness) + ViewDebateResult.GetValueString(config.HappinessDelta, false));
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_LifeskillCombat_Spectator) + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness) + ViewDebateResult.GetValueString(config.HappinessDelta, false));
			}
			bool flag2 = config.ExpB != 0;
			if (flag2)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Exp) + ViewDebateResult.GetValuePercentString(config.ExpB, false));
			}
			bool flag3 = config.ExpC != 0;
			if (flag3)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Exp) + ViewDebateResult.GetValuePercentString(config.ExpC, false));
			}
			bool flag4 = config.AuthorityB != 0;
			if (flag4)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority) + ViewDebateResult.GetValuePercentString(config.AuthorityB, false));
			}
			bool flag5 = config.AuthorityC != 0;
			if (flag5)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_EventLog_Result_Gain) + LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority) + ViewDebateResult.GetValuePercentString(config.AuthorityC, false));
			}
			bool flag6 = config.FavorIncreaseB != 0;
			if (flag6)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityUpInfection) + ViewDebateResult.GetValuePercentString(config.FavorIncreaseB, false));
			}
			bool flag7 = config.FavorIncreaseC != 0;
			if (flag7)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityUpInfection) + ViewDebateResult.GetValuePercentString(config.FavorIncreaseC, false));
			}
			bool flag8 = config.FavorDecreaseB != 0;
			if (flag8)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityDownInfection) + ViewDebateResult.GetValuePercentString(config.FavorDecreaseB, true));
			}
			bool flag9 = config.FavorDecreaseC != 0;
			if (flag9)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_FavorabilityDownInfection) + ViewDebateResult.GetValuePercentString(config.FavorDecreaseC, true));
			}
			string desc = sb.ToString();
			sb.Clear();
			EasyPool.Free<StringBuilder>(sb);
			return desc;
		}

		// Token: 0x06008635 RID: 34357 RVA: 0x003E7974 File Offset: 0x003E5B74
		private string GetDebateCommentDesc(DebateCommentItem config)
		{
			StringBuilder sb = new StringBuilder();
			sbyte behaviorType = SingletonObject.getInstance<LifeSkillCombatModel>().TaiwuCharData.BehaviorType;
			sb.AppendLine(config.ResultTip);
			bool flag = config.Happiness[(int)behaviorType] != 0;
			if (flag)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Self) + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness) + ViewDebateResult.GetValueString((int)config.Happiness[(int)behaviorType], false));
			}
			bool flag2 = config.Favor != 0;
			if (flag2)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_LifeskillCombat_Spectator) + LocalStringManager.Get(LanguageKey.LK_Favorability) + ViewDebateResult.GetValueString((int)config.Favor, false));
			}
			string desc = sb.ToString();
			sb.Clear();
			EasyPool.Free<StringBuilder>(sb);
			return desc;
		}

		// Token: 0x06008636 RID: 34358 RVA: 0x003E7A3C File Offset: 0x003E5C3C
		public override void QuickHide()
		{
			base.QuickHide();
			AudioManager.Instance.PlaySound("ui_default_confirm", false, false);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("LifeSkillBattleComplete", "WinState", this._debateResult.IsTaiwuWin);
			TaiwuEventDomainMethod.Call.TriggerListener("LifeSkillBattleComplete", true);
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
			UIManager.Instance.HideUI(UIElement.LifeSkillCombatOld);
			UIManager.Instance.HideUI(UIElement.Debate);
			SingletonObject.getInstance<LifeSkillCombatModel>().End(this._debateResult.IsTaiwuWin);
			GEvent.OnEvent(UiEvents.OnTaiwuReadingBookProgressMayChange, null);
		}

		// Token: 0x06008637 RID: 34359 RVA: 0x003E7AE8 File Offset: 0x003E5CE8
		public static string GetValueString(int value, bool displayZero)
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

		// Token: 0x06008638 RID: 34360 RVA: 0x003E7B6C File Offset: 0x003E5D6C
		public static string GetValuePercentString(int value, bool reverseColor)
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

		// Token: 0x040066FF RID: 26367
		[SerializeField]
		private InfinityScroll selfEvaluationScroll;

		// Token: 0x04006700 RID: 26368
		[SerializeField]
		private InfinityScroll enemyEvaluationScroll;

		// Token: 0x04006701 RID: 26369
		[SerializeField]
		private DebateResultCharacterArea selfCharacterArea;

		// Token: 0x04006702 RID: 26370
		[SerializeField]
		private DebateResultCharacterArea enemyCharacterArea;

		// Token: 0x04006703 RID: 26371
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04006704 RID: 26372
		[SerializeField]
		private DebateResultWinLose resultWinLose;

		// Token: 0x04006705 RID: 26373
		private DebateResult _debateResult;

		// Token: 0x04006706 RID: 26374
		private readonly List<ViewDebateResult.EvaluationData> _selfEvaluationDataList = new List<ViewDebateResult.EvaluationData>();

		// Token: 0x04006707 RID: 26375
		private readonly List<ViewDebateResult.EvaluationData> _enemyEvaluationDataList = new List<ViewDebateResult.EvaluationData>();

		// Token: 0x0200206F RID: 8303
		private class EvaluationData
		{
			// Token: 0x0600F73A RID: 63290 RVA: 0x00628B7E File Offset: 0x00626D7E
			public EvaluationData(int templateId, ViewDebateResult.EEvaluationType type, int count)
			{
				this.TemplateId = templateId;
				this.Type = type;
				this.Count = count;
			}

			// Token: 0x0400D10E RID: 53518
			public readonly int TemplateId;

			// Token: 0x0400D10F RID: 53519
			public readonly ViewDebateResult.EEvaluationType Type;

			// Token: 0x0400D110 RID: 53520
			public readonly int Count;
		}

		// Token: 0x02002070 RID: 8304
		private enum EEvaluationType
		{
			// Token: 0x0400D112 RID: 53522
			CombatEvaluation,
			// Token: 0x0400D113 RID: 53523
			DebateEvaluation,
			// Token: 0x0400D114 RID: 53524
			DebateComment
		}
	}
}
