using System;
using System.IO;
using GameData.Adventure.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200019B RID: 411
public class AdventureEditorElementPicker : UIBehaviour, IAdventureEditorElementHandler
{
	// Token: 0x060016E7 RID: 5863 RVA: 0x0008C1C6 File Offset: 0x0008A3C6
	public void OnClick(FileInfo file, DirectoryInfo directory)
	{
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x0008C1CC File Offset: 0x0008A3CC
	public void OnDoubleClick(FileInfo file, DirectoryInfo directory)
	{
		bool flag = file != null && file.Exists;
		if (flag)
		{
			int id;
			bool flag2 = AdventureElementSnapshot.LoadHeaderFromFile(file.FullName, out id);
			if (flag2)
			{
				UnityEvent<int> unityEvent = this.onElementIdSelected;
				if (unityEvent != null)
				{
					unityEvent.Invoke(id);
				}
			}
		}
		else
		{
			bool flag3 = directory != null && directory.Exists;
			if (flag3)
			{
				this.container.ChangeDirectory(directory);
			}
		}
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x0008C233 File Offset: 0x0008A433
	public void OnRightClick(FileInfo file, DirectoryInfo directory)
	{
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x0008C236 File Offset: 0x0008A436
	public void OnSelectEmpty()
	{
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x0008C23C File Offset: 0x0008A43C
	public IAdventureEditorElementHandler.DragPostProcess OnDragValidate(GameObject dropTarget, FileInfo file, DirectoryInfo directory)
	{
		return IAdventureEditorElementHandler.DragPostProcess.Failed;
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x0008C24F File Offset: 0x0008A44F
	protected override void Awake()
	{
		base.Awake();
		this.container.Bind(this);
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x0008C268 File Offset: 0x0008A468
	public void Setup(int elementId)
	{
		bool opened = this.Opened;
		if (opened)
		{
			this.Opened = false;
		}
		this._initialElementId = elementId;
		AdventureEditorKit.UpdateElementCache();
		string path = AdventureEditorKit.GetElementPathFromCache(this._initialElementId);
		UnityEvent<string> unityEvent = this.onDisplayNameChanged;
		if (unityEvent != null)
		{
			AdventureElementSnapshot data;
			unityEvent.Invoke(string.IsNullOrEmpty(path) ? LanguageKey.LK_None.Tr().SetColor("grey").ColorReplace() : (AdventureEditorElementInfo.GetShortFileName(Path.GetFileName(path)) + ":" + (AdventureEditorKit.TryGetElementInAnywhere(this._initialElementId, out data) ? data.Name.Value : string.Empty)));
		}
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x0008C30C File Offset: 0x0008A50C
	public void AcceptReset()
	{
		UnityEvent<int> unityEvent = this.onElementIdSelected;
		if (unityEvent != null)
		{
			unityEvent.Invoke(0);
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x060016EF RID: 5871 RVA: 0x0008C322 File Offset: 0x0008A522
	// (set) Token: 0x060016F0 RID: 5872 RVA: 0x0008C330 File Offset: 0x0008A530
	public bool Opened
	{
		get
		{
			return this.panelObject.activeSelf;
		}
		set
		{
			bool flag = this.panelObject.activeSelf != value;
			if (flag)
			{
				if (value)
				{
					this._prevExternalSelected = AdventureEditorElement.CurrentSelected;
					AdventureEditorElement.CurrentSelected = null;
					DirectoryInfo dir = null;
					AdventureEditorKit.UpdateElementCache();
					string path = AdventureEditorKit.GetElementPathFromCache(this._initialElementId);
					bool flag2 = !string.IsNullOrEmpty(path);
					if (flag2)
					{
						FileInfo fileInfo = new FileInfo(path);
						dir = fileInfo.Directory;
						AdventureEditorElement.CurrentSelected = fileInfo;
					}
					bool flag3 = dir != null;
					if (flag3)
					{
						this.container.ChangeDirectory(new DirectoryInfo(dir.FullName));
					}
					else
					{
						this.container.Redirect();
					}
				}
				else
				{
					AdventureEditorElement.CurrentSelected = this._prevExternalSelected;
					this._prevExternalSelected = null;
				}
			}
			this.panelObject.SetActive(value);
			if (value)
			{
				Canvas.ForceUpdateCanvases();
				this.AdjustPanelChildrenToScreen();
			}
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x0008C418 File Offset: 0x0008A618
	private void AdjustPanelChildrenToScreen()
	{
		UIManager instance = UIManager.Instance;
		Camera uiCamera = (instance != null) ? instance.UiCamera : null;
		bool flag = uiCamera == null;
		if (!flag)
		{
			Canvas referenceCanvas = uiCamera.GetComponentInChildren<Canvas>(true);
			bool flag2 = referenceCanvas == null;
			if (!flag2)
			{
				Camera eventCamera = (referenceCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : (referenceCanvas.worldCamera ?? uiCamera);
				for (int i = 0; i < this.panelObject.transform.childCount; i++)
				{
					RectTransform childRect = this.panelObject.transform.GetChild(i) as RectTransform;
					bool flag3 = childRect == null || !childRect.gameObject.activeInHierarchy;
					if (!flag3)
					{
						childRect.GetWorldCorners(this._corners);
						float minX = float.MaxValue;
						float maxX = float.MinValue;
						float minY = float.MaxValue;
						float maxY = float.MinValue;
						for (int j = 0; j < 4; j++)
						{
							Vector2 cornerScreenPos = RectTransformUtility.WorldToScreenPoint(eventCamera, this._corners[j]);
							bool flag4 = cornerScreenPos.x < minX;
							if (flag4)
							{
								minX = cornerScreenPos.x;
							}
							bool flag5 = cornerScreenPos.x > maxX;
							if (flag5)
							{
								maxX = cornerScreenPos.x;
							}
							bool flag6 = cornerScreenPos.y < minY;
							if (flag6)
							{
								minY = cornerScreenPos.y;
							}
							bool flag7 = cornerScreenPos.y > maxY;
							if (flag7)
							{
								maxY = cornerScreenPos.y;
							}
						}
						Vector2 deltaScreen = Vector2.zero;
						bool flag8 = minX < 0f;
						if (flag8)
						{
							deltaScreen.x = -minX;
						}
						else
						{
							bool flag9 = maxX > (float)Screen.width;
							if (flag9)
							{
								deltaScreen.x = (float)Screen.width - maxX;
							}
						}
						bool flag10 = minY < 0f;
						if (flag10)
						{
							deltaScreen.y = -minY;
						}
						else
						{
							bool flag11 = maxY > (float)Screen.height;
							if (flag11)
							{
								deltaScreen.y = (float)Screen.height - maxY;
							}
						}
						RectTransform parentRect;
						bool flag12;
						if (!(deltaScreen == Vector2.zero))
						{
							parentRect = (childRect.parent as RectTransform);
							flag12 = (parentRect == null);
						}
						else
						{
							flag12 = true;
						}
						bool flag13 = flag12;
						if (!flag13)
						{
							Vector2 currentScreenPos = RectTransformUtility.WorldToScreenPoint(eventCamera, childRect.position);
							Vector2 targetScreenPos = currentScreenPos + deltaScreen;
							Vector2 currentLocalPos;
							bool flag14 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, currentScreenPos, eventCamera, out currentLocalPos);
							if (!flag14)
							{
								Vector2 targetLocalPos;
								bool flag15 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, targetScreenPos, eventCamera, out targetLocalPos);
								if (!flag15)
								{
									Vector2 localDelta = targetLocalPos - currentLocalPos;
									childRect.localPosition += localDelta;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0400127C RID: 4732
	[SerializeField]
	private AdventureEditorElementContainer container;

	// Token: 0x0400127D RID: 4733
	[SerializeField]
	private GameObject panelObject;

	// Token: 0x0400127E RID: 4734
	public UnityEvent<int> onElementIdSelected = new UnityEvent<int>();

	// Token: 0x0400127F RID: 4735
	public UnityEvent<string> onDisplayNameChanged = new UnityEvent<string>();

	// Token: 0x04001280 RID: 4736
	private int _initialElementId = -1;

	// Token: 0x04001281 RID: 4737
	private FileSystemInfo _prevExternalSelected;

	// Token: 0x04001282 RID: 4738
	private readonly Vector3[] _corners = new Vector3[4];
}
