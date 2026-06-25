using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Relation;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B61 RID: 2913
	public class CharacterMenuCharacterTransferInfo : MonoBehaviour
	{
		// Token: 0x0600904E RID: 36942 RVA: 0x00433DC0 File Offset: 0x00431FC0
		public bool SetData(ItemDisplayData itemData, CharacterMenuCharacterTransferInfo.TransferData transferData)
		{
			bool flag = transferData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.desc.gameObject.SetActive(itemData != null);
				bool flag2 = itemData != null;
				if (flag2)
				{
					string itemName = itemData.GetName(false);
					this.desc.SetText(LocalStringManager.GetFormat(LanguageKey.LK_CharacterMenu_Items_Transfer_Tips, itemName), true);
				}
				sbyte originalAlertnessLevel = CharacterAlertnessData.GetLevel(transferData.OriginalAlertness);
				sbyte finalAlertnessLevel = CharacterAlertnessData.GetLevel(transferData.FinalAlertness);
				bool showAlertness = originalAlertnessLevel != finalAlertnessLevel;
				this.changeAlertness.SetActive(showAlertness);
				bool flag3 = showAlertness;
				if (flag3)
				{
					string iconPreview = CommonUtils.GetAlertnessIcon((int)finalAlertnessLevel);
					this.changeAlertnessIcon.SetSprite(iconPreview, false, null);
					bool isPositive = finalAlertnessLevel < originalAlertnessLevel;
					string changeStr = LocalStringManager.Get(isPositive ? LanguageKey.LK_EventLog_Result_Down : LanguageKey.LK_EventLog_Result_Up).SetColor(isPositive ? this.positiveColor : this.negativeColor);
					this.changeAlertnessDesc.SetText(LocalStringManager.Get(LanguageKey.LK_Character_Alertness) + changeStr, true);
				}
				short originalFavor = transferData.OriginalFavor;
				sbyte originalFavorLevel = FavorabilityType.GetFavorabilityType(originalFavor);
				short finalFavor = transferData.FinalFavor;
				sbyte finalFavorLevel = FavorabilityType.GetFavorabilityType(finalFavor);
				bool showFavor = originalFavorLevel != finalFavorLevel;
				this.changeFavor.SetActive(showFavor);
				bool flag4 = showFavor;
				if (flag4)
				{
					string iconPreview2 = CommonUtils.GetFavorabilityIconName(finalFavor, true);
					this.changeFavorIcon.SetSprite(iconPreview2, false, null);
					bool isPositive2 = finalFavorLevel > originalFavorLevel;
					string changeStr2 = LocalStringManager.Get(isPositive2 ? LanguageKey.LK_EventLog_Result_Up : LanguageKey.LK_EventLog_Result_Down).SetColor(isPositive2 ? this.positiveColor : this.negativeColor);
					this.changeFavorDesc.SetText(LocalStringManager.Get(LanguageKey.LK_EventLog_Favorability) + changeStr2, true);
				}
				sbyte originalHappiness = transferData.OriginalHappiness;
				sbyte originalHappinessLevel = HappinessType.GetHappinessType(originalHappiness);
				sbyte finalHappiness = transferData.FinalHappiness;
				sbyte finalHappinessLevel = HappinessType.GetHappinessType(finalHappiness);
				bool showHappiness = originalHappinessLevel != finalHappinessLevel;
				this.changeHappiness.SetActive(showHappiness);
				bool flag5 = showHappiness;
				if (flag5)
				{
					string iconPreview3 = CommonUtils.GetHappinessIconName(finalHappinessLevel);
					this.changeHappinessIcon.SetSprite(iconPreview3, false, null);
					bool isPositive3 = finalHappinessLevel > originalHappinessLevel;
					string changeStr3 = LocalStringManager.Get(isPositive3 ? LanguageKey.LK_EventLog_Result_Up : LanguageKey.LK_EventLog_Result_Down).SetColor(isPositive3 ? this.positiveColor : this.negativeColor);
					this.changeHappinessDesc.SetText(LocalStringManager.Get(LanguageKey.LK_EventLog_Favorability) + changeStr3, true);
				}
				result = (this.desc.gameObject.activeSelf || this.changeAlertness.activeSelf || this.changeFavor.activeSelf || this.changeHappiness.activeSelf);
			}
			return result;
		}

		// Token: 0x04006EEA RID: 28394
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04006EEB RID: 28395
		[SerializeField]
		private GameObject changeAlertness;

		// Token: 0x04006EEC RID: 28396
		[SerializeField]
		private CImage changeAlertnessIcon;

		// Token: 0x04006EED RID: 28397
		[SerializeField]
		private TextMeshProUGUI changeAlertnessDesc;

		// Token: 0x04006EEE RID: 28398
		[SerializeField]
		private GameObject changeFavor;

		// Token: 0x04006EEF RID: 28399
		[SerializeField]
		private CImage changeFavorIcon;

		// Token: 0x04006EF0 RID: 28400
		[SerializeField]
		private TextMeshProUGUI changeFavorDesc;

		// Token: 0x04006EF1 RID: 28401
		[SerializeField]
		private GameObject changeHappiness;

		// Token: 0x04006EF2 RID: 28402
		[SerializeField]
		private CImage changeHappinessIcon;

		// Token: 0x04006EF3 RID: 28403
		[SerializeField]
		private TextMeshProUGUI changeHappinessDesc;

		// Token: 0x04006EF4 RID: 28404
		[SerializeField]
		private Color positiveColor;

		// Token: 0x04006EF5 RID: 28405
		[SerializeField]
		private Color negativeColor;

		// Token: 0x02002179 RID: 8569
		public class TransferData
		{
			// Token: 0x0600FB3F RID: 64319 RVA: 0x006374B8 File Offset: 0x006356B8
			public override string ToString()
			{
				return string.Format("OriginalFavor = {0} FinalFavor = {1} ", this.OriginalFavor, this.FinalFavor) + string.Format("OriginalHappiness = {0} FinalHappiness = {1} ", this.OriginalHappiness, this.FinalHappiness) + string.Format("OriginalAlertness = {0} FinalAlertness = {1}", this.OriginalAlertness, this.FinalAlertness);
			}

			// Token: 0x0400D5F5 RID: 54773
			public short OriginalFavor;

			// Token: 0x0400D5F6 RID: 54774
			public sbyte OriginalHappiness;

			// Token: 0x0400D5F7 RID: 54775
			public int OriginalAlertness;

			// Token: 0x0400D5F8 RID: 54776
			public short FinalFavor;

			// Token: 0x0400D5F9 RID: 54777
			public sbyte FinalHappiness;

			// Token: 0x0400D5FA RID: 54778
			public int FinalAlertness;
		}
	}
}
