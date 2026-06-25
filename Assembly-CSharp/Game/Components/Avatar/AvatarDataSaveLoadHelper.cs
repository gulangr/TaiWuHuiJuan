using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FrameWork.ModSystem;
using GameData.Domains.Character.AvatarSystem;
using MoonSharp.Interpreter;

namespace Game.Components.Avatar
{
	// Token: 0x02000F81 RID: 3969
	[Obsolete("请使用 Game.Views.NewGame.CreationInfoHelper 或 Game.Views.NewGame.AvatarPresetHelper")]
	public class AvatarDataSaveLoadHelper
	{
		// Token: 0x0600B645 RID: 46661 RVA: 0x0052FF74 File Offset: 0x0052E174
		public static void Save(AvatarData avatarData, string filePath, Dictionary<string, string> additionInfoMap = null)
		{
			bool flag = AvatarDataSaveLoadHelper._encode == null;
			if (flag)
			{
				AvatarDataSaveLoadHelper._encode = new UTF8Encoding(false);
			}
			bool flag2 = AvatarDataSaveLoadHelper._avatarFieldInfos == null;
			if (flag2)
			{
				AvatarDataSaveLoadHelper.SetAvatarFieldInfos();
			}
			Table table = LuaGame.Instance.GetMainEnv().NewTable();
			AvatarDataSaveLoadHelper._avatarFieldInfos.ForEach(delegate(int index, FieldInfo field)
			{
				table.Set(field.Name, field.GetValue(avatarData));
				return false;
			});
			bool flag3 = additionInfoMap != null;
			if (flag3)
			{
				foreach (KeyValuePair<string, string> pair in additionInfoMap)
				{
					table.Set(pair.Key, pair.Value);
				}
			}
			LuaGame.LuaFunctionProxy luaFunction = new LuaGame.LuaFunctionProxy(LuaGame.Instance.GetGlobal().Get("table_to_file_string"));
			bool isNull = luaFunction.IsNull;
			if (!isNull)
			{
				string content = luaFunction.Call(new object[]
				{
					table,
					true
				}).CastToString();
				bool flag4 = !string.IsNullOrEmpty(content);
				if (flag4)
				{
					FileInfo fileInfo = new FileInfo(filePath);
					bool flag5 = !Directory.Exists(fileInfo.Directory.FullName);
					if (flag5)
					{
						Directory.CreateDirectory(fileInfo.Directory.FullName);
					}
					File.WriteAllText(filePath, content, AvatarDataSaveLoadHelper._encode);
				}
			}
		}

		// Token: 0x0600B646 RID: 46662 RVA: 0x005300F0 File Offset: 0x0052E2F0
		public static AvatarData Load(string filePath, ref Dictionary<string, string> additionInfoMap)
		{
			bool flag = AvatarDataSaveLoadHelper._encode == null;
			if (flag)
			{
				AvatarDataSaveLoadHelper._encode = new UTF8Encoding(false);
			}
			bool flag2 = AvatarDataSaveLoadHelper._avatarFieldInfos == null;
			if (flag2)
			{
				AvatarDataSaveLoadHelper.SetAvatarFieldInfos();
			}
			AvatarData avatarData = null;
			bool flag3 = File.Exists(filePath);
			if (flag3)
			{
				string content = File.ReadAllText(filePath, AvatarDataSaveLoadHelper._encode).Replace("\\n", "\n").Replace("\\t", "\t");
				object obj = LuaGame.Instance.DoString(content);
				Table table = obj as Table;
				bool flag4 = table != null;
				if (flag4)
				{
					List<string> avatarDataKeys = new List<string>();
					avatarData = new AvatarData();
					AvatarDataSaveLoadHelper._avatarFieldInfos.ForEach(delegate(int index, FieldInfo field)
					{
						bool flag7 = table.ContainsKey(field.Name);
						if (flag7)
						{
							avatarDataKeys.Add(field.Name);
							bool flag8 = field.FieldType == typeof(byte);
							if (flag8)
							{
								field.SetValue(avatarData, table.Get(field.Name));
							}
							else
							{
								bool flag9 = field.FieldType == typeof(short);
								if (flag9)
								{
									field.SetValue(avatarData, table.Get(field.Name));
								}
								else
								{
									bool flag10 = field.FieldType == typeof(ushort);
									if (flag10)
									{
										field.SetValue(avatarData, table.Get(field.Name));
									}
								}
							}
						}
						else
						{
							GLog.TagLog("AvatarLoad", "AvatarData table dose not contains key:" + field.Name + ",table file path is : " + filePath, Array.Empty<object>());
						}
						return false;
					});
					bool flag5 = additionInfoMap != null;
					if (flag5)
					{
						IEnumerable<string> keys = table.GetKeys<string>();
						foreach (string key in keys)
						{
							bool flag6 = !avatarDataKeys.Contains(key);
							if (flag6)
							{
								additionInfoMap.Add(key, table.Get(key));
							}
						}
					}
				}
			}
			return avatarData;
		}

		// Token: 0x0600B647 RID: 46663 RVA: 0x005302B0 File Offset: 0x0052E4B0
		public static AvatarData[] LoadDirectory(string directoryPath)
		{
			string[] files = Directory.GetFiles(directoryPath, ".twa");
			return files.ChangeArrType(delegate(string filePath)
			{
				Dictionary<string, string> infoMap = new Dictionary<string, string>();
				return AvatarDataSaveLoadHelper.Load(filePath, ref infoMap);
			});
		}

		// Token: 0x0600B648 RID: 46664 RVA: 0x005302F8 File Offset: 0x0052E4F8
		private static void SetAvatarFieldInfos()
		{
			AvatarDataSaveLoadHelper._avatarFieldInfos = typeof(AvatarData).GetFields(BindingFlags.Instance | BindingFlags.Public);
			List<FieldInfo> cacheFields = new List<FieldInfo>();
			for (int i = AvatarDataSaveLoadHelper._avatarFieldInfos.Length - 1; i >= 0; i--)
			{
				FieldInfo field = AvatarDataSaveLoadHelper._avatarFieldInfos[i];
				bool flag = field.GetCustomAttribute<NonSerializedAttribute>() == null;
				if (flag)
				{
					cacheFields.Add(field);
				}
			}
			AvatarDataSaveLoadHelper._avatarFieldInfos = cacheFields.ToArray();
		}

		// Token: 0x04008D70 RID: 36208
		private static Encoding _encode;

		// Token: 0x04008D71 RID: 36209
		private static FieldInfo[] _avatarFieldInfos;
	}
}
