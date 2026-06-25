using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using TMPro;

// Token: 0x02000312 RID: 786
public class UpgradeTeammateCommandEffect1 : UpgradeTeammateCommandEffect
{
	// Token: 0x06002E5C RID: 11868 RVA: 0x0016E438 File Offset: 0x0016C638
	protected override void InitRefers()
	{
		base.InitRefers();
		this._effectNameList = base.CGetList<TextMeshProUGUI>("EffectName_");
		this._effectValueList = base.CGetList<TextMeshProUGUI>("EffectValue_");
		this._iconList = base.CGetList<CImage>("Icon_");
		this._upParticleLoopList = base.CGetList<UIParticle>("UpParticleLoop_");
		this._downParticleLoopList = base.CGetList<UIParticle>("DownParticleLoop_");
		this._upParticleOnceList = base.CGetList<UIParticle>("UpParticleOnce_");
		this._downParticleOnceList = base.CGetList<UIParticle>("DownParticleOnce_");
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x0016E4C4 File Offset: 0x0016C6C4
	protected override void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._iconList.Count; i++)
		{
			this._iconList[i].SetSprite(UpgradeTeammateCommandEffectHelper.GetIcon(config, i), false, null);
			this._effectNameList[i].text = config.EffectDisplayTextList[i];
			this._effectValueList[i].text = config.EffectDisplayValueList[i].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor(config, normalConfig, i));
			UpgradeTeammateCommandEffectHelper.RefreshValueLoopParticle(config, normalConfig, i, this._upParticleLoopList[i], this._downParticleLoopList[i]);
		}
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x0016E574 File Offset: 0x0016C774
	public override void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			UpgradeTeammateCommandEffectHelper.PlayUpgradeParticle(advanceConfig, normalConfig, i, this._upParticleOnceList[i], this._downParticleOnceList[i]);
		}
	}

	// Token: 0x04002192 RID: 8594
	private List<TextMeshProUGUI> _effectNameList;

	// Token: 0x04002193 RID: 8595
	private List<TextMeshProUGUI> _effectValueList;

	// Token: 0x04002194 RID: 8596
	private List<CImage> _iconList;

	// Token: 0x04002195 RID: 8597
	private List<UIParticle> _upParticleLoopList;

	// Token: 0x04002196 RID: 8598
	private List<UIParticle> _downParticleLoopList;

	// Token: 0x04002197 RID: 8599
	private List<UIParticle> _upParticleOnceList;

	// Token: 0x04002198 RID: 8600
	private List<UIParticle> _downParticleOnceList;
}
