using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A0 RID: 2208
	public class TooltipDisorderOfQi : MouseTipItem
	{
		// Token: 0x17000C8C RID: 3212
		// (get) Token: 0x060069A6 RID: 27046 RVA: 0x0030963A File Offset: 0x0030783A
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x00309640 File Offset: 0x00307840
		protected override void Init(ArgumentBox argsBox)
		{
			base.Init(argsBox);
			this.Element.ForceListenCommand = true;
			CharacterInjuryDisplayData injuryDisplayData;
			argsBox.Get<CharacterInjuryDisplayData>("CharacterInjuryDisplayData", out injuryDisplayData);
			argsBox.Get("CombatStyle", out this._combatStyle);
			sbyte level = DisorderLevelOfQi.GetDisorderLevelOfQi(injuryDisplayData.DisorderOfQi);
			QiDisorderEffectItem config = QiDisorderEffect.Instance[level];
			this.currentQiDesc.SetText(config.Name, true);
			this.changeOfQiDisorder.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.ChangeOfQiDisorder)).SetColor((injuryDisplayData.ChangeOfQiDisorder > 0) ? "brightred" : "brightblue").ColorReplace();
			this.qiRecoveryFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.RecoveryOfQiDisorderChangeQiDisorderValue)).SetColor((injuryDisplayData.RecoveryOfQiDisorderChangeQiDisorderValue > 0) ? "brightred" : "brightblue").ColorReplace();
			this.buildingFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.BuildingChangeQiDisorderValue)).SetColor((injuryDisplayData.BuildingChangeQiDisorderValue > 0) ? "brightred" : "brightblue").ColorReplace();
			this.eatItemFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.EatItemChangeQiDisorderValue)).SetColor((injuryDisplayData.EatItemChangeQiDisorderValue > 0) ? "brightred" : "brightblue").ColorReplace();
			this.featureFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.FeatureChangeQiDisorderValue)).SetColor((injuryDisplayData.FeatureChangeQiDisorderValue > 0) ? "brightred" : "brightblue").ColorReplace();
			this.qiRecoveryFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.RecoveryOfQiDisorderChangeQiDisorderValue != 0);
			this.buildingFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.BuildingChangeQiDisorderValue != 0);
			this.eatItemFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.EatItemChangeQiDisorderValue != 0);
			this.featureFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.FeatureChangeQiDisorderValue != 0);
			this.healthRecovery.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)config.HealthRecovery)).SetColor((config.HealthRecovery > 0) ? "brightblue" : "brightred").ColorReplace();
			this.injuryRete.text = LanguageKey.LK_Qi_Disorder_State_Content10.TrFormat(config.InjuredRate).SetColor((config.InjuredRate > 0) ? "brightred" : "brightblue").ColorReplace();
			this.poisonResistChange.text = LanguageKey.LK_Qi_Disorder_State_Content10.TrFormat(config.PoisonResistChange).SetColor((config.PoisonResistChange >= 0) ? "brightblue" : "brightred").ColorReplace();
			this.neiliCostInCombat.text = LanguageKey.LK_Qi_Disorder_State_Content10.TrFormat(config.NeiliCostInCombat).SetColor((config.NeiliCostInCombat > 0) ? "brightred" : "brightblue").ColorReplace();
			this.healthRecovery.transform.parent.parent.gameObject.SetActive(config.HealthRecovery != 0);
			this.injuryRete.transform.parent.parent.gameObject.SetActive(config.InjuredRate != 0);
			this.poisonResistChange.transform.parent.parent.gameObject.SetActive(config.PoisonResistChange != 0);
			this.neiliCostInCombat.transform.parent.parent.gameObject.SetActive(config.NeiliCostInCombat != 0);
			bool flag = this._combatStyle;
			if (flag)
			{
				short threshold = DefeatMarkCollection.CalcQiDisorderMarkThreshold((int)injuryDisplayData.DisorderOfQi);
				string thresholdStr = (injuryDisplayData.DisorderOfQi == DisorderLevelOfQi.MaxValue) ? "-" : ((int)(threshold / 10)).ToString();
				this.combatStyle.text = ((int)(injuryDisplayData.DisorderOfQi % threshold / 10)).ToString() + "/" + thresholdStr;
			}
			this.combatStyle.transform.parent.parent.gameObject.SetActive(this._combatStyle);
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x00309AC8 File Offset: 0x00307CC8
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.InnerBreath);
			}
		}

		// Token: 0x04004BDE RID: 19422
		[SerializeField]
		private TextMeshProUGUI currentQiDesc;

		// Token: 0x04004BDF RID: 19423
		[SerializeField]
		private TextMeshProUGUI changeOfQiDisorder;

		// Token: 0x04004BE0 RID: 19424
		[SerializeField]
		private TextMeshProUGUI qiRecoveryFactor;

		// Token: 0x04004BE1 RID: 19425
		[SerializeField]
		private TextMeshProUGUI buildingFactor;

		// Token: 0x04004BE2 RID: 19426
		[SerializeField]
		private TextMeshProUGUI eatItemFactor;

		// Token: 0x04004BE3 RID: 19427
		[SerializeField]
		private TextMeshProUGUI featureFactor;

		// Token: 0x04004BE4 RID: 19428
		[SerializeField]
		private TextMeshProUGUI healthRecovery;

		// Token: 0x04004BE5 RID: 19429
		[SerializeField]
		private TextMeshProUGUI injuryRete;

		// Token: 0x04004BE6 RID: 19430
		[SerializeField]
		private TextMeshProUGUI poisonResistChange;

		// Token: 0x04004BE7 RID: 19431
		[SerializeField]
		private TextMeshProUGUI neiliCostInCombat;

		// Token: 0x04004BE8 RID: 19432
		[SerializeField]
		private TextMeshProUGUI combatStyle;

		// Token: 0x04004BE9 RID: 19433
		private bool _combatStyle;
	}
}
