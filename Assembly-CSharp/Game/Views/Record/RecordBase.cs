using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Building.RecordBase;
using Game.Components.Character.LifeRecord;
using Game.Views.Encyclopedia.Utilities;
using GameData.Domains.LifeRecord;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Record
{
	// Token: 0x020007BF RID: 1983
	public abstract class RecordBase : UIBase
	{
		// Token: 0x060060DC RID: 24796 RVA: 0x002C69EC File Offset: 0x002C4BEC
		public override void OnInit(ArgumentBox argsBox)
		{
			this.record.DataRender = new Func<TransferableRecord, TransferableRecordDataBase, ValueTuple<string, string>>(this.DataRender);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x002C6A3C File Offset: 0x002C4C3C
		protected virtual void Awake()
		{
			this.record.AddScrollEvent(new Action<int>(this.lifeRecordDateSelector.Set));
			this.lifeRecordDateSelector.DateHasData = new Func<int, bool>(this._date.Contains);
			this.AwakeSearch();
		}

		// Token: 0x060060DE RID: 24798 RVA: 0x002C6A8C File Offset: 0x002C4C8C
		public void Refresh()
		{
			this.ReapplyFilter(this.searcher.text);
			this.record.RefreshScrollToEnd();
			this._date.Clear();
			foreach (TransferableRecord item in this.record.DataList.Filtered)
			{
				this._date.Add(item.Date);
			}
			this.lifeRecordDateSelector.Set(this.record.StartDate, this.record.EndDate, this.record);
		}

		// Token: 0x060060DF RID: 24799 RVA: 0x002C6B4C File Offset: 0x002C4D4C
		public void Read(int offset, RawDataPool pool)
		{
			this.record.Read(offset, pool);
			this.Refresh();
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060060E0 RID: 24800
		[return: TupleElementNames(new string[]
		{
			"main",
			"sub"
		})]
		protected abstract ValueTuple<string, string> DataRender(TransferableRecord record, TransferableRecordDataBase data);

		// Token: 0x060060E1 RID: 24801
		public abstract void RequestData();

		// Token: 0x060060E2 RID: 24802 RVA: 0x002C6B70 File Offset: 0x002C4D70
		protected void AwakeSearch()
		{
			this.searcher.onValueChanged.ResetListener(new Action<string>(this.ReapplyFilter));
			this.searcher.onEndEdit.ResetListener(new Action<string>(this.ReapplyFilter));
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x002C6BB0 File Offset: 0x002C4DB0
		protected virtual void ReapplyFilter(string value)
		{
			bool flag = string.IsNullOrEmpty(value);
			if (flag)
			{
				this.record.RefreshFilter(null);
			}
			else
			{
				RecordBase.<>c__DisplayClass14_0 CS$<>8__locals1 = new RecordBase.<>c__DisplayClass14_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.matcher = new OptimizedHtmlPatternMatcher(value);
				this.record.RefreshFilter(new Func<TransferableRecord, bool>(CS$<>8__locals1.<ReapplyFilter>g__Filter|0));
			}
			this._date.Clear();
			foreach (TransferableRecord item in this.record.DataList.Filtered)
			{
				this._date.Add(item.Date);
			}
			this.lifeRecordDateSelector.Set(this.record.StartDate, this.record.EndDate, this.record);
			this.noContent.SetActive(this.record.Count == 0);
		}

		// Token: 0x0400432B RID: 17195
		[SerializeField]
		internal GameObject noContent;

		// Token: 0x0400432C RID: 17196
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400432D RID: 17197
		[SerializeField]
		private CButton resetSearch;

		// Token: 0x0400432E RID: 17198
		[SerializeField]
		protected GeneralRecord record;

		// Token: 0x0400432F RID: 17199
		[SerializeField]
		protected LifeRecordDateSelector lifeRecordDateSelector;

		// Token: 0x04004330 RID: 17200
		[SerializeField]
		protected TMP_InputField searcher;

		// Token: 0x04004331 RID: 17201
		private readonly HashSet<int> _date = new HashSet<int>();
	}
}
