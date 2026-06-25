using System;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A2 RID: 2210
	public class TooltipHealth : MouseTipItem
	{
		// Token: 0x17000C8E RID: 3214
		// (get) Token: 0x060069B4 RID: 27060 RVA: 0x0030A1C2 File Offset: 0x003083C2
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069B5 RID: 27061 RVA: 0x0030A1C8 File Offset: 0x003083C8
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			base.Init(argsBox);
			CharacterInjuryDisplayData injuryDisplayData;
			argsBox.Get<CharacterInjuryDisplayData>("CharacterInjuryDisplayData", out injuryDisplayData);
			ValueTuple<string, float, int> healthInfo = CommonUtils.GetCharacterHealthInfo(injuryDisplayData.Health, injuryDisplayData.LeftMaxHealth, injuryDisplayData.CharacterId);
			string healthLevel = healthInfo.Item1;
			this.currentHealthDesc.SetText(healthLevel, true);
			this.healthRecovery.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.HealthRecovery)).SetColor((injuryDisplayData.HealthRecovery > 0) ? "brightblue" : "brightred").ColorReplace();
			this.qiFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.DisorderOfQiChangeHealthValue)).SetColor((injuryDisplayData.DisorderOfQiChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.injuryFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.InjuryChangeHealthValue)).SetColor((injuryDisplayData.InjuryChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.poisonFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.PoisonChangeHealthValue)).SetColor((injuryDisplayData.PoisonChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.buildingFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.BuildingChangeHealthValue)).SetColor((injuryDisplayData.BuildingChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.eatItemFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.EatItemChangeHealthValue)).SetColor((injuryDisplayData.EatItemChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.featureFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.FeatureChangeHealthValue)).SetColor((injuryDisplayData.FeatureChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.specialEffectFactor.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content13.TrFormat(TooltipHealth.AddFormat((int)injuryDisplayData.SpecialEffectChangeHealthValue)).SetColor((injuryDisplayData.SpecialEffectChangeHealthValue > 0) ? "brightblue" : "brightred").ColorReplace();
			this.qiFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.DisorderOfQiChangeHealthValue != 0);
			this.injuryFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.InjuryChangeHealthValue != 0);
			this.poisonFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.PoisonChangeHealthValue != 0);
			this.buildingFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.BuildingChangeHealthValue != 0);
			this.eatItemFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.EatItemChangeHealthValue != 0);
			this.featureFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.FeatureChangeHealthValue != 0);
			this.specialEffectFactor.transform.parent.parent.gameObject.SetActive(injuryDisplayData.SpecialEffectChangeHealthValue != 0);
			this.combatMark.text = LanguageKey.LK_CharacterMenu_Injury_Health_Tip_Content4.TrFormat(injuryDisplayData.HealthCombatMark).SetColor((injuryDisplayData.HealthCombatMark > 0) ? "brightred" : "brightyellow").ColorReplace();
		}

		// Token: 0x060069B6 RID: 27062 RVA: 0x0030A584 File Offset: 0x00308784
		private void Update()
		{
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Health);
			}
		}

		// Token: 0x060069B7 RID: 27063 RVA: 0x0030A5B8 File Offset: 0x003087B8
		public static string AddFormat(int value)
		{
			bool flag = value > 0;
			string result;
			if (flag)
			{
				result = "+" + value.ToString();
			}
			else
			{
				result = value.ToString();
			}
			return result;
		}

		// Token: 0x04004BEF RID: 19439
		[SerializeField]
		private TextMeshProUGUI currentHealthDesc;

		// Token: 0x04004BF0 RID: 19440
		[SerializeField]
		private TextMeshProUGUI healthRecovery;

		// Token: 0x04004BF1 RID: 19441
		[SerializeField]
		private TextMeshProUGUI qiFactor;

		// Token: 0x04004BF2 RID: 19442
		[SerializeField]
		private TextMeshProUGUI injuryFactor;

		// Token: 0x04004BF3 RID: 19443
		[SerializeField]
		private TextMeshProUGUI poisonFactor;

		// Token: 0x04004BF4 RID: 19444
		[SerializeField]
		private TextMeshProUGUI buildingFactor;

		// Token: 0x04004BF5 RID: 19445
		[SerializeField]
		private TextMeshProUGUI eatItemFactor;

		// Token: 0x04004BF6 RID: 19446
		[SerializeField]
		private TextMeshProUGUI featureFactor;

		// Token: 0x04004BF7 RID: 19447
		[SerializeField]
		private TextMeshProUGUI specialEffectFactor;

		// Token: 0x04004BF8 RID: 19448
		[SerializeField]
		private TextMeshProUGUI combatMark;
	}
}
