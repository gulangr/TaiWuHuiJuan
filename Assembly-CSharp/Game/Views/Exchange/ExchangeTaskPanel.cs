using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Taiwu.ExchangeSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Exchange
{
	// Token: 0x02000A29 RID: 2601
	public class ExchangeTaskPanel : MonoBehaviour
	{
		// Token: 0x06007F76 RID: 32630 RVA: 0x003B5CA0 File Offset: 0x003B3EA0
		public void Init(ExchangeAdvantage summary)
		{
			bool flag = summary == null || !summary.Enabled;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
				this._taskIds = (summary.TaskId ?? Array.Empty<int>());
				this._taskDone = summary.TaskDict;
				foreach (ExchangeTaskLine line in this.lines)
				{
					Object.Destroy(line.gameObject);
				}
				this.lines = Enumerable.Range(0, this._taskIds.Length).Select(delegate(int i)
				{
					ExchangeTaskItem data = ExchangeTask.Instance[this._taskIds[i]];
					Dictionary<int, int> taskDone = this._taskDone;
					int count = (taskDone != null) ? taskDone.GetValueOrDefault(this._taskIds[i]) : 0;
					ExchangeTaskLine line2 = Object.Instantiate<ExchangeTaskLine>(this.linePrefab, this.content, false);
					line2.Set(data, count);
					return line2;
				}).ToArray<ExchangeTaskLine>();
				LayoutRebuilder.MarkLayoutForRebuild(this.content);
				this.empty.SetActive(this._taskIds.Length == 0);
			}
		}

		// Token: 0x06007F77 RID: 32631 RVA: 0x003B5D74 File Offset: 0x003B3F74
		public void Set(ExchangeAdvantage summary)
		{
			this._taskIds = (summary.TaskId ?? Array.Empty<int>());
			bool flag = this.lines.Length != this._taskIds.Length;
			if (flag)
			{
				this.Init(summary);
			}
			else
			{
				this._taskDone = summary.TaskDict;
				for (int i = 0; i < this._taskIds.Length; i++)
				{
					ExchangeTaskItem data = ExchangeTask.Instance[this._taskIds[i]];
					Dictionary<int, int> taskDone = this._taskDone;
					int count = (taskDone != null) ? taskDone.GetValueOrDefault(this._taskIds[i]) : 0;
					this.lines[i].Set(data, count);
				}
			}
		}

		// Token: 0x040061C8 RID: 25032
		[SerializeField]
		private RectTransform content;

		// Token: 0x040061C9 RID: 25033
		[SerializeField]
		private ExchangeTaskLine[] lines;

		// Token: 0x040061CA RID: 25034
		[SerializeField]
		private ExchangeTaskLine linePrefab;

		// Token: 0x040061CB RID: 25035
		[SerializeField]
		private GameObject empty;

		// Token: 0x040061CC RID: 25036
		private int[] _taskIds = Array.Empty<int>();

		// Token: 0x040061CD RID: 25037
		private Dictionary<int, int> _taskDone;
	}
}
