using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200088D RID: 2189
	public class TooltipCombatSkillRequirementItem : MonoBehaviour
	{
		// Token: 0x060068FD RID: 26877 RVA: 0x003037F0 File Offset: 0x003019F0
		public void Set(string iconSpriteName, string title, int actual, int required, bool needGray = false)
		{
			this.Set(iconSpriteName, title, actual, required, needGray, false);
		}

		// Token: 0x060068FE RID: 26878 RVA: 0x00303804 File Offset: 0x00301A04
		public void Set(string iconSpriteName, string title, int actual, int required, bool needGray, bool showSpecialBack)
		{
			bool flag = !string.IsNullOrEmpty(iconSpriteName);
			if (flag)
			{
				this.icon.SetSprite(iconSpriteName, false, null);
			}
			this.titleText.text = title;
			bool flag2 = actual >= 0;
			if (flag2)
			{
				this.actualText.text = actual.ToString();
				this.slashText.text = "/";
				this.requiredText.text = required.ToString();
				this.actualText.gameObject.SetActive(true);
				this.slashText.gameObject.SetActive(true);
			}
			else
			{
				this.actualText.text = "-";
				this.slashText.text = "/";
				this.requiredText.text = required.ToString();
				this.actualText.gameObject.SetActive(true);
				this.slashText.gameObject.SetActive(true);
			}
			if (needGray)
			{
				this.colorStyleRoot.SetColor(Colors.Instance["grey"], null);
			}
			else
			{
				this.titleText.color = Colors.Instance["lightgrey"];
				this.actualText.color = ((actual >= 0 && actual < required) ? Colors.Instance["brightred"] : Colors.Instance["brightblue"]);
				this.slashText.color = Colors.Instance["brightyellow"];
				this.requiredText.color = Colors.Instance["brightyellow"];
				this.icon.SetColor(Color.white);
			}
			bool flag3 = this.specialBack;
			if (flag3)
			{
				this.specialBack.enabled = showSpecialBack;
			}
		}

		// Token: 0x04004B22 RID: 19234
		[SerializeField]
		private CImage icon;

		// Token: 0x04004B23 RID: 19235
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04004B24 RID: 19236
		[SerializeField]
		private TextMeshProUGUI actualText;

		// Token: 0x04004B25 RID: 19237
		[SerializeField]
		private TextMeshProUGUI slashText;

		// Token: 0x04004B26 RID: 19238
		[SerializeField]
		private TextMeshProUGUI requiredText;

		// Token: 0x04004B27 RID: 19239
		[SerializeField]
		private ColorStyleRoot colorStyleRoot;

		// Token: 0x04004B28 RID: 19240
		[SerializeField]
		private CImage specialBack;
	}
}
