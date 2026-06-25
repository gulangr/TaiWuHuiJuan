using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.World;
using GameData.Domains.Character.Creation;
using GameData.Domains.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Game.Views.NewGame
{
	// Token: 0x020007FE RID: 2046
	public class NewGameSubPageBornArea : NewGameSubPage
	{
		// Token: 0x060063D5 RID: 25557 RVA: 0x002DC294 File Offset: 0x002DA494
		protected override void Awake()
		{
			base.Awake();
			this.landFormTypeSelectionKeeper.enabled = false;
			this.stateMapSelectionKeeper.enabled = false;
			this.stateMap.OnStateTemplateIdSelected += this.OnStateTemplateIdSelected;
			this.stateMap.OnStateRefersUpdated += this.OnStateRefersUpdated;
			this.toggleGroupLandFormType.OnActiveIndexChange += this.OnLandFormTypeIndexSelected;
			this.videoPlayer.prepareCompleted += this.OnVideoPrepared;
			this.videoPlayer.errorReceived += this.OnVideoPlayError;
			this.videoPlayer.started += this.OnVideoPlayStart;
			GameObject gameObject = this.videoPlayer.gameObject;
			LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
			gameObject.SetActive(curLanguageType == LocalStringManager.LanguageType.CN || curLanguageType == LocalStringManager.LanguageType.CNH);
			this.closeVideo.ClearAndAddListener(new Action(this.CloseVideoPlay));
			for (int i = 0; i < this.toggleGroupLandFormType.Count(); i++)
			{
				CToggle t = this.toggleGroupLandFormType.Get(i);
				LandFormTypeItem landFormType = Config.LandFormType.Instance.GetItem((sbyte)i);
				t.GetComponentInChildren<TextMeshProUGUI>().text = landFormType.Name;
				TooltipInvoker mouseTipDisPlayer = t.gameObject.GetOrAddComponent<TooltipInvoker>();
				mouseTipDisPlayer.triggerByChildRaycast = true;
				mouseTipDisPlayer.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = mouseTipDisPlayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				mouseTipDisPlayer.RuntimeParam.Set("arg0", landFormType.Name);
				mouseTipDisPlayer.RuntimeParam.Set("arg1", landFormType.Desc);
			}
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x002DC44C File Offset: 0x002DA64C
		public override void Init()
		{
			base.Init();
			string stateTemplateIdStr;
			sbyte stateTemplateId;
			sbyte id = (base.CreationInfoMap.TryGetValue("TaiwuVillageStateTemplateId", out stateTemplateIdStr) && sbyte.TryParse(stateTemplateIdStr, out stateTemplateId)) ? stateTemplateId : ((sbyte)Random.Range(1, 16));
			this.stateMap.SetSelectedStateTemplateId(id);
			this._canJump = false;
			this.videoTextureHolder.enabled = false;
			this.closeVideo.gameObject.SetActive(false);
		}

		// Token: 0x060063D7 RID: 25559 RVA: 0x002DC4BF File Offset: 0x002DA6BF
		private void Start()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.layoutGroupLandFormType.transform as RectTransform);
		}

		// Token: 0x060063D8 RID: 25560 RVA: 0x002DC4D8 File Offset: 0x002DA6D8
		private void OnEnable()
		{
			while (this._queue.Count > 0)
			{
				base.StartCoroutine(this._queue.Dequeue());
			}
		}

		// Token: 0x060063D9 RID: 25561 RVA: 0x002DC50C File Offset: 0x002DA70C
		private void OnDestroy()
		{
			this.toggleGroupLandFormType.OnActiveIndexChange -= this.OnLandFormTypeIndexSelected;
			this.stateMap.OnStateTemplateIdSelected -= this.OnStateTemplateIdSelected;
			this.stateMap.OnStateRefersUpdated -= this.OnStateRefersUpdated;
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x002DC564 File Offset: 0x002DA764
		private void LateUpdate()
		{
			bool flag = this.videoPlayer != null && !this.videoPlayer.isPlaying && this._canJump;
			if (flag)
			{
				this.CloseVideoPlay();
			}
		}

		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x060063DB RID: 25563 RVA: 0x002DC5A3 File Offset: 0x002DA7A3
		public override DialogCmd StartGameCheck
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x060063DC RID: 25564 RVA: 0x002DC5A6 File Offset: 0x002DA7A6
		// (set) Token: 0x060063DD RID: 25565 RVA: 0x002DC5A9 File Offset: 0x002DA7A9
		public override bool StartGameChecked
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		// Token: 0x060063DE RID: 25566 RVA: 0x002DC5AC File Offset: 0x002DA7AC
		public override void DoStartGame(ProtagonistCreationInfo protagonistCreationInfo, ref WorldCreationInfo worldCreationInfo)
		{
			base.CreationInfoMap["TaiwuVillageStateTemplateId"] = this._stateTemplateId.ToString();
			base.CreationInfoMap["TaiwuVillageLandFormType"] = this._landFormType.ToString();
			protagonistCreationInfo.TaiwuVillageStateTemplateId = this._stateTemplateId;
		}

		// Token: 0x060063DF RID: 25567 RVA: 0x002DC600 File Offset: 0x002DA800
		public void Setup()
		{
			string landFormTypeStr;
			sbyte landFormType;
			sbyte initialLandFormType = (base.CreationInfoMap.TryGetValue("TaiwuVillageLandFormType", out landFormTypeStr) && sbyte.TryParse(landFormTypeStr, out landFormType)) ? landFormType : 0;
			this.toggleGroupLandFormType.Init((int)initialLandFormType);
			this.OnLandFormTypeIndexSelected((int)initialLandFormType, -1);
			string stateTemplateIdStr;
			sbyte stateTemplateId;
			sbyte initialStateTemplateId = (sbyte)((base.CreationInfoMap.TryGetValue("TaiwuVillageStateTemplateId", out stateTemplateIdStr) && sbyte.TryParse(stateTemplateIdStr, out stateTemplateId)) ? ((int)stateTemplateId) : Random.Range(1, 16));
			this.stateMap.SetSelectedStateTemplateId(initialStateTemplateId);
			this.stateMap.FocusStateTemplateId(initialStateTemplateId, this.durationFocusState, null);
			this.OnStateTemplateIdSelected(initialStateTemplateId);
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x002DC6A0 File Offset: 0x002DA8A0
		public void ShowCombatSkillTree()
		{
			MapStateItem stateConfig = MapState.Instance[this._stateTemplateId];
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Clear();
			box.Set("SectTemplateId", stateConfig.SectID);
			box.Set("TaiwuFiveElementsType", this.parent.NeiliType);
			UIElement.CombatSkillTree.SetOnInitArgs(box);
			UIManager.Instance.MaskUI(UIElement.CombatSkillTree);
		}

		// Token: 0x060063E1 RID: 25569 RVA: 0x002DC714 File Offset: 0x002DA914
		private void OnLandFormTypeIndexSelected(int index, int _)
		{
			LandFormTypeItem config = Config.LandFormType.Instance[index];
			this.toggleHelper.RefreshLandFormType(config);
			this._landFormType = config.TemplateId;
			base.CreationInfoMap["TaiwuVillageLandFormType"] = this._landFormType.ToString();
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				base.StartCoroutine(this.<OnLandFormTypeIndexSelected>g__DelayProcess|41_0());
			}
			else
			{
				this._queue.Enqueue(this.<OnLandFormTypeIndexSelected>g__DelayProcess|41_0());
			}
		}

		// Token: 0x060063E2 RID: 25570 RVA: 0x002DC798 File Offset: 0x002DA998
		private void OnStateTemplateIdSelected(sbyte stateTemplateId)
		{
			NewGameSubPageBornArea.<>c__DisplayClass42_0 CS$<>8__locals1 = new NewGameSubPageBornArea.<>c__DisplayClass42_0();
			CS$<>8__locals1.<>4__this = this;
			MapStateItem stateConfig = MapState.Instance[stateTemplateId];
			MapAreaItem mainAreaConfig = MapArea.Instance[(short)stateConfig.MainAreaID];
			CS$<>8__locals1.sectConfig = Organization.Instance[stateConfig.SectID];
			this.rawImagePoster.SetTexture(CS$<>8__locals1.<OnStateTemplateIdSelected>g__TextureName|0());
			this.rawImageNameBack.SetTexture(CS$<>8__locals1.<OnStateTemplateIdSelected>g__BackName|1());
			int sectIndex = (int)(CS$<>8__locals1.sectConfig.TemplateId - 1);
			for (int i = 0; i < this.sectNameGroup.childCount; i++)
			{
				GameObject sectNameGo = this.sectNameGroup.GetChild(i).gameObject;
				sectNameGo.SetActive(i == sectIndex);
			}
			for (int j = 0; j < this.spineGroup.childCount; j++)
			{
				GameObject sectNameGo2 = this.spineGroup.GetChild(j).gameObject;
				sectNameGo2.SetActive(j == sectIndex);
			}
			this.stateTitle.text = stateConfig.Name + LanguageKey.LK_Dot_Symbol.Tr() + mainAreaConfig.Name;
			this.labelDesc.text = mainAreaConfig.Desc.ColorReplace();
			CS$<>8__locals1.<OnStateTemplateIdSelected>g__SetupTextSpriteHelper|2(this.labelDesc.gameObject);
			this.labelTitle2.text = CS$<>8__locals1.sectConfig.Name;
			this.labelDesc2.text = CS$<>8__locals1.sectConfig.OrganizationExtraDesc.ColorReplace();
			CS$<>8__locals1.<OnStateTemplateIdSelected>g__SetupTextSpriteHelper|2(this.labelDesc2.gameObject);
			this.imageIcon.SetSprite("ui9_icon_sect_icon_" + CS$<>8__locals1.sectConfig.TemplateId.ToString(), false, null);
			this.toggleHelper.RefreshMapState(stateConfig);
			this._stateTemplateId = stateTemplateId;
			base.CreationInfoMap["TaiwuVillageStateTemplateId"] = this._stateTemplateId.ToString();
			bool isOn = this.toggleHelper.GetComponentInChildren<Toggle>().isOn;
			if (isOn)
			{
				AudioManager.Instance.PlaySound(stateConfig.BirthSound, false, false);
			}
			for (int k = 0; k < this.landFormTypeConfig.Length; k++)
			{
				byte[] bornMapType = stateConfig.BornMapType;
				bool valid = bornMapType == null || bornMapType.Contains((byte)k);
				NewGameSubPageBornArea.LandFormTypeConfiguration c = this.landFormTypeConfig[k];
				c.target.sprite = (valid ? c.normal : c.disabled);
				this.toggleGroupLandFormType.SetInteractable(valid, k);
				c.target.gameObject.SetActive(valid);
			}
			CToggle ctoggle = this.toggleGroupLandFormType.Get(this.toggleGroupLandFormType.GetActiveIndex());
			bool flag = ctoggle != null && !ctoggle.interactable;
			if (flag)
			{
				this.toggleGroupLandFormType.SetToFirstInteractable(true);
			}
			CS$<>8__locals1.<OnStateTemplateIdSelected>g__RefreshLayout|3();
		}

		// Token: 0x060063E3 RID: 25571 RVA: 0x002DCA84 File Offset: 0x002DAC84
		private void OnStateRefersUpdated(sbyte stateTemplateId, Refers refers, bool isSelect)
		{
			NewGameSubPageBornArea.<>c__DisplayClass44_0 CS$<>8__locals1 = new NewGameSubPageBornArea.<>c__DisplayClass44_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.stateTemplateId = stateTemplateId;
			MapStateItem stateConfig = MapState.Instance[CS$<>8__locals1.stateTemplateId];
			OrganizationItem sectConfig = Organization.Instance[stateConfig.SectID];
			TextMeshProUGUI labelName = refers.CGet<TextMeshProUGUI>("Name");
			CS$<>8__locals1.btnJump = refers.CGet<CButton>("Jump");
			RectTransform root;
			bool flag = refers.CTryGet<RectTransform>("Root", out root);
			if (flag)
			{
				root.anchoredPosition = this.stateMap.GetShapeAnchor(CS$<>8__locals1.stateTemplateId);
			}
			labelName.text = stateConfig.Name + LanguageKey.LK_Dot_Symbol.Tr() + sectConfig.Name;
			labelName.transform.parent.SetParent(this.stateMapOpLayer, true);
			CS$<>8__locals1.btnJump.onClick.ResetListener(delegate()
			{
				CS$<>8__locals1.<>4__this.stateMap.FocusStateTemplateId(CS$<>8__locals1.stateTemplateId, CS$<>8__locals1.<>4__this.durationFocusState, CS$<>8__locals1.btnJump.GetComponent<RectTransform>());
			});
			CS$<>8__locals1.btnJump.transform.SetParent(this.stateMapOpLayer, true);
			CS$<>8__locals1.stateImage = refers.CGet<IrregularClickableImage>("Shape");
			if (isSelect)
			{
				bool activeSelf = base.gameObject.activeSelf;
				if (activeSelf)
				{
					base.StartCoroutine(CS$<>8__locals1.<OnStateRefersUpdated>g__DelayProcess|1());
				}
				else
				{
					this._queue.Enqueue(CS$<>8__locals1.<OnStateRefersUpdated>g__DelayProcess|1());
				}
			}
		}

		// Token: 0x060063E4 RID: 25572 RVA: 0x002DCBD0 File Offset: 0x002DADD0
		public void SectIntroducePlay()
		{
			bool flag = LocalStringManager.CurLanguageKey == "CN" || LocalStringManager.CurLanguageKey == "EN";
			if (flag)
			{
				MapStateItem stateConfig = MapState.Instance[this._stateTemplateId];
				this.videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "Movies/SectIntroduce", string.Format("Sect_{0}.mp4", stateConfig.SectID)).PathFix();
				this.videoPlayer.Prepare();
			}
		}

		// Token: 0x060063E5 RID: 25573 RVA: 0x002DCC5C File Offset: 0x002DAE5C
		private void OnVideoPrepared(VideoPlayer source)
		{
			try
			{
				bool flag = this.videoPlayer.renderMode == VideoRenderMode.RenderTexture;
				if (flag)
				{
					RenderTexture texture = new RenderTexture(this.videoPlayer.texture.width, this.videoPlayer.texture.height, 24);
					this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
					this.videoPlayer.targetTexture = texture;
					this.videoTextureHolder.texture = texture;
					this.videoTextureHolder.SetNativeSize();
				}
				this.videoPlayer.SetDirectAudioVolume(0, (float)SingletonObject.getInstance<GlobalSettings>().VideoVolume / 100f);
				this.videoPlayer.frame = 0L;
				AudioManager.Instance.EnterVideoMode();
				this.videoPlayer.Play();
			}
			catch (Exception e)
			{
				GLog.Log("NewGameSubPageBornArea OnEnable video play failed");
				Debug.LogWarning(e);
			}
			finally
			{
				bool flag2 = this.videoPlayer.clip == null && this.videoPlayer.url == null;
				if (flag2)
				{
					GLog.Log("NewGameSubPageBornArea OnEnable video play failed ");
				}
			}
		}

		// Token: 0x060063E6 RID: 25574 RVA: 0x002DCD90 File Offset: 0x002DAF90
		private void OnVideoPlayStart(VideoPlayer source)
		{
			this._canJump = true;
			this.videoTextureHolder.enabled = true;
			this.closeVideo.gameObject.SetActive(true);
			this.parent.BornAreaPlayingVideo = true;
		}

		// Token: 0x060063E7 RID: 25575 RVA: 0x002DCDC5 File Offset: 0x002DAFC5
		private void OnVideoPlayError(VideoPlayer source, string message)
		{
			this.CloseVideoPlay();
		}

		// Token: 0x060063E8 RID: 25576 RVA: 0x002DCDD0 File Offset: 0x002DAFD0
		private void CloseVideoPlay()
		{
			this.parent.BornAreaPlayingVideo = false;
			this.closeVideo.gameObject.SetActive(false);
			this.videoTextureHolder.enabled = false;
			this.videoPlayer.Pause();
			this.videoPlayer.Stop();
			this._canJump = false;
			AudioManager.Instance.ExitVideoMode();
			RenderTexture prevTexture = this.videoPlayer.targetTexture;
			bool flag = prevTexture != null;
			if (flag)
			{
				Object.Destroy(prevTexture);
			}
		}

		// Token: 0x060063EA RID: 25578 RVA: 0x002DCE74 File Offset: 0x002DB074
		[CompilerGenerated]
		private IEnumerator <OnLandFormTypeIndexSelected>g__DelayProcess|41_0()
		{
			yield return this.landFormTypeSelectionKeeper.enabled ? new WaitForEndOfFrame() : new WaitForSeconds(0.2f);
			CToggle toggleLandFormType = this.toggleGroupLandFormType.Get(this.toggleGroupLandFormType.GetActiveIndex());
			bool flag = toggleLandFormType != null;
			if (flag)
			{
				RectTransform rect = this.landFormTypeSelectionKeeper.transform as RectTransform;
				RectTransform rect2 = toggleLandFormType.transform.parent as RectTransform;
				bool flag2 = rect != null && rect2 != null;
				if (flag2)
				{
					rect.position = rect2.position;
				}
				this.landFormTypeSelectionKeeper.enabled = true;
				rect = null;
				rect2 = null;
			}
			yield break;
		}

		// Token: 0x040045B2 RID: 17842
		[SerializeField]
		private NewGameSubPageBornAreaToggleHelper toggleHelper;

		// Token: 0x040045B3 RID: 17843
		[SerializeField]
		private CRawImage rawImagePoster;

		// Token: 0x040045B4 RID: 17844
		[SerializeField]
		private CRawImage rawImageNameBack;

		// Token: 0x040045B5 RID: 17845
		[SerializeField]
		private string patternPosterTexture;

		// Token: 0x040045B6 RID: 17846
		[SerializeField]
		private string patternNameBackTexture;

		// Token: 0x040045B7 RID: 17847
		[SerializeField]
		private VerticalLayoutGroup layoutGroupDesc;

		// Token: 0x040045B8 RID: 17848
		[SerializeField]
		private RectTransform sectNameGroup;

		// Token: 0x040045B9 RID: 17849
		[SerializeField]
		private RectTransform spineGroup;

		// Token: 0x040045BA RID: 17850
		[SerializeField]
		private float durationFocusState;

		// Token: 0x040045BB RID: 17851
		[SerializeField]
		private StateMap stateMap;

		// Token: 0x040045BC RID: 17852
		[SerializeField]
		private RectTransform stateMapOpLayer;

		// Token: 0x040045BD RID: 17853
		[SerializeField]
		private CImage stateMapSelectionKeeper;

		// Token: 0x040045BE RID: 17854
		[SerializeField]
		private TextMeshProUGUI stateTitle;

		// Token: 0x040045BF RID: 17855
		[SerializeField]
		private TextMeshProUGUI labelTitle2;

		// Token: 0x040045C0 RID: 17856
		[SerializeField]
		private TextMeshProUGUI labelDesc;

		// Token: 0x040045C1 RID: 17857
		[SerializeField]
		private TextMeshProUGUI labelDesc2;

		// Token: 0x040045C2 RID: 17858
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x040045C3 RID: 17859
		[SerializeField]
		private CToggleGroup toggleGroupLandFormType;

		// Token: 0x040045C4 RID: 17860
		[SerializeField]
		private LayoutGroup layoutGroupLandFormType;

		// Token: 0x040045C5 RID: 17861
		[SerializeField]
		private CImage landFormTypeSelectionKeeper;

		// Token: 0x040045C6 RID: 17862
		[SerializeField]
		private NewGameSubPageBornArea.LandFormTypeConfiguration[] landFormTypeConfig;

		// Token: 0x040045C7 RID: 17863
		[SerializeField]
		private VideoPlayer videoPlayer;

		// Token: 0x040045C8 RID: 17864
		[SerializeField]
		private CRawImage videoTextureHolder;

		// Token: 0x040045C9 RID: 17865
		[SerializeField]
		private CButton closeVideo;

		// Token: 0x040045CA RID: 17866
		private sbyte _landFormType = -1;

		// Token: 0x040045CB RID: 17867
		private sbyte _stateTemplateId = -1;

		// Token: 0x040045CC RID: 17868
		private readonly Queue<IEnumerator> _queue = new Queue<IEnumerator>();

		// Token: 0x040045CD RID: 17869
		private bool _canJump;

		// Token: 0x02001D3A RID: 7482
		[Serializable]
		internal class LandFormTypeConfiguration
		{
			// Token: 0x0400C567 RID: 50535
			[SerializeField]
			internal CImage target;

			// Token: 0x0400C568 RID: 50536
			[SerializeField]
			internal Sprite normal;

			// Token: 0x0400C569 RID: 50537
			[SerializeField]
			internal Sprite disabled;
		}
	}
}
