using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B19 RID: 2841
	public class CombatSkillBreakerMove : MonoBehaviour
	{
		// Token: 0x17000F68 RID: 3944
		// (get) Token: 0x06008B72 RID: 35698 RVA: 0x00407280 File Offset: 0x00405480
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008B73 RID: 35699 RVA: 0x00407288 File Offset: 0x00405488
		private void Awake()
		{
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.DoBreak));
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.ShowBreak));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.HideBreak));
		}

		// Token: 0x06008B74 RID: 35700 RVA: 0x00407300 File Offset: 0x00405500
		private void OnEnable()
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			bool focus = RectTransformUtility.RectangleContainsScreenPoint(base.GetComponent<RectTransform>(), Input.mousePosition, uiCamera);
			this.ChangeBreakHintActive(focus);
		}

		// Token: 0x06008B75 RID: 35701 RVA: 0x00407338 File Offset: 0x00405538
		public void DoBreak()
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (!flag)
			{
				CombatDomainMethod.Call.ClearAffectingMoveSkillManual(true);
			}
		}

		// Token: 0x06008B76 RID: 35702 RVA: 0x00407361 File Offset: 0x00405561
		public void ShowBreak()
		{
			this.ChangeBreakHintActive(true);
		}

		// Token: 0x06008B77 RID: 35703 RVA: 0x0040736C File Offset: 0x0040556C
		public void HideBreak()
		{
			this.ChangeBreakHintActive(false);
		}

		// Token: 0x06008B78 RID: 35704 RVA: 0x00407377 File Offset: 0x00405577
		private void ChangeBreakHintActive(bool active)
		{
			this.onNormal.SetActive(!active);
			this.onHover.SetActive(active);
		}

		// Token: 0x04006ADE RID: 27358
		[SerializeField]
		private GameObject onNormal;

		// Token: 0x04006ADF RID: 27359
		[SerializeField]
		private GameObject onHover;
	}
}
