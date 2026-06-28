using System;
using System.Collections.Generic;


namespace ST10483814_POE_PART_3
{
    /*Tracks all significant actions performed
       by the chatbot during the current session
    */
    public class ActivityLog
    {
        private List<string> activityLog = new List<string>();

        // Creates and adds a new activity with a timestamp for tracking user actions
        public void AddActivityLog(string actionDescription)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            activityLog.Add($"[{timestamp}] {actionDescription}");
        }

        public void AddActivity(string actionDescription)
        {
            AddActivityLog(actionDescription);
        }


        public string GetRecentActivityLog(int count = 5)
        {
            if (activityLog.Count == 0)
            {
                return "No activities logged yet.\n\n" +
                        "Start using the chatbot\n" +
                        "to see your activities appear here.";
            }


            int startIndex = Math.Max(0, activityLog.Count - count);
            string logHistory = "Recent activities:\n\n";


            // Using a for loop to iterate through and display the most recent activities
            for (int i = startIndex; i < startIndex + count; i++)
            {
                logHistory += $" {i - startIndex + 1}. {activityLog[i]}\n";
            }

            if (activityLog.Count > count)
            {
                logHistory += $"\n Click 'Show More' for full history " +
                             $"{activityLog.Count} activityLog";
            }

            return logHistory;
        }


        public string GetAllActivityLog()
        {
            if (activityLog.Count == 0)
            {
                return "No activities have been logged yet";
            }

            string logHistory = "Here's your complete activity history:\n\n";


            for (int i = 0; i < activityLog.Count; i++)
            {
                logHistory += $" {i + 1}. {activityLog[i]}\n";
            }

            return logHistory;
        }

        // Returns the total number of activities logged onto this session
        public int GetActivityLogCount()
        {
            return activityLog.Count;
        }
    }
}
 
