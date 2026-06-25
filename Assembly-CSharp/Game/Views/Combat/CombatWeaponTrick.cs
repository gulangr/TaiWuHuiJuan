using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Combat.Migrate;
using Game.Views.MouseTips;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AEC RID: 2796
	public class CombatWeaponTrick : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F2E RID: 3886
		// (get) Token: 0x06008961 RID: 35169 RVA: 0x003F8EE5 File Offset: 0x003F70E5
		private Vector2[] TrickAnchoredPos
		{
			get
			{
				return this.ally ? this._selfTrickAnchoredPos : this._enemyTrickAnchoredPos;
			}
		}

		// Token: 0x17000F2F RID: 3887
		// (get) Token: 0x06008962 RID: 35170 RVA: 0x003F8EFD File Offset: 0x003F70FD
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008963 RID: 35171 RVA: 0x003F8F04 File Offset: 0x003F7104
		private void Awake()
		{
			this._lastTrickIndex = -1;
			bool flag = this.currTrickHolder.childCount > 0;
			if (flag)
			{
				RectTransform currTrickTransform = this.currTrickHolder.GetChild(0).GetComponent<RectTransform>();
				RectTransform trickHolder = this.weaponTrickHolder;
				int trickIndex = currTrickTransform.GetComponent<Refers>().UserInt;
				currTrickTransform.SetParent(trickHolder, true);
				currTrickTransform.anchoredPosition = this.TrickAnchoredPos[trickIndex];
				currTrickTransform.localScale = Vector3.one;
				trickHolder.localRotation = Quaternion.identity;
				this.OnTrickHolderRotate();
			}
			bool flag2 = this.ally;
			if (flag2)
			{
				PointerTrigger changeTrickPointerTrigger = this.changeTrick.GetComponent<PointerTrigger>();
				GameObject changeTrickHighlight = changeTrickPointerTrigger.transform.GetChild(1).gameObject;
				changeTrickPointerTrigger.EnterEvent.AddListener(delegate()
				{
					bool interactable = changeTrickPointerTrigger.GetComponent<CButton>().interactable;
					if (interactable)
					{
						changeTrickHighlight.SetActive(true);
					}
				});
				changeTrickPointerTrigger.ExitEvent.AddListener(delegate()
				{
					changeTrickHighlight.SetActive(false);
				});
			}
		}

		// Token: 0x06008964 RID: 35172 RVA: 0x003F9010 File Offset: 0x003F7210
		private void OnEnable()
		{
			this.changeTrickDecoration.gameObject.SetActive(false);
		}

		// Token: 0x06008965 RID: 35173 RVA: 0x003F9028 File Offset: 0x003F7228
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnWeaponTricksChanged = (OnDataChangedEvent)Delegate.Combine(model.OnWeaponTricksChanged, new OnDataChangedEvent(this.RefreshTrickPanel));
			CombatModel model2 = this.Model;
			model2.OnWeaponTrickIndexChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnWeaponTrickIndexChanged, new OnDataChangedEvent(this.SetCurrTrickIndex));
			CombatModel model3 = this.Model;
			model3.OnChangeTrickCountChanged = (OnDataChangedEvent)Delegate.Combine(model3.OnChangeTrickCountChanged, new OnDataChangedEvent(this.OnChangeTrickCountChanged));
			CombatModel model4 = this.Model;
			model4.OnSkillEffectCollectionChanged = (OnDataChangedEvent)Delegate.Combine(model4.OnSkillEffectCollectionChanged, new OnDataChangedEvent(this.OnSkillEffectCollectionChanged));
			CombatModel model5 = this.Model;
			model5.OnChangeTrickProgressChanged = (OnDataChangedEvent)Delegate.Combine(model5.OnChangeTrickProgressChanged, new OnDataChangedEvent(this.OnChangeTrickProgressChanged));
			CombatModel model6 = this.Model;
			model6.OnCanChangeTrickChanged = (OnDataChangedEvent)Delegate.Combine(model6.OnCanChangeTrickChanged, new OnDataChangedEvent(this.OnCanChangeTrickChanged));
			CombatModel model7 = this.Model;
			model7.OnExecutingTeammateCommandChanged = (OnCharacterDataChangedEvent<sbyte>)Delegate.Combine(model7.OnExecutingTeammateCommandChanged, new OnCharacterDataChangedEvent<sbyte>(this.OnExecutingTeammateCommandChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008966 RID: 35174 RVA: 0x003F9160 File Offset: 0x003F7360
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnWeaponTricksChanged = (OnDataChangedEvent)Delegate.Remove(model.OnWeaponTricksChanged, new OnDataChangedEvent(this.RefreshTrickPanel));
			CombatModel model2 = this.Model;
			model2.OnWeaponTrickIndexChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnWeaponTrickIndexChanged, new OnDataChangedEvent(this.SetCurrTrickIndex));
			CombatModel model3 = this.Model;
			model3.OnChangeTrickCountChanged = (OnDataChangedEvent)Delegate.Remove(model3.OnChangeTrickCountChanged, new OnDataChangedEvent(this.OnChangeTrickCountChanged));
			CombatModel model4 = this.Model;
			model4.OnSkillEffectCollectionChanged = (OnDataChangedEvent)Delegate.Remove(model4.OnSkillEffectCollectionChanged, new OnDataChangedEvent(this.OnSkillEffectCollectionChanged));
			CombatModel model5 = this.Model;
			model5.OnChangeTrickProgressChanged = (OnDataChangedEvent)Delegate.Remove(model5.OnChangeTrickProgressChanged, new OnDataChangedEvent(this.OnChangeTrickProgressChanged));
			CombatModel model6 = this.Model;
			model6.OnCanChangeTrickChanged = (OnDataChangedEvent)Delegate.Remove(model6.OnCanChangeTrickChanged, new OnDataChangedEvent(this.OnCanChangeTrickChanged));
			CombatModel model7 = this.Model;
			model7.OnExecutingTeammateCommandChanged = (OnCharacterDataChangedEvent<sbyte>)Delegate.Remove(model7.OnExecutingTeammateCommandChanged, new OnCharacterDataChangedEvent<sbyte>(this.OnExecutingTeammateCommandChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008967 RID: 35175 RVA: 0x003F9298 File Offset: 0x003F7498
		private void OnChangeChar()
		{
			int currCharId = this.Model.ChangingToCharId;
			bool flag = this.ally != this.Model.CharIsAlly(currCharId);
			if (!flag)
			{
				this.RefreshTrickPanelByCharacter(currCharId);
				this.SetCurrTrickIndexByCharacter(currCharId);
				this.UpdateChangeTrickCountByCharacter(currCharId);
				this.OnSkillEffectCollectionChangedByCharacter(currCharId);
				this.UpdateChangeTrickProgressBarByCharacter(currCharId);
				this.UpdateChangeTrickImageByCharacter(currCharId);
			}
		}

		// Token: 0x06008968 RID: 35176 RVA: 0x003F9300 File Offset: 0x003F7500
		public static void UpdateTrickIcon(CombatTrickPrefab trickRefers, sbyte trickType, bool hideCurrTrickMark = true, bool isBigIcon = false, bool isAvoidTrick = false, MouseTipTrickType.AvoidTrickTipHitOddsInfo? avoidTrickTipHitOddsInfo = null)
		{
			TrickTypeItem trickConfig = Config.TrickType.Instance[trickType];
			CImage mainImage = trickRefers.mainImage;
			mainImage.SetSprite(isAvoidTrick ? (isBigIcon ? trickConfig.AvoidBigBackIcon : trickConfig.AvoidBackIcon) : (isBigIcon ? trickConfig.BigBackIcon : trickConfig.BackIcon), false, null);
			trickRefers.trickName.text = trickConfig.ChineseName.SetColor(trickConfig.FontColor);
			if (hideCurrTrickMark)
			{
				trickRefers.currTrickMark.gameObject.SetActive(false);
			}
			bool flag = trickRefers.costPreview != null;
			if (flag)
			{
				trickRefers.costPreview.SetActive(false);
			}
			TooltipInvoker mouseTip = trickRefers.GetComponent<TooltipInvoker>();
			mouseTip.RuntimeParam = null;
			bool flag2 = GameData.Domains.Combat.TrickType.NoBodyDamageTrickType.IndexOf(trickType) == -1;
			if (flag2)
			{
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.Set("TrickType", trickType);
				argumentBox.Set("IsAvoidTrick", isAvoidTrick);
				if (isAvoidTrick)
				{
					argumentBox.SetObject("AvoidTrickTipHitOddsInfo", avoidTrickTipHitOddsInfo.Value);
				}
				mouseTip.RuntimeParam = argumentBox;
			}
		}

		// Token: 0x06008969 RID: 35177 RVA: 0x003F941A File Offset: 0x003F761A
		public RectTransform GetCurrTrickHolder()
		{
			return this.currTrickHolder;
		}

		// Token: 0x0600896A RID: 35178 RVA: 0x003F9422 File Offset: 0x003F7622
		public RectTransform GetChangeTrick()
		{
			return this.changeTrick.GetComponent<RectTransform>();
		}

		// Token: 0x0600896B RID: 35179 RVA: 0x003F9430 File Offset: 0x003F7630
		private void RefreshTrickPanel(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.RefreshTrickPanelByCharacter(charId);
			}
		}

		// Token: 0x0600896C RID: 35180 RVA: 0x003F9474 File Offset: 0x003F7674
		private void RefreshTrickPanelByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				sbyte[] tricks = processor.WeaponTricks;
				for (int i = 0; i < this.weaponTrickHolder.childCount; i++)
				{
					CombatTrickPrefab trickRefers = this.weaponTrickHolder.GetChild(i).GetComponent<CombatTrickPrefab>();
					CombatWeaponTrick.UpdateTrickIcon(trickRefers, tricks[trickRefers.UserInt], true, false, false, null);
				}
				bool flag2 = this.currTrickHolder.childCount > 0;
				if (flag2)
				{
					CombatTrickPrefab trickRefers2 = this.currTrickHolder.GetChild(0).GetComponent<CombatTrickPrefab>();
					CombatWeaponTrick.UpdateTrickIcon(trickRefers2, tricks[trickRefers2.UserInt], false, false, false, null);
				}
			}
		}

		// Token: 0x0600896D RID: 35181 RVA: 0x003F9540 File Offset: 0x003F7740
		private void SetCurrTrickIndex(bool isAlly)
		{
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.SetCurrTrickIndexByCharacter(charId);
			}
		}

		// Token: 0x0600896E RID: 35182 RVA: 0x003F9584 File Offset: 0x003F7784
		private void SetCurrTrickIndexByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				byte index = processor.WeaponTrickIndex;
				int lastTrickIndex = this._lastTrickIndex;
				bool flag2 = lastTrickIndex == (int)index;
				if (!flag2)
				{
					this._lastTrickIndex = (int)index;
					this.weaponTrickHolder.DOKill(false);
					this.weaponTrickHolder.DOLocalRotate(Vector3.zero.SetZ((float)((this.ally ? -60 : 60) * (int)index)), 0.3f, RotateMode.Fast).OnUpdate(new TweenCallback(this.OnTrickHolderRotate)).SetUpdate(true);
					bool flag3 = lastTrickIndex >= 0;
					if (flag3)
					{
						RectTransform lastTrickTransform = this.currTrickHolder.GetChild(0).GetComponent<RectTransform>();
						CombatTrickPrefab lastTrickRefers = lastTrickTransform.GetComponent<CombatTrickPrefab>();
						CImage lastCurrTrickMark = lastTrickRefers.currTrickMark;
						lastTrickTransform.DOKill(false);
						lastTrickTransform.SetParent(this.weaponTrickHolder, true);
						lastTrickTransform.DOAnchorPos(this.TrickAnchoredPos[lastTrickIndex], 0.3f, false).SetUpdate(true);
						CombatWeaponTrick.UpdateTrickIcon(lastTrickRefers, processor.WeaponTricks[lastTrickIndex], true, false, false, null);
						lastCurrTrickMark.DOKill(false);
						lastCurrTrickMark.DOFade(0f, 0.3f).OnComplete(delegate
						{
							lastCurrTrickMark.gameObject.SetActive(false);
						}).SetUpdate(true);
					}
					RectTransform currTrickTransform = null;
					for (int i = 0; i < this.weaponTrickHolder.childCount; i++)
					{
						CombatTrickPrefab trickRefers = this.weaponTrickHolder.GetChild(i).GetComponent<CombatTrickPrefab>();
						bool flag4 = trickRefers.UserInt == (int)index;
						if (flag4)
						{
							currTrickTransform = trickRefers.GetComponent<RectTransform>();
							break;
						}
					}
					bool flag5 = currTrickTransform == null;
					if (flag5)
					{
						currTrickTransform = this.currTrickHolder.GetChild(0).GetComponent<RectTransform>();
					}
					CImage currTrickMark = currTrickTransform.GetComponent<CombatTrickPrefab>().currTrickMark;
					currTrickTransform.DOKill(false);
					currTrickTransform.SetParent(this.currTrickHolder, true);
					currTrickTransform.localScale = Vector3.one;
					currTrickTransform.DOAnchorPos(Vector2.zero, 0.3f, false).SetUpdate(true);
					currTrickMark.DOKill(false);
					currTrickMark.SetAlpha(0f);
					currTrickMark.gameObject.SetActive(true);
					currTrickMark.DOFade(1f, 0.3f).SetUpdate(true);
				}
			}
		}

		// Token: 0x0600896F RID: 35183 RVA: 0x003F97F4 File Offset: 0x003F79F4
		private void OnTrickHolderRotate()
		{
			for (int i = 0; i < this.weaponTrickHolder.childCount; i++)
			{
				this.weaponTrickHolder.GetChild(i).rotation = Quaternion.identity;
			}
		}

		// Token: 0x06008970 RID: 35184 RVA: 0x003F9834 File Offset: 0x003F7A34
		private void OnChangeTrickCountChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateChangeTrickCountByCharacter(charId);
			}
		}

		// Token: 0x06008971 RID: 35185 RVA: 0x003F9878 File Offset: 0x003F7A78
		private void UpdateChangeTrickCountByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				this.changeTrickCount.text = processor.ChangeTrickCount.ToString();
			}
		}

		// Token: 0x06008972 RID: 35186 RVA: 0x003F98BC File Offset: 0x003F7ABC
		private void OnSkillEffectCollectionChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.OnSkillEffectCollectionChangedByCharacter(charId);
			}
		}

		// Token: 0x06008973 RID: 35187 RVA: 0x003F9900 File Offset: 0x003F7B00
		private void OnSkillEffectCollectionChangedByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				SkillEffectCollection effectCollection = processor.SkillEffectCollection;
				int num;
				CombatSkillItem combatSkillItem;
				SpecialEffectItem specialEffectItem;
				bool exist = CombatUtils.FindTransferProportionEffectFromCollection(effectCollection, out num, out combatSkillItem, out specialEffectItem);
				this.changeTrickHighLight.SetActive(processor.CanChangeTrick || exist);
				this.changeTrickDecoration.gameObject.SetActive(exist);
			}
		}

		// Token: 0x06008974 RID: 35188 RVA: 0x003F9964 File Offset: 0x003F7B64
		private void OnChangeTrickProgressChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateChangeTrickProgressBarByCharacter(charId);
			}
		}

		// Token: 0x06008975 RID: 35189 RVA: 0x003F99A8 File Offset: 0x003F7BA8
		private void UpdateChangeTrickProgressBarByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				this.changeTrickProgressBar.fillAmount = (float)processor.ChangeTrickProgress / (float)GlobalConfig.Instance.MaxChangeTrickProgress;
			}
		}

		// Token: 0x06008976 RID: 35190 RVA: 0x003F99F4 File Offset: 0x003F7BF4
		private void OnCanChangeTrickChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateChangeTrickImageByCharacter(charId);
			}
		}

		// Token: 0x06008977 RID: 35191 RVA: 0x003F9A38 File Offset: 0x003F7C38
		private void OnExecutingTeammateCommandChanged(int charId, sbyte oldValue)
		{
			int currCharId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
			bool flag = charId != currCharId;
			if (!flag)
			{
				this.UpdateChangeTrickImageByCharacter(charId);
			}
		}

		// Token: 0x06008978 RID: 35192 RVA: 0x003F9A7C File Offset: 0x003F7C7C
		private void UpdateChangeTrickImageByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				bool canChangeTrick = processor.CanChangeTrick;
				canChangeTrick = (canChangeTrick && this.Model.CanOperateCharacter(charId));
				this.changeTrickHighLight.SetActive(processor.CanChangeTrick);
				bool flag2 = this.ally;
				if (flag2)
				{
					this.changeTrick.GetComponent<CButton>().interactable = canChangeTrick;
				}
			}
		}

		// Token: 0x04006951 RID: 26961
		[SerializeField]
		private bool ally;

		// Token: 0x04006952 RID: 26962
		[SerializeField]
		private RectTransform weaponTrickHolder;

		// Token: 0x04006953 RID: 26963
		[SerializeField]
		private TextMeshProUGUI changeTrickCount;

		// Token: 0x04006954 RID: 26964
		[SerializeField]
		private RectTransform currTrickHolder;

		// Token: 0x04006955 RID: 26965
		[SerializeField]
		private CImage changeTrickDecoration;

		// Token: 0x04006956 RID: 26966
		[SerializeField]
		private CImage changeTrickProgressBar;

		// Token: 0x04006957 RID: 26967
		[SerializeField]
		private CImage changeTrick;

		// Token: 0x04006958 RID: 26968
		[SerializeField]
		private GameObject changeTrickHighLight;

		// Token: 0x04006959 RID: 26969
		[SerializeField]
		private ParticleSystem effTrickHolderHint;

		// Token: 0x0400695A RID: 26970
		private readonly Vector2[] _selfTrickAnchoredPos = new Vector2[]
		{
			new Vector2(82f, 0f),
			new Vector2(41f, 71f),
			new Vector2(-41f, 71f),
			new Vector2(-82f, 0f),
			new Vector2(-41f, -71f),
			new Vector2(41f, -71f)
		};

		// Token: 0x0400695B RID: 26971
		private readonly Vector2[] _enemyTrickAnchoredPos = new Vector2[]
		{
			new Vector2(-80f, 0f),
			new Vector2(-41f, 71f),
			new Vector2(41f, 71f),
			new Vector2(82f, 0f),
			new Vector2(41f, -71f),
			new Vector2(-41f, -71f)
		};

		// Token: 0x0400695C RID: 26972
		private int _lastTrickIndex;
	}
}
