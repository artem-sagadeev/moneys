using Common.Interfaces;

namespace RegularOperations.Entities;

public class RegularPayment : IEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public bool IsAutoExecution { get; set; }
}