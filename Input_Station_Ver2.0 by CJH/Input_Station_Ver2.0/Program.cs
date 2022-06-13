using Input_Station_Ver2._0.DeviceType.SingleJoint;
using Input_Station_Ver2._0.Sensor;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * -----------------------------------------------
 FILAB - Part Dispenser Program by C.J.H 2022
 단계 1. 모터 연결, 모터 On
 단계 2. 데이터 베이스 연결 
    단계 2.1 데이터 베이스에 있는 각 덱의 Serial Number를 들고옴. 
    단게 2.2 고유 Serial Number와 DECK ID를 들고옴.
    단계 2.3 데이터 베이스에는 총 12개의 Deck id 와 Serial Number가 1:1 매칭이 되어 있음. 
 단계 3. Align
    단계 3.1 Scan을 하기전에 RFID 리더기가 Serial Number를 잘 읽을 수 있도록 정렬 시켜야 함.
    단계 3.2 Pallet를 다음 스테이션으로 내보내기 위해서도 정렬을 잘 해야 함.
 단계 4. Scan
    단계 4.1 정렬이 완료된 시점에서 RFID가 Seral Numbe를 읽기 시작함.
    단계 4.2 이 때 처음 읽은 Deck_id를 home_deck_id라고 부르고 그리고 이 반대편에 있는 deck id를 out_deck_id라고 부름.
    단계 4.3 30도 회전하면서 읽음.
    단계 4.4 회전하면서 RFID가 Serial Number를 읽을 것인데, 읽지 못하면 계속 기다림(읽을 때 까지) 
        --> 읽지 못하면 직접 상태 체크.( RFID Sticker가 제대로 중앙에 배치 안되 있음 .) 
        --> 읽더라도 RFID 리더기가 읽는 범위가 옆 Deck에 있는 것도 읽을 수가 있음. ( 소스코드에는 이 부분이 처리가 되어 있음.)
    단계 4.5 
 단계 5. Pallet Out ( Stoppter, Pallet를 올리는 Device, Pallet를 미는 Device 1 , Pallet를 미는 또 다른 Device 2가 존재. )
    Stopper의 역할이 왜 중요한지는 모르겠음. ( Pallet를 내보내기전에 Stopper가 Part Dispenser가 흔들(또는 회전) 하지 않도록 꽉 잡아 놓음 )
    주의 사항 - Pallet가 Out 하기 전에 꼭 회전을 다 돌아야 함. Thread Sleep에 time을 여유를 많이 주는게 좋음. 그렇지 않으면 회전하는 동안 Pallet가 나가게 되면 PD 고장.ㅎ
    단계 5.1 out 11 이라는 명령을 받으면 11번 deck에 있는 pallet를 내보냄.
    단계 5.2 내보내게 되면 out_deck_id = 11 , home_deck_id= 5번이 됨

- 비고 - 
아직 DB관련해서 몇번 덱에 몇개가 들어있고 어떤 제품이 들어있는지는 고려 안됨.
그리고 Pallet Out을 하면 db에 1개가 빠졌다는 것도 고려 안됨.
----------------------------------------------------------
     */
namespace Input_Station_Ver2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "INPUT STATION VER 2.0 BY CJH";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("***********************");
            Console.WriteLine("Welcome!! INPUT STATION");
            Console.WriteLine("***********************");
            Console.WriteLine("");

            /* 모터 켜기 */
            Motor motor = new Motor();
            motor.connect();
            motor.Servo_Power(false);
            Console.WriteLine(" * Mxp Motor on");
            Console.WriteLine("");

        
            /* 데이터 베이스 연결 하기 */
            string[] cartridge_serial_numbers = new string[12];
            DataBase db = new DataBase();
            db.connect();
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            cartridge_serial_numbers = db.Select_all();
            Console.WriteLine("");

            /* Scan을 하기전 Align을 해야함. */
            ProximitySensor proxi_sensor = new ProximitySensor();
            Align align = new Align();
            align.proxi_sensor = proxi_sensor;
            align.motor = motor;
            align.start(motor);


            /* Scan을 하기전에 RFID READER기를 올려야 함*/
            MoveRFIDReader RFID = new MoveRFIDReader();
            RFID.MoveJoint(1);
            Thread.Sleep(2000);


            /* Scan 시작 */


            /* RFID로 Serial 읽기*/
            SerialSensor serial = new SerialSensor(); // 직접 만든  Serial Class
            SerialPort port = serial.Make_Port("COM3"); // Serial Port는 IO.Ports에서 가져온 클래스, Com3는 작업 관리자에서 포트 이름 확인 가능 
            serial.Serial_Connect_Scan(port); // 시리얼 연결을 합니다.

            /* Scan */
            Scan scan = new Scan();
            scan.start(serial, port, motor, db, cartridge_serial_numbers);
            int home_deck_id = scan.home_deck_id;
            int out_deck_id = scan.out_deck_id;

            /* Scan이 다 끝나면 RFID READER기를 내려야 함 */
            MoveRFIDReader nfc = new MoveRFIDReader();
            RFID.MoveJoint(0);
            Thread.Sleep(2000);

            /* */
            DispensePallet dispense_pallet = new DispensePallet();
            

            while (true)
            {
                /* 나는 Deck ID 몇번에 팔레트를 보내자. */
                Console.WriteLine("-------------------------");
                Console.WriteLine("현재 홈의 위치 : " + home_deck_id);
                Console.WriteLine("현재 아웃의 위치 : " + out_deck_id);
                Console.WriteLine("Command EXAMPLE : out 11");
                Console.WriteLine("-------------------------");
                Console.Write("Command : ");

                string cmd = Console.ReadLine();
                Console.WriteLine(cmd.ToString());
                try
                {

                    string[] parameters = cmd.Split(' ');

                    if (parameters[0] == "q")
                    {
                        break;
                    }
                    int deck_id = Convert.ToInt32(parameters[1]);

                    //Double deck_pos = db.get_deck_position(deck_id);
                    //Console.WriteLine(deck_pos);
                    int move_index = out_deck_id - deck_id;
                    Console.WriteLine("move index :" + Math.Abs(move_index));

                    move_index = motor.MoveJoint(move_index);


                    Thread.Sleep(Math.Abs(move_index) * 3000);


                    dispense_pallet.start();

                    
                    out_deck_id = deck_id;
                    if (out_deck_id <= 6)
                    {
                        home_deck_id = out_deck_id + 6;
                    }
                    else
                    {
                        home_deck_id = out_deck_id - 6;
                    }

                    //motor.move_pos((float)deck_pos);



                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("*****Warning******");
                    Console.WriteLine($"error! ({ex.Message})");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

                

            
       


        }
        
    }
}
