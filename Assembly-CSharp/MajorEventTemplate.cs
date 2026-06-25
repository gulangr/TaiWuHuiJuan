using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using UnityEngine;

// Token: 0x02000192 RID: 402
public abstract class MajorEventTemplate<T> : MonoBehaviour
{
	// Token: 0x1700027E RID: 638
	// (get) Token: 0x060016A3 RID: 5795 RVA: 0x0008B09F File Offset: 0x0008929F
	public T Data
	{
		get
		{
			return this.DataList[this.Index];
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x060016A4 RID: 5796
	public abstract IList<T> DataList { get; }

	// Token: 0x060016A5 RID: 5797 RVA: 0x0008B0B4 File Offset: 0x000892B4
	public void RefreshData(int index)
	{
		this.Index = index;
		bool flag = index < this.DataList.Count;
		if (flag)
		{
			this.RefreshData();
		}
	}

	// Token: 0x060016A6 RID: 5798
	public abstract void RefreshData();

	// Token: 0x060016A7 RID: 5799
	public abstract void RefreshAll();

	// Token: 0x060016A8 RID: 5800 RVA: 0x0008B0E4 File Offset: 0x000892E4
	public virtual void Remove()
	{
		this.DataList.RemoveAt(this.Index);
		this.RefreshAll();
	}

	// Token: 0x04001262 RID: 4706
	public int Index;

	// Token: 0x04001263 RID: 4707
	[SerializeField]
	protected UI_AdventureMajorEventEditor parent;

	// Token: 0x04001264 RID: 4708
	[SerializeField]
	protected CButton delete;
}
