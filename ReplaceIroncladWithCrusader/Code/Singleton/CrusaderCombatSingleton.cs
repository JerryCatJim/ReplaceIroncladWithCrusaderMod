using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using ReplaceIroncladWithCrusader.Code.Config;
using ReplaceIroncladWithCrusader.Code.Helper;
using System.Threading.Tasks;

namespace ReplaceIroncladWithCrusader.Code.Singleton;

public class CrusaderCombatSingleton : CustomCrusaderSingletonModel
{
    public CrusaderCombatSingleton() : base(HookType.Combat)
    {

    }
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay != null && cardPlay.Player != null && CrusaderHelper.IsCardOfIronclad(cardPlay.Card) && CrusaderHelper.IsIronclad(cardPlay.Player))
        {
            string animName = CrusaderConfigLoader.GetValueAsString(cardPlay.Card.Id.Entry);
            if (!string.IsNullOrEmpty(animName))
            {
                return CreatureCmd.TriggerAnim(cardPlay.Card.Owner.Creature, "CardPlay/" + animName, 0f);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (CrusaderHelper.IsIronclad(creature) && delta > 0)
        {
            if (!CrusaderHelper.IsLowHealth(creature, 25m, 0m) && CrusaderHelper.IsLowHealth(creature, 25m, delta) && CrusaderHelper.IsInAnyIdle(null, creature))
            {
                return CreatureCmd.TriggerAnim(creature, "Revive", 0.0f);
            }
        }
        return Task.CompletedTask;
    }
}