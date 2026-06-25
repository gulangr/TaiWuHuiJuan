using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.World;
using GameData.Domains.World.Task;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class TaskModel : ISingletonInit, IDisposable
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06001164 RID: 4452 RVA: 0x0006961E File Offset: 0x0006781E
	private TaskChain TaskChain
	{
		get
		{
			return TaskChain.Instance;
		}
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x00069625 File Offset: 0x00067825
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 1, 30, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 1, 47, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0006964C File Offset: 0x0006784C
	public void Init()
	{
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 1, 30, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 1, 47, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0006968C File Offset: 0x0006788C
	private void OnNotifyGameData(List<NotificationWrapper> notificationWrappers)
	{
		foreach (NotificationWrapper wrapper in notificationWrappers)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				this.HandleData(uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x00069710 File Offset: 0x00067910
	private void HandleData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		bool flag = uid.DomainId != 1;
		if (!flag)
		{
			ushort dataId = uid.DataId;
			ushort num = dataId;
			if (num != 30)
			{
				if (num == 47)
				{
					Serializer.Deserialize(dataPool, valueOffset, ref this.currentTopTaskInfoIdList);
					bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
					if (!inGuiding)
					{
						this.topTaskState = TopTaskState.ActiveTop;
						this.UpdateTaskData();
					}
				}
			}
			else
			{
				Serializer.Deserialize(dataPool, valueOffset, ref this.taskDisplayDataList);
				bool inGuiding2 = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (!inGuiding2)
				{
					this.UpdateTaskData();
				}
			}
		}
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x000697A4 File Offset: 0x000679A4
	public List<TaskDisplayData> GetData()
	{
		return this.taskDisplayDataList;
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x000697BC File Offset: 0x000679BC
	public Dictionary<int, TaskGroupData> GetTaskAddData()
	{
		return this.addAlltaskDisplayDataList;
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x000697D4 File Offset: 0x000679D4
	public Dictionary<int, TaskGroupData> GetCurrentTaskGroupData()
	{
		return this.currentGroupData;
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x000697EC File Offset: 0x000679EC
	public void TopEvent(int taskInfoId, int targetIndex = 0)
	{
		WorldDomainMethod.Call.SetTopTask(taskInfoId, targetIndex);
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x000697F8 File Offset: 0x000679F8
	public List<int> GetCurrentTopTaskIDList()
	{
		return this.currentTopTaskInfoIdList;
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x00069810 File Offset: 0x00067A10
	private void UpdateTaskData()
	{
		bool flag = !this.IsExistTask();
		if (!flag)
		{
			this.DataReset();
			List<TaskDisplayData> newTaskDisplayDataList = this.GetLastNotExistTask();
			for (int i = 0; i < newTaskDisplayDataList.Count; i++)
			{
				bool flag2 = this.AddAllTaskDisplayDataListIsExistKey(this.taskDisplayDataList[i]);
				if (!flag2)
				{
					TaskChainItem taskConditionItem = this.TaskChain.GetItem(this.taskDisplayDataList[i].InnerTaskData.TaskChainId);
					bool flag3 = taskConditionItem == null;
					if (flag3)
					{
						Debug.LogError(string.Format("ThisTaskIsNull!CurrentData：TaskChainID:{0}  TaskInfoID:{1}", this.taskDisplayDataList[i].InnerTaskData.TaskChainId, this.taskDisplayDataList[i].InnerTaskData.TaskInfoId));
						return;
					}
					bool flag4 = taskConditionItem.Name.IsNullOrEmpty();
					if (flag4)
					{
						this.addAlltaskDisplayDataList.Add(i + this.TaskChain.Count, new TaskGroupData
						{
							taskChainId = i + this.TaskChain.Count,
							dataList = new List<TaskDisplayData>
							{
								this.taskDisplayDataList[i]
							}
						});
					}
					else
					{
						this.addAlltaskDisplayDataList.Add(this.taskDisplayDataList[i].InnerTaskData.TaskChainId, new TaskGroupData
						{
							taskChainId = this.taskDisplayDataList[i].InnerTaskData.TaskChainId,
							dataList = new List<TaskDisplayData>
							{
								this.taskDisplayDataList[i]
							}
						});
					}
				}
			}
			this.RecordCurrentTask();
			bool flag5 = this.addAlltaskDisplayDataList.Count > 0;
			if (flag5)
			{
				Debug.Log("TaskLog：There are new tasks");
				GEvent.OnEvent(UiEvents.TaskAdd, null);
			}
			bool flag6 = this.lastAlltaskDisplayDataList.Count > 0;
			if (flag6)
			{
				GEvent.OnEvent(UiEvents.TaskRemove, null);
			}
			GEvent.OnEvent(UiEvents.TaskGroupDataUpdated, null);
			this.lastAlltaskDisplayDataList.Clear();
			foreach (TaskDisplayData item in this.taskDisplayDataList)
			{
				this.lastAlltaskDisplayDataList.Add(item.InnerTaskData.TaskInfoId);
			}
			bool flag7 = this.topTaskState > TopTaskState.ActiveTop;
			if (!flag7)
			{
				ArgumentBox data = new ArgumentBox();
				data.SetObject("TopTaskID", this.currentTaskInfoIdList);
				GEvent.OnEvent(UiEvents.TopTask, data);
				this.topTaskState = TopTaskState.NotActiveTop;
			}
		}
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x00069AE4 File Offset: 0x00067CE4
	private bool IsExistTask()
	{
		bool flag = this.taskDisplayDataList.Count > 0;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			this.lastAlltaskDisplayDataList.Clear();
			result = false;
		}
		return result;
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x00069B1C File Offset: 0x00067D1C
	private void DataReset()
	{
		bool flag = this.taskDisplayDataList.Count > 0;
		if (flag)
		{
			this.currentTaskInfoIdList = (from x in this.taskDisplayDataList
			select x.InnerTaskData.TaskInfoId).ToList<int>();
		}
		this.addAlltaskDisplayDataList.Clear();
		this.currentGroupData.Clear();
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x00069B8C File Offset: 0x00067D8C
	private List<TaskDisplayData> GetLastNotExistTask()
	{
		return (from item in this.taskDisplayDataList
		where !this.lastAlltaskDisplayDataList.Contains(item.InnerTaskData.TaskInfoId)
		select item).ToList<TaskDisplayData>();
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x00069BBC File Offset: 0x00067DBC
	private bool AddAllTaskDisplayDataListIsExistKey(TaskDisplayData taskDisplayData)
	{
		TaskGroupData value;
		bool flag = !this.addAlltaskDisplayDataList.TryGetValue(taskDisplayData.InnerTaskData.TaskChainId, out value);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			value.dataList.Add(taskDisplayData);
			result = true;
		}
		return result;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x00069C00 File Offset: 0x00067E00
	private void RecordCurrentTask()
	{
		for (int i = 0; i < this.taskDisplayDataList.Count; i++)
		{
			TaskGroupData value;
			bool flag = this.currentGroupData.TryGetValue(this.taskDisplayDataList[i].InnerTaskData.TaskChainId, out value);
			if (flag)
			{
				value.dataList.Add(this.taskDisplayDataList[i]);
			}
			else
			{
				bool flag2 = this.TaskChain.GetItem(this.taskDisplayDataList[i].InnerTaskData.TaskChainId).Name.IsNullOrEmpty();
				if (flag2)
				{
					this.currentGroupData.Add(i + this.taskChainCount, new TaskGroupData
					{
						taskChainId = i + this.taskChainCount,
						dataList = new List<TaskDisplayData>
						{
							this.taskDisplayDataList[i]
						}
					});
				}
				else
				{
					this.currentGroupData.Add(this.taskDisplayDataList[i].InnerTaskData.TaskChainId, new TaskGroupData
					{
						taskChainId = this.taskDisplayDataList[i].InnerTaskData.TaskChainId,
						dataList = new List<TaskDisplayData>
						{
							this.taskDisplayDataList[i]
						}
					});
				}
			}
		}
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x00069D64 File Offset: 0x00067F64
	public bool CheckRequiredTasksStatus(int requiredFinishedTask, int requiredUntriggeredTask)
	{
		bool flag = requiredFinishedTask >= 0;
		if (flag)
		{
			byte taskStatus;
			bool flag2 = !this.TryGetExtraTriggeredTaskStatus(requiredFinishedTask, out taskStatus) || taskStatus != 2;
			if (flag2)
			{
				return false;
			}
		}
		bool flag3 = requiredUntriggeredTask >= 0;
		if (flag3)
		{
			byte b;
			bool flag4 = this.TryGetExtraTriggeredTaskStatus(requiredUntriggeredTask, out b);
			if (flag4)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x00069DC4 File Offset: 0x00067FC4
	private bool TryGetExtraTriggeredTaskStatus(int taskInfoId, out byte taskStatus)
	{
		for (int i = this.taskDisplayDataList.Count - 1; i >= 0; i--)
		{
			TaskDisplayData task = this.taskDisplayDataList[i];
			bool flag = task.InnerTaskData.TaskInfoId == taskInfoId;
			if (flag)
			{
				taskStatus = task.InnerTaskData.TaskStatus;
				return true;
			}
		}
		taskStatus = 0;
		return false;
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x00069E30 File Offset: 0x00068030
	public bool IsTaskRecorded(int infoId)
	{
		bool flag = this.taskDisplayDataList == null || this.taskDisplayDataList.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (TaskDisplayData data in this.taskDisplayDataList)
			{
				bool flag2 = data.InnerTaskData.TaskInfoId == infoId;
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x00069EC0 File Offset: 0x000680C0
	public bool IsTaskFinished(int infoId)
	{
		bool flag = this.taskDisplayDataList == null || this.taskDisplayDataList.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (TaskDisplayData data in this.taskDisplayDataList)
			{
				bool flag2 = data.InnerTaskData.TaskInfoId == infoId && data.InnerTaskData.TaskStatus == 2;
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x00069F60 File Offset: 0x00068160
	public bool IsTaskInProgress(int infoId)
	{
		bool flag = this.taskDisplayDataList == null || this.taskDisplayDataList.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (TaskDisplayData data in this.taskDisplayDataList)
			{
				bool flag2;
				if (data.InnerTaskData.TaskInfoId == infoId)
				{
					TaskData innerTaskData = data.InnerTaskData;
					flag2 = innerTaskData.IsInProgress;
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0006A000 File Offset: 0x00068200
	public bool IsTaskInProgress(List<int> infoIds)
	{
		bool flag = this.taskDisplayDataList == null || this.taskDisplayDataList.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (TaskDisplayData data in this.taskDisplayDataList)
			{
				bool flag2 = infoIds.Contains(data.InnerTaskData.TaskInfoId);
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x04000F1D RID: 3869
	private int _gameDataListenerId = -1;

	// Token: 0x04000F1E RID: 3870
	private List<TaskDisplayData> taskDisplayDataList = new List<TaskDisplayData>();

	// Token: 0x04000F1F RID: 3871
	private List<int> lastAlltaskDisplayDataList = new List<int>();

	// Token: 0x04000F20 RID: 3872
	private Dictionary<int, TaskGroupData> addAlltaskDisplayDataList = new Dictionary<int, TaskGroupData>();

	// Token: 0x04000F21 RID: 3873
	private Dictionary<int, TaskGroupData> currentGroupData = new Dictionary<int, TaskGroupData>();

	// Token: 0x04000F22 RID: 3874
	private TopTaskState topTaskState = TopTaskState.NotActiveTop;

	// Token: 0x04000F23 RID: 3875
	private int taskChainCount = TaskChain.Instance.Count;

	// Token: 0x04000F24 RID: 3876
	private List<int> currentTaskInfoIdList;

	// Token: 0x04000F25 RID: 3877
	private List<int> currentTopTaskInfoIdList;
}
