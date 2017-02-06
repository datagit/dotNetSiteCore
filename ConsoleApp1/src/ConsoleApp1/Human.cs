using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Human
    {
        private String _name;
        private int _age;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public int Age
        {
            get
            {
                return _age;
            }

            set
            {
                _age = value;
            }
        }

        public Human(String name, int age)
        {
            Name = name;
            Age = age;
        }

        public Human(String name)
        {
            new Human(name, 0);
        }

        public Human(int age)
        {
            new Human("", age);
        }


        ~Human()
        {
            _name = default(String);
            _age = default(int);
        }

    }
}
