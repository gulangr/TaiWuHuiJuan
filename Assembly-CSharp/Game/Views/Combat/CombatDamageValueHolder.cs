using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B26 RID: 2854
	public class CombatDamageValueHolder : MonoBehaviour
	{
		// Token: 0x06008C05 RID: 35845 RVA: 0x0040B500 File Offset: 0x00409700
		private static string ParseInjurySprite(int bodyPart, bool inner, bool reachLimit)
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			builder.Append("combat_top_value_");
			builder.Append(inner ? "inner" : "outer");
			builder.Append("injury_");
			if (reachLimit)
			{
				builder.Append("fatal_");
			}
			builder.Append(bodyPart);
			string ret = builder.ToString();
			EasyPool.Free<StringBuilder>(builder);
			return ret;
		}

		// Token: 0x06008C06 RID: 35846 RVA: 0x0040B578 File Offset: 0x00409778
		public void SetQiDisorder(int charId, short qiDisorder)
		{
			TooltipInvoker mouseTip = this.qiDisorder;
			CharacterDomainMethod.AsyncCall.GetCharacterInjuryDisplayData(null, charId, delegate(int offset, RawDataPool pool)
			{
				CharacterInjuryDisplayData displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				TooltipInvoker mouseTip = mouseTip;
				ArgumentBox argumentBox;
				if ((argumentBox = mouseTip.RuntimeParam) == null)
				{
					argumentBox = (mouseTip.RuntimeParam = new ArgumentBox());
				}
				ArgumentBox args = argumentBox;
				args.Clear();
				args.Set<CharacterInjuryDisplayData>("CharacterInjuryDisplayData", displayData);
				args.Set("CombatStyle", true);
			});
			CImage progress = mouseTip.transform.GetChild(0).GetComponent<CImage>();
			sbyte markCount = DefeatMarkCollection.CalcQiDisorderMarkCount((int)qiDisorder);
			bool fillMaxForce = qiDisorder == DisorderLevelOfQi.MaxValue;
			bool noMarkStyle = markCount == 0;
			short threshold = DefeatMarkCollection.CalcQiDisorderMarkThreshold((int)qiDisorder);
			float progressZeroToOne = (float)(qiDisorder % threshold) / (float)threshold;
			progress.SetSprite(noMarkStyle ? "combat_top_value_qidisorder_1" : "combat_top_value_qidisorder_0", false, null);
			progress.fillAmount = (fillMaxForce ? 1f : (noMarkStyle ? (1f - progressZeroToOne) : progressZeroToOne));
		}

		// Token: 0x06008C07 RID: 35847 RVA: 0x0040B626 File Offset: 0x00409826
		public void SetStep(DamageStepCollection stepCollection)
		{
			this._stepCollection = stepCollection;
		}

		// Token: 0x06008C08 RID: 35848 RVA: 0x0040B630 File Offset: 0x00409830
		public void SetMark(DefeatMarkCollection currCollection, CombatSubProcessorCharacter processor)
		{
			for (sbyte i = 0; i < 7; i += 1)
			{
				sbyte part = i;
				this.SetTip(this.bodyPartList[(int)i], delegate(CombatDamageValueLayoutData data)
				{
					data.MarkCount = (int)currCollection.OuterInjuryMarkList[(int)part];
					data.ReachLimit = (data.MarkCount == 6);
					data.DamageValue = new IntPair(data.DamageValue.First, processor.ContainsBodyPart(part) ? data.DamageValue.Second : -1);
				}, EMarkType.Outer, (int)i);
				this.SetTip(this.bodyPartList[(int)i], delegate(CombatDamageValueLayoutData data)
				{
					data.MarkCount = (int)currCollection.InnerInjuryMarkList[(int)part];
					data.ReachLimit = (data.MarkCount == 6);
					data.DamageValue = new IntPair(data.DamageValue.First, processor.ContainsBodyPart(part) ? data.DamageValue.Second : -1);
				}, EMarkType.Inner, (int)i);
				bool anyLimit = currCollection.OuterInjuryMarkList[(int)part] == 6 || currCollection.InnerInjuryMarkList[(int)part] == 6;
				this.bodyPartList[(int)i].transform.GetChild(2).gameObject.SetActive(anyLimit);
			}
			for (int j = 0; j < 6; j++)
			{
				int type = j;
				this.SetTip(this.poisonList[j], delegate(CombatDamageValueLayoutData data)
				{
					data.MarkCount = (int)currCollection.PoisonMarkList[type];
				}, EMarkType.Poison, j);
			}
			this.SetTip(this.mind, delegate(CombatDamageValueLayoutData data)
			{
				data.MarkCount = currCollection.MindMarkList.Count;
			}, EMarkType.Mind, 0);
			this.SetTip(this.fatal, delegate(CombatDamageValueLayoutData data)
			{
				data.MarkCount = currCollection.FatalDamageMarkCount;
			}, EMarkType.Fatal, 0);
			this.SetTip(this.state, delegate(CombatDamageValueLayoutData data)
			{
				data.MarkCount = (int)currCollection.StateMarkCount;
			}, EMarkType.State, 0);
		}

		// Token: 0x06008C09 RID: 35849 RVA: 0x0040B7C4 File Offset: 0x004099C4
		public void SetHeavyOrBreak(HeavyOrBreakInjuryData heavyOrBreak)
		{
			for (int i = 0; i < 7; i++)
			{
				int part = i;
				this.SetTip(this.bodyPartList[i], delegate(CombatDamageValueLayoutData data)
				{
					data.HeavyOrBreak = heavyOrBreak[part];
				}, EMarkType.Outer, i);
				this.SetTip(this.bodyPartList[i], delegate(CombatDamageValueLayoutData data)
				{
					data.HeavyOrBreak = heavyOrBreak[part];
				}, EMarkType.Inner, i);
			}
		}

		// Token: 0x06008C0A RID: 35850 RVA: 0x0040B847 File Offset: 0x00409A47
		public void SetOuter(int[] values, CombatSubProcessorCharacter processor = null)
		{
			this.SetOuterOrInner(values, false, processor);
		}

		// Token: 0x06008C0B RID: 35851 RVA: 0x0040B853 File Offset: 0x00409A53
		public void SetInner(int[] values, CombatSubProcessorCharacter processor = null)
		{
			this.SetOuterOrInner(values, true, processor);
		}

		// Token: 0x06008C0C RID: 35852 RVA: 0x0040B860 File Offset: 0x00409A60
		public void SetMind(int value)
		{
			int step = this._stepCollection.MindDamageStep;
			this.SetTipProgress(this.mind, value, step, EMarkType.Mind, 0);
		}

		// Token: 0x06008C0D RID: 35853 RVA: 0x0040B88C File Offset: 0x00409A8C
		public void SetFatal(int value)
		{
			int step = this._stepCollection.FatalDamageStep;
			this.SetTipProgress(this.fatal, value, step, EMarkType.Fatal, 0);
		}

		// Token: 0x06008C0E RID: 35854 RVA: 0x0040B8B8 File Offset: 0x00409AB8
		public unsafe void SetPoison(PoisonInts poisons)
		{
			for (int i = 0; i < 6; i++)
			{
				sbyte level = PoisonsAndLevels.CalcPoisonedLevel(*poisons[i]);
				CombatDamageValueHolder.<>c__DisplayClass16_0 CS$<>8__locals1;
				CS$<>8__locals1.thresholds = GlobalConfig.Instance.PoisonLevelThresholds;
				CS$<>8__locals1.maxPoison = 25000;
				int value = *poisons[i] - CombatDamageValueHolder.<SetPoison>g__CalcThreshold|16_0((int)(level - 1), ref CS$<>8__locals1);
				int step = (level == 3) ? -1 : (CombatDamageValueHolder.<SetPoison>g__CalcThreshold|16_0((int)level, ref CS$<>8__locals1) - CombatDamageValueHolder.<SetPoison>g__CalcThreshold|16_0((int)(level - 1), ref CS$<>8__locals1));
				Transform markRoot = this.poisonList[i].transform.GetChild(1);
				for (int j = 0; j < markRoot.childCount; j++)
				{
					markRoot.GetChild(j).gameObject.SetActive(j < (int)level);
				}
				this.SetTipProgress(this.poisonList[i], value, step, EMarkType.Poison, i);
			}
		}

		// Token: 0x06008C0F RID: 35855 RVA: 0x0040B9A4 File Offset: 0x00409BA4
		public void SetState(int buffPower)
		{
			int debuffPower = -buffPower;
			short powerPerMark = GlobalConfig.Instance.DefeatMarkCombatStatePower;
			sbyte maxMarkCount = GlobalConfig.Instance.DefeatMarkCombatStateMaxCount;
			int value = (debuffPower < 0) ? debuffPower : (debuffPower % (int)powerPerMark);
			int step = (int)((debuffPower >= (int)(powerPerMark * (short)maxMarkCount)) ? -1 : powerPerMark);
			this.SetTipProgress(this.state, value, step, EMarkType.State, 0);
		}

		// Token: 0x06008C10 RID: 35856 RVA: 0x0040B9F8 File Offset: 0x00409BF8
		private void SetOuterOrInner(IReadOnlyList<int> values, bool inner, CombatSubProcessorCharacter processor = null)
		{
			int[] steps = inner ? this._stepCollection.InnerDamageSteps : this._stepCollection.OuterDamageSteps;
			for (int i = 0; i < 7; i++)
			{
				int step = (processor != null && !processor.ContainsBodyPart((sbyte)i)) ? -1 : steps[i];
				this.SetTipProgress(this.bodyPartList[i], values[i], step, inner ? EMarkType.Inner : EMarkType.Outer, i);
			}
		}

		// Token: 0x06008C11 RID: 35857 RVA: 0x0040BA6C File Offset: 0x00409C6C
		private void SetTipProgress(TooltipInvoker mouseTip, int value, int step, EMarkType markType, int type = 0)
		{
			this.SetTip(mouseTip, delegate(CombatDamageValueLayoutData data)
			{
				data.DamageValue = new IntPair(value, step);
			}, markType, type);
		}

		// Token: 0x06008C12 RID: 35858 RVA: 0x0040BAA8 File Offset: 0x00409CA8
		private void SetTip(TooltipInvoker mouseTip, Action<CombatDamageValueLayoutData> handler, EMarkType type, int subType = 0)
		{
			ArgumentBox argumentBox;
			if ((argumentBox = mouseTip.RuntimeParam) == null)
			{
				argumentBox = (mouseTip.RuntimeParam = new ArgumentBox());
			}
			ArgumentBox args = argumentBox;
			bool optional = type == EMarkType.Inner;
			string key = optional ? "DamageValueOptional" : "DamageValue";
			CombatDamageValueLayoutData data;
			bool flag = !args.Get<CombatDamageValueLayoutData>(key, out data);
			if (flag)
			{
				DefeatMarkKey markKey = new DefeatMarkKey(type, subType, 0);
				data = new CombatDamageValueLayoutData(markKey);
				args.SetObject(key, data);
			}
			handler(data);
			bool showing = mouseTip.Showing;
			if (showing)
			{
				mouseTip.ShowTips();
			}
			IntPair damageValue = data.DamageValue;
			int num;
			int num2;
			damageValue.Deconstruct(out num, out num2);
			int value = num;
			int step = num2;
			CImage progress = mouseTip.transform.GetChild(optional ? 1 : 0).GetComponent<CImage>();
			bool flag2 = (type == EMarkType.Outer || type == EMarkType.Inner) && step < 0;
			if (flag2)
			{
				progress.gameObject.SetActive(false);
			}
			else
			{
				progress.gameObject.SetActive(true);
				progress.fillAmount = ((data.ReachLimit || step <= 0) ? 1f : ((float)value / (float)step));
			}
		}

		// Token: 0x06008C14 RID: 35860 RVA: 0x0040BBCC File Offset: 0x00409DCC
		[CompilerGenerated]
		internal static int <SetPoison>g__CalcThreshold|16_0(int index, ref CombatDamageValueHolder.<>c__DisplayClass16_0 A_1)
		{
			return (index < 0) ? 0 : ((index >= A_1.thresholds.Length) ? A_1.maxPoison : ((int)A_1.thresholds[index]));
		}

		// Token: 0x04006B2B RID: 27435
		[SerializeField]
		private List<TooltipInvoker> bodyPartList;

		// Token: 0x04006B2C RID: 27436
		[SerializeField]
		private TooltipInvoker mind;

		// Token: 0x04006B2D RID: 27437
		[SerializeField]
		private TooltipInvoker fatal;

		// Token: 0x04006B2E RID: 27438
		[SerializeField]
		private TooltipInvoker qiDisorder;

		// Token: 0x04006B2F RID: 27439
		[SerializeField]
		private List<TooltipInvoker> poisonList;

		// Token: 0x04006B30 RID: 27440
		[SerializeField]
		private TooltipInvoker state;

		// Token: 0x04006B31 RID: 27441
		private DamageStepCollection _stepCollection;
	}
}
