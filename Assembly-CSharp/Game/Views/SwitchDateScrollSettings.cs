using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000712 RID: 1810
	[Serializable]
	public class SwitchDateScrollSettings
	{
		// Token: 0x04003ABA RID: 15034
		[Header("入场")]
		[Tooltip("背景渐显时长，与 effFadeIn 同步")]
		public float bgFadeInDuration = 1f;

		// Token: 0x04003ABB RID: 15035
		[Tooltip("「第 / 年」侧文字渐显时长")]
		public float sideTextFadeDuration = 0.3f;

		// Token: 0x04003ABC RID: 15036
		[Tooltip("leftText 开始渐显后，rightText 延迟启动的时间")]
		public float rightTextStartDelay = 0.2f;

		// Token: 0x04003ABD RID: 15037
		[Tooltip("rightText 渐显结束后，再等待多久开始滚动数字")]
		public float delayBeforeScroll = 0.5f;

		// Token: 0x04003ABE RID: 15038
		[Header("数字列")]
		[Tooltip("每列开始滚动时 CanvasGroup 渐显时长")]
		public float digitFadeInDuration = 0.3f;

		// Token: 0x04003ABF RID: 15039
		[Tooltip("从左到右，每列开始滚动的间隔")]
		public float columnStartInterval = 0.2f;

		// Token: 0x04003AC0 RID: 15040
		[Tooltip("单列滚动总时长（含过冲回正）")]
		public float columnScrollDuration = 3f;

		// Token: 0x04003AC1 RID: 15041
		[Header("滚动表现")]
		public Ease scrollEase = Ease.OutCubic;

		// Token: 0x04003AC2 RID: 15042
		[Min(1f)]
		public int minScrollCycles = 3;

		// Token: 0x04003AC3 RID: 15043
		[Tooltip("超过目标位置的距离（行高倍数），再回正")]
		[Min(0f)]
		public float scrollOvershootLineRatio = 0.25f;

		// Token: 0x04003AC4 RID: 15044
		[Tooltip("滚动总时长中用于回正的比例")]
		[Range(0.05f, 0.4f)]
		public float scrollSettleDurationRatio = 0.15f;

		// Token: 0x04003AC5 RID: 15045
		[Header("退场")]
		[Tooltip("滚动结束后，textGroup 开始渐隐的延迟")]
		public float textGroupFadeOutDelay = 0.8f;

		// Token: 0x04003AC6 RID: 15046
		public float textGroupFadeOutDuration = 0.3f;

		// Token: 0x04003AC7 RID: 15047
		[Tooltip("滚动结束后，播放 effFadeOut 的延迟")]
		public float effFadeOutDelay = 1f;

		// Token: 0x04003AC8 RID: 15048
		[Tooltip("effFadeOut 播放后，bg 开始渐隐前的等待")]
		public float delayBeforeBgFadeOut = 0.3f;

		// Token: 0x04003AC9 RID: 15049
		public float bgFadeOutDuration = 0.5f;

		// Token: 0x04003ACA RID: 15050
		[Header("布局与样式")]
		public float digitHeight = 140f;

		// Token: 0x04003ACB RID: 15051
		public float fontSize = 140f;

		// Token: 0x04003ACC RID: 15052
		public Color digitColor = new Color(0.85f, 0.15f, 0.12f, 1f);
	}
}
