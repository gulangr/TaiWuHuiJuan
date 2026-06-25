using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Config.Common;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract
{
	// Token: 0x020009BB RID: 2491
	public class ViewYuanshanMiniGame : UIBase
	{
		// Token: 0x17000D6E RID: 3438
		// (get) Token: 0x060078AD RID: 30893 RVA: 0x00382774 File Offset: 0x00380974
		// (set) Token: 0x060078AE RID: 30894 RVA: 0x0038277C File Offset: 0x0038097C
		public uint Stage
		{
			get
			{
				return this._stage;
			}
			set
			{
				bool flag = this._stage == value || value > 3U;
				if (!flag)
				{
					this.StageDescription.alignment = TextAlignmentOptions.Left;
					TMP_Text stageDescription = this.StageDescription;
					ConfigData<MiniGameYuanshanItem, byte> instance = MiniGameYuanshan.Instance;
					this._stage = value;
					stageDescription.text = instance[(int)value].Name;
					MiniGameYuanshanItem config = MiniGameYuanshan.Instance[(int)this._stage];
					this.Icon.SetSprite("ui9_icon_sectpopup_4_selectremain_img_level_" + this._stage.ToString(), false, null);
					this.Progress.fillAmount = this._stage / 3f;
					this.ProgressPreview.fillAmount = Math.Min((this._stage + 1U) / 3f, 1f);
					bool flag2 = (uint)config.TemplateId >= ViewYuanshanMiniGame.Finish;
					if (flag2)
					{
						base.StartCoroutine(this.ShowEffectCoroutine(this.LevelEffList[(int)this._stage], new Action(this.OnExit)));
					}
					else
					{
						this.StageTips.PresetParam[0] = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_Stage);
						this.StageTips.PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.UI_MiniGame_Yuanshan_Stage_Desc, config.SwapCount, MiniGameYuanshan.Instance[(int)ViewYuanshanMiniGame.Finish].Name);
					}
				}
			}
		}

		// Token: 0x060078AF RID: 30895 RVA: 0x003828DA File Offset: 0x00380ADA
		public IEnumerator ShowEffectCoroutine(ParticleSystem EffectShown, Action callback = null)
		{
			AudioManager.Instance.Play(new AudioCommand
			{
				AudioName = this.PlayEffectStr,
				Loop = false,
				CanSetPitchByGlobal = false,
				Volume = 60
			});
			EffectShown.Play();
			yield return new WaitForSeconds(this._effDurationTest);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		// Token: 0x060078B0 RID: 30896 RVA: 0x003828F8 File Offset: 0x00380AF8
		public override void OnInit(ArgumentBox argBox)
		{
			this._stage = uint.MaxValue;
			int stage;
			this.Stage = (uint)(argBox.Get(SectMainStoryEventArgKey.DefValue.YuanshanMiniGameStage, out stage) ? stage : 0);
			this.SetCanStart(true);
			int trueBox = Random.Range(0, 3);
			Array.Sort<YuanshanBox>(this.YuanshanBox, (YuanshanBox x, YuanshanBox y) => x.Self.anchoredPosition.x.CompareTo(y.Self.anchoredPosition.x));
			for (int i = 0; i < this.YuanshanBox.Length; i++)
			{
				this.YuanshanBox[i].IsTrueBox = (trueBox == i);
			}
			foreach (YuanshanBox box in this.YuanshanBox)
			{
				box.SetSelectable(false);
				box.SkeletonGraphic.AnimationState.SetAnimation(0, box.IsTrueBox ? box.Opened : box.Closed, box.IsTrueBox);
			}
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.OnShowed));
		}

		// Token: 0x060078B1 RID: 30897 RVA: 0x00382A19 File Offset: 0x00380C19
		private void OnShowed()
		{
			base.DelayCall(delegate
			{
				this.LevelEffList[(int)this._stage].Play();
			}, 0.5f);
		}

		// Token: 0x060078B2 RID: 30898 RVA: 0x00382A34 File Offset: 0x00380C34
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string text = name;
			string a = text;
			if (!(a == "BtnStart"))
			{
				if (a == "ButtonCloseView")
				{
					this.QuickHide();
				}
			}
			else
			{
				this.StartGame(string.Empty);
			}
		}

		// Token: 0x060078B3 RID: 30899 RVA: 0x00382A80 File Offset: 0x00380C80
		public void StartGame(string _)
		{
			base.StopAllCoroutines();
			this.Effect.SetActive(false);
			AudioManager.Instance.StopSound(this.PlayEffectStr);
			this.BtnStartText.text = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_BtnStart_Processing);
			this.SetCanStart(false);
			foreach (YuanshanBox box in this.YuanshanBox)
			{
				box.CloseBox();
			}
		}

		// Token: 0x060078B4 RID: 30900 RVA: 0x00382AF4 File Offset: 0x00380CF4
		public void StartSwap(ViewYuanshanMiniGame.EAnimPhase phase)
		{
			if (phase != ViewYuanshanMiniGame.EAnimPhase.Start)
			{
				if (phase != ViewYuanshanMiniGame.EAnimPhase.Swap)
				{
					throw new ArgumentException("Invalid phase", "phase");
				}
				base.StartCoroutine(this.SwapCoroutine());
			}
			else
			{
				foreach (YuanshanBox box in this.YuanshanBox)
				{
					bool isTrueBox = box.IsTrueBox;
					if (isTrueBox)
					{
						box.CloseBox();
						break;
					}
				}
			}
		}

		// Token: 0x060078B5 RID: 30901 RVA: 0x00382B65 File Offset: 0x00380D65
		private IEnumerator SwapCoroutine()
		{
			float duration = MiniGameYuanshan.Instance[(int)this._stage].SwapDuration;
			int count = MiniGameYuanshan.Instance[(int)this._stage].SwapCount;
			int num;
			for (int i = 0; i < count; i = num + 1)
			{
				foreach (object s in this.HardSwap(duration))
				{
					yield return s;
					s = null;
				}
				IEnumerator enumerator = null;
				num = i;
			}
			foreach (YuanshanBox box in this.YuanshanBox)
			{
				box.SetSelectable(true);
				box = null;
			}
			YuanshanBox[] array = null;
			this.BtnStartText.text = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_BtnStart_Processing);
			yield break;
			yield break;
		}

		// Token: 0x060078B6 RID: 30902 RVA: 0x00382B74 File Offset: 0x00380D74
		public void SetCanStart(bool canStart)
		{
			Selectable closeButton = this.CloseButton;
			this.BtnStart.interactable = canStart;
			closeButton.interactable = canStart;
			this._isProcessing = !canStart;
			foreach (YuanshanBox box in this.YuanshanBox)
			{
				box.Selector.interactable = false;
			}
		}

		// Token: 0x060078B7 RID: 30903 RVA: 0x00382BD4 File Offset: 0x00380DD4
		public void OnBoxSelected(bool isTrueBox)
		{
			this.BtnStartText.text = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_BtnStart);
			this.SetCanStart(false);
			foreach (YuanshanBox box in this.YuanshanBox)
			{
				box.ShowBone(isTrueBox);
				box.SetSelectable(false);
			}
		}

		// Token: 0x060078B8 RID: 30904 RVA: 0x00382C2C File Offset: 0x00380E2C
		public void LockAllSelection()
		{
			foreach (YuanshanBox box in this.YuanshanBox)
			{
				box.Selector.interactable = false;
			}
		}

		// Token: 0x060078B9 RID: 30905 RVA: 0x00382C63 File Offset: 0x00380E63
		public IEnumerable HardSwap(float duration)
		{
			AudioManager.Instance.PlaySound(this.BoxMove, true, false);
			MiniGameYuanshanItem config = MiniGameYuanshan.Instance[(int)this._stage];
			ValueTuple<int, int, int> perm = this.Permutations[Random.Range(0, 5)];
			Vector3[] initLoc = (from x in this.YuanshanBox
			select x.Self.localPosition).ToArray<Vector3>();
			Vector3[] endLoc = new Vector3[]
			{
				initLoc[perm.Item1],
				initLoc[perm.Item2],
				initLoc[perm.Item3]
			};
			IEnumerable<YuanshanBox> yuanshanBox = this.YuanshanBox;
			IEnumerable<Vector3> second = endLoc;
			Func<YuanshanBox, Vector3, TweenerCore<Vector3, Vector3, VectorOptions>> <>9__1;
			Func<YuanshanBox, Vector3, TweenerCore<Vector3, Vector3, VectorOptions>> resultSelector;
			if ((resultSelector = <>9__1) == null)
			{
				resultSelector = (<>9__1 = ((YuanshanBox box, Vector3 loc) => box.Self.DOLocalMove(loc, duration, false)));
			}
			foreach (TweenerCore<Vector3, Vector3, VectorOptions> anim in yuanshanBox.Zip(second, resultSelector).ToArray<TweenerCore<Vector3, Vector3, VectorOptions>>())
			{
				bool active = anim.active;
				if (active)
				{
					yield return anim.WaitForCompletion();
				}
				anim = null;
			}
			TweenerCore<Vector3, Vector3, VectorOptions>[] array = null;
			AudioManager.Instance.StopSound(this.BoxMove);
			yield break;
		}

		// Token: 0x060078BA RID: 30906 RVA: 0x00382C7A File Offset: 0x00380E7A
		public IEnumerable Swap(YuanshanBox box1, YuanshanBox box2, float duration)
		{
			Vector3 loc = box1.Self.position;
			Vector3 loc2 = box2.Self.position;
			TweenerCore<Vector3, Vector3, VectorOptions> delay = box1.Self.DOMove(loc2, duration, false);
			yield return box2.Self.DOMove(loc, duration, false).WaitForCompletion();
			yield return delay.WaitForCompletion();
			yield break;
		}

		// Token: 0x060078BB RID: 30907 RVA: 0x00382CA0 File Offset: 0x00380EA0
		public override void QuickHide()
		{
			bool isProcessing = this._isProcessing;
			if (!isProcessing)
			{
				UIElement dialog = UIElement.Dialog;
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				string key = "Cmd";
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_ConfirmExit);
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_ConfirmContent);
				dialogCmd.Yes = new Action(this.OnExit);
				dialogCmd.No = delegate()
				{
				};
				dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x060078BC RID: 30908 RVA: 0x00382D43 File Offset: 0x00380F43
		public void OnExit()
		{
			base.QuickHide();
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("FinishYuanshanMiniGame", SectMainStoryEventArgKey.DefValue.YuanshanMiniGameStage, (int)((GMFunc.YuanShanExitWithStage < 0) ? this.Stage : ((uint)GMFunc.YuanShanExitWithStage)));
			TaiwuEventDomainMethod.Call.TriggerListener("FinishYuanshanMiniGame", true);
		}

		// Token: 0x060078BD RID: 30909 RVA: 0x00382D84 File Offset: 0x00380F84
		private void Awake()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			YuanshanBox[] yuanshanBox = this.YuanshanBox;
			for (int i = 0; i < yuanshanBox.Length; i++)
			{
				YuanshanBox box = yuanshanBox[i];
				box.PointerTrigger.EnterEvent.RemoveAllListeners();
				box.PointerTrigger.EnterEvent.AddListener(delegate()
				{
					box.Hover.gameObject.SetActive(!box.Locked);
				});
				box.PointerTrigger.ExitEvent.RemoveAllListeners();
				box.PointerTrigger.ExitEvent.AddListener(delegate()
				{
					box.Hover.gameObject.SetActive(false);
				});
			}
		}

		// Token: 0x060078BE RID: 30910 RVA: 0x00382E47 File Offset: 0x00381047
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x060078BF RID: 30911 RVA: 0x00382E61 File Offset: 0x00381061
		private void TopUiChanged(ArgumentBox argBox)
		{
			this.Self.SetActive(UIManager.Instance.IsFocusElement(this.Element));
		}

		// Token: 0x04005B5B RID: 23387
		internal static uint Finish = 3U;

		// Token: 0x04005B5C RID: 23388
		private uint _stage;

		// Token: 0x04005B5D RID: 23389
		public GameObject Effect;

		// Token: 0x04005B5E RID: 23390
		public CButton CloseButton;

		// Token: 0x04005B5F RID: 23391
		private bool _isProcessing = false;

		// Token: 0x04005B60 RID: 23392
		public string PlayEffectStr = "SFX_BoxMove_sensing";

		// Token: 0x04005B61 RID: 23393
		public string GetBoxCorrect = "SFX_BoxMove_get";

		// Token: 0x04005B62 RID: 23394
		public string GetBoxIncorrect = "SFX_BoxMove_empty";

		// Token: 0x04005B63 RID: 23395
		public string BoxMove = "SFX_BoxMove_move_loop";

		// Token: 0x04005B64 RID: 23396
		public string GameStart = "SFX_BoxMove_put";

		// Token: 0x04005B65 RID: 23397
		public float _effDurationTest = 1f;

		// Token: 0x04005B66 RID: 23398
		private static readonly WaitForSeconds _effDuration = new WaitForSeconds(0.5f);

		// Token: 0x04005B67 RID: 23399
		public TooltipInvoker StageTips;

		// Token: 0x04005B68 RID: 23400
		public TextMeshProUGUI StageDescription;

		// Token: 0x04005B69 RID: 23401
		public GameObject Self;

		// Token: 0x04005B6A RID: 23402
		public GameObject Tips;

		// Token: 0x04005B6B RID: 23403
		public CButton BtnStart;

		// Token: 0x04005B6C RID: 23404
		public TextMeshProUGUI BtnStartText;

		// Token: 0x04005B6D RID: 23405
		public CImage Icon;

		// Token: 0x04005B6E RID: 23406
		public YuanshanBox[] YuanshanBox;

		// Token: 0x04005B6F RID: 23407
		public ParticleSystem[] LevelEffList;

		// Token: 0x04005B70 RID: 23408
		public CImage Progress;

		// Token: 0x04005B71 RID: 23409
		public CImage ProgressPreview;

		// Token: 0x04005B72 RID: 23410
		public readonly ValueTuple<int, int, int>[] Permutations = new ValueTuple<int, int, int>[]
		{
			new ValueTuple<int, int, int>(0, 2, 1),
			new ValueTuple<int, int, int>(1, 0, 2),
			new ValueTuple<int, int, int>(1, 2, 0),
			new ValueTuple<int, int, int>(2, 0, 1),
			new ValueTuple<int, int, int>(2, 1, 0)
		};

		// Token: 0x02001F00 RID: 7936
		public enum EAnimPhase
		{
			// Token: 0x0400CBE2 RID: 52194
			Start,
			// Token: 0x0400CBE3 RID: 52195
			Swap
		}
	}
}
