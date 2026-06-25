using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B3F RID: 2879
	public class CombatPrepareProgress : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000FA8 RID: 4008
		// (get) Token: 0x06008F34 RID: 36660 RVA: 0x0042BFDC File Offset: 0x0042A1DC
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008F35 RID: 36661 RVA: 0x0042BFE4 File Offset: 0x0042A1E4
		private void Awake()
		{
			bool flag = this.interrupt != null;
			if (flag)
			{
				this.interrupt.onClick.AddListener(new UnityAction(this.DoInterrupt));
			}
		}

		// Token: 0x06008F36 RID: 36662 RVA: 0x0042C020 File Offset: 0x0042A220
		private void DoInterrupt()
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (!flag)
			{
				CombatDomainMethod.Call.InterruptOtherActionManual();
			}
		}

		// Token: 0x06008F37 RID: 36663 RVA: 0x0042C048 File Offset: 0x0042A248
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.UpdateInterrupt));
			CombatModel model = this.Model;
			model.OnPreparingOtherActionInterruptTypeChanged = (OnDataChangedEvent)Delegate.Combine(model.OnPreparingOtherActionInterruptTypeChanged, new OnDataChangedEvent(this.UpdateInterrupt));
		}

		// Token: 0x06008F38 RID: 36664 RVA: 0x0042C098 File Offset: 0x0042A298
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.UpdateInterrupt));
			CombatModel model = this.Model;
			model.OnPreparingOtherActionInterruptTypeChanged = (OnDataChangedEvent)Delegate.Remove(model.OnPreparingOtherActionInterruptTypeChanged, new OnDataChangedEvent(this.UpdateInterrupt));
		}

		// Token: 0x06008F39 RID: 36665 RVA: 0x0042C0E8 File Offset: 0x0042A2E8
		private void UpdateInterrupt()
		{
			bool flag = this.interrupt == null;
			if (!flag)
			{
				EOtherActionInterruptType type = this.Model.SelfCharacter.PreparingOtherActionInterruptType;
				bool shouldActive = type != EOtherActionInterruptType.HideClose;
				this.interrupt.gameObject.SetActive(shouldActive);
				this.interrupt.interactable = (type == EOtherActionInterruptType.Allow);
				TooltipInvoker mouseTip = this.interrupt.gameObject.GetOrAddComponent<TooltipInvoker>();
				mouseTip.Type = TipType.SingleDesc;
				mouseTip.enabled = (type == EOtherActionInterruptType.ForceFlee);
				string[] presetParam = mouseTip.PresetParam;
				bool flag2 = presetParam == null || presetParam.Length != 1;
				if (flag2)
				{
					mouseTip.PresetParam = new string[1];
				}
				mouseTip.PresetParam[0] = LanguageKey.LK_Combat_EscapeForce.Tr();
			}
		}

		// Token: 0x06008F3A RID: 36666 RVA: 0x0042C1A8 File Offset: 0x0042A3A8
		private void UpdateInterrupt(bool isAlly)
		{
			if (isAlly)
			{
				this.UpdateInterrupt();
			}
		}

		// Token: 0x04006D6D RID: 28013
		public CImage actionIcon;

		// Token: 0x04006D6E RID: 28014
		public TextMeshProUGUI actionNum;

		// Token: 0x04006D6F RID: 28015
		public CImage progressBar;

		// Token: 0x04006D70 RID: 28016
		public TextMeshProUGUI tips;

		// Token: 0x04006D71 RID: 28017
		public CImage item;

		// Token: 0x04006D72 RID: 28018
		[SerializeField]
		private CButton interrupt;
	}
}
