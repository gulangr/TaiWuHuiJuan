using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.SortAndFilter.Secret;
using Game.Views.MouseTips;
using GameData.Domains.Character.Display;
using GameData.Utilities.Information;

namespace Game.Views.TipsBuilders
{
	// Token: 0x0200074F RID: 1871
	[CommonTipBuilder(38, TipType.SecretInformation)]
	public class CommonTipsBuilder_SecretInformation : ICommonTipBuilder
	{
		// Token: 0x06005AD1 RID: 23249 RVA: 0x002A20D0 File Offset: 0x002A02D0
		public ArgumentBox BuildTip(CommonTipItem commonTipItem, ArgumentBox arg)
		{
			CommonTipSimpleRuntime runtime = CommonTipsHelper.GetOrCreateSimpleRuntimeTipForBuild(commonTipItem, arg);
			runtime.ShowAllAtoms();
			SecretSortAndFilterData data;
			bool flag = !arg.Get<SecretSortAndFilterData>("SecretSortAndFilterData", out data) || data == null;
			ArgumentBox result;
			if (flag)
			{
				result = arg;
			}
			else
			{
				SecretInformationItem config = SecretInformation.Instance[data.Data.SecretInformationTemplateId];
				BasicGameData basicData = SingletonObject.getInstance<BasicGameData>();
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				int taiwuCharId = basicData.TaiwuCharId;
				int remainTime = (int)config.Duration - (basicData.CurrDate - data.Data.OccurenceDate);
				runtime.Set("SecretInformation.Name", config.Name);
				runtime.Set("SecretInformation.Desc", InformationUtils.MakeSecretInformationDescription(data));
				runtime.Set("SecretInformation.Location", CommonTipsBuilder_SecretInformation.BuildLocation(data, mapModel));
				runtime.Set("SecretInformation.Actor", null);
				runtime.Set("SecretInformation.RelatedCharacters", null);
				runtime.Set("SecretInformation.RemainCount", data.CanUseCount.ToString().SetColor("pinkyellow"));
				runtime.Set("SecretInformation.AuthorityCost", data.Data.AuthorityCostWhenDisseminating.ToString());
				runtime.Set("SecretInformation.HolderCount", (data.Data.HolderCount > 0) ? data.Data.HolderCount.ToString() : null);
				runtime.Set("SecretInformation.RemainTime", CommonTipsBuilder_SecretInformation.BuildRemainTime(remainTime));
				runtime.Set("SecretInformation.HolderLimitDesc", (data.Data.HolderCount > 0) ? LocalStringManager.GetFormat(LanguageKey.LK_SecretInformation_HolderCount, config.MaxPersonAmount.ToString().SetColor("pinkyellow")).ColorReplace() : null);
				runtime.Set("SecretInformation.SourceCharacter", CommonTipsBuilder_SecretInformation.BuildSourceCharacter(data, taiwuCharId));
				runtime.Set("SecretInformation.BroadcastDesc", string.IsNullOrEmpty(config.BroadcastDesc) ? null : config.BroadcastDesc.ColorReplace());
				CommonTipsBuilder_SecretInformation.BuildParticipantTexts(runtime, data, config, taiwuCharId);
				bool hasLocation = !string.IsNullOrEmpty(runtime.GetArgument("SecretInformation.Location"));
				bool hasActor = !string.IsNullOrEmpty(runtime.GetArgument("SecretInformation.Actor"));
				bool hasRelated = !string.IsNullOrEmpty(runtime.GetArgument("SecretInformation.RelatedCharacters"));
				bool hasHolderCount = data.Data.HolderCount > 0;
				bool hasHolderLimitDesc = !string.IsNullOrEmpty(runtime.GetArgument("SecretInformation.HolderLimitDesc"));
				bool hasSourceCharacter = !string.IsNullOrEmpty(runtime.GetArgument("SecretInformation.SourceCharacter"));
				bool hasBroadcastDesc = !string.IsNullOrEmpty(runtime.GetArgument("SecretInformation.BroadcastDesc"));
				runtime.SetAtomVisible("Default", "location", hasLocation);
				runtime.SetAtomVisible("Default", "actor", hasActor);
				runtime.SetAtomVisible("Default", "related", hasRelated);
				runtime.SetAtomVisible("Default", "holderCount", hasHolderCount);
				runtime.SetAtomVisible("Default", "holderLimitDesc", hasHolderLimitDesc);
				runtime.SetAtomVisible("Default", "sourceCharacter", hasSourceCharacter);
				runtime.SetParagraphVisible("Public", hasBroadcastDesc);
				result = arg;
			}
			return result;
		}

		// Token: 0x06005AD2 RID: 23250 RVA: 0x002A23C8 File Offset: 0x002A05C8
		private static void BuildParticipantTexts(CommonTipSimpleRuntime runtime, SecretSortAndFilterData data, SecretInformationItem config, int taiwuCharId)
		{
			SecretInformationEffectItem effect = SecretInformationEffect.Instance[config.DefaultEffectId];
			List<string> relatedCharacters = new List<string>();
			string actor = null;
			byte[] parametersPack = data.Data.ParametersPack;
			if (parametersPack != null)
			{
				parametersPack.ExtractSecretParameters(config, delegate(int index, int charId)
				{
					string displayName = CommonTipsBuilder_SecretInformation.GetCharacterName(data, charId, taiwuCharId);
					bool flag = string.IsNullOrEmpty(displayName);
					if (!flag)
					{
						bool flag2 = effect.ActorIndex == index;
						if (flag2)
						{
							actor = displayName;
						}
						else
						{
							relatedCharacters.Add(displayName);
						}
					}
				});
			}
			runtime.Set("SecretInformation.Actor", actor);
			runtime.Set("SecretInformation.RelatedCharacters", (relatedCharacters.Count > 0) ? string.Join("、", relatedCharacters) : null);
		}

		// Token: 0x06005AD3 RID: 23251 RVA: 0x002A247C File Offset: 0x002A067C
		private static string BuildLocation(SecretSortAndFilterData data, WorldMapModel mapModel)
		{
			return (data.Data.Location.areaTemplateId >= 0) ? mapModel.GetFullBlockName(data.Data.Location, true, true, true, true).SetColor("pinkyellow") : null;
		}

		// Token: 0x06005AD4 RID: 23252 RVA: 0x002A24C4 File Offset: 0x002A06C4
		private static string BuildRemainTime(int remainMonths)
		{
			return (remainMonths >= 0) ? remainMonths.ToString() : "-";
		}

		// Token: 0x06005AD5 RID: 23253 RVA: 0x002A24E8 File Offset: 0x002A06E8
		private static string BuildSourceCharacter(SecretSortAndFilterData data, int taiwuCharId)
		{
			bool flag = data.Data.SourceCharacterId < 0;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = (CommonTipsBuilder_SecretInformation.GetCharacterName(data, data.Data.SourceCharacterId, taiwuCharId) ?? data.Data.SourceCharacterId.ToString().SetColor("pinkyellow"));
			}
			return result;
		}

		// Token: 0x06005AD6 RID: 23254 RVA: 0x002A2540 File Offset: 0x002A0740
		private static string GetCharacterName(SecretSortAndFilterData data, int charId, int taiwuCharId)
		{
			CharacterDisplayData displayData;
			bool flag = !data.Characters.TryGetValue(charId, out displayData);
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = NameCenter.GetCharMonasticTitleAndNameByDisplayData(displayData, displayData.CharacterId == taiwuCharId, false).SetColorIfNotEmpty("pinkyellow");
			}
			return result;
		}

		// Token: 0x04003E94 RID: 16020
		private const string ParagraphDefault = "Default";

		// Token: 0x04003E95 RID: 16021
		private const string PinkYellow = "pinkyellow";
	}
}
