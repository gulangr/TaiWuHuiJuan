using System;
using FrameWork;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.SectInteract.Fulong
{
	// Token: 0x020009EA RID: 2538
	public class ViewChickenMap : UIBase
	{
		// Token: 0x06007CAE RID: 31918 RVA: 0x0039F608 File Offset: 0x0039D808
		public static void Open(IAsyncMethodRequestHandler caller = null)
		{
			UIManager.Instance.ShowUI(UIElement.ChickenMap, true);
			ViewChickenMap.LastUseChickenMapAreaId = SingletonObject.getInstance<WorldMapModel>().CurrentAreaId;
			BuildingDomainMethod.AsyncCall.ClickChickenMap(caller, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref ViewChickenMap.ChickenMapInteractable);
			});
		}

		// Token: 0x17000DAB RID: 3499
		// (get) Token: 0x06007CAF RID: 31919 RVA: 0x0039F65C File Offset: 0x0039D85C
		public static bool IsChickenMapInteractable
		{
			get
			{
				return ViewChickenMap.LastUseChickenMapAreaId != SingletonObject.getInstance<WorldMapModel>().CurrentAreaId || ViewChickenMap.ChickenMapInteractable;
			}
		}

		// Token: 0x06007CB0 RID: 31920 RVA: 0x0039F677 File Offset: 0x0039D877
		public override void OnInit(ArgumentBox ab)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04005EE6 RID: 24294
		public static short LastUseChickenMapAreaId = -1;

		// Token: 0x04005EE7 RID: 24295
		public static bool ChickenMapInteractable = false;

		// Token: 0x04005EE8 RID: 24296
		public static bool AllChickenInTaiwuVillage = false;
	}
}
