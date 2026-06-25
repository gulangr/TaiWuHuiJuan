using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Map;
using GameData.Utilities;
using Map.RenderSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Map
{
	// Token: 0x0200093F RID: 2367
	public class MapCamera : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06006E66 RID: 28262 RVA: 0x0033092C File Offset: 0x0032EB2C
		private void OnBeginDrag()
		{
			bool elastic = this.Elastic;
			if (elastic)
			{
				bool flag = this._dragTweener != null;
				if (flag)
				{
					this._dragTweener.Kill(false);
					this._dragTweener = null;
				}
			}
			this._prevMousePos = UIManager.Instance.MousePosToLocalPos(this.MoveAndScaleTarget.parent as RectTransform);
		}

		// Token: 0x06006E67 RID: 28263 RVA: 0x0033098C File Offset: 0x0032EB8C
		private void OnDrag()
		{
			Vector2 nowMousePos = UIManager.Instance.MousePosToLocalPos(this.MoveAndScaleTarget.parent as RectTransform);
			Vector2 offset = nowMousePos - this._prevMousePos;
			float scale = this.MoveAndScaleTarget.localScale.x;
			this._cameraPos.x = this._cameraPos.x - offset.x / scale;
			this._cameraPos.y = this._cameraPos.y - offset.y / scale;
			this.SetCameraPos(this._cameraPos);
			this._prevMousePos = nowMousePos;
		}

		// Token: 0x06006E68 RID: 28264 RVA: 0x00330A18 File Offset: 0x0032EC18
		private void OnEndDrag()
		{
			int currAreaIndex = this.MapModel.GetAreaIndexInState(this.MapModel.ShowingAreaId);
			bool flag = this.GetInsideAreaIndex() != currAreaIndex;
			if (flag)
			{
				Vector2 currPos = this._cameraPos;
				Vector2 deltaPos = this.GetClosestValidPos() - this._cameraPos;
				this._dragTweener = DOVirtual.Float(0f, 1f, 0.5f, delegate(float val)
				{
					this.SetCameraPos(currPos + deltaPos * val);
				}).SetAutoKill(true);
			}
			else
			{
				bool elastic = this.Elastic;
				if (elastic)
				{
					float xSpeed = Input.GetAxis("Mouse X") * this.SpeedFactor;
					float ySpeed = Input.GetAxis("Mouse Y") * this.SpeedFactor;
					this._dragTweener = DOVirtual.Float(1f, 0f, this.ElasticDuration, delegate(float val)
					{
						xSpeed *= val;
						ySpeed *= val;
						MapCamera <>4__this = this;
						<>4__this._cameraPos.x = <>4__this._cameraPos.x - xSpeed;
						MapCamera <>4__this2 = this;
						<>4__this2._cameraPos.y = <>4__this2._cameraPos.y - ySpeed;
						bool flag2 = this.GetInsideAreaIndex() != currAreaIndex;
						if (flag2)
						{
							this._dragTweener.Kill(false);
						}
						else
						{
							this.SetCameraPos(this._cameraPos);
						}
					}).SetAutoKill(true);
				}
			}
		}

		// Token: 0x06006E69 RID: 28265 RVA: 0x00330B3D File Offset: 0x0032ED3D
		private void SetCameraPos(Vector2 pos)
		{
			this._cameraPos = pos;
			this.MoveAndScaleTarget.anchoredPosition = -pos * this.MoveAndScaleTarget.localScale.x;
		}

		// Token: 0x06006E6A RID: 28266 RVA: 0x00330B70 File Offset: 0x0032ED70
		public void MoveCameraTo(Vector2 pos, bool doAnimation = false, TweenCallback onComplete = null)
		{
			bool flag = this._dragTweener != null;
			if (flag)
			{
				this._dragTweener.Kill(false);
				this._dragTweener = null;
			}
			bool flag2 = this._aniTweener != null;
			if (flag2)
			{
				this._aniTweener.Kill(false);
				this._aniTweener = null;
			}
			bool flag3 = doAnimation && Vector3.Distance(pos, this._cameraPos) > 0.1f;
			if (flag3)
			{
				Vector2 currPos = this._cameraPos;
				Vector2 deltaPos = pos - this._cameraPos;
				this._aniTweener = DOVirtual.Float(0f, 1f, 0.2f, delegate(float val)
				{
					this.SetCameraPos(currPos + deltaPos * val);
				}).OnComplete(onComplete).SetAutoKill(true);
			}
			else
			{
				this.SetCameraPos(pos);
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}

		// Token: 0x06006E6B RID: 28267 RVA: 0x00330C62 File Offset: 0x0032EE62
		public void StopDrag()
		{
			this._dragging = false;
		}

		// Token: 0x06006E6C RID: 28268 RVA: 0x00330C6C File Offset: 0x0032EE6C
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._pointerEntered = true;
		}

		// Token: 0x06006E6D RID: 28269 RVA: 0x00330C76 File Offset: 0x0032EE76
		public void OnPointerExit(PointerEventData eventData)
		{
			this._pointerEntered = false;
		}

		// Token: 0x06006E6E RID: 28270 RVA: 0x00330C80 File Offset: 0x0032EE80
		public void InitCameraMoveRange()
		{
			for (int i = 0; i < this.AreaCameraPolygons.Length; i++)
			{
				bool flag = this.AreaCameraPolygons[i] == null;
				if (flag)
				{
					this.AreaCameraPolygons[i] = new List<Vector2>();
				}
				bool flag2 = this.AreaBorderPolygons[i] == null;
				if (flag2)
				{
					this.AreaBorderPolygons[i] = new List<Vector2>();
				}
			}
			List<MapBlockData> areaBlocks = EasyPool.Get<List<MapBlockData>>();
			bool flag3 = this.MapModel.CurrentStateId >= 0;
			if (flag3)
			{
				for (int j = 0; j < 9; j++)
				{
					short areaId = this.MapModel.GetAreaIdByStateIndex(j);
					this.MapModel.GetAreaBlocks(areaId, areaBlocks);
					areaBlocks.RemoveAll((MapBlockData block) => !this.MapModel.IsEdgeBlock(new Location(areaId, block.BlockId)));
					this.InitAreaCameraPolygon(areaBlocks, this.AreaOffset[j], this.AreaCameraPolygons[j]);
					this.InitAreaBorderPolygon(areaBlocks, this.AreaOffset[j], this.AreaBorderPolygons[j]);
				}
			}
			else
			{
				this.MapModel.GetAreaBlocks(this.MapModel.CurrentAreaId, areaBlocks);
				areaBlocks.RemoveAll((MapBlockData block) => !this.MapModel.IsEdgeBlock(new Location(this.MapModel.CurrentAreaId, block.BlockId)));
				this.InitAreaCameraPolygon(areaBlocks, this.AreaOffset[0], this.AreaCameraPolygons[0]);
				this.InitAreaBorderPolygon(areaBlocks, this.AreaOffset[0], this.AreaBorderPolygons[0]);
				for (int k = 1; k < this.AreaCameraPolygons.Length; k++)
				{
					this.AreaCameraPolygons[k].Clear();
				}
				for (int l = 1; l < this.AreaBorderPolygons.Length; l++)
				{
					this.AreaBorderPolygons[l].Clear();
				}
			}
			EasyPool.Free<List<MapBlockData>>(areaBlocks);
		}

		// Token: 0x06006E6F RID: 28271 RVA: 0x00330E74 File Offset: 0x0032F074
		private void InitAreaCameraPolygon(List<MapBlockData> blockDataList, Vector2 offset, List<Vector2> polygon)
		{
			List<Vector2> blockPosList = EasyPool.Get<List<Vector2>>();
			int minXIndex = -1;
			int maxXIndex = -1;
			blockPosList.Clear();
			short blockId = 0;
			while ((int)blockId < blockDataList.Count)
			{
				MapBlockData blockData = blockDataList[(int)blockId];
				ByteCoordinate blockCoord = blockData.GetBlockPos();
				Vector2 pos = new Vector2((float)(blockCoord.X + blockCoord.Y) * MapRenderSystem.MapBlockPosInterval.x, (float)(blockCoord.Y - blockCoord.X) * MapRenderSystem.MapBlockPosInterval.y);
				MapBlockData leftNeighbor = this.MapModel.GetNeighbor(blockData, MoveDirection.Left, true);
				MapBlockData rightNeighbor = this.MapModel.GetNeighbor(blockData, MoveDirection.Right, true);
				MapBlockData topNeighbor = this.MapModel.GetNeighbor(blockData, MoveDirection.Up, true);
				MapBlockData bottomNeighbor = this.MapModel.GetNeighbor(blockData, MoveDirection.Down, true);
				bool flag = leftNeighbor == null && bottomNeighbor == null;
				if (flag)
				{
					pos.x -= MapRenderSystem.MapBlockPosInterval.x + 10f;
				}
				else
				{
					bool flag2 = rightNeighbor == null && topNeighbor == null;
					if (flag2)
					{
						pos.x += MapRenderSystem.MapBlockPosInterval.x + 10f;
					}
				}
				bool flag3 = pos.y > 0f;
				if (flag3)
				{
					pos.y += MapRenderSystem.MapBlockPosInterval.y * 2f + 10f;
				}
				else
				{
					pos.y -= 10f;
				}
				bool flag4 = minXIndex < 0 || pos.x < blockPosList[minXIndex].x;
				if (flag4)
				{
					minXIndex = blockPosList.Count;
				}
				bool flag5 = maxXIndex < 0 || pos.x > blockPosList[maxXIndex].x;
				if (flag5)
				{
					maxXIndex = blockPosList.Count;
				}
				blockPosList.Add(pos);
				blockId += 1;
			}
			polygon.Clear();
			this.QuickHull(blockPosList, blockPosList[minXIndex], blockPosList[maxXIndex], 1, offset, ref polygon);
			this.QuickHull(blockPosList, blockPosList[maxXIndex], blockPosList[minXIndex], 1, offset, ref polygon);
			EasyPool.Free<List<Vector2>>(blockPosList);
		}

		// Token: 0x06006E70 RID: 28272 RVA: 0x00331088 File Offset: 0x0032F288
		private void QuickHull(List<Vector2> pointList, Vector2 linePoint1, Vector2 linePoint2, int side, Vector2 offset, ref List<Vector2> polygon)
		{
			List<Vector2> correctSidePointList = EasyPool.Get<List<Vector2>>();
			int maxDistIndex = -1;
			float maxDist = 0f;
			Vector2 intersect = default(Vector2);
			correctSidePointList.Clear();
			for (int i = 0; i < pointList.Count; i++)
			{
				Vector2 point = pointList[i];
				bool flag = MapCamera.GetPointOnLineSide(point, linePoint1, linePoint2) == side;
				if (flag)
				{
					float dist = MapCamera.GetPointToLineDistance(point, linePoint1, linePoint2, ref intersect);
					bool flag2 = dist > maxDist;
					if (flag2)
					{
						maxDist = dist;
						maxDistIndex = i;
					}
					correctSidePointList.Add(point);
				}
			}
			bool flag3 = maxDistIndex == -1;
			if (flag3)
			{
				bool flag4 = !polygon.Contains(linePoint1);
				if (flag4)
				{
					polygon.Add(linePoint1 + offset);
				}
				bool flag5 = !polygon.Contains(linePoint2);
				if (flag5)
				{
					polygon.Add(linePoint2 + offset);
				}
				EasyPool.Free<List<Vector2>>(correctSidePointList);
			}
			else
			{
				this.QuickHull(correctSidePointList, linePoint1, pointList[maxDistIndex], side, offset, ref polygon);
				this.QuickHull(correctSidePointList, pointList[maxDistIndex], linePoint2, side, offset, ref polygon);
				EasyPool.Free<List<Vector2>>(correctSidePointList);
			}
		}

		// Token: 0x06006E71 RID: 28273 RVA: 0x003311A8 File Offset: 0x0032F3A8
		private void InitAreaBorderPolygon(List<MapBlockData> blockDataList, Vector2 offset, List<Vector2> polygon)
		{
			float offset2 = Mathf.Sqrt(450f);
			float offset3 = 30f / Mathf.Sin(0.482217f);
			float offset4 = 30f / Mathf.Sin(1.0885793f);
			MapBlockData currBlock = blockDataList[0];
			MapBlockData leftBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Left, true);
			MapBlockData rightBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Right, true);
			MapBlockData topBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Up, true);
			MapBlockData bottomBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Down, true);
			bool finalBlockSetted = false;
			bool flag = leftBlock == null;
			MoveDirection blockPointDirection;
			if (flag)
			{
				blockPointDirection = MoveDirection.Left;
			}
			else
			{
				bool flag2 = topBlock == null;
				if (flag2)
				{
					blockPointDirection = MoveDirection.Up;
				}
				else
				{
					bool flag3 = rightBlock == null;
					if (flag3)
					{
						blockPointDirection = MoveDirection.Right;
					}
					else
					{
						blockPointDirection = MoveDirection.Down;
					}
				}
			}
			polygon.Clear();
			do
			{
				ByteCoordinate blockPos = currBlock.GetBlockPos();
				Vector2 pos = new Vector2((float)(blockPos.X + blockPos.Y) * MapRenderSystem.MapBlockPosInterval.x, (float)(blockPos.Y - blockPos.X) * MapRenderSystem.MapBlockPosInterval.y);
				Vector2 borderPointOffset = Vector2.zero;
				bool flag4 = blockPointDirection == MoveDirection.Left;
				bool gotoNextBlock;
				if (flag4)
				{
					MapBlockData bottomLeftBlock = (leftBlock != null) ? this.MapModel.GetNeighbor(leftBlock, MoveDirection.Down, true) : ((bottomBlock != null) ? this.MapModel.GetNeighbor(bottomBlock, MoveDirection.Left, true) : null);
					pos.x -= MapRenderSystem.MapBlockPosInterval.x;
					pos.y += MapRenderSystem.MapBlockPosInterval.y;
					int neighborBlockCount = ((leftBlock != null) ? 1 : 0) + ((bottomBlock != null) ? 1 : 0) + ((bottomLeftBlock != null) ? 1 : 0);
					bool flag5 = neighborBlockCount == 0;
					if (flag5)
					{
						borderPointOffset.x = -offset3;
					}
					else
					{
						bool flag6 = neighborBlockCount == 1;
						if (flag6)
						{
							borderPointOffset.x = -offset2;
							borderPointOffset.y = ((leftBlock != null) ? (-offset2) : offset2);
						}
						else
						{
							borderPointOffset.y = ((leftBlock == null) ? offset4 : (-offset4));
						}
					}
					gotoNextBlock = (topBlock != null);
					bool flag7 = !gotoNextBlock;
					if (flag7)
					{
						blockPointDirection = MoveDirection.Up;
					}
				}
				else
				{
					bool flag8 = blockPointDirection == MoveDirection.Up;
					if (flag8)
					{
						MapBlockData topLeftBlock = (topBlock != null) ? this.MapModel.GetNeighbor(topBlock, MoveDirection.Left, true) : ((leftBlock != null) ? this.MapModel.GetNeighbor(leftBlock, MoveDirection.Up, true) : null);
						pos.y += MapRenderSystem.MapBlockPosInterval.y * 2f;
						int neighborBlockCount = ((topBlock != null) ? 1 : 0) + ((leftBlock != null) ? 1 : 0) + ((topLeftBlock != null) ? 1 : 0);
						bool flag9 = neighborBlockCount == 0;
						if (flag9)
						{
							borderPointOffset.y = offset4;
						}
						else
						{
							bool flag10 = neighborBlockCount == 1;
							if (flag10)
							{
								borderPointOffset.y = offset2;
								borderPointOffset.x = ((topBlock != null) ? (-offset2) : offset2);
							}
							else
							{
								borderPointOffset.x = ((topBlock == null) ? offset3 : (-offset3));
							}
						}
						gotoNextBlock = (rightBlock != null);
						bool flag11 = !gotoNextBlock;
						if (flag11)
						{
							blockPointDirection = MoveDirection.Right;
						}
					}
					else
					{
						bool flag12 = blockPointDirection == MoveDirection.Right;
						if (flag12)
						{
							MapBlockData topRightBlock = (rightBlock != null) ? this.MapModel.GetNeighbor(rightBlock, MoveDirection.Up, true) : ((topBlock != null) ? this.MapModel.GetNeighbor(topBlock, MoveDirection.Right, true) : null);
							pos.x += MapRenderSystem.MapBlockPosInterval.x;
							pos.y += MapRenderSystem.MapBlockPosInterval.y;
							int neighborBlockCount = ((rightBlock != null) ? 1 : 0) + ((topBlock != null) ? 1 : 0) + ((topRightBlock != null) ? 1 : 0);
							bool flag13 = neighborBlockCount == 0;
							if (flag13)
							{
								borderPointOffset.x = offset3;
							}
							else
							{
								bool flag14 = neighborBlockCount == 1;
								if (flag14)
								{
									borderPointOffset.x = offset2;
									borderPointOffset.y = ((rightBlock != null) ? offset2 : (-offset2));
								}
								else
								{
									borderPointOffset.y = ((rightBlock == null) ? (-offset4) : offset4);
								}
							}
							gotoNextBlock = (bottomBlock != null);
							bool flag15 = !gotoNextBlock;
							if (flag15)
							{
								blockPointDirection = MoveDirection.Down;
							}
						}
						else
						{
							MapBlockData bottomRightBlock = (bottomBlock != null) ? this.MapModel.GetNeighbor(bottomBlock, MoveDirection.Right, true) : ((rightBlock != null) ? this.MapModel.GetNeighbor(rightBlock, MoveDirection.Down, true) : null);
							int neighborBlockCount = ((bottomBlock != null) ? 1 : 0) + ((rightBlock != null) ? 1 : 0) + ((bottomRightBlock != null) ? 1 : 0);
							bool flag16 = neighborBlockCount == 0;
							if (flag16)
							{
								borderPointOffset.y = -offset4;
							}
							else
							{
								bool flag17 = neighborBlockCount == 1;
								if (flag17)
								{
									borderPointOffset.y = -offset2;
									borderPointOffset.x = ((bottomBlock != null) ? offset2 : (-offset2));
								}
								else
								{
									borderPointOffset.x = ((bottomBlock == null) ? (-offset3) : offset3);
								}
							}
							gotoNextBlock = (leftBlock != null);
							bool flag18 = !gotoNextBlock;
							if (flag18)
							{
								blockPointDirection = MoveDirection.Left;
							}
						}
					}
				}
				polygon.Add(offset + pos + borderPointOffset);
				bool flag19 = gotoNextBlock;
				if (flag19)
				{
					bool flag20 = blockPointDirection == MoveDirection.Left;
					if (flag20)
					{
						MapBlockData topLeftBlock2 = this.MapModel.GetNeighbor(topBlock, MoveDirection.Left, true);
						currBlock = ((topLeftBlock2 != null) ? topLeftBlock2 : topBlock);
						blockPointDirection = ((topLeftBlock2 != null) ? MoveDirection.Down : MoveDirection.Left);
					}
					else
					{
						bool flag21 = blockPointDirection == MoveDirection.Up;
						if (flag21)
						{
							MapBlockData topRightBlock2 = this.MapModel.GetNeighbor(rightBlock, MoveDirection.Up, true);
							currBlock = ((topRightBlock2 != null) ? topRightBlock2 : rightBlock);
							blockPointDirection = ((topRightBlock2 != null) ? MoveDirection.Left : MoveDirection.Up);
						}
						else
						{
							bool flag22 = blockPointDirection == MoveDirection.Right;
							if (flag22)
							{
								MapBlockData bottomRightBlock2 = this.MapModel.GetNeighbor(bottomBlock, MoveDirection.Right, true);
								currBlock = ((bottomRightBlock2 != null) ? bottomRightBlock2 : bottomBlock);
								blockPointDirection = ((bottomRightBlock2 != null) ? MoveDirection.Up : MoveDirection.Right);
							}
							else
							{
								bool flag23 = blockPointDirection == MoveDirection.Down;
								if (flag23)
								{
									MapBlockData bottomLeftBlock2 = this.MapModel.GetNeighbor(leftBlock, MoveDirection.Down, true);
									currBlock = ((bottomLeftBlock2 != null) ? bottomLeftBlock2 : leftBlock);
									blockPointDirection = ((bottomLeftBlock2 != null) ? MoveDirection.Right : MoveDirection.Down);
								}
							}
						}
					}
					blockDataList.Remove(currBlock);
					bool flag24 = !finalBlockSetted;
					if (flag24)
					{
						blockDataList.Add(currBlock);
						finalBlockSetted = true;
					}
					bool flag25 = blockDataList.Count > 0;
					if (flag25)
					{
						leftBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Left, true);
						rightBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Right, true);
						topBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Up, true);
						bottomBlock = this.MapModel.GetNeighbor(currBlock, MoveDirection.Down, true);
					}
				}
			}
			while (blockDataList.Count > 0);
		}

		// Token: 0x06006E72 RID: 28274 RVA: 0x003317A0 File Offset: 0x0032F9A0
		private int GetInsideAreaIndex()
		{
			for (int i = 0; i < this.AreaCameraPolygons.Length; i++)
			{
				bool flag = this.MapModel.CurrentStateId < 0 && i > 0;
				if (flag)
				{
					break;
				}
				bool flag2 = MapCamera.IsPointInsidePolygon(this._cameraPos, this.AreaCameraPolygons[i]);
				if (flag2)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06006E73 RID: 28275 RVA: 0x00331804 File Offset: 0x0032FA04
		private Vector2 GetClosestValidPos()
		{
			Vector2 intersectPoint = this._cameraPos;
			float minDistance = float.MaxValue;
			Vector2 intersect = Vector2.zero;
			List<Vector2> polygon = this.AreaCameraPolygons[this.MapModel.GetAreaIndexInState(this.MapModel.ShowingAreaId)];
			for (int i = 1; i < polygon.Count; i++)
			{
				float distance = MapCamera.GetPointToLineDistance(this._cameraPos, polygon[i - 1], polygon[i], ref intersect);
				bool flag = distance < minDistance;
				if (flag)
				{
					minDistance = distance;
					intersectPoint.x = intersect.x;
					intersectPoint.y = intersect.y;
				}
			}
			return intersectPoint;
		}

		// Token: 0x06006E74 RID: 28276 RVA: 0x003318B4 File Offset: 0x0032FAB4
		private static int GetPointOnLineSide(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
		{
			float value = (point.y - linePoint1.y) * (linePoint2.x - linePoint1.x) - (linePoint2.y - linePoint1.y) * (point.x - linePoint1.x);
			return (value > 0f) ? 1 : ((value < 0f) ? -1 : 0);
		}

		// Token: 0x06006E75 RID: 28277 RVA: 0x00331918 File Offset: 0x0032FB18
		private static float GetPointToLineDistance(Vector2 point, Vector2 linePoint1, Vector2 linePoint2, ref Vector2 intersectPoint)
		{
			bool flag = linePoint1 == linePoint2;
			float result;
			if (flag)
			{
				intersectPoint.x = linePoint1.x;
				intersectPoint.y = linePoint1.y;
				result = Vector2.Distance(point, linePoint1);
			}
			else
			{
				float dx = linePoint2.x - linePoint1.x;
				float dy = linePoint2.y - linePoint1.y;
				float t = ((point.x - linePoint1.x) * dx + (point.y - linePoint1.y) * dy) / (dx * dx + dy * dy);
				bool flag2 = t < 0f;
				Vector2 intersect;
				if (flag2)
				{
					intersect = linePoint1;
				}
				else
				{
					bool flag3 = t > 1f;
					if (flag3)
					{
						intersect = linePoint2;
					}
					else
					{
						intersect = new Vector2(linePoint1.x + t * dx, linePoint1.y + t * dy);
					}
				}
				intersectPoint.x = intersect.x;
				intersectPoint.y = intersect.y;
				result = Vector2.Distance(point, intersectPoint);
			}
			return result;
		}

		// Token: 0x06006E76 RID: 28278 RVA: 0x00331A08 File Offset: 0x0032FC08
		private static bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
		{
			bool result = false;
			int count = polygon.Count;
			for (int i = 0; i < count; i++)
			{
				Vector2 linePoint = polygon[i];
				Vector2 linePoint2 = polygon[(i + 1) % count];
				bool flag = ((linePoint2.y < point.y && linePoint.y >= point.y) || (linePoint.y < point.y && linePoint2.y >= point.y)) && linePoint2.x + (point.y - linePoint2.y) / (linePoint.y - linePoint2.y) * (linePoint.x - linePoint2.x) < point.x;
				if (flag)
				{
					result = !result;
				}
			}
			return result;
		}

		// Token: 0x06006E77 RID: 28279 RVA: 0x00331ADC File Offset: 0x0032FCDC
		private void LateUpdate()
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.WorldMap);
			if (!flag)
			{
				bool dragging = this._dragging;
				if (dragging)
				{
					bool key = Input.GetKey(KeyCode.Mouse0);
					if (key)
					{
						this.OnDrag();
					}
					else
					{
						this._dragging = false;
						this.OnEndDrag();
					}
				}
				bool flag2 = !this._pointerEntered && !ViewWorldMap.PointerOnAnyMapBlock;
				if (!flag2)
				{
					float scrollValue = Input.GetAxis("Mouse ScrollWheel");
					bool flag3 = Math.Abs(scrollValue) > 0f;
					if (flag3)
					{
						float scaleValue = Mathf.Clamp(scrollValue * this.ScaleFactor + this.MoveAndScaleTarget.localScale.x, this.ScaleRange.x, this.ScaleRange.y);
						bool flag4 = scrollValue < 0f && this.MoveAndScaleTarget.localScale.x <= this.ScaleRange.x && SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(3);
						if (flag4)
						{
							ConchShipCursor.Instance.AddWheelProgress(-scrollValue * 2f);
						}
						else
						{
							bool flag5 = scrollValue > 0f;
							if (flag5)
							{
								ConchShipCursor.Instance.AddWheelProgress(-1f);
							}
						}
						this.MoveAndScaleTarget.localScale = scaleValue * Vector3.one;
						this.SetCameraPos(this._cameraPos);
						Action<float> onScaleEvent = this.OnScaleEvent;
						if (onScaleEvent != null)
						{
							onScaleEvent(scaleValue);
						}
					}
					bool keyDown = Input.GetKeyDown(KeyCode.Mouse0);
					if (keyDown)
					{
						this._dragging = true;
						this.OnBeginDrag();
					}
				}
			}
		}

		// Token: 0x04005221 RID: 21025
		public Action<float> OnScaleEvent;

		// Token: 0x04005222 RID: 21026
		public WorldMapModel MapModel;

		// Token: 0x04005223 RID: 21027
		[HideInInspector]
		public RectTransform MoveAndScaleTarget;

		// Token: 0x04005224 RID: 21028
		[HideInInspector]
		public Vector2[] AreaOffset = new Vector2[9];

		// Token: 0x04005225 RID: 21029
		[HideInInspector]
		public List<Vector2>[] AreaCameraPolygons = new List<Vector2>[9];

		// Token: 0x04005226 RID: 21030
		[HideInInspector]
		public List<Vector2>[] AreaBorderPolygons = new List<Vector2>[9];

		// Token: 0x04005227 RID: 21031
		[Tooltip("是否缓动")]
		public bool Elastic;

		// Token: 0x04005228 RID: 21032
		[Tooltip("缓动速度参数")]
		[Range(0f, 1f)]
		public float SpeedFactor;

		// Token: 0x04005229 RID: 21033
		[Tooltip("缓动持续时间")]
		public float ElasticDuration = 2.5f;

		// Token: 0x0400522A RID: 21034
		public Vector2 ScaleRange;

		// Token: 0x0400522B RID: 21035
		public float ScaleFactor;

		// Token: 0x0400522C RID: 21036
		private Vector2 _cameraPos;

		// Token: 0x0400522D RID: 21037
		private Tweener _dragTweener;

		// Token: 0x0400522E RID: 21038
		private Tweener _aniTweener;

		// Token: 0x0400522F RID: 21039
		private Vector2 _prevMousePos;

		// Token: 0x04005230 RID: 21040
		private bool _dragging;

		// Token: 0x04005231 RID: 21041
		private bool _pointerEntered;
	}
}
