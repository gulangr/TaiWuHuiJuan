using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B01 RID: 2817
	public class CombatAiOptionPanel : MonoBehaviour, ICombatComponent
	{
		// Token: 0x06008AA3 RID: 35491 RVA: 0x00402CD5 File Offset: 0x00400ED5
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.AddEvent(ECombatEvents.OnClickAiOptionButton, new OnCombatEvent(this.OnClickAiOptionButton));
		}

		// Token: 0x06008AA4 RID: 35492 RVA: 0x00402D0B File Offset: 0x00400F0B
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.RemoveEvent(ECombatEvents.OnClickAiOptionButton, new OnCombatEvent(this.OnClickAiOptionButton));
		}

		// Token: 0x17000F4B RID: 3915
		// (get) Token: 0x06008AA5 RID: 35493 RVA: 0x00402D41 File Offset: 0x00400F41
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008AA6 RID: 35494 RVA: 0x00402D48 File Offset: 0x00400F48
		public AiOptions GetAiOptions()
		{
			return this.combatAiOptions.AiOptions;
		}

		// Token: 0x06008AA7 RID: 35495 RVA: 0x00402D68 File Offset: 0x00400F68
		private void OnDataReady()
		{
			this.syncOptions.ClearAndAddListener(new Action(this.SyncAiOptions));
			this.openAllOptions.ClearAndAddListener(new Action(this.OpenAllOptions));
			this.closeAllOptions.ClearAndAddListener(new Action(this.CloseAllOptions));
			this.btnClose.ClearAndAddListener(new Action(this.Hide));
			this.btnClose2.ClearAndAddListener(new Action(this.Hide));
			this.combatAiOptions.autoSave = false;
			this.combatAiOptions.Load();
			this.combatAiOptions.onOptionsChanged.RemoveAllListeners();
			this.combatAiOptions.onOptionsChanged.AddListener(new UnityAction(this.OnAiOptionsChanged));
			CombatDomainMethod.Call.SetAiOptions(this.combatAiOptions.AiOptions);
			this.saveTips.gameObject.SetActive(false);
			this.ChangeSyncOptionsInteractable(false);
			Transform transform = this.openAllOptions.transform.Find("Background/Checkmark");
			this._openAllCheckmark = ((transform != null) ? transform.gameObject : null);
			Transform transform2 = this.closeAllOptions.transform.Find("Background/Checkmark");
			this._closeAllCheckmark = ((transform2 != null) ? transform2.gameObject : null);
			this.UpdateAllOptionsCheckmarks();
		}

		// Token: 0x06008AA8 RID: 35496 RVA: 0x00402EB4 File Offset: 0x004010B4
		private void OnClickAiOptionButton()
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (!flag)
			{
				this.combatAiOptions.title.SetActive(base.gameObject.activeSelf);
				bool activeSelf = base.gameObject.activeSelf;
				if (activeSelf)
				{
					this.Hide();
				}
				else
				{
					this.Show();
				}
			}
		}

		// Token: 0x06008AA9 RID: 35497 RVA: 0x00402F11 File Offset: 0x00401111
		private void OnAiOptionsChanged()
		{
			this.saveTips.SetActive(false);
			this.ChangeSyncOptionsInteractable(true);
			this.UpdateAllOptionsCheckmarks();
			this.Model.RaiseEvent(ECombatEvents.OnAiOptionsChanged);
		}

		// Token: 0x06008AAA RID: 35498 RVA: 0x00402F40 File Offset: 0x00401140
		private void UpdateAllOptionsCheckmarks()
		{
			bool flag = this._openAllCheckmark != null;
			if (flag)
			{
				this._openAllCheckmark.SetActive(this.combatAiOptions.IsAllOptionsOpen());
			}
			bool flag2 = this._closeAllCheckmark != null;
			if (flag2)
			{
				this._closeAllCheckmark.SetActive(this.combatAiOptions.IsAllOptionsClosed());
			}
		}

		// Token: 0x06008AAB RID: 35499 RVA: 0x00402F9C File Offset: 0x0040119C
		private void ChangeSyncOptionsInteractable(bool interactable)
		{
			DisableStyleRoot syncOptionsBtnStyle = this.syncOptions.GetComponent<DisableStyleRoot>();
			syncOptionsBtnStyle.SetStyleEffect(!interactable, false);
			syncOptionsBtnStyle.GetComponent<CButton>().interactable = interactable;
			syncOptionsBtnStyle.transform.Find("SyncImage").GetComponent<CImage>().SetSprite(interactable ? "sp_icon_tongbu_0" : "sp_icon_tongbu_1", false, null);
		}

		// Token: 0x06008AAC RID: 35500 RVA: 0x00402FFB File Offset: 0x004011FB
		private void SyncAiOptions()
		{
			this.combatAiOptions.Save();
			this.saveTips.SetActive(true);
			this.ChangeSyncOptionsInteractable(false);
		}

		// Token: 0x06008AAD RID: 35501 RVA: 0x0040301F File Offset: 0x0040121F
		private void OpenAllOptions()
		{
			this.combatAiOptions.OpenAllOptions();
		}

		// Token: 0x06008AAE RID: 35502 RVA: 0x0040302E File Offset: 0x0040122E
		private void CloseAllOptions()
		{
			this.combatAiOptions.CloseAllOptions();
		}

		// Token: 0x06008AAF RID: 35503 RVA: 0x0040303D File Offset: 0x0040123D
		private void Show()
		{
			base.gameObject.SetActive(true);
			this.UpdateAllOptionsCheckmarks();
			this.Model.RaiseEvent(ECombatEvents.OnAiOptionPanelShow);
		}

		// Token: 0x06008AB0 RID: 35504 RVA: 0x00403062 File Offset: 0x00401262
		public void Hide()
		{
			base.gameObject.SetActive(false);
			this.Model.RaiseEvent(ECombatEvents.OnAiOptionPanelHide);
			this.saveTips.SetActive(false);
		}

		// Token: 0x04006A4D RID: 27213
		[SerializeField]
		private CombatAiOptions combatAiOptions;

		// Token: 0x04006A4E RID: 27214
		[SerializeField]
		private GameObject saveTips;

		// Token: 0x04006A4F RID: 27215
		[SerializeField]
		private CButton syncOptions;

		// Token: 0x04006A50 RID: 27216
		[SerializeField]
		private CButton openAllOptions;

		// Token: 0x04006A51 RID: 27217
		[SerializeField]
		private CButton closeAllOptions;

		// Token: 0x04006A52 RID: 27218
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04006A53 RID: 27219
		[SerializeField]
		private CButton btnClose2;

		// Token: 0x04006A54 RID: 27220
		private GameObject _openAllCheckmark;

		// Token: 0x04006A55 RID: 27221
		private GameObject _closeAllCheckmark;
	}
}
