﻿namespace Frelance.Infrastructure.Entities;

public class Addresses
{
    public int Id { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string StreetNumber { get; set; }
    public string ZipCode { get; set; }
}