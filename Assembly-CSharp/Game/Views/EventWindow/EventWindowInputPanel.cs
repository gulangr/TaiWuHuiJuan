using System;
using System.Collections;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A3D RID: 2621
	public class EventWindowInputPanel : MonoBehaviour
	{
		// Token: 0x17000E2A RID: 3626
		// (get) Token: 0x0600816C RID: 33132 RVA: 0x003C3877 File Offset: 0x003C1A77
		public TMP_InputField InputField
		{
			get
			{
				return this.inputField;
			}
		}

		// Token: 0x17000E2B RID: 3627
		// (get) Token: 0x0600816D RID: 33133 RVA: 0x003C3880 File Offset: 0x003C1A80
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

		// Token: 0x17000E2C RID: 3628
		// (get) Token: 0x0600816E RID: 33134 RVA: 0x003C38B0 File Offset: 0x003C1AB0
		private TaiwuEventDisplayData Data
		{
			get
			{
				return this.Model.DisplayingEventData;
			}
		}

		// Token: 0x0600816F RID: 33135 RVA: 0x003C38C0 File Offset: 0x003C1AC0
		public void Refresh(CButton confirm, CButton cancel, GameObject buttons, Action<bool> onSetWaitSelect, Action refreshConfirmButtonTips)
		{
			this._cancel = cancel;
			this._confirm = confirm;
			this._refreshConfirmButtonTips = refreshConfirmButtonTips;
			CanvasGroup canvasGroup = this.sensitiveWarningTip;
			canvasGroup.alpha = 0f;
			this.inputField.text = string.Empty;
			TextMeshProUGUI placeHolder = (TextMeshProUGUI)this.inputField.placeholder;
			int[] range = this.Data.ExtraData.InputRequestData.NumberRange;
			this.btnRandomName.Refresh(this.inputField, this.Data.ExtraData.InputRequestData.InputDataType);
			switch (this.Data.ExtraData.InputRequestData.InputDataType)
			{
			case 0:
				placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_StringHolderTips);
				this.inputField.contentType = TMP_InputField.ContentType.Standard;
				break;
			case 1:
			{
				this.inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
				bool flag = range == null;
				if (flag)
				{
					placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_StringHolderTips);
				}
				else
				{
					placeHolder.text = LocalStringManager.GetFormat(LanguageKey.LK_EventInput_IntegerTips, range[0], range[1]);
				}
				break;
			}
			case 2:
				placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_NameHolderTips);
				this.inputField.contentType = TMP_InputField.ContentType.Name;
				this.inputField.characterLimit = range[1];
				break;
			case 3:
				placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_GivenNameHolderTips);
				this.inputField.contentType = TMP_InputField.ContentType.Name;
				this.inputField.characterLimit = range[1];
				break;
			case 4:
				placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_SurNameHolderTips);
				this.inputField.contentType = TMP_InputField.ContentType.Name;
				this.inputField.characterLimit = range[1];
				break;
			default:
				this.inputField.contentType = TMP_InputField.ContentType.Standard;
				break;
			}
			this.inputField.onValueChanged.RemoveAllListeners();
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputValueChanged));
			this.inputField.onEndEdit.RemoveAllListeners();
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEditCheckSensitiveWord));
			this.inputTips.text = string.Empty;
			if (onSetWaitSelect != null)
			{
				onSetWaitSelect(true);
			}
			this._cancel.gameObject.SetActive(this.Data.EventOptionInfos.Count == 2);
			buttons.SetActive(true);
			bool canConfirm = !this.inputField.text.IsNullOrEmpty();
			this._confirm.interactable = canConfirm;
			this.RefreshPartnerGiveName();
		}

		// Token: 0x06008170 RID: 33136 RVA: 0x003C3B70 File Offset: 0x003C1D70
		private void Update()
		{
			bool flag = this._confirm == null || !this._confirm.gameObject.activeInHierarchy;
			if (!flag)
			{
				bool spaceDown = CommonCommandKit.Space.Check(UIElement.EventWindow, false, false, false, true, false);
				bool flag2 = spaceDown && !this.IsInputFieldActive();
				if (flag2)
				{
					Button.ButtonClickedEvent onClick = this._confirm.onClick;
					if (onClick != null)
					{
						onClick.Invoke();
					}
				}
			}
		}

		// Token: 0x06008171 RID: 33137 RVA: 0x003C3BEC File Offset: 0x003C1DEC
		private bool IsInputFieldActive()
		{
			return this.inputField != null && this.inputField.isFocused;
		}

		// Token: 0x06008172 RID: 33138 RVA: 0x003C3C1C File Offset: 0x003C1E1C
		private void OnInputValueChanged(string inputStr)
		{
			bool canConfirm = false;
			bool flag = this.Data.ExtraData.InputRequestData.InputDataType == 1;
			if (flag)
			{
				int intValue;
				bool flag2 = int.TryParse(inputStr, out intValue);
				if (flag2)
				{
					bool flag3 = this.Data.ExtraData.InputRequestData.NumberRange != null;
					if (flag3)
					{
						int[] range = this.Data.ExtraData.InputRequestData.NumberRange;
						bool flag4 = intValue >= range[0] && intValue <= range[1];
						if (flag4)
						{
							canConfirm = true;
							this.inputTips.text = string.Empty;
						}
						else
						{
							this.inputTips.text = LocalStringManager.GetFormat(LanguageKey.LK_EventInput_IntegerTips, range[0], range[1]);
						}
					}
					else
					{
						canConfirm = true;
						this.inputTips.text = string.Empty;
					}
				}
			}
			else
			{
				bool flag5 = this.Data.ExtraData.InputRequestData.InputDataType == 2 || this.Data.ExtraData.InputRequestData.InputDataType == 3;
				if (flag5)
				{
					int[] lengthRange = this.Data.ExtraData.InputRequestData.NumberRange;
					bool flag6 = CommonUtils.FixToShowAbleString(ref inputStr, this.inputField.textComponent.font);
					if (flag6)
					{
						this.inputField.SetTextWithoutNotify(inputStr);
					}
					bool flag7 = this.inputField.text.IsNullOrEmpty();
					if (flag7)
					{
						this.Data.ExtraData.InputRequestData.ConfirmDisableTips = "LK_Event_InputMode_ConfirmButtonTips_NoInput";
					}
					canConfirm = (!inputStr.IsNullOrEmpty() && inputStr.Length <= lengthRange[1] && inputStr.Length >= lengthRange[0]);
					canConfirm = (canConfirm && !NameCenter.HasInvalidCharForName(inputStr));
				}
				else
				{
					canConfirm = (inputStr.Length > 0);
				}
			}
			this._confirm.interactable = canConfirm;
			Action refreshConfirmButtonTips = this._refreshConfirmButtonTips;
			if (refreshConfirmButtonTips != null)
			{
				refreshConfirmButtonTips();
			}
		}

		// Token: 0x06008173 RID: 33139 RVA: 0x003C3E20 File Offset: 0x003C2020
		private void OnEndEditCheckSensitiveWord(string inputStr)
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
					hasSensitiveWord = this.inputField.SensitiveWordHandle(ref inputStr);
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
					this.inputField.SetTextWithoutNotify(string.Empty);
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

		// Token: 0x06008174 RID: 33140 RVA: 0x003C3FAF File Offset: 0x003C21AF
		private IEnumerator CoDelyCall(float delay, Action actionAfterDelay)
		{
			yield return new WaitForSeconds(delay);
			if (actionAfterDelay != null)
			{
				actionAfterDelay();
			}
			yield break;
		}

		// Token: 0x06008175 RID: 33141 RVA: 0x003C3FCC File Offset: 0x003C21CC
		private void RefreshPartnerGiveName()
		{
			this.partnerGiveNameBtn.gameObject.SetActive(this.Data.ExtraData.InputRequestData.ShowPartnerBtn);
			bool showPartnerBtn = this.Data.ExtraData.InputRequestData.ShowPartnerBtn;
			if (showPartnerBtn)
			{
				this.partnerGiveNameBtn.interactable = this.Data.ExtraData.InputRequestData.CanUsePartnerBtn;
				this.partnerGiveNameMouseTip.enabled = !this.Data.ExtraData.InputRequestData.CanUsePartnerBtn;
				this.partnerGiveNameBtn.onClick.ResetListener(delegate()
				{
					ValueTuple<string, string> name = this.Data.ExtraData.InputRequestData.ChildFullName.GetName(this.Data.ExtraData.InputRequestData.ChildGender, SingletonObject.getInstance<BasicGameData>().CustomTexts);
					string surName = name.Item1;
					string givenName = name.Item2;
					this.inputField.text = givenName;
				});
			}
		}

		// Token: 0x040062D9 RID: 25305
		[SerializeField]
		private ButtonRandomName btnRandomName;

		// Token: 0x040062DA RID: 25306
		[SerializeField]
		private CanvasGroup sensitiveWarningTip;

		// Token: 0x040062DB RID: 25307
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x040062DC RID: 25308
		[SerializeField]
		private TextMeshProUGUI inputTips;

		// Token: 0x040062DD RID: 25309
		[SerializeField]
		private CButton partnerGiveNameBtn;

		// Token: 0x040062DE RID: 25310
		[SerializeField]
		private TooltipInvoker partnerGiveNameMouseTip;

		// Token: 0x040062DF RID: 25311
		private Coroutine _sensitiveWordTipCoroutine;

		// Token: 0x040062E0 RID: 25312
		private Tween _sensitiveWordTipTween;

		// Token: 0x040062E1 RID: 25313
		public Action _refreshConfirmButtonTips;

		// Token: 0x040062E2 RID: 25314
		private CButton _cancel;

		// Token: 0x040062E3 RID: 25315
		private CButton _confirm;

		// Token: 0x040062E4 RID: 25316
		private EventModel _model;
	}
}
