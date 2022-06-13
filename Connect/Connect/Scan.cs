
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Connect.RFID;
using Input_Station_Ver2._0;
using System.IO.Ports;
using Connect.MXP_MOTOR;

namespace Connect
{
    public class Scan
    {
        public string[] cartridge_name_list;
        public void start(Motor motor)
        {
            cartridge_name_list = new string[12];

            NFC nfc = new NFC();
            nfc.NFC_MoveUp();
            Thread.Sleep(5000); 

            Serial serial = new Serial(); // 직접 만든  Serial Class
            SerialPort Port = serial.Make_Port("COM3"); // Serial Port는 IO.Ports에서 가져온 클래스, Com3는 작업 관리자에서 포트 이름 확인 가능 
            serial.Serial_Connect_Scan(Port); // 시리얼 연결을 합니다.
             
            for (int cartridge_Count = 0; cartridge_Count < 12; cartridge_Count++)
            {
                
                Console.WriteLine("------------------");
                Console.WriteLine(cartridge_Count+1);
                serial.Serial_Read_Send(Port);
                Thread.Sleep(1000);
                // Read_Send를 실행하게 되면 RFID 리더기한테 읽어라고 명령을 보내고, 값을 다시 받아오게 됩니다.
                // 위 시간동안 time.sleep() 을 주지 않게 되면 리더기가 값을 읽지 못하는 경우가 발생을 합니다.
                // 못 읽는 경우는 어떻게 할까?
                while (serial.Serial_Number.Equals("EMPTY")){
                    serial.Serial_Read_Send(Port);
                    if (!serial.Serial_Number.Equals("EMPTY"))
                    {
                        break;
                    }
                }
            
                Console.WriteLine(serial.Serial_Number);
                cartridge_name_list[cartridge_Count] = serial.Serial_Number;
                serial.Serial_Number = "";
                Console.WriteLine("------------------");
                
                


                motor.Rel_NMove(1); // 30도만큼 움직일 꺼임.
                Thread.Sleep(3500); // 30도 만큼 움직이는 동안 기다려야함.
            }
            nfc.NFC_MoveDown();
        } 

    }
}
