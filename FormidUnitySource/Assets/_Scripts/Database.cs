 using System;
 using System.Data;

 using MySql.Data;
 using MySql.Data.MySqlClient;

 public class Tutorial2
 {
     public static void Main()
     {
         string connStr = "server=localhost;user=root;database=FormidActiveSeating;port=3306;password=password";
         MySqlConnection conn = new MySqlConnection(connStr);
         try
         {
             Console.WriteLine("Connecting to MySQL...");
             conn.Open();

             string sql = "SELECT * FROM username";
             MySqlCommand cmd = new MySqlCommand(sql, conn);
             object result = cmd.ExecuteScalar();
             if (result != null)
             {
        
                 Console.WriteLine("Number of countries in the world database is: " + result.ToString());
             }

         }
         catch (Exception ex)
         {
             Console.WriteLine(ex);
         }

         conn.Close();
         Console.WriteLine("Done.");
     }
 }