using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.Cricket.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AAE RID: 2734
	public class CricketBettingRewardItemView : MonoBehaviour
	{
		// Token: 0x17000EC0 RID: 3776
		// (get) Token: 0x06008642 RID: 34370 RVA: 0x003E7EA4 File Offset: 0x003E60A4
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x06008643 RID: 34371 RVA: 0x003E7EAC File Offset: 0x003E60AC
		public void SetData(int index, CricketWagerData reward, Action<int> onSelect)
		{
			CricketBettingRewardItemView.<>c__DisplayClass16_0 CS$<>8__locals1 = new CricketBettingRewardItemView.<>c__DisplayClass16_0();
			CS$<>8__locals1.<>4__this = this;
			CricketBettingRewardItemView.<>c__DisplayClass16_0 CS$<>8__locals2 = CS$<>8__locals1;
			int version = this.renderVersion + 1;
			this.renderVersion = version;
			CS$<>8__locals2.version = version;
			this.hoverRoot.SetActive(false);
			this.gradeBackImage.SetSprite(string.Format("{0}{1}", "ui_back_cricketcombat_prepare_bet_", Mathf.Max(0, (int)reward.Wager.Grade)), false, null);
			this.rewardCardItem.gameObject.SetActive(false);
			this.rewardName.text = string.Empty;
			switch (reward.Wager.Type)
			{
			case 0:
				this.SetResourceReward(reward);
				break;
			case 1:
				ItemDomainMethod.AsyncCall.GetItemDisplayData(null, reward.Wager.ItemKey, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
				{
					bool flag = CS$<>8__locals1.version != CS$<>8__locals1.<>4__this.renderVersion;
					if (!flag)
					{
						ItemDisplayData itemDisplayData = null;
						Serializer.Deserialize(dataPool, offset, ref itemDisplayData);
						CS$<>8__locals1.<>4__this.SetRewardCardItem(itemDisplayData, true);
						CS$<>8__locals1.<>4__this.rewardName.text = LocalStringManager.Get(string.Format("LK_ItemType_{0}", (int)itemDisplayData.Key.ItemType));
					}
				});
				break;
			case 3:
				this.SetRewardCardItem(CricketBettingRewardItemView.CreateExpContent(reward), false);
				this.rewardName.text = LanguageKey.LK_Exp.Tr();
				break;
			}
			this.RefreshCrickets(reward);
		}

		// Token: 0x06008644 RID: 34372 RVA: 0x003E7FC9 File Offset: 0x003E61C9
		private void SetResourceReward(CricketWagerData reward)
		{
			this.SetRewardCardItem(CricketBettingRewardItemView.CreateResourceContent(reward), false);
			this.rewardName.text = ResourceType.Instance[reward.Wager.WagerResourceType].Name;
		}

		// Token: 0x06008645 RID: 34373 RVA: 0x003E8000 File Offset: 0x003E6200
		private void SetRewardCardItem(ITradeableContent content, bool showTip)
		{
			this.rewardRowItemMain.SetData(content);
			this.rewardCardItem.Set(this.rewardRowItemMain, showTip);
			this.rewardCardItem.gameObject.SetActive(true);
		}

		// Token: 0x06008646 RID: 34374 RVA: 0x003E8038 File Offset: 0x003E6238
		private static ItemDisplayData CreateResourceContent(CricketWagerData reward)
		{
			return ItemDisplayData.CreateResource(reward.Wager.WagerResourceType, reward.Wager.Count, -1);
		}

		// Token: 0x06008647 RID: 34375 RVA: 0x003E8068 File Offset: 0x003E6268
		private static ItemDisplayData CreateExpContent(CricketWagerData reward)
		{
			return new ItemDisplayData(12, 8)
			{
				Amount = reward.Wager.Count
			};
		}

		// Token: 0x06008648 RID: 34376 RVA: 0x003E8094 File Offset: 0x003E6294
		private void RefreshCrickets(CricketWagerData reward)
		{
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			for (int i = 0; i < this.cricketList.Length; i++)
			{
				CardItem cricketObj = this.cricketList[i];
				bool visible = reward.IsShowCricket(i) && !CricketFairCombatHelper.IsEnabled;
				this.cricketRowItems[i].SetData(reward.Crickets[i]);
				cricketObj.Set(this.cricketRowItems[i], true);
				this.cricketRowItems[i].ItemBack.SetCountVisible(false);
				CricketBettingRewardItemView.CleanupRedundantCricketView(cricketObj);
				this.cricketNames[i].text = (visible ? reward.Crickets[i].GetName(true) : LanguageKey.LK_CricketCombat_UnknownName.Tr());
				cricketObj.CricketView.skeletonGraphic.enabled = visible;
				cricketObj.CricketView.GetComponent<CEmptyGraphic>().enabled = visible;
				cricketObj.CricketView.SetSingImagePositionToZero();
				TooltipInvoker cricketTipDisplayer = cricketObj.CricketView.GetComponent<TooltipInvoker>();
				cricketTipDisplayer.enabled = visible;
				bool flag = visible;
				if (flag)
				{
					TooltipInvoker tooltipInvoker = cricketTipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					cricketTipDisplayer.RuntimeParam.SetObject("ItemData", reward.Crickets[i]);
					cricketTipDisplayer.Refresh(false, -1);
				}
				cricketObj.GetComponent<TooltipInvoker>().enabled = visible;
				PassClickToParent passClickToParent = cricketObj.CricketView.GetComponent<PassClickToParent>();
				bool flag2 = passClickToParent == null;
				if (flag2)
				{
					passClickToParent = cricketObj.CricketView.gameObject.AddComponent<PassClickToParent>();
				}
				passClickToParent.parentTarget = base.gameObject;
				cricketObj.gameObject.SetActive(true);
				CImage boxImage = this.cricketBoxImages[i];
				boxImage.sprite = (visible ? this.cricketBoxVisibleSprite : this.cricketBoxCoveredSprite);
				boxImage.SetEnabled(boxImage.sprite != null);
			}
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.hoverRoot.SetActive(true);
				for (int j = 0; j < this.cricketList.Length; j++)
				{
					CardItem cricketObj2 = this.cricketList[j];
					bool flag3 = cricketObj2.CricketView == null;
					if (!flag3)
					{
						cricketObj2.CricketView.Sing(true, true, true, -1f, null, 0.5f);
					}
				}
			});
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.hoverRoot.SetActive(false);
				for (int j = 0; j < this.cricketList.Length; j++)
				{
					CardItem cricketObj2 = this.cricketList[j];
					bool flag3 = cricketObj2.CricketView == null;
					if (!flag3)
					{
						cricketObj2.CricketView.StopSing(0.2f);
					}
				}
			});
		}

		// Token: 0x06008649 RID: 34377 RVA: 0x003E82CC File Offset: 0x003E64CC
		private static void CleanupRedundantCricketView(CardItem cricketObj)
		{
			bool flag = cricketObj.CricketView == null;
			if (!flag)
			{
				Transform holder = cricketObj.CricketView.transform.parent;
				bool flag2 = holder == null;
				if (!flag2)
				{
					for (int ci = holder.childCount - 1; ci >= 0; ci--)
					{
						Transform child = holder.GetChild(ci);
						bool flag3 = child != cricketObj.CricketView.transform;
						if (flag3)
						{
							Object.DestroyImmediate(child.gameObject);
						}
					}
				}
			}
		}

		// Token: 0x04006714 RID: 26388
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04006715 RID: 26389
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04006716 RID: 26390
		[SerializeField]
		private GameObject hoverRoot;

		// Token: 0x04006717 RID: 26391
		[SerializeField]
		private CImage gradeBackImage;

		// Token: 0x04006718 RID: 26392
		[SerializeField]
		private CardItem rewardCardItem;

		// Token: 0x04006719 RID: 26393
		[SerializeField]
		private RowItemMain rewardRowItemMain;

		// Token: 0x0400671A RID: 26394
		[SerializeField]
		private TextMeshProUGUI rewardName;

		// Token: 0x0400671B RID: 26395
		[SerializeField]
		private CardItem[] cricketList;

		// Token: 0x0400671C RID: 26396
		[SerializeField]
		private RowItemMain[] cricketRowItems;

		// Token: 0x0400671D RID: 26397
		[SerializeField]
		private TextMeshProUGUI[] cricketNames;

		// Token: 0x0400671E RID: 26398
		[SerializeField]
		private CImage[] cricketBoxImages;

		// Token: 0x0400671F RID: 26399
		[SerializeField]
		private Sprite cricketBoxVisibleSprite;

		// Token: 0x04006720 RID: 26400
		[SerializeField]
		private Sprite cricketBoxCoveredSprite;

		// Token: 0x04006721 RID: 26401
		private int renderVersion;
	}
}
