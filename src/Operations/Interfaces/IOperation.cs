﻿using Common.Interfaces;

namespace Operations.Interfaces;

public interface IOperation : IEntity
{
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Guid CardId { get; set; }
}