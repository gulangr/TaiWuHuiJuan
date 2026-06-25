using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x020000C4 RID: 196
public class UIGroup : UIElement
{
	// Token: 0x0600069D RID: 1693 RVA: 0x0002E49C File Offset: 0x0002C69C
	public override void Show()
	{
		this.StateReady = false;
		int listenerIdReadyCount = 0;
		int showedCount = 0;
		using (List<UIElement>.Enumerator enumerator = this.Elements.GetEnumerator())
		{
			Action <>9__0;
			while (enumerator.MoveNext())
			{
				UIElement elem = enumerator.Current;
				UIElement elem3 = elem;
				Delegate onListenerIdReady = elem3.OnListenerIdReady;
				Action b;
				if ((b = <>9__0) == null)
				{
					b = (<>9__0 = delegate()
					{
						int listenerIdReadyCount = listenerIdReadyCount;
						listenerIdReadyCount++;
						bool flag = listenerIdReadyCount == this.Elements.Count;
						if (flag)
						{
							Action onListenerIdReady2 = this.OnListenerIdReady;
							if (onListenerIdReady2 != null)
							{
								onListenerIdReady2();
							}
						}
					});
				}
				elem3.OnListenerIdReady = (Action)Delegate.Combine(onListenerIdReady, b);
				UIElement elem2 = elem;
				elem2.OnShowed = (Action)Delegate.Combine(elem2.OnShowed, new Action(delegate()
				{
					elem.ServeGroup = this;
					int showedCount = showedCount;
					showedCount++;
					bool flag = showedCount == this.Elements.Count;
					if (flag)
					{
						for (int i = 0; i < this.Elements.Count; i++)
						{
							this.Elements[i].SetTransformToLastSibling();
						}
						Action onShowed = this.OnShowed;
						if (onShowed != null)
						{
							onShowed();
						}
						this.OnShowed = null;
						this.StateReady = true;
					}
				}));
				elem.Show();
			}
		}
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0002E5A4 File Offset: 0x0002C7A4
	public override void Hide(bool quickHide = false)
	{
		this.StateReady = false;
		int finishCnt = 0;
		Action <>9__0;
		foreach (UIElement elem in this.Elements)
		{
			bool flag = UIManager.Instance.IsElementActive(elem);
			if (flag)
			{
				int finishCnt3 = finishCnt;
				finishCnt = finishCnt3 + 1;
				bool flag2 = finishCnt == this.Elements.Count;
				if (flag2)
				{
					Action onHide = this.OnHide;
					if (onHide != null)
					{
						onHide();
					}
					this.OnHide = null;
				}
			}
			else
			{
				UIElement uielement = elem;
				Delegate onHide2 = uielement.OnHide;
				Action b;
				if ((b = <>9__0) == null)
				{
					b = (<>9__0 = delegate()
					{
						int finishCnt2 = finishCnt;
						finishCnt = finishCnt2 + 1;
						bool flag3 = finishCnt == this.Elements.Count;
						if (flag3)
						{
							Action onHide3 = this.OnHide;
							if (onHide3 != null)
							{
								onHide3();
							}
							this.OnHide = null;
						}
					});
				}
				uielement.OnHide = (Action)Delegate.Combine(onHide2, b);
				elem.Hide(quickHide);
			}
		}
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x0002E6B0 File Offset: 0x0002C8B0
	public override void SetTransformToLastSibling()
	{
		foreach (UIElement cell in this.Elements)
		{
			cell.SetTransformToLastSibling();
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0002E708 File Offset: 0x0002C908
	public bool HasElement(UIElement elem)
	{
		foreach (UIElement cell in this.Elements)
		{
			bool flag = cell == elem;
			if (flag)
			{
				return true;
			}
			UIGroup group = cell as UIGroup;
			bool flag2 = group != null && group.HasElement(elem);
			if (flag2)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0002E78C File Offset: 0x0002C98C
	public override bool NeedWaitData()
	{
		foreach (UIElement cell in this.Elements)
		{
			bool flag = cell.NeedWaitData();
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0002E7F0 File Offset: 0x0002C9F0
	public override string ToString()
	{
		StringBuilder sbuilder = new StringBuilder();
		sbuilder.Append("[");
		int finishCnt = 0;
		foreach (UIElement elem in this.Elements)
		{
			sbuilder.Append(elem.ToString());
			finishCnt++;
			bool flag = finishCnt != this.Elements.Count;
			if (flag)
			{
				sbuilder.Append("|");
			}
		}
		sbuilder.Append("]");
		return sbuilder.ToString();
	}

	// Token: 0x0400073D RID: 1853
	public bool TempGroup;

	// Token: 0x0400073E RID: 1854
	public List<UIElement> Elements;

	// Token: 0x0400073F RID: 1855
	public bool StateReady;
}
