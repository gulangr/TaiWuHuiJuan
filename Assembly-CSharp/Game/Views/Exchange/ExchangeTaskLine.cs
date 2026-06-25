using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A28 RID: 2600
	public class ExchangeTaskLine : MonoBehaviour
	{
		// Token: 0x06007F73 RID: 32627 RVA: 0x003B5A28 File Offset: 0x003B3C28
		private string Color(int count)
		{
			bool flag = this._task.Advantage > 0;
			bool flag2 = this._task.Limit == -1;
			bool flag3 = count == 0;
			bool flag4 = count < this._task.Limit;
			if (!true)
			{
			}
			string result;
			if (!flag3)
			{
				if (!flag2 && !flag4)
				{
					result = "orange";
				}
				else
				{
					result = "brightyellow";
				}
			}
			else
			{
				result = "grey";
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007F74 RID: 32628 RVA: 0x003B5A98 File Offset: 0x003B3C98
		public void Set(ExchangeTaskItem task, int count)
		{
			this._task = task;
			this.textDesc.text = string.Format("{0}({1}/{2})", task.Desc, count, (task.Limit < 0) ? LanguageKey.LK_Infinity.Tr() : task.Limit.ToString()).SetColor(this.Color(count));
			TMP_Text tmp_Text = this.textValue;
			CImage cimage = this.img;
			if (task.Advantage >= 0)
			{
				string text = LanguageKey.LK_Exchange_Advantage_Task_Display_Taiwu.TrFormat(task.Advantage).ColorReplace();
				Sprite sprite = this.incSp;
				tmp_Text.text = text;
				cimage.sprite = sprite;
			}
			else
			{
				string text = LanguageKey.LK_Exchange_Advantage_Task_Display_Target.TrFormat(-task.Advantage).ColorReplace();
				Sprite sprite = this.decSp;
				tmp_Text.text = text;
				cimage.sprite = sprite;
			}
			this.textDesc.ForceMeshUpdate(false, false);
			this.textValue.ForceMeshUpdate(false, false);
			float minValue = float.MinValue;
			float maxValue = float.MaxValue;
			float left = minValue;
			float right = maxValue;
			bool flag = this.textDesc.textInfo.characterCount > 0;
			if (flag)
			{
				left = this.textDesc.textInfo.characterInfo[this.textDesc.textInfo.characterCount - 1].bottomRight.x;
			}
			TMP_CharacterInfo[] info = this.textValue.textInfo.characterInfo;
			bool flag2 = info != null && info.Length > 0;
			if (flag2)
			{
				right = info[0].topLeft.x;
			}
			bool flag3 = left > right;
			if (flag3)
			{
				this.textDesc.text = string.Format("{0}({1}/{2})\n ", task.Desc, count, (task.Limit < 0) ? LanguageKey.LK_Infinity.Tr() : task.Limit.ToString()).SetColor(this.Color(count));
			}
		}

		// Token: 0x040061C2 RID: 25026
		[SerializeField]
		private Sprite incSp;

		// Token: 0x040061C3 RID: 25027
		[SerializeField]
		private Sprite decSp;

		// Token: 0x040061C4 RID: 25028
		[SerializeField]
		private CImage img;

		// Token: 0x040061C5 RID: 25029
		[SerializeField]
		private TMP_Text textDesc;

		// Token: 0x040061C6 RID: 25030
		[SerializeField]
		private TMP_Text textValue;

		// Token: 0x040061C7 RID: 25031
		private ExchangeTaskItem _task;
	}
}
