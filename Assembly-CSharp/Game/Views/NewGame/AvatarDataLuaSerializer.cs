using System;
using System.Collections.Generic;
using System.Reflection;
using FrameWork.ModSystem;
using GameData.Domains.Character.AvatarSystem;
using MoonSharp.Interpreter;

namespace Game.Views.NewGame
{
	// Token: 0x020007DB RID: 2011
	public static class AvatarDataLuaSerializer
	{
		// Token: 0x0600621F RID: 25119 RVA: 0x002D0764 File Offset: 0x002CE964
		public static HashSet<string> GetAvatarFieldKeys()
		{
			AvatarDataLuaSerializer.EnsureFieldInfos();
			return AvatarDataLuaSerializer._avatarFieldKeys;
		}

		// Token: 0x06006220 RID: 25120 RVA: 0x002D0784 File Offset: 0x002CE984
		public static void SerializeToTable(AvatarData avatarData, Table table)
		{
			AvatarDataLuaSerializer.EnsureFieldInfos();
			foreach (FieldInfo field in AvatarDataLuaSerializer._avatarFieldInfos)
			{
				table.Set(field.Name, field.GetValue(avatarData));
			}
		}

		// Token: 0x06006221 RID: 25121 RVA: 0x002D07C8 File Offset: 0x002CE9C8
		public static AvatarData DeserializeFromTable(Table table)
		{
			AvatarDataLuaSerializer.EnsureFieldInfos();
			AvatarData avatarData = new AvatarData();
			foreach (FieldInfo field in AvatarDataLuaSerializer._avatarFieldInfos)
			{
				bool flag = !table.ContainsKey(field.Name);
				if (!flag)
				{
					bool flag2 = field.FieldType == typeof(byte);
					if (flag2)
					{
						field.SetValue(avatarData, table.Get(field.Name));
					}
					else
					{
						bool flag3 = field.FieldType == typeof(sbyte);
						if (flag3)
						{
							field.SetValue(avatarData, table.Get(field.Name));
						}
						else
						{
							bool flag4 = field.FieldType == typeof(short);
							if (flag4)
							{
								field.SetValue(avatarData, table.Get(field.Name));
							}
							else
							{
								bool flag5 = field.FieldType == typeof(ushort);
								if (flag5)
								{
									field.SetValue(avatarData, table.Get(field.Name));
								}
								else
								{
									bool flag6 = field.FieldType == typeof(bool);
									if (flag6)
									{
										field.SetValue(avatarData, table.Get(field.Name));
									}
								}
							}
						}
					}
				}
			}
			return avatarData;
		}

		// Token: 0x06006222 RID: 25122 RVA: 0x002D0938 File Offset: 0x002CEB38
		public static string Serialize(AvatarData avatarData)
		{
			Table table = LuaGame.Instance.GetMainEnv().NewTable();
			AvatarDataLuaSerializer.SerializeToTable(avatarData, table);
			return AvatarDataLuaSerializer.TableToFileString(table);
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x002D0968 File Offset: 0x002CEB68
		public static string Serialize(AvatarData avatarData, Dictionary<string, string> additionalInfo)
		{
			Table table = LuaGame.Instance.GetMainEnv().NewTable();
			AvatarDataLuaSerializer.SerializeToTable(avatarData, table);
			bool flag = additionalInfo != null;
			if (flag)
			{
				foreach (KeyValuePair<string, string> pair in additionalInfo)
				{
					table.Set(pair.Key, pair.Value);
				}
			}
			return AvatarDataLuaSerializer.TableToFileString(table);
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x002D09F8 File Offset: 0x002CEBF8
		public static AvatarData Deserialize(string luaContent, out Dictionary<string, string> additionalInfo)
		{
			additionalInfo = new Dictionary<string, string>();
			string content = luaContent.Replace("\\n", "\n").Replace("\\t", "\t");
			Table table = LuaGame.Instance.DoString(content) as Table;
			bool flag = table == null;
			AvatarData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				AvatarData avatarData = AvatarDataLuaSerializer.DeserializeFromTable(table);
				HashSet<string> avatarKeys = AvatarDataLuaSerializer.GetAvatarFieldKeys();
				foreach (string key in table.GetKeys<string>())
				{
					bool flag2 = !avatarKeys.Contains(key);
					if (flag2)
					{
						additionalInfo.Add(key, table.Get(key));
					}
				}
				result = avatarData;
			}
			return result;
		}

		// Token: 0x06006225 RID: 25125 RVA: 0x002D0ACC File Offset: 0x002CECCC
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

		// Token: 0x06006226 RID: 25126 RVA: 0x002D0B28 File Offset: 0x002CED28
		private static void EnsureFieldInfos()
		{
			bool flag = AvatarDataLuaSerializer._avatarFieldInfos != null;
			if (!flag)
			{
				FieldInfo[] allFields = typeof(AvatarData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				List<FieldInfo> validFields = new List<FieldInfo>();
				AvatarDataLuaSerializer._avatarFieldKeys = new HashSet<string>();
				foreach (FieldInfo field in allFields)
				{
					bool flag2 = field.GetCustomAttribute<NonSerializedAttribute>() == null;
					if (flag2)
					{
						validFields.Add(field);
						AvatarDataLuaSerializer._avatarFieldKeys.Add(field.Name);
					}
				}
				AvatarDataLuaSerializer._avatarFieldInfos = validFields.ToArray();
			}
		}

		// Token: 0x04004451 RID: 17489
		private static FieldInfo[] _avatarFieldInfos;

		// Token: 0x04004452 RID: 17490
		private static HashSet<string> _avatarFieldKeys;
	}
}
