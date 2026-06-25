using System;
using System.Collections;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B1 RID: 2481
	public class KongsangChacacterItem : MonoBehaviour
	{
		// Token: 0x17000D63 RID: 3427
		// (get) Token: 0x0600781F RID: 30751 RVA: 0x0037E1F8 File Offset: 0x0037C3F8
		private ViewKongsangSpecialInteract ParentView
		{
			get
			{
				return UIElement.KongsangSpecialInteract.UiBaseAs<ViewKongsangSpecialInteract>();
			}
		}

		// Token: 0x06007820 RID: 30752 RVA: 0x0037E204 File Offset: 0x0037C404
		public void Awake()
		{
			this.deleteBtn.ClearAndAddListener(delegate
			{
				this.ParentView.OnCharacterDeleteBtnClicked(this._curCharacterId);
			});
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(true);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
			base.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				UIElement.KongsangSpecialInteract.UiBaseAs<ViewKongsangSpecialInteract>().ShowSelectCharacterUI();
			});
		}

		// Token: 0x06007821 RID: 30753 RVA: 0x0037E290 File Offset: 0x0037C490
		public void Set(CharacterDisplayData data, int protectRemainMonths)
		{
			this.avatar.Refresh(data, true);
			this.canvasGroup.alpha = 1f;
			this.effCanvasGroup.alpha = 1f;
			this.characterNameText.text = NameCenter.GetNameByDisplayData(data, false, false);
			this.characterOrgInfoText.text = CommonUtils.GetIdentityString(data.OrgInfo, data.Gender, data.PhysiologicalAge, false);
			int darkAshRemainingTotal = data.DarkAshCounter.Total;
			int featureRemainingMonths = (darkAshRemainingTotal > 0) ? darkAshRemainingTotal : protectRemainMonths;
			this.timeText.text = featureRemainingMonths.ToString();
			this.timeHolder.gameObject.SetActive(featureRemainingMonths > 0);
			this.timeHolder.GetComponent<CImage>().sprite = this.monthArray[(darkAshRemainingTotal > 0) ? 0 : 1];
			this.featureTip.enabled = (featureRemainingMonths > 0);
			base.GetComponent<CImage>().sprite = this.backArray[(darkAshRemainingTotal > 0) ? 1 : 0];
			this.statusHolder.gameObject.SetActive(false);
			this.effCanvasGroup.gameObject.SetActive(darkAshRemainingTotal > 0);
			this.protectEffCanvasGroup.gameObject.SetActive(protectRemainMonths > 0);
			this.eff_kongsang_touxiang.gameObject.SetActive(darkAshRemainingTotal > 0);
			this.eff_kongsang_touxiang2.gameObject.SetActive(protectRemainMonths > 0);
			this._curCharacterId = data.CharacterId;
			this._curTotalRemainingMonths = featureRemainingMonths;
			this.ParentView.AppendCharacterItem(this);
			this.characterTip.Type = TipType.Character;
			TooltipInvoker tooltipInvoker = this.characterTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.characterTip.RuntimeParam.Set("charId", data.CharacterId);
			this.featureTip.Type = TipType.Feature;
			tooltipInvoker = this.featureTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			bool flag = darkAshRemainingTotal > 0;
			if (flag)
			{
				this.featureTip.RuntimeParam.Set("FeatureId", 216);
			}
			else
			{
				bool flag2 = protectRemainMonths > 0;
				if (flag2)
				{
					this.featureTip.RuntimeParam.Set("FeatureId", 738);
				}
			}
			this.featureTip.RuntimeParam.Set("CharacterId", data.CharacterId);
		}

		// Token: 0x06007822 RID: 30754 RVA: 0x0037E4EC File Offset: 0x0037C6EC
		public IEnumerator PlayClearEff()
		{
			bool flag = this._curTotalRemainingMonths > 0;
			if (flag)
			{
				this._curTotalRemainingMonths = 0;
			}
			this.effCanvasGroup.DOFade(0f, 0.8f);
			yield return new WaitForSeconds(0.8f);
			this.canvasGroup.DOFade(0f, 0.8f);
			yield return null;
			yield break;
		}

		// Token: 0x04005AC4 RID: 23236
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04005AC5 RID: 23237
		[SerializeField]
		private RectTransform statusHolder;

		// Token: 0x04005AC6 RID: 23238
		[SerializeField]
		private TextMeshProUGUI characterNameText;

		// Token: 0x04005AC7 RID: 23239
		[SerializeField]
		private TextMeshProUGUI characterOrgInfoText;

		// Token: 0x04005AC8 RID: 23240
		[SerializeField]
		private RectTransform timeHolder;

		// Token: 0x04005AC9 RID: 23241
		[SerializeField]
		private TextMeshProUGUI timeText;

		// Token: 0x04005ACA RID: 23242
		[SerializeField]
		private CButton deleteBtn;

		// Token: 0x04005ACB RID: 23243
		[SerializeField]
		private Sprite[] backArray;

		// Token: 0x04005ACC RID: 23244
		[SerializeField]
		private Sprite[] monthArray;

		// Token: 0x04005ACD RID: 23245
		[SerializeField]
		private ParticleSystem eff_kongsang_touxiang;

		// Token: 0x04005ACE RID: 23246
		[SerializeField]
		private ParticleSystem eff_kongsang_touxiang2;

		// Token: 0x04005ACF RID: 23247
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04005AD0 RID: 23248
		[SerializeField]
		private CanvasGroup effCanvasGroup;

		// Token: 0x04005AD1 RID: 23249
		[SerializeField]
		private CanvasGroup protectEffCanvasGroup;

		// Token: 0x04005AD2 RID: 23250
		[SerializeField]
		private TooltipInvoker characterTip;

		// Token: 0x04005AD3 RID: 23251
		[SerializeField]
		private TooltipInvoker featureTip;

		// Token: 0x04005AD4 RID: 23252
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04005AD5 RID: 23253
		[SerializeField]
		private RectTransform hover;

		// Token: 0x04005AD6 RID: 23254
		private int _curCharacterId;

		// Token: 0x04005AD7 RID: 23255
		private int _curTotalRemainingMonths;
	}
}
