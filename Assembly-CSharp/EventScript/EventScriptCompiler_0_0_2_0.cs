using System;
using System.Collections.Generic;
using System.IO;
using Config;
using EventEditor.EventScript;

namespace EventScript
{
	// Token: 0x020006DB RID: 1755
	public class EventScriptCompiler_0_0_2_0 : IEventScriptCompiler
	{
		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06005383 RID: 21379 RVA: 0x0026C030 File Offset: 0x0026A230
		public string Version
		{
			get
			{
				return "0.0.2.0";
			}
		}

		// Token: 0x06005384 RID: 21380 RVA: 0x0026C038 File Offset: 0x0026A238
		void IEventScriptCompiler.CompileScriptMetaData(BinaryWriter binaryWriter, EventScriptEditorData scriptEditorData)
		{
			Dictionary<string, int> labels = new Dictionary<string, int>();
			for (int index = 0; index < scriptEditorData.Instructions.Count; index++)
			{
				EventInstructionEditorData inst = scriptEditorData.Instructions[index];
				bool flag = inst.FuncId == 7;
				if (flag)
				{
					labels.Add(inst.Args[0].Value, index);
				}
			}
			binaryWriter.Write(labels.Count);
			foreach (KeyValuePair<string, int> pair in labels)
			{
				binaryWriter.Write(pair.Key);
				binaryWriter.Write(pair.Value);
			}
		}

		// Token: 0x06005385 RID: 21381 RVA: 0x0026C104 File Offset: 0x0026A304
		void IEventScriptCompiler.CompileInstruction(BinaryWriter binaryWriter, EventInstructionEditorData instructionData)
		{
			binaryWriter.Write(instructionData.Indent);
			binaryWriter.Write(instructionData.AssignToVar ?? string.Empty);
			binaryWriter.Write(instructionData.FuncId);
			EventArgumentEditorData[] args = instructionData.Args;
			binaryWriter.Write((args != null) ? args.Length : 0);
			bool flag = instructionData.FuncId == 15;
			if (flag)
			{
				binaryWriter.Write("\"string.Empty\"");
			}
			else
			{
				EventFunctionItem config = EventFunction.Instance[instructionData.FuncId];
				bool flag2 = instructionData.Args == null || instructionData.Args.Length != config.ParameterTypes.Length;
				if (flag2)
				{
					throw new Exception("Instruction argument count mismatch. Please edit.");
				}
				EventArgumentEditorData[] args2 = instructionData.Args;
				bool flag3 = args2 == null || args2.Length <= 0;
				if (!flag3)
				{
					for (int i = 0; i < instructionData.Args.Length; i++)
					{
						binaryWriter.Write(instructionData.GetArgString(i) ?? string.Empty);
					}
				}
			}
		}

		// Token: 0x06005386 RID: 21382 RVA: 0x0026C20C File Offset: 0x0026A40C
		void IEventScriptCompiler.CompileConditionListMetaData(BinaryWriter binaryWriter, EventScriptEditorData scriptEditorData)
		{
		}

		// Token: 0x06005387 RID: 21383 RVA: 0x0026C210 File Offset: 0x0026A410
		void IEventScriptCompiler.CompileCondition(BinaryWriter binaryWriter, EventInstructionEditorData instructionData)
		{
			binaryWriter.Write(instructionData.Indent);
			binaryWriter.Write(instructionData.Reverse);
			binaryWriter.Write(instructionData.FuncId);
			EventArgumentEditorData[] args = instructionData.Args;
			binaryWriter.Write((args != null) ? args.Length : 0);
			bool flag = instructionData.FuncId == 15;
			if (flag)
			{
				binaryWriter.Write(string.Empty);
			}
			else
			{
				EventFunctionItem config = EventFunction.Instance[instructionData.FuncId];
				bool flag2 = instructionData.Args == null || instructionData.Args.Length != config.ParameterTypes.Length;
				if (flag2)
				{
					throw new Exception("Instruction argument count mismatch. Please edit.");
				}
				EventArgumentEditorData[] args2 = instructionData.Args;
				bool flag3 = args2 == null || args2.Length <= 0;
				if (!flag3)
				{
					for (int i = 0; i < instructionData.Args.Length; i++)
					{
						binaryWriter.Write(instructionData.GetArgString(i) ?? string.Empty);
					}
				}
			}
		}
	}
}
