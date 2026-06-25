using System;
using Config;

// Token: 0x02000311 RID: 785
public class UpgradeTeammateCommandEffect : Refers
{
	// Token: 0x06002E56 RID: 11862 RVA: 0x0016E3E1 File Offset: 0x0016C5E1
	public void Init()
	{
		this.InitRefers();
		this._inited = true;
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x0016E3F4 File Offset: 0x0016C5F4
	public void Refresh(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
		bool inited = this._inited;
		if (inited)
		{
			this.Init();
		}
		this.RefreshImpl(config, normalConfig);
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x0016E41C File Offset: 0x0016C61C
	protected virtual void InitRefers()
	{
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x0016E41F File Offset: 0x0016C61F
	protected virtual void RefreshImpl(TeammateCommandItem config, TeammateCommandItem normalConfig)
	{
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x0016E422 File Offset: 0x0016C622
	public virtual void PlayUpgradeParticle(TeammateCommandItem advanceConfig, TeammateCommandItem normalConfig)
	{
	}

	// Token: 0x04002191 RID: 8593
	private bool _inited = false;
}
