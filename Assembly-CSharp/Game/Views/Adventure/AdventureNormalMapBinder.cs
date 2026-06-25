using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C6A RID: 3178
	public class AdventureNormalMapBinder : MonoBehaviour
	{
		// Token: 0x0600A1D8 RID: 41432 RVA: 0x004BAC71 File Offset: 0x004B8E71
		private void Start()
		{
			base.Invoke("TryBindAllAtlases", 0.1f);
		}

		// Token: 0x0600A1D9 RID: 41433 RVA: 0x004BAC88 File Offset: 0x004B8E88
		public void TryBindAllAtlases()
		{
			bool flag = this._atlasBindings == null;
			if (!flag)
			{
				foreach (AdventureNormalMapBinder.AtlasBinding binding in this._atlasBindings)
				{
					this.TryBindAtlas(binding);
				}
			}
		}

		// Token: 0x0600A1DA RID: 41434 RVA: 0x004BACCC File Offset: 0x004B8ECC
		private void TryBindAtlas(AdventureNormalMapBinder.AtlasBinding binding)
		{
			bool flag = binding == null || binding.isBound;
			if (!flag)
			{
				bool flag2 = binding.sampleSprite == null;
				if (!flag2)
				{
					bool flag3 = binding.targetMaterial == null;
					if (!flag3)
					{
						Sprite sprite = binding.sampleSprite;
						int count = sprite.GetSecondaryTextures(this._secondaryTextures);
						for (int i = 0; i < count; i++)
						{
							bool flag4 = this._secondaryTextures[i].name == "_BumpMap";
							if (flag4)
							{
								binding.targetMaterial.SetTexture(AdventureNormalMapBinder.BumpMapId, this._secondaryTextures[i].texture);
								binding.isBound = true;
								Debug.Log("[AdventureNormalMapBinder] 法线图集已绑定: " + this._secondaryTextures[i].texture.name + " -> " + binding.targetMaterial.name);
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A1DB RID: 41435 RVA: 0x004BADD4 File Offset: 0x004B8FD4
		public void BindFromSprite(Sprite sprite, Material material)
		{
			bool flag = sprite == null || material == null;
			if (!flag)
			{
				int count = sprite.GetSecondaryTextures(this._secondaryTextures);
				for (int i = 0; i < count; i++)
				{
					bool flag2 = this._secondaryTextures[i].name == "_BumpMap";
					if (flag2)
					{
						material.SetTexture(AdventureNormalMapBinder.BumpMapId, this._secondaryTextures[i].texture);
						Debug.Log("[AdventureNormalMapBinder] 法线图集已绑定: " + this._secondaryTextures[i].texture.name + " -> " + material.name);
						break;
					}
				}
			}
		}

		// Token: 0x04007DC5 RID: 32197
		[Header("图集绑定配置")]
		[SerializeField]
		private AdventureNormalMapBinder.AtlasBinding[] _atlasBindings;

		// Token: 0x04007DC6 RID: 32198
		private static readonly int BumpMapId = Shader.PropertyToID("_BumpMap");

		// Token: 0x04007DC7 RID: 32199
		private SecondarySpriteTexture[] _secondaryTextures = new SecondarySpriteTexture[4];

		// Token: 0x020023A3 RID: 9123
		[Serializable]
		public class AtlasBinding
		{
			// Token: 0x0400DF82 RID: 57218
			[Tooltip("用于采样的Sprite（运行时会从它获取法线图集）")]
			public Sprite sampleSprite;

			// Token: 0x0400DF83 RID: 57219
			[Tooltip("要绑定法线贴图的目标材质")]
			public Material targetMaterial;

			// Token: 0x0400DF84 RID: 57220
			[HideInInspector]
			public bool isBound;
		}
	}
}
