using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.RecordSelect
{
	// Token: 0x020007B8 RID: 1976
	public class ContentItem : MonoBehaviour
	{
		// Token: 0x06006062 RID: 24674 RVA: 0x002C34CC File Offset: 0x002C16CC
		public void Set(bool isAdded, string contentName, string author, Action action, string tipContent)
		{
			this.status.text = (isAdded ? LanguageKey.LK_RecordContent_Increased_Tip_Title.Tr() : LanguageKey.LK_RecordContent_Reduced_Tip_Title.Tr());
			this.back.sprite = this.sprites[isAdded ? 1 : 0];
			this.contentName.text = contentName;
			this.author.text = author;
			this.cButton.ClearAndAddListener(delegate
			{
				Action action2 = action;
				if (action2 != null)
				{
					action2();
				}
			});
			TooltipInvoker tooltipInvoker = this.bgTipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.bgTipDisplayer.RuntimeParam.Set("arg0", tipContent);
			this.bgTipDisplayer.enabled = !tipContent.IsNullOrEmpty();
			this.RefreshTip(this.tipDisplayer, isAdded);
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x002C35B4 File Offset: 0x002C17B4
		private void RefreshTip(TooltipInvoker tip, bool isAdded)
		{
			tip.Type = TipType.Simple;
			bool flag = tip.PresetParam == null || tip.PresetParam.Length < 2;
			if (flag)
			{
				tip.PresetParam = new string[2];
			}
			LanguageKey titleKey = isAdded ? LanguageKey.LK_RecordContent_Increased_Tip_Title : LanguageKey.LK_RecordContent_Reduced_Tip_Title;
			LanguageKey contentKey = isAdded ? LanguageKey.LK_RecordContent_Increased_Tip_Content : LanguageKey.LK_RecordContent_Reduced_Tip_Content;
			tip.PresetParam[0] = LocalStringManager.Get(titleKey);
			tip.PresetParam[1] = LocalStringManager.Get(contentKey);
		}

		// Token: 0x040042CB RID: 17099
		[SerializeField]
		private TextMeshProUGUI status;

		// Token: 0x040042CC RID: 17100
		[SerializeField]
		private TextMeshProUGUI contentName;

		// Token: 0x040042CD RID: 17101
		[SerializeField]
		private TextMeshProUGUI author;

		// Token: 0x040042CE RID: 17102
		[SerializeField]
		private CButton cButton;

		// Token: 0x040042CF RID: 17103
		[SerializeField]
		private CImage back;

		// Token: 0x040042D0 RID: 17104
		[SerializeField]
		private TooltipInvoker tipDisplayer;

		// Token: 0x040042D1 RID: 17105
		[SerializeField]
		private TooltipInvoker bgTipDisplayer;

		// Token: 0x040042D2 RID: 17106
		[SerializeField]
		private Sprite[] sprites;
	}
}
