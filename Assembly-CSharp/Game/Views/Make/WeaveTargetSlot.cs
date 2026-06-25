using System;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000963 RID: 2403
	public class WeaveTargetSlot : MonoBehaviour
	{
		// Token: 0x0600735A RID: 29530 RVA: 0x00359980 File Offset: 0x00357B80
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(true);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
			this.button.ClearAndAddListener(delegate
			{
				Action action = this._action;
				if (action != null)
				{
					action();
				}
			});
			this.genderToggleGroup.Init(-1);
			this.bodyTypeToggleGroup.Init(-1);
			this.genderToggleGroup.OnActiveIndexChange += this.OnGenderActiveIndexChange;
			this.bodyTypeToggleGroup.OnActiveIndexChange += this.OnBodyActiveIndexChange;
		}

		// Token: 0x0600735B RID: 29531 RVA: 0x00359A2A File Offset: 0x00357C2A
		private void OnGenderActiveIndexChange(int arg1, int arg2)
		{
			Action<int, int> action = this.genderToggleGroupAction;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		// Token: 0x0600735C RID: 29532 RVA: 0x00359A41 File Offset: 0x00357C41
		private void OnBodyActiveIndexChange(int arg1, int arg2)
		{
			Action<int, int> action = this.bodyTypeToggleGroupAction;
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		// Token: 0x0600735D RID: 29533 RVA: 0x00359A58 File Offset: 0x00357C58
		public void Set(AvatarData avatarData, short displayAge, string title, bool isEnable, Action action, int genderActiveIndex, int bodyTypeActiveIndex, Action<int, int> genderToggleGroupAction, Action<int, int> bodyTypeToggleGroupAction, short templateId)
		{
			this.avatar.gameObject.SetActive(avatarData != null);
			this.imageAdd.gameObject.SetActive(avatarData == null);
			this.toggleGroupHolder.gameObject.SetActive(avatarData != null);
			bool flag = avatarData != null;
			if (flag)
			{
				this.avatar.Refresh(avatarData, displayAge);
			}
			this.textTitle.text = title;
			this._action = action;
			this.imageAdd.sprite = (isEnable ? this.spriteImageAddEnable : this.spriteImageAddDisable);
			this.genderToggleGroup.gameObject.SetActive(avatarData != null);
			this.genderToggleGroup.SetWithoutNotify(genderActiveIndex);
			this.bodyTypeToggleGroup.gameObject.SetActive(avatarData != null);
			this.bodyTypeToggleGroup.SetWithoutNotify(bodyTypeActiveIndex);
			this.genderToggleGroupAction = genderToggleGroupAction;
			this.bodyTypeToggleGroupAction = bodyTypeToggleGroupAction;
			this.button.interactable = isEnable;
			this.pointerTrigger.enabled = isEnable;
			bool flag2 = !isEnable;
			if (flag2)
			{
				this.hover.gameObject.SetActive(false);
			}
			bool flag3 = templateId >= 0;
			if (flag3)
			{
				this.gradeLine.SetColor(Colors.Instance.GradeColors[(int)Clothing.Instance[templateId].Grade]);
			}
		}

		// Token: 0x0600735E RID: 29534 RVA: 0x00359BB4 File Offset: 0x00357DB4
		public void SetToFaceless()
		{
			bool flag = this.avatar == null;
			if (!flag)
			{
				bool flag2 = this.avatar.Data == null;
				if (!flag2)
				{
					this.avatar.RefreshAsClothTree(this.avatar.Data, 18, this.facelessHeadSprites, this.facelessSkinColor.ColorToHexString("#"));
				}
			}
		}

		// Token: 0x04005590 RID: 21904
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04005591 RID: 21905
		[SerializeField]
		private CButton button;

		// Token: 0x04005592 RID: 21906
		[SerializeField]
		private RectTransform hover;

		// Token: 0x04005593 RID: 21907
		[SerializeField]
		private CImage imageAdd;

		// Token: 0x04005594 RID: 21908
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005595 RID: 21909
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04005596 RID: 21910
		[SerializeField]
		private CToggleGroup genderToggleGroup;

		// Token: 0x04005597 RID: 21911
		[SerializeField]
		private CToggleGroup bodyTypeToggleGroup;

		// Token: 0x04005598 RID: 21912
		[SerializeField]
		private RectTransform toggleGroupHolder;

		// Token: 0x04005599 RID: 21913
		[SerializeField]
		private CImage gradeLine;

		// Token: 0x0400559A RID: 21914
		[SerializeField]
		private Sprite spriteImageAddEnable;

		// Token: 0x0400559B RID: 21915
		[SerializeField]
		private Sprite spriteImageAddDisable;

		// Token: 0x0400559C RID: 21916
		[SerializeField]
		private Sprite[] facelessHeadSprites = new Sprite[6];

		// Token: 0x0400559D RID: 21917
		[SerializeField]
		private Color facelessSkinColor;

		// Token: 0x0400559E RID: 21918
		private Action _action;

		// Token: 0x0400559F RID: 21919
		private Action<int, int> genderToggleGroupAction;

		// Token: 0x040055A0 RID: 21920
		private Action<int, int> bodyTypeToggleGroupAction;
	}
}
