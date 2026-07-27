using System;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 物品堆叠容器（不在 Assets 下挂，运行时新建）。
    /// data-config §C.2 规则 3：跨资产引用用 ID 字符串，不用 SO 引用。
    /// </summary>
    [Serializable]
    public class ItemStack
    {
        [Tooltip("物品 ID（对应 ItemSO.id），不用 SO 引用避免 GUID 漂移")]
        public string itemId;

        [Min(0)] public int count = 1;

        public ItemStack() { }
        public ItemStack(string itemId, int count)
        {
            this.itemId = itemId;
            this.count = count;
        }

        public bool IsEmpty => string.IsNullOrEmpty(itemId) || count <= 0;

        public override string ToString() => $"{itemId} x{count}";
    }
}
