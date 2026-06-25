using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Jieqing;
using Game.Views.MapBlockCharList;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A3 RID: 2467
	public class JieQingMurderMap : Refers, IMapBlockCharHolder
	{
		// Token: 0x060076C1 RID: 30401 RVA: 0x003751E8 File Offset: 0x003733E8
		private void CharListAwake()
		{
			this.charScroll.UpdateData(0);
			this.charScroll.OnItemRender += this.OnRenderCharNormal;
			this.characterFilter.onSelect.ResetListener(new Action<string>(this.BeginEditSearchName));
			this.characterFilter.onDeselect.ResetListener(new Action<string>(this.EndEdit));
			this.characterFilter.onValueChanged.ResetListener(new Action<string>(this.SetSearchResult));
			this.characterFilter.onEndEdit.ResetListener(new Action<string>(this.SetSearchResult));
			this.characterFilterClearButton.onClick.ResetListener(delegate()
			{
				this.characterFilter.text = "";
			});
		}

		// Token: 0x060076C2 RID: 30402 RVA: 0x003752AC File Offset: 0x003734AC
		private int GetCount(int active)
		{
			return this._displayDataList.Count;
		}

		// Token: 0x060076C3 RID: 30403 RVA: 0x003752BC File Offset: 0x003734BC
		public void ReapplyCharFilter()
		{
			JieQingMurderMap.<>c__DisplayClass22_0 CS$<>8__locals1 = new JieQingMurderMap.<>c__DisplayClass22_0();
			CS$<>8__locals1.<>4__this = this;
			this._searchedCharDisplayDataList.Clear();
			JieqingMurderSortAndFilterController sortAndFilterController = this._sortAndFilterController;
			if (sortAndFilterController != null)
			{
				sortAndFilterController.NotifyDataChanged(this._displayDataList);
			}
			CS$<>8__locals1.searching = this.characterFilter.text;
			bool flag = this._displayDataList.Count > 0;
			if (flag)
			{
				JieQingMurderMap.<>c__DisplayClass22_1 CS$<>8__locals2 = new JieQingMurderMap.<>c__DisplayClass22_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				JieQingMurderMap.<>c__DisplayClass22_1 CS$<>8__locals3 = CS$<>8__locals2;
				JieqingMurderSortAndFilterController sortAndFilterController2 = this._sortAndFilterController;
				Func<CharacterDisplayData, bool> filter;
				if ((filter = ((sortAndFilterController2 != null) ? sortAndFilterController2.GenerateFilter() : null)) == null && (filter = JieQingMurderMap.<>c.<>9__22_0) == null)
				{
					filter = (JieQingMurderMap.<>c.<>9__22_0 = ((CharacterDisplayData _) => true));
				}
				CS$<>8__locals3.filter = filter;
				this._searchedCharDisplayDataList = (from t in this._displayDataList
				where CS$<>8__locals2.filter(t) && (string.IsNullOrWhiteSpace(CS$<>8__locals2.CS$<>8__locals1.searching) || CS$<>8__locals2.CS$<>8__locals1.<>4__this.GetNameStr(t).Contains(CS$<>8__locals2.CS$<>8__locals1.searching))
				select t).ToList<CharacterDisplayData>();
			}
			this.charScroll.SetDataCount(this._searchedCharDisplayDataList.Count);
			JieqingMurderSortAndFilterController sortAndFilterController3 = this._sortAndFilterController;
			if (sortAndFilterController3 != null)
			{
				sortAndFilterController3.AfterFilter(this._searchedCharDisplayDataList);
			}
		}

		// Token: 0x060076C4 RID: 30404 RVA: 0x003753B9 File Offset: 0x003735B9
		private int GetOrg(CharacterDisplayData data)
		{
			return MapBlockChar.GetTextData(data).Item3;
		}

		// Token: 0x060076C5 RID: 30405 RVA: 0x003753C6 File Offset: 0x003735C6
		private int GetOrg(int index)
		{
			return this.GetNameData(index).Item3;
		}

		// Token: 0x060076C6 RID: 30406 RVA: 0x003753D4 File Offset: 0x003735D4
		private string GetNameStr(int index)
		{
			ValueTuple<string, string, int> nameData = this.GetNameData(index);
			string nameText = nameData.Item1;
			if (nameText != null)
			{
				string gradeText = nameData.Item2;
				if (gradeText != null)
				{
					return JieQingMurderMap.HtmlTagRemover.Replace(nameText + "\n" + gradeText, "");
				}
			}
			return "";
		}

		// Token: 0x060076C7 RID: 30407 RVA: 0x00375420 File Offset: 0x00373620
		private string GetNameStr(CharacterDisplayData data)
		{
			ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(data);
			string nameText = textData.Item1;
			if (nameText != null)
			{
				string gradeText = textData.Item2;
				if (gradeText != null)
				{
					return JieQingMurderMap.HtmlTagRemover.Replace(nameText + "\n" + gradeText, "");
				}
			}
			return "";
		}

		// Token: 0x060076C8 RID: 30408 RVA: 0x0037546A File Offset: 0x0037366A
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		private ValueTuple<string, string, int> GetNameData(int index)
		{
			return MapBlockChar.GetTextData(this._displayDataList[index]);
		}

		// Token: 0x060076C9 RID: 30409 RVA: 0x0037547D File Offset: 0x0037367D
		public void RefreshOrg(int _)
		{
			this.ReapplyCharFilter();
		}

		// Token: 0x060076CA RID: 30410 RVA: 0x00375486 File Offset: 0x00373686
		public void RefreshMapState(int _)
		{
			this.ReapplyCharFilter();
		}

		// Token: 0x060076CB RID: 30411 RVA: 0x0037548F File Offset: 0x0037368F
		public void SetSearchResult(string _)
		{
			this.ReapplyCharFilter();
		}

		// Token: 0x060076CC RID: 30412 RVA: 0x00375498 File Offset: 0x00373698
		public void BeginEditSearchName(string _)
		{
			JieQingMurderMap.IsFocusOnSearchInputField = true;
			bool flag = !string.IsNullOrWhiteSpace(this.characterFilter.text);
			if (flag)
			{
				this.ReapplyCharFilter();
			}
		}

		// Token: 0x060076CD RID: 30413 RVA: 0x003754CA File Offset: 0x003736CA
		public void EndEdit(string _)
		{
			this.SetSearchResult(_);
			JieQingMurderMap.IsFocusOnSearchInputField = false;
		}

		// Token: 0x060076CE RID: 30414 RVA: 0x003754DC File Offset: 0x003736DC
		private void OnRenderCharNormal(int index, GameObject obj)
		{
			bool flag = GameApp.Quiting || GameApp.ReadyToQuit;
			if (!flag)
			{
				MapBlockChar component = obj.GetComponent<MapBlockChar>();
				bool canSeeDetail = this._canSeeDetail;
				CharacterDisplayData characterDisplayData = this._searchedCharDisplayDataList.CheckIndex(index) ? this._searchedCharDisplayDataList[index] : null;
				bool flag2 = characterDisplayData == null;
				if (flag2)
				{
				}
				component.Set(this, characterDisplayData, false, false);
				Refers refers = obj.GetComponent<Refers>();
				int point = this._charPointDic[characterDisplayData.CharacterId];
				refers.CGet<TextMeshProUGUI>("Text").text = point.ToString();
				int levelIndex = CommonUtils.GetJieqingSignLevelIndex(point);
				refers.CGet<CImage>("Icon").SetSprite(string.Format("{0}{1}", "ui9_back_sectpopup_12_icon_2_", levelIndex), false, null);
				TooltipInvoker mouseTips = refers.CGet<TooltipInvoker>("JieqingSignTips");
				mouseTips.Type = TipType.JieqingInteractCharTips;
				TooltipInvoker tooltipInvoker = mouseTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				mouseTips.RuntimeParam.Set("charId", characterDisplayData.CharacterId);
				CButton followButton = refers.CGet<CButton>("FollowButton");
				byte creatingType = characterDisplayData.CreatingType;
				GameObject selected = followButton.transform.Find("Selected").gameObject;
				selected.SetActive(characterDisplayData != null && characterDisplayData.IsFollowedByTaiwu);
				bool interactable = !this._isFollowListMax;
				followButton.interactable = interactable;
				followButton.ClearAndAddListener(delegate
				{
					this.OnClickFollow(characterDisplayData);
				});
				TooltipInvoker tipDisplayer = followButton.GetComponent<TooltipInvoker>();
				tipDisplayer.Type = TipType.Simple;
				tooltipInvoker = tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Following_Tips_Title));
				tipDisplayer.RuntimeParam.Set("arg1", LocalStringManager.Get(interactable ? LanguageKey.LK_Following_Tips_Content : LanguageKey.LK_Following_Tips_Content_Disable));
				refers.CGet<CImage>("OrgImage").sprite = this.GetOrganizationIconSprite((int)characterDisplayData.OrgInfo.OrgTemplateId);
			}
		}

		// Token: 0x060076CF RID: 30415 RVA: 0x00375730 File Offset: 0x00373930
		private Sprite GetOrganizationIconSprite(int orgTemplateId)
		{
			OrganizationItem orgCfg = Organization.Instance[orgTemplateId];
			bool flag = orgCfg == null;
			Sprite result;
			if (flag)
			{
				result = this.organizationIconSprite[0];
			}
			else
			{
				bool flag2 = orgCfg.TemplateId == 16;
				if (flag2)
				{
					result = this.organizationIconSprite[1];
				}
				else
				{
					bool flag3 = orgCfg.SettlementType == EOrganizationSettlementType.Village;
					if (flag3)
					{
						switch (orgCfg.Goodness)
						{
						case -1:
							return this.organizationIconSprite[4];
						case 0:
							return this.organizationIconSprite[3];
						case 1:
							return this.organizationIconSprite[2];
						}
					}
					result = this.organizationIconSprite[0];
				}
			}
			return result;
		}

		// Token: 0x060076D0 RID: 30416 RVA: 0x003757D8 File Offset: 0x003739D8
		private void OnClickFollow(CharacterDisplayData characterDisplayData)
		{
			bool flag = characterDisplayData == null;
			if (!flag)
			{
				bool isFollowed = characterDisplayData.IsFollowedByTaiwu;
				bool flag2 = isFollowed;
				if (flag2)
				{
					TaiwuDomainMethod.Call.TaiwuUnfollowNpc(characterDisplayData.CharacterId);
				}
				else
				{
					TaiwuDomainMethod.Call.TaiwuFollowNpc(characterDisplayData.CharacterId);
				}
				characterDisplayData.IsFollowedByTaiwu = !characterDisplayData.IsFollowedByTaiwu;
				this.RefreshFollowNpc();
			}
		}

		// Token: 0x060076D1 RID: 30417 RVA: 0x0037582C File Offset: 0x00373A2C
		private void RefreshFollowNpc()
		{
			TaiwuDomainMethod.AsyncCall.GetIsFollowingNpcListMax(null, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._isFollowListMax);
				this.charScroll.SetDataCount(this._searchedCharDisplayDataList.Count);
			});
		}

		// Token: 0x060076D2 RID: 30418 RVA: 0x00375844 File Offset: 0x00373A44
		public bool CanClick(DisplayType type, int id)
		{
			return true;
		}

		// Token: 0x060076D3 RID: 30419 RVA: 0x00375858 File Offset: 0x00373A58
		public void OnClick(DisplayType type, int id)
		{
			bool clickLocked = this._clickLocked;
			if (!clickLocked)
			{
				this._clickLocked = true;
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, delegate
				{
					this._clickLocked = false;
				});
				foreach (CharacterDisplayData item in this._searchedCharDisplayDataList)
				{
					bool flag = item.CharacterId == id;
					if (flag)
					{
						this.MapFocus(item.Location.AreaId);
						break;
					}
				}
			}
		}

		// Token: 0x17000D57 RID: 3415
		// (get) Token: 0x060076D4 RID: 30420 RVA: 0x003758FC File Offset: 0x00373AFC
		private WorldMapModel _worldMapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x060076D5 RID: 30421 RVA: 0x00375903 File Offset: 0x00373B03
		private void Awake()
		{
			this.Init();
			this.CharListAwake();
			this.InitSortAndFilter();
		}

		// Token: 0x060076D6 RID: 30422 RVA: 0x0037591C File Offset: 0x00373B1C
		private void InitSortAndFilter()
		{
			bool flag = this.sortAndFilter == null;
			if (!flag)
			{
				this._sortAndFilterController = new JieqingMurderSortAndFilterController(this.sortAndFilter);
				this._sortAndFilterController.Init(new Action(this.ReapplyCharFilter), "JieqingMurder");
			}
		}

		// Token: 0x060076D7 RID: 30423 RVA: 0x0037596C File Offset: 0x00373B6C
		private void OnEnable()
		{
			bool dataInited = this._dataInited;
			if (dataInited)
			{
				this.ReapplyCharFilter();
			}
		}

		// Token: 0x060076D8 RID: 30424 RVA: 0x00375990 File Offset: 0x00373B90
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._mapModel = SingletonObject.getInstance<WorldMapModel>();
				bool flag = this.partWorldView2;
				if (flag)
				{
					JieQingAreaMapInterface jieQingAreaMapInterface = this.partWorldView2;
					jieQingAreaMapInterface.OnAreaInit = (Action<JieQingAreaHelper, short>)Delegate.Combine(jieQingAreaMapInterface.OnAreaInit, new Action<JieQingAreaHelper, short>(this.OnAreaInit2));
					this.partWorldView2.OnScale += this.SetMapElementScale;
					this.partWorldView2.InitAreas();
				}
				this._inited = true;
			}
		}

		// Token: 0x060076D9 RID: 30425 RVA: 0x00375A18 File Offset: 0x00373C18
		private void OnAreaInit(Refers areaRefers, short areaId)
		{
			MapAreaItem areaConfig = this._mapModel.Areas[(int)areaId].GetConfig();
			sbyte orgId;
			bool flag = this.CheckAreaSect(areaConfig, out orgId);
			if (flag)
			{
				this.CreateSectEffect(areaRefers.CGet<RectTransform>("EffHolder"), orgId);
			}
		}

		// Token: 0x060076DA RID: 30426 RVA: 0x00375A5C File Offset: 0x00373C5C
		private void OnAreaInit2(JieQingAreaHelper areaHelper, short areaId)
		{
			MapAreaItem areaConfig = this._mapModel.Areas[(int)areaId].GetConfig();
			sbyte orgId;
			bool flag = this.CheckAreaSect(areaConfig, out orgId);
			if (flag)
			{
				this.CreateSectEffect(areaHelper.EffHolder, orgId);
			}
			this.RefreshJieqingSectBuildingEff();
		}

		// Token: 0x060076DB RID: 30427 RVA: 0x00375AA4 File Offset: 0x00373CA4
		private void CreateSectEffect(RectTransform effHolder, sbyte orgId)
		{
			bool flag = !effHolder;
			if (!flag)
			{
				UIParticle eff = Object.Instantiate<UIParticle>(this.sectEffPrefab);
				this._sectEffDic[orgId] = eff;
				eff.transform.SetParent(effHolder);
				eff.transform.localPosition = Vector3.zero;
				eff.transform.localScale = Vector3.one;
				eff.gameObject.SetActive(false);
			}
		}

		// Token: 0x060076DC RID: 30428 RVA: 0x00375B18 File Offset: 0x00373D18
		private bool CheckAreaSect(MapAreaItem areaConfig, out sbyte orgId)
		{
			foreach (sbyte item in areaConfig.OrganizationId)
			{
				bool flag = item >= 1 && item <= 15;
				if (flag)
				{
					orgId = item;
					return true;
				}
			}
			orgId = -1;
			return false;
		}

		// Token: 0x060076DD RID: 30429 RVA: 0x00375B68 File Offset: 0x00373D68
		private void OnDisable()
		{
			GEvent.OnEvent(UiEvents.OnJieqingSignStateRefresh, null);
		}

		// Token: 0x060076DE RID: 30430 RVA: 0x00375B7C File Offset: 0x00373D7C
		private void SetMapElementScale(Vector3 vector)
		{
			JieQingAreaMapInterface jieQingAreaMapInterface = this.partWorldView2;
			if (jieQingAreaMapInterface != null)
			{
				jieQingAreaMapInterface.UpdateFixedScaleItems(vector);
			}
		}

		// Token: 0x060076DF RID: 30431 RVA: 0x00375B94 File Offset: 0x00373D94
		private void RefreshAreaTargetSignAmount()
		{
			bool flag = !this._dataInited;
			if (!flag)
			{
				int signState = 0;
				SingletonObject.getInstance<GlobalSettings>().JieQingMurderSignDisplay = signState;
				SingletonObject.getInstance<GlobalSettings>().SaveSettings();
				Dictionary<int, List<CharacterDisplayData>> filteredAreaTargetDic = new Dictionary<int, List<CharacterDisplayData>>();
				foreach (List<CharacterDisplayData> orgCharacterList in this._characterDisplayDataDic.Values)
				{
					foreach (CharacterDisplayData charData in orgCharacterList)
					{
						bool flag2 = !filteredAreaTargetDic.ContainsKey((int)charData.Location.AreaId);
						if (flag2)
						{
							filteredAreaTargetDic[(int)charData.Location.AreaId] = new List<CharacterDisplayData>();
						}
						filteredAreaTargetDic[(int)charData.Location.AreaId].Add(charData);
					}
				}
				this.UpdateAreaTargetCount(filteredAreaTargetDic);
			}
		}

		// Token: 0x060076E0 RID: 30432 RVA: 0x00375CB4 File Offset: 0x00373EB4
		public void OnInit(ViewJieQingInteract parent, bool initialized)
		{
			this._isShow = true;
			this._parent = parent;
			this.Init();
			this._dataInited = false;
			JieQingAreaMapInterface jieQingAreaMapInterface = this.partWorldView2;
			if (jieQingAreaMapInterface != null)
			{
				jieQingAreaMapInterface.SetPointerVisible(false);
			}
			this.InitSelectableAreas();
			this.RefreshJieqingSectBuildingEff();
		}

		// Token: 0x060076E1 RID: 30433 RVA: 0x00375CF4 File Offset: 0x00373EF4
		public void OnListenerIdReady()
		{
			this.GetAllCharacterData();
		}

		// Token: 0x060076E2 RID: 30434 RVA: 0x00375D00 File Offset: 0x00373F00
		private void RefreshJieqingSectBuildingEff()
		{
			using (Dictionary<sbyte, UIParticle>.Enumerator enumerator = this._sectEffDic.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JieQingMurderMap.<>c__DisplayClass79_0 CS$<>8__locals1 = new JieQingMurderMap.<>c__DisplayClass79_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.item = enumerator.Current;
					UIParticle eff = CS$<>8__locals1.item.Value;
					ExtraDomainMethod.AsyncCall.IsSectBuiltExtraLegacyBuilding(null, CS$<>8__locals1.item.Key, delegate(int offset, RawDataPool dataPool)
					{
						bool qiwenxingtaiBuilt = false;
						Serializer.Deserialize(dataPool, offset, ref qiwenxingtaiBuilt);
						eff.gameObject.SetActive(qiwenxingtaiBuilt);
						JieQingAreaMapInterface jieQingAreaMapInterface = CS$<>8__locals1.<>4__this.partWorldView2;
						Transform effHighlight = (jieQingAreaMapInterface != null) ? jieQingAreaMapInterface.GetStateHighlightEffect((short)CS$<>8__locals1.item.Key) : null;
						bool flag = effHighlight;
						if (flag)
						{
							effHighlight.gameObject.SetActive(qiwenxingtaiBuilt);
						}
					});
				}
			}
		}

		// Token: 0x060076E3 RID: 30435 RVA: 0x00375DAC File Offset: 0x00373FAC
		private void GetAllCharacterData()
		{
			this._characterDisplayDataDic.Clear();
			this._charPointDic.Clear();
			this._displayDataList.Clear();
			this.requested = this._availableSectIds.Length;
			for (int i = 0; i < this._availableSectIds.Length; i++)
			{
				short tempData = this._availableSectIds[i];
				ExtraDomainMethod.AsyncCall.GetSectMembersWorthExtraLegacyPoint(this._parent, (sbyte)tempData, delegate(int offset, RawDataPool pool)
				{
					SectJieqingWorthDisplayData charIdPointDic = null;
					Serializer.Deserialize(pool, offset, ref charIdPointDic);
					this.ProcessCharIdPoint(tempData, ((charIdPointDic != null) ? charIdPointDic.Value : null) ?? new Dictionary<int, int>());
				});
			}
		}

		// Token: 0x060076E4 RID: 30436 RVA: 0x00375E40 File Offset: 0x00374040
		private void ProcessCharIdPoint(short orgTemplateId, Dictionary<int, int> charIdPointDic)
		{
			foreach (KeyValuePair<int, int> item in charIdPointDic)
			{
				this._charPointDic[item.Key] = item.Value;
			}
			List<int> targetIds = charIdPointDic.Keys.ToList<int>();
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this._parent, targetIds, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayData> tempDataList = null;
				Serializer.Deserialize(pool, offset, ref tempDataList);
				bool flag = tempDataList != null;
				if (flag)
				{
					this._characterDisplayDataDic[(int)orgTemplateId] = tempDataList;
					this._displayDataList.AddRange(tempDataList);
				}
				this.requested--;
				bool flag2 = this.requested <= 0;
				if (flag2)
				{
					this.UpdateCharList(true);
				}
			});
		}

		// Token: 0x060076E5 RID: 30437 RVA: 0x00375EE0 File Offset: 0x003740E0
		private void UpdateCharList(bool scrollToTop)
		{
			this._dataInited = true;
			this.ReapplyCharFilter();
			this.RefreshAreaTargetSignAmount();
		}

		// Token: 0x060076E6 RID: 30438 RVA: 0x00375EF8 File Offset: 0x003740F8
		private void MapFocus(short areaId)
		{
			bool flag = areaId < 0;
			if (!flag)
			{
				JieQingAreaMapInterface jieQingAreaMapInterface = this.partWorldView2;
				if (jieQingAreaMapInterface != null)
				{
					jieQingAreaMapInterface.FocusArea(areaId);
				}
			}
		}

		// Token: 0x060076E7 RID: 30439 RVA: 0x00375F23 File Offset: 0x00374123
		private void InitSelectableAreas()
		{
			this.GetAreaDisplayData();
		}

		// Token: 0x060076E8 RID: 30440 RVA: 0x00375F2D File Offset: 0x0037412D
		private void GetAreaDisplayData()
		{
			CommandManager.AddCommandMethodCall(EPriority.CallGetAreaDisplayData, 2, 35, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._areaDisplayData);
				this.UpdateAreaInfo();
			}, null);
		}

		// Token: 0x060076E9 RID: 30441 RVA: 0x00375F48 File Offset: 0x00374148
		private void UpdateAreaInfo()
		{
			bool flag = this.partWorldView2;
			if (flag)
			{
				this.partWorldView2.Refresh(this._areaDisplayData);
			}
			this.RefreshAreaTargetSignAmount();
		}

		// Token: 0x060076EA RID: 30442 RVA: 0x00375F80 File Offset: 0x00374180
		private void UpdateAreaTargetCount(Dictionary<int, List<CharacterDisplayData>> filteredAreaTargetDic)
		{
			for (short areaId = 0; areaId < 135; areaId += 1)
			{
				JieQingAreaMapInterface jieQingAreaMapInterface = this.partWorldView2;
				JieQingAreaHelper areaHelper = (jieQingAreaMapInterface != null) ? jieQingAreaMapInterface.GetAreaHelper(areaId) : null;
				bool flag = areaHelper == null;
				if (!flag)
				{
					List<CharacterDisplayData> helperCharDataList;
					bool flag2 = filteredAreaTargetDic.TryGetValue((int)areaId, out helperCharDataList);
					if (flag2)
					{
						areaHelper.SetMurderSignCount(helperCharDataList.Count);
					}
					else
					{
						areaHelper.HideMurderSign();
					}
				}
			}
		}

		// Token: 0x04005994 RID: 22932
		public static bool IsFocusOnSearchInputField = false;

		// Token: 0x04005995 RID: 22933
		[SerializeField]
		private InfinityScroll charScroll;

		// Token: 0x04005996 RID: 22934
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x04005997 RID: 22935
		[SerializeField]
		private Vector2 originPosition;

		// Token: 0x04005998 RID: 22936
		[SerializeField]
		private Vector2 targetPosition;

		// Token: 0x04005999 RID: 22937
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x0400599A RID: 22938
		[SerializeField]
		private TMP_InputField characterFilter;

		// Token: 0x0400599B RID: 22939
		[SerializeField]
		private CButton characterFilterClearButton;

		// Token: 0x0400599C RID: 22940
		[Header("门派正邪头像框 中立:0-2  正派:3-5  邪派:6-8")]
		[SerializeField]
		private Sprite[] sectGoodnessSprite;

		// Token: 0x0400599D RID: 22941
		[SerializeField]
		private Sprite[] organizationIconSprite;

		// Token: 0x0400599E RID: 22942
		private MapBlockCharacterList _blockData;

		// Token: 0x0400599F RID: 22943
		private JieqingMurderSortAndFilterController _sortAndFilterController;

		// Token: 0x040059A0 RID: 22944
		private const int Special = 0;

		// Token: 0x040059A1 RID: 22945
		private const int Normal = 1;

		// Token: 0x040059A2 RID: 22946
		private const int Infected = 2;

		// Token: 0x040059A3 RID: 22947
		private const int Animal = 3;

		// Token: 0x040059A4 RID: 22948
		private const int Enemy = 4;

		// Token: 0x040059A5 RID: 22949
		private const int RandomEnemy = 5;

		// Token: 0x040059A6 RID: 22950
		private const int Caravan = 6;

		// Token: 0x040059A7 RID: 22951
		private const int Grave = 7;

		// Token: 0x040059A8 RID: 22952
		private static readonly Regex HtmlTagRemover = new Regex("<[^>]*>", RegexOptions.Compiled);

		// Token: 0x040059A9 RID: 22953
		private bool _isFollowListMax = false;

		// Token: 0x040059AA RID: 22954
		private bool _isShow = true;

		// Token: 0x040059AB RID: 22955
		private bool _canInteract;

		// Token: 0x040059AC RID: 22956
		private bool _isHideCharacterSet;

		// Token: 0x040059AD RID: 22957
		private Tweener _tween;

		// Token: 0x040059AE RID: 22958
		private bool _clickLocked;

		// Token: 0x040059AF RID: 22959
		[SerializeField]
		private JieQingAreaMapInterface partWorldView2;

		// Token: 0x040059B0 RID: 22960
		[SerializeField]
		private UIParticle sectEffPrefab;

		// Token: 0x040059B1 RID: 22961
		private const int honourSect = 5;

		// Token: 0x040059B2 RID: 22962
		private const int neutralSect = 5;

		// Token: 0x040059B3 RID: 22963
		private const int dishonourSect = 5;

		// Token: 0x040059B4 RID: 22964
		private short[] _availableSectIds = new short[]
		{
			1,
			2,
			4,
			3,
			5,
			6,
			7,
			9,
			8,
			10,
			11,
			12,
			13,
			14,
			15
		};

		// Token: 0x040059B5 RID: 22965
		private ViewJieQingInteract _parent;

		// Token: 0x040059B6 RID: 22966
		private bool _inited = false;

		// Token: 0x040059B7 RID: 22967
		private bool _canSeeDetail = true;

		// Token: 0x040059B8 RID: 22968
		private const int SectFilterTypeCount = 16;

		// Token: 0x040059B9 RID: 22969
		private bool _dataInited = false;

		// Token: 0x040059BA RID: 22970
		private List<CharacterDisplayData> _searchedCharDisplayDataList = new List<CharacterDisplayData>();

		// Token: 0x040059BB RID: 22971
		private AreaDisplayData[] _areaDisplayData;

		// Token: 0x040059BC RID: 22972
		private Dictionary<int, int> _charPointDic = new Dictionary<int, int>();

		// Token: 0x040059BD RID: 22973
		private List<CharacterDisplayData> _displayDataList = new List<CharacterDisplayData>();

		// Token: 0x040059BE RID: 22974
		private Dictionary<int, List<CharacterDisplayData>> _characterDisplayDataDic = new Dictionary<int, List<CharacterDisplayData>>();

		// Token: 0x040059BF RID: 22975
		private Dictionary<sbyte, UIParticle> _sectEffDic = new Dictionary<sbyte, UIParticle>();

		// Token: 0x040059C0 RID: 22976
		private WorldMapModel _mapModel;

		// Token: 0x040059C1 RID: 22977
		private int requested = 0;
	}
}
