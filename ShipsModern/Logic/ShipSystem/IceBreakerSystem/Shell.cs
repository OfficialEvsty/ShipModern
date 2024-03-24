
namespace ShipsForm.Logic.ShipSystem.IceBreakerSystem
{
    public class Shell
    {
        private byte i_iceResisLevel;
        public byte IceResistLevel { get { return i_iceResisLevel; } }
        public Shell(byte iceR)
        {
            i_iceResisLevel = iceR;
        }
    }
}
