using System;
using System.Collections.Generic;
using Config;
using Config.Common;
using EventScript;
using GameData.Adventure.Editor;
using Newtonsoft.Json;

namespace EventEditor.EventScript
{
	// Token: 0x0200065A RID: 1626
	[Serializable]
	public class EventInstructionEditorData
	{
		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x06004D72 RID: 19826 RVA: 0x0024882C File Offset: 0x00246A2C
		// (set) Token: 0x06004D73 RID: 19827 RVA: 0x00248840 File Offset: 0x00246A40
		[JsonIgnore]
		public int FuncId
		{
			get
			{
				EventFunctionItem functionConfig = this.FunctionConfig;
				return (functionConfig != null) ? functionConfig.TemplateId : -1;
			}
			set
			{
				this.FunctionName = SingletonObject.getInstance<EventEditorModel>().GetConfigRefKey(EventFunction.Instance, value);
			}
		}

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06004D74 RID: 19828 RVA: 0x00248858 File Offset: 0x00246A58
		[JsonIgnore]
		public EventFunctionItem FunctionConfig
		{
			get
			{
				return EventFunction.Instance[this.FunctionName];
			}
		}

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06004D75 RID: 19829 RVA: 0x0024886C File Offset: 0x00246A6C
		[JsonIgnore]
		public bool CanOperateAlone
		{
			get
			{
				EventFunctionItem funcConfig = this.FunctionConfig;
				bool result;
				if (!funcConfig.IndentNext && funcConfig.FollowUp < 0)
				{
					List<int> requiredPreviousCommands = funcConfig.RequiredPreviousCommands;
					result = (requiredPreviousCommands == null || requiredPreviousCommands.Count <= 0);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x06004D76 RID: 19830 RVA: 0x002488B2 File Offset: 0x00246AB2
		public EventInstructionEditorData()
		{
		}

		// Token: 0x06004D77 RID: 19831 RVA: 0x002488BC File Offset: 0x00246ABC
		public EventInstructionEditorData(EventInstructionEditorData other)
		{
			this.Indent = other.Indent;
			this.AssignToVar = other.AssignToVar;
			this.FunctionName = other.FunctionName;
			this.Reverse = other.Reverse;
			this.Args = ((other.Args.Length != 0) ? new EventArgumentEditorData[other.Args.Length] : Array.Empty<EventArgumentEditorData>());
			for (int i = 0; i < this.Args.Length; i++)
			{
				this.Args[i] = new EventArgumentEditorData(other.Args[i]);
			}
		}

		// Token: 0x06004D78 RID: 19832 RVA: 0x00248950 File Offset: 0x00246B50
		public EventInstructionEditorData(string text)
		{
			string[] elements = text.Split('|', StringSplitOptions.None);
			this.Indent = int.Parse(elements[0]);
			this.AssignToVar = (string.IsNullOrWhiteSpace(elements[1]) ? null : elements[1]);
			this.FuncId = int.Parse(elements[2]);
			int[] parameterTypes = this.FunctionConfig.ParameterTypes;
			int argCount = (parameterTypes != null) ? parameterTypes.Length : 0;
			this.Args = ((argCount > 0) ? new EventArgumentEditorData[argCount] : Array.Empty<EventArgumentEditorData>());
			bool flag = !string.IsNullOrWhiteSpace(elements[3]);
			if (flag)
			{
				string[] argStrings = elements[3].Split(',', StringSplitOptions.None);
				for (int i = 0; i < argStrings.Length; i++)
				{
					this.Args[i] = new EventArgumentEditorData(argStrings[i], true);
				}
			}
			bool flag2 = elements.CheckIndex(4);
			if (flag2)
			{
				this.Reverse = (int.Parse(elements[4]) == 1);
			}
		}

		// Token: 0x06004D79 RID: 19833 RVA: 0x00248A34 File Offset: 0x00246C34
		public EventInstructionEditorData(int funcId)
		{
			this.FuncId = funcId;
			EventFunctionItem functionCfg = this.FunctionConfig;
			int argCount = functionCfg.ParameterTypes.Length;
			this.Args = ((argCount > 0) ? new EventArgumentEditorData[argCount] : Array.Empty<EventArgumentEditorData>());
			for (int i = 0; i < argCount; i++)
			{
				this.Args[i] = new EventArgumentEditorData(EventArgument.Instance[functionCfg.ParameterTypes[i]]);
			}
		}

		// Token: 0x06004D7A RID: 19834 RVA: 0x00248AA8 File Offset: 0x00246CA8
		public EventInstructionEditorData(int funcId, params string[] args)
		{
			this.FuncId = funcId;
			int[] parameterTypes = this.FunctionConfig.ParameterTypes;
			int argCount = (parameterTypes != null) ? parameterTypes.Length : 0;
			bool flag = argCount != args.Length;
			if (flag)
			{
				throw new ArgumentException(string.Format("Argument count mismatch: {0} expected, {1} given.", argCount, args.Length));
			}
			this.Args = new EventArgumentEditorData[args.Length];
			for (int i = 0; i < this.Args.Length; i++)
			{
				this.Args[i] = new EventArgumentEditorData(args[i], true);
			}
		}

		// Token: 0x06004D7B RID: 19835 RVA: 0x00248B3C File Offset: 0x00246D3C
		public void AdjustIndention(EventInstructionEditorData prevInst)
		{
			bool flag = prevInst == null;
			if (flag)
			{
				this.Indent = 0;
			}
			else
			{
				EventFunctionItem prevFuncCfg = EventFunction.Instance[prevInst.FuncId];
				int indentChange = 0;
				bool indentNext = prevFuncCfg.IndentNext;
				if (indentNext)
				{
					indentChange++;
				}
				List<int> requiredPreviousCommands = this.FunctionConfig.RequiredPreviousCommands;
				bool flag2 = requiredPreviousCommands != null && requiredPreviousCommands.Count > 0;
				if (flag2)
				{
					indentChange--;
				}
				this.Indent = Math.Max(0, prevInst.Indent + indentChange);
			}
		}

		// Token: 0x06004D7C RID: 19836 RVA: 0x00248BBC File Offset: 0x00246DBC
		public bool FindStringInArg(string str, out string value)
		{
			value = null;
			foreach (EventArgumentEditorData arg in this.Args)
			{
				bool flag = !string.IsNullOrEmpty(arg.Value) && arg.Value.Contains(str);
				if (flag)
				{
					value = arg.Value;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004D7D RID: 19837 RVA: 0x00248C20 File Offset: 0x00246E20
		public string GetArgString(int index)
		{
			EventArgumentItem argConfig = EventArgument.Instance[this.FunctionConfig.ParameterTypes[index]];
			EventArgumentEditorData argData = this.Args[index];
			bool flag = argData.Value.IsNullOrEmpty();
			if (flag)
			{
				throw EventScriptCompilerException.CreateAtArg(25, argConfig, argData, index, null);
			}
			bool flag2 = !argData.IsExpression;
			if (flag2)
			{
				int templateId;
				bool flag3 = EventValue.Instance.RefNameMap.TryGetValue(argData.Value, out templateId) && templateId >= 0 && EventValue.Instance[templateId].EventArgument == argConfig.TemplateId;
				if (flag3)
				{
					EventValueItem valueCfg = EventValue.Instance[templateId];
					switch (valueCfg.Type)
					{
					case EEventValueType.Global:
						return valueCfg.Alias;
					case EEventValueType.Event:
						return valueCfg.ArgBoxKey.IsNullOrEmpty() ? valueCfg.Alias : valueCfg.ArgBoxKey;
					case EEventValueType.Constant:
					{
						int eventArgument = valueCfg.EventArgument;
						return (eventArgument == 5 || eventArgument == 4) ? ("\"" + valueCfg.ConstValue + "\"") : valueCfg.ConstValue;
					}
					}
				}
				bool flag4 = !string.IsNullOrEmpty(argConfig.ConfigTable);
				if (flag4)
				{
					IConfigData configTable = ConfigCollection.NameMap[argConfig.ConfigTable];
					int value;
					bool flag5 = !configTable.RefNameMap.TryGetValue(argData.Value, out value);
					if (flag5)
					{
						throw EventScriptCompilerException.CreateAtArg(26, argConfig, argData, index, null);
					}
					IEventArgumentCollectionFormatter formatter = configTable as IEventArgumentCollectionFormatter;
					bool flag6 = formatter != null;
					if (flag6)
					{
						return EventEditorScript.WrapText(formatter.ToArgString(value));
					}
					return value.ToString();
				}
				else
				{
					bool flag7 = argConfig.TemplateId == 91;
					if (flag7)
					{
						LanguageKey languageKey;
						bool flag8 = !Enum.TryParse<LanguageKey>(argData.Value, out languageKey);
						if (flag8)
						{
							throw EventScriptCompilerException.CreateAtArg(26, argConfig, argData, index, null);
						}
						return "\"" + argData.Value + "\"";
					}
					else
					{
						bool flag9 = argConfig.TemplateId == 22;
						if (flag9)
						{
							return SingletonObject.getInstance<EventEditorModel>().GetItemType(argData.Value).ToString();
						}
						bool flag10 = argConfig.TemplateId == 33;
						if (flag10)
						{
							return SingletonObject.getInstance<EventEditorModel>().GetWorldFunctionType(argData.Value).ToString();
						}
						bool flag11 = argConfig.TemplateId == 5;
						if (flag11)
						{
							string value2 = SingletonObject.getInstance<EventEditorModel>().GetEventGuidByEventNamePrioritizingGroup(Export.ExportingEventGroup, argData.Value);
							bool flag12 = string.IsNullOrEmpty(value2);
							if (flag12)
							{
								throw EventScriptCompilerException.CreateAtArg(27, argConfig, argData, index, null);
							}
							return "\"" + value2 + "\"";
						}
						else
						{
							bool flag13 = argConfig.TemplateId == 61;
							if (flag13)
							{
								AdventureEditorKit.UpdateElementCache();
								AdventureElementSnapshot element = AdventureEditorKit.GetElementFromCache(argData.Value);
								bool flag14 = element != null;
								if (flag14)
								{
									return element.Id.ToString();
								}
								throw EventScriptCompilerException.CreateAtArg(36, argConfig, argData, index, null);
							}
							else
							{
								bool flag15 = argConfig.TemplateId == 71;
								if (flag15)
								{
									foreach (AdventureMajorEventSnapshot data in AdventureEditorKit.GetMajorEvents())
									{
										bool flag16 = data.Name != null && data.Name.Equals(argData.Value);
										if (flag16)
										{
											return data.Id.ToString();
										}
									}
									throw EventScriptCompilerException.CreateAtArg(37, argConfig, argData, index, null);
								}
								bool flag17 = argConfig.TemplateId == 81;
								if (flag17)
								{
									foreach (AdventureSnapshot data2 in AdventureEditorKit.GetAdventures())
									{
										bool flag18 = data2.Name != null && data2.Name.Equals(argData.Value);
										if (flag18)
										{
											return data2.Id.ToString();
										}
									}
									throw EventScriptCompilerException.CreateAtArg(38, argConfig, argData, index, null);
								}
								string[] customEnumText = argConfig.CustomEnumText;
								bool flag19 = customEnumText != null && customEnumText.Length > 0;
								if (flag19)
								{
									int value3 = argConfig.CustomEnumText.IndexOf(argData.Value);
									int[] customEnumValues = argConfig.CustomEnumValues;
									bool flag20 = customEnumValues != null && customEnumValues.Length > 0;
									if (!flag20)
									{
										return value3.ToString();
									}
									bool flag21 = !argConfig.CustomEnumValues.CheckIndex(value3);
									if (flag21)
									{
										throw EventScriptCompilerException.CreateAtArg(28, argConfig, argData, index, null);
									}
									return argConfig.CustomEnumValues[value3].ToString();
								}
								else
								{
									bool flag22 = argConfig.TemplateId == 3;
									if (flag22)
									{
										bool flag23 = argData.Value == "0";
										if (flag23)
										{
											return "false";
										}
										bool flag24 = argData.Value == "1";
										if (flag24)
										{
											return "true";
										}
									}
								}
							}
						}
					}
				}
			}
			return argData.Value;
		}

		// Token: 0x06004D7E RID: 19838 RVA: 0x0024917C File Offset: 0x0024737C
		public bool IsArgumentReferenceType(int index)
		{
			EventArgumentItem argConfig = EventArgument.Instance[this.FunctionConfig.ParameterTypes[index]];
			return !string.IsNullOrEmpty(argConfig.ConfigTable);
		}

		// Token: 0x040035BA RID: 13754
		public int Indent;

		// Token: 0x040035BB RID: 13755
		public string AssignToVar;

		// Token: 0x040035BC RID: 13756
		public string FunctionName;

		// Token: 0x040035BD RID: 13757
		public EventArgumentEditorData[] Args;

		// Token: 0x040035BE RID: 13758
		public bool Reverse;
	}
}
