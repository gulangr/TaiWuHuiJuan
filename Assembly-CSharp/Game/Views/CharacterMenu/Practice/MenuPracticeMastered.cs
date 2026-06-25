using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu.Practice
{
	// Token: 0x02000BB5 RID: 2997
	public class MenuPracticeMastered : MonoBehaviour
	{
		// Token: 0x060096E0 RID: 38624 RVA: 0x00465C13 File Offset: 0x00463E13
		public void Init(Action onClick)
		{
			this.switchToggle.Init(-1);
			this.switchToggle.OnActiveIndexChange += this.OnSwitchMastered;
			this._onClickAction = onClick;
		}

		// Token: 0x060096E1 RID: 38625 RVA: 0x00465C44 File Offset: 0x00463E44
		private void OnSwitchMastered(int prevIndex, int curIndex)
		{
			bool mastered = curIndex == 1;
			Action onClickAction = this._onClickAction;
			if (onClickAction != null)
			{
				onClickAction();
			}
			base.CancelInvoke("HideMasteredSwitchEffect");
			this.HideMasteredSwitchEffect();
			string preparePlayAudioName = "";
			bool flag = mastered;
			if (flag)
			{
				this.particleOff.SetActive(true);
				bool flag2 = this.switchTurnOnAudio != null;
				if (flag2)
				{
					preparePlayAudioName = this.switchTurnOnAudio.name;
				}
			}
			else
			{
				this.particleOn.SetActive(true);
				bool flag3 = this.switchTurnOffAudio != null;
				if (flag3)
				{
					preparePlayAudioName = this.switchTurnOffAudio.name;
				}
			}
			bool flag4 = !string.IsNullOrEmpty(preparePlayAudioName);
			if (flag4)
			{
				AudioManager.Instance.PlaySound(preparePlayAudioName, false, false);
			}
			base.Invoke("HideMasteredSwitchEffect", 1f);
		}

		// Token: 0x060096E2 RID: 38626 RVA: 0x00465D10 File Offset: 0x00463F10
		public void Set(CombatSkillDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				bool canOperate = (data.Mastered ? (data.GridCount >= 1) : (data.GridCount >= 2)) && (SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(data.CharId) || SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(data.CharId)) && data.TemplateId >= 0;
				bool flag2 = !canOperate;
				if (flag2)
				{
					base.gameObject.SetActive(false);
				}
				else
				{
					this.state.SetSpriteOnly(data.Mastered ? "ui9_icon_practice_mastered_1" : "ui9_icon_practice_mastered_0", false, null);
					int baseGridCount = (int)(data.Mastered ? data.GridCount : (data.GridCount - 1));
					int masterGridCount = (int)(data.Mastered ? (data.GridCount + 1) : data.GridCount);
					this.baseValue.text = baseGridCount.ToString();
					this.masteredValue.text = masterGridCount.ToString();
					for (int i = 0; i < this.basePointList.Length; i++)
					{
						this.basePointList[i].gameObject.SetActive(i < baseGridCount);
					}
					for (int j = 0; j < this.masterPointList.Length; j++)
					{
						this.masterPointList[j].gameObject.SetActive(j < masterGridCount);
					}
					this.switchToggle.SetWithoutNotify(data.Mastered ? 0 : 1);
					base.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060096E3 RID: 38627 RVA: 0x00465EB9 File Offset: 0x004640B9
		public void SetInteractable(bool interactable)
		{
			this.switchToggle.gameObject.SetActive(interactable);
		}

		// Token: 0x060096E4 RID: 38628 RVA: 0x00465ECE File Offset: 0x004640CE
		private void HideMasteredSwitchEffect()
		{
			this.particleOn.SetActive(false);
			this.particleOff.SetActive(false);
		}

		// Token: 0x040073BC RID: 29628
		[SerializeField]
		private CImage state;

		// Token: 0x040073BD RID: 29629
		[SerializeField]
		private TextMeshProUGUI baseValue;

		// Token: 0x040073BE RID: 29630
		[SerializeField]
		private TextMeshProUGUI masteredValue;

		// Token: 0x040073BF RID: 29631
		[SerializeField]
		private CImage[] basePointList;

		// Token: 0x040073C0 RID: 29632
		[SerializeField]
		private CImage[] masterPointList;

		// Token: 0x040073C1 RID: 29633
		[SerializeField]
		private GameObject particleOn;

		// Token: 0x040073C2 RID: 29634
		[SerializeField]
		private GameObject particleOff;

		// Token: 0x040073C3 RID: 29635
		[SerializeField]
		private CToggleGroup switchToggle;

		// Token: 0x040073C4 RID: 29636
		public AudioClip switchTurnOnAudio;

		// Token: 0x040073C5 RID: 29637
		public AudioClip switchTurnOffAudio;

		// Token: 0x040073C6 RID: 29638
		private Action _onClickAction;
	}
}
