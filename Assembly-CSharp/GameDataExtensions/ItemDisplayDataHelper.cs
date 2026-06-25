using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Config.ConfigCells.Character;
using Game.Views.Make;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.World.Task;
using GameData.Utilities;

namespace GameDataExtensions
{
	// Token: 0x020006D6 RID: 1750
	public static class ItemDisplayDataHelper
	{
		// Token: 0x0600536A RID: 21354 RVA: 0x0026B23C File Offset: 0x0026943C
		public static string GetName(this ITradeableContent itemDisplayData, bool withGradeColor = false)
		{
			NameRelatedData nameData;
			bool flag;
			if (itemDisplayData.CharacterId != -1)
			{
				nameData = itemDisplayData.NameRelatedData;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			string result;
			if (flag2)
			{
				result = NameCenter.GetMonasticTitleOrDisplayName(ref nameData, false, false);
			}
			else
			{
				bool isInvalid = itemDisplayData.Key.Equals(ItemKey.Invalid);
				bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(itemDisplayData);
				bool flag3 = isRandomMake;
				if (flag3)
				{
					isInvalid = true;
				}
				bool isCricket = itemDisplayData.Key.ItemType == 11;
				bool flag4 = ItemTemplateHelper.IsJiao(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId) && !ItemTemplateHelper.IsJiaoEgg(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId) && itemDisplayData.JiaoLoongDisplayData != null;
				string name;
				if (flag4)
				{
					name = itemDisplayData.JiaoLoongDisplayData.JiaoLoongNameRelatedData.GetName();
				}
				else
				{
					bool isResource = itemDisplayData.IsResource;
					if (isResource)
					{
						sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
						ResourceTypeItem resourceTypeConfig = ResourceType.Instance[resourceType];
						name = resourceTypeConfig.Name;
					}
					else
					{
						name = (isInvalid ? ((UIManager.Instance.IsFocusElement(UIElement.MakeOld) || UIManager.Instance.IsFocusElement(UIElement.Make)) ? LocalStringManager.Get(LanguageKey.LK_Make_None_Tool) : LocalStringManager.Get(LanguageKey.LK_None)) : (isCricket ? itemDisplayData.CalcCricketName() : ItemTemplateHelper.GetName(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId)));
					}
				}
				if (withGradeColor)
				{
					sbyte grade = ItemTemplateHelper.GetGrade(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
					name = name.SetColor(Colors.Instance.GradeColors[(int)grade]);
				}
				result = name;
			}
			return result;
		}

		// Token: 0x0600536B RID: 21355 RVA: 0x0026B408 File Offset: 0x00269608
		public static bool IsItemLockedByTask(ItemKey itemData)
		{
			List<int> locks = itemData.GetConfig().TaskLock;
			return locks != null && locks.Count > 0 && SingletonObject.getInstance<TaskModel>().GetData().Any((TaskDisplayData task) => (task.InnerTaskData.TaskStatus & 6) == 0 && locks.Contains(task.InnerTaskData.TaskChainId));
		}

		// Token: 0x0600536C RID: 21356 RVA: 0x0026B460 File Offset: 0x00269660
		public static bool IsItemLockedByTask(ITradeableContent itemData)
		{
			return itemData.Key.IsValid() && ItemDisplayDataHelper.IsItemLockedByTask(itemData.Key);
		}

		// Token: 0x0600536D RID: 21357 RVA: 0x0026B48C File Offset: 0x0026968C
		public static bool ShouldShowPower(this ItemDisplayData displayData)
		{
			bool flag = !displayData.PowerInfo.AnyValue;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				IReadOnlyList<PropertyAndValue> requirements = ItemDisplayDataHelper.CalcRequirements(displayData.Key);
				result = (requirements != null && requirements.Count > 0);
			}
			return result;
		}

		// Token: 0x0600536E RID: 21358 RVA: 0x0026B4D0 File Offset: 0x002696D0
		public static IReadOnlyList<PropertyAndValue> CalcRequirements(ItemKey itemKey)
		{
			sbyte itemType = itemKey.ItemType;
			if (!true)
			{
			}
			List<PropertyAndValue> result;
			switch (itemType)
			{
			case 0:
				result = Weapon.Instance[itemKey.TemplateId].RequiredCharacterProperties;
				break;
			case 1:
				result = Armor.Instance[itemKey.TemplateId].RequiredCharacterProperties;
				break;
			case 2:
				result = Accessory.Instance[itemKey.TemplateId].RequiredCharacterProperties;
				break;
			default:
				result = null;
				break;
			}
			if (!true)
			{
			}
			return result;
		}
	}
}
