using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000FA2 RID: 4002
	public static class ToggleGroupAnimUtility
	{
		// Token: 0x0600B7C6 RID: 47046 RVA: 0x0053BEF4 File Offset: 0x0053A0F4
		public static Sequence SecondToggleSubChangeAnim(SecondToggleSubAnimInfo outInfo, SecondToggleSubAnimInfo inInfo, Action onEnd = null)
		{
			Sequence seq = DOTween.Sequence();
			outInfo.Init();
			bool flag = !Mathf.Approximately(outInfo.StartAlpha, outInfo.EndAlpha) && outInfo.FadeDuration > 0f;
			if (flag)
			{
				seq.Insert(0f, outInfo.Cg.DOFade(outInfo.EndAlpha, outInfo.FadeDuration));
			}
			bool flag2 = !Mathf.Approximately(outInfo.StartScale, outInfo.EndScale) && outInfo.ScaleDuration > 0f;
			if (flag2)
			{
				seq.Insert(0f, outInfo.RectTs.DOScale(outInfo.EndScale, outInfo.ScaleDuration));
			}
			bool flag3 = outInfo.StartAnchorPos != outInfo.EndAnchorPos && outInfo.MoveDuration > 0f;
			if (flag3)
			{
				seq.Insert(0f, outInfo.RectTs.DOAnchorPos(outInfo.EndAnchorPos, outInfo.MoveDuration, false));
			}
			inInfo.Init();
			bool flag4 = !Mathf.Approximately(inInfo.StartAlpha, inInfo.EndAlpha) && inInfo.FadeDuration > 0f;
			if (flag4)
			{
				seq.Insert(0f, inInfo.Cg.DOFade(inInfo.EndAlpha, inInfo.FadeDuration));
			}
			bool flag5 = !Mathf.Approximately(inInfo.StartScale, inInfo.EndScale) && inInfo.ScaleDuration > 0f;
			if (flag5)
			{
				seq.Insert(0f, inInfo.RectTs.DOScale(inInfo.EndScale, inInfo.ScaleDuration));
			}
			bool flag6 = inInfo.StartAnchorPos != inInfo.EndAnchorPos && inInfo.MoveDuration > 0f;
			if (flag6)
			{
				seq.Insert(0f, inInfo.RectTs.DOAnchorPos(inInfo.EndAnchorPos, inInfo.MoveDuration, false));
			}
			seq.OnComplete(delegate
			{
				outInfo.RectTs.gameObject.SetActive(false);
				inInfo.RectTs.gameObject.SetActive(true);
				Action onEnd2 = onEnd;
				if (onEnd2 != null)
				{
					onEnd2();
				}
			});
			return seq;
		}

		// Token: 0x0600B7C7 RID: 47047 RVA: 0x0053C1DC File Offset: 0x0053A3DC
		public static Sequence SecondToggleContentRefreshAnim(SecondToggleSubAnimInfo inInfo, Action onEnd = null)
		{
			Sequence seq = DOTween.Sequence();
			inInfo.Init();
			bool flag = !Mathf.Approximately(inInfo.StartAlpha, inInfo.EndAlpha) && inInfo.FadeDuration > 0f;
			if (flag)
			{
				seq.Insert(0f, inInfo.Cg.DOFade(inInfo.EndAlpha, inInfo.FadeDuration));
			}
			bool flag2 = !Mathf.Approximately(inInfo.StartScale, inInfo.EndScale) && inInfo.ScaleDuration > 0f;
			if (flag2)
			{
				seq.Insert(0f, inInfo.RectTs.DOScale(inInfo.EndScale, inInfo.ScaleDuration));
			}
			bool flag3 = inInfo.StartAnchorPos != inInfo.EndAnchorPos && inInfo.MoveDuration > 0f;
			if (flag3)
			{
				seq.Insert(0f, inInfo.RectTs.DOAnchorPos(inInfo.EndAnchorPos, inInfo.MoveDuration, false));
			}
			seq.OnComplete(delegate
			{
				inInfo.RectTs.gameObject.SetActive(true);
				Action onEnd2 = onEnd;
				if (onEnd2 != null)
				{
					onEnd2();
				}
			});
			return seq;
		}

		// Token: 0x0600B7C8 RID: 47048 RVA: 0x0053C36C File Offset: 0x0053A56C
		public static Sequence SecondToggleContentRefreshAnim(SecondToggleSubAnimInfo outInfo, SecondToggleSubAnimInfo inInfo, Action onEnd = null)
		{
			Sequence seq = DOTween.Sequence();
			outInfo.Init();
			bool flag = !Mathf.Approximately(outInfo.StartAlpha, outInfo.EndAlpha) && outInfo.FadeDuration > 0f;
			if (flag)
			{
				seq.Insert(0f, outInfo.Cg.DOFade(outInfo.EndAlpha, outInfo.FadeDuration));
			}
			bool flag2 = !Mathf.Approximately(outInfo.StartScale, outInfo.EndScale) && outInfo.ScaleDuration > 0f;
			if (flag2)
			{
				seq.Insert(0f, outInfo.RectTs.DOScale(outInfo.EndScale, outInfo.ScaleDuration));
			}
			bool flag3 = outInfo.StartAnchorPos != outInfo.EndAnchorPos && outInfo.MoveDuration > 0f;
			if (flag3)
			{
				seq.Insert(0f, outInfo.RectTs.DOAnchorPos(outInfo.EndAnchorPos, outInfo.MoveDuration, false));
			}
			inInfo.Init();
			bool flag4 = !Mathf.Approximately(inInfo.StartAlpha, inInfo.EndAlpha) && inInfo.FadeDuration > 0f;
			if (flag4)
			{
				seq.Insert(0f, inInfo.Cg.DOFade(inInfo.EndAlpha, inInfo.FadeDuration));
			}
			bool flag5 = !Mathf.Approximately(inInfo.StartScale, inInfo.EndScale) && inInfo.ScaleDuration > 0f;
			if (flag5)
			{
				seq.Insert(0f, inInfo.RectTs.DOScale(inInfo.EndScale, inInfo.ScaleDuration));
			}
			bool flag6 = inInfo.StartAnchorPos != inInfo.EndAnchorPos && inInfo.MoveDuration > 0f;
			if (flag6)
			{
				seq.Insert(0f, inInfo.RectTs.DOAnchorPos(inInfo.EndAnchorPos, inInfo.MoveDuration, false));
			}
			seq.OnUpdate(delegate
			{
				for (int i = 0; i < outInfo.RectTs.childCount; i++)
				{
					GameObject go = outInfo.RectTs.GetChild(i).gameObject;
					go.SetActive(true);
				}
			});
			seq.OnComplete(delegate
			{
				inInfo.RectTs.gameObject.SetActive(true);
				Action onEnd2 = onEnd;
				if (onEnd2 != null)
				{
					onEnd2();
				}
			});
			return seq;
		}
	}
}
