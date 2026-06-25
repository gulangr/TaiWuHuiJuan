using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.LifeRecord;
using GameData.Domains.Organization;
using UnityEngine;

namespace Game.Views.Record
{
	// Token: 0x020007C0 RID: 1984
	public class ViewSettlementPrisonRecords : RecordBase
	{
		// Token: 0x060060E5 RID: 24805 RVA: 0x002C6CCC File Offset: 0x002C4ECC
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("SettlementId", out this._settlementId);
			base.OnInit(argsBox);
		}

		// Token: 0x060060E6 RID: 24806 RVA: 0x002C6CE9 File Offset: 0x002C4EE9
		public override void RequestData()
		{
			OrganizationDomainMethod.AsyncCall.GetReversedSettlementPrisonRecordCollection(this, this._settlementId, new AsyncMethodCallbackDelegate(base.Read));
		}

		// Token: 0x060060E7 RID: 24807 RVA: 0x002C6D08 File Offset: 0x002C4F08
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		protected override ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			SettlementPrisonRecordItem config = SettlementPrisonRecord.Instance[record.RecordType];
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

		// Token: 0x04004332 RID: 17202
		private short _settlementId;
	}
}
