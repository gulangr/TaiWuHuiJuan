using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UI;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class UIManager : MonoBehaviour
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0002E8B5 File Offset: 0x0002CAB5
	// (set) Token: 0x060006A4 RID: 1700 RVA: 0x0002E8AD File Offset: 0x0002CAAD
	public static UIManager Instance { get; private set; }

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0002E8BC File Offset: 0x0002CABC
	// (set) Token: 0x060006A7 RID: 1703 RVA: 0x0002E8C4 File Offset: 0x0002CAC4
	public bool BlockHotKey
	{
		get
		{
			return this._blockHotKey;
		}
		private set
		{
			this._blockHotKey = value;
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060006A8 RID: 1704 RVA: 0x0002E8CD File Offset: 0x0002CACD
	// (set) Token: 0x060006A9 RID: 1705 RVA: 0x0002E8D5 File Offset: 0x0002CAD5
	public bool EscHandled { get; private set; }

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060006AA RID: 1706 RVA: 0x0002E8DE File Offset: 0x0002CADE
	// (set) Token: 0x060006AB RID: 1707 RVA: 0x0002E8E6 File Offset: 0x0002CAE6
	public UIVisableHandler UIVisableHandler { get; private set; }

	// Token: 0x060006AC RID: 1708 RVA: 0x0002E8EF File Offset: 0x0002CAEF
	public RectTransform GetLayer(UILayer layer)
	{
		return this._layerRoots.ContainsKey(layer) ? ((RectTransform)this._layerRoots[layer]) : null;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0002E914 File Offset: 0x0002CB14
	private void Awake()
	{
		UIManager.Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		this.UiCamera = base.GetComponent<Camera>();
		this._curElements = new List<UIElement>();
		this._cachedElements = new List<UIElement>();
		this._uiStack = new Stack<UIGroup>();
		this.InitLayerRoots();
		this.MaxCacheElementCount = (sbyte)(UIElement.ConstantElements.Count + 5);
		GEvent.Add(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIHide));
		GEvent.Add(UiEvents.OnUIElementShow, new GEvent.Callback(this.OnUIShow));
		this.UIVisableHandler = new UIVisableHandler();
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0002E9B8 File Offset: 0x0002CBB8
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIHide));
		GEvent.Remove(UiEvents.OnUIElementShow, new GEvent.Callback(this.OnUIShow));
		this.UIVisableHandler.Dispose();
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x0002E9F8 File Offset: 0x0002CBF8
	private void Update()
	{
		bool flag = this._curElements.Count > 0;
		if (flag)
		{
			this.EscHandled = false;
			this.CheckQuickHide();
		}
		this.UIVisableHandler.TrySendCoverStateChangeEvent();
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x0002EA38 File Offset: 0x0002CC38
	private void LateUpdate()
	{
		Vector2 screenSize = UIManager.Instance.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
		bool flag = Mathf.Abs(UIManager._screenWidth - screenSize.x) > 0.1f;
		if (flag)
		{
			float maskWidth = Mathf.Max((screenSize.x - 2560f) / 2f, 0f);
			UIManager._screenWidth = screenSize.x;
			UIManager.Instance.MaskLeft.rectTransform.sizeDelta = Vector2.zero.SetX(maskWidth);
			UIManager.Instance.MaskRight.rectTransform.sizeDelta = Vector2.zero.SetX(maskWidth);
		}
		bool flag2 = Mathf.Abs(UIManager._screenHeight - screenSize.y) > 0.1f;
		if (flag2)
		{
			float maskHeight = Mathf.Max((screenSize.y - 1440f) / 2f, 0f);
			UIManager._screenHeight = screenSize.y;
			UIManager.Instance.MaskTop.rectTransform.sizeDelta = Vector2.zero.SetY(maskHeight);
			UIManager.Instance.MaskBottom.rectTransform.sizeDelta = Vector2.zero.SetY(maskHeight);
		}
		this.UIVisableHandler.TrySendCoverStateChangeEvent();
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0002EB7C File Offset: 0x0002CD7C
	public void PlaceUI(UIBase uiBase)
	{
		UILayer layer = uiBase.UiType;
		Transform transParent;
		bool flag = this._layerRoots.TryGetValue(layer, out transParent);
		if (flag)
		{
			foreach (object obj in transParent)
			{
				Transform child = (Transform)obj;
				bool flag2 = child.name == uiBase.name;
				if (flag2)
				{
					Object.Destroy(child.gameObject);
				}
			}
			uiBase.transform.SetParent(transParent, false);
			uiBase.transform.SetAsLastSibling();
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.SetObject("UIBase", uiBase);
			GEvent.OnEvent(UiEvents.OnUIBasePlaced, box);
		}
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x0002EC54 File Offset: 0x0002CE54
	public Vector2 MousePosToLocalPos(RectTransform rect)
	{
		bool flag = null == rect;
		Vector2 result;
		if (flag)
		{
			result = Vector2.zero;
		}
		else
		{
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, this.UiCamera, out pos);
			result = pos;
		}
		return result;
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0002EC94 File Offset: 0x0002CE94
	public void HideUI(UIElement elem)
	{
		bool flag = elem == null;
		if (!flag)
		{
			bool flag2 = this._curElements.Count == 1 && this._curElements[0] == elem;
			if (flag2)
			{
				this.StackBack(null);
			}
			else
			{
				bool flag3 = !this._curElements.Contains(elem);
				if (!flag3)
				{
					bool isTopUi = this._curElements[this._curElements.Count - 1] == elem;
					this._curElements.Remove(elem);
					elem.OnHide = (Action)Delegate.Combine(elem.OnHide, new Action(delegate()
					{
						this.BlockHotKey = false;
					}));
					GLog.LogWithTag(elem, Array.Empty<object>());
					this.BlockHotKey = true;
					elem.Hide(false);
					bool flag4 = isTopUi && !UIElement.Loading.Exist;
					if (flag4)
					{
						GEvent.OnEvent(UiEvents.TopUiChanged, null);
					}
				}
			}
		}
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x0002ED88 File Offset: 0x0002CF88
	public void ShowUI(UIElement elem, bool pushNoMaskState = true)
	{
		try
		{
			bool flag = this._curElements.Contains(elem);
			if (!flag)
			{
				elem.OnShowed = (Action)Delegate.Combine(elem.OnShowed, new Action(delegate()
				{
					this.BlockHotKey = false;
				}));
				GLog.LogWithTag(elem, Array.Empty<object>());
				this.BlockHotKey = true;
				this.RemoveUIElementCache(elem);
				elem.Show();
				bool exist = UIElement.PermanentTips.Exist;
				if (exist)
				{
					UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().ClearAllPermanentTips();
				}
				SingletonObject.getInstance<TooltipManager>().HideTips(TipType.Count, true);
				this._curElements.Add(elem);
				bool flag2 = !UIElement.Loading.Exist;
				if (flag2)
				{
					GEvent.OnEvent(UiEvents.TopUiChanged, null);
				}
				bool flag3 = pushNoMaskState && elem.IsFullScreen;
				if (flag3)
				{
					UIMaskManager instance = SingletonObject.getInstance<UIMaskManager>();
					if (instance != null)
					{
						instance.PushNoMaskState();
					}
					elem.OnHide = (Action)Delegate.Combine(elem.OnHide, new Action(delegate()
					{
						UIMaskManager instance2 = SingletonObject.getInstance<UIMaskManager>();
						if (instance2 != null)
						{
							instance2.PopNoMaskState();
						}
					}));
				}
			}
		}
		catch (Exception e)
		{
			this.BlockHotKey = false;
			GLog.TagError("UIManager", e.ToString(), Array.Empty<object>());
		}
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x0002EEE8 File Offset: 0x0002D0E8
	public void MaskUI(UIElement elem)
	{
		UIManager.<>c__DisplayClass42_0 CS$<>8__locals1 = new UIManager.<>c__DisplayClass42_0();
		CS$<>8__locals1.elem = elem;
		bool flag = this._curElements != null && this._curElements.Contains(CS$<>8__locals1.elem);
		if (!flag)
		{
			CS$<>8__locals1.attachedTarget = null;
			CS$<>8__locals1.manager = SingletonObject.getInstance<UIMaskManager>();
			UIMaskManager manager = CS$<>8__locals1.manager;
			if (manager != null)
			{
				manager.RegisterMaskedElement(CS$<>8__locals1.elem);
			}
			UIElement elem2 = CS$<>8__locals1.elem;
			elem2.OnActive = (Action)Delegate.Combine(elem2.OnActive, new Action(CS$<>8__locals1.<MaskUI>g__AttachMask|0));
			UIElement elem3 = CS$<>8__locals1.elem;
			elem3.OnHide = (Action)Delegate.Combine(elem3.OnHide, new Action(CS$<>8__locals1.<MaskUI>g__DetachMask|1));
			this.ShowUI(CS$<>8__locals1.elem, false);
		}
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x0002EFB0 File Offset: 0x0002D1B0
	private void RestoreMasksForCurrentElements()
	{
		UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
		bool flag = manager == null;
		if (!flag)
		{
			foreach (UIElement cell in this._curElements)
			{
				using (IEnumerator<UIElement> enumerator2 = UIManager.EnumerateLeafElements(cell).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						UIElement element = enumerator2.Current;
						bool flag2 = !manager.IsMaskedElement(element);
						if (!flag2)
						{
							bool flag3 = element.UiBase == null;
							if (!flag3)
							{
								manager.AttachMaskTo(element.UiBase.transform);
								UIElement element2 = element;
								element2.OnHide = (Action)Delegate.Combine(element2.OnHide, new Action(delegate()
								{
									bool flag4 = element.UiBase != null;
									if (flag4)
									{
										manager.DetachMask(element.UiBase.transform);
									}
								}));
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x0002F110 File Offset: 0x0002D310
	private static IEnumerable<UIElement> EnumerateLeafElements(UIElement elem)
	{
		UIGroup group = elem as UIGroup;
		bool flag = group != null;
		if (flag)
		{
			foreach (UIElement child in group.Elements)
			{
				foreach (UIElement leaf in UIManager.EnumerateLeafElements(child))
				{
					yield return leaf;
					leaf = null;
				}
				IEnumerator<UIElement> enumerator2 = null;
				child = null;
			}
			List<UIElement>.Enumerator enumerator = default(List<UIElement>.Enumerator);
		}
		else
		{
			yield return elem;
		}
		yield break;
		yield break;
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0002F120 File Offset: 0x0002D320
	public void MaskComponent(RectTransform rectTransform)
	{
		bool flag = rectTransform == null;
		if (!flag)
		{
			UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
			if (manager != null)
			{
				manager.AttachMaskTo(rectTransform);
			}
			rectTransform.gameObject.SetActive(true);
		}
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0002F15C File Offset: 0x0002D35C
	public void UnMaskComponent(RectTransform rectTransform)
	{
		bool flag = rectTransform == null;
		if (!flag)
		{
			rectTransform.gameObject.SetActive(false);
			UIMaskManager manager = SingletonObject.getInstance<UIMaskManager>();
			if (manager != null)
			{
				manager.DetachMask(rectTransform);
			}
			if (manager != null)
			{
				manager.MoveMaskToSafeParent();
			}
		}
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x0002F1A4 File Offset: 0x0002D3A4
	public void ChangeToUI(UIElement elem)
	{
		bool flag = elem == null;
		if (!flag)
		{
			try
			{
				bool flag2 = this._curElements != null && this._curElements.Count > 0;
				if (flag2)
				{
					List<UIElement> prevElements = new List<UIElement>(this._curElements);
					foreach (UIElement showingElem in prevElements)
					{
						bool flag3 = showingElem == elem;
						if (flag3)
						{
							return;
						}
					}
					UIElement elem2 = elem;
					Action<UIElement> <>9__1;
					elem2.OnShowed = (Action)Delegate.Combine(elem2.OnShowed, new Action(delegate()
					{
						List<UIElement> prevElements = prevElements;
						Action<UIElement> action;
						if ((action = <>9__1) == null)
						{
							action = (<>9__1 = delegate(UIElement cell)
							{
								bool flag6;
								if (cell != elem)
								{
									UIGroup group = elem as UIGroup;
									flag6 = (group != null && group.HasElement(cell));
								}
								else
								{
									flag6 = true;
								}
								bool flag7 = flag6;
								if (!flag7)
								{
									cell.Hide(false);
								}
							});
						}
						prevElements.ForEach(action);
					}));
				}
				GLog.LogWithTag(elem, Array.Empty<object>());
				this.BlockHotKey = true;
				this.RemoveUIElementCache(elem);
				UIElement elem3 = elem;
				elem3.OnShowed = (Action)Delegate.Combine(elem3.OnShowed, new Action(delegate()
				{
					this.BlockHotKey = false;
					this.CheckCollectUI();
				}));
				bool exist = UIElement.PermanentTips.Exist;
				if (exist)
				{
					UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().ClearAllPermanentTips();
				}
				bool flag4 = this._curElements != null;
				if (flag4)
				{
					this._curElements.Clear();
					this._curElements.Add(elem);
					bool flag5 = !UIElement.Loading.Exist;
					if (flag5)
					{
						GEvent.OnEvent(UiEvents.TopUiChanged, null);
					}
				}
				elem.Show();
				this.ClearStack();
			}
			catch (Exception e)
			{
				this.BlockHotKey = false;
				GLog.TagError("UIManager", e.ToString(), Array.Empty<object>());
			}
		}
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x0002F3C0 File Offset: 0x0002D5C0
	public void StackToUI(UIElement elem)
	{
		bool flag = elem == null;
		if (!flag)
		{
			List<UIElement> prevElements = EasyPool.Get<List<UIElement>>();
			bool flag2 = this._curElements != null && this._curElements.Count > 0;
			if (flag2)
			{
				prevElements.AddRange(this._curElements);
				bool flag3 = this._curElements.Contains(UIElement.BlackMask);
				if (flag3)
				{
					prevElements.Remove(UIElement.BlackMask);
				}
				List<UIElement> elements = EasyPool.Get<List<UIElement>>();
				foreach (UIElement cell in prevElements)
				{
					elements.AddRange(this.GetAllElements(cell));
				}
				foreach (UIElement cell2 in elements)
				{
					bool flag4;
					if (cell2 != elem)
					{
						UIGroup uiGroup = elem as UIGroup;
						flag4 = (uiGroup != null && uiGroup.HasElement(cell2));
					}
					else
					{
						flag4 = true;
					}
					bool flag5 = flag4;
					if (!flag5)
					{
						cell2.Hide(false);
					}
				}
				EasyPool.Free<List<UIElement>>(elements);
				UIGroup group;
				bool flag6;
				if (prevElements.Count == 1)
				{
					group = (prevElements[0] as UIGroup);
					flag6 = (group != null);
				}
				else
				{
					flag6 = false;
				}
				bool flag7 = flag6;
				UIGroup stackGroup;
				if (flag7)
				{
					stackGroup = group;
				}
				else
				{
					stackGroup = new UIGroup
					{
						Elements = new List<UIElement>(prevElements),
						TempGroup = true
					};
				}
				this._uiStack.Push(stackGroup);
				EasyPool.Free<List<UIElement>>(prevElements);
			}
			GLog.LogWithTag(elem, Array.Empty<object>());
			this.BlockHotKey = true;
			this.RemoveUIElementCache(elem);
			elem.OnShowed = (Action)Delegate.Combine(elem.OnShowed, new Action(delegate()
			{
				this.BlockHotKey = false;
				this.CheckCollectUI();
			}));
			bool flag8 = this._curElements != null;
			if (flag8)
			{
				this._curElements.Clear();
				this._curElements.Add(elem);
				bool flag9 = !UIElement.Loading.Exist;
				if (flag9)
				{
					GEvent.OnEvent(UiEvents.TopUiChanged, null);
				}
			}
			elem.Show();
			bool exist = UIElement.PermanentTips.Exist;
			if (exist)
			{
				UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().ClearAllPermanentTips();
			}
		}
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x0002F604 File Offset: 0x0002D804
	public void StackBack(UIElement ignoreElement = null)
	{
		bool flag = this._uiStack == null || this._uiStack.Count <= 0;
		if (!flag)
		{
			UIGroup elem = this._uiStack.Pop();
			bool flag2 = elem == null;
			if (!flag2)
			{
				try
				{
					bool flag3 = this._curElements != null && this._curElements.Count > 0;
					if (flag3)
					{
						List<UIElement> prevElements = new List<UIElement>(this._curElements);
						UIGroup elem3 = elem;
						Action<UIElement> <>9__1;
						elem3.OnShowed = (Action)Delegate.Combine(elem3.OnShowed, new Action(delegate()
						{
							List<UIElement> prevElements = prevElements;
							Action<UIElement> action;
							if ((action = <>9__1) == null)
							{
								action = (<>9__1 = delegate(UIElement cell)
								{
									List<UIElement> elemList = this.GetAllElements(cell);
									foreach (UIElement rawElement in elemList)
									{
										bool flag7;
										if (rawElement != elem)
										{
											if (elem != null)
											{
												UIGroup uiGroup = elem;
												flag7 = uiGroup.HasElement(rawElement);
											}
											else
											{
												flag7 = false;
											}
										}
										else
										{
											flag7 = true;
										}
										bool flag8 = flag7;
										if (!flag8)
										{
											bool flag9 = ignoreElement != null && rawElement == ignoreElement;
											if (!flag9)
											{
												rawElement.Hide(false);
											}
										}
									}
								});
							}
							prevElements.ForEach(action);
						}));
					}
					UIGroup elem2 = elem;
					elem2.OnShowed = (Action)Delegate.Combine(elem2.OnShowed, new Action(delegate()
					{
						this.BlockHotKey = false;
						this.CheckCollectUI();
						this.RestoreMasksForCurrentElements();
					}));
					this.BlockHotKey = true;
					this.RemoveUIElementCache(elem);
					bool exist = UIElement.PermanentTips.Exist;
					if (exist)
					{
						UIElement.PermanentTips.UiBaseAs<UI_PermanentTips>().ClearAllPermanentTips();
					}
					bool flag4 = this._curElements != null;
					if (flag4)
					{
						this._curElements.Clear();
						bool tempGroup = elem.TempGroup;
						if (tempGroup)
						{
							this._curElements.AddRange(elem.Elements);
						}
						else
						{
							this._curElements.Add(elem);
						}
						bool flag5 = ignoreElement != null && ignoreElement.IsShowing;
						if (flag5)
						{
							this._curElements.Add(ignoreElement);
						}
						bool flag6 = !UIElement.Loading.Exist;
						if (flag6)
						{
							GEvent.OnEvent(UiEvents.TopUiChanged, null);
						}
					}
					elem.Show();
				}
				catch (Exception e)
				{
					this.BlockHotKey = false;
					GLog.TagError("UIManager", e.ToString(), Array.Empty<object>());
				}
			}
		}
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x0002F838 File Offset: 0x0002DA38
	public void ClearStack()
	{
		this._uiStack.Clear();
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x0002F848 File Offset: 0x0002DA48
	public bool IsElementActive(UIElement element)
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.Loading;
		bool result;
		if (flag)
		{
			result = (element == UIElement.Loading);
		}
		else
		{
			bool flag2 = this._curElements != null;
			if (flag2)
			{
				foreach (UIElement cell in this._curElements)
				{
					UIGroup group = cell as UIGroup;
					bool flag3 = group != null;
					if (flag3)
					{
						bool flag4 = group.HasElement(element);
						if (flag4)
						{
							return true;
						}
						bool flag5 = cell == element;
						if (flag5)
						{
							return group.Elements.All((UIElement e) => e.Exist);
						}
					}
					bool flag6 = cell == element;
					if (flag6)
					{
						return element.Exist;
					}
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x0002F94C File Offset: 0x0002DB4C
	public bool IsFocusElement(UIElement element)
	{
		bool flag = this._curElements != null && this._curElements.Count > 0;
		bool result;
		if (flag)
		{
			UIElement topElement = this._curElements[this._curElements.Count - 1];
			bool flag2 = element is UIGroup && topElement is UIGroup;
			if (flag2)
			{
				result = (element == topElement);
			}
			else
			{
				UIGroup group = topElement as UIGroup;
				result = ((group != null) ? group.HasElement(element) : (topElement == element));
			}
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x0002F9D4 File Offset: 0x0002DBD4
	public bool IsInStack(UIElement element)
	{
		foreach (UIGroup state in this._uiStack)
		{
			bool flag = state.HasElement(element);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x0002FA3C File Offset: 0x0002DC3C
	public void HideAll()
	{
		bool flag = this._curElements == null;
		if (!flag)
		{
			List<UIElement> toHideList = new List<UIElement>(this._curElements);
			this._curElements.Clear();
			foreach (UIElement elem in toHideList)
			{
				elem.Hide(true);
			}
			GEvent.OnEvent(UiEvents.TopUiChanged, null);
		}
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0002FAC4 File Offset: 0x0002DCC4
	public static void DestroyUiBase(UIBase uiBase)
	{
		uiBase.Element.Hide(true);
		uiBase.Element.Destroy();
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x0002FAE0 File Offset: 0x0002DCE0
	public void DestroyAll(List<UIElement> keepUIs, bool force = false)
	{
		List<UIBase> uiBases = new List<UIBase>();
		foreach (KeyValuePair<UILayer, Transform> pair in this._layerRoots)
		{
			uiBases.AddRange(pair.Value.GetComponentsInTopChildren(true));
		}
		this._uiStack.Clear();
		uiBases.ForEach(delegate(UIBase cell)
		{
			bool flag = cell.Element != null;
			if (flag)
			{
				bool force2 = force;
				if (force2)
				{
					bool flag2 = !keepUIs.Contains(cell.Element);
					if (flag2)
					{
						UIManager.DestroyUiBase(cell);
					}
				}
				else
				{
					bool flag3 = !keepUIs.Contains(cell.Element) && !UIElement.ConstantElements.Contains(cell.Element) && !this.IsElementActive(cell.Element);
					if (flag3)
					{
						UIManager.DestroyUiBase(cell);
					}
				}
			}
		});
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x0002FB88 File Offset: 0x0002DD88
	public void SetMaskBackShow(bool showState)
	{
		bool flag = null != this.MaskBack;
		if (flag)
		{
			this.MaskBack.gameObject.SetActive(showState);
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x0002FBB8 File Offset: 0x0002DDB8
	private void InitLayerRoots()
	{
		this._layerRoots = new Dictionary<UILayer, Transform>();
		Transform canvasTrans = base.transform.GetChild(0);
		for (UILayer layer = UILayer.LayerBack; layer <= UILayer.LayerVeryTop; layer++)
		{
			Transform child = canvasTrans.Find(layer.ToString());
			bool flag = child;
			if (flag)
			{
				this._layerRoots.Add(layer, child);
			}
		}
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0002FC21 File Offset: 0x0002DE21
	public void SetEscHandler(Action handler)
	{
		this._escHandler = handler;
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x0002FC2C File Offset: 0x0002DE2C
	public bool CheckEscHandler(Action handler)
	{
		return this._escHandler == handler;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x0002FC4A File Offset: 0x0002DE4A
	public void SetCommonSortAndFilterEscHandler(Action handler)
	{
		this._commonSortAndFilterEscHandler = handler;
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x0002FC54 File Offset: 0x0002DE54
	private void CheckQuickHide()
	{
		bool blockHotKey = this.BlockHotKey;
		if (!blockHotKey)
		{
			UIElement topElement = this._curElements[this._curElements.Count - 1];
			bool escHit = CommonCommandKit.Esc.Check(topElement, false, false, false, true, false);
			bool rightMouseHit = CommonCommandKit.RightMouse.Check(topElement, false, false, false, true, false);
			bool flag = escHit || rightMouseHit;
			if (flag)
			{
				bool flag2 = this._commonSortAndFilterEscHandler != null;
				if (flag2)
				{
					Action handler = this._commonSortAndFilterEscHandler;
					this._commonSortAndFilterEscHandler = null;
					handler();
				}
				else
				{
					bool flag3 = this._escHandler != null;
					if (flag3)
					{
						Action handler2 = this._escHandler;
						this._escHandler = null;
						handler2();
						this.EscHandled = true;
					}
					else
					{
						bool flag4 = escHit && (topElement == UIElement.StateMainWorld || topElement == UIElement.TaiwuCrossArchive || topElement == UIElement.StateAdventureRemake || topElement == UIElement.StateBuilding || topElement == UIElement.StateAdventureRemakeSpecialBottom || topElement == UIElement.StateMajorEvent || topElement == UIElement.PastTaiwuVillage || topElement == UIElement.StateCombat || topElement == UIElement.LifeSkillCombatOld || topElement == UIElement.Debate);
						if (flag4)
						{
							bool flag5 = topElement != UIElement.StateCombat || !UIElement.CombatResult.Exist;
							if (flag5)
							{
								CommandManager.AddCommand<CommandMaskUI, UIElement>(EPriority.OpenUISystemOption, UIElement.SystemOption);
								this.EscHandled = true;
							}
						}
						else
						{
							bool leftMouseDown = CommonCommandKit.LeftMouse.Check(topElement, true, false, false, true, false);
							bool flag6 = !topElement.CanQuickHide || leftMouseDown;
							if (!flag6)
							{
								SingletonObject.getInstance<TooltipManager>().HideTips(TipType.Count, true);
								bool flag7 = null != topElement.UiBase;
								if (flag7)
								{
									topElement.UiBase.QuickHide();
									this.EscHandled = true;
								}
								else
								{
									bool flag8 = topElement is UIGroup && this._uiStack.Count > 0;
									if (flag8)
									{
										this.EscHandled = true;
										this.StackBack(null);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x0002FE48 File Offset: 0x0002E048
	private void CheckCollectUI()
	{
		bool flag = this._cachedElements.Count >= (int)this.MaxCacheElementCount;
		if (flag)
		{
			for (int i = this._cachedElements.Count - 1; i >= 0; i--)
			{
				UIElement cell = this._cachedElements[i];
				bool flag2 = UIElement.ConstantElements.Contains(cell) || this.IsInStack(cell);
				if (!flag2)
				{
					cell.Destroy();
					this._cachedElements.Remove(cell);
				}
			}
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x0002FED4 File Offset: 0x0002E0D4
	private List<UIElement> GetAllElements(UIElement elem)
	{
		List<UIElement> allElements = new List<UIElement>();
		UIGroup group = elem as UIGroup;
		bool flag = group != null;
		if (flag)
		{
			foreach (UIElement childElem in group.Elements)
			{
				bool flag2 = childElem is UIGroup;
				if (flag2)
				{
					allElements.AddRange(this.GetAllElements(childElem));
				}
				else
				{
					allElements.Add(childElem);
				}
			}
		}
		else
		{
			allElements.Add(elem);
		}
		return allElements;
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x0002FF7C File Offset: 0x0002E17C
	private void RemoveUIElementCache(UIElement element)
	{
		UIGroup group = element as UIGroup;
		bool flag = group != null;
		if (flag)
		{
			for (int i = 0; i < group.Elements.Count; i++)
			{
				this._cachedElements.Remove(group.Elements[i]);
			}
		}
		else
		{
			List<UIElement> cachedElements = this._cachedElements;
			if (cachedElements != null)
			{
				cachedElements.Remove(element);
			}
		}
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x0002FFE4 File Offset: 0x0002E1E4
	private void OnUIShow(ArgumentBox argBox)
	{
		UIElement element;
		bool flag = argBox.Get<UIElement>("Element", out element);
		if (flag)
		{
			this._cachedElements.Remove(element);
		}
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00030014 File Offset: 0x0002E214
	private void OnUIHide(ArgumentBox argBox)
	{
		UIElement element;
		bool flag = argBox.Get<UIElement>("Element", out element);
		if (flag)
		{
			bool flag2 = this._cachedElements.Contains(element) || UIElement.ConstantElements.Contains(element);
			if (!flag2)
			{
				this._cachedElements.Add(element);
			}
		}
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00030064 File Offset: 0x0002E264
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		bool flag = this.FXAA != null && this.FXAASwitch;
		if (flag)
		{
			Graphics.Blit(src, dest, this.FXAA);
		}
		else
		{
			Graphics.Blit(src, dest);
		}
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x000300A8 File Offset: 0x0002E2A8
	public bool CheckPopupElementIsInTop(UIElement element)
	{
		bool flag = element.UiBase == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Transform layerRoot = this._layerRoots[UILayer.LayerPopUp];
			int topIndex = layerRoot.childCount - 1;
			while (topIndex > 0)
			{
				Transform topElement = layerRoot.GetChild(topIndex);
				bool activeSelf = topElement.gameObject.activeSelf;
				if (activeSelf)
				{
					UIMask uimask;
					bool flag2 = topElement.TryGetComponent<UIMask>(out uimask);
					if (!flag2)
					{
						return element.UiBase.transform == topElement;
					}
					topIndex--;
				}
				else
				{
					topIndex--;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x0003013D File Offset: 0x0002E33D
	public void RemoveElement(UIElement element)
	{
		List<UIElement> curElements = this._curElements;
		if (curElements != null)
		{
			curElements.Remove(element);
		}
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00030153 File Offset: 0x0002E353
	public void InsertElementToTop(UIElement element)
	{
		this._curElements.Add(element);
	}

	// Token: 0x04000748 RID: 1864
	public sbyte MaxCacheElementCount;

	// Token: 0x04000749 RID: 1865
	private List<UIElement> _cachedElements;

	// Token: 0x0400074A RID: 1866
	private Stack<UIGroup> _uiStack;

	// Token: 0x0400074B RID: 1867
	private List<UIElement> _curElements;

	// Token: 0x0400074C RID: 1868
	private Dictionary<UILayer, Transform> _layerRoots;

	// Token: 0x0400074D RID: 1869
	private Action _escHandler;

	// Token: 0x0400074E RID: 1870
	private Action _commonSortAndFilterEscHandler;

	// Token: 0x0400074F RID: 1871
	[HideInInspector]
	public Camera UiCamera;

	// Token: 0x04000750 RID: 1872
	private static float _screenWidth;

	// Token: 0x04000751 RID: 1873
	private static float _screenHeight;

	// Token: 0x04000752 RID: 1874
	public CImage MaskTop;

	// Token: 0x04000753 RID: 1875
	public CImage MaskBottom;

	// Token: 0x04000754 RID: 1876
	public CImage MaskLeft;

	// Token: 0x04000755 RID: 1877
	public CImage MaskRight;

	// Token: 0x04000756 RID: 1878
	public CImage MaskBack;

	// Token: 0x04000757 RID: 1879
	public Material FXAA;

	// Token: 0x04000758 RID: 1880
	public bool FXAASwitch;

	// Token: 0x04000759 RID: 1881
	private bool _blockHotKey;
}
