#region using

using System.ComponentModel;

#endregion using

namespace Dorado.VWS.Model.Enum
{
    public enum EnumTaskStatus
    {
        /// <summary>
        ///     任务等待领取
        /// </summary>
        [Description("Wait")]
        Wait = 1,

        /// <summary>
        ///     任务被领取，正在处理中
        /// </summary>
        [Description("Running")]
        Running = 2,

        /// <summary>
        ///     任务结果：成功
        /// </summary>
        [Description("Successed")]
        Successed = 3,

        /// <summary>
        ///     任务结果：失败
        /// </summary>
        [Description("Failed")]
        Failed = 4,
    }
}