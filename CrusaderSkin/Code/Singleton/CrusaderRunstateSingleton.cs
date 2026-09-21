using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using CrusaderSkin.Code.Helper;
using System.Threading.Tasks;

namespace CrusaderSkin.Code.Singleton;

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
                    CrusaderHelper.InitCrusaderVisualAfterCombatStart(null, creature);
                }
            }
        }
        return Task.CompletedTask;
    }
}
