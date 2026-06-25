using System;
using System.Collections.Generic;
using Config;
using GameData.Common.Algorithm;
using GameData.Domains.Map;
using GameData.Utilities;
using GameDataExtensions;

// Token: 0x02000024 RID: 36
public class AStarMap
{
	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000111 RID: 273 RVA: 0x00008083 File Offset: 0x00006283
	private WorldMapModel MapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000112 RID: 274 RVA: 0x0000808A File Offset: 0x0000628A
	private int MapSize
	{
		get
		{
			return (this.MapModel.CurrentAreaId < 0) ? -1 : ((int)this.MapModel.AreaMapSize[this.MapModel.CurrentAreaId]);
		}
	}

	// Token: 0x06000113 RID: 275 RVA: 0x000080B8 File Offset: 0x000062B8
	public AStarMap()
	{
		this._algorithm = new AStarAlgorithmSimple<IntPos2>(new AStarAlgorithmSimple<IntPos2>.GetValidNeighbors(this.GetBlockValidNeighbors), new AStarAlgorithmSimple<IntPos2>.GetMoveCost(this.GetBlockPathFindingCost));
	}

	// Token: 0x06000114 RID: 276 RVA: 0x000080E8 File Offset: 0x000062E8
	public int GetMoveCostActionPoint(short blockId)
	{
		bool flag = this.MapSize < 0;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			byte mapSize = (byte)this.MapSize;
			ByteCoordinate from = WorldMapModel.IndexToCoordinate(this.MapModel.CurrentBlockId, mapSize);
			ByteCoordinate to = WorldMapModel.IndexToCoordinate(blockId, mapSize);
			IReadOnlyList<IntPos2> tempPath = this._algorithm.FindShortestPath(from.ToIntPos(), to.ToIntPos());
			bool flag2 = tempPath == null;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				int cost = 0;
				foreach (IntPos2 posInt in tempPath)
				{
					ByteCoordinate pos = posInt.ToByteCoordinate();
					bool flag3 = pos == from;
					if (!flag3)
					{
						short pathBlockId = WorldMapModel.CoordinateToIndex(pos, mapSize);
						MapBlockData blockData = this.MapModel.GetBlockData(pathBlockId);
						cost += blockData.MoveCostActionPoint;
					}
				}
				result = cost;
			}
		}
		return result;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x000081E0 File Offset: 0x000063E0
	public void FindWay(IList<Location> path, short blockId)
	{
		bool flag = this.MapSize < 0;
		if (!flag)
		{
			byte mapSize = (byte)this.MapSize;
			ByteCoordinate from = WorldMapModel.IndexToCoordinate(this.MapModel.CurrentBlockId, mapSize);
			ByteCoordinate to = WorldMapModel.IndexToCoordinate(blockId, mapSize);
			IReadOnlyList<IntPos2> tempPath = this._algorithm.FindShortestPath(from.ToIntPos(), to.ToIntPos());
			bool flag2 = tempPath == null;
			if (!flag2)
			{
				foreach (IntPos2 posInt in tempPath)
				{
					ByteCoordinate pos = posInt.ToByteCoordinate();
					short pathBlockId = WorldMapModel.CoordinateToIndex(pos, mapSize);
					path.Add(new Location(this.MapModel.CurrentAreaId, pathBlockId));
				}
			}
		}
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000082B4 File Offset: 0x000064B4
	private IEnumerable<IntPos2> GetBlockValidNeighbors(IntPos2 pos)
	{
		int mapSize = this.MapSize;
		bool flag = mapSize < 0;
		if (flag)
		{
			yield break;
		}
		foreach (IntPos2 direction in IntPos2.Directions)
		{
			IntPos2 neighbor = pos + direction;
			bool flag2 = neighbor.X < 0 || neighbor.X >= mapSize || neighbor.Y < 0 || neighbor.Y >= mapSize;
			if (!flag2)
			{
				bool flag3 = this.GetBlockPathFindingCost(pos) < 0;
				if (!flag3)
				{
					yield return neighbor;
				}
			}
		}
		IntPos2[] array = null;
		yield break;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x000082CC File Offset: 0x000064CC
	private int GetBlockPathFindingCost(IntPos2 pos)
	{
		short blockIndex = WorldMapModel.PositionToIndex((byte)pos.X, (byte)pos.Y, (byte)this.MapSize);
		MapBlockData blockData = this.MapModel.GetBlockData(blockIndex);
		MapBlockItem config = blockData.GetConfig();
		bool flag = config.Type == EMapBlockType.Invalid;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			int costBase = AStarMap.GetBlockPathFindingCostBase(blockData, config);
			int costTime = blockData.MoveCostActionPoint / 10;
			result = Math.Max(costBase, costTime);
		}
		return result;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00008340 File Offset: 0x00006540
	private static int GetBlockPathFindingCostBase(MapBlockData blockData, MapBlockItem config)
	{
		bool destroyed = blockData.Destroyed;
		int result;
		if (destroyed)
		{
			result = GlobalConfig.Instance.MapDestroyedBlockPathingCost;
		}
		else
		{
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			bool flag;
			if (settings.AvoidHereticAttackBlocks)
			{
				HashSet<int> hashSet = blockData.EnemyCharacterSet;
				flag = (hashSet != null && hashSet.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				result = GlobalConfig.Instance.MapDestroyedBlockPathingCost + 1;
			}
			else
			{
				bool flag3;
				if (settings.AvoidInfectedCharacterBlocks)
				{
					HashSet<int> hashSet = blockData.InfectedCharacterSet;
					flag3 = (hashSet != null && hashSet.Count > 0);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				if (flag4)
				{
					result = GlobalConfig.Instance.MapDestroyedBlockPathingCost + 1;
				}
				else
				{
					result = (int)config.PathFindingCost;
				}
			}
		}
		return result;
	}

	// Token: 0x040000AE RID: 174
	private readonly AStarAlgorithmSimple<IntPos2> _algorithm;
}
