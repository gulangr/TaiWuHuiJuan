using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using TMPro;

// Token: 0x02000316 RID: 790
public class UpgradeTeammateCommandEffect5 : UpgradeTeammateCommandEffect
{
	// Token: 0x06002E6C RID: 11884 RVA: 0x0016E988 File Offset: 0x0016CB88
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

	// Token: 0x06002E6D RID: 11885 RVA: 0x0016EA14 File Offset: 0x0016CC14
	protected override void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			bool flag = i < this._iconList.Count;
			if (flag)
			{
				this._iconList[i].SetSprite(UpgradeTeammateCommandEffectHelper.GetIcon(config, i), false, null);
			}
			this._effectNameList[i].text = config.EffectDisplayTextList[i];
			this._effectValueList[i].text = config.EffectDisplayValueList[i].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor(config, normalConfig, i));
			UpgradeTeammateCommandEffectHelper.RefreshValueLoopParticle(config, normalConfig, i, this._upParticleLoopList[i], this._downParticleLoopList[i]);
		}
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x0016EAD4 File Offset: 0x0016CCD4
	public override void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
		for (int i = 0; i < this._effectNameList.Count; i++)
		{
			UpgradeTeammateCommandEffectHelper.PlayUpgradeParticle(advanceConfig, normalConfig, i, this._upParticleOnceList[i], this._downParticleOnceList[i]);
		}
	}

	// Token: 0x040021AC RID: 8620
	private List<TextMeshProUGUI> _effectNameList;

	// Token: 0x040021AD RID: 8621
	private List<TextMeshProUGUI> _effectValueList;

	// Token: 0x040021AE RID: 8622
	private List<CImage> _iconList;

	// Token: 0x040021AF RID: 8623
	private List<UIParticle> _upParticleLoopList;

	// Token: 0x040021B0 RID: 8624
	private List<UIParticle> _downParticleLoopList;

	// Token: 0x040021B1 RID: 8625
	private List<UIParticle> _upParticleOnceList;

	// Token: 0x040021B2 RID: 8626
	private List<UIParticle> _downParticleOnceList;
}
