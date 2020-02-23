using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookScanning
{
    public class FileWriter
    {
        public string path = @"C:\Users\annus\source\repos\BookScanning\BookScanning\BookScanning\Output\";

        private static readonly char[] TrimNewLineChars = Environment.NewLine.ToCharArray();

        public List<Library> sortedLibraries = null;

        public void OrderLibraries(List<Library> libraries, bool areRatingsSame, int numberOfDaysTotal, int numberOfBooksTotal)
        {
            if (numberOfDaysTotal == numberOfBooksTotal)
            {
                sortedLibraries = libraries.OrderBy(x => x.DaysToSignoff).ToList();
                return;
            }

            if (numberOfBooksTotal == 100000 && numberOfDaysTotal < 1000)
            {
                sortedLibraries = libraries.OrderBy(x => x.BooksCanSendPerDay).ToList();
                return;
            }

            if (!areRatingsSame)
            {
                sortedLibraries = libraries.OrderBy(x => x.Efficiency).ToList();
                return;
            }

            if (areRatingsSame)
            {
                sortedLibraries = libraries.OrderByDescending(x => x.Efficiency).ToList();
                return;
            }
        }

        public void SignoffAndShipBooks(List<Library> libraries, Dictionary<int, int> BookToRatingGlobal, int numberOfDaysTotal, string fileName, bool areRatingsSame, int numberOfBooksTotal)
        {
            if (File.Exists(path + fileName))
            {
                File.Delete(path + fileName);
            }

            OrderLibraries(libraries, areRatingsSame, numberOfDaysTotal, numberOfBooksTotal);

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
                var daysPassed = Convert.ToDouble(library.DaysToSignoff);
                if (library.booksSortedByRatingDictionary != null)
                {
                    foreach (var bookId in library.booksSortedByRatingDictionary.Keys.ToArray())
                    {
                        if (daysPassed == Convert.ToDouble(numberOfDaysTotal))
                        {
                            break;
                        }

                        if (BookToRatingGlobal.ContainsKey(bookId))
                        {
                            BookToRatingGlobal.Remove(bookId);

                            if (library.BooksCanSendPerDay >= library.BooksInLibrary)
                            {
                                booksProcessed += library.BooksInLibrary;
                                ++daysPassed;
                                break;
                            }
                            else if (library.BooksCanSendPerDay == 1)
                            {
                                ++booksProcessed;
                                ++daysPassed;
                            }
                            else if (library.BooksCanSendPerDay > 1 && library.BooksCanSendPerDay < library.BooksInLibrary)
                            {
                                ++booksProcessed;
                                daysPassed += 1 / Convert.ToDouble(library.BooksCanSendPerDay);
                            }
                        }
                        else
                        {
                            library.booksSortedByRatingDictionary.Remove(bookId);
                            continue;
                        }
                    }
                }
                else
                {
                    foreach (var bookId in library.Books)
                    {
                        if (daysPassed == Convert.ToDouble(numberOfDaysTotal))
                        {
                            break;
                        }

                        if (BookToRatingGlobal.ContainsKey(bookId))
                        {
                            BookToRatingGlobal.Remove(bookId);

                            if (library.BooksCanSendPerDay >= library.BooksInLibrary)
                            {
                                booksProcessed += library.BooksInLibrary;
                                ++daysPassed;
                                break;
                            }
                            else if (library.BooksCanSendPerDay == 1)
                            {
                                ++booksProcessed;
                                ++daysPassed;
                            }
                            else if (library.BooksCanSendPerDay > 1 && library.BooksCanSendPerDay < library.BooksInLibrary)
                            {
                                ++booksProcessed;
                                daysPassed += 1 / Convert.ToDouble(library.BooksCanSendPerDay);
                            }
                        }
                        else
                        {
                            library.Books = library.Books.Where(x => x != bookId).ToArray();
                            continue;
                        }
                    }
                }

                if (booksProcessed == 0)
                {
                    break;
                }

                var arrayOfBookIds = new int[library.Books.Length];
                if (library.booksSortedByRatingDictionary != null)
                {
                    arrayOfBookIds = library.booksSortedByRatingDictionary.Keys.ToArray();
                }
                else
                {
                    arrayOfBookIds = library.Books;
                }

                if (arrayOfBookIds.Length > booksProcessed)
                {
                    Array.Resize(ref arrayOfBookIds, booksProcessed);
                }

                libraryAndBooksString.Append(library.Id).Append(" ").Append(arrayOfBookIds.Length).AppendLine();
                libraryAndBooksString.Append(string.Join(" ", arrayOfBookIds)).AppendLine();

                using (StreamWriter file = new StreamWriter(path + fileName, true))
                {
                    file.WriteLine(libraryAndBooksString.ToString().TrimEnd(TrimNewLineChars));
                }

                libraryAndBooksString.Clear();

                ++librariesProcessed;
            }


            CreateEntry(librariesProcessed.ToString(), fileName);
        }

        public void CreateEntry(string lineToAdd, string fileName)
        {
            var txtLines = File.ReadAllLines(path + fileName).ToList();   //Fill a list with the lines from the txt file.
            txtLines.Insert(0, lineToAdd);                                //Insert the line you want to add last under the tag 'item1'.
            File.WriteAllLines(path + fileName, txtLines);                //Add the lines including the new one.
        }
    }
}
