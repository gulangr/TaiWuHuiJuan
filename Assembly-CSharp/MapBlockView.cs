using System;
using Config;
using GameData.Domains.Map;
using Map.RenderSystem;
using UnityEngine;

// Token: 0x02000254 RID: 596
[RequireComponent(typeof(CImage))]
public class MapBlockView : MonoBehaviour
{
	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06002771 RID: 10097 RVA: 0x00122F5C File Offset: 0x0012115C
	private CImage Image
	{
		get
		{
			bool flag = this._image == null;
			if (flag)
			{
				this._image = base.GetComponent<CImage>();
			}
			return this._image;
		}
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x00122F90 File Offset: 0x00121190
	public void Refresh(MapBlockData blockData, MapBlockData rootBlockData)
	{
		string spriteName = MapBlockView.GetMapBlockSpriteName(blockData, rootBlockData);
		CImage image = this.Image;
		bool hasVillagerWorkCorrectImage = MapBlockView.CheckMapBlockHasVillagerWorkCorrectImage(blockData, rootBlockData);
		bool flag = hasVillagerWorkCorrectImage;
		if (flag)
		{
			image.SetSprite(spriteName, true, null);
		}
		else
		{
			MapAtlasInfo.Instance.GetSprite(spriteName, delegate(Sprite sprite)
			{
				image.enabled = (sprite != null);
				image.sprite = sprite;
				image.SetNativeSize();
			});
		}
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x00122FF0 File Offset: 0x001211F0
	internal static string GetMapBlockSpriteName(MapBlockData blockData, MapBlockData rootBlockData)
	{
		return SingletonObject.getInstance<MapRenderSystem>().GetMapBlockSpriteNameByFix(rootBlockData ?? blockData);
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x00123014 File Offset: 0x00121214
	internal static bool CheckMapBlockHasVillagerWorkCorrectImage(MapBlockData blockData, MapBlockData rootBlockData)
	{
		MapBlockItem config = (rootBlockData == null) ? blockData.GetConfig() : rootBlockData.GetConfig();
		bool flag = config == null;
		return !flag && config.BlockHasFix;
	}

	// Token: 0x04001CAF RID: 7343
	private CImage _image;
}
