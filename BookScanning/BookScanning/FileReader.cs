using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookScanning
{
    public class FileReader
    {
        public string path = @"C:\Users\annus\source\repos\BookScanning\BookScanning\BookScanning\Input\";

        public FileWriter _FileWriter = new FileWriter();

        public List<Library> Libraries = null;
        public Dictionary<int, int> BookToRatingGlobal = null;

        public int NumberOfBooks = 0;
        public int NumberOfLibraries = 0;
        public int NumberOfDays = 0;

        public void Process(string fileName)
        {
            Init(fileName);

            var allBookIds = new List<int>();

            Libraries = new List<Library>(NumberOfLibraries);
            int[] ratingsArray = new int[NumberOfBooks];

            var lines = File.ReadAllLines(path + fileName);

            var booksInLibrary = 0;
            var daysToSignoff = 0;
            var booksCanSendPerDay = 0;


            var librariesVisited = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                if (i < 1) continue;

                var line = lines[i];

                // very second line of the file contains ratings of all books 
                if (i == 1)
                {
                    var valuesFromLine = line.Split(" ");

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
                    var libraryStats = line.Split(" ");
                    booksInLibrary = int.Parse(libraryStats[0]);
                    daysToSignoff = int.Parse(libraryStats[1]);
                    booksCanSendPerDay = int.Parse(libraryStats[2]);

                    continue;
                }

                else
                {
                    // if i is odd number, then it contains ID's of the books in library
                    var books = new List<int>();
                    var values = line.Split(" ");
                    foreach (var value in values)
                    {
                        var bookId = int.Parse(value);
                        books.Add(bookId);
                    }

                    var library = new Library()
                    {
                        Id = librariesVisited, 
                        BooksInLibrary = booksInLibrary,
                        DaysToSignoff = daysToSignoff,
                        BooksCanSendPerDay = booksCanSendPerDay,
                        Books = books.ToArray(),
                        Efficiency = CalculateLibraryEfficiency(daysToSignoff, booksCanSendPerDay, books.Count)
                    };

                    Libraries.Add(library);

                    ++librariesVisited;

                    allBookIds.AddRange(books);
                }

            }

            var distinctBookIdsGlobal = allBookIds.Distinct().ToArray();

            BookToRatingGlobal = MapBooksToRatings(distinctBookIdsGlobal, ratingsArray);

            var areRatingsSame = AreRatingsSame(ratingsArray);
            if (!areRatingsSame)
            {
                BookToRatingGlobal = BookToRatingGlobal.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                SortBooksByRatingPerLibrary();
            }

            _FileWriter.SignoffAndShipBooks(Libraries, BookToRatingGlobal, NumberOfDays, NumberOfBooks, fileName);
        }

        public Dictionary<int, int> MapBooksToRatings(int[] distinctBookIds, int[] ratingsArray)
        {
            var bookToRating = new Dictionary<int, int>(distinctBookIds.Length);
            for (int i = 0; i < distinctBookIds.Length; i++)
            {
                int d = distinctBookIds[i];
                int r = ratingsArray[i];
                bookToRating.Add(d, r);
            }

            return bookToRating;
        }

        public void Init(string fileName)
        {
            // very first line of file contains total number of books; libraries; and days to process
            var firstLine = File.ReadLines(path + fileName).First();

            string[] values = firstLine.Split(" ");

            NumberOfBooks = int.Parse(values[0]);
            NumberOfLibraries = int.Parse(values[1]);
            NumberOfDays = int.Parse(values[2]);
        }

        public double CalculateLibraryEfficiency(int signoffDays, int booksPerDay, int booksInLibrary)
        {
            return Convert.ToDouble(signoffDays) + (Convert.ToDouble(booksInLibrary) / Convert.ToDouble(booksPerDay));
        }

        public void SortBooksByRatingPerLibrary()
        {
            foreach (var library in Libraries)
            {
                if (library.Books == null || library.Books.Length == 0)
                {
                    break;
                }

                library.booksSortedByRatingDictionary = new Dictionary<int, int>();

                for (int i = 0; i < library.Books.Length; i++)
                {
                    var bookId = library.Books[i];
                    BookToRatingGlobal.TryGetValue(bookId, out int rating);
                    library.booksSortedByRatingDictionary.Add(bookId, rating);
                }
                library.booksSortedByRatingDictionary = 
                    library.booksSortedByRatingDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public bool AreRatingsSame(int[] ratingsArray)
        {
            return ratingsArray.Distinct().Count() == 1;
        }
    }
}
