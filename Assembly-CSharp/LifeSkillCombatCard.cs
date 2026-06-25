using System;
using Config;

// Token: 0x0200023B RID: 571
public class LifeSkillCombatCard
{
	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x0600251C RID: 9500 RVA: 0x001110E8 File Offset: 0x0010F2E8
	public DebateStrategyItem Config
	{
		get
		{
			return DebateStrategy.Instance[(short)this.EffectCardId];
		}
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x001110FA File Offset: 0x0010F2FA
	public LifeSkillCombatCard(sbyte effectCardId, int count = 1)
	{
		this.EffectCardId = effectCardId;
		this.Count = count;
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x00111112 File Offset: 0x0010F312
	public LifeSkillCombatCard Clone()
	{
		return new LifeSkillCombatCard(this.EffectCardId, 1);
	}

	// Token: 0x04001BAD RID: 7085
	public readonly sbyte EffectCardId;

	// Token: 0x04001BAE RID: 7086
	public int Count;

	// Token: 0x02001541 RID: 5441
	public enum Level
	{
		// Token: 0x0400A3F7 RID: 41975
		Primary,
		// Token: 0x0400A3F8 RID: 41976
		Middle,
		// Token: 0x0400A3F9 RID: 41977
		High,
		// Token: 0x0400A3FA RID: 41978
		Count
	}
}
