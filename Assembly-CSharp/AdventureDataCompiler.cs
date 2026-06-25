using System;
using System.Collections.Generic;
using System.IO;
using EventEditor.EventScript;
using EventScript;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Domains.TaiwuEvent;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class AdventureDataCompiler : IDataCompiler
{
	// Token: 0x060015EC RID: 5612 RVA: 0x000880CC File Offset: 0x000862CC
	public InstructionCompiled Compile(InstructionAdaptor adaptor, string debugInfo)
	{
		bool flag = adaptor == null || string.IsNullOrEmpty(adaptor.EventScriptJson);
		InstructionCompiled result;
		if (flag)
		{
			result = new InstructionCompiled(Array.Empty<byte>());
		}
		else
		{
			bool flag2 = !EventScriptId.IsAdventureType(adaptor.EventScriptType);
			if (flag2)
			{
				Debug.LogError(string.Format("{0} compiled failed with invalid event script type {1}", debugInfo, adaptor.EventScriptType));
				result = new InstructionCompiled(Array.Empty<byte>());
			}
			else
			{
				try
				{
					using (MemoryStream stream = new MemoryStream())
					{
						using (BinaryWriter writer = new BinaryWriter(stream))
						{
							EventScriptEditorData script = JsonConvert.DeserializeObject<EventScriptEditorData>(adaptor.EventScriptJson);
							EventScriptId scriptId = new EventScriptId(adaptor.EventScriptType, new AdventureScriptRef(debugInfo));
							this._scriptCompiler.Compile(writer, scriptId, script);
							writer.Flush();
							result = new InstructionCompiled(stream.ToArray());
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogError(string.Format("{0} compiled failed with error {1} by {2}", debugInfo, e, adaptor.EventScriptJson));
					result = new InstructionCompiled(Array.Empty<byte>());
				}
			}
		}
		return result;
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x00088200 File Offset: 0x00086400
	public AdventureLocalStringRef Compile(AdventureLocalString ls, string refName)
	{
		this.LocalStringMappings[refName] = ls;
		return new AdventureLocalStringRef
		{
			Key = refName
		};
	}

	// Token: 0x040011FA RID: 4602
	public readonly Dictionary<string, string> LocalStringMappings = new Dictionary<string, string>();

	// Token: 0x040011FB RID: 4603
	private readonly IEventScriptCompiler _scriptCompiler = new EventScriptCompiler_0_0_2_0();
}
