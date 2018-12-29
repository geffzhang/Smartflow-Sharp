/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
namespace Smartflow
{
    public interface ILogging
    {
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="exception"></param>
        void Error(string exception);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);
    }
}
