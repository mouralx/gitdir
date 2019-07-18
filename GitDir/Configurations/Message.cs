using System.Linq;

public class Message
{
    public string Identifier { get; set; }
    public MessageText[] Texts { get; set; }
    public string GetText(string countryCode)
    {
        return Texts.SingleOrDefault(mt => mt.CountryCode.ToLower() == countryCode.ToLower())?.Text;
    }
}