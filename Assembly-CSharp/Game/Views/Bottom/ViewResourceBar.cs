using System;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C59 RID: 3161
	public class ViewResourceBar : UIBase
	{
		// Token: 0x0600A0F0 RID: 41200 RVA: 0x004B2744 File Offset: 0x004B0944
		private void Awake()
		{
			this.menu.onClick.ResetListener(new Action(this.ShowMenu));
			this.fnBtn.onClick.ResetListener(new Action(this.FnBtn));
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Add(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Add(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
			GEvent.Add(UiEvents.OnMusicPlayerPlayStateChange, new GEvent.Callback(this.OnMusicPlayerPlayStateChange));
			GEvent.Add(UiEvents.OnMusicPlayerEnabledStateChange, new GEvent.Callback(this.OnMusicPlayerEnabledStateChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x0600A0F1 RID: 41201 RVA: 0x004B2848 File Offset: 0x004B0A48
		private void Update()
		{
			bool flag = MainInterfaceFunctionCommandKit.QuickEntry.Check(this.Element, false, false, false, true, false) && this.fnBtn.interactable;
			if (flag)
			{
				this.fnBtn.onClick.Invoke();
			}
		}

		// Token: 0x0600A0F2 RID: 41202 RVA: 0x004B2890 File Offset: 0x004B0A90
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Remove(UiEvents.StartPlanOrRemoveBuilding, new GEvent.Callback(this.StartPlanOrRemoveBuilding));
			GEvent.Remove(UiEvents.CancelPlanOrRemoveBuilding, new GEvent.Callback(this.CancelPlanOrRemoveBuilding));
			GEvent.Remove(UiEvents.OnMusicPlayerPlayStateChange, new GEvent.Callback(this.OnMusicPlayerPlayStateChange));
			GEvent.Remove(UiEvents.OnMusicPlayerEnabledStateChange, new GEvent.Callback(this.OnMusicPlayerEnabledStateChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}

		// Token: 0x0600A0F3 RID: 41203 RVA: 0x004B2958 File Offset: 0x004B0B58
		private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
		{
			this.PlayAnim(false);
		}

		// Token: 0x0600A0F4 RID: 41204 RVA: 0x004B2962 File Offset: 0x004B0B62
		private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
		{
			this.PlayAnim(true);
		}

		// Token: 0x0600A0F5 RID: 41205 RVA: 0x004B296C File Offset: 0x004B0B6C
		private void PlayAnim(bool isShow)
		{
			base.transform.DOLocalMoveY((float)(isShow ? 720 : 1440), 0.3f, false);
		}

		// Token: 0x0600A0F6 RID: 41206 RVA: 0x004B2990 File Offset: 0x004B0B90
		private void OnEnable()
		{
			GEvent.Add(UiEvents.UpdateAllBlockInfo, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Add(UiEvents.MapPickupEffectChanged, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x0600A0F7 RID: 41207 RVA: 0x004B2A20 File Offset: 0x004B0C20
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.UpdateAllBlockInfo, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(EEvents.OnAreaSpiritualDebtChange, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.RefreshCall));
			GEvent.Remove(UiEvents.MapPickupEffectChanged, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x0600A0F8 RID: 41208 RVA: 0x004B2AB0 File Offset: 0x004B0CB0
		public override void OnInit(ArgumentBox _)
		{
			this.menuTip.PresetParam[0] = LanguageKey.LK_System_Option_Tip_Title.Tr();
			this.fnTip.PresetParam[0] = LanguageKey.LK_Main_Operation_Tip_Title.Tr();
			this.Refresh();
			this.musicPlayerSmallView.Init();
		}

		// Token: 0x0600A0F9 RID: 41209 RVA: 0x004B2AFF File Offset: 0x004B0CFF
		private void RefreshCall(ArgumentBox _)
		{
			this.Refresh();
		}

		// Token: 0x0600A0FA RID: 41210 RVA: 0x004B2B08 File Offset: 0x004B0D08
		public void Refresh()
		{
			this.menuTip.PresetParam[1] = LanguageKey.LK_System_Option_Tip_Desc.TrFormat(CommonCommandKit.Esc);
			this.fnTip.PresetParam[1] = LanguageKey.LK_Main_Operation_Tip_Desc.TrFormat(MainInterfaceFunctionCommandKit.QuickEntry);
			TaiwuDomainMethod.AsyncCall.RequestTaiwuResourceDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				TaiwuResourceDisplayData data = new TaiwuResourceDisplayData();
				Serializer.Deserialize(pool, offset, ref data);
				for (sbyte type = 0; type < 8; type += 1)
				{
					this.baseResource[(int)type].SetBaseType(type);
				}
				this.exp.SetBaseType(8);
				for (int i = 0; i < 8; i++)
				{
					this.baseResource[i].Set(data.Resources[i], data.ResourcesDelta[i]);
				}
				bool taiwuVillageUnlocked = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
				this.population.gameObject.SetActive(taiwuVillageUnlocked);
				this.debt.gameObject.SetActive(taiwuVillageUnlocked);
				this.population.SetPopulation(data.Villager, data.IdleVillager);
				this.debt.Set(data.Debt, 0);
				this.exp.Set(data.Exp, 0);
				this.populationTip.PresetParam[0] = LanguageKey.LK_MouseTip_Taiwu_Villager_Title.Tr();
				this.populationTip.PresetParam[1] = LanguageKey.LK_MouseTip_Taiwu_Villager_Detail.TrFormat(new object[]
				{
					data.TaiwuPopulationTipsTotal,
					data.TaiwuPopulationTipsAdult,
					data.TaiwuPopulationTipsAdultWorking + data.TaiwuPopulationTipsAdultAssign,
					data.TaiwuPopulationTipsAdultWorking,
					data.TaiwuPopulationTipsAdultAssign,
					data.TaiwuPopulationTipsAdultIdle,
					data.TaiwuPopulationTipsChild,
					data.TaiwuPopulationTipsChildWorking,
					data.TaiwuPopulationTipsChildIdle,
					data.TaiwuPopulationTipsInJail
				});
				this.debtTps.PresetParam[0] = LanguageKey.LK_MouseTip_Taiwu_Debt_Title.Tr();
				MapAreaData[] areas = SingletonObject.getInstance<WorldMapModel>().Areas;
				this.debtTps.PresetParam[1] = LanguageKey.LK_MouseTip_Taiwu_Debt_Detail.TrFormat(areas.CheckIndex(data.AreaId) ? areas[data.AreaId].GetConfig().Name : LanguageKey.LK_Character_Location_Format_Invalid_3.Tr(), data.Debt);
			});
			this.musicPlayerSmallView.RefreshShowState();
			this.musicPlayerSmallView.RefreshPlayState();
		}

		// Token: 0x0600A0FB RID: 41211 RVA: 0x004B2B79 File Offset: 0x004B0D79
		private void ShowMenu()
		{
			UIManager.Instance.MaskUI(UIElement.SystemOption);
		}

		// Token: 0x0600A0FC RID: 41212 RVA: 0x004B2B8B File Offset: 0x004B0D8B
		private void FnBtn()
		{
			UIManager.Instance.MaskUI(UIElement.MainOperation);
		}

		// Token: 0x0600A0FD RID: 41213 RVA: 0x004B2B9D File Offset: 0x004B0D9D
		private void CancelPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(true);
		}

		// Token: 0x0600A0FE RID: 41214 RVA: 0x004B2BAD File Offset: 0x004B0DAD
		private void StartPlanOrRemoveBuilding(ArgumentBox argBox)
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600A0FF RID: 41215 RVA: 0x004B2BBD File Offset: 0x004B0DBD
		private void OnMusicPlayerPlayStateChange(ArgumentBox _)
		{
			this.musicPlayerSmallView.RefreshPlayState();
		}

		// Token: 0x0600A100 RID: 41216 RVA: 0x004B2BCB File Offset: 0x004B0DCB
		private void OnMusicPlayerEnabledStateChange(ArgumentBox _)
		{
			this.musicPlayerSmallView.RefreshShowState();
		}

		// Token: 0x0600A101 RID: 41217 RVA: 0x004B2BD9 File Offset: 0x004B0DD9
		private void OnTopUiChanged(ArgumentBox argBox)
		{
			this.musicPlayerSmallView.RefreshShowState();
			this.musicPlayerSmallView.RefreshPlayState();
		}

		// Token: 0x04007CD1 RID: 31953
		[SerializeField]
		private CButton menu;

		// Token: 0x04007CD2 RID: 31954
		[SerializeField]
		private CButton fnBtn;

		// Token: 0x04007CD3 RID: 31955
		[SerializeField]
		private ResourceItem[] baseResource;

		// Token: 0x04007CD4 RID: 31956
		[SerializeField]
		private ResourceItem exp;

		// Token: 0x04007CD5 RID: 31957
		[SerializeField]
		private ResourceItem population;

		// Token: 0x04007CD6 RID: 31958
		[SerializeField]
		private ResourceItem debt;

		// Token: 0x04007CD7 RID: 31959
		[SerializeField]
		private TooltipInvoker populationTip;

		// Token: 0x04007CD8 RID: 31960
		[SerializeField]
		private TooltipInvoker debtTps;

		// Token: 0x04007CD9 RID: 31961
		[SerializeField]
		private TooltipInvoker fnTip;

		// Token: 0x04007CDA RID: 31962
		[SerializeField]
		private TooltipInvoker menuTip;

		// Token: 0x04007CDB RID: 31963
		[SerializeField]
		private MusicPlayerSmallView musicPlayerSmallView;
	}
}
