using System.Collections.Generic;

namespace Ravensong.Data
{
    /// <summary>
    /// 所有 ScriptableObject 数据的验证接口。
    /// 锁定决策（data-config §C.1 规则 1-5）：
    ///   1. 三道关卡：Edit-time / Play-mode-start / Runtime per-access
    ///   2. 缺字段 = 警告，不是 null 返回
    ///   3. SchemaVersion 不可改，加字段用 [Obsolete]
    /// </summary>
    public interface IDataValidatable
    {
        /// <summary>当前 schema 版本（用于加载时迁移检查）</summary>
        string SchemaVersion { get; }

        /// <summary>Edit-time / Play-mode-start 调，返回所有错误（空 list = valid）</summary>
        List<string> Validate();
    }
}
