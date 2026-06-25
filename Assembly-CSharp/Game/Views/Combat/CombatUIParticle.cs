using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B1C RID: 2844
	public class CombatUIParticle : MonoBehaviour, ICombatComponent
	{
		// Token: 0x06008B95 RID: 35733 RVA: 0x00407BE8 File Offset: 0x00405DE8
		public void Setup()
		{
		}

		// Token: 0x06008B96 RID: 35734 RVA: 0x00407BEB File Offset: 0x00405DEB
		public void Close()
		{
		}

		// Token: 0x06008B97 RID: 35735 RVA: 0x00407BEE File Offset: 0x00405DEE
		private void Update()
		{
			this.UpdateTianSuiBaoLuParticleFollow();
		}

		// Token: 0x06008B98 RID: 35736 RVA: 0x00407BF8 File Offset: 0x00405DF8
		public void PlayTianSuiBaoLuParticle(bool isAlly, RectTransform selfSkeletonTrans, RectTransform useItemButtonTrans, bool isDirect)
		{
			Transform startPositionFollowTarget = CombatUIParticle.FindSkeletonTransform(selfSkeletonTrans, "waist_x");
			bool flag = startPositionFollowTarget == null;
			if (flag)
			{
				startPositionFollowTarget = selfSkeletonTrans;
			}
			CombatUIParticle.TianSuiBaoLuParticleInstance instance = new CombatUIParticle.TianSuiBaoLuParticleInstance
			{
				FollowTarget = startPositionFollowTarget
			};
			this._tianSuiBaoLuInstances.Add(instance);
			string startParticleKey = isDirect ? "eff_combat_ui_fulushoujilan" : "eff_combat_ui_fulushoujihong";
			string moveParticleKey = isDirect ? "eff_combat_ui_fuluyindaoxianlan" : "eff_combat_ui_fuluyindaoxianhong";
			string endParticleKey = isDirect ? "eff_combat_ui_fuluxishoulan" : "eff_combat_ui_fuluxishouhong";
			string startSoundName = isAlly ? (isDirect ? "se_c_025_baolu_1" : "se_c_025_baolu_2") : (isDirect ? "se_c_025_baolu_enemy_1" : "se_c_025_baolu_enemy_2");
			Vector2 startParticleOffset = new Vector2(0f, 200f);
			Vector3 tartReferenceWorldPos = selfSkeletonTrans.position;
			Vector3 startOffsetInWorldSpace = selfSkeletonTrans.TransformVector(startParticleOffset);
			Vector3 startWorldPos = tartReferenceWorldPos + startOffsetInWorldSpace;
			Vector3 startLocalPos = this.particleRoot.InverseTransformPoint(startWorldPos);
			if (isAlly)
			{
				Vector2 endParticleOffset = new Vector2(23f, 0f);
				Vector3 endReferenceWorldPos = useItemButtonTrans.position;
				Vector3 endOffsetInWorldSpace = useItemButtonTrans.TransformVector(endParticleOffset);
				Vector3 endWorldPos = endReferenceWorldPos + endOffsetInWorldSpace;
				Vector3 endLocalPos = this.particleRoot.InverseTransformPoint(endWorldPos);
				base.StartCoroutine(this.PlayTianSuiBaoLuParticleSequence(instance, startParticleKey, moveParticleKey, endParticleKey, startLocalPos, endLocalPos, startSoundName));
			}
			else
			{
				base.StartCoroutine(this.PlayTianSuiBaoLuEnemyParticleSequence(instance, startParticleKey, endParticleKey, startLocalPos, startSoundName));
			}
		}

		// Token: 0x06008B99 RID: 35737 RVA: 0x00407D58 File Offset: 0x00405F58
		private static Transform FindSkeletonTransform(Transform parent, string boneName)
		{
			bool flag = parent == null;
			Transform result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				Transform child = parent.Find(boneName);
				bool flag2 = child != null;
				if (flag2)
				{
					result2 = child;
				}
				else
				{
					foreach (object obj in parent)
					{
						Transform childTransform = (Transform)obj;
						Transform result = CombatUIParticle.FindSkeletonTransform(childTransform, boneName);
						bool flag3 = result != null;
						if (flag3)
						{
							return result;
						}
					}
					result2 = null;
				}
			}
			return result2;
		}

		// Token: 0x06008B9A RID: 35738 RVA: 0x00407DFC File Offset: 0x00405FFC
		private IEnumerator PlayTianSuiBaoLuParticleSequence(CombatUIParticle.TianSuiBaoLuParticleInstance instance, string startParticleKey, string moveParticleKey, string endParticleKey, Vector3 startLocalPos, Vector3 endLocalPos, string startSoundName)
		{
			yield return new WaitForSeconds(0.1f);
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("soundName", startSoundName);
			GEvent.OnEvent(UiEvents.PlayCombatSoundOnce, argumentBox);
			yield return new WaitForSeconds(0.2f);
			GameObject startParticle = this.CreateParticle(startParticleKey);
			RectTransform startRect = startParticle.GetComponent<RectTransform>();
			startRect.localPosition = startLocalPos;
			this.PlayOnce(startParticle, 1f);
			instance.StartParticleObject = startParticle;
			yield return new WaitForSeconds(0.9f);
			GameObject moveParticle = this.CreateParticle(moveParticleKey);
			moveParticle.SetActive(true);
			RectTransform moveRect = moveParticle.GetComponent<RectTransform>();
			moveRect.localPosition = startLocalPos;
			bool flag = instance.StartParticleObject == null;
			if (flag)
			{
				Object.Destroy(moveParticle);
				this._tianSuiBaoLuInstances.Remove(instance);
				yield break;
			}
			Vector3 flyStartPos = instance.StartParticleObject.transform.position;
			Vector3 flyStartLocalPosition = this.particleRoot.InverseTransformPoint(flyStartPos);
			base.StartCoroutine(CombatUIParticle.MoveParticle(moveRect, flyStartLocalPosition, endLocalPos, 0.4f, moveParticle));
			yield return new WaitForSeconds(0.4f);
			GameObject endParticle = this.CreateParticle(endParticleKey);
			RectTransform endRect = endParticle.GetComponent<RectTransform>();
			endRect.localPosition = endLocalPos;
			this.PlayOnce(endParticle, 1f);
			yield return new WaitForSeconds(0.3f);
			Object.Destroy(moveParticle);
			this._tianSuiBaoLuInstances.Remove(instance);
			yield break;
		}

		// Token: 0x06008B9B RID: 35739 RVA: 0x00407E4C File Offset: 0x0040604C
		private void UpdateTianSuiBaoLuParticleFollow()
		{
			for (int i = this._tianSuiBaoLuInstances.Count - 1; i >= 0; i--)
			{
				CombatUIParticle.TianSuiBaoLuParticleInstance instance = this._tianSuiBaoLuInstances[i];
				bool flag = instance.FollowTarget == null || instance.StartParticleObject == null;
				if (flag)
				{
					this._tianSuiBaoLuInstances.RemoveAt(i);
				}
				else
				{
					instance.StartParticleObject.transform.position = new Vector3(instance.FollowTarget.position.x, instance.StartParticleObject.transform.position.y, instance.StartParticleObject.transform.position.z);
				}
			}
		}

		// Token: 0x06008B9C RID: 35740 RVA: 0x00407F0F File Offset: 0x0040610F
		private static IEnumerator MoveParticle(RectTransform particleRect, Vector3 startPos, Vector3 endPos, float duration, GameObject particle)
		{
			float elapsedTime = 0f;
			ParticleSystem particleSystemComponent = particle.transform.GetChild(0).GetComponent<ParticleSystem>();
			bool flag = particleSystemComponent;
			if (flag)
			{
				particleSystemComponent.Play();
			}
			bool useSimpleArc = Random.Range(0, 2) == 0;
			float distance = Vector3.Distance(startPos, endPos);
			float arcDepthFactor = Random.Range(0.2f, 0.4f);
			float arcDepth = distance * arcDepthFactor;
			Vector3 arcDirection = (Random.Range(0, 2) == 0) ? Vector3.up : Vector3.down;
			bool flag2 = useSimpleArc;
			if (flag2)
			{
				Vector3 midPoint = (startPos + endPos) * 0.5f;
				Vector3 controlPoint = midPoint + arcDirection * arcDepth;
				while (elapsedTime < duration)
				{
					elapsedTime += Time.deltaTime;
					float t = elapsedTime / duration;
					t = Mathf.SmoothStep(0f, 1f, t);
					Vector3 position = CombatUIParticle.CalculateQuadraticBezierPoint(startPos, controlPoint, endPos, t);
					particleRect.localPosition = position;
					yield return null;
					position = default(Vector3);
				}
				midPoint = default(Vector3);
				controlPoint = default(Vector3);
			}
			else
			{
				Vector3 oneThirdPoint = Vector3.Lerp(startPos, endPos, 0.33f);
				Vector3 twoThirdPoint = Vector3.Lerp(startPos, endPos, 0.67f);
				Vector3 controlPoint2 = oneThirdPoint + arcDirection * arcDepth;
				Vector3 controlPoint3 = twoThirdPoint - arcDirection * arcDepth;
				while (elapsedTime < duration)
				{
					elapsedTime += Time.deltaTime;
					float t2 = elapsedTime / duration;
					t2 = Mathf.SmoothStep(0f, 1f, t2);
					Vector3 position2 = CombatUIParticle.CalculateCubicBezierPoint(startPos, controlPoint2, controlPoint3, endPos, t2);
					particleRect.localPosition = position2;
					yield return null;
					position2 = default(Vector3);
				}
				oneThirdPoint = default(Vector3);
				twoThirdPoint = default(Vector3);
				controlPoint2 = default(Vector3);
				controlPoint3 = default(Vector3);
			}
			particleRect.localPosition = endPos;
			yield break;
		}

		// Token: 0x06008B9D RID: 35741 RVA: 0x00407F3B File Offset: 0x0040613B
		private IEnumerator PlayTianSuiBaoLuEnemyParticleSequence(CombatUIParticle.TianSuiBaoLuParticleInstance instance, string startParticleKey, string endParticleKey, Vector3 startLocalPos, string startSoundName)
		{
			yield return new WaitForSeconds(0.1f);
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("soundName", startSoundName);
			GEvent.OnEvent(UiEvents.PlayCombatSoundOnce, argumentBox);
			yield return new WaitForSeconds(0.2f);
			GameObject startParticle = this.CreateParticle(startParticleKey);
			RectTransform startRect = startParticle.GetComponent<RectTransform>();
			startRect.localPosition = startLocalPos;
			this.PlayOnce(startParticle, 1f);
			instance.StartParticleObject = startParticle;
			yield return new WaitForSeconds(1f);
			GameObject endParticle = this.CreateParticle(endParticleKey);
			RectTransform endRect = endParticle.GetComponent<RectTransform>();
			bool flag = instance.StartParticleObject == null;
			if (flag)
			{
				this._tianSuiBaoLuInstances.Remove(instance);
				yield break;
			}
			Vector3 currentWorldPos = instance.StartParticleObject.transform.position;
			Vector3 currentLocalPos = this.particleRoot.InverseTransformPoint(currentWorldPos);
			endRect.localPosition = currentLocalPos;
			this.PlayOnce(endParticle, 1f);
			this._tianSuiBaoLuInstances.Remove(instance);
			yield break;
		}

		// Token: 0x06008B9E RID: 35742 RVA: 0x00407F6F File Offset: 0x0040616F
		public void PlayAbsorbNeiliAllocation(RectTransform from, RectTransform to, byte neiliAllocationType)
		{
			base.StartCoroutine(this.PlayAbsorbNeiliAllocationCoroutine(from, to, neiliAllocationType));
		}

		// Token: 0x06008B9F RID: 35743 RVA: 0x00407F82 File Offset: 0x00406182
		private IEnumerator PlayAbsorbNeiliAllocationCoroutine(RectTransform from, RectTransform to, byte neiliAllocationType)
		{
			yield return new WaitForSeconds(0.3f);
			GameObject moveParticle = this.CreateParticle(string.Format("eff_combat_ui_absorb_neiliallocation_{0}", neiliAllocationType));
			RectTransform moveRect = moveParticle.GetComponent<RectTransform>();
			moveRect.localPosition = this.GetToLocalPos(from);
			moveParticle.SetActive(true);
			base.StartCoroutine(this.MoveParticle(moveRect, from, to, 0.3f, moveParticle));
			yield return new WaitForSeconds(0.35f);
			GameObject absorbParticle = this.CreateParticle(string.Format("eff_combat_ui_absorb_neiliallocation_{0}sj", neiliAllocationType));
			RectTransform absorbRect = absorbParticle.GetComponent<RectTransform>();
			absorbRect.localPosition = this.GetToLocalPos(to);
			absorbParticle.SetActive(true);
			yield return new WaitForSeconds(1f);
			Object.Destroy(absorbParticle);
			Object.Destroy(moveParticle);
			yield break;
		}

		// Token: 0x06008BA0 RID: 35744 RVA: 0x00407FA8 File Offset: 0x004061A8
		private Vector3 GetToLocalPos(RectTransform to)
		{
			Transform toBody = CombatUIParticle.FindSkeletonTransform(to, "BodyPart1");
			return this.particleRoot.InverseTransformPoint(to.position + to.TransformVector(toBody.localPosition));
		}

		// Token: 0x06008BA1 RID: 35745 RVA: 0x00407FEA File Offset: 0x004061EA
		private IEnumerator MoveParticle(RectTransform particleRect, RectTransform fromRect, RectTransform toRect, float duration, GameObject particle)
		{
			Vector3 endPos = this.GetToLocalPos(toRect);
			Vector3 startPos = this.GetToLocalPos(fromRect);
			float elapsedTime = 0f;
			ParticleSystem particleSystemComponent = particle.transform.GetChild(0).GetComponent<ParticleSystem>();
			bool flag = particleSystemComponent;
			if (flag)
			{
				particleSystemComponent.Play();
			}
			bool useSimpleArc = Random.Range(0, 2) == 0;
			float distance = Vector3.Distance(startPos, endPos);
			float arcDepthFactor = Random.Range(0.2f, 0.4f);
			float arcDepth = distance * arcDepthFactor;
			Vector3 arcDirection = (Random.Range(0, 2) == 0) ? Vector3.up : Vector3.down;
			bool flag2 = useSimpleArc;
			if (flag2)
			{
				Vector3 midPoint = (startPos + endPos) * 0.5f;
				Vector3 controlPoint = midPoint + arcDirection * arcDepth;
				while (elapsedTime < duration)
				{
					elapsedTime += Time.deltaTime;
					float t = elapsedTime / duration;
					t = Mathf.SmoothStep(0f, 1f, t);
					Vector3 position = CombatUIParticle.CalculateQuadraticBezierPoint(this.GetToLocalPos(fromRect), controlPoint, this.GetToLocalPos(toRect), t);
					particleRect.localPosition = position;
					yield return null;
					position = default(Vector3);
				}
				midPoint = default(Vector3);
				controlPoint = default(Vector3);
			}
			else
			{
				Vector3 oneThirdPoint = Vector3.Lerp(startPos, endPos, 0.33f);
				Vector3 twoThirdPoint = Vector3.Lerp(startPos, endPos, 0.67f);
				Vector3 controlPoint2 = oneThirdPoint + arcDirection * arcDepth;
				Vector3 controlPoint3 = twoThirdPoint - arcDirection * arcDepth;
				while (elapsedTime < duration)
				{
					elapsedTime += Time.deltaTime;
					float t2 = elapsedTime / duration;
					t2 = Mathf.SmoothStep(0f, 1f, t2);
					Vector3 position2 = CombatUIParticle.CalculateCubicBezierPoint(this.GetToLocalPos(fromRect), controlPoint2, controlPoint3, this.GetToLocalPos(toRect), t2);
					particleRect.localPosition = position2;
					yield return null;
					position2 = default(Vector3);
				}
				oneThirdPoint = default(Vector3);
				twoThirdPoint = default(Vector3);
				controlPoint2 = default(Vector3);
				controlPoint3 = default(Vector3);
			}
			particleRect.localPosition = this.GetToLocalPos(toRect);
			yield break;
		}

		// Token: 0x06008BA2 RID: 35746 RVA: 0x00408020 File Offset: 0x00406220
		private void PlayOnce(GameObject particleObject, float lifetime)
		{
			particleObject.SetActive(true);
			ParticleSystem p = particleObject.transform.GetChild(0).GetComponent<ParticleSystem>();
			bool flag = p != null;
			if (flag)
			{
				p.Play();
			}
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(lifetime, delegate
			{
				Object.Destroy(particleObject);
			});
		}

		// Token: 0x06008BA3 RID: 35747 RVA: 0x0040808C File Offset: 0x0040628C
		private GameObject CreateParticle(string key)
		{
			GameObject template = this.GetTemplate(key);
			bool flag = template == null;
			GameObject result;
			if (flag)
			{
				Debug.LogError("CombatUIParticle template '" + key + "' not found");
				result = null;
			}
			else
			{
				GameObject particle = Object.Instantiate<GameObject>(template, this.particleRoot);
				result = particle;
			}
			return result;
		}

		// Token: 0x06008BA4 RID: 35748 RVA: 0x004080DC File Offset: 0x004062DC
		private GameObject GetTemplate(string key)
		{
			bool flag = this.templateRoot == null;
			GameObject result;
			if (flag)
			{
				Debug.LogError("CombatUIParticle templateRoot is null");
				result = null;
			}
			else
			{
				Transform child = this.templateRoot.Find(key);
				bool flag2 = child == null;
				if (flag2)
				{
					Debug.LogError("CombatUIParticle template '" + key + "' not found");
					result = null;
				}
				else
				{
					result = child.gameObject;
				}
			}
			return result;
		}

		// Token: 0x06008BA5 RID: 35749 RVA: 0x00408148 File Offset: 0x00406348
		private static Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
		{
			float u = 1f - t;
			float tt = t * t;
			float uu = u * u;
			Vector3 point = uu * p0;
			point += 2f * u * t * p1;
			return point + tt * p2;
		}

		// Token: 0x06008BA6 RID: 35750 RVA: 0x0040819C File Offset: 0x0040639C
		private static Vector3 CalculateCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float u = 1f - t;
			float tt = t * t;
			float uu = u * u;
			float uuu = uu * u;
			float ttt = tt * t;
			Vector3 point = uuu * p0;
			point += 3f * uu * t * p1;
			point += 3f * u * tt * p2;
			return point + ttt * p3;
		}

		// Token: 0x04006AE9 RID: 27369
		private readonly List<CombatUIParticle.TianSuiBaoLuParticleInstance> _tianSuiBaoLuInstances = new List<CombatUIParticle.TianSuiBaoLuParticleInstance>();

		// Token: 0x04006AEA RID: 27370
		[SerializeField]
		private RectTransform templateRoot;

		// Token: 0x04006AEB RID: 27371
		[SerializeField]
		private RectTransform particleRoot;

		// Token: 0x020020D9 RID: 8409
		private class TianSuiBaoLuParticleInstance
		{
			// Token: 0x0400D271 RID: 53873
			public Transform FollowTarget;

			// Token: 0x0400D272 RID: 53874
			public GameObject StartParticleObject;
		}
	}
}
