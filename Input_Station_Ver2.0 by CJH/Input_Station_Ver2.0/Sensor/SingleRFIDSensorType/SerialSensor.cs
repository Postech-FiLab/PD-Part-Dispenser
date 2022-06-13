using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Station_Ver2._0.Sensor
{
    public class SerialSensor
    {

        public string Serial_Number;

        public SerialPort Make_Port(string portname)
        {

            SerialPort Port = new SerialPort();
            Port.PortName = portname;
            Port.BaudRate = 115200;
            Port.DataBits = 8;
            Port.Parity = Parity.None;
            Port.Handshake = Handshake.None;
            Port.StopBits = StopBits.One;
            Port.Encoding = Encoding.UTF8;
            Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
            return Port;
        }



        public void Serial_Connect_Scan(SerialPort Port)
        {
            try
            {
                Port.Open();
                Console.WriteLine("시리얼 연결 성공");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 포트에 데이터가 들어오면 자동으로 아래 이벤트가 작동이 됨.
        /// 데이터가 들어오는 Port가 object 타입으로 warping 되서 들어옴.
        /// 그래서 쓰기 편하기 위해 object 타입을 다시 serialport로 타입 변환 하여 소문자 sender로 만듬
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e) // 보조 스레드
        {

            int intRecSize = 0;
            SerialPort port = (SerialPort)sender;
            try
            {
                intRecSize = port.BytesToRead;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            byte[] buff = new byte[intRecSize];

            if (intRecSize != 0)
            {
                port.Read(buff, 0, intRecSize);
            }

            if (intRecSize == 13)
            {
                try
                {

                    for (int iTemp = 7; iTemp < 12; iTemp++)
                    {
                        Serial_Number += buff[iTemp].ToString("X2");
                    }
                    // SerialNumber public변수에 Serial_num 입력

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else if (intRecSize != 13)
            {

                Serial_Number = "EMPTY";
                //Console.WriteLine("감지 불가");

            }


        }

        public void Serial_Read_Send(object Port_Obj)
        {
            SerialPort Port = new SerialPort();
            Port = (SerialPort)Port_Obj;
            string tbSendMessage = "02 16 00 00 16 03";
            byte[] byteSendData = new byte[200];
            int iSendCount = 0;
            try
            {

                foreach (string s in tbSendMessage.Split(' '))
                {
                    if (s != null && s != "")
                    {
                        byteSendData[iSendCount++] = Convert.ToByte(s, 16);
                    }
                }
                Port.Write(byteSendData, 0, iSendCount);
                Console.WriteLine("Port 명령 보냈습니다.");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
