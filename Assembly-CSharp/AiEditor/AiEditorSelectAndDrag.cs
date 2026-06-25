using System;
using System.Collections.Generic;
using FrameWork.Tools;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000677 RID: 1655
	public class AiEditorSelectAndDrag : MonoBehaviour
	{
		// Token: 0x06004E32 RID: 20018 RVA: 0x0024CBF0 File Offset: 0x0024ADF0
		private static bool ApproximatelyVector(Vector2 a, Vector2 b)
		{
			return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x06004E33 RID: 20019 RVA: 0x0024CC29 File Offset: 0x0024AE29
		// (set) Token: 0x06004E34 RID: 20020 RVA: 0x0024CC31 File Offset: 0x0024AE31
		public bool PointerOver { get; private set; }

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06004E35 RID: 20021 RVA: 0x0024CC3A File Offset: 0x0024AE3A
		private bool SkipUpdateHovering
		{
			get
			{
				return this._selectKeyDown || this._actionKeyDown;
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x06004E36 RID: 20022 RVA: 0x0024CC4D File Offset: 0x0024AE4D
		// (set) Token: 0x06004E37 RID: 20023 RVA: 0x0024CC55 File Offset: 0x0024AE55
		public bool Dragging { get; private set; }

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x06004E38 RID: 20024 RVA: 0x0024CC5E File Offset: 0x0024AE5E
		// (set) Token: 0x06004E39 RID: 20025 RVA: 0x0024CC66 File Offset: 0x0024AE66
		public bool MultiSelecting { get; private set; }

		// Token: 0x06004E3A RID: 20026 RVA: 0x0024CC70 File Offset: 0x0024AE70
		private void Awake()
		{
			bool flag = this.raycast == null;
			if (flag)
			{
				this.raycast = base.gameObject.GetOrAddComponent<AiEditorRaycast>();
			}
		}

		// Token: 0x06004E3B RID: 20027 RVA: 0x0024CCA0 File Offset: 0x0024AEA0
		private void Update()
		{
			bool flag = this._handler == null;
			if (!flag)
			{
				this.PointerOver = this.raycast.IsPointerOver(this.target);
				bool flag2 = this.autoUpdate;
				if (flag2)
				{
					this.AutoCollectComponents();
				}
				bool flag3 = !this.SkipUpdateHovering;
				if (flag3)
				{
					this.UpdateHoveringComponent();
				}
				this.UpdateSelectStatus();
				this.UpdateActionStatus();
				this.UpdateDragStatus();
			}
		}

		// Token: 0x06004E3C RID: 20028 RVA: 0x0024CD0F File Offset: 0x0024AF0F
		public void Bind(ISelectAndDragHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06004E3D RID: 20029 RVA: 0x0024CD1C File Offset: 0x0024AF1C
		public void AutoCollectComponents()
		{
			this.target.GetComponentsInTopChildren(this._allComponents, false);
			bool flag = !this._allComponents.Contains(this._hoveringComponent);
			if (flag)
			{
				this._hoveringComponent = null;
			}
		}

		// Token: 0x06004E3E RID: 20030 RVA: 0x0024CD5C File Offset: 0x0024AF5C
		private void UpdateHoveringComponent()
		{
			this._cachedComponents.Clear();
			foreach (ISelectAndDragComponent component in this._allComponents)
			{
				bool flag = component.OverlapsMouse();
				if (flag)
				{
					this._cachedComponents.Add(component);
				}
			}
			this._cachedComponents.Sort(new Comparison<ISelectAndDragComponent>(this.Comparison));
			ISelectAndDragComponent newHoveringComponent = (this._cachedComponents.Count > 0) ? this._cachedComponents[0] : null;
			bool flag2 = newHoveringComponent == this._hoveringComponent;
			if (!flag2)
			{
				ISelectAndDragComponent hoveringComponent = this._hoveringComponent;
				if (hoveringComponent != null)
				{
					hoveringComponent.PointerExit();
				}
				this._hoveringComponent = newHoveringComponent;
				ISelectAndDragComponent hoveringComponent2 = this._hoveringComponent;
				if (hoveringComponent2 != null)
				{
					hoveringComponent2.PointerEnter();
				}
			}
		}

		// Token: 0x06004E3F RID: 20031 RVA: 0x0024CE40 File Offset: 0x0024B040
		private int Comparison(ISelectAndDragComponent x, ISelectAndDragComponent y)
		{
			return y.RectTransform.GetSiblingIndex().CompareTo(x.RectTransform.GetSiblingIndex());
		}

		// Token: 0x06004E40 RID: 20032 RVA: 0x0024CE70 File Offset: 0x0024B070
		private void UpdateSelectStatus()
		{
			bool flag = Input.GetKeyDown(KeyCode.Mouse0) && this.PointerOver;
			if (flag)
			{
				this.UpdateSelectStatusKeyDown();
			}
			bool flag2 = Input.GetKey(KeyCode.Mouse0) && this._selectKeyDown;
			if (flag2)
			{
				this.UpdateSelectStatusKey();
			}
			bool flag3 = Input.GetKeyUp(KeyCode.Mouse0) && this._selectKeyDown;
			if (flag3)
			{
				this.UpdateSelectStatusKeyUp();
			}
		}

		// Token: 0x06004E41 RID: 20033 RVA: 0x0024CEE0 File Offset: 0x0024B0E0
		private void UpdateSelectStatusKeyDown()
		{
			this._selectKeyDown = true;
			this._selectKeyDownPos = Input.mousePosition;
			bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			if (flag)
			{
				this._selectType = AiEditorSelectAndDrag.ESelectType.Union;
			}
			else
			{
				bool flag2 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				if (flag2)
				{
					this._selectType = AiEditorSelectAndDrag.ESelectType.Subtract;
				}
				else
				{
					this._selectType = AiEditorSelectAndDrag.ESelectType.None;
				}
			}
			bool flag3 = this._selectType == AiEditorSelectAndDrag.ESelectType.None && (this._hoveringComponent == null || !this.allowDragComponent);
			if (flag3)
			{
				this.SelectEmpty();
			}
			else
			{
				bool flag4 = this._hoveringComponent != null && this.allowDragComponent;
				if (flag4)
				{
					this.ApplySelectTypeByHovering();
				}
			}
		}

		// Token: 0x06004E42 RID: 20034 RVA: 0x0024CFA0 File Offset: 0x0024B1A0
		private void UpdateSelectStatusKey()
		{
			Vector3 mousePos = Input.mousePosition;
			bool flag = !this._selectKeyMoving && !AiEditorSelectAndDrag.ApproximatelyVector(mousePos, this._selectKeyDownPos);
			if (flag)
			{
				this._selectKeyMoving = true;
				bool flag2 = this._hoveringComponent != null && this.allowDragComponent;
				if (flag2)
				{
					this.BeginDrag(AiEditorSelectAndDrag.EDragType.Select);
				}
				else
				{
					this.BeginMultiSelect();
				}
			}
			bool flag3 = !this._selectKeyMoving;
			if (!flag3)
			{
				this.OnMultiSelect();
			}
		}

		// Token: 0x06004E43 RID: 20035 RVA: 0x0024D01C File Offset: 0x0024B21C
		private void UpdateSelectStatusKeyUp()
		{
			bool flag = this._draggingType == AiEditorSelectAndDrag.EDragType.Select;
			if (flag)
			{
				this.EndDrag();
			}
			bool flag2 = this._hoveringComponent != null && !this.allowDragComponent && !this.MultiSelecting;
			if (flag2)
			{
				this.ApplySelectTypeByHovering();
			}
			this.EndMultiSelect();
			this._selectKeyDown = (this._selectKeyMoving = false);
		}

		// Token: 0x06004E44 RID: 20036 RVA: 0x0024D07C File Offset: 0x0024B27C
		private void UpdateActionStatus()
		{
			bool flag = Input.GetKeyDown(KeyCode.Mouse1) && this.PointerOver;
			if (flag)
			{
				this._actionKeyDown = true;
				this._actionKeyDownPos = Input.mousePosition;
			}
			bool flag2 = Input.GetKey(KeyCode.Mouse1) && this._actionKeyDown;
			if (flag2)
			{
				Vector3 mousePos = Input.mousePosition;
				bool flag3 = this._actionKeyMoving || AiEditorSelectAndDrag.ApproximatelyVector(mousePos, this._actionKeyDownPos);
				if (flag3)
				{
					return;
				}
				this._actionKeyMoving = true;
				this.BeginDrag(AiEditorSelectAndDrag.EDragType.Action);
			}
			bool flag4 = Input.GetKeyUp(KeyCode.Mouse1) && this._actionKeyDown;
			if (flag4)
			{
				bool flag5 = this._draggingType == AiEditorSelectAndDrag.EDragType.Action;
				if (flag5)
				{
					this.EndDrag();
				}
				bool flag6 = !this._actionKeyMoving;
				if (flag6)
				{
					bool flag7 = this._hoveringComponent == null;
					if (flag7)
					{
						this._handler.ActionContext();
					}
					else
					{
						bool flag8 = this._handler.IsSelectedComponent(this._hoveringComponent);
						if (flag8)
						{
							this._handler.ActionComponents();
						}
						else
						{
							this.UnselectAll();
							this.SelectSingle(this._hoveringComponent);
							this._handler.ActionComponents();
						}
					}
				}
				this._actionKeyDown = (this._actionKeyMoving = false);
			}
		}

		// Token: 0x06004E45 RID: 20037 RVA: 0x0024D1D0 File Offset: 0x0024B3D0
		private void TryParseDraggingPoint(out Vector2 localPoint)
		{
			this.TryParseDraggingPoint(Input.mousePosition, out localPoint);
		}

		// Token: 0x06004E46 RID: 20038 RVA: 0x0024D1E4 File Offset: 0x0024B3E4
		private void TryParseDraggingPoint(Vector2 screenPoint, out Vector2 localPoint)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._draggingRoot, screenPoint, UIManager.Instance.UiCamera, out localPoint);
		}

		// Token: 0x06004E47 RID: 20039 RVA: 0x0024D200 File Offset: 0x0024B400
		private void BeginDrag(AiEditorSelectAndDrag.EDragType type)
		{
			bool dragging = this.Dragging;
			if (!dragging)
			{
				bool flag = this._selectKeyMoving && this._hoveringComponent != null && this.allowDragComponent;
				Vector2 draggingKeyDownPos;
				if (flag)
				{
					this._draggingTarget = this._hoveringComponent.RectTransform;
					draggingKeyDownPos = this._selectKeyDownPos;
				}
				else
				{
					bool actionKeyMoving = this._actionKeyMoving;
					if (!actionKeyMoving)
					{
						return;
					}
					this._draggingTarget = this.target;
					draggingKeyDownPos = this._actionKeyDownPos;
				}
				this.Dragging = true;
				this._draggingType = type;
				this._handler.BeginDrag();
				this._draggingStartPos = this._draggingTarget.anchoredPosition;
				this._draggingRoot = (RectTransform)this._draggingTarget.parent;
				Vector2 localPoint;
				this.TryParseDraggingPoint(draggingKeyDownPos, out localPoint);
				this._draggingOffset = this._draggingTarget.anchoredPosition - localPoint;
			}
		}

		// Token: 0x06004E48 RID: 20040 RVA: 0x0024D2E0 File Offset: 0x0024B4E0
		private void UpdateDragStatus()
		{
			bool flag = !this.Dragging;
			if (!flag)
			{
				Vector2 localPoint;
				this.TryParseDraggingPoint(out localPoint);
				this._draggingTarget.anchoredPosition = localPoint + this._draggingOffset;
			}
		}

		// Token: 0x06004E49 RID: 20041 RVA: 0x0024D320 File Offset: 0x0024B520
		private void EndDrag()
		{
			bool flag = !this.Dragging;
			if (!flag)
			{
				this.Dragging = false;
				ISelectAndDragComponent component = this._draggingTarget.GetComponent<ISelectAndDragComponent>();
				this._handler.EndDrag(component, this._draggingStartPos, this._draggingTarget.anchoredPosition);
			}
		}

		// Token: 0x06004E4A RID: 20042 RVA: 0x0024D36F File Offset: 0x0024B56F
		private void SelectSingle(ISelectAndDragComponent component)
		{
			this._handler.Select(component);
		}

		// Token: 0x06004E4B RID: 20043 RVA: 0x0024D37F File Offset: 0x0024B57F
		private void UnselectSingle(ISelectAndDragComponent component)
		{
			this._handler.Unselect(component);
		}

		// Token: 0x06004E4C RID: 20044 RVA: 0x0024D390 File Offset: 0x0024B590
		private void UnselectAll()
		{
			this._cachedComponents.Clear();
			this._cachedComponents.AddRange(this._handler.SelectedComponents);
			foreach (ISelectAndDragComponent component in this._cachedComponents)
			{
				this._handler.Unselect(component);
			}
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x0024D410 File Offset: 0x0024B610
		private void ApplySelectTypeByHovering()
		{
			bool flag = this._handler.IsSelectedComponent(this._hoveringComponent);
			if (flag)
			{
				bool flag2 = this._selectType == AiEditorSelectAndDrag.ESelectType.Subtract;
				if (flag2)
				{
					this.UnselectSingle(this._hoveringComponent);
				}
			}
			else
			{
				bool flag3 = this._selectType == AiEditorSelectAndDrag.ESelectType.None;
				if (flag3)
				{
					this.UnselectAll();
				}
				bool flag4 = this._selectType != AiEditorSelectAndDrag.ESelectType.Subtract;
				if (flag4)
				{
					this.SelectSingle(this._hoveringComponent);
				}
			}
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x0024D484 File Offset: 0x0024B684
		private void SelectEmpty()
		{
			this._handler.SelectEmpty();
		}

		// Token: 0x06004E4F RID: 20047 RVA: 0x0024D494 File Offset: 0x0024B694
		private void BeginMultiSelect()
		{
			bool flag = !this._selectKeyDown || this.MultiSelecting;
			if (!flag)
			{
				this.MultiSelecting = true;
				this._multiSelectStartLocalPos = this.target.ScreenToLocalPoint(this._selectKeyDownPos);
				Vector2 startScreenPos = this.target.LocalToScreenPoint(this._multiSelectStartLocalPos);
				this._handler.BeginMultiSelect(startScreenPos);
			}
		}

		// Token: 0x06004E50 RID: 20048 RVA: 0x0024D4F8 File Offset: 0x0024B6F8
		private void OnMultiSelect()
		{
			bool flag = !this.MultiSelecting;
			if (!flag)
			{
				this._multiSelectCurrScreenPos = Input.mousePosition;
				Vector2 startScreenPos = this.target.LocalToScreenPoint(this._multiSelectStartLocalPos);
				this._handler.OnMultiSelect(startScreenPos, this._multiSelectCurrScreenPos);
			}
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x0024D54C File Offset: 0x0024B74C
		private void EndMultiSelect()
		{
			bool flag = !this.MultiSelecting;
			if (!flag)
			{
				this.MultiSelecting = false;
				this._handler.EndMultiSelect();
				this._cachedComponents.Clear();
				Vector2 multiSelectEndLocalPos = this.target.ScreenToLocalPoint(this._multiSelectCurrScreenPos);
				Rect multiSelectRect = RectTransformUtilityAdvanced.PointToRect(this._multiSelectStartLocalPos, multiSelectEndLocalPos);
				foreach (ISelectAndDragComponent component in this._allComponents)
				{
					bool flag2 = component.Overlaps(multiSelectRect);
					if (flag2)
					{
						this._cachedComponents.Add(component);
					}
				}
				foreach (ISelectAndDragComponent component2 in this._cachedComponents)
				{
					bool flag3 = this._selectType != AiEditorSelectAndDrag.ESelectType.Subtract;
					if (flag3)
					{
						this.SelectSingle(component2);
					}
					else
					{
						this.UnselectSingle(component2);
					}
				}
			}
		}

		// Token: 0x0400361B RID: 13851
		private const KeyCode SelectKey = KeyCode.Mouse0;

		// Token: 0x0400361C RID: 13852
		private const KeyCode ActionKey = KeyCode.Mouse1;

		// Token: 0x0400361E RID: 13854
		[SerializeField]
		private bool autoUpdate = true;

		// Token: 0x0400361F RID: 13855
		[SerializeField]
		private bool allowDragComponent = true;

		// Token: 0x04003620 RID: 13856
		[SerializeField]
		private RectTransform target;

		// Token: 0x04003621 RID: 13857
		[SerializeField]
		private AiEditorRaycast raycast;

		// Token: 0x04003622 RID: 13858
		private ISelectAndDragHandler _handler;

		// Token: 0x04003623 RID: 13859
		private readonly List<ISelectAndDragComponent> _allComponents = new List<ISelectAndDragComponent>();

		// Token: 0x04003624 RID: 13860
		private readonly List<ISelectAndDragComponent> _cachedComponents = new List<ISelectAndDragComponent>();

		// Token: 0x04003625 RID: 13861
		private ISelectAndDragComponent _hoveringComponent;

		// Token: 0x04003626 RID: 13862
		private Vector2 _selectKeyDownPos;

		// Token: 0x04003627 RID: 13863
		private bool _selectKeyDown;

		// Token: 0x04003628 RID: 13864
		private bool _selectKeyMoving;

		// Token: 0x04003629 RID: 13865
		private AiEditorSelectAndDrag.ESelectType _selectType;

		// Token: 0x0400362A RID: 13866
		private Vector2 _actionKeyDownPos;

		// Token: 0x0400362B RID: 13867
		private bool _actionKeyDown;

		// Token: 0x0400362C RID: 13868
		private bool _actionKeyMoving;

		// Token: 0x0400362E RID: 13870
		private AiEditorSelectAndDrag.EDragType _draggingType;

		// Token: 0x0400362F RID: 13871
		private RectTransform _draggingRoot;

		// Token: 0x04003630 RID: 13872
		private RectTransform _draggingTarget;

		// Token: 0x04003631 RID: 13873
		private Vector2 _draggingStartPos;

		// Token: 0x04003632 RID: 13874
		private Vector2 _draggingOffset;

		// Token: 0x04003634 RID: 13876
		private Vector2 _multiSelectStartLocalPos;

		// Token: 0x04003635 RID: 13877
		private Vector2 _multiSelectCurrScreenPos;

		// Token: 0x02001AA7 RID: 6823
		public enum EDragType
		{
			// Token: 0x0400B6BB RID: 46779
			Select,
			// Token: 0x0400B6BC RID: 46780
			Action
		}

		// Token: 0x02001AA8 RID: 6824
		public enum ESelectType
		{
			// Token: 0x0400B6BE RID: 46782
			None,
			// Token: 0x0400B6BF RID: 46783
			Union,
			// Token: 0x0400B6C0 RID: 46784
			Subtract
		}
	}
}
