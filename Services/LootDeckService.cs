namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Creates and manages loot deck draws and discard behavior.
/// </summary>
public class LootDeckService : ILootDeckService
{
    /// <inheritdoc />
    public List<LootDeckCard> CreateLootDeck()
    {
        var deck = new List<LootDeckCard>();
        AddCards(deck, LootType.Cash, "Cash", "Recovered cash bundle.", 8);
        AddCards(deck, LootType.Materials, "Materials", "Recovered construction and repair materials.", 6);
        AddCards(deck, LootType.MedicalSupplies, "Medical Supplies", "Field medicine and stims.", 5);
        AddCards(deck, LootType.ArmorSupplies, "Armor Supplies", "Plates and reinforcement materials.", 5);
        AddCards(deck, LootType.WeaponUpgrade, "Weapon Upgrade", "Improved weapon calibration kit.", 3);
        AddCards(deck, LootType.Intel, "Intel", "Tactical intelligence data.", 4);
        AddCards(deck, LootType.ClassifiedIntel, "Classified Intel", "High-value classified records.", 2);
        return deck.OrderBy(_ => Random.Shared.Next()).ToList();
    }

    /// <inheritdoc />
    public LootDeckCard DrawLoot(GameSession session)
    {
        if (session.LootDeck.Count == 0)
        {
            session.LootDeck = session.LootDiscard.OrderBy(_ => Random.Shared.Next()).ToList();
            session.LootDiscard = new List<LootDeckCard>();
            session.EventLog.Add("Loot deck reshuffled from discard pile.");
        }

        if (session.LootDeck.Count == 0)
        {
            return new LootDeckCard
            {
                Type = LootType.Materials,
                Name = "Materials",
                Description = "Fallback salvage materials."
            };
        }

        var card = session.LootDeck[0];
        session.LootDeck.RemoveAt(0);
        return card;
    }

    /// <inheritdoc />
    public void DiscardLoot(GameSession session, LootDeckCard card)
    {
        session.LootDiscard.Add(card);
    }

    private static void AddCards(ICollection<LootDeckCard> cards, LootType type, string name, string description, int count)
    {
        for (var index = 0; index < count; index++)
        {
            cards.Add(new LootDeckCard
            {
                Type = type,
                Name = name,
                Description = description
            });
        }
    }
}
