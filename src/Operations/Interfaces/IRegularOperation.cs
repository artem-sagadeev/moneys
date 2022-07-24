using Operations.Enums;

namespace Operations.Interfaces;

public interface IRegularOperation : IOperation
{
    public Frequency Frequency { get; set; }
    
    public bool IsActive { get; set; }
}