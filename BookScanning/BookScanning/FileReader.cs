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

        public int numberOfBooks = 0;
        public int numberOfLibraries = 0;
        public int numberOfDays = 0;

        public void Process(string fileName)
        {
            Init(fileName);

            var allBookIds = new List<int>();

            Libraries = new List<Library>(numberOfLibraries);
            int[] ratingsArray = new int[numberOfBooks];

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
                        Efficiency = CalculateLibraryEfficiency(daysToSignoff, booksCanSendPerDay, booksInLibrary),
                        Books = books.ToArray()
                    };

                    Libraries.Add(library);

                    ++librariesVisited;

                    allBookIds.AddRange(books);
                }

            }

            var distinctBookIdsGlobal = allBookIds.Distinct().ToArray();

            BookToRatingGlobal = MapBooksToRatings(distinctBookIdsGlobal, ratingsArray);

            SortBooksByRatingPerLibrary();

            _FileWriter.SignoffAndShipBooks(Libraries, BookToRatingGlobal, numberOfDays, fileName);
        }

        public Dictionary<int, int> MapBooksToRatings(int[] distinctBookIds, int[] ratingsArray)
        {
            var bookToRating = new Dictionary<int, int>(numberOfBooks);
            for (int i = 0; i < numberOfBooks; i++)
            {
                bookToRating.Add(distinctBookIds[i], ratingsArray[i]);
            }

            return bookToRating.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public void Init(string fileName)
        {
            // very first line of file contains total number of books; libraries; and days to process
            var firstLine = File.ReadLines(path + fileName).First();

            string[] values = firstLine.Split(" ");

            numberOfBooks = int.Parse(values[0]);
            numberOfLibraries = int.Parse(values[1]);
            numberOfDays = int.Parse(values[2]);
        }

        public double CalculateLibraryEfficiency(int signoffDays, int booksPerDay, int booksInLibrary)
        {
            return Convert.ToDouble(signoffDays) + (Convert.ToDouble(booksInLibrary) / Convert.ToDouble(booksPerDay));
        }

        public void SortBooksByRatingPerLibrary()
        {
            foreach (var library in Libraries)
            {
                library.booksSortedByRatingDictionary = new Dictionary<int, int>(library.BooksInLibrary);

                for (int i = 0; i < library.BooksInLibrary; i++)
                {
                    var bookId = library.Books[i];
                    BookToRatingGlobal.TryGetValue(bookId, out int rating);
                    library.booksSortedByRatingDictionary.Add(bookId, rating);
                }
                library.booksSortedByRatingDictionary = 
                    library.booksSortedByRatingDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
