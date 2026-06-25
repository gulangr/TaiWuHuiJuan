using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.EventWindow;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A3A RID: 2618
	public class EventWindowDirectSelectCharacterListPanel : MonoBehaviour
	{
		// Token: 0x17000E29 RID: 3625
		// (get) Token: 0x06008149 RID: 33097 RVA: 0x003C2B94 File Offset: 0x003C0D94
		// (set) Token: 0x0600814A RID: 33098 RVA: 0x003C2B9C File Offset: 0x003C0D9C
		public List<int> SelectedCharIdList { get; private set; } = new List<int>();

		// Token: 0x0600814B RID: 33099 RVA: 0x003C2BA5 File Offset: 0x003C0DA5
		private void Awake()
		{
		}

		// Token: 0x0600814C RID: 33100 RVA: 0x003C2BA8 File Offset: 0x003C0DA8
		private void RefreshCharacterScroll()
		{
			CommonUtils.PrepareEnoughChildren(this.characterLayout, this.characterTemplate.gameObject, this.CanSelectCharIdList.Count, null);
			for (int i = 0; i < this.CanSelectCharIdList.Count; i++)
			{
				this.OnCharacterApprovedTaiwuItemRender(i, this.characterLayout.GetChild(i).gameObject);
			}
		}

		// Token: 0x0600814D RID: 33101 RVA: 0x003C2C18 File Offset: 0x003C0E18
		private void RefreshAvatarScroll()
		{
			CommonUtils.PrepareEnoughChildren(this.characterLayout, this.characterTemplate.gameObject, this._selectOneAvatarRelatedDataList.Count, null);
			for (int i = 0; i < this._selectOneAvatarRelatedDataList.Count; i++)
			{
				this.OnCharacterAvatarItemRender(i, this.characterLayout.GetChild(i).gameObject);
			}
		}

		// Token: 0x0600814E RID: 33102 RVA: 0x003C2C88 File Offset: 0x003C0E88
		public void RefreshShowApprove(SelectApprovedTaiwu selectApprovedTaiwu, CButton confirmButton)
		{
			this._selectApprovedTaiwu = selectApprovedTaiwu;
			this._confirmButton = confirmButton;
			this.txtSelectedApprovement.gameObject.SetActive(true);
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
			this.RefreshCharacterScroll();
			this.RefreshAmountAndButton();
		}

		// Token: 0x0600814F RID: 33103 RVA: 0x003C2D54 File Offset: 0x003C0F54
		public void RefreshShowAvatar(List<AvatarRelatedData> selectOneAvatarRelatedDataList, CButton confirmButton, Action<int> onSelectAvatar)
		{
			this._onSelectAvatar = onSelectAvatar;
			this._confirmButton = confirmButton;
			this.txtSelectedApprovement.gameObject.SetActive(false);
			this._selectOneAvatarRelatedDataList = selectOneAvatarRelatedDataList;
			this._confirmButton.interactable = false;
			this.RefreshAvatarScroll();
		}

		// Token: 0x06008150 RID: 33104 RVA: 0x003C2D92 File Offset: 0x003C0F92
		private void RefreshAmountAndButton()
		{
			this._confirmButton.interactable = this.SelectApprovedTaiwuResult(this.SelectedCharIdList);
			this.RefreshApproveText();
		}

		// Token: 0x06008151 RID: 33105 RVA: 0x003C2DB4 File Offset: 0x003C0FB4
		private void OpenCharacterMenu(int charId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06008152 RID: 33106 RVA: 0x003C2E04 File Offset: 0x003C1004
		private void OnCharacterApprovedTaiwuItemRender(int index, GameObject go)
		{
			EventWindowDirectSelectCharacterListItem characterItem = go.GetComponent<EventWindowDirectSelectCharacterListItem>();
			int charId = this.CanSelectCharIdList[index];
			int indexCache = index;
			bool selected = this.SelectedCharIdList.Contains(charId);
			uint loadingVersion = characterItem.BeginAvatarLoading();
			Action <>9__1;
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool dataPool)
			{
				EventWindowDirectSelectCharacterListItem characterItem;
				bool flag = !characterItem.IsAvatarLoadingVersionValid(loadingVersion);
				if (!flag)
				{
					CharacterDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					characterItem = characterItem;
					CharacterDisplayData data = displayData;
					short approvingRate = this.GetApprovingRate(charId);
					bool selected = selected;
					Action actionOnClick;
					if ((actionOnClick = <>9__1) == null)
					{
						actionOnClick = (<>9__1 = delegate()
						{
							this.OnSelectChar(charId, indexCache);
						});
					}
					characterItem.SetApprove(data, approvingRate, selected, actionOnClick);
					characterItem.EndAvatarLoading(loadingVersion);
				}
			});
			TooltipInvoker mouseTipDisplayer = characterItem.mouseTip;
			mouseTipDisplayer.enabled = false;
			short selectTotal = this.GetTotalApprovingRate(this.SelectedCharIdList);
			bool approveEnough = selectTotal >= this._selectApprovedTaiwu.TargetApprovingRate;
			characterItem.SetInteractable(selected || !approveEnough);
		}

		// Token: 0x06008153 RID: 33107 RVA: 0x003C2ED4 File Offset: 0x003C10D4
		private void OnCharacterApprovedTaiwuItemRefreshInteractable(int index, GameObject go)
		{
			EventWindowDirectSelectCharacterListItem characterItem = go.GetComponent<EventWindowDirectSelectCharacterListItem>();
			int charId = this.CanSelectCharIdList[index];
			bool selected = this.SelectedCharIdList.Contains(charId);
			short selectTotal = this.GetTotalApprovingRate(this.SelectedCharIdList);
			bool approveEnough = selectTotal >= this._selectApprovedTaiwu.TargetApprovingRate;
			characterItem.SetInteractable(selected || !approveEnough);
		}

		// Token: 0x06008154 RID: 33108 RVA: 0x003C2F38 File Offset: 0x003C1138
		private void OnCharacterAvatarItemRender(int index, GameObject go)
		{
			EventWindowDirectSelectCharacterListItem characterItem = go.GetComponent<EventWindowDirectSelectCharacterListItem>();
			AvatarRelatedData avatarData = this._selectOneAvatarRelatedDataList[index];
			int indexCache = index;
			uint loadingVersion = characterItem.BeginAvatarLoading();
			characterItem.SetAvatar(avatarData, delegate
			{
				this.OnSelectAvatar(indexCache);
			});
			characterItem.EndAvatarLoading(loadingVersion);
			TooltipInvoker mouseTipDisplayer = characterItem.mouseTip;
			mouseTipDisplayer.enabled = false;
		}

		// Token: 0x06008155 RID: 33109 RVA: 0x003C2FA4 File Offset: 0x003C11A4
		private void OnSelectAvatar(int indexCache)
		{
			bool flag = this.SelectedCharIdList.Contains(indexCache);
			if (flag)
			{
				this.SelectedCharIdList.Remove(indexCache);
				Action<int> onSelectAvatar = this._onSelectAvatar;
				if (onSelectAvatar != null)
				{
					onSelectAvatar(-1);
				}
			}
			else
			{
				this.SelectedCharIdList.Add(indexCache);
				Action<int> onSelectAvatar2 = this._onSelectAvatar;
				if (onSelectAvatar2 != null)
				{
					onSelectAvatar2(indexCache);
				}
			}
			this.OnCharacterAvatarItemRender(indexCache, this.characterLayout.GetChild(indexCache).gameObject);
			this._confirmButton.interactable = (this.SelectedCharIdList.Count > 0);
		}

		// Token: 0x06008156 RID: 33110 RVA: 0x003C303C File Offset: 0x003C123C
		private void OnSelectChar(int charId, int index)
		{
			bool flag = this.SelectedCharIdList.Contains(charId);
			if (flag)
			{
				this.SelectedCharIdList.Remove(charId);
			}
			else
			{
				this.SelectedCharIdList.Add(charId);
			}
			this.OnCharacterApprovedTaiwuItemRender(index, this.characterLayout.GetChild(index).gameObject);
			this.RefreshAmountAndButton();
			this.RefreshOnlyInteractable();
		}

		// Token: 0x06008157 RID: 33111 RVA: 0x003C30A4 File Offset: 0x003C12A4
		private void RefreshOnlyInteractable()
		{
			for (int i = 0; i < this.CanSelectCharIdList.Count; i++)
			{
				this.OnCharacterApprovedTaiwuItemRefreshInteractable(i, this.characterLayout.GetChild(i).gameObject);
			}
		}

		// Token: 0x06008158 RID: 33112 RVA: 0x003C30E8 File Offset: 0x003C12E8
		private void RefreshApproveText()
		{
			short selectTotal = this.GetTotalApprovingRate(this.SelectedCharIdList);
			string selectedStr = ((float)selectTotal / 10f).ToString().SetColor((selectTotal >= this._selectApprovedTaiwu.TargetApprovingRate) ? "brightblue" : "brightred");
			this.txtSelectedApprovement.text = LocalStringManager.GetFormat(LanguageKey.Lk_EventWindow_ApprovementCost, selectedStr, (float)this._selectApprovedTaiwu.TargetApprovingRate / 10f);
		}

		// Token: 0x06008159 RID: 33113 RVA: 0x003C3164 File Offset: 0x003C1364
		private bool SelectApprovedTaiwuResult(List<int> selectedCharIdList)
		{
			return this.GetTotalApprovingRate(selectedCharIdList) >= this._selectApprovedTaiwu.TargetApprovingRate;
		}

		// Token: 0x0600815A RID: 33114 RVA: 0x003C3190 File Offset: 0x003C1390
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

		// Token: 0x0600815B RID: 33115 RVA: 0x003C31D8 File Offset: 0x003C13D8
		private short GetApprovingRate(int charId)
		{
			short approvingRate;
			this._selectApprovedTaiwu.CharacterApprovingRate.TryGetValue(charId, out approvingRate);
			return approvingRate;
		}

		// Token: 0x0600815C RID: 33116 RVA: 0x003C31FF File Offset: 0x003C13FF
		public void Clear()
		{
			this.SelectedCharIdList.Clear();
		}

		// Token: 0x0600815D RID: 33117 RVA: 0x003C320E File Offset: 0x003C140E
		private void OnDisable()
		{
			this.SelectedCharIdList.Clear();
		}

		// Token: 0x040062B0 RID: 25264
		[Header("顶部信息")]
		[SerializeField]
		private TextMeshProUGUI txtSelectedApprovement;

		// Token: 0x040062B1 RID: 25265
		[SerializeField]
		private RectTransform characterLayout;

		// Token: 0x040062B2 RID: 25266
		[SerializeField]
		private EventWindowDirectSelectCharacterListItem characterTemplate;

		// Token: 0x040062B3 RID: 25267
		private Action _refreshConfirmButtonTips;

		// Token: 0x040062B4 RID: 25268
		private CButton _confirmButton;

		// Token: 0x040062B5 RID: 25269
		private Action<int> _onSelectAvatar;

		// Token: 0x040062B7 RID: 25271
		public readonly List<int> CanSelectCharIdList = new List<int>();

		// Token: 0x040062B8 RID: 25272
		private Dictionary<int, CharacterDisplayData> _characterDisplayDataDic = new Dictionary<int, CharacterDisplayData>();

		// Token: 0x040062B9 RID: 25273
		private SelectApprovedTaiwu _selectApprovedTaiwu;

		// Token: 0x040062BA RID: 25274
		private List<AvatarRelatedData> _selectOneAvatarRelatedDataList;

		// Token: 0x040062BB RID: 25275
		private bool _isEnough = false;
	}
}
