using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Adventure;
using GameData.Domains.World.Task;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.TaskPanel
{
	// Token: 0x02000758 RID: 1880
	public class ViewTaskPopUpPanel : UIBase
	{
		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x06005AFC RID: 23292 RVA: 0x002A3559 File Offset: 0x002A1759
		private TaskModel TaskModel
		{
			get
			{
				return SingletonObject.getInstance<TaskModel>();
			}
		}

		// Token: 0x06005AFD RID: 23293 RVA: 0x002A3560 File Offset: 0x002A1760
		public override void OnInit(ArgumentBox argsBox)
		{
			this.verticalScrollbar.value = 0f;
			this.taskGroupToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.taskGroupToggleGroup, 0, null);
			this._taskInfoId = -1;
			argsBox.Get("TaskInfoId", out this._taskInfoId);
			this.RefreshTaskGroupToggleGroup();
			this.GetTaskDataAndShow();
			this.content.GetComponent<ContentSizeFitter>().enabled = true;
			this.content.GetComponent<VerticalLayoutGroup>().enabled = true;
		}

		// Token: 0x06005AFE RID: 23294 RVA: 0x002A35EC File Offset: 0x002A17EC
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string text = name;
			string a = text;
			if (a == "ButtonCloseView")
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005AFF RID: 23295 RVA: 0x002A361D File Offset: 0x002A181D
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopTask, new GEvent.Callback(this.TopTask));
		}

		// Token: 0x06005B00 RID: 23296 RVA: 0x002A363C File Offset: 0x002A183C
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopTask, new GEvent.Callback(this.TopTask));
		}

		// Token: 0x06005B01 RID: 23297 RVA: 0x002A365B File Offset: 0x002A185B
		private void Awake()
		{
			this.taskGroupToggleGroup.OnActiveIndexChange += this.OnToggleChange;
		}

		// Token: 0x06005B02 RID: 23298 RVA: 0x002A3676 File Offset: 0x002A1876
		private void OnDestroy()
		{
			this.taskGroupToggleGroup.OnActiveIndexChange -= this.OnToggleChange;
		}

		// Token: 0x06005B03 RID: 23299 RVA: 0x002A3694 File Offset: 0x002A1894
		private void TopTask(ArgumentBox box)
		{
			bool flag = this._tween != null;
			if (!flag)
			{
				List<TaskGroup> list = this.content.GetComponentsInChildren<TaskGroup>().ToList<TaskGroup>();
				List<int> taskIDList;
				box.Get<List<int>>("TopTaskID", out taskIDList);
				this.content.GetComponent<ContentSizeFitter>().enabled = false;
				this.content.GetComponent<VerticalLayoutGroup>().enabled = false;
				float heightValue = 0f;
				this.BuildTaskIdToIndexMap(this._taskIdToIndexMap, list);
				this._processedIndices.Clear();
				int count = 0;
				for (int i = 0; i < Math.Min(taskIDList.Count, list.Count); i++)
				{
					int tempIndex;
					bool flag2 = !this._taskIdToIndexMap.TryGetValue(taskIDList[i], out tempIndex);
					if (!flag2)
					{
						bool flag3 = this._processedIndices.Contains(tempIndex);
						if (!flag3)
						{
							this._processedIndices.Add(tempIndex);
							bool flag4 = tempIndex == count;
							if (flag4)
							{
								list[tempIndex].SetTopIcon();
								heightValue += (list[tempIndex].transform as RectTransform).rect.height;
								count++;
							}
							else
							{
								bool flag5 = count == 0;
								if (flag5)
								{
									Vector3 targetPos = new Vector3(list[tempIndex].transform.localPosition.x, 0f, list[tempIndex].transform.localPosition.z);
									this._tween = list[tempIndex].transform.DOLocalMove(targetPos, 0.1f, false);
									Tween tween = this._tween;
									tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, new TweenCallback(delegate()
									{
										list[tempIndex].transform.SetSiblingIndex(0);
										list[tempIndex].SetTopIcon();
										this._tween = null;
									}));
								}
								else
								{
									this._tween = list[tempIndex].transform.DOLocalMove(new Vector3(list[tempIndex].transform.localPosition.x, -heightValue, list[tempIndex].transform.localPosition.z), 0.1f, false);
									Tween tween2 = this._tween;
									tween2.onComplete = (TweenCallback)Delegate.Combine(tween2.onComplete, new TweenCallback(delegate()
									{
										list[tempIndex].SetTopIcon();
										this._tween = null;
									}));
									list[tempIndex].transform.SetSiblingIndex(count);
								}
								heightValue += (list[tempIndex].transform as RectTransform).rect.height;
								count++;
							}
						}
					}
				}
				base.DelayCall(delegate
				{
					this.content.GetComponent<ContentSizeFitter>().enabled = true;
					this.content.GetComponent<VerticalLayoutGroup>().enabled = true;
				}, 0.1f);
			}
		}

		// Token: 0x06005B04 RID: 23300 RVA: 0x002A3A20 File Offset: 0x002A1C20
		private void BuildTaskIdToIndexMap(Dictionary<int, int> map, List<TaskGroup> list)
		{
			map.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				foreach (TaskDisplayData taskData in list[i].GetTaskGroupData())
				{
					map.TryAdd(taskData.InnerTaskData.TaskInfoId, i);
				}
			}
		}

		// Token: 0x06005B05 RID: 23301 RVA: 0x002A3AA8 File Offset: 0x002A1CA8
		private void GetTaskDataAndShow()
		{
			this._taskGroupDataDictionary = this.TaskModel.GetCurrentTaskGroupData();
			foreach (TaskGroup item in this._taskGroupList)
			{
				Object.Destroy(item.gameObject);
			}
			this._taskGroupList.Clear();
			bool flag = this._taskGroupDataDictionary.Count == 0;
			if (!flag)
			{
				List<int> topTaskInfoIDList = this.TaskModel.GetCurrentTopTaskIDList();
				CToggleGroup toggleGroup = this.taskGroupToggleGroup;
				bool flag2 = this._taskInfoId != -1;
				if (flag2)
				{
					this.taskGroupToggleGroup.SetWithoutNotify(0);
				}
				int key = toggleGroup.GetActiveIndex();
				this._taskGroupDataList.Clear();
				bool inAdventure = this.AdventureModel.AdventureTaiwu.InAdventure;
				switch (key)
				{
				case 0:
					this.background.SetTexture("ui9_tex_lowerpopup_notes_bg_0");
					foreach (TaskGroupData item2 in this._taskGroupDataDictionary.Values)
					{
						TaskChainItem config = TaskChain.Instance.GetItem(item2.dataList[0].InnerTaskData.TaskChainId);
						bool flag3 = !inAdventure && config.RelateAdventure;
						if (!flag3)
						{
							bool flag4 = item2.dataList.Any((TaskDisplayData v) => v.FinishedDate == -1);
							if (flag4)
							{
								this._taskGroupDataList.Add(item2);
							}
						}
					}
					break;
				case 1:
					this.background.SetTexture("ui9_tex_lowerpopup_notes_bg_1");
					foreach (TaskGroupData item3 in this._taskGroupDataDictionary.Values)
					{
						TaskChainItem config2 = TaskChain.Instance.GetItem(item3.dataList[0].InnerTaskData.TaskChainId);
						bool relateAdventure = config2.RelateAdventure;
						if (!relateAdventure)
						{
							bool flag5 = config2.Group == (ETaskChainGroup)(key - 1);
							if (flag5)
							{
								bool flag6 = item3.dataList.Any((TaskDisplayData v) => v.FinishedDate == -1);
								if (flag6)
								{
									this._taskGroupDataList.Add(item3);
								}
							}
						}
					}
					break;
				case 2:
				case 3:
					this.background.SetTexture("ui9_tex_lowerpopup_notes_bg_2");
					foreach (TaskGroupData item4 in this._taskGroupDataDictionary.Values)
					{
						TaskChainItem config3 = TaskChain.Instance.GetItem(item4.dataList[0].InnerTaskData.TaskChainId);
						bool relateAdventure2 = config3.RelateAdventure;
						if (!relateAdventure2)
						{
							bool flag7 = config3.Group == (ETaskChainGroup)(key - 1);
							if (flag7)
							{
								bool flag8 = item4.dataList.Any((TaskDisplayData v) => v.FinishedDate == -1);
								if (flag8)
								{
									this._taskGroupDataList.Add(item4);
								}
							}
						}
					}
					break;
				case 4:
					this.background.SetTexture("ui9_tex_lowerpopup_notes_bg_2");
					foreach (TaskGroupData item5 in this._taskGroupDataDictionary.Values)
					{
						TaskChainItem config4 = TaskChain.Instance.GetItem(item5.dataList[0].InnerTaskData.TaskChainId);
						bool relateAdventure3 = config4.RelateAdventure;
						if (relateAdventure3)
						{
							bool flag9 = item5.dataList.Any((TaskDisplayData v) => v.FinishedDate == -1);
							if (flag9)
							{
								this._taskGroupDataList.Add(item5);
							}
						}
					}
					break;
				case 5:
					this.background.SetTexture("ui9_tex_lowerpopup_notes_bg_2");
					foreach (TaskGroupData item6 in this._taskGroupDataDictionary.Values)
					{
						TaskChainItem config5 = TaskChain.Instance.GetItem(item6.dataList[0].InnerTaskData.TaskChainId);
						bool flag10 = !inAdventure && config5.RelateAdventure;
						if (!flag10)
						{
							bool flag11 = item6.dataList.Any((TaskDisplayData v) => v.FinishedDate != -1);
							if (flag11)
							{
								this._taskGroupDataList.Add(item6);
							}
							this._taskGroupDataList.Sort((TaskGroupData x, TaskGroupData y) => y.dataList.Max((TaskDisplayData a) => a.FinishedDate).CompareTo(x.dataList.Max((TaskDisplayData a) => a.FinishedDate)));
						}
					}
					break;
				}
				bool isFinishedPage = key == this.taskGroupToggleGroup.GetAll().Count - 1;
				foreach (TaskGroupData item7 in this._taskGroupDataList)
				{
					GameObject obj = Object.Instantiate<GameObject>(this.taskGroupItem, this.content, false);
					obj.SetActive(true);
					bool isBlocked = item7.dataList[0].InnerTaskData.IsBlocked;
					if (isBlocked)
					{
						obj.transform.SetAsLastSibling();
					}
					TaskGroup taskGroup = obj.GetComponent<TaskGroup>();
					taskGroup.SetData(item7.dataList, topTaskInfoIDList, this, isFinishedPage);
					this._taskGroupList.Add(taskGroup);
				}
				bool flag12 = this._taskInfoId != -1;
				if (flag12)
				{
					List<TaskGroup> list = this.content.GetComponentsInChildren<TaskGroup>().ToList<TaskGroup>();
					int index = this.GetTaskIdToIndex(this._taskInfoId, list);
					CScrollRect cScrollRect = this.taskScrollRect;
					cScrollRect.ScrollTo(list[index].transform as RectTransform, 0.3f);
					this._taskInfoId = -1;
				}
			}
		}

		// Token: 0x06005B06 RID: 23302 RVA: 0x002A4180 File Offset: 0x002A2380
		private int GetTaskIdToIndex(int taskId, List<TaskGroup> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				foreach (TaskDisplayData taskData in list[i].GetTaskGroupData())
				{
					bool flag = taskData.InnerTaskData.TaskInfoId == taskId;
					if (flag)
					{
						return i;
					}
				}
			}
			return 0;
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x002A4210 File Offset: 0x002A2410
		public bool IsRuntimeTopAnimation()
		{
			return this._tween != null;
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x002A422B File Offset: 0x002A242B
		private void OnToggleChange(int newTog, int oldTog)
		{
			this.GetTaskDataAndShow();
		}

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06005B09 RID: 23305 RVA: 0x002A4235 File Offset: 0x002A2435
		private AdventureRemakeModel AdventureModel
		{
			get
			{
				return SingletonObject.getInstance<AdventureRemakeModel>();
			}
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x002A423C File Offset: 0x002A243C
		private void RefreshTaskGroupToggleGroup()
		{
			GameObject gameObject = this.taskGroupToggleGroup.Get(4).gameObject;
			AdventureTaiwu adventureTaiwu = this.AdventureModel.AdventureTaiwu;
			gameObject.SetActive(adventureTaiwu != null && adventureTaiwu.InAdventure);
		}

		// Token: 0x04003EBC RID: 16060
		[SerializeField]
		private float animaTime;

		// Token: 0x04003EBD RID: 16061
		[SerializeField]
		private GameObject taskGroupItem;

		// Token: 0x04003EBE RID: 16062
		[SerializeField]
		private RectTransform content;

		// Token: 0x04003EBF RID: 16063
		[SerializeField]
		private CToggleGroup taskGroupToggleGroup;

		// Token: 0x04003EC0 RID: 16064
		[SerializeField]
		private CScrollbar verticalScrollbar;

		// Token: 0x04003EC1 RID: 16065
		[SerializeField]
		private CScrollRect taskScrollRect;

		// Token: 0x04003EC2 RID: 16066
		[SerializeField]
		private CRawImage background;

		// Token: 0x04003EC3 RID: 16067
		private Dictionary<int, TaskGroupData> _taskGroupDataDictionary = new Dictionary<int, TaskGroupData>();

		// Token: 0x04003EC4 RID: 16068
		private readonly List<TaskGroup> _taskGroupList = new List<TaskGroup>();

		// Token: 0x04003EC5 RID: 16069
		private readonly Dictionary<int, int> _taskIdToIndexMap = new Dictionary<int, int>();

		// Token: 0x04003EC6 RID: 16070
		private readonly HashSet<int> _processedIndices = new HashSet<int>();

		// Token: 0x04003EC7 RID: 16071
		private Tween _tween;

		// Token: 0x04003EC8 RID: 16072
		private readonly List<TaskGroupData> _taskGroupDataList = new List<TaskGroupData>();

		// Token: 0x04003EC9 RID: 16073
		private int _taskInfoId;

		// Token: 0x04003ECA RID: 16074
		private const sbyte AdventureToggleIndex = 4;
	}
}
