using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000175 RID: 373
public class AdventureEditorElement : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	// Token: 0x17000251 RID: 593
	// (get) Token: 0x060014AB RID: 5291 RVA: 0x0008041D File Offset: 0x0007E61D
	// (set) Token: 0x060014AC RID: 5292 RVA: 0x00080425 File Offset: 0x0007E625
	public FileInfo File { get; private set; }

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x060014AD RID: 5293 RVA: 0x0008042E File Offset: 0x0007E62E
	// (set) Token: 0x060014AE RID: 5294 RVA: 0x00080436 File Offset: 0x0007E636
	public DirectoryInfo Directory { get; private set; }

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x060014AF RID: 5295 RVA: 0x0008043F File Offset: 0x0007E63F
	public RectTransform RectTransform
	{
		get
		{
			return (RectTransform)base.transform;
		}
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x0008044C File Offset: 0x0007E64C
	private void Awake()
	{
		CButton btn = base.GetComponent<CButton>();
		btn.ClearAndAddListener(new Action(this.OnClick));
		this._canvasGroup = base.GetComponent<CanvasGroup>();
		bool flag = this._canvasGroup == null;
		if (flag)
		{
			this._canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x000804A4 File Offset: 0x0007E6A4
	private void LateUpdate()
	{
		bool flag = this.selectedSign;
		if (flag)
		{
			GameObject gameObject = this.selectedSign;
			bool active;
			if (AdventureEditorElement.CurrentSelected != null && !string.IsNullOrEmpty(AdventureEditorElement.CurrentSelected.FullName))
			{
				FileInfo file = this.File;
				if (!(((file != null) ? file.FullName : null) == AdventureEditorElement.CurrentSelected.FullName))
				{
					DirectoryInfo directory = this.Directory;
					active = (((directory != null) ? directory.FullName : null) == AdventureEditorElement.CurrentSelected.FullName);
				}
				else
				{
					active = true;
				}
			}
			else
			{
				active = false;
			}
			gameObject.SetActive(active);
		}
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x00080530 File Offset: 0x0007E730
	public void OnClick()
	{
		AdventureEditorElement.CurrentSelected = ((this.File != null) ? this.File : this.Directory);
		bool flag = this._lastClickTime < 0f || this._lastClickTime + this.doubleClickDuration < Time.unscaledTime;
		if (flag)
		{
			this._lastClickTime = Time.unscaledTime;
			IAdventureEditorElementHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnClick(this.File, this.Directory);
			}
		}
		else
		{
			this._lastClickTime = -1f;
			IAdventureEditorElementHandler handler2 = this._handler;
			if (handler2 != null)
			{
				handler2.OnDoubleClick(this.File, this.Directory);
			}
		}
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x000805DC File Offset: 0x0007E7DC
	public void OnPointerClick(PointerEventData eventData)
	{
		bool flag = eventData.button == PointerEventData.InputButton.Right;
		if (flag)
		{
			IAdventureEditorElementHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnRightClick(this.File, this.Directory);
			}
		}
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x00080617 File Offset: 0x0007E817
	public void Bind(IAdventureEditorElementHandler handler)
	{
		this._handler = handler;
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x00080620 File Offset: 0x0007E820
	public void Set(FileInfo file, AdventureElementSnapshot data)
	{
		this.File = file;
		this.Directory = null;
		AdventureEditorKit.SetAdventureElementIcon(this.icon, data.Icon);
		this.text.text = file.Name.Replace(".adve", "");
		this._elementId = data.Id;
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x00080680 File Offset: 0x0007E880
	public void Set(DirectoryInfo directory, string overrideNameFormat = "")
	{
		this.File = null;
		this.Directory = directory;
		this.icon.sprite = this.dirIcon;
		this.text.text = (string.IsNullOrEmpty(overrideNameFormat) ? directory.Name : string.Format(overrideNameFormat, directory.Name));
		this._elementId = -1;
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000806E0 File Offset: 0x0007E8E0
	public void OnBeginDrag(PointerEventData eventData)
	{
		this._originalLocalPos = base.transform.localPosition;
		this._originalParent = base.transform.parent;
		RectTransform selfRect = base.transform as RectTransform;
		bool flag = selfRect != null;
		if (flag)
		{
			int originalIndex = base.transform.GetSiblingIndex();
			bool flag2 = this._dragParentPlaceHolder;
			if (flag2)
			{
				Object.DestroyImmediate(this._dragParentPlaceHolder);
			}
			this._dragParentPlaceHolder = new GameObject(base.name, new Type[]
			{
				typeof(RectTransform)
			});
			this._dragParentPlaceHolder.transform.SetParent(base.transform.parent);
			this._dragParentPlaceHolder.transform.SetSiblingIndex(originalIndex);
			GameObject dragParentPlaceHolder = this._dragParentPlaceHolder;
			RectTransform dragParentRect = ((dragParentPlaceHolder != null) ? dragParentPlaceHolder.transform : null) as RectTransform;
			bool flag3 = dragParentRect != null;
			if (flag3)
			{
				dragParentRect.sizeDelta = selfRect.rect.size;
				dragParentRect.localPosition = this._originalLocalPos;
			}
		}
		RectTransform rootCanvas = AdventureEditorElement.<OnBeginDrag>g__GetTopmostRectTransform|31_0(base.GetComponent<RectTransform>());
		base.transform.SetParent(rootCanvas, true);
		this._dragParentRect = rootCanvas;
		this._canvasGroup.blocksRaycasts = false;
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x0008082C File Offset: 0x0007EA2C
	public void OnDrag(PointerEventData eventData)
	{
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._dragParentRect, eventData.position, eventData.pressEventCamera, out localPoint);
		base.transform.localPosition = localPoint;
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x00080868 File Offset: 0x0007EA68
	public void OnEndDrag(PointerEventData eventData)
	{
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = eventData.position;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);
		GameObject dropTarget = results.Where(delegate(RaycastResult result)
		{
			RaycastResult raycastResult = result;
			bool result2;
			if (raycastResult.gameObject != base.gameObject)
			{
				raycastResult = result;
				result2 = (raycastResult.gameObject != this._dragParentPlaceHolder);
			}
			else
			{
				result2 = false;
			}
			return result2;
		}).Select(delegate(RaycastResult result)
		{
			RaycastResult raycastResult = result;
			return raycastResult.gameObject;
		}).FirstOrDefault<GameObject>();
		IAdventureEditorElementHandler handler = this._handler;
		switch ((handler != null) ? handler.OnDragValidate(dropTarget, this.File, this.Directory) : IAdventureEditorElementHandler.DragPostProcess.Failed)
		{
		case IAdventureEditorElementHandler.DragPostProcess.Failed:
		case IAdventureEditorElementHandler.DragPostProcess.Stay:
			base.transform.SetParent(this._originalParent, false);
			base.transform.localPosition = this._originalLocalPos;
			base.transform.SetSiblingIndex(this._dragParentPlaceHolder.transform.GetSiblingIndex());
			break;
		case IAdventureEditorElementHandler.DragPostProcess.Move:
			base.transform.SetParent(dropTarget.transform, false);
			base.transform.localPosition = Vector3.zero;
			break;
		case IAdventureEditorElementHandler.DragPostProcess.Delete:
			Object.Destroy(base.gameObject);
			break;
		}
		bool flag = this._dragParentPlaceHolder;
		if (flag)
		{
			Object.DestroyImmediate(this._dragParentPlaceHolder);
			this._dragParentPlaceHolder = null;
		}
		bool flag2 = this._canvasGroup;
		if (flag2)
		{
			this._canvasGroup.blocksRaycasts = true;
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x00080A04 File Offset: 0x0007EC04
	[CompilerGenerated]
	internal static RectTransform <OnBeginDrag>g__GetTopmostRectTransform|31_0(RectTransform start)
	{
		Transform current = start.parent;
		RectTransform result = start;
		while (current != null)
		{
			RectTransform rt = current as RectTransform;
			bool flag = rt != null && rt.GetComponent<Canvas>() != null;
			if (flag)
			{
				result = rt;
				break;
			}
			current = current.parent;
		}
		return result;
	}

	// Token: 0x04001145 RID: 4421
	[SerializeField]
	private GameObject selectedSign;

	// Token: 0x04001146 RID: 4422
	[SerializeField]
	private CImage icon;

	// Token: 0x04001147 RID: 4423
	[SerializeField]
	private TextMeshProUGUI text;

	// Token: 0x04001148 RID: 4424
	[SerializeField]
	private Sprite dirIcon;

	// Token: 0x04001149 RID: 4425
	[SerializeField]
	private float doubleClickDuration = 0.5f;

	// Token: 0x0400114A RID: 4426
	private float _lastClickTime = -1f;

	// Token: 0x0400114B RID: 4427
	private IAdventureEditorElementHandler _handler;

	// Token: 0x0400114C RID: 4428
	internal static FileSystemInfo CurrentSelected;

	// Token: 0x0400114D RID: 4429
	private int _elementId = -1;

	// Token: 0x0400114E RID: 4430
	private Vector2 _originalLocalPos;

	// Token: 0x0400114F RID: 4431
	private RectTransform _dragParentRect;

	// Token: 0x04001150 RID: 4432
	private Transform _originalParent;

	// Token: 0x04001151 RID: 4433
	private CanvasGroup _canvasGroup;

	// Token: 0x04001154 RID: 4436
	private GameObject _dragParentPlaceHolder;
}
