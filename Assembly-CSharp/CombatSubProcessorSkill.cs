using System;
using System.Collections.Generic;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200011D RID: 285
public class CombatSubProcessorSkill : CombatSubProcessor, ICombatNotifySubProcessor
{
	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000A7C RID: 2684 RVA: 0x00043D6D File Offset: 0x00041F6D
	public CombatSkillKey Key { get; }

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00043D75 File Offset: 0x00041F75
	ulong ICombatNotifySubProcessor.SubId0
	{
		get
		{
			return (ulong)this.Key;
		}
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00043D82 File Offset: 0x00041F82
	public CombatSubProcessorSkill(CombatSkillKey key)
	{
		this.Key = key;
		base.Setup();
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00043D9A File Offset: 0x00041F9A
	[CombatNotifyData(7, 0, 2U)]
	private void HandlerDataActivationState(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.ActivationState);
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00043DAB File Offset: 0x00041FAB
	[CombatNotifyData(7, 0, 12U)]
	private void HandlerDataDirection(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Direction);
		OnCombatSkillDataChangedEvent onCombatSkillDirectionChanged = CombatSubProcessor.Model.OnCombatSkillDirectionChanged;
		if (onCombatSkillDirectionChanged != null)
		{
			onCombatSkillDirectionChanged(this.Key);
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x00043DD8 File Offset: 0x00041FD8
	[CombatNotifyData(8, 29, 1U)]
	private void HandlerDataCanUse(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CanUse);
		OnCombatSkillDataChangedEvent onCombatSkillCanUseChanged = CombatSubProcessor.Model.OnCombatSkillCanUseChanged;
		if (onCombatSkillCanUseChanged != null)
		{
			onCombatSkillCanUseChanged(this.Key);
		}
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x00043E05 File Offset: 0x00042005
	[CombatNotifyData(8, 29, 2U)]
	private void HandlerDataLeftCdFrame(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.LeftCdFrame);
		OnCombatSkillDataChangedEvent onCombatSkillLeftCdFrameChanged = CombatSubProcessor.Model.OnCombatSkillLeftCdFrameChanged;
		if (onCombatSkillLeftCdFrameChanged != null)
		{
			onCombatSkillLeftCdFrameChanged(this.Key);
		}
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00043E32 File Offset: 0x00042032
	[CombatNotifyData(8, 29, 3U)]
	private void HandlerDataTotalCdFrame(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.TotalCdFrame);
		OnCombatSkillDataChangedEvent onCombatSkillTotalCdFrameChanged = CombatSubProcessor.Model.OnCombatSkillTotalCdFrameChanged;
		if (onCombatSkillTotalCdFrameChanged != null)
		{
			onCombatSkillTotalCdFrameChanged(this.Key);
		}
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00043E60 File Offset: 0x00042060
	[CombatNotifyData(8, 29, 7U)]
	private void HandlerDataBanReason(RawDataPool pool, int offset)
	{
		List<CombatSkillBanReasonData> deserializeBanReason = this.BanReason as List<CombatSkillBanReasonData>;
		Serializer.Deserialize(pool, offset, ref deserializeBanReason);
		this.BanReason = deserializeBanReason;
		if (this.BanReason == null)
		{
			this.BanReason = Array.Empty<CombatSkillBanReasonData>();
		}
		OnCombatSkillDataChangedEvent onCombatSkillBanReasonChanged = CombatSubProcessor.Model.OnCombatSkillBanReasonChanged;
		if (onCombatSkillBanReasonChanged != null)
		{
			onCombatSkillBanReasonChanged(this.Key);
		}
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00043EBC File Offset: 0x000420BC
	[CombatNotifyData(8, 29, 8U)]
	private void HandlerDataEffectData(RawDataPool pool, int offset)
	{
		List<CombatSkillEffectData> deserializeEffectData = this.EffectData as List<CombatSkillEffectData>;
		Serializer.Deserialize(pool, offset, ref deserializeEffectData);
		this.EffectData = deserializeEffectData;
		if (this.EffectData == null)
		{
			this.EffectData = Array.Empty<CombatSkillEffectData>();
		}
		OnCombatSkillDataChangedEvent onCombatSkillEffectDataChanged = CombatSubProcessor.Model.OnCombatSkillEffectDataChanged;
		if (onCombatSkillEffectDataChanged != null)
		{
			onCombatSkillEffectDataChanged(this.Key);
		}
	}

	// Token: 0x04000D68 RID: 3432
	private const ushort CombatSkills = 0;

	// Token: 0x04000D69 RID: 3433
	private const ushort SkillDataDict = 29;

	// Token: 0x04000D6A RID: 3434
	public ushort ActivationState;

	// Token: 0x04000D6B RID: 3435
	public sbyte Direction;

	// Token: 0x04000D6C RID: 3436
	public bool CanUse;

	// Token: 0x04000D6D RID: 3437
	public short LeftCdFrame;

	// Token: 0x04000D6E RID: 3438
	public short TotalCdFrame;

	// Token: 0x04000D6F RID: 3439
	public IReadOnlyList<CombatSkillBanReasonData> BanReason;

	// Token: 0x04000D70 RID: 3440
	public IReadOnlyList<CombatSkillEffectData> EffectData;
}
