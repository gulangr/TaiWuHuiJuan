using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AEB RID: 2795
	public class CombatQuickActions : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F2D RID: 3885
		// (get) Token: 0x0600895C RID: 35164 RVA: 0x003F8E60 File Offset: 0x003F7060
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x0600895D RID: 35165 RVA: 0x003F8E67 File Offset: 0x003F7067
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.CombatPrepared, new OnCombatEvent(this.UpdateCanOperate));
		}

		// Token: 0x0600895E RID: 35166 RVA: 0x003F8E83 File Offset: 0x003F7083
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.CombatPrepared, new OnCombatEvent(this.UpdateCanOperate));
		}

		// Token: 0x0600895F RID: 35167 RVA: 0x003F8EA0 File Offset: 0x003F70A0
		private void UpdateCanOperate()
		{
			bool canOperate = this.Model.CanOperateSelfCharacter;
			foreach (CButton action in this.actions)
			{
				action.interactable = canOperate;
			}
		}

		// Token: 0x04006950 RID: 26960
		[SerializeField]
		private CButton[] actions;
	}
}
