using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000854 RID: 2132
	public class MouseTipFeatureMedal : MouseTipBase
	{
		// Token: 0x17000C6C RID: 3180
		// (get) Token: 0x0600678B RID: 26507 RVA: 0x002F4E0F File Offset: 0x002F300F
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600678C RID: 26508 RVA: 0x002F4E12 File Offset: 0x002F3012
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x0600678D RID: 26509 RVA: 0x002F4E20 File Offset: 0x002F3020
		public override void Refresh(ArgumentBox argBox)
		{
			sbyte medalType;
			argBox.Get("MouseTipMedalType", out medalType);
			bool flag = medalType < 0 || medalType > 2;
			if (!flag)
			{
				string medalName = CommonUtils.GetFeatureMedalTypeText((int)medalType);
				this.desc.text = LocalStringManager.Get(string.Format("LK_MouseTip_FeatureMedal_Desc_{0}", medalType)).ColorReplace();
				this.titleLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_Title, medalName);
				this.descIcon.SetSprite(CommonUtils.GetFeatureMedalIcon((int)medalType, 0), false, null);
				this.commandTitleLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_SubTitle, medalName);
				List<TeammateCommandItem> commands = this.GetCommands(medalType);
				this.PreserveCommandItems(commands.Count);
				for (int i = 0; i < commands.Count; i++)
				{
					Transform itemTrans = this.commandArea.GetChild(i + 1);
					MouseTipFeatureMedalCommandItem commandItem = itemTrans.GetComponent<MouseTipFeatureMedalCommandItem>();
					commandItem.Set(medalType, commands[i]);
				}
				if (!true)
				{
				}
				int num;
				switch (medalType)
				{
				case 0:
					num = 0;
					break;
				case 1:
					num = 1;
					break;
				case 2:
					num = 3;
					break;
				default:
					num = -1;
					break;
				}
				if (!true)
				{
				}
				int iconIndex = num;
				this.extraSubTitle.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_FeatureMedal_ExtraTitle, iconIndex, medalName).ColorReplace();
				TMPTextSpriteHelper extraTitleHelper;
				bool flag2 = this.extraSubTitle.TryGetComponent<TMPTextSpriteHelper>(out extraTitleHelper);
				if (flag2)
				{
					extraTitleHelper.Parse();
				}
				bool flag3 = this.effectTextLabelList.Count > 0;
				if (flag3)
				{
					this.effectTextLabelList[0].text = LocalStringManager.GetFormat("LK_MouseTip_FeatureMedal_ExtraContent_0", iconIndex, medalName).ColorReplace();
					TMPTextSpriteHelper helper0;
					bool flag4 = this.effectTextLabelList[0].TryGetComponent<TMPTextSpriteHelper>(out helper0);
					if (flag4)
					{
						helper0.Parse();
					}
				}
				for (int j = 1; j < this.effectTextLabelList.Count; j++)
				{
					TextMeshProUGUI label = this.effectTextLabelList[j];
					label.transform.parent.gameObject.SetActive(medalType == 2);
					bool flag5 = medalType != 2;
					if (!flag5)
					{
						label.text = LocalStringManager.Get(string.Format("LK_MouseTip_FeatureMedal_ExtraContent_{0}", j)).ColorReplace();
						TMPTextSpriteHelper helper;
						bool flag6 = label.TryGetComponent<TMPTextSpriteHelper>(out helper);
						if (flag6)
						{
							helper.Parse();
						}
					}
				}
			}
		}

		// Token: 0x0600678E RID: 26510 RVA: 0x002F5088 File Offset: 0x002F3288
		private List<TeammateCommandItem> GetCommands(sbyte medalType)
		{
			this._cachedCommands.Clear();
			foreach (TeammateCommandItem configItem in ((IEnumerable<TeammateCommandItem>)TeammateCommand.Instance))
			{
				bool flag = configItem.MedalType == medalType && configItem.Type == ETeammateCommandType.Normal;
				if (flag)
				{
					this._cachedCommands.Add(configItem);
				}
			}
			return this._cachedCommands;
		}

		// Token: 0x0600678F RID: 26511 RVA: 0x002F510C File Offset: 0x002F330C
		private void PreserveCommandItems(int count)
		{
			RectTransform parent = this.commandArea;
			for (int i = 0; i < count; i++)
			{
				bool flag = i >= parent.childCount - 1;
				if (flag)
				{
					Object.Instantiate<GameObject>(this.commandTemplate, parent);
				}
				parent.GetChild(i + 1).gameObject.SetActive(true);
			}
			for (int j = count; j < parent.childCount - 1; j++)
			{
				parent.GetChild(j + 1).gameObject.SetActive(false);
			}
		}

		// Token: 0x04004920 RID: 18720
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04004921 RID: 18721
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04004922 RID: 18722
		[SerializeField]
		private CImage descIcon;

		// Token: 0x04004923 RID: 18723
		[SerializeField]
		private TextMeshProUGUI commandTitleLabel;

		// Token: 0x04004924 RID: 18724
		[SerializeField]
		private RectTransform commandArea;

		// Token: 0x04004925 RID: 18725
		[SerializeField]
		private GameObject commandTemplate;

		// Token: 0x04004926 RID: 18726
		[SerializeField]
		private TextMeshProUGUI extraSubTitle;

		// Token: 0x04004927 RID: 18727
		[SerializeField]
		private List<TextMeshProUGUI> effectTextLabelList;

		// Token: 0x04004928 RID: 18728
		private const int OtherChildCount = 1;

		// Token: 0x04004929 RID: 18729
		private readonly List<TeammateCommandItem> _cachedCommands = new List<TeammateCommandItem>();
	}
}
