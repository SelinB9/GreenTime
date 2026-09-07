using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace GreenTime.Models;


// Kategori türleri: Green (Eğitici/Geliştirici) veya Red (Eğlence)
public enum CategoryType
{
    Green,
    Red
}

public class CuratedPlaylist
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string LanguageCode { get; set; } = "TR";//TR VEYA EN
    public DateTime CreatedUtc { get; set; }

    //BİR PLAYLİSTİNİN BİRDEN FAZLA VİDEOSU OLUR (1-to-many ilişki)
    public ICollection<CuratedVideo> Videos { get; set; } = new List<CuratedVideo>();

}