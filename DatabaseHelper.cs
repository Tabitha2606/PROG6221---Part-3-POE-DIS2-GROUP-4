using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace ST10483814_POE_PART_3
{
    // Responsible for handling all communication with the SQL Server
    public class DatabaseHelper
    {

        private string connectionString =
            @"Server=(LocalDB)\MSSQLLocalDB;" +
            @"Database=CybersecurityBotDatabase;" +
            @"Trusted_Connection=True;";


        public bool TestDatabaseConnection()
        {
            try
            {
                // Creates a new database connection object
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true; // Successfully connected to the database
                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to the database: " + error.Message);
                return false;
            }
        }


        public int GetOrCreateUser(string userName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                string searchUserQuery =
                    "SELECT UserId FROM Users " +
                    "WHERE UserName = @name";

                // Create a SQL command to execute the user search query using the database connection 
                SqlCommand searchUserCmd = new SqlCommand(searchUserQuery, conn);
                searchUserCmd.Parameters.AddWithValue("@name", userName);

                object result = searchUserCmd.ExecuteScalar(); // Execute the query to retrieve the UserId


                if (result != null)
                {
                    int existingUserId = Convert.ToInt32(result);
                    return existingUserId;
                }


                // Create a new user record in the database if the user does not exist
                string insertUserQuery =
                    "INSERT INTO Users (UserName) " +
                    "VALUES (@name)";

                SqlCommand insertUserCmd = new SqlCommand(insertUserQuery, conn);
                insertUserCmd.Parameters.AddWithValue("@name", userName);
                insertUserCmd.ExecuteNonQuery(); // Execute the query to insert the new user


                // Retrieve the user's ID by their username
                string getNewUserIdQuery =
                    "SELECT UserId FROM Users " +
                    "WHERE UserName = @name";

                SqlCommand getUserIdCmd = new SqlCommand(getNewUserIdQuery, conn);
                getUserIdCmd.Parameters.AddWithValue("@name", userName); // Replace @name parameter with the actual username

                int newUserId = Convert.ToInt32(getUserIdCmd.ExecuteScalar());

                return newUserId;
            }
        }


        public void AddTask(int userId, string title, string description, DateTime? reminderDate)
        {
            // The connection string directs the application to the database connection
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open(); // This opens the connection to enable database connection

                string addTaskQuery =
                    "INSERT INTO Tasks " +
                    "(UserId, Title, Description, " +
                    "ReminderDate, IsCompleted, DateCreated) " +
                    "VALUES " +
                    "(@userId, @title, @description, " +
                    "@reminderDate, 0, @createdDate)";


                SqlCommand addTaskCmd = new SqlCommand(addTaskQuery, conn);
                addTaskCmd.Parameters.AddWithValue("@userId", userId);
                addTaskCmd.Parameters.AddWithValue("@title", title);
                addTaskCmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                addTaskCmd.Parameters.AddWithValue("@reminderDate", reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
                addTaskCmd.Parameters.AddWithValue("@createdDate", DateTime.Now);

                addTaskCmd.ExecuteNonQuery(); 

            }
        }


        public List<TaskItem> GetUserTasks(int userId)
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Fetch all tasks for the specified user which are sorted with the most recent tasks appearing first
                string getTasksQuery =
                    "SELECT * FROM Tasks " +
                    "WHERE UserId = @userId " +
                    "ORDER BY DateCreated DESC";


                SqlCommand getTasksCmd = new SqlCommand(getTasksQuery, conn);
                getTasksCmd.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = getTasksCmd.ExecuteReader())
                {

                    // Using a while loop to read row from the database results
                    while (reader.Read())
                    {
                        /* Read each task row from the database,
                           create a TaskItem object and add it to the list
                        */
                        tasks.Add(new TaskItem
                        {
                            TaskId = reader.GetInt32(
                                reader.GetOrdinal("TaskId")),


                            Title = reader.GetString(
                                reader.GetOrdinal("Title")),


                            Description = reader.IsDBNull(
                                reader.GetOrdinal("Description"))
                                ? ""
                                : reader.GetString(
                                    reader.GetOrdinal("Description")),


                            IsCompleted = reader.GetBoolean(
                                reader.GetOrdinal("IsCompleted")),


                            ReminderDate = reader.IsDBNull(
                                reader.GetOrdinal("ReminderDate"))
                                ? (DateTime?)null
                                : reader.GetDateTime(
                                    reader.GetOrdinal("ReminderDate")),


                            DateCreated = reader.GetDateTime(
                                reader.GetOrdinal("DateCreated")),

                        });
                    }

                }
            }

            return tasks;

            }


        public void MarkAsCompleted(int taskId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string updateTasksQuery =
                    "UPDATE Tasks " +
                    "SET IsCompleted = 1 " +
                    "WHERE TaskId = @taskId";

                // Create a SQL command to mark the task as completed in the database
                SqlCommand updateTasksCmd = new SqlCommand(updateTasksQuery, conn);
                updateTasksCmd.Parameters.AddWithValue("@taskId", taskId);

                updateTasksCmd.ExecuteNonQuery();
            }
        }


        public void DeleteTask(int taskId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string deleteTasksQuery =
                    "DELETE FROM Tasks " +
                    "WHERE TaskId = @taskId";

                // Create a SQL command to delete the task from the database
                SqlCommand deleteTasksCmd = new SqlCommand(deleteTasksQuery, conn);
                deleteTasksCmd.Parameters.AddWithValue("@taskId", taskId);

                deleteTasksCmd.ExecuteNonQuery();
            }
        }

    }

}


     
       

            
            


    

