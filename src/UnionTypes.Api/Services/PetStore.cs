using System.Collections.Concurrent;
using UnionTypes.Api.Models;

namespace UnionTypes.Api.Services;

public class PetStore
{
    private readonly ConcurrentDictionary<int, Pet> _pets = new()
    {
        [1] = new Dog("Rex", "Husky"),
        [2] = new Cat("Misty", 7),
        [3] = new Bird("Kiwi", true),
    };
    private int _nextId = 3;

    public IEnumerable<PetEntry> GetAll() =>
        _pets.OrderBy(p => p.Key).Select(p => new PetEntry(p.Key, p.Value));

    public Pet? Find(int id) => _pets.TryGetValue(id, out var pet) ? pet : (Pet?)null;

    public PetEntry Add(Pet pet)
    {
        int id = Interlocked.Increment(ref _nextId);
        _pets[id] = pet;
        return new PetEntry(id, pet);
    }
}
