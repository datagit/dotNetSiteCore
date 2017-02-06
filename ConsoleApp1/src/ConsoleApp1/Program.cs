using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello world!");

            //Person p = new Person("Dat Dao", 100);
            //p.sayHello();

            ////anonymous type
            //var student = new {
            //    firstName = "Dat.Dao",
            //    age = 10
            //};

            //Console.WriteLine("this is name's Student: {0}", student.firstName);

            ////lambda expression
            //int[] numbers = { 1,2,3,4,5,6,7,8,9,10 };
            //int oldNumbers = numbers.Count(n => n % 2 == 1);
            //Console.WriteLine("Count old number is {0}", oldNumbers);

            //Console.ReadKey();


            Program.Testing();
            Console.ReadKey();
        }

        public static void Testing() {
            string[] words = { "hello", "wonderful", "LINQ", "beautiful", "world" };
            //Get only short words
            var shortWords = from word in words
                             where word.Length <= 5
                             select word;
            var shortWords2 = words.Where<string>(w => w.StartsWith("he"));

            //Print each word out
            foreach (var word in shortWords2) {
                Console.WriteLine(word);
            }
            Console.ReadLine();
        }
    }
}
