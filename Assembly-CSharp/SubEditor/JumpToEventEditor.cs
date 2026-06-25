using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace SubEditor
{
	// Token: 0x0200069B RID: 1691
	public class JumpToEventEditor : MonoBehaviour
	{
		// Token: 0x06004F52 RID: 20306 RVA: 0x00251D9E File Offset: 0x0024FF9E
		private void OnEnable()
		{
			this.eventEditorBtn.ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("EventId", this.inputFieldGuid.text);
				argBox.Set("FromAdventureRemake", true);
				UIElement.EventEditor.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.EventEditor, true);
			});
		}

		// Token: 0x04003690 RID: 13968
		public CButton eventEditorBtn;

		// Token: 0x04003691 RID: 13969
		public TMP_InputField inputFieldGuid;
	}
}
