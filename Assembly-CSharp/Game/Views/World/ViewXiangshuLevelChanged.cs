using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia;
using Game.Views.Migrate;
using GameData.Domains.Global;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x02000732 RID: 1842
	public class ViewXiangshuLevelChanged : UIBase
	{
		// Token: 0x06005875 RID: 22645 RVA: 0x00290E34 File Offset: 0x0028F034
		private void Update()
		{
			bool keyUp = Input.GetKeyUp(KeyCode.Space);
			if (keyUp)
			{
				bool interactable = this.buttonGlobal.interactable;
				if (interactable)
				{
					this.QuickHide();
				}
			}
		}

		// Token: 0x06005876 RID: 22646 RVA: 0x00290E68 File Offset: 0x0028F068
		public override void OnInit(ArgumentBox argsBox)
		{
			ViewXiangshuLevelChanged.<>c__DisplayClass25_0 CS$<>8__locals1 = new ViewXiangshuLevelChanged.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = argsBox == null;
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				argsBox.Get("XiangshuLevelNew", out CS$<>8__locals1.xiangshuLevelNew);
				argsBox.Get("XiangshuLevelOld", out CS$<>8__locals1.xiangshuLevelOld);
				argsBox.Get("ConsummateLevelNew", out this.consummateLevelNew);
				argsBox.Get("ConsummateLevelOld", out this.consummateLevelOld);
				this.title.text = ViewXiangshuLevelChanged.<OnInit>g__RenderTitle|25_4(CS$<>8__locals1.xiangshuLevelNew);
				this.desc.text = ViewXiangshuLevelChanged.<OnInit>g__RenderDesc|25_5(CS$<>8__locals1.xiangshuLevelNew);
				Transform panel = this.cornerLeftPanel;
				ViewXiangshuLevelChanged.<OnInit>g__SetupDirect|25_2(panel.GetChild(0), LanguageKey.LK_Consummate_Level.Tr(), this.consummateLevelOld.ToString(), this.consummateLevelNew.ToString());
				bool flag2 = CS$<>8__locals1.xiangshuLevelNew == CS$<>8__locals1.xiangshuLevelOld;
				if (flag2)
				{
					for (int i = 1; i < 4; i++)
					{
						panel.GetChild(i).gameObject.SetActive(false);
					}
				}
				else
				{
					CS$<>8__locals1.<OnInit>g__Setup|1(panel.GetChild(1), LanguageKey.LK_CombatSkillTree_OrganizationApproveLimitBase.Tr(), delegate(sbyte level)
					{
						short[] arr = GlobalConfig.Instance.SectApprovingRateUpperLimits;
						return (arr.CheckIndex((int)level) ? string.Format("{0}%", arr[(int)level]) : string.Empty).SetColor("lightblue");
					});
					CS$<>8__locals1.<OnInit>g__Setup|1(panel.GetChild(2), LanguageKey.LK_AbleFallenClassLevel.Tr(), new Func<sbyte, string>(ViewXiangshuLevelChanged.<OnInit>g__RenderOrgGrade|25_3));
					CS$<>8__locals1.<OnInit>g__Setup|1(panel.GetChild(3), LanguageKey.LK_AbleJoinUsClassLevel.Tr(), new Func<sbyte, string>(ViewXiangshuLevelChanged.<OnInit>g__RenderOrgGrade|25_3));
				}
				for (int j = 0; j < this.containerFence.childCount; j++)
				{
					Transform child = this.containerFence.GetChild(j);
					child.GetChild(0).gameObject.SetActive(j <= (int)CS$<>8__locals1.xiangshuLevelOld);
				}
				this.progressValue.fillAmount = 1f * (float)this.consummateLevelOld / (float)(ConsummateLevel.Instance.Count - 1);
				this.buttonGlobal.ClearAndAddListener(new Action(this.QuickHide));
				this.buttonEncyclopedia.ClearAndAddListener(delegate
				{
					EncyclopediaTipLinkItem config = EncyclopediaTipLink.Instance[76];
					ViewEncyclopediaPanel.OpenLink(config);
				});
				this.PrepareAnim();
				UIElement element = this.Element;
				element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
				{
					bool activeInHierarchy = CS$<>8__locals1.<>4__this.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						CS$<>8__locals1.<>4__this._coroutine = CS$<>8__locals1.<>4__this.StartCoroutine(CS$<>8__locals1.<>4__this.StartAnim(CS$<>8__locals1.xiangshuLevelNew));
					}
				}));
			}
		}

		// Token: 0x06005877 RID: 22647 RVA: 0x002910F4 File Offset: 0x0028F2F4
		private void OnDisable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(244);
			bool flag = SingletonObject.getInstance<WorldMapModel>().RemoveSwordTombRootMapBlock != null;
			if (flag)
			{
				GEvent.OnEvent(UiEvents.DefeatSwordTomb, null);
			}
			bool flag2 = this._coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._coroutine);
			}
			this._coroutine = null;
			this.StopAllAudio();
		}

		// Token: 0x06005878 RID: 22648 RVA: 0x00291158 File Offset: 0x0028F358
		private void PrepareAnim()
		{
			this.uiMask.DOKill(false);
			this.content.DOKill(false);
			this.uiMask.color = Color.clear;
			this.content.alpha = 0f;
			for (int i = 0; i < this.xiangshuEffect.Length; i++)
			{
				this.xiangshuEffect[i].SetActive(false);
			}
		}

		// Token: 0x06005879 RID: 22649 RVA: 0x002911CA File Offset: 0x0028F3CA
		private IEnumerator StartAnim(sbyte xiangshuLevel)
		{
			AudioManager.Instance.SetMusicMute(true);
			this._currAmbienceVolume = AudioManager.Instance.GetAmbienceVolume();
			AudioManager.Instance.SetAmbienceVolume(0f);
			this.buttonGlobal.interactable = false;
			this.uiMask.DOColor(Color.black, 0.5f);
			yield return new WaitForSeconds(0.5f);
			bool flag = ViewXiangshuLevelChanged.xiangshuAudioName.CheckIndex((int)xiangshuLevel) && !ViewXiangshuLevelChanged.xiangshuAudioName[(int)xiangshuLevel].IsNullOrEmpty();
			if (flag)
			{
				AudioManager.Instance.PlaySound(ViewXiangshuLevelChanged.xiangshuAudioName[(int)xiangshuLevel], false, false);
				this._currAudioName = ViewXiangshuLevelChanged.xiangshuAudioName[(int)xiangshuLevel];
			}
			bool flag2 = ViewXiangshuLevelChanged.xiangshuAudioLoopName.CheckIndex((int)xiangshuLevel) && !ViewXiangshuLevelChanged.xiangshuAudioLoopName[(int)xiangshuLevel].IsNullOrEmpty();
			if (flag2)
			{
				string[] arr = ViewXiangshuLevelChanged.xiangshuAudioLoopName[(int)xiangshuLevel].Split("|", StringSplitOptions.None);
				int num;
				for (int i = 0; i < arr.Length; i = num + 1)
				{
					AudioManager.Instance.PlaySound(arr[i], true, false);
					this._currAudioLoopName.Add(arr[i]);
					num = i;
				}
				arr = null;
			}
			this._currSpineAnimIndex = (int)(this.xiangshuEffect.CheckIndex((int)xiangshuLevel) ? xiangshuLevel : 0);
			this.xiangshuEffect[this._currSpineAnimIndex].SetActive(true);
			bool flag3 = ViewXiangshuLevelChanged.xiangshuSpineAnimName.CheckIndex(this._currSpineAnimIndex) && !ViewXiangshuLevelChanged.xiangshuSpineAnimName[this._currSpineAnimIndex].IsNullOrEmpty();
			if (flag3)
			{
				bool flag4 = this.xiangshuSpine.CheckIndex(this._currSpineAnimIndex);
				if (flag4)
				{
					this.xiangshuSpine[this._currSpineAnimIndex].AnimationState.SetAnimation(0, ViewXiangshuLevelChanged.xiangshuSpineAnimName[this._currSpineAnimIndex], false);
					this.xiangshuSpine[this._currSpineAnimIndex].AnimationState.Complete -= this.AnimationStateOnEnd;
					this.xiangshuSpine[this._currSpineAnimIndex].AnimationState.Complete += this.AnimationStateOnEnd;
				}
			}
			this.uiMask.DOColor(Color.clear, 0.5f).SetAutoKill(true);
			yield return new WaitForSeconds(0.5f);
			this.content.DOFade(1f, 0.5f).SetAutoKill(true);
			yield return new WaitForSeconds(0.8f);
			float fillAmount = 1f * (float)this.consummateLevelNew / (float)(ConsummateLevel.Instance.Count - 1);
			this.progressValue.DOFillAmount(fillAmount, 0.2f).OnComplete(delegate
			{
				for (int j = 0; j < this.containerFence.childCount; j++)
				{
					Transform child = this.containerFence.GetChild(j);
					child.GetChild(0).gameObject.SetActive(j <= (int)xiangshuLevel);
				}
			}).SetAutoKill(true);
			yield return new WaitForSeconds(0.5f);
			this.buttonGlobal.interactable = true;
			yield break;
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x002911E0 File Offset: 0x0028F3E0
		private void AnimationStateOnEnd(TrackEntry trackEntry)
		{
			bool flag = !ViewXiangshuLevelChanged.xiangshuSpineAnimName.CheckIndex(this._currSpineAnimIndex) || ViewXiangshuLevelChanged.xiangshuSpineAnimName[this._currSpineAnimIndex].IsNullOrEmpty();
			if (!flag)
			{
				string loopAnimName = ViewXiangshuLevelChanged.xiangshuSpineAnimName[this._currSpineAnimIndex] + "_loop";
				this.xiangshuSpine[this._currSpineAnimIndex].AnimationState.SetAnimation(0, loopAnimName, true);
				this.xiangshuSpine[this._currSpineAnimIndex].AnimationState.Complete -= this.AnimationStateOnEnd;
			}
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x00291270 File Offset: 0x0028F470
		private void StopAllAudio()
		{
			bool flag = !string.IsNullOrEmpty(this._currAudioName);
			if (flag)
			{
				AudioManager.Instance.StopSound(this._currAudioName);
			}
			for (int i = 0; i < this._currAudioLoopName.Count; i++)
			{
				AudioManager.Instance.StopSound(this._currAudioLoopName[i]);
			}
			this._currAudioName = "";
			this._currAudioLoopName.Clear();
			AudioManager.Instance.SetMusicMute(!SingletonObject.getInstance<GlobalSettings>().BgmOn);
			AudioManager.Instance.SetAmbienceVolume(this._currAmbienceVolume);
		}

		// Token: 0x0600587E RID: 22654 RVA: 0x00291464 File Offset: 0x0028F664
		[CompilerGenerated]
		internal static void <OnInit>g__SetupDirect|25_2(Transform tF, string title, string before, string after)
		{
			tF.gameObject.SetActive(true);
			XiangShuLevelChangedParameter refer = tF.GetComponent<XiangShuLevelChangedParameter>();
			refer.title.text = title;
			TMPTextSpriteHelper helper = refer.value.gameObject.GetOrAddComponent<TMPTextSpriteHelper>();
			helper.SpriteSizeFitType = TMPTextSpriteHelper.SizeFitType.Static;
			helper.StaticSpriteSize = new Vector2(44f, 32f);
			refer.value.text = before + " <SpName=ui9_icon_change_arrow_0> " + after;
			helper.Parse();
		}

		// Token: 0x0600587F RID: 22655 RVA: 0x002914E0 File Offset: 0x0028F6E0
		[CompilerGenerated]
		internal static string <OnInit>g__RenderOrgGrade|25_3(sbyte xiangshuLevel)
		{
			if (!true)
			{
			}
			string str;
			switch (xiangshuLevel)
			{
			case 0:
				str = LanguageKey.LK_OrgGrade_Short_0.Tr();
				break;
			case 1:
				str = LanguageKey.LK_OrgGrade_Short_1.Tr();
				break;
			case 2:
				str = LanguageKey.LK_OrgGrade_Short_2.Tr();
				break;
			case 3:
				str = LanguageKey.LK_OrgGrade_Short_3.Tr();
				break;
			case 4:
				str = LanguageKey.LK_OrgGrade_Short_4.Tr();
				break;
			case 5:
				str = LanguageKey.LK_OrgGrade_Short_5.Tr();
				break;
			case 6:
				str = LanguageKey.LK_OrgGrade_Short_6.Tr();
				break;
			case 7:
				str = LanguageKey.LK_OrgGrade_Short_7.Tr();
				break;
			default:
				str = LanguageKey.LK_OrgGrade_Short_8.Tr();
				break;
			}
			if (!true)
			{
			}
			return str.SetGradeColor((int)xiangshuLevel);
		}

		// Token: 0x06005880 RID: 22656 RVA: 0x0029159E File Offset: 0x0028F79E
		[CompilerGenerated]
		internal static string <OnInit>g__RenderTitle|25_4(sbyte xiangshuLevel)
		{
			return WorldState.Instance[ViewXiangshuLevelChanged.DataKey[Math.Clamp((int)(xiangshuLevel - 1), 0, ViewXiangshuLevelChanged.DataKey.Length - 1)]].Name;
		}

		// Token: 0x06005881 RID: 22657 RVA: 0x002915C7 File Offset: 0x0028F7C7
		[CompilerGenerated]
		internal static string <OnInit>g__RenderDesc|25_5(sbyte xiangshuLevel)
		{
			return WorldState.Instance[ViewXiangshuLevelChanged.DataKey[Math.Clamp((int)(xiangshuLevel - 1), 0, ViewXiangshuLevelChanged.DataKey.Length - 1)]].Desc.Split("\n\n", StringSplitOptions.None)[0];
		}

		// Token: 0x04003CC5 RID: 15557
		[SerializeField]
		private CRawImage bigPictureBackGround;

		// Token: 0x04003CC6 RID: 15558
		[SerializeField]
		private List<Texture> bigPictureBackGroundImages;

		// Token: 0x04003CC7 RID: 15559
		[SerializeField]
		private Transform cornerLeftPanel;

		// Token: 0x04003CC8 RID: 15560
		[SerializeField]
		private CButton buttonEncyclopedia;

		// Token: 0x04003CC9 RID: 15561
		[SerializeField]
		private CButton buttonGlobal;

		// Token: 0x04003CCA RID: 15562
		[SerializeField]
		private RectTransform containerFence;

		// Token: 0x04003CCB RID: 15563
		[SerializeField]
		private CImage progressValue;

		// Token: 0x04003CCC RID: 15564
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04003CCD RID: 15565
		[SerializeField]
		private TMP_Text desc;

		// Token: 0x04003CCE RID: 15566
		[SerializeField]
		private CRawImage uiMask;

		// Token: 0x04003CCF RID: 15567
		[SerializeField]
		private CanvasGroup content;

		// Token: 0x04003CD0 RID: 15568
		[SerializeField]
		private GameObject[] xiangshuEffect;

		// Token: 0x04003CD1 RID: 15569
		[SerializeField]
		private SkeletonGraphic[] xiangshuSpine;

		// Token: 0x04003CD2 RID: 15570
		private static string[] xiangshuSpineAnimName = new string[]
		{
			"",
			"CrisisAlert_step1_2",
			"CrisisAlert_step1_3",
			"CrisisAlert_step2_1",
			"CrisisAlert_step2_2",
			"CrisisAlert_step2_3",
			"CrisisAlert_step3_1",
			"CrisisAlert_step3_2",
			"CrisisAlert_step3_3",
			""
		};

		// Token: 0x04003CD3 RID: 15571
		private static string[] xiangshuAudioName = new string[]
		{
			"",
			"XiangShu0-2",
			"XiangShu2-4",
			"XiangShu4-6",
			"XiangShu6-8",
			"XiangShu8-10",
			"XiangShu10-12",
			"XiangShu12-14",
			"XiangShu14-16",
			""
		};

		// Token: 0x04003CD4 RID: 15572
		private static string[] xiangshuAudioLoopName = new string[]
		{
			"",
			"XiangShu_WindLoop",
			"XiangShu_WindSandLoop",
			"XiangShu_WindSandLoop",
			"XiangShu_ColdWindLoop",
			"XiangShu_WindFireLoop",
			"XiangShu_WindFireLoop|XiangShu_ThunderLoop",
			"XiangShu_WindFireLoop|XiangShu_ThunderLoop",
			"XiangShu_WindFireLoop|XiangShu_ThunderLoop",
			"XiangShu_WindFireLoop|XiangShu_ThunderLoop"
		};

		// Token: 0x04003CD5 RID: 15573
		private sbyte consummateLevelNew;

		// Token: 0x04003CD6 RID: 15574
		private sbyte consummateLevelOld;

		// Token: 0x04003CD7 RID: 15575
		private Coroutine _coroutine;

		// Token: 0x04003CD8 RID: 15576
		private int _currSpineAnimIndex;

		// Token: 0x04003CD9 RID: 15577
		private string _currAudioName;

		// Token: 0x04003CDA RID: 15578
		private List<string> _currAudioLoopName = new List<string>();

		// Token: 0x04003CDB RID: 15579
		private float _currAmbienceVolume;

		// Token: 0x04003CDC RID: 15580
		private static readonly sbyte[] DataKey = new sbyte[]
		{
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9
		};

		// Token: 0x02001BEB RID: 7147
		internal static class ArgumentId
		{
			// Token: 0x0400BF09 RID: 48905
			internal const string XiangshuLevelNew = "XiangshuLevelNew";

			// Token: 0x0400BF0A RID: 48906
			internal const string XiangshuLevelOld = "XiangshuLevelOld";

			// Token: 0x0400BF0B RID: 48907
			internal const string ConsummateLevelNew = "ConsummateLevelNew";

			// Token: 0x0400BF0C RID: 48908
			internal const string ConsummateLevelOld = "ConsummateLevelOld";
		}
	}
}
