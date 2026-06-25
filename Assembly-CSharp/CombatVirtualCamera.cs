using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class CombatVirtualCamera
{
	// Token: 0x0600134E RID: 4942 RVA: 0x00076DB0 File Offset: 0x00074FB0
	private static bool IsFinite(float value)
	{
		return !float.IsNaN(value) && !float.IsInfinity(value);
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x00076DD8 File Offset: 0x00074FD8
	public CombatVirtualCamera(CombatVirtualCamera.Config config)
	{
		this._config = config;
		this._scale = 1f;
		this._targetScale = 1f;
		this._position = 0f;
		this._targetPosition = 0f;
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x00076E41 File Offset: 0x00075041
	public void SetScaleFactor(float scaleFactor)
	{
		this._configScaleFactor = scaleFactor;
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x00076E4C File Offset: 0x0007504C
	public void Reset()
	{
		this._scale = 1f;
		this._targetScale = 1f;
		this._position = 0f;
		this._targetPosition = 0f;
		this._lockedPosition = false;
		this._lockedScale = false;
		this._lockedTargetScale = false;
		this._manuallySetTargetScale = 1f;
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x00076EA6 File Offset: 0x000750A6
	public void LockPosition()
	{
		this._lockedPosition = true;
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x00076EB0 File Offset: 0x000750B0
	public void UnlockPosition()
	{
		this._lockedPosition = false;
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x00076EBA File Offset: 0x000750BA
	public void LockScale()
	{
		this._lockedScale = true;
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x00076EC4 File Offset: 0x000750C4
	public void UnlockScale()
	{
		this._lockedScale = false;
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00076ED0 File Offset: 0x000750D0
	public static float Distance(float a, float b)
	{
		return Mathf.Abs(a - b);
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x00076EEC File Offset: 0x000750EC
	public float GetScreenDistance(float selfScreenPos, float enemyScreenPos)
	{
		bool lockedTargetScale = this._lockedTargetScale;
		float result;
		if (lockedTargetScale)
		{
			result = this._manuallySetTargetScale;
		}
		else
		{
			result = CombatVirtualCamera.Distance(selfScreenPos, enemyScreenPos);
		}
		return result;
	}

	// Token: 0x06001358 RID: 4952 RVA: 0x00076F18 File Offset: 0x00075118
	public void UpdateTargetData(float selfScreenPos, float enemyScreenPos)
	{
		bool flag = !CombatVirtualCamera.IsFinite(selfScreenPos) || !CombatVirtualCamera.IsFinite(enemyScreenPos);
		if (!flag)
		{
			float newPosX = (selfScreenPos + enemyScreenPos) / 2f;
			this._targetPosition = newPosX;
			bool flag2 = !this._lockedTargetScale;
			if (flag2)
			{
				float screenDistance = CombatVirtualCamera.Distance(selfScreenPos, enemyScreenPos);
				float newScale = this.CalcScaleByScreenDistance(screenDistance);
				this._targetScale = newScale;
			}
		}
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00076F7C File Offset: 0x0007517C
	public float GetEquivalentScaleFactor()
	{
		return this.CalcEquivalentScaleFactor(this._scale);
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x00076F9C File Offset: 0x0007519C
	private float CalcScaleByScreenDistance(float screenDistance)
	{
		float scaleFactor = this.CalcScaleFactor(screenDistance);
		float newScale = Mathf.Lerp(this._config.MinSpineScale, this._config.MaxSpineScale, scaleFactor);
		return newScale * this._configScaleFactor;
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x00076FDC File Offset: 0x000751DC
	private float CalcEquivalentScaleFactor(float scale)
	{
		return Mathf.Clamp01((scale - this._config.MinSpineScale) / (this._config.MaxSpineScale - this._config.MinSpineScale));
	}

	// Token: 0x0600135C RID: 4956 RVA: 0x0007701A File Offset: 0x0007521A
	public void SetTargetScaleManually(float targetScale)
	{
		this._lockedTargetScale = true;
		this._targetScale = targetScale;
	}

	// Token: 0x0600135D RID: 4957 RVA: 0x0007702C File Offset: 0x0007522C
	public void SetTargetScaleManuallyByScreenDistance(float screenDistance)
	{
		this._manuallySetTargetScale = screenDistance;
		float scale = this.CalcScaleByScreenDistance(screenDistance);
		this.SetTargetScaleManually(scale);
	}

	// Token: 0x0600135E RID: 4958 RVA: 0x00077051 File Offset: 0x00075251
	public void UnlockTargetScale()
	{
		this._lockedTargetScale = false;
	}

	// Token: 0x0600135F RID: 4959 RVA: 0x0007705C File Offset: 0x0007525C
	public float TweenValueTo(float targetValue, float currentValue, float deltaTime, string key)
	{
		bool flag = !CombatVirtualCamera.IsFinite(targetValue);
		float result;
		if (flag)
		{
			result = currentValue;
		}
		else
		{
			bool flag2 = !CombatVirtualCamera.IsFinite(currentValue);
			if (flag2)
			{
				currentValue = targetValue;
			}
			CombatVirtualCamera.TweenState state;
			bool flag3 = !this._velocityDict.TryGetValue(key, out state);
			if (flag3)
			{
				state = new CombatVirtualCamera.TweenState
				{
					Velocity = 0f
				};
				this._velocityDict[key] = state;
			}
			bool flag4 = !CombatVirtualCamera.IsFinite(state.Velocity);
			if (flag4)
			{
				state.Velocity = 0f;
			}
			float target = Mathf.SmoothDamp(currentValue, targetValue, ref state.Velocity, 0.15f, float.PositiveInfinity, deltaTime);
			result = target;
		}
		return result;
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x00077108 File Offset: 0x00075308
	public void OnUpdate(float deltaTime)
	{
		bool flag = !CombatVirtualCamera.IsFinite(deltaTime) || deltaTime <= 0f;
		if (!flag)
		{
			bool flag2 = !CombatVirtualCamera.IsFinite(this._scale);
			if (flag2)
			{
				this._scale = 1f;
			}
			bool flag3 = !CombatVirtualCamera.IsFinite(this._position);
			if (flag3)
			{
				this._position = 0f;
			}
			bool flag4 = !CombatVirtualCamera.IsFinite(this._targetScale);
			if (flag4)
			{
				this._targetScale = this._scale;
			}
			bool flag5 = !CombatVirtualCamera.IsFinite(this._targetPosition);
			if (flag5)
			{
				this._targetPosition = this._position;
			}
			bool flag6 = !this._lockedScale;
			if (flag6)
			{
				this._scale = this.TweenValueTo(this._targetScale, this._scale, deltaTime, "internal_scale");
			}
			bool flag7 = !this._lockedPosition;
			if (flag7)
			{
				this._position = this.TweenValueTo(this._targetPosition, this._position, deltaTime, "internal_position");
			}
		}
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x00077208 File Offset: 0x00075408
	private float CalcScaleFactor(float screenDistance)
	{
		float range = this._config.MaxScaleScreenDistance - this._config.MinScaleScreenDistance;
		bool flag = Mathf.Approximately(range, 0f);
		float result;
		if (flag)
		{
			result = 0f;
		}
		else
		{
			result = Mathf.Clamp01((this._config.MaxScaleScreenDistance - screenDistance) / range);
		}
		return result;
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x00077260 File Offset: 0x00075460
	public float GetScale()
	{
		return this._scale;
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x00077278 File Offset: 0x00075478
	public float GetBackgroundOffset()
	{
		return -this._position;
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x00077294 File Offset: 0x00075494
	public float GetBackgroundScale(float screenDistance)
	{
		return this.CalcScaleFactor(screenDistance);
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x000772B0 File Offset: 0x000754B0
	public float GetSpineLayerScale()
	{
		return this._scale;
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x000772C8 File Offset: 0x000754C8
	public float CalculateSpineLayerPositionX(float logicScreenPos)
	{
		return logicScreenPos - this._position;
	}

	// Token: 0x04001035 RID: 4149
	private float _position;

	// Token: 0x04001036 RID: 4150
	private float _scale;

	// Token: 0x04001037 RID: 4151
	private bool _lockedPosition;

	// Token: 0x04001038 RID: 4152
	private bool _lockedScale;

	// Token: 0x04001039 RID: 4153
	private bool _lockedTargetScale;

	// Token: 0x0400103A RID: 4154
	private float _manuallySetTargetScale = 1f;

	// Token: 0x0400103B RID: 4155
	public const float SpineLayerPositionX = 0f;

	// Token: 0x0400103C RID: 4156
	private readonly CombatVirtualCamera.Config _config;

	// Token: 0x0400103D RID: 4157
	private float _configScaleFactor = 1f;

	// Token: 0x0400103E RID: 4158
	private float _targetPosition;

	// Token: 0x0400103F RID: 4159
	private float _targetScale;

	// Token: 0x04001040 RID: 4160
	private const float ReferenceTweenDuration = 0.15f;

	// Token: 0x04001041 RID: 4161
	private readonly Dictionary<string, CombatVirtualCamera.TweenState> _velocityDict = new Dictionary<string, CombatVirtualCamera.TweenState>();

	// Token: 0x0200123C RID: 4668
	private class TweenState
	{
		// Token: 0x040099EA RID: 39402
		public float Velocity;
	}

	// Token: 0x0200123D RID: 4669
	public readonly struct Config
	{
		// Token: 0x0600C51B RID: 50459 RVA: 0x0057E074 File Offset: 0x0057C274
		public Config(float maxScaleScreenDistance, float minScaleScreenDistance, float maxSpineScale, float minSpineScale)
		{
			this.MaxScaleScreenDistance = maxScaleScreenDistance;
			this.MinScaleScreenDistance = minScaleScreenDistance;
			this.MaxSpineScale = maxSpineScale;
			this.MinSpineScale = minSpineScale;
		}

		// Token: 0x040099EB RID: 39403
		public readonly float MinScaleScreenDistance;

		// Token: 0x040099EC RID: 39404
		public readonly float MaxScaleScreenDistance;

		// Token: 0x040099ED RID: 39405
		public readonly float MinSpineScale;

		// Token: 0x040099EE RID: 39406
		public readonly float MaxSpineScale;
	}
}
