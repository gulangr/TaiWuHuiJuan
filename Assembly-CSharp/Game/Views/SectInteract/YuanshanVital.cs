using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009BF RID: 2495
	public class YuanshanVital : MonoBehaviour
	{
		// Token: 0x06007927 RID: 31015 RVA: 0x00385144 File Offset: 0x00383344
		public void Init(int vitalIndex, Action<int> onClickBtnLeave, Action<int> onClickBtnTransfer)
		{
			this._spine = this.spines.GetChild(vitalIndex).GetComponent<SkeletonGraphic>();
			this._spine.gameObject.SetActive(true);
			this._spine.AnimationState.Complete += delegate(TrackEntry entry)
			{
				bool flag = entry.Animation.Name != "idle";
				if (flag)
				{
					bool flag2 = entry.Animation.Name == "out";
					if (flag2)
					{
						this.SetShow(false);
					}
					this._spine.AnimationState.SetAnimation(0, "idle", true);
				}
			};
			this.btnLeave.ClearAndAddListener(delegate
			{
				onClickBtnLeave(vitalIndex);
			});
			this.btnTransfer.ClearAndAddListener(delegate
			{
				onClickBtnTransfer(vitalIndex);
			});
			this.btnOpen.ClearAndAddListener(new Action(this.ShowCharacterMenu));
			this.show.SetActive(false);
			this.hide.SetActive(false);
			this.emptyTips.SetActive(false);
			this.frame.SetActive(false);
			this.particle.SetActive(false);
		}

		// Token: 0x06007928 RID: 31016 RVA: 0x00385248 File Offset: 0x00383448
		public void Set(bool isGoodEnd, CharacterDisplayData vital, SectStoryThreeVitalsCharacter data)
		{
			this._data = data;
			this._id = vital.CharacterId;
			this._vitalName = NameCenter.GetMonasticTitleOrDisplayName(vital, false);
			int reverseThreshold = isGoodEnd ? GlobalConfig.Instance.ThreeVitalsThresholdHigh : GlobalConfig.Instance.ThreeVitalsThresholdLow;
			this.progress.Set(reverseThreshold, GlobalConfig.Instance.ThreeVitalsMaxInfection);
			this.progress.SetTips(isGoodEnd, data);
			this.SetShow(!data.IsInPrison);
		}

		// Token: 0x06007929 RID: 31017 RVA: 0x003852C8 File Offset: 0x003834C8
		public void SetShow(bool value)
		{
			if (value)
			{
				this.show.SetActive(true);
				this.hide.SetActive(false);
				this.emptyTips.SetActive(false);
				this.frame.SetActive(true);
				this.particle.SetActive(true);
				this.vitalName.text = this._vitalName;
				this.progress.SetProgress(this._data.Infection);
			}
			else
			{
				this.show.SetActive(false);
				this.hide.SetActive(true);
				this.emptyTips.SetActive(true);
				this.frame.SetActive(false);
				this.particle.SetActive(false);
				this.vitalName.text = LanguageKey.LK_Common_None.Tr().SetColor("494949");
			}
		}

		// Token: 0x0600792A RID: 31018 RVA: 0x003853AC File Offset: 0x003835AC
		public void Leave()
		{
			this.frame.SetActive(false);
			this._spine.AnimationState.SetAnimation(0, "out", false);
		}

		// Token: 0x0600792B RID: 31019 RVA: 0x003853D4 File Offset: 0x003835D4
		public void Come()
		{
			this.SetShow(true);
			this._spine.AnimationState.SetAnimation(0, "in", false);
		}

		// Token: 0x0600792C RID: 31020 RVA: 0x003853F8 File Offset: 0x003835F8
		public void ShowBubble(string text)
		{
			this.bubbleText.text = text;
			this.bubble.SetActive(true);
			bool flag = this._closeBubbleCoroutines != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(this._closeBubbleCoroutines);
			}
			this._closeBubbleCoroutines = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(2f, new Action(this.HideBubble));
			this._spine.AnimationState.SetAnimation(0, "talk", false);
		}

		// Token: 0x0600792D RID: 31021 RVA: 0x00385478 File Offset: 0x00383678
		public void HideBubble()
		{
			bool flag = this.bubble != null;
			if (flag)
			{
				this.bubble.SetActive(false);
			}
		}

		// Token: 0x0600792E RID: 31022 RVA: 0x003854A4 File Offset: 0x003836A4
		public void ShowCharacterMenu()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", this._id);
			argBox.Set("CanOperate", false);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x04005BBC RID: 23484
		public Transform spines;

		// Token: 0x04005BBD RID: 23485
		public TextMeshProUGUI vitalName;

		// Token: 0x04005BBE RID: 23486
		public GameObject bubble;

		// Token: 0x04005BBF RID: 23487
		public TextMeshProUGUI bubbleText;

		// Token: 0x04005BC0 RID: 23488
		public CButton btnTransfer;

		// Token: 0x04005BC1 RID: 23489
		public CButton btnLeave;

		// Token: 0x04005BC2 RID: 23490
		public CButton btnOpen;

		// Token: 0x04005BC3 RID: 23491
		public GameObject show;

		// Token: 0x04005BC4 RID: 23492
		public GameObject hide;

		// Token: 0x04005BC5 RID: 23493
		public GameObject frame;

		// Token: 0x04005BC6 RID: 23494
		public GameObject emptyTips;

		// Token: 0x04005BC7 RID: 23495
		public GameObject particle;

		// Token: 0x04005BC8 RID: 23496
		public YuanshanVitalProgress progress;

		// Token: 0x04005BC9 RID: 23497
		private SkeletonGraphic _spine;

		// Token: 0x04005BCA RID: 23498
		private SectStoryThreeVitalsCharacter _data;

		// Token: 0x04005BCB RID: 23499
		private Coroutine _closeBubbleCoroutines;

		// Token: 0x04005BCC RID: 23500
		private string _vitalName = "";

		// Token: 0x04005BCD RID: 23501
		private int _id;

		// Token: 0x04005BCE RID: 23502
		private const int Duration = 2;

		// Token: 0x04005BCF RID: 23503
		private const string Gray = "494949";
	}
}
