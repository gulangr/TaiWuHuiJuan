using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000296 RID: 662
public class MouseTipEquipLoad : MouseTipBase
{
	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x06002A00 RID: 10752 RVA: 0x0013EE67 File Offset: 0x0013D067
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x0013EE6C File Offset: 0x0013D06C
	protected override void Init(ArgumentBox argsBox)
	{
		base.CGet<TextMeshProUGUI>("Desc1").text = LocalStringManager.Get(LanguageKey.LK_Tip_EquipLoad_Desc1);
		base.CGet<TextMeshProUGUI>("Desc2").text = LocalStringManager.Get(LanguageKey.LK_Tip_EquipLoad_Desc2);
		base.CGet<TMPTextSpriteHelper>("Desc1SpriteHelper").Parse();
		int currentLoad;
		argsBox.Get("currentLoad", out currentLoad);
		int maxLoad;
		argsBox.Get("maxLoad", out maxLoad);
		this.RefreshLoadText(currentLoad, maxLoad);
		bool flag = currentLoad <= maxLoad || maxLoad <= 0;
		if (flag)
		{
			base.CGet<GameObject>("OverloadEffect").SetActive(false);
		}
		else
		{
			base.CGet<GameObject>("OverloadEffect").SetActive(true);
			this.RefreshOverloadEffect(currentLoad, maxLoad, "AttackSpeed", GlobalConfig.Instance.EquipLoadSpeedPercent, LanguageKey.LK_AttackSpeed);
			this.RefreshOverloadEffect(currentLoad, maxLoad, "MoveSpeed", GlobalConfig.Instance.EquipLoadSpeedPercent, LanguageKey.LK_RecoveryOfMobility);
			this.RefreshOverloadEffect(currentLoad, maxLoad, "CastSpeed", GlobalConfig.Instance.EquipLoadSpeedPercent, LanguageKey.LK_CastSpeed);
			this.RefreshOverloadEffect(currentLoad, maxLoad, "RecoveryOfFlaw", GlobalConfig.Instance.EquipHealSpeedPercent, LanguageKey.LK_RecoveryOfFlaw);
			this.RefreshOverloadEffect(currentLoad, maxLoad, "RecoveryOfBlockedAcupoint", GlobalConfig.Instance.EquipHealSpeedPercent, LanguageKey.LK_RecoveryOfBlockedAcupoint);
			this.RefreshOverloadEffect(currentLoad, maxLoad, "WeaponSwitchSpeed", GlobalConfig.Instance.EquipHealSpeedPercent, LanguageKey.LK_WeaponSwitchSpeed);
		}
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x0013EFD0 File Offset: 0x0013D1D0
	private void RefreshOverloadEffect(int currentLoad, int maxLoad, string refName, int[] effectConfigArray, LanguageKey effectNameKey)
	{
		int index = Math.Min((currentLoad - 1) / maxLoad - 1, effectConfigArray.Length - 1);
		int result = effectConfigArray[index] - 100;
		TextMeshProUGUI buffTextUi = base.CGet<TextMeshProUGUI>("Buff" + refName);
		TextMeshProUGUI debuffTextUi = base.CGet<TextMeshProUGUI>("Debuff" + refName);
		buffTextUi.text = ((result > 0) ? string.Format("+{0}%", result) : "");
		debuffTextUi.text = ((result > 0) ? "" : string.Format("{0}%", result));
		base.CGet<TextMeshProUGUI>("Tips" + refName).text = LocalStringManager.Get(effectNameKey);
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x0013F080 File Offset: 0x0013D280
	private void RefreshLoadText(int currentLoad, int maxLoad)
	{
		bool isOverload = currentLoad > maxLoad;
		string percentString = string.Format("{0}%", Math.Round((double)((float)currentLoad / (float)maxLoad * 100f)));
		base.CGet<TextMeshProUGUI>("NormalLoadText").text = (isOverload ? "" : percentString);
		base.CGet<TextMeshProUGUI>("OverloadLoadText").text = (isOverload ? percentString : "");
	}
}
