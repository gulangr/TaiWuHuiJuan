using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.CombatSkill;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AFF RID: 2815
	public class CombatStateAssistSkill : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F49 RID: 3913
		// (get) Token: 0x06008A8F RID: 35471 RVA: 0x00402415 File Offset: 0x00400615
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A90 RID: 35472 RVA: 0x0040241C File Offset: 0x0040061C
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnCombatSkillDirectionChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model.OnCombatSkillDirectionChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillDirectionChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			CombatModel model2 = this.Model;
			model2.OnAssistSkillListChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnAssistSkillListChanged, new OnDataChangedEvent(this.OnAssistSkillListChanged));
		}

		// Token: 0x06008A91 RID: 35473 RVA: 0x00402494 File Offset: 0x00400694
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnCombatSkillDirectionChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model.OnCombatSkillDirectionChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillDirectionChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			CombatModel model2 = this.Model;
			model2.OnAssistSkillListChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnAssistSkillListChanged, new OnDataChangedEvent(this.OnAssistSkillListChanged));
		}

		// Token: 0x06008A92 RID: 35474 RVA: 0x0040250C File Offset: 0x0040070C
		private void OnChangeChar()
		{
			int currCharId = this.Model.ChangingToCharId;
			bool flag = this.Model.CharIsAlly(currCharId) != this.ally;
			if (!flag)
			{
				this.UpdateCount(currCharId);
				this.UpdateMouseTip(this.ally, currCharId);
			}
		}

		// Token: 0x06008A93 RID: 35475 RVA: 0x0040255C File Offset: 0x0040075C
		private void OnAssistSkillListChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int currCharId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateCount(currCharId);
				this.UpdateMouseTip(isAlly, currCharId);
			}
		}

		// Token: 0x06008A94 RID: 35476 RVA: 0x004025AC File Offset: 0x004007AC
		private void UpdateMouseTip(bool isAlly, int currCharId)
		{
			CombatStateAssistSkill.<>c__DisplayClass8_0 CS$<>8__locals1 = new CombatStateAssistSkill.<>c__DisplayClass8_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.currCharId = currCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(CS$<>8__locals1.currCharId, out processor);
			if (!flag)
			{
				CombatStateAssistSkill.<>c__DisplayClass8_0 CS$<>8__locals2 = CS$<>8__locals1;
				CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(CS$<>8__locals1.currCharId);
				CS$<>8__locals2.assistSkillList = ((orDefault != null) ? orDefault.AssistSkillList : null);
				List<short> matchList = (from pair in this.Model.ProcessorSkills
				where CS$<>8__locals1.<>4__this.IsMatchingSkill(pair, CS$<>8__locals1.currCharId, CS$<>8__locals1.assistSkillList)
				select pair.Key.SkillTemplateId).ToList<short>();
				TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.GetComponent<DisableStyleRoot>().SetStyleEffect(matchList == null || matchList.Count <= 0, false);
				bool flag2 = matchList != null && matchList.Count > 0;
				if (flag2)
				{
					mouseTip.Type = TipType.CombatSkillBuff;
					mouseTip.RuntimeParam.Clear();
					mouseTip.RuntimeParam.Set("IsAlly", isAlly);
					mouseTip.RuntimeParam.Set("IsNeiGong", false);
					mouseTip.RuntimeParam.Set("CharId", CS$<>8__locals1.currCharId);
					mouseTip.RuntimeParam.SetObject("CombatSkillIdList", matchList);
				}
				else
				{
					mouseTip.Type = TipType.SingleDesc;
					mouseTip.RuntimeParam.Clear();
					mouseTip.RuntimeParam.Set("arg0", LanguageKey.LK_Combat_Jueji_Empty_Tip.Tr());
				}
			}
		}

		// Token: 0x06008A95 RID: 35477 RVA: 0x00402750 File Offset: 0x00400950
		private void OnCombatSkillDirectionChanged(CombatSkillKey combatSkillKey)
		{
			int currCharId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
			int combatSkillCharId = combatSkillKey.CharId;
			bool flag = combatSkillCharId != currCharId;
			if (!flag)
			{
				this.UpdateCount(currCharId);
				this.UpdateMouseTip(this.ally, currCharId);
			}
		}

		// Token: 0x06008A96 RID: 35478 RVA: 0x004027AC File Offset: 0x004009AC
		private void UpdateCount(int currCharId)
		{
			CombatStateAssistSkill.<>c__DisplayClass10_0 CS$<>8__locals1 = new CombatStateAssistSkill.<>c__DisplayClass10_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.currCharId = currCharId;
			CombatStateAssistSkill.<>c__DisplayClass10_0 CS$<>8__locals2 = CS$<>8__locals1;
			CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(CS$<>8__locals1.currCharId);
			CS$<>8__locals2.assistSkillList = ((orDefault != null) ? orDefault.AssistSkillList : null);
			int count = this.Model.ProcessorSkills.Count((KeyValuePair<CombatSkillKey, CombatSubProcessorSkill> pair) => CS$<>8__locals1.<>4__this.IsMatchingSkill(pair, CS$<>8__locals1.currCharId, CS$<>8__locals1.assistSkillList));
			this.txtCount.text = count.ToString();
		}

		// Token: 0x06008A97 RID: 35479 RVA: 0x00402828 File Offset: 0x00400A28
		private bool IsMatchingSkill(KeyValuePair<CombatSkillKey, CombatSubProcessorSkill> pair, int currCharId, List<short> neigongList)
		{
			return pair.Key.CharId == currCharId && neigongList != null && neigongList.Contains(pair.Key.SkillTemplateId);
		}

		// Token: 0x04006A49 RID: 27209
		[SerializeField]
		private bool ally;

		// Token: 0x04006A4A RID: 27210
		[SerializeField]
		private TextMeshProUGUI txtCount;
	}
}
