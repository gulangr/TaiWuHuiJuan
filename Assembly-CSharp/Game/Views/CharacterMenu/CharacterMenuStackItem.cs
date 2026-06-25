using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B6A RID: 2922
	public class CharacterMenuStackItem : MonoBehaviour
	{
		// Token: 0x06009095 RID: 37013 RVA: 0x0043622C File Offset: 0x0043442C
		private void Awake()
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
		}

		// Token: 0x06009096 RID: 37014 RVA: 0x0043626C File Offset: 0x0043446C
		private void OnDestroy()
		{
			bool flag = this.toggle != null;
			if (flag)
			{
				this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
		}

		// Token: 0x06009097 RID: 37015 RVA: 0x004362AC File Offset: 0x004344AC
		private void OnToggleValueChanged(bool isOn)
		{
			if (isOn)
			{
				Action onSelectAction = this._onSelectAction;
				if (onSelectAction != null)
				{
					onSelectAction();
				}
			}
		}

		// Token: 0x06009098 RID: 37016 RVA: 0x004362D4 File Offset: 0x004344D4
		public void Setup(string displayName, Action<Game.Components.Avatar.Avatar> avatarRefresher, bool isSelected, bool isDisabled, Action onSelect)
		{
			bool flag = this.nameText != null;
			if (flag)
			{
				this.nameText.text = displayName;
			}
			if (avatarRefresher != null)
			{
				avatarRefresher(this.avatar);
			}
			bool flag2 = this.toggle != null;
			if (flag2)
			{
				this.toggle.SetIsOnWithoutNotify(isSelected);
				this.toggle.interactable = !isSelected;
			}
			this._onSelectAction = onSelect;
		}

		// Token: 0x06009099 RID: 37017 RVA: 0x00436348 File Offset: 0x00434548
		public void SetRelationText(string relation)
		{
			bool flag = this.relationText != null;
			if (flag)
			{
				this.relationText.text = (relation ?? string.Empty);
			}
		}

		// Token: 0x0600909A RID: 37018 RVA: 0x0043637C File Offset: 0x0043457C
		public void SetRelationActive(bool active)
		{
			bool flag = this.relationNode != null;
			if (flag)
			{
				this.relationNode.SetActive(active);
			}
			else
			{
				bool flag2 = this.relationText != null;
				if (flag2)
				{
					this.relationText.gameObject.SetActive(active);
				}
			}
		}

		// Token: 0x17000FB7 RID: 4023
		// (get) Token: 0x0600909B RID: 37019 RVA: 0x004363CB File Offset: 0x004345CB
		public Game.Components.Avatar.Avatar Avatar
		{
			get
			{
				return this.avatar;
			}
		}

		// Token: 0x04006F4D RID: 28493
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04006F4E RID: 28494
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04006F4F RID: 28495
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006F50 RID: 28496
		[SerializeField]
		private TextMeshProUGUI relationText;

		// Token: 0x04006F51 RID: 28497
		[SerializeField]
		private GameObject relationNode;

		// Token: 0x04006F52 RID: 28498
		private Action _onSelectAction;
	}
}
