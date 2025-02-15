﻿namespace Frelance.Infrastructure.Entities;

public class Addresses
{
    public int Id { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string ZipCode { get; set; }

    public Addresses()
    {

    }
    public Addresses(string country, string city, string street, string streetNumber, string zipCode)
    {
        Country = country;
        City = city;
        Street = street;
        StreetNumber = streetNumber;
        ZipCode = zipCode;
    }

    public Addresses(int id, string country, string city, string street, string streetNumber, string zipCode)
    {
        Id = id;
        Country = country;
        City = city;
        Street = street;
        StreetNumber = streetNumber;
        ZipCode = zipCode;
    }
}