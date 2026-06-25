using System;
using FrameWork.UISystem.UIElements;
using TMPro;

namespace Game.Views.Legacy.AdventureEditor.Migrate
{
	// Token: 0x02000A0B RID: 2571
	public class AdventureElementTriggerEventEditorTemplate : AdventureAbstractListEditorTemplate
	{
		// Token: 0x04005FFD RID: 24573
		public CToggle onlyOnce;

		// Token: 0x04005FFE RID: 24574
		public TMP_InputField guid;

		// Token: 0x04005FFF RID: 24575
		public TMP_InputField comment;

		// Token: 0x04006000 RID: 24576
		public CDropdown triggerType;
	}
}
