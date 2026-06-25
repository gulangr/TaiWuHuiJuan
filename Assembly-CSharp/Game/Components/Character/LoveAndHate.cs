using System;
using System.Text;
using FrameWork;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F36 RID: 3894
	public class LoveAndHate : MonoBehaviour
	{
		// Token: 0x0600B31A RID: 45850 RVA: 0x00518934 File Offset: 0x00516B34
		public void Set(CharacterLoveAndHateItemInfo data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this._currentData = data;
				base.gameObject.SetActive(data.CreatingType == 1);
				bool flag2 = data.CreatingType != 1;
				if (!flag2)
				{
					this.RefreshLove();
					this.RefreshHate();
					this.RefreshTime();
					this.RefreshTip();
				}
			}
		}

		// Token: 0x0600B31B RID: 45851 RVA: 0x0051899E File Offset: 0x00516B9E
		public void SetEmpty()
		{
			this._currentData = null;
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600B31C RID: 45852 RVA: 0x005189B8 File Offset: 0x00516BB8
		private void RefreshLove()
		{
			bool flag = this.loveTitle != null;
			if (flag)
			{
				this.loveTitle.text = LocalStringManager.Get(LanguageKey.LK_Loving).SetColor("favorite");
			}
			bool flag2 = this.loveValue != null;
			if (flag2)
			{
				string questionMark = LocalStringManager.Get(LanguageKey.LK_QuestioMark);
				string lovingName = questionMark + questionMark + questionMark;
				bool lovingItemRevealed = this._currentData.LovingItemRevealed;
				if (lovingItemRevealed)
				{
					lovingName = ((this._currentData.LovingItemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._currentData.LovingItemSubType)) : LocalStringManager.Get(LanguageKey.LK_None));
				}
				this.loveValue.text = lovingName;
			}
		}

		// Token: 0x0600B31D RID: 45853 RVA: 0x00518A78 File Offset: 0x00516C78
		private void RefreshHate()
		{
			bool flag = this.hateTitle != null;
			if (flag)
			{
				this.hateTitle.text = LocalStringManager.Get(LanguageKey.LK_Hate).SetColor("hate");
			}
			bool flag2 = this.hateValue != null;
			if (flag2)
			{
				string questionMark = LocalStringManager.Get(LanguageKey.LK_QuestioMark);
				string hateName = questionMark + questionMark + questionMark;
				bool hatingItemRevealed = this._currentData.HatingItemRevealed;
				if (hatingItemRevealed)
				{
					hateName = ((this._currentData.HatingItemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._currentData.HatingItemSubType)) : LocalStringManager.Get(LanguageKey.LK_None));
				}
				this.hateValue.text = hateName;
			}
		}

		// Token: 0x0600B31E RID: 45854 RVA: 0x00518B38 File Offset: 0x00516D38
		private void RefreshTime()
		{
			bool flag = this.timeLabel == null;
			if (!flag)
			{
				int time = this._currentData.HobbyExpirationDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
				string timeStr = (time <= 0) ? "-" : time.ToString();
				this.timeLabel.text = timeStr;
			}
		}

		// Token: 0x0600B31F RID: 45855 RVA: 0x00518B90 File Offset: 0x00516D90
		private void RefreshTip()
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.Type = TipType.Simple;
				string title = LocalStringManager.Get(LanguageKey.LK_Hobby_Tip_Tittle);
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				string questionMark = LocalStringManager.Get(LanguageKey.LK_QuestioMark);
				string lovingName = questionMark + questionMark + questionMark;
				bool lovingItemRevealed = this._currentData.LovingItemRevealed;
				if (lovingItemRevealed)
				{
					lovingName = ((this._currentData.LovingItemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._currentData.LovingItemSubType)) : LocalStringManager.Get(LanguageKey.LK_None));
				}
				bool flag2 = !this._currentData.LovingItemRevealed || (this._currentData.LovingItemRevealed && this._currentData.LovingItemSubType >= 0);
				if (flag2)
				{
					string lovingTip = LocalStringManager.GetFormat(LanguageKey.LK_Hobby_Tip_Loving, lovingName.SetColor("favorite"));
					strBuilder.Append(lovingTip);
					strBuilder.Append("\n");
				}
				string hateName = questionMark + questionMark + questionMark;
				bool hatingItemRevealed = this._currentData.HatingItemRevealed;
				if (hatingItemRevealed)
				{
					hateName = ((this._currentData.HatingItemSubType >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._currentData.HatingItemSubType)) : LocalStringManager.Get(LanguageKey.LK_None));
				}
				bool flag3 = !this._currentData.HatingItemRevealed || (this._currentData.HatingItemRevealed && this._currentData.HatingItemSubType >= 0);
				if (flag3)
				{
					string hateTip = LocalStringManager.GetFormat(LanguageKey.LK_Hobby_Tip_Hate, hateName.SetColor("hate"));
					strBuilder.Append(hateTip);
					strBuilder.Append("\n");
				}
				int time = this._currentData.HobbyExpirationDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
				string timeStr = (time <= 0) ? "-" : time.ToString();
				string timeTip = LocalStringManager.GetFormat(LanguageKey.LK_Hobby_Tip_Time, timeStr);
				strBuilder.Append(timeTip);
				string content = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
				this.mouseTip.PresetParam = new string[]
				{
					title,
					content
				};
			}
		}

		// Token: 0x04008B1C RID: 35612
		[Header("喜爱物品")]
		[SerializeField]
		private TextMeshProUGUI loveTitle;

		// Token: 0x04008B1D RID: 35613
		[SerializeField]
		private TextMeshProUGUI loveValue;

		// Token: 0x04008B1E RID: 35614
		[Header("厌恶物品")]
		[SerializeField]
		private TextMeshProUGUI hateTitle;

		// Token: 0x04008B1F RID: 35615
		[SerializeField]
		private TextMeshProUGUI hateValue;

		// Token: 0x04008B20 RID: 35616
		[Header("剩余时间")]
		[SerializeField]
		private TextMeshProUGUI timeLabel;

		// Token: 0x04008B21 RID: 35617
		[Header("鼠标提示")]
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008B22 RID: 35618
		private CharacterLoveAndHateItemInfo _currentData;
	}
}
