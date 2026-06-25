using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001025 RID: 4133
	public class TemplatedContainerAssemblyNew : MonoBehaviour
	{
		// Token: 0x0600BD21 RID: 48417 RVA: 0x0055F12C File Offset: 0x0055D32C
		public void Rebuild<T, F>(IReadOnlyList<F> list, Action<T, F> action)
		{
			this.Rebuild<T>(list.Count, delegate(T t, int i)
			{
				action(t, list[i]);
			});
		}

		// Token: 0x0600BD22 RID: 48418 RVA: 0x0055F16C File Offset: 0x0055D36C
		public void Rebuild<T>(int count, Action<T, int> action)
		{
			bool templateInner = this.template.transform.IsChildOf(this.container);
			this.CachedCount = count;
			bool flag = this.setTemplateInactive && !templateInner;
			if (flag)
			{
				this.template.gameObject.SetActive(false);
			}
			this.RemoveRedundantItems(count);
			bool flag2 = this._coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._coroutine);
			}
			this._coroutine = this.GenerateAndRefreshEnoughItems<T>(count, action);
			TemplatedContainerAssemblyNew.BuildType buildType = this.itemsBuildType;
			TemplatedContainerAssemblyNew.BuildType buildType2 = buildType;
			if (buildType2 != TemplatedContainerAssemblyNew.BuildType.Coroutine)
			{
				while (this._coroutine.MoveNext())
				{
				}
			}
			else
			{
				base.StartCoroutine(this._coroutine);
			}
		}

		// Token: 0x0600BD23 RID: 48419 RVA: 0x0055F228 File Offset: 0x0055D428
		public void HandleChild(Action<GameObject, int> action)
		{
			int index = 0;
			for (int i = 0; i < this.container.childCount; i++)
			{
				Transform child = this.container.GetChild(i);
				bool flag = this.ignoreObjects.Contains(child.gameObject) || this.ignoreObjectsFront.Contains(child.gameObject);
				if (!flag)
				{
					action(child.gameObject, index++);
				}
			}
		}

		// Token: 0x17001551 RID: 5457
		// (get) Token: 0x0600BD24 RID: 48420 RVA: 0x0055F2A2 File Offset: 0x0055D4A2
		// (set) Token: 0x0600BD25 RID: 48421 RVA: 0x0055F2AA File Offset: 0x0055D4AA
		internal int CachedCount { get; private set; }

		// Token: 0x0600BD26 RID: 48422 RVA: 0x0055F2B4 File Offset: 0x0055D4B4
		private int GetUsefulCount()
		{
			return this.container.childCount - this.ignoreObjects.Count - this.ignoreObjectsFront.Count;
		}

		// Token: 0x0600BD27 RID: 48423 RVA: 0x0055F2E9 File Offset: 0x0055D4E9
		private IEnumerator GenerateAndRefreshEnoughItems<T>(int targetCount, Action<T, int> action)
		{
			int childCount = this.container.childCount;
			int index = 0;
			int i = 0;
			int num;
			while (i < childCount && index < targetCount)
			{
				Transform child = this.container.GetChild(i);
				bool flag = this.ignoreObjects.Contains(child.gameObject) || this.ignoreObjectsFront.Contains(child.gameObject);
				if (!flag)
				{
					child.gameObject.SetActive(true);
					child.localScale = Vector3.one;
					if (action != null)
					{
						T component = child.GetComponent<T>();
						num = index;
						index = num + 1;
						action(component, num);
					}
					child = null;
				}
				num = i;
				i = num + 1;
			}
			while (index < targetCount)
			{
				RectTransform child2 = Object.Instantiate<RectTransform>(this.template, this.container, false);
				child2.localScale = Vector3.one;
				child2.gameObject.SetActive(true);
				if (action != null)
				{
					T component2 = child2.GetComponent<T>();
					num = index;
					index = num + 1;
					action(component2, num);
				}
				yield return new WaitForEndOfFrame();
				child2 = null;
			}
			for (int j = 0; j < this.ignoreObjects.Count; j = num + 1)
			{
				GameObject go = this.ignoreObjects[j];
				bool flag2 = go != null;
				if (flag2)
				{
					go.transform.SetAsLastSibling();
				}
				go = null;
				num = j;
			}
			for (int k = this.ignoreObjectsFront.Count - 1; k >= 0; k = num - 1)
			{
				GameObject go2 = this.ignoreObjectsFront[k];
				bool flag3 = go2 != null;
				if (flag3)
				{
					go2.transform.SetAsFirstSibling();
				}
				go2 = null;
				num = k;
			}
			this._coroutine = null;
			UnityEvent unityEvent = this.onAfterBuild;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			yield break;
		}

		// Token: 0x0600BD28 RID: 48424 RVA: 0x0055F308 File Offset: 0x0055D508
		private void RemoveRedundantItems(int targetCount)
		{
			int childCount = this.container.childCount;
			int usefulCount = this.GetUsefulCount();
			int i = childCount - 1;
			while (i >= 0 && usefulCount > targetCount)
			{
				Transform child = this.container.GetChild(i);
				bool flag = this.ignoreObjects.Contains(child.gameObject) || this.ignoreObjectsFront.Contains(child.gameObject);
				if (!flag)
				{
					bool flag2 = this.onlyHideRemovingItems || child == this.template.transform;
					if (flag2)
					{
						child.gameObject.SetActive(false);
					}
					else
					{
						Object.DestroyImmediate(child.gameObject);
					}
					usefulCount--;
				}
				i--;
			}
		}

		// Token: 0x04009184 RID: 37252
		[SerializeField]
		public RectTransform template;

		// Token: 0x04009185 RID: 37253
		[SerializeField]
		public RectTransform container;

		// Token: 0x04009186 RID: 37254
		[SerializeField]
		public List<GameObject> ignoreObjects = new List<GameObject>();

		// Token: 0x04009187 RID: 37255
		[SerializeField]
		public List<GameObject> ignoreObjectsFront = new List<GameObject>();

		// Token: 0x04009188 RID: 37256
		public TemplatedContainerAssemblyNew.BuildType itemsBuildType = TemplatedContainerAssemblyNew.BuildType.Standard;

		// Token: 0x04009189 RID: 37257
		public bool onlyHideRemovingItems = true;

		// Token: 0x0400918A RID: 37258
		public UnityEvent onAfterBuild;

		// Token: 0x0400918B RID: 37259
		[SerializeField]
		private bool setTemplateInactive = true;

		// Token: 0x0400918D RID: 37261
		private IEnumerator _coroutine;

		// Token: 0x0200266F RID: 9839
		public enum BuildType
		{
			// Token: 0x0400EAA8 RID: 60072
			Standard,
			// Token: 0x0400EAA9 RID: 60073
			Coroutine
		}
	}
}
