using System;
using System.Diagnostics;
using EasyButtons;
using FrameWork.UISystem.UI;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200102C RID: 4140
	[RequireComponent(typeof(UIBase))]
	public class UIViewCoveredBehaviour : MonoBehaviour
	{
		// Token: 0x14000099 RID: 153
		// (add) Token: 0x0600BD44 RID: 48452 RVA: 0x0056007C File Offset: 0x0055E27C
		// (remove) Token: 0x0600BD45 RID: 48453 RVA: 0x005600B4 File Offset: 0x0055E2B4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<UIViewCoveredBehaviour> OnAnyUIViewCoveredStateChanged;

		// Token: 0x17001556 RID: 5462
		// (get) Token: 0x0600BD46 RID: 48454 RVA: 0x005600E9 File Offset: 0x0055E2E9
		// (set) Token: 0x0600BD47 RID: 48455 RVA: 0x005600F1 File Offset: 0x0055E2F1
		public bool IsCovered { get; private set; } = false;

		// Token: 0x0600BD48 RID: 48456 RVA: 0x005600FA File Offset: 0x0055E2FA
		private void Awake()
		{
			this._uiBase = base.GetComponent<UIBase>();
		}

		// Token: 0x0600BD49 RID: 48457 RVA: 0x00560109 File Offset: 0x0055E309
		private void OnEnable()
		{
			this.IsCovered = false;
			UIVisableHandler.OnAnyUIVisableChanged += this.OnCoverStateChanged;
			this.OnCoverStateChanged();
		}

		// Token: 0x0600BD4A RID: 48458 RVA: 0x0056012D File Offset: 0x0055E32D
		private void OnDisable()
		{
			UIVisableHandler.OnAnyUIVisableChanged -= this.OnCoverStateChanged;
			this.SetCoveredState(false, false);
		}

		// Token: 0x0600BD4B RID: 48459 RVA: 0x0056014C File Offset: 0x0055E34C
		private Canvas GetCanvas()
		{
			bool flag = this._canvas == null;
			if (flag)
			{
				this._canvas = base.GetComponentInParent<Canvas>(true);
			}
			return this._canvas;
		}

		// Token: 0x0600BD4C RID: 48460 RVA: 0x00560184 File Offset: 0x0055E384
		private void OnCoverStateChanged()
		{
			bool flag = this._uiBase == null;
			if (flag)
			{
				base.enabled = false;
			}
			else
			{
				bool flag2 = this._uiBase.UiFlags.HasFlag(UIFlag.IncludeCoverCheck);
				bool covered;
				if (flag2)
				{
					covered = UIManager.Instance.UIVisableHandler.IsFullCovered(this._uiBase.Element);
				}
				else
				{
					Canvas canvas = this.GetCanvas();
					int sortingLayerID = canvas.sortingLayerID;
					int sortingOrder = canvas.sortingOrder;
					int siblingIndex = base.transform.GetSiblingIndex();
					covered = UIManager.Instance.UIVisableHandler.IsFullCovered(sortingLayerID, sortingOrder, siblingIndex);
				}
				this.SetCoveredState(covered, true);
			}
		}

		// Token: 0x0600BD4D RID: 48461 RVA: 0x00560234 File Offset: 0x0055E434
		private void SetCoveredState(bool covered, bool triggerEvent = false)
		{
			bool prevCovered = this.IsCovered;
			this.IsCovered = covered;
			foreach (GameObject obj in this.controlledObjs)
			{
				bool flag = obj != null;
				if (flag)
				{
					bool flag2 = obj.activeSelf == covered;
					if (flag2)
					{
						obj.SetActive(!covered);
					}
				}
			}
			bool flag3 = this.controlledCanvas != null;
			if (flag3)
			{
				foreach (Canvas canvas in this.controlledCanvas)
				{
					bool flag4 = canvas != null;
					if (flag4)
					{
						canvas.enabled = !covered;
					}
				}
			}
			bool flag5 = triggerEvent && prevCovered != covered;
			if (flag5)
			{
				Action<UIViewCoveredBehaviour> onAnyUIViewCoveredStateChanged = this.OnAnyUIViewCoveredStateChanged;
				if (onAnyUIViewCoveredStateChanged != null)
				{
					onAnyUIViewCoveredStateChanged(this);
				}
			}
		}

		// Token: 0x0600BD4E RID: 48462 RVA: 0x00560312 File Offset: 0x0055E512
		[Button("添加所有子Canvas到控制列表")]
		[ContextMenu("添加所有子Canvas到控制列表")]
		public void SetAllChildCanvasToControlled()
		{
			this.controlledCanvas = base.GetComponentsInChildren<Canvas>(true);
		}

		// Token: 0x040091B6 RID: 37302
		[Tooltip("被挡住时，会隐藏的Canvas，不被挡住时则显示")]
		[SerializeField]
		private Canvas[] controlledCanvas;

		// Token: 0x040091B7 RID: 37303
		[Tooltip("被挡住时，会隐藏的GameObject，不被挡住时则显示")]
		[SerializeField]
		private GameObject[] controlledObjs;

		// Token: 0x040091B8 RID: 37304
		private UIBase _uiBase;

		// Token: 0x040091B9 RID: 37305
		private Canvas _canvas;
	}
}
