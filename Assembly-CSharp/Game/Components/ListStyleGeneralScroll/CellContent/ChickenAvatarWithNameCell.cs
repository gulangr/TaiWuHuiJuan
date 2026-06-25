using System;
using Game.Components.Character;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB8 RID: 3768
	public class ChickenAvatarWithNameCell : MonoBehaviour, ICellContent<ChickenAvatarWithNameCellData>, ICellContent
	{
		// Token: 0x0600AEDE RID: 44766 RVA: 0x004FAB64 File Offset: 0x004F8D64
		public void SetData(ChickenAvatarWithNameCellData data)
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
					this.avatarWithName.Set(data);
					bool flag3 = data.MouseTipModifier != null;
					if (flag3)
					{
						this.avatarWithName.SetMouseTipModifier(data.MouseTipModifier);
					}
				}
			}
		}

		// Token: 0x0600AEDF RID: 44767 RVA: 0x004FABC9 File Offset: 0x004F8DC9
		public void SetSelected(bool selected)
		{
			this.avatarWithName.SetSelected(selected);
		}

		// Token: 0x0600AEE0 RID: 44768 RVA: 0x004FABDC File Offset: 0x004F8DDC
		public bool SetDisabled(bool disabled)
		{
			ChickenAvatarWithName chickenAvatarWithName = this.avatarWithName;
			if (chickenAvatarWithName != null)
			{
				chickenAvatarWithName.SetDisabled(disabled);
			}
			return true;
		}

		// Token: 0x0400873D RID: 34621
		[SerializeField]
		private ChickenAvatarWithName avatarWithName;
	}
}
