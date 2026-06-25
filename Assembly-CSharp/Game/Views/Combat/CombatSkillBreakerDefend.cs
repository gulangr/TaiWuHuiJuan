using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B17 RID: 2839
	public class CombatSkillBreakerDefend : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F65 RID: 3941
		// (get) Token: 0x06008B5F RID: 35679 RVA: 0x00407006 File Offset: 0x00405206
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F66 RID: 3942
		// (get) Token: 0x06008B60 RID: 35680 RVA: 0x0040700D File Offset: 0x0040520D
		private GameObject BreakHint
		{
			get
			{
				return this.breakHint;
			}
		}

		// Token: 0x06008B61 RID: 35681 RVA: 0x00407018 File Offset: 0x00405218
		private void Awake()
		{
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.DoBreak));
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.ShowBreak));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.HideBreak));
		}

		// Token: 0x06008B62 RID: 35682 RVA: 0x0040708D File Offset: 0x0040528D
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.CombatPrepared, new OnCombatEvent(this.UpdateCanOperate));
		}

		// Token: 0x06008B63 RID: 35683 RVA: 0x004070A9 File Offset: 0x004052A9
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.CombatPrepared, new OnCombatEvent(this.UpdateCanOperate));
		}

		// Token: 0x06008B64 RID: 35684 RVA: 0x004070C8 File Offset: 0x004052C8
		private void UpdateCanOperate()
		{
			base.GetComponent<CButton>().interactable = this.Model.CanOperateSelfCharacter;
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.enabled = this.Model.CanOperateSelfCharacter;
		}

		// Token: 0x06008B65 RID: 35685 RVA: 0x00407106 File Offset: 0x00405306
		public void DoBreak()
		{
			CombatDomainMethod.Call.ClearAffectingDefenseSkillManual(true);
		}

		// Token: 0x06008B66 RID: 35686 RVA: 0x00407110 File Offset: 0x00405310
		public void ShowBreak()
		{
			this.ChangeBreakHintActive(true);
		}

		// Token: 0x06008B67 RID: 35687 RVA: 0x0040711B File Offset: 0x0040531B
		public void HideBreak()
		{
			this.ChangeBreakHintActive(false);
		}

		// Token: 0x06008B68 RID: 35688 RVA: 0x00407128 File Offset: 0x00405328
		private void ChangeBreakHintActive(bool active)
		{
			GameObject breakHint = this.BreakHint;
			bool flag = breakHint.activeSelf != active;
			if (flag)
			{
				breakHint.SetActive(active);
			}
		}

		// Token: 0x04006ADB RID: 27355
		[SerializeField]
		private GameObject breakHint;
	}
}
