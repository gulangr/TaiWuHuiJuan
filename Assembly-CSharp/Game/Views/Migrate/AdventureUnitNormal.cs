using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using FrameWork.UISystem.Components;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x020008E4 RID: 2276
	public class AdventureUnitNormal : TemplatedContainerAssemblyNew
	{
		// Token: 0x06006CF6 RID: 27894 RVA: 0x00323F3C File Offset: 0x0032213C
		public void UpdateParticleHolderActiveState()
		{
			bool flag = this.normalBlockParticleHolder != null;
			if (flag)
			{
				this.normalBlockParticleHolder.enabled = (this.normalBlockParticleHolder.transform.childCount > 0);
			}
		}

		// Token: 0x06006CF7 RID: 27895 RVA: 0x00323F7C File Offset: 0x0032217C
		public void RebuildBaseGround(string groundSpritePrefix)
		{
			this.RebuildBaseGround(delegate(CImage image, int index)
			{
				image.SetSprite(string.Format("{0}{1}", groundSpritePrefix, index), true, null);
			});
		}

		// Token: 0x06006CF8 RID: 27896 RVA: 0x00323FAC File Offset: 0x003221AC
		internal void RebuildBaseGround(Action<CImage, int> spriteGiver)
		{
			base.Rebuild<RectTransform>(AdventureUnitNormal.Offset.Count, delegate(RectTransform newGo, int index)
			{
				AdventureUnitMicro unitMicro = newGo.GetComponent<AdventureUnitMicro>();
				CImage ground = unitMicro.groundSurface;
				spriteGiver(ground, index);
				bool flag = ground.sprite != null;
				if (flag)
				{
					newGo.GetComponent<RectTransform>().anchoredPosition = AdventureUnitNormal.Offset[index] * ground.sprite.rect.size;
				}
			});
		}

		// Token: 0x06006CF9 RID: 27897 RVA: 0x00323FE4 File Offset: 0x003221E4
		public void SetMicroHolderActive(bool isActive)
		{
			this.microHolder.SetActive(isActive);
		}

		// Token: 0x06006CFA RID: 27898 RVA: 0x00323FF4 File Offset: 0x003221F4
		public void SetUnitMicro(AdventureUnitMicro unitMicro, int index)
		{
			this.UnitMicroArray[index] = unitMicro;
		}

		// Token: 0x06006CFB RID: 27899 RVA: 0x00324000 File Offset: 0x00322200
		public AdventureUnitMicro GetUnitMicro(int index)
		{
			return this.UnitMicroArray[index];
		}

		// Token: 0x04004F2D RID: 20269
		[SerializeField]
		private GameObject microHolder;

		// Token: 0x04004F2E RID: 20270
		[SerializeField]
		public CanvasGroup canvasGroup;

		// Token: 0x04004F2F RID: 20271
		public UIParticle normalBlockParticleHolder;

		// Token: 0x04004F30 RID: 20272
		private static readonly IReadOnlyList<Vector2> Offset = new Vector2[]
		{
			new Vector2(0f, 1f),
			new Vector2(-0.5f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(-1f, 0f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(-0.5f, -0.5f),
			new Vector2(0.5f, -0.5f),
			new Vector2(0f, -1f)
		};

		// Token: 0x04004F31 RID: 20273
		private AdventureUnitMicro[] UnitMicroArray = new AdventureUnitMicro[9];
	}
}
