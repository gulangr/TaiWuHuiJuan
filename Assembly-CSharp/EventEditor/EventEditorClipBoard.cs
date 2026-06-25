using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000623 RID: 1571
	public static class EventEditorClipBoard
	{
		// Token: 0x06004A64 RID: 19044 RVA: 0x0022D590 File Offset: 0x0022B790
		public static EventEditorData PasteEvent(EventEditorData targetData)
		{
			EventEditorClipBoard.<>c__DisplayClass3_0 CS$<>8__locals1 = new EventEditorClipBoard.<>c__DisplayClass3_0();
			CS$<>8__locals1.targetData = targetData;
			bool flag = EventEditorClipBoard.CopiedEvent == null;
			EventEditorData targetData2;
			if (flag)
			{
				targetData2 = CS$<>8__locals1.targetData;
			}
			else
			{
				if (CS$<>8__locals1.targetData == null)
				{
					CS$<>8__locals1.targetData = new EventEditorData();
				}
				bool flag2 = EventEditorClipBoard.CopiedEvent.EventOrder >= 0;
				if (flag2)
				{
					CS$<>8__locals1.targetData.EventOrder = EventEditorClipBoard.CopiedEvent.EventOrder;
				}
				bool flag3 = !string.IsNullOrEmpty(EventEditorClipBoard.CopiedEvent.EventType);
				if (flag3)
				{
					CS$<>8__locals1.targetData.EventType = EventEditorClipBoard.CopiedEvent.EventType;
				}
				bool flag4 = !string.IsNullOrEmpty(EventEditorClipBoard.CopiedEvent.EventGroup) && string.IsNullOrEmpty(CS$<>8__locals1.targetData.EventGroup);
				if (flag4)
				{
					CS$<>8__locals1.targetData.EventGroup = EventEditorClipBoard.CopiedEvent.EventGroup;
				}
				CS$<>8__locals1.targetData.ForceSingle = EventEditorClipBoard.CopiedEvent.ForceSingle;
				bool flag5 = !EventEditorClipBoard.CopiedEvent.TriggerType.IsNullOrEmpty();
				if (flag5)
				{
					CS$<>8__locals1.targetData.TriggerType = EventEditorClipBoard.CopiedEvent.TriggerType;
				}
				bool flag6 = !EventEditorClipBoard.CopiedEvent.DecideRole.IsNullOrEmpty();
				if (flag6)
				{
					CS$<>8__locals1.targetData.DecideRole = EventEditorClipBoard.CopiedEvent.DecideRole;
				}
				bool flag7 = !EventEditorClipBoard.CopiedEvent.TargetRole.IsNullOrEmpty();
				if (flag7)
				{
					CS$<>8__locals1.targetData.TargetRole = EventEditorClipBoard.CopiedEvent.TargetRole;
				}
				CS$<>8__locals1.targetData.InternalTexture = EventEditorClipBoard.CopiedEvent.InternalTexture;
				bool flag8 = !EventEditorClipBoard.CopiedEvent.TexturePath.IsNullOrEmpty();
				if (flag8)
				{
					CS$<>8__locals1.targetData.TexturePath = EventEditorClipBoard.CopiedEvent.TexturePath;
				}
				bool flag9 = !EventEditorClipBoard.CopiedEvent.EventTexture.IsNullOrEmpty();
				if (flag9)
				{
					CS$<>8__locals1.targetData.EventTexture = EventEditorClipBoard.CopiedEvent.EventTexture;
				}
				CS$<>8__locals1.targetData.ControlMask = EventEditorClipBoard.CopiedEvent.ControlMask;
				CS$<>8__locals1.targetData.ControlMaskCode = EventEditorClipBoard.CopiedEvent.ControlMaskCode;
				CS$<>8__locals1.targetData.MaskTweenTime = EventEditorClipBoard.CopiedEvent.MaskTweenTime;
				Dictionary<int, EventEditorData.Option> srcOptions = EventEditorClipBoard.CopiedEvent.Options;
				Dictionary<int, EventEditorData.Option> targetOptions = CS$<>8__locals1.targetData.Options;
				bool flag10 = targetOptions == null;
				if (flag10)
				{
					targetOptions = new Dictionary<int, EventEditorData.Option>();
					CS$<>8__locals1.targetData.Options = targetOptions;
				}
				for (int i = targetOptions.Count + 1; i <= srcOptions.Count; i++)
				{
					SingletonObject.getInstance<EventEditorModel>().EventAddNewOption(CS$<>8__locals1.targetData);
				}
				foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in srcOptions)
				{
					int num;
					EventEditorData.Option option;
					keyValuePair.Deconstruct(out num, out option);
					int j = num;
					EventEditorData.Option srcOption = option;
					EventEditorClipBoard.EventOptionCopyData optionCopyData = EventEditorClipBoard.GetOptionCopyData(EventEditorClipBoard.CopiedEvent, j, false);
					EventEditorClipBoard.PasteOptionInternal(srcOption, optionCopyData, CS$<>8__locals1.targetData, j);
				}
				CS$<>8__locals1.targetSaveDir = Save.GetEventSaveDir(CS$<>8__locals1.targetData);
				foreach (int k in targetOptions.Keys.ToArray<int>())
				{
					EventEditorData.Option table = targetOptions[k];
					string guid = table.Guid;
					string filePath = Path.Combine(CS$<>8__locals1.targetSaveDir, guid + ".cs");
					bool flag11 = File.Exists(filePath);
					if (flag11)
					{
						File.Delete(filePath);
					}
					targetOptions.Remove(k);
				}
				CS$<>8__locals1.targetGuid = CS$<>8__locals1.targetData.EventGuid;
				CS$<>8__locals1.srcGuid = EventEditorClipBoard.CopiedEvent.EventGuid;
				CS$<>8__locals1.srcSaveDir = Save.GetEventSaveDir(EventEditorClipBoard.CopiedEvent);
				string srcEventCodePath = Path.Combine(CS$<>8__locals1.srcSaveDir, CS$<>8__locals1.srcGuid + ".cs");
				bool flag12 = File.Exists(srcEventCodePath);
				if (flag12)
				{
					string[] eventScriptLines = File.ReadAllLines(srcEventCodePath);
					bool flag13 = !Directory.Exists(CS$<>8__locals1.targetSaveDir);
					if (flag13)
					{
						Directory.CreateDirectory(CS$<>8__locals1.targetSaveDir);
					}
					string tarEventScriptFilePath = Path.Combine(CS$<>8__locals1.targetSaveDir, CS$<>8__locals1.targetGuid + ".cs");
					Regex regex = new Regex("\\$\\{(?<Key>[a-z|A-Z][a-z|A-Z|0-9]+)\\}");
					string eventScriptTemplateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventAPITemplate.template");
					string[] templateScriptLines = File.ReadAllLines(eventScriptTemplateFilePath);
					eventScriptLines[0] = regex.Replace(templateScriptLines[0], (Match match) => Export.GetScriptReplaceString(match.Groups["Key"].Value, CS$<>8__locals1.targetData));
					File.WriteAllLines(tarEventScriptFilePath, eventScriptLines);
				}
				CS$<>8__locals1.<PasteEvent>g__CopyFile|0(".tws");
				CS$<>8__locals1.<PasteEvent>g__CopyFile|0("_condition.tws");
				CS$<>8__locals1.<PasteEvent>g__CopyFile|0("_boolState.twe");
				CS$<>8__locals1.targetData.Dirty = true;
				CS$<>8__locals1.targetData.TmEdit = Save.GetTimeStamp();
				targetData2 = CS$<>8__locals1.targetData;
			}
			return targetData2;
		}

		// Token: 0x06004A65 RID: 19045 RVA: 0x0022DA5C File Offset: 0x0022BC5C
		public static EventEditorClipBoard.EventOptionCopyData GetOptionCopyData(EventEditorData eventData, int optionIndex, bool copy = true)
		{
			EventEditorClipBoard.EventOptionCopyData data = new EventEditorClipBoard.EventOptionCopyData
			{
				SrcEventGuid = eventData.EventGuid,
				OptionTable = eventData.Options[optionIndex]
			};
			string saveDirectory = Save.GetEventSaveDir(eventData);
			bool flag = !string.IsNullOrEmpty(saveDirectory);
			if (flag)
			{
				string optionGuid = data.OptionTable.Guid;
				string optionCodeFilePath = Path.Combine(saveDirectory, optionGuid + ".cs").PathFix();
				bool flag2 = File.Exists(optionCodeFilePath);
				if (flag2)
				{
					data.OptionCode = File.ReadAllText(optionCodeFilePath);
				}
				string optionScriptFilePath = Path.Combine(saveDirectory, optionGuid + ".tws").PathFix();
				bool flag3 = File.Exists(optionScriptFilePath);
				if (flag3)
				{
					data.OptionScript = File.ReadAllText(optionScriptFilePath);
				}
				string optionAvailableConditionPath = Path.Combine(saveDirectory, optionGuid + "_available.tws").PathFix();
				bool flag4 = File.Exists(optionAvailableConditionPath);
				if (flag4)
				{
					data.OptionAvailableConditionList = File.ReadAllText(optionAvailableConditionPath);
				}
				string optionVisibleConditionPath = Path.Combine(saveDirectory, optionGuid + "_visible.tws").PathFix();
				bool flag5 = File.Exists(optionVisibleConditionPath);
				if (flag5)
				{
					data.OptionVisibleConditionList = File.ReadAllText(optionVisibleConditionPath);
				}
				string optionConsumptionPath = Path.Combine(saveDirectory, optionGuid + "_cost.twe").PathFix();
				bool flag6 = File.Exists(optionConsumptionPath);
				if (flag6)
				{
					data.OptionConsumptions = File.ReadAllText(optionConsumptionPath);
				}
			}
			if (copy)
			{
				EventEditorClipBoard.CopiedOptionData = data;
			}
			return data;
		}

		// Token: 0x06004A66 RID: 19046 RVA: 0x0022DBC4 File Offset: 0x0022BDC4
		public static void PasteAllEventOptionsAsRedirect(EventEditorData eventData)
		{
			foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in EventEditorClipBoard.CopiedEvent.Options)
			{
				int num;
				EventEditorData.Option option2;
				keyValuePair.Deconstruct(out num, out option2);
				EventEditorData.Option option = option2;
				SingletonObject.getInstance<EventEditorModel>().EventAddNewOption(eventData);
				EventEditorClipBoard.RedirectToCopiedOptionInternal(EventEditorClipBoard.CopiedEvent.EventGuid, option, eventData, eventData.Options.Count);
			}
		}

		// Token: 0x06004A67 RID: 19047 RVA: 0x0022DC54 File Offset: 0x0022BE54
		public static void RedirectToCopiedOption(EventEditorData eventData, int optionIndex)
		{
			EventEditorClipBoard.RedirectToCopiedOptionInternal(EventEditorClipBoard.CopiedOptionData.SrcEventGuid, EventEditorClipBoard.CopiedOptionData.OptionTable, eventData, optionIndex);
		}

		// Token: 0x06004A68 RID: 19048 RVA: 0x0022DC74 File Offset: 0x0022BE74
		private static void RedirectToCopiedOptionInternal(string srcEventGuid, EventEditorData.Option srcOption, EventEditorData targetEventData, int targetOptionIndex)
		{
			bool flag = srcEventGuid.IsNullOrEmpty() || srcOption == null;
			if (!flag)
			{
				Dictionary<int, EventEditorData.Option> targetEventOptions = targetEventData.Options;
				EventEditorData.Option option;
				EventEditorData.Option targetOption = targetEventOptions.TryGetValue(targetOptionIndex, out option) ? option : new EventEditorData.Option();
				targetOption.RedirectTargetOption = new ValueTuple<string, string>(srcEventGuid, srcOption.Guid);
				targetEventOptions[targetOptionIndex] = targetOption;
				bool flag2 = string.IsNullOrEmpty(targetOption.Content) && !string.IsNullOrEmpty(srcOption.Content);
				if (flag2)
				{
					targetOption.InternalContent = srcOption.Content;
				}
			}
		}

		// Token: 0x06004A69 RID: 19049 RVA: 0x0022DD00 File Offset: 0x0022BF00
		public static EventEditorData.Option PasteOption(EventEditorData eventData, int optionIndex)
		{
			return EventEditorClipBoard.PasteOptionInternal(EventEditorClipBoard.CopiedOptionData.OptionTable, EventEditorClipBoard.CopiedOptionData, eventData, optionIndex);
		}

		// Token: 0x06004A6A RID: 19050 RVA: 0x0022DD28 File Offset: 0x0022BF28
		private static EventEditorData.Option PasteOptionInternal(EventEditorData.Option srcOption, EventEditorClipBoard.EventOptionCopyData copiedOptionData, EventEditorData targetEventData, int targetOptionIndex)
		{
			Dictionary<int, EventEditorData.Option> targetEventOptions = targetEventData.Options;
			EventEditorData.Option option;
			EventEditorData.Option targetOption = targetEventOptions.TryGetValue(targetOptionIndex, out option) ? option : new EventEditorData.Option();
			bool flag = srcOption == null;
			EventEditorData.Option result;
			if (flag)
			{
				result = targetOption;
			}
			else
			{
				EventEditorClipBoard.<>c__DisplayClass9_0 CS$<>8__locals1;
				CS$<>8__locals1.srcOptionGuid = srcOption.Guid;
				CS$<>8__locals1.targetOptionGuid = targetOption.Guid;
				string targetOptionKey = targetOption.OptionKey;
				targetOption.Guid = srcOption.Guid;
				targetOption.OptionKey = srcOption.OptionKey;
				targetOption.OneTimeOnly = srcOption.OneTimeOnly;
				targetOption.Important = srcOption.Important;
				targetOption.Behavior = srcOption.Behavior;
				targetOption.DefaultState = srcOption.DefaultState;
				targetOption.InternalContent = srcOption.Content;
				targetOption.Guid = CS$<>8__locals1.targetOptionGuid;
				targetOption.OptionKey = targetOptionKey;
				targetEventOptions[targetOptionIndex] = targetOption;
				CS$<>8__locals1.directory = Save.GetEventSaveDir(targetEventData);
				bool flag2 = !string.IsNullOrEmpty(copiedOptionData.OptionCode);
				if (flag2)
				{
					string optionFilePath = Path.Combine(CS$<>8__locals1.directory, CS$<>8__locals1.targetOptionGuid + ".cs");
					bool flag3 = !File.Exists(optionFilePath);
					if (flag3)
					{
						SingletonObject.getInstance<EventEditorModel>().GenerateOptionCodeFile(targetEventData, CS$<>8__locals1.targetOptionGuid);
					}
					string targetOptionScriptContent = copiedOptionData.OptionCode.Replace(CS$<>8__locals1.srcOptionGuid, CS$<>8__locals1.targetOptionGuid);
					File.WriteAllText(optionFilePath, targetOptionScriptContent);
				}
				EventEditorClipBoard.<PasteOptionInternal>g__WriteFile|9_0(copiedOptionData.OptionScript, ".tws", ref CS$<>8__locals1);
				EventEditorClipBoard.<PasteOptionInternal>g__WriteFile|9_0(copiedOptionData.OptionVisibleConditionList, "_visible.tws", ref CS$<>8__locals1);
				EventEditorClipBoard.<PasteOptionInternal>g__WriteFile|9_0(copiedOptionData.OptionAvailableConditionList, "_available.tws", ref CS$<>8__locals1);
				EventEditorClipBoard.<PasteOptionInternal>g__WriteFile|9_0(copiedOptionData.OptionConsumptions, "_cost.twe", ref CS$<>8__locals1);
				result = targetOption;
			}
			return result;
		}

		// Token: 0x06004A6B RID: 19051 RVA: 0x0022DED4 File Offset: 0x0022C0D4
		[CompilerGenerated]
		internal static void <PasteOptionInternal>g__WriteFile|9_0(string content, string pathPostfix, ref EventEditorClipBoard.<>c__DisplayClass9_0 A_2)
		{
			bool flag = string.IsNullOrWhiteSpace(content);
			if (!flag)
			{
				bool flag2 = !Directory.Exists(A_2.directory);
				if (flag2)
				{
					Directory.CreateDirectory(A_2.directory);
				}
				string path = Path.Combine(A_2.directory, A_2.targetOptionGuid + pathPostfix);
				File.WriteAllText(path, content.Replace(A_2.srcOptionGuid, A_2.targetOptionGuid));
			}
		}

		// Token: 0x04003388 RID: 13192
		public static EventEditorData CopiedEvent;

		// Token: 0x04003389 RID: 13193
		public static EventEditorClipBoard.EventOptionCopyData CopiedOptionData;

		// Token: 0x020019FD RID: 6653
		public class EventOptionCopyData
		{
			// Token: 0x0400B47F RID: 46207
			public EventEditorData.Option OptionTable;

			// Token: 0x0400B480 RID: 46208
			public string SrcEventGuid;

			// Token: 0x0400B481 RID: 46209
			public string OptionCode;

			// Token: 0x0400B482 RID: 46210
			public string OptionScript;

			// Token: 0x0400B483 RID: 46211
			public string OptionAvailableConditionList;

			// Token: 0x0400B484 RID: 46212
			public string OptionVisibleConditionList;

			// Token: 0x0400B485 RID: 46213
			public string OptionConsumptions;
		}
	}
}
