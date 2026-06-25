using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FF3 RID: 4083
	public class LanguageRuleTips : MonoBehaviour, ILanguage, IPointerEnterHandler, IEventSystemHandler
	{
		// Token: 0x0600BA41 RID: 47681 RVA: 0x0054D2B8 File Offset: 0x0054B4B8
		private void OnEnable()
		{
			if (this.label == null)
			{
				this.label = base.GetComponent<TextMeshProUGUI>();
			}
			bool flag = !this._initialized;
			if (flag)
			{
				this._originalOverflowMode = this.label.overflowMode;
				this._initialized = true;
			}
			this.RefreshWhenNotCn();
		}

		// Token: 0x0600BA42 RID: 47682 RVA: 0x0054D308 File Offset: 0x0054B508
		private void RefreshWhenNotCn()
		{
			bool flag = this.label == null;
			if (!flag)
			{
				bool flag2 = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
				if (flag2)
				{
					this.label.overflowMode = this._originalOverflowMode;
					bool flag3 = this.tip;
					if (flag3)
					{
						this.tip.enabled = false;
					}
				}
				else
				{
					this.label.overflowMode = TextOverflowModes.Ellipsis;
					this.RefreshTips();
				}
			}
		}

		// Token: 0x0600BA43 RID: 47683 RVA: 0x0054D37C File Offset: 0x0054B57C
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			if (languageType != LocalStringManager.LanguageType.CN)
			{
				if (languageType == LocalStringManager.LanguageType.EN)
				{
					this.label.overflowMode = TextOverflowModes.Ellipsis;
					this.RefreshTips();
				}
			}
			else
			{
				this.label.overflowMode = this._originalOverflowMode;
				bool flag = this.tip;
				if (flag)
				{
					this.tip.enabled = false;
				}
			}
		}

		// Token: 0x0600BA44 RID: 47684 RVA: 0x0054D3DF File Offset: 0x0054B5DF
		public void Refresh()
		{
			this.RefreshWhenNotCn();
		}

		// Token: 0x0600BA45 RID: 47685 RVA: 0x0054D3E9 File Offset: 0x0054B5E9
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.RefreshWhenNotCn();
		}

		// Token: 0x0600BA46 RID: 47686 RVA: 0x0054D3F4 File Offset: 0x0054B5F4
		private void RefreshTips()
		{
			bool flag = this.tip == null;
			if (flag)
			{
				this.CreateTip();
			}
			this.tip.enabled = true;
			this.tip.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = this.tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			bool flag2 = this.textLanguage != null;
			if (flag2)
			{
				this.tip.RuntimeParam.Set("arg0", LocalStringManager.Get(this.textLanguage.Key));
			}
			else
			{
				this.tip.RuntimeParam.Set("arg0", this.label.text);
			}
		}

		// Token: 0x0600BA47 RID: 47687 RVA: 0x0054D4B0 File Offset: 0x0054B6B0
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

		// Token: 0x0600BA48 RID: 47688 RVA: 0x0054D540 File Offset: 0x0054B740
		public void SetTipActive(bool active)
		{
			bool flag = this.tip;
			if (flag)
			{
				this.tip.gameObject.SetActive(active);
			}
		}

		// Token: 0x04009002 RID: 36866
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04009003 RID: 36867
		[SerializeField]
		private TextLanguage textLanguage;

		// Token: 0x04009004 RID: 36868
		[Header("tips显示组件，如果不设置，会在label上生成一个子节点去接受点击")]
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04009005 RID: 36869
		private TextOverflowModes _originalOverflowMode;

		// Token: 0x04009006 RID: 36870
		private bool _initialized;
	}
}
