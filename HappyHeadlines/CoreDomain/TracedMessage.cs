namespace CoreDomain;

public class TracedMessage<T>
{
   public required T Data { get; set; }

   public Dictionary<string, string> Headers { get; set; } = new();
}