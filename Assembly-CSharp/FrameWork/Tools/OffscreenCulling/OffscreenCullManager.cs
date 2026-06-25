using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace FrameWork.Tools.OffscreenCulling
{
	// Token: 0x02001038 RID: 4152
	[DefaultExecutionOrder(10000)]
	public class OffscreenCullManager : MonoBehaviour
	{
		// Token: 0x17001561 RID: 5473
		// (get) Token: 0x0600BDAD RID: 48557 RVA: 0x00561EAD File Offset: 0x005600AD
		public static bool IsQuitting
		{
			get
			{
				return OffscreenCullManager._appQuitting;
			}
		}

		// Token: 0x17001562 RID: 5474
		// (get) Token: 0x0600BDAE RID: 48558 RVA: 0x00561EB4 File Offset: 0x005600B4
		public static OffscreenCullManager Instance
		{
			get
			{
				bool appQuitting = OffscreenCullManager._appQuitting;
				OffscreenCullManager result;
				if (appQuitting)
				{
					result = null;
				}
				else
				{
					bool flag = OffscreenCullManager._instance == null;
					if (flag)
					{
						GameObject go = new GameObject("[OffscreenCullManager]");
						Object.DontDestroyOnLoad(go);
						OffscreenCullManager._instance = go.AddComponent<OffscreenCullManager>();
					}
					result = OffscreenCullManager._instance;
				}
				return result;
			}
		}

		// Token: 0x0600BDAF RID: 48559 RVA: 0x00561F08 File Offset: 0x00560108
		public void Register(OffscreenCuller culler)
		{
			Profiler.BeginSample("CullerTest.Register");
			bool flag = culler == null;
			if (!flag)
			{
				this.EnsureCullerCapacity(this._cullerCount + 1);
				culler.ManagerIndex = this._cullerCount;
				this._cullers[this._cullerCount] = culler;
				this._cullerCount++;
				this._arraysDirty = true;
				Profiler.EndSample();
			}
		}

		// Token: 0x0600BDB0 RID: 48560 RVA: 0x00561F74 File Offset: 0x00560174
		public void Unregister(OffscreenCuller culler)
		{
			Profiler.BeginSample("CullerTest.Unregister");
			bool flag = culler == null;
			if (!flag)
			{
				int idx = culler.ManagerIndex;
				bool flag2 = idx < 0 || idx >= this._cullerCount || this._cullers[idx] != culler;
				if (flag2)
				{
					idx = this.FindCullerIndex(culler);
					bool flag3 = idx < 0;
					if (flag3)
					{
						return;
					}
				}
				this.SwapRemove(idx);
				this._cullerCount--;
				culler.ManagerIndex = -1;
				this._arraysDirty = true;
				Profiler.EndSample();
			}
		}

		// Token: 0x0600BDB1 RID: 48561 RVA: 0x00562004 File Offset: 0x00560204
		private void SwapRemove(int idx)
		{
			int last = this._cullerCount - 1;
			OffscreenCuller moved = this._cullers[last];
			this._cullers[idx] = moved;
			bool flag = idx != last && moved != null;
			if (flag)
			{
				moved.ManagerIndex = idx;
			}
			this._cullers[last] = null;
		}

		// Token: 0x0600BDB2 RID: 48562 RVA: 0x00562050 File Offset: 0x00560250
		private void LateUpdate()
		{
			Profiler.BeginSample("CullerTest.LateUpdate");
			this.CompleteJobIfRunning();
			int count = this._cullerCount;
			bool flag = count == 0;
			if (!flag)
			{
				this.EnsureArrays(count);
				Camera uiCamera = (UIManager.Instance != null) ? UIManager.Instance.UiCamera : null;
				bool flag2 = uiCamera == null;
				if (!flag2)
				{
					for (int i = 0; i < count; i++)
					{
						OffscreenCuller c = this._cullers[i];
						bool localCornersDirty = c.LocalCornersDirty;
						if (localCornersDirty)
						{
							c.RefreshLocalCorners();
							Vector3[] lc = c.CachedLocalCorners;
							int b = i * 4;
							this._localCorners[b] = lc[0];
							this._localCorners[b + 1] = lc[1];
							this._localCorners[b + 2] = lc[2];
							this._localCorners[b + 3] = lc[3];
						}
						this._paddings[i] = c.ScreenPadding;
					}
					Matrix4x4 vp = uiCamera.projectionMatrix * uiCamera.worldToCameraMatrix;
					OffscreenCullJob job = new OffscreenCullJob
					{
						LocalCorners = this._localCorners,
						Paddings = this._paddings,
						VP = vp,
						ScreenWidth = (float)Screen.width,
						ScreenHeight = (float)Screen.height,
						Results = this._results
					};
					this._handle = job.Schedule(this._transforms, default(JobHandle));
					this._jobRunning = true;
					Profiler.EndSample();
				}
			}
		}

		// Token: 0x0600BDB3 RID: 48563 RVA: 0x0056220C File Offset: 0x0056040C
		private void OnEnable()
		{
			Canvas.preWillRenderCanvases += this.ApplyResults;
		}

		// Token: 0x0600BDB4 RID: 48564 RVA: 0x00562221 File Offset: 0x00560421
		private void OnDisable()
		{
			Canvas.preWillRenderCanvases -= this.ApplyResults;
		}

		// Token: 0x0600BDB5 RID: 48565 RVA: 0x00562238 File Offset: 0x00560438
		private void ApplyResults()
		{
			Profiler.BeginSample("CullerTest.ApplyResults");
			bool jobRunning = this._jobRunning;
			if (jobRunning)
			{
				Profiler.BeginSample("CullerTest.WaitForJob");
				this._handle.Complete();
				this._jobRunning = false;
				Profiler.EndSample();
				int count = this._cullerCount;
				int i = 0;
				while (i < count && i < this._results.Length)
				{
					OffscreenCuller c = this._cullers[i];
					bool flag = c == null;
					if (!flag)
					{
						Profiler.BeginSample("CullerTest.WantsCheckThisFrame");
						bool check = c.WantsCheckThisFrame();
						Profiler.EndSample();
						bool flag2 = !check;
						if (!flag2)
						{
							Profiler.BeginSample("CullerTest.ApplyResultOne");
							c.ApplyOnScreen(this._results[i]);
							Profiler.EndSample();
						}
					}
					i++;
				}
			}
			Profiler.EndSample();
		}

		// Token: 0x0600BDB6 RID: 48566 RVA: 0x0056231C File Offset: 0x0056051C
		private void CompleteJobIfRunning()
		{
			bool jobRunning = this._jobRunning;
			if (jobRunning)
			{
				Profiler.BeginSample("CullerTest.WaitForJob");
				this._handle.Complete();
				this._jobRunning = false;
				Profiler.EndSample();
			}
		}

		// Token: 0x0600BDB7 RID: 48567 RVA: 0x0056235C File Offset: 0x0056055C
		private void EnsureArrays(int count)
		{
			bool flag = !this._arraysDirty && this._transforms.isCreated && count <= this._capacity;
			if (!flag)
			{
				this.CompleteJobIfRunning();
				this.RebuildArrays(count);
				this._arraysDirty = false;
			}
		}

		// Token: 0x0600BDB8 RID: 48568 RVA: 0x005623AC File Offset: 0x005605AC
		private void RebuildArrays(int count)
		{
			Profiler.BeginSample("CullerTest.RebuildArrays");
			int cap = Mathf.Max(1024, Mathf.NextPowerOfTwo(count));
			this.DisposeArrays();
			this._transforms = new TransformAccessArray(count, -1);
			for (int i = 0; i < count; i++)
			{
				this._transforms.Add(this._cullers[i].TargetTransform);
			}
			this._localCorners = new NativeArray<Vector3>(cap * 4, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._paddings = new NativeArray<float>(cap, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._results = new NativeArray<bool>(cap, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._capacity = cap;
			for (int j = 0; j < count; j++)
			{
				OffscreenCuller c = this._cullers[j];
				Vector3[] lc = c.GetCachedLocalCornersWithRefresh();
				int b = j * 4;
				this._localCorners[b] = lc[0];
				this._localCorners[b + 1] = lc[1];
				this._localCorners[b + 2] = lc[2];
				this._localCorners[b + 3] = lc[3];
				this._paddings[j] = c.ScreenPadding;
			}
			Profiler.EndSample();
		}

		// Token: 0x0600BDB9 RID: 48569 RVA: 0x005624F0 File Offset: 0x005606F0
		private void EnsureCullerCapacity(int requiredCount)
		{
			bool flag = this._cullers.Length >= requiredCount;
			if (!flag)
			{
				int newCapacity = Mathf.Max(1024, Mathf.NextPowerOfTwo(requiredCount));
				OffscreenCuller[] newBuffer = new OffscreenCuller[newCapacity];
				bool flag2 = this._cullerCount > 0;
				if (flag2)
				{
					Array.Copy(this._cullers, newBuffer, this._cullerCount);
				}
				this._cullers = newBuffer;
			}
		}

		// Token: 0x0600BDBA RID: 48570 RVA: 0x00562554 File Offset: 0x00560754
		private int FindCullerIndex(OffscreenCuller culler)
		{
			for (int i = 0; i < this._cullerCount; i++)
			{
				bool flag = this._cullers[i] == culler;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600BDBB RID: 48571 RVA: 0x00562594 File Offset: 0x00560794
		private void DisposeArrays()
		{
			bool isCreated = this._transforms.isCreated;
			if (isCreated)
			{
				this._transforms.Dispose();
			}
			bool isCreated2 = this._localCorners.IsCreated;
			if (isCreated2)
			{
				this._localCorners.Dispose();
			}
			bool isCreated3 = this._paddings.IsCreated;
			if (isCreated3)
			{
				this._paddings.Dispose();
			}
			bool isCreated4 = this._results.IsCreated;
			if (isCreated4)
			{
				this._results.Dispose();
			}
		}

		// Token: 0x0600BDBC RID: 48572 RVA: 0x00562610 File Offset: 0x00560810
		private void OnDestroy()
		{
			this.CompleteJobIfRunning();
			this.DisposeArrays();
			bool flag = OffscreenCullManager._instance == this;
			if (flag)
			{
				OffscreenCullManager._instance = null;
			}
		}

		// Token: 0x0600BDBD RID: 48573 RVA: 0x00562641 File Offset: 0x00560841
		private void OnApplicationQuit()
		{
			OffscreenCullManager._appQuitting = true;
		}

		// Token: 0x040091EE RID: 37358
		private static OffscreenCullManager _instance;

		// Token: 0x040091EF RID: 37359
		private static bool _appQuitting;

		// Token: 0x040091F0 RID: 37360
		private OffscreenCuller[] _cullers = new OffscreenCuller[1024];

		// Token: 0x040091F1 RID: 37361
		private int _cullerCount;

		// Token: 0x040091F2 RID: 37362
		private TransformAccessArray _transforms;

		// Token: 0x040091F3 RID: 37363
		private NativeArray<Vector3> _localCorners;

		// Token: 0x040091F4 RID: 37364
		private NativeArray<float> _paddings;

		// Token: 0x040091F5 RID: 37365
		private NativeArray<bool> _results;

		// Token: 0x040091F6 RID: 37366
		private int _capacity;

		// Token: 0x040091F7 RID: 37367
		private bool _arraysDirty;

		// Token: 0x040091F8 RID: 37368
		private JobHandle _handle;

		// Token: 0x040091F9 RID: 37369
		private bool _jobRunning;

		// Token: 0x040091FA RID: 37370
		private const int InitialCapacity = 1024;
	}
}
