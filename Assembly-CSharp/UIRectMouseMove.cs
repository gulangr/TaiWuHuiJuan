using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class UIRectMouseMove : MonoBehaviour
{
	// Token: 0x060005EF RID: 1519 RVA: 0x00027A9C File Offset: 0x00025C9C
	public void SetEnabled(bool value)
	{
		this._enabled = value;
		if (value)
		{
			Cursor.lockState = CursorLockMode.Confined;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00027ACA File Offset: 0x00025CCA
	private void Awake()
	{
		this._awakeMovePos = this._moveRectTransform.anchoredPosition;
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00027AE0 File Offset: 0x00025CE0
	private void LateUpdate()
	{
		this.Clamp(Vector2.zero);
		bool flag = !this._enabled;
		if (!flag)
		{
			Vector2 direction;
			bool isNearlyScreenBorder = this.IsNearlyScreenBorder(out direction);
			bool flag2 = this.IsMouseInScreenRectTransform() && !isNearlyScreenBorder;
			if (!flag2)
			{
				Vector2 localDirection;
				Vector2 intensity = this.GetMouseIntensity(direction, out localDirection);
				bool flag3 = intensity != Vector2.zero && localDirection != Vector2.zero;
				if (flag3)
				{
					ValueTuple<Vector2, Vector2> minMax = this.GetRectMinMax();
					bool flag4 = localDirection.x < 0f;
					float moveX;
					if (flag4)
					{
						moveX = Mathf.Abs(this._moveRectTransform.anchoredPosition.x - minMax.Item2.x);
					}
					else
					{
						moveX = -Mathf.Abs(this._moveRectTransform.anchoredPosition.x - minMax.Item1.x);
					}
					bool flag5 = localDirection.y < 0f;
					float moveY;
					if (flag5)
					{
						moveY = Mathf.Abs(this._moveRectTransform.anchoredPosition.y - minMax.Item2.y);
					}
					else
					{
						moveY = -Mathf.Abs(this._moveRectTransform.anchoredPosition.y - minMax.Item1.y);
					}
					Vector2 deltaOffset = this.SpeedFactor * Time.deltaTime * new Vector2(moveX, moveY) * intensity;
					this.Clamp(deltaOffset);
				}
			}
		}
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x00027C68 File Offset: 0x00025E68
	private void Clamp(Vector2 offest)
	{
		ValueTuple<Vector2, Vector2> minMax = this.GetRectMinMax();
		float edgeX = this._screenRectTransform.rect.width * 0.5f;
		float edgeY = this._screenRectTransform.rect.height * 0.5f;
		float offsetX = Mathf.Clamp(this._moveRectTransform.anchoredPosition.x + offest.x, minMax.Item1.x + edgeX, minMax.Item2.x - edgeX);
		float offsetY = Mathf.Clamp(this._moveRectTransform.anchoredPosition.y + offest.y, minMax.Item1.y + edgeY, minMax.Item2.y - edgeY);
		this._moveRectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x00027D38 File Offset: 0x00025F38
	private bool IsMouseInScreenRectTransform()
	{
		return RectTransformUtility.RectangleContainsScreenPoint(this._screenRectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00027D6C File Offset: 0x00025F6C
	private bool IsMouseInMoveRectTransform()
	{
		return RectTransformUtility.RectangleContainsScreenPoint(this._moveRectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00027DA0 File Offset: 0x00025FA0
	private bool IsNearlyBorder()
	{
		ValueTuple<Vector2, Vector2> minMax = this.GetRectMinMax();
		float edgeX = this._screenRectTransform.rect.width * 0.5f;
		float edgeY = this._screenRectTransform.rect.height * 0.5f;
		float distance = 20f;
		bool flag = Mathf.Abs(this._moveRectTransform.anchoredPosition.x - (minMax.Item2.x - edgeX)) < distance && Input.GetAxis("Mouse X") < 0f;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = Mathf.Abs(this._moveRectTransform.anchoredPosition.x - (minMax.Item1.x + edgeX)) < distance && Input.GetAxis("Mouse X") > 0f;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = Mathf.Abs(this._moveRectTransform.anchoredPosition.y - (minMax.Item2.y - edgeY)) < distance && Input.GetAxis("Mouse Y") < 0f;
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool flag4 = Mathf.Abs(this._moveRectTransform.anchoredPosition.y - (minMax.Item1.y + edgeY)) < distance && Input.GetAxis("Mouse Y") > 0f;
					result = flag4;
				}
			}
		}
		return result;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00027F18 File Offset: 0x00026118
	private bool IsNearlyScreenBorder(out Vector2 direction)
	{
		float edgeX = this._screenRectTransform.rect.width * 0.5f;
		float edgeY = this._screenRectTransform.rect.height * 0.5f;
		direction = Vector2.zero;
		float distance = 10f;
		bool flag = false;
		Vector2 localPos;
		bool flag2 = RectTransformUtility.ScreenPointToLocalPointInRectangle(this._screenRectTransform, Input.mousePosition, UIManager.Instance.UiCamera, out localPos);
		if (flag2)
		{
			bool flag3 = edgeX - Mathf.Abs(this._screenRectTransform.anchoredPosition.x - localPos.x) < distance && localPos.x < this._screenRectTransform.anchoredPosition.x;
			if (flag3)
			{
				direction += Vector2.left;
				flag = true;
			}
			bool flag4 = edgeX - Mathf.Abs(this._screenRectTransform.anchoredPosition.x - localPos.x) < distance && localPos.x > this._screenRectTransform.anchoredPosition.x;
			if (flag4)
			{
				direction += Vector2.right;
				flag = true;
			}
			bool flag5 = edgeY - Mathf.Abs(this._screenRectTransform.anchoredPosition.y - localPos.y) < distance && localPos.y < this._screenRectTransform.anchoredPosition.y;
			if (flag5)
			{
				direction += Vector2.down;
				flag = true;
			}
			bool flag6 = edgeY - Mathf.Abs(this._screenRectTransform.anchoredPosition.y - localPos.y) < distance && localPos.y > this._screenRectTransform.anchoredPosition.y;
			if (flag6)
			{
				direction += Vector2.up;
				flag = true;
			}
		}
		direction = direction.normalized;
		return flag;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00028128 File Offset: 0x00026328
	private Vector2 GetMouseIntensity(Vector2 direction, out Vector2 localDirection)
	{
		Vector2 intensity = Vector2.zero;
		localDirection = Vector2.zero;
		Vector2 localMousePos;
		bool flag = RectTransformUtility.ScreenPointToLocalPointInRectangle(this._screenRectTransform, Input.mousePosition, UIManager.Instance.UiCamera, out localMousePos);
		if (flag)
		{
			Vector2 originPos = this._screenRectTransform.anchoredPosition;
			Vector2 op = localMousePos - originPos;
			bool flag2 = direction.x != 0f;
			if (flag2)
			{
				bool flag3 = direction.x > 0f;
				float angleX;
				if (flag3)
				{
					angleX = Vector2.Angle(Vector2.right, op);
				}
				else
				{
					angleX = Vector2.Angle(Vector2.left, op);
				}
				float radians = angleX * 0.017453292f;
				intensity.x = Mathf.Cos(radians);
				intensity.y = Mathf.Sin(radians);
			}
			bool flag4 = direction.y != 0f;
			if (flag4)
			{
				bool flag5 = direction.y > 0f;
				float angleY;
				if (flag5)
				{
					angleY = Vector2.Angle(Vector2.up, op);
				}
				else
				{
					angleY = Vector2.Angle(Vector2.down, op);
				}
				float radians2 = angleY * 0.017453292f;
				intensity.y = Mathf.Cos(radians2);
				intensity.x = Mathf.Sin(radians2);
			}
			bool flag6 = op.y < 0f;
			if (flag6)
			{
				localDirection += Vector2.down;
			}
			else
			{
				localDirection += Vector2.up;
			}
			bool flag7 = op.x < 0f;
			if (flag7)
			{
				localDirection += Vector2.left;
			}
			else
			{
				localDirection += Vector2.right;
			}
		}
		return intensity;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00028300 File Offset: 0x00026500
	private ValueTuple<Vector2, Vector2> GetRectMinMax()
	{
		Vector3 localScale = this._moveRectTransform.localScale;
		Rect selfRect = this._moveRectTransform.rect;
		Vector2 min = selfRect.min;
		Vector2 max = selfRect.max;
		min.x += (float)this._adjustPadding.left;
		min.y += (float)this._adjustPadding.bottom;
		max.x -= (float)this._adjustPadding.right;
		max.y -= (float)this._adjustPadding.top;
		min.x *= localScale.x;
		min.y *= localScale.y;
		max.x *= localScale.x;
		max.y *= localScale.y;
		min += this._awakeMovePos;
		max += this._awakeMovePos;
		return new ValueTuple<Vector2, Vector2>(min, max);
	}

	// Token: 0x040004E1 RID: 1249
	[Header("移动的UI对象")]
	[SerializeField]
	private RectTransform _moveRectTransform;

	// Token: 0x040004E2 RID: 1250
	[Header("固定屏幕范围的UI对象")]
	[SerializeField]
	private RectTransform _screenRectTransform;

	// Token: 0x040004E3 RID: 1251
	[Header("边界修正")]
	[SerializeField]
	private RectOffset _adjustPadding;

	// Token: 0x040004E4 RID: 1252
	[Header("缓动速度参数")]
	public float SpeedFactor = 5f;

	// Token: 0x040004E5 RID: 1253
	private bool _enabled;

	// Token: 0x040004E6 RID: 1254
	private Vector2 _awakeMovePos;
}
