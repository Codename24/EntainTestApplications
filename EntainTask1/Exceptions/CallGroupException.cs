namespace EntainTask1.Exceptions
{
    public class CallGroupException : Exception
    {
        public CallGroupException(string message, Exception error): base(message, error) { }
    }
}
