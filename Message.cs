namespace SignalBlaze
{
    public record Message(
        Guid Id,
        string User,
        string Content,
        DateTime DateCreated
    );
}
