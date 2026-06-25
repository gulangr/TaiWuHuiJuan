using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B18 RID: 2840
	public class CombatSkillBreakerDefendExtra : MonoBehaviour
	{
		// Token: 0x17000F67 RID: 3943
		// (get) Token: 0x06008B6A RID: 35690 RVA: 0x0040715E File Offset: 0x0040535E
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008B6B RID: 35691 RVA: 0x00407168 File Offset: 0x00405368
		private void Awake()
		{
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.DoBreak));
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.ShowBreak));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.HideBreak));
		}

		// Token: 0x06008B6C RID: 35692 RVA: 0x004071E0 File Offset: 0x004053E0
		private void OnEnable()
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			bool focus = RectTransformUtility.RectangleContainsScreenPoint(base.GetComponent<RectTransform>(), Input.mousePosition, uiCamera);
			this.ChangeBreakHintActive(focus);
		}

		// Token: 0x06008B6D RID: 35693 RVA: 0x00407218 File Offset: 0x00405418
		public void DoBreak()
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (!flag)
			{
				CombatDomainMethod.Call.ClearAffectingDefenseSkillManual(true);
			}
		}

		// Token: 0x06008B6E RID: 35694 RVA: 0x00407241 File Offset: 0x00405441
		public void ShowBreak()
		{
			this.ChangeBreakHintActive(true);
		}

		// Token: 0x06008B6F RID: 35695 RVA: 0x0040724C File Offset: 0x0040544C
		public void HideBreak()
		{
			this.ChangeBreakHintActive(false);
		}

		// Token: 0x06008B70 RID: 35696 RVA: 0x00407257 File Offset: 0x00405457
		private void ChangeBreakHintActive(bool active)
		{
			this.onNormal.SetActive(!active);
			this.onHover.SetActive(active);
		}

		// Token: 0x04006ADC RID: 27356
		[SerializeField]
		private GameObject onNormal;

		// Token: 0x04006ADD RID: 27357
		[SerializeField]
		private GameObject onHover;
	}
}
