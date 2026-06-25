using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.SettlementInformation
{
	// Token: 0x02000790 RID: 1936
	public class ViewSettlementInformation : UIBase
	{
		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x06005DD6 RID: 24022 RVA: 0x002B2641 File Offset: 0x002B0841
		private BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06005DD7 RID: 24023 RVA: 0x002B2648 File Offset: 0x002B0848
		public void Awake()
		{
			this.loadingAnimation.gameObject.SetActive(false);
			this._itemPool = new PoolItem("UI_SettlementInformationCharInfoObject", this.charTemplate.gameObject);
			this.mainToggleGroup.Init(-1);
			this.mainToggleGroup.allowSwitchOff = (this.mainToggleGroup.allowUncheck = true);
			this.mainToggleGroup.OnActiveIndexChange += delegate(int newTog, int _)
			{
				switch (newTog)
				{
				case 0:
					this.OnClickTaiwuDropDownButton();
					break;
				case 1:
					this.OnClickSectDropDownButton();
					break;
				case 2:
					this.OnClickTownDropDownButton();
					break;
				}
			};
			this.gradeGroup.Init(-1);
			this.gradeGroup.OnActiveIndexChange += this.OnClickGrade;
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x002B26E7 File Offset: 0x002B08E7
		private void OnDestroy()
		{
			this._itemPool.Destroy();
			this._itemPool = null;
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x002B2700 File Offset: 0x002B0900
		public override void OnInit(ArgumentBox argsBox)
		{
			ViewSettlementInformation.TaiwuCharIdCache = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			model.RequestExtraMapBlockData((from area in model.Areas
			from settlement in area.SettlementInfos
			where settlement.BlockId >= 0
			select new Location(area.GetAreaId(), settlement.BlockId)).ToList<Location>(), true, false);
			this.blockBase.SetActive(false);
			short settlementId;
			bool flag = argsBox != null && argsBox.Get("SettlementId", out settlementId) && settlementId != 0;
			if (flag)
			{
				ViewSettlementInformation.LastOpenSettlementId = (int)settlementId;
			}
			this.searchName.text = "";
			this.searchName.gameObject.SetActive(false);
			this.btnClose.onClick.AddListener(new UnityAction(this.OnClickClose));
			foreach (object obj in this.townRoot)
			{
				Transform one = (Transform)obj;
				Object.Destroy(one.gameObject);
			}
			foreach (object obj2 in this.sectRoot)
			{
				Transform one2 = (Transform)obj2;
				Object.Destroy(one2.gameObject);
			}
			for (int i = 0; i < 9; i++)
			{
				foreach (object obj3 in this.charRoot.GetChild(i))
				{
					Transform one3 = (Transform)obj3;
					Object.Destroy(one3.gameObject);
				}
			}
			this.remainTime2Refresh.text = LanguageKey.LK_SettlementInformation_Refresh_Paused.Tr();
			this.NeedDataListenerId = true;
			this._curSettlementInDisplay = -1;
			this.wheelScale.GetComponent<PointerTrigger>().SetBindElement(this.Element);
			this.wheelScale.OnScale = new Action<Vector3>(this.OnScale);
			this.OnScale(this.charRoot.localScale);
			this.charRoot.gameObject.SetActive(false);
			this._contentWidth = (this.charRoot.parent as RectTransform).rect.width;
			this.loadingAnimation.gameObject.SetActive(false);
			UnityEvent unityEvent = this.onUiInit;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			bool flag2 = !this._firstEnter;
			if (flag2)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(98);
			}
		}

		// Token: 0x06005DDA RID: 24026 RVA: 0x002B2A64 File Offset: 0x002B0C64
		private void OnClickGrade(int index, int oldIndex)
		{
			bool flag = index == -1;
			if (flag)
			{
				this.charRoot.DOKill(false);
				this.charRoot.DOAnchorPosX(0f, 0.2f, false);
				this.selectBase.gameObject.SetActive(false);
			}
			else
			{
				int charIndex = 8 - index;
				float origin = ((RectTransform)this.charRoot.GetChild(0)).anchoredPosition.y;
				float target = ((RectTransform)this.charRoot.GetChild(charIndex)).anchoredPosition.y;
				this.charRoot.DOKill(false);
				this.charRoot.DOAnchorPos(new Vector2(0f, (origin - target) * this.charRoot.localScale.y), 0.2f, false);
				ConstraintSource source = this.selectBasePosition.GetSource(0);
				bool flag2 = oldIndex >= 0;
				if (flag2)
				{
					this.gradeGroup.Get(oldIndex).GetComponent<SettlementInformationGradeInfo>().SetCharContainerExpandStatus(false);
				}
				SettlementInformationGradeInfo layer = this.gradeGroup.Get(index).GetComponent<SettlementInformationGradeInfo>();
				layer.SetCharContainerExpandStatus(true);
				source.sourceTransform = layer.charContainer;
				this.selectBasePosition.SetSource(0, source);
				this.selectBase.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005DDB RID: 24027 RVA: 0x002B2BAE File Offset: 0x002B0DAE
		private void OnListenerIdReady()
		{
			TaiwuDomainMethod.AsyncCall.GetAllVisitedSettlements(this, new AsyncMethodCallbackDelegate(this.HandleSettlementData));
		}

		// Token: 0x06005DDC RID: 24028 RVA: 0x002B2BC4 File Offset: 0x002B0DC4
		private void OnClickTaiwuDropDownButton()
		{
			bool flag = this.mainToggleGroup.GetActiveIndex() != 0;
			if (flag)
			{
				this.mainToggleGroup.Set(0, false);
			}
			else
			{
				SettlementDisplayData settlementDisplayData = this._visitedTownList.Find((SettlementDisplayData town) => town.OrgTemplateId == 16);
				this.OnClickSettlement(settlementDisplayData.SettlementId, settlementDisplayData);
			}
		}

		// Token: 0x06005DDD RID: 24029 RVA: 0x002B2C30 File Offset: 0x002B0E30
		private void OnClickSectDropDownButton()
		{
			bool flag = this.mainToggleGroup.GetActiveIndex() != 1;
			if (flag)
			{
				this.mainToggleGroup.Set(1, false);
			}
			else
			{
				bool skipAutoSelectSettlement = this._skipAutoSelectSettlement;
				if (!skipAutoSelectSettlement)
				{
					SettlementDisplayData settlementDisplayData = this._visitedSectList[0];
					this.OnClickSettlement(settlementDisplayData.SettlementId, settlementDisplayData);
				}
			}
		}

		// Token: 0x06005DDE RID: 24030 RVA: 0x002B2C8C File Offset: 0x002B0E8C
		private void OnClickTownDropDownButton()
		{
			bool flag = this.mainToggleGroup.GetActiveIndex() != 2;
			if (flag)
			{
				this.mainToggleGroup.Set(2, false);
			}
			else
			{
				bool skipAutoSelectSettlement = this._skipAutoSelectSettlement;
				if (!skipAutoSelectSettlement)
				{
					this.SelectFirstNonTaiwuSettlement();
				}
			}
		}

		// Token: 0x06005DDF RID: 24031 RVA: 0x002B2CD4 File Offset: 0x002B0ED4
		private void SelectFirstNonTaiwuSettlement()
		{
			bool flag = this._visitedTownList.Count == 1;
			if (flag)
			{
				SettlementDisplayData town3 = this._visitedTownList[0];
				this.OnClickSettlement(town3.SettlementId, town3);
			}
			else
			{
				HashSet<short> visitedAreas = new HashSet<short>(from town in this._visitedTownList
				select town.AreaTemplateId);
				HashSet<sbyte> states = new HashSet<sbyte>(from area in visitedAreas
				select MapArea.Instance[area].StateID);
				using (HashSet<sbyte>.Enumerator enumerator = states.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						sbyte state = enumerator.Current;
						IEnumerable<short> areasInState = from area in visitedAreas
						where MapArea.Instance[area].StateID == state
						select area;
						using (IEnumerator<short> enumerator2 = areasInState.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								short area = enumerator2.Current;
								IEnumerable<SettlementDisplayData> townsInArea = from town in this._visitedTownList
								where town.AreaTemplateId == area
								select town;
								foreach (SettlementDisplayData town2 in townsInArea)
								{
									bool flag2 = town2.OrgTemplateId != 16;
									if (flag2)
									{
										this.OnClickSettlement(town2.SettlementId, town2);
										return;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x002B2EA0 File Offset: 0x002B10A0
		private void Update()
		{
			bool flag = this._curSettlementInDisplay >= 0;
			if (flag)
			{
				this._AutoUpdateTime -= Time.deltaTime;
				bool flag2 = this._AutoUpdateTime < 0f;
				if (flag2)
				{
					this.OnSetScrollOffset(Vector2.zero);
					this._AutoUpdateTime = 0.5f;
				}
			}
		}

		// Token: 0x06005DE1 RID: 24033 RVA: 0x002B2EFC File Offset: 0x002B10FC
		private void LateUpdate()
		{
			bool flag = this._setScrollOffsetDelay > 0f;
			if (flag)
			{
				this._setScrollOffsetDelay -= Time.deltaTime;
			}
			bool flag2 = this._delayedSetScrollOffset != null;
			if (flag2)
			{
				ValueTuple<Vector2, float, float> i = this._delayedSetScrollOffset.Value;
				i.Item2 -= Time.deltaTime;
				bool flag3 = i.Item2 < 0f;
				if (flag3)
				{
					this.OnSetScrollOffset(i.Item1);
					i.Item3 -= Time.deltaTime;
					bool flag4 = i.Item3 < 0f;
					if (flag4)
					{
						this._delayedSetScrollOffset = null;
					}
				}
				this._delayedSetScrollOffset = new ValueTuple<Vector2, float, float>?(i);
			}
			bool flag5 = this._searchInformationChanged != null;
			if (flag5)
			{
				ValueTuple<float, float> j = this._searchInformationChanged.Value;
				j.Item2 -= Time.deltaTime;
				bool flag6 = j.Item2 < 0f;
				if (flag6)
				{
					this.charRoot.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
					LayoutRebuilder.ForceRebuildLayoutImmediate(this.charRoot);
					this._searchInformationChanged = null;
				}
				this._searchInformationChanged = new ValueTuple<float, float>?(j);
			}
			bool activeSelf = this.selectBase.gameObject.activeSelf;
			if (activeSelf)
			{
				this.selectBase.anchorMin = new Vector2(0.5f, 0.5f - this.charRoot.localScale.y / 2f);
				this.selectBase.anchorMax = new Vector2(0.5f, 0.5f + this.charRoot.localScale.y / 2f);
			}
		}

		// Token: 0x06005DE2 RID: 24034 RVA: 0x002B30B0 File Offset: 0x002B12B0
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ShowCombatSkillTree"))
			{
				if (!(a == "BtnAreaScroll"))
				{
					if (!(a == "BtnLaw"))
					{
						if (a == "BtnWanted")
						{
							UIElement.SettlementBounty.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SettlementId", (short)this._curSettlementInDisplay).Set("NotDisplayButtonImprison", true));
							UIManager.Instance.MaskUI(UIElement.SettlementBounty);
						}
					}
					else
					{
						SettlementDisplayData settlementData = this._visitedSettlements.Find((SettlementDisplayData s) => s.SettlementId == this._curSettlementInDisplay);
						UIElement.SectLaw.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("IsSect", Organization.Instance[settlementData.OrgTemplateId].IsSect).Set("StateTemplateId", MapArea.Instance[settlementData.AreaTemplateId].StateID));
						UIManager.Instance.ShowUI(UIElement.SectLaw, true);
					}
				}
				else
				{
					SettlementDisplayData settlementData2 = this._visitedSettlements.Find((SettlementDisplayData s) => s.SettlementId == this._curSettlementInDisplay);
					UIElement.AreaStoryScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("orgTemplateId", settlementData2.OrgTemplateId).Set("prosper", this.Model.AreaIsTaskStatus((int)settlementData2.AreaTemplateId, 1)));
					UIManager.Instance.ShowUI(UIElement.AreaStoryScroll, true);
				}
			}
			else if (-1 != this._curSettlementInDisplay)
			{
				SettlementDisplayData settlementData3 = default(SettlementDisplayData);
				bool meetFlag = false;
				for (int i = 0; i < this._visitedSettlements.Count; i++)
				{
					bool flag = this._visitedSettlements[i].SettlementId == this._curSettlementInDisplay;
					if (flag)
					{
						meetFlag = true;
						settlementData3 = this._visitedSettlements[i];
						break;
					}
				}
				bool flag2 = !meetFlag;
				if (flag2)
				{
					throw new Exception(string.Format("Can not find settlement with id:{0}", this._curSettlementInDisplay));
				}
				OrganizationItem config = Organization.Instance[settlementData3.OrgTemplateId];
				bool flag3 = !config.IsSect;
				if (!flag3)
				{
					UIElement.CombatSkillTree.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SectTemplateId", settlementData3.OrgTemplateId));
					UIManager.Instance.MaskUI(UIElement.CombatSkillTree);
				}
			}
		}

		// Token: 0x06005DE3 RID: 24035 RVA: 0x002B3324 File Offset: 0x002B1524
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			this.mainToggleGroup.DeSelectWithoutNotify();
			base.QuickHide();
		}

		// Token: 0x06005DE4 RID: 24036 RVA: 0x002B334C File Offset: 0x002B154C
		private void HandleSettlementData(int offset, RawDataPool dataPool)
		{
			ViewSettlementInformation.<>c__DisplayClass61_0 CS$<>8__locals1 = new ViewSettlementInformation.<>c__DisplayClass61_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._charViewportHeight < 0;
			if (flag)
			{
				this._charViewportHeight = Mathf.RoundToInt(this.charRoot.GetComponentInParent<RectTransform>().rect.height);
			}
			this._visitedSettlements.Clear();
			this._settleBtnDic.Clear();
			Serializer.Deserialize(dataPool, offset, ref this._visitedSettlements);
			this._visitedSettlements.RemoveAll((SettlementDisplayData a) => a.OrgTemplateId == 0);
			this._visitedSectList.Clear();
			this._visitedSectList.AddRange(from a in this._visitedSettlements
			where Organization.Instance[a.OrgTemplateId].IsSect
			select a);
			this._visitedTownList.Clear();
			this._visitedTownList.AddRange(from a in this._visitedSettlements
			where !CS$<>8__locals1.<>4__this._visitedSectList.Contains(a)
			select a);
			CS$<>8__locals1.mapModel = SingletonObject.getInstance<WorldMapModel>();
			this._visitedTownList.RemoveAll((SettlementDisplayData s) => s.AreaTemplateId == CS$<>8__locals1.mapModel.BrokenPerformAreaSettlementData.Item3 && s.OrgTemplateId == 38);
			this.sectButton.SetActive(this._visitedSectList.Count > 0);
			this.townButton.SetActive(this._visitedTownList.Any((SettlementDisplayData item) => item.OrgTemplateId != 16));
			foreach (object obj3 in this.sectRoot)
			{
				Object.Destroy((GameObject)obj3);
			}
			foreach (object obj2 in this.townRoot)
			{
				Object.Destroy((GameObject)obj2);
			}
			using (List<SettlementDisplayData>.Enumerator enumerator3 = this._visitedSectList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					ViewSettlementInformation.<>c__DisplayClass61_1 CS$<>8__locals2 = new ViewSettlementInformation.<>c__DisplayClass61_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.one = enumerator3.Current;
					SettlementHolder obj = Object.Instantiate<SettlementHolder>(this.sectTemplate, this.sectRoot, false);
					obj.gameObject.SetActive(true);
					obj.SetData(CS$<>8__locals2.one);
					obj.toggle.onValueChanged.ResetListener(delegate(bool isOn)
					{
						bool flag7 = isOn && CS$<>8__locals2.CS$<>8__locals1.<>4__this._curSettlementInDisplay != obj.SettlementId;
						if (flag7)
						{
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.OnClickSettlement(CS$<>8__locals2.one.SettlementId, CS$<>8__locals2.one);
						}
						else
						{
							bool flag8 = CS$<>8__locals2.CS$<>8__locals1.<>4__this._curSettlementInDisplay == obj.SettlementId;
							if (flag8)
							{
								obj.toggle.isOn = true;
							}
						}
					});
					this._settleBtnDic[CS$<>8__locals2.one.SettlementId] = obj.transform;
				}
			}
			HashSet<short> areas = new HashSet<short>(from a in this._visitedTownList
			select a.AreaTemplateId);
			HashSet<sbyte> states = new HashSet<sbyte>(from a in areas
			select MapArea.Instance[a].StateID);
			using (HashSet<sbyte>.Enumerator enumerator4 = states.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					ViewSettlementInformation.<>c__DisplayClass61_3 CS$<>8__locals4 = new ViewSettlementInformation.<>c__DisplayClass61_3();
					CS$<>8__locals4.CS$<>8__locals3 = CS$<>8__locals1;
					CS$<>8__locals4.state = enumerator4.Current;
					IEnumerable<short> areasInState = from a in areas
					where MapArea.Instance[a].StateID == CS$<>8__locals4.state
					select a;
					using (IEnumerator<short> enumerator5 = areasInState.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							ViewSettlementInformation.<>c__DisplayClass61_4 CS$<>8__locals5 = new ViewSettlementInformation.<>c__DisplayClass61_4();
							CS$<>8__locals5.CS$<>8__locals4 = CS$<>8__locals4;
							CS$<>8__locals5.area = enumerator5.Current;
							CityHolder areaObj = null;
							IEnumerable<SettlementDisplayData> townsInArea = from a in this._visitedTownList
							where a.AreaTemplateId == CS$<>8__locals5.area
							select a;
							using (IEnumerator<SettlementDisplayData> enumerator6 = townsInArea.GetEnumerator())
							{
								while (enumerator6.MoveNext())
								{
									ViewSettlementInformation.<>c__DisplayClass61_5 CS$<>8__locals6 = new ViewSettlementInformation.<>c__DisplayClass61_5();
									CS$<>8__locals6.CS$<>8__locals5 = CS$<>8__locals5;
									CS$<>8__locals6.town = enumerator6.Current;
									bool flag2 = CS$<>8__locals6.town.OrgTemplateId == 16;
									if (!flag2)
									{
										bool flag3 = areaObj == null;
										SettlementHolder townObj;
										if (flag3)
										{
											CityHolder tmpAreaObj = Object.Instantiate<CityHolder>(this.cityHolderTemplate, this.townRoot, false);
											tmpAreaObj.SetLocation(CS$<>8__locals6.CS$<>8__locals5.CS$<>8__locals4.state, CS$<>8__locals6.CS$<>8__locals5.area);
											townObj = tmpAreaObj.AddSettlement(CS$<>8__locals6.town);
											bool flag4 = CS$<>8__locals6.town.OrgTemplateId != 16;
											if (flag4)
											{
												areaObj = tmpAreaObj;
											}
										}
										else
										{
											townObj = areaObj.AddSettlement(CS$<>8__locals6.town);
										}
										townObj.toggle.onValueChanged.ResetListener(delegate(bool isOn)
										{
											bool flag7 = isOn && CS$<>8__locals6.CS$<>8__locals5.CS$<>8__locals4.CS$<>8__locals3.<>4__this._curSettlementInDisplay != CS$<>8__locals6.town.SettlementId;
											if (flag7)
											{
												CS$<>8__locals6.CS$<>8__locals5.CS$<>8__locals4.CS$<>8__locals3.<>4__this.OnClickSettlement(CS$<>8__locals6.town.SettlementId, CS$<>8__locals6.town);
											}
											else
											{
												bool flag8 = CS$<>8__locals6.CS$<>8__locals5.CS$<>8__locals4.CS$<>8__locals3.<>4__this._curSettlementInDisplay == CS$<>8__locals6.town.SettlementId;
												if (flag8)
												{
													townObj.toggle.isOn = true;
												}
											}
										});
										this._settleBtnDic[CS$<>8__locals6.town.SettlementId] = townObj.transform;
									}
								}
							}
						}
					}
				}
			}
			this.townRoot.parent.GetComponentsInParent<CScrollRect>(true)[0].ScrollTo(Vector2.zero, 0.3f);
			this.sectRoot.parent.GetComponentsInParent<CScrollRect>(true)[0].ScrollTo(Vector2.zero, 0.3f);
			this.mainHolderBg.texture = this.normal;
			bool flag5 = ViewSettlementInformation.LastOpenSettlementId >= 0;
			if (flag5)
			{
				for (int i = 0; i < this._visitedSettlements.Count; i++)
				{
					SettlementDisplayData settlement = this._visitedSettlements[i];
					bool flag6 = settlement.SettlementId == ViewSettlementInformation.LastOpenSettlementId;
					if (flag6)
					{
						this.OnClickSettlement(ViewSettlementInformation.LastOpenSettlementId, settlement);
						break;
					}
				}
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x002B3A48 File Offset: 0x002B1C48
		private void OnClickSettlement(int id, SettlementDisplayData settlementDisplayData = default(SettlementDisplayData))
		{
			this.gradeGroup.Set(8, false);
			bool flag = this._curSettlementInDisplay == id;
			if (!flag)
			{
				this._curSettlementInDisplay = id;
				foreach (CityHolder city in this.townRoot.GetComponentsInChildren<CityHolder>(true))
				{
					city.SwitchOn(id);
				}
				foreach (SettlementHolder settlement in this.sectRoot.GetComponentsInChildren<SettlementHolder>(true))
				{
					settlement.SwitchOn(id);
				}
				this.searchName.interactable = false;
				sbyte status = Organization.Instance[settlementDisplayData.OrgTemplateId].IsSect ? this.Model.GetAreaTaskStatus((int)settlementDisplayData.AreaTemplateId) : 0;
				bool finished = status != 0;
				CRawImage crawImage = this.mainHolderBg;
				if (!true)
				{
				}
				Texture texture;
				if (status != 1)
				{
					if (status != 2)
					{
						texture = this.normal;
					}
					else
					{
						texture = this.decay;
					}
				}
				else
				{
					texture = this.bloom;
				}
				if (!true)
				{
				}
				crawImage.texture = texture;
				bool flag2 = settlementDisplayData.SettlementId > 0 && Organization.Instance[settlementDisplayData.OrgTemplateId].IsSect;
				if (flag2)
				{
					OrganizationDomainMethod.AsyncCall.GetOrganizationCombatSkillsDisplayData(this, settlementDisplayData.OrgTemplateId, delegate(int offset, RawDataPool dataPool)
					{
						OrganizationCombatSkillsDisplayData data = new OrganizationCombatSkillsDisplayData();
						Serializer.Deserialize(dataPool, offset, ref data);
						this.sectSupportText.SetText(LanguageKey.LK_Building_SectSupport.TrFormat((int)(data.ApprovingRate / 10)), true);
						this.sectSkill.SetActive(true);
						this.sectBounty.SetActive(true);
						this.sectSupportBase.SetActive(true);
						this.sectEnding.SetActive(finished);
					});
				}
				else
				{
					this.sectSkill.SetActive(false);
					this.sectBounty.SetActive(false);
					this.sectSupportBase.SetActive(false);
					this.sectEnding.SetActive(false);
				}
				WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
				string sprite2 = "";
				foreach (MapAreaData area in model.Areas)
				{
					bool flag3 = area.SettlementInfos == null;
					if (!flag3)
					{
						foreach (SettlementInfo settlement2 in area.SettlementInfos)
						{
							bool flag4 = (int)settlement2.SettlementId == settlementDisplayData.SettlementId;
							if (flag4)
							{
								MapBlockData curr = model.GetBlockData(new Location(area.GetAreaId(), settlement2.BlockId));
								MapBlockData root = (curr.RootBlockId < 0) ? null : model.GetBlockData(new Location(area.GetAreaId(), curr.RootBlockId));
								sprite2 = MapBlockView.GetMapBlockSpriteName(curr, root);
								bool flag5 = !string.IsNullOrEmpty(sprite2);
								if (!flag5)
								{
									sprite2 = MapBlockView.GetMapBlockSpriteName(curr, curr);
									break;
								}
							}
						}
						bool flag6 = !string.IsNullOrEmpty(sprite2);
						if (flag6)
						{
							break;
						}
					}
				}
				bool flag7 = !string.IsNullOrEmpty(sprite2);
				if (flag7)
				{
					this.blockGraph.SetSprite(sprite2, false, null);
					bool flag8 = !this.blockGraph.enabled;
					if (flag8)
					{
						MapAtlasInfo.Instance.GetSprite(sprite2, delegate(Sprite sprite)
						{
							this.blockGraph.enabled = (sprite != null);
							this.blockGraph.sprite = sprite;
						});
					}
				}
				this.blockName.text = ((settlementDisplayData.RandomNameId != -1) ? LocalTownNames.Instance.TownNameCore[(int)settlementDisplayData.RandomNameId].Name : Organization.Instance[settlementDisplayData.OrgTemplateId].Name);
				this.blockBase.SetActive(true);
				ViewSettlementInformation.LastOpenSettlementId = id;
				OrganizationDomainMethod.AsyncCall.GetSettlementMembers(this, (short)id, new AsyncMethodCallbackDelegate(this.HandleSettlementMembers));
				bool flag9 = (int)model.GetTaiwuVillageSettlementId() == id;
				if (flag9)
				{
					TaiwuDomainMethod.AsyncCall.GetAllVillagerRoleDisplayData(this, new AsyncMethodCallbackDelegate(this.HandleTaiwuVillagerRoleData));
				}
				this._skipAutoSelectSettlement = true;
				bool flag10 = settlementDisplayData.OrgTemplateId == 16;
				if (flag10)
				{
					this.mainToggleGroup.Set(0, false);
				}
				else
				{
					sbyte orgTemplateId = settlementDisplayData.OrgTemplateId;
					bool flag11 = orgTemplateId >= 1 && orgTemplateId <= 15;
					if (flag11)
					{
						this.mainToggleGroup.Set(1, false);
					}
					else
					{
						this.mainToggleGroup.Set(2, false);
					}
				}
				this._skipAutoSelectSettlement = false;
			}
		}

		// Token: 0x06005DE6 RID: 24038 RVA: 0x002B3E41 File Offset: 0x002B2041
		private IEnumerator HandleSettlementMembersProcess(List<CharacterDisplayData> chars)
		{
			this.loadingAnimation.gameObject.SetActive(true);
			this.charRoot.gameObject.SetActive(false);
			Stopwatch watch = new Stopwatch();
			WaitForEndOfFrame wait = new WaitForEndOfFrame();
			int splitInterval = 16;
			watch.Restart();
			foreach (CharacterDisplayData one in chars)
			{
				sbyte grade = one.OrgInfo.Grade;
				OrganizationItem orgCfg = Organization.Instance[one.OrgInfo.OrgTemplateId];
				string gradeName = OrganizationMember.Instance[orgCfg.Members[(int)grade]].GradeName;
				Transform panel = this.charRoot.GetChild((int)(8 - grade));
				GameObject charObj = this._itemPool.GetObject();
				SettlementChar charToggle = charObj.GetComponent<SettlementChar>();
				charToggle.Unknown = true;
				charObj.SetActive(true);
				charObj.transform.SetParent(panel, false);
				charToggle.SettlementId = this._curSettlementInDisplay;
				charToggle.Set(one);
				bool flag = watch.ElapsedMilliseconds > (long)splitInterval;
				if (flag)
				{
					yield return wait;
					watch.Restart();
				}
				orgCfg = null;
				panel = null;
				charObj = null;
				charToggle = null;
				one = null;
			}
			List<CharacterDisplayData>.Enumerator enumerator = default(List<CharacterDisplayData>.Enumerator);
			this.searchName.interactable = true;
			this.SearchByName();
			this.loadingAnimation.gameObject.SetActive(false);
			this.charRoot.gameObject.SetActive(true);
			this.OnScale(this.charRoot.localScale);
			this.OnSetScrollOffset(Vector2.zero);
			this._charInfoEnteringCoroutine = null;
			yield break;
			yield break;
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x002B3E58 File Offset: 0x002B2058
		private void SearchByName()
		{
			this.searchName.SetTextWithoutNotify("");
			this.searchName.onValueChanged.ResetListener(delegate(string inputValue)
			{
				this.UpdateDisplayDataBySearch(this.searchName);
			});
			this.searchName.onEndEdit.ResetListener(delegate(string inputValue)
			{
				this.UpdateDisplayDataBySearch(this.searchName);
			});
			bool flag = !this.searchName.gameObject.activeSelf;
			if (flag)
			{
				this.searchName.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005DE8 RID: 24040 RVA: 0x002B3EDC File Offset: 0x002B20DC
		private void UpdateDisplayDataBySearch(TMP_InputField searchName)
		{
			string inputValue = searchName.text;
			CommonUtils.FixToShowAbleString(ref inputValue, searchName.textComponent.font);
			inputValue = inputValue.Replace(" ", string.Empty);
			bool flag = !string.IsNullOrEmpty(inputValue);
			if (flag)
			{
				inputValue = inputValue.Substring(0, Mathf.Min(inputValue.Length, searchName.characterLimit - 1));
			}
			searchName.SetTextWithoutNotify(inputValue);
			int focus = -1;
			for (int i = 0; i < this.charRoot.childCount; i++)
			{
				Transform panel = this.charRoot.GetChild(i);
				for (int j = 0; j < panel.childCount; j++)
				{
					SettlementChar settlementChar = panel.GetChild(j).GetComponent<SettlementChar>();
					bool flag2 = settlementChar.SettlementId != this._curSettlementInDisplay;
					if (!flag2)
					{
						bool active = inputValue.IsNullOrEmpty() || settlementChar.CharName.Contains(inputValue);
						settlementChar.gameObject.SetActive(active);
						bool flag3 = active;
						if (flag3)
						{
							settlementChar.Refresh();
							bool flag4 = focus == -1;
							if (flag4)
							{
								focus = i;
							}
						}
					}
				}
			}
			bool flag5 = focus != -1;
			if (flag5)
			{
				this.gradeGroup.Set(8 - focus, false);
			}
			this._searchInformationChanged = new ValueTuple<float, float>?(new ValueTuple<float, float>(Time.deltaTime, 0.2f));
		}

		// Token: 0x06005DE9 RID: 24041 RVA: 0x002B4044 File Offset: 0x002B2244
		private void HandleSettlementMembers(int offset, RawDataPool dataPool)
		{
			this.charRoot.gameObject.SetActive(false);
			bool flag = this._charInfoEnteringCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._charInfoEnteringCoroutine);
				this._charInfoEnteringCoroutine = null;
				this.searchName.interactable = true;
			}
			this.charRoot.gameObject.SetActive(true);
			List<CharacterDisplayData> chars = null;
			Serializer.Deserialize(dataPool, offset, ref chars);
			for (int i = 0; i < 9; i++)
			{
				foreach (object obj in this.charRoot.GetChild(i))
				{
					Transform one = (Transform)obj;
					this._itemPool.DestroyObject(one.gameObject);
				}
			}
			sbyte b;
			this._singleRowCharCnt = ((chars == null) ? 0 : (from a in Enumerable.Range(0, 8)
			select chars.Count((CharacterDisplayData b) => (int)b.OrgInfo.Grade == a)).Max());
			this.charRoot.sizeDelta = this.charRoot.sizeDelta.SetX(Mathf.Max(this._contentWidth / this.charRoot.localScale.x, (float)(this._singleRowCharCnt * 240 + 200)));
			bool flag2 = chars == null;
			if (!flag2)
			{
				base.StartCoroutine(this._charInfoEnteringCoroutine = this.HandleSettlementMembersProcess(chars));
				SettlementDisplayData settlementData = this._visitedSettlements.First((SettlementDisplayData a) => a.SettlementId == this._curSettlementInDisplay);
				OrganizationItem configData = Organization.Instance[settlementData.OrgTemplateId];
				int duration = settlementData.InfluencePowerUpdateDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
				bool flag3 = duration >= 0 && configData.InfluencePowerUpdateInterval > 0 && !settlementData.IsInfluencePowerUpdatePaused;
				if (flag3)
				{
					this.remainTime2Refresh.text = LanguageKey.LK_SettlementInformation_Refresh_Duration.TrFormat(duration);
				}
				else
				{
					this.remainTime2Refresh.text = LanguageKey.LK_SettlementInformation_Refresh_Paused.Tr();
				}
				Dictionary<sbyte, int> gradeGroupDict = chars.GroupBy((CharacterDisplayData c) => c.OrgInfo.Grade, (sbyte grade, IEnumerable<CharacterDisplayData> list) => new
				{
					grade = grade,
					count = list.Count<CharacterDisplayData>()
				}).ToDictionary(g => g.grade, g => g.count);
				sbyte status = this.Model.GetAreaTaskStatus((int)settlementData.AreaTemplateId);
				bool isUp = status == 1;
				bool isDown = status == 2;
				sbyte grade2 = (sbyte)configData.Members.Length;
				for (;;)
				{
					b = grade2;
					grade2 = b - 1;
					if (b <= 0)
					{
						break;
					}
					int curCount;
					gradeGroupDict.TryGetValue(grade2, out curCount);
					this.gradeBar.GetChild((int)grade2).GetComponent<SettlementInformationGradeInfo>().Set(configData, (short)settlementData.OrgTemplateId, grade2, curCount, isUp, isDown);
				}
			}
		}

		// Token: 0x06005DEA RID: 24042 RVA: 0x002B4388 File Offset: 0x002B2588
		private void HandleTaiwuVillagerRoleData(int offset, RawDataPool dataPool)
		{
			List<VillagerRoleManageDisplayData> roleManageDisplayList = new List<VillagerRoleManageDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref roleManageDisplayList);
			RectTransform grades = this.gradeBar;
			SettlementDisplayData settlementData = this._visitedSettlements.First((SettlementDisplayData a) => a.SettlementId == this._curSettlementInDisplay);
			OrganizationItem configData = Organization.Instance[settlementData.OrgTemplateId];
			sbyte i = (sbyte)configData.Members.Length;
			for (;;)
			{
				sbyte b = i;
				i = b - 1;
				if (b <= 0)
				{
					break;
				}
				SettlementInformationGradeInfo grade = grades.GetChild((int)i).GetComponent<SettlementInformationGradeInfo>();
				int count = -1;
				bool flag = VillagerRoleUtils.CanGetRoleIdFromOrgMemberId(configData.Members[(int)i]);
				if (flag)
				{
					short roleTemplateId = VillagerRoleUtils.GetRoleIdFromOrgMemberId(configData.Members[(int)i]);
					List<int> characterIds = roleManageDisplayList[(int)roleTemplateId].CharacterIds;
					count = ((characterIds != null) ? characterIds.Count : grade.curCount);
				}
				grade.SetTaiwu(configData, i, count);
			}
		}

		// Token: 0x06005DEB RID: 24043 RVA: 0x002B445C File Offset: 0x002B265C
		public void OnSetScrollOffset(Vector2 offset)
		{
			bool flag = this._setScrollOffsetDelay > 0f;
			if (flag)
			{
				this._delayedSetScrollOffset = new ValueTuple<Vector2, float, float>?(new ValueTuple<Vector2, float, float>(offset, 0.03f, 1f));
			}
			else
			{
				List<float> positions = new List<float>();
				RectTransform grades = this.gradeBar;
				float containerHeight = grades.rect.height;
				float height = 0f;
				float spacing = 8f;
				float[] cumRevHeight = (from x in Enumerable.Range(0, grades.childCount + 1)
				select (x == 0) ? 0f : (height += ((RectTransform)grades.GetChild(grades.childCount - x).transform).rect.height + spacing)).ToArray<float>();
				height = 0f;
				int i = 0;
				int len = grades.childCount;
				while (i < len)
				{
					Transform grade = grades.GetChild(i);
					RectTransform gradeTransform = (RectTransform)grade.transform;
					float scaledGroupHeight = gradeTransform.rect.height * this.charRoot.localScale.x;
					gradeTransform.position = gradeTransform.position.SetY(((RectTransform)this.charRoot.GetChild(8 - i)).position.y - 2f * gradeTransform.lossyScale.y);
					gradeTransform.anchoredPosition = new Vector2(0f, Mathf.Clamp(gradeTransform.anchoredPosition.y + scaledGroupHeight * 0.5f, -containerHeight + gradeTransform.rect.height * 0.5f + height + this.gradePaddingBottom, -gradeTransform.rect.height * 0.5f - cumRevHeight[grades.childCount - i - 1] - this.gradePaddingTop));
					height += gradeTransform.rect.height + spacing;
					positions.Add(gradeTransform.anchoredPosition.y);
					i++;
				}
				ValueTuple<float, int> min = new ValueTuple<float, int>(float.MaxValue, -1);
				int j = 0;
				int len2 = positions.Count;
				while (j < len2)
				{
					float delta = Mathf.Abs(positions[j] - -containerHeight * 0.5f);
					bool flag2 = delta < min.Item1;
					if (flag2)
					{
						min.Item1 = delta;
						min.Item2 = j;
					}
					j++;
				}
				this.gradeGroup.SetWithoutNotify(min.Item2);
			}
		}

		// Token: 0x06005DEC RID: 24044 RVA: 0x002B4704 File Offset: 0x002B2904
		public void OnScale(Vector3 scale)
		{
			ViewSettlementInformation.<>c__DisplayClass76_0 CS$<>8__locals1 = new ViewSettlementInformation.<>c__DisplayClass76_0();
			CS$<>8__locals1.<>4__this = this;
			this.selectBasePosition.enabled = false;
			this._setScrollOffsetDelay = 0.1f;
			VerticalLayoutGroup layout = this.charRoot.GetComponent<VerticalLayoutGroup>();
			RectOffset padding = layout.padding;
			padding.bottom = (padding.top = (int)((float)this._charViewportHeight / Mathf.Max(scale.y, this.wheelScale.Min.y) / 2f) - 92);
			layout.padding = padding;
			this.charRoot.sizeDelta = this.charRoot.sizeDelta.SetX(Mathf.Max(this._contentWidth / scale.x, 0f));
			ViewSettlementInformation.<>c__DisplayClass76_0 CS$<>8__locals2 = CS$<>8__locals1;
			int num = this._positionEnableVersion + 1;
			this._positionEnableVersion = num;
			CS$<>8__locals2.version = num;
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				bool flag = CS$<>8__locals1.version == CS$<>8__locals1.<>4__this._positionEnableVersion;
				if (flag)
				{
					CS$<>8__locals1.<>4__this.selectBasePosition.enabled = true;
				}
			});
		}

		// Token: 0x06005DED RID: 24045 RVA: 0x002B47F4 File Offset: 0x002B29F4
		public void OnClickClose()
		{
			bool flag = this._charInfoEnteringCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._charInfoEnteringCoroutine);
				this._charInfoEnteringCoroutine = null;
			}
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x040040AA RID: 16554
		public static int TaiwuCharIdCache;

		// Token: 0x040040AB RID: 16555
		[SerializeField]
		private TMP_Text sectSupportText;

		// Token: 0x040040AC RID: 16556
		[SerializeField]
		private TMP_Text blockName;

		// Token: 0x040040AD RID: 16557
		[SerializeField]
		private GameObject sectSkill;

		// Token: 0x040040AE RID: 16558
		[SerializeField]
		private GameObject sectBounty;

		// Token: 0x040040AF RID: 16559
		[SerializeField]
		private GameObject sectEnding;

		// Token: 0x040040B0 RID: 16560
		[SerializeField]
		private GameObject sectSupportBase;

		// Token: 0x040040B1 RID: 16561
		[SerializeField]
		private GameObject blockBase;

		// Token: 0x040040B2 RID: 16562
		[SerializeField]
		private CityHolder cityHolderTemplate;

		// Token: 0x040040B3 RID: 16563
		[SerializeField]
		private SettlementHolder sectTemplate;

		// Token: 0x040040B4 RID: 16564
		[SerializeField]
		private CImage blockGraph;

		// Token: 0x040040B5 RID: 16565
		[SerializeField]
		private CButton btnClose;

		// Token: 0x040040B6 RID: 16566
		[SerializeField]
		private SettlementChar charTemplate;

		// Token: 0x040040B7 RID: 16567
		[SerializeField]
		private TMP_InputField searchName;

		// Token: 0x040040B8 RID: 16568
		[SerializeField]
		private float gradePaddingTop = 24f;

		// Token: 0x040040B9 RID: 16569
		[SerializeField]
		private float gradePaddingBottom = 24f;

		// Token: 0x040040BA RID: 16570
		[SerializeField]
		private TextMeshProUGUI remainTime2Refresh;

		// Token: 0x040040BB RID: 16571
		[SerializeField]
		private RectTransform gradeBar;

		// Token: 0x040040BC RID: 16572
		[SerializeField]
		private RectTransform charRoot;

		// Token: 0x040040BD RID: 16573
		[SerializeField]
		private MouseWheelScale wheelScale;

		// Token: 0x040040BE RID: 16574
		[SerializeField]
		private UnityEvent onUiInit;

		// Token: 0x040040BF RID: 16575
		[SerializeField]
		private LoadingAnimation loadingAnimation;

		// Token: 0x040040C0 RID: 16576
		[SerializeField]
		private RectTransform townRoot;

		// Token: 0x040040C1 RID: 16577
		[SerializeField]
		private RectTransform sectRoot;

		// Token: 0x040040C2 RID: 16578
		[SerializeField]
		private CToggleGroup mainToggleGroup;

		// Token: 0x040040C3 RID: 16579
		[SerializeField]
		private CToggleGroup gradeGroup;

		// Token: 0x040040C4 RID: 16580
		[SerializeField]
		private CRawImage mainHolderBg;

		// Token: 0x040040C5 RID: 16581
		[SerializeField]
		private Texture normal;

		// Token: 0x040040C6 RID: 16582
		[SerializeField]
		private Texture bloom;

		// Token: 0x040040C7 RID: 16583
		[SerializeField]
		private Texture decay;

		// Token: 0x040040C8 RID: 16584
		private List<SettlementDisplayData> _visitedSettlements = new List<SettlementDisplayData>();

		// Token: 0x040040C9 RID: 16585
		private readonly List<SettlementDisplayData> _visitedTownList = new List<SettlementDisplayData>();

		// Token: 0x040040CA RID: 16586
		private readonly List<SettlementDisplayData> _visitedSectList = new List<SettlementDisplayData>();

		// Token: 0x040040CB RID: 16587
		private int _charViewportHeight = -1;

		// Token: 0x040040CC RID: 16588
		private int _curSettlementInDisplay;

		// Token: 0x040040CD RID: 16589
		private int _singleRowCharCnt;

		// Token: 0x040040CE RID: 16590
		private float _contentWidth;

		// Token: 0x040040CF RID: 16591
		public static int LastOpenSettlementId = -1;

		// Token: 0x040040D0 RID: 16592
		private Dictionary<int, Transform> _settleBtnDic = new Dictionary<int, Transform>();

		// Token: 0x040040D1 RID: 16593
		private const string _charInfoObjectKey = "UI_SettlementInformationCharInfoObject";

		// Token: 0x040040D2 RID: 16594
		private IEnumerator _charInfoEnteringCoroutine;

		// Token: 0x040040D3 RID: 16595
		private PoolItem _itemPool;

		// Token: 0x040040D4 RID: 16596
		private bool _firstEnter;

		// Token: 0x040040D5 RID: 16597
		private float _AutoUpdateTime;

		// Token: 0x040040D6 RID: 16598
		[SerializeField]
		private GameObject sectButton;

		// Token: 0x040040D7 RID: 16599
		[SerializeField]
		private GameObject townButton;

		// Token: 0x040040D8 RID: 16600
		private bool _skipAutoSelectSettlement;

		// Token: 0x040040D9 RID: 16601
		[TupleElementNames(new string[]
		{
			"RefreshTime",
			"DelayTime"
		})]
		private ValueTuple<float, float>? _searchInformationChanged;

		// Token: 0x040040DA RID: 16602
		private float _setScrollOffsetDelay;

		// Token: 0x040040DB RID: 16603
		[TupleElementNames(new string[]
		{
			"Offset",
			"Delay",
			"Duration"
		})]
		private ValueTuple<Vector2, float, float>? _delayedSetScrollOffset;

		// Token: 0x040040DC RID: 16604
		[SerializeField]
		private RectTransform selectBase;

		// Token: 0x040040DD RID: 16605
		[SerializeField]
		private PositionConstraint selectBasePosition;

		// Token: 0x040040DE RID: 16606
		private int _positionEnableVersion;
	}
}
