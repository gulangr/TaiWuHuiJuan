using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using FrameWork.UISystem.Components;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace FrameWork.Tools.OffscreenCulling
{
	// Token: 0x02001035 RID: 4149
	[DisallowMultipleComponent]
	public class OffscreenCuller : MonoBehaviour
	{
		// Token: 0x1700155B RID: 5467
		// (get) Token: 0x0600BD93 RID: 48531 RVA: 0x00561717 File Offset: 0x0055F917
		// (set) Token: 0x0600BD94 RID: 48532 RVA: 0x0056171F File Offset: 0x0055F91F
		public int ManagerIndex { get; set; } = -1;

		// Token: 0x1700155C RID: 5468
		// (get) Token: 0x0600BD95 RID: 48533 RVA: 0x00561728 File Offset: 0x0055F928
		public Transform TargetTransform
		{
			get
			{
				return (this.rectTransform != null) ? this.rectTransform : base.transform;
			}
		}

		// Token: 0x1700155D RID: 5469
		// (get) Token: 0x0600BD96 RID: 48534 RVA: 0x00561746 File Offset: 0x0055F946
		public Vector3[] CachedLocalCorners
		{
			get
			{
				return this._localCorners;
			}
		}

		// Token: 0x1700155E RID: 5470
		// (get) Token: 0x0600BD97 RID: 48535 RVA: 0x0056174E File Offset: 0x0055F94E
		public bool LocalCornersDirty
		{
			get
			{
				return this._localCornersDirty;
			}
		}

		// Token: 0x1700155F RID: 5471
		// (get) Token: 0x0600BD98 RID: 48536 RVA: 0x00561756 File Offset: 0x0055F956
		public float ScreenPadding
		{
			get
			{
				return this.screenPadding;
			}
		}

		// Token: 0x17001560 RID: 5472
		// (get) Token: 0x0600BD99 RID: 48537 RVA: 0x0056175E File Offset: 0x0055F95E
		public bool IsCulled
		{
			get
			{
				return this._culled;
			}
		}

		// Token: 0x0600BD9A RID: 48538 RVA: 0x00561768 File Offset: 0x0055F968
		public static bool IsSupportedComponent(Component comp)
		{
			return comp is SkeletonGraphic || comp is UIParticle || comp is ParticleSystem || comp is Graphic || comp is Animator;
		}

		// Token: 0x0600BD9B RID: 48539 RVA: 0x005617A8 File Offset: 0x0055F9A8
		private void Awake()
		{
			bool flag = this.rectTransform == null;
			if (flag)
			{
				this.rectTransform = (base.transform as RectTransform);
			}
			this._frameOffset = (base.GetInstanceID() & int.MaxValue);
			bool flag2 = this.isLinkToCoveredBehaviour;
			if (flag2)
			{
				this._coveredBehaviour = base.GetComponentInParent<UIViewCoveredBehaviour>();
			}
			bool flag3 = this._coveredBehaviour == null;
			if (flag3)
			{
				this.isLinkToCoveredBehaviour = false;
			}
			this.BuildEntries();
		}

		// Token: 0x0600BD9C RID: 48540 RVA: 0x00561820 File Offset: 0x0055FA20
		private void OnEnable()
		{
			this._localCornersDirty = true;
			bool flag = this.isLinkToCoveredBehaviour && this._coveredBehaviour != null;
			if (flag)
			{
				this._coveredBehaviour.OnAnyUIViewCoveredStateChanged += this.OnCoveredStateChanged;
				this._suppressedByCover = this._coveredBehaviour.IsCovered;
			}
			else
			{
				this._suppressedByCover = false;
			}
			bool flag2 = !this._suppressedByCover;
			if (flag2)
			{
				this.RegisterToManager();
			}
		}

		// Token: 0x0600BD9D RID: 48541 RVA: 0x0056189C File Offset: 0x0055FA9C
		private void OnDisable()
		{
			bool flag = this.isLinkToCoveredBehaviour && this._coveredBehaviour != null;
			if (flag)
			{
				this._coveredBehaviour.OnAnyUIViewCoveredStateChanged -= this.OnCoveredStateChanged;
			}
			this.UnregisterFromManager();
			bool culled = this._culled;
			if (culled)
			{
				this.SetAllEnable();
				this._culled = false;
			}
		}

		// Token: 0x0600BD9E RID: 48542 RVA: 0x00561900 File Offset: 0x0055FB00
		private void OnCoveredStateChanged(UIViewCoveredBehaviour _)
		{
			bool covered = this._coveredBehaviour != null && this._coveredBehaviour.IsCovered;
			bool flag = covered == this._suppressedByCover;
			if (!flag)
			{
				this._suppressedByCover = covered;
				bool flag2 = covered;
				if (flag2)
				{
					this.UnregisterFromManager();
				}
				else
				{
					this._localCornersDirty = true;
					this.RegisterToManager();
				}
			}
		}

		// Token: 0x0600BD9F RID: 48543 RVA: 0x00561964 File Offset: 0x0055FB64
		private void RegisterToManager()
		{
			bool registered = this._registered;
			if (!registered)
			{
				OffscreenCullManager instance = OffscreenCullManager.Instance;
				if (instance != null)
				{
					instance.Register(this);
				}
				this._registered = true;
			}
		}

		// Token: 0x0600BDA0 RID: 48544 RVA: 0x00561998 File Offset: 0x0055FB98
		private void UnregisterFromManager()
		{
			bool flag = !this._registered;
			if (!flag)
			{
				bool flag2 = !OffscreenCullManager.IsQuitting;
				if (flag2)
				{
					OffscreenCullManager instance = OffscreenCullManager.Instance;
					if (instance != null)
					{
						instance.Unregister(this);
					}
				}
				this._registered = false;
			}
		}

		// Token: 0x0600BDA1 RID: 48545 RVA: 0x005619DA File Offset: 0x0055FBDA
		private void OnRectTransformDimensionsChange()
		{
			Profiler.BeginSample("CullerTest.OnRectTransformDimensionsChange");
			this._localCornersDirty = true;
			Profiler.EndSample();
		}

		// Token: 0x0600BDA2 RID: 48546 RVA: 0x005619F8 File Offset: 0x0055FBF8
		public bool WantsCheckThisFrame()
		{
			bool flag = !this._initialized || this._entries == null || this._entries.Length == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int interval = this._culled ? Mathf.Max(1, this.showCheckIntervalFrames) : Mathf.Max(1, this.hideCheckIntervalFrames);
				result = ((Time.frameCount + this._frameOffset) % interval == 0);
			}
			return result;
		}

		// Token: 0x0600BDA3 RID: 48547 RVA: 0x00561A64 File Offset: 0x0055FC64
		public void ApplyOnScreen(bool onScreen)
		{
			if (onScreen)
			{
				bool culled = this._culled;
				if (culled)
				{
					this.SetAllEnable();
					this._culled = false;
				}
			}
			else
			{
				bool flag = this.awalysSuppressOnHide || !this._culled;
				if (flag)
				{
					this.SetAllDisable(true);
					this._culled = true;
				}
			}
		}

		// Token: 0x0600BDA4 RID: 48548 RVA: 0x00561AC0 File Offset: 0x0055FCC0
		public void RefreshLocalCorners()
		{
			bool flag = this.rectTransform != null;
			if (flag)
			{
				this.rectTransform.GetLocalCorners(this._localCorners);
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					this._localCorners[i] = Vector3.zero;
				}
			}
			this._localCornersDirty = false;
		}

		// Token: 0x0600BDA5 RID: 48549 RVA: 0x00561B1C File Offset: 0x0055FD1C
		public Vector3[] GetCachedLocalCornersWithRefresh()
		{
			bool localCornersDirty = this.LocalCornersDirty;
			if (localCornersDirty)
			{
				this.RefreshLocalCorners();
			}
			return this.CachedLocalCorners;
		}

		// Token: 0x0600BDA6 RID: 48550 RVA: 0x00561B47 File Offset: 0x0055FD47
		public void ForceLocalCornersDirty()
		{
			this._localCornersDirty = true;
		}

		// Token: 0x0600BDA7 RID: 48551 RVA: 0x00561B50 File Offset: 0x0055FD50
		public void BuildEntries()
		{
			int i = (this.handledComponents != null) ? this.handledComponents.Count : 0;
			bool flag = this._entries == null || this._entries.Length != i;
			if (flag)
			{
				this._entries = new OffscreenCuller.VisualEntry[i];
			}
			int index = 0;
			for (int j = 0; j < i; j++)
			{
				Component comp = this.handledComponents[j];
				bool flag2 = comp == null;
				if (!flag2)
				{
					this._entries[index++] = new OffscreenCuller.VisualEntry(comp);
				}
			}
			while (index < this._entries.Length)
			{
				this._entries[index] = OffscreenCuller.VisualEntry.Empty;
				index++;
			}
			this._initialized = true;
			bool culled = this._culled;
			if (culled)
			{
				this.SetAllDisable(true);
			}
		}

		// Token: 0x0600BDA8 RID: 48552 RVA: 0x00561C24 File Offset: 0x0055FE24
		private void SetAllDisable(bool takeSnapshot)
		{
			for (int i = 0; i < this._entries.Length; i++)
			{
				OffscreenCuller.VisualEntry visualEntry = this._entries[i];
				if (visualEntry != null)
				{
					visualEntry.Disable(takeSnapshot);
				}
			}
		}

		// Token: 0x0600BDA9 RID: 48553 RVA: 0x00561C60 File Offset: 0x0055FE60
		private void SetAllEnable()
		{
			for (int i = 0; i < this._entries.Length; i++)
			{
				OffscreenCuller.VisualEntry visualEntry = this._entries[i];
				if (visualEntry != null)
				{
					visualEntry.Enable();
				}
			}
		}

		// Token: 0x040091D4 RID: 37332
		[Tooltip("用于检测位置的 RectTransform（通常是本物体）。")]
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x040091D5 RID: 37333
		[Tooltip("是否联动 UIViewCoveredBehaviour 的 IsCovered 状态。")]
		[SerializeField]
		private bool isLinkToCoveredBehaviour = false;

		// Token: 0x040091D6 RID: 37334
		[Tooltip("【显现→隐藏】方向的检测间隔帧数：当前在屏时，每隔多少帧检测一次是否该隐藏。")]
		private int hideCheckIntervalFrames = 5;

		// Token: 0x040091D7 RID: 37335
		[Tooltip("【隐藏→显现】方向的检测间隔帧数：当前已隐藏时，每隔多少帧检测一次是否该恢复。\n通常设小一点让回屏更跟手。")]
		private int showCheckIntervalFrames = 1;

		// Token: 0x040091D8 RID: 37336
		[Tooltip("屏幕边界外扩的像素余量。>0 时物体要完全移出屏幕再多 N 像素才剔除，可减少边界抖动。")]
		[SerializeField]
		private float screenPadding = 0f;

		// Token: 0x040091D9 RID: 37337
		[Header("受控的画面组件（Prefab 中手动配置，或点下方按钮自动拾取）")]
		[SerializeField]
		private List<Component> handledComponents = new List<Component>();

		// Token: 0x040091DA RID: 37338
		[Tooltip("是否总是在hide的时候更新disable状态，防止其他地方开启，会有额外的性能消耗")]
		[SerializeField]
		private bool awalysSuppressOnHide = false;

		// Token: 0x040091DB RID: 37339
		private OffscreenCuller.VisualEntry[] _entries;

		// Token: 0x040091DC RID: 37340
		private readonly Vector3[] _localCorners = new Vector3[4];

		// Token: 0x040091DD RID: 37341
		private bool _culled;

		// Token: 0x040091DE RID: 37342
		private int _frameOffset;

		// Token: 0x040091DF RID: 37343
		private bool _initialized;

		// Token: 0x040091E0 RID: 37344
		private bool _localCornersDirty = true;

		// Token: 0x040091E1 RID: 37345
		private UIViewCoveredBehaviour _coveredBehaviour;

		// Token: 0x040091E2 RID: 37346
		private bool _registered;

		// Token: 0x040091E3 RID: 37347
		private bool _suppressedByCover;

		// Token: 0x0200267E RID: 9854
		private sealed class VisualEntry
		{
			// Token: 0x06011BFD RID: 72701 RVA: 0x00688C44 File Offset: 0x00686E44
			public VisualEntry(Component target)
			{
				this._target = target;
				if (!true)
				{
				}
				OffscreenCuller.VisualEntry.Kind kind;
				if (!(target is SkeletonGraphic))
				{
					if (!(target is UIParticle))
					{
						if (!(target is ParticleSystem))
						{
							if (!(target is Graphic))
							{
								if (!(target is Behaviour))
								{
									if (!(target is Renderer))
									{
										kind = OffscreenCuller.VisualEntry.Kind.Unknown;
									}
									else
									{
										kind = OffscreenCuller.VisualEntry.Kind.Renderer;
									}
								}
								else
								{
									kind = OffscreenCuller.VisualEntry.Kind.Behaviour;
								}
							}
							else
							{
								kind = OffscreenCuller.VisualEntry.Kind.Graphic;
							}
						}
						else
						{
							kind = OffscreenCuller.VisualEntry.Kind.ParticleSystem;
						}
					}
					else
					{
						kind = OffscreenCuller.VisualEntry.Kind.UIParticle;
					}
				}
				else
				{
					kind = OffscreenCuller.VisualEntry.Kind.Skeleton;
				}
				if (!true)
				{
				}
				this._kind = kind;
			}

			// Token: 0x06011BFE RID: 72702 RVA: 0x00688CC0 File Offset: 0x00686EC0
			public void Disable(bool takeSnapshot)
			{
				bool flag = this._target == null;
				if (!flag)
				{
					bool flag2 = takeSnapshot && !this._disabledByUs;
					if (flag2)
					{
						this._disabledByUs = true;
						this.Snapshot();
					}
					this.ApplyDisable();
				}
			}

			// Token: 0x06011BFF RID: 72703 RVA: 0x00688D0C File Offset: 0x00686F0C
			private void Snapshot()
			{
				switch (this._kind)
				{
				case OffscreenCuller.VisualEntry.Kind.Graphic:
					this._wasEnabled = ((Graphic)this._target).enabled;
					break;
				case OffscreenCuller.VisualEntry.Kind.Skeleton:
				{
					SkeletonGraphic s = (SkeletonGraphic)this._target;
					this._wasEnabled = s.enabled;
					this._wasActive = s.freeze;
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.UIParticle:
				{
					UIParticle p = (UIParticle)this._target;
					this._wasEnabled = p.enabled;
					this._wasActive = (p.enabled && !p.isPaused);
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.ParticleSystem:
					this._wasActive = ((ParticleSystem)this._target).isPlaying;
					break;
				case OffscreenCuller.VisualEntry.Kind.Behaviour:
					this._wasEnabled = ((Behaviour)this._target).enabled;
					break;
				case OffscreenCuller.VisualEntry.Kind.Renderer:
					this._wasEnabled = ((Renderer)this._target).enabled;
					break;
				}
			}

			// Token: 0x06011C00 RID: 72704 RVA: 0x00688E08 File Offset: 0x00687008
			private void ApplyDisable()
			{
				switch (this._kind)
				{
				case OffscreenCuller.VisualEntry.Kind.Graphic:
				{
					Graphic g = (Graphic)this._target;
					bool enabled = g.enabled;
					if (enabled)
					{
						g.enabled = false;
					}
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.Skeleton:
				{
					SkeletonGraphic s = (SkeletonGraphic)this._target;
					bool enabled2 = s.enabled;
					if (enabled2)
					{
						s.enabled = false;
					}
					s.freeze = true;
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.UIParticle:
				{
					UIParticle p = (UIParticle)this._target;
					bool flag = !p.isPaused;
					if (flag)
					{
						p.Pause();
					}
					bool enabled3 = p.enabled;
					if (enabled3)
					{
						p.enabled = false;
					}
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.ParticleSystem:
				{
					ParticleSystem ps = (ParticleSystem)this._target;
					bool isPlaying = ps.isPlaying;
					if (isPlaying)
					{
						ps.Pause(true);
					}
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.Behaviour:
				{
					Behaviour b = (Behaviour)this._target;
					bool enabled4 = b.enabled;
					if (enabled4)
					{
						b.enabled = false;
					}
					break;
				}
				case OffscreenCuller.VisualEntry.Kind.Renderer:
				{
					Renderer r = (Renderer)this._target;
					bool enabled5 = r.enabled;
					if (enabled5)
					{
						r.enabled = false;
					}
					break;
				}
				}
			}

			// Token: 0x06011C01 RID: 72705 RVA: 0x00688F48 File Offset: 0x00687148
			public void Enable()
			{
				bool flag = this._target == null || !this._disabledByUs;
				if (!flag)
				{
					this._disabledByUs = false;
					switch (this._kind)
					{
					case OffscreenCuller.VisualEntry.Kind.Graphic:
					{
						Graphic g = (Graphic)this._target;
						bool wasEnabled = this._wasEnabled;
						if (wasEnabled)
						{
							g.enabled = true;
						}
						break;
					}
					case OffscreenCuller.VisualEntry.Kind.Skeleton:
					{
						SkeletonGraphic s = (SkeletonGraphic)this._target;
						s.freeze = this._wasActive;
						bool wasEnabled2 = this._wasEnabled;
						if (wasEnabled2)
						{
							s.enabled = true;
						}
						break;
					}
					case OffscreenCuller.VisualEntry.Kind.UIParticle:
					{
						UIParticle p = (UIParticle)this._target;
						bool wasEnabled3 = this._wasEnabled;
						if (wasEnabled3)
						{
							p.enabled = true;
						}
						bool wasActive = this._wasActive;
						if (wasActive)
						{
							p.Play();
						}
						break;
					}
					case OffscreenCuller.VisualEntry.Kind.ParticleSystem:
					{
						ParticleSystem ps = (ParticleSystem)this._target;
						bool wasActive2 = this._wasActive;
						if (wasActive2)
						{
							ps.Play(true);
						}
						break;
					}
					case OffscreenCuller.VisualEntry.Kind.Behaviour:
					{
						Behaviour b = (Behaviour)this._target;
						bool wasEnabled4 = this._wasEnabled;
						if (wasEnabled4)
						{
							b.enabled = true;
						}
						break;
					}
					case OffscreenCuller.VisualEntry.Kind.Renderer:
					{
						Renderer r = (Renderer)this._target;
						bool wasEnabled5 = this._wasEnabled;
						if (wasEnabled5)
						{
							r.enabled = true;
						}
						break;
					}
					}
				}
			}

			// Token: 0x0400EADD RID: 60125
			public static readonly OffscreenCuller.VisualEntry Empty = new OffscreenCuller.VisualEntry(null);

			// Token: 0x0400EADE RID: 60126
			private readonly OffscreenCuller.VisualEntry.Kind _kind;

			// Token: 0x0400EADF RID: 60127
			private readonly Component _target;

			// Token: 0x0400EAE0 RID: 60128
			private bool _wasEnabled;

			// Token: 0x0400EAE1 RID: 60129
			private bool _wasActive;

			// Token: 0x0400EAE2 RID: 60130
			private bool _disabledByUs;

			// Token: 0x020026D8 RID: 9944
			private enum Kind
			{
				// Token: 0x0400EBB9 RID: 60345
				Graphic,
				// Token: 0x0400EBBA RID: 60346
				Skeleton,
				// Token: 0x0400EBBB RID: 60347
				UIParticle,
				// Token: 0x0400EBBC RID: 60348
				ParticleSystem,
				// Token: 0x0400EBBD RID: 60349
				Behaviour,
				// Token: 0x0400EBBE RID: 60350
				Renderer,
				// Token: 0x0400EBBF RID: 60351
				Unknown
			}
		}
	}
}
