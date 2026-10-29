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
        var cards = new List<Card>();
        AddCards(cards, "Walk", "Move 1 tile.", CardType.Movement, count: 10, movementValue: 1);
        AddCards(cards, "Sprint", "Move 2 tiles.", CardType.Movement, count: 6, movementValue: 2);
        AddCards(cards, "Advance", "Move 3 tiles.", CardType.Movement, count: 4, movementValue: 3);
        AddCards(cards, "Vehicle", "Move 4 tiles.", CardType.Movement, count: 2, movementValue: 4);

        AddCards(cards, "Aim Shot", "+2 combat damage.", CardType.Combat, count: 4, damage: 2);
        AddCards(cards, "Full Auto", "+4 combat damage.", CardType.Combat, count: 4, damage: 4);
        AddCards(cards, "Grenade", "+5 combat damage.", CardType.Combat, count: 2, damage: 5);

        AddCards(cards, "Med Kit", "Restore 10 health.", CardType.Loot, count: 2, healValue: 10);
        AddCards(cards, "Armor Plate", "Block 5 incoming damage.", CardType.Loot, count: 2, armorValue: 5);

        var shuffled = cards.OrderBy(_ => _random.Next()).ToList();
        return new Deck { Cards = shuffled };
    }

    private static void AddCards(
        ICollection<Card> cards,
        string name,
        string description,
        CardType type,
        int count,
        int movementValue = 0,
        int damage = 0,
        int healValue = 0,
        int armorValue = 0)
    {
        for (var index = 0; index < count; index++)
        {
            cards.Add(new Card
            {
                Name = name,
                Description = description,
                Type = type,
                MovementValue = movementValue,
                Damage = damage,
                HealValue = healValue,
                ArmorValue = armorValue
            });
        }
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
