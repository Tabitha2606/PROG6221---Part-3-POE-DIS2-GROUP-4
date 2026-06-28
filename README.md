# PROG6221---Part-3-POE-DIS2-GROUP-4

# ZenShield Awareness Bot

## Cybersecurity Awareness Bot for South African Citizens

**Version:** 3.0  
**Author:** Tabitha (ST10483814)  
**Module:** PROG6221 — Programming 2A  
**Institution:** IIE Rosebank College

---

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Technologies Used](#technologies-used)
4. [Project Structure](#project-structure)
5. [Installation & Setup](#installation--setup)
6. [Database Setup](#database-setup)
7. [How to Run](#how-to-run)
8. [Usage Instructions](#usage-instructions)
9. [NLP Commands](#nlp-commands)
10. [Chat Examples](#chat-examples)
11. [Video Presentation](#video-presentation)
12. [GitHub Releases](#github-releases)

---

## Overview

ZenShield Awareness Bot is a comprehensive WPF desktop application designed to educate and protect South African citizens from cyber threats. The application combines natural language processing for intuitive chatbot conversations, persistent task management with SQL Server database storage, an interactive 12-question cybersecurity quiz, and session-based activity tracking.

**Tagline:** Together We Stay Cyber Safe Always 🇿🇦

---

## Features

### Chat Tab
- Natural Language Processing (NLP) for intent detection
- Topic-specific responses covering passwords, phishing, malware, safe browsing, privacy, and social engineering
- Sentiment detection that adapts tone based on user's emotional state
- Memory feature for follow-up questions
- Typing indicator for realistic chat experience
- Personalised responses using user's name

### Tasks Tab
- Add tasks with title, description, and optional reminder date
- Mark tasks as complete (turns green)
- Delete tasks with confirmation dialog
- Database storage using SQL Server LocalDB
- Visual colour coding: Cyan for pending tasks, Green for completed tasks
- Two-step task creation through chat dialogue

### Quiz Tab
- 12 cybersecurity multiple-choice questions
- Immediate feedback with correct/wrong indicators and explanations
- Progress bar and question counter
- Final score with percentage and performance feedback
- Restart quiz option

### Activity Log Tab
- Session-based activity tracking
- Colour-coded badges: Chat (cyan), Task (orange), Complete (green), Quiz (purple), Session (blue)
- Timestamps for each activity
- Show More/Less toggle for viewing recent or all entries
- Refresh button to update the log display

---

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# 8.0 | Application logic and backend |
| WPF (.NET 8.0) | User interface with XAML |
| SQL Server LocalDB | Data persistence for users and tasks |
| Microsoft.Data.SqlClient | Database communication |
| Consolas Font | Cyber/tech aesthetic |
| WAV Audio | Voice greeting on startup |

---

## Project Structure
ST10483814_POE_PART_3/
│
├── MainWindow.xaml # 4-tab GUI layout
├── MainWindow.xaml.cs # All interaction logic
│
├── ChatBotResponses.cs # NLP and response generation
├── AudioPlayer.cs # WAV audio playback
├── AsciiArt.cs # ASCII art banner
│
├── DatabaseHelper.cs # SQL Server communication
├── TaskItem.cs # Database task model
├── TaskDisplayItem.cs # UI display model for tasks
│
├── QuizHandler.cs # 12-question quiz logic
│
├── ActivityLog.cs # Session activity tracking
├── ActivityLogItem.cs # Activity log display model
│
├── Properties/
│ └── AssemblyInfo.cs # Assembly metadata
│
└── ST10483814_POE_PART_3.csproj # Project configuration

text

---

## Installation & Setup

### Requirements
- Visual Studio 2022 with .NET Desktop Development workload
- SQL Server LocalDB (included with Visual Studio)

### Step 1: Clone the Repository
git clone https://github.com/yourusername/ST10483814-POE-PART-3.git
cd ST10483814-POE-PART-3

text

### Step 2: Open the Project
1. Double-click ST10483814_POE_PART_3.sln to open in Visual Studio
2. Wait for NuGet packages to restore

### Step 3: Build the Project
- Press Ctrl + Shift + B to build
- Or go to Build > Build Solution

### Step 4: Verify Dependencies
- Ensure Microsoft.Data.SqlClient is installed (NuGet package)
- If not, run: `Install-Package Microsoft.Data.SqlClient`

---

Prerequisites

### Option A: Automatic Setup (Recommended)
The application will attempt to connect to the database on startup. If it doesn't exist, you'll need to publish the database:

1. Open SQL Server Object Explorer in Visual Studio
2. Right-click on SQL Server > Add SQL Server
3. Server name: (LocalDB)\MSSQLLocalDB
4. Run the SQL scripts below

### Option B: Manual Setup
Open SQL Server Management Studio (SSMS) and connect to (LocalDB)\MSSQLLocalDB, then run:

```sql
-- Create the database
CREATE DATABASE CybersecurityBotDatabase;
GO

USE CybersecurityBotDatabase;
GO

-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(100) NOT NULL UNIQUE,
    JoinDate DATETIME DEFAULT GETDATE()
);

-- Tasks Table
CREATE TABLE Tasks (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    ReminderDate DATETIME,
    IsCompleted BIT DEFAULT 0,
    DateCreated DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Insert test user (optional)
INSERT INTO Users (UserName) VALUES ('TestUser');
Option C: Visual Studio Setup
Open Server Explorer

Right-click Data Connections > Add Connection

Server name: (LocalDB)\MSSQLLocalDB

Select or enter database name: CybersecurityBotDatabase

Click OK

How To Run
Open the solution in Visual Studio
Press F5 to run the application
Enter your name when prompted

Explore the 4 tabs:
Chat: Type questions about cybersecurity
Tasks: Manage your tasks
Quiz: Test your knowledge
Activity Log: View your session history

Usage Instructions
Chat Tab Usage
Type your message in the input box at the bottom
Press Enter or click Send
The bot will respond with relevant information
Type help to see all available commands
Type exit or bye to end the conversation

Tasks Tab Usage
Fill in the task title (required)
Add a description (optional)
Select a reminder date (optional)
Click + Add Task to save
Click on any task to select it
Click Mark Complete to finish a task (turns green)
Click Delete Task to remove it (confirmation required)

Quiz Tab Usage
Click Start Quiz to begin
Select an answer by clicking A, B, C, or D
Read the feedback explaining the correct answer
Click Next Question to continue
After all questions, view your final score
Click Restart Quiz to try again

Activity Log Tab Usage
View all recent activities with timestamps
Click Refresh to update the log
Click Show More to see full history
Click Show Less to return to recent view

NLP Commands
Command	Description	Example
add task - [task name]	Create a task via chat	add task - enable 2FA
add a task - [task name]	Alternative phrasing	add a task - change password
create task - [task name]	Alternative phrasing	create task - backup files
remind me to [task name]	Alternative phrasing	remind me to update antivirus
view tasks	Open the Tasks tab	view tasks
show tasks	Alternative phrasing	show tasks
my tasks	Alternative phrasing	my tasks
quiz me	Open the Quiz tab	quiz me
start quiz	Alternative phrasing	start quiz
test my knowledge	Alternative phrasing	test my knowledge
show log	Open the Activity Log tab	show log
recent activity	Alternative phrasing	recent activity
help	Show available commands	help
exit / bye	End the conversation	exit

Chat Examples
Example 1: Adding a Task via Chat
text
You: add task - enable two factor authentication
ZenShield: Task: "enable two factor authentication" noted! Would you like a reminder? 
         (Reply: Yes / No / tomorrow / 3 days / 1 week)

You: yes, remind me in 3 days
ZenShield: ✅ Task saved: "enable two factor authentication" with reminder on 01 Jul 2026.


Example 2: Asking About Cybersecurity
text
You: I want to learn about passwords
ZenShield: Password safety is extremely important Tabitha!
         A strong password must be at least 12 characters long and include a mix of letters, 
         numbers and symbols. Avoid using your personal details such as your name or birthday...


Example 3: Viewing Tasks
text
You: view tasks
ZenShield: Opening your tasks! You can view and manage them in the Tasks tab.
[Automatically navigates to the Tasks tab]


Video Presentation
The full demonstration video covers:

Introduction (0:00 - 0:30)
Code Walkthrough (1:00 - 2:30)
Chat Tab + NLP (2:30 - 3:30)
Task Adding via Chat (3:30 - 4:00)
Tasks Tab CRUD Operations (4:00 - 5:00)
Quiz Tab (5:00 - 5:30)
Activity Log Tab (5:30 - 6:30)

GitHub Releases
Release	Description
v1.0	Console-based chatbot with NLP and keyword responses
v2.0	WPF GUI with database integration
v3.0	Full application with quiz and activity log


