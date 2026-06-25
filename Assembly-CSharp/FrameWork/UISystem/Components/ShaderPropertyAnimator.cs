using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001023 RID: 4131
	[RequireComponent(typeof(Renderer))]
	public class ShaderPropertyAnimator : MonoBehaviour
	{
		// Token: 0x0600BD13 RID: 48403 RVA: 0x0055ED24 File Offset: 0x0055CF24
		private void OnEnable()
		{
			this._targetRenderer = base.GetComponent<Renderer>();
			bool flag = this._propertyBlock == null;
			if (flag)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			foreach (ShaderPropertyAnimator.ShaderPropertyTask task in this.tasks)
			{
				base.StartCoroutine(this.RunTask(task));
			}
		}

		// Token: 0x0600BD14 RID: 48404 RVA: 0x0055EDA8 File Offset: 0x0055CFA8
		private void OnDisable()
		{
			bool flag = this._targetRenderer;
			if (flag)
			{
				this._targetRenderer.SetPropertyBlock(null);
			}
		}

		// Token: 0x0600BD15 RID: 48405 RVA: 0x0055EDD4 File Offset: 0x0055CFD4
		private IEnumerator RunTask(ShaderPropertyAnimator.ShaderPropertyTask task)
		{
			bool flag = task.startTime > 0f;
			if (flag)
			{
				yield return new WaitForSeconds(task.startTime);
			}
			float duration = task.endTime - task.startTime;
			bool flag2 = duration <= 0f;
			if (flag2)
			{
				this.SetPropertyValue(task.endValue);
				yield break;
			}
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float progress = Mathf.Clamp01(elapsed / duration);
				float currentValue = Mathf.Lerp(task.startValue, task.endValue, progress);
				this.SetPropertyValue(currentValue);
				yield return null;
			}
			this.SetPropertyValue(task.endValue);
			yield break;
		}

		// Token: 0x0600BD16 RID: 48406 RVA: 0x0055EDEC File Offset: 0x0055CFEC
		private void SetPropertyValue(float value)
		{
			bool flag = !this._targetRenderer;
			if (!flag)
			{
				this._targetRenderer.GetPropertyBlock(this._propertyBlock);
				this._propertyBlock.SetFloat(this.propertyName, value);
				this._targetRenderer.SetPropertyBlock(this._propertyBlock);
			}
		}

		// Token: 0x04009177 RID: 37239
		[Header("Shader Settings")]
		[Tooltip("目标属性名称，默认为 _RongJie")]
		public string propertyName = "_RongJie";

		// Token: 0x04009178 RID: 37240
		[Header("Animation Timeline")]
		[Tooltip("动画序列")]
		public List<ShaderPropertyAnimator.ShaderPropertyTask> tasks;

		// Token: 0x04009179 RID: 37241
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x0400917A RID: 37242
		private Renderer _targetRenderer;

		// Token: 0x0200266A RID: 9834
		[Serializable]
		public class ShaderPropertyTask
		{
			// Token: 0x0400EA88 RID: 60040
			[Tooltip("动画开始的时间（秒）")]
			public float startTime;

			// Token: 0x0400EA89 RID: 60041
			[Tooltip("动画结束的时间（秒）")]
			public float endTime;

			// Token: 0x0400EA8A RID: 60042
			[Tooltip("动画开始时的属性值")]
			[Range(-1f, 1f)]
			public float startValue;

			// Token: 0x0400EA8B RID: 60043
			[Tooltip("动画结束时的属性值")]
			[Range(-1f, 1f)]
			public float endValue;
		}
	}
}
