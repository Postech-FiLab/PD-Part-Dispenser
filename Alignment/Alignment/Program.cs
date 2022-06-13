using Alignment.MXP_MOTOR;
using System;
using System.Threading;


namespace Alignment
{
    class Program
    {

        static void Main(string[] args)
        {

            float home_positon;

            Motor motor = new Motor();
            motor.connect();
            motor.Servo_Power(false); // false가 항상 모터 on임 

            
            Read_Optic_Sensor optic_sensor = new Read_Optic_Sensor();
            int[] input_numbers = new int[] { 11, 15, 13 } ;
            int[] enable = new int[] {1,1,1};
      
            while (true)
            {
                string[] result = optic_sensor.Read(input_numbers, enable);
                if (result[0].Equals("1") && result[1].Equals("1") && result[2].Equals("1")) // 처음에 이미 정렬이 되어 있는 경우 
                {
                    
                    Console.WriteLine(result[0] + "," + result[1] + "," + result[2]);
                    Console.WriteLine("정렬 끝");
                    Single home_position = motor.Now_Actual_Position();
                    Console.WriteLine("Home Position은 " + home_position + " 로 설정이 되었습니다");
                    Console.ReadKey();
                    break;
                }
                // 정렬이 되어 있지 않은 경우

                motor.position_calibration = 0.5f;
                motor.Rel_PMove(1);
                Console.WriteLine(result[0] + "," + result[1] + "," + result[2]);
                Thread.Sleep(1000);

            }

           
            // 약간 비틀려져 있는 경우면 어떻게 할까? 
        }

       


    }
}
