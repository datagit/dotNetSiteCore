using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Person: Human
    {
        public Person(String name, int age): base(name, age)
        {

        }

        public void sayHello()
        {
            Console.WriteLine("Hello {0}, age {1}", this.Name, this.Age);
        }
        
    }
}
