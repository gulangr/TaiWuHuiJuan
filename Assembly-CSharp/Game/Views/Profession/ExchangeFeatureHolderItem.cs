using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CharacterDataMonitor;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007C5 RID: 1989
	public class ExchangeFeatureHolderItem : MonoBehaviour
	{
		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x06006103 RID: 24835 RVA: 0x002C795C File Offset: 0x002C5B5C
		public InfinityScroll FeatureScroll
		{
			get
			{
				return this.featureScroll;
			}
		}

		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x06006104 RID: 24836 RVA: 0x002C7964 File Offset: 0x002C5B64
		public InfinityScroll FeatureScrollTobeExchange
		{
			get
			{
				return this.featureScrollTobeExchange;
			}
		}

		// Token: 0x17000BD4 RID: 3028
		// (get) Token: 0x06006105 RID: 24837 RVA: 0x002C796C File Offset: 0x002C5B6C
		public List<int> FeaturesLeft
		{
			get
			{
				return this._featuresLeft;
			}
		}

		// Token: 0x17000BD5 RID: 3029
		// (get) Token: 0x06006106 RID: 24838 RVA: 0x002C7974 File Offset: 0x002C5B74
		public List<int> FeaturesToBeExchange
		{
			get
			{
				return this._featuresToBeExchange;
			}
		}

		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x06006107 RID: 24839 RVA: 0x002C797C File Offset: 0x002C5B7C
		private ViewTravelingTaoistMonkSkill2 Parent
		{
			get
			{
				return UIElement.TravelingTaoistMonkSkill2.UiBaseAs<ViewTravelingTaoistMonkSkill2>();
			}
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x002C7988 File Offset: 0x002C5B88
		private void Awake()
		{
			this.featureScroll.OnItemRender += this.OnLeftFeatureRender;
			this.featureScrollTobeExchange.OnItemRender += this.OnTobeExchangeFeatureRender;
			this.replaceBtn.ClearAndAddListener(delegate
			{
				CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
				List<int> charIdList = monitor.GetTaiwuTeamCharIds();
				List<int> res = charIdList.Union(monitor.GetTaiwuSpecialGroup()).ToPoolList<int>();
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this._requestHandler, res, delegate(int offset, RawDataPool pool)
				{
					List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
					Serializer.Deserialize(pool, offset, ref displayData);
					List<ISelectCharacterData> selectList = (from item in displayData
					select new BasicSelectCharacterDataAdapter(item)).ToList<ISelectCharacterData>();
					CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
					config.InteractionMode = ESelectCharacterInteractionMode.Slot;
					config.SelectionMode = ESelectCharacterSelectionMode.Single;
					config.BannedCharacterIds = (from x in displayData
					where AgeGroup.GetAgeGroup(x.PhysiologicalAge) <= 0
					select x.CharacterId).Union(monitor.GetTaiwuSpecialGroup()).ToHashSet<int>();
					config.MinSelectionCount = 0;
					UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", selectList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(delegate(List<int> v)
					{
						bool flag = v.Count > 0;
						if (flag)
						{
							this.Parent.ReplaceCharacter(v[0]);
						}
					})));
					UIManager.Instance.MaskUI(UIElement.SelectChar);
					EasyPool.Free<List<int>>(res);
				});
			});
		}

		// Token: 0x06006109 RID: 24841 RVA: 0x002C79DE File Offset: 0x002C5BDE
		private void OnTobeExchangeFeatureRender(int index, GameObject obj)
		{
			this.SetExchangeFeatureItem(index, obj.GetComponent<ExchangeFeatureItem>(), false);
		}

		// Token: 0x0600610A RID: 24842 RVA: 0x002C79F0 File Offset: 0x002C5BF0
		private void OnLeftFeatureRender(int index, GameObject obj)
		{
			this.SetExchangeFeatureItem(index, obj.GetComponent<ExchangeFeatureItem>(), true);
		}

		// Token: 0x0600610B RID: 24843 RVA: 0x002C7A04 File Offset: 0x002C5C04
		private void SetExchangeFeatureItem(int index, ExchangeFeatureItem exchangeFeatureItem, bool isLeft)
		{
			short featureId = isLeft ? ((short)this._featuresLeft[index]) : ((short)this._featuresToBeExchange[index]);
			ExchangeFeatureHolderItem.FeatureLocation location;
			if (isLeft)
			{
				location = (this._isTaiwu ? ExchangeFeatureHolderItem.FeatureLocation.TaiwuLeft : ExchangeFeatureHolderItem.FeatureLocation.TargetLeft);
			}
			else
			{
				location = (this._isTaiwu ? ExchangeFeatureHolderItem.FeatureLocation.TaiwuTobeExchanged : ExchangeFeatureHolderItem.FeatureLocation.TargetTobeExchanged);
			}
			exchangeFeatureItem.Set(featureId, this._charId, this._isTaiwu, delegate
			{
				Action<int, ExchangeFeatureHolderItem.FeatureLocation> onClickFeature = this.OnClickFeature;
				if (onClickFeature != null)
				{
					onClickFeature((int)featureId, location);
				}
			});
			exchangeFeatureItem.SetPointerEventCallbacks(delegate(int id)
			{
				Action<int, ExchangeFeatureHolderItem.FeatureLocation> onPointerEnterFeature = this.OnPointerEnterFeature;
				if (onPointerEnterFeature != null)
				{
					onPointerEnterFeature(id, location);
				}
			}, delegate(int id)
			{
				Action<int, ExchangeFeatureHolderItem.FeatureLocation> onPointerExitFeature = this.OnPointerExitFeature;
				if (onPointerExitFeature != null)
				{
					onPointerExitFeature(id, location);
				}
			});
		}

		// Token: 0x0600610C RID: 24844 RVA: 0x002C7AB8 File Offset: 0x002C5CB8
		public void Set(IAsyncMethodRequestHandler requestHandler, int charId, int taiwuId, List<int> featuresLeft, List<int> featuresToBeExchange, Dictionary<int, List<int>> featureGroup, Dictionary<int, int> mutexFeatureDic, bool shouldRefreshMutexFeatureDic = false)
		{
			this._requestHandler = requestHandler;
			this._charId = charId;
			this._taiwuId = taiwuId;
			this._isTaiwu = (charId == taiwuId);
			this._featuresLeft = featuresLeft;
			this._featuresToBeExchange = featuresToBeExchange;
			bool flag = charId == -1;
			if (!flag)
			{
				this.RefreshAvatar(charId);
				this.SetupClickCharBtn(charId);
				this.SetupFeatureScrolls();
				this.SetupMonitor(charId, shouldRefreshMutexFeatureDic, featureGroup, mutexFeatureDic);
				this.SetupMouseTipDisplayer(charId);
				this.CheckAvailableTeammates(charId);
			}
		}

		// Token: 0x0600610D RID: 24845 RVA: 0x002C7B34 File Offset: 0x002C5D34
		private void RefreshAvatar(int charId)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this._requestHandler, charId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				this.avatar.Refresh(displayData, true);
				this.charName.text = NameCenter.GetNameByDisplayData(displayData, charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false);
			});
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x002C7B74 File Offset: 0x002C5D74
		private void SetupClickCharBtn(int charId)
		{
			this.clickCharBtn.ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", charId);
				argBox.Set("CanOperate", false);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			});
		}

		// Token: 0x0600610F RID: 24847 RVA: 0x002C7BA7 File Offset: 0x002C5DA7
		private void SetupFeatureScrolls()
		{
			this.featureScroll.SetDataCount(this._featuresLeft.Count);
			this.featureScrollTobeExchange.SetDataCount(this._featuresToBeExchange.Count);
		}

		// Token: 0x06006110 RID: 24848 RVA: 0x002C7BD8 File Offset: 0x002C5DD8
		private void SetupMonitor(int charId, bool shouldRefreshMutexFeatureDic, Dictionary<int, List<int>> featureGroup, Dictionary<int, int> mutexFeatureDic)
		{
			this._monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(charId, false);
			bool init = this._monitor.Init;
			if (init)
			{
				this.RefreshFeature(featureGroup);
				ExchangeFeatureHolderItem.SetMutexFeatureDic(shouldRefreshMutexFeatureDic, featureGroup, mutexFeatureDic);
				this._monitor.AddFeatureListener(delegate()
				{
					this.RefreshFeature(featureGroup);
				});
			}
			else
			{
				this._monitor.AddFeatureListener(delegate()
				{
					this.RefreshFeature(featureGroup);
					ExchangeFeatureHolderItem.SetMutexFeatureDic(shouldRefreshMutexFeatureDic, featureGroup, mutexFeatureDic);
				});
			}
		}

		// Token: 0x06006111 RID: 24849 RVA: 0x002C7C88 File Offset: 0x002C5E88
		private static void SetMutexFeatureDic(bool shouldRefreshMutexFeatureDic, Dictionary<int, List<int>> featureGroup, Dictionary<int, int> mutexFeatureDic)
		{
			if (shouldRefreshMutexFeatureDic)
			{
				mutexFeatureDic.Clear();
				foreach (KeyValuePair<int, List<int>> pair in featureGroup)
				{
					bool flag = pair.Value.Count <= 1;
					if (!flag)
					{
						mutexFeatureDic.TryAdd(pair.Value[0], pair.Value[1]);
						mutexFeatureDic.TryAdd(pair.Value[1], pair.Value[0]);
					}
				}
			}
		}

		// Token: 0x06006112 RID: 24850 RVA: 0x002C7D40 File Offset: 0x002C5F40
		private void RefreshFeature(Dictionary<int, List<int>> featureGroup)
		{
			this._featuresLeft.Clear();
			foreach (short id in this._monitor.FeatureIds)
			{
				CharacterFeatureItem config = CharacterFeature.Instance[id];
				bool canBeExchanged = config.CanBeExchanged;
				if (canBeExchanged)
				{
					bool flag = !this._featuresToBeExchange.Contains((int)id);
					if (flag)
					{
						this._featuresLeft.Add((int)id);
					}
					bool flag2 = !featureGroup.ContainsKey((int)config.MutexGroupId);
					if (flag2)
					{
						featureGroup.Add((int)config.MutexGroupId, new List<int>());
					}
					featureGroup[(int)config.MutexGroupId].Add((int)id);
				}
			}
			this.featureScroll.UpdateData(this._featuresLeft.Count);
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x002C7E30 File Offset: 0x002C6030
		private void SetupMouseTipDisplayer(int charId)
		{
			TooltipInvoker tooltipInvoker = this.mouseTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.mouseTipDisplayer.RuntimeParam.Set("charId", charId);
			this.mouseTipDisplayer.enabled = true;
		}

		// Token: 0x06006114 RID: 24852 RVA: 0x002C7E80 File Offset: 0x002C6080
		private void CheckAvailableTeammates(int currentCharId)
		{
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			List<int> charIdList = monitor.GetTaiwuTeamCharIds();
			IReadOnlyCollection<int> specialGroup = monitor.GetTaiwuSpecialGroup();
			List<int> allCandidates = (from id in charIdList
			where id != currentCharId
			select id).ToList<int>();
			bool flag = allCandidates.Count == 0;
			if (flag)
			{
				this.replaceBtn.interactable = false;
				this.replaceBtnTip.PresetParam[0] = LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2_ChangeChar_NoCharacter.Tr();
			}
			else
			{
				Func<CharacterDisplayDataForGeneralScrollList, bool> <>9__2;
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(this._requestHandler, allCandidates.ToPoolList<int>(), delegate(int offset, RawDataPool pool)
				{
					List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
					Serializer.Deserialize(pool, offset, ref displayData);
					IEnumerable<CharacterDisplayDataForGeneralScrollList> source = displayData;
					Func<CharacterDisplayDataForGeneralScrollList, bool> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ((CharacterDisplayDataForGeneralScrollList x) => AgeGroup.GetAgeGroup(x.PhysiologicalAge) > 0 && !specialGroup.Contains(x.CharacterId)));
					}
					int validCount = source.Count(predicate);
					this.replaceBtn.interactable = (validCount > 0);
					this.replaceBtnTip.PresetParam[0] = ((validCount > 0) ? LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2_ChangeChar_Tip.Tr() : LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2_ChangeChar_NoCharacter.Tr());
				});
				EasyPool.Free<List<int>>(allCandidates);
			}
		}

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06006115 RID: 24853 RVA: 0x002C7F30 File Offset: 0x002C6130
		// (remove) Token: 0x06006116 RID: 24854 RVA: 0x002C7F68 File Offset: 0x002C6168
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, ExchangeFeatureHolderItem.FeatureLocation> OnClickFeature;

		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06006117 RID: 24855 RVA: 0x002C7FA0 File Offset: 0x002C61A0
		// (remove) Token: 0x06006118 RID: 24856 RVA: 0x002C7FD8 File Offset: 0x002C61D8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, ExchangeFeatureHolderItem.FeatureLocation> OnPointerEnterFeature;

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06006119 RID: 24857 RVA: 0x002C8010 File Offset: 0x002C6210
		// (remove) Token: 0x0600611A RID: 24858 RVA: 0x002C8048 File Offset: 0x002C6248
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, ExchangeFeatureHolderItem.FeatureLocation> OnPointerExitFeature;

		// Token: 0x0400434F RID: 17231
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004350 RID: 17232
		[SerializeField]
		private CButton replaceBtn;

		// Token: 0x04004351 RID: 17233
		[SerializeField]
		private CButton clickCharBtn;

		// Token: 0x04004352 RID: 17234
		[SerializeField]
		private InfinityScroll featureScroll;

		// Token: 0x04004353 RID: 17235
		[SerializeField]
		private InfinityScroll featureScrollTobeExchange;

		// Token: 0x04004354 RID: 17236
		[SerializeField]
		private TextMeshProUGUI charName;

		// Token: 0x04004355 RID: 17237
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x04004356 RID: 17238
		[SerializeField]
		private TooltipInvoker replaceBtnTip;

		// Token: 0x04004357 RID: 17239
		private FeatureMonitor _monitor;

		// Token: 0x04004358 RID: 17240
		private List<int> _featuresLeft;

		// Token: 0x04004359 RID: 17241
		private List<int> _featuresToBeExchange;

		// Token: 0x0400435A RID: 17242
		private int _charId;

		// Token: 0x0400435B RID: 17243
		private int _taiwuId;

		// Token: 0x0400435C RID: 17244
		private IAsyncMethodRequestHandler _requestHandler;

		// Token: 0x0400435D RID: 17245
		private bool _isTaiwu;

		// Token: 0x02001D07 RID: 7431
		public enum FeatureLocation
		{
			// Token: 0x0400C4C5 RID: 50373
			TaiwuLeft,
			// Token: 0x0400C4C6 RID: 50374
			TaiwuTobeExchanged,
			// Token: 0x0400C4C7 RID: 50375
			TargetLeft,
			// Token: 0x0400C4C8 RID: 50376
			TargetTobeExchanged
		}
	}
}
