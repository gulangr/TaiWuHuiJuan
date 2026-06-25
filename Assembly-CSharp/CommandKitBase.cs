using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameData.Serializer;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class CommandKitBase
{
	// Token: 0x1700002D RID: 45
	// (get) Token: 0x060000C4 RID: 196 RVA: 0x000055F2 File Offset: 0x000037F2
	// (set) Token: 0x060000C5 RID: 197 RVA: 0x000055FA File Offset: 0x000037FA
	public byte Id { get; protected set; }

	// Token: 0x060000C6 RID: 198 RVA: 0x00005604 File Offset: 0x00003804
	private void ResetToDefault()
	{
		for (int i = 0; i < this.GroupCommand.Length; i++)
		{
			this.GroupCommand[i].Reset();
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000563C File Offset: 0x0000383C
	public static void Init()
	{
		string archiveDir = GameApp.GetArchiveDirPath();
		bool flag = !Directory.Exists(archiveDir);
		if (flag)
		{
			Directory.CreateDirectory(archiveDir);
		}
		CommandKitBase._saveFilePath = Path.Combine(archiveDir, "CustomKeyCommands.lua");
		CommandKitBase.Load();
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x0000567C File Offset: 0x0000387C
	private static void Load()
	{
		bool flag = !File.Exists(CommandKitBase._saveFilePath);
		if (!flag)
		{
			Dictionary<int, Dictionary<int, Dictionary<string, object>>> dict;
			GameData.Serializer.CommonObjectSerializer.Deserialize<Dictionary<int, Dictionary<int, Dictionary<string, object>>>>(File.ReadAllText(CommandKitBase._saveFilePath), out dict, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			foreach (CommandKitBase commandKit in CommandKitBase.CommandKitArray)
			{
				Dictionary<int, Dictionary<string, object>> kit;
				bool flag2 = dict.TryGetValue((int)commandKit.Id, out kit);
				if (flag2)
				{
					foreach (HotKeyCommand t in commandKit.GroupCommand)
					{
						byte id = t.Id;
						Dictionary<string, object> command;
						bool flag3 = kit.TryGetValue((int)id, out command);
						if (flag3)
						{
							KeyCode key = (KeyCode)int.Parse(command["Key"].ToString());
							KeyCode fnKey = KeyCode.None;
							object functionKey;
							bool flag4 = command.TryGetValue("FunctionKey", out functionKey);
							if (flag4)
							{
								fnKey = (KeyCode)int.Parse(functionKey.ToString());
							}
							t.SetCustomKey(key, fnKey, false);
							KeyCode mouseKey = KeyCode.None;
							object MouseKey;
							bool flag5 = command.TryGetValue("MouseKey", out MouseKey);
							if (flag5)
							{
								mouseKey = (KeyCode)int.Parse(MouseKey.ToString());
							}
							KeyCode fnMouseKey = KeyCode.None;
							object FnMouseKey;
							bool flag6 = command.TryGetValue("FunctionMouseKey", out FnMouseKey);
							if (flag6)
							{
								fnMouseKey = (KeyCode)int.Parse(FnMouseKey.ToString());
							}
							t.SetCustomKey(mouseKey, fnMouseKey, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x000057DC File Offset: 0x000039DC
	public static void SaveHotKeyConfig()
	{
		bool globalNeedSave = false;
		Dictionary<int, Dictionary<int, Dictionary<string, object>>> main = new Dictionary<int, Dictionary<int, Dictionary<string, object>>>();
		Dictionary<int, Dictionary<string, object>> kit = new Dictionary<int, Dictionary<string, object>>();
		for (int i = 0; i < CommandKitBase.CommandKitArray.Length; i++)
		{
			bool hasCustomKey = false;
			CommandKitBase commandKit = CommandKitBase.CommandKitArray[i];
			for (int j = 0; j < commandKit.GroupCommand.Length; j++)
			{
				ValueTuple<bool, List<KeyCode>> saveInfo = commandKit.GroupCommand[j].GetSaveInfo();
				bool needSave = saveInfo.Item1;
				List<KeyCode> saveList = saveInfo.Item2;
				bool flag = needSave;
				if (flag)
				{
					hasCustomKey = true;
					Dictionary<string, object> command = new Dictionary<string, object>();
					command["Key"] = (short)saveList[0];
					bool flag2 = saveList.Count > 1;
					if (flag2)
					{
						command["FunctionKey"] = (short)saveList[1];
					}
					bool flag3 = saveList.Count > 2;
					if (flag3)
					{
						command["MouseKey"] = (short)saveList[2];
					}
					bool flag4 = saveList.Count > 3;
					if (flag4)
					{
						command["FunctionMouseKey"] = (short)saveList[3];
					}
					command["Command"] = commandKit.GroupCommand[j].ToString();
					kit[(int)commandKit.GroupCommand[j].Id] = command;
				}
			}
			bool flag5 = hasCustomKey;
			if (flag5)
			{
				globalNeedSave = true;
				main[(int)commandKit.Id] = kit;
				kit = new Dictionary<int, Dictionary<string, object>>();
			}
		}
		bool flag6 = globalNeedSave;
		if (flag6)
		{
			string saveContent;
			GameData.Serializer.CommonObjectSerializer.Serialize<Dictionary<int, Dictionary<int, Dictionary<string, object>>>>(main, out saveContent, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			File.WriteAllText(CommandKitBase._saveFilePath, saveContent, Encoding.UTF8);
		}
		else
		{
			bool flag7 = File.Exists(CommandKitBase._saveFilePath);
			if (flag7)
			{
				File.Delete(CommandKitBase._saveFilePath);
			}
		}
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000059B4 File Offset: 0x00003BB4
	public static void Reset()
	{
		for (int i = 0; i < CommandKitBase.CommandKitArray.Length; i++)
		{
			CommandKitBase.CommandKitArray[i].ResetToDefault();
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060000CB RID: 203 RVA: 0x000059E7 File Offset: 0x00003BE7
	public virtual bool ShowInSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000059EA File Offset: 0x00003BEA
	public static void SetDisable(bool isDisable)
	{
		CommandKitBase._disable = isDisable;
	}

	// Token: 0x060000CD RID: 205 RVA: 0x000059F4 File Offset: 0x00003BF4
	public static bool GetDisable()
	{
		return CommandKitBase._disable;
	}

	// Token: 0x04000075 RID: 117
	public LanguageKey GroupDescLanguageId;

	// Token: 0x04000076 RID: 118
	public HotKeyCommand[] GroupCommand;

	// Token: 0x04000077 RID: 119
	public HotKeyCommand[] TipDisplayCommandArray;

	// Token: 0x04000078 RID: 120
	private static string _saveFilePath;

	// Token: 0x04000079 RID: 121
	public const byte Common = 1;

	// Token: 0x0400007A RID: 122
	public const byte EventEditor = 2;

	// Token: 0x0400007B RID: 123
	public const byte Combat = 3;

	// Token: 0x0400007C RID: 124
	public const byte Map = 4;

	// Token: 0x0400007D RID: 125
	public const byte EventWindow = 5;

	// Token: 0x0400007E RID: 126
	public const byte Encyclopedia = 6;

	// Token: 0x0400007F RID: 127
	public const byte Adventure = 7;

	// Token: 0x04000080 RID: 128
	public const byte TabSwitch = 8;

	// Token: 0x04000081 RID: 129
	public const byte Tips = 9;

	// Token: 0x04000082 RID: 130
	public const byte MainInterface = 10;

	// Token: 0x04000083 RID: 131
	public const byte MainInterfaceFunction = 11;

	// Token: 0x04000084 RID: 132
	public const byte CharacterMenu = 12;

	// Token: 0x04000085 RID: 133
	public const byte CombatBehavior = 13;

	// Token: 0x04000086 RID: 134
	public static CommandKitBase CommonCommandKit = new CommonCommandKit();

	// Token: 0x04000087 RID: 135
	public static CommandKitBase CombatCommandKit = new CombatCommandKit();

	// Token: 0x04000088 RID: 136
	public static CommandKitBase MapCommandKit = new MapCommandKit();

	// Token: 0x04000089 RID: 137
	public static CommandKitBase EventWindowKit = new EventWindowCommandKit();

	// Token: 0x0400008A RID: 138
	public static CommandKitBase TabSwitchCommandKit = new TabSwitchCommandKit();

	// Token: 0x0400008B RID: 139
	public static CommandKitBase TipsCommandKit = new TipsCommandKit();

	// Token: 0x0400008C RID: 140
	public static CommandKitBase MainInterfaceCommandKit = new MainInterfaceCommandKit();

	// Token: 0x0400008D RID: 141
	public static CommandKitBase MainInterfaceFunctionCommandKit = new MainInterfaceFunctionCommandKit();

	// Token: 0x0400008E RID: 142
	public static CommandKitBase CharacterMenuCommandKit = new CharacterMenuCommandKit();

	// Token: 0x0400008F RID: 143
	public static CommandKitBase CombatBehaviorCommandKit = new CombatBehaviorCommandKit();

	// Token: 0x04000090 RID: 144
	private static bool _disable;

	// Token: 0x04000091 RID: 145
	public static readonly CommandKitBase[] CommandKitArray = new CommandKitBase[]
	{
		CommandKitBase.CommonCommandKit,
		CommandKitBase.CombatCommandKit,
		CommandKitBase.MapCommandKit,
		CommandKitBase.EventWindowKit,
		CommandKitBase.TabSwitchCommandKit,
		CommandKitBase.TipsCommandKit,
		CommandKitBase.MainInterfaceCommandKit,
		CommandKitBase.MainInterfaceFunctionCommandKit,
		CommandKitBase.CharacterMenuCommandKit,
		CommandKitBase.CombatBehaviorCommandKit
	};
}
