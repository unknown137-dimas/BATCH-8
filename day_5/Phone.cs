abstract class Phone : ICall, IMessage

{
    public abstract void Call(string contactNumber);

    public abstract void Message(string message, string contactNumber);
}