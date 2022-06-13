using System;
using System.Threading;

namespace Input_Station_Ver2._0.DeviceType.SingleJoint
{
    public class Motor
    {
        public enum MXP_MotionBlockIndex
        {
            mcPower = 0,
            mcReset,
            mcStop,
            mcHalt,
            mcHome,
            mcMoveAbsolute,
            mcMoveRelative,
            mcMoveVelocity,
            mcMoveLinearAbsolute,
            mcMoveLinearRelative,
            mcMoveCircularAbsolute,
            mcMoveCircularRelative,
            mcGroupStop,
            mcWriteParameter,
            mcWriteBoolParameter,
            mcET_ReadParameter,
            mcET_WriteParameter,
            mcWriteOutputs,
            mcWriteDigitalOutputs,
            mcGearIn,
            mcGearOut,
            mcGearInPos,
            mcCamIn,
            mcCamOut,
            mcCamTableSelect,
            mcSetTouchProbe,
            mcDirectTorqueControl
        }

        /// <summary>
        /// MXP Connect 하는 함수.
        /// Kernel 초기화 --> MXP_InitKernel()
        /// 커널 동작 시킴 --> MXP_SystemRun()
        /// 프로그램을 시작하기전 커널 초기화, 커널 동작순으로 무조건 선행이 되어야 한다.
        /// </summary>
        public void connect()
        {
            UInt32 Status = 0;
            UInt32 Connection_Check = 0;
            if (MXP.MXP_InitKernel(ref Status) != MXP.MXP_ret.RET_NO_ERROR)
            {
                Console.WriteLine(" * Mxp Init Kernel fail");
            }
            else
            {
                if (MXP.MXP_SystemRun() == 0)
                {
                    Console.WriteLine(" * MXP Sysetm Run and Connected");
                }

            }

            // STATE-OP( 통신 준비 완료 ) 상태까지는 수초의 시간이 소요되므로 
            // 네트워크 정상상태를 체크 하기 위해서는 아래와 같이 응용이 필요합니다.
            // While문 돌면서 State 값이 NET_STATE_OP 값과 같아짐.
            while (Connection_Check == 0)
            {
                UInt32 State = 0;
                UInt32 SlaveNo = 0;
                MXP.MXP_IsSlaveOnline(SlaveNo, out State); // 개별 slave의 네트워크 상태를 반환함.
                if (State == (UInt32)MXP.MXP_ONLINESTATE_ENUM.NET_STATE_OP)
                {
                    Connection_Check = 1;
                    Console.WriteLine(" * Operated 상태가 되었습니다.");
                    break;

                }

            }
        }
        /// <summary>
        /// MaxAxis 왜 필요한지 아직 모름.
        /// </summary>
        static UInt32 MaxAxis = 0;
        public static UInt32 IndexCal(UInt32 Index)
        {
            return Index * MaxAxis;
        }

        /// < SUMMARY >
        ///  * Servo on/off 상태를 제어하는 함수.
        ///  * 인자 servoonoff : false --> Servo on 
        ///  
        /// </SUMMARY>
        /// 
        public void Servo_Power(bool servoonoff)

        {
            MXP.MXP_POWER_IN Pow_In = new MXP.MXP_POWER_IN { };
            if (!servoonoff)
            {
                Pow_In.Axis.AxisNo = 0;
                Pow_In.Enable = 1;
                Int32 Ret = MXP.MXP_PowerCmd(0, ref Pow_In);
                if (Ret == MXP.MXP_ret.RET_NO_ERROR)
                {
                    servoonoff = true;
                    Console.WriteLine(" * Servo Power on");
                }
            }
            else
            {
                Pow_In.Axis.AxisNo = 0;
                Pow_In.Enable = 0;
                Int32 Ret = MXP.MXP_PowerCmd(0, ref Pow_In);
                if (Ret == MXP.MXP_ret.RET_NO_ERROR)
                {
                    servoonoff = false;
                }

            }
        }



        public Single home_position = 0f;
        public int MoveJoint(int move_index)
        {
            if (move_index >= 0)
            {
                if (move_index >= 6) // 예를 들어 기존 out_deck-id 위치 11 -> 4로 가고싶은 경우, move index =  7 
                {
                    move_index = 12 - move_index;
                    Rel_NMove_30(move_index);
                }
                else // 예를 들어 기존 out_deck-id 위치 11 -> 6 로 가고싶은 경우, move index =  5
                {
                    Rel_PMove_30(move_index);
                }

            }
            else // 기존 out-deck-id 위치가 6 -> 11로 가고 싶은 경우 , move index = -5 , 
            {
                if (move_index <= -6)  // 기존 out-deck-id 위치가 1 -> 11로 가고 싶은 경우 , move index = -10 , 
                {
                    move_index = 12 + move_index;
                    Rel_PMove_30(move_index);
                }
                else // 기존 out-deck-id 위치가 1 -> 4로 가고 싶은 경우 , move index = -3 , 
                {

                    Rel_NMove_30(-1 * move_index);
                }
            }
            return move_index;
        }

        /// 30도 회전을 위한 position (distance) 값:    46.91666667
        /// <summary>
        /// Rel_PMove()
        /// Positive 방향으로 30도 회전하는 함수 
        /// </summary>
        public int Rel_PMove_30(int index)
        {
            MXP.MXP_MOVERELATIVE_IN Po_Rel_In;
            Po_Rel_In.Axis.AxisNo = 0;
            Po_Rel_In.ContinuousUpdate = 0;
            Po_Rel_In.Distance = 46.875f * index; //= 30;
            Po_Rel_In.Velocity = 35;
            Po_Rel_In.Acceleration = (float)10;
            Po_Rel_In.Deceleration = (float)10;
            Po_Rel_In.Jerk = 10000;
            Po_Rel_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Po_Rel_In.Execute = 0;
            //Thread.Sleep(500);
            Int32 Status = MXP.MXP_MoveRelativeCmd(110, ref Po_Rel_In);
            Thread.Sleep(1000);
            Po_Rel_In.Execute = 1;
            Status = MXP.MXP_MoveRelativeCmd(110, ref Po_Rel_In);
            Thread.Sleep(500);
            return Status;
        }
        public int Rel_NMove_30(int index)
        {
            MXP.MXP_MOVERELATIVE_IN Ne_Rel_In;
            Ne_Rel_In.Axis.AxisNo = 0;
            Ne_Rel_In.ContinuousUpdate = 0;

            Ne_Rel_In.Distance = -1 * index * 46.875f;
            Ne_Rel_In.Velocity = 35;
            Ne_Rel_In.Acceleration = (float)10;
            Ne_Rel_In.Deceleration = (float)10;
            Ne_Rel_In.Jerk = 10000;
            Ne_Rel_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Ne_Rel_In.Execute = 0;
            MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(1000);
            Ne_Rel_In.Execute = 1;
            int result = MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(500);
            return result;

        }
        /// <summary>
        /// Rel_NMove() 
        /// Negative 방향으로 30도 회전
        /// </summary>
        /// 

        public float position_calibration = 46.875f;
        public int Rel_NMove(int index)
        {

            MXP.MXP_MOVERELATIVE_IN Ne_Rel_In;
            Ne_Rel_In.Axis.AxisNo = 0;
            Ne_Rel_In.ContinuousUpdate = 0;

            Ne_Rel_In.Distance = -1 * position_calibration;
            Ne_Rel_In.Velocity = 35;
            Ne_Rel_In.Acceleration = (float)10;
            Ne_Rel_In.Deceleration = (float)10;
            Ne_Rel_In.Jerk = 10000;
            Ne_Rel_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Ne_Rel_In.Execute = 0;
            MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(1000);
            Ne_Rel_In.Execute = 1;
            int result = MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(500);
            return result;
        }
        public int Rel_PMove(int index)
        {

            MXP.MXP_MOVERELATIVE_IN Ne_Rel_In;
            Ne_Rel_In.Axis.AxisNo = 0;
            Ne_Rel_In.ContinuousUpdate = 0;

            Ne_Rel_In.Distance = position_calibration;
            Ne_Rel_In.Velocity = 35;
            Ne_Rel_In.Acceleration = (float)10;
            Ne_Rel_In.Deceleration = (float)10;
            Ne_Rel_In.Jerk = 10000;
            Ne_Rel_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Ne_Rel_In.Execute = 0;
            MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(1000);
            Ne_Rel_In.Execute = 1;
            int result = MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(500);
            return result;
        }
        
       
        public void reset()
        // 축의 이상상태를 해제하고 해당 축의 Error Stop 상태를 StandStill 상태로 리셋한다.
        // 현재 Position이 초기화 되지 않는다.
        {
            MXP.MXP_RESET_OUT Out = new MXP.MXP_RESET_OUT { };
            MXP.MXP_MOVEABSOLUTE_IN x = new MXP.MXP_MOVEABSOLUTE_IN { };
            x.Execute = 0;
            UInt32 i = 0;
            Motion_Function.MXP_MC_Reset(i, IndexCal((UInt32)MXP_MotionBlockIndex.mcReset) + i, false, Out);

        }


        public Single Now_Actual_Position()
        // 명령된 축의 현재위치를 출력한다. 
        {

            Single x = Motion_Function.MXP_MC_ReadActualPosition((Int32)0);
            return x;

        }
        public void Exit()
        {
            Int32 Status = 0;
            MXP.MXP_GetKernelStatus(out Status);
            if (Status >= MXP.MXP_SysStatus.Run)
            {
                if (MXP.MXP_SystemStop() == 0)
                {
                    MXP.MXP_Destroy();
                }
            }
        }



        public void MXP_Home_Example()
        {
            MXP.MXP_HOME_IN In;
            In.Axis.AxisNo = 0;
            In.BufferMode = 0; //Not Supported
            In.Position = 0; //Not Supported
            In.Execute = 0;
            MXP.MXP_HomeCmd(20, ref In);
            In.Execute = 1;
            MXP.MXP_HomeCmd(20, ref In);
        }

        
    }
}
