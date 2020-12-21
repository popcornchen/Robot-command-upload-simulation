using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace demo
{
    class test
    {
        public static string constr = "server=192.168.3.6; user=root; database=test; port=3306; pwd=angel070711";
        public int tag;

        /*-----------------测试数据库连接函数--------------------*/
        public void conDB()
        {
            MySqlConnection con = new MySqlConnection(constr);
            try
            {
                con.Open();
                Console.WriteLine("connection successful");
                con.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("connection error");
            }
        }

        /*-----------------输入路径需求--------------------*/
        public string[] request()
        {
            string[] route_split;
            while (true)
            {
                Console.WriteLine("输入路径需求，'-'隔开：\n");
                string route = Console.ReadLine();

                //检查路径合法性(实际路径合法性检查更复杂，可限制输入)
                string[] condition = { "-" };
                route_split = route.Split(condition, StringSplitOptions.RemoveEmptyEntries);
                if (route_split.Length == 2)
                {
                    if (route_split[0] == route_split[1])
                    {
                        Console.WriteLine("起点与终点不能一致，重新输入\n");
                        continue;
                    }
                }
                else if (route_split.Length == 3)
                {
                    if (route_split[0] == route_split[1] | route_split[1] == route_split[2] | route_split[0] == route_split[2])
                    {
                        Console.WriteLine("无效过程点，重新输入\n");
                        continue;
                    }
                }
                break;                
            }
            return route_split;
        }

        /*-----------------数据库信息初始化--------------------*/
        public int db_initial()
        {
            MySqlConnection con = new MySqlConnection(constr);
            con.Open();
            string command_init = "Truncate table command";
            MySqlCommand delete = new MySqlCommand(command_init, con);
            delete.ExecuteNonQuery();  //初始化清空一下表，是否需要保留数据？
            
            //机器人初始化指令先输进去
            string start = "insert into command(Motion, Step) value('04', 1)";
            MySqlCommand insert1 = new MySqlCommand(start, con);
            insert1.ExecuteNonQuery();
            string login = "insert into command(Motion, Step) value('02 4C 03 4F', 2)";
            MySqlCommand insert2 = new MySqlCommand(login, con);
            insert2.ExecuteNonQuery();
            con.Close();
            return 0;
        }
        
        /*-----------------路径分析，串口指令上传--------------------*/
        public int db_upload(string[] data)
        {
            MySqlConnection con = new MySqlConnection(constr);
            con.Open();
            string command = " ";
            //路径两两校验，逐个写入
            for (int i = 0; i < data.Length-1; i++)
            {
                switch (data[i])
                {
                    case "1":
                        switch (data[i+1])
                        {
                            case "2":
                                command = "'02 47 0C 03 48'";
                                break;
                            case "3":
                                command = "'02 47 14 03 50'";
                                break;
                        }
                        break;
                    case "2":
                        switch (data[i+1])
                        {
                            case "1":
                                command = "'02 47 15 03 51'";
                                break;
                            case "3":
                                command = "'02 47 0B 03 4F'";
                                break;
                        }
                        break;
                    case "3":
                        switch (data[i+1])
                        {
                            case "1":
                                command = "'02 47 12 03 56'";
                                break;
                            case "2":
                                command = "'02 47 1B 03 5F'";
                                break;
                        }
                        break;
                }
                string content = "insert into command(Motion, Step) value(" + command + ", " + (i+3).ToString() + ")";
                MySqlCommand insert = new MySqlCommand(content, con);
                insert.ExecuteNonQuery();
            }
            string setflag = "Update robot_monitor set processed=0 where line=0";
            MySqlCommand changeflag = new MySqlCommand(setflag, con);
            changeflag.ExecuteNonQuery();
            Console.WriteLine("修改完成");
            con.Close();

            return 0;
        }
    }
}


namespace Robot_Command_Upload_Simulation
{
    class Program
    {
        static void Main(string[] args)
        {
            demo.test test_beta = new demo.test();
            test_beta.conDB();
            test_beta.db_initial();
            string[] input = test_beta.request();
            test_beta.db_upload(input);
            Console.ReadLine();
        }
    }
}
