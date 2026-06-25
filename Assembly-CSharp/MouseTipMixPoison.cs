using System;
using Config;
using FrameWork;
using TMPro;

// Token: 0x020002C1 RID: 705
public class MouseTipMixPoison : MouseTipBase
{
	// Token: 0x06002AE5 RID: 10981 RVA: 0x0014A79C File Offset: 0x0014899C
	protected override void Init(ArgumentBox argsBox)
	{
		short id;
		argsBox.Get("MedicineId", out id);
		MedicineItem medicineConfig = Medicine.Instance[id];
		base.CGet<TextMeshProUGUI>("Name").text = medicineConfig.Name;
		Refers mixPoisonEffect = base.CGet<Refers>("MixPoisonEffect");
		mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectInCombatName").text = medicineConfig.Name + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
		mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectOutCombatName").text = medicineConfig.Name + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
		MouseTip_Util.UpdateMixPoisonEffectText(mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectInCombatDesc"), medicineConfig.SpecialEffectDesc);
		MouseTip_Util.UpdateMixPoisonEffectText(mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectOutCombatDesc"), medicineConfig.Desc);
	}
}
