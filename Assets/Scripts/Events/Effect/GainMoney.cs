using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMoney : CardEffect
{
    int money;

    public GainMoney(int money) { 
        this.money = money;
    }
    public override void ExecuteEffect(PlayerManager player) {
        player.CmdGainMoney(money);
    }
}
