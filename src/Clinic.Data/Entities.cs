namespace Clinic.Data;

public class Pet
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Species { get; set; }
}

// Rows arrive from a lab feed, sometimes before the pet itself is registered.
public class VaccinationRecord
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public required string Vaccine { get; set; }
    public DateOnly GivenOn { get; set; }
}

// Written by the front desk, always after the pet exists: keep the real constraint.
public class Visit
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateTime StartsAt { get; set; }
}

public class OnCallShift
{
    public int Id { get; set; }
    public required string Vet { get; set; }
    public DateTime StartsAt { get; set; }
}
