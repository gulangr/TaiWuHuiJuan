using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000826 RID: 2086
	public class CommonParagraph : MonoBehaviour
	{
		// Token: 0x17000C54 RID: 3156
		// (get) Token: 0x06006650 RID: 26192 RVA: 0x002EB5D9 File Offset: 0x002E97D9
		public Transform AtomContainer
		{
			get
			{
				return this.atomContainer;
			}
		}

		// Token: 0x06006651 RID: 26193 RVA: 0x002EB5E4 File Offset: 0x002E97E4
		public void SetBackground(ParagraphBackgroundType type)
		{
			int index;
			bool flag = type == ParagraphBackgroundType.None || !CommonParagraph.BackgroundImageIndex.TryGetValue(type, out index);
			if (flag)
			{
				this.backgroundImage.SetEnabled(false);
			}
			else
			{
				this.backgroundImage.SetSprite(string.Format("{0}{1}", "ui9_back_mousetip_base_special_", index), false, null);
				this.backgroundImage.SetEnabled(true);
			}
		}

		// Token: 0x04004794 RID: 18324
		[SerializeField]
		private Transform atomContainer;

		// Token: 0x04004795 RID: 18325
		[SerializeField]
		private CImagePinned backgroundImage;

		// Token: 0x04004796 RID: 18326
		private static readonly Dictionary<ParagraphBackgroundType, int> BackgroundImageIndex = new Dictionary<ParagraphBackgroundType, int>
		{
			{
				ParagraphBackgroundType.Poison,
				0
			},
			{
				ParagraphBackgroundType.Buff,
				1
			},
			{
				ParagraphBackgroundType.VeryImportant,
				3
			},
			{
				ParagraphBackgroundType.Debuff,
				4
			},
			{
				ParagraphBackgroundType.NormalImportant,
				5
			}
		};
	}
}
