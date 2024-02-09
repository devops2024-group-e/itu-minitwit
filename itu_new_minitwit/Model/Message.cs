using System;

namespace itu_new_minitwit.Model;

public class Message
{
    public string Username { get; set; }
    public string Text { get; set; }
    public string Email { get; set; }
    public DateTime PublishedDate { get; set; }
}