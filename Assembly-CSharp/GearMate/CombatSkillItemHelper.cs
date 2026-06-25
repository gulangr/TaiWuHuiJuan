using System;
using System.Collections.Generic;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000614 RID: 1556
	internal class CombatSkillItemHelper
	{
		// Token: 0x0600490D RID: 18701 RVA: 0x002227BD File Offset: 0x002209BD
		public CombatSkillItemHelper(Refers entry)
		{
			this.InitRefers(entry);
		}

		// Token: 0x0600490E RID: 18702 RVA: 0x002227D0 File Offset: 0x002209D0
		public void RefreshCombatSkillSlot(CombatSkillDisplayData skillData)
		{
			bool flag = skillData == null;
			if (flag)
			{
				this._combatSkillView.gameObject.SetActive(false);
			}
			else
			{
				this._combatSkillView.gameObject.SetActive(true);
				this._combatSkillView.SetData(skillData, true, false, true, false);
			}
		}

		// Token: 0x0600490F RID: 18703 RVA: 0x00222820 File Offset: 0x00220A20
		public void RefreshPageLayout(CombatSkillDisplayData skillData)
		{
			bool flag = skillData == null;
			if (flag)
			{
				this._pageLayout.gameObject.SetActive(false);
			}
			else
			{
				ushort activateState = skillData.ActivationState;
				bool broken = CombatSkillStateHelper.IsBrokenOut(activateState);
				bool flag2 = !broken;
				if (flag2)
				{
					this._pageLayout.gameObject.SetActive(false);
				}
				else
				{
					this._pageLayout.gameObject.SetActive(true);
					sbyte outlinePageType = CombatSkillStateHelper.GetActiveOutlinePageType(activateState);
					CommonUtils.PrepareEnoughChildren(this._pageLayout.transform, this._gearMateBreakPageItem.gameObject, 6, null);
					for (byte i = 0; i < 6; i += 1)
					{
						Refers refers = this._pageLayout.GetChild((int)i).GetComponent<Refers>();
						CImage icon = refers.GetComponent<CImage>();
						TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
						bool flag3 = i == 0;
						if (flag3)
						{
							icon.SetSprite("popup_gearmate_roundbase_0", false, null);
							label.text = LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", outlinePageType));
						}
						else
						{
							sbyte direction = CombatSkillStateHelper.GetPageActiveDirection(activateState, i);
							icon.SetSprite((direction == 0) ? "popup_gearmate_roundbase_2" : "popup_gearmate_roundbase_1", false, null);
							label.text = ((direction == 0) ? LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", (int)(i - 1))).SetColor("brightblue") : LocalStringManager.Get(string.Format("LK_CombatSkill_Reverse_Page_{0}", (int)(i - 1))).SetColor("brightred"));
						}
					}
				}
			}
		}

		// Token: 0x06004910 RID: 18704 RVA: 0x002229B8 File Offset: 0x00220BB8
		public void RefreshBonusLayout(IEnumerable<SkillBreakPlateBonus> bonusList, short skillId = -1, LifeSkillShorts lifeSkillAttainments = default(LifeSkillShorts), IAsyncMethodRequestHandler requestHandler = null)
		{
			this._bonusLayout.Refresh(bonusList, skillId, lifeSkillAttainments, requestHandler);
		}

		// Token: 0x06004911 RID: 18705 RVA: 0x002229CC File Offset: 0x00220BCC
		private void InitRefers(Refers entry)
		{
			this._combatSkillView = entry.CGet<CombatSkillView>("CombatSkillView");
			this._pageLayout = entry.CGet<RectTransform>("PageLayout");
			this._gearMateBreakPageItem = entry.CGet<Refers>("GearMateBreakPageItem");
			this._bonusLayout = entry.CGet<SkillBreakPlateBonusLayout>("BonusLayout");
		}

		// Token: 0x040032BD RID: 12989
		private CombatSkillView _combatSkillView;

		// Token: 0x040032BE RID: 12990
		private RectTransform _pageLayout;

		// Token: 0x040032BF RID: 12991
		private Refers _gearMateBreakPageItem;

		// Token: 0x040032C0 RID: 12992
		private SkillBreakPlateBonusLayout _bonusLayout;

		// Token: 0x040032C1 RID: 12993
		private Refers _gearMateBreakBonusItem;
	}
}
