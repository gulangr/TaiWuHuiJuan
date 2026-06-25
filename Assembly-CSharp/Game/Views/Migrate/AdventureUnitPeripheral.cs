using System;
using Coffee.UIExtensions;
using Config;
using Game.Views.Adventure;
using GameData.Domains.Map;
using Map.RenderSystem;
using UnityEngine;

namespace Game.Views.Migrate
{
	// Token: 0x020008E5 RID: 2277
	public class AdventureUnitPeripheral : MonoBehaviour
	{
		// Token: 0x06006CFE RID: 27902 RVA: 0x00324110 File Offset: 0x00322310
		public void UpdateParticleHolderActiveState()
		{
			bool flag = this.normalBlockParticleHolder != null;
			if (flag)
			{
				this.normalBlockParticleHolder.enabled = (this.normalBlockParticleHolder.transform.childCount > 0);
			}
		}

		// Token: 0x06006CFF RID: 27903 RVA: 0x00324150 File Offset: 0x00322350
		public void SetBlockPiece(MapBlockData blockData)
		{
			MapRenderSystem renderSystem = SingletonObject.getInstance<MapRenderSystem>();
			CImage cImage = this.blockPiece.GetComponent<CImage>();
			MapBlockItem config = blockData.GetConfig();
			bool blockHasFix = config.BlockHasFix;
			if (blockHasFix)
			{
				cImage.SetSprite(renderSystem.GetMapBlockSpriteNameByFix(blockData), true, null);
			}
			else
			{
				string iconName = renderSystem.GetMapBlockSpriteName(blockData);
				MapAtlasInfo.Instance.GetSprite(iconName, delegate(Sprite sprite)
				{
					cImage.enabled = (sprite != null);
					cImage.sprite = sprite;
					cImage.SetNativeSize();
				});
			}
		}

		// Token: 0x06006D00 RID: 27904 RVA: 0x003241C7 File Offset: 0x003223C7
		public void SetBlockPieceActive(bool active)
		{
			this.blockPiece.gameObject.SetActive(active);
		}

		// Token: 0x06006D01 RID: 27905 RVA: 0x003241DC File Offset: 0x003223DC
		public void SetPieceBrightness(float brightness)
		{
			AdventureOutsideBlockVertexModifier modifier = this.blockPiece.gameObject.GetOrAddComponent<AdventureOutsideBlockVertexModifier>();
			modifier.Brightness = brightness;
		}

		// Token: 0x06006D02 RID: 27906 RVA: 0x00324204 File Offset: 0x00322404
		public void SetPieceSaturation(float saturation)
		{
			AdventureOutsideBlockVertexModifier modifier = this.blockPiece.gameObject.GetOrAddComponent<AdventureOutsideBlockVertexModifier>();
			modifier.Saturation = saturation;
		}

		// Token: 0x04004F32 RID: 20274
		[SerializeField]
		public RectTransform blockPiece;

		// Token: 0x04004F33 RID: 20275
		[SerializeField]
		public CanvasGroup canvasGroup;

		// Token: 0x04004F34 RID: 20276
		[SerializeField]
		public UIParticle normalBlockParticleHolder;
	}
}
