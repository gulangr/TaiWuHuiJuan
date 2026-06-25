using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200026F RID: 623
public class MouseTipAdventure : MouseTipBase
{
	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x060028FE RID: 10494 RVA: 0x001301FE File Offset: 0x0012E3FE
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x00130204 File Offset: 0x0012E404
	protected override void Init(ArgumentBox argsBox)
	{
		this._canOpenCharacterList = false;
		argsBox.Get<Location>("Location", out this._location);
		AdventureSiteData adventureSiteData;
		argsBox.Get<AdventureSiteData>("AdventureSiteData", out adventureSiteData);
		base.CGet<TextMeshProUGUI>("MoreInfoTips0").text = LocalStringManager.Get(LanguageKey.LK_KeyDown_Tips);
		base.CGet<TextMeshProUGUI>("MoreInfoTips1").text = LocalStringManager.Get(LanguageKey.LK_Adventure_Tips_More_Info);
		Config.AdventureItem configData = Adventure.Instance[adventureSiteData.TemplateId];
		base.CGet<TextMeshProUGUI>("Title").SetText(configData.Name, true);
		bool dispatched = SingletonObject.getInstance<BuildingModel>().CheckBlockHasWork(this._location, 11);
		base.CGet<TextMeshProUGUI>("Desc").SetText(configData.Desc, true);
		Refers difficultyLayout = base.CGet<Refers>("DifficultyLayout");
		sbyte combatDifficulty = GameData.Domains.Adventure.SharedMethods.GetAdventureCombatDifficulty(configData.TemplateId, GameData.Domains.World.SharedMethods.GetXiangshuLevel(SingletonObject.getInstance<BasicGameData>().XiangshuProgress));
		difficultyLayout.CGet<TextMeshProUGUI>("CombatDifficulty").text = LocalStringManager.GetFormat(LanguageKey.LK_Item_Operation_LifeSkill_Require_Meet, LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_CombatSystem), combatDifficulty.ToString().SetGradeColor((int)combatDifficulty));
		sbyte lifeDifficulty = GameData.Domains.Adventure.SharedMethods.GetAdventureLifeSkillDifficulty(configData.TemplateId, GameData.Domains.World.SharedMethods.GetXiangshuLevel(SingletonObject.getInstance<BasicGameData>().XiangshuProgress));
		difficultyLayout.CGet<TextMeshProUGUI>("LifeDifficulty").text = LocalStringManager.GetFormat(LanguageKey.LK_Item_Operation_LifeSkill_Require_Meet, LocalStringManager.Get(LanguageKey.LK_LifeSkill), lifeDifficulty.ToString().SetGradeColor((int)lifeDifficulty));
		Refers costLayout = base.CGet<Refers>("CostLayout");
		float curTime = SingletonObject.getInstance<TimeManager>().GetRemainingFloatActionPointConvertToDays();
		costLayout.CGet<TextMeshProUGUI>("Time").text = LocalStringManager.GetFormat(LanguageKey.LK_Refine_Resource_Require_Meet, LocalStringManager.Get(LanguageKey.LK_ActionPoint), curTime.ToString("f1").SetColor((curTime >= (float)configData.TimeCost) ? "brightblue" : "brightred"), configData.TimeCost.ToString("f1").SetColor("pinkyellow"));
		this.NeedWaitData = true;
		TaiwuDomainMethod.AsyncCall.GetAllResources(this, ItemSourceType.Resources, delegate(int offset, RawDataPool dataPool)
		{
			ValueTuple<ItemSourceType, ResourceInts> tuple = default(ValueTuple<ItemSourceType, ResourceInts>);
			Serializer.Deserialize(dataPool, offset, ref tuple);
			ResourceInts resources = tuple.Item2;
			List<Refers> refersList = costLayout.CGetList<Refers>("ResourceLayout");
			for (int i = 0; i < 8; i++)
			{
				ResourceTypeItem resourceConfig = Config.ResourceType.Instance[i];
				Refers refers = refersList[i];
				int curResource = resources.Get(i);
				int needResource = configData.ResCost[i];
				bool show = needResource > 0;
				refers.gameObject.SetActive(show);
				bool flag6 = !show;
				if (!flag6)
				{
					string color = (curResource >= needResource) ? "brightblue" : "brightred";
					refers.CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
					refers.CGet<TextMeshProUGUI>("Amount").text = LocalStringManager.GetFormat(LanguageKey.LK_Refine_Resource_Require_Meet, resourceConfig.Name, CommonUtils.GetDisplayStringForNum(curResource, 100000).SetColor(color), CommonUtils.GetDisplayStringForNum(needResource, 100000).SetColor("pinkyellow"));
				}
			}
			this.Element.ShowAfterRefresh();
		});
		base.CGet<GameObject>("AdventureLayout").SetActive(false);
		Config.AdventureItem adventureConfig = null;
		EnemyNestItem enemyNestItem = null;
		bool flag = adventureSiteData != null && adventureSiteData.SiteState >= 1;
		if (flag)
		{
			adventureConfig = Adventure.Instance[adventureSiteData.TemplateId];
			bool flag2 = adventureConfig.Type == 4 || adventureConfig.Type == 5;
			if (flag2)
			{
				EnemyNest.Instance.Iterate(delegate(EnemyNestItem item)
				{
					bool flag6 = item.AdventureId == (int)adventureSiteData.TemplateId;
					bool result;
					if (flag6)
					{
						enemyNestItem = item;
						result = false;
					}
					else
					{
						result = true;
					}
					return result;
				});
			}
		}
		Refers enemyNestLayout = base.CGet<Refers>("EnemyNestLayout");
		Refers dispatchLayout = base.CGet<Refers>("DispatchLayout");
		bool flag3 = enemyNestItem != null && adventureConfig != null;
		if (flag3)
		{
			enemyNestLayout.CGet<TextMeshProUGUI>("SubTitle").SetText(enemyNestItem.TipTitle, true);
			string desc = enemyNestItem.TipDesc.SetColor("pinkyellow");
			string typeText = LocalStringManager.Get((enemyNestItem.NestType == 0) ? LanguageKey.LK_Block_Tip_NemeyNest_Type_0 : LanguageKey.LK_Block_Tip_NemeyNest_Type_1);
			bool flag4 = adventureSiteData.RemainingMonths > 0;
			if (flag4)
			{
				desc += LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_NemeyNest_Time, typeText, adventureSiteData.RemainingMonths.ToString()).SetColor("brightred");
			}
			enemyNestLayout.CGet<TextMeshProUGUI>("Desc").SetText(desc, true);
			enemyNestLayout.gameObject.SetActive(true);
			bool flag5 = adventureSiteData.SiteState >= 2;
			if (flag5)
			{
				string workText = LocalStringManager.Get(LanguageKey.LK_Dispatch_Type_Supervise);
				string subTitle = LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_SubTitle, LocalStringManager.Get(LanguageKey.LK_Dispatch), workText);
				dispatchLayout.CGet<TextMeshProUGUI>("SubTitle").text = subTitle;
				dispatchLayout.CGet<TextMeshProUGUI>("Desc").text = this.GetDispatchStateString(adventureSiteData, dispatched);
				dispatchLayout.CGet<TextMeshProUGUI>("State").text = string.Empty;
				dispatchLayout.gameObject.SetActive(true);
				difficultyLayout.gameObject.SetActive(false);
				costLayout.gameObject.SetActive(false);
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(dispatchLayout.GetComponent<RectTransform>());
				});
			}
			else
			{
				dispatchLayout.gameObject.SetActive(false);
			}
		}
		else
		{
			enemyNestLayout.gameObject.SetActive(false);
			dispatchLayout.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x001306E4 File Offset: 0x0012E8E4
	private string GetDispatchStateString(AdventureSiteData adventureSiteData, bool dispatched)
	{
		string tipContent = string.Empty;
		LanguageKey key = LanguageKey.Invalid;
		bool flag = adventureSiteData.SiteState == 2;
		if (flag)
		{
			key = (dispatched ? LanguageKey.LK_Adventure_Conquered_Dispatched_Tip : LanguageKey.LK_Adventure_Conquered_NotDispatched_Tip);
		}
		else
		{
			bool flag2 = adventureSiteData.SiteState == 3;
			if (flag2)
			{
				key = (dispatched ? LanguageKey.LK_Adventure_HasTribute_Dispatched_Tip : LanguageKey.LK_Adventure_HasTribute_NotDispatched_Tip);
			}
		}
		bool flag3 = key > LanguageKey.EventEditor_Error_DuplicateGroupKey;
		if (flag3)
		{
			tipContent = LocalStringManager.Get(key).SetColor("pinkyellow");
		}
		return tipContent;
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x00130760 File Offset: 0x0012E960
	private void Update()
	{
		bool flag = UIManager.Instance.IsElementActive(UIElement.TaiwuVillagers);
		if (!flag)
		{
			bool altDown = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
			bool flag2 = altDown && this._canOpenCharacterList;
			if (flag2)
			{
				bool exist = UIManager.Instance.IsElementActive(UIElement.UltimateSelectCharacter);
				bool flag3 = !exist;
				if (flag3)
				{
					this.OpenCharacterList();
				}
			}
		}
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x001307D0 File Offset: 0x0012E9D0
	private void OpenCharacterList()
	{
		CharacterSortFilterSettings sortFilterSettings = new CharacterSortFilterSettings();
		sortFilterSettings.FilterType = 1;
		sortFilterSettings.TargetLocation = this._location;
		CharacterDomainMethod.AsyncCall.InitializeCharacterSortFilter(null, sortFilterSettings, delegate(int offset, RawDataPool dataPool)
		{
			CharacterList characterList = default(CharacterList);
			Serializer.Deserialize(dataPool, offset, ref characterList);
		});
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		string title = LocalStringManager.Get(LanguageKey.LK_Adventure_Participant_Roster);
		argBox.Set("Title", title);
		argBox.SetObject("SortFilterSettings", sortFilterSettings);
		argBox.SetObject("OnConfirmSelect", null);
		argBox.SetObject("OnCancelSelect", null);
		Action<int> action = delegate(int charId)
		{
			ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
			argBox2.Set("CharacterId", charId);
			argBox2.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
			UIElement.CharacterMenu.SetOnInitArgs(argBox2);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		};
		argBox.SetObject("OnClickChar", action);
		UIElement.UltimateSelectCharacter.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.UltimateSelectCharacter, true);
	}

	// Token: 0x04001DE8 RID: 7656
	private List<CharacterDisplayData> _dataList = new List<CharacterDisplayData>();

	// Token: 0x04001DE9 RID: 7657
	private Location _location;

	// Token: 0x04001DEA RID: 7658
	private bool _canOpenCharacterList;
}
