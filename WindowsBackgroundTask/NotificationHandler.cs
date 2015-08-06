using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace WindowsBackgroundTask
{
    public static class NotificationHandler
    {
        public async static void Run()
        {
            TimeTrigger hourlyTrigger = new TimeTrigger(15, false);

            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();
            }
            catch
            {

            }

            string entryPoint = "WindowsBackgroundTask.BackgroundTask";
            string taskName = "HVZeelandBackgroundWorker";
            SystemCondition userCondition = new SystemCondition(SystemConditionType.InternetAvailable);

            BackgroundTaskRegistration task = RegisterBackgroundTask(entryPoint, taskName, hourlyTrigger, userCondition);

        }

        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint,
                                                                string taskName,
                                                                IBackgroundTrigger trigger,
                                                                IBackgroundCondition condition)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {

                if (cur.Value.Name == taskName)
                {
                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {

                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }
    }
}
