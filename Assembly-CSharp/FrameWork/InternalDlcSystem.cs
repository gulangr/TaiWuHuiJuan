using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FrameWork
{
	// Token: 0x02000FDE RID: 4062
	public class InternalDlcSystem : ISingletonInit, IDisposable
	{
		// Token: 0x0600B99C RID: 47516 RVA: 0x00549B8A File Offset: 0x00547D8A
		public void Dispose()
		{
		}

		// Token: 0x0600B99D RID: 47517 RVA: 0x00549B8D File Offset: 0x00547D8D
		public void Init()
		{
			this.internalDlcConfigDict = new Dictionary<InternalDlcType, InternalDlcConfig>();
			this.LoadAllInternalDlc();
		}

		// Token: 0x0600B99E RID: 47518 RVA: 0x00549BA4 File Offset: 0x00547DA4
		public void LoadInternalDlc(InternalDlcType internalDlcType)
		{
			bool flag = this.internalDlcConfigDict.ContainsKey(internalDlcType);
			if (flag)
			{
				this.internalDlcConfigDict[internalDlcType].LoadDll();
			}
			else
			{
				Debug.LogWarning(string.Format("LoadInternalDlc: {0} not in Configs!", internalDlcType));
			}
		}

		// Token: 0x0600B99F RID: 47519 RVA: 0x00549BF0 File Offset: 0x00547DF0
		public bool IsDllExist(InternalDlcType internalDlcType)
		{
			bool flag = this.internalDlcConfigDict.ContainsKey(internalDlcType);
			return flag && this.internalDlcConfigDict[internalDlcType].CheckDependDllExist();
		}

		// Token: 0x0600B9A0 RID: 47520 RVA: 0x00549C28 File Offset: 0x00547E28
		private void LoadAllInternalDlc()
		{
			string folderPath = Path.Combine(Application.dataPath, this.ConfigPath);
			string[] files = Directory.GetFiles(folderPath, "*.asset", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				string filePath = folderPath + file;
				ResLoader.Load<InternalDlcConfig>(filePath, delegate(InternalDlcConfig internalDlcConfig)
				{
					bool flag = this.internalDlcConfigDict.ContainsKey(internalDlcConfig.InternalDlcType);
					if (flag)
					{
						this.internalDlcConfigDict[internalDlcConfig.InternalDlcType] = internalDlcConfig;
					}
					else
					{
						this.internalDlcConfigDict.Add(internalDlcConfig.InternalDlcType, internalDlcConfig);
					}
					bool autoLoadDll = internalDlcConfig.AutoLoadDll;
					if (autoLoadDll)
					{
						internalDlcConfig.LoadDll();
					}
				}, null, false);
			}
		}

		// Token: 0x04008FA8 RID: 36776
		private readonly string ConfigPath = "RemakeResources/InternalDlcConfigs";

		// Token: 0x04008FA9 RID: 36777
		private Dictionary<InternalDlcType, InternalDlcConfig> internalDlcConfigDict;
	}
}
