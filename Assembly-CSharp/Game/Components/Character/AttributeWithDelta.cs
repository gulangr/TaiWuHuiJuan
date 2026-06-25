using System;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F10 RID: 3856
	public class AttributeWithDelta : MonoBehaviour
	{
		// Token: 0x0600B1C8 RID: 45512 RVA: 0x0050F81E File Offset: 0x0050DA1E
		public void Set(CharacterAttributeDisplayData baseVal, CharacterAttributeDisplayData deltaVal, CharacterAttributeDisplayData colorData = null)
		{
			this._colorData = colorData;
			this.self.Set(baseVal);
			this.Set(deltaVal);
		}

		// Token: 0x0600B1C9 RID: 45513 RVA: 0x0050F840 File Offset: 0x0050DA40
		public void Set(CharacterAttributeDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetDeltaEmpty();
			}
			else
			{
				this._currentDeltaData = data;
				this.RefreshDeltaMainAttributes();
				this.RefreshDeltaPenetrations();
				this.RefreshDeltaPenetrationResists();
				this.RefreshDeltaHitValues();
				this.RefreshDeltaAvoidValues();
				this.RefreshDeltaSecondaryAttributes();
			}
		}

		// Token: 0x0600B1CA RID: 45514 RVA: 0x0050F894 File Offset: 0x0050DA94
		public void SetDeltaEmpty()
		{
			this._currentDeltaData = null;
			bool flag = this.deltaMainAttributeItems != null;
			if (flag)
			{
				foreach (TMP_Text item in this.deltaMainAttributeItems)
				{
					item.text = "";
				}
			}
			bool flag2 = this.deltaPenetrationOuter != null;
			if (flag2)
			{
				this.deltaPenetrationOuter.text = "";
			}
			bool flag3 = this.deltaPenetrationInner != null;
			if (flag3)
			{
				this.deltaPenetrationInner.text = "";
			}
			bool flag4 = this.deltaPenetrationResistOuter != null;
			if (flag4)
			{
				this.deltaPenetrationResistOuter.text = "";
			}
			bool flag5 = this.deltaPenetrationResistOuter != null;
			if (flag5)
			{
				this.deltaPenetrationResistOuter.text = "";
			}
			bool flag6 = this.deltaHitValueItems != null;
			if (flag6)
			{
				foreach (TMP_Text item2 in this.deltaHitValueItems)
				{
					item2.text = "";
				}
			}
			bool flag7 = this.deltaAvoidValueItems != null;
			if (flag7)
			{
				foreach (TMP_Text item3 in this.deltaAvoidValueItems)
				{
					item3.text = "";
				}
			}
			bool flag8 = this.deltaRecoveryOfStance != null;
			if (flag8)
			{
				this.deltaRecoveryOfStance.text = "";
			}
			bool flag9 = this.deltaRecoveryOfBreath != null;
			if (flag9)
			{
				this.deltaRecoveryOfBreath.text = "";
			}
			bool flag10 = this.deltaAttackSpeed != null;
			if (flag10)
			{
				this.deltaAttackSpeed.text = "";
			}
			bool flag11 = this.deltaMoveSpeed != null;
			if (flag11)
			{
				this.deltaMoveSpeed.text = "";
			}
			bool flag12 = this.deltaWeaponSwitchSpeed != null;
			if (flag12)
			{
				this.deltaWeaponSwitchSpeed.text = "";
			}
			bool flag13 = this.deltaCastSpeed != null;
			if (flag13)
			{
				this.deltaCastSpeed.text = "";
			}
			bool flag14 = this.deltaRecoveryOfFlaw != null;
			if (flag14)
			{
				this.deltaRecoveryOfFlaw.text = "";
			}
			bool flag15 = this.deltaRecoveryOfBlockedAcupoint != null;
			if (flag15)
			{
				this.deltaRecoveryOfBlockedAcupoint.text = "";
			}
			bool flag16 = this.deltaRecoveryOfQiDisorder != null;
			if (flag16)
			{
				this.deltaRecoveryOfQiDisorder.text = "";
			}
			bool flag17 = this.deltaInnerRatio != null;
			if (flag17)
			{
				this.deltaInnerRatio.text = "";
			}
		}

		// Token: 0x0600B1CB RID: 45515 RVA: 0x0050FB44 File Offset: 0x0050DD44
		private static void SetText(TMP_Text text, int item, int color)
		{
			bool flag = text == null;
			if (!flag)
			{
				string content = (item > 0) ? LanguageKey.LK_CharacterMenuEquip_Delta_Plus.TrFormat(item) : ((item < 0) ? LanguageKey.LK_CharacterMenuEquip_Delta_Minus.TrFormat(-item) : "");
				text.text = ((color != 0 && item != 0) ? content.SetColor((item > 0) ? "brightblue" : "brightred") : content);
			}
		}

		// Token: 0x0600B1CC RID: 45516 RVA: 0x0050FBB8 File Offset: 0x0050DDB8
		private static void SetPercentText(TMP_Text text, int item, int color)
		{
			bool flag = text == null;
			if (!flag)
			{
				string content = (item > 0) ? LanguageKey.LK_CharacterMenuEquip_DeltaPercent_Plus.TrFormat(item) : ((item < 0) ? LanguageKey.LK_CharacterMenuEquip_DeltaPercent_Minus.TrFormat(-item) : "");
				text.text = ((color != 0 && item != 0) ? content.SetColor((item > 0) ? "brightblue" : "brightred") : content);
			}
		}

		// Token: 0x0600B1CD RID: 45517 RVA: 0x0050FC2C File Offset: 0x0050DE2C
		private unsafe void RefreshDeltaMainAttributes()
		{
			bool flag = this.deltaMainAttributeItems == null || this._currentDeltaData == null;
			if (!flag)
			{
				int[] displayOrder = new int[]
				{
					0,
					3,
					1,
					4,
					2,
					5
				};
				int i = 0;
				while (i < this.deltaMainAttributeItems.Length && i < displayOrder.Length)
				{
					bool flag2 = this.deltaMainAttributeItems[i] == null;
					if (!flag2)
					{
						int attrIndex = displayOrder[i];
						TMP_Text text = this.deltaMainAttributeItems[i];
						int item = (int)(*this._currentDeltaData.MaxMainAttributes[attrIndex]);
						CharacterAttributeDisplayData colorData = this._colorData;
						AttributeWithDelta.SetText(text, item, ((int)((colorData != null) ? new short?(*colorData.MaxMainAttributes[attrIndex]) : null)) ?? int.MinValue);
					}
					i++;
				}
			}
		}

		// Token: 0x0600B1CE RID: 45518 RVA: 0x0050FD10 File Offset: 0x0050DF10
		private void RefreshDeltaPenetrations()
		{
			bool flag = this._currentDeltaData == null;
			if (!flag)
			{
				TMP_Text text = this.deltaPenetrationOuter;
				int outer = this._currentDeltaData.AtkPenetrability.Outer;
				CharacterAttributeDisplayData colorData = this._colorData;
				AttributeWithDelta.SetText(text, outer, (colorData != null) ? colorData.AtkPenetrability.Outer : int.MinValue);
				TMP_Text text2 = this.deltaPenetrationInner;
				int inner = this._currentDeltaData.AtkPenetrability.Inner;
				CharacterAttributeDisplayData colorData2 = this._colorData;
				AttributeWithDelta.SetText(text2, inner, (colorData2 != null) ? colorData2.AtkPenetrability.Inner : int.MinValue);
			}
		}

		// Token: 0x0600B1CF RID: 45519 RVA: 0x0050FD9C File Offset: 0x0050DF9C
		private void RefreshDeltaPenetrationResists()
		{
			bool flag = this._currentDeltaData == null;
			if (!flag)
			{
				TMP_Text text = this.deltaPenetrationResistOuter;
				int outer = this._currentDeltaData.DefPenetrability.Outer;
				CharacterAttributeDisplayData colorData = this._colorData;
				AttributeWithDelta.SetText(text, outer, (colorData != null) ? colorData.DefPenetrability.Outer : int.MinValue);
				TMP_Text text2 = this.deltaPenetrationResistInner;
				int inner = this._currentDeltaData.DefPenetrability.Inner;
				CharacterAttributeDisplayData colorData2 = this._colorData;
				AttributeWithDelta.SetText(text2, inner, (colorData2 != null) ? colorData2.DefPenetrability.Inner : int.MinValue);
			}
		}

		// Token: 0x0600B1D0 RID: 45520 RVA: 0x0050FE28 File Offset: 0x0050E028
		private void RefreshDeltaHitValues()
		{
			bool flag = this.deltaHitValueItems == null || this._currentDeltaData == null;
			if (!flag)
			{
				sbyte i = 0;
				while ((int)i < this.deltaHitValueItems.Length && i < 4)
				{
					bool flag2 = this.deltaHitValueItems[(int)i] == null;
					if (!flag2)
					{
						TMP_Text text = this.deltaHitValueItems[(int)i];
						int item = this._currentDeltaData.AtkHitAttribute[(int)i];
						CharacterAttributeDisplayData colorData = this._colorData;
						AttributeWithDelta.SetText(text, item, (colorData != null) ? colorData.AtkHitAttribute[(int)i] : int.MinValue);
					}
					i += 1;
				}
			}
		}

		// Token: 0x0600B1D1 RID: 45521 RVA: 0x0050FEC0 File Offset: 0x0050E0C0
		private void RefreshDeltaAvoidValues()
		{
			bool flag = this.deltaAvoidValueItems == null || this._currentDeltaData == null;
			if (!flag)
			{
				sbyte i = 0;
				while ((int)i < this.deltaAvoidValueItems.Length && i < 4)
				{
					bool flag2 = this.deltaAvoidValueItems[(int)i] == null;
					if (!flag2)
					{
						TMP_Text text = this.deltaAvoidValueItems[(int)i];
						int item = this._currentDeltaData.DefHitAttribute[(int)i];
						CharacterAttributeDisplayData colorData = this._colorData;
						AttributeWithDelta.SetText(text, item, (colorData != null) ? colorData.DefHitAttribute[(int)i] : int.MinValue);
					}
					i += 1;
				}
			}
		}

		// Token: 0x0600B1D2 RID: 45522 RVA: 0x0050FF58 File Offset: 0x0050E158
		private void RefreshDeltaSecondaryAttributes()
		{
			bool flag = this._currentDeltaData == null;
			if (!flag)
			{
				TMP_Text text = this.deltaRecoveryOfStance;
				int outer = (int)this._currentDeltaData.RecoveryOfStanceAndBreath.Outer;
				CharacterAttributeDisplayData colorData = this._colorData;
				AttributeWithDelta.SetPercentText(text, outer, ((int)((colorData != null) ? new short?(colorData.RecoveryOfStanceAndBreath.Outer) : null)) ?? int.MinValue);
				TMP_Text text2 = this.deltaRecoveryOfBreath;
				int inner = (int)this._currentDeltaData.RecoveryOfStanceAndBreath.Inner;
				CharacterAttributeDisplayData colorData2 = this._colorData;
				AttributeWithDelta.SetPercentText(text2, inner, ((int)((colorData2 != null) ? new short?(colorData2.RecoveryOfStanceAndBreath.Inner) : null)) ?? int.MinValue);
				TMP_Text text3 = this.deltaAttackSpeed;
				int attackSpeed = (int)this._currentDeltaData.AttackSpeed;
				CharacterAttributeDisplayData colorData3 = this._colorData;
				AttributeWithDelta.SetPercentText(text3, attackSpeed, ((int)((colorData3 != null) ? new short?(colorData3.AttackSpeed) : null)) ?? int.MinValue);
				TMP_Text text4 = this.deltaMoveSpeed;
				int moveSpeed = (int)this._currentDeltaData.MoveSpeed;
				CharacterAttributeDisplayData colorData4 = this._colorData;
				AttributeWithDelta.SetPercentText(text4, moveSpeed, ((int)((colorData4 != null) ? new short?(colorData4.MoveSpeed) : null)) ?? int.MinValue);
				TMP_Text text5 = this.deltaWeaponSwitchSpeed;
				int weaponSwitchSpeed = (int)this._currentDeltaData.WeaponSwitchSpeed;
				CharacterAttributeDisplayData colorData5 = this._colorData;
				AttributeWithDelta.SetPercentText(text5, weaponSwitchSpeed, ((int)((colorData5 != null) ? new short?(colorData5.WeaponSwitchSpeed) : null)) ?? int.MinValue);
				TMP_Text text6 = this.deltaCastSpeed;
				int castSpeed = (int)this._currentDeltaData.CastSpeed;
				CharacterAttributeDisplayData colorData6 = this._colorData;
				AttributeWithDelta.SetPercentText(text6, castSpeed, ((int)((colorData6 != null) ? new short?(colorData6.CastSpeed) : null)) ?? int.MinValue);
				TMP_Text text7 = this.deltaRecoveryOfFlaw;
				int recoveryOfFlaw = (int)this._currentDeltaData.RecoveryOfFlaw;
				CharacterAttributeDisplayData colorData7 = this._colorData;
				AttributeWithDelta.SetPercentText(text7, recoveryOfFlaw, ((int)((colorData7 != null) ? new short?(colorData7.RecoveryOfFlaw) : null)) ?? int.MinValue);
				TMP_Text text8 = this.deltaRecoveryOfBlockedAcupoint;
				int recoveryOfBlockedAcupoint = (int)this._currentDeltaData.RecoveryOfBlockedAcupoint;
				CharacterAttributeDisplayData colorData8 = this._colorData;
				AttributeWithDelta.SetPercentText(text8, recoveryOfBlockedAcupoint, ((int)((colorData8 != null) ? new short?(colorData8.RecoveryOfBlockedAcupoint) : null)) ?? int.MinValue);
				TMP_Text text9 = this.deltaRecoveryOfQiDisorder;
				int recoveryOfQiDisorder = (int)this._currentDeltaData.RecoveryOfQiDisorder;
				CharacterAttributeDisplayData colorData9 = this._colorData;
				AttributeWithDelta.SetPercentText(text9, recoveryOfQiDisorder, ((int)((colorData9 != null) ? new short?(colorData9.RecoveryOfQiDisorder) : null)) ?? int.MinValue);
				TMP_Text text10 = this.deltaInnerRatio;
				int innerRatio = (int)this._currentDeltaData.InnerRatio;
				CharacterAttributeDisplayData colorData10 = this._colorData;
				AttributeWithDelta.SetPercentText(text10, innerRatio, ((int)((colorData10 != null) ? new short?(colorData10.InnerRatio) : null)) ?? int.MinValue);
			}
		}

		// Token: 0x040089B9 RID: 35257
		[SerializeField]
		private Attribute self;

		// Token: 0x040089BA RID: 35258
		[Header("主属性")]
		[SerializeField]
		private TMP_Text[] deltaMainAttributeItems;

		// Token: 0x040089BB RID: 35259
		[Header("攻击属性")]
		[SerializeField]
		private TMP_Text deltaPenetrationOuter;

		// Token: 0x040089BC RID: 35260
		[SerializeField]
		private TMP_Text deltaPenetrationInner;

		// Token: 0x040089BD RID: 35261
		[Header("防御属性")]
		[SerializeField]
		private TMP_Text deltaPenetrationResistOuter;

		// Token: 0x040089BE RID: 35262
		[SerializeField]
		private TMP_Text deltaPenetrationResistInner;

		// Token: 0x040089BF RID: 35263
		[Header("命中属性")]
		[SerializeField]
		private TMP_Text[] deltaHitValueItems;

		// Token: 0x040089C0 RID: 35264
		[Header("化解属性")]
		[SerializeField]
		private TMP_Text[] deltaAvoidValueItems;

		// Token: 0x040089C1 RID: 35265
		[Header("次要属性")]
		[SerializeField]
		private TMP_Text deltaRecoveryOfStance;

		// Token: 0x040089C2 RID: 35266
		[SerializeField]
		private TMP_Text deltaRecoveryOfBreath;

		// Token: 0x040089C3 RID: 35267
		[SerializeField]
		private TMP_Text deltaAttackSpeed;

		// Token: 0x040089C4 RID: 35268
		[SerializeField]
		private TMP_Text deltaMoveSpeed;

		// Token: 0x040089C5 RID: 35269
		[SerializeField]
		private TMP_Text deltaWeaponSwitchSpeed;

		// Token: 0x040089C6 RID: 35270
		[SerializeField]
		private TMP_Text deltaCastSpeed;

		// Token: 0x040089C7 RID: 35271
		[SerializeField]
		private TMP_Text deltaRecoveryOfFlaw;

		// Token: 0x040089C8 RID: 35272
		[SerializeField]
		private TMP_Text deltaRecoveryOfBlockedAcupoint;

		// Token: 0x040089C9 RID: 35273
		[SerializeField]
		private TMP_Text deltaRecoveryOfQiDisorder;

		// Token: 0x040089CA RID: 35274
		[SerializeField]
		private TMP_Text deltaInnerRatio;

		// Token: 0x040089CB RID: 35275
		private CharacterAttributeDisplayData _currentDeltaData;

		// Token: 0x040089CC RID: 35276
		private CharacterAttributeDisplayData _colorData;
	}
}
