using System;
using GameData.Domains.Character;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200011E RID: 286
public class CombatSubProcessorTaiwu : CombatSubProcessor, ICombatNotifySubProcessor
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000A86 RID: 2694 RVA: 0x00043F16 File Offset: 0x00042116
	ulong ICombatNotifySubProcessor.SubId0
	{
		get
		{
			return (ulong)((long)this._taiwuCharId);
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00043F1F File Offset: 0x0004211F
	public CombatSubProcessorTaiwu()
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		base.Setup();
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x00043F40 File Offset: 0x00042140
	[CombatNotifyData(4, 0, 58U)]
	private void HandlerDataEatingItems(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.EatingItems);
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00043F50 File Offset: 0x00042150
	[CombatNotifyData(4, 0, 43U)]
	private void HandlerDataCurrMainAttributes(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.CurrMainAttributes);
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00043F60 File Offset: 0x00042160
	[CombatNotifyData(4, 0, 64U)]
	private void HandlerDataXiangshuInfection(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.XiangshuInfection);
	}

	// Token: 0x04000D71 RID: 3441
	private readonly int _taiwuCharId;

	// Token: 0x04000D72 RID: 3442
	private const ushort Objects = 0;

	// Token: 0x04000D73 RID: 3443
	public EatingItems EatingItems;

	// Token: 0x04000D74 RID: 3444
	public MainAttributes CurrMainAttributes;

	// Token: 0x04000D75 RID: 3445
	public byte XiangshuInfection;
}
