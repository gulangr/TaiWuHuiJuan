using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using TMPro;

// Token: 0x02000315 RID: 789
public class UpgradeTeammateCommandEffect4 : UpgradeTeammateCommandEffect
{
	// Token: 0x06002E68 RID: 11880 RVA: 0x0016E81C File Offset: 0x0016CA1C
	protected override void InitRefers()
	{
		base.InitRefers();
		this._effectNameList = base.CGetList<TextMeshProUGUI>("EffectName_");
		this._effectValueList = base.CGetList<TextMeshProUGUI>("EffectValue_");
		this._upParticleOnceList = base.CGetList<UIParticle>("UpParticleOnce_");
		this._upParticleLoopList = base.CGetList<UIParticle>("UpParticleLoop_");
		this._downParticleOnceList = base.CGetList<UIParticle>("DownParticleOnce_");
		this._downParticleLoopList = base.CGetList<UIParticle>("DownParticleLoop_");
		this._tipDiplayer = base.CGet<TooltipInvoker>("TipDiplayer");
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x0016E8A8 File Offset: 0x0016CAA8
	protected override void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			this._effectNameList[i].text = config.EffectDisplayTextList[i];
			this._effectValueList[i].text = config.EffectDisplayValueList[i].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor(config, normalConfig, i));
			UpgradeTeammateCommandEffectHelper.RefreshValueLoopParticle(config, normalConfig, i, this._upParticleLoopList[i], this._downParticleLoopList[i]);
		}
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x0016E934 File Offset: 0x0016CB34
	public override void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			UpgradeTeammateCommandEffectHelper.PlayUpgradeParticle(advanceConfig, normalConfig, i, this._upParticleOnceList[i], this._downParticleOnceList[i]);
		}
	}

	// Token: 0x040021A5 RID: 8613
	private List<TextMeshProUGUI> _effectNameList;

	// Token: 0x040021A6 RID: 8614
	private List<TextMeshProUGUI> _effectValueList;

	// Token: 0x040021A7 RID: 8615
	private List<UIParticle> _upParticleOnceList;

	// Token: 0x040021A8 RID: 8616
	private List<UIParticle> _upParticleLoopList;

	// Token: 0x040021A9 RID: 8617
	private List<UIParticle> _downParticleOnceList;

	// Token: 0x040021AA RID: 8618
	private List<UIParticle> _downParticleLoopList;

	// Token: 0x040021AB RID: 8619
	private TooltipInvoker _tipDiplayer;
}
