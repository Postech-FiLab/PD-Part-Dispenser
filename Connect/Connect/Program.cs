
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DB;
using Input_Station_Ver2._0;
using Connect.MXP_MOTOR;
using Connect.RFID;

/*
* Station Controller : PC 
* Motor : Ether Cat
* Part 인식 : RFID
* Cylincer : Digital Input Output

Part Dispenser와 연결 하는 함수 
1. DB 연결 ( TCP/ IP )
2. MXP 연결 ( EtherCat )
3.  

*/
namespace Connect
{
    class Program
    {


        static void Main(string[] args)
        {

            //Connect_DB db = new Connect_DB();
            //db.connect("192.168.50.2", "3306", "root", "filab1020");
            //Console.WriteLine(db.select_All());

            
            Motor motor = new Motor();
            motor.connect();
            
            motor.Servo_Power(false);
            //motor.MXP_Home_Example();
            //Single x = motor.Now_Actual_Position();
            //motor.align_position = -x;
            //motor.Align();
            //MXP.MXP_READDIGITALINPUT_IN In;
            //MXP.MXP_READDIGITALINPUT_OUT Out;
            //Byte Result;
            //In.Input.SourceNo = 1;
            //In.InputNumber = 13;
            //In.Enable = 1;
            //MXP.MXP_ReadDigitalInput(ref In, out Out);
            //Result = Out.Value;
            //Console.WriteLine(Result.ToString());




            NFC nfc = new NFC();
            nfc.NFC_MoveUp();

            /*스캔*/

            //Scan scan = new Scan();

            //scan.start(motor);


            //Console.WriteLine(scan.cartridge_name_list);

            //Pallet pallet = new Pallet();
            //pallet.Out();

            Console.ReadKey();

           motor.Servo_Power(true);// 모터 서버 오프

           motor.Exit(); // 커널 접속 종료 

           // Thread.Sleep(2000);



        }
    }
}
