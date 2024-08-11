using Lab1.DAL.Entities;

namespace Lab1.Models
{
    public interface IDublinCore
    {
        string Title { get; set; }
        User Creator { get; set; }
        string Subject { get; set; }
        string Description { get; set; }
        string Publisher { get; set; }
        string Contributor { get; set; }
        DateTime Date { get; set; }
        string Type { get; set; }
        string Format { get; set; }
        string Identifier { get; set; }
        string Source { get; set; }
        string Language { get; set; }
        string Relation { get; set; }
        string Coverage { get; set; }
        string Rights { get; set; }
    }
}
