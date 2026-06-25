using System;
using Game.Components.Avatar;
using Game.Views.Cricket;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll.Item
{
	// Token: 0x02000EA8 RID: 3752
	public class CardItem : RowItemLine
	{
		// Token: 0x170013AB RID: 5035
		// (get) Token: 0x0600ADDB RID: 44507 RVA: 0x004F3AB9 File Offset: 0x004F1CB9
		// (set) Token: 0x0600ADDC RID: 44508 RVA: 0x004F3AC1 File Offset: 0x004F1CC1
		public CricketViewNew CricketView { get; private set; }

		// Token: 0x170013AC RID: 5036
		// (get) Token: 0x0600ADDD RID: 44509 RVA: 0x004F3ACA File Offset: 0x004F1CCA
		// (set) Token: 0x0600ADDE RID: 44510 RVA: 0x004F3AD2 File Offset: 0x004F1CD2
		public JiaoEggView JiaoEggView { get; private set; }

		// Token: 0x170013AD RID: 5037
		// (get) Token: 0x0600ADDF RID: 44511 RVA: 0x004F3ADB File Offset: 0x004F1CDB
		private bool IsCricket
		{
			get
			{
				return base.Data != null && base.Data.Key.ItemType == 11;
			}
		}

		// Token: 0x170013AE RID: 5038
		// (get) Token: 0x0600ADE0 RID: 44512 RVA: 0x004F3AFC File Offset: 0x004F1CFC
		private bool IsJiaoEgg
		{
			get
			{
				bool result;
				if (base.Data != null)
				{
					JiaoLoongDisplayData jiaoLoongDisplayData = base.Data.JiaoLoongDisplayData;
					result = (jiaoLoongDisplayData != null && jiaoLoongDisplayData.IsEgg);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x170013AF RID: 5039
		// (get) Token: 0x0600ADE1 RID: 44513 RVA: 0x004F3B2C File Offset: 0x004F1D2C
		private bool IsCharacter
		{
			get
			{
				return base.Data != null && base.Data.CharacterId != -1;
			}
		}

		// Token: 0x0600ADE2 RID: 44514 RVA: 0x004F3B4C File Offset: 0x004F1D4C
		public override void SetDisabled(bool disabled)
		{
			bool flag = this.canvasGroup;
			if (flag)
			{
				this.canvasGroup.alpha = ((!disabled) ? 1f : 0.3f);
			}
			bool flag2 = this.hsvDisable;
			if (flag2)
			{
				bool flag3 = this.CricketView;
				if (flag3)
				{
					this.hsvDisable.RefreshAutoSkip();
				}
				this.hsvDisable.SetInteractable(!disabled);
			}
			bool flag4 = this.CricketView;
			if (flag4)
			{
				this.CricketView.skeletonGraphic.color = ((!disabled) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.3f));
			}
			bool flag5 = this.characterAvatar && this.characterAvatar.gameObject.activeSelf;
			if (flag5)
			{
				this.characterAvatar.SetShadowStrength((!disabled) ? 0f : 0.7f);
			}
		}

		// Token: 0x0600ADE3 RID: 44515 RVA: 0x004F3C44 File Offset: 0x004F1E44
		public override void Set(RowItemMain rowItemMain, bool showTip = true)
		{
			base.Set(rowItemMain, showTip);
			bool flag = this.IsCricket || this.IsJiaoEgg || this.IsCharacter;
			if (flag)
			{
				RowItemMain rowItemMain2 = base.RowItemMain;
				if (rowItemMain2 != null)
				{
					rowItemMain2.ItemBack.SetIcon(string.Empty);
				}
			}
			float itemHeight = this.hasTextPrice ? 240f : 210f;
			base.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
			this.SetPrice();
			this.RefreshCricket();
			this.RefreshJiaoEggView();
			this.RefreshCharacterAvatar();
		}

		// Token: 0x0600ADE4 RID: 44516 RVA: 0x004F3CD8 File Offset: 0x004F1ED8
		public override void OnItemHide()
		{
			base.OnItemHide();
			this.ReturnCricket();
			this.ReturnJiaoEggView();
			this.HideCharacterAvatar();
		}

		// Token: 0x0600ADE5 RID: 44517 RVA: 0x004F3CF8 File Offset: 0x004F1EF8
		private void RefreshCricket()
		{
			bool isCricket = this.IsCricket;
			if (isCricket)
			{
				this.GetCricket();
			}
			else
			{
				this.ReturnCricket();
			}
		}

		// Token: 0x0600ADE6 RID: 44518 RVA: 0x004F3D20 File Offset: 0x004F1F20
		private void GetCricket()
		{
			bool flag = !this.CricketView;
			if (flag)
			{
				this.CricketView = SingletonObject.getInstance<ItemViewPool>().Get<CricketViewNew>();
				Transform cricketTransform = this.CricketView.transform;
				cricketTransform.SetParent(this.cricketHolder);
				cricketTransform.localPosition = Vector3.zero;
				cricketTransform.localScale = Vector3.one * 2f;
				cricketTransform.localRotation = Quaternion.Euler(Vector3.zero);
			}
			this.CricketView.SetCricketData(base.Data.CricketColorId, base.Data.CricketPartId, false, base.Data, false);
		}

		// Token: 0x0600ADE7 RID: 44519 RVA: 0x004F3DCC File Offset: 0x004F1FCC
		private void ReturnCricket()
		{
			bool flag = this.CricketView;
			if (flag)
			{
				bool flag2 = !SingletonObject.IsDestroying;
				if (flag2)
				{
					this.CricketView.skeletonGraphic.enabled = true;
					this.CricketView.GetComponent<TooltipInvoker>().enabled = true;
					ItemViewPool instance = SingletonObject.getInstance<ItemViewPool>();
					if (instance != null)
					{
						instance.Return<CricketViewNew>(this.CricketView);
					}
				}
				this.CricketView = null;
			}
		}

		// Token: 0x0600ADE8 RID: 44520 RVA: 0x004F3E3C File Offset: 0x004F203C
		public void SetPrice()
		{
			this.priceBackGo.SetActive(this.hasTextPrice);
			bool flag = !this.hasTextPrice;
			if (!flag)
			{
				this.textPrice.text = ((base.Data.CharacterId == -1 && !base.Data.Key.HasTemplate) ? "-" : CommonUtils.GetDisplayStringForNum(base.Data.Value));
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.priceBackGo.GetComponent<RectTransform>());
			}
		}

		// Token: 0x0600ADE9 RID: 44521 RVA: 0x004F3EC4 File Offset: 0x004F20C4
		private void RefreshJiaoEggView()
		{
			bool isJiaoEgg = this.IsJiaoEgg;
			if (isJiaoEgg)
			{
				this.GetJiaoEggView();
			}
			else
			{
				this.ReturnJiaoEggView();
			}
		}

		// Token: 0x0600ADEA RID: 44522 RVA: 0x004F3EEC File Offset: 0x004F20EC
		private void GetJiaoEggView()
		{
			bool flag = !this.JiaoEggView;
			if (flag)
			{
				this.JiaoEggView = SingletonObject.getInstance<ItemViewPool>().Get<JiaoEggView>();
				bool flag2 = this.JiaoEggView;
				if (flag2)
				{
					Transform trans = this.JiaoEggView.transform;
					trans.SetParent(this.jiaoEggHolder);
					trans.localPosition = Vector3.zero;
					trans.localScale = Vector3.one * 2f;
					trans.localRotation = Quaternion.identity;
				}
			}
			bool flag3 = this.JiaoEggView;
			if (flag3)
			{
				this.JiaoEggView.Refresh(base.Data.JiaoLoongDisplayData, false);
			}
		}

		// Token: 0x0600ADEB RID: 44523 RVA: 0x004F3FA0 File Offset: 0x004F21A0
		private void ReturnJiaoEggView()
		{
			bool flag = this.JiaoEggView;
			if (flag)
			{
				bool flag2 = !SingletonObject.IsDestroying;
				if (flag2)
				{
					ItemViewPool instance = SingletonObject.getInstance<ItemViewPool>();
					if (instance != null)
					{
						instance.Return<JiaoEggView>(this.JiaoEggView);
					}
				}
				this.JiaoEggView = null;
			}
		}

		// Token: 0x0600ADEC RID: 44524 RVA: 0x004F3FEC File Offset: 0x004F21EC
		private void RefreshCharacterAvatar()
		{
			bool isCharacter = this.IsCharacter;
			if (isCharacter)
			{
				this.ShowCharacterAvatar();
			}
			else
			{
				this.HideCharacterAvatar();
			}
		}

		// Token: 0x0600ADED RID: 44525 RVA: 0x004F4014 File Offset: 0x004F2214
		private void ShowCharacterAvatar()
		{
			bool flag = !this.characterAvatar;
			if (!flag)
			{
				AvatarRelatedData avatarRelatedData = base.Data.AvatarRelatedData;
				bool flag2 = avatarRelatedData == null;
				if (flag2)
				{
					this.HideCharacterAvatar();
				}
				else
				{
					short templateId = base.Data.NameRelatedData.CharTemplateId;
					this.characterAvatar.gameObject.SetActive(true);
					bool flag3 = templateId > 0;
					if (flag3)
					{
						this.characterAvatar.Refresh(avatarRelatedData, templateId);
					}
					else
					{
						this.characterAvatar.Refresh(avatarRelatedData);
					}
				}
			}
		}

		// Token: 0x0600ADEE RID: 44526 RVA: 0x004F40A0 File Offset: 0x004F22A0
		private void HideCharacterAvatar()
		{
			bool flag = this.characterAvatar;
			if (flag)
			{
				this.characterAvatar.gameObject.SetActive(false);
			}
		}

		// Token: 0x0400864C RID: 34380
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x0400864D RID: 34381
		[SerializeField]
		private HSVStyleRoot hsvDisable;

		// Token: 0x0400864E RID: 34382
		[SerializeField]
		private RectTransform cricketHolder;

		// Token: 0x0400864F RID: 34383
		[SerializeField]
		private RectTransform jiaoEggHolder;

		// Token: 0x04008650 RID: 34384
		[SerializeField]
		private Game.Components.Avatar.Avatar characterAvatar;

		// Token: 0x04008651 RID: 34385
		[SerializeField]
		private bool hasTextPrice;

		// Token: 0x04008652 RID: 34386
		[SerializeField]
		private TextMeshProUGUI textPrice;

		// Token: 0x04008653 RID: 34387
		[SerializeField]
		private GameObject priceBackGo;

		// Token: 0x04008654 RID: 34388
		private const float NormalHeight = 210f;

		// Token: 0x04008655 RID: 34389
		private const float HasPriceHeight = 240f;
	}
}
