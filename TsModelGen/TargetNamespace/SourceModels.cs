using System.Collections;
using System.Collections.Generic;
using TsModelGen.NonTargetNamespace;
using TsModelGen.TargetNamespace;

namespace TsModelGen.NonTargetNamespace
{
    public class Person
    {
        public string FirstName { get; set; }
        public int Age;
        public IList Addresses1 { get; set; }
        public IEnumerable Addresses2 { get; set; }
        public List<Address> Addresses3 { get; set; }
        public Address[] Addresses4 { get; set; }
        public ISet<Address> Addresses5 { get; set; }
        public IList<Address> Addresses6 { get; set; }
        public IEnumerable<Address> Addresses7 { get; set; }

        public Address DefaultAddress;
        public IDictionary SomeDictionary;
        public int? NullableInt;
        public bool? NullableBool;
    }
}

namespace TsModelGen.TargetNamespace
{
    public class Employee : Person
    {
        public bool IsSubscribedToNews;
        public decimal Salary { get; set; }
        public IDictionary<string, Address> SpecialAddresses { get; set; }
    }

    public class Address
    {
        public Kind Kind { get; set; }
        public string Street { get; set; }
    }

    public enum Kind
    {
        Domestic,
        International
    }
}
