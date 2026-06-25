using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Views.Migrate;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.Utilities;
using GameDataExtensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map.RenderSystem
{
	// Token: 0x020006D2 RID: 1746
	public class MapRenderSystem
	{
		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x0600531E RID: 21278 RVA: 0x00266C0A File Offset: 0x00264E0A
		// (set) Token: 0x0600531F RID: 21279 RVA: 0x00266C12 File Offset: 0x00264E12
		public int CurSize { get; private set; }

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06005320 RID: 21280 RVA: 0x00266C1B File Offset: 0x00264E1B
		// (set) Token: 0x06005321 RID: 21281 RVA: 0x00266C23 File Offset: 0x00264E23
		public MapRenderSystem.EMapState RenderState { get; private set; } = MapRenderSystem.EMapState.Normal;

		// Token: 0x06005322 RID: 21282 RVA: 0x00266C2C File Offset: 0x00264E2C
		private static string GetBlockEffectPath(string effectName)
		{
			return "RemakeResources/Particle/MapBlockEffect/" + effectName;
		}

		// Token: 0x06005323 RID: 21283 RVA: 0x00266C3C File Offset: 0x00264E3C
		public static void UpdateAreaOffset()
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			List<ByteCoordinate> pos2AreaId = new List<ByteCoordinate>();
			for (int i = 0; i < 9; i++)
			{
				bool flag = mapModel.CurrentStateId < 0 && i > 0;
				if (flag)
				{
					break;
				}
				short areaId = mapModel.GetAreaIdByStateIndex(i);
				sbyte[] worldPos = mapModel.Areas[(int)areaId].GetConfig().WorldMapPos;
				pos2AreaId.Add(new ByteCoordinate((byte)worldPos[0], (byte)worldPos[1]));
			}
			List<byte> offsetCalcXList = new List<byte>();
			List<byte> offsetCalcYList = new List<byte>();
			Vector2 blockPosOffset = Vector2.zero;
			int firstAreaSize = 0;
			sbyte firstAreaPosX = 0;
			sbyte firstAreaPosY = 0;
			MapRenderSystem.AreaOffset[0] = Vector2.zero;
			for (int j = 0; j < 9; j++)
			{
				short areaId2 = mapModel.GetAreaIdByStateIndex(j);
				MapAreaData areaData = mapModel.Areas[(int)areaId2];
				MapAreaItem areaConfig = areaData.GetConfig();
				bool flag2 = j == 0;
				if (flag2)
				{
					firstAreaSize = (int)mapModel.GetAreaSize(areaId2);
					firstAreaPosX = areaConfig.WorldMapPos[0];
					firstAreaPosY = areaConfig.WorldMapPos[1];
				}
				else
				{
					sbyte posX = areaConfig.WorldMapPos[0];
					sbyte posY = areaConfig.WorldMapPos[1];
					int areaSize = (int)mapModel.GetAreaSize(areaId2);
					float offsetX = (float)(firstAreaSize - areaSize) / 2f;
					float offsetY = (float)(firstAreaSize - areaSize) / 2f;
					int minPosX = Mathf.Min((int)posX, (int)firstAreaPosX);
					int maxPosX = Mathf.Max((int)posX, (int)firstAreaPosX);
					int minPosY = Mathf.Min((int)posY, (int)firstAreaPosY);
					int maxPosY = Mathf.Max((int)posY, (int)firstAreaPosY);
					offsetCalcXList.Clear();
					offsetCalcYList.Clear();
					foreach (ByteCoordinate areaPos in pos2AreaId)
					{
						bool flag3 = !offsetCalcXList.Contains(areaPos.X) && minPosX < (int)areaPos.X && (int)areaPos.X < maxPosX;
						if (flag3)
						{
							offsetCalcXList.Add(areaPos.X);
							offsetX += (float)((posX < firstAreaPosX) ? -35 : 35);
						}
						bool flag4 = !offsetCalcYList.Contains(areaPos.Y) && minPosY < (int)areaPos.Y && (int)areaPos.Y < maxPosY;
						if (flag4)
						{
							offsetCalcYList.Add(areaPos.Y);
							offsetY += (float)((posY < firstAreaPosY) ? -35 : 35);
						}
					}
					bool flag5 = posX != firstAreaPosX;
					if (flag5)
					{
						offsetX += (float)((posX < firstAreaPosX) ? -35 : 35);
					}
					bool flag6 = posY != firstAreaPosY;
					if (flag6)
					{
						offsetY += (float)((posY < firstAreaPosY) ? -35 : 35);
					}
					blockPosOffset.x = (offsetX + offsetY) * MapRenderSystem.MapBlockPosInterval.x;
					blockPosOffset.y = (offsetY - offsetX) * MapRenderSystem.MapBlockPosInterval.y;
					MapRenderSystem.AreaOffset[j] = blockPosOffset;
				}
			}
		}

		// Token: 0x06005324 RID: 21284 RVA: 0x00266F34 File Offset: 0x00265134
		public static Vector2 GetBlockLocalPos(Location location)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = mapModel.GetBlockData(location);
			bool flag = blockData == null;
			Vector2 result;
			if (flag)
			{
				AdaptableLog.TagWarning("GetBlockLocalPos", string.Format("unexpected {0} when it's not loaded.", location), false);
				result = Vector2.one * -10000f;
			}
			else
			{
				Vector2 blockPosOffset = MapRenderSystem.AreaOffset[mapModel.GetAreaIndexInState(location.AreaId)];
				int num;
				if (blockData.RootBlockId >= 0)
				{
					num = 1;
				}
				else
				{
					MapBlockItem config = blockData.GetConfig();
					num = (int)((config != null) ? config.Size : 1);
				}
				int blockSize = num;
				ByteCoordinate blockIndexPos = blockData.GetBlockPos();
				float anchoredPosX = (float)(blockIndexPos.X + blockIndexPos.Y) * MapRenderSystem.MapBlockPosInterval.x;
				float anchoredPosY = (float)(blockIndexPos.Y - blockIndexPos.X) * MapRenderSystem.MapBlockPosInterval.y;
				Vector2 anchoredPos = new Vector2(anchoredPosX, anchoredPosY);
				bool flag2 = blockSize > 1;
				if (flag2)
				{
					anchoredPos.x += (float)(blockSize - 1) * MapRenderSystem.MapBlockPosInterval.x;
					anchoredPos.y -= (float)(blockSize - 1) * MapRenderSystem.MapBlockPosInterval.y;
				}
				MapBlockItem config2 = blockData.GetConfig();
				blockSize = (int)((config2 != null) ? config2.Size : 1);
				Vector2 blockPos = anchoredPos + blockPosOffset;
				blockPos.y += MapRenderSystem.MapBlockPosInterval.y;
				bool flag3 = blockData.RootBlockId < 0 && blockSize > 1;
				if (flag3)
				{
					blockPos.x -= (float)(blockSize - 1) * MapRenderSystem.MapBlockPosInterval.x;
					blockPos.y += (float)(blockSize - 1) * MapRenderSystem.MapBlockPosInterval.y;
				}
				result = blockPos;
			}
			return result;
		}

		// Token: 0x06005325 RID: 21285 RVA: 0x002670D6 File Offset: 0x002652D6
		public IEnumerator InitAndRenderMap(int mapSize, List<MapBlockData> blockDataList, Dictionary<MapRenderSystem.EMapLayer, MapRawImage> rawImagesMap)
		{
			this.Clear();
			this._mapLayerRawImages = new Dictionary<MapRenderSystem.EMapLayer, MapRawImage>();
			foreach (KeyValuePair<MapRenderSystem.EMapLayer, MapRawImage> pair in rawImagesMap)
			{
				pair.Value.Clear();
				this._mapLayerRawImages.Add(pair.Key, pair.Value);
				pair = default(KeyValuePair<MapRenderSystem.EMapLayer, MapRawImage>);
			}
			Dictionary<MapRenderSystem.EMapLayer, MapRawImage>.Enumerator enumerator = default(Dictionary<MapRenderSystem.EMapLayer, MapRawImage>.Enumerator);
			this.InitRenderSystem();
			MapRenderSystem.EdgeColor = MapRenderSystem.NormalEdgeColor;
			this.CurSize = mapSize;
			MapRawImage blockLayer = this._mapLayerRawImages[MapRenderSystem.EMapLayer.MapBlock];
			MapRawImage blockShadowLayer;
			bool flag = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.MapBlockShadow, out blockShadowLayer) && blockLayer.material == blockShadowLayer.material;
			if (flag)
			{
				blockLayer.material = new UnityEngine.Material(blockLayer.material);
				blockLayer.material.SetInt(MapRenderSystem.MultiTextureAmount, 7);
			}
			Dictionary<string, MapBlockRefreshCommand> toSetUpSpriteCommands = new Dictionary<string, MapBlockRefreshCommand>();
			Dictionary<string, MapBlockRefreshCommand> toSetUpEffectCommands = new Dictionary<string, MapBlockRefreshCommand>();
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			int i = 0;
			int max = blockDataList.Count;
			while (i < max)
			{
				MapBlockData blockData = blockDataList[i];
				bool flag2 = blockData.BlockSubType == EMapBlockSubType.None;
				if (!flag2)
				{
					MapBlockRenderInfo blockRenderInfo = this._blockRenderInfoPool.Get();
					Vector2 bottomCenter = this.MapBlockIndexToRenderAnchor((int)blockData.BlockId);
					blockRenderInfo.SetBlockBottomCenter(bottomCenter, blockData.BlockId);
					MapBlockData neighborLeftData = mapModel.GetNeighbor(blockData, MoveDirection.Left, false);
					bool flag3 = neighborLeftData == null || neighborLeftData.BlockSubType == EMapBlockSubType.None;
					if (flag3)
					{
						blockRenderInfo.SetShowAsLeftEdge(true);
					}
					MapBlockData neighborRightData = mapModel.GetNeighbor(blockData, MoveDirection.Right, false);
					bool flag4 = neighborRightData == null || neighborRightData.BlockSubType == EMapBlockSubType.None;
					if (flag4)
					{
						blockRenderInfo.SetShowAsRightEdge(true);
					}
					MapBlockData neighborDownData = mapModel.GetNeighbor(blockData, MoveDirection.Down, false);
					bool flag5 = neighborDownData == null || neighborDownData.BlockSubType == EMapBlockSubType.None;
					if (flag5)
					{
						blockRenderInfo.SetShowAsDownEdge(true);
					}
					MapBlockData neighborUpData = mapModel.GetNeighbor(blockData, MoveDirection.Up, false);
					bool flag6 = neighborUpData == null || neighborUpData.BlockSubType == EMapBlockSubType.None;
					if (flag6)
					{
						blockRenderInfo.SetShowAsUpEdge(true);
					}
					string spriteName = this.GetMapBlockSpriteName(blockData);
					bool flag7 = !string.IsNullOrEmpty(spriteName);
					if (flag7)
					{
						MapBlockRefreshCommand command;
						bool flag8 = !toSetUpSpriteCommands.TryGetValue(spriteName, out command);
						if (flag8)
						{
							command = MapBlockRefreshCommand.CreateSetUpSpriteRefreshCommand();
							command.SpriteName = spriteName;
							toSetUpSpriteCommands.Add(spriteName, command);
						}
						command.RelatedRenderInfoList.Add(blockRenderInfo);
						bool flag9 = blockData.RootBlockId < 0 && !blockData.ShowDestroyed;
						if (flag9)
						{
							List<string> effectNames = this.GetMapBlockEffectNames(blockData);
							foreach (string effectName in effectNames)
							{
								MapBlockRefreshCommand effectCommand;
								bool flag10 = !toSetUpEffectCommands.TryGetValue(effectName, out effectCommand);
								if (flag10)
								{
									effectCommand = MapBlockRefreshCommand.CreateSetUpEffectRefreshCommand();
									effectCommand.EffectName = effectName;
									toSetUpEffectCommands.Add(effectName, effectCommand);
								}
								effectCommand.RelatedRenderInfoList.Add(blockRenderInfo);
								effectCommand = null;
								effectName = null;
							}
							List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
							effectNames = null;
						}
						command = null;
					}
					this._blockRenderInfosInUse.Add((int)blockData.BlockId, blockRenderInfo);
					blockData = null;
					blockRenderInfo = null;
					bottomCenter = default(Vector2);
					neighborLeftData = null;
					neighborRightData = null;
					neighborDownData = null;
					neighborUpData = null;
					spriteName = null;
				}
				int num = i + 1;
				i = num;
			}
			this._cachedBoundary = Rect.zero;
			EasyPool.Free<List<MapBlockData>>(blockDataList);
			List<MapBlockRefreshCommand> commandList = new List<MapBlockRefreshCommand>();
			foreach (KeyValuePair<string, MapBlockRefreshCommand> pair2 in toSetUpSpriteCommands)
			{
				this.UpdateBlockSprite(pair2.Key, pair2.Value, blockLayer);
				commandList.Add(pair2.Value);
				pair2 = default(KeyValuePair<string, MapBlockRefreshCommand>);
			}
			Dictionary<string, MapBlockRefreshCommand>.Enumerator enumerator3 = default(Dictionary<string, MapBlockRefreshCommand>.Enumerator);
			try
			{
				foreach (KeyValuePair<string, MapBlockRefreshCommand> pair3 in toSetUpEffectCommands)
				{
					MapRenderSystem.<>c__DisplayClass54_0 CS$<>8__locals1 = new MapRenderSystem.<>c__DisplayClass54_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.effectName = pair3.Key;
					bool flag11 = this._blockEffectPoolMap.ContainsKey(pair3.Key);
					if (flag11)
					{
						pair3.Value.RelatedRenderInfoList.ForEach(delegate(MapBlockRenderInfo e)
						{
							CS$<>8__locals1.<>4__this.UpdateBlockEffect(e, CS$<>8__locals1.effectName);
						});
						pair3.Value.Complete = true;
					}
					else
					{
						CS$<>8__locals1.command = pair3.Value;
						string effectPath = MapRenderSystem.GetBlockEffectPath(CS$<>8__locals1.effectName);
						ResLoader.Load<GameObject>(effectPath, delegate(GameObject prefab)
						{
							CS$<>8__locals1.<>4__this._blockEffectPoolMap.Add(CS$<>8__locals1.effectName, new PoolItem(CS$<>8__locals1.effectName, prefab));
							prefab.GetComponent<MapBlockEffect>().EffectName = CS$<>8__locals1.effectName;
							List<MapBlockRenderInfo> relatedRenderInfoList = CS$<>8__locals1.command.RelatedRenderInfoList;
							Action<MapBlockRenderInfo> action;
							if ((action = CS$<>8__locals1.<>9__2) == null)
							{
								action = (CS$<>8__locals1.<>9__2 = delegate(MapBlockRenderInfo e)
								{
									CS$<>8__locals1.<>4__this.UpdateBlockEffect(e, CS$<>8__locals1.effectName);
								});
							}
							relatedRenderInfoList.ForEach(action);
							CS$<>8__locals1.command.Complete = true;
						}, null, false);
						commandList.Add(pair3.Value);
						CS$<>8__locals1 = null;
						effectPath = null;
						pair3 = default(KeyValuePair<string, MapBlockRefreshCommand>);
					}
				}
				Dictionary<string, MapBlockRefreshCommand>.Enumerator enumerator4 = default(Dictionary<string, MapBlockRefreshCommand>.Enumerator);
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				Debug.LogError(e2);
				throw;
			}
			bool complete = false;
			while (!complete)
			{
				complete = true;
				foreach (MapBlockRefreshCommand command2 in commandList)
				{
					bool complete2 = command2.Complete;
					if (!complete2)
					{
						complete = false;
						break;
					}
					bool flag12 = command2.Type == MapBlockRefreshCommand.EMapBlockRefreshType.SetUpSprite;
					if (flag12)
					{
						command2.RelatedRenderInfoList.ForEach(delegate(MapBlockRenderInfo e)
						{
							MapBlockRenderInfo.EMapBlockState state = this.GetBlockStateInAnyway(e.BlockIndex);
							e.SetBlockState(state);
						});
					}
				}
				List<MapBlockRefreshCommand>.Enumerator enumerator5 = default(List<MapBlockRefreshCommand>.Enumerator);
				bool flag13 = !complete;
				if (flag13)
				{
					yield return null;
				}
			}
			this.RenderMap();
			yield break;
		}

		// Token: 0x06005326 RID: 21286 RVA: 0x002670FC File Offset: 0x002652FC
		public void UpdateBlockSprite(MapBlockData blockData)
		{
			bool flag = blockData.AreaId != SingletonObject.getInstance<WorldMapModel>().ShowingAreaId;
			if (!flag)
			{
				string spriteName = this.GetMapBlockSpriteName(blockData);
				short blockId = blockData.BlockId;
				MapBlockRenderInfo renderInfo = this.GetBlockRenderInfo((int)blockId);
				Sprite sprite = renderInfo.GetSprite();
				bool flag2 = null != sprite && sprite.name == spriteName;
				if (!flag2)
				{
					MapRawImage layer = this._mapLayerRawImages[MapRenderSystem.EMapLayer.MapBlock];
					MapBlockRefreshCommand command = MapBlockRefreshCommand.CreateSetUpSpriteRefreshCommand();
					command.SpriteName = spriteName;
					command.RelatedRenderInfoList.Add(renderInfo);
					this.UpdateBlockSprite(spriteName, command, layer);
					this.RenderMap();
				}
			}
		}

		// Token: 0x06005327 RID: 21287 RVA: 0x002671A4 File Offset: 0x002653A4
		public void UpdateBlockEffect(MapBlockData blockData)
		{
			bool flag = blockData.AreaId != SingletonObject.getInstance<WorldMapModel>().ShowingAreaId;
			if (!flag)
			{
				MapBlockRenderInfo renderInfo = this.GetBlockRenderInfo((int)blockData.BlockId);
				bool flag2 = blockData.RootBlockId >= 0 || blockData.ShowDestroyed || !blockData.Visible;
				if (flag2)
				{
					bool flag3 = renderInfo.BlockEffectList == null;
					if (!flag3)
					{
						foreach (MapBlockEffect blockEffect in renderInfo.BlockEffectList)
						{
							this.ReturnMapBlockEffect(blockEffect);
						}
						renderInfo.BlockEffectList.Clear();
					}
				}
				else
				{
					List<string> effectNames = this.GetMapBlockEffectNames(blockData);
					foreach (string effectName in effectNames)
					{
						MapBlockEffect effect = this.GetBlockEffect(blockData.BlockId, effectName);
						bool flag4 = effect == null;
						if (flag4)
						{
							this.AddBlockEffect(blockData.BlockId, effectName);
						}
						else
						{
							effect.CurrentState = this.GetBlockEffectState(blockData.BlockId);
						}
					}
					bool flag5 = renderInfo.BlockEffectList == null;
					if (!flag5)
					{
						for (int i = renderInfo.BlockEffectList.Count - 1; i >= 0; i--)
						{
							MapBlockEffect mapEffect = renderInfo.BlockEffectList[i];
							bool flag6 = effectNames.Contains(mapEffect.EffectName);
							if (!flag6)
							{
								this.ReturnMapBlockEffect(mapEffect);
								renderInfo.BlockEffectList.Remove(mapEffect);
							}
						}
					}
				}
			}
		}

		// Token: 0x06005328 RID: 21288 RVA: 0x00267370 File Offset: 0x00265570
		public void UpdateBlockState(MapBlockData blockData)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool flag = blockData.AreaId != mapModel.ShowingAreaId;
			if (!flag)
			{
				MapBlockRenderInfo.EMapBlockState state = this.GetBlockStateInAnyway(blockData.BlockId);
				this.ChangeBlockState(blockData.BlockId, state, true);
			}
		}

		// Token: 0x06005329 RID: 21289 RVA: 0x002673B8 File Offset: 0x002655B8
		public void AdjustMapBlockAdditionalLightState(int width, bool isObliqueDirection)
		{
			MapRenderSystem.<>c__DisplayClass58_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.RenderState = MapRenderSystem.EMapState.AdjustAdditionalLight;
			CS$<>8__locals1.taiwuLocation = SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
			CS$<>8__locals1.mapSize = (byte)this.CurSize;
			ByteCoordinate origin = WorldMapModel.IndexToCoordinate(CS$<>8__locals1.taiwuLocation.BlockId, CS$<>8__locals1.mapSize);
			this._specifyLocations.Clear();
			bool flag = width == 0;
			if (flag)
			{
				if (isObliqueDirection)
				{
					this.<AdjustMapBlockAdditionalLightState>g__AddByteCoordinate|58_0(origin, ref CS$<>8__locals1);
				}
			}
			else
			{
				for (int dy = -width; dy <= width; dy++)
				{
					for (int dx = -width; dx <= width; dx++)
					{
						int x = dx + (int)origin.X;
						int y = dy + (int)origin.Y;
						bool flag2 = x < 0 || y < 0 || x >= (int)CS$<>8__locals1.mapSize || y >= (int)CS$<>8__locals1.mapSize;
						if (!flag2)
						{
							bool flag3 = !isObliqueDirection && MathUtils.GetManhattanDistance((int)origin.X, (int)origin.Y, x, y, 1) > width;
							if (!flag3)
							{
								this.<AdjustMapBlockAdditionalLightState>g__AddSpecifyXy|58_1(x, y, ref CS$<>8__locals1);
							}
						}
					}
				}
			}
			this.RefreshAllMapBlocks(true);
		}

		// Token: 0x0600532A RID: 21290 RVA: 0x002674F8 File Offset: 0x002656F8
		public void AdjustMapBlockStateToBeggarSkill3(int layer, bool isLight)
		{
			this.RenderState = MapRenderSystem.EMapState.AdjustAdditionalLight;
			Location taiwuLocation = SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
			byte mapSize = (byte)this.CurSize;
			ByteCoordinate origin = WorldMapModel.IndexToCoordinate(taiwuLocation.BlockId, mapSize);
			this._specifyLocations.Add(Location.Invalid);
			List<ValueTuple<int, int>> offsets = new List<ValueTuple<int, int>>();
			switch (layer)
			{
			case 1:
				offsets.Add(new ValueTuple<int, int>(0, 1));
				offsets.Add(new ValueTuple<int, int>(1, 0));
				offsets.Add(new ValueTuple<int, int>(0, -1));
				offsets.Add(new ValueTuple<int, int>(-1, 0));
				break;
			case 2:
				offsets.Add(new ValueTuple<int, int>(-1, 1));
				offsets.Add(new ValueTuple<int, int>(1, -1));
				offsets.Add(new ValueTuple<int, int>(-1, -1));
				offsets.Add(new ValueTuple<int, int>(1, 1));
				offsets.Add(new ValueTuple<int, int>(0, 2));
				offsets.Add(new ValueTuple<int, int>(2, 0));
				offsets.Add(new ValueTuple<int, int>(0, -2));
				offsets.Add(new ValueTuple<int, int>(-2, 0));
				offsets.Add(new ValueTuple<int, int>(2, 1));
				offsets.Add(new ValueTuple<int, int>(1, 2));
				offsets.Add(new ValueTuple<int, int>(-2, 1));
				offsets.Add(new ValueTuple<int, int>(1, -2));
				offsets.Add(new ValueTuple<int, int>(2, -1));
				offsets.Add(new ValueTuple<int, int>(-1, 2));
				offsets.Add(new ValueTuple<int, int>(-2, -1));
				offsets.Add(new ValueTuple<int, int>(-1, -2));
				break;
			case 3:
				offsets.Add(new ValueTuple<int, int>(-2, 2));
				offsets.Add(new ValueTuple<int, int>(2, -2));
				offsets.Add(new ValueTuple<int, int>(-2, -2));
				offsets.Add(new ValueTuple<int, int>(2, 2));
				offsets.Add(new ValueTuple<int, int>(0, 3));
				offsets.Add(new ValueTuple<int, int>(3, 0));
				offsets.Add(new ValueTuple<int, int>(0, -3));
				offsets.Add(new ValueTuple<int, int>(-3, 0));
				offsets.Add(new ValueTuple<int, int>(3, 1));
				offsets.Add(new ValueTuple<int, int>(1, 3));
				offsets.Add(new ValueTuple<int, int>(-3, 1));
				offsets.Add(new ValueTuple<int, int>(1, -3));
				offsets.Add(new ValueTuple<int, int>(3, -1));
				offsets.Add(new ValueTuple<int, int>(-1, 3));
				offsets.Add(new ValueTuple<int, int>(-3, -1));
				offsets.Add(new ValueTuple<int, int>(-1, -3));
				offsets.Add(new ValueTuple<int, int>(3, 2));
				offsets.Add(new ValueTuple<int, int>(2, 3));
				offsets.Add(new ValueTuple<int, int>(-3, 2));
				offsets.Add(new ValueTuple<int, int>(2, -3));
				offsets.Add(new ValueTuple<int, int>(3, -2));
				offsets.Add(new ValueTuple<int, int>(-2, 3));
				offsets.Add(new ValueTuple<int, int>(-3, -2));
				offsets.Add(new ValueTuple<int, int>(-2, -3));
				offsets.Add(new ValueTuple<int, int>(0, 4));
				offsets.Add(new ValueTuple<int, int>(0, -4));
				offsets.Add(new ValueTuple<int, int>(1, 4));
				offsets.Add(new ValueTuple<int, int>(1, -4));
				offsets.Add(new ValueTuple<int, int>(-1, 4));
				offsets.Add(new ValueTuple<int, int>(-1, -4));
				offsets.Add(new ValueTuple<int, int>(4, 0));
				offsets.Add(new ValueTuple<int, int>(-4, 0));
				offsets.Add(new ValueTuple<int, int>(4, 1));
				offsets.Add(new ValueTuple<int, int>(-4, 1));
				offsets.Add(new ValueTuple<int, int>(4, -1));
				offsets.Add(new ValueTuple<int, int>(-4, -1));
				break;
			}
			foreach (ValueTuple<int, int> offset in offsets)
			{
				int x = (int)origin.X + offset.Item1;
				int y = (int)origin.Y + offset.Item2;
				bool flag = x < 0 || y < 0 || x >= (int)mapSize || y >= (int)mapSize;
				if (!flag)
				{
					Location location = new Location(taiwuLocation.AreaId, WorldMapModel.CoordinateToIndex(new ByteCoordinate((byte)x, (byte)y), mapSize));
					if (isLight)
					{
						this._specifyLocations.Add(location);
					}
					else
					{
						this._specifyLocations.Remove(location);
					}
				}
			}
			this.RefreshAllMapBlocks(true);
		}

		// Token: 0x0600532B RID: 21291 RVA: 0x0026797C File Offset: 0x00265B7C
		public void SetMapBlockLightAtLocation(Location location)
		{
			this.RenderState = MapRenderSystem.EMapState.AdjustAdditionalLight;
			this._specifyLocations.Clear();
			this._specifyLocations.Add(location);
			this.RefreshAllMapBlocks(true);
		}

		// Token: 0x0600532C RID: 21292 RVA: 0x002679A8 File Offset: 0x00265BA8
		public void ClearMapBlockAdditionalLightState(bool withAnim = false)
		{
			this.RenderState = MapRenderSystem.EMapState.Normal;
			this._specifyLocations.Clear();
			this.RefreshAllMapBlocks(withAnim);
		}

		// Token: 0x0600532D RID: 21293 RVA: 0x002679C8 File Offset: 0x00265BC8
		public void SetNegativeFilmInArea(List<Location> expectLocations, float duration = 0.3f, bool isAnim = true)
		{
			this.RenderState = MapRenderSystem.EMapState.Negative;
			this._specifyLocations.Clear();
			bool flag = expectLocations != null;
			if (flag)
			{
				this._specifyLocations.AddRange(expectLocations);
			}
			this.RefreshAllMapBlocks(isAnim);
		}

		// Token: 0x0600532E RID: 21294 RVA: 0x00267A07 File Offset: 0x00265C07
		public void ReverseClearNegativeFilm(float duration = 0.3f)
		{
			this.RenderState = MapRenderSystem.EMapState.Normal;
			this._specifyLocations.Clear();
			this.RefreshAllMapBlocks(true);
		}

		// Token: 0x0600532F RID: 21295 RVA: 0x00267A28 File Offset: 0x00265C28
		public void RefreshAllMapBlocks(bool isAnim = true)
		{
			foreach (MapBlockRenderInfo blockRenderInfo in this._blockRenderInfosInUse.Values)
			{
				MapBlockRenderInfo.EMapBlockState newState = this.GetBlockStateInAnyway(blockRenderInfo.BlockIndex);
				this.ChangeBlockState(blockRenderInfo.BlockIndex, newState, isAnim);
				this.SetBlockEffect(blockRenderInfo);
			}
			bool flag = this.RenderState == MapRenderSystem.EMapState.Negative;
			if (flag)
			{
				this.RefreshNegativeFilmBaseColors();
			}
		}

		// Token: 0x06005330 RID: 21296 RVA: 0x00267AB8 File Offset: 0x00265CB8
		private void RefreshNegativeFilmBaseColors()
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool anyUpdated = false;
			foreach (MapBlockRenderInfo blockRenderInfo in this._blockRenderInfosInUse.Values)
			{
				bool flag = blockRenderInfo.State != MapBlockRenderInfo.EMapBlockState.NegativeFilm;
				if (!flag)
				{
					Location location = new Location(mapModel.ShowingAreaId, blockRenderInfo.BlockIndex);
					MapBlockData blockData = mapModel.GetBlockData(location);
					bool flag2 = blockData == null || !blockData.Visible;
					if (!flag2)
					{
						Color32 baseColor = mapModel.IsLocationShouldInSight(location) ? MapBlockRenderInfo.GetStateColor(MapBlockRenderInfo.EMapBlockState.InSight) : MapBlockRenderInfo.GetStateColor(MapBlockRenderInfo.EMapBlockState.Visible);
						blockRenderInfo.SetNegativeFilmBaseColor(baseColor);
						anyUpdated = true;
					}
				}
			}
			bool flag3 = anyUpdated;
			if (flag3)
			{
				this.RenderMap();
			}
		}

		// Token: 0x06005331 RID: 21297 RVA: 0x00267B98 File Offset: 0x00265D98
		public void ChangeBlockState(short blockIndex, MapBlockRenderInfo.EMapBlockState newState, bool animFlag = true)
		{
			MapBlockRenderInfo blockRenderInfo = this.GetBlockRenderInfo((int)blockIndex);
			bool flag = blockRenderInfo == null;
			if (flag)
			{
				Debug.LogWarning(string.Format("block {0} can not change state!", blockIndex));
			}
			else
			{
				bool flag2 = blockRenderInfo.State == newState;
				if (!flag2)
				{
					if (animFlag)
					{
						this.AddBlockAnimationCommand(blockRenderInfo, blockRenderInfo.State, newState);
					}
					else
					{
						blockRenderInfo.SetBlockState(newState);
						blockRenderInfo.TryGenerateBlockVertexData();
						if (this._noAnimationRefreshBlockCoroutine == null)
						{
							this._noAnimationRefreshBlockCoroutine = SingletonObject.getInstance<YieldHelper>().StartCoroutine(this.NoAnimationRefreshBlockCoroutine());
						}
					}
				}
			}
		}

		// Token: 0x06005332 RID: 21298 RVA: 0x00267C24 File Offset: 0x00265E24
		public void AddBlockAnimationCommand(MapBlockRenderInfo blockRenderInfo, MapBlockRenderInfo.EMapBlockState fromState, MapBlockRenderInfo.EMapBlockState toState)
		{
			bool shouldStartDelayAction = this._cacheCommandMap.Count <= 0;
			MapBlockRefreshCommand command;
			bool flag = !this._cacheCommandMap.TryGetValue(blockRenderInfo.BlockIndex, out command);
			if (flag)
			{
				command = MapBlockRefreshCommand.CreateChangeBlockStateRefreshCommand();
				command.FromState = fromState;
				command.ToState = toState;
				command.RelatedRenderInfoList.Add(blockRenderInfo);
				this._cacheCommandMap.Add(blockRenderInfo.BlockIndex, command);
			}
			else
			{
				bool flag2 = command.FromState == toState;
				if (flag2)
				{
					this._cacheCommandMap.Remove(blockRenderInfo.BlockIndex);
				}
				else
				{
					command.ToState = toState;
				}
			}
			bool flag3 = shouldStartDelayAction && this._cacheCommandMap.Count > 0;
			if (flag3)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(8U, new Action(this.DelayStartBlockRefresh));
			}
		}

		// Token: 0x06005333 RID: 21299 RVA: 0x00267CF8 File Offset: 0x00265EF8
		public MapBlockRenderInfo GetBlockRenderInfo(int blockIndex)
		{
			MapBlockRenderInfo blockRenderInfo;
			this._blockRenderInfosInUse.TryGetValue(blockIndex, out blockRenderInfo);
			return blockRenderInfo;
		}

		// Token: 0x06005334 RID: 21300 RVA: 0x00267D1C File Offset: 0x00265F1C
		public sbyte GetBlockCornerCodeAndLinkTarget(MapBlockRenderInfo srcBlockRenderInfo, MoveDirection direction, ref MapBlockRenderInfo target)
		{
			sbyte code = 0;
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData srcBlockData = mapModel.GetBlockData(new Location(mapModel.ShowingAreaId, srcBlockRenderInfo.BlockIndex));
			bool flag = direction == MoveDirection.Up;
			if (flag)
			{
				bool flag2 = srcBlockRenderInfo.ShowAsUpEdge && !srcBlockRenderInfo.ShowAsLeftEdge;
				if (flag2)
				{
					MapBlockData neighborBlockData = mapModel.GetNeighbor(srcBlockData, MoveDirection.Left, true);
					bool flag3 = neighborBlockData == null;
					if (flag3)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo = this.GetBlockRenderInfo((int)neighborBlockData.BlockId);
					bool flag4 = neighborBlockRenderInfo == null || neighborBlockRenderInfo.ShowAsUpEdge;
					if (flag4)
					{
						return code;
					}
					MapBlockData upBlockData = mapModel.GetNeighbor(neighborBlockData, MoveDirection.Up, true);
					bool flag5 = upBlockData == null;
					if (flag5)
					{
						return code;
					}
					MapBlockRenderInfo upBlockRenderInfo = this.GetBlockRenderInfo((int)upBlockData.BlockId);
					bool flag6 = upBlockRenderInfo == null;
					if (flag6)
					{
						return code;
					}
					bool showAsRightEdge = upBlockRenderInfo.ShowAsRightEdge;
					if (showAsRightEdge)
					{
						code = 10;
						target = neighborBlockRenderInfo;
						return code;
					}
				}
				bool flag7 = srcBlockRenderInfo.ShowAsLeftEdge && !srcBlockRenderInfo.ShowAsUpEdge;
				if (flag7)
				{
					MapBlockData neighborBlockData2 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Up, true);
					bool flag8 = neighborBlockData2 == null;
					if (flag8)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo2 = this.GetBlockRenderInfo((int)neighborBlockData2.BlockId);
					bool flag9 = neighborBlockRenderInfo2 == null || neighborBlockRenderInfo2.ShowAsLeftEdge;
					if (flag9)
					{
						return code;
					}
					MapBlockData upBlockData2 = mapModel.GetNeighbor(neighborBlockData2, MoveDirection.Left, true);
					bool flag10 = upBlockData2 == null;
					if (flag10)
					{
						return code;
					}
					MapBlockRenderInfo upBlockRenderInfo2 = this.GetBlockRenderInfo((int)upBlockData2.BlockId);
					bool flag11 = upBlockRenderInfo2 == null;
					if (flag11)
					{
						return code;
					}
					bool showAsDownEdge = upBlockRenderInfo2.ShowAsDownEdge;
					if (showAsDownEdge)
					{
						code = 11;
						target = neighborBlockRenderInfo2;
						return code;
					}
				}
			}
			bool flag12 = direction == MoveDirection.Down;
			if (flag12)
			{
				bool flag13 = srcBlockRenderInfo.ShowAsDownEdge && !srcBlockRenderInfo.ShowAsRightEdge;
				if (flag13)
				{
					MapBlockData neighborBlockData3 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Right, true);
					bool flag14 = neighborBlockData3 == null;
					if (flag14)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo3 = this.GetBlockRenderInfo((int)neighborBlockData3.BlockId);
					bool flag15 = neighborBlockRenderInfo3 == null || neighborBlockRenderInfo3.ShowAsDownEdge;
					if (flag15)
					{
						return code;
					}
					MapBlockData downBlockData = mapModel.GetNeighbor(neighborBlockData3, MoveDirection.Down, true);
					bool flag16 = downBlockData == null;
					if (flag16)
					{
						return code;
					}
					MapBlockRenderInfo downBlockRenderInfo = this.GetBlockRenderInfo((int)downBlockData.BlockId);
					bool flag17 = downBlockRenderInfo == null;
					if (flag17)
					{
						return code;
					}
					bool showAsLeftEdge = downBlockRenderInfo.ShowAsLeftEdge;
					if (showAsLeftEdge)
					{
						code = 20;
						target = neighborBlockRenderInfo3;
						return code;
					}
				}
				bool flag18 = srcBlockRenderInfo.ShowAsRightEdge && !srcBlockRenderInfo.ShowAsDownEdge;
				if (flag18)
				{
					MapBlockData neighborBlockData4 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Down, true);
					bool flag19 = neighborBlockData4 == null;
					if (flag19)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo4 = this.GetBlockRenderInfo((int)neighborBlockData4.BlockId);
					bool flag20 = neighborBlockRenderInfo4 == null || neighborBlockRenderInfo4.ShowAsRightEdge;
					if (flag20)
					{
						return code;
					}
					MapBlockData rightBlockData = mapModel.GetNeighbor(neighborBlockData4, MoveDirection.Right, true);
					bool flag21 = rightBlockData == null;
					if (flag21)
					{
						return code;
					}
					MapBlockRenderInfo downBlockRenderInfo2 = this.GetBlockRenderInfo((int)rightBlockData.BlockId);
					bool flag22 = downBlockRenderInfo2 == null;
					if (flag22)
					{
						return code;
					}
					bool showAsUpEdge = downBlockRenderInfo2.ShowAsUpEdge;
					if (showAsUpEdge)
					{
						code = 21;
						target = neighborBlockRenderInfo4;
						return code;
					}
				}
			}
			bool flag23 = direction == MoveDirection.Left;
			if (flag23)
			{
				bool flag24 = srcBlockRenderInfo.ShowAsLeftEdge && !srcBlockRenderInfo.ShowAsDownEdge;
				if (flag24)
				{
					MapBlockData neighborBlockData5 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Down, true);
					bool flag25 = neighborBlockData5 == null;
					if (flag25)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo5 = this.GetBlockRenderInfo((int)neighborBlockData5.BlockId);
					bool flag26 = neighborBlockRenderInfo5 == null || neighborBlockRenderInfo5.ShowAsLeftEdge;
					if (flag26)
					{
						return code;
					}
					MapBlockData leftBlockData = mapModel.GetNeighbor(neighborBlockData5, MoveDirection.Left, true);
					bool flag27 = leftBlockData == null;
					if (flag27)
					{
						return code;
					}
					MapBlockRenderInfo leftBlockRenderInfo = this.GetBlockRenderInfo((int)leftBlockData.BlockId);
					bool flag28 = leftBlockRenderInfo == null;
					if (flag28)
					{
						return code;
					}
					bool showAsUpEdge2 = leftBlockRenderInfo.ShowAsUpEdge;
					if (showAsUpEdge2)
					{
						code = 30;
						target = neighborBlockRenderInfo5;
						return code;
					}
				}
				bool flag29 = srcBlockRenderInfo.ShowAsDownEdge && !srcBlockRenderInfo.ShowAsLeftEdge;
				if (flag29)
				{
					MapBlockData neighborBlockData6 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Left, true);
					bool flag30 = neighborBlockData6 == null;
					if (flag30)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo6 = this.GetBlockRenderInfo((int)neighborBlockData6.BlockId);
					bool flag31 = neighborBlockRenderInfo6 == null || neighborBlockRenderInfo6.ShowAsDownEdge;
					if (flag31)
					{
						return code;
					}
					MapBlockData downBlockData2 = mapModel.GetNeighbor(neighborBlockData6, MoveDirection.Down, true);
					bool flag32 = downBlockData2 == null;
					if (flag32)
					{
						return code;
					}
					MapBlockRenderInfo leftBlockRenderInfo2 = this.GetBlockRenderInfo((int)downBlockData2.BlockId);
					bool flag33 = leftBlockRenderInfo2 == null;
					if (flag33)
					{
						return code;
					}
					bool showAsRightEdge2 = leftBlockRenderInfo2.ShowAsRightEdge;
					if (showAsRightEdge2)
					{
						code = 31;
						target = neighborBlockRenderInfo6;
						return code;
					}
				}
			}
			bool flag34 = direction == MoveDirection.Right;
			if (flag34)
			{
				bool flag35 = srcBlockRenderInfo.ShowAsRightEdge && !srcBlockRenderInfo.ShowAsUpEdge;
				if (flag35)
				{
					MapBlockData neighborBlockData7 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Up, true);
					bool flag36 = neighborBlockData7 == null;
					if (flag36)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo7 = this.GetBlockRenderInfo((int)neighborBlockData7.BlockId);
					bool flag37 = neighborBlockRenderInfo7 == null || neighborBlockRenderInfo7.ShowAsRightEdge;
					if (flag37)
					{
						return code;
					}
					MapBlockData rightBlockData2 = mapModel.GetNeighbor(neighborBlockData7, MoveDirection.Right, true);
					bool flag38 = rightBlockData2 == null;
					if (flag38)
					{
						return code;
					}
					MapBlockRenderInfo rightBlockRenderInfo = this.GetBlockRenderInfo((int)rightBlockData2.BlockId);
					bool flag39 = rightBlockRenderInfo == null;
					if (flag39)
					{
						return code;
					}
					bool showAsDownEdge2 = rightBlockRenderInfo.ShowAsDownEdge;
					if (showAsDownEdge2)
					{
						code = 40;
						target = neighborBlockRenderInfo7;
						return code;
					}
				}
				bool flag40 = srcBlockRenderInfo.ShowAsUpEdge && !srcBlockRenderInfo.ShowAsRightEdge;
				if (flag40)
				{
					MapBlockData neighborBlockData8 = mapModel.GetNeighbor(srcBlockData, MoveDirection.Right, true);
					bool flag41 = neighborBlockData8 == null;
					if (flag41)
					{
						return code;
					}
					MapBlockRenderInfo neighborBlockRenderInfo8 = this.GetBlockRenderInfo((int)neighborBlockData8.BlockId);
					bool flag42 = neighborBlockRenderInfo8 == null || neighborBlockRenderInfo8.ShowAsUpEdge;
					if (flag42)
					{
						return code;
					}
					MapBlockData upBlockData3 = mapModel.GetNeighbor(neighborBlockData8, MoveDirection.Up, true);
					bool flag43 = upBlockData3 == null;
					if (flag43)
					{
						return code;
					}
					MapBlockRenderInfo rightBlockRenderInfo2 = this.GetBlockRenderInfo((int)upBlockData3.BlockId);
					bool flag44 = rightBlockRenderInfo2 == null;
					if (flag44)
					{
						return code;
					}
					bool showAsLeftEdge2 = rightBlockRenderInfo2.ShowAsLeftEdge;
					if (showAsLeftEdge2)
					{
						code = 41;
						target = neighborBlockRenderInfo8;
						return code;
					}
				}
			}
			return code;
		}

		// Token: 0x06005335 RID: 21301 RVA: 0x002683B0 File Offset: 0x002665B0
		public static Vector2 CalcDefaultMapLayerPosition(short showingAreaId)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			byte showingAreaSize = mapModel.GetAreaSize(showingAreaId);
			float width = (float)(MapRenderSystem.BlockRenderSpace.x * (int)showingAreaSize);
			Vector2 blockPositionOffset = Vector2.zero;
			for (int i = 0; i < 9; i++)
			{
				bool flag = mapModel.GetAreaIdByStateIndex(i) == showingAreaId;
				if (flag)
				{
					blockPositionOffset = MapRenderSystem.AreaOffset[i];
					break;
				}
			}
			return new Vector2(width * 0.5f + blockPositionOffset.x - (float)MapRenderSystem.BlockRenderSpace.x * 0.5f, blockPositionOffset.y + MapRenderSystem.BlockBaseHeight * 0.5f);
		}

		// Token: 0x06005336 RID: 21302 RVA: 0x00268460 File Offset: 0x00266660
		public Rect CalcBoundary(bool force = false)
		{
			bool flag = !force && !Mathf.Approximately(this._cachedBoundary.size.sqrMagnitude, 0f);
			Rect result;
			if (flag)
			{
				result = this._cachedBoundary;
			}
			else
			{
				MapRawImage areaBorder;
				List<UIVertex> borderVertices;
				bool flag2;
				if (this._mapLayerRawImages != null && this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.BlockBaseAndBorder, out areaBorder))
				{
					borderVertices = areaBorder.VertexStream;
					flag2 = (borderVertices != null && borderVertices.Count > 0);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					float xMax = borderVertices.Max((UIVertex v) => v.position.x);
					float xMin = borderVertices.Min((UIVertex v) => v.position.x);
					float yMax = borderVertices.Max((UIVertex v) => v.position.y);
					float yMin = borderVertices.Min((UIVertex v) => v.position.y);
					result = (this._cachedBoundary = new Rect(xMin, yMin, xMax - xMin, yMax - yMin));
				}
				else
				{
					result = (this._cachedBoundary = Rect.zero);
				}
			}
			return result;
		}

		// Token: 0x06005337 RID: 21303 RVA: 0x002685B8 File Offset: 0x002667B8
		public void ReturnMapBlockEffect(MapBlockEffect effect)
		{
			bool flag = null == effect;
			if (!flag)
			{
				bool flag2 = !this._blockEffectPoolMap.ContainsKey(effect.EffectName);
				if (!flag2)
				{
					effect.ChangeVisibility(false);
					this._blockEffectPoolMap[effect.EffectName].DestroyObject(effect.gameObject);
				}
			}
		}

		// Token: 0x06005338 RID: 21304 RVA: 0x00268614 File Offset: 0x00266814
		public void AddBlockEffect(short blockIndex, string effectName)
		{
			MapRenderSystem.<>c__DisplayClass73_0 CS$<>8__locals1 = new MapRenderSystem.<>c__DisplayClass73_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.effectName = effectName;
			CS$<>8__locals1.blockIndex = blockIndex;
			CS$<>8__locals1.blockRenderInfo = this.GetBlockRenderInfo((int)CS$<>8__locals1.blockIndex);
			bool flag = CS$<>8__locals1.blockRenderInfo == null;
			if (!flag)
			{
				CS$<>8__locals1.blockEffect = this.GetBlockEffect(CS$<>8__locals1.blockIndex, CS$<>8__locals1.effectName);
				bool flag2 = null != CS$<>8__locals1.blockEffect;
				if (flag2)
				{
					this.SetBlockEffect(CS$<>8__locals1.blockRenderInfo);
				}
				else
				{
					MapBlockRenderInfo blockRenderInfo = CS$<>8__locals1.blockRenderInfo;
					if (blockRenderInfo.BlockEffectList == null)
					{
						blockRenderInfo.BlockEffectList = new List<MapBlockEffect>();
					}
					bool flag3 = this._blockEffectPoolMap.ContainsKey(CS$<>8__locals1.effectName);
					if (flag3)
					{
						CS$<>8__locals1.<AddBlockEffect>g__SetUpBlockEffect|0();
					}
					else
					{
						string effectPath = MapRenderSystem.GetBlockEffectPath(CS$<>8__locals1.effectName);
						ResLoader.Load<GameObject>(effectPath, delegate(GameObject prefab)
						{
							bool flag4 = CS$<>8__locals1.<>4__this._blockEffectPoolMap.ContainsKey(CS$<>8__locals1.effectName);
							if (flag4)
							{
								base.<AddBlockEffect>g__SetUpBlockEffect|0();
							}
							else
							{
								prefab.GetComponent<MapBlockEffect>().EffectName = CS$<>8__locals1.effectName;
								CS$<>8__locals1.<>4__this._blockEffectPoolMap.Add(CS$<>8__locals1.effectName, new PoolItem(CS$<>8__locals1.effectName, prefab));
								base.<AddBlockEffect>g__SetUpBlockEffect|0();
							}
						}, null, false);
					}
				}
			}
		}

		// Token: 0x06005339 RID: 21305 RVA: 0x002686FC File Offset: 0x002668FC
		public void RemoveBlockEffect(short blockIndex, string effectName)
		{
			MapBlockRenderInfo blockRenderInfo = this.GetBlockRenderInfo((int)blockIndex);
			bool flag = blockRenderInfo == null || blockRenderInfo.BlockEffectList == null;
			if (!flag)
			{
				List<MapBlockEffect> toRemoveEffectList = blockRenderInfo.BlockEffectList.FindAll((MapBlockEffect e) => e.EffectName == effectName);
				foreach (MapBlockEffect blockEffect in toRemoveEffectList)
				{
					this.ReturnMapBlockEffect(blockEffect);
					blockRenderInfo.BlockEffectList.Remove(blockEffect);
				}
			}
		}

		// Token: 0x0600533A RID: 21306 RVA: 0x002687A4 File Offset: 0x002669A4
		public MapBlockEffect GetBlockEffect(short blockIndex, string effectName)
		{
			MapBlockRenderInfo blockRenderInfo = this.GetBlockRenderInfo((int)blockIndex);
			bool flag = blockRenderInfo != null && blockRenderInfo.BlockEffectList != null;
			MapBlockEffect result;
			if (flag)
			{
				result = blockRenderInfo.BlockEffectList.Find((MapBlockEffect e) => e.EffectName == effectName);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600533B RID: 21307 RVA: 0x002687FC File Offset: 0x002669FC
		public string GetMapBlockSpriteName(MapBlockData blockData)
		{
			return this.GetMapBlockSpriteNameInternal(blockData, EMapBlockSpriteFixType.None);
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x00268818 File Offset: 0x00266A18
		public string GetMapBlockSpriteNameByFix(MapBlockData blockData)
		{
			return this.GetMapBlockSpriteNameInternal(blockData, EMapBlockSpriteFixType.FixSingle);
		}

		// Token: 0x0600533D RID: 21309 RVA: 0x00268834 File Offset: 0x00266A34
		public string GetMapBlockSpriteNameByFixWithFull(MapBlockData blockData)
		{
			return this.GetMapBlockSpriteNameInternal(blockData, EMapBlockSpriteFixType.FixFullSize);
		}

		// Token: 0x0600533E RID: 21310 RVA: 0x00268850 File Offset: 0x00266A50
		private string GetMapBlockSpriteNameInternal(MapBlockData blockData, EMapBlockSpriteFixType fixType)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockItem blockConfig = blockData.GetConfig();
			bool flag = blockConfig == null;
			string result2;
			if (flag)
			{
				result2 = string.Empty;
			}
			else
			{
				bool showDestroyed = blockData.ShowDestroyed;
				if (showDestroyed)
				{
					result2 = "map_block_special_destroyed_" + ((int)(blockData.BlockId % 3 + 1)).ToString();
				}
				else
				{
					bool byFix = fixType.IsFix() && blockConfig.BlockHasFix;
					StringBuilder builder = EasyPool.Get<StringBuilder>();
					builder.Append("map_block_");
					bool flag2 = byFix;
					if (flag2)
					{
						builder.Append("fix_");
					}
					builder.Append(blockConfig.Art);
					int number = this.GetMapBlockSpriteNameNumber(blockData);
					bool flag3 = number >= 0;
					if (flag3)
					{
						builder.Append("_");
						builder.Append(number);
					}
					int size = byFix ? -1 : this.GetMapBlockSpriteNameSize(blockData);
					bool flag4 = size >= 0;
					if (flag4)
					{
						builder.Append("_");
						builder.Append(size);
					}
					else
					{
						bool flag5 = blockConfig.Size > 1 && fixType == EMapBlockSpriteFixType.FixFullSize;
						if (flag5)
						{
							builder.Append("_full");
						}
					}
					bool blockHasDirection = blockConfig.BlockHasDirection;
					if (blockHasDirection)
					{
						MapAreaItem areaConfig = mapModel.Areas[(int)blockData.AreaId].GetConfig();
						StringBuilder stringBuilder = builder;
						EMapAreaAreaDirection areaDirection = areaConfig.AreaDirection;
						if (!true)
						{
						}
						string value;
						switch (areaDirection)
						{
						case EMapAreaAreaDirection.South:
							value = "_s";
							break;
						case EMapAreaAreaDirection.North:
							value = "_n";
							break;
						case EMapAreaAreaDirection.West:
							value = "_w";
							break;
						default:
							value = string.Empty;
							break;
						}
						if (!true)
						{
						}
						stringBuilder.Append(value);
					}
					bool blockHasSeason = blockConfig.BlockHasSeason;
					if (blockHasSeason)
					{
						sbyte season = TimeKit.GetCurrSeason();
						bool flag6 = season == 3;
						if (flag6)
						{
							builder.Append("_winter");
						}
					}
					string result = builder.ToString();
					EasyPool.Free<StringBuilder>(builder);
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x0600533F RID: 21311 RVA: 0x00268A38 File Offset: 0x00266C38
		private int GetMapBlockSpriteNameNumber(MapBlockData blockData)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockItem config = blockData.GetConfig();
			int[] blockNumbers = config.BlockNumbers;
			bool flag = blockNumbers == null || blockNumbers.Length <= 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				blockNumbers = config.BlockNumbers;
				bool flag2 = blockNumbers == null || blockNumbers.Length <= 1;
				if (flag2)
				{
					result = config.BlockNumbers[0];
				}
				else
				{
					bool flag3 = config.TemplateId == 0;
					if (flag3)
					{
						result = config.BlockNumbers[mapModel.TaiwuVillageShowShrine ? 0 : 1];
					}
					else
					{
						bool flag4 = config.TemplateId == 16;
						if (flag4)
						{
							result = config.BlockNumbers[mapModel.SecretVillageOnFire ? 1 : 0];
						}
						else
						{
							bool flag5 = config.SubType == EMapBlockSubType.Ruin;
							if (flag5)
							{
								result = config.BlockNumbers[mapModel.IsLocationContainsMaterial(blockData.GetLocation()) ? 1 : 0];
							}
							else
							{
								result = config.BlockNumbers[(int)blockData.BlockId % config.BlockNumbers.Length];
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005340 RID: 21312 RVA: 0x00268B38 File Offset: 0x00266D38
		private int GetMapBlockSpriteNameSize(MapBlockData blockData)
		{
			ByteCoordinate blockPos = blockData.GetBlockPos();
			MapBlockData rootBlockData = blockData.GetRootBlock();
			ByteCoordinate rootBlockPos = rootBlockData.GetBlockPos();
			Vector2Int rootOffset = blockPos.ToVector2Int() - rootBlockPos.ToVector2Int();
			MapBlockItem config = blockData.GetConfig();
			byte size = config.Size;
			if (!true)
			{
			}
			int result;
			if (size != 2)
			{
				if (size != 3)
				{
					result = -1;
				}
				else
				{
					result = MapRenderSystem.MapBlockOffsetMulti3X3[rootOffset];
				}
			}
			else
			{
				result = MapRenderSystem.MapBlockOffsetMulti2X2[rootOffset];
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06005341 RID: 21313 RVA: 0x00268BC0 File Offset: 0x00266DC0
		private Vector2 MapBlockIndexToRenderAnchor(int blockIndex)
		{
			int x = blockIndex / this.CurSize;
			int y = blockIndex % this.CurSize;
			int renderX = x + y - (this.CurSize - 1);
			int renderY = x - y;
			return new Vector2((float)renderX * 0.5f * (float)MapRenderSystem.BlockRenderSpace.x, (float)(renderY - 1) * MapRenderSystem.BlockBaseHeight * 0.5f);
		}

		// Token: 0x06005342 RID: 21314 RVA: 0x00268C24 File Offset: 0x00266E24
		private void Clear()
		{
			foreach (KeyValuePair<int, MapBlockRenderInfo> pair in this._blockRenderInfosInUse)
			{
				pair.Value.Clear();
				this._blockRenderInfoPool.Return(pair.Value);
			}
			this._blockRenderInfosInUse.Clear();
			this._cachedBoundary = Rect.zero;
			bool flag = this._mapLayerRawImages != null;
			if (flag)
			{
				foreach (KeyValuePair<MapRenderSystem.EMapLayer, MapRawImage> pair2 in this._mapLayerRawImages)
				{
					pair2.Value.Clear();
				}
				this._mapLayerRawImages.Clear();
			}
			this._mapLayerRawImages = null;
			this._mapAtlasNameList.Clear();
		}

		// Token: 0x06005343 RID: 21315 RVA: 0x00268D28 File Offset: 0x00266F28
		private void InitRenderSystem()
		{
			MapBlockRenderInfo.System = this;
			bool flag = null == MapRenderSystem.BlockShadowSprite;
			if (flag)
			{
				MapAtlasInfo.Instance.GetSprite(MapRenderSystem.BlockShadowSpriteName, delegate(Sprite sprite)
				{
					MapRenderSystem.BlockShadowSprite = sprite;
				});
			}
			bool flag2 = this._mapLayerRawImages.ContainsKey(MapRenderSystem.EMapLayer.BlockBaseAndBorder);
			if (flag2)
			{
				bool flag3 = null == MapRenderSystem.BlockBaseSprite;
				if (flag3)
				{
					MapAtlasInfo.Instance.GetSprite(MapRenderSystem.BlockBaseSpriteName, delegate(Sprite sprite)
					{
						MapRenderSystem.BlockBaseSprite = sprite;
						this._mapLayerRawImages[MapRenderSystem.EMapLayer.BlockBaseAndBorder].SetTextureToIndex(0, sprite.texture);
					});
				}
				bool flag4 = null == MapRenderSystem.DownAreaEdgeSprite;
				if (flag4)
				{
					MapAtlasInfo.Instance.GetSprite(MapRenderSystem.DownAreaEdgeSpriteName, delegate(Sprite sprite)
					{
						MapRenderSystem.DownAreaEdgeSprite = sprite;
					});
				}
				bool flag5 = null == MapRenderSystem.AreaTableCornerSprite;
				if (flag5)
				{
					MapAtlasInfo.Instance.GetSprite(MapRenderSystem.AreaCornerTableSpriteName, delegate(Sprite sprite)
					{
						MapRenderSystem.AreaTableCornerSprite = sprite;
					});
				}
				bool flag6 = null == MapRenderSystem.UpAreaEdgeSprite;
				if (flag6)
				{
					MapAtlasInfo.Instance.GetSprite(MapRenderSystem.UpAreaEdgeSpriteName, delegate(Sprite sprite)
					{
						MapRenderSystem.UpAreaEdgeSprite = sprite;
					});
				}
				bool flag7 = null == MapRenderSystem.VerticalAreaCornerSprite;
				if (flag7)
				{
					MapAtlasInfo.Instance.GetSprite(MapRenderSystem.VerticalAreaCornerSpriteName, delegate(Sprite sprite)
					{
						MapRenderSystem.VerticalAreaCornerSprite = sprite;
					});
				}
				bool flag8 = null == MapRenderSystem.HorizontalAreaCornerSprite;
				if (flag8)
				{
					MapAtlasInfo.Instance.GetSprite(MapRenderSystem.HorizontalAreaCornerSpriteName, delegate(Sprite sprite)
					{
						MapRenderSystem.HorizontalAreaCornerSprite = sprite;
					});
				}
			}
			bool flag9 = null == MapRenderSystem.BlockBaseShadowSprite && this._mapLayerRawImages.ContainsKey(MapRenderSystem.EMapLayer.BlockBaseShadow);
			if (flag9)
			{
				MapAtlasInfo.Instance.GetSprite(MapRenderSystem.BlockBaseShadowSpriteName, delegate(Sprite sprite)
				{
					MapRenderSystem.BlockBaseShadowSprite = sprite;
				});
			}
			bool flag10 = null == MapRenderSystem.BlockPreparationSprite && this._mapLayerRawImages.ContainsKey(MapRenderSystem.EMapLayer.BlockPreparation);
			if (flag10)
			{
				MapAtlasInfo.Instance.GetSprite(MapRenderSystem.BlockPreparationSpriteName, delegate(Sprite sprite)
				{
					MapRenderSystem.BlockPreparationSprite = sprite;
				});
			}
		}

		// Token: 0x06005344 RID: 21316 RVA: 0x00268FAC File Offset: 0x002671AC
		private void UpdateLayerVertex()
		{
			MapRenderSystem.<>c__DisplayClass85_0 CS$<>8__locals1 = new MapRenderSystem.<>c__DisplayClass85_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.blockLayer = this._mapLayerRawImages[MapRenderSystem.EMapLayer.MapBlock];
			MapRawImage mapRawImage = CS$<>8__locals1.blockLayer;
			if (mapRawImage.VertexStream == null)
			{
				mapRawImage.VertexStream = new List<UIVertex>();
			}
			CS$<>8__locals1.blockLayer.VertexStream.Clear();
			bool flag = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.MapBlockShadow, out CS$<>8__locals1.blockShadowLayer);
			if (flag)
			{
				mapRawImage = CS$<>8__locals1.blockShadowLayer;
				if (mapRawImage.VertexStream == null)
				{
					mapRawImage.VertexStream = new List<UIVertex>();
				}
				CS$<>8__locals1.blockShadowLayer.VertexStream.Clear();
				CS$<>8__locals1.blockShadowLayer.SetTextureToIndex(0, MapRenderSystem.BlockShadowSprite.texture);
			}
			bool flag2 = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.AreaBorder, out CS$<>8__locals1.areaBorderLayer);
			if (flag2)
			{
				mapRawImage = CS$<>8__locals1.areaBorderLayer;
				if (mapRawImage.VertexStream == null)
				{
					mapRawImage.VertexStream = new List<UIVertex>();
				}
				CS$<>8__locals1.areaBorderLayer.VertexStream.Clear();
			}
			bool flag3 = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.BlockPreparation, out CS$<>8__locals1.blockPreparationLayer);
			if (flag3)
			{
				mapRawImage = CS$<>8__locals1.blockPreparationLayer;
				if (mapRawImage.VertexStream == null)
				{
					mapRawImage.VertexStream = new List<UIVertex>();
				}
				CS$<>8__locals1.blockPreparationLayer.VertexStream.Clear();
			}
			bool needBlockBaseAndBorder = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.BlockBaseAndBorder, out CS$<>8__locals1.blockBaseAndBorderLayer);
			bool flag4 = needBlockBaseAndBorder;
			if (flag4)
			{
				mapRawImage = CS$<>8__locals1.blockBaseAndBorderLayer;
				if (mapRawImage.VertexStream == null)
				{
					mapRawImage.VertexStream = new List<UIVertex>();
				}
				CS$<>8__locals1.blockBaseAndBorderLayer.VertexStream.Clear();
			}
			bool flag5 = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.BlockBaseShadow, out CS$<>8__locals1.blockBaseShadowLayer);
			if (flag5)
			{
				mapRawImage = CS$<>8__locals1.blockBaseShadowLayer;
				if (mapRawImage.VertexStream == null)
				{
					mapRawImage.VertexStream = new List<UIVertex>();
				}
				CS$<>8__locals1.blockBaseShadowLayer.VertexStream.Clear();
			}
			CS$<>8__locals1.blockBaseVertexStream = new List<UIVertex>();
			CS$<>8__locals1.areaBorderCornerVertexStream = new List<UIVertex>();
			this.IterateMapBlocksForRender(new Action<MapBlockRenderInfo>(CS$<>8__locals1.<UpdateLayerVertex>g__AppendBlock|0));
			bool flag6 = needBlockBaseAndBorder;
			if (flag6)
			{
				CS$<>8__locals1.blockBaseAndBorderLayer.VertexStream.AddRange(CS$<>8__locals1.areaBorderCornerVertexStream);
				CS$<>8__locals1.blockBaseAndBorderLayer.VertexStream.AddRange(CS$<>8__locals1.blockBaseVertexStream);
			}
		}

		// Token: 0x06005345 RID: 21317 RVA: 0x002691D8 File Offset: 0x002673D8
		private void IterateMapBlocksForRender(Action<MapBlockRenderInfo> action)
		{
			for (int i = this.CurSize - 1; i >= 0; i--)
			{
				for (int j = 0; j <= this.CurSize - i - 1; j++)
				{
					int x = i + j;
					int y = j;
					int index = x * this.CurSize + y;
					action(this.GetBlockRenderInfo(index));
				}
			}
			for (int k = 1; k < this.CurSize; k++)
			{
				for (int l = 0; l <= this.CurSize - k - 1; l++)
				{
					int x2 = l;
					int y2 = k + l;
					int index2 = x2 * this.CurSize + y2;
					action(this.GetBlockRenderInfo(index2));
				}
			}
		}

		// Token: 0x06005346 RID: 21318 RVA: 0x002692B0 File Offset: 0x002674B0
		private void RenderMap()
		{
			this.UpdateLayerVertex();
			foreach (KeyValuePair<MapRenderSystem.EMapLayer, MapRawImage> pair in this._mapLayerRawImages)
			{
				pair.Value.SetAllDirty();
			}
			this._cachedBoundary = Rect.zero;
		}

		// Token: 0x06005347 RID: 21319 RVA: 0x00269320 File Offset: 0x00267520
		private IEnumerator NoAnimationRefreshBlockCoroutine()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				yield return null;
				num = i + 1;
			}
			this._noAnimationRefreshBlockCoroutine = null;
			this.RenderMap();
			yield break;
		}

		// Token: 0x06005348 RID: 21320 RVA: 0x00269330 File Offset: 0x00267530
		private void DelayStartBlockRefresh()
		{
			bool flag = this._cacheCommandMap.Count <= 0;
			if (!flag)
			{
				List<MapBlockRefreshCommand> list = new List<MapBlockRefreshCommand>();
				list.AddRange(this._cacheCommandMap.Values);
				this._cacheCommandMap.Clear();
				int controlCode = list.GetHashCode();
				list.ForEach(delegate(MapBlockRefreshCommand e)
				{
					e.RelatedRenderInfoList[0].ControlCode = controlCode;
					e.RelatedRenderInfoList[0].SetBlockState(e.ToState);
					e.FromColor = e.RelatedRenderInfoList[0].GetCurrentVertexColor();
					bool flag2 = e.FromState == MapBlockRenderInfo.EMapBlockState.Empty || e.FromState == MapBlockRenderInfo.EMapBlockState.Invisible;
					if (flag2)
					{
						e.FromVertexPositions = e.RelatedRenderInfoList[0].GetBlockSpriteVertexPositions();
						this.SetBlockEffect(e.RelatedRenderInfoList[0]);
					}
				});
				DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
				{
					list.ForEach(delegate(MapBlockRefreshCommand e)
					{
						e.ApplyStateChangeProgress(stepValue, controlCode);
					});
					this.ApplyMapAnimationData();
				}).SetAutoKill(true).OnComplete(delegate
				{
					EasyPool.Free<List<MapBlockRefreshCommand>>(list);
					this._cachedBoundary = Rect.zero;
				});
			}
		}

		// Token: 0x06005349 RID: 21321 RVA: 0x002693F4 File Offset: 0x002675F4
		private void ApplyMapAnimationData()
		{
			MapRawImage mapBlockLayer;
			this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.MapBlock, out mapBlockLayer);
			MapRawImage mapBlockShadowLayer;
			this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.MapBlockShadow, out mapBlockShadowLayer);
			bool flag = null != mapBlockLayer;
			if (flag)
			{
				mapBlockLayer.VertexStream.Clear();
			}
			bool flag2 = null != mapBlockShadowLayer;
			if (flag2)
			{
				mapBlockShadowLayer.VertexStream.Clear();
			}
			this.IterateMapBlocksForRender(delegate(MapBlockRenderInfo blockRenderInfo)
			{
				bool flag5 = blockRenderInfo == null;
				if (!flag5)
				{
					UIVertex[] blockVertexArray = blockRenderInfo.GetBlockVertexData(false);
					bool flag6 = null != mapBlockLayer && blockVertexArray != null;
					if (flag6)
					{
						mapBlockLayer.VertexStream.AddRange(blockVertexArray);
					}
					UIVertex[] blockShadowVertexArray = blockRenderInfo.GetBlockShadowVertexData();
					bool flag7 = null != mapBlockShadowLayer && blockShadowVertexArray != null;
					if (flag7)
					{
						mapBlockShadowLayer.VertexStream.AddRange(blockShadowVertexArray);
					}
				}
			});
			bool flag3 = null != mapBlockLayer;
			if (flag3)
			{
				mapBlockLayer.SetVerticesDirty();
			}
			bool flag4 = null != mapBlockShadowLayer;
			if (flag4)
			{
				mapBlockShadowLayer.SetVerticesDirty();
			}
			this._cachedBoundary = Rect.zero;
		}

		// Token: 0x0600534A RID: 21322 RVA: 0x002694C8 File Offset: 0x002676C8
		private void UpdateBlockSprite(string spriteName, MapBlockRefreshCommand command, MapRawImage blockLayer)
		{
			string atlasName = MapAtlasInfo.Instance.GetAtlasNameBySpriteName(spriteName);
			int index = this._mapAtlasNameList.IndexOf(atlasName);
			bool flag = index < 0;
			if (flag)
			{
				index = this._mapAtlasNameList.Count;
				this._mapAtlasNameList.Add(atlasName);
			}
			MapAtlasInfo.Instance.GetSprite(spriteName, delegate(Sprite sprite)
			{
				blockLayer.SetTextureToIndex((sbyte)index, sprite.texture);
				command.RelatedRenderInfoList.ForEach(delegate(MapBlockRenderInfo e)
				{
					e.SetSprite(sprite, (sbyte)index);
				});
				command.Complete = true;
			});
		}

		// Token: 0x0600534B RID: 21323 RVA: 0x00269550 File Offset: 0x00267750
		private void UpdateBlockEffect(MapBlockRenderInfo e, string effectName)
		{
			if (e.BlockEffectList == null)
			{
				e.BlockEffectList = new List<MapBlockEffect>();
			}
			MapBlockEffect blockEffect = this._blockEffectPoolMap[effectName].GetObject().GetComponent<MapBlockEffect>();
			Canvas canvas = blockEffect.DownRoot.GetComponent<Canvas>();
			bool flag = null != canvas;
			if (flag)
			{
				Object.Destroy(canvas);
			}
			blockEffect.transform.SetParent(MapRenderSystem.BlockEffectRoot, false);
			blockEffect.EffectName = effectName;
			blockEffect.name = string.Format("({0}) {1}", e.BlockIndex, blockEffect.EffectName);
			blockEffect.gameObject.SetActive(true);
			blockEffect.ChangeVisibility(e.State == MapBlockRenderInfo.EMapBlockState.Visible);
			e.BlockEffectList.Add(blockEffect);
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x00269610 File Offset: 0x00267810
		private MapBlockRenderInfo.EMapBlockState GetBlockStateInAnyway(short blockId)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			Location location = new Location(mapModel.ShowingAreaId, blockId);
			MapBlockData blockData = mapModel.GetBlockData(new Location(mapModel.ShowingAreaId, blockId));
			bool flag = blockData == null;
			MapBlockRenderInfo.EMapBlockState result;
			if (flag)
			{
				result = MapBlockRenderInfo.EMapBlockState.Empty;
			}
			else
			{
				switch (this.RenderState)
				{
				case MapRenderSystem.EMapState.Normal:
				{
					bool flag2 = mapModel.IsLocationShouldInSight(location);
					if (flag2)
					{
						result = (mapModel.IsLocationInFlameRender(location) ? MapBlockRenderInfo.EMapBlockState.InFlame : MapBlockRenderInfo.EMapBlockState.InSight);
					}
					else
					{
						result = (blockData.Visible ? MapBlockRenderInfo.EMapBlockState.Visible : MapBlockRenderInfo.EMapBlockState.Invisible);
					}
					break;
				}
				case MapRenderSystem.EMapState.AdjustAdditionalLight:
				{
					bool flag3 = !blockData.Visible;
					if (flag3)
					{
						result = MapBlockRenderInfo.EMapBlockState.Invisible;
					}
					else
					{
						result = (this._specifyLocations.Contains(location) ? MapBlockRenderInfo.EMapBlockState.InSight : MapBlockRenderInfo.EMapBlockState.DarkVisible);
					}
					break;
				}
				case MapRenderSystem.EMapState.Negative:
				{
					bool flag4 = !blockData.Visible;
					if (flag4)
					{
						result = MapBlockRenderInfo.EMapBlockState.Invisible;
					}
					else
					{
						result = (this._specifyLocations.Contains(location) ? MapBlockRenderInfo.EMapBlockState.InSight : MapBlockRenderInfo.EMapBlockState.NegativeFilm);
					}
					break;
				}
				default:
					PredefinedLog.Show(11, string.Format("GetBlockStateInAnyway at {0}", this.RenderState));
					result = (blockData.Visible ? MapBlockRenderInfo.EMapBlockState.Visible : MapBlockRenderInfo.EMapBlockState.Invisible);
					break;
				}
			}
			return result;
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x00269730 File Offset: 0x00267930
		private Vector2 GetEffectPosition(MapBlockRenderInfo blockRenderInfo)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = mapModel.GetBlockData(new Location(mapModel.ShowingAreaId, blockRenderInfo.BlockIndex));
			bool flag = blockData.GetConfig().Size == 2;
			if (flag)
			{
				MapBlockData rightBlockData = mapModel.GetNeighbor(blockData, MoveDirection.Right, true);
				bool flag2 = rightBlockData == null;
				if (flag2)
				{
					return Vector2.zero;
				}
				MapBlockRenderInfo rightBlockRenderInfo = this.GetBlockRenderInfo((int)rightBlockData.BlockId);
				bool flag3 = rightBlockRenderInfo != null;
				if (flag3)
				{
					return rightBlockRenderInfo.GetBlockCenter() + 0.5f * MapRenderSystem.BlockBaseHeight * Vector2.up;
				}
			}
			bool flag4 = blockData.GetConfig().Size == 3;
			if (flag4)
			{
				MapBlockData rightBlockData2 = mapModel.GetNeighbor(blockData, MoveDirection.Right, true);
				MapBlockData centerBlockData = mapModel.GetNeighbor(rightBlockData2, MoveDirection.Up, true);
				MapBlockRenderInfo centerBlockRenderInfo = this.GetBlockRenderInfo((int)centerBlockData.BlockId);
				bool flag5 = centerBlockRenderInfo != null;
				if (flag5)
				{
					return centerBlockRenderInfo.GetBlockCenter();
				}
			}
			return blockRenderInfo.GetBlockCenter();
		}

		// Token: 0x0600534E RID: 21326 RVA: 0x0026982C File Offset: 0x00267A2C
		private void SetBlockEffect(MapBlockRenderInfo blockRenderInfo)
		{
			bool flag = blockRenderInfo.BlockEffectList != null && blockRenderInfo.State != MapBlockRenderInfo.EMapBlockState.Invisible;
			if (flag)
			{
				MapRawImage blockLayer = this._mapLayerRawImages[MapRenderSystem.EMapLayer.MapBlock];
				Transform shadowLayerTransform = null;
				MapRawImage shadowLayer;
				bool flag2 = this._mapLayerRawImages.TryGetValue(MapRenderSystem.EMapLayer.MapBlockShadow, out shadowLayer);
				if (flag2)
				{
					shadowLayerTransform = shadowLayer.transform;
				}
				foreach (MapBlockEffect blockEffect in blockRenderInfo.BlockEffectList)
				{
					blockEffect.gameObject.SetActive(true);
					blockEffect.ChangeVisibility(true);
					blockEffect.CurrentState = this.GetBlockEffectState(blockRenderInfo.BlockIndex);
					bool flag3 = null != blockEffect.TopRoot;
					if (flag3)
					{
						blockEffect.TopRoot.SetParent(blockLayer.transform, false);
						blockEffect.TopRoot.localPosition = this.GetEffectPosition(blockRenderInfo);
					}
					bool flag4 = null != shadowLayerTransform && null != blockEffect.DownRoot;
					if (flag4)
					{
						blockEffect.DownRoot.SetParent(shadowLayerTransform, false);
						blockEffect.DownRoot.localPosition = this.GetEffectPosition(blockRenderInfo);
					}
				}
			}
		}

		// Token: 0x0600534F RID: 21327 RVA: 0x00269988 File Offset: 0x00267B88
		private MapBlockEffect.DisplayState GetBlockEffectState(short blockId)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			Location location = new Location(mapModel.ShowingAreaId, blockId);
			MapBlockData blockData = mapModel.GetBlockData(location);
			HashSet<short> relatedBlockIds;
			mapModel.BlockGroupDict.TryGetValue(new Location(mapModel.ShowingAreaId, blockData.RootBlockId), out relatedBlockIds);
			bool flag = mapModel.IsLocationInFlameRender(location);
			MapBlockEffect.DisplayState result;
			if (flag)
			{
				result = MapBlockEffect.DisplayState.Flame;
			}
			else
			{
				bool flag2 = blockData.GetRootBlock().BlockId == mapModel.CurrentBlockData.GetRootBlock().BlockId;
				if (flag2)
				{
					result = MapBlockEffect.DisplayState.Light;
				}
				else
				{
					bool flag3 = mapModel.IsLocationShouldInSight(location);
					if (flag3)
					{
						result = MapBlockEffect.DisplayState.Half;
					}
					else
					{
						bool flag4 = relatedBlockIds == null;
						if (flag4)
						{
							result = MapBlockEffect.DisplayState.Dark;
						}
						else
						{
							foreach (short relatedBlockId in relatedBlockIds)
							{
								bool flag5 = mapModel.IsLocationShouldInSight(new Location(mapModel.ShowingAreaId, relatedBlockId));
								if (flag5)
								{
									return MapBlockEffect.DisplayState.Half;
								}
							}
							result = MapBlockEffect.DisplayState.Dark;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005350 RID: 21328 RVA: 0x00269A94 File Offset: 0x00267C94
		private List<string> GetMapBlockEffectNames(MapBlockData blockData)
		{
			List<string> result = new List<string>();
			string[] effectConfig = blockData.GetConfig().Effect;
			string effectName = (effectConfig != null && effectConfig.Length != 0) ? effectConfig[(int)blockData.BlockId % effectConfig.Length] : null;
			bool flag = !string.IsNullOrEmpty(effectName);
			if (flag)
			{
				result.Add(effectName);
			}
			bool flag2 = MapRenderSystem.GetAdditionEffectInfo == null || !blockData.IsCityTown();
			List<string> result2;
			if (flag2)
			{
				result2 = result;
			}
			else
			{
				Location location = blockData.GetLocation();
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				string additionalEffectName = blockData.GetConfig().Size.ToString();
				List<Location> alterSettlementLocations = mapModel.AlterSettlementLocations;
				bool flag3 = alterSettlementLocations != null && alterSettlementLocations.Contains(location);
				if (flag3)
				{
					additionalEffectName += "_alter";
				}
				GameObject obj = MapRenderSystem.GetAdditionEffectInfo().GetTargetGameObject(additionalEffectName);
				bool flag4 = obj == null;
				if (flag4)
				{
					result2 = result;
				}
				else
				{
					MapBlockEffect blockEffect = obj.GetComponent<MapBlockEffect>();
					bool flag5 = null != blockEffect && !this._blockEffectPoolMap.ContainsKey(blockEffect.name);
					if (flag5)
					{
						blockEffect.EffectName = blockEffect.name;
						this._blockEffectPoolMap.Add(blockEffect.name, new PoolItem(blockEffect.name, blockEffect.gameObject));
					}
					result.Add(obj.name);
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x06005351 RID: 21329 RVA: 0x00269BED File Offset: 0x00267DED
		[Obsolete("This method is obsolete, and will removed in future, use RefreshAllMapBlocks instead.")]
		public void RefreshVisibilityByTaiwuLocation()
		{
			this.RefreshAllMapBlocks(true);
		}

		// Token: 0x06005352 RID: 21330 RVA: 0x00269BF7 File Offset: 0x00267DF7
		[Obsolete("This method is obsolete, and will removed in future, use RefreshAllMapBlocks instead.")]
		public void RefreshVisibilityByTaiwuLocation(bool isAnim)
		{
			this.RefreshAllMapBlocks(isAnim);
		}

		// Token: 0x06005356 RID: 21334 RVA: 0x00269E4B File Offset: 0x0026804B
		[CompilerGenerated]
		private void <AdjustMapBlockAdditionalLightState>g__AddByteCoordinate|58_0(ByteCoordinate coordinate, ref MapRenderSystem.<>c__DisplayClass58_0 A_2)
		{
			this._specifyLocations.Add(new Location(A_2.taiwuLocation.AreaId, WorldMapModel.CoordinateToIndex(coordinate, A_2.mapSize)));
		}

		// Token: 0x06005357 RID: 21335 RVA: 0x00269E75 File Offset: 0x00268075
		[CompilerGenerated]
		private void <AdjustMapBlockAdditionalLightState>g__AddSpecifyXy|58_1(int x, int y, ref MapRenderSystem.<>c__DisplayClass58_0 A_3)
		{
			this.<AdjustMapBlockAdditionalLightState>g__AddByteCoordinate|58_0(new ByteCoordinate((byte)x, (byte)y), ref A_3);
		}

		// Token: 0x04003847 RID: 14407
		public static readonly Vector2Int BlockRenderSpace = new Vector2Int(640, 458);

		// Token: 0x04003848 RID: 14408
		public static readonly float BlockBaseHeight = 332f;

		// Token: 0x04003849 RID: 14409
		public static Vector2 MapBlockPosInterval = new Vector2((float)MapRenderSystem.BlockRenderSpace.x, MapRenderSystem.BlockBaseHeight) * 0.5f;

		// Token: 0x0400384A RID: 14410
		public static readonly Color NormalEdgeColor = new Color(0.19607843f, 0.24313726f, 0.28627452f, 1f);

		// Token: 0x0400384B RID: 14411
		private const float BlockAnimationDuration = 0.3f;

		// Token: 0x0400384C RID: 14412
		public const int MaxTextureCount = 7;

		// Token: 0x0400384D RID: 14413
		private static readonly int MultiTextureAmount = Shader.PropertyToID("_MultiTextureAmount");

		// Token: 0x0400384E RID: 14414
		public static readonly Dictionary<Vector2Int, int> MapBlockOffsetMulti2X2 = new Dictionary<Vector2Int, int>
		{
			{
				new Vector2Int(0, 0),
				1
			},
			{
				new Vector2Int(1, 0),
				2
			},
			{
				new Vector2Int(0, 1),
				3
			},
			{
				new Vector2Int(1, 1),
				4
			}
		};

		// Token: 0x0400384F RID: 14415
		public static readonly Dictionary<Vector2Int, int> MapBlockOffsetMulti3X3 = new Dictionary<Vector2Int, int>
		{
			{
				new Vector2Int(0, 0),
				1
			},
			{
				new Vector2Int(1, 0),
				2
			},
			{
				new Vector2Int(2, 0),
				3
			},
			{
				new Vector2Int(0, 1),
				4
			},
			{
				new Vector2Int(1, 1),
				5
			},
			{
				new Vector2Int(2, 1),
				6
			},
			{
				new Vector2Int(0, 2),
				7
			},
			{
				new Vector2Int(1, 2),
				8
			},
			{
				new Vector2Int(2, 2),
				9
			}
		};

		// Token: 0x04003850 RID: 14416
		private static readonly string BlockBaseSpriteName = "map_block_special_base_1";

		// Token: 0x04003851 RID: 14417
		private static readonly string BlockShadowSpriteName = "map_block_special_shadow_1";

		// Token: 0x04003852 RID: 14418
		private static readonly string BlockBaseShadowSpriteName = "map_block_special_shadow_2";

		// Token: 0x04003853 RID: 14419
		private static readonly string DownAreaEdgeSpriteName = "map_block_special_table_0";

		// Token: 0x04003854 RID: 14420
		private static readonly string AreaCornerTableSpriteName = "map_block_special_table_1";

		// Token: 0x04003855 RID: 14421
		private static readonly string UpAreaEdgeSpriteName = "map_block_special_table_2";

		// Token: 0x04003856 RID: 14422
		private static readonly string VerticalAreaCornerSpriteName = "map_block_special_drop_1";

		// Token: 0x04003857 RID: 14423
		private static readonly string HorizontalAreaCornerSpriteName = "map_block_special_drop_2";

		// Token: 0x04003858 RID: 14424
		private static readonly string BlockPreparationSpriteName = "map_block_special_preparation_1";

		// Token: 0x04003859 RID: 14425
		public static readonly Vector2[] AreaOffset = new Vector2[9];

		// Token: 0x0400385A RID: 14426
		public static Sprite BlockShadowSprite;

		// Token: 0x0400385B RID: 14427
		public static Sprite BlockBaseSprite;

		// Token: 0x0400385C RID: 14428
		public static Sprite BlockBaseShadowSprite;

		// Token: 0x0400385D RID: 14429
		public static Sprite DownAreaEdgeSprite;

		// Token: 0x0400385E RID: 14430
		public static Sprite UpAreaEdgeSprite;

		// Token: 0x0400385F RID: 14431
		public static Sprite AreaTableCornerSprite;

		// Token: 0x04003860 RID: 14432
		public static Sprite VerticalAreaCornerSprite;

		// Token: 0x04003861 RID: 14433
		public static Sprite HorizontalAreaCornerSprite;

		// Token: 0x04003862 RID: 14434
		public static Sprite BlockPreparationSprite;

		// Token: 0x04003863 RID: 14435
		public static Color EdgeColor;

		// Token: 0x04003864 RID: 14436
		public static Transform BlockEffectRoot;

		// Token: 0x04003865 RID: 14437
		public static Func<WorldMapAdditionalEffectsInfo> GetAdditionEffectInfo;

		// Token: 0x04003866 RID: 14438
		public static AnimationCurve BlockAppearAnimCurve;

		// Token: 0x04003868 RID: 14440
		private Dictionary<int, MapBlockRenderInfo> _blockRenderInfosInUse = new Dictionary<int, MapBlockRenderInfo>();

		// Token: 0x04003869 RID: 14441
		private LocalObjectPool<MapBlockRenderInfo> _blockRenderInfoPool = new LocalObjectPool<MapBlockRenderInfo>(400, 900);

		// Token: 0x0400386A RID: 14442
		private Dictionary<short, MapBlockRefreshCommand> _cacheCommandMap = new Dictionary<short, MapBlockRefreshCommand>();

		// Token: 0x0400386B RID: 14443
		private List<string> _mapAtlasNameList = new List<string>();

		// Token: 0x0400386C RID: 14444
		private Dictionary<string, PoolItem> _blockEffectPoolMap = new Dictionary<string, PoolItem>();

		// Token: 0x0400386D RID: 14445
		private Dictionary<MapRenderSystem.EMapLayer, MapRawImage> _mapLayerRawImages;

		// Token: 0x0400386E RID: 14446
		private Coroutine _noAnimationRefreshBlockCoroutine;

		// Token: 0x04003870 RID: 14448
		private readonly List<Location> _specifyLocations = new List<Location>();

		// Token: 0x04003871 RID: 14449
		private Rect _cachedBoundary;

		// Token: 0x02001B0A RID: 6922
		public enum EMapLayer
		{
			// Token: 0x0400B7DA RID: 47066
			MapBlock,
			// Token: 0x0400B7DB RID: 47067
			MapBlockShadow,
			// Token: 0x0400B7DC RID: 47068
			BlockPreparation,
			// Token: 0x0400B7DD RID: 47069
			AreaBorder,
			// Token: 0x0400B7DE RID: 47070
			BlockBaseAndBorder,
			// Token: 0x0400B7DF RID: 47071
			BlockBaseShadow
		}

		// Token: 0x02001B0B RID: 6923
		public enum EMapState
		{
			// Token: 0x0400B7E1 RID: 47073
			Normal,
			// Token: 0x0400B7E2 RID: 47074
			AdjustAdditionalLight,
			// Token: 0x0400B7E3 RID: 47075
			Negative
		}
	}
}
