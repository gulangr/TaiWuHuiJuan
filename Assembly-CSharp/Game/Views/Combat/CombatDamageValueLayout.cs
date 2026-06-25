using System;
using GameData.Domains.Combat;
using TMPro;

namespace Game.Views.Combat
{
	// Token: 0x02000B27 RID: 2855
	public class CombatDamageValueLayout : Refers
	{
		// Token: 0x06008C15 RID: 35861 RVA: 0x0040BBF0 File Offset: 0x00409DF0
		public void Set(CombatDamageValueLayoutData data)
		{
			bool empty = data == null;
			base.gameObject.SetActive(!empty);
			bool flag = empty;
			if (!flag)
			{
				base.CGet<CImage>("MarkIcon").SetSprite(CombatConstants.ParseMarkIcon(data.MarkKey), false, null);
				TextMeshProUGUI markCount = base.CGet<TextMeshProUGUI>("MarkCount");
				markCount.text = LocalStringManager.GetFormat(LanguageKey.LK_Combat_DamageValue_MarkCount, data.MarkCount.ToString(), CombatConstants.ParseDamageMarkName(data.MarkKey)).ColorReplace();
				int step = data.DamageValue.Second;
				TextMeshProUGUI damageValue = base.CGet<TextMeshProUGUI>("DamageValue");
				damageValue.text = (data.ReachLimit ? LocalStringManager.GetFormat(LanguageKey.LK_Combat_Injury_Fatal, (data.MarkKey.Type == EMarkType.Inner) ? LocalStringManager.Get(LanguageKey.LK_Inner_Injury) : LocalStringManager.Get(LanguageKey.LK_Out_Injury)) : LocalStringManager.GetFormat(LanguageKey.LK_Combat_DamageValue_Progress, data.DamageValue.First.ToString(), (step <= 0) ? "-" : step.ToString(), CombatConstants.ParseDamageValueName(data.MarkKey))).ColorReplace();
				damageValue.GetComponent<TMPTextSpriteHelper>().Parse();
			}
		}
	}
}
