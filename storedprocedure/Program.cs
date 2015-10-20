using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace storedprocedure
{
    class Program
    {
        static void Main(string[] args)
        {
            var users = new List<string>(){
                "BOB123",
                "SALLY4235",
                "GREG83",
                "DAN094",
                "JILL555",
                "TOM",
                "BETTY683",
                "KELLY5"
            };
            DataTable dtUserList = users.ConvertToDatatable();
            dtUserList.ToPrintConsole();
            
            int rowsAffected;
            List<string> result = new List<string>();
            
            string connString = ConfigurationManager.ConnectionStrings["helloWorld"].ToString();
            using(SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("usp_insertpropertylisting", conn))
                {
                    cmd.CommandText = "helloWorld.mirrorGUIDs";
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter tableValueParam;
                    tableValueParam = cmd.Parameters.AddWithValue("@ulist", dtUserList);
                    tableValueParam.SqlDbType = SqlDbType.Structured;
                    tableValueParam.TypeName = "helloWorld.GUIDs";

                    rowsAffected = cmd.ExecuteNonQuery();
                    
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                        result.Add(reader.GetString(1));
                }
            }
            if (rowsAffected == -1)
            {
                Console.WriteLine("NOCOUNT is on");
            }
            else
            {
                Console.WriteLine(string.Format("NOCOUNT IS OFF. {0} rows were affected."));
            }

            string userString = String.Join(" ", result.ToArray());
            Console.WriteLine(String.Format("Result: {0}", userString));
            Console.ReadLine();
        }
    }

    public static class DataTools
    {
        public static DataTable ConvertToDatatable<T>(this List<T> data)
        {
            DataTable table = new DataTable();
            DataColumn id = new DataColumn("ID", typeof(int));
            DataColumn col = new DataColumn("TEXTCOL", typeof(string));
            table.Columns.Add(id);
            table.Columns.Add(col);
            for (int i = 0; i < data.Count; i++)
            {
                table.Rows.Add(null, data[i].ToString());
            }
            return table;
        }

        public static void ToPrintConsole(this DataTable dataTable)
        {
            // Print top line
            Console.WriteLine(new string('-', 75));

            // Print col headers
            var colHeaders = dataTable.Columns.Cast<DataColumn>().Select(arg => arg.ColumnName);
            foreach (String s in colHeaders)
            {
                Console.Write("| {0,-20}", s);
            }
            Console.WriteLine();

            // Print line below col headers
            Console.WriteLine(new string('-', 75));

            // Print rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (Object o in row.ItemArray)
                {
                    Console.Write("| {0,-20}", o.ToString());
                }
                Console.WriteLine();
            }

            // Print bottom line
            Console.WriteLine(new string('-', 75));
        }
    }

}
