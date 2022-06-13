using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Station_Ver2._0.DeviceType.SingleJoint
{
    public class Pneumatic_MovePalletPusher2
    {
        public void MoveJoint(int on)
        {
            if (on == 1)
            {
                Pallete_Out2();
            }
            else
            {
                Pallete_Out4();
            }
        }
        static private void Pallete_Out2()
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
    }
}
