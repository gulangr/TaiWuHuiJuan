using System;
using FrameWork.UISystem.UIElements;
using TMPro;

namespace Game.Views.Legacy.AdventureEditor.Migrate
{
	// Token: 0x02000A0A RID: 2570
	public class AdventureElementMoveRuleEditorTemplate : AdventureAbstractListEditorTemplate
	{
		// Token: 0x04005FF7 RID: 24567
		public TMP_InputField moveSpeed;

		// Token: 0x04005FF8 RID: 24568
		public CDropdown elementMoveType;

		// Token: 0x04005FF9 RID: 24569
		public CButton condition;

		// Token: 0x04005FFA RID: 24570
		public AdventureEditorElementPicker targetElementId;

		// Token: 0x04005FFB RID: 24571
		public TMP_InputField targetElementTags;

		// Token: 0x04005FFC RID: 24572
		public TMP_InputField groupIds;
	}
}
