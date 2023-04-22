using System.Runtime.Serialization;

namespace PortfolioBlazorWasm.Models.FactsApi;

[DataContract]
public class FactDto
{
    [DataMember(Name = "id")]
    public string? Id { get; set; }
    [DataMember(Name = "text")]
    public string? Text { get; set; }

    [DataMember(Name = "source")]
    public string? Source { get; set; }
    [DataMember(Name = "source_url")]
    public string? Source_url { get; set; }
    [DataMember(Name = "language")]
    public string? Language { get; set; }
    [DataMember(Name = "permalink")]
    public string? Permalink { get; set; }
}
