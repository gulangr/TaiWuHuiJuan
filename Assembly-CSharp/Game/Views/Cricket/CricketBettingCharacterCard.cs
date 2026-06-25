using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AAD RID: 2733
	public class CricketBettingCharacterCard : MonoBehaviour
	{
		// Token: 0x0600863A RID: 34362 RVA: 0x003E7C10 File Offset: 0x003E5E10
		public void BindClick(Action callback)
		{
			this.button.ClearAndAddListener(delegate
			{
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}
			});
		}

		// Token: 0x0600863B RID: 34363 RVA: 0x003E7C44 File Offset: 0x003E5E44
		public void SetCharacter(CharacterDisplayData displayData, long value, long minValue, bool isTeammate)
		{
			this._characterId = ((displayData != null) ? displayData.CharacterId : -1);
			this.cardItemRowItemMain.SetData(new CricketCombatRewardCharacterContent(displayData));
			this.cardItem.Set(this.cardItemRowItemMain, false);
			this.cardItem.gameObject.SetActive(true);
			this.valueRoot.SetActive(true);
			bool valueInsufficient = value < minValue;
			this.valueText.text = value.ToString().SetColor(valueInsufficient ? "brightred" : "brightyellow");
			this.SetMarkVisible(isTeammate, !isTeammate);
			this.button.interactable = !valueInsufficient;
			this.UpdateTipDisplay(valueInsufficient);
			this.UpdateFavorability(displayData);
		}

		// Token: 0x0600863C RID: 34364 RVA: 0x003E7D04 File Offset: 0x003E5F04
		private void UpdateFavorability(CharacterDisplayData displayData)
		{
			bool flag = displayData == null;
			if (flag)
			{
				this.favorIcon.SetSprite("mousetip_haogan_4", true, null);
				this.favorValue.text = "-";
			}
			else
			{
				int iconIndex = CommonUtils.GetFavorabilityIconIndex(displayData.FavorabilityToTaiwu, true) + 4;
				this.favorIcon.SetSprite("mousetip_haogan_" + iconIndex.ToString(), true, null);
				this.favorValue.text = CommonUtils.GetFavorString(displayData.FavorabilityToTaiwu);
			}
		}

		// Token: 0x0600863D RID: 34365 RVA: 0x003E7D88 File Offset: 0x003E5F88
		private void UpdateTipDisplay(bool valueInsufficient)
		{
			bool flag = this.tipDisplayer == null;
			if (!flag)
			{
				if (valueInsufficient)
				{
					this.tipDisplayer.Type = TipType.SingleDesc;
					this.tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
					this.tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketBetting_ValueInsufficient));
					this.tipDisplayer.enabled = true;
				}
				else
				{
					bool flag2 = this._characterId >= 0;
					if (flag2)
					{
						this.tipDisplayer.Type = TipType.CharacterOnMapBlock;
						this.tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharId", this._characterId);
						this.tipDisplayer.enabled = true;
					}
					else
					{
						this.tipDisplayer.enabled = false;
					}
				}
			}
		}

		// Token: 0x0600863E RID: 34366 RVA: 0x003E7E5C File Offset: 0x003E605C
		public void SetSelected(bool selected)
		{
			CricketBettingCharacterCard.SetGameObjectActive(this.selectedRoot, selected);
		}

		// Token: 0x0600863F RID: 34367 RVA: 0x003E7E6C File Offset: 0x003E606C
		private void SetMarkVisible(bool showTeammate, bool showPrisoner)
		{
			this.teammateMark.SetActive(showTeammate);
			this.prisonerMark.SetActive(showPrisoner);
		}

		// Token: 0x06008640 RID: 34368 RVA: 0x003E7E89 File Offset: 0x003E6089
		private static void SetGameObjectActive(GameObject obj, bool active)
		{
			obj.SetActive(active);
		}

		// Token: 0x04006708 RID: 26376
		[SerializeField]
		private CardItem cardItem;

		// Token: 0x04006709 RID: 26377
		[SerializeField]
		private RowItemMain cardItemRowItemMain;

		// Token: 0x0400670A RID: 26378
		[SerializeField]
		private GameObject valueRoot;

		// Token: 0x0400670B RID: 26379
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x0400670C RID: 26380
		[SerializeField]
		private GameObject teammateMark;

		// Token: 0x0400670D RID: 26381
		[SerializeField]
		private GameObject prisonerMark;

		// Token: 0x0400670E RID: 26382
		[SerializeField]
		private GameObject selectedRoot;

		// Token: 0x0400670F RID: 26383
		[SerializeField]
		private CButton button;

		// Token: 0x04006710 RID: 26384
		[SerializeField]
		private TooltipInvoker tipDisplayer;

		// Token: 0x04006711 RID: 26385
		[SerializeField]
		private CImage favorIcon;

		// Token: 0x04006712 RID: 26386
		[SerializeField]
		private TextMeshProUGUI favorValue;

		// Token: 0x04006713 RID: 26387
		private int _characterId = -1;
	}
}
