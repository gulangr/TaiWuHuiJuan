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
	// Token: 0x02000B00 RID: 2816
	public class CombatStateNeigong : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F4A RID: 3914
		// (get) Token: 0x06008A99 RID: 35481 RVA: 0x0040286A File Offset: 0x00400A6A
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A9A RID: 35482 RVA: 0x00402874 File Offset: 0x00400A74
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnCombatSkillDirectionChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model.OnCombatSkillDirectionChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillDirectionChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			CombatModel model2 = this.Model;
			model2.OnNeigongListChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnNeigongListChanged, new OnDataChangedEvent(this.OnNeigongListChanged));
		}

		// Token: 0x06008A9B RID: 35483 RVA: 0x004028EC File Offset: 0x00400AEC
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnCombatSkillDirectionChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model.OnCombatSkillDirectionChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillDirectionChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			CombatModel model2 = this.Model;
			model2.OnNeigongListChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnNeigongListChanged, new OnDataChangedEvent(this.OnNeigongListChanged));
		}

		// Token: 0x06008A9C RID: 35484 RVA: 0x00402964 File Offset: 0x00400B64
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

		// Token: 0x06008A9D RID: 35485 RVA: 0x004029B4 File Offset: 0x00400BB4
		private void OnNeigongListChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int currCharId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateCount(currCharId);
				this.UpdateMouseTip(isAlly, currCharId);
			}
		}

		// Token: 0x06008A9E RID: 35486 RVA: 0x00402A04 File Offset: 0x00400C04
		private void UpdateMouseTip(bool isAlly, int currCharId)
		{
			CombatStateNeigong.<>c__DisplayClass8_0 CS$<>8__locals1 = new CombatStateNeigong.<>c__DisplayClass8_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.currCharId = currCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(CS$<>8__locals1.currCharId, out processor);
			if (!flag)
			{
				CombatStateNeigong.<>c__DisplayClass8_0 CS$<>8__locals2 = CS$<>8__locals1;
				CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(CS$<>8__locals1.currCharId);
				CS$<>8__locals2.neigongList = ((orDefault != null) ? orDefault.NeigongList : null);
				List<short> matchNeigongList = (from pair in this.Model.ProcessorSkills
				where CS$<>8__locals1.<>4__this.IsMatchingSkill(pair, CS$<>8__locals1.currCharId, CS$<>8__locals1.neigongList)
				select pair.Key.SkillTemplateId).ToList<short>();
				TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.GetComponent<DisableStyleRoot>().SetStyleEffect(matchNeigongList == null || matchNeigongList.Count <= 0, false);
				bool flag2 = matchNeigongList != null && matchNeigongList.Count > 0;
				if (flag2)
				{
					mouseTip.Type = TipType.CombatSkillBuff;
					mouseTip.RuntimeParam.Clear();
					mouseTip.RuntimeParam.Set("IsAlly", isAlly);
					mouseTip.RuntimeParam.Set("IsNeiGong", true);
					mouseTip.RuntimeParam.Set("CharId", CS$<>8__locals1.currCharId);
					mouseTip.RuntimeParam.SetObject("CombatSkillIdList", matchNeigongList);
				}
				else
				{
					mouseTip.Type = TipType.SingleDesc;
					mouseTip.RuntimeParam.Clear();
					mouseTip.RuntimeParam.Set("arg0", LanguageKey.LK_Combat_Neigong_Empty_Tip.Tr());
				}
			}
		}

		// Token: 0x06008A9F RID: 35487 RVA: 0x00402BA8 File Offset: 0x00400DA8
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

		// Token: 0x06008AA0 RID: 35488 RVA: 0x00402C04 File Offset: 0x00400E04
		private void UpdateCount(int currCharId)
		{
			CombatStateNeigong.<>c__DisplayClass10_0 CS$<>8__locals1 = new CombatStateNeigong.<>c__DisplayClass10_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.currCharId = currCharId;
			CombatStateNeigong.<>c__DisplayClass10_0 CS$<>8__locals2 = CS$<>8__locals1;
			CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(CS$<>8__locals1.currCharId);
			CS$<>8__locals2.neigongList = ((orDefault != null) ? orDefault.NeigongList : null);
			int count = this.Model.ProcessorSkills.Count((KeyValuePair<CombatSkillKey, CombatSubProcessorSkill> pair) => CS$<>8__locals1.<>4__this.IsMatchingSkill(pair, CS$<>8__locals1.currCharId, CS$<>8__locals1.neigongList));
			this.txtCount.text = count.ToString();
		}

		// Token: 0x06008AA1 RID: 35489 RVA: 0x00402C80 File Offset: 0x00400E80
		private bool IsMatchingSkill(KeyValuePair<CombatSkillKey, CombatSubProcessorSkill> pair, int currCharId, List<short> neigongList)
		{
			return pair.Key.CharId == currCharId && neigongList != null && neigongList.Contains(pair.Key.SkillTemplateId) && CombatSkillStateHelper.IsBrokenOut(pair.Value.ActivationState);
		}

		// Token: 0x04006A4B RID: 27211
		[SerializeField]
		private bool ally;

		// Token: 0x04006A4C RID: 27212
		[SerializeField]
		private TextMeshProUGUI txtCount;
	}
}
