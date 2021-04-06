using System;
using System.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;

namespace task
{
    class TasksDBService
    {
        private NpgsqlConnection conn;
        public TasksDBService(string connString)
        {
            conn = new NpgsqlConnection(connString);
        }
        
        public void OpenConnection()
        {
            conn.Open();
        }

        // public ListOfFutureCelebrants GetEmployee()
        // {
        //     using (var cmd = new NpgsqlCommand("SELECT name, date_of_birth FROM employee", conn))
        //     {
        //         ListOfFutureCelebrants listOfEmployee = new ListOfFutureCelebrants();
        //         using (var reader = cmd.ExecuteReader())
        //         while (reader.Read())
        //         {
        //             string name = reader.GetString(0);
        //             DateTime date = reader.GetDateTime(1);
        //             listOfEmployee.Add(new Employee(name, date));
        //         }
        //         return listOfEmployee;
        //     }
        // }

        public List<Task> GetTasks()
        {
            using (var cmd = new NpgsqlCommand("SELECT title, description, due_date, done FROM tasks", conn))
            {
                List<Task> listOfTasks = new List<Task>();
                using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    Task task = new Task();

                    task.title = reader.GetString(0);
                    
                    if(!reader.IsDBNull(1))
                        task.description = reader.GetString(1);
                    
                    if(!reader.IsDBNull(2))
                        task.dueDate = reader.GetDateTime(2);
                    
                    task.done = reader.GetBoolean(3);
                    
                    listOfTasks.Add(task);
                }
                return listOfTasks;
            }
        }
        
        public void CreateTask(Task task)
        {
            var insertCmd = new NpgsqlCommand("INSERT INTO tasks (title, description, due_date, done) VALUES (@title, @description, @due_date, @done)", conn);
            
            if(task.title != null)
                insertCmd.Parameters.AddWithValue("title", task.title);
            else
                return;
            
            insertCmd.Parameters.AddWithValue("description", task.description ?? "");
            
            insertCmd.Parameters.AddWithValue("due_date", NpgsqlTypes.NpgsqlDbType.Timestamp, (object)task.dueDate ?? DBNull.Value);
            
            insertCmd.Parameters.AddWithValue("done", task.done);
            
            insertCmd.ExecuteNonQuery();
        }

        public void UpdateTask(Task task, int id)
        {
            var readCmd = new NpgsqlCommand("SELECT title, description, due_date, done FROM tasks WHERE id=@id", conn);
            readCmd.Parameters.AddWithValue("id", id);
            var reader = readCmd.ExecuteReader();
            reader.Read();


            var updateCmd = new NpgsqlCommand("UPDATE tasks SET title=@title, description=@description, due_date=@due_date, done=@done WHERE id=@id", conn);
            
            updateCmd.Parameters.AddWithValue("id", id);
            
            updateCmd.Parameters.AddWithValue("title", 
            task.title == null ? reader.GetString(0) : task.title);
            
            updateCmd.Parameters.AddWithValue("description", 
            task.description == null ? reader.IsDBNull(1) ? "" : reader.GetString(1) : task.description);
            
            updateCmd.Parameters.AddWithValue("due_date",
            task.dueDate == null ? reader.IsDBNull(2) ? DBNull.Value : reader.GetDateTime(2) : task.dueDate);
            
            updateCmd.Parameters.AddWithValue("done", task.done);
            
            reader.Close();
            updateCmd.ExecuteNonQuery();
        }

        public void TaskDelete(int id)
        {
            var deleteCmd = new NpgsqlCommand("DELETE FROM title WHERE id=@id", conn);
            deleteCmd.Parameters.AddWithValue("id", id);
            deleteCmd.ExecuteNonQuery();
        }

    }
}