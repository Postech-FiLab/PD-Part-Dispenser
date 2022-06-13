using Input_Station_Ver2._0.DeviceType.SingleJoint;
using System;
using System.Threading;

namespace Input_Station_Ver2._0
{
    public class DispensePallet
    {
        Pneumatic_MovePalletPusher1 pusher1 = new Pneumatic_MovePalletPusher1();
        Pneumatic_MovePalletPusher2 pusher2 = new Pneumatic_MovePalletPusher2();
        Pneumatic_MovePalletUpper upper = new Pneumatic_MovePalletUpper();

        public void start()
        {
            Thread.Sleep(1000);
            pusher1.MoveJoint(1);
            Thread.Sleep(1000);
            pusher2.MoveJoint(1);
            Thread.Sleep(1000);
            pusher2.MoveJoint(0);
            Thread.Sleep(1000);
            pusher1.MoveJoint(0);
            Thread.Sleep(1000);
            

        }
   


        static private void Cyl_Stopper_Up()
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

    }

}
