using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200084B RID: 2123
	public class MouseTipCombatSkillBanReason : MouseTipBase
	{
		// Token: 0x17000C65 RID: 3173
		// (get) Token: 0x06006713 RID: 26387 RVA: 0x002F05A4 File Offset: 0x002EE7A4
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C66 RID: 3174
		// (get) Token: 0x06006714 RID: 26388 RVA: 0x002F05A7 File Offset: 0x002EE7A7
		private CombatModel CombatModel
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000C67 RID: 3175
		// (get) Token: 0x06006715 RID: 26389 RVA: 0x002F05AE File Offset: 0x002EE7AE
		private int BanReasonCount
		{
			get
			{
				IReadOnlyList<CombatSkillBanReasonData> readOnlyList = this._banReasons;
				return (readOnlyList != null) ? readOnlyList.Count : 0;
			}
		}

		// Token: 0x17000C68 RID: 3176
		// (get) Token: 0x06006716 RID: 26390 RVA: 0x002F05C2 File Offset: 0x002EE7C2
		private int EffectDataCount
		{
			get
			{
				IReadOnlyList<CombatSkillEffectData> effectData = this._effectData;
				return (effectData != null) ? effectData.Count : 0;
			}
		}

		// Token: 0x06006717 RID: 26391 RVA: 0x002F05D6 File Offset: 0x002EE7D6
		protected override void Init(ArgumentBox argsBox)
		{
			this.ExtractArgumentBox(argsBox);
		}

		// Token: 0x06006718 RID: 26392 RVA: 0x002F05E1 File Offset: 0x002EE7E1
		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshByField();
		}

		// Token: 0x06006719 RID: 26393 RVA: 0x002F05F4 File Offset: 0x002EE7F4
		public override void Refresh(ArgumentBox argBox)
		{
			bool flag = !this.ExtractArgumentBox(argBox);
			if (!flag)
			{
				this.RefreshByField();
			}
		}

		// Token: 0x0600671A RID: 26394 RVA: 0x002F061C File Offset: 0x002EE81C
		private bool ExtractArgumentBox(ArgumentBox argBox)
		{
			short combatSkillId;
			argBox.Get("CombatSkillId", out combatSkillId);
			this._configData = CombatSkill.Instance[combatSkillId];
			int charId;
			argBox.Get("CharId", out charId);
			CombatSubProcessorSkill processorSkill;
			bool flag = !this.CombatModel.ProcessorSkills.TryGetValue(new ValueTuple<int, short>(charId, combatSkillId), out processorSkill);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this._banReasons = processorSkill.BanReason;
				this._effectData = processorSkill.EffectData;
				result = true;
			}
			return result;
		}

		// Token: 0x0600671B RID: 26395 RVA: 0x002F06A0 File Offset: 0x002EE8A0
		private void RefreshByField()
		{
			this.title.text = this._configData.Name;
			this.UpdateContent();
		}

		// Token: 0x0600671C RID: 26396 RVA: 0x002F06C4 File Offset: 0x002EE8C4
		private void UpdateContent()
		{
			bool emptyReason = this.BanReasonCount == 0;
			this.banReasons.gameObject.SetActive(!emptyReason);
			bool flag = emptyReason;
			if (!flag)
			{
				this.banReasons.Rebuild<TMP_Text>(this.BanReasonCount, delegate(TMP_Text text, int i)
				{
					text.text = this.GetReasonText(this._banReasons[i]).ColorReplace();
					text.GetComponent<TMPTextSpriteHelper>().Parse();
				});
				ValueTuple<CombatSkillEffectData, LanguageKey>[] data = (from y in this._effectData
				select new ValueTuple<CombatSkillEffectData, LanguageKey>(y, MouseTipCombatSkillBanReason.<UpdateContent>g__GetLanguageKey|19_1(y.Type)) into x
				where x.Item2 != LanguageKey.Invalid
				select x).ToArray<ValueTuple<CombatSkillEffectData, LanguageKey>>();
				bool flag2 = data == null || data.Length <= 0;
				if (flag2)
				{
					this.effects.gameObject.SetActive(false);
				}
				else
				{
					this.effects.gameObject.SetActive(true);
					this.effects.Rebuild<TMP_Text>(data.Length, delegate(TMP_Text text, int i)
					{
						text.text = LocalStringManager.GetFormat(data[i].Item2, data[i].Item1.Value).ColorReplace();
						text.GetComponent<TMPTextSpriteHelper>().Parse();
					});
				}
			}
		}

		// Token: 0x0600671D RID: 26397 RVA: 0x002F07E4 File Offset: 0x002EE9E4
		private string GetReasonText(CombatSkillBanReasonData banReason)
		{
			switch (banReason.Type)
			{
			case ECombatSkillBanReasonType.StanceNotEnough:
				return LanguageKey.LK_CombatSkill_Tip_Simple_Stance.TrFormat(MouseTipCombatSkillBanReason.GetColored(banReason.HasStance, banReason.CostStance), banReason.CostStance);
			case ECombatSkillBanReasonType.BreathNotEnough:
				return LanguageKey.LK_CombatSkill_Tip_Simple_Breath.TrFormat(MouseTipCombatSkillBanReason.GetColored(banReason.HasBreath, banReason.CostBreath), banReason.CostBreath);
			case ECombatSkillBanReasonType.MobilityNotEnough:
				return LanguageKey.LK_CombatSkill_Tip_Simple_Mobility.TrFormat(MouseTipCombatSkillBanReason.GetColored(banReason.HasMobility, banReason.CostMobility), banReason.CostMobility);
			case ECombatSkillBanReasonType.TrickNotEnough:
				return this.ParseTricksNotEnoughString(banReason);
			case ECombatSkillBanReasonType.WugNotEnough:
				return LanguageKey.LK_CombatSkill_Tip_Simple_Trick_Beetle.TrFormat(MouseTipCombatSkillBanReason.GetColored(banReason.HasWug, banReason.CostWug), banReason.CostWug);
			case ECombatSkillBanReasonType.NeiliAllocationNotEnough:
				return LanguageKey.LK_CombatSkill_Tip_Simple_Trick_Beetle.TrFormat(MouseTipCombatSkillBanReason.GetColored(banReason.HasWug, banReason.CostWug), banReason.CostWug);
			}
			return LocalStringManager.Get("LK_MouseTip_BanReasons_" + banReason.Type.ToString()).SetColor("brightred");
		}

		// Token: 0x0600671E RID: 26398 RVA: 0x002F0968 File Offset: 0x002EEB68
		private string ParseTricksNotEnoughString(CombatSkillBanReasonData banReason)
		{
			return LanguageKey.LK_CombatSkill_Tip_Simple_Trick.TrFormat(string.Join(LanguageKey.LK_CombatSkill_Tip_Simple_Sep.Tr(), banReason.HasTricks.Zip(banReason.CostTricks, (NeedTrick has, NeedTrick cost) => LanguageKey.LK_CombatSkill_Tip_Simple_Trick_Item.TrFormat(has.TrickType, MouseTipCombatSkillBanReason.GetColored((int)has.NeedCount, (int)cost.NeedCount), cost.NeedCount))));
		}

		// Token: 0x0600671F RID: 26399 RVA: 0x002F09BE File Offset: 0x002EEBBE
		private static string GetColored(int val, int need)
		{
			return val.ToString().SetColor((val < need) ? "brightred" : "brightblue");
		}

		// Token: 0x06006722 RID: 26402 RVA: 0x002F0A14 File Offset: 0x002EEC14
		[CompilerGenerated]
		internal static LanguageKey <UpdateContent>g__GetLanguageKey|19_1(ECombatSkillEffectType type)
		{
			if (!true)
			{
			}
			LanguageKey result;
			if (type != ECombatSkillEffectType.TaiJiQuanLeveragingValue)
			{
				if (type != ECombatSkillEffectType.ShuiHuoYingQiGongReduceDamage)
				{
					result = LanguageKey.Invalid;
				}
				else
				{
					result = LanguageKey.LK_MouseTip_EffectData_ShuiHuoYingQiGongReduceDamage;
				}
			}
			else
			{
				result = LanguageKey.LK_MouseTip_EffectData_TaiJiQuanLeveragingValue;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x040048AE RID: 18606
		[SerializeField]
		private TMP_Text title;

		// Token: 0x040048AF RID: 18607
		[SerializeField]
		private TemplatedContainerAssemblyNew banReasons;

		// Token: 0x040048B0 RID: 18608
		[SerializeField]
		private TemplatedContainerAssemblyNew effects;

		// Token: 0x040048B1 RID: 18609
		private IReadOnlyList<CombatSkillBanReasonData> _banReasons;

		// Token: 0x040048B2 RID: 18610
		private IReadOnlyList<CombatSkillEffectData> _effectData;

		// Token: 0x040048B3 RID: 18611
		private CombatSkillItem _configData;
	}
}
