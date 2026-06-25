using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Select;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ACE RID: 2766
	public class CricketCombatPolymorphCharacterSelector : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F01 RID: 3841
		// (get) Token: 0x06008848 RID: 34888 RVA: 0x003F3D93 File Offset: 0x003F1F93
		// (set) Token: 0x06008849 RID: 34889 RVA: 0x003F3D9B File Offset: 0x003F1F9B
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x0600884A RID: 34890 RVA: 0x003F3DA4 File Offset: 0x003F1FA4
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.Initialize();
			}
			bool flag2 = type == ECricketCombatGlobalEventType.CricketDataReady;
			if (flag2)
			{
				this.RefreshState();
			}
			bool flag3 = type == ECricketCombatGlobalEventType.SelfCricketChanged;
			if (flag3)
			{
				this.RefreshState();
			}
			bool flag4 = type == ECricketCombatGlobalEventType.CricketPolymorphCharacterChanged;
			if (flag4)
			{
				this.RefreshState();
			}
			bool flag5 = type == ECricketCombatGlobalEventType.AllowSelectCricketChanged;
			if (flag5)
			{
				this.RefreshState();
			}
			bool flag6 = type == ECricketCombatGlobalEventType.CombatStatusChanged;
			if (flag6)
			{
				bool flag7 = CricketCombatKit.Board.Status == ECricketCombatStatus.Combating;
				if (flag7)
				{
					this.button.interactable = false;
					this.buttonPointerTrigger.enabled = false;
				}
			}
		}

		// Token: 0x0600884B RID: 34891 RVA: 0x003F3E3C File Offset: 0x003F203C
		private void Awake()
		{
			this.button.ClearAndAddListener(new Action(this.OnButtonClicked));
			this.buttonPointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnButtonPointerEnter));
			this.buttonPointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnButtonPointerExit));
		}

		// Token: 0x0600884C RID: 34892 RVA: 0x003F3E9C File Offset: 0x003F209C
		private void Initialize()
		{
			this.SetStateHidden();
		}

		// Token: 0x0600884D RID: 34893 RVA: 0x003F3EA8 File Offset: 0x003F20A8
		private void RefreshState()
		{
			bool flag = !CricketPolymorphHelper.IsCricketPolymorphEnabled;
			if (flag)
			{
				this.SetStateHidden();
			}
			else
			{
				ItemDisplayData cricketData = CricketCombatKit.Board.SelfCrickets.GetOrDefault(this.jarIndex);
				bool flag2 = cricketData == null;
				if (flag2)
				{
					this.SetStateHidden();
				}
				else
				{
					CharacterDisplayDataForGeneralScrollList selectedCharacter = CricketCombatKit.Board.GetSelectedCricketPolymorphCharacter(this.jarIndex);
					bool flag3 = selectedCharacter != null;
					if (flag3)
					{
						this.SetStateSelected(selectedCharacter);
					}
					else
					{
						List<CharacterDisplayDataForGeneralScrollList> available = CricketCombatKit.Board.GetAvailableCricketPolymorphCharacters(this.jarIndex);
						bool flag4 = available.Count > 0;
						if (flag4)
						{
							this.SetStateAdd();
						}
						else
						{
							this.SetStateHidden();
						}
					}
				}
			}
		}

		// Token: 0x0600884E RID: 34894 RVA: 0x003F3F50 File Offset: 0x003F2150
		private void SetStateHidden()
		{
			this.root.SetActive(false);
		}

		// Token: 0x0600884F RID: 34895 RVA: 0x003F3F60 File Offset: 0x003F2160
		private void SetStateAdd()
		{
			this.root.SetActive(true);
			this.addRoot.SetActive(true);
			this.selectedRoot.SetActive(false);
			this._isSelected = false;
			this.button.interactable = true;
			this.buttonPointerTrigger.enabled = true;
		}

		// Token: 0x06008850 RID: 34896 RVA: 0x003F3FB8 File Offset: 0x003F21B8
		private void SetStateSelected(CharacterDisplayDataForGeneralScrollList charData)
		{
			this.root.SetActive(true);
			this.addRoot.SetActive(false);
			this.selectedRoot.SetActive(true);
			this.selectedAvatar.Refresh(charData.AvatarRelatedData, charData.CharacterTemplateId);
			this._isSelected = true;
			this.button.interactable = true;
			this.buttonPointerTrigger.enabled = true;
		}

		// Token: 0x06008851 RID: 34897 RVA: 0x003F4028 File Offset: 0x003F2228
		private void OnButtonClicked()
		{
			bool isSelected = this._isSelected;
			if (isSelected)
			{
				CricketCombatKit.Board.DeselectCricketPolymorphCharacter(this.jarIndex);
				this.Handler.OnEvent(ECricketCombatGlobalEventType.CricketPolymorphCharacterChanged, null);
			}
			else
			{
				this.ShowSelectCharacterPanel();
			}
		}

		// Token: 0x06008852 RID: 34898 RVA: 0x003F406C File Offset: 0x003F226C
		private void OnButtonPointerEnter()
		{
			this.hoverNode.SetActive(true);
		}

		// Token: 0x06008853 RID: 34899 RVA: 0x003F407C File Offset: 0x003F227C
		private void OnButtonPointerExit()
		{
			this.hoverNode.SetActive(false);
		}

		// Token: 0x06008854 RID: 34900 RVA: 0x003F408C File Offset: 0x003F228C
		private void ShowSelectCharacterPanel()
		{
			List<CharacterDisplayDataForGeneralScrollList> available = CricketCombatKit.Board.GetAvailableCricketPolymorphCharacters(this.jarIndex);
			bool flag = available.Count == 0;
			if (!flag)
			{
				List<ISelectCharacterData> dataList = new List<ISelectCharacterData>(available.Count);
				foreach (CharacterDisplayDataForGeneralScrollList ch in available)
				{
					dataList.Add(new BasicSelectCharacterDataAdapter(ch));
				}
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.Title = LanguageKey.LK_CricketCombat_SelectCricketPolymorphCharacter.Tr();
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				config.TargetCount = 1;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender
				};
				config.BannedCharacterIds = new HashSet<int>();
				ViewSelectCharacter.Show(config, dataList, new SelectCharacterCallback(this.OnSelectCharacterCallback), null, false);
			}
		}

		// Token: 0x06008855 RID: 34901 RVA: 0x003F417C File Offset: 0x003F237C
		private void OnSelectCharacterCallback(List<int> selectedCharacterIds)
		{
			bool flag = selectedCharacterIds == null || selectedCharacterIds.Count == 0;
			if (!flag)
			{
				CricketCombatKit.Board.SelectCricketPolymorphCharacter(this.jarIndex, selectedCharacterIds[0]);
				this.Handler.OnEvent(ECricketCombatGlobalEventType.CricketPolymorphCharacterChanged, null);
			}
		}

		// Token: 0x04006869 RID: 26729
		[SerializeField]
		private int jarIndex;

		// Token: 0x0400686A RID: 26730
		[SerializeField]
		private GameObject root;

		// Token: 0x0400686B RID: 26731
		[SerializeField]
		private GameObject addRoot;

		// Token: 0x0400686C RID: 26732
		[SerializeField]
		private GameObject selectedRoot;

		// Token: 0x0400686D RID: 26733
		[SerializeField]
		private CButton button;

		// Token: 0x0400686E RID: 26734
		[SerializeField]
		private PointerTrigger buttonPointerTrigger;

		// Token: 0x0400686F RID: 26735
		[SerializeField]
		private GameObject hoverNode;

		// Token: 0x04006870 RID: 26736
		[SerializeField]
		private Game.Components.Avatar.Avatar selectedAvatar;

		// Token: 0x04006871 RID: 26737
		private bool _isSelected;
	}
}
