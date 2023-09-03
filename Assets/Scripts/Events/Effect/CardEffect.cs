using UnityEngine;

public class CardEffect
{
    // Effects only influence the dealer.
    public virtual void ExecuteEffect() { }

    // Effects influence one target player.
    public virtual void ExecuteEffect(PlayerManager player) { }

    public void ExecuteEffect(PlayerManager[] players) { }

    public void ExecuteEffect(GameObject monster) { }

    public void ExecuteEffect(GameObject monster, int diceResult) { }

    public void ExecuteEffect(int diceResult) { }
}