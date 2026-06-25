using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using Config.Common;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000936 RID: 2358
	public class MapBlockChar : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x17000CB8 RID: 3256
		// (get) Token: 0x06006DED RID: 28141 RVA: 0x0032C5FB File Offset: 0x0032A7FB
		// (set) Token: 0x06006DEE RID: 28142 RVA: 0x0032C608 File Offset: 0x0032A808
		public bool Interactable
		{
			get
			{
				return this.btn.interactable;
			}
			set
			{
				this.btn.interactable = value;
			}
		}

		// Token: 0x06006DEF RID: 28143 RVA: 0x0032C617 File Offset: 0x0032A817
		private void Awake()
		{
			this.btn.onClick.ResetListener(new Action(this.OnClick));
		}

		// Token: 0x06006DF0 RID: 28144 RVA: 0x0032C638 File Offset: 0x0032A838
		private void ChangeLanguageType(ArgumentBox _)
		{
			this.nameTextHelper.OnLanguageChange(this.CurLanguageType = LocalStringManager.CurLanguageType);
			this.gradeComponent.OnLanguageChange(this.CurLanguageType);
		}

		// Token: 0x06006DF1 RID: 28145 RVA: 0x0032C674 File Offset: 0x0032A874
		private void PostProcess(bool activeNameIcon, bool setEmptyBg = true)
		{
			foreach (GameObject go in this.deadBase)
			{
				go.SetActive(true);
			}
			foreach (GameObject icon in this.nameIcons)
			{
				icon.SetActive(activeNameIcon);
			}
			bool flag = setEmptyBg && this.mapBlockCharBg;
			if (flag)
			{
				this.mapBlockCharBg.SetBg(null, null);
			}
			this.ChangeLanguageType(null);
		}

		// Token: 0x06006DF2 RID: 28146 RVA: 0x0032C6FA File Offset: 0x0032A8FA
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		public static ValueTuple<string, string, int> GetTextData(CharacterDisplayData charDisplayData)
		{
			return new ValueTuple<string, string, int>(NameCenter.GetMonasticTitleOrDisplayName(charDisplayData, false), CommonUtils.GetOrganizationGradeString(charDisplayData.OrgInfo, charDisplayData.Gender, charDisplayData.PhysiologicalAge, (int)charDisplayData.TemplateId), (int)charDisplayData.OrgInfo.OrgTemplateId);
		}

		// Token: 0x06006DF3 RID: 28147 RVA: 0x0032C730 File Offset: 0x0032A930
		public virtual void Set(IMapBlockCharHolder parent, CharacterDisplayData charDisplayData, bool isSpecialNpc, bool isActive = true)
		{
			this._version++;
			this.type = (isSpecialNpc ? DisplayType.NormalSpecial : DisplayType.NormalCharacter);
			this._parent = parent;
			bool flag = this.mapBlockCharBg;
			if (flag)
			{
				this.mapBlockCharBg.SetBg(charDisplayData, this._parent.LoongInfos);
			}
			this.btn.interactable = true;
			bool flag2;
			if (charDisplayData.CreatingType == 1)
			{
				CharacterItem characterItem = Character.Instance[charDisplayData.TemplateId];
				flag2 = (characterItem == null || characterItem.CanOpenCharacterMenu);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				this.displayer.enabled = true;
				TooltipInvoker tooltipInvoker = this.displayer;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.Set("CharId", charDisplayData.CharacterId).Set("IsMapBlockCharList", true);
			}
			else
			{
				this.displayer.enabled = false;
			}
			this.CharId = charDisplayData.CharacterId;
			this.RefreshAiActionToolTip(charDisplayData.CreatingType == 1);
			this.stat.Set(charDisplayData, isSpecialNpc);
			bool isHeavenlyTree = charDisplayData.TemplateId >= 598 && charDisplayData.TemplateId <= 602;
			this.gradeComponent.gameObject.SetActive(!isHeavenlyTree);
			if (isSpecialNpc)
			{
				this.charAvatar.Refresh(charDisplayData.AvatarRelatedData, charDisplayData.TemplateId);
			}
			else
			{
				this.charAvatar.Refresh(charDisplayData, true);
			}
			TMP_Text tmp_Text = this.nameText;
			ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(charDisplayData);
			tmp_Text.text = textData.Item1;
			string text = textData.Item2;
			this.charAvatar.gameObject.SetActive(true);
			this.nameIcon.sprite = this.favors[(int)(FavorabilityType.GetFavorabilityType(charDisplayData.FavorabilityToTaiwu) + 6)];
			this.gradeComponent.Set(text, (int)charDisplayData.OrgInfo.Grade);
			this.RefreshJieqingSign(charDisplayData.CharacterId, charDisplayData.ConsummateLevel);
			this.PostProcess(isActive, false);
			bool flag4 = !isActive;
			if (flag4)
			{
				this.gradeComponent.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006DF4 RID: 28148 RVA: 0x0032C95C File Offset: 0x0032AB5C
		public void SetGuard(bool show, bool hasGuard)
		{
			this.guardIcon.gameObject.SetActive(false);
		}

		// Token: 0x06006DF5 RID: 28149 RVA: 0x0032C980 File Offset: 0x0032AB80
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		public static ValueTuple<string, string, int> GetTextData(CaravanDisplayData caravanDisplayData)
		{
			string item = LanguageKey.LK_Caravan.Tr();
			ConfigData<MerchantTypeItem, sbyte> instance = Config.MerchantType.Instance;
			MerchantItem merchantItem = Merchant.Instance[(int)caravanDisplayData.MerchantTemplateId];
			MerchantTypeItem merchantTypeItem = instance[(merchantItem != null) ? merchantItem.MerchantType : -1];
			return new ValueTuple<string, string, int>(item, ((merchantTypeItem != null) ? merchantTypeItem.Name : null) ?? "", 0);
		}

		// Token: 0x06006DF6 RID: 28150 RVA: 0x0032C9D8 File Offset: 0x0032ABD8
		public void Set(IMapBlockCharHolder parent, CaravanDisplayData caravanDisplayData)
		{
			this._version++;
			this.type = DisplayType.Caravan;
			this._parent = parent;
			this.btn.interactable = true;
			this.displayer.enabled = false;
			this.CharId = caravanDisplayData.CaravanId;
			this.RefreshAiActionToolTip(false);
			this.stat.Set(caravanDisplayData);
			MerchantItem merchantCfg = Merchant.Instance[(int)caravanDisplayData.MerchantTemplateId];
			TMP_Text tmp_Text = this.nameText;
			ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(caravanDisplayData);
			tmp_Text.text = textData.Item1;
			string text = textData.Item2;
			this.gradeComponent.Set(text, -1);
			MerchantTypeItem merchantType = Config.MerchantType.Instance[(merchantCfg != null) ? merchantCfg.MerchantType : -1];
			bool flag = merchantType != null;
			if (flag)
			{
				int version = this._version;
				ResLoader.LoadModOrGameResource<Texture2D>("NpcFace/SmallFace/" + merchantType.CaravanAvatar, delegate(Texture2D texture)
				{
					bool flag2 = this._version != version;
					if (!flag2)
					{
						this.charAvatar.Refresh(texture);
						this.charAvatar.gameObject.SetActive(true);
					}
				}, null);
			}
			else
			{
				Debug.LogWarning(string.Format("invalid caravanDisplayData.MerchantTemplateId: {0}", caravanDisplayData.MerchantTemplateId));
			}
			this.HideJieqingSign();
			this.PostProcess(false, true);
		}

		// Token: 0x06006DF7 RID: 28151 RVA: 0x0032CB10 File Offset: 0x0032AD10
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		public static ValueTuple<string, string, int> GetTextData(GraveDisplayData graveDisplayData)
		{
			return new ValueTuple<string, string, int>(NameCenter.GetMonasticTitleOrDisplayName(graveDisplayData, false), CommonUtils.GetOrganizationGradeString(new OrganizationInfo(graveDisplayData.NameData.OrgTemplateId, graveDisplayData.NameData.OrgGrade, graveDisplayData.Principal, graveDisplayData.OrgSettlementId), graveDisplayData.NameData.Gender, -1, -1), (int)graveDisplayData.NameData.OrgTemplateId);
		}

		// Token: 0x06006DF8 RID: 28152 RVA: 0x0032CB70 File Offset: 0x0032AD70
		public void Set(IMapBlockCharHolder parent, GraveDisplayData graveDisplayData)
		{
			this._version++;
			this._parent = parent;
			this.btn.interactable = true;
			this.displayer.enabled = false;
			this.CharId = ((graveDisplayData != null) ? graveDisplayData.Id : -1);
			this.RefreshAiActionToolTip(false);
			this.HideJieqingSign();
			this.type = DisplayType.Grave;
			this.charAvatar.Refresh(this.tomb);
			bool flag = this.CharId == -1;
			if (flag)
			{
				this.stat.SetDead(-1, -1, false);
				foreach (GameObject go in this.deadBase)
				{
					go.SetActive(false);
				}
			}
			else
			{
				this.stat.Set(graveDisplayData);
				this.nameIcon.sprite = this.favors[(int)(FavorabilityType.GetFavorabilityType(graveDisplayData.FavorabilityToTaiwu) + 6)];
				TMP_Text tmp_Text = this.nameText;
				ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(graveDisplayData);
				tmp_Text.text = textData.Item1;
				string text = textData.Item2;
				this.gradeComponent.Set(text, (int)graveDisplayData.NameData.OrgGrade);
				this.PostProcess(true, true);
			}
		}

		// Token: 0x06006DF9 RID: 28153 RVA: 0x0032CCA0 File Offset: 0x0032AEA0
		public void SetAsGrave(IMapBlockCharHolder parent, CharacterDisplayData graveDisplayData)
		{
			bool flag = graveDisplayData.AliveState == 0;
			if (flag)
			{
				this.Set(parent, graveDisplayData, false, true);
			}
			else
			{
				this._version++;
				this._parent = parent;
				this.btn.interactable = true;
				this.displayer.enabled = false;
				this.CharId = graveDisplayData.CharacterId;
				this.RefreshAiActionToolTip(false);
				this.HideJieqingSign();
				this.type = DisplayType.Grave;
				this.charAvatar.Refresh(this.tomb);
				bool flag2 = this.CharId == -1;
				if (flag2)
				{
					this.stat.SetDead(-1, -1, false);
				}
				else
				{
					this.stat.Set(graveDisplayData, false);
					this.nameIcon.sprite = this.favors[(int)(FavorabilityType.GetFavorabilityType(graveDisplayData.FavorabilityToTaiwu) + 6)];
					TMP_Text tmp_Text = this.nameText;
					ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(graveDisplayData);
					tmp_Text.text = textData.Item1;
					string text = textData.Item2;
					this.gradeComponent.Set(text, (int)graveDisplayData.OrgInfo.Grade);
					this.PostProcess(true, true);
				}
			}
		}

		// Token: 0x06006DFA RID: 28154 RVA: 0x0032CDC0 File Offset: 0x0032AFC0
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		public static ValueTuple<string, string, int> GetTextData(GameData.Domains.Character.Animal animalData)
		{
			CharacterItem cfg = Character.Instance[animalData.CharacterTemplateId];
			return (cfg != null) ? new ValueTuple<string, string, int>(cfg.Surname + cfg.GivenName, CommonUtils.GetOrganizationGradeString(cfg.OrganizationInfo, cfg.Gender, -1, -1), 0) : new ValueTuple<string, string, int>("", "", 0);
		}

		// Token: 0x06006DFB RID: 28155 RVA: 0x0032CE20 File Offset: 0x0032B020
		public void Set(IMapBlockCharHolder parent, GameData.Domains.Character.Animal animalData)
		{
			this._version++;
			this.type = DisplayType.EnemyAnimal;
			this._parent = parent;
			this.btn.interactable = true;
			this.displayer.enabled = false;
			this.type = DisplayType.EnemyAnimal;
			this.CharId = animalData.Id;
			this.RefreshAiActionToolTip(false);
			this.stat.Set(animalData);
			this.charAvatar.gameObject.SetActive(true);
			this.charAvatar.Refresh(null, animalData.CharacterTemplateId);
			TMP_Text tmp_Text = this.nameText;
			ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(animalData);
			tmp_Text.text = textData.Item1;
			string text = textData.Item2;
			CharacterItem cfg = Character.Instance[animalData.CharacterTemplateId];
			this.gradeComponent.Set(text, (int)((cfg != null) ? cfg.OrganizationInfo.Grade : 0));
			this.HideJieqingSign();
			this.PostProcess(false, true);
		}

		// Token: 0x06006DFC RID: 28156 RVA: 0x0032CF1C File Offset: 0x0032B11C
		[return: TupleElementNames(new string[]
		{
			"name",
			"grade",
			"orgTemplateId"
		})]
		public static ValueTuple<string, string, int> GetTextData(MapTemplateEnemyInfo enemyInfo)
		{
			CharacterItem cfg = Character.Instance[enemyInfo.TemplateId];
			return (cfg != null) ? new ValueTuple<string, string, int>(GameData.Domains.World.SharedMethods.SmallVillageXiangshu((short)cfg.OrganizationInfo.OrgTemplateId, false) ? CommonUtils.GetXiangshuMinion0AnonymousTitle() : (cfg.Surname + cfg.GivenName), CommonUtils.GetOrganizationGradeString(cfg.OrganizationInfo, cfg.Gender, -1, -1), 0) : new ValueTuple<string, string, int>("", "", 0);
		}

		// Token: 0x06006DFD RID: 28157 RVA: 0x0032CF94 File Offset: 0x0032B194
		public void Set(IMapBlockCharHolder parent, MapTemplateEnemyInfo enemyInfo)
		{
			this._version++;
			this.type = DisplayType.EnemyTemplate;
			CharacterItem cfg = Character.Instance[enemyInfo.TemplateId];
			bool flag = cfg != null;
			if (flag)
			{
				this._parent = parent;
				this.btn.interactable = true;
				this.displayer.enabled = false;
				this.stat.Set(enemyInfo);
				this.CharId = -1;
				this.RefreshAiActionToolTip(false);
				int version = this._version;
				ResLoader.LoadModOrGameResource<Texture2D>("NpcFace/SmallFace/" + cfg.FixedAvatarName, delegate(Texture2D texture)
				{
					bool flag2 = this._version != version;
					if (!flag2)
					{
						this.charAvatar.Refresh(texture);
						this.charAvatar.gameObject.SetActive(true);
					}
				}, null);
				this.CharId = (int)cfg.TemplateId;
				TMP_Text tmp_Text = this.nameText;
				ValueTuple<string, string, int> textData = MapBlockChar.GetTextData(enemyInfo);
				tmp_Text.text = textData.Item1;
				string text = textData.Item2;
				this.gradeComponent.Set(text, (int)cfg.OrganizationInfo.Grade);
				this.HideJieqingSign();
				this.PostProcess(false, true);
			}
			else
			{
				Debug.LogWarning(string.Format("invalid enemyInfo.TemplateId: {0}", enemyInfo.TemplateId));
				this.SetUnknown();
			}
		}

		// Token: 0x06006DFE RID: 28158 RVA: 0x0032D0D4 File Offset: 0x0032B2D4
		public void SetUnknown()
		{
			this._version++;
			this._parent = null;
			this.btn.interactable = false;
			this.displayer.enabled = false;
			this.charAvatar.ResetToBlank(false);
			this.stat.SetEmpty();
			this.nameText.text = LanguageKey.LK_Unknown_Character_Name.Tr();
			this.gradeComponent.Set(LanguageKey.LK_UnknownCharName.Tr(), -1);
			this.HideJieqingSign();
			this.PostProcess(false, true);
			this.RefreshAiActionToolTip(false);
		}

		// Token: 0x06006DFF RID: 28159 RVA: 0x0032D170 File Offset: 0x0032B370
		public void OnClick()
		{
			IMapBlockCharHolder parent = this._parent;
			bool flag = parent == null || !parent.CanClick(this.type, this.CharId);
			if (!flag)
			{
				this._parent.OnClick(this.type, this.CharId);
			}
		}

		// Token: 0x06006E00 RID: 28160 RVA: 0x0032D1C0 File Offset: 0x0032B3C0
		public void OnSelfClickBegin()
		{
			IMapBlockCharHolder parent = this._parent;
			bool flag = parent != null && parent.CanClick(this.type, this.CharId);
			if (flag)
			{
				if (this._cached == null)
				{
					this._cached = this.gradeComponent.gradeText.text;
				}
				this.baseBg.sprite = this.clickBg;
			}
			else
			{
				this.baseBg.sprite = this.hoverBg;
			}
		}

		// Token: 0x06006E01 RID: 28161 RVA: 0x0032D238 File Offset: 0x0032B438
		public void OnClickDone()
		{
			bool flag = this._cached != null;
			if (flag)
			{
				this.gradeComponent.gradeText.text = this._cached;
				this._cached = null;
			}
			this.baseBg.sprite = this.normalBg;
		}

		// Token: 0x06006E02 RID: 28162 RVA: 0x0032D285 File Offset: 0x0032B485
		public void OnHover()
		{
			IMapBlockCharHolder parent = this._parent;
			if (parent != null)
			{
				parent.OnHover(this.rectTransform, this);
			}
		}

		// Token: 0x06006E03 RID: 28163 RVA: 0x0032D2A0 File Offset: 0x0032B4A0
		public void OnHoverEnd()
		{
			IMapBlockCharHolder parent = this._parent;
			if (parent != null)
			{
				parent.OnChildHoverEnd();
			}
			IMapBlockCharHolder parent2 = this._parent;
			if (parent2 != null)
			{
				parent2.OnChildHoverEnd(this);
			}
		}

		// Token: 0x06006E04 RID: 28164 RVA: 0x0032D2C8 File Offset: 0x0032B4C8
		private void RefreshAiActionToolTip(bool showTooltip)
		{
			bool flag = this.aiActionTooltip == null;
			if (!flag)
			{
				this.aiActionTooltip.gameObject.SetActive(showTooltip);
				this.aiActionTooltip.Type = TipType.AiAction;
				TooltipInvoker tooltipInvoker = this.aiActionTooltip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.aiActionTooltip.RuntimeParam.Set("charId", this.CharId);
			}
		}

		// Token: 0x06006E05 RID: 28165 RVA: 0x0032D344 File Offset: 0x0032B544
		private void HideJieqingSign()
		{
			bool flag = this.jieqingSign;
			if (flag)
			{
				this.jieqingSign.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006E06 RID: 28166 RVA: 0x0032D374 File Offset: 0x0032B574
		private void RefreshJieqingSign(int charId, sbyte consummateLevel)
		{
			bool flag = !this.jieqingSign;
			if (!flag)
			{
				this.jieqingSign.gameObject.SetActive(false);
				int version = this._version;
				ExtraDomainMethod.AsyncCall.GetCharacterExtraLegacyPointWorthForMapBlock(null, charId, delegate(int offset, RawDataPool dataPool)
				{
					bool flag2 = this._version != version || !this.isActiveAndEnabled;
					if (!flag2)
					{
						int worth = 0;
						Serializer.Deserialize(dataPool, offset, ref worth);
						bool flag3 = !CommonUtils.IsCharacterEligibleForJieqingSeizeFortune(worth, consummateLevel);
						if (!flag3)
						{
							int levelIndex = CommonUtils.GetJieqingSignLevelIndex(worth);
							bool flag4 = levelIndex < 0;
							if (flag4)
							{
								levelIndex = 0;
							}
							this.jieqingSign.gameObject.SetActive(true);
							this.jieqingSign.SetSprite(string.Format("ui9_icon_jieqingmark_{0}_30x30", levelIndex), false, null);
							bool flag5 = this.jieqingSignTips;
							if (flag5)
							{
								this.jieqingSignTips.Type = TipType.JieqingInteractCharTips;
								TooltipInvoker tooltipInvoker = this.jieqingSignTips;
								if (tooltipInvoker.RuntimeParam == null)
								{
									tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
								}
								this.jieqingSignTips.RuntimeParam.Set("charId", charId);
							}
						}
					}
				});
			}
		}

		// Token: 0x06006E07 RID: 28167 RVA: 0x0032D3E8 File Offset: 0x0032B5E8
		public void OnBeginDrag(PointerEventData eventData)
		{
			Transform parent = base.transform.parent;
			ExecuteEvents.ExecuteHierarchy<IBeginDragHandler>((parent != null) ? parent.gameObject : null, eventData, ExecuteEvents.beginDragHandler);
		}

		// Token: 0x06006E08 RID: 28168 RVA: 0x0032D40D File Offset: 0x0032B60D
		public void OnDrag(PointerEventData eventData)
		{
			Transform parent = base.transform.parent;
			ExecuteEvents.ExecuteHierarchy<IDragHandler>((parent != null) ? parent.gameObject : null, eventData, ExecuteEvents.dragHandler);
		}

		// Token: 0x06006E09 RID: 28169 RVA: 0x0032D432 File Offset: 0x0032B632
		public void OnEndDrag(PointerEventData eventData)
		{
			Transform parent = base.transform.parent;
			ExecuteEvents.ExecuteHierarchy<IEndDragHandler>((parent != null) ? parent.gameObject : null, eventData, ExecuteEvents.endDragHandler);
		}

		// Token: 0x04005183 RID: 20867
		[SerializeField]
		private TooltipInvoker aiActionTooltip;

		// Token: 0x04005184 RID: 20868
		[NonSerialized]
		protected IMapBlockCharHolder _parent;

		// Token: 0x04005185 RID: 20869
		[SerializeField]
		internal RectTransform rectTransform;

		// Token: 0x04005186 RID: 20870
		[SerializeField]
		private MapBlockCharStat stat;

		// Token: 0x04005187 RID: 20871
		[SerializeField]
		private Game.Components.Avatar.Avatar charAvatar;

		// Token: 0x04005188 RID: 20872
		[SerializeField]
		public int CharId;

		// Token: 0x04005189 RID: 20873
		[SerializeField]
		public int CharStatus;

		// Token: 0x0400518A RID: 20874
		[SerializeField]
		private DisplayType type;

		// Token: 0x0400518B RID: 20875
		[SerializeField]
		private Sprite[] favors;

		// Token: 0x0400518C RID: 20876
		[SerializeField]
		private Sprite favorUnknown;

		// Token: 0x0400518D RID: 20877
		[SerializeField]
		private Sprite normalBg;

		// Token: 0x0400518E RID: 20878
		[SerializeField]
		private Sprite clickBg;

		// Token: 0x0400518F RID: 20879
		[SerializeField]
		private Sprite hoverBg;

		// Token: 0x04005190 RID: 20880
		[SerializeField]
		private CImage nameIcon;

		// Token: 0x04005191 RID: 20881
		[SerializeField]
		private CImage baseBg;

		// Token: 0x04005192 RID: 20882
		[SerializeField]
		private GameObject[] nameIcons;

		// Token: 0x04005193 RID: 20883
		[SerializeField]
		private GameObject[] deadBase;

		// Token: 0x04005194 RID: 20884
		[SerializeField]
		internal TMP_Text nameText;

		// Token: 0x04005195 RID: 20885
		[SerializeField]
		private LanguageRuleTips nameTextHelper;

		// Token: 0x04005196 RID: 20886
		[SerializeField]
		private CButton btn;

		// Token: 0x04005197 RID: 20887
		[SerializeField]
		private TooltipInvoker displayer;

		// Token: 0x04005198 RID: 20888
		[SerializeField]
		private Texture2D tomb;

		// Token: 0x04005199 RID: 20889
		[SerializeField]
		private GradeComponent gradeComponent;

		// Token: 0x0400519A RID: 20890
		[SerializeField]
		private CImage guardIcon;

		// Token: 0x0400519B RID: 20891
		[CanBeNull]
		[SerializeField]
		private MapBlockCharBg mapBlockCharBg;

		// Token: 0x0400519C RID: 20892
		[CanBeNull]
		[SerializeField]
		private CImage jieqingSign;

		// Token: 0x0400519D RID: 20893
		[CanBeNull]
		[SerializeField]
		private TooltipInvoker jieqingSignTips;

		// Token: 0x0400519E RID: 20894
		private const string JieqingSignPrefix = "ui9_icon_jieqingmark_{0}_30x30";

		// Token: 0x0400519F RID: 20895
		private int _version;

		// Token: 0x040051A0 RID: 20896
		public LocalStringManager.LanguageType CurLanguageType;

		// Token: 0x040051A1 RID: 20897
		private string _cached;

		// Token: 0x02001DEA RID: 7658
		private class Cmp : IEqualityComparer<CharacterItem>
		{
			// Token: 0x0600EEA9 RID: 61097 RVA: 0x0060D20C File Offset: 0x0060B40C
			public bool Equals(CharacterItem x, CharacterItem y)
			{
				short? num = (x != null) ? new short?(x.RandomEnemyId) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				num = ((y != null) ? new short?(y.RandomEnemyId) : null);
				int? num3 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				return num2.GetValueOrDefault() == num3.GetValueOrDefault() & num2 != null == (num3 != null);
			}

			// Token: 0x0600EEAA RID: 61098 RVA: 0x0060D2B2 File Offset: 0x0060B4B2
			public int GetHashCode(CharacterItem x)
			{
				return x.RandomEnemyId.GetHashCode();
			}
		}
	}
}
