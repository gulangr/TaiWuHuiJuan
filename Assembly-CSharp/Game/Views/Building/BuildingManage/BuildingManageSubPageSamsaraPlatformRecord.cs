using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Components.Building.RecordBase;
using Game.Components.Character.LifeRecord;
using Game.Views.Encyclopedia.Utilities;
using GameData.Domains.Building;
using GameData.Domains.LifeRecord;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C01 RID: 3073
	public class BuildingManageSubPageSamsaraPlatformRecord : BuildingManageSubPage
	{
		// Token: 0x06009C2B RID: 39979 RVA: 0x004924EC File Offset: 0x004906EC
		private void Awake()
		{
			this.record.AddScrollEvent(new Action<int>(this.lifeRecordDateSelector.Set));
			this.lifeRecordDateSelector.DateHasData = new Func<int, bool>(this._date.Contains);
			this.searcher.onValueChanged.ResetListener(new Action<string>(this.<Awake>g__ReapplyFilter|4_0));
			this.searcher.onEndEdit.ResetListener(new Action<string>(this.<Awake>g__ReapplyFilter|4_0));
		}

		// Token: 0x06009C2C RID: 39980 RVA: 0x00492570 File Offset: 0x00490770
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			this.record.RefreshScrollToEnd();
			this._date.Clear();
			foreach (TransferableRecord item in this.record.DataList.Filtered)
			{
				this._date.Add(item.Date);
			}
			this.lifeRecordDateSelector.Set(this.record.StartDate, this.record.EndDate, this.record);
		}

		// Token: 0x06009C2D RID: 39981 RVA: 0x00492624 File Offset: 0x00490824
		public override void RequestData()
		{
			this.record.DataRender = new Func<TransferableRecord, TransferableRecordDataBase, ValueTuple<string, string>>(BuildingManageSubPageSamsaraPlatformRecord.DataRender);
			BuildingDomainMethod.AsyncCall.GetReversedSamsaraRecord(this.ParentView, new AsyncMethodCallbackDelegate(this.record.Read));
		}

		// Token: 0x06009C2E RID: 39982 RVA: 0x0049265B File Offset: 0x0049085B
		public void RequestData(ArgumentBox ab)
		{
			BuildingDomainMethod.AsyncCall.GetReversedSamsaraRecord(this.ParentView, delegate(int pool, RawDataPool offset)
			{
				this.record.Read(pool, offset);
				this.record.RefreshScrollToEnd();
			});
		}

		// Token: 0x06009C2F RID: 39983 RVA: 0x00492678 File Offset: 0x00490878
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		private static ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data)
		{
			SamsaraPlatformRecordItem config = SamsaraPlatformRecord.Instance[record.RecordType];
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

		// Token: 0x06009C31 RID: 39985 RVA: 0x00492728 File Offset: 0x00490928
		[CompilerGenerated]
		private void <Awake>g__ReapplyFilter|4_0(string value)
		{
			bool flag = string.IsNullOrEmpty(value);
			if (flag)
			{
				this.record.RefreshFilter(null);
			}
			else
			{
				BuildingManageSubPageSamsaraPlatformRecord.<>c__DisplayClass4_0 CS$<>8__locals1 = new BuildingManageSubPageSamsaraPlatformRecord.<>c__DisplayClass4_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.matcher = new OptimizedHtmlPatternMatcher(value);
				this.record.RefreshFilter(new Func<TransferableRecord, bool>(CS$<>8__locals1.<Awake>g__Filter|1));
			}
			this._date.Clear();
			foreach (TransferableRecord item in this.record.DataList.Filtered)
			{
				this._date.Add(item.Date);
			}
			this.lifeRecordDateSelector.Set(this.record.StartDate, this.record.EndDate, this.record);
		}

		// Token: 0x040078EC RID: 30956
		[SerializeField]
		private GeneralRecord record;

		// Token: 0x040078ED RID: 30957
		[SerializeField]
		private LifeRecordDateSelector lifeRecordDateSelector;

		// Token: 0x040078EE RID: 30958
		[SerializeField]
		private TMP_InputField searcher;

		// Token: 0x040078EF RID: 30959
		private readonly HashSet<int> _date = new HashSet<int>();
	}
}
