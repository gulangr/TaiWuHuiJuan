using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class CircleGenerator : MonoBehaviour
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x0600027F RID: 639 RVA: 0x0000F174 File Offset: 0x0000D374
	private PoolItem ItemPool
	{
		get
		{
			bool flag = this._itemPoolInstance == null && this.Prefab != null;
			if (flag)
			{
				this._itemPoolInstance = new PoolItem("ItemView", this.Prefab);
			}
			return this._itemPoolInstance;
		}
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000F1C0 File Offset: 0x0000D3C0
	public void GenerateObjects()
	{
		bool flag = this.NumberOfObjects == 0;
		if (!flag)
		{
			float angleRange = this.EndAngle - this.StartAngle;
			float angleInterval = angleRange / (float)this.NumberOfObjects;
			for (int i = 0; i < this.NumberOfObjects; i++)
			{
				float angle = this.StartAngle + (float)i * angleInterval;
				float x = Mathf.Sin(0.017453292f * angle) * this.Radius;
				float y = Mathf.Cos(0.017453292f * angle) * this.Radius;
				Vector3 position = new Vector3(x, y, 0f) + base.transform.position;
				Quaternion rotation = Quaternion.identity;
				GameObject newObj = Object.Instantiate<GameObject>(this.Prefab, position, rotation);
				this._gameObjects.Add(newObj);
				newObj.transform.SetParent(base.transform);
				newObj.transform.localScale = this.Size;
			}
		}
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0000F2BC File Offset: 0x0000D4BC
	public void GenerateLoongDebuffObjects(List<short> loongTemplatedIds, bool hasOrnamentation)
	{
		this.ClearExistGO();
		bool flag = this.NumberOfObjects == 0;
		if (!flag)
		{
			float angleRange = this.EndAngle - this.StartAngle;
			float angleInterval = angleRange / (float)this.NumberOfObjects;
			Vector3 parentPosition = base.transform.position;
			for (int i = 0; i < this.NumberOfObjects; i++)
			{
				float angle = this.StartAngle + (float)i * angleInterval;
				float x = Mathf.Sin(0.017453292f * angle) * this.Radius;
				float y = Mathf.Cos(0.017453292f * angle) * this.Radius;
				Vector3 position = new Vector3(x, y, 0f) + parentPosition;
				Quaternion rotation = Quaternion.identity;
				bool flag2 = this._itemPoolInstance == null && this.Prefab != null;
				if (flag2)
				{
					this._itemPoolInstance = new PoolItem("ItemView", this.Prefab);
				}
				GameObject newObj = this.ItemPool.GetObject();
				newObj.transform.position = position;
				newObj.transform.rotation = rotation;
				this._gameObjects.Add(newObj);
				newObj.transform.SetParent(base.transform);
				newObj.transform.localScale = this.Size;
				bool flag3 = i == 0;
				if (flag3)
				{
					newObj.GetComponent<RectTransform>().localPosition += new Vector3(20f, 0f, 0f);
				}
				if (hasOrnamentation)
				{
					bool flag4 = i == 0;
					if (flag4)
					{
						newObj.GetComponent<CImage>().SetSprite("fiveloong_base_0", false, null);
						newObj.GetComponent<RectTransform>().SetHeight(50f);
						newObj.GetComponent<RectTransform>().SetWidth(86f);
						newObj.GetComponent<TooltipInvoker>().enabled = false;
					}
					else
					{
						this.SetLoongDebuffDisplayData(newObj, loongTemplatedIds[i - 1]);
					}
				}
				else
				{
					this.SetLoongDebuffDisplayData(newObj, loongTemplatedIds[i]);
				}
			}
		}
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0000F4D0 File Offset: 0x0000D6D0
	private void SetLoongDebuffDisplayData(GameObject loongDebuff, short loongTemplatedId)
	{
		Refers refers = loongDebuff.GetComponent<Refers>();
		GameObject effect = refers.CGet<GameObject>("Effect");
		loongDebuff.GetComponent<CImage>().SetSprite(CommonUtils.FiveLongImageMapping[loongTemplatedId], false, null);
		GameObject effectPrefab = refers.CGet<GameObject>(CommonUtils.FiveLongEffectMapping[loongTemplatedId]);
		GameObject effectInstance = Object.Instantiate<GameObject>(effectPrefab, effect.transform.position, Quaternion.identity);
		effectInstance.transform.parent = effect.transform;
		effect.GetComponent<UIParticle>().RefreshParticles();
	}

	// Token: 0x06000283 RID: 643 RVA: 0x0000F554 File Offset: 0x0000D754
	public void ClearExistGO()
	{
		bool flag = this._gameObjects.Count > 0;
		if (flag)
		{
			for (int i = 0; i < this._gameObjects.Count; i++)
			{
				this.ItemPool.RemoveObject(this._gameObjects[i]);
			}
			this._gameObjects.Clear();
		}
	}

	// Token: 0x0400013E RID: 318
	[Header("Object Settings")]
	public GameObject Prefab;

	// Token: 0x0400013F RID: 319
	public int NumberOfObjects;

	// Token: 0x04000140 RID: 320
	[Header("Circle Settings")]
	[Range(0f, 20f)]
	public float Radius;

	// Token: 0x04000141 RID: 321
	[Header("Angle Settings")]
	[Range(0f, 360f)]
	public float StartAngle = 0f;

	// Token: 0x04000142 RID: 322
	[Range(0f, 360f)]
	public float EndAngle = 360f;

	// Token: 0x04000143 RID: 323
	[Header("Size Settings")]
	public Vector3 Size = Vector3.one;

	// Token: 0x04000144 RID: 324
	private List<GameObject> _gameObjects = new List<GameObject>();

	// Token: 0x04000145 RID: 325
	private PoolItem _itemPoolInstance = null;

	// Token: 0x04000146 RID: 326
	private const string ItemViewKey = "ItemView";
}
