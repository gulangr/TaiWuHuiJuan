using System;
using System.Runtime.InteropServices;

// Token: 0x02000031 RID: 49
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDirDialog
{
	// Token: 0x040000E5 RID: 229
	public IntPtr hwndOwner = IntPtr.Zero;

	// Token: 0x040000E6 RID: 230
	public IntPtr pidlRoot = IntPtr.Zero;

	// Token: 0x040000E7 RID: 231
	public string pszDisplayName = null;

	// Token: 0x040000E8 RID: 232
	public string lpszTitle = null;

	// Token: 0x040000E9 RID: 233
	public uint ulFlags = 0U;

	// Token: 0x040000EA RID: 234
	public IntPtr lpfn = IntPtr.Zero;

	// Token: 0x040000EB RID: 235
	public IntPtr lParam = IntPtr.Zero;

	// Token: 0x040000EC RID: 236
	public int iImage = 0;
}
