using System;
using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD6 RID: 3030
	public class BuildingExpandTaiwuVillageSteleComponent : MonoBehaviour
	{
		// Token: 0x06009892 RID: 39058 RVA: 0x00471050 File Offset: 0x0046F250
		private void Awake()
		{
			CButton button = base.GetComponent<CButton>();
			button.ClearAndAddListener(new Action(this.OnClick));
			this.unlockEff.Stop();
			this.unlockEff.gameObject.SetActive(false);
		}

		// Token: 0x06009893 RID: 39059 RVA: 0x00471096 File Offset: 0x0046F296
		public void Bind(IBuildingExpandTaiwuVillageSteleHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06009894 RID: 39060 RVA: 0x004710A0 File Offset: 0x0046F2A0
		public void Set(bool upgraded, bool upgradedAllUnlocked, sbyte selectedOrgTemplateId, bool mouseTipEnabled)
		{
			this.shadowIcon.gameObject.SetActive(upgradedAllUnlocked);
			string spriteName;
			if (upgradedAllUnlocked)
			{
				spriteName = this.GetSpriteNameAllUnlock();
			}
			else
			{
				spriteName = this.GetSpriteName(upgraded);
			}
			this.mainIcon.SetSprite(spriteName, false, null);
			base.GetComponent<CButton>().interactable = true;
			this.SetMouseTip(upgraded, mouseTipEnabled);
			this.unlockedLoopEff.gameObject.SetActive(upgraded);
			bool selectedActive = this.orgTemplateId == selectedOrgTemplateId;
			bool flag = this.selected.activeSelf != selectedActive;
			if (flag)
			{
				this.selected.SetActive(selectedActive);
				this.selected2.SetActive(selectedActive);
				this.selected2.GetComponent<CImage>().DOFade(1f, 0f);
			}
			bool activeSelf = this.animationTransition.gameObject.activeSelf;
			if (activeSelf)
			{
				this.animationTransition.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009895 RID: 39061 RVA: 0x00471190 File Offset: 0x0046F390
		public void AnimationToUpgraded(float duration)
		{
			this.mainIcon.SetSprite(this.GetSpriteName(true), false, null);
			this.animationTransition.DOKill(false);
			this.animationTransition.SetSprite(this.GetSpriteName(false), false, null);
			this.animationTransition.gameObject.SetActive(true);
			this.animationTransition.SetAlpha(1f);
			this.animationTransition.DOFade(0f, duration);
		}

		// Token: 0x06009896 RID: 39062 RVA: 0x0047120C File Offset: 0x0046F40C
		public void AnimationToUpgradedAllUnlock(float duration)
		{
			this.mainIcon.SetSprite(this.GetSpriteNameAllUnlock(), false, null);
			this.animationTransition.DOKill(false);
			this.animationTransition.SetSprite(this.GetSpriteName(true), false, null);
			this.animationTransition.gameObject.SetActive(true);
			this.animationTransition.SetAlpha(1f);
			this.animationTransition.DOFade(0f, duration);
			this.shadowIcon.gameObject.SetActive(true);
			this.shadowIcon.SetAlpha(0f);
			this.shadowIcon.DOFade(1f, duration);
		}

		// Token: 0x06009897 RID: 39063 RVA: 0x004712BC File Offset: 0x0046F4BC
		public void PlayUnlockEff()
		{
			this.unlockEff.transform.parent = this.unlockEffRoot;
			this.unlockEff.gameObject.SetActive(true);
			this.unlockEff.Play();
			this.unlockEffMat.SetFloat("_rongjie", -0.1f);
			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(0.2f);
			seq.AppendCallback(delegate
			{
				this.unlockEffMat.DOFloat(1f, "_rongjie", 2f).SetEase(Ease.InQuad);
			});
		}

		// Token: 0x06009898 RID: 39064 RVA: 0x0047133C File Offset: 0x0046F53C
		private void SetMouseTip(bool upgraded, bool mouseTipEnabled)
		{
			TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
			mouseTip.enabled = mouseTipEnabled;
			bool flag = !mouseTipEnabled;
			if (flag)
			{
				bool showing = mouseTip.Showing;
				if (showing)
				{
					mouseTip.HideTips();
				}
			}
			else
			{
				TooltipInvoker tooltipInvoker = mouseTip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = new ArgumentBox());
				}
				ArgumentBox argBox = argumentBox;
				argBox.Set("OrgTemplateId", this.orgTemplateId);
				argBox.Set("Unlocked", upgraded);
				bool showing2 = mouseTip.Showing;
				if (showing2)
				{
					mouseTip.ShowTips();
				}
			}
		}

		// Token: 0x06009899 RID: 39065 RVA: 0x004713C8 File Offset: 0x0046F5C8
		private void OnClick()
		{
			bool activeSelf = this.selected.activeSelf;
			if (activeSelf)
			{
				IBuildingExpandTaiwuVillageSteleHandler handler = this._handler;
				if (handler != null)
				{
					handler.Cancel();
				}
			}
			else
			{
				IBuildingExpandTaiwuVillageSteleHandler handler2 = this._handler;
				if (handler2 != null)
				{
					handler2.Handle(this.orgTemplateId);
				}
			}
		}

		// Token: 0x0600989A RID: 39066 RVA: 0x00471414 File Offset: 0x0046F614
		private string GetSpriteName(bool upgraded)
		{
			return string.Format("{0}{1}_{2}", "ui9_back_building_vow_stele_", this.orgTemplateId, upgraded ? 1 : 0);
		}

		// Token: 0x0600989B RID: 39067 RVA: 0x0047144C File Offset: 0x0046F64C
		private string GetSpriteNameAllUnlock()
		{
			return string.Format("{0}{1}_2", "ui9_back_building_vow_stele_", this.orgTemplateId);
		}

		// Token: 0x0600989C RID: 39068 RVA: 0x00471478 File Offset: 0x0046F678
		public void HideUnlockEff()
		{
			this.unlockEff.gameObject.SetActive(false);
		}

		// Token: 0x0600989D RID: 39069 RVA: 0x0047148D File Offset: 0x0046F68D
		private void Update()
		{
		}

		// Token: 0x0600989E RID: 39070 RVA: 0x00471490 File Offset: 0x0046F690
		private void OnEnable()
		{
			this.selectedArrow.DOKill(false);
			Vector2 posUp = this.arrowPositionUp.anchoredPosition;
			Vector2 posDown = this.arrowPositionDown.anchoredPosition;
			this.selectedArrow.anchoredPosition = posDown;
			this.selectedArrow.DOAnchorPos(posUp, 1f, false).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}

		// Token: 0x0600989F RID: 39071 RVA: 0x004714F0 File Offset: 0x0046F6F0
		private void OnDisable()
		{
			this.selectedArrow.DOKill(false);
		}

		// Token: 0x0400755D RID: 30045
		[SerializeField]
		public sbyte orgTemplateId;

		// Token: 0x0400755E RID: 30046
		[SerializeField]
		public GameObject selected;

		// Token: 0x0400755F RID: 30047
		[SerializeField]
		public GameObject selected2;

		// Token: 0x04007560 RID: 30048
		[SerializeField]
		public RectTransform selectedArrow;

		// Token: 0x04007561 RID: 30049
		[SerializeField]
		public RectTransform arrowPositionUp;

		// Token: 0x04007562 RID: 30050
		[SerializeField]
		public RectTransform arrowPositionDown;

		// Token: 0x04007563 RID: 30051
		[SerializeField]
		public RectTransform hoverObj;

		// Token: 0x04007564 RID: 30052
		[SerializeField]
		public CImage animationTransition;

		// Token: 0x04007565 RID: 30053
		[SerializeField]
		public CImage mainIcon;

		// Token: 0x04007566 RID: 30054
		[SerializeField]
		public CImage shadowIcon;

		// Token: 0x04007567 RID: 30055
		[Header("解锁动画")]
		[SerializeField]
		public UIParticle unlockEff;

		// Token: 0x04007568 RID: 30056
		[SerializeField]
		public Material unlockEffMat;

		// Token: 0x04007569 RID: 30057
		[SerializeField]
		public Transform unlockEffRoot;

		// Token: 0x0400756A RID: 30058
		[Header("解锁后的循环动画")]
		[SerializeField]
		public UIParticle unlockedLoopEff;

		// Token: 0x0400756B RID: 30059
		private IBuildingExpandTaiwuVillageSteleHandler _handler;
	}
}
