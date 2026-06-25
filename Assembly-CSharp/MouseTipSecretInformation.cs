using System;
using Config;
using FrameWork;
using Game.Components.SortAndFilter.Secret;
using GameData.Domains.Character.Display;
using GameData.Utilities.Information;
using TMPro;

// Token: 0x020002CE RID: 718
public class MouseTipSecretInformation : MouseTipBase
{
	// Token: 0x06002B22 RID: 11042 RVA: 0x0014F520 File Offset: 0x0014D720
	protected override void Init(ArgumentBox argsBox)
	{
		SecretSortAndFilterData data;
		bool flag = argsBox.Get<SecretSortAndFilterData>("SecretSortAndFilterData", out data);
		if (flag)
		{
			this.Data = data;
		}
		bool flag2 = this.Data == null;
		if (!flag2)
		{
			this.Desc.text = InformationUtils.MakeSecretInformationDescription(this.Data);
			BasicGameData basicData = SingletonObject.getInstance<BasicGameData>();
			SecretInformationItem config = SecretInformation.Instance[this.Data.Data.SecretInformationTemplateId];
			int usedCount = this.Data.Data.UsedCount;
			int usedCountMax = (this.Data.Data.AuthorityCostWhenDisseminating == 0) ? GlobalConfig.Instance.SecretInformationInBroadcastMaxUseCount : GlobalConfig.Instance.SecretInformationInPrivateMaxUseCount;
			int remainCount = Math.Max(usedCountMax - usedCount, 0);
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.Name.text = config.Name;
			this.Info.text = string.Empty;
			int relatedCount = 0;
			SecretInformationEffectItem effect = SecretInformationEffect.Instance[config.DefaultEffectId];
			TextMeshProUGUI info = this.Info;
			info.text = info.text + "·<color=#pinkyellow>" + LanguageKey.LK_Brackets_Symbol.TrFormat(LanguageKey.LK_SecretInformation_Tips_Info.Tr()) + "</color>\n";
			bool flag3 = this.Data.Data.Location.areaTemplateId >= 0;
			TextMeshProUGUI info2;
			if (flag3)
			{
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				info2 = this.Info;
				info2.text = string.Concat(new string[]
				{
					info2.text,
					"  ·",
					LocalStringManager.Get(LanguageKey.LK_SecretInformation_Tips_Location_Simple),
					": <color=#pinkyellow>",
					mapModel.GetFullBlockName(this.Data.Data.Location, true, true, true, true),
					"</color>\n"
				});
			}
			byte[] parametersPack = this.Data.Data.ParametersPack;
			if (parametersPack != null)
			{
				parametersPack.ExtractSecretParameters(config, delegate(int i, int charId)
				{
					CharacterDisplayData characterDisplayData;
					bool flag9 = this.Data.Characters.TryGetValue(charId, out characterDisplayData);
					if (flag9)
					{
						string charName = NameCenter.GetCharMonasticTitleAndNameByDisplayData(characterDisplayData, characterDisplayData.CharacterId == taiwuCharId, false);
						bool flag10 = effect.ActorIndex == i;
						if (flag10)
						{
							TextMeshProUGUI info17 = this.Info;
							info17.text = string.Concat(new string[]
							{
								info17.text,
								"  ·",
								LocalStringManager.Get(LanguageKey.LK_SecretInformation_Tips_Actor),
								": <color=#pinkyellow>",
								charName,
								"</color>\n"
							});
						}
						else
						{
							bool flag11 = relatedCount <= 0;
							if (flag11)
							{
								TextMeshProUGUI info17 = this.Info;
								info17.text = string.Concat(new string[]
								{
									info17.text,
									"  ·",
									LocalStringManager.Get(LanguageKey.LK_SecretInformation_Tips_Related),
									": <color=#pinkyellow>",
									charName
								});
							}
							else
							{
								TextMeshProUGUI info18 = this.Info;
								info18.text = info18.text + "、" + charName;
							}
							relatedCount++;
						}
					}
				});
			}
			TextMeshProUGUI info3 = this.Info;
			info3.text = info3.text + "·<color=#pinkyellow>" + LanguageKey.LK_Brackets_Symbol.TrFormat(LanguageKey.LK_SecretInformation_Tips_Use.Tr()) + "</color>\n";
			TextMeshProUGUI info4 = this.Info;
			info4.text += string.Format("  ·{0}: <color=#pinkyellow>{1}</color>\n", LocalStringManager.Get(LanguageKey.LK_SecretInformation_Tips_RemainCount_Simple), remainCount);
			TextMeshProUGUI info5 = this.Info;
			info5.text += string.Format("  ·{0}: <SpName=mousetip_ziyuan_7><color=#B975FFFF>{1}{2}</color>\n", LocalStringManager.Get(LanguageKey.LK_SecretInformation_AuthorityCost_Simple), this.Data.Data.AuthorityCostWhenDisseminating, LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority));
			bool flag4 = this.Data.Data.HolderCount > 0;
			if (flag4)
			{
				TextMeshProUGUI info6 = this.Info;
				info6.text += string.Format("  ·{0}: <color=#pinkyellow>{1}</color>\n", LocalStringManager.Get(LanguageKey.LK_SecretInformation_HolderCount_Simple), this.Data.Data.HolderCount);
			}
			int remainTime = (int)config.Duration - (basicData.CurrDate - this.Data.Data.OccurenceDate);
			info2 = this.Info;
			info2.text = string.Concat(new string[]
			{
				info2.text,
				"  ·",
				LocalStringManager.Get(LanguageKey.LK_SecretInformation_RemainTime_Simple),
				": <color=#pinkyellow>",
				(remainTime >= 0) ? string.Format("<SpName=mousetip_shijie>{0}", remainTime) : "-",
				"</color>\n"
			});
			TextMeshProUGUI info7 = this.Info;
			info7.text += "\n";
			bool flag5 = this.Data.Data.HolderCount > 0;
			if (flag5)
			{
				TextMeshProUGUI info8 = this.Info;
				info8.text = info8.text + LocalStringManager.GetFormat(LanguageKey.LK_SecretInformation_HolderCount, string.Format("<color=#pinkyellow>{0}</color>", config.MaxPersonAmount)) + "\n";
			}
			TextMeshProUGUI info9 = this.Info;
			info9.text = info9.text + LocalStringManager.GetFormat(LanguageKey.LK_SecretInformation_RemainTime_Fake, LocalStringManager.Get(LanguageKey.LK_SecretInformation_RemainTime_Simple), string.Format("<color=#pinkyellow>{0}</color>", 0)) + "\n";
			CharacterDisplayData sourceData;
			bool flag6 = this.Data.Data.SourceCharacterId >= 0 && this.Data.Characters.TryGetValue(this.Data.Data.SourceCharacterId, out sourceData);
			if (flag6)
			{
				TextMeshProUGUI info10 = this.Info;
				info10.text = info10.text + "  ·" + LocalStringManager.GetFormat(LanguageKey.LK_SecretInformation_SourceCharacter, "<color=#pinkyellow>" + NameCenter.GetMonasticTitleOrDisplayName(sourceData, sourceData.CharacterId == taiwuCharId) + "</color>") + "\n";
			}
			TextMeshProUGUI info11 = this.Info;
			info11.text += "\n";
			bool flag7 = !string.IsNullOrEmpty(config.BroadcastDesc);
			if (flag7)
			{
				TextMeshProUGUI info12 = this.Info;
				info12.text = info12.text + "·<color=#pinkyellow>" + LanguageKey.LK_Brackets_Symbol.TrFormat(LanguageKey.LK_SecretInformation_Tips_Broadcast.Tr()) + "</color>\n";
				TextMeshProUGUI info13 = this.Info;
				info13.text += config.BroadcastDesc;
				TextMeshProUGUI info14 = this.Info;
				info14.text += "\n";
			}
			this.Info.text = this.Info.text.ColorReplace();
			bool flag8 = relatedCount > 0;
			if (flag8)
			{
				TextMeshProUGUI info15 = this.Info;
				info15.text += "</color>\n";
			}
			TextMeshProUGUI info16 = this.Info;
			info16.text += "\n";
			TMPTextSpriteHelper helper = this.Info.GetComponent<TMPTextSpriteHelper>();
			helper.autoClear = false;
			helper.SpriteSizeFitType = TMPTextSpriteHelper.SizeFitType.Native;
			helper.Parse();
		}
	}

	// Token: 0x04001F23 RID: 7971
	public TextMeshProUGUI Name;

	// Token: 0x04001F24 RID: 7972
	public TextMeshProUGUI Desc;

	// Token: 0x04001F25 RID: 7973
	public TextMeshProUGUI Info;

	// Token: 0x04001F26 RID: 7974
	[NonSerialized]
	public SecretSortAndFilterData Data;
}
