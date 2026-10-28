namespace Operation_Extraction.GameEngine;

using Operation_Extraction.Models;

/// <summary>
/// Provides reusable game rules and deterministic turn resolution logic for the MVP.
/// </summary>
public static class GameplayEngine
{
    public static int CalculateDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public static int CalculateRequiredMovementCards(int distance)
    {
        if (distance <= 0)
        {
            return 0;
        }

        return (int)Math.Ceiling(distance / 3d);
    }

    public static bool CanMoveWithCard(Card card, int distance)
    {
        return card.Type == CardType.Movement && card.MovementValue >= distance;
    }
}
