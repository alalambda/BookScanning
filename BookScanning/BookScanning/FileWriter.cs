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

        public List<Library> SortedLibraries = null;

        public List<Library> OutsiderLibraries = null;

        public void OrderLibraries(List<Library> libraries, string fileName)
        {
            if (fileName == "c_incunabula.txt")
            {
                SortedLibraries = libraries.OrderBy(x => x.DaysToSignoff).ToList();
                return;
            }
            if (fileName == "d_tough_choices.txt")
            {
                SortedLibraries = libraries.OrderByDescending(x => x.Efficiency).ToList();
                return;
            }
            //if (fileName == "e_so_many_books.txt")
            //{
            //    SortedLibraries = libraries.OrderBy(x => Convert.ToDouble(x.DaysToSignoff) / Convert.ToDouble(x.BooksCanSendPerDay))
            //        .Where(x => Convert.ToDouble(x.BooksCanSendPerDay) / Convert.ToDouble(x.BooksInLibrary) * 100 <= 1.0).ToList();

            //    //stats
            //    Console.WriteLine("Id;BooksInLibrary;DaysToSignoff;BooksCanSendPerDay");
            //    SortedLibraries.ForEach(x => Console.WriteLine($"{x.Id};{x.BooksInLibrary};{x.DaysToSignoff};{x.BooksCanSendPerDay}"));
            //    return;
            //}
            //if (fileName == "f_libraries_of_the_world.txt")
            //{
            //    SortedLibraries = libraries.OrderBy(x => Convert.ToDouble(x.DaysToSignoff) / Convert.ToDouble(x.BooksCanSendPerDay))
            //        .Where(x => Convert.ToDouble(x.BooksCanSendPerDay) / Convert.ToDouble(x.BooksInLibrary) * 100 <= 1.0).ToList();

            //    //stats
            //    Console.WriteLine("Id;BooksInLibrary;DaysToSignoff;BooksCanSendPerDay");
            //    SortedLibraries.ForEach(x => Console.WriteLine($"{x.Id};{x.BooksInLibrary};{x.DaysToSignoff};{x.BooksCanSendPerDay}"));
            //    return;
            //}
            else
            {
                SortedLibraries = libraries.OrderBy(x => Convert.ToDouble(x.DaysToSignoff) / Convert.ToDouble(x.BooksCanSendPerDay))
                    .Where(x => Convert.ToDouble(x.BooksCanSendPerDay) / Convert.ToDouble(x.BooksInLibrary) * 100 <= 1.0).ToList();

                OutsiderLibraries = libraries.Where(x => Convert.ToDouble(x.BooksCanSendPerDay) / Convert.ToDouble(x.BooksInLibrary) * 100 > 1.0)
                    .OrderBy(x => Convert.ToDouble(x.DaysToSignoff) / Convert.ToDouble(x.BooksCanSendPerDay)).ToList();

                //SortedLibraries = libraries.OrderBy(x => Convert.ToDouble(x.DaysToSignoff) / Convert.ToDouble(x.BooksCanSendPerDay)).ToList();
                return;
            }
        }

        public void SignoffAndShipBooks(List<Library> libraries, Dictionary<int, int> bookToRatingGlobal, int numberOfDaysTotal, int numberOfBooksTotal, string fileName)
        {
            if (File.Exists(path + fileName))
            {
                File.Delete(path + fileName);
            }

            var ratings = bookToRatingGlobal;

            OrderLibraries(libraries, fileName);

            var librariesProcessed = ProcessLibraries(SortedLibraries, ratings, numberOfDaysTotal, fileName);

            if (OutsiderLibraries != null && OutsiderLibraries.Count > 0)
            {
                librariesProcessed += ProcessLibraries(OutsiderLibraries, ratings, numberOfDaysTotal, fileName);
            }

            CreateEntry(librariesProcessed.ToString(), fileName);
        }

        public int ProcessLibraries(List<Library> librariesToProcess, Dictionary<int, int> bookToRatingGlobal, int numberOfDaysTotal, string fileName)
        {
            var libraryAndBooksString = new StringBuilder();

            var librariesProcessed = 0;

            foreach (var library in librariesToProcess)
            {
                if (bookToRatingGlobal.Count < 1)
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

                        if (bookToRatingGlobal.ContainsKey(bookId))
                        {
                            bookToRatingGlobal.Remove(bookId);

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

                        if (bookToRatingGlobal.ContainsKey(bookId))
                        {
                            bookToRatingGlobal.Remove(bookId);

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

            return librariesProcessed;
        }

        public void CreateEntry(string lineToAdd, string fileName)
        {
            var txtLines = File.ReadAllLines(path + fileName).ToList();
            txtLines.Insert(0, lineToAdd);
            File.WriteAllLines(path + fileName, txtLines);
        }
    }
}
