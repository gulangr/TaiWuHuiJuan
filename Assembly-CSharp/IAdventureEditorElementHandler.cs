using System;
using System.IO;
using UnityEngine;

// Token: 0x02000176 RID: 374
public interface IAdventureEditorElementHandler
{
	// Token: 0x060014BD RID: 5309
	void OnClick(FileInfo file, DirectoryInfo directory);

	// Token: 0x060014BE RID: 5310
	void OnDoubleClick(FileInfo file, DirectoryInfo directory);

	// Token: 0x060014BF RID: 5311
	void OnRightClick(FileInfo file, DirectoryInfo directory);

	// Token: 0x060014C0 RID: 5312
	void OnSelectEmpty();

	// Token: 0x060014C1 RID: 5313
	IAdventureEditorElementHandler.DragPostProcess OnDragValidate(GameObject dropTarget, FileInfo file, DirectoryInfo directory);

	// Token: 0x0200127F RID: 4735
	public enum DragPostProcess
	{
		// Token: 0x04009AAC RID: 39596
		Failed,
		// Token: 0x04009AAD RID: 39597
		Move,
		// Token: 0x04009AAE RID: 39598
		Stay,
		// Token: 0x04009AAF RID: 39599
		Delete
	}
}
