using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alignment
{
    public class Read_Optic_Sensor
    {

        private Byte[] result;
        private string[] str_result = new string[3] { "0", "0", "0" } ;
        private int source_number = 1;
        private int[] input_numbers;
        private int[] enable;

        public Byte[] Result
        {
            get { return this.result; }
            set { this.result = value; }
        }

        public int[] Enable
        {
            get { return this.enable; }
            set { this.enable = value; }
        }

        public int[] InputNum
        {
            get { return this.input_numbers; }
            set { this.input_numbers = value; }
        }


        public string[] Read( int[] input_numbers, int[] enable)
        {
            for (int i = 0; i < input_numbers.Length; i++){
                MXP.MXP_READDIGITALINPUT_IN In;
                MXP.MXP_READDIGITALINPUT_OUT Out;
                Byte Result;
                In.Input.SourceNo = 1;
                In.InputNumber = input_numbers[i];
                //Console.WriteLine(input_numbers[i]);
                //Console.WriteLine(enable[i]);
                In.Enable = Convert.ToByte(enable[i]);
                MXP.MXP_ReadDigitalInput(ref In, out Out);
                Result = Out.Value;
                //Console.WriteLine(Result.ToString());
                str_result[i] = Result.ToString();
            }
            return str_result;
        }






    }
}
