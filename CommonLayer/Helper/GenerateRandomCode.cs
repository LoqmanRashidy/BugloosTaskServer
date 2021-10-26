using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Helper
{
    public class GenerateRandomCode
    {
        //Generate RandomNo
        public static int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
    }
}
