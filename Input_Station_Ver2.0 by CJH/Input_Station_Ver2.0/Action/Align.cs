using Input_Station_Ver2._0.DeviceType.SingleJoint;
using Input_Station_Ver2._0.Sensor;
using System;
using System.Threading;


namespace Input_Station_Ver2._0
{
    public class Align
    {
        public ProximitySensor proxi_sensor;
        public int[] input_numbers = new int[] { 11, 15, 13 };
        public int[] enable = new int[] { 1, 1, 1 };
        public Motor motor;
        public void start(Motor motor)
        {
            while (true)
            {
                string[] result = proxi_sensor.Read(input_numbers, enable);
                if (result[0].Equals("1") && result[1].Equals("1") && result[2].Equals("1")) // 처음에 이미 정렬이 되어 있는 경우 
                {
                    Console.WriteLine(result[0] + "," + result[1] + "," + result[2]);
                    Console.WriteLine("정렬 끝");
                    Single home_position = motor.Now_Actual_Position();
                    Console.WriteLine("Home Position은 " + home_position + " 로 설정이 되었습니다");
                    break;
                }
                // 정렬이 되어 있지 않은 경우
                motor.position_calibration = 0.5f;
                motor.Rel_PMove(1);
                Console.WriteLine(result[0] + "," + result[1] + "," + result[2]);
                Thread.Sleep(1000);
            }
        }    
    } 
}

