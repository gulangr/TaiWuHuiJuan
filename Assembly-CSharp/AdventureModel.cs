using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x020000FF RID: 255
public class AdventureModel : ISingletonInit, IDisposable
{
	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x0600088F RID: 2191 RVA: 0x0003ABC6 File Offset: 0x00038DC6
	// (set) Token: 0x06000890 RID: 2192 RVA: 0x0003ABCE File Offset: 0x00038DCE
	public AdventureModel.EPlayState PlayState { get; private set; } = AdventureModel.EPlayState.Playing;

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000891 RID: 2193 RVA: 0x0003ABD7 File Offset: 0x00038DD7
	public bool IsPlaying
	{
		get
		{
			return this.PlayState == AdventureModel.EPlayState.Playing;
		}
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000892 RID: 2194 RVA: 0x0003ABE2 File Offset: 0x00038DE2
	// (set) Token: 0x06000893 RID: 2195 RVA: 0x0003ABEA File Offset: 0x00038DEA
	public bool PlayStateIsLocked { get; private set; }

	// Token: 0x06000894 RID: 2196 RVA: 0x0003ABF3 File Offset: 0x00038DF3
	public void Init()
	{
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0003ABF6 File Offset: 0x00038DF6
	public void Dispose()
	{
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0003ABFC File Offset: 0x00038DFC
	public void SetPlayState(AdventureModel.EPlayState state)
	{
		bool flag = state == this.PlayState;
		if (!flag)
		{
			this.PlayState = state;
			GEvent.OnEvent(UiEvents.AdventurePlayStateChange, null);
		}
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0003AC34 File Offset: 0x00038E34
	public void SetPlayStateIsLocked(bool isLock)
	{
		bool flag = this.PlayStateIsLocked == isLock;
		if (!flag)
		{
			this.PlayStateIsLocked = isLock;
			GEvent.OnEvent(UiEvents.AdventurePlayStateIsLockedChange, null);
		}
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0003AC6C File Offset: 0x00038E6C
	public void RefreshCarrier(Action<List<ItemDisplayData>> action)
	{
		CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
			Serializer.Deserialize(dataPool, offset, ref equipments);
			action(equipments);
			EasyPool.Free<List<ItemDisplayData>>(equipments);
		});
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0003ACA4 File Offset: 0x00038EA4
	public void GetJiaoTravelSpeed(ItemKey itemKey, Action<sbyte> action)
	{
		bool flag = !itemKey.IsValid();
		if (flag)
		{
			action(0);
		}
		else
		{
			bool flag2 = SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
			if (flag2)
			{
				bool isLoong = itemKey.TemplateId >= 77 && itemKey.TemplateId <= 85;
				bool isJiao = itemKey.TemplateId >= 46 && itemKey.TemplateId <= 76;
				bool flag3 = isLoong;
				if (flag3)
				{
					Func<JiaoItem, bool> <>9__2;
					ExtraDomainMethod.AsyncCall.GetChildrenOfLoongByItemKey(null, itemKey, delegate(int offset, RawDataPool dataPool)
					{
						ChildrenOfLoong loong = null;
						Serializer.Deserialize(dataPool, offset, ref loong);
						bool flag5 = loong == null;
						if (flag5)
						{
							action(0);
						}
						else
						{
							IEnumerable<JiaoItem> instance = Config.Jiao.Instance;
							Func<JiaoItem, bool> predicate;
							if ((predicate = <>9__2) == null)
							{
								predicate = (<>9__2 = ((JiaoItem j) => j.IndexOfCarrierTemplate == itemKey.TemplateId));
							}
							JiaoItem jiaoConfig = instance.Single(predicate);
							int travelSpeed = loong.Properties.Get(jiaoConfig.TemplateId, 0);
							action((sbyte)travelSpeed);
						}
					});
				}
				else
				{
					bool flag4 = isJiao;
					if (flag4)
					{
						ExtraDomainMethod.AsyncCall.GetJiaoByItemKey(null, itemKey, delegate(int offset, RawDataPool dataPool)
						{
							GameData.DLC.FiveLoong.Jiao jiao = null;
							Serializer.Deserialize(dataPool, offset, ref jiao);
							bool flag5 = jiao == null;
							if (flag5)
							{
								action(0);
							}
							else
							{
								int travelSpeed = jiao.Properties.Get(jiao.TemplateId, 0);
								action((sbyte)travelSpeed);
							}
						});
					}
					else
					{
						CarrierItem configData = Carrier.Instance[itemKey.TemplateId];
						action(configData.BaseTravelTimeReduction);
					}
				}
			}
			else
			{
				CarrierItem configData2 = Carrier.Instance[itemKey.TemplateId];
				action(configData2.BaseTravelTimeReduction);
			}
		}
	}

	// Token: 0x0200114C RID: 4428
	public enum EPlayState
	{
		// Token: 0x0400967F RID: 38527
		Playing,
		// Token: 0x04009680 RID: 38528
		Paused,
		// Token: 0x04009681 RID: 38529
		End
	}
}
