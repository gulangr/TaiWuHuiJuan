using System;

// Token: 0x02000174 RID: 372
public interface IAdventureEditorBlockElementHandler
{
	// Token: 0x060014A7 RID: 5287
	void OnClick(AdventureEditorBlockElementPayload payload, bool isDecorate);

	// Token: 0x060014A8 RID: 5288
	void OnDoubleClick(AdventureEditorBlockElementPayload payload, bool isDecorate);

	// Token: 0x060014A9 RID: 5289
	void OnRightClick(AdventureEditorBlockElementPayload payload, bool isDecorate);

	// Token: 0x060014AA RID: 5290
	void OnSelectEmpty();
}
