using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.EventWindow;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A39 RID: 2617
	public class EventWindowCharacterListPanel : MonoBehaviour
	{
		// Token: 0x17000E28 RID: 3624
		// (get) Token: 0x0600812D RID: 33069 RVA: 0x003C21E1 File Offset: 0x003C03E1
		// (set) Token: 0x0600812E RID: 33070 RVA: 0x003C21E9 File Offset: 0x003C03E9
		public List<int> SelectedCharIdList { get; private set; } = new List<int>();

		// Token: 0x0600812F RID: 33071 RVA: 0x003C21F4 File Offset: 0x003C03F4
		private void Awake()
		{
			this.btnDeselectAll.ClearAndAddListener(delegate
			{
				this.SelectedCharIdList.Clear();
				this.ShowSelectPage();
			});
			this.btnOpenSelectUI.ClearAndAddListener(delegate
			{
				this.BeginSelectCharacter();
			});
			this.btnCharacterListSelectNew.ClearAndAddListener(delegate
			{
				this.BeginSelectCharacter();
			});
		}

		// Token: 0x06008130 RID: 33072 RVA: 0x003C224A File Offset: 0x003C044A
		private void BeginSelectCharacter()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(null, this.CanSelectCharIdList, delegate(int offset, RawDataPool pool)
			{
				List<CharacterDisplayDataForGeneralScrollList> displayData = new List<CharacterDisplayDataForGeneralScrollList>();
				Serializer.Deserialize(pool, offset, ref displayData);
				List<ISelectCharacterData> selectList = new List<ISelectCharacterData>();
				bool flag = selectList != null;
				if (flag)
				{
					foreach (CharacterDisplayDataForGeneralScrollList item in displayData)
					{
						selectList.Add(new BasicSelectCharacterDataAdapter(item));
					}
				}
				CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
				config.SelectionMode = ESelectCharacterSelectionMode.Single;
				config.InteractionMode = ESelectCharacterInteractionMode.Instant;
				config.FilterMenuIds = new List<ESelectCharacterFilterMenuId>
				{
					ESelectCharacterFilterMenuId.Gender,
					ESelectCharacterFilterMenuId.BehaviorType,
					ESelectCharacterFilterMenuId.Relation,
					ESelectCharacterFilterMenuId.Organization,
					ESelectCharacterFilterMenuId.Sect,
					ESelectCharacterFilterMenuId.AdoreRelation,
					ESelectCharacterFilterMenuId.EnemyRelation,
					ESelectCharacterFilterMenuId.WorkStatus,
					ESelectCharacterFilterMenuId.RoleArrangementWork,
					ESelectCharacterFilterMenuId.Identity
				};
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				args.SetObject("SelectCharacterConfig", config);
				args.SetObject("SelectCharacterDataList", selectList);
				args.SetObject("SelectCharacterCallback", new SelectCharacterCallback(this.OnSelectCharacter));
				args.Set("ShowInFront", true);
				Action onOpenSelectWindow = this._onOpenSelectWindow;
				if (onOpenSelectWindow != null)
				{
					onOpenSelectWindow();
				}
				UIElement.SelectChar.SetOnInitArgs(args);
				UIManager.Instance.MaskUI(UIElement.SelectChar);
			});
		}

		// Token: 0x06008131 RID: 33073 RVA: 0x003C2268 File Offset: 0x003C0468
		private void OnSelectCharacter(List<int> selectedCharacterIds)
		{
			bool flag = selectedCharacterIds != null;
			if (flag)
			{
				this.SelectedCharIdList.AddRange(selectedCharacterIds);
			}
			this.SelectedCharIdList.Distinct<int>();
			this.ShowInfomationPage();
		}

		// Token: 0x06008132 RID: 33074 RVA: 0x003C229E File Offset: 0x003C049E
		private void ShowInfomationPage()
		{
			this.selectPage.gameObject.SetActive(false);
			this.infomationPage.gameObject.SetActive(true);
			this.RefreshAmountAndDeselectButton();
			this.RefreshCharacterScroll();
		}

		// Token: 0x06008133 RID: 33075 RVA: 0x003C22D4 File Offset: 0x003C04D4
		private void RefreshCharacterScroll()
		{
			this.btnCharacterListSelectNewHolder.transform.parent = base.transform;
			CommonUtils.PrepareEnoughChildren(this.characterLayout, this.characterTemplate.gameObject, this.SelectedCharIdList.Count, null);
			this.btnCharacterListSelectNewHolder.transform.parent = this.characterLayout;
			for (int i = 0; i < this.SelectedCharIdList.Count; i++)
			{
				this.OnCharacterItemListRender(i, this.characterLayout.GetChild(i).gameObject);
			}
		}

		// Token: 0x06008134 RID: 33076 RVA: 0x003C2370 File Offset: 0x003C0570
		public void RefreshSelectCharacter(EventSelectCharacterData selectCharacterData, CButton confirm, bool autoSelect, Action onOpenSelectWindow)
		{
			this._onOpenSelectWindow = onOpenSelectWindow;
			this._eDisplayType = EventWindowCharacterListPanel.EDisplayType.SelectCharacter;
			this._confirmButton = confirm;
			this._selectCharacterData = selectCharacterData;
			this.CanSelectCharIdList.Clear();
			this._characterDisplayDataDic.Clear();
			foreach (CharacterSelectFilter filter in selectCharacterData.FilterList)
			{
				bool flag = filter.AvailableCharactersDisplayDataList == null;
				if (!flag)
				{
					foreach (CharacterDisplayData characterDisplayData in filter.AvailableCharactersDisplayDataList)
					{
						bool flag2 = !this._characterDisplayDataDic.ContainsKey(characterDisplayData.CharacterId);
						if (flag2)
						{
							this._characterDisplayDataDic.Add(characterDisplayData.CharacterId, characterDisplayData);
							this.CanSelectCharIdList.Add(characterDisplayData.CharacterId);
						}
					}
				}
			}
			this._confirmButton.interactable = false;
			this.ShowSelectPage();
			if (autoSelect)
			{
				this.BeginSelectCharacter();
			}
		}

		// Token: 0x06008135 RID: 33077 RVA: 0x003C24B4 File Offset: 0x003C06B4
		public void RefreshShowApprove(SelectApprovedTaiwu selectApprovedTaiwu, CButton confirmButton)
		{
			this._selectApprovedTaiwu = selectApprovedTaiwu;
			this._eDisplayType = EventWindowCharacterListPanel.EDisplayType.ShowApprovedTaiwu;
			this._confirmButton = confirmButton;
			this.CanSelectCharIdList.Clear();
			this._isEnough = false;
			foreach (KeyValuePair<int, short> pair in selectApprovedTaiwu.CharacterApprovingRate)
			{
				bool flag = !this.CanSelectCharIdList.Contains(pair.Key);
				if (flag)
				{
					this.CanSelectCharIdList.Add(pair.Key);
				}
			}
			this._confirmButton.interactable = false;
		}

		// Token: 0x06008136 RID: 33078 RVA: 0x003C2568 File Offset: 0x003C0768
		private void ShowSelectPage()
		{
			this.selectPage.gameObject.SetActive(true);
			this.infomationPage.gameObject.SetActive(false);
			this.RefreshAmountAndDeselectButton();
		}

		// Token: 0x06008137 RID: 33079 RVA: 0x003C2598 File Offset: 0x003C0798
		private void RefreshAmountAndDeselectButton()
		{
			this.txtSelectedAmount.text = string.Format("{0}", this.SelectedCharIdList.Count);
			this.btnDeselectAll.gameObject.SetActive(this.SelectedCharIdList.Count > 0);
			bool flag = this._confirmButton != null;
			if (flag)
			{
				bool flag2 = this._eDisplayType == EventWindowCharacterListPanel.EDisplayType.SelectCharacter;
				if (flag2)
				{
					this._confirmButton.interactable = this._selectCharacterData.IsAvailableSelectResult(this.SelectedCharIdList);
				}
				else
				{
					bool flag3 = this._eDisplayType == EventWindowCharacterListPanel.EDisplayType.ShowApprovedTaiwu;
					if (flag3)
					{
						this._confirmButton.interactable = this.SelectApprovedTaiwuResult(this.SelectedCharIdList);
					}
				}
				this.btnCharacterListSelectNewHolder.gameObject.SetActive(!this._confirmButton.interactable);
			}
		}

		// Token: 0x06008138 RID: 33080 RVA: 0x003C2674 File Offset: 0x003C0874
		private void OnCharacterItemListRender(int index, GameObject go)
		{
			bool flag = this._eDisplayType == EventWindowCharacterListPanel.EDisplayType.SelectCharacter;
			if (flag)
			{
				this.OnCharacterItemRender(index, go);
			}
			else
			{
				bool flag2 = this._eDisplayType == EventWindowCharacterListPanel.EDisplayType.ShowApprovedTaiwu;
				if (flag2)
				{
					this.OnCharacterApprovedTaiwuItemRender(index, go);
				}
			}
		}

		// Token: 0x06008139 RID: 33081 RVA: 0x003C26B4 File Offset: 0x003C08B4
		private void OnCharacterItemRender(int index, GameObject go)
		{
			EventWindowCharacterListItem characterItem = go.GetComponent<EventWindowCharacterListItem>();
			int charId = this.SelectedCharIdList[index];
			CharacterDisplayData characterDisplayData = this._characterDisplayDataDic[charId];
			characterItem.Set(characterDisplayData, delegate
			{
				this.OpenCharacterMenu(charId);
			}, delegate
			{
				this.RemoveSelectedCharacter(charId);
			});
			CharacterItem config = Character.Instance[characterDisplayData.TemplateId];
			TooltipInvoker mouseTipDisplayer = characterItem.mouseTip;
			bool flag = this.CanSelectCharacter(config);
			if (flag)
			{
				mouseTipDisplayer.Type = TipType.Character;
				mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("charId", charId);
			}
			else
			{
				mouseTipDisplayer.Type = TipType.SingleDesc;
				bool flag2 = config.SpecialTemmateType == ECharacterSpecialTemmateType.BeastCarrier;
				if (flag2)
				{
					mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", LocalStringManager.Get(LanguageKey.LK_EventWindow_SpecialTeammate_BeastCarrier_Tip));
				}
				else
				{
					bool flag3 = config.SpecialTemmateType == ECharacterSpecialTemmateType.GearMate;
					if (flag3)
					{
						mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", LocalStringManager.Get(LanguageKey.LK_EventWindow_SpecialTeammate_GearMate_Tip));
					}
				}
			}
		}

		// Token: 0x0600813A RID: 33082 RVA: 0x003C27DC File Offset: 0x003C09DC
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x0600813B RID: 33083 RVA: 0x003C282C File Offset: 0x003C0A2C
		private void RemoveSelectedCharacter(int charId)
		{
			this.SelectedCharIdList.Remove(charId);
			bool flag = this.SelectedCharIdList.Count == 0;
			if (flag)
			{
				this.ShowSelectPage();
			}
			this.RefreshCharacterScroll();
		}

		// Token: 0x0600813C RID: 33084 RVA: 0x003C286C File Offset: 0x003C0A6C
		private void OnCharacterApprovedTaiwuItemRender(int index, GameObject go)
		{
			EventWindowCharacterListItem characterItem = go.GetComponent<EventWindowCharacterListItem>();
			int charId = this.SelectedCharIdList[index];
			Action <>9__1;
			Action <>9__2;
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool dataPool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(dataPool, offset, ref displayData);
				EventWindowCharacterListItem characterItem = characterItem;
				CharacterDisplayData data = displayData;
				short approvingRate = this.GetApprovingRate(charId);
				Action actionOnSearch;
				if ((actionOnSearch = <>9__1) == null)
				{
					actionOnSearch = (<>9__1 = delegate()
					{
						this.OpenCharacterMenu(charId);
					});
				}
				Action actionOnDelete;
				if ((actionOnDelete = <>9__2) == null)
				{
					actionOnDelete = (<>9__2 = delegate()
					{
						this.RemoveSelectedCharacter(charId);
					});
				}
				characterItem.SetApprove(data, approvingRate, actionOnSearch, actionOnDelete);
			});
			TooltipInvoker mouseTipDisplayer = characterItem.mouseTip;
			mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("charId", charId);
		}

		// Token: 0x0600813D RID: 33085 RVA: 0x003C28E8 File Offset: 0x003C0AE8
		private bool SelectApprovedTaiwuResult(List<int> selectedCharIdList)
		{
			return this.GetTotalApprovingRate(selectedCharIdList) >= this._selectApprovedTaiwu.TargetApprovingRate;
		}

		// Token: 0x0600813E RID: 33086 RVA: 0x003C2914 File Offset: 0x003C0B14
		private short GetTotalApprovingRate(List<int> selectedCharIdList)
		{
			short totalApprovingRate = 0;
			for (int i = 0; i < selectedCharIdList.Count; i++)
			{
				int charId = selectedCharIdList[i];
				short approvingRate = this.GetApprovingRate(charId);
				totalApprovingRate += approvingRate;
			}
			return totalApprovingRate;
		}

		// Token: 0x0600813F RID: 33087 RVA: 0x003C295C File Offset: 0x003C0B5C
		private short GetApprovingRate(int charId)
		{
			short approvingRate;
			this._selectApprovedTaiwu.CharacterApprovingRate.TryGetValue(charId, out approvingRate);
			return approvingRate;
		}

		// Token: 0x06008140 RID: 33088 RVA: 0x003C2984 File Offset: 0x003C0B84
		private bool CanSelectCharacter(CharacterItem config)
		{
			return config.SpecialTemmateType == ECharacterSpecialTemmateType.Invalid;
		}

		// Token: 0x06008141 RID: 33089 RVA: 0x003C299F File Offset: 0x003C0B9F
		public void Clear()
		{
			this.SelectedCharIdList.Clear();
		}

		// Token: 0x06008142 RID: 33090 RVA: 0x003C29AE File Offset: 0x003C0BAE
		private void OnDisable()
		{
			this.SelectedCharIdList.Clear();
			this.ShowSelectPage();
		}

		// Token: 0x06008143 RID: 33091 RVA: 0x003C29C4 File Offset: 0x003C0BC4
		private void SetSelectedApprovedTaiwu(string selectedCount)
		{
			ConchShipCursor.Instance.SetSelectApprovedTaiwuCur(selectedCount);
		}

		// Token: 0x0400629D RID: 25245
		[Header("Pages")]
		[SerializeField]
		private GameObject selectPage;

		// Token: 0x0400629E RID: 25246
		[SerializeField]
		private GameObject infomationPage;

		// Token: 0x0400629F RID: 25247
		[Header("SelectPage")]
		[SerializeField]
		private CButton btnOpenSelectUI;

		// Token: 0x040062A0 RID: 25248
		[SerializeField]
		private TextMeshProUGUI txtSelectedAmount;

		// Token: 0x040062A1 RID: 25249
		[Header("InfoPage")]
		[SerializeField]
		private CButton btnDeselectAll;

		// Token: 0x040062A2 RID: 25250
		[SerializeField]
		private RectTransform characterLayout;

		// Token: 0x040062A3 RID: 25251
		[SerializeField]
		private EventWindowCharacterListItem characterTemplate;

		// Token: 0x040062A4 RID: 25252
		[SerializeField]
		private CButton btnCharacterListSelectNew;

		// Token: 0x040062A5 RID: 25253
		[SerializeField]
		private RectTransform btnCharacterListSelectNewHolder;

		// Token: 0x040062A6 RID: 25254
		private Action _refreshConfirmButtonTips;

		// Token: 0x040062A7 RID: 25255
		private CButton _confirmButton;

		// Token: 0x040062A8 RID: 25256
		private Action _onOpenSelectWindow;

		// Token: 0x040062AA RID: 25258
		public readonly List<int> CanSelectCharIdList = new List<int>();

		// Token: 0x040062AB RID: 25259
		private Dictionary<int, CharacterDisplayData> _characterDisplayDataDic = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x040062AC RID: 25260
		private EventWindowCharacterListPanel.EDisplayType _eDisplayType;

		// Token: 0x040062AD RID: 25261
		private SelectApprovedTaiwu _selectApprovedTaiwu;

		// Token: 0x040062AE RID: 25262
		private EventSelectCharacterData _selectCharacterData;

		// Token: 0x040062AF RID: 25263
		private bool _isEnough = false;

		// Token: 0x02001FE7 RID: 8167
		private enum EDisplayType
		{
			// Token: 0x0400CF26 RID: 53030
			SelectCharacter,
			// Token: 0x0400CF27 RID: 53031
			ShowApprovedTaiwu
		}
	}
}
