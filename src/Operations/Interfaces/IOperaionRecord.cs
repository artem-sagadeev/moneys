namespace Operations.Interfaces;

public interface IOperationRecord : IOperation
{
    public DateTime DateTime { get; set; }
}