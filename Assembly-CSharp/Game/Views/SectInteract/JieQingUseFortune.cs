using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A5 RID: 2469
	public class JieQingUseFortune : MonoBehaviour
	{
		// Token: 0x060076F6 RID: 30454 RVA: 0x003762A7 File Offset: 0x003744A7
		public void OnInit(ViewJieQingInteract parent, bool initialized, Action<bool> onUseFortune)
		{
			this._parent = parent;
			this._onUseFortune = onUseFortune;
			this.Init();
			this.legacyGroup.Init(0);
			this.RefreshUseButton();
		}

		// Token: 0x060076F7 RID: 30455 RVA: 0x003762D4 File Offset: 0x003744D4
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this.SetupWorldDetailTitle();
				this.useLegacy.ClearAndAddListener(new Action(this.OnCliclUseLegacy));
				this.legacyScroll.OnInit(false, true, new Action(this.OnLegacyDataReady));
				this.legacyGroup.OnActiveIndexChange += this.OnWorldDetailToggleChange;
				this.legacyScroll.SetListByWorldDetailId(this._currentTogKey);
				this._inited = true;
				JieqingLegacyListComponent jieqingLegacyListComponent = this.legacyScroll;
				jieqingLegacyListComponent.OnSelectedLegacyChange = (Action)Delegate.Combine(jieqingLegacyListComponent.OnSelectedLegacyChange, new Action(this.OnLegacySelectedChange));
				this.fuyuTips.Type = TipType.Fuyu;
				TooltipInvoker tooltipInvoker = this.fuyuTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.fuyuTips.RuntimeParam.SetObject("ItemData", new ItemDisplayData
				{
					Key = new ItemKey
					{
						Id = -1,
						ItemType = 12,
						TemplateId = 239
					}
				});
				this.fuyuTips.RuntimeParam.Set("TemplateDataOnly", true);
			}
		}

		// Token: 0x060076F8 RID: 30456 RVA: 0x0037640B File Offset: 0x0037460B
		private void OnLegacySelectedChange()
		{
			this.RefreshUseButton();
		}

		// Token: 0x060076F9 RID: 30457 RVA: 0x00376415 File Offset: 0x00374615
		private void RefreshUseButton()
		{
			this.useLegacy.interactable = (this.legacyScroll.SelectedCount > 0);
		}

		// Token: 0x060076FA RID: 30458 RVA: 0x00376432 File Offset: 0x00374632
		private void OnWorldDetailToggleChange(int newTogIndex, int oldTogIndex)
		{
			this._currentTogKey = newTogIndex;
			this.legacyScroll.SetListByWorldDetailId(this._currentTogKey);
		}

		// Token: 0x060076FB RID: 30459 RVA: 0x00376450 File Offset: 0x00374650
		private void OnCliclUseLegacy()
		{
			bool hasLegacy = this.legacyScroll.ApplySelectAllLegacy();
			this.legacyScroll.ClearSelected();
			Action<bool> onUseFortune = this._onUseFortune;
			if (onUseFortune != null)
			{
				onUseFortune(hasLegacy);
			}
			this.RefreshUseButton();
			JieQingUseEffectEmit jieQingUseEffectEmit = this.effectEmitter;
			if (jieQingUseEffectEmit != null)
			{
				jieQingUseEffectEmit.EmitArrowEffect(this.effectEmitter.transform);
			}
		}

		// Token: 0x060076FC RID: 30460 RVA: 0x003764AD File Offset: 0x003746AD
		private void OnDisable()
		{
			this.legacyScroll.ClearSelected();
		}

		// Token: 0x060076FD RID: 30461 RVA: 0x003764BC File Offset: 0x003746BC
		private void OnLegacyDataReady()
		{
		}

		// Token: 0x060076FE RID: 30462 RVA: 0x003764BF File Offset: 0x003746BF
		public void HandleStarFortune(int starFortunePoints)
		{
			this.legacyScroll.HandleStarFortunePoint(starFortunePoints);
		}

		// Token: 0x060076FF RID: 30463 RVA: 0x003764CF File Offset: 0x003746CF
		public void HandleCharacterData(CharacterDisplayData characterDisplayData)
		{
			this.legacyScroll.InheritCharacterBehaviorType = ((characterDisplayData != null) ? characterDisplayData.BehaviorType : -1);
		}

		// Token: 0x06007700 RID: 30464 RVA: 0x003764E9 File Offset: 0x003746E9
		public void HandleLegacyPointBonusFactor(int legacyPointBonusFactor)
		{
		}

		// Token: 0x06007701 RID: 30465 RVA: 0x003764EC File Offset: 0x003746EC
		public void HandleWorldCreationInfo(WorldCreationInfo worldCreationInfo)
		{
			this._worldCreationInfo = worldCreationInfo;
			this.legacyScroll._worldCreationInfo = worldCreationInfo;
		}

		// Token: 0x06007702 RID: 30466 RVA: 0x00376502 File Offset: 0x00374702
		public void HandleFeatureIds(List<short> taiwuFeatureIds)
		{
			this.legacyScroll.HandleTaiwuFeatureId(taiwuFeatureIds);
		}

		// Token: 0x06007703 RID: 30467 RVA: 0x00376512 File Offset: 0x00374712
		internal void HandleAvailableLegacyList(List<short> availableLegacyList)
		{
			this.legacyScroll.HandleAvailableLegacyList(availableLegacyList);
		}

		// Token: 0x06007704 RID: 30468 RVA: 0x00376524 File Offset: 0x00374724
		private void SetupWorldDetailTitle()
		{
			sbyte i = 0;
			while ((int)i < this.WorldCreationTitles.Length)
			{
				int level = this._worldCreationInfo.GetGroupLevel(i);
				WorldCreationGroupItem groupCfg = WorldCreationGroup.Instance[i];
				Color color = WorldDetailSettingGroup.GetLevelColor(level);
				this.WorldCreationTitles[(int)i].CGet<TextMeshProUGUI>("TitleLabel").text = groupCfg.Name.SetColor(color);
				i += 1;
			}
		}

		// Token: 0x040059C8 RID: 22984
		public Refers[] WorldCreationTitles;

		// Token: 0x040059C9 RID: 22985
		private ViewJieQingInteract _parent;

		// Token: 0x040059CA RID: 22986
		[SerializeField]
		private JieqingLegacyListComponent legacyScroll;

		// Token: 0x040059CB RID: 22987
		[SerializeField]
		private CButton useLegacy;

		// Token: 0x040059CC RID: 22988
		[SerializeField]
		private CToggleGroup legacyGroup;

		// Token: 0x040059CD RID: 22989
		[SerializeField]
		private TooltipInvoker fuyuTips;

		// Token: 0x040059CE RID: 22990
		[SerializeField]
		private JieQingUseEffectEmit effectEmitter;

		// Token: 0x040059CF RID: 22991
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040059D0 RID: 22992
		private bool _inited = false;

		// Token: 0x040059D1 RID: 22993
		private int _currentTogKey = 0;

		// Token: 0x040059D2 RID: 22994
		private WorldCreationInfo _worldCreationInfo;

		// Token: 0x040059D3 RID: 22995
		private Action<bool> _onUseFortune;
	}
}
