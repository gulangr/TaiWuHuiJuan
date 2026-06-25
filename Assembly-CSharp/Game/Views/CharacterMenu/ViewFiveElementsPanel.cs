using System;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BAF RID: 2991
	public class ViewFiveElementsPanel : UIBase
	{
		// Token: 0x0600967F RID: 38527 RVA: 0x00463710 File Offset: 0x00461910
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("NeiliType", out this._neiliType);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.CallRefresh));
		}

		// Token: 0x06009680 RID: 38528 RVA: 0x0046374C File Offset: 0x0046194C
		private void CallRefresh()
		{
			for (int i = 0; i < this.highlights.childCount; i++)
			{
				this.highlights.GetChild(i).GetChild(0).gameObject.SetActive(i == (int)this._neiliType);
			}
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
		}

		// Token: 0x06009681 RID: 38529 RVA: 0x004637A8 File Offset: 0x004619A8
		private void Awake()
		{
			for (int i = 0; i < this.highlights.childCount; i++)
			{
				TooltipInvoker component = this.highlights.GetChild(i).GetComponent<TooltipInvoker>();
				if (component.RuntimeParam == null)
				{
					component.RuntimeParam = new ArgumentBox().Set("neiliType", i);
				}
			}
		}

		// Token: 0x06009682 RID: 38530 RVA: 0x00463804 File Offset: 0x00461A04
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "CloseBtn";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06009683 RID: 38531 RVA: 0x00463830 File Offset: 0x00461A30
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
			string targetName = this._languageObjName.GetValueOrDefault(languageType, this._languageObjName[LocalStringManager.LanguageType.EN]);
			for (int i = 0; i < this.texts.childCount; i++)
			{
				GameObject obj = this.texts.GetChild(i).gameObject;
				obj.SetActive(targetName == obj.name);
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x04007375 RID: 29557
		public Transform highlights;

		// Token: 0x04007376 RID: 29558
		public Transform texts;

		// Token: 0x04007377 RID: 29559
		private sbyte _neiliType;

		// Token: 0x04007378 RID: 29560
		private readonly Dictionary<LocalStringManager.LanguageType, string> _languageObjName = new Dictionary<LocalStringManager.LanguageType, string>
		{
			{
				LocalStringManager.LanguageType.CN,
				"CN"
			},
			{
				LocalStringManager.LanguageType.EN,
				"EN"
			}
		};
	}
}
