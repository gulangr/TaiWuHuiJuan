using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameData.Domains.Character.AvatarSystem;

namespace Game.Views.NewGame
{
	// Token: 0x020007DF RID: 2015
	public static class CreationInfoHelper
	{
		// Token: 0x0600623D RID: 25149 RVA: 0x002D1382 File Offset: 0x002CF582
		private static string GetFilePath()
		{
			return Path.Combine(GameApp.GetArchiveDirPath(), "CreationInfo.lua");
		}

		// Token: 0x0600623E RID: 25150 RVA: 0x002D1394 File Offset: 0x002CF594
		public static void Save(AvatarData avatarData, Dictionary<string, string> additionalInfo = null)
		{
			if (CreationInfoHelper._encoding == null)
			{
				CreationInfoHelper._encoding = new UTF8Encoding(false);
			}
			string content = AvatarDataLuaSerializer.Serialize(avatarData, additionalInfo);
			bool flag = string.IsNullOrEmpty(content);
			if (!flag)
			{
				string filePath = CreationInfoHelper.GetFilePath();
				FileInfo fileInfo = new FileInfo(filePath);
				bool flag2 = fileInfo.Directory != null && !Directory.Exists(fileInfo.Directory.FullName);
				if (flag2)
				{
					Directory.CreateDirectory(fileInfo.Directory.FullName);
				}
				File.WriteAllText(filePath, content, CreationInfoHelper._encoding);
			}
		}

		// Token: 0x0600623F RID: 25151 RVA: 0x002D1418 File Offset: 0x002CF618
		public static AvatarData Load(out Dictionary<string, string> additionalInfo)
		{
			additionalInfo = new Dictionary<string, string>();
			string filePath = CreationInfoHelper.GetFilePath();
			bool flag = !File.Exists(filePath);
			AvatarData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				if (CreationInfoHelper._encoding == null)
				{
					CreationInfoHelper._encoding = new UTF8Encoding(false);
				}
				string content = File.ReadAllText(filePath, CreationInfoHelper._encoding);
				result = AvatarDataLuaSerializer.Deserialize(content, out additionalInfo);
			}
			return result;
		}

		// Token: 0x06006240 RID: 25152 RVA: 0x002D146D File Offset: 0x002CF66D
		public static bool Exists()
		{
			return File.Exists(CreationInfoHelper.GetFilePath());
		}

		// Token: 0x0400446C RID: 17516
		private const string FileName = "CreationInfo.lua";

		// Token: 0x0400446D RID: 17517
		private static Encoding _encoding;
	}
}
