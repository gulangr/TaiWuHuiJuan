using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000409 RID: 1033
public class ViewProfessionMask : UIBase
{
	// Token: 0x1700064F RID: 1615
	// (get) Token: 0x06003DA3 RID: 15779 RVA: 0x001EF5B7 File Offset: 0x001ED7B7
	private ProfessionData Current
	{
		get
		{
			return SingletonObject.getInstance<ProfessionModel>().GetProfessionData(this._professionId);
		}
	}

	// Token: 0x17000650 RID: 1616
	// (get) Token: 0x06003DA4 RID: 15780 RVA: 0x001EF5C9 File Offset: 0x001ED7C9
	private bool AttributesReceived
	{
		get
		{
			return this._currAttributesReceived && this._maxAttributesReceived && this._changeEventReceived;
		}
	}

	// Token: 0x06003DA5 RID: 15781 RVA: 0x001EF5E4 File Offset: 0x001ED7E4
	public override void OnInit(ArgumentBox argsBox)
	{
		CommandKitBase.SetDisable(true);
		UIElement element = this.Element;
		element.OnDeActive = (Action)Delegate.Combine(element.OnDeActive, new Action(delegate()
		{
			CommandKitBase.SetDisable(false);
		}));
		argsBox.Get("selectSkillIndex", out this._skillIndex);
		bool flag = this._skillIndex < 0 || this._skillIndex > 3;
		if (flag)
		{
			throw new ArgumentOutOfRangeException(string.Format("Profession skill index out of range {0}", this._skillIndex));
		}
		argsBox.Get("ProfessionId", out this._professionId);
		this._skillTemplateId = ViewProfessionMask.GetConfigSkill(this.Current.TemplateId, this._skillIndex);
	}

	// Token: 0x06003DA6 RID: 15782 RVA: 0x001EF6A8 File Offset: 0x001ED8A8
	private void Awake()
	{
		this._defaultAppearancePosition = this.enter.Appearance.transform.position;
		this._defaultSkillGroupPosition = this.enter.SkillGroup.transform.position;
		bool flag = this._originalPositions == null;
		if (flag)
		{
			Transform transform = this.teammateRise.transform;
			this._originalPositions = new Dictionary<Transform, Vector3>
			{
				{
					transform,
					transform.position
				}
			};
		}
		if (this._canvasResets == null)
		{
			this._canvasResets = new CanvasGroup[]
			{
				this.enter.SkillGroup,
				this.propertyChange.ValueRoot,
				this.teammateRise.Root,
				this.begMoney.ValueRoot
			};
		}
		if (this._maskableGraphics == null)
		{
			this._maskableGraphics = new MaskableGraphic[]
			{
				this.enter.Background,
				this.enter.Appearance
			};
		}
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x001EF79E File Offset: 0x001ED99E
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			43U,
			79U
		}));
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x001EF7D0 File Offset: 0x001ED9D0
	private void OnEnable()
	{
		this.ResetAnim();
		GEvent.Add(UiEvents.CloseProfessionMask, new GEvent.Callback(this.CloseMask));
		GEvent.Add(UiEvents.UpdateProfessionIllustrationAndSkill, new GEvent.Callback(this.UpdateIllustrationAndSkill));
		GEvent.Add(UiEvents.ShowProfessionPropertyChange, new GEvent.Callback(this.ShowPropertyChange));
		GEvent.Add(UiEvents.ShowProfessionTeammateUI, new GEvent.Callback(this.ShowTeammateUI));
		GEvent.Add(UiEvents.PlayProfessionAudioSound, new GEvent.Callback(this.PlayProfessionAudio));
		GEvent.Add(UiEvents.PlayProfessionSkillAnimAndShowEffect, new GEvent.Callback(this.PlaySkillAnimAndShowEffect));
	}

	// Token: 0x06003DA9 RID: 15785 RVA: 0x001EF890 File Offset: 0x001EDA90
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.CloseProfessionMask, new GEvent.Callback(this.CloseMask));
		GEvent.Remove(UiEvents.UpdateProfessionIllustrationAndSkill, new GEvent.Callback(this.UpdateIllustrationAndSkill));
		GEvent.Remove(UiEvents.ShowProfessionPropertyChange, new GEvent.Callback(this.ShowPropertyChange));
		GEvent.Remove(UiEvents.ShowProfessionTeammateUI, new GEvent.Callback(this.ShowTeammateUI));
		GEvent.Remove(UiEvents.PlayProfessionAudioSound, new GEvent.Callback(this.PlayProfessionAudio));
		GEvent.Remove(UiEvents.PlayProfessionSkillAnimAndShowEffect, new GEvent.Callback(this.PlaySkillAnimAndShowEffect));
		this.ResetData();
	}

	// Token: 0x06003DAA RID: 15786 RVA: 0x001EF94D File Offset: 0x001EDB4D
	protected void OnDestroy()
	{
		this._cachedTextures.Clear();
	}

	// Token: 0x06003DAB RID: 15787 RVA: 0x001EF95C File Offset: 0x001EDB5C
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			DataUid uid = notification.Uid;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				bool flag = uid.DomainId == 4 && uid.DataId == 0;
				if (flag)
				{
					bool flag2 = notification.Uid.SubId1 == 79U;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this._maxAttributes);
						this._maxAttributesReceived = true;
						this.TryPlayPropertyChange();
					}
					else
					{
						bool flag3 = notification.Uid.SubId1 == 43U && !this.AttributesReceived;
						if (flag3)
						{
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this._currAttributes);
							this._currAttributesReceived = true;
							this.TryPlayPropertyChange();
						}
					}
				}
			}
		}
	}

	// Token: 0x06003DAC RID: 15788 RVA: 0x001EFA88 File Offset: 0x001EDC88
	private void CloseMask(ArgumentBox argBox)
	{
		Sequence hidingAnimSeq = this._hidingAnimSeq;
		if (hidingAnimSeq != null)
		{
			hidingAnimSeq.Kill(false);
		}
		Sequence showAndHideSeq = this._showAndHideSeq;
		this._showAndHideSeq = null;
		this._hidingAnimSeq = showAndHideSeq;
		bool flag = this._hidingAnimSeq != null;
		if (flag)
		{
			this._hidingAnimSeq.PlayBackwards();
			this._hidingAnimSeq.OnStepComplete(delegate
			{
				UIManager.Instance.HideUI(this.Element);
				this._hidingAnimSeq.Kill(false);
				this._hidingAnimSeq = null;
			});
		}
		else
		{
			UIManager.Instance.HideUI(this.Element);
		}
	}

	// Token: 0x06003DAD RID: 15789 RVA: 0x001EFB08 File Offset: 0x001EDD08
	private void UpdateIllustrationAndSkill(ArgumentBox argBox)
	{
		string configTex = ViewProfessionMask.GetConfigTexture(this.Current);
		bool flag = configTex.IsNullOrEmpty();
		if (flag)
		{
			string format = "当前志向未配置立绘贴图, templateId={0}";
			ProfessionData professionData = this.Current;
			throw new Exception(string.Format(format, (professionData != null) ? professionData.TemplateId : -1));
		}
		Texture2D cachedTex;
		bool flag2 = this._cachedTextures.TryGetValue(configTex, out cachedTex);
		if (flag2)
		{
			this.enter.Appearance.texture = cachedTex;
		}
		else
		{
			ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/Profession", configTex), delegate(Texture2D tex)
			{
				this._cachedTextures.Add(configTex, tex);
				this.enter.Appearance.texture = tex;
			}, null, false);
		}
		this.EnterAnim();
	}

	// Token: 0x06003DAE RID: 15790 RVA: 0x001EFBC4 File Offset: 0x001EDDC4
	private void ShowPropertyChange(ArgumentBox argBox)
	{
		this._changeEventReceived = true;
		this.TryPlayPropertyChange();
	}

	// Token: 0x06003DAF RID: 15791 RVA: 0x001EFBD8 File Offset: 0x001EDDD8
	private void ShowTeammateUI(ArgumentBox argBox)
	{
		int teammateCharacterId;
		argBox.Get("teammateCharacterId", out teammateCharacterId);
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
		{
			teammateCharacterId
		}, delegate(int offset, RawDataPool pool)
		{
			List<CharacterDisplayData> displayData = null;
			Serializer.Deserialize(pool, offset, ref displayData);
			this.AnimTeammateRise(displayData[0]);
		});
	}

	// Token: 0x06003DB0 RID: 15792 RVA: 0x001EFC14 File Offset: 0x001EDE14
	private void PlayProfessionAudio(ArgumentBox argBox)
	{
		string audioName;
		bool flag = ViewProfessionMask.ProfessionSkill2SeNames.TryGetValue(this._skillTemplateId, out audioName);
		if (flag)
		{
			AudioManager.Instance.PlaySound(audioName, false, false);
		}
		else
		{
			foreach (string complexSeName in this.GetComplexConfigSeName(argBox))
			{
				AudioManager.Instance.PlaySound(complexSeName, false, false);
			}
		}
	}

	// Token: 0x06003DB1 RID: 15793 RVA: 0x001EFC94 File Offset: 0x001EDE94
	private void PlaySkillAnimAndShowEffect(ArgumentBox argBox)
	{
		string configAnim;
		bool flag = ViewProfessionMask.ProfessionSkill2Anims.TryGetValue(this._skillTemplateId, out configAnim);
		if (flag)
		{
			this.AnimSpineByConfig(configAnim);
		}
		else
		{
			foreach (string complexAnim in this.GetComplexConfigAnim(argBox))
			{
				this.AnimSpineByConfig(complexAnim);
			}
		}
		string configEffect;
		bool flag2 = ViewProfessionMask.ProfessionSkill2Effects.TryGetValue(this._skillTemplateId, out configEffect);
		if (flag2)
		{
			this.AnimEffectByConfig(configEffect);
		}
		else
		{
			ValueTuple<ViewProfessionMask.CoAnimEffect, string[]> custom;
			bool flag3 = ViewProfessionMask.ProfessionSkill2Coroutine.TryGetValue(this._skillTemplateId, out custom);
			if (flag3)
			{
				base.StartCoroutine(custom.Item1(this, custom.Item2, argBox));
			}
			else
			{
				foreach (string complexEffect in this.GetComplexConfigEffect(argBox))
				{
					this.AnimEffectByConfig(complexEffect);
				}
			}
		}
	}

	// Token: 0x17000651 RID: 1617
	// (get) Token: 0x06003DB2 RID: 15794 RVA: 0x001EFDA8 File Offset: 0x001EDFA8
	private Sequence NewEnterSequence
	{
		get
		{
			RectTransform rtAppearance = this.enter.Appearance.GetComponent<RectTransform>();
			RectTransform rtSkillGroup = this.enter.SkillGroup.GetComponent<RectTransform>();
			rtAppearance.position = this.enter.AppearanceMoveRoot.position;
			rtSkillGroup.position = this.enter.SkillGroupMoveRoot.position;
			Sequence showAndHideSeq = DOTween.Sequence();
			showAndHideSeq.SetAutoKill(false);
			showAndHideSeq.Insert(0f, this.enter.Background.DOFade(1f, 0.4f));
			showAndHideSeq.Insert(0.17f, this.enter.Appearance.DOFade(1f, 0.27f));
			showAndHideSeq.Insert(0.17f, this.enter.SkillGroup.DOFade(1f, 0.27f));
			showAndHideSeq.Insert(0.17f, rtAppearance.DOMove(this._defaultAppearancePosition, 0.27f, false).SetEase(Ease.OutCubic));
			showAndHideSeq.Insert(0.17f, rtSkillGroup.DOMove(this._defaultSkillGroupPosition, 0.27f, false).SetEase(Ease.OutCubic));
			return showAndHideSeq;
		}
	}

	// Token: 0x06003DB3 RID: 15795 RVA: 0x001EFED8 File Offset: 0x001EE0D8
	private void EnterAnim()
	{
		int currentTemplateId = this.Current.TemplateId;
		ProfessionItem professionConfig = Profession.Instance[currentTemplateId];
		bool flag = professionConfig == null;
		if (flag)
		{
			throw new Exception(string.Format("Config missing, template id {0} not exist in Profession", this.Current.TemplateId));
		}
		ProfessionSkillItem skillConfig = ProfessionSkill.Instance[this._skillTemplateId];
		this.enter.NameLabel.text = LanguageKey.LK_Profession_And_Skill.TrFormat(professionConfig.Name, skillConfig.Name);
		this.enter.DescLabel.text = skillConfig.Desc;
		this.enter.SkillIcon.SetSprite(skillConfig.Icon, false, null);
		Sequence showAndHideSeq = this._showAndHideSeq;
		if (showAndHideSeq != null)
		{
			showAndHideSeq.Kill(false);
		}
		this._showAndHideSeq = this.NewEnterSequence;
		this._showAndHideSeq.PlayForward();
		this._showAndHideSeq.OnComplete(delegate
		{
			this.enter.ShowSkillEffect(this._skillIndex);
		});
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x001EFFD4 File Offset: 0x001EE1D4
	private unsafe void TryPlayPropertyChange()
	{
		sbyte attributeType;
		bool flag = this.AttributesReceived && ViewProfessionMask.ProfessionSkill2Property.TryGetValue(this._skillTemplateId, out attributeType);
		if (flag)
		{
			this.AnimPropertyChange(attributeType, (int)(*(ref this._currAttributes.Items.FixedElementField + (IntPtr)attributeType * 2)), (int)(*(ref this._maxAttributes.Items.FixedElementField + (IntPtr)attributeType * 2)));
			short now = *(ref this._currAttributes.Items.FixedElementField + (IntPtr)attributeType * 2);
			short max = *(ref this._maxAttributes.Items.FixedElementField + (IntPtr)attributeType * 2);
			int grow = this.Current.GetSeniorityMainAttributeAdditional();
			base.StartCoroutine(this.CoAnimPropertyChange((int)now, (int)max, Mathf.Min((int)now + grow, (int)max)));
		}
	}

	// Token: 0x06003DB5 RID: 15797 RVA: 0x001F0090 File Offset: 0x001EE290
	public void AnimPropertyChange(sbyte attribute, int current, int max)
	{
		this.propertyChange.gameObject.SetActive(true);
		CanvasGroup canvasRoot = this.propertyChange.ValueRoot;
		CImage imgIcon = this.propertyChange.Icon;
		imgIcon.SetSprite(CharacterPropertyDisplay.Instance[(short)attribute].Icon, false, null);
		this.SetProperty(current, max);
		canvasRoot.DOFade(1f, 0.3f).SetDelay(0.3f);
		this.AnimEffectByConfig("shanren1", delegate(GameObject go)
		{
			Transform t = go.transform;
			Renderer[] renderers = new Renderer[]
			{
				t.Find("zuv").GetComponent<Renderer>(),
				t.Find("zuv2").GetComponent<Renderer>()
			};
			int colorId = Shader.PropertyToID("_Color");
			Renderer[] array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				Renderer r = array[i];
				UnityEngine.Material mat = new UnityEngine.Material(r.material);
				r.material = mat;
				Color color = mat.GetColor(colorId);
				color.SetAlpha(1f);
				mat.SetColor(colorId, color);
				DOVirtual.Float(1f, 0f, 0.5f, delegate(float stepValue)
				{
					mat.SetColor(colorId, color.SetAlpha(stepValue));
				}).SetDelay(2f);
			}
		});
	}

	// Token: 0x06003DB6 RID: 15798 RVA: 0x001F0130 File Offset: 0x001EE330
	private IEnumerator CoAnimPropertyChange(int now, int max, int after)
	{
		while (now < after)
		{
			int num = now;
			now = num + 1;
			this.SetProperty(now, max);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003DB7 RID: 15799 RVA: 0x001F0154 File Offset: 0x001EE354
	public void AnimTeammateRise(CharacterDisplayData displayData)
	{
		this.teammateRise.gameObject.SetActive(true);
		this.teammateRise.Avatar.Refresh(displayData, true);
		this.teammateRise.NameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(displayData, displayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		this.teammateRise.transform.DOLocalMoveY(240f, 0.3f, false);
		CanvasGroup canvas = this.teammateRise.Root;
		canvas.alpha = 1f;
		canvas.DOFade(0f, 1f).SetDelay(3f);
	}

	// Token: 0x06003DB8 RID: 15800 RVA: 0x001F0200 File Offset: 0x001EE400
	private void SetProperty(int current, int max)
	{
		Transform valueRoot = this.propertyChange.ValueRoot.transform;
		ViewProfessionMask.ParseSpriteToCache(this._propertyValueCache, valueRoot, this.propertySpriteSize, ViewProfessionMask.ConvertPropertySprites(current, max));
	}

	// Token: 0x06003DB9 RID: 15801 RVA: 0x001F023C File Offset: 0x001EE43C
	private void SetBegMoney(int money)
	{
		Transform valueRoot = this.begMoney.ValueRoot.transform;
		ViewProfessionMask.ParseSpriteToCache(this._begMoneyCache, valueRoot, this.begMoneySpriteSize, ViewProfessionMask.ConvertNumberSprites(money));
	}

	// Token: 0x06003DBA RID: 15802 RVA: 0x001F0274 File Offset: 0x001EE474
	private static void ParseSpriteToCache(List<GameObject> cache, Transform valueRoot, Vector2 size, IEnumerable<string> sprites)
	{
		cache.RemoveAll((GameObject o) => null == o);
		int index = 0;
		foreach (string sprite in sprites)
		{
			bool flag = index == cache.Count;
			GameObject go;
			if (flag)
			{
				cache.Add(go = ViewProfessionMask.GetNewNumberCache(valueRoot, size));
			}
			else
			{
				go = cache[index];
			}
			go.name = sprite;
			go.GetComponent<CImage>().SetSprite(sprite, false, null);
			go.SetActive(true);
			index++;
		}
		for (int i = index; i < cache.Count; i++)
		{
			cache[i].SetActive(false);
		}
	}

	// Token: 0x06003DBB RID: 15803 RVA: 0x001F035C File Offset: 0x001EE55C
	private static GameObject GetNewNumberCache(Transform valueRoot, Vector2 size)
	{
		GameObject obj = new GameObject("new_cache", new Type[]
		{
			typeof(RectTransform),
			typeof(CImage),
			typeof(LayoutElement)
		});
		obj.transform.SetParent(valueRoot, false);
		obj.GetComponent<LayoutElement>().preferredWidth = size.x;
		CImage image = obj.GetComponent<CImage>();
		image.rectTransform.SetSize(size);
		image.preserveAspect = true;
		return obj;
	}

	// Token: 0x06003DBC RID: 15804 RVA: 0x001F03E3 File Offset: 0x001EE5E3
	private void AnimSpineByConfig(string configName)
	{
		this.AnimPlayByConfig(configName, "RemakeResources/Prefab/Legacy/Core/Profession/profession_", this._cachedAnims, new Action<GameObject>(this.AnimSpineByCached));
	}

	// Token: 0x06003DBD RID: 15805 RVA: 0x001F0405 File Offset: 0x001EE605
	private void AnimEffectByConfig(string configName)
	{
		this.AnimPlayByConfig(configName, "RemakeResources/Particle/UIEffectPrefabs/ProfessionSkill/eff_professionskill_", this._cachedEffects, new Action<GameObject>(this.AnimEffectByCached));
	}

	// Token: 0x06003DBE RID: 15806 RVA: 0x001F0428 File Offset: 0x001EE628
	private void AnimEffectByConfig(string configName, Action<GameObject> callback)
	{
		this.AnimPlayByConfig(configName, "RemakeResources/Particle/UIEffectPrefabs/ProfessionSkill/eff_professionskill_", this._cachedEffects, delegate(GameObject cache)
		{
			this.AnimEffectByCached(cache, callback);
		});
	}

	// Token: 0x06003DBF RID: 15807 RVA: 0x001F046C File Offset: 0x001EE66C
	private void AnimEffectByConfig(string configName, Transform root)
	{
		this.AnimPlayByConfig(configName, "RemakeResources/Particle/UIEffectPrefabs/ProfessionSkill/eff_professionskill_", this._cachedEffects, delegate(GameObject cache)
		{
			this.AnimEffectByCached(cache, root);
		});
	}

	// Token: 0x06003DC0 RID: 15808 RVA: 0x001F04B0 File Offset: 0x001EE6B0
	private void AnimEffectByConfig(string configName, Vector2 offset)
	{
		this.AnimPlayByConfig(configName, "RemakeResources/Particle/UIEffectPrefabs/ProfessionSkill/eff_professionskill_", this._cachedEffects, delegate(GameObject cache)
		{
			this.AnimEffectByCached(cache, offset);
		});
	}

	// Token: 0x06003DC1 RID: 15809 RVA: 0x001F04F4 File Offset: 0x001EE6F4
	private void AnimPlayByConfig(string configName, string path, Dictionary<string, GameObject> cache, Action<GameObject> playByCached)
	{
		GameObject cachedPrefab;
		bool flag = cache.TryGetValue(configName, out cachedPrefab);
		if (flag)
		{
			playByCached(cachedPrefab);
		}
		else
		{
			ResLoader.Load<GameObject>(path + configName, delegate(GameObject prefab)
			{
				cache.Add(configName, prefab);
				playByCached(prefab);
			}, null, false);
		}
	}

	// Token: 0x06003DC2 RID: 15810 RVA: 0x001F0568 File Offset: 0x001EE768
	private void AnimSpineByCached(GameObject cachedAnim)
	{
		GameObject go = Object.Instantiate<GameObject>(cachedAnim, this.animationRoot);
		go.SetActive(true);
		this._waitingToDestroy.Add(go);
	}

	// Token: 0x06003DC3 RID: 15811 RVA: 0x001F0598 File Offset: 0x001EE798
	private void AnimEffectByCached(GameObject cachedEffect)
	{
		this.AnimEffectByCached(cachedEffect, Vector2.zero, null, null);
	}

	// Token: 0x06003DC4 RID: 15812 RVA: 0x001F05AA File Offset: 0x001EE7AA
	private void AnimEffectByCached(GameObject cachedEffect, Action<GameObject> callback)
	{
		this.AnimEffectByCached(cachedEffect, Vector2.zero, null, callback);
	}

	// Token: 0x06003DC5 RID: 15813 RVA: 0x001F05BC File Offset: 0x001EE7BC
	private void AnimEffectByCached(GameObject cachedEffect, Transform root)
	{
		this.AnimEffectByCached(cachedEffect, Vector2.zero, root, null);
	}

	// Token: 0x06003DC6 RID: 15814 RVA: 0x001F05CE File Offset: 0x001EE7CE
	private void AnimEffectByCached(GameObject cachedEffect, Vector2 offset)
	{
		this.AnimEffectByCached(cachedEffect, offset, null, null);
	}

	// Token: 0x06003DC7 RID: 15815 RVA: 0x001F05DC File Offset: 0x001EE7DC
	private void AnimEffectByCached(GameObject cachedEffect, Vector2 offset, Transform root, Action<GameObject> callback = null)
	{
		bool flag = root == null;
		if (flag)
		{
			root = this.animationRoot;
		}
		GameObject go = Object.Instantiate<GameObject>(cachedEffect, root);
		go.SetActive(true);
		RectTransform rt = go.GetComponent<RectTransform>();
		rt.anchoredPosition += offset;
		this._waitingToDestroy.Add(go);
		if (callback != null)
		{
			callback(go);
		}
	}

	// Token: 0x06003DC8 RID: 15816 RVA: 0x001F0644 File Offset: 0x001EE844
	private void ResetAnim()
	{
		foreach (KeyValuePair<Transform, Vector3> kvp in this._originalPositions)
		{
			kvp.Key.position = kvp.Value;
		}
		for (int i = 0; i < this._canvasResets.Length; i++)
		{
			this._canvasResets[i].alpha = 0f;
		}
		for (int j = 0; j < this._maskableGraphics.Length; j++)
		{
			this._maskableGraphics[j].color = this._maskableGraphics[j].color.SetAlpha(0f);
		}
		this.enter.HideAllSkillEffects();
		this.begMoney.gameObject.SetActive(false);
		this.rebirthCricket.gameObject.SetActive(false);
		this.rebirthCricket.CricketView.StopLoopSing();
		this.teammateRise.gameObject.SetActive(false);
		this.propertyChange.gameObject.SetActive(false);
	}

	// Token: 0x06003DC9 RID: 15817 RVA: 0x001F077C File Offset: 0x001EE97C
	private void ResetData()
	{
		base.StopAllCoroutines();
		this._maxAttributes = (this._currAttributes = default(MainAttributes));
		this._changeEventReceived = (this._maxAttributesReceived = (this._currAttributesReceived = false));
		foreach (GameObject o in this._waitingToDestroy)
		{
			Object.Destroy(o);
		}
		this._waitingToDestroy.Clear();
	}

	// Token: 0x06003DCA RID: 15818 RVA: 0x001F0818 File Offset: 0x001EEA18
	private IEnumerable<string> GetComplexConfigAnim(ArgumentBox argBox)
	{
		string[] complex;
		bool flag = !ViewProfessionMask.ProfessionSkill2AnimsComplex.TryGetValue(this._skillTemplateId, out complex);
		if (flag)
		{
			yield break;
		}
		int skillTemplateId = this._skillTemplateId;
		int num = skillTemplateId;
		int num2 = num;
		int money;
		if (num2 != 36)
		{
			if (num2 == 45)
			{
				yield return complex[CommonUtils.GetProfessionVisionLevel(this.Current)];
				goto IL_FB;
			}
		}
		else if (argBox.Get("money", out money))
		{
			yield return (money > 0) ? complex[0] : complex[1];
			goto IL_FB;
		}
		yield break;
		IL_FB:
		yield break;
	}

	// Token: 0x06003DCB RID: 15819 RVA: 0x001F082F File Offset: 0x001EEA2F
	private IEnumerable<string> GetComplexConfigEffect(ArgumentBox argBox)
	{
		string[] complex;
		bool flag = !ViewProfessionMask.ProfessionSkill2EffectsComplex.TryGetValue(this._skillTemplateId, out complex);
		if (flag)
		{
			yield break;
		}
		int skillTemplateId = this._skillTemplateId;
		int num = skillTemplateId;
		int num2 = num;
		bool isFirst;
		if (num2 != 46)
		{
			if (num2 == 68)
			{
				bool success;
				if (argBox.Get("success", out success))
				{
					yield return success ? complex[GameApp.RandomRange(1, complex.Length)] : complex[0];
					goto IL_179;
				}
			}
		}
		else if (argBox.Get("isFirst", out isFirst))
		{
			bool flag2 = isFirst;
			if (flag2)
			{
				yield return complex[0];
				yield return complex[1];
			}
			else
			{
				yield return complex[2];
			}
			goto IL_179;
		}
		yield break;
		IL_179:
		yield break;
	}

	// Token: 0x06003DCC RID: 15820 RVA: 0x001F0846 File Offset: 0x001EEA46
	private IEnumerable<string> GetComplexConfigSeName(ArgumentBox argBox)
	{
		string[] complex;
		bool flag = !ViewProfessionMask.ProfessionSkill2SeNamesComplex.TryGetValue(this._skillTemplateId, out complex);
		if (flag)
		{
			yield break;
		}
		int skillTemplateId = this._skillTemplateId;
		int num = skillTemplateId;
		int num2 = num;
		int money;
		if (num2 != 36)
		{
			if (num2 == 68)
			{
				bool success;
				if (argBox.Get("success", out success))
				{
					yield return success ? complex[0] : complex[1];
					goto IL_11B;
				}
			}
		}
		else if (argBox.Get("money", out money))
		{
			yield return (money > 0) ? complex[0] : complex[1];
			goto IL_11B;
		}
		yield break;
		IL_11B:
		yield break;
	}

	// Token: 0x06003DCD RID: 15821 RVA: 0x001F085D File Offset: 0x001EEA5D
	private static IEnumerator CoAnimSavage1(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		string configEffect = param[0];
		int[][] array = new int[5][];
		int num = 0;
		int[] array2 = new int[2];
		array2[0] = -1;
		array[num] = array2;
		array[1] = new int[]
		{
			0,
			-1
		};
		array[2] = new int[2];
		array[3] = new int[]
		{
			0,
			1
		};
		int num2 = 4;
		int[] array3 = new int[2];
		array3[0] = 1;
		array[num2] = array3;
		int[][] posArr = array;
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		Location originLocation = worldMapModel.CurrentLocation;
		byte areaSize = worldMapModel.GetAreaSize(originLocation.AreaId);
		ByteCoordinate origin = WorldMapModel.IndexToCoordinate(originLocation.BlockId, areaSize);
		int num3;
		for (int i = 0; i < posArr.Length; i = num3 + 1)
		{
			num3 = posArr[i][0];
			int num4 = posArr[i][1];
			int offsetX = num3;
			int offsetY = num4;
			num4 = (int)origin.X + offsetX;
			num3 = (int)origin.Y + offsetY;
			int targetX = num4;
			int targetY = num3;
			bool flag = targetX < 0 || targetY < 0 || targetX >= (int)areaSize || targetY >= (int)areaSize;
			if (!flag)
			{
				MapBlockData block = worldMapModel.GetBlockData(WorldMapModel.CoordinateToIndex(new ByteCoordinate((byte)targetX, (byte)targetY), areaSize));
				bool flag2 = block.TemplateId == 126;
				if (!flag2)
				{
					Vector2 offset = ViewProfessionMask.<CoAnimSavage1>g__ConvertMapOffsetFixed|93_0(mask.mapTileOffset, offsetX, offsetY);
					mask.AnimEffectByConfig(configEffect, offset);
					yield return new WaitForSeconds(ViewProfessionMask.AnimDeltaSecondsDoctor1);
					block = null;
					offset = default(Vector2);
				}
			}
			num3 = i;
		}
		yield break;
	}

	// Token: 0x06003DCE RID: 15822 RVA: 0x001F087A File Offset: 0x001EEA7A
	private static IEnumerator CoAnimMartialArtist3(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		mask.AnimEffectByConfig(param[1], Vector2.zero);
		List<short> blockIds;
		argBox.Get<List<short>>("blocks", out blockIds);
		WorldMapModel worldMap = SingletonObject.getInstance<WorldMapModel>();
		MapBlockData currBlock = worldMap.CurrentBlockData;
		ByteCoordinate posTaiwu = currBlock.GetBlockPos();
		string configEffect = param[0];
		bool flag = blockIds == null;
		if (flag)
		{
			yield break;
		}
		int num;
		for (int i = 0; i < blockIds.Count; i = num + 1)
		{
			ByteCoordinate pos = worldMap.GetBlockData(new Location(currBlock.AreaId, blockIds[i])).GetBlockPos();
			Vector2 offset = ViewProfessionMask.ConvertMapOffset(mask.mapTileOffset, (int)(pos.X - posTaiwu.X), (int)(posTaiwu.Y - pos.Y));
			mask.AnimEffectByConfig(configEffect, offset);
			yield return new WaitForSeconds(ViewProfessionMask.AnimDeltaSecondsDoctor1);
			pos = default(ByteCoordinate);
			offset = default(Vector2);
			num = i;
		}
		yield break;
	}

	// Token: 0x06003DCF RID: 15823 RVA: 0x001F0897 File Offset: 0x001EEA97
	private static IEnumerator CoAnimDuke3(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		mask.AnimEffectByConfig(param[1], Vector2.zero);
		List<short> blockIds;
		argBox.Get<List<short>>("blocks", out blockIds);
		WorldMapModel worldMap = SingletonObject.getInstance<WorldMapModel>();
		MapBlockData currBlock = worldMap.CurrentBlockData;
		ByteCoordinate posTaiwu = currBlock.GetBlockPos();
		string configEffect = param[0];
		List<KeyValuePair<short, float>> blocksWithDistance = new List<KeyValuePair<short, float>>();
		foreach (short blockId in blockIds)
		{
			ByteCoordinate pos = worldMap.GetBlockData(new Location(currBlock.AreaId, blockId)).GetBlockPos();
			float distance = Vector2.Distance(new Vector2((float)posTaiwu.X, (float)posTaiwu.Y), new Vector2((float)pos.X, (float)pos.Y));
			blocksWithDistance.Add(new KeyValuePair<short, float>(blockId, distance));
			pos = default(ByteCoordinate);
		}
		List<short>.Enumerator enumerator = default(List<short>.Enumerator);
		blocksWithDistance.Sort((KeyValuePair<short, float> a, KeyValuePair<short, float> b) => a.Value.CompareTo(b.Value));
		foreach (KeyValuePair<short, float> blockWithDistance in blocksWithDistance)
		{
			short blockId2 = blockWithDistance.Key;
			ByteCoordinate pos2 = worldMap.GetBlockData(new Location(currBlock.AreaId, blockId2)).GetBlockPos();
			Vector2 offset = ViewProfessionMask.ConvertMapOffset(mask.mapTileOffset, (int)(pos2.X - posTaiwu.X), (int)(posTaiwu.Y - pos2.Y));
			mask.AnimEffectByConfig(configEffect, offset);
			yield return new WaitForSeconds(ViewProfessionMask.AnimDeltaSecondsDoctor1);
			pos2 = default(ByteCoordinate);
			offset = default(Vector2);
			blockWithDistance = default(KeyValuePair<short, float>);
		}
		List<KeyValuePair<short, float>>.Enumerator enumerator2 = default(List<KeyValuePair<short, float>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06003DD0 RID: 15824 RVA: 0x001F08B4 File Offset: 0x001EEAB4
	private static IEnumerator CoAnimDoctor1(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		int xLeftUp = 0;
		int xRightDown = 0;
		int yLeftUp = 0;
		int yRightDown = 0;
		WorldMapModel worldMap = SingletonObject.getInstance<WorldMapModel>();
		MapBlockData currBlock = worldMap.CurrentBlockData;
		bool flag = currBlock.IsCityTown();
		if (flag)
		{
			MapBlockData root = (currBlock.RootBlockId == -1) ? currBlock : worldMap.GetBlockData(currBlock.RootBlockId);
			byte size = root.GetConfig().Size;
			ByteCoordinate posRoot = root.GetBlockPos();
			ByteCoordinate posTaiwu = currBlock.GetBlockPos();
			xLeftUp = (int)(posRoot.X - posTaiwu.X);
			yLeftUp = (int)(posRoot.Y - posTaiwu.Y);
			yLeftUp = -yLeftUp - (int)size + 1;
			xRightDown = xLeftUp + (int)size - 1;
			yRightDown = yLeftUp + (int)size - 1;
			root = null;
			posRoot = default(ByteCoordinate);
			posTaiwu = default(ByteCoordinate);
		}
		worldMap = null;
		currBlock = null;
		string configEffect = param[0];
		int num;
		for (int x = xLeftUp; x <= xRightDown; x = num + 1)
		{
			for (int y = yLeftUp; y <= yRightDown; y = num + 1)
			{
				Vector2 offset = ViewProfessionMask.ConvertMapOffset(mask.mapTileOffset, x, y);
				mask.AnimEffectByConfig(configEffect, offset);
				yield return new WaitForSeconds(ViewProfessionMask.AnimDeltaSecondsDoctor1);
				offset = default(Vector2);
				num = y;
			}
			num = x;
		}
		yield break;
	}

	// Token: 0x06003DD1 RID: 15825 RVA: 0x001F08D1 File Offset: 0x001EEAD1
	private static IEnumerator CoAnimTraveler2(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		ViewProfessionMask.<>c__DisplayClass97_0 CS$<>8__locals1;
		CS$<>8__locals1.mask = mask;
		CS$<>8__locals1.param = param;
		CS$<>8__locals1.seNames = ViewProfessionMask.ProfessionSkill2SeNamesComplex[CS$<>8__locals1.mask._skillTemplateId];
		bool isFirst;
		argBox.Get("isFirst", out isFirst);
		bool flag = isFirst;
		if (flag)
		{
			ViewProfessionMask.<CoAnimTraveler2>g__PlayEffectAndSound|97_0(1, ref CS$<>8__locals1);
			yield return new WaitForSeconds(0.5f);
			ViewProfessionMask.<CoAnimTraveler2>g__PlayEffectAndSound|97_0(0, ref CS$<>8__locals1);
		}
		else
		{
			ViewProfessionMask.<CoAnimTraveler2>g__PlayEffectAndSound|97_0(2, ref CS$<>8__locals1);
		}
		yield break;
	}

	// Token: 0x06003DD2 RID: 15826 RVA: 0x001F08EE File Offset: 0x001EEAEE
	[Obsolete]
	private static IEnumerator CoAnimBeggar0(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		int money;
		bool flag = !argBox.Get("money", out money);
		if (flag)
		{
			throw new Exception("技能未传入必要参数 money");
		}
		ProfessionMaskBegMoney begMoney = mask.begMoney;
		begMoney.gameObject.SetActive(true);
		CanvasGroup canvasRoot = begMoney.ValueRoot;
		canvasRoot.DOFade(1f, 0.3f).SetDelay(0.5f);
		canvasRoot.DOFade(0f, 0.3f).SetDelay(1.7f);
		mask.AnimSpineByConfig(param[0]);
		mask.SetBegMoney(0);
		yield return new WaitForSeconds(0.5f);
		int num;
		for (int i = 1; i <= 30; i = num + 1)
		{
			mask.SetBegMoney(money * i / 30);
			yield return null;
			num = i;
		}
		yield break;
	}

	// Token: 0x06003DD3 RID: 15827 RVA: 0x001F090B File Offset: 0x001EEB0B
	private static IEnumerator CoAnimDuke2(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox)
	{
		ItemDisplayData displayData;
		bool flag = !argBox.Get<ItemDisplayData>("DisplayData", out displayData);
		if (flag)
		{
			throw new Exception("技能未传入必要参数 DisplayData");
		}
		ProfessionMaskRebirthCricket animBase = mask.rebirthCricket;
		ParticleSystem animEffect = animBase.Effect;
		CricketView animCricket = animBase.CricketView;
		animBase.gameObject.SetActive(true);
		animEffect.Play();
		animCricket.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		animCricket.SetCricketData(displayData.CricketColorId, displayData.CricketPartId, false, displayData, false);
		animCricket.gameObject.SetActive(true);
		animCricket.Sing(false, true, true, -1f, null, 0f);
		yield return new WaitWhile(() => animCricket.IsSinging);
		animCricket.StopSing(0.1f);
		yield return new WaitForSeconds(0.1f);
		yield break;
	}

	// Token: 0x06003DD4 RID: 15828 RVA: 0x001F0928 File Offset: 0x001EEB28
	private static Vector2 ConvertMapOffset(Vector2 unit, int x, int y)
	{
		return new Vector2(unit.x * (float)(x - y), -unit.y * (float)(x + y));
	}

	// Token: 0x06003DD5 RID: 15829 RVA: 0x001F0958 File Offset: 0x001EEB58
	private static int GetConfigSkill(int templateId, int index)
	{
		bool flag = index < 0 || index > 3;
		if (flag)
		{
			throw new ArgumentOutOfRangeException(string.Format("Profession skill index out of range {0}", index));
		}
		ProfessionItem config = Profession.Instance[templateId];
		bool flag2 = config == null;
		if (flag2)
		{
			throw new Exception(string.Format("Config missing, template id {0} not exist in Profession", templateId));
		}
		return (index == 3) ? config.ExtraProfessionSkill : config.ProfessionSkills[index];
	}

	// Token: 0x06003DD6 RID: 15830 RVA: 0x001F09D0 File Offset: 0x001EEBD0
	private static string GetConfigTexture(ProfessionData profession)
	{
		string result;
		if (profession != null)
		{
			ProfessionItem professionItem = Profession.Instance[profession.TemplateId];
			result = (((professionItem != null) ? professionItem.TextureBig : null) ?? string.Empty);
		}
		else
		{
			result = string.Empty;
		}
		return result;
	}

	// Token: 0x06003DD7 RID: 15831 RVA: 0x001F0A11 File Offset: 0x001EEC11
	private static IEnumerable<string> ConvertNumberSprites(int number)
	{
		string strNum = number.ToString();
		foreach (char c in strNum)
		{
			yield return "sp_number_0_" + c;
		}
		string text = null;
		yield break;
	}

	// Token: 0x06003DD8 RID: 15832 RVA: 0x001F0A21 File Offset: 0x001EEC21
	private static IEnumerable<string> ConvertPropertySprites(int current, int max)
	{
		foreach (string sprite in ViewProfessionMask.ConvertNumberSprites(current))
		{
			yield return sprite;
			sprite = null;
		}
		IEnumerator<string> enumerator = null;
		yield return "sp_number_0_10";
		foreach (string sprite2 in ViewProfessionMask.ConvertNumberSprites(max))
		{
			yield return sprite2;
			sprite2 = null;
		}
		IEnumerator<string> enumerator2 = null;
		yield break;
		yield break;
	}

	// Token: 0x06003DDE RID: 15838 RVA: 0x001F10C4 File Offset: 0x001EF2C4
	[CompilerGenerated]
	internal static Vector2 <CoAnimSavage1>g__ConvertMapOffsetFixed|93_0(Vector2 unit, int x, int y)
	{
		return new Vector2(unit.x * (float)(x + y), -unit.y * (float)(x - y));
	}

	// Token: 0x06003DDF RID: 15839 RVA: 0x001F10F2 File Offset: 0x001EF2F2
	[CompilerGenerated]
	internal static void <CoAnimTraveler2>g__PlayEffectAndSound|97_0(int index, ref ViewProfessionMask.<>c__DisplayClass97_0 A_1)
	{
		A_1.mask.AnimEffectByConfig(A_1.param[index]);
		AudioManager.Instance.PlaySound(A_1.seNames[index], false, false);
	}

	// Token: 0x04002C55 RID: 11349
	private const string ProfessionTextureDirectory = "RemakeResources/Textures/Profession";

	// Token: 0x04002C56 RID: 11350
	private const string ProfessionSkillAnimPath = "RemakeResources/Prefab/Legacy/Core/Profession/profession_";

	// Token: 0x04002C57 RID: 11351
	private const string ProfessionSkillEffectPath = "RemakeResources/Particle/UIEffectPrefabs/ProfessionSkill/eff_professionskill_";

	// Token: 0x04002C58 RID: 11352
	private const string ProfessionPropertyValueTextPrefix = "sp_number_0_";

	// Token: 0x04002C59 RID: 11353
	private const string ProfessionPropertyValueTextSplit = "sp_number_0_10";

	// Token: 0x04002C5A RID: 11354
	private const string PropertyChangedEffectName = "shanren1";

	// Token: 0x04002C5B RID: 11355
	private const string PropertyChangedSeName = "animation_occupation_wildman_1";

	// Token: 0x04002C5C RID: 11356
	private static readonly float AnimDeltaSecondsDoctor1 = 0.2f;

	// Token: 0x04002C5D RID: 11357
	private static readonly Dictionary<int, string> ProfessionSkill2Anims = new Dictionary<int, string>
	{
		{
			3,
			"shanren4"
		},
		{
			6,
			"liehu1"
		},
		{
			9,
			"jiangren2"
		},
		{
			14,
			"wushi3"
		},
		{
			17,
			"caijun2"
		},
		{
			18,
			"jinkou"
		},
		{
			33,
			"mingmen2"
		},
		{
			38,
			"qigai3"
		},
		{
			61,
			"fushang2"
		},
		{
			67,
			"caijun2"
		},
		{
			31,
			"wushi3"
		}
	};

	// Token: 0x04002C5E RID: 11358
	private static readonly Dictionary<int, string> ProfessionSkill2Effects = new Dictionary<int, string>
	{
		{
			4,
			"liehu2"
		},
		{
			23,
			"daozhang4"
		},
		{
			26,
			"gaoseng3"
		},
		{
			27,
			"gaoseng4"
		},
		{
			41,
			"pingmin2"
		},
		{
			50,
			"yunyouseng4"
		},
		{
			18,
			"caijun3"
		},
		{
			19,
			"caijun4"
		},
		{
			62,
			"fushang3"
		},
		{
			37,
			"qigai"
		},
		{
			35,
			"qiufan"
		},
		{
			55,
			"jinzhenduming"
		}
	};

	// Token: 0x04002C5F RID: 11359
	private static readonly Dictionary<int, string> ProfessionSkill2SeNames = new Dictionary<int, string>
	{
		{
			0,
			"animation_occupation_wildman_1"
		},
		{
			44,
			"animation_occupation_wildman_1"
		},
		{
			24,
			"animation_occupation_wildman_1"
		},
		{
			20,
			"animation_occupation_wildman_1"
		},
		{
			48,
			"animation_occupation_wildman_1"
		},
		{
			56,
			"animation_occupation_wildman_1"
		},
		{
			1,
			"animation_occupation_wildman_2"
		},
		{
			6,
			"animation_occupation_hunter_1"
		},
		{
			4,
			"animation_occupation_hunter_2"
		},
		{
			9,
			"animation_occupation_craftsman_2"
		},
		{
			14,
			"animation_occupation_warrior_3"
		},
		{
			17,
			"animation_occupation_talented_2"
		},
		{
			23,
			"animation_occupation_taoist_4"
		},
		{
			26,
			"animation_occupation_monk_3"
		},
		{
			27,
			"animation_occupation_monk_4"
		},
		{
			33,
			"animation_occupation_famous_2"
		},
		{
			38,
			"animation_occupation_pauper_3"
		},
		{
			41,
			"animation_occupation_populace_2"
		},
		{
			45,
			"animation_occupation_traveller_2"
		},
		{
			50,
			"animation_occupation_wandering_4"
		},
		{
			53,
			"animation_occupation_doctor_2"
		},
		{
			61,
			"animation_occupation_rich_2"
		},
		{
			62,
			"animation_occupation_rich_3"
		},
		{
			37,
			"SFX_professionskill_qigai"
		},
		{
			35,
			"SFX_professionskill_mingmen"
		},
		{
			3,
			"SFX_professionskill_shanren4"
		},
		{
			47,
			"SFX_professionskill_lvren_play"
		},
		{
			71,
			"SFX_professionskill_wanggong"
		},
		{
			18,
			"SFX_professionskill_caijun3"
		},
		{
			19,
			"SFX_professionskill_caijun4"
		},
		{
			67,
			"SFX_professionskill_guike4"
		},
		{
			31,
			"SFX_professionskill_haoke4"
		},
		{
			15,
			"SFX_professionskill_wushi4"
		}
	};

	// Token: 0x04002C60 RID: 11360
	private static readonly Dictionary<int, string[]> ProfessionSkill2AnimsComplex = new Dictionary<int, string[]>
	{
		{
			36,
			new string[]
			{
				"qigai1_getting",
				"qigai1_nothing"
			}
		},
		{
			45,
			new string[]
			{
				"lvren2_level1",
				"lvren2_level2",
				"lvren2_level3"
			}
		}
	};

	// Token: 0x04002C61 RID: 11361
	private static readonly Dictionary<int, string[]> ProfessionSkill2EffectsComplex = new Dictionary<int, string[]>
	{
		{
			68,
			new string[]
			{
				"wanggong1_shibai",
				"wanggong1_you",
				"wanggong1_zuo"
			}
		}
	};

	// Token: 0x04002C62 RID: 11362
	private static readonly Dictionary<int, string[]> ProfessionSkill2SeNamesComplex = new Dictionary<int, string[]>
	{
		{
			46,
			new string[]
			{
				"animation_occupation_traveller_3_0",
				"animation_occupation_traveller_3_1",
				"animation_occupation_traveller_3_2"
			}
		},
		{
			68,
			new string[]
			{
				"animation_occupation_princes_1_1",
				"animation_occupation_princes_1_2"
			}
		},
		{
			36,
			new string[]
			{
				"animation_occupation_pauper_1_1",
				"animation_occupation_pauper_1_2"
			}
		}
	};

	// Token: 0x04002C63 RID: 11363
	[TupleElementNames(new string[]
	{
		"coAnim",
		"coParam"
	})]
	private static readonly Dictionary<int, ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>> ProfessionSkill2Coroutine = new Dictionary<int, ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>>
	{
		{
			53,
			new ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>(new ViewProfessionMask.CoAnimEffect(ViewProfessionMask.CoAnimDoctor1), new string[]
			{
				"daifu2"
			})
		},
		{
			1,
			new ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>(new ViewProfessionMask.CoAnimEffect(ViewProfessionMask.CoAnimSavage1), new string[]
			{
				"shanren2"
			})
		},
		{
			46,
			new ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>(new ViewProfessionMask.CoAnimEffect(ViewProfessionMask.CoAnimTraveler2), new string[]
			{
				"lvren3_chuxian",
				"lvren3_yindao",
				"lvren3_zha"
			})
		},
		{
			70,
			new ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>(new ViewProfessionMask.CoAnimEffect(ViewProfessionMask.CoAnimDuke2), new string[0])
		},
		{
			71,
			new ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>(new ViewProfessionMask.CoAnimEffect(ViewProfessionMask.CoAnimDuke3), new string[]
			{
				"wanggong3",
				"wanggong4"
			})
		},
		{
			15,
			new ValueTuple<ViewProfessionMask.CoAnimEffect, string[]>(new ViewProfessionMask.CoAnimEffect(ViewProfessionMask.CoAnimMartialArtist3), new string[]
			{
				"wushi4",
				"wushi5"
			})
		}
	};

	// Token: 0x04002C64 RID: 11364
	private static readonly Dictionary<int, sbyte> ProfessionSkill2Property = new Dictionary<int, sbyte>
	{
		{
			0,
			0
		},
		{
			20,
			4
		},
		{
			24,
			2
		},
		{
			44,
			3
		},
		{
			48,
			5
		},
		{
			56,
			1
		}
	};

	// Token: 0x04002C65 RID: 11365
	private const float AnimDuration = 0.3f;

	// Token: 0x04002C66 RID: 11366
	[SerializeField]
	private Vector2 propertySpriteSize = new Vector2(50f, 66f);

	// Token: 0x04002C67 RID: 11367
	[SerializeField]
	private Vector2 begMoneySpriteSize = new Vector2(45f, 59f);

	// Token: 0x04002C68 RID: 11368
	[SerializeField]
	private Vector2 mapTileOffset = new Vector2(315f, 165f);

	// Token: 0x04002C69 RID: 11369
	[SerializeField]
	private ProfessionMaskEnterPanel enter;

	// Token: 0x04002C6A RID: 11370
	[SerializeField]
	private ProfessionMaskBegMoney begMoney;

	// Token: 0x04002C6B RID: 11371
	[SerializeField]
	private ProfessionMaskRebirthCricket rebirthCricket;

	// Token: 0x04002C6C RID: 11372
	[SerializeField]
	private ProfessionMaskTeammateRise teammateRise;

	// Token: 0x04002C6D RID: 11373
	[SerializeField]
	private ProfessionMaskPropertyChange propertyChange;

	// Token: 0x04002C6E RID: 11374
	[SerializeField]
	private RectTransform animationRoot;

	// Token: 0x04002C6F RID: 11375
	private int _skillIndex;

	// Token: 0x04002C70 RID: 11376
	private int _skillTemplateId;

	// Token: 0x04002C71 RID: 11377
	private int _professionId;

	// Token: 0x04002C72 RID: 11378
	private MainAttributes _currAttributes;

	// Token: 0x04002C73 RID: 11379
	private MainAttributes _maxAttributes;

	// Token: 0x04002C74 RID: 11380
	private bool _currAttributesReceived;

	// Token: 0x04002C75 RID: 11381
	private bool _maxAttributesReceived;

	// Token: 0x04002C76 RID: 11382
	private bool _changeEventReceived;

	// Token: 0x04002C77 RID: 11383
	private Sequence _showAndHideSeq;

	// Token: 0x04002C78 RID: 11384
	private Sequence _hidingAnimSeq;

	// Token: 0x04002C79 RID: 11385
	private readonly Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

	// Token: 0x04002C7A RID: 11386
	private readonly Dictionary<string, GameObject> _cachedAnims = new Dictionary<string, GameObject>();

	// Token: 0x04002C7B RID: 11387
	private readonly Dictionary<string, GameObject> _cachedEffects = new Dictionary<string, GameObject>();

	// Token: 0x04002C7C RID: 11388
	private readonly List<GameObject> _waitingToDestroy = new List<GameObject>();

	// Token: 0x04002C7D RID: 11389
	private readonly List<GameObject> _propertyValueCache = new List<GameObject>();

	// Token: 0x04002C7E RID: 11390
	private readonly List<GameObject> _begMoneyCache = new List<GameObject>();

	// Token: 0x04002C7F RID: 11391
	private Vector3 _defaultAppearancePosition;

	// Token: 0x04002C80 RID: 11392
	private Vector3 _defaultSkillGroupPosition;

	// Token: 0x04002C81 RID: 11393
	private Dictionary<Transform, Vector3> _originalPositions;

	// Token: 0x04002C82 RID: 11394
	private CanvasGroup[] _canvasResets;

	// Token: 0x04002C83 RID: 11395
	private MaskableGraphic[] _maskableGraphics;

	// Token: 0x02001899 RID: 6297
	// (Invoke) Token: 0x0600D728 RID: 55080
	private delegate IEnumerator CoAnimEffect(ViewProfessionMask mask, IReadOnlyList<string> param, ArgumentBox argBox);
}
