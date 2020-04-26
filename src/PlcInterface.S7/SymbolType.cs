namespace PlcInterface.S7
{
    internal enum SymbolType
    {
        //Binary numbers
        Bool,

        //Bit strings

        Byte,
        Word,
        DWord,
        LWord, // 1500 Only

        //Integers

        SInt, //1200 & 1500 only
        Int,
        DInt,
        USInt, //1200 & 1500 only
        UInt, //1200 & 1500 only
        UDInt, //1200 & 1500 only
        LInt, //1200 & 1500 only
        ULInt, //1200 & 1500 only

        //Floating-point numbers

        Real,
        LReal, //1200 & 1500 only

        //Timers

        S5Time, //300 & 1500 only
        Time,
        LTime, //1500 only

        //Date and time

        Date,
        TimeOfDay,
        LTOD, //1500 only
        DT, //300 & 1500 only
        LDT, //1500 only
        DTL, //1200 & 1500 only

        //Character strings

        Char,
        WChar, //1200 & 1500 only
        String,
        WString, //1200 & 1500 only

        //PLC data types (UDT)
        UDT,

        //Anonymous structures
        Struct,

        //ARRAY
        Array,

        //Pointer

        Reference, //1500 only
        Variant, //1200 & 1500 only
        Pointer, //300 & 1500 only
        Any, //300 & 1500 only

        //Parameter types

        Timer, //300 & 1500 only
        Counter, //300 & 1500 only
        Block_FC, //300 & 1500 only
        Block_FB, //300 & 1500 only
        Block_DB, //300 only
        Block_SDB, //300 only
        Void,
        Parameter, //1200 & 1500 only

        //System data types

        IEC_Timer,
        IEC_LTimer, //1500 only
        IEC_SCounter, //1200 & 1500 only
        IEC_USCounter, //1200 & 1500 only
        IEC_Counter,
        IEC_UCounter, //1200 & 1500 only
        IEC_DCounter, //1200 & 1500 only
        IEC_UDCounter, //1200 & 1500 only
        IEC_LCounter, //1500 only
        IEC_ULCounter, //1500 only
        Error_Struct, //1200 & 1500 only
        NRef, //1200 & 1500 only
        CRef, //1200 & 1500 only
        VRef, //1200 & 1500 only
        SSL_Header, //300 only
        Conditions, //1200 only
        TAddr_Param, //1200 & 1500 only
        TCon_Param, //1200 & 1500 only
        HSC_Period, //1200 only

        //Hardware data types
        Remote, //1200 & 1500 only

        HW_Any, //1200 & 1500 only
        HW_Device, //1200 & 1500 only
        HW_DPMaster, //1500 only
        HW_DPSlave, //1200 & 1500 only
        HW_IO, //1200 & 1500 only
        HW_IOSystem, //1200 & 1500 only
        HW_SubModule, //1200 & 1500 only
        HW_Module, //1500 only
        HW_Interface, //1200 & 1500 only
        HW_IEPort, //1200 & 1500 only
        HW_HSC, //1200 & 1500 only
        HW_PWM, //1200 & 1500 only
        HW_PTO, //1200 & 1500 only
        Event_Any, //1200 & 1500 only
        Event_Att, //1200 & 1500 only
        Event_HWInt, //1200 & 1500 only
        OB_Any, //1200 & 1500 only
        OB_Delay, //1200 & 1500 only
        OB_TOD, //1200 & 1500 only
        OB_Cyclic, //1200 & 1500 only
        OB_Att, //1200 & 1500 only
        OB_PCycle, //1200 & 1500 only
        OB_HWInt, //1200 & 1500 only
        OB_Diag, //1200 & 1500 only
        OB_TimeError, //1200 & 1500 only
        OB_Startup, //1200 & 1500 only
        Port, //1200 & 1500 only
        RTM, //1200 & 1500 only
        PIP, //1500 only
        Conn_Any, //1200 & 1500 only
        Conn_Prg, //1200 & 1500 only
        Conn_OUC, //1200 & 1500 only
        Conn_R_ID, //1500 only
        DB_Any, //1200 & 1500 only
        DB_WWW, //1200 & 1500 only
        DB_Dyn, //1200 & 1500 only
    }
}