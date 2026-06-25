using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001AF RID: 431
public class UI_BuildingJiaoPool : UIBase
{
	// Token: 0x0600185B RID: 6235 RVA: 0x00095728 File Offset: 0x00093928
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("landFormType", out this._landFormType);
		GameObject terrain = this._terrainList[(int)this._landFormType];
		Random.InitState((int)SingletonObject.getInstance<BasicGameData>().WorldId);
		for (int i = 0; i < 9; i++)
		{
			bool flag = this._poolStyle.ContainsKey(i);
			if (!flag)
			{
				int style = Random.Range(1, 4);
				this._poolStyle.Add(i, style);
			}
		}
		this._root = base.CGet<RectTransform>("Root").GetComponent<Refers>();
		this._baseList = this._root.CGet<RectTransform>("BaseRoot").GetComponentsInChildren<Refers>().ToList<Refers>();
		this._sensitiveWordTipCanvasGroup = base.CGet<CanvasGroup>("SensitiveWarningTip");
		this._sensitiveWordTipCanvasGroup.alpha = 0f;
		this.InitReturnAndChangeLoong();
		Object.Instantiate<GameObject>(terrain, base.CGet<RectTransform>("BgRoot"));
		this.InitRoad((int)this._landFormType);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RefreshFromServer));
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x0009584C File Offset: 0x00093A4C
	private void RefreshFromServer()
	{
		ExtraDomainMethod.AsyncCall.GetJiaoPoolList(this, delegate(int jiaoPoolOffset, RawDataPool jiaoPoolDataPool)
		{
			this._allJiaoPoolData = null;
			Serializer.Deserialize(jiaoPoolDataPool, jiaoPoolOffset, ref this._allJiaoPoolData);
			ExtraDomainMethod.AsyncCall.GetCurrMaxJiaoPoolCount(this, delegate(int maxCountOffset, RawDataPool maxCountDataPool)
			{
				Serializer.Deserialize(maxCountDataPool, maxCountOffset, ref this._currentOpenJiaoPooCount);
				ExtraDomainMethod.AsyncCall.GetFiveLoongDictCount(this, delegate(int isOpenHostingOffset, RawDataPool isOpenHostingDataPool)
				{
					int value = 0;
					Serializer.Deserialize(isOpenHostingDataPool, isOpenHostingOffset, ref value);
					this._isOpenHosting = (value > 0);
					for (int i = 0; i < 9; i++)
					{
						this.ResetUISate(i);
						bool flag = i < this._currentOpenJiaoPooCount;
						if (flag)
						{
							this.JudgmentJiaoPoolState(this._allJiaoPoolData[i], this._baseList[i], this._baseUIList[i], (int)this._landFormType, i);
							this.OpenRoad(i, (int)this._landFormType);
						}
						else
						{
							this.InitUnlockedOpenSpace(this._baseList[i], this._baseUIList[i], (int)this._landFormType);
						}
					}
					this.RefreshChangeLoongButton();
					this.Element.ShowAfterRefresh();
				});
			});
		});
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x00095864 File Offset: 0x00093A64
	private void OnVowLevelChanged(ArgumentBox arg)
	{
		bool isActiveAndEnabled = base.isActiveAndEnabled;
		if (isActiveAndEnabled)
		{
			this.RefreshFromServer();
		}
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x00095888 File Offset: 0x00093A88
	private void OnEnable()
	{
		AudioManager.Instance.PlayAmbience("ui_building_jiaochi_wave_loop", 1f, 100);
		UIElement.JiaoPoolRecord.Show();
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		GEvent.Add(UiEvents.BuildingVowLevelChanged, new GEvent.Callback(this.OnVowLevelChanged));
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x000958EC File Offset: 0x00093AEC
	public override void QuickHide()
	{
		bool flag = !this._isChangeName;
		if (flag)
		{
			base.QuickHide();
		}
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x00095910 File Offset: 0x00093B10
	private void OnDisable()
	{
		SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		GEvent.Remove(UiEvents.BuildingVowLevelChanged, new GEvent.Callback(this.OnVowLevelChanged));
		UIElement.JiaoPoolRecord.Hide(false);
		foreach (Refers item in this._baseList)
		{
			for (int i = 0; i < item.CGet<RectTransform>("Base").childCount; i++)
			{
				item.CGet<RectTransform>("Base").GetChild(i).gameObject.SetActive(false);
			}
			for (int j = 0; j < item.CGet<RectTransform>("Water").childCount; j++)
			{
				item.CGet<RectTransform>("Water").GetChild(j).gameObject.SetActive(false);
			}
			for (int k = 0; k < item.CGet<RectTransform>("Shadow").childCount; k++)
			{
				item.CGet<RectTransform>("Shadow").GetChild(k).gameObject.SetActive(false);
			}
			for (int l = 0; l < item.CGet<RectTransform>("Occlusion").childCount; l++)
			{
				item.CGet<RectTransform>("Occlusion").GetChild(l).gameObject.SetActive(false);
			}
			for (int m = 0; m < item.CGet<RectTransform>("Occlusion").childCount; m++)
			{
				item.CGet<RectTransform>("Occlusion").GetChild(m).gameObject.SetActive(false);
			}
			for (int n = 0; n < item.CGet<RectTransform>("Effect").childCount; n++)
			{
				item.CGet<RectTransform>("Effect").GetChild(n).gameObject.SetActive(false);
			}
			SkeletonGraphic playUnderWaterAnima = item.CGet<SkeletonGraphic>("PlayUnderWaterAnima");
			playUnderWaterAnima.gameObject.SetActive(false);
			SkeletonGraphic playWaterAnima = item.CGet<SkeletonGraphic>("PlayWaterAnima");
			playWaterAnima.gameObject.SetActive(false);
			AudioManager.Instance.StopSound("ui_building_jiaochi_wave_loop");
		}
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x00095B9C File Offset: 0x00093D9C
	private void CloseOldPool(Refers item)
	{
		for (int i = 0; i < item.CGet<RectTransform>("Base").childCount; i++)
		{
			item.CGet<RectTransform>("Base").GetChild(i).gameObject.SetActive(false);
		}
		for (int j = 0; j < item.CGet<RectTransform>("Water").childCount; j++)
		{
			item.CGet<RectTransform>("Water").GetChild(j).gameObject.SetActive(false);
		}
		for (int k = 0; k < item.CGet<RectTransform>("Shadow").childCount; k++)
		{
			item.CGet<RectTransform>("Shadow").GetChild(k).gameObject.SetActive(false);
		}
		for (int l = 0; l < item.CGet<RectTransform>("Occlusion").childCount; l++)
		{
			item.CGet<RectTransform>("Occlusion").GetChild(l).gameObject.SetActive(false);
		}
		for (int m = 0; m < item.CGet<RectTransform>("Occlusion").childCount; m++)
		{
			item.CGet<RectTransform>("Occlusion").GetChild(m).gameObject.SetActive(false);
		}
		for (int n = 0; n < item.CGet<RectTransform>("Effect").childCount; n++)
		{
			item.CGet<RectTransform>("Effect").GetChild(n).gameObject.SetActive(false);
		}
		SkeletonGraphic playUnderWaterAnima = item.CGet<SkeletonGraphic>("PlayUnderWaterAnima");
		playUnderWaterAnima.gameObject.SetActive(false);
		SkeletonGraphic playWaterAnima = item.CGet<SkeletonGraphic>("PlayWaterAnima");
		playWaterAnima.gameObject.SetActive(false);
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x00095D73 File Offset: 0x00093F73
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 98, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 92, ulong.MaxValue, null));
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x00095DA8 File Offset: 0x00093FA8
	private void InitHaveWaterPoolStyle(Refers baseRefers, int landFormType, int style)
	{
		this.CloseOldPool(baseRefers);
		baseRefers.CGet<RectTransform>("Base").Find(string.Format("buildingarea_jiaochi_base_{0}_{1}", landFormType, style)).gameObject.SetActive(true);
		baseRefers.CGet<RectTransform>("Water").Find(string.Format("buildingarea_jiaochi_water_0_{0}", style)).gameObject.SetActive(true);
		baseRefers.CGet<RectTransform>("Shadow").Find(string.Format("buildingarea_jiaochi_shadow_0_{0}", style)).gameObject.SetActive(true);
		baseRefers.CGet<RectTransform>("Occlusion").Find(string.Format("buildingarea_jiaochi_top_{0}_{1}", landFormType, style)).gameObject.SetActive(true);
		baseRefers.CGet<RectTransform>("Effect").Find(string.Format("eff_building_jiaopool_jiaochi{0}", style)).gameObject.SetActive(true);
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x00095EA8 File Offset: 0x000940A8
	private void JudgmentJiaoPoolState(JiaoPool poolData, Refers baseRefers, Refers baseUiRefers, int landFormType, int index)
	{
		baseUiRefers.GetComponent<RectTransform>().position = baseRefers.GetComponent<RectTransform>().position;
		bool flag = poolData.Jiaos.Count > 0;
		if (flag)
		{
			ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(this, index, delegate(int offset, RawDataPool dataPool)
			{
				List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
				Serializer.Deserialize(dataPool, offset, ref jiaoList);
				switch (this.GetPoolState(poolData, jiaoList[0]))
				{
				case UI_BuildingJiaoPool.PoolState.Egg:
					this.InitHaveLuan(baseRefers, baseUiRefers, poolData, jiaoList[0], landFormType, index);
					break;
				case UI_BuildingJiaoPool.PoolState.Minor:
					this.InitHaveUnderageJiao(baseRefers, baseUiRefers, poolData, jiaoList[0], landFormType, index);
					break;
				case UI_BuildingJiaoPool.PoolState.Breeding:
					this.InitBreedingJiaoPool(baseRefers, baseUiRefers, poolData, jiaoList, landFormType, index);
					break;
				case UI_BuildingJiaoPool.PoolState.Adult:
					this.InitHaveAdultJiao(baseRefers, baseUiRefers, poolData, jiaoList[0], landFormType, index);
					break;
				case UI_BuildingJiaoPool.PoolState.BreedingEnd:
					this.InitBreedingEndJiaoPool(baseRefers, baseUiRefers, poolData, jiaoList, landFormType, index);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			});
		}
		else
		{
			this.InitOpenPool(baseRefers, baseUiRefers, landFormType, index);
		}
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x00095F60 File Offset: 0x00094160
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				bool flag = notification.Uid.DomainId == 19 && notification.Uid.DataId == 98;
				if (flag)
				{
					IntPair intPair = default(IntPair);
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref intPair);
					bool flag2 = intPair.First != -1;
					if (flag2)
					{
						this.RenderPoolState(intPair.First, intPair.Second);
					}
				}
				else
				{
					bool flag3 = notification.Uid.DomainId == 19 && notification.Uid.DataId == 92;
					if (flag3)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._allJiaoPoolData);
						this.RefreshPoolProgressBars();
					}
				}
			}
		}
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x00096088 File Offset: 0x00094288
	private void InitUnlockedOpenSpace(Refers baseRefers, Refers baseUiRefers, int landFormType)
	{
		baseRefers.CGet<RectTransform>("Base").Find(string.Format("buildingarea_jiaochi_base_{0}_0", landFormType)).gameObject.SetActive(true);
		baseUiRefers.transform.position = baseRefers.transform.position;
		baseUiRefers.gameObject.SetActive(false);
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000960E8 File Offset: 0x000942E8
	private void InitOpenPool(Refers baseRefers, Refers baseUiRefers, int landFormType, int index)
	{
		this.InitHaveWaterPoolStyle(baseRefers, landFormType, this._poolStyle[index]);
		baseUiRefers.gameObject.SetActive(true);
		TooltipInvoker tip = baseUiRefers.CGet<CButtonObsolete>("Add_Btn").GetComponent<TooltipInvoker>();
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		bool flag = this.IsCurrentLocatedInIndustryBlock();
		if (flag)
		{
			baseUiRefers.CGet<CButtonObsolete>("Add_Btn").interactable = true;
			baseUiRefers.CGet<CButtonObsolete>("Add_Btn").transform.Find("Icon").GetComponent<CImage>().SetSprite("sp_icon_datianjia", false, null);
			box.Set("Title", LocalStringManager.Get(LanguageKey.LK_JiaoPool_InvestJiaoPool));
			box.Set("Desc", LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_Desc));
			box.Set("SubTitle", LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionTitle));
			List<MouseTipDynamicCondition.ConditionData> list = new List<MouseTipDynamicCondition.ConditionData>
			{
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionJiaoLuan)
				},
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionJiaoFoster)
				},
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionJiaoAdult)
				}
			};
			box.SetObject("Conditions", list);
		}
		else
		{
			baseUiRefers.CGet<CButtonObsolete>("Add_Btn").interactable = false;
			baseUiRefers.CGet<CButtonObsolete>("Add_Btn").transform.Find("Icon").GetComponent<CImage>().SetSprite("sp_icon_datianjia_gary", false, null);
			box.Set("Title", LocalStringManager.Get(LanguageKey.LK_JiaoPool_InvestJiaoPool));
			box.Set("Desc", LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_Desc));
			box.Set("SubTitle", LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionTitle));
			List<MouseTipDynamicCondition.ConditionData> list2 = new List<MouseTipDynamicCondition.ConditionData>
			{
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionJiaoLuan)
				},
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionJiaoFoster)
				},
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AddBtnTips_ConditionJiaoAdult)
				},
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = "",
					IsSpecial = true
				},
				new MouseTipDynamicCondition.ConditionData
				{
					Icon = "",
					Name = Regex.Replace(LocalStringManager.Get(LanguageKey.LK_JiaoPoolUnavailableFunction), "[\\r\\n]", ""),
					IsSpecial = true
				}
			};
			box.SetObject("Conditions", list2);
		}
		tip.RuntimeParam = box;
		baseUiRefers.CGet<CButtonObsolete>("Add_Btn").gameObject.SetActive(true);
		baseUiRefers.CGet<CButtonObsolete>("Add_Btn").ClearAndAddListener(delegate
		{
			this.PoolInvestEvent(null, index);
		});
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x0009644F File Offset: 0x0009464F
	private void InitHaveLuan(Refers baseRefers, Refers baseUiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int landFormType, int index)
	{
		this.InitHaveWaterPoolStyle(baseRefers, landFormType, this._poolStyle[index]);
		this.PlayJiaoEggPoolIdleAnima(baseRefers);
		this.InitHaveLuanPoolUI(baseUiRefers, poolData, data, index);
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x00096480 File Offset: 0x00094680
	private void InitHaveLuanPoolUI(Refers baseUiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int index)
	{
		baseUiRefers.CGet<CButtonObsolete>("Add_Btn").gameObject.SetActive(false);
		Refers informationBar = baseUiRefers.CGet<Refers>("InformationBar");
		this.InitInformationBar(informationBar, poolData, data, index, 1, true);
		this.InitInteractiveBtn(baseUiRefers.CGet<CButtonObsolete>("InteractiveBtn"), index, data);
		this.InitStopFeedingBtn(baseUiRefers.CGet<CButtonObsolete>("StopFeeding_Btn"), index, data);
		bool flag = !this._isOpenHosting;
		if (!flag)
		{
			this.InitNurtureHosting(baseUiRefers, poolData, index);
		}
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x00096504 File Offset: 0x00094704
	private void InitHaveUnderageJiao(Refers baseRefers, Refers uiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int landFormType, int index)
	{
		this.InitHaveWaterPoolStyle(baseRefers, landFormType, this._poolStyle[index]);
		this.PlayOneJiaoAnima(baseRefers);
		this.GenerateJiaoAttributeEffect(baseRefers, data, this._poolStyle[index]);
		this.InitHaveUnderageJiaoPoolUI(uiRefers, poolData, data, index);
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x00096558 File Offset: 0x00094758
	private void InitHaveUnderageJiaoPoolUI(Refers baseUiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int index)
	{
		Refers informationBar = baseUiRefers.CGet<Refers>("InformationBar");
		Refers feedingConsumption = baseUiRefers.CGet<Refers>("FeedingConsumption");
		Refers demandAnima = baseUiRefers.CGet<Refers>("DemandAnima");
		this.InitInformationBar(informationBar, poolData, data, index, 1, true);
		this.InitFeedingConsumption(feedingConsumption, data, false);
		this.InitInteractiveBtn(baseUiRefers.CGet<CButtonObsolete>("InteractiveBtn"), index, data);
		this.InitStopFeedingBtn(baseUiRefers.CGet<CButtonObsolete>("StopFeeding_Btn"), index, data);
		this.InitAppeaseButton(baseUiRefers, data, index);
		bool flag = JiaoNurturance.Instance.GetItem(data.NurturanceTemplateId).NurturanceAnimation.IsNullOrEmpty();
		if (flag)
		{
			bool flag2 = !this._isOpenHosting;
			if (!flag2)
			{
				this.InitNurtureHosting(baseUiRefers, poolData, index);
			}
		}
		else
		{
			this.InitNurturanceTips(demandAnima, data);
			this.PlayNurtureTypeAnima(demandAnima, base.CGet<SkeletonDataAsset>(JiaoNurturance.Instance.GetItem(data.NurturanceTemplateId).NurturanceAnimation));
			bool flag3 = !this._isOpenHosting;
			if (!flag3)
			{
				this.InitNurtureHosting(baseUiRefers.CGet<Refers>("DemandAnima"), poolData, index);
			}
		}
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x00096668 File Offset: 0x00094868
	private void InitHaveAdultJiao(Refers baseRefers, Refers uiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int landFormType, int index)
	{
		this.InitHaveWaterPoolStyle(baseRefers, landFormType, this._poolStyle[index]);
		this.PlayOneJiaoAnima(baseRefers);
		this.GenerateJiaoAttributeEffect(baseRefers, data, this._poolStyle[index]);
		this.InitHaveAdultJiaoPoolUI(uiRefers, poolData, data, index);
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x000966BC File Offset: 0x000948BC
	private void InitHaveAdultJiaoPoolUI(Refers baseUiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int index)
	{
		Refers informationBar = baseUiRefers.CGet<Refers>("InformationBar");
		baseUiRefers.CGet<CButtonObsolete>("Breeding_Btn").gameObject.SetActive(true);
		CButtonObsolete breedingBtn = baseUiRefers.CGet<CButtonObsolete>("Breeding_Btn");
		this.InitInformationBar(informationBar, poolData, data, index, 1, true);
		this.InitBreedingBtn(breedingBtn, data, index, false);
		this.InitStopFeedingBtn(baseUiRefers.CGet<CButtonObsolete>("StopFeeding_Btn"), index, data);
		this.InitInteractiveBtn(baseUiRefers.CGet<CButtonObsolete>("InteractiveBtn"), index, data);
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x00096740 File Offset: 0x00094940
	private void InitBreedingJiaoPool(Refers baseRefers, Refers uiRefers, JiaoPool poolData, List<GameData.DLC.FiveLoong.Jiao> dataList, int landFormType, int index)
	{
		this.InitHaveWaterPoolStyle(baseRefers, landFormType, this._poolStyle[index]);
		this.PlayTwoJiaoAnima(baseRefers);
		this.GenerateBreedingAttributeEffect(baseRefers, dataList, this._poolStyle[index]);
		this.InitBreedingJiaoPoolUI(uiRefers, poolData, dataList[0], index);
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x00096798 File Offset: 0x00094998
	private void InitBreedingJiaoPoolUI(Refers baseUiRefers, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int index)
	{
		Refers informationBar = baseUiRefers.CGet<Refers>("InformationBar");
		baseUiRefers.CGet<CButtonObsolete>("Breeding_Btn").gameObject.SetActive(true);
		baseUiRefers.CGet<CButtonObsolete>("StopFeeding_Btn").gameObject.SetActive(false);
		CButtonObsolete breedingBtn = baseUiRefers.CGet<CButtonObsolete>("Breeding_Btn");
		this.InitInformationBar(informationBar, poolData, data, index, 2, false);
		this.InitBreedingBtn(breedingBtn, data, index, true);
		this.InitInteractiveBtn(baseUiRefers.CGet<CButtonObsolete>("InteractiveBtn"), index, null);
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x0009681C File Offset: 0x00094A1C
	private void InitBreedingEndJiaoPool(Refers baseRefers, Refers uiRefers, JiaoPool poolData, List<GameData.DLC.FiveLoong.Jiao> dataList, int landFormType, int index)
	{
		this.InitHaveWaterPoolStyle(baseRefers, landFormType, this._poolStyle[index]);
		this.PlayTwoJiaoAnima(baseRefers);
		this.GenerateBreedingAttributeEffect(baseRefers, dataList, this._poolStyle[index]);
		this.InitBreedingEndJiaoPoolUI(uiRefers, poolData, dataList, index);
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x00096870 File Offset: 0x00094A70
	private void InitBreedingEndJiaoPoolUI(Refers baseUiRefers, JiaoPool poolData, List<GameData.DLC.FiveLoong.Jiao> dataList, int index)
	{
		Refers informationBar = baseUiRefers.CGet<Refers>("InformationBar");
		this.InitInformationBar(informationBar, poolData, dataList[0], index, 3, false);
		CButtonObsolete breedingEnd = baseUiRefers.CGet<Refers>("BreedingEnd").CGet<CButtonObsolete>("BreedingEnd_Btn");
		baseUiRefers.CGet<Refers>("BreedingEnd").gameObject.SetActive(true);
		bool flag = !this.IsCurrentLocatedInIndustryBlock();
		if (flag)
		{
			breedingEnd.interactable = false;
			breedingEnd.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_dragoneggs_gray", false, null);
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			TooltipInvoker tips = (!breedingEnd.gameObject.GetComponent<TooltipInvoker>()) ? breedingEnd.gameObject.AddComponent<TooltipInvoker>() : breedingEnd.gameObject.GetComponent<TooltipInvoker>();
			tips.Type = TipType.SingleDesc;
			box.Set("arg0", Regex.Replace(LocalStringManager.Get(LanguageKey.LK_JiaoPoolUnavailableFunction), "[\\r\\n]", ""));
			tips.enabled = true;
			tips.RuntimeParam = box;
		}
		else
		{
			bool flag2 = breedingEnd.gameObject.GetComponent<TooltipInvoker>();
			if (flag2)
			{
				breedingEnd.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_dragoneggs", false, null);
				breedingEnd.gameObject.GetComponent<TooltipInvoker>().enabled = false;
				breedingEnd.interactable = true;
			}
		}
		breedingEnd.ClearAndAddListener(delegate
		{
			this.TakeOutJiaoEvent(index);
			this.InitOpenPool(this._baseList[index], baseUiRefers, (int)this._landFormType, index);
		});
		baseUiRefers.CGet<CButtonObsolete>("InteractiveBtn").ClearAndAddListener(delegate
		{
			this.TakeOutJiaoEvent(index);
		});
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x00096A3C File Offset: 0x00094C3C
	private void InitInformationBar(Refers informationBar, JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data, int index, int jiaoCount, bool isShowChangeNameBtn)
	{
		informationBar.gameObject.SetActive(true);
		informationBar.GetComponent<Image>().raycastTarget = false;
		int remainingMonth = data.EvolveRemainingMonth;
		short maxMonth = JiaoNurturance.Instance[data.NurturanceTemplateId].NurturanceCostMonth;
		bool flag = jiaoCount > 1;
		if (flag)
		{
			string progressName = LocalStringManager.Get("LK_JiaoPool_BreedingGrow");
			int breedingTime = GlobalConfig.Instance.JiaoBreedingTime;
			float time = (float)(breedingTime - poolData.NextPeriod);
			informationBar.CGet<TextMeshProUGUI>("ProgressName").text = progressName;
			informationBar.CGet<TextMeshProUGUI>("ProgressValue").text = string.Format("{0}/{1}", time, breedingTime);
			informationBar.CGet<CImage>("Bar").fillAmount = time / (float)breedingTime;
		}
		else
		{
			bool flag2 = data.GrowthStage == 0;
			if (flag2)
			{
				int eggIncubationTime = GlobalConfig.Instance.JiaoBreedingTime;
				float time2 = (float)(eggIncubationTime - poolData.NextPeriod);
				string progressName2 = LocalStringManager.Get("LK_JiaoPool_IncubateEgg");
				informationBar.CGet<TextMeshProUGUI>("ProgressName").text = progressName2;
				informationBar.CGet<TextMeshProUGUI>("ProgressValue").text = string.Format("{0}/{1}", time2, eggIncubationTime);
				informationBar.CGet<CImage>("Bar").fillAmount = time2 / (float)eggIncubationTime;
			}
			else
			{
				bool flag3 = data.GrowthStage == 1;
				if (flag3)
				{
					float time3 = (float)((int)maxMonth - remainingMonth);
					string progressName3 = LocalStringManager.Get("LK_JiaoPool_Growing");
					informationBar.CGet<TextMeshProUGUI>("ProgressName").text = progressName3;
					informationBar.CGet<TextMeshProUGUI>("ProgressValue").text = string.Format("{0}/{1}", time3, maxMonth);
					informationBar.CGet<CImage>("Bar").fillAmount = time3 / (float)maxMonth;
				}
				else
				{
					bool flag4 = data.GrowthStage == 2;
					if (flag4)
					{
						string progressName4 = LocalStringManager.Get("LK_JiaoPool_Growing");
						informationBar.CGet<TextMeshProUGUI>("ProgressName").text = progressName4;
						informationBar.CGet<TextMeshProUGUI>("ProgressValue").text = string.Format("{0}/{1}", maxMonth, maxMonth);
						informationBar.CGet<CImage>("Bar").fillAmount = 1f;
					}
				}
			}
		}
		informationBar.CGet<CButtonObsolete>("ChangeName_Btn").gameObject.SetActive(isShowChangeNameBtn);
		this.BindJiaoPoolChangeName(informationBar, index, data.GetNameText(), data);
		this.BindChangeNameBtnEvent(informationBar);
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x00096CAC File Offset: 0x00094EAC
	private void InitStopFeedingBtn(CButtonObsolete btn, int poolId, GameData.DLC.FiveLoong.Jiao data)
	{
		btn.gameObject.SetActive(true);
		TooltipInvoker tips = btn.GetComponent<TooltipInvoker>();
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		bool flag = data.GrowthStage == 0;
		if (flag)
		{
			bool flag2 = this.IsCurrentLocatedInIndustryBlock();
			if (flag2)
			{
				btn.interactable = true;
				btn.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_retrieve_0", true, null);
				btn.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().SetText(LocalStringManager.Get(LanguageKey.LK_JiaoPool_TakeBackEggTips), true);
				box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_TakeBackEggTips));
				box.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_TakeBackEggTips_Desc));
			}
			else
			{
				btn.interactable = false;
				btn.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_retrieve_1", true, null);
				btn.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().SetText(LocalStringManager.Get(LanguageKey.LK_JiaoPool_TakeBackEggTips), true);
				box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_TakeBackEggTips));
				box.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_TakeBackEggTips_Desc) + LocalStringManager.Get(LanguageKey.LK_JiaoPoolUnavailableFunction));
			}
		}
		else
		{
			bool flag3 = this.IsCurrentLocatedInIndustryBlock();
			if (flag3)
			{
				btn.interactable = true;
				btn.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_leave_0", true, null);
				btn.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().SetText(LocalStringManager.Get(LanguageKey.LK_JiaoPool_JiaoLeavePool), true);
				box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_JiaoLeavePool));
				LanguageKey desc = (data.GrowthStage == 1) ? LanguageKey.LK_JiaoPool_JiaoLeavePoolBtnTips_Desc : LanguageKey.LK_JiaoPool_JiaoLeavePoolBtnTips_Desc_2;
				box.Set("arg1", LocalStringManager.Get(desc));
			}
			else
			{
				btn.interactable = false;
				btn.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_leave_1", true, null);
				btn.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().SetText(LocalStringManager.Get(LanguageKey.LK_JiaoPool_JiaoLeavePool), true);
				box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_JiaoLeavePool));
				LanguageKey desc2 = (data.GrowthStage == 1) ? LanguageKey.LK_JiaoPool_JiaoLeavePoolBtnTips_Desc : LanguageKey.LK_JiaoPool_JiaoLeavePoolBtnTips_Desc_2;
				box.Set("arg1", LocalStringManager.Get(desc2) + LocalStringManager.Get(LanguageKey.LK_JiaoPoolUnavailableFunction));
			}
		}
		tips.RuntimeParam = box;
		btn.ClearAndAddListener(delegate
		{
			this.TakeOutJiaoEvent(poolId);
		});
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00096FAC File Offset: 0x000951AC
	private void InitInteractiveBtn(CButtonObsolete btn, int poolId, GameData.DLC.FiveLoong.Jiao data)
	{
		TooltipInvoker tips = btn.GetComponent<TooltipInvoker>();
		tips.enabled = false;
		bool flag = data != null;
		if (flag)
		{
			tips.Type = ((data.GrowthStage == 0) ? TipType.JiaoEgg : TipType.Jiao);
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(this, data.Key, delegate(int offset, RawDataPool dataPool)
			{
				JiaoLoongDisplayData displayData = new JiaoLoongDisplayData();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				bool flag2 = data.GrowthStage != 0;
				if (flag2)
				{
					box.Set("HideLoongView", true);
					box.SetObject("JiaoLoongData", displayData);
				}
				else
				{
					box.SetObject("ItemData", displayData.ItemDisplayData);
				}
				tips.RuntimeParam = box;
				tips.enabled = true;
			});
		}
		btn.gameObject.SetActive(true);
		btn.ClearAndAddListener(delegate
		{
			ExtraDomainMethod.Call.JiaoPoolInteract(poolId);
		});
		PointerTrigger pointerTrigger = btn.GetComponent<PointerTrigger>();
		PointerTrigger pointerTrigger2 = pointerTrigger;
		if (pointerTrigger2.EnterEvent == null)
		{
			pointerTrigger2.EnterEvent = new UnityEvent();
		}
		pointerTrigger.EnterEvent.RemoveAllListeners();
		pointerTrigger.EnterEvent.AddListener(new UnityAction(this.ShowHighlightEffect));
		pointerTrigger2 = pointerTrigger;
		if (pointerTrigger2.ExitEvent == null)
		{
			pointerTrigger2.ExitEvent = new UnityEvent();
		}
		pointerTrigger.ExitEvent.RemoveAllListeners();
		pointerTrigger.ExitEvent.AddListener(new UnityAction(this.HideHighlightEffect));
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000970F8 File Offset: 0x000952F8
	private void InitFeedingConsumption(Refers feedingConsumption, GameData.DLC.FiveLoong.Jiao data, bool isLuan = false)
	{
		JiaoNurturanceItem nurturance = JiaoNurturance.Instance.GetItem(data.NurturanceTemplateId);
		ResourceTypeItem resourceTypeItem = ResourceType.Instance.GetItem(nurturance.ResourceCostType);
		bool flag = nurturance.ResourceCostType == -1 && nurturance.ExpCost <= 0;
		if (!flag)
		{
			feedingConsumption.gameObject.SetActive(true);
			TooltipInvoker tips = feedingConsumption.GetComponent<TooltipInvoker>();
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurtureExpend));
			box.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurtureExpendTips_Desc_0) + LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurtureExpendTips_Desc_1) + LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurtureExpendTips_Desc_2));
			tips.RuntimeParam = box;
			feedingConsumption.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get("LK_JiaoPool_NurtureExpend") + "：";
			bool flag2 = nurturance.ExpCost > 0;
			if (flag2)
			{
				feedingConsumption.CGet<CImage>("ResourceIcon").SetSprite("sp_icon_lilian", false, null);
				feedingConsumption.CGet<TextMeshProUGUI>("ResourceName").text = LocalStringManager.Get("LK_Exp");
			}
			else
			{
				feedingConsumption.CGet<CImage>("ResourceIcon").SetSprite(resourceTypeItem.Icon, false, null);
				feedingConsumption.CGet<TextMeshProUGUI>("ResourceName").text = (resourceTypeItem.Name ?? "");
			}
			feedingConsumption.CGet<TextMeshProUGUI>("ResourceValue").text = string.Format("{0}", isLuan ? 0 : nurturance.ResourceCost);
		}
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x00097284 File Offset: 0x00095484
	private void InitBreedingBtn(CButtonObsolete breedingBtn, GameData.DLC.FiveLoong.Jiao data, int index, bool isMate)
	{
		breedingBtn.gameObject.SetActive(true);
		if (isMate)
		{
			TooltipInvoker tips = breedingBtn.GetComponent<TooltipInvoker>();
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_StopBreeding));
			box.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_StopBreedingTips_Desc));
			tips.RuntimeParam = box;
			breedingBtn.transform.Find("Icon").GetComponent<CImage>().SetSprite("sp_icon_zanting_0", true, null);
			breedingBtn.transform.Find("Title").GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 32f);
			breedingBtn.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get(LanguageKey.LK_JiaoPool_StopBreeding);
			breedingBtn.ClearAndAddListener(delegate
			{
				breedingBtn.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_breed_0", true, null);
				this.TakeOutAllJiao(index);
			});
		}
		else
		{
			breedingBtn.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get(LanguageKey.LK_JiaoPool_Breeding);
			TooltipInvoker tips2 = breedingBtn.GetComponent<TooltipInvoker>();
			ArgumentBox box2 = EasyPool.Get<ArgumentBox>();
			box2.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_Breeding));
			box2.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_BreedingTips_Desc));
			tips2.RuntimeParam = box2;
			breedingBtn.transform.Find("Title").GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 32f);
			breedingBtn.transform.Find("Icon").GetComponent<CImage>().SetSprite("building_jiaochi_icon_breed_0", true, null);
			breedingBtn.ClearAndAddListener(delegate
			{
				this.PoolBreeding(data, index);
			});
		}
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x000974B0 File Offset: 0x000956B0
	private void InitNurturanceTips(Refers demandAnima, GameData.DLC.FiveLoong.Jiao data)
	{
		TooltipInvoker tips = demandAnima.GetComponent<TooltipInvoker>();
		JiaoNurturanceItem nurturance = JiaoNurturance.Instance.GetItem(data.NurturanceTemplateId);
		List<IntPair> basePropertyChange = nurturance.BasePropertyChange;
		short property = (short)basePropertyChange[0].First;
		JiaoPropertyItem propertyItem = Config.JiaoProperty.Instance.GetItem(property);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		string plan = string.Format(LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurturePlanTips_Desc_General_0), nurturance.Name, "<SpName=" + propertyItem.TipsIcon + ">", propertyItem.Name);
		string warning = LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurturePlanTips_Desc_General_1);
		bool flag = basePropertyChange.Count > 0;
		if (flag)
		{
			int rawValue = basePropertyChange[0].Second;
			LanguageKey growKey = propertyItem.IncreaseIsGood ? LanguageKey.LK_JiaoPool_NurturePlanTips_Desc_GrowRemaining : LanguageKey.LK_JiaoPool_NurturePlanTips_Desc_GrowReducing;
			bool flag2 = property == 1;
			string displayValue;
			if (flag2)
			{
				displayValue = ((float)rawValue / 10000f).ToString("0.#");
			}
			else
			{
				bool flag3 = property == 0 || property == 2 || property == 3 || property == 5;
				if (flag3)
				{
					displayValue = string.Format("{0}%", rawValue / 100);
				}
				else
				{
					displayValue = (rawValue / 100).ToString();
				}
			}
			warning = warning + "\n" + string.Format(LocalStringManager.Get(growKey), new object[]
			{
				data.NextPeriod,
				data.GetNameText(),
				propertyItem.TipsIcon,
				propertyItem.Name,
				displayValue
			});
		}
		box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_NurturancePlan));
		box.Set("arg1", plan + warning);
		tips.Type = TipType.Simple;
		tips.RuntimeParam = box;
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x00097674 File Offset: 0x00095874
	private void InitNurtureHosting(Refers refers, JiaoPool poolData, int poolId)
	{
		GameObject entrust = refers.CGet<GameObject>("Entrust");
		GameObject babysitting = refers.CGet<GameObject>("Babysitting");
		CButtonObsolete entrustBtn = refers.CGet<CButtonObsolete>("EntrustBtn");
		CButtonObsolete babysittingBtn = refers.CGet<CButtonObsolete>("BabysittingBtn");
		TooltipInvoker entrustTips = entrust.GetComponent<TooltipInvoker>();
		TooltipInvoker babysittingTips = babysitting.GetComponent<TooltipInvoker>();
		TooltipInvoker entrustBtnTips = entrustBtn.GetComponent<TooltipInvoker>();
		TooltipInvoker babysittingBtnTips = babysittingBtn.GetComponent<TooltipInvoker>();
		entrustTips.enabled = false;
		babysittingTips.enabled = false;
		entrust.SetActive(!poolData.isBabysitting);
		babysitting.SetActive(poolData.isBabysitting);
		entrustBtn.ClearAndAddListener(delegate
		{
			this.SetBabysitting(poolId, true);
			entrust.SetActive(false);
			babysitting.SetActive(true);
		});
		babysittingBtn.ClearAndAddListener(delegate
		{
			this.SetBabysitting(poolId, false);
			entrust.SetActive(true);
			babysitting.SetActive(false);
		});
		ArgumentBox entrustBtnTipsBox = EasyPool.Get<ArgumentBox>();
		entrustBtnTipsBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_Parenting));
		entrustBtnTipsBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_ParentingTips));
		ArgumentBox babysittingBtnBox = EasyPool.Get<ArgumentBox>();
		babysittingBtnBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_EntrustedParenting));
		babysittingBtnBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_EntrustedParentingTips));
		entrustBtnTips.RuntimeParam = entrustBtnTipsBox;
		babysittingBtnTips.RuntimeParam = babysittingBtnBox;
		entrustBtnTips.Type = TipType.Simple;
		babysittingBtnTips.Type = TipType.Simple;
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x000977E8 File Offset: 0x000959E8
	private void InitAppeaseButton(Refers refers, GameData.DLC.FiveLoong.Jiao data, int poolId)
	{
		int currentRemainingTime = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth / 10;
		bool isAppease = currentRemainingTime >= 10;
		CButtonObsolete appeaseBtn = refers.CGet<CButtonObsolete>("AppeaseBtn");
		CButtonObsolete appeaseCoolingBtn = refers.CGet<CButtonObsolete>("AppeaseCoolingBtn");
		appeaseBtn.gameObject.SetActive(false);
		appeaseCoolingBtn.gameObject.SetActive(false);
		TooltipInvoker appeaseBtnTips = appeaseBtn.GetComponent<TooltipInvoker>();
		TooltipInvoker appeaseCoolingBtnTips = appeaseCoolingBtn.GetComponent<TooltipInvoker>();
		List<MouseTipDynamicCondition.ConditionData> conditionData = new List<MouseTipDynamicCondition.ConditionData>();
		MouseTipDynamicCondition.ConditionData conditionElement = new MouseTipDynamicCondition.ConditionData
		{
			Icon = "mousetip_shijian",
			Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AppeaseTipConditionsName)
		};
		bool flag = isAppease;
		if (flag)
		{
			conditionElement.AddValueString = string.Format("{0}<color=#pinkyellow>/10</color>", currentRemainingTime);
		}
		else
		{
			conditionElement.ReduceValueString = string.Format("{0}<color=#pinkyellow>/10</color>", currentRemainingTime);
		}
		conditionData.Add(conditionElement);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("Title", LocalStringManager.Get(LanguageKey.LK_JiaoPool_Appease));
		box.Set("Desc", LocalStringManager.Get(LanguageKey.LK_JiaoPool_AppeaseTipDesc));
		box.Set("SubTitle", LocalStringManager.Get(LanguageKey.LK_JiaoPool_AppeaseTipSubTitle));
		box.SetObject("Conditions", conditionData);
		bool flag2 = data.PettingCoolDown > SingletonObject.getInstance<BasicGameData>().CurrDate;
		if (flag2)
		{
			appeaseCoolingBtn.gameObject.SetActive(true);
			appeaseCoolingBtn.transform.Find("TitleBg/ValueText").GetComponent<TextMeshProUGUI>().text = string.Format("{0}", data.PettingCoolDown - SingletonObject.getInstance<BasicGameData>().CurrDate);
			conditionData.Add(new MouseTipDynamicCondition.ConditionData
			{
				Name = "",
				IsSpecial = true
			});
			conditionData.Add(new MouseTipDynamicCondition.ConditionData
			{
				Name = LocalStringManager.Get(LanguageKey.LK_JiaoPool_AppeaseTipConditionsSpecial),
				IsSpecial = true
			});
			appeaseCoolingBtnTips.RuntimeParam = box;
		}
		else
		{
			appeaseBtn.gameObject.SetActive(true);
			appeaseBtn.interactable = isAppease;
			appeaseBtn.transform.Find("Icon").GetComponent<CImage>().SetSprite(isAppease ? "building_jiaochi_icon_appease_0" : "building_jiaochi_icon_appease_1", false, null);
			appeaseBtn.ClearAndAddListener(delegate
			{
				ExtraDomainMethod.Call.JiaoPoolPetJiao(poolId);
				appeaseBtn.interactable = false;
			});
			appeaseBtnTips.RuntimeParam = box;
		}
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x00097A70 File Offset: 0x00095C70
	private void RefreshPoolProgressBars()
	{
		bool flag = !base.isActiveAndEnabled || this._allJiaoPoolData == null || this._currentOpenJiaoPooCount <= 0;
		if (!flag)
		{
			for (int i = 0; i < this._currentOpenJiaoPooCount; i++)
			{
				bool flag2 = this._allJiaoPoolData[i].Jiaos.Count <= 0;
				if (!flag2)
				{
					int poolId = i;
					Refers informationBar = this._baseUIList[poolId].CGet<Refers>("InformationBar");
					bool flag3 = !informationBar.gameObject.activeSelf;
					if (!flag3)
					{
						ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(this, poolId, delegate(int offset, RawDataPool dataPool)
						{
							List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
							Serializer.Deserialize(dataPool, offset, ref jiaoList);
							bool flag4 = jiaoList.Count <= 0;
							if (!flag4)
							{
								UI_BuildingJiaoPool.PoolState poolState = this.GetPoolState(this._allJiaoPoolData[poolId], jiaoList[0]);
								if (!true)
								{
								}
								int num;
								if (poolState != UI_BuildingJiaoPool.PoolState.Breeding)
								{
									if (poolState != UI_BuildingJiaoPool.PoolState.BreedingEnd)
									{
										num = 1;
									}
									else
									{
										num = 3;
									}
								}
								else
								{
									num = 2;
								}
								if (!true)
								{
								}
								int jiaoCount = num;
								bool showChangeName = poolState == UI_BuildingJiaoPool.PoolState.Egg || poolState == UI_BuildingJiaoPool.PoolState.Minor || poolState == UI_BuildingJiaoPool.PoolState.Adult;
								this.InitInformationBar(informationBar, this._allJiaoPoolData[poolId], jiaoList[0], poolId, jiaoCount, showChangeName);
							}
						});
					}
				}
			}
		}
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x00097B50 File Offset: 0x00095D50
	private void ResetUISate(int poolId)
	{
		Refers ui = this._baseUIList[poolId];
		ui.CGet<CButtonObsolete>("Breeding_Btn").gameObject.SetActive(false);
		ui.CGet<CButtonObsolete>("StopFeeding_Btn").gameObject.SetActive(false);
		ui.CGet<CButtonObsolete>("Add_Btn").gameObject.SetActive(false);
		ui.CGet<Refers>("BreedingEnd").gameObject.SetActive(false);
		ui.CGet<Refers>("InformationBar").gameObject.SetActive(false);
		ui.CGet<Refers>("FeedingConsumption").gameObject.SetActive(false);
		ui.CGet<Refers>("DemandAnima").gameObject.SetActive(false);
		ui.CGet<CButtonObsolete>("InteractiveBtn").gameObject.SetActive(false);
		ui.CGet<GameObject>("Entrust").SetActive(false);
		ui.CGet<GameObject>("Babysitting").SetActive(false);
		ui.CGet<CButtonObsolete>("AppeaseBtn").gameObject.SetActive(false);
		ui.CGet<CButtonObsolete>("AppeaseCoolingBtn").gameObject.SetActive(false);
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x00097C78 File Offset: 0x00095E78
	private void InitReturnAndChangeLoong()
	{
		Refers refers = this._root.CGet<Refers>("Return");
		refers.CGet<CButtonObsolete>("ChangeLoong_Btn").ClearAndAddListener(delegate
		{
			UIManager.Instance.ShowUI(UIElement.JiaoChangeLoong, true);
		});
		refers.CGet<CButtonObsolete>("Return_Btn").ClearAndAddListener(delegate
		{
			UIManager.Instance.HideUI(UIElement.BuildingJiaoPool);
		});
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00097CF8 File Offset: 0x00095EF8
	private void InitRoad(int landFormType)
	{
		foreach (GameObject item in this._roadList)
		{
			item.GetComponent<CImage>().SetSprite(string.Format("buildingarea_jiaochi_road_{0}", landFormType), false, null);
		}
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x00097D68 File Offset: 0x00095F68
	private void OpenRoad(int index, int landFormType)
	{
		List<int> data = this._poolTurningPointConfig[index];
		foreach (int item in data)
		{
			this._roadList[item].gameObject.SetActive(true);
		}
		bool flag = data.Count == 2;
		if (flag)
		{
			GameObject roadTurningPoint = base.CGet<Refers>("RoadTurningPoint").CGet<GameObject>(string.Format("Connect_{0}", (index < 2) ? index : (index - 1)));
			roadTurningPoint.GetComponent<CImage>().SetSprite(string.Format("buildingarea_jiaochi_connect_{0}", landFormType), false, null);
			roadTurningPoint.SetActive(true);
		}
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00097E3C File Offset: 0x0009603C
	private void PlayJiaoEggPoolEntryAnima(SkeletonGraphic playAnima)
	{
		CommonUtils.SetSkeletonDataAsset(playAnima, base.CGet<SkeletonDataAsset>("LuanAnimaData"), "default", base.CGet<SkeletonDataAsset>("LuanAnimaData").GetAnimationStateData().SkeletonData.Animations.Items[0].Name, true);
		playAnima.AnimationState.SetAnimation(0, playAnima.SkeletonData.Animations.Items[0].Name, false);
		playAnima.gameObject.SetActive(true);
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x00097EBC File Offset: 0x000960BC
	private void PlayJiaoEggPoolIdleAnima(Refers refers)
	{
		SkeletonGraphic playAnima = refers.CGet<SkeletonGraphic>("PlayWaterAnima");
		CommonUtils.SetSkeletonDataAsset(playAnima, base.CGet<SkeletonDataAsset>("LuanAnimaData"), "default", base.CGet<SkeletonDataAsset>("LuanAnimaData").GetAnimationStateData().SkeletonData.Animations.Items[1].Name, true);
		playAnima.AnimationState.SetAnimation(1, playAnima.SkeletonData.Animations.Items[1].Name, true);
		playAnima.gameObject.SetActive(true);
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x00097F48 File Offset: 0x00096148
	private void PlayOneJiaoAnima(Refers refers)
	{
		int value = Random.Range(9, 18);
		SkeletonGraphic playAnima = refers.CGet<SkeletonGraphic>("PlayUnderWaterAnima");
		Spine.Animation[] items = base.CGet<SkeletonDataAsset>("JiaoAnimaData").GetAnimationStateData().SkeletonData.Animations.Items;
		CommonUtils.SetSkeletonDataAsset(playAnima, base.CGet<SkeletonDataAsset>("JiaoAnimaData"), "default", items[value].Name, true);
		playAnima.AnimationState.SetAnimation(0, items[value].Name, true);
		playAnima.gameObject.SetActive(true);
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x00097FD0 File Offset: 0x000961D0
	private void PlayTwoJiaoAnima(Refers refers)
	{
		int value = Random.Range(0, 9);
		SkeletonGraphic playAnima = refers.CGet<SkeletonGraphic>("PlayUnderWaterAnima");
		Spine.Animation[] items = base.CGet<SkeletonDataAsset>("JiaoAnimaData").GetAnimationStateData().SkeletonData.Animations.Items;
		CommonUtils.SetSkeletonDataAsset(playAnima, base.CGet<SkeletonDataAsset>("JiaoAnimaData"), "default", items[value].Name, true);
		playAnima.AnimationState.SetAnimation(1, items[value].Name, true);
		playAnima.gameObject.SetActive(true);
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x00098058 File Offset: 0x00096258
	private void PlayNurtureTypeAnima(Refers refers, SkeletonDataAsset skeletonDataAsset)
	{
		refers.gameObject.SetActive(true);
		SkeletonGraphic playAnima = refers.CGet<SkeletonGraphic>("Anima");
		CommonUtils.SetSkeletonDataAsset(playAnima, skeletonDataAsset, "default", skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations.Items[0].Name, true);
		playAnima.AnimationState.SetAnimation(0, playAnima.SkeletonData.Animations.Items[0].Name, true);
		playAnima.gameObject.SetActive(true);
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x000980DC File Offset: 0x000962DC
	private void GenerateJiaoAttributeEffect(Refers refers, GameData.DLC.FiveLoong.Jiao data, int style)
	{
		List<ValueTuple<float, float>> posList = this._attributeEffectPosList[style];
		RectTransform effectParent = refers.CGet<RectTransform>("Effect");
		JiaoItem jiaoConfig = this.GetJiaoConfig(data.TemplateId);
		List<string> colorList = jiaoConfig.ColorList;
		for (int i = 0; i < colorList.Count; i++)
		{
			effectParent.Find(colorList[i]).transform.localPosition = new Vector2(posList[i].Item1, posList[i].Item2);
			effectParent.Find(colorList[i]).gameObject.SetActive(true);
		}
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0009818C File Offset: 0x0009638C
	private void GenerateBreedingAttributeEffect(Refers refers, List<GameData.DLC.FiveLoong.Jiao> dataList, int style)
	{
		HashSet<string> colorHashSet = new HashSet<string>();
		List<string> colorList = new List<string>();
		List<ValueTuple<float, float>> posList = this._attributeEffectPosList[style];
		RectTransform effectParent = refers.CGet<RectTransform>("Effect");
		foreach (string t in (from jiao in dataList
		where jiao.GrowthStage == 2
		select this.GetJiaoConfig(jiao.TemplateId)).SelectMany((JiaoItem jiaoConfig) => jiaoConfig.ColorList))
		{
			colorHashSet.Add(t);
		}
		colorList.AddRange(colorHashSet);
		for (int i = 0; i < colorList.Count; i++)
		{
			effectParent.Find(colorList[i]).transform.localPosition = new Vector2(posList[i].Item1, posList[i].Item2);
			effectParent.Find(colorList[i]).gameObject.SetActive(true);
		}
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x000982DC File Offset: 0x000964DC
	private JiaoItem GetJiaoConfig(short templateId)
	{
		return Config.Jiao.Instance.GetItem(templateId);
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x000982FC File Offset: 0x000964FC
	private void PoolInvestEvent(GameData.DLC.FiveLoong.Jiao data, int index)
	{
		ArgumentBox argumentBox = new ArgumentBox();
		Action<int, ItemKey> <>9__3;
		ExtraDomainMethod.AsyncCall.GetAllJiaoForPool(this, delegate(int offset, RawDataPool dataPool)
		{
			this.itemDisplayDataList.Clear();
			Serializer.Deserialize(dataPool, offset, ref this.itemDisplayDataList);
			List<ItemDisplayData> displayData = new List<ItemDisplayData>();
			bool flag = this.itemDisplayDataList.Count > 0;
			if (flag)
			{
				foreach (ItemDisplayData itemDisplay in this.itemDisplayDataList)
				{
					bool flag2 = itemDisplay.ItemSourceType != 7;
					if (flag2)
					{
						displayData.Add(itemDisplay);
					}
				}
			}
			List<ItemKey> allItemKey = (from display in displayData
			select display.Key).ToList<ItemKey>();
			ExtraDomainMethod.AsyncCall.GetJiaosByItemKeys(this, allItemKey, delegate(int allItemkeyOffset, RawDataPool allItemkeyDataPool)
			{
				List<GameData.DLC.FiveLoong.Jiao> allJiaoData = new List<GameData.DLC.FiveLoong.Jiao>();
				Serializer.Deserialize(allItemkeyDataPool, allItemkeyOffset, ref allJiaoData);
				ArgumentBox argumentBox;
				argumentBox.Set("Type", 0);
				argumentBox.Set<GameData.DLC.FiveLoong.Jiao>("CurrentJiao", data);
				argumentBox.Set("PoolId", index);
				argumentBox.SetObject("AllItemDisplayData", displayData);
				argumentBox.SetObject("AllJiaoData", allJiaoData);
				argumentBox.Set("Title", LocalStringManager.Get("LK_JiaoPool_InvestJiaoPool"));
				argumentBox = argumentBox;
				string key = "OnConfirm";
				Action<int, ItemKey> arg;
				if ((arg = <>9__3) == null)
				{
					arg = (<>9__3 = delegate(int poolId, ItemKey itemKey)
					{
						ExtraDomainMethod.AsyncCall.GetJiaoByItemKey(this, itemKey, delegate(int jiaoOffset, RawDataPool jiaoDataPool)
						{
							GameData.DLC.FiveLoong.Jiao selectJiao = new GameData.DLC.FiveLoong.Jiao();
							Serializer.Deserialize(jiaoDataPool, jiaoOffset, ref selectJiao);
							bool flag3 = selectJiao.GrowthStage == 0;
							if (flag3)
							{
								ExtraDomainMethod.Call.PutEggIntoPool(index, itemKey);
							}
							else
							{
								ExtraDomainMethod.Call.PutJiaoInPool(index, selectJiao.Id);
							}
						});
					});
				}
				argumentBox.SetObject(key, arg);
				UIElement.JiaoPoolSelectItem.SetOnInitArgs(argumentBox);
				UIManager.Instance.ShowUI(UIElement.JiaoPoolSelectItem, true);
			});
		});
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x00098344 File Offset: 0x00096544
	private void PoolBreeding(GameData.DLC.FiveLoong.Jiao data, int index)
	{
		ArgumentBox argumentBox = new ArgumentBox();
		AsyncMethodCallbackDelegate <>9__4;
		Action<int, ItemKey> <>9__3;
		AsyncMethodCallbackDelegate <>9__2;
		ExtraDomainMethod.AsyncCall.GetAllJiaoForPool(this, delegate(int offset, RawDataPool dataPool)
		{
			this.itemDisplayDataList.Clear();
			Serializer.Deserialize(dataPool, offset, ref this.itemDisplayDataList);
			List<ItemKey> allItemKey = (from display in this.itemDisplayDataList
			select display.Key).ToList<ItemKey>();
			IAsyncMethodRequestHandler <>4__this = this;
			List<ItemKey> keys = allItemKey;
			AsyncMethodCallbackDelegate callback;
			if ((callback = <>9__2) == null)
			{
				callback = (<>9__2 = delegate(int allItemkeyOffset, RawDataPool allItemkeyDataPool)
				{
					List<GameData.DLC.FiveLoong.Jiao> allJiaoData = new List<GameData.DLC.FiveLoong.Jiao>();
					Serializer.Deserialize(allItemkeyDataPool, allItemkeyOffset, ref allJiaoData);
					ArgumentBox argumentBox;
					argumentBox.Set("Type", 1);
					argumentBox.Set<GameData.DLC.FiveLoong.Jiao>("CurrentJiao", data);
					argumentBox.Set("PoolId", index);
					argumentBox.Set("IsCurrentLocatedInIndustryBlock", this.IsCurrentLocatedInIndustryBlock());
					argumentBox.SetObject("AllItemDisplayData", this.itemDisplayDataList);
					argumentBox.SetObject("AllJiaoPool", this._allJiaoPoolData);
					argumentBox.SetObject("AllJiaoData", allJiaoData);
					argumentBox.Set("Title", LocalStringManager.Get("LK_JiaoPool_Breeding_Title"));
					argumentBox = argumentBox;
					string key = "OnConfirm";
					Action<int, ItemKey> arg;
					if ((arg = <>9__3) == null)
					{
						arg = (<>9__3 = delegate(int poolId, ItemKey itemKey)
						{
							IAsyncMethodRequestHandler <>4__this2 = this;
							AsyncMethodCallbackDelegate callback2;
							if ((callback2 = <>9__4) == null)
							{
								callback2 = (<>9__4 = delegate(int jiaoOffset, RawDataPool jiaoDataPool)
								{
									GameData.DLC.FiveLoong.Jiao jiaoData = new GameData.DLC.FiveLoong.Jiao();
									Serializer.Deserialize(jiaoDataPool, jiaoOffset, ref jiaoData);
									ExtraDomainMethod.Call.PutAnotherJiaoInPool(index, jiaoData.Id);
									this.RenderPoolState(index, 4);
								});
							}
							ExtraDomainMethod.AsyncCall.GetJiaoByItemKey(<>4__this2, itemKey, callback2);
						});
					}
					argumentBox.SetObject(key, arg);
					UIElement.JiaoPoolSelectItem.SetOnInitArgs(argumentBox);
					UIManager.Instance.ShowUI(UIElement.JiaoPoolSelectItem, true);
				});
			}
			ExtraDomainMethod.AsyncCall.GetJiaosByItemKeys(<>4__this, keys, callback);
		});
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x0009838C File Offset: 0x0009658C
	private void RenderPoolState(int poolId, int poolState)
	{
		switch (poolState)
		{
		case 1:
			this.PoolInvestEndEvent(poolId, 1, this._baseList[poolId], this._baseUIList[poolId]);
			break;
		case 2:
			this.PoolInvestEndEvent(poolId, 2, this._baseList[poolId], this._baseUIList[poolId]);
			break;
		case 3:
			this.PoolInvestEndEvent(poolId, 3, this._baseList[poolId], this._baseUIList[poolId]);
			break;
		case 4:
			this.PoolInvestEndEvent(poolId, 4, this._baseList[poolId], this._baseUIList[poolId]);
			break;
		case 5:
			this.ChangeNurturance(poolId);
			break;
		case 6:
			this.NurtureEnd(poolId);
			break;
		case 7:
			this.ResetUISate(poolId);
			this.InitOpenPool(this._baseList[poolId], this._baseUIList[poolId], (int)this._landFormType, poolId);
			break;
		case 8:
			this.ResetUISate(poolId);
			this.InitOpenPool(this._baseList[poolId], this._baseUIList[poolId], (int)this._landFormType, poolId);
			break;
		case 10:
			this.ResetUISate(poolId);
			this.InitOpenPool(this._baseList[poolId], this._baseUIList[poolId], (int)this._landFormType, poolId);
			break;
		case 11:
			this.ResetAllPoolUIState();
			break;
		}
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x00098524 File Offset: 0x00096724
	private void ResetAllPoolUIState()
	{
		ExtraDomainMethod.AsyncCall.GetJiaoPoolList(this, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._allJiaoPoolData);
			for (int i = 0; i < this._currentOpenJiaoPooCount; i++)
			{
				bool flag = this._allJiaoPoolData[i].Jiaos.Count <= 0;
				if (!flag)
				{
					int value = i;
					ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(this, i, delegate(int poolOffset, RawDataPool poolDataPool)
					{
						int index = value;
						List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
						Serializer.Deserialize(poolDataPool, poolOffset, ref jiaoList);
						switch (this.GetPoolState(this._allJiaoPoolData[index], jiaoList[0]))
						{
						case UI_BuildingJiaoPool.PoolState.Egg:
							this.InitHaveLuan(this._baseList[index], this._baseUIList[index], this._allJiaoPoolData[index], jiaoList[0], (int)this._landFormType, index);
							break;
						case UI_BuildingJiaoPool.PoolState.Minor:
							this.InitHaveUnderageJiao(this._baseList[index], this._baseUIList[index], this._allJiaoPoolData[index], jiaoList[0], (int)this._landFormType, index);
							break;
						case UI_BuildingJiaoPool.PoolState.Breeding:
							this.InitBreedingJiaoPool(this._baseList[index], this._baseUIList[index], this._allJiaoPoolData[index], jiaoList, (int)this._landFormType, index);
							break;
						case UI_BuildingJiaoPool.PoolState.Adult:
							this.InitHaveAdultJiao(this._baseList[index], this._baseUIList[index], this._allJiaoPoolData[index], jiaoList[0], (int)this._landFormType, index);
							break;
						case UI_BuildingJiaoPool.PoolState.BreedingEnd:
							this.InitBreedingEndJiaoPool(this._baseList[index], this._baseUIList[index], this._allJiaoPoolData[index], jiaoList, (int)this._landFormType, index);
							break;
						default:
							throw new ArgumentOutOfRangeException();
						}
					});
				}
			}
			ExtraDomainMethod.Call.ResetJiaoPoolStatus();
		});
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x0009853C File Offset: 0x0009673C
	private void PoolInvestEndEvent(int poolId, int status, Refers baseRefers, Refers uiRefers)
	{
		uiRefers.CGet<CButtonObsolete>("Add_Btn").gameObject.SetActive(false);
		bool flag = status == 1;
		if (flag)
		{
			this.PlayJiaoEggPoolEntryAnima(baseRefers.CGet<SkeletonGraphic>("PlayWaterAnima"));
			AudioManager.Instance.PlaySound("ui_building_jiaochi_luan", false, false);
			AsyncMethodCallbackDelegate <>9__3;
			AsyncMethodCallbackDelegate <>9__2;
			base.StartCoroutine(this.WaitAnimaEnd(1f, delegate
			{
				IAsyncMethodRequestHandler <>4__this = this;
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__2) == null)
				{
					callback = (<>9__2 = delegate(int offset, RawDataPool dataPool)
					{
						Serializer.Deserialize(dataPool, offset, ref this._allJiaoPoolData);
						IAsyncMethodRequestHandler <>4__this2 = this;
						int poolId2 = poolId;
						AsyncMethodCallbackDelegate callback2;
						if ((callback2 = <>9__3) == null)
						{
							callback2 = (<>9__3 = delegate(int offset2, RawDataPool dataPool2)
							{
								List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
								Serializer.Deserialize(dataPool2, offset2, ref jiaoList);
								this.InitHaveLuan(baseRefers, uiRefers, this._allJiaoPoolData[poolId], jiaoList[0], (int)this._landFormType, poolId);
								bool flag2 = !UIManager.Instance.CheckPopupElementIsInTop(this.Element);
								if (flag2)
								{
									this.ResetUICanvasSort();
								}
								ExtraDomainMethod.Call.ResetJiaoPoolStatus();
							});
						}
						ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(<>4__this2, poolId2, callback2);
					});
				}
				ExtraDomainMethod.AsyncCall.GetJiaoPoolList(<>4__this, callback);
			}));
		}
		else
		{
			AsyncMethodCallbackDelegate <>9__4;
			ExtraDomainMethod.AsyncCall.GetJiaoPoolList(this, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._allJiaoPoolData);
				IAsyncMethodRequestHandler <>4__this = this;
				int poolId2 = poolId;
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__4) == null)
				{
					callback = (<>9__4 = delegate(int offset2, RawDataPool dataPool2)
					{
						List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
						Serializer.Deserialize(dataPool2, offset2, ref jiaoList);
						bool flag2 = status == 2;
						if (flag2)
						{
							this.InitHaveUnderageJiao(baseRefers, uiRefers, this._allJiaoPoolData[poolId], jiaoList[0], (int)this._landFormType, poolId);
						}
						else
						{
							bool flag3 = status == 3;
							if (flag3)
							{
								this.InitHaveAdultJiao(baseRefers, uiRefers, this._allJiaoPoolData[poolId], jiaoList[0], (int)this._landFormType, poolId);
							}
							else
							{
								bool flag4 = status == 4;
								if (flag4)
								{
									this.InitBreedingJiaoPool(baseRefers, uiRefers, this._allJiaoPoolData[poolId], jiaoList, (int)this._landFormType, poolId);
								}
							}
						}
						ExtraDomainMethod.Call.ResetJiaoPoolStatus();
					});
				}
				ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(<>4__this, poolId2, callback);
			});
		}
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x00098600 File Offset: 0x00096800
	private void ChangeNurturance(int poolId)
	{
		AsyncMethodCallbackDelegate <>9__1;
		ExtraDomainMethod.AsyncCall.GetJiaoPoolList(this, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._allJiaoPoolData);
			IAsyncMethodRequestHandler <>4__this = this;
			int poolId2 = poolId;
			AsyncMethodCallbackDelegate callback;
			if ((callback = <>9__1) == null)
			{
				callback = (<>9__1 = delegate(int offset2, RawDataPool dataPool2)
				{
					List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
					Serializer.Deserialize(dataPool2, offset2, ref jiaoList);
					bool flag = jiaoList[0].GrowthStage == 0;
					if (flag)
					{
						this.ResetUISate(poolId);
						this.InitHaveLuanPoolUI(this._baseUIList[poolId], this._allJiaoPoolData[poolId], jiaoList[0], poolId);
						ExtraDomainMethod.Call.ResetJiaoPoolStatus();
					}
					bool flag2 = jiaoList[0].GrowthStage == 1;
					if (flag2)
					{
						this.ResetUISate(poolId);
						this.InitHaveUnderageJiaoPoolUI(this._baseUIList[poolId], this._allJiaoPoolData[poolId], jiaoList[0], poolId);
						ExtraDomainMethod.Call.ResetJiaoPoolStatus();
					}
				});
			}
			ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(<>4__this, poolId2, callback);
		});
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x00098638 File Offset: 0x00096838
	private void TakeOutJiaoEvent(int poolId)
	{
		this.ResetUISate(poolId);
		this.InitOpenPool(this._baseList[poolId], this._baseUIList[poolId], (int)this._landFormType, poolId);
		ExtraDomainMethod.AsyncCall.PutJiaoOutOfPool(this, poolId, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemKey> itemkeyList = new List<ItemKey>();
			Serializer.Deserialize(dataPool, offset, ref itemkeyList);
			ItemDomainMethod.AsyncCall.GetItemDisplayDataList(this, itemkeyList, delegate(int displayOffset, RawDataPool displayDataPool)
			{
				List<ItemDisplayData> displayDatas = new List<ItemDisplayData>();
				Serializer.Deserialize(displayDataPool, displayOffset, ref displayDatas);
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.SetObject("ItemList", displayDatas);
				UIElement.GetItem.SetOnInitArgs(argumentBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
				ExtraDomainMethod.Call.ResetJiaoPoolStatus();
			});
		});
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00098688 File Offset: 0x00096888
	private void NurtureEnd(int poolId)
	{
		AsyncMethodCallbackDelegate <>9__1;
		ExtraDomainMethod.AsyncCall.GetJiaoPoolList(this, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._allJiaoPoolData);
			IAsyncMethodRequestHandler <>4__this = this;
			int poolId2 = poolId;
			AsyncMethodCallbackDelegate callback;
			if ((callback = <>9__1) == null)
			{
				callback = (<>9__1 = delegate(int offset2, RawDataPool dataPool2)
				{
					List<GameData.DLC.FiveLoong.Jiao> jiaoList = new List<GameData.DLC.FiveLoong.Jiao>();
					Serializer.Deserialize(dataPool2, offset2, ref jiaoList);
					this.ResetUISate(poolId);
					this.InitHaveAdultJiao(this._baseList[poolId], this._baseUIList[poolId], this._allJiaoPoolData[poolId], jiaoList[0], (int)this._landFormType, poolId);
				});
			}
			ExtraDomainMethod.AsyncCall.GetJiaoPoolAllJiaoData(<>4__this, poolId2, callback);
		});
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x000986BD File Offset: 0x000968BD
	private void SetBabysitting(int poolId, bool isOn)
	{
		ExtraDomainMethod.Call.SetIsBabysittingMode(poolId, isOn);
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x000986C8 File Offset: 0x000968C8
	private IEnumerator WaitAnimaEnd(float time, Action action)
	{
		yield return new WaitForSeconds(time);
		action();
		yield break;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x000986E8 File Offset: 0x000968E8
	private void BindChangeNameBtnEvent(Refers refers)
	{
		CButtonObsolete changeNameBtn = refers.CGet<CButtonObsolete>("ChangeName_Btn");
		TooltipInvoker tips = changeNameBtn.GetComponent<TooltipInvoker>();
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("arg0", LocalStringManager.Get(LanguageKey.LK_JiaoPool_ChangeName));
		box.Set("arg1", LocalStringManager.Get(LanguageKey.LK_JiaoPool_ChangeNameBtnTips_Desc));
		tips.RuntimeParam = box;
		changeNameBtn.ClearAndAddListener(delegate
		{
			this.ChangeJiaoName(refers);
		});
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x00098770 File Offset: 0x00096970
	private void BindJiaoPoolChangeName(Refers refers, int jiaoId, string jiaoName, GameData.DLC.FiveLoong.Jiao data)
	{
		UI_BuildingJiaoPool.<>c__DisplayClass78_0 CS$<>8__locals1 = new UI_BuildingJiaoPool.<>c__DisplayClass78_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.data = data;
		CS$<>8__locals1.refers = refers;
		CS$<>8__locals1.jiaoId = jiaoId;
		CS$<>8__locals1.loongName = CS$<>8__locals1.refers.CGet<TextMeshProUGUI>("LoongName");
		CS$<>8__locals1.inputField = CS$<>8__locals1.refers.CGet<TMP_InputField>("ChangeName");
		CS$<>8__locals1.loongName.text = jiaoName;
		CS$<>8__locals1.inputField.SetTextWithoutNotify("");
		CS$<>8__locals1.inputField.onDeselect.RemoveAllListeners();
		CS$<>8__locals1.inputField.onDeselect.AddListener(new UnityAction<string>(CS$<>8__locals1.<BindJiaoPoolChangeName>g__EndEdit|0));
		CS$<>8__locals1.inputField.onSubmit.RemoveAllListeners();
		CS$<>8__locals1.inputField.onSubmit.AddListener(new UnityAction<string>(CS$<>8__locals1.<BindJiaoPoolChangeName>g__EndEdit|0));
		CS$<>8__locals1.inputField.onEndEdit.RemoveAllListeners();
		CS$<>8__locals1.inputField.onEndEdit.AddListener(new UnityAction<string>(CS$<>8__locals1.<BindJiaoPoolChangeName>g__EndEdit|0));
		CS$<>8__locals1.inputField.gameObject.SetActive(false);
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x00098888 File Offset: 0x00096A88
	private void SetOriginalName(TextMeshProUGUI loongName, GameData.DLC.FiveLoong.Jiao data)
	{
		bool flag = data.GrowthStage == 0;
		if (flag)
		{
			string value = Config.Material.Instance[data.Key.TemplateId].Name;
			loongName.text = value;
			ExtraDomainMethod.Call.ChangeJiaoName(data.Id, value);
		}
		else
		{
			loongName.text = Config.Jiao.Instance.GetItem(data.TemplateId).Name;
			ExtraDomainMethod.Call.ChangeJiaoName(data.Id, Config.Jiao.Instance.GetItem(data.TemplateId).Name);
		}
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x00098918 File Offset: 0x00096B18
	private void ChangeJiaoName(Refers informationBar)
	{
		this._isChangeName = true;
		TextMeshProUGUI loongName = informationBar.CGet<TextMeshProUGUI>("LoongName");
		TMP_InputField inputField = informationBar.CGet<TMP_InputField>("ChangeName");
		TMP_FontAsset fontAsset = inputField.textComponent.font;
		CButtonObsolete btn = informationBar.CGet<CButtonObsolete>("ChangeName_Btn");
		Regex reg = new Regex("^[一-龥]+$");
		loongName.gameObject.SetActive(false);
		inputField.gameObject.SetActive(true);
		btn.gameObject.SetActive(false);
		inputField.onValueChanged.RemoveAllListeners();
		inputField.textComponent.rectTransform.localPosition = Vector3.zero;
		inputField.onValueChanged.RemoveAllListeners();
		inputField.characterLimit = 6;
		inputField.onValueChanged.AddListener(delegate(string valueStr)
		{
			bool flag2 = CommonUtils.FixToShowAbleString(ref valueStr, fontAsset);
			if (flag2)
			{
				valueStr = valueStr.Replace(" ", string.Empty);
				bool flag3 = !string.IsNullOrEmpty(valueStr);
				if (flag3)
				{
					valueStr = valueStr.Substring(0, Mathf.Min(valueStr.Length, inputField.characterLimit - 1));
				}
				inputField.SetTextWithoutNotify(valueStr);
			}
			bool flag4 = reg.IsMatch(valueStr);
			if (!flag4)
			{
				valueStr = "";
				inputField.SetTextWithoutNotify(valueStr);
			}
		});
		inputField.onEndEdit.RemoveAllListeners();
		inputField.onEndEdit.AddListener(delegate(string valueStr)
		{
			bool flag2 = CommonUtils.FixToShowAbleString(ref valueStr, fontAsset);
			if (flag2)
			{
				valueStr = valueStr.Replace(" ", string.Empty).Substring(0, inputField.characterLimit - 1);
				inputField.SetTextWithoutNotify(valueStr);
			}
			inputField.textComponent.transform.localPosition = Vector2.zero;
			inputField.transform.Find("Text Area/Caret").localPosition = Vector2.zero;
		});
		inputField.gameObject.SetActive(true);
		bool flag = inputField.text.IsNullOrEmpty();
		if (flag)
		{
			inputField.Select();
		}
		inputField.InputOnSelectBindMoveTextEnd();
		inputField.ActivateInputField();
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x00098A90 File Offset: 0x00096C90
	private UI_BuildingJiaoPool.PoolState GetPoolState(JiaoPool poolData, GameData.DLC.FiveLoong.Jiao data)
	{
		switch (poolData.Jiaos.Count)
		{
		case 1:
			if (data.GrowthStage == 0)
			{
				return UI_BuildingJiaoPool.PoolState.Egg;
			}
			if (data.GrowthStage == 1)
			{
				return UI_BuildingJiaoPool.PoolState.Minor;
			}
			if (data.GrowthStage == 2)
			{
				return UI_BuildingJiaoPool.PoolState.Adult;
			}
			break;
		case 2:
			return UI_BuildingJiaoPool.PoolState.Breeding;
		case 3:
			return UI_BuildingJiaoPool.PoolState.BreedingEnd;
		}
		return UI_BuildingJiaoPool.PoolState.Empty;
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x00098AFC File Offset: 0x00096CFC
	private void TakeOutAllJiao(int index)
	{
		AsyncMethodCallbackDelegate <>9__1;
		ExtraDomainMethod.AsyncCall.PutJiaoOutOfPool(this, index, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemKey> itemKeys = new List<ItemKey>();
			Serializer.Deserialize(dataPool, offset, ref itemKeys);
			IAsyncMethodRequestHandler <>4__this = this;
			List<ItemKey> itemKeyList = itemKeys;
			AsyncMethodCallbackDelegate callback;
			if ((callback = <>9__1) == null)
			{
				callback = (<>9__1 = delegate(int itemOffset, RawDataPool itemDataPool)
				{
					List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
					Serializer.Deserialize(itemDataPool, itemOffset, ref itemDisplayDatas);
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
					argumentBox.SetObject("ItemList", itemDisplayDatas);
					UIElement.GetItem.SetOnInitArgs(argumentBox);
					UIManager.Instance.MaskUI(UIElement.GetItem);
					this.InitOpenPool(this._baseList[index], this._baseUIList[index], (int)this._landFormType, index);
				});
			}
			ItemDomainMethod.AsyncCall.GetItemDisplayDataList(<>4__this, itemKeyList, callback);
		});
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x00098B38 File Offset: 0x00096D38
	private void TopUiChanged(ArgumentBox box)
	{
		bool isTop = UIManager.Instance.CheckPopupElementIsInTop(this.Element);
		bool flag = isTop;
		if (flag)
		{
			this._sortCheckCoroutine = null;
			this.ResetUICanvasSort();
		}
		else
		{
			this.SetUICanvasSort();
			bool flag2 = this._sortCheckCoroutine == null;
			if (flag2)
			{
				this._sortCheckCoroutine = base.StartCoroutine(this.DelayedSortCheck());
			}
		}
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x00098B98 File Offset: 0x00096D98
	private IEnumerator DelayedSortCheck()
	{
		yield return null;
		this._sortCheckCoroutine = null;
		bool flag = !UIElement.EventWindow.Exist && !UIElement.Dialog.Exist;
		if (flag)
		{
			this.ResetUICanvasSort();
		}
		yield break;
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x00098BA8 File Offset: 0x00096DA8
	private void ResetUICanvasSort()
	{
		foreach (Refers item in this._baseUIList)
		{
			item.GetComponent<Canvas>().sortingOrder = 602;
		}
		foreach (Refers item2 in this._baseList)
		{
			foreach (ParticleSystem particle in item2.CGet<RectTransform>("Effect").GetComponentsInChildren<ParticleSystem>())
			{
				particle.GetComponent<Renderer>().sortingOrder = 601;
			}
		}
		base.CGet<GameObject>("Return").SetActive(true);
		this.RefreshChangeLoongButton();
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x00098CA4 File Offset: 0x00096EA4
	private void SetUICanvasSort()
	{
		foreach (Refers item in this._baseUIList)
		{
			item.GetComponent<Canvas>().sortingOrder = 509;
		}
		foreach (Refers item2 in this._baseList)
		{
			foreach (ParticleSystem particle in item2.CGet<RectTransform>("Effect").GetComponentsInChildren<ParticleSystem>())
			{
				particle.GetComponent<Renderer>().sortingOrder = 509;
			}
		}
		base.CGet<GameObject>("Return").SetActive(false);
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x00098D98 File Offset: 0x00096F98
	private bool IsCurrentLocatedInIndustryBlock()
	{
		MapBlockData block = SingletonObject.getInstance<WorldMapModel>().GetBlockData(SingletonObject.getInstance<WorldMapModel>().CurrentLocation);
		return WorldMapModel.IsSettlementBlock(MapBlock.Instance.GetItem(block.TemplateId));
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x00098DD4 File Offset: 0x00096FD4
	private bool IsCurrentLocatedInTaiwucun()
	{
		MapBlockData block = SingletonObject.getInstance<WorldMapModel>().GetBlockData(SingletonObject.getInstance<WorldMapModel>().CurrentLocation);
		return block.BlockSubType == EMapBlockSubType.TaiwuCun;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x00098E04 File Offset: 0x00097004
	private void Update()
	{
		bool flag = !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			CanvasGroup canvasGroup = base.CGet<CanvasGroup>("JiaoPoolMouseEffect");
			canvasGroup.alpha = 1f;
			UIParticle uiParticle = canvasGroup.GetComponent<UIParticle>();
			this._mouseStayRect = null;
			this.RefreshMouseStayRect();
			bool hasMove = this._mouseLastPos != Vector3.zero && this._mouseLastPos != Input.mousePosition;
			this._mouseLastPos = Input.mousePosition;
			bool flag2 = this._mouseStayRect == null || hasMove;
			if (flag2)
			{
				Tweener appearTweener = this._appearTweener;
				if (appearTweener != null)
				{
					appearTweener.Rewind(true);
				}
				uiParticle.scale = 0f;
			}
			else
			{
				bool flag3 = uiParticle.scale != 0f;
				if (!flag3)
				{
					Transform parent = null;
					for (int i = 0; i < this._mouseStayRect.childCount; i++)
					{
						Transform child = this._mouseStayRect.GetChild(i);
						bool activeSelf = child.gameObject.activeSelf;
						if (activeSelf)
						{
							parent = child;
						}
					}
					bool flag4 = null != parent;
					if (flag4)
					{
						Vector2 mouseLocalPos;
						RectTransformUtility.ScreenPointToLocalPointInRectangle(this._mouseStayRect, Input.mousePosition, UIManager.Instance.UiCamera, out mouseLocalPos);
						canvasGroup.transform.SetParent(parent);
						canvasGroup.transform.localPosition = mouseLocalPos;
						if (this._appearTweener == null)
						{
							this._appearTweener = DOVirtual.Float(0f, 1f, 2f, delegate(float value)
							{
								uiParticle.scale = value * 144f;
							}).SetAutoKill(false);
						}
						this._appearTweener.Restart(true, -1f);
					}
				}
			}
		}
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x00098FD8 File Offset: 0x000971D8
	private void ShowHighlightEffect()
	{
		bool flag = this._mouseStayRect == null;
		if (flag)
		{
			this.RefreshMouseStayRect();
		}
		bool flag2 = this._mouseStayRect == null;
		if (!flag2)
		{
			RectTransform effect = base.CGet<RectTransform>("JiaoPoolHighlightEffect");
			effect.gameObject.SetActive(true);
			Transform parent = null;
			for (int i = 0; i < this._mouseStayRect.childCount; i++)
			{
				Transform child = this._mouseStayRect.GetChild(i);
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					parent = child;
				}
			}
			effect.SetParent(parent);
			effect.SetAsFirstSibling();
			effect.localPosition = Vector3.zero;
			effect.anchoredPosition = Vector2.zero;
		}
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x00099094 File Offset: 0x00097294
	private void HideHighlightEffect()
	{
		RectTransform effect = base.CGet<RectTransform>("JiaoPoolHighlightEffect");
		effect.gameObject.SetActive(false);
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x000990BC File Offset: 0x000972BC
	private void RefreshMouseStayRect()
	{
		foreach (Refers refers in this._baseList)
		{
			RectTransform rectTrans = refers.CGet<RectTransform>("Water");
			bool flag = RectTransformUtility.RectangleContainsScreenPoint(rectTrans, Input.mousePosition, UIManager.Instance.UiCamera);
			if (flag)
			{
				this._mouseStayRect = rectTrans;
				break;
			}
		}
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x00099144 File Offset: 0x00097344
	private void RefreshChangeLoongButton()
	{
		ExtraDomainMethod.AsyncCall.GetJiaoEvolutionPageStatus(this, delegate(int offset, RawDataPool dataPool)
		{
			sbyte status = 0;
			Serializer.Deserialize(dataPool, offset, ref status);
			CButtonObsolete enterButton = base.CGet<CButtonObsolete>("ChangeLoong_Btn");
			enterButton.interactable = (status == 0);
			enterButton.GetComponent<DisableStyleRoot>().SetStyleEffect(status != 0, false);
			enterButton.GetComponent<PointerScaleAnim>().enabled = (status == 0);
			base.CGet<GameObject>("Eff_hl_notice").SetActive(status == 0);
			TooltipInvoker tipDisplayer = enterButton.GetComponent<TooltipInvoker>();
			string content = LocalStringManager.Get(LanguageKey.LK_JiaoPool_HualoongTips_Desc);
			bool flag = status == 2;
			if (flag)
			{
				content = content + "\n\n" + LocalStringManager.Get(LanguageKey.LK_JiaoPool_NoAvailable_Jiao);
			}
			bool flag2 = status == 1;
			if (flag2)
			{
				content = content + "\n\n" + LocalStringManager.Get(LanguageKey.LK_JiaoPool_Taiwu_NotInVillage);
			}
			tipDisplayer.PresetParam = new string[]
			{
				LocalStringManager.Get(LanguageKey.LK_JiaoPool_Hualong),
				content
			};
		});
	}

	// Token: 0x04001393 RID: 5011
	private Refers _root;

	// Token: 0x04001394 RID: 5012
	private RectTransform _poolBaseRoot;

	// Token: 0x04001395 RID: 5013
	private Refers _returnRefers;

	// Token: 0x04001396 RID: 5014
	private CanvasGroup _sensitiveWordTipCanvasGroup;

	// Token: 0x04001397 RID: 5015
	private Coroutine _sensitiveWordTipCoroutine;

	// Token: 0x04001398 RID: 5016
	private Tween _sensitiveWordTipTween;

	// Token: 0x04001399 RID: 5017
	[SerializeField]
	private List<GameObject> _roadList = new List<GameObject>();

	// Token: 0x0400139A RID: 5018
	[SerializeField]
	private List<GameObject> _terrainList = new List<GameObject>();

	// Token: 0x0400139B RID: 5019
	private List<Refers> _baseList = new List<Refers>();

	// Token: 0x0400139C RID: 5020
	[SerializeField]
	private List<Refers> _baseUIList;

	// Token: 0x0400139D RID: 5021
	private int _currentOpenJiaoPooCount;

	// Token: 0x0400139E RID: 5022
	private List<JiaoPool> _allJiaoPoolData;

	// Token: 0x0400139F RID: 5023
	private List<JiaoPoolRecordList> _poolRecordList = new List<JiaoPoolRecordList>();

	// Token: 0x040013A0 RID: 5024
	private List<ItemDisplayData> itemDisplayDataList = new List<ItemDisplayData>();

	// Token: 0x040013A1 RID: 5025
	private sbyte _landFormType;

	// Token: 0x040013A2 RID: 5026
	private bool _isRenderEnd;

	// Token: 0x040013A3 RID: 5027
	private bool _isChangeName;

	// Token: 0x040013A4 RID: 5028
	private bool _isOpenHosting;

	// Token: 0x040013A5 RID: 5029
	[SerializeField]
	private float animaTime;

	// Token: 0x040013A6 RID: 5030
	private Dictionary<int, List<int>> _poolTurningPointConfig = new Dictionary<int, List<int>>
	{
		{
			0,
			new List<int>
			{
				0,
				3
			}
		},
		{
			1,
			new List<int>
			{
				1,
				4
			}
		},
		{
			2,
			new List<int>
			{
				2
			}
		},
		{
			3,
			new List<int>
			{
				5,
				8
			}
		},
		{
			4,
			new List<int>
			{
				6,
				9
			}
		},
		{
			5,
			new List<int>
			{
				7
			}
		},
		{
			6,
			new List<int>
			{
				10
			}
		},
		{
			7,
			new List<int>
			{
				11
			}
		},
		{
			8,
			new List<int>()
		}
	};

	// Token: 0x040013A7 RID: 5031
	private Dictionary<int, List<ValueTuple<float, float>>> _attributeEffectPosList = new Dictionary<int, List<ValueTuple<float, float>>>
	{
		{
			1,
			new List<ValueTuple<float, float>>
			{
				new ValueTuple<float, float>(-228f, 151f),
				new ValueTuple<float, float>(-208f, 141f),
				new ValueTuple<float, float>(-186f, 134f),
				new ValueTuple<float, float>(-160f, 141f),
				new ValueTuple<float, float>(-137f, 151f)
			}
		},
		{
			2,
			new List<ValueTuple<float, float>>
			{
				new ValueTuple<float, float>(-216f, 166f),
				new ValueTuple<float, float>(-198f, 157f),
				new ValueTuple<float, float>(-168f, 150f),
				new ValueTuple<float, float>(-140f, 157f),
				new ValueTuple<float, float>(-116f, 166f)
			}
		},
		{
			3,
			new List<ValueTuple<float, float>>
			{
				new ValueTuple<float, float>(-173f, 175f),
				new ValueTuple<float, float>(-155f, 165f),
				new ValueTuple<float, float>(-126f, 158f),
				new ValueTuple<float, float>(-98f, 165f),
				new ValueTuple<float, float>(-74f, 175f)
			}
		}
	};

	// Token: 0x040013A8 RID: 5032
	private Dictionary<int, int> _poolStyle = new Dictionary<int, int>();

	// Token: 0x040013A9 RID: 5033
	private Coroutine _sortCheckCoroutine;

	// Token: 0x040013AA RID: 5034
	private Tweener _appearTweener;

	// Token: 0x040013AB RID: 5035
	private RectTransform _mouseStayRect;

	// Token: 0x040013AC RID: 5036
	private Vector3 _mouseLastPos;

	// Token: 0x020012F7 RID: 4855
	private enum PoolState
	{
		// Token: 0x04009C29 RID: 39977
		Empty,
		// Token: 0x04009C2A RID: 39978
		Egg,
		// Token: 0x04009C2B RID: 39979
		Minor,
		// Token: 0x04009C2C RID: 39980
		Breeding,
		// Token: 0x04009C2D RID: 39981
		Adult,
		// Token: 0x04009C2E RID: 39982
		BreedingEnd
	}
}
