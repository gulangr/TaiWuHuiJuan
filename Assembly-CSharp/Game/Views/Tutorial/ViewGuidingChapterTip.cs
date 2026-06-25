using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

namespace Game.Views.Tutorial
{
	// Token: 0x02000749 RID: 1865
	public class ViewGuidingChapterTip : UIBase
	{
		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x06005A61 RID: 23137 RVA: 0x0029F05B File Offset: 0x0029D25B
		private BasicGameData BasicGameData
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>();
			}
		}

		// Token: 0x06005A62 RID: 23138 RVA: 0x0029F062 File Offset: 0x0029D262
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<List<short>>("NewTriggeredTemplateIdList", out this._newTriggeredTemplateIdList);
			this.UpdateData(this._newTriggeredTemplateIdList);
			this.RefreshHideState();
			this.hotKeyText.text = MainInterfaceFunctionCommandKit.Tutorial.ToString();
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x0029F0A1 File Offset: 0x0029D2A1
		private void OnEnable()
		{
			GEvent.Add(EEvents.GuidingChapterDataChange, new GEvent.Callback(this.GuidingChapterDataChange));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x0029F0D5 File Offset: 0x0029D2D5
		private void OnDisable()
		{
			GEvent.Remove(EEvents.GuidingChapterDataChange, new GEvent.Callback(this.GuidingChapterDataChange));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x06005A65 RID: 23141 RVA: 0x0029F10C File Offset: 0x0029D30C
		private void Awake()
		{
			this.detail.gameObject.SetActive(false);
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.SetDetailVisible(true);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				bool flag = this.countdown < 5f;
				if (flag)
				{
					this.countdown = 5f;
					this.countdownMax = this.countdown;
				}
				this.SetDetailVisible(false);
			});
			this.pageSwitch.OnValueChanged = new Action<int>(this.OnPageValueChanged);
			this.button.onClick.ResetListener(delegate()
			{
				this.OpenGuidingChapter(this.curTemplateId);
			});
		}

		// Token: 0x06005A66 RID: 23142 RVA: 0x0029F19A File Offset: 0x0029D39A
		private void SetDetailVisible(bool show)
		{
			this.detail.gameObject.SetActive(show);
			this.RefreshPageTextDisplay();
		}

		// Token: 0x06005A67 RID: 23143 RVA: 0x0029F1B8 File Offset: 0x0029D3B8
		private void OpenGuidingChapter(short templateId = -1)
		{
			bool flag = !this.CanOpenGuidingChapter();
			if (!flag)
			{
				bool flag2 = templateId < 0;
				if (flag2)
				{
					templateId = ((this.curTemplateId >= 0) ? this.curTemplateId : this.BasicGameData.GetDefaultTriggeredGuidingChapterTemplateId());
				}
				bool flag3 = templateId < 0;
				if (!flag3)
				{
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
					argumentBox.Set("TemplateId", templateId);
					UIElement.TutorialGuidingChapter.SetOnInitArgs(argumentBox);
					UIManager.Instance.ShowUI(UIElement.TutorialGuidingChapter, true);
				}
			}
		}

		// Token: 0x06005A68 RID: 23144 RVA: 0x0029F238 File Offset: 0x0029D438
		private bool CanOpenGuidingChapter()
		{
			bool flag = GlobalOperations.CurrGameWorldType != 1;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !SingletonObject.getInstance<GlobalSettings>().Guiding;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this.BasicGameData.HaveAnyTriggeredGuidingChapter();
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = UIManager.Instance.IsElementActive(UIElement.TutorialGuidingChapter);
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = UIManager.Instance.IsElementActive(UIElement.Loading);
							result = !flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005A69 RID: 23145 RVA: 0x0029F2BC File Offset: 0x0029D4BC
		private static bool IsTutorialHotKeyPressed()
		{
			HotKeyCommand tutorial = MainInterfaceFunctionCommandKit.Tutorial;
			bool flag = !tutorial.Check(UIElement.StateMainWorld, false, false, false, true, true);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				UIManager uiManager = UIManager.Instance;
				foreach (UIElement element in ViewGuidingChapterTip.TutorialHotKeyBlockedElements)
				{
					bool flag2 = uiManager.IsFocusElement(element);
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06005A6A RID: 23146 RVA: 0x0029F32C File Offset: 0x0029D52C
		private void TopUiChanged(ArgumentBox argBox)
		{
			this.RefreshHideState();
		}

		// Token: 0x06005A6B RID: 23147 RVA: 0x0029F338 File Offset: 0x0029D538
		private void RefreshHideState()
		{
			bool flag = !UIManager.Instance.IsElementActive(UIElement.TutorialGuidingChapter);
			if (flag)
			{
				this.RemoveReadItemsFromQueue();
			}
			UIManager uiManager = UIManager.Instance;
			GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
			bool hide = !settings.Guiding || uiManager.IsElementActive(UIElement.SystemOption) || uiManager.IsElementActive(UIElement.TutorialGuidingChapter) || uiManager.IsElementActive(UIElement.Loading);
			bool flag2 = hide;
			if (flag2)
			{
				this.SetHideState(true);
			}
			else
			{
				bool flag3 = this.showingTemplateIdList.Count > 0;
				if (flag3)
				{
					this.SetHideState(false);
				}
			}
		}

		// Token: 0x06005A6C RID: 23148 RVA: 0x0029F3D0 File Offset: 0x0029D5D0
		private void SetHideState(bool hide)
		{
			this.canvasGroup.alpha = (float)(hide ? 0 : 1);
			this.interactArea.GetComponent<CImage>().raycastTarget = !hide;
			this.button.GetComponent<CImage>().raycastTarget = !hide;
			this.hideState = hide;
		}

		// Token: 0x06005A6D RID: 23149 RVA: 0x0029F423 File Offset: 0x0029D623
		private void GuidingChapterDataChange(ArgumentBox argBox)
		{
			argBox.Get<List<short>>("NewTriggeredTemplateIdList", out this._newTriggeredTemplateIdList);
			this.UpdateData(this._newTriggeredTemplateIdList);
		}

		// Token: 0x06005A6E RID: 23150 RVA: 0x0029F448 File Offset: 0x0029D648
		private void UpdateData(List<short> newTriggeredTemplateIdList)
		{
			this.showingTemplateIdList.AddRange(newTriggeredTemplateIdList);
			bool flag = this.showingTemplateIdList.Count == 1;
			if (flag)
			{
				this.curTemplateId = this.showingTemplateIdList[0];
				this.SetInfo(this.curTemplateId);
				this.countdown = 30f;
				this.countdownMax = this.countdown;
			}
			else
			{
				bool flag2 = this.showingTemplateIdList.Count > 1;
				if (flag2)
				{
					bool flag3 = this.countdown >= 5f;
					if (flag3)
					{
						this.countdown = 5f;
					}
					this.countdownMax = 5f;
				}
			}
			this.pageSwitch.Init(1, this.showingTemplateIdList.Count, 1);
			this.RefreshPageTextDisplay();
			this.sword.gameObject.SetActive(true);
		}

		// Token: 0x06005A6F RID: 23151 RVA: 0x0029F520 File Offset: 0x0029D720
		private void SetInfo(short templateId)
		{
			GuidingChapterItem config = GuidingChapter.Instance[templateId];
			this.itemName.SetText(config.Name, true);
			this.SetDesc(templateId);
			this.RefreshPageTextDisplay();
		}

		// Token: 0x06005A70 RID: 23152 RVA: 0x0029F55C File Offset: 0x0029D75C
		private void OnPageValueChanged(int index)
		{
			bool flag = this.curTemplateId < 0;
			if (!flag)
			{
				this.curTemplateId = this.showingTemplateIdList[index - 1];
				this.SetInfo(this.curTemplateId);
			}
		}

		// Token: 0x06005A71 RID: 23153 RVA: 0x0029F59C File Offset: 0x0029D79C
		private void SetDesc(short templateId)
		{
			GuidingChapterItem config = GuidingChapter.Instance[templateId];
			string finalDesc = string.Empty;
			for (int i = 0; i < config.PartDesc.Length; i++)
			{
				string descStr = config.PartDesc[i];
				finalDesc = finalDesc + "·" + descStr + "\n \n";
			}
			this.desc.SetText(finalDesc, true);
		}

		// Token: 0x06005A72 RID: 23154 RVA: 0x0029F600 File Offset: 0x0029D800
		private void RemoveReadItemsFromQueue()
		{
			bool flag = this.showingTemplateIdList.Count == 0;
			if (!flag)
			{
				this.showingTemplateIdList.RemoveAll((short templateId) => this.BasicGameData.GetTriggeredGuidingChapterState(templateId) != 0);
				bool flag2 = this.showingTemplateIdList.Count == 0;
				if (flag2)
				{
					this.SetHideState(true);
					this.itemName.SetText(string.Empty, true);
					this.curTemplateId = -1;
					this.RefreshPageTextDisplay();
				}
				else
				{
					bool flag3 = !this.showingTemplateIdList.Contains(this.curTemplateId);
					if (flag3)
					{
						this.curTemplateId = this.showingTemplateIdList[0];
						this.pageSwitch.Init(1, this.showingTemplateIdList.Count, 1);
						this.SetInfo(this.curTemplateId);
					}
					else
					{
						this.pageSwitch.Init(this.showingTemplateIdList.IndexOf(this.curTemplateId) + 1, this.showingTemplateIdList.Count, 1);
						this.RefreshPageTextDisplay();
					}
					this.countdown = ((this.showingTemplateIdList.Count == 1) ? 30f : 5f);
					this.countdownMax = this.countdown;
				}
			}
		}

		// Token: 0x06005A73 RID: 23155 RVA: 0x0029F734 File Offset: 0x0029D934
		private void RefreshPageTextDisplay()
		{
			bool flag = this.pageText == null;
			if (!flag)
			{
				int count = this.showingTemplateIdList.Count;
				bool showPageText = count > 0 && !this.detail.activeSelf;
				this.pageText.gameObject.SetActive(showPageText);
				bool flag2 = showPageText;
				if (flag2)
				{
					TMP_Text tmp_Text = this.pageText;
					Func<int, int, string> styleCheck = this.pageSwitch.StyleCheck;
					tmp_Text.SetText(((styleCheck != null) ? styleCheck(this.pageSwitch.Value, this.pageSwitch.MaxValue) : null) ?? string.Format("{0}/{1}", this.pageSwitch.Value, this.pageSwitch.MaxValue), true);
				}
			}
		}

		// Token: 0x06005A74 RID: 23156 RVA: 0x0029F7FC File Offset: 0x0029D9FC
		private void Update()
		{
			bool flag = ViewGuidingChapterTip.IsTutorialHotKeyPressed() && this.CanOpenGuidingChapter();
			if (flag)
			{
				this.OpenGuidingChapter(this.curTemplateId);
			}
			bool flag2 = this.BasicGameData.AdvancingMonthState != 0;
			if (flag2)
			{
				this.SetHideState(true);
			}
			else
			{
				bool flag3 = this.showingTemplateIdList.Count > 0 && this.countdown <= 0f;
				if (flag3)
				{
					this.showingTemplateIdList.Remove(this.curTemplateId);
					bool flag4 = this.showingTemplateIdList.Count == 0;
					if (flag4)
					{
						this.SetHideState(true);
						this.itemName.SetText(string.Empty, true);
						this.curTemplateId = -1;
						this.RefreshPageTextDisplay();
					}
					else
					{
						this.pageSwitch.Init(1, this.showingTemplateIdList.Count, 1);
						this.curTemplateId = this.showingTemplateIdList[0];
						this.SetInfo(this.curTemplateId);
						this.countdown = ((this.showingTemplateIdList.Count == 1) ? 30f : 5f);
						this.countdownMax = this.countdown;
					}
				}
				else
				{
					bool flag5 = !this.detail.gameObject.activeSelf && !this.hideState && !UIManager.Instance.IsElementActive(UIElement.TutorialGuidingChapter);
					if (flag5)
					{
						this.countdown -= Time.fixedDeltaTime;
						this.countDownProgress.fillAmount = this.countdown / this.countdownMax;
					}
				}
			}
		}

		// Token: 0x04003E47 RID: 15943
		[SerializeField]
		private TextMeshProUGUI itemName;

		// Token: 0x04003E48 RID: 15944
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04003E49 RID: 15945
		[SerializeField]
		private TextMeshProUGUI hotKeyText;

		// Token: 0x04003E4A RID: 15946
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04003E4B RID: 15947
		[SerializeField]
		private GameObject sword;

		// Token: 0x04003E4C RID: 15948
		[SerializeField]
		private GameObject detail;

		// Token: 0x04003E4D RID: 15949
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04003E4E RID: 15950
		[SerializeField]
		private ModIdSwitch pageSwitch;

		// Token: 0x04003E4F RID: 15951
		[SerializeField]
		private CButton button;

		// Token: 0x04003E50 RID: 15952
		[SerializeField]
		private RectTransform interactArea;

		// Token: 0x04003E51 RID: 15953
		[SerializeField]
		private CImage countDownProgress;

		// Token: 0x04003E52 RID: 15954
		[SerializeField]
		private TextMeshProUGUI pageText;

		// Token: 0x04003E53 RID: 15955
		private List<short> _newTriggeredTemplateIdList = new List<short>();

		// Token: 0x04003E54 RID: 15956
		private readonly List<short> showingTemplateIdList = new List<short>();

		// Token: 0x04003E55 RID: 15957
		private short curTemplateId = -1;

		// Token: 0x04003E56 RID: 15958
		private float countdown;

		// Token: 0x04003E57 RID: 15959
		private float countdownMax;

		// Token: 0x04003E58 RID: 15960
		private bool hideState;

		// Token: 0x04003E59 RID: 15961
		private const float SingleExistDuration = 30f;

		// Token: 0x04003E5A RID: 15962
		private const float MultiExistDuration = 5f;

		// Token: 0x04003E5B RID: 15963
		private static readonly UIElement[] TutorialHotKeyBlockedElements = new UIElement[0];
	}
}
