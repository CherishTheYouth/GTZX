using System;
using System.IO;

namespace Helper
{
    /// <summary>
    /// 日志类，用于记录系统的各类日志信息。
    /// </summary>
    public class LogHelper
    {
        public static string LogFolerName
        {
            get
            {
                var folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
                return folderName;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="contentText">日志内容</param>
        public static void WriteLog(LogLevel level, string contentText)
        {
            var message = string.Format("{0} [{1}] {2}{3}", DateTime.Now, level, contentText, Environment.NewLine);
            WriteLog(message, "log");
        }

        /// <summary>
        /// 日志记录异常信息
        /// </summary>
        /// <param name="exception">异常</param>
        public static void WriteLog(Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            var message = string.Format("{0} [{1}] {2} {3}{4}{5}", DateTime.Now,
                LogLevel.Error,
                exception.Message, Environment.NewLine, exception.StackTrace, Environment.NewLine);
            WriteLog(message, "log");
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="fileNameWithoutExtension">文件名（不包含扩展名）</param>
        /// <param name="autoCreateNewLog">是否自动根据文件大小创建新的日志文件（超过10m）</param>
        private static void WriteLog(string message, string fileNameWithoutExtension, bool autoCreateNewLog = true)
        {
            try
            {
                var folderName = LogFolerName;
                var fileName = Path.Combine(folderName, fileNameWithoutExtension + ".txt");
                if (!File.Exists(fileName))
                {
                    File.Create(fileName).Close();
                }

                if (autoCreateNewLog)
                {
                    var fileInfo = new FileInfo(fileName);
                    if (fileInfo.Length >= 10485760)
                    {
                        for (var i = 0; i <= 1000; i++)
                        {
                            var newFileName = string.Format("{0}{1}{2}.txt", folderName, fileNameWithoutExtension, i);
                            if (File.Exists(newFileName)) continue;
                            File.Move(fileName, newFileName);
                            break;
                        }
                    }
                }

                using (var streamWriter = new StreamWriter(fileName, true))
                {
                    streamWriter.Write(message);
                    streamWriter.Flush();
                }
            }
            catch
            {
                // 忽略
            }
        }
    }

    /// <summary>
    /// 日志的级别。
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}