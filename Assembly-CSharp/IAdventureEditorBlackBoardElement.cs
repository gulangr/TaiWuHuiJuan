using System;
using GameData.Adventure.Editor;

// Token: 0x0200016C RID: 364
public interface IAdventureEditorBlackBoardElement : IAdventureBlackBoardElement<EAdventureEditType>
{
	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06001453 RID: 5203 RVA: 0x0007ED2B File Offset: 0x0007CF2B
	bool AutoLoad
	{
		get
		{
			return true;
		}
	}
}
