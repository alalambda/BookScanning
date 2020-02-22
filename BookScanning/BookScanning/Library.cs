﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BookScanning
{
    public class Library
    {
        public int BooksInLibrary { get; set; }
        public int DaysToSignoff { get; set; }
        public int BooksCanSendPerDay { get; set; }
        public double Efficiency { get; set; }
        public int[] BookIds { get; set; }
        public int[] BookRatings { get; set; }
    }
}
