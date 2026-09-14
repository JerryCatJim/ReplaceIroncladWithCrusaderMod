using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using ReplaceIroncladWithCrusader.Code.Helper;
using System.Threading.Tasks;

namespace ReplaceIroncladWithCrusader.Code.Singleton;

public class CrusaderRunstateSingleton : CustomCrusaderSingletonModel
{
    public CrusaderRunstateSingleton() : base(HookType.Run)
    {

    }
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom combatRoom)
        {
            foreach (Creature creature in combatRoom.CombatState.PlayerCreatures)
            {
                if (CrusaderHelper.IsIronclad(creature))
                {
                    CrusaderHelper.SetBannerAndScriptureVisibility(creature.GetCreatureNode(), "Idle");
                }
            }
        }
        return Task.CompletedTask;
    }
}
