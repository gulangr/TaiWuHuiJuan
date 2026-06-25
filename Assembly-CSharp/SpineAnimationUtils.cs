using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

// Token: 0x0200035F RID: 863
public static class SpineAnimationUtils
{
	// Token: 0x06003233 RID: 12851 RVA: 0x0018C17C File Offset: 0x0018A37C
	public static Attachment GetExternAttachment(this ISkeletonAnimation skeleton, string slotName, string attachmentName)
	{
		bool flag = SpineAnimationUtils.ExternSkeletonSpriteProvider == null;
		Attachment result;
		if (flag)
		{
			result = null;
		}
		else
		{
			Sprite externSprite = SpineAnimationUtils.ExternSkeletonSpriteProvider(slotName, attachmentName);
			bool flag2 = null == externSprite;
			if (flag2)
			{
				result = null;
			}
			else
			{
				externSprite.name = attachmentName;
				if (SpineAnimationUtils.ExternSkeletonMaterial == null)
				{
					SpineAnimationUtils.ExternSkeletonMaterial = new Material(Shader.Find("Spine/Skeleton"));
				}
				if (SpineAnimationUtils.ExternSkeletonSpriteAttachmentsCache == null)
				{
					SpineAnimationUtils.ExternSkeletonSpriteAttachmentsCache = new Dictionary<Sprite, Attachment>();
				}
				Attachment cachedAttachment;
				bool flag3 = SpineAnimationUtils.ExternSkeletonSpriteAttachmentsCache.TryGetValue(externSprite, out cachedAttachment);
				if (flag3)
				{
					result = cachedAttachment;
				}
				else
				{
					Attachment templateAttachment = skeleton.Skeleton.FindSlot(slotName).Attachment;
					cachedAttachment = externSprite.ToRegionAttachmentPMAClone(SpineAnimationUtils.ExternSkeletonMaterial, TextureFormat.RGBA32, false, 0f);
					RegionAttachment regionAttachment = cachedAttachment as RegionAttachment;
					RegionAttachment prevRegionAttachment;
					bool flag4;
					if (regionAttachment != null)
					{
						prevRegionAttachment = (templateAttachment as RegionAttachment);
						flag4 = (prevRegionAttachment != null);
					}
					else
					{
						flag4 = false;
					}
					bool flag5 = flag4;
					if (flag5)
					{
						regionAttachment.SetScale(Vector2.one * 10f);
						regionAttachment.UpdateRegion();
						regionAttachment.Rotation = prevRegionAttachment.Rotation;
						regionAttachment.SetPositionOffset(prevRegionAttachment.X, prevRegionAttachment.Y);
						regionAttachment.UpdateRegion();
					}
					SpineAnimationUtils.ExternSkeletonSpriteAttachmentsCache.Add(externSprite, cachedAttachment);
					result = cachedAttachment;
				}
			}
		}
		return result;
	}

	// Token: 0x040024BB RID: 9403
	public static SpineAnimationUtils.ExternSkeletonSpriteProviderDelegate ExternSkeletonSpriteProvider;

	// Token: 0x040024BC RID: 9404
	public static Material ExternSkeletonMaterial;

	// Token: 0x040024BD RID: 9405
	public static Dictionary<Sprite, Attachment> ExternSkeletonSpriteAttachmentsCache;

	// Token: 0x0200171F RID: 5919
	// (Invoke) Token: 0x0600D326 RID: 54054
	public delegate Sprite ExternSkeletonSpriteProviderDelegate(string slotName, string attachmentName);
}
