using System;
using System.Collections;
using System.Diagnostics;
using GameData.Domains.LifeRecord;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F55 RID: 3925
	public abstract class Record<TData> : MonoBehaviour, ILifeRecord where TData : RenderedRecordDataBase, new()
	{
		// Token: 0x17001467 RID: 5223
		// (get) Token: 0x0600B3DA RID: 46042 RVA: 0x0051D9E9 File Offset: 0x0051BBE9
		public TransferableRecordDataBase Data
		{
			get
			{
				return this._dataList.Data;
			}
		}

		// Token: 0x17001468 RID: 5224
		// (get) Token: 0x0600B3DB RID: 46043 RVA: 0x0051D9F6 File Offset: 0x0051BBF6
		public RenderedRecordDataList<TData> DataList
		{
			get
			{
				return this._dataList;
			}
		}

		// Token: 0x0600B3DC RID: 46044 RVA: 0x0051DA00 File Offset: 0x0051BC00
		public void ScrollToCell(int index, bool smooth = true, bool requestRefill = false)
		{
			bool flag = index != this.scrollRect.ItemTypeStart;
			if (flag)
			{
				this.scrollRect.SrollToCell(index, (float)((smooth && Math.Abs(this.scrollRect.ItemTypeStart - index) < this.refillOrScrollDiff && base.gameObject.activeInHierarchy) ? this.refillScrollSpeed : 0));
			}
			else if (requestRefill)
			{
				this.scrollRect.RefillCellsAtCurrentPosition();
			}
		}

		// Token: 0x0600B3DD RID: 46045 RVA: 0x0051DA78 File Offset: 0x0051BC78
		public void ScrollToMonth(int date, bool smooth, bool requestRefill = false, ESelectDateDirection direction = ESelectDateDirection.SelectDefault)
		{
			bool flag = (this.scrollRect.totalCount = this._dataList.Count) == 0;
			if (flag)
			{
				this.scrollRect.RefillCells(0, false);
			}
			else
			{
				int index = this._dataList.Count - 1 - this._dataList.GetDateStartIndex(date, ESelectDateDirection.SelectDefault);
				this.ScrollToCell(index, smooth, requestRefill);
				Action onScrollEvent = this.OnScrollEvent;
				if (onScrollEvent != null)
				{
					onScrollEvent();
				}
			}
		}

		// Token: 0x0600B3DE RID: 46046 RVA: 0x0051DAF0 File Offset: 0x0051BCF0
		public void OnDimensionsChange()
		{
			bool flag = this.scrollRect.totalCount == 0;
			if (!flag)
			{
				bool flag2 = this.scrollRect.ItemTypeStart == 0;
				if (flag2)
				{
					this.ScrollToCell(0, true, false);
				}
				else
				{
					bool flag3 = this.scrollRect.ItemTypeEnd > this.scrollRect.totalCount - 2;
					if (flag3)
					{
						this.ScrollToEnd();
					}
				}
			}
		}

		// Token: 0x0600B3DF RID: 46047 RVA: 0x0051DB55 File Offset: 0x0051BD55
		public void ScrollToEnd()
		{
			this.scrollRect.RefillCellsFromEnd(0, false);
			base.StartCoroutine(this.DelayedScrollToEnd());
		}

		// Token: 0x0600B3E0 RID: 46048 RVA: 0x0051DB73 File Offset: 0x0051BD73
		private IEnumerator DelayedScrollToEnd()
		{
			yield return null;
			bool flag = this.scrollRect && base.gameObject.activeSelf && this.scrollRect.totalCount > 0;
			if (flag)
			{
				this.scrollRect.RefillCellsFromEnd(0, false);
			}
			yield break;
		}

		// Token: 0x0600B3E1 RID: 46049 RVA: 0x0051DB84 File Offset: 0x0051BD84
		public void TryScrollToEnd()
		{
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				this.ScrollToEnd();
			}
			else
			{
				this._needScrollToEnd = true;
			}
		}

		// Token: 0x0600B3E2 RID: 46050 RVA: 0x0051DBB4 File Offset: 0x0051BDB4
		public void OnEnable()
		{
			bool flag = !this._needScrollToEnd;
			if (!flag)
			{
				this._needScrollToEnd = false;
				this.ScrollToEnd();
			}
		}

		// Token: 0x0600B3E3 RID: 46051 RVA: 0x0051DBE0 File Offset: 0x0051BDE0
		public void RefreshFilter(Func<TransferableRecord, bool> canShow)
		{
			int date = this.CurrDate;
			this._dataList.CanShow = canShow;
			this._dataList.RefreshFilter();
			this.scrollRect.totalCount = this._dataList.Count;
			bool flag = this._dataList.Count > 0;
			if (flag)
			{
				int index = this._dataList.Count - 1 - this._dataList.GetDateStartIndex(date, ESelectDateDirection.SelectDefault);
				this.scrollRect.RefillCells(Math.Max(0, index), false);
			}
			else
			{
				this.scrollRect.RefillCells(0, false);
			}
			Action onScrollEvent = this.OnScrollEvent;
			if (onScrollEvent != null)
			{
				onScrollEvent();
			}
		}

		// Token: 0x1400008B RID: 139
		// (add) Token: 0x0600B3E4 RID: 46052 RVA: 0x0051DC8C File Offset: 0x0051BE8C
		// (remove) Token: 0x0600B3E5 RID: 46053 RVA: 0x0051DCC4 File Offset: 0x0051BEC4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action OnScrollEvent;

		// Token: 0x0600B3E6 RID: 46054 RVA: 0x0051DCF9 File Offset: 0x0051BEF9
		public void OnScroll(float val)
		{
			this.ScrollToCell((int)((float)this._dataList.Count * val), true, false);
		}

		// Token: 0x17001469 RID: 5225
		// (get) Token: 0x0600B3E7 RID: 46055 RVA: 0x0051DD14 File Offset: 0x0051BF14
		public int CurrDate
		{
			get
			{
				return (this.scrollRect.ItemTypeStart < this._dataList.Count) ? this._dataList.Filtered[this._dataList.Count - 1 - (this.scrollRect.ItemTypeStart * 3 + this.scrollRect.ItemTypeEnd) / 4].Date : 1;
			}
		}

		// Token: 0x0600B3E8 RID: 46056 RVA: 0x0051DD7C File Offset: 0x0051BF7C
		public void AddScrollEvent(Action<int> cb)
		{
			this.OnScrollEvent += delegate()
			{
				bool flag = this.scrollRect.ItemTypeStart < this._dataList.Count;
				if (flag)
				{
					cb(Math.Max(1, this.CurrDate));
				}
			};
		}

		// Token: 0x0600B3E9 RID: 46057 RVA: 0x0051DDB4 File Offset: 0x0051BFB4
		public virtual TData RenderData(TransferableRecord record, TransferableRecordDataBase dataBase)
		{
			TData data = Activator.CreateInstance<TData>();
			data.SetData(record, dataBase);
			return data;
		}

		// Token: 0x0600B3EA RID: 46058
		protected abstract void RegisterPrefabs();

		// Token: 0x0600B3EB RID: 46059
		protected abstract void UnregisterPrefabs();

		// Token: 0x0600B3EC RID: 46060
		protected abstract void AwakeScroll();

		// Token: 0x0600B3ED RID: 46061 RVA: 0x0051DDDB File Offset: 0x0051BFDB
		private void Awake()
		{
			this.RegisterPrefabs();
			this.AwakeScroll();
			this.scrollRect.OnScrollEvent += delegate()
			{
				Action onScrollEvent = this.OnScrollEvent;
				if (onScrollEvent != null)
				{
					onScrollEvent();
				}
			};
		}

		// Token: 0x0600B3EE RID: 46062 RVA: 0x0051DE04 File Offset: 0x0051C004
		private void OnDestroy()
		{
			this.UnregisterPrefabs();
		}

		// Token: 0x04008BDF RID: 35807
		public Action<RenderedRecordDataList<TData>> OnAllRecordReceived = null;

		// Token: 0x04008BE0 RID: 35808
		[SerializeField]
		protected RecordBase basePrefab;

		// Token: 0x04008BE1 RID: 35809
		[SerializeField]
		protected RecordTitle titlePrefab;

		// Token: 0x04008BE2 RID: 35810
		[SerializeField]
		protected RecordContent contentPrefab;

		// Token: 0x04008BE3 RID: 35811
		[SerializeField]
		protected RecordFoot footPrefab;

		// Token: 0x04008BE4 RID: 35812
		[SerializeField]
		protected LoopScrollRect scrollRect;

		// Token: 0x04008BE5 RID: 35813
		[SerializeField]
		protected NameButton fullBtnPrefab;

		// Token: 0x04008BE6 RID: 35814
		[SerializeField]
		protected NameButton leftBtnPrefab;

		// Token: 0x04008BE7 RID: 35815
		[SerializeField]
		protected NameButton rightBtnPrefab;

		// Token: 0x04008BE8 RID: 35816
		[SerializeField]
		protected int refillOrScrollDiff = 50;

		// Token: 0x04008BE9 RID: 35817
		[SerializeField]
		protected int refillScrollSpeed = 3000;

		// Token: 0x04008BEA RID: 35818
		[SerializeField]
		protected bool autoScrollToEnd = true;

		// Token: 0x04008BEB RID: 35819
		[NonSerialized]
		protected RenderedRecordDataList<TData> _dataList;

		// Token: 0x04008BEC RID: 35820
		private bool _needScrollToEnd = false;
	}
}
