using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000845 RID: 2117
	public class ItemPanelRelation : MonoBehaviour
	{
		// Token: 0x060066EE RID: 26350 RVA: 0x002EF484 File Offset: 0x002ED684
		public void Set(IReadOnlyList<NameAndAvatar> chars, int total)
		{
			this.count.text = LanguageKey.LK_MouseTip_LegendaryBook_Relation_Count.TrFormat(total);
			this.render.Rebuild<AvatarAndName>(5, delegate(AvatarAndName item, int index)
			{
				bool flag = chars.CheckIndexReadOnly(index);
				if (flag)
				{
					item.Set(chars[index]);
				}
				else
				{
					item.SetEmpty();
				}
			});
		}

		// Token: 0x04004874 RID: 18548
		private const int RefreshCount = 5;

		// Token: 0x04004875 RID: 18549
		[SerializeField]
		private TemplatedContainerAssemblyNew render;

		// Token: 0x04004876 RID: 18550
		[SerializeField]
		private TMP_Text count;
	}
}
