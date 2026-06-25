using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000033 RID: 51
public class OrderedHashSet<T> : IEnumerable<!0>, IEnumerable
{
	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060001CB RID: 459 RVA: 0x0000B7AC File Offset: 0x000099AC
	public int Count
	{
		get
		{
			return this._items.Count;
		}
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000B7BC File Offset: 0x000099BC
	public bool Add(T item)
	{
		bool flag = this._indexMap.ContainsKey(item);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			this._indexMap[item] = this._items.Count;
			this._items.Add(item);
			result = true;
		}
		return result;
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000B808 File Offset: 0x00009A08
	public bool Remove(T item)
	{
		int index;
		bool flag = !this._indexMap.TryGetValue(item, out index);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			this._indexMap.Remove(item);
			int lastIndex = this._items.Count - 1;
			bool flag2 = index != lastIndex;
			if (flag2)
			{
				T last = this._items[lastIndex];
				this._items[index] = last;
				this._indexMap[last] = index;
			}
			this._items.RemoveAt(lastIndex);
			result = true;
		}
		return result;
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000B897 File Offset: 0x00009A97
	public bool Contains(T item)
	{
		return this._indexMap.ContainsKey(item);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000B8A8 File Offset: 0x00009AA8
	public int IndexOf(T item)
	{
		int index;
		return this._indexMap.TryGetValue(item, out index) ? index : -1;
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000B8C9 File Offset: 0x00009AC9
	public T GetByIndex(int index)
	{
		return this._items[index];
	}

	// Token: 0x17000041 RID: 65
	public T this[int index]
	{
		get
		{
			return this._items[index];
		}
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000B8E5 File Offset: 0x00009AE5
	public IEnumerator<T> GetEnumerator()
	{
		return this._items.GetEnumerator();
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000B8F7 File Offset: 0x00009AF7
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// Token: 0x040000ED RID: 237
	private readonly List<T> _items = new List<T>();

	// Token: 0x040000EE RID: 238
	private readonly Dictionary<T, int> _indexMap = new Dictionary<T, int>();
}
