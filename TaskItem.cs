using System;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Media;


namespace ST10483814_POE_PART_3
{
    // Represents one row from the Tasks table in the database
    public class TaskItem
    {

        public int TaskId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? ReminderDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime DateCreated { get; set; }

        public string TitleDisplay => IsCompleted ? $"✓ {Title}" : Title;
        public string RawTitle => Title;

        public Brush TitleColor => IsCompleted
            ? new SolidColorBrush(Color.FromRgb(100, 200, 100))  // Green for completed
            : new SolidColorBrush(Color.FromRgb(0, 255, 255));

        public Visibility DescriptionVisibility => string.IsNullOrWhiteSpace(Description)
           ? Visibility.Collapsed
           : Visibility.Visible;

        public Visibility ReminderVisibility => ReminderDate.HasValue
            ? Visibility.Visible
            : Visibility.Collapsed;
        public string ReminderDisplay => ReminderDate.HasValue
           ? $"📅 Reminder: {ReminderDate.Value:dd MMM yyyy}"
           : "";



        /* Overrides ToString() to control how the task appears
           in the Tasks tab ListView
        */
        public override string ToString()
        {
            string status = IsCompleted ? "✅ " : "⏳ ";

            string reminder = ReminderDate.HasValue
                ? $"| 📅 {ReminderDate.Value:dd MMM yyyy}"
                    : "| No reminder set";

            return $"{status} {Title} {reminder}";


        }




    }
}
