using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

// Token: 0x0200022D RID: 557
public static class ItemDisplayDataUtils
{
	// Token: 0x0600237A RID: 9082 RVA: 0x00104B4C File Offset: 0x00102D4C
	public static string GetUsingTypeName(this ItemDisplayData displayData)
	{
		string color = string.Empty;
		LanguageKey key = LanguageKey.EventEditor_Error_DuplicateGroupKey;
		bool isReadingFinished = displayData.IsReadingFinished;
		if (isReadingFinished)
		{
			key = LanguageKey.LK_ItemUsingType_ReadingFinished;
			color = "lightblue";
		}
		switch (displayData.UsingType)
		{
		case ItemDisplayData.ItemUsingType.Reading:
			key = LanguageKey.LK_ItemUsingType_Reading;
			color = "lightgreen";
			break;
		case ItemDisplayData.ItemUsingType.EquipmentPlaned:
			key = LanguageKey.LK_ItemUsingType_EquipmentPlaned;
			color = "yellow";
			break;
		case ItemDisplayData.ItemUsingType.Equiped:
			key = LanguageKey.LK_ItemUsingType_Equiping;
			color = "lightblue";
			break;
		case ItemDisplayData.ItemUsingType.Breeding:
			key = LanguageKey.LK_ItemUsingType_Breeding;
			color = "yellow";
			break;
		case ItemDisplayData.ItemUsingType.Referring:
			key = LanguageKey.LK_ItemUsingType_Referring;
			color = "lightgreen";
			break;
		}
		bool isInCurrentCricketPreset = displayData.IsInCurrentCricketPreset;
		if (isInCurrentCricketPreset)
		{
			key = LanguageKey.LK_ItemUsingType_CricketPreset;
			color = "yellow";
		}
		return LocalStringManager.Get(key).SetColor(color);
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x00104C18 File Offset: 0x00102E18
	public static string GetUsingOperationTypeName(this ItemDisplayData displayData, ItemDisplayData.ItemUsingOperationType usingOperationType)
	{
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (usingOperationType)
		{
		case ItemDisplayData.ItemUsingOperationType.Default:
			languageKey = LanguageKey.LK_ItemUsingOperationType_Default;
			break;
		case ItemDisplayData.ItemUsingOperationType.Sell:
			languageKey = LanguageKey.LK_ItemUsingOperationType_Sell;
			break;
		case ItemDisplayData.ItemUsingOperationType.Store:
			languageKey = LanguageKey.LK_ItemUsingOperationType_Store;
			break;
		case ItemDisplayData.ItemUsingOperationType.Present:
			languageKey = LanguageKey.LK_ItemUsingOperationType_Present;
			break;
		case ItemDisplayData.ItemUsingOperationType.Bet:
			languageKey = LanguageKey.LK_ItemUsingOperationType_Bet;
			break;
		case ItemDisplayData.ItemUsingOperationType.Give:
			languageKey = LanguageKey.LK_ItemUsingOperationType_Give;
			break;
		default:
			throw new ArgumentOutOfRangeException("usingOperationType", usingOperationType, null);
		}
		if (!true)
		{
		}
		LanguageKey key = languageKey;
		return LocalStringManager.Get(key);
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x00104CA0 File Offset: 0x00102EA0
	public static string GetUsingOperationConfirmTip(this ItemDisplayData displayData, ItemDisplayData.ItemUsingOperationType usingOperationType)
	{
		string usingTypeName = displayData.GetUsingTypeName();
		string itemTypeName = (displayData.Key.ItemType == 10) ? LocalStringManager.Get(LanguageKey.LK_ItemType_10) : LocalStringManager.Get(LanguageKey.LK_Item);
		string operationName = displayData.GetUsingOperationTypeName(usingOperationType);
		return LocalStringManager.GetFormat(LanguageKey.LK_ItemUsing_ConfirmTip, usingTypeName, itemTypeName, operationName).ColorReplace();
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x00104D00 File Offset: 0x00102F00
	public static bool GetOperationNeedConfirm(this ItemDisplayData displayData, ItemOperationType.EItemOperationType operationType)
	{
		return operationType == ItemOperationType.EItemOperationType.Disassemble || operationType == ItemOperationType.EItemOperationType.Discard || operationType == ItemOperationType.EItemOperationType.Transfer;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x00104D24 File Offset: 0x00102F24
	public static bool GetUsingTypeNeedConfirm(this ItemDisplayData displayData)
	{
		return displayData.UsingType != ItemDisplayData.ItemUsingType.Invalid;
	}
}
