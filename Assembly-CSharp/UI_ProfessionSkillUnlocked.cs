using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class UI_ProfessionSkillUnlocked : UIBase
{
	// Token: 0x170005BE RID: 1470
	// (get) Token: 0x06003732 RID: 14130 RVA: 0x001BC924 File Offset: 0x001BAB24
	private IEnumerable<int> AllProfessions
	{
		get
		{
			int num;
			for (int i = 0; i <= 17; i = num + 1)
			{
				yield return i;
				num = i;
			}
			yield break;
		}
	}

	// Token: 0x06003733 RID: 14131 RVA: 0x001BC943 File Offset: 0x001BAB43
	public override void OnInit(ArgumentBox argsBox)
	{
		this._playingExtraProfessionSkillUnlockedAnim = false;
		this.TryPlayNextSkill();
	}

	// Token: 0x06003734 RID: 14132 RVA: 0x001BC954 File Offset: 0x001BAB54
	private void OnEnable()
	{
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
	}

	// Token: 0x06003735 RID: 14133 RVA: 0x001BC96F File Offset: 0x001BAB6F
	private void Awake()
	{
		this.closeBtn.onClick.ResetListener(new Action(this.QuickHide));
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x001BC990 File Offset: 0x001BAB90
	private void TopUiChanged(ArgumentBox argBox)
	{
		this.QuickHide();
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x001BC99A File Offset: 0x001BAB9A
	private void OnDisable()
	{
		this.HandleWaitingToDestroyed();
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
	}

	// Token: 0x06003738 RID: 14136 RVA: 0x001BC9BC File Offset: 0x001BABBC
	private void HandleWaitingToDestroyed()
	{
		for (int i = this.animationRoot.childCount - 1; i >= 0; i--)
		{
			GameObject go = this.animationRoot.GetChild(i).gameObject;
			Object.Destroy(go);
		}
	}

	// Token: 0x06003739 RID: 14137 RVA: 0x001BCA04 File Offset: 0x001BAC04
	private void Update()
	{
		bool flag = this._animState == UI_ProfessionSkillUnlocked.AnimState.Playing && !this._playingExtraProfessionSkillUnlockedAnim && (CommonCommandKit.RightMouse.Check(this.Element, false, false, true, true, false) || CommonCommandKit.LeftMouse.Check(this.Element, false, false, true, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, true, true, false));
		if (flag)
		{
			base.StartCoroutine(this.SkipPlaySkillUnlocked());
		}
		else
		{
			bool flag2 = this._animState == UI_ProfessionSkillUnlocked.AnimState.Finish && (CommonCommandKit.RightMouse.Check(this.Element, false, false, true, true, false) || CommonCommandKit.LeftMouse.Check(this.Element, false, false, true, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, true, true, false) || CommonCommandKit.Esc.Check(this.Element, false, false, true, true, false));
			if (flag2)
			{
				bool flag3 = UI_ProfessionSkillUnlocked.HasProfessionSkillUnlockedToShow();
				if (flag3)
				{
					this.TryPlayNextSkill();
				}
				else
				{
					bool needPlayExtraProfessionSkillUnlockedAnim = UI_ProfessionSkillUnlocked._needPlayExtraProfessionSkillUnlockedAnim;
					if (needPlayExtraProfessionSkillUnlockedAnim)
					{
						base.StartCoroutine(this.CoPlayExtraSkillUnlocked());
					}
					else
					{
						this.QuickHide();
					}
				}
			}
		}
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x001BCB24 File Offset: 0x001BAD24
	private bool AnimDequeue(out int templateId)
	{
		return UI_ProfessionSkillUnlocked.ProfessionSkillUnlockedQueue.TryDequeue(out templateId);
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x001BCB44 File Offset: 0x001BAD44
	private void TryPlayNextSkill()
	{
		bool flag = !this.AnimDequeue(out this._currTemplateId);
		if (flag)
		{
			UIManager.Instance.HideUI(UIElement.ProfessionSkillUnlocked);
		}
		else
		{
			Debug.LogWarning(string.Format("_currTemplateId is : {0}!", this._currTemplateId));
			this.ResetAnim();
			this.SetSkill();
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				this._animCoroutine = base.StartCoroutine(this.CoPlaySkillUnlocked());
			}
			else
			{
				UIElement element = this.Element;
				element.OnActive = (Action)Delegate.Combine(element.OnActive, new Action(delegate()
				{
					this._animCoroutine = base.StartCoroutine(this.CoPlaySkillUnlocked());
				}));
			}
		}
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x001BCBEC File Offset: 0x001BADEC
	private void CreateSkeletonStartCache(List<Slot> slots, GameObject root, int currentProfession)
	{
		bool flag = root == null;
		if (!flag)
		{
			slots.Clear();
			Skeleton skeleton = root.GetComponentInChildren<SkeletonGraphic>().Skeleton;
			foreach (int professionId in this.AllProfessions)
			{
				int index = professionId + 1;
				bool isCurrent = professionId == currentProfession;
				Slot icon = isCurrent ? skeleton.FindSlot(string.Format("icon_{0}_light", index)) : skeleton.FindSlot(string.Format("icon_{0}_dark", index));
				Slot block = isCurrent ? skeleton.FindSlot(string.Format("blockicon_{0}_light", index)) : skeleton.FindSlot(string.Format("blockicon_{0}_dark", index));
				slots.Add(icon);
				slots.Add(block);
			}
		}
	}

	// Token: 0x0600373D RID: 14141 RVA: 0x001BCCEC File Offset: 0x001BAEEC
	private void SyncSkeletonValues(List<Slot> slots, float alpha, Func<Slot, bool> filter = null, float optAlpha = -1f)
	{
		foreach (Slot slot in slots)
		{
			bool flag = filter == null || filter(slot);
			if (flag)
			{
				slot.A = alpha;
			}
			else
			{
				bool flag2 = optAlpha >= 0f;
				if (flag2)
				{
					slot.A = Mathf.Min(optAlpha, slot.A);
				}
			}
		}
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x001BCD78 File Offset: 0x001BAF78
	private IEnumerator CoPlaySkillUnlocked()
	{
		AudioManager.Instance.PlaySound("animation_occupation_deblocking", false, false);
		this._step0 = Object.Instantiate<GameObject>(this.step0Prefab, this.animationRoot);
		this.backgroundCanvasGroup.DOFade(1f, 1f);
		yield return new WaitForSeconds(0.3f);
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.Playing);
		yield return new WaitForSeconds(0.7f);
		ProfessionSkillItem config = ProfessionSkill.Instance[this._currTemplateId];
		this._lastProfessionName = Profession.Instance[config.Profession].Name;
		List<Slot> slots = this.AnimStep1(0.5f);
		GameObject step = Object.Instantiate<GameObject>(this.step1Prefab, this.animationRoot);
		step.transform.eulerAngles = new Vector3(0f, 0f, (float)(-10 + -20 * config.Profession));
		GameObject stepEx0 = Object.Instantiate<GameObject>((config.Profession % 2 == 0) ? this.stepEx00Prefab : this.stepEx10Prefab, this.animationRoot);
		UI_ProfessionSkillUnlocked.<CoPlaySkillUnlocked>g__SyncAngles|55_0(stepEx0, config.Profession);
		yield return new WaitForSeconds(0.3f);
		this.AnimStep2(1f);
		yield return new WaitForSeconds(1f);
		this.AnimStep3(0.5f);
		yield return new WaitForSeconds(0.5f);
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.Finish);
		EasyPool.Free<List<Slot>>(slots);
		yield break;
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x001BCD87 File Offset: 0x001BAF87
	private IEnumerator CoPlayExtraSkillUnlocked()
	{
		UI_ProfessionSkillUnlocked._needPlayExtraProfessionSkillUnlockedAnim = false;
		this._playingExtraProfessionSkillUnlockedAnim = true;
		GameObject anim = Object.Instantiate<GameObject>(this.extraProfessionSkillUnlockAnimPrefab, this.animationRoot);
		string taiwuName = "";
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			CharacterDisplayData data = null;
			Serializer.Deserialize(dataPool, offset, ref data);
			taiwuName = NameCenter.GetNameByDisplayData(data, true, false);
		});
		this.professionText.gameObject.SetActive(false);
		this.skillUnlockDescText.DOFade(0f, 0.5f);
		this.backgroundCanvasGroup.DOFade(0f, 0.5f).OnComplete(delegate
		{
			this.iconImage.SetSprite("profession_skillicon_special_0", false, null);
			this.nameText.text = LocalStringManager.Get(LanguageKey.LK_ExtraProfessionSkill_Title).SetColor(CommonUtils.GetSkillNameColorByIndex(3));
			this.skillUnlockDescText.text = LocalStringManager.GetFormat(LanguageKey.LK_ExtraProfessionSkill_UnlockDesc, new object[]
			{
				taiwuName,
				this._lastProfessionName,
				taiwuName,
				taiwuName
			}).ColorReplace();
			this.backgroundCanvasGroup.DOFade(1f, 0.5f);
			this.skillUnlockDescText.DOFade(1f, 0.5f);
		});
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.Playing);
		yield return new WaitForSeconds(0.5f);
		GameObject loop = anim.transform.Find("Step1").gameObject;
		ParticleSystemAlphaController controller = loop.GetComponent<ParticleSystemAlphaController>();
		controller.Init();
		controller.SetActiveNoTween(false);
		loop.SetActive(true);
		loop.GetComponent<ParticleSystem>().Play();
		controller.SetActiveWithTween(true, 0.5f, Ease.InOutQuad);
		yield return new WaitForSeconds(0.5f);
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.Finish);
		this._playingExtraProfessionSkillUnlockedAnim = false;
		yield break;
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x001BCD96 File Offset: 0x001BAF96
	private IEnumerator SkipPlaySkillUnlocked()
	{
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.Skiping);
		base.StopCoroutine(this._animCoroutine);
		this.backgroundCanvasGroup.DOFade(1f, 0.01f);
		List<Slot> slots = this.AnimStep1(0f);
		this.AnimStep2(0f);
		this.AnimStep3(0f);
		yield return new WaitForSeconds(0.5f);
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.Finish);
		EasyPool.Free<List<Slot>>(slots);
		yield break;
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x001BCDA8 File Offset: 0x001BAFA8
	private List<Slot> AnimStep1(float duration)
	{
		List<Slot> slots = EasyPool.Get<List<Slot>>();
		bool animStep1AlreadyPlayed = this._animStep1AlreadyPlayed;
		List<Slot> slots2;
		if (animStep1AlreadyPlayed)
		{
			slots2 = slots;
		}
		else
		{
			ProfessionSkillItem config = ProfessionSkill.Instance[this._currTemplateId];
			this.CreateSkeletonStartCache(slots, this._step0, config.Profession);
			DOVirtual.Float(0f, 1f, duration, delegate(float stepValue)
			{
				this.SyncSkeletonValues(slots, stepValue, null, -1f);
			});
			this._animStep1AlreadyPlayed = true;
			slots2 = slots;
		}
		return slots2;
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x001BCE3C File Offset: 0x001BB03C
	private void AnimStep2(float duration)
	{
		bool animStep2AlreadyPlayed = this._animStep2AlreadyPlayed;
		if (!animStep2AlreadyPlayed)
		{
			Object.Instantiate<GameObject>(this.step2Prefab, this.animationRoot);
			this._animStep2AlreadyPlayed = true;
			this.unlockedCanvasGroup.DOFade(1f, duration);
			this.lockedCanvasGroup.DOFade(0f, duration);
		}
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x001BCE94 File Offset: 0x001BB094
	private void AnimStep3(float duration)
	{
		this.circleRoot.DOLocalMoveX(-365f, duration, false).SetEase(Ease.OutQuint);
		this.descRoot.DOLocalMoveX(365f, duration, false).SetEase(Ease.OutQuint);
		this.descRootCanvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuint);
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x001BCEF0 File Offset: 0x001BB0F0
	private void ResetAnim()
	{
		this.HandleWaitingToDestroyed();
		this.SetAnimState(UI_ProfessionSkillUnlocked.AnimState.BeforeStart);
		this.unlockedCanvasGroup.alpha = 0f;
		this.lockedCanvasGroup.alpha = 1f;
		this.backgroundCanvasGroup.alpha = 0f;
		Vector3 circlePos = this.circleRoot.localPosition;
		circlePos.x = 0f;
		this.circleRoot.localPosition = circlePos;
		Vector3 descPos = this.descRoot.localPosition;
		descPos.x = 0f;
		this.descRoot.localPosition = descPos;
		this.descRootCanvasGroup.alpha = 0f;
		this._animStep2AlreadyPlayed = false;
		this._animStep1AlreadyPlayed = false;
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x001BCFAC File Offset: 0x001BB1AC
	private void SetSkill()
	{
		ProfessionSkillItem config = ProfessionSkill.Instance[this._currTemplateId];
		bool flag = config == null;
		if (flag)
		{
			throw new Exception(string.Format("templateId {0} not exist in professionSkill", this._currTemplateId));
		}
		ProfessionItem profession = Profession.Instance[config.Profession];
		this.iconImage.SetSprite(config.BigIcon, false, null);
		this.professionText.text = profession.Name;
		this.professionText.gameObject.SetActive(true);
		int level = (this._currTemplateId == profession.ExtraProfessionSkill) ? 3 : profession.ProfessionSkills.IndexOf(this._currTemplateId);
		this.nameText.text = config.Name;
		this.nameText.color = CommonUtils.GetSkillNameColorByIndex(level);
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			CharacterDisplayData data = null;
			Serializer.Deserialize(dataPool, offset, ref data);
			string taiwuName = NameCenter.GetNameByDisplayData(data, true, false);
			string desc = config.SkillUnlockDesc.GetFormat(taiwuName, taiwuName, taiwuName);
			string explain = config.SkillUnlockExplain.GetFormat(taiwuName, taiwuName, taiwuName);
			this.skillUnlockDescText.SetText(desc.SetColor("pinkyellow") + "\n \n" + explain.SetColor("brightblue"), true);
		});
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x001BD0C1 File Offset: 0x001BB2C1
	public static void ProfessionSkillUnlockedEnqueue(int skillTemplateId)
	{
		UI_ProfessionSkillUnlocked.ProfessionSkillUnlockedQueue.Enqueue(skillTemplateId);
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x001BD0D0 File Offset: 0x001BB2D0
	public static bool HasProfessionSkillUnlockedToShow()
	{
		return UI_ProfessionSkillUnlocked.ProfessionSkillUnlockedQueue.Count > 0;
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x001BD0EF File Offset: 0x001BB2EF
	public static void ClearProfessionSkillUnlockedQueue()
	{
		UI_ProfessionSkillUnlocked.ProfessionSkillUnlockedQueue.Clear();
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x001BD0FD File Offset: 0x001BB2FD
	public static void ExtraProfessionSkillUnlockedEnqueue()
	{
		UI_ProfessionSkillUnlocked._needPlayExtraProfessionSkillUnlockedAnim = true;
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x001BD106 File Offset: 0x001BB306
	private void SetAnimState(UI_ProfessionSkillUnlocked.AnimState animState)
	{
		this._animState = animState;
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x001BD148 File Offset: 0x001BB348
	[CompilerGenerated]
	internal static void <CoPlaySkillUnlocked>g__SyncAngles|55_0(GameObject o, int profession)
	{
		o.transform.eulerAngles = ((profession % 2 == 0) ? new Vector3(0f, 0f, (float)(-20 * profession)) : new Vector3(0f, 0f, (float)(-20 * profession - 20)));
	}

	// Token: 0x040027DF RID: 10207
	private static readonly ConcurrentQueue<int> ProfessionSkillUnlockedQueue = new ConcurrentQueue<int>();

	// Token: 0x040027E0 RID: 10208
	[SerializeField]
	private RectTransform animationRoot;

	// Token: 0x040027E1 RID: 10209
	[SerializeField]
	private GameObject step0Prefab;

	// Token: 0x040027E2 RID: 10210
	[SerializeField]
	private GameObject step1Prefab;

	// Token: 0x040027E3 RID: 10211
	[SerializeField]
	private GameObject step2Prefab;

	// Token: 0x040027E4 RID: 10212
	[SerializeField]
	private GameObject stepEx00Prefab;

	// Token: 0x040027E5 RID: 10213
	[SerializeField]
	private GameObject stepEx10Prefab;

	// Token: 0x040027E6 RID: 10214
	[SerializeField]
	private GameObject extraProfessionSkillUnlockAnimPrefab;

	// Token: 0x040027E7 RID: 10215
	[SerializeField]
	private CanvasGroup backgroundCanvasGroup;

	// Token: 0x040027E8 RID: 10216
	[SerializeField]
	private CanvasGroup unlockedCanvasGroup;

	// Token: 0x040027E9 RID: 10217
	[SerializeField]
	private CanvasGroup lockedCanvasGroup;

	// Token: 0x040027EA RID: 10218
	[SerializeField]
	private RectTransform circleRoot;

	// Token: 0x040027EB RID: 10219
	[SerializeField]
	private RectTransform descRoot;

	// Token: 0x040027EC RID: 10220
	[SerializeField]
	private CanvasGroup descRootCanvasGroup;

	// Token: 0x040027ED RID: 10221
	[SerializeField]
	private CImage iconImage;

	// Token: 0x040027EE RID: 10222
	[SerializeField]
	private TextMeshProUGUI professionText;

	// Token: 0x040027EF RID: 10223
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x040027F0 RID: 10224
	[SerializeField]
	private TextMeshProUGUI skillUnlockDescText;

	// Token: 0x040027F1 RID: 10225
	[SerializeField]
	private GameObject skipTips;

	// Token: 0x040027F2 RID: 10226
	[SerializeField]
	private GameObject closeTips;

	// Token: 0x040027F3 RID: 10227
	[SerializeField]
	private CButton closeBtn;

	// Token: 0x040027F4 RID: 10228
	private const int ExtraSkillIndex = 3;

	// Token: 0x040027F5 RID: 10229
	private const string UnlockSeName = "animation_occupation_deblocking";

	// Token: 0x040027F6 RID: 10230
	private static bool _needPlayExtraProfessionSkillUnlockedAnim = false;

	// Token: 0x040027F7 RID: 10231
	private bool _playingExtraProfessionSkillUnlockedAnim = false;

	// Token: 0x040027F8 RID: 10232
	private string _lastProfessionName;

	// Token: 0x040027F9 RID: 10233
	private const float Step0Delta = 1f;

	// Token: 0x040027FA RID: 10234
	private const float Step1Delta = 0.3f;

	// Token: 0x040027FB RID: 10235
	private const float Step2Delta = 1f;

	// Token: 0x040027FC RID: 10236
	private const float AnimStart = 0.5f;

	// Token: 0x040027FD RID: 10237
	private const float ExtraProfessionSkillFadeTime = 0.5f;

	// Token: 0x040027FE RID: 10238
	private const float DisableSkipTime = 0.3f;

	// Token: 0x040027FF RID: 10239
	private const float DisableCloseTime = 0.5f;

	// Token: 0x04002800 RID: 10240
	private const float AnimSkipTime = 0.01f;

	// Token: 0x04002801 RID: 10241
	private GameObject _step0;

	// Token: 0x04002802 RID: 10242
	private bool _animStep1AlreadyPlayed;

	// Token: 0x04002803 RID: 10243
	private bool _animStep2AlreadyPlayed;

	// Token: 0x04002804 RID: 10244
	private int _currTemplateId;

	// Token: 0x04002805 RID: 10245
	private const float PosXMoveDistance = 365f;

	// Token: 0x04002806 RID: 10246
	private UI_ProfessionSkillUnlocked.AnimState _animState;

	// Token: 0x04002807 RID: 10247
	private Coroutine _animCoroutine;

	// Token: 0x020017D3 RID: 6099
	private enum AnimState
	{
		// Token: 0x0400ACBE RID: 44222
		BeforeStart,
		// Token: 0x0400ACBF RID: 44223
		Playing,
		// Token: 0x0400ACC0 RID: 44224
		Skiping,
		// Token: 0x0400ACC1 RID: 44225
		Finish
	}
}
