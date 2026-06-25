using System;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F43 RID: 3907
	public class TeammateCommandLongItem : MonoBehaviour
	{
		// Token: 0x0600B36C RID: 45932 RVA: 0x0051A848 File Offset: 0x00518A48
		public void Set(short templateId, bool isNormal)
		{
			bool flag = templateId < 0;
			if (flag)
			{
				this.content.SetActive(false);
				bool flag2 = this.empty != null;
				if (flag2)
				{
					this.empty.SetActive(true);
				}
			}
			else
			{
				TeammateCommandItem config = TeammateCommand.Instance[(int)templateId];
				this.command.Set((int)templateId, isNormal);
				this.medalIcon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)config.MedalType, (int)config.MedalCount), false, null);
				this.medalName.text = CommonUtils.GetFeatureMedalTypeText((int)config.MedalType);
				this.medalValue.text = string.Format("x{0}", config.MedalCount);
				this.desc.text = config.Description.ColorReplace();
				this.content.SetActive(true);
				bool flag3 = this.empty != null;
				if (flag3)
				{
					this.empty.SetActive(false);
				}
				this.pointerTrigger.EnterEvent.RemoveAllListeners();
				this.pointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.hover.SetActive(true);
				});
				this.pointerTrigger.ExitEvent.RemoveAllListeners();
				this.pointerTrigger.ExitEvent.AddListener(delegate()
				{
					this.hover.SetActive(false);
				});
			}
		}

		// Token: 0x0600B36D RID: 45933 RVA: 0x0051A99C File Offset: 0x00518B9C
		public void SetIsDisableByType(sbyte type)
		{
			bool enable = type >= 0;
			this.tips.enabled = enable;
			this.pointerTrigger.enabled = !enable;
			bool flag = !enable;
			if (!flag)
			{
				string text = LocalStringManager.GetFormat(LanguageKey.LK_ChangeTeammateCommand_Unavailable_Tips, CommonUtils.GetFeatureMedalTypeText((int)type));
				TooltipInvoker tooltipInvoker = this.tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.tips.RuntimeParam.Set("arg0", text);
			}
		}

		// Token: 0x04008B56 RID: 35670
		[SerializeField]
		protected TeammateCommand command;

		// Token: 0x04008B57 RID: 35671
		[SerializeField]
		protected CImage medalIcon;

		// Token: 0x04008B58 RID: 35672
		[SerializeField]
		protected TextMeshProUGUI medalName;

		// Token: 0x04008B59 RID: 35673
		[SerializeField]
		protected TextMeshProUGUI medalValue;

		// Token: 0x04008B5A RID: 35674
		[SerializeField]
		protected TextMeshProUGUI desc;

		// Token: 0x04008B5B RID: 35675
		[SerializeField]
		protected GameObject content;

		// Token: 0x04008B5C RID: 35676
		[SerializeField]
		protected TooltipInvoker tips;

		// Token: 0x04008B5D RID: 35677
		[SerializeField]
		protected PointerTrigger pointerTrigger;

		// Token: 0x04008B5E RID: 35678
		[SerializeField]
		protected GameObject hover;

		// Token: 0x04008B5F RID: 35679
		[SerializeField]
		protected GameObject empty;
	}
}
