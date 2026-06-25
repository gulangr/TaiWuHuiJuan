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
	// Token: 0x020007C1 RID: 1985
	public class ViewSettlementTreasuryRecords : RecordBase
	{
		// Token: 0x060060E9 RID: 24809 RVA: 0x002C6DAC File Offset: 0x002C4FAC
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("SettlementId", out this._settlementId);
			base.OnInit(argsBox);
		}

		// Token: 0x060060EA RID: 24810 RVA: 0x002C6DC9 File Offset: 0x002C4FC9
		public override void RequestData()
		{
			OrganizationDomainMethod.AsyncCall.GetReversedSettlementTreasuryRecordCollection(this, this._settlementId, new AsyncMethodCallbackDelegate(base.Read));
		}

		// Token: 0x060060EB RID: 24811 RVA: 0x002C6DE8 File Offset: 0x002C4FE8
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		protected override ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			SettlementTreasuryRecordItem config = SettlementTreasuryRecord.Instance[record.RecordType];
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

		// Token: 0x04004333 RID: 17203
		private short _settlementId;
	}
}
