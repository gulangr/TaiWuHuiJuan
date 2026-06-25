using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using Config.Common;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000259 RID: 601
public class UI_Yuanshan : UIBase
{
	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x0600278E RID: 10126 RVA: 0x00123B59 File Offset: 0x00121D59
	// (set) Token: 0x0600278F RID: 10127 RVA: 0x00123B64 File Offset: 0x00121D64
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
				this.Icon.material = (config.GreyIcon ? this.GreyMaterial : this.NormalMaterial);
				bool flag2 = (uint)config.TemplateId >= UI_Yuanshan.Finish;
				if (flag2)
				{
					base.StartCoroutine(this.ShowEffectCoroutine(this.EffectCorrect, new Action(this.OnExit)));
				}
				else
				{
					this.StageTips.PresetParam[0] = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_Stage);
					this.StageTips.PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.UI_MiniGame_Yuanshan_Stage_Desc, config.SwapCount, MiniGameYuanshan.Instance[(int)UI_Yuanshan.Finish].Name);
				}
			}
		}
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x00123C7C File Offset: 0x00121E7C
	public void RefreshEffectPlayStatus()
	{
		MiniGameYuanshanItem config = MiniGameYuanshan.Instance[(int)this._stage];
		foreach (ValueTuple<ParticleSystem, bool> valueTuple in this.EffectLooping.Zip(config.EnableEffect, (ParticleSystem x, bool y) => new ValueTuple<ParticleSystem, bool>(x, y)))
		{
			ParticleSystem effect = valueTuple.Item1;
			bool enable = valueTuple.Item2;
			bool flag = enable;
			if (flag)
			{
				effect.Play();
				effect.gameObject.SetActive(true);
			}
			else
			{
				effect.gameObject.SetActive(false);
				effect.Stop();
			}
		}
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x00123D44 File Offset: 0x00121F44
	public IEnumerator ShowEffectCoroutine(ParticleSystem[] EffectShown, Action callback = null)
	{
		AudioManager.Instance.Play(new AudioCommand
		{
			AudioName = this.PlayEffectStr,
			Loop = false,
			CanSetPitchByGlobal = false,
			Volume = 60
		});
		foreach (ParticleSystem effect in EffectShown)
		{
			effect.Play();
			effect = null;
		}
		ParticleSystem[] array = null;
		foreach (ParticleSystem effect2 in RuntimeHelpers.GetSubArray<ParticleSystem>(EffectShown, Range.EndAt(new Index(1, true))))
		{
			while (effect2.isPlaying)
			{
				yield return null;
			}
			effect2 = null;
		}
		ParticleSystem[] array2 = null;
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x00123D64 File Offset: 0x00121F64
	public void ShowEffect(ParticleSystem[] EffectShown)
	{
		this.SetCanStart(true);
		AudioManager.Instance.Play(new AudioCommand
		{
			AudioName = this.PlayEffectStr,
			Loop = false,
			CanSetPitchByGlobal = false,
			Volume = 60
		});
		base.StartCoroutine(this.ShowEffectCoroutine(EffectShown, new Action(this.RefreshEffectPlayStatus)));
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x00123DC8 File Offset: 0x00121FC8
	public override void OnInit(ArgumentBox argBox)
	{
		this._stage = uint.MaxValue;
		int stage;
		this.Stage = (uint)(argBox.Get(SectMainStoryEventArgKey.DefValue.YuanshanMiniGameStage, out stage) ? stage : 0);
		this.RefreshEffectPlayStatus();
		this.SetCanStart(true);
		this.Tips.SetActive(false);
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
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x00123ED8 File Offset: 0x001220D8
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

	// Token: 0x06002795 RID: 10133 RVA: 0x00123F4C File Offset: 0x0012214C
	public void StartSwap(UI_Yuanshan.EAnimPhase phase)
	{
		if (phase != UI_Yuanshan.EAnimPhase.Start)
		{
			if (phase != UI_Yuanshan.EAnimPhase.Swap)
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

	// Token: 0x06002796 RID: 10134 RVA: 0x00123FBD File Offset: 0x001221BD
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
		this.Tips.SetActive(true);
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

	// Token: 0x06002797 RID: 10135 RVA: 0x00123FCC File Offset: 0x001221CC
	public void SetCanStart(bool canStart)
	{
		CButtonObsolete closeButton = this.CloseButton;
		this.BtnStart.interactable = canStart;
		closeButton.interactable = canStart;
		this._isProcessing = !canStart;
		foreach (YuanshanBox box in this.YuanshanBox)
		{
			box.Selector.interactable = false;
			box.Locked = this._isProcessing;
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x00124038 File Offset: 0x00122238
	public void OnBoxSelected(bool isTrueBox)
	{
		this.BtnStartText.text = LocalStringManager.Get(LanguageKey.UI_MiniGame_Yuanshan_BtnStart);
		this.SetCanStart(false);
		this.Tips.SetActive(false);
		foreach (YuanshanBox box in this.YuanshanBox)
		{
			box.ShowBone(isTrueBox);
		}
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x00124098 File Offset: 0x00122298
	public void LockAllSelection()
	{
		foreach (YuanshanBox box in this.YuanshanBox)
		{
			box.Selector.interactable = false;
		}
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x001240CF File Offset: 0x001222CF
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

	// Token: 0x0600279B RID: 10139 RVA: 0x001240E6 File Offset: 0x001222E6
	public IEnumerable Swap(YuanshanBox box1, YuanshanBox box2, float duration)
	{
		Vector3 loc = box1.Self.position;
		Vector3 loc2 = box2.Self.position;
		TweenerCore<Vector3, Vector3, VectorOptions> delay = box1.Self.DOMove(loc2, duration, false);
		yield return box2.Self.DOMove(loc, duration, false).WaitForCompletion();
		yield return delay.WaitForCompletion();
		yield break;
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x0012410C File Offset: 0x0012230C
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

	// Token: 0x0600279D RID: 10141 RVA: 0x001241AF File Offset: 0x001223AF
	public void OnExit()
	{
		base.QuickHide();
		TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("FinishYuanshanMiniGame", SectMainStoryEventArgKey.DefValue.YuanshanMiniGameStage, (int)((GMFunc.YuanShanExitWithStage < 0) ? this.Stage : ((uint)GMFunc.YuanShanExitWithStage)));
		TaiwuEventDomainMethod.Call.TriggerListener("FinishYuanshanMiniGame", true);
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x001241EF File Offset: 0x001223EF
	private void Awake()
	{
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x00124209 File Offset: 0x00122409
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x00124223 File Offset: 0x00122423
	private void TopUiChanged(ArgumentBox argBox)
	{
		this.Self.SetActive(UIManager.Instance.IsFocusElement(this.Element));
	}

	// Token: 0x04001CCD RID: 7373
	internal static uint Finish = 3U;

	// Token: 0x04001CCE RID: 7374
	private uint _stage;

	// Token: 0x04001CCF RID: 7375
	public UnityEngine.Material GreyMaterial;

	// Token: 0x04001CD0 RID: 7376
	public UnityEngine.Material NormalMaterial;

	// Token: 0x04001CD1 RID: 7377
	public GameObject Effect;

	// Token: 0x04001CD2 RID: 7378
	public ParticleSystem[] EffectCorrect;

	// Token: 0x04001CD3 RID: 7379
	public ParticleSystem[] EffectIncorrect;

	// Token: 0x04001CD4 RID: 7380
	public ParticleSystem[] EffectFinish;

	// Token: 0x04001CD5 RID: 7381
	public ParticleSystem[] EffectLooping;

	// Token: 0x04001CD6 RID: 7382
	public CButtonObsolete CloseButton;

	// Token: 0x04001CD7 RID: 7383
	private bool _isProcessing = false;

	// Token: 0x04001CD8 RID: 7384
	public string PlayEffectStr = "SFX_BoxMove_sensing";

	// Token: 0x04001CD9 RID: 7385
	public string GetBoxCorrect = "SFX_BoxMove_get";

	// Token: 0x04001CDA RID: 7386
	public string GetBoxIncorrect = "SFX_BoxMove_empty";

	// Token: 0x04001CDB RID: 7387
	public string BoxMove = "SFX_BoxMove_move_loop";

	// Token: 0x04001CDC RID: 7388
	public string GameStart = "SFX_BoxMove_put";

	// Token: 0x04001CDD RID: 7389
	public TooltipInvoker StageTips;

	// Token: 0x04001CDE RID: 7390
	public TextMeshProUGUI StageDescription;

	// Token: 0x04001CDF RID: 7391
	public GameObject Self;

	// Token: 0x04001CE0 RID: 7392
	public GameObject Tips;

	// Token: 0x04001CE1 RID: 7393
	public CButtonObsolete BtnStart;

	// Token: 0x04001CE2 RID: 7394
	public TextMeshProUGUI BtnStartText;

	// Token: 0x04001CE3 RID: 7395
	public Image Icon;

	// Token: 0x04001CE4 RID: 7396
	public YuanshanBox[] YuanshanBox;

	// Token: 0x04001CE5 RID: 7397
	public readonly ValueTuple<int, int, int>[] Permutations = new ValueTuple<int, int, int>[]
	{
		new ValueTuple<int, int, int>(0, 2, 1),
		new ValueTuple<int, int, int>(1, 0, 2),
		new ValueTuple<int, int, int>(1, 2, 0),
		new ValueTuple<int, int, int>(2, 0, 1),
		new ValueTuple<int, int, int>(2, 1, 0)
	};

	// Token: 0x020015A0 RID: 5536
	public enum EAnimPhase
	{
		// Token: 0x0400A543 RID: 42307
		Start,
		// Token: 0x0400A544 RID: 42308
		Swap
	}
}
