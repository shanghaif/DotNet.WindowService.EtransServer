using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.Core.Data
{

    /// <summary>
    /// 获取Database单例模式
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月14日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public sealed  class DBO
    {
        private static Database _dataBase = null;    //内部单例模式

        /// <summary>
        /// 描  述：私有构造函数
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private DBO()
        {

        }

        /// <summary>
        /// 描  述：单例模式获取数据库对象，为事务使用
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="connStringName"></param>
        /// <returns></returns>
        public static Database GetInstance()
        {
            //在并发时，使用单一对象
            if (_dataBase == null)
            {
                _dataBase = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                return _dataBase;
            }
            else
            {
                lock (_dataBase)
                {
                    return _dataBase;
                }
            }
        }
    }
}
