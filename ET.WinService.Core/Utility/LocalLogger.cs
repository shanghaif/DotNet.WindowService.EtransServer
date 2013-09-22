using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ET.WinService.Core.Utility;

namespace ET.WinService.Core.Utility
{
    
    /// <summary>
    /// 本地日志发送类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月11日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class LocalLogger
    {
        static LoggerLocal localLogger = null;

        static LocalLogger()
        {
            localLogger = new LoggerLocal();
        }

        /// <summary>
        /// 写本地文件日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLocal(string message)
        {
            localLogger.WriteLocal(message);
        }

        /// <summary>
        /// 写系统事件日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteEvenLog(string message)
        {
            localLogger.WriteEvenLog(message);
        }
    }

    /// <summary>
    /// 记录本地日志
    /// </summary>
    internal class LoggerLocal
    {
        string LogPath = string.Empty;
        public LoggerLocal()
        {
            try
            {
                try
                {
                    string executePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string logFilePath = Path.Combine(executePath, "Logs");

                    LogPath = Path.Combine(logFilePath, "LocalLog.txt");
                    CreateLogPath(LogPath);
                }
                catch (Exception ex)
                {
                    WriteEvenLog("初始化记录本地日志实例失败" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (!System.Diagnostics.EventLog.Exists("Logger"))
                    {
                        System.Diagnostics.EventLog.CreateEventSource("ETServer", "Logger");
                    }
                    System.Diagnostics.EventLog.WriteEntry("ETServer", ex.Message);
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// 写本地文件日志
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public void WriteLocal(string message)
        {
            // 没有配置本地文件路径则写事件日志
            if (string.IsNullOrEmpty(LogPath))
            {
                WriteEvenLog(message);
                return;
            }

            FileStream fs = null;
            lock (this)
            {
                try
                {
                    // 每天一个文件
                    string filePath = LogPath;//.Replace(".", DateTime.Today.ToString("yyyyMMdd."));

                    fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    if (fs != null)
                    {
                        //UTF8Encoding(true)
                        string msg = DateTime.Now.ToString() + "-----" + message + "\r\n";
                        //byte[] msgbyte = System.Text.Encoding.Default.GetBytes(msg);
                        byte[] msgbyte = System.Text.Encoding.UTF8.GetBytes(msg);
                        //byte[] msgbyte = new System.Text.UTF8Encoding(true).GetBytes(msg);
                        fs.Write(msgbyte, 0, msgbyte.Length);
                    }
                }
                catch (Exception ex)
                {
                    WriteEvenLog("写本地日志失败，失败原因：" + ex.Message + "--日志内容：" + message);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 创建日志文件目录
        /// </summary>
        /// <param name="path">文件名称</param>
        void CreateLogPath(string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                WriteEvenLog("没有配置本地日志文件，请检查web.config配置LocalLogPath节点是否正确配置");
                return;
            }

            int index = localPath.LastIndexOf('\\');
            if (index >= 0)
            {
                string path = localPath.Remove(index + 1);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            if (!File.Exists(localPath))
            {
                using (FileStream fs = File.Create(localPath))
                { fs.Close(); }
            }
        }

        /// <summary>
        /// 写系统事件日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteEvenLog(string message)
        {
            try
            {
                if (!System.Diagnostics.EventLog.Exists("Logger"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("ETServer", "Logger");
                }
                System.Diagnostics.EventLog.WriteEntry("ETServer", message);
            }
            finally
            {
            }
        }
    }
}
