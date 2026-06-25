using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom
{
	// Token: 0x02000C49 RID: 3145
	[Obsolete("界面已经不再使用")]
	public class ViewAdvanceDays : UIBase
	{
		// Token: 0x06009FDE RID: 40926 RVA: 0x004AA78C File Offset: 0x004A898C
		private void RefreshScrollLang()
		{
			foreach (WorldStateDisplayItem item in this.scrollLeftItems.Concat(this.scrollRightItems))
			{
				item.Set(this._worldStateData);
			}
		}

		// Token: 0x06009FDF RID: 40927 RVA: 0x004AA7F0 File Offset: 0x004A89F0
		public void Awake()
		{
			foreach (DOTweenAnimation anim in this.anims)
			{
				anim.CreateTween(anim.autoPlay = false, anim.autoKill = false);
				anim.tween.SetUpdate(true);
			}
			this.InitAdvanceDaysBtn();
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.RefreshLang));
		}

		// Token: 0x06009FE0 RID: 40928 RVA: 0x004AA865 File Offset: 0x004A8A65
		public void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.RefreshLang));
		}

		// Token: 0x06009FE1 RID: 40929 RVA: 0x004AA884 File Offset: 0x004A8A84
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
		}

		// Token: 0x06009FE2 RID: 40930 RVA: 0x004AA8A0 File Offset: 0x004A8AA0
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
			foreach (DOTweenAnimation anim in this.anims)
			{
				Tween tween = anim.tween;
				if (tween != null)
				{
					tween.Pause<Tween>();
				}
				Tween tween2 = anim.tween;
				if (tween2 != null)
				{
					tween2.Rewind(true);
				}
			}
		}

		// Token: 0x06009FE3 RID: 40931 RVA: 0x004AA908 File Offset: 0x004A8B08
		private void OnTopUiChange(ArgumentBox _)
		{
			bool flag = !UIManager.Instance.IsFocusElement(this.Element) && !UIManager.Instance.IsFocusElement(UIElement.PopupMenu) && !UIManager.Instance.IsFocusElement(UIElement.Dialog);
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06009FE4 RID: 40932 RVA: 0x004AA959 File Offset: 0x004A8B59
		public void RefreshLang(ArgumentBox _)
		{
			this.RefreshScrollLang();
		}

		// Token: 0x06009FE5 RID: 40933 RVA: 0x004AA964 File Offset: 0x004A8B64
		public override void OnInit(ArgumentBox _)
		{
			this.timeBallEffect.InitEffect();
			this.RefreshAdvanceDaysInfo();
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				WorldDomainMethod.AsyncCall.RequestWorldStateData(this, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._worldStateData);
					this.RefreshLang(null);
					foreach (DOTweenAnimation anim in this.anims)
					{
						bool flag = anim.tween == null;
						if (flag)
						{
							anim.CreateTween(anim.autoPlay = false, anim.autoKill = false);
							anim.tween.SetUpdate(true);
						}
						anim.tween.Restart(true, -1f);
					}
					this.Element.ShowAfterRefresh();
				});
			}));
		}

		// Token: 0x06009FE6 RID: 40934 RVA: 0x004AA9B4 File Offset: 0x004A8BB4
		public void RefreshAdvanceDaysInfo()
		{
			ViewAdvanceDays.<>c__DisplayClass32_0 CS$<>8__locals1 = new ViewAdvanceDays.<>c__DisplayClass32_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			this.slider.maxValue = (float)CS$<>8__locals1.leftDays;
			this.slider.value = (this.slider.minValue = 0f);
			CS$<>8__locals1.<RefreshAdvanceDaysInfo>g__UpdateActionPointText|2(this.slider.value);
			this.slider.onValueChanged.AddListener(new UnityAction<float>(CS$<>8__locals1.<RefreshAdvanceDaysInfo>g__UpdateActionPointText|2));
			this.leftArrow.onClick.ResetListener(delegate()
			{
				CS$<>8__locals1.<>4__this.slider.value = Mathf.Max(0f, CS$<>8__locals1.<>4__this.slider.value - 1f);
			});
			this.rightArrow.onClick.ResetListener(delegate()
			{
				CS$<>8__locals1.<>4__this.slider.value = Mathf.Min(CS$<>8__locals1.<>4__this.slider.maxValue, CS$<>8__locals1.<>4__this.slider.value + 1f);
			});
		}

		// Token: 0x06009FE7 RID: 40935 RVA: 0x004AAA7D File Offset: 0x004A8C7D
		public void InitAdvanceDaysBtn()
		{
			this.advanceDaysConfirm.ClearAndAddListener(new Action(this.Confirm));
			this.advanceDaysCancel.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x06009FE8 RID: 40936 RVA: 0x004AAAB4 File Offset: 0x004A8CB4
		private void Confirm()
		{
			this.QuickHide();
			bool notInAdventure = SingletonObject.getInstance<AdventureRemakeModel>().AdventureTaiwu.NotInAdventure;
			if (notInAdventure)
			{
				bool flag = this.slider.value == 0f;
				if (flag)
				{
					GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, null);
				}
				else
				{
					WorldDomainMethod.Call.AdvanceDaysInMonth((int)this.slider.value);
				}
			}
			else
			{
				bool flag2 = this.slider.value == 0f;
				if (flag2)
				{
					GEvent.OnEvent(UiEvents.ShowAdvanceMonthConfirm, null);
				}
				else
				{
					GEvent.OnEvent(UiEvents.AdventureAdvanceDaysSet, EasyPool.Get<ArgumentBox>().Set("AdvanceDays", (int)this.slider.value));
				}
			}
		}

		// Token: 0x06009FE9 RID: 40937 RVA: 0x004AAB70 File Offset: 0x004A8D70
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.Confirm();
			}
		}

		// Token: 0x04007BB7 RID: 31671
		[SerializeField]
		private DOTweenAnimation[] anims;

		// Token: 0x04007BB8 RID: 31672
		[SerializeField]
		private CSlider slider;

		// Token: 0x04007BB9 RID: 31673
		[SerializeField]
		private CButton advanceDaysConfirm;

		// Token: 0x04007BBA RID: 31674
		[SerializeField]
		private CButton advanceDaysCancel;

		// Token: 0x04007BBB RID: 31675
		[SerializeField]
		private CButton leftArrow;

		// Token: 0x04007BBC RID: 31676
		[SerializeField]
		private CButton rightArrow;

		// Token: 0x04007BBD RID: 31677
		[SerializeField]
		private TMP_Text currActionPoint;

		// Token: 0x04007BBE RID: 31678
		[SerializeField]
		private TMP_Text toActionPoint;

		// Token: 0x04007BBF RID: 31679
		[SerializeField]
		private CImage iconFill;

		// Token: 0x04007BC0 RID: 31680
		[SerializeField]
		private CImage bgFill;

		// Token: 0x04007BC1 RID: 31681
		[SerializeField]
		private CImage titleImage;

		// Token: 0x04007BC2 RID: 31682
		[SerializeField]
		private RectTransform scrollLeftContent;

		// Token: 0x04007BC3 RID: 31683
		[SerializeField]
		private RectTransform scrollRightContent;

		// Token: 0x04007BC4 RID: 31684
		[SerializeField]
		private WorldStateDisplayItem[] scrollLeftItems;

		// Token: 0x04007BC5 RID: 31685
		[SerializeField]
		private WorldStateDisplayItem[] scrollRightItems;

		// Token: 0x04007BC6 RID: 31686
		[SerializeField]
		private Sprite[] advMonth;

		// Token: 0x04007BC7 RID: 31687
		[SerializeField]
		private Sprite[] consumeTime;

		// Token: 0x04007BC8 RID: 31688
		[SerializeField]
		private int fontNormal = 60;

		// Token: 0x04007BC9 RID: 31689
		[SerializeField]
		private int fontMinus = 48;

		// Token: 0x04007BCA RID: 31690
		[SerializeField]
		private TimeBallEffect timeBallEffect;

		// Token: 0x04007BCB RID: 31691
		private static readonly Dictionary<LocalStringManager.LanguageType, int> SpriteIndex = new Dictionary<LocalStringManager.LanguageType, int>
		{
			{
				LocalStringManager.LanguageType.CN,
				0
			},
			{
				LocalStringManager.LanguageType.EN,
				1
			}
		};

		// Token: 0x04007BCC RID: 31692
		private static readonly sbyte[] ScrollLeftDefKeys = new sbyte[]
		{
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			22,
			23,
			24,
			20,
			21
		};

		// Token: 0x04007BCD RID: 31693
		private static readonly sbyte[] ScrollRightDefKeys = new sbyte[]
		{
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			40,
			51,
			48,
			50
		};

		// Token: 0x04007BCE RID: 31694
		private WorldStateData _worldStateData;
	}
}
