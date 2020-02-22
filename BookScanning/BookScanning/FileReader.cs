using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookScanning
{
    public class FileReader
    {
        public List<Library> Libraries = null;

        public int numberOfBooks = 0;
        public int numberOfLibraries = 0;
        public int numberOfDays = 0;

        public int[] ratingsArray = null;
        public int[] distinctBookIds = null;

        public void Process(string fileName)
        {
            Init(fileName);

            var allBookIds = new List<int>();

            Libraries = new List<Library>(numberOfLibraries);
            ratingsArray = new int[numberOfBooks];

            var lines = File.ReadAllLines(fileName);

            for (var i = 0; i < lines.Length; i++)
            {
                var booksInLibrary = 0;
                var daysToSignoff = 0;
                var booksCanSendPerDay = 0;

                if (i < 1) continue;

                var line = lines[i];

                // very second line of the file contains ratings of all books 
                if (i == 1)
                {
                    var valuesFromLine = line.Split(' ');

                    for (int j = 0; j < valuesFromLine.Length; j++)
                    {
                        var rating = int.Parse(valuesFromLine[j]);
                        ratingsArray[j] = rating;
                    }

                    continue;
                }

                // in i is even number, then it contains number of books in library; number of days to signup; and number of books library can send per day
                if (i % 2 == 0)
                {
                    var libraryStats = line.Split(' ');
                    booksInLibrary = int.Parse(libraryStats[0]);
                    daysToSignoff = int.Parse(libraryStats[1]);
                    booksCanSendPerDay = int.Parse(libraryStats[2]);

                    continue;
                }

                // if i is odd number, then it contains ID's of the books in library
                var books = new List<int>();
                var values = line.Split(' ');
                foreach (var value in values)
                {
                    var bookId = int.Parse(value);
                    books.Add(bookId);
                }

                if (i % 2 != 0)
                {
                    var library = new Library()
                    {
                        BooksInLibrary = booksInLibrary,
                        DaysToSignoff = daysToSignoff,
                        BooksCanSendPerDay = booksCanSendPerDay,
                        BookIds = books.ToArray(),
                        BookRatings = GetOrderOfBooksInLibraryByRating(books.ToArray()),
                        Efficiency = CalculateLibraryEfficiency(daysToSignoff, booksCanSendPerDay, booksInLibrary)
                    };

                    Libraries.Add(library);
                }

                allBookIds.Concat(books);
            }

            distinctBookIds = allBookIds.Distinct().ToArray();
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

        public double CalculateLibraryEfficiency(int signoffDays, int booksPerDay, int booksInLibrary)
        {
            return signoffDays + (booksInLibrary / booksPerDay);
        }

        public int[] GetOrderOfBooksInLibraryByRating(int[] books)
        {
            return books.OrderByDescending(x => x).ToArray();
        }
    }
}
