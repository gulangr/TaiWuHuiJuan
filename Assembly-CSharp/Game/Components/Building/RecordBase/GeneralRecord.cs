using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Components.Character.LifeRecord;
using GameData.Domains.LifeRecord;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Components.Building.RecordBase
{
	// Token: 0x02000F67 RID: 3943
	public class GeneralRecord : Record<GeneralRenderedRecordData>
	{
		// Token: 0x17001477 RID: 5239
		// (get) Token: 0x0600B48E RID: 46222 RVA: 0x00522CB8 File Offset: 0x00520EB8
		public int StartDate
		{
			get
			{
				List<TransferableRecord> filtered = this._dataList.Filtered;
				int result;
				if (filtered == null || filtered.Count <= 0)
				{
					result = 0;
				}
				else
				{
					List<TransferableRecord> filtered2 = this._dataList.Filtered;
					result = filtered2[filtered2.Count - 1].Date;
				}
				return result;
			}
		}

		// Token: 0x17001478 RID: 5240
		// (get) Token: 0x0600B48F RID: 46223 RVA: 0x00522D00 File Offset: 0x00520F00
		public int EndDate
		{
			get
			{
				List<TransferableRecord> filtered = this._dataList.Filtered;
				return (filtered != null && filtered.Count > 0) ? this._dataList.Filtered[0].Date : 0;
			}
		}

		// Token: 0x17001479 RID: 5241
		// (get) Token: 0x0600B490 RID: 46224 RVA: 0x00522D3E File Offset: 0x00520F3E
		public int Count
		{
			get
			{
				return this._dataList.Filtered.Count;
			}
		}

		// Token: 0x1700147A RID: 5242
		// (get) Token: 0x0600B491 RID: 46225 RVA: 0x00522D50 File Offset: 0x00520F50
		// (set) Token: 0x0600B492 RID: 46226 RVA: 0x00522D60 File Offset: 0x00520F60
		public Func<TransferableRecord, bool> CanShow
		{
			get
			{
				return this._dataList.CanShow;
			}
			set
			{
				this._canShow = value;
				bool flag = this._dataList != null;
				if (flag)
				{
					this._dataList.CanShow = value;
				}
			}
		}

		// Token: 0x0600B493 RID: 46227 RVA: 0x00522D94 File Offset: 0x00520F94
		protected override void RegisterPrefabs()
		{
			object countLock = GeneralRecord.CountLock;
			lock (countLock)
			{
				bool flag2 = GeneralRecord._enabledCount++ == 0;
				if (flag2)
				{
					PoolManager.SetSrcObject("Game.Components.Building.RecordBase.TitleKey", this.titlePrefab.gameObject);
					PoolManager.SetSrcObject("Game.Components.Building.RecordBase.ContentKey", this.contentPrefab.gameObject);
					PoolManager.SetSrcObject("Game.Components.Building.RecordBase.FootKey", this.footPrefab.gameObject);
					PoolManager.SetSrcObject("Game.Components.Building.RecordBase.FullBtnKey", this.fullBtnPrefab.gameObject);
					PoolManager.SetSrcObject("Game.Components.Building.RecordBase.LeftBtnKey", this.leftBtnPrefab.gameObject);
					PoolManager.SetSrcObject("Game.Components.Building.RecordBase.RightBtnKey", this.rightBtnPrefab.gameObject);
				}
			}
		}

		// Token: 0x0600B494 RID: 46228 RVA: 0x00522E70 File Offset: 0x00521070
		protected override void UnregisterPrefabs()
		{
			object countLock = GeneralRecord.CountLock;
			lock (countLock)
			{
				bool flag2 = --GeneralRecord._enabledCount == 0;
				if (flag2)
				{
					PoolManager.RemoveData("Game.Components.Building.RecordBase.TitleKey");
					PoolManager.RemoveData("Game.Components.Building.RecordBase.ContentKey");
					PoolManager.RemoveData("Game.Components.Building.RecordBase.FootKey");
					PoolManager.RemoveData("Game.Components.Building.RecordBase.FullBtnKey");
					PoolManager.RemoveData("Game.Components.Building.RecordBase.LeftBtnKey");
					PoolManager.RemoveData("Game.Components.Building.RecordBase.RightBtnKey");
				}
			}
		}

		// Token: 0x0600B495 RID: 46229 RVA: 0x00522F04 File Offset: 0x00521104
		protected override void AwakeScroll()
		{
			this.scrollRect.InitLoop(this.basePrefab.gameObject, 0, new Action<Transform, int>(this.SetImpl), new Action<Transform>(GeneralRecord.ResetImpl));
		}

		// Token: 0x0600B496 RID: 46230 RVA: 0x00522F38 File Offset: 0x00521138
		public override GeneralRenderedRecordData RenderData(TransferableRecord record, TransferableRecordDataBase dataBase)
		{
			GeneralRenderedRecordData data = new GeneralRenderedRecordData
			{
				Parent = this
			};
			data.SetData(record, dataBase);
			return data;
		}

		// Token: 0x0600B497 RID: 46231 RVA: 0x00522F64 File Offset: 0x00521164
		public void Read(int offset, RawDataPool pool)
		{
			TransferableRecordDataBase data = new TransferableRecordDataBase();
			Serializer.Deserialize(pool, offset, ref data);
			this.Set(data, this._canShow);
		}

		// Token: 0x0600B498 RID: 46232 RVA: 0x00522F90 File Offset: 0x00521190
		public void Set(TransferableRecordDataBase data, Func<TransferableRecord, bool> canShow = null)
		{
			bool flag = this._dataList == null;
			if (flag)
			{
				this._dataList = new RenderedRecordDataList<GeneralRenderedRecordData>(this);
			}
			else
			{
				this._dataList.Clear();
			}
			this._dataList.CanShow = canShow;
			this._dataList.ReadData(data, false);
			Action<RenderedRecordDataList<GeneralRenderedRecordData>> onAllRecordReceived = this.OnAllRecordReceived;
			if (onAllRecordReceived != null)
			{
				onAllRecordReceived(this._dataList);
			}
			bool autoScrollToEnd = this.autoScrollToEnd;
			if (autoScrollToEnd)
			{
				this.RefreshScrollToEnd();
			}
		}

		// Token: 0x0600B499 RID: 46233 RVA: 0x00523008 File Offset: 0x00521208
		public void RefreshScrollToEnd()
		{
			this.scrollRect.totalCount = this._dataList.Filtered.Count;
			base.TryScrollToEnd();
		}

		// Token: 0x0600B49A RID: 46234 RVA: 0x00523030 File Offset: 0x00521230
		private void SetImpl(Transform item, int index)
		{
			GeneralRecordBase record = item.GetComponent<GeneralRecordBase>();
			bool flag = record == null;
			if (flag)
			{
				Debug.LogError("Invalid transform: GeneralRecordBase not found");
			}
			else
			{
				GeneralRenderedRecordData data = this._dataList[index];
				data.Parent = this;
				record.Set(data, this._dataList.Data);
			}
		}

		// Token: 0x0600B49B RID: 46235 RVA: 0x00523085 File Offset: 0x00521285
		private static void ResetImpl(Transform item)
		{
			GeneralRecordBase component = item.GetComponent<GeneralRecordBase>();
			if (component != null)
			{
				component.Reset();
			}
		}

		// Token: 0x04008CAE RID: 36014
		[TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		public Func<TransferableRecord, TransferableRecordDataBase, ValueTuple<string, string>> DataRender;

		// Token: 0x04008CAF RID: 36015
		private Func<TransferableRecord, bool> _canShow;

		// Token: 0x04008CB0 RID: 36016
		public const string TitleKey = "Game.Components.Building.RecordBase.TitleKey";

		// Token: 0x04008CB1 RID: 36017
		public const string ContentKey = "Game.Components.Building.RecordBase.ContentKey";

		// Token: 0x04008CB2 RID: 36018
		public const string FootKey = "Game.Components.Building.RecordBase.FootKey";

		// Token: 0x04008CB3 RID: 36019
		public const string FullBtnKey = "Game.Components.Building.RecordBase.FullBtnKey";

		// Token: 0x04008CB4 RID: 36020
		public const string LeftBtnKey = "Game.Components.Building.RecordBase.LeftBtnKey";

		// Token: 0x04008CB5 RID: 36021
		public const string RightBtnKey = "Game.Components.Building.RecordBase.RightBtnKey";

		// Token: 0x04008CB6 RID: 36022
		private static int _enabledCount = 0;

		// Token: 0x04008CB7 RID: 36023
		private static readonly object CountLock = new object();
	}
}
