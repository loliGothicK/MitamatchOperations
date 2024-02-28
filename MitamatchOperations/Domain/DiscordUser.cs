namespace Mitama.Domain;

public struct DiscordUser
{
    public string id { get; set; }
    public string username { get; set; }
    public string discriminator { get; set; }
    public string avatar { get; set; }
    public string global_name { get; set; }
    public string email { get; set; }
}
