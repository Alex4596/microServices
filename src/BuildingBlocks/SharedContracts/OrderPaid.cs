namespace SharedContracts;

public record OrderPaid(
    int OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod = "credit_card",
    string Status = "completed"
);
