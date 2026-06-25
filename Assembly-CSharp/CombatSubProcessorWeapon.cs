using System;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200011F RID: 287
public class CombatSubProcessorWeapon : CombatSubProcessor, ICombatNotifySubProcessor
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000A8B RID: 2699 RVA: 0x00043F70 File Offset: 0x00042170
	public ItemKey Key { get; }

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000A8C RID: 2700 RVA: 0x00043F78 File Offset: 0x00042178
	ulong ICombatNotifySubProcessor.SubId0
	{
		get
		{
			return (ulong)((long)this.Key.Id);
		}
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00043F86 File Offset: 0x00042186
	public CombatSubProcessorWeapon(ItemKey key)
	{
		this.Key = key;
		base.Setup();
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00043F9E File Offset: 0x0004219E
	[CombatNotifyData(6, 0, 12U)]
	private void HandlerDataWeight(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Weight);
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00043FAF File Offset: 0x000421AF
	[CombatNotifyData(6, 0, 5U)]
	private void HandlerDataDurability(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.Durability);
		OnWeaponDataChangedEvent onWeaponDurabilityChanged = CombatSubProcessor.Model.OnWeaponDurabilityChanged;
		if (onWeaponDurabilityChanged != null)
		{
			onWeaponDurabilityChanged(this.Key);
		}
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00043FDC File Offset: 0x000421DC
	[CombatNotifyData(6, 0, 2U)]
	private void HandlerDataMaxDurability(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.MaxDurability);
		OnWeaponDataChangedEvent onWeaponDurabilityChanged = CombatSubProcessor.Model.OnWeaponDurabilityChanged;
		if (onWeaponDurabilityChanged != null)
		{
			onWeaponDurabilityChanged(this.Key);
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x00044009 File Offset: 0x00042209
	[CombatNotifyData(8, 30, 2U)]
	private void HandlerDataCanChangeTo(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CanChangeTo);
		OnWeaponDataChangedEvent onWeaponCanChangeToChanged = CombatSubProcessor.Model.OnWeaponCanChangeToChanged;
		if (onWeaponCanChangeToChanged != null)
		{
			onWeaponCanChangeToChanged(this.Key);
		}
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00044036 File Offset: 0x00042236
	[CombatNotifyData(8, 30, 4U)]
	private void HandlerDataCdFrame(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CdFrame);
		OnWeaponDataChangedEvent onWeaponCdFrameChanged = CombatSubProcessor.Model.OnWeaponCdFrameChanged;
		if (onWeaponCdFrameChanged != null)
		{
			onWeaponCdFrameChanged(this.Key);
		}
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00044063 File Offset: 0x00042263
	[CombatNotifyData(8, 30, 7U)]
	private void HandlerDataFixedCdLeftFrame(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.FixedCdLeftFrame);
		OnWeaponDataChangedEvent onWeaponFixedCdLeftFrameChanged = CombatSubProcessor.Model.OnWeaponFixedCdLeftFrameChanged;
		if (onWeaponFixedCdLeftFrameChanged != null)
		{
			onWeaponFixedCdLeftFrameChanged(this.Key);
		}
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00044090 File Offset: 0x00042290
	[CombatNotifyData(8, 30, 8U)]
	private void HandlerDataFixedCdTotalFrame(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.FixedCdTotalFrame);
		OnWeaponDataChangedEvent onWeaponFixedCdTotalFrameChanged = CombatSubProcessor.Model.OnWeaponFixedCdTotalFrameChanged;
		if (onWeaponFixedCdTotalFrameChanged != null)
		{
			onWeaponFixedCdTotalFrameChanged(this.Key);
		}
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000440BD File Offset: 0x000422BD
	[CombatNotifyData(8, 30, 9U)]
	private void HandlerDataInnerRatio(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.InnerRatio);
		OnWeaponDataChangedEvent onWeaponInnerRatioChanged = CombatSubProcessor.Model.OnWeaponInnerRatioChanged;
		if (onWeaponInnerRatioChanged != null)
		{
			onWeaponInnerRatioChanged(this.Key);
		}
	}

	// Token: 0x04000D77 RID: 3447
	private const ushort Weapons = 0;

	// Token: 0x04000D78 RID: 3448
	private const ushort WeaponDataDict = 30;

	// Token: 0x04000D79 RID: 3449
	public int Weight;

	// Token: 0x04000D7A RID: 3450
	public short Durability;

	// Token: 0x04000D7B RID: 3451
	public short MaxDurability;

	// Token: 0x04000D7C RID: 3452
	public bool CanChangeTo;

	// Token: 0x04000D7D RID: 3453
	public short CdFrame;

	// Token: 0x04000D7E RID: 3454
	public short FixedCdLeftFrame;

	// Token: 0x04000D7F RID: 3455
	public short FixedCdTotalFrame;

	// Token: 0x04000D80 RID: 3456
	public sbyte InnerRatio;
}
