using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Buildings
{
	// Token: 0x02000BBF RID: 3007
	public class PracticeRoomPuppetMode : MonoBehaviour, ILanguage
	{
		// Token: 0x06009781 RID: 38785 RVA: 0x0046918C File Offset: 0x0046738C
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			CButton btn = base.GetComponent<CButton>();
			this.RefreshImage(btn);
		}

		// Token: 0x06009782 RID: 38786 RVA: 0x004691AC File Offset: 0x004673AC
		private void RefreshImage(Button btn)
		{
			bool flag = btn == null || this.imageText == null;
			if (!flag)
			{
				string imagePattern = btn.interactable ? this.interactableSpritePattern : this.nonInteractableSpritePattern;
				string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
				string spriteName = string.Format(imagePattern, language);
				this.imageText.SetSprite(spriteName, false, null);
			}
		}

		// Token: 0x06009783 RID: 38787 RVA: 0x00469218 File Offset: 0x00467418
		public void Set(int requirement, int total, bool canUse)
		{
			this._canUse = canUse;
			this._actionPointEnough = (total >= requirement);
			bool canInteract = this._canUse && this._actionPointEnough;
			this.actionPointLabel.text = requirement.ToString().SetColor(this._actionPointEnough ? "brightblue" : "brightred") + "/" + total.ToString();
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform.GetChild(0).GetComponent<RectTransform>());
			CButton btn = base.GetComponent<CButton>();
			btn.interactable = canInteract;
			this.RefreshImage(btn);
		}

		// Token: 0x06009784 RID: 38788 RVA: 0x004692B8 File Offset: 0x004674B8
		public void SetTips(bool isHardMode, sbyte consummate)
		{
			List<GeneralLineData> dataList = new List<GeneralLineData>();
			string title;
			if (isHardMode)
			{
				title = LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_HardModeTitle);
				dataList.Add(new GeneralLineData(7, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_HardModeContent)
				}, null));
				dataList.Add(new GeneralLineData(5, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_Command) + ": " + LocalStringManager.Get(LanguageKey.LK_Common_None).SetColor("pinkyellow")
				}, null));
				dataList.Add(new GeneralLineData(5, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_Reward) + ": " + GlobalConfig.Instance.CombatGetExpBase[(int)consummate].ToString().SetColor("brightblue")
				}, null));
			}
			else
			{
				title = LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_EasyModeTitle);
				dataList.Add(new GeneralLineData(7, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_EasyModeContent)
				}, null));
				dataList.Add(new GeneralLineData(5, new List<string>
				{
					string.Concat(new string[]
					{
						LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_Command),
						": ",
						LocalStringManager.Get(LanguageKey.LK_Combat_Puppet_EnemyUnyieldingFallenToggle_TipsTitle).SetColor("orange"),
						LocalStringManager.Get(LanguageKey.LK_Split_Symbol),
						LocalStringManager.Get(LanguageKey.LK_Combat_Puppet_DisableEnemyAiToggle_TipsTitle).SetColor("orange")
					})
				}, null));
				dataList.Add(new GeneralLineData(5, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Building_KungfuRoomTips_Reward) + ": " + LocalStringManager.Get(LanguageKey.LK_Common_None).SetColor("pinkyellow")
				}, null));
			}
			bool flag = !this._canUse;
			if (flag)
			{
				dataList.Add(new GeneralLineData(7, new List<string>
				{
					LanguageKey.LK_Building_KungfuRoomTips1.Tr().SetColor("brightred")
				}, null));
			}
			bool flag2 = !this._actionPointEnough;
			if (flag2)
			{
				dataList.Add(new GeneralLineData(7, new List<string>
				{
					LanguageKey.LK_Time_Not_Enough.Tr().SetColor("brightred")
				}, null));
			}
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.RuntimeParam.Clear();
			this.mouseTip.RuntimeParam.Set("Title", title);
			this.mouseTip.RuntimeParam.Set("LineCount", dataList.Count);
			this.mouseTip.RuntimeParam.Set("DisableRaycastTarget", true);
			for (int i = 0; i < dataList.Count; i++)
			{
				this.mouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", i + 1), dataList[i]);
			}
			this.mouseTip.Refresh(false, -1);
		}

		// Token: 0x04007427 RID: 29735
		public TextMeshProUGUI actionPointLabel;

		// Token: 0x04007428 RID: 29736
		public TooltipInvoker mouseTip;

		// Token: 0x04007429 RID: 29737
		public CImage imageText;

		// Token: 0x0400742A RID: 29738
		[Tooltip("可用时的文字图片名，和LanguageRuleImagePattern使用方式一样")]
		public string interactableSpritePattern;

		// Token: 0x0400742B RID: 29739
		[Tooltip("不可用时的文字图片名，和LanguageRuleImagePattern使用方式一样")]
		public string nonInteractableSpritePattern;

		// Token: 0x0400742C RID: 29740
		private bool _canUse;

		// Token: 0x0400742D RID: 29741
		private bool _actionPointEnough;
	}
}
