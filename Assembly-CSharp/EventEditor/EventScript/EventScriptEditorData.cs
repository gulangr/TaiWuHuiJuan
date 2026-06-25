using System;
using System.Collections.Generic;
using System.IO;
using Config;

namespace EventEditor.EventScript
{
	// Token: 0x02000658 RID: 1624
	[Serializable]
	public class EventScriptEditorData
	{
		// Token: 0x06004D67 RID: 19815 RVA: 0x0024844D File Offset: 0x0024664D
		public EventScriptEditorData()
		{
			this.Instructions = new List<EventInstructionEditorData>();
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x00248464 File Offset: 0x00246664
		public EventScriptEditorData(string text)
		{
			this.Instructions = new List<EventInstructionEditorData>();
			bool flag = string.IsNullOrEmpty(text);
			if (!flag)
			{
				using (StringReader stringReader = new StringReader(text))
				{
					for (string line = stringReader.ReadLine(); line != null; line = stringReader.ReadLine())
					{
						EventInstructionEditorData instruction = new EventInstructionEditorData(line);
						this.Instructions.Add(instruction);
					}
					stringReader.Close();
				}
			}
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x002484EC File Offset: 0x002466EC
		public void FindInstructions(string text, bool byFuncName, List<ValueTuple<int, string>> resultList)
		{
			if (byFuncName)
			{
				this.FindInstructionsByFuncName(text, resultList);
			}
			else
			{
				this.FindInstructionsByParameter(text, resultList);
			}
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x00248514 File Offset: 0x00246714
		public void FindInstructionsByFuncName(string funcName, List<ValueTuple<int, string>> resultList)
		{
			for (int i = 0; i < this.Instructions.Count; i++)
			{
				EventInstructionEditorData inst = this.Instructions[i];
				bool flag = inst.FunctionName.Contains(funcName);
				if (flag)
				{
					resultList.Add(new ValueTuple<int, string>(i, inst.FunctionName));
				}
				bool flag2 = inst.FunctionConfig.Name.Contains(funcName);
				if (flag2)
				{
					resultList.Add(new ValueTuple<int, string>(i, inst.FunctionConfig.Name));
				}
			}
		}

		// Token: 0x06004D6B RID: 19819 RVA: 0x002485A0 File Offset: 0x002467A0
		public void FindInstructionsByParameter(string paramStr, List<ValueTuple<int, string>> resultList)
		{
			for (int i = 0; i < this.Instructions.Count; i++)
			{
				EventInstructionEditorData inst = this.Instructions[i];
				string value;
				bool flag = inst.FindStringInArg(paramStr, out value);
				if (flag)
				{
					resultList.Add(new ValueTuple<int, string>(i, value));
				}
			}
		}

		// Token: 0x06004D6C RID: 19820 RVA: 0x002485F4 File Offset: 0x002467F4
		public void GetTransitions(string currGroupKey, List<string> resultList)
		{
			foreach (EventInstructionEditorData instruction in this.Instructions)
			{
				EventFunctionItem functionConfig = instruction.FunctionConfig;
				bool flag = !functionConfig.IsTransition;
				if (!flag)
				{
					for (int index = 0; index < functionConfig.ParameterTypes.Length; index++)
					{
						int paramType = functionConfig.ParameterTypes[index];
						bool flag2 = paramType != 5;
						if (!flag2)
						{
							EventArgumentEditorData arg = instruction.Args[index];
							bool flag3 = arg.Value.IsNullOrEmpty();
							if (!flag3)
							{
								bool isExpression = arg.IsExpression;
								if (isExpression)
								{
									bool flag4;
									if (arg.Value.Length > 2 && arg.Value[0] == '"')
									{
										string value = arg.Value;
										flag4 = (value[value.Length - 1] == '"');
									}
									else
									{
										flag4 = false;
									}
									bool flag5 = flag4;
									if (flag5)
									{
										resultList.Add(arg.Value.Substring(1, arg.Value.Length - 2));
									}
								}
								else
								{
									string guidStr = SingletonObject.getInstance<EventEditorModel>().GetEventGuidByEventNamePrioritizingGroup(currGroupKey, arg.Value);
									bool flag6 = !string.IsNullOrEmpty(guidStr);
									if (flag6)
									{
										resultList.Add(guidStr);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x040035B7 RID: 13751
		public readonly List<EventInstructionEditorData> Instructions;
	}
}
