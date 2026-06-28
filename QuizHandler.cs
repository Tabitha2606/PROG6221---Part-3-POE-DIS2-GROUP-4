using System;
using System.Collections.Generic;


namespace ST10483814_POE_PART_3
{
    // Handles all the quiz functionality features
    public class QuizHandler
    {
        private List<QuizQuestion> questions; // Stores all the quiz questions in a list
        private int currentQuestionNumber = 0;
        private int totalScore = 0;

        public QuizHandler()
        {
            PrepareQuestions();
        }

        public int GetCurrentQuestionIndex()
        {
            return currentQuestionNumber;
        }

        // A method that creates and prepares all the quiz questions with cybersecurity related topics
        private void PrepareQuestions()
        {
            questions = new List<QuizQuestion>
            {
                // Question 1
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new List<string>
                    {
                        "A type of software that protects your computer from viruses",
                        "A type of malware that encrypts files and demands payment",
                        "A password manager",
                        "A type of firewall"

                    },
                    CorrectAnswerIndex = 1, // This is the correct answer for the quiz question
                    Explanation = "Ransomware is a type of malware that locks your files " +
                                  "and demands payment in exchange for the decryption key." +
                                  "Regular backups are your best defense to stay protected."

                },

                // Question 2
                new QuizQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Options = new List<string>
                    {
                        "It usually comes from your bank",
                        "It has no attachments",
                        "It contains correct spelling and grammar",
                        "It has urgent requests for personal information"

                    },
                    CorrectAnswerIndex = 3,
                    Explanation = "Phishing emails create urgency to trick you into sharing " +
                                  "personal information. Remember to slow down, think and " +
                                  "always verify the source before responding."

                },

                // Question 3
                new QuizQuestion
                {
                    Question = "What makes a strong password?",
                    Options = new List<string>
                    {
                        "A combination of uppercase, lowercase, numbers, and special characters",
                        "A word found in the dictionary",
                        "Your name and birthday",
                        "The same passwords used for all accounts"

                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Strong passwords should consist of a mix which include an uppercase, " +
                                  "lowercase, numbers, and symbols and must at least be 12 characters  " +
                                  "long. Consider using a password manager to store them safely."

                },

                // Question 4
                new QuizQuestion
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string>
                    {
                        "Using two different passwords for the same account",
                        "Changing passwords twice in a year",
                        "A type of security method that requires two forms of verification",
                        "A password manager"

                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Two-factor authentication (2FA) is a security method that requires two " +
                                  "forms of verification and adds an extra layer of protection to your accounts." +
                                  "Always enable 2FA to keep your accounts secure."

                },

                // Question 5
                new QuizQuestion
                {
                    Question = "Which is an example of social engineering?",
                    Options = new List<string>
                    {
                        "Installing an antivirus software on your laptop",
                        "A scammer calling by pretending to be a bank representative from your bank",
                        "Updating your operating system",
                        "Backing up your files regularly"

                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Social engineering is a tactic used by scammers to manipulate " +
                                  "individuals into sharing confidential information. A scam call is a " +
                                  "common example. Always verify the identity of the anyone who is " +
                                  "requesting for personal information."

                },

                // Question 6
                new QuizQuestion
                {
                    Question = "Why should you avoid using public WI-FI for private transactions",
                    Options = new List<string>
                    {
                        "It only works in cafes",
                        "It's too slow for banking apps",
                        "It may have hidden costs that charges you for usage time ",
                        "Public WI-FI networks lacks encryption, exposing your data to attackers"

                    },
                    CorrectAnswerIndex = 3,
                    Explanation = "Public Wi-Fi has no encryption, so attackers can see everything " +
                                  "you do online. Never log into banking app on public Wi-Fi. " +
                                  "Use a VPN and enable 2FA instead for extra protection."

                },

                // Question 7
                new QuizQuestion
                {
                    Question = "What should you do if you think your account has been hacked?",
                    Options = new List<string>
                    {
                        "Contact the service provider's support team",
                        "Change your password immediately",
                        "Check your account for any suspicious activity",
                        "All of the above"

                    },
                    CorrectAnswerIndex = 3,
                    Explanation = "All of the above actions are important! Don't skip any step which " +
                                  "involves changing your password, enabling 2FA, contacting support, " +
                                  "reviewing activity to prevent future attacks."

                },

                // Question 8
                new QuizQuestion
                {
                    Question = "Why was POPIA introduced in South Africa?",
                    Options = new List<string>
                    {
                        "To protect personal information of individuals",
                        "To increase government control",
                        "To regulate only medical records",
                        "To collect more taxes"

                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "POPIA which stands for Protection of Personal Information " +
                                   "Act was introduced to protect personal information in South " +
                                   "Africa regulating how it is processed. "

                },

                // Question 9
                new QuizQuestion
                {
                    Question = "What personal information should you avoid sharing on social media?",
                    Options = new List<string>
                    {
                        "Your favourite movie",
                        "Your favourite music",
                        "Your full home address and phone number",
                        "Your hobbies"

                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Publicly sharing personal details like your address and phone number " +
                                  "increases the risk of identity theft and fraud. Always keep them private " +
                                  "at all times."

                },

                // Question 10
                new QuizQuestion
                {
                    Question = "What should you look for to ensure a website is secure?",
                    Options = new List<string>
                    {
                        "The word secure in the website name ",
                        "A padlock icon and https:// in the URL bar of the website",
                        "A website that looks so professional",
                        "The website that has a nice logo"

                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "A padlock icon and 'https://' signify a secure connection, " +
                                  "ensuring your data is encrypted and protected from unauthorized access. "


                },

                // Question 11
                new QuizQuestion
                {
                    Question = "What should you do if you receive a suspicious email asking for your password",
                    Options = new List<string>
                    {
                        "Delete it and report it as phishing",
                        "Click the link to verify for curiosity",
                        "Reply with your password",
                        "Forward the link to your friends"

                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "Legitimate companies never ask for passwords via email. Delete suspicious " +
                                  "messages immediately and report them to protect yourself and others from " +
                                  "phishing attacks."

                },

                // Question 12
                new QuizQuestion
                {
                    Question = "How often should you change your passwords for better security?",
                    Options = new List<string>
                    {
                        "Once a year",
                        "Every 3-6 months",
                        "Every 2 years",
                        "Never"

                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Regular password updates every 3-6 months help protect against unauthorized access by " +
                                  "limiting how long a compromised password can be used by attackers. Always update " +
                                  "immediately if you suspect a breach."

                }

            };

        }

        // A method that checks if a question exists at the current index(position)
        public QuizQuestion GetCurrentQuestion()
        {
            if (currentQuestionNumber < questions.Count)
            {
                return questions[currentQuestionNumber];
            }
            return null;
        }


        public string ProcessAnswer(int selectedAnswerNumber)
        {
            QuizQuestion currentQuestion = GetCurrentQuestion();

            // Check if the quiz has ended
            if (currentQuestion == null)
            {
                return "The quiz is complete.";
            }

            string feedback;

            // Compare the user's selected answers with the correct answer
            if (selectedAnswerNumber == currentQuestion.CorrectAnswerIndex)
            {
                totalScore++;
                feedback = $"Correct! {currentQuestion.Explanation}";
            }
            else
            {
                feedback = $"❌ Incorrect. {currentQuestion.Explanation}";
            }

            currentQuestionNumber++;

            return feedback;
        }

        public bool CanQuizContinue()
        {
            return currentQuestionNumber < questions.Count;
        }

        public int GetTotalScore()
        {
            return totalScore;
        }

        public int GetTotalQuestions()
        {
            return questions.Count;
        }


        public string GetQuizResults()
        {
            int totalQuestions = questions.Count;
            double percentage = (double)totalScore / totalQuestions * 100; // Calculate the percentage that the user obtained
            string performanceFeedback;

            // Provide feedback based on the user's performance
            if (totalScore == totalQuestions)
            {
                performanceFeedback = "🏆 Congratulations! You are a cybersecurity star!";
            }
            else if (percentage >= 80)
            {
                performanceFeedback = "🌟 Outstanding! You are a cybersecurity genius!";
            }
            else if (percentage >= 60)
            {
                performanceFeedback = "👏 Great job! You have demonstrated good cybersecurity awareness!";
            }
            else if (percentage >= 40)
            {
                performanceFeedback = "📚 Nice attempt! Focus on the areas you found challenging.";
            }
            else
            {
                performanceFeedback = "🔐 Keep pushing! Cybersecurity is a valuable skill worth mastering.";
            }

            // Return formatted results with specified emojis and percentages
            return $"Quiz complete! You scored {totalScore} out of " +
                   $"{totalQuestions} ({percentage:F0}%). " +
                   $"{performanceFeedback}";
        }


        public void ResetQuiz()
        {
            currentQuestionNumber = 0;
            totalScore = 0;
        }
    }

    // Defines a quiz question with multiple choice options and a correct answer
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }
}
        


                
    
