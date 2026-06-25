using System;
using System.Collections.Generic;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class CharacterMenuStackView : MonoBehaviour
{
	// Token: 0x06001CAE RID: 7342 RVA: 0x000C8B90 File Offset: 0x000C6D90
	public void InitConfirmButton(ViewCharacterMenu owner)
	{
		this.Confirm.ClearAndAddListener(delegate
		{
			this.ResetAndSetNotActive(owner);
		});
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x000C8BCA File Offset: 0x000C6DCA
	public void ClearAndSetNotActive()
	{
		this._scrollIndex = 1;
		this._selectIndex = 0;
		this._characterStateList.Clear();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x000C8BF4 File Offset: 0x000C6DF4
	public void ResetAndSetNotActive(ViewCharacterMenu owner)
	{
		bool flag = this._characterStateList.Count > 0;
		if (flag)
		{
			CharacterMenuStackView.StackCharacterState legacyState = this._characterStateList[0];
			ViewCharacterMenuStack.StackCharacterState newState = new ViewCharacterMenuStack.StackCharacterState
			{
				CharacterId = legacyState.CharacterId,
				CharacterIdList = legacyState.CharacterIdList,
				CanOperate = legacyState.CanOperate,
				PageId = legacyState.PageId,
				SubPageId = legacyState.SubPageId,
				Name = legacyState.Name,
				AvatarRefresher = legacyState.AvatarRefresher
			};
			owner.TransitionToStackCharacterState(newState, true);
		}
		this.ClearAndSetNotActive();
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x000C8C98 File Offset: 0x000C6E98
	public void ToCharacterAndActivate(ViewCharacterMenu owner, int characterId, string displayName = null, Action<Game.Components.Avatar.Avatar> avatarRefresher = null)
	{
		int index = this._characterStateList.FindLastIndex((CharacterMenuStackView.StackCharacterState s) => s.CharacterId == characterId);
		bool flag = index > 0;
		if (flag)
		{
			this.SelectIndex(owner, index, true);
		}
		else
		{
			this.PushAndActivate(owner, characterId, displayName, avatarRefresher);
		}
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x000C8CF4 File Offset: 0x000C6EF4
	public void PushAndActivate(ViewCharacterMenu owner, int characterId, string displayName = null, Action<Game.Components.Avatar.Avatar> avatarRefresher = null)
	{
		CharacterMenuStackView.<>c__DisplayClass13_0 CS$<>8__locals1 = new CharacterMenuStackView.<>c__DisplayClass13_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.owner = owner;
		for (int index = this._characterStateList.Count - 1; index > 0; index--)
		{
			bool flag = index > this._selectIndex;
			if (!flag)
			{
				break;
			}
			this._characterStateList.RemoveAt(index);
		}
		bool flag2 = this._characterStateList.Count <= 0;
		if (flag2)
		{
			ViewCharacterMenuStack.StackCharacterState rootState = CS$<>8__locals1.owner.MakeStackCharacterState();
			this._characterStateList.Add(new CharacterMenuStackView.StackCharacterState
			{
				CharacterId = rootState.CharacterId,
				CharacterIdList = rootState.CharacterIdList,
				CanOperate = rootState.CanOperate,
				PageId = rootState.PageId,
				SubPageId = rootState.SubPageId,
				Name = rootState.Name,
				AvatarRefresher = rootState.AvatarRefresher
			});
		}
		ViewCharacterMenuStack.StackCharacterState newState = CS$<>8__locals1.owner.MakeStackCharacterState();
		CS$<>8__locals1.current = new CharacterMenuStackView.StackCharacterState
		{
			CharacterId = characterId,
			Name = displayName,
			AvatarRefresher = avatarRefresher,
			CharacterIdList = newState.CharacterIdList,
			CanOperate = newState.CanOperate,
			PageId = newState.PageId,
			SubPageId = newState.SubPageId
		};
		ViewCharacterMenuDisplayData displayData = CS$<>8__locals1.owner.DisplayData;
		bool isInTaiwuTeam = ((displayData != null) ? displayData.TaiwuTeamCharIds : null) != null && displayData.TaiwuTeamCharIds.Contains(characterId);
		bool isInTaiwuSpecialGroup = ((displayData != null) ? displayData.TaiwuSpecialGroup : null) != null && displayData.TaiwuSpecialGroup.Contains(characterId);
		bool flag3 = isInTaiwuTeam || isInTaiwuSpecialGroup;
		if (flag3)
		{
			List<int> ids = new List<int>();
			bool flag4 = ((displayData != null) ? displayData.TaiwuTeamCharIds : null) != null;
			if (flag4)
			{
				ids.AddRange(displayData.TaiwuTeamCharIds);
			}
			bool flag5 = ((displayData != null) ? displayData.TaiwuSpecialGroup : null) != null;
			if (flag5)
			{
				ids.AddRange(displayData.TaiwuSpecialGroup);
			}
			CS$<>8__locals1.current.CharacterIdList = ids.ToArray();
			CS$<>8__locals1.<PushAndActivate>g__Finish|0();
		}
		else
		{
			UIManager.Instance.ShowUI(UIElement.BlockInteract, true);
			CharacterDomainMethod.AsyncCall.GetGroupSet(CS$<>8__locals1.owner, characterId, delegate(int offset, RawDataPool pool)
			{
				List<int> group = null;
				Serializer.Deserialize(pool, offset, ref group);
				CS$<>8__locals1.current.CharacterIdList = group.ToArray();
				base.<PushAndActivate>g__Finish|0();
				UIManager.Instance.HideUI(UIElement.BlockInteract);
			});
		}
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x000C8F50 File Offset: 0x000C7150
	public void SelectIndex(ViewCharacterMenu owner, int index, bool withRefresh = true)
	{
		this._selectIndex = index;
		CharacterMenuStackView.StackCharacterState current = this._characterStateList[index];
		ViewCharacterMenuStack.StackCharacterState newState = new ViewCharacterMenuStack.StackCharacterState
		{
			CharacterId = current.CharacterId,
			CharacterIdList = current.CharacterIdList,
			CanOperate = current.CanOperate,
			PageId = current.PageId,
			SubPageId = current.SubPageId,
			Name = current.Name,
			AvatarRefresher = current.AvatarRefresher
		};
		owner.TransitionToStackCharacterState(newState, true);
		if (withRefresh)
		{
			this.OnRefreshList(owner);
		}
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x000C8FF0 File Offset: 0x000C71F0
	private void OnRefreshList(ViewCharacterMenu owner)
	{
		int displayCount = this.Elements.childCount;
		for (;;)
		{
			int selectedDisplayIndex = this.GetDisplayIndex(this._selectIndex);
			bool flag = selectedDisplayIndex < 0;
			if (flag)
			{
				this._scrollIndex--;
			}
			else
			{
				bool flag2 = selectedDisplayIndex >= displayCount;
				if (!flag2)
				{
					break;
				}
				this._scrollIndex++;
			}
		}
		bool flag3 = this._characterStateList.Count < displayCount;
		if (flag3)
		{
			this._scrollIndex = 1;
			this.First.interactable = false;
			this.Last.interactable = false;
			this.Prev.interactable = false;
			this.Next.interactable = false;
		}
		else
		{
			int min = 1;
			int max = this._characterStateList.Count - displayCount;
			int lastDisplayIndex = displayCount - 1;
			this.First.interactable = (this.Prev.interactable = (this._scrollIndex > min));
			this.First.ClearAndAddListener(delegate
			{
				this.SelectIndex(owner, 1, false);
				this._scrollIndex = min;
				int selectedDisplayIndex2 = this.GetDisplayIndex(this._selectIndex);
				bool flag5 = this.Elements.childCount.CheckIndex(selectedDisplayIndex2);
				if (flag5)
				{
					this.OnRefreshList(owner);
				}
				else
				{
					this.SelectIndex(owner, this.GetStateIndex(lastDisplayIndex), true);
				}
			});
			this.Prev.ClearAndAddListener(delegate
			{
				this._scrollIndex--;
				int selectedDisplayIndex2 = this.GetDisplayIndex(this._selectIndex);
				bool flag5 = this.Elements.childCount.CheckIndex(selectedDisplayIndex2);
				if (flag5)
				{
					this.OnRefreshList(owner);
				}
				else
				{
					this.SelectIndex(owner, this.GetStateIndex(lastDisplayIndex), true);
				}
			});
			this.Last.interactable = (this.Next.interactable = (this._scrollIndex < max));
			this.Last.ClearAndAddListener(delegate
			{
				this.SelectIndex(owner, this._characterStateList.Count - 1, false);
				this._scrollIndex = max;
				int selectedDisplayIndex2 = this.GetDisplayIndex(this._selectIndex);
				bool flag5 = this.Elements.childCount.CheckIndex(selectedDisplayIndex2);
				if (flag5)
				{
					this.OnRefreshList(owner);
				}
				else
				{
					this.SelectIndex(owner, this.GetStateIndex(0), true);
				}
			});
			this.Next.ClearAndAddListener(delegate
			{
				this._scrollIndex++;
				int selectedDisplayIndex2 = this.GetDisplayIndex(this._selectIndex);
				bool flag5 = this.Elements.childCount.CheckIndex(selectedDisplayIndex2);
				if (flag5)
				{
					this.OnRefreshList(owner);
				}
				else
				{
					this.SelectIndex(owner, this.GetStateIndex(0), true);
				}
			});
		}
		for (int displayIndex = 0; displayIndex < displayCount; displayIndex++)
		{
			Refers display = this.Elements.GetChild(displayIndex).GetComponent<Refers>();
			int stateIndex = this.GetStateIndex(displayIndex);
			bool flag4 = !this._characterStateList.CheckIndex(stateIndex);
			if (flag4)
			{
				display.gameObject.SetActive(false);
			}
			else
			{
				CToggleObsolete toggle = display.CGet<CToggleObsolete>("Toggle");
				DisableStyleRoot style = display.GetComponent<DisableStyleRoot>();
				CharacterMenuStackView.StackCharacterState state = this._characterStateList[stateIndex];
				toggle.onValueChanged.RemoveAllListeners();
				toggle.interactable = (this._selectIndex != stateIndex);
				toggle.isOn = (this._selectIndex == stateIndex);
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (isOn)
					{
						this.SelectIndex(owner, stateIndex, true);
					}
				});
				display.CGet<TextMeshProUGUI>("Name").text = state.Name;
				Action<Game.Components.Avatar.Avatar> avatarRefresher = state.AvatarRefresher;
				if (avatarRefresher != null)
				{
					avatarRefresher(display.CGet<Game.Components.Avatar.Avatar>("Avatar"));
				}
				style.SetStyleEffect(stateIndex > this._selectIndex, false);
				display.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x000C931A File Offset: 0x000C751A
	private int GetDisplayIndex(int stateIndex, int scrolled)
	{
		return stateIndex - scrolled;
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x000C931F File Offset: 0x000C751F
	private int GetDisplayIndex(int stateIndex)
	{
		return this.GetDisplayIndex(stateIndex, this._scrollIndex);
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x000C932E File Offset: 0x000C752E
	private int GetStateIndex(int displayIndex, int scrolled)
	{
		return scrolled + displayIndex;
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x000C9333 File Offset: 0x000C7533
	private int GetStateIndex(int displayIndex)
	{
		return this.GetStateIndex(displayIndex, this._scrollIndex);
	}

	// Token: 0x0400164D RID: 5709
	public CButtonObsolete Prev;

	// Token: 0x0400164E RID: 5710
	public CButtonObsolete Next;

	// Token: 0x0400164F RID: 5711
	public CButtonObsolete First;

	// Token: 0x04001650 RID: 5712
	public CButtonObsolete Last;

	// Token: 0x04001651 RID: 5713
	public RectTransform Elements;

	// Token: 0x04001652 RID: 5714
	public CButtonObsolete Confirm;

	// Token: 0x04001653 RID: 5715
	private int _selectIndex;

	// Token: 0x04001654 RID: 5716
	private int _scrollIndex;

	// Token: 0x04001655 RID: 5717
	private readonly List<CharacterMenuStackView.StackCharacterState> _characterStateList = new List<CharacterMenuStackView.StackCharacterState>();

	// Token: 0x020013D4 RID: 5076
	public struct StackCharacterState
	{
		// Token: 0x04009F1A RID: 40730
		public int CharacterId;

		// Token: 0x04009F1B RID: 40731
		public int[] CharacterIdList;

		// Token: 0x04009F1C RID: 40732
		public bool CanOperate;

		// Token: 0x04009F1D RID: 40733
		public int PageId;

		// Token: 0x04009F1E RID: 40734
		public int SubPageId;

		// Token: 0x04009F1F RID: 40735
		public string Name;

		// Token: 0x04009F20 RID: 40736
		public Action<Game.Components.Avatar.Avatar> AvatarRefresher;
	}
}
