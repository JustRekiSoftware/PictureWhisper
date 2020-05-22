using System;
using System.Collections.Generic;
using System.Text;

namespace PictureWhisper.Domain.Entites
{
    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        正常,
        已删除,
        未审核,
        已注销
    }
}
