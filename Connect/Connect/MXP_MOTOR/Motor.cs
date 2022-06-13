using Input_Station_Ver2._0;
using System;

using System.Threading;

namespace Connect.MXP_MOTOR
{
    public class Motor
    {
        /// <summary>
        /// 모션 블락 인덱스 번호 
        /// 아직 무엇인지 모름.. 
        /// </summary>
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
                Console.WriteLine("Mxp Init Kernel fail");
            }
            else
            {
                if (MXP.MXP_SystemRun() == 0)
                {
                    Console.WriteLine("MXP Sysetm Run and Connected");
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
                    Console.WriteLine("Operated");
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
                    Console.WriteLine("Servo Power on");
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


        
        public Single align_position = 3375.415f;
        public void Align()
        {
            // 현재 위치에서 Position에 설정된 절대 위치로 이동합니다.
            // 
            MXP.MXP_MOVEABSOLUTE_IN Po_Abs_In = new MXP.MXP_MOVEABSOLUTE_IN { } ;
            Po_Abs_In.Axis.AxisNo = 0;
            Po_Abs_In.ContinuousUpdate = 0;
            Po_Abs_In.Position = align_position;  
            Po_Abs_In.Velocity = 80;
            Po_Abs_In.Acceleration = (float)15;
            Po_Abs_In.Deceleration = (float)15;
            Po_Abs_In.Jerk = 10000;
            Po_Abs_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Po_Abs_In.Execute = 0;
            Int32 Status = MXP.MXP_MoveAbsoluteCmd(110, ref Po_Abs_In);
            Thread.Sleep(1000);
            Po_Abs_In.Execute = 1;
            Status = MXP.MXP_MoveAbsoluteCmd(110, ref Po_Abs_In);
            Thread.Sleep(8000);
            
            // 80속도, 가속도 15일때 330도 돌려면 한 8초 정도 줘야함.( 딱 적당 ) 
        }

        /// 30도 회전을 위한 position (distance) 값:    46.91666667
        float position_calibration = 46.875f;
        /// <summary>
        /// Rel_PMove()
        /// Positive 방향으로 30도 회전하는 함수 
        /// </summary>
        public void Rel_PMove(int index) 
        {
            MXP.MXP_MOVERELATIVE_IN Po_Rel_In;
            Po_Rel_In.Axis.AxisNo = 0;
            Po_Rel_In.ContinuousUpdate = 0;
            Po_Rel_In.Distance = position_calibration * index; //= 30;
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
        }
        /// <summary>
        /// Rel_NMove() 
        /// Negative 방향으로 30도 회전
        /// </summary>
        public void Rel_NMove(int index) 
        {
            MXP.MXP_MOVERELATIVE_IN Ne_Rel_In;
            Ne_Rel_In.Axis.AxisNo = 0;
            Ne_Rel_In.ContinuousUpdate = 0;
            Ne_Rel_In.Distance = -1 * position_calibration * index;
            Ne_Rel_In.Velocity = 35;
            Ne_Rel_In.Acceleration = (float)10;
            Ne_Rel_In.Deceleration = (float)10;
            Ne_Rel_In.Jerk = 10000;
            Ne_Rel_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Ne_Rel_In.Execute = 0;
            MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(1000);
            Ne_Rel_In.Execute = 1;
            MXP.MXP_MoveRelativeCmd(110, ref Ne_Rel_In);
            Thread.Sleep(500);
        }
        /// <summary>
        /// Fast Move 는 Pallet를 Out 시킬 때 빠르게 카트리지를 찾아가기 위해서 
        /// 속도를 빠르게 한 함수임. Rel_NMove 함수나 Rel_PMove 함수는 속도가 느림. 그래서 scan 할때 적절함.
        /// </summary>
        /// <param name="index"></param>
        public void Rel_Fast_Move(int index)
        {
            MXP.MXP_MOVERELATIVE_IN Po_Rel_In;
            Po_Rel_In.Axis.AxisNo = 0;
            Po_Rel_In.ContinuousUpdate = 0;
            Po_Rel_In.Distance = position_calibration * index;
            Po_Rel_In.Velocity = 80;
            Po_Rel_In.Acceleration = (float)15;
            Po_Rel_In.Deceleration = (float)15;
            Po_Rel_In.Jerk = 10000;
            Po_Rel_In.BufferMode = MXP.MXP_BUFFERMODE_ENUM.MXP_BUFFERED;
            Po_Rel_In.Execute = 0;
            MXP.MXP_MoveRelativeCmd(110, ref Po_Rel_In);
            Thread.Sleep(1000);
            Po_Rel_In.Execute = 1;
            MXP.MXP_MoveRelativeCmd(110, ref Po_Rel_In);
            Thread.Sleep(13500);
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
         
            Console.WriteLine(x);
            return x;

        }
        public void Exit()
        {
            Int32 Status = 0;
            MXP.MXP_GetKernelStatus(out Status);
            if(Status >= MXP.MXP_SysStatus.Run)
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
