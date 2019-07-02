using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Xunit;
using System.Linq.Dynamic;

namespace RegexPredicateParser
{
    class Program
    {
        static PredicateParser<Person> pParser = new PredicateParser<Person>();

        static void Main(string[] args)
        {
            //TestSingleComparisonStringEquality();
            TestDynamicQuerable();
        }

        public static void TestSingleComparisonStringEquality()
        {
            string text = "Name == A1";

            var people = GetPeople().AsQueryable();

            var pe = Expression.Parameter(
                typeof(Person),"person");

            Expression exp = pParser.Parse(text);

            var whereCallExpression = Expression.Call(
              typeof(Queryable),
              "Where",
              new[] { typeof(Person)},
              people.Expression,
              Expression.Lambda<Func<Person,bool>>(
                  exp, pe));

            var result = people.Provider.CreateQuery<Person>(
                   whereCallExpression).ToList();

            Assert.Single(result);
        }

        public static void TestDynamicQuerable()
        {
        
            var people = GetPeople()
                .AsQueryable()
                .Where(@"(Name == ""N1"" || Surname == ""S3"")
                && (Height > 0.2 || Age < 3) 
                || Address.Address1==""A5""
                || Address.Address1==""A6""")
                .ToList();

            Assert.Equal(4, people.Count);
        }

        public class Person
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public Address Address { get; set; }
            public string Postcode { get; set; }
            public int Age { get; set; }
            public double Height { get; set; }
        }

        public class Address
        {
            public string Address1 { get; set; }
            public string Postcode { get; set; }

        }

        private static List<Person> GetPeople()
        {
            var people = new List<Person>();

            for(int i = 0; i < 10; i++)
            {
                people.Add(new Person
                {
                    Name = $"N{i}",
                    Surname = $"S{i}",
                    Address = new Address() {
                        Address1 = $"A{i}",
                        Postcode = $"P{i}"
                    },
                    Age = i,
                    Height = i * 0.2d
                });
            }

            return people;
        }
    }
}
