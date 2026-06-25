using System;
using System.Collections;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A38 RID: 2616
	public class EventWindowChangeNameInputPanel : MonoBehaviour
	{
		// Token: 0x17000E24 RID: 3620
		// (get) Token: 0x0600811F RID: 33055 RVA: 0x003C1B17 File Offset: 0x003BFD17
		public TMP_InputField InputFieldSurName
		{
			get
			{
				return this.inputFieldSurName;
			}
		}

		// Token: 0x17000E25 RID: 3621
		// (get) Token: 0x06008120 RID: 33056 RVA: 0x003C1B1F File Offset: 0x003BFD1F
		public TMP_InputField InputFieldGivenName
		{
			get
			{
				return this.inputFieldGivenName;
			}
		}

		// Token: 0x17000E26 RID: 3622
		// (get) Token: 0x06008121 RID: 33057 RVA: 0x003C1B28 File Offset: 0x003BFD28
		private EventModel Model
		{
			get
			{
				bool flag = this._model == null;
				if (flag)
				{
					this._model = SingletonObject.getInstance<EventModel>();
				}
				return this._model;
			}
		}

		// Token: 0x17000E27 RID: 3623
		// (get) Token: 0x06008122 RID: 33058 RVA: 0x003C1B58 File Offset: 0x003BFD58
		private TaiwuEventDisplayData Data
		{
			get
			{
				return this.Model.DisplayingEventData;
			}
		}

		// Token: 0x06008123 RID: 33059 RVA: 0x003C1B68 File Offset: 0x003BFD68
		public void Refresh(CButton confirm, CButton cancel, GameObject buttons, Action<bool> onSetWaitSelect, Action refreshConfirmButtonTips)
		{
			this._cancel = cancel;
			this._confirm = confirm;
			this._refreshConfirmButtonTips = refreshConfirmButtonTips;
			CanvasGroup canvasGroup = this.sensitiveWarningTip;
			canvasGroup.alpha = 0f;
			this.inputFieldSurName.text = string.Empty;
			this.inputFieldGivenName.text = string.Empty;
			int[] range = this.Data.ExtraData.InputRequestData.NumberRange;
			this.btnRandomName.Refresh(this.inputFieldSurName, this.inputFieldGivenName, this.Data.ExtraData.InputRequestData.InputDataType);
			sbyte inputDataType = this.Data.ExtraData.InputRequestData.InputDataType;
			sbyte b = inputDataType;
			if (b != 5)
			{
				throw new Exception("EventWindowChangeNameInputPanel 目前仅用于修改姓名");
			}
			((TextMeshProUGUI)this.inputFieldGivenName.placeholder).text = LanguageKey.LK_EventWindow_InputName_PlaceHolder_GivenName.Tr();
			((TextMeshProUGUI)this.inputFieldSurName.placeholder).text = LanguageKey.LK_EventWindow_InputName_PlaceHolder_SurName.Tr();
			this.inputFieldGivenName.contentType = TMP_InputField.ContentType.Name;
			this.inputFieldGivenName.characterLimit = range[3];
			this.inputFieldSurName.contentType = TMP_InputField.ContentType.Name;
			this.inputFieldSurName.characterLimit = range[1];
			this.inputFieldGivenName.onValueChanged.RemoveAllListeners();
			this.inputFieldGivenName.onValueChanged.AddListener(new UnityAction<string>(this.OnInputValueChangedGivenName));
			this.inputFieldGivenName.onEndEdit.RemoveAllListeners();
			this.inputFieldGivenName.onEndEdit.AddListener(delegate(string str)
			{
				this.OnEndEditCheckSensitiveWord(str, this.inputFieldGivenName);
			});
			this.inputFieldSurName.onValueChanged.RemoveAllListeners();
			this.inputFieldSurName.onValueChanged.AddListener(new UnityAction<string>(this.OnInputValueChangedSurname));
			this.inputFieldSurName.onEndEdit.RemoveAllListeners();
			this.inputFieldSurName.onEndEdit.AddListener(delegate(string str)
			{
				this.OnEndEditCheckSensitiveWord(str, this.inputFieldSurName);
			});
			if (onSetWaitSelect != null)
			{
				onSetWaitSelect(true);
			}
			this._cancel.gameObject.SetActive(this.Data.EventOptionInfos.Count == 2);
			buttons.SetActive(true);
			bool canConfirm = !this.inputFieldGivenName.text.IsNullOrEmpty() && !this.inputFieldSurName.text.IsNullOrEmpty();
			this._confirm.interactable = canConfirm;
		}

		// Token: 0x06008124 RID: 33060 RVA: 0x003C1DD4 File Offset: 0x003BFFD4
		private void OnInputValueChangedSurname(string inputStr)
		{
			TMP_InputField inputField = this.InputFieldSurName;
			int[] lengthRange = this.Data.ExtraData.InputRequestData.NumberRange;
			bool flag = CommonUtils.FixToShowAbleString(ref inputStr, inputField.textComponent.font);
			if (flag)
			{
				inputField.SetTextWithoutNotify(inputStr);
			}
			bool flag2 = inputField.text.IsNullOrEmpty();
			if (flag2)
			{
				this.Data.ExtraData.InputRequestData.ConfirmDisableTips = "LK_Event_InputMode_ConfirmButtonTips_NoInput";
			}
			bool canConfirm = !inputStr.IsNullOrEmpty() && inputStr.Length <= lengthRange[1] && inputStr.Length >= lengthRange[0];
			canConfirm = (canConfirm && !NameCenter.HasInvalidCharForName(inputStr));
			canConfirm = (canConfirm && this.CheckGivenNameValid(lengthRange));
			this._confirm.interactable = canConfirm;
			Action refreshConfirmButtonTips = this._refreshConfirmButtonTips;
			if (refreshConfirmButtonTips != null)
			{
				refreshConfirmButtonTips();
			}
		}

		// Token: 0x06008125 RID: 33061 RVA: 0x003C1EB0 File Offset: 0x003C00B0
		private void OnInputValueChangedGivenName(string inputStr)
		{
			TMP_InputField inputField = this.inputFieldGivenName;
			int[] lengthRange = this.Data.ExtraData.InputRequestData.NumberRange;
			bool flag = CommonUtils.FixToShowAbleString(ref inputStr, inputField.textComponent.font);
			if (flag)
			{
				inputField.SetTextWithoutNotify(inputStr);
			}
			bool flag2 = inputField.text.IsNullOrEmpty();
			if (flag2)
			{
				this.Data.ExtraData.InputRequestData.ConfirmDisableTips = "LK_Event_InputMode_ConfirmButtonTips_NoInput";
			}
			bool canConfirm = !inputStr.IsNullOrEmpty() && inputStr.Length <= lengthRange[1] && inputStr.Length >= lengthRange[0];
			canConfirm = (canConfirm && !NameCenter.HasInvalidCharForName(inputStr));
			canConfirm = (canConfirm && this.CheckSurNameValid(lengthRange));
			this._confirm.interactable = canConfirm;
			Action refreshConfirmButtonTips = this._refreshConfirmButtonTips;
			if (refreshConfirmButtonTips != null)
			{
				refreshConfirmButtonTips();
			}
		}

		// Token: 0x06008126 RID: 33062 RVA: 0x003C1F8C File Offset: 0x003C018C
		private bool CheckGivenNameValid(int[] lengthRange)
		{
			string str = this.inputFieldGivenName.text;
			return !str.IsNullOrEmpty() && str.Length <= lengthRange[1] && str.Length >= lengthRange[0];
		}

		// Token: 0x06008127 RID: 33063 RVA: 0x003C1FD0 File Offset: 0x003C01D0
		private bool CheckSurNameValid(int[] lengthRange)
		{
			string str = this.inputFieldSurName.text;
			return !str.IsNullOrEmpty() && str.Length <= lengthRange[3] && str.Length >= lengthRange[2];
		}

		// Token: 0x06008128 RID: 33064 RVA: 0x003C2014 File Offset: 0x003C0214
		private void OnEndEditCheckSensitiveWord(string inputStr, TMP_InputField inputField)
		{
			bool flag = inputStr.IsNullOrEmpty();
			if (!flag)
			{
				bool hasSensitiveWord = false;
				TaiwuEventDisplayData data = this.Data;
				sbyte? b;
				if (data == null)
				{
					b = null;
				}
				else
				{
					TaiwuEventDisplayExtraData extraData = data.ExtraData;
					if (extraData == null)
					{
						b = null;
					}
					else
					{
						EventInputRequestData inputRequestData = extraData.InputRequestData;
						b = ((inputRequestData != null) ? new sbyte?(inputRequestData.InputDataType) : null);
					}
				}
				sbyte? b2 = b;
				int? num = (b2 != null) ? new int?((int)b2.GetValueOrDefault()) : null;
				int num2 = 1;
				bool flag2 = !(num.GetValueOrDefault() == num2 & num != null);
				if (flag2)
				{
					hasSensitiveWord = inputField.SensitiveWordHandle(ref inputStr);
				}
				bool flag3 = hasSensitiveWord;
				if (flag3)
				{
					CanvasGroup canvasGroup = this.sensitiveWarningTip;
					canvasGroup.alpha = 1f;
					bool flag4 = this._sensitiveWordTipCoroutine != null;
					if (flag4)
					{
						base.StopCoroutine(this._sensitiveWordTipCoroutine);
					}
					Tween sensitiveWordTipTween = this._sensitiveWordTipTween;
					if (sensitiveWordTipTween != null)
					{
						sensitiveWordTipTween.Kill(false);
					}
					this._sensitiveWordTipCoroutine = base.StartCoroutine(this.CoDelyCall(SensitiveWordsSystem.SensitiveWordAnimationStayTime, delegate
					{
						bool activeInHierarchy = canvasGroup.gameObject.activeInHierarchy;
						if (activeInHierarchy)
						{
							this._sensitiveWordTipTween = canvasGroup.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
						}
					}));
					inputField.SetTextWithoutNotify(string.Empty);
					this.Data.ExtraData.InputRequestData.ConfirmDisableTips = "LK_Event_InputMode_ConfirmButtonTips_NoInput";
					this._confirm.interactable = false;
					Action refreshConfirmButtonTips = this._refreshConfirmButtonTips;
					if (refreshConfirmButtonTips != null)
					{
						refreshConfirmButtonTips();
					}
				}
			}
		}

		// Token: 0x06008129 RID: 33065 RVA: 0x003C2199 File Offset: 0x003C0399
		private IEnumerator CoDelyCall(float delay, Action actionAfterDelay)
		{
			yield return new WaitForSeconds(delay);
			if (actionAfterDelay != null)
			{
				actionAfterDelay();
			}
			yield break;
		}

		// Token: 0x04006293 RID: 25235
		[SerializeField]
		private ButtonRandomName btnRandomName;

		// Token: 0x04006294 RID: 25236
		[SerializeField]
		private CanvasGroup sensitiveWarningTip;

		// Token: 0x04006295 RID: 25237
		[SerializeField]
		private TMP_InputField inputFieldSurName;

		// Token: 0x04006296 RID: 25238
		[SerializeField]
		private TMP_InputField inputFieldGivenName;

		// Token: 0x04006297 RID: 25239
		private Coroutine _sensitiveWordTipCoroutine;

		// Token: 0x04006298 RID: 25240
		private Tween _sensitiveWordTipTween;

		// Token: 0x04006299 RID: 25241
		public Action _refreshConfirmButtonTips;

		// Token: 0x0400629A RID: 25242
		private CButton _cancel;

		// Token: 0x0400629B RID: 25243
		private CButton _confirm;

		// Token: 0x0400629C RID: 25244
		private EventModel _model;
	}
}
