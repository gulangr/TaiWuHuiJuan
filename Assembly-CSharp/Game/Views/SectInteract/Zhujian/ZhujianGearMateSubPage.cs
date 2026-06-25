using System;
using System.Collections;
using System.Collections.Generic;
using GameData.GameDataBridge;
using GearMate;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009CC RID: 2508
	public abstract class ZhujianGearMateSubPage : MonoBehaviour
	{
		// Token: 0x17000D7D RID: 3453
		// (get) Token: 0x060079B2 RID: 31154 RVA: 0x00388CC6 File Offset: 0x00386EC6
		protected int TotalDropItemCount
		{
			get
			{
				return this._droppedCount + this._dropItems.Count;
			}
		}

		// Token: 0x060079B3 RID: 31155 RVA: 0x00388CDA File Offset: 0x00386EDA
		public virtual void Init(ViewZhujianGearMate parent)
		{
			this.ParentView = parent;
		}

		// Token: 0x060079B4 RID: 31156 RVA: 0x00388CE4 File Offset: 0x00386EE4
		public virtual void OnShow()
		{
			this.IsVisible = true;
			base.gameObject.SetActive(true);
			this.contentRoot.SetActive(false);
			this.ParentView.ShowLoading(true);
			bool flag = this.ListenerId == 0;
			if (flag)
			{
				this.ListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
			}
			this.OnShowDataRequest();
		}

		// Token: 0x060079B5 RID: 31157 RVA: 0x00388D4E File Offset: 0x00386F4E
		protected virtual void OnShowDataRequest()
		{
		}

		// Token: 0x060079B6 RID: 31158 RVA: 0x00388D54 File Offset: 0x00386F54
		public virtual void OnHide()
		{
			this.IsVisible = false;
			base.gameObject.SetActive(false);
			bool flag = this.ListenerId != 0;
			if (flag)
			{
				GameDataBridge.UnregisterListener(this.ListenerId);
				this.ListenerId = 0;
			}
		}

		// Token: 0x060079B7 RID: 31159 RVA: 0x00388D98 File Offset: 0x00386F98
		public virtual void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				this.GearMateId = gearMateId;
			}
		}

		// Token: 0x060079B8 RID: 31160 RVA: 0x00388DBC File Offset: 0x00386FBC
		protected void SetContentReady()
		{
			this.contentRoot.SetActive(true);
			this.ParentView.OnSubPageReady(this);
		}

		// Token: 0x060079B9 RID: 31161 RVA: 0x00388DDC File Offset: 0x00386FDC
		public virtual bool CanQuickHide()
		{
			return true;
		}

		// Token: 0x060079BA RID: 31162 RVA: 0x00388DF0 File Offset: 0x00386FF0
		public virtual void OnListenerIdReady()
		{
			bool flag = this.ListenerId != 0;
			if (!flag)
			{
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.ListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
				}
			}
		}

		// Token: 0x060079BB RID: 31163 RVA: 0x00388E31 File Offset: 0x00387031
		public virtual void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
		}

		// Token: 0x060079BC RID: 31164 RVA: 0x00388E34 File Offset: 0x00387034
		protected virtual void OnDestroy()
		{
			bool flag = this.ListenerId != 0;
			if (flag)
			{
				GameDataBridge.UnregisterListener(this.ListenerId);
				this.ListenerId = 0;
			}
		}

		// Token: 0x060079BD RID: 31165 RVA: 0x00388E64 File Offset: 0x00387064
		protected string GetGearMateName()
		{
			string gearMateName = null;
			bool flag = this.ParentView != null && this.ParentView.DisplayData != null;
			if (flag)
			{
				gearMateName = NameCenter.GetMonasticTitleOrDisplayName(this.ParentView.DisplayData, false);
			}
			bool flag2 = string.IsNullOrEmpty(gearMateName);
			if (flag2)
			{
				gearMateName = LocalStringManager.Get(LanguageKey.LK_GearMate_Tab_0);
			}
			return gearMateName;
		}

		// Token: 0x060079BE RID: 31166 RVA: 0x00388EC7 File Offset: 0x003870C7
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

		// Token: 0x060079BF RID: 31167 RVA: 0x00388EF4 File Offset: 0x003870F4
		protected IEnumerator AnimateAttributeUpgrade(CImage progress, float duration, int index, int startValue, int increaseCount, float startPercent, float endPercent, Action<int, int> onStep, Action onComplete)
		{
			float timePerStage = duration / (float)(increaseCount + 1);
			int currentValue = startValue;
			float target = (increaseCount > 0) ? 1f : endPercent;
			yield return this.StartCoroutine(ZhujianGearMateSubPage.AnimateProgress(progress, startPercent, target, timePerStage));
			bool flag = increaseCount > 0;
			if (flag)
			{
				int num = currentValue + 1;
				currentValue = num;
				onStep(index, num);
				for (int i = 0; i < increaseCount - 1; i = num + 1)
				{
					yield return this.StartCoroutine(ZhujianGearMateSubPage.AnimateProgress(progress, 0f, 1f, timePerStage));
					num = currentValue + 1;
					currentValue = num;
					onStep(index, num);
					num = i;
				}
				yield return this.StartCoroutine(ZhujianGearMateSubPage.AnimateProgress(progress, 0f, endPercent, timePerStage));
			}
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x060079C0 RID: 31168 RVA: 0x00388F53 File Offset: 0x00387153
		protected static IEnumerator AnimateProgress(CImage progress, float start, float end, float duration)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = elapsed / duration;
				progress.fillAmount = Mathf.Lerp(start, end, t);
				yield return null;
			}
			progress.fillAmount = end;
			yield break;
		}

		// Token: 0x060079C1 RID: 31169 RVA: 0x00388F77 File Offset: 0x00387177
		protected virtual void SetMachineWaterHeight(float duration = 0.5f)
		{
		}

		// Token: 0x060079C2 RID: 31170 RVA: 0x00388F7A File Offset: 0x0038717A
		protected void ItemDrop(GameObject itemObj)
		{
			this._dropItems.Enqueue(itemObj);
		}

		// Token: 0x060079C3 RID: 31171 RVA: 0x00388F8C File Offset: 0x0038718C
		public void PlayDropAnimation()
		{
			bool flag = this._dropCoroutine != null;
			if (!flag)
			{
				this._dropCoroutine = this.StartCoroutine(this.QueueDrop());
			}
		}

		// Token: 0x060079C4 RID: 31172 RVA: 0x00388FBC File Offset: 0x003871BC
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

		// Token: 0x060079C5 RID: 31173 RVA: 0x00389048 File Offset: 0x00387248
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
						item.GetComponent<ItemDrop>().OnTrigger += delegate()
						{
							this.SetMachineWaterHeight(1.5f);
						};
					}
					this._droppedCount++;
				}
				yield return new WaitForSeconds(averageInterval);
			}
			yield break;
		}

		// Token: 0x060079C6 RID: 31174 RVA: 0x00389058 File Offset: 0x00387258
		protected static int CalcGradeProcessValue(sbyte grade)
		{
			return 5 * (int)Math.Pow(2.0, (double)grade);
		}

		// Token: 0x060079C7 RID: 31175 RVA: 0x00389080 File Offset: 0x00387280
		protected new Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.ParentView.StartCoroutine(routine);
		}

		// Token: 0x060079C8 RID: 31176 RVA: 0x0038909E File Offset: 0x0038729E
		protected new void StopCoroutine(Coroutine routine)
		{
			this.ParentView.StopCoroutine(routine);
		}

		// Token: 0x04005C40 RID: 23616
		protected ViewZhujianGearMate ParentView;

		// Token: 0x04005C41 RID: 23617
		protected int GearMateId = -1;

		// Token: 0x04005C42 RID: 23618
		protected bool IsVisible;

		// Token: 0x04005C43 RID: 23619
		protected int ListenerId;

		// Token: 0x04005C44 RID: 23620
		private readonly Queue<GameObject> _dropItems = new Queue<GameObject>();

		// Token: 0x04005C45 RID: 23621
		private Coroutine _dropCoroutine;

		// Token: 0x04005C46 RID: 23622
		private int _droppedCount;

		// Token: 0x04005C47 RID: 23623
		protected Coroutine HeightCoroutine;

		// Token: 0x04005C48 RID: 23624
		protected const float DropItemInterval = 0.3f;

		// Token: 0x04005C49 RID: 23625
		private const int MaxDroopCountPerFrame = 2;

		// Token: 0x04005C4A RID: 23626
		private const float DropItemDuration = 1f;

		// Token: 0x04005C4B RID: 23627
		protected const float TotalAnimTime = 1.5f;

		// Token: 0x04005C4C RID: 23628
		protected const float WaterFlowAnimTime = 0.34f;

		// Token: 0x04005C4D RID: 23629
		protected const float RemainWaterAnimTime = 0.37f;

		// Token: 0x04005C4E RID: 23630
		[SerializeField]
		protected GameObject contentRoot;
	}
}
