﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_7799_9212
{
    class BusLineStation : BuStation
    {
        

        public BusLineStation(int distance, int time, int Dsk, double Latitude, double Longitude, string Name) : base(Dsk, Latitude, Longitude, Name)
        {
            Station_Distance = distance;
            Travel_Time_From_Station = time;
        }
        private int Station_Distance;
        public int SD
        { 
            get { return Station_Distance; }
            set
            { 
                Station_Distance = value;
            }
        }
        private int Travel_Time_From_Station; 
        public int TTFS
        { 
            get { return Travel_Time_From_Station; }
            set
            { 
                Travel_Time_From_Station = value;
            }
        }

    }
}
