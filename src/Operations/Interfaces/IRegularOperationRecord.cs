namespace Operations.Interfaces;

public interface IRegularOperationRecord : IOperation
{
    public Guid RegularOperationId { get; set; }
}