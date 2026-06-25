using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Config;
using FrameWork;
using GameData.Domains.TaiwuEvent.EventOption;
using Newtonsoft.Json;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200062F RID: 1583
	public class Export
	{
		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x06004AF4 RID: 19188 RVA: 0x00232165 File Offset: 0x00230365
		// (set) Token: 0x06004AF5 RID: 19189 RVA: 0x0023216C File Offset: 0x0023036C
		public static string ExportingEventGroup { get; private set; }

		// Token: 0x06004AF6 RID: 19190 RVA: 0x00232174 File Offset: 0x00230374
		public static Dictionary<string, List<string>> GetDefaultEventFunctionInfos()
		{
			bool flag = Export._eventCodeDefaultFunctionInfos != null;
			Dictionary<string, List<string>> eventCodeDefaultFunctionInfos;
			if (flag)
			{
				eventCodeDefaultFunctionInfos = Export._eventCodeDefaultFunctionInfos;
			}
			else
			{
				string text = SingletonObject.getInstance<EventEditorModel>().GetEventCodeTemplateText();
				string[] lines = Regex.Split(text, "\r\n|\r|\n");
				Export._eventCodeDefaultFunctionInfos = Export.GetFunctionInfos(lines);
				eventCodeDefaultFunctionInfos = Export._eventCodeDefaultFunctionInfos;
			}
			return eventCodeDefaultFunctionInfos;
		}

		// Token: 0x06004AF7 RID: 19191 RVA: 0x002321C4 File Offset: 0x002303C4
		public static Dictionary<string, List<string>> GetFileFunctionInfos(string filePath)
		{
			bool flag = !File.Exists(filePath);
			Dictionary<string, List<string>> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string[] lines = File.ReadAllLines(filePath);
				result = Export.GetFunctionInfos(lines);
			}
			return result;
		}

		// Token: 0x06004AF8 RID: 19192 RVA: 0x002321F4 File Offset: 0x002303F4
		public static Dictionary<string, List<string>> GetFunctionInfos(string[] lines)
		{
			Dictionary<string, List<string>> functionsDictionary = new Dictionary<string, List<string>>();
			sbyte leftBraceCount = 0;
			Regex regex = new Regex("{|}");
			Regex functionNameRegex = new Regex("([\\w]+)\\((.*)\\)");
			string prevLine = string.Empty;
			List<string> functionLineList = new List<string>();
			string handlingFunctionName = string.Empty;
			bool isRegionArea = false;
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].TrimEnd();
				bool flag = line.Trim().StartsWith("/") || string.IsNullOrEmpty(line);
				if (!flag)
				{
					bool flag2 = line.StartsWith("#region");
					if (flag2)
					{
						handlingFunctionName = line.Replace("#region", "").Trim();
						isRegionArea = true;
					}
					else
					{
						bool flag3 = line.StartsWith("#endregion");
						if (flag3)
						{
							bool flag4 = functionLineList.Count > 0;
							if (flag4)
							{
								functionsDictionary.Add(handlingFunctionName, functionLineList);
							}
							isRegionArea = false;
							functionLineList = new List<string>();
							handlingFunctionName = string.Empty;
						}
						else
						{
							bool flag5 = !string.IsNullOrEmpty(handlingFunctionName);
							if (flag5)
							{
								functionLineList.Add(line);
							}
							bool flag6 = !isRegionArea;
							if (flag6)
							{
								MatchCollection matchCollection = regex.Matches(line);
								foreach (object obj in matchCollection)
								{
									Match match = (Match)obj;
									bool flag7 = match.Value == "{";
									if (flag7)
									{
										bool flag8 = leftBraceCount <= 0;
										if (flag8)
										{
											string functionNameLine = line;
											bool flag9 = line.Trim().Length <= 1;
											if (flag9)
											{
												functionNameLine = prevLine;
											}
											bool flag10 = !string.IsNullOrEmpty(functionNameLine);
											if (flag10)
											{
												functionLineList.Add(functionNameLine);
												Match functionNameMatch = functionNameRegex.Match(functionNameLine);
												handlingFunctionName = functionNameMatch.Groups[1].Value;
												bool flag11 = string.IsNullOrEmpty(handlingFunctionName);
												if (flag11)
												{
													functionLineList.Clear();
													continue;
												}
											}
											functionLineList.Add(line);
										}
										bool flag12 = !string.IsNullOrEmpty(handlingFunctionName);
										if (flag12)
										{
											leftBraceCount += 1;
										}
									}
									else
									{
										bool flag13 = match.Value == "}";
										if (flag13)
										{
											bool flag14 = !string.IsNullOrEmpty(handlingFunctionName);
											if (flag14)
											{
												leftBraceCount -= 1;
											}
											bool flag15 = leftBraceCount == 0 && !string.IsNullOrEmpty(handlingFunctionName) && functionLineList.Count >= 3;
											if (flag15)
											{
												functionsDictionary.Add(handlingFunctionName, functionLineList);
												functionLineList = new List<string>();
												handlingFunctionName = string.Empty;
											}
										}
									}
								}
							}
							prevLine = line;
						}
					}
				}
			}
			bool flag16 = leftBraceCount != 0;
			if (flag16)
			{
				throw new Exception("Invalid code file format.");
			}
			return functionsDictionary;
		}

		// Token: 0x06004AF9 RID: 19193 RVA: 0x002324E8 File Offset: 0x002306E8
		private static void AppendBuilderLineWithTab(StringBuilder builder, byte tabCount, string line)
		{
			for (int i = 0; i < (int)tabCount; i++)
			{
				builder.Append("\t");
			}
			builder.AppendLine(line);
		}

		// Token: 0x06004AFA RID: 19194 RVA: 0x0023251C File Offset: 0x0023071C
		private static string GetEventOptionCtorString(EventEditorData eventData)
		{
			Dictionary<int, string> eventOptionBehaviorToString = new Dictionary<int, string>
			{
				{
					0,
					"None"
				},
				{
					1,
					"BehaviorJust"
				},
				{
					2,
					"BehaviorKind"
				},
				{
					3,
					"BehaviorEven"
				},
				{
					4,
					"BehaviorRebel"
				},
				{
					5,
					"BehaviorEgoistic"
				}
			};
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<int, EventEditorData.Option> options = eventData.Options;
			foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in options)
			{
				int num;
				EventEditorData.Option option2;
				keyValuePair.Deconstruct(out num, out option2);
				EventEditorData.Option option = option2;
				stringBuilder.Append("new TaiwuEventOption{");
				stringBuilder.Append(" OptionKey = \"" + option.OptionKey + "\",");
				stringBuilder.Append(" OptionGuid = \"" + option.Guid + "\"");
				bool flag = option.Behavior >= 0;
				if (flag)
				{
					int behaviorIndex = option.Behavior;
					string behaviorString;
					bool flag2 = behaviorIndex != 0 && eventOptionBehaviorToString.TryGetValue(behaviorIndex, out behaviorString);
					if (flag2)
					{
						stringBuilder.Append(", Behavior = EventOptionBehavior." + behaviorString);
					}
				}
				bool flag3 = !option.RedirectTargetOption.Item1.IsNullOrEmpty() && !option.RedirectTargetOption.Item2.IsNullOrEmpty();
				if (flag3)
				{
					stringBuilder.Append(string.Concat(new string[]
					{
						", RedirectOption = (\"",
						option.RedirectTargetOption.Item1,
						"\", \"",
						option.RedirectTargetOption.Item2,
						"\")"
					}));
				}
				stringBuilder.Append("},\n\t\t\t\t");
			}
			bool flag4 = stringBuilder.Length > 6;
			if (flag4)
			{
				stringBuilder.Remove(stringBuilder.Length - 6, 6);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004AFB RID: 19195 RVA: 0x00232730 File Offset: 0x00230930
		public static string GetScriptReplaceString(string key, EventEditorData eventData)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
			if (num <= 1892602316U)
			{
				if (num <= 1166507078U)
				{
					if (num <= 630394995U)
					{
						if (num != 46479293U)
						{
							if (num == 630394995U)
							{
								if (key == "EventSortingOrder")
								{
									return eventData.EventOrder.ToString();
								}
							}
						}
						else if (key == "AdvancedUsing")
						{
							return string.Join("\n", Export.AdvancedUsing);
						}
					}
					else if (num != 801617692U)
					{
						if (num != 944298988U)
						{
							if (num == 1166507078U)
							{
								if (key == "EventGroup")
								{
									bool flag = !string.IsNullOrEmpty(eventData.EventGroup);
									if (flag)
									{
										return eventData.EventGroup;
									}
								}
							}
						}
						else if (key == "ModAuthor")
						{
							return SingletonObject.getInstance<EventEditorModel>().ModAuthor;
						}
					}
					else if (key == "EventName")
					{
						string eventNamespace = ModManager.GetWorkingModName();
						return string.Format("{0}Event_{1:N}", eventNamespace, Guid.Parse(eventData.EventGuid));
					}
				}
				else if (num <= 1824943553U)
				{
					if (num != 1623174496U)
					{
						if (num == 1824943553U)
						{
							if (key == "IsHeadEvent")
							{
								string eventTriggerType = eventData.TriggerType;
								return (eventTriggerType != "None" && eventTriggerType != "0" && !string.IsNullOrEmpty(eventTriggerType) && EventTriggerType.Instance.GetByKeyCode(eventTriggerType) != null).ToString().ToLower();
							}
						}
					}
					else if (key == "ForceSingle")
					{
						bool forceSingle = eventData.ForceSingle;
						if (forceSingle)
						{
							return eventData.ForceSingle.ToString().ToLower();
						}
						return "false";
					}
				}
				else if (num != 1847408878U)
				{
					if (num != 1867435785U)
					{
						if (num == 1892602316U)
						{
							if (key == "BaseUsing")
							{
								return string.Join("\n", Export.BaseUsing);
							}
						}
					}
					else if (key == "MaskTweenTime")
					{
						string time = eventData.MaskTweenTime.ToString();
						bool flag2 = string.IsNullOrEmpty(time);
						if (flag2)
						{
							time = "0.0";
						}
						return time;
					}
				}
				else if (key == "ModNamespace")
				{
					return SingletonObject.getInstance<EventEditorModel>().ModNamespace;
				}
			}
			else if (num <= 2179760199U)
			{
				if (num <= 2079510631U)
				{
					if (num != 1979535635U)
					{
						if (num == 2079510631U)
						{
							if (key == "EventType")
							{
								bool flag3 = !string.IsNullOrEmpty(eventData.EventType);
								if (flag3)
								{
									return eventData.EventType;
								}
								return "NoneType";
							}
						}
					}
					else if (key == "EventBackground")
					{
						bool flag4 = !string.IsNullOrEmpty(eventData.EventTexture);
						if (flag4)
						{
							return eventData.EventTexture;
						}
						return string.Empty;
					}
				}
				else if (num != 2140479012U)
				{
					if (num != 2147540871U)
					{
						if (num == 2179760199U)
						{
							if (key == "MainRoleKey")
							{
								bool flag5 = !string.IsNullOrEmpty(eventData.DecideRole);
								if (flag5)
								{
									return eventData.DecideRole;
								}
								return string.Empty;
							}
						}
					}
					else if (key == "TargetRoleKey")
					{
						bool flag6 = !string.IsNullOrEmpty(eventData.TargetRole);
						if (flag6)
						{
							return eventData.TargetRole;
						}
						return string.Empty;
					}
				}
				else if (key == "EventEditorShowName")
				{
					return eventData.EventName;
				}
			}
			else if (num <= 2656313787U)
			{
				if (num != 2513765866U)
				{
					if (num != 2595265402U)
					{
						if (num == 2656313787U)
						{
							if (key == "TriggerType")
							{
								bool flag7 = !string.IsNullOrEmpty(eventData.TriggerType);
								if (flag7)
								{
									string triggerType = eventData.TriggerType;
									bool flag8 = "0" != triggerType && "None" != triggerType && !string.IsNullOrEmpty(triggerType) && EventTriggerType.Instance.GetByKeyCode(eventData.TriggerType) != null;
									if (flag8)
									{
										return "Config.EventTriggerType.DefKey." + triggerType;
									}
								}
								return "-1";
							}
						}
					}
					else if (key == "EscOptionKey")
					{
						string escOptionKey = string.Empty;
						bool flag9 = !string.IsNullOrEmpty(eventData.EscOption);
						if (flag9)
						{
							escOptionKey = eventData.EscOption;
						}
						return escOptionKey;
					}
				}
				else if (key == "EventGuid")
				{
					return eventData.EventGuid;
				}
			}
			else if (num != 3113161579U)
			{
				if (num != 3676122888U)
				{
					if (num == 3792997858U)
					{
						if (key == "MaskControl")
						{
							bool controlMask = eventData.ControlMask;
							if (controlMask)
							{
								bool controlState = eventData.ControlMask;
								bool flag10 = !controlState;
								if (flag10)
								{
									return "NoChange";
								}
								byte code = eventData.ControlMaskCode;
								bool flag11 = code == 0;
								if (flag11)
								{
									return "ShowToMask";
								}
								bool flag12 = 1 == code;
								if (flag12)
								{
									return "HideToRevert";
								}
								bool flag13 = 2 == code;
								if (flag13)
								{
									return "ShowToMaskAndHideToRevert";
								}
								bool flag14 = 3 == code;
								if (flag14)
								{
									return "ShowToMaskToHide";
								}
								bool flag15 = 4 == code;
								if (flag15)
								{
									return "ShowToMaskToRevert";
								}
								bool flag16 = 5 == code;
								if (flag16)
								{
									return "HideToMask";
								}
							}
							return "NoChange";
						}
					}
				}
				else if (key == "EventContent")
				{
					return eventData.EventContent;
				}
			}
			else if (key == "EventOptionArray")
			{
				return Export.GetEventOptionCtorString(eventData);
			}
			return "${" + key + "}";
		}

		// Token: 0x06004AFC RID: 19196 RVA: 0x00232E5C File Offset: 0x0023105C
		private static bool IsMatchSign(string srcString, int offset, string signString)
		{
			int length = signString.Length;
			for (int i = 0; i < length; i++)
			{
				bool flag = signString[i] != srcString[offset + i];
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004AFD RID: 19197 RVA: 0x00232EA8 File Offset: 0x002310A8
		[return: TupleElementNames(new string[]
		{
			"consumeInfo",
			"availableCondition"
		})]
		public static ValueTuple<bool, bool> ProcessOptionOnCreate(List<string> codeLineList)
		{
			Export.<>c__DisplayClass18_0 CS$<>8__locals1;
			CS$<>8__locals1.codeLineList = codeLineList;
			bool flag = CS$<>8__locals1.codeLineList == null || CS$<>8__locals1.codeLineList.Count <= 2;
			ValueTuple<bool, bool> result;
			if (flag)
			{
				result = new ValueTuple<bool, bool>(false, false);
			}
			else
			{
				CS$<>8__locals1.model = SingletonObject.getInstance<EventEditorModel>();
				List<string> cacheList = EasyPool.Get<List<string>>();
				cacheList.AddRange(CS$<>8__locals1.codeLineList);
				CS$<>8__locals1.codeLineList.Clear();
				StringBuilder availableConditionCodeBuilder = new StringBuilder();
				StringBuilder consumeInfoCodeBuilder = new StringBuilder();
				bool matchAvailableCondition = false;
				bool matchConsumeInfo = false;
				bool hasConsumeInfo = false;
				bool hasAvailableCondition = false;
				for (int i = 0; i < cacheList.Count; i++)
				{
					string line = cacheList[i].Trim();
					line = Export.<ProcessOptionOnCreate>g__GetLineCode|18_2(line);
					bool flag2 = line.Contains("OptionAvailableConditions");
					if (flag2)
					{
						matchAvailableCondition = true;
					}
					bool flag3 = line.Contains("OptionConsumeInfos");
					if (flag3)
					{
						matchConsumeInfo = true;
					}
					bool flag4 = !matchAvailableCondition && !matchConsumeInfo;
					if (flag4)
					{
						CS$<>8__locals1.codeLineList.Add(cacheList[i]);
					}
					bool flag5 = matchAvailableCondition;
					if (flag5)
					{
						availableConditionCodeBuilder.Append(line);
						bool flag6 = line.Contains(";");
						if (flag6)
						{
							matchAvailableCondition = false;
							hasAvailableCondition = true;
							Export.<ProcessOptionOnCreate>g__ConvertConditionLines|18_0(availableConditionCodeBuilder.ToString(), ref CS$<>8__locals1);
						}
					}
					bool flag7 = matchConsumeInfo;
					if (flag7)
					{
						consumeInfoCodeBuilder.Append(line);
						bool flag8 = line.Contains(";");
						if (flag8)
						{
							matchConsumeInfo = false;
							hasConsumeInfo = true;
							Export.<ProcessOptionOnCreate>g__ConvertConsumeInfoLines|18_1(consumeInfoCodeBuilder.ToString(), ref CS$<>8__locals1);
						}
					}
				}
				result = new ValueTuple<bool, bool>(hasConsumeInfo, hasAvailableCondition);
			}
			return result;
		}

		// Token: 0x06004AFE RID: 19198 RVA: 0x00233044 File Offset: 0x00231244
		public static Exception ExportEventGroupScripts(EventGroupData groupData, string packageFolderName)
		{
			string eventGroupPath = Path.Combine(ModManager.GetModEventCompileScriptFolder(), packageFolderName + ".twes");
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			try
			{
				model.CompileEventGroupScripts(groupData, eventGroupPath);
			}
			catch (Exception e)
			{
				return e;
			}
			return null;
		}

		// Token: 0x06004AFF RID: 19199 RVA: 0x00233098 File Offset: 0x00231298
		public static IEnumerator CoroutineExportEventGroupList(EventEditorModel model, Export.MetaData metaData, List<string> eventGroupList, bool ignoreMarkedForCompile = false)
		{
			int num;
			for (int i = 0; i < eventGroupList.Count; i = num)
			{
				string eventGroup = eventGroupList[i];
				EventGroupData groupData = model.GetGroupData(eventGroup);
				bool flag = groupData == null;
				if (!flag)
				{
					bool flag2 = ignoreMarkedForCompile && groupData.Compile;
					if (!flag2)
					{
						yield return Export.CoroutineExportEventGroup(model, metaData, groupData);
						bool failed = metaData.Failed;
						if (failed)
						{
							yield break;
						}
						eventGroup = null;
						groupData = null;
					}
				}
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06004B00 RID: 19200 RVA: 0x002330BC File Offset: 0x002312BC
		public static IEnumerator CoroutineExportEventGroup(EventEditorModel model, Export.MetaData metaData, EventGroupData groupData)
		{
			Export.ExportingEventGroup = groupData.Key;
			string csGroupDir = Path.Combine(metaData.CsExportFolder, groupData.Key);
			Export.ClearDirectory(csGroupDir);
			bool flag = !Directory.Exists(csGroupDir);
			if (flag)
			{
				Directory.CreateDirectory(csGroupDir);
			}
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("using System.Collections.Generic;");
			builder.AppendLine("using Config.EventConfig;");
			builder.AppendLine();
			string packageName = "EventPackage_" + groupData.Key;
			string groupGuid = Guid.NewGuid().ToString("N");
			builder.AppendLine(string.Concat(new string[]
			{
				"namespace ",
				metaData.Author,
				".EventConfig.",
				metaData.Namespace,
				".EventGroup",
				groupGuid
			}));
			builder.AppendLine("{");
			builder.AppendLine(string.Concat(new string[]
			{
				"\tpublic class ",
				metaData.Namespace,
				"_",
				packageName,
				" : EventPackage"
			}));
			builder.AppendLine("\t{");
			builder.AppendLine(string.Concat(new string[]
			{
				"\t\tpublic ",
				metaData.Namespace,
				"_",
				packageName,
				"()"
			}));
			builder.AppendLine("\t\t{");
			builder.AppendLine("\t\t\tNameSpace = \"" + metaData.Namespace + "\";");
			builder.AppendLine("\t\t\tAuthor = \"" + metaData.Author + "\";");
			builder.AppendLine(string.Concat(new string[]
			{
				"\t\t\tGroup = \"",
				groupData.Key,
				"_",
				groupGuid,
				"\";"
			}));
			builder.AppendLine("\t\t\tEventList = new List<TaiwuEventItem>{");
			StringBuilder languageBuilder = new StringBuilder();
			languageBuilder.AppendLine("- Group : " + groupData.Key);
			languageBuilder.AppendLine("- GroupName : " + model.EventGroupInfoDic[groupData.Key]);
			languageBuilder.AppendLine("- Language : " + model.SystemLanguage);
			languageBuilder.AppendLine();
			List<ValueTuple<string, string, string>> list = groupData.GetDisplayList(true);
			int num;
			for (int i = 0; i < list.Count; i = num)
			{
				string eventGuid = list[i].Item1;
				EventEditorData eventData = groupData.GetEvent(eventGuid);
				bool flag2 = eventData == null;
				if (flag2)
				{
					Action<Exception> onExportEventFailure = metaData.OnExportEventFailure;
					if (onExportEventFailure != null)
					{
						onExportEventFailure(new Exception("Event file not found in group " + groupData.Key + ": " + eventGuid));
					}
					metaData.Failed = true;
					Export.ExportingEventGroup = null;
					yield break;
				}
				string className = Export.GetScriptReplaceString("EventName", eventData);
				string eventName = eventData.EventName;
				Action<EventEditorData> onExportEvent = metaData.OnExportEvent;
				if (onExportEvent != null)
				{
					onExportEvent(eventData);
				}
				languageBuilder.AppendLine("- EventGuid : " + eventGuid);
				languageBuilder.AppendLine("\t- EventName : " + eventName);
				string specialCharString = new string('\v', 1);
				string content = eventData.EventContent;
				bool flag3 = !string.IsNullOrEmpty(content);
				if (flag3)
				{
					content = content.Replace(specialCharString, "<NL>").Replace("/r", string.Empty);
				}
				languageBuilder.AppendLine("\t\t-- EventContent : " + content);
				Dictionary<int, EventEditorData.Option> options = eventData.Options;
				foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in options)
				{
					EventEditorData.Option option2;
					keyValuePair.Deconstruct(out num, out option2);
					int j = num;
					EventEditorData.Option option = option2;
					languageBuilder.AppendLine(string.Format("\t\t-- Option_{0} : {1}", j, option.Content));
					option = null;
				}
				Dictionary<int, EventEditorData.Option>.Enumerator enumerator = default(Dictionary<int, EventEditorData.Option>.Enumerator);
				languageBuilder.AppendLine();
				try
				{
					Export.ExportEvent(model, eventData, groupGuid);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Action<Exception> onExportEventFailure2 = metaData.OnExportEventFailure;
					if (onExportEventFailure2 != null)
					{
						onExportEventFailure2(e);
					}
					metaData.Failed = true;
					Export.ExportingEventGroup = null;
					yield break;
				}
				builder.Append("\t\t\t\tnew " + className + "()");
				bool flag4 = i < list.Count - 1;
				if (flag4)
				{
					builder.AppendLine(",");
				}
				else
				{
					builder.AppendLine();
				}
				bool flag5 = i % 15 == 0;
				if (flag5)
				{
					yield return null;
				}
				eventGuid = null;
				eventData = null;
				className = null;
				eventName = null;
				specialCharString = null;
				content = null;
				options = null;
				num = i + 1;
			}
			string packageFilename = metaData.Namespace + "_" + packageName;
			Exception e2 = Export.ExportEventGroupScripts(groupData, packageFilename);
			bool flag6 = e2 != null;
			if (flag6)
			{
				Action<Exception> onExportEventFailure3 = metaData.OnExportEventFailure;
				if (onExportEventFailure3 != null)
				{
					onExportEventFailure3(e2);
				}
				metaData.Failed = true;
				Export.ExportingEventGroup = null;
				yield break;
			}
			e2 = null;
			builder.AppendLine("\t\t\t};");
			builder.AppendLine("\t\t}");
			builder.AppendLine("\t}");
			builder.AppendLine("}");
			string managerFilePath = Path.Combine(csGroupDir, packageFilename + ".cs");
			File.WriteAllText(managerFilePath, builder.ToString(), new UTF8Encoding(false));
			string languageFilePath = Path.Combine(metaData.LanguageExportFolder, string.Concat(new string[]
			{
				metaData.Namespace,
				"_",
				packageName,
				"_Language_",
				model.SystemLanguage.ToUpper(),
				".txt"
			}));
			File.WriteAllText(languageFilePath, languageBuilder.ToString(), new UTF8Encoding(false));
			Action<EventGroupData> onExportEventGroupComplete = metaData.OnExportEventGroupComplete;
			if (onExportEventGroupComplete != null)
			{
				onExportEventGroupComplete(groupData);
			}
			Export.ExportingEventGroup = null;
			yield break;
		}

		// Token: 0x06004B01 RID: 19201 RVA: 0x002330DC File Offset: 0x002312DC
		public static void ExportEvent(EventEditorModel model, EventEditorData eventData, string groupGuid)
		{
			string saveDir = Save.GetEventSaveDir(eventData);
			string eventGuid = eventData.EventGuid;
			string eventCodeFilePath = Path.Combine(saveDir, eventGuid + ".cs");
			Regex regex = new Regex("\\$\\{(?<Key>[a-z|A-Z][a-z|A-Z|0-9]+)\\}");
			bool flag = string.IsNullOrEmpty(Export._totalFileContent);
			if (flag)
			{
				string totalFileTemplatePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventTemplate.template");
				bool flag2 = !File.Exists(totalFileTemplatePath);
				if (flag2)
				{
					string msg = "can not find file at " + totalFileTemplatePath + ",check the template file first!";
					throw new Exception(msg);
				}
				Export._totalFileContent = File.ReadAllText(totalFileTemplatePath);
			}
			string totalFileContent = regex.Replace(Export._totalFileContent, delegate(Match match)
			{
				string key = match.Groups["Key"].Value;
				string result = Export.GetScriptReplaceString(match.Groups["Key"].Value, eventData);
				bool flag21 = !string.IsNullOrEmpty(groupGuid) && key == "ModNamespace";
				if (flag21)
				{
					result = result + ".EventGroup" + groupGuid;
				}
				return result;
			});
			bool flag3 = string.IsNullOrEmpty(Export._ctorContent);
			if (flag3)
			{
				string ctorTemplateFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventConstructor.template");
				bool flag4 = !File.Exists(ctorTemplateFilePath);
				if (flag4)
				{
					string msg2 = "can not find file at " + ctorTemplateFilePath + ",check the template file first!";
					throw new Exception(msg2);
				}
				Export._ctorContent = File.ReadAllText(ctorTemplateFilePath);
			}
			string ctorContent = regex.Replace(Export._ctorContent, (Match match) => Export.GetScriptReplaceString(match.Groups["Key"].Value, eventData));
			totalFileContent = totalFileContent.Replace("${Ctor}", ctorContent);
			string scriptUrl = Path.Combine(saveDir, eventGuid + "_boolState.twe");
			Dictionary<short, EventBoolStateInfo> eventBoolState = File.Exists(scriptUrl) ? JsonConvert.DeserializeObject<Dictionary<short, EventBoolStateInfo>>(File.ReadAllText(scriptUrl)) : null;
			StringBuilder boolStateString = new StringBuilder();
			bool flag5 = eventBoolState != null;
			if (flag5)
			{
				int index = 0;
				foreach (KeyValuePair<short, EventBoolStateInfo> pair in eventBoolState)
				{
					boolStateString.Append(string.Format("var state{0} = new EventBoolStateInfo();", index));
					boolStateString.Append(string.Format("state{0}.BoolState = {1};", index, pair.Value.BoolState.ToString().ToLower()));
					boolStateString.Append(string.Format("state{0}.RemoveBeforeNextEvent = {1};", index, pair.Value.RemoveBeforeNextEvent.ToString().ToLower()));
					boolStateString.Append(string.Format("state{0}.EventBoolStateTemplateId = {1};", index, pair.Value.EventBoolStateTemplateId));
					boolStateString.Append(string.Format("BoolStateDict.Add({0},state{1});", pair.Key, index));
					index++;
				}
			}
			totalFileContent = totalFileContent.Replace("${BoolStateDict}", boolStateString.ToString());
			bool flag6 = Export._defaultOptionApiDic == null;
			if (flag6)
			{
				string defaultOptionFilePath = Path.Combine(Application.streamingAssetsPath, "EventScriptsTemplates/TaiwuEventOptionTemplate.template");
				bool flag7 = !File.Exists(defaultOptionFilePath);
				if (flag7)
				{
					string msg3 = "can not find file at " + defaultOptionFilePath + ",check the template file first!";
					throw new Exception(msg3);
				}
				Export._defaultOptionApiDic = Export.GetFileFunctionInfos(defaultOptionFilePath);
			}
			Dictionary<int, EventEditorData.Option> options = eventData.Options;
			StringBuilder apiBuilder = new StringBuilder();
			apiBuilder.AppendLine("private void InitOptions()");
			Export.AppendBuilderLineWithTab(apiBuilder, 2, "{");
			int i = 0;
			foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in options)
			{
				int num;
				EventEditorData.Option option2;
				keyValuePair.Deconstruct(out num, out option2);
				EventEditorData.Option option = option2;
				bool flag8 = i != 0;
				if (flag8)
				{
					apiBuilder.AppendLine();
				}
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].OnOptionVisibleCheck = OnOption{1}VisibleCheck;//option.Guid:{2}", i, i + 1, option.Guid));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].OnOptionVisibleCheck = OnOption{1}VisibleCheck;", i, i + 1));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].OnOptionAvailableCheck = OnOption{1}AvailableCheck;", i, i + 1));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].GetReplacedContent = OnOption{1}GetReplacedContent;", i, i + 1));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].OnOptionSelect = OnOption{1}Select;", i, i + 1));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].GetExtraFormatLanguageKeys = Option{1}GetExtraFormatLanguageKeys;", i, i + 1));
				string defaultState = OptionDefaultStateKeys.Normal;
				bool flag9 = !string.IsNullOrEmpty(option.DefaultState);
				if (flag9)
				{
					defaultState = option.DefaultState;
				}
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].DefaultState = EventOptionState.{1};", i, defaultState));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].OneTimeOnly = {1};", i, option.OneTimeOnly.ToString().ToLower()));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("EventOptions[{0}].Important = {1};", i, option.Important.ToString().ToLower()));
				Export.AppendBuilderLineWithTab(apiBuilder, 3, string.Format("OnOption{0}Create();", i + 1));
				i++;
			}
			apiBuilder.AppendLine();
			Export.AppendBuilderLineWithTab(apiBuilder, 2, "}");
			apiBuilder.AppendLine();
			Dictionary<string, List<string>> eventOverrideApiDic = File.Exists(eventCodeFilePath) ? Export.GetFileFunctionInfos(eventCodeFilePath) : Export.GetDefaultEventFunctionInfos();
			bool flag10 = eventOverrideApiDic != null;
			if (flag10)
			{
				List<string> usingLines;
				bool flag11 = eventOverrideApiDic.TryGetValue("CustomUsings", out usingLines);
				if (flag11)
				{
					StringBuilder usingBuilder = new StringBuilder();
					usingLines.ForEach(delegate(string line)
					{
						usingBuilder.AppendLine(line);
					});
					totalFileContent = totalFileContent.Replace("${CustomUsings}", usingBuilder.ToString());
				}
				else
				{
					totalFileContent = totalFileContent.Replace("${CustomUsings}", string.Empty);
				}
				List<string> fieldLines;
				bool flag12 = eventOverrideApiDic.TryGetValue("CustomFields", out fieldLines);
				if (flag12)
				{
					StringBuilder fieldsBuilder = new StringBuilder();
					fieldLines.ForEach(delegate(string line)
					{
						fieldsBuilder.AppendLine(line);
					});
					totalFileContent = totalFileContent.Replace("${CustomFields}", fieldsBuilder.ToString());
				}
				else
				{
					totalFileContent = totalFileContent.Replace("${CustomFields}", string.Empty);
				}
				foreach (KeyValuePair<string, List<string>> pair2 in eventOverrideApiDic)
				{
					bool flag13 = pair2.Key == "CustomUsings" || pair2.Key == "CustomFields";
					if (!flag13)
					{
						for (int j = 0; j < pair2.Value.Count; j++)
						{
							Export.AppendBuilderLineWithTab(apiBuilder, 2, pair2.Value[j]);
						}
						apiBuilder.AppendLine();
					}
				}
			}
			string eventApiContent = apiBuilder.ToString();
			totalFileContent = totalFileContent.Replace("${EventAPI}", eventApiContent);
			apiBuilder.Clear();
			Dictionary<string, string> functionNameConvertDic = new Dictionary<string, string>
			{
				{
					"OnCreate",
					"OnOption{0}Create"
				},
				{
					"OnVisibleCheck",
					"OnOption{0}VisibleCheck"
				},
				{
					"OnAvailableCheck",
					"OnOption{0}AvailableCheck"
				},
				{
					"OnGetReplacedContent",
					"OnOption{0}GetReplacedContent"
				},
				{
					"OnSelect",
					"OnOption{0}Select"
				},
				{
					"GetExtraFormatLanguageKeys",
					"Option{0}GetExtraFormatLanguageKeys"
				}
			};
			foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in options)
			{
				int num;
				EventEditorData.Option option2;
				keyValuePair.Deconstruct(out num, out option2);
				int k = num;
				EventEditorData.Option optionCellTable = option2;
				string guid = optionCellTable.Guid;
				string optionScriptFilePath = Path.Combine(saveDir, guid + ".cs");
				string optionConsumptionPath = Path.Combine(saveDir, guid + "_cost.twe");
				List<EventOptionCost> costs = File.Exists(optionConsumptionPath) ? JsonConvert.DeserializeObject<List<EventOptionCost>>(File.ReadAllText(optionConsumptionPath)) : null;
				Dictionary<string, List<string>> optionFunctionInfoDic = File.Exists(optionScriptFilePath) ? Export.GetFileFunctionInfos(optionScriptFilePath) : Export._defaultOptionApiDic;
				foreach (KeyValuePair<string, List<string>> pair3 in optionFunctionInfoDic)
				{
					string funcKey = pair3.Key;
					List<string> codeLines = pair3.Value;
					string convertName;
					bool flag14 = functionNameConvertDic.TryGetValue(funcKey, out convertName);
					if (flag14)
					{
						bool flag15 = funcKey == "OnCreate";
						if (flag15)
						{
							ValueTuple<bool, bool> valueTuple = Export.ProcessOptionOnCreate(codeLines);
							bool consumeInfo = valueTuple.Item1;
							bool availableCondition = valueTuple.Item2;
							bool flag16 = costs != null && costs.Count > 0;
							if (flag16)
							{
								bool flag17 = optionFunctionInfoDic == Export._defaultOptionApiDic;
								if (flag17)
								{
									codeLines = new List<string>(codeLines);
								}
								bool flag18 = !consumeInfo;
								if (flag18)
								{
									codeLines.Insert(codeLines.Count - 1, "\tthisOption.OptionConsumeInfos = new List<OptionConsumeInfo>();");
								}
								for (int index2 = 0; index2 < costs.Count; index2++)
								{
									EventOptionCost cost = costs[index2];
									codeLines.Insert(codeLines.Count - 1, string.Format("\tthisOption.OptionConsumeInfos.Add(new OptionConsumeInfo({0}, {1}, {2}));", cost.ConsumeType, cost.CostAmount, cost.AutoConsume.ToString().ToLower()));
									bool flag19 = cost.Expression != null;
									if (flag19)
									{
										codeLines.Insert(codeLines.Count - 1, "\tthisOption.OptionConsumeAmountExpressions ??= new Dictionary<int, string>();");
										codeLines.Insert(codeLines.Count - 1, string.Format("\tthisOption.OptionConsumeAmountExpressions[{0}] = \"{1}\";", index2, cost.Expression));
									}
								}
							}
						}
						string functionNameLine = codeLines[0].Replace(funcKey, string.Format(convertName, k));
						for (int l = 0; l < codeLines.Count; l++)
						{
							Export.AppendBuilderLineWithTab(apiBuilder, 2, (l == 0) ? functionNameLine : codeLines[l].Replace("thisOption", string.Format("EventOptions[{0}]", k - 1)));
						}
						apiBuilder.AppendLine();
					}
					else
					{
						for (int m = 0; m < codeLines.Count; m++)
						{
							Export.AppendBuilderLineWithTab(apiBuilder, 2, codeLines[m].Replace("thisOption", string.Format("EventOptions[{0}]", k - 1)));
						}
						apiBuilder.AppendLine();
					}
				}
				bool flag20 = !optionFunctionInfoDic.ContainsKey("GetExtraFormatLanguageKeys");
				if (flag20)
				{
					List<string> apiLines;
					Export._defaultOptionApiDic.TryGetValue("GetExtraFormatLanguageKeys", out apiLines);
					string convertName2;
					functionNameConvertDic.TryGetValue("GetExtraFormatLanguageKeys", out convertName2);
					string functionNameLine2 = apiLines[0].Replace("GetExtraFormatLanguageKeys", string.Format(convertName2, k));
					for (int n = 0; n < apiLines.Count; n++)
					{
						Export.AppendBuilderLineWithTab(apiBuilder, 2, (n == 0) ? functionNameLine2 : apiLines[n].Replace("thisOption", string.Format("EventOptions[{0}]", k - 1)));
					}
					apiBuilder.AppendLine();
				}
			}
			string optionApiContent = apiBuilder.ToString();
			totalFileContent = totalFileContent.Replace("${Options}", optionApiContent);
			apiBuilder.Clear();
			string eventClassName = Export.GetScriptReplaceString("EventName", eventData);
			string exportDir = ModManager.GetModEventExportCsFilesFolder();
			string eventGroup = eventData.EventGroup;
			string exportFilePath = Path.Combine(exportDir, eventGroup, eventClassName + ".cs");
			File.WriteAllText(exportFilePath, totalFileContent, new UTF8Encoding(false));
		}

		// Token: 0x06004B02 RID: 19202 RVA: 0x00233D28 File Offset: 0x00231F28
		public static void ClearDirectory(string targetDir)
		{
			bool flag = !Directory.Exists(targetDir);
			if (!flag)
			{
				string[] files = Directory.GetFiles(targetDir);
				string[] dirs = Directory.GetDirectories(targetDir);
				foreach (string dir in dirs)
				{
					Export.ClearDirectory(dir);
					Directory.Delete(dir);
				}
				foreach (string file in files)
				{
					File.SetAttributes(file, FileAttributes.Normal);
					File.Delete(file);
				}
			}
		}

		// Token: 0x06004B03 RID: 19203 RVA: 0x00233DB5 File Offset: 0x00231FB5
		public static void ExportEventCsFiles()
		{
		}

		// Token: 0x06004B04 RID: 19204 RVA: 0x00233DB8 File Offset: 0x00231FB8
		private static void ExportEventOptionConditionScript(string directory)
		{
			Dictionary<string, EventOptionConditionConfig> mainTable = SingletonObject.getInstance<EventEditorModel>().EventOptionConditionsConfig;
			string idFilePath = Path.Combine(directory.PathFix(), "EventOption/ConditionId.cs");
			StringBuilder idFileBuilder = new StringBuilder();
			idFileBuilder.AppendLine("////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n// This File is generated by the program, DO NOT EDIT MANUALLY!\n// 此文件由程序生成, 切勿手动编辑!\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n#pragma warning disable 1591\nnamespace GameData.Domains.TaiwuEvent.EventOption\n{\n    /// <summary>\n    /// 选项可用条件检测ID\n    /// </summary>\n    public class ConditionId\n    {");
			string matcherFilePath = Path.Combine(directory.PathFix(), "EventOption/OptionConditionMatcher.cs");
			Dictionary<string, List<string>> existMatchersMap = Export.GetFileFunctionInfos(matcherFilePath);
			StringBuilder addMatchersBuilder = new StringBuilder();
			List<string> keys = new List<string>();
			keys.AddRange(mainTable.Keys);
			keys.Sort((string a, string b) => mainTable[a].Id.CompareTo(mainTable[b].Id));
			foreach (string key in keys)
			{
				EventOptionConditionConfig conditionTab = mainTable[key];
				Export.AppendBuilderLineWithTab(idFileBuilder, 2, "/// <summary>");
				Export.AppendBuilderLineWithTab(idFileBuilder, 2, "/// " + conditionTab.Content);
				Export.AppendBuilderLineWithTab(idFileBuilder, 2, "/// </summary>");
				Export.AppendBuilderLineWithTab(idFileBuilder, 2, string.Format("public const short {0} = {1};", key, conditionTab.Id));
				string apiHeader = string.Concat(new string[]
				{
					"public static bool ",
					key,
					"(",
					Export.<ExportEventOptionConditionScript>g__ArgConfigToCode|25_0(conditionTab.MatcherArg),
					")"
				});
				bool flag = existMatchersMap == null || !existMatchersMap.ContainsKey(key);
				if (flag)
				{
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "/// <summary>");
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "/// " + conditionTab.Content);
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "/// </summary>");
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, apiHeader);
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "{");
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 3, "// TODO Fill Api Content");
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 3, "throw new NotImplementedException();");
					Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "}");
					addMatchersBuilder.AppendLine();
				}
				else
				{
					List<string> matcherContentList;
					bool flag2 = existMatchersMap.TryGetValue(key, out matcherContentList) && apiHeader != matcherContentList[0].Trim();
					if (flag2)
					{
						matcherContentList.Insert(2, "\t\t\t//TODO this matcher has a same declaration in this file which is out of date,you are suggested to delete that matcher after you check or handle this matcher");
						Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "/// <summary>");
						Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "/// " + conditionTab.Content);
						Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, "/// </summary>");
						Export.AppendBuilderLineWithTab(addMatchersBuilder, 2, apiHeader);
						for (int i = 1; i < matcherContentList.Count; i++)
						{
							Export.AppendBuilderLineWithTab(addMatchersBuilder, 0, matcherContentList[i]);
						}
					}
				}
			}
			idFileBuilder.AppendLine("\t}");
			idFileBuilder.AppendLine("}");
			File.WriteAllText(idFilePath, idFileBuilder.ToString(), new UTF8Encoding(false));
			Debug.Log("ConditionId.cs 导出到路径 " + idFilePath);
			StringBuilder matcherFileBuilder = new StringBuilder();
			bool flag3 = File.Exists(matcherFilePath);
			if (flag3)
			{
				string fileContent = File.ReadAllText(matcherFilePath);
				int rightBraceCount = 0;
				for (int j = fileContent.Length - 1; j >= 0; j--)
				{
					bool flag4 = fileContent[j] == '}';
					if (flag4)
					{
						rightBraceCount++;
					}
					bool flag5 = rightBraceCount == 2;
					if (flag5)
					{
						matcherFileBuilder.Append(fileContent.Substring(0, j - 1));
						break;
					}
				}
				matcherFileBuilder.Append(addMatchersBuilder);
				matcherFileBuilder.AppendLine("\t}");
				matcherFileBuilder.AppendLine("}");
			}
			else
			{
				matcherFileBuilder.AppendLine("#pragma warning disable 1591\n\nnamespace GameData.Domains.TaiwuEvent.EventOption\n{\n    /// <summary>\n    /// 选项可用条件检测方法集合\n    /// </summary>\n    public static class OptionConditionMatcher\n    {");
				matcherFileBuilder.Append(addMatchersBuilder);
				matcherFileBuilder.AppendLine("\t}");
				matcherFileBuilder.AppendLine("}");
			}
			File.WriteAllText(matcherFilePath, matcherFileBuilder.ToString(), new UTF8Encoding(false));
			Debug.Log("OptionConditionMatcher.cs 导出到路径 " + matcherFilePath);
		}

		// Token: 0x06004B05 RID: 19205 RVA: 0x002341CC File Offset: 0x002323CC
		public static List<string> GetEventGuidListOrderFromFile(string filePath)
		{
			bool flag = !File.Exists(filePath);
			List<string> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string[] lines = File.ReadAllLines(filePath);
				List<string> resultList = new List<string>();
				HashSet<string> hashSet = new HashSet<string>();
				int i = 0;
				int max = lines.Length;
				while (i < max)
				{
					string contentLine = lines[i].Trim();
					bool flag2 = contentLine.StartsWith("[\"");
					if (flag2)
					{
						string guid = contentLine.Substring(2, 36);
						bool flag3 = hashSet.Add(guid);
						if (flag3)
						{
							resultList.Add(guid);
						}
						else
						{
							Debug.LogWarning(string.Concat(new string[]
							{
								"Duplicate event ",
								guid,
								" detected in file ",
								filePath,
								"."
							}));
						}
					}
					i++;
				}
				result = resultList;
			}
			return result;
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x00234390 File Offset: 0x00232590
		[CompilerGenerated]
		internal static void <ProcessOptionOnCreate>g__ConvertConditionLines|18_0(string codeContent, ref Export.<>c__DisplayClass18_0 A_1)
		{
			A_1.codeLineList.Add(string.Empty);
			A_1.codeLineList.Add("\tthisOption.OptionAvailableConditions = new List<TaiwuEventOptionConditionBase>();");
			List<char> cacheChars = new List<char>();
			List<string> conditionDataCache = new List<string>();
			List<string> orConditionCoreCache = new List<string>();
			int leftBraceCount = 0;
			int rightBraceCount = 0;
			bool listCreateFlag = false;
			bool conditionCreateFlag = false;
			for (int i = 0; i < codeContent.Length; i++)
			{
				bool flag = codeContent[i] == '{';
				if (flag)
				{
					leftBraceCount++;
					bool flag2 = rightBraceCount > 0;
					if (flag2)
					{
						rightBraceCount--;
					}
				}
				else
				{
					bool flag3 = codeContent[i] == '}';
					if (flag3)
					{
						rightBraceCount++;
						leftBraceCount--;
						conditionDataCache.Add(new string(cacheChars.ToArray()));
						cacheChars.Clear();
						bool flag4 = 2 == leftBraceCount;
						if (flag4)
						{
							string key = conditionDataCache[0];
							EventOptionConditionConfig configTable;
							bool flag5 = A_1.model.EventOptionConditionsConfig.TryGetValue(key, out configTable);
							if (!flag5)
							{
								throw new Exception("Invalid condition used: " + key);
							}
							string type = configTable.Constructor;
							conditionDataCache.RemoveAt(0);
							string argString = (conditionDataCache.Count > 0) ? string.Join(", ", conditionDataCache) : string.Empty;
							bool flag6 = !string.IsNullOrEmpty(argString);
							if (flag6)
							{
								argString += ",";
							}
							orConditionCoreCache.Add(string.Concat(new string[]
							{
								"condition = new ",
								type,
								"(ConditionId.",
								key,
								", ",
								argString,
								" OptionConditionMatcher.",
								key,
								");"
							}));
							conditionDataCache.Clear();
						}
						bool flag7 = 2 == rightBraceCount;
						if (flag7)
						{
							for (int j = 0; j < orConditionCoreCache.Count; j++)
							{
								string line = orConditionCoreCache[j];
								bool flag8 = !conditionCreateFlag;
								if (flag8)
								{
									conditionCreateFlag = true;
									line = "\n\t\t\tTaiwuEventOptionConditionBase " + line;
								}
								else
								{
									line = "\n\t\t\t" + line;
								}
								A_1.codeLineList.Add(line);
								bool flag9 = j == 0;
								if (flag9)
								{
									A_1.codeLineList.Add("\tthisOption.OptionAvailableConditions.Add(condition);");
									bool flag10 = orConditionCoreCache.Count > 1;
									if (flag10)
									{
										bool flag11 = !listCreateFlag;
										if (flag11)
										{
											listCreateFlag = true;
											A_1.codeLineList.Add("\tList<TaiwuEventOptionConditionBase> list = new List<TaiwuEventOptionConditionBase> {condition};");
										}
										A_1.codeLineList.Add("\tcondition.OrConditionCore = list;\n");
									}
								}
								else
								{
									A_1.codeLineList.Add("\tlist.Add(condition);\n");
								}
							}
							cacheChars.Clear();
							conditionDataCache.Clear();
							orConditionCoreCache.Clear();
						}
					}
					else
					{
						bool flag12 = leftBraceCount == 3;
						if (flag12)
						{
							bool flag13 = codeContent[i] == ',';
							if (flag13)
							{
								conditionDataCache.Add(new string(cacheChars.ToArray()));
								cacheChars.Clear();
							}
							else
							{
								cacheChars.Add(codeContent[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004B09 RID: 19209 RVA: 0x002346A4 File Offset: 0x002328A4
		[CompilerGenerated]
		internal static void <ProcessOptionOnCreate>g__ConvertConsumeInfoLines|18_1(string codeContent, ref Export.<>c__DisplayClass18_0 A_1)
		{
			A_1.codeLineList.Add(string.Empty);
			A_1.codeLineList.Add("\tthisOption.OptionConsumeInfos = new List<OptionConsumeInfo>();");
			List<string> singleConsumeInfoCache = EasyPool.Get<List<string>>();
			List<char> singleWordCache = EasyPool.Get<List<char>>();
			int braceCount = 0;
			int length = codeContent.Length;
			for (int i = 0; i < length; i++)
			{
				char c = codeContent[i];
				bool flag = c == '{';
				if (flag)
				{
					braceCount++;
				}
				else
				{
					bool flag2 = c == ',';
					if (flag2)
					{
						bool flag3 = singleWordCache.Count > 0;
						if (flag3)
						{
							string word = new string(singleWordCache.ToArray());
							singleConsumeInfoCache.Add(word.Trim());
						}
						singleWordCache.Clear();
					}
					else
					{
						bool flag4 = c == '}';
						if (flag4)
						{
							bool flag5 = singleWordCache.Count > 0;
							if (flag5)
							{
								string word2 = new string(singleWordCache.ToArray());
								singleConsumeInfoCache.Add(word2.Trim());
							}
							singleWordCache.Clear();
							braceCount--;
							bool flag6 = braceCount == 1;
							if (flag6)
							{
								bool flag7 = singleConsumeInfoCache.Count < 2;
								if (flag7)
								{
									throw new Exception("EventExport Error:Option.OptionConsumeInfos syntax error: " + codeContent);
								}
								string consumeTypeArg = "OptionConsumeType." + singleConsumeInfoCache[0];
								string consumeAutoArg = "true";
								bool autoArg;
								bool flag8 = singleConsumeInfoCache.CheckIndex(2) && bool.TryParse(singleConsumeInfoCache[2], out autoArg);
								if (flag8)
								{
									consumeAutoArg = autoArg.ToString().ToLower();
								}
								A_1.codeLineList.Add(string.Concat(new string[]
								{
									"\tthisOption.OptionConsumeInfos.Add(new OptionConsumeInfo(",
									consumeTypeArg,
									", ",
									singleConsumeInfoCache[1],
									", ",
									consumeAutoArg,
									"));"
								}));
							}
							singleConsumeInfoCache.Clear();
						}
						else
						{
							bool flag9 = c == ';';
							if (flag9)
							{
								break;
							}
							bool flag10 = braceCount == 2;
							if (flag10)
							{
								singleWordCache.Add(c);
							}
						}
					}
				}
			}
			EasyPool.Free<List<char>>(singleWordCache);
			EasyPool.Free<List<string>>(singleConsumeInfoCache);
		}

		// Token: 0x06004B0A RID: 19210 RVA: 0x002348B8 File Offset: 0x00232AB8
		[CompilerGenerated]
		internal static string <ProcessOptionOnCreate>g__GetLineCode|18_2(string lineContent)
		{
			bool flag = string.IsNullOrEmpty(lineContent);
			string result2;
			if (flag)
			{
				result2 = string.Empty;
			}
			else
			{
				List<char> resultList = EasyPool.Get<List<char>>();
				bool commentFlag = false;
				int length = lineContent.Length;
				for (int i = 0; i < length; i++)
				{
					char c = lineContent[i];
					bool flag2 = c == '/';
					if (flag2)
					{
						bool flag3 = i + 1 < length;
						if (flag3)
						{
							char cNext = lineContent[i + 1];
							bool flag4 = cNext == '/';
							if (flag4)
							{
								break;
							}
							bool flag5 = cNext == '*';
							if (flag5)
							{
								commentFlag = true;
								i++;
							}
						}
					}
					else
					{
						bool flag6 = c == '*';
						if (flag6)
						{
							bool flag7 = i + 1 < length;
							if (flag7)
							{
								char cNext2 = lineContent[i + 1];
								bool flag8 = cNext2 == '/';
								if (flag8)
								{
									commentFlag = false;
									i++;
								}
							}
						}
					}
					bool flag9 = commentFlag;
					if (!flag9)
					{
						resultList.Add(c);
					}
				}
				string result = new string(resultList.ToArray());
				EasyPool.Free<List<char>>(resultList);
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06004B0B RID: 19211 RVA: 0x002349D4 File Offset: 0x00232BD4
		[CompilerGenerated]
		internal static string <ExportEventOptionConditionScript>g__ArgConfigToCode|25_0(string argConfig)
		{
			bool flag = string.IsNullOrEmpty(argConfig);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string[] argsArray = argConfig.Split(',', StringSplitOptions.None);
				List<string> resultList = new List<string>();
				for (int i = 0; i < argsArray.Length; i++)
				{
					resultList.Add(string.Format("{0} arg{1}", argsArray[i], i));
				}
				result = string.Join(", ", resultList);
			}
			return result;
		}

		// Token: 0x04003410 RID: 13328
		private static readonly string[] BaseUsing = new string[]
		{
			"using System;",
			"using System.Linq;",
			"using System.Collections.Generic;",
			"using Config.EventConfig;",
			"using GameData.Domains;",
			"using GameData.Domains.TaiwuEvent;",
			"using GameData.Domains.TaiwuEvent.Enum;",
			"using GameData.Domains.TaiwuEvent.EventHelper;",
			"using GameData.Domains.TaiwuEvent.DisplayEvent;",
			"using GameData.Domains.TaiwuEvent.EventOption;",
			"using GameData.Domains.Story.SectMainStory;"
		};

		// Token: 0x04003411 RID: 13329
		private static readonly string[] AdvancedUsing = new string[]
		{
			"using GameData.Domains.Character;",
			"using GameData.Domains.Character.AvatarSystem;",
			"using GameData.Domains.Combat;",
			"using GameData.Domains.CombatSkill;",
			"using GameData.Domains.Character.Relation;",
			"using GameData.Domains.Adventure;",
			"using GameData.Domains.Item;",
			"using GameData.Domains.Item.Display;",
			"using GameData.Domains.World;",
			"using GameData.Domains.Information;",
			"using GameData.Domains.Map;",
			"using GameData.Domains.Taiwu.Profession;"
		};

		// Token: 0x04003412 RID: 13330
		private static Dictionary<string, List<string>> _eventCodeDefaultFunctionInfos;

		// Token: 0x04003414 RID: 13332
		private static string _totalFileContent;

		// Token: 0x04003415 RID: 13333
		private static string _ctorContent;

		// Token: 0x04003416 RID: 13334
		private static Dictionary<string, List<string>> _defaultOptionApiDic;

		// Token: 0x02001A13 RID: 6675
		public class MetaData
		{
			// Token: 0x0400B4E3 RID: 46307
			public string Author;

			// Token: 0x0400B4E4 RID: 46308
			public string Namespace;

			// Token: 0x0400B4E5 RID: 46309
			public string CsExportFolder;

			// Token: 0x0400B4E6 RID: 46310
			public string ScriptExportFolder;

			// Token: 0x0400B4E7 RID: 46311
			public string LanguageExportFolder;

			// Token: 0x0400B4E8 RID: 46312
			public Action<EventEditorData> OnExportEvent;

			// Token: 0x0400B4E9 RID: 46313
			public Action<EventGroupData> OnExportEventGroupComplete;

			// Token: 0x0400B4EA RID: 46314
			public Action<Exception> OnExportEventFailure;

			// Token: 0x0400B4EB RID: 46315
			public bool Failed;
		}
	}
}
