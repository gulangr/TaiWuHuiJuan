using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.World.Task;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200030C RID: 780
public static class TaskPanelHelper
{
	// Token: 0x1700050C RID: 1292
	// (get) Token: 0x06002E06 RID: 11782 RVA: 0x0016C1C9 File Offset: 0x0016A3C9
	private static TaskInfo TaskInfo
	{
		get
		{
			return TaskInfo.Instance;
		}
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x0016C1D0 File Offset: 0x0016A3D0
	public static string GetTitleString(int displayType, short mapAreaId, TaskDisplayData item, MapAreaData[] areas, string invalid)
	{
		if (!true)
		{
		}
		string result;
		if (displayType <= 16)
		{
			switch (displayType)
			{
			case 1:
				result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview.ColorReplace();
				goto IL_187;
			case 2:
				result = TaskPanelHelper.GetTitleLocationSplicingString(mapAreaId, item.InnerTaskData, areas, invalid);
				goto IL_187;
			case 3:
				break;
			case 4:
				result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview.ColorReplace();
				goto IL_187;
			default:
				if (displayType == 8)
				{
					result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview.GetFormat(item.CountDown).ColorReplace();
					goto IL_187;
				}
				if (displayType == 16)
				{
					result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview.ColorReplace();
					goto IL_187;
				}
				break;
			}
		}
		else if (displayType <= 64)
		{
			if (displayType == 32)
			{
				result = TaskPanelHelper.GetTitleStringArraySplicingString(item);
				goto IL_187;
			}
			if (displayType == 64)
			{
				result = TaskPanelHelper.GetTitleCountDownSplicingString(item);
				goto IL_187;
			}
		}
		else
		{
			if (displayType == 128)
			{
				result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview.ColorReplace();
				goto IL_187;
			}
			if (displayType == 256)
			{
				result = TaskPanelHelper.GetTitleLocationSplicingString(item.TargetLocations[0].AreaId, item.InnerTaskData, areas, invalid);
				goto IL_187;
			}
		}
		result = "";
		IL_187:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x0016C370 File Offset: 0x0016A570
	private static string GetTitleLocationSplicingString(short mapAreaId, TaskData taskData, MapAreaData[] areas, string invalid)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = mapAreaId < 0;
		string result;
		if (flag)
		{
			result = TaskPanelHelper.TaskInfo.GetItem(taskData.TaskInfoId).TaskOverview.GetFormat(invalid).ColorReplace();
		}
		else
		{
			string stateName = MapState.Instance.GetItem(areas[(int)mapAreaId].GetConfig().StateID).Name;
			string areaName = areas[(int)mapAreaId].GetConfig().Name;
			stringBuilder.Append(stateName);
			stringBuilder.Append("·");
			stringBuilder.Append(areaName);
			result = string.Format(TaskPanelHelper.TaskInfo.GetItem(taskData.TaskInfoId).TaskOverview, stringBuilder).ColorReplace();
		}
		return result;
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x0016C420 File Offset: 0x0016A620
	private static string GetTitleCountDownSplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview;
		int month = Mathf.Clamp(item.CountDown - SingletonObject.getInstance<BasicGameData>().CurrDate, 0, 10);
		bool flag = month == 0;
		string result;
		if (flag)
		{
			result = taskContent.GetFormat("6").ColorReplace();
		}
		else
		{
			result = taskContent.GetFormat(month).ColorReplace();
		}
		return result;
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x0016C494 File Offset: 0x0016A694
	private static string GetBaihuaAmbushString(TaskDisplayData item, MapAreaData[] areas, string invalid)
	{
		int needCount = int.Parse(item.StringArray[1]);
		bool flag = needCount == 0;
		string result;
		if (flag)
		{
			string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescriptionMeet;
			string fiveElementTypeStr = LocalStringManager.Get("LK_FiveElements_Type_" + item.StringArray[0]);
			result = taskContent.GetFormat(fiveElementTypeStr, TaskPanelHelper.GetSettlementNameStr(item, areas, invalid)).ColorReplace();
		}
		else
		{
			string taskContent2 = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
			string fiveElementTypeStr2 = LocalStringManager.Get("LK_FiveElements_Type_" + item.StringArray[0]);
			result = taskContent2.GetFormat(fiveElementTypeStr2, TaskPanelHelper.GetSettlementNameStr(item, areas, invalid), item.StringArray[1]).ColorReplace();
		}
		return result;
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x0016C560 File Offset: 0x0016A760
	private static string GetTitleStringArraySplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskOverview;
		string str = taskContent;
		object[] stringArray = item.StringArray;
		return str.GetFormat(stringArray).ColorReplace();
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x0016C5A0 File Offset: 0x0016A7A0
	public static string GetTaskDescription(int displayType, TaskDisplayData item, MapAreaData[] areas, string invalid)
	{
		if (!true)
		{
		}
		string result;
		if (displayType <= 16)
		{
			switch (displayType)
			{
			case 1:
				result = TaskPanelHelper.GetSkillSplicingString(item);
				goto IL_EC;
			case 2:
				result = TaskPanelHelper.GetLocationSplicingString(item, areas, invalid);
				goto IL_EC;
			case 3:
				break;
			case 4:
				result = TaskPanelHelper.GetLifeSkillSplicingString(item);
				goto IL_EC;
			default:
				if (displayType == 8)
				{
					result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription.GetFormat(item.CountDown).ColorReplace();
					goto IL_EC;
				}
				if (displayType == 16)
				{
					result = TaskPanelHelper.GetHeavenlyTreeLocationSplicingString(item);
					goto IL_EC;
				}
				break;
			}
		}
		else if (displayType <= 64)
		{
			if (displayType == 32)
			{
				result = TaskPanelHelper.GetStringArraySplicingString(item);
				goto IL_EC;
			}
			if (displayType == 64)
			{
				result = TaskPanelHelper.GetCountdownSplicingString(item);
				goto IL_EC;
			}
		}
		else
		{
			if (displayType == 128)
			{
				result = TaskPanelHelper.GetBaihuaAmbushString(item, areas, invalid);
				goto IL_EC;
			}
			if (displayType == 256)
			{
				result = TaskPanelHelper.GetSettlementsNameStr(item, areas, invalid);
				goto IL_EC;
			}
		}
		result = "";
		IL_EC:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x0016C6A4 File Offset: 0x0016A8A4
	private static string GetSkillSplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
		bool flag = item.SkillIdList.Items == null || item.SkillIdList.Items.Count <= 0;
		string result;
		if (flag)
		{
			result = taskContent.GetFormat("").ColorReplace();
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < item.SkillIdList.Items.Count; i++)
			{
				stringBuilder.Append(CombatSkill.Instance.GetItem(item.SkillIdList.Items[i]).Name);
				bool flag2 = i != item.SkillIdList.Items.Count - 1;
				if (flag2)
				{
					stringBuilder.Append('、');
				}
			}
			taskContent = taskContent.GetFormat(stringBuilder).ColorReplace();
			result = taskContent;
		}
		return result;
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x0016C7A4 File Offset: 0x0016A9A4
	private static string GetLifeSkillSplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
		bool flag = item.SkillIdList.Items == null || item.SkillIdList.Items.Count <= 0;
		string result;
		if (flag)
		{
			result = taskContent.GetFormat("").ColorReplace();
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < item.SkillIdList.Items.Count; i++)
			{
				stringBuilder.Append(LifeSkill.Instance.GetItem(item.SkillIdList.Items[i]).Name);
				bool flag2 = i != item.SkillIdList.Items.Count - 1;
				if (flag2)
				{
					stringBuilder.Append('、');
				}
			}
			taskContent = taskContent.GetFormat(stringBuilder).ColorReplace();
			result = taskContent;
		}
		return result;
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x0016C8A4 File Offset: 0x0016AAA4
	private static string GetLocationSplicingString(TaskDisplayData item, MapAreaData[] areas, string invalid)
	{
		return string.Format(TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription, TaskPanelHelper.GetSettlementNameStr(item, areas, invalid)).ColorReplace();
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x0016C8E4 File Offset: 0x0016AAE4
	private static string GetSettlementNameStr(TaskDisplayData item, MapAreaData[] areas, string invalid)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = item.TargetLocation.AreaId < 0;
		string result;
		if (flag)
		{
			result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription.GetFormat(invalid).ColorReplace();
		}
		else
		{
			string stateName = MapState.Instance.GetItem(areas[(int)item.TargetLocation.AreaId].GetConfig().StateID).Name;
			string areaName = areas[(int)item.TargetLocation.AreaId].GetConfig().Name;
			stringBuilder.Append(stateName);
			stringBuilder.Append("·");
			stringBuilder.Append(areaName);
			bool flag2 = item.SettlementNameData.MapBlockTemplateId >= 0;
			if (flag2)
			{
				string settlementName = CommonUtils.GetSettlementString(item.SettlementNameData.RandomNameId, item.SettlementNameData.MapBlockTemplateId);
				stringBuilder.Append(" - " + settlementName);
			}
			result = stringBuilder.ToString();
		}
		return result;
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x0016C9E8 File Offset: 0x0016ABE8
	private static string GetSettlementsNameStr(TaskDisplayData item, MapAreaData[] areas, string invalid)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = item.TargetLocations.Count == 0;
		string result;
		if (flag)
		{
			result = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription.GetFormat(invalid).ColorReplace();
		}
		else
		{
			for (int i = 0; i < item.TargetLocations.Count; i++)
			{
				string stateName = MapState.Instance.GetItem(areas[(int)item.TargetLocations[i].AreaId].GetConfig().StateID).Name;
				string areaName = areas[(int)item.TargetLocations[i].AreaId].GetConfig().Name;
				stringBuilder.Append(stateName);
				stringBuilder.Append("·");
				stringBuilder.Append(areaName);
				bool flag2 = item.SettlementNameDatas[i].MapBlockTemplateId >= 0;
				if (flag2)
				{
					string settlementName = CommonUtils.GetSettlementString(item.SettlementNameDatas[i].RandomNameId, item.SettlementNameDatas[i].MapBlockTemplateId);
					stringBuilder.Append(" - " + settlementName);
				}
				bool flag3 = i < item.TargetLocations.Count - 1;
				if (flag3)
				{
					stringBuilder.Append("、");
				}
			}
			result = string.Format(TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription, stringBuilder).ColorReplace();
		}
		return result;
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x0016CB70 File Offset: 0x0016AD70
	private static string GetCountdownSplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
		int month = Mathf.Clamp(item.CountDown - SingletonObject.getInstance<BasicGameData>().CurrDate, 0, 10);
		bool flag = month == 0;
		string result;
		if (flag)
		{
			result = taskContent.GetFormat("6").ColorReplace();
		}
		else
		{
			result = taskContent.GetFormat(month).ColorReplace();
		}
		return result;
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x0016CBE4 File Offset: 0x0016ADE4
	private static string GetHeavenlyTreeLocationSplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
		List<SectStoryHeavenlyTreeExtendable> list = SingletonObject.getInstance<WorldMapModel>().SectWudangHeavenlyTreeList;
		MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
		StringBuilder stringBuilder = new StringBuilder();
		List<SectStoryHeavenlyTreeExtendable> list2;
		if (list == null)
		{
			list2 = null;
		}
		else
		{
			list2 = (from tree in list
			where tree.MetInDream
			select tree).ToList<SectStoryHeavenlyTreeExtendable>();
		}
		List<SectStoryHeavenlyTreeExtendable> metInDream = list2;
		List<short> locationList = new List<short>();
		bool flag = metInDream != null;
		if (flag)
		{
			IEnumerable<SectStoryHeavenlyTreeExtendable> source = metInDream;
			Func<SectStoryHeavenlyTreeExtendable, bool> <>9__1;
			Func<SectStoryHeavenlyTreeExtendable, bool> predicate;
			if ((predicate = <>9__1) == null)
			{
				predicate = (<>9__1 = ((SectStoryHeavenlyTreeExtendable tree) => !locationList.Contains(tree.Location.AreaId)));
			}
			foreach (SectStoryHeavenlyTreeExtendable tree2 in source.Where(predicate))
			{
				locationList.Add(tree2.Location.AreaId);
			}
		}
		bool flag2 = locationList.Count > 0;
		if (flag2)
		{
			for (int i = 0; i < locationList.Count; i++)
			{
				short location = locationList[i];
				string stateName = MapState.Instance.GetItem(areas[(int)location].GetConfig().StateID).Name;
				string areaName = areas[(int)location].GetConfig().Name;
				stringBuilder.Append(stateName);
				stringBuilder.Append("·");
				stringBuilder.Append(areaName);
				bool flag3 = i != locationList.Count - 1;
				if (flag3)
				{
					stringBuilder.Append('、');
				}
			}
		}
		else
		{
			stringBuilder.Append("");
		}
		taskContent = taskContent.GetFormat(stringBuilder).ColorReplace();
		return taskContent;
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x0016CDE8 File Offset: 0x0016AFE8
	private static string GetStringArraySplicingString(TaskDisplayData item)
	{
		string taskContent = TaskPanelHelper.TaskInfo.GetItem(item.InnerTaskData.TaskInfoId).TaskDescription;
		string str = taskContent;
		object[] stringArray = item.StringArray;
		return str.GetFormat(stringArray).ColorReplace();
	}
}
