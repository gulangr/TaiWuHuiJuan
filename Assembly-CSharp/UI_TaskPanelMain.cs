using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using UnityEngine;

// Token: 0x0200030E RID: 782
[RequireComponent(typeof(UIAnim))]
public class UI_TaskPanelMain : UIBase
{
	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x06002E18 RID: 11800 RVA: 0x0016CED2 File Offset: 0x0016B0D2
	private TaskModel TaskModel
	{
		get
		{
			return SingletonObject.getInstance<TaskModel>();
		}
	}

	// Token: 0x1700050E RID: 1294
	// (get) Token: 0x06002E19 RID: 11801 RVA: 0x0016CED9 File Offset: 0x0016B0D9
	private BubbleBox BubbleBox
	{
		get
		{
			return base.CGet<BubbleBox>("TaskBubbles");
		}
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x0016CEE6 File Offset: 0x0016B0E6
	public override void OnInit(ArgumentBox argsBox)
	{
		this._uiAnim = base.GetComponent<UIAnim>();
		this._uiAnim.Init(Vector3.zero, Vector3.zero.SetY(500f));
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x0016CF15 File Offset: 0x0016B115
	private void Awake()
	{
		this.GetTaskDataForTheFirstTime();
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x0016CF20 File Offset: 0x0016B120
	private void OnEnable()
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!inGuiding)
		{
			bool flag = this.TaskModel.GetCurrentTaskGroupData().Count > 0;
			if (flag)
			{
				this.SetTaskPanelAndRememberBubblesData(this.TaskModel.GetCurrentTaskGroupData().First<KeyValuePair<int, TaskGroupData>>().Value);
			}
			GEvent.Add(UiEvents.TaskAdd, new GEvent.Callback(this.TaskAdd));
			GEvent.Add(UiEvents.TaskRemove, new GEvent.Callback(this.TaskRemove));
			GEvent.Add(UiEvents.ClearTask, new GEvent.Callback(this.ClearTask));
			GEvent.Add(UiEvents.TopTask, new GEvent.Callback(this.TopTaskCallBack));
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		}
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x0016D020 File Offset: 0x0016B220
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.TaskAdd, new GEvent.Callback(this.TaskAdd));
		GEvent.Remove(UiEvents.TaskRemove, new GEvent.Callback(this.TaskRemove));
		GEvent.Remove(UiEvents.ClearTask, new GEvent.Callback(this.ClearTask));
		GEvent.Remove(UiEvents.TopTask, new GEvent.Callback(this.TopTaskCallBack));
		GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
		GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
	}

	// Token: 0x06002E1E RID: 11806 RVA: 0x0016D0D8 File Offset: 0x0016B2D8
	private void TopTaskCallBack(ArgumentBox box)
	{
		this._taskGroupData = this.TaskModel.GetCurrentTaskGroupData().First<KeyValuePair<int, TaskGroupData>>().Value;
		this.SetTaskPanelAndRememberBubblesData(this._taskGroupData);
		this.BubbleBox.DisPlayOnTaskPanelClose();
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x0016D11D File Offset: 0x0016B31D
	private void TaskAdd(ArgumentBox box)
	{
		this.GetAddTaskAndCheckIsOpenTaskPanel();
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x0016D128 File Offset: 0x0016B328
	private void TaskRemove(ArgumentBox argbox)
	{
		this.SetTaskPanelAndRememberBubblesData(this.TaskModel.GetCurrentTaskGroupData().First<KeyValuePair<int, TaskGroupData>>().Value);
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x0016D158 File Offset: 0x0016B358
	private void GetTaskDataForTheFirstTime()
	{
		bool flag = this.TaskDataCheck();
		if (!flag)
		{
			TaskGroupData taskGroup = this.TaskModel.GetCurrentTaskGroupData().First<KeyValuePair<int, TaskGroupData>>().Value;
			this.SetTaskPanelAndRememberBubblesData(taskGroup);
		}
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x0016D194 File Offset: 0x0016B394
	public void StopRotationTask()
	{
		bool flag = this._rotationTaskCoroutine == null;
		if (!flag)
		{
			base.StopCoroutine(this._rotationTaskCoroutine);
			this._rotationTaskCoroutine = null;
			while (this._rotationTaskQueue.Count > 0)
			{
				this._rotationTaskQueue.Dequeue();
			}
			TaskGroupData taskGroup = this.TaskModel.GetCurrentTaskGroupData().First<KeyValuePair<int, TaskGroupData>>().Value;
			this.SetTaskPanelAndRememberBubblesData(taskGroup);
		}
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x0016D208 File Offset: 0x0016B408
	private IEnumerator RotationTask()
	{
		while (this._rotationTaskQueue.Count > 0)
		{
			TaskGroupData data = this._rotationTaskQueue.Dequeue();
			this.SetTaskPanelAndRememberBubblesData(data);
			yield return UI_TaskPanelMain._waitForSeconds3;
			data = default(TaskGroupData);
		}
		TaskGroupData taskGroup = this.TaskModel.GetCurrentTaskGroupData().First<KeyValuePair<int, TaskGroupData>>().Value;
		this.SetTaskPanelAndRememberBubblesData(taskGroup);
		yield break;
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x0016D217 File Offset: 0x0016B417
	private void ClearTask(ArgumentBox box)
	{
		this.BubbleBox.Clear();
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x0016D228 File Offset: 0x0016B428
	private bool TaskDataCheck()
	{
		return SingletonObject.getInstance<TutorialChapterModel>().InGuiding || this.TaskModel.GetData().Count <= 0 || this.TaskModel.GetCurrentTaskGroupData().Count <= 0;
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x0016D27C File Offset: 0x0016B47C
	private void SetTaskPanelAndRememberBubblesData(TaskGroupData data)
	{
		this._taskGroupData = data;
		TaskInfoItem info = TaskInfo.Instance.GetItem(data.dataList[0].InnerTaskData.TaskInfoId);
		this.BubbleBox.SetTextAndNotShow(info.TaskBubblesContent, (float)info.ShowTime / 60f);
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x0016D2D4 File Offset: 0x0016B4D4
	private void GetAddTaskAndCheckIsOpenTaskPanel()
	{
		foreach (int item in this.TaskModel.GetTaskAddData().Keys)
		{
			this._rotationTaskQueue.Enqueue(this.TaskModel.GetTaskAddData()[item]);
		}
		bool flag = this._rotationTaskQueue.Count == 0;
		if (!flag)
		{
			this._rotationTaskCoroutine = base.StartCoroutine(this.RotationTask());
		}
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x0016D374 File Offset: 0x0016B574
	private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
	{
		this._uiAnim.PlayHideAnimation(null, true);
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x0016D384 File Offset: 0x0016B584
	private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
	{
		this._uiAnim.PlayShowAnimation(null, true);
	}

	// Token: 0x0400216B RID: 8555
	private static readonly WaitForSeconds _waitForSeconds3 = new WaitForSeconds(3f);

	// Token: 0x0400216C RID: 8556
	private readonly Queue<TaskGroupData> _rotationTaskQueue = new Queue<TaskGroupData>();

	// Token: 0x0400216D RID: 8557
	private TaskGroupData _taskGroupData;

	// Token: 0x0400216E RID: 8558
	private Coroutine _rotationTaskCoroutine;

	// Token: 0x0400216F RID: 8559
	private UIAnim _uiAnim;

	// Token: 0x04002170 RID: 8560
	private Vector2 _initialPosition;
}
