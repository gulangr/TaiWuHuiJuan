using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C43 RID: 3139
	public class ResourceItem : MonoBehaviour
	{
		// Token: 0x06009F92 RID: 40850 RVA: 0x004A8F6A File Offset: 0x004A716A
		private void Awake()
		{
			CButton cbutton = this.button;
			if (cbutton != null)
			{
				cbutton.onClick.ResetListener(new Action(this.OpenResourceMenu));
			}
		}

		// Token: 0x06009F93 RID: 40851 RVA: 0x004A8F90 File Offset: 0x004A7190
		private void OnEnable()
		{
			bool flag = this.button != null;
			if (flag)
			{
				this.button.interactable = !SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			}
		}

		// Token: 0x06009F94 RID: 40852 RVA: 0x004A8FC7 File Offset: 0x004A71C7
		private void OpenResourceMenu()
		{
			UIManager.Instance.MaskUI(UIElement.ChoosyResource);
		}

		// Token: 0x06009F95 RID: 40853 RVA: 0x004A8FDC File Offset: 0x004A71DC
		public void SetBaseType(sbyte baseType)
		{
			TooltipInvoker tooltipInvoker = this.tipDisplayer;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = new ArgumentBox());
			}
			string key = "ResourceType";
			this.templateId = baseType;
			argumentBox.Set(key, baseType).Set("ShowDetailChange", true).Set("CharName", SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName).SetObject("ResourceDict", SingletonObject.getInstance<BuildingModel>().ResourceDict).Set("ShowOfferUpChange", true);
		}

		// Token: 0x06009F96 RID: 40854 RVA: 0x004A9060 File Offset: 0x004A7260
		public void Set(int baseVal, int deltaVal = 0)
		{
			this.baseValue.text = CommonUtils.GetDisplayStringForNum(baseVal, 100000);
			bool flag = deltaVal > 0;
			if (flag)
			{
				this.deltaValue.text = string.Format("+{0}", deltaVal);
			}
			else
			{
				bool flag2 = deltaVal < 0;
				if (flag2)
				{
					this.deltaValue.text = string.Format("<color=#brightred>{0}</color>", deltaVal).ColorReplace();
				}
			}
			this.deltaRect.gameObject.SetActive(deltaVal != 0);
			TooltipInvoker tooltipInvoker = this.tipDisplayer;
			if (tooltipInvoker != null)
			{
				ArgumentBox runtimeParam = tooltipInvoker.RuntimeParam;
				if (runtimeParam != null)
				{
					runtimeParam.Set("ResourceCount", baseVal);
				}
			}
		}

		// Token: 0x06009F97 RID: 40855 RVA: 0x004A910C File Offset: 0x004A730C
		public void SetPopulation(int baseVal, int deltaVal)
		{
			this.baseValue.text = CommonUtils.GetDisplayStringForNum(baseVal, 100000);
			this.deltaValue.text = deltaVal.ToString();
		}

		// Token: 0x04007B85 RID: 31621
		[SerializeField]
		private TMP_Text baseValue;

		// Token: 0x04007B86 RID: 31622
		[SerializeField]
		private TMP_Text deltaValue;

		// Token: 0x04007B87 RID: 31623
		[SerializeField]
		private RectTransform deltaRect;

		// Token: 0x04007B88 RID: 31624
		[SerializeField]
		private TooltipInvoker tipDisplayer;

		// Token: 0x04007B89 RID: 31625
		[SerializeField]
		private CButton button;

		// Token: 0x04007B8A RID: 31626
		[SerializeField]
		private sbyte templateId;
	}
}
