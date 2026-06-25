using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FrameWork;
using Game.Views.World;
using GameData.Domains.Map;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A0 RID: 2464
	public class JieQingAreaMapInterface : MonoBehaviour
	{
		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06007682 RID: 30338 RVA: 0x003738E8 File Offset: 0x00371AE8
		// (remove) Token: 0x06007683 RID: 30339 RVA: 0x00373920 File Offset: 0x00371B20
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Vector3> OnScale;

		// Token: 0x06007684 RID: 30340 RVA: 0x00373958 File Offset: 0x00371B58
		public void InitAreas()
		{
			this.EnsureReferences();
			bool flag = !this.areaMap;
			if (flag)
			{
				Debug.LogWarning("[JieQingAreaMapInterface] AreaMap is missing.", this);
			}
			else
			{
				if (this._mapModel == null)
				{
					this._mapModel = SingletonObject.getInstance<WorldMapModel>();
				}
				bool flag2 = !this._initialized;
				if (flag2)
				{
					this.areaMap.Init(true, null, false);
					this.areaMap.PostRender = new Action<Area, AreaDisplayData>(this.HandlePostRender);
					this._initialized = true;
				}
				bool flag3 = this.scaler && !this._scaleHooked;
				if (flag3)
				{
					MouseWheelScale mouseWheelScale = this.scaler;
					mouseWheelScale.OnScale = (Action<Vector3>)Delegate.Combine(mouseWheelScale.OnScale, new Action<Vector3>(this.HandleScale));
					this._scaleHooked = true;
				}
			}
		}

		// Token: 0x06007685 RID: 30341 RVA: 0x00373A30 File Offset: 0x00371C30
		public Transform GetStateHighlightEffect(short organizationId)
		{
			int index = (int)(organizationId - 1);
			bool flag = index >= 0 && index < this.stateHighlights.Length;
			Transform result;
			if (flag)
			{
				result = this.stateHighlights[index];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06007686 RID: 30342 RVA: 0x00373A68 File Offset: 0x00371C68
		public void Refresh(AreaDisplayData[] data)
		{
			bool flag = !this._initialized;
			if (flag)
			{
				this.InitAreas();
			}
			bool flag2 = !this.areaMap || data == null;
			if (!flag2)
			{
				this.areaMap.Refresh(data);
				this.areaMap.RefreshShowAreaState(false);
			}
		}

		// Token: 0x06007687 RID: 30343 RVA: 0x00373AC0 File Offset: 0x00371CC0
		public void FocusArea(short areaId)
		{
			bool flag = !this._initialized;
			if (flag)
			{
				this.InitAreas();
			}
			bool flag2 = !this.areaMap || areaId < 0;
			if (!flag2)
			{
				this.areaMap.LookAt(areaId, 0f, default(Vector2));
			}
		}

		// Token: 0x06007688 RID: 30344 RVA: 0x00373B18 File Offset: 0x00371D18
		public void SetPointerVisible(bool visible)
		{
			this.EnsureReferences();
			bool flag = this.currPosPointer;
			if (flag)
			{
				this.currPosPointer.gameObject.SetActive(visible);
			}
		}

		// Token: 0x06007689 RID: 30345 RVA: 0x00373B50 File Offset: 0x00371D50
		public JieQingAreaHelper GetAreaHelper(short areaId)
		{
			JieQingAreaHelper helper;
			this._helpers.TryGetValue(areaId, out helper);
			return helper;
		}

		// Token: 0x0600768A RID: 30346 RVA: 0x00373B72 File Offset: 0x00371D72
		public IEnumerable<JieQingAreaHelper> GetAllAreaHelpers()
		{
			return from helper in this._helpers.Values
			where helper
			select helper;
		}

		// Token: 0x0600768B RID: 30347 RVA: 0x00373BA4 File Offset: 0x00371DA4
		public void UpdateFixedScaleItems(Vector3 scale)
		{
			foreach (JieQingAreaHelper helper in this._helpers.Values)
			{
				bool flag = helper;
				if (flag)
				{
					helper.UpdateFixedScaleItems(scale);
				}
			}
		}

		// Token: 0x0600768C RID: 30348 RVA: 0x00373C0C File Offset: 0x00371E0C
		private void OnDestroy()
		{
			bool flag = this.scaler && this._scaleHooked;
			if (flag)
			{
				MouseWheelScale mouseWheelScale = this.scaler;
				mouseWheelScale.OnScale = (Action<Vector3>)Delegate.Remove(mouseWheelScale.OnScale, new Action<Vector3>(this.HandleScale));
			}
		}

		// Token: 0x0600768D RID: 30349 RVA: 0x00373C5C File Offset: 0x00371E5C
		public void HideAllStateHighlights()
		{
			foreach (RectTransform highlight in this.stateHighlights)
			{
				bool flag = highlight;
				if (flag)
				{
					highlight.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600768E RID: 30350 RVA: 0x00373CA0 File Offset: 0x00371EA0
		private void EnsureReferences()
		{
			if (this.areaMap == null)
			{
				this.areaMap = base.GetComponentInChildren<AreaMap>(true);
			}
			if (this.scaler == null)
			{
				this.scaler = base.GetComponentInChildren<MouseWheelScale>(true);
			}
			if (this.currPosPointer == null)
			{
				this.currPosPointer = base.GetComponentsInChildren<RectTransform>(true).FirstOrDefault((RectTransform rect) => rect.name == "CurrPosPointer");
			}
		}

		// Token: 0x0600768F RID: 30351 RVA: 0x00373D14 File Offset: 0x00371F14
		private void HandleScale(Vector3 scale)
		{
			bool flag = this.areaMap;
			if (flag)
			{
				float nameScaleFactor = TargetScaleAlign.GetAlignScale(scale.x, new Vector2(0.5f, 1f), new Vector2(1.5f, 1f), false);
				this.areaMap.OnScale(1f / Mathf.Max(0.01f, scale.x), nameScaleFactor);
			}
			Action<Vector3> onScale = this.OnScale;
			if (onScale != null)
			{
				onScale(scale);
			}
		}

		// Token: 0x06007690 RID: 30352 RVA: 0x00373D94 File Offset: 0x00371F94
		private void HandlePostRender(Area area, AreaDisplayData displayData)
		{
			if (this._mapModel == null)
			{
				this._mapModel = SingletonObject.getInstance<WorldMapModel>();
			}
			bool flag = this._mapModel == null;
			if (!flag)
			{
				short areaId = this._mapModel.GetAreaIdByAreaTemplateId(area.Config.TemplateId);
				bool flag2 = areaId < 0;
				if (!flag2)
				{
					JieQingAreaHelper helper = this.GetOrCreateHelper(area, areaId);
					helper.Init(area, areaId);
					JieQingAreaMapInterface.ConfigureTip(area, areaId, displayData);
					bool flag3 = this._initializedAreaIds.Add(areaId);
					if (flag3)
					{
						Action<JieQingAreaHelper, short> onAreaInit = this.OnAreaInit;
						if (onAreaInit != null)
						{
							onAreaInit(helper, areaId);
						}
					}
				}
			}
		}

		// Token: 0x06007691 RID: 30353 RVA: 0x00373E28 File Offset: 0x00372028
		private JieQingAreaHelper GetOrCreateHelper(Area area, short areaId)
		{
			JieQingAreaHelper helper;
			bool flag = this._helpers.TryGetValue(areaId, out helper) && helper;
			JieQingAreaHelper result;
			if (flag)
			{
				bool flag2 = helper.transform.parent != area.transform;
				if (flag2)
				{
					helper.transform.SetParent(area.transform, false);
				}
				result = helper;
			}
			else
			{
				helper = area.GetComponentInChildren<JieQingAreaHelper>(true);
				bool flag3 = !helper;
				if (flag3)
				{
					bool flag4 = this.helperPrefab;
					if (flag4)
					{
						helper = Object.Instantiate<JieQingAreaHelper>(this.helperPrefab, area.transform, false);
					}
					else
					{
						GameObject helperGo = new GameObject("JieQingAreaHelper");
						helperGo.transform.SetParent(area.transform, false);
						helper = helperGo.AddComponent<JieQingAreaHelper>();
					}
				}
				helper.transform.SetParent(area.transform, false);
				helper.transform.localPosition = Vector3.zero;
				helper.transform.localScale = Vector3.one;
				helper.transform.SetAsLastSibling();
				this._helpers[areaId] = helper;
				result = helper;
			}
			return result;
		}

		// Token: 0x06007692 RID: 30354 RVA: 0x00373F48 File Offset: 0x00372148
		private static void ConfigureTip(Area area, short areaId, AreaDisplayData displayData)
		{
			TooltipInvoker tipComp = area.Displayer;
			tipComp.Type = TipType.MapArea;
			TooltipInvoker tooltipInvoker = tipComp;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipComp.RuntimeParam.Set("areaId", areaId).Set<AreaDisplayData>("displayData", displayData);
			tipComp.enabled = true;
		}

		// Token: 0x04005966 RID: 22886
		public AreaMap areaMap;

		// Token: 0x04005967 RID: 22887
		public MouseWheelScale scaler;

		// Token: 0x04005968 RID: 22888
		[SerializeField]
		private JieQingAreaHelper helperPrefab;

		// Token: 0x04005969 RID: 22889
		[SerializeField]
		private RectTransform currPosPointer;

		// Token: 0x0400596A RID: 22890
		[SerializeField]
		private RectTransform[] stateHighlights;

		// Token: 0x0400596B RID: 22891
		public Action<JieQingAreaHelper, short> OnAreaInit;

		// Token: 0x0400596D RID: 22893
		private readonly Dictionary<short, JieQingAreaHelper> _helpers = new Dictionary<short, JieQingAreaHelper>();

		// Token: 0x0400596E RID: 22894
		private readonly HashSet<short> _initializedAreaIds = new HashSet<short>();

		// Token: 0x0400596F RID: 22895
		private WorldMapModel _mapModel;

		// Token: 0x04005970 RID: 22896
		private bool _initialized;

		// Token: 0x04005971 RID: 22897
		private bool _scaleHooked;
	}
}
