using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.World.Task;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200030F RID: 783
public class UI_TaskPopUpPanel2 : UIBase
{
	// Token: 0x1700050F RID: 1295
	// (get) Token: 0x06002E2C RID: 11820 RVA: 0x0016D3B9 File Offset: 0x0016B5B9
	private TaskModel TaskModel
	{
		get
		{
			return SingletonObject.getInstance<TaskModel>();
		}
	}

	// Token: 0x17000510 RID: 1296
	// (get) Token: 0x06002E2D RID: 11821 RVA: 0x0016D3C0 File Offset: 0x0016B5C0
	private GameObject TaskGroupItem
	{
		get
		{
			return base.CGet<GameObject>("Task_Group_Item");
		}
	}

	// Token: 0x17000511 RID: 1297
	// (get) Token: 0x06002E2E RID: 11822 RVA: 0x0016D3CD File Offset: 0x0016B5CD
	private RectTransform Parent
	{
		get
		{
			return base.CGet<RectTransform>("Content");
		}
	}

	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x06002E2F RID: 11823 RVA: 0x0016D3DA File Offset: 0x0016B5DA
	private CButtonObsolete ButtonClosePopup
	{
		get
		{
			return base.CGet<CButtonObsolete>("ButtonClosePopup");
		}
	}

	// Token: 0x17000513 RID: 1299
	// (get) Token: 0x06002E30 RID: 11824 RVA: 0x0016D3E7 File Offset: 0x0016B5E7
	private RectTransform BottomPanel
	{
		get
		{
			return base.CGet<RectTransform>("BottomPanel");
		}
	}

	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x06002E31 RID: 11825 RVA: 0x0016D3F4 File Offset: 0x0016B5F4
	private CScrollbarLegacy VerticalScrollbar
	{
		get
		{
			return base.CGet<CScrollbarLegacy>("VerticalScrollbar");
		}
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x0016D401 File Offset: 0x0016B601
	private void Awake()
	{
		this.ButtonClosePopup.ClearAndAddListener(delegate
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = this.BottomPanel.transform.DOLocalMove(new Vector3(0f, 1380f, 0f), this.animaTime, false);
			tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
			{
				UIManager.Instance.HideUI(UIElement.TaskPopPanel);
			}));
		});
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x0016D41C File Offset: 0x0016B61C
	public override void QuickHide()
	{
		TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = this.BottomPanel.transform.DOLocalMove(new Vector3(0f, 1380f, 0f), this.animaTime, false);
		tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
		{
			base.QuickHide();
		}));
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x0016D478 File Offset: 0x0016B678
	public override void OnInit(ArgumentBox argsBox)
	{
		this.VerticalScrollbar.value = 0f;
		CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("ToggleGroup");
		toggleGroup.InitPreOnToggle(-1);
		CToggleGroupObsolete ctoggleGroupObsolete = toggleGroup;
		ctoggleGroupObsolete.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(ctoggleGroupObsolete.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange));
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
		{
			this.BottomPanel.transform.DOLocalMove(new Vector3(0f, 0f, 0f), this.animaTime, false);
		}));
		this._taskInfoId = -1;
		argsBox.Get("TaskInfoId", out this._taskInfoId);
		this.GetTaskDataAndShow();
		this.Parent.GetComponent<ContentSizeFitter>().enabled = true;
		this.Parent.GetComponent<VerticalLayoutGroup>().enabled = true;
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x0016D538 File Offset: 0x0016B738
	private void OnEnable()
	{
		GEvent.Add(UiEvents.TopTask, new GEvent.Callback(this.TopTask));
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x0016D557 File Offset: 0x0016B757
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.TopTask, new GEvent.Callback(this.TopTask));
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x0016D578 File Offset: 0x0016B778
	private void TopTask(ArgumentBox box)
	{
		bool flag = this._tween != null;
		if (!flag)
		{
			List<TaskGroup> list = this.Parent.GetComponentsInChildren<TaskGroup>().ToList<TaskGroup>();
			List<int> taskIDList;
			box.Get<List<int>>("TopTaskID", out taskIDList);
			this.Parent.GetComponent<ContentSizeFitter>().enabled = false;
			this.Parent.GetComponent<VerticalLayoutGroup>().enabled = false;
			float heightValue = list[0].GetComponent<RectTransform>().rect.height;
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
						count++;
						bool flag4 = i == 0;
						if (flag4)
						{
							Vector3 targetPos = new Vector3(list[tempIndex].transform.localPosition.x, -heightValue, list[tempIndex].transform.localPosition.z);
							this._tween = list[tempIndex].transform.DOLocalMove(targetPos, 0.1f, false);
							Tween tween = this._tween;
							tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, new TweenCallback(delegate()
							{
								list[tempIndex].OpenTopIcon();
								list[tempIndex].transform.SetSiblingIndex(0);
								this.Parent.GetComponent<ContentSizeFitter>().enabled = true;
								this.Parent.GetComponent<VerticalLayoutGroup>().enabled = true;
								this._tween = null;
							}));
						}
						else
						{
							list[tempIndex].transform.DOLocalMove(new Vector3(list[tempIndex].transform.localPosition.x, -heightValue * (float)count, list[tempIndex].transform.localPosition.z), 0.1f, false);
							list[tempIndex].transform.SetSiblingIndex(count - 1);
							list[tempIndex].CloseTopIcon();
						}
					}
				}
			}
		}
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x0016D85C File Offset: 0x0016BA5C
	private void BuildTaskIdToIndexMap(Dictionary<int, int> map, List<TaskGroup> list)
	{
		map.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			foreach (TaskDisplayData taskData in list[i].GetTaskGroupData())
			{
				bool flag = !map.ContainsKey(taskData.InnerTaskData.TaskInfoId);
				if (flag)
				{
					map[taskData.InnerTaskData.TaskInfoId] = i;
				}
			}
		}
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x0016D900 File Offset: 0x0016BB00
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
			CToggleGroupObsolete toggleGroup = base.CGet<CToggleGroupObsolete>("ToggleGroup");
			int key = toggleGroup.GetActive().Key;
			this._taskGroupDataList.Clear();
			int num = key;
			int num2 = num;
			if (num2 != 0)
			{
				if (num2 - 1 <= 2)
				{
					foreach (TaskGroupData item2 in this._taskGroupDataDictionary.Values)
					{
						bool flag2 = TaskChain.Instance.GetItem(item2.dataList[0].InnerTaskData.TaskChainId).Group == (ETaskChainGroup)(key - 1);
						if (flag2)
						{
							this._taskGroupDataList.Add(item2);
						}
					}
				}
			}
			else
			{
				this._taskGroupDataList.AddRange(this._taskGroupDataDictionary.Values);
			}
			foreach (TaskGroupData item3 in this._taskGroupDataList)
			{
				GameObject obj = Object.Instantiate<GameObject>(this.TaskGroupItem, this.Parent, false);
				obj.SetActive(true);
				bool isBlocked = item3.dataList[0].InnerTaskData.IsBlocked;
				if (isBlocked)
				{
					obj.transform.SetAsLastSibling();
				}
				TaskGroup taskGroup = obj.GetComponent<TaskGroup>();
				taskGroup.SetData(item3.dataList, topTaskInfoIDList, this);
				this._taskGroupList.Add(taskGroup);
			}
			bool flag3 = this._taskInfoId != -1;
			if (flag3)
			{
				List<TaskGroup> list = this.Parent.GetComponentsInChildren<TaskGroup>().ToList<TaskGroup>();
				int index = this.GetTaskIdToIndex(this._taskInfoId, list);
				CScrollRectLegacy cScrollRect = base.CGet<CScrollRectLegacy>("VerticalScrollView");
				cScrollRect.ScrollTo(list[index].transform as RectTransform, 0.3f, default(Vector2));
				this._taskInfoId = -1;
			}
		}
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x0016DBB4 File Offset: 0x0016BDB4
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

	// Token: 0x06002E3B RID: 11835 RVA: 0x0016DC44 File Offset: 0x0016BE44
	public bool IsRuntimeTopAnimation()
	{
		return this._tween != null;
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x0016DC5F File Offset: 0x0016BE5F
	private void OnToggleChange(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		this.GetTaskDataAndShow();
	}

	// Token: 0x04002171 RID: 8561
	[SerializeField]
	private float animaTime;

	// Token: 0x04002172 RID: 8562
	private Dictionary<int, TaskGroupData> _taskGroupDataDictionary = new Dictionary<int, TaskGroupData>();

	// Token: 0x04002173 RID: 8563
	private List<TaskGroup> _taskGroupList = new List<TaskGroup>();

	// Token: 0x04002174 RID: 8564
	private readonly Dictionary<int, int> _taskIdToIndexMap = new Dictionary<int, int>();

	// Token: 0x04002175 RID: 8565
	private readonly HashSet<int> _processedIndices = new HashSet<int>();

	// Token: 0x04002176 RID: 8566
	private Tween _tween;

	// Token: 0x04002177 RID: 8567
	private readonly List<TaskGroupData> _taskGroupDataList = new List<TaskGroupData>();

	// Token: 0x04002178 RID: 8568
	private int _taskInfoId;
}
