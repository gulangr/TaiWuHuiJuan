using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008D6 RID: 2262
	public class ModSettingGroup : MonoBehaviour
	{
		// Token: 0x06006C51 RID: 27729 RVA: 0x0031F06C File Offset: 0x0031D26C
		public void Refresh(int index, int listCount, string title, bool isSelected, Action<int> onClick, Action<int, int> onMove, Action<int, string> onRename, Action<int> onDelete)
		{
			this.textTitle.text = title;
			this.objSelected.SetActive(isSelected);
			this.button.ClearAndAddListener(delegate
			{
				onClick(index);
			});
			this.buttonPrev.interactable = (index > 0);
			this.buttonPrev.ClearAndAddListener(delegate
			{
				onMove(index, index - 1);
			});
			this.buttonNext.interactable = (index < listCount - 1);
			this.buttonNext.ClearAndAddListener(delegate
			{
				onMove(index, index + 1);
			});
			this.buttonRename.ClearAndAddListener(new Action(this.EnterRename));
			this.buttonDelete.ClearAndAddListener(delegate
			{
				onDelete(index);
			});
			this.inputField.SetTextWithoutNotify(title);
			this.inputField.onEndEdit.RemoveAllListeners();
			this.inputField.onEndEdit.AddListener(delegate(string value)
			{
				onRename(index, value.Trim());
			});
			this.ExitRename();
		}

		// Token: 0x06006C52 RID: 27730 RVA: 0x0031F1A8 File Offset: 0x0031D3A8
		private void EnterRename()
		{
			this.rootNormal.gameObject.SetActive(false);
			this.inputField.gameObject.SetActive(true);
			EventSystem.current.SetSelectedGameObject(this.inputField.gameObject);
		}

		// Token: 0x06006C53 RID: 27731 RVA: 0x0031F1E5 File Offset: 0x0031D3E5
		private void ExitRename()
		{
			this.rootNormal.gameObject.SetActive(true);
			this.inputField.gameObject.SetActive(false);
		}

		// Token: 0x04004E91 RID: 20113
		[SerializeField]
		private CButton button;

		// Token: 0x04004E92 RID: 20114
		[SerializeField]
		private CButton buttonPrev;

		// Token: 0x04004E93 RID: 20115
		[SerializeField]
		private CButton buttonNext;

		// Token: 0x04004E94 RID: 20116
		[SerializeField]
		private CButton buttonRename;

		// Token: 0x04004E95 RID: 20117
		[SerializeField]
		private CButton buttonDelete;

		// Token: 0x04004E96 RID: 20118
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04004E97 RID: 20119
		[SerializeField]
		private GameObject objSelected;

		// Token: 0x04004E98 RID: 20120
		[SerializeField]
		private GameObject rootNormal;

		// Token: 0x04004E99 RID: 20121
		[SerializeField]
		private TMP_InputField inputField;
	}
}
