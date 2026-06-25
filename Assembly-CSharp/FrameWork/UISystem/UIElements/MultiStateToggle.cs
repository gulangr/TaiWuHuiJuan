using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02000FFB RID: 4091
	[RequireComponent(typeof(Button))]
	public class MultiStateToggle : MonoBehaviour
	{
		// Token: 0x17001504 RID: 5380
		// (get) Token: 0x0600BABB RID: 47803 RVA: 0x00550F5C File Offset: 0x0054F15C
		public int State
		{
			get
			{
				return this.currentState;
			}
		}

		// Token: 0x0600BABC RID: 47804 RVA: 0x00550F64 File Offset: 0x0054F164
		private void Awake()
		{
			Button button = base.GetComponent<Button>();
			button.onClick.AddListener(new UnityAction(this.Toggle));
			this.UpdateState();
		}

		// Token: 0x0600BABD RID: 47805 RVA: 0x00550F98 File Offset: 0x0054F198
		private void Start()
		{
			this.UpdateState();
		}

		// Token: 0x0600BABE RID: 47806 RVA: 0x00550FA2 File Offset: 0x0054F1A2
		public void Toggle()
		{
			this.currentState = (this.currentState + 1) % this.stateObjects.Length;
			this.UpdateState();
			this.onValueChanged.Invoke(this.currentState);
		}

		// Token: 0x0600BABF RID: 47807 RVA: 0x00550FD8 File Offset: 0x0054F1D8
		private void UpdateState()
		{
			for (int i = 0; i < this.stateObjects.Length; i++)
			{
				this.stateObjects[i].SetActive(i == this.currentState);
			}
		}

		// Token: 0x0600BAC0 RID: 47808 RVA: 0x00551016 File Offset: 0x0054F216
		public void UpdateState(int newState)
		{
			this.currentState = newState;
			this.UpdateState();
		}

		// Token: 0x04009042 RID: 36930
		[SerializeField]
		private GameObject[] stateObjects;

		// Token: 0x04009043 RID: 36931
		public StateChangedEvent onValueChanged = new StateChangedEvent();

		// Token: 0x04009044 RID: 36932
		private int currentState = 0;
	}
}
