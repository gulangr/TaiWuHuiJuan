using System;
using System.Diagnostics;
using Game.Components.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C6 RID: 2502
	public class ZhujianGearMateAttribute : MonoBehaviour
	{
		// Token: 0x06007962 RID: 31074 RVA: 0x00386A80 File Offset: 0x00384C80
		private void OnEnable()
		{
			bool isChinese = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			Sprite sprite = isChinese ? this.bgSpriteCn : this.bgSpriteEn;
			this.background.sprite = sprite;
			RectTransform rt = base.GetComponent<RectTransform>();
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.rect.width);
			this.arrowLayout.spacing = (isChinese ? this.arrowLayoutSpaceCn : this.arrowLayoutSpaceEn);
		}

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x06007963 RID: 31075 RVA: 0x00386AF0 File Offset: 0x00384CF0
		// (remove) Token: 0x06007964 RID: 31076 RVA: 0x00386B28 File Offset: 0x00384D28
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> OnFilterButtonClicked;

		// Token: 0x06007965 RID: 31077 RVA: 0x00386B60 File Offset: 0x00384D60
		public void InitFilterButton()
		{
			bool flag = this.filterButton != null;
			if (flag)
			{
				this.filterButton.onClick.AddListener(delegate()
				{
					Action<int> onFilterButtonClicked = this.OnFilterButtonClicked;
					if (onFilterButtonClicked != null)
					{
						onFilterButtonClicked(this._attributeType);
					}
				});
			}
		}

		// Token: 0x06007966 RID: 31078 RVA: 0x00386B9B File Offset: 0x00384D9B
		public void SetLightActive(bool active)
		{
			this.lightEffect.SetActive(active);
		}

		// Token: 0x06007967 RID: 31079 RVA: 0x00386BAC File Offset: 0x00384DAC
		public void Refresh(int attributeType, int currentAttrValue, int percent, int previewPercent, bool showPreview, int increaseCount = 0, int deltaPercent = 0)
		{
			this._attributeType = attributeType;
			this.icon.SetSprite("ui9_icon_attribute_big_" + attributeType.ToString(), false, null);
			string attributeName = Attribute.GetMainAttributeName((sbyte)attributeType);
			this.SetTitle(attributeName);
			this.curValue.text = currentAttrValue.ToString();
			string gradeColor = "PersonalityType_Calm";
			this.progressText.text = ((showPreview && deltaPercent > 0) ? (string.Format("{0}%", percent).SetColor(gradeColor) + string.Format("+{0}%", deltaPercent).SetColor("FiveElementType_Xuanyin")) : string.Format("{0}%", percent).SetColor(gradeColor));
			this.progressBar.fillAmount = (float)percent / 100f;
			this.upgradePreview.fillAmount = (float)previewPercent / 100f;
			this.upgradePreview.gameObject.SetActive(showPreview);
			int targetValue = Mathf.Clamp(currentAttrValue + increaseCount, 0, (int)GlobalConfig.Instance.MaxValueOfMaxMainAttributes);
			this.newValue.text = targetValue.ToString();
		}

		// Token: 0x06007968 RID: 31080 RVA: 0x00386CD0 File Offset: 0x00384ED0
		public void SetProgress(float percent)
		{
			this.progressBar.fillAmount = percent;
		}

		// Token: 0x06007969 RID: 31081 RVA: 0x00386CE0 File Offset: 0x00384EE0
		public void SetValueText(string text)
		{
			this.curValue.text = text;
		}

		// Token: 0x0600796A RID: 31082 RVA: 0x00386CF0 File Offset: 0x00384EF0
		public void SetTitle(string titleContent)
		{
			this.title.text = titleContent;
		}

		// Token: 0x04005C04 RID: 23556
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04005C05 RID: 23557
		[SerializeField]
		private TextMeshProUGUI curValue;

		// Token: 0x04005C06 RID: 23558
		[SerializeField]
		private TextMeshProUGUI progressText;

		// Token: 0x04005C07 RID: 23559
		[SerializeField]
		private CImage progressBar;

		// Token: 0x04005C08 RID: 23560
		[SerializeField]
		private CImage upgradePreview;

		// Token: 0x04005C09 RID: 23561
		[SerializeField]
		private CImage icon;

		// Token: 0x04005C0A RID: 23562
		[SerializeField]
		private GameObject lightEffect;

		// Token: 0x04005C0B RID: 23563
		[SerializeField]
		private TextMeshProUGUI newValue;

		// Token: 0x04005C0C RID: 23564
		[SerializeField]
		private Button filterButton;

		// Token: 0x04005C0D RID: 23565
		[SerializeField]
		private CImage background;

		// Token: 0x04005C0E RID: 23566
		[SerializeField]
		private Sprite bgSpriteCn;

		// Token: 0x04005C0F RID: 23567
		[SerializeField]
		private Sprite bgSpriteEn;

		// Token: 0x04005C10 RID: 23568
		[SerializeField]
		private HorizontalLayoutGroup arrowLayout;

		// Token: 0x04005C11 RID: 23569
		[SerializeField]
		private float arrowLayoutSpaceCn = 25f;

		// Token: 0x04005C12 RID: 23570
		[SerializeField]
		private float arrowLayoutSpaceEn = 16f;

		// Token: 0x04005C14 RID: 23572
		private int _attributeType;
	}
}
