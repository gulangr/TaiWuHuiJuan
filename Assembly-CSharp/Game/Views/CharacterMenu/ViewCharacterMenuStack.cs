using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BAE RID: 2990
	public class ViewCharacterMenuStack : MonoBehaviour
	{
		// Token: 0x06009673 RID: 38515 RVA: 0x00462F20 File Offset: 0x00461120
		public void InitConfirmButton(ViewCharacterMenu owner)
		{
			this.backButton.ClearAndAddListener(delegate
			{
				this.ResetAndSetNotActive(owner);
			});
		}

		// Token: 0x06009674 RID: 38516 RVA: 0x00462F5A File Offset: 0x0046115A
		public void ClearAndSetNotActive()
		{
			this._scrollIndex = 1;
			this._selectIndex = 0;
			this._characterStateList.Clear();
			base.gameObject.SetActive(false);
			GEvent.OnEvent(UiEvents.RefreshCharacterMenuStack, null);
		}

		// Token: 0x06009675 RID: 38517 RVA: 0x00462F98 File Offset: 0x00461198
		public void ResetAndSetNotActive(ViewCharacterMenu owner)
		{
			bool flag = this._characterStateList.Count > 0;
			if (flag)
			{
				owner.TransitionToStackCharacterState(this._characterStateList[0], true);
			}
			this.ClearAndSetNotActive();
			owner.OnStackViewDeactivated();
		}

		// Token: 0x06009676 RID: 38518 RVA: 0x00462FDC File Offset: 0x004611DC
		public void ToCharacterAndActivate(ViewCharacterMenu owner, int characterId, string displayName = null, Action<Game.Components.Avatar.Avatar> avatarRefresher = null, string relationText = null)
		{
			int index = this._characterStateList.FindLastIndex((ViewCharacterMenuStack.StackCharacterState s) => s.CharacterId == characterId);
			bool flag = index > 0;
			if (flag)
			{
				ViewCharacterMenuStack.StackCharacterState existingState = this._characterStateList[index];
				bool flag2 = displayName != null;
				if (flag2)
				{
					existingState.Name = displayName;
				}
				bool flag3 = avatarRefresher != null;
				if (flag3)
				{
					existingState.AvatarRefresher = avatarRefresher;
				}
				bool flag4 = relationText != null;
				if (flag4)
				{
					existingState.RelationText = relationText;
				}
				this._characterStateList[index] = existingState;
				this.SelectIndex(owner, index, true);
				GEvent.OnEvent(UiEvents.RefreshCharacterMenuStack, null);
			}
			else
			{
				this.PushAndActivate(owner, characterId, displayName, avatarRefresher, relationText);
			}
		}

		// Token: 0x06009677 RID: 38519 RVA: 0x004630A0 File Offset: 0x004612A0
		public void PushAndActivate(ViewCharacterMenu owner, int characterId, string displayName = null, Action<Game.Components.Avatar.Avatar> avatarRefresher = null, string relationText = null)
		{
			ViewCharacterMenuStack.<>c__DisplayClass14_0 CS$<>8__locals1 = new ViewCharacterMenuStack.<>c__DisplayClass14_0();
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
				this._characterStateList.Add(CS$<>8__locals1.owner.MakeStackCharacterState());
			}
			int lastStateIndex = this._characterStateList.Count - 1;
			bool flag3 = lastStateIndex >= 0 && this._characterStateList[lastStateIndex].CharacterId == characterId;
			if (flag3)
			{
				ViewCharacterMenuStack.StackCharacterState existingState = this._characterStateList[lastStateIndex];
				bool flag4 = displayName != null;
				if (flag4)
				{
					existingState.Name = displayName;
				}
				bool flag5 = avatarRefresher != null;
				if (flag5)
				{
					existingState.AvatarRefresher = avatarRefresher;
				}
				bool flag6 = relationText != null;
				if (flag6)
				{
					existingState.RelationText = relationText;
				}
				this._characterStateList[lastStateIndex] = existingState;
				this.SelectIndex(CS$<>8__locals1.owner, lastStateIndex, true);
				GEvent.OnEvent(UiEvents.RefreshCharacterMenuStack, null);
			}
			else
			{
				CS$<>8__locals1.current = CS$<>8__locals1.owner.MakeStackCharacterState();
				CS$<>8__locals1.current.CharacterId = characterId;
				CS$<>8__locals1.current.Name = displayName;
				CS$<>8__locals1.current.AvatarRefresher = avatarRefresher;
				CS$<>8__locals1.current.RelationText = relationText;
				CS$<>8__locals1.current.CanOperate = false;
				ViewCharacterMenuDisplayData displayData = CS$<>8__locals1.owner.DisplayData;
				bool isInTaiwuTeam = ((displayData != null) ? displayData.TaiwuTeamCharIds : null) != null && displayData.TaiwuTeamCharIds.Contains(characterId);
				bool isInTaiwuSpecialGroup = ((displayData != null) ? displayData.TaiwuSpecialGroup : null) != null && displayData.TaiwuSpecialGroup.Contains(characterId);
				bool flag7 = isInTaiwuTeam || isInTaiwuSpecialGroup;
				if (flag7)
				{
					List<int> ids = new List<int>();
					bool flag8 = ((displayData != null) ? displayData.TaiwuTeamCharIds : null) != null;
					if (flag8)
					{
						ids.AddRange(displayData.TaiwuTeamCharIds);
					}
					bool flag9 = ((displayData != null) ? displayData.TaiwuSpecialGroup : null) != null;
					if (flag9)
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
		}

		// Token: 0x06009678 RID: 38520 RVA: 0x00463320 File Offset: 0x00461520
		public void SelectIndex(ViewCharacterMenu owner, int index, bool withRefresh = true)
		{
			this._selectIndex = index;
			ViewCharacterMenuStack.StackCharacterState current = this._characterStateList[index];
			owner.TransitionToStackCharacterState(current, true);
			if (withRefresh)
			{
				this.OnRefreshList(owner);
			}
		}

		// Token: 0x06009679 RID: 38521 RVA: 0x00463358 File Offset: 0x00461558
		private void OnRefreshList(ViewCharacterMenu owner)
		{
			int displayCount = Math.Min(this.Elements.childCount, 7);
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
					bool flag8 = selectedDisplayIndex2 >= 0 && selectedDisplayIndex2 < displayCount;
					if (flag8)
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
					bool flag8 = selectedDisplayIndex2 >= 0 && selectedDisplayIndex2 < displayCount;
					if (flag8)
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
					bool flag8 = selectedDisplayIndex2 >= 0 && selectedDisplayIndex2 < displayCount;
					if (flag8)
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
					bool flag8 = selectedDisplayIndex2 >= 0 && selectedDisplayIndex2 < displayCount;
					if (flag8)
					{
						this.OnRefreshList(owner);
					}
					else
					{
						this.SelectIndex(owner, this.GetStateIndex(0), true);
					}
				});
			}
			for (int displayIndex = 0; displayIndex < this.Elements.childCount; displayIndex++)
			{
				CharacterMenuStackItem item = this.Elements.GetChild(displayIndex).GetComponent<CharacterMenuStackItem>();
				bool flag4 = item == null;
				if (!flag4)
				{
					bool flag5 = displayIndex >= displayCount;
					if (flag5)
					{
						item.gameObject.SetActive(false);
					}
					else
					{
						int stateIndex = this.GetStateIndex(displayIndex);
						bool flag6 = stateIndex < 0 || stateIndex >= this._characterStateList.Count;
						if (flag6)
						{
							item.gameObject.SetActive(false);
						}
						else
						{
							ViewCharacterMenuStack.StackCharacterState state = this._characterStateList[stateIndex];
							int nextStateIndex = stateIndex + 1;
							bool hasNext = nextStateIndex < this._characterStateList.Count;
							item.SetRelationActive(hasNext);
							bool flag7 = hasNext;
							if (flag7)
							{
								ViewCharacterMenuStack.StackCharacterState nextState = this._characterStateList[nextStateIndex];
								item.SetRelationText(nextState.RelationText);
							}
							item.Setup(state.Name, state.AvatarRefresher, this._selectIndex == stateIndex, stateIndex > this._selectIndex, delegate
							{
								this.SelectIndex(owner, stateIndex, true);
							});
							item.gameObject.SetActive(true);
						}
					}
				}
			}
		}

		// Token: 0x0600967A RID: 38522 RVA: 0x004636D4 File Offset: 0x004618D4
		private int GetDisplayIndex(int stateIndex, int scrolled)
		{
			return stateIndex - scrolled;
		}

		// Token: 0x0600967B RID: 38523 RVA: 0x004636D9 File Offset: 0x004618D9
		private int GetDisplayIndex(int stateIndex)
		{
			return this.GetDisplayIndex(stateIndex, this._scrollIndex);
		}

		// Token: 0x0600967C RID: 38524 RVA: 0x004636E8 File Offset: 0x004618E8
		private int GetStateIndex(int displayIndex, int scrolled)
		{
			return scrolled + displayIndex;
		}

		// Token: 0x0600967D RID: 38525 RVA: 0x004636ED File Offset: 0x004618ED
		private int GetStateIndex(int displayIndex)
		{
			return this.GetStateIndex(displayIndex, this._scrollIndex);
		}

		// Token: 0x0400736B RID: 29547
		public CButton Prev;

		// Token: 0x0400736C RID: 29548
		public CButton Next;

		// Token: 0x0400736D RID: 29549
		public CButton First;

		// Token: 0x0400736E RID: 29550
		public CButton Last;

		// Token: 0x0400736F RID: 29551
		public RectTransform Elements;

		// Token: 0x04007370 RID: 29552
		public CButton backButton;

		// Token: 0x04007371 RID: 29553
		private int _selectIndex;

		// Token: 0x04007372 RID: 29554
		private int _scrollIndex;

		// Token: 0x04007373 RID: 29555
		private const int MaxDisplayCount = 7;

		// Token: 0x04007374 RID: 29556
		private readonly List<ViewCharacterMenuStack.StackCharacterState> _characterStateList = new List<ViewCharacterMenuStack.StackCharacterState>();

		// Token: 0x0200224A RID: 8778
		public struct StackCharacterState
		{
			// Token: 0x0400D911 RID: 55569
			public int CharacterId;

			// Token: 0x0400D912 RID: 55570
			public int[] CharacterIdList;

			// Token: 0x0400D913 RID: 55571
			public bool CanOperate;

			// Token: 0x0400D914 RID: 55572
			public int PageId;

			// Token: 0x0400D915 RID: 55573
			public int SubPageId;

			// Token: 0x0400D916 RID: 55574
			public string Name;

			// Token: 0x0400D917 RID: 55575
			public Action<Game.Components.Avatar.Avatar> AvatarRefresher;

			// Token: 0x0400D918 RID: 55576
			public string RelationText;
		}
	}
}
