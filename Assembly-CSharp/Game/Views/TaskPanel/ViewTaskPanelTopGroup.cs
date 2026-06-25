using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

namespace Game.Views.TaskPanel
{
	// Token: 0x02000757 RID: 1879
	public class ViewTaskPanelTopGroup : UIBase
	{
		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x06005AF7 RID: 23287 RVA: 0x002A3489 File Offset: 0x002A1689
		private TaskModel TaskModel
		{
			get
			{
				return SingletonObject.getInstance<TaskModel>();
			}
		}

		// Token: 0x06005AF8 RID: 23288 RVA: 0x002A3490 File Offset: 0x002A1690
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<TaskGroupData>("GroupData", out this._groupData);
			argsBox.Get<RectTransform>("Anchor", out this._anchor);
			this.Refresh();
		}

		// Token: 0x06005AF9 RID: 23289 RVA: 0x002A34C0 File Offset: 0x002A16C0
		private void Refresh()
		{
			bool flag = this.taskGroup == null;
			if (!flag)
			{
				this.taskGroup.Clear();
				this.taskGroup.gameObject.SetActive(true);
				List<int> topTaskInfoIdList = this.TaskModel.GetCurrentTopTaskIDList();
				this.taskGroup.SetData(this._groupData.dataList, topTaskInfoIdList, null, false);
				this.taskGroup.GetComponent<PositionFollower>().Target = this._anchor;
			}
		}

		// Token: 0x06005AFA RID: 23290 RVA: 0x002A353A File Offset: 0x002A173A
		public override void QuickHide()
		{
			this.taskGroup.Clear();
			base.QuickHide();
		}

		// Token: 0x04003EB9 RID: 16057
		[SerializeField]
		private TaskGroup taskGroup;

		// Token: 0x04003EBA RID: 16058
		private TaskGroupData _groupData;

		// Token: 0x04003EBB RID: 16059
		private RectTransform _anchor;
	}
}
