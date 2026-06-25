using System;
using Game.Components.Character;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB0 RID: 3760
	public class AvatarWithNameCell : MonoBehaviour, ICellContent<AvatarWithNameCellData>, ICellContent
	{
		// Token: 0x0600AEC4 RID: 44740 RVA: 0x004FA1E0 File Offset: 0x004F83E0
		public void SetData(AvatarWithNameCellData data)
		{
			bool flag = this.avatarWithName == null;
			if (!flag)
			{
				bool flag2 = data == null;
				if (flag2)
				{
					this.avatarWithName.SetEmpty();
				}
				else
				{
					bool asGrave = data.AsGrave;
					if (asGrave)
					{
						this.avatarWithName.Set(data.DisplayName, data.CharacterId, data.OnClickCallback, false, false);
					}
					else
					{
						bool asMerchant = data.AsMerchant;
						if (asMerchant)
						{
							this.avatarWithName.Set(data.TemplateId);
						}
						else
						{
							this.avatarWithName.Set(data.AvatarRelatedData, data.TemplateId, data.DisplayName, data.CharacterId, data.OnClickCallback, data.IsCompanion, data.Followed);
						}
					}
					bool flag3 = data.MouseTipModifier != null;
					if (flag3)
					{
						this.avatarWithName.SetMouseTipModifier(data.MouseTipModifier);
					}
				}
			}
		}

		// Token: 0x0600AEC5 RID: 44741 RVA: 0x004FA2C6 File Offset: 0x004F84C6
		public void SetSelected(bool selected)
		{
			this.avatarWithName.SetSelected(selected);
		}

		// Token: 0x0600AEC6 RID: 44742 RVA: 0x004FA2D8 File Offset: 0x004F84D8
		public bool SetDisabled(bool disabled)
		{
			AvatarWithName avatarWithName = this.avatarWithName;
			if (avatarWithName != null)
			{
				avatarWithName.SetDisabled(disabled);
			}
			return true;
		}

		// Token: 0x04008720 RID: 34592
		[SerializeField]
		private AvatarWithName avatarWithName;
	}
}
