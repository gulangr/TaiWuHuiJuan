using System;
using System.Collections;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.Adventure
{
	// Token: 0x02000F84 RID: 3972
	public class AdventureRemakeFinish : MonoBehaviour, ILanguage
	{
		// Token: 0x17001499 RID: 5273
		// (get) Token: 0x0600B68C RID: 46732 RVA: 0x00533628 File Offset: 0x00531828
		// (set) Token: 0x0600B68D RID: 46733 RVA: 0x00533630 File Offset: 0x00531830
		public bool Finish
		{
			get
			{
				return this._finish;
			}
			set
			{
				this._finish = value;
				base.gameObject.SetActive(this._finish);
			}
		}

		// Token: 0x0600B68E RID: 46734 RVA: 0x0053364C File Offset: 0x0053184C
		public void Show(Action exitAction, bool majorEvent)
		{
			UIManager.Instance.MaskComponent(base.GetComponent<RectTransform>());
			AudioManager.Instance.PlaySound("ui_EventComplete", false, false);
			this.exitButton.interactable = false;
			this.Finish = true;
			this.isMajorEvent = majorEvent;
			this.exitButton.ClearAndAddListener(delegate
			{
				bool flag3 = this.gameObject.activeInHierarchy && !this.stageOut;
				if (flag3)
				{
					this.StartCoroutine(this.SwitchStageOut(exitAction));
				}
			});
			foreach (GameObject jian in this.jianbing)
			{
				bool flag = jian == null;
				if (!flag)
				{
					jian.gameObject.SetActive(majorEvent);
				}
			}
			foreach (GameObject die in this.hudie)
			{
				bool flag2 = die == null;
				if (!flag2)
				{
					die.SetActive(!majorEvent);
				}
			}
			bool isCN = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
			this.ResetColor();
			this.StateIn();
		}

		// Token: 0x0600B68F RID: 46735 RVA: 0x00533764 File Offset: 0x00531964
		private void ResetColor()
		{
			foreach (ParticleSystemRenderer render in this.particleIn.GetComponentsInChildren<ParticleSystemRenderer>())
			{
				bool flag = render.material == null;
				if (!flag)
				{
					render.material.SetColor(AdventureRemakeFinish.ColorID, Color.white);
				}
			}
			this.stageOut = false;
			this.particleLoop.gameObject.SetActive(false);
			foreach (ParticleSystemRenderer render2 in this.particleLoop.GetComponentsInChildren<ParticleSystemRenderer>())
			{
				bool flag2 = render2.material == null;
				if (!flag2)
				{
					render2.material.SetColor(AdventureRemakeFinish.ColorID, Color.white);
				}
			}
		}

		// Token: 0x0600B690 RID: 46736 RVA: 0x00533834 File Offset: 0x00531A34
		private void StateIn()
		{
			this.PlayDissolveAnimation(this.juanzhouMaterial, 0f, 0.8f, 0.2f, null);
			this.PlayDissolveAnimation(this.adventureTitleInMaterial, 0f, 0.8f, 0.2f, null);
			this.PlayDissolveAnimation(this.majorEventTitleInMaterial, 0f, 0.8f, 0.2f, null);
			this.PlayDissolveAnimation(this.yun1Material, 0.2f, 0.7f, 0.1f, null);
			this.PlayDissolveAnimation(this.yun2Material, 0.2f, 0.4f, 0.4f, null);
			this.PlayDissolveAnimation(this.yun3Material, 0.2f, 0.7f, 0.1f, null);
			this.PlayDissolveAnimation(this.yun4Material, 0f, 0.8f, 0.2f, null);
			this.PlayDissolveAnimation(this.yun5Material, 0.4f, 0.6f, 0f, null);
			this.PlayDissolveAnimation(this.yun6Material, 0.3f, 0.7f, 0f, null);
			this.PlayDissolveAnimation(this.shan1Material, 0.2f, 0.7f, 0.1f, null);
			this.PlayDissolveAnimation(this.shan2Material, 0.2f, 0.7f, 0.1f, null);
			this.PlayDissolveAnimation(this.shan3Material, 0.5f, 0.5f, 0f, null);
			this.PlayDissolveAnimation(this.shan4Material, 0.6f, 0.4f, 0f, null);
			this.PlayDissolveAnimation(this.shan5Material, 0.6f, 0.4f, 0f, null);
			this.PlayDissolveAnimation(this.jianzhu1Material, 0.5f, 0.5f, 0f, null);
			this.PlayDissolveAnimation(this.jianzhu2Material, 0.2f, 0.6f, 0.2f, null);
			this.PlayDissolveAnimation(this.jianzhu3Material, 0.5f, 0.5f, 0f, new Action(this.StageInEnd));
			this.particleIn.gameObject.SetActive(true);
		}

		// Token: 0x0600B691 RID: 46737 RVA: 0x00533A4C File Offset: 0x00531C4C
		private void StageInEnd()
		{
			this.particleLoop.gameObject.SetActive(true);
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				base.StartCoroutine(this.SwitchStageLoop());
			}
		}

		// Token: 0x0600B692 RID: 46738 RVA: 0x00533A88 File Offset: 0x00531C88
		private IEnumerator SwitchStageLoop()
		{
			this.StateInFadeOut();
			yield return new WaitForSeconds(this.switchDuration);
			this.exitButton.interactable = true;
			this.particleIn.gameObject.SetActive(false);
			yield break;
		}

		// Token: 0x0600B693 RID: 46739 RVA: 0x00533A98 File Offset: 0x00531C98
		private void StateInFadeOut()
		{
			ParticleSystemRenderer[] renderers = this.particleIn.GetComponentsInChildren<ParticleSystemRenderer>();
			for (int i = 0; i < renderers.Length; i++)
			{
				ParticleSystemRenderer render = renderers[i];
				bool flag = render.materials == null;
				if (!flag)
				{
					DOVirtual.Float(1f, 0f, this.switchDuration, delegate(float value)
					{
						render.material.SetColor(AdventureRemakeFinish.ColorID, new Color(1f, 1f, 1f, value));
					});
				}
			}
		}

		// Token: 0x0600B694 RID: 46740 RVA: 0x00533B0D File Offset: 0x00531D0D
		private IEnumerator SwitchStageOut(Action exitAction)
		{
			this.stageOut = true;
			ParticleSystemRenderer[] renderers = this.particleLoop.GetComponentsInChildren<ParticleSystemRenderer>();
			int num;
			for (int i = 0; i < renderers.Length; i = num + 1)
			{
				AdventureRemakeFinish.<>c__DisplayClass67_0 CS$<>8__locals1 = new AdventureRemakeFinish.<>c__DisplayClass67_0();
				CS$<>8__locals1.render = renderers[i];
				bool flag = CS$<>8__locals1.render.materials == null;
				if (!flag)
				{
					DOVirtual.Float(1f, 0f, 0.8f, delegate(float value)
					{
						CS$<>8__locals1.render.material.SetColor(AdventureRemakeFinish.ColorID, new Color(1f, 1f, 1f, value));
					});
					CS$<>8__locals1 = null;
				}
				num = i;
			}
			yield return new WaitForSeconds(0.8f);
			UIManager.Instance.UnMaskComponent(base.GetComponent<RectTransform>());
			if (exitAction != null)
			{
				exitAction();
			}
			yield break;
		}

		// Token: 0x0600B695 RID: 46741 RVA: 0x00533B24 File Offset: 0x00531D24
		public void PlayDissolveAnimation(Material material, float duration1, float duration2, float duration3, Action complete)
		{
			material.SetFloat(AdventureRemakeFinish.RongjieID, 1f);
			Sequence dissolveSequence = DOTween.Sequence();
			bool flag = duration1 > 0f;
			if (flag)
			{
				dissolveSequence.AppendInterval(duration1);
			}
			dissolveSequence.Append(DOTween.To(() => material.GetFloat(AdventureRemakeFinish.RongjieID), delegate(float x)
			{
				material.SetFloat(AdventureRemakeFinish.RongjieID, x);
			}, -0.1f, duration2));
			bool flag2 = duration3 > 0f;
			if (flag2)
			{
				dissolveSequence.AppendInterval(duration3);
			}
			dissolveSequence.Play<Sequence>();
			dissolveSequence.OnComplete(delegate
			{
				Action complete2 = complete;
				if (complete2 != null)
				{
					complete2();
				}
			});
		}

		// Token: 0x0600B696 RID: 46742 RVA: 0x00533BD4 File Offset: 0x00531DD4
		private void Awake()
		{
			this.particleIn.gameObject.SetActive(false);
			this.particleLoop.gameObject.SetActive(false);
			this.juanzhouMaterial = this.juanzhou.material;
			this.yun1Material = this.yun1.material;
			this.yun2Material = this.yun2.material;
			this.yun3Material = this.yun3.material;
			this.yun4Material = this.yun4.material;
			this.yun5Material = this.yun5.material;
			this.yun6Material = this.yun6.material;
			this.shan1Material = this.shan1.material;
			this.shan2Material = this.shan2.material;
			this.shan3Material = this.shan3.material;
			this.shan4Material = this.shan4.material;
			this.shan5Material = this.shan5.material;
			this.jianzhu1Material = this.jianzhu1.material;
			this.jianzhu2Material = this.jianzhu2.material;
			this.jianzhu3Material = this.jianzhu3.material;
		}

		// Token: 0x0600B697 RID: 46743 RVA: 0x00533D08 File Offset: 0x00531F08
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			this.ResetLanguageState();
			this.currentLanguage = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			this.currentLanguageIndex = -1;
			for (int i = 0; i < this.localizationTitles.Length; i++)
			{
				bool flag = this.localizationTitles[i].languageId == this.currentLanguage;
				if (flag)
				{
					this.currentLanguageIndex = i;
					break;
				}
			}
			bool flag2 = this.currentLanguageIndex < 0;
			if (flag2)
			{
				GLog.TagError("AdventureRemakeFinish", "Language " + this.currentLanguage + " not found in localizationTitles!", Array.Empty<object>());
				this.currentLanguageIndex = 0;
			}
			this.RefreshLanguage();
		}

		// Token: 0x0600B698 RID: 46744 RVA: 0x00533DC0 File Offset: 0x00531FC0
		private void ResetLanguageState()
		{
			foreach (AdventureRemakeFinish.LocalizationTitles param in this.localizationTitles)
			{
				foreach (GameObject title in param.adventureTitles)
				{
					bool flag = title == null;
					if (!flag)
					{
						title.gameObject.SetActive(false);
					}
				}
				foreach (GameObject title2 in param.majorEventTitles)
				{
					bool flag2 = title2 == null;
					if (!flag2)
					{
						title2.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600B699 RID: 46745 RVA: 0x00533E78 File Offset: 0x00532078
		private void RefreshLanguage()
		{
			AdventureRemakeFinish.LocalizationTitles param = this.localizationTitles[this.currentLanguageIndex];
			this.adventureTitleInMaterial = param.adventureTitleIn.material;
			this.majorEventTitleInMaterial = param.majorEventTitleIn.material;
			foreach (GameObject title in param.adventureTitles)
			{
				bool flag = title == null;
				if (!flag)
				{
					title.gameObject.SetActive(!this.isMajorEvent);
				}
			}
			foreach (GameObject title2 in param.majorEventTitles)
			{
				bool flag2 = title2 == null;
				if (!flag2)
				{
					title2.gameObject.SetActive(this.isMajorEvent);
				}
			}
		}

		// Token: 0x04008DA0 RID: 36256
		[SerializeField]
		private CButton exitButton;

		// Token: 0x04008DA1 RID: 36257
		[SerializeField]
		private CanvasGroup canvas;

		// Token: 0x04008DA2 RID: 36258
		[SerializeField]
		private ParticleSystem particleIn;

		// Token: 0x04008DA3 RID: 36259
		[SerializeField]
		private ParticleSystem particleLoop;

		// Token: 0x04008DA4 RID: 36260
		[SerializeField]
		private Renderer juanzhou;

		// Token: 0x04008DA5 RID: 36261
		[SerializeField]
		private Renderer yun1;

		// Token: 0x04008DA6 RID: 36262
		[SerializeField]
		private Renderer yun2;

		// Token: 0x04008DA7 RID: 36263
		[SerializeField]
		private Renderer yun3;

		// Token: 0x04008DA8 RID: 36264
		[SerializeField]
		private Renderer yun4;

		// Token: 0x04008DA9 RID: 36265
		[SerializeField]
		private Renderer yun5;

		// Token: 0x04008DAA RID: 36266
		[SerializeField]
		private Renderer yun6;

		// Token: 0x04008DAB RID: 36267
		[SerializeField]
		private Renderer shan1;

		// Token: 0x04008DAC RID: 36268
		[SerializeField]
		private Renderer shan2;

		// Token: 0x04008DAD RID: 36269
		[SerializeField]
		private Renderer shan3;

		// Token: 0x04008DAE RID: 36270
		[SerializeField]
		private Renderer shan4;

		// Token: 0x04008DAF RID: 36271
		[SerializeField]
		private Renderer shan5;

		// Token: 0x04008DB0 RID: 36272
		[SerializeField]
		private Renderer jianzhu1;

		// Token: 0x04008DB1 RID: 36273
		[SerializeField]
		private Renderer jianzhu2;

		// Token: 0x04008DB2 RID: 36274
		[SerializeField]
		private Renderer jianzhu3;

		// Token: 0x04008DB3 RID: 36275
		[SerializeField]
		private Renderer cnAdventureTitleIn;

		// Token: 0x04008DB4 RID: 36276
		[SerializeField]
		private Renderer enAdventureTitleIn;

		// Token: 0x04008DB5 RID: 36277
		[SerializeField]
		private Renderer cnMajorEventTitleIn;

		// Token: 0x04008DB6 RID: 36278
		[SerializeField]
		private Renderer enMajorEventTitleIn;

		// Token: 0x04008DB7 RID: 36279
		[SerializeField]
		private GameObject[] jianbing = new GameObject[2];

		// Token: 0x04008DB8 RID: 36280
		[SerializeField]
		private GameObject[] hudie = new GameObject[2];

		// Token: 0x04008DB9 RID: 36281
		[SerializeField]
		private GameObject[] cnAdventureTitles = new GameObject[2];

		// Token: 0x04008DBA RID: 36282
		[SerializeField]
		private GameObject[] enAdventureTitles = new GameObject[2];

		// Token: 0x04008DBB RID: 36283
		[SerializeField]
		private GameObject[] cnMajorEventTitles = new GameObject[2];

		// Token: 0x04008DBC RID: 36284
		[SerializeField]
		private GameObject[] enMajorEventTitles = new GameObject[2];

		// Token: 0x04008DBD RID: 36285
		[SerializeField]
		private AdventureRemakeFinish.LocalizationTitles[] localizationTitles;

		// Token: 0x04008DBE RID: 36286
		public float switchDuration = 0.5f;

		// Token: 0x04008DBF RID: 36287
		private Material juanzhouMaterial;

		// Token: 0x04008DC0 RID: 36288
		private Material yun1Material;

		// Token: 0x04008DC1 RID: 36289
		private Material yun2Material;

		// Token: 0x04008DC2 RID: 36290
		private Material yun3Material;

		// Token: 0x04008DC3 RID: 36291
		private Material yun4Material;

		// Token: 0x04008DC4 RID: 36292
		private Material yun5Material;

		// Token: 0x04008DC5 RID: 36293
		private Material yun6Material;

		// Token: 0x04008DC6 RID: 36294
		private Material shan1Material;

		// Token: 0x04008DC7 RID: 36295
		private Material shan2Material;

		// Token: 0x04008DC8 RID: 36296
		private Material shan3Material;

		// Token: 0x04008DC9 RID: 36297
		private Material shan4Material;

		// Token: 0x04008DCA RID: 36298
		private Material shan5Material;

		// Token: 0x04008DCB RID: 36299
		private Material jianzhu1Material;

		// Token: 0x04008DCC RID: 36300
		private Material jianzhu2Material;

		// Token: 0x04008DCD RID: 36301
		private Material jianzhu3Material;

		// Token: 0x04008DCE RID: 36302
		private Material adventureTitleInMaterial;

		// Token: 0x04008DCF RID: 36303
		private Material enAdventureTitleInMaterial;

		// Token: 0x04008DD0 RID: 36304
		private Material majorEventTitleInMaterial;

		// Token: 0x04008DD1 RID: 36305
		private Material enMajorEventTitleInMaterial;

		// Token: 0x04008DD2 RID: 36306
		private static readonly int RongjieID = Shader.PropertyToID("_rongjie");

		// Token: 0x04008DD3 RID: 36307
		private static readonly int ColorID = Shader.PropertyToID("_Color");

		// Token: 0x04008DD4 RID: 36308
		private string currentLanguage;

		// Token: 0x04008DD5 RID: 36309
		private int currentLanguageIndex;

		// Token: 0x04008DD6 RID: 36310
		private bool isMajorEvent;

		// Token: 0x04008DD7 RID: 36311
		private bool _finish;

		// Token: 0x04008DD8 RID: 36312
		private bool stageOut;

		// Token: 0x020025CB RID: 9675
		[Serializable]
		public struct LocalizationTitles
		{
			// Token: 0x0400E929 RID: 59689
			public string languageId;

			// Token: 0x0400E92A RID: 59690
			public GameObject[] adventureTitles;

			// Token: 0x0400E92B RID: 59691
			public GameObject[] majorEventTitles;

			// Token: 0x0400E92C RID: 59692
			public Renderer adventureTitleIn;

			// Token: 0x0400E92D RID: 59693
			public Renderer majorEventTitleIn;
		}
	}
}
