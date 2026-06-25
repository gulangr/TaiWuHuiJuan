using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Utilities;
using Map.RenderSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.World
{
	// Token: 0x0200072A RID: 1834
	public class SimpleWorldMap : TemplatedContainerAssemblyNew
	{
		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x060057A3 RID: 22435 RVA: 0x0028B475 File Offset: 0x00289675
		public CToggleGroup InfoLayerToggleGroup
		{
			get
			{
				return this.infoLayerToggleGroup;
			}
		}

		// Token: 0x060057A4 RID: 22436 RVA: 0x0028B480 File Offset: 0x00289680
		private void Awake()
		{
			this.scale.OnScale = delegate(Vector3 _)
			{
				this.SyncMapLayer();
				this.dragMove.AdjustOffsetAfterScale();
			};
			this.scale.OnScaleFinish = new Action(this.SyncMapLayer);
			this.dragMove.EndDragCallback = new Action(this.SyncMapLayer);
		}

		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x060057A5 RID: 22437 RVA: 0x0028B4D3 File Offset: 0x002896D3
		public RectTransform InfoLayer
		{
			get
			{
				return this.infoLayer;
			}
		}

		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x060057A6 RID: 22438 RVA: 0x0028B4DB File Offset: 0x002896DB
		// (set) Token: 0x060057A7 RID: 22439 RVA: 0x0028B4E3 File Offset: 0x002896E3
		public Vector2 CachedMapContentSize { get; protected set; }

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x060057A8 RID: 22440 RVA: 0x0028B4EC File Offset: 0x002896EC
		public IDictionary<short, Vector2> OriginalBlockPosition
		{
			get
			{
				return this._originalBlockPosition;
			}
		}

		// Token: 0x060057A9 RID: 22441 RVA: 0x0028B4F4 File Offset: 0x002896F4
		public void RebuildMap(byte areaSize, IList<MapBlockData> blockList, Action<MapBlockData, Graphic> materialUpdate = null)
		{
			this.RebuildMap(areaSize, blockList, Vector2.zero, materialUpdate);
		}

		// Token: 0x060057AA RID: 22442 RVA: 0x0028B508 File Offset: 0x00289708
		public void RebuildMap(byte areaSize, IList<MapBlockData> blockList, Vector2 distanceAdditional, Action<MapBlockData, Graphic> materialUpdate = null)
		{
			SimpleWorldMap.<>c__DisplayClass23_0 CS$<>8__locals1 = new SimpleWorldMap.<>c__DisplayClass23_0();
			CS$<>8__locals1.areaSize = areaSize;
			CS$<>8__locals1.blockList = blockList;
			CS$<>8__locals1.materialUpdate = materialUpdate;
			CS$<>8__locals1.<>4__this = this;
			this._originalBlockPosition.Clear();
			this.mapLayer.sizeDelta = (this.infoLayer.sizeDelta = Vector2.zero);
			CS$<>8__locals1.rectTrans = this.template;
			CS$<>8__locals1.distance = CS$<>8__locals1.rectTrans.rect.size * CS$<>8__locals1.rectTrans.localScale * 0.5f + distanceAdditional;
			CS$<>8__locals1.mapCenterPos = Vector2.zero;
			CS$<>8__locals1.range = default(Rect);
			CollectionUtils.Sort<MapBlockData>(CS$<>8__locals1.blockList, delegate(MapBlockData a, MapBlockData b)
			{
				Vector2 vector;
				Vector2 posA = base.<RebuildMap>g__CalcBlockPosition|2(a, CS$<>8__locals1.mapCenterPos, CS$<>8__locals1.areaSize, out vector);
				Vector2 posB = base.<RebuildMap>g__CalcBlockPosition|2(b, CS$<>8__locals1.mapCenterPos, CS$<>8__locals1.areaSize, out vector);
				int diff = posB.y.CompareTo(posA.y);
				bool flag = diff != 0;
				int result;
				if (flag)
				{
					result = diff;
				}
				else
				{
					MapBlockItem cfgA = a.GetConfig();
					MapBlockItem cfgB = b.GetConfig();
					bool flag2 = cfgB != null && cfgA != null;
					if (flag2)
					{
						int diff2 = cfgA.Size.CompareTo(cfgB.Size);
						bool flag3 = diff2 != 0;
						if (flag3)
						{
							return diff2;
						}
					}
					result = posA.x.CompareTo(posB.x);
				}
				return result;
			});
			CS$<>8__locals1.modifying = new List<Transform>();
			base.Rebuild<Transform>(CS$<>8__locals1.blockList.Count, delegate(Transform blockPiece, int i)
			{
				SimpleWorldMap.<>c__DisplayClass23_1 CS$<>8__locals6 = new SimpleWorldMap.<>c__DisplayClass23_1();
				CS$<>8__locals6.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals6.blockPiece = blockPiece;
				CS$<>8__locals1.modifying.Add(CS$<>8__locals6.blockPiece);
				CS$<>8__locals6.blockData = CS$<>8__locals1.blockList[i];
				CS$<>8__locals6.blockPiece.name = CS$<>8__locals6.blockData.BlockId.ToString();
				MapBlockData rootBlockData = (CS$<>8__locals6.blockData.RootBlockId > -1) ? CS$<>8__locals1.blockList.First((MapBlockData x) => x.BlockId == CS$<>8__locals6.blockData.RootBlockId) : null;
				CS$<>8__locals6.blockPiece.gameObject.SetActive(CS$<>8__locals6.blockData.Visible && rootBlockData == null && CS$<>8__locals6.blockData.TemplateId != 126);
				MapBlockItem blockConfig = ((rootBlockData != null) ? rootBlockData.GetConfig() : null) ?? CS$<>8__locals6.blockData.GetConfig();
				MapRenderSystem renderSystem = SingletonObject.getInstance<MapRenderSystem>();
				bool hasFix = blockConfig.BlockHasFix;
				bool hasSingleFix = hasFix && blockConfig.Size <= 1;
				string spriteName = hasSingleFix ? renderSystem.GetMapBlockSpriteNameByFix(CS$<>8__locals6.blockData) : (hasFix ? renderSystem.GetMapBlockSpriteNameByFixWithFull(CS$<>8__locals6.blockData) : ((!string.IsNullOrWhiteSpace(blockConfig.Art)) ? renderSystem.GetMapBlockSpriteName(CS$<>8__locals6.blockData) : ""));
				bool flag = string.IsNullOrWhiteSpace(spriteName);
				if (flag)
				{
					CS$<>8__locals6.blockPiece.gameObject.GetOrAddComponent<CImage>().enabled = false;
				}
				else
				{
					bool flag2 = hasSingleFix;
					if (flag2)
					{
						AtlasInfo.Instance.GetSprite(spriteName, new Action<Sprite>(CS$<>8__locals6.<RebuildMap>g__OnGetSprite|5));
					}
					else
					{
						bool flag3 = hasFix;
						if (flag3)
						{
							ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/MapBlockFixFullSize", spriteName), delegate(Texture2D tex)
							{
								CImage origin = CS$<>8__locals6.blockPiece.GetComponent<CImage>();
								bool hasOrigin = origin != null;
								UnityEngine.Material material = null;
								bool flag5 = hasOrigin;
								if (flag5)
								{
									material = origin.material;
									Object.DestroyImmediate(origin);
								}
								CRawImage cRawImage = CS$<>8__locals6.blockPiece.gameObject.GetOrAddComponent<CRawImage>();
								cRawImage.texture = tex;
								bool flag6 = hasOrigin;
								if (flag6)
								{
									cRawImage.material = material;
								}
								cRawImage.SetNativeSize();
								Action<MapBlockData, Graphic> materialUpdate2 = CS$<>8__locals6.CS$<>8__locals1.materialUpdate;
								if (materialUpdate2 != null)
								{
									materialUpdate2(CS$<>8__locals6.blockData, cRawImage);
								}
							}, null, false);
						}
						else
						{
							MapAtlasInfo.Instance.GetSprite(spriteName, new Action<Sprite>(CS$<>8__locals6.<RebuildMap>g__OnGetSprite|5));
						}
					}
				}
				Vector2 originPosition;
				Vector2 position = base.<RebuildMap>g__CalcBlockPosition|2(CS$<>8__locals6.blockData, CS$<>8__locals1.mapCenterPos, CS$<>8__locals1.areaSize, out originPosition);
				CS$<>8__locals6.blockPiece.transform.localPosition = position;
				CS$<>8__locals1.<>4__this._originalBlockPosition[CS$<>8__locals6.blockData.BlockId] = originPosition;
				bool flag4 = !CS$<>8__locals6.blockData.Visible && !CS$<>8__locals1.<>4__this.sizeContainsInvisibleBlock;
				if (!flag4)
				{
					CS$<>8__locals1.range.xMax = Math.Max(CS$<>8__locals1.range.xMax, position.x);
					CS$<>8__locals1.range.yMax = Math.Max(CS$<>8__locals1.range.yMax, position.y);
					CS$<>8__locals1.range.xMin = Math.Min(CS$<>8__locals1.range.xMin, position.x);
					CS$<>8__locals1.range.yMin = Math.Min(CS$<>8__locals1.range.yMin, position.y);
				}
			});
			SimpleWorldMap.<>c__DisplayClass23_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.range.xMax = CS$<>8__locals2.range.xMax + this.xMax;
			SimpleWorldMap.<>c__DisplayClass23_0 CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals3.range.xMin = CS$<>8__locals3.range.xMin + this.xMin;
			SimpleWorldMap.<>c__DisplayClass23_0 CS$<>8__locals4 = CS$<>8__locals1;
			CS$<>8__locals4.range.yMax = CS$<>8__locals4.range.yMax + this.yMax;
			SimpleWorldMap.<>c__DisplayClass23_0 CS$<>8__locals5 = CS$<>8__locals1;
			CS$<>8__locals5.range.yMin = CS$<>8__locals5.range.yMin + this.yMin;
			Vector2 mapContentSize = new Vector2(CS$<>8__locals1.range.xMax - CS$<>8__locals1.range.xMin, CS$<>8__locals1.range.yMax - CS$<>8__locals1.range.yMin);
			this.mapLayer.localPosition = Vector3.zero;
			this.mapLayer.sizeDelta = (this.infoLayer.sizeDelta = mapContentSize);
			Vector2 delta = new Vector2(CS$<>8__locals1.range.xMin, CS$<>8__locals1.range.yMin + mapContentSize.y * 0.5f);
			foreach (Transform item in CS$<>8__locals1.modifying)
			{
				item.transform.localPosition = item.transform.localPosition - delta;
			}
			this.CachedMapContentSize = mapContentSize;
		}

		// Token: 0x060057AB RID: 22443 RVA: 0x0028B770 File Offset: 0x00289970
		public void ResetScale(Vector3 scale)
		{
			this.mapLayer.pivot = new Vector2(0.5f, 0.5f);
			this.mapLayer.localScale = scale;
			this.SyncMapLayer();
		}

		// Token: 0x060057AC RID: 22444 RVA: 0x0028B7A4 File Offset: 0x002899A4
		public void LookAtBlock(short blockId)
		{
			RectTransform block = this.FindBlock(blockId);
			this.LookAt(-block.localPosition * this.mapLayer.localScale.x);
		}

		// Token: 0x060057AD RID: 22445 RVA: 0x0028B7E4 File Offset: 0x002899E4
		public void AdventureRemakeLookAtBlock(short blockId, Vector3 localOffset, float scale)
		{
			SimpleWorldMap.<>c__DisplayClass26_0 CS$<>8__locals1;
			CS$<>8__locals1.localOffset = localOffset;
			CS$<>8__locals1.scale = scale;
			Rect rect = ((RectTransform)base.transform).rect;
			Vector3 pos = -this.FindBlock(blockId).localPosition;
			pos *= CS$<>8__locals1.scale;
			float offsetX = SimpleWorldMap.<AdventureRemakeLookAtBlock>g__GetOffsetX|26_0(ref CS$<>8__locals1);
			float offsetY = SimpleWorldMap.<AdventureRemakeLookAtBlock>g__GetOffsetY|26_1(ref CS$<>8__locals1);
			pos += new Vector3(offsetX, offsetY);
			this.LookAt(pos);
		}

		// Token: 0x060057AE RID: 22446 RVA: 0x0028B85C File Offset: 0x00289A5C
		public void AdventureRemakeLookAtBlock(Vector3 localPosition, Vector3 localOffset, float scale)
		{
			SimpleWorldMap.<>c__DisplayClass27_0 CS$<>8__locals1;
			CS$<>8__locals1.localOffset = localOffset;
			CS$<>8__locals1.scale = scale;
			Rect rect = ((RectTransform)base.transform).rect;
			Vector3 pos = -localPosition + new Vector3(rect.width / 2f, -rect.height);
			pos *= CS$<>8__locals1.scale;
			float offsetX = SimpleWorldMap.<AdventureRemakeLookAtBlock>g__GetOffsetX|27_0(ref CS$<>8__locals1);
			float offsetY = SimpleWorldMap.<AdventureRemakeLookAtBlock>g__GetOffsetY|27_1(ref CS$<>8__locals1);
			pos += new Vector3(offsetX, offsetY);
			this.LookAt(pos);
		}

		// Token: 0x060057AF RID: 22447 RVA: 0x0028B8E8 File Offset: 0x00289AE8
		public void LookAt(Vector3 pos)
		{
			this.mapLayer.localPosition = pos;
			this.SyncMapLayer();
			this.dragMove.AdjustOffsetAfterScale();
		}

		// Token: 0x060057B0 RID: 22448 RVA: 0x0028B90B File Offset: 0x00289B0B
		public void LookAt(Vector2 pos)
		{
			this.mapLayer.anchoredPosition = pos;
			this.SyncMapLayer();
			this.dragMove.AdjustOffsetAfterScale();
		}

		// Token: 0x060057B1 RID: 22449 RVA: 0x0028B92E File Offset: 0x00289B2E
		public void LookAtGlobal(Vector3 pos, Vector3 localOffset)
		{
			this.mapLayer.position = pos;
			this.mapLayer.localPosition += localOffset;
			this.SyncMapLayer();
			this.dragMove.AdjustOffsetAfterScale();
		}

		// Token: 0x060057B2 RID: 22450 RVA: 0x0028B969 File Offset: 0x00289B69
		public RectTransform FindBlock(short blockId)
		{
			return (RectTransform)this.mapLayer.Find(blockId.ToString());
		}

		// Token: 0x060057B3 RID: 22451 RVA: 0x0028B984 File Offset: 0x00289B84
		private void SyncMapLayer()
		{
			this.mapLayer.SetPivot(new Vector2(0.5f, 0.5f));
			this.infoLayer.localPosition = this.mapLayer.localPosition;
			this.infoLayer.localScale = this.mapLayer.localScale;
			this.infoLayer.pivot = this.mapLayer.pivot;
		}

		// Token: 0x060057B6 RID: 22454 RVA: 0x0028BA28 File Offset: 0x00289C28
		[CompilerGenerated]
		internal static float <AdventureRemakeLookAtBlock>g__GetOffsetX|26_0(ref SimpleWorldMap.<>c__DisplayClass26_0 A_0)
		{
			return Mathf.Lerp(A_0.localOffset.x / 0.5f, A_0.localOffset.x, (A_0.scale - 0.5f) / 0.5f);
		}

		// Token: 0x060057B7 RID: 22455 RVA: 0x0028BA70 File Offset: 0x00289C70
		[CompilerGenerated]
		internal static float <AdventureRemakeLookAtBlock>g__GetOffsetY|26_1(ref SimpleWorldMap.<>c__DisplayClass26_0 A_0)
		{
			return Mathf.Lerp(0f, A_0.localOffset.y, (A_0.scale - 0.5f) / 0.5f);
		}

		// Token: 0x060057B8 RID: 22456 RVA: 0x0028BAAC File Offset: 0x00289CAC
		[CompilerGenerated]
		internal static float <AdventureRemakeLookAtBlock>g__GetOffsetX|27_0(ref SimpleWorldMap.<>c__DisplayClass27_0 A_0)
		{
			return Mathf.Lerp(A_0.localOffset.x / 0.5f, 0f, (A_0.scale - 0.5f) / 0.5f);
		}

		// Token: 0x060057B9 RID: 22457 RVA: 0x0028BAEC File Offset: 0x00289CEC
		[CompilerGenerated]
		internal static float <AdventureRemakeLookAtBlock>g__GetOffsetY|27_1(ref SimpleWorldMap.<>c__DisplayClass27_0 A_0)
		{
			return Mathf.Lerp(A_0.localOffset.y * 0.5f, A_0.localOffset.y * 2f, (A_0.scale - 0.5f) / 0.5f);
		}

		// Token: 0x04003C22 RID: 15394
		[SerializeField]
		private RectTransform mapLayer;

		// Token: 0x04003C23 RID: 15395
		[SerializeField]
		private RectTransform infoLayer;

		// Token: 0x04003C24 RID: 15396
		[SerializeField]
		private bool sizeContainsInvisibleBlock;

		// Token: 0x04003C25 RID: 15397
		[SerializeField]
		private UIRectDragMove dragMove;

		// Token: 0x04003C26 RID: 15398
		[SerializeField]
		internal MouseWheelScale scale;

		// Token: 0x04003C27 RID: 15399
		[SerializeField]
		private CToggleGroup infoLayerToggleGroup;

		// Token: 0x04003C28 RID: 15400
		[SerializeField]
		private float xMax;

		// Token: 0x04003C29 RID: 15401
		[SerializeField]
		private float xMin;

		// Token: 0x04003C2A RID: 15402
		[SerializeField]
		private float yMax = 200f;

		// Token: 0x04003C2B RID: 15403
		[SerializeField]
		private float yMin;

		// Token: 0x04003C2C RID: 15404
		private readonly Dictionary<short, Vector2> _originalBlockPosition = new Dictionary<short, Vector2>();
	}
}
