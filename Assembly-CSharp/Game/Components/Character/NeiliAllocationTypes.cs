using System;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F3B RID: 3899
	public class NeiliAllocationTypes : MonoBehaviour
	{
		// Token: 0x0600B340 RID: 45888 RVA: 0x005197F0 File Offset: 0x005179F0
		public static string GetColorByType(byte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = "attack";
				break;
			case 1:
				result = "agile";
				break;
			case 2:
				result = "defense";
				break;
			case 3:
				result = "assist";
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B341 RID: 45889 RVA: 0x00519850 File Offset: 0x00517A50
		public void Init()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				byte type = (byte)i;
				ComponentNeiliAllocationType obj = base.transform.GetChild(i).GetComponent<ComponentNeiliAllocationType>();
				obj.Init(type);
				obj.GetComponent<CImage>().SetSprite(string.Format("{0}{1}", "ui9_back_neili_allocation_type_", type), false, null);
			}
		}

		// Token: 0x0600B342 RID: 45890 RVA: 0x005198BC File Offset: 0x00517ABC
		public void Init(Action<byte> onClickAdd, Action<byte> onClickMinus, Action<byte> onPointerEnter, Action<byte> onPointerExit, Action<byte, bool> onBtnPointerEnter = null)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				byte type = (byte)i;
				ComponentNeiliAllocationType obj = base.transform.GetChild(i).GetComponent<ComponentNeiliAllocationType>();
				PointerTrigger pointerTrigger = obj.GetComponent<PointerTrigger>();
				obj.Init(type);
				obj.GetComponent<CImage>().SetSprite(string.Format("{0}{1}", "ui9_back_neili_allocation_type_", type), false, null);
				bool flag = onClickAdd != null;
				if (flag)
				{
					obj.btnAdd.ClearAndAddListener(delegate
					{
						onClickAdd(type);
					});
					PointerTrigger btnPointerTrigger = obj.btnAdd.GetComponent<PointerTrigger>();
					btnPointerTrigger.EnterEvent.RemoveAllListeners();
					btnPointerTrigger.ExitEvent.RemoveAllListeners();
					bool flag2 = onBtnPointerEnter != null;
					if (flag2)
					{
						btnPointerTrigger.EnterEvent.AddListener(delegate()
						{
							onBtnPointerEnter(type, true);
						});
					}
					bool flag3 = onPointerExit != null;
					if (flag3)
					{
						btnPointerTrigger.ExitEvent.AddListener(delegate()
						{
							onPointerExit(type);
						});
					}
				}
				bool flag4 = onClickMinus != null;
				if (flag4)
				{
					obj.btnMinus.ClearAndAddListener(delegate
					{
						onClickMinus(type);
					});
					PointerTrigger btnPointerTrigger2 = obj.btnMinus.GetComponent<PointerTrigger>();
					btnPointerTrigger2.EnterEvent.RemoveAllListeners();
					btnPointerTrigger2.ExitEvent.RemoveAllListeners();
					bool flag5 = onBtnPointerEnter != null;
					if (flag5)
					{
						btnPointerTrigger2.EnterEvent.AddListener(delegate()
						{
							onBtnPointerEnter(type, false);
						});
					}
					bool flag6 = onPointerExit != null;
					if (flag6)
					{
						btnPointerTrigger2.ExitEvent.AddListener(delegate()
						{
							onPointerExit(type);
						});
					}
				}
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.RemoveAllListeners();
				bool flag7 = onPointerEnter != null;
				if (flag7)
				{
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						onPointerEnter(type);
					});
				}
				bool flag8 = onPointerExit != null;
				if (flag8)
				{
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						onPointerExit(type);
					});
				}
			}
		}

		// Token: 0x0600B343 RID: 45891 RVA: 0x00519B55 File Offset: 0x00517D55
		public void Set(byte type, int value, int extraValue, bool isAdd = true)
		{
			base.transform.GetChild((int)type).GetComponent<ComponentNeiliAllocationType>().Set(value, extraValue, isAdd);
		}

		// Token: 0x0600B344 RID: 45892 RVA: 0x00519B74 File Offset: 0x00517D74
		public void SetCanInteract(byte type, bool canAddInteract, bool canMinusInteract)
		{
			ComponentNeiliAllocationType item = base.transform.GetChild((int)type).GetComponent<ComponentNeiliAllocationType>();
			item.btnAdd.interactable = canAddInteract;
			item.btnMinus.interactable = canMinusInteract;
			item.btnAdd.GetComponent<PointerTrigger>().enabled = canAddInteract;
			item.btnMinus.GetComponent<PointerTrigger>().enabled = canMinusInteract;
		}
	}
}
