﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class LineTrip
    {
        public int Id { get; set; }
        public int Lineld { get; set; }
        public TimeSpan StartAt { get; set; }
        public TimeSpan Frequency { get; set; }
        public TimeSpan FinishAt { get; set; }

        public override string ToString() => this.ToStringProperty();

    }
}
