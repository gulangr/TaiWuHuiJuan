using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200002B RID: 43
internal class LRUCache<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	// Token: 0x06000186 RID: 390 RVA: 0x0000A398 File Offset: 0x00008598
	public LRUCache(int capacity)
	{
		bool flag = capacity <= 0;
		if (flag)
		{
			throw new ArgumentOutOfRangeException("Cache capacity must be greater than 0");
		}
		this.Capacity = capacity;
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000187 RID: 391 RVA: 0x0000A3EB File Offset: 0x000085EB
	public int Capacity { get; }

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x06000188 RID: 392 RVA: 0x0000A3F3 File Offset: 0x000085F3
	public int Count
	{
		get
		{
			return this.keyToEntryTable.Count;
		}
	}

	// Token: 0x1700003B RID: 59
	public TValue this[TKey key]
	{
		get
		{
			TValue value;
			bool flag = this.TryGetValue(key, out value);
			if (flag)
			{
				return value;
			}
			throw new KeyNotFoundException(string.Format("Item with key '{0}' is not cached", key));
		}
		set
		{
			this.Add(key, value);
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600018B RID: 395 RVA: 0x0000A442 File Offset: 0x00008642
	public ICollection<TKey> Keys
	{
		get
		{
			return (from kv in this
			select kv.Key).ToList<TKey>();
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x0600018C RID: 396 RVA: 0x0000A46E File Offset: 0x0000866E
	public ICollection<TValue> Values
	{
		get
		{
			return (from kv in this
			select kv.Value).ToList<TValue>();
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000A49C File Offset: 0x0000869C
	public bool TryGetValue(TKey key, out TValue value)
	{
		LRUCache<TKey, TValue>.CacheEntry cacheEntry;
		bool flag = this.keyToEntryTable.TryGetValue(key, out cacheEntry);
		bool result;
		if (flag)
		{
			this.MakeCacheEntryRecent(cacheEntry);
			value = cacheEntry.Value;
			result = true;
		}
		else
		{
			value = default(TValue);
			result = false;
		}
		return result;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000A4E1 File Offset: 0x000086E1
	public bool ContainsKey(TKey key)
	{
		return this.keyToEntryTable.ContainsKey(key);
	}

	// Token: 0x0600018F RID: 399 RVA: 0x0000A4F0 File Offset: 0x000086F0
	public void Add(TKey key, TValue value)
	{
		LRUCache<TKey, TValue>.CacheEntry cacheEntry;
		bool flag = this.keyToEntryTable.TryGetValue(key, out cacheEntry);
		if (flag)
		{
			cacheEntry.Value = value;
		}
		else
		{
			bool flag2 = this.keyToEntryTable.Count >= this.Capacity && this.leastRecentEntry != null;
			if (flag2)
			{
				this.keyToEntryTable.Remove(this.leastRecentEntry.Key);
				LRUCache<TKey, TValue>.CacheEntry newLeastRecentEntry = this.leastRecentEntry.Previous;
				cacheEntry = this.leastRecentEntry;
				cacheEntry.Key = key;
				cacheEntry.Value = value;
				cacheEntry.Next = null;
				cacheEntry.Previous = null;
				this.leastRecentEntry = newLeastRecentEntry;
				this.leastRecentEntry.Next = null;
			}
			else
			{
				cacheEntry = new LRUCache<TKey, TValue>.CacheEntry(key, value);
				bool flag3 = this.leastRecentEntry == null;
				if (flag3)
				{
					this.leastRecentEntry = cacheEntry;
				}
			}
			this.keyToEntryTable.Add(key, cacheEntry);
		}
		this.MakeCacheEntryRecent(cacheEntry);
	}

	// Token: 0x06000190 RID: 400 RVA: 0x0000A5E0 File Offset: 0x000087E0
	public bool Remove(TKey key)
	{
		LRUCache<TKey, TValue>.CacheEntry cacheEntry;
		bool flag = this.keyToEntryTable.TryGetValue(key, out cacheEntry);
		bool result;
		if (flag)
		{
			this.keyToEntryTable.Remove(cacheEntry.Key);
			this.RemoveEntryFromList(cacheEntry);
			bool flag2 = cacheEntry == this.mostRecentEntry;
			if (flag2)
			{
				this.mostRecentEntry = cacheEntry.Next;
			}
			bool flag3 = cacheEntry == this.leastRecentEntry;
			if (flag3)
			{
				this.leastRecentEntry = cacheEntry.Previous;
			}
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000A660 File Offset: 0x00008860
	public void Clear()
	{
		this.keyToEntryTable.Clear();
		while (this.mostRecentEntry != null)
		{
			LRUCache<TKey, TValue>.CacheEntry nextEntry = this.mostRecentEntry.Next;
			this.mostRecentEntry.Next = null;
			bool flag = nextEntry != null;
			if (flag)
			{
				nextEntry.Previous = null;
			}
			this.mostRecentEntry = nextEntry;
		}
		this.leastRecentEntry = null;
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000A6C4 File Offset: 0x000088C4
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		bool flag = this.keyToEntryTable.Count == 0;
		if (flag)
		{
			yield break;
		}
		LRUCache<TKey, TValue>.CacheEntry entry;
		for (entry = this.mostRecentEntry; entry != null; entry = entry.Next)
		{
			yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
		}
		entry = null;
		yield break;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000A6D4 File Offset: 0x000088D4
	private void MakeCacheEntryRecent(LRUCache<TKey, TValue>.CacheEntry entry)
	{
		bool flag = entry != this.mostRecentEntry;
		if (flag)
		{
			this.RemoveEntryFromList(entry);
			bool flag2 = entry == this.leastRecentEntry && entry.Previous != null;
			if (flag2)
			{
				this.leastRecentEntry = entry.Previous;
			}
			entry.Previous = null;
			entry.Next = this.mostRecentEntry;
			bool flag3 = this.mostRecentEntry != null;
			if (flag3)
			{
				this.mostRecentEntry.Previous = entry;
			}
			this.mostRecentEntry = entry;
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000A75C File Offset: 0x0000895C
	private void RemoveEntryFromList(LRUCache<TKey, TValue>.CacheEntry entry)
	{
		bool flag = entry.Previous != null;
		if (flag)
		{
			entry.Previous.Next = entry.Next;
		}
		bool flag2 = entry.Next != null;
		if (flag2)
		{
			entry.Next.Previous = entry.Previous;
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000195 RID: 405 RVA: 0x0000A7AC File Offset: 0x000089AC
	bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly { get; } = 0;

	// Token: 0x06000196 RID: 406 RVA: 0x0000A7B4 File Offset: 0x000089B4
	void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> item)
	{
		this.Add(item.Key, item.Value);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000A7CC File Offset: 0x000089CC
	bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		TValue value;
		return this.TryGetValue(item.Key, out value) && object.Equals(value, item.Value);
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000A804 File Offset: 0x00008A04
	bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		return this.Remove(item.Key);
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000A814 File Offset: 0x00008A14
	void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
	{
		foreach (KeyValuePair<TKey, TValue> kv in this)
		{
			array[index++] = kv;
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000A868 File Offset: 0x00008A68
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// Token: 0x040000C0 RID: 192
	private readonly Dictionary<TKey, LRUCache<TKey, TValue>.CacheEntry> keyToEntryTable = new Dictionary<TKey, LRUCache<TKey, TValue>.CacheEntry>();

	// Token: 0x040000C1 RID: 193
	private LRUCache<TKey, TValue>.CacheEntry leastRecentEntry = null;

	// Token: 0x040000C2 RID: 194
	private LRUCache<TKey, TValue>.CacheEntry mostRecentEntry = null;

	// Token: 0x020010A3 RID: 4259
	private class CacheEntry
	{
		// Token: 0x17001593 RID: 5523
		// (get) Token: 0x0600C000 RID: 49152 RVA: 0x0056A634 File Offset: 0x00568834
		// (set) Token: 0x0600C001 RID: 49153 RVA: 0x0056A63C File Offset: 0x0056883C
		public TKey Key { get; set; }

		// Token: 0x17001594 RID: 5524
		// (get) Token: 0x0600C002 RID: 49154 RVA: 0x0056A645 File Offset: 0x00568845
		// (set) Token: 0x0600C003 RID: 49155 RVA: 0x0056A64D File Offset: 0x0056884D
		public TValue Value { get; set; }

		// Token: 0x17001595 RID: 5525
		// (get) Token: 0x0600C004 RID: 49156 RVA: 0x0056A656 File Offset: 0x00568856
		// (set) Token: 0x0600C005 RID: 49157 RVA: 0x0056A65E File Offset: 0x0056885E
		public LRUCache<TKey, TValue>.CacheEntry Previous { get; set; }

		// Token: 0x17001596 RID: 5526
		// (get) Token: 0x0600C006 RID: 49158 RVA: 0x0056A667 File Offset: 0x00568867
		// (set) Token: 0x0600C007 RID: 49159 RVA: 0x0056A66F File Offset: 0x0056886F
		public LRUCache<TKey, TValue>.CacheEntry Next { get; set; }

		// Token: 0x0600C008 RID: 49160 RVA: 0x0056A678 File Offset: 0x00568878
		public CacheEntry(TKey key, TValue item)
		{
			this.Key = key;
			this.Value = item;
		}
	}
}
