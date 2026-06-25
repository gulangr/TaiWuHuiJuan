using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Adventure;
using GameData.Domains.Map;
using GameData.Domains.World.Task;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C45 RID: 3141
	public class ViewTaskPanelMain2 : UIBase
	{
		// Token: 0x06009F9A RID: 40858 RVA: 0x004A914B File Offset: 0x004A734B
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06009F9B RID: 40859 RVA: 0x004A914E File Offset: 0x004A734E
		private void Awake()
		{
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		}

		// Token: 0x06009F9C RID: 40860 RVA: 0x004A9189 File Offset: 0x004A7389
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
		}

		// Token: 0x06009F9D RID: 40861 RVA: 0x004A91C4 File Offset: 0x004A73C4
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopTask, new GEvent.Callback(this.TopTaskCallBack));
			GEvent.Add(UiEvents.TaskGroupDataUpdated, new GEvent.Callback(this.TaskGroupDataUpdatedCallBack));
			GEvent.Add(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Add(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
			this.RefreshTitleLayout();
		}

		// Token: 0x06009F9E RID: 40862 RVA: 0x004A9244 File Offset: 0x004A7444
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopTask, new GEvent.Callback(this.TopTaskCallBack));
			GEvent.Remove(UiEvents.TaskGroupDataUpdated, new GEvent.Callback(this.TaskGroupDataUpdatedCallBack));
			GEvent.Remove(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Remove(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
		}

		// Token: 0x06009F9F RID: 40863 RVA: 0x004A92BC File Offset: 0x004A74BC
		private void TopTaskCallBack(ArgumentBox box)
		{
			this.RefreshTitleLayout();
		}

		// Token: 0x06009FA0 RID: 40864 RVA: 0x004A92C6 File Offset: 0x004A74C6
		private void TaskGroupDataUpdatedCallBack(ArgumentBox box)
		{
			this.RefreshTitleLayout();
		}

		// Token: 0x06009FA1 RID: 40865 RVA: 0x004A92D0 File Offset: 0x004A74D0
		private void RefreshTitleLayout()
		{
			Dictionary<ETaskChainGroup, TaskGroupData> groupDict = EasyPool.Get<Dictionary<ETaskChainGroup, TaskGroupData>>();
			AdventureTaiwu adventureTaiwu = this.AdventureModel.AdventureTaiwu;
			bool inAdventure = adventureTaiwu != null && adventureTaiwu.InAdventure;
			foreach (KeyValuePair<int, TaskGroupData> keyValuePair in ViewTaskPanelMain2.TaskData)
			{
				int num;
				TaskGroupData taskGroupData;
				keyValuePair.Deconstruct(out num, out taskGroupData);
				TaskGroupData groupData = taskGroupData;
				bool flag = groupData.dataList.Count == 0;
				if (!flag)
				{
					bool flag2 = !groupData.dataList.Any((TaskDisplayData v) => v.FinishedDate == -1 && v.InnerTaskData.TaskStatus == 0);
					if (!flag2)
					{
						int realTaskChainId = groupData.dataList[0].InnerTaskData.TaskChainId;
						TaskChainItem chainConfig = TaskChain.Instance.GetItem(realTaskChainId);
						bool flag3 = chainConfig == null;
						if (!flag3)
						{
							bool flag4 = inAdventure && !chainConfig.RelateAdventure;
							if (!flag4)
							{
								bool flag5 = !inAdventure && chainConfig.RelateAdventure;
								if (!flag5)
								{
									bool flag6 = !groupDict.ContainsKey(chainConfig.Group);
									if (flag6)
									{
										groupDict.Add(chainConfig.Group, groupData);
									}
								}
							}
						}
					}
				}
			}
			CommonUtils.PrepareEnoughChildren(this.titleLayout, this.titleTemplate.gameObject, groupDict.Count, null);
			MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
			string invalid = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid);
			int childIndex = 0;
			for (ETaskChainGroup groupType = ETaskChainGroup.MainStory; groupType < ETaskChainGroup.Count; groupType++)
			{
				TaskGroupData groupData2;
				bool flag7 = !groupDict.TryGetValue(groupType, out groupData2);
				if (!flag7)
				{
					Transform child = this.titleLayout.GetChild(childIndex);
					TaskPanelMain2Item refers = child.GetComponent<TaskPanelMain2Item>();
					TextMeshProUGUI titleLabel = refers.text;
					CImage bg = refers.bg;
					CButton button = refers.GetComponent<CButton>();
					PointerTrigger pointerTrigger = refers.GetComponent<PointerTrigger>();
					CImage icon = refers.icon;
					TaskDisplayData displayData = groupData2.dataList[0];
					TaskData taskData = displayData.InnerTaskData;
					TaskInfoItem taskInfo = TaskInfo.Instance.GetItem(taskData.TaskInfoId);
					titleLabel.text = ((displayData.DisplayType != 0) ? TaskPanelHelper.GetTitleString(displayData.DisplayType, displayData.TargetLocation.AreaId, displayData, areas, invalid) : taskInfo.TaskOverview.ColorReplace());
					bg.SetSprite(ViewTaskPanelMain2.GetTaskBackground(groupType), false, null);
					icon.SetSprite(ViewTaskPanelMain2.GetTaskIcon(groupType), false, null);
					button.ClearAndAddListener(delegate
					{
						bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
						if (!inGuiding)
						{
							ArgumentBox args = EasyPool.Get<ArgumentBox>();
							args.Set("TaskInfoId", taskData.TaskInfoId);
							UIElement.TaskPopPanel.SetOnInitArgs(args);
							UIManager.Instance.MaskUI(UIElement.TaskPopPanel);
						}
					});
					TaskGroupData currentGroupData = groupData2;
					ETaskChainGroup type = groupType;
					pointerTrigger.EnterEvent.RemoveAllListeners();
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						ViewTaskPanelMain2.ShowGroupDetail(child.GetComponent<RectTransform>(), currentGroupData);
						bg.SetSprite(ViewTaskPanelMain2.GetTaskBackgroundHover(type), false, null);
					});
					pointerTrigger.ExitEvent.RemoveAllListeners();
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						ViewTaskPanelMain2.HideGroupDetail();
						bg.SetSprite(ViewTaskPanelMain2.GetTaskBackground(type), false, null);
					});
					childIndex++;
				}
			}
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			worldMapModel.ClearAllTemporaryMarkListForTask();
			List<Location> targetLocations = new List<Location>();
			foreach (TaskGroupData groupData3 in groupDict.Values)
			{
				int taskIndex = groupData3.dataList.FindIndex((TaskDisplayData x) => x.TargetLocation.IsValid() && x.FinishedDate == -1);
				bool flag8 = taskIndex != -1;
				if (flag8)
				{
					Location location = groupData3.dataList[taskIndex].TargetLocation;
					bool flag9 = worldMapModel.IsAreaInCurrentState(location.AreaId);
					if (flag9)
					{
						targetLocations.Add(location);
					}
				}
			}
			bool flag10 = targetLocations.Count > 0;
			if (flag10)
			{
				worldMapModel.AddLocationsToTemporaryMarkListForTask(targetLocations);
			}
			EasyPool.Free<Dictionary<ETaskChainGroup, TaskGroupData>>(groupDict);
		}

		// Token: 0x06009FA2 RID: 40866 RVA: 0x004A9724 File Offset: 0x004A7924
		private static void ShowGroupDetail(RectTransform titleRect, TaskGroupData groupData)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("Anchor", titleRect);
			argBox.SetObject("GroupData", groupData);
			UIElement.TaskPanelTopGroup.SetOnInitArgs(argBox);
			UIElement.TaskPanelTopGroup.Show();
		}

		// Token: 0x06009FA3 RID: 40867 RVA: 0x004A976E File Offset: 0x004A796E
		private static void HideGroupDetail()
		{
			UIElement.TaskPanelTopGroup.Hide(false);
		}

		// Token: 0x06009FA4 RID: 40868 RVA: 0x004A9780 File Offset: 0x004A7980
		private static string GetTaskBackground(ETaskChainGroup eTaskChainGroup)
		{
			return "ui9_back_main_notes_typestrip_{0}_0".GetFormat((int)eTaskChainGroup);
		}

		// Token: 0x06009FA5 RID: 40869 RVA: 0x004A97A4 File Offset: 0x004A79A4
		private static string GetTaskBackgroundHover(ETaskChainGroup eTaskChainGroup)
		{
			return "ui9_back_main_notes_typestrip_{0}_1".GetFormat((int)eTaskChainGroup);
		}

		// Token: 0x06009FA6 RID: 40870 RVA: 0x004A97C8 File Offset: 0x004A79C8
		private static string GetTaskIcon(ETaskChainGroup eTaskChainGroup)
		{
			string str = "ui9_icon_lowerpopup_notes_icon_type_";
			int num = (int)eTaskChainGroup;
			return str + num.ToString();
		}

		// Token: 0x170010D5 RID: 4309
		// (get) Token: 0x06009FA7 RID: 40871 RVA: 0x004A97ED File Offset: 0x004A79ED
		private static Dictionary<int, TaskGroupData> TaskData
		{
			get
			{
				return ViewTaskPanelMain2.TaskModel.GetCurrentTaskGroupData();
			}
		}

		// Token: 0x170010D6 RID: 4310
		// (get) Token: 0x06009FA8 RID: 40872 RVA: 0x004A97F9 File Offset: 0x004A79F9
		private static TaskModel TaskModel
		{
			get
			{
				return SingletonObject.getInstance<TaskModel>();
			}
		}

		// Token: 0x170010D7 RID: 4311
		// (get) Token: 0x06009FA9 RID: 40873 RVA: 0x004A9800 File Offset: 0x004A7A00
		private AdventureRemakeModel AdventureModel
		{
			get
			{
				return SingletonObject.getInstance<AdventureRemakeModel>();
			}
		}

		// Token: 0x06009FAA RID: 40874 RVA: 0x004A9807 File Offset: 0x004A7A07
		private void CancelPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x06009FAB RID: 40875 RVA: 0x004A9817 File Offset: 0x004A7A17
		private void StartPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06009FAC RID: 40876 RVA: 0x004A9827 File Offset: 0x004A7A27
		private void PlayAnimToHideMainUI(ArgumentBox argBox)
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06009FAD RID: 40877 RVA: 0x004A9837 File Offset: 0x004A7A37
		private void PlayAnimToShowMainUI(ArgumentBox argBox)
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x04007B8E RID: 31630
		[SerializeField]
		private RectTransform titleLayout;

		// Token: 0x04007B8F RID: 31631
		[SerializeField]
		private TaskPanelMain2Item titleTemplate;
	}
}
