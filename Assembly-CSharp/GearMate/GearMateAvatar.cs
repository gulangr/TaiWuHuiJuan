using System;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000611 RID: 1553
	public class GearMateAvatar : Refers
	{
		// Token: 0x060048D2 RID: 18642 RVA: 0x002212D4 File Offset: 0x0021F4D4
		public void Init(UI_GearMate uiGearMate)
		{
			this.InitRefers();
			this._parent = uiGearMate;
			this._btnChangeGearMate.ClearAndAddListener(new Action(this.OnChangeGearMateButtonClicked));
			this.SetBtnChangeGearMateCanInteract(SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuGearMateGroup().Count > 1);
		}

		// Token: 0x060048D3 RID: 18643 RVA: 0x00221324 File Offset: 0x0021F524
		private void SetBtnChangeGearMateCanInteract(bool canInteract)
		{
			this._btnChangeGearMate.interactable = canInteract;
			this._btnChangeGearMate.transform.GetChild(0).gameObject.SetActive(canInteract);
			this._btnChangeGearMate.transform.GetChild(1).gameObject.SetActive(!canInteract);
		}

		// Token: 0x060048D4 RID: 18644 RVA: 0x0022137C File Offset: 0x0021F57C
		public void RefreshCharacter(CharacterDisplayData gearMateDisplayData)
		{
			bool useGearMateSpine = gearMateDisplayData.TemplateId == 722;
			this._avatar.gameObject.SetActive(!useGearMateSpine);
			this._gearMateSpine.gameObject.SetActive(useGearMateSpine);
			bool flag = useGearMateSpine;
			if (flag)
			{
				this._gearMateSpine.AnimationState.Complete -= this.OnSpineAnimationComplete;
				this._gearMateSpine.AnimationState.Complete += this.OnSpineAnimationComplete;
				this._gearMateSpine.AnimationState.SetAnimation(0, "idle", true);
			}
			else
			{
				this._avatar.Refresh(gearMateDisplayData, true);
			}
			this._gearMateName.text = NameCenter.GetMonasticTitleOrDisplayName(gearMateDisplayData, false);
		}

		// Token: 0x060048D5 RID: 18645 RVA: 0x0022143C File Offset: 0x0021F63C
		private void OnSpineAnimationComplete(TrackEntry entry)
		{
			bool flag = entry.Animation.Name != "idle";
			if (flag)
			{
				this._gearMateSpine.AnimationState.SetAnimation(0, "idle", true);
			}
		}

		// Token: 0x060048D6 RID: 18646 RVA: 0x0022147B File Offset: 0x0021F67B
		private void OnChangeGearMateButtonClicked()
		{
			this._parent.OpenSelectGearMateView();
		}

		// Token: 0x060048D7 RID: 18647 RVA: 0x0022148C File Offset: 0x0021F68C
		public void ShowBubble(string bubbleText, float duration)
		{
			this._bubbleText.text = bubbleText;
			this._bubble.SetActive(true);
			bool flag = this._closeBubbleCoroutine != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(this._closeBubbleCoroutine);
			}
			this._closeBubbleCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(duration, delegate
			{
				bool flag2 = this._bubble != null;
				if (flag2)
				{
					this._bubble.SetActive(false);
				}
			});
		}

		// Token: 0x060048D8 RID: 18648 RVA: 0x002214F0 File Offset: 0x0021F6F0
		public void DoGearMateAnimation(string animationName)
		{
			bool activeSelf = this._gearMateSpine.gameObject.activeSelf;
			if (activeSelf)
			{
				this._gearMateSpine.AnimationState.SetAnimation(0, animationName, false);
			}
		}

		// Token: 0x060048D9 RID: 18649 RVA: 0x00221528 File Offset: 0x0021F728
		public void HideBubble()
		{
			this._bubble.SetActive(false);
		}

		// Token: 0x060048DA RID: 18650 RVA: 0x00221538 File Offset: 0x0021F738
		private void InitRefers()
		{
			this._gearMateName = base.CGet<TextMeshProUGUI>("GearMateName");
			this._btnChangeGearMate = base.CGet<CButtonObsolete>("BtnChangeGearMate");
			this._bubble = base.CGet<GameObject>("Bubble");
			this._bubbleText = base.CGet<TextMeshProUGUI>("BubbleText");
			this._avatar = base.CGet<Game.Components.Avatar.Avatar>("Avatar");
			this._gearMateSpine = base.CGet<SkeletonGraphic>("GearMateSpine");
		}

		// Token: 0x0400329D RID: 12957
		private TextMeshProUGUI _gearMateName;

		// Token: 0x0400329E RID: 12958
		private CButtonObsolete _btnChangeGearMate;

		// Token: 0x0400329F RID: 12959
		private GameObject _bubble;

		// Token: 0x040032A0 RID: 12960
		private TextMeshProUGUI _bubbleText;

		// Token: 0x040032A1 RID: 12961
		private Game.Components.Avatar.Avatar _avatar;

		// Token: 0x040032A2 RID: 12962
		private SkeletonGraphic _gearMateSpine;

		// Token: 0x040032A3 RID: 12963
		private UI_GearMate _parent;

		// Token: 0x040032A4 RID: 12964
		private Coroutine _closeBubbleCoroutine;
	}
}
