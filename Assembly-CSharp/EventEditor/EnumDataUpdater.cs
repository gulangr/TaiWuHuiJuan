using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using GameData.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000622 RID: 1570
	public static class EnumDataUpdater
	{
		// Token: 0x06004A53 RID: 19027 RVA: 0x0022CB0C File Offset: 0x0022AD0C
		public static void DoUpdate()
		{
			bool loginState = EnumDataUpdater.Login();
			bool flag = loginState;
			if (flag)
			{
				string downloadDir = Application.dataPath;
				bool flag2 = EnumDataUpdater.Download(downloadDir, "EventEditorData", "544355782186023781");
				if (flag2)
				{
					EnumDataUpdater.GenerateLuaConfigFile();
				}
				else
				{
					Debug.LogError("EventEditorData下载失败！！");
				}
				EnumDataUpdater.Logout();
			}
			else
			{
				Debug.LogError("由于登录到Driver失败，编辑器无法正常运行！");
			}
		}

		// Token: 0x06004A54 RID: 19028 RVA: 0x0022CB6C File Offset: 0x0022AD6C
		public static void GenerateLuaConfigFile()
		{
			string downloadDir = Application.dataPath;
			SingletonObject.getInstance<EventEditorModel>();
			string filePath = Path.Combine(downloadDir, "EventEditorData.xlsx");
			using (FileStream fStream = new FileStream(filePath, FileMode.Open))
			{
				IWorkbook workbook = WorkbookFactory.Create(fStream);
				string saveContent;
				GameData.Serializer.CommonObjectSerializer.Serialize<EnumDataUpdater.ConfigSet>(new EnumDataUpdater.ConfigSet
				{
					OptionConditionConfig = EnumDataUpdater.CreateEventOptionConditionConfigTable(workbook.GetSheet("EventOptionCondition"))
				}, out saveContent, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
				File.WriteAllText(EnumDataUpdater.GetLuaFilePath(), saveContent, new UTF8Encoding(false));
			}
			EnumDataUpdater.DoLoad();
			File.Delete(filePath);
		}

		// Token: 0x06004A55 RID: 19029 RVA: 0x0022CC04 File Offset: 0x0022AE04
		private static string GetLuaFilePath()
		{
			string modWorkingRootDirectory = ModManager.GetModEventEditorDataFolder();
			return Path.Combine(modWorkingRootDirectory, "EventEditorConfig.lua").PathFix();
		}

		// Token: 0x06004A56 RID: 19030 RVA: 0x0022CC2C File Offset: 0x0022AE2C
		private static bool LoadServerAccountConfig()
		{
			string configPath = Path.Combine(Application.dataPath, "Tmp", "ServerAcountConfig.json");
			bool flag = !File.Exists(configPath);
			bool result;
			if (flag)
			{
				Debug.LogError("未找到服务器账号配置文件，请在 " + configPath + " 创建文件，内容格式：{\"Account\": \"...\", \"Password\": \"...\"}");
				result = false;
			}
			else
			{
				try
				{
					string json = File.ReadAllText(configPath);
					JObject jsonObj = JObject.Parse(json);
					EnumDataUpdater.Account = (string)jsonObj["Account"];
					EnumDataUpdater.Password = (string)jsonObj["Password"];
					bool flag2 = string.IsNullOrEmpty(EnumDataUpdater.Account) || string.IsNullOrEmpty(EnumDataUpdater.Password);
					if (flag2)
					{
						Debug.LogError("服务器账号配置文件中Account或Password为空");
						result = false;
					}
					else
					{
						result = true;
					}
				}
				catch (Exception e)
				{
					Debug.LogError("读取服务器账号配置文件失败: " + e.Message);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06004A57 RID: 19031 RVA: 0x0022CD1C File Offset: 0x0022AF1C
		private static HttpWebResponse GetHttpResponse(string url, int timeout)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.Timeout = timeout;
			return (HttpWebResponse)request.GetResponse();
		}

		// Token: 0x06004A58 RID: 19032 RVA: 0x0022CD5C File Offset: 0x0022AF5C
		private static bool Login()
		{
			bool flag = !EnumDataUpdater.LoadServerAccountConfig();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(EnumDataUpdater.Account) || string.IsNullOrEmpty(EnumDataUpdater.Password);
				if (flag2)
				{
					result = false;
				}
				else
				{
					string realLoginUrl = string.Format(EnumDataUpdater.LoginUrl, EnumDataUpdater.Account, EnumDataUpdater.Password);
					HttpWebResponse response = EnumDataUpdater.GetHttpResponse(realLoginUrl, 6000);
					Stream myResponseStream = response.GetResponseStream();
					Stream stream = myResponseStream;
					if (stream == null)
					{
						throw new InvalidOperationException();
					}
					StreamReader myStreamReader = new StreamReader(stream, Encoding.UTF8);
					string loginResult = myStreamReader.ReadToEnd();
					myStreamReader.Close();
					myResponseStream.Close();
					bool flag3 = !string.IsNullOrEmpty(loginResult);
					if (flag3)
					{
						JObject jObj = JsonConvert.DeserializeObject<JObject>(loginResult);
						JToken jtoken = jObj["success"];
						bool flag4 = ((jtoken != null) ? jtoken.ToString().ToLower() : null) == "true";
						if (flag4)
						{
							JToken jtoken2 = jObj["data"];
							EnumDataUpdater._sid = (string)((jtoken2 != null) ? jtoken2["sid"] : null);
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06004A59 RID: 19033 RVA: 0x0022CE77 File Offset: 0x0022B077
		private static void Logout()
		{
			EnumDataUpdater.GetHttpResponse(EnumDataUpdater.LogoutUrl, 6000);
		}

		// Token: 0x06004A5A RID: 19034 RVA: 0x0022CE8C File Offset: 0x0022B08C
		private static bool Download(string dirFolder, string fileName, string filePathPatten)
		{
			string filePath = Path.Combine(dirFolder, fileName + ".xlsx");
			bool result;
			try
			{
				bool flag = File.Exists(filePath);
				if (flag)
				{
					File.Delete(filePath);
				}
				string downloadLink = string.Format(EnumDataUpdater.DownloadUrl, fileName, filePathPatten, EnumDataUpdater._sid);
				HttpWebResponse response = EnumDataUpdater.GetHttpResponse(downloadLink, 20000);
				using (Stream myResponseStream = response.GetResponseStream())
				{
					using (FileStream fileStream = File.Create(filePath))
					{
						byte[] buffer = new byte[1024];
						int bytesRead;
						do
						{
							bytesRead = ((myResponseStream != null) ? myResponseStream.Read(buffer, 0, buffer.Length) : 0);
							fileStream.Write(buffer, 0, bytesRead);
						}
						while (bytesRead > 0);
					}
				}
				Debug.Log(filePath);
				result = true;
			}
			catch (Exception e)
			{
				string str = "Failed to download excel file ";
				string str2 = "/r";
				Exception ex = e;
				throw new Exception(str + fileName + str2 + ((ex != null) ? ex.ToString() : null));
			}
			return result;
		}

		// Token: 0x06004A5B RID: 19035 RVA: 0x0022CFA8 File Offset: 0x0022B1A8
		private static bool IsStringCellAvailable(ICell cell)
		{
			return cell != null && cell.CellType == CellType.String && !cell.StringCellValue.IsNullOrEmpty();
		}

		// Token: 0x06004A5C RID: 19036 RVA: 0x0022CFD8 File Offset: 0x0022B1D8
		private static bool IsNumberCellAvailable(ICell cell)
		{
			return cell != null && cell.CellType == CellType.Numeric;
		}

		// Token: 0x06004A5D RID: 19037 RVA: 0x0022CFFC File Offset: 0x0022B1FC
		private static Dictionary<int, EnumDataUpdater.EventTriggerTypeInfo> CreateEventTriggerTypeInfoTable(ISheet sheet)
		{
			Dictionary<int, EnumDataUpdater.EventTriggerTypeInfo> trigger = new Dictionary<int, EnumDataUpdater.EventTriggerTypeInfo>();
			bool flag = sheet == null;
			Dictionary<int, EnumDataUpdater.EventTriggerTypeInfo> result;
			if (flag)
			{
				result = trigger;
			}
			else
			{
				for (int i = 2; i <= sheet.LastRowNum; i++)
				{
					IRow row = sheet.GetRow(i);
					bool flag2 = row == null;
					if (!flag2)
					{
						ICell idCell = row.GetCell(0);
						ICell cnCell = row.GetCell(1);
						ICell visibleCell = row.GetCell(2);
						ICell enCell = row.GetCell(3);
						bool flag3 = EnumDataUpdater.IsStringCellAvailable(cnCell) && EnumDataUpdater.IsStringCellAvailable(enCell) && EnumDataUpdater.IsNumberCellAvailable(idCell);
						if (flag3)
						{
							ICell namespaceCell = row.GetCell(4);
							EnumDataUpdater.EventTriggerTypeInfo element = new EnumDataUpdater.EventTriggerTypeInfo
							{
								Id = (short)idCell.NumericCellValue
							};
							bool flag4 = EnumDataUpdater.IsStringCellAvailable(namespaceCell);
							if (flag4)
							{
								element.NameSpace = namespaceCell.StringCellValue;
							}
							element.TriggerTypeLocal = cnCell.StringCellValue;
							element.TriggerTypeCode = enCell.StringCellValue;
							bool playerVisible = visibleCell == null || visibleCell.BooleanCellValue;
							element.PlayerVisible = playerVisible;
							EnumDataUpdater.EventTriggerTypeInfo eventTriggerTypeInfo = element;
							if (eventTriggerTypeInfo.TriggerArgumentInfoList == null)
							{
								eventTriggerTypeInfo.TriggerArgumentInfoList = new Dictionary<int, EnumDataUpdater.EventTriggerTypeInfo.Argument>();
							}
							for (int j = 5; j <= (int)row.LastCellNum; j += 3)
							{
								ICell typeCell = row.GetCell(j);
								bool flag5 = !EnumDataUpdater.IsStringCellAvailable(typeCell);
								if (flag5)
								{
									break;
								}
								ICell descCell = row.GetCell(j + 2);
								bool flag6 = !EnumDataUpdater.IsStringCellAvailable(descCell);
								if (flag6)
								{
									break;
								}
								ICell keyCell = row.GetCell(j + 1);
								bool flag7 = !EnumDataUpdater.IsStringCellAvailable(keyCell);
								if (flag7)
								{
									Debug.Log(descCell.StringCellValue + "没有指定参数盒子里存储的Key，请确认该参数信息是否本就不想指定参数盒子的key！");
								}
								element.TriggerArgumentInfoList[element.TriggerArgumentInfoList.Count + 1] = new EnumDataUpdater.EventTriggerTypeInfo.Argument
								{
									Type = typeCell.StringCellValue,
									Key = keyCell.StringCellValue,
									Desc = descCell.StringCellValue
								};
							}
							trigger[(int)element.Id] = element;
						}
					}
				}
				result = trigger;
			}
			return result;
		}

		// Token: 0x06004A5E RID: 19038 RVA: 0x0022D230 File Offset: 0x0022B430
		private static Dictionary<string, EventOptionConditionConfig> CreateEventOptionConditionConfigTable(ISheet sheet)
		{
			Dictionary<string, EventOptionConditionConfig> table = new Dictionary<string, EventOptionConditionConfig>();
			bool flag = sheet == null;
			Dictionary<string, EventOptionConditionConfig> result;
			if (flag)
			{
				result = table;
			}
			else
			{
				StringBuilder languageFileBuilder = new StringBuilder();
				int index = 0;
				for (int i = 2; i <= sheet.LastRowNum; i++)
				{
					IRow row = sheet.GetRow(i);
					bool flag2 = row == null;
					if (!flag2)
					{
						ICell codeCell = row.GetCell(0);
						ICell ctorCell = row.GetCell(1);
						ICell ctorArgCell = row.GetCell(2);
						ICell matcherArgCell = row.GetCell(3);
						ICell contentCell = row.GetCell(4);
						bool flag3 = EnumDataUpdater.IsStringCellAvailable(codeCell) && EnumDataUpdater.IsStringCellAvailable(ctorCell);
						if (flag3)
						{
							languageFileBuilder.AppendLine((contentCell == null) ? string.Empty : contentCell.StringCellValue);
							table[codeCell.StringCellValue] = new EventOptionConditionConfig
							{
								Id = index++,
								MatcherArg = ((matcherArgCell == null) ? string.Empty : matcherArgCell.StringCellValue),
								Constructor = ((ctorCell == null) ? string.Empty : ctorCell.StringCellValue),
								Content = ((contentCell == null) ? string.Empty : contentCell.StringCellValue),
								ConstructorArgs = ((ctorArgCell == null) ? string.Empty : ctorArgCell.StringCellValue)
							};
						}
					}
				}
				result = table;
			}
			return result;
		}

		// Token: 0x06004A5F RID: 19039 RVA: 0x0022D38C File Offset: 0x0022B58C
		private static string GetLuaConfigFileContent()
		{
			string filePathInsideProject = "EventEditor/EventEditorConfig";
			return Resources.Load<TextAsset>(filePathInsideProject).text;
		}

		// Token: 0x06004A60 RID: 19040 RVA: 0x0022D3B0 File Offset: 0x0022B5B0
		private static bool HasServerAccountConfig()
		{
			string configPath = Path.Combine(Application.dataPath, "Tmp", "ServerAcountConfig.json");
			bool flag = !File.Exists(configPath);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				try
				{
					string json = File.ReadAllText(configPath);
					JObject jsonObj = JObject.Parse(json);
					string account = (string)jsonObj["Account"];
					string password = (string)jsonObj["Password"];
					result = (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password));
				}
				catch
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06004A61 RID: 19041 RVA: 0x0022D44C File Offset: 0x0022B64C
		public static void DoLoad()
		{
			EnumDataUpdater.EnumDataConfig configTable;
			GameData.Serializer.CommonObjectSerializer.Deserialize<EnumDataUpdater.EnumDataConfig>(EnumDataUpdater.GetLuaConfigFileContent(), out configTable, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			EventEditorModel model = SingletonObject.getInstance<EventEditorModel>();
			model.SetForbidEventGroupMap(configTable.GroupConfig);
			model.EventOptionConditionsConfig = configTable.OptionConditionConfig;
		}

		// Token: 0x06004A62 RID: 19042 RVA: 0x0022D488 File Offset: 0x0022B688
		public static void DoExportToLuaWithEventGroup()
		{
			string filePath = EnumDataUpdater.GetLuaFilePath();
			bool flag = !File.Exists(filePath) && EnumDataUpdater.HasServerAccountConfig();
			if (flag)
			{
				EnumDataUpdater.DoUpdate();
			}
			GLog.TagLog("EventEditorModel", "LuaConfigFilePath=" + filePath, Array.Empty<object>());
			EnumDataUpdater.EnumDataConfig configTable;
			GameData.Serializer.CommonObjectSerializer.Deserialize<EnumDataUpdater.EnumDataConfig>(EnumDataUpdater.GetLuaConfigFileContent(), out configTable, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			Dictionary<string, string> eventGroupInfoMap = SingletonObject.getInstance<EventEditorModel>().EventGroupInfoDic;
			foreach (KeyValuePair<string, string> pair in eventGroupInfoMap)
			{
				configTable.GroupConfig[pair.Key] = pair.Value;
			}
			filePath = Path.Combine(Application.dataPath, "Resources/EventEditor/EventEditorConfig.txt");
			string saveContent;
			GameData.Serializer.CommonObjectSerializer.Serialize<EnumDataUpdater.EnumDataConfig>(configTable, out saveContent, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			File.WriteAllText(filePath, saveContent, new UTF8Encoding(false));
		}

		// Token: 0x0400337F RID: 13183
		private const string LuaFileName = "EventEditorConfig.lua";

		// Token: 0x04003380 RID: 13184
		public const string EventEditorDataFileName = "EventEditorData";

		// Token: 0x04003381 RID: 13185
		private const string FileId = "544355782186023781";

		// Token: 0x04003382 RID: 13186
		private static readonly string LoginUrl = "https://server.conchship.com.cn:4433/drive/webapi/auth.cgi?api=SYNO.API.Auth&version=3&method=login&account={0}&passwd={1}&session=FileStation&format=sid";

		// Token: 0x04003383 RID: 13187
		private static readonly string DownloadUrl = "https://server.conchship.com.cn:4433/drive/webapi/entry.cgi/{0}.xlsx?api=SYNO.Office.Export&method=download&version=1&session=FileStation&path=%22id%3A{1}%22&_sid={2}";

		// Token: 0x04003384 RID: 13188
		private static readonly string LogoutUrl = "https://server.conchship.com.cn:4433/drive/webapi/auth.cgi?api=SYNO.API.Auth&version=1&method=logout&session=FileStation";

		// Token: 0x04003385 RID: 13189
		private static string Account;

		// Token: 0x04003386 RID: 13190
		private static string Password;

		// Token: 0x04003387 RID: 13191
		private static string _sid;

		// Token: 0x020019FA RID: 6650
		internal class ConfigSet
		{
			// Token: 0x0400B475 RID: 46197
			public Dictionary<string, EventOptionConditionConfig> OptionConditionConfig;
		}

		// Token: 0x020019FB RID: 6651
		internal class EventTriggerTypeInfo
		{
			// Token: 0x0400B476 RID: 46198
			public short Id;

			// Token: 0x0400B477 RID: 46199
			public string NameSpace;

			// Token: 0x0400B478 RID: 46200
			public string TriggerTypeLocal;

			// Token: 0x0400B479 RID: 46201
			public string TriggerTypeCode;

			// Token: 0x0400B47A RID: 46202
			public bool PlayerVisible;

			// Token: 0x0400B47B RID: 46203
			public Dictionary<int, EnumDataUpdater.EventTriggerTypeInfo.Argument> TriggerArgumentInfoList;

			// Token: 0x020026C5 RID: 9925
			internal class Argument
			{
				// Token: 0x0400EB78 RID: 60280
				public string Type;

				// Token: 0x0400EB79 RID: 60281
				public string Key;

				// Token: 0x0400EB7A RID: 60282
				public string Desc;
			}
		}

		// Token: 0x020019FC RID: 6652
		internal class EnumDataConfig
		{
			// Token: 0x0400B47C RID: 46204
			public Dictionary<string, string> GroupConfig = new Dictionary<string, string>();

			// Token: 0x0400B47D RID: 46205
			public Dictionary<short, EventTriggerType> TriggerTypeConfig = new Dictionary<short, EventTriggerType>();

			// Token: 0x0400B47E RID: 46206
			public Dictionary<string, EventOptionConditionConfig> OptionConditionConfig = new Dictionary<string, EventOptionConditionConfig>();
		}
	}
}
