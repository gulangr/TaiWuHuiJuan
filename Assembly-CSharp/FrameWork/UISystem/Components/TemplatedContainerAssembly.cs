using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001024 RID: 4132
	[Obsolete("使用TemplatedContainerAssemblyNew")]
	public class TemplatedContainerAssembly : MonoBehaviour
	{
		// Token: 0x0600BD18 RID: 48408 RVA: 0x0055EE5C File Offset: 0x0055D05C
		public void Rebuild(int count, Action<Refers, int> action)
		{
			bool templateInner = this.Template.transform.IsChildOf(this.Container);
			this.CachedCount = count;
			bool flag = !templateInner;
			if (flag)
			{
				this.Template.gameObject.SetActive(false);
			}
			this.RemoveRedundantItems(count);
			bool flag2 = this._coroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._coroutine);
			}
			this._coroutine = this.GenerateAndRefreshEnoughItems(count, action);
			TemplatedContainerAssembly.BuildType itemsBuildType = this.ItemsBuildType;
			TemplatedContainerAssembly.BuildType buildType = itemsBuildType;
			if (buildType != TemplatedContainerAssembly.BuildType.Coroutine)
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

		// Token: 0x0600BD19 RID: 48409 RVA: 0x0055EF0C File Offset: 0x0055D10C
		public void Rebuild<T>(IList<T> list, Action<Refers, int, T> action)
		{
			this.Rebuild(list.Count, delegate(Refers refers, int index)
			{
				action(refers, index, list[index]);
			});
		}

		// Token: 0x0600BD1A RID: 48410 RVA: 0x0055EF4C File Offset: 0x0055D14C
		public void HandleChild(Action<Refers, int> action)
		{
			int index = 0;
			for (int i = 0; i < this.Container.childCount; i++)
			{
				Transform child = this.Container.GetChild(i);
				Refers refer = child.GetComponent<Refers>();
				bool flag = this.IgnoreObjects.Contains(child.gameObject) || this.ignoreObjectsFront.Contains(child.gameObject) || refer == null;
				if (!flag)
				{
					action(refer, index++);
				}
			}
		}

		// Token: 0x17001550 RID: 5456
		// (get) Token: 0x0600BD1B RID: 48411 RVA: 0x0055EFD3 File Offset: 0x0055D1D3
		// (set) Token: 0x0600BD1C RID: 48412 RVA: 0x0055EFDB File Offset: 0x0055D1DB
		internal int CachedCount { get; private set; }

		// Token: 0x0600BD1D RID: 48413 RVA: 0x0055EFE4 File Offset: 0x0055D1E4
		private int GetUsefulCount()
		{
			return this.Container.childCount - this.IgnoreObjects.Count - this.ignoreObjectsFront.Count;
		}

		// Token: 0x0600BD1E RID: 48414 RVA: 0x0055F019 File Offset: 0x0055D219
		private IEnumerator GenerateAndRefreshEnoughItems(int targetCount, Action<Refers, int> action)
		{
			int childCount = this.Container.childCount;
			int index = 0;
			int i = 0;
			int num;
			while (i < childCount && index < targetCount)
			{
				Transform child = this.Container.GetChild(i);
				bool flag = this.IgnoreObjects.Contains(child.gameObject) || this.ignoreObjectsFront.Contains(child.gameObject);
				if (!flag)
				{
					child.gameObject.SetActive(true);
					child.localScale = Vector3.one;
					if (action != null)
					{
						Refers component = child.GetComponent<Refers>();
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
				RectTransform child2 = Object.Instantiate<Refers>(this.Template).RectTransform;
				child2.SetParent(this.Container, false);
				child2.localScale = Vector3.one;
				child2.gameObject.SetActive(true);
				if (action != null)
				{
					Refers component2 = child2.GetComponent<Refers>();
					num = index;
					index = num + 1;
					action(component2, num);
				}
				yield return new WaitForEndOfFrame();
				child2 = null;
			}
			for (int j = 0; j < this.IgnoreObjects.Count; j = num + 1)
			{
				GameObject go = this.IgnoreObjects[j];
				go.transform.SetAsLastSibling();
				go = null;
				num = j;
			}
			for (int k = 0; k < this.ignoreObjectsFront.Count; k = num + 1)
			{
				GameObject go2 = this.ignoreObjectsFront[k];
				go2.transform.SetAsFirstSibling();
				go2 = null;
				num = k;
			}
			this._coroutine = null;
			UnityEvent onAfterBuild = this.OnAfterBuild;
			if (onAfterBuild != null)
			{
				onAfterBuild.Invoke();
			}
			yield break;
		}

		// Token: 0x0600BD1F RID: 48415 RVA: 0x0055F038 File Offset: 0x0055D238
		private void RemoveRedundantItems(int targetCount)
		{
			int childCount = this.Container.childCount;
			int usefulCount = this.GetUsefulCount();
			int i = childCount - 1;
			while (i >= 0 && usefulCount > targetCount)
			{
				Transform child = this.Container.GetChild(i);
				bool flag = this.IgnoreObjects.Contains(child.gameObject) || this.ignoreObjectsFront.Contains(child.gameObject);
				if (!flag)
				{
					bool flag2 = this.OnlyHideRemovingItems || child == this.Template.transform;
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

		// Token: 0x0400917B RID: 37243
		public Refers Template;

		// Token: 0x0400917C RID: 37244
		public RectTransform Container;

		// Token: 0x0400917D RID: 37245
		public TemplatedContainerAssembly.BuildType ItemsBuildType = TemplatedContainerAssembly.BuildType.Standard;

		// Token: 0x0400917E RID: 37246
		public bool OnlyHideRemovingItems = true;

		// Token: 0x0400917F RID: 37247
		public UnityEvent OnAfterBuild;

		// Token: 0x04009180 RID: 37248
		[SerializeField]
		public List<GameObject> IgnoreObjects = new List<GameObject>();

		// Token: 0x04009181 RID: 37249
		[SerializeField]
		public List<GameObject> ignoreObjectsFront = new List<GameObject>();

		// Token: 0x04009183 RID: 37251
		private IEnumerator _coroutine;

		// Token: 0x0200266C RID: 9836
		public enum BuildType
		{
			// Token: 0x0400EA95 RID: 60053
			Standard,
			// Token: 0x0400EA96 RID: 60054
			Coroutine
		}
	}
}
