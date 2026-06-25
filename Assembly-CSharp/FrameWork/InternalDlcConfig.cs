using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace FrameWork
{
	// Token: 0x02000FDC RID: 4060
	[CreateAssetMenu(fileName = "InternalDlcConfig", menuName = "ScriptableObjects/InternalDlcConfig", order = 1)]
	public class InternalDlcConfig : ScriptableObject
	{
		// Token: 0x0600B998 RID: 47512 RVA: 0x00549B08 File Offset: 0x00547D08
		public string GetDllPath()
		{
			return Path.Combine(Application.dataPath, this.DllPath);
		}

		// Token: 0x0600B999 RID: 47513 RVA: 0x00549B2C File Offset: 0x00547D2C
		public bool CheckDependDllExist()
		{
			string dllPath = this.GetDllPath();
			return File.Exists(dllPath);
		}

		// Token: 0x0600B99A RID: 47514 RVA: 0x00549B4C File Offset: 0x00547D4C
		public bool LoadDll()
		{
			bool flag = this.CheckDependDllExist();
			bool result;
			if (flag)
			{
				Assembly.LoadFile(this.GetDllPath());
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04008FA3 RID: 36771
		public bool AutoLoadDll = true;

		// Token: 0x04008FA4 RID: 36772
		public InternalDlcType InternalDlcType;

		// Token: 0x04008FA5 RID: 36773
		public string DllPath;
	}
}
