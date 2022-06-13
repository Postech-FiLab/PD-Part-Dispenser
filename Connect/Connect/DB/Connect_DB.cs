using MySql.Data.MySqlClient;
using System;


namespace DB
{
    class Connect_DB
    {
        public MySqlConnection myConn;
        
        public void connect(string ip,string port,string user_name, string password)
        {
            try
            {
                string myConnection = "datasource=192.168.50.2; port=3306; username=root; password=filab1020";
                //string myConnection = "Server=" + ip + "; "  +"Database="+ db_name +"; "+ "Uid=" + user_name + "; " + "Pwd=" + password;
                myConn = new MySqlConnection(myConnection);
                myConn.Open();
                Console.WriteLine("DB 연결 성공");
                            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex.Source);
            }
       
        }
        public string select_All()
        {
            string temp = string.Empty;
            
            string select_all = "SELECT * FROM postech_tb.part_table";
            MySqlCommand cmd = new MySqlCommand(select_all,myConn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if(rdr == null)
            {
                temp = "No return";
            }
            else
            {
                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (i != rdr.FieldCount - 1)
                            temp += rdr[i] + ";";
                        else if (i == rdr.FieldCount - 1)
                            temp += rdr[i] + "\n";
                    }
                    Console.WriteLine(temp);
                }
            }
            rdr.Close();
           
            return select_all;
        }

        
       
       
    }
}
