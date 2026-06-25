using System;
using System.Collections.Generic;
using GameData.Domains.Map;
using UnityEngine;

// Token: 0x020003F2 RID: 1010
public class MapElementPickupEffect : MapElementBase
{
	// Token: 0x06003CBF RID: 15551 RVA: 0x001E9464 File Offset: 0x001E7664
	public static bool CheckMaybeExist(Location location)
	{
		bool flag = MapElementBase.MapModel.ViewMode == WorldMapModel.EViewMode.Info && MapElementBase.MapModel.CurrentAreaId == location.AreaId;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = MapElementBase.MapModel.ShowingAreaId != location.AreaId;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = MapElementBase.MapModel.CurrentAreaId != location.AreaId;
				result = (!flag3 && MapElementBase.MapModel.MapPickupEffectQueueDict.ContainsKey(location));
			}
		}
		return result;
	}

	// Token: 0x1700062D RID: 1581
	// (get) Token: 0x06003CC0 RID: 15552 RVA: 0x001E94EA File Offset: 0x001E76EA
	public override EMapLayer Layer
	{
		get
		{
			return EMapLayer.Cricket;
		}
	}

	// Token: 0x06003CC1 RID: 15553 RVA: 0x001E94ED File Offset: 0x001E76ED
	public override void Scale(float wheel)
	{
		base.ScaleReverse(wheel);
	}

	// Token: 0x06003CC2 RID: 15554 RVA: 0x001E94F8 File Offset: 0x001E76F8
	protected override void OnCreate()
	{
	}

	// Token: 0x06003CC3 RID: 15555 RVA: 0x001E94FC File Offset: 0x001E76FC
	protected override void OnRefresh()
	{
		Queue<MapElementPickupDisplayData> queue = MapElementBase.MapModel.MapPickupEffectQueueDict[base.BlockLocation];
		bool flag = queue.Count == 0;
		if (!flag)
		{
			MapElementPickupDisplayData displayData = queue.Dequeue();
			bool flag2 = displayData != null;
			if (flag2)
			{
				this.StartOneEffect(displayData);
			}
		}
	}

	// Token: 0x06003CC4 RID: 15556 RVA: 0x001E9546 File Offset: 0x001E7746
	protected override void OnCollect()
	{
	}

	// Token: 0x06003CC5 RID: 15557 RVA: 0x001E954C File Offset: 0x001E774C
	private void Update()
	{
		bool flag = this._pendingEffects.Count == 0;
		if (!flag)
		{
			float deltaTime = Time.deltaTime;
			MapElementPickupEffect.PendingEffect first = this._pendingEffects.Peek();
			bool flag2 = this._playingEffects.Count == 0;
			if (flag2)
			{
				first.ElapsedTime = 0.8f;
			}
			first.ElapsedTime += deltaTime;
			bool flag3 = first.ElapsedTime >= 0.8f;
			if (flag3)
			{
				this._pendingEffects.Dequeue();
				this.StartOneEffectInternal(first.DisplayData);
			}
		}
	}

	// Token: 0x06003CC6 RID: 15558 RVA: 0x001E95DC File Offset: 0x001E77DC
	private void StartOneEffect(MapElementPickupDisplayData displayData)
	{
		this._pendingEffects.Enqueue(new MapElementPickupEffect.PendingEffect
		{
			DisplayData = displayData,
			ElapsedTime = 0f
		});
	}

	// Token: 0x06003CC7 RID: 15559 RVA: 0x001E9604 File Offset: 0x001E7804
	private void StartOneEffectInternal(MapElementPickupDisplayData displayData)
	{
		MapPickupEffectItem effect = Object.Instantiate<MapPickupEffectItem>(this.prefab, this.effectRoot);
		this._playingEffects.Add(effect);
		effect.Refresh(displayData, this);
	}

	// Token: 0x06003CC8 RID: 15560 RVA: 0x001E963C File Offset: 0x001E783C
	public void DestroyEffect(MapPickupEffectItem effectItem)
	{
		this._playingEffects.Remove(effectItem);
		Object.Destroy(effectItem.gameObject);
		bool flag = this._pendingEffects.Count == 0 && this._playingEffects.Count == 0 && MapElementBase.MapModel.MapPickupEffectQueueDict[base.BlockLocation].Count == 0;
		if (flag)
		{
			MapElementBase.MapModel.MapPickupEffectQueueDict.Remove(base.BlockLocation);
		}
	}

	// Token: 0x04002B94 RID: 11156
	private readonly HashSet<MapPickupEffectItem> _playingEffects = new HashSet<MapPickupEffectItem>();

	// Token: 0x04002B95 RID: 11157
	private const float EffectInterval = 0.8f;

	// Token: 0x04002B96 RID: 11158
	private readonly Queue<MapElementPickupEffect.PendingEffect> _pendingEffects = new Queue<MapElementPickupEffect.PendingEffect>();

	// Token: 0x04002B97 RID: 11159
	public RectTransform effectRoot;

	// Token: 0x04002B98 RID: 11160
	public MapPickupEffectItem prefab;

	// Token: 0x02001881 RID: 6273
	private class PendingEffect
	{
		// Token: 0x0400AEBF RID: 44735
		public MapElementPickupDisplayData DisplayData;

		// Token: 0x0400AEC0 RID: 44736
		public float ElapsedTime;
	}
}
