using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B23 RID: 2851
	public class CombatAvatar : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F6F RID: 3951
		// (get) Token: 0x06008BDA RID: 35802 RVA: 0x0040A188 File Offset: 0x00408388
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008BDB RID: 35803 RVA: 0x0040A190 File Offset: 0x00408390
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.AddEvent(ECombatEvents.OnTimeScaleChanged, new OnCombatEvent(this.OnTimeScaleChanged));
			this.Model.AddEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.AddEvent(ECombatEvents.CombatBeginReady, new OnCombatEvent(this.OnCombatBeginReady));
		}

		// Token: 0x06008BDC RID: 35804 RVA: 0x0040A204 File Offset: 0x00408404
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.RemoveEvent(ECombatEvents.OnTimeScaleChanged, new OnCombatEvent(this.OnTimeScaleChanged));
			this.Model.RemoveEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
			this.Model.RemoveEvent(ECombatEvents.CombatBeginReady, new OnCombatEvent(this.OnCombatBeginReady));
			this.ClearCharacterAvatar();
		}

		// Token: 0x06008BDD RID: 35805 RVA: 0x0040A280 File Offset: 0x00408480
		private void Awake()
		{
			bool flag = !this.ally;
			if (flag)
			{
				bool flag2 = this.immortalTips != null;
				if (flag2)
				{
					this.immortalTips.SetActive(false);
				}
			}
			bool flag3 = this._characterAvatar == null;
			if (flag3)
			{
				this._characterAvatar = new CharacterAvatar(this.avatar, true);
			}
			PointerTrigger pointerTrigger = this.openCharMenu.GetComponent<PointerTrigger>();
			this.openCharMenu.ClearAndAddListener(delegate
			{
				this.ShowCharMenu(this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId);
			});
			bool flag4 = this.openCharMenuTips != null;
			if (flag4)
			{
				GameObject gameObject = this.openCharMenuTips;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06008BDE RID: 35806 RVA: 0x0040A324 File Offset: 0x00408524
		private void OnDataReady()
		{
			this.SetupCharacterAvatar();
			bool flag = this.openCharMenuTips != null;
			if (flag)
			{
				GameObject gameObject = this.openCharMenuTips;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			this.UpdateMainCharInjuryTips();
		}

		// Token: 0x06008BDF RID: 35807 RVA: 0x0040A363 File Offset: 0x00408563
		private void OnCombatBeginReady()
		{
			this.SetupCharacterAvatar();
		}

		// Token: 0x06008BE0 RID: 35808 RVA: 0x0040A370 File Offset: 0x00408570
		private void OnChangeChar()
		{
			bool isAlly = this.Model.CharIsAlly(this.Model.ChangingFromCharId);
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				int toCharId = this.Model.ChangingToCharId;
				base.StartCoroutine(this.ChangeCombatChar(isAlly, toCharId));
				this.UpdateMainCharInjuryTipsByChar(toCharId);
				CharacterDisplayData displayData = this.Model.DisplayDataCache[toCharId];
				this.charName.text = CombatUtils.GetNameString(displayData, isAlly);
			}
		}

		// Token: 0x06008BE1 RID: 35809 RVA: 0x0040A3F0 File Offset: 0x004085F0
		private void OnTimeScaleChanged()
		{
			bool paused = this.Model.TimeScale == 0f;
			this.openCharMenu.interactable = paused;
			PointerTrigger pointerTrigger = this.openCharMenu.GetComponent<PointerTrigger>();
			pointerTrigger.enabled = paused;
			bool flag = this.openCharMenuTips != null;
			if (flag)
			{
				GameObject gameObject = this.openCharMenuTips;
				if (gameObject != null)
				{
					gameObject.SetActive(paused);
				}
			}
			bool flag2 = paused;
			if (flag2)
			{
				bool flag3 = RectTransformUtility.RectangleContainsScreenPoint(this.openCharMenu.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag3)
				{
					pointerTrigger.EnterEvent.Invoke();
				}
			}
		}

		// Token: 0x06008BE2 RID: 35810 RVA: 0x0040A492 File Offset: 0x00408692
		private IEnumerator ChangeCombatChar(bool isAlly, int toCharId)
		{
			RectTransform avatarTransform = this.avatar.GetComponent<RectTransform>();
			float aniTime = CombatUtils.ChangeCharacterWaitTime();
			avatarTransform.DOAnchorPosX((float)(isAlly ? -500 : 500), aniTime, false);
			yield return new WaitForSeconds(aniTime + 0.5f);
			this._characterAvatar.CharacterId = toCharId;
			avatarTransform.DOAnchorPosX(0f, aniTime, false);
			yield break;
		}

		// Token: 0x06008BE3 RID: 35811 RVA: 0x0040A4B0 File Offset: 0x004086B0
		private void SetupCharacterAvatar()
		{
			bool flag = this._characterAvatar == null;
			if (flag)
			{
				this._characterAvatar = new CharacterAvatar(this.avatar, true);
			}
			this._characterAvatar.CharacterId = -1;
			int charId = this.ally ? this.Model.SelfTeam[0] : this.Model.EnemyTeam[0];
			this._characterAvatar.CharacterId = charId;
			this.charName.text = CombatUtils.GetNameString(this.Model.DisplayDataCache[charId], this.ally);
		}

		// Token: 0x06008BE4 RID: 35812 RVA: 0x0040A54C File Offset: 0x0040874C
		private void ClearCharacterAvatar()
		{
			bool flag = this._characterAvatar != null;
			if (flag)
			{
				this._characterAvatar.CharacterId = -1;
			}
		}

		// Token: 0x06008BE5 RID: 35813 RVA: 0x0040A574 File Offset: 0x00408774
		private void UpdateMainCharInjuryTips()
		{
			int charId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
			this.UpdateMainCharInjuryTipsByChar(charId);
		}

		// Token: 0x06008BE6 RID: 35814 RVA: 0x0040A5AC File Offset: 0x004087AC
		private void UpdateMainCharInjuryTipsByChar(int charId)
		{
			TooltipInvoker tip = this.openCharMenu.GetComponent<TooltipInvoker>();
			CombatUtils.UpdateInjuryTips(tip, charId);
		}

		// Token: 0x06008BE7 RID: 35815 RVA: 0x0040A5D0 File Offset: 0x004087D0
		private void ShowCharMenu(int charId)
		{
			bool isAlly = this.Model.SelfTeam.Contains(charId);
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				CombatUtils.ShowCharMenu(charId);
			}
		}

		// Token: 0x04006B13 RID: 27411
		[SerializeField]
		private bool ally;

		// Token: 0x04006B14 RID: 27412
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006B15 RID: 27413
		[SerializeField]
		private TextMeshProUGUI charName;

		// Token: 0x04006B16 RID: 27414
		[SerializeField]
		private CButton openCharMenu;

		// Token: 0x04006B17 RID: 27415
		[SerializeField]
		private GameObject openCharMenuTips;

		// Token: 0x04006B18 RID: 27416
		[SerializeField]
		private GameObject immortalTips;

		// Token: 0x04006B19 RID: 27417
		private CharacterAvatar _characterAvatar;
	}
}
