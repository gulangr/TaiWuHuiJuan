using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item.Display;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ADB RID: 2779
	public class CricketJar : MonoBehaviour
	{
		// Token: 0x17000F17 RID: 3863
		// (get) Token: 0x060088B6 RID: 34998 RVA: 0x003F55CF File Offset: 0x003F37CF
		public CricketViewNew Cricket
		{
			get
			{
				return this.cricketView;
			}
		}

		// Token: 0x17000F18 RID: 3864
		// (get) Token: 0x060088B7 RID: 34999 RVA: 0x003F55D7 File Offset: 0x003F37D7
		public RectTransform CricketRectTransform
		{
			get
			{
				return (RectTransform)this.cricketView.transform;
			}
		}

		// Token: 0x17000F19 RID: 3865
		// (get) Token: 0x060088B8 RID: 35000 RVA: 0x003F55E9 File Offset: 0x003F37E9
		// (set) Token: 0x060088B9 RID: 35001 RVA: 0x003F55F1 File Offset: 0x003F37F1
		public CricketJarRoot Handler { get; set; }

		// Token: 0x060088BA RID: 35002 RVA: 0x003F55FC File Offset: 0x003F37FC
		private void Awake()
		{
			bool flag = this.ally;
			if (flag)
			{
				this.cancelButton.onClick.ResetListener(new Action(this.OnClickCancelButton));
			}
		}

		// Token: 0x060088BB RID: 35003 RVA: 0x003F5631 File Offset: 0x003F3831
		private void OnDisable()
		{
			this.Cricket.StopLoopSing();
		}

		// Token: 0x060088BC RID: 35004 RVA: 0x003F5640 File Offset: 0x003F3840
		public void EnableSelect(bool enable)
		{
			bool flag = !enable && this.goHover != null;
			if (flag)
			{
				this.goHover.SetActive(false);
			}
			PointerTrigger pointTrigger = base.GetComponent<PointerTrigger>();
			bool flag2 = pointTrigger != null;
			if (flag2)
			{
				pointTrigger.enabled = enable;
			}
			CButton cBtn = base.GetComponent<CButton>();
			bool flag3 = cBtn != null;
			if (flag3)
			{
				cBtn.interactable = enable;
			}
		}

		// Token: 0x060088BD RID: 35005 RVA: 0x003F56AC File Offset: 0x003F38AC
		public void SetVisible(bool visible)
		{
			CricketViewNew cricket = this.Cricket;
			cricket.skeletonGraphic.enabled = visible;
			cricket.GetComponent<TooltipInvoker>().enabled = visible;
			this.cricketName.SetVisible(visible);
			bool flag = this.gaiZi != null;
			if (flag)
			{
				this.gaiZi.gameObject.SetActive(!visible);
			}
		}

		// Token: 0x060088BE RID: 35006 RVA: 0x003F570F File Offset: 0x003F390F
		public void ShowName()
		{
			this.cricketName.SetVisible(true);
		}

		// Token: 0x060088BF RID: 35007 RVA: 0x003F571F File Offset: 0x003F391F
		public void HideName()
		{
			this.cricketName.Hide();
		}

		// Token: 0x060088C0 RID: 35008 RVA: 0x003F5730 File Offset: 0x003F3930
		public void SetSelected(bool selected)
		{
			bool flag = this.goSelected != null;
			if (flag)
			{
				this.goSelected.SetActive(selected);
			}
		}

		// Token: 0x060088C1 RID: 35009 RVA: 0x003F575C File Offset: 0x003F395C
		public void Clear(int order)
		{
			this.cricketName.SetOrder(order);
			this.cricketName.SetVisible(true);
			this.goResult.SetActive(false);
			CricketViewNew cricket = this.Cricket;
			bool inited = cricket.Inited;
			if (inited)
			{
				cricket.StopLoopSing();
			}
			RectTransform cricketRt = cricket.GetComponent<RectTransform>();
			cricketRt.SetParent(this.cricketView.transform.parent);
			cricketRt.anchoredPosition = Vector2.zero;
			this.SetNoCricket(true);
			this.RefreshFairCombatPoint(null);
			this.SetCancelButtonActive(false);
		}

		// Token: 0x060088C2 RID: 35010 RVA: 0x003F57EC File Offset: 0x003F39EC
		private void SetNoCricket(bool noCricket)
		{
			this.cricketName.SetNoCricket(noCricket);
			this.goCricketInfo.SetActive(!noCricket);
			bool flag = !this.ally;
			if (!flag)
			{
				bool flag2 = this.goNoCricket != null;
				if (flag2)
				{
					this.goNoCricket.SetActive(noCricket);
				}
			}
		}

		// Token: 0x060088C3 RID: 35011 RVA: 0x003F5848 File Offset: 0x003F3A48
		public void Set(ItemDisplayData itemData, int order)
		{
			CricketJar.<>c__DisplayClass32_0 CS$<>8__locals1 = new CricketJar.<>c__DisplayClass32_0();
			CS$<>8__locals1.<>4__this = this;
			this.SetNoCricket(false);
			this.goResult.SetActive(false);
			this.RefreshFairCombatPoint(itemData);
			this.SetCancelButtonActive(true);
			CS$<>8__locals1.cricket = this.Cricket;
			CS$<>8__locals1.cricket.SetCricketData(itemData.CricketColorId, itemData.CricketPartId, true, itemData, false);
			CS$<>8__locals1.cricket.gameObject.SetActive(true);
			CS$<>8__locals1.cricket.PlayAnimation(ECricketAnim.Idle, true, false);
			CS$<>8__locals1.onSingCallBack = (this.ally ? null : new Action<float>(CS$<>8__locals1.<Set>g__OnSingStart|0));
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo((float)Random.Range(1, CS$<>8__locals1.cricket.Level + 1), delegate
			{
				CS$<>8__locals1.cricket.Sing(true, true, true, -1f, CS$<>8__locals1.onSingCallBack, 0f);
			});
			this.cricketName.Set(itemData, order);
		}

		// Token: 0x060088C4 RID: 35012 RVA: 0x003F5928 File Offset: 0x003F3B28
		private void RefreshFairCombatPoint(ItemDisplayData itemData)
		{
			bool flag = !this.ally;
			if (!flag)
			{
				bool showPoint = this.ally && CricketFairCombatHelper.IsEnabled && itemData != null;
				this.fairCombatPointRoot.SetActive(showPoint);
				bool flag2 = !showPoint;
				if (!flag2)
				{
					this.fairCombatPointText.text = CricketFairCombatHelper.GetCricketCost(itemData).ToString();
				}
			}
		}

		// Token: 0x060088C5 RID: 35013 RVA: 0x003F5990 File Offset: 0x003F3B90
		public void Settlement(bool win)
		{
			CricketViewNew cricket = this.Cricket;
			RectTransform cricketRt = cricket.GetComponent<RectTransform>();
			cricketRt.SetParent(this.goCricketInfo.transform);
			cricketRt.SetAsFirstSibling();
			cricketRt.localEulerAngles = new Vector3(0f, 0f, (float)(this.ally ? -90 : 90));
			cricketRt.anchoredPosition = Vector2.zero;
			cricketRt.localScale = Vector3.one * 0.3f;
			this.goResult.SetActive(true);
			this.goWin.SetActive(win);
			this.goLose.SetActive(!win);
			cricket.Fade(true);
			this.ShowName();
			if (win)
			{
				cricket.PlayAnimation(ECricketAnim.Idle, true, false);
				cricket.Sing(true, true, true, -1f, null, 0f);
			}
			else
			{
				cricket.StopLoopSing();
			}
		}

		// Token: 0x060088C6 RID: 35014 RVA: 0x003F5A75 File Offset: 0x003F3C75
		public void InitData(CricketJarRoot handler, int index)
		{
			this.Handler = handler;
			this.jarIndex = index;
		}

		// Token: 0x060088C7 RID: 35015 RVA: 0x003F5A87 File Offset: 0x003F3C87
		public void OnClickCancelButton()
		{
			this.Handler.SetSelfCrickets(this.jarIndex);
		}

		// Token: 0x060088C8 RID: 35016 RVA: 0x003F5A9C File Offset: 0x003F3C9C
		public void SetCancelButtonActive(bool isShow)
		{
			bool flag = this.ally;
			if (flag)
			{
				this.cancelButton.gameObject.SetActive(isShow);
			}
		}

		// Token: 0x040068A6 RID: 26790
		[SerializeField]
		private CricketViewNew cricketView;

		// Token: 0x040068A7 RID: 26791
		[SerializeField]
		private GameObject goResult;

		// Token: 0x040068A8 RID: 26792
		[SerializeField]
		private GameObject goWin;

		// Token: 0x040068A9 RID: 26793
		[SerializeField]
		private GameObject goLose;

		// Token: 0x040068AA RID: 26794
		[SerializeField]
		private GameObject goCricketInfo;

		// Token: 0x040068AB RID: 26795
		[SerializeField]
		private GameObject fairCombatPointRoot;

		// Token: 0x040068AC RID: 26796
		[SerializeField]
		private TextMeshProUGUI fairCombatPointText;

		// Token: 0x040068AD RID: 26797
		[SerializeField]
		private CButton cancelButton;

		// Token: 0x040068AE RID: 26798
		[SerializeField]
		private SkeletonGraphic gaiZi;

		// Token: 0x040068AF RID: 26799
		[SerializeField]
		private CricketName cricketName;

		// Token: 0x040068B0 RID: 26800
		[SerializeField]
		private GameObject goNoCricket;

		// Token: 0x040068B1 RID: 26801
		[SerializeField]
		private GameObject goHover;

		// Token: 0x040068B2 RID: 26802
		[SerializeField]
		private GameObject goSelected;

		// Token: 0x040068B3 RID: 26803
		[SerializeField]
		private bool ally;

		// Token: 0x040068B5 RID: 26805
		private int jarIndex;
	}
}
