using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.Achievement
{
	// Token: 0x02000C83 RID: 3203
	public class ViewAchievementPopUp : UIBase
	{
		// Token: 0x17001111 RID: 4369
		// (get) Token: 0x0600A388 RID: 41864 RVA: 0x004C8983 File Offset: 0x004C6B83
		private HashSet<short> ToPopupAchievements
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().ToPopupAchievements;
			}
		}

		// Token: 0x17001112 RID: 4370
		// (get) Token: 0x0600A389 RID: 41865 RVA: 0x004C898F File Offset: 0x004C6B8F
		private CanvasGroup CanvasGroup
		{
			get
			{
				return base.GetComponent<CanvasGroup>();
			}
		}

		// Token: 0x17001113 RID: 4371
		// (get) Token: 0x0600A38A RID: 41866 RVA: 0x004C8997 File Offset: 0x004C6B97
		// (set) Token: 0x0600A38B RID: 41867 RVA: 0x004C89A0 File Offset: 0x004C6BA0
		private ViewAchievementPopUp.EAchievementPopUpState CurrState
		{
			get
			{
				return this._currState;
			}
			set
			{
				ViewAchievementPopUp.EAchievementPopUpState currState = this._currState;
				this._currState = value;
				bool isActiveAndEnabled = base.isActiveAndEnabled;
				if (isActiveAndEnabled)
				{
					switch (value)
					{
					case ViewAchievementPopUp.EAchievementPopUpState.Appearing:
						this.Refresh(this._currTemplateId);
						this.ResetTween(0f);
						this._currTween = this.CanvasGroup.DOFade(1f, 1f).SetUpdate(UpdateType.Normal, true).OnComplete(new TweenCallback(this.ChangeState));
						this._currDuration = 0f;
						break;
					case ViewAchievementPopUp.EAchievementPopUpState.Displaying:
						this.ResetTween(1f);
						this._currTween = DOVirtual.Float(this._currDuration, 4f, 4f - this._currDuration, delegate(float currDuration)
						{
							this._currDuration = currDuration;
						}).SetUpdate(UpdateType.Normal, true).OnComplete(new TweenCallback(this.ChangeState));
						break;
					case ViewAchievementPopUp.EAchievementPopUpState.Disappearing:
						this.ResetTween(1f);
						this._currTween = this.CanvasGroup.DOFade(0f, 1f).SetUpdate(UpdateType.Normal, true).OnComplete(new TweenCallback(this.ChangeState));
						break;
					case ViewAchievementPopUp.EAchievementPopUpState.Inactive:
					{
						bool flag = currState == ViewAchievementPopUp.EAchievementPopUpState.Inactive;
						if (flag)
						{
							this._shouldHide = true;
							UIElement.AchievementPopUp.Hide(false);
						}
						else
						{
							this.ChangeState();
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x0600A38C RID: 41868 RVA: 0x004C8B09 File Offset: 0x004C6D09
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.ResetListener(new Action(this.OnPointerEnter));
			this.pointerTrigger.ExitEvent.ResetListener(new Action(this.OnPointerExit));
		}

		// Token: 0x0600A38D RID: 41869 RVA: 0x004C8B46 File Offset: 0x004C6D46
		public override void OnInit(ArgumentBox argsBox)
		{
			this._shouldHide = false;
		}

		// Token: 0x0600A38E RID: 41870 RVA: 0x004C8B50 File Offset: 0x004C6D50
		private void OnEnable()
		{
			this.ChangeState();
			AudioManager.Instance.PlaySound("ui_achievement", false, false);
		}

		// Token: 0x0600A38F RID: 41871 RVA: 0x004C8B6C File Offset: 0x004C6D6C
		private void OnDisable()
		{
			this.ResetTween(0f);
		}

		// Token: 0x0600A390 RID: 41872 RVA: 0x004C8B7C File Offset: 0x004C6D7C
		public override void QuickHide()
		{
			bool flag = !this._shouldHide;
			if (!flag)
			{
				base.QuickHide();
			}
		}

		// Token: 0x0600A391 RID: 41873 RVA: 0x004C8BA0 File Offset: 0x004C6DA0
		private void Update()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(1);
			if (mouseButtonDown)
			{
				bool isHovering = this._isHovering;
				if (isHovering)
				{
					this.OnPointerRightClick();
				}
			}
		}

		// Token: 0x0600A392 RID: 41874 RVA: 0x004C8BCB File Offset: 0x004C6DCB
		private void OnPointerEnter()
		{
			this._isHovering = true;
			this.ChangeState();
		}

		// Token: 0x0600A393 RID: 41875 RVA: 0x004C8BDC File Offset: 0x004C6DDC
		private void OnPointerExit()
		{
			this._isHovering = false;
		}

		// Token: 0x0600A394 RID: 41876 RVA: 0x004C8BE8 File Offset: 0x004C6DE8
		private void OnPointerClick()
		{
			bool flag = this._isHovering && UIManager.Instance.IsElementActive(UIElement.WorldMap);
			if (flag)
			{
				this._achievementQueue.Clear();
				this.CurrState = ViewAchievementPopUp.EAchievementPopUpState.Inactive;
				this._shouldHide = true;
				this.QuickHide();
				UIElement.Achievement.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("TemplateId", this._displayingTemplateId));
				UIManager.Instance.ShowUI(UIElement.Achievement, true);
			}
		}

		// Token: 0x0600A395 RID: 41877 RVA: 0x004C8C6C File Offset: 0x004C6E6C
		private void OnPointerRightClick()
		{
			bool isHovering = this._isHovering;
			if (isHovering)
			{
				this.CurrState = ViewAchievementPopUp.EAchievementPopUpState.Inactive;
			}
		}

		// Token: 0x0600A396 RID: 41878 RVA: 0x004C8C8C File Offset: 0x004C6E8C
		private void ChangeState()
		{
			ViewAchievementPopUp.EAchievementPopUpState currState = this.CurrState;
			if (!true)
			{
			}
			ViewAchievementPopUp.EAchievementPopUpState currState2;
			switch (currState)
			{
			case ViewAchievementPopUp.EAchievementPopUpState.Appearing:
				currState2 = ViewAchievementPopUp.EAchievementPopUpState.Displaying;
				break;
			case ViewAchievementPopUp.EAchievementPopUpState.Displaying:
				currState2 = (this._isHovering ? ViewAchievementPopUp.EAchievementPopUpState.Displaying : ViewAchievementPopUp.EAchievementPopUpState.Disappearing);
				break;
			case ViewAchievementPopUp.EAchievementPopUpState.Disappearing:
				currState2 = (this._isHovering ? ViewAchievementPopUp.EAchievementPopUpState.Displaying : ViewAchievementPopUp.EAchievementPopUpState.Inactive);
				break;
			case ViewAchievementPopUp.EAchievementPopUpState.Inactive:
				currState2 = ((this.TryGetNext(out this._currTemplateId) && this.CanvasGroup != null) ? ViewAchievementPopUp.EAchievementPopUpState.Appearing : ViewAchievementPopUp.EAchievementPopUpState.Inactive);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			this.CurrState = currState2;
		}

		// Token: 0x0600A397 RID: 41879 RVA: 0x004C8D14 File Offset: 0x004C6F14
		private void ResetTween(float alpha)
		{
			bool flag = this._currTween != null;
			if (flag)
			{
				this._currTween.Kill(false);
				this._currTween = null;
			}
			bool flag2 = this.CanvasGroup != null;
			if (flag2)
			{
				this.CanvasGroup.alpha = alpha;
			}
		}

		// Token: 0x0600A398 RID: 41880 RVA: 0x004C8D64 File Offset: 0x004C6F64
		private void Refresh(short templateId)
		{
			AchievementInfoItem config = AchievementInfo.Instance[templateId];
			this.icon.SetSprite(config.Icon, false, null);
			this.title.text = config.Name;
			this.desc.text = config.Desc;
			this._displayingTemplateId = templateId;
		}

		// Token: 0x0600A399 RID: 41881 RVA: 0x004C8DBD File Offset: 0x004C6FBD
		private void Add(short templateId)
		{
			this._achievementQueue.Enqueue(templateId);
		}

		// Token: 0x0600A39A RID: 41882 RVA: 0x004C8DD0 File Offset: 0x004C6FD0
		private bool TryGetNext(out short templateId)
		{
			bool flag = this.ToPopupAchievements.Count > 0;
			if (flag)
			{
				foreach (short id in this.ToPopupAchievements)
				{
					this.Add(id);
				}
				this.ToPopupAchievements.Clear();
			}
			return this._achievementQueue.TryDequeue(out templateId);
		}

		// Token: 0x04007F3C RID: 32572
		[SerializeField]
		private CImage icon;

		// Token: 0x04007F3D RID: 32573
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04007F3E RID: 32574
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04007F3F RID: 32575
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04007F40 RID: 32576
		private const float Duration = 4f;

		// Token: 0x04007F41 RID: 32577
		private const float StartDuration = 1f;

		// Token: 0x04007F42 RID: 32578
		private const float EndDuration = 1f;

		// Token: 0x04007F43 RID: 32579
		private Queue<short> _achievementQueue = new Queue<short>();

		// Token: 0x04007F44 RID: 32580
		private bool _isHovering;

		// Token: 0x04007F45 RID: 32581
		private bool _shouldHide;

		// Token: 0x04007F46 RID: 32582
		private float _currDuration;

		// Token: 0x04007F47 RID: 32583
		private short _currTemplateId;

		// Token: 0x04007F48 RID: 32584
		private short _displayingTemplateId;

		// Token: 0x04007F49 RID: 32585
		private Tween _currTween;

		// Token: 0x04007F4A RID: 32586
		private ViewAchievementPopUp.EAchievementPopUpState _currState = ViewAchievementPopUp.EAchievementPopUpState.Inactive;

		// Token: 0x020023ED RID: 9197
		private enum EAchievementPopUpState
		{
			// Token: 0x0400E0F6 RID: 57590
			Appearing,
			// Token: 0x0400E0F7 RID: 57591
			Displaying,
			// Token: 0x0400E0F8 RID: 57592
			Disappearing,
			// Token: 0x0400E0F9 RID: 57593
			Inactive
		}
	}
}
