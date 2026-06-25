using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FrameWork.ModSystem;
using GameData.Domains.Character.AvatarSystem;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x020007FA RID: 2042
	public static class NewGameSubPageAvatarPresetHelper
	{
		// Token: 0x060063B0 RID: 25520 RVA: 0x002DB0C4 File Offset: 0x002D92C4
		private static string GetCustomPresetsFilePath()
		{
			return Path.Combine(GameApp.GetArchiveDirPath(), "CustomPresets.lua");
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x002DB0D8 File Offset: 0x002D92D8
		public static void SaveCustomPresets(List<AvatarPreset> presets)
		{
			if (NewGameSubPageAvatarPresetHelper._encoding == null)
			{
				NewGameSubPageAvatarPresetHelper._encoding = new UTF8Encoding(false);
			}
			Table mainTable = LuaGame.Instance.GetMainEnv().NewTable();
			for (int i = 0; i < presets.Count; i++)
			{
				AvatarPreset preset = presets[i];
				Table presetTable = LuaGame.Instance.GetMainEnv().NewTable();
				AvatarDataLuaSerializer.SerializeToTable(preset.Data, presetTable);
				presetTable.Set("PresetName", preset.Name);
				presetTable.Set("IsTransGender", preset.IsTransGender);
				presetTable.Set("Gender", preset.Gender);
				mainTable.Set(i + 1, presetTable);
			}
			string content = NewGameSubPageAvatarPresetHelper.TableToFileString(mainTable);
			bool flag = !string.IsNullOrEmpty(content);
			if (flag)
			{
				string filePath = NewGameSubPageAvatarPresetHelper.GetCustomPresetsFilePath();
				FileInfo fileInfo = new FileInfo(filePath);
				bool flag2 = fileInfo.Directory != null && !Directory.Exists(fileInfo.Directory.FullName);
				if (flag2)
				{
					Directory.CreateDirectory(fileInfo.Directory.FullName);
				}
				File.WriteAllText(filePath, content, NewGameSubPageAvatarPresetHelper._encoding);
			}
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x002DB200 File Offset: 0x002D9400
		public static List<AvatarPreset> LoadCustomPresets()
		{
			List<AvatarPreset> presets = new List<AvatarPreset>();
			string filePath = NewGameSubPageAvatarPresetHelper.GetCustomPresetsFilePath();
			bool flag = !File.Exists(filePath);
			List<AvatarPreset> result;
			if (flag)
			{
				result = presets;
			}
			else
			{
				if (NewGameSubPageAvatarPresetHelper._encoding == null)
				{
					NewGameSubPageAvatarPresetHelper._encoding = new UTF8Encoding(false);
				}
				string content = File.ReadAllText(filePath, NewGameSubPageAvatarPresetHelper._encoding);
				result = NewGameSubPageAvatarPresetHelper.ParsePresetsContent(content);
			}
			return result;
		}

		// Token: 0x060063B3 RID: 25523 RVA: 0x002DB258 File Offset: 0x002D9458
		public static List<AvatarPreset> LoadBuiltinPresets()
		{
			TextAsset textAsset = ResLoader.SyncLoad<TextAsset>("RemakeResources/Data/FixedPresets");
			bool flag = textAsset == null;
			List<AvatarPreset> result;
			if (flag)
			{
				result = new List<AvatarPreset>();
			}
			else
			{
				List<AvatarPreset> presets = NewGameSubPageAvatarPresetHelper.ParsePresetsContent(textAsset.text);
				foreach (AvatarPreset preset in presets)
				{
					bool flag2 = preset.Data == null;
					if (!flag2)
					{
						preset.Data.SetGrowableElementShowingAbility(0, true);
						preset.Data.SetGrowableElementShowingState(0, true);
						preset.Data.SetGrowableElementShowingAbility(6, true);
						preset.Data.SetGrowableElementShowingState(6, true);
						preset.Data.SetGrowableElementShowingAbility(1, true);
						preset.Data.SetGrowableElementShowingState(1, true);
						preset.Data.SetGrowableElementShowingAbility(2, true);
						preset.Data.SetGrowableElementShowingState(2, true);
					}
				}
				result = presets;
			}
			return result;
		}

		// Token: 0x060063B4 RID: 25524 RVA: 0x002DB368 File Offset: 0x002D9568
		private static List<AvatarPreset> ParsePresetsContent(string content)
		{
			List<AvatarPreset> presets = new List<AvatarPreset>();
			bool flag = string.IsNullOrEmpty(content);
			List<AvatarPreset> result;
			if (flag)
			{
				result = presets;
			}
			else
			{
				content = content.Replace("\\n", "\n").Replace("\\t", "\t");
				Table mainTable = LuaGame.Instance.DoString(content) as Table;
				bool flag2 = mainTable == null;
				if (flag2)
				{
					result = presets;
				}
				else
				{
					for (int i = 1; i <= mainTable.Length; i++)
					{
						Table presetTable = mainTable.Get(i);
						bool flag3 = presetTable == null;
						if (!flag3)
						{
							AvatarData data = AvatarDataLuaSerializer.DeserializeFromTable(presetTable);
							AvatarPreset preset = new AvatarPreset
							{
								Name = (presetTable.Get("PresetName") ?? LanguageKey.LK_NewGame_Avatar_SubPage_Preset_DefaultName.TrFormat(i)),
								Data = data,
								IsTransGender = presetTable.GetOrDefault("IsTransGender", false),
								Gender = presetTable.GetOrDefault("Gender", data.Gender)
							};
							presets.Add(preset);
						}
					}
					result = presets;
				}
			}
			return result;
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x002DB492 File Offset: 0x002D9692
		public static bool CustomPresetsExist()
		{
			return File.Exists(NewGameSubPageAvatarPresetHelper.GetCustomPresetsFilePath());
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x002DB4A0 File Offset: 0x002D96A0
		private static string TableToFileString(Table table)
		{
			Closure luaFunc = LuaGame.Instance.GetGlobal().Get("table_to_file_string");
			bool flag = luaFunc == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				LuaGame.LuaFunctionProxy funcProxy = new LuaGame.LuaFunctionProxy(luaFunc);
				result = funcProxy.Call(new object[]
				{
					table,
					true
				}).CastToString();
			}
			return result;
		}

		// Token: 0x0400458C RID: 17804
		private const string PresetNameKey = "PresetName";

		// Token: 0x0400458D RID: 17805
		private const string IsTransGenderKey = "IsTransGender";

		// Token: 0x0400458E RID: 17806
		private const string GenderKey = "Gender";

		// Token: 0x0400458F RID: 17807
		private const string CustomPresetsFileName = "CustomPresets.lua";

		// Token: 0x04004590 RID: 17808
		public const int MaxCustomPresets = 21;

		// Token: 0x04004591 RID: 17809
		private static Encoding _encoding;
	}
}
