using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F42 RID: 3906
	public class TeammateCommand : MonoBehaviour
	{
		// Token: 0x0600B368 RID: 45928 RVA: 0x0051A6A4 File Offset: 0x005188A4
		public void Set(int commandId, Dictionary<int, int> medalAvailability)
		{
			bool flag = medalAvailability == null;
			if (!flag)
			{
				bool flag2 = commandId < 0;
				if (!flag2)
				{
					TeammateCommandItem config = TeammateCommand.Instance[commandId];
					bool flag3 = config == null;
					if (!flag3)
					{
						this.nameLabel.SetText(config.Name, true);
						bool isMedalEnough = config.MedalType < 0 || (medalAvailability.ContainsKey((int)config.MedalType) && (int)config.MedalCount <= medalAvailability[(int)config.MedalType]);
						this.stateImage.sprite = (isMedalEnough ? ((config.Type == ETeammateCommandType.Advance) ? this.advanceSprite : this.normalSprite) : this.disableSprite);
						this.SetupTip(commandId);
					}
				}
			}
		}

		// Token: 0x0600B369 RID: 45929 RVA: 0x0051A764 File Offset: 0x00518964
		public void Set(int commandId, bool isNormal)
		{
			bool flag = commandId < 0;
			if (!flag)
			{
				TeammateCommandItem config = TeammateCommand.Instance[commandId];
				bool flag2 = config == null;
				if (!flag2)
				{
					this.nameLabel.SetText(config.Name, true);
					this.stateImage.sprite = (isNormal ? ((config.Type == ETeammateCommandType.Advance) ? this.advanceSprite : this.normalSprite) : this.disableSprite);
					this.SetupTip(commandId);
				}
			}
		}

		// Token: 0x0600B36A RID: 45930 RVA: 0x0051A7DC File Offset: 0x005189DC
		private void SetupTip(int commandId)
		{
			this.tip.Type = TipType.TeammateCommand;
			TooltipInvoker tooltipInvoker = this.tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.tip.enabled = MouseTipTeammateCommand.CanUse((sbyte)commandId);
			this.tip.RuntimeParam.Set("CommandId", (sbyte)commandId);
		}

		// Token: 0x04008B50 RID: 35664
		[SerializeField]
		protected TextMeshProUGUI nameLabel;

		// Token: 0x04008B51 RID: 35665
		[SerializeField]
		protected CImage stateImage;

		// Token: 0x04008B52 RID: 35666
		[SerializeField]
		protected Sprite normalSprite;

		// Token: 0x04008B53 RID: 35667
		[SerializeField]
		protected Sprite disableSprite;

		// Token: 0x04008B54 RID: 35668
		[SerializeField]
		protected Sprite advanceSprite;

		// Token: 0x04008B55 RID: 35669
		[SerializeField]
		protected TooltipInvoker tip;
	}
}
