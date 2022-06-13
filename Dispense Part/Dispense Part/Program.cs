using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispense_Part
{
    class Program
    {
        /*
         정렬이 되어 있다고 가정을 한 상태에서 팔레트만 내보내는 과정만 있음.
             */
        static void Main(string[] args)
        {

            Motor motor = new Motor();
            motor.connect();
            motor.Servo_Power(false); // false가 항상 모터 on임 

            Out();

        }
        /*
         Out 7 : Stopper 
         
         Pallet Fisrt Pusher ==> Pallet_Out1(),Pallet_Out5() ==> Pallet Number = 5
         Pallet Second Pusher ==> Pallet_Out2(),Pallet_Out4() == Pallet Number = 3
         Pallet Lift ==> Pallet_Out3(),Pallet_Out6() == Pallet Number = 3
         \
         Out 3 : 
             
             */
        static public void Out()

        {
            //Cyl_Stopper_Up();
            Thread.Sleep(1000);
            Pallete_Out1();
            Thread.Sleep(1000);
            Pallete_Out2();
            Thread.Sleep(1000);
            Pallete_Out3();
            Thread.Sleep(1000);
            Pallete_Out4();
            Thread.Sleep(1000);
            Pallete_Out5();
            Thread.Sleep(1000);
            Pallete_Out6();
            Thread.Sleep(1000);
            //Cyl_Stopper_Down();

        }
        static private void Pallete_Out1()
        {
            // What is OUT1 ?
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;

            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 5;
            stopper_up_In.Value = 1;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        static  private void Pallete_Out2()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 3;
            stopper_up_In.Value = 1;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        static private void Pallete_Out3()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 1;
            stopper_up_In.Value = 1;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        static private void Pallete_Out4()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 3;
            stopper_up_In.Value = 0;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        static private void Pallete_Out5()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 5;
            stopper_up_In.Value = 0;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        static private void Pallete_Out6()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 1;
            stopper_up_In.Value = 0;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        static  private void Cyl_Stopper_Up()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 7;
            stopper_up_In.Value = 1;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }
        static private void Cyl_Stopper_Down()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_down_In;
            stopper_down_In.Output.SourceNo = 1;
            stopper_down_In.OutputNumber = 7;
            stopper_down_In.Value = 0;
            stopper_down_In.ExecutionMode = 0;
            stopper_down_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_down_In);
            stopper_down_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_down_In);
        }
        static bool test = false;
        static private void ReadDigitalOutput_On()
        {
            MXP.MXP_READDIGITALOUTPUT_IN Read_In;
            MXP.MXP_READDIGITALOUTPUT_OUT Read_Out;
            byte Result;

            Read_In.Output.SourceNo = 1;
            Read_In.OutputNumber = 7;

            Read_In.Enable = 0;
            MXP.MXP_ReadDigitalOutput(ref Read_In, out Read_Out);

            Result = Read_Out.Value;
            if (Result == 0)
            {
                test = false;
            }
        }

        static private void ReadDigitalOutput_Off()
        {
            MXP.MXP_READDIGITALOUTPUT_IN Read_In;
            MXP.MXP_READDIGITALOUTPUT_OUT Read_Out;
            byte Result;

            Read_In.Output.SourceNo = 1;
            Read_In.OutputNumber = 7;

            Read_In.Enable = 1;
            MXP.MXP_ReadDigitalOutput(ref Read_In, out Read_Out);

            Result = Read_Out.Value;
            if (Result == 1)
            {
                test = true;
            }
        }

    }
}
