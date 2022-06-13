using Scan.MXP_MOTOR;
using Scan.RFID;
using Scan.DB;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;


namespace Scan
{
    class Program
    {
        static void Main(string[] args)
        {
            /*모터 켜기 */
            Motor motor = new Motor();
            motor.connect();
            motor.Servo_Power(false);


            /* 정렬하기 */
            Alignment alignment = new Alignment();
            Single home_position = alignment.start_alignment(motor);
            motor.home_position = home_position; // 모터 객체에 home position 등록 

            string[] cartridge_serial_numbers = new string[12];

            /* DB 연결하기 */
            /* DB 에서 카트리지 시리얼 넘버를 가져와야함 */
            DataBase db = new DataBase();
            db.connect();
            cartridge_serial_numbers = db.Select_all();

            /* 각 DECK ID 별로 위치를 기억해두어야 함. */
            List<Single> deck_position = new List<Single>();
         
            /* RFID 읽기 + Scan 시작 */

           
            NFC nfc = new NFC();
            nfc.NFC_MoveUp();
            Thread.Sleep(2000);

            Serial serial = new Serial(); // 직접 만든  Serial Class
            SerialPort Port = serial.Make_Port("COM3"); // Serial Port는 IO.Ports에서 가져온 클래스, Com3는 작업 관리자에서 포트 이름 확인 가능 
            serial.Serial_Connect_Scan(Port); // 시리얼 연결을 합니다.


            for (int cartridge_Count = 0; cartridge_Count < 12; cartridge_Count++)
            {

                Console.WriteLine("------------------");
                Console.WriteLine(cartridge_Count + 1);
                serial.Serial_Read_Send(Port);
                Thread.Sleep(3000);
                // Read_Send를 실행하게 되면 RFID 리더기한테 읽어라고 명령을 보내고, 값을 다시 받아오게 됩니다.
                // 위 시간동안 time.sleep() 을 주지 않게 되면 리더기가 값을 읽지 못하는 경우가 발생을 합니다.
                // 그리고 Sleep을 적게 줄 경우 리더기가 두 개의 값을 보낼 수도 있음. ( RFID 리더기가 2개의 카트리지의 시리얼 넘버를 읽음. )
                // 못 읽는 경우는 어떻게 할까? --> EMPTY라는 상황 인건데 --> Tag 상태를 보고 태그가 잘 붙어 있는지 확인하세요.
                while (serial.Serial_Number.Equals("EMPTY")) // 계속 Empty라면 읽을때 까지 계속 읽습니다.
                {
                    serial.Serial_Read_Send(Port);
                    if (!serial.Serial_Number.Equals("EMPTY"))
                    {
                        break;
                    }
                }
                Console.WriteLine(serial.Serial_Number);
                // EMPTY 문자열과 Serial Number 같이 한 줄에 출력되는 경우가 있음. --> 정확한 이유는 모름.
                if (serial.Serial_Number.Contains("EMPTY"))
                {
                    Console.WriteLine("EMPTY 값이 들어왔습니다. 태그 상태 혹은 회전 속도가 너무 빠른건지 체크 해주세요.");
                    serial.Serial_Number = serial.Serial_Number.Replace("EMPTY","");
                }
                // 시리얼 번호가 전 카트리지의 시리얼 넘버가 같이 보여지는 경우가 있습니다. //EA9E9DB503 CAA69DB5EB
                if (serial.Serial_Number.Length > 10)
                {
                    serial.Serial_Number = serial.Serial_Number.Substring(10, 10);
                }
                for(int j = 0; j < 12; j ++)
                {
                    if( cartridge_serial_numbers[j] == serial.Serial_Number) // DB에 저장된 카트리지 시리얼 넘버와 읽은 시리얼 넘버와 같을 경우
                    {
                        Console.WriteLine(cartridge_serial_numbers[j]);
                        Console.WriteLine(serial.Serial_Number);
                        // 우리는 Part를 내보낼 때를 생각해야 하기 때문에 이때의 position은 
                        // 예를들어 deck id = 1을 scan을 했다면 팔레트 보내는 쪽에 DEC의 위치는 7의 위치임 
                        int serial_num = db.get_deck_id(serial.Serial_Number);
                        db.insert_deck_position(motor.Now_Actual_Position(), serial_num);
                    }
                }
                serial.Serial_Number = "";
                Console.WriteLine("------------------");
                //Motion_Function.MXP_MC_Reset();
                motor.position_calibration = 46.875f;
                motor.Rel_NMove(1); // 30도만큼 움직일 꺼임.
                Thread.Sleep(4000); // 30도 만큼 움직이는 동안 기다려야함.
            }
            
            nfc.NFC_MoveDown();

            for(int i = 0; i < 12;i++)
            {
                Console.WriteLine((i+1) + "번째 카트리지 시리얼 번호 : " + cartridge_serial_numbers[i]);
            }


            Console.WriteLine("*************************************");
            Console.WriteLine("**** 아무키나 누르면 종료가 됩니다.****");
            Console.ReadKey();

            motor.Servo_Power(true);// 모터 서버 오프
            motor.Exit(); // 커널 접속 종료 
        }   
    }
}
