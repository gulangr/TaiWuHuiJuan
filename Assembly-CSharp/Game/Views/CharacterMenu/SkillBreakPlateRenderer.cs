using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.Components.EffectPlayer;
using Game.Views.SkillBreak;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B92 RID: 2962
	public class SkillBreakPlateRenderer : MonoBehaviour
	{
		// Token: 0x17000FCC RID: 4044
		// (get) Token: 0x06009209 RID: 37385 RVA: 0x0043FED0 File Offset: 0x0043E0D0
		private RectTransform rectTransform
		{
			get
			{
				return base.GetComponent<RectTransform>();
			}
		}

		// Token: 0x17000FCD RID: 4045
		// (get) Token: 0x0600920A RID: 37386 RVA: 0x0043FED8 File Offset: 0x0043E0D8
		// (set) Token: 0x0600920B RID: 37387 RVA: 0x0043FEE0 File Offset: 0x0043E0E0
		public GameData.Domains.Taiwu.SkillBreakPlate DisplayPlate { get; private set; }

		// Token: 0x0600920C RID: 37388 RVA: 0x0043FEE9 File Offset: 0x0043E0E9
		private void Awake()
		{
			this.InitRefers();
			this.SetLineFlowImageVisible(false);
		}

		// Token: 0x0600920D RID: 37389 RVA: 0x0043FEFC File Offset: 0x0043E0FC
		private void OnEnable()
		{
			this._isEnable = true;
			bool flag = this.linesFlowImage.gameObject.activeSelf && this.linesFlowImage.texture;
			if (flag)
			{
				this.RefreshLineFlowImageColor();
			}
		}

		// Token: 0x0600920E RID: 37390 RVA: 0x0043FF44 File Offset: 0x0043E144
		private void OnDestroy()
		{
			bool flag = this.linesFlowImage.texture;
			if (flag)
			{
				Object.Destroy(this.linesFlowImage.texture);
			}
		}

		// Token: 0x0600920F RID: 37391 RVA: 0x0043FF7C File Offset: 0x0043E17C
		public void QuickHide()
		{
			for (int i = 0; i < this.normalPathRoot.childCount; i++)
			{
				Transform child = this.normalPathRoot.GetChild(i);
				child.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009210 RID: 37392 RVA: 0x0043FFC0 File Offset: 0x0043E1C0
		public IEnumerator RefreshOneShotParticlesCo(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			this.PlayVisibleEffect(plate, lastPlate);
			yield return new WaitForEndOfFrame();
			this.PlaySelectedEffect(plate, lastPlate);
			yield return new WaitForEndOfFrame();
			this.PlayPowerChangeEffect(plate, lastPlate);
			yield return new WaitForEndOfFrame();
			this.PlayBonusFillEffect(plate, lastPlate);
			yield return new WaitForEndOfFrame();
			this.PlayBreakFailEffect(plate, lastPlate);
			yield return new WaitForEndOfFrame();
			this.PlayGridTypeChangeEffect(plate, lastPlate);
			yield return new WaitForEndOfFrame();
			yield break;
		}

		// Token: 0x06009211 RID: 37393 RVA: 0x0043FFE0 File Offset: 0x0043E1E0
		public void Refresh(GameData.Domains.Taiwu.SkillBreakPlate plate, short skillId, LifeSkillShorts lifeSkillAttainments, int currentExp, int baseNeedExp, IAsyncMethodRequestHandler requestHandler = null, GameData.Domains.Taiwu.SkillBreakPlate oldPlate = null, Action OnRefreshNodeFinish = null)
		{
			this.InitRefers();
			this.DisplayPlate = plate;
			this._oldPlate = oldPlate;
			this._triangleGrid = new SkillBreakTriangleGrid((int)plate.Width, (int)plate.Height, this.triangleBase, this.triangleHeight);
			this._requestHandler = requestHandler;
			this._skillId = skillId;
			this._currentExp = currentExp;
			this._baseNeedExp = baseNeedExp;
			this._lifeSkillAttainments = lifeSkillAttainments;
			this.rectTransform.SetSize(this._triangleGrid.AreaSize);
			this.RefreshLines(skillId);
			this.RefreshSelectedPath();
			this.RefreshCanSelectPath();
			this.RefreshFailedPath();
			this.RefreshLoopParticles(plate);
			this.RefreshNodes(OnRefreshNodeFinish);
			this.RefreshRanges();
			base.StartCoroutine(this.OpenUIPlayEffect());
		}

		// Token: 0x06009212 RID: 37394 RVA: 0x004400A8 File Offset: 0x0043E2A8
		public bool TryGetCellWrapper(SkillBreakPlateIndex coordinate, out SkillBreakPlateCellWrapper cell)
		{
			this.InitRefers();
			cell = null;
			bool flag = this.DisplayPlate == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.IsCoordinateValid(coordinate);
				if (flag2)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < this.cellRoot.childCount; i++)
					{
						SkillBreakPlateCellWrapper wrapper = this.cellRoot.GetChild(i).GetComponent<SkillBreakPlateCellWrapper>();
						bool flag3 = wrapper == null;
						if (!flag3)
						{
							bool flag4 = wrapper.CurrentCoordinate != coordinate;
							if (!flag4)
							{
								cell = wrapper;
								return true;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06009213 RID: 37395 RVA: 0x00440148 File Offset: 0x0043E348
		private void RefreshNodes(Action OnRefreshNodeFinish = null)
		{
			int cellCount = SkillBreakPlateRenderer.CalculateCellCount(this.DisplayPlate);
			CommonUtils.PrepareEnoughChildren(this.cellRoot.transform, this.cellTemplate.gameObject, cellCount, null);
			SkillBreakPlateGrid currentCellGrid = null;
			bool flag = this.DisplayPlate.Current != SkillBreakPlateIndex.Invalid;
			if (flag)
			{
				currentCellGrid = this.DisplayPlate.GetGridAt(this.DisplayPlate.Current);
			}
			bool flag2 = this._refreshNodesCo != null;
			if (flag2)
			{
				base.StopCoroutine(this._refreshNodesCo);
			}
			this._refreshNodesCo = base.StartCoroutine(this.RefreshNodesCo(currentCellGrid, OnRefreshNodeFinish));
		}

		// Token: 0x06009214 RID: 37396 RVA: 0x004401EC File Offset: 0x0043E3EC
		private IEnumerator RefreshNodesCo(SkillBreakPlateGrid currentCellGrid, Action OnRefreshNodeFinish = null)
		{
			HashSet<SkillBreakPlateIndex> currentInvisibleSet = SkillBreakPlateRenderer.CollectInvisibleSet(this.DisplayPlate);
			bool shouldRefreshLinesFlow = SkillBreakPlateRenderer.HasNewInvisibleCells(currentInvisibleSet, this._lastInvisibleSet);
			this._bonusCells.Clear();
			bool flag = shouldRefreshLinesFlow;
			if (flag)
			{
				this.SetLineFlowImageVisible(false);
				foreach (object obj in this.dotsRoot)
				{
					Transform child = (Transform)obj;
					Object.Destroy(child.gameObject);
					child = null;
				}
				IEnumerator enumerator = null;
			}
			int count = 0;
			foreach (SkillBreakCellBonus bonusCell in this._bonusCells)
			{
				bonusCell.Button.interactable = false;
				bonusCell = null;
			}
			List<SkillBreakCellBonus>.Enumerator enumerator2 = default(List<SkillBreakCellBonus>.Enumerator);
			foreach (ValueTuple<int, int, int> valueTuple in this.IterAllNodes())
			{
				int x = valueTuple.Item1;
				int y = valueTuple.Item2;
				int index = valueTuple.Item3;
				this.PlaceNode(new ValueTuple<int, int>(x, y), index);
				SkillBreakPlateGrid cellData;
				SkillBreakPlateCellWrapper cell = this.RefreshNode(new ValueTuple<int, int>(x, y), index, currentCellGrid, out cellData);
				count++;
				bool flag2 = count % 40 == 0;
				if (flag2)
				{
					yield return null;
				}
				bool flag3 = shouldRefreshLinesFlow;
				if (flag3)
				{
					bool flag4 = cellData.State == ESkillBreakGridState.Invisible;
					if (flag4)
					{
						GameObject clone = Object.Instantiate<GameObject>(cell.gameObject, this.dotsRoot);
						clone.transform.localScale = Vector3.one;
						clone = null;
					}
				}
				cell = null;
				cellData = null;
			}
			IEnumerator<ValueTuple<int, int, int>> enumerator3 = null;
			bool flag5 = shouldRefreshLinesFlow;
			if (flag5)
			{
				IEnumerator sub = this.RefreshLinesFlow();
				yield return new WaitForEndOfFrame();
				while (sub.MoveNext())
				{
					object obj2 = sub.Current;
					yield return obj2;
				}
				this._lastInvisibleSet.Clear();
				this._lastInvisibleSet.UnionWith(currentInvisibleSet);
				sub = null;
			}
			if (OnRefreshNodeFinish != null)
			{
				OnRefreshNodeFinish();
			}
			yield break;
			yield break;
		}

		// Token: 0x06009215 RID: 37397 RVA: 0x0044020C File Offset: 0x0043E40C
		private SkillBreakPlateCellWrapper PreRefreshNode(SkillBreakPlateIndex pos, int childIndex, SkillBreakPlateGrid currentCellGrid, out SkillBreakPlateGrid cellData)
		{
			Transform child = this.cellRoot.GetChild(childIndex);
			SkillBreakPlateCellWrapper cell = child.GetComponent<SkillBreakPlateCellWrapper>();
			cellData = this.DisplayPlate.GetGridAt(pos);
			cell.Refresh(this.DisplayPlate, cellData, pos, this.OnCellClicked, this._skillId, this._lifeSkillAttainments, this._currentExp, this._baseNeedExp, this._requestHandler, this._oldPlate, this.OnPointerEnter, this.OnPointerExit);
			return cell;
		}

		// Token: 0x06009216 RID: 37398 RVA: 0x0044028C File Offset: 0x0043E48C
		private SkillBreakPlateCellWrapper RefreshNode(SkillBreakPlateIndex pos, int childIndex, SkillBreakPlateGrid currentCellGrid, out SkillBreakPlateGrid cellData)
		{
			Transform child = this.cellRoot.GetChild(childIndex);
			SkillBreakPlateCellWrapper cell = child.GetComponent<SkillBreakPlateCellWrapper>();
			cellData = this.DisplayPlate.GetGridAt(pos);
			cell.Refresh(this.DisplayPlate, cellData, pos, this.OnCellClicked, this._skillId, this._lifeSkillAttainments, this._currentExp, this._baseNeedExp, this._requestHandler, this._oldPlate, this.OnPointerEnter, this.OnPointerExit);
			SkillBreakCellBonus bonusCell = cell.ActiveCell as SkillBreakCellBonus;
			bool flag = bonusCell != null;
			if (flag)
			{
				this._bonusCells.Add(bonusCell);
			}
			bool flag2 = cellData.TemplateId == 0;
			SkillBreakPlateCellWrapper result;
			if (flag2)
			{
				cell.SetStartPointGray(this._grayStartPoints.Contains(pos));
				result = cell;
			}
			else
			{
				bool flag3 = currentCellGrid != null;
				if (flag3)
				{
					IReadOnlyList<SkillBreakPlateIndex> selectPath = this.DisplayPlate.SelectPath;
					bool isInPath = selectPath != null && selectPath.Contains(pos);
					bool flag4 = pos != this.DisplayPlate.Current && !isInPath && this.DisplayPlate.CalcDistance(pos, this.DisplayPlate.Current) <= (int)currentCellGrid.Template.NextStepOffset && cellData.State != ESkillBreakGridState.CanSelect;
					if (flag4)
					{
						cell.SetNormalAndSpecialContentAllImageDoubleDim();
					}
				}
				result = cell;
			}
			return result;
		}

		// Token: 0x06009217 RID: 37399 RVA: 0x004403DC File Offset: 0x0043E5DC
		private void PlaceNode(SkillBreakPlateIndex coordinate, int childIndex)
		{
			Transform child = this.cellRoot.GetChild(childIndex);
			Vector2 pos = this._triangleGrid.GetPointPosition(coordinate.X, coordinate.Y);
			child.GetComponent<RectTransform>().anchoredPosition = this.GetChildPosition(pos);
		}

		// Token: 0x06009218 RID: 37400 RVA: 0x00440424 File Offset: 0x0043E624
		private IEnumerator RefreshLinesFlow()
		{
			Camera component = this.linesFlow.GetComponent<Camera>();
			Canvas canvas = this.linesFlow.GetComponentInChildren<Canvas>();
			RenderMode originMode = canvas.renderMode;
			Vector2 originAnchoredPosition = this.linesFlow.anchoredPosition;
			Vector3 originScale = this.linesFlow.localScale;
			int originSiblingIndex = this.linesFlow.GetSiblingIndex();
			Transform originParent = this.linesFlow.parent;
			this.linesFlow.SetParent(null);
			SkillBreakPlateRenderer.<RefreshLinesFlow>g__SetLayerRecursively|37_0(this.linesFlow);
			this.linesFlow.localPosition = Vector3.zero;
			this.linesFlow.localScale = Vector3.one;
			RenderTexture flowMap = new RenderTexture(4096, 4096, GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.None);
			component.targetTexture = flowMap;
			component.enabled = true;
			canvas.enabled = true;
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.worldCamera = component;
			canvas.planeDistance = 250f;
			component.Render();
			yield return new WaitForEndOfFrame();
			component.Render();
			yield return new WaitForEndOfFrame();
			bool flag = this.linesFlowImage.texture;
			if (flag)
			{
				Object.Destroy(this.linesFlowImage.texture);
			}
			this.linesFlowImage.texture = flowMap;
			this.linesFlowImage.SetNativeSize();
			this.SetLineFlowImageVisible(true);
			this.RefreshLineFlowImageColor();
			component.enabled = false;
			canvas.renderMode = originMode;
			canvas.enabled = false;
			this.linesFlow.SetParent(originParent, true);
			this.linesFlow.anchoredPosition = originAnchoredPosition;
			this.linesFlow.localScale = originScale;
			this.linesFlow.SetSiblingIndex(originSiblingIndex);
			this.linesFlowParticle.Play();
			yield break;
		}

		// Token: 0x06009219 RID: 37401 RVA: 0x00440434 File Offset: 0x0043E634
		private static HashSet<SkillBreakPlateIndex> CollectInvisibleSet(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			HashSet<SkillBreakPlateIndex> set = new HashSet<SkillBreakPlateIndex>();
			bool flag = plate == null;
			HashSet<SkillBreakPlateIndex> result;
			if (flag)
			{
				result = set;
			}
			else
			{
				foreach (SkillBreakPlateIndex index in plate.GetIndexes())
				{
					SkillBreakPlateGrid grid = plate.GetGridAt(index);
					bool flag2 = grid.State == ESkillBreakGridState.Invisible;
					if (flag2)
					{
						set.Add(index);
					}
				}
				result = set;
			}
			return result;
		}

		// Token: 0x0600921A RID: 37402 RVA: 0x004404BC File Offset: 0x0043E6BC
		private static bool HasNewInvisibleCells(HashSet<SkillBreakPlateIndex> current, HashSet<SkillBreakPlateIndex> last)
		{
			bool flag = last.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (SkillBreakPlateIndex index in current)
				{
					bool flag2 = !last.Contains(index);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600921B RID: 37403 RVA: 0x00440534 File Offset: 0x0043E734
		[return: TupleElementNames(new string[]
		{
			"x",
			"y",
			"childIndex"
		})]
		private IEnumerable<ValueTuple<int, int, int>> IterAllNodes()
		{
			int index = 0;
			int num;
			for (int y = 0; y < (int)this.DisplayPlate.Height; y = num + 1)
			{
				for (int x = 0; x < (int)this.DisplayPlate.Width; x = num + 1)
				{
					bool flag = !GameData.Domains.Taiwu.SkillBreakPlate.IsProtrusion(y) && x == (int)(this.DisplayPlate.Width - 1);
					if (!flag)
					{
						yield return new ValueTuple<int, int, int>(x, y, index);
						num = index;
						index = num + 1;
					}
					num = x;
				}
				num = y;
			}
			yield break;
		}

		// Token: 0x0600921C RID: 37404 RVA: 0x00440544 File Offset: 0x0043E744
		private bool IsCoordinateValid(SkillBreakPlateIndex coordinate)
		{
			bool flag = coordinate.Y < 0 || coordinate.Y >= (int)this.DisplayPlate.Height;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int rowLength = (int)(GameData.Domains.Taiwu.SkillBreakPlate.IsProtrusion(coordinate.Y) ? this.DisplayPlate.Width : (this.DisplayPlate.Width - 1));
				result = (coordinate.X >= 0 && coordinate.X < rowLength);
			}
			return result;
		}

		// Token: 0x0600921D RID: 37405 RVA: 0x004405C4 File Offset: 0x0043E7C4
		private static int CalculateCellCount(GameData.Domains.Taiwu.SkillBreakPlate displayPlate)
		{
			int count = 0;
			for (int y = 0; y < (int)displayPlate.Height; y++)
			{
				count += (int)(GameData.Domains.Taiwu.SkillBreakPlate.IsProtrusion(y) ? displayPlate.Width : (displayPlate.Width - 1));
			}
			return count;
		}

		// Token: 0x0600921E RID: 37406 RVA: 0x0044060C File Offset: 0x0043E80C
		private void RefreshLines(short skillId)
		{
			bool flag = skillId == this._lastLineSkillId;
			if (!flag)
			{
				this._lastLineSkillId = skillId;
				List<List<Vector2>> lines = this.GenerateLines();
				CommonUtils.PrepareEnoughChildren(this.lineRenderersRoot.transform, this.lineRendererTemplate.gameObject, lines.Count, null);
				ValueTuple<float, float, float, float> bounds = this.CalcLinesBounds(lines);
				Vector2 size = new Vector2(bounds.Item3 - bounds.Item1, bounds.Item4 - bounds.Item2);
				for (int i = 0; i < lines.Count; i++)
				{
					List<Vector2> line = lines[i];
					LineRenderer2D lineRenderer = this.lineRenderersRoot.GetChild(i).GetComponent<LineRenderer2D>();
					lineRenderer.Vertices = line.ToArray();
					lineRenderer.GetComponent<RectTransform>().SetSize(size);
				}
				this.FreeLines(lines);
			}
		}

		// Token: 0x0600921F RID: 37407 RVA: 0x004406F0 File Offset: 0x0043E8F0
		[return: TupleElementNames(new string[]
		{
			"minX",
			"minY",
			"maxX",
			"maxY"
		})]
		private ValueTuple<float, float, float, float> CalcLinesBounds(List<List<Vector2>> lines)
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			foreach (List<Vector2> pos in lines)
			{
				ValueTuple<float, float, float, float> valueTuple = this.CalcLineBounds(pos);
				float minX2 = valueTuple.Item1;
				float minY2 = valueTuple.Item2;
				float maxX2 = valueTuple.Item3;
				float maxY2 = valueTuple.Item4;
				minX = Mathf.Min(minX, minX2);
				minY = Mathf.Min(minY, minY2);
				maxX = Mathf.Max(maxX, maxX2);
				maxY = Mathf.Max(maxY, maxY2);
			}
			return new ValueTuple<float, float, float, float>(minX, minY, maxX, maxY);
		}

		// Token: 0x06009220 RID: 37408 RVA: 0x004407B0 File Offset: 0x0043E9B0
		[return: TupleElementNames(new string[]
		{
			"minX",
			"minY",
			"maxX",
			"maxY"
		})]
		private ValueTuple<float, float, float, float> CalcLineBounds(List<Vector2> line)
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			foreach (Vector2 pos in line)
			{
				minX = Mathf.Min(minX, pos.x);
				minY = Mathf.Min(minY, pos.y);
				maxX = Mathf.Max(maxX, pos.x);
				maxY = Mathf.Max(maxY, pos.y);
			}
			return new ValueTuple<float, float, float, float>(minX, minY, maxX, maxY);
		}

		// Token: 0x06009221 RID: 37409 RVA: 0x00440860 File Offset: 0x0043EA60
		private List<List<Vector2>> GenerateLines()
		{
			List<List<Vector2>> result = EasyPool.Get<List<List<Vector2>>>();
			result.Clear();
			for (int y = 0; y < this._triangleGrid.Height; y++)
			{
				List<Vector2> line = EasyPool.Get<List<Vector2>>();
				line.Clear();
				int endX = GameData.Domains.Taiwu.SkillBreakPlate.IsProtrusion(y) ? this._triangleGrid.Width : (this._triangleGrid.Width - 1);
				for (int x = 0; x < endX; x++)
				{
					Vector2 pos = this.GetChildPosition(x, y);
					line.Add(pos);
				}
				result.Add(line);
			}
			for (int y2 = 0; y2 < this._triangleGrid.Height; y2++)
			{
				bool flag = !GameData.Domains.Taiwu.SkillBreakPlate.IsProtrusion(y2);
				if (!flag)
				{
					bool flag2 = y2 > 0;
					if (flag2)
					{
						result.Add(this.GenerateZigZagLine(y2, -1));
					}
					bool flag3 = y2 < this._triangleGrid.Height - 1;
					if (flag3)
					{
						result.Add(this.GenerateZigZagLine(y2, 1));
					}
				}
			}
			return result;
		}

		// Token: 0x06009222 RID: 37410 RVA: 0x0044097A File Offset: 0x0043EB7A
		private void FreeLines(List<List<Vector2>> lines)
		{
			lines.ForEach(new Action<List<Vector2>>(EasyPool.Free<List<Vector2>>));
			EasyPool.Free<List<List<Vector2>>>(lines);
		}

		// Token: 0x06009223 RID: 37411 RVA: 0x00440998 File Offset: 0x0043EB98
		private List<Vector2> GenerateZigZagLine(int y, int deltaY)
		{
			List<Vector2> resultLine = EasyPool.Get<List<Vector2>>();
			resultLine.Clear();
			for (int x = 0; x < this._triangleGrid.Width; x++)
			{
				Vector2 pos0 = this.GetChildPosition(x, y);
				resultLine.Add(pos0);
				bool flag = x == this._triangleGrid.Width - 1;
				if (flag)
				{
					break;
				}
				Vector2 pos = this.GetChildPosition(x, y + deltaY);
				resultLine.Add(pos);
			}
			return resultLine;
		}

		// Token: 0x06009224 RID: 37412 RVA: 0x00440A14 File Offset: 0x0043EC14
		private void RefreshSelectedPath()
		{
			SkillBreakPlateRenderer.<>c__DisplayClass50_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.segments = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			CS$<>8__locals1.segments.Clear();
			CS$<>8__locals1.longSegments = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			CS$<>8__locals1.longSegments.Clear();
			SkillBreakPlateIndex lastCoord = SkillBreakPlateIndex.Invalid;
			bool flag = this.DisplayPlate.SelectPath != null;
			if (flag)
			{
				SkillBreakPlateIndex? possibleStartPoint = null;
				foreach (SkillBreakPlateIndex coord in this.DisplayPlate.SelectPath)
				{
					SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(coord);
					bool flag2 = cell.TemplateId == 0;
					if (flag2)
					{
						possibleStartPoint = new SkillBreakPlateIndex?(coord);
					}
					else
					{
						bool flag3 = cell.State != ESkillBreakGridState.Selected;
						if (!flag3)
						{
							bool flag4 = possibleStartPoint != null;
							if (flag4)
							{
								this.<RefreshSelectedPath>g__Add|50_0(new SkillBreakPlateRenderer.Segment(possibleStartPoint.Value, coord), ref CS$<>8__locals1);
								possibleStartPoint = null;
								lastCoord = coord;
							}
							else
							{
								this.<RefreshSelectedPath>g__Add|50_0(new SkillBreakPlateRenderer.Segment(lastCoord, coord), ref CS$<>8__locals1);
								lastCoord = coord;
							}
						}
					}
				}
			}
			this.RenderPaths(CS$<>8__locals1.segments, this.normalPathTemplate, this.normalPathRoot, true, false, null, this._isEnable);
			this.RenderPaths(CS$<>8__locals1.longSegments, this.longPathTemplate, this.longPathRoot, true, false, null, false);
			bool isEnable = this._isEnable;
			if (isEnable)
			{
				this.RenderPaths(CS$<>8__locals1.segments, this.effectNormalPathTemplate, this.effectNormalPathRoot, false, false, null, false);
			}
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(CS$<>8__locals1.segments);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(CS$<>8__locals1.longSegments);
		}

		// Token: 0x06009225 RID: 37413 RVA: 0x00440BD8 File Offset: 0x0043EDD8
		private void RefreshCanSelectPath()
		{
			SkillBreakPlateIndex startPos = this.DisplayPlate.Current;
			HashSet<SkillBreakPlateRenderer.Segment> shortSegments = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			HashSet<SkillBreakPlateRenderer.Segment> typeOneLongSegments = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			HashSet<SkillBreakPlateRenderer.Segment> typeTwoLongSegments = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			shortSegments.Clear();
			typeOneLongSegments.Clear();
			typeTwoLongSegments.Clear();
			bool flag = this.DisplayPlate.SelectPath != null && startPos != SkillBreakPlateIndex.Invalid;
			if (flag)
			{
				var segments = from coord in this.DisplayPlate.GetIndexes()
				let cell = this.DisplayPlate.GetGridAt(coord)
				where cell.State == ESkillBreakGridState.CanSelect
				let distance = this.GetRealDistance(startPos, coord)
				select new
				{
					coord,
					distance
				};
				var enumerable = segments.ToList();
				shortSegments.UnionWith(from s in enumerable
				where !this.IsLongDistance(s.distance)
				select new SkillBreakPlateRenderer.Segment(startPos, s.coord));
				typeOneLongSegments.UnionWith(from s in enumerable
				where this.TypeOneLongDistance(s.distance)
				select new SkillBreakPlateRenderer.Segment(startPos, s.coord));
				typeTwoLongSegments.UnionWith(from s in enumerable
				where this.TypeTwoLongDistance(s.distance)
				select new SkillBreakPlateRenderer.Segment(startPos, s.coord));
			}
			else
			{
				using (IEnumerator<SkillBreakPlateIndex> enumerator = this.DisplayPlate.GetIndexes().GetEnumerator())
				{
					Func<SkillBreakPlateIndex, <>f__AnonymousType1<SkillBreakPlateIndex, SkillBreakPlateGrid>> <>9__10;
					Func<<>f__AnonymousType3<SkillBreakPlateIndex, float>, bool> <>9__14;
					while (enumerator.MoveNext())
					{
						SkillBreakPlateIndex pointIndex = enumerator.Current;
						SkillBreakPlateGrid pointCell = this.DisplayPlate.GetGridAt(pointIndex);
						bool flag2 = pointCell.TemplateId != 0;
						if (!flag2)
						{
							IEnumerable<SkillBreakPlateIndex> indexes = this.DisplayPlate.GetIndexes();
							var selector;
							if ((selector = <>9__10) == null)
							{
								selector = (<>9__10 = ((SkillBreakPlateIndex coord) => new
								{
									coord = coord,
									cell = this.DisplayPlate.GetGridAt(coord)
								}));
							}
							var segments2 = from <>h__TransparentIdentifier0 in indexes.Select(selector)
							where cell.State == ESkillBreakGridState.CanSelect
							let distance = this.GetRealDistance(pointIndex, coord)
							select new
							{
								coord,
								distance
							};
							var enumerable2 = segments2.ToList();
							HashSet<SkillBreakPlateRenderer.Segment> hashSet = shortSegments;
							var source = enumerable2;
							var predicate;
							if ((predicate = <>9__14) == null)
							{
								predicate = (<>9__14 = (s => !this.IsLongDistance(s.distance)));
							}
							hashSet.UnionWith(from s in source.Where(predicate)
							select new SkillBreakPlateRenderer.Segment(pointIndex, s.coord));
						}
					}
				}
			}
			HashSet<SkillBreakPlateRenderer.Segment> newShortCanSelectPaths = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			HashSet<SkillBreakPlateRenderer.Segment> oldShortCanSelectPaths = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			newShortCanSelectPaths.Clear();
			oldShortCanSelectPaths.Clear();
			bool flag3 = this._oldPlate != null;
			if (flag3)
			{
				newShortCanSelectPaths.UnionWith(shortSegments.Except(this._lastCanSelectShortSegments));
				oldShortCanSelectPaths.UnionWith(this._lastCanSelectShortSegments.Intersect(shortSegments));
			}
			else
			{
				oldShortCanSelectPaths.UnionWith(shortSegments);
			}
			this.RenderPaths(newShortCanSelectPaths, this.canSelectShortPathTemplate, this.canSelectShortPathWithAnimationRoot, false, true, new SkillBreakPlateRenderer.RefreshSegmentStyleFunc(this.RefreshCanSelectShortPathByExp), false);
			this.RenderPaths(oldShortCanSelectPaths, this.canSelectShortPathTemplate, this.canSelectShortPathRoot, false, false, new SkillBreakPlateRenderer.RefreshSegmentStyleFunc(this.RefreshCanSelectShortPathByExp), false);
			this.RenderPaths(typeOneLongSegments, this.canSelectLongPathTemplate, this.canSelectLongPathRoot, true, false, null, false);
			this.RenderPaths(typeTwoLongSegments, this.canSelectLongPathTemplate2, this.canSelectLongPathRoot2, true, false, null, false);
			this._lastCanSelectShortSegments.Clear();
			this._lastCanSelectShortSegments.UnionWith(shortSegments);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(shortSegments);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(typeOneLongSegments);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(typeTwoLongSegments);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(newShortCanSelectPaths);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(oldShortCanSelectPaths);
		}

		// Token: 0x06009226 RID: 37414 RVA: 0x0044100C File Offset: 0x0043F20C
		private void RefreshCanSelectShortPathByExp(SkillBreakPlateRenderer.Segment segment, GameObject pathObj)
		{
			SkillBreakPlateIndex end = segment.End;
			SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(end);
			bool flag = cell.State != ESkillBreakGridState.CanSelect;
			if (!flag)
			{
				CImage image = pathObj.GetComponent<CImage>();
				bool flag2 = image == null;
				if (!flag2)
				{
					int needExp = SkillBreakPlateUtils.GetNeedExp(this._skillId, this.DisplayPlate, end, this._baseNeedExp);
					bool expEnough = this._currentExp >= needExp;
					image.SetSprite(expEnough ? "skillbreak_strip_3" : "skillbreak_strip_5", false, null);
				}
			}
		}

		// Token: 0x06009227 RID: 37415 RVA: 0x00441098 File Offset: 0x0043F298
		private void RefreshFailedPath()
		{
			HashSet<SkillBreakPlateRenderer.Segment> segments = EasyPool.Get<HashSet<SkillBreakPlateRenderer.Segment>>();
			segments.Clear();
			SkillBreakPlateIndex? lastCoord = null;
			bool flag = this.DisplayPlate.SelectPath != null;
			if (flag)
			{
				foreach (SkillBreakPlateIndex coord in this.DisplayPlate.SelectPath)
				{
					SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(coord);
					bool flag2 = cell.State == ESkillBreakGridState.Failed;
					if (flag2)
					{
						bool flag3 = lastCoord != null && !this.IsLongConnection(lastCoord.Value, coord);
						if (flag3)
						{
							segments.Add(new SkillBreakPlateRenderer.Segment(lastCoord.Value, coord));
						}
					}
					bool flag4 = cell.State == ESkillBreakGridState.Selected || cell.TemplateId == 0;
					if (flag4)
					{
						lastCoord = new SkillBreakPlateIndex?(coord);
					}
				}
			}
			this.RenderPaths(segments, this.failPathTemplate, this.failedPathRoot, false, false, null, false);
			EasyPool.Free<HashSet<SkillBreakPlateRenderer.Segment>>(segments);
		}

		// Token: 0x06009228 RID: 37416 RVA: 0x004411BC File Offset: 0x0043F3BC
		private void RenderPaths(HashSet<SkillBreakPlateRenderer.Segment> segments, RectTransform template, RectTransform root, bool isParticle = false, bool needGrowAnimation = false, SkillBreakPlateRenderer.RefreshSegmentStyleFunc refreshSegmentStyleFunc = null, bool isCloseActive = false)
		{
			CommonUtils.PrepareEnoughChildren(root.transform, template.gameObject, segments.Count, null);
			int i = 0;
			foreach (SkillBreakPlateRenderer.Segment segment in segments)
			{
				SkillBreakPlateIndex start2 = segment.Start;
				SkillBreakPlateIndex end2 = segment.End;
				SkillBreakPlateIndex start = start2;
				SkillBreakPlateIndex end = end2;
				Transform child = root.GetChild(i);
				i++;
				child.gameObject.name = this.GetPathName(start, end);
				Vector2 startPos = this._triangleGrid.GetPointPosition(start.X, start.Y);
				Vector2 endPos = this._triangleGrid.GetPointPosition(end.X, end.Y);
				Vector2 center = (startPos + endPos) / 2f;
				Vector2 centerChildPos = this.GetChildPosition(center);
				RectTransform rect = child.GetComponent<RectTransform>();
				float distance = this.GetRealDistance(start, end);
				if (refreshSegmentStyleFunc != null)
				{
					refreshSegmentStyleFunc(segment, child.gameObject);
				}
				if (isParticle)
				{
					this.SetSizeForUIParticle(distance, child, template);
				}
				else
				{
					this.SetSizeForRectTransform(distance, child);
					if (needGrowAnimation)
					{
						CImage image = rect.GetComponent<CImage>();
						image.type = Image.Type.Filled;
						image.fillAmount = 0f;
						image.DOKill(false);
						image.DOFillAmount(1f, 0.3f);
					}
				}
				if (isCloseActive)
				{
					child.gameObject.SetActive(false);
				}
				rect.anchoredPosition = centerChildPos;
				rect.rotation = Quaternion.Euler(0f, 0f, this._triangleGrid.GetRotationZ(start.X, start.Y, end.X, end.Y));
			}
		}

		// Token: 0x06009229 RID: 37417 RVA: 0x004413BC File Offset: 0x0043F5BC
		private void SetSizeForRectTransform(float realDistance, Transform pathNode)
		{
			RectTransform rectTransform = pathNode.GetComponent<RectTransform>();
			rectTransform.SetSize(new Vector2(realDistance, rectTransform.rect.height));
		}

		// Token: 0x0600922A RID: 37418 RVA: 0x004413EC File Offset: 0x0043F5EC
		private void SetSizeForUIParticle(float realDistance, Transform pathNode, RectTransform template)
		{
			UIParticle particle = pathNode.GetComponent<UIParticle>();
			Transform child = particle.transform.GetChild(0);
			float baseScale = template.GetChild(0).localScale.x;
			float baseLength = this._triangleGrid.TriangleBase;
			float scale = realDistance / baseLength;
			child.localScale = new Vector3(scale * baseScale, child.localScale.y, child.localScale.z);
			particle.Play();
		}

		// Token: 0x0600922B RID: 37419 RVA: 0x00441460 File Offset: 0x0043F660
		private string GetPathName(SkillBreakPlateIndex start, SkillBreakPlateIndex end)
		{
			return string.Format("({0},{1})_({2},{3})", new object[]
			{
				start.X,
				start.Y,
				end.X,
				end.Y
			});
		}

		// Token: 0x0600922C RID: 37420 RVA: 0x004414C0 File Offset: 0x0043F6C0
		private float GetRealDistance(SkillBreakPlateIndex start, SkillBreakPlateIndex end)
		{
			Vector2 startPos = this._triangleGrid.GetPointPosition(start.X, start.Y);
			Vector2 endPos = this._triangleGrid.GetPointPosition(end.X, end.Y);
			return Vector2.Distance(startPos, endPos);
		}

		// Token: 0x0600922D RID: 37421 RVA: 0x00441510 File Offset: 0x0043F710
		private bool IsLongConnection(SkillBreakPlateIndex start, SkillBreakPlateIndex end)
		{
			float dis = this.GetRealDistance(start, end);
			return this.IsLongDistance(dis);
		}

		// Token: 0x0600922E RID: 37422 RVA: 0x00441534 File Offset: 0x0043F734
		private bool IsLongDistance(float distance)
		{
			return distance / this.triangleBase >= 1.2f;
		}

		// Token: 0x0600922F RID: 37423 RVA: 0x00441558 File Offset: 0x0043F758
		private bool TypeTwoLongDistance(float distance)
		{
			float value = distance / this.triangleBase;
			return value <= 3.6000001f && value >= 1.2f;
		}

		// Token: 0x06009230 RID: 37424 RVA: 0x0044158C File Offset: 0x0043F78C
		private bool TypeOneLongDistance(float distance)
		{
			float value = distance / this.triangleBase;
			return value > 3.6000001f;
		}

		// Token: 0x06009231 RID: 37425 RVA: 0x004415B0 File Offset: 0x0043F7B0
		private void RefreshRanges()
		{
			List<SkillBreakPlateIndex> rangeList = EasyPool.Get<List<SkillBreakPlateIndex>>();
			List<SkillBreakPlateIndex> rangeList2 = EasyPool.Get<List<SkillBreakPlateIndex>>();
			List<SkillBreakPlateIndex> rangeList3 = EasyPool.Get<List<SkillBreakPlateIndex>>();
			rangeList.Clear();
			rangeList2.Clear();
			rangeList3.Clear();
			List<SkillBreakPlateIndex>[] rangeLists = new List<SkillBreakPlateIndex>[]
			{
				rangeList,
				rangeList2,
				rangeList3
			};
			foreach (SkillBreakPlateIndex coord in this.DisplayPlate.GetIndexes())
			{
				SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(coord);
				bool flag = cell.TemplateId != 2;
				if (!flag)
				{
					int range = this.DisplayPlate.GetBonus(coord).ImpactRange;
					bool flag2 = !SkillBreakPlateRenderer.IsValidRange(range);
					if (!flag2)
					{
						rangeLists[range - 1].Add(coord);
					}
				}
			}
			for (int i = 0; i < rangeLists.Length; i++)
			{
				RectTransform root = this.GetRangeRoot(i + 1);
				GameObject template = this.GetRangeTemplate(i + 1);
				CommonUtils.PrepareEnoughChildren(root.transform, template.gameObject, rangeLists[i].Count, null);
				for (int j = 0; j < rangeLists[i].Count; j++)
				{
					SkillBreakPlateIndex coord2 = rangeLists[i][j];
					Transform child = root.GetChild(j);
					Vector2 pos = this._triangleGrid.GetPointPosition(coord2.X, coord2.Y);
					child.GetComponent<RectTransform>().anchoredPosition = this.GetChildPosition(pos);
				}
			}
			EasyPool.Free<List<SkillBreakPlateIndex>>(rangeList);
			EasyPool.Free<List<SkillBreakPlateIndex>>(rangeList2);
			EasyPool.Free<List<SkillBreakPlateIndex>>(rangeList3);
		}

		// Token: 0x06009232 RID: 37426 RVA: 0x00441778 File Offset: 0x0043F978
		private GameObject GetRangeTemplate(int range)
		{
			return this.rangeTemplateList[range - 1];
		}

		// Token: 0x06009233 RID: 37427 RVA: 0x00441798 File Offset: 0x0043F998
		private RectTransform GetRangeRoot(int range)
		{
			return this.rangeRootList[range - 1];
		}

		// Token: 0x06009234 RID: 37428 RVA: 0x004417B8 File Offset: 0x0043F9B8
		private static bool IsValidRange(int range)
		{
			return range >= 1 && range <= 3;
		}

		// Token: 0x06009235 RID: 37429 RVA: 0x004417D8 File Offset: 0x0043F9D8
		private void PlayVisibleEffect(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			bool hasVisibleEffect = false;
			foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
			{
				SkillBreakPlateGrid cell = plate.GetGridAt(coord);
				ESkillBreakGridState state = cell.State;
				bool flag = (state == ESkillBreakGridState.Showed || state == ESkillBreakGridState.CanSelect) && lastPlate != null && lastPlate.GetGridAt(coord).State == ESkillBreakGridState.Invisible && (plate.Current == SkillBreakPlateIndex.Invalid || this.IsLongConnection(coord, plate.Current));
				if (flag)
				{
					SkillBreakPlateRenderer.<>c__DisplayClass70_0 CS$<>8__locals1 = new SkillBreakPlateRenderer.<>c__DisplayClass70_0();
					UIParticle template = (cell.Template.Type == ESkillBreakGridTypeType.Normal) ? this.normalVisibleParticleTemplate : this.specialVisibleParticleTemplate;
					Vector2 pos = this.GetChildPosition(coord.X, coord.Y);
					CS$<>8__locals1.particleObject = Object.Instantiate<UIParticle>(template, this.oneShotParticleRoot);
					CS$<>8__locals1.particleObject.rectTransform.anchoredPosition = pos;
					this._uiParticlePlayHelper.PlayOnceParticle(CS$<>8__locals1.particleObject, 0.5f, new Action(CS$<>8__locals1.<PlayVisibleEffect>g__OnFinished|0));
					hasVisibleEffect = true;
				}
			}
			bool flag2 = hasVisibleEffect;
			if (flag2)
			{
				AudioManager.Instance.PlaySound("study_reveal", false, false);
			}
		}

		// Token: 0x06009236 RID: 37430 RVA: 0x0044193C File Offset: 0x0043FB3C
		private void PlaySelectedEffect(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			bool hasSelectedEffect = false;
			foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
			{
				SkillBreakPlateRenderer.<>c__DisplayClass71_0 CS$<>8__locals1 = new SkillBreakPlateRenderer.<>c__DisplayClass71_0();
				SkillBreakPlateGrid cell = plate.GetGridAt(coord);
				bool needPlay = cell.State == ESkillBreakGridState.Selected && (lastPlate == null || lastPlate.GetGridAt(coord).State != ESkillBreakGridState.Selected);
				bool flag = !needPlay;
				if (!flag)
				{
					ESkillBreakGridTypeType type = cell.Template.Type;
					if (!true)
					{
					}
					ValueTuple<UIParticle, float> valueTuple;
					if (type != ESkillBreakGridTypeType.Bonus)
					{
						if (type != ESkillBreakGridTypeType.Normal)
						{
							valueTuple = new ValueTuple<UIParticle, float>(this.specialSelectedParticleTemplate, 0.5f);
						}
						else
						{
							valueTuple = new ValueTuple<UIParticle, float>(this.normalSelectedParticleTemplate, 0.5f);
						}
					}
					else
					{
						valueTuple = new ValueTuple<UIParticle, float>(this.bonusConnectParticleTemplate, 1.4f);
					}
					if (!true)
					{
					}
					ValueTuple<UIParticle, float> valueTuple2 = valueTuple;
					UIParticle template = valueTuple2.Item1;
					float duration = valueTuple2.Item2;
					Vector2 pos = this.GetChildPosition(coord.X, coord.Y);
					CS$<>8__locals1.particleObject = Object.Instantiate<UIParticle>(template, this.oneShotParticleRoot);
					CS$<>8__locals1.particleObject.rectTransform.anchoredPosition = pos;
					this._uiParticlePlayHelper.PlayOnceParticle(CS$<>8__locals1.particleObject, duration, new Action(CS$<>8__locals1.<PlaySelectedEffect>g__OnFinished|0));
					hasSelectedEffect = true;
				}
			}
			bool flag2 = hasSelectedEffect;
			if (flag2)
			{
				AudioManager.Instance.PlaySound("study_change_success", false, false);
			}
		}

		// Token: 0x06009237 RID: 37431 RVA: 0x00441AD4 File Offset: 0x0043FCD4
		private void PlayPowerChangeEffect(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
			{
				SkillBreakPlateRenderer.<>c__DisplayClass72_0 CS$<>8__locals1 = new SkillBreakPlateRenderer.<>c__DisplayClass72_0();
				SkillBreakPlateGrid cell = plate.GetGridAt(coord);
				bool needPlay = cell.TemplateId != 2 && lastPlate != null && plate.CalcAddMaxPower(coord) != lastPlate.CalcAddMaxPower(coord);
				bool flag = !needPlay;
				if (!flag)
				{
					Vector2 pos = this.GetChildPosition(coord.X, coord.Y);
					CS$<>8__locals1.particleObject = Object.Instantiate<UIParticle>(this.powerChangeParticleTemplate, this.oneShotParticleRoot);
					CS$<>8__locals1.particleObject.rectTransform.anchoredPosition = pos;
					this._uiParticlePlayHelper.PlayOnceParticle(CS$<>8__locals1.particleObject, 1f, new Action(CS$<>8__locals1.<PlayPowerChangeEffect>g__OnFinished|0));
				}
			}
		}

		// Token: 0x06009238 RID: 37432 RVA: 0x00441BD0 File Offset: 0x0043FDD0
		private void PlayBonusFillEffect(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
			{
				SkillBreakPlateRenderer.<>c__DisplayClass73_0 CS$<>8__locals1 = new SkillBreakPlateRenderer.<>c__DisplayClass73_0();
				SkillBreakPlateBonus bonus = plate.GetBonus(coord);
				bool flag = lastPlate == null;
				if (!flag)
				{
					SkillBreakPlateBonus lastBonus = lastPlate.GetBonus(coord);
					bool typeChanged = bonus.Type != lastBonus.Type;
					bool flag2 = typeChanged;
					bool needPlay;
					if (flag2)
					{
						needPlay = (bonus.Type > ESkillBreakPlateBonusType.None);
					}
					else
					{
						ESkillBreakPlateBonusType type = bonus.Type;
						if (!true)
						{
						}
						bool flag3;
						switch (type)
						{
						case ESkillBreakPlateBonusType.None:
							flag3 = false;
							break;
						case ESkillBreakPlateBonusType.Item:
							flag3 = (bonus.ItemTemplateId != lastBonus.ItemTemplateId);
							break;
						case ESkillBreakPlateBonusType.Relation:
							flag3 = (bonus.RelationCharId != lastBonus.RelationCharId || bonus.RelationType != lastBonus.RelationType);
							break;
						case ESkillBreakPlateBonusType.Exp:
							flag3 = (bonus.ExpLevel != lastBonus.ExpLevel);
							break;
						case ESkillBreakPlateBonusType.Friend:
							flag3 = (bonus.RelationCharId != lastBonus.RelationCharId);
							break;
						default:
							throw new ArgumentOutOfRangeException();
						}
						if (!true)
						{
						}
						needPlay = flag3;
					}
					bool flag4 = !needPlay;
					if (!flag4)
					{
						Vector2 pos = this.GetChildPosition(coord.X, coord.Y);
						CS$<>8__locals1.particleObject = Object.Instantiate<UIParticle>(this.bonusFillParticleTemplate, this.oneShotParticleRoot);
						CS$<>8__locals1.particleObject.scale = (float)this.GetBonusParticleScale(bonus.ImpactRange);
						CS$<>8__locals1.particleObject.rectTransform.anchoredPosition = pos;
						this._uiParticlePlayHelper.PlayOnceParticle(CS$<>8__locals1.particleObject, 1f, new Action(CS$<>8__locals1.<PlayBonusFillEffect>g__OnFinished|0));
					}
				}
			}
		}

		// Token: 0x06009239 RID: 37433 RVA: 0x00441DBC File Offset: 0x0043FFBC
		private int GetBonusParticleScale(int impactRange)
		{
			bool flag = impactRange == 1;
			int result;
			if (flag)
			{
				result = 88;
			}
			else
			{
				bool flag2 = impactRange == 3;
				if (flag2)
				{
					result = 200;
				}
				else
				{
					result = 144;
				}
			}
			return result;
		}

		// Token: 0x0600923A RID: 37434 RVA: 0x00441DF4 File Offset: 0x0043FFF4
		private void PlayBreakFailEffect(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			bool flag = plate.SelectPath == null;
			if (!flag)
			{
				foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
				{
					SkillBreakPlateRenderer.<>c__DisplayClass75_0 CS$<>8__locals1 = new SkillBreakPlateRenderer.<>c__DisplayClass75_0();
					if (plate.SelectPath.Count <= 0)
					{
						goto IL_72;
					}
					IReadOnlyList<SkillBreakPlateIndex> selectPath = plate.SelectPath;
					if (!coord.Equals(selectPath[selectPath.Count - 1]))
					{
						goto IL_72;
					}
					bool flag2 = ViewCharacterMenuSkillBreakPlate.IsGridBreakFail(plate, lastPlate) || ViewCharacterMenuSkillBreakPlate.IsHardStudyGrid(plate, lastPlate);
					IL_73:
					bool needPlay = flag2;
					bool flag3 = !needPlay;
					if (flag3)
					{
						continue;
					}
					Vector2 pos = this.GetChildPosition(coord.X, coord.Y);
					CS$<>8__locals1.particleObject = Object.Instantiate<UIParticle>(this.breakFailParticleTemplate, this.oneShotParticleRoot);
					CS$<>8__locals1.particleObject.rectTransform.anchoredPosition = pos;
					this._uiParticlePlayHelper.PlayOnceParticle(CS$<>8__locals1.particleObject, 1f, new Action(CS$<>8__locals1.<PlayBreakFailEffect>g__OnFinished|0));
					continue;
					IL_72:
					flag2 = false;
					goto IL_73;
				}
			}
		}

		// Token: 0x0600923B RID: 37435 RVA: 0x00441F10 File Offset: 0x00440110
		private void PlayGridTypeChangeEffect(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			bool flag = plate.SelectPath == null;
			if (!flag)
			{
				bool playMusic = false;
				foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
				{
					SkillBreakPlateRenderer.<>c__DisplayClass76_0 CS$<>8__locals1 = new SkillBreakPlateRenderer.<>c__DisplayClass76_0();
					SkillBreakPlateGrid cell = plate.GetGridAt(coord);
					SkillBreakPlateGrid lastCell = lastPlate.GetGridAt(coord);
					if (plate.SelectPath.Count <= 0 || cell.TemplateId == lastCell.TemplateId)
					{
						goto IL_8B;
					}
					IReadOnlyList<SkillBreakPlateIndex> selectPath = plate.SelectPath;
					if (!coord.Equals(selectPath[selectPath.Count - 1]))
					{
						goto IL_8B;
					}
					bool flag2 = ViewCharacterMenuSkillBreakPlate.IsHardStudyGrid(plate, lastPlate);
					IL_8C:
					bool needPlay = flag2;
					bool flag3 = !needPlay;
					if (flag3)
					{
						continue;
					}
					playMusic = true;
					Vector2 pos = this.GetChildPosition(coord.X, coord.Y);
					CS$<>8__locals1.particleObject = Object.Instantiate<UIParticle>(this.gridTypeChangeParticleTemplate, this.oneShotParticleRoot);
					CS$<>8__locals1.particleObject.rectTransform.anchoredPosition = pos;
					this._uiParticlePlayHelper.PlayOnceParticle(CS$<>8__locals1.particleObject, 1f, new Action(CS$<>8__locals1.<PlayGridTypeChangeEffect>g__OnFinished|0));
					continue;
					IL_8B:
					flag2 = false;
					goto IL_8C;
				}
				bool flag4 = playMusic;
				if (flag4)
				{
					AudioManager.Instance.PlaySound("study_gate", false, false);
				}
			}
		}

		// Token: 0x0600923C RID: 37436 RVA: 0x00442064 File Offset: 0x00440264
		private void RefreshLoopParticles(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			this._grayStartPoints.Clear();
			List<SkillBreakPlateIndex> endPoints = EasyPool.Get<List<SkillBreakPlateIndex>>();
			endPoints.Clear();
			List<SkillBreakPlateIndex> portals = EasyPool.Get<List<SkillBreakPlateIndex>>();
			portals.Clear();
			List<SkillBreakPlateIndex> madPoints = EasyPool.Get<List<SkillBreakPlateIndex>>();
			madPoints.Clear();
			List<SkillBreakPlateIndex> selectedStartPoints = EasyPool.Get<List<SkillBreakPlateIndex>>();
			selectedStartPoints.Clear();
			foreach (SkillBreakPlateIndex coord in plate.GetIndexes())
			{
				SkillBreakPlateGrid cell = plate.GetGridAt(coord);
				bool flag = cell.TemplateId == 1 && plate.Finished;
				if (flag)
				{
					endPoints.Add(coord);
				}
				bool flag2 = cell.TemplateId == 0;
				if (flag2)
				{
					this._grayStartPoints.Add(coord);
				}
				bool flag3 = cell.TemplateId == 21;
				if (flag3)
				{
					ESkillBreakGridState state = cell.State;
					ESkillBreakGridState eskillBreakGridState = state;
					if (eskillBreakGridState != ESkillBreakGridState.CanSelect)
					{
						if (eskillBreakGridState == ESkillBreakGridState.Selected)
						{
							goto IL_FE;
						}
					}
					else if (plate.Current != SkillBreakPlateIndex.Invalid && plate.GetGridAt(plate.Current).TemplateId == 21)
					{
						goto IL_FE;
					}
					goto IL_10A;
					IL_FE:
					portals.Add(coord);
				}
				IL_10A:
				bool recordedStepIsGoneMad = cell.RecordedStepIsGoneMad;
				if (recordedStepIsGoneMad)
				{
					madPoints.Add(coord);
				}
			}
			int i = 0;
			for (;;)
			{
				int num = i;
				IReadOnlyList<SkillBreakPlateIndex> selectPath = plate.SelectPath;
				if (num >= ((selectPath != null) ? (selectPath.Count - 1) : 0))
				{
					break;
				}
				SkillBreakPlateIndex index = plate.SelectPath[i];
				bool flag4 = selectedStartPoints.Contains(index);
				if (!flag4)
				{
					bool flag5 = plate.GetGridAt(index).TemplateId != 0;
					if (!flag5)
					{
						SkillBreakPlateIndex next = plate.SelectPath[i + 1];
						bool flag6 = plate.GetGridAt(next).State == ESkillBreakGridState.Selected;
						if (flag6)
						{
							selectedStartPoints.Add(index);
							this._grayStartPoints.Remove(index);
						}
					}
				}
				i++;
			}
			bool flag7 = selectedStartPoints.Count == 0;
			if (flag7)
			{
				this._grayStartPoints.Clear();
			}
			CommonUtils.PrepareEnoughChildren(this.endPointParticleRoot.transform, this.endPointParticleTemplate.gameObject, endPoints.Count, null);
			for (int j = 0; j < endPoints.Count; j++)
			{
				Transform child = this.endPointParticleRoot.transform.GetChild(j);
				child.gameObject.SetActive(true);
				child.GetComponent<RectTransform>().anchoredPosition = this.GetChildPosition(endPoints[j].X, endPoints[j].Y);
			}
			CommonUtils.PrepareEnoughChildren(this.startPointParticleRoot.transform, this.startPointParticleTemplate.gameObject, selectedStartPoints.Count, null);
			for (int k = 0; k < selectedStartPoints.Count; k++)
			{
				Transform child2 = this.startPointParticleRoot.transform.GetChild(k);
				child2.gameObject.SetActive(true);
				child2.GetComponent<RectTransform>().anchoredPosition = this.GetChildPosition(selectedStartPoints[k].X, selectedStartPoints[k].Y);
			}
			CommonUtils.PrepareEnoughChildren(this.portalParticleRoot.transform, this.portalParticleTemplate.gameObject, portals.Count, null);
			for (int l = 0; l < portals.Count; l++)
			{
				Transform child3 = this.portalParticleRoot.transform.GetChild(l);
				child3.gameObject.SetActive(true);
				child3.GetComponent<RectTransform>().anchoredPosition = this.GetChildPosition(portals[l].X, portals[l].Y);
			}
			CommonUtils.PrepareEnoughChildren(this.madParticleRoot.transform, this.madParticleTemplate.gameObject, madPoints.Count, null);
			for (int m = 0; m < madPoints.Count; m++)
			{
				Transform child4 = this.madParticleRoot.transform.GetChild(m);
				child4.gameObject.SetActive(true);
				child4.GetComponent<RectTransform>().anchoredPosition = this.GetChildPosition(madPoints[m].X, madPoints[m].Y);
			}
			EasyPool.Free<List<SkillBreakPlateIndex>>(endPoints);
			EasyPool.Free<List<SkillBreakPlateIndex>>(selectedStartPoints);
			EasyPool.Free<List<SkillBreakPlateIndex>>(portals);
			EasyPool.Free<List<SkillBreakPlateIndex>>(madPoints);
		}

		// Token: 0x0600923D RID: 37437 RVA: 0x0044251C File Offset: 0x0044071C
		private Vector2 GetChildPosition(Vector2 gridPosition)
		{
			Rect rect = this.rectTransform.rect;
			return new Vector2(gridPosition.x - rect.width / 2f, gridPosition.y - rect.height / 2f);
		}

		// Token: 0x0600923E RID: 37438 RVA: 0x00442568 File Offset: 0x00440768
		private Vector2 GetChildPosition(int gridX, int gridY)
		{
			return this.GetChildPosition(this._triangleGrid.GetPointPosition(gridX, gridY));
		}

		// Token: 0x0600923F RID: 37439 RVA: 0x00442590 File Offset: 0x00440790
		public Vector2 GetChildWorldPosition(int gridX, int gridY)
		{
			return this.rectTransform.TransformPoint(this.GetChildPosition(gridX, gridY));
		}

		// Token: 0x06009240 RID: 37440 RVA: 0x004425C0 File Offset: 0x004407C0
		private void InitRefers()
		{
			bool refersInitialized = this._refersInitialized;
			if (!refersInitialized)
			{
				this._refersInitialized = true;
			}
		}

		// Token: 0x17000FCE RID: 4046
		// (get) Token: 0x06009241 RID: 37441 RVA: 0x004425E1 File Offset: 0x004407E1
		// (set) Token: 0x06009242 RID: 37442 RVA: 0x004425F0 File Offset: 0x004407F0
		internal bool UseLineFlowEffect
		{
			get
			{
				return this.linesFlowParticle.isActiveAndEnabled;
			}
			set
			{
				bool current = this.linesFlowParticle.gameObject.activeSelf;
				bool flag = current != value;
				if (flag)
				{
					this.linesFlowParticle.gameObject.SetActive(value);
					if (value)
					{
						this.linesFlowParticle.Play();
					}
				}
				this.RefreshLineFlowImageColor();
			}
		}

		// Token: 0x06009243 RID: 37443 RVA: 0x00442648 File Offset: 0x00440848
		private void RefreshLineFlowImageColor()
		{
			int colorIdx = this.UseLineFlowEffect ? 1 : 0;
			bool flag = this.linesFlowImageColors.CheckIndex(colorIdx);
			if (flag)
			{
				this.linesFlowCover.DOColor(this.linesFlowImageColors[colorIdx], 0.1f);
			}
		}

		// Token: 0x06009244 RID: 37444 RVA: 0x00442690 File Offset: 0x00440890
		public void Clear()
		{
			this._lastLineSkillId = -1;
			this._lastInvisibleSet.Clear();
			this.SetLineFlowImageVisible(false);
		}

		// Token: 0x06009245 RID: 37445 RVA: 0x004426B0 File Offset: 0x004408B0
		private void SetLineFlowImageVisible(bool visible)
		{
			GameObject go = this.linesFlowImage.gameObject;
			bool flag = go.activeSelf != visible;
			if (flag)
			{
				go.SetActive(visible);
			}
		}

		// Token: 0x06009246 RID: 37446 RVA: 0x004426E4 File Offset: 0x004408E4
		public void RefreshHighlightSameType(SkillBreakPlateIndex coordinate, bool hovering)
		{
			sbyte targetId = this.DisplayPlate.GetGridAt(coordinate).TemplateId;
			bool flag = SkillBreakGridType.Instance[targetId].Type != ESkillBreakGridTypeType.Special;
			if (!flag)
			{
				foreach (ValueTuple<int, int, int> valueTuple in this.IterAllNodes())
				{
					int x = valueTuple.Item1;
					int y = valueTuple.Item2;
					int index = valueTuple.Item3;
					bool flag2 = this.DisplayPlate.GetGridAt(new ValueTuple<int, int>(x, y)).TemplateId != targetId || (this.DisplayPlate.GetGridAt(new ValueTuple<int, int>(x, y)).State != ESkillBreakGridState.CanSelect && this.DisplayPlate.GetGridAt(new ValueTuple<int, int>(x, y)).State > ESkillBreakGridState.Showed);
					if (!flag2)
					{
						Transform child = this.cellRoot.GetChild(index);
						SkillBreakPlateCellWrapper cell = child.GetComponent<SkillBreakPlateCellWrapper>();
						cell.SetHighlight(hovering);
					}
				}
			}
		}

		// Token: 0x06009247 RID: 37447 RVA: 0x0044280C File Offset: 0x00440A0C
		private IEnumerator OpenUIPlayEffect()
		{
			SkillBreakPlateIndex skillBreakPlateIndex = this.DisplayPlate.Current;
			bool flag = skillBreakPlateIndex.X != -1;
			if (flag)
			{
				this._isEnable = false;
				yield return new WaitForSeconds(1.5f);
				int num;
				for (int i = 0; i < this.effectNormalPathRoot.childCount; i = num + 1)
				{
					Transform child = this.effectNormalPathRoot.GetChild(i);
					bool flag2 = child != null;
					if (flag2)
					{
						Animation anim = child.GetComponentInChildren<Animation>();
						anim.Play("eff_skillbreak_liang_002");
						yield return new WaitForSeconds(0.05f);
						Transform normal = this.normalPathRoot.GetChild(i);
						normal.gameObject.SetActive(true);
						yield return new WaitForSeconds(0.05f);
						anim = null;
						normal = null;
					}
					child = null;
					num = i;
				}
				Transform particleObject = this.effectNormalPathRoot2.Find("EffectNormalPathTemplate2");
				bool flag3 = particleObject == null;
				if (flag3)
				{
					particleObject = Object.Instantiate<RectTransform>(this.effectNormalPathTemplate2, this.effectNormalPathRoot2);
				}
				skillBreakPlateIndex = this.DisplayPlate.Current;
				int x = skillBreakPlateIndex.X;
				skillBreakPlateIndex = this.DisplayPlate.Current;
				Vector2 pos = this.GetChildPosition(x, skillBreakPlateIndex.Y);
				particleObject.GetComponent<RectTransform>().anchoredPosition = pos;
				particleObject.gameObject.SetActive(true);
				particleObject.name = "EffectNormalPathTemplate2";
				yield return new WaitForSeconds(0.6f);
				particleObject.gameObject.SetActive(false);
				particleObject = null;
				pos = default(Vector2);
			}
			yield return null;
			yield break;
		}

		// Token: 0x06009249 RID: 37449 RVA: 0x00442890 File Offset: 0x00440A90
		[CompilerGenerated]
		internal static void <RefreshLinesFlow>g__SetLayerRecursively|37_0(Transform obj)
		{
			int layer = obj.gameObject.layer;
			SkillBreakCellBase breakCellBase = obj.GetComponentInChildren<SkillBreakCellBase>();
			bool flag = breakCellBase != null;
			if (flag)
			{
				SkillBreakCellEmpty breakCellEmpty = breakCellBase as SkillBreakCellEmpty;
				bool flag2 = breakCellEmpty != null;
				if (flag2)
				{
					breakCellEmpty.GetComponent<Graphic>().enabled = true;
				}
				else
				{
					breakCellBase.gameObject.SetActive(false);
				}
			}
			foreach (object obj2 in obj.transform)
			{
				Transform child = (Transform)obj2;
				child.gameObject.layer = layer;
				SkillBreakPlateRenderer.<RefreshLinesFlow>g__SetLayerRecursively|37_0(child);
			}
		}

		// Token: 0x0600924A RID: 37450 RVA: 0x00442954 File Offset: 0x00440B54
		[CompilerGenerated]
		private void <RefreshSelectedPath>g__Add|50_0(SkillBreakPlateRenderer.Segment segment, ref SkillBreakPlateRenderer.<>c__DisplayClass50_0 A_2)
		{
			bool flag = this.IsLongConnection(segment.Start, segment.End);
			if (flag)
			{
				A_2.longSegments.Add(segment);
			}
			else
			{
				A_2.segments.Add(segment);
			}
		}

		// Token: 0x0400708E RID: 28814
		[Tooltip("三角形的底边长")]
		[SerializeField]
		private float triangleBase = 50f;

		// Token: 0x0400708F RID: 28815
		[Tooltip("三角形的高度")]
		[SerializeField]
		private float triangleHeight = 50f * Mathf.Cos(0.5235988f);

		// Token: 0x04007090 RID: 28816
		private SkillBreakTriangleGrid _triangleGrid;

		// Token: 0x04007092 RID: 28818
		private GameData.Domains.Taiwu.SkillBreakPlate _oldPlate;

		// Token: 0x04007093 RID: 28819
		private short _skillId;

		// Token: 0x04007094 RID: 28820
		private int _baseNeedExp;

		// Token: 0x04007095 RID: 28821
		private IAsyncMethodRequestHandler _requestHandler;

		// Token: 0x04007096 RID: 28822
		private LifeSkillShorts _lifeSkillAttainments;

		// Token: 0x04007097 RID: 28823
		private readonly HashSet<SkillBreakPlateIndex> _lastInvisibleSet = new HashSet<SkillBreakPlateIndex>();

		// Token: 0x04007098 RID: 28824
		private int _currentExp;

		// Token: 0x04007099 RID: 28825
		private List<SkillBreakCellBonus> _bonusCells = new List<SkillBreakCellBonus>();

		// Token: 0x0400709A RID: 28826
		private readonly List<SkillBreakPlateIndex> _grayStartPoints = new List<SkillBreakPlateIndex>();

		// Token: 0x0400709B RID: 28827
		[NonSerialized]
		public Action<SkillBreakPlateIndex> OnCellClicked;

		// Token: 0x0400709C RID: 28828
		[NonSerialized]
		public Action<SkillBreakPlateIndex> OnPointerEnter;

		// Token: 0x0400709D RID: 28829
		[NonSerialized]
		public Action<SkillBreakPlateIndex> OnPointerExit;

		// Token: 0x0400709E RID: 28830
		private readonly UIParticlePlayHelper _uiParticlePlayHelper = new UIParticlePlayHelper();

		// Token: 0x0400709F RID: 28831
		private bool _refersInitialized;

		// Token: 0x040070A0 RID: 28832
		private bool _isEnable;

		// Token: 0x040070A1 RID: 28833
		private Coroutine _refreshNodesCo;

		// Token: 0x040070A2 RID: 28834
		private short _lastLineSkillId = -1;

		// Token: 0x040070A3 RID: 28835
		private readonly HashSet<SkillBreakPlateRenderer.Segment> _lastCanSelectShortSegments = new HashSet<SkillBreakPlateRenderer.Segment>();

		// Token: 0x040070A4 RID: 28836
		private const float BaseValue = 1.2f;

		// Token: 0x040070A5 RID: 28837
		[SerializeField]
		private RectTransform linesFlow;

		// Token: 0x040070A6 RID: 28838
		[SerializeField]
		private CRawImage linesFlowImage;

		// Token: 0x040070A7 RID: 28839
		[SerializeField]
		private Graphic linesFlowCover;

		// Token: 0x040070A8 RID: 28840
		[SerializeField]
		private Color[] linesFlowImageColors;

		// Token: 0x040070A9 RID: 28841
		[SerializeField]
		private RectTransform dotsRoot;

		// Token: 0x040070AA RID: 28842
		[SerializeField]
		private UIParticle linesFlowParticle;

		// Token: 0x040070AB RID: 28843
		[Header("原Refers")]
		[SerializeField]
		private List<RectTransform> rangeRootList;

		// Token: 0x040070AC RID: 28844
		[SerializeField]
		private List<GameObject> rangeTemplateList;

		// Token: 0x040070AD RID: 28845
		[SerializeField]
		private UIParticle bonusConnectParticleTemplate;

		// Token: 0x040070AE RID: 28846
		[SerializeField]
		private UIParticle bonusFillParticleTemplate;

		// Token: 0x040070AF RID: 28847
		[SerializeField]
		private RectTransform canSelectLongPathRoot;

		// Token: 0x040070B0 RID: 28848
		[SerializeField]
		private RectTransform canSelectLongPathRoot2;

		// Token: 0x040070B1 RID: 28849
		[SerializeField]
		private RectTransform canSelectLongPathTemplate;

		// Token: 0x040070B2 RID: 28850
		[SerializeField]
		private RectTransform canSelectShortPathRoot;

		// Token: 0x040070B3 RID: 28851
		[SerializeField]
		private RectTransform canSelectShortPathTemplate;

		// Token: 0x040070B4 RID: 28852
		[SerializeField]
		private RectTransform canSelectShortPathWithAnimationRoot;

		// Token: 0x040070B5 RID: 28853
		[SerializeField]
		private RectTransform cellRoot;

		// Token: 0x040070B6 RID: 28854
		[SerializeField]
		private SkillBreakPlateCellWrapper cellTemplate;

		// Token: 0x040070B7 RID: 28855
		[SerializeField]
		private RectTransform endPointParticleRoot;

		// Token: 0x040070B8 RID: 28856
		[SerializeField]
		private RectTransform endPointParticleTemplate;

		// Token: 0x040070B9 RID: 28857
		[SerializeField]
		private RectTransform failPathTemplate;

		// Token: 0x040070BA RID: 28858
		[SerializeField]
		private RectTransform failedPathRoot;

		// Token: 0x040070BB RID: 28859
		[SerializeField]
		private LineRenderer2D lineRendererTemplate;

		// Token: 0x040070BC RID: 28860
		[SerializeField]
		private RectTransform lineRenderersRoot;

		// Token: 0x040070BD RID: 28861
		[SerializeField]
		private RectTransform longPathRoot;

		// Token: 0x040070BE RID: 28862
		[SerializeField]
		private RectTransform longPathTemplate;

		// Token: 0x040070BF RID: 28863
		[SerializeField]
		private RectTransform madParticleRoot;

		// Token: 0x040070C0 RID: 28864
		[SerializeField]
		private UIParticle madParticleTemplate;

		// Token: 0x040070C1 RID: 28865
		[SerializeField]
		private RectTransform normalPathRoot;

		// Token: 0x040070C2 RID: 28866
		[SerializeField]
		private RectTransform normalPathTemplate;

		// Token: 0x040070C3 RID: 28867
		[SerializeField]
		private UIParticle normalSelectedParticleTemplate;

		// Token: 0x040070C4 RID: 28868
		[SerializeField]
		private UIParticle normalVisibleParticleTemplate;

		// Token: 0x040070C5 RID: 28869
		[SerializeField]
		private RectTransform oneShotParticleRoot;

		// Token: 0x040070C6 RID: 28870
		[SerializeField]
		private RectTransform portalParticleRoot;

		// Token: 0x040070C7 RID: 28871
		[SerializeField]
		private UIParticle portalParticleTemplate;

		// Token: 0x040070C8 RID: 28872
		[SerializeField]
		private UIParticle powerChangeParticleTemplate;

		// Token: 0x040070C9 RID: 28873
		[SerializeField]
		private UIParticle specialSelectedParticleTemplate;

		// Token: 0x040070CA RID: 28874
		[SerializeField]
		private UIParticle specialVisibleParticleTemplate;

		// Token: 0x040070CB RID: 28875
		[SerializeField]
		private RectTransform startPointParticleRoot;

		// Token: 0x040070CC RID: 28876
		[SerializeField]
		private UIParticle startPointParticleTemplate;

		// Token: 0x040070CD RID: 28877
		[SerializeField]
		private UIParticle gridTypeChangeParticleTemplate;

		// Token: 0x040070CE RID: 28878
		[SerializeField]
		private UIParticle breakFailParticleTemplate;

		// Token: 0x040070CF RID: 28879
		[SerializeField]
		private RectTransform canSelectLongPathTemplate2;

		// Token: 0x040070D0 RID: 28880
		[SerializeField]
		private RectTransform effectNormalPathRoot;

		// Token: 0x040070D1 RID: 28881
		[SerializeField]
		private RectTransform effectNormalPathRoot2;

		// Token: 0x040070D2 RID: 28882
		[SerializeField]
		private RectTransform effectNormalPathTemplate;

		// Token: 0x040070D3 RID: 28883
		[SerializeField]
		private RectTransform effectNormalPathTemplate2;

		// Token: 0x02002197 RID: 8599
		// (Invoke) Token: 0x0600FB8A RID: 64394
		private delegate void RefreshSegmentStyleFunc(SkillBreakPlateRenderer.Segment segment, GameObject segmentObj);

		// Token: 0x02002198 RID: 8600
		private struct Segment : IEquatable<SkillBreakPlateRenderer.Segment>
		{
			// Token: 0x0600FB8D RID: 64397 RVA: 0x00637CAC File Offset: 0x00635EAC
			public Segment(SkillBreakPlateIndex start, SkillBreakPlateIndex end)
			{
				this.Start = start;
				this.End = end;
			}

			// Token: 0x0600FB8E RID: 64398 RVA: 0x00637CC0 File Offset: 0x00635EC0
			public bool Equals(SkillBreakPlateRenderer.Segment other)
			{
				return this.Start.Equals(other.Start) && this.End.Equals(other.End);
			}

			// Token: 0x0600FB8F RID: 64399 RVA: 0x00637CFC File Offset: 0x00635EFC
			public override bool Equals(object obj)
			{
				bool result;
				if (obj is SkillBreakPlateRenderer.Segment)
				{
					SkillBreakPlateRenderer.Segment other = (SkillBreakPlateRenderer.Segment)obj;
					result = this.Equals(other);
				}
				else
				{
					result = false;
				}
				return result;
			}

			// Token: 0x0600FB90 RID: 64400 RVA: 0x00637D28 File Offset: 0x00635F28
			public override int GetHashCode()
			{
				return HashCode.Combine<SkillBreakPlateIndex, SkillBreakPlateIndex>(this.Start, this.End);
			}

			// Token: 0x0400D647 RID: 54855
			public SkillBreakPlateIndex Start;

			// Token: 0x0400D648 RID: 54856
			public SkillBreakPlateIndex End;
		}
	}
}
