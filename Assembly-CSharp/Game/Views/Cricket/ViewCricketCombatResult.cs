using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Cricket
{
	// Token: 0x02000ABD RID: 2749
	public class ViewCricketCombatResult : UIBase
	{
		// Token: 0x17000EDD RID: 3805
		// (get) Token: 0x06008756 RID: 34646 RVA: 0x003EF1C9 File Offset: 0x003ED3C9
		private int TaiwuHappinessDelta
		{
			get
			{
				return this._isWin ? GlobalConfig.Instance.OtherCombatWinHappiness[(int)this._taiwuChar.BehaviorType] : GlobalConfig.Instance.OtherCombatLoseHappiness[(int)this._taiwuChar.BehaviorType];
			}
		}

		// Token: 0x17000EDE RID: 3806
		// (get) Token: 0x06008757 RID: 34647 RVA: 0x003EF201 File Offset: 0x003ED401
		private int EnemyFavorabilityDelta
		{
			get
			{
				return this._isWin ? GlobalConfig.Instance.OtherCombatLoseFavorability[(int)this._enemyChar.BehaviorType] : GlobalConfig.Instance.OtherCombatWinFavorability[(int)this._enemyChar.BehaviorType];
			}
		}

		// Token: 0x06008758 RID: 34648 RVA: 0x003EF23C File Offset: 0x003ED43C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._rewardRequestVersion++;
			this._wager = Wager.Invalid;
			this._extraWager = Wager.Invalid;
			this._wagerChar = null;
			argsBox.Get("IsWin", out this._isWin);
			argsBox.Get<Wager>("Wager", out this._wager);
			argsBox.Get<CharacterDisplayData>("WagerChar", out this._wagerChar);
			argsBox.Get<Wager>("ExtraWager", out this._extraWager);
			argsBox.Get<CharacterDisplayData>("TaiwuChar", out this._taiwuChar);
			argsBox.Get<CharacterDisplayData>("EnemyChar", out this._enemyChar);
			this._hasData = true;
			this._rewardDataList.Clear();
			this._extraRewardDataList.Clear();
			this.RefreshRewardData();
			this.RefreshView();
		}

		// Token: 0x06008759 RID: 34649 RVA: 0x003EF30E File Offset: 0x003ED50E
		private void Awake()
		{
			this._uiReady = true;
			this.RefreshView();
		}

		// Token: 0x0600875A RID: 34650 RVA: 0x003EF320 File Offset: 0x003ED520
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600875B RID: 34651 RVA: 0x003EF350 File Offset: 0x003ED550
		public override void QuickHide()
		{
			UIManager.Instance.HideUI(UIElement.CricketCombat);
			UIManager.Instance.HideUI(this.Element);
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("CricketCombatOver", "WinState", this._isWin);
			TaiwuEventDomainMethod.Call.TriggerListener("CricketCombatOver", true);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
		}

		// Token: 0x0600875C RID: 34652 RVA: 0x003EF3AC File Offset: 0x003ED5AC
		private void RefreshRewardData()
		{
			this.BuildRewardData(this._wager, this._wagerChar, this._rewardDataList, this._isWin);
			this.BuildRewardData(this._extraWager, null, this._extraRewardDataList, this._isWin, true);
		}

		// Token: 0x0600875D RID: 34653 RVA: 0x003EF3E9 File Offset: 0x003ED5E9
		private void BuildRewardData(Wager wager, CharacterDisplayData wagerChar, List<CricketCombatRewardData> rewardDataList, bool isGain)
		{
			this.BuildRewardData(wager, wagerChar, rewardDataList, isGain, false);
		}

		// Token: 0x0600875E RID: 34654 RVA: 0x003EF3FC File Offset: 0x003ED5FC
		private void BuildRewardData(Wager wager, CharacterDisplayData wagerChar, List<CricketCombatRewardData> rewardDataList, bool isGain, bool isExtraReward)
		{
			switch (wager.Type)
			{
			case 0:
				rewardDataList.Add(this.CreateResourceRewardData(wager, isGain));
				break;
			case 1:
				this.RequestItemRewardData(wager, rewardDataList, isGain);
				break;
			case 2:
				if (isExtraReward)
				{
					Debug.LogWarning(string.Format("Extra cricket combat wager should not be character. CharId={0}", wager.CharId));
				}
				else
				{
					bool flag = wagerChar != null;
					if (flag)
					{
						rewardDataList.Add(this.CreateCharacterRewardData(wagerChar, isGain));
					}
					else
					{
						this.RequestCharacterRewardData(wager, rewardDataList, isGain);
					}
				}
				break;
			case 3:
				rewardDataList.Add(this.CreateExpRewardData(wager, isGain));
				break;
			}
		}

		// Token: 0x0600875F RID: 34655 RVA: 0x003EF4B0 File Offset: 0x003ED6B0
		private CricketCombatRewardData CreateResourceRewardData(Wager wager, bool isGain)
		{
			short templateId = (short)wager.WagerResourceType;
			ItemDisplayData itemData = new ItemDisplayData(12, templateId)
			{
				Amount = wager.Count
			};
			return new CricketCombatRewardData
			{
				Kind = CricketCombatRewardKind.Resource,
				Content = itemData,
				DisplayName = Config.ResourceType.Instance[wager.WagerResourceType].Name,
				Delta = (isGain ? wager.Count : (-wager.Count)),
				IsGain = isGain,
				ShowDelta = true,
				ShowCount = true
			};
		}

		// Token: 0x06008760 RID: 34656 RVA: 0x003EF53C File Offset: 0x003ED73C
		private CricketCombatRewardData CreateExpRewardData(Wager wager, bool isGain)
		{
			ItemDisplayData itemData = new ItemDisplayData(12, 8)
			{
				Amount = wager.Count
			};
			return new CricketCombatRewardData
			{
				Kind = CricketCombatRewardKind.Exp,
				Content = itemData,
				DisplayName = LanguageKey.LK_Exp.Tr(),
				Delta = (isGain ? wager.Count : (-wager.Count)),
				IsGain = isGain,
				ShowDelta = true,
				ShowCount = true
			};
		}

		// Token: 0x06008761 RID: 34657 RVA: 0x003EF5B4 File Offset: 0x003ED7B4
		private CricketCombatRewardData CreateCharacterRewardData(CharacterDisplayData characterDisplayData, bool isGain)
		{
			return new CricketCombatRewardData
			{
				Kind = CricketCombatRewardKind.Character,
				Content = new CricketCombatRewardCharacterContent(characterDisplayData),
				CharacterDisplayData = characterDisplayData,
				DisplayName = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, false),
				Delta = 0,
				IsGain = isGain,
				ShowDelta = false,
				ShowCount = false
			};
		}

		// Token: 0x06008762 RID: 34658 RVA: 0x003EF610 File Offset: 0x003ED810
		private CricketCombatRewardData CreateItemRewardData(ItemDisplayData itemDisplayData, Wager wager, bool isGain)
		{
			return new CricketCombatRewardData
			{
				Kind = CricketCombatRewardKind.Item,
				Content = itemDisplayData,
				DisplayName = ItemTemplateHelper.GetName(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId),
				Delta = (isGain ? wager.Count : (-wager.Count)),
				IsGain = isGain,
				ShowDelta = true,
				ShowCount = true
			};
		}

		// Token: 0x06008763 RID: 34659 RVA: 0x003EF684 File Offset: 0x003ED884
		private void RequestItemRewardData(Wager wager, List<CricketCombatRewardData> rewardDataList, bool isGain)
		{
			int requestVersion = this._rewardRequestVersion;
			int charId = this.GetWagerOwnerCharId(isGain);
			ItemDomainMethod.AsyncCall.GetItemDisplayData(null, wager.ItemKey, charId, delegate(int offset, RawDataPool dataPool)
			{
				bool flag = requestVersion != this._rewardRequestVersion;
				if (!flag)
				{
					ItemDisplayData itemDisplayData = null;
					Serializer.Deserialize(dataPool, offset, ref itemDisplayData);
					rewardDataList.Clear();
					rewardDataList.Add(this.CreateItemRewardData(itemDisplayData, wager, isGain));
					this.RefreshView();
				}
			});
		}

		// Token: 0x06008764 RID: 34660 RVA: 0x003EF6EC File Offset: 0x003ED8EC
		private void RequestCharacterRewardData(Wager wager, List<CricketCombatRewardData> rewardDataList, bool isGain)
		{
			int requestVersion = this._rewardRequestVersion;
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, wager.CharId, delegate(int offset, RawDataPool dataPool)
			{
				bool flag = requestVersion != this._rewardRequestVersion;
				if (!flag)
				{
					CharacterDisplayData characterDisplayData = null;
					Serializer.Deserialize(dataPool, offset, ref characterDisplayData);
					rewardDataList.Clear();
					bool flag2 = characterDisplayData != null;
					if (flag2)
					{
						rewardDataList.Add(this.CreateCharacterRewardData(characterDisplayData, isGain));
					}
					this.RefreshView();
				}
			});
		}

		// Token: 0x06008765 RID: 34661 RVA: 0x003EF73A File Offset: 0x003ED93A
		private int GetWagerOwnerCharId(bool isGain)
		{
			return isGain ? this._taiwuChar.CharacterId : this._enemyChar.CharacterId;
		}

		// Token: 0x06008766 RID: 34662 RVA: 0x003EF758 File Offset: 0x003ED958
		private void RefreshView()
		{
			bool flag = !this._uiReady || !this._hasData;
			if (!flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(284);
				this.RefreshStateArea();
				this.RefreshInfoArea();
				this.RefreshRewardArea();
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x06008767 RID: 34663 RVA: 0x003EF7AC File Offset: 0x003ED9AC
		private void RefreshStateArea()
		{
			this.winImage.SetActive(this._isWin);
			this.loseImage.SetActive(!this._isWin);
			this.resultInfoTitleText.text = LanguageKey.LK_Combat_Result_Title.Tr();
			this.rewardTitleText.text = (this._isWin ? LanguageKey.LK_CricketCombat_Result_SubTitle_Win : LanguageKey.LK_CricketCombat_Result_SubTitle_Lose).Tr();
			this.extraRewardTitleText.text = (this._isWin ? LanguageKey.LK_CricketCombat_Result_ExtraGain : LanguageKey.LK_CricketCombat_Result_ExtraLose).Tr();
		}

		// Token: 0x06008768 RID: 34664 RVA: 0x003EF841 File Offset: 0x003EDA41
		private void RefreshInfoArea()
		{
			this.taiwuResultInfoItem.Set(this._taiwuChar, true, CricketCombatResultMetricType.Happiness, this.TaiwuHappinessDelta);
			this.enemyResultInfoItem.Set(this._enemyChar, false, CricketCombatResultMetricType.Favorability, this.EnemyFavorabilityDelta);
		}

		// Token: 0x06008769 RID: 34665 RVA: 0x003EF878 File Offset: 0x003EDA78
		private void RefreshRewardArea()
		{
			this.RefreshRewardGrid(this.rewardItemRoot, this._rewardDataList);
			bool showExtraRewardArea = this._extraWager.Type != -1;
			this.extraRewardArea.SetActive(showExtraRewardArea);
			bool flag = showExtraRewardArea;
			if (flag)
			{
				this.RefreshRewardGrid(this.extraRewardItemRoot, this._extraRewardDataList);
			}
		}

		// Token: 0x0600876A RID: 34666 RVA: 0x003EF8D0 File Offset: 0x003EDAD0
		private void RefreshRewardGrid(GridLayoutGroup rewardGrid, List<CricketCombatRewardData> rewardDataList)
		{
			CommonUtils.PrepareEnoughChildren(rewardGrid.transform, this.rewardItemTemplate.gameObject, rewardDataList.Count, null);
			for (int i = 0; i < rewardDataList.Count; i++)
			{
				rewardGrid.transform.GetChild(i).GetComponent<CricketCombatResultRewardItem>().Set(rewardDataList[i]);
			}
		}

		// Token: 0x0600876B RID: 34667 RVA: 0x003EF938 File Offset: 0x003EDB38
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "Confirm")
			{
				this.QuickHide();
			}
		}

		// Token: 0x040067DC RID: 26588
		[SerializeField]
		private GameObject winImage;

		// Token: 0x040067DD RID: 26589
		[SerializeField]
		private GameObject loseImage;

		// Token: 0x040067DE RID: 26590
		[SerializeField]
		private TextMeshProUGUI resultInfoTitleText;

		// Token: 0x040067DF RID: 26591
		[SerializeField]
		private TextMeshProUGUI rewardTitleText;

		// Token: 0x040067E0 RID: 26592
		[SerializeField]
		private CricketCombatResultInfoItem taiwuResultInfoItem;

		// Token: 0x040067E1 RID: 26593
		[SerializeField]
		private CricketCombatResultInfoItem enemyResultInfoItem;

		// Token: 0x040067E2 RID: 26594
		[SerializeField]
		private GridLayoutGroup rewardItemRoot;

		// Token: 0x040067E3 RID: 26595
		[SerializeField]
		private GameObject extraRewardArea;

		// Token: 0x040067E4 RID: 26596
		[SerializeField]
		private TextMeshProUGUI extraRewardTitleText;

		// Token: 0x040067E5 RID: 26597
		[SerializeField]
		private GridLayoutGroup extraRewardItemRoot;

		// Token: 0x040067E6 RID: 26598
		[SerializeField]
		private CricketCombatResultRewardItem rewardItemTemplate;

		// Token: 0x040067E7 RID: 26599
		private readonly List<CricketCombatRewardData> _rewardDataList = new List<CricketCombatRewardData>();

		// Token: 0x040067E8 RID: 26600
		private readonly List<CricketCombatRewardData> _extraRewardDataList = new List<CricketCombatRewardData>();

		// Token: 0x040067E9 RID: 26601
		private bool _uiReady;

		// Token: 0x040067EA RID: 26602
		private bool _hasData;

		// Token: 0x040067EB RID: 26603
		private bool _isWin;

		// Token: 0x040067EC RID: 26604
		private Wager _wager = Wager.Invalid;

		// Token: 0x040067ED RID: 26605
		private Wager _extraWager = Wager.Invalid;

		// Token: 0x040067EE RID: 26606
		private CharacterDisplayData _wagerChar;

		// Token: 0x040067EF RID: 26607
		private CharacterDisplayData _taiwuChar;

		// Token: 0x040067F0 RID: 26608
		private CharacterDisplayData _enemyChar;

		// Token: 0x040067F1 RID: 26609
		private int _rewardRequestVersion;
	}
}
