namespace Red.Graphics
{
    public enum PixelFormat
    {
        UnsignedShort = 5123, // 0x00001403
        UnsignedInt = 5125, // 0x00001405
        StencilIndex = 6401, // 0x00001901
        DepthComponent = 6402, // 0x00001902
        Red = 6403, // 0x00001903
        RedExt = 6403, // 0x00001903
        Green = 6404, // 0x00001904
        Blue = 6405, // 0x00001905
        Alpha = 6406, // 0x00001906
        Rgb = 6407, // 0x00001907
        Rgba = 6408, // 0x00001908
        AbgrExt = 32768, // 0x00008000
        CmykExt = 32780, // 0x0000800C
        CmykaExt = 32781, // 0x0000800D
        Bgr = 32992, // 0x000080E0
        BgrExt = 32992, // 0x000080E0
        Bgra = 32993, // 0x000080E1
        BgraExt = 32993, // 0x000080E1
        BgraImg = 32993, // 0x000080E1
        Ycrcb422Sgix = 33211, // 0x000081BB
        Ycrcb444Sgix = 33212, // 0x000081BC
        RG = 33319, // 0x00008227
        RGInteger = 33320, // 0x00008228
        DepthStencil = 34041, // 0x000084F9
        RedInteger = 36244, // 0x00008D94
        GreenInteger = 36245, // 0x00008D95
        BlueInteger = 36246, // 0x00008D96
        RgbInteger = 36248, // 0x00008D98
        RgbaInteger = 36249, // 0x00008D99
        BgrInteger = 36250, // 0x00008D9A
        BgraInteger = 36251, // 0x00008D9B
    }
}