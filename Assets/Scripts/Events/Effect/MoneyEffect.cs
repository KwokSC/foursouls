public class MoneyEffect : ICardEffect
{
    public int money;

    public PlayerManager player;

    public void ExecuteEffect() {
        player.CmdMoneyChange(money); 
    }
}
