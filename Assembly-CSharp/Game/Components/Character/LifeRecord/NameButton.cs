using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F52 RID: 3922
	public class NameButton : CButton
	{
		// Token: 0x0600B3C8 RID: 46024 RVA: 0x0051D2BC File Offset: 0x0051B4BC
		protected override void Awake()
		{
			base.Awake();
			base.onClick.AddListener(new UnityAction(this.OnClick));
		}

		// Token: 0x0600B3C9 RID: 46025 RVA: 0x0051D2E0 File Offset: 0x0051B4E0
		public void Set(int charId, NameAndLifeRelatedData charName)
		{
			this.CharId = charId;
			base.gameObject.SetActive(true);
			this.mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", (base.interactable = ((this.HasTomb = charName.HasTomb) || charName.LifeState == 0)) ? (this.HasTomb ? LanguageKey.LK_LIfeRecord_Character_Grave.Tr() : LanguageKey.LK_LIfeRecord_Character_Alive.Tr()) : LanguageKey.LK_LIfeRecord_Character_Dead.Tr());
		}

		// Token: 0x0600B3CA RID: 46026 RVA: 0x0051D370 File Offset: 0x0051B570
		public void OnClick()
		{
			GEvent.OnEvent(UiEvents.OnLifeRecordNameButtonClicked, EasyPool.Get<ArgumentBox>().Set("CharId", this.CharId));
			bool hasTomb = this.HasTomb;
			if (hasTomb)
			{
				ViewLifeRecords.Show(this.CharId);
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataListForRelations(null, new List<int>
				{
					this.CharId
				}, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayDataForRelations> charData = new List<CharacterDisplayDataForRelations>();
					Serializer.Deserialize(dataPool, offset, ref charData);
					GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, EasyPool.Get<ArgumentBox>().SetObject("TargetPageIndex", ECharacterSubToggleBase.StoryBase));
					bool exist = UIElement.CharacterMenu.Exist;
					if (exist)
					{
						UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().GotoCharacterWithStackView(charData[0], null);
					}
					else
					{
						UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", this.CharId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.StoryBase, ECharacterSubPage.None)));
						UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					}
				});
			}
		}

		// Token: 0x04008BD6 RID: 35798
		public int CharId;

		// Token: 0x04008BD7 RID: 35799
		public bool HasTomb;

		// Token: 0x04008BD8 RID: 35800
		public RectTransform rectTransform;

		// Token: 0x04008BD9 RID: 35801
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;
	}
}
