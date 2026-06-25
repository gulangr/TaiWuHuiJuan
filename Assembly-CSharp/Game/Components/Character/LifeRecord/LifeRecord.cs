using System;
using System.Runtime.CompilerServices;
using Config;
using GameData.Domains.LifeRecord;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F48 RID: 3912
	public class LifeRecord : Record<RenderedRecordData>
	{
		// Token: 0x0600B388 RID: 45960 RVA: 0x0051B71C File Offset: 0x0051991C
		protected override void RegisterPrefabs()
		{
			object countLock = LifeRecord.CountLock;
			lock (countLock)
			{
				bool flag2 = LifeRecord._enabledCount++ == 0;
				if (flag2)
				{
					PoolManager.SetSrcObject(LifeRecord.LifeRecordTitleKey, this.titlePrefab.gameObject);
					PoolManager.SetSrcObject(LifeRecord.LifeRecordContentKey, this.contentPrefab.gameObject);
					PoolManager.SetSrcObject(LifeRecord.LifeRecordFootKey, this.footPrefab.gameObject);
					PoolManager.SetSrcObject(LifeRecord.LifeRecordNameFullBtnKey, this.fullBtnPrefab.gameObject);
					PoolManager.SetSrcObject(LifeRecord.LifeRecordNameLeftBtnKey, this.leftBtnPrefab.gameObject);
					PoolManager.SetSrcObject(LifeRecord.LifeRecordNameRightBtnKey, this.rightBtnPrefab.gameObject);
					LifeRecord.InvisibleContent = new string('·', 50) + LanguageKey.LK_LifeRecord_NeedMoreFavor.Tr();
					LifeRecord.InvisibleContentDesc = "";
				}
			}
		}

		// Token: 0x0600B389 RID: 45961 RVA: 0x0051B820 File Offset: 0x00519A20
		protected override void UnregisterPrefabs()
		{
			object countLock = LifeRecord.CountLock;
			lock (countLock)
			{
				bool flag2 = --LifeRecord._enabledCount == 0;
				if (flag2)
				{
					PoolManager.RemoveData(LifeRecord.LifeRecordTitleKey);
					PoolManager.RemoveData(LifeRecord.LifeRecordContentKey);
					PoolManager.RemoveData(LifeRecord.LifeRecordFootKey);
					PoolManager.RemoveData(LifeRecord.LifeRecordNameFullBtnKey);
					PoolManager.RemoveData(LifeRecord.LifeRecordNameLeftBtnKey);
					PoolManager.RemoveData(LifeRecord.LifeRecordNameRightBtnKey);
				}
			}
		}

		// Token: 0x0600B38A RID: 45962 RVA: 0x0051B8B4 File Offset: 0x00519AB4
		protected override void AwakeScroll()
		{
			this.scrollRect.InitLoop(this.basePrefab.gameObject, 0, new Action<Transform, int>(this.SetImpl), new Action<Transform>(LifeRecord.ResetImpl));
		}

		// Token: 0x0600B38B RID: 45963 RVA: 0x0051B8E8 File Offset: 0x00519AE8
		public void Set(IAsyncMethodRequestHandler receiver, int charId, bool isDreamBack = false)
		{
			this._isDreamBack = isDreamBack;
			lock (this)
			{
				bool processing = this._processing;
				if (processing)
				{
					this._newProcessingId = charId;
					return;
				}
				this._processing = true;
			}
			if (this._dataList == null)
			{
				this._dataList = new RenderedRecordDataList<RenderedRecordData>(this);
			}
			bool flag2 = this._dataList.SetId(charId, false);
			if (flag2)
			{
				this.scrollRect.totalCount = 0;
				this.scrollRect.RefillCells(0, false);
			}
			TransferableRecordDataBase data = this._dataList.Data;
			this.AsyncSet(receiver, charId, (data != null) ? data.LifeRecordCount : 0);
		}

		// Token: 0x0600B38C RID: 45964 RVA: 0x0051B9A8 File Offset: 0x00519BA8
		private void AsyncSet(IAsyncMethodRequestHandler receiver, int charId, int startCount)
		{
			bool isDreamBack = this._isDreamBack;
			LifeRecordDomainMethod.AsyncCall.GetReversedRecord(receiver, charId, startCount, LifeRecord.PageSize, this._isDreamBack, delegate(int offset, RawDataPool dataPool)
			{
				TransferableLifeRecordData data = new TransferableLifeRecordData();
				Serializer.Deserialize(dataPool, offset, ref data);
				LifeRecord <>4__this = this;
				lock (<>4__this)
				{
					bool flag2 = this._newProcessingId != -1;
					if (flag2)
					{
						this._dataList.Clear();
						this.AsyncSet(receiver, this._newProcessingId, 0);
						this._newProcessingId = -1;
						return;
					}
				}
				bool needFetchExtraData = data.LifeRecordCount == LifeRecord.PageSize;
				this._dataList.ReadData(data, true);
				bool flag3 = needFetchExtraData;
				if (flag3)
				{
					this.AsyncSet(receiver, charId, this._dataList.Data.LifeRecordCount);
					this.scrollRect.totalCount = this._dataList.Count;
					this.TryScrollToEnd();
				}
				else
				{
					LifeRecord <>4__this2 = this;
					lock (<>4__this2)
					{
						bool flag5 = this._newProcessingId != -1;
						if (flag5)
						{
							this._dataList.Clear();
							this.AsyncSet(receiver, charId, 0);
							this._newProcessingId = -1;
							return;
						}
						bool flag6 = isDreamBack != this._isDreamBack;
						if (flag6)
						{
							this._dataList.Clear();
							this.AsyncSet(receiver, charId, 0);
							return;
						}
						this._processing = false;
					}
					this.scrollRect.totalCount = this._dataList.Count;
					Action<RenderedRecordDataList<RenderedRecordData>> onAllRecordReceived = this.OnAllRecordReceived;
					if (onAllRecordReceived != null)
					{
						onAllRecordReceived(this._dataList);
					}
					bool autoScrollToEnd = this.autoScrollToEnd;
					if (autoScrollToEnd)
					{
						this.TryScrollToEnd();
					}
				}
			});
		}

		// Token: 0x0600B38D RID: 45965 RVA: 0x0051BA07 File Offset: 0x00519C07
		public void Clear()
		{
			RenderedRecordDataList<RenderedRecordData> dataList = this._dataList;
			if (dataList != null)
			{
				dataList.SetId(-1, false);
			}
			this._processing = false;
		}

		// Token: 0x0600B38E RID: 45966 RVA: 0x0051BA28 File Offset: 0x00519C28
		private void SetImpl(Transform item, int index)
		{
			LifeRecordBase record = item.GetComponent<LifeRecordBase>();
			bool flag = record == null;
			if (flag)
			{
				Debug.LogError("Invalid transform: LifeRecordBase not found");
			}
			else
			{
				record.Set(this._dataList[index], this._dataList.Data);
			}
		}

		// Token: 0x0600B38F RID: 45967 RVA: 0x0051BA74 File Offset: 0x00519C74
		private static void ResetImpl(Transform item)
		{
			LifeRecordBase component = item.GetComponent<LifeRecordBase>();
			if (component != null)
			{
				component.Reset();
			}
		}

		// Token: 0x17001457 RID: 5207
		// (get) Token: 0x0600B390 RID: 45968 RVA: 0x0051BA88 File Offset: 0x00519C88
		// (set) Token: 0x0600B391 RID: 45969 RVA: 0x0051BA90 File Offset: 0x00519C90
		public LifeRecord.DisplayType LifeRecordDisplayType
		{
			get
			{
				return this._lifeRecordDisplayType;
			}
			set
			{
				bool flag = value == this._lifeRecordDisplayType;
				if (!flag)
				{
					this._lifeRecordDisplayType = value;
					bool flag2 = this._dataList == null;
					if (flag2)
					{
						this.scrollRect.totalCount = 0;
						this.scrollRect.RefillCells(0, false);
					}
					else
					{
						base.RefreshFilter(new Func<TransferableRecord, bool>(this.<set_LifeRecordDisplayType>g__CanShow|26_0));
					}
				}
			}
		}

		// Token: 0x0600B394 RID: 45972 RVA: 0x0051BB74 File Offset: 0x00519D74
		[CompilerGenerated]
		private bool <set_LifeRecordDisplayType>g__CanShow|26_0(TransferableRecord x)
		{
			bool result;
			if (x.RecordType >= 0)
			{
				LifeRecordItem item = LifeRecord.Instance[x.RecordType];
				result = (item == null || (this._lifeRecordDisplayType & (LifeRecord.DisplayType)(1 << (int)(item.DisplayType + 1))) > (LifeRecord.DisplayType)0);
			}
			else
			{
				result = (x.RecordType != -1 || (this._lifeRecordDisplayType & LifeRecord.DisplayType.Great) > (LifeRecord.DisplayType)0);
			}
			return result;
		}

		// Token: 0x04008B86 RID: 35718
		public static string LifeRecordTitleKey = "Game.Components.Character.LifeRecordTitleKey";

		// Token: 0x04008B87 RID: 35719
		public static string LifeRecordContentKey = "Game.Components.Character.LifeRecordContentKey";

		// Token: 0x04008B88 RID: 35720
		public static string LifeRecordFootKey = "Game.Components.Character.LifeRecordFootKey";

		// Token: 0x04008B89 RID: 35721
		public static string LifeRecordNameFullBtnKey = "Game.Components.Character.LifeRecordNameFullBtnKey";

		// Token: 0x04008B8A RID: 35722
		public static string LifeRecordNameLeftBtnKey = "Game.Components.Character.LifeRecordNameLeftBtnKey";

		// Token: 0x04008B8B RID: 35723
		public static string LifeRecordNameRightBtnKey = "Game.Components.Character.LifeRecordNameRightBtnKey";

		// Token: 0x04008B8C RID: 35724
		private static int _enabledCount = 0;

		// Token: 0x04008B8D RID: 35725
		private static readonly object CountLock = new object();

		// Token: 0x04008B8E RID: 35726
		internal static string InvisibleContent;

		// Token: 0x04008B8F RID: 35727
		internal static string InvisibleContentDesc;

		// Token: 0x04008B90 RID: 35728
		private static int PageSize = 10000;

		// Token: 0x04008B91 RID: 35729
		private bool _processing;

		// Token: 0x04008B92 RID: 35730
		private int _newProcessingId = -1;

		// Token: 0x04008B93 RID: 35731
		private bool _isDreamBack;

		// Token: 0x04008B94 RID: 35732
		private LifeRecord.DisplayType _lifeRecordDisplayType = LifeRecord.DisplayType.All;

		// Token: 0x02002583 RID: 9603
		[Flags]
		public enum DisplayType
		{
			// Token: 0x0400E824 RID: 59428
			None = 1,
			// Token: 0x0400E825 RID: 59429
			Great = 2,
			// Token: 0x0400E826 RID: 59430
			Normal = 4,
			// Token: 0x0400E827 RID: 59431
			Relation = 8,
			// Token: 0x0400E828 RID: 59432
			Study = 16,
			// Token: 0x0400E829 RID: 59433
			Produce = 32,
			// Token: 0x0400E82A RID: 59434
			Combat = 64,
			// Token: 0x0400E82B RID: 59435
			Negative = 128,
			// Token: 0x0400E82C RID: 59436
			Crime = 256,
			// Token: 0x0400E82D RID: 59437
			All = 511
		}
	}
}
