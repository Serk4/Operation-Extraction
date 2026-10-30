namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Creates and manages encounter deck draws and discard behavior.
/// </summary>
public class EncounterDeckService : IEncounterDeckService
{
    /// <inheritdoc />
    public List<EncounterDeckCard> CreateEncounterDeck()
    {
        var deck = new List<EncounterDeckCard>();
        AddCards(deck, EncounterCardType.Nothing, "Nothing", "No hostiles or notable activity.", 8);
        AddCards(deck, EncounterCardType.EnemyPatrol, "Enemy Patrol", "Light enemy patrol engages.", 5);
        AddCards(deck, EncounterCardType.MilitiaSquad, "Militia Squad", "Organized militia pushes your position.", 4);
        AddCards(deck, EncounterCardType.MercenarySquad, "Mercenary Squad", "Well-equipped mercenaries engage.", 3);
        AddCards(deck, EncounterCardType.SupplyCache, "Supply Cache", "Supply cache discovered nearby.", 3);
        AddCards(deck, EncounterCardType.SafeHouse, "Safe House", "Temporary shelter provides recovery.", 2);
        AddCards(deck, EncounterCardType.BossEncounter, "Boss Encounter", "Elite hostile unit blocks your route.", 1);
        return deck.OrderBy(_ => Random.Shared.Next()).ToList();
    }

    /// <inheritdoc />
    public EncounterDeckCard DrawEncounter(GameSession session)
    {
        if (session.EncounterDeck.Count == 0)
        {
            session.EncounterDeck = session.EncounterDiscard.OrderBy(_ => Random.Shared.Next()).ToList();
            session.EncounterDiscard = new List<EncounterDeckCard>();
            session.EventLog.Add("Encounter deck reshuffled from discard pile.");
        }

        if (session.EncounterDeck.Count == 0)
        {
            return new EncounterDeckCard
            {
                Type = EncounterCardType.Nothing,
                Name = "Nothing",
                Description = "No encounter."
            };
        }

        var card = session.EncounterDeck[0];
        session.EncounterDeck.RemoveAt(0);
        return card;
    }

    /// <inheritdoc />
    public void DiscardEncounter(GameSession session, EncounterDeckCard card)
    {
        session.EncounterDiscard.Add(card);
    }

    private static void AddCards(ICollection<EncounterDeckCard> cards, EncounterCardType type, string name, string description, int count)
    {
        for (var index = 0; index < count; index++)
        {
            cards.Add(new EncounterDeckCard
            {
                Type = type,
                Name = name,
                Description = description
            });
        }
    }
}
