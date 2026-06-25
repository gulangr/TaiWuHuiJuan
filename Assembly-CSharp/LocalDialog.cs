using System;
using System.Runtime.InteropServices;

// Token: 0x02000032 RID: 50
public class LocalDialog
{
	// Token: 0x060001C0 RID: 448
	[DllImport("Comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern bool GetOpenFileName([In] [Out] OpenFileName ofn);

	// Token: 0x060001C1 RID: 449 RVA: 0x0000B58C File Offset: 0x0000978C
	public static bool GetUnityOpenFileName([In] [Out] OpenFileName ofn)
	{
		return LocalDialog.GetOpenFileName(ofn);
	}

	// Token: 0x060001C2 RID: 450
	[DllImport("Comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern bool GetSaveFileName([In] [Out] OpenFileName ofn);

	// Token: 0x060001C3 RID: 451 RVA: 0x0000B5A4 File Offset: 0x000097A4
	public static bool GetUnitySaveFileName([In] [Out] OpenFileName ofn)
	{
		return LocalDialog.GetSaveFileName(ofn);
	}

	// Token: 0x060001C4 RID: 452
	[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr SHBrowseForFolder([In] [Out] OpenDirDialog ofn);

	// Token: 0x060001C5 RID: 453
	[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In] [Out] char[] fileName);

	// Token: 0x060001C6 RID: 454 RVA: 0x0000B5BC File Offset: 0x000097BC
	public static string GetUnitySaveDir(string title, string folder = "")
	{
		IntPtr pidlPtr = LocalDialog.SHBrowseForFolder(new OpenDirDialog
		{
			pszDisplayName = new string(new char[2000]),
			lpszTitle = title,
			ulFlags = 80U
		});
		char[] charArray = new char[2000];
		for (int i = 0; i < 2000; i++)
		{
			charArray[i] = '\0';
		}
		LocalDialog.SHGetPathFromIDList(pidlPtr, charArray);
		string fullDirPath = new string(charArray);
		return fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000B64C File Offset: 0x0000984C
	public static string GetUnitySelectFileName(string filter, string initialDir = "")
	{
		OpenFileName openFileName = new OpenFileName();
		openFileName.structSize = Marshal.SizeOf<OpenFileName>(openFileName);
		openFileName.filter = filter;
		openFileName.file = new string(new char[256]);
		openFileName.maxFile = openFileName.file.Length;
		openFileName.fileTitle = new string(new char[64]);
		openFileName.maxFileTitle = openFileName.fileTitle.Length;
		openFileName.initialDir = initialDir;
		openFileName.flags = 530440;
		return LocalDialog.GetUnityOpenFileName(openFileName) ? openFileName.file : string.Empty;
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000B6E8 File Offset: 0x000098E8
	public static string SelectSaveFilePath(string filter = "Any Files(*.*)\0*.*\0", string initialDir = "")
	{
		OpenFileName ofn = new OpenFileName();
		ofn.structSize = Marshal.SizeOf<OpenFileName>(ofn);
		ofn.filter = filter;
		ofn.file = new string(new char[256]);
		ofn.maxFile = ofn.file.Length;
		ofn.fileTitle = new string(new char[64]);
		ofn.maxFileTitle = ofn.fileTitle.Length;
		ofn.initialDir = initialDir;
		ofn.flags = 524288;
		return LocalDialog.GetSaveFileName(ofn) ? ofn.file : string.Empty;
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000B788 File Offset: 0x00009988
	public static string SelectLoadFilePath(string filter = "Any Files(*.*)\0*.*\0", string initialDir = "")
	{
		return LocalDialog.GetUnitySelectFileName(filter, initialDir);
	}
}
