using System;
using FrameWork.UI.LanguageRule;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE6 RID: 3814
	public class SingleTextCell : MonoBehaviour, ICellContent<string>, ICellContent, ICellContent<SingleTextWithTipData>
	{
		// Token: 0x0600AF42 RID: 44866 RVA: 0x004FD8E4 File Offset: 0x004FBAE4
		public void SetData(string data)
		{
			this.label.text = (data ?? string.Empty);
			TMPTextSpriteHelper spriteHelper;
			bool flag = this.label.TryGetComponent<TMPTextSpriteHelper>(out spriteHelper);
			if (flag)
			{
				spriteHelper.Parse();
			}
			LanguageRuleTips ruleTips;
			bool flag2 = this.label.TryGetComponent<LanguageRuleTips>(out ruleTips);
			if (flag2)
			{
				ruleTips.Refresh();
			}
			bool flag3 = this.tip;
			if (flag3)
			{
				this.tip.enabled = false;
			}
		}

		// Token: 0x0600AF43 RID: 44867 RVA: 0x004FD958 File Offset: 0x004FBB58
		public void SetData(SingleTextWithTipData data)
		{
			this.SetData(data.content);
			bool flag = !this.tip;
			if (flag)
			{
				this.CreateTip();
			}
			this.tip.enabled = (data.tipAction != null);
			Action<TooltipInvoker> tipAction = data.tipAction;
			if (tipAction != null)
			{
				tipAction(this.tip);
			}
		}

		// Token: 0x0600AF44 RID: 44868 RVA: 0x004FD9BC File Offset: 0x004FBBBC
		private void CreateTip()
		{
			GameObject go = new GameObject("tip", new Type[]
			{
				typeof(RectTransform)
			});
			go.transform.SetParent(this.label.transform, false);
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
			go.AddComponent<CEmptyGraphic>();
			this.tip = go.AddComponent<TooltipInvoker>();
		}

		// Token: 0x040087D8 RID: 34776
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040087D9 RID: 34777
		private TooltipInvoker tip;
	}
}
