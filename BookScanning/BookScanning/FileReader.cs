using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookScanning
{
    public class FileReader
    {
        public int numberOfBooks = 0;
        public int numberOfLibraries = 0;
        public int numberOfDays = 0;

        public int[] ratingsArray = null;
        public int[] distinctBookIds = null;

        public void Process(string fileName)
        {
            Init(fileName);

            var bookIds = new List<int>();
            ratingsArray = new int[numberOfBooks];

            var lines = File.ReadAllLines(fileName);

            for (var i = 0; i < lines.Length; i++)
            {
                if (i < 1) continue;

                var line = lines[i];

                // very second line of the file contains ratings of all books 
                if (i == 1)
                {
                    var values = line.Split(' ');

                    for (int j = 0; j < values.Length; j++)
                    {
                        var rating = int.Parse(values[j]);
                        ratingsArray[j] = rating;
                    }

                    continue;
                }

                // in i is even number, then it contains number of books in library; number of days to signup; and number of books library can send per day
                if (i % 2 == 0)
                {
                   
                }
                // if i is odd number, then it contains ID's of the books in library
                else
                {
                    int bookCount = 0;
                    string[] values = line.Split(' ');
                    foreach (var value in values)
                    {
                        var bookId = int.Parse(value);
                        bookIds.Add(bookId);
                        ++bookCount;
                    }
                }
            }

            distinctBookIds = bookIds.Distinct().ToArray();
        }

        public void Init(string fileName)
        {
            // very first line of file contains total number of books; libraries; and days to process
            var firstLine = File.ReadLines(fileName).First();

            string[] values = firstLine.Split(' ');

            numberOfBooks = int.Parse(values[0]);
            numberOfLibraries = int.Parse(values[1]);
            numberOfDays = int.Parse(values[2]);
        }

        public double CalculateLibraryEfficiency(int libraryId, int signoff, int booksPerDay, int libraryRating)
        {

        }

        public int CalculateLibraryRating(int[] books, int[] ratings)
        {
            var libraryRating = 0;

        }

        public int[] OrderBooksInLibraryByRating(int[] books)
        {
            return books.OrderByDescending(x => x).ToArray();
        }

        public int[] CalculateOrderOfLibraries(int[] signoffs, int[] booksPerDay, int[] books)
        {
            // pair of library ID and it's time to deliver
            var order = new Dictionary<int, int>();

            for (int i = 0; i < numberOfLibraries; i++)
            {
                var timeToDeliver = signoffs[i] + 1 / booksPerDay[i] * books[i];
                order.Add(i, timeToDeliver); 
            }

            var sorted = from entry in order orderby entry.Value descending select entry;

            return sorted.Select(x => x.Key).ToArray();
        }
    }
}
