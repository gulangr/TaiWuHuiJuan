using System;
using FrameWork;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B1A RID: 2842
	public class CombatSkillInterruptCasting : MonoBehaviour
	{
		// Token: 0x17000F69 RID: 3945
		// (get) Token: 0x06008B7A RID: 35706 RVA: 0x004073A0 File Offset: 0x004055A0
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F6A RID: 3946
		// (get) Token: 0x06008B7B RID: 35707 RVA: 0x004073A7 File Offset: 0x004055A7
		private GameObject CanInterruptGameObject
		{
			get
			{
				return this.canInterrupt;
			}
		}

		// Token: 0x17000F6B RID: 3947
		// (get) Token: 0x06008B7C RID: 35708 RVA: 0x004073AF File Offset: 0x004055AF
		private bool CanInterrupt
		{
			get
			{
				return this._canInterruptByCasting && !this.BanInterruptByTutorial();
			}
		}

		// Token: 0x06008B7D RID: 35709 RVA: 0x004073C8 File Offset: 0x004055C8
		private bool BanInterruptByTutorial()
		{
			TutorialChapterModel model = SingletonObject.getInstance<TutorialChapterModel>();
			return !model.GetFunctionStatus(19);
		}

		// Token: 0x06008B7E RID: 35710 RVA: 0x004073EC File Offset: 0x004055EC
		private void Awake()
		{
			GEvent.Add(UiEvents.OnSkillCasting, new GEvent.Callback(this.OnSkillCasting));
			GEvent.Add(UiEvents.OnTryInterruptSkillCasting, delegate(ArgumentBox v)
			{
				this.DoInterrupt();
			});
			GEvent.Add(UiEvents.OnSkillCasted, new GEvent.Callback(this.OnSkillCasted));
		}

		// Token: 0x06008B7F RID: 35711 RVA: 0x00407450 File Offset: 0x00405650
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnSkillCasting, new GEvent.Callback(this.OnSkillCasting));
			GEvent.Add(UiEvents.OnTryInterruptSkillCasting, delegate(ArgumentBox v)
			{
				this.DoInterrupt();
			});
			GEvent.Remove(UiEvents.OnSkillCasted, new GEvent.Callback(this.OnSkillCasted));
		}

		// Token: 0x06008B80 RID: 35712 RVA: 0x004074B2 File Offset: 0x004056B2
		private void OnSkillCasting(ArgumentBox argumentBox)
		{
			this._canInterruptByCasting = true;
		}

		// Token: 0x06008B81 RID: 35713 RVA: 0x004074BC File Offset: 0x004056BC
		private void OnSkillCasted(ArgumentBox argumentBox)
		{
			this._canInterruptByCasting = false;
		}

		// Token: 0x06008B82 RID: 35714 RVA: 0x004074C8 File Offset: 0x004056C8
		public void DoInterrupt()
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (!flag)
			{
				bool flag2 = !this.CanInterrupt;
				if (!flag2)
				{
					CombatDomainMethod.Call.InterruptSkillManual(-1, true);
				}
			}
		}

		// Token: 0x06008B83 RID: 35715 RVA: 0x00407504 File Offset: 0x00405704
		public void ShowHint()
		{
			bool flag = this.CanInterrupt;
			if (flag)
			{
				this.CanInterruptGameObject.SetActive(true);
			}
		}

		// Token: 0x06008B84 RID: 35716 RVA: 0x00407529 File Offset: 0x00405729
		public void HideHint()
		{
			this.CanInterruptGameObject.SetActive(false);
		}

		// Token: 0x04006AE0 RID: 27360
		[SerializeField]
		private GameObject canInterrupt;

		// Token: 0x04006AE1 RID: 27361
		private bool _canInterruptByCasting = true;
	}
}
