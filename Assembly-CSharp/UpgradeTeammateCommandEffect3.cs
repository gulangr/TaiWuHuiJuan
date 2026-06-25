using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using TMPro;

// Token: 0x02000314 RID: 788
public class UpgradeTeammateCommandEffect3 : UpgradeTeammateCommandEffect
{
	// Token: 0x06002E64 RID: 11876 RVA: 0x0016E6C0 File Offset: 0x0016C8C0
	protected override void InitRefers()
	{
		base.InitRefers();
		this._effectNameList = base.CGetList<TextMeshProUGUI>("EffectName_");
		this._effectValueList = base.CGetList<TextMeshProUGUI>("EffectValue_");
		this._upParticleLoopList = base.CGetList<UIParticle>("UpParticleLoop_");
		this._downParticleLoopList = base.CGetList<UIParticle>("DownParticleLoop_");
		this._upParticleOnceList = base.CGetList<UIParticle>("UpParticleOnce_");
		this._downParticleOnceList = base.CGetList<UIParticle>("DownParticleOnce_");
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x0016E73C File Offset: 0x0016C93C
	protected override void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			this._effectNameList[i].text = config.EffectDisplayTextList[i];
			this._effectValueList[i].text = config.EffectDisplayValueList[i].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor(config, normalConfig, i));
			UpgradeTeammateCommandEffectHelper.RefreshValueLoopParticle(config, normalConfig, i, this._upParticleLoopList[i], this._downParticleLoopList[i]);
		}
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x0016E7C8 File Offset: 0x0016C9C8
	public override void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			UpgradeTeammateCommandEffectHelper.PlayUpgradeParticle(advanceConfig, normalConfig, i, this._upParticleOnceList[i], this._downParticleOnceList[i]);
		}
	}

	// Token: 0x0400219F RID: 8607
	private List<TextMeshProUGUI> _effectNameList;

	// Token: 0x040021A0 RID: 8608
	private List<TextMeshProUGUI> _effectValueList;

	// Token: 0x040021A1 RID: 8609
	private List<UIParticle> _upParticleLoopList;

	// Token: 0x040021A2 RID: 8610
	private List<UIParticle> _downParticleLoopList;

	// Token: 0x040021A3 RID: 8611
	private List<UIParticle> _upParticleOnceList;

	// Token: 0x040021A4 RID: 8612
	private List<UIParticle> _downParticleOnceList;
}
