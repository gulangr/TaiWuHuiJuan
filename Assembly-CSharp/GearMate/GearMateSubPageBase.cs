using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using UnityEngine;

namespace GearMate
{
	// Token: 0x0200061A RID: 1562
	public class GearMateSubPageBase : Refers
	{
		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x0600497B RID: 18811 RVA: 0x00226035 File Offset: 0x00224235
		public virtual bool IsLeaf
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x0600497C RID: 18812 RVA: 0x00226038 File Offset: 0x00224238
		// (set) Token: 0x0600497D RID: 18813 RVA: 0x00226040 File Offset: 0x00224240
		private protected UI_GearMate Parent { protected get; private set; }

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x0600497E RID: 18814 RVA: 0x00226049 File Offset: 0x00224249
		protected CharacterDisplayData GearMateDisplayData
		{
			get
			{
				return this.Parent.GearMateDisplayData;
			}
		}

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x0600497F RID: 18815 RVA: 0x00226056 File Offset: 0x00224256
		protected GearMate GearMate
		{
			get
			{
				return this.Parent.GearMate;
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06004980 RID: 18816 RVA: 0x00226063 File Offset: 0x00224263
		protected int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06004981 RID: 18817 RVA: 0x0022606F File Offset: 0x0022426F
		protected int TotalDropItemCount
		{
			get
			{
				return this._droppedCount + this._dropItems.Count;
			}
		}

		// Token: 0x06004982 RID: 18818 RVA: 0x00226084 File Offset: 0x00224284
		public void Init(UI_GearMate parent)
		{
			this.Parent = parent;
			bool flag = !this._initialized;
			if (flag)
			{
				this.InitInternal();
				bool isLeaf = this.IsLeaf;
				if (isLeaf)
				{
					this.GetListenerId();
					parent.RegisterLeafSubPage(this);
				}
			}
			this._initialized = true;
		}

		// Token: 0x06004983 RID: 18819 RVA: 0x002260D2 File Offset: 0x002242D2
		private void GetListenerId()
		{
			this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		}

		// Token: 0x06004984 RID: 18820 RVA: 0x002260EC File Offset: 0x002242EC
		public void UnRegisterListener()
		{
			bool flag = this._listenerId != -1;
			if (flag)
			{
				GameDataBridge.UnregisterListener(this._listenerId);
				this._listenerId = -1;
			}
		}

		// Token: 0x06004985 RID: 18821 RVA: 0x0022611F File Offset: 0x0022431F
		protected virtual void InitInternal()
		{
		}

		// Token: 0x06004986 RID: 18822 RVA: 0x00226122 File Offset: 0x00224322
		public virtual void OnEnableBySwitchPage(int pageIndex)
		{
		}

		// Token: 0x06004987 RID: 18823 RVA: 0x00226128 File Offset: 0x00224328
		private void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						this.HandleMethodReturn(notification, wrapper);
					}
				}
				else
				{
					this.HandleDataModification(notification, wrapper);
				}
			}
		}

		// Token: 0x06004988 RID: 18824 RVA: 0x002261AC File Offset: 0x002243AC
		protected virtual void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
		}

		// Token: 0x06004989 RID: 18825 RVA: 0x002261AF File Offset: 0x002243AF
		public virtual void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
		}

		// Token: 0x0600498A RID: 18826 RVA: 0x002261B2 File Offset: 0x002243B2
		public virtual void OnListenerIdReady()
		{
		}

		// Token: 0x0600498B RID: 18827 RVA: 0x002261B5 File Offset: 0x002243B5
		public virtual void OnGearMateCharacterIdChanged(int lastId)
		{
		}

		// Token: 0x0600498C RID: 18828 RVA: 0x002261B8 File Offset: 0x002243B8
		public virtual void Confirm(ItemKeyAndCount itemKeyAndCount, ItemSourceType itemSourceType)
		{
		}

		// Token: 0x0600498D RID: 18829 RVA: 0x002261BB File Offset: 0x002243BB
		public virtual void PlayUpgradeAnim(Action action)
		{
		}

		// Token: 0x0600498E RID: 18830 RVA: 0x002261BE File Offset: 0x002243BE
		public virtual void OnGearMateDataChanged()
		{
		}

		// Token: 0x0600498F RID: 18831 RVA: 0x002261C1 File Offset: 0x002243C1
		public virtual void OnItemChanged(ItemKey itemKey, int amount, bool queueAnim = false, bool isAllSelected = false, bool playItemAnim = true)
		{
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x002261C4 File Offset: 0x002243C4
		protected void ItemDrop(GameObject itemObj)
		{
			this._dropItems.Enqueue(itemObj);
		}

		// Token: 0x06004991 RID: 18833 RVA: 0x002261D4 File Offset: 0x002243D4
		public void PlayDropAnimation()
		{
			bool flag = this._dropCoroutine != null;
			if (!flag)
			{
				this._dropCoroutine = this.StartCoroutine(this.QueueDrop());
			}
		}

		// Token: 0x06004992 RID: 18834 RVA: 0x00226204 File Offset: 0x00224404
		public void StopDropAnimation()
		{
			bool flag = this._dropCoroutine == null;
			if (!flag)
			{
				this.StopCoroutine(this._dropCoroutine);
				this._droppedCount = 0;
				this._dropCoroutine = null;
				foreach (GameObject item in this._dropItems)
				{
					Object.Destroy(item);
				}
				this._dropItems.Clear();
			}
		}

		// Token: 0x06004993 RID: 18835 RVA: 0x00226290 File Offset: 0x00224490
		private IEnumerator QueueDrop()
		{
			bool flag = this._dropItems.Count == 0;
			if (flag)
			{
				yield return null;
			}
			this._droppedCount = 0;
			float averageInterval = 1f / (float)this._dropItems.Count;
			bool flag2 = averageInterval > 0.3f;
			if (flag2)
			{
				averageInterval = 0.3f;
			}
			while (this._dropItems.Count > 0)
			{
				int count = 1;
				bool flag3 = Time.deltaTime > averageInterval;
				if (flag3)
				{
					count = Mathf.RoundToInt(Time.deltaTime / averageInterval);
				}
				int i = 0;
				for (;;)
				{
					int num = i;
					i = num + 1;
					GameObject item;
					bool flag4 = num < count && this._dropItems.TryDequeue(out item);
					if (!flag4)
					{
						break;
					}
					bool flag5 = i >= 2;
					if (flag5)
					{
						Object.Destroy(item.gameObject);
					}
					else
					{
						item.SetActive(true);
					}
					bool flag6 = this._droppedCount == 0;
					if (flag6)
					{
						item.GetComponent<ItemDrop>().OnTrigger += this.SetMachineWaterHeight;
					}
					this._droppedCount++;
				}
				yield return new WaitForSeconds(averageInterval);
			}
			yield break;
		}

		// Token: 0x06004994 RID: 18836 RVA: 0x0022629F File Offset: 0x0022449F
		protected virtual void SetMachineWaterHeight()
		{
		}

		// Token: 0x06004995 RID: 18837 RVA: 0x002262A2 File Offset: 0x002244A2
		public virtual void ResetProcessValue(bool isChangeSource = false)
		{
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06004996 RID: 18838 RVA: 0x002262A5 File Offset: 0x002244A5
		protected int ListenerId
		{
			get
			{
				return this._listenerId;
			}
		}

		// Token: 0x06004997 RID: 18839 RVA: 0x002262B0 File Offset: 0x002244B0
		protected virtual IList<UIBase.MonitorDataField> GetMonitorFields()
		{
			return null;
		}

		// Token: 0x06004998 RID: 18840 RVA: 0x002262C4 File Offset: 0x002244C4
		public void InitMonitorFields()
		{
			this._listeningField.Clear();
			IList<UIBase.MonitorDataField> fields = this.GetMonitorFields();
			bool flag = fields == null;
			if (!flag)
			{
				foreach (UIBase.MonitorDataField dataField in fields)
				{
					this.AddDataMonitor(dataField);
				}
			}
		}

		// Token: 0x06004999 RID: 18841 RVA: 0x00226330 File Offset: 0x00224530
		private void AddDataMonitor(UIBase.MonitorDataField dataField)
		{
			bool flag = dataField.SubId1List != null;
			if (flag)
			{
				GameDataBridge.AddDataMonitor(this.ListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataMonitor(this.ListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
			this._listeningField.Add(dataField);
		}

		// Token: 0x0600499A RID: 18842 RVA: 0x002263A0 File Offset: 0x002245A0
		public virtual void UnMonitorFields()
		{
			foreach (UIBase.MonitorDataField field in this._listeningField)
			{
				this.AddDataUnMonitor(field, false);
			}
			this._listeningField.Clear();
		}

		// Token: 0x0600499B RID: 18843 RVA: 0x00226408 File Offset: 0x00224608
		protected void RemoveMonitorFieldId(ushort domainId, ushort dataId, ulong subId0)
		{
			this.AddDataUnMonitor(new UIBase.MonitorDataField(domainId, dataId, subId0, null), true);
		}

		// Token: 0x0600499C RID: 18844 RVA: 0x0022641C File Offset: 0x0022461C
		protected void AppendMonitorFieldId(UIBase.MonitorDataField dataField)
		{
			this.AddDataMonitor(dataField);
		}

		// Token: 0x0600499D RID: 18845 RVA: 0x00226428 File Offset: 0x00224628
		private void AddDataUnMonitor(UIBase.MonitorDataField dataField, bool removeFromSet = true)
		{
			bool flag = dataField.SubId1List != null;
			if (flag)
			{
				GameDataBridge.AddDataUnMonitor(this.ListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, dataField.SubId1List);
			}
			else
			{
				GameDataBridge.AddDataUnMonitor(this.ListenerId, dataField.DomainId, dataField.DataId, dataField.SubId0, uint.MaxValue);
			}
			if (removeFromSet)
			{
				this._listeningField.Remove(dataField);
			}
		}

		// Token: 0x0600499E RID: 18846 RVA: 0x0022649A File Offset: 0x0022469A
		public virtual void SetButtonState(bool state)
		{
		}

		// Token: 0x0600499F RID: 18847 RVA: 0x002264A0 File Offset: 0x002246A0
		public virtual bool ConfirmButtonInteractable()
		{
			CButtonObsolete button;
			bool flag = this.CTryGet<CButtonObsolete>("ButtonConfirm", out button);
			return flag && button.interactable;
		}

		// Token: 0x060049A0 RID: 18848 RVA: 0x002264CE File Offset: 0x002246CE
		public virtual void ConfirmClick()
		{
		}

		// Token: 0x060049A1 RID: 18849 RVA: 0x002264D1 File Offset: 0x002246D1
		public virtual void PointEnterConfirmButton()
		{
			AudioManager.Instance.PlaySound("ui_default_hover", false, false);
		}

		// Token: 0x060049A2 RID: 18850 RVA: 0x002264E8 File Offset: 0x002246E8
		public virtual bool IsMaxLevel()
		{
			return false;
		}

		// Token: 0x060049A3 RID: 18851 RVA: 0x002264FC File Offset: 0x002246FC
		protected string GetGearMateName()
		{
			return NameCenter.GetMonasticTitleOrDisplayName(this.Parent.GearMateDisplayData, false);
		}

		// Token: 0x060049A4 RID: 18852 RVA: 0x00226520 File Offset: 0x00224720
		public virtual bool CheckItemInteractable(ItemDisplayData itemDisplayData, out int canSelectCount)
		{
			canSelectCount = itemDisplayData.Amount;
			return true;
		}

		// Token: 0x060049A5 RID: 18853 RVA: 0x0022653B File Offset: 0x0022473B
		public virtual void RefreshItemTipNotInteractable(ItemView itemView)
		{
		}

		// Token: 0x060049A6 RID: 18854 RVA: 0x0022653E File Offset: 0x0022473E
		protected IEnumerator ScaleCoroutine(Transform transform1, float time, Vector3 targetScale, Action action = null)
		{
			Vector3 startScale = transform1.localScale;
			float elapsed = 0f;
			bool flag = time >= Time.deltaTime;
			if (flag)
			{
				while (elapsed < time)
				{
					transform1.localScale = Vector3.Lerp(startScale, targetScale, elapsed / time);
					elapsed += Time.deltaTime;
					yield return null;
				}
			}
			transform1.localScale = targetScale;
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x060049A7 RID: 18855 RVA: 0x0022656C File Offset: 0x0022476C
		protected IEnumerator ContinuousScaleCoroutine(Transform transform1, int count, float time, Vector3 targetScale, int index = 0, Action<int, int> middleAction = null, Action lastAction = null)
		{
			Vector3 startScale = transform1.localScale;
			float elapsed = 0f;
			int curCount = 0;
			while (elapsed < time * (float)(count + 1))
			{
				elapsed += Time.deltaTime;
				int tempCount = (int)(elapsed / time);
				transform1.localScale = Vector3.Lerp((tempCount == 0) ? startScale : new Vector3(0.001f, 1f, 1f), (count - tempCount > 0) ? new Vector3(1f, 1f, 1f) : targetScale, elapsed % time / time);
				bool flag = tempCount - curCount >= 1 && tempCount <= count;
				if (flag)
				{
					curCount = tempCount;
					bool flag2 = count > 0;
					if (flag2)
					{
						if (middleAction != null)
						{
							middleAction(index, Math.Max(count - curCount, 1));
						}
					}
				}
				yield return null;
			}
			transform1.localScale = targetScale;
			if (lastAction != null)
			{
				lastAction();
			}
			yield break;
		}

		// Token: 0x060049A8 RID: 18856 RVA: 0x002265BB File Offset: 0x002247BB
		protected IEnumerator SetProcessYellowTransparent(GameObject processYellow, Action action = null, float time = 0.5f)
		{
			float curTime = 0f;
			List<Gradient> colors = new List<Gradient>();
			int num;
			for (int i = 0; i < processYellow.transform.childCount; i = num + 1)
			{
				ParticleSystem.ColorOverLifetimeModule col = processYellow.transform.GetChild(i).GetComponent<ParticleSystem>().colorOverLifetime;
				colors.Add(col.color.gradient);
				col = default(ParticleSystem.ColorOverLifetimeModule);
				num = i;
			}
			while (curTime < time)
			{
				for (int j = 0; j < processYellow.transform.childCount; j = num + 1)
				{
					ParticleSystem.MainModule particleMain = processYellow.transform.GetChild(j).GetComponent<ParticleSystem>().main;
					Color color = particleMain.startColor.color;
					float alphaValue = color.a * (1f - curTime / time);
					ParticleSystem.ColorOverLifetimeModule col2 = processYellow.transform.GetChild(j).GetComponent<ParticleSystem>().colorOverLifetime;
					Gradient grad = new Gradient();
					grad.SetKeys(col2.color.gradient.colorKeys, new GradientAlphaKey[]
					{
						new GradientAlphaKey(alphaValue, 0f),
						new GradientAlphaKey(alphaValue, 1f)
					});
					col2.color = grad;
					particleMain = default(ParticleSystem.MainModule);
					color = default(Color);
					col2 = default(ParticleSystem.ColorOverLifetimeModule);
					grad = null;
					num = j;
				}
				curTime += Time.deltaTime;
				yield return null;
			}
			for (int k = 0; k < processYellow.transform.childCount; k = num + 1)
			{
				ParticleSystem.ColorOverLifetimeModule col3 = processYellow.transform.GetChild(k).GetComponent<ParticleSystem>().colorOverLifetime;
				col3.color = colors[k];
				col3 = default(ParticleSystem.ColorOverLifetimeModule);
				num = k;
			}
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x060049A9 RID: 18857 RVA: 0x002265E0 File Offset: 0x002247E0
		protected static int CalcGradeProcessValue(sbyte grade)
		{
			return 5 * (int)Math.Pow(2.0, (double)grade);
		}

		// Token: 0x060049AA RID: 18858 RVA: 0x00226608 File Offset: 0x00224808
		protected new Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.Parent.StartCoroutine(routine);
		}

		// Token: 0x060049AB RID: 18859 RVA: 0x00226626 File Offset: 0x00224826
		protected new void StopCoroutine(Coroutine routine)
		{
			this.Parent.StopCoroutine(routine);
		}

		// Token: 0x04003313 RID: 13075
		private int _listenerId = -1;

		// Token: 0x04003315 RID: 13077
		private bool _initialized;

		// Token: 0x04003316 RID: 13078
		private readonly Queue<GameObject> _dropItems = new Queue<GameObject>();

		// Token: 0x04003317 RID: 13079
		private Coroutine _dropCoroutine;

		// Token: 0x04003318 RID: 13080
		private int _droppedCount;

		// Token: 0x04003319 RID: 13081
		protected Coroutine HeightCoroutine;

		// Token: 0x0400331A RID: 13082
		private readonly HashSet<UIBase.MonitorDataField> _listeningField = new HashSet<UIBase.MonitorDataField>();

		// Token: 0x0400331B RID: 13083
		protected const float DropItemInterval = 0.3f;

		// Token: 0x0400331C RID: 13084
		private const int MaxDroopCountPerFrame = 2;

		// Token: 0x0400331D RID: 13085
		private const float DropItemDuration = 1f;

		// Token: 0x0400331E RID: 13086
		protected const float TotalAnimTime = 1.5f;

		// Token: 0x0400331F RID: 13087
		protected const float WaterFlowAnimTime = 0.34f;

		// Token: 0x04003320 RID: 13088
		protected const float RemainWaterAnimTime = 0.37f;
	}
}
