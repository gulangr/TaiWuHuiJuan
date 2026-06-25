using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Map;
using GameData.Domains.World.Task;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000309 RID: 777
public class TaskGroup : MonoBehaviour
{
	// Token: 0x06002DD3 RID: 11731 RVA: 0x0016AD40 File Offset: 0x00168F40
	private void Awake()
	{
		base.GetComponent<CButtonObsolete>().onClick.AddListener(new UnityAction(this.TopTask));
		base.GetComponent<PointerTrigger>().EnterEvent.AddListener(delegate()
		{
			bool isBlocked = this._isBlocked;
			if (!isBlocked)
			{
				bool flag = this._contentBack != null;
				if (flag)
				{
					this._contentBack.SetSprite("ui_mainpopup_note_base_2_1", false, null);
				}
				bool flag2 = this._topIconHolder != null;
				if (flag2)
				{
					this._topIconHolder.SetActive(true);
				}
				switch (this._taskChainCfg.GetItem(this.ChainId).Group)
				{
				case ETaskChainGroup.MainStory:
					this._thisCImage.SetSprite("ui_mainpopup_note_title_main_1_0", false, null);
					break;
				case ETaskChainGroup.OptionalTasks:
					this._thisCImage.SetSprite("ui_mainpopup_note_title_main_1_2", false, null);
					break;
				case ETaskChainGroup.SectMainStory:
					this._thisCImage.SetSprite("ui_mainpopup_note_title_main_1_1", false, null);
					break;
				case ETaskChainGroup.CustomizeTasks:
					this._thisCImage.SetSprite("ui_mainpopup_note_title_main_1_3", false, null);
					break;
				case ETaskChainGroup.Count:
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		});
		base.GetComponent<PointerTrigger>().ExitEvent.AddListener(delegate()
		{
			bool flag = this._taskData == null;
			if (!flag)
			{
				bool flag2 = this._isBlocked || (this.TaskModel.GetCurrentTopTaskIDList() != null && this.TaskModel.GetCurrentTopTaskIDList().Contains(this._taskData[0].InnerTaskData.TaskInfoId));
				if (!flag2)
				{
					bool flag3 = this._contentBack != null;
					if (flag3)
					{
						this._topIconHolder.SetActive(false);
					}
					bool flag4 = this._topIconHolder != null;
					if (flag4)
					{
						this._contentBack.SetSprite("ui_mainpopup_note_base_2_0", false, null);
					}
					switch (this._taskChainCfg.GetItem(this.ChainId).Group)
					{
					case ETaskChainGroup.MainStory:
						this._thisCImage.SetSprite("ui_mainpopup_note_title_main_0_0", false, null);
						break;
					case ETaskChainGroup.OptionalTasks:
						this._thisCImage.SetSprite("ui_mainpopup_note_title_main_0_2", false, null);
						break;
					case ETaskChainGroup.SectMainStory:
						this._thisCImage.SetSprite("ui_mainpopup_note_title_main_0_1", false, null);
						break;
					case ETaskChainGroup.CustomizeTasks:
						this._thisCImage.SetSprite("ui_mainpopup_note_title_main_0_3", false, null);
						break;
					case ETaskChainGroup.Count:
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		});
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x0016ADA8 File Offset: 0x00168FA8
	public void SetData(List<TaskDisplayData> data, List<int> topTaskInfoIDList, UI_TaskPopUpPanel2 taskPopUpPanel)
	{
		this._isBlocked = false;
		this._taskPopUpPanel = taskPopUpPanel;
		this._taskData = data;
		this._thisCImage.SetSprite(this.GetTaskBackground(this._taskChainCfg.GetItem(this.ChainId).Group), false, null);
		this._taskIcon.SetSprite(this.GetTaskIcon(this._taskChainCfg.GetItem(this.ChainId).Group, this._taskChainCfg.GetItem(this.ChainId).Sect), false, null);
		switch (this._taskChainCfg.GetItem(this.ChainId).Group)
		{
		case ETaskChainGroup.MainStory:
			this._taskIcon.GetComponent<TooltipInvoker>().enabled = false;
			goto IL_F9;
		case ETaskChainGroup.OptionalTasks:
			this._taskIcon.GetComponent<TooltipInvoker>().enabled = false;
			goto IL_F9;
		case ETaskChainGroup.SectMainStory:
			this._taskIcon.GetComponent<TooltipInvoker>().enabled = true;
			goto IL_F9;
		case ETaskChainGroup.Count:
			goto IL_F9;
		}
		throw new ArgumentOutOfRangeException();
		IL_F9:
		ArgumentBox argsBox = new ArgumentBox();
		argsBox.Set("arg0", Organization.Instance.GetItem(this._taskChainCfg.GetItem(this.ChainId).Sect).Name ?? "");
		this._taskIcon.GetComponent<TooltipInvoker>().RuntimeParam = argsBox;
		foreach (TaskDisplayData item in this._taskData)
		{
			this.GenerateTask(item, topTaskInfoIDList);
		}
		bool isBlocked = this._isBlocked;
		if (isBlocked)
		{
			this.OpenTaskHinder();
		}
		else
		{
			this.CloseTaskHinder();
		}
		bool flag = this._encyclopediaLinkScroll != null;
		if (flag)
		{
			this._encyclopediaLinkScroll.OnItemRender = new Action<int, Refers>(this.OnEncyclopediaLinkRender);
		}
		bool flag2 = this._jumpButton != null;
		if (flag2)
		{
			this._jumpButton.gameObject.SetActive(this._taskData[0].TargetLocation.IsValid());
			this._jumpButton.ClearAndAddListener(delegate
			{
				SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(this._taskData[0].TargetLocation, 0);
				this._taskPopUpPanel.QuickHide();
			});
		}
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x0016AFF0 File Offset: 0x001691F0
	public void OpenTopIcon()
	{
		this._topIcon.SetActive(true);
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x0016B000 File Offset: 0x00169200
	public void CloseTopIcon()
	{
		this._topIcon.SetActive(false);
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x0016B010 File Offset: 0x00169210
	public List<TaskDisplayData> GetTaskGroupData()
	{
		return this._taskData;
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x0016B028 File Offset: 0x00169228
	public void Clear()
	{
		foreach (GameObject taskItem in this._taskInfoList)
		{
			bool flag = taskItem != null;
			if (flag)
			{
				Object.Destroy(taskItem);
			}
		}
		this._taskInfoList.Clear();
		this._isBlocked = false;
		this._taskData = null;
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x0016B0A8 File Offset: 0x001692A8
	private void GenerateTask(TaskDisplayData data, List<int> topTaskInfoIDList)
	{
		GameObject obj = Object.Instantiate<GameObject>(this._taskItem, this._taskContents, false);
		obj.SetActive(true);
		int taskInfoId = data.InnerTaskData.TaskInfoId;
		MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
		string invalid = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid);
		string taskDescription = (data.DisplayType != 0) ? TaskPanelHelper.GetTaskDescription(data.DisplayType, data, areas, invalid) : this._taskInfoCfg.GetItem(taskInfoId).TaskDescription;
		bool flag = !this._taskChainCfg.GetItem(data.InnerTaskData.TaskChainId).Name.IsNullOrEmpty();
		if (flag)
		{
			this._titleTMP.text = this._taskChainCfg.GetItem(data.InnerTaskData.TaskChainId).Name;
			obj.GetComponent<TaskGroup_TaskItem>().SetData(this._taskInfoCfg.GetItem(taskInfoId).TaskTitle, taskDescription);
		}
		else
		{
			this._titleTMP.text = this._taskInfoCfg.GetItem(taskInfoId).TaskTitle;
			obj.GetComponent<TaskGroup_TaskItem>().SetData("", taskDescription);
		}
		this._taskInfoList.Add(obj);
		bool isBlocked = data.InnerTaskData.IsBlocked;
		if (isBlocked)
		{
			this._isBlocked = true;
		}
		bool flag2 = this._topIconHolder != null;
		if (flag2)
		{
			this._topIconHolder.SetActive(topTaskInfoIDList != null && topTaskInfoIDList.Contains(taskInfoId));
		}
		this._topIcon.SetActive(topTaskInfoIDList != null && topTaskInfoIDList.Contains(taskInfoId));
		this._taskItem.SetActive(false);
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x0016B23C File Offset: 0x0016943C
	private void TopTask()
	{
		bool flag = this._taskData[0].InnerTaskData.IsBlocked || this._taskPopUpPanel.IsRuntimeTopAnimation();
		if (!flag)
		{
			int taskInfoId = this._taskData[0].InnerTaskData.TaskInfoId;
			TaskChainItem taskChain = this._taskChainCfg.GetItem(this._taskData[0].InnerTaskData.TaskChainId);
			bool flag2 = !taskChain.Name.IsNullOrEmpty() && taskChain.Type == ETaskChainType.Parallel;
			if (flag2)
			{
				taskInfoId = this._taskChainCfg.GetItem(this._taskData[0].InnerTaskData.TaskChainId).TaskList[0];
			}
			List<int> currentTopTaskIDList = this.TaskModel.GetCurrentTopTaskIDList();
			bool flag3 = currentTopTaskIDList == null || !currentTopTaskIDList.Contains(taskInfoId);
			if (flag3)
			{
				SingletonObject.getInstance<TaskModel>().TopEvent(taskInfoId, 0);
			}
			else
			{
				SingletonObject.getInstance<TaskModel>().TopEvent(taskInfoId, -1);
			}
		}
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x0016B348 File Offset: 0x00169548
	private void OpenTaskHinder()
	{
		this._taskHinder.SetActive(true);
		this._verticalLayoutGroup.padding.bottom = 0;
		this._titleTMP.alpha = 0.33333334f;
		this._thisCImage.SetAlpha(0.33333334f);
		this._taskIcon.SetAlpha(0.33333334f);
		foreach (GameObject item in this._taskInfoList)
		{
			item.GetComponent<TaskGroup_TaskItem>().BlockedState();
		}
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x0016B3F8 File Offset: 0x001695F8
	private void CloseTaskHinder()
	{
		this._taskHinder.SetActive(false);
		this._verticalLayoutGroup.padding.bottom = 0;
		this._titleTMP.alpha = 1f;
		this._thisCImage.SetAlpha(1f);
		this._taskIcon.SetAlpha(1f);
		foreach (GameObject item in this._taskInfoList)
		{
			item.GetComponent<TaskGroup_TaskItem>().DefaultState();
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x0016B4A8 File Offset: 0x001696A8
	private string GetTaskBackground(ETaskChainGroup eTaskChainGroup)
	{
		if (!true)
		{
		}
		string result;
		switch (eTaskChainGroup)
		{
		case ETaskChainGroup.MainStory:
			result = "ui_mainpopup_note_title_main_0_0";
			break;
		case ETaskChainGroup.OptionalTasks:
			result = "ui_mainpopup_note_title_main_0_2";
			break;
		case ETaskChainGroup.SectMainStory:
			result = "ui_mainpopup_note_title_main_0_1";
			break;
		case ETaskChainGroup.CustomizeTasks:
			result = "ui_mainpopup_note_title_main_0_3";
			break;
		default:
			throw new ArgumentOutOfRangeException("eTaskChainGroup", eTaskChainGroup, null);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x0016B510 File Offset: 0x00169710
	private string GetTaskIcon(ETaskChainGroup eTaskChainGroup, sbyte sect = -1)
	{
		if (!true)
		{
		}
		string result;
		switch (eTaskChainGroup)
		{
		case ETaskChainGroup.MainStory:
			result = "ui_mainpopup_note_icon_type_0";
			break;
		case ETaskChainGroup.OptionalTasks:
			result = "ui_mainpopup_note_icon_type_2";
			break;
		case ETaskChainGroup.SectMainStory:
			result = "ui_mainpopup_note_icon_type_1";
			break;
		case ETaskChainGroup.CustomizeTasks:
			result = "ui_mainpopup_note_icon_type_3";
			break;
		case ETaskChainGroup.Count:
			result = "";
			break;
		default:
			throw new ArgumentOutOfRangeException("eTaskChainGroup");
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x0016B57A File Offset: 0x0016977A
	private void OnEncyclopediaLinkRender(int index, Refers refers)
	{
	}

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x06002DE0 RID: 11744 RVA: 0x0016B57D File Offset: 0x0016977D
	private TaskInfo _taskInfoCfg
	{
		get
		{
			return TaskInfo.Instance;
		}
	}

	// Token: 0x17000505 RID: 1285
	// (get) Token: 0x06002DE1 RID: 11745 RVA: 0x0016B584 File Offset: 0x00169784
	private TaskChain _taskChainCfg
	{
		get
		{
			return TaskChain.Instance;
		}
	}

	// Token: 0x17000506 RID: 1286
	// (get) Token: 0x06002DE2 RID: 11746 RVA: 0x0016B58B File Offset: 0x0016978B
	private TaskModel TaskModel
	{
		get
		{
			return SingletonObject.getInstance<TaskModel>();
		}
	}

	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x06002DE3 RID: 11747 RVA: 0x0016B592 File Offset: 0x00169792
	private int ChainId
	{
		get
		{
			return (this._taskData.Count > 0) ? this._taskData[0].InnerTaskData.TaskChainId : 0;
		}
	}

	// Token: 0x04002126 RID: 8486
	[SerializeField]
	private TextMeshProUGUI _titleTMP;

	// Token: 0x04002127 RID: 8487
	[SerializeField]
	private GameObject _taskItem;

	// Token: 0x04002128 RID: 8488
	[SerializeField]
	private VerticalLayoutGroup _verticalLayoutGroup;

	// Token: 0x04002129 RID: 8489
	[SerializeField]
	private GameObject _taskHinder;

	// Token: 0x0400212A RID: 8490
	[SerializeField]
	private RectTransform _taskContents;

	// Token: 0x0400212B RID: 8491
	[SerializeField]
	private CImage _thisCImage;

	// Token: 0x0400212C RID: 8492
	[SerializeField]
	private CButtonObsolete _playVideoButton;

	// Token: 0x0400212D RID: 8493
	[SerializeField]
	private GameObject _topIcon;

	// Token: 0x0400212E RID: 8494
	[SerializeField]
	private CImage _topIconBg;

	// Token: 0x0400212F RID: 8495
	[SerializeField]
	private GameObject _topIconHolder;

	// Token: 0x04002130 RID: 8496
	[SerializeField]
	private CImage _taskIcon;

	// Token: 0x04002131 RID: 8497
	[SerializeField]
	private CButtonObsolete _jumpButton;

	// Token: 0x04002132 RID: 8498
	[SerializeField]
	private InfinityScrollLegacy _encyclopediaLinkScroll;

	// Token: 0x04002133 RID: 8499
	[SerializeField]
	private CImage _contentBack;

	// Token: 0x04002134 RID: 8500
	private const string MainStoryTaskSpritePath = "ui_mainpopup_note_title_main_0_0";

	// Token: 0x04002135 RID: 8501
	private const string SectMainStorySpritePath = "ui_mainpopup_note_title_main_0_1";

	// Token: 0x04002136 RID: 8502
	private const string OptionalTasksSpritePath = "ui_mainpopup_note_title_main_0_2";

	// Token: 0x04002137 RID: 8503
	private const string CustomizeTasksSpritePath = "ui_mainpopup_note_title_main_0_3";

	// Token: 0x04002138 RID: 8504
	private const string MainStoryTaskHighlightSpritePath = "ui_mainpopup_note_title_main_1_0";

	// Token: 0x04002139 RID: 8505
	private const string SectMainStoryHighlightSpritePath = "ui_mainpopup_note_title_main_1_1";

	// Token: 0x0400213A RID: 8506
	private const string OptionalTasksHighlightSpritePath = "ui_mainpopup_note_title_main_1_2";

	// Token: 0x0400213B RID: 8507
	private const string CustomizeTasksHighlightSpritePath = "ui_mainpopup_note_title_main_1_3";

	// Token: 0x0400213C RID: 8508
	private const int BlockedColor = 83;

	// Token: 0x0400213D RID: 8509
	private const string MainStoryTaskIconPath = "ui_mainpopup_note_icon_type_0";

	// Token: 0x0400213E RID: 8510
	private const string SectMainStoryIconPath = "ui_mainpopup_note_icon_type_1";

	// Token: 0x0400213F RID: 8511
	private const string OptionalTasksIconPath = "ui_mainpopup_note_icon_type_2";

	// Token: 0x04002140 RID: 8512
	private const string CustomizeTasksIconPath = "ui_mainpopup_note_icon_type_3";

	// Token: 0x04002141 RID: 8513
	private List<TaskDisplayData> _taskData;

	// Token: 0x04002142 RID: 8514
	private List<GameObject> _taskInfoList = new List<GameObject>();

	// Token: 0x04002143 RID: 8515
	private UI_TaskPopUpPanel2 _taskPopUpPanel;

	// Token: 0x04002144 RID: 8516
	private readonly string[] SectImg = new string[]
	{
		"charactermenu3_19_menpai_0",
		"charactermenu3_19_menpai_1",
		"charactermenu3_19_menpai_2",
		"charactermenu3_19_menpai_3",
		"charactermenu3_19_menpai_4",
		"charactermenu3_19_menpai_5",
		"charactermenu3_19_menpai_6",
		"charactermenu3_19_menpai_7",
		"charactermenu3_19_menpai_8",
		"charactermenu3_19_menpai_9",
		"charactermenu3_19_menpai_10",
		"charactermenu3_19_menpai_11",
		"charactermenu3_19_menpai_12",
		"charactermenu3_19_menpai_13",
		"charactermenu3_19_menpai_14",
		"charactermenu3_19_menpai_15"
	};

	// Token: 0x04002145 RID: 8517
	private Color _color;

	// Token: 0x04002146 RID: 8518
	private bool _isBlocked = false;
}
