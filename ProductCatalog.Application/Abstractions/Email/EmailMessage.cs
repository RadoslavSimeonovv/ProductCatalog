namespace ProductCatalog.Application.Abstractions.Email;

public sealed record EmailMessage(
string To,
string Subject,
string HtmlBody,
string? From = null,
string? ReplyTo = null);