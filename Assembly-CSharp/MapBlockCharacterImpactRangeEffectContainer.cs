using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Story.MainStory;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003C3 RID: 963
public class MapBlockCharacterImpactRangeEffectContainer : MapBlockEightDirectionEffectContainer
{
	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x06003A31 RID: 14897 RVA: 0x001D9E99 File Offset: 0x001D8099
	public bool IsShowing
	{
		get
		{
			return this._edgeBlocks.Count > 0;
		}
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x001D9EA9 File Offset: 0x001D80A9
	private void OnEnable()
	{
		this._dispatcher = DispatcherUtils.RegisterDispatcher();
		GEvent.Add(UiEvents.OnWorldMapCharacterImpactRangeChanged, new GEvent.Callback(this.OnCharacterImpactRangeChanged));
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x001D9ED3 File Offset: 0x001D80D3
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnWorldMapCharacterImpactRangeChanged, new GEvent.Callback(this.OnCharacterImpactRangeChanged));
		this.ClearCache();
		this.Hide();
		DispatcherUtils.UnregisterDispatcher(this._dispatcher);
		this._dispatcher = null;
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x001D9F14 File Offset: 0x001D8114
	public void UseSettlementEffectPrefabs(MapBlockEightDirectionEffectContainer settlementContainer)
	{
		bool flag = settlementContainer == null;
		if (!flag)
		{
			this.EffectPrefabs = settlementContainer.EffectPrefabs;
			this.SingleEffectPrefabs = settlementContainer.SingleEffectPrefabs;
		}
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x001D9F48 File Offset: 0x001D8148
	private void OnCharacterImpactRangeChanged(ArgumentBox arg)
	{
		int charId;
		arg.Get("charId", out charId);
		bool visible;
		arg.Get("visible", out visible);
		this.SetVisible(charId, visible);
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x001D9F7C File Offset: 0x001D817C
	public void SetVisible(int charId, bool visible)
	{
		bool flag = !visible;
		if (flag)
		{
			this._pendingCharId = -1;
			this._showingCharId = -1;
			this.Hide();
		}
		else
		{
			this._showingCharId = charId;
			this._pendingCharId = charId;
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this._dispatcher, charId, new AsyncMethodCallbackDelegate(this.OnCharacterDisplayDataReceived));
		}
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x001D9FD4 File Offset: 0x001D81D4
	private void OnCharacterDisplayDataReceived(int offset, RawDataPool pool)
	{
		CharacterDisplayData data = null;
		Serializer.Deserialize(pool, offset, ref data);
		bool flag = data == null || this._pendingCharId < 0 || data.CharacterId != this._pendingCharId;
		if (!flag)
		{
			this._pendingCharId = -1;
			bool flag2 = this._showingCharId == data.CharacterId;
			if (flag2)
			{
				this.Show(data);
			}
		}
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x001DA035 File Offset: 0x001D8235
	private void ClearCache()
	{
		this._pendingCharId = -1;
		this._showingCharId = -1;
	}

	// Token: 0x06003A39 RID: 14905 RVA: 0x001DA048 File Offset: 0x001D8248
	public void Show(CharacterDisplayData data)
	{
		bool flag = data == null;
		if (flag)
		{
			this.Hide();
		}
		else
		{
			TwelveImmortalsItem config = data.GetTwelveImmortalsConfig();
			bool flag2 = config == null || data.TemplateId < 1075 || data.TemplateId > 1086;
			if (flag2)
			{
				this.Hide();
			}
			else
			{
				Location location = data.Location;
				bool flag3 = !location.IsValid();
				if (flag3)
				{
					this.Hide();
				}
				else
				{
					this.BuildEdgeBlocks(location, config.ImpactRange);
					List<Location> locations = EasyPool.Get<List<Location>>();
					foreach (short blockId in this._edgeBlocks.Keys)
					{
						locations.Add(new Location(this._areaId, blockId));
					}
					base.RefreshAll(locations);
					EasyPool.Free<List<Location>>(locations);
				}
			}
		}
	}

	// Token: 0x06003A3A RID: 14906 RVA: 0x001DA148 File Offset: 0x001D8348
	public void Hide()
	{
		base.Clear();
		this._edgeBlocks.Clear();
		this._areaId = -1;
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x001DA168 File Offset: 0x001D8368
	public override void TryRefresh(Location location, bool visible, short settlementBlockId = 0)
	{
		GameObject effect;
		bool flag = !this._placedEffects.TryGetValue(new ValueTuple<Location, short>(location, settlementBlockId), out effect);
		if (!flag)
		{
			bool show = this._edgeBlocks.Count > 0 && location.AreaId == this._areaId && this._edgeBlocks.ContainsKey(location.BlockId);
			effect.SetActive(show);
		}
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x001DA1CC File Offset: 0x001D83CC
	protected override bool GetEffectVisible(MapBlockData blockData)
	{
		return this._edgeBlocks.Count > 0 && blockData.AreaId == this._areaId && this._edgeBlocks.ContainsKey(blockData.BlockId);
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x001DA200 File Offset: 0x001D8400
	public int GetPrefabIndex(Location location, short settlementBlockId)
	{
		sbyte neighborBit;
		bool flag = location.AreaId != this._areaId || !this._edgeBlocks.TryGetValue(location.BlockId, out neighborBit);
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			if (!true)
			{
			}
			int num;
			switch (neighborBit)
			{
			case 1:
				num = 7;
				goto IL_92;
			case 2:
				num = 5;
				goto IL_92;
			case 3:
				num = 6;
				goto IL_92;
			case 4:
				num = 2;
				goto IL_92;
			case 5:
				num = 4;
				goto IL_92;
			case 8:
				num = 0;
				goto IL_92;
			case 10:
				num = 3;
				goto IL_92;
			case 12:
				num = 1;
				goto IL_92;
			}
			num = -1;
			IL_92:
			if (!true)
			{
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06003A3E RID: 14910 RVA: 0x001DA2A8 File Offset: 0x001D84A8
	private void BuildEdgeBlocks(Location centerLocation, int impactRange)
	{
		this._edgeBlocks.Clear();
		this._areaId = centerLocation.AreaId;
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		List<MapBlockData> rangeBlocks = EasyPool.Get<List<MapBlockData>>();
		MapBlockCharacterImpactRangeEffectContainer.CollectImpactRangeBlocks(mapModel, centerLocation.AreaId, centerLocation.BlockId, impactRange, rangeBlocks);
		bool flag = rangeBlocks.Count == 0;
		if (flag)
		{
			EasyPool.Free<List<MapBlockData>>(rangeBlocks);
		}
		else
		{
			Dictionary<short, int> blockDict = EasyPool.Get<Dictionary<short, int>>();
			foreach (MapBlockData block in rangeBlocks)
			{
				blockDict[block.BlockId] = 0;
			}
			byte areaSize = mapModel.GetAreaSize(this._areaId);
			foreach (short blockId in blockDict.Keys)
			{
				this._edgeBlocks[blockId] = MapBlockEightDirectionType.GetDirectionType(blockDict, areaSize, blockId);
			}
			EasyPool.Free<Dictionary<short, int>>(blockDict);
			EasyPool.Free<List<MapBlockData>>(rangeBlocks);
		}
	}

	// Token: 0x06003A3F RID: 14911 RVA: 0x001DA3D0 File Offset: 0x001D85D0
	private static void CollectImpactRangeBlocks(WorldMapModel mapModel, short areaId, short blockId, int maxSteps, List<MapBlockData> neighborBlocks)
	{
		byte areaSize = mapModel.GetAreaSize(areaId);
		List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
		mapModel.GetAreaBlocks(areaId, areaBlocks);
		MapBlockData centerBlock = areaBlocks[(int)blockId];
		ByteCoordinate blockPos = WorldMapModel.IndexToCoordinate(centerBlock.BlockId, areaSize);
		neighborBlocks.Clear();
		byte x = (byte)Math.Max((int)blockPos.X - maxSteps, 0);
		while ((int)x < Math.Min((int)blockPos.X + maxSteps + 1, (int)areaSize))
		{
			byte y = (byte)Math.Max((int)blockPos.Y - maxSteps, 0);
			while ((int)y < Math.Min((int)blockPos.Y + maxSteps + 1, (int)areaSize))
			{
				ByteCoordinate pos = new ByteCoordinate(x, y);
				MapBlockData neighborBlock = areaBlocks[(int)WorldMapModel.CoordinateToIndex(pos, areaSize)];
				bool flag = neighborBlock.IsPassable() && blockPos.GetManhattanDistance(pos) <= maxSteps && !neighborBlocks.Contains(neighborBlock);
				if (flag)
				{
					neighborBlocks.Add(neighborBlock);
				}
				y += 1;
			}
			x += 1;
		}
		EasyPool.Free<List<MapBlockData>>(areaBlocks);
	}

	// Token: 0x040029FA RID: 10746
	private readonly Dictionary<short, sbyte> _edgeBlocks = new Dictionary<short, sbyte>();

	// Token: 0x040029FB RID: 10747
	private short _areaId = -1;

	// Token: 0x040029FC RID: 10748
	private int _showingCharId = -1;

	// Token: 0x040029FD RID: 10749
	private int _pendingCharId = -1;

	// Token: 0x040029FE RID: 10750
	private DispatcherInstance _dispatcher;
}
