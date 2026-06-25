using System;
using System.Collections;
using FrameWork;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class BubbleBox : MonoBehaviour
{
	// Token: 0x06002DC4 RID: 11716 RVA: 0x0016A988 File Offset: 0x00168B88
	private void OnEnable()
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!inGuiding)
		{
			GEvent.Add(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Add(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		}
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x0016A9F0 File Offset: 0x00168BF0
	private void OnDisable()
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!inGuiding)
		{
			GEvent.Remove(EEvents.OnMonthChange, new GEvent.Callback(this.OnMonthChange));
			GEvent.Remove(EEvents.OnActionPointChange, new GEvent.Callback(this.OnDaysInMonthChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			this.Clear();
		}
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x0016AA5E File Offset: 0x00168C5E
	private void OnMonthChange(ArgumentBox box)
	{
		this.isCurrentMonthShow = false;
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x0016AA68 File Offset: 0x00168C68
	private void OnDaysInMonthChange(ArgumentBox box)
	{
		bool flag = SingletonObject.getInstance<BasicGameData>().ActionPointConsumed < GlobalConfig.Instance.TaiwuBubbleBoxDisplayRequirement;
		if (!flag)
		{
			bool flag2 = !this.isCurrentMonthShow;
			if (flag2)
			{
				this.DisPlay();
			}
		}
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x0016AAA8 File Offset: 0x00168CA8
	private void OnTopUiChanged(ArgumentBox box)
	{
		bool flag = this.needDisPlayOnTaskPanelClose && UIManager.Instance.IsFocusElement(UIElement.StateMainWorld);
		if (flag)
		{
			this.needDisPlayOnTaskPanelClose = false;
			this.DisPlay();
		}
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x0016AAE4 File Offset: 0x00168CE4
	public void SetTextAndNotShow(string bubbleText, float showTime)
	{
		this.bubble.SetText(bubbleText, false);
		this._time = showTime;
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x0016AAFC File Offset: 0x00168CFC
	private void DisPlay()
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!inGuiding)
		{
			bool flag = this._timing != null;
			if (flag)
			{
				base.StopCoroutine(this._timing);
				this._timing = null;
			}
			this._timing = base.StartCoroutine(this.ShowBubbles());
			this.isCurrentMonthShow = true;
		}
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x0016AB58 File Offset: 0x00168D58
	public void DisPlayOnTaskPanelClose()
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (!inGuiding)
		{
			this.needDisPlayOnTaskPanelClose = true;
		}
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x0016AB7D File Offset: 0x00168D7D
	private IEnumerator ShowBubbles()
	{
		GEvent.OnEvent(UiEvents.TaskBubbleStart, null);
		this.bubble.Show();
		yield return new WaitForSeconds(this._time);
		this.bubble.Hide();
		this._timing = null;
		GEvent.OnEvent(UiEvents.TaskBubbleEnded, null);
		yield break;
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x0016AB8C File Offset: 0x00168D8C
	private IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
		this.bubble.Hide();
		yield break;
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x0016ABA2 File Offset: 0x00168DA2
	public void Clear()
	{
		this.bubble.SetText("", false);
		this.bubble.Hide();
	}

	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x06002DCF RID: 11727 RVA: 0x0016ABC4 File Offset: 0x00168DC4
	private Bubble bubble
	{
		get
		{
			return (this._bubbleCache != null) ? this._bubbleCache : (this._bubbleCache = base.transform.Find("LeftCloudBubble").GetComponent<Bubble>());
		}
	}

	// Token: 0x0400211B RID: 8475
	private float _time = 0f;

	// Token: 0x0400211C RID: 8476
	private Coroutine _timing;

	// Token: 0x0400211D RID: 8477
	private bool isCurrentMonthShow = false;

	// Token: 0x0400211E RID: 8478
	private bool needDisPlayOnTaskPanelClose = false;

	// Token: 0x0400211F RID: 8479
	private Bubble _bubbleCache;
}
