﻿using System;

namespace BookScanning
{
    public class Program
    {

        static void Main(string[] args)
        {
            var files = new string[]
            {
                "a_example.txt",
                "b_read_on.txt"
            };

            var fileReader = new FileReader();

            foreach (var fileName in files)
            {
                fileReader.Process(fileName);
            }
        }
    }
}
