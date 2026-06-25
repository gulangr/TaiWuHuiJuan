using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Components.Character
{
	// Token: 0x02000F3A RID: 3898
	public class NameCharacter : MonoBehaviour
	{
		// Token: 0x17001450 RID: 5200
		// (get) Token: 0x0600B32F RID: 45871 RVA: 0x0051925D File Offset: 0x0051745D
		public string FamilyName
		{
			get
			{
				return this.familyName.text;
			}
		}

		// Token: 0x17001451 RID: 5201
		// (get) Token: 0x0600B330 RID: 45872 RVA: 0x0051926A File Offset: 0x0051746A
		public string GivenName
		{
			get
			{
				return this.givenName.text;
			}
		}

		// Token: 0x17001452 RID: 5202
		// (get) Token: 0x0600B331 RID: 45873 RVA: 0x00519277 File Offset: 0x00517477
		public string FixedFamilyName
		{
			get
			{
				return this.FamilyName.IsNullOrEmpty() ? LanguageKey.LK_None.Tr().SetColor("red") : this.FamilyName;
			}
		}

		// Token: 0x17001453 RID: 5203
		// (get) Token: 0x0600B332 RID: 45874 RVA: 0x005192A2 File Offset: 0x005174A2
		public string FixedGivenName
		{
			get
			{
				return this.GivenName.IsNullOrEmpty() ? LanguageKey.LK_None.Tr().SetColor("red") : this.GivenName;
			}
		}

		// Token: 0x17001454 RID: 5204
		// (get) Token: 0x0600B333 RID: 45875 RVA: 0x005192CD File Offset: 0x005174CD
		public string Name
		{
			get
			{
				return (LocalStringManager.CurLanguageKey == "EN") ? (this.GivenName + this.FamilyName) : (this.FamilyName + this.GivenName);
			}
		}

		// Token: 0x17001455 RID: 5205
		// (get) Token: 0x0600B334 RID: 45876 RVA: 0x00519304 File Offset: 0x00517504
		public string FixedName
		{
			get
			{
				return (LocalStringManager.CurLanguageKey == "EN") ? (this.FixedGivenName + this.FixedFamilyName) : (this.FixedFamilyName + this.FixedGivenName);
			}
		}

		// Token: 0x0600B335 RID: 45877 RVA: 0x0051933C File Offset: 0x0051753C
		public void Init(Action<string, string> onRefresh, Action onClickRandomName)
		{
			this._onRefresh = onRefresh;
			this.familyName.onValueChanged.AddListener(new UnityAction<string>(this.OnFamilyNameChange));
			this.familyName.onEndEdit.AddListener(new UnityAction<string>(this.OnFamilyNameEndEdit));
			this.familyName.characterLimit = NameCenter.GetMaxSurnameLength();
			this.givenName.onValueChanged.AddListener(new UnityAction<string>(this.OnGivenNameChange));
			this.givenName.onEndEdit.AddListener(new UnityAction<string>(this.OnGivenNameEndEdit));
			this.givenName.characterLimit = NameCenter.GetMaxNameLength();
			this.randomName.ClearAndAddListener(onClickRandomName);
		}

		// Token: 0x0600B336 RID: 45878 RVA: 0x005193F4 File Offset: 0x005175F4
		public void Refresh(string familyNameStr, string givenNameStr)
		{
			this.familyName.SetTextWithoutNotify(familyNameStr);
			this.givenName.SetTextWithoutNotify(givenNameStr);
			bool flag = familyNameStr.IsNullOrEmpty();
			if (flag)
			{
				familyNameStr = LocalStringManager.Get(LanguageKey.LK_NewGame_Empty).SetColor("brightred");
			}
			bool flag2 = givenNameStr.IsNullOrEmpty();
			if (flag2)
			{
				givenNameStr = LocalStringManager.Get(LanguageKey.LK_NewGame_Empty).SetColor("brightred");
			}
			Action<string, string> onRefresh = this._onRefresh;
			if (onRefresh != null)
			{
				onRefresh(familyNameStr, givenNameStr);
			}
		}

		// Token: 0x0600B337 RID: 45879 RVA: 0x00519470 File Offset: 0x00517670
		public void SetInteractable(bool interactable)
		{
			Selectable selectable = this.familyName;
			this.givenName.interactable = interactable;
			selectable.interactable = interactable;
		}

		// Token: 0x0600B338 RID: 45880 RVA: 0x0051949C File Offset: 0x0051769C
		private void OnFamilyNameChange(string value)
		{
			bool hasSpace = value.Contains(' ');
			bool hasIllegalChar = CommonUtils.FixToShowAbleString(ref value, this.familyName.textComponent.font);
			bool flag = (hasSpace || hasIllegalChar) && LocalStringManager.CurLanguageKey == "CN";
			if (flag)
			{
				string cleanedValue = value.Replace(" ", string.Empty);
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.UI_NewGame_CreateTip_InvalidName);
				CommonUtils.ShowDialog(title, content, delegate()
				{
					this.familyName.SetTextWithoutNotify(cleanedValue);
					this.Refresh(cleanedValue, this.GivenName);
				}, EDialogType.None);
			}
			else
			{
				bool flag2 = hasIllegalChar;
				if (flag2)
				{
					this.familyName.SetTextWithoutNotify(value);
				}
			}
		}

		// Token: 0x0600B339 RID: 45881 RVA: 0x00519554 File Offset: 0x00517754
		private void OnFamilyNameEndEdit(string value)
		{
			int maxLength = NameCenter.GetMaxSurnameLength();
			bool flag = value.Length > maxLength;
			if (flag)
			{
				value = value.Substring(0, maxLength);
				this.familyName.SetTextWithoutNotify(value);
			}
			bool flag2 = this.CheckSensitiveWord(value, this.GivenName);
			if (flag2)
			{
				this.SensitiveWordTipShow();
				value = string.Empty;
				this.familyName.SetTextWithoutNotify(value);
			}
			this.Refresh(value, this.GivenName);
		}

		// Token: 0x0600B33A RID: 45882 RVA: 0x005195CC File Offset: 0x005177CC
		private void OnGivenNameChange(string value)
		{
			bool hasSpace = value.Contains(' ');
			bool hasIllegalChar = CommonUtils.FixToShowAbleString(ref value, this.givenName.textComponent.font);
			bool flag = (hasSpace || hasIllegalChar) && LocalStringManager.CurLanguageKey == "CN";
			if (flag)
			{
				string cleanedValue = value.Replace(" ", string.Empty);
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.UI_NewGame_CreateTip_InvalidName);
				CommonUtils.ShowDialog(title, content, delegate()
				{
					this.givenName.SetTextWithoutNotify(cleanedValue);
					this.Refresh(this.FamilyName, cleanedValue);
				}, EDialogType.None);
			}
			else
			{
				bool flag2 = hasIllegalChar;
				if (flag2)
				{
					this.givenName.SetTextWithoutNotify(value);
				}
			}
		}

		// Token: 0x0600B33B RID: 45883 RVA: 0x00519684 File Offset: 0x00517884
		private void OnGivenNameEndEdit(string value)
		{
			int maxLength = NameCenter.GetMaxNameLength();
			bool flag = value.Length > maxLength;
			if (flag)
			{
				value = value.Substring(0, maxLength);
				this.givenName.SetTextWithoutNotify(value);
			}
			bool flag2 = this.CheckSensitiveWord(this.FamilyName, value);
			if (flag2)
			{
				this.SensitiveWordTipShow();
				value = string.Empty;
				this.givenName.SetTextWithoutNotify(value);
			}
			this.Refresh(this.FamilyName, value);
		}

		// Token: 0x0600B33C RID: 45884 RVA: 0x005196FC File Offset: 0x005178FC
		private bool CheckSensitiveWord(string sur, string given)
		{
			List<SensitiveWordsMatchResult> list = SensitiveWordsSystem.Instance.TryMatch(sur, 10);
			if (list == null || list.Count <= 0)
			{
				List<SensitiveWordsMatchResult> list2 = SensitiveWordsSystem.Instance.TryMatch(given, 10);
				if (list2 == null || list2.Count <= 0)
				{
					List<SensitiveWordsMatchResult> list3 = SensitiveWordsSystem.Instance.TryMatch(sur + given, 10);
					return list3 != null && list3.Count > 0;
				}
			}
			return true;
		}

		// Token: 0x0600B33D RID: 45885 RVA: 0x00519770 File Offset: 0x00517970
		private void SensitiveWordTipShow()
		{
			this.sensitive.alpha = 1f;
			this._sensitiveSequence = DOTween.Sequence();
			this._sensitiveSequence.AppendInterval(SensitiveWordsSystem.SensitiveWordAnimationStayTime);
			this._sensitiveSequence.AppendCallback(delegate
			{
				this.sensitive.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
			});
			this._sensitiveSequence.Play<Sequence>();
		}

		// Token: 0x04008B33 RID: 35635
		[SerializeField]
		private TMP_InputField familyName;

		// Token: 0x04008B34 RID: 35636
		[SerializeField]
		private TMP_InputField givenName;

		// Token: 0x04008B35 RID: 35637
		[SerializeField]
		private CanvasGroup sensitive;

		// Token: 0x04008B36 RID: 35638
		[SerializeField]
		private CButton randomName;

		// Token: 0x04008B37 RID: 35639
		private Sequence _sensitiveSequence;

		// Token: 0x04008B38 RID: 35640
		private Action<string, string> _onRefresh;
	}
}
