using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Story;
using GameData.Domains.Story.MainStory;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C31 RID: 3121
	public class IronPlate : MonoBehaviour
	{
		// Token: 0x06009E9F RID: 40607 RVA: 0x004A35CC File Offset: 0x004A17CC
		public void Refresh(IronPlateData data, bool playAnim = false)
		{
			bool isUnlocked = data != null && data.IsUnlocked;
			base.gameObject.SetActive(isUnlocked);
			bool flag = !isUnlocked;
			if (!flag)
			{
				this._selectedCharId = ((data.FollowingCharId >= 0) ? data.FollowingCharId : -1);
				if (playAnim)
				{
					this.rootEffect.SetActive(true);
				}
				int curDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				bool showCooldown = !data.IsCooldownEnd(curDate);
				this.rootCooldown.SetActive(showCooldown);
				bool flag2 = showCooldown;
				if (flag2)
				{
					int time = data.CooldownDate - curDate;
					this.textCooldown.text = time.ToString();
				}
				this.styleRoot.SetInteractable(!showCooldown);
				this.button.interactable = !showCooldown;
			}
		}

		// Token: 0x06009EA0 RID: 40608 RVA: 0x004A3696 File Offset: 0x004A1896
		private void Awake()
		{
			this.button.ClearAndAddListener(new Action(this.OnClickButton));
		}

		// Token: 0x06009EA1 RID: 40609 RVA: 0x004A36B1 File Offset: 0x004A18B1
		private void OnClickButton()
		{
			StoryDomainMethod.AsyncCall.GetIronPlateOptionCharIdList(null, delegate(int offset, RawDataPool pool)
			{
				List<int> charIdList = null;
				Serializer.Deserialize(pool, offset, ref charIdList);
				bool flag = charIdList == null || charIdList.Count <= 0;
				if (!flag)
				{
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(null, charIdList, delegate(int offset, RawDataPool pool)
					{
						List<CharacterDisplayDataForGeneralScrollList> charDataList = null;
						Serializer.Deserialize(pool, offset, ref charDataList);
						bool flag2 = charDataList == null || charDataList.Count <= 0;
						if (!flag2)
						{
							CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
							config.InfoText = LanguageKey.LK_Bottom_IconPlate_SelectChar_Tip.Tr();
							config.InteractionMode = ESelectCharacterInteractionMode.Slot;
							config.SelectionMode = ESelectCharacterSelectionMode.Single;
							config.TargetCount = 1;
							bool flag3 = this._selectedCharId >= 0;
							if (flag3)
							{
								config.InitialSelectedCharacterIds = new List<int>
								{
									this._selectedCharId
								};
							}
							ViewSelectCharacter.Show(config, charDataList, new SelectCharacterCallback(this.SelectCharacterCallback), null, false);
						}
					});
				}
			});
		}

		// Token: 0x06009EA2 RID: 40610 RVA: 0x004A36C8 File Offset: 0x004A18C8
		private void SelectCharacterCallback(List<int> selectedCharacterIds)
		{
			int charId = (selectedCharacterIds != null) ? selectedCharacterIds.GetOrDefault(0, -1) : -1;
			StoryDomainMethod.Call.SetIconPlateFollowingCharId(charId);
		}

		// Token: 0x04007AB9 RID: 31417
		[SerializeField]
		private CButton button;

		// Token: 0x04007ABA RID: 31418
		[SerializeField]
		private GameObject rootEffect;

		// Token: 0x04007ABB RID: 31419
		[SerializeField]
		private GameObject rootCooldown;

		// Token: 0x04007ABC RID: 31420
		[SerializeField]
		private TextMeshProUGUI textCooldown;

		// Token: 0x04007ABD RID: 31421
		[SerializeField]
		private HSVStyleRoot styleRoot;

		// Token: 0x04007ABE RID: 31422
		private int _selectedCharId = -1;
	}
}
