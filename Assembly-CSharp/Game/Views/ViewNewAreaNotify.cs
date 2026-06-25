using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	// Token: 0x020006EA RID: 1770
	public class ViewNewAreaNotify : UIBase
	{
		// Token: 0x060053EB RID: 21483 RVA: 0x0026E0B4 File Offset: 0x0026C2B4
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox != null;
			if (flag)
			{
				argsBox.Get("IsVisited", out ViewNewAreaNotify._isVisited);
				argsBox.Get("AreaTemplateId", out ViewNewAreaNotify._areaTemplateId);
			}
			this._showTime = (ViewNewAreaNotify._isVisited ? this.DefaultShowTimeArrive : this.DefaultShowTimeFound);
		}

		// Token: 0x060053EC RID: 21484 RVA: 0x0026E109 File Offset: 0x0026C309
		public static void Setup(short areaTemplateId, bool visited)
		{
			ViewNewAreaNotify._isVisited = visited;
			ViewNewAreaNotify._areaTemplateId = areaTemplateId;
		}

		// Token: 0x060053ED RID: 21485 RVA: 0x0026E118 File Offset: 0x0026C318
		private void OnEnable()
		{
			bool flag = ViewNewAreaNotify._areaTemplateId < 0;
			if (flag)
			{
				UIManager.Instance.HideUI(this.Element);
			}
			else
			{
				this.SetupPage();
				this.animRoot.alpha = 1f;
				this.DisplayAnimation();
				ViewNewAreaNotify._areaTemplateId = -1;
			}
		}

		// Token: 0x060053EE RID: 21486 RVA: 0x0026E16C File Offset: 0x0026C36C
		private void DisplayAnimation()
		{
			this.bgRoot.alpha = 0f;
			this.bgRoot2.alpha = 0f;
			this.iconRoot.alpha = 0f;
			float bgFadeInDuration = ViewNewAreaNotify._isVisited ? 0.3f : 0.2f;
			float iconFadeInDelay = ViewNewAreaNotify._isVisited ? 0.26666668f : 0.13333334f;
			float iconFadeInDuration = ViewNewAreaNotify._isVisited ? 0.33333334f : 0.2f;
			float fadeOutDelay = ViewNewAreaNotify._isVisited ? 1.5333333f : 0.9f;
			float fadeOutDuration = ViewNewAreaNotify._isVisited ? 0.2f : 0.16666667f;
			Sequence seq = DOTween.Sequence();
			seq.AppendInterval(0.2f);
			seq.AppendCallback(delegate
			{
				this.bgRoot.DOFade(1f, bgFadeInDuration);
			});
			seq.AppendCallback(delegate
			{
				this.bgRoot2.DOFade(1f, bgFadeInDuration);
			});
			seq.AppendInterval(iconFadeInDelay);
			seq.AppendCallback(delegate
			{
				this.iconRoot.DOFade(1f, iconFadeInDuration);
			});
			seq.AppendInterval(fadeOutDelay);
			seq.AppendCallback(delegate
			{
				this.bgRoot.DOFade(0f, fadeOutDuration);
			});
			seq.AppendCallback(delegate
			{
				this.bgRoot2.DOFade(0f, fadeOutDuration);
			});
			seq.AppendCallback(delegate
			{
				this.iconRoot.DOFade(0f, fadeOutDuration);
			});
			seq.AppendInterval(fadeOutDuration);
			seq.AppendCallback(delegate
			{
				UIManager.Instance.HideUI(this.Element);
			});
			seq.Play<Sequence>();
		}

		// Token: 0x060053EF RID: 21487 RVA: 0x0026E2E8 File Offset: 0x0026C4E8
		private void SetupPage()
		{
			this.newAreaBg.gameObject.SetActive(!ViewNewAreaNotify._isVisited);
			this.arriveBg.gameObject.SetActive(ViewNewAreaNotify._isVisited);
			this.RefreshTitle();
			MapAreaItem areaConfig = MapArea.Instance[ViewNewAreaNotify._areaTemplateId];
			MapStateItem stateConfig = MapState.Instance[areaConfig.StateID];
			this.areaName.text = stateConfig.Name + LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + areaConfig.Name;
			this.areaIcon.SetSprite(areaConfig.BigMapIcon, false, null);
			sbyte orgTemplateId = MapState.Instance[areaConfig.StateID].SectID;
			OrganizationItem orgConfig = Organization.Instance[orgTemplateId];
			bool flag = ViewNewAreaNotify._areaTemplateId == (short)stateConfig.SectAreaID;
			if (flag)
			{
				this.subAreaNameText.text = orgConfig.Name;
				this.ShowSubAreaIcon(string.Format("{0}{1}", "ui9_tex_main_arrive_icon_1_", (int)(orgTemplateId - 1)));
			}
			else
			{
				this.ShowSubAreaIcon(string.Empty);
				foreach (MerchantTypeItem merchantTypeConfig in ((IEnumerable<MerchantTypeItem>)MerchantType.Instance))
				{
					bool flag2 = merchantTypeConfig.HeadArea == ViewNewAreaNotify._areaTemplateId;
					if (flag2)
					{
						this.subAreaNameText.text = merchantTypeConfig.Name + LocalStringManager.Get(LanguageKey.LK_NewAreaNotify_Merchant_Head);
						this.ShowSubAreaIcon(string.Format("{0}{1}", "ui9_tex_main_arrive_icon_2_", merchantTypeConfig.TemplateId));
						break;
					}
					bool flag3 = merchantTypeConfig.BranchArea == ViewNewAreaNotify._areaTemplateId;
					if (flag3)
					{
						this.subAreaNameText.text = merchantTypeConfig.Name + LocalStringManager.Get(LanguageKey.LK_NewAreaNotify_Merchant_Branch);
						this.ShowSubAreaIcon(string.Format("{0}{1}", "ui9_tex_main_arrive_icon_2_", merchantTypeConfig.TemplateId));
						break;
					}
				}
			}
		}

		// Token: 0x060053F0 RID: 21488 RVA: 0x0026E500 File Offset: 0x0026C700
		private void RefreshTitle()
		{
			string languageShortName = this.GetCurrentLanguageShortName();
			string arriveOrFound = ViewNewAreaNotify._isVisited ? this._iconNameArrive : this._iconNameFound;
			this.areaTitleImg.SetSprite(string.Format(this._iconNameFormat, arriveOrFound, languageShortName), false, null);
		}

		// Token: 0x060053F1 RID: 21489 RVA: 0x0026E548 File Offset: 0x0026C748
		private string GetCurrentLanguageShortName()
		{
			string result;
			switch (LocalStringManager.CurLanguageType)
			{
			case LocalStringManager.LanguageType.EN:
				result = this._languageShortNameEn;
				break;
			case LocalStringManager.LanguageType.KO:
				result = this._languageShortNameKr;
				break;
			case LocalStringManager.LanguageType.CNH:
				result = this._languageShortNameTc;
				break;
			default:
				result = this._languageShortNameSc;
				break;
			}
			return result;
		}

		// Token: 0x060053F2 RID: 21490 RVA: 0x0026E599 File Offset: 0x0026C799
		private void OnDisable()
		{
		}

		// Token: 0x060053F3 RID: 21491 RVA: 0x0026E59C File Offset: 0x0026C79C
		public override void QuickHide()
		{
			base.QuickHide();
		}

		// Token: 0x060053F4 RID: 21492 RVA: 0x0026E5A8 File Offset: 0x0026C7A8
		private void TopUiChanged(ArgumentBox argBox)
		{
			bool flag = this.animRoot == null;
			if (!flag)
			{
				bool flag2 = !UIManager.Instance.IsFocusElement(UIElement.StateMainWorld) && !UIManager.Instance.IsFocusElement(this.Element);
				if (flag2)
				{
					this.animRoot.alpha = 0f;
				}
			}
		}

		// Token: 0x060053F5 RID: 21493 RVA: 0x0026E608 File Offset: 0x0026C808
		private void ShowSubAreaIcon(string iconName)
		{
			bool flag = string.IsNullOrWhiteSpace(iconName);
			if (flag)
			{
				this.contentLayoutGroup.padding.top = this.singleIconTop;
				this.subArea.gameObject.SetActive(false);
			}
			else
			{
				this.contentLayoutGroup.padding.top = this.doubleIconTop;
				this.subArea.gameObject.SetActive(true);
				this.subAreaIcon.SetTexture(iconName);
			}
		}

		// Token: 0x060053F6 RID: 21494 RVA: 0x0026E688 File Offset: 0x0026C888
		public static bool CheckNeedShow()
		{
			return ViewNewAreaNotify._areaTemplateId >= 0;
		}

		// Token: 0x040038D0 RID: 14544
		[Header("发现地区时的显示时间")]
		public float DefaultShowTimeFound = 2f;

		// Token: 0x040038D1 RID: 14545
		[Header("抵达地区时的显示时间")]
		public float DefaultShowTimeArrive = 1f;

		// Token: 0x040038D2 RID: 14546
		[SerializeField]
		private CanvasGroup animRoot;

		// Token: 0x040038D3 RID: 14547
		[SerializeField]
		private CanvasGroup bgRoot;

		// Token: 0x040038D4 RID: 14548
		[SerializeField]
		private CanvasGroup bgRoot2;

		// Token: 0x040038D5 RID: 14549
		[SerializeField]
		private CanvasGroup iconRoot;

		// Token: 0x040038D6 RID: 14550
		[SerializeField]
		private CRawImage newAreaBg;

		// Token: 0x040038D7 RID: 14551
		[SerializeField]
		private CRawImage arriveBg;

		// Token: 0x040038D8 RID: 14552
		[Header("提示Title图片")]
		[SerializeField]
		private CImage areaTitleImg;

		// Token: 0x040038D9 RID: 14553
		[SerializeField]
		private CImage areaIcon;

		// Token: 0x040038DA RID: 14554
		[SerializeField]
		private TextMeshProUGUI areaName;

		// Token: 0x040038DB RID: 14555
		[Header("门派或商会")]
		[SerializeField]
		private GameObject subArea;

		// Token: 0x040038DC RID: 14556
		[SerializeField]
		private CRawImage subAreaIcon;

		// Token: 0x040038DD RID: 14557
		[SerializeField]
		private TextMeshProUGUI subAreaNameText;

		// Token: 0x040038DE RID: 14558
		[SerializeField]
		private VerticalLayoutGroup contentLayoutGroup;

		// Token: 0x040038DF RID: 14559
		[SerializeField]
		private int singleIconTop = 0;

		// Token: 0x040038E0 RID: 14560
		[SerializeField]
		private int doubleIconTop = 30;

		// Token: 0x040038E1 RID: 14561
		private float _showTime;

		// Token: 0x040038E2 RID: 14562
		private static bool _isVisited = false;

		// Token: 0x040038E3 RID: 14563
		private static short _areaTemplateId = -1;

		// Token: 0x040038E4 RID: 14564
		private string _iconNameFormat = "ui9_back_{0}_text_{1}";

		// Token: 0x040038E5 RID: 14565
		private string _iconNameArrive = "arrive";

		// Token: 0x040038E6 RID: 14566
		private string _iconNameFound = "found";

		// Token: 0x040038E7 RID: 14567
		private string _languageShortNameSc = "sc";

		// Token: 0x040038E8 RID: 14568
		private string _languageShortNameTc = "tc";

		// Token: 0x040038E9 RID: 14569
		private string _languageShortNameEn = "en";

		// Token: 0x040038EA RID: 14570
		private string _languageShortNameJp = "jp";

		// Token: 0x040038EB RID: 14571
		private string _languageShortNameKr = "kr";
	}
}
