using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using Game.Components.SortAndFilter.Secret;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.World;
using GameData.Utilities;
using GameData.Utilities.Information;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200021A RID: 538
public static class InformationUtils
{
	// Token: 0x06002279 RID: 8825 RVA: 0x000FF195 File Offset: 0x000FD395
	[Obsolete]
	public static void RefreshNormalInformationView(Refers informationView, NormalInformation normalInformation)
	{
		InformationUtils.RefreshNormalInformationView(informationView, normalInformation, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000FF1AA File Offset: 0x000FD3AA
	[Obsolete]
	public static void RefreshNormalInformationView(Refers informationView, NormalInformation normalInformation, int characterId)
	{
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000FF1B0 File Offset: 0x000FD3B0
	[Obsolete]
	public static void RefreshSecretInformationView(Refers secretInformationView, SecretInformationDisplayData secretInformationDisplayData, SecretInformationDisplayPackage secretInformationDisplayPackage)
	{
		InformationUtils.<>c__DisplayClass4_0 CS$<>8__locals1 = new InformationUtils.<>c__DisplayClass4_0();
		CS$<>8__locals1.secretInformationDisplayPackage = secretInformationDisplayPackage;
		TimeManager timeManager = SingletonObject.getInstance<TimeManager>();
		BasicGameData basicData = SingletonObject.getInstance<BasicGameData>();
		CS$<>8__locals1.config = SecretInformation.Instance[secretInformationDisplayData.SecretInformationTemplateId];
		CToggleObsolete toggle = secretInformationView.GetComponentInChildren<CToggleObsolete>(true);
		PointerTrigger pointerTrigger = secretInformationView.GetComponentInChildren<PointerTrigger>();
		CS$<>8__locals1.elements = secretInformationView.CGet<RectTransform>("Elements");
		UnityEvent exitEvent = pointerTrigger.ExitEvent;
		if (exitEvent != null)
		{
			exitEvent.Invoke();
		}
		pointerTrigger.enabled = false;
		toggle.enabled = false;
		int date = secretInformationDisplayData.OccurenceDate;
		int lifeTime = (int)CS$<>8__locals1.config.Duration - (basicData.CurrDate - date);
		secretInformationView.CGet<TextMeshProUGUI>("LifeTime").text = ((lifeTime >= 0) ? lifeTime.ToString() : "-");
		secretInformationView.CGet<TextMeshProUGUI>("Date").text = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
		{
			timeManager.GetYearByDate(date),
			(int)(timeManager.GetMonthInYear(date) + 1),
			LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetCurrSeason())),
			LocalStringManager.Get(LanguageKey.LK_Month)
		});
		foreach (object obj in CS$<>8__locals1.elements)
		{
			RectTransform element = (RectTransform)obj;
			element.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = false;
		}
		bool hasSource = false;
		int elementDisplayCount = 0;
		bool flag = secretInformationDisplayData.SourceCharacterId >= 0;
		if (flag)
		{
			SecretInformationElement element2 = CS$<>8__locals1.elements.GetChild(0).GetComponent<SecretInformationElement>();
			CharacterDisplayData characterDisplayData;
			bool flag2 = CS$<>8__locals1.secretInformationDisplayPackage.CharacterData.TryGetValue(secretInformationDisplayData.SourceCharacterId, out characterDisplayData);
			if (flag2)
			{
				element2.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
				element2.UiName.text = LocalStringManager.Get(LanguageKey.LK_SecretInformation_DisseminationBranchCharacter);
				element2.DisplayAvatar(characterDisplayData);
				elementDisplayCount++;
				hasSource = true;
			}
		}
		byte[] parametersPack = secretInformationDisplayData.ParametersPack;
		if (parametersPack != null)
		{
			parametersPack.ExtractSecretParameters(CS$<>8__locals1.config, delegate(int i, int charId)
			{
				CharacterDisplayData characterDisplayData2;
				SecretInformationElement element4;
				bool flag3;
				if (CS$<>8__locals1.secretInformationDisplayPackage.CharacterData.TryGetValue(charId, out characterDisplayData2))
				{
					element4 = base.<RefreshSecretInformationView>g__SetupElement|7(i);
					flag3 = (element4 != null);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				if (flag4)
				{
					element4.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
					element4.DisplayAvatar(characterDisplayData2);
					elementDisplayCount++;
				}
			}, delegate(int i, Location location)
			{
				SecretInformationElement element4 = base.<RefreshSecretInformationView>g__SetupElement|7(i);
				bool flag3 = element4 != null;
				if (flag3)
				{
					element4.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
					element4.DisplayLocation(location);
					elementDisplayCount++;
				}
			}, delegate(int i, sbyte resourceType)
			{
				SecretInformationElement element4 = base.<RefreshSecretInformationView>g__SetupElement|7(i);
				bool flag3 = element4 != null;
				if (flag3)
				{
					element4.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
					element4.DisplayResourceType(resourceType);
					elementDisplayCount++;
				}
			}, delegate(int i, ItemKey itemKey)
			{
				SecretInformationElement element4 = base.<RefreshSecretInformationView>g__SetupElement|7(i);
				bool flag3 = element4 != null;
				if (flag3)
				{
					element4.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
					element4.DisplayItem(itemKey);
					elementDisplayCount++;
				}
			}, delegate(int i, short combatSkillTemplateId)
			{
				SecretInformationElement element4 = base.<RefreshSecretInformationView>g__SetupElement|7(i);
				bool flag3 = element4 != null;
				if (flag3)
				{
					element4.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
					element4.DisplayCombatSkill(combatSkillTemplateId);
					elementDisplayCount++;
				}
			}, delegate(int i, short lifeSkillTemplateId)
			{
				SecretInformationElement element4 = base.<RefreshSecretInformationView>g__SetupElement|7(i);
				bool flag3 = element4 != null;
				if (flag3)
				{
					element4.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive = true;
					element4.DisplayLifeSkill(lifeSkillTemplateId);
					elementDisplayCount++;
				}
			}, delegate(int _, int _)
			{
			});
		}
		int elementPosition = 50;
		int elementPositionDelta = (elementDisplayCount > 3) ? Convert.ToInt32(CS$<>8__locals1.elements.rect.width / (float)elementDisplayCount) : 100;
		foreach (object obj2 in CS$<>8__locals1.elements)
		{
			RectTransform element3 = (RectTransform)obj2;
			bool fakeActive = element3.gameObject.GetOrAddComponent<FakeActiveController>().FakeActive;
			if (fakeActive)
			{
				element3.transform.localPosition = element3.transform.localPosition.SetX((float)elementPosition);
				elementPosition += elementPositionDelta;
			}
		}
		secretInformationView.CGet<TextMeshProUGUI>("Name").text = CS$<>8__locals1.config.Name;
		secretInformationView.CGet<TextMeshProUGUI>("HolderCount").text = secretInformationDisplayData.HolderCount.ToString();
		secretInformationView.CGet<TextMeshProUGUI>("HolderCount").transform.parent.gameObject.SetActive(secretInformationDisplayData.HolderCount > 0);
		TextMeshProUGUI authorityText = secretInformationView.CGet<TextMeshProUGUI>("AuthorityCost");
		authorityText.text = secretInformationDisplayData.AuthorityCostWhenDisseminating.ToString();
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		ResourceMonitor monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(taiwuCharId, false);
		int authority = monitor.Resources[7];
		authorityText.text = ((authority > secretInformationDisplayData.AuthorityCostWhenDisseminating) ? secretInformationDisplayData.AuthorityCostWhenDisseminating.ToString() : secretInformationDisplayData.AuthorityCostWhenDisseminating.ToString().SetColor("FavorabilityType_Hateful6"));
		int usedCount = secretInformationDisplayData.UsedCount;
		int usedCountMax = (secretInformationDisplayData.AuthorityCostWhenDisseminating == 0) ? GlobalConfig.Instance.SecretInformationInBroadcastMaxUseCount : GlobalConfig.Instance.SecretInformationInPrivateMaxUseCount;
		int remainCount = Math.Max(usedCountMax - usedCount, 0);
		string text = ((remainCount <= 0) ? string.Format("<color=#7F7F7F>{0}</color>", remainCount) : string.Format("{0}", remainCount)) + " " + LocalStringManager.Get(LanguageKey.LK_Side_Quest_Icon_Text);
		text.ColorReplace();
		secretInformationView.CGet<TextMeshProUGUI>("UsedCount").text = text;
		toggle.interactable = (usedCount < usedCountMax);
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000FF6EC File Offset: 0x000FD8EC
	public unsafe static string MakeSecretInformationDescription(SecretSortAndFilterData data)
	{
		List<string> args = new List<string>();
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		int* charIds2 = stackalloc int[(UIntPtr)16];
		int* charIds = charIds2;
		SecretInformationItem config = SecretInformation.Instance[data.Data.SecretInformationTemplateId];
		byte[] parametersPack = data.Data.ParametersPack;
		if (parametersPack != null)
		{
			parametersPack.ExtractSecretParameters(config, delegate(int i, int charId)
			{
				CharacterDisplayData characterDisplayData3;
				args.Add(data.Characters.TryGetValue(charId, out characterDisplayData3) ? NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData3, charId == taiwuCharId) : string.Format("{0}", charId));
				charIds[i] = charId;
			}, delegate(int _, Location location)
			{
				bool flag3 = mapModel.Areas.CheckIndex((int)location.AreaId);
				if (flag3)
				{
					MapAreaData area = mapModel.Areas[(int)location.AreaId];
					args.Add(area.GetConfig().Name ?? "");
				}
				else
				{
					args.Add(LocalStringManager.Get(LanguageKey.LK_Unknown_Area_Name));
				}
			}, delegate(int _, sbyte _)
			{
			}, delegate(int _, ItemKey itemKey)
			{
				args.Add(ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId));
			}, delegate(int _, short templateId)
			{
				args.Add(CombatSkill.Instance[templateId].Name);
			}, delegate(int _, short templateId)
			{
				args.Add(LifeSkill.Instance[templateId].Name);
			}, delegate(int _, int _)
			{
			});
		}
		while (args.Count < 4)
		{
			args.Add(string.Empty);
		}
		short templateId2 = config.TemplateId;
		short num = templateId2;
		if (num != 39)
		{
			if (num == 40)
			{
				int charId3 = charIds[1];
				CharacterDisplayData characterDisplayData;
				bool flag = data.Characters.TryGetValue(charId3, out characterDisplayData);
				if (flag)
				{
					args[2] = ((characterDisplayData.Gender == 0) ? LocalStringManager.Get(LanguageKey.LK_Relation_StepChild_Daughter) : LocalStringManager.Get(LanguageKey.LK_Relation_StepChild_Son));
				}
			}
		}
		else
		{
			int charId2 = charIds[1];
			CharacterDisplayData characterDisplayData2;
			bool flag2 = data.Characters.TryGetValue(charId2, out characterDisplayData2);
			if (flag2)
			{
				args[2] = ((characterDisplayData2.Gender == 0) ? LocalStringManager.Get(LanguageKey.LK_Relation_StepParent_Mother) : LocalStringManager.Get(LanguageKey.LK_Relation_StepParent_Father));
			}
		}
		int j = 0;
		int count = args.Count;
		while (j < count)
		{
			args[j] = "<color=#pinkyellow>" + args[j] + "</color>";
			j++;
		}
		return config.Desc.GetFormat(new object[]
		{
			args[0],
			args[1],
			args[2],
			args[3]
		}).ColorReplace();
	}

	// Token: 0x04001A86 RID: 6790
	public static List<short> NonTaiwuSortId = new List<short>
	{
		159,
		160,
		161,
		162,
		163,
		164
	};

	// Token: 0x04001A87 RID: 6791
	public static List<short> TaiwuSortId = new List<short>
	{
		159,
		160,
		161,
		162,
		163
	};
}
