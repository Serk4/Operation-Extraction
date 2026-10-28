namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Creates and shuffles the extraction card deck used during a match.
/// </summary>
public class CardService : ICardService
{
    private readonly Random _random;

    public CardService()
    {
        _random = new Random();
    }

    public Deck CreateDeck()
    {
        var cards = new List<Card>
        {
            new() { Name = "Walk", Description = "Short movement burst.", Type = CardType.Movement, MovementValue = 2 },
            new() { Name = "Sprint", Description = "Fast repositioning.", Type = CardType.Movement, MovementValue = 3 },
            new() { Name = "Tactical Advance", Description = "Controlled advance across the zone.", Type = CardType.Movement, MovementValue = 4 },
            new() { Name = "Off Road Vehicle", Description = "Long-range repositioning across rough terrain.", Type = CardType.Movement, MovementValue = 6 },
            new() { Name = "Aim Shot", Description = "Reliable precision attack.", Type = CardType.Combat, Damage = 5 },
            new() { Name = "Full Auto", Description = "Heavy combat output.", Type = CardType.Combat, Damage = 8 },
            new() { Name = "Grenade", Description = "Area denial and burst damage.", Type = CardType.Combat, Damage = 6 },
            new() { Name = "Armor Up", Description = "Improve defensive posture.", Type = CardType.Combat, ArmorValue = 6 },
            new() { Name = "Medical Kit", Description = "Restore health during a push.", Type = CardType.Loot, HealValue = 15 },
            new() { Name = "Armor Plate", Description = "Adds durable mitigation.", Type = CardType.Loot, ArmorValue = 8 },
            new() { Name = "Walk", Description = "Short movement burst.", Type = CardType.Movement, MovementValue = 2 },
            new() { Name = "Sprint", Description = "Fast repositioning.", Type = CardType.Movement, MovementValue = 3 },
            new() { Name = "Aim Shot", Description = "Reliable precision attack.", Type = CardType.Combat, Damage = 5 },
            new() { Name = "Armor Plate", Description = "Adds durable mitigation.", Type = CardType.Loot, ArmorValue = 8 }
        };

        var shuffled = cards.OrderBy(_ => _random.Next()).ToList();
        return new Deck { Cards = shuffled };
    }

    public List<Card> DrawOpeningHand(Deck deck, int count)
    {
        var hand = new List<Card>();
        for (var index = 0; index < count && deck.Cards.Count > 0; index++)
        {
            var nextCard = deck.Cards[0];
            deck.Cards.RemoveAt(0);
            hand.Add(nextCard);
        }

        return hand;
    }
}
