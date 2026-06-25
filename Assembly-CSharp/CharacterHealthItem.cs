using System;
using CharacterDataMonitor;
using TMPro;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class CharacterHealthItem : Refers
{
	// Token: 0x06002E96 RID: 11926 RVA: 0x0016FFCE File Offset: 0x0016E1CE
	public void Setup()
	{
		this.InitRefers();
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x0016FFD8 File Offset: 0x0016E1D8
	public void Refresh(int characterId)
	{
		bool flag = this._ageHealthMonitor != null;
		if (flag)
		{
			this._ageHealthMonitor.RemoveOnHealthChangeEventListener(new Action(this.OnHealthChanged));
		}
		this._ageHealthMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(characterId, false);
		this._ageHealthMonitor.AddOnHealthChangeEventListener(new Action(this.OnHealthChanged));
		this._ageHealthMonitor.Refresh();
		bool init = this._ageHealthMonitor.Init;
		if (init)
		{
			this._ageHealthMonitor.OnDataInit();
		}
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x0017005E File Offset: 0x0016E25E
	private void OnHealthChanged()
	{
		this.RefreshRecovery();
		this.RefreshHealth();
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x00170070 File Offset: 0x0016E270
	private void OnDisable()
	{
		bool flag = this._ageHealthMonitor != null;
		if (flag)
		{
			this._ageHealthMonitor.RemoveOnHealthChangeEventListener(new Action(this.OnHealthChanged));
			this._ageHealthMonitor = null;
		}
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x001700AC File Offset: 0x0016E2AC
	private void RefreshHealth()
	{
		ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(this._ageHealthMonitor.Health, this._ageHealthMonitor.LeftMaxHealth, this._ageHealthMonitor.CharacterId);
		this._progress.fillAmount = info.Item2;
		this._stateDesc.text = info.Item1;
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x00170108 File Offset: 0x0016E308
	private void RefreshRecovery()
	{
		short recovery = this._ageHealthMonitor.HealthRecovery;
		Debug.Log(recovery);
		this._recovery.SetSprite((recovery >= 0) ? "specialfollow_icon_health_0" : "specialfollow_icon_health_1", false, null);
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x0017014C File Offset: 0x0016E34C
	private void InitRefers()
	{
		this._recovery = base.CGet<CImage>("Recovery");
		this._stateDesc = base.CGet<TextMeshProUGUI>("StateDesc");
		this._progress = base.CGet<CImage>("Progress");
	}

	// Token: 0x040021D2 RID: 8658
	private AgeHealthMonitor _ageHealthMonitor;

	// Token: 0x040021D3 RID: 8659
	private CImage _recovery;

	// Token: 0x040021D4 RID: 8660
	private TextMeshProUGUI _stateDesc;

	// Token: 0x040021D5 RID: 8661
	private CImage _progress;
}
