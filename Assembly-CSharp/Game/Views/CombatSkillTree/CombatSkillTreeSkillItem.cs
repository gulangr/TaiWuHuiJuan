using System;
using System.IO;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Game.Views.CombatSkillTree
{
	// Token: 0x02000AE5 RID: 2789
	public class CombatSkillTreeSkillItem : MonoBehaviour
	{
		// Token: 0x0600890C RID: 35084 RVA: 0x003F6C48 File Offset: 0x003F4E48
		private void Awake()
		{
			PointerTrigger videoPointerTrigger = this.videoGo.GetComponent<PointerTrigger>();
			videoPointerTrigger.EnterEvent.RemoveAllListeners();
			videoPointerTrigger.EnterEvent.AddListener(new UnityAction(this.StartPlayVideo));
			videoPointerTrigger.ExitEvent.RemoveAllListeners();
			videoPointerTrigger.ExitEvent.AddListener(new UnityAction(this.StopPlayVideo));
		}

		// Token: 0x0600890D RID: 35085 RVA: 0x003F6CAC File Offset: 0x003F4EAC
		private void OnDestroy()
		{
			bool flag = this._renderTexture != null;
			if (flag)
			{
				Object.Destroy(this._renderTexture);
			}
			this._renderTexture = null;
		}

		// Token: 0x0600890E RID: 35086 RVA: 0x003F6CE0 File Offset: 0x003F4EE0
		public void Set(bool isVisible, CombatSkillItem combatSkillItem, CombatSkillDisplayData combatSkillDisplayData)
		{
			this.unLockRoot.gameObject.SetActive(isVisible);
			this.lockMask.gameObject.SetActive(!isVisible);
			this.mouseTipDisplayer.enabled = isVisible;
			Color gradeColor = Colors.Instance.GradeColors[(int)combatSkillItem.Grade];
			if (isVisible)
			{
				this._combatSkillItem = combatSkillItem;
				this.SetSkillName(combatSkillItem.Name.SetColor(gradeColor));
				TextMeshProUGUI textMeshProUGUI = this.skillName;
				int length = combatSkillItem.Name.Length;
				if (!true)
				{
				}
				int num;
				if (length > 5)
				{
					if (length != 6)
					{
						num = 18;
					}
					else
					{
						num = 22;
					}
				}
				else
				{
					num = 24;
				}
				if (!true)
				{
				}
				textMeshProUGUI.fontSize = (float)num;
				this.icon.SetSprite(combatSkillItem.Icon, false, null);
				this.icon.SetColor(CommonUtils.GetFiveElementColor((int)combatSkillItem.FiveElements));
				this.frame.SetSprite(CombatSkillTreeSkillItem.FramePaths[(int)combatSkillItem.EquipType] + combatSkillItem.Grade.ToString(), false, null);
				bool isLearned = combatSkillDisplayData != null;
				this.status.gameObject.SetActive(isLearned);
				ArgumentBox args = EasyPool.Get<ArgumentBox>();
				args.Set("CombatSkillId", combatSkillItem.TemplateId);
				int charId = -1;
				bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
				if (flag)
				{
					charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				}
				args.Set("CharId", charId);
				args.Set("PracticeLevel", -1);
				args.Set("ShowOnlyTemplateInfo", true);
				this.mouseTipDisplayer.RuntimeParam = args;
				this.effectDirection.gameObject.SetActive(combatSkillDisplayData != null && combatSkillDisplayData.BreakSuccess);
				this.fiveElementsFrame.gameObject.SetActive(false);
				bool flag2 = isLearned;
				if (flag2)
				{
					bool breakSuccess = combatSkillDisplayData.BreakSuccess;
					if (breakSuccess)
					{
						this.status.text = LanguageKey.LK_CombatSkillTree_SkillMax2.Tr();
						bool direction = CombatSkillStateHelper.GetCombatSkillDirection(combatSkillDisplayData.ActivationState) == 0;
						this.effectDirection.SetSpriteOnly("ui9_icon_combat_skill_effect_direction_" + (direction ? "0" : "1"), false, null);
					}
					else
					{
						this.status.text = LanguageKey.LK_CombatSkillTree_SkillLearned2.Tr();
					}
				}
				sbyte equipType = combatSkillItem.EquipType;
				bool isNeedVideo = equipType == 1 || equipType == 3;
				this.videoGo.SetActive(isNeedVideo);
				bool flag3 = isNeedVideo;
				if (flag3)
				{
					this.LoadVideo();
				}
			}
			else
			{
				foreach (TextMeshProUGUI item in this.lockLabels)
				{
					item.text = item.text.SetColor(gradeColor);
				}
			}
		}

		// Token: 0x0600890F RID: 35087 RVA: 0x003F6FA4 File Offset: 0x003F51A4
		private void SetSkillName(string text)
		{
			this.skillName.text = text;
			this.skillName.text.ColorReplace();
			TextMeshProUGUI textMeshProUGUI = this.skillName;
			int num2;
			if (LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN)
			{
				int length = text.Length;
				if (!true)
				{
				}
				int num;
				if (length > 5)
				{
					if (length != 6)
					{
						num = 18;
					}
					else
					{
						num = 22;
					}
				}
				else
				{
					num = 24;
				}
				if (!true)
				{
				}
				num2 = num;
			}
			else
			{
				num2 = 20;
			}
			textMeshProUGUI.fontSize = (float)num2;
			this.skillName.ForceMeshUpdate(false, true);
			this.skillName.overflowMode = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis);
			LanguageRuleTips tips = this.skillName.GetComponent<LanguageRuleTips>();
			tips.OnLanguageChange(LocalStringManager.CurLanguageType);
			tips.SetTipActive(LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN);
		}

		// Token: 0x06008910 RID: 35088 RVA: 0x003F7068 File Offset: 0x003F5268
		private void StartPlayVideo()
		{
			bool hasVideo = this.videoPlayer.clip != null;
			this.videoContent.SetActive(hasVideo);
			this.videoEmpty.SetActive(!hasVideo);
			this.videoPopup.SetActive(true);
			RectTransform videoPopupRect = this.videoPopup.GetComponent<RectTransform>();
			Vector3[] tipCorners = new Vector3[4];
			videoPopupRect.GetWorldCorners(tipCorners);
			Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(tipCorners[0]);
			float anchoredPosY = (itemScreenPos.y < 0f) ? (videoPopupRect.rect.height - this.videoGo.GetComponent<RectTransform>().rect.height) : 0f;
			videoPopupRect.anchoredPosition = new Vector2(videoPopupRect.anchoredPosition.x, anchoredPosY);
			bool flag = !hasVideo;
			if (!flag)
			{
				this.videoPlayer.SetDirectAudioVolume(0, (float)SingletonObject.getInstance<GlobalSettings>().VideoVolume / 100f);
				this.videoPlayer.Play();
			}
		}

		// Token: 0x06008911 RID: 35089 RVA: 0x003F7174 File Offset: 0x003F5374
		private void LoadVideo()
		{
			this.videoPlayer.clip = null;
			bool flag = this._combatSkillItem == null;
			if (!flag)
			{
				string videoName = this._combatSkillItem.VideoName;
				bool flag2 = videoName.IsNullOrEmpty();
				if (!flag2)
				{
					string videoPath = Path.Combine("CombatSkillVideos/", videoName).PathFix();
					VideoClip clip = Resources.Load<VideoClip>(videoPath);
					bool flag3 = clip == null;
					if (!flag3)
					{
						bool flag4 = this._renderTexture != null;
						if (flag4)
						{
							Object.DestroyImmediate(this._renderTexture);
							this._renderTexture = null;
						}
						this._renderTexture = new RenderTexture((int)clip.width, (int)clip.height, 0);
						this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
						this.videoPlayer.targetTexture = this._renderTexture;
						this.videoPlayer.isLooping = true;
						this.videoPlayer.clip = clip;
						this.videoRawImage.texture = this._renderTexture;
					}
				}
			}
		}

		// Token: 0x06008912 RID: 35090 RVA: 0x003F7274 File Offset: 0x003F5474
		private void StopPlayVideo()
		{
			this.videoPlayer.Stop();
			this.videoPopup.SetActive(false);
			RectTransform videoPopupRect = this.videoPopup.GetComponent<RectTransform>();
			videoPopupRect.anchoredPosition = new Vector2(videoPopupRect.anchoredPosition.x, 0f);
		}

		// Token: 0x040068F8 RID: 26872
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x040068F9 RID: 26873
		[SerializeField]
		private RectTransform unLockRoot;

		// Token: 0x040068FA RID: 26874
		[SerializeField]
		private CImage frame;

		// Token: 0x040068FB RID: 26875
		[SerializeField]
		private CImage icon;

		// Token: 0x040068FC RID: 26876
		[SerializeField]
		private CImage effectDirection;

		// Token: 0x040068FD RID: 26877
		[SerializeField]
		private TextMeshProUGUI status;

		// Token: 0x040068FE RID: 26878
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x040068FF RID: 26879
		[SerializeField]
		private RectTransform lockMask;

		// Token: 0x04006900 RID: 26880
		[SerializeField]
		private TextMeshProUGUI[] lockLabels;

		// Token: 0x04006901 RID: 26881
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x04006902 RID: 26882
		[SerializeField]
		private GameObject videoGo;

		// Token: 0x04006903 RID: 26883
		[SerializeField]
		private GameObject videoPopup;

		// Token: 0x04006904 RID: 26884
		[SerializeField]
		private GameObject videoContent;

		// Token: 0x04006905 RID: 26885
		[SerializeField]
		private GameObject videoEmpty;

		// Token: 0x04006906 RID: 26886
		[SerializeField]
		private VideoPlayer videoPlayer;

		// Token: 0x04006907 RID: 26887
		[SerializeField]
		private CRawImage videoRawImage;

		// Token: 0x04006908 RID: 26888
		private CombatSkillItem _combatSkillItem;

		// Token: 0x04006909 RID: 26889
		private RenderTexture _renderTexture;

		// Token: 0x0400690A RID: 26890
		private const string VideoPath = "CombatSkillVideos/";

		// Token: 0x0400690B RID: 26891
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_tree_type_neigong_",
			"ui9_icon_combat_skill_tree_type_attack_",
			"ui9_icon_combat_skill_tree_type_agile_",
			"ui9_icon_combat_skill_tree_type_defense_",
			"ui9_icon_combat_skill_tree_type_assist_"
		};
	}
}
