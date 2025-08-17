using GamePrototype.Utils;
namespace GamePrototype.Items.ConsumItems
{
    public sealed class Gold : ConsumItem
    {
        public override bool Stackable => true;
        public Gold(): base(GameConstants.Gold)
        {
        }
    }
}
