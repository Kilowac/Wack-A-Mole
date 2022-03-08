using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WackAMole {
    class Mole {
        public Bitmap[] animation = new Bitmap[41];
        //The faster one is 12 elements
        //public Bitmap[] animation = new Bitmap[12];
        public Mole(){
            animation[0] = new Bitmap(Properties.Resources.Hole);
            animation[1] = new Bitmap(Properties.Resources.Emerge);
            animation[2] = new Bitmap(Properties.Resources.Apex);
            animation[3] = new Bitmap(Properties.Resources.Peek);
            animation[4] = new Bitmap(Properties.Resources.Peek);
            animation[5] = new Bitmap(Properties.Resources.Peek);
            animation[6] = new Bitmap(Properties.Resources.Peek);
            animation[7] = new Bitmap(Properties.Resources.Peek);
            ///*
            animation[8] = new Bitmap(Properties.Resources.Peek);
            animation[9] = new Bitmap(Properties.Resources.Peek);
            animation[10] = new Bitmap(Properties.Resources.Peek);
            animation[11] = new Bitmap(Properties.Resources.Peek);
            animation[12] = new Bitmap(Properties.Resources.Peek);
            animation[13] = new Bitmap(Properties.Resources.Peek);
            animation[14] = new Bitmap(Properties.Resources.Peek);
            animation[15] = new Bitmap(Properties.Resources.Peek);
            animation[16] = new Bitmap(Properties.Resources.Peek);
            animation[17] = new Bitmap(Properties.Resources.Peek);
            animation[18] = new Bitmap(Properties.Resources.Peek);
            animation[19] = new Bitmap(Properties.Resources.Peek);
            animation[20] = new Bitmap(Properties.Resources.Peek);
            animation[21] = new Bitmap(Properties.Resources.Peek);
            animation[22] = new Bitmap(Properties.Resources.Peek);
            animation[23] = new Bitmap(Properties.Resources.Peek);
            animation[24] = new Bitmap(Properties.Resources.Peek);
            animation[25] = new Bitmap(Properties.Resources.Peek);
            animation[26] = new Bitmap(Properties.Resources.Peek);
            animation[27] = new Bitmap(Properties.Resources.Peek);
            animation[28] = new Bitmap(Properties.Resources.Peek);
            animation[29] = new Bitmap(Properties.Resources.Peek);
            animation[30] = new Bitmap(Properties.Resources.Peek);
            animation[31] = new Bitmap(Properties.Resources.Peek);
            animation[32] = new Bitmap(Properties.Resources.Peek);
            animation[33] = new Bitmap(Properties.Resources.Peek);
            animation[34] = new Bitmap(Properties.Resources.Peek);
            animation[35] = new Bitmap(Properties.Resources.Peek);
            animation[36] = new Bitmap(Properties.Resources.Peek);
            //*/
            animation[37] = new Bitmap(Properties.Resources.Apex);
            animation[38] = new Bitmap(Properties.Resources.Hide);
            animation[39] = new Bitmap(Properties.Resources.Hole);
            animation[40] = new Bitmap(Properties.Resources.Pow);
        }
        // 40 pictures for 3 seconds; need the animation to play smoothly
    }
}
