using System;
using System.IO;
using EventEditor.EventScript;
using GameData.Domains.TaiwuEvent;

namespace EventScript
{
	// Token: 0x020006DC RID: 1756
	public interface IEventScriptCompiler
	{
		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06005389 RID: 21385
		string Version { get; }

		// Token: 0x0600538A RID: 21386 RVA: 0x0026C318 File Offset: 0x0026A518
		void CompileVersion(BinaryWriter binaryWriter)
		{
			ulong versionUlong = ModManager.VersionStringToUlong(this.Version);
			binaryWriter.Write(versionUlong);
		}

		// Token: 0x0600538B RID: 21387 RVA: 0x0026C33C File Offset: 0x0026A53C
		unsafe void CompileScriptId(BinaryWriter binaryWriter, EventScriptId scriptId)
		{
			binaryWriter.Write((byte)scriptId.Type);
			bool flag = EventScriptId.IsAdventureType(scriptId.Type);
			if (flag)
			{
				binaryWriter.Write(scriptId.AdventureScriptRef.DebugInfo);
			}
			else
			{
				Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)16], 16);
				Span<byte> buffer = span;
				scriptId.EventScriptRef.Guid.TryWriteBytes(buffer);
				binaryWriter.Write(buffer);
				bool flag2 = EventScriptId.IsOptionType(scriptId.Type);
				if (flag2)
				{
					scriptId.EventScriptRef.SubGuid.TryWriteBytes(buffer);
					binaryWriter.Write(buffer);
				}
			}
		}

		// Token: 0x0600538C RID: 21388 RVA: 0x0026C3E4 File Offset: 0x0026A5E4
		void Compile(BinaryWriter binaryWriter, EventScriptId scriptId, EventScriptEditorData scriptData)
		{
			this.CompileScriptId(binaryWriter, scriptId);
			bool flag = EventScriptId.IsConditionList(scriptId.Type);
			if (flag)
			{
				this.CompileConditionList(binaryWriter, scriptData);
			}
			else
			{
				this.CompileEventScript(binaryWriter, scriptData);
			}
		}

		// Token: 0x0600538D RID: 21389 RVA: 0x0026C420 File Offset: 0x0026A620
		void CompileEventScript(BinaryWriter binaryWriter, EventScriptEditorData scriptData)
		{
			this.CompileScriptMetaData(binaryWriter, scriptData);
			binaryWriter.Write(scriptData.Instructions.Count);
			for (int i = 0; i < scriptData.Instructions.Count; i++)
			{
				EventInstructionEditorData inst = scriptData.Instructions[i];
				try
				{
					this.CompileInstruction(binaryWriter, inst);
				}
				catch (Exception e)
				{
					throw EventScriptCompilerException.CreateAtRow(i, inst, e);
				}
			}
		}

		// Token: 0x0600538E RID: 21390
		void CompileScriptMetaData(BinaryWriter binaryWriter, EventScriptEditorData scriptEditorData);

		// Token: 0x0600538F RID: 21391
		void CompileInstruction(BinaryWriter binaryWriter, EventInstructionEditorData instructionData);

		// Token: 0x06005390 RID: 21392 RVA: 0x0026C49C File Offset: 0x0026A69C
		void CompileConditionList(BinaryWriter binaryWriter, EventScriptEditorData scriptData)
		{
			this.CompileConditionListMetaData(binaryWriter, scriptData);
			binaryWriter.Write(scriptData.Instructions.Count);
			for (int i = 0; i < scriptData.Instructions.Count; i++)
			{
				EventInstructionEditorData inst = scriptData.Instructions[i];
				try
				{
					this.CompileCondition(binaryWriter, scriptData.Instructions[i]);
				}
				catch (Exception e)
				{
					throw EventScriptCompilerException.CreateAtRow(i, inst, e);
				}
			}
		}

		// Token: 0x06005391 RID: 21393
		void CompileConditionListMetaData(BinaryWriter binaryWriter, EventScriptEditorData scriptEditorData);

		// Token: 0x06005392 RID: 21394
		void CompileCondition(BinaryWriter binaryWriter, EventInstructionEditorData instructionData);

		// Token: 0x04003880 RID: 14464
		public const string ScriptExtension = "twes";
	}
}
