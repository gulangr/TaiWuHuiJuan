using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using Game.Components.LineGenerator;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x02000725 RID: 1829
	public class AreaMap : MonoBehaviour
	{
		// Token: 0x06005764 RID: 22372 RVA: 0x00289684 File Offset: 0x00287884
		private void RegisterPrefabs()
		{
			bool prefabRegistered = this._prefabRegistered;
			if (!prefabRegistered)
			{
				this._prefabRegistered = true;
				object countLock = AreaMap.CountLock;
				lock (countLock)
				{
					bool flag2 = AreaMap._enabledCount++ == 0;
					if (flag2)
					{
						PoolManager.SetSrcObject("Game.Views.World.AreaMap.AreaStateItemKey", this.areaStatePrefab.gameObject);
					}
				}
			}
		}

		// Token: 0x06005765 RID: 22373 RVA: 0x00289700 File Offset: 0x00287900
		private void UnregisterPrefabs()
		{
			object countLock = AreaMap.CountLock;
			lock (countLock)
			{
				bool flag2 = --AreaMap._enabledCount == 0;
				if (flag2)
				{
					PoolManager.RemoveData("Game.Views.World.AreaMap.AreaStateItemKey");
				}
			}
		}

		// Token: 0x06005766 RID: 22374 RVA: 0x0028975C File Offset: 0x0028795C
		public void BeforeScale(bool centerZoom = false)
		{
			UIRectDragMove uirectDragMove = this.dragMove;
			MouseWheelScale.BeforeScale(((uirectDragMove != null) ? uirectDragMove.transform : null) as RectTransform, centerZoom);
		}

		// Token: 0x06005767 RID: 22375 RVA: 0x0028977C File Offset: 0x0028797C
		public void OnScale(float invAmplifyFactor, float invNameAmplifyFactor = -1f)
		{
			bool flag = this.generator;
			if (flag)
			{
				this.generator.SetWidth(this.lineWidth * invAmplifyFactor);
			}
			bool flag2 = this.isTravel || this.isNameScaleEnable;
			if (flag2)
			{
				foreach (State state in this.states)
				{
					foreach (Area area in state.areas)
					{
						area.SetNameScale((invNameAmplifyFactor < 0f) ? invAmplifyFactor : invNameAmplifyFactor);
					}
				}
			}
			UIRectDragMove uirectDragMove = this.dragMove;
			RectTransform rectTransform = ((uirectDragMove != null) ? uirectDragMove.transform : null) as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetPivot(new Vector2(0.5f, 0.5f));
			}
			bool flag3 = this.dragMove;
			if (flag3)
			{
				Action afterClampCallback = this.dragMove.AfterClampCallback;
				if (afterClampCallback != null)
				{
					afterClampCallback();
				}
			}
		}

		// Token: 0x06005768 RID: 22376 RVA: 0x00289870 File Offset: 0x00287A70
		public void RefreshShowArea(Func<short, bool> showArea)
		{
			Func<Area, bool> <>9__0;
			foreach (State state in this.states)
			{
				state.ShowIcon(showArea);
				IEnumerable<Area> areas = state.areas;
				Func<Area, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((Area area) => showArea(area.Config.TemplateId)));
				}
				bool active = areas.Any(predicate);
				state.gameObject.SetActive(active);
			}
			CToggleGroup ctoggleGroup = this.stateInfoToggleGroup;
			int? num = (ctoggleGroup != null) ? new int?(ctoggleGroup.GetActiveIndex()) : null;
			bool flag;
			if (num != null)
			{
				int idx = num.GetValueOrDefault();
				flag = (idx == -1 || !this.stateInfoToggleGroup.Get(idx).gameObject.activeInHierarchy);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				foreach (ValueTuple<CToggle, int> valueTuple in this.stateInfoToggleGroup.GetAll().Select((CToggle x, int i) => new ValueTuple<CToggle, int>(x, i)))
				{
					CToggle state2 = valueTuple.Item1;
					int k = valueTuple.Item2;
					bool activeInHierarchy = state2.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						this.stateInfoToggleGroup.Set(k, true);
						break;
					}
				}
			}
		}

		// Token: 0x06005769 RID: 22377 RVA: 0x002899F0 File Offset: 0x00287BF0
		public void RefreshShowAreaState(bool showAreaState)
		{
			foreach (State state in this.states)
			{
				state.ShowAreaState(showAreaState);
			}
		}

		// Token: 0x17000A88 RID: 2696
		private Area this[int id]
		{
			get
			{
				return (id < 135) ? this.states[id / 9][id % 9] : this.selector.Get(id).GetComponent<Area>();
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x0600576B RID: 22379 RVA: 0x00289A51 File Offset: 0x00287C51
		// (set) Token: 0x0600576C RID: 22380 RVA: 0x00289A5C File Offset: 0x00287C5C
		public short HighlightedAreaTemplateId
		{
			get
			{
				return this._highlightedAreaTemplateId;
			}
			set
			{
				bool flag = this._highlightedAreaTemplateId >= 0;
				if (flag)
				{
					foreach (State state in this.states)
					{
						foreach (Area area in state.areas)
						{
							bool flag2 = area.Config.TemplateId == this._highlightedAreaTemplateId;
							if (flag2)
							{
								area.SetHighlight(false);
							}
						}
					}
				}
				this._highlightedAreaTemplateId = value;
				bool flag3 = value >= 0;
				if (flag3)
				{
					foreach (State state2 in this.states)
					{
						foreach (Area area2 in state2.areas)
						{
							bool flag4 = area2.Config.TemplateId == value;
							if (flag4)
							{
								area2.SetHighlight(true);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600576D RID: 22381 RVA: 0x00289B53 File Offset: 0x00287D53
		public void ClearHighlight()
		{
			this.HighlightedAreaTemplateId = -1;
		}

		// Token: 0x0600576E RID: 22382 RVA: 0x00289B60 File Offset: 0x00287D60
		public void SelectedAreaTemplateIdWithoutNotify(short value)
		{
			foreach (State state in this.states)
			{
				foreach (Area area in state.areas)
				{
					bool flag = area.Config.TemplateId == value;
					if (flag)
					{
						area.toggle.SetIsOnWithoutNotify(true);
						area.RefreshHighlight();
						this.selector.NotifyToggle(area.toggle, true, false);
						this._selectedAreaTemplateId = value;
						return;
					}
				}
			}
			this._selectedAreaTemplateId = -1;
			this.selector.DeSelect(false);
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x0600576F RID: 22383 RVA: 0x00289C06 File Offset: 0x00287E06
		// (set) Token: 0x06005770 RID: 22384 RVA: 0x00289C10 File Offset: 0x00287E10
		public short SelectedAreaTemplateId
		{
			get
			{
				return this._selectedAreaTemplateId;
			}
			set
			{
				foreach (State state in this.states)
				{
					foreach (Area area in state.areas)
					{
						bool flag = area.Config.TemplateId == value;
						if (flag)
						{
							area.toggle.isOn = true;
							this.selector.NotifyToggle(area.toggle, true, true);
							this._selectedAreaTemplateId = value;
							return;
						}
					}
				}
				this._selectedAreaTemplateId = -1;
				this.selector.DeSelect(false);
			}
		}

		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x06005771 RID: 22385 RVA: 0x00289CAE File Offset: 0x00287EAE
		public short SelectedAreaId
		{
			get
			{
				return this.MapModel.GetAreaIdByAreaTemplateId(this._selectedAreaTemplateId);
			}
		}

		// Token: 0x06005772 RID: 22386 RVA: 0x00289CC4 File Offset: 0x00287EC4
		public void RegisterMouseHoverEvent(Action<short> onMouseEnterAreaTemplateId, Action<short> onMouseExitAreaTemplateId)
		{
			this.OnMouseEnterAreaTemplateId = onMouseEnterAreaTemplateId;
			this.OnMouseExitAreaTemplateId = onMouseExitAreaTemplateId;
			foreach (State state in this.states)
			{
				foreach (Area area in state.areas)
				{
					area.map = this;
				}
			}
		}

		// Token: 0x06005773 RID: 22387 RVA: 0x00289D24 File Offset: 0x00287F24
		private void Awake()
		{
			this.OnScale(1f / base.transform.localScale.x, -1f);
			bool flag = this.stateInfoToggleGroup;
			if (flag)
			{
				this.stateInfoToggleGroup.Init(-1);
			}
			this.RegisterPrefabs();
			this.selector.allowSwitchOff = (this.selector.allowUncheck = true);
			this.selector.OnActiveIndexChange += delegate(int newTog, int _)
			{
				bool flag3 = newTog == -1;
				if (flag3)
				{
					this._selectedAreaTemplateId = -1;
				}
				else
				{
					bool flag4 = this.OnSelectAreaTemplateId == null && this.OnSelectArea == null;
					if (flag4)
					{
						this.selector.DeSelect(false);
					}
					else
					{
						this._selectedAreaTemplateId = this[newTog].Config.TemplateId;
						Action<short> onSelectAreaTemplateId = this.OnSelectAreaTemplateId;
						if (onSelectAreaTemplateId != null)
						{
							onSelectAreaTemplateId(this._selectedAreaTemplateId);
						}
						Action<short> onSelectArea = this.OnSelectArea;
						if (onSelectArea != null)
						{
							onSelectArea(this.MapModel.GetAreaIdByAreaTemplateId(this._selectedAreaTemplateId));
						}
					}
				}
			};
			this.selector.Init(-1);
			bool flag2 = !this.dragMove;
			if (!flag2)
			{
				this.dragMove.BeginDragCallback = new Action(this.OnDragBegin);
				this.dragMove.EndDragCallback = new Action(this.OnDragEnd);
			}
		}

		// Token: 0x06005774 RID: 22388 RVA: 0x00289DF8 File Offset: 0x00287FF8
		private void OnDestroy()
		{
			this.UnregisterPrefabs();
		}

		// Token: 0x06005775 RID: 22389 RVA: 0x00289E04 File Offset: 0x00288004
		private void OnEnable()
		{
			bool flag = this.dragMove;
			if (flag)
			{
				base.StartCoroutine(this.AdjustLocation);
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06005776 RID: 22390 RVA: 0x00289E30 File Offset: 0x00288030
		private IEnumerator AdjustLocation
		{
			get
			{
				yield return null;
				this.dragMove.AdjustOffsetAfterScale();
				yield break;
			}
		}

		// Token: 0x06005777 RID: 22391 RVA: 0x00289E50 File Offset: 0x00288050
		private void OnDragBegin()
		{
			foreach (State state in this.states)
			{
				state.OnDragBegin();
			}
			AreaMap.<OnDragBegin>g__SetInfinityLoop|53_0();
		}

		// Token: 0x06005778 RID: 22392 RVA: 0x00289E88 File Offset: 0x00288088
		private void OnDragEnd()
		{
			UIManager.Instance.SetEscHandler(null);
			foreach (State state in this.states)
			{
				state.OnDragEnd();
			}
		}

		// Token: 0x06005779 RID: 22393 RVA: 0x00289EC4 File Offset: 0x002880C4
		public void Init(bool canSelectBrokenArea, IAsyncMethodRequestHandler dataHandler, bool requestData = true)
		{
			AreaStateItemController.Checker.Set(SingletonObject.getInstance<GlobalSettings>());
			this._dataHandler = dataHandler;
			this._selectedAreaTemplateId = -1;
			this.MapModel = SingletonObject.getInstance<WorldMapModel>();
			this._canSelectBrokenArea = canSelectBrokenArea;
			this.RefreshTaiwuLocation();
			if (requestData)
			{
				this.RequestData();
			}
		}

		// Token: 0x0600577A RID: 22394 RVA: 0x00289F15 File Offset: 0x00288115
		public void RequestData()
		{
			AreaMap.RequestData(this._dataHandler, new Action<AreaDisplayData[]>(this.Refresh));
		}

		// Token: 0x0600577B RID: 22395 RVA: 0x00289F30 File Offset: 0x00288130
		public static void RequestData(IAsyncMethodRequestHandler dataHandler, Action<AreaDisplayData[]> callback)
		{
			MapDomainMethod.AsyncCall.GetAllAreaDisplayData(dataHandler, delegate(int offset, RawDataPool pool)
			{
				AreaDisplayData[] data = Array.Empty<AreaDisplayData>();
				Serializer.Deserialize(pool, offset, ref data);
				callback(data);
			});
		}

		// Token: 0x0600577C RID: 22396 RVA: 0x00289F60 File Offset: 0x00288160
		public void Refresh(AreaDisplayData[] data)
		{
			this.RegisterPrefabs();
			Action<AreaDisplayData[]> postRenderStart = this.PostRenderStart;
			if (postRenderStart != null)
			{
				postRenderStart(data);
			}
			foreach (State state in this.states)
			{
				state.Init(this._canSelectBrokenArea, this.isTravel, data, this.MapModel);
			}
			WorldMapModel map = SingletonObject.getInstance<WorldMapModel>();
			this.TryRefreshRoute(MapArea.Instance.Select(delegate(MapAreaItem x)
			{
				WorldMapModel map = map;
				short areaId = (map != null) ? map.GetAreaIdByAreaTemplateId(x.TemplateId) : -1;
				return data.CheckIndex((int)areaId) && data[(int)areaId].IsUnlocked;
			}).ToArray<bool>());
			Action<AreaDisplayData[]> postRenderEnd = this.PostRenderEnd;
			if (postRenderEnd != null)
			{
				postRenderEnd(data);
			}
		}

		// Token: 0x0600577D RID: 22397 RVA: 0x0028A018 File Offset: 0x00288218
		public void CancelSelect()
		{
			this.selector.DeSelect(false);
		}

		// Token: 0x0600577E RID: 22398 RVA: 0x0028A028 File Offset: 0x00288228
		private void RefreshTaiwuLocation()
		{
			int currArea = (int)((this.MapModel.CurrentAreaId == -1) ? -1 : this.MapModel.Areas[(int)this.MapModel.CurrentAreaId].GetTemplateId());
			bool flag = this.taiwuLocation;
			if (flag)
			{
				this.taiwuLocation.gameObject.SetActive(false);
			}
			foreach (State state in this.states)
			{
				bool flag2 = state.RefreshTaiwuLocation(currArea, this.taiwuLocation);
				if (flag2)
				{
					break;
				}
			}
		}

		// Token: 0x0600577F RID: 22399 RVA: 0x0028A0B8 File Offset: 0x002882B8
		public void LookAt(Vector2 position, float duration = 0f, float finalScale = -1f, Vector2 tolerance = default(Vector2))
		{
			bool flag = !this.self;
			if (!flag)
			{
				this.self.SetPivot(new Vector2(0.5f, 0.5f));
				bool flag2 = tolerance != default(Vector2);
				if (flag2)
				{
					Vector2 currLoc = this.self.anchoredPosition / -this.self.localScale.x;
					Vector2 diff = position - currLoc;
					position = currLoc + new Vector2(Mathf.Sign(diff.x) * Mathf.Max(0f, Mathf.Abs(diff.x) - tolerance.x / Mathf.Max(0.01f, this.self.localScale.x)), Mathf.Sign(diff.y) * Mathf.Max(0f, Mathf.Abs(diff.y) - tolerance.y / Mathf.Max(0.01f, this.self.localScale.y)));
				}
				bool flag3 = duration > 0f;
				if (flag3)
				{
					this.self.DOAnchorPos(position * -((finalScale < 0f) ? this.self.localScale.x : finalScale), duration, false).OnUpdate(new TweenCallback(this.dragMove.AdjustOffsetAfterScale));
				}
				else
				{
					this.self.anchoredPosition = position * -this.self.localScale.x;
					bool activeInHierarchy = base.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						base.StartCoroutine(this.AdjustLocation);
					}
				}
				bool flag4 = this.dragMove;
				if (flag4)
				{
					Action afterClampCallback = this.dragMove.AfterClampCallback;
					if (afterClampCallback != null)
					{
						afterClampCallback();
					}
				}
			}
		}

		// Token: 0x06005780 RID: 22400 RVA: 0x0028A298 File Offset: 0x00288498
		public void LookAt(short areaId, float duration = 0f, Vector2 tolerance = default(Vector2))
		{
			bool flag = !this.MapModel.Areas.CheckIndex((int)areaId);
			if (!flag)
			{
				this.LookAtTemplate(this.MapModel.Areas[(int)areaId].GetTemplateId(), duration, tolerance);
			}
		}

		// Token: 0x06005781 RID: 22401 RVA: 0x0028A2DC File Offset: 0x002884DC
		public void LookAtTemplate(short areaTemplateId, float duration = 0f, Vector2 tolerance = default(Vector2))
		{
			bool flag = this.stateInfoToggleGroup;
			if (flag)
			{
				Func<Area, bool> <>9__0;
				foreach (State state in this.states)
				{
					IEnumerable<Area> areas = state.areas;
					Func<Area, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((Area area) => area.Config.TemplateId == areaTemplateId));
					}
					bool flag2 = areas.Any(predicate);
					if (flag2)
					{
						this.stateInfoToggleGroup.Set((int)(state.templateId - 1), false);
						break;
					}
				}
			}
			else
			{
				this.LookAt(this.GetLocation(areaTemplateId), duration, -1f, tolerance);
			}
		}

		// Token: 0x06005782 RID: 22402 RVA: 0x0028A38C File Offset: 0x0028858C
		public Vector2 GetLocation(short areaTemplateId)
		{
			foreach (State state in this.states)
			{
				foreach (Area area in state.areas)
				{
					bool flag = area.Config.TemplateId == areaTemplateId;
					if (flag)
					{
						return area.transform.localPosition + state.transform.localPosition;
					}
				}
			}
			return this.ExtraLocationDict.GetValueOrDefault(areaTemplateId);
		}

		// Token: 0x06005783 RID: 22403 RVA: 0x0028A420 File Offset: 0x00288620
		public Transform GetTransform(short areaTemplateId)
		{
			return (from state in this.states
			from area in state.areas
			select new
			{
				state,
				area
			} into t
			where t.area.Config.TemplateId == areaTemplateId
			select t.area.transform).FirstOrDefault<Transform>();
		}

		// Token: 0x06005784 RID: 22404 RVA: 0x0028A4C0 File Offset: 0x002886C0
		public void LookAtTaiwuVillage()
		{
			this.LookAt(this.MapModel.GetTaiwuVillageAreaId(), 0f, default(Vector2));
		}

		// Token: 0x06005785 RID: 22405 RVA: 0x0028A4F0 File Offset: 0x002886F0
		public void SetStateHover(bool enable)
		{
			foreach (State state in this.states)
			{
				StateMask stateMask = state.stateMask;
				if (stateMask != null)
				{
					stateMask.SetEnabled(enable);
				}
			}
		}

		// Token: 0x06005786 RID: 22406 RVA: 0x0028A52B File Offset: 0x0028872B
		public void SetAreaInteractable(short areaId, bool interactable)
		{
			this.SetAreaTemplateInteractable(this.MapModel.Areas[(int)areaId].GetTemplateId(), interactable, true);
		}

		// Token: 0x06005787 RID: 22407 RVA: 0x0028A548 File Offset: 0x00288748
		public void SetAreaTemplateInteractable(short areaTemplateId, bool interactable, bool alsoSetInteractable = true)
		{
			foreach (State state in this.states)
			{
				foreach (Area area in state.areas)
				{
					bool flag = area.Config.TemplateId == areaTemplateId;
					if (flag)
					{
						area.SetStyle(!interactable, alsoSetInteractable);
					}
				}
			}
		}

		// Token: 0x06005788 RID: 22408 RVA: 0x0028A5B4 File Offset: 0x002887B4
		private static bool CheckUnlock(int routeTemplateId, bool[] unlockStatus)
		{
			MapRouteItem route = MapRoute.Instance[routeTemplateId];
			return (unlockStatus.CheckIndex((int)route.FromId) ? unlockStatus[(int)route.FromId] : route.ExtraFromId.Any((short x) => unlockStatus[(int)x])) && (unlockStatus.CheckIndex((int)route.ToId) ? unlockStatus[(int)route.ToId] : route.ExtraToId.Any((short x) => unlockStatus[(int)x]));
		}

		// Token: 0x06005789 RID: 22409 RVA: 0x0028A658 File Offset: 0x00288858
		private void TryRefreshRoute(bool[] unlockStatus)
		{
			bool flag = !this.roadHolder;
			if (!flag)
			{
				foreach (KeyValuePair<short, short> keyValuePair in this._roadIndex)
				{
					short num;
					short num2;
					keyValuePair.Deconstruct(out num, out num2);
					short i = num;
					short v = num2;
					this.roadHolder.GetChild((int)v).GetComponent<CImage>().gameObject.SetActive(false);
					bool flag2 = this.roadHighlightHolder;
					if (flag2)
					{
						this.roadHighlightHolder.GetChild((int)v).GetComponent<CImage>().gameObject.SetActive(false);
					}
					this._index.Add(i);
					this._inactive.Add(v);
				}
				this._roadIndex.Clear();
				foreach (MapRouteItem route in ((IEnumerable<MapRouteItem>)MapRoute.Instance))
				{
					if (route == null)
					{
						goto IL_10A;
					}
					float[] loc = route.PathLoc;
					if (loc == null)
					{
						goto IL_10A;
					}
					bool flag3 = loc.Length == 2;
					IL_10B:
					bool flag4 = flag3;
					if (flag4)
					{
						CImage highlight = null;
						List<short> inactive = this._inactive;
						bool flag5 = inactive != null && inactive.Count > 0;
						CImage road;
						if (flag5)
						{
							List<short> inactive2 = this._inactive;
							short index = inactive2[inactive2.Count - 1];
							this._inactive.RemoveAt(this._inactive.Count - 1);
							road = this.roadHolder.GetChild((int)index).GetComponent<CImage>();
							road.gameObject.SetActive(true);
							bool flag6 = this.roadHighlightHolder;
							if (flag6)
							{
								highlight = this.roadHighlightHolder.GetChild((int)index).GetComponent<CImage>();
								highlight.gameObject.SetActive(true);
							}
						}
						else
						{
							road = new GameObject("Road").AddComponent<CImage>();
							road.color = this.darkRoadColor;
							road.transform.SetParent(this.roadHolder, false);
							road.raycastTarget = false;
							RectTransform tf = road.transform as RectTransform;
							tf.anchorMin = (tf.anchorMax = Vector2.one * 0.5f);
							bool flag7 = this.roadHighlightHolder;
							if (flag7)
							{
								highlight = new GameObject("Road").AddComponent<CImage>();
								highlight.color = this.brightRoadColor;
								highlight.transform.SetParent(this.roadHighlightHolder, false);
								highlight.transform.localScale = (road.transform.localScale = Vector3.one);
								highlight.raycastTarget = false;
								tf = (highlight.transform as RectTransform);
								tf.anchorMin = (tf.anchorMax = Vector2.one * 0.5f);
							}
						}
						road.SetSprite(this.roadHighlightHolder ? ("ui9_back_largemap_gray_road_" + route.InternalName) : ("ui9_back_largemap_colorful_road_" + route.InternalName), false, null);
						road.SetNativeSize();
						road.transform.localPosition = new Vector2(loc[0], loc[1]);
						bool flag8 = this.roadHighlightHolder;
						if (flag8)
						{
							highlight.SetSprite("ui9_back_largemap_colorful_road_" + route.InternalName, false, null);
							highlight.SetNativeSize();
							highlight.transform.localPosition = new Vector2(loc[0], loc[1]);
							highlight.gameObject.SetActive(AreaMap.CheckUnlock((int)route.TemplateId, unlockStatus));
						}
						this._roadIndex[route.TemplateId] = (short)road.transform.GetSiblingIndex();
						continue;
					}
					continue;
					IL_10A:
					flag3 = false;
					goto IL_10B;
				}
			}
		}

		// Token: 0x0600578D RID: 22413 RVA: 0x0028ABEA File Offset: 0x00288DEA
		[CompilerGenerated]
		internal static void <OnDragBegin>g__SetInfinityLoop|53_0()
		{
			UIManager.Instance.SetEscHandler(new Action(AreaMap.<OnDragBegin>g__SetInfinityLoop|53_0));
		}

		// Token: 0x04003BDE RID: 15326
		public Action<AreaDisplayData[]> PostRenderStart = null;

		// Token: 0x04003BDF RID: 15327
		public Action<Area, AreaDisplayData> PostRender = null;

		// Token: 0x04003BE0 RID: 15328
		public Action<AreaDisplayData[]> PostRenderEnd = null;

		// Token: 0x04003BE1 RID: 15329
		public const string AreaStateItemKey = "Game.Views.World.AreaMap.AreaStateItemKey";

		// Token: 0x04003BE2 RID: 15330
		private static readonly object CountLock = new object();

		// Token: 0x04003BE3 RID: 15331
		private static int _enabledCount = 0;

		// Token: 0x04003BE4 RID: 15332
		[SerializeField]
		private AreaStateItem areaStatePrefab;

		// Token: 0x04003BE5 RID: 15333
		private bool _prefabRegistered;

		// Token: 0x04003BE6 RID: 15334
		[SerializeField]
		private bool isTravel;

		// Token: 0x04003BE7 RID: 15335
		[Tooltip("是否启用名称缩放，启用后会在缩放地图时会根据比例调整名称缩放，关闭时大小随地图缩放。（isTravel为true时强制有效）")]
		[SerializeField]
		private bool isNameScaleEnable;

		// Token: 0x04003BE8 RID: 15336
		[SerializeField]
		internal CToggleGroup selector;

		// Token: 0x04003BE9 RID: 15337
		[SerializeField]
		private RectTransform self;

		// Token: 0x04003BEA RID: 15338
		[SerializeField]
		private RectTransform stateHolder;

		// Token: 0x04003BEB RID: 15339
		[SerializeField]
		private RectTransform stateMaskHolder;

		// Token: 0x04003BEC RID: 15340
		[SerializeField]
		internal State[] states;

		// Token: 0x04003BED RID: 15341
		[SerializeField]
		internal Area areaPrefab;

		// Token: 0x04003BEE RID: 15342
		[SerializeField]
		private PositionFollower taiwuLocation;

		// Token: 0x04003BEF RID: 15343
		[CanBeNull]
		[SerializeField]
		internal UIRectDragMove dragMove;

		// Token: 0x04003BF0 RID: 15344
		[SerializeField]
		public Game.Components.LineGenerator.Line2DGenerator generator;

		// Token: 0x04003BF1 RID: 15345
		[SerializeField]
		private float lineWidth = 8f;

		// Token: 0x04003BF2 RID: 15346
		[SerializeField]
		internal string areaStateItemSpriteSuffix = "";

		// Token: 0x04003BF3 RID: 15347
		[CanBeNull]
		[SerializeField]
		private CToggleGroup stateInfoToggleGroup;

		// Token: 0x04003BF4 RID: 15348
		[UsedImplicitly]
		[NonSerialized]
		public Action<short> OnSelectAreaTemplateId;

		// Token: 0x04003BF5 RID: 15349
		[NonSerialized]
		public Action<short> OnSelectArea;

		// Token: 0x04003BF6 RID: 15350
		[NonSerialized]
		public Action<short> OnMouseEnterAreaTemplateId;

		// Token: 0x04003BF7 RID: 15351
		[NonSerialized]
		public Action<short> OnMouseExitAreaTemplateId;

		// Token: 0x04003BF8 RID: 15352
		[NonSerialized]
		public WorldMapModel MapModel;

		// Token: 0x04003BF9 RID: 15353
		private short _selectedAreaTemplateId = -1;

		// Token: 0x04003BFA RID: 15354
		private short _highlightedAreaTemplateId = -1;

		// Token: 0x04003BFB RID: 15355
		private bool _canSelectBrokenArea;

		// Token: 0x04003BFC RID: 15356
		private IAsyncMethodRequestHandler _dataHandler;

		// Token: 0x04003BFD RID: 15357
		public Dictionary<short, Vector2> ExtraLocationDict = new Dictionary<short, Vector2>();

		// Token: 0x04003BFE RID: 15358
		[SerializeField]
		private Color brightRoadColor = new Color(1f, 1f, 1f, 0.43137255f);

		// Token: 0x04003BFF RID: 15359
		[SerializeField]
		private Color darkRoadColor = new Color(1f, 1f, 1f, 1f);

		// Token: 0x04003C00 RID: 15360
		[CanBeNull]
		[SerializeField]
		private UnityEngine.Material highlightMaterial;

		// Token: 0x04003C01 RID: 15361
		[CanBeNull]
		[SerializeField]
		private RectTransform roadHolder;

		// Token: 0x04003C02 RID: 15362
		[CanBeNull]
		[SerializeField]
		private RectTransform roadHighlightHolder;

		// Token: 0x04003C03 RID: 15363
		private readonly Dictionary<short, short> _roadIndex = new Dictionary<short, short>();

		// Token: 0x04003C04 RID: 15364
		private readonly List<short> _index = new List<short>();

		// Token: 0x04003C05 RID: 15365
		private readonly List<short> _inactive = new List<short>();
	}
}
