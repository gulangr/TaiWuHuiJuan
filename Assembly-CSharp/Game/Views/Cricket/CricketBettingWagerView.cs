using System;
using Config;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views.Cricket
{
	// Token: 0x02000AAF RID: 2735
	public class CricketBettingWagerView : MonoBehaviour
	{
		// Token: 0x0600864D RID: 34381 RVA: 0x003E842C File Offset: 0x003E662C
		public void SetData(Wager wager, long value = 0L)
		{
			CricketBettingWagerView.<>c__DisplayClass10_0 CS$<>8__locals1 = new CricketBettingWagerView.<>c__DisplayClass10_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.value = value;
			CS$<>8__locals1.wager = wager;
			CricketBettingWagerView.<>c__DisplayClass10_0 CS$<>8__locals2 = CS$<>8__locals1;
			int version = this.renderVersion + 1;
			this.renderVersion = version;
			CS$<>8__locals2.version = version;
			sbyte type = CS$<>8__locals1.wager.Type;
			sbyte b = type;
			if (b != 1)
			{
				if (b != 2)
				{
					this.Render(CS$<>8__locals1.wager, null, null, CS$<>8__locals1.value);
				}
				else
				{
					this.Render(CS$<>8__locals1.wager, null, null, CS$<>8__locals1.value);
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, CS$<>8__locals1.wager.CharId, delegate(int offset, RawDataPool dataPool)
					{
						bool flag = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this.renderVersion;
						if (!flag)
						{
							CharacterDisplayData characterDisplayData = null;
							Serializer.Deserialize(dataPool, offset, ref characterDisplayData);
							CS$<>8__locals1.<>4__this.Render(CS$<>8__locals1.wager, null, characterDisplayData, CS$<>8__locals1.value);
						}
					});
				}
			}
			else
			{
				this.Render(CS$<>8__locals1.wager, null, null, CS$<>8__locals1.value);
				ItemDomainMethod.AsyncCall.GetItemDisplayData(null, CS$<>8__locals1.wager.ItemKey, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					bool flag = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this.renderVersion;
					if (!flag)
					{
						ItemDisplayData itemDisplayData = null;
						Serializer.Deserialize(dataPool, offset, ref itemDisplayData);
						long num2;
						if (CS$<>8__locals1.value <= 0L)
						{
							long? num = (itemDisplayData != null) ? new long?(itemDisplayData.Value) : null;
							num2 = CS$<>8__locals1.wager.CalcWagerValue(((num != null) ? new int?((int)num.GetValueOrDefault()) : null).GetValueOrDefault(), 0, 0, 0, -1, 0);
						}
						else
						{
							num2 = CS$<>8__locals1.value;
						}
						long finalValue = num2;
						CS$<>8__locals1.<>4__this.Render(CS$<>8__locals1.wager, itemDisplayData, null, finalValue);
					}
				});
			}
		}

		// Token: 0x0600864E RID: 34382 RVA: 0x003E850F File Offset: 0x003E670F
		public void SetData(Wager wager, ItemDisplayData itemDisplayData, long value = 0L)
		{
			this.renderVersion++;
			this.Render(wager, itemDisplayData, null, value);
		}

		// Token: 0x0600864F RID: 34383 RVA: 0x003E852C File Offset: 0x003E672C
		private void Render(Wager wager, ItemDisplayData itemDisplayData, CharacterDisplayData characterDisplayData, long value)
		{
			bool isInvalid = wager.Type == -1;
			this.emptyRoot.SetActive(isInvalid);
			this.contentRoot.SetActive(!isInvalid);
			this.wagerItemRoot.SetActive(false);
			this.cardItem.gameObject.SetActive(false);
			this.teammateMark.SetActive(false);
			this.prisonerMark.SetActive(false);
			switch (wager.Type)
			{
			case 0:
				this.wagerItemRoot.SetActive(true);
				this.SetCardItem(CricketBettingWagerView.CreateResourceContent(wager), false, new sbyte?(wager.Grade));
				break;
			case 1:
			{
				this.wagerItemRoot.SetActive(true);
				bool flag = itemDisplayData != null;
				if (flag)
				{
					this.SetCardItem(itemDisplayData, true, null);
				}
				break;
			}
			case 2:
			{
				this.wagerItemRoot.SetActive(true);
				bool isTeammate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(wager.CharId);
				this.teammateMark.SetActive(isTeammate);
				this.prisonerMark.SetActive(!isTeammate);
				bool flag2 = characterDisplayData != null;
				if (flag2)
				{
					this.SetCardItem(CricketBettingWagerView.CreateCharacterContent(characterDisplayData), true, null);
				}
				break;
			}
			case 3:
				this.wagerItemRoot.SetActive(true);
				this.SetCardItem(CricketBettingWagerView.CreateExpContent(wager), false, new sbyte?(wager.Grade));
				break;
			}
			this.SetInfo(wager, itemDisplayData, characterDisplayData, value);
		}

		// Token: 0x06008650 RID: 34384 RVA: 0x003E86B8 File Offset: 0x003E68B8
		private void SetInfo(Wager wager, ItemDisplayData itemDisplayData, CharacterDisplayData characterDisplayData, long value)
		{
			this.infoNameText.text = this.GetInfoName(wager, itemDisplayData, characterDisplayData);
			long wagerValue = (value > 0L) ? value : CricketBettingWagerView.GetDefaultValue(wager, itemDisplayData);
			this.infoValueText.text = CommonUtils.GetDisplayStringForNum(wagerValue);
		}

		// Token: 0x06008651 RID: 34385 RVA: 0x003E8700 File Offset: 0x003E6900
		private string GetInfoName(Wager wager, ItemDisplayData itemDisplayData, CharacterDisplayData characterDisplayData)
		{
			sbyte type = wager.Type;
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case -1:
				result = LocalStringManager.Get("LK_CricketWager_Invalid");
				break;
			case 0:
				result = Config.ResourceType.Instance[wager.WagerResourceType].Name;
				break;
			case 1:
				result = (((itemDisplayData != null) ? itemDisplayData.GetName(false) : null) ?? ItemTemplateHelper.GetName(wager.ItemKey.ItemType, wager.ItemKey.TemplateId));
				break;
			case 2:
				result = ((characterDisplayData == null) ? string.Empty : NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId));
				break;
			case 3:
				result = LocalStringManager.Get(LanguageKey.LK_Exp);
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008652 RID: 34386 RVA: 0x003E87D4 File Offset: 0x003E69D4
		private void SetCardItem(ITradeableContent content, bool showTip, sbyte? gradeOverride = null)
		{
			this.cardItemRowItemMain.SetData(content);
			this.cardItem.Set(this.cardItemRowItemMain, showTip);
			bool flag = gradeOverride != null;
			if (flag)
			{
				this.cardItemRowItemMain.ItemBack.SetGradeOverride(gradeOverride.Value);
			}
			this.cardItem.gameObject.SetActive(true);
		}

		// Token: 0x06008653 RID: 34387 RVA: 0x003E8838 File Offset: 0x003E6A38
		private static long GetDefaultValue(Wager wager, ItemDisplayData itemDisplayData)
		{
			sbyte type = wager.Type;
			if (!true)
			{
			}
			long result;
			switch (type)
			{
			case -1:
				result = 0L;
				goto IL_94;
			case 1:
			{
				long? num = (itemDisplayData != null) ? new long?(itemDisplayData.Value) : null;
				result = wager.CalcWagerValue(((num != null) ? new int?((int)num.GetValueOrDefault()) : null).GetValueOrDefault(), 0, 0, 0, -1, 0);
				goto IL_94;
			}
			case 2:
				result = 0L;
				goto IL_94;
			}
			result = wager.CalcWagerValue(0, 0, 0, 0, -1, 0);
			IL_94:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008654 RID: 34388 RVA: 0x003E88E4 File Offset: 0x003E6AE4
		private static ItemDisplayData CreateExpContent(Wager wager)
		{
			return new ItemDisplayData(12, 8)
			{
				Amount = wager.Count
			};
		}

		// Token: 0x06008655 RID: 34389 RVA: 0x003E890C File Offset: 0x003E6B0C
		private static ItemDisplayData CreateResourceContent(Wager wager)
		{
			return ItemDisplayData.CreateResource(wager.WagerResourceType, wager.Count, -1);
		}

		// Token: 0x06008656 RID: 34390 RVA: 0x003E8930 File Offset: 0x003E6B30
		private static ITradeableContent CreateCharacterContent(CharacterDisplayData displayData)
		{
			return new CricketCombatRewardCharacterContent(displayData);
		}

		// Token: 0x04006722 RID: 26402
		[SerializeField]
		private GameObject emptyRoot;

		// Token: 0x04006723 RID: 26403
		[SerializeField]
		private GameObject contentRoot;

		// Token: 0x04006724 RID: 26404
		[SerializeField]
		private TextMeshProUGUI infoNameText;

		// Token: 0x04006725 RID: 26405
		[FormerlySerializedAs("infoCountText")]
		[SerializeField]
		private TextMeshProUGUI infoValueText;

		// Token: 0x04006726 RID: 26406
		[SerializeField]
		private GameObject wagerItemRoot;

		// Token: 0x04006727 RID: 26407
		[SerializeField]
		private CardItem cardItem;

		// Token: 0x04006728 RID: 26408
		[SerializeField]
		private RowItemMain cardItemRowItemMain;

		// Token: 0x04006729 RID: 26409
		[SerializeField]
		private GameObject teammateMark;

		// Token: 0x0400672A RID: 26410
		[SerializeField]
		private GameObject prisonerMark;

		// Token: 0x0400672B RID: 26411
		private int renderVersion;
	}
}
