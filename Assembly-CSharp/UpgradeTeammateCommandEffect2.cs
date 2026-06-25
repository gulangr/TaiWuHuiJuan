using System;
using Coffee.UIExtensions;
using Config;
using TMPro;

// Token: 0x02000313 RID: 787
public class UpgradeTeammateCommandEffect2 : UpgradeTeammateCommandEffect
{
	// Token: 0x06002E60 RID: 11872 RVA: 0x0016E5C8 File Offset: 0x0016C7C8
	protected override void InitRefers()
	{
		base.InitRefers();
		this._effectName = base.CGet<TextMeshProUGUI>("EffectName");
		this._effectValue = base.CGet<TextMeshProUGUI>("EffectValue");
		this._upParticleOnce = base.CGet<UIParticle>("UpParticleOnce");
		this._upParticleLoop = base.CGet<UIParticle>("UpParticleLoop");
		this._downParticleOnce = base.CGet<UIParticle>("DownParticleOnce");
		this._downParticleLoop = base.CGet<UIParticle>("DownParticleLoop");
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x0016E644 File Offset: 0x0016C844
	protected override void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		this._effectName.text = config.EffectDisplayTextList[0];
		this._effectValue.text = config.EffectDisplayValueList[0].SetColor(UpgradeTeammateCommandEffectHelper.GetValueColor(config, normalConfig, 0));
		UpgradeTeammateCommandEffectHelper.RefreshValueLoopParticle(config, normalConfig, 0, this._upParticleLoop, this._downParticleLoop);
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x0016E69C File Offset: 0x0016C89C
	public override void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
		UpgradeTeammateCommandEffectHelper.PlayUpgradeParticle(advanceConfig, normalConfig, 0, this._upParticleOnce, this._downParticleOnce);
	}

	// Token: 0x04002199 RID: 8601
	private TextMeshProUGUI _effectName;

	// Token: 0x0400219A RID: 8602
	private TextMeshProUGUI _effectValue;

	// Token: 0x0400219B RID: 8603
	private UIParticle _upParticleOnce;

	// Token: 0x0400219C RID: 8604
	private UIParticle _upParticleLoop;

	// Token: 0x0400219D RID: 8605
	private UIParticle _downParticleOnce;

	// Token: 0x0400219E RID: 8606
	private UIParticle _downParticleLoop;
}
