using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009AB RID: 2475
	public class SwapSoulCharacterItem : MonoBehaviour
	{
		// Token: 0x17000D5F RID: 3423
		// (get) Token: 0x06007782 RID: 30594 RVA: 0x00379D6F File Offset: 0x00377F6F
		public AvatarData AvatarData
		{
			get
			{
				return (this.data != null && this.avatar.gameObject.activeSelf) ? this.avatar.Data : null;
			}
		}

		// Token: 0x17000D60 RID: 3424
		// (get) Token: 0x06007783 RID: 30595 RVA: 0x00379D99 File Offset: 0x00377F99
		private ViewSwapSoul Parent
		{
			get
			{
				return UIElement.SwapSoul.UiBaseAs<ViewSwapSoul>();
			}
		}

		// Token: 0x17000D61 RID: 3425
		// (get) Token: 0x06007784 RID: 30596 RVA: 0x00379DA5 File Offset: 0x00377FA5
		public bool IsEmpty
		{
			get
			{
				return this.data == null || this.data.CharacterId < 0;
			}
		}

		// Token: 0x06007785 RID: 30597 RVA: 0x00379DC0 File Offset: 0x00377FC0
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.hoverMask.gameObject.SetActive(this.data != null);
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.hoverMask.gameObject.SetActive(false);
			});
			this.selectCharBtn.ClearAndAddListener(delegate
			{
				this.Parent.OnSelectCharBtnClicked(this.Index, this.IsBody);
			});
			this.infoBtn.ClearAndAddListener(delegate
			{
				this.Parent.OnInfoBtnClicked(this.Index, this.IsBody);
			});
			this.deleteBtn.ClearAndAddListener(delegate
			{
				this.Parent.OnDeleteBtnClicked(this.Index, this.IsBody);
			});
		}

		// Token: 0x06007786 RID: 30598 RVA: 0x00379E74 File Offset: 0x00378074
		public void Set(CharacterDisplayData characterDisplayData, int soulLimit = 1)
		{
			this.hoverMask.gameObject.SetActive(false);
			this.avatar.gameObject.SetActive(characterDisplayData != null && this.Index < soulLimit);
			this.characterName.transform.parent.gameObject.SetActive(characterDisplayData != null && this.Index < soulLimit);
			this.selectCharBtn.interactable = (this.Index < soulLimit);
			this.selectCharBtn.GetComponent<TooltipInvoker>().enabled = (this.IsBody || !this.selectCharBtn.interactable);
			this.infoBtn.gameObject.SetActive(characterDisplayData != null && this.IsBody);
			this.deleteBtn.gameObject.SetActive(characterDisplayData != null);
			this.mouseTipDisplayer.enabled = (characterDisplayData != null);
			bool flag = characterDisplayData != null;
			if (flag)
			{
				this.data = characterDisplayData;
				this.avatar.Refresh(this.data, false);
				this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(this.data, SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this.data.CharacterId);
				TooltipInvoker tooltipInvoker = this.mouseTipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTipDisplayer.RuntimeParam.Set("charId", this.data.CharacterId);
			}
			else
			{
				this.data = null;
			}
		}

		// Token: 0x04005A4D RID: 23117
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005A4E RID: 23118
		[SerializeField]
		private TextMeshProUGUI characterName;

		// Token: 0x04005A4F RID: 23119
		[SerializeField]
		private CButton selectCharBtn;

		// Token: 0x04005A50 RID: 23120
		[SerializeField]
		private CButton infoBtn;

		// Token: 0x04005A51 RID: 23121
		[SerializeField]
		private CButton deleteBtn;

		// Token: 0x04005A52 RID: 23122
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x04005A53 RID: 23123
		[SerializeField]
		private RectTransform hoverMask;

		// Token: 0x04005A54 RID: 23124
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04005A55 RID: 23125
		public int Index;

		// Token: 0x04005A56 RID: 23126
		public bool IsBody;

		// Token: 0x04005A57 RID: 23127
		private CharacterDisplayData data;
	}
}
