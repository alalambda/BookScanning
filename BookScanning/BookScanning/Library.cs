using System;
using System.Collections.Generic;
using System.Text;

namespace BookScanning
{
    public class Library
    {
        public int Id { get; set; }
        public int BooksInLibrary { get; set; }
        public int DaysToSignoff { get; set; }
        public int BooksCanSendPerDay { get; set; }
        public double Efficiency { get; set; }
        public int[] Books { get; set; }
        public Dictionary<int, int> booksSortedByRatingDictionary { get; set; }
    }
}
