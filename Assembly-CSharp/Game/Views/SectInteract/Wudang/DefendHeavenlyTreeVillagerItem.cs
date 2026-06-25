using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009DD RID: 2525
	public class DefendHeavenlyTreeVillagerItem : MonoBehaviour
	{
		// Token: 0x17000D9D RID: 3485
		// (get) Token: 0x06007B67 RID: 31591 RVA: 0x00395084 File Offset: 0x00393284
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x06007B68 RID: 31592 RVA: 0x0039508C File Offset: 0x0039328C
		public void SetData(MapBlockData blockData, CharacterDisplayData characterDisplayData, bool isSelected, int enemyCount, int villagerCount, Action<int> onSelect, Action onCancel)
		{
			DefendHeavenlyTreeVillagerItem.<>c__DisplayClass19_0 CS$<>8__locals1 = new DefendHeavenlyTreeVillagerItem.<>c__DisplayClass19_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.characterDisplayData = characterDisplayData;
			CS$<>8__locals1.onSelect = onSelect;
			this.selectedObj.SetActive(isSelected);
			MapBlockData rootBlockData = (blockData.RootBlockId > -1) ? SingletonObject.getInstance<WorldMapModel>().GetBlockData(new Location(blockData.AreaId, blockData.RootBlockId)) : null;
			this.mapBlockView.Refresh(blockData, rootBlockData);
			Location location = blockData.GetLocation();
			MapDomainMethod.AsyncCall.GetBlockFullName(null, location, delegate(int offsetData, RawDataPool poolData)
			{
				FullBlockName fullBlockName = default(FullBlockName);
				Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
				string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, true, true, true, true);
				CS$<>8__locals1.<>4__this.textBlockName.text = blockName;
			});
			bool flag = CS$<>8__locals1.characterDisplayData == null;
			if (flag)
			{
				this.avatar.ResetToBlank(false);
				this.imageVillagerHealth.gameObject.SetActive(false);
				this.textVillagerName.text = string.Empty;
				this.textVillagerState.text = string.Empty;
				this.buttonView.interactable = false;
				this.buttonCancel.interactable = false;
				this.tip.enabled = false;
				this.noneCharObj.SetActive(true);
				this.nameBackObj.SetActive(false);
			}
			else
			{
				this.avatar.Refresh(CS$<>8__locals1.characterDisplayData, true);
				this.textVillagerName.text = NameCenter.GetMonasticTitleOrDisplayName(CS$<>8__locals1.characterDisplayData, false);
				bool isOnBlock = CS$<>8__locals1.characterDisplayData.Location == blockData.GetLocation();
				this.textVillagerState.text = (isOnBlock ? LanguageKey.LK_DefendHeavenlyTree_Villager_State_Ready.Tr() : LanguageKey.LK_DefendHeavenlyTree_Villager_State_Mode.Tr());
				EHealthType healthType = CommonUtils.GetHealthType(CS$<>8__locals1.characterDisplayData.Health, CS$<>8__locals1.characterDisplayData.LeftMaxHealth, CS$<>8__locals1.characterDisplayData.CharacterId);
				string healthIcon = CommonUtils.GetHealthIcon(healthType);
				this.imageVillagerHealth.SetSprite(healthIcon, false, null);
				this.imageVillagerHealth.gameObject.SetActive(true);
				this.buttonView.interactable = true;
				this.buttonCancel.interactable = true;
				this.buttonView.ClearAndAddListener(delegate
				{
					CS$<>8__locals1.<>4__this.ShowCharacterMenu(CS$<>8__locals1.characterDisplayData.CharacterId);
				});
				this.buttonCancel.ClearAndAddListener(onCancel);
				this.tip.enabled = this.CheckCharacterMayDead(CS$<>8__locals1.characterDisplayData);
				bool enabled = this.tip.enabled;
				if (enabled)
				{
					string charName = NameCenter.GetMonasticTitleOrDisplayName(CS$<>8__locals1.characterDisplayData, false);
					string content = LocalStringManager.GetFormat(LanguageKey.LK_DefendHeavenlyTree_Villager_Tip, charName).ColorReplace();
					this.tip.PresetParam = new string[]
					{
						content
					};
				}
				this.noneCharObj.SetActive(false);
				this.nameBackObj.SetActive(true);
			}
			this.rootVillagerCount.gameObject.SetActive(false);
			this.textVillagerCount.text = villagerCount.ToString();
			this.textEnemyCount.text = LanguageKey.LK_DefendHeavenlyTree_Enemy.TrFormat(enemyCount);
			DefendHeavenlyTreeVillagerItem.<>c__DisplayClass19_0 CS$<>8__locals2 = CS$<>8__locals1;
			CharacterDisplayData characterDisplayData2 = CS$<>8__locals1.characterDisplayData;
			CS$<>8__locals2.charId = ((characterDisplayData2 != null) ? characterDisplayData2.CharacterId : -1);
			this.buttonChange.ClearAndAddListener(delegate
			{
				CS$<>8__locals1.onSelect(CS$<>8__locals1.charId);
			});
		}

		// Token: 0x06007B69 RID: 31593 RVA: 0x0039539C File Offset: 0x0039359C
		private void ShowCharacterMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06007B6A RID: 31594 RVA: 0x003953DC File Offset: 0x003935DC
		private bool CheckCharacterMayDead(CharacterDisplayData characterDisplayData)
		{
			bool flag = characterDisplayData.LeftMaxHealth <= 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int healthIntValue = CommonUtils.Health2Age(characterDisplayData.Health);
				int leftHealthIntValue = CommonUtils.Health2Age(characterDisplayData.LeftMaxHealth);
				result = ((float)healthIntValue <= (float)leftHealthIntValue * 0.25f);
			}
			return result;
		}

		// Token: 0x04005DBB RID: 23995
		[SerializeField]
		private MapBlockView mapBlockView;

		// Token: 0x04005DBC RID: 23996
		[SerializeField]
		private TextMeshProUGUI textBlockName;

		// Token: 0x04005DBD RID: 23997
		[SerializeField]
		private TextMeshProUGUI textEnemyCount;

		// Token: 0x04005DBE RID: 23998
		[SerializeField]
		private GameObject rootVillagerCount;

		// Token: 0x04005DBF RID: 23999
		[SerializeField]
		private TextMeshProUGUI textVillagerCount;

		// Token: 0x04005DC0 RID: 24000
		[SerializeField]
		private TextMeshProUGUI textVillagerName;

		// Token: 0x04005DC1 RID: 24001
		[SerializeField]
		private TextMeshProUGUI textVillagerState;

		// Token: 0x04005DC2 RID: 24002
		[SerializeField]
		private GameObject noneCharObj;

		// Token: 0x04005DC3 RID: 24003
		[SerializeField]
		private GameObject nameBackObj;

		// Token: 0x04005DC4 RID: 24004
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005DC5 RID: 24005
		[SerializeField]
		private CImage imageVillagerHealth;

		// Token: 0x04005DC6 RID: 24006
		[SerializeField]
		private CButton buttonChange;

		// Token: 0x04005DC7 RID: 24007
		[SerializeField]
		private CButton buttonView;

		// Token: 0x04005DC8 RID: 24008
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04005DC9 RID: 24009
		[SerializeField]
		private CButton button;

		// Token: 0x04005DCA RID: 24010
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04005DCB RID: 24011
		[SerializeField]
		private GameObject selectedObj;
	}
}
