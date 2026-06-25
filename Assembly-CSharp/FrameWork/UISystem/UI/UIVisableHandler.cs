using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FrameWork.UISystem.UI
{
	// Token: 0x02000FF8 RID: 4088
	public class UIVisableHandler : IDisposable
	{
		// Token: 0x1400008E RID: 142
		// (add) Token: 0x0600BAA1 RID: 47777 RVA: 0x005504B4 File Offset: 0x0054E6B4
		// (remove) Token: 0x0600BAA2 RID: 47778 RVA: 0x005504E8 File Offset: 0x0054E6E8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnAnyUIVisableChanged;

		// Token: 0x0600BAA3 RID: 47779 RVA: 0x0055051C File Offset: 0x0054E71C
		public UIVisableHandler()
		{
			GEvent.Add(UiEvents.OnUIElementShow, new GEvent.Callback(this.OnElementShow));
			GEvent.Add(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnElementHide));
			GEvent.Add(UiEvents.OnUIElementShowFinish, new GEvent.Callback(this.OnElementFinishShow));
			GEvent.Add(UiEvents.OnUIElementHideStart, new GEvent.Callback(this.OnElementStartHide));
		}

		// Token: 0x0600BAA4 RID: 47780 RVA: 0x005505C4 File Offset: 0x0054E7C4
		public void Dispose()
		{
			GEvent.Remove(UiEvents.OnUIElementShow, new GEvent.Callback(this.OnElementShow));
			GEvent.Remove(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnElementHide));
			GEvent.Remove(UiEvents.OnUIElementShowFinish, new GEvent.Callback(this.OnElementFinishShow));
			GEvent.Remove(UiEvents.OnUIElementHideStart, new GEvent.Callback(this.OnElementStartHide));
		}

		// Token: 0x0600BAA5 RID: 47781 RVA: 0x00550634 File Offset: 0x0054E834
		private void OnElementFinishShow(ArgumentBox arg)
		{
			UIElement element;
			bool flag = !arg.Get<UIElement>("Element", out element);
			if (!flag)
			{
				bool flag2 = this.IsIgnoreAnim(element);
				if (!flag2)
				{
					bool flag3 = !this.AddElement(element);
					if (!flag3)
					{
						this._isNeedSendChangedEvent = true;
					}
				}
			}
		}

		// Token: 0x0600BAA6 RID: 47782 RVA: 0x00550680 File Offset: 0x0054E880
		private void OnElementStartHide(ArgumentBox arg)
		{
			UIElement element;
			bool flag = !arg.Get<UIElement>("Element", out element);
			if (!flag)
			{
				bool flag2 = this.IsIgnoreAnim(element);
				if (!flag2)
				{
					bool flag3 = !this.RemoveElement(element);
					if (!flag3)
					{
						this._isNeedSendChangedEvent = true;
					}
				}
			}
		}

		// Token: 0x0600BAA7 RID: 47783 RVA: 0x005506CC File Offset: 0x0054E8CC
		private void OnElementShow(ArgumentBox arg)
		{
			UIElement element;
			bool flag = !arg.Get<UIElement>("Element", out element);
			if (!flag)
			{
				bool flag2 = !this.IsIgnoreAnim(element);
				if (!flag2)
				{
					bool flag3 = !this.AddElement(element);
					if (!flag3)
					{
						this._isNeedSendChangedEvent = true;
					}
				}
			}
		}

		// Token: 0x0600BAA8 RID: 47784 RVA: 0x0055071C File Offset: 0x0054E91C
		private void OnElementHide(ArgumentBox arg)
		{
			UIElement element;
			bool flag = !arg.Get<UIElement>("Element", out element);
			if (!flag)
			{
				bool flag2 = !this.IsIgnoreAnim(element);
				if (!flag2)
				{
					bool flag3 = !this.RemoveElement(element);
					if (!flag3)
					{
						this._isNeedSendChangedEvent = true;
					}
				}
			}
		}

		// Token: 0x0600BAA9 RID: 47785 RVA: 0x0055076C File Offset: 0x0054E96C
		public void TrySendCoverStateChangeEvent()
		{
			bool isNeedSendChangedEvent = this._isNeedSendChangedEvent;
			if (isNeedSendChangedEvent)
			{
				this._isNeedSendChangedEvent = false;
				Action onAnyUIVisableChanged = UIVisableHandler.OnAnyUIVisableChanged;
				if (onAnyUIVisableChanged != null)
				{
					onAnyUIVisableChanged();
				}
			}
		}

		// Token: 0x0600BAAA RID: 47786 RVA: 0x005507A0 File Offset: 0x0054E9A0
		private bool IsIgnoreAnim(UIElement element)
		{
			return element.UiFlags.HasFlag(UIFlag.FullCoverIgnoreAnim);
		}

		// Token: 0x0600BAAB RID: 47787 RVA: 0x005507C8 File Offset: 0x0054E9C8
		private bool AddElement(UIElement element)
		{
			bool flag = !element.UiFlags.HasFlag(UIFlag.IncludeCoverCheck);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._elementNodeLookup.ContainsKey(element);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = element.UiBase == null;
					if (flag3)
					{
						result = false;
					}
					else
					{
						Canvas canvas = element.UiBase.GetComponentInParent<Canvas>(true);
						bool flag4 = canvas == null;
						if (flag4)
						{
							result = false;
						}
						else
						{
							LinkedListNode<UIVisableHandler.UIInfo> node = this.GetEmptyNode();
							node.Value = new UIVisableHandler.UIInfo
							{
								element = element,
								canvas = canvas
							};
							this.AddToList(node);
							this._elementNodeLookup[element] = node;
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600BAAC RID: 47788 RVA: 0x0055088C File Offset: 0x0054EA8C
		private bool RemoveElement(UIElement element)
		{
			LinkedListNode<UIVisableHandler.UIInfo> node;
			bool flag = !this._elementNodeLookup.TryGetValue(element, out node);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.RemoveNode(node);
				result = true;
			}
			return result;
		}

		// Token: 0x0600BAAD RID: 47789 RVA: 0x005508C4 File Offset: 0x0054EAC4
		private void RemoveNode(LinkedListNode<UIVisableHandler.UIInfo> node)
		{
			this._showingElements.Remove(node);
			this._elementNodeLookup.Remove(node.Value.element);
			node.Value = default(UIVisableHandler.UIInfo);
			this._tempStack.Push(node);
		}

		// Token: 0x0600BAAE RID: 47790 RVA: 0x00550914 File Offset: 0x0054EB14
		private LinkedListNode<UIVisableHandler.UIInfo> GetEmptyNode()
		{
			LinkedListNode<UIVisableHandler.UIInfo> node;
			bool flag = !this._tempStack.TryPop(out node);
			if (flag)
			{
				node = new LinkedListNode<UIVisableHandler.UIInfo>(default(UIVisableHandler.UIInfo));
			}
			return node;
		}

		// Token: 0x0600BAAF RID: 47791 RVA: 0x0055094C File Offset: 0x0054EB4C
		private void AddToList(LinkedListNode<UIVisableHandler.UIInfo> node)
		{
			bool flag = this._showingElements.Count == 0;
			if (flag)
			{
				this._showingElements.AddFirst(node);
			}
			else
			{
				LinkedListNode<UIVisableHandler.UIInfo> current = this._showingElements.First;
				while (current != null)
				{
					bool isNull = current.Value.isNull;
					if (isNull)
					{
						LinkedListNode<UIVisableHandler.UIInfo> temp = current.Next;
						this.RemoveNode(current);
						current = temp;
					}
					else
					{
						bool flag2 = this.IsBefore(node.Value, current.Value);
						if (flag2)
						{
							this._showingElements.AddBefore(current, node);
							return;
						}
						current = current.Next;
					}
				}
				this._showingElements.AddLast(node);
			}
		}

		// Token: 0x0600BAB0 RID: 47792 RVA: 0x005509FC File Offset: 0x0054EBFC
		private bool IsBefore(UIVisableHandler.UIInfo a, UIVisableHandler.UIInfo b)
		{
			bool flag = a.element == b.element;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = a.canvas.sortingLayerID != b.canvas.sortingLayerID;
				if (flag2)
				{
					result = (a.canvas.sortingLayerID < b.canvas.sortingLayerID);
				}
				else
				{
					bool flag3 = a.canvas.sortingOrder != b.canvas.sortingOrder;
					if (flag3)
					{
						result = (a.canvas.sortingOrder < b.canvas.sortingOrder);
					}
					else
					{
						result = (a.element.UiBase.transform.GetSiblingIndex() < b.element.UiBase.transform.GetSiblingIndex());
					}
				}
			}
			return result;
		}

		// Token: 0x0600BAB1 RID: 47793 RVA: 0x00550ACC File Offset: 0x0054ECCC
		private bool IsBefore(UIVisableHandler.UIInfo a, int sortingLayerID, int sortingOrder, int siblingIndex)
		{
			bool flag = a.canvas.sortingLayerID != sortingLayerID;
			bool result;
			if (flag)
			{
				result = (a.canvas.sortingLayerID < sortingLayerID);
			}
			else
			{
				bool flag2 = a.canvas.sortingOrder != sortingOrder;
				if (flag2)
				{
					result = (a.canvas.sortingOrder < sortingOrder);
				}
				else
				{
					result = (a.element.UiBase.transform.GetSiblingIndex() < siblingIndex);
				}
			}
			return result;
		}

		// Token: 0x0600BAB2 RID: 47794 RVA: 0x00550B48 File Offset: 0x0054ED48
		public void ForceUpdateElements()
		{
			List<UIElement> elements = this._tempElementList;
			for (LinkedListNode<UIVisableHandler.UIInfo> current = this._showingElements.First; current != null; current = current.Next)
			{
				elements.Add(current.Value.element);
				current.Value = default(UIVisableHandler.UIInfo);
				this._tempStack.Push(current);
			}
			this._elementNodeLookup.Clear();
			foreach (UIElement ele in elements)
			{
				bool flag = !ele.UiFlags.HasFlag(UIFlag.IncludeCoverCheck) || ele.UiBase == null;
				if (!flag)
				{
					this.AddElement(ele);
				}
			}
			this._tempElementList.Clear();
			Action onAnyUIVisableChanged = UIVisableHandler.OnAnyUIVisableChanged;
			if (onAnyUIVisableChanged != null)
			{
				onAnyUIVisableChanged();
			}
		}

		// Token: 0x0600BAB3 RID: 47795 RVA: 0x00550C50 File Offset: 0x0054EE50
		public bool IsFullCovered(UIElement element)
		{
			bool flag = element == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				LinkedListNode<UIVisableHandler.UIInfo> node;
				bool flag2 = !this._elementNodeLookup.TryGetValue(element, out node);
				if (flag2)
				{
					result = false;
				}
				else
				{
					LinkedListNode<UIVisableHandler.UIInfo> current = node.Next;
					while (current != null)
					{
						bool isNull = current.Value.isNull;
						if (isNull)
						{
							LinkedListNode<UIVisableHandler.UIInfo> temp = current.Next;
							this.RemoveNode(current);
							current = temp;
						}
						else
						{
							bool flag3 = current.Value.element.UiFlags.HasFlag(UIFlag.FullCover);
							if (flag3)
							{
								return true;
							}
							current = current.Next;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600BAB4 RID: 47796 RVA: 0x00550D04 File Offset: 0x0054EF04
		public bool IsFullCovered(int sortingLayerID, int sortingOrder, int siblingIndex)
		{
			LinkedListNode<UIVisableHandler.UIInfo> current = this._showingElements.Last;
			while (current != null)
			{
				bool isNull = current.Value.isNull;
				if (isNull)
				{
					LinkedListNode<UIVisableHandler.UIInfo> temp = current.Previous;
					this.RemoveNode(current);
					current = temp;
				}
				else
				{
					bool flag = this.IsBefore(current.Value, sortingLayerID, sortingOrder, siblingIndex);
					if (flag)
					{
						break;
					}
					bool flag2 = current.Value.element.UiFlags.HasFlag(UIFlag.FullCover);
					if (flag2)
					{
						return true;
					}
					current = current.Previous;
				}
			}
			return false;
		}

		// Token: 0x0400903A RID: 36922
		private LinkedList<UIVisableHandler.UIInfo> _showingElements = new LinkedList<UIVisableHandler.UIInfo>();

		// Token: 0x0400903B RID: 36923
		private Dictionary<UIElement, LinkedListNode<UIVisableHandler.UIInfo>> _elementNodeLookup = new Dictionary<UIElement, LinkedListNode<UIVisableHandler.UIInfo>>();

		// Token: 0x0400903C RID: 36924
		private Stack<LinkedListNode<UIVisableHandler.UIInfo>> _tempStack = new Stack<LinkedListNode<UIVisableHandler.UIInfo>>();

		// Token: 0x0400903D RID: 36925
		private List<UIElement> _tempElementList = new List<UIElement>();

		// Token: 0x0400903E RID: 36926
		private bool _isNeedSendChangedEvent = false;

		// Token: 0x0200263A RID: 9786
		private struct UIInfo
		{
			// Token: 0x17001BAA RID: 7082
			// (get) Token: 0x06011B51 RID: 72529 RVA: 0x0068720D File Offset: 0x0068540D
			public bool isNull
			{
				get
				{
					return this.element.UiBase == null || this.canvas == null;
				}
			}

			// Token: 0x0400EA00 RID: 59904
			public UIElement element;

			// Token: 0x0400EA01 RID: 59905
			public Canvas canvas;
		}
	}
}
