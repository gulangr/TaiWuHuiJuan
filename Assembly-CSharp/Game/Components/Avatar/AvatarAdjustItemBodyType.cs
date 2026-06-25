using System;

namespace Game.Components.Avatar
{
	// Token: 0x02000F75 RID: 3957
	public class AvatarAdjustItemBodyType : AvatarAdjustItemBase
	{
		// Token: 0x0600B590 RID: 46480 RVA: 0x0052B8B1 File Offset: 0x00529AB1
		private new void Awake()
		{
			this.Closed = false;
		}

		// Token: 0x0600B591 RID: 46481 RVA: 0x0052B8BC File Offset: 0x00529ABC
		protected override void Start()
		{
			byte curIndex = this.Controller.GetGroupIndexByAvatarId(base.Data.AvatarId).Item1;
			CToggleGroupObsolete toggleGroup = this.Refers.CGet<CToggleGroupObsolete>("BodyTypeBase");
			toggleGroup.InitPreOnToggle(-1);
			toggleGroup.OnActiveToggleChange = delegate(CToggleObsolete toggleNew, CToggleObsolete toggleOld)
			{
				this.Controller.SetCurAvatarGroupIndex(toggleNew.Key);
				this.OnQuickAdjustTriggered(0);
			};
			bool flag = null != this.Controller;
			if (flag)
			{
				this.SetId((int)curIndex);
				toggleGroup.Set((int)curIndex, true, false);
				this.OnQuickAdjustTriggered(0);
			}
		}

		// Token: 0x0600B592 RID: 46482 RVA: 0x0052B93D File Offset: 0x00529B3D
		private void OnEnable()
		{
			this.SetShowState();
		}

		// Token: 0x0600B593 RID: 46483 RVA: 0x0052B947 File Offset: 0x00529B47
		public override void OnOpen(bool anim)
		{
			this.SetShowState();
		}

		// Token: 0x0600B594 RID: 46484 RVA: 0x0052B954 File Offset: 0x00529B54
		private void SetShowState()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				CToggleGroupObsolete toggleGroup = this.Refers.CGet<CToggleGroupObsolete>("BodyTypeBase");
				toggleGroup.Set((int)this.Controller.GetGroupIndexByAvatarId(base.Data.AvatarId).Item1, true, false);
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					for (sbyte i = 0; i <= 2; i += 1)
					{
						bool canInteract = this.CustomAssetsFilterHandler(i);
						toggleGroup.SetInteractable(canInteract, (int)i);
						toggleGroup.Get((int)i).gameObject.GetOrAddComponent<DisableStyleRoot>().SetStyleEffect(!canInteract, false);
					}
				}
			}
		}

		// Token: 0x04008D50 RID: 36176
		public Func<sbyte, bool> CustomAssetsFilterHandler;
	}
}
