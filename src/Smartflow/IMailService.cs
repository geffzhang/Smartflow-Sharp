/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
namespace Smartflow
{
    /// <summary>
    /// 邮件服务接口
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// 提供邮件通知接口
        /// </summary>
        /// <param name="to">接收人邮箱地址</param>
        /// <param name="body">发送内容</param>
        void Notification(string[] to, string body);
    }
}
