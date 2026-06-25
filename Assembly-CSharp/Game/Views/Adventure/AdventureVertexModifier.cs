using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Adventure;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Adventure
{
	// Token: 0x02000C74 RID: 3188
	[RequireComponent(typeof(Graphic))]
	public class AdventureVertexModifier : BaseMeshEffect
	{
		// Token: 0x170010F4 RID: 4340
		// (get) Token: 0x0600A1FD RID: 41469 RVA: 0x004BB8B8 File Offset: 0x004B9AB8
		// (set) Token: 0x0600A1FE RID: 41470 RVA: 0x004BB8C0 File Offset: 0x004B9AC0
		public AdventureBlockIndex GridIndex
		{
			get
			{
				return this.gridIndex;
			}
			set
			{
				bool flag = this.gridIndex != value;
				if (flag)
				{
					this.gridIndex = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x170010F5 RID: 4341
		// (get) Token: 0x0600A1FF RID: 41471 RVA: 0x004BB903 File Offset: 0x004B9B03
		public Graphic TargetGraphic
		{
			get
			{
				return base.graphic;
			}
		}

		// Token: 0x170010F6 RID: 4342
		// (get) Token: 0x0600A200 RID: 41472 RVA: 0x004BB90B File Offset: 0x004B9B0B
		// (set) Token: 0x0600A201 RID: 41473 RVA: 0x004BB914 File Offset: 0x004B9B14
		public float Brightness
		{
			get
			{
				return this.brightness;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.brightness, value);
				if (flag)
				{
					this.brightness = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x170010F7 RID: 4343
		// (get) Token: 0x0600A202 RID: 41474 RVA: 0x004BB95A File Offset: 0x004B9B5A
		// (set) Token: 0x0600A203 RID: 41475 RVA: 0x004BB964 File Offset: 0x004B9B64
		public float Saturation
		{
			get
			{
				return this.saturation;
			}
			set
			{
				bool flag = !Mathf.Approximately(this.saturation, value);
				if (flag)
				{
					this.saturation = value;
					bool flag2 = base.graphic != null;
					if (flag2)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		// Token: 0x0600A204 RID: 41476 RVA: 0x004BB9AA File Offset: 0x004B9BAA
		protected virtual void ModifyVertex(ref UIVertex vertex, Rect rect, int index)
		{
			vertex.uv3 = Vector4.zero;
		}

		// Token: 0x0600A205 RID: 41477 RVA: 0x004BB9B8 File Offset: 0x004B9BB8
		private void EnsureSpriteNormalMap()
		{
			Image image = base.graphic as Image;
			bool flag = image == null || image.sprite == null;
			if (!flag)
			{
				Texture normalTexture = AdventureVertexModifier.GetSpriteNormalTexture(image.sprite);
				bool flag2 = normalTexture == null;
				if (flag2)
				{
					Material current = base.graphic.material;
					bool flag3 = current != null && AdventureVertexModifier._normalMaterialBases.ContainsKey(current);
					if (flag3)
					{
						AdventureVertexModifier._pendingMaterialRestores.Add(image);
					}
				}
				else
				{
					Material currentMaterial = base.graphic.material;
					bool flag4 = currentMaterial == null || !currentMaterial.HasProperty(AdventureVertexModifier.BumpMapId);
					if (!flag4)
					{
						bool flag5 = currentMaterial.GetTexture(AdventureVertexModifier.BumpMapId) == normalTexture;
						if (!flag5)
						{
							Material baseMaterial = AdventureVertexModifier.GetBaseMaterial(currentMaterial);
							AdventureVertexModifier._pendingMaterialSets.Add(new ValueTuple<Image, Material>(image, AdventureVertexModifier.GetNormalMaterial(baseMaterial, normalTexture)));
						}
					}
				}
			}
		}

		// Token: 0x0600A206 RID: 41478 RVA: 0x004BBAAC File Offset: 0x004B9CAC
		private static void ApplyPendingMaterialChanges()
		{
			Canvas.willRenderCanvases -= AdventureVertexModifier.ApplyPendingMaterialChanges;
			AdventureVertexModifier._willRenderRegistered = false;
			for (int i = 0; i < AdventureVertexModifier._pendingMaterialSets.Count; i++)
			{
				ValueTuple<Image, Material> valueTuple = AdventureVertexModifier._pendingMaterialSets[i];
				Image image = valueTuple.Item1;
				Material material = valueTuple.Item2;
				bool flag = image != null;
				if (flag)
				{
					image.material = material;
				}
			}
			AdventureVertexModifier._pendingMaterialSets.Clear();
			for (int j = 0; j < AdventureVertexModifier._pendingMaterialRestores.Count; j++)
			{
				Image image2 = AdventureVertexModifier._pendingMaterialRestores[j];
				bool flag2 = image2 == null;
				if (!flag2)
				{
					Material mat = image2.material;
					Material baseMat;
					bool flag3 = mat != null && AdventureVertexModifier._normalMaterialBases.TryGetValue(mat, out baseMat);
					if (flag3)
					{
						image2.material = baseMat;
					}
				}
			}
			AdventureVertexModifier._pendingMaterialRestores.Clear();
		}

		// Token: 0x0600A207 RID: 41479 RVA: 0x004BBBA4 File Offset: 0x004B9DA4
		private void ScheduleMaterialChange()
		{
			bool flag = !AdventureVertexModifier._willRenderRegistered && (AdventureVertexModifier._pendingMaterialSets.Count > 0 || AdventureVertexModifier._pendingMaterialRestores.Count > 0);
			if (flag)
			{
				AdventureVertexModifier._willRenderRegistered = true;
				Canvas.willRenderCanvases += AdventureVertexModifier.ApplyPendingMaterialChanges;
			}
		}

		// Token: 0x0600A208 RID: 41480 RVA: 0x004BBBF8 File Offset: 0x004B9DF8
		private void RestoreBaseMaterial()
		{
			Material currentMaterial = base.graphic.material;
			Material baseMaterial;
			bool flag = currentMaterial != null && AdventureVertexModifier._normalMaterialBases.TryGetValue(currentMaterial, out baseMaterial);
			if (flag)
			{
				base.graphic.material = baseMaterial;
			}
		}

		// Token: 0x0600A209 RID: 41481 RVA: 0x004BBC3C File Offset: 0x004B9E3C
		private static Texture GetSpriteNormalTexture(Sprite sprite)
		{
			int count = sprite.GetSecondaryTextures(AdventureVertexModifier._secondaryTextures);
			for (int i = 0; i < count; i++)
			{
				bool flag = AdventureVertexModifier._secondaryTextures[i].name == "_BumpMap";
				if (flag)
				{
					return AdventureVertexModifier._secondaryTextures[i].texture;
				}
			}
			return null;
		}

		// Token: 0x0600A20A RID: 41482 RVA: 0x004BBCA0 File Offset: 0x004B9EA0
		private static Material GetBaseMaterial(Material material)
		{
			Material baseMaterial;
			return AdventureVertexModifier._normalMaterialBases.TryGetValue(material, out baseMaterial) ? baseMaterial : material;
		}

		// Token: 0x0600A20B RID: 41483 RVA: 0x004BBCC8 File Offset: 0x004B9EC8
		private static Material GetNormalMaterial(Material baseMaterial, Texture normalTexture)
		{
			Dictionary<Texture, Material> materialsByTexture;
			bool flag = !AdventureVertexModifier._normalMaterials.TryGetValue(baseMaterial, out materialsByTexture);
			if (flag)
			{
				materialsByTexture = new Dictionary<Texture, Material>();
				AdventureVertexModifier._normalMaterials.Add(baseMaterial, materialsByTexture);
			}
			Material material;
			bool flag2 = materialsByTexture.TryGetValue(normalTexture, out material);
			Material result;
			if (flag2)
			{
				result = material;
			}
			else
			{
				material = new Material(baseMaterial)
				{
					name = baseMaterial.name + "_" + normalTexture.name
				};
				material.SetTexture(AdventureVertexModifier.BumpMapId, normalTexture);
				materialsByTexture.Add(normalTexture, material);
				AdventureVertexModifier._normalMaterialBases.Add(material, baseMaterial);
				result = material;
			}
			return result;
		}

		// Token: 0x0600A20C RID: 41484 RVA: 0x004BBD60 File Offset: 0x004B9F60
		public override void ModifyMesh(VertexHelper vh)
		{
			bool flag = !this.IsActive() || base.graphic == null;
			if (!flag)
			{
				this.EnsureSpriteNormalMap();
				this.ScheduleMaterialChange();
				Rect rect = base.graphic.rectTransform.rect;
				int count = vh.currentVertCount;
				UIVertex vert = default(UIVertex);
				for (int i = 0; i < count; i++)
				{
					vh.PopulateUIVertex(ref vert, i);
					vert.uv2 = new Vector4((float)this.gridIndex.Gx, (float)this.gridIndex.Gy, this.brightness, this.saturation);
					this.ModifyVertex(ref vert, rect, i);
					vh.SetUIVertex(vert, i);
				}
			}
		}

		// Token: 0x04007DF7 RID: 32247
		private static readonly HashSet<Canvas> _shaderChannelsEnabled = new HashSet<Canvas>();

		// Token: 0x04007DF8 RID: 32248
		private static readonly Dictionary<Material, Material> _normalMaterialBases = new Dictionary<Material, Material>();

		// Token: 0x04007DF9 RID: 32249
		private static readonly Dictionary<Material, Dictionary<Texture, Material>> _normalMaterials = new Dictionary<Material, Dictionary<Texture, Material>>();

		// Token: 0x04007DFA RID: 32250
		private static readonly SecondarySpriteTexture[] _secondaryTextures = new SecondarySpriteTexture[4];

		// Token: 0x04007DFB RID: 32251
		private static readonly int BumpMapId = Shader.PropertyToID("_BumpMap");

		// Token: 0x04007DFC RID: 32252
		private const string BumpMapName = "_BumpMap";

		// Token: 0x04007DFD RID: 32253
		[TupleElementNames(new string[]
		{
			"image",
			"material"
		})]
		private static readonly List<ValueTuple<Image, Material>> _pendingMaterialSets = new List<ValueTuple<Image, Material>>();

		// Token: 0x04007DFE RID: 32254
		private static readonly List<Image> _pendingMaterialRestores = new List<Image>();

		// Token: 0x04007DFF RID: 32255
		private static bool _willRenderRegistered;

		// Token: 0x04007E00 RID: 32256
		protected AdventureBlockIndex gridIndex = AdventureBlockIndex.Center;

		// Token: 0x04007E01 RID: 32257
		[SerializeField]
		protected float brightness = 1f;

		// Token: 0x04007E02 RID: 32258
		[SerializeField]
		protected float saturation = 1f;
	}
}
