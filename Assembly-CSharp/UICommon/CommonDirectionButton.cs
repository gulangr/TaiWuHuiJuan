using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UICommon
{
	// Token: 0x020005C2 RID: 1474
	[RequireComponent(typeof(CButtonObsolete))]
	public class CommonDirectionButton : MonoBehaviour
	{
		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06004612 RID: 17938 RVA: 0x0020DEF1 File Offset: 0x0020C0F1
		public CButtonObsolete Button
		{
			get
			{
				return base.GetComponent<CButtonObsolete>();
			}
		}

		// Token: 0x06004613 RID: 17939 RVA: 0x0020DEF9 File Offset: 0x0020C0F9
		private void Awake()
		{
			this.Button.ClearAndAddListener(new Action(this.OnButtonClicked));
		}

		// Token: 0x06004614 RID: 17940 RVA: 0x0020DF14 File Offset: 0x0020C114
		public void ClearAndAddButtonListener(UnityAction action)
		{
			this._buttonAction.RemoveAllListeners();
			this._buttonAction.AddListener(action);
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x0020DF30 File Offset: 0x0020C130
		public void AddButtonListener(UnityAction action)
		{
			this._buttonAction.AddListener(action);
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x0020DF40 File Offset: 0x0020C140
		public void RemoveButtonListener(UnityAction action)
		{
			this._buttonAction.RemoveListener(action);
		}

		// Token: 0x06004617 RID: 17943 RVA: 0x0020DF50 File Offset: 0x0020C150
		private void OnButtonClicked()
		{
			this.NextDirection();
			Button.ButtonClickedEvent buttonAction = this._buttonAction;
			if (buttonAction != null)
			{
				buttonAction.Invoke();
			}
		}

		// Token: 0x06004618 RID: 17944 RVA: 0x0020DF6C File Offset: 0x0020C16C
		public void NextDirection()
		{
			this._direction = (CommonDirectionButton.EDirection)((int)(this._direction + 1) % Enum.GetNames(typeof(CommonDirectionButton.EDirection)).Length);
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x0020DF90 File Offset: 0x0020C190
		public void SetDirection(CommonDirectionButton.EDirection direction)
		{
			this._direction = direction;
			this.RefreshByCurrentDirection();
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x0020DFA4 File Offset: 0x0020C1A4
		public void RefreshByCurrentDirection()
		{
			for (int i = 0; i < this.directionStateIcons.Length; i++)
			{
				this.directionStateIcons[i].SetActive(i == (int)this._direction);
			}
		}

		// Token: 0x04003091 RID: 12433
		[SerializeField]
		private GameObject[] directionStateIcons;

		// Token: 0x04003092 RID: 12434
		private CommonDirectionButton.EDirection _direction = CommonDirectionButton.EDirection.None;

		// Token: 0x04003093 RID: 12435
		private readonly Button.ButtonClickedEvent _buttonAction = new Button.ButtonClickedEvent();

		// Token: 0x0200198D RID: 6541
		public enum EDirection : sbyte
		{
			// Token: 0x0400B2AE RID: 45742
			None,
			// Token: 0x0400B2AF RID: 45743
			Down,
			// Token: 0x0400B2B0 RID: 45744
			Up
		}
	}
}
