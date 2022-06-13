using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scan.DB
{
    class DataBase
    {
        private static MySqlConnection dbConnection;

        string[] cartridge_serial_list = new string[12];


        public void connect()
        {
            string connectString = string.Format("Server={0};Port={1};Database={2};Uid ={3};Pwd={4};Connect Timeout=28000",
        "192.168.50.2", "3306", "postech_tb", "root", "filab1020");
            try
            {
                dbConnection = new MySqlConnection(connectString);
           
                dbConnection.Open();
                Console.WriteLine("마리아 DB 오픈 완료");

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }

        public string[] Select_all()
        {
            DataTable dataTable = new DataTable();
            string select_all_sql = "SELECT * FROM part_dispenser";
            MySqlCommand cmd = new MySqlCommand(select_all_sql, dbConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dataTable);

            //Console.WriteLine(dataTable.Rows);
            //Console.WriteLine(dataTable.Columns);
            Console.Write("Columns : ");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                Console.Write("{0}, ", dataTable.Columns[i]);
            }
            Console.WriteLine("");
            Console.WriteLine("-----------------------------------");
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow dr = dataTable.Rows[i];
                Console.WriteLine("{0}, {1}, {2}", dr[0], dr[1], dr[2]);
                Console.WriteLine(dr[1].GetType());
                cartridge_serial_list[i] = (String)dr[1];
            }
    
            Console.WriteLine("-----------------------------------");          
            da.Dispose();
            return cartridge_serial_list;
        }

        public void insert_deck_position(Single position,int serial_number)
        {
            // UPDATE Users SET weight = 160, desiredWeight = 145 WHERE id = 1;
            string insert_deck_pos_sql = "UPDATE postech_tb.part_dispenser SET " +
                                         "part_dispenser.deck_position = " + position +
                                         " WHERE part_dispenser.cartridge_serial_number = '" + serial_number + "'";
            MySqlCommand cmd = new MySqlCommand(insert_deck_pos_sql, dbConnection);
            cmd.ExecuteNonQuery();

        }

        public int get_deck_id(string serial_number)
            // Serial Number와 일치하는 Deck id 가져오기
        {
            string[] row1 = new string[3];
            DataTable dataTable = new DataTable();
            string select_all_sql = "SELECT * FROM postech_tb.part_dispenser" +
                                    " WHERE part_dispenser.cartridge_serial_number = " + "'" + serial_number + "'";
            MySqlCommand cmd = new MySqlCommand(select_all_sql, dbConnection);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

            da.Fill(dataTable);
       
            DataRow dr = dataTable.Rows[0];
            Console.WriteLine("{0}, {1}, {2}", dr[0], dr[1], dr[2]);
            Console.WriteLine(dr[1].GetType());

            return (int)dr[0];
          


        }




    }
}
