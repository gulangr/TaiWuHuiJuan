using System;
using Config;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EDD RID: 3805
	public class RefineAttribute : MonoBehaviour, ICellContent<ItemDisplayData>, ICellContent
	{
		// Token: 0x0600AF32 RID: 44850 RVA: 0x004FD220 File Offset: 0x004FB420
		public void SetData(ItemDisplayData target)
		{
			short[] allMaterialTemplateIds = target.RefiningEffects.GetAllMaterialTemplateIds();
			sbyte targetItemType = target.RealKey.ItemType;
			for (int i = 0; i < 5; i++)
			{
				CImage icon = this.iconArray[i];
				short materialTemplateId = allMaterialTemplateIds[i];
				bool flag = materialTemplateId < 0;
				if (flag)
				{
					icon.SetSprite("ui9_icon_item_empty_small", false, null);
				}
				else
				{
					MaterialItem materialConfig = Config.Material.Instance[materialTemplateId];
					RefiningEffectItem refineConfig = RefiningEffect.Instance[materialConfig.RefiningEffect];
					if (!true)
					{
					}
					sbyte b;
					switch (targetItemType)
					{
					case 0:
						b = (sbyte)refineConfig.WeaponType;
						break;
					case 1:
						b = (sbyte)refineConfig.ArmorType;
						break;
					case 2:
						b = (sbyte)refineConfig.AccessoryType;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!true)
					{
					}
					sbyte propertyType = b;
					icon.SetSprite(TipsRefiningEffect.RefiningIconName[(int)targetItemType][(int)propertyType], false, null);
				}
			}
		}

		// Token: 0x040087BA RID: 34746
		[SerializeField]
		private CImage[] iconArray = new CImage[5];
	}
}
