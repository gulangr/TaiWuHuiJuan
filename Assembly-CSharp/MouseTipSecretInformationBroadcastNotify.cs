using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Utilities;
using GameData.Utilities.Information;
using TMPro;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class MouseTipSecretInformationBroadcastNotify : MouseTipBase
{
	// Token: 0x06002B24 RID: 11044 RVA: 0x0014FB40 File Offset: 0x0014DD40
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitUIReference();
		bool flag = !argsBox.Get<SecretInformationBroadcastTipsData>("Data", out this._tipsData);
		if (flag)
		{
			Debug.LogError("can not show MouseTipSecretInformationBroadcastNotify : no Data set to argument box!");
			this.QuickHide();
		}
		else
		{
			argsBox.Get<SecretInformationBroadcastTipsExtraData>("ExtraData", out this._tipsExtraData);
			this.CacheShowData();
			this.SetLabelValues();
			this.Element.OnActive = new Action(this.LayoutElements);
		}
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x0014FBBC File Offset: 0x0014DDBC
	private void Awake()
	{
		this.InitUIReference();
		MonthlyNotificationItem config = MonthlyNotification.Instance.GetItem(216);
		base.CGet<TextMeshProUGUI>("Title").text = config.Name;
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x0014FBF8 File Offset: 0x0014DDF8
	protected override void OnDisable()
	{
		base.OnDisable();
		this._tipsData = null;
		this._dataCacheFlag = false;
		this._notifyReason = string.Empty;
		this._desc = string.Empty;
		this._fameInfluenceOfMainCharacter = string.Empty;
		this._fameInfluenceOfTargetCharacter1 = string.Empty;
		this._fameInfluenceOfTargetCharacter2 = string.Empty;
		this._happinessUp = string.Empty;
		this._happinessDown = string.Empty;
		this._favorToMainCharacterUp = string.Empty;
		this._favorToTargetCharacter1Up = string.Empty;
		this._favorToTargetCharacter2Up = string.Empty;
		this._favorToMainCharacterDown = string.Empty;
		this._favorToTargetCharacter1Down = string.Empty;
		this._favorToTargetCharacter2Down = string.Empty;
		this._startEnemyRelationToActor = string.Empty;
		this._startEnemyRelationToReactor = string.Empty;
		this._startEnemyRelationToSecactor = string.Empty;
		this._startEnemyRelationToSource = string.Empty;
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x0014FCD8 File Offset: 0x0014DED8
	private void SetLabelValues()
	{
		this._notifyReasonLabel.text = this._notifyReason.ColorReplace();
		this._descLabel.text = this._desc.ColorReplace();
		this._fameInfluenceOfMainCharacterLabel.text = this._fameInfluenceOfMainCharacter.ColorReplace();
		this._fameInfluenceOfTargetCharacter1Label.text = this._fameInfluenceOfTargetCharacter1.ColorReplace();
		this._fameInfluenceOfTargetCharacter2Label.text = this._fameInfluenceOfTargetCharacter2.ColorReplace();
		this._happinessUpLabel.text = this._happinessUp.ColorReplace();
		this._happinessDownLabel.text = this._happinessDown.ColorReplace();
		this._favorToMainCharacterUpLabel.text = this._favorToMainCharacterUp.ColorReplace();
		this._favorToTargetCharacter1UpLabel.text = this._favorToTargetCharacter1Up.ColorReplace();
		this._favorToTargetCharacter2UpLabel.text = this._favorToTargetCharacter2Up.ColorReplace();
		this._favorToMainCharacterDownLabel.text = this._favorToMainCharacterDown.ColorReplace();
		this._favorToTargetCharacter1DownLabel.text = this._favorToTargetCharacter1Down.ColorReplace();
		this._favorToTargetCharacter2DownLabel.text = this._favorToTargetCharacter2Down.ColorReplace();
		this._startEnemyRelationToActorLabel.text = this._startEnemyRelationToActor.ColorReplace();
		this._startEnemyRelationToReactorLabel.text = this._startEnemyRelationToReactor.ColorReplace();
		this._startEnemyRelationToSecactorLabel.text = this._startEnemyRelationToSecactor.ColorReplace();
		this._startEnemyRelationToSourceLabel.text = this._startEnemyRelationToSource.ColorReplace();
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x0014FE70 File Offset: 0x0014E070
	private void LayoutElements()
	{
		float baseHeight = 108f;
		RectTransform selfRect = base.transform as RectTransform;
		bool flag = !string.IsNullOrEmpty(this._descLabel.text);
		if (flag)
		{
			this._descLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._descLabel.preferredHeight;
		}
		bool flag2 = !string.IsNullOrEmpty(this._fameInfluenceOfMainCharacterLabel.text);
		if (flag2)
		{
			baseHeight += 20f;
			this._fameInfluenceOfMainCharacterLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._fameInfluenceOfMainCharacterLabel.preferredHeight;
		}
		bool flag3 = !string.IsNullOrEmpty(this._fameInfluenceOfTargetCharacter1Label.text);
		if (flag3)
		{
			baseHeight += 20f;
			this._fameInfluenceOfTargetCharacter1Label.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._fameInfluenceOfTargetCharacter1Label.preferredHeight;
		}
		bool flag4 = !string.IsNullOrEmpty(this._fameInfluenceOfTargetCharacter2Label.text);
		if (flag4)
		{
			baseHeight += 20f;
			this._fameInfluenceOfTargetCharacter2Label.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._fameInfluenceOfTargetCharacter2Label.preferredHeight;
		}
		bool flag5 = !string.IsNullOrEmpty(this._happinessUpLabel.text) || !string.IsNullOrEmpty(this._happinessDownLabel.text);
		if (flag5)
		{
			baseHeight += 20f;
		}
		bool flag6 = !string.IsNullOrEmpty(this._happinessUpLabel.text);
		if (flag6)
		{
			this._happinessUpLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._happinessUpLabel.preferredHeight;
		}
		bool flag7 = !string.IsNullOrEmpty(this._happinessDownLabel.text);
		if (flag7)
		{
			this._happinessDownLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._happinessDownLabel.preferredHeight;
		}
		bool flag8 = !string.IsNullOrEmpty(this._favorToMainCharacterUpLabel.text) || !string.IsNullOrEmpty(this._favorToTargetCharacter1UpLabel.text) || !string.IsNullOrEmpty(this._favorToTargetCharacter2UpLabel.text) || !string.IsNullOrEmpty(this._favorToMainCharacterDownLabel.text) || !string.IsNullOrEmpty(this._favorToTargetCharacter1DownLabel.text) || !string.IsNullOrEmpty(this._favorToTargetCharacter2DownLabel.text);
		if (flag8)
		{
			baseHeight += 20f;
		}
		bool flag9 = !string.IsNullOrEmpty(this._favorToMainCharacterUpLabel.text);
		if (flag9)
		{
			this._favorToMainCharacterUpLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._favorToMainCharacterUpLabel.preferredHeight;
		}
		bool flag10 = !string.IsNullOrEmpty(this._favorToTargetCharacter1UpLabel.text);
		if (flag10)
		{
			this._favorToTargetCharacter1UpLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._favorToTargetCharacter1UpLabel.preferredHeight;
		}
		bool flag11 = !string.IsNullOrEmpty(this._favorToTargetCharacter2UpLabel.text);
		if (flag11)
		{
			this._favorToTargetCharacter2UpLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._favorToTargetCharacter2UpLabel.preferredHeight;
		}
		bool flag12 = !string.IsNullOrEmpty(this._favorToMainCharacterDownLabel.text);
		if (flag12)
		{
			this._favorToMainCharacterDownLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._favorToMainCharacterDownLabel.preferredHeight;
		}
		bool flag13 = !string.IsNullOrEmpty(this._favorToTargetCharacter1DownLabel.text);
		if (flag13)
		{
			this._favorToTargetCharacter1DownLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._favorToTargetCharacter1DownLabel.preferredHeight;
		}
		bool flag14 = !string.IsNullOrEmpty(this._favorToTargetCharacter2DownLabel.text);
		if (flag14)
		{
			this._favorToTargetCharacter2DownLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._favorToTargetCharacter2DownLabel.preferredHeight;
		}
		bool flag15 = !string.IsNullOrEmpty(this._startEnemyRelationToActorLabel.text) || !string.IsNullOrEmpty(this._startEnemyRelationToReactorLabel.text) || !string.IsNullOrEmpty(this._startEnemyRelationToSecactorLabel.text) || !string.IsNullOrEmpty(this._startEnemyRelationToSourceLabel.text);
		if (flag15)
		{
			baseHeight += 20f;
		}
		bool flag16 = !string.IsNullOrEmpty(this._startEnemyRelationToActorLabel.text);
		if (flag16)
		{
			this._startEnemyRelationToActorLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._startEnemyRelationToActorLabel.preferredHeight;
		}
		bool flag17 = !string.IsNullOrEmpty(this._startEnemyRelationToReactorLabel.text);
		if (flag17)
		{
			this._startEnemyRelationToReactorLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._startEnemyRelationToReactorLabel.preferredHeight;
		}
		bool flag18 = !string.IsNullOrEmpty(this._startEnemyRelationToSecactorLabel.text);
		if (flag18)
		{
			this._startEnemyRelationToSecactorLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._startEnemyRelationToSecactorLabel.preferredHeight;
		}
		bool flag19 = !string.IsNullOrEmpty(this._startEnemyRelationToSourceLabel.text);
		if (flag19)
		{
			this._startEnemyRelationToSourceLabel.rectTransform.anchoredPosition = Vector2.down * baseHeight;
			baseHeight += this._startEnemyRelationToSourceLabel.preferredHeight;
		}
		selfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight + 20f);
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x00150414 File Offset: 0x0014E614
	private void InitUIReference()
	{
		bool uiInitFlag = this._uiInitFlag;
		if (!uiInitFlag)
		{
			this._notifyReasonLabel = base.CGet<TextMeshProUGUI>("NotifyReason");
			this._descLabel = base.CGet<TextMeshProUGUI>("SecretInformationDesc");
			this._fameInfluenceOfMainCharacterLabel = base.CGet<TextMeshProUGUI>("MainCharacterFameInfluence");
			this._fameInfluenceOfTargetCharacter1Label = base.CGet<TextMeshProUGUI>("TargetCharacter1FameInfluence");
			this._fameInfluenceOfTargetCharacter2Label = base.CGet<TextMeshProUGUI>("TargetCharacter2FameInfluence");
			this._happinessUpLabel = base.CGet<TextMeshProUGUI>("HappinessUp");
			this._happinessDownLabel = base.CGet<TextMeshProUGUI>("HappinessDown");
			this._favorToMainCharacterUpLabel = base.CGet<TextMeshProUGUI>("FavorToMainCharacterUp");
			this._favorToTargetCharacter1UpLabel = base.CGet<TextMeshProUGUI>("FavorToTargetCharacter1Up");
			this._favorToTargetCharacter2UpLabel = base.CGet<TextMeshProUGUI>("FavorToTargetCharacter2Up");
			this._favorToMainCharacterDownLabel = base.CGet<TextMeshProUGUI>("FavorToMainCharacterDown");
			this._favorToTargetCharacter1DownLabel = base.CGet<TextMeshProUGUI>("FavorToTargetCharacter1Down");
			this._favorToTargetCharacter2DownLabel = base.CGet<TextMeshProUGUI>("FavorToTargetCharacter2Down");
			this._startEnemyRelationToActorLabel = base.CGet<TextMeshProUGUI>("StartEnemyRelationToActor");
			this._startEnemyRelationToReactorLabel = base.CGet<TextMeshProUGUI>("StartEnemyRelationToReactor");
			this._startEnemyRelationToSecactorLabel = base.CGet<TextMeshProUGUI>("StartEnemyRelationToSecactor");
			this._startEnemyRelationToSourceLabel = base.CGet<TextMeshProUGUI>("StartEnemyRelationToSource");
			this._uiInitFlag = true;
		}
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x0015055C File Offset: 0x0014E75C
	private string GetCharacterNameDisplayString(int charId)
	{
		string nameString = string.Empty;
		NameRelatedData nameRelatedData;
		bool flag = SecretInformationBroadcastTipsData.NameRelatedDataMap.TryGetValue(charId, out nameRelatedData);
		if (flag)
		{
			bool isTaiwu = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == charId;
			nameString = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, isTaiwu, false).SetColor("pinkyellow");
		}
		return nameString;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x001505AC File Offset: 0x0014E7AC
	private void CacheShowData()
	{
		MouseTipSecretInformationBroadcastNotify.<>c__DisplayClass45_0 CS$<>8__locals1 = new MouseTipSecretInformationBroadcastNotify.<>c__DisplayClass45_0();
		CS$<>8__locals1.<>4__this = this;
		bool dataCacheFlag = this._dataCacheFlag;
		if (!dataCacheFlag)
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			string taiwuName = this.GetCharacterNameDisplayString(taiwuCharId);
			bool flag = this._tipsData.BroadcastType == 0;
			if (flag)
			{
				this._notifyReason = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_RelatedInfo_Broadcasted, taiwuName);
			}
			else
			{
				bool flag2 = this._tipsData.BroadcastType == 1;
				if (flag2)
				{
					this._notifyReason = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_SelfDisseminate_Broadcasted, taiwuName);
				}
				else
				{
					this._notifyReason = string.Empty;
				}
			}
			this._desc = string.Empty;
			SecretInformationItem mainConfig = SecretInformation.Instance.GetItem(this._tipsData.DisplayData.SecretInformationTemplateId);
			SecretInformationEffectItem effectConfig = SecretInformationEffect.Instance.GetItem(mainConfig.DefaultEffectId);
			CS$<>8__locals1.charIds = EasyPool.Get<List<int>>();
			byte[] parametersPack = this._tipsData.DisplayData.ParametersPack;
			if (parametersPack != null)
			{
				parametersPack.ExtractSecretParameters(mainConfig, delegate(int _, int charId)
				{
					bool flag33 = SecretInformationBroadcastTipsData.DisplayPackage.CharacterData.ContainsKey(charId);
					if (flag33)
					{
						CS$<>8__locals1.charIds.Add(charId);
					}
				});
			}
			int mainCharacterId = -1;
			bool flag3 = CS$<>8__locals1.charIds.CheckIndex(effectConfig.ActorIndex);
			if (flag3)
			{
				mainCharacterId = CS$<>8__locals1.charIds[effectConfig.ActorIndex];
			}
			int targetCharacter1Id = -1;
			bool flag4 = CS$<>8__locals1.charIds.CheckIndex(effectConfig.ReactorIndex);
			if (flag4)
			{
				targetCharacter1Id = CS$<>8__locals1.charIds[effectConfig.ReactorIndex];
			}
			int targetCharacter2Id = -1;
			bool flag5 = CS$<>8__locals1.charIds.CheckIndex(effectConfig.SecactorIndex);
			if (flag5)
			{
				targetCharacter2Id = CS$<>8__locals1.charIds[effectConfig.SecactorIndex];
			}
			string mainCharacterName = (mainCharacterId >= 0) ? this.GetCharacterNameDisplayString(mainCharacterId) : string.Empty;
			string targetCharacter1Name = (targetCharacter1Id >= 0) ? this.GetCharacterNameDisplayString(targetCharacter1Id) : string.Empty;
			string targetCharacter2Name = (targetCharacter2Id >= 0) ? this.GetCharacterNameDisplayString(targetCharacter2Id) : string.Empty;
			string fameInfluenceBase = LocalStringManager.Get(LanguageKey.UI_MouseTip_SecretInforBroadcast_FameInfluenced);
			CS$<>8__locals1.stringResultCache = EasyPool.Get<List<string>>();
			bool flag6 = this._tipsData.FameActionsOfMain != null && this._tipsData.FameActionsOfMain.Count > 0;
			if (flag6)
			{
				CS$<>8__locals1.stringResultCache.Clear();
				CS$<>8__locals1.stringResultCache.Add(fameInfluenceBase.GetFormat(mainCharacterName));
				CS$<>8__locals1.<CacheShowData>g__DecodeFameActionList|1(this._tipsData.FameActionsOfMain);
				this._fameInfluenceOfMainCharacter = string.Join("\n", CS$<>8__locals1.stringResultCache);
			}
			else
			{
				this._fameInfluenceOfMainCharacter = string.Empty;
			}
			bool flag7 = this._tipsData.FameActionsOfTarget1 != null && this._tipsData.FameActionsOfTarget1.Count > 0;
			if (flag7)
			{
				CS$<>8__locals1.stringResultCache.Clear();
				CS$<>8__locals1.stringResultCache.Add(fameInfluenceBase.GetFormat(targetCharacter1Name));
				CS$<>8__locals1.<CacheShowData>g__DecodeFameActionList|1(this._tipsData.FameActionsOfTarget1);
				this._fameInfluenceOfTargetCharacter1 = string.Join("\n", CS$<>8__locals1.stringResultCache);
			}
			else
			{
				this._fameInfluenceOfTargetCharacter1 = string.Empty;
			}
			bool flag8 = this._tipsData.FameActionsOfTarget2 != null && this._tipsData.FameActionsOfTarget2.Count > 0;
			if (flag8)
			{
				CS$<>8__locals1.stringResultCache.Clear();
				CS$<>8__locals1.stringResultCache.Add(fameInfluenceBase.GetFormat(targetCharacter2Name));
				CS$<>8__locals1.<CacheShowData>g__DecodeFameActionList|1(this._tipsData.FameActionsOfTarget2);
				this._fameInfluenceOfTargetCharacter2 = string.Join("\n", CS$<>8__locals1.stringResultCache);
			}
			else
			{
				this._fameInfluenceOfTargetCharacter2 = string.Empty;
			}
			string moreString = LocalStringManager.Get(LanguageKey.UI_MouseTip_SecretInforBroadcast_AndMoreCharacters);
			bool flag9 = this._tipsData.HappinessUpCharacters != null && this._tipsData.HappinessUpCharacters.Count > 0;
			if (flag9)
			{
				bool needMoreString = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.HappinessUpCharacters);
				string names = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag10 = needMoreString;
				if (flag10)
				{
					names += moreString;
				}
				this._happinessUp = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersHappinessUp, names);
			}
			else
			{
				this._happinessUp = string.Empty;
			}
			bool flag11 = this._tipsData.HappinessDownCharacters != null && this._tipsData.HappinessDownCharacters.Count > 0;
			if (flag11)
			{
				bool needMoreString2 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.HappinessDownCharacters);
				string names2 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag12 = needMoreString2;
				if (flag12)
				{
					names2 += moreString;
				}
				this._happinessDown = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersHappinessDown, names2);
			}
			else
			{
				this._happinessDown = string.Empty;
			}
			bool flag13 = this._tipsData.FavorToMainUpCharacters != null && this._tipsData.FavorToMainUpCharacters.Count > 0;
			if (flag13)
			{
				bool needMoreString3 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.FavorToMainUpCharacters);
				string names3 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag14 = needMoreString3;
				if (flag14)
				{
					names3 += moreString;
				}
				this._favorToMainCharacterUp = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersFavorUp, names3, mainCharacterName);
			}
			else
			{
				this._favorToMainCharacterUp = string.Empty;
			}
			bool flag15 = this._tipsData.FavorToTarget1UpCharacters != null && this._tipsData.FavorToTarget1UpCharacters.Count > 0;
			if (flag15)
			{
				bool needMoreString4 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.FavorToTarget1UpCharacters);
				string names4 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag16 = needMoreString4;
				if (flag16)
				{
					names4 += moreString;
				}
				this._favorToTargetCharacter1Up = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersFavorUp, names4, targetCharacter1Name);
			}
			else
			{
				this._favorToTargetCharacter1Up = string.Empty;
			}
			bool flag17 = this._tipsData.FavorToTarget2UpCharacters != null && this._tipsData.FavorToTarget2UpCharacters.Count > 0;
			if (flag17)
			{
				bool needMoreString5 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.FavorToTarget2UpCharacters);
				string names5 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag18 = needMoreString5;
				if (flag18)
				{
					names5 += moreString;
				}
				this._favorToTargetCharacter2Up = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersFavorUp, names5, targetCharacter2Name);
			}
			else
			{
				this._favorToTargetCharacter2Up = string.Empty;
			}
			bool flag19 = this._tipsData.FavorToMainDownCharacters != null && this._tipsData.FavorToMainDownCharacters.Count > 0;
			if (flag19)
			{
				bool needMoreString6 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.FavorToMainDownCharacters);
				string names6 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag20 = needMoreString6;
				if (flag20)
				{
					names6 += moreString;
				}
				this._favorToMainCharacterDown = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersFavorDown, names6, mainCharacterName);
			}
			else
			{
				this._favorToMainCharacterDown = string.Empty;
			}
			bool flag21 = this._tipsData.FavorToTarget1DownCharacters != null && this._tipsData.FavorToTarget1DownCharacters.Count > 0;
			if (flag21)
			{
				bool needMoreString7 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.FavorToTarget1DownCharacters);
				string names7 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag22 = needMoreString7;
				if (flag22)
				{
					names7 += moreString;
				}
				this._favorToTargetCharacter1Down = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersFavorDown, names7, targetCharacter1Name);
			}
			else
			{
				this._favorToTargetCharacter1Down = string.Empty;
			}
			bool flag23 = this._tipsData.FavorToTarget2DownCharacters != null && this._tipsData.FavorToTarget2DownCharacters.Count > 0;
			if (flag23)
			{
				bool needMoreString8 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsData.FavorToTarget2DownCharacters);
				string names8 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag24 = needMoreString8;
				if (flag24)
				{
					names8 += moreString;
				}
				this._favorToTargetCharacter2Down = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_CharactersFavorDown, names8, targetCharacter2Name);
			}
			else
			{
				this._favorToTargetCharacter2Down = string.Empty;
			}
			bool flag25 = this._tipsExtraData != null && this._tipsExtraData.StartEnemyRelationCharactersToActor != null && this._tipsExtraData.StartEnemyRelationCharactersToActor.Count > 0;
			if (flag25)
			{
				bool needMoreString9 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsExtraData.StartEnemyRelationCharactersToActor);
				string names9 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag26 = needMoreString9;
				if (flag26)
				{
					names9 += moreString;
				}
				this._startEnemyRelationToActor = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_StartEmemyRelation, names9, mainCharacterName);
			}
			else
			{
				this._startEnemyRelationToActor = string.Empty;
			}
			bool flag27 = this._tipsExtraData != null && this._tipsExtraData.StartEnemyRelationCharactersToReactor != null && this._tipsExtraData.StartEnemyRelationCharactersToReactor.Count > 0;
			if (flag27)
			{
				bool needMoreString10 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsExtraData.StartEnemyRelationCharactersToReactor);
				string names10 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag28 = needMoreString10;
				if (flag28)
				{
					names10 += moreString;
				}
				this._startEnemyRelationToReactor = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_StartEmemyRelation, names10, targetCharacter1Name);
			}
			else
			{
				this._startEnemyRelationToReactor = string.Empty;
			}
			bool flag29 = this._tipsExtraData != null && this._tipsExtraData.StartEnemyRelationCharactersToSecactor != null && this._tipsExtraData.StartEnemyRelationCharactersToSecactor.Count > 0;
			if (flag29)
			{
				bool needMoreString11 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsExtraData.StartEnemyRelationCharactersToSecactor);
				string names11 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag30 = needMoreString11;
				if (flag30)
				{
					names11 += moreString;
				}
				this._startEnemyRelationToSecactor = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_StartEmemyRelation, names11, targetCharacter2Name);
			}
			else
			{
				this._startEnemyRelationToSecactor = string.Empty;
			}
			bool flag31 = this._tipsExtraData != null && this._tipsExtraData.StartEnemyRelationCharactersToSource != null && this._tipsExtraData.StartEnemyRelationCharactersToSource.Count > 1;
			if (flag31)
			{
				int sourceCharId = this._tipsExtraData.StartEnemyRelationCharactersToSource[0];
				this._tipsExtraData.StartEnemyRelationCharactersToSource.RemoveAt(0);
				string sourceCharName = (sourceCharId >= 0) ? this.GetCharacterNameDisplayString(sourceCharId) : string.Empty;
				bool needMoreString12 = CS$<>8__locals1.<CacheShowData>g__DecodeCharacterList|2(this._tipsExtraData.StartEnemyRelationCharactersToSource);
				string names12 = string.Join("、", CS$<>8__locals1.stringResultCache);
				bool flag32 = needMoreString12;
				if (flag32)
				{
					names12 += moreString;
				}
				this._startEnemyRelationToSource = LocalStringManager.GetFormat(LanguageKey.UI_MouseTip_SecretInforBroadcast_StartEmemyRelation, names12, sourceCharName);
				this._tipsExtraData.StartEnemyRelationCharactersToSource.Insert(0, sourceCharId);
			}
			else
			{
				this._startEnemyRelationToSource = string.Empty;
			}
			this._dataCacheFlag = true;
			this._tipsData = null;
			EasyPool.Free<List<string>>(CS$<>8__locals1.stringResultCache);
			EasyPool.Free<List<int>>(CS$<>8__locals1.charIds);
		}
	}

	// Token: 0x04001F27 RID: 7975
	private SecretInformationBroadcastTipsData _tipsData;

	// Token: 0x04001F28 RID: 7976
	private SecretInformationBroadcastTipsExtraData _tipsExtraData;

	// Token: 0x04001F29 RID: 7977
	private bool _dataCacheFlag;

	// Token: 0x04001F2A RID: 7978
	private bool _uiInitFlag;

	// Token: 0x04001F2B RID: 7979
	private string _notifyReason;

	// Token: 0x04001F2C RID: 7980
	private TextMeshProUGUI _notifyReasonLabel;

	// Token: 0x04001F2D RID: 7981
	private string _desc;

	// Token: 0x04001F2E RID: 7982
	private TextMeshProUGUI _descLabel;

	// Token: 0x04001F2F RID: 7983
	private string _fameInfluenceOfMainCharacter;

	// Token: 0x04001F30 RID: 7984
	private TextMeshProUGUI _fameInfluenceOfMainCharacterLabel;

	// Token: 0x04001F31 RID: 7985
	private string _fameInfluenceOfTargetCharacter1;

	// Token: 0x04001F32 RID: 7986
	private TextMeshProUGUI _fameInfluenceOfTargetCharacter1Label;

	// Token: 0x04001F33 RID: 7987
	private string _fameInfluenceOfTargetCharacter2;

	// Token: 0x04001F34 RID: 7988
	private TextMeshProUGUI _fameInfluenceOfTargetCharacter2Label;

	// Token: 0x04001F35 RID: 7989
	private string _happinessUp;

	// Token: 0x04001F36 RID: 7990
	private TextMeshProUGUI _happinessUpLabel;

	// Token: 0x04001F37 RID: 7991
	private string _happinessDown;

	// Token: 0x04001F38 RID: 7992
	private TextMeshProUGUI _happinessDownLabel;

	// Token: 0x04001F39 RID: 7993
	private string _favorToMainCharacterUp;

	// Token: 0x04001F3A RID: 7994
	private TextMeshProUGUI _favorToMainCharacterUpLabel;

	// Token: 0x04001F3B RID: 7995
	private string _favorToTargetCharacter1Up;

	// Token: 0x04001F3C RID: 7996
	private TextMeshProUGUI _favorToTargetCharacter1UpLabel;

	// Token: 0x04001F3D RID: 7997
	private string _favorToTargetCharacter2Up;

	// Token: 0x04001F3E RID: 7998
	private TextMeshProUGUI _favorToTargetCharacter2UpLabel;

	// Token: 0x04001F3F RID: 7999
	private string _favorToMainCharacterDown;

	// Token: 0x04001F40 RID: 8000
	private TextMeshProUGUI _favorToMainCharacterDownLabel;

	// Token: 0x04001F41 RID: 8001
	private string _favorToTargetCharacter1Down;

	// Token: 0x04001F42 RID: 8002
	private TextMeshProUGUI _favorToTargetCharacter1DownLabel;

	// Token: 0x04001F43 RID: 8003
	private string _favorToTargetCharacter2Down;

	// Token: 0x04001F44 RID: 8004
	private TextMeshProUGUI _favorToTargetCharacter2DownLabel;

	// Token: 0x04001F45 RID: 8005
	private string _startEnemyRelationToActor;

	// Token: 0x04001F46 RID: 8006
	private TextMeshProUGUI _startEnemyRelationToActorLabel;

	// Token: 0x04001F47 RID: 8007
	private string _startEnemyRelationToReactor;

	// Token: 0x04001F48 RID: 8008
	private TextMeshProUGUI _startEnemyRelationToReactorLabel;

	// Token: 0x04001F49 RID: 8009
	private string _startEnemyRelationToSecactor;

	// Token: 0x04001F4A RID: 8010
	private TextMeshProUGUI _startEnemyRelationToSecactorLabel;

	// Token: 0x04001F4B RID: 8011
	private string _startEnemyRelationToSource;

	// Token: 0x04001F4C RID: 8012
	private TextMeshProUGUI _startEnemyRelationToSourceLabel;
}
