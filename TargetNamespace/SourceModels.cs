using System.Collections.Generic;

namespace TsModelGen.TargetNamespace
{
    public class Person
    {
        public string FirstName { get; set; }

        public int Age;

        public List<Address> Addresses { get; set; }

        public Address DefaultAddress;
    }

    public class Employee : Person
    {
        public bool IsSubscribedToNews;
        public decimal Salary { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
    }
}
