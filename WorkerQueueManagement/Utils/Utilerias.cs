using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WorkerQueueManagement.Models;
using Serilog;

namespace WorkerQueueManagement.Utils
{
    public class Utilerias
    {
        public static void ImprimirLog(AppLog appLog, int responseCode, string message, string typeLog)
        {
            appLog.ResponseCode = responseCode;
            appLog.Message = message;
            appLog.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (typeLog.Equals("Error"))
            {
                appLog.Level = 40;
                Log.Error(JsonSerializer.Serialize(appLog));
            }
            else if (typeLog.Equals("Information"))
            {
                appLog.Level = 20;
                Log.Information(JsonSerializer.Serialize(appLog));
            }
            else if (typeLog.Equals("Debug")) 
           {
                    appLog.Level = 10;
                    Log.Debug(JsonSerializer.Serialize(appLog));
           }
           appLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff")) - appLog.ResponseTime;
        }
   }
}