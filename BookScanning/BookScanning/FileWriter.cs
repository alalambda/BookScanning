using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookScanning
{
    public class FileWriter
    {
        public StringBuilder output = new StringBuilder();
        public List<Library> sortedLibraries = null;

        public void OrderLibraries(List<Library> libraries)
        {
            sortedLibraries = libraries.OrderByDescending(x => x.Efficiency).ToList();
        }

        public void SignoffAndShipBooks(List<Library> libraries, Dictionary<int,int> BookToRatingGlobal, int numberOfDaysTotal, string fileName)
        {
            OrderLibraries(libraries);

            var libraryAndBooksString = new StringBuilder();

            var librariesProcessed = 0;
            foreach (var library in sortedLibraries)
            { 
                if (BookToRatingGlobal.Count < 1)
                {
                    break;
                }

                if (library.DaysToSignoff >= numberOfDaysTotal)
                {
                    continue;
                }

                var booksProcessed = 0;
                var daysPassed = library.DaysToSignoff;
                foreach (var bookId in library.BooksSortedByRating)
                {
                    if (daysPassed == numberOfDaysTotal)
                    {
                        break;
                    }

                    if (BookToRatingGlobal.ContainsKey(bookId))
                    {
                        BookToRatingGlobal.Remove(bookId);
                        booksProcessed += library.BooksCanSendPerDay;
                        ++daysPassed;
                    }
                    else
                    {
                        continue;
                    }
                }

                var trimmedBookArray = new int[booksProcessed];
                Array.Copy(library.BooksSortedByRating, trimmedBookArray, booksProcessed);

                libraryAndBooksString.Append(library.Id).Append(" ").Append(booksProcessed).AppendLine();
                libraryAndBooksString.Append(string.Join(" ", trimmedBookArray)).AppendLine();

                output.AppendLine();

                ++librariesProcessed;
            }

            libraryAndBooksString.Insert(0, librariesProcessed)
                .Insert(librariesProcessed.ToString().Length, Environment.NewLine);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"Output/" + fileName))
            {
                file.WriteLine(libraryAndBooksString.ToString());
            }
        }
    }
}
