using Sharp7;

namespace PlcInterface.S7
{
    internal enum AreaType
    {
        DB = S7Consts.S7AreaDB,
        Inputs = S7Consts.S7AreaPE,
        Outputs = S7Consts.S7AreaPA,
        Merkers = S7Consts.S7AreaMK,
        Timers = S7Consts.S7AreaTM,
        Counters = S7Consts.S7AreaCT
    }
}