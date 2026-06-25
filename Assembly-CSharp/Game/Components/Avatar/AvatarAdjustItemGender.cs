using System;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F7C RID: 3964
	public class AvatarAdjustItemGender : AvatarAdjustItemBase
	{
		// Token: 0x0600B60A RID: 46602 RVA: 0x0052E94A File Offset: 0x0052CB4A
		private new void Awake()
		{
			this.Closed = false;
		}

		// Token: 0x0600B60B RID: 46603 RVA: 0x0052E954 File Offset: 0x0052CB54
		private new void Start()
		{
			this.ToggleGroup.InitPreOnToggle(-1);
			this.ToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnGenderChange);
			this.GenderRevers.onValueChanged.AddListener(new UnityAction<bool>(this.OnReverseGenderValueChange));
		}

		// Token: 0x0600B60C RID: 46604 RVA: 0x0052E9A4 File Offset: 0x0052CBA4
		public override void OnOpen(bool anim)
		{
			this.ToggleGroup.Set((int)this.Controller.GetGender(), true, false);
			this.GenderRevers.SetIsOnWithoutNotify(this.Controller.GetTransGender());
			this.GenderReverseLabel.text = LocalStringManager.Get((this.Controller.GetGender() == 1) ? LanguageKey.UI_NewGame_FemaleLike : LanguageKey.UI_NewGame_MaleLike);
		}

		// Token: 0x0600B60D RID: 46605 RVA: 0x0052EA10 File Offset: 0x0052CC10
		private void OnGenderChange(CToggleObsolete togNew, CToggleObsolete togPre)
		{
			this.Controller.SetGender((sbyte)togNew.Key);
			this.GenderReverseLabel.text = LocalStringManager.Get((this.Controller.GetGender() == 1) ? LanguageKey.UI_NewGame_FemaleLike : LanguageKey.UI_NewGame_MaleLike);
			Action onRefreshDisplay = this.OnRefreshDisplay;
			if (onRefreshDisplay != null)
			{
				onRefreshDisplay();
			}
		}

		// Token: 0x0600B60E RID: 46606 RVA: 0x0052EA6E File Offset: 0x0052CC6E
		private void OnReverseGenderValueChange(bool value)
		{
			this.Controller.SetTransGender(value);
			Action onRefreshDisplay = this.OnRefreshDisplay;
			if (onRefreshDisplay != null)
			{
				onRefreshDisplay();
			}
		}

		// Token: 0x04008D61 RID: 36193
		public CToggleGroupObsolete ToggleGroup;

		// Token: 0x04008D62 RID: 36194
		public CToggleObsolete GenderRevers;

		// Token: 0x04008D63 RID: 36195
		public TextMeshProUGUI GenderReverseLabel;

		// Token: 0x04008D64 RID: 36196
		public Action OnRefreshDisplay;
	}
}
