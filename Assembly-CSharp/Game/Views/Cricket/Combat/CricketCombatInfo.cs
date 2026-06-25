using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC8 RID: 2760
	public class CricketCombatInfo : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000EFE RID: 3838
		// (get) Token: 0x06008817 RID: 34839 RVA: 0x003F2E99 File Offset: 0x003F1099
		// (set) Token: 0x06008818 RID: 34840 RVA: 0x003F2EA1 File Offset: 0x003F10A1
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x06008819 RID: 34841 RVA: 0x003F2EAC File Offset: 0x003F10AC
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.Initialize();
			}
			bool flag2 = type == ECricketCombatGlobalEventType.CharacterDataReady;
			if (flag2)
			{
				this.DoUpdate();
			}
			bool flag3 = type == ECricketCombatGlobalEventType.PauseStateChanged;
			if (flag3)
			{
				this.OnPauseStateChanged(argBox);
			}
		}

		// Token: 0x0600881A RID: 34842 RVA: 0x003F2EEC File Offset: 0x003F10EC
		private void Initialize()
		{
			this.avatar.ResetToBlank(false);
			this._characterId = -1;
			this.openCharMenu.ClearAndAddListener(new Action(this.OpenCharMenu));
			this.openCharMenu.interactable = false;
			this.UpdateCharMenuTip(false);
		}

		// Token: 0x0600881B RID: 34843 RVA: 0x003F2F3C File Offset: 0x003F113C
		private void DoUpdate()
		{
			CharacterDisplayData data = this.ally ? CricketCombatKit.Board.SelfChar : CricketCombatKit.Board.EnemyChar;
			this._characterId = data.CharacterId;
			this.avatar.Refresh(data, true);
			this.txtMeshForName.text = NameCenter.GetMonasticTitleOrDisplayName(data, this.ally);
		}

		// Token: 0x0600881C RID: 34844 RVA: 0x003F2F9C File Offset: 0x003F119C
		private void OpenCharMenu()
		{
			bool flag = this._characterId < 0;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", this._characterId);
				argBox.Set("CanOperate", false);
				argBox.Set("PreviousView", 4);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				this.Handler.OnEvent(ECricketCombatGlobalEventType.CharacterMenuShowed, null);
				Time.timeScale = 1f;
				UIElement.CharacterMenu.OnHide = delegate()
				{
					this.Handler.OnEvent(ECricketCombatGlobalEventType.CharacterMenuHide, null);
					Time.timeScale = 0f;
				};
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x0600881D RID: 34845 RVA: 0x003F3038 File Offset: 0x003F1238
		private void OnPauseStateChanged(ArgumentBox argBox)
		{
			bool nowPauseState;
			bool flag = !argBox.Get("NowPauseState", out nowPauseState);
			if (!flag)
			{
				this.openCharMenu.interactable = nowPauseState;
				this.UpdateCharMenuTip(nowPauseState);
			}
		}

		// Token: 0x0600881E RID: 34846 RVA: 0x003F3074 File Offset: 0x003F1274
		private void UpdateCharMenuTip(bool isInteractable)
		{
			TooltipInvoker tip = this.openCharMenu.GetComponent<TooltipInvoker>();
			bool flag = tip == null;
			if (!flag)
			{
				tip.enabled = !isInteractable;
				bool flag2 = !isInteractable;
				if (flag2)
				{
					tip.Type = TipType.SingleDesc;
					tip.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCombat_ViewCharacter_Tip));
				}
			}
		}

		// Token: 0x04006855 RID: 26709
		[SerializeField]
		private bool ally;

		// Token: 0x04006856 RID: 26710
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006857 RID: 26711
		[SerializeField]
		private TextMeshProUGUI txtMeshForName;

		// Token: 0x04006858 RID: 26712
		[SerializeField]
		private CButton openCharMenu;

		// Token: 0x04006859 RID: 26713
		private int _characterId = -1;
	}
}
