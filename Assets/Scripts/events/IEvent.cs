public interface IEvent
{
    string Type { get; }
    object Data { get; set; }
}
