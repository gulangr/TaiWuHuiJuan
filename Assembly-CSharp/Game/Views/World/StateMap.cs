using System;
using System.Diagnostics;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x0200072C RID: 1836
	public class StateMap : MonoBehaviour
	{
		// Token: 0x14000070 RID: 112
		// (add) Token: 0x060057C9 RID: 22473 RVA: 0x0028C0D0 File Offset: 0x0028A2D0
		// (remove) Token: 0x060057CA RID: 22474 RVA: 0x0028C108 File Offset: 0x0028A308
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<sbyte> OnStateTemplateIdSelected;

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x060057CB RID: 22475 RVA: 0x0028C140 File Offset: 0x0028A340
		// (remove) Token: 0x060057CC RID: 22476 RVA: 0x0028C178 File Offset: 0x0028A378
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<sbyte, Refers, bool> OnStateRefersUpdated;

		// Token: 0x060057CD RID: 22477 RVA: 0x0028C1B0 File Offset: 0x0028A3B0
		private void Awake()
		{
			this.toggleTemplate.gameObject.SetActive(false);
			int idx = 0;
			foreach (GameObject shape in this.statePositionAndShapes)
			{
				RectTransform shapeRect = shape.GetComponent<RectTransform>();
				Vector2 anchoredPosition = shapeRect.anchoredPosition;
				CToggle toggle = Object.Instantiate<GameObject>(this.toggleTemplate.gameObject, this.toggleGroup.transform).GetComponent<CToggle>();
				IrregularClickableImage toggleImage = toggle.GetComponent<IrregularClickableImage>();
				toggleImage.sprite = shape.GetComponent<CImage>().sprite;
				toggleImage.SetNativeSize();
				toggle.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
				toggle.gameObject.SetActive(true);
				Refers targetRefers = toggle.GetComponent<Refers>();
				bool flag = targetRefers != null;
				if (flag)
				{
					Action<sbyte, Refers, bool> onStateRefersUpdated = this.OnStateRefersUpdated;
					if (onStateRefersUpdated != null)
					{
						onStateRefersUpdated(StateMap.Index2StateTemplateId(idx), targetRefers, idx == this._selectedToggleIndex);
					}
				}
				this.toggleGroup.Add(toggle);
				shape.gameObject.SetActive(false);
				idx++;
			}
			this.toggleGroup.OnActiveIndexChange += this.OnStateActiveIndexChange;
			this.toggleGroup.Init(this._selectedToggleIndex);
			this._selectedToggleIndex = -1;
		}

		// Token: 0x060057CE RID: 22478 RVA: 0x0028C2F1 File Offset: 0x0028A4F1
		private void OnDestroy()
		{
			this.toggleGroup.OnActiveIndexChange -= this.OnStateActiveIndexChange;
		}

		// Token: 0x060057CF RID: 22479 RVA: 0x0028C30C File Offset: 0x0028A50C
		private void LateUpdate()
		{
			this.OnUpdateScale();
		}

		// Token: 0x060057D0 RID: 22480 RVA: 0x0028C316 File Offset: 0x0028A516
		private static sbyte Index2StateTemplateId(int index)
		{
			return (sbyte)(1 + index);
		}

		// Token: 0x060057D1 RID: 22481 RVA: 0x0028C31C File Offset: 0x0028A51C
		private static int StateTemplate2Index(sbyte stateTemplateId)
		{
			return (int)(stateTemplateId - 1);
		}

		// Token: 0x060057D2 RID: 22482 RVA: 0x0028C324 File Offset: 0x0028A524
		private void OnStateActiveIndexChange(int index, int lastIndex)
		{
			sbyte stateTemplateId = StateMap.Index2StateTemplateId(index);
			Action<sbyte> onStateTemplateIdSelected = this.OnStateTemplateIdSelected;
			if (onStateTemplateIdSelected != null)
			{
				onStateTemplateIdSelected(stateTemplateId);
			}
			CToggle ctoggle = this.toggleGroup.Get(index);
			Refers targetRefers = (ctoggle != null) ? ctoggle.GetComponent<Refers>() : null;
			bool flag = targetRefers != null;
			if (flag)
			{
				Action<sbyte, Refers, bool> onStateRefersUpdated = this.OnStateRefersUpdated;
				if (onStateRefersUpdated != null)
				{
					onStateRefersUpdated(stateTemplateId, targetRefers, true);
				}
			}
		}

		// Token: 0x060057D3 RID: 22483 RVA: 0x0028C384 File Offset: 0x0028A584
		private void OnUpdateScale()
		{
			RectTransform rectTsSelf = base.transform.GetComponent<RectTransform>();
			Vector3 screenPos = Input.mousePosition;
			Camera uiCamera = UIManager.Instance.UiCamera;
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTsSelf, screenPos, uiCamera, out pos);
			float halfWidth = rectTsSelf.rect.width / 2f;
			float halfHeight = rectTsSelf.rect.height / 2f;
			bool flag = pos.x < -1f * halfWidth || pos.x > halfWidth || pos.y > halfHeight || pos.y < -1f * halfHeight;
			if (!flag)
			{
				float scrollValue = Input.GetAxis("Mouse ScrollWheel");
				bool flag2 = Math.Abs(scrollValue) > 0f;
				if (flag2)
				{
					float scaleValue = Mathf.Clamp(scrollValue * 0.25f + this._lastScale, 0.75f, 1f);
					this._lastScale = scaleValue;
					this.dragRoot.transform.localScale = Vector3.one * scaleValue;
				}
			}
		}

		// Token: 0x060057D4 RID: 22484 RVA: 0x0028C494 File Offset: 0x0028A694
		public Vector2 GetShapeAnchor(sbyte stateTemplateId)
		{
			int index = StateMap.StateTemplate2Index(stateTemplateId);
			RectTransform childRect;
			bool flag;
			if (this.statePositionAndShapes.CheckIndex(index))
			{
				RectTransform stateRect = this.statePositionAndShapes[index].transform as RectTransform;
				if (stateRect != null && stateRect.childCount > 0)
				{
					childRect = (stateRect.GetChild(0) as RectTransform);
					flag = (childRect != null);
					goto IL_49;
				}
			}
			flag = false;
			IL_49:
			bool flag2 = flag;
			Vector2 result;
			if (flag2)
			{
				result = childRect.anchoredPosition;
			}
			else
			{
				result = Vector2.zero;
			}
			return result;
		}

		// Token: 0x060057D5 RID: 22485 RVA: 0x0028C504 File Offset: 0x0028A704
		public void SetSelectedStateTemplateId(sbyte stateTemplateId)
		{
			int index = StateMap.StateTemplate2Index(stateTemplateId);
			CToggle tog = this.toggleGroup.Get(index);
			bool flag = tog != null;
			if (flag)
			{
				this.toggleGroup.Set(index, false);
			}
			else
			{
				this._selectedToggleIndex = index;
			}
		}

		// Token: 0x060057D6 RID: 22486 RVA: 0x0028C548 File Offset: 0x0028A748
		public void FocusStateTemplateId(sbyte stateTemplateId, float duration, RectTransform root = null)
		{
			int index = StateMap.StateTemplate2Index(stateTemplateId);
			CToggle target = this.toggleGroup.Get(index);
			bool flag = target != null;
			if (flag)
			{
				this.toggleGroup.Set(index, false);
				bool flag2 = root;
				if (flag2)
				{
					this._lastScale = 1f;
					this.dragRoot.transform.localScale = Vector3.one;
					this.dragRoot.GetComponent<RectTransform>().DOAnchorPos(-root.anchoredPosition + this.offsetFocus, duration, false);
				}
				else
				{
					this.dragRoot.GetComponent<RectTransform>().DOAnchorPos(-target.GetComponent<RectTransform>().anchoredPosition + this.offsetFocus, duration, false);
				}
			}
		}

		// Token: 0x04003C3C RID: 15420
		[SerializeField]
		private GameObject[] statePositionAndShapes;

		// Token: 0x04003C3D RID: 15421
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x04003C3E RID: 15422
		[SerializeField]
		private CToggle toggleTemplate;

		// Token: 0x04003C3F RID: 15423
		[SerializeField]
		private UIRectDragMove dragRoot;

		// Token: 0x04003C40 RID: 15424
		[SerializeField]
		private Vector2 offsetFocus;

		// Token: 0x04003C41 RID: 15425
		private const float MaxScale = 1f;

		// Token: 0x04003C42 RID: 15426
		private const float MinScale = 0.75f;

		// Token: 0x04003C43 RID: 15427
		private const float ScaleFactor = 0.25f;

		// Token: 0x04003C44 RID: 15428
		private float _lastScale;

		// Token: 0x04003C45 RID: 15429
		private int _selectedToggleIndex = -1;
	}
}
