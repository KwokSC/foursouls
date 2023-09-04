public class MoneyEffect : ICardEffect
{
    public int money;

    public void ExecuteEffect(PlayerManager player) {
        player.CmdMoneyChange(money); 
    }
}