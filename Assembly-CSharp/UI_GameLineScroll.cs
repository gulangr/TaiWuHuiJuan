using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

// Token: 0x02000382 RID: 898
public class UI_GameLineScroll : UIBase
{
	// Token: 0x060034E1 RID: 13537 RVA: 0x001A63F0 File Offset: 0x001A45F0
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("index", out this._unlockIndex);
		argsBox.Get("levelIndex", out this._levelIndex);
		Refers curPage = base.CGet<Refers>((this._levelIndex == 0) ? "FirstLevel" : "SecondLevel");
		ScrollHelper.OnInit(curPage, this._unlockIndex, true, -1);
		this.InitAreaStoryScroll(curPage);
		CToggleGroupObsolete toggleGroup = curPage.CGet<CToggleGroupObsolete>("ToggleGroup");
		toggleGroup.InitPreOnToggle(-1);
		toggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete oldTog)
		{
			List<GameObject> refers = curPage.CGetList<GameObject>("ScrollRoot");
			refers[oldTog.Key].SetActive(false);
			refers[newTog.Key].SetActive(true);
		};
	}

	// Token: 0x060034E2 RID: 13538 RVA: 0x001A6494 File Offset: 0x001A4694
	private void InitAreaStoryScroll(Refers curPage)
	{
		GameObject areaStoryScrollHolder = curPage.CGet<GameObject>("AreaStoryScrollHolder");
		sbyte i = 0;
		while ((int)i < areaStoryScrollHolder.transform.childCount)
		{
			Refers areaStoryScrollItem = areaStoryScrollHolder.transform.GetChild((int)i).GetComponent<Refers>();
			Refers areaStoryScroll = areaStoryScrollItem.CGet<Refers>("AreaStorySroll");
			GameObject hover = areaStoryScrollItem.CGet<GameObject>("Hover");
			GameObject disable = areaStoryScrollItem.CGet<GameObject>("Disable");
			PointerTrigger scrollPointerTrigger = areaStoryScrollItem.GetComponent<PointerTrigger>();
			CButtonObsolete button = areaStoryScrollItem.GetComponent<CButtonObsolete>();
			ParticleSystem highLight = areaStoryScroll.CGet<ParticleSystem>("HighLight");
			GameObject highLightHolder;
			areaStoryScroll.CTryGet<GameObject>("HighLightHolder", out highLightHolder);
			sbyte orgTemplateId = i + 1;
			bool isShow = SingletonObject.getInstance<BuildingModel>().GetStateTaskStatus((int)orgTemplateId) != 0;
			button.interactable = isShow;
			disable.SetActive(isShow);
			bool flag = isShow;
			if (flag)
			{
				areaStoryScrollItem.GetComponent<HSVStyleRoot>().SetDefault();
				scrollPointerTrigger.EnterEvent.AddListener(delegate()
				{
					bool flag2 = highLightHolder != null;
					if (flag2)
					{
						highLightHolder.SetActive(true);
					}
					highLight.gameObject.SetActive(true);
					highLight.Simulate(3f);
					highLight.Play();
					hover.SetActive(true);
				});
				scrollPointerTrigger.ExitEvent.AddListener(delegate()
				{
					bool flag2 = highLightHolder != null;
					if (flag2)
					{
						highLightHolder.SetActive(false);
					}
					highLight.gameObject.SetActive(false);
					hover.SetActive(false);
				});
				bool prosper = SingletonObject.getInstance<BuildingModel>().GetStateTaskStatus((int)orgTemplateId) == 1;
				this.RefreshTexture(areaStoryScroll, (int)orgTemplateId, prosper, true);
				button.ClearAndAddListener(delegate
				{
					ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("orgTemplateId", orgTemplateId).Set("prosper", prosper);
					UIElement.AreaStoryScroll.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.AreaStoryScroll, true);
				});
			}
			else
			{
				areaStoryScrollItem.GetComponent<HSVStyleRoot>().SetDefaultBlack();
				this.RefreshTexture(areaStoryScroll, -1, false, false);
				scrollPointerTrigger.EnterEvent.RemoveAllListeners();
			}
			i += 1;
		}
	}

	// Token: 0x060034E3 RID: 13539 RVA: 0x001A6660 File Offset: 0x001A4860
	private void RefreshTexture(Refers areaStoryScroll, int orgTemplateId, bool prosper, bool isShow = true)
	{
		areaStoryScroll.CGet<TextMeshProUGUI>("AreaName").gameObject.SetActive(isShow);
		areaStoryScroll.CGet<CImage>("NameImage").gameObject.SetActive(isShow);
		areaStoryScroll.CGet<CImage>("StatusImage").gameObject.SetActive(isShow);
		bool flag = !isShow;
		if (!flag)
		{
			areaStoryScroll.CGet<TextMeshProUGUI>("AreaName").text = Organization.Instance[orgTemplateId].SectMainStory.Name;
			areaStoryScroll.CGet<CImage>("NameImage").SetSprite(string.Format("ui_scroll_sect_name_{0}", orgTemplateId - 1), false, null);
			areaStoryScroll.CGet<CImage>("StatusImage").SetSprite(prosper ? "ui_scroll_sect_img_feature_0" : "ui_scroll_sect_img_feature_1", false, null);
		}
	}

	// Token: 0x060034E4 RID: 13540 RVA: 0x001A672E File Offset: 0x001A492E
	private void Update()
	{
		ScrollHelper.Update();
	}

	// Token: 0x060034E5 RID: 13541 RVA: 0x001A6738 File Offset: 0x001A4938
	public override void QuickHide()
	{
		bool flag = ScrollHelper.QuickHide();
		if (flag)
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			base.QuickHide();
			TaiwuEventDomainMethod.Call.TriggerListener("GameLineScrollShowed", true);
		}
	}

	// Token: 0x060034E6 RID: 13542 RVA: 0x001A6778 File Offset: 0x001A4978
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Close";
		if (flag)
		{
			this.QuickHide();
		}
		else
		{
			ScrollHelper.OnClick(btn.GetComponent<CButtonObsolete>());
		}
	}

	// Token: 0x060034E7 RID: 13543 RVA: 0x001A67AF File Offset: 0x001A49AF
	public void ShowIllustration()
	{
		ScrollHelper.ShowIllustration(false);
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x001A67B9 File Offset: 0x001A49B9
	public void HideIllustration()
	{
		ScrollHelper.HideIllustration(false);
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x001A67C4 File Offset: 0x001A49C4
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		Refers curPage = base.CGet<Refers>((this._levelIndex == 0) ? "FirstLevel" : "SecondLevel");
		GameObject areaStoryScrollHolder = curPage.CGet<GameObject>("AreaStoryScrollHolder");
		sbyte i = 0;
		while ((int)i < areaStoryScrollHolder.transform.childCount)
		{
			Refers areaStoryScrollItem = areaStoryScrollHolder.transform.GetChild((int)i).GetComponent<Refers>();
			Refers areaStoryScroll = areaStoryScrollItem.CGet<Refers>("AreaStorySroll");
			GameObject nameEnHolder = areaStoryScroll.CGet<GameObject>("AreaNameEnHolder");
			nameEnHolder.SetActive(languageType > LocalStringManager.LanguageType.CN);
			nameEnHolder.transform.GetChild(0).GetComponent<LanguageRuleExpandOnHover>().OnLanguageChange(languageType);
			i += 1;
		}
		ScrollHelper.OnLanguageChange(languageType);
	}

	// Token: 0x04002663 RID: 9827
	private int _unlockIndex;

	// Token: 0x04002664 RID: 9828
	private int _levelIndex;
}
