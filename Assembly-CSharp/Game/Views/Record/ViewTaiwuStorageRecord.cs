using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Utilities;
using GameData.Domains.LifeRecord;
using GameData.Domains.Organization.TaiwuVillageStoragesRecord;
using GameData.Domains.Taiwu;
using UnityEngine;

namespace Game.Views.Record
{
	// Token: 0x020007C2 RID: 1986
	public class ViewTaiwuStorageRecord : RecordBase
	{
		// Token: 0x060060ED RID: 24813 RVA: 0x002C6E8C File Offset: 0x002C508C
		public static sbyte GetStorageType(int togKey)
		{
			if (!true)
			{
			}
			sbyte result;
			switch (togKey)
			{
			case 0:
				result = 1;
				break;
			case 1:
				result = 2;
				break;
			case 2:
				result = 3;
				break;
			case 3:
				result = 4;
				break;
			default:
				throw new ArgumentOutOfRangeException("togKey", togKey, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060060EE RID: 24814 RVA: 0x002C6EDC File Offset: 0x002C50DC
		public static int GetStorageKey(sbyte togKey)
		{
			if (!true)
			{
			}
			int result;
			switch (togKey)
			{
			case 1:
				result = 0;
				break;
			case 2:
				result = 1;
				break;
			case 3:
				result = 2;
				break;
			case 4:
				result = 3;
				break;
			default:
				throw new ArgumentOutOfRangeException("togKey", togKey, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060060EF RID: 24815 RVA: 0x002C6F30 File Offset: 0x002C5130
		protected override void Awake()
		{
			base.Awake();
			this.currPage.Init(-1);
			this.currPage.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this._storageType = ViewTaiwuStorageRecord.GetStorageType(newTog);
				base.Refresh();
			};
			this.lifeRecordDateSelector.DateHasData = new Func<int, bool>(ViewTaiwuStorageRecord._date.Contains);
		}

		// Token: 0x060060F0 RID: 24816 RVA: 0x002C6F88 File Offset: 0x002C5188
		public override void OnInit(ArgumentBox argsBox)
		{
			TaiwuVillageStorageType storageType;
			this._storageType = (sbyte)((argsBox != null && argsBox.Get<TaiwuVillageStorageType>("StorageType", out storageType)) ? storageType : TaiwuVillageStorageType.Treasury);
			this.currPage.Set(ViewTaiwuStorageRecord.GetStorageKey(this._storageType), false);
			base.OnInit(argsBox);
		}

		// Token: 0x060060F1 RID: 24817 RVA: 0x002C6FD2 File Offset: 0x002C51D2
		public override void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetReversedTaiwuVillageStoragesRecordCollection(this, new AsyncMethodCallbackDelegate(base.Read));
		}

		// Token: 0x060060F2 RID: 24818 RVA: 0x002C6FE8 File Offset: 0x002C51E8
		protected override void ReapplyFilter(string value)
		{
			bool flag = string.IsNullOrEmpty(value);
			if (flag)
			{
				this.record.RefreshFilter(new Func<TransferableRecord, bool>(this.FilterBase));
			}
			else
			{
				ViewTaiwuStorageRecord.<>c__DisplayClass11_0 CS$<>8__locals1 = new ViewTaiwuStorageRecord.<>c__DisplayClass11_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.matcher = new OptimizedHtmlPatternMatcher(value);
				this.record.RefreshFilter(new Func<TransferableRecord, bool>(CS$<>8__locals1.<ReapplyFilter>g__Filter|0));
			}
			ViewTaiwuStorageRecord._date.Clear();
			ViewTaiwuStorageRecord._date.Clear();
			foreach (TransferableRecord item in this.record.DataList.Filtered)
			{
				ViewTaiwuStorageRecord._date.Add(item.Date);
			}
			this.lifeRecordDateSelector.Set(this.record.StartDate, this.record.EndDate, this.record);
			this.noContent.SetActive(this.record.Count == 0);
		}

		// Token: 0x060060F3 RID: 24819 RVA: 0x002C7104 File Offset: 0x002C5304
		private bool FilterBase(TransferableRecord rawRecord)
		{
			if (rawRecord.RecordType >= 0)
			{
				TaiwuVillageStoragesRecordItem cfg = TaiwuVillageStoragesRecord.Instance[rawRecord.RecordType];
				if (cfg != null)
				{
					List<ValueTuple<sbyte, int>> arguments = rawRecord.Arguments;
					if (arguments != null && arguments.Count > 0)
					{
						List<ValueTuple<sbyte, int>> arguments2 = rawRecord.Arguments;
						return arguments2[arguments2.Count - 1].Item2 == (int)this._storageType;
					}
				}
			}
			return true;
		}

		// Token: 0x060060F4 RID: 24820 RVA: 0x002C7164 File Offset: 0x002C5364
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		protected override ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			TaiwuVillageStoragesRecordItem config = TaiwuVillageStoragesRecord.Instance[record.RecordType];
			bool flag = config != null;
			ValueTuple<string, string> result;
			if (flag)
			{
				result = new ValueTuple<string, string>(string.Format(config.Desc, (from x in record.Arguments
				select GameMessageUtils.ReadArguments(x.Item1, x.Item2, data)).ToArray<object>()).ColorReplace(), "");
			}
			else
			{
				Debug.LogWarning(string.Format("Invalid record type: {0}", record.RecordType));
				result = new ValueTuple<string, string>("", "");
			}
			return result;
		}

		// Token: 0x04004334 RID: 17204
		[SerializeField]
		private CToggleGroup currPage;

		// Token: 0x04004335 RID: 17205
		private static HashSet<int> _date = new HashSet<int>();

		// Token: 0x04004336 RID: 17206
		private sbyte _storageType;

		// Token: 0x04004337 RID: 17207
		private TaiwuVillageStoragesRecordCollection _taiwuVillageStoragesRecordCollection = new TaiwuVillageStoragesRecordCollection();

		// Token: 0x04004338 RID: 17208
		private readonly List<TaiwuVillageStoragesRecordRenderInfo> _taiwuVillageStoragesRecordRenderInfos = new List<TaiwuVillageStoragesRecordRenderInfo>();

		// Token: 0x02001D04 RID: 7428
		public static class TogKey
		{
			// Token: 0x0400C4BD RID: 50365
			public const int Warehouse = 0;

			// Token: 0x0400C4BE RID: 50366
			public const int Treasury = 1;

			// Token: 0x0400C4BF RID: 50367
			public const int Stock = 2;

			// Token: 0x0400C4C0 RID: 50368
			public const int Trough = 3;
		}
	}
}
