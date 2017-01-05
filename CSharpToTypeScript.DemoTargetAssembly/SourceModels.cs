using System;
using System.Collections;
using System.Collections.Generic;
using CSharpToTypeScript.NonTargetNamespace;
using CSharpToTypeScript.TargetNamespace;

namespace CSharpToTypeScript.NonTargetNamespace
{
    public class Person
    {
        public string FirstName { get; set; }
        public int Age;
        public IList Addresses1 { get; set; }
        public IEnumerable Addresses2 { get; set; }
        public List<Address> Addresses3 { get; set; }
        public Address[] Addresses4 { get; set; }
        public Array SomeArray { get; set; }
        public ISet<Address> Addresses5 { get; set; }
        public IList<Address> Addresses6 { get; set; }
        public IEnumerable<Address> Addresses7 { get; set; }

        public Address DefaultAddress;
        public IDictionary SomeDictionary;
        public int? NullableInt;
        public bool? NullableBool;
    }
}

namespace CSharpToTypeScript.TargetNamespace
{
    public class Employee : Person
    {
        public bool IsSubscribedToNews;
        public decimal Salary { get; set; }
        public IDictionary<string, Address> SpecialAddresses { get; set; }
        public IEnumerable Enumerable;
        public IEnumerable<int> EnumerableOfInt;
        public IEnumerable<Money> EnumerableOfMoney;
    }

    public struct Money
    {
        public decimal Amount;
        public Currency Currency;
    }

    public struct Currency
    {
        public string Name;
        public char? CharSymbol;
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

    [Obsolete]
    public class KnownButIgnored
    {
        public string NoNoNo { get; set; }
    }

    public class OkayishClassWithBadProperty
    {
        [Obsolete]
        public string DontUseIt { get; set; }
    }
}
