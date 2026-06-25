using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using Game.Components.Building.RecordBase;
using GameData.Domains.Building;
using GameData.Domains.LifeRecord;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C1A RID: 3098
	public class ProductionRecord : MonoBehaviour, IProductionComponent
	{
		// Token: 0x06009D79 RID: 40313 RVA: 0x0049C0E2 File Offset: 0x0049A2E2
		public void Setup(IProductionHandler handler)
		{
			this._handler = handler;
			this.record.CanShow = ((TransferableRecord rec) => rec.RecordType < 0 || ShopEvent.Instance[rec.RecordType].ShopEventType == 0);
		}

		// Token: 0x06009D7A RID: 40314 RVA: 0x0049C118 File Offset: 0x0049A318
		public void Refresh()
		{
			this.record.DataRender = new Func<TransferableRecord, TransferableRecordDataBase, ValueTuple<string, string>>(ProductionRecord.DataRender);
			BuildingDomainMethod.AsyncCall.GetReversedBlockShopEvent(this._handler.Async, this._handler.Key, delegate(int x, RawDataPool y)
			{
				this.record.Read(x, y);
				this.record.RefreshScrollToEnd();
			});
		}

		// Token: 0x06009D7B RID: 40315 RVA: 0x0049C168 File Offset: 0x0049A368
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		private static ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			ShopEventItem config = ShopEvent.Instance[record.RecordType];
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

		// Token: 0x040079F3 RID: 31219
		private IProductionHandler _handler;

		// Token: 0x040079F4 RID: 31220
		[SerializeField]
		private GeneralRecord record;
	}
}
