using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using TMPro;

// Token: 0x02000317 RID: 791
public class UpgradeTeammateCommandEffect6 : UpgradeTeammateCommandEffect
{
	// Token: 0x06002E70 RID: 11888 RVA: 0x0016EB28 File Offset: 0x0016CD28
	protected override void InitRefers()
	{
		base.InitRefers();
		this._effectNameList = base.CGetList<TextMeshProUGUI>("EffectName_");
		this._effectValueList = base.CGetList<TextMeshProUGUI>("EffectValue_");
		this._downParticleLoopList = base.CGetList<UIParticle>("DownParticleLoop_");
		this._downParticleOnceList = base.CGetList<UIParticle>("DownParticleOnce_");
		this._upParticleLoopList = base.CGetList<UIParticle>("UpParticleLoop_");
		this._upParticleOnceList = base.CGetList<UIParticle>("UpParticleOnce_");
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x0016EBA4 File Offset: 0x0016CDA4
	protected override void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			this._effectNameList[i].text = config.EffectDisplayTextList[i];
			this._effectValueList[i].text = config.EffectDisplayValueList[i].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor(config, normalConfig, i));
			UpgradeTeammateCommandEffectHelper.RefreshValueLoopParticle(config, normalConfig, i, this._upParticleLoopList[i], this._downParticleLoopList[i]);
		}
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x0016EC30 File Offset: 0x0016CE30
	public override void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			UpgradeTeammateCommandEffectHelper.PlayUpgradeParticle(advanceConfig, normalConfig, i, this._upParticleOnceList[i], this._downParticleOnceList[i]);
		}
	}

	// Token: 0x040021B3 RID: 8627
	private List<TextMeshProUGUI> _effectNameList;

	// Token: 0x040021B4 RID: 8628
	private List<TextMeshProUGUI> _effectValueList;

	// Token: 0x040021B5 RID: 8629
	private List<UIParticle> _downParticleLoopList;

	// Token: 0x040021B6 RID: 8630
	private List<UIParticle> _downParticleOnceList;

	// Token: 0x040021B7 RID: 8631
	private List<UIParticle> _upParticleLoopList;

	// Token: 0x040021B8 RID: 8632
	private List<UIParticle> _upParticleOnceList;
}
