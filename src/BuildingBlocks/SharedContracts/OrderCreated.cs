namespace SharedContracts;

public record OrderCreated(
    int OrderId,
    string Product, 
    int Quantity
    );
