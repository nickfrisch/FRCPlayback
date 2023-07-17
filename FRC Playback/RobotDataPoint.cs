using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRC_Playback
{
    enum RobotColor
    {
        RED,
        BLUE,
        UNKNOWN
    }

    internal class RobotDataPoint
    {

        public RobotDataPoint(RobotColor clr, int num, double _x, double _y)
        {
            color = clr;
            robotNumber = num;
            x = _x;
            y = _y;
        }

        public RobotDataPoint(RobotColor clr, int num, double _x, double _y, double _vx, double _vy)
        {
            color = clr;
            robotNumber = num;
            x = _x;
            y = _y;

            Vx = _vx;
            Vy = _vy;
        }

        public RobotDataPoint(RobotColor clr, int num, double _x, double _y, double _vx, double _vy, double _ax, double _ay)
        {
            color = clr;
            robotNumber = num;
            x = _x;
            y = _y;
            
            Vx = _vx;
            Vy = _vy;

            Ax = _ax;
            Ay = _ay;
        }

        public RobotColor color;
        public int robotNumber;

        public double x { get; set; }
        public double y { get; set; }
        // public double theta { get; set; }

        public double Vx { get; set; }
        public double Vy { get; set; }
        // public double omega { get; set; }
         
        public double Ax { get; set; }
        public double Ay { get; set; }
        // public double alpha { get; set; }
    }
}
