using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Station_Ver2._0
{
    public class MoveRFIDReader
    {
        string[] serial_number = { "", "" };

        public void MoveJoint(int on)
        {
            if(on == 1)
            {
                RFID_MoveUp();
            }
            else
            {
                RFID_MoveDown();
            }
        }
        public void RFID_MoveUp()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 9;
            stopper_up_In.Value = 1;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }

        public void RFID_MoveDown()
        {
            MXP.MXP_WRITEDIGITALOUTPUT_IN stopper_up_In;
            stopper_up_In.Output.SourceNo = 1;
            stopper_up_In.OutputNumber = 9;
            stopper_up_In.Value = 0;
            stopper_up_In.ExecutionMode = 0;
            stopper_up_In.Execute = 0;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
            stopper_up_In.Execute = 1;
            MXP.MXP_WriteDigitalOutputCmd(240, ref stopper_up_In);
        }
    }
}
