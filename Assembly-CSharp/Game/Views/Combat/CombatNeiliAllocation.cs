using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Coffee.UIExtensions;
using FrameWork;
using Game.Views.Combat.Migrate;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B31 RID: 2865
	public class CombatNeiliAllocation : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F75 RID: 3957
		// (get) Token: 0x06008C43 RID: 35907 RVA: 0x0040CB49 File Offset: 0x0040AD49
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008C44 RID: 35908 RVA: 0x0040CB50 File Offset: 0x0040AD50
		private void OnEnable()
		{
			this._setStatus.Clear();
		}

		// Token: 0x06008C45 RID: 35909 RVA: 0x0040CB60 File Offset: 0x0040AD60
		private void OnDisable()
		{
			foreach (GameObject effect in this._effectToDestroy.Values)
			{
				Object.Destroy(effect);
			}
			this._effectToDestroy.Clear();
		}

		// Token: 0x06008C46 RID: 35910 RVA: 0x0040CBC8 File Offset: 0x0040ADC8
		private void UpdateOriginNeiliAllocation(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				for (byte i = 0; i < 4; i += 1)
				{
					CombatNeiliAllocationPrefab neiliAllocationRefers = base.transform.GetChild((int)i).GetComponent<CombatNeiliAllocationPrefab>();
					neiliAllocationRefers.origValue.text = processor.OriginNeiliAllocation[(int)i].ToString();
					this.UpdateNeiliAllocationStatus(i, processor.NeiliAllocation, processor.OriginNeiliAllocation);
				}
			}
		}

		// Token: 0x06008C47 RID: 35911 RVA: 0x0040CC4C File Offset: 0x0040AE4C
		private unsafe void UpdateCurrentNeiliAllocation(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				for (byte i = 0; i < 4; i += 1)
				{
					short value = *processor.NeiliAllocation[(int)i];
					CombatNeiliAllocationPrefab element = base.transform.GetChild((int)i).GetComponent<CombatNeiliAllocationPrefab>();
					element.currValue.text = value.ToString().SetColor(CombatNeiliAllocation.NeiliAllocationFontColor[(int)i]);
					this.UpdateNeiliAllocationStatus(i, processor.NeiliAllocation, processor.OriginNeiliAllocation);
				}
			}
		}

		// Token: 0x06008C48 RID: 35912 RVA: 0x0040CCE0 File Offset: 0x0040AEE0
		private void UpdateNeiliAllocationCd(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				CountdownData silenceFrameData = processor.NeiliAllocationCd;
				for (byte i = 0; i < 4; i += 1)
				{
					CombatNeiliAllocationPrefab element = base.transform.GetChild((int)i).GetComponent<CombatNeiliAllocationPrefab>();
					element.inCdMark.enabled = silenceFrameData.On;
				}
			}
		}

		// Token: 0x06008C49 RID: 35913 RVA: 0x0040CD50 File Offset: 0x0040AF50
		private unsafe void UpdateNeiliAllocationEffectTips(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				sbyte neiliType = processor.NeiliType;
				NeiliAllocation neiliAllocation = processor.AllocatedNeiliEffects;
				for (byte i = 0; i < 4; i += 1)
				{
					short value = *neiliAllocation[(int)i];
					CombatNeiliAllocationPrefab neiliAllocationRefers = base.transform.GetChild((int)i).GetComponent<CombatNeiliAllocationPrefab>();
					TooltipInvoker component = neiliAllocationRefers.GetComponent<TooltipInvoker>();
					ArgumentBox argumentBox;
					if ((argumentBox = component.RuntimeParam) == null)
					{
						argumentBox = (component.RuntimeParam = new ArgumentBox());
					}
					ArgumentBox param = argumentBox;
					param.Clear();
					param.Set("neiliType", neiliType);
					param.Set("current", *processor.NeiliAllocation[(int)i]);
					param.Set("origin", *processor.OriginNeiliAllocation[(int)i]);
					param.Set("effect", value);
					param.Set("type", i);
					param.Set("isNeiliLack", this.isNeiliLack[(int)i]);
				}
			}
		}

		// Token: 0x06008C4A RID: 35914 RVA: 0x0040CE74 File Offset: 0x0040B074
		private void UpdateNeiliAllocationStatus(byte neiliAllocationType, NeiliAllocation current, NeiliAllocation origin)
		{
			CombatNeiliAllocationPrefab element = base.transform.GetChild((int)neiliAllocationType).GetComponent<CombatNeiliAllocationPrefab>();
			ENeiliAllocationStatusType status = current.GetStatus(origin, neiliAllocationType);
			ENeiliAllocationStatusType setStatus;
			bool flag = this._setStatus.TryGetValue(neiliAllocationType, out setStatus) && setStatus == status;
			if (!flag)
			{
				this._setStatus[neiliAllocationType] = status;
				element.icon.SetSprite(this.ParseNeiliAllocationSpriteName(status, neiliAllocationType), false, null);
				this.InstantiateStatusEffect(status, neiliAllocationType, element.effectHolder);
			}
		}

		// Token: 0x06008C4B RID: 35915 RVA: 0x0040CEF0 File Offset: 0x0040B0F0
		private string ParseNeiliAllocationSpriteName(ENeiliAllocationStatusType status, byte neiliAllocationType)
		{
			if (!true)
			{
			}
			string result;
			if (status != ENeiliAllocationStatusType.Scatter)
			{
				if (status != ENeiliAllocationStatusType.Leak)
				{
					result = this._neiliAllocationSpriteNames[(int)neiliAllocationType];
				}
				else
				{
					result = this._neiliAllocationLeakSpriteNames[(int)neiliAllocationType];
				}
			}
			else
			{
				result = this._neiliAllocationScatterSpriteNames[(int)neiliAllocationType];
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008C4C RID: 35916 RVA: 0x0040CF38 File Offset: 0x0040B138
		private void InstantiateStatusEffect(ENeiliAllocationStatusType status, byte neiliAllocationType, UIParticle elementRoot)
		{
			GameObject effect;
			bool flag = this._effectToDestroy.TryGetValue(neiliAllocationType, out effect);
			if (flag)
			{
				Object.Destroy(effect);
			}
			if (!true)
			{
			}
			GameObject gameObject;
			if (status != ENeiliAllocationStatusType.Full)
			{
				if (status != ENeiliAllocationStatusType.Bulge)
				{
					gameObject = null;
				}
				else
				{
					gameObject = this.effectBulgeList[(int)neiliAllocationType];
				}
			}
			else
			{
				gameObject = this.effectFullList[(int)neiliAllocationType];
			}
			if (!true)
			{
			}
			GameObject newEffectInRefers = gameObject;
			bool flag2 = newEffectInRefers == null;
			if (!flag2)
			{
				GameObject newEffect = Object.Instantiate<GameObject>(newEffectInRefers, elementRoot.transform);
				Vector3 effectScale = elementRoot.transform.localScale;
				newEffect.transform.localScale = new Vector3(1f / effectScale.x, 1f / effectScale.y, 1f / effectScale.z);
				elementRoot.RefreshParticles();
				elementRoot.Play();
				this._effectToDestroy[neiliAllocationType] = newEffect;
			}
		}

		// Token: 0x06008C4D RID: 35917 RVA: 0x0040D018 File Offset: 0x0040B218
		private void OnDataNeiliAllocationCdChanged(bool isAlly)
		{
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateNeiliAllocationCd(charId);
			}
		}

		// Token: 0x06008C4E RID: 35918 RVA: 0x0040D05C File Offset: 0x0040B25C
		private void OnOriginNeiliAllocationChanged(bool isAlly)
		{
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateOriginNeiliAllocation(charId);
			}
		}

		// Token: 0x06008C4F RID: 35919 RVA: 0x0040D0A0 File Offset: 0x0040B2A0
		private void CheckUpdateNeiliAllocationEffectTips(bool isAlly)
		{
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateNeiliAllocationEffectTips(charId);
			}
		}

		// Token: 0x06008C50 RID: 35920 RVA: 0x0040D0E4 File Offset: 0x0040B2E4
		private void OnNeiliAllocationChanged(bool isAlly)
		{
			bool isCombatOver = this.Model.IsCombatOver;
			if (!isCombatOver)
			{
				bool flag = this.ally != isAlly;
				if (!flag)
				{
					int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
					this.UpdateDataMonitor(charId);
					this.UpdateNeiliNeiliLackNotice();
					this.UpdateCurrentNeiliAllocation(charId);
				}
			}
		}

		// Token: 0x06008C51 RID: 35921 RVA: 0x0040D148 File Offset: 0x0040B348
		private void OnChangeChar()
		{
			int currCharId = this.Model.ChangingToCharId;
			bool flag = this.Model.CharIsAlly(currCharId) != this.ally;
			if (!flag)
			{
				bool flag2 = !this.Model.IsCombatOver;
				if (flag2)
				{
					this.UpdateCurrentNeiliAllocation(currCharId);
					this.UpdateNeiliAllocationEffectTips(currCharId);
				}
				this.UpdateOriginNeiliAllocation(currCharId);
				this.UpdateNeiliAllocationCd(currCharId);
				this.UpdateDataMonitor(currCharId);
			}
		}

		// Token: 0x06008C52 RID: 35922 RVA: 0x0040D1BC File Offset: 0x0040B3BC
		private void UpdateDataMonitor(int charId)
		{
			EquipCombatSkillMonitor dataMonitor = this._dataMonitor;
			if (dataMonitor != null)
			{
				dataMonitor.RemoveCurrNeiliListener(new Action(this.UpdateNeiliNeiliLackNotice));
			}
			this._dataMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipCombatSkillMonitor>(charId, false);
			this._dataMonitor.AddCurrNeiliListener(new Action(this.UpdateNeiliNeiliLackNotice));
		}

		// Token: 0x06008C53 RID: 35923 RVA: 0x0040D214 File Offset: 0x0040B414
		private unsafe void UpdateNeiliNeiliLackNotice()
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(this._dataMonitor.CharacterId, out processor);
			if (!flag)
			{
				CountdownData silenceFrameData = processor.NeiliAllocationCd;
				for (byte i = 0; i < 4; i += 1)
				{
					CombatNeiliAllocationPrefab element = base.transform.GetChild((int)i).GetComponent<CombatNeiliAllocationPrefab>();
					int costNeili = CombatHelper.CalcNeiliCostInCombat(*processor.NeiliAllocation[(int)i], DisorderLevelOfQi.GetDisorderLevelOfQi(processor.DisorderOfQi));
					this.isNeiliLack[(int)i] = (this._dataMonitor.CurrNeili < costNeili && *processor.NeiliAllocation[(int)i] < *processor.OriginNeiliAllocation[(int)i]);
					element.inCdMark.enabled = (silenceFrameData.On || this.isNeiliLack[(int)i]);
				}
				this.UpdateNeiliAllocationEffectTips(this._dataMonitor.CharacterId);
			}
		}

		// Token: 0x06008C54 RID: 35924 RVA: 0x0040D308 File Offset: 0x0040B508
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnDataNeiliAllocationCdChanged = (OnDataChangedEvent)Delegate.Combine(model.OnDataNeiliAllocationCdChanged, new OnDataChangedEvent(this.OnDataNeiliAllocationCdChanged));
			CombatModel model2 = this.Model;
			model2.OnOriginNeiliAllocationChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnOriginNeiliAllocationChanged, new OnDataChangedEvent(this.OnOriginNeiliAllocationChanged));
			CombatModel model3 = this.Model;
			model3.OnNeiliTypeChanged = (OnDataChangedEvent)Delegate.Combine(model3.OnNeiliTypeChanged, new OnDataChangedEvent(this.CheckUpdateNeiliAllocationEffectTips));
			CombatModel model4 = this.Model;
			model4.OnAllocatedNeiliEffectsChanged = (OnDataChangedEvent)Delegate.Combine(model4.OnAllocatedNeiliEffectsChanged, new OnDataChangedEvent(this.CheckUpdateNeiliAllocationEffectTips));
			CombatModel model5 = this.Model;
			model5.OnNeiliAllocationChanged = (OnDataChangedEvent)Delegate.Combine(model5.OnNeiliAllocationChanged, new OnDataChangedEvent(this.OnNeiliAllocationChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008C55 RID: 35925 RVA: 0x0040D3F4 File Offset: 0x0040B5F4
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnDataNeiliAllocationCdChanged = (OnDataChangedEvent)Delegate.Remove(model.OnDataNeiliAllocationCdChanged, new OnDataChangedEvent(this.OnDataNeiliAllocationCdChanged));
			CombatModel model2 = this.Model;
			model2.OnOriginNeiliAllocationChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnOriginNeiliAllocationChanged, new OnDataChangedEvent(this.OnOriginNeiliAllocationChanged));
			CombatModel model3 = this.Model;
			model3.OnNeiliTypeChanged = (OnDataChangedEvent)Delegate.Remove(model3.OnNeiliTypeChanged, new OnDataChangedEvent(this.CheckUpdateNeiliAllocationEffectTips));
			CombatModel model4 = this.Model;
			model4.OnAllocatedNeiliEffectsChanged = (OnDataChangedEvent)Delegate.Remove(model4.OnAllocatedNeiliEffectsChanged, new OnDataChangedEvent(this.CheckUpdateNeiliAllocationEffectTips));
			CombatModel model5 = this.Model;
			model5.OnNeiliAllocationChanged = (OnDataChangedEvent)Delegate.Remove(model5.OnNeiliAllocationChanged, new OnDataChangedEvent(this.OnNeiliAllocationChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			EquipCombatSkillMonitor dataMonitor = this._dataMonitor;
			if (dataMonitor != null)
			{
				dataMonitor.RemoveCurrNeiliListener(new Action(this.UpdateNeiliNeiliLackNotice));
			}
		}

		// Token: 0x04006B57 RID: 27479
		public static readonly string[] NeiliAllocationFontColor = new string[]
		{
			"attack",
			"agile",
			"defense",
			"assist"
		};

		// Token: 0x04006B58 RID: 27480
		private readonly string[] _neiliAllocationSpriteNames = new string[]
		{
			"ui9_icon_neili_small_0",
			"ui9_icon_neili_small_1",
			"ui9_icon_neili_small_2",
			"ui9_icon_neili_small_3"
		};

		// Token: 0x04006B59 RID: 27481
		private readonly string[] _neiliAllocationLeakSpriteNames = new string[]
		{
			"ui9_icon_neili_leak_0",
			"ui9_icon_neili_leak_1",
			"ui9_icon_neili_leak_2",
			"ui9_icon_neili_leak_3"
		};

		// Token: 0x04006B5A RID: 27482
		private readonly string[] _neiliAllocationScatterSpriteNames = new string[]
		{
			"ui9_icon_neili_scatter_0",
			"ui9_icon_neili_scatter_1",
			"ui9_icon_neili_scatter_2",
			"ui9_icon_neili_scatter_3"
		};

		// Token: 0x04006B5B RID: 27483
		private readonly string[] _neiliAllocationFullEffects = new string[]
		{
			"EffectFull0",
			"EffectFull1",
			"EffectFull2",
			"EffectFull3"
		};

		// Token: 0x04006B5C RID: 27484
		private readonly string[] _neiliAllocationBulgeEffects = new string[]
		{
			"EffectBulge0",
			"EffectBulge1",
			"EffectBulge2",
			"EffectBulge3"
		};

		// Token: 0x04006B5D RID: 27485
		[Tooltip("是否为己方的ui组件")]
		public bool ally;

		// Token: 0x04006B5E RID: 27486
		[SerializeField]
		private List<GameObject> effectFullList;

		// Token: 0x04006B5F RID: 27487
		[SerializeField]
		private List<GameObject> effectBulgeList;

		// Token: 0x04006B60 RID: 27488
		private readonly Dictionary<byte, ENeiliAllocationStatusType> _setStatus = new Dictionary<byte, ENeiliAllocationStatusType>();

		// Token: 0x04006B61 RID: 27489
		private readonly Dictionary<byte, GameObject> _effectToDestroy = new Dictionary<byte, GameObject>();

		// Token: 0x04006B62 RID: 27490
		private EquipCombatSkillMonitor _dataMonitor;

		// Token: 0x04006B63 RID: 27491
		private readonly bool[] isNeiliLack = new bool[4];
	}
}
