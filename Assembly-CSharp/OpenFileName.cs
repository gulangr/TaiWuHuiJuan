using System;
using System.Runtime.InteropServices;

// Token: 0x02000030 RID: 48
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
	// Token: 0x040000CE RID: 206
	public int structSize = 0;

	// Token: 0x040000CF RID: 207
	public IntPtr dlgOwner = IntPtr.Zero;

	// Token: 0x040000D0 RID: 208
	public IntPtr instance = IntPtr.Zero;

	// Token: 0x040000D1 RID: 209
	public string filter = null;

	// Token: 0x040000D2 RID: 210
	public string customFilter = null;

	// Token: 0x040000D3 RID: 211
	public int maxCustFilter = 0;

	// Token: 0x040000D4 RID: 212
	public int filterIndex = 0;

	// Token: 0x040000D5 RID: 213
	public string file = null;

	// Token: 0x040000D6 RID: 214
	public int maxFile = 0;

	// Token: 0x040000D7 RID: 215
	public string fileTitle = null;

	// Token: 0x040000D8 RID: 216
	public int maxFileTitle = 0;

	// Token: 0x040000D9 RID: 217
	public string initialDir = null;

	// Token: 0x040000DA RID: 218
	public string title = null;

	// Token: 0x040000DB RID: 219
	public int flags = 0;

	// Token: 0x040000DC RID: 220
	public short fileOffset = 0;

	// Token: 0x040000DD RID: 221
	public short fileExtension = 0;

	// Token: 0x040000DE RID: 222
	public string defExt = null;

	// Token: 0x040000DF RID: 223
	public IntPtr custData = IntPtr.Zero;

	// Token: 0x040000E0 RID: 224
	public IntPtr hook = IntPtr.Zero;

	// Token: 0x040000E1 RID: 225
	public string templateName = null;

	// Token: 0x040000E2 RID: 226
	public IntPtr reservedPtr = IntPtr.Zero;

	// Token: 0x040000E3 RID: 227
	public int reservedInt = 0;

	// Token: 0x040000E4 RID: 228
	public int flagsEx = 0;
}
