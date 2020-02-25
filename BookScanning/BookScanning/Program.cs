using System;

namespace BookScanning
{
    public class Program
    {

        static void Main(string[] args)
        {
            var files = new string[]
            {
                //"a_example.txt",
                //"b_read_on.txt",
                //"c_incunabula.txt",
                //"d_tough_choices.txt",
                //"e_so_many_books.txt",
                "f_libraries_of_the_world.txt"
            };

            var fileReader = new FileReader();

            foreach (var fileName in files)
            {
                fileReader.Process(fileName);
            }
        }
    }
}
