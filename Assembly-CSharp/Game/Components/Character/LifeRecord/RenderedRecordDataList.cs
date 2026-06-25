using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.LifeRecord;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F56 RID: 3926
	public class RenderedRecordDataList<TData> where TData : RenderedRecordDataBase, new()
	{
		// Token: 0x0600B3F1 RID: 46065 RVA: 0x0051DE52 File Offset: 0x0051C052
		public RenderedRecordDataList(Record<TData> parent)
		{
			this.Parent = parent;
		}

		// Token: 0x1700146A RID: 5226
		// (get) Token: 0x0600B3F2 RID: 46066 RVA: 0x0051DE80 File Offset: 0x0051C080
		public List<TransferableRecord> Filtered
		{
			get
			{
				return this._filtered ?? this.Data.Record;
			}
		}

		// Token: 0x1700146B RID: 5227
		public TData this[int index]
		{
			get
			{
				TData item;
				bool flag = this.Rendered.TryGetValue(this.Filtered.Count - index, out item);
				TData result;
				if (flag)
				{
					result = item;
				}
				else
				{
					bool flag2 = this.Filtered.Count > 0 && index < this.Filtered.Count;
					if (flag2)
					{
						result = (this.Rendered[this.Filtered.Count - index] = this.Parent.RenderData(this.Filtered[this.Filtered.Count - 1 - index], this.Data));
					}
					else
					{
						string format = "[GeneralRecord] illegal state detected, data has filtered count {0} and record count {1}, data is null: {2}\nRendered has count {3} but does not has {4} rendered; Filter has count {5} thus data i";
						object[] array = new object[6];
						int num = 0;
						List<TransferableRecord> filtered = this._filtered;
						array[num] = ((filtered != null) ? filtered.Count : -1);
						int num2 = 1;
						TransferableRecordDataBase data = this.Data;
						int? num3;
						if (data == null)
						{
							num3 = null;
						}
						else
						{
							List<TransferableRecord> record = data.Record;
							num3 = ((record != null) ? new int?(record.Count) : null);
						}
						array[num2] = (num3 ?? -1);
						array[2] = (this.Data == null);
						array[3] = this.Rendered.Count;
						array[4] = index;
						array[5] = this.Filtered.Count;
						Debug.LogWarning(string.Format(format, array));
						result = Activator.CreateInstance<TData>();
					}
				}
				return result;
			}
		}

		// Token: 0x1700146C RID: 5228
		// (get) Token: 0x0600B3F4 RID: 46068 RVA: 0x0051E008 File Offset: 0x0051C208
		public int Count
		{
			get
			{
				List<TransferableRecord> list;
				if ((list = this._filtered) == null)
				{
					TransferableRecordDataBase data = this.Data;
					list = ((data != null) ? data.Record : null);
				}
				List<TransferableRecord> list2 = list;
				return (list2 != null) ? list2.Count : 0;
			}
		}

		// Token: 0x0600B3F5 RID: 46069 RVA: 0x0051E034 File Offset: 0x0051C234
		public int GetDateStartIndex(int date, ESelectDateDirection direction = ESelectDateDirection.SelectDefault)
		{
			bool flag = this.Filtered.Count == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int x = this.Filtered.BinarySearch(new TransferableRecord(date, -2), this._comparer);
				int num;
				if (x < 0)
				{
					if (!true)
					{
					}
					int value;
					if (direction != ESelectDateDirection.SelectBefore)
					{
						if (direction != ESelectDateDirection.SelectAfter)
						{
							value = ~x;
						}
						else
						{
							value = ~x - 1;
						}
					}
					else
					{
						value = ~x;
					}
					if (!true)
					{
					}
					num = Math.Clamp(value, 0, this.Filtered.Count - 1);
				}
				else
				{
					num = x;
				}
				result = num;
			}
			return result;
		}

		// Token: 0x0600B3F6 RID: 46070 RVA: 0x0051E0C4 File Offset: 0x0051C2C4
		public bool SetId(int id, bool isDreamBack = false)
		{
			TransferableRecordDataBase data = this.Data;
			bool flag = data != null && data.CharId == id;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.Clear();
				result = true;
			}
			return result;
		}

		// Token: 0x0600B3F7 RID: 46071 RVA: 0x0051E0FB File Offset: 0x0051C2FB
		public void Clear()
		{
			this.Data = null;
			this._filtered = null;
			this.Rendered.Clear();
		}

		// Token: 0x0600B3F8 RID: 46072 RVA: 0x0051E118 File Offset: 0x0051C318
		public void ReadData(TransferableRecordDataBase data, bool pageMode = false)
		{
			bool flag;
			if (pageMode)
			{
				TransferableRecordDataBase data2 = this.Data;
				int? num = (data2 != null) ? new int?(data2.CharId) : null;
				int charId = data.CharId;
				flag = (num.GetValueOrDefault() == charId & num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				for (int i = this.Count - this.Data.HeaderCount + 1; i <= this.Data.Record.Count; i++)
				{
					this.Rendered.Remove(i);
				}
				this.Data.TransferData(data);
			}
			else
			{
				this.Data = data;
			}
			this.RefreshFilter();
		}

		// Token: 0x0600B3F9 RID: 46073 RVA: 0x0051E1D4 File Offset: 0x0051C3D4
		public void RefreshFilter()
		{
			this.Rendered.Clear();
			bool flag = this.Data.Record.Count > 0;
			if (flag)
			{
				List<TransferableRecord> record = this.Data.Record;
				int date = record[record.Count - 1].Date;
				int i = this.Data.Record.Count - 1;
				while (i >= this.Data.Record.Count - 5 && i >= 0)
				{
					bool flag2 = this.Data.Record[i].Date < date;
					if (flag2)
					{
						this.Data.Record[i].Date = date;
					}
					else
					{
						date = this.Data.Record[i].Date;
					}
					i--;
				}
			}
			this._filtered = ((this.CanShow != null) ? this.Data.Record.Where(this.CanShow).ToList<TransferableRecord>() : this.Data.Record.ToList<TransferableRecord>());
			bool[] filter = new bool[this._filtered.Count];
			Array.Fill<bool>(filter, true);
			int j = this._filtered.Count - 1;
			while (j-- > 0)
			{
				bool flag3 = this._filtered[j].RecordType == -3 && this._filtered[j + 1].RecordType == -2;
				if (flag3)
				{
					filter[j] = (filter[j + 1] = false);
				}
				else
				{
					bool flag4 = this._filtered[j].RecordType == -2 && this._filtered[j + 1].RecordType == -2;
					if (flag4)
					{
						filter[j + 1] = false;
					}
				}
			}
			for (int k = 0; k < this._filtered.Count; k++)
			{
				bool flag5 = this._filtered[k].RecordType == -2;
				if (!flag5)
				{
					break;
				}
				filter[k] = false;
			}
			this._filtered = this._filtered.Where((TransferableRecord _, int index) => filter[index]).ToList<TransferableRecord>();
		}

		// Token: 0x04008BEE RID: 35822
		public Record<TData> Parent;

		// Token: 0x04008BEF RID: 35823
		public TransferableRecordDataBase Data;

		// Token: 0x04008BF0 RID: 35824
		public readonly Dictionary<int, TData> Rendered = new Dictionary<int, TData>();

		// Token: 0x04008BF1 RID: 35825
		private List<TransferableRecord> _filtered = null;

		// Token: 0x04008BF2 RID: 35826
		private RenderedRecordDataList<TData>.Comparer _comparer = default(RenderedRecordDataList<TData>.Comparer);

		// Token: 0x04008BF3 RID: 35827
		public Func<TransferableRecord, bool> CanShow;

		// Token: 0x0200258B RID: 9611
		private struct Comparer : IComparer<TransferableRecord>
		{
			// Token: 0x06010C07 RID: 68615 RVA: 0x0066E194 File Offset: 0x0066C394
			public int Compare(TransferableRecord y, TransferableRecord x)
			{
				int res = x.Date.CompareTo(y.Date);
				return (res != 0) ? res : (x.RecordType != -2).CompareTo(y.RecordType != -2);
			}
		}
	}
}
