using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.World;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.Domains.World.Task;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.TaskPanel
{
	// Token: 0x02000755 RID: 1877
	public class TaskGroup : MonoBehaviour
	{
		// Token: 0x06005ADE RID: 23262 RVA: 0x002A272C File Offset: 0x002A092C
		private void Awake()
		{
			bool flag = this._topButton != null;
			if (flag)
			{
				this._topButton.onClick.AddListener(new UnityAction(this.TopTask));
			}
			else
			{
				this._taskTopButton.onClick.AddListener(new UnityAction(this.TopTask));
			}
			this._taskTopButton.GetComponent<PointerTrigger>().EnterEvent.AddListener(delegate()
			{
				bool flag2 = this._isBlocked || this._isInFinishedPage;
				if (!flag2)
				{
					this._hover.gameObject.SetActive(true);
					bool activeSelf = this._topIcon.activeSelf;
					if (activeSelf)
					{
						this._topIcon.GetComponent<CImage>().SetSprite("ui9_icon_lowerpopup_notes_icon_top_1", false, null);
					}
					else
					{
						this._topIcon.SetActive(true);
						this._topIcon.GetComponent<CImage>().SetSprite("ui9_icon_lowerpopup_notes_icon_top_0", false, null);
					}
					this._isEnterEvent = true;
				}
			});
			this._taskTopButton.GetComponent<PointerTrigger>().ExitEvent.AddListener(delegate()
			{
				bool flag2 = this._taskData == null || this._isBlocked || this._isInFinishedPage;
				if (!flag2)
				{
					this._hover.gameObject.SetActive(false);
					this._isEnterEvent = false;
					this.SetTopIcon();
				}
			});
		}

		// Token: 0x06005ADF RID: 23263 RVA: 0x002A27CC File Offset: 0x002A09CC
		public void SetData(List<TaskDisplayData> data, List<int> topTaskInfoIDList, ViewTaskPopUpPanel taskPopUpPanel, bool isFinishedPage)
		{
			this._isInFinishedPage = isFinishedPage;
			bool flag = !this._isInFinishedPage;
			if (flag)
			{
				this._hSVStyleRoot.SetDefault();
			}
			this._isBlocked = false;
			this._taskPopUpPanel = taskPopUpPanel;
			this._taskData = data;
			TaskChainItem config = this._taskChainCfg.GetItem(this.ChainId);
			this._thisCImage.SetSprite(this.GetTaskBackground(config.Group, config.RelateAdventure), false, null);
			this._taskIcon.SetSprite(this.GetTaskIcon(config.Group, config.RelateAdventure), false, null);
			switch (config.Group)
			{
			case ETaskChainGroup.MainStory:
				this._taskIcon.GetComponent<TooltipInvoker>().enabled = false;
				goto IL_FA;
			case ETaskChainGroup.OptionalTasks:
				this._taskIcon.GetComponent<TooltipInvoker>().enabled = false;
				goto IL_FA;
			case ETaskChainGroup.SectMainStory:
				this._taskIcon.GetComponent<TooltipInvoker>().enabled = true;
				goto IL_FA;
			case ETaskChainGroup.Count:
				goto IL_FA;
			}
			throw new ArgumentOutOfRangeException();
			IL_FA:
			ArgumentBox argsBox = new ArgumentBox();
			argsBox.Set("arg0", Organization.Instance.GetItem(config.Sect).Name ?? "");
			this._taskIcon.GetComponent<TooltipInvoker>().RuntimeParam = argsBox;
			int finishedDate = -1;
			int maxFinishedDate = -1;
			int maxFinishedIndex = -1;
			this.contentHolderLayoutGroup.enabled = true;
			this.contentHolderLayoutElement.enabled = false;
			this.hideText.gameObject.SetActive(false);
			bool flag2 = taskPopUpPanel == null;
			if (flag2)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					bool flag10 = this.contentHolder.rect.height >= 900f;
					if (flag10)
					{
						this.contentHolderLayoutGroup.enabled = false;
						this.contentHolderLayoutElement.enabled = true;
						this.hideText.gameObject.SetActive(true);
						this.hideText.text = LanguageKey.LK_TaskPopUpPanel_More.Tr().ColorReplace();
					}
				});
			}
			for (int i = 0; i < this._taskData.Count; i++)
			{
				TaskDisplayData item = this._taskData[i];
				bool flag3 = !this._isInFinishedPage;
				if (flag3)
				{
					bool flag4 = item.FinishedDate == -1;
					if (flag4)
					{
						this.GenerateTask(item, topTaskInfoIDList, taskPopUpPanel != null);
					}
				}
				else
				{
					bool flag5 = item.FinishedDate != -1 && item.FinishedDate > maxFinishedDate;
					if (flag5)
					{
						maxFinishedDate = item.FinishedDate;
						maxFinishedIndex = i;
					}
				}
			}
			bool flag6 = this._isInFinishedPage && maxFinishedIndex != -1;
			if (flag6)
			{
				finishedDate = maxFinishedDate;
				this.GenerateTask(this._taskData[maxFinishedIndex], topTaskInfoIDList, taskPopUpPanel != null);
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
			bool flag7 = this._jumpButton != null;
			if (flag7)
			{
				int taskIndex = this._taskData.FindIndex((TaskDisplayData x) => x.TargetLocation.IsValid() && x.FinishedDate == -1);
				bool isShow = taskIndex != -1 && !this._isInFinishedPage && this._taskData[taskIndex].FinishedDate == -1;
				this._jumpButton.interactable = !UIElement.AdventureRemake.IsShowing;
				bool flag8 = this._minStorylineObj;
				if (flag8)
				{
					this._jumpButton.transform.parent.gameObject.SetActive(config.Group == ETaskChainGroup.MainStory || isShow);
					this._minStorylineObj.SetActive(config.Group == ETaskChainGroup.MainStory);
					this._jumpButton.gameObject.SetActive(isShow);
				}
				else
				{
					this._jumpButton.transform.parent.gameObject.SetActive(isShow);
				}
				Action <>9__3;
				this._jumpButton.ClearAndAddListener(delegate
				{
					WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
					bool flag10 = model.GetStateId(this._taskData[taskIndex].TargetLocation.AreaId) != model.CurrentStateId;
					if (flag10)
					{
						UIElement.Bottom.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("KeepAnim", false));
						UIManager.Instance.ShowUI(UIElement.StatePartWorldMap, true);
						YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
						uint frame = 2U;
						Action job;
						if ((job = <>9__3) == null)
						{
							job = (<>9__3 = delegate()
							{
								ViewPartWorldMap.HighlightArea(this._taskData[taskIndex].TargetLocation.AreaId);
							});
						}
						instance.DelayFrameDo(frame, job);
					}
					else
					{
						TaskGroup.<>c__DisplayClass1_1 CS$<>8__locals2 = new TaskGroup.<>c__DisplayClass1_1();
						CS$<>8__locals2.jumpLocation = this._taskData[taskIndex].TargetLocation;
						SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(CS$<>8__locals2.jumpLocation, 0);
						GlobalSettings.OnMapElementDisplayRuleItemStateChanged -= CS$<>8__locals2.<SetData>g__OnMapElementDisplayRuleItemStateChanged|4;
						GlobalSettings.OnMapElementDisplayRuleItemStateChanged += CS$<>8__locals2.<SetData>g__OnMapElementDisplayRuleItemStateChanged|4;
					}
					this._taskPopUpPanel.QuickHide();
				});
			}
			this._finishedDate.gameObject.SetActive(finishedDate != -1);
			bool isInFinishedPage = this._isInFinishedPage;
			if (isInFinishedPage)
			{
				this._hSVStyleRoot.SetDefaultGrayAndBlack();
				int date = finishedDate;
				int year = date / 12;
				int month = date % 12;
				bool flag9 = month < 0;
				if (flag9)
				{
					month += 12;
				}
				this._finishedDate.text = LanguageKey.LK_Game_Time.TrFormat(new object[]
				{
					year + 1,
					month + 1,
					LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(date))),
					Month.Instance[month].Name
				}).ColorReplace();
			}
		}

		// Token: 0x06005AE0 RID: 23264 RVA: 0x002A2C5C File Offset: 0x002A0E5C
		public void SetTopIcon()
		{
			bool flag = this.TaskModel.GetCurrentTopTaskIDList() != null && this.TaskModel.GetCurrentTopTaskIDList().Contains(this._taskData[0].InnerTaskData.TaskInfoId);
			if (flag)
			{
				this._topIcon.SetActive(true);
				this._topIcon.GetComponent<CImage>().SetSprite("ui9_icon_lowerpopup_notes_icon_top_1", false, null);
			}
			else
			{
				bool isEnterEvent = this._isEnterEvent;
				if (isEnterEvent)
				{
					this._topIcon.GetComponent<CImage>().SetSprite("ui9_icon_lowerpopup_notes_icon_top_0", false, null);
					this._topIcon.SetActive(true);
				}
				else
				{
					this._topIcon.SetActive(false);
				}
			}
		}

		// Token: 0x06005AE1 RID: 23265 RVA: 0x002A2D0E File Offset: 0x002A0F0E
		public void CloseTopIcon()
		{
			this._topIcon.SetActive(false);
		}

		// Token: 0x06005AE2 RID: 23266 RVA: 0x002A2D20 File Offset: 0x002A0F20
		public List<TaskDisplayData> GetTaskGroupData()
		{
			return this._taskData;
		}

		// Token: 0x06005AE3 RID: 23267 RVA: 0x002A2D38 File Offset: 0x002A0F38
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

		// Token: 0x06005AE4 RID: 23268 RVA: 0x002A2DB8 File Offset: 0x002A0FB8
		private void GenerateTask(TaskDisplayData data, List<int> topTaskInfoIDList, bool isInTaskPanel = true)
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
			TaskChainItem taskChainConfig = this._taskChainCfg[data.InnerTaskData.TaskChainId];
			string taskChainIconName = (taskChainConfig == null) ? string.Empty : taskChainConfig.TaskChainIcon;
			this.taskChainIcon.SetSprite(taskChainIconName, false, null);
			this._taskInfoList.Add(obj);
			bool isBlocked = data.InnerTaskData.IsBlocked;
			if (isBlocked)
			{
				this._isBlocked = true;
			}
			this._topIcon.SetActive(topTaskInfoIDList != null && topTaskInfoIDList.Contains(taskInfoId) && !this._isInFinishedPage && isInTaskPanel);
			this._taskItem.SetActive(false);
		}

		// Token: 0x06005AE5 RID: 23269 RVA: 0x002A2F6C File Offset: 0x002A116C
		private void TopTask()
		{
			bool flag = this._isBlocked || this._taskPopUpPanel.IsRuntimeTopAnimation() || this._isInFinishedPage;
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

		// Token: 0x06005AE6 RID: 23270 RVA: 0x002A3068 File Offset: 0x002A1268
		private void OpenTaskHinder()
		{
			this._taskHinder.SetActive(true);
			this._verticalLayoutGroup.padding.bottom = 0;
		}

		// Token: 0x06005AE7 RID: 23271 RVA: 0x002A308A File Offset: 0x002A128A
		private void CloseTaskHinder()
		{
			this._taskHinder.SetActive(false);
			this._verticalLayoutGroup.padding.bottom = 0;
		}

		// Token: 0x06005AE8 RID: 23272 RVA: 0x002A30AC File Offset: 0x002A12AC
		private string GetTaskBackground(ETaskChainGroup eTaskChainGroup, bool relateAdventure)
		{
			bool flag = this._taskPopUpPanel == null;
			string result;
			if (flag)
			{
				if (!true)
				{
				}
				string text;
				switch (eTaskChainGroup)
				{
				case ETaskChainGroup.MainStory:
					text = "ui9_back_main_notes_typestrip_0";
					break;
				case ETaskChainGroup.OptionalTasks:
					text = "ui9_back_main_notes_typestrip_1";
					break;
				case ETaskChainGroup.SectMainStory:
					text = "ui9_back_main_notes_typestrip_2";
					break;
				case ETaskChainGroup.CustomizeTasks:
					text = "ui9_back_main_notes_typestrip_3";
					break;
				default:
					throw new ArgumentOutOfRangeException("eTaskChainGroup", eTaskChainGroup, null);
				}
				if (!true)
				{
				}
				result = text;
			}
			else if (relateAdventure)
			{
				result = "ui9_back_lowerpopup_notes_typestrip_4";
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (eTaskChainGroup)
				{
				case ETaskChainGroup.MainStory:
					text = "ui9_back_lowerpopup_notes_typestrip_0";
					break;
				case ETaskChainGroup.OptionalTasks:
					text = "ui9_back_lowerpopup_notes_typestrip_1";
					break;
				case ETaskChainGroup.SectMainStory:
					text = "ui9_back_lowerpopup_notes_typestrip_2";
					break;
				case ETaskChainGroup.CustomizeTasks:
					text = "ui9_back_lowerpopup_notes_typestrip_3";
					break;
				default:
					throw new ArgumentOutOfRangeException("eTaskChainGroup", eTaskChainGroup, null);
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06005AE9 RID: 23273 RVA: 0x002A3188 File Offset: 0x002A1388
		private string GetTaskIcon(ETaskChainGroup eTaskChainGroup, bool relateAdventure)
		{
			string result;
			if (relateAdventure)
			{
				result = "ui9_icon_lowerpopup_notes_icon_type_3";
			}
			else
			{
				if (!true)
				{
				}
				string text;
				switch (eTaskChainGroup)
				{
				case ETaskChainGroup.MainStory:
					text = "ui9_icon_lowerpopup_notes_icon_type_0";
					break;
				case ETaskChainGroup.OptionalTasks:
					text = "ui9_icon_lowerpopup_notes_icon_type_1";
					break;
				case ETaskChainGroup.SectMainStory:
					text = "ui9_icon_lowerpopup_notes_icon_type_2";
					break;
				case ETaskChainGroup.CustomizeTasks:
					text = "ui9_icon_lowerpopup_notes_icon_type_2";
					break;
				case ETaskChainGroup.Count:
					text = "";
					break;
				default:
					throw new ArgumentOutOfRangeException("eTaskChainGroup");
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06005AEA RID: 23274 RVA: 0x002A31FF File Offset: 0x002A13FF
		private void OnEncyclopediaLinkRender(int index, GameObject refers)
		{
		}

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x06005AEB RID: 23275 RVA: 0x002A3202 File Offset: 0x002A1402
		private TaskInfo _taskInfoCfg
		{
			get
			{
				return TaskInfo.Instance;
			}
		}

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x06005AEC RID: 23276 RVA: 0x002A3209 File Offset: 0x002A1409
		private TaskChain _taskChainCfg
		{
			get
			{
				return TaskChain.Instance;
			}
		}

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x06005AED RID: 23277 RVA: 0x002A3210 File Offset: 0x002A1410
		private TaskModel TaskModel
		{
			get
			{
				return SingletonObject.getInstance<TaskModel>();
			}
		}

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x06005AEE RID: 23278 RVA: 0x002A3217 File Offset: 0x002A1417
		private int ChainId
		{
			get
			{
				return (this._taskData.Count > 0) ? this._taskData[0].InnerTaskData.TaskChainId : 0;
			}
		}

		// Token: 0x04003E99 RID: 16025
		[SerializeField]
		private TextMeshProUGUI _titleTMP;

		// Token: 0x04003E9A RID: 16026
		[SerializeField]
		private CImage taskChainIcon;

		// Token: 0x04003E9B RID: 16027
		[SerializeField]
		private GameObject _taskItem;

		// Token: 0x04003E9C RID: 16028
		[SerializeField]
		private VerticalLayoutGroup _verticalLayoutGroup;

		// Token: 0x04003E9D RID: 16029
		[SerializeField]
		private GameObject _taskHinder;

		// Token: 0x04003E9E RID: 16030
		[SerializeField]
		private RectTransform _taskContents;

		// Token: 0x04003E9F RID: 16031
		[SerializeField]
		private CImage _thisCImage;

		// Token: 0x04003EA0 RID: 16032
		[SerializeField]
		private GameObject _topIcon;

		// Token: 0x04003EA1 RID: 16033
		[SerializeField]
		private CImage _taskIcon;

		// Token: 0x04003EA2 RID: 16034
		[SerializeField]
		private CButton _jumpButton;

		// Token: 0x04003EA3 RID: 16035
		[SerializeField]
		private CScrollRect _encyclopediaLinkScroll;

		// Token: 0x04003EA4 RID: 16036
		[SerializeField]
		private CImage _contentBack;

		// Token: 0x04003EA5 RID: 16037
		[SerializeField]
		private CImage _hover;

		// Token: 0x04003EA6 RID: 16038
		[SerializeField]
		private CButton _taskTopButton;

		// Token: 0x04003EA7 RID: 16039
		[SerializeField]
		private HSVStyleRoot _hSVStyleRoot;

		// Token: 0x04003EA8 RID: 16040
		[SerializeField]
		private TextMeshProUGUI _finishedDate;

		// Token: 0x04003EA9 RID: 16041
		[SerializeField]
		private CButton _topButton;

		// Token: 0x04003EAA RID: 16042
		[SerializeField]
		private GameObject _minStorylineObj;

		// Token: 0x04003EAB RID: 16043
		[Header("隐藏部分手记内容提示")]
		[SerializeField]
		private TextMeshProUGUI hideText;

		// Token: 0x04003EAC RID: 16044
		[SerializeField]
		private RectTransform contentHolder;

		// Token: 0x04003EAD RID: 16045
		[SerializeField]
		private VerticalLayoutGroup contentHolderLayoutGroup;

		// Token: 0x04003EAE RID: 16046
		[SerializeField]
		private LayoutElement contentHolderLayoutElement;

		// Token: 0x04003EAF RID: 16047
		private List<TaskDisplayData> _taskData;

		// Token: 0x04003EB0 RID: 16048
		private List<GameObject> _taskInfoList = new List<GameObject>();

		// Token: 0x04003EB1 RID: 16049
		private ViewTaskPopUpPanel _taskPopUpPanel;

		// Token: 0x04003EB2 RID: 16050
		private bool _isBlocked = false;

		// Token: 0x04003EB3 RID: 16051
		private bool _isInFinishedPage = false;

		// Token: 0x04003EB4 RID: 16052
		private bool _isEnterEvent = false;
	}
}
