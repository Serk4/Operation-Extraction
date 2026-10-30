namespace Operation_Extraction.Services;

using Operation_Extraction.Models;

/// <summary>
/// Creates, draws, and discards cards for the movement deck.
/// </summary>
public class MovementDeckService : IMovementDeckService
{
    /// <inheritdoc />
    public Deck CreateMovementDeck()
    {
        var cards = new List<Card>();
        AddCards(cards, "Walk", "Move 1 tile.", 10, 1);
        AddCards(cards, "Sprint", "Move 2 tiles.", 6, 2);
        AddCards(cards, "Advance", "Move 3 tiles.", 4, 3);
        AddCards(cards, "Vehicle", "Move 4 tiles.", 2, 4);

        var shuffled = cards.OrderBy(_ => Random.Shared.Next()).ToList();
        return new Deck { Cards = shuffled };
    }

    /// <inheritdoc />
    public List<Card> DrawCards(GameSession session, int count)
    {
        var drawn = new List<Card>();
        for (var drawIndex = 0; drawIndex < count; drawIndex++)
        {
            if (session.MovementDeck.Cards.Count == 0)
            {
                if (session.MovementDiscard.Count == 0)
                {
                    break;
                }

                session.MovementDeck.Cards = session.MovementDiscard.OrderBy(_ => Random.Shared.Next()).ToList();
                session.MovementDiscard = new List<Card>();
                session.EventLog.Add("Movement deck reshuffled from discard pile.");
            }

            var nextCard = session.MovementDeck.Cards[0];
            session.MovementDeck.Cards.RemoveAt(0);
            drawn.Add(nextCard);
        }

        return drawn;
    }

    /// <inheritdoc />
    public void DiscardCard(GameSession session, Card card)
    {
        session.MovementDiscard.Add(card);
    }

    private static void AddCards(ICollection<Card> cards, string name, string description, int count, int movement)
    {
        for (var index = 0; index < count; index++)
        {
            cards.Add(new Card
            {
                Name = name,
                Description = description,
                MovementValue = movement
            });
        }
    }
}
