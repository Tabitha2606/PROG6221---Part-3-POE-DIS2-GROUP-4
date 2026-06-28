
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace ST10483814_POE_PART_3
{

    /*The MainWindow class is code-behind for MainWindow.xaml 
      which handles the GUI interaction logic
     */
    public partial class MainWindow : Window
    {
        private ChatBotResponses bot;

        private DatabaseHelper database = new DatabaseHelper();
        private QuizHandler quiz = new QuizHandler();
        private ActivityLog activity = new ActivityLog();

        private List<ActivityLogItem> allLogItems = new List<ActivityLogItem>();

        private int currentUserId = 0;

        
        private bool isNameEntered = false; // Checks if the user has provided their name yet

        private bool taskWaitingForReminder = false;

        private TaskItem currentTaskBeingAdded = null;
        
        private DispatcherTimer clockTimer; // A timer that updates the clock display every second

        private AudioPlayer audioPlayer = new AudioPlayer();




        // A constructor for the MainWindow class
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Attempt to connect to the database and notify the user if the connection fails
            if (!database.TestDatabaseConnection())
            {
                MessageBox.Show(
                    "Database connection failed.\n\n" +
                    "Please ensure that the CybersecurityBotDB is available.\n" +
                    "Chat and Quiz functionality can still work without database access.",
                    "Database Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            audioPlayer.PlayWav();
            StartClock();
            DisplayWelcome(); 
            txtUserInput.Focus();
        }


        /* A method that initializes and starts the clock timer to update the
           status bar with the current time
        */
        private void StartClock()
        {
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);  
            clockTimer.Tick += (s, e) =>
            {
                
                txtDateTime.Text = DateTime.Now.ToString("dd MMM yyyy — HH:mm:ss");
            };
            clockTimer.Start();  // Starts the timer

        }


        // A method that displays a welcome message and prompts for the user's name 
        private void DisplayWelcome()
        {
            // Adds a coloured message to the chat history display
            AppendMessage("ZenShield",
                "Hello there! Welcome to the ZenShield Awareness Bot!",
                Brushes.Cyan);

            AppendMessage("ZenShield",
                "I am your personal assistant to helping you stay safe online.",
                Brushes.Cyan);

            AppendMessage("ZenShield",
                "Together we stay cyber safe always! 🔒",
                Brushes.Cyan);

            AppendMessage("ZenShield",
                "Please enter your name to get started",
                Brushes.Cyan);

            AppendDivider(); // Adds a grey line between sections to the chat for better readability
        }


        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }


        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Checks if the Enter key was pressed by user
            {
                ProcessInput();
                e.Handled = true;
            }
        }

        private void txtUserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(txtUserInput.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Clear all blocks from the
             * RichTextBox document
             */
            chatHistorySection.Document.Blocks.Clear();

            /*
             * Show fresh welcome message
             * after clearing the chat
             */
            AppendMessage("ZenShield",
                "Chat cleared! How can I help you?",
                Brushes.Cyan);

            AppendDivider();
            txtUserInput.Focus();
        }



        // A method that is used to process user input and handle relevant chatbot responses
        private void ProcessInput()
        {
            string userInput = txtUserInput.Text.Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                AppendMessage("ZenShield",
                    "Hmmm I didn't quite catch that! Please type something so I can help you...",
                    Brushes.Yellow);
                return;
            }

            AppendMessage("You", userInput, Brushes.Magenta);

            // Clear the current input box and focus it for the next message
            txtUserInput.Clear();
            txtUserInput.Focus();


            if (!isNameEntered)
            {
                ProcessName(userInput);
                return;
            }
            ProcessChat(userInput);
        }


        /* A method that captures and processes the user's name at the start
          of the conversation and provides a personalized welcome message
        */
        private void ProcessName(string name)
        {
            // An instance of the ChatBotResponsesclass and passes the user's name to it
            bot = new ChatBotResponses(name);

            isNameEntered = true;

            this.Title = $"ZenShield Awareness Bot — Protecting {name}";


            try
            {
                // Get or create the user in the database
                currentUserId = database.GetOrCreateUser(name);
               
                LoadTasksFromDatabase();

            }
            catch (Exception error)
            {
                
                MessageBox.Show(
                    "Database error: " + error.Message + "\n\n" +
                    "Please try restarting the application or contact support if the issue persists.",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            LogActivity("Session", "session", $"Session started for: {name}");


            AppendMessage("ZenShield",
                $"Good day {name}! It is such a pleasure to meet you!",
                Brushes.Cyan);

            AppendMessage("ZenShield",
                $"Awareness is your greatest weapon against cybercrime {name}.",
                Brushes.Cyan);

            AppendMessage("ZenShield",
                "I am here to make sure you are always informed and always protected online.",
                Brushes.Cyan);

            AppendMessage("ZenShield",
                "Type 'help' to see everything I can assist you with.",
                Brushes.Yellow);

            AppendMessage("ZenShield",
                "Type 'exit' or 'bye' to end our conversation.",
                Brushes.Yellow);

            AppendDivider();
        }


        private void ProcessChat(string userInput)
        {
            // Convert input to lowercase and remove extra spaces for accurate keyword matching
            string lowerInput = userInput.ToLower().Trim();

            if (lowerInput == "exit" ||
                lowerInput == "bye")
            {

                AppendMessage("ZenShield",
                    $"Goodbye! It was great chatting with you {bot.GetUserName}!",
                    Brushes.Cyan);

                AppendMessage("ZenShield",
                    "Rememeber to stay vigilant and cyber safe always.",
                    Brushes.Cyan);


                txtUserInput.IsEnabled = false;
                btnSend.IsEnabled = false;

                AppendDivider();
                return;
            }

            if (taskWaitingForReminder)
            {
                ProcessReminderResponse(userInput);
                return;
            }

            // Check for user commands via NLP
            string nlpCommand = bot.ProcessNaturalLanguage(lowerInput);

            if (nlpCommand == "ADD_TASK")
            {
                HandleTaskAddRequest(userInput);
                return;
            }


            if (nlpCommand == "VIEW_TASKS")
            {
                AppendMessage("ZenShield",
                    "Opening your tasks! You can view and manage them in the Tasks tab.",
                    Brushes.Cyan);
                mainTabControl.SelectedIndex = 1;
                if (currentUserId > 0)
                    LoadTasksFromDatabase();
                LogActivity("Chat", "navigation", "Opened Tasks tab via chat");
                AppendDivider();
                return;
            }


            if (nlpCommand == "QUIZ_START")
            {
                AppendMessage("ZenShield",
                    "Great! Opening the Quiz tab. Let's put your cybersecurity knowledge to the test!",
                    Brushes.Cyan);
                mainTabControl.SelectedIndex = 2;
                LogActivity("Chat", "navigation", "Opened Quiz tab via chat");
                AppendDivider();
                return;
            }

            if (nlpCommand == "SHOW_LOG")
            {
                AppendMessage("ZenShield",
                    "Opening your Activity Log now!",
                    Brushes.Cyan);
                mainTabControl.SelectedIndex = 3;
                RefreshLogDisplay(showAll: false);
                LogActivity("Chat", "navigation", "Opened Activity Log via chat");
                AppendDivider();
                return;
            }


            string response = bot.GetResponse(lowerInput);
            LogActivity("Chat", "navigate",
                $"Chat about: {userInput.Substring(0, Math.Min(40, userInput.Length))}");
            ShowTypingThenRespond(response);

            AppendDivider();

        }



        private async void ShowTypingThenRespond(string response)
        {
            //Display typing indicator in dark grey to show bot is busy
            AppendMessage("ZenShield", "typing...",
                new SolidColorBrush(Color.FromRgb(80, 80, 100)));


            // Wait 1 second to simulate the bot thinking and typing
            await Task.Delay(1000);

            var doc = chatHistorySection.Document;
            if (doc.Blocks.LastBlock != null)
            {
                doc.Blocks.Remove(doc.Blocks.LastBlock);
                doc.Blocks.Remove(doc.Blocks.LastBlock);
            }

            
            AppendMessage("ZenShield", response, Brushes.White);
            AppendDivider();
        }

        // A method that handles the user's request to add a new task
        private void HandleTaskAddRequest(string userInput)
        {
            string taskName = userInput
                .Replace("add task - ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("add task ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("create task - ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("remind me to ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (string.IsNullOrWhiteSpace(taskName))
            {
                AppendMessage("ZenShield",
                    "Please provide a task name. Example: 'add task - enable 2FA'",
                    Brushes.Yellow);
                return;
            }

            currentTaskBeingAdded = new TaskItem
            {
                Title = taskName,
                IsCompleted = false,
                DateCreated = DateTime.Now
            };

            // Set flag so next message is reminder response
            taskWaitingForReminder = true;

            // Ask about reminder
            AppendMessage("ZenShield",
                $"Task: \"{taskName}\" noted! Would you like a reminder? " +
                "(Reply: Yes / No / tomorrow / 3 days / 1 week)",
                Brushes.Cyan);

            AppendDivider();
        }

        private void ProcessReminderResponse(string response)
        {
            // Reset flag so next message is normal chat
            taskWaitingForReminder = false;

            string lower = response.ToLower();
            DateTime? reminderDate = null;


            if (lower.Contains("no") ||
                lower.Contains("never") ||
                lower.Contains("none"))
            {
                reminderDate = null; // No reminder
            }
            else if (lower.Contains("tomorrow"))
            {
                reminderDate = DateTime.Now.AddDays(1);
            }
            else if (lower.Contains("week"))
            {
                int weeks = ExtractNumber(response);
                reminderDate = DateTime.Now.AddDays(weeks > 0 ? weeks * 7 : 7);
            }
            else if (lower.Contains("day"))
            {
                int days = ExtractNumber(response);
                reminderDate = DateTime.Now.AddDays(days > 0 ? days : 3);
            }
            else if (lower.Contains("yes") ||
                     lower.Contains("remind"))
            {
                reminderDate = DateTime.Now.AddDays(3); // Default to 3 days
            }

            try
            {
                // Save task to database
                database.AddTask(currentUserId, currentTaskBeingAdded.Title, "", reminderDate);

                string reminderText = reminderDate.HasValue
                    ? $"with reminder on {reminderDate.Value:dd MMM yyyy}"
                    : "with no reminder";

                LogActivity("Task", "task",
                    $"Task added via chat: '{currentTaskBeingAdded.Title}'" +
                    (reminderDate.HasValue ? $" — Reminder: {reminderDate.Value:dd MMM yyyy}" : ""));


                LoadTasksFromDatabase();


                AppendMessage("ZenShield",
                    $"✅ Task saved: \"{currentTaskBeingAdded.Title}\" {reminderText}.",
                    Brushes.Cyan);
            }
            catch (Exception error)
            {
                MessageBox.Show($"Error saving task: {error.Message}");
            }

            // Clear temporary task data
            currentTaskBeingAdded = null;
            AppendDivider();
        }




        // Loads all tasks for the current user from the database 
        private void LoadTasksFromDatabase()
        {
            try
            {

                List<TaskItem> tasks = database.GetUserTasks(currentUserId);
                var displayItems = new List<TaskItem>();

                foreach (TaskItem t in tasks)
                {
                    displayItems.Add(TaskItem.FromTaskItem(t));
                }

                // Update the ListBox
                lvTasks.ItemsSource = null;
                lvTasks.ItemsSource = displayItems;

            }
            catch (Exception error)
            {
                MessageBox.Show(
                    "Couldn't load your tasks. Please try again.\n\n" +
                    $"Error: {error.Message}",
                    "Tasks Error", 
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
    }


        // A method that handles the selection change event of the task list view    
        private void lvTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTasks.SelectedItem != null)
            {
                var selected = (TaskItem)lvTasks.SelectedItem;
                
                txtTaskStatus.Text = selected.IsCompleted
                    ? "Selected: completed task"
                    : $"Selected: {selected.RawTitle}";
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            // Checks if title is empty
            if (string.IsNullOrWhiteSpace(txtTaskTitle.Text))
            {
                MessageBox.Show(
                    "Please enter a task title.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                string title = txtTaskTitle.Text.Trim();
                string description = txtTaskDescription.Text.Trim();
                DateTime? reminder = dpReminderDate.SelectedDate;

               
                database.AddTask(currentUserId, title, description, reminder);

                
                LogActivity("Task", "task",
                    $"Task added: '{title}'" +
                    (reminder.HasValue
                        ? $" — Reminder: {reminder.Value:dd MMM yyyy}"
                        : ""));

                // Clear the form
                txtTaskTitle.Clear();
                txtTaskDescription.Clear();
                dpReminderDate.SelectedDate = null;

                LoadTasksFromDatabase();

                txtTaskStatus.Text = $"Task '{title}' added successfully!";

            }
            catch (Exception error)
            {
                MessageBox.Show("Error adding task: " + error.Message);
            }
        }


        private void btnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            // Checks if a task is selected
            if (lvTasks.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a task first.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }


            try
            {
                var selected = (TaskItem)lvTasks.SelectedItem;

                // Check if already completed
                if (selected.IsCompleted)
                {
                    MessageBox.Show(
                            "This task is already completed!",
                            "Already Done",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    return;

                }   
                    

                // Mark as completed in database
                database.MarkAsCompleted(selected.TaskId);

               
                LogActivity("TaskComplete", "task",
                    $"Task completed: '{selected.RawTitle}'");

                // Refresh the task list
                LoadTasksFromDatabase();

                txtTaskStatus.Text = "Task marked as complete!";
            }
            catch (Exception error)
            {
                MessageBox.Show($"Error: { error.Message}");
            }
        }


        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            // Validate: Check if a task is selected
            if (lvTasks.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a task first.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                var selected = (TaskDisplayItem)lvTasks.SelectedItem;

                // Confirm deletion with the user
                MessageBoxResult confirm = MessageBox.Show(
                    $"Delete '{selected.RawTitle}'?\nThis cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                   
                    database.DeleteTask(selected.TaskId);

                    
                    LogActivity("Task", "task",
                         $"Task deleted: '{selected.RawTitle}'");

                    // Refresh the task list
                    LoadTasksFromDatabase();

                    txtTaskStatus.Text = "Task has been deleted successfully.";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"Error: {error.Message}");
            }
        }


        // A method that handles the click event of the Start Quiz button
        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            quiz.ResetQuiz(); // Reset to question 1

            LogActivity("Quiz", "quiz", "Quiz started");

           
            btnStartQuiz.Visibility = Visibility.Collapsed;
            btnNextQuestion.Visibility = Visibility.Collapsed;
            feedbackBorder.Visibility = Visibility.Collapsed;

            DisplayQuizQuestion();
        }

        private void DisplayQuizQuestion()
        {
            
            QuizQuestion question = quiz.GetCurrentQuestion();

            // Checks if there no any questions available, show final score
            if (question == null)
            {
                ShowFinalScore();
                return;
            }

            
            int current = quiz.GetCurrentQuestionIndex() + 1;
            int total = quiz.GetTotalQuestions();

            txtQuizCounter.Text = $"Question {current} of {total}";
            progressQuiz.Value = current - 1;

            
            txtQuizQuestion.Text = question.Question;

            quizOptionsPanel.Children.Clear();
            feedbackBorder.Visibility = Visibility.Collapsed;
            btnNextQuestion.Visibility = Visibility.Collapsed;

            // Create answer buttons (A, B, C, D)
            for (int i = 0; i < question.Options.Count; i++)
            {
                int index = i;
                char letter = (char)('A' + i);

                Button optBtn = new Button
                {
                    Content = $"{letter})  {question.Options[i]}",
                    Background = new SolidColorBrush(Color.FromRgb(30, 30, 50)),
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(51, 51, 85)),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(12, 9, 12, 9),
                    Margin = new Thickness(0, 0, 0, 6),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Cursor = Cursors.Hand
                };

               
                optBtn.Template = CreateRoundedButtonTemplate();

               
                optBtn.Click += (s, ev) => HandleQuizAnswer(index);

               
                quizOptionsPanel.Children.Add(optBtn);
            }
        }


        private void HandleQuizAnswer(int selectedIndex)
        {
            QuizQuestion question = quiz.GetCurrentQuestion();

            // Check if the answer is correct
            string feedback= quiz.ProcessAnswer(selectedIndex);

            char correctLetter = (char)('A' + question.CorrectAnswerIndex);

            
            for (int i = 0; i < quizOptionsPanel.Children.Count; i++)
            {
                Button btn = (Button)quizOptionsPanel.Children[i];
                btn.IsEnabled = false; 

                if (i == question.CorrectAnswerIndex)
                {
                    // Correct answer: Green
                    btn.Background = new SolidColorBrush(Color.FromRgb(26, 58, 26));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
                else if (i == selectedIndex && !feedback.Contains("Correct"))
                {
                    // Wrong answer: Red
                    btn.Background = new SolidColorBrush(Color.FromRgb(58, 26, 26));
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(255, 68, 68));
                    btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 68, 68));
                }
            }

            // Show feedback box
            feedbackBorder.Visibility = Visibility.Visible;

            if (feedback.Contains("Correct"))
            {
                
                feedbackBorder.Background = new SolidColorBrush(Color.FromRgb(26, 58, 26));
                feedbackBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                feedbackBorder.BorderThickness = new Thickness(1);
                txtQuizFeedback.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                txtQuizFeedback.Text = $"✅  Correct!  {feedback}";
            }
            else
            {
                
                feedbackBorder.Background = new SolidColorBrush(Color.FromRgb(58, 26, 26));
                feedbackBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 68, 6));
                feedbackBorder.BorderThickness = new Thickness(1);
                txtQuizFeedback.Foreground = new SolidColorBrush(Color.FromRgb(255, 68, 68));
                txtQuizFeedback.Text = feedback;
                    
            }

            // Show the Next Question button
            btnNextQuestion.Visibility = Visibility.Visible;
        }


        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (quiz.CanQuizContinue())
                DisplayQuizQuestion(); // Show next question
            else
                ShowFinalScore(); // Quiz complete
        }

        private void ShowFinalScore()
        {
            int score = quiz.GetTotalScore();
            int total = quiz.GetTotalQuestions();
            string results = quiz.GetQuizResults();

            // Log quiz completion
            LogActivity("Quiz", "quiz", $"Quiz completed — Score: {score}/{total}");

            // Display results
            txtQuizCounter.Text = "Quiz Complete!";
            txtQuizQuestion.Text = results;
               

            quizOptionsPanel.Children.Clear();
            progressQuiz.Value = total;

            feedbackBorder.Visibility = Visibility.Collapsed;
            btnNextQuestion.Visibility = Visibility.Collapsed;
            btnStartQuiz.Content = "↺  Take Quiz Again";
            btnStartQuiz.Visibility = Visibility.Visible;
        }

        private void btnRestartQuiz_Click(object sender, RoutedEventArgs e)
        {
            btnStartQuiz_Click(sender, e);
        }


        private void LogActivity(string type, string activityType, string description)
        {
            // Add to the in-memory activity log
            activityLogList.AddActivity(description);

            // Create a display item with timestamp
            string time = DateTime.Now.ToString("HH:mm");
            var item = ActivityLogItem.Create(time, type, description);

            // Insert at the beginning (newest first)
            allLogItems.Insert(0, item);
        }


        private void RefreshLogDisplay(bool showAll)
        {
            if (showAll)
                activityLogList.ItemsSource = allLogItems; // Show all
            else
                activityLogList.ItemsSource = allLogItems.Count > 5
                    ? allLogItems.GetRange(0, 5) // Show only 5 most recent
                    : allLogItems;
        }


        private void btnRefreshLog_Click(object sender, RoutedEventArgs e)
        {
            RefreshLogDisplay(showAll: false);
        }

        /// <summary>
        /// Toggles between showing 5 entries and showing all entries.
        /// Updates the button text to "Show More" or "Show Less" accordingly.
        /// </summary>
        private void btnShowMoreLog_Click(object sender, RoutedEventArgs e)
        {
            // Check current state: showing all or showing only 5?
            bool isShowingAll = activityLogList.ItemsSource == allLogItems;

            // Toggle the display mode
            RefreshLogDisplay(showAll: !isShowingAll);

            // Update button text
            btnShowMoreLog.Content = isShowingAll ? "Show More" : "Show Less";
        }
























        private void AppendMessage(string sender, string message, Brush colour)
        {
            FlowDocument doc = chatHistorySection.Document; // A document that is used to display the chat history in a rich text format
            Paragraph paragraph = new Paragraph();

            Run senderRun = new Run($"{sender}: ")
            {
                FontWeight = FontWeights.Bold,
                Foreground = colour
            };

            paragraph.Inlines.Add(senderRun); // This includes the sender's name in the paragraph content


            Run messageRun = new Run(message)
            {
                Foreground = Brushes.White
            };

            paragraph.Inlines.Add(messageRun);
            paragraph.Margin = new Thickness(0, 2, 0, 2); // Adds spacing between messages
            doc.Blocks.Add(paragraph); // Adds the paragraph to the document to be displayed in the chat history

            // Scrolls down to the bottom to ensure that most recent messages are visible to the user
            scrollViewer.ScrollToBottom();
        }


        private void AppendDivider()
        {
            FlowDocument doc = chatHistorySection.Document;
            Paragraph divider = new Paragraph(
                new Run(
                      " ───────────────────────────────────────────────────────────────────────────────────── ")
                {
                    Foreground = new SolidColorBrush(
                        Color.FromRgb(50, 50, 80))
                });

            divider.Margin = new Thickness(0, 2, 0, 2);
            doc.Blocks.Add(divider);

            scrollViewer.ScrollToBottom();

        }

        private int ExtractNumber(string text)
        {
            // Remove all non-digit characters
            string digits = System.Text.RegularExpressions.Regex.Replace(text, "[^0-9]", "");
            return int.TryParse(digits, out int n) ? n : 0;
        }

        private ControlTemplate CreateRoundedButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));

            // Bind the Background property
            border.SetBinding(Border.BackgroundProperty,
                new System.Windows.Data.Binding("Background")
                {
                    RelativeSource = new System.Windows.Data.RelativeSource(
                        System.Windows.Data.RelativeSourceMode.TemplatedParent)
                });

            // Bind the BorderBrush property
            border.SetBinding(Border.BorderBrushProperty,
                new System.Windows.Data.Binding("BorderBrush")
                {
                    RelativeSource = new System.Windows.Data.RelativeSource(
                        System.Windows.Data.RelativeSourceMode.TemplatedParent)
                });

            border.SetBinding(Border.BorderThicknessProperty,
               new System.Windows.Data.Binding("BorderThickness")
               {
                   RelativeSource = new System.Windows.Data.RelativeSource(
                       System.Windows.Data.RelativeSourceMode.TemplatedParent)
               });

            // Set fixed corner radius (rounded corners)
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));

            // Bind the Padding property
            border.SetBinding(Border.PaddingProperty,
                new System.Windows.Data.Binding("Padding")
                {
                    RelativeSource = new System.Windows.Data.RelativeSource(
                        System.Windows.Data.RelativeSourceMode.TemplatedParent)
                });

            // Create the content presenter
            var content = new FrameworkElementFactory(typeof(ContentPresenter));
            content.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            content.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Build the visual tree
            border.AppendChild(content);
            template.VisualTree = border;
            return template;
        }

        private void btnTabQuiz_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}




            
