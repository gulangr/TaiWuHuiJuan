using System;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200011C RID: 284
public class CombatSubProcessorPlayground : CombatSubProcessor
{
	// Token: 0x06000A79 RID: 2681 RVA: 0x00043D28 File Offset: 0x00041F28
	[CombatNotifyData(8, 38)]
	private void HandlerDataDisableEnemyAi(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.DisableEnemyAi);
		CombatSubProcessor.Model.RaiseEvent(ECombatEvents.OnDisableEnemyAiChanged);
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00043D46 File Offset: 0x00041F46
	[CombatNotifyData(8, 37)]
	private void HandlerDataEnemyUnyieldingFallen(RawDataPool pool, int offset)
	{
		Serializer.Deserialize(pool, offset, ref this.EnemyUnyieldingFallen);
		CombatSubProcessor.Model.RaiseEvent(ECombatEvents.OnEnemyUnyieldingFallenChanged);
	}

	// Token: 0x04000D65 RID: 3429
	public bool DisableEnemyAi;

	// Token: 0x04000D66 RID: 3430
	public bool EnemyUnyieldingFallen;
}
